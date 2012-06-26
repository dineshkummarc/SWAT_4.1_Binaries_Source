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
using System.Text.RegularExpressions;
using SWAT.DataAccess;
using NUnit.Framework;
using System.Data.SqlClient;

namespace SWAT.Tests.Database
{
    [TestFixture]
    public class MSSQLTests : DatabaseTests
    {
        public MSSQLTests()
            : base(DatabaseType.MSSQL)
        {

        }

        [Test]
        public void NumberOfDBOpenConnectionsTest()
        {
            string numberOfConnections = "";
            string numberOfConnections2 = "";

            _browser.SetQuery("SELECT COUNT(*) FROM sys.sysprocesses WHERE dbid = DB_ID('localDbName1')");
            numberOfConnections = _browser.GetDbRecord(0, 0);

            _browser.ConnectToMssql("localhost", "swat", "swat", 15);
            _browser.SetQuery("SELECT COUNT(*) FROM sys.sysprocesses WHERE dbid = DB_ID('localDBName1')");
            numberOfConnections2 = _browser.GetDbRecord(0, 0);

            Assert.AreEqual(numberOfConnections, numberOfConnections2);
        }

        [Test]
        public void ConnectToLocalDbTest()
        {
            _browser.ConnectToMssql("localhost", "swat", "swat");
            _browser.SetDatabase(localDbName1);
            _browser.SetQuery("SELECT * FROM test1");
        }

        [Test]
        public void DatabaseByteArrayInputTest()
        {
            string name = "drTest2";
            string[,] values = new string[3, 3] { { "0x01", "0x2700", "0x07A4C8B7DE" },
                                                 { "0x0201", "0x0A3C", "0x" },
                                                 { "0x3010", "NULL", "0x12" } };
            _browser.SetQuery("if exists (select name from sys.tables where name = '" + name + "') drop table " + name);
            _browser.SetQuery("create table " + name + "(bin1 varbinary(max), bin2 binary(2), bin varbinary(10))");

            //set table values
            _browser.SetQuery("Insert into " + name + " values(0x01, 0x27, 0x7a4c8b7de)");
            _browser.SetQuery("Insert into " + name + " values(0x0201, 0xA3C, 0x)");
            _browser.SetQuery("Insert into " + name + " values(0x3010, null, 0x12)");

            _browser.SetQuery("SELECT * FROM " + name + " ORDER BY bin1");
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));

