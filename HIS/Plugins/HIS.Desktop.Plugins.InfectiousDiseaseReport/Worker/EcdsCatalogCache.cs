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
    }
}
