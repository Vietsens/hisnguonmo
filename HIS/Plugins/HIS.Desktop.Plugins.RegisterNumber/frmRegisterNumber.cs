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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterNumber
{
    public partial class frmRegisterNumber : HIS.Desktop.Utility.FormBase
    {
        internal List<HIS_REGISTER_GATE> currentRegisterGate { get; set; }
        internal V_HIS_CARD currentHisCard { get; set; }
        int positionHandleControlInfo = -1;
        internal Inventec.Desktop.Common.Modules.Module currentModule;

        public frmRegisterNumber(Inventec.Desktop.Common.Modules.Module currentModule)
		:base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmRegisterNumber_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                SetIconFrm();

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                ValidateBedForm();
                LoadDataRegisterGate();
                dtDateTime.EditValue = DateTime.Now;
                txtCardCode.Focus();
                btnSave.Enabled = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RegisterNumber.Resources.Lang", typeof(HIS.Desktop.Plugins.RegisterNumber.frmRegisterNumber).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRegisterName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmRegisterNumber.cboRegisterName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDoor.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.lciDoor.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCardCode.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.lciCardCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCardNumber.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.lciHeinCardNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDate.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.lciDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientNumber.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.lciPatientNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAdress.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.lciAdress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumberSTT.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.lciNumberSTT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDOB.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.lciDOB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGender.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.lciGender.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmRegisterNumber.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemNew.Caption = Inventec.Common.Resource.Get.Value("frmRegisterNumber.barButtonItemNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmRegisterNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataRegisterGate()
        {
            try
            {
                CommonParam param = new CommonParam();
                currentRegisterGate = new List<HIS_REGISTER_GATE>();
                MOS.Filter.HisRegisterGateFilter filter = new MOS.Filter.HisRegisterGateFilter();
                currentRegisterGate = new BackendAdapter(param).Get<List<HIS_REGISTER_GATE>>(HisRequestUriStore.HIS_REGISTER_GATE_GET, ApiConsumers.MosConsumer, filter, param);
                if (currentRegisterGate != null && currentRegisterGate.Count > 0)
                {
                    FillDataCboRegisterGate(cboRegisterName, currentRegisterGate);
                    cboRegisterName.EditValue = currentRegisterGate[0].ID;
                    txtRegisterCode.Text = currentRegisterGate[0].REGISTER_GATE_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataCboRegisterGate(DevExpress.XtraEditors.LookUpEdit cboRegisterGate, object data)
        {
            try
            {
                cboRegisterGate.Properties.DataSource = data;
                cboRegisterGate.Properties.DisplayMember = "REGISTER_GATE_NAME";
                cboRegisterGate.Properties.ValueMember = "ID";
                cboRegisterGate.Properties.ForceInitialize();
                cboRegisterGate.Properties.Columns.Clear();
                cboRegisterGate.Properties.Columns.Add(new LookUpColumnInfo("REGISTER_GATE_CODE", "", 50));
                cboRegisterGate.Properties.Columns.Add(new LookUpColumnInfo("REGISTER_GATE_NAME", "", 200));
                cboRegisterGate.Properties.ShowHeader = false;
                cboRegisterGate.Properties.ImmediatePopup = true;
                cboRegisterGate.Properties.DropDownRows = 10;
                cboRegisterGate.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRegisterCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadRegisterGateCombo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadRegisterGateCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboRegisterName.EditValue = null;
                    cboRegisterName.Focus();
                    cboRegisterName.ShowPopup();
                }
                else
                {
                    var data = currentRegisterGate.Where(o => o.REGISTER_GATE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboRegisterName.EditValue = data[0].ID;
                            txtRegisterCode.Text = data[0].REGISTER_GATE_CODE;
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.REGISTER_GATE_CODE == searchCode);
                            if (search != null)
                            {
                                cboRegisterName.EditValue = search.ID;
                                txtRegisterCode.Text = search.REGISTER_GATE_CODE;
                            }
                            else
                            {
                                cboRegisterName.EditValue = null;
                                cboRegisterName.Focus();
                                cboRegisterName.ShowPopup();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRegisterName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRegisterName.Text != null)
                    {
                        var rs = currentRegisterGate.Where(p => p.ID == (long)cboRegisterName.EditValue).FirstOrDefault();
                        if (rs != null)
                        {
                            txtRegisterCode.Text = rs.REGISTER_GATE_CODE;
                            dtDateTime.Focus();
                            dtDateTime.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRegisterName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboRegisterName.EditValue != null)
                    {
                        var rs = currentRegisterGate.Where(p => p.ID == (long)cboRegisterName.EditValue).FirstOrDefault();
                        if (rs != null)
                        {
                            txtRegisterCode.Text = rs.REGISTER_GATE_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCardCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    GetDataByCardCode(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataByCardCode(string card_code)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                if (String.IsNullOrEmpty(card_code))
                {
                    MessageBox.Show("Vui lòng nhập số thẻ", "Thông báo");
                    txtCardCode.Focus();
                    txtCardCode.SelectAll();
                    return;
                }
                else
                {
                    currentHisCard = new V_HIS_CARD();
                    currentHisCard = new BackendAdapter(param).Get<V_HIS_CARD>(HisRequestUriStore.HIS_CARD_GET_VIEW_BY_CODE, ApiConsumers.MosConsumer, card_code, param);
                    if (currentHisCard != null)
                    {
                        //ReloadFrm
                        FillInfoPatient(currentHisCard);
                        btnSave.Enabled = true;
                        txtRegisterCode.Focus();
                        txtRegisterCode.SelectAll();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin của số thẻ: " + card_code + "\n Vui lòng nhập lại", "Thông báo");
                        txtCardCode.Focus();
                        txtCardCode.SelectAll();
                        return;
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillInfoPatient(V_HIS_CARD data)
        {
            try
            {
                if (data != null)
                {
                    lblPatientName.Text = data.VIR_PATIENT_NAME;
                    lblGender.Text = data.GENDER_NAME;
                    lblAdress.Text = data.VIR_ADDRESS;
                    lblDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                    lblNumber.Text = null;
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
                this.positionHandleControlInfo = -1;
                if (!dxValidationProvider1.Validate())
                    return;


                CommonParam param = new CommonParam();
                bool success = false;
                HIS_REGISTER_REQ input = new HIS_REGISTER_REQ();
                if (cboRegisterName.EditValue != null)
                {
                    input.REGISTER_GATE_ID = (long)cboRegisterName.EditValue;
                }
                input.REGISTER_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtDateTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (currentHisCard != null)
                {
                    input.SERVICE_CODE = currentHisCard.SERVICE_CODE;
                    input.PATIENT_ID = currentHisCard.PATIENT_ID;
                }
                input.CARD_CODE = txtCardCode.Text;
                var rs = new BackendAdapter(param).Post<HIS_REGISTER_REQ>(HisRequestUriStore.HIS_REGISTER_REQ_CREATE, ApiConsumers.MosConsumer, input, param);
                if (rs != null)
                {
                    success = true;
                    lblNumber.Text = rs.NUM_ORDER.ToString();
                    btnSave.Enabled = false;
                }
                #region Show message
                MessageManager.Show(this.ParentForm,param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                txtPatientCode.Text = null;
                txtHeinCardNumber.Text = null;
                txtCardCode.Text = null;
                if (currentRegisterGate != null && currentRegisterGate.Count > 0)
                {
                    cboRegisterName.EditValue = currentRegisterGate[0].ID;
                    txtRegisterCode.Text = currentRegisterGate[0].REGISTER_GATE_CODE;
                }
                else
                {
                    cboRegisterName.EditValue = null;
                    txtRegisterCode.Text = null;
                }
                dtDateTime.EditValue = DateTime.Now;
                lblAdress.Text = null;
                lblDOB.Text = null;
                lblGender.Text = null;
                lblPatientName.Text = null;
                lblNumber.Text = null;

                btnSave.Enabled = false;
                txtCardCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItemNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlInfo == -1)
                {
                    positionHandleControlInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlInfo > edit.TabIndex)
                {
                    positionHandleControlInfo = edit.TabIndex;
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

        private void ValidateBedForm()
        {

            ValidateLookupWithTextEdit(cboRegisterName, txtRegisterCode);
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
