/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Cache danh mục ECDS trong RAM theo phiên. Nạp 1 lần, dùng nhiều — TRÁNH gọi API trong vòng lặp batch.
 * Cascade cache theo khóa cha (xã theo tỉnh, thôn theo xã, cấp độ bệnh theo bệnh).
 */
using HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.Worker
{
    internal class EcdsCatalogCache
    {
        // Tên danh mục ECDS
        internal const string DM_TINH = "tinh";
        internal const string DM_XA = "xa";
        internal const string DM_THON = "thon";
        internal const string DM_BENH = "benh";
        internal const string DM_DANTOC = "dan-toc";
        internal const string DM_NGHENGHIEP = "nghe-nghiep";
        internal const string DM_COSO = "don-vi";
        internal const string DM_CAPDOBENH = "phan-loai-lam-sang"; // cấp độ/phân loại theo bệnh

        private readonly EcdsApiWorker api;

        // danh mục tĩnh (cache theo tên)
        private readonly Dictionary<string, List<DanhMucItemDto>> _staticCache
            = new Dictionary<string, List<DanhMucItemDto>>();
        // danh mục phân cấp (cache theo tên + khóa cha)
        private readonly Dictionary<string, List<DanhMucItemDto>> _cascadeCache
            = new Dictionary<string, List<DanhMucItemDto>>();

        internal EcdsCatalogCache(EcdsApiWorker apiWorker)
        {
            this.api = apiWorker;
        }

        /// <summary>Danh mục tĩnh (tỉnh, dân tộc, nghề nghiệp, cơ sở, bệnh...).</summary>
        internal List<DanhMucItemDto> GetStatic(string tenDanhMuc)
        {
            try
            {
                List<DanhMucItemDto> list;
                if (_staticCache.TryGetValue(tenDanhMuc, out list) && list != null)
                    return list;

                list = api.LayDanhMuc(tenDanhMuc, new SearchDanhMucFastDto());
                _staticCache[tenDanhMuc] = list;
                return list;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<DanhMucItemDto>();
            }
        }

        /// <summary>Danh mục phân cấp theo khóa cha (VD xã theo maTinh, thôn theo maXa, cấp độ theo maIcd10Benh).</summary>
        internal List<DanhMucItemDto> GetCascade(string tenDanhMuc, SearchDanhMucFastDto filter, string cacheKey)
        {
            try
            {
                string key = tenDanhMuc + "|" + (cacheKey ?? "");
                List<DanhMucItemDto> list;
                if (_cascadeCache.TryGetValue(key, out list) && list != null)
                    return list;

                list = api.LayDanhMuc(tenDanhMuc, filter);
                _cascadeCache[key] = list;
                return list;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<DanhMucItemDto>();
            }
        }

        private static readonly char[] MaSeparators = new[] { ',', ';', ' ', '/', '|' };

        /// <summary>
        /// Khớp 2 mã ICD theo PHÂN CẤP (không phân biệt hoa/thường):
        /// - bằng nhau; HOẶC
        /// - key là mã con của token (VD hồ sơ "A15.0" thuộc category cổng "A15"); HOẶC
        /// - token là mã con của key.
        /// Dùng cho danh mục bệnh cổng gộp mã ở cấp category ("A15, A16").
        /// </summary>
        internal static bool IcdMatch(string token, string key)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(key)) return false;
            string a = token.Trim(), b = key.Trim();
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return true;
            if (b.StartsWith(a + ".", StringComparison.OrdinalIgnoreCase)) return true;   // A15.0 thuộc A15
            if (a.StartsWith(b + ".", StringComparison.OrdinalIgnoreCase)) return true;   // ngược lại
            return false;
        }

        /// <summary>
        /// Danh mục bệnh cổng gộp nhiều mã ("A15, A16"). Chọn TOKEN khớp nhất với mã ICD hồ sơ
        /// để gửi 1 mã ICD-10 cổng biết (tránh gửi cả chuỗi gộp làm cổng từ chối):
        /// ưu tiên token TRÙNG chính xác, rồi token là CHA của mã hồ sơ; không khớp -> token đầu.
        /// </summary>
        internal static string ResolveDiseaseIcdToken(string groupMa, string patientIcd)
        {
            if (string.IsNullOrEmpty(groupMa)) return null;
            var toks = groupMa.Split(MaSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (toks.Length == 0) return groupMa.Trim();
            if (!string.IsNullOrEmpty(patientIcd))
            {
                string key = patientIcd.Trim();
                foreach (var t in toks)
                    if (string.Equals(t.Trim(), key, StringComparison.OrdinalIgnoreCase)) return t.Trim();
                foreach (var t in toks)
                    if (IcdMatch(t, key)) return t.Trim();
            }
            return toks[0].Trim();
        }

        private List<DanhMucItemDto> _benhExpanded;

        /// <summary>
        /// Danh mục bệnh cổng, TÁCH mỗi item gộp nhiều mã ("A33, A34, A35" = Uốn ván) thành
        /// nhiều dòng riêng (A33/A34/A35 cùng tên "Uốn ván", cùng id cổng). Dùng bind combo bệnh
        /// với ValueMember = "ma" để người dùng chọn đúng 1 mã ICD.
        /// </summary>
        internal List<DanhMucItemDto> GetBenhExpanded()
        {
            try
            {
                if (_benhExpanded != null) return _benhExpanded;
                var src = GetStatic(DM_BENH);
                var outList = new List<DanhMucItemDto>();
                if (src != null)
                {
                    foreach (var it in src)
                    {
                        if (it == null) continue;
                        if (string.IsNullOrEmpty(it.ma)) { outList.Add(it); continue; }
                        var toks = it.ma.Split(MaSeparators, StringSplitOptions.RemoveEmptyEntries);
                        if (toks.Length <= 1) { outList.Add(it); continue; }
                        foreach (var tok in toks)
                            outList.Add(new DanhMucItemDto { id = it.id, ma = tok.Trim(), ten = it.ten });
                    }
                }
                _benhExpanded = outList;
                return _benhExpanded;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return GetStatic(DM_BENH);
            }
        }

        /// <summary>Tìm 1 mã (token) trong danh mục bệnh đã tách khớp ICD hồ sơ (ưu tiên trùng, rồi phân cấp). Null nếu không thấy.</summary>
        internal string FindBenhTokenByIcd(string patientIcd)
        {
            try
            {
                if (string.IsNullOrEmpty(patientIcd)) return null;
                var list = GetBenhExpanded();
                if (list == null) return null;
                string key = patientIcd.Trim();
                foreach (var it in list)
                    if (it != null && string.Equals(it.ma, key, StringComparison.OrdinalIgnoreCase)) return it.ma;
                foreach (var it in list)
                    if (it != null && IcdMatch(it.ma, key)) return it.ma;
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Tra ID cổng của bệnh theo mã (token) đang chọn trên combo đã tách. Null nếu không thấy.</summary>
        internal long? FindBenhIdByToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return null;
                var list = GetBenhExpanded();
                if (list == null) return null;
                string key = token.Trim();
                foreach (var it in list)
                    if (it != null && string.Equals(it.ma, key, StringComparison.OrdinalIgnoreCase)) return it.id;
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Tra ID ECDS theo mã (đối chiếu HIS -> ECDS). Trả null nếu không thấy.
        /// Hỗ trợ item.ma dạng DANH SÁCH nhiều mã (VD danh mục bệnh cổng: "A00, A00.0, A00.1, A00.9"):
        /// khớp nếu maHis TRÙNG cả chuỗi HOẶC là 1 token trong danh sách.
        /// </summary>
        internal long? FindIdByMa(List<DanhMucItemDto> list, string maHis)
        {
            try
            {
                if (list == null || string.IsNullOrEmpty(maHis)) return null;
                string key = maHis.Trim();
                foreach (var item in list)
                {
                    if (string.IsNullOrEmpty(item.ma)) continue;
                    if (string.Equals(item.ma.Trim(), key, StringComparison.OrdinalIgnoreCase))
                        return item.id;                                   // trùng cả chuỗi
                    var toks = item.ma.Split(MaSeparators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var tok in toks)
                        if (IcdMatch(tok, key))
                            return item.id;                               // 1 token khớp (kể cả mã con/category)
                }
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Tra MÃ cổng theo ID danh mục cổng (dùng khi combo giữ ValueMember = id). Null nếu không thấy.</summary>
        internal string FindMaById(List<DanhMucItemDto> list, long? id)
        {
            try
            {
                if (list == null || !id.HasValue) return null;
                var item = list.Find(o => o != null && o.id == id.Value);
                return item != null ? item.ma : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Đối chiếu MÃ nội bộ (HIS/SDA) -> MÃ cổng tương ứng trong danh mục.
        /// Khớp theo cả chuỗi hoặc 1 token (item.ma có thể là danh sách nhiều mã).
        /// KHÔNG thấy -> trả lại maHis (giả định mã trùng chuẩn quốc gia).
        /// </summary>
        internal string FindMaByMa(List<DanhMucItemDto> list, string maHis)
        {
            try
            {
                if (string.IsNullOrEmpty(maHis) || list == null) return null;
                string key = maHis.Trim();
                long keyNum; bool keyIsNum = long.TryParse(key, out keyNum);

                foreach (var item in list)
                {
                    if (item == null || string.IsNullOrEmpty(item.ma)) continue;
                    string ma = item.ma.Trim();
                    if (string.Equals(ma, key, StringComparison.OrdinalIgnoreCase))
                        return item.ma;                                   // trùng cả chuỗi
                    // Khớp theo SỐ (bỏ số 0 ở đầu): HIS "01" == cổng "1".
                    long maNum;
                    if (keyIsNum && long.TryParse(ma, out maNum) && maNum == keyNum)
                        return item.ma;
                    // item.ma có thể là danh sách nhiều mã.
                    var toks = item.ma.Split(MaSeparators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var tok in toks)
                    {
                        string t = tok.Trim();
                        if (string.Equals(t, key, StringComparison.OrdinalIgnoreCase))
                            return item.ma;
                        long tNum;
                        if (keyIsNum && long.TryParse(t, out tNum) && tNum == keyNum)
                            return item.ma;
                    }
                }
                return null;   // KHÔNG đối chiếu được -> caller bỏ trường (optional), tránh gửi mã sai làm cổng từ chối
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}
