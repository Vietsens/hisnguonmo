/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Dựng giao diện bằng code: GroupControl header + XtraTabControl (5 tab) + footer buttons.
 * (Có thể chuyển sang Designer/LayoutControl tinh chỉnh sau — tham khảo mockup docs/ecds-ui-*.png.)
 */
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    public partial class frmInfectiousDiseaseReport
    {
        private void BuildUi()
        {
            this.SuspendLayout();
            try
            {
                dxErr = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();

                BuildHeader();
                BuildTabs();
                BuildFooter();

                // Thứ tự dock: Fill trước, sau đó Top/Bottom
                this.Controls.Add(pnlBody);
                this.Controls.Add(grpHeader);
                this.Controls.Add(pnlFooter);
            }
            finally
            {
                this.ResumeLayout(false);
            }
        }

        #region Header
        private void BuildHeader()
        {
            grpHeader = new GroupControl();
            grpHeader.Text = "Thông tin bệnh nhân & điều trị";
            grpHeader.Dock = DockStyle.Top;
            grpHeader.Height = 108;

            var lc = new LayoutControl();
            lc.Dock = DockStyle.Fill;
            grpHeader.Controls.Add(lc);

            lblTreatmentCodeVal = NewValueLabel();
            lblPatientNameVal = NewValueLabel();
            lblDobVal = NewValueLabel();
            lblGenderVal = NewValueLabel();
            lblIcdVal = NewValueLabel();
            lblDepartmentVal = NewValueLabel();
            lblPushStatus = NewValueLabel();
            lblPushStatus.Text = "● Chưa đẩy lên cổng";

            var root = lc.Root;
            root.GroupBordersVisible = false;
            AddRow(root, "Mã điều trị:", lblTreatmentCodeVal);
            AddRow(root, "Bệnh nhân:", lblPatientNameVal);
            AddRow(root, "Ngày sinh:", lblDobVal);
            AddRow(root, "Giới tính:", lblGenderVal);
            AddRow(root, "ICD điều trị:", lblIcdVal);
            AddRow(root, "Khoa:", lblDepartmentVal);
            AddRow(root, "Trạng thái đẩy:", lblPushStatus);
        }

        private LabelControl NewValueLabel()
        {
            var l = new LabelControl();
            l.AutoSizeMode = LabelAutoSizeMode.None;
            l.Height = 20;
            return l;
        }
        #endregion

        #region Tabs
        private void BuildTabs()
        {
            pnlBody = new PanelControl();
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            tabMain = new XtraTabControl();
            tabMain.Dock = DockStyle.Fill;
            pnlBody.Controls.Add(tabMain);

            tabCaBenh = new XtraTabPage() { Text = "Ca bệnh" };
            tabHanhChinh = new XtraTabPage() { Text = "Hành chính" };
            tabTrieuChung = new XtraTabPage() { Text = "Triệu chứng & XN" };
            tabSotRet = new XtraTabPage() { Text = "Sốt rét" };
            tabNguoiBaoCao = new XtraTabPage() { Text = "Người báo cáo" };
            tabMain.TabPages.AddRange(new XtraTabPage[]
            { tabCaBenh, tabHanhChinh, tabTrieuChung, tabSotRet, tabNguoiBaoCao });

            BuildTabCaBenh();
            BuildTabHanhChinh();
            BuildTabTrieuChung();
            BuildTabSotRet();
            BuildTabNguoiBaoCao();
        }

        private LayoutControl NewTabLayout(XtraTabPage page)
        {
            var lc = new LayoutControl();
            lc.Dock = DockStyle.Fill;
            lc.Root.GroupBordersVisible = false;
            page.Controls.Add(lc);
            return lc;
        }

        private void BuildTabCaBenh()
        {
            lcCaBenh = NewTabLayout(tabCaBenh);
            cboBenh = new LookUpEdit();
            cboCapDoBenh = new LookUpEdit();
            cboLoaiChanDoan = new LookUpEdit();
            cboTinhTrang = new LookUpEdit();
            cboBenhVienChuyenToi = new LookUpEdit();
            dteNgayKhoiPhat = NewDate();
            dteNgayNhapVien = NewDate();
            dteNgayRaVien = NewDate();
            dteNgayTuVong = NewDate();
            txtTinhTrangKhac = new TextEdit();
            txtChanDoanRaVien = NewMemo();
            txtSubDiagnosis = NewMemo();
            txtComplication = NewMemo();
            txtGhiChu = NewMemo();

            var r = lcCaBenh.Root;
            AddRow(r, "Bệnh (ICD-10) (*):", cboBenh);
            AddRow(r, "Phân độ bệnh:", cboCapDoBenh);
            AddRow(r, "Phân loại chẩn đoán (*):", cboLoaiChanDoan);
            AddRow(r, "Tình trạng hiện nay (*):", cboTinhTrang);
            AddRow(r, "Ngày khởi phát:", dteNgayKhoiPhat);
            AddRow(r, "Ngày nhập viện (*):", dteNgayNhapVien);
            AddRow(r, "Ngày ra viện:", dteNgayRaVien);
            AddRow(r, "BV chuyển tới:", cboBenhVienChuyenToi);
            AddRow(r, "Ngày tử vong:", dteNgayTuVong);
            AddRow(r, "Tình trạng khác:", txtTinhTrangKhac);
            AddRow(r, "Chẩn đoán ra viện:", txtChanDoanRaVien, 44);
            AddRow(r, "Chẩn đoán phụ:", txtSubDiagnosis, 44);
            AddRow(r, "Chẩn đoán biến chứng:", txtComplication, 44);
            AddRow(r, "Ghi chú:", txtGhiChu, 44);
        }

        private void BuildTabHanhChinh()
        {
            lcHanhChinh = NewTabLayout(tabHanhChinh);
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
            txtDiaChi = new TextEdit();
            cboTinhTru = new LookUpEdit();
            cboXaTru = new LookUpEdit();
            txtDiaChiTru = new TextEdit();

            var r = lcHanhChinh.Root;
            AddRow(r, "Họ và tên (*):", txtHoTen);
            AddRow(r, "Ngày sinh (*):", dteNgaySinh);
            AddRow(r, "Tuổi (*):", spnTuoi);
            AddRow(r, "Giới tính (*):", cboGioiTinh);
            AddRow(r, "", chkMangThai);
            AddRow(r, "Số CCCD/CMND (*):", txtCccd);
            AddRow(r, "Số điện thoại (*):", txtDienThoai);
            AddRow(r, "Dân tộc (*):", cboDanToc);
            AddRow(r, "Nghề nghiệp (*):", cboNgheNghiep);
            AddRow(r, "Nơi làm việc:", txtNoiLamViec);
            AddRow(r, "Tỉnh/TP hiện nay (*):", cboTinh);
            AddRow(r, "Xã/Phường hiện nay (*):", cboXa);
            AddRow(r, "Địa chỉ chi tiết (*):", txtDiaChi);
            AddRow(r, "Tỉnh/TP thường trú:", cboTinhTru);
            AddRow(r, "Xã/Phường thường trú:", cboXaTru);
            AddRow(r, "Địa chỉ TT chi tiết:", txtDiaChiTru);
        }

        private void BuildTabTrieuChung()
        {
            lcTrieuChung = NewTabLayout(tabTrieuChung);
            chkSot = new CheckEdit() { Text = "Sốt" };
            chkRetRun = new CheckEdit() { Text = "Rét run" };
            chkVaMoHoi = new CheckEdit() { Text = "Vã mồ hôi" };
            chkKhac = new CheckEdit() { Text = "Triệu chứng khác" };
            txtMoTaKhac = new TextEdit();
            chkTuongTuGiaDinh = new CheckEdit() { Text = "Ca tương tự trong gia đình" };
            chkTuongTuNoiLamViec = new CheckEdit() { Text = "Ca tương tự nơi làm việc" };
            txtTienSuDichTe = NewMemo();
            cboSuDungVacXin = new LookUpEdit();
            spnSoLan = new SpinEdit();
            cboLayMau = new LookUpEdit();
            cboLoaiXN = new LookUpEdit();
            txtLoaiXNKhac = new TextEdit();
            cboKetQuaXN = new LookUpEdit();
            dteNgayThucHienXN = NewDate();
            dteNgayTraKQ = NewDate();
            cboDonViXN = new LookUpEdit();
            cboLoaiPhatHien = new LookUpEdit();
            lblCoSoDieuTriVal = NewValueLabel();

            var r = lcTrieuChung.Root;
            AddRow(r, "Triệu chứng:", chkSot);
            AddRow(r, "", chkRetRun);
            AddRow(r, "", chkVaMoHoi);
            AddRow(r, "", chkKhac);
            AddRow(r, "Mô tả TC khác:", txtMoTaKhac);
            AddRow(r, "Dịch tễ:", chkTuongTuGiaDinh);
            AddRow(r, "", chkTuongTuNoiLamViec);
            AddRow(r, "Tiền sử dịch tễ:", txtTienSuDichTe, 44);
            AddRow(r, "Sử dụng vắc xin:", cboSuDungVacXin);
            AddRow(r, "Số lần sử dụng:", spnSoLan);
            AddRow(r, "Lấy mẫu XN:", cboLayMau);
            AddRow(r, "Loại xét nghiệm:", cboLoaiXN);
            AddRow(r, "Loại XN khác:", txtLoaiXNKhac);
            AddRow(r, "Kết quả XN:", cboKetQuaXN);
            AddRow(r, "Ngày thực hiện XN:", dteNgayThucHienXN);
            AddRow(r, "Ngày trả kết quả:", dteNgayTraKQ);
            AddRow(r, "Đơn vị thực hiện XN:", cboDonViXN);
            AddRow(r, "Loại cơ sở điều trị:", cboLoaiPhatHien);
            AddRow(r, "Cơ sở điều trị:", lblCoSoDieuTriVal);
        }

        private void BuildTabSotRet()
        {
            lcSotRet = NewTabLayout(tabSotRet);
            cboPhuongPhapPhatHien = new LookUpEdit();
            cboLoaiSotRet = new LookUpEdit();
            cboDonViXNSotRet = new LookUpEdit();
            cboLoaiCoSoXN = new LookUpEdit();
            cboKetQuaSoiLam = new LookUpEdit();
            cboKetQuaRdt = new LookUpEdit();
            cboXnG6pd = new LookUpEdit();
            cboPhanLoaiG6pd = new LookUpEdit();
            cboDaTungMac = new LookUpEdit();
            txtMatDoKST = new TextEdit();
            txtKqG6pd = new TextEdit();
            dteNgayBatDauDieuTri = NewDate();
            chkGiaoBao = new CheckEdit() { Text = "Có giao báo" };

            var r = lcSotRet.Root;
            AddRow(r, "PP phát hiện sốt rét:", cboPhuongPhapPhatHien);
            AddRow(r, "Loại sốt rét chẩn đoán:", cboLoaiSotRet);
            AddRow(r, "Đơn vị XN sốt rét:", cboDonViXNSotRet);
            AddRow(r, "Loại cơ sở XN sốt rét:", cboLoaiCoSoXN);
            AddRow(r, "Kết quả soi lam:", cboKetQuaSoiLam);
            AddRow(r, "Kết quả RDT:", cboKetQuaRdt);
            AddRow(r, "Mật độ ký sinh trùng:", txtMatDoKST);
            AddRow(r, "XN G6PD:", cboXnG6pd);
            AddRow(r, "KQ định lượng G6PD:", txtKqG6pd);
            AddRow(r, "Phân loại G6PD:", cboPhanLoaiG6pd);
            AddRow(r, "Đã từng mắc SR:", cboDaTungMac);
            AddRow(r, "Ngày bắt đầu điều trị:", dteNgayBatDauDieuTri);
            AddRow(r, "", chkGiaoBao);
            // TODO: 2 grid con (danh sách thuốc sốt rét, lịch sử di chuyển) — bổ sung sau.
        }

        private void BuildTabNguoiBaoCao()
        {
            lcNguoiBaoCao = NewTabLayout(tabNguoiBaoCao);
            txtNguoiBaoCao = new TextEdit();
            txtDienThoaiBaoCao = new TextEdit();
            txtEmailBaoCao = new TextEdit();
            lblMaDonViVal = NewValueLabel();

            var r = lcNguoiBaoCao.Root;
            AddRow(r, "Người báo cáo (*):", txtNguoiBaoCao);
            AddRow(r, "SĐT người báo cáo (*):", txtDienThoaiBaoCao);
            AddRow(r, "Email người báo cáo (*):", txtEmailBaoCao);
            AddRow(r, "Mã đơn vị báo cáo:", lblMaDonViVal);
        }
        #endregion

        #region Footer
        private void BuildFooter()
        {
            pnlFooter = new PanelControl();
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 40;
            pnlFooter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            btnGetData = NewButton("Lấy dữ liệu từ HIS (Ctrl+L)", 8, 190);
            btnCheck = NewButton("Kiểm tra danh mục (Ctrl+K)", 204, 190);
            btnPush = NewButton("Đẩy lên cổng (Ctrl+S)", 470, 160);
            btnNew = NewButton("Mới (Ctrl+N)", 636, 110);
            btnClose = NewButton("Đóng (Esc)", 752, 100);

            btnGetData.Click += btnGetData_Click;
            btnCheck.Click += btnCheck_Click;
            btnPush.Click += btnPush_Click;
            btnNew.Click += btnNew_Click;
            btnClose.Click += btnClose_Click;

            pnlFooter.Controls.AddRange(new Control[] { btnGetData, btnCheck, btnPush, btnNew, btnClose });
        }

        private SimpleButton NewButton(string text, int x, int width)
        {
            var b = new SimpleButton();
            b.Text = text;
            b.Location = new Point(x, 7);
            b.Size = new Size(width, 26);
            b.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            return b;
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

        /// <summary>Thêm 1 dòng label-editor vào LayoutControl (label căn phải, ẩn viền group).</summary>
        private LayoutControlItem AddRow(LayoutControlGroup group, string caption, Control ctrl, int height = 22)
        {
            if (ctrl.Height < height) ctrl.Height = height;
            LayoutControlItem lci = group.AddItem();
            lci.Control = ctrl;
            lci.Text = caption;
            lci.TextLocation = DevExpress.Utils.Locations.Left;
            lci.TextSize = new Size(140, 20);
            lci.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            if (string.IsNullOrEmpty(caption))
                lci.TextVisible = false;
            return lci;
        }
        #endregion
    }
}
