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
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.IO;

namespace SWAT.DataAccess
{
    public abstract class Database : IDisposable
	{
		#region Variables

		protected IDbConnection _connection;
        protected DataTable result;
        protected static Dictionary<string, DataBack> backupTables = null;
		protected DatabaseType _dbType;
        private string[] _savedDbDate = null;
        private int _monthInd, _dayInd, _yearInd, _hoursInd, _minutesInd, _secondsInd;
        
		// Dictionary to convert the supported datetime formats in SQL into datetime formats in Oracle
		static protected Dictionary<int, string> formats = new Dictionary<int, string>() 
		{
			{ 0,		"mon dd yyyy hh:miAM"			},
			{ 100,		"mon dd yyyy hh:miPM"			},
			{ 101,		"MM/DD/YYYY"					},
			{ 102,		"yyyy.mm.dd"					},
			{ 103,		"dd/mm/yyyy"					},
			{ 104,		"dd.mm.yyyy"					},
			{ 105,		"dd-mm-yyyy"					},
			{ 106,		"dd mon yyyy"					},
			{ 107,		"Mon dd, yyyy"					},
			{ 108,		"hh:mi:ss"						},
			{ 9,		"mon dd yyyy hh:mi:ss:ff3AM"	},
			{ 109,		"mon dd yyyy hh:mi:ss:ff3PM"	},
			{ 110,		"mm-dd-yyyy"					},
			{ 111,		"yyyy/mm/dd"					},
			{ 112,		"yyyymmdd"						},
			{ 13,		"dd mon yyyy hh:mm:ss:ff3"		},
			{ 113,		"dd mon yyyy hh24:mi:ss:ff3"	},
			{ 114,		"hh24:mi:ss:ff3"				},
			{ 20,		"yyyy-mm-dd hh:mi:ss"			},
			{ 120,		"yyyy-mm-dd hh24:mi:ss"			},
			{ 21,		"yyyy-mm-dd hh:mi:ss:ff3"		},
			{ 121,		"yyyy-mm-dd hh24:mi:ss:ff3"		},
			// The proper format for 126 is yyyy-mm-ddThh:mm:ss:ff3,
			//	the / will be replaced with a T when called because
			//	setting a query with the T will cause an error in Oracle
			{ 126,		"yyyy-mm-dd/hh:mm:ss:ff3"		}
		};

        //RemoveZero dependencies
        //int[] validFormats = { 0, 100, 101, 102, 103, 104, 105, 106, 107, 108, 9, 109, 110, 111, 112, 113, 13, 114, 120, 121, 126, 20, 21, 26 };
        string[] formats100 = { "MMM  d yyyy h:mmtt", "MMM  d yyyy  h:mmtt", "MMM d yyyy h:mmtt", "MMM d yyyy  h:mmtt" };
        string[] formats109 = { "MMM  d yyyy h:mm:ss:ffftt", "MMM  d yyyy  h:mm:ss:ffftt", "MMM d yyyy h:mm:ss:ffftt", "MMM d yyyy  h:mm:ss:ffftt" };

		#endregion

        //#region Properties

        //public DatabaseType DBType
        //{
        //    get { return _dbType; }
        //}

        ///// <summary>
        ///// Dictionary with datetime formats for SQL/Oracle. The keys are SQL datetime format and the value
        ///// is the corresponding Oracle datetime format.
        ///// </summary>
        //public static Dictionary<int, string> SQLOracleDateTimeFormats
        //{
        //    get { return formats; }
        //}

        //#endregion


        #region Database Connection

        public void Connect(string serverName, string userName, string password)
        {
            Dispose();
            Connect(serverName, userName, password, 15);

        }

        /// <summary>
        /// Creates the connection string in order to connect to the database.
        /// Default value for MSSQL connect timeout is 15 seconds.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="connectionTimeout"></param>
        public abstract void Connect(string serverName, string userName, string password, int connectionTimeout);

        public void Disconnect()
        {
              _connection.Close();
        }

        #endregion Database Connection


        #region Assertions

