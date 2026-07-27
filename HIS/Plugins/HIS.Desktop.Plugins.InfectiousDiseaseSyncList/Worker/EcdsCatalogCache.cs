/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Cache danh mục ECDS trong RAM theo phiên. Nạp 1 lần, dùng nhiều — TRÁNH gọi API trong vòng lặp batch.
 * Cascade cache theo khóa cha (xã theo tỉnh, thôn theo xã, cấp độ bệnh theo bệnh).
 */
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Worker
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

        /// <summary>Tra ID ECDS theo mã (đối chiếu HIS -> ECDS). Trả null nếu không thấy.</summary>
        internal long? FindIdByMa(List<DanhMucItemDto> list, string maHis)
        {
            try
            {
                if (list == null || string.IsNullOrEmpty(maHis)) return null;
                foreach (var item in list)
                {
                    if (string.Equals(item.ma, maHis, StringComparison.OrdinalIgnoreCase))
                        return item.id;
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
