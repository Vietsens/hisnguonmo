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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    /// <summary>
    /// Bat buoc nhap day du thong tin kham benh ngoai tru theo QD130.
    /// Chi kich hoat khi key HIS.DESKTOP.EXAM.REQUIRED_FIELDS_QD130 = 1
    /// VA ho so co loai dieu tri Kham. Khac di thi thoat som, giu nguyen hanh vi cu.
    ///
    /// Nguyen tac khong canh bao trung: truong nao da co co che canh bao dang BAT
    /// (cau hinh cu, khai bao phong, rang buoc san co) thi kiem tra nay BO QUA,
    /// de co che cu tu canh bao. Chi canh bao nhung truong chua co co che nao.
    /// </summary>
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private bool isRequiredFieldsQd130;

        // Cac co ghi nhan chi so DHST da co co che canh bao dang bat,
        // duoc gan trong UcDHSTValidateControl.
        private bool dhstRequiredAllQd130;
        private bool dhstRequiredWeightQd130;
        private bool dhstRequiredPulseBpQd130;
        private bool dhstRequiredTemperatureQd130;

        private void LoadQd130Config()
        {
            try
            {
                isRequiredFieldsQd130 = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.REQUIRED_FIELDS_QD130) == "1";
            }
            catch (Exception ex)
            {
                isRequiredFieldsQd130 = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsApplyQd130()
        {
            try
            {
                return isRequiredFieldsQd130
                    && this.treatment != null
                    && this.treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// To do nhan cac truong bat buoc theo QD130.
        /// To do ca 9 nhom, ke ca truong do co che cu dam nhiem canh bao,
        /// vi day la chi dan hien thi cho bac si, khong phai canh bao.
        /// </summary>
        private void ApplyQd130Appearance()
        {
            try
            {
                if (!IsApplyQd130()) return;

                SetQd130CaptionRequired(lblCaptionHospitalizationReason);
                SetQd130CaptionRequired(lblCaptionPathologicalProcess);
                SetQd130CaptionRequired(lblCaptionPathologicalHistory);
                SetQd130CaptionRequired(lciKhamToanThan);
                SetQd130CaptionRequired(lblInformationExam);
                SetQd130CaptionRequired(lciIcdText);
                SetQd130CaptionRequired(lblCaptionConclude);

                // Dau hieu sinh ton co ban - 8 chi so thuoc tab Co ban
                SetQd130CaptionRequired(lciExecuteTime);
                SetQd130CaptionRequired(lciPulse);
                SetQd130CaptionRequired(lciBreathRate);
                SetQd130CaptionRequired(lciBloodPressure);
                SetQd130CaptionRequired(lciTemperature);
                SetQd130CaptionRequired(lciHeight);
                SetQd130CaptionRequired(lciWeight);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetQd130CaptionRequired(DevExpress.XtraLayout.LayoutControlItem item)
        {
            try
            {
                if (item == null) return;
                item.AppearanceItemCaption.ForeColor = Color.Maroon;
                item.AppearanceItemCaption.Options.UseForeColor = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Cac truong da co co che canh bao dang bat - kiem tra QD130 bo qua

        /// <summary>
        /// Ly do kham: da co cau hinh HospitalizationReasonRequired hoac cau hinh theo ma doi tuong.
        /// </summary>
        private bool IsQd130SkipHospitalizationReason()
        {
            try
            {
                long required = Inventec.Common.TypeConvert.Parse.ToInt64(
                    HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.HOSPITALIZATION_REASON__REQUIRED));
                if (required == 1) return true;

                if (String.IsNullOrEmpty(HisConfigCFG.HospitalizationReasonRequiredByPatientCode)) return false;
                if (this.treatment == null) return false;

                HIS_PATIENT_TYPE patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                    .FirstOrDefault(o => o.ID == this.treatment.TDL_PATIENT_TYPE_ID);
                if (patientType == null) return false;

                return HisConfigCFG.HospitalizationReasonRequiredByPatientCode.Split(',')
                    .Select(o => (o ?? "").Trim())
                    .Contains(patientType.PATIENT_TYPE_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Chieu cao: da co cau hinh RequiredWeightHeight_Option (tru lua chon 3 - chi can nang).
        /// </summary>
        private bool IsQd130SkipHeight()
        {
            if (dhstRequiredAllQd130) return true;
            string opt = HisConfigCFG.RequiredWeightHeight_Option;
            return opt == "1" || (opt == "2" && chkHospitalize != null && chkHospitalize.Checked);
        }

        /// <summary>
        /// Can nang: ngoai cau hinh RequiredWeightHeight_Option con co cau hinh theo nhom tuoi va doi tuong.
        /// </summary>
        private bool IsQd130SkipWeight()
        {
            if (dhstRequiredAllQd130 || dhstRequiredWeightQd130) return true;
            string opt = HisConfigCFG.RequiredWeightHeight_Option;
            return opt == "1" || opt == "3" || (opt == "2" && chkHospitalize != null && chkHospitalize.Checked);
        }

        #endregion

        /// <summary>
        /// Kiem tra cac truong CHUA co co che canh bao nao. Thieu thi gom vao MOT thong bao,
        /// dua con tro ve o thieu dau tien va dung thao tac.
        /// </summary>
        private bool CheckQd130RequiredFields()
        {
            try
            {
                if (!IsApplyQd130()) return true;

                List<string> missing = new List<string>();
                Action focusFirst = null;

                // 1. Ly do kham - bo qua neu cau hinh cu dang bat
                if (!IsQd130SkipHospitalizationReason())
                {
                    AddQd130Missing(missing, ref focusFirst, "Lý do khám",
                        IsQd130Empty(txtHospitalizationReason), () => FocusQd130Edit(txtHospitalizationReason));
                }

                // 2. Qua trinh benh ly - bo qua neu cau hinh chung cua man dang bat
                if (this.requiredControl != 1)
                {
                    AddQd130Missing(missing, ref focusFirst, "Quá trình bệnh lý",
                        IsQd130Empty(txtPathologicalProcess), () => FocusQd130Edit(txtPathologicalProcess));
                }

                // 3, 4, 5. Chua co co che nao - luon kiem tra
                AddQd130Missing(missing, ref focusFirst, "Tiền sử bệnh",
                    IsQd130Empty(txtPathologicalHistory), () => FocusQd130Edit(txtPathologicalHistory));

                AddQd130Missing(missing, ref focusFirst, "Khám toàn thân",
                    IsQd130Empty(txtKhamToanThan), () => FocusQd130Edit(txtKhamToanThan));

                AddQd130Missing(missing, ref focusFirst, "Khám bộ phận",
                    IsQd130Empty(txtKhamBoPhan), () => FocusQd130Edit(txtKhamBoPhan));

                // 6. Chan doan chinh - bo qua neu phong khai bat buoc nhap chan doan
                if (this.isAllowNoIcd)
                {
                    AddQd130Missing(missing, ref focusFirst, "Chẩn đoán chính",
                        IsQd130Empty(txtIcdCode), () => FocusQd130Edit(txtIcdCode));
                }

                // 7. Dau hieu sinh ton co ban
                if (!dhstRequiredAllQd130)
                {
                    AddQd130Missing(missing, ref focusFirst, "Thời gian đo dấu hiệu sinh tồn",
                        IsQd130Empty(dtExecuteTime), () => FocusQd130Dhst(dtExecuteTime));

                    AddQd130Missing(missing, ref focusFirst, "Nhịp thở",
                        IsQd130Empty(spinBreathRate), () => FocusQd130Dhst(spinBreathRate));
                }

                if (!dhstRequiredAllQd130 && !dhstRequiredPulseBpQd130)
                {
                    AddQd130Missing(missing, ref focusFirst, "Mạch",
                        IsQd130Empty(spinPulse), () => FocusQd130Dhst(spinPulse));

                    AddQd130Missing(missing, ref focusFirst, "Huyết áp tối đa",
                        IsQd130Empty(spinBloodPressureMax), () => FocusQd130Dhst(spinBloodPressureMax));

                    AddQd130Missing(missing, ref focusFirst, "Huyết áp tối thiểu",
                        IsQd130Empty(spinBloodPressureMin), () => FocusQd130Dhst(spinBloodPressureMin));
                }

                if (!dhstRequiredAllQd130 && !dhstRequiredTemperatureQd130)
                {
                    AddQd130Missing(missing, ref focusFirst, "Nhiệt độ",
                        IsQd130Empty(spinTemperature), () => FocusQd130Dhst(spinTemperature));
                }

                if (!IsQd130SkipHeight())
                {
                    AddQd130Missing(missing, ref focusFirst, "Chiều cao",
                        IsQd130Empty(spinHeight), () => FocusQd130Dhst(spinHeight));
                }

                if (!IsQd130SkipWeight())
                {
                    AddQd130Missing(missing, ref focusFirst, "Cân nặng",
                        IsQd130Empty(spinWeight), () => FocusQd130Dhst(spinWeight));
                }

                // 8. Phuong phap dieu tri - cau hinh cu chi ap luc ket thuc dieu tri
                // va khong ap cho loai dieu tri Kham, nen luon kiem tra o day.
                AddQd130Missing(missing, ref focusFirst, "Phương pháp điều trị",
                    IsQd130Empty(txtTreatmentInstruction), () => FocusQd130Edit(txtTreatmentInstruction));

                // 9. Tinh trang ket thuc dieu tri (Loai ra vien, Ket qua) da bat buoc san
                // trong phan Ket thuc dieu tri - khong kiem tra lai o day.

                if (missing.Count == 0) return true;

                XtraMessageBox.Show(
                    "Hồ sơ chưa đầy đủ thông tin bắt buộc: " + String.Join(", ", missing)
                        + ". Vui lòng bổ sung đầy đủ thông tin trước khi tiếp tục.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                if (focusFirst != null) focusFirst();
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true;
            }
        }

        private void AddQd130Missing(List<string> missing, ref Action focusFirst, string fieldName, bool isEmpty, Action focus)
        {
            if (!isEmpty) return;
            missing.Add(fieldName);
            if (focusFirst == null) focusFirst = focus;
        }

        private bool IsQd130Empty(BaseEdit control)
        {
            try
            {
                if (control == null) return false;
                if (control is MemoEdit || control is TextEdit)
                {
                    return String.IsNullOrWhiteSpace(control.Text);
                }
                return control.EditValue == null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        private void FocusQd130Edit(BaseEdit control)
        {
            try
            {
                if (control == null) return;
                control.Focus();
                control.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// O dau hieu sinh ton nam trong tab, phai bat sang tab Co ban thi bac si moi nhin thay.
        /// </summary>
        private void FocusQd130Dhst(BaseEdit control)
        {
            try
            {
                if (tabDHST != null && lcgDhstCoBan != null)
                {
                    tabDHST.SelectedTabPage = lcgDhstCoBan;
                }
                FocusQd130Edit(control);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
