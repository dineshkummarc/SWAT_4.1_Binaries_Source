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

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("Misc")]
    public class ComparisonTestFixture
    {
        WebBrowser _browser;

        public ComparisonTestFixture()
        {
            _browser = new WebBrowser(BrowserType.FireFox);
        }

         
        [TestCase("5", "6", true)]
        [TestCase("Apple", "Dog", true)]
        [TestCase("dog", "Apple", false)]
        [TestCase("6", "5", false)]
        [TestCase("5", "5", false)]
        [TestCase("10", "2", false)]
        [TestCase("23","apple",true)]
        [TestCase("apple","32",false)]
        public void AssertLessThan_Is_Correct(String val1, String val2, bool expected)
        {
            bool assertFail = true;

            try
            {
                _browser.AssertLessThan(val1, val2);
            }
            catch
            {
                assertFail = false;
            }

            Assert.AreEqual(expected, assertFail);
        }

         
        [TestCase("5", "6", false)]
        [TestCase("Apple", "Dog", false)]
        [TestCase("dog", "Apple", true)]
        [TestCase("6", "5", true)]
        [TestCase("5", "5", false)]
        [TestCase("10","2",true)]
        [TestCase("apple","45",true)]
        public void AssertGreaterThan_Is_Correct(String val1, String val2, bool expected)
        {
            bool assertFail = true;

            try
            {
                _browser.AssertGreaterThan(val1, val2);
            }
            catch
            {
                assertFail = false;
            }

            Assert.AreEqual(expected, assertFail);
        }

         
        [TestCase("5", "6", true)]
        [TestCase("Apple", "Dog", true)]
        [TestCase("dog", "Apple", false)]
        [TestCase("6", "5", false)]
        [TestCase("5", "5", true)]
        [TestCase("Apple","apple",true)]
        [TestCase("10","5",false)]
        public void AssertLessThanOrEqual_Is_Correct(String val1, String val2, bool expected)
        {
            bool assertFail = true;

            try
            {
                _browser.AssertLessThanOrEqual(val1, val2);
            }
            catch
            {
                assertFail = false;
            }

            Assert.AreEqual(expected, assertFail);
        }

         
        [TestCase("5", "6", false)]
        [TestCase("Apple", "Dog", false)]
        [TestCase("dog", "Apple", true)]
        [TestCase("6", "5", true)]
        [TestCase("5", "5", true)]
        [TestCase("apple","Apple",true)]
        [TestCase("10","5",true)]
        public void AssertGreaterThanOrEqual_Is_Correct(String val1, String val2, bool expected)
        {
            bool assertFail = true;

            try
            {
                _browser.AssertGreaterThanOrEqual(val1, val2);
            }
            catch
            {
                assertFail = false;
            }

            Assert.AreEqual(expected, assertFail);
        }

         
        [TestCase("5", "6", false)]
        [TestCase("Apple", "Dog", false)]
        [TestCase("dog", "Apple", false)]
        [TestCase("6", "5", false)]
        [TestCase("5", "5", true)]
        [TestCase("Apple", "apple", true)]
        [TestCase("Apple", "Apple", true)]
        [TestCase("110","11",false)]
        public void AssertEqualTo_Is_Correct(String val1, String val2, bool expected)
        {
            bool assertFail = true;

            try
            {
                _browser.AssertEqualTo(val1, val2);
            }
            catch
            {
                assertFail = false;
            }

            Assert.AreEqual(expected, assertFail);
        }

         
        [TestCase("5", "6", "Expected value was \"6\". Actual DB value was \"5\".")]
        [TestCase("09/24/1982", "08/01/1985", "Expected value was \"08/01/1985\". Actual DB value was \"09/24/1982\".")]
        [TestCase("dog", "Apple", "Expected value was \"Apple\". Actual DB value was \"dog\".")]
        [TestCase("True", "False", "Expected value was \"False\". Actual DB value was \"True\".")]
        public void TestRecordNotFoundExceptionErrorMessages(string val1, string val2, string errorMessage)
        {
            string result;

            try
            {                
                throw new RecordNotFoundException(val1, val2);
            }
            catch (RecordNotFoundException e)
            {
                result = e.Message;
            }

            Assert.AreEqual(result, errorMessage);
        }
    }
}
