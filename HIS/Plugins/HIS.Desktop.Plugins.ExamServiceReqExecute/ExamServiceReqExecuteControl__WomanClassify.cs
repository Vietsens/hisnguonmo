/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 * All rights reserved.
 * Tab "Phân loại phụ nữ" — checklist Phụ nữ mang thai / Phụ nữ cho con bú
 * phục vụ cảnh báo thuốc MIMS (Drug Pregnancy / Drug Lactation).
 * Tab được tạo RUNTIME (không sửa designer), chỉ hiện khi config bật và bệnh nhân nữ.
 * Dữ liệu lưu bảng HIS_MIMS_PATIENT_PROFILE (1 bản ghi active / bệnh nhân, update tại chỗ).
 */
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Utility;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        #region Declare — Phan loai phu nu (MIMS)
        DevExpress.XtraTab.XtraTabPage xtraTabPageWomanClassify;
        DevExpress.XtraEditors.CheckEdit chkMimsPregnant;
        DevExpress.XtraEditors.LabelControl lblMimsPregnantMonth;
        DevExpress.XtraEditors.SpinEdit spinMimsPregnantMonth;
        DevExpress.XtraEditors.LabelControl lblMimsPregnantMonthUnit;
        DevExpress.XtraEditors.CheckEdit chkMimsLactating;
        DevExpress.XtraEditors.LabelControl lblMimsLactatingMonth;
        DevExpress.XtraEditors.SpinEdit spinMimsLactatingMonth;
        DevExpress.XtraEditors.LabelControl lblMimsLactatingMonthUnit;

        /// <summary>
        /// Bản ghi trạng thái hiện tại của bệnh nhân trong HIS_MIMS_PATIENT_PROFILE (null = chưa có).
        /// </summary>
        MimsPatientProfileRecord mimsPatientProfileRecord;

        /// <summary>
        /// Chặn event CheckedChanged khi đang fill dữ liệu từ DB lên control.
        /// </summary>
        bool isWomanClassifyFilling = false;
        #endregion

        /// <summary>
        /// Tạo tab "Phân loại phụ nữ" trên vùng tab phải (xtraTabControlInfo).
        /// Chỉ tạo khi config HIS.Desktop.Mims.IsCheckPregnancyLactation = 1 và bệnh nhân nữ.
        /// Gọi trong ExamServiceReqExecuteControl_Load sau khi this.treatment đã có dữ liệu.
        /// </summary>
        private void InitWomanClassifyTab()
        {
            try
            {
                if (!HisConfigCFG.IsCheckMimsPregnancyLactation)
                    return;
                if (this.treatment == null
                    || this.treatment.TDL_PATIENT_GENDER_ID != IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    return;
                if (this.xtraTabPageWomanClassify != null)
                    return;

                this.xtraTabPageWomanClassify = new DevExpress.XtraTab.XtraTabPage();
                this.xtraTabPageWomanClassify.Name = "xtraTabPageWomanClassify";
                this.xtraTabPageWomanClassify.Text = GetWomanClassifyLangText(
                    "ExamServiceReqExecuteControl.xtraTabPageWomanClassify.Text", "Phân loại phụ nữ");

                this.chkMimsPregnant = new DevExpress.XtraEditors.CheckEdit();
                this.chkMimsPregnant.Name = "chkMimsPregnant";
                this.chkMimsPregnant.Properties.Caption = GetWomanClassifyLangText(
                    "ExamServiceReqExecuteControl.chkMimsPregnant.Text", "Phụ nữ mang thai");
                this.chkMimsPregnant.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                this.chkMimsPregnant.Bounds = new System.Drawing.Rectangle(8, 14, 250, 20);
                this.chkMimsPregnant.CheckedChanged += new EventHandler(chkMimsPregnant_CheckedChanged);

                this.lblMimsPregnantMonth = new DevExpress.XtraEditors.LabelControl();
                this.lblMimsPregnantMonth.Text = GetWomanClassifyLangText(
                    "ExamServiceReqExecuteControl.lblMimsPregnantMonth.Text", "Mang thai bao nhiêu tháng:");
                this.lblMimsPregnantMonth.Location = new System.Drawing.Point(30, 44);

                this.spinMimsPregnantMonth = new DevExpress.XtraEditors.SpinEdit();
                this.spinMimsPregnantMonth.Name = "spinMimsPregnantMonth";
                this.spinMimsPregnantMonth.Bounds = new System.Drawing.Rectangle(180, 41, 62, 20);
                this.spinMimsPregnantMonth.Properties.IsFloatValue = false;
                this.spinMimsPregnantMonth.Properties.MinValue = 0;
                this.spinMimsPregnantMonth.Properties.MaxValue = 9;
                this.spinMimsPregnantMonth.Enabled = false;

                this.lblMimsPregnantMonthUnit = new DevExpress.XtraEditors.LabelControl();
                this.lblMimsPregnantMonthUnit.Text = "(tháng)";
                this.lblMimsPregnantMonthUnit.Location = new System.Drawing.Point(248, 44);

                this.chkMimsLactating = new DevExpress.XtraEditors.CheckEdit();
                this.chkMimsLactating.Name = "chkMimsLactating";
                this.chkMimsLactating.Properties.Caption = GetWomanClassifyLangText(
                    "ExamServiceReqExecuteControl.chkMimsLactating.Text", "Phụ nữ cho con bú");
                this.chkMimsLactating.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                this.chkMimsLactating.Bounds = new System.Drawing.Rectangle(8, 76, 250, 20);
                this.chkMimsLactating.CheckedChanged += new EventHandler(chkMimsLactating_CheckedChanged);

                this.lblMimsLactatingMonth = new DevExpress.XtraEditors.LabelControl();
                this.lblMimsLactatingMonth.Text = GetWomanClassifyLangText(
                    "ExamServiceReqExecuteControl.lblMimsLactatingMonth.Text", "Con bú bao nhiêu tháng:");
                this.lblMimsLactatingMonth.Location = new System.Drawing.Point(30, 106);

                this.spinMimsLactatingMonth = new DevExpress.XtraEditors.SpinEdit();
                this.spinMimsLactatingMonth.Name = "spinMimsLactatingMonth";
                this.spinMimsLactatingMonth.Bounds = new System.Drawing.Rectangle(180, 103, 62, 20);
                this.spinMimsLactatingMonth.Properties.IsFloatValue = false;
                this.spinMimsLactatingMonth.Properties.MinValue = 0;
                this.spinMimsLactatingMonth.Properties.MaxValue = 48;
                this.spinMimsLactatingMonth.Enabled = false;

                this.lblMimsLactatingMonthUnit = new DevExpress.XtraEditors.LabelControl();
                this.lblMimsLactatingMonthUnit.Text = "(tháng)";
                this.lblMimsLactatingMonthUnit.Location = new System.Drawing.Point(248, 106);

                this.xtraTabPageWomanClassify.Controls.Add(this.chkMimsPregnant);
                this.xtraTabPageWomanClassify.Controls.Add(this.lblMimsPregnantMonth);
                this.xtraTabPageWomanClassify.Controls.Add(this.spinMimsPregnantMonth);
                this.xtraTabPageWomanClassify.Controls.Add(this.lblMimsPregnantMonthUnit);
                this.xtraTabPageWomanClassify.Controls.Add(this.chkMimsLactating);
                this.xtraTabPageWomanClassify.Controls.Add(this.lblMimsLactatingMonth);
                this.xtraTabPageWomanClassify.Controls.Add(this.spinMimsLactatingMonth);
                this.xtraTabPageWomanClassify.Controls.Add(this.lblMimsLactatingMonthUnit);

                this.xtraTabControlInfo.TabPages.Add(this.xtraTabPageWomanClassify);

                LoadWomanClassifyData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lấy caption theo ngôn ngữ, fallback tiếng Việt khi thiếu key resource
        /// (tab tạo runtime sau khi SetCaptionByLanguageKey đã chạy).
        /// </summary>
        private string GetWomanClassifyLangText(string key, string defaultText)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(
                    key,
                    Base.ResourceLangManager.LanguageUCExamServiceReqExecute,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                if (!string.IsNullOrEmpty(value) && value != key)
                    return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultText;
        }

        private void chkMimsPregnant_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isWomanClassifyFilling) return;
                this.spinMimsPregnantMonth.Enabled = this.chkMimsPregnant.Checked;
                if (!this.chkMimsPregnant.Checked)
                    this.spinMimsPregnantMonth.Value = 0;
                this.spinMimsPregnantMonth.ErrorText = "";
                // Mang thai / cho con bú loại trừ nhau: tick bên này thì bên kia bỏ tick + disable
                if (this.chkMimsPregnant.Checked)
                {
                    if (this.chkMimsLactating.Checked)
                        this.chkMimsLactating.Checked = false;
                    this.chkMimsLactating.Enabled = false;
                }
                else
                {
                    this.chkMimsLactating.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMimsLactating_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isWomanClassifyFilling) return;
                this.spinMimsLactatingMonth.Enabled = this.chkMimsLactating.Checked;
                if (!this.chkMimsLactating.Checked)
                    this.spinMimsLactatingMonth.Value = 0;
                // Mang thai / cho con bú loại trừ nhau: tick bên này thì bên kia bỏ tick + disable
                if (this.chkMimsLactating.Checked)
                {
                    if (this.chkMimsPregnant.Checked)
                        this.chkMimsPregnant.Checked = false;
                    this.chkMimsPregnant.Enabled = false;
                }
                else
                {
                    this.chkMimsPregnant.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nạp bất đồng bộ bản ghi HIS_MIMS_PATIENT_PROFILE của bệnh nhân rồi fill lên tab
        /// (không chặn UI; bệnh nhân chưa có bản ghi thì tab để trống).
        /// </summary>
        private void LoadWomanClassifyData()
        {
            try
            {
                if (this.xtraTabPageWomanClassify == null || this.treatment == null) return;
                long patientId = this.treatment.PATIENT_ID;
                Task.Run(() =>
                {
                    try
                    {
                        var record = MimsPatientProfileWorker.GetByPatientId(patientId);
                        this.mimsPatientProfileRecord = record;
                        if (this.IsDisposed || !this.IsHandleCreated) return;
                        this.BeginInvoke((MethodInvoker)delegate { FillWomanClassifyToControls(record); });
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillWomanClassifyToControls(MimsPatientProfileRecord record)
        {
            try
            {
                if (this.xtraTabPageWomanClassify == null) return;
                this.isWomanClassifyFilling = true;
                bool isPregnant = record != null && record.IS_PREGNANT == 1;
                bool isLactating = record != null && record.IS_LACTATING == 1;
                this.chkMimsPregnant.Checked = isPregnant;
                this.spinMimsPregnantMonth.Value = (record != null && record.PREGNANT_MONTH != null) ? record.PREGNANT_MONTH.Value : 0;
                this.spinMimsPregnantMonth.Enabled = isPregnant;
                this.chkMimsLactating.Checked = isLactating;
                this.spinMimsLactatingMonth.Value = (record != null && record.LACTATING_MONTH != null) ? record.LACTATING_MONTH.Value : 0;
                this.spinMimsLactatingMonth.Enabled = isLactating;
                // Loại trừ nhau: bên kia đang tick thì disable bên này (checkbox đang tick luôn thao tác được để bỏ tick)
                this.chkMimsPregnant.Enabled = isPregnant || !isLactating;
                this.chkMimsLactating.Enabled = isLactating || !isPregnant;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isWomanClassifyFilling = false;
            }
        }

        /// <summary>
        /// So sánh dữ liệu trên tab với bản ghi đã nạp — không đổi thì không gọi API (tối ưu).
        /// </summary>
        private bool IsWomanClassifyChanged()
        {
            short uiPregnant = (short)(this.chkMimsPregnant.Checked ? 1 : 0);
            short uiLactating = (short)(this.chkMimsLactating.Checked ? 1 : 0);
            short uiPregnantMonth = (short)this.spinMimsPregnantMonth.Value;
            short uiLactatingMonth = (short)this.spinMimsLactatingMonth.Value;

            var record = this.mimsPatientProfileRecord;
            if (record == null)
                return uiPregnant == 1 || uiLactating == 1;

            return (record.IS_PREGNANT ?? 0) != uiPregnant
                || (record.IS_LACTATING ?? 0) != uiLactating
                || (record.PREGNANT_MONTH ?? 0) != uiPregnantMonth
                || (record.LACTATING_MONTH ?? 0) != uiLactatingMonth;
        }

        /// <summary>
        /// Validate tab trước khi lưu ca khám: tick "Phụ nữ mang thai" thì bắt buộc nhập số tháng 1..9.
        /// Trả về false + báo lỗi tại control khi không hợp lệ.
        /// </summary>
        private bool ValidWomanClassify()
        {
            try
            {
                if (this.xtraTabPageWomanClassify == null) return true;
                if (this.chkMimsPregnant.Checked && (this.spinMimsPregnantMonth.Value < 1 || this.spinMimsPregnantMonth.Value > 9))
                {
                    this.xtraTabControlInfo.SelectedTabPage = this.xtraTabPageWomanClassify;
                    this.spinMimsPregnantMonth.ErrorText = "Nhập số tháng mang thai (1-9)";
                    this.spinMimsPregnantMonth.Focus();
                    return false;
                }
                this.spinMimsPregnantMonth.ErrorText = "";
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true;
            }
        }

        /// <summary>
        /// Lưu trạng thái tab vào HIS_MIMS_PATIENT_PROFILE — gọi SAU khi ExamUpdate thành công.
        /// Không đổi dữ liệu → không gọi API. Chạy bất đồng bộ, lỗi chỉ log (không rollback ca khám).
        /// Bảng chỉ giữ 1 bản ghi active / bệnh nhân: có ID → Update tại chỗ, chưa có → Create.
        /// </summary>
        private void SaveWomanClassify()
        {
            try
            {
                if (this.xtraTabPageWomanClassify == null || this.treatment == null) return;
                if (!IsWomanClassifyChanged()) return;
                if (this.chkMimsPregnant.Checked && (this.spinMimsPregnantMonth.Value < 1 || this.spinMimsPregnantMonth.Value > 9))
                {
                    // ExamUpdate đã lưu xong — chỉ cảnh báo phần phân loại chưa được ghi nhận
                    Inventec.Common.Logging.LogSystem.Warn("SaveWomanClassify: thiếu số tháng mang thai, bỏ qua lưu HIS_MIMS_PATIENT_PROFILE");
                    return;
                }

                var record = this.mimsPatientProfileRecord;
                if (record == null)
                    record = new MimsPatientProfileRecord();
                record.PATIENT_ID = this.treatment.PATIENT_ID;
                record.TREATMENT_ID = this.treatmentId;
                record.IS_PREGNANT = (short)(this.chkMimsPregnant.Checked ? 1 : 0);
                record.PREGNANT_MONTH = this.chkMimsPregnant.Checked ? (short?)this.spinMimsPregnantMonth.Value : null;
                record.IS_LACTATING = (short)(this.chkMimsLactating.Checked ? 1 : 0);
                record.LACTATING_MONTH = (this.chkMimsLactating.Checked && this.spinMimsLactatingMonth.Value > 0)
                    ? (short?)this.spinMimsLactatingMonth.Value : null;

                Task.Run(() =>
                {
                    try
                    {
                        var saved = MimsPatientProfileWorker.Save(record);
                        if (saved != null)
                            this.mimsPatientProfileRecord = saved;
                        else
                            Inventec.Common.Logging.LogSystem.Warn("SaveWomanClassify: luu HIS_MIMS_PATIENT_PROFILE that bai"
                                + Inventec.Common.Logging.LogUtil.TraceData(
                                    Inventec.Common.Logging.LogUtil.GetMemberName(() => record), record));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
