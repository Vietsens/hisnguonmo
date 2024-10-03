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
using HIS.UC.DateEditor.ADO;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.UC.DateEditor.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.UC.DateEditor.Resources;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.Controls;

namespace HIS.UC.DateEditor
{
    public partial class UCDate : UserControl
    {
        DateInitADO InitAdo { get; set; }
        LanguageInputADO languageInputADO;
        int positionHandle = -1;
        List<DateTime?> intructionTimeSelecteds = new List<DateTime?>();
        long intructionTimeSelected = 0;
        DateTime timeSelested = new DateTime();
        DelegateSelectMultiDate selectMultiDate;
        DelegatNextFocus nextFocus;
        DelegateChangeIntructionTime changeIntructionTime;
        bool isInitForm = true;
        bool isStopEventChangeMultiDate;

        public UCDate()
            : this(null)
        {

        }

        public UCDate(DateInitADO data)
        {
            InitializeComponent();
            if (data != null)
            {
                isInitForm = true;
                this.InitAdo = data;
                this.languageInputADO = data.LanguageInputADO;
                if (data.Height > 0 && data.Width > 0)
                {
                    this.Size = new Size(data.Width, data.Height);
                }
                if (data.DelegateNextFocus != null)
                    nextFocus = data.DelegateNextFocus;
                if(data.DateInputADO != null)
                    chkMultiIntructionTime.Enabled = !data.DateInputADO.IsDutruTime ?? true;// có thời gian dự trù thì disable check chọn nhiều ngày y lệnh

                if (data.DelegateSelectMultiDate != null)
                    selectMultiDate = data.DelegateSelectMultiDate;

                if (data.DelegateChangeIntructionTime != null)
                    changeIntructionTime = data.DelegateChangeIntructionTime;

                if (data.IsValidate)
                    lciDateEditor.AppearanceItemCaption.ForeColor = Color.Maroon;
                else
                    lciDateEditor.AppearanceItemCaption.ForeColor = Color.Black;
                SetResourceMessage();
                SetCaptionByLanguageKey();
            }
        }

