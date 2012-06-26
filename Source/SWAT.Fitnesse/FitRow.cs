using System.Collections.Specialized;
using fit;

namespace SWAT.Fitnesse
{
    class FitRow : TableRow
    {
        private Parse row;

        public FitRow(object r)
        {
            row = (Parse)r;
        }

        public override string GetCellAt(int pos)
        {
            return replaceCharacterEntities(row.Parts.At(pos).Body);
        }

        public override void SetCellAt(int pos, string value)
        {
            row.Parts.At(pos).SetBody(value); 
        }

        public override StringCollection GetParameters(bool getFirstItem)
        {
            StringCollection l = new StringCollection();
            Parse cells = row.Leaf;

            if (!getFirstItem)
                cells = cells.More;

            while (cells != null)
            {
                string str = cells.Body;
                HtmlString htmlStr = null;
                try
                {
                    htmlStr = new HtmlString(str);
                }
                catch (System.Exception)
                {
                    throw new System.Exception("Caughts fit exception, str = " + str);
                }

                string cellText = htmlStr.ToPlainText();
                ReplaceSymbols(ref cellText);
                cells.SetBody(cellText);
                l.Add(cellText);
                cells = cells.More;
            }

            return l;
        }

        private string replaceCharacterEntities(string row)
        {
            row = row.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");
            return row;
        }
    }
}
