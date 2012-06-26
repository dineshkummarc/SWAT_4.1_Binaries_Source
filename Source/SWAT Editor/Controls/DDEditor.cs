using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using SWAT;

namespace SWAT_Editor.Controls
{
    public partial class DDEditor : UserControl
    {
        private int _startIndex;
        private string _testName;
        private string _resultPath;
        private bool _ddeTestPassed;
        private string _ddeFailureReason;
        private int _totalTestsPassed;
        private int _totalTestsFailed;
        private List<CommandResult> _testsRun;
        private int _unnamedTests = 0;
        private ResultHandler _resultHandler;
        private string _tempPath = "C:\\DDETemp.txt";
        private Queue<string> _allTestNames;
        private int _testNum = 0;
        private const string _testFront = "Test: ";

        #region Constructors

        public DDEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region Accessors

        public int StartIndex
        {
            get { return _startIndex; }
            set { _startIndex = value; }
        }

        public string TestName
        {
            get { return _testName; }
            set { _testName = value; }
        }

        public string ResultPath
        {
            get { return _resultPath; }
            set { _resultPath = value; }
        }

        public bool TestPassed
        {
            get { return _ddeTestPassed; }
            set { _ddeTestPassed = value; }
        }

        public string FailureReason
        {
            get { return _ddeFailureReason; }
            set { _ddeFailureReason = value; }
        }

        public int TotalTestsPassed
        {
            get { return _totalTestsPassed; }
            set { _totalTestsPassed = value; }
        }

        public int TotalTestsFailed
        {
            get { return _totalTestsFailed; }
            set { _totalTestsFailed = value; }
        }

        public bool OverrideVariables
        {
            get { return overrideFitnesseChkBox.Checked; }
        }

        public List<CommandResult> TestsRun
        {
            get { return _testsRun; }
            set { _testsRun = value; }
        }

        public ResultHandler ResultHandler
        {
            get { return _resultHandler; }
            set { _resultHandler = value; }
        }

        public string TempPath
        {
            get { return _tempPath; }
        }

        public Queue<string> AllTestNames
        {
            get { return _allTestNames; }
            set { _allTestNames = value; }
        }

        public int TestNum
        {
            get { return _testNum; }
            set { _testNum = value; }
        }

        public string TestFront
        {
            get { return _testFront; }
        }

        public bool Active
        {
            get { return this.Visible; }
        }

        #endregion

        #region Events

        private void DestinationTextBox_TextChanged(object sender, EventArgs e)
        {
            OutputFolderIsValid();
        }

        private void XMLTextBox_TextChanged(object sender, EventArgs e)
        {
            XMLIsValid();
        }

        private void TestFileTextBox_TextChanged(object sender, EventArgs e)
        {
            TestFileIsValid();
        }

        private void xMLBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                XMLTextBox.Text = ofd.FileName;
        }

