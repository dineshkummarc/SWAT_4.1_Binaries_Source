using System.Collections.Generic;
using System.Collections.Specialized;

namespace SWAT.Fitnesse
{
    class SlimRow : TableRow
    {
        private List<string> row;

        public SlimRow(object r)
        {
            row = new List<string>( (List<string>)r );
        }

        public override string GetCellAt(int pos)
        {
            if (pos == 0)
                return HtmlString.UnEscape(row[pos]);
            else
                return row[pos];
        }

        public override void SetCellAt(int pos, string value)
        {
            row[pos] = value;
        }

        public override StringCollection GetParameters(bool getFirstItem)
        {
            StringCollection pList = new StringCollection();

            int i = (getFirstItem)? 0 : 1;

            for (; i < row.Count; i++)
            {
                string cellText = new HtmlString(row[i]).ToPlainText();
                ReplaceSymbols(ref cellText);
                row[i] = cellText;

                pList.Add(row[i]);
            }

            return pList;
        }

        public List<string> GetRow()
        {
            return row;
        }
    }
}
