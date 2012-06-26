using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SWAT.DataAccess
{
    public class MSSql : Database
    {
        #region Variables

        

        #endregion

        #region Constructors

        public MSSql()
        {
           _connection = new SqlConnection();
           _dbType = DatabaseType.MSSQL;
           RestoreConnections();           
        }

        #endregion

        #region Database Override Methods

        public override void Connect(string serverName, string userName, string password, int connectionTimeout)
        {
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder();
            connStringBuilder.DataSource = serverName;
            connStringBuilder.UserID = userName;
            connStringBuilder.Password = password;
            connStringBuilder.ConnectTimeout = connectionTimeout;

            _connection.ConnectionString = connStringBuilder.ConnectionString;
            _connection.Open();
        }

        public override void SetDatabase(string database)
        {
            _connection.ChangeDatabase(database);
        }

        public override string GetDbDate(int format, bool removeZero)
        {
            string convertedFormat = ConvertSQLToOracleDateTimeFormat(format);

            if (convertedFormat.Equals(String.Empty))
                throw new InvalidDateFormatException(format);

            SetQuery(string.Format("SELECT CONVERT(VARCHAR, GETDATE(),{0});", format));
            return RetrieveDate(format, removeZero);
        }

        #endregion

    }
}
