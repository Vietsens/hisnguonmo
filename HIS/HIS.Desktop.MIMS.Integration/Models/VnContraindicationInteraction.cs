using System;

namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Chi tiết một cặp tương tác trong kết quả VN Contraindication Alert (CAP_TUONG_TAC)
    /// </summary>
    public class VnContraindicationInteraction
    {
        /// <summary>
        /// Tên cặp tương tác, ví dụ: "[Aspirin] - [Ketorolac]" (CapTuongTac)
        /// </summary>
        public string PairName { get; set; }

        /// <summary>
        /// Hoạt chất/thành phẩm thứ nhất (HoatChat_1)
        /// </summary>
        public string Drug1 { get; set; }

        /// <summary>
        /// Hoạt chất/thành phẩm thứ hai (HoatChat_2)
        /// </summary>
        public string Drug2 { get; set; }

        /// <summary>
        /// Mức độ nghiêm trọng, ví dụ: "Chống chỉ định", "Chống chỉ định có điều kiện" (MucDoNghiemTrong)
        /// </summary>
        public string InteractionLevel { get; set; }

        /// <summary>
        /// Hậu quả của tương tác (HauQuaCuaTuongTac)
        /// </summary>
        public string ClinicalConsequence { get; set; }

        /// <summary>
        /// Cơ chế tương tác (CoCheTuongTac)
        /// </summary>
        public string Mechanism { get; set; }

        /// <summary>
        /// Cách xử trí tương tác (XuTriTuongTac)
        /// </summary>
        public string Management { get; set; }

        /// <summary>
        /// Tài liệu tham khảo (TaiLieuThamKhao)
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Tuyên bố miễn trừ trách nhiệm (TuyenBoMienTruTrachNhiem)
        /// </summary>
        public string Disclaimer { get; set; }
    }
}
