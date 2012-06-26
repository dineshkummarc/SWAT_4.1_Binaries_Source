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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace SWAT.DataAccess
{
    public enum DataOperation
    {
        Delete = 1,
        Insert = 2,
        Update = Delete | Insert
    }

    public class DataBack
    {
        readonly List<string> _sqlStatements;
        readonly List<string> _sqlRemoveConstraints;
        readonly List<string> _sqlRestoreConstraints;
        public IDbConnection connection { get; set; }

        public DataBack(IDbConnection conn)
        {
            _sqlStatements = new List<string>();
            _sqlRemoveConstraints = new List<string>();
            _sqlRestoreConstraints = new List<string>();
            connection = conn;
            WriteToConsole = false;
        }

        public bool WriteToConsole { get; set; }

        public void CreateStatements(string tableName, string filter, DataOperation operation)
        {
            bool isNotView = !IsView(tableName);

            if (isNotView)
                _sqlRemoveConstraints.Add(CreateRemoveContraintsStatement(tableName));

            if ((operation & DataOperation.Delete) == DataOperation.Delete)
                _sqlStatements.Add(CreateDeleteStatement(tableName, filter));

            if ((operation & DataOperation.Insert) == DataOperation.Insert)
            {
                DataTable dt = SelectDataTable(tableName, filter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                        _sqlStatements.Add(CreateInsertStatement(row, tableName, isNotView));
                }
            }

            if (isNotView)
                _sqlRestoreConstraints.Add(CreateRestoreContraintsStatement(tableName));
        }

        public void RestoreData()
        {
            StringBuilder updateSqls = new StringBuilder();
            foreach (string sqlStr in _sqlRemoveConstraints)
            {
                updateSqls.Append(sqlStr);
                updateSqls.Append("\n");
            } 

            foreach (string sqlStr in _sqlStatements)
            {
                updateSqls.Append(sqlStr);
                updateSqls.Append(";\n");
            } 

            foreach (string sqlStr in _sqlRestoreConstraints)
            {
                updateSqls.Append(sqlStr);
                updateSqls.Append("\n");
            } 

            if (updateSqls.Length > 0)
            {
                IDbCommand dbCommand;

                if (connection is SqlConnection)
                    dbCommand = new SqlCommand(updateSqls.ToString(), (SqlConnection)connection);
                else
                {
                    dbCommand = connection.CreateCommand();
                    dbCommand.CommandText = updateSqls.ToString();
                    dbCommand.Connection = connection;
                }

                dbCommand.CommandTimeout = 120; // Waits a maximun of 2 mins to finish command
                dbCommand.ExecuteNonQuery();
            
            }
            _sqlRemoveConstraints.Clear();
            _sqlStatements.Clear();
            _sqlRestoreConstraints.Clear();
        }

        private bool IsView(string tableName)
        {
            bool isView = false;
            IDbCommand dbCommand;

            if (connection is SqlConnection)
            {
                // Get query that returns the if the table is a view or not
                dbCommand = new SqlCommand("select type from sysobjects where name = '" + tableName + "'",
                                            (SqlConnection)connection);
                dbCommand.CommandType = CommandType.Text;
            }
            else
            {   //do we need to add a connection string?
                dbCommand = connection.CreateCommand();
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = "select TABLE_TYPE from ALL_OBJECT_TABLES where TABLE_NAME = '" + tableName + "'";
                dbCommand.Connection = connection;
            }

            string resp = ((string)dbCommand.ExecuteScalar());
            if (resp != null) resp = resp.Trim();
            isView = "V".Equals(resp);

            return isView;
        }

        private string CreateRemoveContraintsStatement(string tableName)
        {
            if (connection is SqlConnection)
                return string.Format("ALTER TABLE {0} NOCHECK CONSTRAINT all", tableName);

            string oracleCmd = "begin for c in ( select 'alter table ' || table_name"
                + " || ' disable constraint ' || constraint_name as disable_constraint_name "
			    + " from user_constraints where table_name = '" + tableName + "' ) loop "
                + " execute immediate c.disable_constraint_name; end loop;";
            
            return oracleCmd;
        }

        private string CreateRestoreContraintsStatement(string tableName)
        {
            if (connection is SqlConnection)
                return string.Format("ALTER TABLE {0} CHECK CONSTRAINT all", tableName);

            string oracleCmd = "for c in ( select 'alter table ' || table_name"
                + " || ' enable constraint ' || constraint_name as enable_constraint_name "
                + " from user_constraints where table_name = '" + tableName + "' ) loop "
                + " execute immediate c.enable_constraint_name; end loop; end;";

            return oracleCmd;
        }

        private string CreateDeleteStatement(string tableName, string filter)
        {
            if (!string.IsNullOrEmpty(filter))
                filter = string.Format("Where {0}", filter);
            return string.Format("Delete {0} {1}", tableName, filter);
        }

        private string CreateInsertStatement(DataRow row, string tableName, bool isNotView)
        {
            StringBuilder insertSql = new StringBuilder("Insert Into ");
            StringBuilder insertColumns = new StringBuilder();
            StringBuilder insertValues = new StringBuilder();
            insertSql.Append(tableName);

            bool firstColumn = true;
            foreach (DataColumn column in row.Table.Columns)
            {
                if (!firstColumn)
                {
                    insertColumns.Append(", ");
                    insertValues.Append(", ");
                }
                insertColumns.Append(column.ColumnName);
                insertValues.Append(GetValueForSql(column, row));

                firstColumn = false;
            }

            insertSql.Append(" (");
            insertSql.Append(insertColumns);
            insertSql.Append(") Values(");
            insertSql.Append(insertValues);
            insertSql.Append(")");

            if (connection is SqlConnection && isNotView)
                SetIdentity(tableName, insertSql);

            return insertSql.ToString();
        }

        private void SetIdentity(string tableName, StringBuilder insertSql)
        {
            string identityProperty = string.Format("select objectproperty(object_id('{0}'), 'TableHasIdentity')", tableName);
            IDbCommand dbCommand = new SqlCommand(identityProperty, (SqlConnection)connection);

            object possiblyDBNull = dbCommand.ExecuteScalar();
            if (!(possiblyDBNull == DBNull.Value) && Convert.ToBoolean(possiblyDBNull))
            {
                insertSql.Insert(0, string.Format(" set identity_insert {0} On ", tableName));
                insertSql.Append(string.Format(" set identity_insert {0} Off ", tableName));
            }
        }

        private string GetValueForSql(DataColumn column, DataRow row)
        {
            string sqlValue;

            if (row[column].GetType() == typeof(DBNull))
                sqlValue = "NULL";
            else if (column.DataType == typeof(String) || column.DataType == typeof(DateTime) || column.DataType == typeof(Guid))
            {
                sqlValue = string.Format("'{0}'", row[column].ToString().Replace("'", "''"));
            }
            else if (column.DataType == typeof(Boolean))
            {
                sqlValue = Convert.ToByte(row[column]).ToString();
            }
            else if (column.DataType == typeof(Byte[]))
            {
                sqlValue = "0x" + BitConverter.ToString((byte[])row[column]);
                sqlValue = sqlValue.Replace("-", "");                
            }
            else
                sqlValue = row[column].ToString();

            return sqlValue.Trim();
        }

        private DataTable SelectDataTable(string tableName, string filter)
        {
            DataTable table;
            if (!string.IsNullOrEmpty(filter))
                filter = string.Format("Where {0}", filter);
            string selectSql = string.Format("Select * From {0} {1}", tableName, filter);
            try
            {
                if (connection is SqlConnection)
                {
                    using (SqlCommand sqlCommand = new SqlCommand(selectSql, (SqlConnection)connection))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                        {
                            table = new DataTable(tableName);
                            da.Fill(table);
                        }
                    }
                }
                else //Oracle connection
                {
                    using (OracleCommand oCommand = (OracleCommand)connection.CreateCommand())
                    {
                        oCommand.CommandText = selectSql;
                        oCommand.Connection = (OracleConnection)connection;
                        using (OracleDataAdapter da = new OracleDataAdapter(oCommand))
                        {
                            table = new DataTable(tableName);
                            da.Fill(table);
                        }
                    }
                }
            }
            catch (Exception)  //Table OR row name may not exist
            {
                throw new NullReferenceException("Table '" + tableName + "', with filter '" + filter + "', is not valid.");
            }
            
            return table;
        }
    }
}
