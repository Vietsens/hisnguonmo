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
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using HIS.Desktop.Plugins.MaterialUpdate.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.MaterialUpdate
{
    public partial class FormMaterialUpdate : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1 material;
        System.Globalization.CultureInfo cultureLang;
        int positionHandle = -1;
        #endregion

        #region Construct
        public FormMaterialUpdate(Inventec.Desktop.Common.Modules.Module moduleData)
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

        public FormMaterialUpdate(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1 data, Inventec.Desktop.Common.Modules.Module moduleData)
            : this(moduleData)
        {
            try
            {
                this.material = data;
                this.Text = moduleData.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormMaterialUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //icon
                SetIcon();
                LoadComboImpResource();
                LoadDataCboInformationBid();
                //Load du lieu
                FillData();

                //valid
                ValidControls();

                if (this.material != null)
                {
                    EnableBBGN(this.material.ID);
                }

                //Focus
                spinImpPrice.Focus();
                spinImpPrice.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void LoadKeysFromlanguage()
        {
            try
            {
                this.lciExpiredDate.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MATERIAL_UPDATE__LCI_EXPIRED_DATE",
                    Resources.ResourceLanguageManager.LanguageFormMaterialUpdate,
                    cultureLang);
                this.lciImpPrice.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MATERIAL_UPDATE__LCI_IMP_PRICE",
                    Resources.ResourceLanguageManager.LanguageFormMaterialUpdate,
                    cultureLang);
                this.lciImpVatRatio.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MATERIAL_UPDATE__LCI_IMP_VAT_RATIO",
                    Resources.ResourceLanguageManager.LanguageFormMaterialUpdate,
                    cultureLang);
                this.lciPackgeNumber.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MATERIAL_UPDATE__LCI_PACKGE_NUMBER",
                    Resources.ResourceLanguageManager.LanguageFormMaterialUpdate,
                    cultureLang);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MATERIAL_UPDATE__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageFormMaterialUpdate,
                    cultureLang);
                this.lciGroupBid.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MATERIAL_UPDATE__LCI_BID_GROUP_CODE",
                    Resources.ResourceLanguageManager.LanguageFormMaterialUpdate,
                    cultureLang);
                this.lciNumberBid.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MATERIAL_UPDATE__LCI_BID_NUM_ORDER",
                    Resources.ResourceLanguageManager.LanguageFormMaterialUpdate,
                    cultureLang);
                this.lciPackBid.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MATERIAL_UPDATE__LCI_BID_PACKAGE_CODE",
                    Resources.ResourceLanguageManager.LanguageFormMaterialUpdate,
                    cultureLang);
                this.lciBBGN.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MATERIAL_UPDATE__LCI_BBGN",
                    Resources.ResourceLanguageManager.LanguageFormMaterialUpdate,
                    cultureLang);
                this.lciBBGN.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_MATERIAL_UPDATE__LCI_BBGN_TOOLTIP",
                    Resources.ResourceLanguageManager.LanguageFormMaterialUpdate,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private void EnableBBGN(long materialId)
        {
            if (CheckMaterialExistExp(materialId))
            {
                lciBBGN.Enabled = false;

            }
            else
            {
                lciBBGN.Enabled = true;
            }
        }

        private bool CheckMaterialExistExp(long materialId)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMaterialFilter filter = new MOS.Filter.HisExpMestMaterialFilter();
                filter.MATERIAL_ID = materialId;
                var expMaterials = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (expMaterials != null && expMaterials.Count() > 0)
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

        private void FillData()
        {
            try
            {
                WaitingManager.Show();
                this.spinImpPrice.EditValue = material.IMP_PRICE;
                this.spinImpProfit.EditValue = material.PROFIT_RATIO * 100;
                this.spinImpVatRatio.EditValue = material.IMP_VAT_RATIO * 100;
                this.txtPackgeNumber.Text = material.PACKAGE_NUMBER;
                this.dtExpiredDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(material.EXPIRED_DATE ?? 0);
                this.txtNumberBid.Text = material.TDL_BID_NUM_ORDER;
                this.txtPackBid.Text = material.TDL_BID_PACKAGE_CODE;
                this.labelContractCode.Text = material.MEDICAL_CONTRACT_CODE;
                this.labelContractName.Text = material.MEDICAL_CONTRACT_NAME;
                this.txtGroupBid.Text = material.TDL_BID_GROUP_CODE;
                this.txtBidNumber.Text = material.TDL_BID_NUMBER;
                this.txtBidMaterialTypeCode.Text = material.BID_MATERIAL_TYPE_CODE;
                this.txtBidMaterialTypeName.Text = material.BID_MATERIAL_TYPE_NAME;
                this.txtBidExtraCode.Text = material.TDL_BID_EXTRA_CODE;
                this.txtBidYear.Text = material.TDL_BID_YEAR;
                this.txtTTThau.Text = material.TT_THAU;
                this.cboImpSource.EditValue = material.IMP_SOURCE_ID;
                this.cboInformationBid.EditValue = material.INFORMATION_BID;
                if (material.IS_SALE_EQUAL_IMP_PRICE == 1)
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

        private void LoadComboImpResource()
        {
            try
            {
                var listImpResource = BackendDataWorker.Get<HIS_IMP_SOURCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("IMP_SOURCE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("IMP_SOURCE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("IMP_SOURCE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboImpSource, listImpResource, controlEditorADO);
                cboImpSource.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboInformationBid()
        {
            try
            {
                List<ThongTinThau> listFilterType = new List<ThongTinThau>();
                listFilterType.Add(new ThongTinThau(1, "VT thường"));
                listFilterType.Add(new ThongTinThau(2, "VT áp thầu"));
                listFilterType.Add(new ThongTinThau(3, "VT tự sản xuất"));
                listFilterType.Add(new ThongTinThau(4, "vật tư trước ngày tt 14/2020"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("ID", "", 30, 1));
                columnInfos.Add(new ColumnInfo("FilterTypeName", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("FilterTypeName", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboInformationBid, listFilterType.ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        #region event
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                bool success = false;
                btnSave.Focus();
                positionHandle = -1;
                if (!dxValidationProvider.Validate())
                    return;

                var hisMaterial = GetDataSave();
                if (hisMaterial == null)
                    return;

                WaitingManager.Show();
                var apiresul = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<MOS.EFMODEL.DataModels.HIS_MATERIAL>(ApiConsumer.HisRequestUriStore.HIS_MATERIAL_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, hisMaterial, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiresul != null)
                {
                    success = true;
                    this.Close();
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this, paramCommon, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private HIS_MATERIAL GetDataSave()
        {
            HIS_MATERIAL result = new HIS_MATERIAL();
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MATERIAL>(result, material);
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

                result.PACKAGE_NUMBER = txtPackgeNumber.Text;
                result.TDL_BID_NUM_ORDER = txtNumberBid.Text;
                result.TDL_BID_PACKAGE_CODE = txtPackBid.Text;
                result.TDL_BID_GROUP_CODE = txtGroupBid.Text;
                result.TDL_BID_NUMBER = txtBidNumber.Text;
                result.BID_MATERIAL_TYPE_CODE = txtBidMaterialTypeCode.Text;
                result.BID_MATERIAL_TYPE_NAME = txtBidMaterialTypeName.Text;
                result.TDL_BID_YEAR = txtBidYear.Text;
                result.TDL_BID_EXTRA_CODE = txtBidExtraCode.Text;
                result.TT_THAU = txtTTThau.Text;
                if (cboImpSource.EditValue != null)
                    result.IMP_SOURCE_ID = (long)cboImpSource.EditValue;
                else
                    result.IMP_SOURCE_ID = null;
                if (chkBBGN.CheckState == CheckState.Checked)
                {
                    result.IS_SALE_EQUAL_IMP_PRICE = 1;
                }
                else
                {
                    result.IS_SALE_EQUAL_IMP_PRICE = null;
                }
                if (cboInformationBid.EditValue != null)
                {
                    result.INFORMATION_BID = long.Parse(cboInformationBid.EditValue.ToString() ?? "0");
                }
                else
                {
                    result.INFORMATION_BID = null;
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
                    txtNumberBid.Focus();
                    txtNumberBid.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNumberBid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtPackBid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtGroupBid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        #region validate
        private void ValidControls()
        {
            try
            {
                ValidImpPrice();
                ValidImpVatRatio();
                //ValidExpriedDate();
                if (material.BID_ID != null)
                {
                    ValidBidPackage(txtPackBid, true);
                    ValidYearQD(true);
                }
                else
                {
                    ValidBidPackage(txtPackBid, false);
                    ValidYearQD(false);
                    lciPackBid.AppearanceItemCaption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
                    layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
                }
                //ValidMaxlength(txtPackBid, 2, true);
                if (!string.IsNullOrEmpty(txtBidNumber.Text))
                    ValidBidNumber();
                ValidBidControlMaxlength(txtGroupBid, 2);
                ValidBidControlMaxlength(txtPackBid, 4);
                ValidBidControlMaxlength(txtBidYear, 20);
                ValidBidControlMaxlength(txtBidNumber, 50, false);
                ValidBidControlMaxlength(txtBidMaterialTypeCode, 50, false);
                ValidBidControlMaxlength(txtBidMaterialTypeName, 500, false);
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
                ImpVatRatioValidationRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
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
                impPriceValidationRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                impPriceValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(spinImpPrice, impPriceValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidBidPackage(TextEdit control, bool check)
        {
            try
            {
                BidPackageValidate bidPackageValidate = new BidPackageValidate();
                bidPackageValidate.check = check;
                bidPackageValidate.txtBidPackage = control;
                bidPackageValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(control, bidPackageValidate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidMaxlength(Control control, int maxLength, bool isRequired)
        {
            try
            {
                ControlMaxLengthValidationRule bidPackageValidate = new ControlMaxLengthValidationRule();
                bidPackageValidate.editor = control;
                bidPackageValidate.maxLength = maxLength;
                bidPackageValidate.IsRequired = isRequired;
                bidPackageValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(control, bidPackageValidate);
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
        private void ValidBidTypeCode()
        {
            try
            {
                BidNumberValidationRule bidNumberValidationRule = new BidNumberValidationRule();
            }
            catch (Exception ex)
            {

                throw;
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

        private void spinImpPrice_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                if ((int)spinImpPrice.Value < 0)
                {
                    spinImpPrice.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpPrice_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (spinImpPrice.EditValue != null && (spinImpPrice.Value < 0 || spinImpPrice.Value > 1000000000000000000)) e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpVatRatio_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                if ((int)spinImpVatRatio.Value < 0)
                {
                    spinImpVatRatio.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpVatRatio_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (spinImpVatRatio.EditValue != null && (spinImpVatRatio.Value < 0 || spinImpVatRatio.Value > 100)) e.Cancel = true;
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
        #endregion

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
                        btnSave.Select();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkBBGN_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    btnSave.Select();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void spinImpProfit_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                if ((int)spinImpProfit.Value < 0)
                {
                    spinImpProfit.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpProfit_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (spinImpProfit.EditValue != null && (spinImpProfit.Value < 0 || spinImpProfit.Value > 1000000000000000000)) e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpSource_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboImpSource.Text.Trim() == "")
                {
                    cboImpSource.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboImpSource_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                cboImpSource.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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

        private void cboImpSource_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
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
