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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace SWAT_Editor
{
    public partial class ReportBugForm : Form
    {
        delegate void SetLabelPropertiesCallback(string Text);
        delegate void SetButtonPropertiesCallback();

        #region Constructor
        public ReportBugForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                //disable 'send' button
                btnSend.Enabled = false;

                //check connection
                if (!isConnected())
                    throw new Exception("Email cannot be sent. Please check your connection");

                //validating email address of the sender
                Regex emailValidation = new Regex("[a-z][\\w\\.-]*[a-z\\d]@[a-z\\d]*[\\w\\.-]*[a-z\\d]\\.[a-z][a-z\\.]*[a-z]", RegexOptions.IgnoreCase);
                Match m = emailValidation.Match(txtFrom.Text);
                if (!m.Success)
                    throw new EmailFormatException("Enter a valid e-mail address on the 'From' field");

                //validating that the subject and body of the email are not empty
                if (txtSubject.Text.Equals("") || txtBody.Text.Equals(""))
                    throw new Exception("The subject and/or body of the email cannot be empty");

                string from = txtFrom.Text;
                string to = "aubriel_leyva@ultimatesoftware.com";// lblTo.Text.Substring(14);
                string subject = txtSubject.Text;
                string body = txtBody.Text;

                MailMessage mail = new MailMessage(from, to, subject, body);
                mail.IsBodyHtml = false;

                SmtpClient client = new SmtpClient("localhost");
                client.Credentials = CredentialCache.DefaultNetworkCredentials;
                client.Send(mail);
                this.Close();
                mail.Dispose();

                MessageBox.Show("Your bug report has been sent",
                    "Successfully Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (EmailFormatException ef)
            {
                displayErrorMsgOkButton(ef.Message, "Wrong Email Format");
                txtFrom.Focus();
                txtFrom.Select(0, txtFrom.TextLength);
            }
            catch (SmtpFailedRecipientException sf)
            {
                displayErrorMsgOkButton(sf.Message + "\nPlease try the following or contact your system administrator\n\n" +
                    "1) Go to Start - Control Panel,\n" +
                    "2) Go to Administrative tools,\n" +
                    "3) Go to Internet Information Services,\n" +
                    "4) Expand your Local Computer,\n" +
                    "5) On your default SMTP, right click and select Properties,\n" +
                    "6) On the Access tab, select Relay,\n" +
                    "7) Click Add on the window that appears,\n" +
                    "8) Under IP Address, enter 10.50.52.136 and click OK three times.", "Connection Error");
            }
            catch (SmtpException ex)
            {
                displayErrorMsgOkButton(ex.StatusCode.ToString(), "Exception");
            }
            catch (Exception ex)
            {
                handleException(ex.Message);
            }
            finally
            {
                //enable 'send' button
                btnSend.Enabled = true;
            }

        }

        private void handleException(string message)
        {
            displayErrorMsgOkButton(message, "Error");

            if (txtSubject.TextLength == 0)
                txtSubject.Focus();
            else
                txtBody.Focus();
        }

        //checking connection
        private bool isConnected()
        {
            try
            {
                //get content from wiki helpemail
                HttpWebRequest emailFromWiki =
                    (HttpWebRequest)WebRequest.Create("http://www.yahoo.com/");
                HttpWebResponse theEmail = (HttpWebResponse)emailFromWiki.GetResponse();

                theEmail.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //display errors
        public void displayErrorMsgOkButton(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void load(Form f, string whichForm)
        {
            this.Text = whichForm;

            if (whichForm.Equals("Report A Bug"))
            {
                this.Show(f);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void getEmailToSendBug()
        {
            try
            {
                //get content from wiki helpemail
                HttpWebRequest emailFromWiki =
                    (HttpWebRequest)WebRequest.Create("http://ulti-swat.wiki.sourceforge.net/helpemail");
                HttpWebResponse theEmail = (HttpWebResponse)emailFromWiki.GetResponse();
                
                //read the stream returned by the web
                StreamReader readEmail = new StreamReader(theEmail.GetResponseStream(), System.Text.Encoding.ASCII);
                string email = readEmail.ReadToEnd();

                //trying to find the email
                Regex findEmail = new Regex("mailto:[^\"]*");
                Match m = findEmail.Match(email);

                //set the email from the wiki as the lblTo text
                setLblToProperties(m.ToString().Substring(7));

                //closing connections and reader
                theEmail.Close();
                readEmail.Close();
            }
            catch(Exception)
            {
                //if there's an exception, then send email to a fixed email address
                //instead of the email from the wiki, and set the lblTo lable to that address
                setLblToProperties("DEV_FRG_swat_framework@ultimatesoftware.com");
            }
            finally
            {
                //enable the 'send' button and change text to 'send'
                setBtnSendProperties();
            }
        }

        //avoid problems with crossing threads while setting 'send' btn properties
        private void setBtnSendProperties()
        {
            if (txtBody.InvokeRequired)
            {
                SetButtonPropertiesCallback stc = new SetButtonPropertiesCallback(setBtnSendProperties);
                this.Invoke(stc, new object[] { });
            }
            else
            {
                btnSend.Enabled = true;
                btnSend.Text = "Send";
            }
        }

        //avoid problems with crossing threads while setting 'to' lbl properties
        private void setLblToProperties(string str)
        {
            if (txtBody.InvokeRequired)
            {
                SetLabelPropertiesCallback slp = new SetLabelPropertiesCallback(setLblToProperties);
                this.Invoke(slp, new object[] { str });
            }
            else
                lblTo.Text = "To:           " + str;
        }

        private void ReportBugForm_Load(object sender, EventArgs e)
        {
            ThreadStart ts = new ThreadStart(getEmailToSendBug);
            Thread theThread = new Thread(ts);
            theThread.Start();
        }
        #endregion

        #region Exceptions

        public class EmailFormatException : Exception
        {
            public EmailFormatException(string message) : base(message)
            {
            }
        }

        #endregion
    }
}
