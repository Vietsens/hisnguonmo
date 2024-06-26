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
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Resources;
namespace HIS.Desktop.Plugins.DepositService.DepositService
{
    public partial class frmDepositService : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DepositService.Resources.Lang", typeof(HIS.Desktop.Plugins.DepositService.DepositService.frmDepositService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmDepositService.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmDepositService.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmDepositService.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmDepositService.bbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSaveAndPrint.Caption = Inventec.Common.Resource.Get.Value("frmDepositService.bbtnSaveAndPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmDepositService.cboPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("frmDepositService.cboAccountBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmDepositService.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmDepositService.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ddbPrint.Text = Inventec.Common.Resource.Get.Value("frmDepositService.ddbPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblAmount.Text = Inventec.Common.Resource.Get.Value("frmDepositService.lblAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("frmDepositService.lblTotalFromNumberOder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblDescription.Text = Inventec.Common.Resource.Get.Value("frmDepositService.lblDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblTransactionCode.Text = Inventec.Common.Resource.Get.Value("frmDepositService.lblTransactionCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblAccountBook.Text = Inventec.Common.Resource.Get.Value("frmDepositService.lblAccountBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lblPayForm.Text = Inventec.Common.Resource.Get.Value("frmDepositService.lblPayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTransactionTime.Text = Inventec.Common.Resource.Get.Value("frmDepositService.lciTransactionTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TEXT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.layoutControlItem14.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem14.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem13.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem11.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem12.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem7.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoClose.Properties.Caption = Inventec.Common.Resource.Get.Value("frmDepositService.chkAutoClose.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoClose.ToolTip = Inventec.Common.Resource.Get.Value("frmDepositService.chkAutoClose.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem11Option.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem11Option.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !string.IsNullOrEmpty(moduleData.text))
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmDepositService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmDepositService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }


                // UC tree sereServ

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
