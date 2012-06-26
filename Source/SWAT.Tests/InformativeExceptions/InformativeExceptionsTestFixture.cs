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
 

namespace SWAT.Tests.InformativeExceptions
{
    public abstract class InformativeExceptionsTestFixture : BrowserTestFixture
    {
        public InformativeExceptionsTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        [Test]
        public void TestInformativeExceptionsSettings()
        {
            //Save the value to reset later
            bool reset = SWAT.WantInformativeExceptions.GetInformativeExceptions;

            //Turn off informative Exceptions setting
            SWAT.WantInformativeExceptions.GetInformativeExceptions = false;
            SWAT.UserConfigHandler.Save();

            Assert.AreEqual(false, WantInformativeExceptions.GetInformativeExceptions);

            //Turn on informative exceptions setting
            SWAT.WantInformativeExceptions.GetInformativeExceptions = true;
            SWAT.UserConfigHandler.Save();

            Assert.AreEqual(true, WantInformativeExceptions.GetInformativeExceptions);

            //Reset the value
            SWAT.WantInformativeExceptions.GetInformativeExceptions = reset;
            SWAT.UserConfigHandler.Save();
        }

         
        [TestCase(false)]
        [TestCase(true)]
        public void TestInformativeExceptions(bool setInformativeExceptions)
        {
            // setup
            string identifier = "id:IDontExist";
            string tagName = "test";
            bool assertElementDoesNotExistExceptionPassed = false;
            bool assertElementExistExceptionPassed = false;
            bool stimulateElementExceptionPassed = false;
            bool getElementExceptionPassed = false;
            bool setElementAttributeExceptionPassed = false;

            //Save the value to reset later
            bool reset = SWAT.WantInformativeExceptions.GetInformativeExceptions;

            try
            {
                SWAT.WantInformativeExceptions.GetInformativeExceptions = setInformativeExceptions;
                SWAT.UserConfigHandler.Save();

                if (!setInformativeExceptions)
                {
                    try
                    {
                        _browser.AssertElementDoesNotExist(IdentifierType.Expression, "id=txtOne", "input");
                    }
                    catch (SWAT.AssertionFailedException result)
                    {
                        if (result.Message.Equals(string.Format("Element with Expression {0} was found.", "id=txtOne")))
                        {
                            assertElementDoesNotExistExceptionPassed = true;
                        }
                    }

                    try
                    {
                        _browser.AssertElementExists(IdentifierType.Expression, identifier, tagName);
                    }
                    catch (SWAT.AssertionFailedException result)
                    {
                        if (result.Message.Equals(string.Format("Element with Expression {0} was not found.", identifier)))
                        {
                            assertElementExistExceptionPassed = true;
                        }
                    }

                    try
                    {
                        _browser.StimulateElement(IdentifierType.Expression, identifier, "onclick", tagName);
                    }
                    catch (SWAT.ElementNotFoundException result)
                    {
                        if (result.Message.Equals(string.Format("Unable to find element with Expression '{0}'", identifier)))
                        {
                            stimulateElementExceptionPassed = true;
                        }
                    }

                    try
                    {
                        _browser.GetElementAttribute(IdentifierType.Expression, identifier, "test");
                    }
                    catch (SWAT.ElementNotFoundException result)
                    {
                        if (result.Message.Equals(string.Format("Unable to find element with Expression '{0}'", identifier)))
                        {
                            getElementExceptionPassed = true;
                        }
                    }

                    try
                    {
                        _browser.SetElementAttribute(IdentifierType.Expression, identifier, "test", "test");
                    }
                    catch (SWAT.ElementNotFoundException result)
                    {
                        if (result.Message.Equals(string.Format("Unable to find element with Expression '{0}'", identifier)))
                        {
                            setElementAttributeExceptionPassed = true;
                        }
                    }
                }
                else
                {
                    try
                    {
                        _browser.AssertElementDoesNotExist(IdentifierType.Expression, "id=txtOne", "input");
                    }
                    catch (SWAT.AssertionFailedException result)
                    {
                        if (result.Message.Equals(string.Format("Element with Expression {0} and tag {1} was found.", "id=txtOne", "input")))
                        {
                            assertElementDoesNotExistExceptionPassed = true;
                        }
                    }

                    try
                    {
                        _browser.AssertElementExists(IdentifierType.Expression, identifier, tagName);
                    }
                    catch (SWAT.AssertionFailedException result)
                    {
                        if (result.Message.Equals(string.Format("Element with Expression {0} and tag {1} was not found.", identifier, tagName)))
                        {
                            assertElementExistExceptionPassed = true;
                        }
                    }

                    try
                    {
                        _browser.StimulateElement(IdentifierType.Expression, identifier, "onclick", tagName);
                    }
                    catch (SWAT.ElementNotFoundException result)
                    {
                        if (result.Message.Equals(string.Format("Unable to find element with Expression '{0}' and tag '{1}'", identifier, tagName)))
                        {
                            stimulateElementExceptionPassed = true;
                        }
                    }

                    try
                    {
                        _browser.GetElementAttribute(IdentifierType.Expression, identifier, "test", tagName);
                    }
                    catch (SWAT.ElementNotFoundException result)
                    {
                        if (result.Message.Equals(string.Format("Unable to find element with Expression '{0}' and tag '{1}'", identifier, tagName)))
                        {
                            getElementExceptionPassed = true;
                        }
                    }

                    try
                    {
                        _browser.SetElementAttribute(IdentifierType.Expression, identifier, "test", "test", tagName);
                    }
                    catch (SWAT.ElementNotFoundException result)
                    {
                        if (result.Message.Equals(string.Format("Unable to find element with Expression '{0}' and tag '{1}'", identifier, tagName)))
                        {
                            setElementAttributeExceptionPassed = true;
                        }
                    }
                }

                Assert.IsTrue(assertElementExistExceptionPassed);
                Assert.IsTrue(stimulateElementExceptionPassed);
                Assert.IsTrue(getElementExceptionPassed);
                Assert.IsTrue(setElementAttributeExceptionPassed);
            }
            finally
            {
                //Reset the value
                SWAT.WantInformativeExceptions.GetInformativeExceptions = reset;
                SWAT.UserConfigHandler.Save();
            }
        }
    }
}
