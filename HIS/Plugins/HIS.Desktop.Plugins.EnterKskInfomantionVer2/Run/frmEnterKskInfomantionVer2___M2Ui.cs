/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong61141 - Mau 02 (TT 25/2026/TT-BYT) on tab "Ksk duoi 18 tuoi", portal M2 of So Y te TP.HCM.
 *
 * Builds 3 extra sub-tabs inside the under-18 tab control (xtraTabControl3), appended at the END so
 * indexes of the existing sub-tabs do not change:
 *   - "Mau 02 - Hanh chinh, tien su" : payer / place / school / guardian / history / vaccination
 *   - "Mau 02 - Danh gia tam than"   : B1 (ADHD, 18 questions) + B2 (ASD, 10 questions)
 *   - "Mau 02 - Kham lam sang"       : 10 sections (normal / pre ICD / final ICD), spine, 52 teeth,
 *                                      paraclinical results, suggestion
 *
 * MULTI-SITE SAFETY: nothing is built unless the hospital declared the portal connection key
 * (IsSytHcmDeclared) — same switch as M3/M4. Not declared -> no control, no API call, no network.
 *
 * All controls are built once at form load; catalog-bound controls are (re)bound in BindM2Catalogs,
 * which is called again when the portal catalogs arrive on the background thread.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Declarations =====

        private const string M2_CAT__DOI_TUONG = "KSKDK_DoiTuongKham";
        private const string M2_CAT__DIA_DIEM = "Diadiemkham";
        private const string M2_CAT__HINH_THUC = "M2_HTKhamSucKhoe";
        private const string M2_CAT__HINH_THUC_CT = "M2_HTChiTraKhamSucKhoe";
        private const string M2_CAT__TRUONG = "TruongHoc";
        private const string M2_CAT__XA_PHUONG = "DiaChiHienTai_XaPhuong";
        private const string M2_CAT__TS_GIA_DINH = "TS_GiaDinh_MacBenh_DanhSachBenh";
        private const string M2_CAT__TS_BAN_THAN = "TS_BanThan_MacBenh_DanhSachBenh";
        private const string M2_CAT__SAN_KHOA = "SanKhoa_GhiChu";
        private const string M2_CAT__COT_SONG = "CotSong";
        private const string M2_CAT__CONG_COT_SONG = "CongCotSong";
        private const string M2_CAT__VEO_COT_SONG = "VeoCotSong";
        private const string M2_CAT__MUC_DO_VEO = "MucDoVeoCotSong";
        private const string M2_CAT__TINH_TRANG_RANG = "NenTangKSK_TinhTrangRang";
        private const string M2_CAT__DE_NGHI = "KetLuan_DeNghi";

        /// <summary>Catalog codes added to the default download list for M2 (see SYT_CATALOG_CODES_DEFAULT).</summary>
        private static readonly string[] M2_CATALOG_CODES = new string[]
        {
            M2_CAT__DOI_TUONG, M2_CAT__HINH_THUC, M2_CAT__HINH_THUC_CT, M2_CAT__TRUONG, "MoiQuanHeVoiTre",
            "Tinh_trang_tiem", M2_CAT__SAN_KHOA, "RoiLoanGiamChuY_6_18_tuoi", "PhoTuKy_6_11_tuoi",
            M2_CAT__COT_SONG, M2_CAT__CONG_COT_SONG, M2_CAT__VEO_COT_SONG, M2_CAT__MUC_DO_VEO
        };

        private const int M2_LBL_W = 175;
        private const int M2_EDIT_W = 300;
        private const int M2_COL2_X = 495;
        private const int M2_LBL2_W = 120;
        private const int M2_FULL_W = 740;
        private const int M2_ROW_H = 22;
        private const int M2_ROW_GAP = 6;
        private const int M2_MARGIN = 10;
        private const int M2_TOOTH_W = 54;

        private bool isM2Inited = false;

        private DevExpress.XtraTab.XtraTabPage tabM2Info;
        private DevExpress.XtraTab.XtraTabPage tabM2Mental;
        private DevExpress.XtraTab.XtraTabPage tabM2Clinical;

        /// <summary>Simple vertical layout cursor inside one scrollable pane.</summary>
        private class M2Pane
        {
            public XtraScrollableControl Scroll { get; set; }
            public int Y { get; set; }
        }

        // Administration
        private CheckedComboBoxEdit cboM2PatientTypes;
        private GridLookUpEdit cboM2ExamPlace;
        private GridLookUpEdit cboM2PaySource;
        private GridLookUpEdit cboM2PaySourceDetail;
        private TextEdit txtM2PayOther;
        // School
        private RadioGroup rdoM2GoToSchool;
        private GridLookUpEdit cboM2School;
        private TextEdit txtM2Class;
        private TextEdit txtM2SchoolAddress;
        private GridLookUpEdit cboM2SchoolWard;
        // Guardian
        private TextEdit txtM2GuardianCccd;
        private TextEdit txtM2GuardianPhone;
        // History
        private RadioGroup rdoM2FamilyHas;
        private CheckedComboBoxEdit cboM2FamilyList;
        private RadioGroup rdoM2Obstetric;
        private CheckedComboBoxEdit cboM2ObstetricAbnormal;
        private TextEdit txtM2MotherDisease;
        private RadioGroup rdoM2PersonalHas;
        private CheckedComboBoxEdit cboM2PersonalList;
        private PanelControl pnlM2PersonalIcd;
        private UserControl ucM2PersonalIcd;
        private RadioGroup rdoM2Treating;
        private MemoEdit txtM2TreatingMedicine;
        // Vaccination
        private GridControl grdM2Vaccine;
        private GridView grvM2Vaccine;
        private List<KskM2VaccineRowADO> m2VaccineRows;
        // Mental
        private DateEdit dteM2AdhdDate;
        private GridControl grdM2Adhd;
        private GridView grvM2Adhd;
        private List<KskM2QuestionRowADO> m2AdhdRows;
        private MemoEdit txtM2AdhdResult;
        private TextEdit txtM2AdhdNote;
        private DateEdit dteM2AsdDate;
        private GridControl grdM2Asd;
        private GridView grvM2Asd;
        private List<KskM2QuestionRowADO> m2AsdRows;
        private TextEdit txtM2AsdScore;
        private MemoEdit txtM2AsdResult;
        private TextEdit txtM2AsdNote;
        private LabelControl lblM2AsdAgeNote;
        // Clinical
        private class M2SectionRow
        {
            public KskM2SectionDef Def { get; set; }
            public CheckEdit ChkNormal { get; set; }
            public PanelControl PnlPre { get; set; }
            public PanelControl PnlFinal { get; set; }
            public UserControl UcPre { get; set; }
            public UserControl UcFinal { get; set; }
        }
        private readonly List<M2SectionRow> m2SectionRows = new List<M2SectionRow>();
        private RadioGroup rdoM2Spine;
        private RadioGroup rdoM2Kyphosis;
        private RadioGroup rdoM2Scoliosis;
        private RadioGroup rdoM2ScoliosisLevel;
        private readonly Dictionary<string, LookUpEdit> m2ToothEdits = new Dictionary<string, LookUpEdit>();
        private DateEdit dteM2TestDate;
        private TextEdit txtM2TestUnit;
        private MemoEdit txtM2TestResult;
        private MemoEdit txtM2ImagingResult;
        private RadioGroup rdoM2Suggest;
        private TextEdit txtM2SuggestOther;

        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider m2ErrorProvider;

        /// <summary>Guard: values are being filled by code, skip change handlers.</summary>
        private bool m2Filling = false;

        #endregion

        #region ===== Build =====

        /// <summary>
        /// Builds the 3 Mau 02 sub-tabs. Called once at form load, right after the M3/M4 tabs.
        /// Not declared portal -> returns immediately (multi-site safety).
        /// </summary>
        private void InitM2Tabs()
        {
            try
            {
                if (isM2Inited) return;
                if (this.xtraTabControl3 == null) return;
                if (!IsSytHcmDeclared())
                {
                    LogSystem.Debug("SytHcm/M2: chua khai bao cau hinh cong -> KHONG dung tab Mau 02");
                    return;
                }

                m2ErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this);

                this.xtraTabControl3.SuspendLayout();
                try
                {
                    tabM2Info = NewM2Tab("tabM2Info", "Mẫu 02 - Hành chính, tiền sử");
                    BuildM2InfoTab((XtraScrollableControl)tabM2Info.Controls[0]);

                    tabM2Mental = NewM2Tab("tabM2Mental", "Mẫu 02 - Đánh giá tâm thần");
                    BuildM2MentalTab((XtraScrollableControl)tabM2Mental.Controls[0]);

                    tabM2Clinical = NewM2Tab("tabM2Clinical", "Mẫu 02 - Khám lâm sàng");
                    BuildM2ClinicalTab((XtraScrollableControl)tabM2Clinical.Controls[0]);

                    this.xtraTabControl3.TabPages.Add(tabM2Info);
                    this.xtraTabControl3.TabPages.Add(tabM2Mental);
                    this.xtraTabControl3.TabPages.Add(tabM2Clinical);
                }
                finally { this.xtraTabControl3.ResumeLayout(true); }

                ApplyM2GuardianCaption();
                HideM2ReplacedControls();
                ShowM2PhysicalRank();
                BindM2Catalogs();
                ApplyM2EnableRules();
                isM2Inited = true;
                // The default tab may already be filled before this point (lazy load runs earlier).
                FillM2Controls();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private DevExpress.XtraTab.XtraTabPage NewM2Tab(string name, string text)
        {
            DevExpress.XtraTab.XtraTabPage tab = new DevExpress.XtraTab.XtraTabPage();
            tab.Name = name;
            tab.Text = text;
            XtraScrollableControl scroll = new XtraScrollableControl();
            scroll.Name = "scroll" + name;
            scroll.Dock = DockStyle.Fill;
            scroll.AutoScroll = true;
            tab.Controls.Add(scroll);
            return tab;
        }

        private void BuildM2InfoTab(XtraScrollableControl scroll)
        {
            M2Pane p = new M2Pane { Scroll = scroll, Y = M2_MARGIN };

            M2Title(p, "THÔNG TIN ĐỐI TƯỢNG - CHI TRẢ");
            cboM2PatientTypes = NewM2CheckedCombo("cboM2PatientTypes");
            cboM2ExamPlace = NewM2Lookup("cboM2ExamPlace");
            M2Row2(p, "Đối tượng khám", cboM2PatientTypes, "Địa điểm khám", cboM2ExamPlace);
            cboM2PaySource = NewM2Lookup("cboM2PaySource");
            cboM2PaySource.EditValueChanged += M2_EnableRule_Changed;
            cboM2PaySourceDetail = NewM2Lookup("cboM2PaySourceDetail");
            M2Row2(p, "Hình thức chi trả", cboM2PaySource, "Chi tiết", cboM2PaySourceDetail);
            txtM2PayOther = NewM2Text("txtM2PayOther", 4000);
            M2Row1(p, "Nguồn khác, ghi rõ", txtM2PayOther, M2_FULL_W, M2_ROW_H);

            M2Title(p, "HỌC TẬP");
            rdoM2GoToSchool = NewM2YesNo("rdoM2GoToSchool", "Có", "Không");
            rdoM2GoToSchool.EditValueChanged += M2_EnableRule_Changed;
            M2Row1(p, "Trẻ có đi học", rdoM2GoToSchool, 200, M2_ROW_H);
            cboM2School = NewM2Lookup("cboM2School");
            cboM2School.Properties.PopupFormWidth = 500;
            txtM2Class = NewM2Text("txtM2Class", 10);
            M2Row2(p, "Tên trường", cboM2School, "Lớp", txtM2Class);
            txtM2SchoolAddress = NewM2Text("txtM2SchoolAddress", 1000);
            cboM2SchoolWard = NewM2Lookup("cboM2SchoolWard");
            M2Row2(p, "Địa chỉ trường", txtM2SchoolAddress, "Xã/Phường", cboM2SchoolWard);

            M2Title(p, "MẸ HOẶC NGƯỜI GIÁM HỘ");
            M2Note(p, "Họ tên và mối quan hệ với trẻ nhập ở tab \"Khám thể lực\" (ô Họ và tên mẹ hoặc người giám hộ).");
            txtM2GuardianCccd = NewM2Text("txtM2GuardianCccd", 12);
            txtM2GuardianPhone = NewM2Text("txtM2GuardianPhone", 10);
            M2Row2(p, "CCCD người giám hộ", txtM2GuardianCccd, "Điện thoại", txtM2GuardianPhone);

            M2Title(p, "TIỀN SỬ BỆNH TẬT");
            rdoM2FamilyHas = NewM2YesNo("rdoM2FamilyHas", "Có", "Không");
            rdoM2FamilyHas.EditValueChanged += M2_EnableRule_Changed;
            cboM2FamilyList = NewM2CheckedCombo("cboM2FamilyList");
            M2RowRadioPlus(p, "1. Tiền sử gia đình", rdoM2FamilyHas, 200, cboM2FamilyList);

            rdoM2Obstetric = NewM2YesNo("rdoM2Obstetric", "Bình thường", "Không bình thường");
            rdoM2Obstetric.EditValueChanged += M2_EnableRule_Changed;
            cboM2ObstetricAbnormal = NewM2CheckedCombo("cboM2ObstetricAbnormal");
            M2RowRadioPlus(p, "2a. Sản khoa", rdoM2Obstetric, 260, cboM2ObstetricAbnormal);
            txtM2MotherDisease = NewM2Text("txtM2MotherDisease", 1000);
            M2Row1(p, "Mẹ bị bệnh khi mang thai", txtM2MotherDisease, M2_FULL_W, M2_ROW_H);

            rdoM2PersonalHas = NewM2YesNo("rdoM2PersonalHas", "Có", "Không");
            rdoM2PersonalHas.EditValueChanged += M2_EnableRule_Changed;
            cboM2PersonalList = NewM2CheckedCombo("cboM2PersonalList");
            M2RowRadioPlus(p, "2c. Tiền sử bệnh/tật", rdoM2PersonalHas, 200, cboM2PersonalList);
            pnlM2PersonalIcd = NewM2IcdPanel("pnlM2PersonalIcd");
            M2Row1(p, "Bệnh khác (mã ICD)", pnlM2PersonalIcd, M2_FULL_W, M2_ROW_H);
            ucM2PersonalIcd = BuildSytIcdEditor(pnlM2PersonalIcd);

            rdoM2Treating = NewM2YesNo("rdoM2Treating", "Có", "Không");
            rdoM2Treating.EditValueChanged += M2_EnableRule_Changed;
            M2Row1(p, "2d. Đang điều trị bệnh", rdoM2Treating, 200, M2_ROW_H);
            txtM2TreatingMedicine = new MemoEdit();
            txtM2TreatingMedicine.Name = "txtM2TreatingMedicine";
            txtM2TreatingMedicine.TextChanged += TxtM2TreatingMedicine_TextChanged;
            M2Row1(p, "Tên bệnh, thuốc đang dùng", txtM2TreatingMedicine, M2_FULL_W, 46);
            if (this.txtMedicineUsing3 != null) this.txtMedicineUsing3.TextChanged += TxtMedicineUsing3_M2Mirror;

            M2Title(p, "2b. TIÊM CHỦNG");
            SimpleButton btnAllDone = new SimpleButton();
            btnAllDone.Name = "btnM2VaccineAllDone";
            btnAllDone.Text = "Đánh dấu tất cả: Đã tiêm";
            btnAllDone.Click += BtnM2VaccineAllDone_Click;
            M2Place(p, btnAllDone, M2_MARGIN, 180, M2_ROW_H);
            p.Y += M2_ROW_H + M2_ROW_GAP;
            BuildM2VaccineGrid();
            M2Place(p, grdM2Vaccine, M2_MARGIN, M2_LBL_W + M2_FULL_W, 26 * 21 + 30);
            p.Y += 26 * 21 + 30 + M2_ROW_GAP;
            M2BottomPad(p);
        }

        private void BuildM2MentalTab(XtraScrollableControl scroll)
        {
            M2Pane p = new M2Pane { Scroll = scroll, Y = M2_MARGIN };

            M2Title(p, "B1. SÀNG LỌC RỐI LOẠN GIẢM CHÚ Ý - TĂNG ĐỘNG");
            dteM2AdhdDate = NewM2Date("dteM2AdhdDate");
            M2Row1(p, "Ngày đánh giá", dteM2AdhdDate, 150, M2_ROW_H);
            m2AdhdRows = KskM2Definition.ADHD_QUESTIONS
                .Select(q => new KskM2QuestionRowADO { No = q.No, Field = q.Field, Text = q.Text }).ToList();
            grdM2Adhd = BuildM2QuestionGrid("grdM2Adhd", m2AdhdRows,
                new string[] { "Không có", "Thỉnh thoảng", "Thường xuyên" }, out grvM2Adhd);
            int hAdhd = 18 * 36 + 30;
            M2Place(p, grdM2Adhd, M2_MARGIN, M2_LBL_W + M2_FULL_W, hAdhd);
            p.Y += hAdhd + M2_ROW_GAP;
            txtM2AdhdResult = NewM2Memo("txtM2AdhdResult", 200);
            M2Row1(p, "Kết quả", txtM2AdhdResult, M2_FULL_W, 40);
            txtM2AdhdNote = NewM2Text("txtM2AdhdNote", 200);
            M2Row1(p, "Ghi chú", txtM2AdhdNote, M2_FULL_W, M2_ROW_H);

            M2Title(p, "B2. SÀNG LỌC RỐI LOẠN PHỔ TỰ KỶ (chỉ đánh giá trẻ từ 6 đến 11 tuổi)");
            lblM2AsdAgeNote = M2Note(p, "");
            dteM2AsdDate = NewM2Date("dteM2AsdDate");
            txtM2AsdScore = NewM2Text("txtM2AsdScore", 5);
            txtM2AsdScore.Properties.ReadOnly = true;
            M2Row2(p, "Ngày đánh giá", dteM2AsdDate, "Tổng điểm", txtM2AsdScore);
            m2AsdRows = KskM2Definition.ASD_QUESTIONS
                .Select(q => new KskM2QuestionRowADO { No = q.No, Field = q.Field, Text = q.Text }).ToList();
            grdM2Asd = BuildM2QuestionGrid("grdM2Asd", m2AsdRows,
                new string[] { "Hoàn toàn đồng ý", "Có chút đồng ý", "Có chút không đồng ý", "Hoàn toàn không đồng ý" },
                out grvM2Asd);
            int hAsd = 10 * 36 + 30;
            M2Place(p, grdM2Asd, M2_MARGIN, M2_LBL_W + M2_FULL_W, hAsd);
            p.Y += hAsd + M2_ROW_GAP;
            txtM2AsdResult = NewM2Memo("txtM2AsdResult", 200);
            M2Row1(p, "Kết quả", txtM2AsdResult, M2_FULL_W, 40);
            txtM2AsdNote = NewM2Text("txtM2AsdNote", 200);
            M2Row1(p, "Ghi chú", txtM2AsdNote, M2_FULL_W, M2_ROW_H);
            M2BottomPad(p);
        }

        private void BuildM2ClinicalTab(XtraScrollableControl scroll)
        {
            M2Pane p = new M2Pane { Scroll = scroll, Y = M2_MARGIN };
            M2Note(p, "Phân loại sức khỏe, người khám, thị lực, thính lực của từng chuyên khoa nhập ở tab \"Khám lâm sàng\".");

            foreach (KskM2SectionDef def in KskM2Definition.SECTIONS)
            {
                if (def.Key == "cotsong_clskhac") BuildM2SpineBlock(p);
                if (def.Key == "rhm") BuildM2TeethBlock(p);
                BuildM2Section(p, def);
            }

            M2Title(p, "CẬN LÂM SÀNG");
            dteM2TestDate = NewM2Date("dteM2TestDate");
            txtM2TestUnit = NewM2Text("txtM2TestUnit", 200);
            M2Row2(p, "Ngày xét nghiệm", dteM2TestDate, "Đơn vị XN", txtM2TestUnit);
            txtM2TestResult = NewM2Memo("txtM2TestResult", 200);
            M2Row1(p, "Kết quả xét nghiệm", txtM2TestResult, M2_FULL_W, 40);
            txtM2ImagingResult = NewM2Memo("txtM2ImagingResult", 200);
            M2Row1(p, "Kết quả CĐHA", txtM2ImagingResult, M2_FULL_W, 40);

            M2Title(p, "ĐỀ NGHỊ");
            rdoM2Suggest = NewM2CatalogRadio("rdoM2Suggest", 1);
            M2Row1(p, "Đề nghị", rdoM2Suggest, M2_FULL_W, 120);
            txtM2SuggestOther = NewM2Text("txtM2SuggestOther", 500);
            M2Row1(p, "Khác, ghi rõ", txtM2SuggestOther, M2_FULL_W, M2_ROW_H);
            M2BottomPad(p);
        }

        private void BuildM2Section(M2Pane p, KskM2SectionDef def)
        {
            M2Title(p, def.Caption);
            M2SectionRow row = new M2SectionRow { Def = def };
            row.ChkNormal = new CheckEdit();
            row.ChkNormal.Name = "chkM2Normal_" + def.Key;
            row.ChkNormal.Properties.Caption = "Chưa phát hiện bất thường";
            row.ChkNormal.Tag = row;
            row.ChkNormal.CheckedChanged += ChkM2Normal_CheckedChanged;
            M2Place(p, row.ChkNormal, M2_MARGIN + M2_LBL_W, 250, M2_ROW_H);
            p.Y += M2_ROW_H + M2_ROW_GAP;

            row.PnlPre = NewM2IcdPanel("pnlM2Pre_" + def.Key);
            M2Row1(p, "Chẩn đoán sơ bộ (ICD)", row.PnlPre, M2_FULL_W, M2_ROW_H);
            row.UcPre = BuildSytIcdEditor(row.PnlPre);
            row.PnlFinal = NewM2IcdPanel("pnlM2Final_" + def.Key);
            M2Row1(p, "Chẩn đoán xác định (ICD)", row.PnlFinal, M2_FULL_W, M2_ROW_H);
            row.UcFinal = BuildSytIcdEditor(row.PnlFinal);
            m2SectionRows.Add(row);
        }

        private void BuildM2SpineBlock(M2Pane p)
        {
            M2Title(p, "1g. Khám cột sống (đánh giá còng vẹo cột sống cho trẻ lớp 1, lớp 6 và lớp 10)");
            rdoM2Spine = NewM2CatalogRadio("rdoM2Spine", 3);
            rdoM2Spine.EditValueChanged += M2_EnableRule_Changed;
            M2Row1(p, "Cột sống", rdoM2Spine, 400, M2_ROW_H);
            rdoM2Kyphosis = NewM2CatalogRadio("rdoM2Kyphosis", 3);
            M2Row1(p, "Còng cột sống", rdoM2Kyphosis, 400, M2_ROW_H);
            rdoM2Scoliosis = NewM2CatalogRadio("rdoM2Scoliosis", 3);
            rdoM2Scoliosis.EditValueChanged += M2_EnableRule_Changed;
            M2Row1(p, "Vẹo cột sống", rdoM2Scoliosis, 400, M2_ROW_H);
            rdoM2ScoliosisLevel = NewM2CatalogRadio("rdoM2ScoliosisLevel", 3);
            M2Row1(p, "Mức độ vẹo", rdoM2ScoliosisLevel, M2_FULL_W, M2_ROW_H);
        }

        private void BuildM2TeethBlock(M2Pane p)
        {
            M2Title(p, "4. Răng - Hàm - Mặt: sơ đồ răng (răng vĩnh viễn + răng sữa)");
            SimpleButton btnNormal = new SimpleButton();
            btnNormal.Name = "btnM2TeethAllNormal";
            btnNormal.Text = "Đặt tất cả: Bình thường";
            btnNormal.Click += BtnM2TeethAllNormal_Click;
            M2Place(p, btnNormal, M2_MARGIN, 170, M2_ROW_H);
            SimpleButton btnClear = new SimpleButton();
            btnClear.Name = "btnM2TeethClear";
            btnClear.Text = "Xóa sơ đồ";
            btnClear.Click += BtnM2TeethClear_Click;
            M2Place(p, btnClear, M2_MARGIN + 180, 100, M2_ROW_H);
            p.Y += M2_ROW_H + M2_ROW_GAP;

            M2Note(p, "Hàm trên");
            M2ToothRow(p, KskM2Definition.TEETH_UPPER, 0);
            M2ToothRow(p, KskM2Definition.TEETH_PRIMARY_UPPER, 3);
            M2Note(p, "Hàm dưới");
            M2ToothRow(p, KskM2Definition.TEETH_PRIMARY_LOWER, 3);
            M2ToothRow(p, KskM2Definition.TEETH_LOWER, 0);

            // Legend as on the paper form (digits: permanent teeth, letters: primary teeth).
            M2Note(p, "Tình trạng răng (số: răng vĩnh viễn, chữ: răng sữa):  0/A: Bình thường    1/B: Sâu    2/C: Trám sâu lại    3/D: Trám tốt    4/E: Mất do sâu");
            M2Note(p, "5: Mất lý do khác    6/F: Bít hố rãnh    7/G: Trụ, cầu    8: Chưa mọc    9: Loại trừ");
        }

        /// <summary>One row of teeth: tooth number above, status drop-down below.</summary>
        private void M2ToothRow(M2Pane p, string[] teeth, int offsetCells)
        {
            int x = M2_MARGIN + offsetCells * (M2_TOOTH_W + 2);
            foreach (string t in teeth)
            {
                LabelControl lbl = new LabelControl();
                lbl.Name = "lblM2Tooth_" + t;
                lbl.Text = t;
                lbl.AutoSizeMode = LabelAutoSizeMode.None;
                lbl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                M2Place(p, lbl, x, M2_TOOTH_W, 16);

                LookUpEdit cbo = new LookUpEdit();
                cbo.Name = "cboM2Tooth_" + t;
                cbo.Properties.NullText = "";
                // Cell shows the legend symbol (0/A, 1/B...), the drop-down shows symbol + status name.
                cbo.Properties.DisplayMember = "SYMBOL";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.ShowHeader = false;
                cbo.Properties.PopupWidth = 200;
                cbo.Properties.Columns.Add(new LookUpColumnInfo("SYMBOL", "", 30));
                cbo.Properties.Columns.Add(new LookUpColumnInfo("NAME", "", 170));
                cbo.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                cbo.Properties.Appearance.Options.UseTextOptions = true;
                string toothNo = t;
                cbo.EditValueChanged += (s, e) => UpdateM2ToothTip((LookUpEdit)s, toothNo);
                // Narrow cell: no drop-down arrow, opens on a single click, no typing.
                cbo.Properties.Buttons[0].Visible = false;
                cbo.Properties.ShowDropDown = ShowDropDown.SingleClick;
                cbo.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                cbo.ToolTip = "Răng " + t;
                M2PlaceAt(p, cbo, x, p.Y + 16, M2_TOOTH_W, M2_ROW_H);
                m2ToothEdits[t] = cbo;
                x += M2_TOOTH_W + 2;
            }
            p.Y += 16 + M2_ROW_H + M2_ROW_GAP;
        }

        /// <summary>Tooltip of a tooth cell: tooth number + full status name.</summary>
        private void UpdateM2ToothTip(LookUpEdit cbo, string toothNo)
        {
            try
            {
                KskM2ToothItemADO item = cbo.GetSelectedDataRow() as KskM2ToothItemADO;
                cbo.ToolTip = item != null ? ("Răng " + toothNo + ": " + item.NAME) : ("Răng " + toothNo);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void BuildM2VaccineGrid()
        {
            m2VaccineRows = KskM2Definition.VACCINES.Select(v => new KskM2VaccineRowADO
            {
                Field = v.Field,
                Disease = v.Disease,
                Schedule = v.Schedule,
                Dose = v.Dose
            }).ToList();
            m2VaccineRows.Add(new KskM2VaccineRowADO { Field = KskM2Definition.VACCINE_OTHER_KEY, Disease = "Các loại khác", IsOther = true });

            grdM2Vaccine = new GridControl();
            grdM2Vaccine.Name = "grdM2Vaccine";
            grvM2Vaccine = new GridView(grdM2Vaccine);
            grdM2Vaccine.MainView = grvM2Vaccine;
            grvM2Vaccine.OptionsView.ShowGroupPanel = false;
            grvM2Vaccine.OptionsView.ShowIndicator = false;
            grvM2Vaccine.OptionsCustomization.AllowSort = false;
            grvM2Vaccine.OptionsCustomization.AllowFilter = false;
            grvM2Vaccine.OptionsView.ColumnAutoWidth = true;

            RepositoryItemDateEdit repDate = new RepositoryItemDateEdit();
            repDate.DisplayFormat.FormatString = "dd/MM/yyyy";
            repDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repDate.EditFormat.FormatString = "dd/MM/yyyy";
            repDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repDate.Mask.EditMask = "dd/MM/yyyy";
            RepositoryItemCheckEdit repChk = new RepositoryItemCheckEdit();
            repChk.EditValueChanged += (s, e) => { grvM2Vaccine.PostEditor(); };
            grdM2Vaccine.RepositoryItems.AddRange(new RepositoryItem[] { repDate, repChk });

            AddM2Col(grvM2Vaccine, "Disease", "Phòng bệnh", 190, null, true);
            AddM2Col(grvM2Vaccine, "Schedule", "Lịch khuyến cáo", 150, null, true);
            AddM2Col(grvM2Vaccine, "Dose", "Mũi tiêm/uống", 80, null, true);
            AddM2Col(grvM2Vaccine, "InjectDate", "Ngày tiêm/uống", 100, repDate, true);
            AddM2Col(grvM2Vaccine, "Done", "Đã tiêm", 70, repChk, true);
            AddM2Col(grvM2Vaccine, "NotDone", "Chưa tiêm", 70, repChk, true);
            AddM2Col(grvM2Vaccine, "Unknown", "Không nhớ rõ", 80, repChk, true);

            grvM2Vaccine.ShowingEditor += GrvM2Vaccine_ShowingEditor;
            grvM2Vaccine.CellValueChanged += GrvM2Vaccine_CellValueChanged;
            grdM2Vaccine.DataSource = m2VaccineRows;
        }

        private GridControl BuildM2QuestionGrid(string name, List<KskM2QuestionRowADO> rows, string[] answerCaptions,
            out GridView view)
        {
            GridControl grd = new GridControl();
            grd.Name = name;
            GridView v = new GridView(grd);
            grd.MainView = v;
            v.OptionsView.ShowGroupPanel = false;
            v.OptionsView.ShowIndicator = false;
            v.OptionsView.RowAutoHeight = true;
            v.OptionsCustomization.AllowSort = false;
            v.OptionsCustomization.AllowFilter = false;
            v.OptionsView.ColumnAutoWidth = true;

            RepositoryItemMemoEdit repMemo = new RepositoryItemMemoEdit();
            repMemo.WordWrap = true;
            RepositoryItemCheckEdit repChk = new RepositoryItemCheckEdit();
            repChk.EditValueChanged += (s, e) => { v.PostEditor(); };
            grd.RepositoryItems.AddRange(new RepositoryItem[] { repMemo, repChk });

            AddM2Col(v, "No", "TT", 35, null, false);
            AddM2Col(v, "Text", "Nội dung", 470, repMemo, false);
            string[] fields = { "C1", "C2", "C3", "C4" };
            for (int i = 0; i < answerCaptions.Length; i++)
                AddM2Col(v, fields[i], answerCaptions[i], 85, repChk, true);

            v.CellValueChanged += GrvM2Question_CellValueChanged;
            grd.DataSource = rows;
            view = v;
            return grd;
        }

        private static void AddM2Col(GridView view, string field, string caption, int width, RepositoryItem rep,
            bool allowEdit)
        {
            GridColumn col = view.Columns.AddVisible(field, caption);
            col.Width = width;
            col.OptionsColumn.AllowEdit = allowEdit;
            col.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            col.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            if (rep != null) col.ColumnEdit = rep;
        }

        #endregion

        #region ===== Layout helpers =====

        private LabelControl M2Title(M2Pane p, string text)
        {
            LabelControl lbl = new LabelControl();
            lbl.Name = "lblM2Title_" + p.Scroll.Controls.Count;
            lbl.Text = text;
            lbl.Appearance.Font = new Font(lbl.Font, FontStyle.Bold);
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            p.Y += 4;
            M2Place(p, lbl, M2_MARGIN, M2_LBL_W + M2_FULL_W, 18);
            p.Y += 18 + M2_ROW_GAP;
            return lbl;
        }

        private LabelControl M2Note(M2Pane p, string text)
        {
            LabelControl lbl = new LabelControl();
            lbl.Name = "lblM2Note_" + p.Scroll.Controls.Count;
            lbl.Text = text;
            lbl.Appearance.ForeColor = Color.DimGray;
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            M2Place(p, lbl, M2_MARGIN, M2_LBL_W + M2_FULL_W, 16);
            p.Y += 16 + M2_ROW_GAP;
            return lbl;
        }

        private void M2Row1(M2Pane p, string caption, Control edit, int width, int height)
        {
            M2Caption(p, caption, M2_MARGIN, M2_LBL_W);
            M2Place(p, edit, M2_MARGIN + M2_LBL_W, width, height);
            p.Y += height + M2_ROW_GAP;
        }

        private void M2Row2(M2Pane p, string caption1, Control edit1, string caption2, Control edit2)
        {
            M2Caption(p, caption1, M2_MARGIN, M2_LBL_W);
            M2Place(p, edit1, M2_MARGIN + M2_LBL_W, M2_EDIT_W, M2_ROW_H);
            M2Caption(p, caption2, M2_COL2_X, M2_LBL2_W);
            M2Place(p, edit2, M2_COL2_X + M2_LBL2_W, M2_EDIT_W, M2_ROW_H);
            p.Y += M2_ROW_H + M2_ROW_GAP;
        }

        /// <summary>Radio group followed by an extra editor on the same line (list of diseases...).</summary>
        private void M2RowRadioPlus(M2Pane p, string caption, RadioGroup rdo, int radioWidth, Control extra)
        {
            M2Caption(p, caption, M2_MARGIN, M2_LBL_W);
            M2Place(p, rdo, M2_MARGIN + M2_LBL_W, radioWidth, M2_ROW_H);
            int x = M2_MARGIN + M2_LBL_W + radioWidth + 6;
            M2Place(p, extra, x, M2_MARGIN + M2_LBL_W + M2_FULL_W - x, M2_ROW_H);
            p.Y += M2_ROW_H + M2_ROW_GAP;
        }

        private void M2Caption(M2Pane p, string caption, int x, int width)
        {
            LabelControl lbl = new LabelControl();
            lbl.Name = "lblM2Cap_" + p.Scroll.Controls.Count;
            lbl.Text = caption + ":";
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            lbl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            lbl.ToolTip = caption;
            M2PlaceAt(p, lbl, x, p.Y + 3, width - 6, 16);
        }

        private void M2Place(M2Pane p, Control c, int x, int width, int height)
        {
            M2PlaceAt(p, c, x, p.Y, width, height);
        }

        private static void M2PlaceAt(M2Pane p, Control c, int x, int y, int width, int height)
        {
            c.Location = new Point(x, y);
            c.Size = new Size(width, height);
            p.Scroll.Controls.Add(c);
        }

        private void M2BottomPad(M2Pane p)
        {
            LabelControl pad = new LabelControl();
            pad.Name = "lblM2Pad_" + p.Scroll.Controls.Count;
            pad.Text = "";
            M2Place(p, pad, M2_MARGIN, 10, 20);
        }

        #endregion

        #region ===== Control factories =====

        private TextEdit NewM2Text(string name, int maxLength)
        {
            TextEdit t = new TextEdit();
            t.Name = name;
            t.Properties.MaxLength = maxLength;
            return t;
        }

        private MemoEdit NewM2Memo(string name, int maxLength)
        {
            MemoEdit t = new MemoEdit();
            t.Name = name;
            t.Properties.MaxLength = maxLength;
            return t;
        }

        private DateEdit NewM2Date(string name)
        {
            DateEdit d = new DateEdit();
            d.Name = name;
            d.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            d.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            d.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            d.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            d.Properties.Mask.EditMask = "dd/MM/yyyy";
            d.Properties.NullText = "";
            return d;
        }

        private GridLookUpEdit NewM2Lookup(string name)
        {
            GridLookUpEdit cbo = new GridLookUpEdit();
            cbo.Name = name;
            cbo.MenuManager = this.barManager1;
            cbo.Properties.NullText = "";
            cbo.Properties.DisplayMember = "NAME";
            cbo.Properties.ValueMember = "ID";
            cbo.Properties.DataSource = new List<KskCodeNameADO>();
            cbo.Properties.View.OptionsView.ShowGroupPanel = false;
            cbo.Properties.View.OptionsView.ShowAutoFilterRow = true;
            cbo.Properties.View.OptionsView.ShowColumnHeaders = false;
            cbo.Properties.PopupFormWidth = 350;
            // The delete button is added by InitClearButtonForGridLookUpEdits (whole form, after init).
            cbo.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            cbo.Properties.View.Columns.AddVisible("NAME", "");
            return cbo;
        }

        private CheckedComboBoxEdit NewM2CheckedCombo(string name)
        {
            CheckedComboBoxEdit cbo = new CheckedComboBoxEdit();
            cbo.Name = name;
            cbo.Properties.SeparatorChar = ',';
            cbo.Properties.NullText = "";
            cbo.Properties.DropDownRows = 12;
            return cbo;
        }

        /// <summary>Two-option radio: first option value 1, second option value 0.</summary>
        private RadioGroup NewM2YesNo(string name, string first, string second)
        {
            RadioGroup r = new RadioGroup();
            r.Name = name;
            r.Properties.Columns = 2;
            r.Properties.Items.Add(new RadioGroupItem((short)1, first));
            r.Properties.Items.Add(new RadioGroupItem((short)0, second));
            r.EditValue = null;
            r.Properties.Appearance.BackColor = Color.Transparent;
            r.Properties.Appearance.Options.UseBackColor = true;
            r.Properties.BorderStyle = BorderStyles.NoBorder;
            return r;
        }

        /// <summary>Radio filled from a portal catalog (values = portal ids, long).</summary>
        private RadioGroup NewM2CatalogRadio(string name, int columns)
        {
            RadioGroup r = new RadioGroup();
            r.Name = name;
            r.Properties.Columns = columns;
            r.EditValue = null;
            r.Properties.Appearance.BackColor = Color.Transparent;
            r.Properties.Appearance.Options.UseBackColor = true;
            r.Properties.BorderStyle = BorderStyles.NoBorder;
            return r;
        }

        private static PanelControl NewM2IcdPanel(string name)
        {
            PanelControl pnl = new PanelControl();
            pnl.Name = name;
            pnl.BorderStyle = BorderStyles.NoBorder;
            return pnl;
        }

        #endregion

        #region ===== Radio: selection changes on click only =====

        /// <summary>
        /// Every RadioGroup under <paramref name="root"/> ignores the mouse wheel (same handler as the
        /// under-6 tab), so scrolling over a radio never jumps to another option; only a click changes it.
        /// Containers also hook ControlAdded, which covers radios built later (lazy tabs, rebuilt lists).
        /// Attaching is idempotent (-= then +=).
        /// </summary>
        private void HookRadioGroupNoWheel(Control root)
        {
            try
            {
                if (root == null) return;
                RadioGroup rg = root as RadioGroup;
                if (rg != null)
                {
                    rg.MouseWheel -= RdoUnderSix_MouseWheel;
                    rg.MouseWheel += RdoUnderSix_MouseWheel;
                    return;
                }
                root.ControlAdded -= RadioNoWheel_ControlAdded;
                root.ControlAdded += RadioNoWheel_ControlAdded;
                foreach (Control c in root.Controls) HookRadioGroupNoWheel(c);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void RadioNoWheel_ControlAdded(object sender, ControlEventArgs e)
        {
            try
            {
                if (e != null) HookRadioGroupNoWheel(e.Control);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Captions on existing tabs =====

        /// <summary>
        /// D8: the existing "accompany person" name + relationship is reused as the guardian of Mau 02,
        /// so its caption is changed when the M2 tabs are on.
        /// </summary>
        private void ApplyM2GuardianCaption()
        {
            try
            {
                if (this.txtAccompanyPersonName3 == null) return;
                DevExpress.XtraLayout.LayoutControlItem lci = FindLayoutItemOf(this.txtAccompanyPersonName3);
                if (lci != null)
                {
                    lci.Text = "Họ và tên mẹ hoặc người giám hộ:";
                    lci.OptionsToolTip.ToolTip = "Họ và tên mẹ hoặc người giám hộ (đối với trẻ ≤ 16 tuổi) — Mẫu 02";
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// D9: family history is stored as portal ids in the old column PATHOLOGICAL_HISTORY_FAMILY, the
        /// free-text box would then show raw ids -> hidden while the M2 tabs are on.
        /// </summary>
        private void HideM2ReplacedControls()
        {
            try
            {
                // Family history -> portal catalog; upper/lower jaw text -> tooth chart of Mau 02.
                // Hidden only: loaded values stay in the controls and are saved back unchanged.
                foreach (Control c in new Control[] { this.txtPathologicalHistoryFamily3, this.txtExamStomatologyUpper3, this.txtExamStomatologyLower3 })
                {
                    if (c == null) continue;
                    DevExpress.XtraLayout.LayoutControlItem lci = FindLayoutItemOf(c);
                    if (lci != null) lci.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Mau 02 prints "Phân loại thể lực" and the portal M2 requires kham_thuc_the.phanloai, both read from
        /// the under-18 DHST_RANK. Its combo (already loaded/saved/enabled by the existing code) sits in the
        /// Designer's hidden items, so bring it back above "Người khám" — only when Mau 02 is built
        /// (portal declared); other hospitals keep the current layout.
        /// </summary>
        private void ShowM2PhysicalRank()
        {
            try
            {
                if (this.layoutControlItem211 == null || this.layoutControlItem445 == null || this.layoutControl22 == null) return;
                this.layoutControl22.BeginUpdate();
                try
                {
                    if (this.layoutControlItem211.IsHidden)
                    {
                        this.layoutControlItem211.RestoreFromCustomization(this.layoutControlItem445,
                            DevExpress.XtraLayout.Utils.InsertType.Top);
                        // Keep the combo short like the other inputs; the rest of the row is empty space.
                        DevExpress.XtraLayout.EmptySpaceItem fill = new DevExpress.XtraLayout.EmptySpaceItem();
                        fill.Name = "esM2PhysicalRank";
                        fill.TextSize = new Size(0, 0);
                        this.layoutControlItem211.Parent.AddItem(fill, this.layoutControlItem211,
                            DevExpress.XtraLayout.Utils.InsertType.Right);
                        this.layoutControlItem211.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                        this.layoutControlItem211.MinSize = new Size(250, 24);
                        this.layoutControlItem211.MaxSize = new Size(this.layoutControlItem201.Width, 24);
                    }
                    // Label width is shared by the whole tab (85 px) -> short caption, full name in the tooltip.
                    this.layoutControlItem211.Text = "Phân loại:";
                    this.layoutControlItem211.TextSize = this.layoutControlItem445.TextSize;
                    this.layoutControlItem211.OptionsToolTip.ToolTip = "Phân loại thể lực — in vào Mẫu 02, bắt buộc khi đẩy cổng Sở Y tế (mẫu M2)";
                    this.layoutControlItem211.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                finally { this.layoutControl22.EndUpdate(); }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion
    }
}
