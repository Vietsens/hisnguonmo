/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Xml831
{
    /// <summary>
    /// Map entity HIS -&gt; model XML QĐ831. Bước này CHỈ xử lý 2 khối dạng list:
    /// &lt;TIEMCHUNG&gt; (từ HIS_HEALTH_VACCINATION) và &lt;DANHSACHHOSOKHAMCHUABENH&gt;
    /// (từ HIS_SERVICE_REQ + HIS_KSK_GENERAL + HIS_DHST). CANLAMSANG chưa xử lý.
    /// </summary>
    internal static class Ksk831Builder
    {
        #region ASSEMBLE
        /// <summary>Gộp các khối thành &lt;DATA&gt;. signature: chuỗi chữ ký (rỗng nếu không ký).</summary>
        internal static Data BuildData(Header header, ThongTinChung ttc, TienSu tienSu, TiemChung tiemChung,
            List<HoSoKhamChuaBenh> hoSoList, string signature)
        {
            return new Data
            {
                Header = header,
                Body = new Body
                {
                    HoSoSucKhoe = new HoSoSucKhoe
                    {
                        ThongTinChung = ttc,
                        TienSu = tienSu,
                        TiemChung = tiemChung,
                        DanhSachHoSoKhamChuaBenh = hoSoList
                    }
                },
                Security = new Security { Signature = signature ?? "" }
            };
        }
        #endregion

        #region HEADER
        /// <summary>
        /// &lt;HEADER&gt;: SENDER_CODE = HIS_BRANCH.HEIN_MEDI_ORG_CODE, SENDER_NAME = HIS_BRANCH.BRANCH_NAME.
        /// Các trường còn lại chỉ phục vụ ghi log (đặt giá trị mặc định: version 1.0, ngày giao dịch, action 0).
        /// </summary>
        internal static Header BuildHeader(HIS_BRANCH branch)
        {
            return new Header
            {
                MessageVersion = "1.0",
                SenderCode = (branch != null) ? branch.HEIN_MEDI_ORG_CODE : null,
                SenderName = (branch != null) ? branch.BRANCH_NAME : null,
                TransactionType = null,
                TransactionName = null,
                TransactionDate = DateTime.Now.ToString("dd/MM/yyyy"),
                TransactionId = null,
                RequestId = null,
                ActionType = "0"
            };
        }
        #endregion

        #region TIEMCHUNG
        /// <summary>
        /// HIS_HEALTH_VACCINATION -&gt; &lt;TIEMCHUNG&gt; (mỗi bản ghi = 1 &lt;THONGTINMUITIEM&gt;).
        /// SOMUIUONVANMETIEM (cấp TIEMCHUNG) = HIS_KSK_PROFILE.MOTHER_TETANUS_DOSE.
        /// </summary>
        internal static TiemChung BuildTiemChung(IEnumerable<HIS_HEALTH_VACCINATION> vaccinations, HIS_KSK_PROFILE profile)
        {
            var result = new TiemChung { ThongTinMuiTiem = new List<ThongTinMuiTiem>() };
            if (profile != null) result.SoMuiUongVanMeTiem = ToStr(profile.MOTHER_TETANUS_DOSE);
            if (vaccinations == null) return result;

            foreach (var v in vaccinations)
            {
                if (v == null) continue;
                result.ThongTinMuiTiem.Add(new ThongTinMuiTiem
                {
                    MaVacXin = v.VACCINE_CODE,
                    TenVacXin = v.VACCINE_NAME,
                    LoaiVacXin = ToStr(v.VACCINE_GROUP),
                    // TRANGTHAI: 1 = đã chủng ngừa, 0 = chưa. IS_NOT_VACCINATED=1 (chưa tiêm) -> "0", ngược lại "1".
                    TrangThai = (ToInt(v.IS_NOT_VACCINATED) == 1) ? "0" : "1",
                    NgayTiem = FormatDate(v.VACCINATED_TIME),
                    ThangThai = ToStr(v.PREGNANCY_MONTH),
                    PhanUngSauTiem = v.REACTION,
                    NgayHenTiem = FormatDate(v.APPOINTMENT_TIME),
                    SoMuiUongVanMeTiem = ToStr(v.DOSE_ORDER)
                });
            }
            return result;
        }
        #endregion

        #region THONGTINCHUNG
        /// <summary>
        /// HIS_PATIENT (+ HIS_KSK_PROFILE cho điện thoại cố định / mã hộ) -&gt; &lt;THONGTINCHUNG&gt;.
        /// QUANHE_GIADINH lấy từ HIS_KSK_RELATION (theo KSK_PROFILE_ID). Một số trường HIS chưa lưu
        /// (VNEID, mã nhân khẩu, mã thôn/xóm, quan hệ chủ hộ dạng code) -&gt; để null (đánh TODO).
        /// </summary>
        internal static ThongTinChung BuildThongTinChung(HIS_PATIENT p, HIS_KSK_PROFILE profile, List<HIS_KSK_RELATION> relations)
        {
            if (p == null) return null;

            // SOCMND/NGAYCAP/NOICAP: ưu tiên theo giấy tờ CCCD -> CMND -> Passport (dùng cả 3 trường cùng loại).
            string soGiayTo, ngayCap, noiCap;
            if (!string.IsNullOrEmpty(p.CCCD_NUMBER))
            { soGiayTo = p.CCCD_NUMBER; ngayCap = FormatDate(p.CCCD_DATE); noiCap = p.CCCD_PLACE; }
            else if (!string.IsNullOrEmpty(p.CMND_NUMBER))
            { soGiayTo = p.CMND_NUMBER; ngayCap = FormatDate(p.CMND_DATE); noiCap = p.CMND_PLACE; }
            else if (!string.IsNullOrEmpty(p.PASSPORT_NUMBER))
            { soGiayTo = p.PASSPORT_NUMBER; ngayCap = FormatDate(p.PASSPORT_DATE); noiCap = p.PASSPORT_PLACE; }
            else
            { soGiayTo = null; ngayCap = null; noiCap = null; }

            string maHo = !string.IsNullOrEmpty(p.HOUSEHOLD_CODE) ? p.HOUSEHOLD_CODE
                : (profile != null ? profile.HOUSEHOLD_CODE : null);

            return new ThongTinChung
            {
                MaDinhDanh = p.CCCD_NUMBER,          // TODO: xác nhận nguồn "mã định danh" (CCCD hay UUID/mã định danh y tế)
                VneId = null,                        // TODO: HIS chưa lưu VNeID
                MaBhxh = p.SOCIAL_INSURANCE_NUMBER,
                MaThe = p.TDL_HEIN_CARD_NUMBER,
                MaHoGiaDinh = maHo,
                MaNhanKhau = null,                   // TODO: HIS chưa lưu mã nhân khẩu
                HoTen = p.VIR_PATIENT_NAME,
                QuanHeChuHo = null,                  // TODO: chỉ có HOUSEHOLD_RELATION_NAME (tên), cần map sang code
                GioiTinh = ToStr(p.GENDER_ID),       // 1=Nam, 2=Nữ (khớp GENDER_ID)
                NhomMauHeAbo = p.BLOOD_ABO_CODE,
                NhomMauHeRh = p.BLOOD_RH_CODE,
                NgaySinh = FormatDate(p.DOB),
                MaTinhKhaiSinh = p.BORN_PROVINCE_CODE,
                MaDanToc = p.ETHNIC_CODE,
                MaQuocTich = p.NATIONAL_CODE,
                MaTonGiao = MapMaTonGiao(p.RELIGION_NAME),   // map tên tôn giáo -> mã (1..16)
                MaNgheNghiep = p.CAREER_CODE,
                SoCmnd = soGiayTo,
                NgayCap = ngayCap,
                NoiCap = noiCap,
                DiaChiThuongTru = p.VIR_ADDRESS,
                MaTinhThuongTru = p.PROVINCE_CODE,
                MaHuyenThuongTru = p.DISTRICT_CODE,
                MaXaThuongTru = p.COMMUNE_CODE,
                MaThonXomThuongTru = null,           // TODO: HIS chưa có mã thôn/xóm
                DiaChiHienTai = p.VIR_HT_ADDRESS,
                MaTinhHienTai = p.HT_PROVINCE_CODE,
                MaHuyenHienTai = p.HT_DISTRICT_CODE,
                MaXaHienTai = p.HT_COMMUNE_CODE,
                MaThonXomHienTai = null,             // TODO: HIS chưa có mã thôn/xóm
                DienThoaiCd = (profile != null) ? profile.PHONE_FIXED : null,
                DienThoaiDd = p.MOBILE,
                Email = p.EMAIL,
                QuanHeGiaDinh = BuildQuanHeGiaDinh(relations, (profile != null) ? profile.ID : 0)
            };
        }

        /// <summary>
        /// HIS_KSK_RELATION (theo KSK_PROFILE_ID) -&gt; danh sách &lt;QUANHE_GIADINH&gt;.
        /// LOAI_QUANHE=RELATION_CODE, TEN_QUANHE=RELATION_NAME, MADINHDANH=IDENTITY_CODE, HOTEN=RELATED_PERSON_NAME,
        /// DIENTHOAI=PHONE, DIDONG=MOBILE, GIAMHO=IS_GUARDIAN(1/0).
        /// </summary>
        private static List<QuanHeGiaDinh> BuildQuanHeGiaDinh(List<HIS_KSK_RELATION> relations, long profileId)
        {
            if (relations == null || relations.Count == 0) return null;
            var list = new List<QuanHeGiaDinh>();
            foreach (var r in relations)
            {
                if (r == null) continue;
                if (ToInt(r.IS_DELETE) == 1) continue;                          // bỏ bản ghi đã xóa
                if (profileId > 0 && (r.KSK_PROFILE_ID ?? 0) != profileId) continue;  // đúng hồ sơ
                list.Add(new QuanHeGiaDinh
                {
                    LoaiQuanHe = r.RELATION_CODE,
                    TenQuanHe = r.RELATION_NAME,
                    MaDinhDanh = r.IDENTITY_CODE,
                    HoTen = r.RELATED_PERSON_NAME,
                    DienThoai = r.PHONE,
                    DiDong = r.MOBILE,
                    GiamHo = ToFlag(r.IS_GUARDIAN)
                });
            }
            return list.Count > 0 ? list : null;
        }
        #endregion

        #region TIENSU
        // Nhóm danh mục trong HIS_DISEASE_TYPE (theo cấu hình nghiệp vụ).
        private const long DISEASE_TYPE_ID__KHUYET_TAT = 53;
        // Tiền sử BẢN THÂN
        private const long DISEASE_TYPE_ID__DIUNG = 50;            // dị ứng bản thân
        private const long DISEASE_TYPE_ID__BENHTAT = 49;         // bệnh tật bản thân
        // Tiền sử GIA ĐÌNH
        private const long DISEASE_TYPE_ID__DIUNG_GIADINH = 52;   // dị ứng gia đình
        private const long DISEASE_TYPE_ID__BENHTAT_GIADINH = 51; // bệnh tật gia đình

        /// <summary>
        /// Dựng &lt;TIENSU&gt;: TINHTRANG_LUCSINH + YEUTO_SUCKHOE + SUCKHOE_SINHSAN + VANDEKHAC (từ HIS_KSK_PROFILE),
        /// TIENSU_PHAUTHUAT + phần tránh thai/số lần có thai (từ HIS_KSK_GENERAL),
        /// KHUYETTAT (nhóm HIS_DISEASE_TYPE=53, dòng tích trong HIS_DISEASE_DETAIL_RESULT + danh mục HIS_DISEASE_DETAIL).
        /// TIEUSU_BENHTAT / TIENSU_GIADINH (list dị ứng/bệnh) làm ở bước sau.
        /// </summary>
        internal static TienSu BuildTienSu(HIS_KSK_PROFILE profile, HIS_KSK_GENERAL general,
            List<HIS_DISEASE_DETAIL_RESULT> diseaseResults, IDictionary<long, HIS_DISEASE_DETAIL> diseaseDetailById)
        {
            var ts = new TienSu();

            // TINHTRANG_LUCSINH
            if (profile != null)
            {
                // LOAIDE: 0 = Đẻ thường, 1 = Đẻ mổ. Có cờ đẻ mổ -> "1", còn lại -> "0".
                string loaiDe = (ToInt(profile.BIRTH_CESAREAN) == 1) ? "1" : "0";
                ts.TinhTrangLucSinh = new TinhTrangLucSinh
                {
                    LoaiDe = loaiDe,
                    DeThieuThang = ToFlag(profile.BIRTH_PRETERM), // 1 = Có, 0 = Không
                    BiNgatLucDe = ToFlag(profile.BIRTH_ASPHYXIA),
                    CanNang = profile.BIRTH_WEIGHT,
                    ChieuDai = profile.BIRTH_LENGTH,
                    DiTatBamSinh = profile.CONGENITAL_DEFECT,
                    Khac = profile.BIRTH_PROBLEM
                };
            }

            // YEUTO_SUCKHOE: 4 yếu tố nguy cơ (hút thuốc/rượu bia/ma túy/hoạt động thể lực) + phơi nhiễm/hố xí/khác.
            if (profile != null)
            {
                var y = new YeuToSucKhoe
                {
                    // TRANGTHAI: 0=không,1=có,2=thường xuyên. DABO: 0=không dùng,1=đã bỏ (chỉ Thuốc/Rượu/Ma túy).
                    // Quy tắc: DABO=0 -> TRANGTHAI bắt buộc = 0. Thể chất KHÔNG có DABO.
                    DsYeuTo = new List<DsYeuTo>
                    {
                        MakeRiskFactor("1", "Hút thuốc lá",      profile.SMOKING,           profile.SMOKING_REGULAR,           profile.SMOKING_QUIT, null,                          true),
                        MakeRiskFactor("2", "Uống rượu bia",     profile.ALCOHOL,           null,                              profile.ALCOHOL_QUIT, ToStr(profile.ALCOHOL_GLASS_NUM), true),
                        MakeRiskFactor("3", "Sử dụng ma túy",    profile.DRUG_USE,          profile.DRUG_REGULAR,              profile.DRUG_QUIT,    null,                          true),
                        MakeRiskFactor("4", "Hoạt động thể lực", profile.PHYSICAL_ACTIVITY, profile.PHYSICAL_ACTIVITY_REGULAR, null,                 null,                          false)
                    },
                    YeuToTiepXuc = profile.OCCUPATIONAL_EXPOSURE,
                    ThoiGianTiepXuc = profile.EXPOSURE_DURATION,
                    LoaiHoXiGd = profile.TOILET_TYPE,
                    Khac = profile.OTHER_RISK_FACTOR
                };
                ts.YeuToSucKhoe = y;
            }

            // KHUYETTAT: các dòng tích thuộc nhóm danh mục khuyết tật (DISEASE_TYPE=53).
            ts.KhuyetTat = BuildKhuyetTat(diseaseResults, diseaseDetailById);

            // TIEUSU_BENHTAT (bản thân): dị ứng (nhóm DIUNG) + bệnh tật (nhóm BENHTAT).
            ts.TieuSuBenhTat = BuildTieuSuBenhTat(diseaseResults, diseaseDetailById,
                DISEASE_TYPE_ID__DIUNG, DISEASE_TYPE_ID__BENHTAT);

            // TIENSU_GIADINH: cùng cơ chế, nhóm gia đình (dị ứng 52 / bệnh 51).
            // NGUOIMAC tách từ OTHER ("<mô tả> | Người mắc: <người>") — xem SplitOther.
            ts.TienSuGiaDinh = BuildTieuSuBenhTat(diseaseResults, diseaseDetailById,
                DISEASE_TYPE_ID__DIUNG_GIADINH, DISEASE_TYPE_ID__BENHTAT_GIADINH);

            // TIENSU_PHAUTHUAT: có đánh dấu "không phẫu thuật" -> "Không"; ngược lại lấy tiền sử/ghi chú phẫu thuật.
            if (general != null)
            {
                ts.TienSuPhauThuat = (ToInt(general.IS_NOT_SURGERY) == 1)
                    ? "Không"
                    : FirstNonEmpty(general.HISTORY_SURGERY, general.NOTE_SURGICAL);
            }

            // SUCKHOE_SINHSAN: chỉ số sinh sản (profile: số lần đẻ/phá thai/đủ tháng/non...; general: có thai/sảy/con sống/tránh thai).
            if (profile != null || general != null)
            {
                ts.SucKhoeSinhSan = new SucKhoeSinhSan
                {
                    BienPhapTranhThai = (general != null) ? general.NOTE_CONTRACEPTIVES : null,
                    KyThaiCuoi = (profile != null) ? profile.LAST_PREGNANCY_PERIOD : null,
                    SoLanCoThai = (general != null) ? ToStr(general.PREGNANCY) : null,
                    SoLanSayThai = (general != null) ? ToStr(general.ABORTUS) : null,
                    SoLanPhaThai = (profile != null) ? ToStr(profile.ABORTION_NUM) : null,
                    SoLanSinhDe = (general != null) ? ToStr(general.RECURRENT) : null,   // số lần sinh đẻ = RECURRENT
                    SoLanDeThuong = (profile != null) ? profile.NORMAL_DELIVERY : null,
                    SoLanDeMo = (profile != null) ? profile.CESAREAN_DELIVERY : null,
                    SoLanDeKho = (profile != null) ? profile.DIFFICULT_DELIVERY : null,
                    SoLanDeDuThang = (profile != null) ? ToStr(profile.FULL_TERM_BIRTH_NUM) : null,
                    SoLanDeNon = (profile != null) ? ToStr(profile.PRETERM_BIRTH_NUM) : null,
                    SoConHienSong = (general != null) ? ToStr(general.ALIVE) : null,
                    BenhPhuKhoa = (profile != null) ? profile.GYNECOLOGICAL_DISEASE : null
                };
            }

            // VANDEKHAC
            if (profile != null) ts.VanDeKhac = profile.OTHER_ISSUE;

            return ts;
        }

        /// <summary>
        /// 1 yếu tố nguy cơ (&lt;DSYEUTO&gt;). applyDaBo=true (Thuốc/Rượu/Ma túy): DABO=đã bỏ(*_QUIT), DABO=0 -&gt; TRANGTHAI=0.
        /// applyDaBo=false (Thể chất): không có DABO. TRANGTHAI: 0/1/2 (2=thường xuyên nếu có cờ regular).
        /// </summary>
        private static DsYeuTo MakeRiskFactor(string loai, string tenLoai, object mainFlag, object regularFlag,
            object quitFlag, string soLuong, bool applyDaBo)
        {
            var y = new DsYeuTo { Loai = loai, TenLoai = tenLoai, SoLuong = soLuong };
            if (applyDaBo)
            {
                string daBo = ToFlag(quitFlag);                          // 1 = đã bỏ, 0 = không dùng
                y.DaBo = daBo;
                y.TrangThai = (daBo == "0") ? "0" : RiskLevel(mainFlag, regularFlag);
            }
            else
            {
                y.DaBo = null;                                           // thể chất: không áp dụng "đã bỏ"
                y.TrangThai = RiskLevel(mainFlag, regularFlag);
            }
            return y;
        }

        /// <summary>Mức độ 0/1/2: lấy từ cờ chính (kẹp 0..2); nếu &lt;2 mà có cờ "thường xuyên" -&gt; 2.</summary>
        private static string RiskLevel(object mainFlag, object regularFlag)
        {
            int lvl = ToInt(mainFlag);
            if (lvl > 2) lvl = 2;
            if (lvl < 0) lvl = 0;
            if (lvl < 2 && ToInt(regularFlag) == 1) lvl = 2;
            return lvl.ToString();
        }

        /// <summary>
        /// Dựng &lt;KHUYETTAT&gt; từ HIS_DISEASE_DETAIL_RESULT (dòng tích, IS_CHECK=1) thuộc nhóm khuyết tật
        /// (HIS_DISEASE_DETAIL.DISEASE_TYPE_ID = 53). LOAI = NUM_ORDER (1..7), TENLOAI = NAME, MOTA = OTHER.
        /// </summary>
        private static List<DsKhuyetTat> BuildKhuyetTat(
            List<HIS_DISEASE_DETAIL_RESULT> results, IDictionary<long, HIS_DISEASE_DETAIL> detailById)
        {
            if (results == null || detailById == null) return null;
            var list = new List<DsKhuyetTat>();
            foreach (var r in results)
            {
                if (r == null || ToInt(r.IS_CHECK) != 1) continue;          // chỉ mục được tích
                long did = r.DISEASE_DETAIL_ID ?? 0;
                if (did <= 0) continue;
                HIS_DISEASE_DETAIL d;
                if (!detailById.TryGetValue(did, out d) || d == null) continue;
                if ((d.DISEASE_TYPE_ID ?? 0) != DISEASE_TYPE_ID__KHUYET_TAT) continue;   // chỉ nhóm khuyết tật (53)
                list.Add(new DsKhuyetTat { Loai = ToStr(d.NUM_ORDER), TenLoai = d.NAME, MoTa = r.OTHER });
            }
            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Dựng khối dị ứng + bệnh tật (dùng cho &lt;TIEUSU_BENHTAT&gt; bản thân; tái dùng được cho gia đình).
        /// DIUNG: dòng tích nhóm allergyTypeId -&gt; DsDiUng (LOAI=NUM_ORDER 1..4, TEN=NAME, MOTA=OTHER).
        /// BENHTAT: dòng tích nhóm diseaseTypeId -&gt; DsBenh (LOAIBENH=NUM_ORDER 1..15, TRANGTHAI=IS_CHECK, TENBENH=NAME, MOTA=OTHER).
        /// </summary>
        private static TieuSuBenhTat BuildTieuSuBenhTat(
            List<HIS_DISEASE_DETAIL_RESULT> results, IDictionary<long, HIS_DISEASE_DETAIL> detailById,
            long allergyTypeId, long diseaseTypeId)
        {
            if (results == null || detailById == null) return null;

            List<DsDiUng> diUng = null;
            List<DsBenh> benhTat = null;

            foreach (var pair in CheckedRowsInGroup(results, detailById, allergyTypeId))
            {
                if (diUng == null) diUng = new List<DsDiUng>();
                string moTa, nguoiMac;
                SplitOther(pair.Value.OTHER, out moTa, out nguoiMac);   // gia đình: OTHER = "<mô tả> | Người mắc: <người>"
                diUng.Add(new DsDiUng
                {
                    Loai = ToStr(pair.Key.NUM_ORDER),
                    MaSo = null,
                    Ten = pair.Key.NAME,
                    MoTa = moTa,
                    NguoiMac = nguoiMac
                });
            }
            foreach (var pair in CheckedRowsInGroup(results, detailById, diseaseTypeId))
            {
                if (benhTat == null) benhTat = new List<DsBenh>();
                string moTa, nguoiMac;
                SplitOther(pair.Value.OTHER, out moTa, out nguoiMac);
                benhTat.Add(new DsBenh
                {
                    LoaiBenh = ToStr(pair.Key.NUM_ORDER),
                    TenBenh = pair.Key.NAME,
                    TrangThai = ToFlag(pair.Value.IS_CHECK),   // 1 = đang mắc
                    MoTa = moTa,
                    NguoiMac = nguoiMac
                });
            }

            if (diUng == null && benhTat == null) return null;
            return new TieuSuBenhTat { DiUng = diUng, BenhTat = benhTat };
        }

        /// <summary>Các dòng ĐÃ TÍCH (IS_CHECK=1) thuộc 1 nhóm HIS_DISEASE_TYPE. typeId &lt;= 0 -&gt; rỗng.</summary>
        private static IEnumerable<KeyValuePair<HIS_DISEASE_DETAIL, HIS_DISEASE_DETAIL_RESULT>> CheckedRowsInGroup(
            List<HIS_DISEASE_DETAIL_RESULT> results, IDictionary<long, HIS_DISEASE_DETAIL> detailById, long typeId)
        {
            var rows = new List<KeyValuePair<HIS_DISEASE_DETAIL, HIS_DISEASE_DETAIL_RESULT>>();
            if (results == null || detailById == null || typeId <= 0) return rows;
            foreach (var r in results)
            {
                if (r == null || ToInt(r.IS_CHECK) != 1) continue;
                long did = r.DISEASE_DETAIL_ID ?? 0;
                if (did <= 0) continue;
                HIS_DISEASE_DETAIL d;
                if (!detailById.TryGetValue(did, out d) || d == null) continue;
                if ((d.DISEASE_TYPE_ID ?? 0) != typeId) continue;
                rows.Add(new KeyValuePair<HIS_DISEASE_DETAIL, HIS_DISEASE_DETAIL_RESULT>(d, r));
            }
            return rows;
        }
        #endregion

        #region DANHSACHHOSOKHAMCHUABENH
        /// <summary>
        /// Danh sách lượt khám -&gt; &lt;DANHSACHHOSOKHAMCHUABENH&gt;. Mỗi HIS_SERVICE_REQ = 1 &lt;HOSOKHAMCHUABENH&gt;.
        /// generalBySrId: HIS_KSK_GENERAL theo SERVICE_REQ_ID; dhstById: HIS_DHST theo ID (lấy theo DHST_ID);
        /// clsByTreatmentId: danh sách CLS (&lt;DICHVU&gt;) theo TREATMENT_ID (dựng từ BuildClsByTreatment).
        /// </summary>
        internal static List<HoSoKhamChuaBenh> BuildHoSoKhamChuaBenhList(
            IEnumerable<HIS_SERVICE_REQ> serviceReqs,
            IDictionary<long, HIS_KSK_GENERAL> generalBySrId,
            IDictionary<long, HIS_DHST> dhstById,
            IDictionary<long, List<DichVu>> clsByTreatmentId,
            HIS_KSK_PROFILE profile)
        {
            var list = new List<HoSoKhamChuaBenh>();
            if (serviceReqs == null) return list;

            foreach (var sr in serviceReqs)
            {
                if (sr == null) continue;
                HIS_KSK_GENERAL general = null;
                if (generalBySrId != null) generalBySrId.TryGetValue(sr.ID, out general);

                HIS_DHST dhst = null;
                long dhstId = (general != null && general.DHST_ID.HasValue) ? general.DHST_ID.Value : (sr.DHST_ID ?? 0);
                if (dhstById != null && dhstId > 0) dhstById.TryGetValue(dhstId, out dhst);

                List<DichVu> cls = null;
                long tr = sr.TREATMENT_ID;
                if (clsByTreatmentId != null && tr > 0) clsByTreatmentId.TryGetValue(tr, out cls);

                list.Add(BuildHoSoKhamChuaBenh(sr, general, dhst, cls, profile));
            }
            return list;
        }

        /// <summary>1 lượt khám -&gt; &lt;HOSOKHAMCHUABENH&gt;. profile: cho TC_TT_VD (CLINICAL_DEVELOPMENT_ASSESS).</summary>
        internal static HoSoKhamChuaBenh BuildHoSoKhamChuaBenh(HIS_SERVICE_REQ sr, HIS_KSK_GENERAL general, HIS_DHST dhst, List<DichVu> canLamSang, HIS_KSK_PROFILE profile)
        {
            var h = new HoSoKhamChuaBenh
            {
                // Hành chính lượt khám
                MaLk = sr.TDL_TREATMENT_CODE,   // TODO: dựng MA_LK chuẩn BHYT (STT/năm_maCSKCB_loai_maLK) nếu cần
                NgayKham = FormatDate(sr.INTRUCTION_TIME),
                NgayBatDau = FormatDate(sr.START_TIME),
                NgayKetThuc = FormatDate(sr.FINISH_TIME),
                MaBacSi = sr.EXECUTE_LOGINNAME,
                BacSiKham = sr.EXECUTE_USERNAME,
                LyDoKham = sr.HOSPITALIZATION_REASON,
                BenhSu = sr.PATHOLOGICAL_PROCESS,
                TuVan = !string.IsNullOrEmpty(sr.CONCLUSION_CONSULTATION) ? sr.CONCLUSION_CONSULTATION : sr.ADVISE
            };

            // Sinh hiệu (HIS_DHST)
            if (dhst != null)
            {
                h.Mach = ToStr(dhst.PULSE);
                h.NhietDo = ToStr(dhst.TEMPERATURE);
                h.HuyetApTt = ToStr(dhst.BLOOD_PRESSURE_MAX);
                h.HuyetApTd = ToStr(dhst.BLOOD_PRESSURE_MIN);
                h.NhipTho = ToStr(dhst.BREATH_RATE);
                h.ChieuCao = ToStr(dhst.HEIGHT);
                h.ChiSoBmi = ToStr(dhst.VIR_BMI);
                h.CanNang = ToStr(dhst.WEIGHT);
                h.VongBung = ToStr(dhst.BELLY);
            }

            // Khám lâm sàng (HIS_KSK_GENERAL)
            if (general != null)
            {
                h.MatPhai = general.EXAM_EYESIGHT_RIGHT;
                h.MatTrai = general.EXAM_EYESIGHT_LEFT;
                // SUDUNGKINH: có giá trị kính (thị lực có kính) -> "1" (có đeo kính), ngược lại "0".
                h.SuDungKinh = (!string.IsNullOrEmpty(general.EXAM_EYESIGHT_GLASS_LEFT)
                                || !string.IsNullOrEmpty(general.EXAM_EYESIGHT_GLASS_RIGHT)) ? "1" : "0";
                h.KhamNiemMac = general.BODY_SKIN;   // khám niêm mạc lưu ở BODY_SKIN
                h.KhamToanThanKhac = general.BODY_OTHER;
                h.KhamTimMach = general.EXAM_CIRCULATION;
                h.KhamHoHap = general.EXAM_RESPIRATORY;
                h.KhamTieuHoa = general.EXAM_DIGESTION;
                h.KhamTietNieu = general.EXAM_KIDNEY_UROLOGY;
                h.KhamCoXuongKhop = general.EXAM_MUSCLE_BONE;
                h.KhamNoiTiet = general.EXAM_OEND;
                h.KhamThanKinh = general.EXAM_NEUROLOGICAL;
                h.KhamTamThan = general.EXAM_MENTAL;
                h.KhamNgoaiKhoa = general.EXAM_SURGERY;
                h.KhamPhuKhoa = general.EXAM_OBSTETRIC;
                h.KhamTaiMuiHong = general.EXAM_ENT;
                h.KhamRhm = general.EXAM_STOMATOLOGY;
                h.KhamMat = general.EXAM_EYE;
                h.KhamDaLieu = general.EXAM_DERMATOLOGY;
                h.KhamDinhDuong = general.EXAM_NUTRION;
                h.KhamVanDong = general.EXAM_OCCUPATIONAL_THERAPY;
                h.Khac = general.EXAM_OTHER;
            }

            // TC_TT_VD: lưu ở HIS_KSK_PROFILE.CLINICAL_DEVELOPMENT_ASSESS.
            if (profile != null) h.TcTtVd = profile.CLINICAL_DEVELOPMENT_ASSESS;

            // CANLAMSANG: chỉ dịch vụ Xét nghiệm + Chẩn đoán hình ảnh, mã/tên theo BHYT (xem BuildClsByTreatment).
            h.CanLamSang = (canLamSang != null && canLamSang.Count > 0) ? canLamSang : null;

            // Chẩn đoán / kết luận (ưu tiên HIS_KSK_GENERAL, fallback HIS_SERVICE_REQ)
            string maBenh = (general != null) ? general.CONCLUSION_ICD_CODE : null;
            string tenBenh = (general != null) ? general.CONCLUSION_ICD_NAME : null;
            string ketLuan = (general != null) ? general.DISEASES : null;
            if (string.IsNullOrEmpty(maBenh)) maBenh = sr.ICD_CODE;
            if (string.IsNullOrEmpty(tenBenh)) tenBenh = sr.ICD_NAME;
            if (string.IsNullOrEmpty(ketLuan)) ketLuan = !string.IsNullOrEmpty(sr.CONCLUSION) ? sr.CONCLUSION : tenBenh;
            if (!string.IsNullOrEmpty(maBenh) || !string.IsNullOrEmpty(tenBenh) || !string.IsNullOrEmpty(ketLuan))
            {
                h.ChanDoanKetLuan = new List<ChanDoanBenh>
                {
                    new ChanDoanBenh { MaBenh = maBenh, TenBenh = tenBenh, KetLuan = ketLuan }
                };
            }
            return h;
        }
        #endregion

        #region CANLAMSANG (CLS)
        /// <summary>
        /// Dựng danh sách CLS (&lt;DICHVU&gt;) theo TREATMENT_ID. CHỈ lấy dịch vụ loại BHYT là
        /// Xét nghiệm (HEIN_SERVICE_TYPE = XN) và Chẩn đoán hình ảnh (CDHA), đã thực hiện (IS_NO_EXECUTE null).
        /// MADICHVU/TENDICHVU = mã/tên theo BHYT (TDL_HEIN_SERVICE_BHYT_CODE/NAME); MANHOM: XN=1, CDHA=2.
        /// KETQUA: XN = giá trị chỉ số (TEIN.VALUE, nhiều chỉ số nối bằng "; "); CDHA = kết luận (CONCLUDE) hoặc mô tả (DESCRIPTION).
        /// </summary>
        internal static Dictionary<long, List<DichVu>> BuildClsByTreatment(
            List<V_HIS_SERE_SERV_2> sereServs, List<V_HIS_SERE_SERV_TEIN> teins)
        {
            var result = new Dictionary<long, List<DichVu>>();
            if (sereServs == null || sereServs.Count == 0) return result;

            long xn = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__XN;
            long cdha = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CDHA;

            // Gom chỉ số XN theo SERE_SERV_ID.
            var teinBySs = new Dictionary<long, List<V_HIS_SERE_SERV_TEIN>>();
            if (teins != null)
                foreach (var te in teins)
                {
                    if (te == null) continue;
                    long k = te.SERE_SERV_ID;
                    if (k <= 0) continue;
                    List<V_HIS_SERE_SERV_TEIN> l;
                    if (!teinBySs.TryGetValue(k, out l)) { l = new List<V_HIS_SERE_SERV_TEIN>(); teinBySs[k] = l; }
                    l.Add(te);
                }

            foreach (var ss in sereServs)
            {
                if (ss == null || ss.IS_NO_EXECUTE != null) continue;   // chỉ dịch vụ ĐÃ thực hiện
                long typeId = ss.TDL_HEIN_SERVICE_TYPE_ID ?? 0;
                if (typeId != xn && typeId != cdha) continue;           // chỉ XN / CDHA
                long tr = ss.TDL_TREATMENT_ID ?? 0;
                if (tr <= 0) continue;

                string ketQua;
                if (typeId == xn)
                {
                    // XN: nối giá trị các chỉ số; nếu không có chỉ số -> lấy kết luận/mô tả của dịch vụ.
                    List<V_HIS_SERE_SERV_TEIN> ssTeins;
                    teinBySs.TryGetValue(ss.ID, out ssTeins);
                    if (ssTeins != null && ssTeins.Count > 0)
                    {
                        var vals = ssTeins.Where(x => x != null && !string.IsNullOrEmpty(x.VALUE)).Select(x => x.VALUE.Trim());
                        ketQua = string.Join("; ", vals.ToArray());
                    }
                    else ketQua = "";
                    if (string.IsNullOrEmpty(ketQua)) ketQua = FirstNonEmpty(ss.CONCLUDE, ss.DESCRIPTION);
                }
                else
                {
                    // CDHA: kết luận (CONCLUDE) ưu tiên, fallback mô tả (DESCRIPTION).
                    ketQua = FirstNonEmpty(ss.CONCLUDE, ss.DESCRIPTION);
                }

                List<DichVu> rows;
                if (!result.TryGetValue(tr, out rows)) { rows = new List<DichVu>(); result[tr] = rows; }
                rows.Add(new DichVu
                {
                    MaDichVu = ss.TDL_HEIN_SERVICE_BHYT_CODE,
                    MaNhom = (typeId == xn) ? "1" : "2",
                    TenDichVu = ss.TDL_HEIN_SERVICE_BHYT_NAME,
                    KetQua = ketQua
                });
            }
            return result;
        }
        #endregion

        #region Helpers
        /// <summary>Số thời gian yyyyMMddHHmmss / yyyyMMdd -&gt; "dd/MM/yyyy". Null/0 -&gt; null.</summary>
        private static string FormatDate(object o)
        {
            try
            {
                if (o == null) return null;
                long v = Convert.ToInt64(o);
                if (v <= 0) return null;
                string s = v.ToString();
                if (s.Length < 8) return null;
                return s.Substring(6, 2) + "/" + s.Substring(4, 2) + "/" + s.Substring(0, 4);
            }
            catch { return null; }
        }

        private static string ToStr(object o)
        {
            return o == null ? null : o.ToString();
        }

        private static int ToInt(object o)
        {
            try { return o == null ? 0 : Convert.ToInt32(o); }
            catch { return 0; }
        }

        /// <summary>Cờ Có/Không: giá trị = 1 -&gt; "1" (Có), ngược lại -&gt; "0" (Không).</summary>
        private static string ToFlag(object o)
        {
            return (ToInt(o) == 1) ? "1" : "0";
        }

        /// <summary>
        /// Tách OTHER (HIS_DISEASE_DETAIL_RESULT) dạng "&lt;mô tả&gt; | Người mắc: &lt;người mắc&gt;" thành mô tả + người mắc.
        /// Không có phần "Người mắc:" -&gt; toàn bộ là mô tả, người mắc = null (tiền sử bản thân).
        /// </summary>
        private static void SplitOther(string other, out string moTa, out string nguoiMac)
        {
            moTa = other;
            nguoiMac = null;
            if (string.IsNullOrEmpty(other)) return;
            const string MARKER = "Người mắc:";
            int idx = other.IndexOf(MARKER, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return;
            nguoiMac = other.Substring(idx + MARKER.Length).Trim();
            string left = other.Substring(0, idx).TrimEnd();
            if (left.EndsWith("|")) left = left.Substring(0, left.Length - 1).TrimEnd();
            moTa = left;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            if (values != null)
                foreach (var v in values)
                    if (!string.IsNullOrEmpty(v)) return v;
            return "";
        }

        /// <summary>
        /// Map TÊN tôn giáo (HIS_PATIENT.RELIGION_NAME) -&gt; MÃ theo danh mục (1..15). Không khớp -&gt; null.
        /// Thứ tự theo danh mục nghiệp vụ (16 tôn giáo được công nhận).
        /// </summary>
        private static string MapMaTonGiao(string religionName)
        {
            if (string.IsNullOrWhiteSpace(religionName)) return null;
            string n = religionName.Trim();
            if (EqName(n, "Phật giáo")) return "1";
            else if (EqName(n, "Công giáo")) return "2";
            else if (EqName(n, "Tin Lành")) return "3";
            else if (EqName(n, "Cao Đài")) return "4";
            else if (EqName(n, "Phật giáo Hòa Hảo")) return "5";
            else if (EqName(n, "Hồi giáo")) return "6";
            else if (EqName(n, "Tôn giáo Baha'i")) return "7";
            else if (EqName(n, "Tịnh độ Cư sỹ Phật hội Việt Nam")) return "8";
            else if (EqName(n, "Đạo Tứ Ân Hiếu Nghĩa")) return "9";
            else if (EqName(n, "Bửu Sơn Kỳ Hương")) return "10";
            else if (EqName(n, "Giáo hội Phật đường Nam Tông Minh Sư đạo")) return "11";
            else if (EqName(n, "Hội thánh Minh lý đạo - Tam Tông Miếu")) return "12";
            else if (EqName(n, "Chăm Bà la môn")) return "13";
            else if (EqName(n, "Giáo hội Các Thánh hữu Ngày sau của Chúa Giê Su Ky Tô (Mormon)")) return "14";
            else if (EqName(n, "Phật giáo Hiếu Nghĩa Tà Lơn")) return "15";
            else if (EqName(n, "Giáo hội Cơ đốc Phục lâm Việt Nam")) return "16";
            else return null;
        }

        private static bool EqName(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}
