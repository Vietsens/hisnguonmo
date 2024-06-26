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
using Inventec.Common.BankQrCode;
using Inventec.Common.BankQrCode.ADO;
using Inventec.Common.QRCoder;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Common.BankQrCode
{
    public class QrCodeProcessor
    {
        public static Dictionary<string, object> CreateQrImage(HIS_TRANS_REQ data, List<HIS_CONFIG> configValue)
        {
            Dictionary<string, object> result = null;
            if (data != null && configValue != null && configValue.Count > 0)
            {
                result = new Dictionary<string, object>();
                List<Task> taskall = new List<Task>();
                foreach (var config in configValue)
                {
                    Task tsQr = Task.Factory.StartNew((object obj) =>
                    {
                        HIS_CONFIG cfg = obj as HIS_CONFIG;

                        string value = !String.IsNullOrWhiteSpace(cfg.VALUE) ? cfg.VALUE : cfg.DEFAULT_VALUE;
                        if (!String.IsNullOrWhiteSpace(value))
                        {
                            ProvinceType bankType = ProvinceType.BIDV;
                            string key = GetTemplateKey(cfg.KEY, ref bankType);

                            BankQrCodeInputADO inputData = new BankQrCodeInputADO();
                            inputData.Amount = data.AMOUNT;
                            inputData.TransactionCode = data.TRANS_REQ_CODE;
                            inputData.SystemConfig = value;
                            inputData.Purpose = data.TDL_TREATMENT_CODE;
                            BankQrCodeProcessor bankQrCode = new BankQrCodeProcessor(inputData);
                            ResultQrCode qrData = bankQrCode.GetQrCode(bankType);
                            if (!String.IsNullOrWhiteSpace(qrData.Data))
                            {
                                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData.Data, QRCodeGenerator.ECCLevel.Q);
                                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                                byte[] qrCodeImage = qrCode.GetGraphic(20);
                                result[key] = qrCodeImage;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputData), inputData));
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => qrData), qrData));
                            }
                        }
                    }, config);
                    taskall.Add(tsQr);
                }

                Task.WaitAll(taskall.ToArray());
            }

            if (data != null && !String.IsNullOrWhiteSpace(data.TRANS_REQ_CODE))
            {
                result["TRANS_REQ_CODE"] = data.TRANS_REQ_CODE;
                result["PAYMENT_QR_AMOUNT"] = data.AMOUNT;
            }

            return result;
        }

        private static string GetTemplateKey(string key, ref ProvinceType bankType)
        {
            string result = null;
            switch (key)
            {
                case "HIS.Desktop.Plugins.PaymentQrCode.VietinbankInfo":
                    result = "PAYMENT_QR_CODE_VIETINBANK";
                    bankType = ProvinceType.VIETINBANK;
                    break;
                case "HIS.Desktop.Plugins.PaymentQrCode.BIDVInfo":
                    result = "PAYMENT_QR_CODE_BIDV";
                    bankType = ProvinceType.BIDV;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
