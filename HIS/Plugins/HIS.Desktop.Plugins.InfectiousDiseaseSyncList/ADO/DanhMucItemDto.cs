/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * 1 phần tử danh mục ECDS trả về từ /api/fast/v1/danh-muc/*.
 * Lưu ý: ID danh mục ECDS là số nội bộ (VD TINH_ID=709, XA_ID=127976).
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO
{
    public class DanhMucItemDto
    {
        /// <summary>ID nội bộ ECDS (dùng để đẩy lên cổng).</summary>
        public long id { get; set; }

        /// <summary>Mã (mã GSO / ICD / mã liên thông — dùng để đối chiếu với HIS).</summary>
        public string ma { get; set; }

        /// <summary>Tên hiển thị.</summary>
        public string ten { get; set; }

        /// <summary>Mã cha (nếu là danh mục phân cấp: xã theo tỉnh, thôn theo xã...).</summary>
        public string maCha { get; set; }
    }
}
