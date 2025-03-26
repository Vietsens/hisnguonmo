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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Loader
{
    class UserNameLoader
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit comboBoxEdit1)
        {
            try
            {
                comboBoxEdit1.Properties.DataSource = Config.AcsFormTypeConfig.HisAcsUserCashier;

                comboBoxEdit1.Properties.DisplayMember = "USERNAME";
                comboBoxEdit1.Properties.ValueMember = "LOGINNAME";

                comboBoxEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                comboBoxEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                comboBoxEdit1.Properties.ImmediatePopup = true;
                comboBoxEdit1.ForceInitialize();
                comboBoxEdit1.Properties.View.Columns.Clear();
                comboBoxEdit1.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = comboBoxEdit1.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = comboBoxEdit1.Properties.View.Columns.AddField("USERNAME");
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

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboRoom, List<MOS.EFMODEL.DataModels.V_HIS_ROOM> listData)
        {
            try
            {
                cboRoom.Properties.DataSource = listData;
                cboRoom.Properties.DisplayMember = "ROOM_NAME";
                cboRoom.Properties.ValueMember = "ID";

                cboRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboRoom.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboRoom.Properties.ImmediatePopup = true;
                cboRoom.ForceInitialize();
                cboRoom.Properties.View.Columns.Clear();
                cboRoom.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboRoom.Properties.View.Columns.AddField("ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboRoom.Properties.View.Columns.AddField("ROOM_NAME");
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

        internal static void LoadDataToCombo(DevExpress.XtraEditors.LookUpEdit cboRoom)
        {
            try
            {
                cboRoom.Properties.DataSource = Config.HisFormTypeConfig.VHisRooms;
                cboRoom.Properties.DisplayMember = "ROOM_NAME";
                cboRoom.Properties.ValueMember = "ID";
                cboRoom.Properties.ForceInitialize();
                cboRoom.Properties.Columns.Clear();
                cboRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_CODE", "", 100));
                cboRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_NAME", "", 200));
                cboRoom.Properties.ShowHeader = false;
                cboRoom.Properties.ImmediatePopup = true;
                cboRoom.Properties.DropDownRows = 10;
                cboRoom.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToCombo(DevExpress.XtraEditors.LookUpEdit cboRoom, List<MOS.EFMODEL.DataModels.V_HIS_ROOM> listData)
        {
            try
            {
                cboRoom.Properties.DataSource = listData;
                cboRoom.Properties.DisplayMember = "ROOM_NAME";
                cboRoom.Properties.ValueMember = "ID";
                cboRoom.Properties.ForceInitialize();
                cboRoom.Properties.Columns.Clear();
                cboRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_CODE", "", 100));
                cboRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_NAME", "", 200));
                cboRoom.Properties.ShowHeader = false;
                cboRoom.Properties.ImmediatePopup = true;
                cboRoom.Properties.DropDownRows = 10;
                cboRoom.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
