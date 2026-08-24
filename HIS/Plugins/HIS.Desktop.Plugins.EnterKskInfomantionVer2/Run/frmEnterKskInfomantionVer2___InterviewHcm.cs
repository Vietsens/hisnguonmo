/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tab "Hỏi bệnh lâm sàng HCM" — mục D của Mẫu 4 (TT25) do Sở Y tế TP.HCM ban hành, đặt cạnh tab
 * "Khám lâm sàng HCM". CHỈ dựng khi viện đã khai báo cấu hình cổng.
 *
 * NGUỒN CHUẨN LÀ BIỂU MẪU IN, không phải bản tin của cổng: thứ tự mục, câu chữ và mã câu (D1.1,
 * D2.3...) bám đúng tờ giấy bác sĩ đang dùng, để người nhập dò theo mẫu mà tích.
 *
 * ĐỢT NÀY CHỈ DỰNG GIAO DIỆN — chưa lưu, chưa nạp, chưa đẩy cổng. Mỗi ô đã mang sẵn tên trường
 * trong `Tag`, nên phần lưu/nạp sau này chỉ việc duyệt ô.
 *
 * LUẬT KHOÁ/MỞ lấy nguyên văn từ biểu mẫu, gom vào một BẢNG LUẬT thay vì rải if lẻ tẻ:
 *   D1.2 Có  -> bỏ qua D2      D1.3 Có  -> bỏ qua D3      D1.4 Có  -> bỏ qua D4
 *   D1.12 Có -> bỏ qua D6      D1.13 Có -> bỏ qua D7      D1.14 Có -> bỏ qua D8.5
 *   D8.5.1 Có -> mới hỏi D8.5.2, D8.5.3
 *   D5 (ung thư) KHÔNG có điều kiện — hỏi mọi người.
 * Nhóm bị khoá thì XOÁ TRẮNG, vì theo mẫu đó là phần "bỏ qua", không được mang câu trả lời cũ.
 *
 * KHÁC BIỆT GIỮA MẪU IN VÀ BẢN TIN CỔNG (đã báo người yêu cầu, chờ Sở trả lời):
 *   - D3 tầm soát phổi tắc nghẽn mạn tính: mẫu in có 3 câu, bản tin cổng KHÔNG có nhóm tương ứng.
 *     Vẫn dựng đủ 3 ô để in được tờ giấy; tên trường tạm đặt tiền tố `bpq_`, chưa đẩy cổng.
 *   - Nhồi máu cơ tim và Đột quỵ: mẫu in KHÔNG có ô nơi điều trị, nên tab cũng không dựng, dù bản
 *     tin cổng có `benh_nhoimau_cskcb` / `benh_dotqui_cskcb`.
 *
 * Bố cục dựng bằng TOẠ ĐỘ TƯỜNG MINH trong một vùng cuộn, không dùng LayoutControl: khối này gần
 * trăm ô xếp theo nhóm, để bộ xếp bố cục tự tính thì không kiểm soát được số hàng.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Hằng số bố cục =====

        private const int IHCM_LEFT = 12;
        private const int IHCM_INDENT = 16;      // thụt vào cho câu hỏi trong một mục
        private const int IHCM_ROW_H = 26;
        private const int IHCM_TITLE_H = 32;
        private const int IHCM_SUB_H = 28;
        private const int IHCM_Q_W = 720;        // ô tích câu hỏi — mỗi câu MỘT dòng như mẫu in
        private const int IHCM_DIS_W = 260;      // ô tích tên bệnh ở mục D1
        private const int IHCM_LBL_W = 600;      // nhãn câu hỏi của mục dùng ô chọn
        private const int IHCM_CBO_W = 300;
        private const int IHCM_BOTTOM_PAD = 40;
        private const int IHCM_GAP_SECTION = 16;   // khoang tho truoc moi muc
        private const int IHCM_GAP_NOTE = 6;       // khoang tho truoc dong dan

        /// <summary>Mã danh mục của cổng dùng cho các ô chọn của tab này.</summary>
        private const string IHCM_CAT__CSKCB = "CoSoKham_ChuaBenh";
        private const string IHCM_CAT__TAN_SUAT = "KSKDK_TamSoatOption";
        private const string IHCM_CAT__SUY_YEU = "KSKDK_TinhTrangSuyYeu";

        #endregion

        private DevExpress.XtraTab.XtraTabPage tabInterviewHcm;
        private XtraScrollableControl scrollInterviewHcm;

        /// <summary>Mọi ô nhập của tab, khoá = tên trường của cổng — dùng cho lưu/nạp về sau.</summary>
        private readonly Dictionary<string, Control> dicInterviewHcm =
            new Dictionary<string, Control>();

        /// <summary>Mọi ô đã dựng theo ĐÚNG THỨ TỰ — để cắt ra từng nhóm cho bảng luật.</summary>
        private readonly List<Control> lstIhcmCtrl = new List<Control>();

        /// <summary>Một dòng của bảng luật khoá/mở.</summary>
        private sealed class IhcmRule
        {
            /// <summary>Tên trường của ô điều kiện.</summary>
            public string Source;

            /// <summary>true: tích thì MỞ nhóm. false: tích thì KHOÁ nhóm (mẫu ghi "bỏ qua").</summary>
            public bool EnableWhenTicked;

            public Control[] Targets;
        }

        private readonly List<IhcmRule> lstIhcmRule = new List<IhcmRule>();

        private int ihcmY;

        /// <summary>Chặn gọi lồng: xoá trắng ô tích cũng phát sinh sự kiện đổi giá trị.</summary>
        private bool ihcmApplyingRule;

        /// <summary>
        /// Dựng tab. Gọi được nhiều lần, chỉ dựng một lượt.
        /// </summary>
        private void InitInterviewHcmTab()
        {
            try
            {
                if (tabInterviewHcm != null) return;
                if (this.xtraTabControl2 == null) return;

                // AN TOÀN ĐA VIỆN: chỉ viện đã khai báo cổng Sở Y tế TP.HCM mới có tab này.
                if (!IsSytHcmDeclared())
                {
                    LogSystem.Debug("SytHcm: chua khai bao cau hinh cong -> KHONG dung tab Hoi benh lam sang");
                    return;
                }

                tabInterviewHcm = new DevExpress.XtraTab.XtraTabPage();
                tabInterviewHcm.Name = "tabInterviewHcm";
                tabInterviewHcm.Text = "Hỏi bệnh lâm sàng HCM";

                scrollInterviewHcm = new XtraScrollableControl();
                scrollInterviewHcm.Name = "scrollInterviewHcm";
                scrollInterviewHcm.Dock = DockStyle.Fill;
                scrollInterviewHcm.AutoScroll = true;
                tabInterviewHcm.Controls.Add(scrollInterviewHcm);

                ihcmY = 8;
                BuildInterviewHcmBody();
                UpdateInterviewHcmEnabled();          // áp luật ngay lượt dựng

                ApplyPendingInterviewHcm();     // ho so nap truoc khi tab kip dung

                this.xtraTabControl2.TabPages.Add(tabInterviewHcm);
                LogSystem.Warn("SytHcm/HoiBenh: da dung tab — " + dicInterviewHcm.Count
                    + " o nhap, " + lstIhcmRule.Count + " luat khoa/mo");
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Dựng thân tab theo đúng thứ tự mục D của biểu mẫu in.
        /// </summary>
        private void BuildInterviewHcmBody()
        {
            // ================== B — Tiền sử gia đình (RIÊNG của mẫu M4) ==================
            // KHÔNG dùng lại ô tích tiền sử gia đình ở tab Khám lâm sàng: danh mục bên đó là của
            // mẫu M3 (Truyền nhiễm, Lao, Động kinh, Rối loạn tâm thần...), thiếu hẳn Tăng huyết áp,
            // Phổi tắc nghẽn mạn tính, Trầm cảm-lo âu; còn "Tim mạch" bên đó KHÔNG phải "Tim mạch
            // sớm" có kèm mốc tuổi của mẫu này. Suy từ bên kia sang là đẩy dữ liệu sai.
            IhcmTitle("B. TIỀN SỬ GIA ĐÌNH VÀ CÁC YẾU TỐ LIÊN QUAN SỨC KHOẺ");
            IhcmSubTitle("B1. Trong gia đình, ông, bà, bố, mẹ, anh, chị em ruột có mắc các bệnh dưới đây không?");
            IhcmCheck("tsb_giadinh", "Có người mắc bệnh (chọn Có thì khai tiếp danh sách bên dưới)", IHCM_Q_W);

            int gB1 = IhcmMark();
            IhcmQuestionList(new string[][]
            {
                new[] { "tsbgd_timmach",      "B1.1. Tim mạch sớm (nam < 55 tuổi, nữ < 65 tuổi)." },
                new[] { "tsbgd_tanghuyetap",  "B1.2. Tăng huyết áp." },
                new[] { "tsbgd_daithaoduong", "B1.3. Đái tháo đường." },
                new[] { "tsbgd_phoi",         "B1.4. Phổi tắc nghẽn mạn tính." },
                new[] { "tsbgd_henphequan",   "B1.5. Hen phế quản / các bệnh dị ứng." },
                new[] { "tsbgd_tramcam",      "B1.6. Trầm cảm, lo âu." },
                new[] { "tsbgd_ungthu",       "B1.7. Ung thư." }
            });
            IhcmAddRule("tsb_giadinh", true, IhcmSince(gB1));

            // ================== C — Khám thực thể (phần thiếu của mẫu M4) ==================
            // Chiều cao, cân nặng, vòng bụng, mạch, huyết áp, nhịp thở đã có ở bản ghi sinh hiệu và
            // tab Khám thể lực — KHÔNG nhập lại ở đây để khỏi hai nơi lệch nhau. Riêng cân nặng một
            // năm trước là câu hỏi bệnh nhân, HIS không có chỗ nào lưu, nên nhập ở đây.
            IhcmTitle("C. KHÁM THỰC THỂ");
            IhcmNumber("cannang_namtruoc", "C2. Cân nặng 1 năm trước", "kg");

            IhcmTitle("D. HỎI BỆNH VÀ KHÁM LÂM SÀNG");

            // ================== D1 — Bệnh đang mắc / đã từng mắc ==================
            IhcmSubTitle("D1. Ông, bà có đang hoặc đã từng mắc các bệnh dưới đây không?");
            IhcmCheck("macbenh", "Có mắc bệnh (chọn Có thì khai tiếp danh sách bên dưới)", IHCM_Q_W);

            int gD1 = IhcmMark();

            // Mẫu in: 14 bệnh; riêng Nhồi máu cơ tim và Đột quỵ KHÔNG có ô nơi điều trị.
            IhcmDisease("benh_tanghuyetap", "D1.1. Tăng huyết áp", true);
            IhcmDisease("benh_daithaoduong", "D1.2. Đái tháo đường", true);
            IhcmDisease("benh_phoi", "D1.3. Phổi tắc nghẽn mạn tính", true);
            IhcmDisease("benh_hen", "D1.4. Hen phế quản", true);
            IhcmDisease("benh_ungthu", "D1.5. Ung thư", true);
            IhcmDisease("benh_suytim", "D1.6. Suy tim", true);
            IhcmDisease("benh_khop", "D1.7. Thoái hoá khớp", true);
            IhcmDisease("benh_thanmam", "D1.8. Bệnh thận mạn", true);
            IhcmDisease("benh_timthieumau", "D1.9. Bệnh tim thiếu máu cục bộ", true);
            IhcmDisease("benh_nhoimau", "D1.10. Nhồi máu cơ tim", false);
            IhcmDisease("benh_dotqui", "D1.11. Đột quỵ", false);
            IhcmDisease("benh_roiloan_tramcam", "D1.12. Trầm cảm", true);
            IhcmDisease("benh_roiloan_loau", "D1.13. Rối loạn lo âu", true);
            IhcmDisease("benh_sasut_tritue", "D1.14. Sa sút trí tuệ", true);
            IhcmText("benh_khac_hoibenh", "Khác, ghi rõ");

            // Chưa trả lời "có mắc bệnh" thì cả danh sách bệnh đóng.
            IhcmAddRule("macbenh", true, IhcmSince(gD1));

            // ================== D2 — Đái tháo đường ==================
            int gD2 = IhcmMark();
            IhcmSubTitle("D2. Tầm soát về bệnh đái tháo đường (bỏ qua nếu D1.2 chọn Có)");
            IhcmQuestionList(new string[][]
            {
                new[] { "dtd_maudoi",      "D2.1. Gần đây có cảm thấy mau đói, ăn nhiều lần trong ngày." },
                new[] { "dtd_khatnuoc",    "D2.2. Cảm thấy khát nước, uống nhiều nước (uống trên 3 lít/24 giờ)." },
                new[] { "dtd_ditieunhieu", "D2.3. Đi tiểu nhiều lần trong ngày (bình thường 4-7 lần/24 giờ)." },
                new[] { "dtd_sutcan",      "D2.4. Gần đây có thấy bị sụt cân nhiều (mặc quần áo rộng hơn trước)." },
                new[] { "dtd_vetthuong",   "D2.5. Xuất hiện những vết thương ngoài da khó lành." }
            });
            IhcmAddRule("benh_daithaoduong", false, IhcmSince(gD2));

            // ================== D3 — Phổi tắc nghẽn mạn tính ==================
            // Mẫu in có mục này nhưng bản tin của cổng chưa có nhóm tương ứng — dựng để in, chưa đẩy.
            int gD3 = IhcmMark();
            IhcmSubTitle("D3. Tầm soát về bệnh phổi tắc nghẽn mạn tính (bỏ qua nếu D1.3 chọn Có)");
            IhcmQuestionList(new string[][]
            {
                new[] { "bpq_ho_hangngay", "D3.1. Ho vài lần trong ngày ở hầu hết các ngày." },
                new[] { "bpq_khacdam",     "D3.2. Khạc đàm ở hầu hết các ngày." },
                new[] { "bpq_khotho",      "D3.3. Dễ bị khó thở hơn những người cùng tuổi." }
            });
            IhcmAddRule("benh_phoi", false, IhcmSince(gD3));

            // ================== D4 — Hen phế quản ==================
            int gD4 = IhcmMark();
            IhcmSubTitle("D4. Tầm soát về bệnh hen phế quản (bỏ qua nếu D1.4 chọn Có)");
            IhcmQuestionList(new string[][]
            {
                new[] { "hpq_khokhe",              "D4.1. Xuất hiện những cơn khò khè/thở rít hay những đợt khò khè, thở rít tái đi tái lại." },
                new[] { "hpq_ho_demkhuya",         "D4.2. Ho gây khó chịu lúc đêm khuya." },
                new[] { "hpq_ho_thucgiac",         "D4.3. Bị thức giấc vì cơn ho hay khó thở bất cứ khi nào." },
                new[] { "hpq_ho_vandong",          "D4.4. Ho, khò khè hay thở rít sau khi vận động thể lực (chạy, tập thể dục)." },
                new[] { "hpq_hohap_theomua",       "D4.5. Có vấn đề hô hấp vào mùa nhất định nào đó trong năm." },
                new[] { "hpq_ho_chatkichthich",    "D4.6. Ho, khò khè hay nặng ngực khi hít phải chất kích thích trong không khí." },
                new[] { "hpq_dotcamlanh",          "D4.7. Bị những đợt cảm lạnh nhập vào phổi HOẶC phải điều trị hơn mười ngày mới khỏi." },
                new[] { "hpq_trieuchung_caithien", "D4.8. Khi có những triệu chứng hô hấp, có cải thiện với điều trị hen thích hợp." }
            });
            IhcmAddRule("benh_hen", false, IhcmSince(gD4));

            // ================== D5 — Ung thư (KHÔNG có điều kiện, hỏi mọi người) ==================
            IhcmSubTitle("D5. Tầm soát về bệnh ung thư — hiện tại ông/bà có những dấu hiệu nào dưới đây không?");
            IhcmQuestionList(new string[][]
            {
                new[] { "ut_vetloet",                      "D5.1. Những vết loét trên cơ thể lâu lành." },
                new[] { "ut_hodai",                        "D5.2. Ho dai dẳng hoặc khàn tiếng." },
                new[] { "ut_ankhongtieu",                  "D5.3. Ăn không tiêu hoặc nuốt khó." },
                new[] { "ut_thaydoi_thoiquen_ruotbongdai", "D5.4. Thay đổi thói quen của ruột và bàng quang (tiêu, tiểu nhiều lần hoặc tiêu chảy xen kẽ táo bón)." },
                new[] { "ut_cucu",                         "D5.5. Có một chỗ dày lên hoặc một cục u ở vú hoặc ở nơi nào đó trong cơ thể." },
                new[] { "ut_notruoi",                      "D5.6. Xuất hiện những nốt ruồi bị thay đổi về màu, hình ảnh, kích thước." },
                new[] { "ut_hachto",                       "D5.7. Sờ thấy ở cổ, nách, bẹn có những hạch to không bình thường." },
                new[] { "ut_utai_nghetmui",                "D5.8. Bị ù tai, nghẹt mũi kéo dài uống thuốc không giảm." },
                new[] { "ut_sutcan",                       "D5.9. Sụt cân, da xanh xao thiếu máu không rõ nguyên nhân." },
                new[] { "ut_chaymau_dauvu",                "D5.10. Chảy máu hoặc tiết dịch bất thường ở đầu vú." },
                new[] { "ut_chaymau_amdao",                "D5.11. Bị chảy máu, dịch ra bất thường ở âm đạo (chỉ áp dụng đối với nữ)." }
            });

            // ================== D6 — Rối loạn trầm cảm ==================
            int gD6 = IhcmMark();
            IhcmSubTitle("D6. Tầm soát rối loạn trầm cảm (bỏ qua nếu D1.12 chọn Có)");
            IhcmNote("Trong vòng 2 tuần vừa qua, có bao nhiêu lần ông, bà bị lo lắng buồn phiền vì những vấn đề dưới đây?");
            IhcmComboList(IHCM_CAT__TAN_SUAT, new string[][]
            {
                new[] { "rltc_ithungthu",      "D6.1. Ít hứng thú hoặc là không có niềm vui thích làm việc gì." },
                new[] { "rltc_channan",        "D6.2. Cảm thấy chán nản kiệt sức, hay tuyệt vọng." },
                new[] { "rltc_khongu",         "D6.3. Khó ngủ, ngủ không lâu hoặc ngủ quá nhiều." },
                new[] { "rltc_metmoi",         "D6.4. Cảm thấy mệt mỏi hoặc ít sức lực." },
                new[] { "rltc_khongngonmieng", "D6.5. Cảm thấy ăn không ngon miệng hoặc ăn quá nhiều." },
                new[] { "rltc_camthayte",      "D6.6. Cảm thấy mình tệ, thất bại hoặc làm gia đình thất vọng." },
                new[] { "rltc_khotaptrung",    "D6.7. Khó tập trung vào những việc như đọc sách, báo, hoặc xem tivi." },
                new[] { "rltc_chamchap",       "D6.8. Đi đứng, nói năng chậm chạp — hoặc ngược lại, quá bồn chồn không yên." },
                new[] { "rltc_ynghi",          "D6.9. Có ý nghĩ gây đau đớn cho bản thân hoặc nghĩ thà mình chết đi." }
            });
            IhcmAddRule("benh_roiloan_tramcam", false, IhcmSince(gD6));

            // ================== D7 — Rối loạn lo âu ==================
            int gD7 = IhcmMark();
            IhcmSubTitle("D7. Tầm soát rối loạn lo âu (bỏ qua nếu D1.13 chọn Có)");
            IhcmNote("Trong vòng 2 tuần vừa qua, ông bà có bao nhiêu lần bị lo lắng buồn phiền vì những vấn đề dưới đây?");
            IhcmComboList(IHCM_CAT__TAN_SUAT, new string[][]
            {
                new[] { "rlla_canthang",        "D7.1. Cảm thấy căng thẳng, lo lắng hoặc bất an." },
                new[] { "rlla_kiemsoat_lolang", "D7.2. Cảm thấy không thể ngưng hoặc kiểm soát lo lắng." },
                new[] { "rlla_lolang_nhieuthu", "D7.3. Lo lắng quá mức về nhiều thứ." },
                new[] { "rlla_khothugian",      "D7.4. Khó thư giãn." },
                new[] { "rlla_bucrut",          "D7.5. Bứt rứt đến mức khó ngồi yên." },
                new[] { "rlla_bucboi",          "D7.6. Trở nên dễ bực bội hoặc cáu kỉnh." },
                new[] { "rlla_lolang",          "D7.7. Cảm thấy lo lắng như thể điều gì khủng khiếp có thể xảy ra." }
            });
            IhcmAddRule("benh_roiloan_loau", false, IhcmSince(gD7));

            // ================== D8 — Thể chất, tinh thần, vận động ==================
            IhcmSubTitle("D8. Đánh giá thể chất, tinh thần, vận động");

            IhcmNote("D8.1. Các hoạt động sống cơ bản hàng ngày của ông, bà hiện nay như thế nào?");
            IhcmQuestionList(new string[][]
            {
                new[] { "hds_tutam",                 "D8.1.1. Có thể tự tắm." },
                new[] { "hds_tumacquanao",           "D8.1.2. Có thể tự mặc quần áo." },
                new[] { "hds_tudivesinh",            "D8.1.3. Có thể tự đi vệ sinh." },
                new[] { "hds_tudichuyen_khoigiuong", "D8.1.4. Có thể tự di chuyển ra khỏi giường." },
                new[] { "hds_kiemsoat_tieutieu",     "D8.1.5. Có thể kiểm soát việc tiêu tiểu của mình." },
                new[] { "hds_tuanuong",              "D8.1.6. Ông/bà có thể tự ăn uống." }
            });

            IhcmNote("D8.2. Các hoạt động sinh hoạt hàng ngày của ông, bà hiện nay như thế nào?");
            IhcmQuestionList(new string[][]
            {
                new[] { "hdhn_nghedt",           "D8.2.1. Có thể tự nghe được điện thoại." },
                new[] { "hdhn_tumua_vatdung",    "D8.2.2. Có thể tự mua được tất cả vật dụng cần thiết của mình." },
                new[] { "hdhn_tunauan",          "D8.2.3. Có thể tự nấu một bữa ăn hoàn chỉnh, từ dự tính món, sơ chế đến nấu chín." },
                new[] { "hdhn_tulamviecnha",     "D8.2.4. Có thể tự làm được tất cả các công việc nhà." },
                new[] { "hdhn_tugiatquanao",     "D8.2.5. Có thể tự giặt được quần áo cá nhân, những món đồ nhỏ như vớ, khăn nhỏ." },
                new[] { "hdhn_tulaixe_batxe",    "D8.2.6. Có thể tự lái xe hoặc tự bắt xe đi ra khỏi nhà." },
                new[] { "hdhn_tuchia_uongthuoc", "D8.2.7. Có thể tự chia và tự lấy thuốc uống đúng liều và đúng cữ." },
                new[] { "hdhn_tugiutien",        "D8.2.8. Có thể tự giữ tiền, quản lý tiền của mình." }
            });

            IhcmNote("D8.3. Đánh giá tình trạng suy yếu");
            IhcmCombo("ttsy_metmoi", "D8.3.1. Số lần cảm thấy mệt mỏi trong 4 tuần qua.", IHCM_CAT__SUY_YEU);
            IhcmQuestionList(new string[][]
            {
                new[] { "ttsy_khokhan_leothang", "D8.3.2. Có gặp khó khăn khi leo liên tiếp 10 bậc thang không nghỉ và không có sự trợ giúp." },
                new[] { "ttsy_khokhan_dibo",     "D8.3.3. Có gặp khó khăn khi tự đi bộ vài trăm mét." }
            });

            IhcmNote("D8.4. Đánh giá nguy cơ té ngã");
            IhcmQuestionList(new string[][]
            {
                new[] { "dgtn_bite",       "D8.4.1. Trong năm qua, có bị té." },
                new[] { "dgtn_loso_binga", "D8.4.2. Có lo sợ về việc bị té ngã." },
                new[] { "dgtn_didung",     "D8.4.3. Có cảm giác đi đứng không vững." }
            });

            int gD85 = IhcmMark();
            IhcmNote("D8.5. Đánh giá mức độ giảm nhận thức (bỏ qua nếu D1.14 chọn Có)");
            IhcmCheck("gnt_trinho_bigiam", "D8.5.1. Có thấy trí nhớ bị giảm.", IHCM_Q_W);

            // Luật lồng: chỉ hỏi tiếp hai câu sau khi D8.5.1 trả lời Có.
            int gD85sub = IhcmMark();
            IhcmQuestionList(new string[][]
            {
                new[] { "gnt_ghinho",     "D8.5.2. Ghi nhớ 3 từ (bông hoa, cánh cửa, cây lúa) và định hướng không gian, thời gian." },
                new[] { "gnt_nho_noilai", "D8.5.3. Nhớ và nói lại đủ ba từ đã nhớ lúc nãy." }
            });
            IhcmAddRule("benh_sasut_tritue", false, IhcmSince(gD85));      // cha trước
            IhcmAddRule("gnt_trinho_bigiam", true, IhcmSince(gD85sub));    // con sau

            // ================== Dấu hiệu khác ==================
            IhcmSubTitle("Dấu hiệu khác");
            IhcmText("dauhieu_khac", "Dấu hiệu khác, ghi rõ");

            // Đệm đáy phải là MỘT Ô THẬT: vùng cuộn lấy đáy của ô cuối cùng làm giới hạn,
            // cộng thêm vào biến đếm thì không sinh ra khoảng trống nào.
            LabelControl pad = new LabelControl();
            pad.Name = "lblIhcmBottomPad";
            pad.Text = "";
            pad.Location = new System.Drawing.Point(IHCM_LEFT, ihcmY);
            pad.Size = new System.Drawing.Size(10, IHCM_BOTTOM_PAD);
            IhcmAdd(pad);
            ihcmY += IHCM_BOTTOM_PAD;
        }

        #region ===== Bảng luật khoá/mở =====

        /// <summary>Đánh dấu vị trí hiện tại để lát nữa cắt ra một nhóm.</summary>
        private int IhcmMark()
        {
            return lstIhcmCtrl.Count;
        }

        /// <summary>Mọi ô đã dựng kể từ mốc — chính là một mục của biểu mẫu.</summary>
        private Control[] IhcmSince(int mark)
        {
            if (mark < 0 || mark > lstIhcmCtrl.Count) return new Control[0];
            return lstIhcmCtrl.GetRange(mark, lstIhcmCtrl.Count - mark).ToArray();
        }

        /// <summary>
        /// Thêm một dòng luật. Thứ tự thêm là thứ tự áp dụng, nên luật CHA phải thêm trước luật CON
        /// (D8.5 trước D8.5.2/D8.5.3), để khi mục cha bị bỏ qua thì ô điều kiện của con đã tắt sẵn.
        /// </summary>
        private void IhcmAddRule(string source, bool enableWhenTicked, Control[] targets)
        {
            if (string.IsNullOrEmpty(source) || targets == null || targets.Length == 0) return;

            IhcmRule r = new IhcmRule();
            r.Source = source;
            r.EnableWhenTicked = enableWhenTicked;
            r.Targets = targets;
            lstIhcmRule.Add(r);
        }

        /// <summary>
        /// Áp toàn bộ bảng luật. Gọi khi dựng tab, khi nạp hồ sơ và mỗi lần người dùng tích một ô.
        /// </summary>
        private void UpdateInterviewHcmEnabled()
        {
            if (ihcmApplyingRule) return;
            try
            {
                ihcmApplyingRule = true;

                for (int k = 0; k < lstIhcmRule.Count; k++)
                {
                    IhcmRule r = lstIhcmRule[k];

                    Control src;
                    if (!dicInterviewHcm.TryGetValue(r.Source, out src)) continue;
                    CheckEdit chk = src as CheckEdit;
                    if (chk == null) continue;

                    // Ô điều kiện đang bị khoá thì coi như CHƯA trả lời — nhờ vậy luật lồng nhau
                    // (D1.14 -> D8.5.1 -> D8.5.2) chỉ cần một lượt duyệt.
                    bool ticked = chk.Enabled && chk.Checked;
                    bool enable = r.EnableWhenTicked ? ticked : !ticked;

                    for (int i = 0; i < r.Targets.Length; i++)
                    {
                        Control c = r.Targets[i];
                        if (c == null) continue;
                        c.Enabled = enable;

                        // Mục bị "bỏ qua" theo mẫu thì không được giữ câu trả lời cũ.
                        if (!enable) IhcmClearValue(c);
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { ihcmApplyingRule = false; }
        }

        private static void IhcmClearValue(Control c)
        {
            try
            {
                CheckEdit chk = c as CheckEdit;
                if (chk != null) { chk.Checked = false; return; }

                GridLookUpEdit cbo = c as GridLookUpEdit;
                if (cbo != null) { cbo.EditValue = null; return; }

                MemoEdit txt = c as MemoEdit;
                if (txt != null) { txt.Text = ""; return; }

                SpinEdit spin = c as SpinEdit;
                if (spin != null) { spin.EditValue = null; return; }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void IhcmCheck_CheckedChanged(object sender, EventArgs e)
        {
            // Dang do du lieu tu ho so: luat chay giua chung se xoa trang o vua do vao.
            if (ihcmLoading) return;
            UpdateInterviewHcmEnabled();
        }

        #endregion

        #region ===== Các hàm dựng ô =====

        /// <summary>Ghi nhận ô vừa dựng: đưa lên màn hình và vào danh sách để cắt nhóm.</summary>
        private void IhcmAdd(Control c)
        {
            scrollInterviewHcm.Controls.Add(c);
            lstIhcmCtrl.Add(c);
        }

        /// <summary>Tiêu đề mục lớn (D. HỎI BỆNH VÀ KHÁM LÂM SÀNG).</summary>
        private void IhcmTitle(string text)
        {
            LabelControl lbl = new LabelControl();
            lbl.Text = text;
            lbl.Appearance.Font = new System.Drawing.Font(lbl.Appearance.Font.FontFamily, 10F,
                System.Drawing.FontStyle.Bold);
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            lbl.Location = new System.Drawing.Point(IHCM_LEFT, ihcmY + 8);
            lbl.Size = new System.Drawing.Size(IHCM_Q_W, 20);
            IhcmAdd(lbl);
            ihcmY += IHCM_TITLE_H + 6;
        }

        /// <summary>Tiêu đề một mục con (D1, D2...) — đây là phần bị khoá/mở theo bảng luật.</summary>
        private void IhcmSubTitle(string text)
        {
            // Ngat quang TRUOC tieu de: khong co no thi muc moi dinh sat dong cuoi cua muc tren.
            ihcmY += IHCM_GAP_SECTION;

            LabelControl lbl = new LabelControl();
            lbl.Text = text;
            lbl.Appearance.Font = new System.Drawing.Font(lbl.Appearance.Font, System.Drawing.FontStyle.Bold);
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            lbl.Location = new System.Drawing.Point(IHCM_LEFT, ihcmY);
            lbl.Size = new System.Drawing.Size(IHCM_Q_W + IHCM_CBO_W, 18);
            IhcmAdd(lbl);
            ihcmY += IHCM_SUB_H;
        }

        /// <summary>Dòng dẫn của mục — câu hỏi chung đứng trước danh sách câu con.</summary>
        private void IhcmNote(string text)
        {
            LabelControl lbl = new LabelControl();
            lbl.Text = text;
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            ihcmY += IHCM_GAP_NOTE;
            lbl.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT, ihcmY);
            lbl.Size = new System.Drawing.Size(IHCM_Q_W + IHCM_CBO_W, 18);
            IhcmAdd(lbl);
            ihcmY += 22;
        }

        /// <summary>Một ô tích chiếm trọn dòng — mỗi câu hỏi một dòng, đọc thẳng theo mẫu in.</summary>
        private void IhcmCheck(string field, string caption, int width)
        {
            CheckEdit chk = new CheckEdit();
            chk.Name = "chkIhcm_" + field;
            chk.Text = caption;
            chk.Tag = field;                       // tên trường của cổng — dùng cho lưu/nạp sau này
            chk.Properties.AllowGrayed = false;
            chk.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT, ihcmY);
            chk.Size = new System.Drawing.Size(width, 22);
            chk.CheckedChanged += IhcmCheck_CheckedChanged;
            IhcmAdd(chk);
            dicInterviewHcm[field] = chk;
            ihcmY += IHCM_ROW_H;
        }

        private void IhcmQuestionList(string[][] items)
        {
            foreach (string[] it in items) IhcmCheck(it[0], it[1], IHCM_Q_W);
        }

        /// <summary>
        /// Một bệnh ở mục D1. Chỉ những bệnh mà MẪU IN có ghi "CS KBCB đang điều trị" mới dựng ô
        /// chọn nơi điều trị; ô này chỉ mở khi đã tích tên bệnh.
        /// </summary>
        private void IhcmDisease(string field, string caption, bool hasCskcb)
        {
            int y = ihcmY;

            CheckEdit chk = new CheckEdit();
            chk.Name = "chkIhcm_" + field;
            chk.Text = caption;
            chk.Tag = field;
            chk.Properties.AllowGrayed = false;
            chk.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT, y);
            chk.Size = new System.Drawing.Size(IHCM_DIS_W, 22);
            chk.CheckedChanged += IhcmCheck_CheckedChanged;
            IhcmAdd(chk);
            dicInterviewHcm[field] = chk;

            if (hasCskcb)
            {
                int mark = IhcmMark();

                LabelControl lbl = new LabelControl();
                lbl.Text = "CS KBCB đang điều trị:";
                lbl.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT + IHCM_DIS_W, y + 3);
                lbl.Size = new System.Drawing.Size(130, 18);
                IhcmAdd(lbl);

                GridLookUpEdit cbo = BuildIhcmCombo(field + "_cskcb", IHCM_CAT__CSKCB);
                cbo.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT + IHCM_DIS_W + 134, y);
                cbo.Size = new System.Drawing.Size(IHCM_CBO_W, 22);
                IhcmAdd(cbo);

                IhcmAddRule(field, true, IhcmSince(mark));   // tích bệnh mới chọn được nơi điều trị
            }

            ihcmY += IHCM_ROW_H;
        }


        /// <summary>Ô nhập số có đơn vị — dùng cho các trị số của mục C.</summary>
        private void IhcmNumber(string field, string caption, string unit)
        {
            LabelControl lbl = new LabelControl();
            lbl.Text = caption;
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            lbl.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT, ihcmY + 3);
            lbl.Size = new System.Drawing.Size(260, 18);
            IhcmAdd(lbl);

            SpinEdit spin = new SpinEdit();
            spin.Name = "spinIhcm_" + field;
            spin.Tag = field;
            spin.Properties.MinValue = 0;
            spin.Properties.MaxValue = 500;
            spin.Properties.Mask.UseMaskAsDisplayFormat = true;
            spin.Properties.Mask.EditMask = "n1";
            spin.EditValue = null;
            spin.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT + 264, ihcmY);
            spin.Size = new System.Drawing.Size(110, 22);
            IhcmAdd(spin);
            dicInterviewHcm[field] = spin;

            LabelControl lblUnit = new LabelControl();
            lblUnit.Text = unit;
            lblUnit.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT + 380, ihcmY + 3);
            lblUnit.Size = new System.Drawing.Size(40, 18);
            IhcmAdd(lblUnit);

            ihcmY += IHCM_ROW_H;
        }

        /// <summary>Câu hỏi trả lời bằng ô chọn (mức tần suất, mức suy yếu).</summary>
        private void IhcmCombo(string field, string caption, string catalogCode)
        {
            LabelControl lbl = new LabelControl();
            lbl.Text = caption;
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            lbl.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT, ihcmY + 3);
            lbl.Size = new System.Drawing.Size(IHCM_LBL_W - 8, 18);
            IhcmAdd(lbl);

            GridLookUpEdit cbo = BuildIhcmCombo(field, catalogCode);
            cbo.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT + IHCM_LBL_W, ihcmY);
            cbo.Size = new System.Drawing.Size(IHCM_CBO_W, 22);
            IhcmAdd(cbo);

            ihcmY += IHCM_ROW_H;
        }

        private void IhcmComboList(string catalogCode, string[][] items)
        {
            foreach (string[] it in items) IhcmCombo(it[0], it[1], catalogCode);
        }

        private void IhcmText(string field, string caption)
        {
            LabelControl lbl = new LabelControl();
            lbl.Text = caption;
            lbl.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT, ihcmY + 3);
            lbl.Size = new System.Drawing.Size(140, 18);
            IhcmAdd(lbl);

            MemoEdit txt = new MemoEdit();
            txt.Name = "txtIhcm_" + field;
            txt.Tag = field;
            txt.Location = new System.Drawing.Point(IHCM_LEFT + IHCM_INDENT + 144, ihcmY);
            // Thẳng lề phải với cột ô chọn, nếu rộng hơn thì cả tab sinh thanh cuộn ngang.
            txt.Size = new System.Drawing.Size(IHCM_LBL_W + IHCM_CBO_W - 144, 44);
            IhcmAdd(txt);
            dicInterviewHcm[field] = txt;

            ihcmY += 52;
        }

        /// <summary>
        /// Ô chọn đổ từ danh mục của cổng. Danh mục chưa tải về thì ô vẫn dựng nhưng rỗng —
        /// tab hiện đủ hình hài, đổ danh mục là việc của lượt sau.
        /// </summary>
        private GridLookUpEdit BuildIhcmCombo(string field, string catalogCode)
        {
            GridLookUpEdit cbo = new GridLookUpEdit();
            cbo.Name = "cboIhcm_" + field;
            cbo.Tag = field;
            cbo.MenuManager = this.barManager1;
            cbo.Properties.NullText = "";
            try
            {
                List<KskCodeNameADO> data = ToCodeNameList(catalogCode);
                if (data.Count > 0)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = cbo.Properties.View;

                    // Tat tu sinh cot TRUOC khi gan nguon, neu khong DevExpress dung luon ten
                    // thuoc tinh (ID, NAME) lam tieu de cot.
                    view.OptionsBehavior.AutoPopulateColumns = false;
                    view.OptionsView.ShowGroupPanel = false;
                    view.OptionsView.ShowAutoFilterRow = true;

                    cbo.Properties.DataSource = data;
                    cbo.Properties.DisplayMember = "NAME";
                    cbo.Properties.ValueMember = "ID";

                    view.Columns.Clear();
                    DevExpress.XtraGrid.Columns.GridColumn colId = view.Columns.AddVisible("ID", "Mã");
                    DevExpress.XtraGrid.Columns.GridColumn colName = view.Columns.AddVisible("NAME", "Tên");
                    colId.Width = 70;
                    colName.Width = 400;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            dicInterviewHcm[field] = cbo;
            return cbo;
        }

        #endregion
    }
}
