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
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MealRationDetail.MealRationDetail
{
    public partial class frmMealRationDetail : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            ////Khoi tao doi tuong resource
            Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MealRationDetail.Resources.Lang", typeof(HIS.Desktop.Plugins.MealRationDetail.MealRationDetail.frmMealRationDetail).Assembly);
            ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
            this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineStt.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnMedicineStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnMedicineMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnMedicineMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnMedicineServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineAmount.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnMedicineAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());          
            this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("frmMealRationDetail.repositoryItemPictureEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridControlPrescriptionColumnRemove.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridControlPrescriptionColumnRemove.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridControlPrescriptionExpMestCode.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridControlPrescriptionExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnPrescriptionVirPatientName.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnPrescriptionVirPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
         
            this.gridColumnPrescriptionDob.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnPrescriptionDob.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnPrescriptionGenderName.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnPrescriptionGenderName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bar1.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.bbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnApproval.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.btnApproval.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnApprovalCancel.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.btnExport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.cboPrint.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.cboPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.groupControlCommonInfomation.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.groupControlCommonInfomation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciCreator.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.lciCreator.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciCreateTime.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.lciCreateTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciLblReqName.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.lciLblReqName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.toggleSwitchTheoNhom.Properties.OffText = Inventec.Common.Resource.Get.Value("frmMealRationDetail.toggleSwitchTheoNhom.Properties.OffText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.toggleSwitchTheoNhom.Properties.OnText = Inventec.Common.Resource.Get.Value("frmMealRationDetail.toggleSwitchTheoNhom.Properties.OnText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciExpMestCode.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.lciExpMestCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciMedistock.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.lciMedistock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridControlPrescriptionColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridControlPrescriptionColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineStt.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnMedicineStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciReqDepartment.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.lciReqDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciLblReqTime.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.lciLblReqTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            //this.lblReqTime.Text = Inventec.Common.Resource.Get.Value("frmMealRationDetail.lblReqTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            this.gridColumnRationTimeName.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnRationTimeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnIntructionTime.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnIntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnPatientClassifyName.Caption = Inventec.Common.Resource.Get.Value("frmMealRationDetail.gridColumnPatientClassifyName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }
    }
}
