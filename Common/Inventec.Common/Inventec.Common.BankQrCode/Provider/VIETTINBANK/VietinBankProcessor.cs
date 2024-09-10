using Inventec.Common.BankQrCode.ADO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1;
using Inventec.Common.String;

namespace Inventec.Common.BankQrCode.Provider.VIETTINBANK
{
    class VietinBankProcessor : IRun
    {
        private ADO.BankQrCodeInputADO InputData;

        public VietinBankProcessor(ADO.BankQrCodeInputADO InputData)
        {
            // TODO: Complete member initialization
            this.InputData = InputData;
        }

        ResultQrCode IRun.Run()
        {
            ResultQrCode result = new ResultQrCode();
            try
            {
                ConfigADO configData = null;
                if (CheckInputData(ref result, ref configData))
                {
                    QRBean bean = new QRBean()
                    {
                        payLoad = configData.payLoad, // Mac dinh
                        pointOIMethod = configData.pointOTMethod, // Mac dinh doi voi QR co so hoa don
                        masterMerchant = configData.masterMerchant, // master Merchant VietinBank
                        merchantCode = configData.merchantCode, //MasterMerchant VNPAY
                        merchantCC = configData.merchantCC, // Do ben VNPAY cung cap
                        merchantCity = configData.merchantCity,
                        ccy = configData.ccy, // Loai Tien Te,
                        countryCode = configData.CounttryCode,
                        merchantName = configData.merchantName,
                        amount = this.InputData.Amount.ToString("G27", CultureInfo.InvariantCulture),
                    };

                    DateTime tranTime = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(configData.expDate) && tranTime != System.DateTime.MinValue)
                    {
                        long addMinute = 0;
                        if (long.TryParse(configData.expDate, out addMinute))
                        {
                            tranTime.AddMinutes(addMinute);
                        }
                        else
                        {
                            tranTime.AddMinutes(10);
                        }
                    }

                    QRAddtionalBean addBean = new QRAddtionalBean()
                    {
                        billNumber = this.InputData.TransactionCode, // so hoa don duy nhat trong 1 ngay
                        storeID = configData.storeID, // Ten cua Terminal do ben VTB gui
                        terminalID = configData.terminalId, // Ma TerminalID do VTB gui
                        purpose = this.InputData.Purpose, // Ghi chu dinh kem,
                        expDate = tranTime.ToString("yyyyMMddHHmmss")
                    };

                    bean.addtionalBean = addBean;

                    QrPack qrPack = new QrPack();

                    result.Data = qrPack.pack(bean, "").qrData;
                }
            }
            catch (Exception ex)
            {
                result = new ResultQrCode();
                result.Message = "Run Exception: " + ex.Message;
            }
            return result;
        }

        ResultQrCode IRun.RunConsumer()
        {
            return null;
        }

        private bool CheckInputData(ref ResultQrCode result, ref ConfigADO configData)
        {
            try
            {
                if (this.InputData == null)
                {
                    result = new ResultQrCode();
                    result.Message = "Sai dữ liệu khở tạo.";
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(this.InputData.SystemConfig))
                {
                    configData = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigADO>(this.InputData.SystemConfig);
                }
                else
                {
                    result = new ResultQrCode();
                    result.Message = "Sai định dạng cấu hình hệ thống.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                result = new ResultQrCode();
                result.Message = "Check data Exception: " + ex.Message;
                return false;
            }

            return true;
        }
    }
}
