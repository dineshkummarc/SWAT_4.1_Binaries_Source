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
    public enum BrowserType
    {
        InternetExplorer = 0,
        FireFox = 1,
        Safari = 2,
        Chrome = 3,
        Null = 4
    }

    public class BrowserFactory
    {
        public static IBrowser CreateBrowser(BrowserType browserType)
        {
            switch (browserType)
            {
#if MACOSX
				 default:
                    return new Safari();	
#else
                case BrowserType.InternetExplorer: 
                    return new InternetExplorer();

                case BrowserType.FireFox:
                    return new FireFox();

                case BrowserType.Safari:
                    return new Safari();

                case BrowserType.Chrome:
                    return new Chrome();

                default:
                    return new InternetExplorer();


#endif
            }

        }
    }
}
