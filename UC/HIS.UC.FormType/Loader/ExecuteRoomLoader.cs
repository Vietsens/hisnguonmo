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
    class ExecuteRoomLoader
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboExecuteRoom)
        {
            try
            {
                cboExecuteRoom.Properties.DataSource = Config.HisFormTypeConfig.VHisExecuteRooms;
                cboExecuteRoom.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                cboExecuteRoom.Properties.ValueMember = "ID";

                cboExecuteRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboExecuteRoom.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboExecuteRoom.Properties.ImmediatePopup = true;
                cboExecuteRoom.ForceInitialize();
                cboExecuteRoom.Properties.View.Columns.Clear();
                cboExecuteRoom.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboExecuteRoom.Properties.View.Columns.AddField("EXECUTE_ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboExecuteRoom.Properties.View.Columns.AddField("EXECUTE_ROOM_NAME");
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

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboExecuteRoom, List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> listData)
        {
            try
            {
                cboExecuteRoom.Properties.DataSource = listData;
                cboExecuteRoom.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                cboExecuteRoom.Properties.ValueMember = "ID";

                cboExecuteRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboExecuteRoom.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboExecuteRoom.Properties.ImmediatePopup = true;
                cboExecuteRoom.ForceInitialize();
                cboExecuteRoom.Properties.View.Columns.Clear();
                cboExecuteRoom.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboExecuteRoom.Properties.View.Columns.AddField("EXECUTE_ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboExecuteRoom.Properties.View.Columns.AddField("EXECUTE_ROOM_NAME");
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

        internal static void LoadDataToComboExecuteRoom(DevExpress.XtraEditors.LookUpEdit cboExecuteRoom)
        {
            try
            {
                cboExecuteRoom.Properties.DataSource = Config.HisFormTypeConfig.VHisExecuteRooms;
                cboExecuteRoom.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                cboExecuteRoom.Properties.ValueMember = "ID";
                cboExecuteRoom.Properties.ForceInitialize();
                cboExecuteRoom.Properties.Columns.Clear();
                cboExecuteRoom.Properties.Columns.Add(new LookUpColumnInfo("EXECUTE_ROOM_CODE", "", 100));
                cboExecuteRoom.Properties.Columns.Add(new LookUpColumnInfo("EXECUTE_ROOM_NAME", "", 200));
                cboExecuteRoom.Properties.ShowHeader = false;
                cboExecuteRoom.Properties.ImmediatePopup = true;
                cboExecuteRoom.Properties.DropDownRows = 10;
                cboExecuteRoom.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToComboExecuteRoom(DevExpress.XtraEditors.LookUpEdit cboExecuteRoom, List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> listData)
        {
            try
            {
                cboExecuteRoom.Properties.DataSource = listData;
                cboExecuteRoom.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                cboExecuteRoom.Properties.ValueMember = "ID";
                cboExecuteRoom.Properties.ForceInitialize();
                cboExecuteRoom.Properties.Columns.Clear();
                cboExecuteRoom.Properties.Columns.Add(new LookUpColumnInfo("EXECUTE_ROOM_CODE", "", 100));
                cboExecuteRoom.Properties.Columns.Add(new LookUpColumnInfo("EXECUTE_ROOM_NAME", "", 200));
                cboExecuteRoom.Properties.ShowHeader = false;
                cboExecuteRoom.Properties.ImmediatePopup = true;
                cboExecuteRoom.Properties.DropDownRows = 10;
                cboExecuteRoom.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
