namespace HIS.Desktop.Plugins.ApprovalExamSpecialist
{
    /// <summary>
    /// Phieu duoc ky so khi bam "Duyet va ky" (viec 57944).
    /// Mapping gia tri cau hinh HIS_CONFIG
    /// "HIS.Desktop.Plugins.ApprovalExamSpecialist.ApproveAndSignOption".
    /// Gia tri rong / khong hop le => TrackingOnly (giu nguyen hanh vi viec 56271).
    /// </summary>
    public enum EnumApproveAndSignOption
    {
        /// <summary>Chi ky to dieu tri (Mps000062) - mac dinh</summary>
        TrackingOnly = 1,

        /// <summary>Chi ky phieu ket qua kham chuyen khoa (Mps000500)</summary>
        ExamResultOnly = 2,

        /// <summary>Ky to dieu tri (Mps000062) roi ky tiep phieu ket qua kham chuyen khoa (Mps000500)</summary>
        Both = 3
    }
}
