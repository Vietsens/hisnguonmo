using System;

namespace Inventec.Common.IntegrationError
{
    /// <summary>
    /// Exception đã biết rõ nguồn lỗi. Các lớp gọi API ném exception này để lớp trên không phải đoán lại từ ex.Message.
    /// </summary>
    [Serializable]
    public class IntegrationException : Exception
    {
        /// <summary>Nguồn gây lỗi.</summary>
        public ErrorSource ErrorSource { get; private set; }

        /// <summary>Đối tác đang gọi (có thể null).</summary>
        public IntegrationPartner Partner { get; private set; }

        /// <summary>Mã HTTP (0 nếu không áp dụng).</summary>
        public int StatusCode { get; private set; }

        public IntegrationException(ErrorSource source, IntegrationPartner partner, string message)
            : this(source, partner, message, null)
        {
        }

        public IntegrationException(ErrorSource source, IntegrationPartner partner, string message, Exception inner)
            : base(message, inner)
        {
            this.ErrorSource = source;
            this.Partner = partner;
        }

        public IntegrationException(ErrorSource source, IntegrationPartner partner, int statusCode, string message)
            : base(message)
        {
            this.ErrorSource = source;
            this.Partner = partner;
            this.StatusCode = statusCode;
        }

        protected IntegrationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
