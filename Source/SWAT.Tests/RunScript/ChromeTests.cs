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

namespace SWAT.Tests.RunScript
{
    [TestFixture]
    [Category("Chrome")]
    public class ChromeTests : RunScriptTests
    {
        public ChromeTests()
            : base(BrowserType.Chrome)
        {

        }

        [Test]
        public void runJavaScriptTest_frameIteration_withDoubleQuotes()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                Assert.That(_browser.RunScriptSaveResult("javascript", "new Function(\"var count=0; for (var i=0;i<self.frames.length;i++) {count++;} return 'count: ' + count;\")()").Contains("count: 2"));
            }
            finally
            {
                // cleanup
                NavigateToSwatTestPage();
            }
        }

        [Test]
        public void runJavaScriptTest_iFrames()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                string frameCount = _browser.RunScriptSaveResult("javascript", "new Function(\"var coll = [], oIframe, IFrameDoc, el, i = 0, j; var all_IFrames = document.getElementsByTagName('iframe'); i = 0; while (oIframe = all_IFrames.item(i++)) { if (oIframe.contentDocument){IFrameDoc = oIframe.contentDocument; }else if (oIframe.contentWindow) {IFrameDoc = oIframe.contentWindow.document;}else if (oIframe.document) {IFrameDoc = oIframe.document;} coll = IFrameDoc.getElementsByTagName('input'), j = 0; while(el = coll.item(j++)){el.value = 'Frame: ' + i + ' Element: ' + j;}}return all_IFrames.length;\")()");
                Assert.That(frameCount.Contains("2"));
            }
            finally
            {
                // cleanup
                NavigateToSwatTestPage();
            }
        } 
    }
}
