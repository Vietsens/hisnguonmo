using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Controls.EditorLoader
{
    public class ControlEditorADO
    {
        internal const int DEFAULT__POPUP_WIDTH = 300;
        internal const int DEFAULT__COLUMN_WIDTH = 100;
        internal const int DEFAULT__DROP_DOWN_ROW = 10;
        public ControlEditorADO()
        { }

        public ControlEditorADO(string _DisplayMember, string _ValueMember, List<ColumnInfo> _ColumnInfos)
            : this(_DisplayMember, _ValueMember, _ColumnInfos, false, DEFAULT__POPUP_WIDTH, DEFAULT__DROP_DOWN_ROW)
        {

        }

        public ControlEditorADO(string _DisplayMember, string _ValueMember, List<ColumnInfo> _ColumnInfos, bool _ShowHeader)
            : this(_DisplayMember, _ValueMember, _ColumnInfos, _ShowHeader, DEFAULT__POPUP_WIDTH, DEFAULT__DROP_DOWN_ROW)
        {

        }

        public ControlEditorADO(string _DisplayMember, string _ValueMember, List<ColumnInfo> _ColumnInfos, bool _ShowHeader, int _PopupWidth)
            : this(_DisplayMember, _ValueMember, _ColumnInfos, _ShowHeader, _PopupWidth, DEFAULT__DROP_DOWN_ROW)
        {

        }

        public ControlEditorADO(string _DisplayMember, string _ValueMember, List<ColumnInfo> _ColumnInfos, bool _ShowHeader, int _PopupWidth, int _DropDownRows)
        {
            this.DisplayMember = _DisplayMember;
            this.ValueMember = _ValueMember;
            this.ColumnInfos = _ColumnInfos;
            this.ShowHeader = _ShowHeader;
            this.PopupWidth = _PopupWidth;
            this.DropDownRows = _DropDownRows;
        }

        public string DisplayMember { get; set; }
        public string ValueMember { get; set; }
        public bool ShowHeader { get; set; }
        public bool ImmediatePopup { get; set; }
        public int DropDownRows { get; set; }
        public int PopupWidth { get; set; }
        public List<ColumnInfo> ColumnInfos { get; set; }
    }
}
