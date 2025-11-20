using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TestSample
{
    public class TestSampleColumn
    {
        public string Caption { get; set; }
        public string FieldName { get; set; }
        public int ColumnWidth { get; set; }
        public int VisibleIndex { get; set; }
        public bool Visible { get; set; }
        public bool AllowEdit { get; set; }
        public System.Drawing.Image image { get; set; }
        public DevExpress.Utils.FormatInfo Format { get; set; }
        public DevExpress.Data.UnboundColumnType UnboundColumnType { get; set; }
        public string Tooltip { get; set; }

        public TestSampleColumn()
        {

        }
        public TestSampleColumn(string caption, string fieldName, int columnWidth, bool allowEdit)
            : this(caption, fieldName, columnWidth, -1, true, allowEdit, null)
        {

        }

        public TestSampleColumn(string caption, string fieldName, int columnWidth, int visibleIndex, bool visible, bool allowEdit, DevExpress.Utils.FormatInfo format)
        {
            this.Caption = caption;
            this.FieldName = fieldName;
            this.ColumnWidth = columnWidth;
            this.VisibleIndex = visibleIndex;
            this.Visible = visible;
            this.AllowEdit = allowEdit;
            this.Format = format;
        }
    }
}
