using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.InviteConsultation.ADO
{
    /// <summary>
    /// Frontend-only ADO mở rộng HIS_SPECIALIST_EXAM với 2 field bổ sung
    /// (IsAutoCreateTracking + MedicalInstruction) theo PTTK_38078 B.4.1.
    ///
    /// Lý do dùng INHERIT thay vì WRAP:
    ///   - Backend API /api/HisSpecialistExam/Create deserialize root JSON vào HIS_SPECIALIST_EXAM
    ///   - Nếu wrap (root.HisSpecialistExam = {...}) → backend nhận root.HisSpecialistExam là null → MOS013 error
    ///   - Khi inherit → JSON root level = HIS_SPECIALIST_EXAM fields + 2 field phụ → backend deserialize bình thường;
    ///     2 field phụ bị ignore nếu backend chưa hỗ trợ, được xử lý nếu backend đã update (B.3.1)
    ///
    /// IsAutoCreateTracking = 1 ở bản ghi đầu tiên → backend tự sinh tờ điều trị A
    /// MedicalInstruction = chuỗi "Mời hội chẩn lúc {HH:mm dd/MM/yyyy} Khoa phòng mời hội chẩn: {Khoa A, Khoa B, ...}"
    ///   → backend gán vào HIS_TRACKING.MEDICAL_INSTRUCTION của tờ điều trị A
    /// </summary>
    public class HisSpecialistExamCreateAutoTrackingFeSDO : HIS_SPECIALIST_EXAM
    {
        public short IsAutoCreateTracking { get; set; }

        public string MedicalInstruction { get; set; }
    }
}
