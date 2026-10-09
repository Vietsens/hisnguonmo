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
    public partial class frmDispenseScreenConfig : FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.Text");
                this.lciMediStock.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lciMediStock.Text");
                this.lciPreparing.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lciPreparing.Text");
                this.chkPreparing.Properties.Caption = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.chkPreparing.Properties.Caption");
                this.chkWaiting.Properties.Caption = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.chkWaiting.Properties.Caption");
                this.chkDone.Properties.Caption = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.chkDone.Properties.Caption");
                this.lblDisplayHeader.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lblDisplayHeader.Text");
                this.lciTitleSize.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lciTitleSize.Text");
                this.lciTitleColor.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lciTitleColor.Text");
                this.lciListSize.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lciListSize.Text");
                this.lciListColor.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lciListColor.Text");
                this.lciCallingSize.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lciCallingSize.Text");
                this.lciCallingColor.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lciCallingColor.Text");
                this.lciCallingBackColor.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lciCallingBackColor.Text");
                this.lciInviteText.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.lciInviteText.Text");
                this.txtInviteText.Properties.NullValuePrompt = ResourceLanguageManager.GetValue("frmDispenseWaitingScreen.DefaultInviteText");
                this.btnOpen.Text = ResourceLanguageManager.GetValue("frmDispenseScreenConfig.btnOpen.Text");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
