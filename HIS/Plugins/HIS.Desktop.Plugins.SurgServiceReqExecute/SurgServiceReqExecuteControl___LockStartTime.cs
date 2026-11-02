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
using HIS.Desktop.Plugins.SurgServiceReqExecute.Config;
using HIS.Desktop.Utility;
using System;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class SurgServiceReqExecuteControl : UserControlBase
    {
        //61482: key HIS.Desktop.Plugins.IsLockSubclinicalStartTime = 1 -> thoi gian bat dau chi lay bang nut "Lay thoi gian hien tai", da luu thi khong cho sua
        private EditorButton btnGetStartTime;
        private bool isStartTimeSaved;

        /// <param name="isClearUnsaved">true khi vua nap du lieu dich vu: xoa gia tri mac dinh de bat buoc bam nut. SetEnableControl truyen false de khong xoa gio nguoi dung vua lay</param>
        private void ProcessLockStartTime(bool isClearUnsaved)
        {
            try
            {
                if (!HisConfigCFG.IsLockSubclinicalStartTime)
                    return;

                if (btnGetStartTime == null)
                {
                    btnGetStartTime = new EditorButton(ButtonPredefines.Redo);
                    btnGetStartTime.ToolTip = "Lấy thời gian hiện tại";
                    dtStart.Properties.Buttons.Add(btnGetStartTime);
                }

                //SereServExt co the la ban copy tu dich vu khac (dicSereServCopy, FillDataFromSereServLast) nen phai so SERE_SERV_ID
                isStartTimeSaved = this.sereServ != null && this.SereServExt != null
                    && this.SereServExt.SERE_SERV_ID == this.sereServ.ID && this.SereServExt.BEGIN_TIME.HasValue;
                if (!isStartTimeSaved && isClearUnsaved)
                    dtStart.EditValue = null;
                dtStart.ReadOnly = true;
                dtStart.Properties.Buttons[0].Visible = false;
                dtStart.Properties.Buttons[1].Visible = false;
                btnGetStartTime.Visible = !isStartTimeSaved;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
