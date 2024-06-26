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

namespace HIS.Desktop.Plugins.AggrExpMestDetail.AggrExpMestDetail
{
    public partial class frmAggrExpMestDetail : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            ////Khoi tao doi tuong resource
            Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AggrExpMestDetail.Resources.Lang", typeof(HIS.Desktop.Plugins.AggrExpMestDetail.AggrExpMestDetail.frmAggrExpMestDetail).Assembly);

            ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
            this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineStt.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnPrescriptionServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnPrescriptionServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineAmount.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.repositoryItemPictureEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridControlPrescriptionColumnRemove.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlPrescriptionColumnRemove.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridControlPrescriptionExpMestCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlPrescriptionExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnPrescriptionVirPatientName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnPrescriptionVirPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnDetailTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnDetailTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnDetailPrice.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnDetailPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnPrescriptionDob.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnPrescriptionDob.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnPrescriptionGenderName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnPrescriptionGenderName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bar1.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.bbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnApproval.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.btnApproval.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnExport.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.btnExport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.cboPrint.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.cboPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.groupControlCommonInfomation.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.groupControlCommonInfomation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciCreator.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.lciCreator.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciCreateTime.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.lciCreateTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciLblReqName.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.lciLblReqName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciExpMestCode.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.lciExpMestCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciMedistock.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.lciMedistock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridControlPrescriptionColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlPrescriptionColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnMedicineStt.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciReqDepartment.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.lciReqDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }
    }
}
