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
            try
            {
                LogSystem.Info("Start GuaranteeRegisterUse " + LogUtil.TraceData("input: ", registerUse.registerUseRequest));
                LogSystem.Info("input GuaranteeRegisterUse " + registerUse.registerUseRequest);
                RegisterUseResponse registerUseResponse = new RegisterUseResponse();
                if (!this.ValidateRegisterUse(registerUse.registerUseRequest, ref registerUseResponse))
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + registerUseResponse.Message);
                    return null;
                }

                Base.ApiConsumer consumer = new Base.ApiConsumer(registerUse.baseUri, registerUse.applicationCode, registerUse.limet, registerUse.cskcbbd);
                if (registerUse.registerUseRequest.Signature == null || registerUse.registerUseRequest.Signature == "")
                {
                    string name = ApiConsumer.NormalizeString(registerUse.registerUseRequest.PatientFullName);
                    string Signature = name + registerUse.registerUseRequest.PatientDateOfBirth + registerUse.registerUseRequest.PatientCccd + registerUse.registerUseRequest.RequestAmount + registerUse.applicationCode;
                    registerUse.registerUseRequest.Signature = consumer.ConvertSHA256(Signature);
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
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        public UseResponse GuaranteeUse(DataInput use)
        {
            try
            {
                LogSystem.Info("Start GuaranteeUse " + LogUtil.TraceData("input: ", use.useRequest));
                LogSystem.Info("input GuaranteeUse " + use.useRequest);
                UseResponse responseUser = new UseResponse();

                // Validate dữ liệu đầu vào
                if (!this.ValidateUse(use.useRequest, ref responseUser))
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + responseUser.Message);
                    return null;
                }
                
                // Khởi tạo API Consumer
                Base.ApiConsumer consumer = new Base.ApiConsumer(use.baseUri, use.applicationCode, use.limet, use.cskcbbd);

                if (use.useRequest.Signature == null || use.useRequest.Signature == "")
                {
                    string Signature = use.useRequest.RequestId + use.useRequest.Amount;
                    use.useRequest.Signature = consumer.ConvertSHA256(Signature);
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
                        return null;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("API call failed - Status: " +responseUser?.Status+", Message: "+responseUser?.Message);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        public CancelRegisterUseResponse GuaranteeCancelRegisterUse(DataInput cancelRegisterUse)
        {
            try
            {
                LogSystem.Info("Start GuaranteeCancelRegisterUse " + LogUtil.TraceData("input: ", cancelRegisterUse.cancelRegisterUseRequest));
                LogSystem.Info("input GuaranteeCancelRegisterUse " + cancelRegisterUse.cancelRegisterUseRequest);
                CancelRegisterUseResponse cancelRegisterUseResponse = new CancelRegisterUseResponse();
                if (!this.ValiCancelRegisterUse(cancelRegisterUse.cancelRegisterUseRequest, ref cancelRegisterUseResponse))
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + cancelRegisterUseResponse.Message);
                    return null;
                }

                Base.ApiConsumer consumer = new Base.ApiConsumer(cancelRegisterUse.baseUri, cancelRegisterUse.applicationCode, cancelRegisterUse.limet, cancelRegisterUse.cskcbbd);

                if (cancelRegisterUse.cancelRegisterUseRequest.Signature == null || cancelRegisterUse.cancelRegisterUseRequest.Signature == "")
                {
                    string Signature = cancelRegisterUse.cancelRegisterUseRequest.RequestId + cancelRegisterUse.cancelRegisterUseRequest.Amount;
                    cancelRegisterUse.cancelRegisterUseRequest.Signature = consumer.ConvertSHA256(Signature);
                }

                LogSystem.Info("Start GuaranteeCancelRegisterUse API" + LogUtil.TraceData("input: ", cancelRegisterUse.cancelRegisterUseRequest));
                LogSystem.Info("input GuaranteeCancelRegisterUse API" + cancelRegisterUse.cancelRegisterUseRequest);
                cancelRegisterUseResponse = consumer.CreateRequest<CancelRegisterUseResponse>(Base.API.API_GUARANTEE_CANCEL_REGISTER_USE, cancelRegisterUse.cancelRegisterUseRequest);

                if (cancelRegisterUseResponse != null && cancelRegisterUseResponse.Success)
                {
                    if (cancelRegisterUseResponse.Data?.ResponseStatus?.Status == "0")
                    {
                        Inventec.Common.Logging.LogSystem.Info("GuaranteeCancelRegisterUse Success - RequestId: " + cancelRegisterUseResponse.Data.Data.TransId);
                        return cancelRegisterUseResponse;
                    }
                    else
                    {
                        var errorCode = cancelRegisterUseResponse.Data?.ResponseStatus?.ErrorCode;
                        var errorDesc = cancelRegisterUseResponse.Data?.ResponseStatus?.ErrorDesc;
                        Inventec.Common.Logging.LogSystem.Error("API Error - Code: " + errorCode + " Desc: " + errorDesc);
                        return null;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("API call failed - Status: " + cancelRegisterUseResponse?.Status + ", Message: " + cancelRegisterUseResponse?.Message);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        public AvailableBalanceInfoResponse GuaranteeAvailableBalanceInfoResponse(DataInput availableBalanceInfo)
        {
            try
            {
                LogSystem.Info("Start GuaranteeAvailableBalanceInfoResponse " + LogUtil.TraceData("input: ", availableBalanceInfo.availableBalanceInfoRequest));
                LogSystem.Info("input GuaranteeAvailableBalanceInfoResponse " + availableBalanceInfo.availableBalanceInfoRequest);
                AvailableBalanceInfoResponse availableBalanceInfoResponse = new AvailableBalanceInfoResponse();
                if (!this.ValiAvailableBalanceInfo(availableBalanceInfo.availableBalanceInfoRequest, ref availableBalanceInfoResponse))
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + availableBalanceInfoResponse.Message);
                    return null;
                }

                Base.ApiConsumer consumer = new Base.ApiConsumer(availableBalanceInfo.baseUri, availableBalanceInfo.applicationCode, availableBalanceInfo.limet, availableBalanceInfo.cskcbbd);

                availableBalanceInfoResponse = consumer.CreateRequest<AvailableBalanceInfoResponse>(Base.API.API_GUARANTEE_AVAILABLE_BALANCE_INFO, availableBalanceInfo.availableBalanceInfoRequest);

                if (availableBalanceInfoResponse == null || !availableBalanceInfoResponse.Success)
                {
                    Inventec.Common.Logging.LogSystem.Error("API call failed - Status: " + availableBalanceInfoResponse?.Status + ", Message: " + availableBalanceInfoResponse?.Message);
                    return null;
                }
                else
                {
                    if (availableBalanceInfoResponse.Data.IsValid)
                    {
                        Inventec.Common.Logging.LogSystem.Info("GuaranteeAvailableBalanceInfoResponse Success - data: " + availableBalanceInfoResponse.Data);
                        return availableBalanceInfoResponse;
                    }
                    else
                    {
                        var errorCode = availableBalanceInfoResponse.Data?.ErrorCode;
                        var errorDesc = availableBalanceInfoResponse.Data?.ErrorMessage;
                        Inventec.Common.Logging.LogSystem.Error("API Error - Code: " + errorCode + " Desc: " + errorDesc);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
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
                    mess = "Không xác định được mã giao dịch (RequestId)";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.Amount))
                {
                    mess = "Không xác định được số tiền thanh toán (Amount)";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.Remark))
                {
                    mess = "Không xác định được diễn giải giao dịch (Remark)";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.ContractNumber))
                {
                    mess = "Không xác định được số hợp đồng (ContractNumber)";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.PatientFullName))
                {
                    mess = "Không xác định được tên bệnh nhân (PatientName)";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.PatientDateOfBirth))
                {
                    mess = "Không xác định được ngày sinh (Dob - yyyyMMddHHmmss)";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.PatientCccd))
                {
                    mess = "Không xác định được số CCCD/CMND (CccdNumber)";
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
                    mess = "Không xác định được số tiền thanh toán (RequestAmount)";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.Remark))
                {
                    mess = "Không xác định được diễn giải giao dịch (Remark)";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.ApplicationCode))
                {
                    mess = "Không xác định được mã ứng dụng của HIS(ApplicationCode)";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.PatientFullName))
                {
                    mess = "Không xác định được tên bệnh nhân (PatientName)";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.PatientDateOfBirth))
                {
                    mess = "Không xác định được ngày sinh (Dob - yyyyMMddHHmmss)";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.PatientCccd))
                {
                    mess = "Không xác định được số CCCD/CMND (PatientCccd)";
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
                    mess = "Không xác định được mã giao dịch (RequestId)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.Amount))
                {
                    mess = "Không xác định được số tiền thanh toán (Amount)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.Remark))
                {
                    mess = "Không xác định được diễn giải giao dịch (Remark)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.ContractNumber))
                {
                    mess = "Không xác định được số hợp đồng (ContractNumber)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientFullName))
                {
                    mess = "Không xác định được tên bệnh nhân (PatientName)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientDateOfBirth))
                {
                    mess = "Không xác định được ngày sinh (Dob - yyyyMMddHHmmss)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientCccd))
                {
                    mess = "Không xác định được số CCCD/CMND (CccdNumber)";
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
                    mess = "Không xác định được mã giao dịch (RequestId)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientFullName))
                {
                    mess = "Không xác định được tên bệnh nhân (PatientName)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientDateOfBirth))
                {
                    mess = "Không xác định được ngày sinh (Dob - yyyyMMddHHmmss)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientCccd))
                {
                    mess = "Không xác định được số CCCD/CMND (CccdNumber)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.Remark))
                {
                    mess = "Không xác định được diễn giải giao dịch (Remark)";
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
