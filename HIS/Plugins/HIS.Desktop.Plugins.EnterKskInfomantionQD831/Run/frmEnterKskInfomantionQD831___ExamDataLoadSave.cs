/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Load/Save qua API mới:
 *  - GET  /api/HisKskProfile/GetFull  (input MOS.Filter.HisKskProfileFilter, output HisKskProfileFullSDO) -> nạp form.
 *  - POST /api/HisKskProfile/SaveExam (input/output HisKskProfileExamSDO)                            -> ghi A+B+C+D (profile+khám+CLS+tiêm chủng+quan hệ).
 */
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    public partial class frmEnterKskInfomantionQD831
    {
        private HisKskProfileFullSDO _loadedFull; // giữ lại để nạp các tab lazy khi mở
        private List<HIS_HEALTH_VACCINATION> _loadedVaccs; // tiêm chủng đã lưu (ưu tiên exam-level, fallback top-level)
        private long _loadedKskServiceReqId = 0;  // y lệnh đang nạp dữ liệu KSK (tránh nạp lại đè chỉnh sửa)

        // ==================== CLEAR (xóa toàn bộ dữ liệu nhập trên form) ====================
        /// <summary>Xóa sạch mọi control nhập của KSK (giữ header bệnh nhân + panel y lệnh). Gọi lúc mở form + khi đổi y lệnh.</summary>
        private void ClearAllKskControls()
        {
            try
            {
                _loadedFull = null;
                this.familyRelations = null;
                ClearEdits(
                    this.txtLyDoKham,
                    this.txtHouseholdCode, this.txtLandlinePhone, this.txtTcSoMuiUonVan,
                    this.memoTsCanNangLucDe, this.memoTsChieuDaiLucDe, this.memoTsDiTatBamSinh, this.memoTsVanDeSinh,
                    this.memoTsSoLyRuou, this.memoTsTiepXucNgheNghiep, this.memoTsThoiGianTiepXuc, this.cboTsHoXi, this.memoTsNguyCoKhac,
                    this.memoTsKyCoThaiCuoiCung, this.memoTsSoLanPhaThai, this.memoTsSoLanDeDuThang, this.memoTsSoLanDeNon,
                    this.memoTsDeThuong, this.memoTsDeMo, this.memoTsDeKho, this.memoTsBenhPhuKhoa, this.memoTsVanDeKhacTong,
                    this.memoKlsVanDong, this.memoKlsDanhGiaPhatTrien,
                    this.memoKlsBenhSu, this.txtKlsKhongKinhMP, this.txtKlsKhongKinhMT, this.txtKlsCoKinhMP, this.txtKlsCoKinhMT,
                    this.memoKlsDaNiemMac, this.memoKlsToanThanKhac, this.memoKlsTimMach, this.memoKlsHoHap, this.memoKlsTieuHoa,
                    this.memoKlsTietNieu, this.memoKlsCoXuongKhop, this.memoKlsThanKinh, this.memoKlsTamThan, this.memoKlsNgoaiKhoa,
                    this.memoKlsSanPhuKhoa, this.memoKlsTaiMuiHong, this.memoKlsRangHamMat, this.memoKlsMat, this.memoKlsDaLieu,
                    this.memoKlsDinhDuong, this.memoKlsNoiTiet, this.memoKlsKhac,
                    this.memoClsHuyetHoc, this.memoClsSinhHoaMau, this.memoClsSinhHoaNuocTieu, this.memoClsSieuAmOB, this.memoClsTuVan,
                    this.memoTsPhauThuat, this.memoTsBienPhapTranhThai,
                    this.txtKlsMach, this.txtKlsNhietDo, this.txtKlsHa, this.txtKlsNhipTho, this.txtKlsCanNang, this.txtKlsCao, this.txtKlsBmi, this.txtKlsVongBung,
                    this.memoTsSoLanCoThai, this.memoTsSoLanSayThai, this.memoTsSoLanSinhDe, this.memoTsSoConHienSong);
                ClearChecks(this.chkTsDeThuong, this.chkTsDeMo, this.chkTsDeThieuThang, this.chkTsBiNgat,
                    this.chkTsHutThuongXuyen, this.chkTsHutDaBo, this.chkTsRuouDaBo,
                    this.chkTsMaTuyThuongXuyen, this.chkTsMaTuyDaBo, this.chkTsTheLucThuongXuyen);
                ClearRadios(this.rdoTsHutThuoc, this.rdoTsRuouBia, this.rdoTsMaTuy, this.rdoTsTheLuc);

                if (this.cboClsBacSiKham != null) this.cboClsBacSiKham.EditValue = null;
                if (this.subIcdProcessorCls != null && this.ucClsSecondaryIcd != null)
                    this.subIcdProcessorCls.Reload(this.ucClsSecondaryIcd, new SecondaryIcdDataADO { ICD_SUB_CODE = "", ICD_TEXT = "" });

                ClearDiseaseGridsAndDynamic();
                ClearVaccineGrids();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private static void ClearEdits(params BaseEdit[] edits)
        {
            foreach (var e in edits) if (e != null) e.EditValue = null;
        }
        private static void ClearChecks(params CheckEdit[] cs) { foreach (var c in cs) if (c != null) c.Checked = false; }
        private static void ClearRadios(params RadioGroup[] rs) { foreach (var r in rs) if (r != null) r.EditValue = null; }

        private void ClearDiseaseGridsAndDynamic()
        {
            if (this.benhTatCheckMap != null) foreach (var kv in this.benhTatCheckMap) if (kv.Value != null) kv.Value.Checked = false;
            if (this.benhTatTextMap != null) foreach (var kv in this.benhTatTextMap) if (kv.Value != null) kv.Value.Text = "";
            if (this.diseaseGridRows != null)
                foreach (var rows in this.diseaseGridRows.Values)
                {
                    if (rows == null) continue;
                    foreach (var row in rows) { if (row.IsCheckbox) row.Chon = false; row.MoTa = ""; row.NguoiMac = ""; }
                }
            RefreshGrid(this.gcTsDiUngBanThan); RefreshGrid(this.gcTsKhuyetTat);
            RefreshGrid(this.gcTsDiUngGiaDinh); RefreshGrid(this.gcTsBenhTatGiaDinh);
        }

        private void ClearVaccineGrids()
        {
            var r1 = this.gcTcVaccine1 != null ? this.gcTcVaccine1.DataSource as BindingList<TcVaccineRow> : null;
            var r2 = this.gcTcVaccine2 != null ? this.gcTcVaccine2.DataSource as BindingList<TcVaccineRow> : null;
            foreach (var rows in new[] { r1, r2 })
                if (rows != null) foreach (var row in rows) { row.IsNotVaccinated = false; row.VaccinatedTime = null; row.Reaction = ""; row.AppointmentTime = null; }
            var r3 = this.gcTcVaccine3 != null ? this.gcTcVaccine3.DataSource as BindingList<TcContentRow> : null;
            if (r3 != null) foreach (var row in r3) { row.IsNotVaccinated = false; row.PregnancyMonth = ""; row.Reaction = ""; row.AppointmentTime = null; }
            RefreshGrid(this.gcTcVaccine1); RefreshGrid(this.gcTcVaccine2); RefreshGrid(this.gcTcVaccine3);
        }

        // ==================== SAVE (POST /api/HisKskProfile/SaveExam) ====================
        private HisKskProfileExamSDO SaveExamToApi(HisKskProfileExamSDO exam, CommonParam param)
        {
            try
            {
                if (exam == null) return null;
                // Log FULL dữ liệu đầu vào trước khi POST (toàn bộ SDO -> JSON).
                LogSystem.Debug("KskSave.SaveExam INPUT DATA: " + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => exam), exam));
                var result = new BackendAdapter(param).Post<HisKskProfileExamSDO>(
                    "api/HisKskProfile/SaveExam", ApiConsumers.MosConsumer, exam, param);
                if (result == null) LogSystem.Warn("SaveExam trả null (xem param/log backend).");
                return result;
            }
            catch (Exception ex) { LogSystem.Error(ex); return null; }
        }

        // ==================== LOAD (GET /api/HisKskProfile/GetFull) ====================
        /// <summary>Nạp hồ sơ đã lưu theo y lệnh hiện tại rồi đổ lên form (phần sẵn sàng; tab lazy nạp khi mở).</summary>
        private void LoadKskProfileFull()
        {
            try
            {
                if (currentServiceReq == null) return;
                var filter = new MOS.Filter.HisKskProfileFilter { SERVICE_REQ_ID = currentServiceReq.ID };
                var param = new CommonParam();
                LogSystem.Debug("KskLoad.GetFull INPUT SERVICE_REQ_ID=" + currentServiceReq.ID);
                _loadedFull = new BackendAdapter(param).Get<HisKskProfileFullSDO>(
                    "api/HisKskProfile/GetFull", ApiConsumers.MosConsumer, filter, param);
                if (_loadedFull == null) { LogSystem.Debug("KskLoad.GetFull: _loadedFull = NULL (BE không trả dữ liệu)"); return; }
                // Log FULL output GetFull (toàn bộ SDO trả về -> JSON).
                LogSystem.Debug("KskLoad.GetFull OUTPUT DATA: " + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => _loadedFull), _loadedFull));

                var exam = _loadedFull.ExamHistory != null ? _loadedFull.ExamHistory.FirstOrDefault() : null;

                // Hồ sơ (Part A): ưu tiên PatientInfo.Profiles, fallback exam.HisKskProfile (BE có thể trả ở exam-level theo SDO mới).
                var profile = _loadedFull.PatientInfo != null && _loadedFull.PatientInfo.Profiles != null
                    ? _loadedFull.PatientInfo.Profiles.FirstOrDefault() : null;
                if (profile == null && exam != null) profile = exam.HisKskProfile;

                LogSystem.Debug(string.Format(
                    "KskLoad.GetFull RESULT: profile={0}, examHistory={1}, relations(full)={2}, relations(exam)={3}, vaccination(full)={4}",
                    profile != null ? "CÓ" : "null",
                    _loadedFull.ExamHistory != null ? _loadedFull.ExamHistory.Count : 0,
                    _loadedFull.Relations != null ? _loadedFull.Relations.Count : 0,
                    exam != null && exam.HisKskRelations != null ? exam.HisKskRelations.Count : 0,
                    _loadedFull.Vaccination != null ? _loadedFull.Vaccination.Count : 0));

                if (profile != null) PopulateProfile(profile);

                // Danh sách quan hệ người nhà: ưu tiên top-level Relations, fallback exam.HisKskRelations (mở popup sẽ hiển thị lại).
                var rels = (_loadedFull.Relations != null && _loadedFull.Relations.Count > 0)
                    ? _loadedFull.Relations
                    : (exam != null ? exam.HisKskRelations : null);
                this.familyRelations = rels != null
                    ? rels.Where(r => r != null).ToList()
                    : new List<HIS_KSK_RELATION>();

                // Lý do khám: nếu profile không trả INTRUCTION_NOTE thì lấy từ exam.IntructionNote.
                if ((profile == null || string.IsNullOrEmpty(profile.INTRUCTION_NOTE))
                    && exam != null && !string.IsNullOrEmpty(exam.IntructionNote))
                    SetText(this.txtLyDoKham, exam.IntructionNote);

                // Tiêm chủng: ưu tiên exam-level, fallback top-level; chọn list KHÔNG rỗng (tránh [] chặn fallback).
                _loadedVaccs = (exam != null && exam.HisHealthVaccinations != null && exam.HisHealthVaccinations.Count > 0)
                    ? exam.HisHealthVaccinations
                    : (_loadedFull.Vaccination != null ? _loadedFull.Vaccination : new List<HIS_HEALTH_VACCINATION>());

                if (exam != null)
                {
                    PopulateGeneralReady(exam.HisKskGeneral);           // Khám LS + CLS memo + mục5 + PARA (control sẵn có)
                    PopulateDhst(exam.HisDhst);                          // sinh tồn
                    PopulateDiseaseResults(exam.HisDiseaseDetailResults); // checklist mục 3/4/6 + bệnh tật động (tab Tiền sử default)
                    if (_khamClsLoaded) PopulateClsExtras(exam.HisKskGeneral);
                }
                // Tiêm chủng nạp độc lập (kể cả khi ExamHistory rỗng nhưng có Vaccination top-level).
                if (_tiemChungLoaded) PopulateVaccinations(_loadedVaccs);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Gọi từ SelectedPageChanged sau khi Init tab lazy: đổ dữ liệu đã load cho tab đó.</summary>
        private void PopulateLazyTab(DevExpress.XtraTab.XtraTabPage page)
        {
            try
            {
                if (_loadedFull == null) return;
                var exam = _loadedFull.ExamHistory != null ? _loadedFull.ExamHistory.FirstOrDefault() : null;
                if (page == this.xtraTabPageKhamCanLamSang)
                {
                    if (exam != null) PopulateClsExtras(exam.HisKskGeneral);
                }
                else if (page == this.xtraTabPageTiemChung)
                {
                    PopulateVaccinations(_loadedVaccs); // độc lập exam
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        // ==================== POPULATE ====================
        private void PopulateProfile(HIS_KSK_PROFILE p)
        {
            if (p == null) return;
            SetText(this.txtHouseholdCode, p.HOUSEHOLD_CODE);
            SetText(this.txtLandlinePhone, p.PHONE_FIXED);
            SetVal(this.txtTcSoMuiUonVan, p.MOTHER_TETANUS_DOSE);
            if (!string.IsNullOrEmpty(p.INTRUCTION_NOTE)) SetText(this.txtLyDoKham, p.INTRUCTION_NOTE); // Lý do khám đã lưu (đè mặc định từ y lệnh)
            // Mục 1
            SetChk(this.chkTsDeThuong, p.BIRTH_NORMAL_DELIVERY);
            SetChk(this.chkTsDeMo, p.BIRTH_CESAREAN);
            SetChk(this.chkTsDeThieuThang, p.BIRTH_PRETERM);
            SetChk(this.chkTsBiNgat, p.BIRTH_ASPHYXIA);
            SetText(this.memoTsCanNangLucDe, p.BIRTH_WEIGHT);
            SetText(this.memoTsChieuDaiLucDe, p.BIRTH_LENGTH);
            SetText(this.memoTsDiTatBamSinh, p.CONGENITAL_DEFECT);
            SetText(this.memoTsVanDeSinh, p.BIRTH_PROBLEM);
            // Mục 2
            SetRadio(this.rdoTsHutThuoc, p.SMOKING);
            SetChk(this.chkTsHutThuongXuyen, p.SMOKING_REGULAR);
            SetChk(this.chkTsHutDaBo, p.SMOKING_QUIT);
            SetRadio(this.rdoTsRuouBia, p.ALCOHOL);
            SetText(this.memoTsSoLyRuou, p.ALCOHOL_GLASS_NUM != null ? p.ALCOHOL_GLASS_NUM.ToString() : null);
            SetChk(this.chkTsRuouDaBo, p.ALCOHOL_QUIT);
            SetRadio(this.rdoTsMaTuy, p.DRUG_USE);
            SetChk(this.chkTsMaTuyThuongXuyen, p.DRUG_REGULAR);
            SetChk(this.chkTsMaTuyDaBo, p.DRUG_QUIT);
            SetRadio(this.rdoTsTheLuc, p.PHYSICAL_ACTIVITY);
            SetChk(this.chkTsTheLucThuongXuyen, p.PHYSICAL_ACTIVITY_REGULAR);
            SetText(this.memoTsTiepXucNgheNghiep, p.OCCUPATIONAL_EXPOSURE);
            SetText(this.memoTsThoiGianTiepXuc, p.EXPOSURE_DURATION);
            SetVal(this.cboTsHoXi, p.TOILET_TYPE);
            SetText(this.memoTsNguyCoKhac, p.OTHER_RISK_FACTOR);
            // Mục 7 (phần PROFILE)
            SetText(this.memoTsKyCoThaiCuoiCung, p.LAST_PREGNANCY_PERIOD);
            SetVal(this.memoTsSoLanPhaThai, p.ABORTION_NUM);
            SetVal(this.memoTsSoLanDeDuThang, p.FULL_TERM_BIRTH_NUM);
            SetVal(this.memoTsSoLanDeNon, p.PRETERM_BIRTH_NUM);
            SetText(this.memoTsDeThuong, p.NORMAL_DELIVERY);
            SetText(this.memoTsDeMo, p.CESAREAN_DELIVERY);
            SetText(this.memoTsDeKho, p.DIFFICULT_DELIVERY);
            SetText(this.memoTsBenhPhuKhoa, p.GYNECOLOGICAL_DISEASE);
            // Mục 8 + Khám LS (cột PROFILE)
            SetText(this.memoTsVanDeKhacTong, p.OTHER_ISSUE);
            SetText(this.memoKlsVanDong, p.CLINICAL_MOTOR);
            SetText(this.memoKlsDanhGiaPhatTrien, p.CLINICAL_DEVELOPMENT_ASSESS);
        }

        /// <summary>General — phần control SẴN CÓ lúc Load (Khám LS memo/thị lực/toàn thân/cơ quan + CLS memo + mục5 + PARA).</summary>
        private void PopulateGeneralReady(HIS_KSK_GENERAL g)
        {
            if (g == null) return;
            SetText(this.memoKlsBenhSu, g.PATHOLOGICAL_HISTORY);
            SetText(this.txtKlsKhongKinhMP, g.EXAM_EYESIGHT_RIGHT);
            SetText(this.txtKlsKhongKinhMT, g.EXAM_EYESIGHT_LEFT);
            SetText(this.txtKlsCoKinhMP, g.EXAM_EYESIGHT_GLASS_RIGHT);
            SetText(this.txtKlsCoKinhMT, g.EXAM_EYESIGHT_GLASS_LEFT);
            SetText(this.memoKlsDaNiemMac, g.BODY_SKIN);
            SetText(this.memoKlsToanThanKhac, g.BODY_OTHER);
            SetText(this.memoKlsTimMach, g.EXAM_CIRCULATION);
            SetText(this.memoKlsHoHap, g.EXAM_RESPIRATORY);
            SetText(this.memoKlsTieuHoa, g.EXAM_DIGESTION);
            SetText(this.memoKlsTietNieu, g.EXAM_KIDNEY_UROLOGY);
            SetText(this.memoKlsCoXuongKhop, g.EXAM_MUSCLE_BONE);
            SetText(this.memoKlsThanKinh, g.EXAM_NEUROLOGICAL);
            SetText(this.memoKlsTamThan, g.EXAM_MENTAL);
            SetText(this.memoKlsNgoaiKhoa, g.EXAM_SURGERY);
            SetText(this.memoKlsSanPhuKhoa, g.EXAM_OBSTETRIC);
            SetText(this.memoKlsTaiMuiHong, g.EXAM_ENT);
            SetText(this.memoKlsRangHamMat, g.EXAM_STOMATOLOGY);
            SetText(this.memoKlsMat, g.EXAM_EYE);
            SetText(this.memoKlsDaLieu, g.EXAM_DERMATOLOGY);
            SetText(this.memoKlsDinhDuong, g.EXAM_NUTRION);
            SetText(this.memoKlsNoiTiet, g.EXAM_OEND);
            SetText(this.memoKlsKhac, g.EXAM_OTHER);
            // CLS memo
            SetText(this.memoClsHuyetHoc, g.NOTE_BLOOD);
            SetText(this.memoClsSinhHoaMau, g.NOTE_BIOCHEMICAL);
            SetText(this.memoClsSinhHoaNuocTieu, g.NOTE_TEST_URINE);
            SetText(this.memoClsSieuAmOB, g.NOTE_SUPERSONIC);
            SetText(this.memoClsTuVan, g.TREATMENT_INSTRUCTION);
            // Mục 5 (phẫu thuật) + Mục 7 PARA + tránh thai
            SetText(this.memoTsPhauThuat, g.HISTORY_SURGERY);
            SetVal(this.memoTsSoLanCoThai, g.PREGNANCY);
            SetVal(this.memoTsSoLanSayThai, g.ABORTUS);
            SetVal(this.memoTsSoLanSinhDe, g.RECURRENT);
            SetVal(this.memoTsSoConHienSong, g.ALIVE);
            SetText(this.memoTsBienPhapTranhThai, g.NOTE_CONTRACEPTIVES);
        }

        /// <summary>ICD kết luận + Bác sĩ khám (chỉ có sau khi tab Khám CLS đã Init).</summary>
        private void PopulateClsExtras(HIS_KSK_GENERAL g)
        {
            if (g == null) return;
            if (this.cboClsBacSiKham != null) this.cboClsBacSiKham.EditValue = g.CONCLUDER_LOGINNAME;
            if (this.subIcdProcessorCls != null && this.ucClsSecondaryIcd != null
                && (!string.IsNullOrEmpty(g.CONCLUSION_ICD_CODE) || !string.IsNullOrEmpty(g.CONCLUSION_ICD_NAME)))
            {
                this.subIcdProcessorCls.Reload(this.ucClsSecondaryIcd,
                    new SecondaryIcdDataADO { ICD_SUB_CODE = g.CONCLUSION_ICD_CODE, ICD_TEXT = g.CONCLUSION_ICD_NAME });
            }
        }

        private void PopulateDhst(HIS_DHST d)
        {
            if (d == null) return;
            SetVal(this.txtKlsMach, d.PULSE);
            SetVal(this.txtKlsNhietDo, d.TEMPERATURE);
            string bp = (d.BLOOD_PRESSURE_MAX != null ? d.BLOOD_PRESSURE_MAX.ToString() : "")
                + (d.BLOOD_PRESSURE_MIN != null ? "/" + d.BLOOD_PRESSURE_MIN.ToString() : "");
            SetText(this.txtKlsHa, bp);
            SetVal(this.txtKlsNhipTho, d.BREATH_RATE);
            SetVal(this.txtKlsCanNang, d.WEIGHT);
            SetVal(this.txtKlsCao, d.HEIGHT);
            SetVal(this.txtKlsBmi, d.VIR_BMI);
            SetVal(this.txtKlsVongBung, d.BELLY);
        }

        private void PopulateDiseaseResults(List<HIS_DISEASE_DETAIL_RESULT> results)
        {
            if (results == null || results.Count == 0) return;
            var map = new Dictionary<long, HIS_DISEASE_DETAIL_RESULT>();
            foreach (var r in results) if (r != null && r.DISEASE_DETAIL_ID != null) map[r.DISEASE_DETAIL_ID.Value] = r;

            // Bệnh tật động (49)
            if (this.benhTatCheckMap != null)
                foreach (var kv in this.benhTatCheckMap)
                    if (map.ContainsKey(kv.Key) && kv.Value != null) kv.Value.Checked = (map[kv.Key].IS_CHECK == 1);
            if (this.benhTatTextMap != null)
                foreach (var kv in this.benhTatTextMap)
                    if (map.ContainsKey(kv.Key) && kv.Value != null) kv.Value.Text = map[kv.Key].OTHER ?? "";

            // 4 grid (50/53/52/51)
            if (this.diseaseGridRows != null)
                foreach (var rows in this.diseaseGridRows.Values)
                {
                    if (rows == null) continue;
                    foreach (var row in rows)
                    {
                        if (!map.ContainsKey(row.DetailId)) continue;
                        var r = map[row.DetailId];
                        if (row.IsCheckbox) row.Chon = (r.IS_CHECK == 1);
                        string mota, nguoiMac;
                        SplitOther(r.OTHER, out mota, out nguoiMac);
                        row.MoTa = mota;
                        if (!string.IsNullOrEmpty(nguoiMac)) row.NguoiMac = nguoiMac;
                    }
                }
            RefreshGrid(this.gcTsDiUngBanThan); RefreshGrid(this.gcTsKhuyetTat);
            RefreshGrid(this.gcTsDiUngGiaDinh); RefreshGrid(this.gcTsBenhTatGiaDinh);
        }

        private void PopulateVaccinations(List<HIS_HEALTH_VACCINATION> vaccs)
        {
            LogSystem.Debug("KskLoad.PopulateVaccinations: vaccs=" + (vaccs != null ? vaccs.Count : 0)
                + ", tiemChungLoaded=" + _tiemChungLoaded
                + ", grid1Rows=" + GridRowCount(this.gcTcVaccine1)
                + ", grid2Rows=" + GridRowCount(this.gcTcVaccine2)
                + ", grid3Rows=" + GridRowCount(this.gcTcVaccine3));
            if (vaccs == null || vaccs.Count == 0) return;
            FillVaccineGrid(this.gcTcVaccine1, vaccs);
            FillVaccineGrid(this.gcTcVaccine2, vaccs);
            FillContentGrid(this.gcTcVaccine3, vaccs);
        }

        private static int GridRowCount(GridControl grid)
        {
            try
            {
                if (grid == null || grid.DataSource == null) return -1;
                var l = grid.DataSource as System.Collections.IList;
                return l != null ? l.Count : -1;
            }
            catch { return -1; }
        }

        private void FillVaccineGrid(GridControl grid, List<HIS_HEALTH_VACCINATION> vaccs)
        {
            var rows = grid != null ? grid.DataSource as BindingList<TcVaccineRow> : null;
            if (rows == null) return;
            foreach (var row in rows)
            {
                var v = vaccs.FirstOrDefault(x => MatchVaccine(x, row.VaccineCode, row.VaccineName, row.VaccineGroup));
                if (v == null) continue;
                row.IsNotVaccinated = (v.IS_NOT_VACCINATED == 1);
                row.VaccinatedTime = NumToDate(v.VACCINATED_TIME);
                row.Reaction = v.REACTION;
                row.AppointmentTime = NumToDate(v.APPOINTMENT_TIME);
            }
            RefreshGrid(grid);
        }

        private void FillContentGrid(GridControl grid, List<HIS_HEALTH_VACCINATION> vaccs)
        {
            var rows = grid != null ? grid.DataSource as BindingList<TcContentRow> : null;
            if (rows == null) return;
            foreach (var row in rows)
            {
                var v = vaccs.FirstOrDefault(x => MatchVaccine(x, row.VaccineCode, row.VaccineName, row.VaccineGroup));
                if (v == null) continue;
                row.IsNotVaccinated = (v.IS_NOT_VACCINATED == 1);
                row.PregnancyMonth = v.PREGNANCY_MONTH != null ? v.PREGNANCY_MONTH.ToString() : null;
                row.Reaction = v.REACTION;
                row.AppointmentTime = NumToDate(v.APPOINTMENT_TIME);
            }
            RefreshGrid(grid);
        }

        private static bool MatchVaccine(HIS_HEALTH_VACCINATION v, string code, string name, short group)
        {
            if (v == null) return false;
            if ((v.VACCINE_GROUP ?? -1) != group) return false;
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(v.VACCINE_CODE))
                return string.Equals(code.Trim(), v.VACCINE_CODE.Trim(), StringComparison.OrdinalIgnoreCase);
            return string.Equals((name ?? "").Trim(), (v.VACCINE_NAME ?? "").Trim(), StringComparison.OrdinalIgnoreCase);
        }

        // ==================== BMI tự động tính (không cho nhập) — giống EnterKskV2 ====================
        /// <summary>Khóa ô BMI + gắn tự tính khi Cân nặng / Chiều cao thay đổi.</summary>
        private void InitBmiAutoCalc()
        {
            try
            {
                if (this.txtKlsBmi != null)
                {
                    this.txtKlsBmi.Properties.ReadOnly = true;             // không cho nhập
                    this.txtKlsBmi.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    this.txtKlsBmi.TabStop = false;
                }
                if (this.txtKlsCanNang != null)
                {
                    this.txtKlsCanNang.EditValueChanged -= RecalcBmi;
                    this.txtKlsCanNang.EditValueChanged += RecalcBmi;
                }
                if (this.txtKlsCao != null)
                {
                    this.txtKlsCao.EditValueChanged -= RecalcBmi;
                    this.txtKlsCao.EditValueChanged += RecalcBmi;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>BMI = cân nặng(kg) / (chiều cao(m))². Chiều cao nhập theo cm.</summary>
        private void RecalcBmi(object sender, EventArgs e)
        {
            try
            {
                if (this.txtKlsBmi == null) return;
                decimal h = SpinDecimal(this.txtKlsCao);
                decimal w = SpinDecimal(this.txtKlsCanNang);
                decimal bmi = 0;
                if (h > 0) { decimal hm = h / 100m; bmi = w / (hm * hm); }
                this.txtKlsBmi.EditValue = (bmi > 0) ? (object)Math.Round(bmi, 2) : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static decimal SpinDecimal(SpinEdit s)
        {
            if (s == null || s.EditValue == null || s.EditValue is System.DBNull) return 0;
            try { return Convert.ToDecimal(s.Value); } catch { return 0; }
        }

        // ---- setters/helpers ----
        private static void SetText(BaseEdit e, string v) { if (e != null) e.Text = v ?? ""; }
        private static void SetVal(BaseEdit e, object v) { if (e != null) e.EditValue = v; }
        private static void SetChk(CheckEdit c, short? v) { if (c != null) c.Checked = (v == 1); }
        private static void SetRadio(RadioGroup r, short? v) { if (r != null) r.EditValue = v.HasValue ? (object)(int)v.Value : null; }

        private static DateTime? NumToDate(long? num)
        {
            if (num == null || num.Value <= 0) return null;
            try
            {
                string s = num.Value.ToString();
                if (s.Length < 8) return null;
                return new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)), int.Parse(s.Substring(6, 2)));
            }
            catch { return null; }
        }

        private static void SplitOther(string other, out string mota, out string nguoiMac)
        {
            mota = other; nguoiMac = null;
            if (string.IsNullOrEmpty(other)) { mota = null; return; }
            int idx = other.IndexOf("Người mắc:", StringComparison.Ordinal);
            if (idx >= 0)
            {
                nguoiMac = other.Substring(idx + "Người mắc:".Length).Trim();
                mota = other.Substring(0, idx).TrimEnd(' ', '|').Trim();
            }
        }

        private static void RefreshGrid(GridControl grid)
        {
            try { var gv = grid != null ? grid.MainView as GridView : null; if (gv != null) gv.RefreshData(); }
            catch { }
        }
    }
}
