/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Dựng khối KHÁM LÂM SÀNG của gói dữ liệu đẩy lên Nền tảng KSK Sở Y tế TP.HCM (mẫu M3).
 *
 * Khối này gồm 15 mục khám, mỗi mục 6 trường:
 *      <mục>_chuaphathienbatthuong   0/1
 *      <mục>_chandoansobo            0/1   <- suy ra: có mã bệnh sơ bộ thì 1
 *      <mục>_chandoansobo_icd        chuỗi mã bệnh
 *      <mục>_chandoanxacdinh         0/1   <- suy ra: có mã bệnh xác định thì 1
 *      <mục>_chandoanxacdinh_icd     chuỗi mã bệnh
 *      <mục>_phanloai                Id danh mục phân loại của cổng (1016–1020)
 *
 * NGUỒN DỮ LIỆU — chỉ đọc từ cơ sở dữ liệu, không gọi mạng:
 *      chưa phát hiện bất thường + mã bệnh -> bảng HIS_KSK_SYT_HCM
 *      phân loại                           -> bảng HIS_KSK_OVER_EIGHTEEN (cột EXAM_*_RANK),
 *                                             riêng Phụ khoa ở HIS_KSK_SYT_HCM
 *
 * QUY ĐỔI PHÂN LOẠI: hồ sơ lưu mã của HIS, cổng nhận Id của cổng. Quy đổi khóa theo
 * CẤP ĐỘ I–V, không so khớp tên nguyên văn (hai bên viết khác nhau: "Loại I" / "Loại 1")
 * và không suy Id bằng phép tính (mã của Sở là dữ liệu, Sở đổi mã là sai ngay).
 * Danh mục của cổng đọc từ tệp đã lưu tại máy — do màn hình nhập KSK tải về.
 */
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>Một mục trong danh mục của cổng Sở Y tế.</summary>
    internal class KskSytCatalogItem
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    internal static class KskSytHcmPayload
    {
        #region ===== Bản đồ 15 mục khám =====

        /// <summary>
        /// Một mục khám: khóa trường của cổng · tiền tố cột ở bảng mẫu M3 · cột phân loại.
        /// Khóa trường của cổng trùng khóa mục trên màn hình nhập nên đọc rất dễ đối chiếu.
        /// </summary>
        private class Section
        {
            public string Field { get; set; }        // noikhoa, hohap, ...
            public string ColumnPrefix { get; set; } // EXAM_CIRCULATION, ...
            public string RankColumn { get; set; }   // EXAM_CIRCULATION_RANK, ...
            /// <summary>true = phân loại nằm ở bảng mẫu M3, không phải bảng KSK.</summary>
            public bool RankInSytTable { get; set; }
        }

        /// <summary>
        /// 15 mục theo dữ liệu mẫu của Sở. `noikhoa` ứng với mục **a) Tuần hoàn**, các mục b→h là
        /// trường riêng — nên KHÔNG phải gộp 8 phân loại nội khoa thành một như mẫu M4.
        /// </summary>
        private static readonly List<Section> SECTIONS = new List<Section>
        {
            Sec("noikhoa",      "EXAM_CIRCULATION",  "EXAM_CIRCULATION_RANK"),
            Sec("hohap",        "EXAM_RESPIRATORY",  "EXAM_RESPIRATORY_RANK"),
            Sec("tieuhoa",      "EXAM_DIGESTION",    "EXAM_DIGESTION_RANK"),
            // Cột ở bảng KSK là ..._UROLOGY_, ở bảng mẫu M3 rút thành ..._URO_ (giới hạn 30 ký tự của Oracle).
            Sec("thantietnieu", "EXAM_KIDNEY_URO",   "EXAM_KIDNEY_UROLOGY_RANK"),
            Sec("noitiet",      "EXAM_OEND",         "EXAM_OEND_RANK"),
            Sec("coxuongkhop",  "EXAM_MUSCLE_BONE",  "EXAM_MUSCLE_BONE_RANK"),
            Sec("thankinh",     "EXAM_NEUROLOGICAL", "EXAM_NEUROLOGICAL_RANK"),
            Sec("tamthan",      "EXAM_MENTAL",       "EXAM_MENTAL_RANK"),
            Sec("ngoaikhoa",    "EXAM_SURGERY",      "EXAM_SURGERY_RANK"),
            Sec("dalieu",       "EXAM_DERMATOLOGY",  "EXAM_DERMATOLOGY_RANK"),
            Sec("sankhoa",      "EXAM_OBSTETRIC",    "EXAM_OBSTETRIC_RANK"),
            // Phụ khoa là mục tách mới: bảng KSK chỉ có một cặp cột dùng chung cho sản phụ khoa.
            SecSyt("phukhoa",   "EXAM_GYNECOLOGY",   "EXAM_GYNECOLOGY_RANK"),
            Sec("mat",          "EXAM_EYE",          "EXAM_EYE_RANK"),
            Sec("tmh",          "EXAM_ENT",          "EXAM_ENT_RANK"),
            Sec("rhm",          "EXAM_STOMATOLOGY",  "EXAM_STOMATOLOGY_RANK")
        };

        private static Section Sec(string field, string prefix, string rankCol)
        {
            return new Section { Field = field, ColumnPrefix = prefix, RankColumn = rankCol, RankInSytTable = false };
        }

        private static Section SecSyt(string field, string prefix, string rankCol)
        {
            return new Section { Field = field, ColumnPrefix = prefix, RankColumn = rankCol, RankInSytTable = true };
        }

        /// <summary>
        /// Mục Thần kinh có tên trường LỆCH so với 14 mục còn lại: `thankinh_chuandoansobo`
        /// (**chuan**, không phải **chan**). Đặc tả M3 ghi như vậy ở CẢ HAI chỗ — ví dụ body mục 2.2
        /// và bảng chi tiết mục 2.3 dòng 37 — nên gửi theo đúng đặc tả.
        ///
        /// Chữ ký băm trên chính body nên sai một tên trường là 400 cả hồ sơ. Nếu thực tế cổng trả 400
        /// mà các mục khác đều đúng thì đổi cờ này về false để gửi theo tên chuẩn.
        /// </summary>
        private const bool SYT_THANKINH_TYPO_FIELD = true;

        private const string SYT_CODE__PHAN_LOAI = "KSKDK_PhanLoai_SK";

        #endregion

        #region ===== Dựng khối khám lâm sàng =====

        /// <summary>
        /// Dựng 15 mục khám lâm sàng. Trả về cặp "tên trường của cổng" -> "giá trị", để nơi gọi
        /// trộn vào gói dữ liệu đầy đủ. Thiếu dữ liệu thì trả trường rỗng chứ không bỏ trường —
        /// cổng đối chiếu theo danh sách trường nên thiếu trường dễ bị từ chối.
        /// </summary>
        internal static Dictionary<string, object> BuildClinicalExam(
            HIS_KSK_SYT_HCM syt, HIS_KSK_OVER_EIGHTEEN ksk, List<HIS_HEALTH_EXAM_RANK> hisRanks)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            try
            {
                int missingRank = 0;
                int borrowedRank = 0;

                // Phân loại TỔNG của hồ sơ — dùng bù cho mục khám không có phân loại riêng.
                // Cổng bắt buộc mọi mục phải có phân loại, mà nhiều viện chỉ nhập phân loại tổng.
                long? overallRankId = GetLong(ksk, "HEALTH_EXAM_RANK_ID");
                long? overallSytRank = ResolveSytRankId(overallRankId, hisRanks);
                foreach (Section s in SECTIONS)
                {
                    short? isNormal = GetShort(syt, s.ColumnPrefix + "_IS_NORMAL");
                    string preIcd = GetStr(syt, s.ColumnPrefix + "_PRE_ICD_CODE");
                    string finalIcd = GetStr(syt, s.ColumnPrefix + "_ICD_CODE");

                    body[s.Field + "_chuaphathienbatthuong"] = (isNormal == 1) ? 1 : 0;

                    string preFlagName = (s.Field == "thankinh" && SYT_THANKINH_TYPO_FIELD)
                        ? "thankinh_chuandoansobo" : s.Field + "_chandoansobo";
                    body[preFlagName] = !string.IsNullOrWhiteSpace(preIcd) ? 1 : 0;
                    body[s.Field + "_chandoansobo_icd"] = preIcd ?? "";

                    body[s.Field + "_chandoanxacdinh"] = !string.IsNullOrWhiteSpace(finalIcd) ? 1 : 0;
                    body[s.Field + "_chandoanxacdinh_icd"] = finalIcd ?? "";

                    long? hisRankId = s.RankInSytTable
                        ? GetLong(syt, s.RankColumn)
                        : GetLong(ksk, s.RankColumn);
                    long? sytRankId = ResolveSytRankId(hisRankId, hisRanks);
                    if (hisRankId.HasValue && hisRankId.Value > 0 && !sytRankId.HasValue) missingRank++;

                    // Mục này không có phân loại riêng -> lấy phân loại tổng của chính hồ sơ đó.
                    // Đây KHÔNG phải bịa số: là phân loại sức khỏe do bác sĩ kết luận cho hồ sơ.
                    if (!sytRankId.HasValue && overallSytRank.HasValue)
                    {
                        sytRankId = overallSytRank;
                        borrowedRank++;
                    }
                    body[s.Field + "_phanloai"] = sytRankId.HasValue ? (object)sytRankId.Value : null;
                }

                if (borrowedRank > 0)
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: " + borrowedRank
                        + " muc kham KHONG co phan loai rieng -> lay bu phan loai TONG cua ho so. "
                        + "Muon gui dung tung muc thi bac si phai nhap phan loai o tab Kham lam sang.");

                if (missingRank > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: " + missingRank
                        + " muc kham co phan loai nhung KHONG quy doi duoc sang Id cua cong "
                        + "-> gui trong. Kiem tra danh muc " + SYT_CODE__PHAN_LOAI + " da tai ve chua");
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
            return body;
        }

        #endregion

        #region ===== Quy đổi phân loại sức khỏe =====

        /// <summary>
        /// Đọc cấp độ 1–5 từ một chuỗi phân loại sức khỏe: "2" · "Loại 2" · "Loại II" · "Sức khỏe loại IV".
        ///
        /// VÌ SAO KHÔNG DÙNG LẠI HÀM CỦA 4 CỔNG CŨ: hàm đó tìm số La Mã bằng cách quét cả chuỗi, mà chữ
        /// "LOẠI" có sẵn chữ "I" ở cuối nên "Loại I" đến "Loại V" đều bị đọc thành 1. Ở đây lấy TỪ CUỐI
        /// CÙNG của chuỗi rồi mới đọc, nên không mắc chữ "I" trong "LOẠI".
        /// </summary>
        internal static int ParseSytRankLevel(string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value)) return 0;

                string[] parts = value.Trim().Split(new char[] { ' ', '\t', ':', '-', '.', ',' },
                    StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return 0;

                string last = parts[parts.Length - 1].Trim().ToUpperInvariant();

                int n;
                if (int.TryParse(last, out n)) return (n >= 1 && n <= 5) ? n : 0;

                switch (last)
                {
                    case "V": return 5;
                    case "IV": return 4;
                    case "III": return 3;
                    case "II": return 2;
                    case "I": return 1;
                    default: return 0;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return 0; }
        }

        /// <summary>Cấp độ 1–5 của một phân loại sức khỏe trong danh mục HIS (theo mã, không có thì theo tên).</summary>
        private static int ResolveHisRankLevel(long hisRankId, List<HIS_HEALTH_EXAM_RANK> hisRanks)
        {
            try
            {
                if (hisRanks == null) return 0;
                foreach (var r in hisRanks)
                {
                    if (r == null || r.ID != hisRankId) continue;
                    int lv = ParseSytRankLevel(r.HEALTH_EXAM_RANK_CODE);
                    if (lv == 0) lv = ParseSytRankLevel(r.HEALTH_EXAM_RANK_NAME);
                    if (lv == 0)
                        Inventec.Common.Logging.LogSystem.Warn("SytHcm: khong doc duoc cap do phan loai "
                            + "suc khoe cua HIS — ma=\"" + r.HEALTH_EXAM_RANK_CODE + "\", ten=\""
                            + r.HEALTH_EXAM_RANK_NAME + "\"");
                    return lv;
                }
                return 0;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return 0; }
        }

        /// <summary>Bảng cấp độ I–V -> Id của cổng, dựng một lần từ danh mục đã tải về.</summary>
        private static Dictionary<int, long> sytRankByLevel;

        /// <summary>
        /// Quy đổi phân loại của HIS sang Id của cổng.
        /// Hai bước, đều khóa theo CẤP ĐỘ: HIS -> cấp độ 1..5, rồi cấp độ -> Id của cổng.
        /// </summary>
        internal static long? ResolveSytRankId(long? hisRankId, List<HIS_HEALTH_EXAM_RANK> hisRanks)
        {
            try
            {
                if (!hisRankId.HasValue || hisRankId.Value <= 0) return null;

                int level = ResolveHisRankLevel(hisRankId.Value, hisRanks);
                if (level < 1 || level > 5) return null;

                if (sytRankByLevel == null) sytRankByLevel = BuildSytRankByLevel();
                long id;
                return sytRankByLevel.TryGetValue(level, out id) ? (long?)id : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Dựng bảng cấp độ -> Id từ danh mục của cổng, đọc cấp độ trong TÊN mục ("Loại III" -> 3).
        /// Đọc từ danh mục chứ không tính theo mã, để Sở đổi mã thì phần mềm tự đúng theo.
        /// </summary>
        private static Dictionary<int, long> BuildSytRankByLevel()
        {
            Dictionary<int, long> map = new Dictionary<int, long>();
            try
            {
                List<KskSytCatalogItem> items = ReadCachedCatalog(SYT_CODE__PHAN_LOAI);
                if (items == null || items.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: chua co danh muc "
                        + SYT_CODE__PHAN_LOAI + " tai may -> khong quy doi duoc phan loai suc khoe. "
                        + "Mo man hinh nhap KSK mot lan de tai danh muc ve.");
                    return map;
                }

                foreach (KskSytCatalogItem it in items)
                {
                    if (it == null) continue;
                    int level = ParseSytRankLevel(it.Name);
                    long id;
                    if (level >= 1 && level <= 5 && long.TryParse(it.Id, out id) && !map.ContainsKey(level))
                        map[level] = id;
                }

                if (map.Count < 5)
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: bang quy doi phan loai suc khoe CHI CO "
                        + map.Count + "/5 cap do -> ho so co phan loai thieu se gui trong. "
                        + "Kiem tra ten muc trong danh muc " + SYT_CODE__PHAN_LOAI);
                else
                    Inventec.Common.Logging.LogSystem.Info("SytHcm: bang quy doi phan loai suc khoe du 5/5 cap do");
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return map;
        }

        #endregion

        #region ===== Đọc danh mục đã lưu tại máy =====

        /// <summary>
        /// Thư mục lưu danh mục của cổng — dùng chung với màn hình nhập KSK, nơi tải danh mục về.
        /// Ở đây CHỈ ĐỌC, không gọi mạng: chức năng đẩy chỉ lấy dữ liệu từ cơ sở dữ liệu và
        /// danh mục đã có sẵn.
        /// </summary>
        private static string GetCatalogDir()
        {
            // Thư mục chạy ứng dụng, cùng chỗ với Logs — PHẢI khớp nơi màn hình nhập KSK ghi ra.
            try
            {
                string dir = Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    "SytHcmCatalog");
                if (Directory.Exists(dir)) return dir;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

            // Máy không cho ghi vào thư mục ứng dụng -> màn hình nhập đã lùi về thư mục riêng của
            // người dùng Windows, đọc theo đó.
            return Path.Combine(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "HIS"), Path.Combine("KskSytHcm", "Catalog"));
        }

        private static readonly Dictionary<string, List<KskSytCatalogItem>> catalogCache
            = new Dictionary<string, List<KskSytCatalogItem>>();

        internal static List<KskSytCatalogItem> ReadCachedCatalog(string code)
        {
            try
            {
                List<KskSytCatalogItem> cached;
                if (catalogCache.TryGetValue(code, out cached)) return cached;

                string file = Path.Combine(GetCatalogDir(), code + ".json");
                List<KskSytCatalogItem> items = null;
                if (File.Exists(file))
                {
                    string json = File.ReadAllText(file, System.Text.Encoding.UTF8);
                    items = JsonConvert.DeserializeObject<List<KskSytCatalogItem>>(json);
                }
                catalogCache[code] = items;
                return items;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        #endregion

        #region ===== Đọc cột theo tên =====

        private static readonly Dictionary<string, PropertyInfo> propCache
            = new Dictionary<string, PropertyInfo>();

        private static PropertyInfo GetProp(object o, string column)
        {
            if (o == null) return null;
            string key = o.GetType().Name + "." + column;
            PropertyInfo pi;
            if (propCache.TryGetValue(key, out pi)) return pi;
            pi = o.GetType().GetProperty(column);
            propCache[key] = pi;
            if (pi == null)
                Inventec.Common.Logging.LogSystem.Warn("SytHcm: " + o.GetType().Name
                    + " KHONG co cot '" + column + "' -> gui trong, can sua lai bang anh xa cot");
            return pi;
        }

        private static string GetStr(object o, string column)
        {
            PropertyInfo pi = GetProp(o, column);
            return (pi != null) ? pi.GetValue(o, null) as string : null;
        }

        private static short? GetShort(object o, string column)
        {
            PropertyInfo pi = GetProp(o, column);
            return (pi != null) ? pi.GetValue(o, null) as short? : null;
        }

        private static long? GetLong(object o, string column)
        {
            PropertyInfo pi = GetProp(o, column);
            return (pi != null) ? pi.GetValue(o, null) as long? : null;
        }

        #endregion
    }
}
