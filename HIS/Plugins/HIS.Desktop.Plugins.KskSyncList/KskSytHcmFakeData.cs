/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * ======================= TỆP TẠM — XÓA KHI XONG KIỂM THỬ =======================
 *
 * Dữ liệu GIẢ để thử đẩy bản tin mẫu M3 lên Nền tảng KSK Sở Y tế TP.HCM khi phần lấy dữ liệu
 * thật chưa xong. Toàn bộ tệp này là tạm:
 *
 *   · BuildCanLamSan()  — khối cận lâm sàng: KẾT QUẢ của 34 chỉ tiêu (14 xét nghiệm máu,
 *                         5 sinh hóa máu, 11 nước tiểu, 4 tầm soát nữ) + chẩn đoán hình ảnh
 *                         + cận lâm sàng khác, và bản thứ hai cho khám định kỳ (tiền tố kskdk_).
 *   · BuildFullBody()   — CẢ bản tin 6 khối bằng dữ liệu giả, để thử đẩy end-to-end.
 *
 * KIỂU DỮ LIỆU LẤY THEO ĐÚNG VÍ DỤ BODY của đặc tả, không theo bảng chi tiết, vì hai mục đó
 * ghi khác nhau ở nhóm nước tiểu (ví dụ body gửi SỐ, bảng chi tiết ghi chuỗi 20 ký tự). Bản
 * ví dụ là bản Sở đã chạy thật nên gửi sai kiểu dễ bị từ chối hơn.
 *
 * KHÔNG có dữ liệu bệnh nhân thật: tên, số định danh, ngày sinh đều bịa; số định danh dùng
 * dải 0000... để không trùng người thật.
 *
 * CÁCH XÓA KHI HẾT DÙNG:
 *   1. Xóa tệp này và mục <Compile Include="KskSytHcmFakeData.cs" /> trong csproj.
 *   2. Trong KskSytHcmBodyBuilder.Build, bỏ tham số `fakeParaclinical` và nhánh gọi tới đây.
 *   3. Trong frmKskSytClsMap, bỏ nút "Đẩy thử hồ sơ giả".
 * Trình biên dịch sẽ chỉ ra hết chỗ còn tham chiếu, không sợ sót.
 */
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.KskSyncList
{
    internal static class KskSytHcmFakeData
    {
        /// <summary>Id danh mục Âm tính / Dương tính của cổng — dùng cho chỉ tiêu Nitrit nước tiểu.</summary>
        private const int SYT_AM_TINH = 5120;

        /// <summary>
        /// Khối cận lâm sàng — KẾT QUẢ của 34 chỉ tiêu, giá trị nằm trong khoảng bình thường
        /// của người khỏe để cổng không báo lỗi khoảng giá trị.
        /// </summary>
        internal static Dictionary<string, object> BuildCanLamSan()
        {
            var b = new Dictionary<string, object>();

            #region 14 chỉ tiêu xét nghiệm máu — ví dụ body gửi SỐ

            b["xnm_slhc"] = 4.5m;               // số lượng hồng cầu   T/l
            b["xnm_huyetsacto"] = 140m;         // huyết sắc tố        g/l
            b["xnm_hematocrit"] = 42m;          // hematocrit          %
            b["xnm_mcv"] = 90m;                 // thể tích trung bình hồng cầu  fl
            b["xnm_mch"] = 30m;                 // lượng Hb trung bình hồng cầu  pg
            b["xnm_mchc"] = 330m;               // nồng độ Hb trung bình         g/l
            b["xnm_rdw"] = 12.5m;               // độ phân bố hồng cầu           %
            b["xnm_slbc"] = 7.2m;               // số lượng bạch cầu             G/l
            // 5 loại bạch cầu — đặc tả yêu cầu SỐ LƯỢNG TUYỆT ĐỐI (G/l), không phải tỉ lệ %.
            b["xnm_slbc_trungtinh"] = 4.1m;
            b["xnm_slbc_lympho"] = 2.2m;
            b["xnm_slbc_donnhan"] = 0.5m;
            b["xnm_slbc_aitoan"] = 0.3m;
            b["xnm_slbc_aikiem"] = 0.1m;
            b["xnm_sltc"] = 250m;               // số lượng tiểu cầu             G/l

            #endregion

            #region 5 chỉ tiêu sinh hóa máu — ví dụ body gửi CHUỖI

            b["shm_duongmau"] = "5.1";          // đường máu     mmol/l
            b["shm_ure"] = "4.2";               // ure           mmol/l
            b["shm_creatinin"] = "80";          // creatinin     µmol/l
            b["shm_asat_got"] = "20";           // ASAT/GOT      U/l
            b["shm_alat_gpt"] = "25";           // ALAT/GPT      U/l

            #endregion

            #region 11 chỉ tiêu nước tiểu

            // Ví dụ body gửi SỐ cho cả nhóm này, bảng chi tiết lại ghi chuỗi 20 ký tự.
            // Theo ví dụ body. Lưu ý khi gắn dữ liệu thật: kết quả nước tiểu ở HIS thường là
            // chữ ("âm tính", "vết") — nếu cổng thật chỉ nhận số thì phải quy ước lại với Sở.
            b["xnnt_titrong"] = 1.015m;         // tỉ trọng
            b["xnnt_ph"] = 6.0m;                // pH
            b["xnnt_bachcau"] = 0m;             // bạch cầu
            b["xnnt_hongcau"] = 0m;             // hồng cầu
            // Nitrit là chỉ tiêu DUY NHẤT của nhóm này nhận Id danh mục: 5120 Âm tính / 5119 Dương tính.
            b["xnnt_nitrit"] = SYT_AM_TINH;
            b["xnnt_protein"] = 0m;             // protein
            b["xnnt_glucose"] = 0m;             // glucose
            b["xnnt_cetonic"] = 0m;             // thể cetonic
            b["xnnt_bilirubin"] = 0m;           // bilirubin
            b["xnnt_urobilinogen"] = 0m;        // urobilinogen
            b["xnnt_khac"] = "Trong giới hạn bình thường";

            #endregion

            #region Chẩn đoán hình ảnh · cận lâm sàng khác

            b["chuan_doan_hinh_anh"] = "X-quang tim phổi thẳng: không thấy bất thường";
            b["can_lam_sang_khac"] = 0m;
            b["can_lam_sang_khac_chi_tiet"] = "";

            #endregion

            #region Bản thứ hai cho khám sức khỏe định kỳ — cùng chỉ tiêu, tiền tố kskdk_

            // Đặc tả có HAI bộ cận lâm sàng trùng chỉ số: một bộ cho "đi học, đi làm" và một bộ
            // cho "khám định kỳ". HIS KHÔNG có cờ phân biệt hồ sơ thuộc bộ nào — khi gắn dữ liệu
            // thật phải chốt quy tắc với Sở, xem báo cáo. Ở đây điền cả hai bộ giống nhau để
            // bản tin đủ trường.
            foreach (var kv in new Dictionary<string, object>(b))
            {
                if (kv.Key.StartsWith("xnm_") || kv.Key.StartsWith("shm_")
                    || kv.Key.StartsWith("xnnt_") || kv.Key == "chuan_doan_hinh_anh")
                {
                    b["kskdk_" + kv.Key] = kv.Value;
                }
            }

            #endregion

            #region 4 chỉ tiêu tầm soát riêng nữ lao động — Phụ lục XXV Thông tư 32/2023/TT-BYT

            b["xet_nghiem_te_bao_co_tu_cung"] = "Không thấy tế bào bất thường";
            b["xet_nghiem_hpv"] = "Âm tính";
            b["xquang_nhu"] = "Không thấy bất thường";
            b["sieu_am_2_tuyen_vu"] = "Không thấy bất thường";

            #endregion

            Inventec.Common.Logging.LogSystem.Warn(
                "SytHcm: khoi CAN LAM SAN dang dung DU LIEU GIA (KskSytHcmFakeData) — "
                + b.Count + " truong. Chi dung de kiem thu, KHONG duoc dung o moi truong that.");
            return b;
        }

        /// <summary>
        /// Cả bản tin 6 khối bằng dữ liệu giả — để thử đẩy end-to-end khi chưa có hồ sơ thật.
        /// Các trường cần Id danh mục của cổng thì lấy đúng Id CÓ THẬT trong danh mục đã tải về,
        /// để cổng không báo "Id không tồn tại" và ta biết chắc lỗi (nếu có) nằm ở chỗ khác.
        /// </summary>
        internal static object BuildFullBody()
        {
            Inventec.Common.Logging.LogSystem.Warn(
                "SytHcm: dang gui HO SO GIA de kiem thu — khong phai du lieu benh nhan that");

            var tthc = new Dictionary<string, object>();
            tthc["ngay_kham"] = DateTime.Now.ToString("yyyy-MM-dd");
            tthc["doi_tuong_kham"] = "3";
            tthc["dia_diem_kham"] = 4052;                       // Cơ Sở Khám Chữa Bệnh
            tthc["dinh_danh_ca_nhan"] = "001099000001";
            tthc["ho_ten"] = "Nguyễn Văn Kiểm Thử";
            tthc["ngay_sinh"] = "1990-01-01";
            tthc["gioi_tinh"] = KskSytHcmBodyBuilder.MapByName("GioiTinh", "Nam");
            tthc["the_bhyt"] = "";
            tthc["dan_toc_id"] = KskSytHcmBodyBuilder.MapByName("DanToc", "Kinh");
            tthc["sdt"] = "0900000000";
            tthc["nhom_mau_id"] = null;
            tthc["yeu_to_nhom_mau_id"] = null;
            tthc["dia_chi_hien_tai"] = "Số 1, Đường Kiểm Thử";
            tthc["ward_id"] = null;
            tthc["ward_code"] = "";
            tthc["nghenghiep_id"] = null;
            tthc["nghenghiep_code"] = "";
            tthc["noi_cong_tac"] = 1;
            tthc["noi_cong_tac_xa_phuong"] = null;
            tthc["hinh_thuc_chi_tra_khamsk"] = 4041;            // Người dân tự chi trả
            tthc["hinh_thuc_chi_tra_khamsk_chi_tiet"] = 5133;   // Tự thực hiện
            tthc["nguonkhac_ghiro"] = "";
            tthc["ly_do_kham"] = "Kiểm thử kết nối";

            var dsBenh = new Dictionary<string, object>();
            foreach (string f in new string[]
            {
                "benh_5nam","benh_than_kinh","benh_mat","benh_tai","benh_tim","pt_tim_mach",
                "tang_ha","kho_tho","benh_phoi","benh_than","nghien_ruou_bia","dai_thao_duong",
                "benh_tam_than","mat_y_thuc","ngat_chong_mat","benh_tieu_hoa","roi_loan_giac_ngu",
                "tai_bien_mach_mau_nao","cot_song","su_dung_ruou_bia","su_dung_ma_tuy"
            }) dsBenh[f] = 0;
            dsBenh["benh_khac"] = "";

            var tienSu = new Dictionary<string, object>();
            tienSu["giadinh_macbenh"] = 0;
            tienSu["giadinh_danhsachbenh"] = null;
            tienSu["giadinh_danhsachbenh_icd"] = null;
            tienSu["giadinh_macbenh_tenbenh"] = "";
            tienSu["ds_benh_ban_than"] = dsBenh;
            tienSu["dieu_tri_benh_co_khong"] = 0;
            tienSu["dieu_tri_benh_liet_ke"] = "";
            tienSu["thai_san_co_khong"] = 0;
            tienSu["thai_san_liet_ke"] = "";

            var theLuc = new Dictionary<string, object>();
            theLuc["chieucao"] = 170m;
            theLuc["cannang"] = 65m;
            theLuc["bmi"] = 22.5m;
            theLuc["mach"] = 75m;
            theLuc["huyetaptamthu"] = 120m;
            theLuc["huyetaptamtruong"] = 80m;
            theLuc["nhiptho"] = 18m;
            theLuc["phanloai"] = 1016;                          // Loại I

            // 15 mục khám: bình thường hết, KHÔNG gửi mã bệnh nào — chỉ tiêu mã bệnh còn chờ Sở
            // xác nhận nhận Id danh mục hay nhận mã ICD.
            var lamSang = new Dictionary<string, object>();
            foreach (string k in new string[]
            {
                "noikhoa","hohap","tieuhoa","thantietnieu","noitiet","coxuongkhop","thankinh",
                "tamthan","ngoaikhoa","dalieu","sankhoa","phukhoa","mat","tmh","rhm"
            })
            {
                lamSang[k + "_chuaphathienbatthuong"] = 1;
                lamSang[(k == "thankinh") ? "thankinh_chuandoansobo" : k + "_chandoansobo"] = 0;
                lamSang[k + "_chandoansobo_icd"] = "";
                lamSang[k + "_chandoanxacdinh"] = 0;
                lamSang[k + "_chandoanxacdinh_icd"] = "";
                lamSang[k + "_phanloai"] = 1016;
            }
            foreach (string k in new string[]
            {
                "mat_khongkinh_mp","mat_khongkinh_mt","mat_kinhlo_mp","mat_kinhlo_mt",
                "mat_cokinh_mp","mat_cokinh_mt","mat_docau_mp","mat_docau_mt",
                "mat_dotru_mp","mat_dotru_mt","mat_truc_mp","mat_truc_mt"
            }) lamSang[k] = 10m;
            lamSang["tmh_taitrai_noithuong"] = 5m;
            lamSang["tmh_taitrai_noitham"] = 5m;
            lamSang["tmh_taiphai_noithuong"] = 5m;
            lamSang["tmh_taiphai_noitham"] = 5m;
            // Sơ đồ răng: Id 191 = Bình thường trong danh mục tình trạng răng của cổng.
            lamSang["chi_tiet_kham_rang"] = new Dictionary<string, object> { { "11", 191 }, { "16", 191 } };

            return new
            {
                tthc = tthc,
                tien_su = tienSu,
                kham_the_luc = theLuc,
                kham_lam_san = lamSang,
                can_lam_san = BuildCanLamSan(),
                ket_luan = new Dictionary<string, object> { { "de_nghi", "Kiểm thử kết nối" } }
            };
        }
    }
}
