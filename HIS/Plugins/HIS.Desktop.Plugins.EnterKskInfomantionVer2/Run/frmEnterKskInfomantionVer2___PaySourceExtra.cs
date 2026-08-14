/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Bổ sung 3 ô nhập ngay DƯỚI "Nguồn chi trả" ở tab KSK người từ 18 tuổi trở lên:
 *      Nguồn khác, ghi rõ: [ô chữ]          <- chỉ mở khi Nguồn chi trả là "Nguồn khác"
 *      Hình thức chi trả:  (o) Khám theo hợp đồng  (o) Khám phi địa giới  (o) Tự thực hiện
 *                          LUÔN chọn được, không phụ thuộc Nguồn chi trả
 *      Địa điểm khám:      [combo chọn 1]
 *
 * VÌ SAO "Hình thức chi trả" là ô chọn một chứ không phải 2 ô tích:
 * cổng có sẵn danh mục "hình thức chi trả chi tiết" gồm ĐÚNG 2 mục loại trừ nhau —
 * Khám theo hợp đồng và Tự thực hiện. Trước đây thiết kế 2 ô tích riêng nên cùng một
 * thông tin nằm hai chỗ và có thể chọi nhau (tích "theo hợp đồng" nhưng chi tiết lại là
 * "tự thực hiện"). Nay dùng một ô chọn, lưu đúng một cột mã của cổng.
 *
 * Combo Địa điểm khám CHỌN 1 (một giấy khám chỉ có một địa điểm), dựng y hệt combo
 * "Nguồn chi trả".
 *
 * Ba ô này được DỰNG BẰNG MÃ và chèn vào LayoutControl có sẵn, KHÔNG sửa file Designer
 * (file Designer của màn hình này hơn 14.000 dòng, sửa tay rất dễ hỏng bố cục).
 *
 * GIAI ĐOẠN NÀY: CHỈ DỰNG GIAO DIỆN, chưa nạp/lưu dữ liệu (xem TODO(BE) ở cuối file).
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Khai báo =====

        private TextEdit txtKskPaySourceOther;      // Nguồn khác, ghi rõ
        private RadioGroup rdoKskPaySourceDetail;   // Hình thức chi trả chi tiết (chọn 1)
        private GridLookUpEdit cboKskExamPlace;     // Địa điểm khám (chọn 1)

        private bool paySourceExtraInited = false;

        /// <summary>
        /// Mã Nguồn chi trả của HIS được coi là "nguồn khác" -> bắt buộc ghi rõ.
        /// Dùng khi viện CHƯA bật cổng Sở Y tế; bật cổng rồi thì so theo mã của cổng
        /// (danh sách <see cref="sytOtherPaySourceIds"/> dựng lúc đổ danh mục).
        /// </summary>
        private static readonly short[] KSK_PAY_SOURCE_OTHER = new short[] { 9 };

        /// <summary>
        /// Mã Nguồn chi trả của HIS được coi là "ngân sách nhà nước hỗ trợ" -> mở ô chọn
        /// "Hình thức chi trả". Danh mục HIS không có mục tên đúng như vậy nên quy ước gồm
        /// 1 = Ngân sách Trung ương và 2 = Ngân sách Địa phương. Bật cổng Sở Y tế thì so theo
        /// mã của cổng (danh sách <see cref="sytStateBudgetPaySourceIds"/>).
        /// </summary>
        private static readonly short[] KSK_PAY_SOURCE_STATE_BUDGET = new short[] { 1, 2 };

        // Bề rộng nhãn lấy y hệt ô "Nguồn chi trả" để các dòng thẳng cột với nhau.
        private const int KSK_EXTRA_LABEL_W = 115;
        private const int KSK_EXTRA_LABEL_H = 20;

        private const int KSK_EXTRA_ROW_H = 24;


        #endregion

        #region ===== Dựng giao diện =====

        /// <summary>
        /// Dựng và chèn 3 ô nhập vào LayoutControl chứa "Nguồn chi trả", ngay dưới ô đó.
        /// Gọi sau khi các combo hành chính đã khởi tạo.
        /// </summary>
        private void InitPaySourceExtraControls()
        {
            try
            {
                if (paySourceExtraInited) return;
                if (this.layoutControl5 == null || this.layoutControlItem659 == null) return;

                // AN TOÀN ĐA VIỆN: 3 ô này là chỉ tiêu riêng của cổng Sở Y tế TP.HCM, viện khác
                // không dùng nên không dựng — bố cục màn hình giữ nguyên y như trước.
                if (!IsSytHcmDeclared())
                {
                    Inventec.Common.Logging.LogSystem.Debug(
                        "SytHcm: chua khai bao cau hinh cong -> KHONG dung 3 o nhap duoi Nguon chi tra");
                    return;
                }

                paySourceExtraInited = true;

                txtKskPaySourceOther = new TextEdit();
                txtKskPaySourceOther.Name = "txtKskPaySourceOther";
                txtKskPaySourceOther.Properties.MaxLength = 2000;   // bằng bề rộng cột lưu
                txtKskPaySourceOther.Enabled = false;               // mặc định khóa

                rdoKskPaySourceDetail = new RadioGroup();
                rdoKskPaySourceDetail.Name = "rdoKskPaySourceDetail";
                rdoKskPaySourceDetail.Properties.Columns = 2;       // 2 mục nằm ngang
                rdoKskPaySourceDetail.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Near;
                rdoKskPaySourceDetail.Properties.Appearance.BackColor = Color.Transparent;
                rdoKskPaySourceDetail.Properties.BorderStyle = BorderStyles.NoBorder;

                cboKskExamPlace = new GridLookUpEdit();
                cboKskExamPlace.Name = "cboKskExamPlace";
                cboKskExamPlace.MenuManager = this.barManager1;
                cboKskExamPlace.Properties.NullText = "";

                this.layoutControl5.BeginUpdate();
                try
                {
                    // Xếp lần lượt từ trên xuống, mỗi dòng gắn ngay dưới dòng vừa đặt.
                    // Không tách hàng ngang nữa vì ô chọn "Hình thức chi trả" tự chứa 2 mục.

                    // Dòng 1: Nguồn khác, ghi rõ — nằm ngay dưới Nguồn chi trả vì thuộc về ô đó.
                    LayoutControlItem lciOther = (LayoutControlItem)this.layoutControl5.AddItem(
                        "Nguồn khác, ghi rõ:", txtKskPaySourceOther);
                    SetupKskExtraItem(lciOther, "Nguồn khác, ghi rõ:");
                    lciOther.Move(this.layoutControlItem659, InsertType.Bottom);

                    // Dòng 2: Hình thức chi trả (chọn 1).
                    LayoutControlItem lciDetail = (LayoutControlItem)this.layoutControl5.AddItem(
                        "Hình thức chi trả:", rdoKskPaySourceDetail);
                    SetupKskExtraItem(lciDetail, "Hình thức chi trả:");
                    // KHÔNG ghim bề rộng: danh mục của cổng có 3 mục, ghim rồi chia cho số cột
                    // thì mỗi mục quá hẹp và các mục ĐÈ LÊN NHAU. Để ô chọn trải hết bề ngang
                    // của dòng, số cột tính theo số mục thực tế lúc đổ danh mục.
                    // Bề rộng để 0 = không giới hạn, chỉ ghim chiều cao cho chữ không bị cắt.
                    lciDetail.SizeConstraintsType = SizeConstraintsType.Custom;
                    lciDetail.MinSize = new Size(0, 26);
                    lciDetail.MaxSize = new Size(0, 26);
                    lciDetail.Move(lciOther, InsertType.Bottom);

                    // Dòng 3: Địa điểm khám.
                    LayoutControlItem lciPlace = (LayoutControlItem)this.layoutControl5.AddItem(
                        "Địa điểm khám:", cboKskExamPlace);
                    SetupKskExtraItem(lciPlace, "Địa điểm khám:");
                    lciPlace.Move(lciDetail, InsertType.Bottom);
                }
                finally { this.layoutControl5.EndUpdate(); }

                InitKskPaySourceDetail();
                InitKskExamPlaceCombo();

                // Trạng thái khóa/mở của ô "Nguồn khác, ghi rõ" bám theo Nguồn chi trả.
                cboPaymentSource.EditValueChanged -= cboPaymentSource_ExtraEnableChanged;
                cboPaymentSource.EditValueChanged += cboPaymentSource_ExtraEnableChanged;
                UpdatePaySourceExtraEnable();

                // Danh muc cua So co the da tai xong TRUOC khi cac o nay kip tao
                // (doc tu tep chi het hon mot giay). Do lai o day de khong bi truot.
                ApplySytCatalogToControls();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Ghim bề rộng một ô nhập để DevExpress không kéo giãn chiếm hết dòng.</summary>
        private static void SetKskExtraFixedWidth(LayoutControlItem lci, int width)
        {
            lci.SizeConstraintsType = SizeConstraintsType.Custom;
            lci.MinSize = new Size(width, KSK_EXTRA_ROW_H);
            lci.MaxSize = new Size(width, KSK_EXTRA_ROW_H);
            lci.Size = new Size(width, KSK_EXTRA_ROW_H);
        }

        /// <summary>Canh nhãn y hệt ô "Nguồn chi trả" để các dòng thẳng cột.</summary>
        private static void SetupKskExtraItem(LayoutControlItem lci, string caption)
        {
            lci.AppearanceItemCaption.Options.UseTextOptions = true;
            lci.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            lci.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            lci.Text = caption;
            lci.TextAlignMode = TextAlignModeItem.CustomSize;
            lci.TextSize = new Size(KSK_EXTRA_LABEL_W, KSK_EXTRA_LABEL_H);
            lci.TextToControlDistance = 5;
        }

        #endregion

        #region ===== Ô "Nguồn khác, ghi rõ" =====

        private void cboPaymentSource_ExtraEnableChanged(object sender, EventArgs e)
        {
            try { UpdatePaySourceExtraEnable(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Cập nhật trạng thái khóa/mở của hai ô bám theo Nguồn chi trả:
        ///   - "Nguồn khác, ghi rõ": mở khi nguồn chi trả là **nguồn khác**, ngược lại khóa và xóa trắng.
        ///   - "Hình thức chi trả": mở khi nguồn chi trả là **ngân sách nhà nước hỗ trợ**, ngược lại
        ///     khóa và bỏ chọn.
        /// Đều xóa giá trị khi khóa, để không giữ lại lựa chọn của nguồn chi trả trước đó — nếu giữ thì
        /// ô đang khóa vẫn có giá trị và vẫn được lưu, đẩy lên cổng thành dữ liệu sai.
        /// </summary>
        private void UpdatePaySourceExtraEnable()
        {
            try
            {
                if (txtKskPaySourceOther != null)
                {
                    bool isOther = IsKskPaySourceOther();
                    txtKskPaySourceOther.Enabled = isOther;
                    if (!isOther) txtKskPaySourceOther.Text = string.Empty;
                }

                // Ô "Hình thức chi trả" LUÔN chọn được — không phụ thuộc Nguồn chi trả
                // (xác nhận của người yêu cầu). Chỉ ô "Nguồn khác, ghi rõ" mới khóa/mở.
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Nguồn chi trả đang chọn có phải ngân sách nhà nước hỗ trợ.</summary>
        private bool IsKskPaySourceStateBudget()
        {
            try
            {
                if (cboPaymentSource == null || cboPaymentSource.EditValue == null) return false;

                // Da do danh muc cua cong -> so theo ma cua So.
                if (sytStateBudgetPaySourceIds != null && sytStateBudgetPaySourceIds.Count > 0)
                {
                    int id;
                    if (!int.TryParse(cboPaymentSource.EditValue.ToString(), out id)) return false;
                    return sytStateBudgetPaySourceIds.Contains(id);
                }

                // Chua bat cong SYT -> so theo ma cua HIS.
                short v;
                if (!short.TryParse(cboPaymentSource.EditValue.ToString(), out v)) return false;
                return KSK_PAY_SOURCE_STATE_BUDGET.Contains(v);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return false; }
        }

        private bool IsKskPaySourceOther()
        {
            try
            {
                if (cboPaymentSource == null || cboPaymentSource.EditValue == null) return false;

                // Da do danh muc cua cong -> so theo ma cua So (mang duoc dung khi do danh muc).
                if (sytOtherPaySourceIds != null && sytOtherPaySourceIds.Count > 0)
                {
                    int id;
                    if (!int.TryParse(cboPaymentSource.EditValue.ToString(), out id)) return false;
                    return sytOtherPaySourceIds.Contains(id);
                }

                // Chua bat cong SYT -> so theo ma cua HIS nhu cu.
                short v;
                if (!short.TryParse(cboPaymentSource.EditValue.ToString(), out v)) return false;
                return KSK_PAY_SOURCE_OTHER.Contains(v);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return false; }
        }

        /// <summary>Nội dung "Nguồn khác, ghi rõ" (null nếu ô đang khóa hoặc để trống).</summary>
        private string GetKskPaySourceOtherValue()
        {
            try
            {
                if (txtKskPaySourceOther == null || !txtKskPaySourceOther.Enabled) return null;
                string v = (txtKskPaySourceOther.Text ?? "").Trim();
                return v.Length > 0 ? v : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Kiểm tra trước khi lưu: chọn "nguồn khác" thì bắt buộc ghi rõ.
        /// Trả về false kèm thông báo để nơi gọi dừng việc lưu.
        /// </summary>
        private bool ValidKskPaySourceOther()
        {
            try
            {
                if (!IsKskPaySourceOther()) return true;
                if (!string.IsNullOrWhiteSpace(txtKskPaySourceOther.Text)) return true;

                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Nguồn chi trả là \"Nguồn khác\" nên phải ghi rõ nguồn chi trả.",
                    "Thông báo",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);
                txtKskPaySourceOther.Focus();
                return false;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return true; }
        }

        #endregion

        #region ===== Ô chọn "Hình thức chi trả" (chọn 1) =====

        /// <summary>
        /// Danh sách hình thức chi trả chi tiết. TẠM THỜI theo mẫu phiếu với mã 1/2, chờ
        /// danh mục của cổng — bật cổng thì mã thật của cổng ghi đè vào đúng chỗ này.
        /// </summary>
        private static List<KskCodeNameADO> BuildKskPaySourceDetailList()
        {
            return new List<KskCodeNameADO>
            {
                new KskCodeNameADO(1, "Khám theo hợp đồng"),
                new KskCodeNameADO(2, "Khám phi địa giới"),
                new KskCodeNameADO(3, "Tự thực hiện")
            };
        }

        private void InitKskPaySourceDetail()
        {
            try
            {
                SetKskPaySourceDetailSource(BuildKskPaySourceDetailList());
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Đổ danh sách mục vào ô chọn, giữ lại mục đang chọn nếu vẫn còn trong danh sách.</summary>
        private bool SetKskPaySourceDetailSource(List<KskCodeNameADO> data)
        {
            try
            {
                if (rdoKskPaySourceDetail == null || data == null || data.Count == 0) return false;

                object keep = rdoKskPaySourceDetail.EditValue;
                rdoKskPaySourceDetail.Properties.Items.Clear();
                // Số cột = số mục -> tất cả nằm trên MỘT dòng và tự chia đều bề ngang.
                // Để cố định 2 cột thì danh mục 3 mục sẽ xuống dòng hoặc chồng chữ.
                rdoKskPaySourceDetail.Properties.Columns = data.Count;
                foreach (var it in data)
                {
                    rdoKskPaySourceDetail.Properties.Items.Add(new RadioGroupItem(it.ID, it.NAME));
                }

                // Mục đang chọn không còn trong danh sách mới -> bỏ chọn, tránh hiển thị sai.
                bool stillThere = keep != null && data.Any(o => o.ID.ToString() == keep.ToString());
                rdoKskPaySourceDetail.EditValue = stillThere ? keep : null;
                if (!stillThere) rdoKskPaySourceDetail.SelectedIndex = -1;
                return true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return false; }
        }

        /// <summary>Mã hình thức chi trả đang chọn (null nếu chưa chọn) — dùng khi lưu.</summary>
        private long? GetKskPaySourceDetailValue()
        {
            try
            {
                if (rdoKskPaySourceDetail == null || rdoKskPaySourceDetail.EditValue == null) return null;
                long v;
                return long.TryParse(rdoKskPaySourceDetail.EditValue.ToString(), out v) ? (long?)v : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Đổ mã hình thức chi trả đã lưu vào ô chọn.</summary>
        private void SetKskPaySourceDetailValue(long? code)
        {
            try
            {
                if (rdoKskPaySourceDetail == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: o Hinh thuc chi tra CHUA duoc dung"
                        + " -> khong nap duoc gia tri " + (code.HasValue ? code.Value.ToString() : "rong"));
                    return;
                }
                // CHAN DOAN: ghi ro gia tri doc tu CSDL va o nhap dang co nhung muc nao. Lech nhau
                // tuc la danh muc cua So chua ve kip luc nap ho so.
                if (code.HasValue)
                {
                    var have = new List<string>();
                    foreach (RadioGroupItem it in rdoKskPaySourceDetail.Properties.Items)
                        if (it != null && it.Value != null) have.Add(it.Value.ToString());
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: nap Hinh thuc chi tra tu CSDL = "
                        + code.Value + " ; o nhap dang co cac muc [" + string.Join(",", have.ToArray())
                        + "] -> " + (have.Contains(code.Value.ToString()) ? "KHOP" : "*** KHONG KHOP ***"));
                }
                if (!code.HasValue)
                {
                    rdoKskPaySourceDetail.EditValue = null;
                    rdoKskPaySourceDetail.SelectedIndex = -1;
                    return;
                }
                // Gán ĐÚNG KIỂU của mục trong danh sách, không gán thẳng long.
                // Danh mục của cổng dựng mục bằng int; DevExpress so sánh EditValue với Value của mục
                // theo kiểu đối tượng nên long 5084 KHÔNG khớp int 5084 -> ô không sáng mục nào.
                foreach (RadioGroupItem it in rdoKskPaySourceDetail.Properties.Items)
                {
                    if (it == null || it.Value == null) continue;
                    if (it.Value.ToString() != code.Value.ToString()) continue;
                    rdoKskPaySourceDetail.EditValue = it.Value;
                    return;
                }

                // Không có mục nào khớp: danh mục của cổng chưa về, hoặc Sở đã bỏ mục này.
                Inventec.Common.Logging.LogSystem.Warn("SytHcm: Hinh thuc chi tra " + code.Value
                    + " khong co trong danh sach muc cua o -> de trong");
                rdoKskPaySourceDetail.EditValue = null;
                rdoKskPaySourceDetail.SelectedIndex = -1;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Combo Địa điểm khám (chọn 1) =====

        /// <summary>
        /// Danh sách Địa điểm khám. TẠM THỜI theo mẫu phiếu, chờ danh mục của cổng —
        /// bật cổng thì danh mục thật ghi đè vào đúng chỗ này.
        /// </summary>
        private static List<KskCodeNameADO> BuildKskExamPlaceList()
        {
            return new List<KskCodeNameADO>
            {
                new KskCodeNameADO(1, "Tại cơ sở y tế"),
                new KskCodeNameADO(2, "Tại trạm y tế xã, phường"),
                new KskCodeNameADO(3, "Tại nhà"),
                new KskCodeNameADO(4, "Khám lưu động")
            };
        }

        /// <summary>
        /// Combo Địa điểm khám: CHỌN 1, hiển thị cột Mã + Tên — dựng y hệt combo "Nguồn chi trả".
        /// Một giấy khám sức khỏe chỉ diễn ra tại một địa điểm nên không cho tích nhiều.
        /// </summary>
        private void InitKskExamPlaceCombo()
        {
            try
            {
                cboKskExamPlace.Properties.DataSource = BuildKskExamPlaceList();
                cboKskExamPlace.Properties.DisplayMember = "NAME";
                cboKskExamPlace.Properties.ValueMember = "ID";
                cboKskExamPlace.Properties.NullText = "";

                DevExpress.XtraGrid.Columns.GridColumn colId = cboKskExamPlace.Properties.View.Columns.AddField("ID");
                colId.VisibleIndex = 1; colId.Width = 45; colId.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn colName = cboKskExamPlace.Properties.View.Columns.AddField("NAME");
                colName.VisibleIndex = 2; colName.Width = 360; colName.Caption = "Tên";
                cboKskExamPlace.Properties.PopupFormWidth = 430;
                cboKskExamPlace.Properties.View.OptionsView.ShowColumnHeaders = true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Mã Địa điểm khám đang chọn (null nếu chưa chọn) — dùng khi lưu.
        /// Kiểu số dài vì đây là mã định danh của cổng, không phải mã của HIS.
        /// </summary>
        private long? GetKskExamPlaceValue()
        {
            try
            {
                if (cboKskExamPlace == null || cboKskExamPlace.EditValue == null) return null;
                long v;
                return long.TryParse(cboKskExamPlace.EditValue.ToString(), out v) ? (long?)v : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Đổ mã Địa điểm khám đã lưu vào combo.</summary>
        private void SetKskExamPlaceValue(long? code)
        {
            try
            {
                if (!code.HasValue) { cboKskExamPlace.EditValue = null; return; }

                // Cùng lý do như ô Hình thức chi trả: nguồn của ô có ID kiểu int, gán long thì ô
                // không tra ra dòng nào và hiện trống.
                var src = cboKskExamPlace.Properties.DataSource as System.Collections.Generic.List<KskCodeNameADO>;
                if (src != null)
                {
                    foreach (KskCodeNameADO it in src)
                    {
                        if (it == null) continue;
                        if (it.ID.ToString() != code.Value.ToString()) continue;
                        cboKskExamPlace.EditValue = it.ID;
                        return;
                    }
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: Dia diem kham " + code.Value
                        + " khong co trong danh muc cua o -> de trong");
                    cboKskExamPlace.EditValue = null;
                    return;
                }
                cboKskExamPlace.EditValue = code.Value;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #endregion

        // Nạp/lưu 3 ô này: xem frmEnterKskInfomantionVer2___SytHcmData.cs.
        // Ghi chú giữ lại vì có ràng buộc thứ tự khi nạp:
        //   - Nguồn khác, ghi rõ  -> GetKskPaySourceOtherValue() vào cột PAY_SOURCE_OTHER.
        //     Trước khi lưu phải gọi ValidKskPaySourceOther() — chọn "nguồn khác" thì bắt buộc ghi rõ.
        //   - Hình thức chi trả   -> GetKskPaySourceDetailValue() / SetKskPaySourceDetailValue(...)
        //     vào cột SYT_PAY_SOURCE_DETAIL_ID. CHỈ ghi khi ô chọn đang dùng danh mục của cổng
        //     (IsSytCodeInPaySourceDetail()), vì cột này chứa mã định danh của cổng.
        //   - Địa điểm khám       -> GetKskExamPlaceValue() / SetKskExamPlaceValue(...) vào cột
        //     SYT_EXAM_PLACE_ID như thiết kế (PTTK mục 2.2.b)
        //   - Khi nạp lại hồ sơ: gọi UpdatePaySourceExtraEnable() SAU khi gán Nguồn chi trả, rồi mới gán
        //     nội dung ô "Nguồn khác, ghi rõ" và ô chọn "Hình thức chi trả" — vì khóa sẽ tự xóa giá trị.
    }
}
