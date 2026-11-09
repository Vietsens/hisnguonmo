using Inventec.Common.IntegrationError;
using System;

namespace HIS.Desktop.Plugins.RegisterExamKiosk
{
    /// <summary>
    /// vCong 62522: lỗi tích hợp ở màn kiosk (bệnh nhân tự thao tác): câu hiện cho bệnh nhân ngắn gọn kèm nhãn nguồn lỗi
    /// để nhân viên biết báo ai; thông báo phân loại đầy đủ (liên hệ + chi tiết kỹ thuật) ghi vào log.
    /// </summary>
    internal static class KioskIntegrationError
    {
        /// <summary>Dịch vụ xác thực sinh trắc học chạy trên máy kiosk (http://localhost:7000).</summary>
        internal static IntegrationPartner VerifyServicePartner
        {
            get { return IntegrationPartner.LocalService("dịch vụ xác thực sinh trắc học trên máy trạm"); }
        }

        /// <summary>Dịch vụ nhận diện khuôn mặt VVN (cấu hình FaceRegAddressVvn).</summary>
        internal static IntegrationPartner FaceRegPartner
        {
            get { return IntegrationPartner.Ai("dịch vụ nhận diện khuôn mặt VVN"); }
        }

        /// <summary>Ghi log thông báo đầy đủ, trả về câu ngắn cho bệnh nhân kèm nhãn nguồn lỗi.</summary>
        internal static string ForPatient(string shortText, ErrorSource source, IntegrationPartner partner, string fullMessage)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("[Kiosk] " + fullMessage);
                return shortText + Environment.NewLine + "(" + IntegrationErrorMessage.GetLabel(source, partner != null ? partner.Kind : PartnerKind.Other)
                    + " - vui lòng báo nhân viên hỗ trợ)";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return shortText;
            }
        }

        /// <summary>Từ exception: tự phân loại nguồn lỗi.</summary>
        internal static string ForPatient(string shortText, Exception ex, IntegrationPartner partner, string action)
        {
            return ForPatient(shortText, IntegrationErrorClassifier.Classify(ex), partner, IntegrationErrorMessage.FromException(ex, partner, action));
        }
    }
}
