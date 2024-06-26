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
namespace HIS.Desktop.Plugins.ExpMestDetailBCS.ExpMestViewDetail
{
    public partial class frmExpMestViewDetail : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExpMestDetailBCS.Resources.Lang", typeof(HIS.Desktop.Plugins.ExpMestDetailBCS.ExpMestViewDetail.frmExpMestViewDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.bbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupControlCommonInformation.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.groupControlCommonInformation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lbiVirPatientName.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lbiVirPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpiredDate.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciExpiredDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGenderName.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciGenderName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciVirAddress.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciVirAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciUseTimeFromTo.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciUseTimeFromTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciAdvise.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciAdvise.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciIcdName.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciIcdName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciSumPrice.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciSumPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciIcdCode.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciIcdCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciIcdText.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRequestRoom.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciRequestRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRequestDepartment.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciRequestDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnApproval.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.btnApproval.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExport.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.btnExport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPageRequestMedicine.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.tabPageRequestMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineSTT.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.repositoryItemPictureEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineAmount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineSumInStock.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineSumInStock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicinePrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicinePrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineExpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineExpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineDiscount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineDiscount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMedicineTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMedicineTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodBloodCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodBloodCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPageRequestMaterial.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.tabPageRequestMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialMateStt.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialMateStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestBloodBloodCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestBloodBloodCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialMaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialMaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialMaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialMaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit2.NullText = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.repositoryItemPictureEdit2.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialAmount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialTotalAmount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialTotalAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialDiscount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialDiscount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialExpVat.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialExpVat.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestMaterialTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestMaterialTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TabPageApprovalMedicine.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.TabPageApprovalMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineStt.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit4.NullText = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.repositoryItemPictureEdit4.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineAmount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineSumInStock.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineSumInStock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicinePrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicinePrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineExpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineExpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineDiscount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineDiscount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMedicineTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMedicineTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TabPageApprovalMaterial.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.TabPageApprovalMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialStt.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialMaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialMaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialMaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialMaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit5.NullText = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.repositoryItemPictureEdit5.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialAmount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialTotalAmount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialTotalAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialDiscount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialDiscount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialExpVat.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialExpVat.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalMaterialTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApprovalMaterialTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TabPageRequestBlood.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.TabPageRequestBlood.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestBloodSTT.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestBloodSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestBloodBloodTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestBloodBloodTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestBloodBloodTypeName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestBloodBloodTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestBloodBloodAboCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestBloodBloodAboCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestBloodBloodRhCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestBloodBloodRhCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestBloodVolumn.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestBloodVolumn.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestBloodServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestBloodServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestBloodAmount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestBloodAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn34.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn35.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumn35.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn36.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn37.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumn37.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn38.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumn38.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn39.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumn39.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRequestBloodPatientTypeName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnRequestBloodPatientTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit3.NullText = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.repositoryItemPictureEdit3.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TabPageExportBlood.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.TabPageExportBlood.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodStt.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodBloodTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodBloodTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodBloodTypeName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodBloodTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodBloodAboCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodBloodAboCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodBloodRhCode.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodBloodRhCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodAmount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodExpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodExpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodDiscount.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodDiscount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproveBloodTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.gridColumnApproveBloodTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpMestReasonName.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciExpMestReasonName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciApprovalUserName.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciApprovalUserName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpMestSttName.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.lciExpMestSttName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPrint.Text = Inventec.Common.Resource.Get.Value("frmExpMestViewDetail.cboPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
