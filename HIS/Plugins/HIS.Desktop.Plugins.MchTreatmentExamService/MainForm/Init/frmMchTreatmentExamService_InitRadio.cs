using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {

        /// <summary>
        /// Khởi tạo nhiều nhóm radio
        /// </summary>
        private void InitializeMultipleRadioGroups()
        {
            try
            {
                //Tab 1: 10 nhóm - 29 checkbox
                RegisterRadioGroup("ScreenPurpose", chkScreenPurpose_KB1, chkScreenPurpose_KSL1);
                RegisterRadioGroup("Gynecology", chkGynecologyN1, chkGynecologyY1);
                RegisterRadioGroup("ViaViliTest", chkViaViliTestKTH1, chkViaViliTestAT1, chkViaViliTestDT1, chkViaViliTestNNUT1);
                RegisterRadioGroup("CytologyTest", chkCytologyTestKTH1, chkCytologyTestBT1, chkCytologyTestBaT1);
                RegisterRadioGroup("HpvTest", chkHpvTestKTH1, chkHpvTestAT1, chkHpvTestDT1);
                RegisterRadioGroup("BreastExam", chkBreastExamKTH1, chkBreastExamBT1, chkBreastExamPHUC1);
                RegisterRadioGroup("BreastUltraSound", chkBreastUltraSoundKTH1, chkBreastUltraSoundBT1, chkBreastUltraSoundBaT1);
                RegisterRadioGroup("Mamography", chkMamographyKTH1, chkMamographyBT1, chkMamographyBaT1);
                RegisterRadioGroup("MentalStatus", chkMentalStatusBT1, chkMentalStatusNC1, chkMentalStatusBaT1);
                RegisterRadioGroup("MotionStatus", chkMotionStatusBT1, chkMotionStatusNC1, chkMotionStatusBaT1);
                //Tab 2: 11 nhóm - 27 checkbox
                RegisterRadioGroup("PelvicMeasurement", chkPelvicMeasurementBT2, chkPelvicMeasurementBaT2);
                RegisterRadioGroup("AnemiaStatus", chkAnemiaStatusKTH2, chkAnemiaStatusK2, chkAnemiaStatusC2);
                RegisterRadioGroup("UrineProtein", chkUrineProteinKTH2, chkUrineProteinK2, chkUrineProteinC2);
                RegisterRadioGroup("TestHiv", chkTestHivK2, chkTestHivC2);
                RegisterRadioGroup("TestHepatitisB", chkTestHepatitisBKTH2, chkTestHepatitisBAT2, chkTestHepatitisBDT2);
                RegisterRadioGroup("TestSyphilis", chkTestSyphilisKTH2, chkTestSyphilisAT2, chkTestSyphilisDT2);
                RegisterRadioGroup("TestBloodGlucose", chkTestBloodGlucoseKTH2, chkTestBloodGlucoseBT2, chkTestBloodGlucoseBaT2);
                RegisterRadioGroup("PrenatalScreening", chkPrenatalScreeningK2, chkPrenatalScreeningC2);
                RegisterRadioGroup("FetalHeart", chkFetalHeartK2, chkFetalHeartC2);
                RegisterRadioGroup("FetalPosition", chkFetalPositionBT2, chkFetalPositionBaT2);
                RegisterRadioGroup("BirthPrediction", chkBirthPredictionDT2, chkBirthPredictionDCNC2);
                //Tab 3: 
                //M:13 nhóm - 32 checkbox
                RegisterRadioGroup("AntenatalVisits", chkAntenatalVisitsK4, chkAntenatalVisitsC4);
                RegisterRadioGroup("TestHivScreen", chkTestHivScreenK3, chkBirthPredictionDCNC2);
                RegisterRadioGroup("TestHivIntrapartum", chkTestHivIntrapartumK3, chkTestHivIntrapartumC3);
                RegisterRadioGroup("TestSyphilisScreen", chkTestSyphilisScreenKTH3, chkTestSyphilisScreenAT3, chkTestSyphilisScreenDT3);
                RegisterRadioGroup("TestSyphilisIntrapartum", chkTestSyphilisIntrapartumKTH3, chkTestSyphilisIntrapartumAT3, chkTestSyphilisIntrapartumDT3);
                RegisterRadioGroup("TestHepbScreen", chkTestHepbScreenKTH3, chkTestHepbScreenAT3, chkTestHepbScreenDT3);
                RegisterRadioGroup("TestHepbIntrapartum", chkTestHepbIntrapartumKTH3, chkTestHepbIntrapartumAT3, chkTestHepbIntrapartumDT3);
                RegisterRadioGroup("TestGlucoseIntrapartum", chkTestGlucoseIntrapartumKTH3, chkTestGlucoseIntrapartumBT3, chkTestGlucoseIntrapartumBaT3);
                RegisterRadioGroup("DiagnosisGdm", chkDiagnosisGdmBT3, chkDiagnosisGdmTK3, chkDiagnosisGdmMT3);
                RegisterRadioGroup("FullTetanusDose", chkFullTetanusDoseKD3, chkFullTetanusDoseDM3);
                RegisterRadioGroup("MotherDeath", chkMotherDeathK3, chkMotherDeathC3);
                RegisterRadioGroup("FirstWeekCare", chkFirstWeekCareK3, chkFirstWeekCareC3);
                RegisterRadioGroup("Week2To6Care", chkWeek2To6CareK3, chkWeek2To6CareC3);

                //C:13 nhóm - 26 checkbox
                RegisterRadioGroup("IsDeath", chkIsDeathK3, chkIsDeathC3);
                RegisterRadioGroup("AbandonedChild", chkAbandonedChildK3, chkAbandonedChildC3);
                RegisterRadioGroup("LiveBirth", chkLiveBirthS3, chkLiveBirthC3);
                RegisterRadioGroup("NewbornScreening", chkNewbornScreeningK3, chkNewbornScreeningC3);
                RegisterRadioGroup("EssentialNewbornCare", chkEssentialNewbornCareK3, chkEssentialNewbornCareC3);
                RegisterRadioGroup("EarlyBreastfeeding", chkEarlyBreastfeedingK3, chkEarlyBreastfeedingC3);
                RegisterRadioGroup("VitaminK1", chkVitaminK1K3, chkVitaminK1C3);
                RegisterRadioGroup("HepbVaccine", chkHepbVaccineK3, chkHepbVaccineBf24h, chkHepbVaccineAf24h);
                RegisterRadioGroup("KangarooCare", chkKangarooCareK3, chkKangarooCareNQ3, chkKangarooCareLT3);
                RegisterRadioGroup("HasBirthCertificate", chkHasBirthCertificateCC3, chkHasBirthCertificateDC3);
                RegisterRadioGroup("BirthCertificateRound", chkBirthCertificateRoundLD3, chkBirthCertificateRoundLL3);
                RegisterRadioGroup("CareWeek1", chkCareWeek1K3, chkCareWeek1C3);
                RegisterRadioGroup("CareWeek2To6", chkCareWeek2To6K3, chkCareWeek2To6C3);

                //Tab 5 
                RegisterRadioGroup("AbortionComplication", chkAbortionComplicationKTH5, chkAbortionComplicationKTTC5, chkchkAbortionComplicationTH5);
                
                // Điền sẵn phương án đầu tiên cho các nhóm ô tích chọn khi tạo mới
                SetDefaultRadioGroups();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Đăng ký một nhóm radio
        /// </summary>
        private void RegisterRadioGroup(string groupName, params CheckEdit[] checkEdits)
        {
            try
            {
                var group = checkEdits.ToList();
                radioGroups[groupName] = group;

                foreach (var check in group)
                {
                    check.CheckedChanged -= RadioCheck_CheckedChanged;
                    check.CheckedChanged += RadioCheck_CheckedChanged;
                    check.Tag = groupName; // Gắn tên nhóm vào Tag
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RadioCheck_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var currentCheck = sender as CheckEdit;
                if (currentCheck == null) return;

                string groupName = currentCheck.Tag != null ? currentCheck.Tag.ToString() : null;
                if (string.IsNullOrEmpty(groupName) || !radioGroups.ContainsKey(groupName)) return;

                // QĐ 3412: người dùng chọn lại thì tự xóa cảnh báo bắt buộc của nhóm radio
                ClearRequiredErrorForRadioGroup(groupName);

                var group = radioGroups[groupName];

                // Kiểm tra xem group này có bắt buộc chọn hay không
                bool isRequiredGroup = IsRequiredRadioGroup(groupName);

                if (currentCheck.Checked)
                {
                    // Uncheck tất cả các checkbox khác trong nhóm
                    foreach (var check in group)
                    {
                        if (check != currentCheck && check.Checked)
                        {
                            check.CheckedChanged -= RadioCheck_CheckedChanged;
                            check.Checked = false;
                            check.CheckedChanged += RadioCheck_CheckedChanged;
                        }
                    }
                }
                else if (isRequiredGroup)
                {
                    // Nếu là group bắt buộc và user cố gắng uncheck
                    // Kiểm tra xem còn checkbox nào checked không
                    bool hasAnyChecked = group.Any(c => c.Checked);

                    if (!hasAnyChecked)
                    {
                        // Không cho phép uncheck - re-check lại
                        currentCheck.CheckedChanged -= RadioCheck_CheckedChanged;
                        currentCheck.Checked = true;
                        currentCheck.CheckedChanged += RadioCheck_CheckedChanged;

                        Inventec.Common.Logging.LogSystem.Debug("RadioCheck_CheckedChanged: Group '" + groupName + "' is required, preventing uncheck");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiểm tra group có bắt buộc chọn hay không
        /// </summary>
        private bool IsRequiredRadioGroup(string groupName)
        {
            // Danh sách các group bắt buộc chọn (phải có ít nhất 1 checkbox được check)
            List<string> requiredGroups = new List<string>
            {
                "ScreenPurpose",    // Tab 1: Loại khám (Khám bệnh / Khám sàng lọc)
                "Gynecology"        // Tab 1: Điều trị PK (Không / Có)
            };

            return requiredGroups.Contains(groupName);
        }

        /// <summary>
        /// Danh sách nhóm ô tích chọn được điền sẵn phương án đầu tiên khi tạo mới.
        /// 47 nhóm thuộc 4 mục: Khám sàng lọc, Trẻ em dưới 6 tuổi, Khám thai, Sinh đẻ (Mẹ + Con).
        /// KHÔNG áp dụng cho mục Phá thai (nhóm AbortionComplication) và mục Tránh thai (không có ô tích chọn).
        /// Phương án đầu tiên của mỗi nhóm ứng với trường hợp thông thường:
        /// Bình thường / Không thực hiện / Không / Sống / Đẻ thường / Chưa cấp / Lần đầu / Không đủ.
        /// </summary>
        private static readonly string[] DEFAULT_FIRST_OPTION_RADIO_GROUPS = new string[]
        {
            // Mục Khám sàng lọc
            "ScreenPurpose",
            "Gynecology",
            "ViaViliTest",
            "CytologyTest",
            "HpvTest",
            "BreastExam",
            "BreastUltraSound",
            "Mamography",

            // Mục Trẻ em dưới 6 tuổi
            "MentalStatus",
            "MotionStatus",

            // Mục Khám thai
            "PelvicMeasurement",
            "AnemiaStatus",
            "UrineProtein",
            "TestHiv",
            "TestHepatitisB",
            "TestSyphilis",
            "TestBloodGlucose",
            "PrenatalScreening",
            "FetalHeart",
            "FetalPosition",
            "BirthPrediction",

            // Mục Sinh đẻ - Mẹ
            "AntenatalVisits",
            "TestHivScreen",
            "TestHivIntrapartum",
            "TestSyphilisScreen",
            "TestSyphilisIntrapartum",
            "TestHepbScreen",
            "TestHepbIntrapartum",
            "TestGlucoseIntrapartum",
            "DiagnosisGdm",
            "FullTetanusDose",
            "MotherDeath",
            "FirstWeekCare",
            "Week2To6Care",

            // Mục Sinh đẻ - Con
            "IsDeath",
            "AbandonedChild",
            "LiveBirth",
            "NewbornScreening",
            "EssentialNewbornCare",
            "EarlyBreastfeeding",
            "VitaminK1",
            "HepbVaccine",
            "KangarooCare",
            "HasBirthCertificate",
            "BirthCertificateRound",
            "CareWeek1",
            "CareWeek2To6"
        };

        /// <summary>
        /// Điền sẵn phương án đầu tiên cho các nhóm ô tích chọn.
        /// Chỉ áp dụng khi tạo mới hoặc sau khi bấm Làm lại — mở hồ sơ đã lưu thì không gọi hàm này.
        /// </summary>
        private void SetDefaultRadioGroups()
        {
            try
            {
                foreach (string groupName in DEFAULT_FIRST_OPTION_RADIO_GROUPS)
                {
                    SetDefaultFirstOptionForGroup(groupName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Điền phương án đầu tiên cho một nhóm. Nhóm đã có ô được chọn thì giữ nguyên,
        /// nên không ghi đè lựa chọn của người dùng.
        /// </summary>
        private void SetDefaultFirstOptionForGroup(string groupName)
        {
            try
            {
                if (radioGroups == null || !radioGroups.ContainsKey(groupName)) return;

                var group = radioGroups[groupName];
                if (group == null || group.Count == 0) return;

                if (group.Any(c => c.Checked)) return;

                var firstCheck = group[0];
                firstCheck.CheckedChanged -= RadioCheck_CheckedChanged;
                firstCheck.Checked = true;
                firstCheck.CheckedChanged += RadioCheck_CheckedChanged;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy CheckEdit đang được chọn trong nhóm
        /// </summary>
        private CheckEdit GetSelectedCheck(string groupName)
        {
            try
            {
                if (radioGroups.ContainsKey(groupName))
                {
                    return radioGroups[groupName].FirstOrDefault(c => c.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// Lấy Tag của CheckEdit đang được chọn
        /// </summary>
        private object GetSelectedTag(string groupName)
        {
            try
            {
                var selected = GetSelectedCheck(groupName);
                if (selected != null)
                {
                    return selected.Properties.Tag ?? selected.Tag;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// Set CheckEdit được chọn trong nhóm
        /// </summary>
        private void SetSelectedCheck(string groupName, CheckEdit checkToSelect)
        {
            try
            {
                if (radioGroups.ContainsKey(groupName) && radioGroups[groupName].Contains(checkToSelect))
                {
                    foreach (var check in radioGroups[groupName])
                    {
                        check.CheckedChanged -= RadioCheck_CheckedChanged;
                        check.Checked = (check == checkToSelect);
                        check.CheckedChanged += RadioCheck_CheckedChanged;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xóa tất cả lựa chọn trong nhóm
        /// </summary>
        private void ClearGroup(string groupName)
        {
            try
            {
                if (radioGroups.ContainsKey(groupName))
                {
                    foreach (var check in radioGroups[groupName])
                    {
                        check.CheckedChanged -= RadioCheck_CheckedChanged;
                        check.Checked = false;
                        check.CheckedChanged += RadioCheck_CheckedChanged;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xóa tất cả lựa chọn trong tất cả các nhóm
        /// </summary>
        private void ClearAllGroups()
        {
            try
            {
                foreach (var groupName in radioGroups.Keys.ToList())
                {
                    ClearGroup(groupName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả các nhóm
        /// </summary>
        private List<string> GetAllGroupNames()
        {
            return radioGroups.Keys.ToList();
        }

        /// <summary>
        /// Kiểm tra nhóm đã được chọn hay chưa
        /// </summary>
        private bool IsGroupSelected(string groupName)
        {
            return GetSelectedCheck(groupName) != null;
        }

    }
}
