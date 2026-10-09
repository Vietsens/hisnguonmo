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
using HIS.Desktop.Plugins.CallPatientDrugStoreDispense.Resources;
using HIS.Desktop.Utility;
using System;

namespace HIS.Desktop.Plugins.CallPatientDrugStoreDispense
{
    public partial class frmDispenseWaitingScreen : FormBase
    {
        string textScreenTitle;
        string textScreenName;
        string textBigTitle;
        string textCallingLabel;
        string textStatusPreparing;
        string textStatusWaiting;
        string textStatusDone;
        string textDefaultInvite;

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.Text = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.Text");
                this.gcStt.Caption = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.gcStt.Caption");
                this.gcPatientName.Caption = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.gcPatientName.Caption");
                this.gcDobYear.Caption = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.gcDobYear.Caption");
                this.gcStatus.Caption = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.gcStatus.Caption");
                this.textScreenTitle = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.ScreenTitle");
                this.textScreenName = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.ScreenName");
                this.textBigTitle = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.BigTitle");
                this.textCallingLabel = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.CallingLabel");
                this.textStatusPreparing = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.StatusPreparing");
                this.textStatusWaiting = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.StatusWaiting");
                this.textStatusDone = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.StatusDone");
                this.textDefaultInvite = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.DefaultInviteText");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
