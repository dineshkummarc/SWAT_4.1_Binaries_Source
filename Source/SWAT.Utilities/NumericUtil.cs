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
    public static class NumericUtil
    {
        /// <summary>
        /// Checks if an integer is even.
        /// </summary>
        /// <param name="n">an integer.</param>
        /// <returns>true if even, false if odd.</returns>
        public static bool isEven(int n)
        {
            if (n % 2 == 0) return true;
            return false;
        }
        /// <summary>
        /// Checks if an integer is odd.
        /// </summary>
        /// <param name="n">an integer.</param>
        /// <returns>true if odd, false if even.</returns>
        public static bool isOdd(int n)
        {
            if (n % 2 == 0) return false;
            return true;
        }
    }
}
