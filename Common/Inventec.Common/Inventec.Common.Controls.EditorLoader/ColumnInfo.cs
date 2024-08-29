using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Controls.EditorLoader
{
    public class ColumnInfo
    {
        public enum FormatType
        {
            None = 0,
            Numeric = 1,
            DateTime = 2,
            Custom = 3,
        }
        public enum HorzAlignment
        {
            Default = 0,
            Near = 1,
            Center = 2,
            Far = 3,
        }
        public ColumnInfo() { }

        public ColumnInfo(string _fieldName, string _caption, int _width, int _VisibleIndex)
            : this(_fieldName, _caption, _width, _VisibleIndex, false)
        {
        }

        public ColumnInfo(string _fieldName, string _caption, int _width, int _VisibleIndex, bool _FixedWidth)
        {
            this.fieldName = _fieldName;
            this.caption = _caption;
            this.width = _width;
            this.VisibleIndex = _VisibleIndex;
            this.FixedWidth = _FixedWidth;
        }

        public string fieldName { get; set; }
        public string caption { get; set; }
        public int width { get; set; }
        public FormatType formatType { get; set; }
        public HorzAlignment horzAlignment { get; set; }
        public string formatString { get; set; }
        public bool visible = true;
        public int VisibleIndex { get; set; }
        public bool FixedWidth = true;
    }
}
