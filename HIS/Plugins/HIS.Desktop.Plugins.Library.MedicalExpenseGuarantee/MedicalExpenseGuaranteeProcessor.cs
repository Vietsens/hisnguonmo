        
using HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee.ADO;
using HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee.Base;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee
{
    public class MedicalExpenseGuaranteeProcessor
    {
        public RegisterUseResponse GuaranteeRegisterUse(DataInput registerUse)
        {
            RegisterUseResponse registerUseResponse = new RegisterUseResponse(); 
            try
            {
                LogSystem.Info("input GuaranteeRegisterUse " + registerUse);
                LogSystem.Info("Start GuaranteeRegisterUse " + LogUtil.TraceData("input: ", registerUse.registerUseRequest));
                LogSystem.Info("input GuaranteeRegisterUse " + registerUse.registerUseRequest);

                string requiredFieldMsg = ValidateRequiredFields(registerUse);
                if (!string.IsNullOrWhiteSpace(requiredFieldMsg))
                {
                    registerUseResponse.Message = requiredFieldMsg;
                    registerUseResponse.Success = false;
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + registerUseResponse.Message);
                    return registerUseResponse;
                }

                if (!this.ValidateRegisterUse(registerUse.registerUseRequest, ref registerUseResponse)) 
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + registerUseResponse.Message);
                    return registerUseResponse;
                }

                Base.ApiConsumer consumer = new Base.ApiConsumer(registerUse.hasUri, registerUse.acsUri, registerUse.applicationCode, registerUse.limet, registerUse.cskcbbd, registerUse.username, registerUse.password);
                if (registerUse.registerUseRequest != null)
                {
                    registerUse.registerUseRequest.RequestAmount = registerUse.registerUseRequest.RequestAmount.Replace(".", "").Replace(",", "");

                    if (registerUse.registerUseRequest.Signature == null || registerUse.registerUseRequest.Signature == "")
                    {
                        //string name = ApiConsumer.NormalizeVietnameseName(registerUse.registerUseRequest.PatientFullName); 
                        //string Signature = name.ToLower() + registerUse.registerUseRequest.PatientDateOfBirth + registerUse.registerUseRequest.PatientCccd + registerUse.registerUseRequest.RequestAmount + registerUse.applicationCode;
                        string Signature = registerUse.registerUseRequest.PatientDateOfBirth + registerUse.registerUseRequest.PatientCccd + registerUse.registerUseRequest.RequestAmount + registerUse.applicationCode;
                        registerUse.registerUseRequest.Signature = consumer.ConvertSHA256(Signature);
                    }
                }

                LogSystem.Info("Start GuaranteeRegisterUse API" + LogUtil.TraceData("input: ", registerUse.registerUseRequest)); 
                LogSystem.Info("input GuaranteeRegisterUse API" + registerUse.registerUseRequest);
                // Gọi API Register Use
                registerUseResponse = consumer.CreateRequest<RegisterUseResponse>(Base.API.API_GUARANTEE_REGISTER_USE, registerUse.registerUseRequest);

                if (registerUseResponse != null && registerUseResponse.Success)
                {
                    return registerUseResponse;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("API call failed - Status: " + registerUseResponse?.Status + ", ErrorMessage: " + registerUseResponse?.Data.ErrorMessage + " ErrorCode: " + registerUseResponse?.Data.ErrorCode);
                    return registerUseResponse;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return registerUseResponse;
            }
        }
        public UseResponse GuaranteeUse(DataInput use)
        {
            UseResponse responseUser = new UseResponse();
            try
            {
                LogSystem.Info("Start GuaranteeUse " + LogUtil.TraceData("input: ", use.useRequest));
                LogSystem.Info("input GuaranteeUse " + use.useRequest);

                string requiredFieldMsg = ValidateRequiredFields(use);
                if (!string.IsNullOrWhiteSpace(requiredFieldMsg))
                {
                    responseUser.Message = requiredFieldMsg;
                    responseUser.Success = false;
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + responseUser.Message);
                    return responseUser;
                }

                // Validate dữ liệu đầu vào
                if (!this.ValidateUse(use.useRequest, ref responseUser))
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + responseUser.Message);
                    return responseUser;
                }

                // Khởi tạo API Consumer
                Base.ApiConsumer consumer = new Base.ApiConsumer(use.hasUri, use.acsUri, use.applicationCode, use.limet, use.cskcbbd, use.username, use.password);

                if (use.useRequest != null)
                {
                    use.useRequest.Amount = use.useRequest.Amount.Replace(".", "").Replace(",", "");

                    if (use.useRequest.Signature == null || use.useRequest.Signature == "")
                    {
                        string Signature = use.useRequest.RequestId + use.useRequest.Amount;
                        use.useRequest.Signature = consumer.ConvertSHA256(Signature);
                    }
                }

                LogSystem.Info("Start GuaranteeUse API" + LogUtil.TraceData("input: ", use.useRequest));
                LogSystem.Info("input GuaranteeUse API" + use.useRequest);
                // Gọi API Use
                responseUser = consumer.CreateRequest<UseResponse>(Base.API.API_GUARANTEE_USE, use.useRequest);

                if (responseUser != null && responseUser.Success)
                {
                    if (responseUser.Data?.ResponseStatus?.Status == "0")
                    {
                        var transId = responseUser.Data.Data.TransId;
                        var availableBalance = responseUser.Data.Data.AvailableBalance;
                        var refNo = responseUser.Data.Data.RefNo;

                        Inventec.Common.Logging.LogSystem.Info("GuaranteeUse Success - TransId: " + transId + ", RefNo: "+ refNo + ", AvailableBalance: "+availableBalance);
                        return responseUser;
                    }
                    else
                    {
                        var errorCode = responseUser.Data?.ResponseStatus?.ErrorCode;
                        var errorDesc = responseUser.Data?.ResponseStatus?.ErrorDesc;
                        Inventec.Common.Logging.LogSystem.Error("API Error - Code: " + errorCode + " Desc: "+ errorDesc);
                        return responseUser;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("API call failed - Status: " +responseUser?.Status+ ", ErrorDesc: " + responseUser.Data?.ResponseStatus?.ErrorDesc);
                    return responseUser;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return responseUser;
            }
        }
        public CancelRegisterUseResponse GuaranteeCancelRegisterUse(DataInput cancelRegisterUse)
        {
            CancelRegisterUseResponse cancelRegisterUseResponse = new CancelRegisterUseResponse();
            try
            {
                LogSystem.Info("Start GuaranteeCancelRegisterUse " + LogUtil.TraceData("input: ", cancelRegisterUse.cancelRegisterUseRequest));
                LogSystem.Info("input GuaranteeCancelRegisterUse " + cancelRegisterUse.cancelRegisterUseRequest);

                string requiredFieldMsg = ValidateRequiredFields(cancelRegisterUse);
                if (!string.IsNullOrWhiteSpace(requiredFieldMsg))
                {
                    cancelRegisterUseResponse.Message = requiredFieldMsg;
                    cancelRegisterUseResponse.Success = false;
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + cancelRegisterUseResponse.Message);
                    return cancelRegisterUseResponse;
                }

                if (!this.ValiCancelRegisterUse(cancelRegisterUse.cancelRegisterUseRequest, ref cancelRegisterUseResponse))
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + cancelRegisterUseResponse.Message);
                    return cancelRegisterUseResponse;
                }

                Base.ApiConsumer consumer = new Base.ApiConsumer(cancelRegisterUse.hasUri, cancelRegisterUse.acsUri, cancelRegisterUse.applicationCode, cancelRegisterUse.limet, cancelRegisterUse.cskcbbd, cancelRegisterUse.username, cancelRegisterUse.password);

                if (cancelRegisterUse.cancelRegisterUseRequest != null)
                {
                    cancelRegisterUse.cancelRegisterUseRequest.Amount = cancelRegisterUse.cancelRegisterUseRequest.Amount.Replace(".", "").Replace(",", "");

                    if (cancelRegisterUse.cancelRegisterUseRequest.Signature == null || cancelRegisterUse.cancelRegisterUseRequest.Signature == "")
                    {
                        string Signature = cancelRegisterUse.cancelRegisterUseRequest.RequestId + cancelRegisterUse.cancelRegisterUseRequest.Amount;
                        cancelRegisterUse.cancelRegisterUseRequest.Signature = consumer.ConvertSHA256(Signature);
                    }
                }

                LogSystem.Info("Start GuaranteeCancelRegisterUse API" + LogUtil.TraceData("input: ", cancelRegisterUse.cancelRegisterUseRequest));
                LogSystem.Info("input GuaranteeCancelRegisterUse API" + cancelRegisterUse.cancelRegisterUseRequest);
                cancelRegisterUseResponse = consumer.CreateRequest<CancelRegisterUseResponse>(Base.API.API_GUARANTEE_CANCEL_REGISTER_USE, cancelRegisterUse.cancelRegisterUseRequest);

                if (cancelRegisterUseResponse != null && cancelRegisterUseResponse.Success)
                {
                    if (cancelRegisterUseResponse.Data?.ResponseStatus?.Status == "0")
                    {
                        Inventec.Common.Logging.LogSystem.Info("GuaranteeCancelRegisterUse Success - RequestId: " + cancelRegisterUseResponse?.Data?.ResponseStatus?.ErrorDesc);
                        return cancelRegisterUseResponse;
                    }
                    else
                    {
                        var errorCode = cancelRegisterUseResponse.Data?.ResponseStatus?.ErrorCode;
                        var errorDesc = cancelRegisterUseResponse.Data?.ResponseStatus?.ErrorDesc;
                        Inventec.Common.Logging.LogSystem.Error("API Error - Code: " + errorCode + " Desc: " + errorDesc);
                        return cancelRegisterUseResponse;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("API call failed - Status: " + cancelRegisterUseResponse?.Status + ", errorDesc: " + cancelRegisterUseResponse?.Data?.ResponseStatus?.ErrorDesc);
                    return cancelRegisterUseResponse;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return cancelRegisterUseResponse;
            }
        }
        public AvailableBalanceInfoResponse GuaranteeAvailableBalanceInfoResponse(DataInput availableBalanceInfo)
        {
            AvailableBalanceInfoResponse availableBalanceInfoResponse = new AvailableBalanceInfoResponse();
            try
            {
                LogSystem.Info("Start GuaranteeAvailableBalanceInfoResponse " + LogUtil.TraceData("input: ", availableBalanceInfo.availableBalanceInfoRequest));
                LogSystem.Info("input GuaranteeAvailableBalanceInfoResponse " + availableBalanceInfo.availableBalanceInfoRequest);

                string requiredFieldMsg = ValidateRequiredFields(availableBalanceInfo);
                if (!string.IsNullOrWhiteSpace(requiredFieldMsg))
                {
                    availableBalanceInfoResponse.Message = requiredFieldMsg;
                    availableBalanceInfoResponse.Success = false;
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + availableBalanceInfoResponse.Message);
                    return availableBalanceInfoResponse;
                }

                if (!this.ValiAvailableBalanceInfo(availableBalanceInfo.availableBalanceInfoRequest, ref availableBalanceInfoResponse))
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + availableBalanceInfoResponse.Message);
                    return availableBalanceInfoResponse;
                }

                if (availableBalanceInfo.cskcbbd != null)
                {
                    availableBalanceInfo.availableBalanceInfoRequest.HospitalCode = availableBalanceInfo.cskcbbd;
                }

                Base.ApiConsumer consumer = new Base.ApiConsumer(availableBalanceInfo.hasUri, availableBalanceInfo.acsUri, availableBalanceInfo.applicationCode, availableBalanceInfo.limet, availableBalanceInfo.cskcbbd, availableBalanceInfo.username, availableBalanceInfo.password);

                availableBalanceInfoResponse = consumer.CreateRequest<AvailableBalanceInfoResponse>(Base.API.API_GUARANTEE_AVAILABLE_BALANCE_INFO, availableBalanceInfo.availableBalanceInfoRequest);

                if (availableBalanceInfoResponse == null || !availableBalanceInfoResponse.Success)
                {
                    Inventec.Common.Logging.LogSystem.Error("API call failed - Status: " + availableBalanceInfoResponse?.Status + ", Message: " + availableBalanceInfoResponse?.Message);
                    return availableBalanceInfoResponse;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("GuaranteeAvailableBalanceInfoResponse Success - data: " + availableBalanceInfoResponse.Data);
                    return availableBalanceInfoResponse;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return availableBalanceInfoResponse;
            }
        }
        private string ValidateRequiredFields(DataInput input)
        {
            if (input == null)
                return "Dữ liệu request không được để trống";
            if (string.IsNullOrWhiteSpace(input.hasUri))
                return "Trường hasUri là bắt buộc";
            if (string.IsNullOrWhiteSpace(input.acsUri))
                return "Trường acsUri là bắt buộc";
            if (string.IsNullOrWhiteSpace(input.username))
                return "Trường username là bắt buộc";
            if (string.IsNullOrWhiteSpace(input.password))
                return "Trường password là bắt buộc";
            if (string.IsNullOrWhiteSpace(input.applicationCode))
                return "Trường applicationCode là bắt buộc";
            if (string.IsNullOrWhiteSpace(input.limet))
                return "Trường limet là bắt buộc";
            if (string.IsNullOrWhiteSpace(input.cskcbbd))
                return "Trường cskcbbd là bắt buộc";
            return null;
        }
        private bool ValidateUse(UseRequest dataUser, ref UseResponse response) 
        {
            bool result = true;
            try
            {
                string mess = "";

                if (dataUser == null)
                {
                    mess = "Dữ liệu request không được để trống";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.RequestId))
                {
                    mess = "Không xác định được mã giao dịch";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.Amount))
                {
                    mess = "Không xác định được số tiền thanh toán";
                }
                else if (dataUser.Amount == "0" || dataUser.Amount == "")
                {
                    mess = "Số tiền không hợp lệ, số tiền dịch vụ phải > 0";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.Remark))
                {
                    mess = "Không xác định được diễn giải giao dịch";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.ContractNumber))
                {
                    mess = "Không xác định được số hợp đồng";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.PatientFullName))
                {
                    mess = "Không xác định được tên bệnh nhân";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.PatientDateOfBirth))
                {
                    mess = "Không xác định được ngày sinh";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.PatientCccd))
                {
                    mess = "Không xác định được số CCCD/CMND";
                }

                if (!string.IsNullOrWhiteSpace(mess))
                {
                    result = false;
                    if (response == null)
                        response = new UseResponse();
                    response.Message = mess;
                    response.Success = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
        private bool ValidateRegisterUse(RegisterUseRequest dataUseRequest, ref RegisterUseResponse response)
        {
            bool result = true;
            try
            {
                string mess = "";

                if (dataUseRequest == null)
                {
                    mess = "Dữ liệu request không được để trống";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.RequestAmount))
                {
                    mess = "Không xác định được số tiền thanh toán";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.Remark))
                {
                    mess = "Không xác định được diễn giải giao dịch";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.ApplicationCode))
                {
                    mess = "Không xác định được mã ứng dụng của HIS";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.PatientFullName))
                {
                    mess = "Không xác định được tên bệnh nhân";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.PatientDateOfBirth))
                {
                    mess = "Không xác định được ngày sinh";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.PatientCccd))
                {
                    mess = "Không xác định được số CCCD/CMND";
                }

                if (!string.IsNullOrWhiteSpace(mess))
                {
                    result = false;
                    if (response == null)
                        response = new RegisterUseResponse();
                    response.Message = mess;
                    response.Success = false;
                    response.Status = 400;
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
        private bool ValiCancelRegisterUse(CancelRegisterUseRequest dataCancelRegisterUse, ref CancelRegisterUseResponse response)
        {
            bool result = true;
            try
            {
                string mess = "";

                if (dataCancelRegisterUse == null)
                {
                    mess = "Dữ liệu request không được để trống";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.RequestId))
                {
                    mess = "Không xác định được mã giao dịch";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.Amount))
                {
                    mess = "Không xác định được số tiền thanh toán";
                }
                else if (dataCancelRegisterUse.Amount == "0" || dataCancelRegisterUse.Amount == "")
                {
                    mess = "Số tiền không hợp lệ, số tiền dịch vụ phải > 0";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.Remark))
                {
                    mess = "Không xác định được diễn giải giao dịch";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.ContractNumber))
                {
                    mess = "Không xác định được số hợp đồng";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientFullName))
                {
                    mess = "Không xác định được tên bệnh nhân";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientDateOfBirth))
                {
                    mess = "Không xác định được ngày sinh";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientCccd))
                {
                    mess = "Không xác định được số CCCD/CMND";
                }

                if (!string.IsNullOrWhiteSpace(mess))
                {
                    result = false;
                    if (response == null)
                        response = new CancelRegisterUseResponse();
                    response.Message = mess;
                    response.Success = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
        private bool ValiAvailableBalanceInfo(AvailableBalanceInfoRequest dataCancelRegisterUse, ref AvailableBalanceInfoResponse response)
        {
            bool result = true;
            try
            {
                string mess = "";

                if (dataCancelRegisterUse == null)
                {
                    mess = "Dữ liệu request không được để trống";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.RequestId))
                {
                    mess = "Không xác định được mã giao dịch";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientFullName))
                {
                    mess = "Không xác định được tên bệnh nhân";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientDateOfBirth))
                {
                    mess = "Không xác định được ngày sinh";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientCccd))
                {
                    mess = "Không xác định được số CCCD/CMND";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.Remark))
                {
                    mess = "Không xác định được diễn giải giao dịch";
                }

                if (!string.IsNullOrWhiteSpace(mess))
                {
                    result = false;
                    if (response == null)
                        response = new AvailableBalanceInfoResponse();
                    response.Message = mess;
                    response.Success = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
    }
}
