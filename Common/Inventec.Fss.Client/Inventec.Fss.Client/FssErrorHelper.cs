using System;
using System.Net.Http;

namespace Inventec.Fss.Client
{
    /// <summary>
    /// vCong 62522: dựng exception/thông báo mang nguồn lỗi cho thư viện FSS (kết nối máy chủ lưu trữ / FSS trả lỗi / địa chỉ cấu hình sai).
    /// Không đổi chữ ký public của FileUpload/FileDownload/FileDelete: vẫn ném FileUploadException/FileDownloadException/FileDeleteException,
    /// nhưng Message là thông báo đã phân loại và InnerException là IntegrationException.
    /// </summary>
    public static class FssErrorHelper
    {
        [ThreadStatic]
        private static string lastErrorMessage;

        /// <summary>Thông báo lỗi (đã phân loại nguồn) của thao tác FSS gần nhất trên luồng hiện tại; null nếu thành công.</summary>
        public static string LastErrorMessage
        {
            get { return lastErrorMessage; }
        }

        public static Inventec.Common.IntegrationError.IntegrationPartner Partner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.FileStorage; }
        }

        internal static void Reset()
        {
            lastErrorMessage = null;
        }

        /// <summary>Địa chỉ cấu hình FSS rỗng/sai định dạng → lỗi thiết lập tại viện.</summary>
        internal static Uri ParseBaseUri(string baseUri, string action)
        {
            Uri result;
            if (String.IsNullOrWhiteSpace(baseUri) || !Uri.TryCreate(baseUri, UriKind.Absolute, out result))
            {
                Inventec.Common.IntegrationError.IntegrationException ie = new Inventec.Common.IntegrationError.IntegrationException(
                    Inventec.Common.IntegrationError.ErrorSource.Config, Partner, "Địa chỉ máy chủ lưu trữ file (FSS) trong cấu hình chưa có hoặc sai định dạng: " + baseUri);
                throw ie;
            }
            return result;
        }

        /// <summary>Exception khi gọi (timeout, không kết nối, SSL...) → IntegrationException + thông báo đầy đủ.</summary>
        internal static Inventec.Common.IntegrationError.IntegrationException Wrap(Exception ex, string uri, string action, out string message)
        {
            Inventec.Common.IntegrationError.IntegrationException ie = Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(ex, Partner, uri) as Inventec.Common.IntegrationError.IntegrationException
                ?? new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Software, Partner, ex != null ? ex.Message : "", ex);
            message = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ie, Partner, action);
            lastErrorMessage = message;
            return ie;
        }

        /// <summary>HTTP không thành công → IntegrationException theo mã HTTP + thông báo đầy đủ.</summary>
        internal static Inventec.Common.IntegrationError.IntegrationException FromStatus(HttpResponseMessage resp, string uri, string action, out string message)
        {
            Inventec.Common.IntegrationError.IntegrationException ie = Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, uri, resp);
            message = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ie, Partner, action);
            lastErrorMessage = message;
            return ie;
        }

        internal static string Combine(string baseUri, string path)
        {
            return (baseUri ?? "").TrimEnd('/') + "/" + (path ?? "").TrimStart('/');
        }
    }
}
