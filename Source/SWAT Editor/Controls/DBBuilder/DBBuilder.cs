/********************************************************************************
    This file is part of Simple Web Automation Toolkit, 
    Copyright (C) 2007 by Ultimate Software, Inc. All rights reserved.

    Simple Web Automation Toolkit is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License version 3 as published by
    the Free Software Foundation; 

    Simple Web Automation Toolkit is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

 */

/********************************************************************************/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Diagnostics;
using SWAT_Editor;
using System.Collections;
using SWAT_Editor.Controls.DBBuilder;
using SWAT_Editor.Recorder;
using System.Threading;


namespace SWAT_Editor.Controls.DBBuilder
{
    public partial class DBBuilder : UserControl
    {
        private bool localFileLoaded;
        private IDbConnection dbConnection;
        private String _type, _server, _db, _usr, _pass, _sqlQ;
        private bool blockInput = false;
        private ColumnEditor.ColumnEditor colEditor;
        private SWATWikiGenerator _writer;
        private long BoxesSelected=0;
        private bool qObjDoesntExist = false;
        private bool includeConnectTime = true;
        private int connectTimeout;
        String cellValue = "";
        private String _generatedQuery = "";
        private String _selectOutput = "";

        private bool _thereIsARowUnchecked;

        public DBBuilder()
        {
            InitializeComponent();
            LoadSettings();
            _writer = new SWATWikiGenerator();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!blockInput)
            {
                switch (keyData)
                {
                    case Keys.F5:
                        executeQuery();
                        return true;
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
            else
                return true;
        }

        #region Properties
        public IDbConnection DataConnection
        {
            get { return dbConnection; }
            set { dbConnection = value; }
        }

        public bool ShowConnectionSettings
        {
            get { return gpbConnectionSettings.Visible; }
            set
            {
                gpbConnectionSettings.Visible = value;
                //ResizeControl();
            }
        }

        public DataGridView GetGrid
        {
            get { return this.dgvDataTable; }
        }

        #endregion

        #region Form Events
        private void bckAssertCreator_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            imgWait.Visible = false;
            blockInput = false;
            _selectOutput = (String)e.Result;
            txtAssertOutput.Text = (String)e.Result;
        }

        private void bckAssertCreator_DoWork(object sender, DoWorkEventArgs e)
        {
            if (rbDelete.Checked) e.Result = generateDeleteQuery();
            else if (rbInsert.Checked) e.Result = generateInsertQuery();
            else e.Result = CreateAssertStatements();
        }

        private void btnAssertCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(txtAssertOutput.Text);
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            IncludeAllRecords(false);
            btnDeselectAll.Enabled = false;
        }
        private void executeQuery()
        {           
            SetTabOrder();
            if (PopulateDataGridView())
            {                 
                IncludeAllRecords(true);
                StoreFormValues();

                if (!dataOutputTablePanel.Visible)
                {
                    nextButtonDBBuilder_Click(this, new EventArgs());
                    lblInvalidQuery.Visible = false; //Added as part of MessageBox -> Label change. -GT
                }

                SaveSettings();
            }
            else
            {
                previousButtonDBBuilder_Click(this, new EventArgs());                
                qObjDoesntExist = true;
            }
        }

        private void btnExecuteQuery_Click(object sender, EventArgs e)
        {
            if (criticalInformationIsNotThere())
            {
                //MessageBox.Show("Cannot complete the query because at least one of the fields is missing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //Replaced message box with label to display error message. -GT
                lblInvalidQuery.Text = "Warning: Cannot complete the query because at least one of the fields is missing.";
                lblInvalidQuery.Visible = true;
                return;
            }

            executeQuery();
            rowCountLabel.Text = string.Format( "{0} Records", this.dgvDataTable.Rows.Count.ToString() );

            if (this.dgvDataTable.DataSource != null)
            {
                this.btnDeselectAll.Enabled = true;
                this.btnSelectAll.Enabled = false;
            }
            else
            {
                this.btnDeselectAll.Enabled = false;
                this.btnSelectAll.Enabled = false;
            }
            this.editColumnBtn.Enabled = true;
        }

