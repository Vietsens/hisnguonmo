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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.MedicineUpdate.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.SDO;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.MedicineUpdate
{
    public partial class FormMedicineUpdate : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        MOS.EFMODEL.DataModels.V_HIS_MEDICINE_1 medicine;
        System.Globalization.CultureInfo cultureLang;
        int positionHandle = -1;
        #endregion

        #region Construct
        public FormMedicineUpdate(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormMedicineUpdate(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_1 data, Inventec.Desktop.Common.Modules.Module moduleData)
            : this(moduleData)
        {
            try
            {
                this.medicine = data;
                this.Text = moduleData.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        #region load
        private void FormMedicineUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //icon
                SetIcon();
                //
                InitComboImpSource();

                //Load du lieu
                FillData();

                chkUpdateCKVP.Checked = true;

                if (this.medicine != null)
                {
                    EnableBBGN(this.medicine.ID);
                }
                //valid
                ValidControls();

                //Focus
                spinImpPrice.Focus();
                spinImpPrice.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboImpSource()
        {
            try
            {
                var source = BackendDataWorker.Get<HIS_IMP_SOURCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("IMP_SOURCE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("IMP_SOURCE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("IMP_SOURCE_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboImpSource, source, controlEditorADO);
                cboImpSource.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.lciExpiredDate.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_EXPIRED_DATE",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciImpPrice.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_IMP_PRICE",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciImpVatRatio.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_IMP_VAT_RATIO",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciPackgeNumber.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_PACKGE_NUMBER",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciMedicineBytNumOrder.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_MEDICINE_BYT_NUM_ORDER",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciMedicineIsStarMark.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_MEDICINE_IS_STAR_MARK",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciMedicineRegisterNumOrder.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_MEDICINE_REGISTER_NUM_ORDER",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciMedicineTcyNumOrder.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_MEDICINE_TCY_NUM_ORDER",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciSTTBid.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_BID_NUM_ORDER",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciGroupBid.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_BID_GROUP_CODE",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciPackBid.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_BID_PACKAGE_CODE",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciBBGN.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_BBGN",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
                this.lciBBGN.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MEDICINE_UPDATE__LCI_BBGN_TOOLTIP",
                    Resources.ResourceLanguageManager.LanguageFormMedicineUpdate,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableBBGN(long medicineId)
        {
            if (CheckMedicineExistExp(medicineId))
            {
                lciBBGN.Enabled = false;

            }
            else
            {
                lciBBGN.Enabled = true;
            }
        }

        private bool CheckMedicineExistExp(long medicineId)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineFilter filter = new MOS.Filter.HisExpMestMedicineFilter();
                filter.MEDICINE_ID = medicineId;
                var expMedicines = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (expMedicines != null && expMedicines.Count() > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillData()
        {
            try
            {
                WaitingManager.Show();
                this.spinImpPrice.EditValue = medicine.IMP_PRICE;
                this.spinImpProfit.EditValue = medicine.PROFIT_RATIO * 100;
                this.spinImpVatRatio.EditValue = medicine.IMP_VAT_RATIO * 100;
                this.txtPackgeNumber.Text = medicine.PACKAGE_NUMBER;
                this.dtExpiredDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(medicine.EXPIRED_DATE ?? 0);
                this.lblContractCode.Text = medicine.MEDICAL_CONTRACT_CODE;
                this.lblContractName.Text = medicine.MEDICAL_CONTRACT_NAME;
                this.txtMedicineBytNumOrder.Text = medicine.MEDICINE_BYT_NUM_ORDER;
                this.txtMedicineRegisterNumOrder.Text = medicine.MEDICINE_REGISTER_NUMBER;
                this.txtMedicineTcyNumOrder.Text = medicine.MEDICINE_TCY_NUM_ORDER;
                this.txtMaHoatChatBHYT.Text = medicine.ACTIVE_INGR_BHYT_CODE;
                this.txtConcentra.Text = medicine.CONCENTRA;
                this.txtHeinServiceBHYTName.Text = medicine.HEIN_SERVICE_BHYT_NAME;
                this.txtTenHoatChatBHYT.Text = medicine.ACTIVE_INGR_BHYT_NAME;
                if (medicine.MEDICINE_IS_STAR_MARK == 1)
                {
                    this.chkMedicineIsStarMark.CheckState = CheckState.Checked;
                }
                else
                {
                    this.chkMedicineIsStarMark.CheckState = CheckState.Unchecked;
                }
                this.txtSTTBid.Text = medicine.TDL_BID_NUM_ORDER;
                this.txtPackBid.Text = medicine.TDL_BID_PACKAGE_CODE;
                this.txtGroupBid.Text = medicine.TDL_BID_GROUP_CODE;
                this.txtBidNumber.Text = medicine.TDL_BID_NUMBER;
                this.txtBidYear.Text = medicine.TDL_BID_YEAR;
                this.txtBidExtraCode.Text = medicine.TDL_BID_EXTRA_CODE;
                this.cboImpSource.EditValue = medicine.IMP_SOURCE_ID;
                this.txtTTThau.Text = medicine.TT_THAU;
                if (medicine.IS_SALE_EQUAL_IMP_PRICE == 1)
                {
                    this.chkBBGN.CheckState = CheckState.Checked;
                }
                else
                {
                    this.chkBBGN.CheckState = CheckState.Unchecked;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event
        private void barButtonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_MEDICINE GetDataSave()
        {
            HIS_MEDICINE result = new HIS_MEDICINE();
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDICINE>(result, medicine);
                result.IMP_PRICE = spinImpPrice.Value;
                result.PROFIT_RATIO = spinImpProfit.Value / 100;
                result.IMP_VAT_RATIO = spinImpVatRatio.Value / 100;
                if (dtExpiredDate.EditValue != null && dtExpiredDate.DateTime != DateTime.MinValue)
                {
                    if (dtExpiredDate.DateTime < DateTime.Now)
                        if (MessageBox.Show("Hạn dùng nhỏ hơn ngày hiện tại. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                            return null;
                    result.EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else result.EXPIRED_DATE = null;
                result.IMP_SOURCE_ID = cboImpSource.EditValue != null ? (long?)Convert.ToInt64(cboImpSource.EditValue.ToString()) : null;
                result.PACKAGE_NUMBER = txtPackgeNumber.Text;
                result.MEDICINE_BYT_NUM_ORDER = txtMedicineBytNumOrder.Text;
                result.MEDICINE_REGISTER_NUMBER = txtMedicineRegisterNumOrder.Text;
                result.MEDICINE_TCY_NUM_ORDER = txtMedicineTcyNumOrder.Text;
                result.ACTIVE_INGR_BHYT_CODE = txtMaHoatChatBHYT.Text;
                result.CONCENTRA = txtConcentra.Text;
                result.HEIN_SERVICE_BHYT_NAME = txtHeinServiceBHYTName.Text;
                result.ACTIVE_INGR_BHYT_NAME = txtTenHoatChatBHYT.Text;
                if (chkMedicineIsStarMark.CheckState == CheckState.Checked)
                {
                    result.MEDICINE_IS_STAR_MARK = 1;
                }
                else
                {
                    result.MEDICINE_IS_STAR_MARK = null;
                }
                result.TDL_BID_EXTRA_CODE = txtBidExtraCode.Text.Trim();
                result.TDL_BID_PACKAGE_CODE = txtPackBid.Text.Trim();
                result.TDL_BID_GROUP_CODE = txtGroupBid.Text.Trim();
                result.TDL_BID_NUM_ORDER = txtSTTBid.Text.Trim();
                result.TDL_BID_NUMBER = txtBidNumber.Text.Trim();
                result.TDL_BID_YEAR = txtBidYear.Text.Trim();
                result.TT_THAU = txtTTThau.Text.Trim();
                if (chkBBGN.CheckState == CheckState.Checked)
                {
                    result.IS_SALE_EQUAL_IMP_PRICE = 1;
                }
                else
                {
                    result.IS_SALE_EQUAL_IMP_PRICE = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void spinImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpProfit.Focus();
                    spinImpProfit.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpVatRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPackgeNumber.Focus();
                    txtPackgeNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackgeNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpiredDate.Focus();
                    dtExpiredDate.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtMedicineBytNumOrder.Focus();
                    txtMedicineBytNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineBytNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkMedicineIsStarMark.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMedicineIsStarMark_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineRegisterNumOrder.Focus();
                    txtMedicineRegisterNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineRegisterNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineTcyNumOrder.Focus();
                    txtMedicineTcyNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineTcyNumOrder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSTTBid.Focus();
                    txtSTTBid.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSTTBid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGroupBid.Focus();
                    txtGroupBid.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGroupBid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPackBid.Focus();
                    txtPackBid.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPackBid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBidNumber.Focus();
                    txtBidNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Validation
        private void ValidControls()
        {
            try
            {
                ValidImpPrice();
                ValidImpVatRatio();
                //ValidExpriedDate();
                if (!string.IsNullOrEmpty(txtBidNumber.Text))
                    ValidBidNumber();

                if (medicine.BID_ID != null)
                {
                    ValidBidPackage(true);
                    ValidYearQD(true);
                }
                else
                {
                    ValidBidPackage(false);
                    ValidYearQD(false);
                    lciPackBid.AppearanceItemCaption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
                    layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
                }

                ValidBidControlMaxlength(txtGroupBid, 4);
                //ValidBidControlMaxlength(txtPackBid, 4);
                //ValidBidControlMaxlength(txtBidYear, 20);
                ValidBidControlMaxlength(txtBidNumber, 50, false);
                ValidBidControlMaxlength(txtMaHoatChatBHYT, 500, false);
                ValidBidControlMaxlength(txtConcentra, 1000, false);
                ValidBidControlMaxlength(txtHeinServiceBHYTName, 1500);
                ValidBidControlMaxlength(txtTenHoatChatBHYT, 1000, false);
                ValidBidControlMaxlength(txtBidExtraCode, 50, false);
                ValidBidControlMaxlength(txtTTThau, 50, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidExpriedDate()
        {
            try
            {
                ExpiredDateValidationRule ExpriedDateValidationRule = new ExpiredDateValidationRule();
                ExpriedDateValidationRule.dtExpiredDate = dtExpiredDate;
                ExpriedDateValidationRule.ErrorText = Resources.ResourceMessage.NguoiDungNhapNgayKhongHopLe;
                ExpriedDateValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(dtExpiredDate, ExpriedDateValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidImpVatRatio()
        {
            try
            {
                ImpVatRatioValidationRule ImpVatRatioValidationRule = new ImpVatRatioValidationRule();
                ImpVatRatioValidationRule.spinImpVatRatio = spinImpVatRatio;
                //ImpVatRatioValidationRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                ImpVatRatioValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(spinImpVatRatio, ImpVatRatioValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidImpPrice()
        {
            try
            {
                ImpPriceValidationRule impPriceValidationRule = new ImpPriceValidationRule();
                impPriceValidationRule.spinImpPrice = spinImpPrice;
                //impPriceValidationRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                impPriceValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(spinImpPrice, impPriceValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidBidPackage(bool check)
        {
            try
            {
                BidPackageValidate bidPackageValidate = new BidPackageValidate();
                bidPackageValidate.check = check;
                bidPackageValidate.txtBidPackage = txtPackBid;
                bidPackageValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(txtPackBid, bidPackageValidate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBidNumber()
        {
            try
            {
                BidNumberValidationRule bidNumberValidationRule = new BidNumberValidationRule();
                bidNumberValidationRule.txtBidNumber = txtBidNumber;
                bidNumberValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(txtBidNumber, bidNumberValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidYearQD(bool check)
        {
            try
            {
                BidYearValidationRule singleControl = new BidYearValidationRule();
                singleControl.check = check;
                singleControl.txtBidYear = txtBidYear;
                singleControl.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(txtBidYear, singleControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        private void txtBidNumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBidYear.Focus();
                    txtBidYear.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBidYear_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lciBBGN.Enabled == true)
                    {
                        chkBBGN.Focus();
                    }
                    else
                    {
                        btnSave.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void chkBBGN_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void chkBBGN_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaHoatChatBHYT.Focus();
                    txtMaHoatChatBHYT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaHoatChatBHYT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTenHoatChatBHYT.Focus();
                    txtTenHoatChatBHYT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTenHoatChatBHYT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtConcentra.Focus();
                    txtConcentra.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Public method

        #endregion

        private void ValidBidControlMaxlength(DevExpress.XtraEditors.TextEdit control, int maxlength)
        {
            try
            {
                Validation.BidMaxLengthValidationRule _rule = new Validation.BidMaxLengthValidationRule();
                _rule.txtBid = control;
                _rule.maxlength = maxlength;
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(control, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBidControlMaxlength(DevExpress.XtraEditors.TextEdit control, int maxlength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule _rule = new ControlMaxLengthValidationRule();
                _rule.editor = control;
                _rule.maxLength = maxlength;
                _rule.IsRequired = IsRequired;
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(control, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpProfit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpVatRatio.Focus();
                    spinImpVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                bool success = false;
                bool isCalledApi = false;
                btnSave.Focus();
                positionHandle = -1;
                if (!dxValidationProvider.Validate())
                    return;

                //WaitingManager.Show();
                HisMedicineSDO sdo = new HisMedicineSDO();
                HIS_MEDICINE hisMedicine = GetDataSave();
                if (hisMedicine == null) return;

                sdo.HisMedicine = hisMedicine;
                if (chkUpdateAll.Checked)
                    sdo.IsUpdateAll = true;
                else
                    sdo.IsUpdateUnlock = true;

                if (string.IsNullOrEmpty(txtMaHoatChatBHYT.Text) || string.IsNullOrEmpty(txtConcentra.Text) || string.IsNullOrEmpty(txtHeinServiceBHYTName.Text))
                {
                    List<string> isNull = new List<string>();

                    if (string.IsNullOrEmpty(txtMaHoatChatBHYT.Text))
                    {
                        isNull.Add("Mã Hoạt Chất BHYT");
                    }
                    if (string.IsNullOrEmpty(txtConcentra.Text))
                    {
                        isNull.Add("Hàm Lượng");
                    }
                    if (string.IsNullOrEmpty(txtHeinServiceBHYTName.Text))
                    {
                        isNull.Add("Tên BHYT");
                    }

                    if (MessageBox.Show("Bạn chưa nhập thông tin " + string.Join(", ", isNull) + ". Bạn có chắc muốn lưu không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<HIS_MEDICINE>("api/HisMedicine/UpdateSdo", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        isCalledApi = true;
                        if (apiresul != null)
                        {
                            success = true;
                            this.Close();
                        }

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(txtMaHoatChatBHYT.Text))
                        {
                            txtMaHoatChatBHYT.Focus();
                        }
                        else if (string.IsNullOrEmpty(txtConcentra.Text))
                        {
                            txtConcentra.Focus();
                        }
                        else if (string.IsNullOrEmpty(txtHeinServiceBHYTName.Text))
                        {
                            txtHeinServiceBHYTName.Focus();
                        }
                    }
                }
                else
                {
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<HIS_MEDICINE>("api/HisMedicine/UpdateSdo", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    isCalledApi = true;
                    if (apiresul != null)
                    {
                        success = true;
                        this.Close();
                    }

                }


                //WaitingManager.Hide();
                #region Show message
                if (isCalledApi)
                    MessageManager.Show(this, paramCommon, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                //Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void txtConcentra_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinServiceBHYTName.Focus();
                    txtHeinServiceBHYTName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinServiceBHYTName_KeyDown(object sender, KeyEventArgs e)
        {
       
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinServiceBHYTName.Focus();
                    txtHeinServiceBHYTName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpSource_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboImpSource.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTTThau_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
