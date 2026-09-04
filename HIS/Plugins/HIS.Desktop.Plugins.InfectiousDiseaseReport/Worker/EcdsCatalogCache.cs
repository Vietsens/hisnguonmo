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
                        if (string.Equals(tok.Trim(), key, StringComparison.OrdinalIgnoreCase))
                            return item.id;                               // là 1 token trong danh sách mã
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
