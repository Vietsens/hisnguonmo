using System;

namespace Inventec.Common.IntegrationError
{
    /// <summary>
    /// Mô tả hệ thống đối tác đang gọi: tên hiển thị + loại (để chọn câu liên hệ).
    /// </summary>
    public class IntegrationPartner
    {
        /// <summary>Tên hiển thị, VD "Viettel-CA", "cổng giám định BHXH", "eCDS".</summary>
        public string Name { get; private set; }

        /// <summary>Loại đối tác.</summary>
        public PartnerKind Kind { get; private set; }

        /// <summary>Câu liên hệ riêng (tùy chọn) thay cho câu mặc định theo loại khi lỗi thuộc về đối tác.</summary>
        public string Contact { get; private set; }

        public IntegrationPartner(PartnerKind kind, string name)
            : this(kind, name, null)
        {
        }

        public IntegrationPartner(PartnerKind kind, string name, string contact)
        {
            this.Kind = kind;
            this.Name = name;
            this.Contact = contact;
        }

        public override string ToString()
        {
            return String.IsNullOrWhiteSpace(this.Name) ? this.Kind.ToString() : this.Name;
        }

        #region Đối tác hay dùng

        public static IntegrationPartner Bhxh
        {
            get { return new IntegrationPartner(PartnerKind.Bhxh, "cổng giám định BHXH"); }
        }

        public static IntegrationPartner Sign(string caName)
        {
            return new IntegrationPartner(PartnerKind.DigitalSign, String.IsNullOrWhiteSpace(caName) ? "dịch vụ ký số" : caName);
        }

        public static IntegrationPartner Bank(string bankName)
        {
            return new IntegrationPartner(PartnerKind.Bank, String.IsNullOrWhiteSpace(bankName) ? "ngân hàng" : bankName);
        }

        public static IntegrationPartner EInvoice(string providerName)
        {
            return new IntegrationPartner(PartnerKind.EInvoice, providerName);
        }

        public static IntegrationPartner HealthPortal(string portalName)
        {
            return new IntegrationPartner(PartnerKind.HealthPortal, String.IsNullOrWhiteSpace(portalName) ? "cổng dữ liệu y tế" : portalName);
        }

        public static IntegrationPartner Pharmacy
        {
            get { return new IntegrationPartner(PartnerKind.Pharmacy, "cổng liên thông đơn thuốc quốc gia"); }
        }

        public static IntegrationPartner Lis(string name)
        {
            return new IntegrationPartner(PartnerKind.Lis, String.IsNullOrWhiteSpace(name) ? "hệ thống LIS" : name);
        }

        public static IntegrationPartner Pacs(string name)
        {
            return new IntegrationPartner(PartnerKind.Pacs, String.IsNullOrWhiteSpace(name) ? "hệ thống PACS" : name);
        }

        public static IntegrationPartner Device(string name)
        {
            return new IntegrationPartner(PartnerKind.Device, String.IsNullOrWhiteSpace(name) ? "thiết bị" : name);
        }

        public static IntegrationPartner Ai(string name)
        {
            return new IntegrationPartner(PartnerKind.AiService, String.IsNullOrWhiteSpace(name) ? "dịch vụ AI" : name);
        }

        public static IntegrationPartner FileStorage
        {
            get { return new IntegrationPartner(PartnerKind.FileStorage, "máy chủ lưu trữ file (FSS)"); }
        }

        public static IntegrationPartner LocalService(string name)
        {
            return new IntegrationPartner(PartnerKind.LocalService, String.IsNullOrWhiteSpace(name) ? "dịch vụ trên máy trạm" : name);
        }

        public static IntegrationPartner Emr(string name)
        {
            return new IntegrationPartner(PartnerKind.Emr, String.IsNullOrWhiteSpace(name) ? "hệ thống bệnh án điện tử" : name);
        }

        public static IntegrationPartner Other(string name)
        {
            return new IntegrationPartner(PartnerKind.Other, name);
        }

        #endregion
    }
}
