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
