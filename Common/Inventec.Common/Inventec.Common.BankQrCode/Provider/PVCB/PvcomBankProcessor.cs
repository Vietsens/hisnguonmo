using Inventec.Common.BankQrCode.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode.Provider.PVCB
{
    class PvcomBankProcessor : IRun
    {
        private ADO.BankQrCodeInputADO InputData;

        public PvcomBankProcessor(ADO.BankQrCodeInputADO InputData)
        {
            // TODO: Complete member initialization
            this.InputData = InputData;
        }

        ResultQrCode IRun.RunConsumer()
        {
            ResultQrCode result = new ResultQrCode();
            try
            {
                if (InputData.ConsumerInfo != null)
                {
                    CreatQrData apiData = new CreatQrData();
                    apiData.methodCode = InputData.ConsumerInfo.pointOTMethod;
                    apiData.guid = "A000000727";
                    apiData.acquierOrBnb = InputData.ConsumerInfo.bnbID;
                    apiData.qrType = InputData.ConsumerInfo.transferType;
                    apiData.merchantOrConsumer = InputData.ConsumerInfo.ConsumerID;
                    apiData.countryCode = InputData.ConsumerInfo.countryCode;
                    apiData.ccy = InputData.ConsumerInfo.ccy;
                    if (this.InputData.Amount > 0)
                    {
                        apiData.amount = QrCodeUtil.ProcessConvertAmount(this.InputData.Amount);
                    }
                    apiData.billNumber = this.InputData.TransactionCode;

                    var qrcode = new Qrcode(apiData);

                    result.Data = qrcode.createConsumer();
                }
            }
            catch (Exception ex)
            {
                result = new ResultQrCode();
                result.Message = "Run Exception: " + ex.Message;
            }
            return result;
        }

        ResultQrCode IRun.Run()
        {
            return null;
        }
    }
}
