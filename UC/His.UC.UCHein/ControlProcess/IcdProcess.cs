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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.ControlProcess
{
    public class IcdProcess
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboChanDoanTD, object data)
        {
            try
            {
                cboChanDoanTD.Properties.DataSource = data;
                cboChanDoanTD.Properties.DisplayMember = "ICD_NAME";
                cboChanDoanTD.Properties.ValueMember = "ID";

                cboChanDoanTD.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboChanDoanTD.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboChanDoanTD.Properties.ImmediatePopup = true;
                cboChanDoanTD.ForceInitialize();
                cboChanDoanTD.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboChanDoanTD.Properties.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboChanDoanTD.Properties.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 400;

                GridColumn aColumnNameUnsigned = cboChanDoanTD.Properties.View.Columns.AddField("ICD_NAME_UNSIGNED");
                aColumnNameUnsigned.Caption = "Tên";
                aColumnNameUnsigned.Visible = false;
                aColumnNameUnsigned.VisibleIndex = -1;
                aColumnNameUnsigned.Width = 5;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
