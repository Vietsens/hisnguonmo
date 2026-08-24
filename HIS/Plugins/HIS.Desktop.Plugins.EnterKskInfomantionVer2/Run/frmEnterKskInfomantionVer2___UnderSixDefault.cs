/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab "Trẻ em dưới 6 tuổi" — MẶC ĐỊNH cho 3 mục toàn radio đánh giá:
 *   IV. ĐÁNH GIÁ PHÁT TRIỂN TINH THẦN - VẬN ĐỘNG  (lcgPhatTrien8)
 *   V.  ĐÁNH GIÁ TIÊM CHỦNG                        (lcgTiemChung8)
 *   VI. KHÁM LÂM SÀNG                              (lcgKhamLamSang8, gồm các mục con 1 … 6)
 * KHÔNG nhận I/II/III: đó là số đo riêng của từng trẻ (nhiệt độ, mạch, cân nặng, vòng đầu…),
 * đặt mặc định sẽ ghi số liệu sai lệch cho bệnh nhân.
 *
 * Danh mục cho lưới thiết lập (form ⚙ Thiết lập → tab "Mặc định nhập KSK") được dựng ĐỘNG
 * bằng cách duyệt các layout group trên, KHÔNG hardcode danh sách ô:
 *   - "Mục"             = LayoutControlGroup có chứa TRỰC TIẾP RadioGroup (IV, V, 1. Da, 2.1 … 2.5, 3 … 6)
 *   - "Nội dung"        = từng RadioGroup trong nhóm đó, nhãn lấy từ LayoutControlItem.Text
 *   - "Giá trị mặc định"= từng RadioGroupItem của chính RadioGroup đó
 * Thêm/bớt ô trong Designer thì lưới thiết lập tự đổi theo, không phải sửa ở đây.
 *
 * Mỗi RadioGroup thuộc đúng 1 "Mục" (chỉ lấy nhóm chứa TRỰC TIẾP) nên không có đường trùng.
 * Bỏ qua item Visibility.Never — mục VI đang ẩn 10 ô ghi chú và 10 combo bác sĩ khám.
 *
 * Giá trị mặc định lưu theo MÁY qua ControlStateWorker — xem KskDefaultSettingUtil.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private List<KskDefaultGroupADO> underSixDefaultGroups;
        private List<KskDefaultFieldADO> underSixDefaultFields;
        private List<KskDefaultValueADO> underSixDefaultValues;

        /// <summary>Tên RadioGroup → control, để áp mặc định không phải duyệt lại layout.</summary>
        private Dictionary<string, RadioGroup> underSixDefaultRadios;

        #region ===== Dựng danh mục từ layout =====

        /// <summary>
        /// Duyệt layout các mục IV, V, VI dựng 3 danh mục + map tên→control.
        /// Idempotent: gọi nhiều lần chỉ dựng 1 lần.
        /// Chạy được kể cả khi tab "Trẻ em dưới 6 tuổi" chưa mở lần nào — control do Designer tạo sẵn,
        /// lazy-load chỉ ảnh hưởng phần ĐỔ DỮ LIỆU.
        /// </summary>
        private void BuildUnderSixDefaultCatalog()
        {
            try
            {
                if (this.underSixDefaultGroups != null) return;

                this.underSixDefaultGroups = new List<KskDefaultGroupADO>();
                this.underSixDefaultFields = new List<KskDefaultFieldADO>();
                this.underSixDefaultValues = new List<KskDefaultValueADO>();
                this.underSixDefaultRadios = new Dictionary<string, RadioGroup>();

                // Các mục cho phép đặt mặc định, THEO THỨ TỰ hiện trên form.
                // Chỉ nhận mục toàn radio "đánh giá" (IV, V, VI) — I/II/III là số đo của từng trẻ
                // (nhiệt độ, cân nặng, vòng đầu…) nên đặt mặc định không có nghĩa.
                var roots = new LayoutControlGroup[] { this.lcgPhatTrien8, this.lcgTiemChung8, this.lcgKhamLamSang8 };

                // Gom theo nhóm trước, sắp xếp sau: thứ tự Items trong Designer KHÔNG theo số mục
                // (1. Da → 6. Cơ xương → 2.1 …), combo hiện như vậy thì bác sĩ rất khó tìm.
                var collected = new List<KeyValuePair<KskDefaultGroupADO, List<KskDefaultFieldADO>>>();
                var rootIndex = new Dictionary<string, int>();
                for (int i = 0; i < roots.Length; i++)
                {
                    if (roots[i] == null) continue;
                    int from = collected.Count;
                    CollectUnderSixDefaultGroup(roots[i], collected);
                    for (int j = from; j < collected.Count; j++) rootIndex[collected[j].Key.GROUP_NAME] = i;
                }

                // Khóa sắp xếp: thứ tự mục lớn (IV → V → VI) rồi mới đến số mục con trong đó.
                // Phải chia 2 bậc vì mục lớn dùng số La Mã còn mục con dùng số Ả Rập.
                collected.Sort((a, b) =>
                {
                    int ra = rootIndex[a.Key.GROUP_NAME], rb = rootIndex[b.Key.GROUP_NAME];
                    if (ra != rb) return ra.CompareTo(rb);
                    return CompareSectionCaption(a.Key.GROUP_CAPTION, b.Key.GROUP_CAPTION);
                });

                foreach (var entry in collected)
                {
                    this.underSixDefaultGroups.Add(entry.Key);
                    this.underSixDefaultFields.AddRange(entry.Value);
                }
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>
        /// So sánh 2 tiêu đề nhóm theo số mục đứng đầu ("2.1. Khám đầu - cổ" → 2,1) để 2.2 nằm
        /// trước 2.10 và 3 nằm sau 2.5. Không đọc được số thì so chuỗi.
        /// </summary>
        private int CompareSectionCaption(string a, string b)
        {
            int[] na = ParseSectionNumber(a);
            int[] nb = ParseSectionNumber(b);
            for (int i = 0; i < Math.Max(na.Length, nb.Length); i++)
            {
                int va = i < na.Length ? na[i] : -1;
                int vb = i < nb.Length ? nb[i] : -1;
                if (va != vb) return va.CompareTo(vb);
            }
            return string.Compare(a ?? "", b ?? "", StringComparison.CurrentCulture);
        }

        /// <summary>Bóc dãy số đầu tiêu đề: "2.5. Khám miệng, răng" → [2,5]; không có → mảng rỗng.</summary>
        private int[] ParseSectionNumber(string caption)
        {
            var parts = new List<int>();
            if (string.IsNullOrEmpty(caption)) return parts.ToArray();
            int i = 0, value = 0;
            bool hasDigit = false;
            while (i < caption.Length)
            {
                char c = caption[i];
                if (char.IsDigit(c)) { value = value * 10 + (c - '0'); hasDigit = true; }
                else if (c == '.' && hasDigit) { parts.Add(value); value = 0; hasDigit = false; }
                else break;
                i++;
            }
            if (hasDigit) parts.Add(value);
            return parts.ToArray();
        }

        /// <summary>Đệ quy 1 nhóm layout: lấy RadioGroup con trực tiếp, rồi đi tiếp vào nhóm con.</summary>
        private void CollectUnderSixDefaultGroup(LayoutControlGroup group,
                                                 List<KeyValuePair<KskDefaultGroupADO, List<KskDefaultFieldADO>>> collected)
        {
            try
            {
                if (group == null || group.Items == null) return;

                var fields = new List<KskDefaultFieldADO>();
                var order = new Dictionary<string, System.Drawing.Point>();
                var childGroups = new List<LayoutControlGroup>();

                foreach (BaseLayoutItem item in group.Items)
                {
                    LayoutControlGroup child = item as LayoutControlGroup;
                    if (child != null) { childGroups.Add(child); continue; }

                    LayoutControlItem lci = item as LayoutControlItem;
                    if (lci == null || lci.Visibility == LayoutVisibility.Never) continue;

                    RadioGroup rdo = lci.Control as RadioGroup;
                    if (rdo == null || string.IsNullOrEmpty(rdo.Name)) continue;
                    if (this.underSixDefaultRadios.ContainsKey(rdo.Name)) continue;

                    fields.Add(new KskDefaultFieldADO()
                    {
                        GROUP_NAME = group.Name,
                        FIELD_NAME = rdo.Name,
                        FIELD_CAPTION = CleanCaption(lci.Text)
                    });
                    order[rdo.Name] = lci.Location;
                    this.underSixDefaultRadios.Add(rdo.Name, rdo);
                    CollectUnderSixDefaultValues(rdo);
                }

                // Chỉ nhóm có ô mới lên danh sách "Mục" — nhóm chỉ chứa nhóm con (VD "2. Đầu - cổ")
                // không hiện, tránh 1 ô đi được bằng 2 đường Mục khác nhau.
                if (fields.Count > 0)
                {
                    // Sắp theo vị trí trên form (trên→dưới, trái→phải): thứ tự Items trong Designer
                    // không khớp thứ tự bác sĩ nhìn thấy.
                    fields.Sort((a, b) =>
                    {
                        System.Drawing.Point pa = order[a.FIELD_NAME], pb = order[b.FIELD_NAME];
                        if (pa.Y != pb.Y) return pa.Y.CompareTo(pb.Y);
                        return pa.X.CompareTo(pb.X);
                    });

                    collected.Add(new KeyValuePair<KskDefaultGroupADO, List<KskDefaultFieldADO>>(
                        new KskDefaultGroupADO()
                        {
                            GROUP_NAME = group.Name,
                            GROUP_CAPTION = CleanCaption(group.Text)
                        }, fields));
                }

                foreach (var child in childGroups) CollectUnderSixDefaultGroup(child, collected);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ các lựa chọn của 1 RadioGroup thành datasource cột "Giá trị mặc định".</summary>
        private void CollectUnderSixDefaultValues(RadioGroup rdo)
        {
            try
            {
                if (rdo == null || rdo.Properties == null || rdo.Properties.Items == null) return;
                foreach (RadioGroupItem it in rdo.Properties.Items)
                {
                    if (it == null || it.Value == null) continue;
                    long value;
                    try { value = Convert.ToInt64(it.Value); }
                    catch (Exception exVal) { LogSystem.Warn(exVal); continue; }

                    this.underSixDefaultValues.Add(new KskDefaultValueADO()
                    {
                        FIELD_NAME = rdo.Name,
                        VALUE_KEY = KskDefaultSettingUtil.BuildValueKey(rdo.Name, value),
                        VALUE = value,
                        VALUE_CAPTION = CleanCaption(it.Description)
                    });
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Bỏ khoảng trắng thừa và dấu hai chấm cuối nhãn layout ("Kết quả:" → "Kết quả").</summary>
        private string CleanCaption(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            string result = text.Trim();
            if (result.EndsWith(":")) result = result.Substring(0, result.Length - 1).TrimEnd();
            return result;
        }

        #endregion

        #region ===== Áp mặc định vào control =====

        /// <summary>
        /// Điền mặc định đã cấu hình vào các RadioGroup mục VI.
        /// <paramref name="overwriteFilled"/> = true (bấm "Áp dụng ngay") → ghi đè cả ô đã chọn;
        /// false (tự động lúc mở bản ghi mới) → chỉ điền ô còn trống, không đè tay bác sĩ.
        /// </summary>
        private void ApplyUnderSixDefaults(bool overwriteFilled)
        {
            try
            {
                List<KskDefaultRowADO> saved;
                bool autoApply;
                KskDefaultSettingUtil.Load(out saved, out autoApply);
                ApplyUnderSixDefaultRows(saved, overwriteFilled);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>
        /// Áp danh sách dòng thiết lập vào RadioGroup. CHỈ dòng có tích "Dùng" (IS_USED) được áp —
        /// dòng bỏ tích vẫn còn nguyên trong ControlState, chỉ là tạm không dùng.
        /// Tách khỏi <see cref="ApplyUnderSixDefaults"/> để test được mà không cần ControlState.
        /// </summary>
        private int ApplyUnderSixDefaultRows(List<KskDefaultRowADO> rows, bool overwriteFilled)
        {
            int applied = 0, skippedUnused = 0, skippedFilled = 0;
            try
            {
                BuildUnderSixDefaultCatalog();
                if (rows == null || rows.Count == 0) return 0;
                if (this.underSixDefaultRadios == null || this.underSixDefaultRadios.Count == 0) return 0;

                foreach (var row in rows)
                {
                    if (row == null) continue;
                    if (!row.IS_USED) { skippedUnused++; continue; }

                    long? value = KskDefaultSettingUtil.ParseValueFromKey(row.VALUE_KEY);
                    if (value == null) continue;

                    RadioGroup rdo;
                    if (!this.underSixDefaultRadios.TryGetValue(row.FIELD_NAME, out rdo) || rdo == null) continue;
                    if (!overwriteFilled && GetRadioValue(rdo) != null) { skippedFilled++; continue; }
                    SetRadioValue(rdo, value.Value);
                    applied++;
                }
                LogSystem.Debug("ApplyUnderSixDefaultRows__overwrite=" + overwriteFilled
                    + "__applied=" + applied + "/" + rows.Count
                    + "__khongDung=" + skippedUnused + "__daCoGiaTri=" + skippedFilled);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
            return applied;
        }

        /// <summary>
        /// Gọi khi mở y lệnh CHƯA có bản ghi HIS_KSK_UNDER_SIX. Chỉ chạy nếu người dùng đã bật
        /// "Tự động điền mặc định khi mở bản ghi mới" trong form Thiết lập.
        /// </summary>
        private void ApplyUnderSixDefaultsOnNewRecord()
        {
            try
            {
                List<KskDefaultRowADO> saved;
                bool autoApply;
                KskDefaultSettingUtil.Load(out saved, out autoApply);
                if (!autoApply || !saved.Any(o => o.IS_USED)) return;
                ApplyUnderSixDefaults(false);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion
    }
}
