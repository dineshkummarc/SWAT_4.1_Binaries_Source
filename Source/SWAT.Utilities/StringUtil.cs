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

namespace SWAT.Utilities
{
    public static class StringUtil
    {
        /// <summary>
        /// Counts the number of occurrences of substr in str.
        /// </summary>
        /// <param name="str">the source string.</param>
        /// <param name="substr">the substring.</param>
        /// <returns>the number of occurrences of substr in str.</returns>
        public static int occurrences(String str, String substr)
        {
            if (String.IsNullOrEmpty(substr)) return 0;
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentException(
                    String.Format("Cannot count occurrences of substring: {0}, in a null string.", substr));
            }
            if (!str.Contains(substr))
            {
                return 0;
            }
            int count = 0;
            String source = str.Substring(0, str.Length);//clone of str
            while (source.Contains(substr))
            {
                count++;
                source = removeSubStr(source, substr);
            }
            return count;
        }
        /// <summary>
        /// Removes all the occurrences of a given substring in a given string.
        /// </summary>
        /// <param name="str">the source string.</param>
        /// <param name="substr">the substring to remove.</param>
        /// <returns>a copy of the original string without any occurrences of substr.</returns>
        public static String removeSubstring(String str, String substr)
        {
            if (String.IsNullOrEmpty(substr)) return str;
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentException(
                    String.Format("Cannot remove substring: {0}, from a null string.", substr));
            }
            if (!str.Contains(substr))
            {
                return str;
            }

            String source = str.Substring(0,str.Length);//clone of str

            int indexStart = str.IndexOf(substr);
            int indexLast = str.LastIndexOf(substr);

            if (indexStart == indexLast)
            {
                //only one occurrence
                return removeSubStr(source, substr);
            }
            else
            {
                //more than one occurrence
                while (source.Contains(substr))
                {
                    source = removeSubStr(source, substr);
                }
            }
            return source;
        }
        private static String removeSubStr(String src, String sub)
        {
            return src.Remove(src.IndexOf(sub), sub.Length);
        }
        /// <summary>
        /// Removes the last occurrence of substr in str.
        /// </summary>
        /// <param name="str">the source string.</param>
        /// <param name="substr">the substring.</param>
        /// <returns>a copy of the original string with the last occurrence fo substr removed.</returns>
        public static String removeLastSubstring(String str, String substr)
        {
            if (String.IsNullOrEmpty(substr)) return str;
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentException(
                    String.Format("Cannot remove substring: {0}, from a null string.", substr));
            }
            if (!str.Contains(substr))
            {
                return str;
            }

            String source = str.Substring(0, str.Length);//clone of str

            return source.Remove(source.LastIndexOf(substr), substr.Length);
        }
        /// <summary>
        /// Removes any non-alphabetic characters from a string.
        /// </summary>
        /// <param name="srt">the string to process.</param>
        /// <returns>a copy of the original string with non-alpha chars removed.</returns>
        /// <exception cref="ArgumentException">thrown if str is null.</exception>
        public static String removeNonAlpha(String str)
        {
            if (str == null)
            {
                throw new ArgumentException("Cannot remove characters from a null string.");
            }
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            //extract non-alpha
            //remove them
            System.Text.RegularExpressions.Regex nonAlphPattern = 
                new System.Text.RegularExpressions.Regex("[a-zA-Z][a-zA-Z]*");
            if (!nonAlphPattern.IsMatch(str))
            {
                return String.Empty;//if all the chars in str are non-alpha, then return ""
            }
            else
            {
                //str contains non-alpanumeric
                StringBuilder clone = new StringBuilder(str.Length);
                System.Text.RegularExpressions.MatchCollection matches = nonAlphPattern.Matches(str);
                foreach (System.Text.RegularExpressions.Match m in matches)
                {
                    //clone.Remove(m.Index, m.Length);
                    clone.Append(str.Substring(m.Index, m.Length));
                }
                return clone.ToString();
            }
        }
        /// <summary>
        /// Removes the first(from left to right) non alphanumeric substring from a given string.
        /// </summary>
        /// <param name="str">the given string</param>
        /// <returns>a new string with the first non alphanumeric substring removed.</returns>
        /// <exception cref="ArgumentException">if str is null.</exception>
        public static String removeFirstNonAlpha(String str)
        {
            if (str == null)
            {
                throw new ArgumentException("Cannot remove characters from a null string.");
            }
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            //extract non-alpha
            //remove them
            System.Text.RegularExpressions.Regex nonAlphPattern =
                new System.Text.RegularExpressions.Regex("[a-zA-Z][a-zA-Z]*");
            if (!nonAlphPattern.IsMatch(str))
            {
                return String.Empty;//if all the chars in str are non-alpha, then return ""
            }
            else
            {
                //str contains non-alpanumeric
                StringBuilder clone = new StringBuilder(str.Length);
                System.Text.RegularExpressions.MatchCollection matches = nonAlphPattern.Matches(str);
                clone.Append(str.Substring(matches[0].Index, (str.Length - matches[0].Index)));
                return clone.ToString();
            }
        }
    }
}
