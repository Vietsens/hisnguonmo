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
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.MedicineTypeCreate.Validtion;
using DevExpress.XtraEditors;
using DevExpress.Data;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using System.Globalization;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.Popup
{
    public partial class frmDieuChinhLieu : Form
    {
        #region Delare
        long? currentMedicineTypeId;
        Inventec.Desktop.Common.Modules.Module module;
        List<HIS_ICD> listIcd;
        List<HIS_SERVICE> listHisService;
        List<HIS_TEST_INDEX> listHisTestIndex;
        List<HIS_MEDICINE_SERVICE> listMedinceServiceCnThan;
        List<HIS_MEDICINE_SERVICE> listMedinceServiceDVXN;
        private string[] icdSeparators = new string[] { ";" };

        #endregion
        #region ConStructor Load
        public frmDieuChinhLieu(Inventec.Desktop.Common.Modules.Module _Module, long? medicineTypeId)
        {
            InitializeComponent();
            try
            {
                this.module = _Module;
                this.currentMedicineTypeId = medicineTypeId;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void frmDieuChinhLieu_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
                InitDataComboHisService();
                InitDataComboHisTestIndex();
                xtraTabControl1.SelectedTabPageIndex = 0;
                SetDefaultValueControl();
                FillDataToGridControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                chkeGFR.Checked = true;
                cboService.EditValue = null;
                cboTestIndex.EditValue = null;
                txtValueServiceFrom.Text = null;
                txtValueServiceTo.Text = null;
                txtAmountIndayFrom.Text = null;
                txtAmountIndayTo.Text = null;
                mmWarningContent.Text = null;
                txtIcdCode.Text = null;
                txtIcdName.Text = null;
                cboService2.EditValue = null;
                cboTestIndex2.EditValue = null;
                txtValueServiceFrom2.Text = null;
                txtValueServiceTo2.Text = null;
                txtAmountIndayFrom2.Text = null;
                txtAmountIndayTo2.Text = null;
                mmWarningContent2.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ResetForm()
        {
            try
            {
                txtValueServiceFrom.Text = null;
                txtValueServiceTo.Text = null;
                txtAmountIndayFrom.Text = null;
                txtAmountIndayTo.Text = null;
                mmWarningContent.Text = null;
                txtIcdCode.Text = null;
                txtIcdName.Text = null;
                cboService2.EditValue = null;
                cboTestIndex2.EditValue = null;
                txtValueServiceFrom2.Text = null;
                txtValueServiceTo2.Text = null;
                txtAmountIndayFrom2.Text = null;
                txtAmountIndayTo2.Text = null;
                mmWarningContent2.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #region LoadData
        private void LoadData()
        {
            try
            {
                listIcd = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                this.listHisService = BackendDataWorker.Get<HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();
                this.listHisTestIndex = BackendDataWorker.Get<HIS_TEST_INDEX>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #region Initcombo

        private void InitDataComboHisService()
        {
            try
            {
                if (listHisService != null && listHisService.Count > 0)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 150, 1));
                    columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 400);
                    ControlEditorLoader.Load(this.cboService, listHisService, controlEditorADO);
                    ControlEditorLoader.Load(this.cboService2, listHisService, controlEditorADO);
                    cboService.Properties.ImmediatePopup = true;
                    cboService2.Properties.ImmediatePopup = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitDataComboHisTestIndex()
        {
            try
            {
                if (listHisTestIndex != null && listHisService.Count > 0)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("TEST_INDEX_CODE", "", 150, 1));
                    columnInfos.Add(new ColumnInfo("TEST_INDEX_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("TEST_INDEX_NAME", "ID", columnInfos, false, 400);
                    ControlEditorLoader.Load(this.cboTestIndex, listHisTestIndex, controlEditorADO);
                    ControlEditorLoader.Load(this.cboTestIndex2, listHisTestIndex, controlEditorADO);
                    cboTestIndex.Properties.ImmediatePopup = true;
                    cboTestIndex2.Properties.ImmediatePopup = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region method
        private void FillDataToGridControl()
        {
            try
            {
                if (this.currentMedicineTypeId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisMedicineServiceFilter filter = new HisMedicineServiceFilter();
                    filter.MEDICINE_TYPE_ID = this.currentMedicineTypeId;
                    var data = new BackendAdapter(param).Get<List<HIS_MEDICINE_SERVICE>>("api/HisMedicineService/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (data != null)
                    {
                        this.listMedinceServiceCnThan = data.Where(o => o.DATA_TYPE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__EGFR || o.DATA_TYPE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__CRCL).ToList();
                        this.listMedinceServiceDVXN = data.Where(o => o.DATA_TYPE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE).ToList();
                        if (this.listMedinceServiceCnThan != null && listMedinceServiceCnThan.Count > 0)
                        {
                            HIS_MEDICINE_SERVICE medicineServiceCnThan = listMedinceServiceCnThan.First();
                            chkCrCl.Checked = medicineServiceCnThan != null && medicineServiceCnThan.DATA_TYPE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__CRCL;
                            cboTestIndex.EditValue = medicineServiceCnThan.TEST_INDEX_ID;
                        }

                    }
                }
                gridControlWarning.BeginUpdate();
                gridControlWarning.DataSource = listMedinceServiceCnThan;
                gridControlWarning.EndUpdate();

                gridControlWarning2.BeginUpdate();
                gridControlWarning2.DataSource = listMedinceServiceDVXN;
                gridControlWarning2.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal bool ShowPopupIcdChoose()
        {
            try
            {
                WaitingManager.Show();
                frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdCode.Text, this.txtIcdName.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, listIcd);
                WaitingManager.Hide();
                FormSecondaryIcd.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }
        private bool CheckIcdWrongCode()
        {
            bool valid = true;
            try
            {
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                if (!String.IsNullOrEmpty(this.txtIcdCode.Text.Trim()))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    List<string> lstIcdSubName = new List<string>();
                    List<string> lstIcdCodes = new List<string>();
                    string[] arrIcdExtraCodes = this.txtIcdCode.Text.Trim().Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = listIcd.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.Trim().ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                                lstIcdCodes.Add(icdByCode.ICD_CODE);
                                lstIcdSubName.Add(icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode.Trim());
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode.Trim());
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        if (lstIcdCodes != null && lstIcdCodes.Count > 0)
                        {
                            this.txtIcdCode.Text = String.Join(";", lstIcdCodes);
                            this.txtIcdName.Text = String.Join(";", lstIcdSubName);
                        }
                        else
                        {
                            this.txtIcdCode.Text = null;
                            this.txtIcdName.Text = null;
                        }
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            valid = false;
                            XtraMessageBox.Show(String.Format("Mã ICD sau không tồn tại hoặc sai định dạng: {0}. Phần mềm chỉ nhận danh sách các mã ICD ngăn cách nhau bởi dấu chấm phẩy. Vui lòng kiểm tra lại", string.Join(",", arrWrongCodes)), "Thông báo", MessageBoxButtons.OK);
                            ShowPopupIcdChoose();
                        }
                    }
                    else
                    {
                        this.txtIcdCode.Text = null;
                        this.txtIcdName.Text = null;
                    }
                }
                else
                {
                    this.txtIcdCode.Text = null;
                    this.txtIcdName.Text = null;
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        #endregion
        #region validate
        private void ValidateIcd()
        {
            try
            {
                BenhPhuValidationRule mainRule = new BenhPhuValidationRule();
                mainRule.maBenhPhuTxt = txtIcdCode;
                mainRule.tenBenhPhuTxt = txtIcdName;
                mainRule.ErrorType = ErrorType.Warning;
                mainRule.listIcd = this.listIcd;
                this.dxValidationProviderDieuChinhLieu.SetValidationRule(txtIcdCode, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateTestIndex(GridLookUpEdit cbo)
        {
            try
            {
                ValidateCombox vali = new ValidateCombox();
                vali.gridLockup = cbo;
                vali.ErrorType = ErrorType.Warning;
                vali.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                dxValidationProviderDieuChinhLieu.SetValidationRule(cbo, vali);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetValidWhenAddTab2()
        {
            try
            {
                ValidateTestIndex(cboTestIndex2);
                ValidateIcd();
                ValidMaxlengthTxtWarningContent(mmWarningContent2, 4000);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void RemoveControlErrorTab2()
        {
            try
            {
                dxValidationProviderDieuChinhLieu.RemoveControlError(txtIcdCode);
                dxValidationProviderDieuChinhLieu.RemoveControlError(txtIcdName);
                dxValidationProviderDieuChinhLieu.RemoveControlError(cboTestIndex2);
                dxValidationProviderDieuChinhLieu.RemoveControlError(mmWarningContent2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidMaxlengthTxtWarningContent(DevExpress.XtraEditors.BaseEdit baseEdit, int maxlength)
        {
            try
            {
                ValidateMaxLengthBaseEdit validateMaxLength = new ValidateMaxLengthBaseEdit();
                validateMaxLength.baseEdit = baseEdit;
                validateMaxLength.maxLength = maxlength;
                dxValidationProviderDieuChinhLieu.SetValidationRule(baseEdit, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
        #region Event
        private void bbtnItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<HIS_MEDICINE_SERVICE> listMedicineService = new List<HIS_MEDICINE_SERVICE>();
                if (listMedinceServiceCnThan != null && listMedinceServiceCnThan.Count > 0)
                {
                    if (cboTestIndex.EditValue == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn chỉ số xét nghiệm", "Thông báo", MessageBoxButtons.OK);
                        return;
                    }
                    else
                    {
                        long testIndexId = (long)cboTestIndex.EditValue;
                        if (chkeGFR.Checked)
                        {
                            listMedinceServiceCnThan.ForEach(o => { o.MEDICINE_TYPE_ID = currentMedicineTypeId; o.DATA_TYPE = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__EGFR; o.TEST_INDEX_ID = testIndexId; });
                        }
                        else
                        {
                            listMedinceServiceCnThan.ForEach(o => { o.MEDICINE_TYPE_ID = currentMedicineTypeId; o.DATA_TYPE = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__CRCL; o.TEST_INDEX_ID = testIndexId; });
                        }
                    }
                    listMedicineService.AddRange(listMedinceServiceCnThan);
                }
                if (listMedinceServiceDVXN != null && listMedinceServiceDVXN.Count > 0)
                {
                    listMedinceServiceDVXN.ForEach(o => { o.MEDICINE_TYPE_ID = currentMedicineTypeId; o.DATA_TYPE = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE; });
                    listMedicineService.AddRange(listMedinceServiceDVXN);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listMedicineService), listMedicineService));
                if (listMedicineService != null && listMedicineService.Count > 0)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    var resultData = new BackendAdapter(param).Post<List<HIS_MEDICINE_SERVICE>>(
                   "api/HisMedicineService/CreateOrUpdate", ApiConsumers.MosConsumer, listMedicineService, param);
                    if (resultData != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<HIS_MEDICINE_SERVICE>();
                        ResetForm();
                        listMedinceServiceCnThan = resultData.Where(o => o.DATA_TYPE != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE).ToList();
                        listMedinceServiceDVXN = resultData.Where(o => o.DATA_TYPE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE).ToList();
                        gridControlWarning.BeginUpdate();
                        gridControlWarning.DataSource = listMedinceServiceCnThan;
                        gridControlWarning.EndUpdate();
                        gridControlWarning2.BeginUpdate();
                        gridControlWarning2.DataSource = listMedinceServiceDVXN;
                        gridControlWarning2.EndUpdate();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdCode.Text, this.txtIcdName.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, listIcd);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void stringIcds(string icdCode, string icdName)
        {
            try
            {
                txtIcdCode.Text = icdCode;
                txtIcdName.Text = icdName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboService.EditValue != null)
                {
                    cboService.Properties.Buttons[1].Visible = true;
                    if (cboService.EditValue != cboService.OldEditValue)
                    {
                        var lstTestIndex = listHisTestIndex.Where(o => o.TEST_SERVICE_TYPE_ID == (long)cboService.EditValue);
                        if (lstTestIndex != null)
                            cboTestIndex.Properties.DataSource = lstTestIndex.ToList();
                        else
                            cboTestIndex.Properties.DataSource = null;
                    }
                }
                else
                {
                    cboTestIndex.Properties.DataSource = listHisTestIndex;
                    cboService.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboService_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboService.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ValidMaxlengthTxtWarningContent(mmWarningContent, 4000);
                RemoveControlErrorTab2();
                dxValidationProviderDieuChinhLieu.RemoveControlError(cboTestIndex);
                if (!dxValidationProviderDieuChinhLieu.Validate())
                    return;
                string message = "";
                if (String.IsNullOrEmpty(txtValueServiceFrom.Text) && String.IsNullOrEmpty(txtValueServiceTo.Text))
                {
                    message += "Bạn chưa nhập thông tin Chỉ số chức năng thận. ";
                }
                else if (!String.IsNullOrEmpty(txtValueServiceFrom.Text) && !String.IsNullOrEmpty(txtValueServiceTo.Text) && ConvertValue(txtValueServiceFrom.Text) > ConvertValue(txtValueServiceTo.Text))
                {
                    message += "Chỉ số chức năng thận từ không được lớn hơn Chỉ số chức năng thận đến. ";
                }
                if (String.IsNullOrEmpty(txtAmountIndayFrom.Text) && String.IsNullOrEmpty(txtAmountIndayTo.Text))
                {
                    message += "Bạn chưa nhập thông tin Số lượng thuốc đã kê trong ngày. ";
                }
                else if (!String.IsNullOrEmpty(txtAmountIndayFrom.Text) && !String.IsNullOrEmpty(txtAmountIndayTo.Text) && ConvertValue(txtAmountIndayFrom.Text) > ConvertValue(txtAmountIndayTo.Text))
                {
                    message += "Số lượng thuốc đã kê trong ngày từ không được lớn hơn Số lượng thuốc đã kê trong ngày đến. ";
                }
                if (String.IsNullOrEmpty(mmWarningContent.Text))
                {
                    message += "Bạn chưa nhập nội dung cảnh báo. ";
                }
                if (!String.IsNullOrEmpty(message))
                    XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                else
                {
                    HIS_MEDICINE_SERVICE medicineService = new HIS_MEDICINE_SERVICE();
                    medicineService.VALUE_SERVICE_FROM = ConvertValue(txtValueServiceFrom.Text);
                    medicineService.VALUE_SERVICE_TO = ConvertValue(txtValueServiceTo.Text);
                    medicineService.AMOUNT_INDAY_FROM = ConvertValue(txtAmountIndayFrom.Text);
                    medicineService.AMOUNT_INDAY_TO = ConvertValue(txtAmountIndayTo.Text);
                    medicineService.WARNING_CONTENT = mmWarningContent.Text;
                    listMedinceServiceCnThan.Add(medicineService);
                    gridControlWarning.BeginUpdate();
                    gridControlWarning.DataSource = listMedinceServiceCnThan;
                    gridControlWarning.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private decimal? ConvertValue(string txtValue)
        {
            decimal? amountTmp = null;
            try
            {
                if (!String.IsNullOrEmpty(txtValue))
                {
                    CultureInfo culture = new CultureInfo("en-US");
                    if (txtValue.Contains(","))
                        culture = new CultureInfo("fr-FR");
                    amountTmp = Convert.ToDecimal(txtValue, culture);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return amountTmp;
            }
            return amountTmp;
        }

        private void btnAdd2_Click(object sender, EventArgs e)
        {
            try
            {
                SetValidWhenAddTab2();
                dxValidationProviderDieuChinhLieu.RemoveControlError(mmWarningContent);
                dxValidationProviderDieuChinhLieu.RemoveControlError(cboTestIndex);
                if (!dxValidationProviderDieuChinhLieu.Validate())
                    return;
                string message = "";
                if (String.IsNullOrEmpty(txtValueServiceFrom2.Text) && String.IsNullOrEmpty(txtValueServiceTo2.Text))
                {
                    message += "Bạn chưa nhập thông tin Kết quả xét nghiệm. ";
                }
                else if (!String.IsNullOrEmpty(txtValueServiceFrom2.Text) && !String.IsNullOrEmpty(txtValueServiceTo2.Text) && ConvertValue(txtValueServiceFrom2.Text) > ConvertValue(txtValueServiceTo2.Text))
                {
                    message += "Kết quả xét nghiệm từ không được lớn hơn Kết quả xét nghiệm đến. ";
                }
                if (String.IsNullOrEmpty(txtAmountIndayFrom2.Text) && String.IsNullOrEmpty(txtAmountIndayTo2.Text))
                {
                    message += "Bạn chưa nhập thông tin Số lượng thuốc đã kê trong ngày. ";
                }
                else if (!String.IsNullOrEmpty(txtAmountIndayFrom2.Text) && !String.IsNullOrEmpty(txtAmountIndayTo2.Text) && ConvertValue(txtAmountIndayFrom2.Text) > ConvertValue(txtAmountIndayTo2.Text))
                {
                    message += "Số lượng thuốc đã kê trong ngày từ không được lớn hơn Số lượng thuốc đã kê trong ngày đến. ";
                }
                if (String.IsNullOrEmpty(mmWarningContent2.Text))
                {
                    message += "Bạn chưa nhập nội dung cảnh báo. ";
                }
                if (!String.IsNullOrEmpty(message))
                    XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                else
                {
                    HIS_MEDICINE_SERVICE medicineService = new HIS_MEDICINE_SERVICE();
                    medicineService.ICD_CODE = txtIcdCode.Text;
                    medicineService.ICD_NAME = txtIcdName.Text;
                    medicineService.TEST_INDEX_ID = (long)cboTestIndex2.EditValue;
                    medicineService.VALUE_SERVICE_FROM = ConvertValue(txtValueServiceFrom2.Text);
                    medicineService.VALUE_SERVICE_TO = ConvertValue(txtValueServiceTo2.Text);
                    medicineService.AMOUNT_INDAY_FROM = ConvertValue(txtAmountIndayFrom2.Text);
                    medicineService.AMOUNT_INDAY_TO = ConvertValue(txtAmountIndayTo2.Text);
                    medicineService.WARNING_CONTENT = mmWarningContent2.Text;
                    listMedinceServiceDVXN.Add(medicineService);
                    gridControlWarning2.BeginUpdate();
                    gridControlWarning2.DataSource = listMedinceServiceDVXN;
                    gridControlWarning2.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewWarning_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (HIS_MEDICINE_SERVICE)gridViewWarning.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    if (rowData != null)
                    {
                        if (rowData.ID > 0)
                        {
                            bool success = false;
                            CommonParam param = new CommonParam();
                            success = new BackendAdapter(param).Post<bool>("api/HisMedicineService/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                            if (success)
                            {
                                BackendDataWorker.Reset<HIS_MEDICINE_SERVICE>();
                                listMedinceServiceCnThan.Remove(rowData);
                            }
                            MessageManager.Show(this, param, success);
                        }
                        else
                        {
                            listMedinceServiceCnThan.Remove(rowData);
                        }
                        gridControlWarning.BeginUpdate();
                        gridControlWarning.DataSource = listMedinceServiceCnThan;
                        gridControlWarning.EndUpdate();
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkeGFR_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkeGFR.Checked)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdCode.Text, this.txtIcdName.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, listIcd);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!CheckIcdWrongCode())
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboService2_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboService2.EditValue != null)
                {
                    cboService2.Properties.Buttons[1].Visible = true;
                    if (cboService2.EditValue != cboService2.OldEditValue)
                    {
                        var lstTestIndex = listHisTestIndex.Where(o => o.TEST_SERVICE_TYPE_ID == (long)cboService2.EditValue);
                        if (lstTestIndex != null)
                            cboTestIndex2.Properties.DataSource = lstTestIndex.ToList();
                        else
                            cboTestIndex2.Properties.DataSource = null;
                    }
                }
                else
                {
                    cboTestIndex2.Properties.DataSource = listHisTestIndex;
                    cboService2.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboService2_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboService.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewWarning2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var data = (HIS_MEDICINE_SERVICE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "TEST_INDEX_NAME")
                    {
                        var testIndex = listHisTestIndex.FirstOrDefault(o => o.ID == data.TEST_INDEX_ID);
                        if (testIndex != null)
                            e.Value = testIndex.TEST_INDEX_NAME;
                        else
                            e.Value = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDelete2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (HIS_MEDICINE_SERVICE)gridViewWarning2.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    if (rowData != null)
                    {
                        if (rowData.ID > 0)
                        {
                            bool success = false;
                            CommonParam param = new CommonParam();
                            success = new BackendAdapter(param).Post<bool>("api/HisMedicineService/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                            if (success)
                            {
                                BackendDataWorker.Reset<HIS_MEDICINE_SERVICE>();
                                listMedinceServiceDVXN.Remove(rowData);
                            }
                            MessageManager.Show(this, param, success);
                        }
                        else
                        {
                            listMedinceServiceDVXN.Remove(rowData);
                        }
                        gridControlWarning2.BeginUpdate();
                        gridControlWarning2.DataSource = listMedinceServiceDVXN;
                        gridControlWarning2.EndUpdate();
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnItemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    btnAdd_Click(null, null);
                }
                else
                {
                    btnAdd2_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboService.EditValue != null)
                    {
                        cboTestIndex.Focus();
                        cboTestIndex.ShowPopup();
                    }
                    else
                    {
                        cboService.Focus();
                        cboService.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestIndex_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTestIndex.EditValue != null)
                    {
                        txtValueServiceFrom.Focus();
                        txtValueServiceFrom.SelectAll();
                    }
                    else
                    {
                        cboTestIndex.Focus();
                        cboTestIndex.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtValueServiceFrom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtValueServiceTo.Focus();
                    txtValueServiceTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtValueServiceTo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAmountIndayFrom.Focus();
                    txtAmountIndayFrom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAmountIndayFrom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAmountIndayTo.Focus();
                    txtAmountIndayTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAmountIndayTo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    mmWarningContent.Focus();
                    mmWarningContent.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboService2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboService2.EditValue != null)
                    {
                        cboTestIndex2.Focus();
                        cboTestIndex2.ShowPopup();
                    }
                    else
                    {
                        cboService2.Focus();
                        cboService2.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestIndex2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTestIndex2.EditValue != null)
                    {
                        txtValueServiceFrom2.Focus();
                        txtValueServiceFrom2.SelectAll();
                    }
                    else
                    {
                        cboTestIndex2.Focus();
                        cboTestIndex2.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtValueServiceFrom2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtValueServiceTo2.Focus();
                    txtValueServiceTo2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtValueServiceTo2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAmountIndayFrom2.Focus();
                    txtAmountIndayFrom2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAmountIndayFrom2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAmountIndayTo2.Focus();
                    txtAmountIndayTo2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAmountIndayTo2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    mmWarningContent2.Focus();
                    mmWarningContent2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
