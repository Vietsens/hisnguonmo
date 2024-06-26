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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.SampleRoom
{
    public class SampleRoomColumn
    {
        public string Caption { get; set; }
        public string FieldName { get; set; }
        public int ColumnWidth { get; set; }
        public int VisibleIndex { get; set; }
        public bool Visible { get; set; }
        public bool AllowEdit { get; set; }
        public String ToolTip { get; set; } 
        public StringAlignment imageAlignment { get; set; }
        public Image image { get; set; }
        public DevExpress.Utils.FormatInfo Format { get; set; }
        public DevExpress.Data.UnboundColumnType UnboundColumnType { get; set; }

        public SampleRoomColumn()
        {

        }
        public SampleRoomColumn(string caption, string fieldName, int columnWidth, bool allowEdit)
            : this(caption, fieldName, columnWidth, -1, true, allowEdit, null, null, StringAlignment.Center)
        {

        }
        public SampleRoomColumn(string caption, string fieldName, int columnWidth, int visibleIndex, bool visible, bool allowEdit, DevExpress.Utils.FormatInfo format, Image image, StringAlignment imageAlignment)
        {
            this.Caption = caption;
            this.FieldName = fieldName;
            this.ColumnWidth = columnWidth;
            this.VisibleIndex = visibleIndex;
            this.Visible = visible;
            this.AllowEdit = allowEdit;
            this.Format = format;
            this.image = image;
            this.imageAlignment = imageAlignment;
        }
    }
}
