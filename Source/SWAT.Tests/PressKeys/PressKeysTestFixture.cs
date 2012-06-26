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
using SWAT.Tests;
using NUnit.Framework;
using SWAT.Reflection;

namespace SWAT.Tests.PressKeys
{
	public abstract class PressKeysTestFixture : BrowserTestFixture
	{
		public PressKeysTestFixture(BrowserType browserType)
			: base(browserType)
		{

		}

		[TearDown]
		public override void TestTeardown()
		{
			this.NavigateToSwatTestPage();
		}

        [SetUp]
        public override void TestSetup()
        {
            _browser.AttachToWindow("Test Page");
        }

		#region PressKeys

        [Test]
        public void PressKeysFiresAllKeyEventsOnFocusedElements()
        {
            //test onKeyUp
            _browser.PressKeys(IdentifierType.Id, "txtBox1", "keyup test", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id=divTxtBox1;innerHtml=keyup", "div");

            //test onKeyDown
            _browser.PressKeys(IdentifierType.Id, "txtBox2", "keydown test", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id=divTxtBox2;innerHtml=keydown", "div");

            //testOnKeyPress
            _browser.PressKeys(IdentifierType.Id, "txtBox3", "keypress test", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id=divTxtBox3;innerHtml=keypress", "div");
        }

        [Test]
        public void PressKeysCorrectlyHandlesKeyEventsBeingCanceled()
        {
            NavigateToSwatTestPage();
            _browser.PressKeys(IdentifierType.Id, "cancelEventTestBox", "none of these keystrokes should register", "input");
            string retrieved = _browser.GetElementAttribute(IdentifierType.Id, "cancelEventTestBox", "value", "input");
            Assert.AreEqual("This should not change", retrieved);
        }

		[Test]
		[Ignore]
		public void PressKeysInWindowWithSpecialCharsTest()
		{
			_browser.NavigateBrowser(getTestPage("SpanishCharactersPage.html"));
			_browser.PressKeys(IdentifierType.Id, "attachTest", "abc", "textarea");
			_browser.AssertElementExists(IdentifierType.Expression, "value=abcMama mia, here I go again");
			_browser.PressKeys(IdentifierType.Id, "attachTest", @"\{BACKSPACE\}", "textarea");
			_browser.AssertElementExists(IdentifierType.Expression, "value=abMama mia, here I go again");
		}

		[Test]
		[ExpectedException(typeof(ElementNotFoundException))]
		public void PressKeysFailsWhenCannotFindElementTest()
		{
			_browser.PressKeys(IdentifierType.Id, "doesNotExist", "foo", "input");
		}

		[Test]
        [ExpectedException]
		public void PressKeysFailsWhenEndCodeSequenceDoesNotExistTest()
		{
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", @"\{TAB", "input");
		}

		[Test]
		public void PressKeysFailsWhenTryingToRunSpecialKeyCodesInALockedDesktopTest()
		{
            if (_browserType == BrowserType.Safari)
                Assert.Ignore("Test is irrelevant for Safari.");

			bool failed = false;
            try
            {
                SetForceBrowserPressKeys(true);
                ReflectionHelper.InvokeMethod<object>(_browser, "PressKeys", IdentifierType.Id, "txtPressKey", @"\{TAB\}", "input");
            }
            catch (Exception e)
            {
                if (e.Message.Contains("locked desktop"))
                    failed = true;
            }
            SetForceBrowserPressKeys(false);
            Assert.IsTrue(failed, @"PressKeys didn't throw the correct exception when trying to type a special key code in a locked desktop.");
		}

		[Test]
        [ExpectedException]
		public void PressKeysFailsWhenTryingToRunSpecialKeyCodeThatDoesNotExistTest()
		{
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", @"\{TEST\}", "input");
		}

		[Test]
        [ExpectedException]
		public void PressKeysFailsWhenAltCombinationIsNotSupportedTest()
		{
			_browser.NavigateBrowser(getTestPage("KeyEventsTestPage.htm"));
			_browser.PressKeys(IdentifierType.Id, "txtAltCombo", @"\{ALT+F1\}", "input");
		}

		[Test]
		public void PressKeysAltLetterTest()
		{
			try
			{
				_browser.NavigateBrowser(getTestPage("KeyEventsTestPage.htm"));

				_browser.PressKeys(IdentifierType.Id, "txtAltCombo", @"\{ALT+Q\}", "input");
				string result = _browser.GetElementAttribute(IdentifierType.Id, "altQResults", "innerHTML", "td");

				Assert.AreEqual("Passed", result);
			}
			finally
			{
				// Clean up
				this.NavigateToSwatTestPage();
			}
		}

		[Test]
		public void PressKeysAltNumberTest()
		{
			try
			{
				_browser.NavigateBrowser(getTestPage("KeyEventsTestPage.htm"));

				_browser.PressKeys(IdentifierType.Id, "txtAltCombo", @"\{ALT+1\}", "input");
				string result = _browser.GetElementAttribute(IdentifierType.Id, "alt1Results", "innerHTML", "td");

				Assert.AreEqual("Passed", result);
			}
			finally
			{
				// Clean up
				this.NavigateToSwatTestPage();
			}
		}

		[Test]
		public void PressKeysAppendTextTest()
		{
			try
			{
				SetForceBrowserPressKeys(true);
				_browser.PressKeys(IdentifierType.Id, "txtPressKey", "first_", "input");
				_browser.PressKeys(IdentifierType.Id, "txtPressKey", "second", "input");
				string result = _browser.GetElementAttribute(IdentifierType.Id, "txtPressKey", "value", "input");
				Assert.AreEqual("first_second", result, "PressKeys is not working correctly when appending characters.");
			}
			finally
			{
                SetForceBrowserPressKeys(false);
				this.NavigateToSwatTestPage();
			}
		}

		[Test]
		public void PressKeysInLockedDesktopTest()
		{
			string expectedResult = String.Empty;
            if (_browserType == BrowserType.Safari)
                Assert.Ignore("Ignoring For Safari");
			try
			{
				expectedResult = "locked";
				SetForceBrowserPressKeys(true);
			    ReflectionHelper.InvokeMethod(_browser, "PressKeys", IdentifierType.Id, "txtPressKey", "locked", "input");
			}
			finally
			{   
				// Clean up
				SetForceBrowserPressKeys(false);
			}

			string actualResult = _browser.GetElementAttribute(IdentifierType.Id, "txtPressKey", "value", "input");
			Assert.AreEqual(expectedResult, actualResult, "PressKeys is not working correctly in a locked desktop.");
		}

		[Test]
		public void PressKeysAlphabetLowercaseCharactersTest()
		{
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "abcdefghijklmnopqrstuvwxyz", "input");
			string result = _browser.GetElementAttribute(IdentifierType.Id, "txtPressKey", "value", "input");
			Assert.AreEqual("abcdefghijklmnopqrstuvwxyz", result, "PressKeys is not working correctly for lowercase alphabet characters.");
		}

		[Test]
		public void PressKeysAlphabetUppercaseCharactersTest()
		{
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "input");
			string result = _browser.GetElementAttribute(IdentifierType.Id, "txtPressKey", "value", "input");
			Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZ", result, "PressKeys is not working correctly for uppercase alphabet characters.");
		}

		[Test]
		public void PressKeysNumberCharactersTest()
		{
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "0123456789", "input");
			string result = _browser.GetElementAttribute(IdentifierType.Id, "txtPressKey", "value", "input");
			Assert.AreEqual("0123456789", result, "PressKeys is not working correctly for number characters.");
		}

		[Test]
		public void PressKeysSpecialCharactersTest()
		{
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", ",./;'[]\\<>?:\"{}|~!@#$%^&*()_+=-`", "input");
			string result = _browser.GetElementAttribute(IdentifierType.Id, "txtPressKey", "value", "input");
			Assert.AreEqual(",./;'[]\\<>?:\"{}|~!@#$%^&*()_+=-`", result, "PressKeys is not working correctly for special characters.");
		}

		[Test]
		public void PressKeysBackspaceLeftRightArrowKeysTest()
		{
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "test this");
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "\\{LEFT_ARROW\\}");
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "\\{BACKSPACE\\}");
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "\\{RIGHT_ARROW\\}");
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "\\{BACKSPACE\\}");
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "\\{LEFT_ARROW\\}");
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "\\{BACKSPACE\\}");
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "\\{RIGHT_ARROW\\}");
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "\\{BACKSPACE\\}");
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "\\{BACKSPACE\\}");

			string result = _browser.GetElementAttribute(IdentifierType.Id, "txtPressKey", "value", "input");
			Assert.AreEqual("test", result);
		}

		[Test]
		public void PressKeysTabKeyTest()
		{
			_browser.PressKeys(IdentifierType.Id, "txtBox1", "this", "input");
			_browser.PressKeys(IdentifierType.Id, "txtBox1", "\\{TAB\\}");
			_browser.PressKeys(IdentifierType.Id, "txtBox2", "tests", "input");
			_browser.PressKeys(IdentifierType.Id, "txtBox2", "\\{TAB\\}");
			_browser.PressKeys(IdentifierType.Id, "txtBox3", "tab", "input");
			_browser.PressKeys(IdentifierType.Id, "txtBox3", "\\{TAB\\}");
			_browser.PressKeys(IdentifierType.Id, "txtBox4", "key", "input");

			StringBuilder sb = new StringBuilder();
			sb.Append(_browser.GetElementAttribute(IdentifierType.Id, "txtBox1", "value") + " ");
			sb.Append(_browser.GetElementAttribute(IdentifierType.Id, "txtBox2", "value") + " ");
			sb.Append(_browser.GetElementAttribute(IdentifierType.Id, "txtBox3", "value") + " ");
			sb.Append(_browser.GetElementAttribute(IdentifierType.Id, "txtBox4", "value"));

			Assert.AreEqual("this tests tab key", sb.ToString());
		}

		[Test]
		public void PressKeysLongStringTest()
		{
			string text = "In view, a humble vaudevillian veteran, cast vicariously as both victim and villain by the vicissitudes of Fate. This visage, no mere veneer of vanity, is a vestige of the vox populi, now vacant, vanished. However, this valorous visitation of a by-gone vexation, stands vivified and has vowed to vanquish these venal and virulent vermin van-guarding vice and vouchsafing the violently vicious and voracious violation of volition.";
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", text, "input");
			string result = _browser.GetElementAttribute(IdentifierType.Id, "txtPressKey", "value", "input");
			Assert.AreEqual(text, result, "PressKeys is not working correctly with long strings.");
		}

		[Test]
		public void PressKeysInternationalCharactersTest()
		{
			string text = "çÇáàãéèíìóòõúùü";
			//string text = "âäàáãâåÄÅÀÁÂÃçÇêëèéÈÉÊËïîìíÎÌÍÏñÑôöòóõÔÖÕÒÓšŠûúùüÜÙÚÛÿýŸÝžŽ";
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", text, "input");
			_browser.AssertElementExists(IdentifierType.Expression, "id:txtPressKey;value:" + text, "input");
		}

		//[Test]
		//public void TestPressKeysSpecialCharactersOnKeyUp()
		//{
		//    TestSetup();

		//    char[] alphabet = ",./;'[]\\<>?:\"{}|~!@#$%^&*()_+=-`".ToCharArray();
		//    string word, result;
		//    StringBuilder errorMessage = new StringBuilder("KeyUp Errors: ");
		//    bool isBroken = false;

		//    for (int i = 0; i < alphabet.Length; i++)
		//    {
		//        word = alphabet[i].ToString();
		//        _browser.PressKeys(IdentifierType.Id, "txtBox1", word, "input");
		//        result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox1", "innerHtml", "div");
		//        if (!result.Equals("keyup"))
		//        {
		//            if (!isBroken)
		//                isBroken = true;
		//            errorMessage.Append(word + " ");
		//        }
		//        _browser.SetElementAttribute(IdentifierType.Id, "txtBox1", "value", "", "input");
		//        _browser.SetElementAttribute(IdentifierType.Id, "divTxtBox1", "innerHtml", "", "div");
		//    }

		//    Assert.AreEqual(false, isBroken, errorMessage.ToString());
		//}

		//[Test]
		//public void TestPressKeysSpecialCharactersOnKeyDown()
		//{
		//    TestSetup();

		//    char[] alphabet = ",./;'[]\\<>?:\"{}|~!@#$%^&*()_+=-`".ToCharArray();
		//    string word, result;
		//    StringBuilder errorMessage = new StringBuilder("KeyDown Errors: ");
		//    bool isBroken = false;

		//    for (int i = 0; i < alphabet.Length; i++)
		//    {
		//        word = alphabet[i].ToString();
		//        _browser.PressKeys(IdentifierType.Id, "txtBox2", word, "input");
		//        result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox2", "innerHtml", "div");
		//        if (!result.Equals("keydown"))
		//        {
		//            if (!isBroken)
		//                isBroken = true;
		//            errorMessage.Append(word + " ");
		//        }
		//        _browser.SetElementAttribute(IdentifierType.Id, "txtBox2", "value", "", "input");
		//        _browser.SetElementAttribute(IdentifierType.Id, "divTxtBox2", "innerHtml", "", "div");
		//    }

		//    Assert.AreEqual(false, isBroken, errorMessage.ToString());
		//}

		[Test]
		public void PressKeysSpecialCharactersOnKeyPressTest()
		{
			char[] alphabet = ",./;'[]\\<>?:\"{}|~!@#$%^&*()_+=-`".ToCharArray();
			string word, result;
			StringBuilder errorMessage = new StringBuilder("KeyPress isn't triggering because of KeyUp/KeyDown Errors: ");
			bool isBroken = false;

			for (int i = 0; i < alphabet.Length; i++)
			{
				word = alphabet[i].ToString();
				_browser.PressKeys(IdentifierType.Id, "txtBox3", word, "input");
				result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox3", "innerHtml", "div");
				if (!result.Equals("keypress"))
				{
					isBroken = true;
					errorMessage.Append(word + " ");
				}
				_browser.SetElementAttribute(IdentifierType.Id, "txtBox3", "value", "", "input");
				_browser.SetElementAttribute(IdentifierType.Id, "divTxtBox3", "innerHtml", "", "div");
			}

			Assert.AreEqual(false, isBroken, errorMessage.ToString());
		}

		[Test]
		public void PressKeysFailsWhenAPopupIsLaunchedCausingATimingIssueTest()
		{
			try
			{
                _browser.PressKeys(IdentifierType.Id, "txtOne", "testing", "input");
				_browser.StimulateElement(IdentifierType.Id, "btnNewWindow", "onclick", "input");
				_browser.SetElementAttribute(IdentifierType.Id, "txtOne", "value", "", "input");
				_browser.PressKeys(IdentifierType.Id, "txtOne", "123", "input");
				Assert.AreEqual("123", _browser.GetElementAttribute(IdentifierType.Id, "txtOne", "value", "input"));
			}
			finally
			{
				_browser.KillAllOpenBrowsers();
				OpenSwatTestPage();
			}
		}

		//[Test]
		//public void TestPressKeysNumbersOnKeyUp()
		//{
		//    TestSetup();

		//    char[] alphabet = "0123456789".ToCharArray();
		//    string word, result;
		//    StringBuilder errorMessage = new StringBuilder("KeyUp Errors: ");
		//    bool isBroken = false;

		//    for (int i = 0; i < alphabet.Length; i++)
		//    {
		//        word = alphabet[i].ToString();
		//        _browser.PressKeys(IdentifierType.Id, "txtBox1", word, "input");
		//        result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox1", "innerHtml", "div");
		//        if (!result.Equals("keyup"))
		//        {
		//            if (!isBroken)
		//                isBroken = true;
		//            errorMessage.Append(word + " ");
		//        }
		//        _browser.SetElementAttribute(IdentifierType.Id, "txtBox1", "value", "", "input");
		//        _browser.SetElementAttribute(IdentifierType.Id, "divTxtBox1", "innerHtml", "", "div");
		//    }

		//    Assert.AreEqual(false, isBroken, errorMessage.ToString());
		//}

		//[Test]
		//public void TestPressKeysNumbersOnKeyDown()
		//{
		//    TestSetup();

		//    char[] alphabet = "0123456789".ToCharArray();
		//    string word, result;
		//    StringBuilder errorMessage = new StringBuilder("KeyDown Errors: ");
		//    bool isBroken = false;

		//    for (int i = 0; i < alphabet.Length; i++)
		//    {
		//        word = alphabet[i].ToString();
		//        _browser.PressKeys(IdentifierType.Id, "txtBox2", word, "input");
		//        result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox2", "innerHtml", "div");
		//        if (!result.Equals("keydown"))
		//        {
		//            if (!isBroken)
		//                isBroken = true;
		//            errorMessage.Append(word + " ");
		//        }
		//        _browser.SetElementAttribute(IdentifierType.Id, "txtBox2", "value", "", "input");
		//        _browser.SetElementAttribute(IdentifierType.Id, "divTxtBox2", "innerHtml", "", "div");
		//    }

		//    Assert.AreEqual(false, isBroken, errorMessage.ToString());
		//}

		[Test]
		public void PressKeysNumbersOnKeyPressTest()
		{
			char[] alphabet = "0123456789".ToCharArray();
			string word, result;
			StringBuilder errorMessage = new StringBuilder("KeyPress isn't triggering because of KeyUp/KeyDown Errors: ");
			bool isBroken = false;

			for (int i = 0; i < alphabet.Length; i++)
			{
				word = alphabet[i].ToString();
				_browser.PressKeys(IdentifierType.Id, "txtBox3", word, "input");
				result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox3", "innerHtml", "div");
				if (!result.Equals("keypress"))
				{
					isBroken = true;
					errorMessage.Append(word + " ");
				}
				_browser.SetElementAttribute(IdentifierType.Id, "txtBox3", "value", "", "input");
				_browser.SetElementAttribute(IdentifierType.Id, "divTxtBox3", "innerHtml", "", "div");
			}

			Assert.AreEqual(false, isBroken, errorMessage.ToString());
		}

		//[Test]
		//public void TestPressKeysAlphabetLowercaseOnKeyUp()
		//{
		//    TestSetup();

		//    char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
		//    string word, result;
		//    StringBuilder errorMessage = new StringBuilder("KeyUp Errors: ");
		//    bool isBroken = false;

		//    for (int i = 0; i < alphabet.Length; i++)
		//    {
		//        word = alphabet[i].ToString();
		//        _browser.PressKeys(IdentifierType.Id, "txtBox1", word, "input");
		//        result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox1", "innerHtml", "div");
		//        if (!result.Equals("keyup"))
		//        {
		//            if (!isBroken)
		//                isBroken = true;
		//            errorMessage.Append(word + " ");
		//        }
		//        _browser.SetElementAttribute(IdentifierType.Id, "txtBox1", "value", "", "input");
		//        _browser.SetElementAttribute(IdentifierType.Id, "divTxtBox1", "innerHtml", "", "div");
		//    }

		//    Assert.AreEqual(false, isBroken, errorMessage.ToString());
		//}

		//[Test]
		//public void TestPressKeysAlphabetLowercaseOnKeyDown()
		//{
		//    TestSetup();

		//    char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
		//    string word, result;
		//    StringBuilder errorMessage = new StringBuilder("KeyDown Errors: ");
		//    bool isBroken = false;

		//    for (int i = 0; i < alphabet.Length; i++)
		//    {
		//        word = alphabet[i].ToString();
		//        _browser.PressKeys(IdentifierType.Id, "txtBox2", word, "input");
		//        result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox2", "innerHtml", "div");
		//        if (!result.Equals("keydown"))
		//        {
		//            if (!isBroken)
		//                isBroken = true;
		//            errorMessage.Append(word + " ");
		//        }
		//        _browser.SetElementAttribute(IdentifierType.Id, "txtBox2", "value", "", "input");
		//        _browser.SetElementAttribute(IdentifierType.Id, "divTxtBox2", "innerHtml", "", "div");
		//    }

		//    Assert.AreEqual(false, isBroken, errorMessage.ToString());
		//}

		[Test]
		public void PressKeysAlphabetLowercaseOnKeyPressTest()
		{
			char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
			string word, result;
			StringBuilder errorMessage = new StringBuilder("KeyPress isn't triggering because of KeyUp/KeyDown Errors: ");
			bool isBroken = false;

			for (int i = 0; i < alphabet.Length; i++)
			{
				word = alphabet[i].ToString();
				_browser.PressKeys(IdentifierType.Id, "txtBox3", word, "input");
				result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox3", "innerHtml", "div");
				if (!result.Equals("keypress"))
				{
					if (!isBroken)
						isBroken = true;
					errorMessage.Append(word + " ");
				}
				_browser.SetElementAttribute(IdentifierType.Id, "txtBox3", "value", "", "input");
				_browser.SetElementAttribute(IdentifierType.Id, "divTxtBox3", "innerHtml", "", "div");
			}

			Assert.AreEqual(false, isBroken, errorMessage.ToString());
		}

		//[Test]
		//public void TestPressKeysAlphabetUppercaseOnKeyUp()
		//{
		//    TestSetup();

		//    char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
		//    string word, result;
		//    StringBuilder errorMessage = new StringBuilder("KeyUp Errors: ");
		//    bool isBroken = false;

		//    for (int i = 0; i < alphabet.Length; i++)
		//    {
		//        word = alphabet[i].ToString();
		//        _browser.PressKeys(IdentifierType.Id, "txtBox1", word, "input");
		//        result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox1", "innerHtml", "div");
		//        if (!result.Equals("keyup"))
		//        {
		//            if (!isBroken)
		//                isBroken = true;
		//            errorMessage.Append(word + " ");
		//        }
		//        _browser.SetElementAttribute(IdentifierType.Id, "txtBox1", "value", "", "input");
		//        _browser.SetElementAttribute(IdentifierType.Id, "divTxtBox1", "innerHtml", "", "div");
		//    }

		//    Assert.AreEqual(false, isBroken, errorMessage.ToString());
		//}

		//[Test]
		//public void TestPressKeysAlphabetUppercaseOnKeyDown()
		//{
		//    TestSetup();

		//    char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
		//    string word, result;
		//    StringBuilder errorMessage = new StringBuilder("KeyDown Errors: ");
		//    bool isBroken = false;

		//    for (int i = 0; i < alphabet.Length; i++)
		//    {
		//        word = alphabet[i].ToString();
		//        _browser.PressKeys(IdentifierType.Id, "txtBox2", word, "input");
		//        result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox2", "innerHtml", "div");
		//        if (!result.Equals("keydown"))
		//        {
		//            if (!isBroken)
		//                isBroken = true;
		//            errorMessage.Append(word + " ");
		//        }
		//        _browser.SetElementAttribute(IdentifierType.Id, "txtBox2", "value", "", "input");
		//        _browser.SetElementAttribute(IdentifierType.Id, "divTxtBox2", "innerHtml", "", "div");
		//    }

		//    Assert.AreEqual(false, isBroken, errorMessage.ToString());
		//}

		[Test]
		public void PressKeysAlphabetUppercaseOnKeyPressTest()
		{
			char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
			string word, result;
			StringBuilder errorMessage = new StringBuilder("KeyPress isn't triggering because of KeyUp/KeyDown Errors: ");
			bool isBroken = false;

			for (int i = 0; i < alphabet.Length; i++)
			{
				word = alphabet[i].ToString();
				_browser.PressKeys(IdentifierType.Id, "txtBox3", word, "input");
				result = _browser.GetElementAttribute(IdentifierType.Id, "divTxtBox3", "innerHtml", "div");
				if (!result.Equals("keypress"))
				{
					isBroken = true;
					errorMessage.Append(word + " ");
				}
				_browser.SetElementAttribute(IdentifierType.Id, "txtBox3", "value", "", "input");
				_browser.SetElementAttribute(IdentifierType.Id, "divTxtBox3", "innerHtml", "", "div");
			}

			Assert.AreEqual(false, isBroken, errorMessage.ToString());
		}

		[Test]
		public void TestPressKeysInsideIFrame()
		{
			_browser.PressKeys(IdentifierType.Id, "txtBox4", "inside", "input");
			string result = _browser.GetElementAttribute(IdentifierType.Id, "txtBox4", "value", "input");
			Assert.AreEqual("inside", result, "PressKeys is not working correctly inside an IFrame.");
		}

		[Test]
		public void TestPressKeysOutsideIFrame()
		{
			_browser.PressKeys(IdentifierType.Id, "txtPressKey", "outside", "input");
			string result = _browser.GetElementAttribute(IdentifierType.Id, "txtPressKey", "value", "input");
			Assert.AreEqual("outside", result, "PressKeys is not working correctly outside an IFrame.");
		}

		[Test]
		public void TestPressKeysInGoogle()
		{
			try
			{
				_browser.NavigateBrowser(getTestPage("GoogleTestPage.htm"));

				_browser.PressKeys(IdentifierType.Name, "q", "what by id");
				_browser.AssertElementExists(IdentifierType.Expression, "name:q;value:what by id", "input");

				_browser.PressKeys(IdentifierType.Expression, "name:q", "what by name");
				_browser.AssertElementExists(IdentifierType.Expression, "name:q;value:what by name", "input");

				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.PressKeys(IdentifierType.Name, "q", "\\{BACKSPACE\\}");
				_browser.AssertElementExists(IdentifierType.Expression, "name:q;value:what by id", "input");
			}
			finally
			{
				// Clean up
				this.NavigateToSwatTestPage();
			}
		}

		//[Test]
		//public void TestPressKeysEnterUpDownArrowKeys()
		//{
		//    TestSetup();

		//    bool exception = false;

		//    _browser.PressKeys(IdentifierType.Id, "txtBox1", "\\{ALT\\}");
		//    _browser.PressKeys(IdentifierType.Id, "txtBox1", "f");
		//    _browser.PressKeys(IdentifierType.Id, "txtBox1", "\\{UP_ARROW\\}");
		//    _browser.PressKeys(IdentifierType.Id, "txtBox1", "\\{DOWN_ARROW\\}");
		//    _browser.PressKeys(IdentifierType.Id, "txtBox1", "\\{UP_ARROW\\}");
		//    _browser.PressKeys(IdentifierType.Id, "txtBox1", "\\{ENTER\\}");


		//    try
		//    {
		//        _browser.AssertBrowserDoesNotExist("SWAT Test Page");
		//    }
		//    catch
		//    {
		//        exception = true;
		//    }

		//    AttachToOriginalWindow();

		//    Assert.AreEqual(false, exception, "The UP, DOWN, ENTER, or 'Alt' key is not triggering properly.");
		//}

		//[Test]
		//public void TestPressKeysAltKey()
		//{
		//    TestSetup();

		//    bool exception = false;

		//    _browser.PressKeys(IdentifierType.Id, "txtBox1", "\\{ALT\\}");
		//    _browser.PressKeys(IdentifierType.Id, "txtBox1", "f");
		//    _browser.PressKeys(IdentifierType.Id, "txtBox1", "x");


		//    try
		//    {
		//        _browser.AssertBrowserDoesNotExist("SWAT Test Page");
		//    }
		//    catch
		//    {
		//        exception = true;
		//    }

		//    AttachToOriginalWindow();

		//    Assert.AreEqual(false, exception, "The 'Alt' key is not triggering properly.");
		//}

		#endregion
		
	}
}
