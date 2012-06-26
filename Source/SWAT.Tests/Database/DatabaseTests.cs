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
using NUnit.Framework;
using SWAT;
using SWAT.DataAccess;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.IO;
using SWAT.Reflection;

namespace SWAT.Tests.Database
{
    [Category("Database")]
    public abstract class DatabaseTests : BaseDatabaseTestFixture
    { 
        public DatabaseTests(DatabaseType type) : base(type)
        {
        }

        [Test]
        public void AssertOptionalSetIdentityParameterRestoreTableTest()
        {
            if (_DbType == DatabaseType.MSSQL)
            {
                _browser.SetQuery("ALTER TABLE test1 ADD PKey int IDENTITY");

                _browser.InsertIntoTable("Insert into test1 values('REBECCA', 'NICEVILLE', 'PURPLE')");
                _browser.InsertIntoTable("Insert into test1 values('DEYERLE', 'DESTIN', 'ORANGLE')");
                _browser.InsertIntoTable("Insert into test1 values('ULTIMATE', 'WESTON', 'MAROON')");
                _browser.BackupTable("TeSt1");
                _browser.RestoreTable("TEST1");

                _browser.SetQuery("SELECT * FROM test1");
                Assert.AreEqual("1", _browser.GetDbRecord(0, 3));
                Assert.AreEqual("2", _browser.GetDbRecord(1, 3));
                Assert.AreEqual("3", _browser.GetDbRecord(2, 3));
            }
        }

        [Test]
        public void AssertBackUpRestoreTableCommandsCaseInsensitiveTest()
        {
            string[,] values = new string[1, 3] { {"BOB", "DESTIN", "BLUE"} };
            string[,] values2 = new string[1, 3] { { "BOB", "DESTIN", "GREEN"} };

            _browser.InsertIntoTable("Insert into test1 values('BOB', 'DESTIN', 'BLUE')");

            _browser.BackupTable("TeSt1");

            _browser.SetQuery("SELECT * FROM test1");

            for (int row = 0; row < 1; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));

            _browser.UpdateTable("UPDATE test1 SET color = 'GREEN' WHERE color = 'BLUE'");

            _browser.SetQuery("SELECT * FROM test1");

            for (int row = 0; row < 1; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values2[row, col], _browser.GetDbRecord(row, col));

            _browser.RestoreTable("TEST1");

            _browser.SetQuery("SELECT * FROM test1");