        private void btnFetchDBs_Click(object sender, EventArgs e)
        {
            String temp = cboDatabase.Text;
            cboDatabase.Text = "master";
            cboDatabase.Items.Clear();

            if (TestConnection(true))
            {
                if (dbConnection == null)
                    CreateConnection();                

                IDbCommand dbCmd = dbConnection.CreateCommand();

                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                dbCmd.CommandText = "SELECT name FROM SYS.DATABASES";
                IDataReader dbReader = dbCmd.ExecuteReader();

                while (dbReader.Read())
                {
                    cboDatabase.Items.Add(dbReader.GetString(0));
                }

                dbReader.Close();
                dbConnection.Close();
                cboDatabase.Focus();
                SendKeys.Send("{F4}");
            }

            cboDatabase.Text = temp;
        }

        private void btnOpenSavedSQL_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofdOpenFile = new OpenFileDialog();
            ofdOpenFile.Filter = "SQL Files (*.sql)|*.sql|Text Files (*.txt)|*.txt|All Files(*.*)|*.*";
            ofdOpenFile.Title = "Open SQL Script/Query File";
            ofdOpenFile.ShowDialog();

            loadLocalFile(ofdOpenFile.FileName);
        }

        public void loadLocalFile(string fileName)
        {
            localFileLoaded = true;

            if ( !string.IsNullOrEmpty(fileName) )
            {
                StreamReader strReader = new StreamReader(fileName);
                StringBuilder strBuilder = new StringBuilder();
                String line;

                while ((line = strReader.ReadLine()) != null)
                {
                    strBuilder.AppendLine(line);
                }

                txtSQLQuery.Text = "";
                txtSQLQuery.Text = strBuilder.ToString();
            }
        }

        private void btnSaveSQL_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfdSaveFile = new SaveFileDialog();
            sfdSaveFile.Filter = "SQL Files (*.sql)|*.sql|Text Files (*.txt)|*.txt|All Files(*.*)|*.*";
            sfdSaveFile.Title = "Save SQL Query";
            sfdSaveFile.AddExtension = true;
            sfdSaveFile.ShowDialog();

            if (!sfdSaveFile.FileName.Equals(""))
            {
                StreamWriter strWriter = new StreamWriter(sfdSaveFile.FileName);
                strWriter.Write(txtSQLQuery.Text);
                strWriter.Flush();
                strWriter.Close();
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            IncludeAllRecords(true);
            btnSelectAll.Enabled = false;
            btnDeselectAll.Enabled = true;
        }

        private void btnSetModifiers_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dgr in dgvDataTable.Rows)
                dgr.Cells[1].Value = cboModifiers.Text;
        }

        private void SWATDBBuilder_Resize(object sender, EventArgs e)
        {
            //ResizeControl();
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
           TestConnection();
        }

        private void dgvDataTable_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void dgvDataTable_CellBeginEdit(object sender, EventArgs e)
        {
            cellValue = dgvDataTable.CurrentCell.FormattedValue.ToString();
        }

        private void dgvDataTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

            if (!(e.Context == (DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.Display)))
            {
                String dataType = dgvDataTable.Columns[e.ColumnIndex].ValueType.UnderlyingSystemType.ToString();
                String msg = "The value you entered is not a valid " + dataType + ". Would you like to revert to the previous value?";
                DialogResult d = MessageBox.Show(msg, "Data Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (d == DialogResult.Yes)
                    dgvDataTable.CancelEdit();
            }
        }

        private void gridContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this.gridContextMenuStrip.Items.Clear();
            if (dgvDataTable.Columns.Count == 2)
                return;

            ToolStripMenuItem all = new ToolStripMenuItem("Include All");
            ToolStripMenuItem none = new ToolStripMenuItem("Hide All");
            all.Click += new EventHandler(gridContextMenu_Click);
            none.Click += new EventHandler(gridContextMenu_Click);
            this.gridContextMenuStrip.Items.Add(all);
            this.gridContextMenuStrip.Items.Add(none);
            this.gridContextMenuStrip.Items.Add(new ToolStripSeparator());

            foreach (DataGridViewColumn c in dgvDataTable.Columns)
            {
                if (!(c.Name.Equals("includeRow") || c.Name.Equals("modifier")))
                {
                    ToolStripMenuItem newTSI = new ToolStripMenuItem();
                    newTSI.Text = c.HeaderText;
                    newTSI.Click += new EventHandler(gridContextMenu_Click);

                    if (c.Visible)
                        newTSI.Checked = true;

                    this.gridContextMenuStrip.Items.Add(newTSI);

                }
            }
        }

        private void gridContextMenu_Click(object sender, EventArgs e)
        {
            String cellHeader = ((ToolStripMenuItem)sender).Text;

            if (cellHeader.Equals("Include All"))
            {
                foreach (DataGridViewColumn c in dgvDataTable.Columns)
                {
                    if (!(c.HeaderText.Equals("Include?") || c.HeaderText.Equals("SWAT Modifier")))
                        c.Visible = true;
                }
                //lblHiddenColumnWarning.Visible = false;
                return;
            }
            else if (cellHeader.Equals("Hide All"))
            {
                foreach (DataGridViewColumn c in dgvDataTable.Columns)
                {
                    if (!(c.HeaderText.Equals("Include?") || c.HeaderText.Equals("SWAT Modifier")))
                        c.Visible = false;
                }
                //lblHiddenColumnWarning.Visible = true;
                return;
            }

            //lblHiddenColumnWarning.Visible = false;
            foreach (DataGridViewColumn c in dgvDataTable.Columns)
            {
                if (c.HeaderText.Equals(cellHeader))
                    c.Visible = !c.Visible;
                //if (!c.Visible)
                //lblHiddenColumnWarning.Visible = true;
            }
        }

        private void panels_VisibleChanged(object sender, EventArgs e)
        {
            lblUncheckedRowWarning.Visible = false;

            if (_sqlQ == null && dataOutputTablePanel.Visible)
            {
                DialogResult execute = MessageBox.Show("No queries have been run. Would you like to attempt to run \r\na query with the values on the form?","No Query Executed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (execute == DialogResult.Yes)
                {
                    btnExecuteQuery_Click(this, new EventArgs());                    
                }
                else
                {
                    previousButtonDBBuilder_Click( this, new EventArgs() );
                }
            }

            if (!CompareInternalValues() && dataOutputTablePanel.Visible)
            {
                DialogResult reExecute = MessageBox.Show("You have changed one or more values that could affect the query \r\nand therefore the assertion list. Would you like to re-run the query \r\nwith these new values?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (reExecute == DialogResult.Yes)
                    btnExecuteQuery_Click(this, new EventArgs());                
            }

            if (dataOutputTablePanel.Visible)
            {
                if (qObjDoesntExist)
                    qObjDoesntExist = false;

                blockInput = true;
                imgWait.Visible = true;
                rbSelect.Checked = true;

                refreshAssertStatements();

                checkRowUncheckedForWarningLbl();
            }
        }

        #endregion

        #region Helper Methods        

        private Panel currentPanel()
        {
            if (sqlConnectionSetUpPanel.Visible)
                return sqlConnectionSetUpPanel;
            if (dataOutputTablePanel.Visible)
                return dataOutputTablePanel;
            if (swatCodePanel.Visible)
                return swatCodePanel;
            return null;
        }
                
        private bool CompareIfEqual(String val1, String val2)
        {
            if (val1.CompareTo(val2) == 0)
                return true;
            return false;
        }

        private bool CompareInternalValues()
        {
            bool comparison = true;
            comparison &= CompareIfEqual(cboConnectionType.Text, _type);
            comparison &= CompareIfEqual(txtServer.Text, _server);
            comparison &= CompareIfEqual(cboDatabase.Text, _db);
            comparison &= CompareIfEqual(txtUsername.Text, _usr);
            comparison &= CompareIfEqual(txtPassword.Text, _pass);
            comparison &= CompareIfEqual(StripSQLComments(), _sqlQ);

            return comparison;
        }

        // Looks for at least one visible column of data -GT
        private bool HasVisibleColumn(DataGridView dgvDataTable)
        {
            for (int colIndex = 2; colIndex < dgvDataTable.ColumnCount; colIndex++)
                if (dgvDataTable.Columns[colIndex].Visible)
                    return true;
            return false;
        }

        // Looks for at least one included row of data. -GT
        private bool HasIncludedRow(DataGridView dgvDataTable)
        {
            foreach (DataGridViewRow row in dgvDataTable.Rows)
                if (Convert.ToBoolean(row.Cells[0].Value))
                    return true;
            return false;
        }

        private String CreateAssertStatements()
        {
            StringBuilder asserts = new StringBuilder();

            addImportStatements(ref asserts);
            
            _writer.WriteBeginAssertStatement(asserts, _type, _server, _db, _usr, _pass, _sqlQ, includeConnectTime, connectTimeout);

            string[] columnNames = new string[dgvDataTable.ColumnCount];

            if (btnTableView.Checked)
            {                
                // Added conditions for displaying CompareData in TableView
                // so it will not show up for 0 row/column data. -GT
                if (HasVisibleColumn(dgvDataTable) && HasIncludedRow(dgvDataTable))
                {
                    _writer.WriteBeginCompareData(asserts);

                    for (int colIndex = 0; colIndex < dgvDataTable.Columns.Count; colIndex++)
                    {
                        columnNames[colIndex] = dgvDataTable.Columns[colIndex].HeaderText;

                        if (colIndex >= 2 && dgvDataTable.Columns[colIndex].Visible)
                        {
                            _writer.WriteColumnNames(asserts, columnNames[colIndex]);
                        }
                    }

                    asserts.Append("\r\n");

                    foreach (DataGridViewRow row in dgvDataTable.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells[0].Value))
						{
							_writer.WriteColumnSepator(asserts);



                            for (int colIndex = 2; colIndex < dgvDataTable.ColumnCount; colIndex++)
                            {
                                if (dgvDataTable.Columns[colIndex].Visible)
                                {
                                    Object cellValue = row.Cells[colIndex].Value;
                                    String modifier = (String)row.Cells[1].Value;

                                    if (cellValue is DBNull)
                                        cellValue = "##Null##";
                                    else if (cellValue is String)
                                        if (cellValue.ToString().CompareTo("") == 0)
                                            cellValue = "##EmptyString##";
                                        else
                                        
                                            cellValue = ((String)cellValue).Replace("\r\n", "");
                                        
                                    cellValue = cellValue.ToString().Trim();

                                    _writer.WriteDataTableView(asserts, cellValue.ToString());
                                }
                            }
                            asserts.Append("\r\n");
                        }
                        

                    }
                    asserts.Append("|EndCompareData|\r\n"); // Moved here from WriteEndStatements -GT
                }
                _writer.WriteEndStatement(asserts, dgvDataTable.Rows.Count);
            }

            else if (btnCellView.Checked)
            {
                for (int colIndex = 0; colIndex < dgvDataTable.Columns.Count; colIndex++)
                {
                    columnNames[colIndex] = dgvDataTable.Columns[colIndex].HeaderText;
                }

                foreach (DataGridViewRow row in dgvDataTable.Rows)
                {
                    if (Convert.ToBoolean(row.Cells[0].Value))
                    {
                        for (int colIndex = 2; colIndex < dgvDataTable.ColumnCount; colIndex++)
                        {
                            if (dgvDataTable.Columns[colIndex].Visible)
                            {
                                Object cellValue = row.Cells[colIndex].Value;
                                String modifier = (String)row.Cells[1].Value;

                                if (cellValue is DBNull)

                                    cellValue = "##Null##";
                                else if (cellValue is String)
                                    if (cellValue.ToString().CompareTo("") == 0)
                                        cellValue = "##EmptyString##";
                                    else
                                        cellValue = ((String)cellValue).Replace("\r\n", "");

                                cellValue = cellValue.ToString().Trim();

                                _writer.WriteDataCellView(asserts, modifier, row.Index, columnNames[colIndex], cellValue.ToString());
                            }
                        }
                    }
                }
                _writer.WriteEndStatement(asserts, dgvDataTable.Rows.Count);
            }
            
            return asserts.ToString();
        }

        private void CreateConnection()
        {
            if (!gpbConnectionSettings.Visible && !(dbConnection == null))
                return;

            if (cboConnectionType.Text.CompareTo("") == 0)
                throw new Exception("You must select a server connection type.");

            if (cboDatabase.Text.CompareTo("") == 0)
                throw new Exception("You must specify a database to use.");

            if (txtServer.Text.CompareTo("") == 0)
                throw new Exception("You must specify a database server.");
            int connectionTimeout = SWAT_Editor.Properties.Settings.Default.ConnectionTimeout;
            
            switch (cboConnectionType.Text)
            {
                case "MSSQL": dbConnection = new SqlConnection("Server=" + txtServer.Text + ";Database= "
                     + cboDatabase.Text + ";Uid=" + txtUsername.Text + ";Pwd=" + txtPassword.Text +
                     ";trusted_connection=no" + ";Connection Timeout=" + connectionTimeout);
                break;
                case "Oracle": throw new Exception("This feature has not yet been implemented.");                 
            }
        }

        private void IncludeAllRecords(bool setAllTo)
        {
            foreach (DataGridViewRow dgr in dgvDataTable.Rows)
                dgr.Cells[0].Value = setAllTo;
        }

        private void LoadSettings()
        {
            cboConnectionType.Text = Properties.Settings.Default.dbType;
            txtServer.Text = Properties.Settings.Default.dbServer;
            cboDatabase.Text = Properties.Settings.Default.dbName;
            txtUsername.Text = Properties.Settings.Default.dbUsr;
            txtPassword.Text = Properties.Settings.Default.dbPass;

            if (!localFileLoaded)
                txtSQLQuery.Text = Properties.Settings.Default.dbQuery;
            else
                localFileLoaded = false;

            connectTimeout = SWAT_Editor.Properties.Settings.Default.ConnectionTimeout;
        }

        private bool PopulateDataGridView()
        {
            try
            {
                String sqlQuery = txtSQLQuery.Text.Trim();
                if (sqlQuery.CompareTo("") == 0)
                    throw new Exception("You must enter a query to process.");

                CreateConnection();

                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                IDbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = sqlQuery;

                IDataReader dbReader = dbCommand.ExecuteReader();

                DataTable dbTable = new DataTable();
                dbTable.Load(dbReader);
                dbReader.Close();
                dbConnection.Close();

                /* Removed to allow a query to return 0 records. -GT
                if (dbTable.Rows.Count == 0)
                    throw new Exception("No Rows were returned from your query.");
                 */

                dgvDataTable.DataSource = dbTable;

                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "SWAT", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //Replaced message box with label to display error message. -GT
                lblInvalidQuery.Text = "Error: " + e.Message;
                lblInvalidQuery.Visible = true;
                return false;
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.dbType = cboConnectionType.Text;
            Properties.Settings.Default.dbServer = txtServer.Text;
            Properties.Settings.Default.dbName = cboDatabase.Text;
            Properties.Settings.Default.dbUsr = txtUsername.Text;
            Properties.Settings.Default.dbPass = txtPassword.Text;
            Properties.Settings.Default.dbQuery = txtSQLQuery.Text;
            Properties.Settings.Default.Save();
        }

        private void StoreFormValues()
        {
            _type = cboConnectionType.Text;
            _server = txtServer.Text;
            _db = cboDatabase.Text;
            _usr = txtUsername.Text;
            _pass = txtPassword.Text;
            _sqlQ = StripSQLComments();
        }

        private String StripSQLComments()
        {
            String sql = txtSQLQuery.Text.Replace(Environment.NewLine, "#nl#");
            sql = sql.Trim() + "#nl#*/";

            //Strip single line comments
            int startSingle = -1;
            while ((startSingle = sql.IndexOf("--")) >= 0)
            {
                int newLineIndex = sql.IndexOf("#nl#", startSingle);
                sql = sql.Substring(0, startSingle) + sql.Substring(newLineIndex);
            }

            //Strip multiline comments
            int startMulti = -1;
            while ((startMulti = sql.IndexOf("/*")) >= 0)
            {
                int endIndex = sql.IndexOf("*/");
                sql = sql.Substring(0, startMulti) + sql.Substring(endIndex + 2);
            }

            while (sql.IndexOf("  ") >= 0)
                sql = sql.Replace("  ", " ");

            sql = sql.Replace("\t", "");
            sql = sql.Replace("#nl#", " ");
            sql = sql.Replace("*/", "");
            return sql;
        }

        private bool TestConnection()
        {
            return TestConnection(false);
        }

        private bool TestConnection(bool SuppressSuccess)
        {
            try
            {
                CreateConnection();
                dbConnection.Open();
                dbConnection.Close();
                SaveSettings();

                if (!SuppressSuccess)
                    MessageBox.Show("The connection was succesful.", "SWAT DB", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SWAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        private void DBBuilderWizard_Load(object sender, EventArgs e)
        {
            LoadSettings();
            SetTabOrder();
        }

        public void SetTabOrder()
        {
            if (string.IsNullOrEmpty(cboDatabase.Text))
            {
                cboConnectionType.Select();
            }

            else if (!string.IsNullOrEmpty(cboDatabase.Text))
            {
                cboDatabase.Select();
            }
        }


        private void checkRowUncheckedForWarningLbl()
        {
            if (btnTableView.Checked == true)
            {
                _thereIsARowUnchecked = false;
                
                
                
                foreach (DataGridViewRow row in dgvDataTable.Rows)
                {
                    Thread.Sleep(1); 

                    if (row.Cells[0].Value == null)
                    {
                        row.Cells[0].Value = true;
                    }
                    if (!(Convert.ToBoolean(row.Cells[0].Value)))
                    {
                        _thereIsARowUnchecked = true;
                        break;
                    }
                }

                lblUncheckedRowWarning.Visible = _thereIsARowUnchecked;
            }
            else if (btnTableView.Checked == false)
            {
                lblUncheckedRowWarning.Visible = false;
            }

        }

        private void btnTableView_CheckedChanged(object sender, EventArgs e)
        {
            refreshAssertStatements();

            checkRowUncheckedForWarningLbl();

        }

        private void chkImport_CheckedChanged(object sender, EventArgs e)
        {
            refreshAssertStatements();
        }

        private void refreshAssertStatements()
        {
            if (!bckAssertCreator.IsBusy)
            {
                blockInput = true;
                imgWait.BringToFront();
                imgWait.Visible = true;                
                bckAssertCreator.RunWorkerAsync();
            }
        }

        private String extractTableNameFromQuery()
        {
            String _sqlQuery = _sqlQ;
            int startAt = _sqlQuery.ToUpper().IndexOf("FROM") + "FROM".Length;
            _sqlQuery = _sqlQuery.Substring(startAt, _sqlQuery.Length - 1 - startAt);
            _sqlQuery = _sqlQuery.Split(' ')[1];

            return _sqlQuery;
        }

        private String generateInsertQuery()
        {
            // Added conditions to prevent generating insert queries with 0 rows/columns selected
            if ((dgvDataTable.Rows.Count == 0) || !HasVisibleColumn(dgvDataTable) || !HasIncludedRow(dgvDataTable))
            {
                _generatedQuery = "Please execute a query with at least 1 row/column of data in order to generate its INSERT query equivalent.\n";
            }
            else
            {
                _generatedQuery = "";
                StringBuilder asserts = new StringBuilder();
                addImportStatements(ref asserts);

                _writer.WriteBeginQueryStatement(asserts, _type, _server, _db, _usr, _pass, includeConnectTime, connectTimeout);

                foreach (DataGridViewRow row in dgvDataTable.Rows)
                {
                    
                    if (Convert.ToBoolean(row.Cells[0].Value))
                    {
                        String modifier = (String)row.Cells[1].Value;
                        _writer.WriteInsertQuery(asserts, modifier, extractTableNameFromQuery());
                        
                        for (int i = 2; i < dgvDataTable.ColumnCount; i++)
                        {
                          if(dgvDataTable.Columns[i].Visible)
                              _writer.WriteVisibleColumnNames(asserts, dgvDataTable.Columns[i].HeaderText);
                        }

                        asserts.Remove(asserts.Length - 1, 1);
                        asserts.Append(") VALUES (");

                        for (int colIndex = 2; colIndex < dgvDataTable.ColumnCount; colIndex++)
                        {
                            if (dgvDataTable.Columns[colIndex].Visible)
                            {
                                Object cellValue = row.Cells[colIndex].Value;


                                if (cellValue is int || cellValue is bool || cellValue is DBNull)
                                {
                                    string finalValue = string.Empty;

                                    if (cellValue is int)
                                        finalValue = cellValue.ToString();
                                    else if (cellValue is bool)
                                        finalValue = (Convert.ToBoolean(cellValue)) ? "1" : "0";
                                    else if (cellValue is DBNull)
                                        finalValue = "NULL";

                                    asserts.AppendFormat("{0}, ", finalValue);
                                }
                                else
                                {
                                    asserts.AppendFormat("'{0}', ", cellValue.ToString().Replace("'", "''").Replace("\r\n", ""));
                                }
                            }
                        }
                        asserts = asserts.Remove(asserts.Length - 2, 1);
                        _writer.WriteEndInsertQueryStatement(asserts);
                    }
                }

                _generatedQuery = asserts.ToString(); ;
            }
            return _generatedQuery;
        }

        private String generateDeleteQuery()
        {
            // Added conditions to prevent generating delete queries with 0 rows/columns selected
            if ((dgvDataTable.Rows.Count == 0) || !HasVisibleColumn(dgvDataTable) || !HasIncludedRow(dgvDataTable))
            {
                _generatedQuery = "Please execute a query with at least 1 row/column of data in order to generate its DELETE query equivalent.\n";
            }
            else
            {
                _generatedQuery = "";
                StringBuilder asserts = new StringBuilder();
                addImportStatements(ref asserts);

                _writer.WriteBeginQueryStatement(asserts, _type, _server, _db, _usr, _pass, includeConnectTime, connectTimeout);

                foreach (DataGridViewRow row in dgvDataTable.Rows)
                {

                    if (Convert.ToBoolean(row.Cells[0].Value))
                    {
                        String modifier = (String)row.Cells[1].Value;
                        _writer.WriteDeleteQuery(asserts, modifier, extractTableNameFromQuery());
                   

                        for (int colIndex = 2; colIndex < dgvDataTable.ColumnCount; colIndex++)
                        {
                            if (dgvDataTable.Columns[colIndex].Visible)
                            {
                                Object cellValue = row.Cells[colIndex].Value;

                                if (cellValue is DBNull)

                                    cellValue = "##Null##";
                                else if (cellValue is String)
                                    if (cellValue.ToString().CompareTo("") == 0)
                                        cellValue = "##EmptyString##";
                                    else
                                        cellValue = ((String)cellValue).Replace("\r\n", "");

                                if (((String)cellValue.ToString()).Contains("##Null##"))
                                    asserts.AppendFormat("ISNULL({0}) AND ", dgvDataTable.Columns[colIndex].HeaderText);
                                else if (((String)cellValue.ToString()).Contains("##EmptyString##"))
                                    asserts.AppendFormat("{0}='' AND ", dgvDataTable.Columns[colIndex].HeaderText);
                                else
                                {
                                    int result;
                                    if (int.TryParse(cellValue.ToString(), out result))
                                    {
                                        asserts.AppendFormat("{0} = {1} AND ", dgvDataTable.Columns[colIndex].HeaderText, cellValue.ToString());
                                    }
                                    else
                                    {
                                        asserts.AppendFormat("{0} = '{1}' AND ", dgvDataTable.Columns[colIndex].HeaderText, cellValue.ToString());
                                    }
                                }
                            }
                        }
                        asserts = asserts.Remove(asserts.Length - 4, 3);
                        _writer.WriteEndDeleteQueryStatement(asserts);
                    }
                }

                _generatedQuery = asserts.ToString(); ;
            }
            return _generatedQuery;
        }


        private void rbSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSelect.Checked)
            {
                btnTableView.Visible = true;
                btnCellView.Visible = true;
                refreshAssertStatements();
            }
            if (rbInsert.Checked)
            {
                btnTableView.Visible = false;
                btnCellView.Visible = false;
                refreshAssertStatements();
            }
            if (rbDelete.Checked)
            {
                btnTableView.Visible = false;
                btnCellView.Visible = false;
                refreshAssertStatements();
            }
        }

        private void addImportStatements(ref StringBuilder sb)
        {
            if (chkImport.Checked)
            {
                _writer.WriteImportStatement(sb);

                if (SWAT_Editor.Properties.Settings.Default.TestBrowserType == "ie")
                {
                    _writer.WriteInternetExplorerSWATFixture(sb);
                }
                else if (SWAT_Editor.Properties.Settings.Default.TestBrowserType == "ff")
                {
                    _writer.WriteFireFoxSWATFixture(sb);
                }
                else if (SWAT_Editor.Properties.Settings.Default.TestBrowserType == "chrome")
                {
                    _writer.WriteChromeSWATFixture(sb);
                }
                else if (SWAT_Editor.Properties.Settings.Default.TestBrowserType == "safari")
                {
                    _writer.WriteSafariSWATFixture(sb);
                }
            }
        }

        private void editColumnBtn_Click(object sender, EventArgs e)
        {
            colEditor = new ColumnEditor.ColumnEditor(this);
            colEditor.ShowDialog();
        }

        private void btnCloseSQL_Click_1(object sender, EventArgs e)
        {
            this.ParentForm.Close();
        }

        private void dgvDataTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                foreach (DataGridViewCell cell in dgvDataTable.SelectedCells)
                {
                    if (cell.ColumnIndex == 0)
                    {
                        cell.Value = !(Convert.ToBoolean(cell.Value));
                    }
                }
            }

        }

        private void dgvDataTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 0 && dgvDataTable.DataSource != null && dgvDataTable[e.ColumnIndex, e.RowIndex] != null)
            {
   					if (!(Convert.ToBoolean(dgvDataTable[e.ColumnIndex, e.RowIndex].Value)))
					{	
                        BoxesSelected--;
                        btnSelectAll.Enabled = true;
                        if (BoxesSelected == 0)
                        {
                            btnDeselectAll.Enabled = false;
                        }
                    }
                    else
                    {
                        BoxesSelected++;
                        btnDeselectAll.Enabled = true;
                        if (BoxesSelected == dgvDataTable.RowCount)
                        {
                            btnSelectAll.Enabled = false;
                        }
                    }
              }
        }

        private void dgvDataTable_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                //dgvDataTable[e.ColumnIndex, e.RowIndex].Value = !(bool)dgvDataTable[e.ColumnIndex, e.RowIndex].Value;
                dgvDataTable.CurrentCell.Value = !(Convert.ToBoolean(dgvDataTable.CurrentCell.Value));
            }
            e.Cancel = true;
        }
        
        private void nextButtonDBBuilder_Click(object sender, EventArgs e)
        {
            if (criticalInformationIsNotThere())
            {
                //MessageBox.Show("Cannot complete the query because at least one of the fields is missing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //Replaced message box with label to display error message. -GT
                lblInvalidQuery.Text = "Warning: Cannot complete the query because at least one of the fields is missing.";
                lblInvalidQuery.Visible = true;
                return;
            }

            if (sqlConnectionSetUpPanel.Visible)
            {
                sqlConnectionSetUpPanel.Visible = false;
                dataOutputTablePanel.Visible = true;

                if (qObjDoesntExist)
                    return;

                previousButtonDBBuilder.Enabled = true;
					 refreshBtn.Enabled = true;
            }
            else if (dataOutputTablePanel.Visible)
            {
                dataOutputTablePanel.Visible = false;
                swatCodePanel.Visible = true;
                nextButtonDBBuilder.Enabled = false;
					 refreshBtn.Enabled = false;
                qObjDoesntExist = false;
					 refreshAssertStatements();
            }
        }

        private void previousButtonDBBuilder_Click(object sender, EventArgs e)
        {
            if (dataOutputTablePanel.Visible)
            {
                dataOutputTablePanel.Visible = false;
                sqlConnectionSetUpPanel.Visible = true;
                previousButtonDBBuilder.Enabled = false;
					 refreshBtn.Enabled = false;

            }
				else if (swatCodePanel.Visible)
				{
					swatCodePanel.Visible = false;
					dataOutputTablePanel.Visible = true;
					nextButtonDBBuilder.Enabled = true;
					refreshBtn.Enabled = true;
				}
        }

		 private void refreshBtn_Click(object sender, EventArgs e)
		 {
			 btnExecuteQuery_Click(this, new EventArgs());
			 refreshAssertStatements(); 
		 }

        private void labelModifiersHelp_Click(object sender, EventArgs e)
        {
            Process helpPage = 
                Process.Start("IExplore.exe", "http://ulti-swat.wiki.sourceforge.net/QS_Fitnesse_CommandModifiers");

            if (helpPage == null)
                MessageBox.Show("Could not open Internet Explorer", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);                
        }

        private bool criticalInformationIsNotThere()
        {
            bool ret = false;

            if (sqlConnectionSetUpPanel.Visible)
            {
                if (string.IsNullOrEmpty(txtServer.Text) || string.IsNullOrEmpty(txtUsername.Text) ||
                    string.IsNullOrEmpty(txtSQLQuery.Text))
                    ret = true;                
            }

            return ret;
        }

        private void includeConnectTimeout_CheckedChanged(object sender, EventArgs e)
        {
            if (includeConnectTimeoutCheckBox.Checked)
                includeConnectTime = true;
            else
                includeConnectTime = false;

            chkImport_CheckedChanged(this, new EventArgs());
        }
    }
    public class InvalidDataConnectionException : Exception { }
}