        private void testFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                TestFileTextBox.Text = ofd.FileName;
        }

        private void destinationBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();

            if (result == DialogResult.OK)
                DestinationTextBox.Text = folderDialog.SelectedPath;
        }

        #endregion

        #region Internal Helpers

        private bool XMLIsValid()
        {
            bool valid;
            if (File.Exists(XMLTextBox.Text))
            {
                xmlFileErrorProvider.SetError(this.XMLTextBox, "");
                valid = true;
            }
            else
            {
                xmlFileErrorProvider.SetError(this.XMLTextBox, "Choose a valid XML file");
                valid = false;
            }

            return valid;
        }

        private bool TestFileIsValid()
        {
            bool valid;
            if (File.Exists(TestFileTextBox.Text))
            {
                testFileErrorProvider.SetError(this.TestFileTextBox, "");
                valid = true;
            }
            else
            {
                testFileErrorProvider.SetError(this.TestFileTextBox, "Choose a valid test file");
                valid = false;
            }

            return valid;
        }

        private bool OutputFolderIsValid()
        {
            bool valid;
            if (Directory.Exists(DestinationTextBox.Text))
            {
                destinationErrorProvider.SetError(this.DestinationTextBox, "");
                valid = true;
            }
            else
            {
                destinationErrorProvider.SetError(this.DestinationTextBox, "Choose a valid output folder");
                valid = false;
            }

            return valid;
        }

        private string varReplaceStopPoint(string completeTest, string variable)
        {
            return System.Text.RegularExpressions.Regex.Split(completeTest, @"!define[\s]" + variable)[0];
        }

        private void replaceVariables(string variable, string value)
        {
            if (variable == null || value == null)
                return;

            string original;
            using (StreamReader sr = new StreamReader(TempPath))
            {
                original = sr.ReadToEnd();
            }
                
            string test = OverrideVariables ? original : varReplaceStopPoint(original, variable);
            int length = test.Length;

            while (test.Contains("${" + variable + "}"))
            {
                test = test.Replace("${" + variable + "}", value);
            }
            test = test + original.Substring(length);
            File.WriteAllText(TempPath, test);
        }

        private string getTestStatistics()
        {
            return "Passed:\t" + _resultHandler.TotalRight + " Wrong:\t" + _resultHandler.TotalWrong + " Ignored:\t" + _resultHandler.TotalIgnored;
        }

        #endregion

        #region Public Methods

        public bool canStart()
        {
            if (XMLIsValid() & TestFileIsValid() & OutputFolderIsValid())
                return true;

            return false;
        }

        public XmlTextReader getXMLFileContent()
        {
            return new XmlTextReader(XMLTextBox.Text);
        }

        public TextReader getTestFileContent()
        {
            return new StreamReader(TestFileTextBox.Text);
        }

        public string getDestinationFolder()
        {
            return DestinationTextBox.Text;
        }

        public void replaceVariables(XmlTextReader xml)
        {
            string varName;
            string varValue;

            while (xml.LocalName.Equals("Variable"))
            {
                xml.Read();
                varName = getChosenElement(xml, "name");
                varValue = getChosenElement(xml, "value");
                replaceVariables(varName, varValue);
                try { xml.ReadEndElement(); }//Variable
                catch (XmlException e) { break; };
            }
        }

        public void concludeTest(CommandResult displayName)
        {
            displayName.FullCommand = TestName;
            displayName.Message = getTestStatistics();
            TestsRun.Add(new CommandResult(displayName));
            displayName = new CommandResult();
        }

        public void createSummaryFile(BrowserType browser)
        {
            DateTime timeNow = DateTime.Now;
            string fileFormat = string.Format("Date {0}_{1}_{2} Time {3}_{4}_{5}", timeNow.Month, timeNow.Day, timeNow.Year, timeNow.Hour, timeNow.Minute, timeNow.Second);

            ResultHandler = new ResultHandler("TestSummary_" + DateTime.Now, browser, TestsRun);
            ResultHandler.SaveResultsAsHtml(ResultPath + "TestSummary_" + fileFormat + ".html");
        }

        #endregion

        #region Helper Methods

        public void ResetTotalTestStatistics()
        {
            TotalTestsPassed = 0;
            TotalTestsFailed = 0;
            _unnamedTests = 0;
            TestsRun = new List<CommandResult>();
        }

        public void ResetDataForNextTest()
        {
            FailureReason = null;
            TestPassed = true;
            TestName = AllTestNames.Dequeue();
            ResultPath = getDestinationFolder().TrimEnd(new char[] {'\\'}) + "\\";
        }

        public string determineTestName(XmlTextReader xml)
        {
            xml.MoveToAttribute("tcid");
            string name = xml.Value;
            return name != string.Empty ? name : "Untitled" + _unnamedTests++;
        }

        public bool determineTestEnabled(XmlTextReader xml)
        {
            xml.MoveToAttribute("enabled");
            string enabled = xml.Value;
            return (enabled.ToLower().Equals("true") || enabled.ToLower().Equals("false")) ? Boolean.Parse(enabled) : false;
        }

        public string getChosenElement(XmlTextReader xml, string element)
        {
            string content;
            try
            {
                xml.ReadStartElement(element);
                content = xml.ReadContentAsString();
                xml.ReadEndElement();
            }
            catch (XmlException e)
            {
                content = null;
            }
            return content;
        }

        public void incrementPassFailCounter()
        {
            if (TestPassed)
                TotalTestsPassed++;
            else
                TotalTestsFailed++;
        }

        #endregion
    }
}