            //assert original table values
            for (int row = 0; row < 1; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));
        }

        [Test]
        public void AssertFakeTableNotFoundExceptionTest()
        {
            bool exceptionThrown = false;
            string msg = "";
            string expMsg = "System.NullReferenceException: Table 'nonexistingTable', with filter '', is not valid.";
            int msgLength = expMsg.Length;

            try
            {
                _browser.BackupTable("nonexistingTable");
            }
            catch (NullReferenceException e)
            {
                exceptionThrown = true;
                msg = e.ToString();
                msg = msg.Substring(0, msgLength);
            }

            Assert.AreEqual(true, exceptionThrown);
            Assert.AreEqual(expMsg, msg);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void AssertDataTableIsRestoredTest(bool usePrimaryKey)
        {
            if (usePrimaryKey)
            {
                if (_DbType == DatabaseType.Oracle)
                {
                    Assert.Ignore("Oracle does not use primary keys as defined in MSSql");
                }
                else if (_DbType == DatabaseType.MSSQL)
                {
                    _browser.SetQuery("ALTER TABLE test1 ADD PKey int PRIMARY KEY IDENTITY");
                }
            }

            string[,] values = new string[3, 3] { {"JAKE", "MIAMI", "RED"},
                                                  {"", "", ""},
                                                  {"ROB", "WESTON", "BLUE"}};
            string[,] values2 = new string[3, 3] { {"JAKE2", "MIAMI", "RED"},
                                                   {"", "", ""},
                                                   {"ROB", "WESTON2", "BLUE2"}};

            //set table values
            _browser.InsertIntoTable("Insert into test1 values('JAKE', 'MIAMI', 'RED')");
            _browser.InsertIntoTable("Insert into test1 values(NULL, NULL, NULL)");
            _browser.InsertIntoTable("Insert into test1 values('ROB', 'WESTON', 'BLUE')");            

            _browser.BackupTable(tableName);

            _browser.SetQuery("SELECT * FROM test1");

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));

            //Make changes
            _browser.UpdateTable("UPDATE test1 SET name = 'JAKE2' WHERE name = 'JAKE'");
            _browser.UpdateTable("UPDATE test1 SET city = 'WESTON2' WHERE city = 'WESTON'");
            _browser.UpdateTable("UPDATE test1 SET color = 'BLUE2' WHERE color = 'BLUE'");

            _browser.SetQuery("SELECT * FROM test1");

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values2[row, col], _browser.GetDbRecord(row, col));

            _browser.RestoreTable(tableName);

            _browser.SetQuery("SELECT * FROM test1");

            //assert original table values
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));
        }

        [Test]
        public void AssertMultipleTablesAreRestoredTest()
        {
            //string[] tableNames = new string[3] { "test1", "test2", "test3" };
            string[] tableNames = new string[3] { tableName, tableName2, tableName3 };

            string[,] values = new string[3, 3] { {"EDDIE", "", "GREEN"},
                                                  {"JAKE", "", "RED"},
                                                  {"ROB", "WESTON", "BLUE"}};
            string[,] values2 = new string[3, 3] { {"EDDIE", "", "GREEN2"},
                                                   {"JAKE2", "", "RED"},
                                                   {"ROB", "WESTON2", "BLUE"}};

            //insert new values into the 3 tables
            foreach (string table in tableNames)
            {
                _browser.InsertIntoTable(string.Format("Insert into {0} values('EDDIE', NULL, 'GREEN')", table));
                _browser.InsertIntoTable(string.Format("Insert into {0} values('JAKE', '', 'RED')", table));
                _browser.InsertIntoTable(string.Format("Insert into {0} values('ROB', 'WESTON', 'BLUE')", table));

                _browser.BackupTable(table);
            }

            //verify the data matches
            foreach (string table in tableNames)
            {
                _browser.SetQuery(string.Format("SELECT * FROM {0}", table));

                for (int row = 0; row < 3; row++)
                    for (int col = 0; col < 3; col++)
                        Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));
            }

            //change the vaues for the 3 tables
            foreach (string table in tableNames)
            {
                _browser.UpdateTable(string.Format("UPDATE {0} SET name = 'JAKE2' WHERE name = 'JAKE'", table));
                _browser.UpdateTable(string.Format("UPDATE {0} SET city = 'WESTON2' WHERE city = 'WESTON'", table));
                _browser.UpdateTable(string.Format("UPDATE {0} SET color = 'GREEN2' WHERE color = 'GREEN'", table));
            }

            //verify the data was changed
            foreach (string table in tableNames)
            {
                _browser.SetQuery(string.Format("SELECT * FROM {0} ORDER BY name", table));

                for (int row = 0; row < 3; row++)
                    for (int col = 0; col < 3; col++)
                        Assert.AreEqual(values2[row, col], _browser.GetDbRecord(row, col));
            }

            //setdatabase is not supported by oracle
            if (_DbType != DatabaseType.Oracle)
            {
                //change database 
                _browser.SetDatabase(localDbName2);

                //attempt restore all
                _browser.RestoreAllTables();

                //change database back to original
                _browser.SetDatabase(localDbName1);


                //verify the data was not restored from new database
                foreach (string table in tableNames)
                {
                    _browser.SetQuery(string.Format("SELECT * FROM {0} ORDER BY name", table));

                    for (int row = 0; row < 3; row++)
                        for (int col = 0; col < 3; col++)
                            Assert.AreEqual(values2[row, col], _browser.GetDbRecord(row, col));
                }
            }

            //attempt restore all again
            _browser.RestoreAllTables();

            //verify the original data was restored on all tables
            foreach (string table in tableNames)
            {
                _browser.SetQuery(string.Format("SELECT * FROM {0} ORDER BY name", table));

                for (int row = 0; row < 3; row++)
                    for (int col = 0; col < 3; col++)
                        Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));
            }
        }
        
        [Test]
        public void AssertDataWithCorrectFilterIsRestoredTest()
        {
            string tableName = "test1";
            string filter = "name = 'ROB' OR color = 'RED'";  //chooses those rows
            string filter2 = "name = 'ROB'";
            string[,] values = new string[3, 3] { {"JAKE", "", "RED"},
                                                  {"EDDIE", "", "GREEN"},
                                                  {"ROB", "WESTON", "BLUE"}};
            string[,] values2 = new string[3, 3] { {"JAKE2", "", "RED"},
                                                   {"EDDIE", "", "GREEN2"},
                                                   {"ROB", "WESTON2", "BLUE"}};
            string[,] values3 = new string[3, 3] { {"JAKE2", "", "RED"},
                                                   {"EDDIE", "", "GREEN2"},
                                                   {"ROB", "WESTON", "BLUE"}};

            //set table values
            _browser.InsertIntoTable("Insert into test1 values('JAKE', '', 'RED')");
            _browser.InsertIntoTable("Insert into test1 values('EDDIE', NULL, 'GREEN')");
            _browser.InsertIntoTable("Insert into test1 values('ROB', 'WESTON', 'BLUE')");            

            _browser.BackupTable(tableName, filter2);
            _browser.BackupTable(tableName, filter);

            _browser.SetQuery("SELECT * FROM test1");

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values[row, col], _browser.GetDbRecord(row, col));

            //Make changes
            _browser.UpdateTable("UPDATE test1 SET name = 'JAKE2' WHERE name = 'JAKE'");
            _browser.UpdateTable("UPDATE test1 SET city = 'WESTON2' WHERE city = 'WESTON'");
            _browser.UpdateTable("UPDATE test1 SET color = 'GREEN2' WHERE color = 'GREEN'");

            _browser.SetQuery("SELECT * FROM test1");

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values2[row, col], _browser.GetDbRecord(row, col));

            _browser.RestoreTable(tableName, filter2);

            _browser.SetQuery("SELECT * FROM test1");

            //assert original table values
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values3[row, col], _browser.GetDbRecord(row, col));
        }
        
        [Test]
        public void AssertDataTableWithFilterIsRestoredTest()
        {
            string filter = "name = 'ROB' OR color = 'RED'";  //"WHERE" is added in DataBack
            string[,] values = new string[3, 3] { {"JAKE", "MIAMI", "RED"},
                                                  {"NATALY", "NAPLES", "GREEN"},
                                                  {"ROB", "WESTON", "BLUE"}};
            string[,] values2 = new string[3, 3] { {"JAKE2", "MIAMI", "RED"},
                                                   {"NATALY", "NAPLES", "GREEN2"},
                                                   {"ROB", "WESTON2", "BLUE"}};
            string[,] values3 = new string[3, 3] { {"JAKE", "MIAMI", "RED"},
                                                   {"NATALY", "NAPLES", "GREEN2"},
                                                   {"ROB", "WESTON", "BLUE"}};

            //set table values
            _browser.SetQuery("Insert into test1 values('JAKE', 'MIAMI', 'RED')");
            _browser.SetQuery("Insert into test1 values('ROB', 'WESTON', 'BLUE')");
            _browser.SetQuery("Insert into test1 values('NATALY', 'NAPLES', 'GREEN')");

            _browser.BackupTable(tableName, filter);

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

            _browser.RestoreTable(tableName, filter);

            _browser.SetQuery("SELECT * FROM test1 ORDER BY name");

            //assert original table values
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Assert.AreEqual(values3[row, col], _browser.GetDbRecord(row, col));
        } 

        [Test]
        public void AssertTableNotPreviouslyBackedupExceptionTest()
        {
            bool exceptionThrown = false;
            string msg = "";
            string expMsg = "System.NullReferenceException: bogusTableNameToCatchException not previously backed up.";
            int expLength = expMsg.Length;

            try
            {
                _browser.RestoreTable("bogusTableNameToCatchException");
            }
            catch (NullReferenceException e)
            {
                exceptionThrown = true;
                msg = e.ToString();
            }

            Assert.AreEqual(true, exceptionThrown);
            Assert.AreEqual(expMsg, msg.Substring(0, expLength));
        }

        [Test]
        public void AssertTableWithFilterNotPreviouslyBackedupExceptionTest()
        {
            bool exceptionThrown = false;
            string expMsg = "System.NullReferenceException: Table bogusTableNameToCatchException with filter bogusFilter = 1 was not previously backed up.";
            int expLength = expMsg.Length;
            string msg = "";

            try
            {
                _browser.RestoreTable("bogusTableNameToCatchException", "bogusFilter = 1");
            }
            catch (NullReferenceException e)
            {
                exceptionThrown = true;
                msg = e.ToString();
            }

            Assert.AreEqual(true, exceptionThrown);
            Assert.AreEqual(expMsg, msg.Substring(0, expLength));
        }

        [TestCase(1, true)]
        [TestCase(3, false)]
        public void AssertRecordCountTest(int count, bool exceptionExpected)
        {
            bool exceptionThrown = false;

            //set table values
            _browser.SetQuery("Insert into test1 values('JAKE', 'MIAMI', 'RED')");
            _browser.SetQuery("Insert into test1 values('ROB', 'WESTON', 'BLUE')");
            _browser.SetQuery("Insert into test1 values('NATALY', 'NAPLES', 'GREEN')");

            _browser.SetQuery("SELECT * FROM test1 ORDER BY name");

            try
            {
                _browser.AssertRecordCount(count);
            }
            catch (AssertRowCountFailedException)
            {
                exceptionThrown = true;
            }

            Assert.AreEqual(exceptionExpected, exceptionThrown);
        }

        [Test]
        public void AssertRecordValuesFailsTest()
        {
            bool exceptionThrown = false;

            //set table values
            _browser.SetQuery("Insert into test1 values('', 'MIAMI', 'RED')");

            _browser.SetQuery("SELECT * FROM test1");

            try
            {
                _browser.AssertRecordValues(0, 1, "CHICAGO");
            }
            catch (RecordNotFoundException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            exceptionThrown = false;

            try
            {
                _browser.AssertRecordValues("Should be: the empty string");
            }
            catch (RecordNotFoundException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            exceptionThrown = false;

            try
            {
                _browser.AssertRecordValues(99, 0, "this should fail");
            }
            catch (RecordNotFoundException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            exceptionThrown = false;

            try
            {
                _browser.AssertRecordValues(0, 99, "this should fail");
            }
            catch (ColumnIndexOutOfBoundsException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);


        }

        [TestCase("JAKE", "", "RED", "'JAKE', '', 'RED'")]
        [TestCase("EDDIE", "", "GREEN", "'EDDIE', NULL, 'GREEN'")]
        [TestCase("ROB", "WESTON", "BLUE", "'ROB', 'WESTON', 'BLUE'")]
        public void AssertRecordValues(string name, string city, string color, string queryValues)
        {
            //set table values
            _browser.InsertIntoTable(String.Format("Insert into test1 values({0})", queryValues));

            _browser.SetQuery("SELECT * FROM test1");

            // Run assertions on each row
            _browser.AssertRecordValues(0, 0, name);
            _browser.AssertRecordValuesByColumnName("name", name);

            _browser.AssertRecordValues(0, 1, city);
            _browser.AssertRecordValuesByColumnName("city", city);

            _browser.AssertRecordValues(0, 2, color);
            _browser.AssertRecordValuesByColumnName("color", color);
         
        }

		[Test]
		public void GetDbDateTest()
		{
			try
			{
				string wrongdateformat = _browser.GetDbDate(130);
			}
			catch (InvalidDateFormatException e)
			{
                Assert.IsTrue(e.Message.Equals("Format 130 not supported"));
			}
			try
			{
				string wrongdateformat = _browser.GetDbDate(131, true);
			}
			catch (InvalidDateFormatException e)
			{
				Assert.IsTrue(e.Message.Equals("Format 131 not supported"));
			}

			string dateformat0 = invokeRemoveZero(false, "Apr 12 2010 11:23AM", 0);
			string dateformat100 = invokeRemoveZero(false, "Apr  4 2010 10:30AM", 100);
			string dateformat101 = invokeRemoveZero(false, "04/04/2010", 101);
			string dateformat102 = invokeRemoveZero(false, "2010.04.04", 102);
			string dateformat103 = invokeRemoveZero(false, "04/04/2010", 103);
			string dateformat104 = invokeRemoveZero(false, "04.04.2010", 104);
			string dateformat105 = invokeRemoveZero(false, "04-04-2010", 105);
			string dateformat106 = invokeRemoveZero(false, "04 Apr 2010", 106);
			string dateformat107 = invokeRemoveZero(false, "Apr 04, 2010", 107);
			string dateformat108 = invokeRemoveZero(false, "15:05:03", 108);
			string dateformat109 = invokeRemoveZero(false, "Apr  4 2010 10:30:05:555AM", 109);
			string dateformat9 = invokeRemoveZero(false, "Apr 14 2010 10:30:05:555AM", 9);
			string dateformat110 = invokeRemoveZero(false, "04-04-2010", 110);
			string dateformat111 = invokeRemoveZero(false, "2010/04/04", 111);
			string dateformat112 = invokeRemoveZero(false, "20100404", 112);
			string dateformat113 = invokeRemoveZero(false, "04 Apr 2010 10:30:05:555", 113);
			string dateformat114 = invokeRemoveZero(false, "15:05:03:555", 114);
			string dateformat13 = invokeRemoveZero(false, "04 Apr 2010 10:30:05:555", 13);
			string dateformat120 = invokeRemoveZero(false, "2010-04-04 10:05:03", 120);
			string dateformat121 = invokeRemoveZero(false, "2010-04-04 10:05:03.555", 121);
			string dateformat20 = invokeRemoveZero(false, "2010-04-04 10:05:03", 20);
			string dateformat21 = invokeRemoveZero(false, "2010-04-04 10:05:03.555", 21);
			string dateformat126 = invokeRemoveZero(false, "2010-04-04T10:05:03.555", 126);

			Regex matchType101103 = new Regex("\\d{2}/\\d{2}/\\d{4}");
			Regex matchType102 = new Regex(@"\d{4}\.\d{2}\.\d{2}");
			Regex matchType104 = new Regex(@"\d{2}\.\d{2}\.\d{4}");
			Regex matchType105 = new Regex(@"\d{2}-\d{2}-\d{4}");
			Regex matchType106113 = new Regex("\\d{2} \\w{3} \\d{4}");
			Regex matchType107 = new Regex(@"\w{3} \d{2}, \d{4}");
			Regex matchType108 = new Regex("\\w{2}:\\w{2}:\\w{2}");
			Regex matchType109100 = new Regex("\\w{3}[ ]{1,2}\\d{1,2} \\d{4}");
			Regex matchType110 = new Regex("\\d{2}-\\d{2}-\\d{4}");
			Regex matchType111 = new Regex("\\d{4}/\\d{2}/\\d{2}");
			Regex matchType112 = new Regex("\\d{4}\\d{2}\\d{2}");
			Regex matchType114 = new Regex("\\w{2}:\\w{2}:\\w{2}:\\w{3}");
			Regex matchType120121126 = new Regex("\\d{4}-\\d{2}-\\d{2}");
			Regex matchType130 = new Regex("\\d{2} \\w{3} \\d{4}");
			Regex matchType131 = new Regex("\\d{2}/\\w{3}/\\d{4}");


			Assert.IsTrue(matchType109100.Match(dateformat100).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat0).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType101103.Match(dateformat101).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType101103.Match(dateformat103).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType106113.Match(dateformat106).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType106113.Match(dateformat113).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType106113.Match(dateformat13).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType102.Match(dateformat102).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType104.Match(dateformat104).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType105.Match(dateformat105).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType107.Match(dateformat107).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType108.Match(dateformat108).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat109).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat9).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType110.Match(dateformat110).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType111.Match(dateformat111).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType112.Match(dateformat112).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType114.Match(dateformat114).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat120).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat121).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat20).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat21).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat126).Success, "Invalid formats are returned");

			dateformat101 = _browser.GetDbDate();
			Assert.IsTrue(matchType101103.Match(dateformat101).Success, "Invalid formats are returned");


			dateformat0 = _browser.GetDbDate(0);
			dateformat100 = _browser.GetDbDate(100);
			dateformat101 = _browser.GetDbDate(101);
			dateformat102 = _browser.GetDbDate(102);
			dateformat103 = _browser.GetDbDate(103);
			dateformat104 = _browser.GetDbDate(104);
			dateformat105 = _browser.GetDbDate(105);
			dateformat106 = _browser.GetDbDate(106);
			dateformat107 = _browser.GetDbDate(107);
			dateformat108 = _browser.GetDbDate(108);
			dateformat109 = _browser.GetDbDate(109);
			dateformat9 = _browser.GetDbDate(9);
			dateformat110 = _browser.GetDbDate(110);
			dateformat111 = _browser.GetDbDate(111);
			dateformat112 = _browser.GetDbDate(112);
			dateformat113 = _browser.GetDbDate(113);
			dateformat114 = _browser.GetDbDate(114);
			dateformat13 = _browser.GetDbDate(13);
			dateformat120 = _browser.GetDbDate(120);
			dateformat121 = _browser.GetDbDate(121);
			dateformat20 = _browser.GetDbDate(20);
			dateformat21 = _browser.GetDbDate(21);
			dateformat126 = _browser.GetDbDate(126);



			Assert.IsTrue(matchType109100.Match(dateformat100).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat0).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType101103.Match(dateformat101).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType101103.Match(dateformat103).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType106113.Match(dateformat106).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType106113.Match(dateformat113).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType106113.Match(dateformat13).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType102.Match(dateformat102).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType104.Match(dateformat104).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType105.Match(dateformat105).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType107.Match(dateformat107).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType108.Match(dateformat108).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat109).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat9).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType110.Match(dateformat110).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType111.Match(dateformat111).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType112.Match(dateformat112).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType114.Match(dateformat114).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat120).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat121).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat20).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat21).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat126).Success, "Invalid formats are returned");



			dateformat0 = invokeRemoveZero(true, "Apr 12 2010 11:23AM", 0);
			dateformat100 = invokeRemoveZero(true, "Apr  4 2010 10:30AM", 100);
			dateformat101 = invokeRemoveZero(true, "04/04/2010", 101);
			dateformat102 = invokeRemoveZero(true, "2010.04.04", 102);
			dateformat103 = invokeRemoveZero(true, "04/04/2010", 103);
			dateformat104 = invokeRemoveZero(true, "04.04.2010", 104);
			dateformat105 = invokeRemoveZero(true, "04-04-2010", 105);
			dateformat106 = invokeRemoveZero(true, "04 Apr 2010", 106);
			dateformat107 = invokeRemoveZero(true, "Apr 04, 2010", 107);
			dateformat108 = invokeRemoveZero(true, "15:05:03", 108);
			dateformat109 = invokeRemoveZero(true, "Apr  4 2010 10:30:05:555AM", 109);
			dateformat9 = invokeRemoveZero(true, "Apr 14 2010 10:30:05:555AM", 9);
			dateformat110 = invokeRemoveZero(true, "04-04-2010", 110);
			dateformat111 = invokeRemoveZero(true, "2010/04/04", 111);
			dateformat112 = invokeRemoveZero(true, "20100404", 112);
			dateformat113 = invokeRemoveZero(true, "04 Apr 2010 10:30:05:555", 113);
			dateformat114 = invokeRemoveZero(true, "15:05:03:555", 114);
			dateformat13 = invokeRemoveZero(true, "04 Apr 2010 10:30:05:555", 13);
			dateformat120 = invokeRemoveZero(true, "2010-04-04 10:05:03", 120);
			dateformat121 = invokeRemoveZero(true, "2010-04-04 10:05:03.555", 121);
			dateformat20 = invokeRemoveZero(true, "2010-04-04 10:05:03", 20);
			dateformat21 = invokeRemoveZero(true, "2010-04-04 10:05:03.555", 21);
			dateformat126 = invokeRemoveZero(true, "2010-04-04T10:05:03.555", 126);

			matchType101103 = new Regex("\\d{1}/\\d{1}/\\d{4}");
			matchType102 = new Regex(@"\d{4}\.\d{1}\.\d{1}");
			matchType104 = new Regex(@"\d{1}\.\d{1}\.\d{4}");
			matchType105 = new Regex(@"\d{1}-\d{1}-\d{4}");
			matchType106113 = new Regex("\\d{1} \\w{3} \\d{4}");
			matchType107 = new Regex(@"\w{3} \d{1}, \d{4}");
			matchType109100 = new Regex("\\w{3}[ ]{1,2}\\d{1,2} \\d{4}");
			matchType110 = new Regex("\\d{1}-\\d{1}-\\d{4}");
			matchType111 = new Regex("\\d{4}/\\d{1}/\\d{1}");
			matchType112 = new Regex("\\d{4}\\d{1}\\d{1}");
			matchType120121126 = new Regex("\\d{4}-\\d{1}-\\d{1}");
			matchType130 = new Regex("\\d{1} \\w{3} \\d{4}");
			matchType131 = new Regex("\\d{1}/\\w{3}/\\d{4}");

			Assert.IsTrue(matchType109100.Match(dateformat100).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat0).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType101103.Match(dateformat101).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType101103.Match(dateformat103).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType106113.Match(dateformat106).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType106113.Match(dateformat113).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType106113.Match(dateformat13).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType102.Match(dateformat102).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType104.Match(dateformat104).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType105.Match(dateformat105).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType107.Match(dateformat107).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType108.Match(dateformat108).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat109).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat9).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType110.Match(dateformat110).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType111.Match(dateformat111).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType112.Match(dateformat112).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType114.Match(dateformat114).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat120).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat121).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat20).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat21).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType120121126.Match(dateformat126).Success, "Invalid formats are returned");


			//Formats that add zeros
			dateformat0 = invokeRemoveZero(true, "Apr 12 2010  1:23PM", 0);
			dateformat9 = invokeRemoveZero(true, "Apr 14 2010  9:30:05:555AM", 9);

			Assert.IsTrue(matchType109100.Match(dateformat0).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat9).Success, "Invalid formats are returned");

			dateformat0 = invokeRemoveZero(true, "Apr  2 2010  1:23PM", 0);
			dateformat9 = invokeRemoveZero(true, "Apr  4 2010  9:30:05:555AM", 9);

			Assert.IsTrue(matchType109100.Match(dateformat0).Success, "Invalid formats are returned");
			Assert.IsTrue(matchType109100.Match(dateformat9).Success, "Invalid formats are returned");

		}

        [Test]
        public void IncorrectStatementTypeExceptionTests()
        {
            bool exceptionThrown = false;
            try
            {
                _browser.UpdateTable("Insert into test1 values('JAKE', 'MIAMI', 'RED')");
            }
            catch (IncorrectStatementTypeException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception was not thrown.");
            exceptionThrown = false;

            try
            {
                _browser.InsertIntoTable("DELETE from test1 WHERE name = 'JAKE'");
            }
            catch (IncorrectStatementTypeException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception was not thrown.");
            exceptionThrown = false;

            try
            {
                _browser.DeleteFromTable("UPDATE test1 SET name = 'JAKE2' WHERE name = 'JAKE'");
            }
            catch (IncorrectStatementTypeException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception was not thrown.");
        }

		[Test]
		public void SaveDbDateTest()
		{
			Regex matchMON = new Regex("\\w{3}");
			Regex matchMMorDD = new Regex("\\d{1,2}");
			Regex matchMorD = new Regex("\\d{1,2}");
			Regex matchYYYY = new Regex("\\d{4}");
			Regex matchHH = new Regex("\\d{1,2}");
			Regex matchMM = new Regex("\\d{1,2}");
			Regex matchSS = new Regex("\\d{1,2}");


			_browser.SaveDbDate();
			string month = _browser.GetSavedDbDateMonth();
			string day = _browser.GetSavedDbDateDay();
			string year = _browser.GetSavedDbDateYear();

			Assert.IsTrue(matchMMorDD.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad default date format");

			_browser.SaveDbDate(100);
			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			string hours = _browser.GetSavedDbDate("hours");
			string minutes = _browser.GetSavedDbDate("minutes");
			Assert.IsTrue(matchMON.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");
			Assert.IsTrue(matchHH.Match(hours).Success && matchMM.Match(minutes).Success, "Bad date format");

			_browser.SaveDbDate(109);
			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			hours = _browser.GetSavedDbDate("hours");
			minutes = _browser.GetSavedDbDate("minutes");
			string seconds = _browser.GetSavedDbDate("seconds");
			Assert.IsTrue(matchMON.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");
			Assert.IsTrue(matchHH.Match(hours).Success && matchMM.Match(minutes).Success && matchSS.Match(seconds).Success, "Bad date format");

			_browser.SaveDbDate(108);
			hours = _browser.GetSavedDbDate("hours");
			minutes = _browser.GetSavedDbDate("minutes");
			seconds = _browser.GetSavedDbDate("seconds");
			Assert.IsTrue(matchHH.Match(hours).Success && matchMM.Match(minutes).Success && matchSS.Match(seconds).Success, "Bad date format");


			_browser.SaveDbDate(113);
			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			hours = _browser.GetSavedDbDate("hours");
			minutes = _browser.GetSavedDbDate("minutes");
			seconds = _browser.GetSavedDbDate("seconds");
			Assert.IsTrue(matchMON.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");
			Assert.IsTrue(matchHH.Match(hours).Success && matchMM.Match(minutes).Success && matchSS.Match(seconds).Success, "Bad date format");

			_browser.SaveDbDate(120);
			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			hours = _browser.GetSavedDbDate("hours");
			minutes = _browser.GetSavedDbDate("minutes");
			seconds = _browser.GetSavedDbDate("seconds");
			Assert.IsTrue(matchMMorDD.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");
			Assert.IsTrue(matchHH.Match(hours).Success && matchMM.Match(minutes).Success && matchSS.Match(seconds).Success, "Bad date format");

			_browser.SaveDbDate(121);
			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			hours = _browser.GetSavedDbDate("hours");
			minutes = _browser.GetSavedDbDate("minutes");
			seconds = _browser.GetSavedDbDate("seconds");
			Assert.IsTrue(matchMMorDD.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");
			Assert.IsTrue(matchHH.Match(hours).Success && matchMM.Match(minutes).Success && matchSS.Match(seconds).Success, "Bad date format");

			_browser.SaveDbDate(126);
			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			hours = _browser.GetSavedDbDate("hours");
			minutes = _browser.GetSavedDbDate("minutes");
			seconds = _browser.GetSavedDbDate("seconds");
			Assert.IsTrue(matchMMorDD.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");
			Assert.IsTrue(matchHH.Match(hours).Success && matchMM.Match(minutes).Success && matchSS.Match(seconds).Success, "Bad date format");


			_browser.SaveDbDate(107);
			month = _browser.GetSavedDbDateMonth();
			day = _browser.GetSavedDbDateDay();
			year = _browser.GetSavedDbDateYear();

			Assert.IsTrue(matchMON.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");

			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			Assert.IsTrue(matchMON.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");


			_browser.SaveDbDate(103);
			month = _browser.GetSavedDbDateMonth();
			day = _browser.GetSavedDbDateDay();
			year = _browser.GetSavedDbDateYear();

			Assert.IsTrue(matchMMorDD.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date formats dd mm yyyy");

			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			Assert.IsTrue(matchMMorDD.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");

			_browser.SaveDbDate(102);
			month = _browser.GetSavedDbDateMonth();
			day = _browser.GetSavedDbDateDay();
			year = _browser.GetSavedDbDateYear();

			Assert.IsTrue(matchMMorDD.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date formats yyyy dd mm");

			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			Assert.IsTrue(matchMMorDD.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");

			_browser.SaveDbDate(112);
			month = _browser.GetSavedDbDateMonth();
			day = _browser.GetSavedDbDateDay();
			year = _browser.GetSavedDbDateYear();

			Assert.IsTrue(matchMMorDD.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format 112");

			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			Assert.IsTrue(matchMMorDD.Match(month).Success && matchMMorDD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");

			_browser.SaveDbDate(112, true);
			month = _browser.GetSavedDbDateMonth();
			day = _browser.GetSavedDbDateDay();
			year = _browser.GetSavedDbDateYear();

			Assert.IsTrue(matchMorD.Match(month).Success && matchMorD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format 112");

			month = _browser.GetSavedDbDate("month");
			day = _browser.GetSavedDbDate("day");
			year = _browser.GetSavedDbDate("year");
			Assert.IsTrue(matchMorD.Match(month).Success && matchMorD.Match(day).Success && matchYYYY.Match(year).Success, "Bad date format with MON");


			bool exceptionThrown = false;
			try { _browser.SaveDbDate(131); }
			catch (Exception) { exceptionThrown = true; }

			Assert.IsTrue(exceptionThrown, "Bad format not detected");

			exceptionThrown = false;
			try { _browser.GetSavedDbDateDay(); }
			catch (Exception) { exceptionThrown = true; }

			Assert.IsTrue(exceptionThrown, "No saved date not detected");

			exceptionThrown = false;
			try { _browser.GetSavedDbDateMonth(); }
			catch (Exception) { exceptionThrown = true; }

			Assert.IsTrue(exceptionThrown, "No saved date not detected");

			exceptionThrown = false;
			try { _browser.GetSavedDbDateYear(); }
			catch (Exception) { exceptionThrown = true; }

			Assert.IsTrue(exceptionThrown, "No saved date not detected");

			exceptionThrown = false;
			try { _browser.GetSavedDbDate("Minutes"); }
			catch (Exception) { exceptionThrown = true; }

			Assert.IsTrue(exceptionThrown, "No saved date not detected");

			exceptionThrown = false;
			try
			{
				_browser.SaveDbDate(110);
				_browser.GetSavedDbDate("Hours");
			}
			catch (Exception) { exceptionThrown = true; }

			Assert.IsTrue(exceptionThrown, "Wrong command use not detected");

			exceptionThrown = false;
			try
			{
				_browser.SaveDbDate(110);
				_browser.GetSavedDbDate("Hello");
			}
			catch (Exception) { exceptionThrown = true; }

			Assert.IsTrue(exceptionThrown, "Wrong command use not detected");

			try
			{
				_browser.SaveDbDate(108);
				_browser.GetSavedDbDate("month");
			}
			catch (Exception) { exceptionThrown = true; }

			Assert.IsTrue(exceptionThrown, "Wrong command use not detected");
		}

        [Test]
        public void SQLCommandQueryReturnsQueryBeforeTimeoutExpiresTest()
        {
            string message = "";
            bool testPassed = true;
            DateTime start = DateTime.Now;
            int timeout = 60;

            try
            {
                _browser.SetQuery("Insert into test1(name) values('steve')", timeout);

            }
            catch (Exception e)
            {
                testPassed = false;
                message = e.ToString();
            }

            if (DateTime.Now >= start.AddSeconds(timeout))
                testPassed = false;

            Assert.IsTrue(testPassed, message);
        }

        [Test]
        public void GetRecordTest()
        {            
            string expectedValue = "JAKE";
            string actualValue;            

            // Set up the table
            _browser.SetQuery(String.Format("Insert into test1 values('{0}', 'MIAMI', 'RED')", expectedValue));
            _browser.SetQuery("SELECT * FROM test1");

            actualValue = _browser.GetDbRecord();

            Assert.AreEqual(expectedValue, actualValue);

            actualValue = _browser.GetDbRecordByColumnName("name");

            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void GetRecordFailsTest()
        {
            bool exceptionThrown = false;

            try
            {
                _browser.GetDbRecord(-1, -1);
            }
            catch (IndexOutOfBoundsException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "IndexOutOfBoundsException not thrown.");
            exceptionThrown = false;

            try
            {
                _browser.GetDbRecord();
            }
            catch (RowIndexOutOfBoundsException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "RowIndexOutOfBoundsException not thrown.");
            exceptionThrown = false;

            // Set up the table
            _browser.InsertIntoTable("Insert into test1 values('JAKE', 'MIAMI', 'RED')");
            _browser.SetQuery("SELECT * FROM test1");

            try
            {
                _browser.GetDbRecord(0, 99);
            }
            catch (ColumnIndexOutOfBoundsException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "ColumnIndexOutOfBoundsException not thrown.");
            exceptionThrown = false;

            try
            {
                _browser.GetDbRecordByColumnName("this column name doesn't exist");
            }
            catch (ColumnDoesNotExistException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "ColumnDoesNotExistException not thrown.");

        }

        [Test]
        public void DeleteFromTableTest()
        {
            bool exceptionThrown = false;
            // Set up the table
            _browser.InsertIntoTable("Insert into test1 values('JAKE', 'MIAMI', 'RED')");
            _browser.SetQuery("SELECT * FROM test1");

            _browser.DeleteFromTable("DELETE FROM test1 WHERE name='JAKE'");
            _browser.SetQuery("SELECT * FROM test1");

            // Throw an exception because we deleted the only entry in the database
            try
            {
                _browser.GetDbRecord();
            }
            catch (RowIndexOutOfBoundsException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "RowIndexOutOfBoundsException not thrown.");
        }

        [Test]
        public void DatabaseDisposedProperlyTest()
        {
            DataAccess.Database database = ReflectionHelper.GetField<DataAccess.Database>(_browser, "_database");
            System.Data.IDbConnection connection = ReflectionHelper.GetField<System.Data.IDbConnection>(database,
                                                                                                        "_connection");
            if (connection.State == System.Data.ConnectionState.Open)
                database.Dispose();
            else
                Assert.Fail("Connection is already closed");

            Assert.AreEqual(connection.State, System.Data.ConnectionState.Closed);

            // Reconnect for the TearDown
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

        [TestCase(false)]
        [TestCase(true)]
        public void SetQueryFromFileTest(bool withTimeout)
        {
            string relativePath = System.Configuration.ConfigurationManager.AppSettings["DatabaseTestFiles"];
            string AddElementsTest =  relativePath + (_DbType == DatabaseType.MSSQL ? @"AddElementsTest.sql" : @"AddElementsOracleTest.sql");

            if (withTimeout)
            {
                _browser.SetQuery("/file:" + AddElementsTest, 30);
            }
            else
            {
                _browser.SetQuery("/file:" + AddElementsTest);
            }
            _browser.SetQuery("SELECT * FROM test1");
            Assert.IsTrue(_browser.GetDbRecord(0, 0).Equals("David"));
            Assert.IsTrue(_browser.GetDbRecord(1, 1).Equals("San Antonio"));
            Assert.IsTrue(_browser.GetDbRecord(2, 2).Equals("Green"));
        }

        [TestCase(false)]
        [TestCase(true)]
        [ExpectedException(typeof(FileNotFoundException))]
        public void SetQueryFromFileDoesNotExistTest(bool withTimeout)
        {
            if (withTimeout)
            {
                _browser.SetQuery("/file:C:\\FictionalMcFakeMcgee.sql", 30);
            }
            else
            {
                _browser.SetQuery("/file:C:\\FictionalMcFakeMcgee.sql");
            }
        }

        public virtual void insertTestRecord()
        {
            using (WebBrowser testBrowser = new WebBrowser(BrowserType.InternetExplorer))
            {
                testBrowser.ConnectToMssql("localhost", "swat", "swat");
                testBrowser.SetDatabase(localDbName1);
                System.Threading.Thread.Sleep(1000);
                testBrowser.InsertIntoTable("Insert into test1(name) values('steve')");
                testBrowser.Dispose();
            }
        }

		protected string invokeRemoveZero(bool removezero, string date, int format)
		{
			object[] param = new object[] { removezero, date, format };
		    DataAccess.Database db = ReflectionHelper.GetField<object>(_browser, "_database") as DataAccess.Database;
		    return ReflectionHelper.InvokeMethod<object>(db, "RemoveZero", param) as string;

		}

        
    }
}
