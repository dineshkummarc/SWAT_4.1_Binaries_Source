using System.Text;

namespace SWAT.Utilities
{
    public class HtmlUtil
    {
        private StringBuilder _buffer = new StringBuilder();

        public override string ToString()
        {
            return _buffer.ToString();
        }

        public HtmlUtil GenerateHeaderWithTitle(string title)
        {
            _buffer.AppendLine("<html><head><title>" + title + " Results</title></head>");
            return this;
        }

        public HtmlUtil GenerateTextWithSize(string text, int size)
        {
            _buffer.AppendLine("<font size=" + size + ">" + text + "</font>");
            return this;
        }

        public HtmlUtil GenerateBreakline()
        {
            _buffer.AppendLine("<br>");
            return this;
        }

        public HtmlUtil GenerateFooter()
        {
            _buffer.AppendLine("</html>");
            return this;
        }

        public HtmlUtil StartCenter()
        {
            _buffer.AppendLine("<center>");
            return this;
        }

        public HtmlUtil StartBody()
        {
            _buffer.AppendLine("<body>");
            return this;
        }

        public HtmlUtil EndCenter()
        {
            _buffer.AppendLine("</center>");
            return this;
        }

        public HtmlUtil EndBody()
        {
            _buffer.AppendLine("</body>");
            return this;
        }

        public HtmlUtil GenerateTableColumnWithWidth(string column, string width)
        {
            _buffer.AppendLine("<td width=" + width + ">" + column + "</td>");
            return this;
        }

        public HtmlUtil GenerateTableHeaderWithWidth(string width)
        {
            _buffer.AppendLine("<table width=" + width + ">");
            return this;
        }

        public HtmlUtil GenerateTableFooter()
        {
            _buffer.AppendLine("</table>");
            return this;
        }

        public HtmlUtil GenerateTableRowHeader()
        {
            _buffer.AppendLine("<tr>");
            return this;
        }

        public HtmlUtil GenerateTableColumnHeaderWithWidthWithBgColor(string width, string bgColor)
        {
            _buffer.AppendLine("<td width=" + width + " bgcolor=" + bgColor + ">");
            return this;
        }

        public HtmlUtil GenerateTableColumnHeaderWithWidthWithBgColorWithAlignWithFontColor(string width, string bgColor, string alignment, string color)
        {
            _buffer.AppendLine("<td align=\"" + alignment + "\" width=" + width + " bgcolor=" + bgColor + "><font color=" + color + ">");
            return this;
        }

        public HtmlUtil GenerateDivWithStyle(string text)
        {
            _buffer.AppendLine("<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">" + text + "</div>");
            return this;
        }

        public HtmlUtil GenerateTableColumnFooter()
        {
            _buffer.AppendLine("</td>");
            return this;
        }

        public HtmlUtil GenerateTableColumn(string data)
        {
            _buffer.AppendLine("<td>" + data + "</td>");
            return this;
        }

        public HtmlUtil GenerateTableColumnWithTextColorWithBGColor(string data, string textColor, string bgColor)
        {
            _buffer.AppendLine("<td bgcolor=" + bgColor + "><font color=" + textColor + ">" + data + "</td>");
            return this;
        }

        public HtmlUtil GenerateTableRowFooter()
        {
            _buffer.AppendLine("</tr>");
            return this;
        }
    }
}
