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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using HIS.Desktop.Plugins.ImpMestCreate.Base;
using HIS.Desktop.Plugins.ImpMestCreate.Config;
using HIS.Desktop.Utility;
using HIS.UC.MaterialType;
using HIS.UC.MaterialType.ADO;
using HIS.UC.MedicineType;
using HIS.UC.MedicineType.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    public partial class UCImpMestCreate : UserControlBase
    {
        MedicineTypeInitADO adoMe;
        MaterialTypeInitADO adoMa;
        private void InitMedicineTypeTree()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.medicineProcessor = new MedicineTypeProcessor();
                adoMe = new MedicineTypeInitADO();

                adoMe.IsShowSearchPanel = true;
                adoMe.MedicineTypeClick = medicineTypeTree_Click;
                adoMe.MedicineTypeRowEnter = medicineTypeTree_RowEnter;
                adoMe.MedicineTypeColumns = new List<MedicineTypeColumn>();
                adoMe.IsAutoWidth = false;
                adoMe.IsShowBid = true;
                adoMe.IsShowContract = true;
                adoMe.IsHightLightFilter = true;
                adoMe.ParentFieldName = "ParentField";
                adoMe.KeyFieldName = "KeyField";
                //ado.listBids = listBids;
                adoMe.cboBid_EditValueChanged = cboBid_EditValueChangedUCMedicine;
                adoMe.cboContract_EditValueChanged = cboContract_EditValueChangedUCMedicine;
                adoMe.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TXT_KEYWORD__NULL_VALUE", Base.ResourceLangManager.LanguageUCImpMestCreate, culture);

                //MedicineTypeCode
                MedicineTypeColumn colMedicineTypeCode = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_MEDICINE_TYPE_CODE", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "MEDICINE_TYPE_CODE", 70, false);
                colMedicineTypeCode.VisibleIndex = 0;
                adoMe.MedicineTypeColumns.Add(colMedicineTypeCode);

                //MedicineTypeName
                MedicineTypeColumn colMedicineTypeName = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_MEDICINE_TYPE_NAME", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "MEDICINE_TYPE_NAME", 250, false);
                colMedicineTypeName.VisibleIndex = 1;
                adoMe.MedicineTypeColumns.Add(colMedicineTypeName);

                //ServiceUnitName
                MedicineTypeColumn colServiceUnitName = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "SERVICE_UNIT_NAME", 50, false);
                colServiceUnitName.VisibleIndex = 2;
                adoMe.MedicineTypeColumns.Add(colServiceUnitName);

                //Hàm lượng
                MedicineTypeColumn colHamLuong = new MedicineTypeColumn("Hàm lượng", "CONCENTRA", 100, false);
                colHamLuong.VisibleIndex = 3;
                adoMe.MedicineTypeColumns.Add(colHamLuong);

                //Quy cách đóng gói
                MedicineTypeColumn colQuyCachDongGoi = new MedicineTypeColumn("Quy cách đóng gói", "PACKING_TYPE_NAME", 100, false);
                colQuyCachDongGoi.VisibleIndex = 4;
                adoMe.MedicineTypeColumns.Add(colQuyCachDongGoi);

                //Mã hoạt chất
                MedicineTypeColumn colMaHoatChat = new MedicineTypeColumn("Mã hoạt chất", "ACTIVE_INGR_BHYT_CODE", 50, false);
                colMaHoatChat.VisibleIndex = 5;
                adoMe.MedicineTypeColumns.Add(colMaHoatChat);

                //Tên hoạt chất
                MedicineTypeColumn colTenHoatChat = new MedicineTypeColumn("Tên hoạt chất", "ACTIVE_INGR_BHYT_NAME", 150, false);
                colTenHoatChat.VisibleIndex = 6;
                adoMe.MedicineTypeColumns.Add(colTenHoatChat);

                //NationalName
                MedicineTypeColumn colNationalName = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_NATIONAL_NAME", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "NATIONAL_NAME", 100, false);
                colNationalName.VisibleIndex = 7;
                adoMe.MedicineTypeColumns.Add(colNationalName);

                //ManufactureName
                MedicineTypeColumn colManufactureName = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_MANUFACTURE_NAME", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "MANUFACTURER_NAME", 120, false);
                colManufactureName.VisibleIndex = 8;
                adoMe.MedicineTypeColumns.Add(colManufactureName);

                //RegisterNumber
                MedicineTypeColumn colRegisterNumber = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_REGISTER_NUMBER", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "REGISTER_NUMBER", 70, false);
                colRegisterNumber.VisibleIndex = 9;
                adoMe.MedicineTypeColumns.Add(colRegisterNumber);
                // Mã đường dùng
                MedicineTypeColumn colMaduongdung = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_MEDICINE_LINE_NAME", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "MEDICINE_USE_FORM_NAME", 110, false);
                colMaduongdung.VisibleIndex = 10;
                adoMe.MedicineTypeColumns.Add(colMaduongdung);

                // Số lượng (thầu)
                MedicineTypeColumn colSoluong = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_AMOUNT_IN_BID", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "AMOUNT_IN_BID", 90, false);
                colSoluong.VisibleIndex = 12;
                colSoluong.Format = new DevExpress.Utils.FormatInfo();
                colSoluong.Format.FormatString = "#,##0.";
                colSoluong.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMe.MedicineTypeColumns.Add(colSoluong);

                // Giá nhập (thầu)
                MedicineTypeColumn colGianhap = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_IMP_PRICE_IN_BID", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "IMP_PRICE_IN_BID", 100, false);
                colGianhap.VisibleIndex = 13;
                colGianhap.Format = new DevExpress.Utils.FormatInfo();
                colGianhap.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                colGianhap.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMe.MedicineTypeColumns.Add(colGianhap);

                // VAT (thầu)
                MedicineTypeColumn colVAT = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_VAT_IN_BID", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "IMP_VAT_RATIO_IN_BID", 70, false);
                colVAT.VisibleIndex = 14;
                colVAT.Format = new DevExpress.Utils.FormatInfo();
                colVAT.Format.FormatString = "#,##0.";
                colVAT.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMe.MedicineTypeColumns.Add(colVAT);

                // Giảm giá từ
                MedicineTypeColumn colDiscountFrom = new MedicineTypeColumn("Giảm giá từ", "DISCOUNT_FROM_DATE_STR", 100, false);
                colDiscountFrom.VisibleIndex = 15;
                adoMe.MedicineTypeColumns.Add(colDiscountFrom);

                // Giảm giá từ
                MedicineTypeColumn colDiscountTo = new MedicineTypeColumn("Giảm giá đến", "DISCOUNT_TO_DATE_STR", 100, false);
                colDiscountTo.VisibleIndex = 16;
                adoMe.MedicineTypeColumns.Add(colDiscountTo);

                // Giá gốc
                MedicineTypeColumn colPriceDf = new MedicineTypeColumn("Giá gốc", "IMP_PRICE_IN_CONTRACT", 100, false);
                colPriceDf.VisibleIndex = 17;
                colPriceDf.Format = new DevExpress.Utils.FormatInfo();
                colPriceDf.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                colPriceDf.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMe.MedicineTypeColumns.Add(colPriceDf);

                // Số lượng (hợp đồng)
                MedicineTypeColumn colSoluongC = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_AMOUNT_IN_CONTRACT", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "AMOUNT_IN_CONTRACT", 90, false);
                colSoluongC.VisibleIndex = -1;
                colSoluongC.Format = new DevExpress.Utils.FormatInfo();
                colSoluongC.Format.FormatString = "#,##0.";
                colSoluongC.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMe.MedicineTypeColumns.Add(colSoluongC);

                // Giá nhập (hợp đồng)
                MedicineTypeColumn colGianhapC = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_IMP_PRICE_IN_CONTRACT", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "IMP_PRICE_IN_CONTRACT", 100, false);
                colGianhapC.VisibleIndex = -1;
                colGianhapC.Format = new DevExpress.Utils.FormatInfo();
                colGianhapC.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                colGianhapC.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMe.MedicineTypeColumns.Add(colGianhapC);

                // VAT (hợp đồng)
                MedicineTypeColumn colVATC = new MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_VAT_IN_CONTRACT", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "IMP_VAT_RATIO_IN_CONTRACT", 70, false);
                colVATC.VisibleIndex = -1;
                colVATC.Format = new DevExpress.Utils.FormatInfo();
                colVATC.Format.FormatString = "#,##0.";
                colVATC.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMe.MedicineTypeColumns.Add(colVATC);

                // Nhóm thầu
                MedicineTypeColumn colNhomThauA = new MedicineTypeColumn("Nhóm thầu", "TDL_BID_GROUP_CODE", 70, false);
                colNhomThauA.VisibleIndex = -1;
                adoMe.MedicineTypeColumns.Add(colNhomThauA);

                // Thông tin thầu
                MedicineTypeColumn colTTThau = new MedicineTypeColumn("Thông tin thầu", "TT_THAU", 130, false);
                colTTThau.VisibleIndex = -1;
                adoMe.MedicineTypeColumns.Add(colTTThau);

                this.ucMedicineTypeTree = (UserControl)medicineProcessor.Run(adoMe);
                if (this.ucMedicineTypeTree != null)
                {
                    this.xtraTabPageMedicine.Controls.Add(this.ucMedicineTypeTree);
                    this.ucMedicineTypeTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMaterialTypeTree()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                this.materialProcessor = new MaterialTypeTreeProcessor();
                adoMa = new MaterialTypeInitADO();

                adoMa.IsShowSearchPanel = true;
                adoMa.MaterialTypeClick = materialTypeTree_Click;
                adoMa.MaterialTypeRowEnter = materialTypeTree_RowEnter;
                adoMa.MaterialTypeColumns = new List<MaterialTypeColumn>();
                adoMa.IsAutoWidth = false;
                adoMa.IsShowBid = true;
                adoMa.IsShowContract = true;
                adoMa.IsHightLightFilter = true;
                adoMa.ParentFieldName = "ParentField";
                adoMa.KeyFieldName = "KeyField";
                //adoMa.listBids = listBids;
                adoMa.cboBid_EditValueChanged = cboBid_EditValueChangedUCMaterial;
                adoMa.cboContract_EditValueChanged = cboContract_EditValueChangedUCMaterial;
                adoMa.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TXT_KEYWORD__NULL_VALUE", Base.ResourceLangManager.LanguageUCImpMestCreate, culture);

                //MaterialTypeCode
                MaterialTypeColumn colMaterialTypeCode = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MATERIAL__COLUMN_MATERIAL_TYPE_CODE", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "MATERIAL_TYPE_CODE", 70, false);
                colMaterialTypeCode.VisibleIndex = 0;
                adoMa.MaterialTypeColumns.Add(colMaterialTypeCode);

                //MedicineTypeName
                MaterialTypeColumn colMaterialTypeName = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MATERIAL__COLUMN_MATERIAL_TYPE_NAME", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "MATERIAL_TYPE_NAME", 250, false);
                colMaterialTypeName.VisibleIndex = 1;
                adoMa.MaterialTypeColumns.Add(colMaterialTypeName);

                //ServiceUnitName
                MaterialTypeColumn colServiceUnitName = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MATERIAL__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "SERVICE_UNIT_NAME", 50, false);
                colServiceUnitName.VisibleIndex = 2;
                adoMa.MaterialTypeColumns.Add(colServiceUnitName);

                //Hàm lượng
                MaterialTypeColumn colHamLuong = new MaterialTypeColumn("Hàm lượng", "CONCENTRA", 100, false);
                colHamLuong.VisibleIndex = 3;
                adoMa.MaterialTypeColumns.Add(colHamLuong);

                //Quy cách đóng gói
                MaterialTypeColumn colQuyCachDongGoi = new MaterialTypeColumn("Quy cách đóng gói", "PACKING_TYPE_NAME", 100, false);
                colQuyCachDongGoi.VisibleIndex = 4;
                adoMa.MaterialTypeColumns.Add(colQuyCachDongGoi);

                //NationalName
                MaterialTypeColumn colNationalName = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MATERIAL__COLUMN_NATIONAL_NAME", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "NATIONAL_NAME", 120, false);
                colServiceUnitName.VisibleIndex = 5;
                adoMa.MaterialTypeColumns.Add(colNationalName);

                //ManufactureName
                MaterialTypeColumn colManufactureName = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MATERIAL__COLUMN_MANUFACTURE_NAME", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "MANUFACTURER_NAME", 150, false);
                colManufactureName.VisibleIndex = 6;
                adoMa.MaterialTypeColumns.Add(colManufactureName);

                // Số lượng (thầu)
                MaterialTypeColumn colSoluong = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MATERIAL__COLUMN_AMOUNT_IN_BID", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "AMOUNT_IN_BID", 90, false);
                colSoluong.VisibleIndex = 8;
                colSoluong.Format = new DevExpress.Utils.FormatInfo();
                colSoluong.Format.FormatString = "#,##0.";
                colSoluong.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMa.MaterialTypeColumns.Add(colSoluong);

                // Giá nhập (thầu)
                MaterialTypeColumn colGianhap = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MATERIAL__COLUMN_IMP_PRICE_IN_BID", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "IMP_PRICE_IN_BID", 100, false);
                colGianhap.VisibleIndex = 9;
                colGianhap.Format = new DevExpress.Utils.FormatInfo();
                colGianhap.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                colGianhap.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMa.MaterialTypeColumns.Add(colGianhap);

                // VAT (thầu)
                MaterialTypeColumn colVAT = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MATERIAL__COLUMN_VAT_IN_BID", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "IMP_VAT_RATIO_IN_BID", 70, false);
                colVAT.VisibleIndex = 10;
                colVAT.Format = new DevExpress.Utils.FormatInfo();
                colVAT.Format.FormatString = "#,##0.";
                colVAT.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMa.MaterialTypeColumns.Add(colVAT);

                // Giảm giá từ
                MaterialTypeColumn colDiscountFrom = new MaterialTypeColumn("Giảm giá từ", "DISCOUNT_FROM_DATE_STR", 100, false);
                colDiscountFrom.VisibleIndex = 11;
                adoMa.MaterialTypeColumns.Add(colDiscountFrom);

                // Giảm giá đến
                MaterialTypeColumn colDiscountTo = new MaterialTypeColumn("Giảm giá đến", "DISCOUNT_TO_DATE_STR", 100, false);
                colDiscountTo.VisibleIndex = 12;
                adoMa.MaterialTypeColumns.Add(colDiscountTo);

                // Giá gốc
                MaterialTypeColumn colPriceDf = new MaterialTypeColumn("Giá gốc", "IMP_PRICE_IN_CONTRACT", 100, false);
                colPriceDf.VisibleIndex = 13;
                colPriceDf.Format = new DevExpress.Utils.FormatInfo();
                colPriceDf.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                colPriceDf.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMa.MaterialTypeColumns.Add(colPriceDf);


                // Số lượng (hợp đồng)
                MaterialTypeColumn colSoluongC = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_AMOUNT_IN_CONTRACT", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "AMOUNT_IN_CONTRACT", 90, false);
                colSoluongC.VisibleIndex = -1;
                colSoluongC.Format = new DevExpress.Utils.FormatInfo();
                colSoluongC.Format.FormatString = "#,##0.";
                colSoluongC.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMa.MaterialTypeColumns.Add(colSoluongC);

                // Giá nhập (hợp đồng)
                MaterialTypeColumn colGianhapC = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_IMP_PRICE_IN_CONTRACT", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "IMP_PRICE_IN_CONTRACT", 100, false);
                colGianhapC.VisibleIndex = -1;
                colGianhapC.Format = new DevExpress.Utils.FormatInfo();
                colGianhapC.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                colGianhapC.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMa.MaterialTypeColumns.Add(colGianhapC);

                // VAT (hợp đồng)
                MaterialTypeColumn colVATC = new MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_VAT_IN_CONTRACT", Base.ResourceLangManager.LanguageUCImpMestCreate, culture), "IMP_VAT_RATIO_IN_CONTRACT", 70, false);
                colVATC.VisibleIndex = -1;
                colVATC.Format = new DevExpress.Utils.FormatInfo();
                colVATC.Format.FormatString = "#,##0.";
                colVATC.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                adoMa.MaterialTypeColumns.Add(colVATC);


                // Nhóm thầu
                MaterialTypeColumn colNhomThau = new MaterialTypeColumn("Nhóm thầu", "TDL_BID_GROUP_CODE", 70, false);
                colNhomThau.VisibleIndex = -1;
                adoMa.MaterialTypeColumns.Add(colNhomThau);

                // Thông tin thầu
                MaterialTypeColumn colTTThau = new MaterialTypeColumn("Thông tin thầu", "TT_THAU", 130, false);
                colTTThau.VisibleIndex = -1;
                adoMa.MaterialTypeColumns.Add(colTTThau);

                this.ucMaterialTypeTree = (UserControl)materialProcessor.Run(adoMa);
                if (this.ucMaterialTypeTree != null)
                {
                    this.xtraTabPageMaterial.Controls.Add(this.ucMaterialTypeTree);
                    this.ucMaterialTypeTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private CancellationTokenSource cancellationTokenSource;
        private void medicineTypeTree_Click(UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("medicineTypeTree_Click()-Start");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("data", data));
                Thread threadLoadMedicine = null;
                if (this.configWarningDiff == null)
                {
                    configWarningDiff = new HIS_CONFIG();
                    configWarningDiff.VALUE = HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.IsWarningDifferenceLastImp");

                }
                if (configWarningDiff != null && configWarningDiff.VALUE == "1")
                {
                    threadLoadMedicine = new Thread(() => GetMedicine(data.ID));

                }
                if (btnCancel1.Enabled == true)
                {
                    SetEnableButton(false);
                    ResetValueControlDetail();
                    SetFocuTreeMediMate();
                    SetEnableButton(false);
                }
                dtExpiredDate.EditValue = null;
                txtExpiredDate.Text = "";
                txtPackageNumber.Text = "";
                txtBid.Text = "";
                txtHeinServiceBidMateType.Text = "";
                dxValidationProvider2.RemoveControlError(txtHeinServiceBidMateType);
                dxValidationProvider2.RemoveControlError(spinImpAmount);
                dxValidationProvider2.RemoveControlError(txtPackageNumber);
                dxValidationProvider2.RemoveControlError(txtExpiredDate);

                if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC
                    && medistock.IS_ALLOW_IMP_SUPPLIER == 0)
                {
                    return;
                }

                WaitingManager.Show();
                this.currrentServiceAdo = null;

                if (data != null)
                {
                    if (threadLoadMedicine != null) threadLoadMedicine.Start();
                    if (data.IS_SALE_EQUAL_IMP_PRICE == 1)
                    {
                        chkImprice.Checked = true;
                        chkImprice.Enabled = false;
                    }
                    else
                    {
                        chkImprice.Checked = false;
                        chkImprice.Enabled = true;
                    }

                    this.currrentServiceAdo = new ADO.VHisServiceADO((V_HIS_MEDICINE_TYPE)data);
                    this.currrentServiceAdo.ADJUST_AMOUNT = data.ADJUST_AMOUNT;
                    ChangeColorMedicine(this.currrentServiceAdo);

                    if (medicineProcessor.GetBid(ucMedicineTypeTree) != null && dicBidMedicine.Count > 0 && dicBidMedicine.ContainsKey(Base.StaticMethod.GetTypeKey(data.ID, data.BidGroupCode)))
                    {
                        var bidMedi = dicBidMedicine[Base.StaticMethod.GetTypeKey(data.ID, data.BidGroupCode)];
                        if (bidMedi != null)
                        {
                            this.currrentServiceAdo.TDL_BID_GROUP_CODE = bidMedi.BID_GROUP_CODE;
                            this.currrentServiceAdo.TDL_BID_NUM_ORDER = bidMedi.BID_NUM_ORDER;
                            this.currrentServiceAdo.TDL_BID_PACKAGE_CODE = bidMedi.BID_PACKAGE_CODE;
                            this.currrentServiceAdo.TDL_BID_YEAR = bidMedi.BID_YEAR;
                            this.currrentServiceAdo.TDL_BID_NUMBER = bidMedi.BID_NUMBER;
                            this.currrentServiceAdo.EXPIRED_DATE = bidMedi.EXPIRED_DATE;
                            this.currrentServiceAdo.monthLifespan = bidMedi.MONTH_LIFESPAN;
                        }
                    }

                    DevExpress.XtraEditors.DXErrorProvider.ValidationRule validationRule = new DevExpress.XtraEditors.DXErrorProvider.ValidationRule();
                    dxValidationProvider2.SetValidationRule(dtExpiredDate, validationRule);
                    dxValidationProvider2.SetValidationRule(txtExpiredDate, validationRule);
                    layoutExpiredDate.AppearanceItemCaption.ForeColor = Color.Black;
                    SoloValidBidControl(txtPackageNumber, layoutPackageNumber, false);

                    //#35237 :ko bắt buộc nhập số lô, HSD
                    if (!this.currrentServiceAdo.IsAllowMissingPkgInfo)
                    {
                        if (this.currrentServiceAdo.IsRequireHsd)
                        {
                            ValidControlExpiredDate1(dtExpiredDate);
                            ValidControlExpiredDate1(txtExpiredDate);
                            layoutExpiredDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                        }

                        if (this.cboImpMestType.EditValue != null
                            && Inventec.Common.TypeConvert.Parse.ToInt64(this.cboImpMestType.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC
                            && (this.currrentServiceAdo.MEDICINE_LINE_ID == null
                            || (this.currrentServiceAdo.MEDICINE_LINE_ID > 0
                            && this.currrentServiceAdo.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__TTD))
                            )
                        {
                            ValidControlExpiredDate1(dtExpiredDate);
                            ValidControlExpiredDate1(txtExpiredDate);
                            SoloValidBidControl(txtPackageNumber, layoutPackageNumber, true);
                            layoutExpiredDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                        }
                        else
                        {
                            SoloValidBidControl(txtPackageNumber, layoutPackageNumber, false);
                        }
                    }
                }

                VisibleLayoutTemperature();
                this.SetPrimary();
                SetValueByServiceAdo();
                LoadServicePatyByAdo();
                SetValueByContractMety(data);

                if (IsHasValueChooice && IsNCC)
                {
                    SetEnableControl(this.IsAllowedToEnableMedicinesInformation);
                }
                TimerFocusImpAmount.Start();
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("medicineTypeTree_Click()-Ended");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueByContractMety(HIS.UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                if (this.currentContract != null)
                {
                    this.spinEditGiaTrongThau.Enabled = true;
                    this.spinImpPrice.Enabled = true;
                    //this.cboDosageForm.Enabled = true;
                    this.cboNationals.Enabled = true;     
                    this.cboHangSX.Enabled = true;
                    this.txtSoDangKy.Enabled = true;
                    this.SpMaxReuseCount.Enabled = false;
                    this.spinImpPriceVAT.Enabled = false;
                    if (dicContractMety.Count > 0 && data != null)
                    {
                        var listvalue = dicContractMety.Select(s => s.Value).ToList();

                        this.MedicalContractMety = listvalue.FirstOrDefault(o => o.ID == data.MEDI_CONTRACT_METY_ID);
                        if (this.MedicalContractMety != null)
                        {
                            Inventec.Common.Logging.LogSystem.Info("MedicalContractMety: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => MedicalContractMety), MedicalContractMety));

                            if (this.MedicalContractMety.IMP_VAT_RATIO != null && this.MedicalContractMety.IMP_VAT_RATIO > 0)
                            {
                                spinImpVatRatio.Value = this.MedicalContractMety.IMP_VAT_RATIO.Value * 100;
                                spinEditThueXuat.Value = this.MedicalContractMety.IMP_VAT_RATIO.Value * 100;
                                spinImpVatRatio.Enabled = false;
                            }
                            else
                            {
                                spinEditThueXuat.Value = 0;
                                spinImpVatRatio.Value = 0;
                                spinImpVatRatio.Enabled = true;
                            }

                            if (this.MedicalContractMety.EXPIRED_DATE != null && this.MedicalContractMety.EXPIRED_DATE > 0)
                            {
                                this.txtExpiredDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.MedicalContractMety.EXPIRED_DATE ?? 0);
                                this.dtExpiredDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.MedicalContractMety.EXPIRED_DATE ?? 0) ?? DateTime.Now;
                            }

                            if (this.MedicalContractMety.CONTRACT_PRICE.HasValue)
                            {
                                spinEditGiaTrongThau.Value = this.MedicalContractMety.CONTRACT_PRICE.Value;

                                spinImpPrice.Value = this.MedicalContractMety.CONTRACT_PRICE.Value / (1 + (this.MedicalContractMety.IMP_VAT_RATIO.HasValue ? this.MedicalContractMety.IMP_VAT_RATIO.Value : 0));
                                spinImpPrice1.Value = this.MedicalContractMety.CONTRACT_PRICE.Value / (1 + (this.MedicalContractMety.IMP_VAT_RATIO.HasValue ? this.MedicalContractMety.IMP_VAT_RATIO.Value : 0));
                            }
                            else
                            {
                                spinEditGiaTrongThau.Value = 0;
                                spinImpPrice.Value = 0;
                                spinImpPrice1.Value = 0;
                            }

                            //cập nhật chính sách giá tại sự kiện spinImpPriceVAT_EditValueChanged không cần cập nhật tay
                            //LoadServicePatyByContractPrice(spinEditGiaTrongThau.Value, spinImpVatRatio.Value);

                            this.currrentServiceAdo.CONTRACT_PRICE = this.MedicalContractMety.CONTRACT_PRICE;
                            this.currrentServiceAdo.MEDICAL_CONTRACT_ID = this.MedicalContractMety.MEDICAL_CONTRACT_ID;
                            this.currrentServiceAdo.DISCOUNT_FROM_DATE = this.MedicalContractMety.DISCOUNT_FROM_DATE;
                            this.currrentServiceAdo.DISCOUNT_TO_DATE = this.MedicalContractMety.DISCOUNT_TO_DATE;
                            var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => (this.MedicalContractMety.NATIONAL_NAME ?? "").Trim().ToLower() == (p.NATIONAL_NAME ?? "").Trim().ToLower()).FirstOrDefault();
                            if (national != null)
                            {
                                cboNationals.EditValue = national.ID;
                                chkEditNational.Checked = true;
                                txtNationalMainText.Text = national.NATIONAL_NAME;
                            }
                            else
                            {
                                chkEditNational.Checked = true;
                                txtNationalMainText.Text = this.MedicalContractMety.NATIONAL_NAME;
                            }

                            if (this.MedicalContractMety.MANUFACTURER_ID != null)
                            {
                                cboHangSX.EditValue = this.MedicalContractMety.MANUFACTURER_ID;
                            }

                            if (this.MedicalContractMety.MONTH_LIFESPAN.HasValue)
                            {
                                this.currrentServiceAdo.monthLifespan = this.MedicalContractMety.MONTH_LIFESPAN;
                            }
                            //Nếu chọn hợp đồng thì sẽ ưu tiên lấy thông tin trong hợp đồng. Hết
                            //cboDosageForm.EditValue = MedicalContractMety.DOSAGE_FORM;// lấy dữ liệu từ thầu hoặc loại thuốc - đang bắt buộc
                            txtSoDangKy.Text = this.MedicalContractMety.MEDICINE_REGISTER_NUMBER;
                            txtNognDoHL.Text = this.MedicalContractMety.CONCENTRA;
                            spinImpPriceVAT.Value = (spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100));
                            spinCanImpAmount.Value = (this.MedicalContractMety.AMOUNT - (this.MedicalContractMety.IN_AMOUNT ?? 0)) + (currrentServiceAdo.ADJUST_AMOUNT ?? 0);

                            txtBidNumber.Text = MedicalContractMety.BID_NUMBER;
                            txtBidNumOrder.Text = MedicalContractMety.BID_NUM_ORDER;
                            txtBid.Text = MedicalContractMety.BID_PACKAGE_CODE;
                            txtBidGroupCode.Text = MedicalContractMety.BID_GROUP_CODE;

                        }
                    }
                }
                else
                {
                    spinEditGiaTrongThau.Enabled = false;
                    spinImpVatRatio.Enabled = true;
                    if (data != null)
                    {
                        txtNognDoHL.Text = data.CONCENTRA;
                        var na = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => (data.NATIONAL_NAME ?? "").Trim().ToLower() == (p.NATIONAL_NAME ?? "").Trim().ToLower()).FirstOrDefault();
                        if (na != null)
                        {
                            cboNationals.EditValue = na.ID;
                            chkEditNational.Checked = true;
                            txtNationalMainText.Text = na.NATIONAL_NAME;
                        }
                        else
                        {
                            chkEditNational.Checked = true;
                            txtNationalMainText.Text = data.NATIONAL_NAME;
                        }
                    }
                    else
                    {
                        txtNognDoHL.Text = null;
                        txtNationalMainText.Text = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueByContractMaty(HIS.UC.MaterialType.ADO.MaterialTypeADO data)
        {
            try
            {

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                txtSoDangKy.Text = data != null ? data.REGISTER_NUMBER : "";
                if (this.currentContract != null)
                {
                    this.spinEditGiaTrongThau.Enabled = true;
                    this.spinImpPrice.Enabled = true;
                    this.cboNationals.Enabled = true;
                    this.cboHangSX.Enabled = true;
                    this.txtSoDangKy.Enabled = true;
                    this.SpMaxReuseCount.Enabled = false;
                    this.spinImpPriceVAT.Enabled = false;

                    if (dicContractMaty.Count > 0 && data != null)
                    {
                        var listvalue = dicContractMaty.Select(s => s.Value).ToList();

                        this.MedicalContractMaty = listvalue.FirstOrDefault(o => o.ID == data.MEDI_CONTRACT_MATY_ID);
                        if (this.MedicalContractMaty != null)
                        {
                            Inventec.Common.Logging.LogSystem.Info("MedicalContractMaty: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => MedicalContractMaty), MedicalContractMaty));

                            if (this.MedicalContractMaty.IMP_VAT_RATIO != null && this.MedicalContractMaty.IMP_VAT_RATIO > 0)
                            {
                                spinImpVatRatio.Value = this.MedicalContractMaty.IMP_VAT_RATIO.Value * 100;
                                spinEditThueXuat.Value = this.MedicalContractMaty.IMP_VAT_RATIO.Value * 100;
                                spinImpVatRatio.Enabled = false;
                            }
                            else
                            {
                                spinEditThueXuat.Value = 0;
                                spinImpVatRatio.Value = 0;
                                spinImpVatRatio.Enabled = true;
                            }

                            if (this.MedicalContractMaty.EXPIRED_DATE != null && this.MedicalContractMaty.EXPIRED_DATE > 0)
                            {
                                this.txtExpiredDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.MedicalContractMaty.EXPIRED_DATE ?? 0);
                                this.dtExpiredDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.MedicalContractMaty.EXPIRED_DATE ?? 0) ?? DateTime.Now;
                            }

                            if (this.MedicalContractMaty.CONTRACT_PRICE != null)
                            {
                                spinEditGiaTrongThau.Value = this.MedicalContractMaty.CONTRACT_PRICE.Value;

                                spinImpPrice.Value = this.MedicalContractMaty.CONTRACT_PRICE.Value / (1 + (this.MedicalContractMaty.IMP_VAT_RATIO.HasValue ? this.MedicalContractMaty.IMP_VAT_RATIO.Value : 0));
                                spinImpPrice1.Value = this.MedicalContractMaty.CONTRACT_PRICE.Value / (1 + (this.MedicalContractMaty.IMP_VAT_RATIO.HasValue ? this.MedicalContractMaty.IMP_VAT_RATIO.Value : 0));

                            }
                            else
                            {
                                spinEditGiaTrongThau.Value = 0;
                                spinImpPrice.Value = 0;
                                spinImpPrice1.Value = 0;
                            }

                            //cập nhật chính sách giá tại sự kiện spinImpPriceVAT_EditValueChanged không cần cập nhật tay
                            //LoadServicePatyByContractPrice(spinEditGiaTrongThau.Value, spinImpVatRatio.Value);

                            this.currrentServiceAdo.CONTRACT_PRICE = this.MedicalContractMaty.CONTRACT_PRICE;
                            this.currrentServiceAdo.MEDICAL_CONTRACT_ID = this.MedicalContractMaty.MEDICAL_CONTRACT_ID;
                            this.currrentServiceAdo.DISCOUNT_FROM_DATE = this.MedicalContractMaty.DISCOUNT_FROM_DATE;
                            this.currrentServiceAdo.DISCOUNT_TO_DATE = this.MedicalContractMaty.DISCOUNT_TO_DATE;
                            this.cboInformationBid.SelectedIndex = (int)(this.currrentServiceAdo.HisMaterial.INFORMATION_BID ?? 0) - 1;
                            var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => (this.MedicalContractMaty.NATIONAL_NAME ?? "").Trim().ToLower() == (p.NATIONAL_NAME ?? "").Trim().ToLower()).FirstOrDefault();
                            if (national != null)
                            {
                                cboNationals.EditValue = national.ID;
                                chkEditNational.Checked = true;
                                txtNationalMainText.Text = national.NATIONAL_NAME;
                            }
                            else
                            {
                                chkEditNational.Checked = true;
                                txtNationalMainText.Text = this.MedicalContractMaty.NATIONAL_NAME;
                            }

                            if (this.MedicalContractMaty.MANUFACTURER_ID != null)
                            {
                                cboHangSX.EditValue = this.MedicalContractMaty.MANUFACTURER_ID;
                            }

                            txtNognDoHL.Text = this.MedicalContractMaty.CONCENTRA;

                            spinImpPriceVAT.Value = (spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100));

                            decimal amountMap = 0;
                            if (this.currrentServiceAdo.MAP_MEDI_MATE_ID.HasValue && this.listServiceADO != null && this.listServiceADO.Count > 0)
                            {
                                var listMap = this.listServiceADO.Where(o => o.MAP_MEDI_MATE_ID == this.currrentServiceAdo.MAP_MEDI_MATE_ID).ToList();
                                if (listMap != null && listMap.Count > 0)
                                {
                                    amountMap = listMap.Sum(s => s.IMP_AMOUNT);
                                }
                            }

                            spinCanImpAmount.Value = (this.MedicalContractMaty.AMOUNT - (this.MedicalContractMaty.IN_AMOUNT ?? 0)) - amountMap + (currrentServiceAdo.ADJUST_AMOUNT ?? 0);

                            if (this.MedicalContractMaty.MONTH_LIFESPAN.HasValue)
                            {
                                this.currrentServiceAdo.monthLifespan = this.MedicalContractMaty.MONTH_LIFESPAN;
                            }

                            txtBidNumber.Text = MedicalContractMaty.BID_NUMBER;
                            txtBidNumOrder.Text = MedicalContractMaty.BID_NUM_ORDER;
                            txtBid.Text = MedicalContractMaty.BID_PACKAGE_CODE;
                            txtBidGroupCode.Text = MedicalContractMaty.BID_GROUP_CODE;
                        }
                    }
                }
                else
                {
                    spinEditGiaTrongThau.Enabled = false;
                    spinImpVatRatio.Enabled = true;
                    if (data != null)
                    {
                        txtNognDoHL.Text = data.CONCENTRA;
                        var na = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => (data.NATIONAL_NAME ?? "").Trim().ToLower() == (p.NATIONAL_NAME ?? "").Trim().ToLower()).FirstOrDefault();
                        if (na != null)
                        {
                            cboNationals.EditValue = na.ID;
                            chkEditNational.Checked = true;
                            txtNationalMainText.Text = na.NATIONAL_NAME;
                        }
                        else
                        {
                            chkEditNational.Checked = true;
                            txtNationalMainText.Text = data.NATIONAL_NAME;
                        }
                    }
                    else
                    {
                        txtNognDoHL.Text = null;
                        txtNationalMainText.Text = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineTypeTree_RowEnter(UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                medicineTypeTree_Click(data);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialTypeTree_Click(UC.MaterialType.ADO.MaterialTypeADO data)
        {
            try
            {
                Thread threadLoadMaterial = null;
                if (this.configWarningDiff == null)
                {
                    configWarningDiff = new HIS_CONFIG();
                    configWarningDiff.VALUE = HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.IsWarningDifferenceLastImp");// BackendDataWorker.Get<HIS_CONFIG>().Where(s => s.KEY == "HIS.Desktop.Plugins.ImpMestCreate.IsWarningDifferenceLastImp").FirstOrDefault();

                }
                if (configWarningDiff != null && configWarningDiff.VALUE == "1")
                {
                    threadLoadMaterial = new Thread(() => GetMaterial(data.ID));

                }
                if (btnCancel1.Enabled == true)
                {
                    SetEnableButton(false);
                    ResetValueControlDetail();
                    SetFocuTreeMediMate();
                    SetEnableButton(false);
                }
                dtExpiredDate.EditValue = null;
                txtExpiredDate.Text = "";
                txtPackageNumber.Text = "";
                txtBid.Text = "";
                txtBid.Properties.MaxLength = 4;
                txtHeinServiceBidMateType.Text = "";
                dxValidationProvider2.RemoveControlError(txtHeinServiceBidMateType);
                dxValidationProvider2.RemoveControlError(spinImpAmount);
                dxValidationProvider2.RemoveControlError(txtPackageNumber);
                dxValidationProvider2.RemoveControlError(txtExpiredDate);

                if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && medistock.IS_ALLOW_IMP_SUPPLIER == 0)
                {
                    return;
                }

                WaitingManager.Show();
                this.currrentServiceAdo = null;
                if (data != null)
                {
                    if (threadLoadMaterial != null) threadLoadMaterial.Start();
                    if (data.IS_SALE_EQUAL_IMP_PRICE == 1)
                    {
                        chkImprice.Checked = true;
                        chkImprice.Enabled = false;
                    }
                    else
                    {
                        chkImprice.Checked = false;
                        chkImprice.Enabled = true;
                    }

                    this.currrentServiceAdo = new ADO.VHisServiceADO((V_HIS_MATERIAL_TYPE)data);
                    this.currrentServiceAdo.ADJUST_AMOUNT = data.ADJUST_AMOUNT;
                    this.cboInformationBid.Enabled = true;
                    if (materialProcessor.GetBid(ucMaterialTypeTree) != null && dicBidMaterial.Count > 0 && dicBidMaterial.ContainsKey(Base.StaticMethod.GetTypeKey(data.ID, data.BidGroupCode)))
                    {
                        var bidMate = dicBidMaterial[Base.StaticMethod.GetTypeKey(data.ID, data.BidGroupCode)];
                        if (bidMate != null)
                        {
                            this.currrentServiceAdo.TDL_BID_GROUP_CODE = bidMate.BID_GROUP_CODE;
                            this.currrentServiceAdo.TDL_BID_NUM_ORDER = bidMate.BID_NUM_ORDER;
                            this.currrentServiceAdo.TDL_BID_PACKAGE_CODE = bidMate.BID_PACKAGE_CODE;
                            this.currrentServiceAdo.TDL_BID_YEAR = bidMate.BID_YEAR;
                            this.currrentServiceAdo.TDL_BID_NUMBER = bidMate.BID_NUMBER;
                            this.currrentServiceAdo.EXPIRED_DATE = bidMate.EXPIRED_DATE;
                            this.currrentServiceAdo.monthLifespan = bidMate.MONTH_LIFESPAN;
                            if (currrentServiceAdo.HisMaterial == null)
                                currrentServiceAdo.HisMaterial = new HIS_MATERIAL();
                            this.currrentServiceAdo.HisMaterial.INFORMATION_BID = (int?)bidMate.INFORMATION_BID;
                            if (this.currrentServiceAdo.HisMaterial.INFORMATION_BID.HasValue)
                            {
                                this.cboInformationBid.Enabled = false;
                            }
                        }
                    }

                    this.SoloValidBidControl(txtPackageNumber, layoutPackageNumber, false);

                    if (this.currrentServiceAdo.IsRequireHsd)
                    {
                        ValidControlExpiredDate1(dtExpiredDate);
                        ValidControlExpiredDate1(txtExpiredDate);
                        layoutExpiredDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                    }
                    else
                    {
                        DevExpress.XtraEditors.DXErrorProvider.ValidationRule validationRule = new DevExpress.XtraEditors.DXErrorProvider.ValidationRule();
                        dxValidationProvider2.SetValidationRule(dtExpiredDate, validationRule);
                        dxValidationProvider2.SetValidationRule(txtExpiredDate, validationRule);
                        layoutExpiredDate.AppearanceItemCaption.ForeColor = Color.Black;
                    }
                }

                VisibleLayoutTemperature();
                this.SetPrimary();
                LoadServicePatyByAdo();
                SetValueByServiceAdo();
                SetValueByContractMaty(data);

                if (IsHasValueChooice && IsNCC)
                {
                    SetEnableControl(this.IsAllowedToEnableMedicinesInformation);
                }
                TimerFocusImpAmount.Start();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialTypeTree_RowEnter(UC.MaterialType.ADO.MaterialTypeADO data)
        {
            try
            {
                materialTypeTree_Click(data);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueByServiceAdo()
        {
            try
            {
                if (this.currrentServiceAdo != null)
                {
                    //xuandv chuyen len tren de k trung voi jtra vttsd
                    if (medistock != null && medistock.ID > 0 && medistock.IS_BUSINESS != 1)
                    {
                        //disable vẫn giữ trạng thái check?uncheck "bán bằng giá nhập" theo loại thuốc trong danh mục thuốc vật tư
                        if (IsDisableChkImprice)
                            chkImprice.Enabled = false;
                        else
                        {
                            //chkImprice.Enabled = true;
                            chkImprice.Checked = true;
                        }
                    }

                    GetLastImpPrice();

                    ProcessBidByType();

                    EventProcessMaterialReUse();

                    spinImpAmount.Value = 0;
                    btnAdd1.Enabled = true;
                    bool valid = false;

                    ValidBidCheckValidate();
                    if (cboGoiThau.EditValue != null && this.currentBid == null && listBids != null && listBids.Count() > 0)//nambg #26003
                    {
                        this.currentBid = listBids.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboGoiThau.EditValue.ToString()));
                    }

                    if (this.currentBid != null && !checkOutBid.Checked)
                    {
                        valid = true;
                        MethodProcessBefoClick();
                        spinEditGiaTrongThau.Value = currrentServiceAdo.IMP_PRICE;
                    }
                    else
                    {
                        //thông tin nước sản xuất ... lấy trong loại thuốc/vật tư
                        ProcessInfoFromType();

                        spinEditGiaTrongThau.Value = 0;
                        txtBidYear.Enabled = true;
                        txtBidNumber.Enabled = true;
                        txtBidNumOrder.Enabled = true;
                        txtBidGroupCode.Enabled = true;
                        txtBid.Enabled = true;
                    }

                    spinImpPrice1.Value = currrentServiceAdo.IMP_PRICE;
                    spinImpPrice.Value = currrentServiceAdo.IMP_PRICE;
                    decimal vatRatio = currrentServiceAdo.IMP_VAT_RATIO * 100;
                    spinImpVatRatio.Value = vatRatio;
                    spinEditThueXuat.Value = IsShowThueXuat ? vatRatio : 0;
                    Inventec.Common.Logging.LogSystem.Debug("vatRatio-----------" + vatRatio);
                    if (vatRatio <= 0)
                    {
                        spinImpVatRatio.Value = VatConfig;
                    }

                    ValidBid();

                    if (!valid)
                    {
                        txtBidGroupCode.Text = "";
                        txtBidYear.Text = "";
                        txtBidNumber.Text = "";
                        txtBidNumOrder.Text = "";
                        spinCanImpAmount.Value = 0;
                    }

                    ValidateCboGoiThau();
                }
                else
                {
                    spinImpAmount.Value = 0;
                    spinImpPrice1.Value = 0;
                    spinImpPrice.Value = 0;
                    spinEditGiaTrongThau.Value = 0;
                    spinImpVatRatio.Value = 0;
                    spinEditThueXuat.Value = 0;
                    txtBidNumOrder.Text = "";
                    txtBidYear.Text = "";
                    txtBidNumber.Text = "";
                    spinCanImpAmount.Value = 0;
                    btnAdd1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessInfoFromType()
        {
            try
            {
                if (this.currrentServiceAdo != null)
                {
                    cboHangSX.EditValue = null;
                    cboNationals.EditValue = null;
                    txtNationalMainText.Text = null;
                    chkEditNational.Checked = false;
                    txtNognDoHL.Text = null;
                    txtSoDangKy.Text = null;
                    this.txtPackingJoinBid.Text = null;
                    this.txtHeinServiceBidMateType.Text = null;
                    this.txtActiveIngrBhytName.Text = null;
                    this.cboDosageForm.EditValue = null;
                    this.cboMedicineUseForm.EditValue = null;

                    if (this.currrentServiceAdo.IsMedicine)
                    {
                        var dataMediType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == this.currrentServiceAdo.MEDI_MATE_ID);
                        if (dataMediType != null)
                        {
                            if (!string.IsNullOrEmpty(dataMediType.NATIONAL_NAME))
                            {
                                var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => (dataMediType.NATIONAL_NAME ?? "").Trim().ToLower() == (p.NATIONAL_NAME ?? "").Trim().ToLower()).FirstOrDefault();
                                if (national != null)
                                {
                                    cboNationals.EditValue = national.ID;
                                    chkEditNational.Checked = false;
                                    txtNationalMainText.Text = national.NATIONAL_NAME;
                                }
                                else
                                {
                                    chkEditNational.Checked = true;
                                    txtNationalMainText.Text = dataMediType.NATIONAL_NAME;
                                }
                            }

                            txtNognDoHL.Text = dataMediType.CONCENTRA;
                            txtSoDangKy.Text = dataMediType.REGISTER_NUMBER;
                            if (dataMediType.MANUFACTURER_ID > 0)
                                cboHangSX.EditValue = dataMediType.MANUFACTURER_ID;

                            this.txtPackingJoinBid.Text = dataMediType.PACKING_TYPE_NAME;
                            this.txtHeinServiceBidMateType.Text = dataMediType.HEIN_SERVICE_BHYT_NAME;
                            this.txtActiveIngrBhytName.Text = dataMediType.ACTIVE_INGR_BHYT_NAME;
                            this.cboDosageForm.EditValue = dataMediType.DOSAGE_FORM;
                            this.cboMedicineUseForm.EditValue = dataMediType.MEDICINE_USE_FORM_ID;
                            txtBidNumber.Text = this.currrentServiceAdo.TDL_BID_NUMBER;
                        }
                    }
                    else
                    {
                        var dataMateType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == this.currrentServiceAdo.MEDI_MATE_ID);
                        if (dataMateType != null)
                        {
                            if (!string.IsNullOrEmpty(dataMateType.NATIONAL_NAME))
                            {
                                var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => (dataMateType.NATIONAL_NAME ?? "").Trim().ToLower() == (p.NATIONAL_NAME ?? "").Trim().ToLower()).FirstOrDefault();
                                if (national != null)
                                {
                                    cboNationals.EditValue = national.ID;
                                    chkEditNational.Checked = false;
                                    txtNationalMainText.Text = national.NATIONAL_NAME;
                                }
                                else
                                {
                                    chkEditNational.Checked = true;
                                    txtNationalMainText.Text = dataMateType.NATIONAL_NAME;
                                }
                            }
                            txtSoDangKy.Text = dataMateType.REGISTER_NUMBER;
                            txtNognDoHL.Text = dataMateType.CONCENTRA;
                            if (dataMateType.MANUFACTURER_ID > 0)
                                cboHangSX.EditValue = dataMateType.MANUFACTURER_ID;
                            txtBidNumber.Text = this.currrentServiceAdo.TDL_BID_NUMBER;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetLastImpPrice()
        {
            try
            {
                spinEditGiaNhapLanTruoc.EditValue = null;

                if (this.currrentServiceAdo.IsMedicine)
                {
                    MOS.Filter.HisMedicineTypeFilter filter = new HisMedicineTypeFilter();
                    filter.ID = this.currrentServiceAdo.MEDI_MATE_ID;
                    var data = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE_TYPE>>("api/HisMedicineType/Get", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                    if (data != null)
                    {
                        decimal? giaNhapLanTruoc = (data.LAST_IMP_PRICE ?? 0) * (1 + (data.LAST_IMP_VAT_RATIO ?? 0));
                        spinEditGiaNhapLanTruoc.Value = giaNhapLanTruoc ?? 0;
                        this.currrentServiceAdo.GiaBan = (data.LAST_EXP_PRICE ?? 0) * (1 + data.LAST_EXP_VAT_RATIO ?? 0);

                    }
                }
                else
                {
                    MOS.Filter.HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                    filter.ID = this.currrentServiceAdo.MEDI_MATE_ID;
                    var data = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL_TYPE>>("api/HisMaterialType/Get", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                    if (data != null)
                    {
                        decimal? giaNhapLanTruoc = (data.LAST_IMP_PRICE ?? 0) * (1 + (data.LAST_IMP_VAT_RATIO ?? 0));
                        spinEditGiaNhapLanTruoc.Value = giaNhapLanTruoc ?? 0;
                        this.currrentServiceAdo.GiaBan = (data.LAST_EXP_PRICE ?? 0) * (1 + data.LAST_EXP_VAT_RATIO ?? 0);


                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MethodProcessBefoClick()
        {
            try
            {
                if (this.currrentServiceAdo.IsMedicine)
                {
                    V_HIS_BID_MEDICINE_TYPE bidMediType = new V_HIS_BID_MEDICINE_TYPE();
                    if (this.currentBid != null
                        && this._dicMedicineTypes != null
                        && this._dicMedicineTypes.ContainsKey(this.currentBid.ID))
                    {
                        bidMediType = this._dicMedicineTypes[this.currentBid.ID].FirstOrDefault(p => p.MEDICINE_TYPE_ID == this.currrentServiceAdo.MEDI_MATE_ID && p.BID_GROUP_CODE == this.currrentServiceAdo.TDL_BID_GROUP_CODE);
                    }

                    if ((bidMediType == null || bidMediType.ID == 0) && dicBidMedicine.ContainsKey(Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)))
                    {
                        bidMediType = dicBidMedicine[Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)];
                    }

                    if (bidMediType != null && bidMediType.ID > 0)
                    {
                        if (!string.IsNullOrEmpty(bidMediType.NATIONAL_NAME))
                        {
                            var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => (bidMediType.NATIONAL_NAME ?? "").Trim().ToLower() == (p.NATIONAL_NAME ?? "").Trim().ToLower()).FirstOrDefault();
                            if (national != null)
                            {
                                cboNationals.EditValue = national.ID;
                                chkEditNational.Checked = false;
                                txtNationalMainText.Text = national.NATIONAL_NAME;
                            }
                            else
                            {
                                chkEditNational.Checked = true;
                                txtNationalMainText.Text = bidMediType.NATIONAL_NAME;
                            }
                        }
                        else
                        {
                            cboNationals.EditValue = null;
                            txtNationalMainText.Text = null;
                            chkEditNational.Checked = false;
                        }

                        if (bidMediType.MANUFACTURER_ID > 0)
                            cboHangSX.EditValue = bidMediType.MANUFACTURER_ID;
                        else
                            cboHangSX.EditValue = null;

                        txtNognDoHL.Text = String.IsNullOrWhiteSpace(bidMediType.CONCENTRA) && this.IsSetBhytInfoFromTypeByDefault ? this.currrentServiceAdo.CONCENTRA : bidMediType.CONCENTRA;
                        txtSoDangKy.Text = String.IsNullOrWhiteSpace(bidMediType.MEDICINE_REGISTER_NUMBER) && this.IsSetBhytInfoFromTypeByDefault ? this.currrentServiceAdo.REGISTER_NUMBER : bidMediType.REGISTER_NUMBER;
                        txtPackingJoinBid.Text = String.IsNullOrWhiteSpace(bidMediType.PACKING_TYPE_NAME) && this.IsSetBhytInfoFromTypeByDefault ? this.currrentServiceAdo.packingTypeName : bidMediType.PACKING_TYPE_NAME;
                        txtHeinServiceBidMateType.Text = String.IsNullOrWhiteSpace(bidMediType.HEIN_SERVICE_BHYT_NAME) && this.IsSetBhytInfoFromTypeByDefault ? this.currrentServiceAdo.heinServiceBhytName : bidMediType.HEIN_SERVICE_BHYT_NAME;
                        txtActiveIngrBhytName.Text = String.IsNullOrWhiteSpace(bidMediType.ACTIVE_INGR_BHYT_NAME) && this.IsSetBhytInfoFromTypeByDefault ? this.currrentServiceAdo.activeIngrBhytName : bidMediType.ACTIVE_INGR_BHYT_NAME;
                        cboDosageForm.EditValue = String.IsNullOrWhiteSpace(bidMediType.DOSAGE_FORM) && this.IsSetBhytInfoFromTypeByDefault ? this.currrentServiceAdo.HisMedicine.DOSAGE_FORM : bidMediType.DOSAGE_FORM;

                        if (!bidMediType.MEDICINE_USE_FORM_ID.HasValue && this.IsSetBhytInfoFromTypeByDefault)
                        {
                            this.cboMedicineUseForm.EditValue = this.currrentServiceAdo.medicineUseFormId;
                        }
                        else
                        {
                            this.cboMedicineUseForm.EditValue = bidMediType.MEDICINE_USE_FORM_ID;
                        }

                        txtBidNumOrder.Text = bidMediType.BID_NUM_ORDER;
                        txtBidGroupCode.Text = bidMediType.BID_GROUP_CODE;
                        txtBidYear.Text = bidMediType.BID_YEAR;
                        txtBidNumber.Text = bidMediType.BID_NUMBER;
                        txtBid.Text = bidMediType.BID_PACKAGE_CODE;
                        if (bidMediType.EXPIRED_DATE != null && bidMediType.EXPIRED_DATE > 0)
                        {
                            this.txtExpiredDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(bidMediType.EXPIRED_DATE ?? 0);
                            this.dtExpiredDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bidMediType.EXPIRED_DATE ?? 0) ?? DateTime.Now;
                        }

                        if (bidMediType.VALID_FROM_TIME > 0)
                        {
                            dtHieuLucTu.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bidMediType.VALID_FROM_TIME ?? 0) ?? DateTime.Now;
                        }
                        else
                        {
                            dtHieuLucTu.EditValue = null;
                        }

                        if (bidMediType.VALID_TO_TIME > 0)
                        {
                            dtHieuLucDen.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bidMediType.VALID_TO_TIME ?? 0) ?? DateTime.Now;
                        }
                        else
                        {
                            dtHieuLucDen.EditValue = null;
                        }

                        spinCanImpAmount.Value = Math.Round(bidMediType.AMOUNT + (bidMediType.AMOUNT * bidMediType.IMP_MORE_RATIO ?? 0) - (bidMediType.IN_AMOUNT ?? 0) + (currrentServiceAdo.ADJUST_AMOUNT ?? 0), MidpointRounding.AwayFromZero);
                        this.currrentServiceAdo.BidImpPrice = bidMediType.IMP_PRICE;
                        this.currrentServiceAdo.BidImpVatRatio = bidMediType.IMP_VAT_RATIO;
                        if (bidMediType.IMP_PRICE.HasValue)
                        {
                            currrentServiceAdo.IMP_PRICE = bidMediType.IMP_PRICE.Value;
                        }

                        if (bidMediType.IMP_VAT_RATIO.HasValue)
                        {
                            currrentServiceAdo.IMP_VAT_RATIO = bidMediType.IMP_VAT_RATIO.Value;
                            currrentServiceAdo.ImpVatRatio = bidMediType.IMP_VAT_RATIO.Value * 100;
                        }

                        if (spinCanImpAmount.Value <= HisBidAlertAmountCFG.Bid_AlertAmount)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessageManager.SoLuongKhaNhapCuThuocVatTuTrongGoiThauNhoHon, HisBidAlertAmountCFG.Bid_AlertAmount), Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                        }
                    }
                    else
                    {
                        ProcessInfoFromType();
                        txtBidNumOrder.Text = "";
                        txtBidYear.Text = "";
                        txtBidNumber.Text = "";
                        spinCanImpAmount.Value = 0;
                    }
                }
                else
                {
                    V_HIS_BID_MATERIAL_TYPE bidMateType = new V_HIS_BID_MATERIAL_TYPE();
                    if (this._dicMaterialTypes != null && this._dicMaterialTypes.ContainsKey(this.currentBid.ID))
                    {
                        bidMateType = this._dicMaterialTypes[this.currentBid.ID].FirstOrDefault(p => p.MATERIAL_TYPE_ID == currrentServiceAdo.MEDI_MATE_ID && p.BID_GROUP_CODE == this.currrentServiceAdo.TDL_BID_GROUP_CODE);

                        if ((bidMateType == null || bidMateType.ID == 0) && currrentServiceAdo.MAP_MEDI_MATE_ID.HasValue)
                        {
                            bidMateType = this._dicMaterialTypes[this.currentBid.ID].FirstOrDefault(p => p.MATERIAL_TYPE_ID == currrentServiceAdo.MAP_MEDI_MATE_ID && p.BID_GROUP_CODE == this.currrentServiceAdo.TDL_BID_GROUP_CODE);
                        }
                    }

                    if ((bidMateType == null || bidMateType.ID == 0) && dicBidMaterial.ContainsKey(Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)))
                    {
                        bidMateType = dicBidMaterial[Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)];
                    }

                    if (bidMateType != null && bidMateType.ID > 0)
                    {
                        if (!string.IsNullOrEmpty(bidMateType.NATIONAL_NAME))
                        {
                            var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => (bidMateType.NATIONAL_NAME ?? "").Trim().ToLower() == (p.NATIONAL_NAME ?? "").Trim().ToLower()).FirstOrDefault();
                            if (national != null)
                            {
                                cboNationals.EditValue = national.ID;
                                chkEditNational.Checked = false;
                                txtNationalMainText.Text = national.NATIONAL_NAME;
                            }
                            else
                            {
                                chkEditNational.Checked = true;
                                txtNationalMainText.Text = bidMateType.NATIONAL_NAME;
                            }
                        }
                        else
                        {
                            cboNationals.EditValue = null;
                            txtNationalMainText.Text = null;
                            chkEditNational.Checked = false;
                        }

                        txtNognDoHL.Text = bidMateType.CONCENTRA;
                        if (bidMateType.MANUFACTURER_ID > 0)
                            cboHangSX.EditValue = bidMateType.MANUFACTURER_ID;
                        else
                            cboHangSX.EditValue = null;

                        txtPackingJoinBid.Text = bidMateType.BID_MATERIAL_TYPE_CODE;
                        txtHeinServiceBidMateType.Text = bidMateType.BID_MATERIAL_TYPE_NAME;

                        txtBidNumOrder.Text = bidMateType.BID_NUM_ORDER;
                        txtBidGroupCode.Text = bidMateType.BID_GROUP_CODE;
                        txtBidYear.Text = bidMateType.BID_YEAR;
                        txtBidNumber.Text = bidMateType.BID_NUMBER;
                        txtBid.Text = bidMateType.BID_PACKAGE_CODE;
                        cboInformationBid.SelectedIndex = (int)(bidMateType.INFORMATION_BID ?? 0) - 1;
                        if (bidMateType.EXPIRED_DATE != null && bidMateType.EXPIRED_DATE > 0)
                        {
                            this.txtExpiredDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(bidMateType.EXPIRED_DATE ?? 0);
                            this.dtExpiredDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bidMateType.EXPIRED_DATE ?? 0) ?? DateTime.Now;
                        }

                        if (bidMateType.VALID_FROM_TIME > 0)
                        {
                            dtHieuLucTu.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bidMateType.VALID_FROM_TIME ?? 0) ?? DateTime.Now;
                        }
                        else
                        {
                            dtHieuLucTu.EditValue = null;
                        }

                        if (bidMateType.VALID_TO_TIME > 0)
                        {
                            dtHieuLucDen.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bidMateType.VALID_TO_TIME ?? 0) ?? DateTime.Now;
                        }
                        else
                        {
                            dtHieuLucDen.EditValue = null;
                        }

                        decimal amountMap = 0;
                        if (this.currrentServiceAdo.MAP_MEDI_MATE_ID.HasValue && this.listServiceADO != null && this.listServiceADO.Count > 0)
                        {
                            var listMap = this.listServiceADO.Where(o => o.MAP_MEDI_MATE_ID == this.currrentServiceAdo.MAP_MEDI_MATE_ID).ToList();
                            if (listMap != null && listMap.Count > 0)
                            {
                                amountMap = listMap.Sum(s => s.IMP_AMOUNT);
                            }
                        }

                        spinCanImpAmount.Value = Math.Round(bidMateType.AMOUNT + (bidMateType.AMOUNT * bidMateType.IMP_MORE_RATIO ?? 0) - (bidMateType.IN_AMOUNT ?? 0) - amountMap + (currrentServiceAdo.ADJUST_AMOUNT ?? 0), MidpointRounding.AwayFromZero);
                        this.currrentServiceAdo.BidImpPrice = bidMateType.IMP_PRICE;
                        this.currrentServiceAdo.BidImpVatRatio = bidMateType.IMP_VAT_RATIO;
                        if (bidMateType.IMP_PRICE.HasValue)
                        {
                            currrentServiceAdo.IMP_PRICE = bidMateType.IMP_PRICE.Value;
                        }

                        if (bidMateType.IMP_VAT_RATIO.HasValue)
                        {
                            currrentServiceAdo.IMP_VAT_RATIO = bidMateType.IMP_VAT_RATIO.Value;
                            currrentServiceAdo.ImpVatRatio = bidMateType.IMP_VAT_RATIO.Value * 100;
                        }

                        if (spinCanImpAmount.Value <= HisBidAlertAmountCFG.Bid_AlertAmount)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessageManager.SoLuongKhaNhapCuThuocVatTuTrongGoiThauNhoHon, HisBidAlertAmountCFG.Bid_AlertAmount), Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                        }

                        var dataMatyType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == this.MedicalContractMaty.MATERIAL_TYPE_ID);
                        if (dataMatyType != null)
                        {
                            txtSoDangKy.Text = dataMatyType.REGISTER_NUMBER;
                        }
                    }
                    else
                    {
                        ProcessInfoFromType();

                        txtBidNumOrder.Text = "";
                        txtBidYear.Text = "";
                        txtBidNumber.Text = "";
                        spinCanImpAmount.Value = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServicePatyByAdo()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadServicePatyByAdo()-Start");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.currrentServiceAdo", this.currrentServiceAdo));
                if (this.currrentServiceAdo != null && chkImprice.Checked == false && (!this._VACCINE_EXP_PRICE_OPTION || !currrentServiceAdo.IsVaccin))
                {
                    if (!dicServicePaty.ContainsKey(this.currrentServiceAdo.SERVICE_ID) || dicServicePaty[this.currrentServiceAdo.SERVICE_ID].Count == 0)
                    {
                        Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

                        var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_PATY>>("api/HisServicePaty/GetAppliedView", ApiConsumers.MosConsumer, null, null, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, "serviceId", this.currrentServiceAdo.SERVICE_ID, "treatmentTime", null);
                        int row = 1;
                        List<HIS_MEDICINE_PATY> listMedcinePaty = new List<HIS_MEDICINE_PATY>();
                        List<HIS_MATERIAL_PATY> listMaterialPaty = new List<HIS_MATERIAL_PATY>();
                        CommonParam param = new CommonParam();
                        if (currrentServiceAdo.HisMedicine != null)
                        {
                            listMedcinePaty = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MEDICINE_PATY>>("api/HisMedicinePaty/GetOfLast", ApiConsumers.MosConsumer, currrentServiceAdo.HisMedicine.MEDICINE_TYPE_ID, param);
                        }

                        if (currrentServiceAdo.HisMaterial != null)
                        {
                            listMaterialPaty = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MATERIAL_PATY>>("api/HisMaterialPaty/GetOfLast", ApiConsumers.MosConsumer, currrentServiceAdo.HisMaterial.MATERIAL_TYPE_ID, param);
                        }

                        foreach (var item in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
                        {
                            VHisServicePatyADO ado = new VHisServicePatyADO();
                            ado.PATIENT_TYPE_NAME = item.PATIENT_TYPE_NAME;
                            ado.PATIENT_TYPE_ID = item.ID;
                            ado.PATIENT_TYPE_CODE = item.PATIENT_TYPE_CODE;
                            ado.IsNotSell = true;
                            ado.SERVICE_TYPE_ID = this.currrentServiceAdo.SERVICE_TYPE_ID;
                            ado.SERVICE_ID = this.currrentServiceAdo.SERVICE_ID;
                            if (listMedcinePaty != null && listMedcinePaty.Count > 0)
                            {
                                var dataMePaty = listMedcinePaty.Where(o => o.PATIENT_TYPE_ID == ado.PATIENT_TYPE_ID).ToList();
                                ado.PRE_PRICE_Str = dataMePaty != null && dataMePaty.Count > 0 ? (dataMePaty.FirstOrDefault().EXP_PRICE * (1 + dataMePaty.FirstOrDefault().EXP_VAT_RATIO)) : 0;
                            }
                            if (listMaterialPaty != null && listMaterialPaty.Count > 0)
                            {
                                var dataMaPaty = listMaterialPaty.Where(o => o.PATIENT_TYPE_ID == ado.PATIENT_TYPE_ID).ToList();
                                ado.PRE_PRICE_Str = dataMaPaty != null && dataMaPaty.Count > 0 ? (dataMaPaty.FirstOrDefault().EXP_PRICE * (1 + dataMaPaty.FirstOrDefault().EXP_VAT_RATIO)) : 0;
                            }
                            ado.ID = row;
                            row++;
                            dicPaty[item.ID] = ado;
                        }

                        if (listServicePaty != null && listServicePaty.Count > 0)
                        {
                            foreach (var item in listServicePaty)
                            {
                                if (dicPaty.ContainsKey(item.PATIENT_TYPE_ID))
                                {
                                    var ado = dicPaty[item.PATIENT_TYPE_ID];
                                    if (!ado.IsSetExpPrice)
                                    {
                                        ado.ExpPrice = item.PRICE;
                                        ado.VAT_RATIO = item.VAT_RATIO;
                                        ado.ExpVatRatio = item.VAT_RATIO * 100;
                                        ado.ExpPriceVat = item.PRICE * (1 + item.VAT_RATIO);
                                        ado.PRICE = item.PRICE * (1 + item.VAT_RATIO);
                                        ado.PercentProfit = 0;
                                        ado.IsNotSell = false;
                                        ado.IsSetExpPrice = true;
                                    }
                                }
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dicPaty", dicPaty));

                        dicServicePaty[this.currrentServiceAdo.SERVICE_ID] = dicPaty.Select(s => s.Value).ToList();
                    }

                    var listData = dicServicePaty[this.currrentServiceAdo.SERVICE_ID];
                    AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                    listServicePatyAdo = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(listData);
                    decimal price = (1 + this.currrentServiceAdo.IMP_VAT_RATIO) * this.currrentServiceAdo.IMP_PRICE;

                    foreach (var item in listServicePatyAdo)
                    {
                        if (this.currrentServiceAdo.IsServiceUnitPrimary)
                        {
                            item.VAT_RATIO = this.currrentServiceAdo.IMP_VAT_RATIO;
                            item.ExpPrice = this.currrentServiceAdo.IMP_PRICE;
                            item.ExpVatRatio = item.VAT_RATIO * 100;
                        }
                        item.IsReusable = this.currrentServiceAdo.IsReusable;
                        item.IsIdentity = this.currrentServiceAdo.IsIdentity;
                        //                           item.ExpPriceVat = item.ExpPrice * (1 + item.VAT_RATIO);
                        item.ExpPriceVat = price;
                        item.PercentProfit = CheckProfitCfg(price, item.ID);
                        item.PRICE = (1 + item.PercentProfit / 100) * item.ExpPriceVat;
                    }
                }
                else
                {
                    listServicePatyAdo = new List<VHisServicePatyADO>();
                }

                listServicePatyAdo = listServicePatyAdo.OrderByDescending(o => o.IsNotSell == false).ToList();
                gridControlServicePaty.BeginUpdate();
                UpdateExpPrice();
                long tp_ = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AmountDecimalNumber"));
                long tp = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AutoRoundExpPriceOption"));
                if (listServicePatyAdo != null && listServicePatyAdo.Count > 0)
                {
                    foreach (var item in listServicePatyAdo)
                    {
                        if (tp == 1)
                        {
                            item.PRICE = Math.Round(item.PRICE, (int)tp_);
                        }
                        if (tp == 2)
                        {
                            item.PRICE = RoundDown(item.PRICE, tp_);
                        }
                        if (tp == 3)
                        {
                            item.PRICE = RoundUp(item.PRICE, tp_);
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listServicePatyAdo", listServicePatyAdo));
                gridControlServicePaty.DataSource = listServicePatyAdo;
                gridControlServicePaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Debug("LoadServicePatyByAdo()-Ended");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPrimary()
        {
            try
            {
                if (this.medistock != null && this.medistock.IS_BUSINESS == 1)
                {
                    var data = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(p => p.ID == this.currrentServiceAdo.SERVICE_UNIT_ID);
                    if (data != null && data.IS_PRIMARY == 1)
                    {
                        this.currrentServiceAdo.IsServiceUnitPrimary = true;
                    }
                    else
                    {
                        this.currrentServiceAdo.IsServiceUnitPrimary = false;
                    }
                }
                else
                {
                    this.currrentServiceAdo.IsServiceUnitPrimary = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal CheckProfitCfg(decimal price, long patientTypeId)
        {
            decimal result = 0;
            try
            {
                if (this.medistock != null && this.medistock.IS_BUSINESS == 1 && !(chkNoProfitBhyt.Checked && patientTypeId == Config.PatientTypeCFG.PATIENT_TYPE_ID__BHYT))
                {
                    //#21344
                    var data = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(p => p.ID == this.currrentServiceAdo.SERVICE_UNIT_ID);
                    if (data != null && data.IS_PRIMARY == 1)
                    {

                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._HisSaleProfitCfgs), this._HisSaleProfitCfgs));
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currrentServiceAdo), this.currrentServiceAdo));
                        var dataSales = this._HisSaleProfitCfgs.Where(p => (this.currrentServiceAdo.IsMedicine ? (p.IS_MEDICINE == 1 || (this.currrentServiceAdo.isFunctionalFood == 1 && p.IS_FUNCTIONAL_FOOD == 1) || (this.currrentServiceAdo.isFunctionalFood != 1 && p.IS_COMMON_MEDICINE == 1)) : p.IS_MATERIAL == 1)
                            && ((p.IMP_PRICE_FROM != null && p.IMP_PRICE_TO != null) ? (p.IMP_PRICE_FROM <= price && p.IMP_PRICE_TO >= price) :
                                ((p.IMP_PRICE_FROM != null && p.IMP_PRICE_TO == null) ? p.IMP_PRICE_FROM <= price :
                                    ((p.IMP_PRICE_FROM == null && p.IMP_PRICE_TO != null) ? p.IMP_PRICE_TO >= price : true)))
                            ).ToList();

                        if (dataSales != null && dataSales.Count > 0)
                        {
                            if (medistock.IS_SHOW_DRUG_STORE == 1)
                            {
                                dataSales = dataSales.Where(o => o.IS_DRUG_STORE == 1).ToList();
                            }
                            else
                            {
                                dataSales = dataSales.Where(o => o.IS_DRUG_STORE == null).ToList();
                            }
                            if (dataSales != null && dataSales.Count > 0)
                            {
                                dataSales = dataSales.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                result = dataSales.FirstOrDefault().RATIO;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void EventProcessMaterialReUse()
        {
            try
            {
                if (!this.currrentServiceAdo.IsMedicine && (this.currrentServiceAdo.IsReusable || this.currrentServiceAdo.IsIdentity))
                {
                    TxtSerialNumber.Enabled = true;
                    SpMaxReuseCount.Enabled = true;
                    spinEditGiaVeSinh.Enabled = true;
                    lciReUseMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciSeriNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciGiaVeSinh.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    SpMaxReuseCount.EditValue = 0;
                    SpMaxReuseCount.EditValue = this.currrentServiceAdo.MAX_REUSE_COUNT;
                    SpPricingMaxReuseCount.EditValue = this.currrentServiceAdo.PRICING_MAX_REUSE_COUNT;
                    TxtSerialNumber.Text = this.currrentServiceAdo.SERIAL_NUMBER;
                    spinEditGiaVeSinh.EditValue = this.currrentServiceAdo.VS_PRICE > 0 ? this.currrentServiceAdo.VS_PRICE : 0;
                    if (this.currrentServiceAdo.IsIdentity && this.IdentityMaterialOption == "1")
                    {
                        SpMaxReuseCount.EditValue = null;
                        lciReUseMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    if (this.IdentityMaterialOption == "2" && (this.currrentServiceAdo.IsIdentity || this.currrentServiceAdo.IsReusable))
                    {
                        TxtSerialNumber.Text = null;
                        SpMaxReuseCount.EditValue = 0;
                        lciReUseMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciSeriNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    if (this.currrentServiceAdo.IsReusable)
                    {
                        SpMaxReuseCount.EditValue = this.currrentServiceAdo.MAX_REUSE_COUNT;
                        lciReUseMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                    //Bat buoc nhap so seri va so lan tai su dung
                    //TODO
                    if ((SpMaxReuseCount.EditValue == null
                        || String.IsNullOrWhiteSpace(SpMaxReuseCount.Text)
                        || SpMaxReuseCount.Value < 1)
                        && (String.IsNullOrWhiteSpace(TxtSerialNumber.Text)))
                    {
                        chkImprice.Enabled = true;
                    }
                    else
                    {
                        chkImprice.Enabled = false;
                        chkImprice.Checked = false;
                    }
                    ValidateControlSeri(this.currrentServiceAdo);
                }
                else
                {
                    TxtSerialNumber.Enabled = false;
                    SpMaxReuseCount.Enabled = false;
                    spinEditGiaVeSinh.Enabled = false;
                    TxtSerialNumber.Text = "";
                    SpMaxReuseCount.EditValue = null;
                    spinEditGiaVeSinh.EditValue = 0;
                    SpPricingMaxReuseCount.EditValue = null;
                    lciGiaVeSinh.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciReUseMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciSeriNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.dxValidationProvider2.SetValidationRule(this.SpMaxReuseCount, null);
                    this.dxValidationProvider2.SetValidationRule(this.SpPricingMaxReuseCount, null);
                    this.dxValidationProvider2.SetValidationRule(this.TxtSerialNumber, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateControlSeri(VHisServiceADO ado)
        {
            try
            {

                lciSeriNumber.AppearanceItemCaption.ForeColor = Color.Black;
                lciReUseMaterial.AppearanceItemCaption.ForeColor = Color.Black;
                this.dxValidationProvider2.SetValidationRule(this.SpMaxReuseCount, null);
                this.dxValidationProvider2.SetValidationRule(this.SpPricingMaxReuseCount, null);
                this.dxValidationProvider2.SetValidationRule(this.TxtSerialNumber, null);

                if (IdentityMaterialOption == "1")
                {
                    lciSeriNumber.AppearanceItemCaption.ForeColor = Color.Maroon;
                    Validation.TxtSeriNumberValidationRule _rule1 = new Validation.TxtSeriNumberValidationRule();
                    _rule1.txtText = this.TxtSerialNumber;
                    _rule1.ErrorText = "Trường dữ liệu bắt buộc";
                    _rule1.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    this.dxValidationProvider2.SetValidationRule(this.TxtSerialNumber, _rule1);
                }
                if (ado.IsReusable)
                {
                    lciReUseMaterial.AppearanceItemCaption.ForeColor = Color.Maroon;
                    Validation.ReUseMaterialValidationRule _rule2 = new Validation.ReUseMaterialValidationRule();
                    _rule2.spinEdit = this.SpMaxReuseCount;
                    _rule2.ErrorText = "Trường dữ liệu bắt buộc";
                    _rule2.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    this.dxValidationProvider2.SetValidationRule(this.SpMaxReuseCount, _rule2);

                    Validation.ReUseMaterialValidationRule _rule3 = new Validation.ReUseMaterialValidationRule();
                    _rule3.spinEdit = this.SpPricingMaxReuseCount;
                    _rule3.ErrorText = "Trường dữ liệu bắt buộc";
                    _rule3.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    this.dxValidationProvider2.SetValidationRule(this.SpPricingMaxReuseCount, _rule3);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReUpdatePrice(bool IsReusable = false)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ReUpdatePrice()-Start");
                foreach (var item in listServicePatyAdo)
                {
                    if (this.currrentServiceAdo.IsReusable && !item.IsNotSell)
                        item.PRICE = spinEditGiaVeSinh.Value + ((this.currrentServiceAdo.IMP_PRICE * (1 + spinImpVatRatio.Value / 100)) / (SpPricingMaxReuseCount.Value == 0 ? 1 : SpPricingMaxReuseCount.Value));
                    item.ExpPriceVat = item.PRICE * (1 + item.VAT_RATIO);
                }
                if (!IsReusable)
                    listServicePatyAdo = listServicePatyAdo != null ? listServicePatyAdo.OrderByDescending(o => o.IsNotSell == false).ToList() : listServicePatyAdo;
                gridControlServicePaty.BeginUpdate();
                UpdateExpPrice();

                long tp_ = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AmountDecimalNumber"));
                long tp = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AutoRoundExpPriceOption"));
                if (listServicePatyAdo != null && listServicePatyAdo.Count > 0)
                {
                    foreach (var item in listServicePatyAdo)
                    {
                        if (tp == 1)
                        {
                            item.PRICE = Math.Round(item.PRICE, (int)tp_);
                        }
                        if (tp == 2)
                        {
                            item.PRICE = RoundDown(item.PRICE, tp_);
                        }
                        if (tp == 3)
                        {
                            item.PRICE = RoundUp(item.PRICE, tp_);
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listServicePatyAdo", listServicePatyAdo));
                gridControlServicePaty.DataSource = listServicePatyAdo;
                gridControlServicePaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Debug("ReUpdatePrice()-Ended");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReUpdateProfit()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ReUpdateProfit()-Start");
                decimal price = (1 + spinImpVatRatio.Value / 100) * spinImpPrice.Value;
                decimal price1 = (1 + spinImpVatRatio.Value / 100) * spinImpPrice1.Value;
                foreach (var item in listServicePatyAdo)
                {
                    if (this.currrentServiceAdo.IsServiceUnitPrimary)
                    {
                        item.VAT_RATIO = (spinImpVatRatio.Value / 100);
                        if (spinImpPrice1.Enabled && spinImpPrice1.Visible)
                        {
                            item.ExpPrice = spinImpPrice1.Value;
                            item.ExpPriceVat = price1;
                            item.PercentProfit = CheckProfitCfg(price1, item.ID);
                        }

                        if (spinImpPrice.Enabled && spinImpPrice.Visible)
                        {
                            item.ExpPrice = spinImpPrice.Value;
                            item.ExpPriceVat = price;
                            item.PercentProfit = CheckProfitCfg(price, item.ID);
                        }

                        item.ExpVatRatio = item.VAT_RATIO * 100;
                        item.PRICE = (1 + item.PercentProfit / 100) * item.ExpPriceVat;
                    }
                    else
                    {
                        item.ExpPriceVat = item.ExpPrice * (1 + item.VAT_RATIO);
                        if (spinImpPrice1.Enabled && spinImpPrice1.Visible)
                        {
                            item.PercentProfit = CheckProfitCfg(price1, item.ID);
                        }

                        if (spinImpPrice.Enabled && spinImpPrice.Visible)
                        {
                            item.PercentProfit = CheckProfitCfg(price, item.ID);
                        }

                        item.PRICE = (1 + item.PercentProfit / 100) * item.ExpPriceVat;
                    }
                }

                listServicePatyAdo = listServicePatyAdo.OrderByDescending(o => o.IsNotSell == false).ToList();
                gridControlServicePaty.BeginUpdate();
                UpdateExpPrice();
                long tp_ = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AmountDecimalNumber"));
                long tp = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AutoRoundExpPriceOption"));
                if (listServicePatyAdo != null && listServicePatyAdo.Count > 0)
                {
                    foreach (var item in listServicePatyAdo)
                    {
                        if (tp == 1)
                        {
                            item.PRICE = Math.Round(item.PRICE, (int)tp_);
                        }
                        if (tp == 2)
                        {
                            item.PRICE = RoundDown(item.PRICE, tp_);
                        }
                        if (tp == 3)
                        {
                            item.PRICE = RoundUp(item.PRICE, tp_);
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listServicePatyAdo", listServicePatyAdo));
                gridControlServicePaty.DataSource = listServicePatyAdo;
                gridControlServicePaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Debug("ReUpdateProfit()-Ended");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBid()
        {
            try
            {
                ValidBidControlMaxlength(txtBidNumOrder, 50);

                ValidBidControlMaxlength(txtkyHieuHoaDon, 20);
                ValidBidControlMaxlength(txtHeinServiceBidMateType, 1500);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBidControlMaxlength(DevExpress.XtraEditors.TextEdit control, int maxlength)
        {
            try
            {
                Validation.BidMaxLengthValidationRule _rule = new Validation.BidMaxLengthValidationRule();
                _rule.txtBid = control;
                _rule.maxlength = maxlength;
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider2.SetValidationRule(control, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBidCheckValidate()
        {
            try
            {
                this.dxValidationProvider2.RemoveControlError(txtBid);//2
                this.dxValidationProvider2.RemoveControlError(txtBidGroupCode);//2
                this.dxValidationProvider2.RemoveControlError(txtBidYear);//20
                this.dxValidationProvider2.RemoveControlError(txtBidNumber);//20

                ValidBidControl(txtBid, lciBid, false, 4, true);
                ValidBidControl(txtBidNumber, lciBidNumber, false, 50);
                ValidBidControl(txtBidGroupCode, layoutControlItem6, false, 4);
                ValidBidControl(txtBidYear, layoutControlItem8, false, 20);

                if (!checkOutBid.Checked)
                {
                    if (IsValidateInfoBid && this.currrentServiceAdo != null)
                    {
                        ValidBidControl(txtBid, lciBid, true, 4, true);
                        ValidBidControl(txtBidNumber, lciBidNumber, true, 50);
                        ValidBidControl(txtBidGroupCode, layoutControlItem6, this.currrentServiceAdo.IsMedicine, 4);
                        ValidBidControl(txtBidYear, layoutControlItem8, !this.currrentServiceAdo.IsMedicine, 20);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBidControl(
            DevExpress.XtraEditors.TextEdit control,
            DevExpress.XtraLayout.LayoutControlItem lci,
            bool isValid,
            int maxLength, bool isBidPackage = false)
        {
            try
            {
                if (isValid)
                    lci.AppearanceItemCaption.ForeColor = Color.Maroon;
                else
                    lci.AppearanceItemCaption.ForeColor = Color.Black;

                Validation.BidValidationRule _rule = new Validation.BidValidationRule();
                _rule.txtBid = control;
                _rule.IsValidate = isValid;
                _rule.maxLength = maxLength;
                _rule.isBidPackage = isBidPackage;
                _rule.ErrorText = "Trường dữ liệu bắt buộc";
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider2.SetValidationRule(control, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SoloValidBidControl(DevExpress.XtraEditors.TextEdit control, DevExpress.XtraLayout.LayoutControlItem lci, bool isValid)
        {
            try
            {
                if (isValid)
                    lci.AppearanceItemCaption.ForeColor = Color.Maroon;
                else
                    lci.AppearanceItemCaption.ForeColor = Color.Black;

                Validation.SoLoValidationRule _rule = new Validation.SoLoValidationRule();
                _rule.txtBid = control;
                _rule.IsValidate = isValid;
                _rule.ErrorText = "Trường dữ liệu bắt buộc";
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider2.SetValidationRule(control, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBid_EditValueChangedUCMedicine(long? bidId)
        {
            try
            {
                WaitingManager.Show();
                IsLoadGridMedicine = true;
                IsChangeBid = true;
                IsHasValueChooice = false;
                SetEnableControl(false);
                if (bidId == null)
                {
                    this.currentBid = null;
                    Contract_RowClick();
                    if (IsLoadGridMedicine)
                        SetDataSourceGridControlMediMateMedicine();
                    isEnableGoiThau = false;
                    cboGoiThau.EditValue = null;
                }
                else
                {
                    this.currentBid = null;
                    if (bidId > 0)
                    {
                        txtBidNumOrder.Enabled = false;
                        txtBidYear.Enabled = false;
                        txtBidNumber.Enabled = false;
                        txtBidGroupCode.Enabled = false;
                        txtBid.Enabled = false;
                        txtBidNumOrder.Text = "";
                        txtBidYear.Text = "";
                        txtBidNumber.Text = "";
                        txtBidGroupCode.Text = "";
                        txtBid.Text = "";
                        this.currentBid = listBids.FirstOrDefault(o => o.ID == bidId);
                        Contract_RowClick();
                        isEnableGoiThau = true;
                        cboGoiThau.Enabled = false;
                        cboInformationBid.EditValue = null;
                        InitComboGoiThau(listBids, (long)bidId);
                        IsHasValueChooice = true;
                        if (IsNCC)
                        {
                            SetEnableControl(this.IsAllowedToEnableMedicinesInformation);
                        }
                    }

                    LoadDataByBidMedicine();
                    if (IsLoadGridMedicine)
                        SetDataSourceGridControlMediMateMedicine();
                }

                IsChangeBid = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSourceGridControlMediMateMedicine()
        {
            try
            {
                if (medistock.IS_BUSINESS == 1)
                {
                    if (medistock.IS_NEW_MEDICINE == 1 && medistock.IS_TRADITIONAL_MEDICINE == 1)
                    {
                        listMedicineTypeTemp = listMedicineTypeTemp.Where(o => o.IS_BUSINESS == 1).ToList();
                    }
                    else if (medistock.IS_NEW_MEDICINE == 1)
                    {
                        listMedicineTypeTemp = listMedicineTypeTemp.Where(o =>
                            o.IS_BUSINESS == 1
                            && (o.MEDICINE_LINE_ID == null
                            || o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__TTD)
                            ).ToList();
                    }
                    else if (medistock.IS_TRADITIONAL_MEDICINE == 1)
                    {
                        listMedicineTypeTemp = listMedicineTypeTemp.Where(o =>
                            o.IS_BUSINESS == 1
                            && (o.MEDICINE_LINE_ID == null
                            || o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__CP_YHCT
                            || o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT)
                            ).ToList();
                    }
                    else
                        listMedicineTypeTemp = listMedicineTypeTemp.Where(o => o.IS_BUSINESS == 1).ToList();
                    if (listMedicineTypeTemp != null && listMedicineTypeTemp.Count > 0)
                    {
                        if (medistock.IS_SHOW_DRUG_STORE == 1)
                        {
                            listMedicineTypeTemp = listMedicineTypeTemp.Where(o => o.IS_DRUG_STORE == 1).ToList();
                        }
                        else
                        {
                            listMedicineTypeTemp = listMedicineTypeTemp.Where(o => o.IS_DRUG_STORE == null).ToList();
                        }
                    }
                }
                else
                {
                    if (medistock.IS_NEW_MEDICINE == 1 && medistock.IS_TRADITIONAL_MEDICINE == 1)
                    {
                        listMedicineTypeTemp = listMedicineTypeTemp.Where(o => o.IS_BUSINESS != 1).ToList();
                    }
                    else if (medistock.IS_NEW_MEDICINE == 1)
                    {
                        listMedicineTypeTemp = listMedicineTypeTemp.Where(o =>
                            o.IS_BUSINESS != 1
                            && (o.MEDICINE_LINE_ID == null
                            || o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__TTD)
                            ).ToList();
                    }
                    else if (medistock.IS_TRADITIONAL_MEDICINE == 1)
                    {
                        listMedicineTypeTemp = listMedicineTypeTemp.Where(o =>
                            o.IS_BUSINESS != 1
                            && (o.MEDICINE_LINE_ID == null
                            || o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__CP_YHCT
                            || o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT)
                            ).ToList();
                    }
                    else
                        listMedicineTypeTemp = listMedicineTypeTemp.Where(o => o.IS_BUSINESS != 1).ToList();
                }

                if (this.currentBid != null)
                {
                    if (dicBidMedicine.Count > 0)
                    {
                        var listId = dicBidMedicine.Select(s => s.Key).ToList();
                        listMedicineType = new List<MedicineTypeADO>();
                        foreach (var item in dicBidMedicine)
                        {
                            if (item.Value == null)
                            {
                                continue;
                            }
                            //var strGroupKey = Base.StaticMethod.GetTypeKey(11389, item.Value.BID_GROUP_CODE);
                            //var medicineType = listMedicineTypeTemp.Where(o => o.ID == item.Value.MEDICINE_TYPE_ID).FirstOrDefault();
                            var medicineType = listMedicineTypeTemp.FirstOrDefault(o => Base.StaticMethod.GetTypeKey(o.ID, item.Value.BID_GROUP_CODE) == item.Key);
                            if (medicineType == null)
                                continue;
                            MedicineTypeADO medicineTypeADO = new MedicineTypeADO(medicineType);
                            medicineTypeADO.AMOUNT_IN_BID = item.Value.AMOUNT;
                            medicineTypeADO.IMP_PRICE_IN_BID = item.Value.IMP_PRICE;
                            medicineTypeADO.IMP_VAT_RATIO_IN_BID = (item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null);
                            medicineTypeADO.BidGroupCode = item.Value.BID_GROUP_CODE;
                            medicineTypeADO.TDL_BID_GROUP_CODE = item.Value.BID_GROUP_CODE;
                            medicineTypeADO.KeyField = Base.StaticMethod.GetTypeKey(item.Value.MEDICINE_TYPE_ID, item.Value.BID_GROUP_CODE);
                            medicineTypeADO.ADJUST_AMOUNT = item.Value.ADJUST_AMOUNT;
                            listMedicineType.Add(medicineTypeADO);
                        }
                    }
                    else
                    {
                        listMedicineType = new List<MedicineTypeADO>();
                    }
                }


                //Inventec.Common.Logging.LogSystem.Debug("dicBidMedicine_______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicBidMedicine), dicBidMedicine));


                //Inventec.Common.Logging.LogSystem.Debug("listMedicineTypeTemp_______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listMedicineTypeTemp.Select(o => o.ID)), listMedicineTypeTemp.Select(o => o.ID)));

                //Inventec.Common.Logging.LogSystem.Debug("dicContractMety_______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicContractMety), dicContractMety));

                if (this.currentContract != null)
                {
                    if (dicContractMety.Count > 0)
                    {
                        var listId = dicContractMety.Select(s => s.Key).ToList();
                        listMedicineType = new List<MedicineTypeADO>();
                        foreach (var item in dicContractMety)
                        {
                            var medicineType = listMedicineTypeTemp.FirstOrDefault(o => Base.StaticMethod.GetTypeKey(o.ID, item.Value.BID_GROUP_CODE) == item.Key.Substring(0, item.Key.LastIndexOf("_")));
                            if (medicineType == null)
                                continue;
                            MedicineTypeADO medicineTypeADO = new MedicineTypeADO(medicineType);
                            medicineTypeADO.AMOUNT_IN_CONTRACT = item.Value.AMOUNT;
                            medicineTypeADO.IMP_PRICE_IN_CONTRACT = item.Value.CONTRACT_PRICE;
                            medicineTypeADO.IMP_VAT_RATIO_IN_CONTRACT = (item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null);
                            medicineTypeADO.MEDI_CONTRACT_METY_ID = item.Value.ID;
                            medicineTypeADO.BidGroupCode = item.Value.BID_GROUP_CODE;
                            medicineTypeADO.TDL_BID_GROUP_CODE = item.Value.BID_GROUP_CODE;
                            medicineTypeADO.DISCOUNT_FROM_DATE = item.Value.DISCOUNT_FROM_DATE;
                            medicineTypeADO.DISCOUNT_TO_DATE = item.Value.DISCOUNT_TO_DATE;
                            medicineTypeADO.DISCOUNT_FROM_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_FROM_DATE ?? 0);
                            medicineTypeADO.DISCOUNT_TO_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_TO_DATE ?? 0);
                            listMedicineType.Add(medicineTypeADO);
                        }
                    }
                    else
                    {
                        listMedicineType = new List<MedicineTypeADO>();
                    }
                }

                if (this.currentBid == null && this.currentContract == null)
                {
                    listMedicineType = (from r in listMedicineTypeTemp select new MedicineTypeADO(r)).ToList();
                }

                if (listMedicineType != null && listMedicineType.Count > 0)
                {
                    listMedicineType.ForEach(o =>
                    {
                        if (o.IMP_UNIT_ID.HasValue) o.SERVICE_UNIT_NAME = o.IMP_UNIT_NAME;
                    });
                }

                this.medicineProcessor.Reload(this.ucMedicineTypeTree, listMedicineType);
                if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    if (this.currentBid != null)
                    {
                        var lstSupplierIds = this.currentBid.SUPPLIER_IDS.Split(',').ToList();
                        if (lstSupplierIds.Count > 0)
                        {
                            var lst = listSupplier.Where(o => lstSupplierIds.Exists(p => p.Equals(o.ID.ToString()))).ToList();
                            LoadDataToComboSupplier(lst);
                        }
                        else
                        {
                            LoadDataToComboSupplier(null);
                        }
                    }
                    else
                    {
                        LoadDataToComboSupplier(listSupplier);
                    }
                }
                IsLoadGridMedicine = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Contract_RowClick()
        {
            try
            {
                if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && dtDocumentDate.EditValue != null) //&& txtNhaCC.EditValue != null
                {
                    WaitingManager.Show();

                    var DocumentDate = Convert.ToInt64(dtDocumentDate.DateTime.ToString("yyyyMMddHHmmss"));

                    Inventec.Common.Logging.LogSystem.Debug("Contract_RowClick__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listContracts), listContracts));
                    ContractOfBidAndSup = new List<HIS_MEDICAL_CONTRACT>();
                    if (this.listContracts != null && this.listContracts.Count > 0)
                    {
                        ContractOfBidAndSup.AddRange(this.listContracts);
                    }

                    long? BidId = null;

                    if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                    {
                        BidId = medicineProcessor.GetBid(this.ucMedicineTypeTree);
                    }
                    else if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                    {
                        BidId = materialProcessor.GetBid(this.ucMaterialTypeTree);
                    }

                    if (BidId != null)
                    {
                        ContractOfBidAndSup = ContractOfBidAndSup.Where(o => o.BID_ID == BidId.Value).ToList();
                    }

                    Inventec.Common.Logging.LogSystem.Debug("Contract_RowClick__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentSupplierForEdit), currentSupplierForEdit));
                    if (this.currentSupplierForEdit != null)
                    {
                        if (checkOutBid.Checked)
                        {
                            ContractOfBidAndSup = ContractOfBidAndSup.Where(o => o.SUPPLIER_ID == this.currentSupplierForEdit.ID && (o.VALID_TO_DATE == null || o.VALID_TO_DATE >= DocumentDate)
                               && (o.VALID_FROM_DATE == null || o.VALID_FROM_DATE <= DocumentDate) && o.BID_ID == null).OrderBy(o => o.VALID_TO_DATE).ToList();
                        }
                        else
                        {
                            ContractOfBidAndSup = ContractOfBidAndSup.Where(o => o.SUPPLIER_ID == this.currentSupplierForEdit.ID && (o.VALID_TO_DATE == null || o.VALID_TO_DATE >= DocumentDate) && (o.VALID_FROM_DATE == null || o.VALID_FROM_DATE <= DocumentDate) && o.BID_ID != null).OrderBy(o => o.VALID_TO_DATE).ToList();
                        }
                        medicineProcessor.EnableContract(this.ucMedicineTypeTree, true);
                        materialProcessor.EnableContract(this.ucMaterialTypeTree, true);
                        medicineProcessor.ReloadContract(this.ucMedicineTypeTree, ContractOfBidAndSup);
                        materialProcessor.ReloadContract(this.ucMaterialTypeTree, ContractOfBidAndSup);

                    }
                    else
                    {
                        medicineProcessor.ReloadContract(this.ucMedicineTypeTree, null);
                        materialProcessor.ReloadContract(this.ucMaterialTypeTree, null);
                        medicineProcessor.EnableContract(this.ucMedicineTypeTree, false);
                        materialProcessor.EnableContract(this.ucMaterialTypeTree, false);
                        materialProcessor.SetEditValueContract(this.ucMaterialTypeTree, null);
                        medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                        this.layoutControlItem7.Text = "Giá trong thầu";
                    }
                    if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                    {
                        if (currentContract != null && !ContractOfBidAndSup.Exists(o => o.ID == currentContract.ID))
                        {
                            this.currentContract = null;
                            medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                        }
                        if (medicineProcessor.GetContract(this.ucMedicineTypeTree) != null || this.currentContract != null)
                        {
                            if (oldContract != currentContract || IsChangeTabPage)
                                LoadDataByContractMety();
                            SetDataSourceGridControlMediMateMedicine();
                        }

                    }
                    else if ((xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial))
                    {
                        if (currentContract != null && !ContractOfBidAndSup.Exists(o => o.ID == currentContract.ID))
                        {
                            this.currentContract = null;
                            materialProcessor.SetEditValueContract(this.ucMaterialTypeTree, null);
                        }
                        if (materialProcessor.GetContract(this.ucMaterialTypeTree) != null || this.currentContract != null)
                        {
                            if (oldContract != currentContract || IsChangeTabPage)
                                LoadDataByContractMaty();
                            SetDataSourceGridControlMediMateMaterial();
                        }

                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataByBidMedicine()
        {
            try
            {
                dicBidMedicine.Clear();
                if (this.currentBid != null)
                {
                    HisBidMedicineTypeViewFilter mediFilter = new HisBidMedicineTypeViewFilter();
                    mediFilter.BID_ID = this.currentBid.ID;
                    mediFilter.IS_ACTIVE = 1;
                    listBidMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MEDICINE_TYPE>>(HisRequestUriStore.HIS_BID_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);
                    List<long> listSupplierIds = new List<long>();
                    if (listBidMedicine != null && listBidMedicine.Count > 0)
                    {
                        if (currentSupplier != null && currentSupplier.ID > 0)
                        {
                            listBidMedicine = listBidMedicine.Where(o => o.SUPPLIER_ID == currentSupplier.ID).ToList();
                        }

                        foreach (var item in listBidMedicine)
                        {
                            dicBidMedicine[Base.StaticMethod.GetTypeKey(item.MEDICINE_TYPE_ID, item.BID_GROUP_CODE)] = item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboContract_EditValueChangedUCMedicine(long? ContractId)
        {
            try
            {
                IsHasValueChooice = false;
                IsLoadGridMedicine = true;
                IsLoadContract = true;
                SetEnableControl(false);
                WaitingManager.Show();
                this.oldContract = this.currentContract;
                if (ContractId == null)
                {
                    this.layoutControlItem7.Text = "Giá trong thầu";

                    this.currentContract = null;
                    if (!IsChangeBid)
                        SetDataSourceGridControlMediMateMedicine();

                }
                else
                {
                    this.layoutControlItem7.Text = "Giá hợp đồng";

                    this.currentContract = null;
                    this.currentContract = listContracts.FirstOrDefault(o => o.ID == ContractId);

                    this.medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, this.currentContract.ID);

                    if (this.currentContract.BID_ID.HasValue)
                    {
                        this.medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, this.currentContract.BID_ID.Value);
                    }
                    IsHasValueChooice = true;
                    if (IsNCC)
                    {
                        SetEnableControl(this.IsAllowedToEnableMedicinesInformation);
                    }
                    if (IsLoadContract && oldContract != currentContract)
                        LoadDataByContractMety();
                    if (IsLoadGridMedicine && !IsChangeBid)
                        SetDataSourceGridControlMediMateMedicine();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataByContractMety()
        {
            try
            {
                dicContractMety.Clear();

                if (this.currentContract != null)
                {
                    HisMediContractMetyViewFilter metyFilter = new HisMediContractMetyViewFilter();
                    metyFilter.MEDICAL_CONTRACT_ID = this.currentContract.ID;

                    //if (this.dicBidMedicine != null && this.dicBidMedicine.Count > 0)
                    //{
                    //    metyFilter.BID_MEDICINE_TYPE_IDs = this.dicBidMedicine.Select(o => o.Value.ID).ToList();
                    //}

                    var lstContractMety = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_CONTRACT_METY>>("api/HisMediContractMety/GetView", ApiConsumers.MosConsumer, metyFilter, null);
                    if (lstContractMety != null && lstContractMety.Count > 0)
                    {
                        if (currentBid != null)
                        {
                            lstContractMety = lstContractMety.Where(o => o.BID_ID == currentBid.ID).ToList();
                        }
                        foreach (var item in lstContractMety)
                        {
                            dicContractMety[Base.StaticMethod.GetTypeKey(item.MEDICINE_TYPE_ID, item.BID_GROUP_CODE, item.ID)] = item;

                        }
                    }
                }
                IsLoadContract = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetBids(ref List<V_HIS_BID_1> bids)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBidViewFilter filter = new HisBidViewFilter();
                filter.IS_ACTIVE = 1;
                filter.VALID_TIME = Inventec.Common.DateTime.Get.Now();
                bids = new BackendAdapter(param).Get<List<V_HIS_BID_1>>("api/HisBid/GetView1", ApiConsumers.MosConsumer, filter, param);
                if (IsShowingApprovalBid)
                {
                    bids = bids.Where(o => o.APPROVAL_TIME != null).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetContracts(ref List<HIS_MEDICAL_CONTRACT> listContracts)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicalContractFilter filter = new HisMedicalContractFilter();
                filter.IS_ACTIVE = 1;
                listContracts = new BackendAdapter(param).Get<List<HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBid_EditValueChangedUCMaterial(long? bidId)
        {
            try
            {

                SetEnableControl(false);
                IsLoadGridMaterial = true;
                IsChangeBid = true;
                IsHasValueChooice = false;
                WaitingManager.Show();
                if (bidId == null)
                {
                    this.currentBid = null;
                    Contract_RowClick();
                    if (IsLoadGridMaterial)
                        SetDataSourceGridControlMediMateMaterial();

                    isEnableGoiThau = false;
                    cboGoiThau.EditValue = null;
                }
                else
                {
                    WaitingManager.Show();
                    this.currentBid = null;
                    if (bidId > 0)
                    {
                        txtBidNumOrder.Enabled = false;
                        txtBidYear.Enabled = false;
                        txtBidNumber.Enabled = false;
                        txtBidGroupCode.Enabled = false;
                        txtBid.Enabled = false;
                        txtBidNumOrder.Text = "";
                        txtBidYear.Text = "";
                        txtBidNumber.Text = "";
                        txtBidGroupCode.Text = "";
                        txtBid.Text = "";
                        this.currentBid = listBids.FirstOrDefault(o => o.ID == bidId);
                        Contract_RowClick();
                        isEnableGoiThau = true;
                        cboGoiThau.Enabled = false;
                        cboInformationBid.Enabled = true;
                        cboInformationBid.EditValue = null;
                        InitComboGoiThau(listBids, (long)bidId);
                        IsHasValueChooice = true;
                        if (IsNCC)
                        {
                            SetEnableControl(this.IsAllowedToEnableMedicinesInformation);
                        }
                    }

                    LoadDataByBidMaterial();
                    if (IsLoadGridMaterial)
                        SetDataSourceGridControlMediMateMaterial();
                }

                IsChangeBid = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboContract_EditValueChangedUCMaterial(long? ContractId)
        {
            try
            {
                SetEnableControl(false);
                WaitingManager.Show();
                this.IsLoadContract = true;
                this.IsLoadGridMaterial = true;
                this.oldContract = currentContract;
                if (ContractId == null)
                {
                    this.currentContract = null;
                    this.layoutControlItem7.Text = "Giá trong thầu";
                    if (!IsChangeBid)
                        SetDataSourceGridControlMediMateMaterial();
                }
                else
                {
                    this.currentContract = null;
                    if (ContractId > 0)
                    {
                        this.currentContract = listContracts.FirstOrDefault(o => o.ID == ContractId);

                        if (this.currentContract.ID != null)
                        {
                            this.layoutControlItem7.Text = "Giá hợp đồng";
                        }

                        this.materialProcessor.SetEditValueContract(this.ucMaterialTypeTree, this.currentContract.ID);

                        if (this.currentContract.BID_ID.HasValue)
                        {
                            this.materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, this.currentContract.BID_ID);
                        }
                        IsHasValueChooice = true;
                        if (IsNCC)
                        {
                            SetEnableControl(this.IsAllowedToEnableMedicinesInformation);
                        }

                    }
                    if (IsLoadContract && oldContract != currentContract)
                        LoadDataByContractMaty();
                    if (IsLoadGridMaterial && !IsChangeBid)
                        SetDataSourceGridControlMediMateMaterial();
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSourceGridControlMediMateMaterial()
        {
            try
            {
                if (medistock.IS_BUSINESS == 1)
                {
                    listMaterialTypeTemp = listMaterialTypeTemp.Where(o => o.IS_BUSINESS == 1).ToList();
                    if (listMaterialTypeTemp != null && listMaterialTypeTemp.Count > 0)
                    {
                        if (medistock.IS_SHOW_DRUG_STORE == 1)
                        {
                            listMaterialTypeTemp = listMaterialTypeTemp.Where(o => o.IS_DRUG_STORE == 1).ToList();
                        }
                        else
                        {
                            listMaterialTypeTemp = listMaterialTypeTemp.Where(o => o.IS_DRUG_STORE == null).ToList();
                        }
                    }
                }
                else
                {
                    listMaterialTypeTemp = listMaterialTypeTemp.Where(o => o.IS_BUSINESS != 1).ToList();
                }


                if (this.currentBid != null)
                {
                    if (dicBidMaterial.Count > 0)
                    {
                        //var listId = dicBidMaterial.Select(s => s.Key).ToList();
                        //listMaterialTypeTemp = listMaterialTypeTemp.Where(s => listId.Contains(s.ID)).ToList();

                        listMaterialType = new List<MaterialTypeADO>();

                        //var listId = dicBidMaterial.Select(s => s.Key).ToList();
                        //listMaterialTypeTemp = listMaterialTypeTemp.Where(s => listId.Contains(s.ID)).ToList();
                        foreach (var item in dicBidMaterial)
                        {
                            if (item.Value == null)
                            {
                                continue;
                            }

                            var materialType = listMaterialTypeTemp.FirstOrDefault(o => Base.StaticMethod.GetTypeKey(o.ID, item.Value.BID_GROUP_CODE) == item.Key);
                            if (materialType == null)
                                continue;
                            MaterialTypeADO materialTypeADO = new MaterialTypeADO(materialType);
                            materialTypeADO.AMOUNT_IN_BID = item.Value.AMOUNT;
                            materialTypeADO.IMP_PRICE_IN_BID = item.Value.IMP_PRICE;
                            materialTypeADO.IMP_VAT_RATIO_IN_BID = item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null;
                            materialTypeADO.BidGroupCode = item.Value.BID_GROUP_CODE;
                            materialTypeADO.KeyField = Base.StaticMethod.GetTypeKey(materialType.ID, item.Value.BID_GROUP_CODE);
                            materialTypeADO.ADJUST_AMOUNT = item.Value.ADJUST_AMOUNT;
                            listMaterialType.Add(materialTypeADO);
                        }
                    }
                    else
                    {
                        listMaterialType = new List<MaterialTypeADO>();
                    }
                }

                if (this.currentContract != null)
                {
                    if (dicContractMaty.Count > 0)
                    {
                        listMaterialType = new List<MaterialTypeADO>();
                        foreach (var item in dicContractMaty)
                        {
                            var materialType = listMaterialTypeTemp.FirstOrDefault(o => Base.StaticMethod.GetTypeKey(o.ID, item.Value.BID_GROUP_CODE) == item.Key.Substring(0, item.Key.LastIndexOf("_")));
                            if (materialType == null)
                                continue;
                            MaterialTypeADO materialTypeADO = new MaterialTypeADO(materialType);
                            materialTypeADO.AMOUNT_IN_CONTRACT = item.Value.AMOUNT;
                            materialTypeADO.IMP_PRICE_IN_CONTRACT = item.Value.CONTRACT_PRICE;
                            materialTypeADO.IMP_VAT_RATIO_IN_CONTRACT = item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null;
                            materialTypeADO.MEDI_CONTRACT_MATY_ID = item.Value.ID;
                            materialTypeADO.BidGroupCode = item.Value.BID_GROUP_CODE;
                            materialTypeADO.DISCOUNT_FROM_DATE = item.Value.DISCOUNT_FROM_DATE;
                            materialTypeADO.DISCOUNT_TO_DATE = item.Value.DISCOUNT_TO_DATE;
                            materialTypeADO.DISCOUNT_FROM_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_FROM_DATE ?? 0);
                            materialTypeADO.DISCOUNT_TO_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_TO_DATE ?? 0);
                            listMaterialType.Add(materialTypeADO);
                        }
                    }
                    else
                    {
                        listMaterialType = new List<MaterialTypeADO>();
                    }
                }
                if (this.currentBid == null && this.currentContract == null)
                {
                    listMaterialType = (from r in listMaterialTypeTemp select new MaterialTypeADO(r)).ToList();
                }

                if (listMaterialType != null && listMaterialType.Count > 0)
                {
                    listMaterialType.ForEach(o =>
                    {
                        if (o.IMP_UNIT_ID.HasValue) o.SERVICE_UNIT_NAME = o.IMP_UNIT_NAME;
                    });
                }
                Inventec.Common.Logging.LogSystem.Info("listMaterialType count: " + listMaterialType.Count());
                this.materialProcessor.Reload(this.ucMaterialTypeTree, listMaterialType);
                if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    if (this.currentBid != null && this.currentBid.SUPPLIER_IDS != null)
                    {
                        var lstSupplierIds = this.currentBid.SUPPLIER_IDS.Split(',').ToList();
                        if (lstSupplierIds.Count > 0)
                        {
                            var lst = listSupplier.Where(o => lstSupplierIds.Exists(p => p.Equals(o.ID.ToString()))).ToList();
                            LoadDataToComboSupplier(lst);
                        }
                        else
                        {
                            LoadDataToComboSupplier(null);
                        }

                    }
                    else
                    {
                        LoadDataToComboSupplier(listSupplier);
                    }
                }
                IsLoadGridMaterial = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataByContractMaty()
        {
            try
            {
                dicContractMaty.Clear();

                if (this.currentContract != null)
                {
                    HisMediContractMatyViewFilter matyFilter = new HisMediContractMatyViewFilter();
                    matyFilter.MEDICAL_CONTRACT_ID = this.currentContract.ID;
                    //if (this.dicBidMaterial != null && this.dicBidMaterial.Count > 0)
                    //{
                    //    matyFilter.BID_MATERIAL_TYPE_IDs = this.dicBidMaterial.Select(o => o.Value.ID).ToList();
                    //}
                    var lstContractMaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_CONTRACT_MATY>>("api/HisMediContractMaty/GetView", ApiConsumers.MosConsumer, matyFilter, null);
                    if (lstContractMaty != null && lstContractMaty.Count > 0)
                    {
                        if (currentBid != null)
                        {
                            lstContractMaty = lstContractMaty.Where(o => o.BID_ID == currentBid.ID).ToList();
                        }
                        foreach (var item in lstContractMaty)
                        {
                            dicContractMaty[Base.StaticMethod.GetTypeKey(item.MATERIAL_TYPE_ID, item.BID_GROUP_CODE, item.ID)] = item;
                        }
                    }
                }
                IsLoadContract = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataByBidMaterial()
        {
            try
            {
                dicBidMaterial.Clear();
                if (this.currentBid != null)
                {
                    HisBidMaterialTypeViewFilter mateFilter = new HisBidMaterialTypeViewFilter();
                    mateFilter.BID_ID = this.currentBid.ID;
                    mateFilter.IS_ACTIVE = 1;
                    var listBidMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MATERIAL_TYPE>>(HisRequestUriStore.HIS_BID_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);
                    if (listBidMaterial != null && listBidMaterial.Count > 0)
                    {
                        if (currentSupplier != null && currentSupplier.ID > 0)
                        {
                            listBidMaterial = listBidMaterial.Where(o => o.SUPPLIER_ID == currentSupplier.ID).ToList();
                        }

                        foreach (var item in listBidMaterial)
                        {
                            if (item.MATERIAL_TYPE_ID.HasValue)
                            {
                                dicBidMaterial[Base.StaticMethod.GetTypeKey(item.MATERIAL_TYPE_ID ?? 0, item.BID_GROUP_CODE)] = item;
                            }
                            else if (item.MATERIAL_TYPE_MAP_ID.HasValue)
                            {
                                var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.MATERIAL_TYPE_MAP_ID == item.MATERIAL_TYPE_MAP_ID).ToList();
                                if (materialType != null && materialType.Count > 0)
                                {
                                    foreach (var maty in materialType)
                                    {
                                        dicBidMaterial[Base.StaticMethod.GetTypeKey(maty.ID, item.BID_GROUP_CODE)] = item;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
