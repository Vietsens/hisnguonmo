/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tiền sử ICD (R5/R8): cụm chọn mã ICD đặt cạnh ô văn bản tự do tiền sử ở mỗi tab.
 * - 1 giá trị (mã + tên) cho mỗi nhóm, dùng chung lượt khám, đồng bộ hiển thị giữa các tab (R5).
 * - Cảnh báo khi có nội dung tiền sử nhưng chưa chọn ICD, vẫn cho lưu (R8).
 * - LƯU/ĐỌC xuống HIS_KSK_GENERAL chờ BE bổ sung cột (xem các hàm có TODO(BE)).
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Tiền sử ICD — Fields =====

        /// <summary>1 giá trị [mã, tên] cho mỗi nhóm tiền sử — dùng chung cả lượt khám.</summary>
        private readonly Dictionary<KskHistoryGroup, string[]> dicHistoryIcdValue =
            new Dictionary<KskHistoryGroup, string[]>();

        /// <summary>Mọi instance UC theo nhóm (trên các tab khác nhau) — để đồng bộ hiển thị.</summary>
        private readonly Dictionary<KskHistoryGroup, List<UcKskHistoryIcd>> dicHistoryIcdUc =
            new Dictionary<KskHistoryGroup, List<UcKskHistoryIcd>>();

        /// <summary>Ô văn bản tự do theo nhóm — phục vụ cảnh báo R8 (có nội dung mà chưa chọn ICD).</summary>
        private readonly Dictionary<KskHistoryGroup, List<Control>> dicHistoryIcdAnchorText =
            new Dictionary<KskHistoryGroup, List<Control>>();

        private List<HIS_ICD> historyIcdDataSource;
        private int historyIcdPageSize = 50;

        #endregion

        #region ===== Init / Embed =====

        /// <summary>
        /// Nhúng cụm chọn ICD vào các tab có nhóm tiền sử (B.4.1). Gọi 1 lần ở Load,
        /// SAU FillDataToPages (để control gốc đã nằm trong LayoutControl).
        /// HIỆN TẠI: làm bản mẫu cho tab "Ksk định kỳ" (General). Các tab khác nhân bản sau khi duyệt.
        /// </summary>
        private void InitKskHistoryIcdForTabs()
        {
            try
            {
                this.historyIcdDataSource = BackendDataWorker.Get<HIS_ICD>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.ICD_CODE).ToList();
                this.historyIcdPageSize = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;

                // Mỗi cụm: nếu có PanelControl host đặt sẵn trong Designer (theo tên) → nhúng vào panel (KHÔNG xô layout).
                // Nếu CHƯA có panel → chèn tạm vào LayoutControl cạnh ô gốc (hiện được ngay, layout có thể xô).

                // ===== TAB 0 — Ksk định kỳ (General) =====
                EmbedHistoryIcd("pnlKskIcdPersonal0", this.txtPathologicalHistory, "KskHistoryIcd.Caption.Personal", KskHistoryGroup.Personal, this.txtPathologicalHistory);
                EmbedHistoryIcd("pnlKskIcdOccupational0", this.txtOccuOne, "KskHistoryIcd.Caption.Occupational", KskHistoryGroup.Occupational, this.txtOccuOne, this.txtDiseaseOccuTwo);
                EmbedHistoryIcd("pnlKskIcdObstetric0", this.txtExamObstetric, "KskHistoryIcd.Caption.Obstetric", KskHistoryGroup.Obstetric, this.txtExamObstetric);

                // ===== TAB 1 — Ksk trên 18 tuổi (OverEighteen) =====
                EmbedHistoryIcd("pnlKskIcdFamily1", this.txtPathologicalHistoryFamily, "KskHistoryIcd.Caption.Family", KskHistoryGroup.Family, this.txtPathologicalHistoryFamily);
                EmbedHistoryIcd("pnlKskIcdPersonal1", this.txtPathologicalHistory2, "KskHistoryIcd.Caption.Personal", KskHistoryGroup.Personal, this.txtPathologicalHistory2);
                // (Bỏ EmbedHistoryIcd "pnlKskIcdObstetric1"/txtMaternityHistory: panel không tồn tại → fallback chèn UC 300px
                //  vào layout runtime, đè khu tiền sử. Obstetric ICD của trên-18 đã dùng pnlKskIcdObstetricExam2 (gắn ô khám sản khoa) bên dưới.)
                // Mã ICD sản khoa ở mục 3 (Sản phụ khoa) tab Khám lâm sàng — panel riêng, cùng nhóm Obstetric.
                EmbedHistoryIcd("pnlKskIcdObstetricExam2", this.txtExamObstetric2, "KskHistoryIcd.Caption.Obstetric", KskHistoryGroup.Obstetric, this.txtExamObstetric2);

                // ===== TAB 2 — Ksk dưới 18 tuổi (UnderEight) =====
                EmbedHistoryIcd("pnlKskIcdFamily2", this.txtPathologicalHistoryFamily3, "KskHistoryIcd.Caption.Family", KskHistoryGroup.Family, this.txtPathologicalHistoryFamily3);
                EmbedHistoryIcd("pnlKskIcdPersonal2", this.txtPathologicalHistory3, "KskHistoryIcd.Caption.Personal", KskHistoryGroup.Personal, this.txtPathologicalHistory3);
                EmbedHistoryIcd("pnlKskIcdObstetric2", this.txtMarternityHistory3, "KskHistoryIcd.Caption.Obstetric", KskHistoryGroup.Obstetric, this.txtMarternityHistory3);

                // ===== TAB 3 — Ksk lái xe (có Gia đình/Bản thân/Sản khoa — suffix 4) =====
                EmbedHistoryIcd("pnlKskIcdFamily3", this.txtPathologicalHistoryFamily4, "KskHistoryIcd.Caption.Family", KskHistoryGroup.Family, this.txtPathologicalHistoryFamily4);
                EmbedHistoryIcd("pnlKskIcdPersonal3", this.txtPathologicalHistory4, "KskHistoryIcd.Caption.Personal", KskHistoryGroup.Personal, this.txtPathologicalHistory4);
                EmbedHistoryIcd("pnlKskIcdObstetric3", this.txtMaternityHistory4, "KskHistoryIcd.Caption.Obstetric", KskHistoryGroup.Obstetric, this.txtMaternityHistory4);

                // ===== TAB 4 — Ksk lái xe ô tô (chỉ có tiền sử bản thân — suffix 5) =====
                EmbedHistoryIcd("pnlKskIcdPersonal4", this.txtDiseaseOne5, "KskHistoryIcd.Caption.Personal", KskHistoryGroup.Personal, this.txtDiseaseOne5, this.txtDiseaseTwo5);

                // ===== TAB 7 — Trẻ em dưới 6 tuổi (tiền sử gia đình + bản thân — suffix 8) =====
                EmbedHistoryIcd("pnlKskIcdFamily8", this.memHistoryFamily8, "KskHistoryIcd.Caption.Family", KskHistoryGroup.Family, this.memHistoryFamily8);
                EmbedHistoryIcd("pnlKskIcdPersonal8", this.memHistoryPersonal8, "KskHistoryIcd.Caption.Personal", KskHistoryGroup.Personal, this.memHistoryPersonal8);

                // ===== Nhóm "Bệnh đang điều trị" — cụm ICD độc lập (không có ô text tự do) cho mọi tab có tiền sử =====
                EmbedHistoryIcd("pnlKskIcdTreatment0", this.txtPathologicalHistory, "KskHistoryIcd.Caption.Treatment", KskHistoryGroup.Treatment);
                EmbedHistoryIcd("pnlKskIcdTreatment8", this.memHistoryPersonal8, "KskHistoryIcd.Caption.Treatment", KskHistoryGroup.Treatment);
                EmbedHistoryIcd("pnlKskIcdTreatment1", this.txtPathologicalHistory2, "KskHistoryIcd.Caption.Treatment", KskHistoryGroup.Treatment);
                EmbedHistoryIcd("pnlKskIcdTreatment2", this.txtPathologicalHistory3, "KskHistoryIcd.Caption.Treatment", KskHistoryGroup.Treatment);
                EmbedHistoryIcd("pnlKskIcdTreatment3", this.txtPathologicalHistory4, "KskHistoryIcd.Caption.Treatment", KskHistoryGroup.Treatment);
                EmbedHistoryIcd("pnlKskIcdTreatment4", this.txtDiseaseOne5, "KskHistoryIcd.Caption.Treatment", KskHistoryGroup.Treatment);
                EmbedHistoryIcd("pnlKskIcdTreatment6", this.txtPathologicalHistory7, "KskHistoryIcd.Caption.Treatment", KskHistoryGroup.Treatment);

                // ===== TAB 6 — Ksk nghề nghiệp (Occupational) =====
                EmbedHistoryIcd("pnlKskIcdFamily6", this.txtPathologicalFamily, "KskHistoryIcd.Caption.Family", KskHistoryGroup.Family, this.txtPathologicalFamily);
                EmbedHistoryIcd("pnlKskIcdPersonal6", this.txtPathologicalHistory7, "KskHistoryIcd.Caption.Personal", KskHistoryGroup.Personal, this.txtPathologicalHistory7);
                EmbedHistoryIcd("pnlKskIcdObstetric6", this.txtExamObstetric7, "KskHistoryIcd.Caption.Obstetric", KskHistoryGroup.Obstetric, this.txtExamObstetric7);

                // (Đã bỏ block trùng "TAB 7 — pnlKskIcdFamily7/Personal7": panel không tồn tại nên rơi vào
                //  nhánh dự phòng chèn UC 300px vào layout runtime → đè tiêu đề hành chính. Under-6 đã nhúng
                //  đúng ở pnlKskIcdFamily8/Personal8 phía trên.)
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Nhúng 1 UcKskHistoryIcd: ưu tiên PanelControl host (đặt sẵn trong Designer, theo tên) — không xô layout;
        /// nếu chưa có panel thì chèn tạm vào LayoutControl cạnh <paramref name="fallbackAnchor"/>.
        /// </summary>
        private void EmbedHistoryIcd(string hostPanelName, Control fallbackAnchor, string captionKey,
            KskHistoryGroup group, params Control[] warningTextControls)
        {
            try
            {
                // Đăng ký ô text để cảnh báo R8 (kể cả khi chưa nhúng được cụm).
                if (warningTextControls != null)
                    foreach (Control c in warningTextControls) RegisterHistoryAnchorText(group, c);

                UcKskHistoryIcd uc = new UcKskHistoryIcd();
                uc.Group = group;

                bool placed;
                Control host = FindHostControl(hostPanelName);
                if (host != null)
                {
                    // Bỏ viền PanelControl host — tránh viền 2 lớp (viền panel + viền TextEdit trong UC), chỉ giữ 1 lớp.
                    var pnlHost = host as DevExpress.XtraEditors.PanelControl;
                    if (pnlHost != null) pnlHost.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

                    host.Controls.Add(uc);
                    uc.Dock = System.Windows.Forms.DockStyle.Fill;
                    placed = true;
                }
                else
                {
                    placed = InsertControlIntoLayoutAfter(fallbackAnchor, uc, GetLangValue(captionKey), 300);
                }
                if (!placed) return;

                uc.InitUc(this.historyIcdDataSource, this.historyIcdPageSize);
                uc.IcdChanged += OnHistoryIcdChanged;
                RegisterHistoryUc(group, uc);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Tìm control host theo Name trong toàn form (đệ quy). Null nếu chưa đặt.</summary>
        private Control FindHostControl(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) return null;
                Control[] found = this.Controls.Find(name, true);
                return (found != null && found.Length > 0) ? found[0] : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Chèn 1 control vào LayoutControl chứa <paramref name="anchor"/>, ngay bên dưới item của anchor.
        /// Dùng làm phương án tạm khi chưa có panel host (layout có thể xô). Trả về true nếu chèn được.
        /// </summary>
        private bool InsertControlIntoLayoutAfter(Control anchor, Control control, string caption, int minWidth)
        {
            try
            {
                if (anchor == null || control == null) return false;
                DevExpress.XtraLayout.LayoutControl lc = anchor.Parent as DevExpress.XtraLayout.LayoutControl;
                if (lc == null) return false;
                DevExpress.XtraLayout.LayoutControlItem anchorItem =
                    lc.GetItemByControl(anchor) as DevExpress.XtraLayout.LayoutControlItem;

                lc.BeginUpdate();
                try
                {
                    lc.Controls.Add(control);
                    DevExpress.XtraLayout.LayoutControlItem item =
                        lc.GetItemByControl(control) as DevExpress.XtraLayout.LayoutControlItem;
                    if (item == null) return false;
                    if (string.IsNullOrEmpty(caption))
                    {
                        item.TextVisible = false;
                    }
                    else
                    {
                        item.Text = caption;
                        item.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
                        item.TextSize = new System.Drawing.Size(150, 20);
                        item.TextToControlDistance = 5;
                    }
                    item.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                    item.MinSize = new System.Drawing.Size(minWidth, 24);
                    item.MaxSize = new System.Drawing.Size(0, 24);
                    if (anchorItem != null)
                        item.Move(anchorItem, DevExpress.XtraLayout.Utils.InsertType.Bottom);
                }
                finally { lc.EndUpdate(); }
                return true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        private void RegisterHistoryUc(KskHistoryGroup group, UcKskHistoryIcd uc)
        {
            List<UcKskHistoryIcd> list;
            if (!dicHistoryIcdUc.TryGetValue(group, out list))
            {
                list = new List<UcKskHistoryIcd>();
                dicHistoryIcdUc[group] = list;
            }
            list.Add(uc);
        }

        private void RegisterHistoryAnchorText(KskHistoryGroup group, Control c)
        {
            if (c == null) return;
            List<Control> list;
            if (!dicHistoryIcdAnchorText.TryGetValue(group, out list))
            {
                list = new List<Control>();
                dicHistoryIcdAnchorText[group] = list;
            }
            if (!list.Contains(c)) list.Add(c);
        }

        #endregion

        #region ===== Đồng bộ giá trị giữa các tab (R5) =====

        /// <summary>User chọn ICD ở 1 cụm → lưu vào store + đẩy sang mọi cụm cùng nhóm trên các tab khác.</summary>
        private void OnHistoryIcdChanged(KskHistoryGroup group, string codes, string names)
        {
            try
            {
                dicHistoryIcdValue[group] = new string[] { codes ?? "", names ?? "" };
                List<UcKskHistoryIcd> list;
                if (dicHistoryIcdUc.TryGetValue(group, out list))
                    foreach (UcKskHistoryIcd u in list)
                        if (u != null) u.SetData(codes, names);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ giá trị ICD tiền sử (từ store / DB) vào tất cả cụm UC. Gọi ở Load sau Init.</summary>
        private void LoadKskHistoryIcdToUc()
        {
            try
            {
                LoadKskHistoryIcdFromGeneral(currentKskGeneral);
                // O chon benh cua cong (nhom gia dinh) khong nam trong dicHistoryIcdUc nen phai do rieng.
                string[] famVal;
                if (dicHistoryIcdValue.TryGetValue(KskHistoryGroup.Family, out famVal)
                    && famVal != null && famVal.Length == 2)
                    FillSytFamilyIcd(famVal[0], famVal[1]);

                foreach (var kv in dicHistoryIcdUc)
                {
                    string codes = "", names = "";
                    string[] val;
                    if (dicHistoryIcdValue.TryGetValue(kv.Key, out val) && val != null && val.Length == 2)
                    {
                        codes = val[0];
                        names = val[1];
                    }
                    foreach (UcKskHistoryIcd u in kv.Value)
                        if (u != null) u.SetData(codes, names);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Map DB — chờ BE bổ sung cột HIS_KSK_GENERAL =====

        /// <summary>Lấy mã ICD đã chọn của 1 nhóm (ghép ;).</summary>
        private string GetHistoryIcdCode(KskHistoryGroup group)
        {
            string[] v;
            return dicHistoryIcdValue.TryGetValue(group, out v) && v != null && v.Length > 0 ? v[0] : "";
        }

        /// <summary>Lấy tên ICD đã chọn của 1 nhóm (ghép ;).</summary>
        private string GetHistoryIcdName(KskHistoryGroup group)
        {
            string[] v;
            return dicHistoryIcdValue.TryGetValue(group, out v) && v != null && v.Length > 1 ? v[1] : "";
        }

        /// <summary>Đọc mã + tên ICD 5 nhóm tiền sử từ HIS_KSK_GENERAL vào store (dùng chung lượt khám).</summary>
        private void LoadKskHistoryIcdFromGeneral(HIS_KSK_GENERAL g)
        {
            // g == null (lượt khám CHƯA có bản ghi) → XÓA store, KHÔNG return: dicHistoryIcdValue là
            // readonly Dictionary sống suốt đời form và không được reset ở ReloadForServiceReq, nên
            // return sớm là LoadKskHistoryIcdToUc đọc lại đúng mã ICD tiền sử của bệnh nhân TRƯỚC.
            if (g == null) { dicHistoryIcdValue.Clear(); return; }
            try
            {
                dicHistoryIcdValue[KskHistoryGroup.Family] =
                    new string[] { g.FAMILY_HISTORY_ICD_CODE ?? "", g.FAMILY_HISTORY_ICD_NAME ?? "" };
                // Viện đã bật cổng -> ô mã ICD tiền sử gia đình là ô chọn bệnh riêng, đổ vào luôn.
                FillSytFamilyIcd(g.FAMILY_HISTORY_ICD_CODE, g.FAMILY_HISTORY_ICD_NAME);
                dicHistoryIcdValue[KskHistoryGroup.Personal] =
                    new string[] { g.PERSONAL_HISTORY_ICD_CODE ?? "", g.PERSONAL_HISTORY_ICD_NAME ?? "" };
                dicHistoryIcdValue[KskHistoryGroup.Occupational] =
                    new string[] { g.OCCUPATIONAL_DISEASE_ICD_CODE ?? "", g.OCCUPATIONAL_DISEASE_ICD_NAME ?? "" };
                dicHistoryIcdValue[KskHistoryGroup.Obstetric] =
                    new string[] { g.OBSTETRIC_DISEASE_ICD_CODE ?? "", g.OBSTETRIC_DISEASE_ICD_NAME ?? "" };
                dicHistoryIcdValue[KskHistoryGroup.Treatment] =
                    new string[] { g.TREATING_DISEASE_ICD_CODE ?? "", g.TREATING_DISEASE_ICD_NAME ?? "" };
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Ghi mã + tên ICD 5 nhóm tiền sử (từ store) vào HIS_KSK_GENERAL trước khi gọi API lưu.</summary>
        private void FillKskHistoryIcdToGeneral(HIS_KSK_GENERAL g)
        {
            if (g == null) return;
            try
            {
                // Ô chọn bệnh riêng của cổng (nếu đã dựng) là nguồn đúng; chưa dựng thì lấy như cũ.
                string famIds, famNames;
                if (GetSytFamilyIcdValue(out famIds, out famNames))
                {
                    g.FAMILY_HISTORY_ICD_CODE = NullIfEmpty(famIds);
                    g.FAMILY_HISTORY_ICD_NAME = NullIfEmpty(famNames);
                }
                else
                {
                    // Ô chọn bệnh của cổng chưa dựng hoặc đang rỗng -> lấy theo cách cũ; cách cũ cũng
                    // rỗng thì GIỮ NGUYÊN giá trị đang có trong bản ghi, KHÔNG ghi null.
                    //
                    // Không giữ thì mở hồ sơ rồi bấm Cập nhật là mất mã bệnh tiền sử gia đình — ô cũ
                    // đã bị thay bằng ô của cổng nên nó luôn trả rỗng.
                    string oldCode = NullIfEmpty(GetHistoryIcdCode(KskHistoryGroup.Family));
                    string oldName = NullIfEmpty(GetHistoryIcdName(KskHistoryGroup.Family));

                    g.FAMILY_HISTORY_ICD_CODE = !string.IsNullOrWhiteSpace(oldCode)
                        ? oldCode
                        : ((currentKskGeneral != null) ? currentKskGeneral.FAMILY_HISTORY_ICD_CODE : null);
                    g.FAMILY_HISTORY_ICD_NAME = !string.IsNullOrWhiteSpace(oldName)
                        ? oldName
                        : ((currentKskGeneral != null) ? currentKskGeneral.FAMILY_HISTORY_ICD_NAME : null);
                }
                LogSystem.Warn("SytHcm/TSGD-ICD: gan vao ban ghi -> FAMILY_HISTORY_ICD_CODE=\""
                    + (g.FAMILY_HISTORY_ICD_CODE ?? "") + "\", NAME=\""
                    + (g.FAMILY_HISTORY_ICD_NAME ?? "") + "\"");

                g.PERSONAL_HISTORY_ICD_CODE = NullIfEmpty(GetHistoryIcdCode(KskHistoryGroup.Personal));
                g.PERSONAL_HISTORY_ICD_NAME = NullIfEmpty(GetHistoryIcdName(KskHistoryGroup.Personal));
                g.OCCUPATIONAL_DISEASE_ICD_CODE = NullIfEmpty(GetHistoryIcdCode(KskHistoryGroup.Occupational));
                g.OCCUPATIONAL_DISEASE_ICD_NAME = NullIfEmpty(GetHistoryIcdName(KskHistoryGroup.Occupational));
                g.OBSTETRIC_DISEASE_ICD_CODE = NullIfEmpty(GetHistoryIcdCode(KskHistoryGroup.Obstetric));
                g.OBSTETRIC_DISEASE_ICD_NAME = NullIfEmpty(GetHistoryIcdName(KskHistoryGroup.Obstetric));
                g.TREATING_DISEASE_ICD_CODE = NullIfEmpty(GetHistoryIcdCode(KskHistoryGroup.Treatment));
                g.TREATING_DISEASE_ICD_NAME = NullIfEmpty(GetHistoryIcdName(KskHistoryGroup.Treatment));
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Truyền lại loại mẫu KSK (KSK_TYPE_ID) đã lưu sẵn trong HIS_KSK_GENERAL của lượt khám.
        /// Khi sửa ICD phải gửi kèm KSK_TYPE_ID hiện có (không đổi) để BE map đúng cổng QĐ1551.
        /// Dùng reflection để KHÔNG crash khi MOS.EFMODEL.dll lúc chạy lệch phiên bản (chưa có/khác kiểu
        /// cột KSK_TYPE_ID) — chỉ copy giá trị khi property tồn tại và đọc/ghi được.
        /// </summary>
        private void SetKskTypeIdToGeneral(HIS_KSK_GENERAL g)
        {
            if (g == null || currentKskGeneral == null) return;
            try
            {
                var prop = typeof(HIS_KSK_GENERAL).GetProperty("KSK_TYPE_ID");
                if (prop == null || !prop.CanRead || !prop.CanWrite) return;
                object value = prop.GetValue(currentKskGeneral, null);
                if (value != null) prop.SetValue(g, value, null);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Ghi giá trị KSK_TYPE_ID (loại biểu mẫu QĐ 1551) vào HIS_KSK_GENERAL qua reflection —
        /// KHÔNG lỗi biên dịch/chạy khi MOS.EFMODEL.dll lệch phiên bản (chưa có/khác kiểu cột).
        /// Tự ép về đúng kiểu property (long/long?/int/short...).
        /// </summary>
        private void SetKskTypeIdValue(HIS_KSK_GENERAL g, long typeId)
        {
            if (g == null) return;
            try
            {
                var prop = typeof(HIS_KSK_GENERAL).GetProperty("KSK_TYPE_ID");
                if (prop == null || !prop.CanWrite) return;
                System.Type target = System.Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                object val = System.Convert.ChangeType(typeId, target);
                prop.SetValue(g, val, null);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Xác định KSK_TYPE_ID theo tab đang lưu (bảng phân loại biểu mẫu QĐ 1551):
        ///  0 "Ksk định kỳ" (chỉ GENERAL)=2, 1 "trên 18 tuổi"=2, 2 "dưới 18 tuổi"=1,
        ///  3 "lái xe"=3, 4 "lái xe ô tô"=3, 5 "KSK khác"=2, 6 "nghề nghiệp"=2,
        ///  7 "trẻ dưới 6 tuổi" = tính theo tuổi (6–13).
        /// Trả về null nếu không xác định được (→ giữ nguyên giá trị hiện có).
        /// </summary>
        private long? ResolveKskTypeIdByTab(int tabIndex, HIS_KSK_GENERAL g)
        {
            switch (tabIndex)
            {
                case 0: return 2; // Ksk định kỳ → HIS_KSK_GENERAL (chỉ GENERAL)
                case 1: return 2; // Ksk trên 18 tuổi → HIS_KSK_OVER_EIGHTEEN
                case 2: return 1; // Ksk dưới 18 tuổi → HIS_KSK_UNDER_EIGHTEEN
                case 3: return 3; // Ksk lái xe (định kỳ) → lái xe
                case 4: return 3; // Ksk lái xe ô tô → HIS_KSK_DRIVER
                case 5: return 2; // KSK khác (hôn nhân/di chúc) → HIS_KSK_OTHER
                case 6: return 2; // Ksk nghề nghiệp → HIS_KSK_OCCUPATIONAL
                case 7: return ComputeKskTypeIdUnderSix(g); // Trẻ dưới 6 tuổi → tính theo tuổi (6–13)
                default: return null;
            }
        }

        #endregion

        #region ===== Cảnh báo R8 =====

        /// <summary>
        /// R8: nhóm tiền sử nào có nội dung text nhưng chưa chọn ICD → cảnh báo nhắc chọn mã ICD
        /// (để đẩy cổng theo QĐ 1551), nhưng VẪN cho lưu/tiếp tục (không khóa).
        /// </summary>
        private void ShowKskHistoryIcdWarningIfAny()
        {
            try
            {
                List<KskHistoryGroup> missing = new List<KskHistoryGroup>();
                foreach (var kv in dicHistoryIcdAnchorText)
                {
                    bool hasText = kv.Value != null
                        && kv.Value.Any(c => c != null && !string.IsNullOrWhiteSpace(c.Text));
                    if (!hasText) continue;
                    if (string.IsNullOrWhiteSpace(GetHistoryIcdCodeEffective(kv.Key))) missing.Add(kv.Key);
                }
                if (missing.Count == 0) return;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(GetLangValue("KskHistoryIcd.Warning.MissingIcd"));
                foreach (KskHistoryGroup g in missing)
                    sb.AppendLine("   - " + GetLangValue(GetGroupNameKey(g)));

                XtraMessageBox.Show(sb.ToString(), "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Mã ICD của một nhóm tiền sử, lấy từ ĐÚNG ô đang hiển thị.
        ///
        /// Nhóm gia đình ở viện đã bật cổng Sở Y tế dùng ô chọn bệnh RIÊNG; ô cũ đã bị thay nên đọc
        /// ô cũ luôn ra rỗng, khiến cảnh báo "chưa chọn mã ICD" hiện dù người dùng đã chọn.
        /// </summary>
        private string GetHistoryIcdCodeEffective(KskHistoryGroup group)
        {
            if (group == KskHistoryGroup.Family)
            {
                string ids, names;
                if (GetSytFamilyIcdValue(out ids, out names)) return ids;
            }
            return GetHistoryIcdCode(group);
        }

        private string GetGroupNameKey(KskHistoryGroup group)
        {
            switch (group)
            {
                case KskHistoryGroup.Family: return "KskHistoryIcd.GroupName.Family";
                case KskHistoryGroup.Personal: return "KskHistoryIcd.GroupName.Personal";
                case KskHistoryGroup.Occupational: return "KskHistoryIcd.GroupName.Occupational";
                case KskHistoryGroup.Obstetric: return "KskHistoryIcd.GroupName.Obstetric";
                default: return "KskHistoryIcd.GroupName.Treatment";
            }
        }

        #endregion

        #region ===== Helper localization =====

        /// <summary>Lấy text đa ngôn ngữ từ Lang.resx; fallback về chính key nếu thiếu.</summary>
        private string GetLangValue(string key)
        {
            try
            {
                if (Resources.ResourceLanguageManager.LanguageResource == null)
                {
                    Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager(
                        "HIS.Desktop.Plugins.EnterKskInfomantionVer2.Resources.Lang",
                        typeof(frmEnterKskInfomantionVer2).Assembly);
                }
                string v = Inventec.Common.Resource.Get.Value(key,
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                return string.IsNullOrEmpty(v) ? key : v;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return key;
            }
        }

        #endregion
    }
}
