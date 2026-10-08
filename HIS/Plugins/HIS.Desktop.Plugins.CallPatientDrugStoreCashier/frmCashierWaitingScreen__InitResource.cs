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
    public partial class frmCashierWaitingScreen : FormBase
    {
        string textScreenTitle;
        string textScreenName;
        string textBigTitle;
        string textCallingLabel;
        string textAmountLabel;
        string textStatusBilled;
        string textStatusNotBilled;
        string textDefaultInvite;

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.Text = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.Text");
                this.gcStt.Caption = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.gcStt.Caption");
                this.gcPatientName.Caption = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.gcPatientName.Caption");
                this.gcDobYear.Caption = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.gcDobYear.Caption");
                this.gcAmount.Caption = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.gcAmount.Caption");
                this.gcStatus.Caption = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.gcStatus.Caption");
                this.textScreenTitle = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.ScreenTitle");
                this.textScreenName = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.ScreenName");
                this.textBigTitle = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.BigTitle");
                this.textCallingLabel = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.CallingLabel");
                this.textAmountLabel = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.AmountLabel");
                this.textStatusBilled = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.StatusBilled");
                this.textStatusNotBilled = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.StatusNotBilled");
                this.textDefaultInvite = ResourceLanguageManager.GetValue("frmCashierWaitingScreen.DefaultInviteText");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
