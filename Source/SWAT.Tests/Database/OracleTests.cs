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
using System.Text.RegularExpressions;
using Oracle.DataAccess.Client;

namespace SWAT.Tests.Database
{
    [TestFixture]
    public class OracleTests : DatabaseTests
    {
        public OracleTests() : base(DatabaseType.Oracle)
        {

        }

        [Test]
        public void ConnectToLocalDbTest()
        {
            _browser.ConnectToOracle("localhost", "System", "password");
            _browser.SetQuery("SELECT * FROM test1");
        }
        
        [Test]
        public void BasicConnectionTest()
        {
            _browser.SetQuery("Insert into test1(name) values('Jake')");
        }

        [Test]
        public void SQLCommandQueryReturnsQueryErrorWhenTimeoutExceedsTest()
        {
            string message = "";
            bool testPassed = false;
            bool otherError = false;

            try
            {
                _browser.SetQuery("BEGIN DBMS_LOCK.SLEEP(7); Insert into test1(name) values('steve'); END;", 5);
                //_browser.SetQuery("Insert into test1(name) values('steve')", 1);
            }
            catch (OracleException e)
            {
                testPassed = true;
                message = e.ToString();
            }
            catch (Exception e)
            {
                otherError = true;
                message = e.ToString();
            }

            Assert.IsFalse(otherError, message);
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

        public override void insertTestRecord()
        {
            WebBrowser testBrowser = new WebBrowser(BrowserType.InternetExplorer);
            testBrowser.ConnectToOracle("localhost", "System", "password");
            System.Threading.Thread.Sleep(1000); // Delay to simulate user adding record mid-polling
            testBrowser.SetQuery("Insert into test1(name) values('steve')");
        }

        [Test]
        public void SetDatabaseExceptionTest()
        {
            try
            {
                _browser.SetDatabase("test");
            }
            catch (UnsupportedCommandException e)
            {
                Assert.AreEqual(e.Message, "SetDatabase is not a supported command for Oracle.");
                return;
            }

            //Shouldn't get here
            Assert.Fail();
        }
    }
}
