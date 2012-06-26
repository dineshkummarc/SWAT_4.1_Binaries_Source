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

namespace SWAT
{
    public class KeyboardInput : IKeyboard
    {
        private List<NativeMethods.INPUT32> _keyboardInputs32;
        private List<NativeMethods.INPUT64> _keyboardInputs64;
        private IBrowser _browser;
        private bool forceSixtyFourBit = false;

        private class KeyInfo
        {
            public byte VirtualKeyCode { get; set; }
            public ushort ScanCode { get; set; }
            public ShiftState ShiftState { get; set; }
        }

        #region Enumerations

        public enum KeyEvent
        {
            KeyDown,
            KeyUp
        }

        public enum ShiftState : ushort
        {
            None = 0,
            Shift = 1,
            Ctrl = 2,
            Alt = 4
        }

        #endregion

        #region Constructors

        public KeyboardInput(IBrowser browser)
        {
            _browser = browser;
            _keyboardInputs32 = new List<NativeMethods.INPUT32>(50);
            _keyboardInputs64 = new List<NativeMethods.INPUT64>(50);
        }

        #endregion

        #region Private Helper Methods

        private bool sendMessage()
        {
            NativeMethods.INPUT32[] inputString32 = _keyboardInputs32.ToArray();
            NativeMethods.INPUT64[] inputString64 = _keyboardInputs64.ToArray();

            uint flag = 0;
            //if 32-bit machine
            if (IntPtr.Size == 4 && !forceSixtyFourBit)
            {
                int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(NativeMethods.INPUT32));
                foreach (NativeMethods.INPUT32 keyInput in inputString32)
                {
                    flag = NativeMethods.SendInput(1, new NativeMethods.INPUT32[] { keyInput }, size);
                    System.Threading.Thread.Sleep(10);
                }
            }
            //else for 64-bit machine
            else
            {
                int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(NativeMethods.INPUT64));
                foreach (NativeMethods.INPUT64 keyInput in inputString64)
                {
                    flag = NativeMethods.SendInput(1, new NativeMethods.INPUT64[] { keyInput }, size);
                    System.Threading.Thread.Sleep(5);
                }
            }
            return flag > 0;
        }

        private void resetBuffer()
        {
            _keyboardInputs32 = new List<NativeMethods.INPUT32>(50);
            _keyboardInputs64 = new List<NativeMethods.INPUT64>(50);
        }

        private void bufferKey(byte virtualKeyCode, ushort scanCode, KeyEvent keyEvent)
        {
            //if 32-bit machine
            if (IntPtr.Size == 4 && !forceSixtyFourBit)
                bufferKey32bit(virtualKeyCode, scanCode, keyEvent);
            //else for 64-bit machine
            else
                bufferKey64bit(virtualKeyCode, scanCode, keyEvent);
        }

        private void bufferKey32bit(byte virtualKeyCode, ushort scanCode, KeyEvent keyEvent)
        {
            NativeMethods.INPUT32 input = new NativeMethods.INPUT32();
            input.type = NativeMethods.INPUT_KEYBOARD;
            input.ki.wVk = (byte)virtualKeyCode;
            input.ki.wScan = scanCode;

            if (keyEvent == KeyEvent.KeyUp)
                input.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;

            _keyboardInputs32.Add(input);
        }

        private void bufferKey64bit(byte virtualKeyCode, ushort scanCode, KeyEvent keyEvent)
        {
            NativeMethods.INPUT64 input = new NativeMethods.INPUT64();
            input.type = NativeMethods.INPUT_KEYBOARD;
            input.ki.wVk = (byte)virtualKeyCode;
            input.ki.wScan = scanCode;

            if (keyEvent == KeyEvent.KeyUp)
                input.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;

            _keyboardInputs64.Add(input);
        }

        private bool processShiftState(ShiftState shiftState, KeyEvent keyEvent)
        {
            switch (shiftState)
            {
                case ShiftState.None: return false;

                case ShiftState.Shift:
                    bufferKey((byte)NativeMethods.VK_SHIFT, NativeMethods.SHIFT_CODE, keyEvent);
                    break;
                case ShiftState.Ctrl:
                    bufferKey((byte)NativeMethods.VK_CONTROL, NativeMethods.CTRL_CODE, keyEvent);
                    break;
                case ShiftState.Alt:
                    bufferKey((byte)NativeMethods.VK_MENU, NativeMethods.ALT_CODE, keyEvent);
                    break;

                default:
                    throw new Exception("Unsupported shift state detected. Cannot process PressKey request.");
            }

            return true;
        }

        private uint getNumpadVkCode(uint num)
        {
            switch (num)
            {
                case 0: return 0x60;
                case 1: return 0x61;
                case 2: return 0x62;
                case 3: return 0x63;
                case 4: return 0x64;
                case 5: return 0x65;
                case 6: return 0x66;
                case 7: return 0x67;
                case 8: return 0x68;
                case 9: return 0x69;
            }

            return 0;
        }

		//new

		private bool AttachDetachThreadInput(uint currentThreadId, uint otherThreadId, bool attach)
		{
			return NativeMethods.AttachThreadInput(currentThreadId, otherThreadId, attach);
		}

        #endregion

        #region Public Methods

		public bool SetFocusToWindow(IntPtr windowHandle)
		{
            StringBuilder windowTitle = new StringBuilder(200);
            StringBuilder windowClass = new StringBuilder(200);
            NativeMethods.GetWindowText(windowHandle, windowTitle, 200);
            NativeMethods.GetClassName(windowHandle, windowClass, 200);
            System.Console.WriteLine(String.Format("Entered SetFocusToWindow for window title: {0} and class name {1}", windowTitle, windowClass));
			uint currentThreadId = NativeMethods.GetCurrentThreadId();
			uint processId = 0;
			uint otherThreadId =
				NativeMethods.GetWindowThreadProcessId(windowHandle, out processId);

			bool attached = AttachDetachThreadInput(currentThreadId, otherThreadId, true);
            IntPtr previouslyFocusedWindowHandle = NativeMethods.SetActiveWindow(windowHandle);
            NativeMethods.SwitchToThisWindow(windowHandle, true);
			if (attached)
			{
				attached = AttachDetachThreadInput(currentThreadId, otherThreadId, false);
			}

            System.Console.WriteLine("Exiting SetFocusToWindow");

            return (previouslyFocusedWindowHandle != IntPtr.Zero);
		}

        public bool SetForegroundWindowEx(IntPtr handle)
        {
            StringBuilder windowTitle = new StringBuilder(200);
            StringBuilder windowClass = new StringBuilder(200);
            NativeMethods.GetWindowText(handle, windowTitle, 200);
            NativeMethods.GetClassName(handle, windowClass, 200);
            System.Console.WriteLine(String.Format("Entered SetForegroundWindowEx for window title (from handle) = {0} and window class: {1}", windowTitle, windowClass));
            SetFocusToWindow(handle);
            //NativeMethods.SetForegroundWindow(handle);

            _browser.Sleep(100);

            NativeMethods.GetWindowText(handle, windowTitle, 200);
            NativeMethods.GetClassName(handle, windowClass, 200);
            System.Console.WriteLine(String.Format("Exiting SetForegroundWindowEx. ForegroundWindow is: {0} with window class: {1}", windowTitle, windowClass));

            return NativeMethods.GetForegroundWindow() == handle;
        }

        public void ProcessInternationalKey(string asciiCode)
        {
            try
            {
                bool fireKeyUpForShiftState = processShiftState(ShiftState.Alt, KeyEvent.KeyDown);

                char[] numbers = asciiCode.ToCharArray();
                foreach (char number in numbers)
                    this.ProcessKey(getNumpadVkCode(uint.Parse(number.ToString())));

                if (fireKeyUpForShiftState)
                    processShiftState(ShiftState.Alt, KeyEvent.KeyUp);
            }
            catch (Exception e)
            {
                resetBuffer();
                throw new ArgumentException("Invalid key detected. Cannot process key.", e);
            }
        }

        public void ProcessAltKeyCombination(string keyValue)
        {
            try
            {
                char[] separator = { '+' };
                string[] altCombination = keyValue.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                System.Text.RegularExpressions.Regex alphaNumeric = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9]");

                if (!((altCombination.Length > 1) && (altCombination[0].Length == 3)
                        && (altCombination[1].Length == 1) && (!alphaNumeric.IsMatch(altCombination[1]))))
                    throw new Exception();

                KeyInfo keyInfo = new KeyInfo();
                keyInfo.VirtualKeyCode = (byte)NativeMethods.VkKeyScan(char.Parse(altCombination[1].ToLower())); // low byte contains virtual key code
                keyInfo.ScanCode = (ushort)NativeMethods.MapVirtualKey(keyInfo.VirtualKeyCode, NativeMethods.MAPVK_VK_TO_VSC);
                keyInfo.ShiftState = ShiftState.Alt;

                bool fireKeyUpForShiftState = false;

                if (keyInfo.ShiftState != ShiftState.None)
                    fireKeyUpForShiftState = processShiftState(keyInfo.ShiftState, KeyEvent.KeyDown);

                bufferKey(keyInfo.VirtualKeyCode, keyInfo.ScanCode, KeyEvent.KeyDown);
                bufferKey(keyInfo.VirtualKeyCode, keyInfo.ScanCode, KeyEvent.KeyUp);

                if (fireKeyUpForShiftState)
                    processShiftState(keyInfo.ShiftState, KeyEvent.KeyUp);
            }
            catch (Exception e)
            {
                resetBuffer();
                throw new ArgumentException("Inproper ALT+CHAR combination. ALT cannot be used alone, and only alphanumeric characters can be appended.", e);
            }
        }

        public void ProcessShiftMappedKey(uint mappedKey)
        {
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.VirtualKeyCode = (byte)mappedKey;
            keyInfo.ScanCode = (ushort)NativeMethods.MapVirtualKey(keyInfo.VirtualKeyCode, NativeMethods.MAPVK_VK_TO_VSC);
            keyInfo.ShiftState = ShiftState.Shift;

            bool fireKeyUpForShiftState = processShiftState(keyInfo.ShiftState, KeyEvent.KeyDown);
            bufferKey(keyInfo.VirtualKeyCode, keyInfo.ScanCode, KeyEvent.KeyDown);
            bufferKey(keyInfo.VirtualKeyCode, keyInfo.ScanCode, KeyEvent.KeyUp);
            if (fireKeyUpForShiftState)
            {
                processShiftState(keyInfo.ShiftState, KeyEvent.KeyUp);
            }
        }

        /// <summary>
        /// Adds keyboard input keys to the _keyboardInputs buffer.
        /// </summary>
        /// <param name="keyToPressCode"></param>
        /// <param name="windowTitle"></param>
        public void ProcessKey(uint keyToPressCode)
        {
            try
            {
                KeyInfo keyInfo = new KeyInfo();
                keyInfo.VirtualKeyCode = (byte)keyToPressCode; // low byte contains virtual key code
                keyInfo.ScanCode = (ushort)NativeMethods.MapVirtualKey(keyInfo.VirtualKeyCode, NativeMethods.MAPVK_VK_TO_VSC);
                keyInfo.ShiftState = (ShiftState)(keyToPressCode >> 8);

                bool fireKeyUpForShiftState = false;

                if (keyInfo.ShiftState != ShiftState.None)
                    fireKeyUpForShiftState = processShiftState(keyInfo.ShiftState, KeyEvent.KeyDown);

                bufferKey(keyInfo.VirtualKeyCode, keyInfo.ScanCode, KeyEvent.KeyDown);
                bufferKey(keyInfo.VirtualKeyCode, keyInfo.ScanCode, KeyEvent.KeyUp);

                if (fireKeyUpForShiftState)
                    processShiftState(keyInfo.ShiftState, KeyEvent.KeyUp);
            }
            catch (Exception e)
            {
                resetBuffer();
                throw new ArgumentException("Invalid key detected. Cannot process key.", e);
            }
        }

        /// <summary>
        /// Sends all _keyboardInputs in queue and resets the buffer.
        /// </summary>
        /// <param name="windowTitle"></param>
        public bool SendInputString(string windowTitle)
        {
            System.Console.WriteLine(String.Format("Entered SendInputString for windowTitle: {0}", windowTitle));
            if(!windowTitle.Contains("Modal Dialog"))
                _browser.SetCurrentWindowHandle(windowTitle);

            if (_browser.GetCurrentWindowHandle() == null || _browser.GetCurrentWindowHandle() == IntPtr.Zero)
                throw new Exception("Unable to press keys becuase the current window handle is not set.");

            DateTime endTime = DateTime.Now.AddSeconds(15);

            bool sentInput = false;
            while (DateTime.Now < endTime)
            {
                if (SetForegroundWindowEx(_browser.GetCurrentWindowHandle()))
                //if (SetFocusToWindow(_browser.GetCurrentWindowHandle()))
                {
                    System.Console.WriteLine(String.Format("_browser.GetCurrentWindowHandle() true: About to SendInput for windowTitle = {0}", windowTitle));
                    sentInput = sendMessage();
                    if (!sentInput)
                        throw new PressKeysFailureException();
                    break;
                }
            }

            System.Console.WriteLine(String.Format("Out of while loop in SendInputString. SentInput is ", sentInput.ToString()));

            resetBuffer();
            
            return sentInput;
        }

        public void Copy(IntPtr hWd)
        {
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.VirtualKeyCode = (byte)NativeMethods.VkKeyScan('c'); // low byte contains virtual key code
            keyInfo.ScanCode = (ushort)NativeMethods.MapVirtualKey(keyInfo.VirtualKeyCode, NativeMethods.MAPVK_VK_TO_VSC);
            keyInfo.ShiftState = ShiftState.Ctrl;

            bool fireKeyUpForShiftState = false;

            fireKeyUpForShiftState = processShiftState(keyInfo.ShiftState, KeyEvent.KeyDown);

            bufferKey(keyInfo.VirtualKeyCode, keyInfo.ScanCode, KeyEvent.KeyDown);
            bufferKey(keyInfo.VirtualKeyCode, keyInfo.ScanCode, KeyEvent.KeyUp);

            if (fireKeyUpForShiftState)
                processShiftState(keyInfo.ShiftState, KeyEvent.KeyUp);

            SetForegroundWindowEx(hWd);

            sendMessage();
        }

        #endregion
    }
}
