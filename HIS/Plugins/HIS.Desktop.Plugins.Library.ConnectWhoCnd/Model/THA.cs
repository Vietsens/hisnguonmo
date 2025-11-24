using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ConnectWhoCnd.Model
{
    internal class THA : BENH_NHAN
    {
        public THA(HIS_TREATMENT data)
            :base(data)
        {
            HUT_THUOC = 2;
            RUOU_BIA = 2;
            GIAM_MUOI = 2;
            RAU_TRAI_CAY = 2;
            HOAT_DONG_THE_LUC = 2;
            CHAN_DOAN = null;
            KQDT = null;
            BIEN_CHUNG = null;
        }

        /// <summary>
        /// Ghi ngày người bệnh phát hiện tăng huyết áp gồm 8 ký tự theo định dạng DD/MM/YYYY
        /// </summary>
        public string NGAY_PHAT_HIEN { get; set; }
        /// <summary>
        /// Ghi nơi người bệnh được phát mắc tăng huyết áp theo mã:
        /// - Trạm Y tế: 1
        /// - Bệnh viện huyện: 2
        /// - Bệnh viện tỉnh: 3
        /// - Bệnh viện trung ương: 4
        /// - Bệnh viện tư nhân: 5
        /// - Khác: 6
        /// </summary>
        public string NOI_PHAT_HIEN { get; set; }
        /// <summary>
        /// Ghi ngày người bệnh đến khám, điều trị gồm 8 ký tự theo định dạng DD/MM/YYYY
        /// </summary>
        public string NGAY_KHAM { get; set; }
        /// <summary>
        /// Ghi phân loại người bệnh theo mã:
        /// - Lần đầu tiên đến khám và lấy thuốc: 0
        /// - Mới chuyển về (trước đây được QLĐT tại CSYT khác): 1
        /// - Bệnh nhân cũ: 2
        /// - Bỏ, chuyển: 3
        /// - Chết: 4
        /// - Bệnh nhân quản lý (không cấp thuốc): 5
        /// </summary>
        public int PHAN_LOAI_BN { get; set; }
        /// <summary>
        /// Ghi chỉ số huyết áp tâm thu của người bệnh
        /// Điều kiện: >70 và <300
        /// </summary>
        public string HA_TAM_THU { get; set; }
        /// <summary>
        /// Ghi chỉ số huyết áp tâm trương của người bệnh
        /// Điều kiện: >40 và <250
        /// </summary>
        public string HA_TAM_TRUONG { get; set; }
        /// <summary>
        /// Ghi số kilogram (kg) cân nặng của người bệnh, biểu thị đầy đủ cả số thập phân, dấu thập phân là dấu chấm “.”
        /// Điều kiện: >0 và <150
        /// </summary>
        public decimal? CAN_NANG { get; set; }
        /// <summary>
        /// Ghi chiều cao của người bệnh theo centimet (cm). Nếu có phần thập phân biểu thị bằng dấu chấm “.”
        /// Điều kiện: >30 và <200
        /// </summary>
        public decimal? CHIEU_CAO { get; set; }
        /// <summary>
        /// Ghi kích thước vòng eo của người bệnh theo centimet (cm).  Nếu có phần thập phân biểu thị bằng dấu chấm “.”
        /// Điều kiện: >10 và <150
        /// </summary>
        public string VONG_EO { get; set; }
        /// <summary>
        /// Ghi thông tin về hút thuốc lá theo mã:
        /// - Không: 0
        /// - Có: 1
        /// - Không có thông tin/chưa áp dụng: 2
        /// </summary>
        public int? HUT_THUOC { get; set; }
        /// <summary>
        /// Ghi thông tin về mức độ tiêu thụ rượu bia theo mã dưới:
        /// - Độ 1 - Nguy cơ thấp: 1
        /// - Độ 2 - Nguy cơ cao: 2
        /// - Độ 3 - Nguy cơ rất cao: 3
        /// - Độ 4 - Lệ thuộc rượu bia: 4
        /// </summary>
        public int? RUOU_BIA { get; set; }
        /// <summary>
        /// Ghi thông tin về thực hành giảm muối theo mã dưới:
        /// - Không: 0
        /// - Có: 1
        /// - Không có thông tin/chưa áp dụng: 2
        /// </summary>
        public int? GIAM_MUOI { get; set; }
        /// <summary>
        /// Ghi thông tin về ăn rau và trái cây theo mã dưới:
        /// - Không: 0
        /// - Có: 1
        /// - Không có thông tin/chưa áp dụng: 2
        /// </summary>
        public int? RAU_TRAI_CAY { get; set; }
        /// <summary>
        /// Ghi tình trạng hoạt động thể lực theo mã dưới:
        /// - Không: 0
        /// - Có: 1
        /// - Không có thông tin/chưa áp dụng: 2
        /// </summary>
        public int? HOAT_DONG_THE_LUC { get; set; }
        /// <summary>
        /// Ghi kết quả chẩn đoán theo mã dưới:
        /// - THA độ I: 1
        /// - THA độ II: 2
        /// - THA độ III: 3
        /// - THA được kiểm soát: 4
        /// - Khác/chi tiết: 5
        /// </summary>
        public int? CHAN_DOAN { get; set; }
        /// <summary>
        /// Ghi mỗi thuốc 1 dòng, ghi rõ tên thuốc, hàm lượng, số viên, số ngày.
        /// Ví dụ: Amlodipin 5mg x 28v/28 ngày
        /// </summary>
        public string THUOC { get; set; }
        /// <summary>
        /// Ghi biến chứng theo mã dưới:
        /// - Không: 1
        /// - Có: 2
        /// </summary>
        public int? BIEN_CHUNG { get; set; }
        /// <summary>
        /// Ghi kết quả điều trị theo mã dưới:
        /// 1. Bệnh tiến triển tốt. 
        /// 2. Bệnh không thay đổi. 
        /// 3. Bệnh nặng lên
        /// </summary>
        public int? KQDT { get; set; }
    }
}
