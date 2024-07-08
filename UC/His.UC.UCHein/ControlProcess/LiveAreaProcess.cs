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
    public class LiveAreaProcess
    {
        public static void LoadDataToComboNoiSong(DevExpress.XtraEditors.GridLookUpEdit cboNoiSong, List<MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData> data)
        {
            try
            {
                //cboNoiSong.Properties.DataSource = data;
                //cboNoiSong.Properties.DisplayMember = "HeinLiveName";
                //cboNoiSong.Properties.ValueMember = "HeinLiveCode";
                //cboNoiSong.Properties.ForceInitialize();
                //cboNoiSong.Properties.Columns.Clear();
                //cboNoiSong.Properties.Columns.Add(new LookUpColumnInfo("HeinLiveCode", "", 100));
                //cboNoiSong.Properties.Columns.Add(new LookUpColumnInfo("HeinLiveName", "", 200));
                //cboNoiSong.Properties.ShowHeader = false;
                //cboNoiSong.Properties.ImmediatePopup = true;
                //cboNoiSong.Properties.DropDownRows = 20;
                //cboNoiSong.Properties.PopupWidth = 300;

                cboNoiSong.Properties.DataSource = data;
                cboNoiSong.Properties.DisplayMember = "HeinLiveName";
                cboNoiSong.Properties.ValueMember = "HeinLiveCode";

                cboNoiSong.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboNoiSong.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboNoiSong.Properties.ImmediatePopup = true;
                cboNoiSong.ForceInitialize();
                cboNoiSong.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboNoiSong.Properties.View.Columns.AddField("HeinLiveCode");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 70;

                GridColumn aColumnName = cboNoiSong.Properties.View.Columns.AddField("HeinLiveName");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