        public void AssertRecordCount(int expectedNumberOfRecords)
        {
          if (expectedNumberOfRecords != result.Rows.Count)
              throw new AssertRowCountFailedException(expectedNumberOfRecords, result.Rows.Count);
        }

        public void AssertRecordValues(int row, string colName, string values)
        {
            DataTableReader reader = result.CreateDataReader();
            int col = reader.GetOrdinal(colName);
            reader.Close();

            AssertRecordValues(row, col, values);
            
        }
        
        public void AssertRecordValues(int row, int col, string values)
        {

            DataTableReader reader = result.CreateDataReader();

            for( int i = 0; i <= row; i++ )
            {
                if( !reader.Read() )
                    throw new RecordNotFoundException(row + 1, i);
            }
              
            if( col >= reader.FieldCount )
                throw new ColumnIndexOutOfBoundsException(col);

            if( reader.GetValue(col) is DateTime )
            {
                DateTime record = (DateTime)reader.GetValue(col);
                string formatted = record.ToString();
                if (!values.Contains(":"))
                    formatted = record.ToString("MM/dd/yyyy");

                if( !formatted.ToUpper().Trim().Contains( values ) )
                    throw new RecordNotFoundException(formatted, values);
            }
            else if (reader.GetValue(col) is bool)
            {
                bool myValue = false;
                bool dbValue = reader.GetBoolean(col);
                switch (values.ToUpper().Trim())
                {
                    case "0":
                    case "FALSE": myValue = false; break;
                    case "1":
                    case "TRUE": myValue = true; break;
                    default: throw new RecordNotFoundException(myValue.ToString(), dbValue.ToString());
                }

                if (dbValue != myValue)
                    throw new RecordNotFoundException(dbValue.ToString(), myValue.ToString());
            }
            else if (reader.GetValue(col) is DBNull)
            {
                if (values.ToUpper().CompareTo("##NULL##") != 0 && values.Trim().CompareTo("") != 0)
                    throw new RecordNotFoundException("null", values);
            }
            else
            {
                if (reader.GetValue(col).ToString().CompareTo(values) != 0)
                {
                    //Save original values in case needed for exceptions.
                    String originalUserValue = values;
                    String originalDBValue = reader.GetValue(col).ToString();

                    //Create more readable temporary values we can manipulate.
                    String userValue = originalUserValue;
                    String dbValue = originalDBValue;

                    //Remove carriage returns to make it easier for QA to compare strings.
                    userValue = userValue.Replace(Environment.NewLine, "");
                    dbValue = dbValue.Replace(Environment.NewLine, "");

                    //Make both values upper-case to remove case-sensitivity and trim.
                    userValue = userValue.ToUpper().Trim();
                    dbValue = dbValue.ToUpper().Trim();

                    if (dbValue.CompareTo("") == 0)
                    {
                        if (userValue.CompareTo("") != 0 && userValue.CompareTo("##EMPTYSTRING##") != 0)
                            throw new RecordNotFoundException(originalDBValue, originalUserValue);
                    }
                    else if (dbValue.CompareTo(userValue) != 0)
                    {                          
                        throw new RecordNotFoundException(originalDBValue, originalUserValue);
                    }
                }
            }

            reader.Close();
        }

        public void AssertDBRecordExistsWithTimeout(string sql, int timeout)
        {
            DateTime startTime = DateTime.Now;
            bool recordNotFound = true;

            while (DateTime.Now < startTime.AddMilliseconds(timeout))
            {
                SetQuery(sql);

                try
                {
                    if (GetRecord(0, 0) != null)
                    {
                        recordNotFound = false;
                        break;
                    }
                }
                catch (RowIndexOutOfBoundsException)
                {

                }
                System.Threading.Thread.Sleep(500);

            }
            if (recordNotFound)
            {
                throw new QueryReturnedNoResultsException(sql);
            }
        }

        #endregion Assertions

        
        #region Getters

