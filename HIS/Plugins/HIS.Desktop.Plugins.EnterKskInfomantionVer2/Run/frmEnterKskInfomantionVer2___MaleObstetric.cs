/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Chặn thông tin KHÁM SẢN / SẢN PHỤ KHOA với bệnh nhân giới tính NAM.
 *
 * Lỗi cũ: bệnh nhân Nam vẫn nhập được ô khám sản phụ khoa (và tiền sử sản phụ khoa) -> lưu ra
 * hồ sơ KSK Nam có thông tin khám sản. Xử lý 2 lớp:
 *   1) Giao diện: khi nạp tab, nếu BN là Nam -> XÓA giá trị + KHÓA (ReadOnly) các ô sản phụ khoa.
 *      Dùng ReadOnly chứ không dùng Enabled vì LoginNameEnableControl / SetEnableControl bật-tắt
 *      Enabled theo người khám, sẽ mở lại ô nếu dùng chung cờ Enabled. BN nữ -> trả ReadOnly = false
 *      (đổi y lệnh ở panel trái không bị kẹt khóa của BN trước).
 *   2) Lúc Lưu: xóa trắng mọi cột sản phụ khoa trong SDO gửi đi nếu BN là Nam — chặn cả những đường
 *      đổ dữ liệu bằng code (nhập Excel, thư viện mẫu, lấy từ y lệnh khám, bản ghi cũ đã lỡ lưu sai).
 *
 * Áp dụng MỌI tab có ô sản khoa (định kỳ, trên 18, dưới 18, lái xe định kỳ, lái xe, nghề nghiệp,
 * mẫu HCM). Tab KSK khác / dưới 6 tuổi / tâm thần không có ô sản khoa.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Bệnh nhân của y lệnh đang mở là giới tính Nam.</summary>
        private bool IsMalePatient()
        {
            return this.currentServiceReq != null
                && this.currentServiceReq.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE;
        }

        #region ===== Giao diện =====

        /// <summary>Các ô sản phụ khoa của 1 tab (tab dưới 6 / khác / tâm thần không có).</summary>
        private List<Control> GetObstetricControlsOfTab(int tab)
        {
            List<Control> list = new List<Control>();
            switch (tab)
            {
                case 0: // Ksk định kỳ: khám sản phụ khoa + tiền sử phụ khoa
                    list.AddRange(new Control[]
                    {
                        txtExamObstetric, cboExamObstetricRank, cboExamObstetricLoginName,
                        spnMenarcheAge, chkMenstrualRegular, chkMenstrualIrregular, chkMenstrualYes, chkMenstrualNo,
                        spnCycleFromDay, spnCycleToDay, spnMenstrualDurationFrom, spnMenstrualDurationTo,
                        spnPara1, spnPara2, spnPara3, spnPara4,
                        spnSurgeriesCount, txtObstetricSurgeryNote, chkObstetricSurgeryNone,
                        chkContraceptionYes, chkContraceptionNo, txtContraceptionNote
                    });
                    break;
                case 1: // Trên 18 tuổi
                    list.AddRange(new Control[]
                    {
                        txtMaternityHistory, txtExamObstetric2, cboExamObstetricRank2, cboExamObstetricLoginName2,
                        btnTextLibObstetric2
                    });
                    break;
                case 2: // Dưới 18 tuổi: tiền sử sản khoa + bất thường sản khoa
                    list.AddRange(new Control[] { txtMarternityHistory3, gridLookUpEdit1 });
                    break;
                case 3: // Lái xe định kỳ
                    list.AddRange(new Control[]
                    {
                        txtMaternityHistory4, txtExamMaternity4, txtExamMaternityConclude4,
                        cboExamMaternityRank4, cboExamMaternityLoginName4
                    });
                    break;
                case 4: // Lái xe
                    list.AddRange(new Control[] { txtExamMaternity5, txtExamMaternityConclude5, cboExamMaternityRank5 });
                    break;
                case 6: // Nghề nghiệp
                    list.AddRange(new Control[] { txtExamObstetric7, cboExamObstetricRank7, cboExamObstetricLoginName7 });
                    break;
            }
            return list;
        }

        /// <summary>
        /// Gọi sau khi nạp xong 1 tab (EnsureTabLoaded): BN Nam -> xóa + khóa ô sản phụ khoa của tab;
        /// BN khác -> mở khóa (ReadOnly = false).
        /// </summary>
        private void ApplyMaleObstetricLock(int tab)
        {
            try
            {
                bool isMale = IsMalePatient();
                foreach (Control c in GetObstetricControlsOfTab(tab))
                    LockObstetricControl(c, isMale);

                LockObstetricHistoryIcd(isMale);
                if (tab == TAB_INDEX__OVER_EIGHTEEN)
                    ApplyMaleObstetricLockHcm();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void LockObstetricControl(Control c, bool isMale)
        {
            if (c == null) return;
            CheckEdit chk = c as CheckEdit;
            if (chk != null)
            {
                HookObstetricValueGuard(chk);
                if (isMale) chk.Checked = false;
                chk.Properties.ReadOnly = isMale;
                return;
            }
            BaseEdit be = c as BaseEdit;
            if (be != null)
            {
                HookObstetricValueGuard(be);
                if (isMale) be.EditValue = null;
                be.Properties.ReadOnly = isMale;
                return;
            }
            // Nút thư viện mẫu: chỉ có Enabled, không bị code khác bật lại.
            c.Enabled = !isMale;
        }

        #region ===== Chốt chặn khi CHỌN MẪU =====
        // ReadOnly chỉ chặn người dùng gõ tay. Các đường "chọn mẫu" đổ giá trị bằng code (thư viện mẫu,
        // mẫu JSON Nội khoa, Thiết lập mặc định, Nhập mẫu Excel, tự điền Phân loại PL:Lx, lấy từ y lệnh khám...)
        // vẫn ghi được vào ô ReadOnly -> gắn handler đổi giá trị: BN Nam mà ô sản khoa có giá trị thì trả về trống.

        private readonly HashSet<Control> obstetricGuardedControls = new HashSet<Control>();
        private bool isRevertingObstetricValue;

        private void HookObstetricValueGuard(BaseEdit be)
        {
            if (be == null || obstetricGuardedControls.Contains(be)) return;
            obstetricGuardedControls.Add(be);
            CheckEdit chk = be as CheckEdit;
            if (chk != null)
                chk.CheckedChanged += ObstetricCheckGuard_CheckedChanged;
            else
                be.EditValueChanged += ObstetricValueGuard_EditValueChanged;
        }

        private void ObstetricCheckGuard_CheckedChanged(object sender, EventArgs e)
        {
            if (isRevertingObstetricValue) return;
            try
            {
                CheckEdit chk = sender as CheckEdit;
                if (chk == null || !chk.Checked || !IsMalePatient()) return;
                isRevertingObstetricValue = true;
                chk.Checked = false;
                LogSystem.Debug("EnterKskVer2: BN Nam -> bo tich o san phu khoa " + chk.Name);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { isRevertingObstetricValue = false; }
        }

        private void ObstetricValueGuard_EditValueChanged(object sender, EventArgs e)
        {
            if (isRevertingObstetricValue) return;
            try
            {
                BaseEdit be = sender as BaseEdit;
                if (be == null || !IsMalePatient()) return;
                object v = be.EditValue;
                if (v == null || v == DBNull.Value || string.IsNullOrEmpty(v.ToString())) return;
                isRevertingObstetricValue = true;
                be.EditValue = null;
                LogSystem.Debug("EnterKskVer2: BN Nam -> xoa gia tri mau do vao o san phu khoa " + be.Name);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { isRevertingObstetricValue = false; }
        }

        #endregion

        /// <summary>Cụm ICD tiền sử nhóm Sản khoa (mọi tab): BN Nam -> xóa hiển thị + khóa nút chọn.</summary>
        private void LockObstetricHistoryIcd(bool isMale)
        {
            List<UcKskHistoryIcd> ucs;
            if (!dicHistoryIcdUc.TryGetValue(KskHistoryGroup.Obstetric, out ucs) || ucs == null) return;
            foreach (UcKskHistoryIcd uc in ucs)
            {
                if (uc == null) continue;
                if (isMale) uc.SetData("", "");
                uc.SetReadOnly(isMale);
            }
        }

        /// <summary>
        /// Tab Khám lâm sàng mẫu HCM (trong tab trên 18): mục "Sản khoa" + "Phụ khoa".
        /// Gọi sau khi đổ dữ liệu (FillKskSytHcmClinicalControls) vì ApplyClinicalHcmRowEnabled mở lại ô ICD.
        /// </summary>
        private void ApplyMaleObstetricLockHcm()
        {
            try
            {
                if (!isClinicalExamHcmInited) return;
                bool isMale = IsMalePatient();
                foreach (ClinicalExamHcmRow row in clinicalExamHcmRows)
                {
                    if (row == null) continue;
                    if (row.Key != SYT_HCM_KEY__OBSTETRIC && row.Key != SYT_HCM_KEY__GYNECOLOGY) continue;

                    LockObstetricControl(row.ChkRefuse, isMale);
                    LockObstetricControl(row.ChkNormal, isMale);
                    LockObstetricControl(row.CboRank, isMale);
                    LockObstetricControl(row.CboLoginName, isMale);
                    if (isMale)
                    {
                        SetHcmIcdValue(row.UcPreIcd, null, null);
                        SetHcmIcdValue(row.UcFinalIcd, null, null);
                        SetHcmIcdEnabled(row.UcPreIcd, false);
                        SetHcmIcdEnabled(row.UcFinalIcd, false);
                    }
                    else
                    {
                        ApplyClinicalHcmRowEnabled(row);
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Lúc Lưu =====

        /// <summary>BN Nam -> xóa trắng mọi cột sản phụ khoa trong dữ liệu sắp gửi.</summary>
        private void ClearObstetricDataForMale(HisServiceReqKskExecuteV2SDO sdo)
        {
            try
            {
                if (sdo == null || !IsMalePatient()) return;
                int tab = xtraTabControl1.SelectedTabPageIndex;

                if (sdo.KskGeneral != null) ClearObstetricGeneral(sdo.KskGeneral.HisKskGeneral);

                if (sdo.KskUnderEighteen != null && sdo.KskUnderEighteen.HisKskUnderEighteen != null)
                {
                    sdo.KskUnderEighteen.HisKskUnderEighteen.MATERNITY_HISTORY = null;
                    sdo.KskUnderEighteen.HisKskUnderEighteen.OBSTETRIC_ABNORMAL_CODES = null;
                }

                if (sdo.KskOverEighteen != null && sdo.KskOverEighteen.HisKskOverEighteen != null)
                {
                    HIS_KSK_OVER_EIGHTEEN o = sdo.KskOverEighteen.HisKskOverEighteen;
                    o.MATERNITY_HISTORY = null;
                    o.EXAM_OBSTETRIC = null;
                    o.EXAM_OBSTETRIC_RANK = null;
                    o.EXAM_OBSTETRIC_LOGINNAME = null;
                }
                if (sdo.KskPeriodDriver != null && sdo.KskPeriodDriver.HisKskPeriodDriver != null)
                {
                    HIS_KSK_PERIOD_DRIVER o = sdo.KskPeriodDriver.HisKskPeriodDriver;
                    o.MATERNITY_HISTORY = null;
                    o.EXAM_MATERNITY = null;
                    o.EXAM_MATERNITY_CONCLUDE = null;
                    o.EXAM_MATERNITY_RANK = null;
                    o.EXAM_MATERNITY_LOGINNAME = null;
                }
                if (sdo.HisKskDriverCar != null)
                {
                    sdo.HisKskDriverCar.EXAM_MATERNITY = null;
                    sdo.HisKskDriverCar.EXAM_MATERNITY_CONCLUDE = null;
                    sdo.HisKskDriverCar.EXAM_MATERNITY_RANK = null;
                    sdo.HisKskDriverCar.EXAM_MATERNITY_LOGINNAME = null;
                }
                if (sdo.KskOccupationalV2 != null && sdo.KskOccupationalV2.HisKskOccupational != null)
                {
                    HIS_KSK_OCCUPATIONAL o = sdo.KskOccupationalV2.HisKskOccupational;
                    o.EXAM_OBSTETRIC = null;
                    o.EXAM_OBSTETRIC_RANK = null;
                    o.EXAM_OBSTETRIC_LOGINNAME = null;
                }
                LogSystem.Info("EnterKskVer2: BN Nam -> xoa thong tin kham san phu khoa truoc khi luu. ServiceReqId="
                    + sdo.ServiceReqId + ", tab=" + tab);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void ClearObstetricGeneral(HIS_KSK_GENERAL g)
        {
            if (g == null) return;
            g.EXAM_OBSTETRIC = null;
            g.EXAM_OBSTETRIC_RANK = null;
            g.EXAM_OBSTETRIC_LOGINNAME = null;
            g.MENSTRUAL_STAR_AGE = null;
            g.MENSTRUAL_NATURE = null;
            g.MENSTRUAL_ABDOMINAL_PAINS = null;
            g.MENSTRUAL_CYCLE_FROM = null;
            g.MENSTRUAL_CYCLE_TO = null;
            g.MENSTRUAL_AMOUNT_FROM = null;
            g.MENSTRUAL_AMOUNT_TO = null;
            g.PREGNANCY = null;
            g.ABORTUS = null;
            g.RECURRENT = null;
            g.ALIVE = null;
            g.NUMBER_OF_SURGERIES = null;
            g.NOTE_SURGICAL = null;
            if (g.IS_NOT_SURGERY.HasValue) g.IS_NOT_SURGERY = 0;
            g.IS_USING_CONTRACEPTIVES = null;
            g.NOTE_CONTRACEPTIVES = null;
            g.OBSTETRIC_DISEASE_ICD_CODE = null;
            g.OBSTETRIC_DISEASE_ICD_NAME = null;
            dicHistoryIcdValue.Remove(KskHistoryGroup.Obstetric);
        }

        /// <summary>Bảng mẫu M4 (HCM): BN Nam -> xóa mục Sản khoa + Phụ khoa.</summary>
        private void ClearObstetricSytHcmForMale(HIS_KSK_SYT_HCM d)
        {
            if (d == null || !IsMalePatient()) return;
            d.IS_REFUSE_OBSTETRIC = null;
            d.IS_REFUSE_GYNECOLOGY = null;
            d.EXAM_OBSTETRIC_IS_NORMAL = null;
            d.EXAM_OBSTETRIC_PRE_ICD_CODE = null;
            d.EXAM_OBSTETRIC_PRE_ICD_NAME = null;
            d.EXAM_OBSTETRIC_ICD_CODE = null;
            d.EXAM_OBSTETRIC_ICD_NAME = null;
            d.EXAM_GYNECOLOGY_IS_NORMAL = null;
            d.EXAM_GYNECOLOGY_PRE_ICD_CODE = null;
            d.EXAM_GYNECOLOGY_PRE_ICD_NAME = null;
            d.EXAM_GYNECOLOGY_ICD_CODE = null;
            d.EXAM_GYNECOLOGY_ICD_NAME = null;
            d.EXAM_GYNECOLOGY_RANK = null;
            d.EXAM_GYNECOLOGY_LOGINNAME = null;
        }

        #endregion
    }
}
