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
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisDepartment.Base
{
    class GlobalStore
    {
        internal static void LoadDataGridLookUpEdit(DevExpress.XtraEditors.GridLookUpEdit comboEdit, string code, string name, string value, object data)
        {
            try
            {
                comboEdit.Properties.DataSource = data;
                comboEdit.Properties.DisplayMember = name;
                comboEdit.Properties.ValueMember = value;
                comboEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                comboEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                comboEdit.Properties.ImmediatePopup = true;
                comboEdit.ForceInitialize();
                comboEdit.Properties.View.Columns.Clear();
                comboEdit.Properties.PopupFormSize = new System.Drawing.Size(300, 250);

                GridColumn aColumnCode = comboEdit.Properties.View.Columns.AddField(code);
                aColumnCode.Caption = "";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = comboEdit.Properties.View.Columns.AddField(name);
                aColumnName.Caption = "";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
