/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
namespace HIS.Desktop.Plugins.HisPatientPackage
{
    /// <summary>
    /// Định danh (ModuleLink) các plugin được mở từ màn Danh sách gói.
    /// KHÔNG hardcode string trực tiếp trong code.
    /// </summary>
    internal class ModuleLinkString
    {
        /// <summary>Module link của chính plugin này (màn 6.2 Danh sách gói).</summary>
        internal const string HisPatientPackage = "HIS.Desktop.Plugins.HisPatientPackage";

        /// <summary>Màn 6.1 Đăng ký/Sửa gói — plugin RIÊNG. Nút "Sửa" mở (truyền BN + gói).</summary>
        internal const string PatientPackageRegister = "HIS.Desktop.Plugins.PatientPackageRegister";

        /// <summary>Thanh toán khác — nút "Thanh toán" mở (truyền BN + gói).</summary>
        internal const string TransactionBillOther = "HIS.Desktop.Plugins.TransactionBillOther";

        /// <summary>Hoàn ứng dịch vụ — nút "Hoàn tiền" mở (truyền BN + gói).</summary>
        internal const string TransactionRepay = "HIS.Desktop.Plugins.TransactionRepay";
    }
}
