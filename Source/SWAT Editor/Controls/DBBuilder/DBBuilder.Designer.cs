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


namespace SWAT_Editor.Controls.DBBuilder
{
    partial class DBBuilder
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBBuilder));
            this.sqlConnectionSetUpPanel = new System.Windows.Forms.Panel();
            this.gbSQLQuery = new System.Windows.Forms.GroupBox();
            this.btnExecuteQuery = new System.Windows.Forms.Button();
            this.txtSQLQuery = new System.Windows.Forms.RichTextBox();
            this.btnOpenSavedSQL = new System.Windows.Forms.Button();
            this.btnSaveSQL = new System.Windows.Forms.Button();
            this.gpbConnectionSettings = new System.Windows.Forms.GroupBox();
            this.pnlInnerConnSettings = new System.Windows.Forms.Panel();
            this.cboDatabase = new System.Windows.Forms.ComboBox();
            this.btnFetchDBs = new System.Windows.Forms.Button();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblDB = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.lblServerType = new System.Windows.Forms.Label();
            this.cboConnectionType = new System.Windows.Forms.ComboBox();
            this.dataOutputTablePanel = new System.Windows.Forms.Panel();
            this.swatContainer = new System.Windows.Forms.SplitContainer();
            this.dgvDataTable = new System.Windows.Forms.DataGridView();
            this.includeRow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.modifier = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.labelModifiersHelp = new System.Windows.Forms.Label();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.rowCountLabel = new System.Windows.Forms.Label();
            this.cboModifiers = new System.Windows.Forms.ComboBox();
            this.btnSetModifiers = new System.Windows.Forms.Button();
            this.editColumnBtn = new System.Windows.Forms.Button();
            this.swatCodePanel = new System.Windows.Forms.Panel();
            this.txtAssertOutput = new SWAT_Editor.Controls.TextEditor.DocumentTextBox();
            this.btnAssertCopy = new System.Windows.Forms.Button();
            this.lblUncheckedRowWarning = new System.Windows.Forms.Label();
            this.gpbOutputType = new System.Windows.Forms.GroupBox();
            this.includeConnectTimeoutCheckBox = new System.Windows.Forms.CheckBox();
            this.btnTableView = new System.Windows.Forms.RadioButton();
            this.btnCellView = new System.Windows.Forms.RadioButton();
            this.chkImport = new System.Windows.Forms.CheckBox();
            this.groupCRUD = new System.Windows.Forms.GroupBox();
            this.rbSelect = new System.Windows.Forms.RadioButton();
            this.rbDelete = new System.Windows.Forms.RadioButton();
            this.rbInsert = new System.Windows.Forms.RadioButton();
            this.gridContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bckAssertCreator = new System.ComponentModel.BackgroundWorker();
            this.imgWait = new System.Windows.Forms.PictureBox();
            this.previousButtonDBBuilder = new System.Windows.Forms.Button();
            this.nextButtonDBBuilder = new System.Windows.Forms.Button();
            this.toolTipDBBuilderWizard = new System.Windows.Forms.ToolTip(this.components);
            this.DBBuilderContainer = new System.Windows.Forms.SplitContainer();
            this.refreshBtn = new System.Windows.Forms.Button();
            this.lblInvalidQuery = new System.Windows.Forms.Label();
            this.sqlConnectionSetUpPanel.SuspendLayout();
            this.gbSQLQuery.SuspendLayout();
            this.gpbConnectionSettings.SuspendLayout();
            this.pnlInnerConnSettings.SuspendLayout();
            this.dataOutputTablePanel.SuspendLayout();
            this.swatContainer.Panel1.SuspendLayout();
            this.swatContainer.Panel2.SuspendLayout();
            this.swatContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataTable)).BeginInit();
            this.swatCodePanel.SuspendLayout();
            this.gpbOutputType.SuspendLayout();
            this.groupCRUD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgWait)).BeginInit();
            this.DBBuilderContainer.Panel1.SuspendLayout();
            this.DBBuilderContainer.Panel2.SuspendLayout();
            this.DBBuilderContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // sqlConnectionSetUpPanel
            // 
            this.sqlConnectionSetUpPanel.AutoScroll = true;
            this.sqlConnectionSetUpPanel.AutoScrollMinSize = new System.Drawing.Size(612, 358);
            this.sqlConnectionSetUpPanel.Controls.Add(this.gbSQLQuery);
            this.sqlConnectionSetUpPanel.Controls.Add(this.gpbConnectionSettings);
            this.sqlConnectionSetUpPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlConnectionSetUpPanel.Location = new System.Drawing.Point(0, 0);
            this.sqlConnectionSetUpPanel.Name = "sqlConnectionSetUpPanel";
            this.sqlConnectionSetUpPanel.Size = new System.Drawing.Size(693, 426);
            this.sqlConnectionSetUpPanel.TabIndex = 17;
            this.sqlConnectionSetUpPanel.VisibleChanged += new System.EventHandler(this.panels_VisibleChanged);
            // 
            // gbSQLQuery
            // 
            this.gbSQLQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSQLQuery.Controls.Add(this.btnExecuteQuery);
            this.gbSQLQuery.Controls.Add(this.txtSQLQuery);
            this.gbSQLQuery.Controls.Add(this.btnOpenSavedSQL);
            this.gbSQLQuery.Controls.Add(this.btnSaveSQL);
            this.gbSQLQuery.Location = new System.Drawing.Point(3, 126);
            this.gbSQLQuery.Name = "gbSQLQuery";
            this.gbSQLQuery.Size = new System.Drawing.Size(687, 300);
            this.gbSQLQuery.TabIndex = 14;
            this.gbSQLQuery.TabStop = false;
            this.gbSQLQuery.Text = "SQL Query:";
            // 
            // btnExecuteQuery
            // 
            this.btnExecuteQuery.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnExecuteQuery.Image = ((System.Drawing.Image)(resources.GetObject("btnExecuteQuery.Image")));
            this.btnExecuteQuery.Location = new System.Drawing.Point(628, 185);
            this.btnExecuteQuery.Name = "btnExecuteQuery";
            this.btnExecuteQuery.Size = new System.Drawing.Size(40, 40);
            this.btnExecuteQuery.TabIndex = 12;
            this.toolTipDBBuilderWizard.SetToolTip(this.btnExecuteQuery, "Execute Query");
            this.btnExecuteQuery.UseVisualStyleBackColor = true;
            this.btnExecuteQuery.Click += new System.EventHandler(this.btnExecuteQuery_Click);
            // 
            // txtSQLQuery
            // 
            this.txtSQLQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSQLQuery.Location = new System.Drawing.Point(22, 32);
            this.txtSQLQuery.Name = "txtSQLQuery";
            this.txtSQLQuery.Size = new System.Drawing.Size(584, 216);
            this.txtSQLQuery.TabIndex = 11;
            this.txtSQLQuery.Text = "";
            // 
            // btnOpenSavedSQL
            // 
            this.btnOpenSavedSQL.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnOpenSavedSQL.Image = global::SWAT_Editor.Properties.Resources.document_open;
            this.btnOpenSavedSQL.Location = new System.Drawing.Point(628, 123);
            this.btnOpenSavedSQL.Name = "btnOpenSavedSQL";
            this.btnOpenSavedSQL.Size = new System.Drawing.Size(40, 40);
            this.btnOpenSavedSQL.TabIndex = 3;
            this.toolTipDBBuilderWizard.SetToolTip(this.btnOpenSavedSQL, "Open SQL Query");
            this.btnOpenSavedSQL.UseVisualStyleBackColor = true;
            this.btnOpenSavedSQL.Click += new System.EventHandler(this.btnOpenSavedSQL_Click);
            // 
            // btnSaveSQL
            // 
            this.btnSaveSQL.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSaveSQL.Image = global::SWAT_Editor.Properties.Resources.media_floppy;
            this.btnSaveSQL.Location = new System.Drawing.Point(628, 59);
            this.btnSaveSQL.Name = "btnSaveSQL";
            this.btnSaveSQL.Size = new System.Drawing.Size(40, 40);
            this.btnSaveSQL.TabIndex = 4;
            this.toolTipDBBuilderWizard.SetToolTip(this.btnSaveSQL, "Save SQL Query");
            this.btnSaveSQL.UseVisualStyleBackColor = true;
            this.btnSaveSQL.Click += new System.EventHandler(this.btnSaveSQL_Click);
            // 
            // gpbConnectionSettings
            // 
            this.gpbConnectionSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpbConnectionSettings.Controls.Add(this.pnlInnerConnSettings);
            this.gpbConnectionSettings.Location = new System.Drawing.Point(3, 3);
            this.gpbConnectionSettings.Name = "gpbConnectionSettings";
            this.gpbConnectionSettings.Padding = new System.Windows.Forms.Padding(0);
            this.gpbConnectionSettings.Size = new System.Drawing.Size(687, 117);
            this.gpbConnectionSettings.TabIndex = 1;
            this.gpbConnectionSettings.TabStop = false;
            this.gpbConnectionSettings.Text = "Database Connection:";
            // 
            // pnlInnerConnSettings
            // 
            this.pnlInnerConnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.pnlInnerConnSettings.Controls.Add(this.cboDatabase);
            this.pnlInnerConnSettings.Controls.Add(this.btnFetchDBs);
            this.pnlInnerConnSettings.Controls.Add(this.btnTestConnection);
            this.pnlInnerConnSettings.Controls.Add(this.txtPassword);
            this.pnlInnerConnSettings.Controls.Add(this.lblPassword);
            this.pnlInnerConnSettings.Controls.Add(this.txtUsername);
            this.pnlInnerConnSettings.Controls.Add(this.lblUsername);
            this.pnlInnerConnSettings.Controls.Add(this.lblDB);
            this.pnlInnerConnSettings.Controls.Add(this.txtServer);
            this.pnlInnerConnSettings.Controls.Add(this.lblServer);
            this.pnlInnerConnSettings.Controls.Add(this.lblServerType);
            this.pnlInnerConnSettings.Controls.Add(this.cboConnectionType);
            this.pnlInnerConnSettings.Location = new System.Drawing.Point(44, 16);
            this.pnlInnerConnSettings.Name = "pnlInnerConnSettings";
            this.pnlInnerConnSettings.Size = new System.Drawing.Size(597, 92);
            this.pnlInnerConnSettings.TabIndex = 0;
            // 
            // cboDatabase
            // 
            this.cboDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.cboDatabase.FormattingEnabled = true;
            this.cboDatabase.Location = new System.Drawing.Point(429, 22);
            this.cboDatabase.Name = "cboDatabase";
            this.cboDatabase.Size = new System.Drawing.Size(141, 21);
            this.cboDatabase.TabIndex = 10;
            // 
            // btnFetchDBs
            // 
            this.btnFetchDBs.Location = new System.Drawing.Point(182, 48);
            this.btnFetchDBs.Name = "btnFetchDBs";
            this.btnFetchDBs.Size = new System.Drawing.Size(124, 23);
            this.btnFetchDBs.TabIndex = 8;
            this.btnFetchDBs.Text = "Fetch Database List";
            this.btnFetchDBs.UseVisualStyleBackColor = true;
            this.btnFetchDBs.Click += new System.EventHandler(this.btnFetchDBs_Click);
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnTestConnection.Location = new System.Drawing.Point(311, 48);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(100, 23);
            this.btnTestConnection.TabIndex = 11;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.txtPassword.Location = new System.Drawing.Point(323, 22);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 7;
            // 
            // lblPassword
            // 
            this.lblPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(320, 3);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "Password:";
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.txtUsername.Location = new System.Drawing.Point(217, 22);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 20);
            this.txtUsername.TabIndex = 5;
            // 
            // lblUsername
            // 
            this.lblUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(214, 3);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(58, 13);
            this.lblUsername.TabIndex = 4;
            this.lblUsername.Text = "Username:";
            // 
            // lblDB
            // 
            this.lblDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblDB.AutoSize = true;
            this.lblDB.Location = new System.Drawing.Point(426, 3);
            this.lblDB.Name = "lblDB";
            this.lblDB.Size = new System.Drawing.Size(56, 13);
            this.lblDB.TabIndex = 9;
            this.lblDB.Text = "Database:";
            // 
            // txtServer
            // 
            this.txtServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.txtServer.Location = new System.Drawing.Point(110, 22);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(100, 20);
            this.txtServer.TabIndex = 3;
            // 
            // lblServer
            // 
            this.lblServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(107, 3);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(41, 13);
            this.lblServer.TabIndex = 2;
            this.lblServer.Text = "Server:";
            // 
            // lblServerType
            // 
            this.lblServerType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblServerType.AutoSize = true;
            this.lblServerType.Location = new System.Drawing.Point(15, 3);
            this.lblServerType.Name = "lblServerType";
            this.lblServerType.Size = new System.Drawing.Size(34, 13);
            this.lblServerType.TabIndex = 0;
            this.lblServerType.Text = "Type:";
            // 
            // cboConnectionType
            // 
            this.cboConnectionType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.cboConnectionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConnectionType.FormattingEnabled = true;
            this.cboConnectionType.Items.AddRange(new object[] {
            "MSSQL",
            "Oracle"});
            this.cboConnectionType.Location = new System.Drawing.Point(18, 21);
            this.cboConnectionType.Name = "cboConnectionType";
            this.cboConnectionType.Size = new System.Drawing.Size(82, 21);
            this.cboConnectionType.TabIndex = 1;
            // 
            // dataOutputTablePanel
            // 
            this.dataOutputTablePanel.AutoScroll = true;
            this.dataOutputTablePanel.AutoScrollMinSize = new System.Drawing.Size(717, 380);
            this.dataOutputTablePanel.Controls.Add(this.swatContainer);
            this.dataOutputTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataOutputTablePanel.Location = new System.Drawing.Point(0, 0);
            this.dataOutputTablePanel.Name = "dataOutputTablePanel";
            this.dataOutputTablePanel.Size = new System.Drawing.Size(693, 426);
            this.dataOutputTablePanel.TabIndex = 18;
            this.dataOutputTablePanel.Visible = false;
            this.dataOutputTablePanel.VisibleChanged += new System.EventHandler(this.panels_VisibleChanged);
            // 
            // swatContainer
            // 
            this.swatContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.swatContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.swatContainer.Location = new System.Drawing.Point(0, 0);
            this.swatContainer.Name = "swatContainer";
            this.swatContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // swatContainer.Panel1
            // 
            this.swatContainer.Panel1.Controls.Add(this.dgvDataTable);
            // 
            // swatContainer.Panel2
            // 
            this.swatContainer.Panel2.Controls.Add(this.btnDeselectAll);
            this.swatContainer.Panel2.Controls.Add(this.labelModifiersHelp);
            this.swatContainer.Panel2.Controls.Add(this.btnSelectAll);
            this.swatContainer.Panel2.Controls.Add(this.rowCountLabel);
            this.swatContainer.Panel2.Controls.Add(this.cboModifiers);
            this.swatContainer.Panel2.Controls.Add(this.btnSetModifiers);
            this.swatContainer.Panel2.Controls.Add(this.editColumnBtn);
            this.swatContainer.Size = new System.Drawing.Size(717, 409);
            this.swatContainer.SplitterDistance = 366;
            this.swatContainer.TabIndex = 13;
            // 
            // dgvDataTable
            // 
            this.dgvDataTable.AllowUserToAddRows = false;
            this.dgvDataTable.AllowUserToDeleteRows = false;
            this.dgvDataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDataTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.includeRow,
            this.modifier});
            this.dgvDataTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDataTable.Location = new System.Drawing.Point(0, 0);
            this.dgvDataTable.Name = "dgvDataTable";
            this.dgvDataTable.Size = new System.Drawing.Size(717, 366);
            this.dgvDataTable.TabIndex = 5;
            this.dgvDataTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDataTable_CellValueChanged);
            this.dgvDataTable.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgvDataTable_ColumnAdded);
            this.dgvDataTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDataTable_CellContentClick);
            // 
            // includeRow
            // 
            this.includeRow.Frozen = true;
            this.includeRow.HeaderText = "Include?";
            this.includeRow.Name = "includeRow";
            this.includeRow.Width = 55;
            // 
            // modifier
            // 
            this.modifier.HeaderText = "SWAT Modifier";
            this.modifier.Items.AddRange(new object[] {
            "@",
            "@@",
            "@@@",
            "?",
            "??",
            "?!",
            "??!"});
            this.modifier.Name = "modifier";
            // 
            // btnDeselectAll
            // 
            this.btnDeselectAll.Enabled = false;
            this.btnDeselectAll.Location = new System.Drawing.Point(94, 8);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(75, 23);
            this.btnDeselectAll.TabIndex = 7;
            this.btnDeselectAll.Text = "E&xclude All";
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            this.btnDeselectAll.Click += new System.EventHandler(this.btnDeselectAll_Click);
            // 
            // labelModifiersHelp
            // 
            this.labelModifiersHelp.AutoSize = true;
            this.labelModifiersHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelModifiersHelp.Image = ((System.Drawing.Image)(resources.GetObject("labelModifiersHelp.Image")));
            this.labelModifiersHelp.Location = new System.Drawing.Point(426, 8);
            this.labelModifiersHelp.MaximumSize = new System.Drawing.Size(23, 23);
            this.labelModifiersHelp.MinimumSize = new System.Drawing.Size(23, 23);
            this.labelModifiersHelp.Name = "labelModifiersHelp";
            this.labelModifiersHelp.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelModifiersHelp.Size = new System.Drawing.Size(23, 23);
            this.labelModifiersHelp.TabIndex = 12;
            this.toolTipDBBuilderWizard.SetToolTip(this.labelModifiersHelp, "Command Modifiers Help");
            this.labelModifiersHelp.Click += new System.EventHandler(this.labelModifiersHelp_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Enabled = false;
            this.btnSelectAll.Location = new System.Drawing.Point(12, 8);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 6;
            this.btnSelectAll.Text = "Include &All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // rowCountLabel
            // 
            this.rowCountLabel.AutoSize = true;
            this.rowCountLabel.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rowCountLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rowCountLabel.Location = new System.Drawing.Point(589, 11);
            this.rowCountLabel.MaximumSize = new System.Drawing.Size(130, 17);
            this.rowCountLabel.MinimumSize = new System.Drawing.Size(130, 17);
            this.rowCountLabel.Name = "rowCountLabel";
            this.rowCountLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rowCountLabel.Size = new System.Drawing.Size(130, 17);
            this.rowCountLabel.TabIndex = 11;
            this.rowCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboModifiers
            // 
            this.cboModifiers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboModifiers.FormattingEnabled = true;
            this.cboModifiers.Items.AddRange(new object[] {
            "",
            "@",
            "@@",
            "@@@",
            "?",
            "??",
            "?!",
            "??!"});
            this.cboModifiers.Location = new System.Drawing.Point(339, 10);
            this.cboModifiers.Name = "cboModifiers";
            this.cboModifiers.Size = new System.Drawing.Size(73, 21);
            this.cboModifiers.TabIndex = 8;
            // 
            // btnSetModifiers
            // 
            this.btnSetModifiers.Location = new System.Drawing.Point(462, 8);
            this.btnSetModifiers.Name = "btnSetModifiers";
            this.btnSetModifiers.Size = new System.Drawing.Size(115, 23);
            this.btnSetModifiers.TabIndex = 9;
            this.btnSetModifiers.Text = "Apply Modifier To All";
            this.btnSetModifiers.UseVisualStyleBackColor = true;
            this.btnSetModifiers.Click += new System.EventHandler(this.btnSetModifiers_Click);
            // 
            // editColumnBtn
            // 
            this.editColumnBtn.Enabled = false;
            this.editColumnBtn.Location = new System.Drawing.Point(193, 8);
            this.editColumnBtn.Name = "editColumnBtn";
            this.editColumnBtn.Size = new System.Drawing.Size(129, 23);
            this.editColumnBtn.TabIndex = 10;
            this.editColumnBtn.Text = "Add/Remove Columns";
            this.editColumnBtn.UseVisualStyleBackColor = true;
            this.editColumnBtn.Click += new System.EventHandler(this.editColumnBtn_Click);
            // 
            // swatCodePanel
            // 
            this.swatCodePanel.AutoScroll = true;
            this.swatCodePanel.AutoScrollMinSize = new System.Drawing.Size(699, 364);
            this.swatCodePanel.Controls.Add(this.txtAssertOutput);
            this.swatCodePanel.Controls.Add(this.btnAssertCopy);
            this.swatCodePanel.Controls.Add(this.lblUncheckedRowWarning);
            this.swatCodePanel.Controls.Add(this.gpbOutputType);
            this.swatCodePanel.Controls.Add(this.groupCRUD);
            this.swatCodePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.swatCodePanel.Location = new System.Drawing.Point(0, 0);
            this.swatCodePanel.Name = "swatCodePanel";
            this.swatCodePanel.Size = new System.Drawing.Size(693, 426);
            this.swatCodePanel.TabIndex = 19;
            this.swatCodePanel.Visible = false;
            this.swatCodePanel.VisibleChanged += new System.EventHandler(this.panels_VisibleChanged);
            // 
            // txtAssertOutput
            // 
            this.txtAssertOutput.AllowDrop = true;
            this.txtAssertOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAssertOutput.BackColor = System.Drawing.Color.White;
            this.txtAssertOutput.Location = new System.Drawing.Point(9, 81);
            this.txtAssertOutput.Name = "txtAssertOutput";
            this.txtAssertOutput.ReadOnly = true;
            this.txtAssertOutput.Size = new System.Drawing.Size(681, 313);
            this.txtAssertOutput.TabIndex = 10;
            this.txtAssertOutput.Text = "";
            // 
            // btnAssertCopy
            // 
            this.btnAssertCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAssertCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnAssertCopy.Image")));
            this.btnAssertCopy.Location = new System.Drawing.Point(637, 35);
            this.btnAssertCopy.Name = "btnAssertCopy";
            this.btnAssertCopy.Size = new System.Drawing.Size(40, 40);
            this.btnAssertCopy.TabIndex = 9;
            this.toolTipDBBuilderWizard.SetToolTip(this.btnAssertCopy, "Copy SWAT code to clipboard");
            this.btnAssertCopy.UseVisualStyleBackColor = true;
            this.btnAssertCopy.Click += new System.EventHandler(this.btnAssertCopy_Click);
            // 
            // lblUncheckedRowWarning
            // 
            this.lblUncheckedRowWarning.AutoSize = true;
            this.lblUncheckedRowWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUncheckedRowWarning.ForeColor = System.Drawing.Color.Red;
            this.lblUncheckedRowWarning.Location = new System.Drawing.Point(413, 49);
            this.lblUncheckedRowWarning.Name = "lblUncheckedRowWarning";
            this.lblUncheckedRowWarning.Size = new System.Drawing.Size(220, 26);
            this.lblUncheckedRowWarning.TabIndex = 8;
            this.lblUncheckedRowWarning.Text = "NOTE : Unchecked rows in \"SQL\nData\" will not appear in \"Table View\"";
            this.lblUncheckedRowWarning.Visible = false;
            // 
            // gpbOutputType
            // 
            this.gpbOutputType.Controls.Add(this.includeConnectTimeoutCheckBox);
            this.gpbOutputType.Controls.Add(this.btnTableView);
            this.gpbOutputType.Controls.Add(this.btnCellView);
            this.gpbOutputType.Controls.Add(this.chkImport);
            this.gpbOutputType.Location = new System.Drawing.Point(214, 3);
            this.gpbOutputType.Name = "gpbOutputType";
            this.gpbOutputType.Size = new System.Drawing.Size(188, 72);
            this.gpbOutputType.TabIndex = 7;
            this.gpbOutputType.TabStop = false;
            this.gpbOutputType.Text = "Layout Options";
            // 
            // includeConnectTimeoutCheckBox
            // 
            this.includeConnectTimeoutCheckBox.AutoSize = true;
            this.includeConnectTimeoutCheckBox.Checked = true;
            this.includeConnectTimeoutCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeConnectTimeoutCheckBox.Location = new System.Drawing.Point(20, 32);
            this.includeConnectTimeoutCheckBox.Name = "includeConnectTimeoutCheckBox";
            this.includeConnectTimeoutCheckBox.Size = new System.Drawing.Size(145, 17);
            this.includeConnectTimeoutCheckBox.TabIndex = 5;
            this.includeConnectTimeoutCheckBox.Text = "Include Connect Timeout";
            this.includeConnectTimeoutCheckBox.UseVisualStyleBackColor = true;
            this.includeConnectTimeoutCheckBox.CheckedChanged += new System.EventHandler(this.includeConnectTimeout_CheckedChanged);
            // 
            // btnTableView
            // 
            this.btnTableView.AutoSize = true;
            this.btnTableView.Checked = true;
            this.btnTableView.Location = new System.Drawing.Point(20, 49);
            this.btnTableView.Name = "btnTableView";
            this.btnTableView.Size = new System.Drawing.Size(78, 17);
            this.btnTableView.TabIndex = 2;
            this.btnTableView.TabStop = true;
            this.btnTableView.Text = "Table View";
            this.btnTableView.UseVisualStyleBackColor = true;
            this.btnTableView.CheckedChanged += new System.EventHandler(this.btnTableView_CheckedChanged);
            // 
            // btnCellView
            // 
            this.btnCellView.AutoSize = true;
            this.btnCellView.Location = new System.Drawing.Point(101, 49);
            this.btnCellView.Name = "btnCellView";
            this.btnCellView.Size = new System.Drawing.Size(68, 17);
            this.btnCellView.TabIndex = 3;
            this.btnCellView.Text = "Cell View";
            this.btnCellView.UseVisualStyleBackColor = true;
            // 
            // chkImport
            // 
            this.chkImport.AutoSize = true;
            this.chkImport.Checked = true;
            this.chkImport.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImport.Location = new System.Drawing.Point(20, 15);
            this.chkImport.Name = "chkImport";
            this.chkImport.Size = new System.Drawing.Size(149, 17);
            this.chkImport.TabIndex = 4;
            this.chkImport.Text = "Include Import Statements";
            this.chkImport.UseVisualStyleBackColor = true;
            this.chkImport.CheckedChanged += new System.EventHandler(this.chkImport_CheckedChanged);
            // 
            // groupCRUD
            // 
            this.groupCRUD.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.groupCRUD.Controls.Add(this.rbSelect);
            this.groupCRUD.Controls.Add(this.rbDelete);
            this.groupCRUD.Controls.Add(this.rbInsert);
            this.groupCRUD.Location = new System.Drawing.Point(9, 3);
            this.groupCRUD.Margin = new System.Windows.Forms.Padding(0);
            this.groupCRUD.Name = "groupCRUD";
            this.groupCRUD.Padding = new System.Windows.Forms.Padding(0);
            this.groupCRUD.Size = new System.Drawing.Size(188, 72);
            this.groupCRUD.TabIndex = 6;
            this.groupCRUD.TabStop = false;
            this.groupCRUD.Text = "Operation Type";
            // 
            // rbSelect
            // 
            this.rbSelect.AutoSize = true;
            this.rbSelect.Checked = true;
            this.rbSelect.Location = new System.Drawing.Point(5, 17);
            this.rbSelect.Name = "rbSelect";
            this.rbSelect.Size = new System.Drawing.Size(55, 17);
            this.rbSelect.TabIndex = 4;
            this.rbSelect.TabStop = true;
            this.rbSelect.Text = "Select";
            this.rbSelect.UseVisualStyleBackColor = true;
            this.rbSelect.CheckedChanged += new System.EventHandler(this.rbSelect_CheckedChanged);
            // 
            // rbDelete
            // 
            this.rbDelete.AutoSize = true;
            this.rbDelete.Location = new System.Drawing.Point(123, 17);
            this.rbDelete.Name = "rbDelete";
            this.rbDelete.Size = new System.Drawing.Size(56, 17);
            this.rbDelete.TabIndex = 4;
            this.rbDelete.Text = "Delete";
            this.rbDelete.UseVisualStyleBackColor = true;
            this.rbDelete.CheckedChanged += new System.EventHandler(this.rbSelect_CheckedChanged);
            // 
            // rbInsert
            // 
            this.rbInsert.AutoSize = true;
            this.rbInsert.Location = new System.Drawing.Point(66, 17);
            this.rbInsert.Name = "rbInsert";
            this.rbInsert.Size = new System.Drawing.Size(51, 17);
            this.rbInsert.TabIndex = 4;
            this.rbInsert.Text = "Insert";
            this.rbInsert.UseVisualStyleBackColor = true;
            this.rbInsert.CheckedChanged += new System.EventHandler(this.rbSelect_CheckedChanged);
            // 
            // gridContextMenuStrip
            // 
            this.gridContextMenuStrip.Name = "gridContextMenuStrip";
            this.gridContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // bckAssertCreator
            // 
            this.bckAssertCreator.WorkerReportsProgress = true;
            this.bckAssertCreator.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bckAssertCreator_DoWork);
            this.bckAssertCreator.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bckAssertCreator_RunWorkerCompleted);
            // 
            // imgWait
            // 
            this.imgWait.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.imgWait.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imgWait.Image = ((System.Drawing.Image)(resources.GetObject("imgWait.Image")));
            this.imgWait.Location = new System.Drawing.Point(302, 125);
            this.imgWait.Name = "imgWait";
            this.imgWait.Size = new System.Drawing.Size(99, 106);
            this.imgWait.TabIndex = 21;
            this.imgWait.TabStop = false;
            this.imgWait.Visible = false;
            // 
            // previousButtonDBBuilder
            // 
            this.previousButtonDBBuilder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.previousButtonDBBuilder.Enabled = false;
            this.previousButtonDBBuilder.Image = ((System.Drawing.Image)(resources.GetObject("previousButtonDBBuilder.Image")));
            this.previousButtonDBBuilder.Location = new System.Drawing.Point(604, 3);
            this.previousButtonDBBuilder.Name = "previousButtonDBBuilder";
            this.previousButtonDBBuilder.Size = new System.Drawing.Size(40, 40);
            this.previousButtonDBBuilder.TabIndex = 16;
            this.previousButtonDBBuilder.UseVisualStyleBackColor = true;
            this.previousButtonDBBuilder.Click += new System.EventHandler(this.previousButtonDBBuilder_Click);
            // 
            // nextButtonDBBuilder
            // 
            this.nextButtonDBBuilder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButtonDBBuilder.Image = ((System.Drawing.Image)(resources.GetObject("nextButtonDBBuilder.Image")));
            this.nextButtonDBBuilder.Location = new System.Drawing.Point(650, 3);
            this.nextButtonDBBuilder.Name = "nextButtonDBBuilder";
            this.nextButtonDBBuilder.Size = new System.Drawing.Size(40, 40);
            this.nextButtonDBBuilder.TabIndex = 15;
            this.nextButtonDBBuilder.UseVisualStyleBackColor = true;
            this.nextButtonDBBuilder.Click += new System.EventHandler(this.nextButtonDBBuilder_Click);
            // 
            // DBBuilderContainer
            // 
            this.DBBuilderContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DBBuilderContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.DBBuilderContainer.Location = new System.Drawing.Point(0, 0);
            this.DBBuilderContainer.Name = "DBBuilderContainer";
            this.DBBuilderContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // DBBuilderContainer.Panel1
            // 
            this.DBBuilderContainer.Panel1.Controls.Add(this.refreshBtn);
            this.DBBuilderContainer.Panel1.Controls.Add(this.previousButtonDBBuilder);
            this.DBBuilderContainer.Panel1.Controls.Add(this.nextButtonDBBuilder);
            this.DBBuilderContainer.Panel1.Controls.Add(this.lblInvalidQuery);
            // 
            // DBBuilderContainer.Panel2
            // 
            this.DBBuilderContainer.Panel2.Controls.Add(this.swatCodePanel);
            this.DBBuilderContainer.Panel2.Controls.Add(this.sqlConnectionSetUpPanel);
            this.DBBuilderContainer.Panel2.Controls.Add(this.imgWait);
            this.DBBuilderContainer.Panel2.Controls.Add(this.dataOutputTablePanel);
            this.DBBuilderContainer.Size = new System.Drawing.Size(693, 476);
            this.DBBuilderContainer.SplitterDistance = 46;
            this.DBBuilderContainer.TabIndex = 22;
            // 
            // refreshBtn
            // 
            this.refreshBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshBtn.Enabled = false;
            this.refreshBtn.Image = ((System.Drawing.Image)(resources.GetObject("refreshBtn.Image")));
            this.refreshBtn.Location = new System.Drawing.Point(546, 3);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(40, 40);
            this.refreshBtn.TabIndex = 17;
            this.refreshBtn.UseVisualStyleBackColor = true;
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // lblInvalidQuery
            // 
            this.lblInvalidQuery.AutoSize = true;
            this.lblInvalidQuery.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblInvalidQuery.ForeColor = System.Drawing.Color.Red;
            this.lblInvalidQuery.Location = new System.Drawing.Point(22, 17);
            this.lblInvalidQuery.Name = "invalidQueryLabel";
            this.lblInvalidQuery.Size = new System.Drawing.Size(117, 13);
            this.lblInvalidQuery.TabIndex = 18;
            this.lblInvalidQuery.Text = "Invalid Query Label";
            this.lblInvalidQuery.Visible = false;
            // 
            // DBBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DBBuilderContainer);
            this.Name = "DBBuilder";
            this.Size = new System.Drawing.Size(693, 476);
            this.Load += new System.EventHandler(this.DBBuilderWizard_Load);
            this.Resize += new System.EventHandler(this.SWATDBBuilder_Resize);
            this.sqlConnectionSetUpPanel.ResumeLayout(false);
            this.gbSQLQuery.ResumeLayout(false);
            this.gpbConnectionSettings.ResumeLayout(false);
            this.pnlInnerConnSettings.ResumeLayout(false);
            this.pnlInnerConnSettings.PerformLayout();
            this.dataOutputTablePanel.ResumeLayout(false);
            this.swatContainer.Panel1.ResumeLayout(false);
            this.swatContainer.Panel2.ResumeLayout(false);
            this.swatContainer.Panel2.PerformLayout();
            this.swatContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataTable)).EndInit();
            this.swatCodePanel.ResumeLayout(false);
            this.swatCodePanel.PerformLayout();
            this.gpbOutputType.ResumeLayout(false);
            this.gpbOutputType.PerformLayout();
            this.groupCRUD.ResumeLayout(false);
            this.groupCRUD.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgWait)).EndInit();
            this.DBBuilderContainer.Panel1.ResumeLayout(false);
            this.DBBuilderContainer.Panel1.PerformLayout();
            this.DBBuilderContainer.Panel2.ResumeLayout(false);
            this.DBBuilderContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button nextButtonDBBuilder;
        private System.Windows.Forms.Button previousButtonDBBuilder;
        private System.Windows.Forms.Panel sqlConnectionSetUpPanel;
        private System.Windows.Forms.GroupBox gpbConnectionSettings;
        private System.Windows.Forms.Panel pnlInnerConnSettings;
        private System.Windows.Forms.ComboBox cboDatabase;
        private System.Windows.Forms.Button btnFetchDBs;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Label lblServerType;
        private System.Windows.Forms.ComboBox cboConnectionType;
        private System.Windows.Forms.GroupBox gbSQLQuery;
        private System.Windows.Forms.Button btnExecuteQuery;
        private System.Windows.Forms.RichTextBox txtSQLQuery;
        private System.Windows.Forms.Button btnOpenSavedSQL;
        private System.Windows.Forms.Button btnSaveSQL;
        private System.Windows.Forms.Panel dataOutputTablePanel;
        private System.Windows.Forms.DataGridView dgvDataTable;
        private System.Windows.Forms.DataGridViewCheckBoxColumn includeRow;
        private System.Windows.Forms.DataGridViewComboBoxColumn modifier;
        private System.Windows.Forms.Button editColumnBtn;
        private System.Windows.Forms.Button btnSetModifiers;
        private System.Windows.Forms.ComboBox cboModifiers;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Panel swatCodePanel;
        private System.Windows.Forms.GroupBox groupCRUD;
        private System.Windows.Forms.RadioButton rbSelect;
        private System.Windows.Forms.RadioButton rbDelete;
        private System.Windows.Forms.RadioButton rbInsert;
        private System.Windows.Forms.GroupBox gpbOutputType;
        private System.Windows.Forms.RadioButton btnTableView;
        private System.Windows.Forms.RadioButton btnCellView;
        private System.Windows.Forms.CheckBox chkImport;
        private System.Windows.Forms.Label lblUncheckedRowWarning;
        private System.Windows.Forms.Button btnAssertCopy;
        private SWAT_Editor.Controls.TextEditor.DocumentTextBox txtAssertOutput;
        private System.Windows.Forms.ContextMenuStrip gridContextMenuStrip;
        private System.ComponentModel.BackgroundWorker bckAssertCreator;
        private System.Windows.Forms.PictureBox imgWait;
        private System.Windows.Forms.ToolTip toolTipDBBuilderWizard;
        private System.Windows.Forms.Label rowCountLabel;
        private System.Windows.Forms.Label labelModifiersHelp;
		 private System.Windows.Forms.SplitContainer swatContainer;
		 private System.Windows.Forms.SplitContainer DBBuilderContainer;
        private System.Windows.Forms.CheckBox includeConnectTimeoutCheckBox;
        private System.Windows.Forms.Button refreshBtn;
        private System.Windows.Forms.Label lblInvalidQuery;
    }
}
