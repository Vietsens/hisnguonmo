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
using HIS.Desktop.Plugins.CallPatientDrugStoreCashier.Resources;
using HIS.Desktop.Utility;
using System;

namespace HIS.Desktop.Plugins.CallPatientDrugStoreCashier
{
    public partial class frmCashierScreenConfig : FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.Text");
                this.lciMediStock.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciMediStock.Text");
                this.lciBillFilter.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciBillFilter.Text");
                this.radioBillFilter.Properties.Items[0].Description = ResourceLanguageManager.GetValue("frmCashierScreenConfig.radioBillFilter.All");
                this.radioBillFilter.Properties.Items[1].Description = ResourceLanguageManager.GetValue("frmCashierScreenConfig.radioBillFilter.NotBilled");
                this.radioBillFilter.Properties.Items[2].Description = ResourceLanguageManager.GetValue("frmCashierScreenConfig.radioBillFilter.Billed");
                this.lciHideAmount.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciHideAmount.Text");
                this.chkHideAmount.Properties.Caption = ResourceLanguageManager.GetValue("frmCashierScreenConfig.chkHideAmount.Properties.Caption");
                this.lblDisplayHeader.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lblDisplayHeader.Text");
                this.lciTitleSize.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciTitleSize.Text");
                this.lciTitleColor.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciTitleColor.Text");
                this.lciListSize.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciListSize.Text");
                this.lciListColor.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciListColor.Text");
                this.lciCallingSize.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciCallingSize.Text");
                this.lciCallingColor.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciCallingColor.Text");
                this.lciCallingBackColor.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciCallingBackColor.Text");
                this.lciInviteText.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.lciInviteText.Text");
                this.txtInviteText.Properties.NullValuePrompt = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.DefaultInviteText");
                this.btnOpen.Text = ResourceLanguageManager.GetValue("frmCashierScreenConfig.btnOpen.Text");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
