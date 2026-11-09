using Inventec.Common.IntegrationError;
using System;

namespace Inventec.Common.BankQrCode
{
    /// <summary>
    /// (vCong 62522) Dựng thông báo lỗi tạo mã QR ngân hàng có phân loại nguồn lỗi
    /// (cấu hình tại viện / dữ liệu / lỗi code HIS) thay cho "Run Exception: ...".
    /// </summary>
    internal static class QrCodeErrorUtil
    {
        internal static IntegrationPartner Partner(string bankName)
        {
            return IntegrationPartner.Bank(string.IsNullOrWhiteSpace(bankName) ? "tạo mã QR" : bankName);
        }

        /// <summary>Cấu hình hệ thống (chuỗi JSON thông tin merchant) rỗng hoặc sai định dạng.</summary>
        internal static string ConfigFormatError(string bankName, string detail)
        {
            return IntegrationErrorMessage.Build(ErrorSource.Config, Partner(bankName),
                "Cấu hình hệ thống tạo mã QR " + IntegrationErrorMessage.GetName(Partner(bankName)) + " rỗng hoặc sai định dạng (chuỗi JSON thông tin merchant do ngân hàng cấp)", detail);
        }

        /// <summary>Lỗi phát sinh khi tạo chuỗi QR (tạo tại máy trạm, không gọi ngân hàng).</summary>
        internal static string RunError(string bankName, Exception ex)
        {
            if (ex != null && IsJsonError(ex))
            {
                return ConfigFormatError(bankName, ex.Message);
            }
            return IntegrationErrorMessage.FromException(ex, Partner(bankName), "tạo mã QR");
        }

        private static bool IsJsonError(Exception ex)
        {
            for (Type t = ex.GetType(); t != null; t = t.BaseType)
            {
                if (t.FullName == "Newtonsoft.Json.JsonException") return true;
            }
            return false;
        }
    }
}
