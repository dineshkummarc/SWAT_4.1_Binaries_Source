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
 

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("Misc")]
  public class ExpressionTestFixture
  {
     
    [TestCase("innerHtml#2:<option>", 2, "innerHtml", "<option>", SWAT.Browser.MatchType.Contains)]
    [TestCase("innerHtml:dude", int.MinValue, "innerHtml", "dude", SWAT.Browser.MatchType.Contains)]
    [TestCase("innerHtml#2=dude", 2, "innerHtml", "dude", SWAT.Browser.MatchType.Equals)]
    public void TestToken(string token, int expectedMatchCount, string attribute, string expectedValue, SWAT.Browser.MatchType matchType)
    {
      SWAT.Browser.ExpressionToken expToken = new Browser.ExpressionToken(token);
      Assert.AreEqual(expectedMatchCount, expToken.ExpectedMatchCount);
      Assert.AreEqual(SWAT.Browser.AttributeNormalizer.Normalize(attribute), expToken.Attribute);
      Assert.AreEqual(expectedValue, expToken.Value);
      Assert.AreEqual(matchType, expToken.MatchType);
    }


     
    [TestCase("innerHtml", "innerHTML")]
    [TestCase("id", "id")]
    public void TestAttributeNormalization(string attr, string expectedResult)
    {
      Assert.AreEqual(expectedResult, SWAT.Browser.AttributeNormalizer.Normalize(attr));
      Assert.AreEqual("innerHTML", SWAT.Browser.AttributeNormalizer.Normalize("innerHtml"));
    }

     
    [TestCase("This should fail")]
    [TestCase("!@#$%^&*()_+-")]
    public void TestTokenFailedTest( string invalidString)
    {
        bool exceptionThrown = false;

        try
        {
            SWAT.Browser.ExpressionToken expToken = new Browser.ExpressionToken(invalidString);
        }
        catch (ArgumentException)
        {
            exceptionThrown = true;
        }

        Assert.IsTrue(exceptionThrown);

    }
  }
}
