using Inventec.Common.BankQrCode.ADO;
using Inventec.Common.BankQrCode.Provider.BIDV.Model;
using Inventec.Common.BankQrCode.Provider.LPBANK.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode.Provider.LPBANK
{
    class LPBankProcessor : IRun
    {
        private ADO.BankQrCodeInputADO InputData;

        public LPBankProcessor(ADO.BankQrCodeInputADO InputData)
        {
            // TODO: Complete member initialization
            this.InputData = InputData;
        }

        ResultQrCode IRun.Run()
        {
            ResultQrCode result = new ResultQrCode();
            try
            {
                if (InputData != null)
                {
                    LpInfoData configArr = Newtonsoft.Json.JsonConvert.DeserializeObject<LpInfoData>(this.InputData.SystemConfig);
                    CreatQrData apiData = new CreatQrData();
                    apiData.payLoad = configArr.payLoad;
                    apiData.methodCode = configArr.methodCode;
                    apiData.guid = configArr.guid;
                    apiData.acquierOrBnb = configArr.acquierOrBnb;
                    apiData.qrType = configArr.qrType;
                    apiData.merchantOrConsumer = string.Format("{0}{1}", configArr.merchantOrConsumer, this.InputData.TransactionCode);
                    apiData.countryCode = configArr.CounttryCode;
                    apiData.ccy = configArr.ccy;
                    if (this.InputData.Amount > 0)
                    {
                        apiData.amount = QrCodeUtil.ProcessConvertAmount(this.InputData.Amount);
                    }
                    apiData.billNumber = this.InputData.TransactionCode;
                    apiData.purpose = InputData.Purpose;
                    var qrcode = new Qrcode(apiData); 
                    result.Data = qrcode.createTotal();
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
    }
}
