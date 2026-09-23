using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MchTreatmentExamService.ADO;
using HIS.Desktop.Plugins.MchTreatmentExamService.MainForm.Dialogs;
using HIS.Desktop.Utilities.Extensions;
using MCH.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Copy Baby Data to Birth Info (Mother + Child)

        // Danh mục HIS_BORN_TYPE (Cách sinh trên giấy chứng sinh)
        private const long BORN_TYPE_ID__THUONG = 1;
        private const long BORN_TYPE_ID__MO = 2;

        // Danh mục HIS_BORN_RESULT (Tình trạng trên giấy chứng sinh)
        private const long BORN_RESULT_ID__SONG = 1;
        private const long BORN_RESULT_ID__CHET = 2;

        // Mã danh mục của màn SKSS (QĐ 3412)
        private const string MCH_BIRTH_METHOD__DE_THUONG = "1";
        private const string MCH_BIRTH_METHOD__MO_LAY_THAI = "2";
        private const string MCH_NEWBORN_CONDITION__BINH_THUONG = "1";
        private const string MCH_COMPLICATION__BANG_HUYET = "1";
        private const string MCH_COMPLICATION__SAN_GIAT = "3";
        private const string MCH_COMPLICATION__VO_TU_CUNG = "4";
        private const string MCH_COMPLICATION__NHIEM_TRUNG = "5";

        /// <summary>
        /// Người dùng bấm nút "Lấy từ GCS" trên mục Sinh đẻ: lấy thông tin Mẹ và Con
        /// từ giấy chứng sinh của lượt điều trị. Có nhiều giấy chứng sinh (sinh đôi...) thì cho chọn.
        /// </summary>
        private void ProcessCopyFromBaby()
        {
            try
            {
                if (Treatment == null || Treatment.ID <= 0)
                {
                    XtraMessageBox.Show("Chưa có thông tin hồ sơ điều trị", "Thông báo",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }

                List<HIS_BABY> babies = GetBabies();
                if (!isBabyLoaded)
                {
                    XtraMessageBox.Show("Không lấy được thông tin từ giấy chứng sinh. Vui lòng thử lại.", "Thông báo",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }
                if (babies == null || babies.Count == 0)
                {
                    XtraMessageBox.Show("Lượt điều trị chưa có giấy chứng sinh", "Thông báo",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }

                HIS_BABY selected = null;
                if (babies.Count == 1)
                {
                    selected = babies[0];
                }
                else
                {
                    using (frmSelectBaby frm = new frmSelectBaby(babies))
                    {
                        if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            selected = frm.SelectedBaby;
                        }
                    }
                }
                if (selected == null) return;

                // Thao tác chủ động → ghi đè ô có nguồn; chuyển sang phần Mẹ để người dùng kiểm tra
                CopyBabyData(selected, true, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Không lấy được thông tin từ giấy chứng sinh: " + ex.Message, "Lỗi",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Lấy thông tin giấy chứng sinh sang mục Sinh đẻ — cả phần Mẹ và phần Con.
        /// </summary>
        /// <param name="baby">Giấy chứng sinh nguồn</param>
        /// <param name="overwrite">
        /// true: thao tác chủ động (nút / biểu tượng trên cây dịch vụ) — ghi đè ô có dữ liệu nguồn,
        /// ô không có nguồn giữ nguyên.
        /// false: tự lấy khi mở mục — chỉ điền ô đang trống.
        /// </param>
        /// <param name="selectChildTab">
        /// null: giữ nguyên mục đang xem; true: chuyển sang Sinh đẻ - Con; false: chuyển sang Sinh đẻ - Mẹ.
        /// </param>
        private void CopyBabyData(HIS_BABY baby, bool overwrite, bool? selectChildTab)
        {
            try
            {
                if (baby == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Baby data is null");
                    return;
                }

                CopyBabyDataToMotherTab(baby, overwrite);
                CopyBabyDataToChildTab(baby, overwrite);

                if (selectChildTab.HasValue && xtraTabControl1 != null && xtraTabControl1.TabPages.Count > 3)
                {
                    xtraTabControl1.SelectedTabPageIndex = 3; // Sinh đẻ (sau khi chèn tab Trẻ em dưới 6 tuổi)
                    xtraTabControl2.SelectedTabPageIndex = selectChildTab.Value ? 1 : 0;
                }

                Inventec.Common.Logging.LogSystem.Debug(
                    "CopyBabyData: Đã lấy giấy chứng sinh sang mục Sinh đẻ (Mẹ + Con). BabyId="
                    + baby.ID + ". Overwrite=" + overwrite);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                if (overwrite)
                {
                    XtraMessageBox.Show(
                        "Có lỗi xảy ra khi copy dữ liệu: " + ex.Message,
                        "Lỗi",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        #region Phần Mẹ

        /// <summary>
        /// Điền phần Sinh đẻ - Mẹ trực tiếp lên các ô nhập từ giấy chứng sinh và hồ sơ điều trị
        /// (lịch sử sinh của mẹ: số lần sinh, đủ tháng, đẻ non, sảy thai lưu trên hồ sơ điều trị).
        /// Không đụng tới các ô không có nguồn (Người khám, Trình độ, xét nghiệm, tiêm uốn ván...).
        /// </summary>
        private void CopyBabyDataToMotherTab(HIS_BABY baby, bool overwrite)
        {
            try
            {
                // Cách đẻ: Thường → Đẻ thường; Mổ → Mổ lấy thai; Khó → không quy đổi được (để người dùng chọn)
                string birthMethod = null;
                if (baby.BORN_TYPE_ID == BORN_TYPE_ID__THUONG) birthMethod = MCH_BIRTH_METHOD__DE_THUONG;
                else if (baby.BORN_TYPE_ID == BORN_TYPE_ID__MO) birthMethod = MCH_BIRTH_METHOD__MO_LAY_THAI;
                SetComboIfAllowed(cboBirthMethod3, birthMethod, overwrite);

                // Tai biến sản khoa (chọn nhiều). "Uốn ván" của mẹ không có mục tương ứng (SKSS chỉ có Uốn ván sơ sinh) → bỏ qua
                List<string> complications = new List<string>();
                if (baby.IS_HAEMORRHAGE == 1) complications.Add(MCH_COMPLICATION__BANG_HUYET);
                if (baby.IS_PUERPERAL == 1) complications.Add(MCH_COMPLICATION__SAN_GIAT);
                if (baby.IS_UTERINE_RUPTURE == 1) complications.Add(MCH_COMPLICATION__VO_TU_CUNG);
                if (baby.IS_BACTERIAL_CONTAMINATION == 1) complications.Add(MCH_COMPLICATION__NHIEM_TRUNG);
                if (complications.Count > 0
                    && (overwrite || MaternalComplication3Selected == null || MaternalComplication3Selected.Count == 0))
                {
                    GridCheckMarksSelection gridCheckMaternal = cboMaternalComplication3.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMaternal != null)
                    {
                        gridCheckMaternal.ClearSelection(cboMaternalComplication3.Properties.View);
                        ProcessSelectMaternalComplication(string.Join(";", complications), gridCheckMaternal);
                    }
                }

                // Số con sinh ra ← Số con sinh trong lần sinh; Số con sống ← Số con hiện sống
                SetSpinIfAllowed(spnNumberNewbornBirth3, baby.NUMBER_CHILDREN_BIRTH, overwrite);
                SetSpinIfAllowed(spnNewbornAlive3, baby.CURRENT_ALIVE, overwrite);

                // Tình trạng con: Sống → Bình thường; Chết → để trống
                string newbornCondition = baby.BORN_RESULT_ID == BORN_RESULT_ID__SONG ? MCH_NEWBORN_CONDITION__BINH_THUONG : null;
                SetComboIfAllowed(cboNewbornCondition3, newbornCondition, overwrite);

                // Lịch sử sinh của mẹ — lưu trên hồ sơ điều trị
                if (Treatment != null)
                {
                    SetSpinIfAllowed(spnBirthOrder3, Treatment.NUMBER_OF_BIRTH, overwrite);
                    SetSpinIfAllowed(spnTermBirths3, Treatment.NUMBER_OF_FULL_TERM_BIRTH, overwrite);
                    SetSpinIfAllowed(spnPretermBirth3, Treatment.NUMBER_OF_PREMATURE_BIRTH, overwrite);
                    SetSpinIfAllowed(spnMiscarriage3, Treatment.NUMBER_OF_MISCARRIAGE, overwrite);
                }

                // Tuần thai ← Số tuần
                SetSpinIfAllowed(spnGestationalWeeks3, baby.WEEK_COUNT, overwrite);

                // Ngày đẻ ← Ngày sinh + Giờ sinh
                if (baby.BORN_TIME.HasValue && baby.BORN_TIME.Value > 0
                    && (overwrite || dteBornTime3.EditValue == null))
                {
                    dteBornTime3.EditValue = ConvertTimeNumberToDate(baby.BORN_TIME.Value);
                }

                // Nơi đẻ: KHÔNG tự lấy — giấy chứng sinh lưu "nơi sinh" (tại viện / CSYT khác / tại nhà...),
                // SKSS lưu "hạng cơ sở" theo QĐ 3412, hai danh mục khác nhau nên khoa tự chọn tay.

                // Địa chỉ nơi đẻ ← Tỉnh / Xã / Địa chỉ nơi sinh trên giấy chứng sinh
                bool hasBabyAddress = !string.IsNullOrWhiteSpace(baby.BIRTH_PROVINCE_CODE)
                    || !string.IsNullOrWhiteSpace(baby.BIRTH_COMMUNE_CODE)
                    || !string.IsNullOrWhiteSpace(baby.BIRTHPLACE);
                if (hasBabyAddress && addressMother != null)
                {
                    var current = addressMother.GetValue();
                    bool hasCurrent = current != null
                        && (!string.IsNullOrWhiteSpace(current.Province_Code)
                            || !string.IsNullOrWhiteSpace(current.Commune_Code)
                            || !string.IsNullOrWhiteSpace(current.Address));
                    if (overwrite || !hasCurrent)
                    {
                        addressMother.SetValue(new UCAddressADO()
                        {
                            Province_Code = baby.BIRTH_PROVINCE_CODE,
                            Province_Name = baby.BIRTH_PROVINCE_NAME,
                            District_Code = baby.BIRTH_DISTRICT_CODE,
                            District_Name = baby.BIRTH_DISTRICT_NAME,
                            Commune_Code = baby.BIRTH_COMMUNE_CODE,
                            Commune_Name = baby.BIRTH_COMMUNE_NAME,
                            Address = baby.BIRTHPLACE
                        });
                    }
                }

                // Mẹ tử vong: chỉ đánh "Có" khi giấy chứng sinh ghi nhận; không tích thì giữ nguyên
                if (baby.IS_MOTHER_DEATH == 1)
                {
                    SetRadioGroupValue("MotherDeath", 1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Chọn mã vào combo khi có nguồn và (ghi đè hoặc combo đang trống).</summary>
        private void SetComboIfAllowed(GridLookUpEdit combo, string code, bool overwrite)
        {
            if (combo == null || string.IsNullOrEmpty(code)) return;
            if (overwrite || string.IsNullOrEmpty(GetComboValue(combo)))
            {
                SetComboValue(combo, code);
            }
        }

        /// <summary>Điền số vào ô số khi có nguồn (khác null, > 0) và (ghi đè hoặc ô đang trống / 0).</summary>
        private void SetSpinIfAllowed(SpinEdit spinEdit, long? value, bool overwrite)
        {
            if (spinEdit == null || !value.HasValue || value.Value <= 0) return;
            if (overwrite || string.IsNullOrEmpty(GetSpinEditStringValue(spinEdit)))
            {
                SetSpinEditStringValue(spinEdit, value.Value.ToString());
            }
        }

        #endregion

        #region Phần Con

        /// <summary>
        /// Lấy thông tin trẻ sơ sinh từ giấy chứng sinh sang mục Sinh đẻ - Con.
        /// Ghi vào bản ghi con rồi đổ lại lên các ô nhập.
        /// </summary>
        private void CopyBabyDataToChildTab(HIS_BABY baby, bool overwrite)
        {
            try
            {
                if (baby == null) return;

                // Đồng bộ dữ liệu người dùng đang nhập vào bản ghi con trước khi ghi đè,
                // để các ô không có nguồn không bị mất khi đổ lại lên màn hình
                if (overwrite) GetDataFromTab3Child();
                if (_child == null) _child = new MCH_CHILD();

                if (CanSet(_child.CHILD_NAME, overwrite)) _child.CHILD_NAME = baby.BABY_NAME;

                // Giới tính: ô chọn theo MÃ giới tính, giấy chứng sinh lưu số định danh → tra danh mục lấy mã
                if (baby.GENDER_ID.HasValue && CanSet(_child.CHILD_GENDER, overwrite))
                {
                    var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == baby.GENDER_ID.Value);
                    if (gender != null && !string.IsNullOrEmpty(gender.GENDER_CODE))
                    {
                        _child.CHILD_GENDER = gender.GENDER_CODE;
                    }
                }

                if (baby.WEIGHT.HasValue && CanSet(_child.WEIGHT, overwrite))
                {
                    _child.WEIGHT = baby.WEIGHT.Value.ToString();
                }

                if (baby.HEIGHT.HasValue && CanSet(_child.HEIGHT, overwrite))
                {
                    _child.HEIGHT = baby.HEIGHT.Value.ToString();
                }

                if (baby.HEAD.HasValue && CanSet(_child.HEAD_CIRCUM, overwrite))
                {
                    _child.HEAD_CIRCUM = baby.HEAD.Value.ToString();
                }

                if (!string.IsNullOrEmpty(baby.MIDWIFE) && CanSet(_child.DELIVERY_ASSISTANT, overwrite))
                    _child.DELIVERY_ASSISTANT = baby.MIDWIFE;
                if (!string.IsNullOrEmpty(baby.ETHNIC_CODE) && CanSet(_child.ETHNIC_CODE, overwrite))
                {
                    _child.ETHNIC_CODE = baby.ETHNIC_CODE;
                    _child.ETHNIC_NAME = baby.ETHNIC_NAME;
                }
                if (!string.IsNullOrEmpty(baby.HEIN_CARD_NUMBER_TMP) && CanSet(_child.TEMPORARY_HEIN_CARD_NUMBER, overwrite))
                    _child.TEMPORARY_HEIN_CARD_NUMBER = baby.HEIN_CARD_NUMBER_TMP;

                if (baby.BIRTH_CERT_NUM.HasValue && CanSet(_child.BIRTH_CERTIFICATE_CODE, overwrite))
                {
                    var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == Treatment.BRANCH_ID);
                    string bornTime = "", birthCert = "";
                    if (baby.ISSUED_DATE != null)
                    {
                        bornTime = baby.ISSUED_DATE.ToString().Substring(2, 2);
                    }
                    if (baby.BIRTH_CERT_NUM != null)
                    {
                        birthCert = baby.BIRTH_CERT_NUM.Value.ToString();
                        if (baby.BIRTH_CERT_NUM.ToString().Length < 5)
                        {
                            birthCert = String.Format("{0:00000}", baby.BIRTH_CERT_NUM);
                        }
                    }
                    _child.BIRTH_CERTIFICATE_CODE = String.Format("{0}.GCS.{1}.{2}", birthCert, branch != null ? branch.HEIN_MEDI_ORG_CODE : null, bornTime);
                }

                if (baby.BORN_TIME.HasValue && (overwrite || !_child.CHILD_BIRTH_DATE.HasValue))
                    _child.CHILD_BIRTH_DATE = baby.BORN_TIME;

                // Tình trạng đẻ ra (Sống / Chết) + Tình trạng con (Sống → Bình thường; Chết → để trống)
                if (baby.BORN_RESULT_ID == BORN_RESULT_ID__SONG)
                {
                    if (CanSet(_child.LIVE_BIRTH, overwrite))
                    {
                        _child.LIVE_BIRTH = "0";
                        _child.IS_DEATH = 0;
                    }
                    if (CanSet(_child.CHILD_STATUS, overwrite))
                    {
                        _child.CHILD_STATUS = MCH_NEWBORN_CONDITION__BINH_THUONG;
                    }
                }
                else if (baby.BORN_RESULT_ID == BORN_RESULT_ID__CHET)
                {
                    if (CanSet(_child.LIVE_BIRTH, overwrite))
                    {
                        _child.LIVE_BIRTH = "1";
                        _child.IS_DEATH = 1;
                    }
                }

                if (baby.IS_INJECT_K1.HasValue && baby.IS_INJECT_K1.Value == 1 && CanSet(_child.VITAMIN_K1, overwrite))
                {
                    _child.VITAMIN_K1 = "1";
                }

                if (baby.IS_INJECT_B.HasValue && baby.IS_INJECT_B.Value == 1 && CanSet(_child.HEPB_VACCINE, overwrite))
                {
                    _child.HEPB_VACCINE = "1";
                }

                bool hasBabyAddress = !string.IsNullOrWhiteSpace(baby.BIRTH_PROVINCE_CODE)
                    || !string.IsNullOrWhiteSpace(baby.BIRTH_COMMUNE_CODE)
                    || !string.IsNullOrWhiteSpace(baby.BIRTHPLACE);
                bool hasChildAddress = !string.IsNullOrWhiteSpace(_child.BIRTH_PROVINCE_CODE)
                    || !string.IsNullOrWhiteSpace(_child.BIRTH_COMMUNE_CODE)
                    || !string.IsNullOrWhiteSpace(_child.BIRTH_ADDRESS);
                if (hasBabyAddress && (overwrite || !hasChildAddress))
                {
                    _child.BIRTH_PROVINCE_CODE = baby.BIRTH_PROVINCE_CODE;
                    _child.BIRTH_PROVINCE_NAME = baby.BIRTH_PROVINCE_NAME;
                    _child.BIRTH_DISTRICT_CODE = baby.BIRTH_DISTRICT_CODE;
                    _child.BIRTH_DISTRICT_NAME = baby.BIRTH_DISTRICT_NAME;
                    _child.BIRTH_COMMUNE_CODE = baby.BIRTH_COMMUNE_CODE;
                    _child.BIRTH_COMMUNE_NAME = baby.BIRTH_COMMUNE_NAME;
                    _child.BIRTH_ADDRESS = baby.BIRTHPLACE;
                }

                if (baby.BIRTH_CERT_NUM.HasValue && baby.BIRTH_CERT_NUM.Value > 0 && CanSet(_child.HAS_BIRTH_CERTIFICATE, overwrite))
                {
                    _child.HAS_BIRTH_CERTIFICATE = "1";
                    _child.BIRTH_CERTIFICATE_DATE = baby.ISSUED_DATE;

                    if (baby.IS_REISSUED.HasValue && baby.IS_REISSUED.Value == 1)
                    {
                        _child.BIRTH_CERTIFICATE_ROUND = "1";
                    }
                    else
                    {
                        _child.BIRTH_CERTIFICATE_ROUND = "0";
                    }
                }

                if (baby.POSTPARTUM_CARE.HasValue)
                {
                    if (baby.POSTPARTUM_CARE.Value == 1 && CanSet(_child.CARE_WEEK_1, overwrite))
                    {
                        _child.CARE_WEEK_1 = "1";
                    }
                    else if (baby.POSTPARTUM_CARE.Value == 2 && CanSet(_child.CARE_WEEK_2_TO_6, overwrite))
                    {
                        _child.CARE_WEEK_2_TO_6 = "1";
                    }
                }

                FillDataToTab3Child();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                if (overwrite) throw;
            }
        }

        /// <summary>Được phép ghi khi ghi đè hoặc giá trị hiện tại đang trống.</summary>
        private static bool CanSet(string current, bool overwrite)
        {
            return overwrite || string.IsNullOrEmpty(current);
        }

        #endregion

        #endregion
    }
}
