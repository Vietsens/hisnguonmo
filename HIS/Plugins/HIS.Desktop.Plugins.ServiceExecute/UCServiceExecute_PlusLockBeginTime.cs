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
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UCServiceExecute : UserControlBase
    {
        //61482: key HIS.Desktop.Plugins.IsLockSubclinicalStartTime = 1 -> thoi gian bat dau chi lay bang nut "Lay thoi gian hien tai", da luu thi khong cho sua
        private EditorButton btnGetBeginTime;

        private void ProcessLockBeginTime(HIS_SERE_SERV_EXT sereServExt)
        {
            try
            {
                if (!Config.AppConfigKeys.IsLockSubclinicalStartTime)
                    return;

                if (btnGetBeginTime == null)
                {
                    btnGetBeginTime = new EditorButton(ButtonPredefines.Redo);
                    btnGetBeginTime.ToolTip = "Lấy thời gian hiện tại";
                    dtBeginTime.Properties.Buttons.Add(btnGetBeginTime);
                    dtBeginTime.ButtonClick += dtBeginTime_ButtonClick;
                }

                bool isSaved = sereServExt != null && sereServExt.BEGIN_TIME.HasValue;
                if (!isSaved)
                    dtBeginTime.EditValue = null;
                dtBeginTime.ReadOnly = true;
                dtBeginTime.Properties.Buttons[0].Visible = false;
                btnGetBeginTime.Visible = !isSaved;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtBeginTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button != btnGetBeginTime)
                    return;

                if (sereServ != null && dicSereServExt.ContainsKey(sereServ.ID) && dicSereServExt[sereServ.ID].BEGIN_TIME.HasValue)
                {
                    btnGetBeginTime.Visible = false;
                    return;
                }
                dtBeginTime.DateTime = (currentTimer == DateTime.MinValue) ? DateTime.Now : currentTimer;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
