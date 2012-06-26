using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace SWAT.DataAccess
{
    public class Oracle : Database
    {
        #region Constructors

        public Oracle()
        {
            _dbType = DatabaseType.Oracle;
            _connection = new OracleConnection();
            RestoreConnections();
        }

        #endregion

        #region Database Override Methods

        public override void Connect(string serverName, string userName, string password, int connectionTimeout)
        {
            //OracleConnectionStringBuilder connStringBuilder = new OracleConnectionStringBuilder();
            //connStringBuilder.DataSource = serverName;
            //connStringBuilder.UserID = userName;
            //connStringBuilder.Password = password;

            //_connection.ConnectionString = connStringBuilder.ConnectionString;

            _connection.ConnectionString = String.Format("User Id={0}; Password={1}; Data Source={2}; Connection Timeout={3};",
                userName, password, serverName, connectionTimeout);
            
            _connection.Open();
            
        }
        
        public override void SetDatabase(string database)
        {
            throw new UnsupportedCommandException("SetDatabase", "Oracle");
        }

        public override string GetDbDate(int format, bool removeZero)
        {
            string convertedFormat = ConvertSQLToOracleDateTimeFormat(format);

            if (convertedFormat.Equals(String.Empty))
                throw new InvalidDateFormatException(format);

            SetQuery("SELECT TO_CHAR(SYSTIMESTAMP, '" + convertedFormat + "') FROM DUAL");

            return RetrieveDate(format, removeZero);
        }

        #endregion
    }
}
