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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.ControlProcess
{
    public class MediOrgProcess
    {
        public static void LoadDataToComboNoiDKKCBBD(DevExpress.XtraEditors.GridLookUpEdit cboMediOrg, object data)
        {
            try
            {
                cboMediOrg.Properties.DataSource = data;
                cboMediOrg.Properties.DisplayMember = "MEDI_ORG_NAME";
                cboMediOrg.Properties.ValueMember = "MEDI_ORG_CODE";

                cboMediOrg.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboMediOrg.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboMediOrg.Properties.ImmediatePopup = true;
                cboMediOrg.ForceInitialize();
                cboMediOrg.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboMediOrg.Properties.View.Columns.AddField("MEDI_ORG_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 70;

                GridColumn aColumnName = cboMediOrg.Properties.View.Columns.AddField("MEDI_ORG_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 300;

                GridColumn aColumnNameUnsigned = cboMediOrg.Properties.View.Columns.AddField("MEDI_ORG_NAME_UNSIGNED");
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
