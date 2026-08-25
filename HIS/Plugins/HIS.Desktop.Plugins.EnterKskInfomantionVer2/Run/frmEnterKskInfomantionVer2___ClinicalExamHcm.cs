/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tab "Khám lâm sàng HCM" (tab con của "Ksk trên 18 tuổi") — đánh số theo Mẫu 03 mục II
 * (Thông tư 25/2026/TT-BYT), phục vụ mẫu phiếu M4 của Nền tảng KSK Sở Y tế TP.HCM.
 *
 * Khác tab "Khám lâm sàng" hiện có: Ô NHẬP KẾT QUẢ (văn bản tự do) được THAY bằng cụm chọn ICD-10:
 *   [ ] Chưa phát hiện bất thường                     <- ô tích
 *   Chẩn đoán sơ bộ: [ô chọn ICD] ...                 <- ô chọn riêng
 *   Chẩn đoán xác định: [ô chọn ICD] ...              <- ô chọn riêng
 *
 * KHÁC với cụm ICD của tab Kết luận (UcKskConclusionIcd): ở đó 3 mục là LỰA CHỌN LOẠI TRỪ
 * NHAU và dùng CHUNG một ô chọn bệnh. Ở đây một lượt khám có thể có ĐỒNG THỜI chẩn đoán sơ bộ
 * và chẩn đoán xác định, nên mỗi loại có ô chọn riêng, còn "chưa phát hiện bất thường" tách
 * thành ô tích. Tích ô này thì hai ô chọn bệnh bị khóa và xóa trống.
 *
 * Các TRỊ SỐ ĐO và PHÂN LOẠI / NGƯỜI KHÁM giữ như tab "Khám lâm sàng".
 *
 * BỐ CỤC: không dùng khung. Mọi dòng của một mục có CÙNG BỀ RỘNG TỔNG với cụm ICD; các ô trong
 * một dòng chia đều phần bề rộng còn lại sau khi trừ nhãn. Toàn bộ được tính lại khi đổi kích
 * thước cửa sổ (LayoutClinicalExamHcm) nên không có dòng ngắn dòng dài và không tràn ngang.
 *
 * ĐỐI CHIẾU SỐ MỤC (Mẫu 03 vs tab "Khám lâm sàng" hiện có — hai bên đánh số khác nhau):
 *   Mẫu 03: 1 Nội khoa · 2 Ngoại khoa · 3 Da liễu · 4 Sản phụ khoa · 5 Mắt · 6 TMH · 7 RHM
 *   Tab cũ: 1 Nội khoa · 2 Ngoại khoa · 3 Sản phụ khoa · 4 Mắt · 5 TMH · 6 RHM · 7 Da liễu
 *   -> Mục 4 (Sản phụ khoa) lấy từ mục 3 tab cũ; mục 5 (Mắt) từ mục 4; mục 6 (TMH) từ mục 5;
 *      mục 7 (RHM) từ mục 6; mục 3 (Da liễu) từ mục 7.
 *
 * TÁCH SẢN KHOA / PHỤ KHOA: mẫu M4 có HAI chỉ tiêu riêng (mỗi bên có chẩn đoán, phân loại và
 * ô "từ chối khám" riêng) trong khi Mẫu 03 và tab cũ chỉ có MỘT mục gộp "Sản phụ khoa".
 * Ở đây giữ mục 4 làm TIÊU ĐỀ NHÓM rồi tách thành 2 mục con a) và b) — cùng cách trình bày
 * với mục 1 Nội khoa (a) Tuần hoàn, b) Hô hấp...). Nhờ vậy số mục 5/6/7 của Mẫu 03 giữ nguyên,
 * không lệch với giấy khám in ra.
 *
 * GIAI ĐOẠN NÀY: CHỈ DỰNG GIAO DIỆN, chưa nạp/lưu dữ liệu (xem TODO(BE) ở cuối file).
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Khám lâm sàng HCM — Mô hình bố cục =====

        /// <summary>Một ô trong dòng: nhãn (bề rộng cố định) + control nhập (chia đều bề rộng còn lại).</summary>
        private class HcmCell
        {
            public LabelControl Lbl { get; set; }
            public int LblWidth { get; set; }
            public Control Edit { get; set; }
        }

        /// <summary>
        /// Một dòng hiển thị. Ba loại: dòng tiêu đề (Title), dòng 1 control chiếm hết bề rộng (Full),
        /// dòng nhiều ô chia đều (Cells).
        /// </summary>
        private class HcmVisualRow
        {
            public int Indent { get; set; }
            public int Height { get; set; }
            public LabelControl Title { get; set; }
            public Control Full { get; set; }
            public List<HcmCell> Cells { get; set; }
            /// <summary>false = ô giữ bề rộng tự nhiên (VD ô tích), không kéo giãn.</summary>
            public bool StretchCells { get; set; }

            public HcmVisualRow()
            {
                Cells = new List<HcmCell>();
                StretchCells = true;
                Height = 22;
            }
        }

        /// <summary>Một mục khám lâm sàng trên tab Khám lâm sàng HCM.</summary>
        private class ClinicalExamHcmRow
        {
            /// <summary>Khóa nội bộ theo tên chỉ tiêu mẫu M4 (noikhoa, hohap, mat, tmh, rhm...).</summary>
            public string Key { get; set; }
            /// <summary>Ô tích "Chưa phát hiện bất thường".</summary>
            public CheckEdit ChkNormal { get; set; }
            /// <summary>Ô chọn ICD-10 cho chẩn đoán SƠ BỘ.</summary>
            public UserControl UcPreIcd { get; set; }
            /// <summary>Ô chọn ICD-10 cho chẩn đoán XÁC ĐỊNH.</summary>
            public UserControl UcFinalIcd { get; set; }
            /// <summary>Khung chứa ô chọn sơ bộ (dựng trước, nhúng ô chọn sau khi mở tab).</summary>
            public PanelControl PnlPreIcd { get; set; }
            /// <summary>Khung chứa ô chọn xác định.</summary>
            public PanelControl PnlFinalIcd { get; set; }
            /// <summary>Phân loại sức khỏe của mục (giữ như tab Khám lâm sàng).</summary>
            public GridLookUpEdit CboRank { get; set; }
            /// <summary>Người khám của mục (giữ như tab Khám lâm sàng).</summary>
            public GridLookUpEdit CboLoginName { get; set; }
            /// <summary>Trị số đo riêng của mục (thị lực / thính lực / hàm răng), khóa = tên chỉ tiêu M4.</summary>
            public Dictionary<string, TextEdit> Fields { get; set; }
            /// <summary>Chỉ có ở mục Sản phụ khoa — Từ chối khám sản khoa.</summary>
            public CheckEdit ChkRefuse { get; set; }

            public ClinicalExamHcmRow()
            {
                Fields = new Dictionary<string, TextEdit>();
            }
        }

        private readonly List<ClinicalExamHcmRow> clinicalExamHcmRows = new List<ClinicalExamHcmRow>();
        private readonly List<HcmVisualRow> clinicalExamHcmVisualRows = new List<HcmVisualRow>();
        private DevExpress.XtraTab.XtraTabPage tabClinicalExamHcm;
        private XtraScrollableControl scrollClinicalExamHcm;
        private bool isClinicalExamHcmInited = false;

        /// <summary>
        /// Các mục khám lâm sàng theo Mẫu 03 (mục Sản phụ khoa tách đôi -> 15 mục). Mỗi dòng: khóa nội bộ · nhãn mục · tiêu đề nhóm đặt
        /// TRƯỚC mục (rỗng nếu không mở nhóm mới) · cờ thụt lề (mục con của nhóm).
        /// </summary>
        private static readonly string[][] CLINICAL_HCM_SECTIONS = new string[][]
        {
            new[] { "noikhoa",      "a) Tuần hoàn",          "1. Nội khoa", "1" },
            new[] { "hohap",        "b) Hô hấp",             "",            "1" },
            new[] { "tieuhoa",      "c) Tiêu hóa",           "",            "1" },
            new[] { "thantietnieu", "d) Thận - Tiết niệu",   "",            "1" },
            new[] { "noitiet",      "đ) Nội tiết",           "",            "1" },
            new[] { "coxuongkhop",  "e) Cơ - xương - khớp",  "",            "1" },
            new[] { "thankinh",     "g) Thần kinh",          "",            "1" },
            new[] { "tamthan",      "h) Tâm thần",           "",            "1" },
            new[] { "ngoaikhoa",    "2. Ngoại khoa",         "",            "" },
            new[] { "dalieu",       "3. Da liễu",            "",            "" },
            new[] { "sankhoa",      "a) Sản khoa",           "4. Sản phụ khoa", "1" },
            new[] { "phukhoa",      "b) Phụ khoa",           "",                "1" },
            new[] { "mat",          "5. Mắt",                "",            "" },
            new[] { "tmh",          "6. Tai - Mũi - Họng",   "",            "" },
            new[] { "rhm",          "7. Răng - Hàm - Mặt",   "",            "" }
        };

        private const int HCM_MARGIN = 8;
        private const int HCM_INDENT = 14;
        private const int HCM_TITLE_H = 20;
        private const int HCM_ROW_H = 22;
        private const int HCM_ROW_GAP = 4;
        private const int HCM_SECTION_GAP = 10;
        private const int HCM_BOTTOM_PAD = 24;
        private const int HCM_CELL_GAP = 10;         // khoảng cách giữa 2 ô trong cùng dòng
        private const int HCM_UC_MIN_WIDTH = 780;    // bề rộng tối thiểu của một dòng
        private const int HCM_EDIT_MIN_W = 70;       // bề rộng tối thiểu 1 ô nhập
        // Bề rộng nhãn — cố định để các dòng canh thẳng cột.
        private const int HCM_LBL_ROW_W = 82;        // nhãn đầu dòng (Không kính / Kính lỗ / Khúc xạ MP...)
        private const int HCM_LBL_SIDE_W = 28;       // MP / MT
        private const int HCM_LBL_REFR_W = 54;       // Độ cầu / Độ trụ / Trục
        private const int HCM_LBL_EAR_W = 84;        // Nói thường / Nói thầm
        private const int HCM_LBL_JAW_W = 66;        // Hàm trên / Hàm dưới
        private const int HCM_LBL_ICD_W = 118;       // nhãn "Chẩn đoán sơ bộ" / "Chẩn đoán xác định"
        private const int HCM_ICD_ROW_H = 24;        // dòng chứa 2 ô chọn ICD
        private const int HCM_ICD_BTN_W = 24;        // nút "..." mở bảng tìm bệnh
        private const int HCM_LBL_RANK_W = 62;
        private const int HCM_LBL_LOGIN_W = 78;

        #endregion

        #region ===== Dựng giao diện =====

        /// <summary>
        /// Dựng tab "Khám lâm sàng HCM" và THÊM VÀO CUỐI tab control của "Ksk trên 18 tuổi".
        /// Thêm vào cuối (không chèn giữa) để KHÔNG đổi chỉ số các tab hiện có — có logic đang so
        /// chỉ số tab "Cận lâm sàng" để bật/tắt vùng lấy chỉ số xét nghiệm tự động.
        /// Cụm ICD-10 và dữ liệu 2 combo khởi tạo LAZY khi mở tab lần đầu.
        /// </summary>
        private void InitClinicalExamHcmTab()
        {
            try
            {
                if (tabClinicalExamHcm != null) return;
                if (this.xtraTabControl2 == null) return;

                // AN TOÀN ĐA VIỆN: tab này chỉ dành cho viện đã khai báo cổng Sở Y tế TP.HCM.
                if (!IsSytHcmDeclared())
                {
                    Inventec.Common.Logging.LogSystem.Debug(
                        "SytHcm: chua khai bao cau hinh cong -> KHONG dung tab Kham lam sang HCM");
                    return;
                }

                tabClinicalExamHcm = new DevExpress.XtraTab.XtraTabPage();
                tabClinicalExamHcm.Name = "tabClinicalExamHcm";
                tabClinicalExamHcm.Text = "Khám lâm sàng HCM";

                scrollClinicalExamHcm = new XtraScrollableControl();
                scrollClinicalExamHcm.Name = "scrollClinicalExamHcm";
                scrollClinicalExamHcm.Dock = DockStyle.Fill;
                scrollClinicalExamHcm.AutoScroll = true;
                tabClinicalExamHcm.Controls.Add(scrollClinicalExamHcm);

                foreach (string[] section in CLINICAL_HCM_SECTIONS)
                {
                    // Tiêu đề nhóm (VD "1. Nội khoa") — ghi 1 lần, trước mục đầu tiên của nhóm.
                    if (!string.IsNullOrEmpty(section[2]))
                        AddHcmTitleRow("lblClinicalHcmGroup_" + section[0], section[2], 0);

                    int indent = (section[3] == "1") ? HCM_INDENT : 0;
                    BuildClinicalExamHcmSection(section[0], section[1], indent);
                }

                // Đệm đáy: vùng cuộn lấy đáy control cuối làm giới hạn, không có dòng này thì
                // "Phân loại / Người khám" của mục cuối sát mép dưới tab.
                HcmVisualRow pad = AddHcmTitleRow("lblClinicalHcmBottomPad", "", 0);
                pad.Height = HCM_BOTTOM_PAD;

                LayoutClinicalExamHcm();
                scrollClinicalExamHcm.SizeChanged += (s, e) => LayoutClinicalExamHcm();

                this.xtraTabControl2.TabPages.Add(tabClinicalExamHcm);
                HideStandardClinicalTab();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Ẩn tab "Khám lâm sàng" thường khi viện đã dùng tab "Khám lâm sàng HCM".
        ///
        /// Hai tab hỏi cùng một nội dung khám, để cả hai thì người nhập không biết nhập ở đâu và
        /// dễ nhập một nửa mỗi bên. CHỈ ẨN, không xoá: các ô của tab cũ vẫn giữ nguyên giá trị đã
        /// lưu và vẫn được ghi xuống cơ sở dữ liệu như trước, nên hồ sơ cũ không mất dữ liệu.
        /// </summary>
        private void HideStandardClinicalTab()
        {
            try
            {
                if (this.xtraTabPage10 == null) return;
                if (!this.xtraTabPage10.PageVisible) return;

                this.xtraTabPage10.PageVisible = false;
                LogSystem.Warn("SytHcm: da an tab Kham lam sang thuong vi dang dung tab Kham lam sang HCM");
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Dựng 1 mục theo thứ tự: tiêu đề mục -> [trị số đo / thông tin riêng] -> cụm ICD-10
        /// (thay ô nhập kết quả) -> Phân loại + Người khám.
        /// </summary>
        private void BuildClinicalExamHcmSection(string key, string caption, int indent)
        {
            ClinicalExamHcmRow row = new ClinicalExamHcmRow { Key = key };
            int inner = indent + HCM_INDENT;   // nội dung thụt vào so với tiêu đề mục

            AddHcmTitleRow("lblClinicalHcmSection_" + key, caption, indent);

            // ===== Trị số đo / thông tin riêng của mục =====
            if (key == "sankhoa" || key == "phukhoa")
            {
                // Mẫu 03 mục II.4 gộp "Sản phụ khoa", nhưng mẫu M4 tách thành HAI chỉ tiêu riêng,
                // mỗi bên có ô "từ chối khám" riêng -> mỗi mục dựng một ô tích riêng.
                row.ChkRefuse = new CheckEdit();
                row.ChkRefuse.Name = "chkClinicalHcm_" + key + "_tuchoikham";
                row.ChkRefuse.Properties.Caption = (key == "sankhoa")
                    ? "Từ chối khám sản khoa"
                    : "Từ chối khám phụ khoa";
                row.ChkRefuse.Width = 330;
                HcmVisualRow vr = NewHcmRow(inner);
                vr.StretchCells = false;                       // ô tích giữ bề rộng tự nhiên
                vr.Cells.Add(NewHcmCell(null, 0, row.ChkRefuse));
            }
            else if (key == "mat")
            {
                // Mẫu 03 mục II.5: Khám thị lực (không kính / kính lỗ / có kính, mỗi dòng MP + MT)
                // và Khám khúc xạ nếu có (mỗi mắt: độ cầu – độ trụ – trục).
                AddHcmEyeSightRow(row, inner, "Không kính", "mat_khongkinh_mp", "mat_khongkinh_mt");
                AddHcmEyeSightRow(row, inner, "Kính lỗ", "mat_kinhlo_mp", "mat_kinhlo_mt");
                AddHcmEyeSightRow(row, inner, "Có kính", "mat_cokinh_mp", "mat_cokinh_mt");
                AddHcmRefractionRow(row, inner, "Khúc xạ MP", "mat_docau_mp", "mat_dotru_mp", "mat_truc_mp");
                AddHcmRefractionRow(row, inner, "Khúc xạ MT", "mat_docau_mt", "mat_dotru_mt", "mat_truc_mt");
            }
            else if (key == "tmh")
            {
                // Tab cũ mục "5. Tai - Mũi - Họng": nói thường / nói thầm cho tai trái và tai phải.
                AddHcmEarRow(row, inner, "Tai trái", "tmh_taitrai_noithuong", "tmh_taitrai_noitham");
                AddHcmEarRow(row, inner, "Tai phải", "tmh_taiphai_noithuong", "tmh_taiphai_noitham");
            }
            else if (key == "rhm")
            {
                // Mẫu 03 mục II.7: tình trạng TỪNG CHIẾC RĂNG, không phải mô tả chữ cho cả hàm.
                // Sơ đồ 32 răng: tích chọn nhiều răng -> chọn trạng thái -> bấm Áp dụng.
                HcmVisualRow vrTeeth = NewHcmRow(inner);
                vrTeeth.Height = HCM_TEETH_PANEL_H;
                vrTeeth.Full = BuildHcmToothChart();
                scrollClinicalExamHcm.Controls.Add(vrTeeth.Full);
            }

            // ===== Ô tích "Chưa phát hiện bất thường" + 2 ô chọn ICD-10 =====
            row.ChkNormal = new CheckEdit();
            row.ChkNormal.Name = "chkClinicalHcmNormal_" + key;
            row.ChkNormal.Properties.Caption = "Chưa phát hiện bất thường";
            row.ChkNormal.Width = 240;
            row.ChkNormal.Tag = row;                       // để handler biết mục nào
            row.ChkNormal.CheckedChanged += ChkClinicalHcmNormal_CheckedChanged;
            HcmVisualRow vrChk = NewHcmRow(inner);
            vrChk.StretchCells = false;
            vrChk.Cells.Add(NewHcmCell(null, 0, row.ChkNormal));

            row.PnlPreIcd = NewHcmIcdPanel("pnlClinicalHcmPreIcd_" + key);
            row.PnlFinalIcd = NewHcmIcdPanel("pnlClinicalHcmFinalIcd_" + key);
            HcmVisualRow vrIcd = NewHcmRow(inner);
            vrIcd.Height = HCM_ICD_ROW_H;
            vrIcd.Cells.Add(NewHcmCell("Chẩn đoán sơ bộ", HCM_LBL_ICD_W, row.PnlPreIcd));
            vrIcd.Cells.Add(NewHcmCell("Chẩn đoán xác định", HCM_LBL_ICD_W, row.PnlFinalIcd));

            // ===== Phân loại + Người khám (giữ như tab Khám lâm sàng) =====
            row.CboRank = BuildHcmCombo("cboClinicalHcmRank_" + key);
            row.CboLoginName = BuildHcmCombo("cboClinicalHcmLogin_" + key);
            HcmVisualRow vrCbo = NewHcmRow(inner);
            vrCbo.Cells.Add(NewHcmCell("Phân loại", HCM_LBL_RANK_W, row.CboRank));
            vrCbo.Cells.Add(NewHcmCell("Người khám", HCM_LBL_LOGIN_W, row.CboLoginName));

            clinicalExamHcmRows.Add(row);
        }

        /// <summary>1 dòng thị lực: [nhãn dòng] MP: [ô] MT: [ô].</summary>
        private void AddHcmEyeSightRow(ClinicalExamHcmRow row, int indent, string rowLabel,
            string keyMp, string keyMt)
        {
            HcmVisualRow vr = NewHcmRow(indent);
            vr.Cells.Add(NewHcmCell(rowLabel, HCM_LBL_ROW_W, null));   // nhãn đầu dòng
            vr.Cells.Add(NewHcmCell("MP", HCM_LBL_SIDE_W, NewHcmEdit(row, keyMp)));
            vr.Cells.Add(NewHcmCell("MT", HCM_LBL_SIDE_W, NewHcmEdit(row, keyMt)));
        }

        /// <summary>1 dòng khúc xạ 1 mắt: [nhãn dòng] Độ cầu: [ô] Độ trụ: [ô] Trục: [ô].</summary>
        private void AddHcmRefractionRow(ClinicalExamHcmRow row, int indent, string rowLabel,
            string keyDoCau, string keyDoTru, string keyTruc)
        {
            HcmVisualRow vr = NewHcmRow(indent);
            vr.Cells.Add(NewHcmCell(rowLabel, HCM_LBL_ROW_W, null));
            vr.Cells.Add(NewHcmCell("Độ cầu", HCM_LBL_REFR_W, NewHcmEdit(row, keyDoCau)));
            vr.Cells.Add(NewHcmCell("Độ trụ", HCM_LBL_REFR_W, NewHcmEdit(row, keyDoTru)));
            vr.Cells.Add(NewHcmCell("Trục", HCM_LBL_REFR_W, NewHcmEdit(row, keyTruc)));
        }

        /// <summary>1 dòng thính lực 1 tai: [nhãn dòng] Nói thường: [ô] Nói thầm: [ô].</summary>
        private void AddHcmEarRow(ClinicalExamHcmRow row, int indent, string rowLabel,
            string keyNormal, string keyWhisper)
        {
            HcmVisualRow vr = NewHcmRow(indent);
            vr.Cells.Add(NewHcmCell(rowLabel, HCM_LBL_ROW_W, null));
            vr.Cells.Add(NewHcmCell("Nói thường", HCM_LBL_EAR_W, NewHcmEdit(row, keyNormal)));
            vr.Cells.Add(NewHcmCell("Nói thầm", HCM_LBL_EAR_W, NewHcmEdit(row, keyWhisper)));
        }

        /// <summary>Khung rỗng để lát nữa nhúng ô chọn ICD vào (nhúng trễ khi mở tab lần đầu).</summary>
        private static PanelControl NewHcmIcdPanel(string name)
        {
            PanelControl pnl = new PanelControl();
            pnl.Name = name;
            pnl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            return pnl;
        }

        /// <summary>
        /// Tích "Chưa phát hiện bất thường" -> KHÓA và XÓA TRỐNG hai ô chọn bệnh của mục đó.
        /// Bỏ tích -> mở lại. Tránh trường hợp vừa ghi chưa phát hiện bất thường vừa có mã bệnh.
        /// </summary>
        private void ChkClinicalHcmNormal_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit chk = sender as CheckEdit;
                if (chk == null) return;
                ClinicalExamHcmRow row = chk.Tag as ClinicalExamHcmRow;
                if (row == null) return;

                bool normal = chk.Checked;
                SetHcmIcdEnabled(row.UcPreIcd, !normal);
                SetHcmIcdEnabled(row.UcFinalIcd, !normal);
                if (normal)
                {
                    SetHcmIcdValue(row.UcPreIcd, null, null);
                    SetHcmIcdValue(row.UcFinalIcd, null, null);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private HcmVisualRow NewHcmRow(int indent)
        {
            HcmVisualRow vr = new HcmVisualRow();
            vr.Indent = indent;
            vr.Height = HCM_ROW_H;
            clinicalExamHcmVisualRows.Add(vr);
            return vr;
        }

        private HcmVisualRow AddHcmTitleRow(string name, string text, int indent)
        {
            LabelControl lbl = new LabelControl();
            lbl.Name = name;
            lbl.Text = text;
            if (!string.IsNullOrEmpty(text))
                lbl.Appearance.Font = new System.Drawing.Font(lbl.Font, System.Drawing.FontStyle.Bold);
            scrollClinicalExamHcm.Controls.Add(lbl);

            HcmVisualRow vr = NewHcmRow(indent);
            vr.Height = HCM_TITLE_H;
            vr.Title = lbl;
            return vr;
        }

        private HcmCell NewHcmCell(string label, int labelWidth, Control edit)
        {
            HcmCell cell = new HcmCell();
            cell.LblWidth = labelWidth;
            if (!string.IsNullOrEmpty(label))
            {
                LabelControl lbl = new LabelControl();
                lbl.Name = "lblClinicalHcmCell_" + label.GetHashCode().ToString("X") + "_" + clinicalExamHcmVisualRows.Count;
                lbl.Text = label + ":";
                lbl.AutoSizeMode = LabelAutoSizeMode.None;
                scrollClinicalExamHcm.Controls.Add(lbl);
                cell.Lbl = lbl;
            }
            cell.Edit = edit;
            if (edit != null) scrollClinicalExamHcm.Controls.Add(edit);
            return cell;
        }

        private TextEdit NewHcmEdit(ClinicalExamHcmRow row, string fieldKey)
        {
            TextEdit txt = new TextEdit();
            txt.Name = "txtClinicalHcm_" + fieldKey;
            row.Fields[fieldKey] = txt;
            return txt;
        }

        private static GridLookUpEdit BuildHcmCombo(string name)
        {
            GridLookUpEdit cbo = new GridLookUpEdit();
            cbo.Name = name;
            // Không đặt thì DevExpress hiện chuỗi mặc định "[EditValue is null]" khi chưa chọn.
            cbo.Properties.NullText = "";
            return cbo;
        }

        #endregion


        #region ===== Ô chọn ICD-10 của tab Khám lâm sàng HCM =====

        /// <summary>
        /// Bộ xử lý của từng ô chọn ICD. Mỗi ô cần giữ bộ xử lý riêng để đọc/ghi/khóa được;
        /// tra theo chính ô chọn đó nên không phải thêm thuộc tính vào lớp dòng.
        /// </summary>
        private readonly Dictionary<UserControl, SecondaryIcdProcessor> hcmIcdProcessors
            = new Dictionary<UserControl, SecondaryIcdProcessor>();

        /// <summary>
        /// Nhúng một ô chọn ICD-10 vào khung đã dựng sẵn, kèm nút "..." mở bảng tìm bệnh.
        /// Danh mục ICD truyền từ ngoài vào để 30 ô dùng chung một danh sách đã sắp xếp.
        /// </summary>

        /// <summary>
        /// Danh mục ICD đang dùng cho các ô chọn bệnh của tab này LÀ CỦA CỔNG hay của HIS.
        /// Cần biết để còn dựng lại khi danh mục của cổng về muộn.
        /// </summary>
        private bool hcmIcdFromSyt;

        /// <summary>
        /// Dựng lại các ô chọn bệnh bằng danh mục ICD CỦA CỔNG, khi danh mục về SAU lúc mở tab.
        ///
        /// Danh mục tải ở luồng nền nên rất thường xuyên về sau khi người dùng đã mở tab. Không dựng
        /// lại thì các ô vẫn giữ danh mục ICD của HIS (khoảng 16 nghìn mục) thay vì của cổng (hơn 11
        /// nghìn), và mã bệnh chọn ra có thể không tồn tại ở cổng.
        ///
        /// Dựng lại kèm đổ lại giá trị đã lưu nên không mất dữ liệu đang có trong hồ sơ.
        /// </summary>
        private void RefreshHcmIcdFromSytCatalog()
        {
            try
            {
                if (!isClinicalExamHcmInited) return;   // chưa dựng -> lần dựng tới đã dùng danh mục mới
                if (hcmIcdFromSyt) return;              // đang dùng danh mục của cổng rồi
                if (sytIcdItems == null || sytIcdItems.Count == 0) return;
                if (clinicalExamHcmRows == null || clinicalExamHcmRows.Count == 0) return;

                LogSystem.Warn("SytCatalog: danh muc ICD cua cong ve MUON -> doi nguon cho o chon"
                    + " benh cua tab Kham lam sang HCM (" + sytIcdItems.Count + " muc)");

                // Ô chọn bệnh là ô của ta nên CHỈ cần đổi nguồn danh mục, không phải dựng lại control.
                foreach (ClinicalExamHcmRow row in clinicalExamHcmRows)
                {
                    RebindSytIcdSource(row.UcPreIcd);
                    RebindSytIcdSource(row.UcFinalIcd);
                }
                hcmIcdFromSyt = true;

                // Ô vừa dựng lại là ô TRỐNG -> phải đổ lại mã bệnh đã lưu của hồ sơ.
                FillKskSytHcmClinicalControls();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Bỏ hết ô cũ trong một khung chứa và giải phóng, tránh chồng hai ô lên nhau.</summary>
        private static void ClearHcmIcdPanel(PanelControl pnl)
        {
            try
            {
                if (pnl == null) return;
                var old = new List<Control>();
                foreach (Control c in pnl.Controls) old.Add(c);
                pnl.Controls.Clear();
                foreach (Control c in old)
                {
                    try { c.Dispose(); } catch (Exception exOne) { LogSystem.Warn(exOne); }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private UserControl BuildHcmIcdEditor(PanelControl pnl, List<HIS_ICD> icdSource)
        {
            // ĐÃ CHUYỂN sang ô chọn bệnh riêng, đọc danh mục ICP của cổng và lưu Id của cổng —
            // xem frmEnterKskInfomantionVer2___SytIcd.cs. Ô có sẵn của phần mềm đọc danh mục của HIS
            // nên chọn ra bệnh cổng không có, đẩy lên bị từ chối.
            return BuildSytIcdEditor(pnl);
#pragma warning disable 0162
            try
            {
                if (pnl == null) return null;

                SecondaryIcdProcessor processor = new SecondaryIcdProcessor(new CommonParam(), icdSource);
                SecondaryIcdInitADO ado = new SecondaryIcdInitADO();
                ado.Width = (pnl.Width > HCM_ICD_BTN_W) ? (pnl.Width - HCM_ICD_BTN_W) : 300;
                ado.Height = HCM_ICD_ROW_H;
                ado.TextLblIcd = "";              // nhãn đã có ở đầu ô, không lặp lại
                ado.TextSize = 0;
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;

                UserControl uc = (UserControl)processor.Run(ado);
                if (uc == null) return null;

                SimpleButton btn = new SimpleButton();
                btn.Name = pnl.Name + "_btn";
                btn.Text = "...";
                btn.Width = HCM_ICD_BTN_W;
                btn.Dock = DockStyle.Right;
                btn.Tag = uc;
                btn.Click += BtnHcmChooseIcd_Click;

                pnl.Controls.Add(uc);
                uc.Dock = DockStyle.Fill;
                pnl.Controls.Add(btn);
                btn.BringToFront();

                hcmIcdProcessors[uc] = processor;
                return uc;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>Nút "..." — mở bảng tìm chọn bệnh, dùng lại đúng bảng của cụm ICD tab Kết luận.</summary>
        private void BtnHcmChooseIcd_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton btn = sender as SimpleButton;
                if (btn == null) return;
                UserControl uc = btn.Tag as UserControl;
                if (uc == null || !uc.Enabled) return;

                string code, name;
                GetHcmIcdValue(uc, out code, out name);
                int pageSize = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;

                UserControl target = uc;   // giữ lại để hàm gọi lại biết ghi vào ô nào
                frmSubIcd frm = new frmSubIcd(
                    new DelegateRefeshIcdChandoanphu(
                        delegate(string icdCodes, string icdNames) { SetHcmIcdValue(target, icdCodes, icdNames); }),
                    code ?? "", name ?? "", pageSize, new List<HIS_ICD>());
                frm.ShowDialog();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Đọc mã và tên bệnh đang chọn của một ô.</summary>
        private void GetHcmIcdValue(UserControl uc, out string icdCode, out string icdName)
        {
            GetSytIcdValue(uc, out icdCode, out icdName);
            return;
#pragma warning disable 0162
            try
            {
                SecondaryIcdProcessor processor;
                if (uc == null || !hcmIcdProcessors.TryGetValue(uc, out processor)) return;
                SecondaryIcdDataADO data = processor.GetValue(uc) as SecondaryIcdDataADO;
                if (data == null) return;
                icdCode = data.ICD_SUB_CODE;
                icdName = data.ICD_TEXT;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ mã và tên bệnh vào một ô. Truyền null để xóa trống.</summary>
        private void SetHcmIcdValue(UserControl uc, string icdCode, string icdName)
        {
            SetSytIcdValue(uc, icdCode, icdName);
            return;
#pragma warning disable 0162
            try
            {
                SecondaryIcdProcessor processor;
                if (uc == null || !hcmIcdProcessors.TryGetValue(uc, out processor)) return;

                if (string.IsNullOrEmpty(icdCode) && string.IsNullOrEmpty(icdName))
                {
                    processor.Reload(uc, null);
                    return;
                }
                SecondaryIcdDataADO data = new SecondaryIcdDataADO();
                data.ICD_SUB_CODE = icdCode;
                data.ICD_TEXT = icdName;
                processor.Reload(uc, data);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Khóa hoặc mở một ô chọn ICD.</summary>
        private void SetHcmIcdEnabled(UserControl uc, bool enabled)
        {
            SetSytIcdEnabled(uc, enabled);
            return;
#pragma warning disable 0162
            try
            {
                SecondaryIcdProcessor processor;
                if (uc == null || !hcmIcdProcessors.TryGetValue(uc, out processor)) return;
                processor.ReadOnly(uc, !enabled);
                uc.Enabled = enabled;
                if (uc.Parent != null)
                {
                    foreach (Control c in uc.Parent.Controls)
                    {
                        if (c is SimpleButton) c.Enabled = enabled;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Tính bố cục =====

        /// <summary>
        /// Đặt lại vị trí/bề rộng toàn bộ dòng theo bề ngang thật của tab. Mọi dòng có CÙNG bề rộng
        /// tổng; các ô nhập trong 1 dòng chia đều phần còn lại sau khi trừ nhãn. Gọi lúc dựng và
        /// mỗi lần đổi kích thước cửa sổ.
        /// </summary>
        private void LayoutClinicalExamHcm()
        {
            try
            {
                if (scrollClinicalExamHcm == null || clinicalExamHcmVisualRows.Count == 0) return;

                // Bề rộng dùng chung cho mọi dòng (tính theo mục thụt lề sâu nhất để mọi dòng bằng nhau).
                int deepest = HCM_MARGIN + HCM_INDENT + HCM_INDENT;
                int rowWidth = scrollClinicalExamHcm.ClientSize.Width - deepest - HCM_MARGIN;
                if (rowWidth < HCM_UC_MIN_WIDTH) rowWidth = HCM_UC_MIN_WIDTH;

                scrollClinicalExamHcm.SuspendLayout();
                try
                {
                    int y = HCM_MARGIN;
                    foreach (HcmVisualRow vr in clinicalExamHcmVisualRows)
                    {
                        int x = HCM_MARGIN + vr.Indent;

                        if (vr.Title != null)
                        {
                            vr.Title.Location = new System.Drawing.Point(x, y);
                            vr.Title.Size = new System.Drawing.Size(rowWidth, vr.Height);
                        }
                        else if (vr.Full != null)
                        {
                            vr.Full.Location = new System.Drawing.Point(x, y);
                            vr.Full.Size = new System.Drawing.Size(rowWidth, vr.Height);
                        }
                        else if (vr.Cells.Count > 0)
                        {
                            LayoutHcmCells(vr, x, y, rowWidth);
                        }

                        y += vr.Height + HCM_ROW_GAP;
                        // Sau dòng cuối của mục (dòng Phân loại / Người khám) thì cách thêm.
                        if (vr.Cells.Count == 2 && vr.Cells[0].Lbl != null
                            && vr.Cells[0].Lbl.Text == "Phân loại:") y += HCM_SECTION_GAP;
                    }
                }
                finally { scrollClinicalExamHcm.ResumeLayout(true); }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đặt các ô trong 1 dòng: nhãn giữ bề rộng cố định, ô nhập chia đều phần còn lại.</summary>
        private void LayoutHcmCells(HcmVisualRow vr, int x, int y, int rowWidth)
        {
            int editCount = 0, sumLabel = 0;
            foreach (HcmCell c in vr.Cells)
            {
                sumLabel += c.LblWidth;
                if (c.Edit != null) editCount++;
            }
            int gaps = (vr.Cells.Count - 1) * HCM_CELL_GAP;
            int editWidth = 0;
            if (editCount > 0)
            {
                editWidth = (rowWidth - sumLabel - gaps) / editCount;
                if (editWidth < HCM_EDIT_MIN_W) editWidth = HCM_EDIT_MIN_W;
            }

            foreach (HcmCell c in vr.Cells)
            {
                if (c.Lbl != null)
                {
                    c.Lbl.Location = new System.Drawing.Point(x, y + 3);
                    c.Lbl.Size = new System.Drawing.Size(c.LblWidth, 16);
                }
                x += c.LblWidth;
                if (c.Edit != null)
                {
                    int w = vr.StretchCells ? editWidth : c.Edit.Width;
                    c.Edit.Location = new System.Drawing.Point(x, y);
                    c.Edit.Size = new System.Drawing.Size(w, HCM_ROW_H);
                    x += w;
                }
                x += HCM_CELL_GAP;
            }
        }

        #endregion

        /// <summary>
        /// Khởi tạo cụm ICD-10 + nạp dữ liệu 2 combo — chạy 1 lần khi mở tab lần đầu.
        /// Nạp bằng đúng 2 hàm dùng chung của form (Phân loại sức khỏe / Người khám).
        /// </summary>
        private void EnsureClinicalExamHcmInited()
        {
            try
            {
                if (isClinicalExamHcmInited) return;
                // Danh mục ICD dựng MỘT LẦN rồi dùng chung cho mọi ô chọn — mỗi mục có 2 ô,
                // 15 mục là 30 ô; sắp xếp lại danh mục cho từng ô sẽ rất chậm.
                // Bat khoa cau hinh cong SYT -> dung danh muc ICD cua cong; nguoc lai dung cua HIS.
                List<HIS_ICD> icdSource = sytIcdSource;
                hcmIcdFromSyt = (sytIcdItems != null && sytIcdItems.Count > 0);
                if (!hcmIcdFromSyt)
                {
                    // Danh mục của cổng chưa về -> tạm dùng của HIS để người dùng nhập được ngay,
                    // và RefreshHcmIcdFromSytCatalog() sẽ dựng lại khi danh mục về.
                    LogSystem.Warn("SytCatalog: danh muc ICD cua cong CHUA co luc mo tab Kham lam sang"
                        + " HCM -> tam dung danh muc ICD cua HIS");
                    icdSource = BackendDataWorker.Get<HIS_ICD>();
                }
                icdSource = (icdSource != null)
                    ? icdSource.OrderBy(x => x.ICD_CODE).ToList()
                    : new List<HIS_ICD>();

                foreach (ClinicalExamHcmRow row in clinicalExamHcmRows)
                {
                    row.UcPreIcd = BuildHcmIcdEditor(row.PnlPreIcd, icdSource);
                    row.UcFinalIcd = BuildHcmIcdEditor(row.PnlFinalIcd, icdSource);
                    if (row.ChkNormal != null && row.ChkNormal.Checked)
                    {
                        SetHcmIcdEnabled(row.UcPreIcd, false);
                        SetHcmIcdEnabled(row.UcFinalIcd, false);
                    }
                    if (row.CboRank != null) SetDataCboRank(row.CboRank);
                    if (row.CboLoginName != null) SetDataCboExamLoginName(row.CboLoginName);
                }
                isClinicalExamHcmInited = true;
                LayoutClinicalExamHcm();

                // Nối ô Phân loại / Người khám với tab "Khám lâm sàng" cũ (cùng một cột trong bảng KSK).
                InitHcmRankMirrors();

                // Các ô chọn bệnh vừa dựng xong -> giờ mới đổ được dữ liệu đã lưu vào tab này.
                FillKskSytHcmClinicalControls();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        // Nạp/lưu dữ liệu tab này: xem frmEnterKskInfomantionVer2___SytHcmData.cs.
        // Ghi chú giữ lại vì có ràng buộc thứ tự khi nạp — mỗi mục:
        //   - chưa phát hiện bất thường -> row.ChkNormal.Checked                     (1 cột cờ)
        //   - chẩn đoán sơ bộ           -> GetHcmIcdValue(row.UcPreIcd)               (mã + tên)
        //   - chẩn đoán xác định        -> GetHcmIcdValue(row.UcFinalIcd)             (mã + tên)
        //   - nạp lại                   -> SetHcmIcdValue(uc, mã, tên) rồi gán ChkNormal SAU CÙNG
        //     (gán ChkNormal trước sẽ xóa trống hai ô chọn bệnh vừa đổ vào)
        //   - phân loại      -> row.CboRank.EditValue        (cột EXAM_*_RANK hiện có)
        //   - người khám     -> row.CboLoginName.EditValue   (cột EXAM_*_LOGINNAME hiện có)
        //     LƯU Ý mục "phukhoa": bảng KSK cũ CHƯA CÓ cột phân loại / người khám cho phụ khoa
        //     (chỉ có EXAM_OBSTETRIC_* dùng chung cho sản phụ khoa) -> phải thêm cột riêng
        //     cho phụ khoa vào bảng dữ liệu mẫu M4, xem PTTK mục 2.2.c
        //   - từ chối khám   -> row.ChkRefuse.Checked         (sankhoa và phukhoa mỗi bên 1 cột cờ)
        //   - trị số đo      -> row.Fields["<khóa>"].Text, dùng chung cột đang có ở tab Khám lâm sàng:
        //       mat_khongkinh_mp/mt   -> EXAM_EYESIGHT_RIGHT / EXAM_EYESIGHT_LEFT
        //       mat_cokinh_mp/mt      -> EXAM_EYESIGHT_GLASS_RIGHT / EXAM_EYESIGHT_GLASS_LEFT
        //       mat_kinhlo_mp/mt      -> CHƯA CÓ CỘT, cần bổ sung
        //       mat_docau_mp/mt · mat_dotru_mp/mt · mat_truc_mp/mt -> CHƯA CÓ CỘT, cần bổ sung
        //       tmh_taitrai_noithuong -> EXAM_ENT_LEFT_NORMAL   · tmh_taitrai_noitham -> EXAM_ENT_LEFT_WHISPER
        //       tmh_taiphai_noithuong -> EXAM_ENT_RIGHT_NORMAL  · tmh_taiphai_noitham -> EXAM_ENT_RIGHT_WHISPER
        //       so do rang            -> xem TODO(BE) trong frmEnterKskInfomantionVer2___ClinicalExamHcmTeeth.cs
        //   - từ chối khám sản khoa -> row.ChkRefuse.Checked (cột mới, xem PTTK)
        // Khi đẩy mẫu M4: lựa chọn 1 -> cờ "chưa phát hiện bất thường"; lựa chọn 2 -> mã ICD vào chỉ tiêu
        // chẩn đoán sơ bộ; lựa chọn 3 -> mã ICD vào chỉ tiêu chẩn đoán xác định.
        // LƯU Ý: 8 ô của mục Mắt (kính lỗ MP/MT và khúc xạ độ cầu/độ trụ/trục mỗi mắt) là ô MỚI —
        // tab "Khám lâm sàng" cũ không có và bảng KSK trên 18 tuổi CHƯA CÓ CỘT. Phải bổ sung 8 cột
        // (kiểu VARCHAR2, cho phép trống) trước khi nạp/lưu; cập nhật lại PTTK mục thiết kế Database.
    }
}
