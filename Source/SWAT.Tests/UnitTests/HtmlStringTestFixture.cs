using System;
using NUnit.Framework;
using SWAT.Fitnesse;
using System.Text;
using SWAT.Reflection;

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("Misc")]
    public class HtmlStringTestFixture
    {
        [Test]
        public void ToPlainTextWorksWithQuotesTest()
        {
            string theHTML = "“”";
            string result = new HtmlString(theHTML).ToPlainText();

            Assert.AreEqual(result, "“”");
        }

        [Test]
        public void ToPlainTextWorksWithApostrophesTest()
        {
            string theHTML = "‘’";
            string result = new HtmlString(theHTML).ToPlainText();

            Assert.AreEqual(result, "‘’");
        }

        [Test]
        public void ToPlainTextWorksWithHTMLTagsTest()
        {
            string theHTML = "<html><head><body><table><tr><td><div><span>test</span></div></td></tr></table></body></head></html>";
            string result = new HtmlString(theHTML).ToPlainText();

            Assert.AreEqual(result, "test");
        }

        [Test]
        public void ToPlainTextWorksWithWhitespaceTest()
        {
            string theHTML = " ";
            string result = new HtmlString(theHTML).ToPlainText();

            Assert.AreEqual(result, string.Empty);
        }

        [Test]
        public void ToPlainTextWorksWithAmpersandHTMLTagsTest()
        {
            string theHTML = "&lt;head&gt;&lt;body&gt;&amp;&nbsp;test&quot;&lt;/body&gt;&lt;/head&gt;";
            string result = new HtmlString(theHTML).ToPlainText();
            System.Console.WriteLine(result);

            Assert.AreEqual(result, "<head><body>& test\"</body></head>");
        }

        [Test]
        public void ToPlainTextWorksWithUnicodeSpaceTest()
        {
            string theHTML = "Test\u00a0Unicode";
            string result = new HtmlString(theHTML).ToPlainText();

            Assert.AreEqual(result, "Test Unicode");
        }

        [Test]
        public void ToPlainTextWorksWithSpecialCharactersTest()
        {
            string theHTML = ")(*&^%$<>#@!!~`:;\"'><.,/?{}[]|\\";
            string result = new HtmlString(theHTML).ToPlainText();
            System.Console.WriteLine(result);

            Assert.AreEqual(result, ")(*&^%$<>#@!!~`:;\"'><.,/?{}[]|\\");
        }

        
        [Test]
        public void ToPlainTextWorksWithFitNesseStandardVersionTest()
        {
            fitSharp.Parser.HtmlString.IsStandard = true; 
            string theHTML = "  <html>&lt;head&gt;&lt;body&gt;&amp;</p  ><p>   &nbsp;t<br>&nbsp;e&nbsp;s&nbsp;t&nbsp;&quot;&lt;/body&gt;&lt;/head&gt;</html>  ";
            string result = new HtmlString(theHTML).ToPlainText();

            Assert.AreEqual(result, "<head><body>&\n  t\n e s t \"</body></head>");
        }
        

        [Test]
        public void UnEscapeWorksWhenNBSPIsPassedAsParameterTest()
        {
            string html = "&nbsp;";
            HtmlString htmlString = new HtmlString(html);
            string result = ReflectionHelper.InvokeMethod<string>(htmlString, "UnEscape", html);

            Assert.AreEqual(" ", result);
        }

        [Test]
        public void TestOutputFormatsParagraphTagsCorrectlyTest()
        {
            StringBuilder myText = new StringBuilder("<");
            string myLastTag = "/p ";
            string input = "p";
            string expected = "<br />";

            TextOutput textOutput = new TextOutput();
            ReflectionHelper.SetField(textOutput, "myText", myText);

            ReflectionHelper.SetField(textOutput, "myLastTag", myLastTag);
            textOutput.AppendTag(input);

            StringBuilder result = (StringBuilder)ReflectionHelper.GetField<object>(textOutput, "myText");

            Assert.AreEqual(expected, result.ToString());
        }
    }
}
