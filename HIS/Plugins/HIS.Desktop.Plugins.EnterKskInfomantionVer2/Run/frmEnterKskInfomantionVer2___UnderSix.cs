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
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Core;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>
    /// Tab "Trẻ em dưới 6 tuổi" (xtraTabPage8) — load/getvalue.
    /// Backend chưa có EFMODEL/SDO/endpoint → dùng local ADO <see cref="KskUnderSixADO"/>, phần lưu để TODO.
    /// </summary>
    public partial class frmEnterKskInfomantionVer2
    {
        private KskUnderSixADO currentKskUnderSix { get; set; }
        private HIS_DHST dhstUnderSix { get; set; }
        // Bản ghi HIS_KSK_UNDER_SIX trả về sau khi LƯU (để IN đúng theo DB).
        private HIS_KSK_UNDER_SIX currentKskUnderSixEf { get; set; }

        // UCSecondaryIcd (chọn mã ICD-10) nhúng vào group "Kết luận theo bệnh (ICD - 10)".
        private SecondaryIcdProcessor subIcdProcessor8;
        private UserControl ucSecondaryIcd8;

        #region ===== Helper đọc/ghi control (radio & checkbox) =====

        /// <summary>Đọc giá trị int từ RadioGroup (null nếu chưa chọn).</summary>
        private long? GetRadioValue(RadioGroup rdo)
        {
            try
            {
                if (rdo != null && rdo.EditValue != null && rdo.EditValue != System.DBNull.Value)
                    return Convert.ToInt64(rdo.EditValue);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return null;
        }

        /// <summary>Set RadioGroup theo giá trị long? (null → bỏ chọn).</summary>
        private void SetRadioValue(RadioGroup rdo, long? value)
        {
            try
            {
                if (rdo == null) return;
                if (value == null) { rdo.EditValue = null; return; }
                // Gán đúng KIỂU value của item (int hoặc long) để RadioGroup chọn đúng — tránh lệch int/long khiến không hiển thị.
                foreach (DevExpress.XtraEditors.Controls.RadioGroupItem it in rdo.Properties.Items)
                {
                    if (it.Value != null && Convert.ToInt64(it.Value) == value.Value)
                    {
                        rdo.EditValue = it.Value;
                        return;
                    }
                }
                rdo.EditValue = (int)value.Value;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>CheckEdit → 1 (tích) / 0 (bỏ).</summary>
        private long? GetCheckValue(CheckEdit chk)
        {
            return (chk != null && chk.Checked) ? (long?)1 : (long?)0;
        }

        /// <summary>Set CheckEdit theo long? (==1 → tích).</summary>
        private void SetCheckValue(CheckEdit chk, long? value)
        {
            if (chk != null) chk.Checked = (value.HasValue && value.Value == 1);
        }

        /// <summary>Đọc giá trị decimal từ SpinEdit (null nếu chưa nhập).</summary>
        private decimal? GetSpinValue(SpinEdit spn)
        {
            try
            {
                if (spn != null && spn.EditValue != null && spn.EditValue != System.DBNull.Value && !string.IsNullOrEmpty(spn.Text))
                    return spn.Value;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return null;
        }

        /// <summary>Set SpinEdit theo decimal? (null → bỏ trống). (Giữ lại cho tương thích, số đo đã chuyển TextEdit.)</summary>
        private void SetSpinValue(SpinEdit spn, decimal? value)
        {
            try { if (spn != null) spn.EditValue = value; }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Trả null nếu chuỗi rỗng/space, ngược lại trả chuỗi đã trim.</summary>
        private string NullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        /// <summary>long? → short? (cột mã số/cờ trong EFMODEL là NUMBER(2,0)=Int16).</summary>
        private short? ToShort(long? v)
        {
            return v.HasValue ? (short?)v.Value : (short?)null;
        }

        /// <summary>
        /// Dựng entity HIS_KSK_UNDER_SIX (mục A–O) từ giá trị form (qua ADO) để IN/LƯU.
        /// Cột số đo = string (VARCHAR2), cột mã/cờ = short? (NUMBER(2,0)).
        /// </summary>
        private HIS_KSK_UNDER_SIX BuildKskUnderSixEf()
        {
            HIS_KSK_UNDER_SIX k = new HIS_KSK_UNDER_SIX();
            try
            {
                KskUnderSixADO a = GetValueUnderSix();
                // Giữ ID + DHST_ID của bản ghi đã tải/đã lưu để UPDATE (không tạo bản trùng)
                if (currentKskUnderSixEf != null)
                {
                    k.ID = currentKskUnderSixEf.ID;
                    if (currentKskUnderSixEf.DHST_ID != null) k.DHST_ID = currentKskUnderSixEf.DHST_ID;
                }
                // A. Liên kết & hành chính
                if (a.SERVICE_REQ_ID.HasValue) k.SERVICE_REQ_ID = a.SERVICE_REQ_ID.Value;
                k.TDL_TREATMENT_ID = a.TDL_TREATMENT_ID;
                k.TDL_PATIENT_ID = a.TDL_PATIENT_ID;
                if (k.DHST_ID == null) k.DHST_ID = a.DHST_ID;
                k.IS_PREMATURE_BIRTH = ToShort(a.IS_PREMATURE_BIRTH);
                k.ETHNIC = a.ETHNIC; k.RESIDENCE = a.RESIDENCE;
                k.ACCOMPANY_PERSON_NAME = a.ACCOMPANY_PERSON_NAME;
                k.ACCOMPANY_RELATIONSHIP = ToShort(a.ACCOMPANY_RELATIONSHIP);
                k.ACCOMPANY_RELATIONSHIP_OTHER = a.ACCOMPANY_RELATIONSHIP_OTHER;
                k.HISTORY_PERSONAL = a.HISTORY_PERSONAL; k.HISTORY_FAMILY = a.HISTORY_FAMILY;
                k.IS_TB_CONTACT = ToShort(a.IS_TB_CONTACT);
                // B. Sinh tồn
                k.TEMPERATURE = a.TEMPERATURE; k.TEMPERATURE_EVAL = ToShort(a.TEMPERATURE_EVAL);
                k.PULSE = a.PULSE; k.PULSE_EVAL = ToShort(a.PULSE_EVAL);
                k.RESPIRATORY_RATE = a.RESPIRATORY_RATE; k.RESPIRATORY_EVAL = ToShort(a.RESPIRATORY_EVAL);
                // C. Dinh dưỡng
                k.BODY_LENGTH = a.BODY_LENGTH; k.BODY_LENGTH_AGE_SD = a.BODY_LENGTH_AGE_SD;
                k.WEIGHT = a.WEIGHT; k.WEIGHT_AGE_SD = a.WEIGHT_AGE_SD;
                k.HEAD_CIRCUMFERENCE = a.HEAD_CIRCUMFERENCE; k.HEAD_CIRC_EVAL = ToShort(a.HEAD_CIRC_EVAL);
                k.ARM_CIRCUMFERENCE = a.ARM_CIRCUMFERENCE;
                k.IS_NUTRITIONAL_EDEMA = ToShort(a.IS_NUTRITIONAL_EDEMA);
                k.IS_ANEMIA_SIGN = ToShort(a.IS_ANEMIA_SIGN);
                k.IS_RICKETS_SIGN = ToShort(a.IS_RICKETS_SIGN);
                k.IS_MALNUTRITION = ToShort(a.IS_MALNUTRITION);
                k.IS_OVERWEIGHT = ToShort(a.IS_OVERWEIGHT);
                // D. Phát triển
                k.MENTAL_DEV_NORMAL = ToShort(a.MENTAL_DEV_NORMAL);
                k.MOTOR_DEV_NORMAL = ToShort(a.MOTOR_DEV_NORMAL);
                k.AUTISM_RISK = ToShort(a.AUTISM_RISK);
                // E. Tiêm chủng
                k.VACCINE_TB = ToShort(a.VACCINE_TB);
                k.VACCINE_HEPB1 = ToShort(a.VACCINE_HEPB1);
                k.VACCINE_FULL_BY_AGE = ToShort(a.VACCINE_FULL_BY_AGE);
                // F. Quan sát chung & Da
                k.CLINICAL_OBSERVATION = a.CLINICAL_OBSERVATION;
                k.SKIN_COLOR = ToShort(a.SKIN_COLOR); k.PALM_EVAL = ToShort(a.PALM_EVAL);
                k.SKIN_NOTE = a.SKIN_NOTE;
                // G. Đầu - cổ
                k.FONTANEL = ToShort(a.FONTANEL); k.HEAD_SHAPE = ToShort(a.HEAD_SHAPE);
                k.NECK_MOTION = ToShort(a.NECK_MOTION); k.HEAD_ABNORMAL_MASS = ToShort(a.HEAD_ABNORMAL_MASS);
                k.HEADNECK_NOTE = a.HEADNECK_NOTE;
                // H. Mắt
                k.EYE_POSITION = ToShort(a.EYE_POSITION); k.EYELID_CONJUNCTIVA = ToShort(a.EYELID_CONJUNCTIVA);
                k.PUPIL = ToShort(a.PUPIL); k.STRABISMUS = ToShort(a.STRABISMUS); k.EYE_NOTE = a.EYE_NOTE;
                // I. Tai
                k.EAR_EARDRUM = ToShort(a.EAR_EARDRUM); k.SOUND_RESPONSE = ToShort(a.SOUND_RESPONSE);
                k.EAR_SWELLING = ToShort(a.EAR_SWELLING); k.EAR_DISCHARGE = ToShort(a.EAR_DISCHARGE);
                k.EAR_NOTE = a.EAR_NOTE;
                // J. Mũi - họng
                k.NOSE_SHAPE = ToShort(a.NOSE_SHAPE); k.RUNNY_NOSE = ToShort(a.RUNNY_NOSE);
                k.STUFFY_NOSE = ToShort(a.STUFFY_NOSE); k.THROAT = ToShort(a.THROAT);
                k.NOSETHROAT_NOTE = a.NOSETHROAT_NOTE;
                // K. Miệng, răng
                k.MOUTH_SHAPE = ToShort(a.MOUTH_SHAPE); k.NEONATAL_TEETH = ToShort(a.NEONATAL_TEETH);
                k.TONGUE_SHAPE = ToShort(a.TONGUE_SHAPE); k.TONGUE_TIE = ToShort(a.TONGUE_TIE);
                k.ORAL_THRUSH = ToShort(a.ORAL_THRUSH); k.SMALL_CHIN = ToShort(a.SMALL_CHIN);
                k.TOOTH_DECAY = ToShort(a.TOOTH_DECAY); k.MOUTHTEETH_NOTE = a.MOUTHTEETH_NOTE;
                // L. Hô hấp
                k.IRREGULAR_BREATH = ToShort(a.IRREGULAR_BREATH); k.CHEST_RETRACTION = ToShort(a.CHEST_RETRACTION);
                k.ABNORMAL_BREATH_SOUND = ToShort(a.ABNORMAL_BREATH_SOUND); k.RESP_FAILURE_SIGN = ToShort(a.RESP_FAILURE_SIGN);
                k.LUNG_AUSCULTATION = ToShort(a.LUNG_AUSCULTATION); k.RESP_NOTE = a.RESP_NOTE;
                // M. Tim mạch
                k.APEX_POSITION = ToShort(a.APEX_POSITION); k.PERIPHERAL_PULSE = ToShort(a.PERIPHERAL_PULSE);
                k.HEART_AUSCULTATION = ToShort(a.HEART_AUSCULTATION); k.CARDIO_NOTE = a.CARDIO_NOTE;
                // N. Bụng và cơ quan sinh dục
                k.ABDOMEN_NAVEL = ToShort(a.ABDOMEN_NAVEL); k.HEPATOSPLENOMEGALY = ToShort(a.HEPATOSPLENOMEGALY);
                k.ABDOMEN_MASS = ToShort(a.ABDOMEN_MASS); k.ANUS = ToShort(a.ANUS);
                k.GENITALIA = ToShort(a.GENITALIA); k.ABDOMEN_NOTE = a.ABDOMEN_NOTE;
                // O. Cơ xương và thần kinh
                k.ASYMMETRIC_MOVEMENT = ToShort(a.ASYMMETRIC_MOVEMENT); k.SUCKING_REFLEX = ToShort(a.SUCKING_REFLEX);
                k.GRASP_REFLEX = ToShort(a.GRASP_REFLEX); k.MORO_REFLEX = ToShort(a.MORO_REFLEX);
                k.MUSCLE_TONE = ToShort(a.MUSCLE_TONE); k.HIP_JOINT = ToShort(a.HIP_JOINT);
                k.MUSCLE_REFLEX = ToShort(a.MUSCLE_REFLEX); k.SPINE_CHECK = ToShort(a.SPINE_CHECK);
                k.LIMBS_JOINTS = ToShort(a.LIMBS_JOINTS); k.GAIT = ToShort(a.GAIT);
                k.RICKETS_SIGN_NEURO = ToShort(a.RICKETS_SIGN_NEURO); k.MUSCULOSKELETAL_NOTE = a.MUSCULOSKELETAL_NOTE;
                // Bác sĩ khám từng mục lâm sàng (LOGINNAME) — combo cboExamDrSkin8..10 theo đúng thứ tự mục.
                k.SKIN_LOGINNAME = GetExamLoginName(this.cboExamDrSkin8);            // 1. Da
                k.HEADNECK_LOGINNAME = GetExamLoginName(this.cboExamDrHeadNeck8);        // 2. Đầu - cổ
                k.EYE_LOGINNAME = GetExamLoginName(this.cboExamDrEye8);             // 2.x Mắt
                k.EAR_LOGINNAME = GetExamLoginName(this.cboExamDrEar8);             // 2.x Tai
                k.NOSETHROAT_LOGINNAME = GetExamLoginName(this.cboExamDrNoseThroat8);      // 2.x Mũi - họng
                k.MOUTHTEETH_LOGINNAME = GetExamLoginName(this.cboExamDrMouthTeeth8);      // 2.x Miệng - răng
                k.RESP_LOGINNAME = GetExamLoginName(this.cboExamDrResp8);            // 3. Hô hấp
                k.CARDIO_LOGINNAME = GetExamLoginName(this.cboExamDrCardio8);          // 4. Tim mạch
                k.ABDOMEN_LOGINNAME = GetExamLoginName(this.cboExamDrAbdomen8);         // 5. Bụng
                k.MUSCULOSKELETAL_LOGINNAME = GetExamLoginName(this.cboExamDrMusc8); // 6. Cơ xương
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return k;
        }

        /// <summary>Lấy LOGINNAME bác sĩ khám từ GridLookUpEdit (EditValue = LOGINNAME); null nếu chưa chọn.</summary>
        private string GetExamLoginName(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                if (cbo != null && cbo.EditValue != null && cbo.EditValue != System.DBNull.Value)
                {
                    string s = cbo.EditValue.ToString();
                    return string.IsNullOrWhiteSpace(s) ? null : s;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return null;
        }

        /// <summary>
        /// Dựng entity HIS_KSK_GENERAL chỉ chứa phần KẾT LUẬN &amp; TƯ VẤN (mục P) của trẻ dưới 6 tuổi.
        /// Theo thiết kế DB: kết luận lưu sang HIS_KSK_GENERAL cùng SERVICE_REQ_ID.
        /// </summary>
        private HIS_KSK_GENERAL BuildKskGeneralConclusionEf()
        {
            HIS_KSK_GENERAL g = new HIS_KSK_GENERAL();
            try
            {
                KskUnderSixADO a = GetValueUnderSix();
                if (a.SERVICE_REQ_ID.HasValue) g.SERVICE_REQ_ID = a.SERVICE_REQ_ID.Value;
                g.HEALTH_CONCLUSION_TYPE = ToShort(a.HEALTH_CONCLUSION_TYPE);
                g.DISEASES = a.DISEASES;
                g.TREATMENT_INSTRUCTION = a.TREATMENT_INSTRUCTION;
                g.HEALTH_EXAM_RANK_ID = a.HEALTH_EXAM_RANK_ID;
                g.CONCLUDER_LOGINNAME = a.CONCLUDER_LOGINNAME;
                g.CONCLUDER_USERNAME = a.CONCLUDER_USERNAME;
                g.CONCLUSION_TIME = a.CONCLUSION_TIME;
                g.CONCLUSION_ICD_TYPE = ToShort(a.CONCLUSION_ICD_TYPE);
                g.CONCLUSION_ICD_CODE = a.CONCLUSION_ICD_CODE;
                g.CONCLUSION_ICD_NAME = a.CONCLUSION_ICD_NAME;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return g;
        }

        /// <summary>Chỉ bật ô "Ghi rõ (quan hệ khác)" khi chọn "Khác" (6).</summary>
        private void UpdateAccompanyRelationshipOtherState()
        {
            try
            {
                bool isOther = (GetRadioValue(this.rdoAccompanyRelationship8) == 6);
                this.txtAccompanyRelationshipOther8.Enabled = isOther;
                if (!isOther) this.txtAccompanyRelationshipOther8.Text = "";
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void rdoAccompanyRelationship8_EditValueChanged(object sender, EventArgs e)
        {
            UpdateAccompanyRelationshipOtherState();
        }

        /// <summary>
        /// Nhúng UC chọn mã ICD-10 (UCSecondaryIcd) vào panel của group "Kết luận theo bệnh (ICD - 10)".
        /// Mẫu giống các chức năng khác (AssignNutrition...): tạo SecondaryIcdProcessor → Run(InitADO) → add vào panel.
        /// Chạy 1 lần.
        /// </summary>
        private void InitUcSecondaryIcd8()
        {
            try
            {
                if (this.pnlSecondaryIcd8 == null) return;
                if (this.ucSecondaryIcd8 != null) return; // đã nhúng
                this.subIcdProcessor8 = new SecondaryIcdProcessor(
                    new CommonParam(),
                    BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList());
                SecondaryIcdInitADO ado = new SecondaryIcdInitADO();
                ado.Width = (this.pnlSecondaryIcd8.Width > 0) ? this.pnlSecondaryIcd8.Width : 381;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ:";
                ado.TextSize = 30;
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                this.ucSecondaryIcd8 = (UserControl)this.subIcdProcessor8.Run(ado);
                if (this.ucSecondaryIcd8 != null)
                {
                    this.pnlSecondaryIcd8.Controls.Add(this.ucSecondaryIcd8);
                    this.ucSecondaryIcd8.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Chỉ cho nhập/ chọn mã ICD khi tích "Chẩn đoán sơ bộ" (1) hoặc "Chẩn đoán xác định" (2).</summary>
        private void UpdateIcdConclusionState()
        {
            try
            {
                long? v = GetRadioValue(this.rdoIcdConclusion8);
                bool needIcd = (v == 2 || v == 3); // 2=Chẩn đoán sơ bộ, 3=Chẩn đoán xác định
                if (this.subIcdProcessor8 != null && this.ucSecondaryIcd8 != null)
                    this.subIcdProcessor8.ReadOnly(this.ucSecondaryIcd8, !needIcd);
                if (this.btnChooseIcd8 != null) this.btnChooseIcd8.Enabled = needIcd;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void rdoIcdConclusion8_EditValueChanged(object sender, EventArgs e)
        {
            UpdateIcdConclusionState();
        }

        /// <summary>
        /// Nút "..." cạnh ô ICD: mở popup chọn bệnh (giống nút cạnh "CĐ phụ" ở ExamServiceReqExecute).
        /// Popup trả mã/ tên ICD đã chọn qua delegate -> ghi ngược vào UC nhúng.
        /// </summary>
        private void btnChooseIcd8_Click(object sender, EventArgs e)
        {
            try
            {
                // Prefill popup bằng giá trị ICD hiện có trong UC nhúng
                string subCode = "", text = "";
                if (this.subIcdProcessor8 != null && this.ucSecondaryIcd8 != null)
                {
                    SecondaryIcdDataADO cur = this.subIcdProcessor8.GetValue(this.ucSecondaryIcd8) as SecondaryIcdDataADO;
                    if (cur != null)
                    {
                        subCode = cur.ICD_SUB_CODE ?? "";
                        text = cur.ICD_TEXT ?? "";
                    }
                }
                int pageSize = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                var icdList = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == 1).ToList();
                HIS.UC.SecondaryIcd.frmSecondaryIcd frm = new HIS.UC.SecondaryIcd.frmSecondaryIcd(
                    DlgChooseIcd8, subCode, text, pageSize, icdList);
                frm.ShowDialog();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Callback từ popup chọn bệnh -> đổ mã/ tên ICD vào UC nhúng.</summary>
        private void DlgChooseIcd8(string icdCodes, string icdNames)
        {
            try
            {
                if (this.subIcdProcessor8 == null || this.ucSecondaryIcd8 == null) return;
                SecondaryIcdDataADO data = new SecondaryIcdDataADO();
                data.ICD_SUB_CODE = icdCodes;
                data.ICD_TEXT = icdNames;
                this.subIcdProcessor8.Reload(this.ucSecondaryIcd8, data);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private bool isUnderSixUiBuilt = false;
        private int curLblW = 150;

        /// <summary>
        /// Dựng giao diện tab theo chuẩn LayoutControl (giống các tab khác):
        /// control -> LayoutControlItem -> LayoutControlGroup -> LayoutControl -> panel.
        /// LayoutControl tự canh hàng nhãn + tự cuộn. Chạy 1 lần lúc load.
        /// </summary>
        private void BuildUnderSixLayout()
        {
            if (isUnderSixUiBuilt) return;
            try
            {
                // Nền sáng (trắng) cho panel + LayoutControl (bỏ màu xám vùng group).
                System.Windows.Forms.Panel[] pnls = new System.Windows.Forms.Panel[] { this.scrUnderSixLeft, this.scrUnderSixMid, this.scrUnderSixRight };
                foreach (var p in pnls) { if (p != null) p.BackColor = System.Drawing.Color.White; }
                DevExpress.XtraLayout.LayoutControl[] lcs = new DevExpress.XtraLayout.LayoutControl[] { this.lcUnderSixLeft, this.lcUnderSixMid, this.lcUnderSixRight };
                foreach (var lc in lcs)
                {
                    if (lc == null) continue;
                    lc.BackColor = System.Drawing.Color.White;
                    lc.Appearance.Control.BackColor = System.Drawing.Color.White;
                    lc.Appearance.Control.Options.UseBackColor = true;
                }
                // Fill trắng cho vùng nội dung group (AppearanceGroup) - bỏ màu xám của skin.
                DevExpress.XtraLayout.LayoutControlGroup[] grps = new DevExpress.XtraLayout.LayoutControlGroup[] {
                    this.lcgRootL8, this.lcgRootM8, this.lcgRootR8,
                    this.lcgHanhChinh8, this.lcgSinhTon8, this.lcgDinhDuong8, this.lcgPhatTrien8, this.lcgTiemChung8,
                    this.lcgKhamLamSang8, this.lcgDa8, this.lcgDauCo8, this.lcgKhamDauCo8, this.lcgMat8, this.lcgTai8,
                    this.lcgMuiHong8, this.lcgMiengRang8, this.lcgHoHap8, this.lcgTimMach8, this.lcgBung8, this.lcgCoXuong8,
                    this.lcgKetLuan8, this.lcgKetLuanSub8, this.lcgKetLuanIcd8 };
                foreach (var gr in grps)
                {
                    if (gr == null) continue;
                    gr.AppearanceGroup.BackColor = System.Drawing.Color.White;
                    gr.AppearanceGroup.Options.UseBackColor = true;
                }
                // Tiêu đề mục = caption của group (hiện đúng I/II/.../VI, 1.Da, 2.x...).
                ApplyUnderSixBehavior(this.scrUnderSixLeft);
                ApplyUnderSixBehavior(this.scrUnderSixMid);
                ApplyUnderSixBehavior(this.scrUnderSixRight);
                isUnderSixUiBuilt = true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Bỏ border RadioGroup + cho bỏ chọn (Delete/chuột phải); xóa default Spin/Combo. Đệ quy.</summary>
        private void ApplyUnderSixBehavior(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                RadioGroup rg = c as RadioGroup;
                if (rg != null)
                {
                    rg.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                    rg.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
                    rg.Properties.Appearance.Options.UseBackColor = true;
                    rg.EditValue = null;
                    rg.KeyDown -= RdoUnderSix_KeyDown; rg.KeyDown += RdoUnderSix_KeyDown;
                    rg.MouseDown -= RdoUnderSix_MouseDown; rg.MouseDown += RdoUnderSix_MouseDown;
                    rg.MouseWheel -= RdoUnderSix_MouseWheel; rg.MouseWheel += RdoUnderSix_MouseWheel;
                    rg.EditValueChanged -= RdoUnderSix_UpdateDeselectToolTip; rg.EditValueChanged += RdoUnderSix_UpdateDeselectToolTip;
                }
                SpinEdit sp = c as SpinEdit;
                if (sp != null) { sp.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True; sp.EditValue = null; }
                GridLookUpEdit gl = c as GridLookUpEdit;
                if (gl != null)
                {
                    gl.Properties.NullText = "";
                    gl.EditValue = null;
                    // Mọi gridlookup trong tab đều có nút X (Delete) để bỏ chọn → EditValue = null.
                    bool hasDelete = false;
                    foreach (DevExpress.XtraEditors.Controls.EditorButton b in gl.Properties.Buttons)
                        if (b.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete) { hasDelete = true; break; }
                    if (!hasDelete)
                        gl.Properties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete));
                    gl.ButtonClick -= GridLookUp_DeleteButtonClick;
                    gl.ButtonClick += GridLookUp_DeleteButtonClick;
                }
                CheckEdit ck = c as CheckEdit;
                if (ck != null)
                {
                    ck.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
                    ck.Properties.Appearance.Options.UseBackColor = true;
                }
                LabelControl lbl = c as LabelControl;
                if (lbl != null)
                {
                    lbl.Appearance.BackColor = System.Drawing.Color.Transparent;
                    lbl.Appearance.Options.UseBackColor = true;
                }
                if (c.Controls.Count > 0) ApplyUnderSixBehavior(c);
            }
        }

        // Click nút X (Delete) trên gridlookup → bỏ chọn (EditValue = null).
        private void GridLookUp_DeleteButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    GridLookUpEdit gl = sender as GridLookUpEdit;
                    if (gl != null) gl.EditValue = null;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        // Chặn lăn chuột làm ĐỔI lựa chọn RadioGroup (tránh nhảy index khi scroll).
        private void RdoUnderSix_MouseWheel(object sender, MouseEventArgs e)
        {
            HandledMouseEventArgs he = e as HandledMouseEventArgs;
            if (he != null) he.Handled = true;
        }

        // Khi radio ĐANG được tích → hiện tooltip nhắc cách bỏ chọn; chưa tích thì xóa tooltip.
        private void RdoUnderSix_UpdateDeselectToolTip(object sender, EventArgs e)
        {
            RadioGroup rg = sender as RadioGroup;
            if (rg == null) return;
            bool ticked = rg.EditValue != null && rg.EditValue != System.DBNull.Value;
            rg.ToolTip = ticked ? "Click chuột phải để bỏ tích chọn" : "";
        }

        // Bỏ chọn RadioGroup khi tích nhầm: nhấn Delete/Backspace.
        private void RdoUnderSix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                RadioGroup rg = sender as RadioGroup;
                if (rg != null) rg.EditValue = null;
            }
        }

        // Bỏ chọn RadioGroup khi tích nhầm: chuột phải, HOẶC chuột trái vào đúng ô đang được tích.
        private void RdoUnderSix_MouseDown(object sender, MouseEventArgs e)
        {
            RadioGroup rg = sender as RadioGroup;
            if (rg == null) return;
            if (e.Button == MouseButtons.Right) { rg.EditValue = null; return; }
            if (e.Button == MouseButtons.Left)
            {
                object prev = rg.EditValue;
                if (prev == null) return; // chưa chọn gì → để chuột trái chọn bình thường
                // Sau khi DevExpress xử lý click: nếu giá trị KHÔNG đổi (click lại ô đang chọn) → bỏ chọn.
                rg.BeginInvoke(new System.Action(delegate ()
                {
                    try { if (object.Equals(rg.EditValue, prev)) rg.EditValue = null; }
                    catch (System.Exception ex) { LogSystem.Warn(ex); }
                }));
            }
        }

        /// <summary>
        /// In đậm tiêu đề mục (LabelControl/Label) ở MỌI tab — text khớp mẫu "I."/"II."/"1."/"2."/"2.1"...
        /// Duyệt đệ quy toàn bộ control của form.
        /// </summary>
        private void BoldAllSectionHeaders(Control parent)
        {
            try
            {
                foreach (Control c in parent.Controls)
                {
                    LabelControl dxl = c as LabelControl;
                    if (dxl != null && IsSectionHeaderText(dxl.Text))
                    {
                        dxl.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                    System.Windows.Forms.Label wl = c as System.Windows.Forms.Label;
                    if (wl != null && IsSectionHeaderText(wl.Text))
                    {
                        wl.Font = new System.Drawing.Font(wl.Font, wl.Font.Style | System.Drawing.FontStyle.Bold);
                    }
                    // Caption của GroupControl
                    GroupControl gc = c as GroupControl;
                    if (gc != null && IsSectionHeaderText(gc.Text))
                    {
                        gc.AppearanceCaption.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                    // Tiêu đề là caption của LayoutControlItem / LayoutControlGroup → duyệt cây item của LayoutControl.
                    DevExpress.XtraLayout.LayoutControl lcx = c as DevExpress.XtraLayout.LayoutControl;
                    if (lcx != null && lcx.Root != null)
                    {
                        BoldLayoutGroupItems(lcx.Root);
                    }
                    if (c.Controls.Count > 0) BoldAllSectionHeaders(c);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>In đậm caption của group + item layout (LayoutControlItem.Text) khớp mẫu tiêu đề. Đệ quy.</summary>
        private void BoldLayoutGroupItems(DevExpress.XtraLayout.LayoutControlGroup g)
        {
            try
            {
                if (g == null) return;
                if (IsSectionHeaderText(g.Text))
                {
                    g.AppearanceGroup.FontStyleDelta = System.Drawing.FontStyle.Bold;
                }
                foreach (DevExpress.XtraLayout.BaseLayoutItem item in g.Items)
                {
                    DevExpress.XtraLayout.LayoutControlGroup sub = item as DevExpress.XtraLayout.LayoutControlGroup;
                    if (sub != null) { BoldLayoutGroupItems(sub); continue; }
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && IsSectionHeaderText(lci.Text))
                    {
                        lci.AppearanceItemCaption.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>True nếu text bắt đầu bằng số La Mã + "." (I. II.) hoặc số/đa cấp + "." (1. 2.1 2.1.) rồi tới chữ.</summary>
        private bool IsSectionHeaderText(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            string t = text.Trim();
            if (t.Length == 0) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(t, @"^(?:[IVXLCDM]{1,6}\.|\d{1,2}\.(?:\d{1,2}\.?)*)\s+\S");
        }

        /// <summary>
        /// Khi chọn tab "Trẻ em dưới 6 tuổi": nếu tuổi tại thời điểm khám &gt;= 72 tháng → cảnh báo.
        /// Trả về true nếu cho tiếp tục (Yes / không đủ tuổi cảnh báo), false nếu cần quay lại tab khác (No).
        /// </summary>
        private bool ConfirmUnderSixAgeAtExam()
        {
            try
            {
                if (currentServiceReq == null) return true;
                long dobNum = currentServiceReq.TDL_PATIENT_DOB;
                if (dobNum <= 0) return true; // không có ngày sinh → không cảnh báo
                long examNum = (currentServiceReq.INTRUCTION_TIME > 0)
                    ? currentServiceReq.INTRUCTION_TIME
                    : System.Convert.ToInt64(System.DateTime.Now.ToString("yyyyMMddHHmmss"));
                int months = AgeInMonthsUnderSix(dobNum, examNum);
                if (months >= 72)
                {
                    System.Windows.Forms.DialogResult r = DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Bệnh nhân đã đủ 6 tuổi (≥ 72 tháng) tại thời điểm khám.\nPhiếu này dùng cho trẻ DƯỚI 6 tuổi.\n\nVẫn tiếp tục nhập tab này?",
                        "Cảnh báo",
                        System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    return r == System.Windows.Forms.DialogResult.Yes;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return true;
        }

        /// <summary>Số tháng tuổi đã tròn (từ ngày sinh tới thời điểm khám). dobNum/examNum dạng yyyyMMdd[HHmmss].</summary>
        private int AgeInMonthsUnderSix(long dobNum, long examNum)
        {
            System.DateTime dob = ParseHisDateNumber(dobNum);
            System.DateTime exam = ParseHisDateNumber(examNum);
            int months = (exam.Year - dob.Year) * 12 + (exam.Month - dob.Month);
            if (exam.Day < dob.Day) months--;
            return months < 0 ? 0 : months;
        }

        private System.DateTime ParseHisDateNumber(long num)
        {
            string s = num.ToString();
            if (s.Length < 8) s = s.PadRight(8, '0'); // vd chỉ có năm "yyyy" → "yyyy0000"
            int y = int.Parse(s.Substring(0, 4));
            int m = int.Parse(s.Substring(4, 2));
            int d = int.Parse(s.Substring(6, 2));
            if (m < 1) m = 1; if (m > 12) m = 12;
            if (d < 1) d = 1;
            int maxd = System.DateTime.DaysInMonth(y, m);
            if (d > maxd) d = maxd;
            return new System.DateTime(y, m, d);
        }

        #endregion

        /// <summary>
        /// Nạp dữ liệu cho tab "Trẻ em dưới 6 tuổi". Gọi từ FillDataToPages().
        /// TODO(backend): khi có api/HisKskUnderSix/Get → load currentKskUnderSix theo SERVICE_REQ_ID
        /// và đổ vào các control (xem mẫu FillDataUnderEighteen ở tab dưới 18 tuổi).
        /// </summary>
        private void FillDataPageUnderSix()
        {
            try
            {
                // Dựng giao diện theo LayoutControl (control->Item->Group->LayoutControl->panel) - chạy 1 lần
                BuildUnderSixLayout();

                // Kết luận theo bệnh (ICD-10): dùng UC chung UcKskConclusionIcd nhúng vào panel8
                // (InitIcdConclusionUcForTabs ở Load tạo dicIcdConclusionUc[7]).

                // Bật/tắt ô "Ghi rõ (quan hệ khác)" theo lựa chọn quan hệ
                this.rdoAccompanyRelationship8.EditValueChanged -= rdoAccompanyRelationship8_EditValueChanged;
                this.rdoAccompanyRelationship8.EditValueChanged += rdoAccompanyRelationship8_EditValueChanged;

                // Combo xếp loại sức khỏe + bác sĩ khám (giống các tab khác)
                SetDataCboRank(this.cboHealthExamRank8);
                SetDataCboExamLoginName(this.cboConcluder8);
                // BS khám từng mục khám lâm sàng (Da/Đầu-cổ/Mắt/Tai...) — load danh sách BS giống cboConcluder8.
                foreach (var cboBs in new DevExpress.XtraEditors.GridLookUpEdit[] {
                    this.cboExamDrSkin8, this.cboExamDrHeadNeck8, this.cboExamDrEye8, this.cboExamDrEar8, this.cboExamDrNoseThroat8,
                    this.cboExamDrMouthTeeth8, this.cboExamDrResp8, this.cboExamDrCardio8, this.cboExamDrAbdomen8, this.cboExamDrMusc8 })
                {
                    if (cboBs != null) SetDataCboExamLoginName(cboBs);
                }
                // Load HIS_KSK_UNDER_SIX (mục A–O) từ backend theo SERVICE_REQ_ID; kết luận (mục P) lấy từ HIS_KSK_GENERAL
                if (currentServiceReq != null)
                {
                    Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                    var filter = new  MOS.Filter.HisKskUnderSixFilter();
                    filter.SERVICE_REQ_ID = currentServiceReq.ID;
                    var data = new Inventec.Common.Adapter.BackendAdapter(param).Get<System.Collections.Generic.List<HIS_KSK_UNDER_SIX>>(
                        "api/HisKskUnderSix/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        currentKskUnderSixEf = data[0];
                        FillUnderSixControlsFromEf(currentKskUnderSixEf, currentKskGeneral);
                    }
                    else
                    {
                        // Chưa có HIS_KSK_UNDER_SIX → vẫn nạp kết luận VII (nếu có) + đổ mặc định I/II/III
                        // từ dữ liệu đã nhập ở màn hình khác (HIS_DHST, HIS_TREATMENT, HIS_BABY).
                        FillUnderSixControlsFromEf(null, currentKskGeneral);
                        FillUnderSixDefaultsFromExisting();
                    }
                }

                // Mặc định bác sĩ kết luận = tài khoản đăng nhập khi chưa có dữ liệu
                if (this.cboConcluder8.EditValue == null && !string.IsNullOrEmpty(this.currentLoginName))
                    this.cboConcluder8.EditValue = this.currentLoginName;

                UpdateAccompanyRelationshipOtherState();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đổ dữ liệu từ DB vào control: mục A–O lấy từ HIS_KSK_UNDER_SIX (k),
        /// kết luận (mục P, trừ ICD) lấy từ HIS_KSK_GENERAL (g). ICD-10 do LoadIcdConclusionToUc đổ vào UC.
        /// </summary>
        private void FillUnderSixControlsFromEf(HIS_KSK_UNDER_SIX k, HIS_KSK_GENERAL g)
        {
            try
            {
                if (k != null)
                {
                    // I. Hành chính
                    SetRadioValue(this.rdoIsPrematureBirth8, k.IS_PREMATURE_BIRTH);
                    this.txtEthnic8.Text = k.ETHNIC;
                    this.txtResidence8.Text = k.RESIDENCE;
                    this.txtAccompanyPersonName8.Text = k.ACCOMPANY_PERSON_NAME;
                    SetRadioValue(this.rdoAccompanyRelationship8, k.ACCOMPANY_RELATIONSHIP);
                    this.txtAccompanyRelationshipOther8.Text = k.ACCOMPANY_RELATIONSHIP_OTHER;
                    this.memHistoryPersonal8.Text = k.HISTORY_PERSONAL;
                    this.memHistoryFamily8.Text = k.HISTORY_FAMILY;
                    SetRadioValue(this.rdoIsTbContact8, k.IS_TB_CONTACT);
                    // II. Sinh tồn (số đo = TextEdit string)
                    this.spnTemperature8.Text = k.TEMPERATURE;
                    SetRadioValue(this.rdoTemperatureEval8, k.TEMPERATURE_EVAL);
                    this.spnPulse8.Text = k.PULSE;
                    SetRadioValue(this.rdoPulseEval8, k.PULSE_EVAL);
                    this.spnRespiratoryRate8.Text = k.RESPIRATORY_RATE;
                    SetRadioValue(this.rdoRespiratoryEval8, k.RESPIRATORY_EVAL);
                    // III. Dinh dưỡng
                    this.spnBodyLength8.Text = k.BODY_LENGTH;
                    this.spnBodyLengthAgeSd8.Text = k.BODY_LENGTH_AGE_SD;
                    this.spnWeight8.Text = k.WEIGHT;
                    this.spnWeightAgeSd8.Text = k.WEIGHT_AGE_SD;
                    this.spnHeadCircumference8.Text = k.HEAD_CIRCUMFERENCE;
                    SetRadioValue(this.rdoHeadCircEval8, k.HEAD_CIRC_EVAL);
                    this.spnArmCircumference8.Text = k.ARM_CIRCUMFERENCE;
                    SetCheckValue(this.chkIsNutritionalEdema8, k.IS_NUTRITIONAL_EDEMA);
                    SetCheckValue(this.chkIsAnemiaSign8, k.IS_ANEMIA_SIGN);
                    SetCheckValue(this.chkIsRicketsSign8, k.IS_RICKETS_SIGN);
                    SetCheckValue(this.chkIsMalnutrition8, k.IS_MALNUTRITION);
                    SetCheckValue(this.chkIsOverweight8, k.IS_OVERWEIGHT);
                    // IV. Phát triển
                    SetRadioValue(this.rdoMentalDevNormal8, k.MENTAL_DEV_NORMAL);
                    SetRadioValue(this.rdoMotorDevNormal8, k.MOTOR_DEV_NORMAL);
                    SetRadioValue(this.rdoAutismRisk8, k.AUTISM_RISK);
                    // V. Tiêm chủng
                    SetRadioValue(this.rdoVaccineTb8, k.VACCINE_TB);
                    SetRadioValue(this.rdoVaccineHepb18, k.VACCINE_HEPB1);
                    SetRadioValue(this.rdoVaccineFullByAge8, k.VACCINE_FULL_BY_AGE);
                    // F. Quan sát chung & Da
                    this.memClinicalObservation8.Text = k.CLINICAL_OBSERVATION;
                    SetRadioValue(this.rdoSkinColor8, k.SKIN_COLOR);
                    SetRadioValue(this.rdoPalmEval8, k.PALM_EVAL);
                    this.memSkinNote8.Text = k.SKIN_NOTE;
                    // G. Đầu - cổ
                    SetRadioValue(this.rdoFontanel8, k.FONTANEL);
                    SetRadioValue(this.rdoHeadShape8, k.HEAD_SHAPE);
                    SetRadioValue(this.rdoNeckMotion8, k.NECK_MOTION);
                    SetRadioValue(this.rdoHeadAbnormalMass8, k.HEAD_ABNORMAL_MASS);
                    this.memHeadNeckNote8.Text = k.HEADNECK_NOTE;
                    // H. Mắt
                    SetRadioValue(this.rdoEyePosition8, k.EYE_POSITION);
                    SetRadioValue(this.rdoEyelidConjunctiva8, k.EYELID_CONJUNCTIVA);
                    SetRadioValue(this.rdoPupil8, k.PUPIL);
                    SetRadioValue(this.rdoStrabismus8, k.STRABISMUS);
                    this.memEyeNote8.Text = k.EYE_NOTE;
                    // I. Tai
                    SetRadioValue(this.rdoEarEardrum8, k.EAR_EARDRUM);
                    SetRadioValue(this.rdoSoundResponse8, k.SOUND_RESPONSE);
                    SetRadioValue(this.rdoEarSwelling8, k.EAR_SWELLING);
                    SetRadioValue(this.rdoEarDischarge8, k.EAR_DISCHARGE);
                    this.memEarNote8.Text = k.EAR_NOTE;
                    // J. Mũi - họng
                    SetRadioValue(this.rdoNoseShape8, k.NOSE_SHAPE);
                    SetRadioValue(this.rdoRunnyNose8, k.RUNNY_NOSE);
                    SetRadioValue(this.rdoStuffyNose8, k.STUFFY_NOSE);
                    SetRadioValue(this.rdoThroat8, k.THROAT);
                    this.memNoseThroatNote8.Text = k.NOSETHROAT_NOTE;
                    // K. Miệng, răng
                    SetRadioValue(this.rdoMouthShape8, k.MOUTH_SHAPE);
                    SetRadioValue(this.rdoNeonatalTeeth8, k.NEONATAL_TEETH);
                    SetRadioValue(this.rdoTongueShape8, k.TONGUE_SHAPE);
                    SetRadioValue(this.rdoTongueTie8, k.TONGUE_TIE);
                    SetRadioValue(this.rdoOralThrush8, k.ORAL_THRUSH);
                    SetRadioValue(this.rdoSmallChin8, k.SMALL_CHIN);
                    SetRadioValue(this.rdoToothDecay8, k.TOOTH_DECAY);
                    this.memMouthTeethNote8.Text = k.MOUTHTEETH_NOTE;
                    // L. Hô hấp
                    SetRadioValue(this.rdoIrregularBreath8, k.IRREGULAR_BREATH);
                    SetRadioValue(this.rdoChestRetraction8, k.CHEST_RETRACTION);
                    SetRadioValue(this.rdoAbnormalBreathSound8, k.ABNORMAL_BREATH_SOUND);
                    SetRadioValue(this.rdoRespFailureSign8, k.RESP_FAILURE_SIGN);
                    SetRadioValue(this.rdoLungAuscultation8, k.LUNG_AUSCULTATION);
                    this.memRespNote8.Text = k.RESP_NOTE;
                    // M. Tim mạch
                    SetRadioValue(this.rdoApexPosition8, k.APEX_POSITION);
                    SetRadioValue(this.rdoPeripheralPulse8, k.PERIPHERAL_PULSE);
                    SetRadioValue(this.rdoHeartAuscultation8, k.HEART_AUSCULTATION);
                    this.memCardioNote8.Text = k.CARDIO_NOTE;
                    // N. Bụng và cơ quan sinh dục
                    SetRadioValue(this.rdoAbdomenNavel8, k.ABDOMEN_NAVEL);
                    SetRadioValue(this.rdoHepatosplenomegaly8, k.HEPATOSPLENOMEGALY);
                    SetRadioValue(this.rdoAbdomenMass8, k.ABDOMEN_MASS);
                    SetRadioValue(this.rdoAnus8, k.ANUS);
                    SetRadioValue(this.rdoGenitalia8, k.GENITALIA);
                    this.memAbdomenNote8.Text = k.ABDOMEN_NOTE;
                    // O. Cơ xương và thần kinh
                    SetRadioValue(this.rdoAsymmetricMovement8, k.ASYMMETRIC_MOVEMENT);
                    SetRadioValue(this.rdoSuckingReflex8, k.SUCKING_REFLEX);
                    SetRadioValue(this.rdoGraspReflex8, k.GRASP_REFLEX);
                    SetRadioValue(this.rdoMoroReflex8, k.MORO_REFLEX);
                    SetRadioValue(this.rdoMuscleTone8, k.MUSCLE_TONE);
                    SetRadioValue(this.rdoHipJoint8, k.HIP_JOINT);
                    SetRadioValue(this.rdoMuscleReflex8, k.MUSCLE_REFLEX);
                    SetRadioValue(this.rdoSpineCheck8, k.SPINE_CHECK);
                    SetRadioValue(this.rdoLimbsJoints8, k.LIMBS_JOINTS);
                    SetRadioValue(this.rdoGait8, k.GAIT);
                    SetRadioValue(this.rdoRicketsSignNeuro8, k.RICKETS_SIGN_NEURO);
                    this.memMusculoskeletalNote8.Text = k.MUSCULOSKELETAL_NOTE;
                    // Bác sĩ khám từng mục (LOGINNAME) → combo (datasource đã set ở FillDataPageUnderSix trước đó).
                    this.cboExamDrSkin8.EditValue = k.SKIN_LOGINNAME;
                    this.cboExamDrHeadNeck8.EditValue = k.HEADNECK_LOGINNAME;
                    this.cboExamDrEye8.EditValue = k.EYE_LOGINNAME;
                    this.cboExamDrEar8.EditValue = k.EAR_LOGINNAME;
                    this.cboExamDrNoseThroat8.EditValue = k.NOSETHROAT_LOGINNAME;
                    this.cboExamDrMouthTeeth8.EditValue = k.MOUTHTEETH_LOGINNAME;
                    this.cboExamDrResp8.EditValue = k.RESP_LOGINNAME;
                    this.cboExamDrCardio8.EditValue = k.CARDIO_LOGINNAME;
                    this.cboExamDrAbdomen8.EditValue = k.ABDOMEN_LOGINNAME;
                    this.cboExamDrMusc8.EditValue = k.MUSCULOSKELETAL_LOGINNAME;
                }
                // VII. Kết luận (trừ ICD) — từ HIS_KSK_GENERAL
                if (g != null)
                {
                    SetRadioValue(this.rdoConclusionHealth8, g.HEALTH_CONCLUSION_TYPE);
                    this.memConclusionDetail8.Text = g.DISEASES;
                    this.memAdviceNextExam8.Text = g.TREATMENT_INSTRUCTION;
                    this.cboHealthExamRank8.EditValue = g.HEALTH_EXAM_RANK_ID;
                    this.cboConcluder8.EditValue = g.CONCLUDER_LOGINNAME;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đổ MẶC ĐỊNH các mục I/II/III khi CHƯA có HIS_KSK_UNDER_SIX, lấy từ dữ liệu đã nhập màn hình khác:
        ///  - II Sinh tồn + III Dinh dưỡng: HIS_DHST (nhiệt độ/mạch/nhịp thở/chiều dài/cân nặng) theo SERVICE_REQ.DHST_ID.
        ///  - I Hành chính: địa chỉ (V_HIS_SERVICE_REQ) + dân tộc/người đưa trẻ (HIS_TREATMENT) + sinh non/vòng đầu lúc sinh (HIS_BABY).
        /// Chỉ điền khi control đang trống (không đè dữ liệu user nhập tay).
        /// </summary>
        private void FillUnderSixDefaultsFromExisting()
        {
            try
            {
                if (currentServiceReq == null) return;
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();

                // ===== II. Sinh tồn + III. Dinh dưỡng: HIS_DHST =====
                if (currentServiceReq.DHST_ID != null && currentServiceReq.DHST_ID > 0)
                {
                    var dhstFilter = new MOS.Filter.HisDhstFilter();
                    dhstFilter.ID = currentServiceReq.DHST_ID;
                    var dataDhst = new Inventec.Common.Adapter.BackendAdapter(param).Get<System.Collections.Generic.List<HIS_DHST>>(
                        "api/HisDhst/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, dhstFilter, param);
                    if (dataDhst != null && dataDhst.Count > 0)
                    {
                        var d = dataDhst[0];
                        if (string.IsNullOrWhiteSpace(this.spnTemperature8.Text)) this.spnTemperature8.Text = NumToStr(d.TEMPERATURE);
                        if (string.IsNullOrWhiteSpace(this.spnPulse8.Text)) this.spnPulse8.Text = NumToStr(d.PULSE);
                        if (string.IsNullOrWhiteSpace(this.spnRespiratoryRate8.Text)) this.spnRespiratoryRate8.Text = NumToStr(d.BREATH_RATE);
                        if (string.IsNullOrWhiteSpace(this.spnBodyLength8.Text)) this.spnBodyLength8.Text = NumToStr(d.HEIGHT);
                        if (string.IsNullOrWhiteSpace(this.spnWeight8.Text)) this.spnWeight8.Text = NumToStr(d.WEIGHT);
                    }
                }

                // ===== I. Hành chính: địa chỉ + tiền sử từ V_HIS_SERVICE_REQ (đã load sẵn, không tốn API) =====
                if (string.IsNullOrWhiteSpace(this.txtResidence8.Text))
                    this.txtResidence8.Text = BuildResidence(currentServiceReq);
                if (string.IsNullOrWhiteSpace(this.memHistoryPersonal8.Text))
                    this.memHistoryPersonal8.Text = currentServiceReq.PATHOLOGICAL_HISTORY;
                if (string.IsNullOrWhiteSpace(this.memHistoryFamily8.Text))
                    this.memHistoryFamily8.Text = currentServiceReq.PATHOLOGICAL_HISTORY_FAMILY;

                // ===== I. Hành chính: dân tộc + người đưa trẻ từ HIS_TREATMENT =====
                if (currentServiceReq.TREATMENT_ID > 0)
                {
                    var treaFilter = new MOS.Filter.HisTreatmentFilter();
                    treaFilter.ID = currentServiceReq.TREATMENT_ID;
                    var dataTrea = new Inventec.Common.Adapter.BackendAdapter(param).Get<System.Collections.Generic.List<HIS_TREATMENT>>(
                        "api/HisTreatment/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, treaFilter, param);
                    if (dataTrea != null && dataTrea.Count > 0)
                    {
                        var t = dataTrea[0];
                        if (string.IsNullOrWhiteSpace(this.txtEthnic8.Text)) this.txtEthnic8.Text = t.TDL_PATIENT_ETHNIC_NAME;
                        if (string.IsNullOrWhiteSpace(this.txtResidence8.Text)) this.txtResidence8.Text = t.TDL_PATIENT_ADDRESS;
                        // Người đưa trẻ + QUAN HỆ: ưu tiên người nhà đã đăng ký (RELATIVE_NAME + RELATIVE_TYPE),
                        // sau đó mẹ (quan hệ=2), rồi bố (quan hệ=1). Set tên + radio quan hệ cùng nhau cho nhất quán.
                        if (string.IsNullOrWhiteSpace(this.txtAccompanyPersonName8.Text))
                        {
                            if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_RELATIVE_NAME))
                            {
                                this.txtAccompanyPersonName8.Text = t.TDL_PATIENT_RELATIVE_NAME;
                                if (GetRadioValue(this.rdoAccompanyRelationship8) == null)
                                    SetRadioValue(this.rdoAccompanyRelationship8, MapAccompanyRelationship(t.TDL_PATIENT_RELATIVE_TYPE));
                            }
                            else if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_MOTHER_NAME))
                            {
                                this.txtAccompanyPersonName8.Text = t.TDL_PATIENT_MOTHER_NAME;
                                if (GetRadioValue(this.rdoAccompanyRelationship8) == null) SetRadioValue(this.rdoAccompanyRelationship8, 2);
                            }
                            else if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_FATHER_NAME))
                            {
                                this.txtAccompanyPersonName8.Text = t.TDL_PATIENT_FATHER_NAME;
                                if (GetRadioValue(this.rdoAccompanyRelationship8) == null) SetRadioValue(this.rdoAccompanyRelationship8, 1);
                            }
                        }
                    }
                }

                // ===== I/III: sinh non + dân tộc dự phòng + vòng đầu lúc sinh từ HIS_BABY =====
                if (currentServiceReq.TREATMENT_ID > 0)
                {
                    var babyFilter = new MOS.Filter.HisBabyFilter();
                    babyFilter.TREATMENT_ID = currentServiceReq.TREATMENT_ID;
                    var dataBaby = new Inventec.Common.Adapter.BackendAdapter(param).Get<System.Collections.Generic.List<V_HIS_BABY>>(
                        "api/HisBaby/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, babyFilter, param);
                    if (dataBaby != null && dataBaby.Count > 0)
                    {
                        var b = dataBaby[0];
                        if (string.IsNullOrWhiteSpace(this.txtEthnic8.Text)) this.txtEthnic8.Text = b.ETHNIC_NAME;
                        // Sinh non = tuổi thai < 37 tuần (1=Có, 0=Không).
                        if (GetRadioValue(this.rdoIsPrematureBirth8) == null && b.WEEK_COUNT != null)
                            SetRadioValue(this.rdoIsPrematureBirth8, (b.WEEK_COUNT.Value < 37) ? (long?)1 : (long?)0);
                        if (string.IsNullOrWhiteSpace(this.txtAccompanyPersonName8.Text) && !string.IsNullOrWhiteSpace(b.FATHER_NAME))
                        {
                            this.txtAccompanyPersonName8.Text = b.FATHER_NAME;
                            if (GetRadioValue(this.rdoAccompanyRelationship8) == null) SetRadioValue(this.rdoAccompanyRelationship8, 1);
                        }
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Suy giá trị radio quan hệ người đưa trẻ (1=Cha,2=Mẹ,3=Ông/bà,4=Anh/chị,5=Họ hàng,6=Khác)
        /// từ text loại quan hệ người nhà (HIS_TREATMENT.TDL_PATIENT_RELATIVE_TYPE). Không chắc → null (để user chọn).
        /// </summary>
        private long? MapAccompanyRelationship(string relativeType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativeType)) return null;
                string s = relativeType.Trim().ToLowerInvariant();
                if (s.Contains("mẹ")) return 2;
                if (s.Contains("cha") || s.Contains("bố")) return 1;
                if (s.Contains("ông") || s.Contains("bà")) return 3;
                if (s.Contains("anh") || s.Contains("chị")) return 4;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return null;
        }

        /// <summary>Ghép địa chỉ đầy đủ: số nhà/thôn + xã + huyện + tỉnh.</summary>
        private string BuildResidence(V_HIS_SERVICE_REQ sr)
        {
            try
            {
                if (sr == null) return null;
                var parts = new System.Collections.Generic.List<string>();
                if (!string.IsNullOrWhiteSpace(sr.TDL_PATIENT_ADDRESS)) parts.Add(sr.TDL_PATIENT_ADDRESS.Trim());
                if (!string.IsNullOrWhiteSpace(sr.TDL_PATIENT_COMMUNE_NAME)) parts.Add(sr.TDL_PATIENT_COMMUNE_NAME.Trim());
                if (!string.IsNullOrWhiteSpace(sr.TDL_PATIENT_DISTRICT_NAME)) parts.Add(sr.TDL_PATIENT_DISTRICT_NAME.Trim());
                if (!string.IsNullOrWhiteSpace(sr.TDL_PATIENT_PROVINCE_NAME)) parts.Add(sr.TDL_PATIENT_PROVINCE_NAME.Trim());
                return parts.Count > 0 ? string.Join(", ", parts) : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private string NumToStr(decimal? v)
        {
            return v.HasValue ? v.Value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture) : null;
        }

        private string NumToStr(long? v)
        {
            return v.HasValue ? v.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Gom giá trị các control của tab thành local ADO.
        /// TODO(backend): khi có EFMODEL HIS_KSK_UNDER_SIX, map ADO này sang EFMODEL trong tầng lưu.
        /// </summary>
        private KskUnderSixADO GetValueUnderSix()
        {
            KskUnderSixADO obj = new KskUnderSixADO();
            try
            {
                if (currentKskUnderSix != null)
                    obj.ID = currentKskUnderSix.ID;
                if (currentServiceReq != null)
                {
                    // TDL_TREATMENT_ID / TDL_PATIENT_ID được backend suy ra từ SERVICE_REQ (giống tab dưới 18 tuổi).
                    obj.SERVICE_REQ_ID = currentServiceReq.ID;
                    obj.TDL_PATIENT_ID = currentServiceReq.TDL_PATIENT_ID;
                }
                // I. Hành chính
                obj.IS_PREMATURE_BIRTH = GetRadioValue(this.rdoIsPrematureBirth8);
                obj.ETHNIC = this.txtEthnic8.Text;
                obj.RESIDENCE = this.txtResidence8.Text;
                obj.ACCOMPANY_PERSON_NAME = this.txtAccompanyPersonName8.Text;
                obj.ACCOMPANY_RELATIONSHIP = GetRadioValue(this.rdoAccompanyRelationship8);
                obj.ACCOMPANY_RELATIONSHIP_OTHER = this.txtAccompanyRelationshipOther8.Text;
                obj.HISTORY_PERSONAL = this.memHistoryPersonal8.Text;
                obj.HISTORY_FAMILY = this.memHistoryFamily8.Text;
                obj.IS_TB_CONTACT = GetRadioValue(this.rdoIsTbContact8);
                // II. Sinh tồn (số đo = TextEdit string)
                obj.TEMPERATURE = NullIfEmpty(this.spnTemperature8.Text);
                obj.TEMPERATURE_EVAL = GetRadioValue(this.rdoTemperatureEval8);
                obj.PULSE = NullIfEmpty(this.spnPulse8.Text);
                obj.PULSE_EVAL = GetRadioValue(this.rdoPulseEval8);
                obj.RESPIRATORY_RATE = NullIfEmpty(this.spnRespiratoryRate8.Text);
                obj.RESPIRATORY_EVAL = GetRadioValue(this.rdoRespiratoryEval8);
                // III. Dinh dưỡng (số đo = TextEdit string)
                obj.BODY_LENGTH = NullIfEmpty(this.spnBodyLength8.Text);
                obj.BODY_LENGTH_AGE_SD = NullIfEmpty(this.spnBodyLengthAgeSd8.Text);
                obj.WEIGHT = NullIfEmpty(this.spnWeight8.Text);
                obj.WEIGHT_AGE_SD = NullIfEmpty(this.spnWeightAgeSd8.Text);
                obj.HEAD_CIRCUMFERENCE = NullIfEmpty(this.spnHeadCircumference8.Text);
                obj.HEAD_CIRC_EVAL = GetRadioValue(this.rdoHeadCircEval8);
                obj.ARM_CIRCUMFERENCE = NullIfEmpty(this.spnArmCircumference8.Text);
                obj.IS_NUTRITIONAL_EDEMA = GetCheckValue(this.chkIsNutritionalEdema8);
                obj.IS_ANEMIA_SIGN = GetCheckValue(this.chkIsAnemiaSign8);
                obj.IS_RICKETS_SIGN = GetCheckValue(this.chkIsRicketsSign8);
                obj.IS_MALNUTRITION = GetCheckValue(this.chkIsMalnutrition8);
                obj.IS_OVERWEIGHT = GetCheckValue(this.chkIsOverweight8);
                // IV. Phát triển tinh thần - vận động
                obj.MENTAL_DEV_NORMAL = GetRadioValue(this.rdoMentalDevNormal8);
                obj.MOTOR_DEV_NORMAL = GetRadioValue(this.rdoMotorDevNormal8);
                obj.AUTISM_RISK = GetRadioValue(this.rdoAutismRisk8);
                // V. Tiêm chủng
                obj.VACCINE_TB = GetRadioValue(this.rdoVaccineTb8);
                obj.VACCINE_HEPB1 = GetRadioValue(this.rdoVaccineHepb18);
                obj.VACCINE_FULL_BY_AGE = GetRadioValue(this.rdoVaccineFullByAge8);
                // VI - F. Da
                obj.CLINICAL_OBSERVATION = this.memClinicalObservation8.Text;
                obj.SKIN_COLOR = GetRadioValue(this.rdoSkinColor8);
                obj.PALM_EVAL = GetRadioValue(this.rdoPalmEval8);
                obj.SKIN_NOTE = this.memSkinNote8.Text;
                // G. Đầu - cổ
                obj.FONTANEL = GetRadioValue(this.rdoFontanel8);
                obj.HEAD_SHAPE = GetRadioValue(this.rdoHeadShape8);
                obj.NECK_MOTION = GetRadioValue(this.rdoNeckMotion8);
                obj.HEAD_ABNORMAL_MASS = GetRadioValue(this.rdoHeadAbnormalMass8);
                obj.HEADNECK_NOTE = this.memHeadNeckNote8.Text;
                // H. Mắt
                obj.EYE_POSITION = GetRadioValue(this.rdoEyePosition8);
                obj.EYELID_CONJUNCTIVA = GetRadioValue(this.rdoEyelidConjunctiva8);
                obj.PUPIL = GetRadioValue(this.rdoPupil8);
                obj.STRABISMUS = GetRadioValue(this.rdoStrabismus8);
                obj.EYE_NOTE = this.memEyeNote8.Text;
                // I. Tai
                obj.EAR_EARDRUM = GetRadioValue(this.rdoEarEardrum8);
                obj.SOUND_RESPONSE = GetRadioValue(this.rdoSoundResponse8);
                obj.EAR_SWELLING = GetRadioValue(this.rdoEarSwelling8);
                obj.EAR_DISCHARGE = GetRadioValue(this.rdoEarDischarge8);
                obj.EAR_NOTE = this.memEarNote8.Text;
                // J. Mũi - họng
                obj.NOSE_SHAPE = GetRadioValue(this.rdoNoseShape8);
                obj.RUNNY_NOSE = GetRadioValue(this.rdoRunnyNose8);
                obj.STUFFY_NOSE = GetRadioValue(this.rdoStuffyNose8);
                obj.THROAT = GetRadioValue(this.rdoThroat8);
                obj.NOSETHROAT_NOTE = this.memNoseThroatNote8.Text;
                // K. Miệng, răng
                obj.MOUTH_SHAPE = GetRadioValue(this.rdoMouthShape8);
                obj.NEONATAL_TEETH = GetRadioValue(this.rdoNeonatalTeeth8);
                obj.TONGUE_SHAPE = GetRadioValue(this.rdoTongueShape8);
                obj.TONGUE_TIE = GetRadioValue(this.rdoTongueTie8);
                obj.ORAL_THRUSH = GetRadioValue(this.rdoOralThrush8);
                obj.SMALL_CHIN = GetRadioValue(this.rdoSmallChin8);
                obj.TOOTH_DECAY = GetRadioValue(this.rdoToothDecay8);
                obj.MOUTHTEETH_NOTE = this.memMouthTeethNote8.Text;
                // L. Hô hấp
                obj.IRREGULAR_BREATH = GetRadioValue(this.rdoIrregularBreath8);
                obj.CHEST_RETRACTION = GetRadioValue(this.rdoChestRetraction8);
                obj.ABNORMAL_BREATH_SOUND = GetRadioValue(this.rdoAbnormalBreathSound8);
                obj.RESP_FAILURE_SIGN = GetRadioValue(this.rdoRespFailureSign8);
                obj.LUNG_AUSCULTATION = GetRadioValue(this.rdoLungAuscultation8);
                obj.RESP_NOTE = this.memRespNote8.Text;
                // M. Tim mạch
                obj.APEX_POSITION = GetRadioValue(this.rdoApexPosition8);
                obj.PERIPHERAL_PULSE = GetRadioValue(this.rdoPeripheralPulse8);
                obj.HEART_AUSCULTATION = GetRadioValue(this.rdoHeartAuscultation8);
                obj.CARDIO_NOTE = this.memCardioNote8.Text;
                // N. Bụng và cơ quan sinh dục
                obj.ABDOMEN_NAVEL = GetRadioValue(this.rdoAbdomenNavel8);
                obj.HEPATOSPLENOMEGALY = GetRadioValue(this.rdoHepatosplenomegaly8);
                obj.ABDOMEN_MASS = GetRadioValue(this.rdoAbdomenMass8);
                obj.ANUS = GetRadioValue(this.rdoAnus8);
                obj.GENITALIA = GetRadioValue(this.rdoGenitalia8);
                obj.ABDOMEN_NOTE = this.memAbdomenNote8.Text;
                // O. Cơ xương và thần kinh
                obj.ASYMMETRIC_MOVEMENT = GetRadioValue(this.rdoAsymmetricMovement8);
                obj.SUCKING_REFLEX = GetRadioValue(this.rdoSuckingReflex8);
                obj.GRASP_REFLEX = GetRadioValue(this.rdoGraspReflex8);
                obj.MORO_REFLEX = GetRadioValue(this.rdoMoroReflex8);
                obj.MUSCLE_TONE = GetRadioValue(this.rdoMuscleTone8);
                obj.HIP_JOINT = GetRadioValue(this.rdoHipJoint8);
                obj.MUSCLE_REFLEX = GetRadioValue(this.rdoMuscleReflex8);
                obj.SPINE_CHECK = GetRadioValue(this.rdoSpineCheck8);
                obj.LIMBS_JOINTS = GetRadioValue(this.rdoLimbsJoints8);
                obj.GAIT = GetRadioValue(this.rdoGait8);
                obj.RICKETS_SIGN_NEURO = GetRadioValue(this.rdoRicketsSignNeuro8);
                obj.MUSCULOSKELETAL_NOTE = this.memMusculoskeletalNote8.Text;
                // VII. Kết luận & tư vấn → HIS_KSK_GENERAL: Kết luận sức khỏe 1=BT,2=Nguy cơ lao,3=Có vấn đề
                obj.HEALTH_CONCLUSION_TYPE = GetRadioValue(this.rdoConclusionHealth8);
                obj.DISEASES = NullIfEmpty(this.memConclusionDetail8.Text);
                obj.TREATMENT_INSTRUCTION = NullIfEmpty(this.memAdviceNextExam8.Text);
                obj.HEALTH_EXAM_RANK_ID = this.cboHealthExamRank8.EditValue != null
                    ? (long?)System.Int64.Parse(this.cboHealthExamRank8.EditValue.ToString()) : null;
                obj.CONCLUDER_LOGINNAME = this.cboConcluder8.EditValue != null ? this.cboConcluder8.EditValue.ToString() : null;
                if (!string.IsNullOrEmpty(obj.CONCLUDER_LOGINNAME))
                {
                    var emp = BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == obj.CONCLUDER_LOGINNAME);
                    obj.CONCLUDER_USERNAME = (emp != null) ? emp.TDL_USERNAME : null;
                }
                // Kết luận theo bệnh (ICD-10): lấy từ UC chung (panel8)
                if (dicIcdConclusionUc.ContainsKey(7) && dicIcdConclusionUc[7] != null)
                {
                    var uc = dicIcdConclusionUc[7];
                    obj.CONCLUSION_ICD_TYPE = uc.GetConclusionIcdType();
                    obj.CONCLUSION_ICD_CODE = NullIfEmpty(uc.GetConclusionIcdCode());
                    obj.CONCLUSION_ICD_NAME = NullIfEmpty(uc.GetConclusionIcdName());
                }
                // Thời gian kết luận = thời điểm lưu (yyyyMMddHHmmss)
                obj.CONCLUSION_TIME = System.Convert.ToInt64(System.DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return obj;
        }

        /// <summary>
        /// Gom số đo sinh tồn (mục II–III) vào HIS_DHST — map các trường có cột tương ứng:
        /// Nhiệt độ→TEMPERATURE, Mạch→PULSE, Nhịp thở→BREATH_RATE, Chiều dài→HEIGHT, Cân nặng→WEIGHT.
        /// (Vòng đầu/chu vi cánh tay không có cột DHST → chỉ lưu ở HIS_KSK_UNDER_SIX.)
        /// Số đo nhập tự do (string) → parse sang số; không parse được thì để null.
        /// </summary>
        private HIS_DHST GetDhstUnderSix()
        {
            HIS_DHST obj = new HIS_DHST();
            try
            {
                if (dhstUnderSix != null) obj.ID = dhstUnderSix.ID;
                // Giữ ID DHST đã lưu (để UPDATE thay vì tạo mới)
                if (currentKskUnderSixEf != null && currentKskUnderSixEf.DHST_ID != null && currentKskUnderSixEf.DHST_ID > 0)
                    obj.ID = currentKskUnderSixEf.DHST_ID.Value;

                obj.TEMPERATURE = ParseDecimalOrNull(this.spnTemperature8.Text);
                obj.PULSE = ParseLongOrNull(this.spnPulse8.Text);
                obj.BREATH_RATE = ParseDecimalOrNull(this.spnRespiratoryRate8.Text);
                obj.HEIGHT = ParseDecimalOrNull(this.spnBodyLength8.Text);   // chiều dài ~ chiều cao (trẻ nhỏ)
                obj.WEIGHT = ParseDecimalOrNull(this.spnWeight8.Text);

                // Người đo = bác sĩ kết luận (nếu có)
                obj.EXECUTE_LOGINNAME = this.cboConcluder8.EditValue != null ? this.cboConcluder8.EditValue.ToString() : null;
                if (!string.IsNullOrEmpty(obj.EXECUTE_LOGINNAME))
                {
                    var emp = BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == obj.EXECUTE_LOGINNAME);
                    obj.EXECUTE_USERNAME = (emp != null) ? emp.TDL_USERNAME : null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return obj;
        }

        /// <summary>Parse chuỗi nhập tự do sang decimal? (null nếu rỗng/không hợp lệ). Bỏ ký tự không phải số/.,-.</summary>
        private decimal? ParseDecimalOrNull(string s)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                string t = System.Text.RegularExpressions.Regex.Replace(s.Trim(), @"[^0-9eE,\.\-]", "").Replace(",", ".");
                decimal d;
                if (decimal.TryParse(t, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                    return d;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return null;
        }

        /// <summary>Parse chuỗi nhập tự do sang long? (làm tròn nếu thập phân; null nếu không hợp lệ).</summary>
        private long? ParseLongOrNull(string s)
        {
            decimal? d = ParseDecimalOrNull(s);
            return d.HasValue ? (long?)System.Math.Round(d.Value) : (long?)null;
        }
    }
}
