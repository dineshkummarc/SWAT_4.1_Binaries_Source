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
using fit;
using SWAT.AbstractionEngine;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using fitnesse.fixtures;
using System.Reflection;
using System.ComponentModel;
using SWAT.Fitnesse;

namespace SWAT.Fitnesse
{
    public class SWATFixture : Fixture
    {
        private TableHandler Handler;
        private bool InDebugMode = false;

        #region Constructor
        public SWATFixture(BrowserType browserType) // this constructor gets called when starting test (i.e. InternetExplorerSWATFixture).
            : base(browserType)
        {
            Handler = new TableHandler(browserType);
            Setup();
        }

        public SWATFixture() //blank constructor for calling commands
        {
            Setup();
        }

        private void Setup()
        {
            this.Processor = new fit.Service.Service(new fitSharp.Machine.Engine.Configuration());
        }

        #endregion Constructor and Setup

        #region Fitnesse output

        public override void DoTable(Parse table)
        {
            TestManager.ResetForNewTable();

            base.DoTable(table);

            if(TableHandler._finishBlockOnFailure)
            {
                TestManager.FinishBlockOnFailure = true;
                TableHandler._finishBlockOnFailure = false;
            }
        }

        public override void DoRow(Parse row)
        {
            FitRow fRow = new FitRow(row);
            TableHandler.ProcessRow(fRow);
            
            HandleRowResult(ref row);
        }

        #endregion Fitnesse output

        #region Utilities

        private void MarkRow(Parse row, List<string> messages)
        {
            for (int colIndex = 0; colIndex < messages.Count; colIndex++)
            {
                string curMsg = messages[colIndex];

                if (curMsg.Equals("pass"))
                    base.Right(row.Parts.At(colIndex));
                else
                {
                    if (curMsg != null)
                        base.Wrong(row.Parts.At(colIndex), curMsg);

                    if (InDebugMode)
                        throw new SWAT.AssertionFailedException("An assertion in SWATFixture/MarkRow has failed.");
                }
            }

        }

        private void HandleRowResult(ref Parse row)
        {
            if (TableHandler.Status == RowStatus.Skipped) //Only for BeginCompareData column names
                return;
            else if (TableHandler.Status == RowStatus.Ignored) //Only if (!TestManager.ShouldExecute(..))
                base.Ignore(row);
            else
            {
                if (TableHandler.Status == RowStatus.Passed)
                    base.Right(row);
                else if (TableHandler.Status == RowStatus.Failed)
                {
                    base.Wrong(row.Parts.At(0), TableHandler.Messages[0]);

                    if (InDebugMode)
                        throw new SWAT.AssertionFailedException("An assertion in SWATFixture/MarkRow has failed.");
                }
                else if (TableHandler.Status == RowStatus.Varied)
                    MarkRow(row, TableHandler.Messages);
            }
        }

        //private bool isSWATFixture(string className)
        //{
        //    return className.Contains("SWAT");
        //}

        #endregion
    }
}
