/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * 1 dòng của file Excel xuất/nhập mẫu khám sức khỏe (việc 57621).
 *
 * Luồng: bấm "Xuất mẫu" -> file Excel đã điền sẵn "Bình thường" / "Loại I" -> người dùng mở file
 * sửa những mục khác thường -> bấm "Nhập mẫu" -> điền trở lại lên form.
 *
 * KHÓA ĐỐI CHIẾU khi nhập lại là MA_O (tên control), KHÔNG phải MUC_KHAM:
 * nhãn mục có thể đổi giữa các bản phát hành, còn tên control thì không.
 * Cột MA_O vì thế được ghi ra file nhưng ẩn đi, người dùng không cần nhìn.
 */
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO
{
    /// <summary>1 mục khám trong file Excel mẫu.</summary>
    public class KskExcelRowADO
    {
        /// <summary>Tên mục hiển thị cho người dùng đọc. VD "a. Tuần hoàn".</summary>
        public string MUC_KHAM { get; set; }

        /// <summary>Kết quả khám, xuất ra đã điền sẵn "Bình thường". Người dùng sửa được.</summary>
        public string KET_QUA { get; set; }

        /// <summary>
        /// Tên phân loại, xuất ra đã điền sẵn "Loại I". Ghi TÊN chứ không ghi ID để người dùng đọc
        /// và sửa được; lúc nhập lại mới tra ngược ra ID trong danh mục HIS_HEALTH_EXAM_RANK.
        /// Mục không có ô Phân loại thì để trống.
        /// </summary>
        public string PHAN_LOAI { get; set; }

        /// <summary>Tên control ô Kết quả — khóa đối chiếu. Rỗng nếu mục này không có ô Kết quả.</summary>
        public string MA_O { get; set; }

        /// <summary>Tên control ô Phân loại — khóa đối chiếu. Rỗng nếu mục này không có ô Phân loại.</summary>
        public string MA_O_PHAN_LOAI { get; set; }

        /// <summary>
        /// Giá trị mặc định RIÊNG của ô Kết quả khi xuất file, dùng cho mục không phải
        /// "Bình thường": thị lực ("10"), thính lực ("5", "0,5").
        /// null = lấy giá trị chung "Bình thường"; chuỗi rỗng = xuất ra ô trống.
        /// CHỈ dùng lúc xuất, không ghi vào file.
        /// </summary>
        public string GIA_TRI_RIENG { get; set; }
    }
}
