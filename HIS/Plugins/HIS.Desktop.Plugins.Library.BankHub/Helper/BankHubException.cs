using Inventec.Common.IntegrationError;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.BankHub.Helper
{
    /// <summary>
    /// Exception đặc thù cho BankHub Client.
    /// (vCong 62522) Kế thừa IntegrationException để nơi hiển thị biết lỗi do kết nối / ngân hàng / thiết lập tại viện.
    /// Vẫn giữ 2 constructor cũ (nguồn lỗi tự đoán theo nội dung) để tương thích.
    /// </summary>
    public class BankHubException : IntegrationException
    {
        /// <summary>Đối tác mặc định khi không truyền tên ngân hàng</summary>
        public static IntegrationPartner DefaultPartner
        {
            get { return IntegrationPartner.Bank("BankHub"); }
        }

        public BankHubException(string message)
            : base(IntegrationErrorClassifier.ClassifyByMessage(message), DefaultPartner, message) { }

        public BankHubException(string message, Exception inner)
            : base(inner != null ? IntegrationErrorClassifier.Classify(inner) : IntegrationErrorClassifier.ClassifyByMessage(message), DefaultPartner, message, inner) { }

        public BankHubException(ErrorSource source, IntegrationPartner partner, string message)
            : base(source, partner ?? DefaultPartner, message) { }

        public BankHubException(ErrorSource source, IntegrationPartner partner, string message, Exception inner)
            : base(source, partner ?? DefaultPartner, message, inner) { }

        /// <summary>Chuyển exception đã phân loại (IntegrationHttpError.WrapCall/FromStatus...) thành BankHubException, giữ nguồn lỗi.</summary>
        public static BankHubException From(Exception classified, IntegrationPartner partner, Exception original)
        {
            BankHubException bankHubEx = classified as BankHubException;
            if (bankHubEx != null) return bankHubEx;
            IntegrationException known = classified as IntegrationException;
            if (known != null)
                return new BankHubException(known.ErrorSource, partner ?? known.Partner, known.Message, original ?? known.InnerException);
            Exception ex = classified ?? original;
            return new BankHubException(IntegrationErrorClassifier.Classify(ex), partner, ex != null ? ex.Message : "", ex);
        }
    }
}
