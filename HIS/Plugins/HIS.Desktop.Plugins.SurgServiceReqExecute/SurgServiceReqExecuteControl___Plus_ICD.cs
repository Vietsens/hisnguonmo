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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Resources;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Validate.ValidationRule;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class SurgServiceReqExecuteControl : UserControlBase
    {
        private async Task ComboMethodICD()
        {
            try
            {
                this.autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                chkIcd1.Enabled = (this.autoCheckIcd != 2);
                chkIcd2.Enabled = (this.autoCheckIcd != 2);
                chkIcd3.Enabled = (this.autoCheckIcd != 2);
                chkIcdCm.Enabled = (this.autoCheckIcd != 2);
                List<HIS_ICD> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_ICD>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_ICD>>("api/HisIcd/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_ICD), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                this.dataIcds = new List<HIS_ICD>();
                if (datas != null && datas.Count > 0)
                    this.dataIcds = datas.Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                DataToComboChuanDoanTD(cboIcd1);
                DataToComboChuanDoanTD(cboIcd2);
                DataToComboChuanDoanTD(cboIcd3);

                List<HIS_ICD_CM> dataCms = null;
                if (BackendDataWorker.IsExistsKey<HIS_ICD_CM>())
                {
                    dataCms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD_CM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataCms = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_ICD_CM>>("api/HisIcdCm/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (dataCms != null) BackendDataWorker.UpdateToRam(typeof(HIS_ICD_CM), dataCms, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                this.dataIcdCms = new List<HIS_ICD_CM>();
                if (dataCms != null && dataCms.Count > 0)
                    this.dataIcdCms = dataCms.Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                DataToComboChuanDoanTD(cboIcdCmName, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DataToComboChuanDoanTD(CustomGridLookUpEditWithFilterMultiColumn cbo, bool isIcdCM = false)
        {
            try
            {
                List<IcdADO> listADO = new List<IcdADO>();
                if (isIcdCM)
                {
                    foreach (var item in dataIcdCms)
                    {
                        IcdADO icd = new IcdADO();
                        icd.ID = item.ID;
                        icd.ICD_CODE = item.ICD_CM_CODE;
                        icd.ICD_NAME = item.ICD_CM_NAME;
                        icd.ICD_NAME_UNSIGN = convertToUnSign3(item.ICD_CM_NAME);
                        listADO.Add(icd);
                    }
                }
                else
                {
                    foreach (var item in dataIcds)
                    {
                        IcdADO icd = new IcdADO();
                        icd.ID = item.ID;
                        icd.ICD_CODE = item.ICD_CODE;
                        icd.ICD_NAME = item.ICD_NAME;
                        icd.ICD_NAME_UNSIGN = convertToUnSign3(item.ICD_NAME);
                        listADO.Add(icd);
                    }
                }

                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "ICD_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("ICD_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["ICD_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private void FillDataToCboIcd(
            TextEdit txtIcdCode,
            TextEdit txtIcdMain,
            CustomGridLookUpEditWithFilterMultiColumn cbo,
            CheckEdit chkEdit,
            string _ICD_CODE,
            string _ICD_NAME)
        {
            try
            {
                if (!string.IsNullOrEmpty(_ICD_CODE))
                {
                    var icd = this.dataIcds.Where(p => p.ICD_CODE == _ICD_CODE).FirstOrDefault();
                    if (icd != null)
                    {
                        txtIcdCode.Text = icd.ICD_CODE;
                        cbo.EditValue = icd.ID;
                        if (this.autoCheckIcd == 1
                            || (!String.IsNullOrEmpty(_ICD_NAME)
                            && (_ICD_NAME ?? "").Trim().ToLower() != (icd.ICD_NAME ?? "").Trim().ToLower()))
                        {
                            chkEdit.Checked = (this.autoCheckIcd != 2);
                            txtIcdMain.Text = _ICD_NAME;
                        }
                        else
                        {
                            chkEdit.Checked = false;
                            txtIcdMain.Text = icd.ICD_NAME;
                        }
                    }
                    //else
                    //{
                    //    txtIcdCode.Text = null;
                    //    cbo.EditValue = null;
                    //    txtIcdMain.Text = null;
                    //    chkEdit.Checked = false;
                    //}
                }
                //else
                //{
                //    txtIcdCode.Text = null;
                //    cbo.EditValue = null;
                //    txtIcdMain.Text = null;
                //    chkEdit.Checked = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCboIcdCm(
            TextEdit txtIcdCode,
            TextEdit txtIcdMain,
            CustomGridLookUpEditWithFilterMultiColumn cbo,
            CheckEdit chkEdit,
            string _ICD_CODE,
            string _ICD_NAME)
        {
            try
            {
                if (!string.IsNullOrEmpty(_ICD_CODE))
                {
                    var icd = this.dataIcdCms.Where(p => p.ICD_CM_CODE == _ICD_CODE).FirstOrDefault();
                    if (icd != null)
                    {
                        txtIcdCode.Text = icd.ICD_CM_CODE;
                        cbo.EditValue = icd.ID;
                        if (this.autoCheckIcd == 1
                            || (!String.IsNullOrEmpty(_ICD_NAME)
                            && (_ICD_NAME ?? "").Trim().ToLower() != (icd.ICD_CM_NAME ?? "").Trim().ToLower()))
                        {
                            chkEdit.Checked = (this.autoCheckIcd != 2);
                            txtIcdMain.Text = _ICD_NAME;
                        }
                        else
                        {
                            chkEdit.Checked = false;
                            txtIcdMain.Text = icd.ICD_CM_NAME;
                        }
                    }
                    //else
                    //{
                    //    txtIcdCode.Text = null;
                    //    cbo.EditValue = null;
                    //    txtIcdMain.Text = null;
                    //    chkEdit.Checked = false;
                    //}
                }
                //else
                //{
                //    txtIcdCode.Text = null;
                //    cbo.EditValue = null;
                //    txtIcdMain.Text = null;
                //    chkEdit.Checked = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ReadOnlyICD(bool isReadOnly, TextEdit txtIcdCode,
            TextEdit txtIcdMain,
            CustomGridLookUpEditWithFilterMultiColumn cbo,
            CheckEdit chkEdit)
        {
            try
            {
                if (isReadOnly)
                {
                    txtIcdCode.ReadOnly = true;
                    txtIcdMain.ReadOnly = true;
                    cbo.ReadOnly = true;
                    chkEdit.ReadOnly = true;
                    cbo.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    txtIcdCode.ReadOnly = false;
                    txtIcdMain.ReadOnly = false;
                    cbo.ReadOnly = false;
                    chkEdit.ReadOnly = false;
                    cbo.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void EnableICD(bool isEnable, TextEdit txtIcdCode,
           TextEdit txtIcdMain,
           CustomGridLookUpEditWithFilterMultiColumn cbo,
           CheckEdit chkEdit)
        {
            try
            {
                txtIcdCode.Enabled = isEnable;
                txtIcdMain.Enabled = isEnable;
                cbo.Enabled = isEnable;
                chkEdit.Enabled = isEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadIcdCombo(string searchCode,
            TextEdit txtIcdCode,
           TextEdit txtIcdMain,
           CustomGridLookUpEditWithFilterMultiColumn cbo,
           CheckEdit chkEdit)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = this.dataIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtIcdCode.Text = result.First().ICD_CODE;
                        txtIcdMain.Text = result.First().ICD_NAME;
                        cbo.EditValue = listData.First().ID;
                        chkEdit.Checked = (chkEdit.Enabled ? (this.autoCheckIcd == 1) : false);

                        if (chkEdit.Checked)
                        {
                            txtIcdMain.Focus();
                            txtIcdMain.SelectAll();
                        }
                        else
                        {
                            cbo.Focus();
                            cbo.SelectAll();
                        }

                        //if (this.refeshIcd != null)
                        //{
                        //    Inventec.Common.Logging.LogSystem.Debug("this.refeshIcd.execute");
                        //    this.refeshIcd(listData.First());
                        //}
                    }
                }

                if (showCbo)
                {
                    cbo.Focus();
                    cbo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadIcdCMCombo(string searchCode, TextEdit txtIcdCode, TextEdit txtIcdMain, CustomGridLookUpEditWithFilterMultiColumn cbo, CheckEdit chkEdit)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = this.dataIcdCms.Where(o => o.ICD_CM_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CM_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtIcdCode.Text = result.First().ICD_CM_CODE;
                        txtIcdMain.Text = result.First().ICD_CM_NAME;
                        cbo.EditValue = listData.First().ID;
                        chkEdit.Checked = (chkEdit.Enabled ? (this.autoCheckIcd == 1) : false);

                        if (chkEdit.Checked)
                        {
                            txtIcdMain.Focus();
                            txtIcdMain.SelectAll();
                        }
                        else
                        {
                            cbo.Focus();
                            cbo.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cbo.Focus();
                    cbo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ValidationICD(int? maxLengthCode,
            int? maxLengthText, bool isRequired,
            TextEdit txtIcdCode,
           TextEdit txtIcdMain,
           CustomGridLookUpEditWithFilterMultiColumn cbo,
            CheckEdit chkEdit,
            DevExpress.XtraLayout.LayoutControlItem lciIcdText)
        {
            try
            {
                if (isRequired)
                {
                    lciIcdText.AppearanceItemCaption.ForeColor = Color.Maroon;

                    ICDValidationRuleControl icdMainRule = new ICDValidationRuleControl();
                    icdMainRule.txtIcdCode = txtIcdCode;
                    icdMainRule.btnBenhChinh = cbo;
                    icdMainRule.txtMainText = txtIcdMain;
                    icdMainRule.chkCheck = chkEdit;
                    icdMainRule.maxLengthCode = maxLengthCode;
                    icdMainRule.maxLengthText = maxLengthText;
                    icdMainRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                    icdMainRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(txtIcdCode, icdMainRule);
                }
                else
                {
                    lciIcdText.AppearanceItemCaption.ForeColor = new System.Drawing.Color();
                    txtIcdCode.ErrorText = "";
                    dxValidationProvider1.RemoveControlError(txtIcdCode);
                    dxValidationProvider1.SetValidationRule(txtIcdCode, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD(TextEdit txtIcdCode,
           TextEdit txtIcdMain,
           CustomGridLookUpEditWithFilterMultiColumn cbo,
            CheckEdit chkEdit,
            TextEdit nextFocus)
        {
            try
            {
                cbo.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_ICD icd = dataIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbo.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtIcdCode.Text = icd.ICD_CODE;
                    txtIcdMain.Text = icd.ICD_NAME;
                    chkEdit.Checked = (chkEdit.Enabled ? (this.autoCheckIcd == 1) : false);
                    if (chkEdit.Checked && nextFocus != null)
                    {
                        nextFocus.Focus();
                        nextFocus.SelectAll();
                    }
                    else if (chkEdit.Enabled)
                    {
                        chkEdit.Focus();
                    }
                    else
                    {
                        if (nextFocus != null)
                        {
                            nextFocus.Focus();
                            nextFocus.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangecboIcdCmTD(TextEdit txtIcdCode, TextEdit txtIcdMain, CustomGridLookUpEditWithFilterMultiColumn cbo, CheckEdit chkEdit, TextEdit nextFocus)
        {
            try
            {
                cbo.Properties.Buttons[1].Visible = true;
                var icd = dataIcdCms.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbo.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtIcdCode.Text = icd.ICD_CM_CODE;
                    txtIcdMain.Text = icd.ICD_CM_NAME;
                    chkEdit.Checked = (chkEdit.Enabled ? (this.autoCheckIcd == 1) : false);
                    if (chkEdit.Checked && nextFocus != null)
                    {
                        nextFocus.Focus();
                        nextFocus.SelectAll();
                    }
                    else if (chkEdit.Enabled)
                    {
                        chkEdit.Focus();
                    }
                    else
                    {
                        if (nextFocus != null)
                        {
                            nextFocus.Focus();
                            nextFocus.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region --- Event ICD ---
        private void txtIcdCode1_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Bạn nhập mã chẩn đoán không đúng. Vui lòng kiểm tra lại";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode2_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Bạn nhập mã chẩn đoán không đúng. Vui lòng kiểm tra lại";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode3_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Bạn nhập mã chẩn đoán không đúng. Vui lòng kiểm tra lại";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCmCode_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Bạn nhập mã chẩn đoán không đúng. Vui lòng kiểm tra lại";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode1.Text.ToUpper(), txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode2.Text.ToUpper(), txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode3.Text.ToUpper(), txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCmCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCMCombo(txtIcdCmCode.Text.ToUpper(), txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode1_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode1.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtIcdCode1);
                        ValidationICD(10, 500, true, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, lciIcd1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode2_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode2.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtIcdCode2);
                        ValidationICD(10, 500, true, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, lciIcd2);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode3_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode3.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtIcdCode1);
                        ValidationICD(10, 500, true, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, lciIcd3);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCmCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataIcdCms.Where(o => o.ICD_CM_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CM_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCmCode.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtIcdCmCode);
                        ValidationICD(10, 500, false, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm, lciIcdCmCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcd1.Properties.Buttons[1].Visible)
                        return;

                    cboIcd1.EditValue = null;
                    txtIcdCode1.Text = "";
                    txtIcd1.Text = "";
                    cboIcd1.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcd2.Properties.Buttons[1].Visible)
                        return;

                    cboIcd2.EditValue = null;
                    txtIcdCode2.Text = "";
                    txtIcd2.Text = "";
                    cboIcd2.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd3_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcd3.Properties.Buttons[1].Visible)
                        return;

                    cboIcd3.EditValue = null;
                    txtIcdCode3.Text = "";
                    txtIcd3.Text = "";
                    cboIcd3.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCmName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcdCmName.Properties.Buttons[1].Visible)
                        return;
                    cboIcdCmName.EditValue = null;
                    txtIcdCmCode.Text = "";
                    txtIcdCmName.Text = "";
                    cboIcdCmName.Properties.Buttons[1].Visible = false;
                    txtIcdCmCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd1_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcd1.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, txtIcdExtraCode);
                    else
                    {
                        txtIcdExtraCode.Focus();
                        txtIcdExtraCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcd2.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, txtIcdCode3);
                    else
                    {
                        txtIcdCode3.Focus();
                        txtIcdCode3.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd3_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcd3.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, txtPtttGroupCode);
                    else
                    {
                        txtPtttGroupCode.Focus();
                        txtPtttGroupCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCmName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcdCmName.EditValue != null)
                        this.ChangecboIcdCmTD(txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm, txtIcdCmSubCode);
                    else
                    {
                        txtIcdCmSubCode.Focus();
                        txtIcdCmSubCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcd1.ClosePopup();
                    cboIcd1.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcd1.ClosePopup();
                    if (cboIcd1.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, txtIcdExtraCode);
                }
                else
                    cboIcd1.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcd2.ClosePopup();
                    cboIcd2.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcd2.ClosePopup();
                    if (cboIcd2.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, txtIcdCode3);
                }
                else
                    cboIcd2.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd3_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcd3.ClosePopup();
                    cboIcd3.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcd3.ClosePopup();
                    if (cboIcd3.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, txtPtttGroupCode);
                }
                else
                    cboIcd3.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCmName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcdCmName.ClosePopup();
                    cboIcdCmName.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcdCmName.ClosePopup();
                    if (cboIcdCmName.EditValue != null)
                        this.ChangecboIcdCmTD(txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm, txtIcdCmSubCode);
                }
                else
                    cboIcdCmName.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcd1.Text))
                {
                    cboIcd1.EditValue = null;
                    txtIcd1.Text = "";
                    chkIcd1.Checked = false;
                }
                else
                {
                    //this._TextIcdName1 = cboIcd1.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcd2.Text))
                {
                    cboIcd2.EditValue = null;
                    txtIcd2.Text = "";
                    chkIcd2.Checked = false;
                }
                else
                {
                    //this._TextIcdName2 = cboIcd2.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcd3.Text))
                {
                    cboIcd3.EditValue = null;
                    txtIcd3.Text = "";
                    chkIcd3.Checked = false;
                }
                else
                {
                    //this._TextIcdName3 = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCmName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcdCmName.Text))
                {
                    cboIcdCmName.EditValue = null;
                    txtIcdCmName.Text = "";
                    chkIcdCm.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcd1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkIcd1.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcd2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkIcd2.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcd3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkIcd3.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCmName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkIcdCm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcd1.Checked == true)
                {
                    cboIcd1.Visible = false;
                    txtIcd1.Visible = true;
                    txtIcd1.Text = cboIcd1.Text;
                    txtIcd1.Focus();
                    txtIcd1.SelectAll();
                }
                else if (chkIcd1.Checked == false)
                {
                    txtIcd1.Visible = false;
                    cboIcd1.Visible = true;
                    txtIcd1.Text = cboIcd1.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcd2.Checked == true)
                {
                    cboIcd2.Visible = false;
                    txtIcd2.Visible = true;
                    txtIcd2.Text = cboIcd2.Text;
                    txtIcd2.Focus();
                    txtIcd2.SelectAll();
                }
                else if (chkIcd2.Checked == false)
                {
                    txtIcd2.Visible = false;
                    cboIcd2.Visible = true;
                    txtIcd2.Text = cboIcd2.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcd3.Checked == true)
                {
                    cboIcd3.Visible = false;
                    txtIcd3.Visible = true;
                    txtIcd3.Text = cboIcd3.Text;
                    txtIcd3.Focus();
                    txtIcd3.SelectAll();
                }
                else if (chkIcd3.Checked == false)
                {
                    txtIcd3.Visible = false;
                    cboIcd3.Visible = true;
                    txtIcd3.Text = cboIcd3.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcdCm_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcdCm.Checked == true)
                {
                    cboIcdCmName.Visible = false;
                    txtIcdCmName.Visible = true;
                    txtIcdCmName.Text = cboIcdCmName.Text;
                    txtIcdCmName.Focus();
                    txtIcdCmName.SelectAll();
                }
                else if (chkIcdCm.Checked == false)
                {
                    txtIcdCmName.Visible = false;
                    cboIcdCmName.Visible = true;
                    txtIcdCmName.Text = cboIcdCmName.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdExtraCode.Focus();
                    txtIcdExtraCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCode3.Focus();
                    txtIcdCode3.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPtttGroupCode.Focus();
                    txtPtttGroupCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcdCm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCmSubCode.Focus();
                    txtIcdCmSubCode.SelectAll();
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
