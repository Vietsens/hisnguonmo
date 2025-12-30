using HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee.ADO;
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
                RegisterUseResponse registerUseResponse = new RegisterUseResponse();
                if (!this.ValidateRegisterUse(registerUse.registerUseRequest, ref registerUseResponse))
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + registerUseResponse.Message);
                    return null;
                }

                Base.ApiConsumer consumer = new Base.ApiConsumer(registerUse.baseUri, registerUse.applicationCode, registerUse.limet);

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
                UseResponse responseUser = new UseResponse();

                // Validate dữ liệu đầu vào
                if (!this.ValidateUse(use.useRequest, ref responseUser))
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + responseUser.Message);
                    return null;
                }
                
                // Khởi tạo API Consumer
                Base.ApiConsumer consumer = new Base.ApiConsumer(use.baseUri, use.applicationCode, use.limet);

                if (use.useRequest.Signature == null || use.useRequest.Signature == "")
                {
                    string Signature = use.useRequest.RequestId + use.useRequest.Amount;
                    use.useRequest.Signature = consumer.ConvertSHA256(Signature);
                }
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
                CancelRegisterUseResponse cancelRegisterUseResponse = new CancelRegisterUseResponse();
                if (!this.ValiCancelRegisterUse(cancelRegisterUse.cancelRegisterUseRequest, ref cancelRegisterUseResponse))
                {
                    Inventec.Common.Logging.LogSystem.Error("Validate failed: " + cancelRegisterUseResponse.Message);
                    return null;
                }

                Base.ApiConsumer consumer = new Base.ApiConsumer(cancelRegisterUse.baseUri, cancelRegisterUse.applicationCode, cancelRegisterUse.limet);

                if (cancelRegisterUse.cancelRegisterUseRequest.Signature == null || cancelRegisterUse.cancelRegisterUseRequest.Signature == "")
                {
                    string Signature = cancelRegisterUse.cancelRegisterUseRequest.RequestId + cancelRegisterUse.cancelRegisterUseRequest.Amount;
                    cancelRegisterUse.cancelRegisterUseRequest.Signature = consumer.ConvertSHA256(Signature);
                }

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

        public bool ValidateUse(UseRequest dataUser, ref UseResponse response)
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
                else if (string.IsNullOrWhiteSpace(dataUser.PatientName))
                {
                    mess = "Không xác định được tên bệnh nhân (PatientName)";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.Dob))
                {
                    mess = "Không xác định được ngày sinh (Dob - yyyyMMddHHmmss)";
                }
                else if (string.IsNullOrWhiteSpace(dataUser.CccdNumber))
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

        public bool ValidateRegisterUse(RegisterUseRequest dataUseRequest, ref RegisterUseResponse response)
        {
            bool result = true;
            try
            {
                string mess = "";

                if (dataUseRequest == null)
                {
                    mess = "Dữ liệu request không được để trống";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.Amount))
                {
                    mess = "Không xác định được số tiền thanh toán (Amount)";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.Remark))
                {
                    mess = "Không xác định được diễn giải giao dịch (Remark)";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.ApplicationCode))
                {
                    mess = "Không xác định được mã ứng dụng của HIS(ApplicationCode)";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.PatientName))
                {
                    mess = "Không xác định được tên bệnh nhân (PatientName)";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.Dob))
                {
                    mess = "Không xác định được ngày sinh (Dob - yyyyMMddHHmmss)";
                }
                else if (string.IsNullOrWhiteSpace(dataUseRequest.CccdNumber))
                {
                    mess = "Không xác định được số CCCD/CMND (CccdNumber)";
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

        public bool ValiCancelRegisterUse(CancelRegisterUseRequest dataCancelRegisterUse, ref CancelRegisterUseResponse response)
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
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.PatientName))
                {
                    mess = "Không xác định được tên bệnh nhân (PatientName)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.Dob))
                {
                    mess = "Không xác định được ngày sinh (Dob - yyyyMMddHHmmss)";
                }
                else if (string.IsNullOrWhiteSpace(dataCancelRegisterUse.CccdNumber))
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
    }
}
