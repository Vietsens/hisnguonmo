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
using Inventec.Desktop.CustomControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisServiceHein
{
    public class InitComboSearchCode
    {
        public static void Init(CustomGridLookUpEditWithFilterMultiColumn cbo, object data, string diplayMember, string valueMember, Size size, List<ColumnInfo> columnInfos)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = diplayMember;
                cbo.Properties.ValueMember = valueMember;
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = size;

                if (columnInfos != null && columnInfos.Count > 0)
                {
                    foreach (var item in columnInfos)
                    {
                        var column = cbo.Properties.View.Columns.AddField(item.FieldName);
                        column.Caption = item.Caption;
                        column.Visible = item.Visible;
                        column.VisibleIndex = item.VisibleIndex;
                        column.Width = item.Width;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }

    public class ColumnInfo
    {
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public bool Visible { get; set; }
        public int VisibleIndex { get; set; }
        public int Width { get; set; }

        public ColumnInfo(string fieldName, string caption, bool visible, int visibleIndex, int width)
        {
            this.FieldName = fieldName;
            this.Caption = caption;
            this.Visible = visible;
            this.VisibleIndex = visibleIndex;
            this.Width = width;
        }
    }

}