        public string GetRecord(int row, int col)
        {
            if( row < 0 || col < 0)
                throw new IndexOutOfBoundsException();

            string record;

            DataTableReader reader = result.CreateDataReader();

            for( int i = 0; i <= row; i++)
            if( !reader.Read() )
            {
                reader.Close();
                throw new RowIndexOutOfBoundsException(row);
            }

            if( col >= reader.FieldCount )
            {
                reader.Close();
                throw new ColumnIndexOutOfBoundsException(col);
            }

            if (reader.GetFieldType(col) == typeof(Byte[]))
            {
                object val = reader.GetValue(col);
                if (val == null || val.GetType() == typeof(System.DBNull))
                    record = "NULL";
                else
                {
                    record = "0x" + BitConverter.ToString((byte[])val);
                    record = record.Replace("-", "");
                }
            }
            else
                record = reader.GetValue(col).ToString();

            reader.Close();
            return record.Trim();
        }

        public string GetRecord(int row, string colName)
        {
            if (!result.Columns.Contains(colName))
                throw new ColumnDoesNotExistException(colName);

            DataTableReader reader = result.CreateDataReader();
            int index = reader.GetOrdinal(colName);
            
            reader.Close();            

            return GetRecord(row, index);
        }

        public abstract string GetDbDate(int format, bool removeZero);

        public string GetSavedDbDateMonth()
        {
            if (_savedDbDate == null)
                throw new Exception("No saved date");
            return _savedDbDate[_monthInd];
        }

        public string GetSavedDbDateDay()
        {
            if (_savedDbDate == null)
                throw new Exception("No saved date");
            return _savedDbDate[_dayInd];
        }

        public string GetSavedDbDateYear()
        {
            if (_savedDbDate == null)
                throw new Exception("No saved date");
            return _savedDbDate[_yearInd];
        }

        public string GetSavedDbDate(string part)
        {
            string partResult;

            if (_savedDbDate == null)
                throw new Exception("No saved date");

            part = part.ToLower();

            switch (part)
            {
                case "year":
                    partResult = GetDatePart(_yearInd, part);
                    break;
                case "month":
                    partResult = GetDatePart(_monthInd, part);
                    break;
                case "day":
                    partResult = GetDatePart(_dayInd, part);
                    break;
                case "hours":
                    partResult = GetDatePart(_hoursInd, part);
                    break;
                case "minutes":
                    partResult = GetDatePart(_minutesInd, part);
                    break;
                case "seconds":
                    partResult = GetDatePart(_secondsInd, part);
                    break;
                default:
                    throw new Exception(part + " is not recognized by this command");

            }
            return partResult;
        }

        #endregion Getters


        #region Setters
       
        public void SetQuery(string SQL)
        {
            if (_connection.ConnectionTimeout == 15)
                SetQuery(SQL, 30);  // 30 is the default timeout
            else
                SetQuery(SQL, _connection.ConnectionTimeout);
        }

        public void SetQuery(string SQL, int timeout)
        {
            result = new DataTable();

            IDbCommand command = _connection.CreateCommand();
            command.CommandText = SQL;
            command.CommandTimeout = timeout;
            IDataReader reader = command.ExecuteReader();

            result.BeginLoadData();
            result.Load(reader);
            result.EndLoadData();

            reader.Close();
        }

        public abstract void SetDatabase(string database);

        public void UpdateTable(string SQL)
          {
              IDbCommand command = _connection.CreateCommand();
              if( !SQL.ToUpper().StartsWith("UPDATE") )
                  throw new IncorrectStatementTypeException("Update", SQL);

              command.CommandText = SQL;
              command.ExecuteNonQuery();
              //reader.Close();
          }

        public void InsertIntoTable(string SQL)
        {
              IDbCommand command = _connection.CreateCommand();
              if( !SQL.ToUpper().StartsWith("INSERT") )
                  throw new IncorrectStatementTypeException("Insert", SQL);

              command.CommandText = SQL;
              command.ExecuteNonQuery();
              //reader.Close();
        }

        public void DeleteFromTable(string SQL)
        {
            IDbCommand command = _connection.CreateCommand();
            if( !SQL.ToUpper().StartsWith("DELETE") )
                throw new IncorrectStatementTypeException("Delete", SQL);
            command.CommandText = SQL;
            command.ExecuteNonQuery();
        }

