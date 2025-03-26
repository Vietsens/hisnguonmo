/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Inventec.Common.BankQrCode.ADO;
using Inventec.Common.BankQrCode.Provider.LPBANK.Model;
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
                    apiData.purpose = this.InputData.TransactionCode;
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
            ResultQrCode resultQrCode = new ResultQrCode();
            try
            {
                if (InputData != null)
                {
                    LpInfoData configArr = Newtonsoft.Json.JsonConvert.DeserializeObject<LpInfoData>(this.InputData.SystemConfig);
                    CreatQrData creatQrData = new CreatQrData();
                    creatQrData.methodCode = configArr.methodCode;
                    creatQrData.guid = "A000000727";
                    creatQrData.acquierOrBnb = configArr.acquierOrBnb;
                    creatQrData.qrType = configArr.qrType;
                    creatQrData.merchantOrConsumer = configArr.merchantOrConsumer;
                    creatQrData.countryCode = configArr.CounttryCode;
                    creatQrData.ccy = configArr.ccy;
                    if (InputData.Amount > 0m)
                    {
                        creatQrData.amount = QrCodeUtil.ProcessConvertAmount(InputData.Amount);
                    }
                    InputData.TransactionCode = string.Format("HD{0}TTBV", InputData.TransactionCode);
                    creatQrData.billNumber = InputData.TransactionCode;
                    creatQrData.purpose = this.InputData.TransactionCode;
                    Qrcode qrcode = new Qrcode(creatQrData);
                    resultQrCode.Data = qrcode.createTotal();
                }
            }
            catch (Exception ex)
            {
                resultQrCode = new ResultQrCode();
                resultQrCode.Message = "Run Exception: " + ex.Message;
            }

            return resultQrCode;
        }
    }
}
