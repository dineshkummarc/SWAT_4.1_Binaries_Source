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
using System.Linq;
using System.Text;
using SWAT.DataAccess;
using NUnit.Framework;

namespace SWAT.Tests.Database
{
    public abstract class BaseDatabaseTestFixture
    {
        protected WebBrowser _browser;
        protected DatabaseType _DbType;
        protected string tableName = "test1";
        protected string tableName2 = "test2";
        protected string tableName3 = "test3";

        protected static string relativePath = System.Configuration.ConfigurationManager.AppSettings["SourceCodePath"];
        protected static string localDbName1 = relativePath + @"SWAT.Tests\Database\Databases\SWATSqlDb.mdf";
        protected static string localDbName2 = relativePath + @"SWAT.Tests\Database\Databases\SWATSqlDb2.mdf";

        public BaseDatabaseTestFixture(DatabaseType type)
        {
            _DbType = type;
            _browser = new WebBrowser(BrowserType.InternetExplorer);
        }

        [TestFixtureSetUp]
        public virtual void SetUpDatabase()
        {
            if (_DbType == DatabaseType.MSSQL)
            {
                _browser.ConnectToMssql("localhost", "swat", "swat");
                _browser.SetDatabase(localDbName1);
            }
            else
            {
                _browser.ConnectToOracle("localhost", "System", "password");
            }
        }

        [TestFixtureTearDown]
        public virtual void Disconnect()
        {
            _browser.Disconnect();
        }

        [SetUp]
        public virtual void SetUp()
        {
            if (_DbType == DatabaseType.MSSQL)
            {
                _browser.SetQuery("if exists (select name from sys.tables where name = 'test1') drop table test1");
                _browser.SetQuery("if exists (select name from sys.tables where name = 'test2') drop table test2");
                _browser.SetQuery("if exists (select name from sys.tables where name = 'test3') drop table test3");
                _browser.SetQuery("if exists (select name from sys.tables where name = 'MSSqlTable') drop table MSSqlTable");
            }

            _browser.SetQuery("create table test1(name varchar(20), city varchar(20), color varchar(10))");
            _browser.SetQuery("create table test2(name varchar(20), city varchar(20), color varchar(10))");
            _browser.SetQuery("create table test3(name varchar(20), city varchar(20), color varchar(10))");

            if (_DbType == DatabaseType.MSSQL)
            {
                _browser.SetQuery("create table MSSqlTable(test bit, bdate datetime default NULL )");
            }
        }

        [TearDown]
        public virtual void TearDown()
        {
            _browser.SetQuery("Drop table test1");
            _browser.SetQuery("Drop table test2");
            _browser.SetQuery("Drop table test3");
            if (_DbType == DatabaseType.MSSQL)
            {
                _browser.SetQuery("Drop table MSSqlTable");
            }
        }
    }
}
