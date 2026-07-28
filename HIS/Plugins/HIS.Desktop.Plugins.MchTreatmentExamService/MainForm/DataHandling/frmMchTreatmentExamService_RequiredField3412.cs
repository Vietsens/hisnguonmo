using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.MchTreatmentExamService.UCAdress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    /// <summary>
    /// Ràng buộc nhập bắt buộc theo QĐ 3412/QĐ-BYT — áp dụng cố định (không dùng key cấu hình).
    /// Mỗi loại khám (tab) có một bộ trường bắt buộc riêng; loại khám không thuộc danh mục
    /// QĐ 3412 => không có rule => không chặn lưu.
    /// </summary>
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        /// <summary>Error provider riêng cho ràng buộc QĐ 3412 (độc lập với dxValidationProvider).</summary>
        private DXErrorProvider dxErrRequired3412;

        /// <summary>
        /// Một trường bắt buộc: nhãn hiển thị, hàm kiểm tra thiếu, control để focus/đánh dấu cảnh báo.
        /// </summary>
        private class RequiredFieldRule
        {
            public string Label;
            public Func<bool> IsMissing;
            public Control MarkControl;
        }

        #endregion

        #region Entry point

        /// <summary>
        /// Validate các trường bắt buộc theo QĐ 3412 cho tab đang chọn.
        /// Trả về true nếu đủ, false nếu thiếu (đã hiện cảnh báo + focus + đánh dấu).
        /// </summary>
        private bool ValidateRequiredFields3412(int tabIndex)
        {
            try
            {
                if (dxErrRequired3412 == null)
                {
                    dxErrRequired3412 = new DXErrorProvider();
                }

                // Xóa cảnh báo cũ trước mỗi lần kiểm tra
                dxErrRequired3412.ClearErrors();

                List<RequiredFieldRule> rules = BuildRequiredRules(tabIndex);
                if (rules == null || rules.Count == 0)
                    return true;

                List<RequiredFieldRule> missing = new List<RequiredFieldRule>();
                foreach (var rule in rules)
                {
                    try
                    {
                        if (rule != null && rule.IsMissing != null && rule.IsMissing())
                            missing.Add(rule);
                    }
                    catch (Exception exRule)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exRule);
                    }
                }

                if (missing.Count == 0)
                    return true;

                // Đánh dấu cảnh báo tại từng control thiếu
                foreach (var rule in missing)
                {
                    MarkRequiredError(rule.MarkControl, rule.Label + " là trường bắt buộc.");
                }

                // Đưa con trỏ tới trường thiếu đầu tiên
                var first = missing.FirstOrDefault(o => o.MarkControl != null);
                if (first != null && first.MarkControl != null)
                {
                    EnsureControlOnActiveTab(first.MarkControl);
                    try
                    {
                        first.MarkControl.Focus();
                        if (first.MarkControl is BaseEdit)
                            ((BaseEdit)first.MarkControl).SelectAll();
                    }
                    catch (Exception exFocus)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exFocus);
                    }
                }

                // Hiện cảnh báo liệt kê các trường còn thiếu
                ShowMissingRequiredMessage(missing);
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                // Có lỗi khi validate thì không chặn lưu để tránh khóa nghiệp vụ
                return true;
            }
        }

        private void ShowMissingRequiredMessage(List<RequiredFieldRule> missing)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Vui lòng nhập đầy đủ các trường bắt buộc theo QĐ 3412/QĐ-BYT:");
                sb.AppendLine();
                foreach (var rule in missing)
                {
                    sb.AppendLine("- " + rule.Label);
                }

                DevExpress.XtraEditors.XtraMessageBox.Show(
                    sb.ToString(),
                    "Thiếu thông tin bắt buộc",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Build rules theo tab

        /// <summary>
        /// Sinh bộ trường bắt buộc theo tab đang chọn.
        /// Ánh xạ tab index: 0-Sàng lọc, 1-Trẻ em dưới 6 tuổi, 2-Khám thai, 3-Sinh đẻ, 4-Tránh thai, 5-Phá thai.
        /// </summary>
        private List<RequiredFieldRule> BuildRequiredRules(int tabIndex)
        {
            List<RequiredFieldRule> rules = new List<RequiredFieldRule>();
            try
            {
                switch (tabIndex)
                {
                    case 0: BuildRulesScreening(rules); break;      // Sàng lọc
                    case 1: BuildRulesChildUnder6(rules); break;    // Trẻ em dưới 6 tuổi
                    case 2: BuildRulesAntenatal(rules); break;      // Khám thai
                    case 3: BuildRulesBirth(rules); break;          // Sinh đẻ
                    case 4: BuildRulesContraception(rules); break;  // Tránh thai
                    case 5: BuildRulesAbortion(rules); break;       // Phá thai
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rules;
        }

        // ---------------- 4.1 Khám thai ----------------
        private void BuildRulesAntenatal(List<RequiredFieldRule> rules)
        {
            AddHeader(rules, dteExam2, cboUser2);
            rules.Add(RuleMultiSelect("Tiền sử nội khoa", cboMedicalHistoryInternal2, () => MedicalHistoryInternal2Selected == null || MedicalHistoryInternal2Selected.Count == 0));
            rules.Add(RuleSpinRange("Tuổi thai (1–42 tuần)", spnGestationalAge2, 1, 42));
            rules.Add(RuleSpin("Huyết áp tâm thu", spnBloodPressureSystolic2));
            rules.Add(RuleSpin("Huyết áp tâm trương", spnBloodPressureDiastolic2));
            rules.Add(RuleRadio("Xét nghiệm thiếu máu", "AnemiaStatus"));
            rules.Add(RuleRadio("Protein niệu", "UrineProtein"));
            rules.Add(RuleRadio("Xét nghiệm HIV", "TestHiv"));
            rules.Add(RuleRadio("Xét nghiệm viêm gan B", "TestHepatitisB"));
            rules.Add(RuleRadio("Xét nghiệm giang mai", "TestSyphilis"));
            rules.Add(RuleRadio("Xét nghiệm đường huyết", "TestBloodGlucose"));
            rules.Add(RuleRadio("Sàng lọc trước sinh", "PrenatalScreening"));
            rules.Add(RuleRadio("Tim thai", "FetalHeart"));
            rules.Add(RuleRadio("Ngôi thai", "FetalPosition"));
            rules.Add(RuleCombo("Chuyên môn người khám thai (Trình độ)", cboDiploma2));
        }

        // ---------------- 4.2 Sinh đẻ ----------------
        private void BuildRulesBirth(List<RequiredFieldRule> rules)
        {
            // a) Thông tin lần sinh (mẹ)
            AddHeader(rules, dteExam3, cboUser3);
            rules.Add(RuleSpin("Tuần thai khi sinh", spnGestationalWeeks3));
            rules.Add(RuleDate("Ngày đẻ", dteBornTime3));
            rules.Add(RuleCombo("Nơi đẻ", cboBirthplaceType3));
            rules.Add(RuleAddress("Địa chỉ nơi đẻ (Tỉnh / Xã / địa chỉ)", addressMother));
            rules.Add(RuleSpin("Lần sinh con", spnBirthOrder3));
            rules.Add(RuleRadio("Sản phụ khám thai 4 lần", "AntenatalVisits"));
            rules.Add(RuleRadio("Xét nghiệm HIV khi mang thai", "TestHivScreen"));
            rules.Add(RuleRadio("Xét nghiệm HIV khi chuyển dạ", "TestHivIntrapartum"));
            rules.Add(RuleRadio("Xét nghiệm giang mai khi mang thai", "TestSyphilisScreen"));
            rules.Add(RuleRadio("Xét nghiệm giang mai khi chuyển dạ", "TestSyphilisIntrapartum"));
            rules.Add(RuleRadio("Xét nghiệm viêm gan B khi mang thai", "TestHepbScreen"));
            rules.Add(RuleRadio("Xét nghiệm viêm gan B khi chuyển dạ", "TestHepbIntrapartum"));
            rules.Add(RuleRadio("Xét nghiệm đường huyết khi chuyển dạ", "TestGlucoseIntrapartum"));
            rules.Add(RuleRadio("Chẩn đoán đái tháo đường thai kỳ", "DiagnosisGdm"));
            rules.Add(RuleRadio("Tiêm uốn ván đủ mũi", "FullTetanusDose"));
            rules.Add(RuleSpin("Số lần đẻ đủ tháng", spnTermBirths3));
            rules.Add(RuleSpin("Số lần đẻ non", spnPretermBirth3));
            rules.Add(RuleSpin("Số lần sảy / phá thai", spnMiscarriage3));
            rules.Add(RuleSpin("Tổng số con hiện có", spnChildCount3));
            rules.Add(RuleCombo("Cách đẻ", cboBirthMethod3));
            // TAI_BIEN_SK (Tai biến sản khoa): QĐ 3412 bản 1.15 (16/12/2025) BỎ bắt buộc.
            rules.Add(RuleSpin("Số con sinh ra lần này", spnNumberNewbornBirth3));
            rules.Add(RuleSpin("Số trẻ đẻ ra sống", spnNewbornAlive3));
            rules.Add(RuleCombo("Tình trạng con", cboNewbornCondition3));
            rules.Add(RuleRadio("Chăm sóc sau sinh tuần đầu", "FirstWeekCare"));
            rules.Add(RuleRadio("Chăm sóc sau sinh 2–6 tuần", "Week2To6Care"));
            rules.Add(RuleCombo("Chuyên môn người đỡ đẻ (Trình độ)", cboDiploma3));
            rules.Add(RuleRadio("Tử vong mẹ", "MotherDeath"));

            // b) Thông tin trẻ sơ sinh
            rules.Add(RuleRadio("Tử vong thai nhi", "IsDeath"));

            // Nếu "Tử vong thai nhi" = Tử vong (mã 1) => khóa/không validate các trường trẻ sơ sinh còn lại
            bool isFetalDeath = GetRadioGroupValue("IsDeath").GetValueOrDefault((short)(-1)) == 1;
            if (isFetalDeath)
                return;

            // Nếu tích "Trẻ sơ sinh bị bỏ rơi" => bắt buộc Số định danh người nuôi dưỡng; ẩn các trường chăm sóc thiết yếu
            bool isAbandoned = GetRadioGroupValue("AbandonedChild").GetValueOrDefault((short)(-1)) == 1;

            rules.Add(RuleRadio("Trẻ đẻ ra sống", "LiveBirth"));
            rules.Add(RuleCombo("Tình trạng con (trẻ sơ sinh)", cboChildStatus3));
            rules.Add(RuleCombo("Giới tính con", cboChildGender3));
            rules.Add(RuleSpin("Cân nặng con", spnW3));
            rules.Add(RuleSpin("Chiều dài con", spnH3));
            rules.Add(RuleSpin("Vòng đầu con", spnVH3));
            rules.Add(RuleRadio("Sàng lọc sơ sinh", "NewbornScreening"));

            if (isAbandoned)
            {
                rules.Add(RuleText("Số định danh người nuôi dưỡng", txtCccdNumber3));
            }
            else
            {
                rules.Add(RuleRadio("Chăm sóc sơ sinh thiết yếu", "EssentialNewbornCare"));
                rules.Add(RuleCombo("Thực hiện da kề da", cboSkinToSkin3));
                rules.Add(RuleRadio("Cho trẻ bú mẹ sớm", "EarlyBreastfeeding"));
                rules.Add(RuleRadio("Chăm sóc Kangaroo", "KangarooCare"));
            }

            rules.Add(RuleRadio("Tiêm vắc xin viêm gan B sơ sinh", "HepbVaccine"));
            rules.Add(RuleRadio("Tiêm vitamin K1 sơ sinh", "VitaminK1"));
            rules.Add(RuleRadio("Cấp giấy chứng sinh", "HasBirthCertificate"));
            rules.Add(RuleText("Mã giấy chứng sinh", txtBirthCertificateCode3));
            rules.Add(RuleRadio("Lần cấp giấy chứng sinh", "BirthCertificateRound"));
            rules.Add(RuleDate("Ngày cấp giấy chứng sinh", dteBirthCertificateDate3));
            rules.Add(RuleDate("Ngày sinh con", dteChildBirthDate3));
            rules.Add(RuleAddress("Địa chỉ nơi đẻ (con)", addressBaby));
            // MA_THE_TAM (Mã thẻ BHYT tạm thời): QĐ 3412 bản 1.15 (16/12/2025) BỎ bắt buộc.
            rules.Add(RuleText("Họ tên người đỡ đẻ", txtDeliveryAssistant3));
            rules.Add(RuleRadio("Chăm sóc sau sinh tuần đầu (con)", "CareWeek1"));
            rules.Add(RuleRadio("Chăm sóc sau sinh 2–6 tuần (con)", "CareWeek2To6"));
        }

        // ---------------- 4.3 Tránh thai ----------------
        private void BuildRulesContraception(List<RequiredFieldRule> rules)
        {
            AddHeader(rules, dteExam4, cboUser4);
            rules.Add(RuleCombo("Biện pháp tránh thai", cboContraceptionMethod4));
            rules.Add(RuleCombo("Tai biến tránh thai", cboContraceptionComplication4));
            rules.Add(RuleCombo("Chuyên môn người thực hiện (Trình độ)", cboDiploma4));
        }

        // ---------------- 4.4 Phá thai ----------------
        private void BuildRulesAbortion(List<RequiredFieldRule> rules)
        {
            AddHeader(rules, dteExam5, cboUser5);
            rules.Add(RuleSpinRange("Tuần tuổi thai khi phá thai (≥ 1)", spnGestationalWeeks5, 1, int.MaxValue));
            rules.Add(RuleCombo("Phương pháp phá thai", cboAbortionMethod5));
            rules.Add(RuleRadio("Tai biến phá thai", "AbortionComplication"));
        }

        // ---------------- 4.5 Khám sàng lọc ----------------
        private void BuildRulesScreening(List<RequiredFieldRule> rules)
        {
            AddHeader(rules, dteExam1, cboUser1);
            rules.Add(RuleRadio("Mục đích khám phụ khoa", "ScreenPurpose"));
            rules.Add(RuleRadio("Điều trị phụ khoa", "Gynecology"));
            rules.Add(RuleCombo("Chẩn đoán ung thư cổ tử cung", cboCervicalCancerDx1));
            rules.Add(RuleCombo("Điều trị tiền ung thư cổ tử cung", cboPreCervicalCancerTreat1));
            rules.Add(RuleRadio("Nghiệm pháp VIA/VILI", "ViaViliTest"));
            rules.Add(RuleRadio("Xét nghiệm tế bào học", "CytologyTest"));
            rules.Add(RuleRadio("Xét nghiệm HPV", "HpvTest"));
            rules.Add(RuleRadio("Khám vú", "BreastExam"));
            rules.Add(RuleRadio("Siêu âm vú", "BreastUltraSound"));
            rules.Add(RuleRadio("X-quang vú", "Mamography"));
        }

        // ---------------- 4.6 Trẻ em dưới 6 tuổi ----------------
        private void BuildRulesChildUnder6(List<RequiredFieldRule> rules)
        {
            AddHeader(rules, dteExam8, cboUser8);
            rules.Add(RuleRadio("Phát triển tinh thần", "MentalStatus"));
            rules.Add(RuleRadio("Phát triển vận động", "MotionStatus"));
            rules.Add(RuleSpin("Cân nặng trẻ", spnW1));
            rules.Add(RuleSpin("Chiều cao trẻ", spnH1));
            rules.Add(RuleSpin("Vòng đầu trẻ", spnHC1));
        }

        /// <summary>Nhóm chung: Ngày khám + Người khám (áp dụng cho mọi loại khám).</summary>
        private void AddHeader(List<RequiredFieldRule> rules, DateEdit dteExam, GridLookUpEdit cboUser)
        {
            rules.Add(RuleDate("Ngày khám", dteExam));
            rules.Add(RuleCombo("Người khám", cboUser));
        }

        #endregion

        #region Rule factory

        private RequiredFieldRule RuleCombo(string label, GridLookUpEdit cbo)
        {
            return new RequiredFieldRule
            {
                Label = label,
                MarkControl = cbo,
                IsMissing = () => GetComboValue(cbo) == null
            };
        }

        private RequiredFieldRule RuleMultiSelect(string label, GridLookUpEdit cbo, Func<bool> isMissing)
        {
            return new RequiredFieldRule { Label = label, MarkControl = cbo, IsMissing = isMissing };
        }

        private RequiredFieldRule RuleSpin(string label, SpinEdit spn)
        {
            return new RequiredFieldRule
            {
                Label = label,
                MarkControl = spn,
                IsMissing = () => IsSpinEmpty(spn)
            };
        }

        private RequiredFieldRule RuleSpinRange(string label, SpinEdit spn, int min, int max)
        {
            return new RequiredFieldRule
            {
                Label = label,
                MarkControl = spn,
                IsMissing = () =>
                {
                    if (IsSpinEmpty(spn)) return true;
                    try
                    {
                        decimal v = spn.Value;
                        return v < min || v > max;
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return true; }
                }
            };
        }

        private RequiredFieldRule RuleDate(string label, DateEdit dte)
        {
            return new RequiredFieldRule
            {
                Label = label,
                MarkControl = dte,
                IsMissing = () => dte == null || dte.EditValue == null
            };
        }

        private RequiredFieldRule RuleText(string label, TextEdit txt)
        {
            return new RequiredFieldRule
            {
                Label = label,
                MarkControl = txt,
                IsMissing = () => txt == null || string.IsNullOrWhiteSpace(txt.Text)
            };
        }

        private RequiredFieldRule RuleRadio(string label, string groupName)
        {
            Control anchor = null;
            if (radioGroups != null && radioGroups.ContainsKey(groupName) && radioGroups[groupName].Count > 0)
                anchor = radioGroups[groupName][0];

            return new RequiredFieldRule
            {
                Label = label,
                MarkControl = anchor,
                IsMissing = () => GetRadioGroupValue(groupName) == null
            };
        }

        private RequiredFieldRule RuleAddress(string label, UCAddress ucAddress)
        {
            return new RequiredFieldRule
            {
                Label = label,
                MarkControl = ucAddress,
                IsMissing = () => IsAddressMissing(ucAddress)
            };
        }

        #endregion

        #region Helpers

        private bool IsSpinEmpty(SpinEdit spn)
        {
            try
            {
                return spn == null || spn.EditValue == null || string.IsNullOrWhiteSpace(spn.EditValue.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true;
            }
        }

        private bool IsAddressMissing(UCAddress ucAddress)
        {
            try
            {
                if (ucAddress == null) return true;
                var ado = ucAddress.GetValue();
                if (ado == null) return true;
                return string.IsNullOrWhiteSpace(ado.Province_Code)
                    || string.IsNullOrWhiteSpace(ado.Commune_Code)
                    || string.IsNullOrWhiteSpace(ado.Address);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true;
            }
        }

        /// <summary>
        /// Đánh dấu cảnh báo tại control và đăng ký sự kiện tự xóa cảnh báo khi người dùng nhập lại.
        /// </summary>
        private void MarkRequiredError(Control ctrl, string message)
        {
            try
            {
                if (ctrl == null || dxErrRequired3412 == null) return;

                if (ctrl is BaseEdit)
                {
                    var edit = (BaseEdit)ctrl;
                    edit.EditValueChanged -= RequiredField3412_EditValueChanged;
                    edit.EditValueChanged += RequiredField3412_EditValueChanged;
                }

                dxErrRequired3412.SetError(ctrl, message, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RequiredField3412_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dxErrRequired3412 != null && sender is Control)
                {
                    dxErrRequired3412.SetError((Control)sender, "");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Xóa cảnh báo tại control anchor của một nhóm radio (gọi từ RadioCheck_CheckedChanged).
        /// </summary>
        private void ClearRequiredErrorForRadioGroup(string groupName)
        {
            try
            {
                if (dxErrRequired3412 == null) return;
                if (radioGroups != null && radioGroups.ContainsKey(groupName) && radioGroups[groupName].Count > 0)
                {
                    dxErrRequired3412.SetError(radioGroups[groupName][0], "");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đảm bảo control nằm trên tab đang hiển thị (kích hoạt cả tab lồng nhau) trước khi focus.
        /// </summary>
        private void EnsureControlOnActiveTab(Control ctrl)
        {
            try
            {
                Control c = ctrl;
                while (c != null)
                {
                    var page = c as DevExpress.XtraTab.XtraTabPage;
                    if (page != null)
                    {
                        var tabControl = page.Parent as DevExpress.XtraTab.XtraTabControl;
                        if (tabControl != null)
                            tabControl.SelectedTabPage = page;
                    }
                    c = c.Parent;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
