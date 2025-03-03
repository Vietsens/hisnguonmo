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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.UC.SecondaryIcd.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Plugins.Library.CheckIcd;
using MOS.UTILITY;

namespace HIS.UC.SecondaryIcd
{
    public partial class UCSecondaryIcd : UserControl
    {
        private DelegateNextFocus DelegateNextFocus { get; set; }
        private int limit = 100;

        private string[] icdSeparators = new string[] { ";" };
        int positionHandleControlLeft = -1;
        DelegateGetIcdMain GetIcdMain { get; set; }
        

        private List<HIS_ICD> ListHisIcds { get; set; }
        private List<V_HIS_ICD> ListViewHisIcds { get; set; }
        private HIS_TREATMENT treatment;
        HIS.Desktop.Plugins.Library.CheckIcd.CheckIcdManager checkIcd;
        private frmSecondaryIcd FormSecondaryIcd { get; set; }
        DelegateCheckICD checkICD { get; set; }

        #region ctor
        public UCSecondaryIcd()
        {
            InitializeComponent();

            this.SetCaptionByLanguageKey();
        }

        public UCSecondaryIcd(SecondaryIcdInitADO data)
            : this()
        {
            try
            {
                this.SetCaptionByLanguageKey();
                if (data != null)
                {
                    if (data.Height > 0 && data.Width > 0)
                    {
                        this.Size = new Size(data.Width, data.Height);
                    }
                    if (data.TextSize > 0)
                    {
                        this.lciIcdSubCode.TextSize = new Size(data.TextSize, this.lciIcdSubCode.Height);
                    }
                    this.DelegateNextFocus = data.DelegateNextFocus;
                    this.GetIcdMain = data.DelegateGetIcdMain;
                    if (data.delegateCheckICD != null)
                    {
                        this.checkICD = data.delegateCheckICD;
                    }

                    if (data != null && !String.IsNullOrEmpty(data.TextLblIcd))
                    {
                        this.lciIcdSubCode.Text = data.TextLblIcd;
                    }
                    if (data != null && !String.IsNullOrEmpty(data.TootiplciIcdSubCode))
                    {
                        this.lciIcdSubCode.OptionsToolTip.ToolTip = data.TootiplciIcdSubCode;
                    }
                    if (data != null && !String.IsNullOrEmpty(data.TextNullValue))
                    {
                        this.txtIcdText.Properties.NullValuePrompt = data.TextNullValue;
                    }

                    if (data.limitDataSource > 0)
                    {
                        this.limit = data.limitDataSource;
                    }

                    if (data.HisIcds != null && data.HisIcds.Count > 0)
                    {
                        ListHisIcds = data.HisIcds.Where(p => p.IS_ACTIVE == 1).ToList(); ;
                        List<HIS_ICD> icdIsTraditionals = (ListHisIcds != null && ListHisIcds.Count > 0) ? ListHisIcds.Where(o => o.IS_TRADITIONAL == Constant.IS_TRUE).ToList() : null;
                        List<HIS_ICD> icdNotIsTraditionals = (data.HisIcds != null && data.HisIcds.Count > 0) ? data.HisIcds.Where(o => o.IS_TRADITIONAL == null || o.IS_TRADITIONAL == Constant.IS_FALSE).ToList() : null;
                        if (icdIsTraditionals != null && icdIsTraditionals.Count > 0 && icdNotIsTraditionals != null && icdNotIsTraditionals.Count > 0)
                        {
                            ListHisIcds = icdNotIsTraditionals;
                        }
                        if (icdIsTraditionals != null && icdIsTraditionals.Count > 0 && icdIsTraditionals.Count == ListHisIcds.Count)
                        {
                            ListHisIcds = icdIsTraditionals;
                        }
                        if (icdNotIsTraditionals != null && icdNotIsTraditionals.Count > 0 && icdNotIsTraditionals.Count == ListHisIcds.Count)
                        {
                            ListHisIcds = icdNotIsTraditionals;
                        }
                    }
                    if (data.ViewHisIcds != null && data.ViewHisIcds.Count > 0)
                    {
                        ListViewHisIcds = data.ViewHisIcds.Where(p => p.IS_ACTIVE == 1).ToList(); ;
                        List<V_HIS_ICD> icdIsTraditionalsV = (ListViewHisIcds != null && ListViewHisIcds.Count > 0) ? ListViewHisIcds.Where(o => o.IS_TRADITIONAL == Constant.IS_TRUE).ToList() : null;
                        List<V_HIS_ICD> icdNotIsTraditionalsV = (data.ViewHisIcds != null && data.ViewHisIcds.Count > 0) ? data.ViewHisIcds.Where(o => o.IS_TRADITIONAL == null || o.IS_TRADITIONAL == Constant.IS_FALSE).ToList() : null;
                        if (icdIsTraditionalsV != null && icdIsTraditionalsV.Count > 0 && icdNotIsTraditionalsV != null && icdNotIsTraditionalsV.Count > 0)
                        {
                            ListViewHisIcds = icdNotIsTraditionalsV;
                        }
                        if (icdIsTraditionalsV != null && icdIsTraditionalsV.Count > 0 && icdIsTraditionalsV.Count == ListViewHisIcds.Count)
                        {
                            ListViewHisIcds = icdIsTraditionalsV;
                        }
                        if (icdNotIsTraditionalsV != null && icdNotIsTraditionalsV.Count > 0 && icdNotIsTraditionalsV.Count == ListViewHisIcds.Count)
                        {
                            ListViewHisIcds = icdNotIsTraditionalsV;
                        }
                    }
                    if (data.hisTreatment != null)
                    {
                        treatment = data.hisTreatment;
                        checkIcd = new CheckIcdManager(null, treatment);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region internal
        internal void Reload(ADO.SecondaryIcdDataADO input)
        {
            try
            {
                if (input != null)
                {
                    this.txtIcdSubCode.Text = input.ICD_SUB_CODE;
                    this.txtIcdText.Text = input.ICD_TEXT;
                }
                else
                {
                    txtIcdSubCode.Text = null;
                    txtIcdText.Text = null;
                }
                this.dxValidationProvider1.RemoveControlError(this.txtIcdSubCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FocusControl()
        {
            try
            {
                txtIcdSubCode.Focus();
                txtIcdSubCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void SetAttachIcd(ADO.SecondaryIcdDataADO input)
        {
            try
            {
                if (input != null)
                {
                    ProcessIcdSub(input.ICD_SUB_CODE, input.ICD_TEXT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessIcdSub(string icdCodes, string icdNames)
        {
            try
            {
                var lstIcdCode = icdCodes.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var lstIcdName = icdNames.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var lstIcdCodeScreen = txtIcdSubCode.Text.Trim().Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                lstIcdCodeScreen.AddRange(lstIcdCode);
                lstIcdCodeScreen = lstIcdCodeScreen.Distinct().ToList();
                string icdCode = string.Join(";", lstIcdCodeScreen);
                var lstIcdNameScreen = txtIcdText.Text.Trim().Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                lstIcdNameScreen.AddRange(lstIcdName);
                lstIcdNameScreen = lstIcdNameScreen.Distinct().ToList();
                string icdName = string.Join(";", lstIcdNameScreen);
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtIcdSubCode.Text = icdCode;
                }
                else
                {
                    txtIcdSubCode.Text = "";
                }
                if (!string.IsNullOrEmpty(icdName))
                {
                    txtIcdText.Text = icdName;
                }
                else
                {
                    txtIcdText.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValue(object input)
        {
            try
            {
                if (input is ADO.SecondaryIcdDataADO)
                {
                    this.txtIcdSubCode.Text = ((ADO.SecondaryIcdDataADO)input).ICD_SUB_CODE;
                    this.txtIcdText.Text = ((ADO.SecondaryIcdDataADO)input).ICD_TEXT;
                }
                else
                {
                    txtIcdSubCode.Text = null;
                    txtIcdText.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ReadOnly(bool isReadOnly)
        {
            try
            {
                if (isReadOnly)
                {
                    txtIcdSubCode.ReadOnly = true;
                    txtIcdText.ReadOnly = true;
                }
                else
                {
                    txtIcdSubCode.ReadOnly = false;
                    txtIcdText.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool GetValidate()
        {
            bool vali = true;
            try
            {
                //Check validate
                this.positionHandleControlLeft = -1;
                if (!dxValidationProvider1.Validate())
                {
                    vali = false;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("UCSecondaryIcd.vali", vali));
            }
            catch (Exception ex)
            {
                vali = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return vali;
        }

        internal object GetValue()
        {
            object result = null;
            try
            {
                ADO.SecondaryIcdDataADO outPut = new ADO.SecondaryIcdDataADO();

                if (!String.IsNullOrEmpty(txtIcdSubCode.Text))
                {
                    outPut.ICD_SUB_CODE = txtIcdSubCode.Text;
                }
                if (!String.IsNullOrEmpty(txtIcdText.Text))
                {
                    outPut.ICD_TEXT = txtIcdText.Text;
                }
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        internal void SetError(string error)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(error))
                {
                    dxErrorProvider1.SetError(txtIcdSubCode, error, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                }
                else
                {
                    dxErrorProvider1.SetError(txtIcdSubCode, "", DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool GetValidateWithMessage(List<string> errorEmpty, List<string> errorOther)
        {
            bool result = true;
            try
            {
                this.positionHandleControlLeft = -1;
                if (!dxValidationProvider1.Validate())
                {
                    result = false;
                }

                if (!result)
                {
                    var invalidControls = dxValidationProvider1.GetInvalidControls();
                    if (invalidControls != null && invalidControls.Count > 0)
                    {
                        foreach (System.Windows.Forms.Control c in invalidControls)
                        {
                            string errorC = this.lciIcdSubCode.Text.Replace(":", "");
                            string errorT = dxValidationProvider1.GetValidationRule(c).ErrorText;
                            if (errorT == Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc)
                         || errorT == Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc))
                            {
                                errorEmpty.Add(errorC);
                            }
                            else
                            {
                                errorC = String.Format("{0}: {1}", errorC, errorT);
                                errorOther.Add(errorC);
                            }
                        }

                        if (errorEmpty != null && errorEmpty.Count > 0)
                        {
                            errorEmpty = errorEmpty.Distinct().ToList();
                        }
                        if (errorOther != null && errorOther.Count > 0)
                        {
                            errorOther = errorOther.Distinct().ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        #endregion

        #region private
        internal bool ShowPopupIcdChoose()
        {
            try
            {
                WaitingManager.Show();
                if (this.ListViewHisIcds != null && this.ListViewHisIcds.Count > 0)
                {
                    this.FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, limit, this.ListViewHisIcds, this.treatment);
                }
                else
                {
                    this.FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, limit, this.ListHisIcds, this.treatment);
                }
                WaitingManager.Hide();
                this.FormSecondaryIcd.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        private void txtIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!ProccessorByIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
                    {
                        e.Handled = true;
                        return;
                    }
                    DelegateNextFocus();
                    if (checkICD != null) checkICD();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        private void txtIcdSubCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    if (this.ListViewHisIcds != null && this.ListViewHisIcds.Count > 0)
                    {
                        this.FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, limit, this.ListViewHisIcds, this.treatment);
                    }
                    else
                    {
                        this.FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, limit, this.ListHisIcds, this.treatment);
                    }
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

        private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (DelegateNextFocus != null)
                    {
                        DelegateNextFocus();
                    }
                    if (checkICD != null) checkICD();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    if (this.ListViewHisIcds != null && this.ListViewHisIcds.Count > 0)
                    {
                        this.FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, limit, this.ListViewHisIcds, this.treatment);
                    }
                    else
                    {
                        this.FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, limit, this.ListHisIcds, this.treatment);
                    }
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
                txtIcdSubCode.Text = icdCode;
                txtIcdText.Text = icdName;
                if (checkICD != null) checkICD();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = processIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdSubCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string processIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                HIS_ICD checkInList = null;

                                if (ListHisIcds != null && ListHisIcds.Count > 0)
                                {
                                    checkInList = ListHisIcds.Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();

                                    if (checkInList == null || checkInList.ID == 0)
                                    {
                                        result += item + IcdUtil.seperator;
                                    }
                                }
                                else
                                {
                                    var ViewicdByCode = ListViewHisIcds.Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                    if (ViewicdByCode == null || ViewicdByCode.ID == 0)
                                    {
                                        result += item + IcdUtil.seperator;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        #endregion

        private void txtIcdSubCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                e.ErrorText = strError;
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private bool CheckIcdWrongCode(ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(this.txtIcdSubCode.Text.Trim()))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    List<string> lstIcdSubName = new List<string>();
                    List<string> lstIcdCodes = new List<string>();



                    // check icd moi
                    string[] arrIcdExtraCodes = this.txtIcdSubCode.Text.Trim().Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        //check icd moi
                        string messErr = null;
                        string erorr_code = null;
                        if (checkIcd != null)
                        {
                            if (!checkIcd.ProcessCheckIcd(null, string.Join(";", arrIcdExtraCodes), ref messErr,false))
                            {
                                if (!string.IsNullOrEmpty(messErr))
                                {
                                    this.txtIcdSubCode.Text = string.Join(";", arrIcdExtraCodes);
                                    XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                                }
                                

                            }
                        }
                        List<HIS_ICD> icdByCode = null;

                        if (ListHisIcds != null && ListHisIcds.Count > 0)
                            icdByCode = ListHisIcds.Where(o => arrIcdExtraCodes.Contains(o.ICD_CODE)).ToList();
                        else
                        {
                            var ViewicdByCode = ListViewHisIcds.Where(o => arrIcdExtraCodes.Contains(o.ICD_CODE)).ToList();
                            ViewicdByCode.ForEach(o =>
                            {
                                HIS_ICD _icd = new HIS_ICD();
                                _icd.ID = o.ID;
                                _icd.ICD_CODE = o.ICD_CODE;
                                _icd.ICD_NAME = o.ICD_NAME;
                                icdByCode.Add(_icd);
                            });

                        }
                        icdByCode = icdByCode.OrderBy(o => Array.IndexOf(arrIcdExtraCodes, o.ICD_CODE)).ToList();
                        if (icdByCode != null && icdByCode.Count > 0)
                        {
                            this.txtIcdSubCode.Text = String.Join(";", icdByCode.Select(s=>s.ICD_CODE).ToList());
                            this.txtIcdText.Text = String.Join(";", icdByCode.Select(s => s.ICD_NAME).ToList());
                        }
                        else
                        {

                            this.txtIcdSubCode.Text = null;
                            this.txtIcdText.Text = null;
                        }
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            valid = false;
                            this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, this.txtIcdText.Text);
                            XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, string.Join(",", arrWrongCodes)), "Thông báo", MessageBoxButtons.OK);
                            ShowPopupIcdChoose();
                        }
                    }
                    else
                    {
                        this.txtIcdSubCode.Text = null;
                        this.txtIcdText.Text = null;
                    }
                }
                else
                {
                    this.txtIcdSubCode.Text = null;
                    this.txtIcdText.Text = null;
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ProccessorByIcdCode(string currentValue)
        {
            bool valid = true;
            try
            {
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                if (!CheckIcdWrongCode(ref strIcdNames, ref strWrongIcdCodes))
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Debug("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
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

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
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

        private void UCSecondaryIcd_Load(object sender, EventArgs e)
        {
            try
            {
                dxErrorProvider1.ClearErrors();
                ValdateSecondaryIcd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCSecondaryIcd
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceMessage.LanguageResourceUCSecondaryIcd = new ResourceManager("HIS.UC.SecondaryIcd.Resources.Lang", typeof(UCSecondaryIcd).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCSecondaryIcd.layoutControl1.Text", Resources.ResourceMessage.LanguageResourceUCSecondaryIcd, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCSecondaryIcd.layoutControl2.Text", Resources.ResourceMessage.LanguageResourceUCSecondaryIcd, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCSecondaryIcd.txtIcdText.Properties.NullValuePrompt", Resources.ResourceMessage.LanguageResourceUCSecondaryIcd, LanguageManager.GetCulture());
                this.lciIcdSubCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCSecondaryIcd.lciIcdSubCode.OptionsToolTip.ToolTip", Resources.ResourceMessage.LanguageResourceUCSecondaryIcd, LanguageManager.GetCulture());
                this.lciIcdSubCode.Text = Inventec.Common.Resource.Get.Value("UCSecondaryIcd.lciIcdSubCode.Text", Resources.ResourceMessage.LanguageResourceUCSecondaryIcd, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
