/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Đổ danh mục tải từ Nền tảng KSK Sở Y tế TP.HCM vào các ô nhập của màn hình KSK.
 *
 * CHỈ CHẠY KHI VIỆN ĐÃ KHAI BÁO KHÓA CẤU HÌNH CỔNG SYT TP.HCM.
 * Chưa khai báo -> không đụng gì, mọi ô nhập giữ nguyên danh mục của HIS như trước.
 * Đây là điều kiện an toàn đa viện của toàn bộ tệp này.
 *
 * Các ô được thay nguồn dữ liệu:
 *   Đối tượng khám              <- M3_DoiTuongKham
 *   Nguồn chi trả               <- ChiTra
 *   Địa điểm khám               <- Diadiemkham
 *   Tình trạng răng             <- NenTangKSK_TinhTrangRang
 *   Chẩn đoán (ICD-10)          <- ICD
 *   2 ô tích hợp đồng / phi địa giới: không có danh mục, nhưng ĐIỀU KIỆN MỞ phải đổi theo
 *                                     vì mã nguồn chi trả giờ là mã của Sở, không còn 1..6 của HIS.
 *
 * LƯU Ý: đây là NGOẠI LỆ TẠM THỜI của nguyên tắc GP3.1 (HIS giữ danh mục HIS, chỉ quy đổi khi
 * đẩy). Theo yêu cầu, các ô trên lấy thẳng danh mục của Sở ngay lúc nhập. Hệ quả: dữ liệu lưu
 * trong HIS sẽ là mã của Sở, nên khi Sở đổi mã thì hồ sơ cũ không còn tra ngược được.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Mã danh mục dùng cho từng ô =====

        private const string SYT_CODE__DOI_TUONG_KHAM = "M3_DoiTuongKham";
        private const string SYT_CODE__NGUON_CHI_TRA = "ChiTra";
        private const string SYT_CODE__DIA_DIEM_KHAM = "Diadiemkham";

        /// <summary>
        /// Hình thức chi trả chi tiết — 2 mục: Khám theo hợp đồng / Tự thực hiện.
        /// Đặc tả M3 gọi danh mục này là `ChiTraChiTiet`, M4 gọi `ChiTraChiTiet_NCT`. Màn hình này
        /// là giấy khám người từ 18 tuổi (mẫu M3) nên ưu tiên tên của M3, không có thì lấy tên M4.
        /// </summary>
        private const string SYT_CODE__HINH_THUC_CHI_TRA_CT = "ChiTraChiTiet";
        private const string SYT_CODE__HINH_THUC_CHI_TRA_CT_M4 = "ChiTraChiTiet_NCT";
        private const string SYT_CODE__TINH_TRANG_RANG = "NenTangKSK_TinhTrangRang";
        private const string SYT_CODE__ICD = "ICD";

        /// <summary>
        /// Từ khóa nhận biết mục "nguồn khác" trong danh mục nguồn chi trả của Sở, dùng để
        /// mở ô "Nguồn khác, ghi rõ". Danh mục của Sở chỉ trả Id + Tên, không có mã chữ,
        /// nên phải nhận theo TÊN; khi cổng bổ sung mã chữ thì đổi sang so mã cho chắc chắn.
        /// </summary>
        private static readonly string[] SYT_OTHER_PAY_SOURCE_KEYWORDS = new string[] { "khác", "khac" };

        /// <summary>Mã nguồn chi trả (của Sở) được coi là "nguồn khác" — dựng lúc đổ danh mục.</summary>
        private static readonly List<int> sytOtherPaySourceIds = new List<int>();

        /// <summary>
        /// Từ khóa nhận biết mục "ngân sách nhà nước hỗ trợ" trong danh mục nguồn chi trả của Sở,
        /// dùng để mở ô chọn "Hình thức chi trả". Danh mục của Sở đặt tên là **Ngân sách thành phố
        /// hỗ trợ** nên nhận theo từ "ngân sách"; danh mục của HIS có Ngân sách Trung ương và
        /// Ngân sách Địa phương cũng khớp cùng từ này.
        /// </summary>
        private static readonly string[] SYT_STATE_BUDGET_KEYWORDS = new string[] { "ngân sách", "ngan sach" };

        /// <summary>Mã nguồn chi trả (của Sở) được coi là ngân sách nhà nước — dựng lúc đổ danh mục.</summary>
        private static readonly List<int> sytStateBudgetPaySourceIds = new List<int>();

        /// <summary>Đã đổ danh mục của Sở vào các ô hay chưa — tránh đổ lại nhiều lần.</summary>
        private bool sytCatalogApplied = false;

        /// <summary>
        /// Ô "Đối tượng khám" đang giữ MÃ CỦA SỞ, không phải mã của HIS.
        /// Tách riêng từng ô: một ô đổ được, ô kia chưa thì chỉ để trống đúng một cột
        /// tương ứng, không để trống cả hai làm mất dữ liệu người dùng vừa chọn.
        /// </summary>
        private bool sytObjectComboUseSytCode = false;

        /// <summary>Ô "Nguồn chi trả" đang giữ MÃ CỦA SỞ, không phải mã của HIS.</summary>
        private bool sytPaySourceComboUseSytCode = false;

        /// <summary>
        /// Ô chọn "Hình thức chi trả" đang giữ MÃ CỦA SỞ. Cột lưu chỉ nhận mã của cổng nên
        /// chưa đổ được danh mục thì KHÔNG ghi giá trị tạm (1/2) vào cột đó.
        /// </summary>
        private bool sytPaySourceDetailUseSytCode = false;

        private bool IsSytCodeInPaySourceDetail() { return sytPaySourceDetailUseSytCode; }

        /// <summary>
        /// Bảng trạng thái răng đang dùng danh mục của Sở. 32 cột răng chứa mã định danh của
        /// cổng nên chưa đổ được danh mục thì KHÔNG ghi mã tạm vào.
        /// </summary>
        private bool sytToothUseSytCode = false;

        private bool IsSytCodeInObjectCombo() { return sytObjectComboUseSytCode; }

        private bool IsSytCodeInPaySourceCombo() { return sytPaySourceComboUseSytCode; }

        #endregion

        #region ===== Đổ danh mục vào các ô nhập =====

        /// <summary>
        /// Gọi sau khi tải xong danh mục. Tự quay về luồng giao diện nếu đang ở luồng nền.
        /// </summary>
        private void ApplySytCatalogSafe()
        {
            try
            {
                // Vua tai lai danh muc -> cho phep do lai vao o nhap.
                // KHONG xoa 2 co "dang giu ma cua So": o nhap van dang giu ma cua So cho den
                // khi do lai xong, xoa som se ho ke cho luot luu roi vao giua.
                sytCatalogApplied = false;
                if (this.IsDisposed || !this.IsHandleCreated) return;
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(ApplySytCatalogToControls));
                    return;
                }
                ApplySytCatalogToControls();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ danh mục của Sở vào các ô nhập. CHỈ chạy khi đã khai báo khóa cấu hình.</summary>
        private void ApplySytCatalogToControls()
        {
            try
            {
                if (sytCatalogApplied) return;
                // Điều kiện an toàn đa viện: chưa khai báo cổng -> giữ nguyên danh mục HIS.
                if (GetSytConnectionInfo() == null)
                {
                    LogSystem.Warn("SytCatalog: chua khai bao cau hinh cong -> KHONG do danh muc, "
                        + "cac o nhap giu nguyen danh muc cua HIS");
                    return;
                }

                bool okObject = ApplySytToObjectCombo();
                bool okPay = ApplySytToPaymentSourceCombo();
                bool okPayDetail = ApplySytToPaySourceDetail();
                bool okSuggest = ApplySytToSuggestCombo();
                bool okPlace = ApplySytToExamPlaceCombo();
                bool okTooth = ApplySytToToothStatus();
                bool okIcd = ApplySytToIcd();
                bool any = okObject || okPay || okPayDetail || okSuggest || okPlace || okTooth || okIcd;

                LogSystem.Warn(string.Format(
                    "SytCatalog: do danh muc vao o nhap — Doi tuong={0}, Nguon chi tra={1}, "
                    + "Hinh thuc chi tra={2}, De nghi={3}, Dia diem kham={4}, Tinh trang rang={5}, ICD={6}",
                    okObject, okPay, okPayDetail, okSuggest, okPlace, okTooth, okIcd));

                if (okObject) sytObjectComboUseSytCode = true;
                if (okPay) sytPaySourceComboUseSytCode = true;
                if (okPayDetail) sytPaySourceDetailUseSytCode = true;
                if (okTooth) sytToothUseSytCode = true;

                if (any)
                {
                    UpdatePaySourceExtraEnable();   // mã nguồn chi trả đã đổi -> tính lại khóa/mở 2 ô tích

                    // Hồ sơ đã mở TRƯỚC khi danh mục về -> nạp lại giá trị, vì lúc đó mã của Sở không
                    // khớp mục nào nên ô nhập không sáng đúng.
                    //
                    // ĐẶT Ở ĐÂY, KHÔNG ĐẶT TRONG NHÁNH "cả 7 ô đều xong": chỉ cần một ô chưa dựng
                    // xong (ICD hoặc Tình trạng răng ở tab chưa mở là chuyện thường) thì nhánh đó
                    // không chạy, và ô Hình thức chi trả sẽ trống mãi. Nạp lại nhiều lượt vô hại vì
                    // chỉ là gán lại đúng giá trị đã đọc từ cơ sở dữ liệu.
                    //
                    // Chỉ nạp lại khi hồ sơ CÓ bản ghi đã lưu: hồ sơ mới nhập dở mà gọi vào đây sẽ
                    // xóa trắng những gì người dùng vừa gõ.
                    if (sytAdminFilledBeforeCatalog && currentKskSytHcm != null)
                    {
                        LogSystem.Warn("SytCatalog: ho so mo truoc khi danh muc ve -> nap lai gia tri");
                        FillKskSytHcmAdminControls();
                    }

                    // Ô chọn bệnh của tab Khám lâm sàng HCM có thể đã dựng bằng danh mục ICD của HIS.
                    if (okIcd) RefreshHcmIcdFromSytCatalog();

                    // Cụm tiền sử gia đình: đổi ô chữ thành danh sách ô tích, và ô mã ICD thành ô
                    // chọn bệnh của cổng. Gọi ở đây vì lúc này danh mục CHẮC CHẮN đã có.
                    BuildSytFamilyHistory();
                    if (okIcd)
                    {
                        BuildSytFamilyIcd();
                        // Ô vừa dựng là ô TRỐNG -> đổ lại mã bệnh đã lưu của hồ sơ. Không có bước này
                        // thì hồ sơ cũ mở ra luôn trắng phần mã ICD tiền sử gia đình.
                        if (currentKskGeneral != null)
                            FillSytFamilyIcd(currentKskGeneral.FAMILY_HISTORY_ICD_CODE,
                                currentKskGeneral.FAMILY_HISTORY_ICD_NAME);
                    }
                }

                // CHI coi la xong khi TAT CA o nhap da nhan duoc danh muc. Neu con o nao truot
                // (thuong do o do chua kip tao xong) thi de ngo de luot goi sau do bu.
                if (okObject && okPay && okPayDetail && okSuggest && okPlace && okTooth && okIcd)
                {
                    sytCatalogApplied = true;
                    LogSystem.Debug("SytCatalog: da do danh muc cua cong vao TAT CA o nhap");
                }
                else if (any)
                {
                    LogSystem.Warn("SytCatalog: con o nhap CHUA nhan duoc danh muc "
                        + "(thuong do o do chua kip tao xong) -> se do bu o luot sau");
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổi danh mục của một ô chọn dạng lưới (mã + tên).</summary>
        private static bool SetCodeNameSource(GridLookUpEdit cbo, List<KskCodeNameADO> data)
        {
            try
            {
                if (cbo == null || data == null || data.Count == 0) return false;
                object keep = cbo.EditValue;
                cbo.Properties.DataSource = data;
                // Giá trị đang chọn không còn trong danh mục mới -> xóa trắng, tránh hiển thị sai.
                if (keep != null && !data.Any(o => o.ID.ToString() == keep.ToString()))
                {
                    cbo.EditValue = null;
                }
                return true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>Chuyển danh mục của Sở sang dạng mã + tên. Bỏ mục không có mã số.</summary>
        private static List<KskCodeNameADO> ToCodeNameList(string catalogCode)
        {
            List<KskCodeNameADO> rs = new List<KskCodeNameADO>();
            try
            {
                foreach (var it in GetSytCatalog(catalogCode))
                {
                    if (it == null) continue;
                    int id;
                    if (!int.TryParse((it.Id ?? "").Trim(), out id)) continue;
                    rs.Add(new KskCodeNameADO(id, it.Name));
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return rs;
        }

        private bool ApplySytToObjectCombo()
        {
            var data = ToCodeNameList(SYT_CODE__DOI_TUONG_KHAM);
            if (data.Count == 0) return false;
            // Combo tích nhiều: bỏ hết tick cũ vì mã của Sở khác mã của HIS.
            try
            {
                var gridCheck = cboObject.Properties.Tag as HIS.Desktop.Utilities.Extensions.GridCheckMarksSelection;
                if (gridCheck != null) gridCheck.ClearSelection(cboObject.Properties.View);
                objectSelecteds = new List<KskCodeNameADO>();
                cboObject.Text = string.Empty;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return SetCodeNameSource(cboObject, data);
        }

        private bool ApplySytToPaymentSourceCombo()
        {
            var data = ToCodeNameList(SYT_CODE__NGUON_CHI_TRA);
            if (data.Count == 0) return false;

            // Dựng lại danh sách mã "nguồn khác", để biết khi nào mở ô ghi rõ.
            sytOtherPaySourceIds.Clear();
            sytStateBudgetPaySourceIds.Clear();
            foreach (var it in data)
            {
                string name = (it.NAME ?? "").ToLowerInvariant();
                foreach (string kw in SYT_OTHER_PAY_SOURCE_KEYWORDS)
                {
                    if (name.Contains(kw)) { sytOtherPaySourceIds.Add(it.ID); break; }
                }
                foreach (string kw in SYT_STATE_BUDGET_KEYWORDS)
                {
                    if (name.Contains(kw)) { sytStateBudgetPaySourceIds.Add(it.ID); break; }
                }
            }
            if (sytOtherPaySourceIds.Count == 0)
            {
                LogSystem.Warn("SytCatalog: khong nhan ra muc 'nguon khac' trong danh muc nguon chi tra "
                             + "-> o 'Nguon khac, ghi ro' se luon bi khoa");
            }
            if (sytStateBudgetPaySourceIds.Count == 0)
            {
                LogSystem.Warn("SytCatalog: khong nhan ra muc 'ngan sach nha nuoc ho tro' trong danh muc "
                             + "nguon chi tra -> o chon 'Hinh thuc chi tra' se luon bi khoa");
            }
            return SetCodeNameSource(cboPaymentSource, data);
        }

        /// <summary>
        /// Đổ các mục Hình thức chi trả chi tiết vào ô chọn — CHỈ theo danh mục của mẫu M3.
        ///
        /// Trước đây danh mục M3 rỗng thì lùi về danh mục của mẫu M4. Bỏ nhánh đó: hai danh mục
        /// KHÁC SỐ MỤC nhau, nên lùi về M4 sẽ thiếu mục và giá trị đã lưu của mục thiếu không chọn
        /// lại được, mà người dùng không hiểu vì sao. Thà không đổ được thì để trống và ghi cảnh báo.
        /// </summary>
        private bool ApplySytToPaySourceDetail()
        {
            var data = ToCodeNameList(SYT_CODE__HINH_THUC_CHI_TRA_CT);
            if (data.Count == 0)
            {
                LogSystem.Warn("SytCatalog: danh muc " + SYT_CODE__HINH_THUC_CHI_TRA_CT
                    + " (mau M3) chua tai duoc -> o Hinh thuc chi tra giu danh sach tam");
                return false;
            }
            return SetKskPaySourceDetailSource(data);
        }

        /// <summary>
        /// Đổ danh mục "Kết luận – đề nghị" của cổng vào ô chọn, và dựng lại danh sách mã được coi
        /// là "Khác" để biết khi nào mở ô ghi rõ.
        /// </summary>
        private bool ApplySytToSuggestCombo()
        {
            var data = ToCodeNameList(SYT_CODE__KET_LUAN_DE_NGHI);
            if (data.Count == 0) return false;

            sytSuggestOtherIds.Clear();
            foreach (var it in data)
            {
                string name = (it.NAME ?? "").ToLowerInvariant();
                foreach (string kw in SYT_SUGGEST_OTHER_KEYWORDS)
                {
                    if (name.Contains(kw)) { sytSuggestOtherIds.Add(it.ID); break; }
                }
            }
            if (sytSuggestOtherIds.Count == 0)
                LogSystem.Warn("SytCatalog: khong nhan ra muc 'Khac' trong danh muc de nghi "
                             + "-> o 'De nghi khac' se luon bi khoa");

            return SetCodeNameSource(cboKskSuggest, data);
        }

        private bool ApplySytToExamPlaceCombo()
        {
            var data = ToCodeNameList(SYT_CODE__DIA_DIEM_KHAM);
            if (data.Count == 0) return false;
            return SetCodeNameSource(cboKskExamPlace, data);
        }

        /// <summary>Thay bảng mã trạng thái răng bằng danh mục của Sở, giữ nguyên bảng màu theo thứ tự.</summary>
        private bool ApplySytToToothStatus()
        {
            try
            {
                var items = GetSytCatalog(SYT_CODE__TINH_TRANG_RANG);
                if (items == null || items.Count == 0) return false;

                List<HcmToothStatus> rs = new List<HcmToothStatus>();
                int i = 0;
                foreach (var it in items)
                {
                    int code;
                    if (!int.TryParse((it.Id ?? "").Trim(), out code)) continue;
                    Color back, fore;
                    PickToothColor(i, out back, out fore);
                    rs.Add(new HcmToothStatus
                    {
                        Code = code,
                        Name = it.Name,
                        ShortName = ShortenToothName(it.Name),
                        BackColor = back,
                        ForeColor = fore
                    });
                    i++;
                }
                if (rs.Count == 0) return false;
                SetHcmToothStatusSource(rs);
                return true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>Bảng màu xoay vòng cho trạng thái răng lấy từ cổng (cổng không trả màu).</summary>
        private static void PickToothColor(int index, out Color back, out Color fore)
        {
            Color[] backs =
            {
                Color.FromArgb(0xE3, 0xF6, 0xE5), Color.FromArgb(0xFF, 0xEB, 0xEE), Color.FromArgb(0xFF, 0xF3, 0xE0),
                Color.FromArgb(0xE3, 0xF2, 0xFD), Color.FromArgb(0xFF, 0xCD, 0xD2), Color.FromArgb(0xEC, 0xEF, 0xF1),
                Color.FromArgb(0xF3, 0xE5, 0xF5), Color.FromArgb(0xFF, 0xF9, 0xC4), Color.FromArgb(0xF5, 0xF5, 0xF5),
                Color.FromArgb(0xFA, 0xFA, 0xFA)
            };
            Color[] fores =
            {
                Color.FromArgb(0x2E, 0x7D, 0x32), Color.FromArgb(0xC6, 0x28, 0x28), Color.FromArgb(0xE6, 0x51, 0x00),
                Color.FromArgb(0x15, 0x65, 0xC0), Color.FromArgb(0xB7, 0x1C, 0x1C), Color.FromArgb(0x45, 0x5A, 0x64),
                Color.FromArgb(0x6A, 0x1B, 0x9A), Color.FromArgb(0x9E, 0x7D, 0x0A), Color.FromArgb(0x9E, 0x9E, 0x9E),
                Color.FromArgb(0x75, 0x75, 0x75)
            };
            back = backs[index % backs.Length];
            fore = fores[index % fores.Length];
        }

        /// <summary>Rút gọn tên trạng thái cho vừa nút răng (~15 ký tự).</summary>
        private static string ShortenToothName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            name = name.Trim();
            return name.Length <= 15 ? name : name.Substring(0, 14) + "…";
        }

        /// <summary>
        /// Đổi danh mục cho các ô chọn chẩn đoán ICD-10 của tab Khám lâm sàng HCM.
        /// Danh mục của Sở chỉ có mã định danh và tên nên dựng bản ghi bệnh tạm để ô chọn dùng được.
        /// </summary>
        private bool ApplySytToIcd()
        {
            try
            {
                var items = GetSytCatalog(SYT_CODE__ICD);
                if (items == null || items.Count == 0) return false;

                List<HIS_ICD> rs = new List<HIS_ICD>();
                List<SytIcdItem> full = new List<SytIcdItem>();
                foreach (var it in items)
                {
                    if (it == null) continue;

                    // Danh mục ICD của cổng KHÔNG có trường mã — mã bệnh nằm ở ĐẦU TÊN, dạng
                    // "V81.7 -- Hành khách đi tàu...". Lấy theo trường mã rồi lùi về mã định danh
                    // thì ô chọn bệnh hiện ra "1", "2", "3" chứ không phải mã bệnh.
                    string code, name;
                    SplitSytIcdName(it.Code, it.Name, out code, out name);
                    if (string.IsNullOrEmpty(code)) continue;
                    rs.Add(new HIS_ICD { ICD_CODE = code, ICD_NAME = name });

                    // Bản GIỮ Id cho ô chọn bệnh của tab HCM. Dùng TÊN NGUYÊN VĂN của cổng
                    // ("V81.7 -- Hành khách đi tàu...") — không tách mã ra khỏi tên, vì cột lưu là Id
                    // nên mã bệnh không còn dùng để làm gì, tách ra chỉ thêm chỗ sai lệch.
                    long icdId;
                    if (long.TryParse((it.Id ?? "").Trim(), out icdId))
                        full.Add(new SytIcdItem { ID = icdId, TEN_BENH = (it.Name ?? "").Trim() });
                }
                if (rs.Count == 0) return false;

                sytIcdSource = rs;
                sytIcdItems = full.OrderBy(o => o.TEN_BENH).ToList();
                LogSystem.Debug("SytCatalog: danh muc ICD cua cong co " + rs.Count + " muc");
                return true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>
        /// Tách một mục ICD của cổng thành MÃ BỆNH và TÊN BỆNH.
        ///
        /// Cổng để trống trường mã ở toàn bộ 11.368 mục; mã nằm ở đầu tên, ngăn bởi "--".
        /// Một số mã còn kèm dấu thập tự — "A06.5† -- Áp xe phổi do amíp" — đánh dấu cặp bệnh
        /// nguyên nhân / biểu hiện. Bỏ dấu đó để mã khớp với mã bệnh của HIS, và phía đẩy cũng bỏ
        /// y như vậy nên hai bên vẫn tra ra nhau.
        /// </summary>
        private static void SplitSytIcdName(string rawCode, string rawName, out string code, out string name)
        {
            code = "";
            name = "";
            try
            {
                string n = (rawName ?? "").Trim();
                if (!string.IsNullOrWhiteSpace(rawCode))
                {
                    code = CleanSytIcdCode(rawCode);
                    name = n;
                    return;
                }
                int i = n.IndexOf("--", StringComparison.Ordinal);
                if (i > 0)
                {
                    code = CleanSytIcdCode(n.Substring(0, i));
                    name = n.Substring(i + 2).Trim();
                }
                else
                {
                    // Không có dấu ngăn -> giữ nguyên cả dòng làm tên, không đoán mã.
                    name = n;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Bỏ dấu thập tự / hoa thị và khoảng trắng khỏi mã bệnh.</summary>
        private static string CleanSytIcdCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "";
            return code.Replace("†", "").Replace("*", "").Trim();
        }

        /// <summary>
        /// Danh mục ICD lấy từ cổng. Khác null thì tab Khám lâm sàng HCM dùng danh mục này
        /// thay cho danh mục ICD của HIS.
        /// </summary>
        private static List<HIS_ICD> sytIcdSource = null;

        #endregion
    }
}
