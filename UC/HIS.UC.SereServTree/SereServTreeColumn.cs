/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.SereServTree
{
    public class SereServTreeColumn
    {
        public SereServTreeColumn()
        {

        }
        public SereServTreeColumn(string caption, string fieldName, int columnWidth, bool allowEdit)
            : this(caption, fieldName, columnWidth, -1, true, allowEdit, null, DevExpress.XtraTreeList.Data.UnboundColumnType.Bound)
        {

        }
        public SereServTreeColumn(string caption, string fieldName, int columnWidth, int visibleIndex, bool visible, bool allowEdit, DevExpress.Utils.FormatInfo format, DevExpress.XtraTreeList.Data.UnboundColumnType unboundType)
        {
            this.Caption = caption;
            this.FieldName = fieldName;
            this.ColumnWidth = columnWidth;
            this.VisibleIndex = visibleIndex;
            this.Visible = visible;
            this.AllowEdit = allowEdit;
            this.Format = format;
            this.UnboundType = unboundType;
        }

        public string Caption { get; set; }
        public string FieldName { get; set; }
        public int ColumnWidth { get; set; }
        public int VisibleIndex { get; set; }
        public bool Visible { get; set; }
        public bool AllowEdit { get; set; }
        public DevExpress.Utils.FormatInfo Format { get; set; }
        public DevExpress.XtraTreeList.Data.UnboundColumnType UnboundType { get; set; }
    }
}
