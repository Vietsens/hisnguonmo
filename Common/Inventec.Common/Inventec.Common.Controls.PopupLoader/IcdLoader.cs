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

namespace Inventec.Desktop.Loader
{
    public class IcdLoader
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboChanDoanTD, List<MOS.EFMODEL.DataModels.HIS_ICD> data)
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
                cboChanDoanTD.Properties.View.OptionsView.ShowColumnHeaders = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.LookUpEdit cboChanDoanTD)
        {

            try
            {
                cboChanDoanTD.Properties.DataSource = Inventec.Desktop.LocalStorage.Storage.HisDataLocalStore.HisIcds;
                cboChanDoanTD.Properties.DisplayMember = "ICD_NAME";
                cboChanDoanTD.Properties.ValueMember = "ID";
                cboChanDoanTD.Properties.ForceInitialize();
                cboChanDoanTD.Properties.Columns.Clear();
                cboChanDoanTD.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ICD_CODE", "", 100));
                cboChanDoanTD.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ICD_NAME", "", 200));
                cboChanDoanTD.Properties.ShowHeader = false;
                cboChanDoanTD.Properties.ImmediatePopup = true;
                cboChanDoanTD.Properties.DropDownRows = 10;
                cboChanDoanTD.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
