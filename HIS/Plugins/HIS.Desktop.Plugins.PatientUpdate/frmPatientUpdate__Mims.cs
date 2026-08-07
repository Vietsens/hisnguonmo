/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 * All rights reserved.
 * Checklist "Phụ nữ mang thai" / "Phụ nữ cho con bú" trên màn Sửa thông tin bệnh nhân
 * phục vụ cảnh báo thuốc MIMS (Drug Pregnancy / Drug Lactation).
 * Controls được tạo RUNTIME (không sửa designer) — chèn vào hàng đầu group "Thông tin bệnh"
 * (vị trí emptySpaceItem5). Dữ liệu dùng chung bảng HIS_MIMS_PATIENT_PROFILE với màn Xử trí khám.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    public partial class frmPatientUpdate : HIS.Desktop.Utility.FormBase
    {
        #region Declare — Phan loai phu nu (MIMS)
        PanelControl pnlMimsWomanClassify;
        CheckEdit chkMimsPregnant;
        SpinEdit spinMimsPregnantMonth;
        LabelControl lblMimsPregnantMonthUnit;
        CheckEdit chkMimsLactating;
        SpinEdit spinMimsLactatingMonth;
        LabelControl lblMimsLactatingMonthUnit;
        DevExpress.XtraLayout.LayoutControlItem lciMimsWomanClassify;

        /// <summary>
        /// Bản ghi trạng thái hiện tại của bệnh nhân trong HIS_MIMS_PATIENT_PROFILE (null = chưa có).
        /// </summary>
        MimsPatientProfileRecord mimsPatientProfileRecord;

        /// <summary>
        /// Chặn event khi đang fill dữ liệu từ DB lên control.
        /// </summary>
        bool isMimsWomanClassifyFilling = false;
        #endregion

        /// <summary>
        /// Tạo nhóm control Phân loại phụ nữ trong group "Thông tin bệnh" (hàng checkbox bệnh).
        /// Chỉ tạo khi config HIS.Desktop.Mims.IsCheckPregnancyLactation = 1.
        /// Gọi sau FillDataPatientToControl trong frmPatientUpdate_Load.
        /// </summary>
        private void InitMimsWomanClassify()
        {
            try
            {
                if (!Config.IsCheckMimsPregnancyLactation)
                    return;
                if (this.pnlMimsWomanClassify != null)
                    return;

                this.pnlMimsWomanClassify = new PanelControl();
                this.pnlMimsWomanClassify.Name = "pnlMimsWomanClassify";
                this.pnlMimsWomanClassify.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

                this.chkMimsPregnant = new CheckEdit();
                this.chkMimsPregnant.Name = "chkMimsPregnant";
                this.chkMimsPregnant.Properties.Caption = "PN mang thai";
                this.chkMimsPregnant.ToolTip = "Phụ nữ mang thai — dùng cho cảnh báo thuốc MIMS (Drug Pregnancy)";
                this.chkMimsPregnant.Bounds = new System.Drawing.Rectangle(0, 1, 105, 20);
                this.chkMimsPregnant.CheckedChanged += new EventHandler(chkMimsPregnant_CheckedChanged);

                this.spinMimsPregnantMonth = new SpinEdit();
                this.spinMimsPregnantMonth.Name = "spinMimsPregnantMonth";
                this.spinMimsPregnantMonth.Bounds = new System.Drawing.Rectangle(107, 1, 48, 20);
                this.spinMimsPregnantMonth.Properties.IsFloatValue = false;
                this.spinMimsPregnantMonth.Properties.MinValue = 0;
                this.spinMimsPregnantMonth.Properties.MaxValue = 9;
                this.spinMimsPregnantMonth.ToolTip = "Số tháng mang thai (1-9)";
                this.spinMimsPregnantMonth.Enabled = false;

                this.lblMimsPregnantMonthUnit = new LabelControl();
                this.lblMimsPregnantMonthUnit.Text = "(tháng)";
                this.lblMimsPregnantMonthUnit.Location = new System.Drawing.Point(158, 4);

                this.chkMimsLactating = new CheckEdit();
                this.chkMimsLactating.Name = "chkMimsLactating";
                this.chkMimsLactating.Properties.Caption = "PN cho con bú";
                this.chkMimsLactating.ToolTip = "Phụ nữ cho con bú — dùng cho cảnh báo thuốc MIMS (Drug Lactation)";
                this.chkMimsLactating.Bounds = new System.Drawing.Rectangle(215, 1, 110, 20);
                this.chkMimsLactating.CheckedChanged += new EventHandler(chkMimsLactating_CheckedChanged);

                this.spinMimsLactatingMonth = new SpinEdit();
                this.spinMimsLactatingMonth.Name = "spinMimsLactatingMonth";
                this.spinMimsLactatingMonth.Bounds = new System.Drawing.Rectangle(327, 1, 48, 20);
                this.spinMimsLactatingMonth.Properties.IsFloatValue = false;
                this.spinMimsLactatingMonth.Properties.MinValue = 0;
                this.spinMimsLactatingMonth.Properties.MaxValue = 48;
                this.spinMimsLactatingMonth.ToolTip = "Số tháng cho con bú";
                this.spinMimsLactatingMonth.Enabled = false;

                this.lblMimsLactatingMonthUnit = new LabelControl();
                this.lblMimsLactatingMonthUnit.Text = "(tháng)";
                this.lblMimsLactatingMonthUnit.Location = new System.Drawing.Point(378, 4);

                this.pnlMimsWomanClassify.Controls.Add(this.chkMimsPregnant);
                this.pnlMimsWomanClassify.Controls.Add(this.spinMimsPregnantMonth);
                this.pnlMimsWomanClassify.Controls.Add(this.lblMimsPregnantMonthUnit);
                this.pnlMimsWomanClassify.Controls.Add(this.chkMimsLactating);
                this.pnlMimsWomanClassify.Controls.Add(this.spinMimsLactatingMonth);
                this.pnlMimsWomanClassify.Controls.Add(this.lblMimsLactatingMonthUnit);

                this.layoutControl6.BeginUpdate();
                try
                {
                    this.layoutControl6.Controls.Add(this.pnlMimsWomanClassify);
                    this.lciMimsWomanClassify = new DevExpress.XtraLayout.LayoutControlItem();
                    this.lciMimsWomanClassify.Name = "lciMimsWomanClassify";
                    this.layoutControlGroup6.AddItem(this.lciMimsWomanClassify);
                    this.lciMimsWomanClassify.Control = this.pnlMimsWomanClassify;
                    this.lciMimsWomanClassify.TextVisible = false;
                    this.lciMimsWomanClassify.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                    this.lciMimsWomanClassify.MinSize = new System.Drawing.Size(440, 24);
                    this.lciMimsWomanClassify.MaxSize = new System.Drawing.Size(561, 24);
                    this.lciMimsWomanClassify.Move(this.emptySpaceItem5, DevExpress.XtraLayout.Utils.InsertType.Left);
                }
                finally
                {
                    this.layoutControl6.EndUpdate();
                }

                // Đổi giới tính trên form -> cập nhật enable/disable checklist
                this.cboGender1.EditValueChanged += new EventHandler(cboGender1_EditValueChanged_MimsWomanClassify);
                UpdateMimsWomanClassifyEnabled();
                LoadMimsWomanClassifyData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Giới tính đang chọn trên form là Nữ?
        /// </summary>
        private bool IsMimsFemaleSelected()
        {
            try
            {
                long genderId = Inventec.Common.TypeConvert.Parse.ToInt64((cboGender1.EditValue ?? "0").ToString());
                return genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        private void cboGender1_EditValueChanged_MimsWomanClassify(object sender, EventArgs e)
        {
            try
            {
                UpdateMimsWomanClassifyEnabled();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nữ -> enable checklist; khác Nữ -> bỏ tick + disable toàn bộ.
        /// </summary>
        private void UpdateMimsWomanClassifyEnabled()
        {
            try
            {
                if (this.pnlMimsWomanClassify == null) return;
                bool isFemale = IsMimsFemaleSelected();
                if (!isFemale)
                {
                    this.isMimsWomanClassifyFilling = true;
                    this.chkMimsPregnant.Checked = false;
                    this.spinMimsPregnantMonth.Value = 0;
                    this.chkMimsLactating.Checked = false;
                    this.spinMimsLactatingMonth.Value = 0;
                    this.isMimsWomanClassifyFilling = false;
                }
                // Loại trừ nhau: bên kia đang tick thì disable bên này (checkbox đang tick luôn thao tác được để bỏ tick)
                this.chkMimsPregnant.Enabled = isFemale && (this.chkMimsPregnant.Checked || !this.chkMimsLactating.Checked);
                this.spinMimsPregnantMonth.Enabled = isFemale && this.chkMimsPregnant.Checked;
                this.chkMimsLactating.Enabled = isFemale && (this.chkMimsLactating.Checked || !this.chkMimsPregnant.Checked);
                this.spinMimsLactatingMonth.Enabled = isFemale && this.chkMimsLactating.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMimsPregnant_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isMimsWomanClassifyFilling) return;
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
                    this.chkMimsLactating.Enabled = IsMimsFemaleSelected();
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
                if (this.isMimsWomanClassifyFilling) return;
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
                    this.chkMimsPregnant.Enabled = IsMimsFemaleSelected();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nạp bất đồng bộ bản ghi HIS_MIMS_PATIENT_PROFILE của bệnh nhân rồi fill lên control.
        /// </summary>
        private void LoadMimsWomanClassifyData()
        {
            try
            {
                if (this.pnlMimsWomanClassify == null || this.PatientId <= 0) return;
                long patientId = this.PatientId;
                Task.Run(() =>
                {
                    try
                    {
                        var record = MimsPatientProfileWorker.GetByPatientId(patientId);
                        this.mimsPatientProfileRecord = record;
                        if (this.IsDisposed || !this.IsHandleCreated) return;
                        this.BeginInvoke((MethodInvoker)delegate { FillMimsWomanClassifyToControls(record); });
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

        private void FillMimsWomanClassifyToControls(MimsPatientProfileRecord record)
        {
            try
            {
                if (this.pnlMimsWomanClassify == null) return;
                if (!IsMimsFemaleSelected()) return;
                this.isMimsWomanClassifyFilling = true;
                bool isPregnant = record != null && record.IS_PREGNANT == 1;
                bool isLactating = record != null && record.IS_LACTATING == 1;
                this.chkMimsPregnant.Checked = isPregnant;
                this.spinMimsPregnantMonth.Value = (record != null && record.PREGNANT_MONTH != null) ? record.PREGNANT_MONTH.Value : 0;
                this.spinMimsPregnantMonth.Enabled = isPregnant;
                this.chkMimsLactating.Checked = isLactating;
                this.spinMimsLactatingMonth.Value = (record != null && record.LACTATING_MONTH != null) ? record.LACTATING_MONTH.Value : 0;
                this.spinMimsLactatingMonth.Enabled = isLactating;
                // Loại trừ nhau: bên kia đang tick thì disable bên này
                this.chkMimsPregnant.Enabled = isPregnant || !isLactating;
                this.chkMimsLactating.Enabled = isLactating || !isPregnant;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isMimsWomanClassifyFilling = false;
            }
        }

        /// <summary>
        /// Validate trước khi lưu: tick "PN mang thai" thì bắt buộc nhập số tháng 1..9.
        /// </summary>
        private bool ValidMimsWomanClassify()
        {
            try
            {
                if (this.pnlMimsWomanClassify == null) return true;
                if (this.chkMimsPregnant.Checked && (this.spinMimsPregnantMonth.Value < 1 || this.spinMimsPregnantMonth.Value > 9))
                {
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
        /// Dữ liệu checklist có thay đổi so với bản ghi đã nạp? Không đổi -> không gọi API.
        /// </summary>
        private bool IsMimsWomanClassifyChanged()
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
        /// Lưu checklist vào HIS_MIMS_PATIENT_PROFILE — gọi SAU khi api/HisPatient/UpdateSdo thành công.
        /// Không đổi dữ liệu -> không gọi API. Chạy nền, lỗi chỉ log (không chặn đóng form).
        /// </summary>
        private void SaveMimsWomanClassify()
        {
            try
            {
                if (this.pnlMimsWomanClassify == null || this.PatientId <= 0) return;
                if (!IsMimsWomanClassifyChanged()) return;

                var record = this.mimsPatientProfileRecord;
                if (record == null)
                    record = new MimsPatientProfileRecord();
                record.PATIENT_ID = this.PatientId;
                if (this.TreatmentId != null && this.TreatmentId > 0)
                    record.TREATMENT_ID = this.TreatmentId;
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
                        if (saved == null)
                            Inventec.Common.Logging.LogSystem.Warn("SaveMimsWomanClassify: luu HIS_MIMS_PATIENT_PROFILE that bai"
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
