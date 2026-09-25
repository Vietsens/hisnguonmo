/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * XUẤT / NHẬP FILE MẪU EXCEL (việc 57621) — phần gắn vào form nhập KSK.
 *
 * "Xuất mẫu": ghi ra file Excel các mục khám lâm sàng của mẫu khám đang mở, cột Kết quả điền sẵn
 * "Bình thường", cột Phân loại điền sẵn "Loại I". Người dùng mở file sửa mục nào khác thường.
 * "Nhập mẫu": đọc file đã sửa, điền lên form. KHÔNG tự lưu xuống DB, bác sĩ xem lại rồi bấm Lưu.
 *
 * >>> BA BẪY BẮT BUỘC XỬ LÝ KHI ĐIỀN <<<
 *  1. Ô đang BỊ KHÓA: một mục đã có người khám khác thì LoginNameEnableControl khóa TOÀN BỘ ô của
 *     mục đó (cả Kết quả, Phân loại, số đo), không riêng ô chọn bác sĩ. Gán vào ô đang khóa là
 *     mất dữ liệu mà không báo gì -> phải kiểm Enabled/ReadOnly NGAY TRƯỚC từng lần gán.
 *  2. Ô Phân loại lưu ID danh mục chứ không lưu chuỗi "Loại I" -> phải tra ngược theo TÊN.
 *  3. Ô chỉ tồn tại trên form khi tab đã dựng; tra control theo tên trong vùng tab, không theo
 *     tên toàn form (nhiều tab có ô trùng tên, chỉ khác hậu tố).
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Kết quả một lần nhập mẫu, để báo lại cho người dùng.</summary>
        private class ExcelApplyResult
        {
            /// <summary>Số ô đã điền.</summary>
            public int Applied;

            /// <summary>Số ô bỏ qua vì đang khóa theo người khám.</summary>
            public int SkippedLocked;

            /// <summary>Số ô Phân loại bỏ qua vì tên không có trong danh mục của bệnh viện.</summary>
            public int SkippedRank;

            /// <summary>Số ô trong file không còn trên màn hình.</summary>
            public int SkippedMissing;
        }

        #region ===== Tra control theo tên trong vùng tab =====

        /// <summary>Bộ nhớ đệm control theo tên, theo từng tab (dựng 1 lần cho mỗi tab).</summary>
        private readonly Dictionary<int, Dictionary<string, BaseEdit>> excelEditorCache
            = new Dictionary<int, Dictionary<string, BaseEdit>>();

        /// <summary>
        /// Bảng tra tên control -> control của một tab. Duyệt ĐỆ QUY vì control nằm lồng nhiều lớp
        /// (LayoutControl lồng nhau, GroupBox, Panel, TableLayoutPanel, tab con của 2 mẫu khám
        /// trên 18 và dưới 18 tuổi) — duyệt một cấp là mất nguyên mục.
        /// </summary>
        private Dictionary<string, BaseEdit> GetExcelEditors(int tabIndex)
        {
            try
            {
                Dictionary<string, BaseEdit> cached;
                if (this.excelEditorCache.TryGetValue(tabIndex, out cached)) return cached;

                var map = new Dictionary<string, BaseEdit>(StringComparer.OrdinalIgnoreCase);
                if (this.xtraTabControl1 != null && tabIndex >= 0 && tabIndex < this.xtraTabControl1.TabPages.Count)
                {
                    CollectEditorsByName(this.xtraTabControl1.TabPages[tabIndex], map);
                }
                this.excelEditorCache[tabIndex] = map;
                return map;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return new Dictionary<string, BaseEdit>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private void CollectEditorsByName(Control parent, Dictionary<string, BaseEdit> map)
        {
            if (parent == null) return;
            foreach (Control c in parent.Controls)
            {
                BaseEdit be = c as BaseEdit;
                if (be != null)
                {
                    if (!string.IsNullOrEmpty(be.Name) && !map.ContainsKey(be.Name)) map.Add(be.Name, be);
                    continue;
                }
                if (c.HasChildren) CollectEditorsByName(c, map);
            }
        }

        #endregion

        #region ===== Nút Xuất mẫu =====

        /// <summary>Nút "Xuất mẫu" — ghi file Excel của mẫu khám đang mở, đã điền sẵn giá trị mặc định.</summary>
        private void btnKskExcelExport_Click(object sender, EventArgs e)
        {
            try
            {
                int tabIndex = this.xtraTabControl1.SelectedTabPageIndex;
                string tabName = this.xtraTabControl1.SelectedTabPage == null
                    ? "" : this.xtraTabControl1.SelectedTabPage.Text;

                var rows = KskExcelMap.GetRows(tabIndex, IsFemalePatient());
                if (rows.Count == 0)
                {
                    XtraMessageBox.Show("Mẫu khám này không có mục nào để xuất.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Điền sẵn giá trị mặc định vào file: đây chính là mục đích của tính năng.
                // Mục có giá trị riêng (thị lực "10", thính lực "5" và "0,5") thì lấy giá trị đó;
                // còn lại lấy giá trị chung "Bình thường".
                string rankName = GetDefaultRankName();
                foreach (var row in rows)
                {
                    if (!string.IsNullOrEmpty(row.MA_O))
                        row.KET_QUA = (row.GIA_TRI_RIENG == null) ? KskExcelMap.DEFAULT_RESULT : row.GIA_TRI_RIENG;
                    if (!string.IsNullOrEmpty(row.MA_O_PHAN_LOAI)) row.PHAN_LOAI = rankName;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Title = "Xuất file mẫu khám sức khỏe";
                    sfd.Filter = "File Excel (*.xlsx)|*.xlsx";
                    sfd.DefaultExt = "xlsx";
                    sfd.AddExtension = true;
                    sfd.FileName = BuildExportFileName(tabName);
                    if (sfd.ShowDialog(this) != DialogResult.OK) return;

                    if (!KskExcelFile.Write(sfd.FileName, tabName, rows))
                    {
                        XtraMessageBox.Show("Không xuất được file. Vui lòng thử lại.", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (XtraMessageBox.Show(
                            "Đã xuất " + rows.Count + " mục ra file:" + Environment.NewLine + sfd.FileName
                            + Environment.NewLine + Environment.NewLine + "Bạn có muốn mở file ngay?",
                            "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        try { System.Diagnostics.Process.Start(sfd.FileName); }
                        catch (Exception exOpen) { LogSystem.Warn(exOpen); }
                    }
                }
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Tên file gợi ý: MauKsk_&lt;mã y lệnh&gt;_&lt;tên mẫu khám không dấu&gt;.</summary>
        private string BuildExportFileName(string tabName)
        {
            try
            {
                string code = (this.currentServiceReq != null && !string.IsNullOrEmpty(this.currentServiceReq.SERVICE_REQ_CODE))
                    ? this.currentServiceReq.SERVICE_REQ_CODE : "";
                string name = RemoveDiacritics(tabName).Replace(" ", "");
                return "MauKsk" + (string.IsNullOrEmpty(code) ? "" : ("_" + code))
                     + (string.IsNullOrEmpty(name) ? "" : ("_" + name));
            }
            catch (Exception ex) { LogSystem.Warn(ex); return "MauKsk"; }
        }

        /// <summary>Bỏ dấu tiếng Việt để tên file không lỗi trên máy khác.</summary>
        private string RemoveDiacritics(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) return "";
                string norm = text.Normalize(System.Text.NormalizationForm.FormD);
                var sb = new System.Text.StringBuilder();
                foreach (char c in norm)
                {
                    if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                        != System.Globalization.UnicodeCategory.NonSpacingMark) sb.Append(c);
                }
                return sb.ToString().Normalize(System.Text.NormalizationForm.FormC)
                         .Replace('đ', 'd').Replace('Đ', 'D');
            }
            catch (Exception ex) { LogSystem.Warn(ex); return text; }
        }

        /// <summary>Tên phân loại mặc định ("Loại I") lấy từ danh mục đang hoạt động.</summary>
        private string GetDefaultRankName()
        {
            try
            {
                var rank = GetRankByLevel(KskExcelMap.DEFAULT_RANK_LEVEL);
                return rank == null ? "" : rank.HEALTH_EXAM_RANK_NAME;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return ""; }
        }

        /// <summary>
        /// Bản ghi phân loại theo mức 1..5. Ưu tiên mã "L1".."L5" (danh mục chuẩn dùng chung seed
        /// sẵn 5 bản ghi này), không có thì lấy bản ghi thứ &lt;mức&gt; theo mã.
        /// </summary>
        private HIS_HEALTH_EXAM_RANK GetRankByLevel(int level)
        {
            try
            {
                if (level < 1) return null;
                var all = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>();
                if (all == null) return null;
                var active = all.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                .OrderBy(o => o.HEALTH_EXAM_RANK_CODE).ToList();
                if (active.Count == 0) return null;

                var byCode = active.FirstOrDefault(o =>
                    string.Equals((o.HEALTH_EXAM_RANK_CODE ?? "").Trim(), "L" + level, StringComparison.OrdinalIgnoreCase));
                if (byCode != null) return byCode;

                return active.Count >= level ? active[level - 1] : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Bệnh nhân đang mở có phải nữ không — để bỏ mục Thai sản với bệnh nhân nam.
        /// Đọc thẳng tên giới tính có sẵn trên view y lệnh, không phải tra lại danh mục.
        /// Không xác định được thì trả true (xuất đủ mục) — thà thừa một dòng còn hơn thiếu.
        /// </summary>
        private bool IsFemalePatient()
        {
            try
            {
                if (this.currentServiceReq == null) return true;
                string name = (this.currentServiceReq.TDL_PATIENT_GENDER_NAME ?? "").Trim();
                if (string.IsNullOrEmpty(name)) return true;
                return name.IndexOf("Nữ", StringComparison.CurrentCultureIgnoreCase) >= 0;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return true; }
        }

        #endregion

        #region ===== Nút Nhập mẫu =====

        /// <summary>Nút "Nhập mẫu" — đọc file Excel đã sửa và điền lên form.</summary>
        private void btnKskExcelImport_Click(object sender, EventArgs e)
        {
            try
            {
                int tabIndex = this.xtraTabControl1.SelectedTabPageIndex;
                string tabName = this.xtraTabControl1.SelectedTabPage == null
                    ? "" : this.xtraTabControl1.SelectedTabPage.Text;

                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Nhập file mẫu khám sức khỏe";
                    ofd.Filter = "File Excel (*.xlsx)|*.xlsx|Tất cả (*.*)|*.*";
                    ofd.CheckFileExists = true;
                    ofd.Multiselect = false;
                    if (ofd.ShowDialog(this) != DialogResult.OK) return;

                    List<KskExcelRowADO> rows;
                    string error;
                    if (!KskExcelFile.Read(ofd.FileName, out rows, out error))
                    {
                        XtraMessageBox.Show(error, "Không nhập được file",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Hỏi xác nhận là BẮT BUỘC: ô đang có giá trị không chỉ do bác sĩ gõ, màn hình
                    // còn tự điền sẵn một số ô từ y lệnh của phòng khám.
                    if (XtraMessageBox.Show(
                            "Nhập mẫu sẽ ghi đè giá trị đang nhập trên mẫu khám " + tabName + ". Tiếp tục?",
                            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    var rs = ApplyExcelRows(tabIndex, rows);
                    XtraMessageBox.Show(BuildExcelApplyMessage(rs), "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Điền các dòng đọc từ file lên form.</summary>
        private ExcelApplyResult ApplyExcelRows(int tabIndex, List<KskExcelRowADO> rows)
        {
            var result = new ExcelApplyResult();
            try
            {
                if (rows == null || rows.Count == 0) return result;
                var editors = GetExcelEditors(tabIndex);

                foreach (var row in rows)
                {
                    if (row == null) continue;

                    // Ô Kết quả
                    if (!string.IsNullOrEmpty(row.MA_O))
                    {
                        BaseEdit edit;
                        if (!editors.TryGetValue(row.MA_O, out edit) || edit == null) result.SkippedMissing++;
                        else if (!IsEditable(edit)) result.SkippedLocked++;
                        else { edit.Text = row.KET_QUA ?? ""; result.Applied++; }
                    }

                    // Ô Phân loại
                    if (!string.IsNullOrEmpty(row.MA_O_PHAN_LOAI))
                    {
                        BaseEdit edit;
                        if (!editors.TryGetValue(row.MA_O_PHAN_LOAI, out edit) || edit == null) result.SkippedMissing++;
                        else if (!IsEditable(edit)) result.SkippedLocked++;
                        else
                        {
                            // Bỏ trống cột Phân loại trong file = xóa ô đó, không phải bỏ qua.
                            if (string.IsNullOrWhiteSpace(row.PHAN_LOAI)) { edit.EditValue = null; result.Applied++; }
                            else
                            {
                                var rank = FindRankByName(row.PHAN_LOAI);
                                if (rank == null) result.SkippedRank++;
                                else { edit.EditValue = rank.ID; result.Applied++; }
                            }
                        }
                    }
                }

                LogSystem.Debug("ApplyExcelRows__tab=" + tabIndex
                    + "__applied=" + result.Applied
                    + "__khoa=" + result.SkippedLocked
                    + "__phanLoai=" + result.SkippedRank
                    + "__khongCon=" + result.SkippedMissing);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
            return result;
        }

        /// <summary>Ô có đang cho sửa không — kiểm ngay trước từng lần gán.</summary>
        private bool IsEditable(BaseEdit edit)
        {
            try
            {
                if (!edit.Enabled) return false;
                if (edit.Properties != null && edit.Properties.ReadOnly) return false;
                return true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>Tra bản ghi phân loại theo TÊN người dùng ghi trong file (không phân biệt hoa thường).</summary>
        private HIS_HEALTH_EXAM_RANK FindRankByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name)) return null;
                string key = name.Trim();
                var all = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>();
                if (all == null) return null;
                return all.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                          .FirstOrDefault(o => string.Equals((o.HEALTH_EXAM_RANK_NAME ?? "").Trim(), key,
                                                             StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>Câu thông báo sau khi nhập: điền được bao nhiêu, bỏ qua cái gì và vì sao.</summary>
        private string BuildExcelApplyMessage(ExcelApplyResult rs)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("Đã điền ").Append(rs.Applied).Append(" ô.");
            if (rs.SkippedLocked > 0)
                sb.Append(" Bỏ qua ").Append(rs.SkippedLocked).Append(" ô đang khóa theo người khám.");
            if (rs.SkippedRank > 0)
                sb.Append(" Bỏ qua ").Append(rs.SkippedRank)
                  .Append(" ô Phân loại không có trong danh mục của bệnh viện.");
            if (rs.SkippedMissing > 0)
                sb.Append(" Bỏ qua ").Append(rs.SkippedMissing).Append(" ô không còn trên màn hình.");
            return sb.ToString();
        }

        #endregion

        #region ===== Bật/mờ 2 nút theo mẫu khám đang mở =====

        /// <summary>
        /// Mẫu khám không có mục nào xuất được (KSK khác) thì mờ 2 nút.
        /// Gọi lúc mở form và mỗi lần đổi mẫu khám.
        /// </summary>
        private void UpdateKskExcelButtonState()
        {
            try
            {
                if (this.xtraTabControl1 == null) return;
                bool has = KskExcelMap.GetRows(this.xtraTabControl1.SelectedTabPageIndex, true).Count > 0;
                if (this.btnKskExcelExport != null) this.btnKskExcelExport.Enabled = has;
                if (this.btnKskExcelImport != null) this.btnKskExcelImport.Enabled = has;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion
    }
}