        public void SaveDbDate(int format, bool removeZero)
        {
            string date;

            try
            {
                date = GetDbDate(format, removeZero);
            }
            catch (InvalidDateFormatException e)
            {
                _savedDbDate = null;
                throw e;
            }

            switch (format)
            {
                case 0:
                case 100:
                    _monthInd = 0;
                    _dayInd = 1;
                    _yearInd = 2;
                    _hoursInd = 3;
                    _minutesInd = 4;
                    _secondsInd = -1;
                    break;
                case 9:
                case 109:
                    _monthInd = 0;
                    _dayInd = 1;
                    _yearInd = 2;
                    _hoursInd = 3;
                    _minutesInd = 4;
                    _secondsInd = 5;
                    break;
                case 101:
                case 107:
                case 110:
                    _monthInd = 0;
                    _dayInd = 1;
                    _yearInd = 2;
                    _hoursInd = -1;
                    _minutesInd = -1;
                    _secondsInd = -1;
                    break;
                case 103:
                case 104:
                case 105:
                case 106:
                    _dayInd = 0;
                    _monthInd = 1;
                    _yearInd = 2;
                    _hoursInd = -1;
                    _minutesInd = -1;
                    _secondsInd = -1;
                    break;
                case 13:
                case 113:
                    _dayInd = 0;
                    _monthInd = 1;
                    _yearInd = 2;
                    _hoursInd = 3;
                    _minutesInd = 4;
                    _secondsInd = 5;
                    break;
                case 102:
                case 111:
                    _yearInd = 0;
                    _monthInd = 1;
                    _dayInd = 2;
                    _hoursInd = -1;
                    _minutesInd = -1;
                    _secondsInd = -1;
                    break;
                case 20:
                case 120:
                case 21:
                case 121:
                case 126:
                    _yearInd = 0;
                    _monthInd = 1;
                    _dayInd = 2;
                    _hoursInd = 3;
                    _minutesInd = 4;
                    _secondsInd = 5;
                    break;
                case 112:
                    _yearInd = 0;
                    _monthInd = 1;
                    _dayInd = 2;
                    _hoursInd = 3;
                    _minutesInd = 4;
                    _secondsInd = 5;

                    GetDbDate(format, removeZero);

                    string dtStr = GetRecord(0, 0);
                    DateTime dt = DateTime.ParseExact(dtStr, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
                    date = String.Format("{0:yyyyMMdd}", dt);

                    _savedDbDate = new String[3];
                    _savedDbDate[0] = String.Format("{0:yyyy}", dt);
                    _savedDbDate[1] = String.Format("{0:MM}", dt);
                    _savedDbDate[2] = String.Format("{0:dd}", dt);

                    if (removeZero)
                    {
                        _savedDbDate[1] = Convert.ToInt16(_savedDbDate[1]).ToString();
                        _savedDbDate[2] = Convert.ToInt16(_savedDbDate[2]).ToString();
                    }

                    return;
                case 108:
                case 114:
                    _yearInd = -1;
                    _monthInd = -1;
                    _dayInd = -1;
                    _hoursInd = 0;
                    _minutesInd = 1;
                    _secondsInd = 2;
                    break;
            }
            //_savedDbDate = date.Split(new string[] { "/", ",", " ", ".", "-", "t", "T", ":", "AM", "PM" }, StringSplitOptions.RemoveEmptyEntries);
            _savedDbDate = date.Split(new string[] { "/", ",", " ", ".", "-", "T", ":", "AM", "PM" }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion Setters


        #region Misc Methods

        protected string RetrieveDate(int format, bool removeZero)
        {
            string date = GetRecord(0, 0);

            // Adjust format 126 into the proper format
            if (format == 126)
                date = date.Replace("/", "T");

            if (removeZero)
                return RemoveZero(removeZero, date, format);
            else
                return date;
        }

        protected void RestoreConnections()
        {
            if (backupTables == null)
                backupTables = new Dictionary<string, DataBack>();
            else
            {
                //need to update the connections in case disconnect was previously called
                foreach (KeyValuePair<string, DataBack> dbEntry in backupTables)
                    dbEntry.Value.connection = _connection;
            }
        }

        public void BackupTable(string tableName)
        {
            BackupTable(tableName, "");
        }

        public void BackupTable(string tableName, string filter)
        {
            string tablename = tableName.ToLower();
            string filter_lowercase = filter.ToLower();
            DataBack backup = new DataBack(_connection);
            backup.CreateStatements(tableName, filter, DataOperation.Update);

            string connStr = _connection.ConnectionString;
            int index1 = connStr.IndexOf("=") + 1;
            int index2 = connStr.IndexOf(";");
            string serverName = connStr.Substring(index1, index2 - index1);
            string databaseName = _connection.Database;

            string fullTableName = serverName + "_" + databaseName + "_" + tablename + filter_lowercase;
            if (backupTables.ContainsKey(fullTableName))
                backupTables.Remove(fullTableName);

            backupTables.Add(fullTableName, backup);
        }

        public void RestoreTable(string tableName)
        {
            string tablename = tableName.ToLower();
            string connStr = _connection.ConnectionString;
            int index1 = connStr.IndexOf("=") + 1;
            int index2 = connStr.IndexOf(";");
            string serverName = connStr.Substring(index1, index2 - index1);
            string databaseName = _connection.Database;

            string fullTableName = serverName + "_" + databaseName + "_" + tablename;
            if (backupTables.ContainsKey(fullTableName))
                backupTables[fullTableName].RestoreData();
            else
                throw new System.NullReferenceException(string.Format("{0} not previously backed up.", tableName));
        }

        public void RestoreTable(string tableName, string filter)
        {
            string tablename = tableName.ToLower();
            string filter_lowercase = filter.ToLower();
            string connStr = _connection.ConnectionString;
            int index1 = connStr.IndexOf("=") + 1;
            int index2 = connStr.IndexOf(";");
            string serverName = connStr.Substring(index1, index2 - index1);
            string databaseName = _connection.Database;

            string fullTableName = serverName + "_" + databaseName + "_" + tablename + filter_lowercase;
            if (backupTables.ContainsKey(fullTableName))
                backupTables[fullTableName].RestoreData();
            else
                throw new System.NullReferenceException(string.Format("Table {0} with filter {1} was not previously backed up.", tableName, filter));
        }

        public void RestoreAllTables()
        {
            Dictionary<string,DataBack>.KeyCollection tableKeys = backupTables.Keys;
            Dictionary<string, DataBack>.KeyCollection.Enumerator keyEnumerator = tableKeys.GetEnumerator();
            string connStr = _connection.ConnectionString;
            int index1 = connStr.IndexOf("=") + 1;
            int index2 = connStr.IndexOf(";");
            string serverName = connStr.Substring(index1, index2 - index1);
            string databaseName = _connection.Database;
            string connectedDataBaseAndServer = serverName + "_" + databaseName;
            string backedUpTable;
            string tableName;
            string tableDatabaseAndServer;
            int indexOfTableName;

            while (keyEnumerator.MoveNext() != false)
            {
                backedUpTable = keyEnumerator.Current;

                indexOfTableName = backedUpTable.LastIndexOf("_") + 1;
                tableName = backedUpTable.Substring(indexOfTableName);
                tableDatabaseAndServer = backedUpTable.Substring(0, indexOfTableName - 1);

                if(string.Compare(tableDatabaseAndServer, connectedDataBaseAndServer) == 0)
                    RestoreTable(tableName);
            }
        }

        public static string ConvertSQLToOracleDateTimeFormat(int format)
        {
            string result = String.Empty;

            Dictionary<int, string>.Enumerator iterator = formats.GetEnumerator();

            while (iterator.MoveNext())
            {
                if (iterator.Current.Key == format)
                    return iterator.Current.Value;
            }

            return result;
        }

        private string RemoveZero(bool removeZero, string date, int format)
        {

            //For formats 100,0,109,9 that add an extra space before single digit days
            switch (format)
            {
                case 0:
                case 100:
                    date = FormatDate(date, formats100, "MMM d yyyy h:mmtt");
                    break;
                case 109:
                case 9:
                    date = FormatDate(date, formats109, "MMM d yyyy h:mm:ss:ffftt");
                    break;
            }

            if (removeZero)
            {
                switch (format)
                {
                    case 101:
                        date = FormatDate(date, "MM/dd/yyyy", "M/d/yyyy");
                        break;
                    case 102:
                        date = FormatDate(date, "yyyy.MM.dd", "yyyy.M.d");
                        break;
                    case 103:
                        date = FormatDate(date, "dd/MM/yyyy", "d/M/yyyy");
                        break;
                    case 104:
                        date = FormatDate(date, "dd.MM.yyyy", "d.M.yyyy");
                        break;
                    case 105:
                        date = FormatDate(date, "dd-MM-yyyy", "d-M-yyyy");
                        break;
                    case 106:
                        date = FormatDate(date, "dd MMM yyyy", "d MMM yyyy");
                        break;
                    case 107:
                        date = FormatDate(date, "MMM d, yyyy", "MMM d, yyyy");
                        break;
                    case 110:
                        date = FormatDate(date, "MM-dd-yyyy", "M-d-yyyy");
                        break;
                    case 111:
                        date = FormatDate(date, "yyyy/MM/dd", "yyyy/M/d");
                        break;
                    case 112:
                        date = FormatDate(date, "yyyyMMdd", "yyyyMd");
                        break;
                    case 113:
                    case 13:
                        date = FormatDate(date, "dd MMM yyyy HH:mm:ss:fff", "d MMM yyyy HH:mm:ss:fff");
                        break;
                    case 120:
                    case 20:
                        date = FormatDate(date, "yyyy-MM-dd HH:mm:ss", "yyyy-M-d HH:mm:ss");
                        break;
                    case 121:
                    case 21:
                        date = FormatDate(date, "yyyy-MM-dd HH:mm:ss.fff", "yyyy-M-d HH:mm:ss.fff");
                        break;
                    case 126:
                        date = FormatDate(date, "yyyy-MM-ddTHH:mm:ss.fff", "yyyy-M-dTHH:mm:ss.fff");
                        break;
                    default:
                        break;
                }
            }
            return date;
        }

        private string FormatDate(string date, string dateFormatFrom, string dateFormatTo)
        {
            DateTime dt = new DateTime();
            DateTime.TryParseExact(date, dateFormatFrom, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt);
            date = String.Format("{0:" + dateFormatTo + "}", dt);
            return date;

        }
        private string FormatDate(string date, string[] dateFormatFrom, string dateFormatTo)
        {
            DateTime dt = new DateTime();
            DateTime.TryParseExact(date, dateFormatFrom, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt);
            date = String.Format("{0:" + dateFormatTo + "}", dt);
            return date;

        }

        private string GetDatePart(int index, string part)
        {
            if (index == -1)
                throw new Exception("The format used does not support " + part);

            return _savedDbDate[index];

        }

        /*public void RunStoredProcedure(string procName, List<ProcParam> parameters)
          {
              IDbCommand command = _connection.CreateCommand();
              command.CommandType = CommandType.StoredProcedure;
              command.CommandText = procName;

              foreach( ProcParam newParam in parameters)
              {
                  IDbDataParameter thisParam = command.CreateParameter();
                  thisParam.
              command.ExecuteNonQuery();
          }*/
        #endregion Misc Methods


        #region IDisposable Members

        public void Dispose()
        {
          if (_connection.State == ConnectionState.Open)
            _connection.Close();
        }

        #endregion






    }

    /*  public class ProcParam 
      {
          private string name;
          private string type;
          private string value;
          private string direction;

          public ProcParam( string myName, string myType, string myValue , string myDirection)
          {
              name = myName;
              type = myType;
              value = myValue
              direction = myDirection;
          }

      }*/
}
