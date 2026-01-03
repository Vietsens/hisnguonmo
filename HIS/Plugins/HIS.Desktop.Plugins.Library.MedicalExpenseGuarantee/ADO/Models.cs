using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee.ADO
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class ResponseStatus
    {
        public string Status { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
        public List<string> ErrInfo { get; set; }
    }

    public class StandardResponseData<T>
    {
        public ResponseStatus ResponseStatus { get; set; }
        public T Data { get; set; }
    }

    #region Base Request Models
    public class BaseGuaranteeRequest
    {
        public string RequestId { get; set; }
        public string Amount { get; set; }
        public string Remark { get; set; }
        public string ContractNumber { get; set; }
    }

    public class PatientInfoRequest : BaseGuaranteeRequest
    {
        public string PatientFullName { get; set; }
        public string PatientDateOfBirth { get; set; }
        public string PatientCccd { get; set; }
        public string Signature { get; set; }
        public string Token { get; set; }
    }
    #endregion

    #region RegisterUse API
    public class RegisterUseRequest
    {
        public string PatientFullName { get; set; }
        public string PatientDateOfBirth { get; set; }
        public string PatientCccd { get; set; }
        public string RequestAmount { get; set; }
        public string ApplicationCode { get; set; }
        public string Remark { get; set; }
        public string Signature { get; set; }
    }

    public class RegisterUseData
    {
        public bool IsValid { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string RequestId { get; set; }
        public string RegisteredAmount { get; set; }
        public string UsedAmount { get; set; }
        public string AvailableBalance { get; set; }
        public string ContractNumber { get; set; }
    }

    public class RegisterUseResponse : ApiResponse<RegisterUseData> { }
    #endregion

    #region Use API
    public class UseRequest : PatientInfoRequest { }

    public class TransactionData
    {
        public string TransId { get; set; }
        public string ContractNumber { get; set; }
        public string InitialBalance { get; set; }
        public string AvailableBalance { get; set; }
        public string RefNo { get; set; }
    }

    public class UseResponse : ApiResponse<StandardResponseData<TransactionData>> { }
    #endregion

    #region CancelRegisterUse API
    public class CancelRegisterUseRequest : PatientInfoRequest { }

    public class CancelData
    {
        public string TransId { get; set; }
    }

    public class CancelRegisterUseResponse : ApiResponse<StandardResponseData<CancelData>> { }
    #endregion

    #region Modify API
    public class ModifyRequest : BaseGuaranteeRequest
    {
        public string Token { get; set; }
    }

    public class ModifyData
    {
        public string TransId { get; set; }
        public string Token { get; set; }
    }

    public class ModifyResponse : ApiResponse<StandardResponseData<ModifyData>> { }
    #endregion

    #region Verify API
    public class VerifyRequest
    {
        public string RequestId { get; set; }
        public string OtpTransId { get; set; }
        public string Otp { get; set; }
    }

    public class VerifyData
    {
        public string TransId { get; set; }
        public string AvailableBalance { get; set; }
        public string ContractNumber { get; set; }
        public string RefNo { get; set; }
        public string Remark { get; set; }
    }

    public class VerifyResponse : ApiResponse<StandardResponseData<VerifyData>> { }
    #endregion

    #region Inquiry API
    public class InquiryRequest
    {
        public string RequestId { get; set; }
        public string Token { get; set; }
    }

    public class ContractInfo
    {
        public string ContractNumber { get; set; }
        public string EffectiveDate { get; set; }
        public string AvailableBalance { get; set; }
        public string Remark { get; set; }
    }

    public class InquiryData
    {
        public string TransId { get; set; }
        public List<ContractInfo> ContractInfo { get; set; }
    }

    public class InquiryResponse : ApiResponse<StandardResponseData<InquiryData>> { }
    #endregion

    #region AvailableBalanceInfo API
    public class AvailableBalanceInfoRequest
    {
        public string RequestId { get; set; }
        public string PatientFullName { get; set; }
        public string PatientDateOfBirth { get; set; }
        public string PatientCccd { get; set; }
        public string ApplicationCode { get; set; }
    }

    public class AvailableBalanceInfoData
    {
        public bool IsValid { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string RequestId { get; set; }
        public string RegisteredAmount { get; set; }
        public string UsedAmount { get; set; }
        public string AvailableBalance { get; set; }
    }

    public class AvailableBalanceInfoResponse : ApiResponse<AvailableBalanceInfoData> { }
    #endregion
}
