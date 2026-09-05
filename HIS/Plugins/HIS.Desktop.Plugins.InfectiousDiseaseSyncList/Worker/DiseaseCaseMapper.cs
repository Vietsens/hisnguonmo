/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Tiện ích ánh xạ dữ liệu HIS -> ECDS.
 * (Form tự đọc control + gọi các helper convert; lớp này giữ hàm dùng chung.)
 */
using System;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Worker
{
    internal class DiseaseCaseMapper
    {
        private readonly EcdsCatalogCache catalog;

        internal DiseaseCaseMapper(EcdsCatalogCache catalogCache)
        {
            this.catalog = catalogCache;
        }

        /// <summary>long yyyyMMddHHmmss -> "dd/MM/yyyy" (định dạng ngày cổng ECDS). Null nếu không hợp lệ.</summary>
        internal static string ToPortalDate(long? timeNumber)
        {
            try
            {
                if (!timeNumber.HasValue || timeNumber.Value <= 0) return null;
                DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber.Value);
                return dt.HasValue ? dt.Value.ToString("dd/MM/yyyy") : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>long yyyyMMddHHmmss -> "yyyy-MM-dd" (null nếu không hợp lệ).</summary>
        internal static string ToIsoDate(long? timeNumber)
        {
            try
            {
                if (!timeNumber.HasValue || timeNumber.Value <= 0) return null;
                DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber.Value);
                return dt.HasValue ? dt.Value.ToString("yyyy-MM-dd") : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Tra ID danh mục ECDS theo mã HIS (đối chiếu qua cache).</summary>
        internal long? MapCodeToEcdsId(string tenDanhMuc, string maHis)
        {
            try
            {
                if (string.IsNullOrEmpty(maHis)) return null;
                return catalog.FindIdByMa(catalog.GetStatic(tenDanhMuc), maHis);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}