            _browser.SetQuery("Drop table " + name);
        }

        [Test]
        public void AssertTableBackupIsNotFoundWithDifferentConnectionTest()
        {
            string tableName = "test1";
            string[,] values = new string[3, 3] { {"JAKE", "MIAMI", "RED"},
                                                  {"NATALY", "NAPLES", "GREEN"},
                                                  {"ROB", "WESTON", "BLUE"}};
            string[,] values2 = new string[3, 3] { {"JAKE2", "MIAMI", "RED"},
                                                   {"NATALY", "NAPLES", "GREEN2"},
                                                   {"ROB", "WESTON2", "BLUE"}};

            //set table values
            _browser.SetQuery("Insert into test1 values('JAKE', 'MIAMI', 'RED')");
            _browser.SetQuery("Insert into test1 values('ROB', 'WESTON', 'BLUE')");
            _browser.SetQuery("Insert into test1 values('NATALY', 'NAPLES', 'GREEN')");

            _browser.BackupTable(tableName);

            _browser.SetQuery("SELECT * FROM test1 ORDER BY name");

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));

            //Make changes
            _browser.SetQuery("UPDATE test1 SET name = 'JAKE2' WHERE name = 'JAKE'");
            _browser.SetQuery("UPDATE test1 SET city = 'WESTON2' WHERE city = 'WESTON'");
            _browser.SetQuery("UPDATE test1 SET color = 'GREEN2' WHERE color = 'GREEN'");

            _browser.SetQuery("SELECT * FROM test1 ORDER BY name");

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values2[row, col], _browser.GetDbRecord(row, col));

            _browser.Disconnect();

            _browser.ConnectToMssql("localhost", "swat", "swat");
            _browser.SetDatabase(localDbName2);

            _browser.SetQuery("if exists (select name from sys.tables where name = 'test1') drop table test1");
            _browser.SetQuery("create table test1(name varchar(20), city varchar(20), color varchar(10))");

            bool exceptionThrown = false;

            try
            {
                _browser.RestoreTable(tableName);
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }
            finally
            {
                _browser.SetDatabase(localDbName1);
            }

            Assert.AreEqual(true, exceptionThrown);
        }

        [Test]
        public void DataReaderErrorTest()
        {
            string name = "drTest1";
            string[] values = new string[] { "0x0FB13A", "false", "5", "NULL" };
            _browser.SetQuery("if exists (select name from sys.tables where name = '" + name + "') drop table " + name);
            _browser.SetQuery("create table " + name + "(city varbinary(max), color varchar(10), number int, test_bytes varbinary(max))"); //also tested on 'binary' values

            //set table values
            _browser.SetQuery("Insert into " + name + " values(0x0FB13A, 'false', 5, NULL)");

            _browser.BackupTable(name);

            //change values
            _browser.SetQuery("Insert into " + name + " values(0x624ab4, 'negatiff', 10, 0x0FB13A)");

            _browser.RestoreTable(name);

            _browser.SetQuery("SELECT * FROM drTest1");
            for (int col = 0; col < values.Length; col++)
                Assert.AreEqual(values[col], _browser.GetDbRecord(0, col));

            _browser.SetQuery("Drop table " + name);
        }

        [Test]
        public void AssertTableWithBooleanValuesIsRestoredTest() //for missing line of coverage
        {
            string boolTest = "boolTest1";
            _browser.SetQuery("create table " + boolTest + "(name varchar(20), bools bit, color varchar(10))");
            string[,] values = new string[3, 3] { {"JAKE", "True", "false"},
                                                  {"NATALY", "True", "lies!!"},
                                                  {"ROB", "False", "BLUE"} };

            string[,] values2 = new string[3, 3] { {"JAKE2", "True", "false"},
                                                  {"NATALY", "True", "lies!!"},
                                                  {"ROB", "True", "BLUE2"} };

            //set table values
            _browser.SetQuery("Insert into boolTest1 values('JAKE', 'True', 'false')");
            _browser.SetQuery("Insert into boolTest1 values('NATALY', 'True', 'lies!!')");
            _browser.SetQuery("Insert into boolTest1 values('ROB', 'False', 'BLUE')");

            _browser.BackupTable(boolTest);

            _browser.SetQuery("SELECT * FROM boolTest1 ORDER BY name");

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));

            //Make changes
            _browser.SetQuery("UPDATE boolTest1 SET name = 'JAKE2' WHERE name = 'JAKE'");
            _browser.SetQuery("UPDATE boolTest1 SET bools = 'True' WHERE bools = 'False'");
            _browser.SetQuery("UPDATE boolTest1 SET color = 'BLUE2' WHERE color = 'BLUE'");

            _browser.SetQuery("SELECT * FROM boolTest1 ORDER BY name");

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values2[row, col], _browser.GetDbRecord(row, col));

            _browser.RestoreTable(boolTest);

            _browser.SetQuery("SELECT * FROM boolTest1 ORDER BY name");

            //assert original table values
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));

            _browser.SetQuery("Drop table " + boolTest);
        }

        [Test]
        public void AssertTableBackupIsSavedAfterASecondConnectionTest()
        {
            string tableName = "test1";
            string[,] values = new string[3, 3] { {"JAKE", "MIAMI", "RED"},
                                                  {"NATALY", "NAPLES", "GREEN"},
                                                  {"ROB", "WESTON", "BLUE"}};
            string[,] values2 = new string[3, 3] { {"JAKE2", "MIAMI", "RED"},
                                                   {"NATALY", "NAPLES", "GREEN2"},
                                                   {"ROB", "WESTON2", "BLUE"}};

            //set table values
            _browser.InsertIntoTable("Insert into test1 values('JAKE', 'MIAMI', 'RED')");
            _browser.InsertIntoTable("Insert into test1 values('ROB', 'WESTON', 'BLUE')");
            _browser.InsertIntoTable("Insert into test1 values('NATALY', 'NAPLES', 'GREEN')");

            _browser.BackupTable(tableName);

            _browser.SetQuery("SELECT * FROM test1 ORDER BY name");

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));

            //Make changes
            _browser.UpdateTable("UPDATE test1 SET name = 'JAKE2' WHERE name = 'JAKE'");
            _browser.UpdateTable("UPDATE test1 SET city = 'WESTON2' WHERE city = 'WESTON'");
            _browser.UpdateTable("UPDATE test1 SET color = 'GREEN2' WHERE color = 'GREEN'");

            _browser.SetQuery("SELECT * FROM test1 ORDER BY name");

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values2[row, col], _browser.GetDbRecord(row, col));

            _browser.Disconnect();

            _browser.ConnectToMssql("localhost", "swat", "swat");
            _browser.SetDatabase(localDbName1);

            _browser.RestoreTable(tableName);

            _browser.SetQuery("SELECT * FROM test1 ORDER BY name");

            //assert original table values
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));
        }


        [Test]
        public void AssertDBRecordExistsWithTimeoutRecordDoesntExistTest()
        {

            bool otherException = false;
            string message = "";
            bool testPassed = false;
            try
            {
                _browser.AssertDBRecordExistsWithTimeout("Select * from test1", 5000);
            }
            catch (QueryReturnedNoResultsException)
            {
                testPassed = true;
            }
            catch (Exception e)
            {
                testPassed = true;
                otherException = true;
                message = e.ToString();
            }


            Assert.AreEqual(true, testPassed);
            Assert.AreEqual(false, otherException, message);

        }

        // Need to test booleans here because Oracle doesn't support it
        [TestCase("FALSE")]
        [TestCase("TRUE")]
        public void AssertRecordValuesBooleanTest(string value)
        {            
            _browser.InsertIntoTable(String.Format("Insert into MSSqlTable values('{0}', '20100101')", value));
            _browser.SetQuery("SELECT * FROM MSSqlTable");
            
            _browser.AssertRecordValues(value);            
        }

        [Test]
        public void QueryNonDefaultTimeoutTest()
        {
            int timeout = 60;
            int leeway = 2;
            DateTime endTime;
            //Causes the server to wait for 1.5 minutes, should time out in both cases
            String query = "WaitFor Delay '00:01:30'";

            //Checks the default timeout of 30 seconds on a Query
            DateTime startTime = DateTime.Now;
            try
            {
                _browser.SetQuery(query);
            }
            catch (System.Data.SqlClient.SqlException)
            {
                endTime = DateTime.Now;
                Assert.IsTrue(endTime.AddSeconds(leeway).CompareTo(startTime.AddSeconds(30)) >= 0
                                && startTime.AddSeconds(30 + leeway).CompareTo(endTime) >= 0);
            }


            //Connects to server specifying a new default timeout, 
            //then checks the endtime to ensure query times out appropriately
            try
            {
                _browser.ConnectToMssql("localhost", "swat", "swat", timeout);
                _browser.SetDatabase(localDbName1);

                startTime = DateTime.Now;
                _browser.SetQuery(query);
            }
            catch (System.Data.SqlClient.SqlException)
            {
                endTime = DateTime.Now;
                Assert.IsTrue(endTime.AddSeconds(leeway).CompareTo(startTime.AddSeconds(timeout)) >= 0
                                && startTime.AddSeconds(timeout + leeway).CompareTo(endTime) >= 0);
            }


            //Checks to make sure the new default query time is overridden by the newly specified timeout in query
            try
            {
                startTime = DateTime.Now;
                _browser.SetQuery(query, 30);
            }
            catch (System.Data.SqlClient.SqlException)
            {
                endTime = DateTime.Now;
                Assert.IsTrue(endTime.AddSeconds(leeway).CompareTo(startTime.AddSeconds(30)) >= 0
                                && startTime.AddSeconds(30 + leeway).CompareTo(endTime) >= 0);
            }

            //Restores original test settings
            _browser.ConnectToMssql("localhost", "swat", "swat");
            _browser.SetDatabase(localDbName1);
        }

        [Test]
        public void SQLCommandQueryReturnsQueryErrorWhenTimeoutExceedsTest()
        {
            string message = "";
            bool testPassed = false;

            try
            {
                _browser.SetQuery("WAITFOR DELAY '00:00:06' Insert into test1(name) values('steve')", 5);
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                testPassed = true;
                message = e.ToString();
            }
            catch (Exception e)
            {
                message = e.ToString();
            }

            Assert.IsTrue(testPassed, message);
        }
        
        [Test]
        public void AssertDBRecordExistsWithTimeoutRecordDoesExistTest()
        {
            _browser.SetQuery("Insert into test1(name) values('steve')");
            bool testPassed = true;
            bool otherException = false;
            string message = "";
            try
            {
                _browser.AssertDBRecordExistsWithTimeout("Select * from test1", 5000);

            }
            catch (QueryReturnedNoResultsException)
            {
                testPassed = false;
            }
            catch (Exception e)
            {
                otherException = true;
                testPassed = false;
                message = e.ToString();

            }

            Assert.AreEqual(true, testPassed);
            Assert.AreEqual(false, otherException, message);
        }

        [Test]
        public void AssertDBRecordExistsWithTimeoutRecordAddedDuringTimeoutTest()
        {
            bool otherException = false;
            string message = "";
            bool testpassed = true;
            try
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(insertTestRecord));
                t.Start();
                _browser.AssertDBRecordExistsWithTimeout("Select * from test1", 10000);
            }
            catch (QueryReturnedNoResultsException)
            {
                testpassed = false;
            }
            catch (Exception e)
            {
                testpassed = false;
                otherException = true;
                message = e.ToString();

            }

            Assert.AreEqual(true, testpassed);
            Assert.AreEqual(false, otherException, message);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void SetQueryFromFileWithGoTest(bool withTimeout)
        {
            string relativePath = System.Configuration.ConfigurationManager.AppSettings["DatabaseTestFiles"];
            string GoTest = relativePath + @"GoTest.sql";

            if(withTimeout)
                _browser.SetQuery("/file:" + GoTest, 30);
            else
                _browser.SetQuery("/file:" + GoTest);

            Assert.IsTrue(_browser.GetDbRecord().Equals("David"));
        }

        [TestCase("TRUE", "01/01/2010", "'TRUE', '01/01/2010'")]
        [TestCase("FALSE", "01/01/2010", "'FALSE', '01/01/2010'")]
        [TestCase("", "01/01/2010", "NULL, '20100101'")]
        public void AssertRecordValuesTest(string boolean, string date, string queryString)
        {
            // set up the table
            _browser.SetQuery(String.Format("Insert into MSSqlTable values({0})", queryString));

            // read from the table
            _browser.SetQuery("SELECT * FROM MSSqlTable");

            // Assertions:
            _browser.AssertRecordValues(0, 0, boolean);
            _browser.AssertRecordValues(0, 1, date);            
        }

        [Test]
        public void AssertRecordValuesTestFailsTest()
        {
            bool exceptionThrown = false;
            // set up the table
            _browser.InsertIntoTable("Insert into MSSqlTable values('', '01/01/2010')");
            _browser.InsertIntoTable("Insert into MSSqlTable values(NULL, '01/01/2010')");
            _browser.InsertIntoTable("Insert into MSSqlTable values('TRUE', '20100101')");

            // read from the table
            _browser.SetQuery("SELECT * FROM MSSqlTable");

            try
            {
                _browser.AssertRecordValues("this should break");
            }
            catch (RecordNotFoundException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            exceptionThrown = false;

            try
            {
                _browser.AssertRecordValues(0, 1, "Should be: 01/01/2010"); 
            }
            catch (RecordNotFoundException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            exceptionThrown = false;

            try
            {
                _browser.AssertRecordValues(1, 0, "Should be: NULL");
            }
            catch (RecordNotFoundException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            exceptionThrown = false;

            try
            {
                _browser.AssertRecordValues(2, 0, "FALSE");
            }
            catch (RecordNotFoundException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            
        }
    }
}
