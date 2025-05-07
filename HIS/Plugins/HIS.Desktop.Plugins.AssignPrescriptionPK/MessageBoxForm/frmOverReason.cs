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
using DevExpress.XtraBars.MessageFilter;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Validate.ValidateRule;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraPrinting.Export.Pdf.PdfImageCache;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm
{
    public partial class frmOverReason : Form
    {
        Action<TreatmentOverReason> reason;
        Action<bool> IsYes;
        private int positionHandleControl;
        string content;
        MediMatyTypeADO Data;
        bool IsUpdateGrid;
        bool IsAssignPres;
        Action<MOS.SDO.HisGfrAlertLogSDO> ActionSendAlert { get; set; }
        List<HIS_MEDICINE_SERVICE> MedicineServices;
        decimal MLCT = 0;
        HIS_MEDICINE_SERVICE Ms = null;
        long treatmentId = 0;
        decimal AmountInDay = 0;
        DateTime dtWarningTime = DateTime.MinValue;
        bool IsWarning = false;
        public frmOverReason(List<HIS_MEDICINE_SERVICE> MedicineServices, string content, Action<TreatmentOverReason> reason, Action<bool> IsYes, MediMatyTypeADO Data, bool IsUpdateGrid, bool IsAssignPres, long treatmentId, Action<HisGfrAlertLogSDO> ActionSendAlert = null, decimal MLCT = 0, HIS_MEDICINE_SERVICE Ms = null, decimal AmountInDay = 0)
        {
            InitializeComponent();
            try
            {
                this.ActionSendAlert = ActionSendAlert;
                this.AmountInDay = AmountInDay;
                this.IsUpdateGrid = IsUpdateGrid;
                this.content = content;
                this.reason = reason;
                this.IsYes = IsYes;
                this.Data = Data;
                this.IsAssignPres = IsAssignPres;
                this.MedicineServices = MedicineServices;
                this.MLCT = MLCT;
                this.Ms = Ms;
                this.treatmentId = treatmentId;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        List<HIS_OVERDOSE_REASON> OverDoseReason = new List<HIS_OVERDOSE_REASON>();
        public void InitComboReason()
        {
            try
            {
                OverDoseReason = BackendDataWorker.Get<HIS_OVERDOSE_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("OVERDOSE_REASON_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("OVERDOSE_REASON_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("OVERDOSE_REASON_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboReason, OverDoseReason, controlEditorADO);
                cboReason.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void frmOverResultTestReason_Load(object sender, EventArgs e)
        {  
            try
            {
                InitComboReason();
                if (IsAssignPres)
                {
                    dtWarningTime = DateTime.Now;
                    lblTitle.Text = "Thuốc {4}\r\nMức lọc cầu thận của bệnh nhân là {0}\r\n{1}.\r\n{2}Bạn có muốn {3} không?\r\nTrong trường hợp chọn \"Có\", vui lòng nhập lý do.\r\n";
                    lblTitle.Text = string.Format(lblTitle.Text, Math.Round(MLCT, 4), Ms.WARNING_CONTENT, (Ms.AMOUNT_INDAY_FROM ?? 0) == 0 ? "" : string.Format("Bạn đang kê vượt liều {0}\r\n", Math.Round((Data.AMOUNT ?? 0) - (Ms.AMOUNT_INDAY_FROM ?? 0) * (Data.UseDays ?? 1) + AmountInDay, 2)), content, Data.MEDICINE_TYPE_NAME);
                    if ((Ms.AMOUNT_INDAY_FROM ?? 0) <= 0)
                        pbImage.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.block;
                    else
                    {
                        IsWarning = true;
                        pbImage.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.warning;
                    }
                    if (!string.IsNullOrEmpty(Data.OVER_KIDNEY_REASON))

                        memReason.Text = Data.OVER_KIDNEY_REASON;

                }
                else
                {
                    lblTitle.Text = "{0}\r\nBạn có muốn {1} không?\r\nTrong trường hợp chọn \"Có\", vui lòng nhập lý do.\r\n";
                    lblTitle.Text = string.Format(lblTitle.Text, string.Join(",", MedicineServices.Select(o => o.WARNING_CONTENT)), content);
                    layoutControlItem2.AppearanceItemCaption.ForeColor = Color.Maroon;
                    if (!string.IsNullOrEmpty(Data.OVER_RESULT_TEST_REASON))

                        memReason.Text = Data.OVER_RESULT_TEST_REASON;
                }

                if (Data.OVER_REASON_ID != null)
                {
                    var oved = OverDoseReason.FirstOrDefault(o => o.ID == Data.OVER_REASON_ID);
                    if (oved != null)
                    {
                        memReason.Text = ReplaceFirstOccurrence(memReason.Text, oved.OVERDOSE_REASON_NAME + "; ", "");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, bool IsRequired)
        {
            try
            {
                ValidateMaxLength validRule = new ValidateMaxLength();
                validRule.textEdit = control;
                validRule.IsRequired = IsRequired;
                validRule.maxLength = 2000;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationGridLookupControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

        private void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                dxValidationProvider1.SetValidationRule(memReason, null);
                dxValidationProvider1.SetValidationRule(cboReason, null);
                string reasonStr = null;
                if ((cboReason.EditValue == null || cboReason.EditValue == "") && string.IsNullOrEmpty(memReason.Text.Trim()))
                {
                    ValidationGridLookupControl(cboReason, dxValidationProvider1);
                    if (!dxValidationProvider1.Validate())
                        return;
                    cboReason.Focus();
                    cboReason.ShowPopup();
                    return;
                }
                else if (cboReason.EditValue != null && cboReason.EditValue != "")
                {
                    var data = OverDoseReason.FirstOrDefault(o => o.ID == Int64.Parse(cboReason.EditValue.ToString()));
                    if (data != null)
                    {
                        if (string.IsNullOrEmpty(memReason.Text.Trim()))
                        {
                            reasonStr = string.Format("{0}", data.OVERDOSE_REASON_NAME);
                            reason(new TreatmentOverReason() { overReason = data.OVERDOSE_REASON_NAME, overReasonId = data.ID, treatmentId = this.treatmentId });
                            goto End;
                        }
                        else
                        {
                            reasonStr = string.Format("{0}; {1}", data.OVERDOSE_REASON_NAME, memReason.Text.Trim());
                            if (Encoding.UTF8.GetByteCount(reasonStr.Trim()) > 2000)
                            {
                                ValidationSingleControl(memReason, dxValidationProvider1, false);
                                var oldText = memReason.Text.Trim();
                                memReason.Text = reasonStr;
                                if (!dxValidationProvider1.Validate())
                                {
                                    memReason.Text = oldText;
                                    return;
                                }
                            }
                            else
                            {
                                reason(new TreatmentOverReason() { overReason = reasonStr, overReasonId = data.ID, treatmentId = this.treatmentId });
                                goto End;
                            }
                        }
                    }
                }
                else
                {
                    if (!IsAssignPres)
                        ValidationSingleControl(memReason, dxValidationProvider1, false);
                    else
                        ValidationSingleControl(memReason, dxValidationProvider1, true);
                    if (!dxValidationProvider1.Validate())
                        return;
                }
                dxValidationProvider1.SetValidationRule(memReason, null);
                dxValidationProvider1.SetValidationRule(cboReason, null);
                reason(new TreatmentOverReason() { overReason = memReason.Text.Trim(), treatmentId = this.treatmentId });
            End:
                IsYes(true);
                if (ActionSendAlert != null)
                    ActionSendAlert(GetAlert(true, reasonStr));
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string ReplaceFirstOccurrence(string s, string oldValue, string newValue)
        {
            int i = s.IndexOf(oldValue);
            return s.Remove(i, oldValue.Length).Insert(i, newValue);
        }
        private void btnNo_Click(object sender, EventArgs e)
        {
            try
            {
                if ((this.IsAssignPres && !string.IsNullOrEmpty(Data.OVER_KIDNEY_REASON)) || (!this.IsAssignPres && !string.IsNullOrEmpty(Data.OVER_RESULT_TEST_REASON)))
                    reason(new TreatmentOverReason() { overReason = this.IsAssignPres ? Data.OVER_KIDNEY_REASON : Data.OVER_RESULT_TEST_REASON, overReasonId = this.Data.OVER_REASON_ID ?? 0, treatmentId = this.treatmentId });
                else
                    reason(new TreatmentOverReason());
                IsYes(false);
                string dataString = memReason.Text.Trim();
                if (cboReason.EditValue != null && cboReason.EditValue != "")
                {
                    var data = OverDoseReason.FirstOrDefault(o => o.ID == Int64.Parse(cboReason.EditValue.ToString()));
                    if (data != null)
                    {
                        if (string.IsNullOrEmpty(memReason.Text.Trim()))
                        {
                            dataString = data.OVERDOSE_REASON_NAME;
                        }
                        else
                        {
                            dataString = string.Format("{0}; {1}", data.OVERDOSE_REASON_NAME, memReason.Text.Trim());
                        }
                    }
                }
                if (ActionSendAlert != null)
                    ActionSendAlert(GetAlert(false, dataString));
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HisGfrAlertLogSDO GetAlert(bool IsYes, string data = null)
        {

            try
            {
                return new HisGfrAlertLogSDO() { MedicineTypeId = Data.ID, WarningTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtWarningTime), WarningContent = lblTitle.Text, WarningType = IsWarning ? "V" : "C", Overdose = IsWarning ? (decimal?)Math.Round((Data.AMOUNT ?? 0) - (Ms.AMOUNT_INDAY_FROM ?? 0) * (Data.UseDays ?? 1) + AmountInDay, 2) : 0, DoctorDecides = IsWarning ? IsYes ? "Có" : "Không" : null, OverKidneyReason = IsWarning ? (data ?? memReason.Text.Trim()) : null };
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

    }
}
