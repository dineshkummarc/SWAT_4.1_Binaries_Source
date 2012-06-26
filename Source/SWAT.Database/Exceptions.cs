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

namespace SWAT.DataAccess
{      

        #region Out of bounds

        public class IndexOutOfBoundsException : Exception
      {
          public IndexOutOfBoundsException() : base ("Index must be greater than or equal to 0")
          {    
          }

          public IndexOutOfBoundsException(string x) : base(x)
          {
          }
      }

        public class RowIndexOutOfBoundsException : IndexOutOfBoundsException
    {
        public RowIndexOutOfBoundsException(int row)
            : base(string.Format("There were less than {0} results returned in the query", row))
        {
        }
    }

        public class ColumnIndexOutOfBoundsException : IndexOutOfBoundsException
    {
        public ColumnIndexOutOfBoundsException(int col)
            : base(string.Format("There are less than {0} fields in each row of the query", col))
        {
        }
    }

        #endregion Out of bounds


        #region Results

        public class RecordNotFoundException : Exception
        {
            public RecordNotFoundException(int expected, int actual) : base(string.Format("Not enough rows returned in query. Expected {0} rows, Actual {1}" ,expected, actual))
            {}

            public RecordNotFoundException(string dbValue, string expValue) : base(string.Format("Expected value was \"{1}\". Actual DB value was \"{0}\".", dbValue, expValue))
            {}
        }

        public class QueryReturnedNoResultsException : Exception
      {
          public QueryReturnedNoResultsException( string SQL )
              : base(string.Format("Query returned no results. SQL: {0}", SQL) ) {}
      }

        public class AssertRowCountFailedException : Exception
        {
            public AssertRowCountFailedException(int expectedRows, int actualRows) 
                : base(string.Format("Expected {0} row(s) but query returned {1} row(s).", expectedRows, actualRows)) { }
        }

        #endregion Results


        #region Misc

        public class IncorrectStatementTypeException : Exception
      {
          public IncorrectStatementTypeException( string expected, string SQL ) : base(string.Format("A statement of type {0} was expected. SQL: {1}", expected, SQL))
          {}
      }

        public class ColumnDoesNotExistException : Exception
      {
          public ColumnDoesNotExistException(string name) : base(string.Format("A Column with the name of {0} was not found in the query.", name))
          {}
      }

        public class InvalidDateFormatException : Exception
        {
            public InvalidDateFormatException(int format)
                : base(string.Format("Format {0} not supported", format))
            { }
        }

        public class UnsupportedCommandException : Exception
        {
            public UnsupportedCommandException(string command, string db)
                : base(string.Format("{0} is not a supported command for {1}.", command, db))
            { }
        }

        #endregion Misc
  }
