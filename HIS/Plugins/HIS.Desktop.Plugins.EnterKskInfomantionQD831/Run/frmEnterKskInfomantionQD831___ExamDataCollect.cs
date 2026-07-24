/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using DevExpress.XtraEditors;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    /// <summary>
    /// Thu thập dữ liệu để LƯU (theo QĐ 831 — Phụ lục A), chia theo từng phần cho dễ kiểm tra/sửa.
    /// CHỈ lấy dữ liệu từ control, KHÔNG gọi API POST. Assemble sẵn HisKskProfileFullSDO để dùng sau.
    /// Phần Tiêm chủng (C) là chỉ đọc -> không thu thập ở đây.
    /// </summary>
    public partial class frmEnterKskInfomantionQD831
    {
        // ==================== PHẦN A — Hồ sơ (HIS_KSK_PROFILE) ====================
        /// <summary>Hành chính nhập được: mã hộ GĐ, ĐT cố định, số mũi UV mẹ (footer Tiêm chủng). Còn lại chỉ đọc từ BN.</summary>
        private HIS_KSK_PROFILE CollectKskProfile()
        {
            var p = new HIS_KSK_PROFILE();
            try
            {
                if (currentServiceReq != null)
                {
                    p.PATIENT_ID = currentServiceReq.TDL_PATIENT_ID;
                    p.TDL_PATIENT_CODE = currentServiceReq.TDL_PATIENT_CODE;
                    p.SERVICE_REQ_ID = currentServiceReq.ID;              // cột mới (EFMODEL cập nhật)
                    p.TREATMENT_ID = currentServiceReq.TREATMENT_ID;      // cột mới (EFMODEL cập nhật)
                }
                p.HOUSEHOLD_CODE = EditText(this.txtHouseholdCode);
                p.PHONE_FIXED = EditText(this.txtLandlinePhone);
                p.INTRUCTION_NOTE = EditText(this.txtLyDoKham); // Lý do khám
                p.MOTHER_TETANUS_DOSE = ShortFromSpin(this.txtTcSoMuiUonVan); // footer "Số mũi vắc xin uốn ván mẹ đã tiêm"

                // Mục 1 — Tình trạng lúc sinh (checkbox -> NUMBER(2,0); memo -> VARCHAR2)
                p.BIRTH_NORMAL_DELIVERY = ChkToShort(this.chkTsDeThuong);      // Sinh thường
                p.BIRTH_CESAREAN = ChkToShort(this.chkTsDeMo);                 // Sinh mổ
                p.BIRTH_PRETERM = ChkToShort(this.chkTsDeThieuThang);          // Sinh thiếu tháng
                p.BIRTH_ASPHYXIA = ChkToShort(this.chkTsBiNgat);              // Bị ngạt
                p.BIRTH_WEIGHT = EditText(this.memoTsCanNangLucDe);            // Cân nặng lúc đẻ
                p.BIRTH_LENGTH = EditText(this.memoTsChieuDaiLucDe);           // Chiều dài lúc đẻ
                p.CONGENITAL_DEFECT = EditText(this.memoTsDiTatBamSinh);       // Dị tật bẩm sinh
                p.BIRTH_PROBLEM = EditText(this.memoTsVanDeSinh);             // Vấn đề khi sinh

                // Mục 2 — Yếu tố nguy cơ (radio Có/Không -> ?Int16; checkbox -> ?Int16; memo/combo -> text; số ly -> ?Int32)
                p.SMOKING = RadioToShort(this.rdoTsHutThuoc);                      // Hút thuốc lá, lào
                p.SMOKING_REGULAR = ChkToShort(this.chkTsHutThuongXuyen);         // Hút thường xuyên
                p.SMOKING_QUIT = ChkToShort(this.chkTsHutDaBo);                   // Đã bỏ hút thuốc
                p.ALCOHOL = RadioToShort(this.rdoTsRuouBia);                      // Uống rượu bia thường xuyên
                p.ALCOHOL_GLASS_NUM = ParseIntStr(EditText(this.memoTsSoLyRuou)); // Số ly/cốc uống/ngày
                p.ALCOHOL_QUIT = ChkToShort(this.chkTsRuouDaBo);                  // Đã bỏ rượu bia
                p.DRUG_USE = RadioToShort(this.rdoTsMaTuy);                       // Sử dụng ma túy
                p.DRUG_REGULAR = ChkToShort(this.chkTsMaTuyThuongXuyen);          // Ma túy thường xuyên
                p.DRUG_QUIT = ChkToShort(this.chkTsMaTuyDaBo);                    // Đã bỏ ma túy
                p.PHYSICAL_ACTIVITY = RadioToShort(this.rdoTsTheLuc);            // Hoạt động thể lực
                p.PHYSICAL_ACTIVITY_REGULAR = ChkToShort(this.chkTsTheLucThuongXuyen); // Thường xuyên tập thể dục
                p.OCCUPATIONAL_EXPOSURE = EditText(this.memoTsTiepXucNgheNghiep); // Yếu tố tiếp xúc
                p.EXPOSURE_DURATION = EditText(this.memoTsThoiGianTiepXuc);       // Thời gian tiếp xúc
                p.TOILET_TYPE = EditText(this.cboTsHoXi);                         // Loại hố xí
                p.OTHER_RISK_FACTOR = EditText(this.memoTsNguyCoKhac);            // Nguy cơ khác

                // Mục 7 — Sản phụ khoa (phần cột ở PROFILE; các chỉ số PARA vẫn ở HIS_KSK_GENERAL)
                p.LAST_PREGNANCY_PERIOD = EditText(this.memoTsKyCoThaiCuoiCung);  // Kỳ có thai cuối cùng
                p.ABORTION_NUM = IntFromSpin(this.memoTsSoLanPhaThai);            // Số lần phá thai
                p.FULL_TERM_BIRTH_NUM = IntFromSpin(this.memoTsSoLanDeDuThang);   // Số lần đẻ đủ tháng
                p.PRETERM_BIRTH_NUM = IntFromSpin(this.memoTsSoLanDeNon);         // Số lần đẻ non
                p.NORMAL_DELIVERY = EditText(this.memoTsDeThuong);               // Đẻ thường
                p.CESAREAN_DELIVERY = EditText(this.memoTsDeMo);                 // Đẻ mổ
                p.DIFFICULT_DELIVERY = EditText(this.memoTsDeKho);              // Đẻ khó
                p.GYNECOLOGICAL_DISEASE = EditText(this.memoTsBenhPhuKhoa);      // Bệnh phụ khoa

                // Mục 8 (Vấn đề khác) + Khám LS (Vận động / Đánh giá phát triển) — cột mới ở PROFILE
                p.OTHER_ISSUE = EditText(this.memoTsVanDeKhacTong);              // Mục 8 - Vấn đề khác
                p.CLINICAL_MOTOR = EditText(this.memoKlsVanDong);               // Khám LS - Vận động
                p.CLINICAL_DEVELOPMENT_ASSESS = EditText(this.memoKlsDanhGiaPhatTrien); // Khám LS - Đánh giá phát triển

                p.IS_ACTIVE = 1;
                p.IS_DELETE = 0;
            }
            catch (Exception ex) { LogSystem.Error(ex); }
            return p;
        }

        // ==================== PHẦN B5/B7/B8 + D — Lần khám (HIS_KSK_GENERAL) ====================
        /// <summary>Chỉ set các ô ĐÃ có control: CLS (huyết học/sinh hóa/nước tiểu/siêu âm), tư vấn, bác sĩ khám, kết luận ICD.</summary>
        private HIS_KSK_GENERAL CollectKskGeneral()
        {
            var g = new HIS_KSK_GENERAL();
            try
            {
                if (currentServiceReq != null) g.SERVICE_REQ_ID = currentServiceReq.ID;

                // Phần D — CLS (tab Khám cận lâm sàng)
                g.NOTE_BLOOD = EditText(this.memoClsHuyetHoc);
                g.NOTE_BIOCHEMICAL = EditText(this.memoClsSinhHoaMau);
                g.NOTE_TEST_URINE = EditText(this.memoClsSinhHoaNuocTieu);
                g.NOTE_SUPERSONIC = EditText(this.memoClsSieuAmOB);

                // Tư vấn + Kết luận: NGÀY KẾT LUẬN = hiện tại; NGƯỜI KẾT LUẬN = Bác sĩ khám trên form.
                g.TREATMENT_INSTRUCTION = EditText(this.memoClsTuVan);
                g.CONCLUSION_TIME = NowTime(); // ngày/giờ kết luận = hiện tại
                if (this.cboClsBacSiKham != null && this.cboClsBacSiKham.EditValue != null)
                {
                    g.CONCLUDER_LOGINNAME = this.cboClsBacSiKham.EditValue.ToString();
                    g.CONCLUDER_USERNAME = (this.cboClsBacSiKham.Text ?? "").Trim();
                }

                // Kết luận ICD-10 (mã + tên, nhiều CĐ)
                if (this.subIcdProcessorCls != null && this.ucClsSecondaryIcd != null)
                {
                    var icd = this.subIcdProcessorCls.GetValue(this.ucClsSecondaryIcd) as SecondaryIcdDataADO;
                    if (icd != null)
                    {
                        g.CONCLUSION_ICD_CODE = icd.ICD_SUB_CODE;
                        g.CONCLUSION_ICD_NAME = icd.ICD_TEXT;
                    }
                }

                // ===== Phần B5 — Tiền sử phẫu thuật (tab "Tiền sử" mục 5) =====
                // Text tự do "bộ phận + năm phẫu thuật" -> HISTORY_SURGERY (giống txtProceduresAndSurgeriesPerformed bên Officials).
                // NOTE_SURGICAL/NUMBER_OF_SURGERIES là ghi chú + số lần phẫu thuật SẢN KHOA (thuộc mục 7), không dùng ở đây.
                g.HISTORY_SURGERY = EditText(this.memoTsPhauThuat);

                // ===== Phần B7 — Sản phụ khoa (tab "Tiền sử" mục 7) — CHỈ các ô có cột sẵn =====
                g.NOTE_CONTRACEPTIVES = EditText(this.memoTsBienPhapTranhThai); // biện pháp tránh thai (text)
                g.PREGNANCY = ShortFromSpin(this.memoTsSoLanCoThai);     // số lần có thai (bộ PARA)
                g.ABORTUS = ShortFromSpin(this.memoTsSoLanSayThai);      // số lần sảy thai
                g.RECURRENT = ShortFromSpin(this.memoTsSoLanSinhDe);     // số lần sinh đẻ (PARA - nghĩa cần chốt)
                g.ALIVE = ShortFromSpin(this.memoTsSoConHienSong);       // số con hiện sống
                // memoTsBenhPhuKhoa: chỉ có cột ICD (OBSTETRIC_DISEASE_ICD_CODE/NAME), không có ô text tự do
                //   -> cần ICD picker hoặc cột text mới -> CHƯA nạp.
                // Các ô đếm khác của mục 7 (phá thai, đẻ thường/mổ/khó, đủ tháng/non) không có cột -> chờ backend.

                // ===== Phần D — KHÁM LÂM SÀNG (tab "Khám lâm sàng") — theo tài liệu QĐ831 mục D =====
                // 1. Bệnh sử (ngày kết luận đã set ở block Bác sĩ khám phía trên)
                g.PATHOLOGICAL_HISTORY = EditText(this.memoKlsBenhSu);

                // 2.2 Thị lực: không kính (RIGHT/LEFT) + có kính (GLASS_RIGHT/LEFT), MP = mắt phải, MT = mắt trái
                g.EXAM_EYESIGHT_RIGHT = EditText(this.txtKlsKhongKinhMP);
                g.EXAM_EYESIGHT_LEFT = EditText(this.txtKlsKhongKinhMT);
                g.EXAM_EYESIGHT_GLASS_RIGHT = EditText(this.txtKlsCoKinhMP);
                g.EXAM_EYESIGHT_GLASS_LEFT = EditText(this.txtKlsCoKinhMT);

                // 2.3.1 Toàn thân
                g.BODY_SKIN = EditText(this.memoKlsDaNiemMac);
                g.BODY_OTHER = EditText(this.memoKlsToanThanKhac);

                // 2.3.2 Khám cơ quan (14 vùng)
                g.EXAM_CIRCULATION = EditText(this.memoKlsTimMach);
                g.EXAM_RESPIRATORY = EditText(this.memoKlsHoHap);
                g.EXAM_DIGESTION = EditText(this.memoKlsTieuHoa);
                g.EXAM_KIDNEY_UROLOGY = EditText(this.memoKlsTietNieu);
                g.EXAM_MUSCLE_BONE = EditText(this.memoKlsCoXuongKhop);
                g.EXAM_NEUROLOGICAL = EditText(this.memoKlsThanKinh);
                g.EXAM_MENTAL = EditText(this.memoKlsTamThan);
                g.EXAM_SURGERY = EditText(this.memoKlsNgoaiKhoa);
                g.EXAM_OBSTETRIC = EditText(this.memoKlsSanPhuKhoa);
                g.EXAM_ENT = EditText(this.memoKlsTaiMuiHong);
                g.EXAM_STOMATOLOGY = EditText(this.memoKlsRangHamMat);
                g.EXAM_EYE = EditText(this.memoKlsMat);
                g.EXAM_DERMATOLOGY = EditText(this.memoKlsDaLieu);
                g.EXAM_NUTRION = EditText(this.memoKlsDinhDuong);
                g.EXAM_OEND = EditText(this.memoKlsNoiTiet); // Nội tiết (EXAM_OEND đã xác nhận = Nội tiết)
                g.EXAM_OTHER = EditText(this.memoKlsKhac);

                // Ghi chú: Vận động + Đánh giá phát triển (Khám LS) nay lưu ở HIS_KSK_PROFILE
                //   (CLINICAL_MOTOR / CLINICAL_DEVELOPMENT_ASSESS) -> thu ở CollectKskProfile.
                //   HEALTH_EXAM_RANK_ID (phân loại sức khỏe) — chưa có control trên form QĐ831 này.
            }
            catch (Exception ex) { LogSystem.Error(ex); }
            return g;
        }

        // ==================== PHẦN B2/B3/B4/B6 — Checklist (HIS_DISEASE_DETAIL_RESULT) ====================
        /// <summary>
        /// Gom checklist tiền sử: Bệnh tật bản thân (control động 49) + 4 grid (Dị ứng BT 50, Khuyết tật 53,
        /// Dị ứng GĐ 52, Bệnh tật GĐ 51). Mỗi mục danh mục = 1 HIS_DISEASE_DETAIL_RESULT.
        /// </summary>
        private List<HIS_DISEASE_DETAIL_RESULT> CollectDiseaseDetailResults()
        {
            var list = new List<HIS_DISEASE_DETAIL_RESULT>();
            try
            {
                // (49) Bệnh tật bản thân — control động: checkbox và/hoặc ô "ghi rõ"
                if (this.benhTatCheckMap != null)
                {
                    foreach (var kv in this.benhTatCheckMap)
                    {
                        var r = NewDiseaseResult(kv.Key);
                        r.IS_CHECK = (short)((kv.Value != null && kv.Value.Checked) ? 1 : 0);
                        if (this.benhTatTextMap != null && this.benhTatTextMap.ContainsKey(kv.Key))
                            r.OTHER = EditText(this.benhTatTextMap[kv.Key]);
                        list.Add(r);
                    }
                    // các mục is_other-only (không có checkbox) -> chỉ OTHER
                    if (this.benhTatTextMap != null)
                        foreach (var kv in this.benhTatTextMap)
                            if (!this.benhTatCheckMap.ContainsKey(kv.Key))
                            {
                                var r = NewDiseaseResult(kv.Key);
                                r.OTHER = EditText(kv.Value);
                                list.Add(r);
                            }
                }

                // (50/53/52/51) 4 grid
                if (this.diseaseGridRows != null)
                {
                    foreach (var rows in this.diseaseGridRows.Values)
                    {
                        if (rows == null) continue;
                        foreach (var row in rows)
                        {
                            var r = NewDiseaseResult(row.DetailId);
                            r.IS_CHECK = (short)((row.Chon.HasValue && row.Chon.Value) ? 1 : 0);
                            r.OTHER = JoinOther(row.MoTa, row.NguoiMac); // mô tả (+ người mắc nếu grid gia đình)
                            list.Add(r);
                        }
                    }
                }
            }
            catch (Exception ex) { LogSystem.Error(ex); }
            return list;
        }

        private HIS_DISEASE_DETAIL_RESULT NewDiseaseResult(long detailId)
        {
            return new HIS_DISEASE_DETAIL_RESULT
            {
                DISEASE_DETAIL_ID = detailId,
                IS_CHECK = 0,
                IS_ACTIVE = 1,
                IS_DELETE = 0
                // KSK_GENERAL_ID gán sau khi có HIS_KSK_GENERAL.ID (lúc lưu thật).
            };
        }

        // ==================== PHẦN D — Sinh tồn (HIS_DHST) ====================
        /// <summary>Sinh tồn (2.1) — mạch/nhiệt/HA/nhịp thở/cân/cao/BMI/vòng bụng. Liên kết DHST_ID gán lúc POST thật.</summary>
        private HIS_DHST CollectDhst()
        {
            try
            {
                var d = new HIS_DHST();
                // Link: HIS_DHST.TREATMENT_ID (bắt buộc) theo lượt điều trị của y lệnh đang xử lý.
                if (currentServiceReq != null && currentServiceReq.TREATMENT_ID > 0)
                    d.TREATMENT_ID = Convert.ToInt64(currentServiceReq.TREATMENT_ID);
                d.PULSE = LongFromSpin(this.txtKlsMach);
                d.TEMPERATURE = DecimalFromSpin(this.txtKlsNhietDo);
                FillBloodPressure(d, EditText(this.txtKlsHa)); // "120/80" -> MAX/MIN (ô ghép, giữ TextEdit)
                d.BREATH_RATE = DecimalFromSpin(this.txtKlsNhipTho);
                d.WEIGHT = DecimalFromSpin(this.txtKlsCanNang);
                d.HEIGHT = DecimalFromSpin(this.txtKlsCao);
                d.VIR_BMI = DecimalFromSpin(this.txtKlsBmi);
                d.BELLY = DecimalFromSpin(this.txtKlsVongBung);
                return d;
            }
            catch (Exception ex) { LogSystem.Error(ex); return null; }
        }

        /// <summary>Tách "HA tối đa/tối thiểu" (vd 120/80) vào BLOOD_PRESSURE_MAX + MIN.</summary>
        private void FillBloodPressure(HIS_DHST d, string raw)
        {
            if (d == null || string.IsNullOrEmpty((raw ?? "").Trim())) return;
            var parts = raw.Split(new[] { '/', '\\', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1) d.BLOOD_PRESSURE_MAX = ParseLong(parts[0]);
            if (parts.Length >= 2) d.BLOOD_PRESSURE_MIN = ParseLong(parts[1]);
        }

        // ==================== ASSEMBLE ====================
        /// <summary>
        /// Gom phần khám (B+D) + tiêm chủng (C) -> HisKskProfileExamSDO cho POST /api/HisKskProfile/SaveExam.
        /// (Phần A hồ sơ HIS_KSK_PROFILE thu riêng ở CollectKskProfile — lưu qua API hồ sơ.)
        /// </summary>
        private HisKskProfileExamSDO CollectExamSdo()
        {
            var exam = new HisKskProfileExamSDO();
            try
            {
                // Chỉ gắn object nếu CÓ ít nhất 1 thông tin nhập; danh sách chỉ giữ dòng có dữ liệu.
                var g = CollectKskGeneral();
                exam.HisKskGeneral = GeneralHasData(g) ? g : null;

                var d = CollectDhst();
                exam.HisDhst = DhstHasData(d) ? d : null;

                exam.HisDiseaseDetailResults = CollectDiseaseDetailResults()
                    .Where(r => r != null && (r.IS_CHECK == 1 || !string.IsNullOrEmpty(r.OTHER))).ToList();

                exam.HisHealthVaccinations = CollectHealthVaccinations()
                    .Where(VaccinationHasData).ToList();

                // Lý do khám (phần A) -> field mới của SDO; BE ghi vào HIS_KSK_PROFILE.INTRUCTION_NOTE.
                exam.IntructionNote = EditText(this.txtLyDoKham);

                // Danh sách quan hệ người nhà (HIS_KSK_RELATION) -> BE gắn theo HIS_KSK_PROFILE.
                exam.HisKskRelations = (this.familyRelations != null)
                    ? this.familyRelations.Where(r => r != null).ToList()
                    : new List<HIS_KSK_RELATION>();

                // Part A hồ sơ (HIS_KSK_PROFILE) — gửi ĐỦ để BE upsert theo SERVICE_REQ_ID (Mục 1/2/6/7/8, UV, hộ, hố xí, vận động...).
                exam.HisKskProfile = CollectKskProfile();

                // Nếu y lệnh đã có bản ghi lưu trước đó -> gán lại ID để UPDATE (không INSERT trùng).
                ApplyExistingIds(exam);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
            return exam;
        }

        // ==================== Gán lại ID bản ghi đã lưu (UPDATE thay vì INSERT trùng) ====================
        /// <summary>
        /// Nếu y lệnh đang xử lý ĐÃ có dữ liệu lưu trước đó (trong _loadedFull) thì gán lại ID (và FK cũ)
        /// vào các object sẽ POST — tránh BE tạo bản ghi mới trùng. Áp cho PROFILE / GENERAL / DHST /
        /// DISEASE_DETAIL_RESULT (match theo DISEASE_DETAIL_ID) / HEALTH_VACCINATION (match theo VACCINE_CODE+GROUP).
        /// (Quan hệ gia đình đã tự mang ID từ popup.)
        /// </summary>
        private void ApplyExistingIds(HisKskProfileExamSDO exam)
        {
            try
            {
                if (exam == null || _loadedFull == null) return;
                var examOld = _loadedFull.ExamHistory != null ? _loadedFull.ExamHistory.FirstOrDefault() : null;
                var profOld = (_loadedFull.PatientInfo != null && _loadedFull.PatientInfo.Profiles != null)
                    ? _loadedFull.PatientInfo.Profiles.FirstOrDefault() : null;
                if (profOld == null && examOld != null) profOld = examOld.HisKskProfile;

                // PROFILE
                if (exam.HisKskProfile != null && profOld != null && profOld.ID > 0)
                {
                    exam.HisKskProfile.ID = profOld.ID;
                    exam.HisKskProfile.LAST_KSK_GENERAL_ID = profOld.LAST_KSK_GENERAL_ID; // giữ liên kết cũ
                }

                if (examOld != null)
                {
                    // GENERAL
                    if (exam.HisKskGeneral != null && examOld.HisKskGeneral != null && examOld.HisKskGeneral.ID > 0)
                    {
                        exam.HisKskGeneral.ID = examOld.HisKskGeneral.ID;
                        exam.HisKskGeneral.DHST_ID = examOld.HisKskGeneral.DHST_ID; // giữ liên kết DHST cũ
                    }
                    // DHST
                    if (exam.HisDhst != null && examOld.HisDhst != null && examOld.HisDhst.ID > 0)
                        exam.HisDhst.ID = examOld.HisDhst.ID;

                    // DISEASE_DETAIL_RESULT — match theo DISEASE_DETAIL_ID
                    if (exam.HisDiseaseDetailResults != null && examOld.HisDiseaseDetailResults != null)
                    {
                        var oldMap = new Dictionary<long, HIS_DISEASE_DETAIL_RESULT>();
                        foreach (var o in examOld.HisDiseaseDetailResults)
                            if (o != null && o.DISEASE_DETAIL_ID != null && !oldMap.ContainsKey(o.DISEASE_DETAIL_ID.Value))
                                oldMap[o.DISEASE_DETAIL_ID.Value] = o;
                        foreach (var r in exam.HisDiseaseDetailResults)
                        {
                            if (r == null || r.DISEASE_DETAIL_ID == null) continue;
                            HIS_DISEASE_DETAIL_RESULT o;
                            if (oldMap.TryGetValue(r.DISEASE_DETAIL_ID.Value, out o) && o != null)
                            {
                                r.ID = o.ID;
                                r.KSK_GENERAL_ID = o.KSK_GENERAL_ID;
                            }
                        }
                    }

                    // HEALTH_VACCINATION — match theo VACCINE_CODE + VACCINE_GROUP
                    var oldVaccs = (examOld.HisHealthVaccinations != null && examOld.HisHealthVaccinations.Count > 0)
                        ? examOld.HisHealthVaccinations : _loadedFull.Vaccination;
                    if (exam.HisHealthVaccinations != null && oldVaccs != null)
                    {
                        var vmap = new Dictionary<string, Queue<HIS_HEALTH_VACCINATION>>();
                        foreach (var o in oldVaccs)
                        {
                            if (o == null) continue;
                            string k = VaccKey(o.VACCINE_CODE, o.VACCINE_GROUP);
                            if (!vmap.ContainsKey(k)) vmap[k] = new Queue<HIS_HEALTH_VACCINATION>();
                            vmap[k].Enqueue(o);
                        }
                        foreach (var h in exam.HisHealthVaccinations)
                        {
                            if (h == null) continue;
                            string k = VaccKey(h.VACCINE_CODE, h.VACCINE_GROUP);
                            Queue<HIS_HEALTH_VACCINATION> q;
                            if (vmap.TryGetValue(k, out q) && q.Count > 0)
                                h.ID = q.Dequeue().ID;
                        }
                    }
                }
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private static string VaccKey(string code, short? group)
        {
            return (code ?? "").Trim() + "|" + (group.HasValue ? group.Value.ToString() : "");
        }

        // ==================== Kiểm tra "có dữ liệu" (chỉ lưu object khi có ≥1 thông tin) ====================
        /// <summary>HIS_KSK_GENERAL có dữ liệu nhập? (bất kỳ ô text nào khác rỗng, hoặc PARA có số). Bỏ qua ID/thời gian tự sinh.</summary>
        private static bool GeneralHasData(HIS_KSK_GENERAL g)
        {
            if (g == null) return false;
            foreach (var pi in typeof(HIS_KSK_GENERAL).GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    var v = pi.GetValue(g, null) as string;
                    if (!string.IsNullOrWhiteSpace(v)) return true;
                }
            }
            return g.PREGNANCY != null || g.ABORTUS != null || g.RECURRENT != null || g.ALIVE != null;
        }

        // Bỏ qua khi xét "profile có dữ liệu nhập": cột link/tự sinh + lý do khám (auto từ y lệnh).
        private static readonly string[] ProfileIgnore = {
            "ID","PATIENT_ID","TDL_PATIENT_CODE","SERVICE_REQ_ID","TREATMENT_ID","IS_ACTIVE","IS_DELETE",
            "GROUP_CODE","INTRUCTION_NOTE","NOTE","SYNC_FAILD_REASON",
            "CREATE_TIME","MODIFY_TIME","CREATOR","MODIFIER","APP_CREATOR","APP_MODIFIER" };
        // Radio Có/Không: chọn "Không" (=0) vẫn tính là có nhập.
        private static readonly string[] ProfileRadioFields = { "SMOKING","ALCOHOL","DRUG_USE","PHYSICAL_ACTIVITY" };

        /// <summary>HIS_KSK_PROFILE (Part A) có dữ liệu người dùng nhập? (bỏ cột link/tự sinh + lý do khám auto).</summary>
        private static bool ProfileHasData(HIS_KSK_PROFILE p)
        {
            if (p == null) return false;
            foreach (var pi in typeof(HIS_KSK_PROFILE).GetProperties())
            {
                var name = pi.Name;
                if (Array.IndexOf(ProfileIgnore, name) >= 0) continue;
                var t = pi.PropertyType;
                bool isNullable = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
                if (!(t == typeof(string) || isNullable)) continue; // bỏ nav object / collection
                var v = pi.GetValue(p, null);
                if (v == null) continue;
                if (t == typeof(string)) { if (!string.IsNullOrWhiteSpace((string)v)) return true; continue; }
                if (Array.IndexOf(ProfileRadioFields, name) >= 0) return true; // radio đã chọn (kể cả 0)
                try { if (Convert.ToDecimal(v) != 0m) return true; } catch { return true; }
            }
            return false;
        }

        /// <summary>HIS_DHST có dữ liệu sinh tồn?</summary>
        private static bool DhstHasData(HIS_DHST d)
        {
            if (d == null) return false;
            return d.PULSE != null || d.TEMPERATURE != null || d.BLOOD_PRESSURE_MAX != null || d.BLOOD_PRESSURE_MIN != null
                || d.BREATH_RATE != null || d.WEIGHT != null || d.HEIGHT != null || d.VIR_BMI != null || d.BELLY != null;
        }

        /// <summary>1 dòng tiêm chủng có dữ liệu nhập?</summary>
        private static bool VaccinationHasData(HIS_HEALTH_VACCINATION v)
        {
            if (v == null) return false;
            return v.IS_NOT_VACCINATED == 1 || v.VACCINATED_TIME != null || v.APPOINTMENT_TIME != null
                || !string.IsNullOrWhiteSpace(v.REACTION) || v.PREGNANCY_MONTH != null;
        }

        // ==================== Helpers ====================
        private static string EditText(BaseEdit e)
        {
            return e != null ? (e.Text ?? "").Trim() : null;
        }

        private static long? ParseLong(string s)
        {
            long v;
            return long.TryParse((s ?? "").Trim(), out v) ? (long?)v : null;
        }

        // ---- Đọc trực tiếp giá trị số từ SpinEdit (control đã nhập số, không cần parse text) ----
        /// <summary>CheckEdit -> NUMBER(2,0): tích = 1, không tích = 0.</summary>
        private static short? ChkToShort(DevExpress.XtraEditors.CheckEdit c)
        {
            return (short)(c != null && c.Checked ? 1 : 0);
        }

        /// <summary>RadioGroup (Có=1/Không=0) -> ?Int16; chưa chọn -> null.</summary>
        private static short? RadioToShort(DevExpress.XtraEditors.RadioGroup r)
        {
            if (r == null || r.EditValue == null || r.EditValue is System.DBNull) return null;
            try { return System.Convert.ToInt16(r.EditValue); } catch { return null; }
        }

        /// <summary>SpinEdit -> ?Int32 (bỏ trống -> null).</summary>
        private static int? IntFromSpin(DevExpress.XtraEditors.SpinEdit s)
        {
            if (s == null || s.EditValue == null || s.EditValue is System.DBNull) return null;
            try { return (int?)System.Convert.ToInt32(s.Value); } catch { return null; }
        }

        private static int? ParseIntStr(string s)
        {
            int v;
            return int.TryParse((s ?? "").Trim(), out v) ? (int?)v : null;
        }

        private static short? ShortFromSpin(DevExpress.XtraEditors.SpinEdit s)
        {
            if (s == null || s.EditValue == null || s.EditValue is System.DBNull) return null;
            try { return (short?)System.Convert.ToInt16(s.Value); } catch { return null; }
        }

        private static long? LongFromSpin(DevExpress.XtraEditors.SpinEdit s)
        {
            if (s == null || s.EditValue == null || s.EditValue is System.DBNull) return null;
            try { return (long?)System.Convert.ToInt64(s.Value); } catch { return null; }
        }

        private static decimal? DecimalFromSpin(DevExpress.XtraEditors.SpinEdit s)
        {
            if (s == null || s.EditValue == null || s.EditValue is System.DBNull) return null;
            try { return (decimal?)s.Value; } catch { return null; }
        }

        /// <summary>Thời điểm hiện tại dạng yyyyMMddHHmmss (long) — dùng cho CONCLUSION_TIME.</summary>
        private static long NowTime()
        {
            try { return Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")); }
            catch { return 0; }
        }

        private static string JoinOther(string mota, string nguoiMac)
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(mota)) parts.Add(mota.Trim());
            if (!string.IsNullOrEmpty(nguoiMac)) parts.Add("Người mắc: " + nguoiMac.Trim());
            return parts.Count > 0 ? string.Join(" | ", parts.ToArray()) : null;
        }
    }
}
