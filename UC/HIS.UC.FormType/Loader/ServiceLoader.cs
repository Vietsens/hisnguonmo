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

namespace HIS.UC.FormType.Loader
{
    class ServiceLoader
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboService)
        {
            try
            {
                cboService.Properties.DataSource = Config.HisFormTypeConfig.VHisServices;
                cboService.Properties.DisplayMember = "SERVICE_NAME";
                cboService.Properties.ValueMember = "ID";

                cboService.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboService.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboService.Properties.ImmediatePopup = true;
                cboService.ForceInitialize();
                cboService.Properties.View.Columns.Clear();
                cboService.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboService.Properties.View.Columns.AddField("SERVICE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboService.Properties.View.Columns.AddField("SERVICE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboService, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> listData)
        {
            try
            {
                cboService.Properties.DataSource = listData;
                cboService.Properties.DisplayMember = "SERVICE_NAME";
                cboService.Properties.ValueMember = "ID";

                cboService.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboService.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboService.Properties.ImmediatePopup = true;
                cboService.ForceInitialize();
                cboService.Properties.View.Columns.Clear();
                cboService.Properties.View.OptionsView.ShowColumnHeaders = true;

                GridColumn aColumnCode = cboService.Properties.View.Columns.AddField("SERVICE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboService.Properties.View.Columns.AddField("SERVICE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToCombo(DevExpress.XtraEditors.LookUpEdit cboService)
        {
            try
            {
                cboService.Properties.DataSource = Config.HisFormTypeConfig.VHisServices;
                cboService.Properties.DisplayMember = "SERVICE_NAME";
                cboService.Properties.ValueMember = "ID";
                cboService.Properties.ForceInitialize();
                cboService.Properties.Columns.Clear();
                cboService.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_CODE", "", 100));
                cboService.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_NAME", "", 200));
                cboService.Properties.ShowHeader = false;
                cboService.Properties.ImmediatePopup = true;
                cboService.Properties.DropDownRows = 10;
                cboService.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToCombo(DevExpress.XtraEditors.LookUpEdit cboService, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> listData)
        {
            try
            {
                cboService.Properties.DataSource = listData;
                cboService.Properties.DisplayMember = "SERVICE_NAME";
                cboService.Properties.ValueMember = "ID";
                cboService.Properties.ForceInitialize();
                cboService.Properties.Columns.Clear();
                cboService.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_CODE", "", 100));
                cboService.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_NAME", "", 200));
                cboService.Properties.ShowHeader = false;
                cboService.Properties.ImmediatePopup = true;
                cboService.Properties.DropDownRows = 10;
                cboService.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
