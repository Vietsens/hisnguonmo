namespace Inventec.Common.IntegrationError
{
    /// <summary>
    /// Loại hệ thống đối tác. Quyết định câu "liên hệ ai" khi lỗi thuộc về đối tác hoặc kết nối.
    /// </summary>
    public enum PartnerKind
    {
        /// <summary>Dịch vụ bên ngoài khác.</summary>
        Other = 0,
        /// <summary>Nhà cung cấp hóa đơn điện tử.</summary>
        EInvoice = 1,
        /// <summary>Cổng giám định BHXH.</summary>
        Bhxh = 2,
        /// <summary>Nhà cung cấp chữ ký số (CA, HSM, ký từ xa).</summary>
        DigitalSign = 3,
        /// <summary>Ngân hàng, cổng thanh toán, QR, máy POS ngân hàng.</summary>
        Bank = 4,
        /// <summary>Cổng dữ liệu y tế của Sở/Bộ Y tế (KSK liên thông, HSSK, eCDS, kho dữ liệu y tế, HOC...).</summary>
        HealthPortal = 5,
        /// <summary>Cổng liên thông đơn thuốc / dược quốc gia.</summary>
        Pharmacy = 6,
        /// <summary>Hệ thống xét nghiệm LIS.</summary>
        Lis = 7,
        /// <summary>Hệ thống chẩn đoán hình ảnh PACS.</summary>
        Pacs = 8,
        /// <summary>Thiết bị kết nối với máy trạm (máy đo, đầu đọc thẻ, vân tay, máy in tem...).</summary>
        Device = 9,
        /// <summary>Dịch vụ AI / tra cứu dược lý bên ngoài.</summary>
        AiService = 10,
        /// <summary>Máy chủ lưu trữ file của viện (FSS).</summary>
        FileStorage = 11,
        /// <summary>Dịch vụ chạy trên chính máy trạm (localhost, WCF cục bộ).</summary>
        LocalService = 12,
        /// <summary>Hệ thống bệnh án điện tử bên ngoài (EMR Toolkit...).</summary>
        Emr = 13
    }
}
