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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using His.UC.UCHein.Base;
using His.UC.UCHein.Config;
using His.UC.UCHein.ControlProcess;
using His.UC.UCHein.Design.TemplateHeinBHYT1.ValidationRule;
using His.UC.UCHein.HisPatient;
using His.UC.UCHein.Utils;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace His.UC.UCHein.Design.TemplateHeinBHYT1
{
    public partial class Template__HeinBHYT1 : UserControl
    {
        private void cboChanDoanTD_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboChanDoanTD.EditValue = null;
                    this.cboChanDoanTD.Properties.Buttons[1].Visible = false;
                    this.txtMaChanDoanTD.Text = "";
                    this.txtMaChanDoanTD.ErrorText = "";
                    this.txtDialogText.Text = "";
                    this.chkHasDialogText.Checked = false;
                    this.chkHasDialogText.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDTMCChiTra_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (HisConfigCFG.IsNotAutoCheck5Y6M)
                    return;
                this.chkJoin5Year.Checked = this.IsShowMessage = (!String.IsNullOrEmpty(this.txtFreeCoPainTime.Text.Trim()));
                this.chkPaid6Month.Checked = false;
                if (!String.IsNullOrEmpty(this.txtFreeCoPainTime.Text.Trim()))
                {
                    this.IsShowMessage = this.chkJoin5Year.Checked;
                    DateTime? dt = HeinUtils.ConvertDateStringToSystemDate(this.txtFreeCoPainTime.Text);
                    this.chkPaid6Month.Checked = dt != null && dt.Value != DateTime.MinValue && Int64.Parse(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt).ToString().Substring(0, 8)) < Int64.Parse(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now).ToString().Substring(0, 8));

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboNoiSong.Focus();
                    this.cboNoiSong.SelectAll();
                    if (this.cboNoiSong.EditValue == null)
                    {
                        this.cboNoiSong.ShowPopup();
                    }
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.PageDown)
                {
                    this.dtFreeCoPainTime.ShowPopup();
                    this.dtFreeCoPainTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtFreeCoPainTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtFreeCoPainTime.Visible = false;
                    this.dtFreeCoPainTime.Update();
                    this.txtFreeCoPainTime.Text = this.dtFreeCoPainTime.DateTime.ToString("dd/MM/yyyy");
                    this.cboNoiSong.Focus();
                    this.cboNoiSong.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtFreeCoPainTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtFreeCoPainTime.Visible = false;
                    this.dtFreeCoPainTime.Update();
                    if (this.dtFreeCoPainTime.EditValue != null && this.dtFreeCoPainTime.DateTime != DateTime.MinValue)
                        this.txtFreeCoPainTime.Text = this.dtFreeCoPainTime.DateTime.ToString("dd/MM/yyyy");
                    else
                        this.txtFreeCoPainTime.Text = null;
                    this.cboNoiSong.Focus();
                    this.cboNoiSong.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Down)
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtFreeCoPainTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtFreeCoPainTime.EditValue = dt;
                        this.dtFreeCoPainTime.Update();
                    }

                    this.dtFreeCoPainTime.Visible = true;
                    this.dtFreeCoPainTime.ShowPopup();
                    this.dtFreeCoPainTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtFreeCoPainTime.Text)) return;

                string date = "";
                if (this.txtFreeCoPainTime.Text.Contains("/"))
                    date = His.UC.UCHein.Utils.HeinUtils.DateToDateRaw(this.txtFreeCoPainTime.Text);

                if (!String.IsNullOrEmpty(date))
                {
                    this.txtFreeCoPainTime.Text = date;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string inputDate = this.txtFreeCoPainTime.Text.Trim();
                if (!String.IsNullOrEmpty(inputDate))
                {
                    if (inputDate.Length == 8)
                    {
                        inputDate = inputDate.Substring(0, 2) + "/" + inputDate.Substring(2, 2) + "/" + inputDate.Substring(4, 4);
                    }

                    var dateFreeCoPainTime = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(inputDate);
                    if (dateFreeCoPainTime != null && dateFreeCoPainTime.Value != DateTime.MinValue)
                    {
                        this.txtFreeCoPainTime.Text = inputDate;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFreeCoPainTime_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                AutoValidate = AutoValidate.EnableAllowFocusChange;
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiChuyenDen_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboNoiChuyenDen.Properties.Buttons[1].Visible = (!String.IsNullOrEmpty((this.cboNoiChuyenDen.EditValue ?? "").ToString()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiChuyenDen_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboNoiChuyenDen.EditValue = null;
                    this.cboNoiChuyenDen.Properties.Buttons[1].Visible = false;
                    this.txtMaNoiChuyenDen.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLyDoChuyen_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboLyDoChuyen.EditValue = null;
                    this.cboLyDoChuyen.Properties.Buttons[1].Visible = false;
                    this.txtMaLyDoChuyen.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLyDoChuyen_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboLyDoChuyen.Properties.Buttons[1].Visible = (!String.IsNullOrEmpty((this.cboLyDoChuyen.EditValue ?? "").ToString()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaChuanDoanTD_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadChuanDoanTDCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboChanDoanTD_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboChanDoanTD.Text))
                {
                    cboChanDoanTD.EditValue = null;
                    txtMaChanDoanTD.Text = "";
                    chkHasDialogText.Checked = false;
                }
                else
                {
                    this._TextIcdName = cboChanDoanTD.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaChanDoanTD_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMaChanDoanTD.Text))
                {
                    var searchResult = ((DataStore.Icds != null && DataStore.Icds.Count > 0) ? DataStore.Icds.Where(o => o.ICD_CODE.ToUpper() == txtMaChanDoanTD.Text.ToUpper()).ToList() : null);
                    if (searchResult == null || searchResult.Count == 0)
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaChanDoanTD_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMaChanDoanTD.Text))
                {
                    var searchResult = ((DataStore.Icds != null && DataStore.Icds.Count > 0) ? DataStore.Icds.Where(o => o.ICD_CODE.ToUpper() == txtMaChanDoanTD.Text.ToUpper()).ToList() : null);
                    if (searchResult == null || searchResult.Count == 0)
                    {
                        e.ErrorText = ResourceMessage.MaBenhChinhKhongHopLe;
                    }
                }
                else
                {
                    e.ErrorText = "";
                }

                AutoValidate = AutoValidate.EnableAllowFocusChange;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHasDialogText_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkHasDialogText.Checked == true)
                {
                    cboChanDoanTD.Visible = false;
                    txtDialogText.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtDialogText.Text = this._TextIcdName;
                    else
                        txtDialogText.Text = cboChanDoanTD.Text;

                    txtDialogText.Focus();
                    txtDialogText.SelectAll();
                }
                else
                {
                    txtDialogText.Visible = false;
                    cboChanDoanTD.Visible = true;
                    //txtDialogText.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHasDialogText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                    SendKeys.Send("^a");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCardFromTime_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCardToTime_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtHNCode.Focus();
                    this.txtHNCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHNCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.lciMediRecordRouteTransfer.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                        && chkMediRecordRouteTransfer.Enabled)
                        this.chkMediRecordRouteTransfer.Focus();
                    else
                        this.FocusMoveOut();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHasDobCertificate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.chkHasDobCertificate.Checked)
                    {
                        this.FocusMoveOut();
                    }
                    else
                    {
                        this.txtSoThe.Focus();
                        this.txtSoThe.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHasDobCertificate_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool enable = (!this.chkHasDobCertificate.Checked);
                if (enable)
                {
                    lblHeincardToDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                    this.ValidHeinCardToTime();
                }
                else
                {
                    lblHeincardToDate.AppearanceItemCaption.ForeColor = new Color();
                    dxValidationProvider1.SetValidationRule(txtHeinCardToTime, null);
                }

                MediOrgProcess.LoadDataToComboNoiDKKCBBD(this.cboDKKCBBD, DataStore.MediOrgs);

                if (this._DelegateSetRelativeAddress != null)
                {
                    this._DelegateSetRelativeAddress(enable);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSoThe_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe);
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSoThe_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string heinCardNumber = this.txtSoThe.Text;
                    heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());

                    this.txtHeinCardFromTime.Focus();
                    this.txtHeinCardFromTime.SelectAll();

                    string cardFormat = this.txtSoThe.Text;
                    //xuandv
                    //Inventec.Desktop.Common.HtmlString.ProcessorString processorString = new Inventec.Desktop.Common.HtmlString.ProcessorString();
                    cardFormat = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(cardFormat, System.Drawing.FontStyle.Bold);
                    cardFormat = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(cardFormat, Color.Green);
                    LogSystem.Debug("txtSoThe_KeyDown => So the ban dau = " + this.txtSoThe.Text + ", so the sau khi da xu ly chuoi = " + heinCardNumber);
                    //xuandv
                    bool valid = true;
                    valid = valid && new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(heinCardNumber);
                    if (valid && !String.IsNullOrEmpty(heinCardNumber))
                    {
                        this.dxErrorProvider1.ClearErrors();
                        var listResult = HisPatientGet.GetSDO(heinCardNumber);
                        if (listResult != null && listResult.Count > 0)
                        {
                            LogSystem.Info("txtSoThe_KeyDown => Tim thay " + listResult.Count + " BN co So the = " + heinCardNumber + ".");
                            if (listResult.Count > 1)
                            {
                                frmPatientChoice frm = new frmPatientChoice(listResult, this.FillDataAfterSelectOnePatient, DataStore.Genders);
                                frm.ShowDialog();
                            }
                            else
                            {
                                //LogSystem.Debug("Truong hop chua nhap noi dkkcb ban dau -> chi show len thong bao So the " + heinCardNumber + " da co nguoi su dung");
                                this.currentPatientSdo = listResult[0];

                                //Fill dữ liệu bệnh nhân (gọi sang module tiếp đón) & dữ liệu đối tượng điều trị vào form
                                this.FillDataAfterSelectOnePatient(listResult[0]);
                                if (entity.IsInitFromCallPatientTypeAlter)
                                    cboDKKCBBD_KeyUp(null, e);
                            }
                        }
                        else
                        {
                            oldeHeinCardNumber = null;
                            CheckExamHistoryFromBHXHApi(heinCardNumber);
                            LogSystem.Info("txtSoThe_KeyDown => khong tim thay Bn cu theo so the = " + heinCardNumber + ", listResult.Count = 0");
                        }

                        if (this.dlgProcessFillDataCareerUnder6AgeByHeinCardNumber != null)
                        {
                            HeinCardData heinCard = new HeinCardData();
                            heinCard.HeinCardNumber = heinCardNumber;
                            this.dlgProcessFillDataCareerUnder6AgeByHeinCardNumber(heinCard, true);
                        }
                        if (this.actChangePatientDob != null)
                            this.actChangePatientDob();
                    }
                    else
                    {
                        this.dxErrorProvider1.SetError(this.txtSoThe, His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapSoTheBHYTKhongHopLe));//xuandv
                        txtSoThe.Focus();
                        txtSoThe.SelectAll();
                    }
                    e.Handled = true;
                    //this.CheckExamHistoryFromBHXHApi(heinCardNumber);
                    //GetHeinCarNumberToCheckTT(heinCardNumber);

                }
                else if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down || e.KeyCode == Keys.PageDown)
                {
                    GetHeinCarNumberToCheckTT(txtSoThe.Text.Trim());
                    this.cboSoThe.ShowPopup();
                    this.cboSoThe.SelectAll();
                    PopupProcess.SelectFirstRowPopup(this.cboSoThe);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // lấy số thẻ để check thông tuyến.

        private void GetHeinCarNumberToCheckTT(string heinCardNumber)
        {
            try
            {
                heinCardNumber = heinCardNumber.Replace(" ", "").ToUpper().Trim();
                heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber);
                this.CheckExamHistoryFromBHXHApi(heinCardNumber);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void UpdateControlEditorTime(DevExpress.XtraEditors.ButtonEdit txtEditorTime, DevExpress.XtraEditors.DateEdit dtEditorTime)
        {
            try
            {
                string strtxtIntructionTime = "";
                if (txtEditorTime.Text.Length == 2 || txtEditorTime.Text.Length == 1)
                {
                    strtxtIntructionTime = "01/01/" + (DateTime.Now.Year - Inventec.Common.TypeConvert.Parse.ToInt64(txtEditorTime.Text)).ToString();
                }
                else if (txtEditorTime.Text.Length == 4)
                    strtxtIntructionTime = "01/01/" + txtEditorTime.Text;
                else if (txtEditorTime.Text.Length == 8)
                {
                    strtxtIntructionTime = txtEditorTime.Text.Substring(0, 2) + "/" + txtEditorTime.Text.Substring(2, 2) + "/" + txtEditorTime.Text.Substring(4, 4);
                }
                else
                    strtxtIntructionTime = txtEditorTime.Text;

                dtEditorTime.EditValue = ConvertDateStringToSystemDate(strtxtIntructionTime);
                dtEditorTime.Update();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private DateTime? ConvertDateStringToSystemDate(string date)
        {
            DateTime? result = null;
            try
            {
                if (!String.IsNullOrEmpty(date))
                {
                    date = date.Replace(" ", "");
                    if (date.Length == 4)
                    {
                        int year = Int16.Parse(date);
                        result = new DateTime(year, 01, 01);
                    }
                    else
                    {
                        int day = Int16.Parse(date.Substring(0, 2));
                        int month = Int16.Parse(date.Substring(3, 2));
                        int year = Int16.Parse(date.Substring(6, 4));
                        result = new DateTime(year, month, day);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }
        public string oldeHeinCardNumber { get; set; }
        private void CheckExamHistoryFromBHXHApi(string heinCardNumber)
        {
            try
            {
                //Code cu
                //if (this.dlgcheckExamHistory != null
                //    && !String.IsNullOrEmpty(heinCardNumber) && !String.IsNullOrEmpty(txtMaDKKCBBD.Text))
                //{
                //    HeinCardData heinCardData = new HeinCardData();
                //    heinCardData.HeinCardNumber = heinCardNumber;
                //    heinCardData.MediOrgCode = txtMaDKKCBBD.Text;
                //    this.dlgcheckExamHistory(heinCardData);
                //}

                if (this.dlgcheckExamHistory != null
                   && !String.IsNullOrEmpty(heinCardNumber))
                {
                    UpdateControlEditorTime(this.txtHeinCardFromTime, this.dtHeinCardFromTime);
                    UpdateControlEditorTime(this.txtHeinCardToTime, this.dtHeinCardToTime);

                    HeinCardData heinCardData = new HeinCardData();
                    heinCardData.HeinCardNumber = heinCardNumber;
                    heinCardData.MediOrgCode = txtMaDKKCBBD.Text;
                    heinCardData.Address = txtAddress.Text;
                    if (this.dtHeinCardFromTime.EditValue != null && this.dtHeinCardFromTime.DateTime != DateTime.MinValue)
                    {
                        heinCardData.FromDate = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(this.dtHeinCardFromTime.DateTime);
                    }
                    if (this.dtHeinCardToTime.EditValue != null && this.dtHeinCardToTime.DateTime != DateTime.MinValue)
                    {
                        heinCardData.ToDate = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(this.dtHeinCardToTime.DateTime);
                    }
                    if (ResultDataADO != null && ResultDataADO.ResultHistoryLDO != null && (ResultDataADO.ResultHistoryLDO.maKetQua == "001" || ResultDataADO.ResultHistoryLDO.maKetQua == "002" || ResultDataADO.ResultHistoryLDO.maKetQua == "050") && oldeHeinCardNumber == heinCardData.HeinCardNumber)
                        return;
                    oldeHeinCardNumber = heinCardNumber;
                    this.dlgcheckExamHistory(heinCardData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSoThe_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    ResetEditorControl.Reset(this.cboSoThe);
                    ResetEditorControl.Reset(this.txtSoThe);
                    ResetEditorControl.Reset(this.dtHeinCardToTime);
                    ResetEditorControl.Reset(this.txtHeinCardToTime);
                    ResetEditorControl.Reset(this.dtHeinCardFromTime);
                    ResetEditorControl.Reset(this.txtHeinCardFromTime);
                    ResetEditorControl.Reset(this.txtMaDKKCBBD);
                    ResetEditorControl.Reset(this.cboDKKCBBD);
                    ResetEditorControl.Reset(this.txtHeinRightRouteCode);
                    ResetEditorControl.Reset(this.cboHeinRightRoute);
                    ResetEditorControl.Reset(this.txtMaChanDoanTD);
                    ResetEditorControl.Reset(this.cboChanDoanTD);
                    ResetEditorControl.Reset(this.txtMaNoiChuyenDen);
                    ResetEditorControl.Reset(this.cboNoiChuyenDen);
                    ResetEditorControl.Reset(this.txtMaHinhThucChuyen);
                    ResetEditorControl.Reset(this.dtTransferInTimeFrom);
                    ResetEditorControl.Reset(this.dtTransferInTimeTo);
                    ResetEditorControl.Reset(this.cboHinhThucChuyen);
                    ResetEditorControl.Reset(this.txtMaLyDoChuyen);
                    ResetEditorControl.Reset(this.cboLyDoChuyen);
                    ResetEditorControl.Reset(this.cboNoiSong);
                    ResetEditorControl.Reset(this.txtMucHuong);
                    ResetEditorControl.Reset(this.txtAddress);
                    this.rdoWrongRoute.Checked = this.rdoRightRoute.Checked = false;
                    this.txtHeinRightRouteCode.Enabled = this.cboHeinRightRoute.Enabled = false;
                    this.txtSoThe.Focus();
                }
                else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
                {
                    ShowComboSoThe();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ShowComboSoThe()
        {
            try
            {
                this.cboSoThe.Visible = true;
                ResetEditorControl.ResetAndFocus(this.cboSoThe, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSoThe_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (txtSoThe.IsModified)
                //{

                //}
                string heinCardNumber = txtSoThe.Text;
                heinCardNumber = heinCardNumber.Replace(" ", "").ToUpper().Trim();
                oldeHeinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber);
                this.CheckHSDAndTECard();
                //if (this.isCallByRegistor == true)//false
                //{
                //    if (this.patientTypeAlterOld != null && this.txtSoThe.Text.Replace("-","").Length == 15 && this.txtSoThe.Text.Replace("-","") != patientTypeAlterOld.HEIN_CARD_NUMBER)
                //        CheckExamHistoryFromBHXHApi(this.txtSoThe.Text);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSoThe_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_HEIN_CARD_NUMBER")
                {
                    var item = ((List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>)this.cboSoThe.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                    {
                        string chkhong = "", chmot = "", chhai = "", chba = "", chbon = "", chnam = "";
                        try
                        {
                            chkhong = item.HEIN_CARD_NUMBER.Substring(0, 2);
                            chmot = item.HEIN_CARD_NUMBER.Substring(2, 1);
                            chhai = item.HEIN_CARD_NUMBER.Substring(3, 2);
                            chba = item.HEIN_CARD_NUMBER.Substring(5, 2);
                            chbon = item.HEIN_CARD_NUMBER.Substring(7, 3);
                            chnam = item.HEIN_CARD_NUMBER.Substring(10, 5);
                        }
                        catch (Exception exx)
                        {
                            LogSystem.Warn("Gan chuoi RENDERER_HEIN_CARD_NUMBER the BHYT loi", exx);
                        }

                        e.Value = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", chkhong, chmot, chhai, chba, chbon, chnam);
                    }
                }
                else if (e.FieldName == "RENDERER_FROM_DATE_TODATE")
                {
                    var item = ((List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>)this.cboSoThe.Properties.DataSource)[e.RecordIndex];
                    if (item != null && item.HEIN_CARD_FROM_TIME > 0 && item.HEIN_CARD_TO_TIME > 0)
                    {
                        string tu = Inventec.Common.DateTime.Convert.TimeNumberToDateString((item.HEIN_CARD_FROM_TIME ?? 0));
                        string den = Inventec.Common.DateTime.Convert.TimeNumberToDateString((item.HEIN_CARD_TO_TIME ?? 0));
                        e.Value = "" + tu + " - " + den;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboSoThe_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboSoThe.EditValue != null)
                    {
                        patientTypeAlterOld = lstPatientTypeAlterMap.FirstOrDefault(o => HeinCardProcess.ProcessHeinCardNumber(o.HEIN_CARD_NUMBER) == cboSoThe.EditValue.ToString());
                        this.HeinCardSelectRowHandler(patientTypeAlterOld);
                        oldeHeinCardNumber = null;
                        this.CheckExamHistoryFromBHXHApi(patientTypeAlterOld.HEIN_CARD_NUMBER);
                        if (this.actChangePatientDob != null)
                            this.actChangePatientDob();
                    }
                    else
                    {
                        this.txtHeinCardFromTime.Focus();
                        this.txtHeinCardFromTime.SelectAll();
                    }
                    this.cboSoThe.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSoThe_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboSoThe.EditValue != null)
                    {
                        patientTypeAlterOld = lstPatientTypeAlterMap.FirstOrDefault(o => HeinCardProcess.ProcessHeinCardNumber(o.HEIN_CARD_NUMBER) == cboSoThe.EditValue.ToString());
                        this.HeinCardSelectRowHandler(patientTypeAlterOld);
                        oldeHeinCardNumber = null;
                        this.CheckExamHistoryFromBHXHApi(patientTypeAlterOld.HEIN_CARD_NUMBER);
                        if (this.actChangePatientDob != null)
                            this.actChangePatientDob();
                    }
                    this.cboSoThe.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinCardFromTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtHeinCardFromTime.Visible = false;
                    this.dtHeinCardFromTime.Update();
                    this.txtHeinCardFromTime.Text = this.dtHeinCardFromTime.DateTime.ToString("dd/MM/yyyy");
                    this.dtHeinCardToTime.Focus();
                    this.dtHeinCardToTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinCardFromTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtHeinCardFromTime.Visible = false;
                    this.dtHeinCardFromTime.Update();
                    this.txtHeinCardFromTime.Text = this.dtHeinCardFromTime.DateTime.ToString("dd/MM/yyyy");
                    this.txtHeinCardToTime.Focus();
                    this.txtHeinCardToTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinCardFromTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.dtHeinCardFromTime.EditValue != null && this.dtHeinCardFromTime.DateTime != DateTime.MinValue)
                {
                    if (this.dtHeinCardFromTime.DateTime.Date > DateTime.Now.Date)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.His_UCHein__TheBHYTChuaDenHanSuDung), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), DefaultBoolean.True);
                        this.txtHeinCardFromTime.Focus();
                    }
                    if (entity.IsInitFromCallPatientTypeAlter)
                    {
                        DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                        if (this.patientTypeAlterOld != null && patientTypeAlterOld.HEIN_CARD_FROM_TIME == Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt))
                            return;
                        //else
                        //    CheckExamHistoryFromBHXHApi(this.txtSoThe.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtHeinCardToTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtHeinCardToTime.Visible = false;
                    this.dtHeinCardToTime.Update();
                    if (this.dtHeinCardToTime.EditValue != null && this.dtHeinCardToTime.DateTime != DateTime.MinValue)
                        this.txtHeinCardToTime.Text = this.dtHeinCardToTime.DateTime.ToString("dd/MM/yyyy");
                    else
                        this.txtHeinCardToTime.Text = null;
                    this.txtDu5Nam.Focus();
                    this.txtDu5Nam.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinCardToTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtHeinCardToTime.Visible = false;
                    this.dtHeinCardToTime.Update();
                    this.txtHeinCardToTime.Text = this.dtHeinCardToTime.DateTime.ToString("dd/MM/yyyy");
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDu5Nam_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtDu5Nam.Visible = false;
                    this.dtDu5Nam.Update();
                    if (this.dtDu5Nam.EditValue != null && this.dtDu5Nam.DateTime != DateTime.MinValue)
                        this.txtDu5Nam.Text = this.dtDu5Nam.DateTime.ToString("dd/MM/yyyy");
                    else
                        this.txtDu5Nam.Text = null;
                    //SendKeys.Send("{TAB}");
                    this.txtAddress.Focus();
                    this.txtAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDu5Nam_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtDu5Nam.Visible = false;
                    this.dtDu5Nam.Update();
                    this.txtDu5Nam.Text = this.dtDu5Nam.DateTime.ToString("dd/MM/yyyy");
                    // SendKeys.Send("{TAB}");
                    this.txtAddress.Focus();
                    this.txtAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDu5Nam_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtDu5Nam.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtDu5Nam.EditValue = dt;
                        this.dtDu5Nam.Update();
                    }

                    this.dtDu5Nam.Visible = true;
                    this.dtDu5Nam.Focus();
                    this.dtDu5Nam.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDu5Nam_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtDu5Nam.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtDu5Nam.EditValue = dt;
                        this.dtDu5Nam.Update();
                    }
                    //SendKeys.Send("{TAB}");
                    this.txtAddress.Focus();
                    this.txtAddress.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtDu5Nam.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtDu5Nam.EditValue = dt;
                        this.dtDu5Nam.Update();
                    }

                    this.dtDu5Nam.Visible = true;
                    this.dtDu5Nam.ShowPopup();
                    this.dtDu5Nam.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinCardToTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.dtHeinCardToTime.EditValue != null && this.dtHeinCardToTime.DateTime != DateTime.MinValue)
                {
                    if (this.isShowCheckKhongKTHSD == "1")
                        this.CheckHSDAndTECard();
                    else if (this.dtHeinCardToTime.DateTime.Date < DateTime.Now.Date)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.His_UCHein__TheBHYTDaHetHanSuDung), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), DefaultBoolean.True);
                        this.txtHeinCardToTime.Focus();
                    }
                    if (entity.IsInitFromCallPatientTypeAlter)
                    {
                        DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                        if (this.patientTypeAlterOld != null && patientTypeAlterOld.HEIN_CARD_TO_TIME == Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt))
                            return;
                        //else
                        //    CheckExamHistoryFromBHXHApi(this.txtSoThe.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHeinCardFromTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardFromTime.EditValue = dt;
                        this.dtHeinCardFromTime.Update();
                    }

                    this.dtHeinCardFromTime.Visible = true;
                    this.dtHeinCardFromTime.ShowPopup();
                    this.dtHeinCardFromTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCardFromTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardFromTime.EditValue = dt;
                        this.dtHeinCardFromTime.Update();
                    }
                    string heinCardNumber = this.txtSoThe.Text;
                    heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());
                    CheckExamHistoryFromBHXHApi(heinCardNumber);

                    this.txtHeinCardToTime.Focus();
                    this.txtHeinCardToTime.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardFromTime.EditValue = dt;
                        this.dtHeinCardFromTime.Update();
                    }

                    this.dtHeinCardFromTime.Visible = true;
                    this.dtHeinCardFromTime.ShowPopup();
                    this.dtHeinCardFromTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCardToTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardToTime.EditValue = dt;
                        this.dtHeinCardToTime.Update();
                    }

                    this.dtHeinCardToTime.Visible = true;
                    this.dtHeinCardToTime.Focus();
                    this.dtHeinCardToTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCardToTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardToTime.EditValue = dt;
                        this.dtHeinCardToTime.Update();
                    }

                    string heinCardNumber = this.txtSoThe.Text;
                    heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());
                    CheckExamHistoryFromBHXHApi(heinCardNumber);


                    // SendKeys.Send("{TAB}");
                    this.txtDu5Nam.Focus();
                    this.txtDu5Nam.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtHeinCardToTime.EditValue = dt;
                        this.dtHeinCardToTime.Update();
                    }

                    this.dtHeinCardToTime.Visible = true;
                    this.dtHeinCardToTime.ShowPopup();
                    this.dtHeinCardToTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaDKKCBBD_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadNoiDKKCBBDCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDKKCBBD_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDKKCBBD.EditValue != null)
                    {
                        this.MediOrgSelectRowChange(true, (cboNoiSong.EditValue ?? "").ToString());
                        string heinCardNumber = txtSoThe.Text;
                        heinCardNumber = heinCardNumber.Replace(" ", "").ToUpper().Trim();
                        heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber);
                        this.CheckExamHistoryFromBHXHApi(heinCardNumber);
                    }
                    else
                        this.rdoWrongRoute.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDKKCBBD_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDKKCBBD.EditValue != null)
                    {
                        this.MediOrgSelectRowChange(true);
                        string heinCardNumber = txtSoThe.Text;
                        heinCardNumber = heinCardNumber.Replace(" ", "").ToUpper().Trim();
                        heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber);
                        this.CheckExamHistoryFromBHXHApi(heinCardNumber);
                    }
                }
                else
                {
                    this.cboDKKCBBD.ShowPopup();
                    PopupProcess.SelectFirstRowPopup(this.cboDKKCBBD);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoWrongRoute_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.rdoWrongRoute.Checked)
                {
                    //if (MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == this.HeinLevelCodeCurrent)
                    //{
                    //    LogSystem.Debug("rdoWrongRoute_CheckedChanged => benh nhan tuyen huyen luon chon vao dung tuyen");
                    //    this.rdoWrongRoute.Checked = false;
                    //    this.rdoRightRoute.Checked = true;
                    //}
                    //else
                    //{
                    this.ChangeDefaultHeinRatio();
                    this.rdoRightRoute.Checked = false;
                    this.txtHeinRightRouteCode.Text = "";
                    this.cboHeinRightRoute.EditValue = null;
                    this.cboHeinRightRoute.Properties.Buttons[1].Visible = false;
                    this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER, true);
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoWrongRoute_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.rdoRightRoute.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoRightRoute_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.rdoRightRoute.Checked && !chkHasDobCertificate.Checked)
                {
                    this.ChangeDefaultHeinRatio();
                    this.rdoWrongRoute.Checked = false;
                    bool rightRoute = (MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == this.HeinLevelCodeCurrent
                                    || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE == this.HeinLevelCodeCurrent);
                    this.SetEnableControlHein(rightRoute ? RightRouterFactory.RIGHT_ROUTER : RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT, true);
                }
                ValidateRightRouteType(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidateRightRouteType(bool LoadDefault = true)
        {

            try
            {//&& !chkTt46.Checked
                var treatment = this.patientTypeAlterOld != null && this.patientTypeAlterOld.TREATMENT_ID > 0 ? His.UC.UCHein.HisTreatment.HisTreatmentGet.GetById(this.patientTypeAlterOld.TREATMENT_ID) : (this.entity.HisTreatment != null && this.entity.HisTreatment.ID > 0) ? this.entity.HisTreatment : null;
                if (LoadDefault)
                    SetDefaultRightCode();
                if (!this.isCallByRegistor && rdoRightRoute.Checked && this.cboDKKCBBD.EditValue != null && (string)this.cboDKKCBBD.EditValue != BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == treatment.BRANCH_ID).HEIN_MEDI_ORG_CODE && (!this.IsNotRequiredRightTypeInCaseOfHavingAreaCode || this.cboNoiSong.EditValue == null) &&!chkBaby.Checked && !chkHasAbsentLetter.Checked && !chkHasWorkingLetter.Checked && !chkTt46.Checked && (!ValidAcceptHeinMediOrgCode((string)this.cboDKKCBBD.EditValue, BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == treatment.BRANCH_ID).ACCEPT_HEIN_MEDI_ORG_CODE)
                        && !ValidSysMediOrgCode((string)this.cboDKKCBBD.EditValue, BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == treatment.BRANCH_ID).SYS_MEDI_ORG_CODE)))
                    ValidRightRouteType();
                else
                {
                    lblRightRouteType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    dxValidationProvider1.SetValidationRule(txtHeinRightRouteCode, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private bool SetDefaultRightCode()
        {
            bool result = false;
            try
            {
                var treatment = this.patientTypeAlterOld != null && this.patientTypeAlterOld.TREATMENT_ID > 0 ? His.UC.UCHein.HisTreatment.HisTreatmentGet.GetById(this.patientTypeAlterOld.TREATMENT_ID) : (this.entity.HisTreatment != null && this.entity.HisTreatment.ID > 0) ? this.entity.HisTreatment : null;
                if (ValidAcceptHeinMediOrgCode((string)this.cboDKKCBBD.EditValue, BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == treatment.BRANCH_ID).ACCEPT_HEIN_MEDI_ORG_CODE)
                        || ValidSysMediOrgCode((string)this.cboDKKCBBD.EditValue, BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == treatment.BRANCH_ID).SYS_MEDI_ORG_CODE))
                    rdoRightRoute.Checked = result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void rdoRightRoute_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.txtHeinRightRouteCode.Enabled)
                    {
                        this.txtHeinRightRouteCode.Focus();
                        this.txtHeinRightRouteCode.SelectAll();
                    }
                    else
                    {
                        this.cboNoiSong.Focus();
                        this.cboNoiSong.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinRightRouteCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadHeinRightRouterTypeCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboHeinRightRoute()
        {
            try
            {
                MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData data = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeStore.GetByCode((cboHeinRightRoute.EditValue ?? "").ToString());
                Inventec.Common.Logging.LogSystem.Debug("ChangecboHeinRightRoute" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                if (data != null && DataStore.HeinRightRouteTypes.Exists(o => o.HeinRightRouteTypeCode == data.HeinRightRouteTypeCode))
                {
                    this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;
                    this.txtHeinRightRouteCode.Text = data.HeinRightRouteTypeCode;
                    if (data.HeinRightRouteTypeCode == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                        this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTCC, false);
                    else if (data.HeinRightRouteTypeCode == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT)
                        this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_HASAPPOINTMENT, false);
                    else
                        this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTGT, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHeinRightRoute_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboHeinRightRoute.Properties.Buttons[1].Visible = false;
                    cboHeinRightRoute.EditValue = null;
                    txtHeinRightRouteCode.Text = "";
                    this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHeinRightRoute_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.dlgautoCheckCC != null)
                {
                    if (this.cboHeinRightRoute.EditValue != null
                        && this.cboHeinRightRoute.EditValue.ToString() == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                    {
                        this.dlgautoCheckCC(true);
                    }
                    else
                    {
                        this.dlgautoCheckCC(false);
                    }
                }
                this.cboHeinRightRoute.Properties.Buttons[1].Visible = (!String.IsNullOrEmpty((this.cboHeinRightRoute.EditValue ?? "").ToString()));

                if (this.cboHeinRightRoute.EditValue != null)
                    this.ChangecboHeinRightRoute();
                else
                    this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__DELETE_CHOICE_TYPE, false);

                ResetValidationRightRoute_Present();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //30383
        //Trong trường hợp người dùng ko chọn "trường hợp" là "chuyển tuyến" thì ko bắt buộc nhập các trường thông tin liên quan đến chuyển tuyến 
        //(vd: số chuyển viện, chuyển đúng tuyến/vượt tuyến, hình thức chuyển, ...)
        private void ResetValidationRightRoute_Present()
        {
            try
            {
                if (this.cboHeinRightRoute.EditValue == null ||
                    (this.cboHeinRightRoute.EditValue != null
                       && this.cboHeinRightRoute.EditValue.ToString() != MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT))
                {
                    this.dxValidationProvider1.SetValidationRule(this.txtMaNoiChuyenDen, null);
                    this.dxValidationProvider1.SetValidationRule(this.txtMaChanDoanTD, null);
                    this.dxValidationProvider1.SetValidationRule(this.txtInCode, null);
                    this.dxValidationProvider1.SetValidationRule(this.chkMediRecordRouteTransfer, null);
                    this.dxValidationProvider1.SetValidationRule(this.chkMediRecordNoRouteTransfer, null);
                    this.dxValidationProvider1.SetValidationRule(this.dtTransferInTimeFrom, null);
                    this.dxValidationProvider1.SetValidationRule(this.dtTransferInTimeTo, null);
                    this.dxValidationProvider1.SetValidationRule(this.txtMaHinhThucChuyen, null);
                    this.dxValidationProvider1.SetValidationRule(this.txtMaLyDoChuyen, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHeinRightRoute_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab )
            //    {
            //        if (lblMediRecordMediOrgForm.Enabled == true)
            //        {
            //            txtMaNoiChuyenDen.Focus();
            //            txtMaNoiChuyenDen.SelectAll();
            //        }
            //        else
            //        {
            //            FocusmoveOut();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboHeinRightRoute_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboHeinRightRoute.EditValue != null)
                    {
                        this.cboNoiSong.Focus();
                        this.cboNoiSong.SelectAll();
                        this.cboNoiSong.ShowPopup();
                    }
                    //        this.ChangecboHeinRightRoute();
                    //    if (this.cboHeinRightRoute.EditValue == null)
                    //        this.ChangecboHeinRightRoute();
                    ////    else
                    //this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__DELETE_CHOICE_TYPE, true);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHeinRightRoute_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboHeinRightRoute.EditValue == null)
                    {
                        this.cboNoiSong.Focus();
                        this.cboNoiSong.ShowPopup();
                    }

                    //if (this.cboHeinRightRoute.EditValue != null)
                    //    this.ChangecboHeinRightRoute();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaNoiChuyenDen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadNoiChuyenDenCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiChuyenDen_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboNoiChuyenDen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MEDI_ORG data = DataStore.MediOrgs.SingleOrDefault(o => o.MEDI_ORG_CODE.Equals(this.cboNoiChuyenDen.EditValue ?? ""));
                        if (data != null)
                        {
                            this.txtMaNoiChuyenDen.Text = data.MEDI_ORG_CODE;
                            this.cboNoiChuyenDen.Properties.Buttons[1].Visible = true;
                            this.ProcessLevelOfMediOrg();
                        }
                    }
                    this.txtMaChanDoanTD.Focus();
                    this.txtMaChanDoanTD.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiChuyenDen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboNoiChuyenDen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MEDI_ORG data = DataStore.MediOrgs.SingleOrDefault(o => o.MEDI_ORG_CODE.Equals(this.cboNoiChuyenDen.EditValue ?? ""));
                        if (data != null)
                        {
                            this.txtMaNoiChuyenDen.Text = data.MEDI_ORG_CODE;
                            this.cboNoiChuyenDen.Properties.Buttons[1].Visible = true;
                            this.ProcessLevelOfMediOrg();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMediRecordRouteTransfer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.chkMediRecordNoRouteTransfer.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMediRecordRouteTransfer_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkMediRecordRouteTransfer.Checked && this.chkMediRecordNoRouteTransfer.Checked)
                {
                    this.chkMediRecordNoRouteTransfer.Checked = !this.chkMediRecordRouteTransfer.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMediRecordNoRouteTransfer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //this.txtMaHinhThucChuyen.Focus();
                    //this.txtMaHinhThucChuyen.SelectAll();
                    dtTransferInTimeFrom.Focus();
                    dtTransferInTimeFrom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMediRecordNoRouteTransfer_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkMediRecordRouteTransfer.Checked && this.chkMediRecordNoRouteTransfer.Checked)
                    this.chkMediRecordRouteTransfer.Checked = !this.chkMediRecordNoRouteTransfer.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaHinhThucChuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadTranPatiFormCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHinhThucChuyen_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboHinhThucChuyen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = DataStore.TranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboHinhThucChuyen.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.txtMaHinhThucChuyen.Text = data.TRAN_PATI_FORM_CODE;
                            this.cboHinhThucChuyen.Properties.Buttons[1].Visible = true;
                        }
                    }

                    this.txtMaLyDoChuyen.Focus();
                    this.txtMaLyDoChuyen.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHinhThucChuyen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboHinhThucChuyen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = DataStore.TranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboHinhThucChuyen.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.txtMaHinhThucChuyen.Text = data.TRAN_PATI_FORM_CODE;
                            //this.txtMaLyDoChuyen.Focus();
                            //this.txtMaLyDoChuyen.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHinhThucChuyen_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboHinhThucChuyen.Properties.Buttons[1].Visible = (!String.IsNullOrEmpty((this.cboHinhThucChuyen.EditValue ?? "").ToString()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHinhThucChuyen_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboHinhThucChuyen.EditValue = null;
                    this.cboHinhThucChuyen.Properties.Buttons[1].Visible = false;
                    this.txtMaHinhThucChuyen.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMaLyDoChuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadTranPatiReasonCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLyDoChuyen_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboLyDoChuyen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = DataStore.TranPatiReasons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboLyDoChuyen.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.cboLyDoChuyen.Properties.Buttons[1].Visible = true;
                            this.txtMaLyDoChuyen.Text = data.TRAN_PATI_REASON_CODE;
                        }
                    }
                    this.FocusMoveOut();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLyDoChuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboLyDoChuyen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = DataStore.TranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboLyDoChuyen.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.cboLyDoChuyen.Properties.Buttons[1].Visible = true;
                            this.FocusMoveOut();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiSong_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IsNotRequiredRightTypeInCaseOfHavingAreaCode), IsNotRequiredRightTypeInCaseOfHavingAreaCode) + Inventec.Common.Logging.LogUtil.TraceData("this.cboNoiSong.EditValue", this.cboNoiSong.EditValue) + Inventec.Common.Logging.LogUtil.TraceData("this.cboDKKCBBD.EditValue", this.cboDKKCBBD.EditValue));

                    ValidateRightRouteType();
                    if (IsNotRequiredRightTypeInCaseOfHavingAreaCode)
                    {
                        string liveArea = (this.cboNoiSong.EditValue ?? "").ToString();
                        His.UC.UCHein.Data.MediOrgADO mediorg = DataStore.MediOrgs.SingleOrDefault(o => o.MEDI_ORG_CODE == (this.cboDKKCBBD.EditValue ?? "").ToString());
                        if (mediorg != null)
                            this.HasChangeValidRightRouteType(mediorg.MEDI_ORG_CODE, liveArea);

                        if ((!String.IsNullOrEmpty(liveArea) && (liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K1 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K2 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K3)))
                        {
                            lblRightRouteType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                            dxValidationProvider1.SetValidationRule(txtHeinRightRouteCode, null);
                            this.rdoRightRoute.Checked = true;
                            this.dxValidationProvider1.RemoveControlError(cboHeinRightRoute);
                        }
                    }

                    if (this.cboNoiSong.EditValue != null)
                    {
                        this.ChangeDefaultHeinRatio();
                        this.cboNoiSong.Properties.Buttons[1].Visible = true;

                        if (txtMaNoiChuyenDen.Enabled == true)
                        {
                            txtMaNoiChuyenDen.Focus();
                            txtMaNoiChuyenDen.SelectAll();
                        }
                        else
                        {
                            txtHNCode.Focus();
                            txtHNCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboNoiSong.Focus();
                        this.cboNoiSong.SelectAll();
                        this.cboNoiSong.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiSong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (IsNotRequiredRightTypeInCaseOfHavingAreaCode)
                    {
                        string liveArea = (this.cboNoiSong.EditValue ?? "").ToString();
                        His.UC.UCHein.Data.MediOrgADO mediorg = DataStore.MediOrgs.SingleOrDefault(o => o.MEDI_ORG_CODE == (this.cboDKKCBBD.EditValue ?? "").ToString());
                        if (mediorg != null)
                            this.HasChangeValidRightRouteType(mediorg.MEDI_ORG_CODE, liveArea);

                        if ((!String.IsNullOrEmpty(liveArea) && (liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K1 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K2 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K3)))
                        {
                            this.rdoRightRoute.Checked = true;
                            this.dxValidationProvider1.RemoveControlError(cboHeinRightRoute);
                        }
                    }

                    if (this.cboNoiSong.EditValue != null)
                    {
                        this.ChangeDefaultHeinRatio();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiSong_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboNoiSong.EditValue = null;
                    this.cboNoiSong.Properties.Buttons[1].Visible = false;

                    His.UC.UCHein.Data.MediOrgADO mediorg = DataStore.MediOrgs.SingleOrDefault(o => o.MEDI_ORG_CODE == (this.cboDKKCBBD.EditValue ?? "").ToString());
                    if (mediorg != null && IsNotRequiredRightTypeInCaseOfHavingAreaCode)
                        this.HasChangeValidRightRouteType(mediorg.MEDI_ORG_CODE);

                    this.ChangeDefaultHeinRatio();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNoiSong_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboNoiSong.Properties.Buttons[1].Visible = (this.cboNoiSong.EditValue != null);

                //30383
                // Khi cấu hình "HIS.Desktop.Plugins.Register.IsNotRequiredRightTypeInCaseOfHavingAreaCode" có giá trị = 1, 
                //nếu người dùng nhập thông tin khu vực (K1, K2, K3) và chọn "Đúng tuyến" thì ko bắt buộc nhập "trường hợp"

                if (cboNoiSong.EditValue != null && rdoRightRoute.Checked && this.IsNotRequiredRightTypeInCaseOfHavingAreaCode)
                {
                    this.dxValidationProvider1.SetValidationRule(this.txtHeinRightRouteCode, null);
                    this.lblRightRouteType.AppearanceItemCaption.ForeColor = Color.Black;
                }
                ResetValidationRightRoute_Present();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChanDoanTD_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (this.cboChanDoanTD.EditValue != null)
                        this.ChangecboChanDoanTD();
                    else if (this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdName))
                        this.ChangecboChanDoanTD_V2_GanICDNAME(this._TextIcdName);
                    else
                        SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboChanDoanTD_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboChanDoanTD.EditValue != null)
                        this.ChangecboChanDoanTD();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkJoin5Year_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.chkPaid6Month.Enabled)
                        this.chkPaid6Month.Focus();
                    else
                    {
                        if (this.txtFreeCoPainTime.Enabled)
                        {
                            this.txtFreeCoPainTime.Focus();
                            this.txtFreeCoPainTime.SelectAll();
                        }
                        else
                        {
                            this.cboNoiSong.Focus();
                            this.cboNoiSong.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Join5YearAndPaid6MonthCheckedChanged()
        {
            try
            {
                if (chkJoin5Year.Checked && chkPaid6Month.Checked)
                {
                    lciFreeCoPainTime.AppearanceItemCaption.ForeColor = Color.Maroon;
                    this.ValidFreeCoPainTime(true);
                }
                else if (!chkPaid6Month.Checked)
                {
                    lciFreeCoPainTime.AppearanceItemCaption.ForeColor = Color.Black;
                    this.ValidFreeCoPainTime(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPaid6Month_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.ChangeDefaultHeinRatio();
                this.Join5YearAndPaid6MonthCheckedChanged();
                if (entity.IsInitFromCallPatientTypeAlter)
                {
                    this.ValidateCheckBox6M(chkPaid6Month.Checked);
                    if (!chkJoin5Year.Checked && !chkPaid6Month.Checked)
                        IsShowMessage = false;
                    else if (chkJoin5Year.Checked && chkPaid6Month.Checked)
                        IsShowMessage = true;
                    if (chkPaid6Month.Checked && (chkPaid6Month.OldEditValue != chkPaid6Month.EditValue))
                    {
                        this.ShowMessageNotAutoCheck5Y6M(this.chkPaid6Month);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateCheckBox6M(bool IsVlid)
        {
            try
            {
                if (IsVlid)
                {
                    lciFreeCoPainTime.AppearanceItemCaption.ForeColor = Color.Maroon;
                    TemplateHeinBHYT1__CheckBox__ValidationRule vld = new TemplateHeinBHYT1__CheckBox__ValidationRule();
                    vld.chk = chkPaid6Month;
                    vld.txt = txtFreeCoPainTime;
                    vld.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    this.dxValidationProvider1.SetValidationRule(this.txtFreeCoPainTime, vld);
                }
                else
                {
                    lciFreeCoPainTime.AppearanceItemCaption.ForeColor = Color.Black;
                    this.dxValidationProvider1.SetValidationRule(this.txtFreeCoPainTime, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateCheckBox5Y(bool IsVlid)
        {
            try
            {
                if (IsVlid)
                {
                    lciDu5Nam.AppearanceItemCaption.ForeColor = Color.Maroon;
                    TemplateHeinBHYT1__CheckBox__ValidationRule vld = new TemplateHeinBHYT1__CheckBox__ValidationRule();
                    vld.chk = chkJoin5Year;
                    vld.txt = txtDu5Nam;
                    vld.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    this.dxValidationProvider1.SetValidationRule(this.txtDu5Nam, vld);
                }
                else
                {
                    lciDu5Nam.AppearanceItemCaption.ForeColor = Color.Black;
                    this.dxValidationProvider1.SetValidationRule(this.txtDu5Nam, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowMessageNotAutoCheck5Y6M(CheckEdit checkEdit)
        {
            try
            {
                IsShowMessage = IsShowMessage && (chkJoin5Year.Checked || chkPaid6Month.Checked);
                if (!HisConfigCFG.IsNotAutoCheck5Y6M || IsShowMessage || IsAutoCheck || (!chkJoin5Year.Checked && !chkPaid6Month.Checked)) return;
                IsShowMessage = true;
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân phải có giấy chứng nhận không cùng chi trả trong năm. Bạn có muốn tiếp tục?", MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    checkEdit.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPaid6Month_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.txtFreeCoPainTime.Enabled)
                    {
                        this.txtFreeCoPainTime.Focus();
                        this.txtFreeCoPainTime.SelectAll();
                    }
                    else
                    {
                        this.cboNoiSong.Focus();
                        this.cboNoiSong.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtMaDKKCBBD.Focus();
                    this.txtMaDKKCBBD.SelectAll();
                    if (this.patientTypeAlterOld != null && !this.txtAddress.Text.Equals(this.patientTypeAlterOld.ADDRESS))
                    {
                        string heinCardNumber = this.txtSoThe.Text;
                        heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());
                        CheckExamHistoryFromBHXHApi(heinCardNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTempQN_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.IsTempQN)
                {
                    if (this.chkTempQN.Checked)
                    {

                        this.DisableControlWhenPatientTypeQN(IsTempQN, true);
                        //lblRightRouteType.AppearanceItemCaption.ForeColor = new Color();
                        //lblHeincardNumber.AppearanceItemCaption.ForeColor = new Color();
                        //lblHeincardToDate.AppearanceItemCaption.ForeColor = new Color();
                        //lblHeincardFromDate.AppearanceItemCaption.ForeColor = new Color();
                        //txtHeinCardToTime.Enabled = false;
                        //txtHeinCardFromTime.Enabled = false;
                        //txtSoThe.Enabled = false;
                        //txtHeinCardToTime.EditValue = null;
                        //dtHeinCardFromTime.EditValue = null;
                        //txtHeinCardFromTime.EditValue = null;
                        //dtHeinCardToTime.EditValue = null;
                        //txtSoThe.EditValue = null;
                        //cboSoThe.EditValue = null;
                        //this.ValidAddress();
                        //this.ValidRightRouteType();
                        //this.ValidHNCode();
                        //this.ValidNoiChuyenDen();
                        //this.ValidIcd();
                        //this.ValidNoiDKKCBBD();


                        //dxValidationProvider1.SetValidationRule(txtSoThe, null);
                        //dxValidationProvider1.SetValidationRule(txtFreeCoPainTime, null);
                        //dxValidationProvider1.SetValidationRule(txtHeinCardToTime, null);
                        //dxValidationProvider1.SetValidationRule(txtHeinCardFromTime, null);
                        //dxValidationProvider1.SetValidationRule(txtHeinRightRouteCode, null);
                        //dxValidationProvider1.SetValidationRule(txtHNCode, null);
                        //dxValidationProvider1.SetValidationRule(txtMaNoiChuyenDen, null);
                        //dxValidationProvider1.SetValidationRule(txtMaChanDoanTD, null);
                    }
                    else
                    {
                        this.DisableControlWhenPatientTypeQN(IsTempQN, false);
                        //lblHeincardNumber.AppearanceItemCaption.ForeColor = Color.Maroon;
                        //lblHeincardToDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                        //lblHeincardFromDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                        //lblRightRouteType.AppearanceItemCaption.ForeColor = Color.Maroon;

                        //txtHeinCardToTime.Enabled = true;
                        //txtHeinCardFromTime.Enabled = true;
                        //txtSoThe.Enabled = true;

                        //this.ValidTxtSoThe();
                        //this.ValidFreeCoPainTime();
                        //this.ValidHeinCardToTime();
                        //this.ValidHeinCardFromTime();
                        //this.ValidRightRouteType();
                        //this.ValidHNCode();
                        //this.ValidNoiChuyenDen();
                        //this.ValidIcd();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidateTranferMediOrg()
        {
            try
            {
                this._TextIcdName = "";
                bool _isPresent = ((string)cboHeinRightRoute.EditValue == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _isPresent), _isPresent) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ObligatoryTranferMediOrg), ObligatoryTranferMediOrg));
                if (_isPresent && this.ObligatoryTranferMediOrg == "1")
                {
                    lblMediRecordMediOrgForm.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciIcdMain.AppearanceItemCaption.ForeColor = Color.Maroon;
                    this.ValidNoiChuyenDen();
                    this.ValidIcdByDTGT();
                }
                else if (_isPresent && this.ObligatoryTranferMediOrg == "3")
                {
                    lblMediRecordMediOrgForm.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciIcdMain.AppearanceItemCaption.ForeColor = Color.Maroon;
                    this.ValidNoiChuyenDen();
                    this.ValidIcdByDTGT();

                    //if (_isValidateAll)
                    //{
                    this.ValidationSingleControl(dtTransferInTimeFrom, dxValidationProvider1);
                    this.ValidationSingleControl(dtTransferInTimeTo, dxValidationProvider1);
                    this.ValidationSingleControl(txtInCode, dxValidationProvider1);
                    this.ValidChuyenTuyen();
                    this.ValidateLookupWithTextEdit(cboHinhThucChuyen, txtMaHinhThucChuyen, dxValidationProvider1);
                    this.ValidateLookupWithTextEdit(cboLyDoChuyen, txtMaLyDoChuyen, dxValidationProvider1);

                    this.lciFordtTransferInTimeFrom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciFordtTransferInTimeTo.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciInCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciMediRecordRouteTransfer.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciTransPatiFormCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciTransPatiReasonCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    //}
                }
                else
                {
                    this.dxValidationProvider1.SetValidationRule(this.txtMaChanDoanTD, null);
                    this.lciIcdMain.AppearanceItemCaption.ForeColor = Color.Black;

                    this.dxValidationProvider1.SetValidationRule(dtTransferInTimeFrom, null);
                    this.dxValidationProvider1.SetValidationRule(dtTransferInTimeTo, null);
                    this.dxValidationProvider1.SetValidationRule(txtInCode, null);
                    this.dxValidationProvider1.SetValidationRule(chkMediRecordRouteTransfer, null);
                    this.dxValidationProvider1.SetValidationRule(txtMaHinhThucChuyen, null);
                    this.dxValidationProvider1.SetValidationRule(txtMaLyDoChuyen, null);
                    this.dxValidationProvider1.SetValidationRule(cboHinhThucChuyen, null);
                    this.dxValidationProvider1.SetValidationRule(cboLyDoChuyen, null);
                    this.dxValidationProvider1.SetValidationRule(txtMaNoiChuyenDen, null);

                    this.lciFordtTransferInTimeFrom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lciFordtTransferInTimeTo.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lciInCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lciMediRecordRouteTransfer.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lciTransPatiFormCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lciTransPatiReasonCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.lblMediRecordMediOrgForm.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
