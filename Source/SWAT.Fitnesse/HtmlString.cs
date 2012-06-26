using System;
using System.Text;
using fit;
using fitSharp.Parser;
using System.Collections.Generic;

namespace SWAT.Fitnesse
{
    public class HtmlString
    {
        public HtmlString(string theHtml)
        {
            myHtml = theHtml;
        }

        public string ToPlainText()
        {
            //UnEscape: Turns "& ;" sequences into the chars they represent. ie: "&lt;"  -->  "<"
            string result = UnEscape(UnFormat(myHtml));
            foreach (char c in result)
            {
                if (c != ' ')
                    return result;
            }
            return string.Empty;
        }

        //**UnFormat**: Turns other special chars into text. ie: "\u00a0"  -->  " "   (..unicodeSpaceTest)
        //Removes whitespace at ends  (..FitNesseStandardVersionTest)
        //Removes html tag?? (..FitNesseStandardVersionTest)
        private string UnFormat(string theInput)
        {
            TextOutput result = new TextOutput();

            while (theInput.Length > 0)
            {
                string leader = FindLeader(theInput);
                result.Append(leader);

                string tag = FindTagAndTrim(ref theInput);
                if (tag.Length == 0) break;

                if (fitSharp.Parser.HtmlString.IsStandard) result.AppendTag(GetTag(tag));  //Should not have tags
            }
            return result.ToString();
        }

        private string FindTagAndTrim(ref string subStr)
        {
            int index1 = subStr.IndexOf("<");
            int index2 = subStr.IndexOf(">");
            string result = "";

            if (index1 >= 0 && index1 < index2 && (subStr[index1 + 1] == '/' || char.IsLetter(subStr[index1 + 1])))
            {
                result = subStr.Substring(index1 + 1, (index2 - index1 - 1));
                subStr = subStr.Substring(index2 + 1);
            }
            else //If there are no more tags, you're done looking at the string
                subStr = "";

            return result;
        }

        private string FindLeader(string subStr)
        {
            int index1 = subStr.IndexOf("<");
            int index2 = subStr.IndexOf(">");

            if (index1 >= 0 && index1 < index2 && (subStr[index1 + 1] == '/' || char.IsLetter(subStr[index1 + 1])))
                return subStr.Substring(0, index1);
            else
                return subStr;

        }
        /*
        private string[] FindTokenPairs(string theInput, string first, string last)
        {
            if (!theInput.Contains("<") || !theInput.Contains(">"))
                return new string[] { theInput };

            List<string> tokens = new List<string>();

            while (theInput.Contains(first) && theInput.Contains(last))
            {
                int index1 = theInput.IndexOf(first);
                int index2 = theInput.IndexOf(last);
                string tag = theInput.Substring(index1 + 1, (index2 - index1 - 1));
                theInput = theInput.Substring(index2+1);

                if (tag.Length > 0 && (tag[0] == '/' || char.IsLetter(tag[0])))
                    tokens.Add('<' + tag.Trim() + '>');
            }

            return tokens.ToArray();
        }

        private static bool IsValidTag(string theBody)
        {
            return theBody[0] == '/' || char.IsLetter(theBody[0]);
        }

        private static TokenBodyFilter ourValidTagFilter = new TokenBodyFilter(IsValidTag);
        */
        private string GetTag(string theInput)
        {
            StringBuilder tag = new StringBuilder();
            int i = 0;
            if (theInput[0] == '/') tag.Append(theInput[i++]);
            while (i < theInput.Length && char.IsLetter(theInput[i]))
            {
                tag.Append(theInput[i++]);
            }
            return tag.ToString().ToLower();
        }

        public static string UnEscape(string theInput)
        {
            Scanner scan = new Scanner(theInput);
            StringBuilder result = new StringBuilder();
            while (true)
            {
                scan.FindTokenPair("&", ";");
                result.Append(scan.Leader);
                if (scan.Body.Length == 0) break;
                if (scan.Body.Equals("lt")) result.Append('<');
                else if (scan.Body.Equals("gt")) result.Append('>');
                else if (scan.Body.Equals("amp")) result.Append('&');
                else if (scan.Body.Equals("nbsp")) result.Append(' ');
                else if (scan.Body.Equals("quot")) result.Append('"');
                else
                {
                    result.Append('&');
                    result.Append(scan.Body);
                    result.Append(';');
                }
            }
            return result.ToString();
        }

        private string myHtml;
    }

    public class TextOutput
    {

        public TextOutput()
        {
            myText = new StringBuilder();
            myLastTag = string.Empty;
            myWhitespace = false;
        }

        //public void Append(Substring theInput)  //Fit ref.
        public void Append(string theInput)
        {
            for (int i = 0; i < theInput.Length; i++)
            {
                char input = theInput[i];
                //Fit ref.
                if (fitSharp.Parser.HtmlString.IsStandard && input != '\u00a0' && char.IsWhiteSpace(input))
                {
                    if (!myWhitespace)
                    {
                        myText.Append(' ');
                        myLastTag = myLastTag + " ";
                    }
                    myWhitespace = true;
                }
                else
                {
                    switch (input)
                    {
                        //case '\u201c':
                        //    input = '"'; break;
                        //case '\u201d':
                        //    input = '"'; break;
                        //case '\u2018':
                        //    input = '\''; break;
                        //case '\u2019':
                        //    input = '\''; break;
                        case '\u00a0':
                            input = ' '; break;
                        case '&':
                            if (theInput.Substring(i + 1).StartsWith("nbsp;"))
                            {
                                input = ' ';
                                i += 5;
                            }
                            break;
                    }
                    myText.Append(input);
                    myWhitespace = false;
                    myLastTag = string.Empty;
                }
            }
        }

        public void AppendTag(string theInput)
        {
            if (theInput == "br")
            {
                myText.Append("<br />");
                myWhitespace = false;
            }
            else if (myLastTag.StartsWith("/p") && theInput == "p")
            {
                if (myLastTag == "/p ") myText.Remove(myText.Length - 1, 1);
                myWhitespace = false;
                myText.Append("<br />");
            }
            myLastTag = theInput;
        }

        public override string ToString()
        {
            return fitSharp.Parser.HtmlString.IsStandard ? myText.ToString().Trim().Replace("<br>", "\n").Replace("<br />", "\n") : myText.ToString();
        }

        private StringBuilder myText;
        private string myLastTag;
        bool myWhitespace;
    }
}