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
namespace HIS.Desktop.Plugins.HisPrepareApprove
{
    public partial class frmHisPrepareApprove : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisPrepareApprove.Resources.Lang", typeof(HIS.Desktop.Plugins.HisPrepareApprove.frmHisPrepareApprove).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnServiceUnitName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnServiceUnitName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnReqAmount.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnReqAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnReqAmount.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnReqAmount.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnApprovalAmount.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnApprovalAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnApprovalAmount.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnApprovalAmount.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnReqUserName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnReqUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnApprovalLoginname.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnApprovalLoginname.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnNationalName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnNationalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnManufacturerName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnManufacturerName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnConcentra.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridViewColumnConcentra.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonI__Refesh.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.barButtonI__Refesh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnServiceUnitName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnServiceUnitName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnReqAMount.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnReqAMount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnReqAMount.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnReqAMount.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnApprovalAmount.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnApprovalAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnApprovalAmount.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnApprovalAmount.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnNationalName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnNationalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnManufacturerName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnManufacturerName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridProcessColumnConcentra.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.gridProcessColumnConcentra.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciUseFrom.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.lciUseFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciUseTo.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.lciUseTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciApprovalLogginName.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareApprove.lciApprovalLogginName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                
                this.Text = this.currentModule != null ? this.currentModule.text : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
