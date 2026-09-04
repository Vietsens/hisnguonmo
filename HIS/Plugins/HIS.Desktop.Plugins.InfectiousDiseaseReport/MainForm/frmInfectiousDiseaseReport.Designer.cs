/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * TOÀN BỘ GIAO DIỆN (design) nằm ở đây — dựng trong InitializeComponent(), KHÔNG dựng ở runtime code-behind.
 * 2 tab = 2 object CHUẨN ĐỊNH DẠNG DỮ LIỆU QĐ 4039/2025/BYT: Đối tượng mắc bệnh + Trường hợp bệnh.
 * Mỗi tab chia theo nhóm có tiêu đề; mỗi nhóm bố cục 2 cột (Move item phải sang cạnh item trái).
 * Logic/data/event nằm ở các partial khác (__Load, __FillData, __Push, __Check, __ListPanel).
 */
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    partial class frmInfectiousDiseaseReport
    {
        private System.ComponentModel.IContainer components = null;

        #region Declare — header controls
        private GroupControl grpHeader;
        private LabelControl lblTreatmentCodeVal, lblPatientNameVal, lblDobVal, lblGenderVal, lblIcdVal, lblDepartmentVal, lblPushStatus;
        #endregion

        #region Declare — thân form (1 TRANG, 2 CỘT: trái = Đối tượng + Chẩn đoán, phải = Diễn biến/XN/Báo cáo)
        private PanelControl pnlBody, pnlFooter;
        private LayoutControl lcLeft, lcRight;
        #endregion

        #region Declare — Đối tượng mắc bệnh (DOI_TUONG_MAC_BENH)
        private TextEdit txtHoTen, txtCccd, txtDienThoai, txtNoiLamViec, txtDiaChi, txtDiaChiTru;
        private DateEdit dteNgaySinh;
        private SpinEdit spnTuoi;
        private LookUpEdit cboGioiTinh, cboDanToc, cboNgheNghiep, cboTinh, cboXa, cboThon, cboTinhTru, cboXaTru;
        private CheckEdit chkMangThai;
        #endregion

        #region Declare — Trường hợp bệnh (TRUONG_HOP_BENH) — chẩn đoán + xét nghiệm + diễn biến + người báo cáo
        private LookUpEdit cboBenh, cboCapDoBenh, cboLoaiChanDoan, cboTinhTrang, cboTinhTrangRaVien, cboBenhVienChuyenToi, cboHinhThucDieuTri;
        private DateEdit dteNgayKhoiPhat, dteNgayNhapVien, dteNgayRaVien, dteNgayTuVong;
        private MemoEdit txtChanDoanRaVien, txtSubDiagnosis, txtComplication, txtGhiChu, txtTienSuDichTe;
        private TextEdit txtTinhTrangKhac, txtLoaiXNKhac;
        private LookUpEdit cboSuDungVacXin, cboLayMau, cboLoaiXN, cboKetQuaXN, cboDonViXN;
        private SpinEdit spnSoLan;
        private DateEdit dteNgayThucHienXN, dteNgayTraKQ;
        private TextEdit txtNguoiBaoCao, txtDienThoaiBaoCao, txtEmailBaoCao;
        private LookUpEdit cboLoaiPhatHien;
        private LabelControl lblCoSoDieuTriVal, lblMaDonViVal;
        #endregion

        #region Declare — footer
        private SimpleButton btnSave, btnPush, btnNew;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErr;
        #endregion

        #region Declare — list panel (danh sách ca bệnh bên trái)
        private PanelControl pnlRight;
        private GroupControl grpList;
        private SplitterControl splitterList;
        private TextEdit txtListKeyword;
        private DateEdit dteListFrom, dteListTo;
        private SimpleButton btnListSearch;
        private GridControl grdList;
        private GridView gvList;
        #endregion

        #region Build-state (chỉ dùng khi dựng bố cục 2 cột)
        private LayoutControl secLc;                 // LayoutControl của tab đang dựng
        private LayoutControlGroup secGroup;         // nhóm hiện tại (có viền + tiêu đề)
        private LayoutControlItem secPendingLeft;    // item cột trái đang chờ ghép cột phải
        private int secIdx;                          // đếm field để luân phiên trái/phải
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            dxErr = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();

            BuildHeader();
            BuildTabs();
            BuildFooter();
            BuildListPanel();   // panel danh sách bên trái (tham khảo EnterKskInfomantionQD831)

            // Vùng bên phải: header (Top) + tabs (Fill) + footer (Bottom)
            pnlRight = new PanelControl();
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlRight.Controls.Add(pnlBody);     // Fill
            pnlRight.Controls.Add(grpHeader);   // Top
            pnlRight.Controls.Add(pnlFooter);   // Bottom

            // Thứ tự dock trên form: Fill trước, sau đó splitter + panel trái
            this.Controls.Add(pnlRight);        // Fill
            this.Controls.Add(splitterList);    // Left (giữa)
            this.Controls.Add(grpList);         // Left (ngoài cùng)

            //
            // frmInfectiousDiseaseReport
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 660);
            this.MinimumSize = new System.Drawing.Size(1000, 560);
            this.Name = "frmInfectiousDiseaseReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Text = "Báo cáo ca bệnh truyền nhiễm — Cổng giám sát quốc gia (ECDS)";
            this.Load += new System.EventHandler(this.frmInfectiousDiseaseReport_Load);
            this.ResumeLayout(false);
        }

        #region Header
        private void BuildHeader()
        {
            grpHeader = new GroupControl();
            grpHeader.Text = "Thông tin bệnh nhân & điều trị";
            grpHeader.Dock = DockStyle.Top;
            grpHeader.Height = 138;   // 3 dòng x 2 cột + dòng trạng thái đẩy (dòng gọn 22px)

            var lc = new LayoutControl();
            lc.Dock = DockStyle.Fill;
            grpHeader.Controls.Add(lc);

            lblTreatmentCodeVal = NewValueLabel(true);
            lblPatientNameVal = NewValueLabel(true);
            lblDobVal = NewValueLabel();
            lblGenderVal = NewValueLabel();
            lblIcdVal = NewValueLabel();
            lblDepartmentVal = NewValueLabel();
            lblPushStatus = NewValueLabel(true);
            lblPushStatus.Text = "● Chưa đẩy lên cổng";

            var root = lc.Root;
            root.GroupBordersVisible = false;

            // 3 dòng x 2 cột (trái | phải) bằng cách Move item phải sang cạnh item trái;
            // dòng trạng thái đẩy chiếm trọn phía dưới.
            var l1 = AddRow(root, "Mã điều trị:", lblTreatmentCodeVal);
            AddRow(root, "Giới tính:", lblGenderVal).Move(l1, InsertType.Right);
            var l2 = AddRow(root, "Bệnh nhân:", lblPatientNameVal);
            AddRow(root, "ICD điều trị:", lblIcdVal).Move(l2, InsertType.Right);
            var l3 = AddRow(root, "Ngày sinh:", lblDobVal);
            AddRow(root, "Khoa:", lblDepartmentVal).Move(l3, InsertType.Right);
            AddRow(root, "Trạng thái đẩy:", lblPushStatus);
        }

        private LabelControl NewValueLabel(bool bold = false)
        {
            var l = new LabelControl();
            l.AutoSizeMode = LabelAutoSizeMode.None;
            l.Height = 20;
            if (bold)
            {
                l.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                l.Appearance.Options.UseFont = true;
            }
            return l;
        }
        #endregion

        #region Body (1 TRANG, 2 CỘT — không cuộn)
        private void BuildTabs()
        {
            pnlBody = new PanelControl();
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            // 2 CỘT thật (TableLayoutPanel 50/50) -> toàn bộ nhóm nằm trên 1 trang, KHÔNG cuộn dọc.
            var tlp = new TableLayoutPanel();
            tlp.Dock = DockStyle.Fill;
            tlp.ColumnCount = 2;
            tlp.RowCount = 1;
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlBody.Controls.Add(tlp);

            lcLeft = new LayoutControl();
            lcLeft.Dock = DockStyle.Fill;
            lcLeft.Root.GroupBordersVisible = false;
            lcRight = new LayoutControl();
            lcRight.Dock = DockStyle.Fill;
            lcRight.Root.GroupBordersVisible = false;
            tlp.Controls.Add(lcLeft, 0, 0);
            tlp.Controls.Add(lcRight, 1, 0);

            // Cột TRÁI: Đối tượng mắc bệnh + Chẩn đoán. Cột PHẢI: Diễn biến/Xét nghiệm/Người báo cáo.
            secLc = lcLeft;
            BuildSectionsDoiTuong();
            BuildSectionsTruongHop();   // tự chuyển secLc -> lcRight sau nhóm "Chẩn đoán"
        }

        /// <summary>Mở 1 nhóm có tiêu đề (group box) trong màn; các field thêm sau sẽ vào nhóm này.</summary>
        private void BeginSection(string title)
        {
            var root = secLc.Root;
            root.GroupBordersVisible = false;
            secGroup = root.AddGroup();
            secGroup.Text = title;
            secGroup.GroupBordersVisible = true;
            secGroup.AppearanceGroup.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            secGroup.AppearanceGroup.Options.UseFont = true;
            // Khoảng đệm trong nhóm (content không dính viền) + tách nhóm phía trên.
            secGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 4, 8, 5);
            secGroup.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 4, 0, 0);
            secPendingLeft = null;
            secIdx = 0;
        }

        /// <summary>
        /// Thêm 1 field vào nhóm hiện tại theo 2 cột.
        /// Item chẵn = cột trái; item lẻ = Move sang PHẢI của item trái ngay trước (tạo hàng 2 ô).
        /// </summary>
        private void F(string caption, Control ctrl, int height = 24)
        {
            var lci = AddRow(secGroup, caption, ctrl, height);
            if (secIdx % 2 == 0)
                secPendingLeft = lci;                          // cột trái
            else if (secPendingLeft != null)
                lci.Move(secPendingLeft, InsertType.Right);    // cột phải — cạnh item trái
            secIdx++;
        }

        /// <summary>Thêm 1 field CHIẾM TRỌN chiều ngang nhóm (cho memo / trường dài).</summary>
        private void FFull(string caption, Control ctrl, int height = 24)
        {
            var lci = AddRow(secGroup, caption, ctrl, height);
            if (ctrl is MemoEdit) lci.TextLocation = DevExpress.Utils.Locations.Top;
            // Reset để field 2 cột tiếp theo bắt đầu hàng mới, không ghép nhầm với item full-width.
            secPendingLeft = null;
            secIdx = 0;
        }

        /// <summary>Nhóm — Đối tượng mắc bệnh (DOI_TUONG_MAC_BENH). Thêm vào lcMain.</summary>
        private void BuildSectionsDoiTuong()
        {
            txtHoTen = new TextEdit();
            dteNgaySinh = NewDate();
            spnTuoi = new SpinEdit();
            cboGioiTinh = new LookUpEdit();
            chkMangThai = new CheckEdit() { Text = "Đang mang thai" };
            txtCccd = new TextEdit();
            txtDienThoai = new TextEdit();
            cboDanToc = new LookUpEdit();
            cboNgheNghiep = new LookUpEdit();
            txtNoiLamViec = new TextEdit();
            cboTinh = new LookUpEdit();
            cboXa = new LookUpEdit();
            cboThon = new LookUpEdit();
            cboThon.Properties.NullText = "";   // tránh hiển thị "[EditValue is null]" khi chưa nạp danh mục
            txtDiaChi = new TextEdit();
            cboTinhTru = new LookUpEdit();
            cboXaTru = new LookUpEdit();
            txtDiaChiTru = new TextEdit();

            // Đổi xã hiện nay -> nạp lại danh sách thôn theo xã (cascade cổng: danh mục "thon").
            cboXa.EditValueChanged += cboXa_EditValueChanged;

            BeginSection("Thông tin cá nhân");
            F("Họ và tên (*):", txtHoTen);
            F("Ngày sinh (*):", dteNgaySinh);
            F("Tuổi:", spnTuoi);
            F("Giới tính (*):", cboGioiTinh);
            F("Số CCCD/CMND (*):", txtCccd);
            F("Số điện thoại (*):", txtDienThoai);
            F("Dân tộc (*):", cboDanToc);
            F("Nghề nghiệp:", cboNgheNghiep);
            F("Nơi làm việc:", txtNoiLamViec);
            F("", chkMangThai);

            BeginSection("Địa chỉ hiện nay");
            F("Tỉnh/TP:", cboTinh);
            F("Xã/Phường:", cboXa);
            F("Thôn/Ấp:", cboThon);
            F("Địa chỉ chi tiết:", txtDiaChi);

            BeginSection("Địa chỉ thường trú");
            F("Tỉnh/TP:", cboTinhTru);
            F("Xã/Phường:", cboXaTru);
            F("Địa chỉ chi tiết:", txtDiaChiTru);
        }

        /// <summary>Nhóm — Trường hợp bệnh (TRUONG_HOP_BENH): chẩn đoán + xét nghiệm + diễn biến + người báo cáo. Thêm vào lcMain.</summary>
        private void BuildSectionsTruongHop()
        {
            // Chẩn đoán
            cboBenh = new LookUpEdit();
            cboCapDoBenh = new LookUpEdit();
            cboLoaiChanDoan = new LookUpEdit();
            cboTinhTrang = new LookUpEdit();
            cboTinhTrangRaVien = new LookUpEdit();
            cboBenhVienChuyenToi = new LookUpEdit();
            cboHinhThucDieuTri = new LookUpEdit();
            dteNgayKhoiPhat = NewDate();
            dteNgayNhapVien = NewDate();
            dteNgayRaVien = NewDate();
            dteNgayTuVong = NewDate();
            txtTinhTrangKhac = new TextEdit();
            txtChanDoanRaVien = NewMemo();
            txtSubDiagnosis = NewMemo();
            txtComplication = NewMemo();
            txtTienSuDichTe = NewMemo();
            txtGhiChu = NewMemo();
            // Vắc xin & xét nghiệm
            cboSuDungVacXin = new LookUpEdit();
            spnSoLan = new SpinEdit();
            cboLayMau = new LookUpEdit();
            cboLoaiXN = new LookUpEdit();
            txtLoaiXNKhac = new TextEdit();
            cboKetQuaXN = new LookUpEdit();
            dteNgayThucHienXN = NewDate();
            dteNgayTraKQ = NewDate();
            cboDonViXN = new LookUpEdit();
            // Người báo cáo
            cboLoaiPhatHien = new LookUpEdit();
            lblCoSoDieuTriVal = NewValueLabel();
            txtNguoiBaoCao = new TextEdit();
            txtDienThoaiBaoCao = new TextEdit();
            txtEmailBaoCao = new TextEdit();
            lblMaDonViVal = NewValueLabel();

            // Đổi bệnh -> nạp lại danh sách phân độ bệnh theo ICD (cascade cổng).
            cboBenh.EditValueChanged += cboBenh_EditValueChanged;

            BeginSection("Chẩn đoán");
            FFull("Bệnh (ICD-10) (*):", cboBenh);       // bệnh lên trên cùng, chiếm trọn chiều ngang
            F("Phân độ bệnh:", cboCapDoBenh);
            F("Phân loại chẩn đoán (*):", cboLoaiChanDoan);
            FFull("Chẩn đoán ra viện:", txtChanDoanRaVien, 40);
            FFull("Chẩn đoán phụ:", txtSubDiagnosis, 40);
            FFull("Chẩn đoán biến chứng:", txtComplication, 40);

            // ---- Sang CỘT PHẢI ----
            secLc = lcRight;

            BeginSection("Diễn biến & Ra viện");
            F("Ngày khởi phát:", dteNgayKhoiPhat);
            F("Ngày nhập viện (*):", dteNgayNhapVien);
            F("Tình trạng hiện nay (*):", cboTinhTrang);
            F("Hình thức điều trị (*):", cboHinhThucDieuTri);
            F("Tình trạng khác:", txtTinhTrangKhac);
            F("Ngày ra viện:", dteNgayRaVien);
            F("Tình trạng ra viện:", cboTinhTrangRaVien);
            F("Ngày tử vong:", dteNgayTuVong);
            F("BV chuyển tới:", cboBenhVienChuyenToi);

            BeginSection("Vắc xin & Xét nghiệm");
            F("Sử dụng vắc xin:", cboSuDungVacXin);
            F("Số lần sử dụng:", spnSoLan);
            F("Lấy mẫu XN:", cboLayMau);
            F("Loại xét nghiệm:", cboLoaiXN);
            F("Loại XN khác:", txtLoaiXNKhac);
            F("Kết quả XN:", cboKetQuaXN);
            F("Ngày thực hiện XN:", dteNgayThucHienXN);
            F("Ngày trả kết quả:", dteNgayTraKQ);
            F("Đơn vị thực hiện XN:", cboDonViXN);

            BeginSection("Người báo cáo");
            F("Loại cơ sở điều trị (*):", cboLoaiPhatHien);
            F("Cơ sở điều trị:", lblCoSoDieuTriVal);
            F("Người báo cáo (*):", txtNguoiBaoCao);
            F("Mã đơn vị báo cáo:", lblMaDonViVal);
            F("SĐT người báo cáo (*):", txtDienThoaiBaoCao);
            F("Email người báo cáo (*):", txtEmailBaoCao);
            FFull("Tiền sử dịch tễ:", txtTienSuDichTe, 40);
            FFull("Ghi chú:", txtGhiChu, 40);
        }
        #endregion

        #region Footer
        private void BuildFooter()
        {
            pnlFooter = new PanelControl();
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 42;
            pnlFooter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            btnSave = NewButton("Lưu (Ctrl+S)", 8, 120);
            btnPush = NewButton("Đẩy lên cổng", 134, 120);
            btnNew = NewButton("Mới (Ctrl+N)", 260, 100);

            btnSave.Click += btnSave_Click;
            btnPush.Click += btnPush_Click;
            btnNew.Click += btnNew_Click;

            pnlFooter.Controls.AddRange(new Control[] { btnSave, btnPush, btnNew });
        }

        private SimpleButton NewButton(string text, int x, int width)
        {
            var b = new SimpleButton();
            b.Text = text;
            b.Location = new Point(x, 8);
            b.Size = new Size(width, 26);
            b.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            return b;
        }
        #endregion

        #region List panel (danh sách ca bệnh bên trái) — dựng UI (data ở __ListPanel.cs)
        private void BuildListPanel()
        {
            grpList = new GroupControl();
            grpList.Text = "Danh sách ca bệnh";
            grpList.Dock = DockStyle.Left;
            grpList.Width = 360;

            // --- Vùng tìm kiếm: LayoutControl tự sắp xếp (KHÔNG dùng toạ độ tuyệt đối) ---
            txtListKeyword = new TextEdit();
            // Ô từ khóa dạng hint (như EnterKskInfomantionQD831): không nhãn, hiển thị gợi ý bên trong.
            txtListKeyword.Properties.NullValuePrompt = "Nội dung tìm kiếm (mã ĐT / mã BN / tên BN)";
            txtListKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            txtListKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            dteListFrom = NewListDate();
            dteListTo = NewListDate();
            btnListSearch = new SimpleButton() { Text = "Tìm" };
            btnListSearch.Height = 26;

            txtListKeyword.KeyDown += txtListKeyword_KeyDown;
            btnListSearch.Click += (s, e) => { try { LoadListSync(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); } };

            var lcSearch = new LayoutControl();
            lcSearch.Dock = DockStyle.Top;
            lcSearch.Height = 112;
            var root = lcSearch.Root;
            root.GroupBordersVisible = false;

            AddListRow(root, "", txtListKeyword);   // không nhãn — dùng NullValuePrompt hint
            var liFrom = AddListRow(root, "Từ ngày:", dteListFrom);
            AddListRow(root, "Đến ngày:", dteListTo).Move(liFrom, InsertType.Right);
            var liBtn = AddListRow(root, "", btnListSearch, 26);
            liBtn.TextVisible = false;
            liBtn.SizeConstraintsType = SizeConstraintsType.Custom;
            liBtn.MinSize = new Size(90, 28);
            liBtn.MaxSize = new Size(90, 28);
            root.Add(new EmptySpaceItem());   // hút phần trống còn lại -> nút Tìm không bị kéo giãn

            // --- Grid ---
            grdList = new GridControl() { Dock = DockStyle.Fill };
            gvList = new GridView(grdList);
            grdList.MainView = gvList;
            gvList.OptionsBehavior.Editable = false;
            gvList.OptionsView.ShowGroupPanel = false;
            gvList.OptionsView.ColumnAutoWidth = false;
            gvList.OptionsView.ShowIndicator = false;
            GridColumn cStt = gvList.Columns.AddVisible("STT"); cStt.Caption = "STT"; cStt.Width = 36;
            GridColumn cCode = gvList.Columns.AddVisible("TREATMENT_CODE"); cCode.Caption = "Mã ĐT"; cCode.Width = 100;
            GridColumn cName = gvList.Columns.AddVisible("PATIENT_NAME"); cName.Caption = "Bệnh nhân"; cName.Width = 140;
            GridColumn cIcd = gvList.Columns.AddVisible("ICD_CODE"); cIcd.Caption = "ICD"; cIcd.Width = 60;
            gvList.Click += gvList_Click;

            grpList.Controls.Add(grdList);       // Fill (thêm trước -> nằm dưới)
            grpList.Controls.Add(lcSearch);      // Top

            splitterList = new SplitterControl();
            splitterList.Dock = DockStyle.Left;
        }

        /// <summary>Thêm 1 dòng label-editor vào LayoutControl panel tìm kiếm (label căn phải, rộng cố định).</summary>
        private LayoutControlItem AddListRow(LayoutControlGroup group, string caption, Control ctrl, int height = 24)
        {
            if (ctrl.Height < height) ctrl.Height = height;
            var lci = group.AddItem();
            lci.Control = ctrl;
            lci.Text = caption;
            lci.TextLocation = DevExpress.Utils.Locations.Left;
            lci.TextSize = new Size(88, 20);
            lci.TextAlignMode = TextAlignModeItem.CustomSize;
            lci.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            if (string.IsNullOrEmpty(caption)) lci.TextVisible = false;
            lci.MinSize = new Size(120, ctrl.Height);
            return lci;
        }

        private DateEdit NewListDate()
        {
            var d = new DateEdit();
            d.Properties.Mask.EditMask = "dd/MM/yyyy";
            d.Properties.Mask.UseMaskAsDisplayFormat = true;
            return d;
        }
        #endregion

        #region Small factories & AddRow
        private DateEdit NewDate()
        {
            var d = new DateEdit();
            d.Properties.Mask.EditMask = "dd/MM/yyyy";
            d.Properties.Mask.UseMaskAsDisplayFormat = true;
            d.EditValue = null;
            return d;
        }

        private MemoEdit NewMemo()
        {
            var m = new MemoEdit();
            m.Height = 40;
            return m;
        }

        /// <summary>
        /// Thêm 1 dòng label-editor vào LayoutControlGroup.
        /// Label căn phải, rộng cố định để các cột thẳng hàng; trường bắt buộc "(*)" tô màu maroon.
        /// </summary>
        private LayoutControlItem AddRow(LayoutControlGroup group, string caption, Control ctrl, int height = 22)
        {
            if (ctrl.Height < height) ctrl.Height = height;
            LayoutControlItem lci = group.AddItem();
            lci.Control = ctrl;
            lci.Text = caption;
            lci.TextLocation = DevExpress.Utils.Locations.Left;
            // Cột nhãn 120px -> gọn, mọi editor bắt đầu cùng 1 mốc (thẳng hàng).
            lci.TextSize = new Size(120, 20);
            lci.TextAlignMode = TextAlignModeItem.CustomSize;
            lci.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            lci.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            lci.AppearanceItemCaption.Options.UseTextOptions = true;

            if (string.IsNullOrEmpty(caption))
                lci.TextVisible = false;

            // Trường bắt buộc: nhãn màu maroon (chuẩn ui_rules).
            if (!string.IsNullOrEmpty(caption) && caption.Contains("(*)"))
            {
                lci.AppearanceItemCaption.ForeColor = Color.Maroon;
                lci.AppearanceItemCaption.Options.UseForeColor = true;
            }

            // Khoảng đệm quanh mỗi dòng (gọn) -> nhịp dọc đều, các ô không dính sát nhau.
            lci.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 1, 4, 2);
            // MinSize nhỏ -> 2 field vừa 1 hàng trong CỘT nửa form (2*200 < ~500px ở 1366), editor không quá rộng.
            lci.MinSize = new Size(200, ctrl.Height);
            return lci;
        }
        #endregion
    }
}
