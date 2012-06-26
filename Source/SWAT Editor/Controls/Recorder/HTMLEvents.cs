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


/*
 * Created by SharpDevelop.
 * User: Jared
 * Date: 9/15/2007
 * Time: 10:01 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using mshtml;
using SHDocVw;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using SWAT_Editor.Recorder;
using System.Threading;
using System.Collections;
using System.Windows.Forms;




namespace SWAT_Editor.Recorder
{

    public delegate void UpdateUIHandler(string newCommands);

    /// <summary>
    /// Description of HTMLEvents.
    /// </summary>
    public class HTMLEvents : HTMLDocumentEvents2
    {
        SWAT.InternetExplorer ie2 = new SWAT.InternetExplorer();
        public static Boolean enableEvents = true;
        IGenerator _builder; //Script builder
        SHDocVw.InternetExplorer ie = null;
        static StringBuilder _sb = new StringBuilder();
        public static int lastBrowser = 1; //Last browser used
        IHTMLDocument2 document = null;
        public static Boolean supress = false; //Supress writing of NavigateBrowser
        private IConnectionPoint pConPt = null;
        public static IHTMLElement _previousClickedElement = null;
        public static HtmlElement _previousSelectElement = null;
        public Guid m_guid = Guid.Empty;
        private int m_dwCookie = 0;
        public int[] m_dispIds = null;
        private static Boolean _AssertElement = false;

        public static Boolean AssertElement
        {
            get { return _AssertElement; }
            set { _AssertElement = value; }
        }

        public HTMLEvents()
        {

        }

        public HTMLEvents(IGenerator builder, object i, SHDocVw.InternetExplorer browser)
        {

            ie = browser;
            document = (IHTMLDocument2)i;
            _builder = builder;
            ConnectToHtmlEvents();
            //_builder.OnAfterWriteCommand+=new OnAfterGenerateCommandEventHandler(processCommand);
            //_uiHandler = uiHandler;
            //_builder.OnFinishedWritingBatchOfCommands+=new EventHandler(updateUI);

        }

        //elem = IHTMLElement
        public void ConnectToHtmlEvents()
        {

            //Funky, yes, with a reason: http://support.microsoft.com/?id=811645
            // Help from http://www.eggheadcafe.com/ng/microsoft.public.dotnet.framework.sdk/post21853543.asp
            // We are going to sink the event here by using COM connection point.  
            HTMLDocument doc = (HTMLDocument)document;
            // I am going to QueryInterface UCOMConnectionPointContainer of the WebBrowser Control  
            System.Runtime.InteropServices.ComTypes.IConnectionPointContainer pConPtCon = (System.Runtime.InteropServices.ComTypes.IConnectionPointContainer)doc;

            // Get access to the IConnectionPoint pointer.  
            // UCOMIConnectionPoint pConPt;  
            // Get the GUID of the HTMLDocumentEvents2  
            Guid guid = typeof(HTMLDocumentEvents2).GUID;
            pConPtCon.FindConnectionPoint(ref guid, out pConPt);
            // define your event handler class IEHTMLDocumentEvents  
            pConPt.Advise(this, out m_dwCookie);
        }

        public bool DisconnectHtmlEvents()
        {
            bool bRet = false;
            //Do we have a connection point
            if (pConPt != null)
            {
                if (m_dwCookie > 0)
                {
                    try
                    {
                        pConPt.Unadvise(m_dwCookie);
                    }
                    catch (Exception) { }
                    m_dwCookie = 0;
                    bRet = true;
                }
                Marshal.ReleaseComObject(pConPt);
            }
            return bRet;
        }


        private void processCommand(string command)
        {
            _sb.AppendLine(command);
        }

        private void updateUI(object sender, EventArgs args)
        {
            //_uiHandler(_sb.ToString());
        }

        public string Script
        {
            get { return _sb.ToString(); }
        }

        HtmlElement processElement(IHTMLElement elem)
        {
            HtmlElement element = new HtmlElement();
            element.Id = elem.id;
            element.InnerHtml = elem.innerHTML;
            element.TagName = elem.tagName;

            return element;
        }

        private void checkAttach()
        {
            if ((int)ie.GetProperty("ID") != lastBrowser)
            {
                if (String.IsNullOrEmpty(ie.LocationName)) return;
                int index = 0;
                string title = ((mshtml.HTMLDocument)ie.Document).title;
                ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
                foreach (SHDocVw.InternetExplorer Browser in m_IEFoundBrowsers)
                {
                    if (Browser.Document is mshtml.HTMLDocument)
                    {
                        if (((HTMLDocument)(Browser.Document)).title.Equals(title))
                        {
                            if ((Browser.GetProperty("ID") != null))
                            {
                                if (((int)Browser.GetProperty("ID")) == ((int)ie.GetProperty("ID"))) break;
                            }
                            index++;
                        }
                    }
                }
                _builder.AttachBrowser(ie.LocationName, index);
                lastBrowser = (int)ie.GetProperty("ID");
                _previousClickedElement = null;
            }
        }

        bool HTMLDocumentEvents2.onclick(IHTMLEventObj pEvtObj)
        {
            
            SWAT_Editor.Recorder.WebBrowserEvents.resetEvents = true;
            IHTMLElement srcElement = pEvtObj.srcElement;
            Debug.WriteLine(pEvtObj.type + " - " + srcElement.tagName);
            HtmlElement element = new HtmlElement();

            if ((srcElement is HTMLSelectElement) && !AssertElement)
            {

                element.Id = (srcElement as HTMLSelectElement).id;
                element.InnerHtml = (srcElement as HTMLSelectElement).innerHTML;
                element.TagName = (srcElement as HTMLSelectElement).tagName;
                element.Name = (srcElement as HTMLSelectElement).name;
                element.Value = (srcElement as HTMLSelectElement).value;
                element.SelectedIndex = (srcElement as HTMLSelectElement).selectedIndex.ToString();
                if (element.Equals(_previousSelectElement))
                {
                    if (!string.IsNullOrEmpty(element.Value) && !element.Value.Equals(_previousSelectElement.Value))
                    {
                        _builder.SetElementProperty(element, "Value");
                        //_builder.StimulateElement(element, "onchange");
                    }
                    else if (!string.IsNullOrEmpty(element.SelectedIndex) && !element.SelectedIndex.Equals(_previousSelectElement.SelectedIndex))
                    {
                        _builder.SetElementProperty(element, "selectedIndex");
                        //_builder.StimulateElement(element, "onchange");
                    }
                }
                _previousSelectElement = element;
                _previousClickedElement = null;

            }

            if ((srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString().Contains("return false")) return false;

            //Debug.WriteLine(pEvtObj.type);
            //supress = true;
            //if (_previousClickedElement != null && pEvtObj.srcElement.contains(_previousClickedElement))
            //    return _AssertElement ? false : true; //we want to ignore events that bubble up

            //Debug.WriteLine("Onclick: " + pEvtObj.srcElement.GetType().ToString());
            //IHTMLElement srcElement = pEvtObj.srcElement;



            //HtmlElement element = new HtmlElement();

            //if (srcElement is HTMLInputElement)
            //{
            //    if ((srcElement as HTMLInputElement).type.Equals("text") && !AssertElement) return true;
            //    element.id = (srcElement as HTMLInputElement).id;
            //    element.innerHtml = (srcElement as HTMLInputElement).innerHTML;
            //    element.tagName = (srcElement as HTMLInputElement).tagName;
            //    element.name = (srcElement as HTMLInputElement).name;
            //    element.value = (srcElement as HTMLInputElement).value;
            //    element.onclick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            //}
            //else if (srcElement is HTMLAnchorElement)
            //{
            //    element.id = (srcElement as HTMLAnchorElement).id;
            //    element.innerHtml = (srcElement as HTMLAnchorElement).innerHTML;
            //    element.tagName = (srcElement as HTMLAnchorElement).tagName;
            //    element.name = (srcElement as HTMLAnchorElement).name;
            //    element.href = (srcElement as HTMLAnchorElement).href;
            //    element.onclick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            //}
            //else if (srcElement is IHTMLElement)
            //{
            //    element.id = (srcElement as IHTMLElement).id;
            //    element.innerHtml = (srcElement as IHTMLElement).innerHTML;
            //    element.tagName = (srcElement as IHTMLElement).tagName;
            //    if (element.tagName.Equals("BODY")) return false;
            //    element.href = string.Empty;
            //    element.onclick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            //} else return false;
            //_previousClickedElement = srcElement;
            //HtmlElement element = processElement(pEvtObj.srcElement);


            if (AssertElement)
            {
                //_builder.AssertElement(element);
                return false;
            }
            else
            {
                //_builder.StimulateElement(element, "onclick");
                return true;
            }

        }


        void HTMLDocumentEvents2.onmouseup(IHTMLEventObj pEvtObj)
        {
            Debug.WriteLine(pEvtObj.type + " " + pEvtObj.srcElement.tagName);
            if (pEvtObj.button != 1) return;
            supress = true;
            if (_previousClickedElement != null && !pEvtObj.srcElement.Equals(_previousClickedElement) && pEvtObj.srcElement.contains(_previousClickedElement))
                return; //we want to ignore events that bubble up

            //if (!m_IsCOnnected) return false;
            //Debug.WriteLine("Onclick: " + pEvtObj.srcElement.GetType().ToString());
            IHTMLElement srcElement = pEvtObj.srcElement;

            HtmlElement element = new HtmlElement();

            if (srcElement is HTMLSpanElement)
            {
                String[] ignoreTypes = { "STRONG", "I", "U", "B" };
                for (int i = 0; i < ignoreTypes.Length; i++)
                    if (ignoreTypes[i].Equals(srcElement.tagName))
                        srcElement = srcElement.parentElement;
            }


            if (srcElement is HTMLInputElement)
            {
                if ((srcElement as HTMLInputElement).type.Equals("text") && !AssertElement) return;
                element.Id = (srcElement as HTMLInputElement).id;
                element.InnerHtml = (srcElement as HTMLInputElement).innerHTML;
                element.TagName = (srcElement as HTMLInputElement).tagName;
                element.Name = (srcElement as HTMLInputElement).name;
                element.Value = (srcElement as HTMLInputElement).value;
                element.OnClick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            }
            else if (srcElement is HTMLAnchorElement)
            {
                element.Id = (srcElement as HTMLAnchorElement).id;
                element.InnerHtml = (srcElement as HTMLAnchorElement).innerHTML;
                element.TagName = (srcElement as HTMLAnchorElement).tagName;
                element.Name = (srcElement as HTMLAnchorElement).name;
                element.Href = (srcElement as HTMLAnchorElement).href;
                element.OnClick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            }
            else if (srcElement is HTMLImg)
            {
                element.Id = (srcElement as HTMLImg).id;
                element.InnerHtml = (srcElement as HTMLImg).innerHTML;
                element.TagName = (srcElement as HTMLImg).tagName;
                element.Name = (srcElement as HTMLImg).name;
                element.Href = (srcElement as HTMLImg).href;
                element.OnClick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            }
            else if (srcElement is IHTMLElement)
            {
                element.Id = (srcElement as IHTMLElement).id;
                element.InnerHtml = (srcElement as IHTMLElement).innerHTML;
                element.TagName = (srcElement as IHTMLElement).tagName;
                if (element.TagName.Equals("BODY")) return;
                element.Href = string.Empty;
                element.OnClick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            }
            else return;
            _previousClickedElement = srcElement;
            //HtmlElement element = processElement(pEvtObj.srcElement);


            if (AssertElement)
            {
                mshtml.IHTMLElement htmlElement = null;
                try
                {
                    if (srcElement.tagName.Equals("SPAN") && srcElement.parentElement.tagName.Equals("TD"))
                        htmlElement = srcElement.parentElement.parentElement;
                    else if (srcElement.tagName.Equals("TD"))
                        htmlElement = srcElement.parentElement;
                    else
                        htmlElement = srcElement;
                }
                catch (COMException)
                {
                    MessageBox.Show("Window was closed before assertion data could be loaded");
                }
                if (htmlElement != null)
                {
                    if (!srcElementsAsserting.Contains(htmlElement))
                    {
                        srcElementsAsserting.Add(htmlElement);
                        srcElementFunctions.Add(htmlElement.onclick);
                        htmlElement.onclick = "{}";
                    }
                    if (!asserting)
                    {
                        asserting = true;
                        Thread assertionThread = new Thread(delegate()
                        {
                            AssertionForm form = new AssertionForm(srcElement);
                            enableEvents = false;

                            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                _builder.AssertElement(form.AssertionType, form.Assertion, element);
                            }
                            for (int i = 0; i < srcElementsAsserting.Count; i++)
                            {
                                try
                                {
                                    mshtml.IHTMLElement htmlElementAsserted = srcElementsAsserting[i] as mshtml.IHTMLElement;
                                    object htmlElementFunction = srcElementFunctions[i];
                                    htmlElementAsserted.onclick = htmlElementFunction;
                                }
                                catch (Exception)
                                {
                                }
                            }
                            srcElementsAsserting.Clear();
                            srcElementFunctions.Clear();
                            enableEvents = true;
                            asserting = false;
                        });
                        assertionThread.Start();
                    }
                }
            }
            else
            {
                if (srcElement is IHTMLSelectElement)
                {
                }else
                _builder.StimulateElement(element, "onclick");
            }
        }

        Boolean asserting;
        ArrayList srcElementFunctions = new ArrayList();
        ArrayList srcElementsAsserting = new ArrayList(); 

        void HTMLDocumentEvents2.onmouseout(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            if (!AssertElement) return;

            pEvtObj.srcElement.style.borderWidth = borderWidth;
            pEvtObj.srcElement.style.borderStyle = borderStyle;
            pEvtObj.srcElement.style.borderColor = borderColor;
        }

        string borderColor = string.Empty;
        string borderWidth = string.Empty;
        string borderStyle = string.Empty;

        void HTMLDocumentEvents2.onmouseover(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            if (!AssertElement) return;
            
            borderWidth = (string.IsNullOrEmpty(pEvtObj.srcElement.style.borderWidth)) ? pEvtObj.srcElement.style.borderWidth : String.Empty;
            borderStyle = (string.IsNullOrEmpty(pEvtObj.srcElement.style.borderStyle)) ? pEvtObj.srcElement.style.borderStyle : String.Empty;
            borderColor = (string.IsNullOrEmpty(pEvtObj.srcElement.style.borderColor)) ? pEvtObj.srcElement.style.borderColor : String.Empty;

            pEvtObj.srcElement.style.borderWidth = "thin";
            pEvtObj.srcElement.style.borderStyle = "solid";
            pEvtObj.srcElement.style.borderColor = "blue";
        }

        void HTMLDocumentEvents2.onfocusin(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            supress = true;
            checkAttach();
            IHTMLElement srcElement = pEvtObj.srcElement;
            //Debug.WriteLine(pEvtObj.type + " - " + srcElement.tagName);

            HtmlElement element = new HtmlElement();
            if ((srcElement is HTMLSelectElement) && !AssertElement)
            {
                element.Id = (srcElement as HTMLSelectElement).id;
                element.InnerHtml = (srcElement as HTMLSelectElement).innerHTML;
                element.TagName = (srcElement as HTMLSelectElement).tagName;
                element.Name = (srcElement as HTMLSelectElement).name;
                element.Value = (srcElement as HTMLSelectElement).value;
                element.SelectedIndex = (srcElement as HTMLSelectElement).selectedIndex.ToString();
                _previousSelectElement = element;
            }
            _previousClickedElement = null;
        }


        void HTMLDocumentEvents2.onfocusout(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            supress = false;
            IHTMLElement srcElement = pEvtObj.srcElement;

            HtmlElement element = new HtmlElement();

            if (AssertElement) return;
            if (srcElement is HTMLTextAreaElementClass)
            {
                element.Id = (srcElement as HTMLTextAreaElementClass).id;
                element.InnerHtml = (srcElement as HTMLTextAreaElementClass).innerHTML;
                element.TagName = (srcElement as HTMLTextAreaElementClass).tagName;
                element.Name = (srcElement as HTMLTextAreaElementClass).name;
                _builder.SetElementProperty(element, "innerHtml");

            }
            else if (srcElement is HTMLInputElement)
            {
                element.Id = (srcElement as HTMLInputElement).id;
                element.InnerHtml = (srcElement as HTMLInputElement).innerHTML;
                element.TagName = (srcElement as HTMLInputElement).tagName;
                element.Name = (srcElement as HTMLInputElement).name;
                element.Value = (srcElement as HTMLInputElement).value;
                switch ((srcElement as HTMLInputElement).type)
                {
                    case "text":
                    case "password":
                    if (string.IsNullOrEmpty(element.Value)) break;
                    _builder.SetElementProperty(element, "Value");
                    break;
                    case "checkbox":
                        if (string.IsNullOrEmpty(element.Value)) break;
                        _builder.SetElementProperty(element, "Value");
                        break;
                    case "default": break;
                }

            }

        }

        bool HTMLDocumentEvents2.onkeypress(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return pEvtObj.keyCode == 13 ? false : true;
        }
        

        #region Unused Events


        bool HTMLDocumentEvents2.onhelp(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }

        bool HTMLDocumentEvents2.ondblclick(IHTMLEventObj pEvtObj)
        {
            
            if (enableEvents == false) return true;
            if (AssertElement == true) return false;
            //Debug.WriteLine("DoubleClick" + pEvtObj.type);

            supress = true;
            if (_previousClickedElement != null && !pEvtObj.srcElement.Equals(_previousClickedElement) && pEvtObj.srcElement.contains(_previousClickedElement))
                return true; ; //we want to ignore events that bubble up

            //if (!m_IsCOnnected) return false;
            //Debug.WriteLine("Onclick: " + pEvtObj.srcElement.GetType().ToString());
            IHTMLElement srcElement = pEvtObj.srcElement;

            HtmlElement element = new HtmlElement();

            if (srcElement is HTMLSpanElement)
            {
                String[] ignoreTypes = { "STRONG", "I", "U", "B" };
                for (int i = 0; i < ignoreTypes.Length; i++)
                    if (ignoreTypes[i].Equals(srcElement.tagName))
                        srcElement = srcElement.parentElement;
            }


            if (srcElement is HTMLInputElement)
            {
                if ((srcElement as HTMLInputElement).type.Equals("text") && !AssertElement) return true;
                element.Id = (srcElement as HTMLInputElement).id;
                element.InnerHtml = (srcElement as HTMLInputElement).innerHTML;
                element.TagName = (srcElement as HTMLInputElement).tagName;
                element.Name = (srcElement as HTMLInputElement).name;
                element.Value = (srcElement as HTMLInputElement).value;
                element.OnClick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            }
            else if (srcElement is HTMLAnchorElement)
            {
                element.Id = (srcElement as HTMLAnchorElement).id;
                element.InnerHtml = (srcElement as HTMLAnchorElement).innerHTML;
                element.TagName = (srcElement as HTMLAnchorElement).tagName;
                element.Name = (srcElement as HTMLAnchorElement).name;
                element.Href = (srcElement as HTMLAnchorElement).href;
                element.OnClick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            }
            else if (srcElement is HTMLImg)
            {
                element.Id = (srcElement as HTMLImg).id;
                element.InnerHtml = (srcElement as HTMLImg).innerHTML;
                element.TagName = (srcElement as HTMLImg).tagName;
                element.Name = (srcElement as HTMLImg).name;
                element.Href = (srcElement as HTMLImg).href;
                element.OnClick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            }
            else if (srcElement is IHTMLElement)
            {
                element.Id = (srcElement as IHTMLElement).id;
                element.InnerHtml = (srcElement as IHTMLElement).innerHTML;
                element.TagName = (srcElement as IHTMLElement).tagName;
                if (element.TagName.Equals("BODY")) return true;
                element.Href = string.Empty;
                element.OnClick = (srcElement as IHTMLElement4).getAttributeNode("onclick").nodeValue.ToString();
            }
            else return false;
            _previousClickedElement = srcElement;
            //HtmlElement element = processElement(pEvtObj.srcElement);

            _builder.StimulateElement(element, "ondblclick");
           
             

            return true;
        }


        void HTMLDocumentEvents2.onkeydown(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.onkeyup(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.onmousedown(IHTMLEventObj pEvtObj)
        {
            checkAttach();
            Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.onmousemove(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }

        void HTMLDocumentEvents2.onreadystatechange(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        bool HTMLDocumentEvents2.onbeforeupdate(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }


        void HTMLDocumentEvents2.onafterupdate(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        bool HTMLDocumentEvents2.onrowexit(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }

        void HTMLDocumentEvents2.onrowenter(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }

        bool HTMLDocumentEvents2.ondragstart(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }

        bool HTMLDocumentEvents2.onselectstart(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }


        bool HTMLDocumentEvents2.onerrorupdate(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }


        bool HTMLDocumentEvents2.oncontextmenu(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }


        bool HTMLDocumentEvents2.onstop(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }


        void HTMLDocumentEvents2.onrowsdelete(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);

        }


        void HTMLDocumentEvents2.onrowsinserted(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.oncellchange(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.onpropertychange(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.ondatasetchanged(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.ondataavailable(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.ondatasetcomplete(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.onbeforeeditfocus(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.onselectionchange(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        bool HTMLDocumentEvents2.oncontrolselect(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }


        bool HTMLDocumentEvents2.onmousewheel(IHTMLEventObj pEvtObj)
        {
            return true;
        }

        void HTMLDocumentEvents2.onactivate(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        void HTMLDocumentEvents2.ondeactivate(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
        }


        bool HTMLDocumentEvents2.onbeforeactivate(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }


        bool HTMLDocumentEvents2.onbeforedeactivate(IHTMLEventObj pEvtObj)
        {
            //Debug.WriteLine(pEvtObj.type);
            return true;
        }

        #endregion


    }
    

}
