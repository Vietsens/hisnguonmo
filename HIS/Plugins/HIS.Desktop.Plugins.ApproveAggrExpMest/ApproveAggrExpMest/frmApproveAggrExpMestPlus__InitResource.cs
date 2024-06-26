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

namespace HIS.Desktop.Plugins.ApproveAggrExpMest.ApproveAggrExpMest
{
    public partial class frmApproveAggrExpMest : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ApproveAggrExpMest.Resources.Lang", typeof(HIS.Desktop.Plugins.ApproveAggrExpMest.ApproveAggrExpMest.frmApproveAggrExpMest).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPrint.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.cboPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPagePrescription.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.xtraTabPagePrescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageRequestMediMateInPrescription.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.xtraTabPageRequestMediMateInPrescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlMedcineInPrescriptionColumnStt.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlMedcineInPrescriptionColumnStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlMedcineInPrescriptionMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlMedcineInPrescriptionMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlMedcineInPrescriptionMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlMedcineInPrescriptionMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlMedcineInPrescriptionServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlMedcineInPrescriptionServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlMedcineInPrescriptionAmount.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlMedcineInPrescriptionAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlMedcineInPrescriptionPackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlMedcineInPrescriptionPackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlMedcineInPrescriptionExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlMedcineInPrescriptionExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlMedcineInPrescriptionMedicineRegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlMedcineInPrescriptionMedicineRegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlMedcineInPrescriptionActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlMedcineInPrescriptionActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlMedcineInPrescriptionActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlMedcineInPrescriptionActiveIngrBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit3.NullText = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.repositoryItemPictureEdit3.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageApprovalMediMateInPrescription.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.xtraTabPageApprovalMediMateInPrescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedcineInPrescriptionSTT.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMedcineInPrescriptionSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedcineInPrescriptionMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMedcineInPrescriptionMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedcineInPrescriptionMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMedcineInPrescriptionMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedcineInPrescriptionServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMedcineInPrescriptionServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedcineInPrescriptionAmount.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMedcineInPrescriptionAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedcineInPrescriptionPackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMedcineInPrescriptionPackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedcineInPrescriptionExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMedcineInPrescriptionExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedcineInPrescriptionMedicineRegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMedcineInPrescriptionMedicineRegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedcineInPrescriptionActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMedcineInPrescriptionActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedcineInPrescriptionActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMedcineInPrescriptionActiveIngrBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit4.NullText = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.repositoryItemPictureEdit4.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlPrescriptionColumnRemove.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlPrescriptionColumnRemove.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlPrescriptionExpMestCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridControlPrescriptionExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPrescriptionVirPatientName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnPrescriptionVirPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPrescriptionDob.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnPrescriptionDob.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPrescriptionGenderName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnPrescriptionGenderName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.bbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageRequestMedicine.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.xtraTabPageRequestMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMedicineStt.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMedicineMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMedicineMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMedicineServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMedicineAmount.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMedicineIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMedicineIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.repositoryItemPictureEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageApprovalMedicine.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.xtraTabPageApprovalMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprvovalMedicineSTT.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprvovalMedicineSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprvovalMedicineMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprvovalMedicineMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprvovalMedicineMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprvovalMedicineMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprvovalMedicineServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprvovalMedicineServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprvovalMedicineAmount.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprvovalMedicineAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprvovalMedicineIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprvovalMedicineIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit5.NullText = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.repositoryItemPictureEdit5.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprvovalMedicineBidNumber.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprvovalMedicineBidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprvovalMedicineExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprvovalMedicineExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprvovalMedicineRegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprvovalMedicineRegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.columnApprvovalMedicineActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.columnApprvovalMedicineActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprvovalMedicineActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprvovalMedicineActiveIngrBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageRequestMaterial.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.xtraTabPageRequestMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMaterialStt.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMaterialStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMaterialMaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMaterialMaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMaterialMaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMaterialMaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMaterialServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMaterialServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMaterialAmount.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMaterialAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMaterialIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMaterialIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit2.NullText = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.repositoryItemPictureEdit2.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMaterialBidNumber.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMaterialBidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMaterialExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMaterialExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMaterialRegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnMaterialRegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageApprovalMaterial.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.xtraTabPageApprovalMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialSTT.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMaterialSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialMaterialType.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMaterialMaterialType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialMaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMaterialMaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMaterialServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialAmount.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMaterialAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMaterialIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit6.NullText = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.repositoryItemPictureEdit6.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialBidNumber.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMaterialBidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMaterialExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialRegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.gridColumnApprovalMaterialRegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupControlCommonInfomation.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.groupControlCommonInfomation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreator.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.lciCreator.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTime.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.lciCreateTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciUserTime.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.lciUserTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !string.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                else
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmApproveAggrExpMest.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
