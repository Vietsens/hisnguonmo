/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Bổ sung cụm "Đề nghị" ở tab Kết luận của KSK người từ 18 tuổi trở lên:
 *      Đề nghị:      [combo chọn 1 — danh mục của cổng Sở Y tế TP.HCM]
 *      Đề nghị khác: [ô chữ]   <- chỉ mở khi combo chọn "Khác"
 *
 * Danh mục `KetLuan_DeNghi` của cổng có 5 mục, trong đó có mục "Khác" — chọn mục đó thì phải
 * ghi rõ nội dung, giống cách ô "Nguồn khác, ghi rõ" hoạt động ở phần Nguồn chi trả.
 *
 * NHẬN BIẾT MỤC "Khác" THEO TÊN, không ghi cứng mã: mã do Sở tự sinh, Sở đổi mã là hỏng.
 *
 * Hai ô này DỰNG BẰNG MÃ và chèn vào LayoutControl có sẵn của tab Kết luận, KHÔNG sửa file
 * Designer (file đó hơn 28.000 dòng, sửa tay rất dễ hỏng bố cục).
 *
 * AN TOÀN ĐA VIỆN: chỉ dựng khi viện đã khai báo cấu hình cổng Sở Y tế TP.HCM.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Khai báo =====

        private GridLookUpEdit cboKskSuggest;      // Đề nghị (chọn 1)
        private TextEdit txtKskSuggestOther;       // Đề nghị khác

        private bool suggestControlsInited = false;

        /// <summary>Mã danh mục "Kết luận – đề nghị" của cổng.</summary>
        private const string SYT_CODE__KET_LUAN_DE_NGHI = "KetLuan_DeNghi";

        /// <summary>
        /// Từ khóa nhận biết mục "Khác" trong danh mục đề nghị, dùng để mở ô ghi rõ.
        /// Nhận theo TÊN vì danh mục của cổng không có mã chữ.
        /// </summary>
        private static readonly string[] SYT_SUGGEST_OTHER_KEYWORDS = new string[] { "khác", "khac" };

        /// <summary>Mã đề nghị (của cổng) được coi là "Khác" — dựng lúc đổ danh mục.</summary>
        private static readonly List<long> sytSuggestOtherIds = new List<long>();

        private const int KSK_SUGGEST_LABEL_W = 115;
        private const int KSK_SUGGEST_ROW_H = 24;

        #endregion

        #region ===== Dựng giao diện =====

        /// <summary>Dựng 2 ô và chèn xuống dưới ô "Các vấn đề sức khỏe" của tab Kết luận.</summary>
        private void InitConclusionSuggestControls()
        {
            try
            {
                if (suggestControlsInited) return;
                if (this.layoutControl21 == null || this.layoutControlItem106 == null) return;

                // AN TOÀN ĐA VIỆN: chỉ viện đã khai báo cổng Sở Y tế TP.HCM mới có 2 ô này.
                if (!IsSytHcmDeclared())
                {
                    LogSystem.Debug("SytHcm: chua khai bao cau hinh cong -> KHONG dung cum De nghi");
                    return;
                }
                suggestControlsInited = true;

                cboKskSuggest = new GridLookUpEdit();
                cboKskSuggest.Name = "cboKskSuggest";
                cboKskSuggest.MenuManager = this.barManager1;
                cboKskSuggest.Properties.NullText = "";

                txtKskSuggestOther = new TextEdit();
                txtKskSuggestOther.Name = "txtKskSuggestOther";
                txtKskSuggestOther.Properties.MaxLength = 500;   // bằng bề rộng cột lưu
                txtKskSuggestOther.Enabled = false;              // mặc định khóa

                this.layoutControl21.BeginUpdate();
                try
                {
                    LayoutControlItem lciSuggest =
                        (LayoutControlItem)this.layoutControl21.AddItem("Đề nghị:", cboKskSuggest);
                    SetupSuggestItem(lciSuggest, "Đề nghị:");
                    lciSuggest.Move(this.layoutControlItem106, InsertType.Bottom);

                    LayoutControlItem lciOther =
                        (LayoutControlItem)this.layoutControl21.AddItem("Đề nghị khác:", txtKskSuggestOther);
                    SetupSuggestItem(lciOther, "Đề nghị khác:");
                    lciOther.Move(lciSuggest, InsertType.Bottom);
                }
                finally { this.layoutControl21.EndUpdate(); }

                InitKskSuggestCombo();

                cboKskSuggest.EditValueChanged -= cboKskSuggest_EditValueChanged;
                cboKskSuggest.EditValueChanged += cboKskSuggest_EditValueChanged;
                UpdateKskSuggestOtherEnable();

                // Danh mục của cổng có thể đã tải xong TRƯỚC khi 2 ô này kịp tạo -> đổ lại cho chắc.
                ApplySytCatalogToControls();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static void SetupSuggestItem(LayoutControlItem lci, string caption)
        {
            lci.AppearanceItemCaption.Options.UseTextOptions = true;
            lci.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            lci.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            lci.Text = caption;
            lci.TextAlignMode = TextAlignModeItem.CustomSize;
            lci.TextSize = new Size(KSK_SUGGEST_LABEL_W, 20);
            lci.TextToControlDistance = 5;
            lci.SizeConstraintsType = SizeConstraintsType.Custom;
            lci.MinSize = new Size(300, KSK_SUGGEST_ROW_H);
            lci.MaxSize = new Size(0, KSK_SUGGEST_ROW_H);
        }

        /// <summary>
        /// Danh sách đề nghị. TẠM THỜI theo mẫu phiếu; bật cổng thì danh mục thật của cổng
        /// ghi đè vào đúng chỗ này (xem ApplySytToSuggestCombo).
        /// </summary>
        private static List<KskCodeNameADO> BuildKskSuggestList()
        {
            return new List<KskCodeNameADO>
            {
                new KskCodeNameADO(1, "Bình thường, hẹn khám định kỳ lần sau"),
                new KskCodeNameADO(2, "Có yếu tố nguy cơ, cần theo dõi thêm"),
                new KskCodeNameADO(3, "Đã có bệnh mạn tính, tiếp tục điều trị theo phác đồ/toa cũ"),
                new KskCodeNameADO(4, "Chuyển tuyến, khám chuyên khoa"),
                new KskCodeNameADO(5, "Khác")
            };
        }

        private void InitKskSuggestCombo()
        {
            try
            {
                cboKskSuggest.Properties.DataSource = BuildKskSuggestList();
                cboKskSuggest.Properties.DisplayMember = "NAME";
                cboKskSuggest.Properties.ValueMember = "ID";
                cboKskSuggest.Properties.NullText = "";

                DevExpress.XtraGrid.Columns.GridColumn colId =
                    cboKskSuggest.Properties.View.Columns.AddField("ID");
                colId.VisibleIndex = 1; colId.Width = 55; colId.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn colName =
                    cboKskSuggest.Properties.View.Columns.AddField("NAME");
                colName.VisibleIndex = 2; colName.Width = 460; colName.Caption = "Nội dung đề nghị";
                cboKskSuggest.Properties.PopupFormWidth = 530;
                cboKskSuggest.Properties.View.OptionsView.ShowColumnHeaders = true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Khóa / mở ô "Đề nghị khác" =====

        private void cboKskSuggest_EditValueChanged(object sender, EventArgs e)
        {
            try { UpdateKskSuggestOtherEnable(); }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Chọn "Khác" -> mở ô ghi rõ; chọn mục khác -> khóa VÀ xóa trắng.
        /// Xóa trắng để không giữ lại nội dung của lựa chọn trước, gây sai dữ liệu đẩy.
        /// </summary>
        private void UpdateKskSuggestOtherEnable()
        {
            try
            {
                if (txtKskSuggestOther == null) return;
                bool isOther = IsKskSuggestOther();
                txtKskSuggestOther.Enabled = isOther;
                if (!isOther) txtKskSuggestOther.Text = string.Empty;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private bool IsKskSuggestOther()
        {
            try
            {
                if (cboKskSuggest == null || cboKskSuggest.EditValue == null) return false;
                long id;
                if (!long.TryParse(cboKskSuggest.EditValue.ToString(), out id)) return false;

                // Đã đổ danh mục của cổng -> so theo mã của Sở.
                if (sytSuggestOtherIds != null && sytSuggestOtherIds.Count > 0)
                    return sytSuggestOtherIds.Contains(id);

                // Chưa bật cổng -> danh sách tạm, mục "Khác" là mục cuối (mã 5).
                return id == 5;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        #endregion

        #region ===== Lấy / đặt giá trị =====

        /// <summary>Mã đề nghị đang chọn (null nếu chưa chọn) — dùng khi lưu.</summary>
        private long? GetKskSuggestValue()
        {
            try
            {
                if (cboKskSuggest == null || cboKskSuggest.EditValue == null) return null;
                long v;
                return long.TryParse(cboKskSuggest.EditValue.ToString(), out v) ? (long?)v : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>Nội dung "Đề nghị khác" (null nếu ô đang khóa hoặc để trống).</summary>
        private string GetKskSuggestOtherValue()
        {
            try
            {
                if (txtKskSuggestOther == null || !txtKskSuggestOther.Enabled) return null;
                string v = (txtKskSuggestOther.Text ?? "").Trim();
                return v.Length > 0 ? v : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Đổ giá trị đã lưu. Thứ tự BẮT BUỘC: đặt combo TRƯỚC, cập nhật khóa/mở, rồi mới đặt
        /// nội dung ô ghi rõ — làm ngược thì bước khóa sẽ xóa mất nội dung vừa đặt.
        /// </summary>
        private void SetKskSuggestValue(long? suggestId, string other)
        {
            try
            {
                if (cboKskSuggest == null) return;
                cboKskSuggest.EditValue = suggestId.HasValue ? (object)suggestId.Value : null;
                UpdateKskSuggestOtherEnable();
                if (txtKskSuggestOther != null && txtKskSuggestOther.Enabled)
                    txtKskSuggestOther.Text = other;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Kiểm tra trước khi lưu: chọn "Khác" thì bắt buộc ghi rõ.
        /// Trả false kèm thông báo để nơi gọi dừng việc lưu.
        /// </summary>
        private bool ValidKskSuggestOther()
        {
            try
            {
                if (!IsKskSuggestOther()) return true;
                if (!string.IsNullOrWhiteSpace(txtKskSuggestOther.Text)) return true;

                XtraMessageBox.Show(
                    "Đề nghị là \"Khác\" nên phải ghi rõ nội dung đề nghị.",
                    "Thông báo",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);
                txtKskSuggestOther.Focus();
                return false;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return true; }
        }

        #endregion

        // TODO(DB): lưu/nạp 2 ô này cần THÊM 2 CỘT vào bảng HIS_KSK_SYT_HCM:
        //     SYT_SUGGEST_ID    NUMBER(19,0)        -- mã đề nghị trong danh mục của cổng
        //     SUGGEST_OTHER     VARCHAR2(500 BYTE)  -- nội dung khi chọn "Khác"
        // Phần lưu/nạp đã viết sẵn trong frmEnterKskInfomantionVer2___SytHcmData.cs và tự hoạt động
        // ngay khi 2 cột có thật; chưa có cột thì nhật ký ghi "KHONG co cot" chứ không làm hỏng gì.
        // Khi đẩy cổng: chỉ tiêu `KetLuan_DeNghi` lấy tên mục đã chọn, hoặc nội dung ô ghi rõ nếu
        // chọn "Khác".
    }
}
