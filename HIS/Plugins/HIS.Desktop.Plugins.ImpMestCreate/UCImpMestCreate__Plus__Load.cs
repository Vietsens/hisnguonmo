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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using HIS.Desktop.Plugins.ImpMestCreate.Config;
using HIS.Desktop.Plugins.ImpMestCreate.Validation;
using HIS.Desktop.Utility;
using HIS.UC.MaterialType.ADO;
using HIS.UC.MedicineType.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    public partial class UCImpMestCreate : UserControlBase
    {
        private void LoadKeyUCLanguage()
        {
            try
            {
                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //Button
                this.btnAdd1.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__BTN_ADD", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__BTN_NEW", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.btnImportExcel.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__BTN_EXPORT_EXCEL", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                //  this.btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__BTN_PRINT", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__BTN_SAVE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.btnSaveDraft.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__BTN_SAVE_DRAFT", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);

                //Layout
                this.layoutAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_AMOUNT", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                //this.layoutBid.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_BID", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                //this.layoutBidInfo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_BID_INFO", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutBidNumber.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_BID_NUM_ORDER", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutCanImpAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_CAN_IMP_AMOUNT", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutDeliverer.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_DELIERVER", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_DESCRIPTION", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutDescriptionPaty.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_DESCRIPTION_PATY", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);

                this.layoutDocumentPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_DOCUMENT_PRICE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutExpiredDate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_EXPIRED_DATE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutImpMestType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_IMP_MEST_TYPE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutImpPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_IMP_PRICE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutImpSource.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_IMP_SOURCE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutImpVatRatio.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_IMP_VAT_RATIO", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutMediStock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_MEDI_STOCK", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutPackageNumber.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutSupplier.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_SUPPLIER", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutImpPriceVat.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_IMP_PRICE_VAT", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutTotalFeePrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_TOTAL_FEE_PRICE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutTotalPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_TOTAL_PRICE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.layoutTotalVatPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_TOTAL_VAT_PRICE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);

                ////Check
                //this.checkOutBid.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__LAYOUT_OUT_BID", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);

                //GridControl ImpMestDetail
                this.gridColumn_ImpMestDetail_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_IMP_MEST_DETAIL__COLUMN_IMP_AMOUNT", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.gridColumn_ImpMestDetail_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_IMP_MEST_DETAIL__COLUMN_EXPIRED_DATE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.gridColumn_ImpMestDetail_ImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_IMP_MEST_DETAIL__COLUMN_IMP_VAT_RATIO", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.gridColumn_ImpMestDetail_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_IMP_MEST_DETAIL__COLUMN_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.gridColumn_ImpMestDetail_Price.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_IMP_MEST_DETAIL__COLUMN_IMP_PRICE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.gridColumn_ImpMestDetail_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_IMP_MEST_DETAIL__COLUMN_STT", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.gridColumn_ImpMestDetail_TypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_IMP_MEST_DETAIL__COLUMN_MEDI_MATE_TYPE_NAME", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);

                //GridControl ServicePaty
                this.gridColumn_ServicePaty_ExpPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_SERVICE_PATY__COLUMN_EXP_PRICE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.gridColumn_ServicePaty_NotSell.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_SERVICE_PATY__COLUMN_NOT_SELL", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.gridColumn_ServicePaty_PatientTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_SERVICE_PATY__COLUMN_PATIENT_TYPE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.gridColumn_ServicePaty_VatRatio.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_SERVICE_PATY__COLUMN_EXP_VAT_RATIO", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.gridColumn_ServicePaty_PriceAndVat.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__GRID_SERVICE_PATY__COLUMN_EXP_PRICE_VAT", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);

                //Xtra Tab
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__XTRA_TAB_MATERIAL", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__XTRA_TAB_MEDICINE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);

                //Repository Button
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__REPOSITORY_BTN_DELETE", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
                this.repositoryItemBtnEdit.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__REPOSITORY_BTN_EDIT", Base.ResourceLangManager.LanguageUCImpMestCreate, cul);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFormat()
        {
            try
            {
                this.spinEditTTChuaVAT.Properties.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinEditTTChuaVAT.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.spinEditTTCoVAT.Properties.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinEditTTCoVAT.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.spinImpPrice.Properties.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinImpPrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.spinImpPriceVAT.Properties.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinImpPriceVAT.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.spinEditGiaTrongThau.Properties.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinEditGiaTrongThau.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.spinEditGiaNhapLanTruoc.Properties.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinEditGiaNhapLanTruoc.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.spinEditGiaVeSinh.Properties.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinEditGiaVeSinh.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.spinDocumentPrice.Properties.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinDocumentPrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

                long tp = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AutoRoundExpPriceOption"));
                long tp_ = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AmountDecimalNumber"));
                if (tp == 1 || tp == 2 || tp == 3)
                {
                    // "#,##0.0000000."
                    if (tp_ > 0)
                    {
                        this.gridColumn_ServicePaty_ExpPrice.DisplayFormat.FormatString = "#,########0." + AddStringByConfig((int)tp_);
                    }
                    else
                    {
                        this.gridColumn_ServicePaty_ExpPrice.DisplayFormat.FormatString = "#,########0.";
                    }
                }
                else
                {
                    this.gridColumn_ServicePaty_ExpPrice.DisplayFormat.FormatString = "#,########0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                }
                this.gridColumn_ServicePaty_ExpPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.gridColumn_PercentProfit.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.gridColumn_PercentProfit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.gridColumn_ServicePaty_PriceAndVat.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.gridColumn_ServicePaty_PriceAndVat.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.gridColumn_ImpMestDetail_Amount.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.gridColumn_ImpMestDetail_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.gridColumn_ImpMestDetail_Price.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.gridColumn_ImpMestDetail_Price.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.gridColumn_ImpMestDetail_ImpVatRatio.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.gridColumn_ImpMestDetail_ImpVatRatio.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkWarningOldBid.Name)
                        {
                            chkWarningOldBid.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkNoProfitBhyt.Name)
                        {
                            chkNoProfitBhyt.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void InitControlStateCheckInOut()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == checkInOutBid.Name)
                        {
                            checkInOutBid.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }


        private void ValidControl()
        {
            try
            {
                ValidControlImpMestType();
                ValidControlMediStock();
                ValidControlSupplier();
                ValidControlDocumentDate();
                ValidControlImpAmount();
                ValidControlImPrice();
                ValidControlImpVatRatio();
                ValidControlExpiredDate();
                ValidControlMaxLength(txtDescription, 500);
                ValidControlMaxLength(txtTaiKhoanCo, 50);
                ValidControlMaxLength(txtTaiKhoanNo, 50);
                ValidControlMaxLength(txtkyHieuHoaDon, 20);
                ValidBidControlMaxlength(txtHeinServiceBidMateType, 1500);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpMestType()
        {
            try
            {
                ImpMestTypeValidationRule impMestTypeRule = new ImpMestTypeValidationRule();
                impMestTypeRule.cboImpMestType = cboImpMestType;
                dxValidationProvider1.SetValidationRule(cboImpMestType, impMestTypeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlMediStock()
        {
            try
            {
                MediStockValidationRule mediStockRule = new MediStockValidationRule();
                mediStockRule.cboMediStock = cboMediStock;
                dxValidationProvider1.SetValidationRule(cboMediStock, mediStockRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlSupplier()
        {
            try
            {
                SupplierValidationRule supplierRule = new SupplierValidationRule();
                supplierRule.cboImpMestType = cboImpMestType;
                supplierRule.cboSupplier = txtNhaCC;
                dxValidationProvider1.SetValidationRule(txtNhaCC, supplierRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDocumentDate()
        {
            try
            {
                bool keyCheck = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("His.Desktop.Plugins.ImpMestCreate.SupplierImpMest.MustEnterDocumentNumberAndDocumentDate") == "1" ? true : false;
                DocumentDateValidationRule docDateRule = new DocumentDateValidationRule();
                docDateRule.txtDocumentDate = txtDocumentDate;
                docDateRule.dtDocumentDate = dtDocumentDate;
                docDateRule.cboImpMestType = cboImpMestType;
                docDateRule.keyCheck = keyCheck;
                dxValidationProvider1.SetValidationRule(dtDocumentDate, docDateRule);
                dxValidationProvider1.SetValidationRule(txtDocumentDate, docDateRule);

                DocumentValidationRule _rule = new DocumentValidationRule();
                _rule.txtDocument = txtDocumentNumber;
                _rule.cboImpMestType = cboImpMestType;
                _rule.keyCheck = keyCheck;
                _rule.ErrorText = "Trường dữ liệu bắt buộc";
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtDocumentNumber, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpAmount()
        {
            try
            {
                ImpAmountValidationRule impAmountRule = new ImpAmountValidationRule();
                impAmountRule.spinImpAmount = spinImpAmount;
                dxValidationProvider2.SetValidationRule(spinImpAmount, impAmountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpiredDate1(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider2.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlImPrice()
        {
            try
            {
                ImpPriceValidationRule impPriceRule = new ImpPriceValidationRule();
                impPriceRule.spinImpPrice = spinImpPrice;
                dxValidationProvider2.SetValidationRule(spinImpPrice, impPriceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlMaxLength(Control control, int maxLength)
        {
            try
            {
                ControlMaxLengthValidationRule maxLengthRule = new ControlMaxLengthValidationRule();
                maxLengthRule.editor = control;
                maxLengthRule.maxLength = maxLength;
                dxValidationProvider1.SetValidationRule(control, maxLengthRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidControlImpVatRatio()
        {
            try
            {
                ImpVatRatioValidationRule impVatRule = new ImpVatRatioValidationRule();
                impVatRule.spinImpVatRatio = spinImpVatRatio;
                dxValidationProvider2.SetValidationRule(spinImpVatRatio, impVatRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpiredDate()
        {
            try
            {
                ExpiredDateValidationRule expiredDateRule = new ExpiredDateValidationRule();
                expiredDateRule.txtExpiredDate = txtExpiredDate;
                expiredDateRule.dtExpiredDate = dtExpiredDate;
                dxValidationProvider2.SetValidationRule(txtExpiredDate, expiredDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetValueControlCommon()
        {
            try
            {
                this.resultADO = null;
                listMediStock = new List<V_HIS_MEDI_STOCK>();
                dicBidMaterial.Clear();
                dicBidMedicine.Clear();
                dicContractMaty.Clear();
                dicContractMety.Clear();
                listServiceADO = new List<VHisServiceADO>();
                gridControlImpMestDetail.DataSource = null;
                cboImpMestType.EditValue = null;
                cboImpMestType.Enabled = true;
                cboImpSource.EditValue = null;
                txtNhaCC.EditValue = null;
                this.currentSupplierForEdit = null;
                txtkyHieuHoaDon.Text = "";
                cboImpSource.EditValue = _ImpSourceId;
                this.currentBid = null;
                this.currentContract = null;
                checkOutBid.Checked = false;
                checkInOutBid.Checked = false;
                this.layoutControlItem7.Text = "Giá trong thầu:";
                txtBid.Text = "";
                txtBidNumOrder.Text = "";
                txtBidYear.Text = "";
                txtBidNumber.Text = "";
                spinImpVatRatio.Value = 0;
                spinEditThueXuat.Value = 0;
                spinDocumentPrice.Value = 0;
                txtDocumentNumber.Text = "";
                dtDocumentDate.EditValue = null;
                txtDocumentDate.Text = "";
                txtDeliverer.Text = "";
                txtDescription.Text = "";
                dropDownButton__Print.Enabled = false;
                btnImportExcel.Enabled = true;
                txtBid.Text = "";
                txtBid.Enabled = false;
                txtBidNumOrder.Enabled = false;
                txtBidYear.Enabled = false;
                txtBidNumber.Enabled = false;
                txtBidGroupCode.Enabled = false;
                spnTemperature.EditValue = null;
                IsRequiedTemperature = false;
                CalculTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetValueControlDetail(bool IsTreeClick = false)
        {
            try
            {
                this.currrentServiceAdo = null;
                this.listServicePatyAdo = new List<VHisServicePatyADO>();
                spinImpAmount.Value = 0;
                spinImpPrice.Value = 0;
                spinImpPrice1.Value = 0;
                spinImpVatRatio.Value = 0;
                spinEditThueXuat.Value = 0;
                spinEditGiaTrongThau.Value = 0;
                spinEditGiaNhapLanTruoc.Value = 0;
                spinEditTTCoVAT.Value = 0;
                spinEditTTChuaVAT.Value = 0;
                spinEditThueXuat.Value = 0;
                dtExpiredDate.EditValue = null;
                txtExpiredDate.Text = "";
                txtPackageNumber.Text = "";
                txtBid.Text = "";
                txtBidGroupCode.Text = "";
                btnAdd1.Enabled = false;
                txtBidNumOrder.Text = "";
                txtBidYear.Text = "";
                txtBidNumber.Text = "";
                spinCanImpAmount.Value = 0;
                chkImprice.Checked = false;
                chkImprice.Enabled = true;
                TxtSerialNumber.Text = "";
                SpMaxReuseCount.Value = 0;
                spinEditGiaVeSinh.Value = 0;
                gridControlServicePaty.DataSource = null;
                if (!IsTreeClick)
                {
                    txtBidExtraCode.Text = "";
                    cboGoiThau.EditValue = null;
                }
                cboGoiThau.Enabled = false;
                cboInformationBid.Enabled = false;
                cboInformationBid.EditValue = null;
                txtBidExtraCode.Enabled = false;
                dtHieuLucTu.EditValue = null;
                dtHieuLucDen.EditValue = null;
                cboHangSX.EditValue = null;
                cboNationals.EditValue = null;
                txtNationalMainText.Text = "";
                txtNognDoHL.Text = "";
                txtSoDangKy.Text = "";
                txtPackingJoinBid.Text = "";
                txtHeinServiceBidMateType.Text = "";
                this.txtActiveIngrBhytName.Text = "";
                this.cboDosageForm.EditValue = "";
                this.cboMedicineUseForm.EditValue = null;
                this.spnTemperature.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadImpMestTypeAllow()
        {
            try
            {
                var listImpMestTypeId = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK,
                     IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC
                 };
                if (HisImpMestTypeAuthorziedCFG.ImpMestType_IsAuthorized)
                {
                    HisImpMestTypeUserFilter impMestTypeUserFilter = new HisImpMestTypeUserFilter();
                    impMestTypeUserFilter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var listData = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_IMP_MEST_TYPE_USER>>("api/HisImpMestTypeUser/Get", ApiConsumers.MosConsumer, impMestTypeUserFilter, null);
                    if (listData != null && listData.Count > 0)
                    {
                        var listId = listData.Select(s => s.IMP_MEST_TYPE_ID).ToList();
                        listImpMestTypeId = listImpMestTypeId.Where(o => listId.Contains(o)).ToList();
                    }
                }

                listImpMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o => listImpMestTypeId.Contains(o.ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboImpMestType()
        {
            try
            {
                cboImpMestType.Properties.DataSource = listImpMestType;
                cboImpMestType.Properties.DisplayMember = "IMP_MEST_TYPE_NAME";
                cboImpMestType.Properties.ValueMember = "ID";
                cboImpMestType.Properties.ForceInitialize();
                cboImpMestType.Properties.Columns.Clear();
                cboImpMestType.Properties.Columns.Add(new LookUpColumnInfo("IMP_MEST_TYPE_CODE", "", 50));
                cboImpMestType.Properties.Columns.Add(new LookUpColumnInfo("IMP_MEST_TYPE_NAME", "", 200));
                cboImpMestType.Properties.ShowHeader = false;
                cboImpMestType.Properties.ImmediatePopup = true;
                cboImpMestType.Properties.DropDownRows = 10;
                cboImpMestType.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboMediStock()
        {
            try
            {
                cboMediStock.Properties.DataSource = null;
                cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStock.Properties.ValueMember = "ID";
                cboMediStock.Properties.ForceInitialize();
                cboMediStock.Properties.Columns.Clear();
                cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 50));
                cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboMediStock.Properties.ShowHeader = false;
                cboMediStock.Properties.ImmediatePopup = true;
                cboMediStock.Properties.DropDownRows = 10;
                cboMediStock.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboSupplier(List<HIS_SUPPLIER> data)
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("SUPPLIER_CODE", "", 80, 1));
                //columnInfos.Add(new ColumnInfo("SUPPLIER_NAME", "", 420, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("SUPPLIER_NAME", "ID", columnInfos, false, 500);
                //controlEditorADO.ImmediatePopup = true;
                //ControlEditorLoader.Load(txtNhaCC, data, controlEditorADO);

                txtNhaCC.Properties.DataSource = data;
                txtNhaCC.Properties.DisplayMember = "SUPPLIER_NAME";
                txtNhaCC.Properties.ValueMember = "ID";

                txtNhaCC.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                txtNhaCC.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                txtNhaCC.ForceInitialize();
                txtNhaCC.Properties.View.Columns.Clear();
                txtNhaCC.Properties.ImmediatePopup = true;
                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = txtNhaCC.Properties.View.Columns.AddField("SUPPLIER_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 140;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = txtNhaCC.Properties.View.Columns.AddField("SUPPLIER_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.ColumnEdit = repositoryItemMemoEdit1;
                aColumnName.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.Word;
                aColumnName.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                aColumnName.Width = 420;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboImpSource()
        {
            try
            {
                var datas = BackendDataWorker.Get<HIS_IMP_SOURCE>().Where(p => p.IS_ACTIVE == 1).ToList();
                cboImpSource.Properties.DataSource = datas;
                cboImpSource.Properties.DisplayMember = "IMP_SOURCE_NAME";
                cboImpSource.Properties.ValueMember = "ID";
                cboImpSource.Properties.ForceInitialize();
                cboImpSource.Properties.Columns.Clear();
                cboImpSource.Properties.Columns.Add(new LookUpColumnInfo("IMP_SOURCE_CODE", "", 40));
                cboImpSource.Properties.Columns.Add(new LookUpColumnInfo("IMP_SOURCE_NAME", "", 80));
                cboImpSource.Properties.ShowHeader = false;
                cboImpSource.Properties.ImmediatePopup = true;
                cboImpSource.Properties.DropDownRows = 10;
                cboImpSource.Properties.PopupWidth = 120;

                if (datas != null && datas.Count > 0)
                {
                    var dataChecks = datas.Where(p => p.IS_DEFAULT == 1).ToList();
                    if (dataChecks != null && dataChecks.Count > 0)
                    {
                        if (dataChecks.Count > 1)
                            dataChecks = dataChecks.OrderBy(p => p.IMP_SOURCE_CODE).ToList();
                        _ImpSourceId = dataChecks[0].ID;
                        cboImpSource.EditValue = _ImpSourceId;
                    }
                }

                //SetDataTaiKhoanNoCo
                this.txtTaiKhoanCo.Text = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.CREDIT_ACCOUNT");
                this.txtTaiKhoanNo.Text = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.DEBIT_ACCOUNT");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultImpMestType()
        {
            try
            {
                listMediStock = new List<V_HIS_MEDI_STOCK>();
                listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                this.currentImpMestType = null;
                this.currentImpMestType = listImpMestType.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC);
                if (this.currentImpMestType != null)
                {
                    cboImpMestType.EditValue = this.currentImpMestType.ID;
                }
                else if (listImpMestType != null && listImpMestType.Count == 1)
                {
                    this.currentImpMestType = listImpMestType.First();
                    cboImpMestType.EditValue = listImpMestType.FirstOrDefault().ID;
                }

                cboMediStock.Properties.DataSource = listMediStock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueMediStock()
        {
            try
            {
                if (!cboMediStock.Enabled || cboMediStock.Properties.DataSource == null || roomId <= 0)
                    return;
                medistock = listMediStock.FirstOrDefault(o => o.ROOM_ID == this.roomId);
                if (medistock != null)
                {
                    cboMediStock.EditValue = medistock.ID;
                    medistockID = medistock.ID;
                    if (medistock.DO_NOT_IMP_MEDICINE == 1)
                    {
                        xtraTabPageMedicine.PageVisible = false;
                    }
                    if (medistock.DO_NOT_IMP_MATERIAL == 1)
                    {
                        xtraTabPageMaterial.PageVisible = false;
                    }    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlEnableImMestTypeManu()
        {
            try
            {
                SetEnableControl(false);
                IsNCC = false;
                IsBID = false;
                IsHasValueChooice = false;
                if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    if (medistock.IS_ALLOW_IMP_SUPPLIER == 1)
                    {
                        IsNCC = true;
                        btnHoiDongKiemNhap.Enabled = false;
                        txtNhaCC.Enabled = true;
                        layoutSupplier.AppearanceItemCaption.ForeColor = Color.Maroon;
                        medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                        materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                        materialProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                        medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                        medicineProcessor.ReloadBid(this.ucMedicineTypeTree, listBids);
                        materialProcessor.ReloadBid(this.ucMaterialTypeTree, listBids);
                        medicineProcessor.EnableContract(this.ucMedicineTypeTree, false);
                        materialProcessor.EnableContract(this.ucMaterialTypeTree, false);
                        materialProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                        medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);

                        this.layoutControlItem7.Text = "Giá trong thầu";

                        checkOutBid.Enabled = true;
                        checkOutBid.Checked = false;
                        checkInOutBid.Checked = false;
                        txtDocumentNumber.Enabled = true;
                        dtDocumentDate.Enabled = true;
                        txtDocumentDate.Enabled = true;
                        txtkyHieuHoaDon.Enabled = true;
                        txtDeliverer.Enabled = true;
                        txtBidGroupCode.Text = "";
                        txtDescription.Enabled = true;
                        spinDocumentPrice.Enabled = true;
                        txtImpMestCode.Enabled = false;
                        txtImpMestCode.Text = "";
                    }
                    else
                    {
                        btnHoiDongKiemNhap.Enabled = false;
                        medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                        materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, null);
                        medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                        materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                        medicineProcessor.ReloadBid(this.ucMedicineTypeTree, listBids);
                        materialProcessor.ReloadBid(this.ucMaterialTypeTree, listBids);

                        medicineProcessor.EnableContract(this.ucMedicineTypeTree, false);
                        materialProcessor.EnableContract(this.ucMaterialTypeTree, false);
                        materialProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                        medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);

                        this.layoutControlItem7.Text = "Giá trong thầu";

                        txtNhaCC.Enabled = false;
                        this.currentSupplierForEdit = null;
                        checkOutBid.Enabled = false;
                        txtDocumentNumber.Text = "";
                        txtDocumentNumber.Enabled = false;
                        dtDocumentDate.EditValue = null;
                        dtDocumentDate.Enabled = false;
                        txtDocumentDate.Enabled = false;
                        txtkyHieuHoaDon.Text = "";
                        txtkyHieuHoaDon.Enabled = false;
                        txtDeliverer.Text = "";
                        txtDeliverer.Enabled = false;
                        txtBidGroupCode.Text = "";
                        spinDocumentPrice.Value = 0;
                        spinDocumentPrice.Enabled = false;

                        spinImpAmount.Value = 0;
                        spinImpAmount.Enabled = false;
                        spinImpPrice.Value = 0;
                        spinImpPrice.Enabled = false;
                        spinImpPrice1.Value = 0;
                        spinImpPrice1.Enabled = false;
                        spinImpVatRatio.Value = 0;
                        spinImpVatRatio.Enabled = false;
                        spinEditThueXuat.Value = 0;

                        spinImpPriceVAT.Value = 0;
                        spinImpPriceVAT.Enabled = false;
                        txtExpiredDate.Text = "";
                        txtExpiredDate.Enabled = false;
                        txtPackageNumber.Text = "";
                        txtPackageNumber.Enabled = false;
                        btnAdd1.Enabled = false;
                        cboImpSource.EditValue = null;
                        cboImpSource.Enabled = false;
                        txtBid.Text = "";
                        txtBid.Enabled = false;
                        txtBidNumOrder.Text = "";
                        txtBidYear.Text = "";
                        txtBidYear.Enabled = false;
                        txtBidNumber.Text = "";
                        txtBidNumber.Enabled = false;
                        txtBidNumOrder.Enabled = false;
                        txtDescription.Text = "";
                        txtDescription.Enabled = false;
                        txtBidGroupCode.Enabled = false;

                        btnSave.Enabled = false;
                        btnSaveDraft.Enabled = false;
                        btnImportExcel.Enabled = false;
                        btnNew.Enabled = false;
                        txtImpMestCode.Enabled = false;
                        txtImpMestCode.Text = "";
                    }
                }
                else if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    btnHoiDongKiemNhap.Enabled = false;
                    //medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                    //materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                    //materialProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    // medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);


                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                    this.currentSupplier = this.currentSupplierForEdit;
                    materialProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    medicineProcessor.ReloadBid(this.ucMedicineTypeTree, listBids);
                    materialProcessor.ReloadBid(this.ucMaterialTypeTree, listBids);



                    medicineProcessor.EnableContract(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableContract(this.ucMaterialTypeTree, false);
                    materialProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                    medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);

                    this.layoutControlItem7.Text = "Giá trong thầu";

                    checkOutBid.Enabled = true;
                    checkOutBid.Checked = false;
                    checkInOutBid.Checked = false;
                    txtBidGroupCode.Text = "";
                    txtNhaCC.Enabled = true;
                    layoutSupplier.AppearanceItemCaption.ForeColor = Color.Black;
                    this.currentSupplierForEdit = null;
                    txtDocumentNumber.Text = "";
                    txtDocumentNumber.Enabled = false;
                    dtDocumentDate.EditValue = null;
                    dtDocumentDate.Enabled = false;
                    txtDocumentDate.Enabled = false;
                    txtkyHieuHoaDon.Text = "";
                    txtkyHieuHoaDon.Enabled = false;
                    txtDeliverer.Text = "";
                    txtDeliverer.Enabled = false;
                    spinDocumentPrice.Value = 0;
                    spinDocumentPrice.Enabled = false;


                    //enable cho control
                    spinImpAmount.Value = 0;
                    spinImpAmount.Enabled = true;
                    spinImpPrice.Value = 0;
                    spinImpPrice.Enabled = true;
                    spinImpPrice1.Value = 0;
                    spinImpPrice1.Enabled = true;
                    spinImpVatRatio.Value = 0;
                    spinImpVatRatio.Enabled = true;
                    spinEditThueXuat.Value = 0;
                    spinImpPriceVAT.Value = 0;
                    spinImpPriceVAT.Enabled = true;
                    txtExpiredDate.Text = "";
                    txtExpiredDate.Enabled = true;
                    txtPackageNumber.Text = "";
                    txtPackageNumber.Enabled = true;
                    btnAdd1.Enabled = true;
                    cboImpSource.EditValue = null;
                    cboImpSource.Enabled = true;
                    txtBid.Text = "";
                    txtBid.Enabled = true;
                    txtBidNumOrder.Text = "";
                    txtBidNumOrder.Enabled = true;
                    txtBidYear.Text = "";
                    txtBidYear.Enabled = true;
                    txtBidNumber.Text = "";
                    txtBidNumber.Enabled = true;
                    txtDescription.Text = "";
                    txtDescription.Enabled = true;
                    txtBidGroupCode.Enabled = true;

                    btnSave.Enabled = true;
                    btnSaveDraft.Enabled = true;
                    btnImportExcel.Enabled = true;
                    btnNew.Enabled = true;

                    txtImpMestCode.Enabled = false;
                    txtImpMestCode.Text = "";


                }
                else if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                {
                    btnHoiDongKiemNhap.Enabled = false;
                    medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, null);
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, false);

                    medicineProcessor.EnableContract(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableContract(this.ucMaterialTypeTree, false);
                    materialProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                    medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);

                    this.layoutControlItem7.Text = "Giá trong thầu";

                    this.currentSupplier = this.currentSupplierForEdit = null;
                    txtNhaCC.Enabled = false;
                    checkOutBid.Enabled = false;
                    txtDocumentNumber.Text = "";
                    txtDocumentNumber.Enabled = false;
                    dtDocumentDate.EditValue = null;
                    dtDocumentDate.Enabled = false;
                    txtDocumentDate.Enabled = false;
                    txtDeliverer.Text = "";
                    txtDeliverer.Enabled = false;
                    txtBidGroupCode.Text = "";
                    spinDocumentPrice.Value = 0;
                    spinDocumentPrice.Enabled = false;

                    //enable cho control
                    spinImpAmount.Value = 0;
                    spinImpAmount.Enabled = true;
                    spinImpPrice.Value = 0;
                    spinImpPrice.Enabled = true;
                    spinImpPrice1.Value = 0;
                    spinImpPrice1.Enabled = true;
                    spinImpVatRatio.Value = 0;
                    spinImpVatRatio.Enabled = true;
                    spinEditThueXuat.Value = 0;
                    spinImpPriceVAT.Value = 0;
                    spinImpPriceVAT.Enabled = true;
                    txtExpiredDate.Text = "";
                    txtExpiredDate.Enabled = true;
                    txtPackageNumber.Text = "";
                    txtPackageNumber.Enabled = true;
                    btnAdd1.Enabled = true;
                    cboImpSource.EditValue = null;
                    cboImpSource.Enabled = true;
                    txtBid.Text = "";
                    txtBid.Enabled = true;
                    txtBidNumOrder.Text = "";
                    txtBidNumOrder.Enabled = true;
                    txtBidYear.Text = "";
                    txtBidYear.Enabled = true;
                    txtBidNumber.Text = "";
                    txtBidNumber.Enabled = true;
                    txtDescription.Text = "";
                    txtDescription.Enabled = true;
                    txtBidGroupCode.Enabled = true;

                    btnSave.Enabled = true;
                    btnSaveDraft.Enabled = true;
                    btnImportExcel.Enabled = true;
                    btnNew.Enabled = true;

                    txtImpMestCode.Enabled = false;
                    txtImpMestCode.Text = "";
                }
                else
                {
                    btnHoiDongKiemNhap.Enabled = false;
                    txtBidGroupCode.Text = "";
                    medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, null);
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, false);

                    medicineProcessor.EnableContract(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableContract(this.ucMaterialTypeTree, false);
                    materialProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                    medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);

                    this.layoutControlItem7.Text = "Giá trong thầu";
                    this.currentSupplier = this.currentSupplierForEdit = null;
                    txtNhaCC.Enabled = false;
                    checkOutBid.Enabled = false;
                    txtDocumentNumber.Text = "";
                    txtDocumentNumber.Enabled = false;
                    dtDocumentDate.EditValue = null;
                    dtDocumentDate.Enabled = false;
                    txtDocumentDate.Enabled = false;
                    txtDeliverer.Text = "";
                    txtDeliverer.Enabled = false;
                    spinDocumentPrice.Value = 0;
                    spinDocumentPrice.Enabled = false;

                    //enable cho control
                    spinImpAmount.Value = 0;
                    spinImpAmount.Enabled = true;
                    spinImpPrice.Value = 0;
                    spinImpPrice.Enabled = true;
                    spinImpPrice1.Value = 0;
                    spinImpPrice1.Enabled = true;
                    spinImpVatRatio.Value = 0;
                    spinImpVatRatio.Enabled = true;
                    spinEditThueXuat.Value = 0;
                    spinImpPriceVAT.Value = 0;
                    spinImpPriceVAT.Enabled = true;
                    txtExpiredDate.Text = "";
                    txtExpiredDate.Enabled = true;
                    txtPackageNumber.Text = "";
                    txtPackageNumber.Enabled = true;
                    btnAdd1.Enabled = true;
                    cboImpSource.EditValue = null;
                    cboImpSource.Enabled = true;
                    txtBid.Text = "";
                    txtBid.Enabled = true;
                    txtBidNumOrder.Text = "";
                    txtBidNumOrder.Enabled = true;
                    txtBidYear.Text = "";
                    txtBidYear.Enabled = true;
                    txtBidNumber.Text = "";
                    txtBidNumber.Enabled = true;
                    txtDescription.Text = "";
                    txtDescription.Enabled = true;
                    txtBidGroupCode.Enabled = true;

                    btnSave.Enabled = true;
                    btnSaveDraft.Enabled = true;
                    btnImportExcel.Enabled = true;
                    btnNew.Enabled = true;

                    txtkyHieuHoaDon.Text = "";
                    txtImpMestCode.Enabled = false;
                    txtImpMestCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSourceGridControlMediMate()
        {
            try
            {
                //if(listMedicineTypeTemp == null|| listMedicineTypeTemp.Count == 0)
                //    listMedicineTypeTemp = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o =>
                //      o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                //      && o.IS_LEAF == 1
                //      && o.IS_STOP_IMP == null).ToList();

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
				#region Vật tư
				//if (listMaterialTypeTemp == null || listMaterialTypeTemp.Count == 0)
				//    listMaterialTypeTemp = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o =>
				//     o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
				//     && o.IS_LEAF == 1
				//     && o.IS_STOP_IMP == null).ToList();

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
				#endregion
                if (this.currentBid != null)
                {
                    if (dicBidMedicine.Count > 0)
                    {
                        foreach (var item in dicBidMedicine)
                        {
                            if (item.Value == null)
                            {
                                continue;
                            }

                            var medicineType = listMedicineTypeTemp.FirstOrDefault(o => o.ID == item.Value.MEDICINE_TYPE_ID);
                            if (medicineType == null)
                                continue;
                            MedicineTypeADO medicineTypeADO = new MedicineTypeADO(medicineType);
                            medicineTypeADO.AMOUNT_IN_BID = item.Value.AMOUNT;
                            medicineTypeADO.IMP_PRICE_IN_BID = item.Value.IMP_PRICE;
                            medicineTypeADO.IMP_VAT_RATIO_IN_BID = item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null;
                            medicineTypeADO.BidGroupCode = item.Value.BID_GROUP_CODE;
                            medicineTypeADO.KeyField = Base.StaticMethod.GetTypeKey(item.Value.MEDICINE_TYPE_ID, item.Value.BID_GROUP_CODE);
                            if (medicineType.IMP_UNIT_ID.HasValue)
                            {
                                medicineTypeADO.SERVICE_UNIT_NAME = medicineType.IMP_UNIT_NAME;
                            }
                            medicineTypeADO.ADJUST_AMOUNT = item.Value.ADJUST_AMOUNT;
                            listMedicineType.Add(medicineTypeADO);
                        }
                    }

                    if (dicBidMaterial.Count > 0)
                    {
                        foreach (var item in dicBidMaterial)
                        {
                            if (item.Value == null)
                            {
                                continue;
                            }

                            var materialType = listMaterialTypeTemp.FirstOrDefault(o => o.ID == item.Value.MATERIAL_TYPE_ID);
                            if (materialType == null)
                                continue;
                            MaterialTypeADO materialTypeADO = new MaterialTypeADO(materialType);
                            materialTypeADO.AMOUNT_IN_BID = item.Value.AMOUNT;
                            materialTypeADO.IMP_PRICE_IN_BID = item.Value.IMP_PRICE;
                            materialTypeADO.IMP_VAT_RATIO_IN_BID = item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null;
                            materialTypeADO.BidGroupCode = item.Value.BID_GROUP_CODE;
                            materialTypeADO.KeyField = Base.StaticMethod.GetTypeKey(item.Value.MATERIAL_TYPE_ID ?? 0, item.Value.BID_GROUP_CODE);
                            if (materialType.IMP_UNIT_ID.HasValue)
                            {
                                materialTypeADO.SERVICE_UNIT_NAME = materialType.IMP_UNIT_NAME;
                            }
                            materialTypeADO.ADJUST_AMOUNT = item.Value.ADJUST_AMOUNT;
                            listMaterialType.Add(materialTypeADO);
                        }
                    }
                }

                if (this.currentContract != null)
                {
                    if (dicContractMety.Count > 0)
                    {
                        foreach (var item in dicContractMety)
                        {
                            var medicineType = listMedicineTypeTemp.FirstOrDefault(o => o.ID == item.Value.MEDICINE_TYPE_ID);
                            if (medicineType == null)
                                continue;
                            MedicineTypeADO medicineTypeADO = new MedicineTypeADO(medicineType);
                            medicineTypeADO.AMOUNT_IN_CONTRACT = item.Value.AMOUNT;
                            medicineTypeADO.IMP_PRICE_IN_CONTRACT = item.Value.CONTRACT_PRICE;
                            medicineTypeADO.IMP_VAT_RATIO_IN_CONTRACT = item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null;
                            medicineTypeADO.MEDI_CONTRACT_METY_ID = item.Value.ID;
                            if (medicineType.IMP_UNIT_ID.HasValue)
                            {
                                medicineTypeADO.SERVICE_UNIT_NAME = medicineType.IMP_UNIT_NAME;
                            }
                            medicineTypeADO.DISCOUNT_FROM_DATE = item.Value.DISCOUNT_FROM_DATE;
                            medicineTypeADO.DISCOUNT_TO_DATE = item.Value.DISCOUNT_TO_DATE;
                            medicineTypeADO.DISCOUNT_FROM_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_FROM_DATE ?? 0);
                            medicineTypeADO.DISCOUNT_TO_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_TO_DATE ?? 0);
                            listMedicineType.Add(medicineTypeADO);
                        }
                    }

                    if (dicContractMaty.Count > 0)
                    {
                        foreach (var item in dicContractMaty)
                        {
                            var materialType = listMaterialTypeTemp.FirstOrDefault(o => o.ID == item.Value.MATERIAL_TYPE_ID);
                            if (materialType == null)
                                continue;
                            MaterialTypeADO materialTypeADO = new MaterialTypeADO(materialType);
                            materialTypeADO.AMOUNT_IN_CONTRACT = item.Value.AMOUNT;
                            materialTypeADO.IMP_PRICE_IN_CONTRACT = item.Value.CONTRACT_PRICE;
                            materialTypeADO.IMP_VAT_RATIO_IN_CONTRACT = item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null;
                            materialTypeADO.MEDI_CONTRACT_MATY_ID = item.Value.ID;
                            if (materialType.IMP_UNIT_ID.HasValue)
                            {
                                materialTypeADO.SERVICE_UNIT_NAME = materialType.IMP_UNIT_NAME;
                            }

                            materialTypeADO.DISCOUNT_FROM_DATE = item.Value.DISCOUNT_FROM_DATE;
                            materialTypeADO.DISCOUNT_TO_DATE = item.Value.DISCOUNT_TO_DATE;
                            materialTypeADO.DISCOUNT_FROM_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_FROM_DATE ?? 0);
                            materialTypeADO.DISCOUNT_TO_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_TO_DATE ?? 0);
                            listMaterialType.Add(materialTypeADO);
                        }
                    }
                }

                if (this.currentBid == null && this.currentContract == null)
                {
                    listMedicineType = (from r in listMedicineTypeTemp select new MedicineTypeADO(r)).ToList();
                    listMaterialType = (from r in listMaterialTypeTemp select new MaterialTypeADO(r)).ToList();
                }

                if (listMedicineType != null && listMedicineType.Count > 0)
                {
                    listMedicineType.ForEach(o =>
                    {
                        if (o.IMP_UNIT_ID.HasValue) o.SERVICE_UNIT_NAME = o.IMP_UNIT_NAME;
                    });
                }

                if (listMaterialType != null && listMaterialType.Count > 0)
                {
                    listMaterialType.ForEach(o =>
                    {
                        if (o.IMP_UNIT_ID.HasValue) o.SERVICE_UNIT_NAME = o.IMP_UNIT_NAME;
                    });
                }

                this.medicineProcessor.Reload(this.ucMedicineTypeTree, listMedicineType);
                this.materialProcessor.Reload(this.ucMaterialTypeTree, listMaterialType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataByBid()
        {
            try
            {
                dicBidMaterial.Clear();
                dicBidMedicine.Clear();

				Inventec.Common.Logging.LogSystem.Debug("LoadDataByBid___"+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentBid), currentBid));
                if (this.currentBid != null)
                {
                    HisBidMedicineTypeViewFilter mediFilter = new HisBidMedicineTypeViewFilter();
                    mediFilter.BID_ID = this.currentBid.ID;
                    mediFilter.IS_ACTIVE = 1;
                    var listBidMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MEDICINE_TYPE>>(HisRequestUriStore.HIS_BID_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);
                    
                    HisBidMaterialTypeViewFilter mateFilter = new HisBidMaterialTypeViewFilter();
                    mateFilter.BID_ID = this.currentBid.ID;
                    mateFilter.IS_ACTIVE = 1;
                    var listBidMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MATERIAL_TYPE>>(HisRequestUriStore.HIS_BID_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);
                    List<long> listSupplierIds = new List<long>();
                    LogSystem.Debug("____listBidMedicine Count: " + listBidMedicine.Count() + "______listBidMaterial Count: " + listBidMaterial.Count());
                    if (listBidMedicine != null && listBidMedicine.Count > 0)
                    {
                        if (listBidMedicine != null && listBidMedicine.Count > 0)
                        {
                            listSupplierIds.AddRange(listBidMedicine.Select(o => o.SUPPLIER_ID).Distinct().ToList());
                        }
                        if (currentSupplier != null)
                        {
                            listBidMedicine = listBidMedicine.Where(o => o.SUPPLIER_ID == currentSupplier.ID).ToList();
                        }
                        foreach (var item in listBidMedicine)
                        {
                            dicBidMedicine[Base.StaticMethod.GetTypeKey(item.MEDICINE_TYPE_ID, item.BID_GROUP_CODE)] = item;
                        }
                    }

                    if (listBidMaterial != null && listBidMaterial.Count > 0)
                    {
                        if (listBidMaterial != null && listBidMaterial.Count > 0)
                        {
                            listSupplierIds.AddRange(listBidMaterial.Select(o => o.SUPPLIER_ID).Distinct().ToList());
                        }
                        if (currentSupplier != null)
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

                    if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                    {
                        if (medicineProcessor.GetBidEnable(this.ucMedicineTypeTree) == true && listSupplierIds != null && listSupplierIds.Count > 0)
                        {
                            listSupplierIds = listSupplierIds.Distinct().ToList();
                            var Suppliers = this.listSupplier.Where(o => listSupplierIds.Contains(o.ID)).ToList();

                            LoadDataToComboSupplier(Suppliers);
                            if (Suppliers != null && Suppliers.Count == 1)
                            {
                                this.currentSupplierForEdit = Suppliers.FirstOrDefault();
                                txtNhaCC.EditValue = this.currentSupplierForEdit.ID;
                            }
                        }
                    }

                    if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                    {
                        if (materialProcessor.GetBidEnable(this.ucMaterialTypeTree) == true && listSupplierIds != null && listSupplierIds.Count > 0)
                        {
                            listSupplierIds = listSupplierIds.Distinct().ToList();
                            var Suppliers = this.listSupplier.Where(o => listSupplierIds.Contains(o.ID)).ToList();

                            LoadDataToComboSupplier(Suppliers);
                            if (Suppliers != null && Suppliers.Count == 1)
                            {
                                this.currentSupplierForEdit = Suppliers.FirstOrDefault();
                                txtNhaCC.EditValue = this.currentSupplierForEdit.ID;
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

        private void SetFocuTreeMediMate()
        {
            try
            {
                if (xtraTabControlMain.SelectedTabPageIndex == 0)
                {
                    medicineProcessor.FocusKeyword(this.ucMedicineTypeTree);
                }
                else
                {
                    materialProcessor.FocusKeyword(this.ucMaterialTypeTree);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboReceive()
        {
            try
            {
                var datas = BackendDataWorker.Get<ACS_USER>().Where(p => p.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 250, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboRecieve, datas, controlEditorADO);
                var loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var defaultLogginNames = datas.Where(o => o.LOGINNAME == loginname).ToList();
                if (defaultLogginNames != null && defaultLogginNames.Count > 0)
                {
                    var defaultLogginName = defaultLogginNames.FirstOrDefault();
                    cboRecieve.EditValue = defaultLogginName.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidGoiThauNewControl()
        {
            try
            {
                Validation.GoiThauNewValidationRule _rule = new Validation.GoiThauNewValidationRule();
                _rule.cbo = this.cboGoiThau;
                _rule.cboImpMestType = this.cboImpMestType;
                _rule.ErrorText = "Trường dữ liệu bắt buộc";
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider2.SetValidationRule(this.cboGoiThau, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSaleProfits()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSaleProfitCfgFilter filter = new HisSaleProfitCfgFilter();
                filter.IS_ACTIVE = 1;
                _HisSaleProfitCfgs = new BackendAdapter(param).Get<List<HIS_SALE_PROFIT_CFG>>("api/HisSaleProfitCfg/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetFontSizeForm()
        {
            var demSize = HIS.Desktop.ApplicationFont.ApplicationFontWorker.GetFontSize();
            if (demSize == HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize1025)
            {
                d = 11;
            }
            else if (demSize == HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize975)
            {
                d = 10;
            }
            else if (demSize == HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize925)
            {
                d = 7;
            }
            else if (demSize == HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize875)
            {
                d = 6;
            }
        }

        private void DataToComboNation(Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo, List<NationalADO> listADO)
        {
            try
            {
                //var dataNationals = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => p.IS_ACTIVE == 1).ToList();
                //List<NationalADO> listADO = new List<NationalADO>();
                //foreach (var item in dataNationals)
                //{
                //    NationalADO national = new NationalADO();
                //    national.ID = item.ID;
                //    national.NATIONAL_CODE = item.NATIONAL_CODE;
                //    national.NATIONAL_NAME = item.NATIONAL_NAME;
                //    national.NATIONAL_NAME_UNSIGN = convertToUnSign3(item.NATIONAL_NAME);
                //    listADO.Add(national);
                //}

                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "NATIONAL_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);
                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("NATIONAL_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("NATIONAL_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("NATIONAL_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["NATIONAL_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DataToComboManufacturer(Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo, List<ManufacturerADO> listADO)
        {
            try
            {
                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "MANUFACTURER_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("MANUFACTURER_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("MANUFACTURER_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("MANUFACTURER_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["MANUFACTURER_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DataToComboDosageForm(Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo, List<DosageFormADO> listADO)
        {
            try
            {
                cbo.EditValue = null;
                cbo.Properties.View.ActiveFilter.Clear();
                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "DOSAGE_FORM_NAME";
                cbo.Properties.ValueMember = "DOSAGE_FORM_NAME";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("DOSAGE_FORM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("DOSAGE_FORM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("DOSAGEFORM_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["DOSAGEFORM_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultCheckNoBid()
        {
            try
            {
                if (IsAutoCheckNoBidCFG.IsAutoCheckNoBid && cboImpMestType.EditValue != null && this.medistock.IS_BUSINESS == 1)
                {
                    long impMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboImpMestType.EditValue ?? "").ToString());
                    if (impMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || impMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                        checkOutBid.CheckState = CheckState.Checked;
                    else
                        checkOutBid.CheckState = CheckState.Unchecked;
                    checkOutBid_CheckedChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalculTotalPrice()
        {
            try
            {
                decimal totalprice = 0;
                decimal totalDocumentPrice = 0;
                decimal totalfeePrice = 0;
                decimal totalvatPrice = 0;
                decimal totalBefVATPrice = 0;
                if (listServiceADO != null && listServiceADO.Count > 0)
                {
                    totalfeePrice = listServiceADO.Sum(s => (s.IMP_AMOUNT * s.IMP_PRICE));
                    totalBefVATPrice = listServiceADO.Sum(s => (s.IMP_AMOUNT * s.IMP_PRICE * (1 + s.IMP_VAT_RATIO)));
                    totalvatPrice = totalBefVATPrice - totalfeePrice;
                    totalDocumentPrice = listServiceADO.Sum(s => s.DOCUMENT_PRICE ?? 0);
                    totalprice = totalfeePrice + totalvatPrice;
                }

                if (!isInit)
                    spinDocumentPrice.EditValue = totalprice;
                lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalprice, ConfigApplications.NumberSeperator);
                lblTotalFeePrice.Text = Inventec.Common.Number.Convert.NumberToString(totalfeePrice, ConfigApplications.NumberSeperator);
                lblTotalVatPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalvatPrice, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeLableMetyMaty()
        {
            try
            {
                if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                {
                    this.lciPackingJoinBid.Text = "QC đóng gói:";
                    this.lciPackingJoinBid.OptionsToolTip.ToolTip = "Quy cách đóng gói";
                    this.lciHeinServiceBidMateType.Text = "Tên BHYT:";
                    this.lciSoDangKy.AppearanceItemCaption.ForeColor = Color.Black;
                    //this.lciPackingJoinBid.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciHeinServiceBidMateType.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciActiveIngrBhytName.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciMedicineUseForm.AppearanceItemCaption.ForeColor = Color.Black;
                    //this.lciDosageForm.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciConcentra.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciActiveIngrBhytName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciMedicineUseForm.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciDosageForm.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                {
                    this.lciPackingJoinBid.Text = "Mã trúng thầu:";
                    this.lciPackingJoinBid.OptionsToolTip.ToolTip = "";
                    this.lciHeinServiceBidMateType.Text = "Tên trúng thầu:";
                    this.lciSoDangKy.AppearanceItemCaption.ForeColor = Color.Black;
                    //this.lciPackingJoinBid.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciHeinServiceBidMateType.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciActiveIngrBhytName.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciMedicineUseForm.AppearanceItemCaption.ForeColor = Color.Black;
                    //this.lciDosageForm.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciConcentra.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciActiveIngrBhytName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lciMedicineUseForm.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lciDosageForm.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeColorMedicine(VHisServiceADO serviceAdo)
        {
            try
            {
                if (serviceAdo.IsMedicine && serviceAdo.isBusiness != 1)
                {
                    this.lciSoDangKy.AppearanceItemCaption.ForeColor = Color.Orange;
                    //this.lciPackingJoinBid.AppearanceItemCaption.ForeColor = Color.Orange;
                    this.lciHeinServiceBidMateType.AppearanceItemCaption.ForeColor = Color.Orange;
                    this.lciActiveIngrBhytName.AppearanceItemCaption.ForeColor = Color.Orange;
                    this.lciMedicineUseForm.AppearanceItemCaption.ForeColor = Color.Orange;
                    //this.lciDosageForm.AppearanceItemCaption.ForeColor = Color.Orange;
                    this.lciConcentra.AppearanceItemCaption.ForeColor = Color.Orange;
                }
                else
                {
                    this.lciSoDangKy.AppearanceItemCaption.ForeColor = Color.Black;
                    //this.lciPackingJoinBid.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciHeinServiceBidMateType.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciActiveIngrBhytName.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciMedicineUseForm.AppearanceItemCaption.ForeColor = Color.Black;
                    //this.lciDosageForm.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lciConcentra.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboMedicineUseForm()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_CODE", "", 80, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 420, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 500);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboMedicineUseForm, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
