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
using System.IO;
using System.Collections.Generic;
using System.Text;
using SWAT.Auto_Complete.AssemblyReader;
using SWAT;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SWAT.Auto_Complete
{
    public class SwatAuto_Complete
    {
        #region Constructor
        public SwatAuto_Complete(RichTextBox txtBox, System.Windows.Forms.ListBox lstBox, RichTextBox txtBoxToolTip)
        {
            //text box, list box, and tool tip text box from command control
            this.textBox = txtBox;
            this.listBox = lstBox;
            this.reader = new SwatReader(System.Windows.Forms.Application.StartupPath + STARTUP_PATH_MODIFIER);
            lastTextLength = 0;

            this.commands = reader.getCommands();

            //set text box, list box and tool tip properties
            this.setProperties();
        }

        public void setProperties()
        {

            //listBox Properties
            this.listBox.Font = this.textBox.Font;
            this.listBox.FormattingEnabled = true;
            this.listBox.DrawMode = System.Windows.Forms.DrawMode.Normal;
            this.listBox.Name = "listBox";
            this.listBox.TabIndex = 5;
            this.listBox.Visible = false;
            this.listBox.Sorted = true;
            this.listBox.ScrollAlwaysVisible = false;
            this.listBox.IntegralHeight = true;

            //listBox event handlers
            this.listBox.SelectedIndexChanged += new EventHandler(listBox_SelectedIndexChanged);
            this.listBox.DoubleClick += new EventHandler(listBox_DoubleClick);

            //textbox properties
            this.textBox.AcceptsTab = true;

            //textBox event handlers
            this.textBox.TextChanged += new EventHandler(textBox_TextChanged);
            this.textBox.KeyUp += new KeyEventHandler(textBox_KeyUp);
            this.textBox.KeyDown += new KeyEventHandler(textBox_KeyDown);
            this.textBox.Click += new EventHandler(textBox_Click);
            this.textBox.LostFocus += new EventHandler(textBox_LostFocus);
            this.textBox.Resize += new EventHandler(textBox_Resize);
        }



        #endregion

        #region EventHandlers

        //send focus to text box to trap up and down keys for navigating through list box
        void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBox.Focus();
        }

        //hide intellisense when focus is lost
        void textBox_LostFocus(object sender, EventArgs e)
        {
            if (!listBox.Focus())
                listBox.Visible = false;
        }

        void textBox_Click(object sender, EventArgs e)
        {
            int k = this.textBox.SelectionStart;
            this.listBox.Visible = false;
        }

        void textBox_Resize(object sender, EventArgs e)
        {
            if (listBox.Visible == true)
                setListBoxPosition();
        }

        void textBox_TextChanged(object sender, EventArgs e)
        {
            string text = textBox.Text;
            int cursorPos = textBox.SelectionStart;

            char x = cursorPos > 1 ? text[cursorPos - 2] : ' ';
            char y = cursorPos > 0 ? text[cursorPos - 1] : ' ';

            if (text.Length - 1 == lastTextLength && x == '!' && y == '-' && !listBox.Visible)
            {
                text = textBox.Text = string.Concat(text.Substring(0, cursorPos),
                    string.Concat("-!", text.Substring(cursorPos, text.Length - cursorPos)));

                textBox.SelectionStart = cursorPos;
            }

            lastTextLength = text.Length;
        }

        //automplete selected item from list box
        void listBox_DoubleClick(object sender, EventArgs e)
        {
            this.autoComplete();
            textBox.Focus();
        }


        void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (listBox.Visible)
            {
                if (isUpOrDownKey(e.KeyCode))
                {
                    updateListBoxSelection(e.KeyCode);
                    e.SuppressKeyPress = true;
                }
                else if (isKeyToAutocomplete(e.KeyCode))
                {
                    e.SuppressKeyPress = true;
                    autoComplete();
                }
            }
        }

        void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            Keys keydata = e.KeyData;
            string value = keydata.ToString();

            if (useIntellisense && !isKeysToIgnoreByTextBox(e.KeyCode) && !insideLiteral(getCurrentCharPosition()))
            {
                if (value.Equals("Oem5") || value.Equals("Oem5, Shift") || value.Equals("Oem1") || value.Equals("D1, Shift"))
                {
                    listBox.Visible = false;
                    processKeyDown(sender, e);
                }
                else if (listBox.Visible == true)
                    processKeyDown(sender, e);
            }
        }

        #endregion

        #region Methods

        public void processKeyDown(object sender, KeyEventArgs e)
        {
            //this string will return whether we have to show a command or a type or a tag or none of these
            string match = findMatch();

            if (!match.Equals("none"))
            {
                if (currentLineToCursor.Equals("|") && listBox.Visible)
                {
                    listBox.Items.Clear();
                    loadList("modifierInverseAndCommands");
                }
                else if (previousMatch.Equals("modifierInverseAndCommands") && (match.Equals("inverseAndCommands") || sender.Equals("autocompletedM")))
                {
                    listBox.Visible = true;
                    listBox.Items.Clear();
                    loadList("inverseAndCommands");
                    if (sender.Equals("autocompleted"))
                        setListBoxPosition();
                }
                else if ((previousMatch.Equals("inverseAndCommands") && match.Equals("commands")) || sender.Equals("autocompletedI"))
                {
                    listBox.Visible = true;
                    listBox.Items.Clear();
                    loadList("commands");
                    if (sender.Equals("autocompleted"))
                        setListBoxPosition();
                }
                else if (listBox.Visible == false)
                {
                    listBox.Items.Clear();
                    if (loadList(match))
                    {
                        setListBoxPosition();
                        listBox.BringToFront();
                        listBox.Visible = true;
                    }
                }
				else if (previousMatch.Equals("fixtures") && match.Equals("fixtures") || sender.Equals("autocompletedF"))
				{
					listBox.Visible = true;
					listBox.Items.Clear();
					loadList("fixtures");
					if (sender.Equals("autocompleted"))
						setListBoxPosition();
				}
                //for going backwards

                else if ((previousMatch.Equals("inverseAndCommands") || previousMatch.Equals("commands")) && (match.Equals("modifierInverseAndCommands")))
                {
                    setListBoxPosition();
                    listBox.Items.Clear();
                    loadList("modifierInverseAndCommands");
                }

                else if ((previousMatch.Equals("commands") && match.Equals("inverseAndCommands")))
                {
                    setListBoxPosition();
                    listBox.Items.Clear();
                    loadList("inverseAndCommands");
                }

                updateListBoxSelection(e.KeyCode);
            }
            else //match is 'none' so we don't show the list
                listBox.Visible = false;
            previousMatch = match;
        }

        private bool isKeysToIgnoreByTextBox(Keys key)
        {
            if (isUpOrDownKey(key) || key.Equals(Keys.VolumeDown) || key.Equals(Keys.VolumeUp)
                || key.Equals(Keys.VolumeMute) || key.Equals(Keys.CapsLock) || key.Equals(Keys.ShiftKey)
                || key.Equals(Keys.Tab) || key.Equals(Keys.Space) || key.Equals(Keys.Enter))
                return true;

            return false;
        }

        private bool isUpOrDownKey(Keys key)
        {
            if (key.Equals(Keys.Up) || key.Equals(Keys.Down))
                return true;

            return false;
        }

        private void updateListBoxSelection(Keys key)
        {
            //handle key up on the listbox
            if (key.Equals(Keys.Up))
            {
                if (this.listBox.SelectedIndex > 0)
                {
                    listBox.SelectedIndex--;
                }
                else
                    listBox.SelectedIndex = 0;
            }
            //handle key down on the listbox
            else if (key.Equals(Keys.Down))
            {
                if (this.listBox.SelectedIndex < this.listBox.Items.Count - 1)
                    this.listBox.SelectedIndex++;
            }

            if (!isUpOrDownKey(key))
            {
                string currentText = getTextToSelectListBox();
                int matchingIndex = this.listBox.FindString(currentText);

                if (currentText.Equals("") || matchingIndex == ListBox.NoMatches)
                    this.listBox.ClearSelected();
                else
                {
                    this.listBox.SelectedIndex = matchingIndex;
                }
            }
        }

        private string getTextToSelectListBox()
        {
            Regex regExp = new Regex(".*\\|");
            string text = Regex.Replace(currentLineToCursor, regExp.ToString(), "");
            int pipes = getPipesOnLineToCursor();

            switch (pipes)
            {
                case 1:
                    //delete modifiers
                    if (canShowModifierInverseAndCommand(text))
                        return text;
                    if (canShowInverseAndCommand(text))
                        return stripModifiers(text);
                    else
                        return stripInverseAndModifiers(text);

                case 3:
                    //delete up to semicolom
                    Regex semiCol = new Regex(".*;");
                    return Regex.Replace(text, semiCol.ToString(), "");
                default:
                    return text;
            }
        }

        private string stripInverseAndModifiers(string str)
        {
            //starts at 1 because 0 is ""
            for (int i = listOfRealExModifiers.Length - 1; i >= 0; i--)
            {
                string realEx = listOfRealExModifiers[i] + "<?>?[A-Za-z]*";
                Regex showCommand = new Regex(realEx);
                Match m = showCommand.Match(str);
                if (m.Success)
                {
                    string strip = Regex.Replace(m.ToString(), listOfRealExModifiers[i] + "<?>?", "");
                    return strip;
                }
            }

            return "";
        }

        private string stripModifiers(string str)
        {
            //starts at 1 because 0 is ""
            for (int i = listOfRealExModifiers.Length - 1; i >= 0; i--)
            {
                string realEx = listOfRealExModifiers[i] + "<?>?[A-Za-z]*";
                Regex showCommand = new Regex(realEx);
                Match m = showCommand.Match(str);
                if (m.Success)
                {
                    string strip = Regex.Replace(m.ToString(), listOfRealExModifiers[i], "");
                    return strip;
                }
            }

            return "";
        }

        //count the pipes on currentlinetocursor
        private int getPipesOnLineToCursor()
        {
            int pipes = 0;
            int index = 0;
            foreach (char pipe in currentLineToCursor.ToCharArray())
            {
                if (!insideLiteral(index) && pipe.Equals('|'))
                    pipes++;

                index++;
            }

            return pipes;
        }

        private string findMatch()
        {
            //get text from beginning of line to cursor
            int lineStart = textBox.GetFirstCharIndexOfCurrentLine();
            currentLineToCursor = textBox.Text.Substring(lineStart, textBox.SelectionStart - lineStart);

            //count the pipes on currentlinetocursor
            int temp = getPipesOnLineToCursor();

            if (noOfPipes != temp)
            {
                listBox.Visible = false;
                noOfPipes = temp;
            }

            switch (noOfPipes)
            {
                case 0:
						return "none";
                case 1:
					if (currentLineToCursor.Equals("!|") || canShowFixture(currentLineToCursor))
						return "fixtures";

					else if (canShowModifierInverseAndCommand(currentLineToCursor))
                        return "modifierInverseAndCommands";

                    else if (canShowInverseAndCommand(currentLineToCursor))
                        return "inverseAndCommands";

                    else if (canShowCommand(currentLineToCursor))
                        return "commands";

                    else
                        return "none";
                case 6:
                    if (!currentLineToCursor.Contains("SetElementAttribute"))
                        return getWhatToShow(currentLineToCursor, 3);
                    else
                        return getWhatToShow(currentLineToCursor, 4);
                case 3:
                    if (isExpression(currentLineToCursor))
                        if (canShowTypesWithoutExpression(currentLineToCursor))
                            return "typesWithoutExpression";
                        else
                            return "none";
                    return getWhatToShow(currentLineToCursor, 1);

                default:
                    return getWhatToShow(currentLineToCursor, noOfPipes - 2);
            }
        }

        private bool isKeyToAutocomplete(Keys key)
        {
            if (key.Equals(Keys.Enter) || key.Equals(Keys.Space) || key.Equals(Keys.Tab) || (key.Equals(Keys.OemPipe) && (Control.ModifierKeys == Keys.Shift)))
                return true;

            return false;
        }

        private bool isExpression(string line)
        {
            string realEx1 = "^\\|...[<>A-Za-z]+\\|[Ee][Xx][Pp][Rr][Ee][Ss][Ss][Ii][Oo][Nn]\\|";
            Regex re1 = new Regex(realEx1);
            Match m = re1.Match(currentLineToCursor);
            if (m.Success)
                return true;

            return false;
        }

        private bool canShowTypesWithoutExpression(string line)
        {
            string realEx1 = "^\\|...[<>A-Za-z]+\\|[Ee][Xx][Pp][Rr][Ee][Ss][Ss][Ii][Oo][Nn]\\|[^\\|^=^:^;^ ]*$";
            string realEx2 = "^\\|...[<>A-Za-z]+\\|[Ee][Xx][Pp][Rr][Ee][Ss][Ss][Ii][Oo][Nn]\\|([^\\|^=^:^;^ ]+[=:][^\\|^=^:^;]*;){1,20}[^\\|^=^:^ ^;]*$";
            Regex re1 = new Regex(realEx1);
            Regex re2 = new Regex(realEx2);
            Match m = re1.Match(line);

            string actualCommand = getCommand(line);
            bool isIdentifierType = reader.getParameter(actualCommand, 0).Equals("SWAT.IdentifierType");

            if (m.Success && !actualCommand.Equals("none") && isIdentifierType)
                return true;
            else
            {
                m = re2.Match(line);
                if (m.Success && !actualCommand.Equals("none") && isIdentifierType)
                    return true;
            }
            return false;
        }

        //returns what to show or none if nothing to be shown
        private string getWhatToShow(string line, int paramIndex)
        {
            string command = getCommand(line);
            if (command.Equals("none"))
                return "none";

            string toShow = reader.getParameter(command, paramIndex);
            if (toShow.StartsWith("SWAT"))
                return toShow;
            else if (toShow.Equals("System.String"))
                return reader.getParameterByName(command, paramIndex);

            return "none";
        }

        //true if the line is correct to show only commands
        private bool canShowCommand(string line)
        {
            //case to show commands - only 1 pipe shown
            foreach (string modifiers in listOfRealExModifiers)
            {
                string realEx = "^\\|" + modifiers + "<*>*[A-Za-z]*$";
                Regex showCommands = new Regex(realEx);
                Match m = showCommands.Match(currentLineToCursor);
                if (m.Success)
                    return true;
            }

            return false;
        }

        //true if the line is correct to show modifiers, inverse, and commands
        private bool canShowInverseAndCommand(string line)
        {
            //case to show modifiers- only 1 pipe shown
            foreach (string modifiers in listOfRealExModifiers)
            {
                string realEx = "^\\|(" + modifiers + "|" + modifiers + "<)$";
                Regex showCommands = new Regex(realEx);
                Match m = showCommands.Match(currentLineToCursor);
                if (m.Success)
                    return true;
            }

            return false;
        }

        private bool canShowModifierInverseAndCommand(string line)
        {
            //case to show modifiers- only 1 pipe shown
            foreach (string modifiers in listOfIncompleteExModifiers)
            {
                string realEx = "^\\|" + modifiers + "$";
                Regex showCommands = new Regex(realEx);
                Match m = showCommands.Match(currentLineToCursor);
                if (m.Success)
                    return true;
            }

            return false;
        }

		private bool canShowFixture(string line)
		{
			//case to show fixtures - only 1 pipe shown
			foreach (string fixture in listOfSWATFixtures)
			{
				//if (fixture.ToLower().IndexOf(line.ToLower().Substring(2)) >= 0)
				//    return true;

				string realEx = @"^!|" + fixture + "[A-Za-z]$";
				Regex showCommands = new Regex(realEx);
				Match m = showCommands.Match(currentLineToCursor);
				if (m.Success)
					return true;
			}

			return false;
		}

        //find the command on the current line, if any
        private string getCommand(string line)
        {
            string command = "";

            foreach (string modifier in listOfRealExModifiers)
            {
                string realEx = "^\\|" + modifier + "<*>*[A-Za-z]*";
                Regex showCommand = new Regex(realEx);
                Match m = showCommand.Match(line);
                if (m.Success)
                {
                    string strip = Regex.Replace(m.ToString(), "\\|" + modifier + "<?>?", "");
                    command = Regex.Replace(strip, "\\|.*", "");
                    if (commands.Contains(command))
                        return command;
                }
            }

            return "none";
        }

        private bool loadList(string str)
        {
            greatestWordOnList = "";

            if (str.Equals("modifierInverseAndCommands"))
            {
                foreach (String modifier in modifiers)
                {
                    this.listBox.Items.Add(modifier);

                    if (modifier.Length > greatestWordOnList.Length)
                        greatestWordOnList = modifier;
                }
                this.listBox.Items.Add("<>");
                foreach (String command in commands)
                {
                    this.listBox.Items.Add(command);

                    if (command.Length > greatestWordOnList.Length)
                        greatestWordOnList = command;
                }
                setListBoxHeigthAndWidth();
                return true;
            }

            //case to load commands
            else if (str.Equals("inverseAndCommands"))
            {
                this.listBox.Items.Add("<>");
                foreach (String command in commands)
                {
                    this.listBox.Items.Add(command);

                    if (command.Length > greatestWordOnList.Length)
                        greatestWordOnList = command;
                }
                setListBoxHeigthAndWidth();
                return true;
            }

            else if (str.Equals("commands"))
            {
                foreach (String command in commands)
                {
                    this.listBox.Items.Add(command);

                    if (command.Length > greatestWordOnList.Length)
                        greatestWordOnList = command;
                }
                setListBoxHeigthAndWidth();
                return true;
            }
            else if (str.Equals("configName"))
            {
                Assembly swatAssembly;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                path = new Uri(Path.Combine(path, "SWAT.Core.dll")).LocalPath;
                swatAssembly = Assembly.LoadFrom(path);

                Type[] AllTypes = swatAssembly.GetTypes();

                //Finds classes with the custom attribute UserSettingAttribute and adds its
                //PropertyInfo for each of its properties to UserSettingsProperties List
                foreach (Type currentType in AllTypes)
                {
                    if (currentType.GetCustomAttributes(typeof(UserSettingAttribute), false).Length > 0)
                    {                      
                        foreach (PropertyInfo currentProperty in currentType.GetProperties())
                        {
                            this.listBox.Items.Add(currentProperty.Name);

                            if (currentProperty.Name.Length > greatestWordOnList.Length)
                                greatestWordOnList = currentProperty.Name;
                        }
                    }
                }
                setListBoxHeigthAndWidth();

                return true;
            }
            else if (str.Equals("typesWithoutExpression"))
                return loadList("SWAT.IdentifierTypeWithoutExpression");

            else if (str.Equals("tagName"))
            {
                string currentCommand = getCommand(currentLineToCursor);
                if (currentCommand.StartsWith("Assert") && !currentCommand.Equals("AssertElementExists") && currentCommand.Equals("AssertElementDoesNotExists"))
                    return false;

                if (currentCommand.Equals("GetElementAttribute"))
                { // added condition since the command is waiting for a variable name.
                    if (getPipesOnLineToCursor() <= 5)
                    {
                        return false;
                    }
                    else
                        return loadList("SWAT.TagName");
                }
                if (currentCommand.Equals("StimulateElement") || currentCommand.Equals("PressKeys") || currentCommand.Equals("AssertElementExistsWithTimeout")) //intellisense will show incorrectly for tagNames.
                {
                    if (getPipesOnLineToCursor() >= 6)
                        return false;
                    else
                        return loadList("SWAT.TagName");
                }

                return loadList("SWAT.TagName");
            }
            else if (str.Equals("attributeValues"))
            {
                if (getCommand(currentLineToCursor).Equals("SetElementAttribute"))
                    return loadList("SWAT.TagName");
                else
                    return false;

            }
            else if (str.Equals("eventName"))
                return loadList("SWAT.Events");

            else if (str.Equals("browserName"))
                return loadList("SWAT.BrowserName");

            else if (str.Equals("attributeName"))
            {
                //needed to condition "GetElementAttribute" as the method is actually expecting
                //a variable name, not an attributeName
                //if (getCommand(currentLineToCursor).Equals("GetElementAttribute"))
                //    return false;

                return loadList("SWAT.Attributes");
            }

            else if (str.Equals("SWAT.AttributeType"))
                return false;

            else if (str.Equals("fixtures"))
            {
				foreach (String fixture in listOfSWATFixtures)
				{
					this.listBox.Items.Add(fixture);

					if (fixture.Length > greatestWordOnList.Length)
						greatestWordOnList = fixture;
				}
				setListBoxHeigthAndWidth();
				return true;
            }

            else
                //load other cases
                return readAndLoadClasssOrEnumsToListBox(str);
        }

        public void setListBoxHeigthAndWidth()
        {
            //setting the width and heigth of the listbox
            System.Drawing.Graphics g = this.listBox.CreateGraphics();
            int width = (int)g.MeasureString(greatestWordOnList, this.listBox.Font).Width + 35;
            int heigth = listBox.ItemHeight * listBox.Items.Count + 15;
            g.Dispose();

            //set listbox width
            this.listBox.Width = width;

            //set listbox heigth
            if (heigth < 100)
                listBox.Height = heigth;
            else
                listBox.Height = 100;
        }

        //loads the intellisense listbox after reading from a class or an enum from the SWAT.Interface class
        public bool readAndLoadClasssOrEnumsToListBox(string str)
        {
            try
            {
                interfaceClass = reader.swatAssemblyProperty.GetType(str);
                if (interfaceClass.IsEnum)
                {
                    Regex regEx = new Regex("SWAT.+ ");
                    FieldInfo[] enumMembers = interfaceClass.GetFields();
                    for (int i = 1; i < enumMembers.Length; i++)
                    {
                        string strName = Regex.Replace(enumMembers[i].ToString(), regEx.ToString(), "");
                        listBox.Items.Add(strName);

                        if (strName.Length > greatestWordOnList.Length)
                            greatestWordOnList = strName;
                    }
                }
                else if (interfaceClass.IsClass)
                {
                    FieldInfo[] classMembers = interfaceClass.GetFields();
                    PropertyInfo[] classProperties = interfaceClass.GetProperties();

                    for (int i = 0; i < classMembers.Length; i++)
                    {
                        if (!classMembers[i].GetType().IsArray)
                        {
                            string strName = (string)classMembers[i].GetValue(classMembers[i]);
                            listBox.Items.Add(strName);

                            if (strName.Length > greatestWordOnList.Length)
                                greatestWordOnList = strName;
                        }
                    }
                    for (int i = 0; i < classProperties.Length; i++)
                    {
                        if (classProperties[i].PropertyType.IsArray)
                        {
                            string[] strArray = (string[])classProperties[i].GetValue(classProperties[i], null);
                            for (int j = 0; j < strArray.Length; j++)
                            {
                                string strName = strArray[j];
                                listBox.Items.Add(strName);

                                if (strName.Length > greatestWordOnList.Length)
                                    greatestWordOnList = strName;
                            }
                        }
                    }

                }

                setListBoxHeigthAndWidth();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Sets the ListBox's position
        private void setListBoxPosition()
        {
            Point location = textBox.GetPositionFromCharIndex(textBox.SelectionStart);
            location.X += 40; // the 40 is approx the size of the line numbers box
            location.Y += listBox.ItemHeight + 5;

            if (location.X + listBox.Width > textBox.Location.X + textBox.Width)
                location.X -= listBox.Width;

            if (location.Y + listBox.Height > textBox.Location.Y + textBox.Height)
                location.Y -= (listBox.Height + listBox.ItemHeight + 5);

            this.listBox.Location = location;
        }

        // Autocompletes the selected item from the listbox to the textbox
        private void autoComplete()
        {
            //get string of selected item in list box
            string selectedItem = "";
            if (this.listBox.SelectedItem != null)
                selectedItem = this.listBox.SelectedItem.ToString();

            //hide listbox
            this.listBox.Visible = false;

            if (!selectedItem.Equals(""))
            {
                switch (noOfPipes)
                {
                    case 1:
                        if (selectedItem.StartsWith("@") || selectedItem.StartsWith("?"))// || selectedItem.)
                        {
                            autoCompleteModifier(selectedItem);
                        }
                        else if (selectedItem.StartsWith("<"))
                            autoCompleteInverse(selectedItem);

						else if (canShowFixture(selectedItem))
							autoCompleteFixture(selectedItem);

                        else
                        {
                            autoCompleteCommand(selectedItem);
                        }
                        break;
                    case 6:
                        autoCompleteDefault(selectedItem);
                        break;
                    case 3:
                        //handle when expression
                        if (isExpression(currentLineToCursor))
                        {
                            autoCompleteWithExpression(selectedItem);
                            break;
                        }
                        else
                            goto default;
                    default:
                        autoCompleteDefault(selectedItem);
                        break;
                }
            }
        }

		private void autoCompleteFixture(string toComplete)
		{
			for (int i = listOfSWATFixtures.Length - 1; i >= 0; i--)
			{
				int startIndex = textBox.GetFirstCharIndexOfCurrentLine() + 1;
				int selectionLength = textBox.SelectionStart - startIndex;
				if ( listOfSWATFixtures[i].IndexOf(toComplete) == 0)
				{
					textBox.Select(startIndex, selectionLength);
					textBox.SelectedText = "|" + toComplete + "|";
					//call process key to process the pipe we're adding to the completion
					listBox.Visible = false;
					processKeyDown("autocompletedF", new KeyEventArgs(Keys.Oem5 | Keys.Shift));
					break;
				}
			}
		}

        private void autoCompleteInverse(string toComplete)
        {
            for (int i = listOfRealExModifiers.Length - 1; i >= 0; i--)
            {
                int startIndex = textBox.GetFirstCharIndexOfCurrentLine() + 1;
                int selectionLength = textBox.SelectionStart - startIndex;

                Regex regExp = new Regex(listOfRealExModifiers[i]);
                Match m = regExp.Match(currentLineToCursor);
                if (m.Success)
                {
                    textBox.Select(startIndex + m.Length, selectionLength - m.Length);
                    textBox.SelectedText = toComplete;
                    //call process key to process the pipe we're adding to the completion
                    processKeyDown("autocompletedI", new KeyEventArgs(Keys.F24));

                    break;
                }
            }
            //call process key to process the pipe we're adding to the completion

        }

        private void autoCompleteModifier(string toComplete)
        {
            int startIndex = textBox.GetFirstCharIndexOfCurrentLine() + 1;
            int selectionLength = textBox.SelectionStart - startIndex;

            textBox.Select(startIndex, selectionLength);
            textBox.SelectedText = toComplete;
            //call process key to process the pipe we're adding to the completion
            processKeyDown("autocompletedM", new KeyEventArgs(Keys.F24));
        }


        private void autoCompleteCommand(string toComplete)
        {
            for (int i = listOfRealExModifiers.Length - 1; i >= 0; i--)
            {
                int startIndex = textBox.GetFirstCharIndexOfCurrentLine() + 1;
                int selectionLength = textBox.SelectionStart - startIndex;

                Regex regExp = new Regex(listOfRealExModifiers[i]);
                Match m = regExp.Match(currentLineToCursor);
                if (m.Success)
                {
                    textBox.Select(startIndex + m.Length, selectionLength - m.Length);
                    if (textBox.SelectedText.Contains(">"))
                        startIndex += 2;
                    textBox.Select(startIndex + m.Length, selectionLength - m.Length);
					textBox.SelectedText = toComplete + "|";
                    //call process key to process the pipe we're adding to the completion
                    listBox.Visible = false;
                    processKeyDown(new object(), new KeyEventArgs(Keys.Oem5 | Keys.Shift));
                    break;
                }
            }
        }

        private void autoCompleteWithExpression(string toComplete)
        {
            int startIndex = textBox.GetFirstCharIndexOfCurrentLine() + findLastCharIndex(';');
            int selectionLength = currentLineToCursor.Length - findLastCharIndex(';');

            Regex regExp = new Regex(".*;");
            Match m = regExp.Match(currentLineToCursor);
            if (m.Success)
            {
                if (toComplete.Equals("parentElement"))
                {
                    processIntellisenseForParentElement(startIndex, selectionLength, toComplete);
                }
                else
                {
                    textBox.Select(startIndex, selectionLength);
                    textBox.SelectedText = toComplete;
                }
            }
            else //this is exactly the same as the autoCompletDefault method but putting an = at the end
            //instead of a pipe... revise and make it more effective later
            {
                int startIndex1 = textBox.GetFirstCharIndexOfCurrentLine() + findLastCharIndex('|');
                int selectionLength1 = currentLineToCursor.Length - findLastCharIndex('|');
                if (toComplete.Equals("parentElement"))
                {
                    processIntellisenseForParentElement(startIndex1, selectionLength1, toComplete);
                }
                else
                {
                    textBox.Select(startIndex1, selectionLength1);
                    textBox.SelectedText = toComplete;
                }
            }
        }

        private void processIntellisenseForParentElement(int startIndex, int selectionLength, string toComplete)
        {
            textBox.Select(startIndex, selectionLength);
            textBox.SelectedText = toComplete + ".";
            listBox.Items.Clear();
            foreach (string s in IdentifierTypeWithoutExpression.Identifiers)
            {
                string wordOnList = "parentElement." + s;
                listBox.Items.Add(wordOnList);
                if (wordOnList.Length > greatestWordOnList.Length)
                {
                    greatestWordOnList = "parentElement" + s;
                }
            }
            setListBoxHeigthAndWidth();
            listBox.Visible = true;
            processKeyDown(new object(), new KeyEventArgs(Keys.Oem5 | Keys.Shift));
        }

        private void autoCompleteDefault(string toComplete)
        {
            int startIndex = textBox.GetFirstCharIndexOfCurrentLine() + findLastCharIndex('|');
            int selectionLength = currentLineToCursor.Length - findLastCharIndex('|');

            textBox.Select(startIndex, selectionLength);
            textBox.SelectedText = toComplete + "|";

            //call process key to process the pipe we're adding to the completion
            processKeyDown(new object(), new KeyEventArgs(Keys.Oem5 | Keys.Shift));
        }

        //sets the intellisense show on/off
        public void setIntellisense(bool setInt)
        {
            useIntellisense = setInt;
            listBox.Visible = false;
        }

        //returns the position of a given character + 1
        private int findLastCharIndex(char character)
        {
            char[] line = currentLineToCursor.ToCharArray();
            for (int i = line.Length - 1; i >= 0; i--)
            {
                if (line[i].Equals(character))
                    return i + 1;
            }

            return 0;
        }
        private bool insideLiteral(int colIndex)
        {
            if (colIndex == -1) { return false; }
            int rowIndex = textBox.GetLineFromCharIndex(textBox.SelectionStart);

            if (textBox.Lines.Length == 0)
            {
                return false;
            }
            try
            {
                string row = (string)textBox.Lines.GetValue(rowIndex);
                int beforeOpenOcc = countStringOccurrences(row, "!-", 0, colIndex);
                int beforeCloseOcc = countStringOccurrences(row, "-!", 0, colIndex);
                int afterCloseOcc = countStringOccurrences(row, "-!", colIndex, row.Length);

                return afterCloseOcc > 0 && beforeOpenOcc > beforeCloseOcc;
            }
            catch (IndexOutOfRangeException) { return false;  }
        }

        private int getCurrentCharPosition()
        {
            if (textBox.Lines.Length == 0)
            {
                return -1;
            }
            int rowIndex = textBox.GetLineFromCharIndex(textBox.SelectionStart);

            try
            {
                string row = (string)textBox.Lines.GetValue(textBox.GetLineFromCharIndex(textBox.SelectionStart));

                textBox.GetCharIndexFromPosition(textBox.GetPositionFromCharIndex(textBox.SelectionStart));
                int rowStartIndex = Win32.SendMessage(textBox.Handle, Win32.EM_LINEINDEX, -1, 0);

                return textBox.SelectionStart - rowStartIndex;
            }
            catch (IndexOutOfRangeException) { return -1; }
        }

        private int countStringOccurrences(string text, string pattern, int start, int end)
        {
            int count = 0;
            int i = start;
            while ((i = text.IndexOf(pattern, i)) != -1 && i < end)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        #endregion

        #region Private Class

        private class Win32
        {
            public Win32()
            {
                // 
                // TODO: Add constructor logic here 
                // 
            }
            [DllImport("User32.Dll")]
            public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
            public const int EM_LINEINDEX = 0xBB;
        }

        #endregion


        #region Private Variables

        //this modifiers are escaped for real expression used
        private string[] modifiers = {
            "@",
            "@@",
            "@@@",
            "?",
            "??",
            "?!",
            "??!"};

        //this modifiers are escaped for real expression used
        private string[] listOfRealExModifiers = {
            "",
            "@",
            "@@",
            "@@@",
            "\\?",
            "\\?\\?",
            "\\?\\!",
            "\\?\\?\\!"};

        private string[] listOfIncompleteExModifiers = {
            "",
            "@",
            "@@",
            "\\?",
            "\\?\\?"};

        private string[] listOfSWATFixtures = {
            "InternetExplorerSWATFixture",
            "FireFoxSWATFixture",
            "ChromeSWATFixture",
            "SafariSWATFixture",
			"SWATFixture",
			"Import"};

        private RichTextBox textBox;
        private System.Windows.Forms.ListBox listBox;
        private SWAT.Auto_Complete.AssemblyReader.SwatReader reader;
        List<string> commands;
        private string currentLineToCursor = "";
        //        private const String STARTUP_PATH_MODIFIER = @"..\..\..\..\SWAT\bin\Debug\SWAT.dll";
        private const String STARTUP_PATH_MODIFIER = @"\SWAT.Core.dll";
        private static int noOfPipes = 0;
        private string previousMatch = "none";
        Type interfaceClass; //used to read parameters of the different commands
        private string greatestWordOnList; //used to set the width of the listbox
        private bool useIntellisense;//used to set or unset the intellisense

        private int lastTextLength;

        #endregion
    }
}
