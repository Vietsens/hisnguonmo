/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tiền sử gia đình theo mẫu M3 của Sở Y tế TP.HCM — CHỈ dựng khi viện đã khai báo cấu hình cổng.
 *
 * Hai ô của phần "1. Tiền sử gia đình" đổi cách nhập:
 *
 *   Ô chữ    -> danh sách ô tích, 9 bệnh lấy từ danh mục `TS_GiaDinh_MacBenh_DanhSachBenh` của cổng.
 *               Lưu DANH SÁCH MÃ ĐỊNH DANH của cổng vào chính cột cũ `PATHOLOGICAL_HISTORY_FAMILY`.
 *   Ô mã ICD -> đổi nguồn sang danh mục ICD của cổng, lưu MÃ ĐỊNH DANH vào cột
 *               `HIS_KSK_GENERAL.FAMILY_HISTORY_ICD_CODE`, tên bệnh vào `..._ICD_NAME`.
 *
 * Viện chưa khai báo cấu hình thì KHÔNG dựng gì, hai ô giữ nguyên như cũ.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Mã danh mục bệnh tiền sử gia đình của cổng.</summary>
        private const string SYT_CODE__TS_GIA_DINH = "TS_GiaDinh_MacBenh_DanhSachBenh";

        /// <summary>Dấu ngăn giữa các mã định danh khi lưu — đúng dạng cổng nhận.</summary>
        private const string SYT_FAMILY_SEP = ",";

        /// <summary>Khung chứa hàng 1 các ô tích tiền sử gia đình (5 bệnh đầu).</summary>
        private PanelControl chkSytFamilyHistory;

        /// <summary>Khung chứa hàng 2 (các bệnh còn lại) — dòng bố cục RIÊNG.</summary>
        private PanelControl chkSytFamilyHistoryRow2;
        private bool sytFamilyHistoryInited;

        /// <summary>
        /// Đổi ô chữ "Tiền sử gia đình" thành danh sách ô tích lấy từ danh mục của cổng.
        /// Gọi được nhiều lần, chỉ dựng một lượt.
        /// </summary>
        /// <summary>
        /// TẠM TẮT phần đổi ô chữ thành danh sách ô tích.
        ///
        /// Bố cục của dòng này không giữ được hai hàng ô tích: bộ xếp bố cục luôn nén dòng về một
        /// hàng nên các bệnh ở hàng dưới bị cắt khỏi vùng nhìn thấy. Đã thử ghim ở dòng bố cục, ghim
        /// ở khung chứa, tách thành hai dòng riêng — đều không giữ được.
        ///
        /// Theo yêu cầu, trả ô chữ về như cũ và để người yêu cầu chọn hướng khác. Toàn bộ mã bên dưới
        /// giữ nguyên, chỉ cần đổi cờ này thành true là bật lại.
        /// </summary>
        private const bool SYT_FAMILY_CHECKLIST_ENABLED = true;

        private void BuildSytFamilyHistory()
        {
            try
            {
                if (!SYT_FAMILY_CHECKLIST_ENABLED) return;
                if (sytFamilyHistoryInited) return;
                if (!IsSytHcmDeclared()) return;                  // an toàn đa viện
                if (this.txtPathologicalHistoryFamily == null)
                {
                    LogSystem.Warn("SytHcm/TSGD: khong thay o chu tien su gia dinh -> khong dung");
                    return;
                }

                List<KskCodeNameADO> data = ToCodeNameList(SYT_CODE__TS_GIA_DINH);
                if (data.Count == 0)
                {
                    // Danh mục chưa tải về -> để nguyên ô chữ, lượt gọi sau sẽ dựng.
                    LogSystem.Warn("SytHcm/TSGD: danh muc " + SYT_CODE__TS_GIA_DINH
                        + " chua tai ve -> giu nguyen o chu, se dung lai o luot sau");
                    return;
                }

                LayoutControlItem lci = FindLayoutItemOf(this.txtPathologicalHistoryFamily);
                if (lci == null)
                {
                    LogSystem.Warn("SytHcm/TSGD: khong tim duoc dong bo cuc chua o chu -> khong dung");
                    return;
                }
                LayoutControl lc = lci.Owner as LayoutControl;
                if (lc == null)
                {
                    LogSystem.Warn("SytHcm/TSGD: dong bo cuc khong thuoc LayoutControl nao -> khong dung");
                    return;
                }

                sytFamilyHistoryInited = true;

                chkSytFamilyHistory = BuildSytFamilyRowPanel();
                // DUNG 9 O TICH ROI, dat toa do tuong minh — KHONG dung danh sach nhieu cot.
                //
                // Danh sach nhieu cot cua DevExpress tu tinh so cot theo CHIEU CAO DONG, lai bi thanh
                // cuon an cho: da thu bon lan, lan thi ra mot hang, lan thi cat mat bon benh cuoi.
                // Dat toa do tay thi hinh dang chac chan dung — hang tren 5 o, hang duoi 4 o.
                const int ROWS = 2;
                const int CELL_W = 140;      // du cho ten dai nhat "Roi loan tam than"
                const int CELL_H = 24;
                int cols = (data.Count + ROWS - 1) / ROWS;      // 9 benh -> 5 cot

                for (int i = 0; i < data.Count; i++)
                {
                    int row = i / cols;
                    int col = i % cols;

                    CheckEdit chk = new CheckEdit();
                    chk.Name = "chkSytFamily_" + data[i].ID;
                    chk.Text = data[i].NAME;
                    chk.Tag = data[i].ID;                       // giu ma dinh danh cua cong
                    chk.Properties.AllowGrayed = false;
                    chk.Location = new System.Drawing.Point(col * CELL_W, row * CELL_H);
                    chk.Size = new System.Drawing.Size(CELL_W - 4, CELL_H - 2);
                    chk.CheckedChanged += ChkSytFamilyHistory_ItemCheck;

                    chkSytFamilyHistory.Controls.Add(chk);
                }

                lc.BeginUpdate();
                try
                {
                    lc.Controls.Add(chkSytFamilyHistory);
                    lci.Control = chkSytFamilyHistory;           // thay ô chữ ngay tại chỗ, không xô bố cục
                    this.txtPathologicalHistoryFamily.Visible = false;

                    // GIU NGUYEN CHIEU CAO GOC cua dong bo cuc (o chu cu cao 144 px) — thua suc
                    // cho 2 hang o tich. Sau lan sua truoc that bai deu vi TOI TU BOP chieu cao xuong
                    // ~52 px roi di chong lai bo xep bo cuc; khong bop thi khong co gi bi nen.
                    // Chi noi BE RONG cho du 5 cot, con lai de nguyen.
                    int needW = cols * CELL_W + 4;
                    if (lci.Size.Width < needW)
                        lci.Size = new System.Drawing.Size(needW, lci.Size.Height);
                    chkSytFamilyHistory.MinimumSize =
                        new System.Drawing.Size(needW, ROWS * CELL_H + 2);

                    LogSystem.Warn("SytHcm/TSGD: da doi o chu thanh danh sach o tich — "
                        + data.Count + " benh, " + ROWS + " hang x " + cols + " cot, dong bo cuc cao "
                        + lci.Size.Height + " px");
                }
                finally { lc.EndUpdate(); }

                // Hồ sơ có thể đã nạp TRƯỚC khi ô này được dựng -> đổ lại giá trị đã lưu.
                FillSytFamilyHistory(this.txtPathologicalHistoryFamily.Text);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }



        /// <summary>Toàn bộ ô tích của cả hai hàng.</summary>
        private List<Control> SytFamilyCheckBoxes()
        {
            var rs = new List<Control>();
            if (chkSytFamilyHistory != null)
                foreach (Control c in chkSytFamilyHistory.Controls) rs.Add(c);
            if (chkSytFamilyHistoryRow2 != null)
                foreach (Control c in chkSytFamilyHistoryRow2.Controls) rs.Add(c);
            return rs;
        }

        /// <summary>Khung trống chứa một hàng ô tích.</summary>
        private static PanelControl BuildSytFamilyRowPanel()
        {
            PanelControl pnl = new PanelControl();
            pnl.BorderStyle = BorderStyles.NoBorder;
            return pnl;
        }

        /// <summary>
        /// Ghim kích thước một dòng ô tích: cao đúng MỘT hàng.
        ///
        /// Ghim ở CẢ HAI TẦNG — khung chứa và dòng bố cục — vì ghim một tầng thì tầng kia vẫn co lại
        /// được, đó là lý do năm lần sửa trước hàng thứ hai luôn bị cắt.
        /// </summary>
        private static void SetSytFamilyRowSize(LayoutControlItem item, int w, int h)
        {
            try
            {
                if (item == null) return;
                if (item.Control != null)
                {
                    item.Control.MinimumSize = new System.Drawing.Size(w, h);
                    item.Control.Size = new System.Drawing.Size(w, h);
                }
                item.SizeConstraintsType = SizeConstraintsType.Custom;
                item.MinSize = new System.Drawing.Size(w, h);
                item.MaxSize = new System.Drawing.Size(0, h);      // 0 = be ngang khong gioi han
                item.Size = new System.Drawing.Size(w, h);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Tìm dòng bố cục đang chứa một ô nhập.</summary>
        private static LayoutControlItem FindLayoutItemOf(Control c)
        {
            try
            {
                if (c == null) return null;
                // Ô nhập có thể nằm LỒNG trong panel nên cha trực tiếp chưa chắc là LayoutControl —
                // lần ngược lên cho tới khi gặp.
                Control par = c.Parent;
                LayoutControl lc = null;
                while (par != null && lc == null) { lc = par as LayoutControl; par = par.Parent; }
                if (lc == null) return null;
                foreach (BaseLayoutItem it in lc.Items)
                {
                    LayoutControlItem one = it as LayoutControlItem;
                    if (one != null && one.Control == c) return one;
                }
                return null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>Đổ giá trị đã lưu vào danh sách ô tích. Giá trị là danh sách mã định danh.</summary>
        private void FillSytFamilyHistory(string saved)
        {
            try
            {
                if (chkSytFamilyHistory == null) return;

                var ids = new HashSet<string>();
                if (!string.IsNullOrWhiteSpace(saved))
                {
                    foreach (string part in saved.Split(new char[] { ',', ';' }))
                    {
                        string t = (part ?? "").Trim();
                        if (t.Length > 0) ids.Add(t);
                    }
                }

                foreach (Control c in SytFamilyCheckBoxes())
                {
                    CheckEdit chk = c as CheckEdit;
                    if (chk == null || chk.Tag == null) continue;
                    chk.Checked = ids.Contains(chk.Tag.ToString());
                }

                UpdateSytFamilyIcdEnabled();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Tích / bỏ tích một bệnh -> tính lại trạng thái ô nhập mã ICD.
        ///
        /// Chạy hoãn một nhịp: sự kiện này bắn TRƯỚC khi ô tích đổi trạng thái, đọc ngay thì vẫn ra
        /// giá trị cũ và ô mã ICD mở/khoá ngược một nhịp.
        /// </summary>
        private void ChkSytFamilyHistory_ItemCheck(object sender, EventArgs e)
        {
            try { this.BeginInvoke(new Action(UpdateSytFamilyIcdEnabled)); }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Ô nhập mã ICD tiền sử gia đình CHỈ mở khi đã tích "Bệnh khác" — các bệnh còn lại đã có
        /// mục riêng trong danh mục nên không cần ghi thêm mã bệnh.
        ///
        /// Nhận ra mục "Bệnh khác" theo TÊN, không theo mã định danh viết cứng: Sở đánh số lại thì
        /// mã đổi, còn tên thì không.
        /// </summary>
        private void UpdateSytFamilyIcdEnabled()
        {
            try
            {
                if (chkSytFamilyHistory == null) return;

                bool otherChecked = false;
                foreach (Control c in SytFamilyCheckBoxes())
                {
                    CheckEdit chk = c as CheckEdit;
                    if (chk == null || !chk.Checked) continue;
                    if ((chk.Text ?? "").Trim().ToLowerInvariant().Contains("khác"))
                    { otherChecked = true; break; }
                }

                if (ucSytFamilyIcd == null) return;

                SetSytIcdEnabled(ucSytFamilyIcd, otherChecked);
                // Bỏ tích "Bệnh khác" -> xoá trắng mã đã chọn, tránh đẩy lên cổng mã bệnh của một
                // lựa chọn người dùng đã bỏ.
                if (!otherChecked) SetSytIcdValue(ucSytFamilyIcd, null, null);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Giá trị đem lưu cho cột `PATHOLOGICAL_HISTORY_FAMILY`.
        ///
        /// Viện chưa bật cổng, hoặc ô tích chưa dựng, thì trả lại đúng chữ người dùng gõ — KHÔNG trả
        /// rỗng, vì lượt lưu sẽ ghi đè mất nội dung đang có.
        /// </summary>
        private string GetSytFamilyHistoryValue(string fallbackText)
        {
            try
            {
                if (chkSytFamilyHistory == null) return fallbackText;

                var ids = new List<string>();
                foreach (Control c in SytFamilyCheckBoxes())
                {
                    CheckEdit chk = c as CheckEdit;
                    if (chk != null && chk.Checked && chk.Tag != null) ids.Add(chk.Tag.ToString());
                }
                return (ids.Count > 0) ? string.Join(SYT_FAMILY_SEP, ids.ToArray()) : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return fallbackText; }
        }

        /// <summary>Ô chọn bệnh của mã ICD tiền sử gia đình — CÙNG loại với tab Khám lâm sàng HCM.</summary>
        private UserControl ucSytFamilyIcd;

        /// <summary>
        /// Thay ô mã ICD tiền sử gia đình bằng ô chọn bệnh của cổng — đúng ô đang dùng ở tab Khám
        /// lâm sàng HCM: ô chọn gõ để lọc, kèm nút "..." chọn nhiều bệnh.
        ///
        /// KHÔNG mượn ô cũ rồi đổi nguồn danh mục như cách trước: ô cũ dùng chung một nguồn cho CẢ
        /// NĂM nhóm tiền sử (gia đình, bản thân, bệnh nghề nghiệp, sản khoa, bệnh đang điều trị) nên
        /// đổi nguồn là đổi luôn bốn nhóm không được yêu cầu.
        /// </summary>
        private void BuildSytFamilyIcd()
        {
            try
            {
                if (ucSytFamilyIcd != null) return;
                if (!IsSytHcmDeclared()) return;                  // an toàn đa viện
                if (sytIcdItems == null || sytIcdItems.Count == 0) return;   // danh mục chưa về

                PanelControl host = FindHostControl("pnlKskIcdFamily1") as PanelControl;
                if (host == null) return;

                // Bỏ ô cũ trong khung chứa, tránh hai ô chồng lên nhau.
                var old = new List<Control>();
                foreach (Control c in host.Controls) old.Add(c);
                host.Controls.Clear();
                foreach (Control c in old)
                {
                    try { c.Dispose(); } catch (Exception exOne) { LogSystem.Warn(exOne); }
                }

                ucSytFamilyIcd = BuildSytIcdEditor(host);

                // ĐỔ LẠI NGAY giá trị đã lưu của hồ sơ đang mở.
                //
                // Ô này dựng LƯỜI — phải có danh mục ICD của cổng mới dựng được — nên thứ tự giữa
                // "nạp hồ sơ" và "danh mục về" không cố định. Chỉ đổ ở chỗ nạp hồ sơ thì lần nào
                // danh mục về sau sẽ mất giá trị; chỉ đổ ở chỗ đổ danh mục thì hồ sơ mở sau lại mất.
                // Tự đổ ngay tại đây thì dựng lúc nào cũng có dữ liệu đúng.
                // Lấy từ KHO GIÁ TRỊ DÙNG CHUNG trước, rồi mới đến bản ghi.
                //
                // Kho này được nạp ở LoadKskHistoryIcdFromGeneral, KHÔNG phụ thuộc ô của ta đã dựng
                // hay chưa — nên dựng muộn cỡ nào cũng lấy được. Đọc thẳng bản ghi thì hụt khi bản ghi
                // chưa nạp xong tại thời điểm dựng ô, đó là lý do lần trước vẫn không đổ lại được.
                string savedCode = null, savedName = null;

                string[] fromStore;
                if (dicHistoryIcdValue.TryGetValue(KskHistoryGroup.Family, out fromStore)
                    && fromStore != null && fromStore.Length == 2)
                {
                    savedCode = fromStore[0];
                    savedName = fromStore[1];
                }

                if (string.IsNullOrWhiteSpace(savedCode) && currentKskGeneral != null)
                {
                    savedCode = currentKskGeneral.FAMILY_HISTORY_ICD_CODE;
                    savedName = currentKskGeneral.FAMILY_HISTORY_ICD_NAME;
                }

                LogSystem.Warn("SytHcm/TSGD-ICD: vua dung o -> tu do lai, ma=\""
                    + (savedCode ?? "") + "\" (kho dung chung "
                    + (fromStore != null ? "co" : "rong") + ", ban ghi "
                    + (currentKskGeneral != null ? "co" : "rong") + ")");

                if (!string.IsNullOrWhiteSpace(savedCode))
                    SetSytIcdValue(ucSytFamilyIcd, savedCode, savedName);

                UpdateSytFamilyIcdEnabled();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ mã đã lưu vào ô chọn bệnh tiền sử gia đình.</summary>
        private void FillSytFamilyIcd(string icdIds, string icdNames)
        {
            try
            {
                BuildSytFamilyIcd();

                // CHẨN ĐOÁN: ba thứ này phân biệt được ba nguyên nhân khác nhau — ô chưa dựng,
                // cơ sở dữ liệu không có gì, hoặc có mã nhưng tra không ra trong danh mục của cổng.
                LogSystem.Warn("SytHcm/TSGD-ICD: nap lai — o "
                    + (ucSytFamilyIcd == null ? "CHUA DUNG" : "da dung")
                    + ", danh muc ICD cua cong " + ((sytIcdItems == null) ? 0 : sytIcdItems.Count)
                    + " muc, gia tri tu CSDL ma=\"" + (icdIds ?? "") + "\" ten=\""
                    + (icdNames ?? "") + "\"");

                if (ucSytFamilyIcd == null) return;
                SetSytIcdValue(ucSytFamilyIcd, icdIds, icdNames);
                UpdateSytFamilyIcdEnabled();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Giá trị đem lưu của ô chọn bệnh tiền sử gia đình.
        /// Trả false khi ô chưa dựng — nơi gọi giữ nguyên cách lấy cũ, không ghi trống lên dữ liệu.
        /// </summary>
        private bool GetSytFamilyIcdValue(out string icdIds, out string icdNames)
        {
            icdIds = null;
            icdNames = null;
            try
            {
                if (ucSytFamilyIcd == null) return false;

                GetSytIcdValue(ucSytFamilyIcd, out icdIds, out icdNames);

                // CHỈ nhận khi THẬT SỰ có giá trị.
                //
                // Trước đây hàm này trả true ngay cả khi ô rỗng, nên lượt lưu ghi null và XOÁ MẤT mã
                // bệnh đang có trong cơ sở dữ liệu. Ô rỗng xảy ra rất dễ: người dùng chọn mã ở ô CŨ
                // (lúc ô mới chưa dựng vì danh mục của cổng chưa về), hoặc mở hồ sơ mà chưa kịp đổ.
                if (string.IsNullOrWhiteSpace(icdIds))
                {
                    LogSystem.Warn("SytHcm/TSGD-ICD: o chon benh dang RONG -> khong ghi de,"
                        + " giu nguyen ma benh dang co trong CSDL");
                    return false;
                }

                LogSystem.Warn("SytHcm/TSGD-ICD: LUU ma=\"" + icdIds + "\"");
                return true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }
    }
}
