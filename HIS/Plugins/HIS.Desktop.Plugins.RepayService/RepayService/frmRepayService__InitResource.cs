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
namespace HIS.Desktop.Plugins.RepayService.RepayService
{
    public partial class frmRepayService : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RepayService.Resources.Lang", typeof(HIS.Desktop.Plugins.RepayService.RepayService.frmRepayService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmRepayService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmRepayService.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmRepayService.cboPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("frmRepayService.cboAccountBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmRepayService.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmRepayService.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ddbPrint.Text = Inventec.Common.Resource.Get.Value("frmRepayService.ddbPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeSereServ.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmRepayService.treeSereServ.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnServiceName.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumnServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnAmount.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumnAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnVirTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumnVirTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnVirTotalHeinPrice.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumnVirTotalHeinPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnVirTotalPatientPrice.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumnVirTotalPatientPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnServiceCode.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumnServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumnIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmRepayService.treeListColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.lblAccountBook.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lblAccountBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPayForm.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lblPayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblAmount.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lblAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lblTotalFromNumberOder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblDescription.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lblDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lblNumberOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblTransactionCode.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lblTransactionCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblCreateTime.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lblCreateTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmRepayService.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.bbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSaveAndPrint.Caption = Inventec.Common.Resource.Get.Value("frmRepayService.bbtnSaveAndPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRepayReasonCode.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lciRepayReasonCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //
                this.lciTransactionTime.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lciTransactionTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTransferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmRepayService.lciTransferAmount.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTransferAmount.Text = Inventec.Common.Resource.Get.Value("frmRepayService.lciTransferAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoClose.Properties.Caption = Inventec.Common.Resource.Get.Value("frmDepositService.chkAutoClose.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoClose.ToolTip = Inventec.Common.Resource.Get.Value("frmDepositService.chkAutoClose.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !string.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                else
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmRepayService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }

                this.chkAutoClose.Properties.Caption =   Inventec.Common.Resource.Get.Value("frmRepayService.chkAutoClose.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoClose.ToolTip = Inventec.Common.Resource.Get.Value("frmRepayService.chkAutoClose.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
