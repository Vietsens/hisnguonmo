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
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MedicineList.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicineList.UCMedicineList__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMedicineList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCMedicineList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCMedicineList.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCMedicineList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControl1.Text = Inventec.Common.Resource.Get.Value("UCMedicineList.navBarControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupImpTime.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.navBarGroupImpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCMedicineList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCMedicineList.lciImpTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpTimeTo.Text = Inventec.Common.Resource.Get.Value("UCMedicineList.lciImpTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicineList.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtMedicineTypeCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicineList.txtMedicineTypeCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCMedicineList.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Stt.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Lock.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_Lock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_MedicinePaty.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_MedicinePaty.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_MedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_MedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_MedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_MedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Amount.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_ImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_ImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ExpriedDate.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_ExpriedDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_PackgeNumber.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_PackgeNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_BidNumOrder.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_BidNumOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_BidNumber.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_BidName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_BidName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_NationalName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_NationalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_SupplierName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_SupplierName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ManufacturerName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_ManufacturerName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_MedicineRegisterNumber.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_MedicineRegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Concentra.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_Concentra.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_ActiveIngrBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_ActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_MedicineUseFromName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_MedicineUseFromName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ImpTime.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_ImpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_CreateTime.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_CreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Creator.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_Creator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_ModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Modifier.Caption = Inventec.Common.Resource.Get.Value("UCMedicineList.Gc_Modifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