        private void SetResourceMessage()
        {
            try
            {
                ResourceMessage.UCDate__CaptionlciDateEditor = languageInputADO.UCDate__CaptionlciDateEditor;
                ResourceMessage.UCDate__CaptionchkMultiIntructionTime = languageInputADO.UCDate__CaptionchkMultiIntructionTime;

                ResourceMessage.ChuaChonNgayChiDinh = languageInputADO.ChuaChonNgayChiDinh;
                ResourceMessage.FormMultiChooseDate__CaptionBtnChoose = languageInputADO.FormMultiChooseDate__CaptionBtnChoose;
                ResourceMessage.FormMultiChooseDate__CaptionCalendaInput = languageInputADO.FormMultiChooseDate__CaptionCalendaInput;
                ResourceMessage.FormMultiChooseDate__CaptionText = languageInputADO.FormMultiChooseDate__CaptionText;
                ResourceMessage.FormMultiChooseDate__CaptionTimeInput = languageInputADO.FormMultiChooseDate__CaptionTimeInput;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                lciDateEditor.Text = ResourceMessage.UCDate__CaptionlciDateEditor;
                chkMultiIntructionTime.Text = ResourceMessage.UCDate__CaptionchkMultiIntructionTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCIcd_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.InitAdo.IsValidate)
                    this.ValidationSingleControl(this.dtInstructionTime, this.dxValidationProvider1, "", IsValidInstructionTime);
                lcichkMultiDate.Visibility = (this.InitAdo.IsVisibleMultiDate ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                if (InitAdo.DateInputADO != null)
                {
                    SetValue(InitAdo.DateInputADO);
                    isInitForm = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool IsValidInstructionTime()
        {
            bool valid = true;
            try
            {
                if (!chkMultiIntructionTime.Checked)
                {
                    valid = ((this.dtInstructionTime.EditValue != null && !String.IsNullOrEmpty(this.dtInstructionTime.Text) && this.dtInstructionTime.DateTime != DateTime.MinValue)) && this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0;
                }
                else
                {
                    valid = (!String.IsNullOrEmpty(txtInstructionTime.Text)) && this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0;
                }
                Inventec.Common.Logging.LogSystem.Debug("IsValidInstructionTime.valid =" + valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void dtInstructionTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Thay đổi ngày chỉ định, phải load lại đối tượng thanh toán của BN tương ứng với ngày đó
                if (!this.isInitForm)
                {
                    this.ChangeIntructionTime(this.dtInstructionTime.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeIntructionTime(DateTime intructTime)
        {
            try
            {
                System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
                this.timeSelested = today.Add(this.timeIntruction.TimeSpan);
                if (chkMultiIntructionTime.Checked)
                {
                    if (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 && intructTime != DateTime.MinValue)
                    {
                        for (int i = 0, j = this.intructionTimeSelecteds.Count; i < j; i++)
                        {
                            this.intructionTimeSelecteds[i] = new DateTime(this.intructionTimeSelecteds[i].Value.Year, this.intructionTimeSelecteds[i].Value.Month, this.intructionTimeSelecteds[i].Value.Day, intructTime.Hour, intructTime.Minute, 0);
                        }
                        this.intructionTimeSelected = Inventec.Common.TypeConvert.Parse.ToInt64(intructionTimeSelecteds.FirstOrDefault().Value.ToString("yyyyMMddHHmm") + "00");
                        if (changeIntructionTime != null)
                        {
                            changeIntructionTime(intructTime);
                        }
                    }
                }
                else
                {
                    this.intructionTimeSelecteds = new List<DateTime?>();
                    this.intructionTimeSelected = Inventec.Common.TypeConvert.Parse.ToInt64(intructTime.ToString("yyyyMMdd") + this.timeSelested.ToString("HHmm") + "00");
                    this.intructionTimeSelecteds.Add(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelected));
                    if (changeIntructionTime != null)
                    {
                        changeIntructionTime(this.dtInstructionTime.DateTime);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInstructionTime_TabMedicine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    timeIntruction.Focus();
                    timeIntruction.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInstructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    timeIntruction.Focus();
                    timeIntruction.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInstructionTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    frmMultiIntructonTime frmChooseIntructionTime = new frmMultiIntructonTime(intructionTimeSelecteds, timeSelested, SelectMultiIntructionTime);
                    frmChooseIntructionTime.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectMultiIntructionTime(List<DateTime?> datas, DateTime time)
        {
            try
            {
                if (datas != null && time != DateTime.MinValue)
                {
                    this.intructionTimeSelecteds = datas as List<DateTime?>;
                    string strTimeDisplay = "";
                    int num = 0;
                    this.intructionTimeSelecteds = this.intructionTimeSelecteds.OrderBy(o => o.Value).ToList();
                    foreach (var item in this.intructionTimeSelecteds)
                    {
                        if (item != null && item.Value != DateTime.MinValue)
                        {
                            strTimeDisplay += (num == 0 ? "" : "; ") + item.Value.ToString("dd/MM");
                            num++;
                        }
                    }
                    if (this.txtInstructionTime.Text != strTimeDisplay)
                    {
                        //Trường hợp chọn nhiều ngày chỉ định thì lấy đối tượng bệnh nhân tuong uong voi intructiontime dau tien duoc chon
                        //Vì các dữ liệu liên quan như chính sách giá, đối tượng chấp nhận thanh toán phải suy ra từ đối tượng BN ở trên
                        this.isInitForm = true;
                        this.timeSelested = time;
                        this.timeIntruction.EditValue = this.timeSelested.ToString("HH:mm");
                        this.txtInstructionTime.Text = strTimeDisplay;
                        this.isInitForm = false;
                    }
                }

                if (selectMultiDate != null)
                {
                    selectMultiDate(datas, time);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMultiIntructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    NextFocus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMultiIntructionTime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isStopEventChangeMultiDate)
                {
                    return;
                }

                this.txtInstructionTime.Visible = this.chkMultiIntructionTime.Checked;
                this.dtInstructionTime.Visible = !this.chkMultiIntructionTime.Checked;

                if (this.chkMultiIntructionTime.Checked)
                {
                    this.timeIntruction.EditValue = DateTime.Now.ToString("HH:mm");
                    string strTimeDisplay = DateTime.Now.ToString("dd/MM");
                    this.txtInstructionTime.Text = strTimeDisplay;
                }

                if (this.InitAdo != null && this.InitAdo.DelegateCheckMultiDate != null)
                    this.InitAdo.DelegateCheckMultiDate(this.chkMultiIntructionTime.Checked);

                if (this.InitAdo != null && this.InitAdo.DelegateMultiDateChanged != null)
                {
                    this.InitAdo.DelegateMultiDateChanged();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timeIntruction_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.chkMultiIntructionTime.Enabled || lcichkMultiDate.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        this.chkMultiIntructionTime.Focus();
                    }
                    else
                    {
                        this.NextFocus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timeIntruction_Leave(object sender, EventArgs e)
        {
            try
            {
                //Thay đổi ngày chỉ định, phải load lại đối tượng thanh toán của BN tương ứng với ngày đó
                if (!this.isInitForm)
                {
                    this.ChangeIntructionTime(this.dtInstructionTime.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(DateInputADO input)
        {
            try
            {
                if (input != null && input.Time != DateTime.MinValue && input.Dates != null && input.Dates.Count > 0)
                {
                    this.InitAdo.DateInputADO = input;

                    this.timeIntruction.EditValue = input.Time.ToString("HH:mm");
                    this.dtInstructionTime.EditValue = input.Time;
                    this.intructionTimeSelecteds = new List<DateTime?>();
                    this.intructionTimeSelecteds.AddRange(input.Dates);
                }
                else
                {
                    this.txtInstructionTime.Visible = false;
                    this.dtInstructionTime.Visible = true;
                    this.timeIntruction.EditValue = DateTime.Now.ToString("HH:mm");
                    this.dtInstructionTime.EditValue = DateTime.Now;
                    this.intructionTimeSelecteds = new List<DateTime?>();
                    this.intructionTimeSelecteds.Add(this.dtInstructionTime.DateTime);
                }
                
                System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
                timeSelested = today.Add(timeIntruction.TimeSpan);
                this.intructionTimeSelected = (this.intructionTimeSelecteds.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).First());
                chkMultiIntructionTime.Checked = false;
                isInitForm = false;
                if(input.IsDutruTime != null)
                    chkMultiIntructionTime.Enabled = !input.IsDutruTime??true;
                
                if (input != null && input.IsVisibleMultiDate.HasValue)
                {
                    lcichkMultiDate.Visibility = (input.IsVisibleMultiDate.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusControl()
        {
            try
            {
                dtInstructionTime.Focus();
                dtInstructionTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue(DateInputADO input)
        {
            try
            {
                if (input != null)
                {
                    this.InitAdo.DateInputADO = input;
                    if (input.Time != null && input.Time != DateTime.MinValue)
                    {
                        this.timeIntruction.EditValue = input.Time.ToString("HH:mm");
                    }
                    if (input.Dates != null && input.Dates.Count > 0)
                    {
                        this.dtInstructionTime.EditValue = input.Dates[0];
                        this.intructionTimeSelecteds = new List<DateTime?>();
                        this.intructionTimeSelecteds.AddRange(input.Dates);
                    }
                    this.intructionTimeSelected = (this.intructionTimeSelecteds.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).First());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue(DateInputHasCheckADO input)
        {
            try
            {
                if (input != null)
                {
                    this.InitAdo.DateInputADO = new DateInputADO();
                    this.InitAdo.DateInputADO.Dates = input.Dates;
                    this.InitAdo.DateInputADO.IsVisibleMultiDate = input.IsVisibleMultiDate;
                    this.InitAdo.DateInputADO.Time = input.Time;
                    this.isInitForm = true;
                    if (input.Time != null && input.Time != DateTime.MinValue)
                    {
                        this.timeIntruction.EditValue = input.Time.ToString("HH:mm");
                    }
                    this.timeSelested = input.Time;

                    this.isStopEventChangeMultiDate = true;
                    this.chkMultiIntructionTime.Checked = input.IsMultiDayChecked;
                    this.txtInstructionTime.Visible = input.IsMultiDayChecked;
                    this.dtInstructionTime.Visible = !input.IsMultiDayChecked;

                    if (input.IsMultiDayChecked)
                    {
                        this.SelectMultiIntructionTime(input.Dates, input.Time);
                    }
                    else
                    {
                        if (input.Dates != null && input.Dates.Count > 0)
                        {
                            this.dtInstructionTime.EditValue = input.Dates[0];
                            this.intructionTimeSelecteds = new List<DateTime?>();
                            this.intructionTimeSelecteds.AddRange(input.Dates);
                        }
                        this.intructionTimeSelected = (this.intructionTimeSelecteds.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).First());

                    }
                    this.isInitForm = false;
                    this.isStopEventChangeMultiDate = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReadOnly(bool isReadOnly)
        {
            try
            {
                //chkMultiIntructionTime.ReadOnly = isReadOnly;
                //chkMultiIntructionTime.Enabled = !isReadOnly;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void EnableControl(bool isEnable)
        {
            try
            {
                if(!isEnable)
				{
                    chkMultiIntructionTime.Checked = false;                
				}                  
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public bool ValidationForm()
        {
            bool result = false;
            try
            {
                this.positionHandle = -1;
                if (dxValidationProvider1.Validate())
                    result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public bool ValidationFormWithMessage(List<string> errorEmpty, List<string> errorOther)
        {
            bool result = false;
            try
            {
                this.positionHandle = -1;
                if (dxValidationProvider1.Validate())
                    result = true;
                if (!result)
                {
                    var invalidControls = dxValidationProvider1.GetInvalidControls();
                    if (invalidControls != null && invalidControls.Count > 0)
                    {
                        foreach (System.Windows.Forms.Control c in invalidControls)
                        {
                            string errorC = lciDateEditor.Text.Replace(":", "");
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

        public bool ResetValidation()
        {
            bool result = false;
            try
            {
                dtInstructionTime.ErrorText = "";
                dxValidationProvider1.RemoveControlError(dtInstructionTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public List<long> GetValue()
        {
            List<long> result = new List<long>();
            try
            {
                System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 2);
                timeSelested = today.Add(timeIntruction.TimeSpan);
                if (chkMultiIntructionTime.Checked)
                {
                    result = (intructionTimeSelecteds.Where(o => o.Value != DateTime.MinValue).Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).ToList());
                }
                else
                {
                    result = new List<long>();
                    if (dtInstructionTime.DateTime != DateTime.MinValue)
                    {
                        result.Add(Inventec.Common.TypeConvert.Parse.ToInt64(dtInstructionTime.DateTime.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00"));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public bool GetChkMultiDateState()
        {
            bool result = false;
            try
            {
                result = (chkMultiIntructionTime.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public void NextFocus()
        {
            try
            {
                if (this.nextFocus != null)
                {
                    this.nextFocus();
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
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

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, string messageErr, IsValidControl isValidControl)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                if (isValidControl != null)
                {
                    validRule.isValidControl = isValidControl;
                    validRule.isUseOnlyCustomValidControl = true;
                }
                if (!String.IsNullOrEmpty(messageErr))
                    validRule.ErrorText = messageErr;
                else
                    validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
