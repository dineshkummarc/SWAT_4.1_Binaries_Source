using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace SWAT.Fitnesse
{
    public abstract class TableRow
    {
        private static Regex _symbolReplacementExpression = new Regex(@">>.+?<<");

        #region Abstract methods

        public abstract string GetCellAt(int pos);

        public abstract void SetCellAt(int pos, string value);

        public abstract StringCollection GetParameters(bool getFirstItem);

        #endregion


        #region Concrete methods

        public StringCollection GetParameters()
        {
            return GetParameters(false);
        }

        protected void ReplaceSymbols(ref string cellText)
        {
            foreach (Match match in _symbolReplacementExpression.Matches(cellText))
            {
                object symbol = TableHandler.VarRetriever.Recall(match.Value.TrimStart('>').TrimEnd('<'));

                if (symbol != null)
                {
                    cellText = cellText.Replace(match.Value, (string)symbol);
                }
            }
        }

        #endregion
    }
}
