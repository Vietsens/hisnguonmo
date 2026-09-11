/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Một dòng của tệp Excel "Nhập khẩu điều kiện lọc" ở màn Đồng bộ khám sức khỏe.
 *
 * CHỈ MỘT CỘT: thư viện đọc Excel ghép CỘT THEO THỨ TỰ THUỘC TÍNH của lớp này, nên lớp có đúng một
 * thuộc tính thì tệp cũng chỉ cần đúng một cột — cột A là mã điều trị, mỗi dòng một mã.
 *
 * Khác với màn Xuất XML QĐ 130: bên đó lọc theo nhiều tiêu chí (ngày vào, ngày ra, mã thẻ, mã bệnh
 * nhân...) nên lớp nhập khẩu của nó có 7 cột. Ở đây người yêu cầu chốt CHỈ lọc theo mã điều trị.
 */
namespace HIS.Desktop.Plugins.KskSyncList.ADO
{
    public class KskTreatmentCodeImportADO
    {
        /// <summary>Mã điều trị đọc từ cột A của tệp Excel.</summary>
        public string TREATMENT_CODE { get; set; }
    }
}
