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
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>
    /// Giao diện tab "Ksk tâm thần" — Giấy khám sức khỏe tâm thần, Mẫu số 04 (Phụ lục XXIV,
    /// Thông tư 25/2026/TT-BYT). Xem PTTK_TBD_Kham_Suc_Khoe_Tam_Than_Mau_04_TT25.md.
    /// Bố cục 3 cột, đọc trên xuống, trái sang phải đúng thứ tự mẫu:
    ///   trái  : I. Hành chính (phần chưa có ở khối Thông tin bệnh nhân) + II. Tiền sử tâm thần + III mục 1–5
    ///   giữa  : III mục 6–12
    ///   phải  : IV. Cận lâm sàng + V. Kết luận (V.1 Có/Không, "Có" thì chọn mã ICD-10)
    /// Không dùng nhóm có khung: tiêu đề mục là nhãn đậm + đường kẻ; ô nhiều dòng tự giãn lấp đầy chiều cao.
    /// Họ tên, giới tính, ngày sinh, CCCD, nhóm máu, Lý do khám đã có ở khối Thông tin bệnh nhân
    /// phía trên dãy tab nên không lặp lại ở đây.
    /// </summary>
    public partial class UcKskMental : XtraUserControl
    {
        private bool isInited = false;

        public UcKskMental()
        {
            InitializeComponent();
        }

        /// <summary>Combo Đối tượng (chọn nhiều) — form khởi tạo nguồn dữ liệu dùng chung với các tab khác.</summary>
        public GridLookUpEdit CboObject { get { return this.cboObject; } }
        /// <summary>Combo Nguồn chi trả (chọn 1).</summary>
        public GridLookUpEdit CboPaymentSource { get { return this.cboPaymentSource; } }
        /// <summary>Combo Bác sĩ khám (người kết luận).</summary>
        public GridLookUpEdit CboConcluder { get { return this.cboConcluder; } }
        /// <summary>Ô Ngày kết luận.</summary>
        public DateEdit DteConclusionTime { get { return this.dteConclusionTime; } }
        /// <summary>V.1: có mắc bệnh tâm thần/rối loạn tâm thần — 1=Có, 0=Không, null=chưa chọn.</summary>
        public RadioGroup RdoMentalDisorder { get { return this.rdoMentalDisorder; } }
        /// <summary>V.1: mã + tên bệnh ICD-10 (chọn nhiều, ghép dấu ;) — chỉ mở khi chọn "Có".</summary>
        public UcKskHistoryIcd MentalDisorderIcd { get { return this.ucMentalDisorderIcd; } }

        /// <summary>
        /// Gắn hành vi cho các ô. Gọi 1 lần sau khi tab được dựng.
        /// <paramref name="icdData"/>/<paramref name="icdPageSize"/>: danh mục ICD cho popup chọn bệnh ở V.1
        /// (form truyền cùng nguồn với cụm ICD tiền sử của các tab khác).
        /// </summary>
        public void InitUc(List<HIS_ICD> icdData, int icdPageSize)
        {
            if (isInited) return;
            isInited = true;
            try
            {
                this.ucMentalDisorderIcd.InitUc(icdData, icdPageSize);
                this.rdoMentalDisorder.EditValueChanged += (s, e) => ApplyMentalDisorderState();
                this.rdoFamilyHistory.EditValueChanged += (s, e) => ApplyHistoryState(this.rdoFamilyHistory, this.txtFamilyHistoryName);
                this.rdoPersonalHistory.EditValueChanged += (s, e) => ApplyHistoryState(this.rdoPersonalHistory, this.txtPersonalHistoryName);
                RefreshState();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Tiền sử tâm thần (mục II): "Nếu có, đề nghị ghi cụ thể tên bệnh" — chỉ khi chọn "Có" mới được
        /// ghi tên bệnh; chọn "Không" hoặc chưa chọn thì khóa ô và bỏ nội dung đã gõ.
        /// </summary>
        private void ApplyHistoryState(RadioGroup rdo, TextEdit txt)
        {
            try
            {
                bool isYes = rdo.EditValue != null && Convert.ToInt32(rdo.EditValue) == 1;
                if (!isYes) txt.Text = null;
                txt.Enabled = isYes;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Đổ phần hành chính chỉ đọc (lấy từ hồ sơ bệnh nhân/hồ sơ điều trị) — mục I.5, I.6, I.10, I.11.
        /// </summary>
        public void SetAdministrativeInfo(string cccdDate, string cccdPlace, string ethnic, string residence, string phone)
        {
            this.txtCccdDate.Text = cccdDate;
            this.txtCccdPlace.Text = cccdPlace;
            this.txtEthnic.Text = ethnic;
            this.txtResidence.Text = residence;
            this.txtPhone.Text = phone;
        }

        /// <summary>Sau khi form xóa trắng các ô của tab: đưa ô tên bệnh tiền sử về trạng thái khóa.</summary>
        public void RefreshState()
        {
            ApplyHistoryState(this.rdoFamilyHistory, this.txtFamilyHistoryName);
            ApplyHistoryState(this.rdoPersonalHistory, this.txtPersonalHistoryName);
            ApplyMentalDisorderState();
        }

        /// <summary>
        /// V.1 "Có mắc bệnh tâm thần/rối loạn tâm thần không? Nếu có ghi cụ thể mã bệnh và mã ICD10":
        /// chỉ khi chọn "Có" mới được chọn bệnh ICD-10; chọn "Không" hoặc chưa chọn thì khóa và bỏ mã đã chọn.
        /// </summary>
        private void ApplyMentalDisorderState()
        {
            try
            {
                bool isYes = this.rdoMentalDisorder.EditValue != null && Convert.ToInt32(this.rdoMentalDisorder.EditValue) == 1;
                if (!isYes) this.ucMentalDisorderIcd.SetData(null, null);
                this.ucMentalDisorderIcd.SetReadOnly(!isYes);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #region Nạp / lấy dữ liệu (HIS_KSK_MENTAL + HIS_DHST)

        /// <summary>Ô huyết áp tâm thu — form dùng để kiểm tra "nhập 1 ô phải đủ 2 ô".</summary>
        public SpinEdit SpnBloodPressureMax { get { return this.spnBloodPressureMax; } }
        /// <summary>Ô huyết áp tâm trương.</summary>
        public SpinEdit SpnBloodPressureMin { get { return this.spnBloodPressureMin; } }

        /// <summary>
        /// Đổ dữ liệu đã lưu lên các ô (mục II–V của Mẫu 04). <paramref name="data"/> null = hồ sơ mới
        /// (các ô để trống); <paramref name="dhst"/> null = không có chỉ số thể lực.
        /// Đối tượng / Nguồn chi trả do form đổ (dùng chung cơ chế combo với các tab khác).
        /// </summary>
        public void FillData(HIS_KSK_MENTAL data, HIS_DHST dhst)
        {
            try
            {
                // Radio trước, ô phụ thuộc sau: đổi radio sẽ khóa/xóa ô tên bệnh và cụm ICD.
                SetRadio(this.rdoFamilyHistory, data != null ? data.IS_FAMILY_MENTAL_HISTORY : null);
                SetRadio(this.rdoPersonalHistory, data != null ? data.IS_PERSONAL_MENTAL_HISTORY : null);
                SetRadio(this.rdoMentalDisorder, data != null ? data.IS_MENTAL_DISORDER : null);
                RefreshState();
                if (data != null)
                {
                    this.txtFamilyHistoryName.Text = data.FAMILY_MENTAL_HISTORY;
                    this.txtPersonalHistoryName.Text = data.PERSONAL_MENTAL_HISTORY;
                    this.memGeneralManifestation.Text = data.GENERAL_MANIFESTATION;
                    this.memConsciousness.Text = data.CONSCIOUSNESS;
                    this.txtOrientationSpace.Text = data.ORIENTATION_SPACE;
                    this.txtOrientationTime.Text = data.ORIENTATION_TIME;
                    this.txtOrientationSurrounding.Text = data.ORIENTATION_SURROUNDING;
                    this.txtOrientationSelf.Text = data.ORIENTATION_SELF;
                    this.memEmotion.Text = data.EMOTION;
                    this.memSensationPerception.Text = data.SENSATION_PERCEPTION;
                    this.memThoughtForm.Text = data.THOUGHT_FORM;
                    this.memThoughtContent.Text = data.THOUGHT_CONTENT;
                    this.memVolitionalActivity.Text = data.VOLITIONAL_ACTIVITY;
                    this.memInstinctiveActivity.Text = data.INSTINCTIVE_ACTIVITY;
                    this.memMemory.Text = data.MEMORY;
                    this.memIntelligence.Text = data.INTELLIGENCE;
                    this.memAttention.Text = data.ATTENTION;
                    this.memExamOther.Text = data.EXAM_OTHER;
                    this.memPsychologicalTest.Text = data.PSYCHOLOGICAL_TEST;
                    this.memOtherSubclinical.Text = data.OTHER_SUBCLINICAL;
                    this.ucMentalDisorderIcd.SetData(data.MENTAL_DISORDER_ICD_CODE, data.MENTAL_DISORDER_ICD_NAME);
                    this.memCognitiveBehavior.Text = data.COGNITIVE_BEHAVIOR_CAPACITY;
                }
                this.spnPulse.EditValue = (dhst != null && dhst.PULSE.HasValue) ? (object)(decimal)dhst.PULSE.Value : null;
                this.spnBloodPressureMax.EditValue = (dhst != null && dhst.BLOOD_PRESSURE_MAX.HasValue) ? (object)(decimal)dhst.BLOOD_PRESSURE_MAX.Value : null;
                this.spnBloodPressureMin.EditValue = (dhst != null && dhst.BLOOD_PRESSURE_MIN.HasValue) ? (object)(decimal)dhst.BLOOD_PRESSURE_MIN.Value : null;
                this.spnHeight.EditValue = (dhst != null && dhst.HEIGHT.HasValue) ? (object)dhst.HEIGHT.Value : null;
                this.spnWeight.EditValue = (dhst != null && dhst.WEIGHT.HasValue) ? (object)dhst.WEIGHT.Value : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Ghi giá trị các ô vào entity trước khi lưu (không đụng ID / liên kết / số giấy).</summary>
        public void GetData(HIS_KSK_MENTAL data)
        {
            if (data == null) return;
            try
            {
                data.IS_FAMILY_MENTAL_HISTORY = GetRadio(this.rdoFamilyHistory);
                data.FAMILY_MENTAL_HISTORY = data.IS_FAMILY_MENTAL_HISTORY == 1 ? NullIfEmpty(this.txtFamilyHistoryName.Text) : null;
                data.IS_PERSONAL_MENTAL_HISTORY = GetRadio(this.rdoPersonalHistory);
                data.PERSONAL_MENTAL_HISTORY = data.IS_PERSONAL_MENTAL_HISTORY == 1 ? NullIfEmpty(this.txtPersonalHistoryName.Text) : null;
                data.GENERAL_MANIFESTATION = NullIfEmpty(this.memGeneralManifestation.Text);
                data.CONSCIOUSNESS = NullIfEmpty(this.memConsciousness.Text);
                data.ORIENTATION_SPACE = NullIfEmpty(this.txtOrientationSpace.Text);
                data.ORIENTATION_TIME = NullIfEmpty(this.txtOrientationTime.Text);
                data.ORIENTATION_SURROUNDING = NullIfEmpty(this.txtOrientationSurrounding.Text);
                data.ORIENTATION_SELF = NullIfEmpty(this.txtOrientationSelf.Text);
                data.EMOTION = NullIfEmpty(this.memEmotion.Text);
                data.SENSATION_PERCEPTION = NullIfEmpty(this.memSensationPerception.Text);
                data.THOUGHT_FORM = NullIfEmpty(this.memThoughtForm.Text);
                data.THOUGHT_CONTENT = NullIfEmpty(this.memThoughtContent.Text);
                data.VOLITIONAL_ACTIVITY = NullIfEmpty(this.memVolitionalActivity.Text);
                data.INSTINCTIVE_ACTIVITY = NullIfEmpty(this.memInstinctiveActivity.Text);
                data.MEMORY = NullIfEmpty(this.memMemory.Text);
                data.INTELLIGENCE = NullIfEmpty(this.memIntelligence.Text);
                data.ATTENTION = NullIfEmpty(this.memAttention.Text);
                data.EXAM_OTHER = NullIfEmpty(this.memExamOther.Text);
                data.PSYCHOLOGICAL_TEST = NullIfEmpty(this.memPsychologicalTest.Text);
                data.OTHER_SUBCLINICAL = NullIfEmpty(this.memOtherSubclinical.Text);
                data.IS_MENTAL_DISORDER = GetRadio(this.rdoMentalDisorder);
                bool hasDisorder = data.IS_MENTAL_DISORDER == 1;
                data.MENTAL_DISORDER_ICD_CODE = hasDisorder ? NullIfEmpty(this.ucMentalDisorderIcd.GetCodes()) : null;
                data.MENTAL_DISORDER_ICD_NAME = hasDisorder ? NullIfEmpty(this.ucMentalDisorderIcd.GetNames()) : null;
                data.COGNITIVE_BEHAVIOR_CAPACITY = NullIfEmpty(this.memCognitiveBehavior.Text);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Ghi chỉ số thể lực (mục III.1) vào HIS_DHST — không đụng ID / người đo.</summary>
        public void GetDhst(HIS_DHST dhst)
        {
            if (dhst == null) return;
            try
            {
                dhst.PULSE = HasSpin(this.spnPulse) ? (long?)Convert.ToInt64(this.spnPulse.Value) : null;
                dhst.BLOOD_PRESSURE_MAX = HasSpin(this.spnBloodPressureMax) ? (long?)Convert.ToInt64(this.spnBloodPressureMax.Value) : null;
                dhst.BLOOD_PRESSURE_MIN = HasSpin(this.spnBloodPressureMin) ? (long?)Convert.ToInt64(this.spnBloodPressureMin.Value) : null;
                dhst.HEIGHT = HasSpin(this.spnHeight) ? (decimal?)Math.Round(this.spnHeight.Value, 2) : null;
                dhst.WEIGHT = HasSpin(this.spnWeight) ? (decimal?)Math.Round(this.spnWeight.Value, 2) : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static bool HasSpin(SpinEdit spn)
        {
            return spn.EditValue != null && spn.EditValue != DBNull.Value;
        }

        /// <summary>Radio Không/Có: mục có Value kiểu int (0/1) — gán đúng kiểu int, không gán short/long.</summary>
        private static void SetRadio(RadioGroup rdo, short? value)
        {
            rdo.EditValue = value.HasValue ? (object)(int)value.Value : null;
        }

        private static short? GetRadio(RadioGroup rdo)
        {
            if (rdo.EditValue == null || rdo.EditValue == DBNull.Value) return null;
            return Convert.ToInt16(rdo.EditValue);
        }

        private static string NullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        #endregion
    }
}
