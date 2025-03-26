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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.BankQrCode;
using Inventec.Common.BankQrCode.ADO;
using Inventec.Common.QRCoder;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.TDO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Common.BankQrCode
{
    public class QrCodeProcessor
    {
        public static Dictionary<string, string> DicContentBank = new Dictionary<string, string>();
        public static Dictionary<string, object> CreateQrImage(HIS_TRANS_REQ data, List<HIS_CONFIG> configValue)
        {
            Dictionary<string, object> result = null;
            if (data != null)
            {
                bool IsQrDynamic = false;
                bool IsGenQrBidvApi = false;
                MOS.TDO.QrPaymentGenerateResultTDO QrBidv = null;
                #region "BIDV"
                if (configValue != null && configValue.Count > 0 && configValue.Exists(p => p.KEY.Replace(" ", "").IndexOf(string.Format(".{0}Info", "BIDV")) > -1))
                {
                    var configBIDV = configValue.FirstOrDefault(p => p.KEY.Replace(" ", "").IndexOf(string.Format(".{0}Info", "BIDV")) > -1);
                    if (configBIDV != null && !string.IsNullOrEmpty(configBIDV.VALUE) && configBIDV.VALUE.IndexOf("GenQrMethod") > -1)
                    {
                        JObject jsonObject = JObject.Parse(configBIDV.VALUE);
                        string genQrMethod = jsonObject["GenQrMethod"].ToString();
                        if (genQrMethod == "OPEN_API")
                        {
                            CommonParam param = new CommonParam();
                            MOS.TDO.QrPaymentGenerateTDO tdo = new MOS.TDO.QrPaymentGenerateTDO();
                            tdo.TransReqId = data.ID;
                            tdo.Bank = "BIDV";
                            tdo.BankConfig = configBIDV.VALUE;
                            QrBidv = new Inventec.Common.Adapter.BackendAdapter(param).Post<QrPaymentGenerateResultTDO>("api/HisTransReq/QrPaymentGenerateImg", ApiConsumers.MosConsumer, tdo, param);
                            if (QrBidv == null)
                            {
                                XtraMessageBox.Show(param.GetMessage());
                                return null;
                            }
                            else
                            {
                                IsGenQrBidvApi = true;
                            }
                        }
                    }
                }
                #endregion
                #region "MBB", "VCB", "CTG"
                else
                {
                    List<string> banks = new List<string>() { "MBB", "VCB", "CTG" };
                    if (string.IsNullOrEmpty(data.QR_TEXT) && configValue != null && configValue.Count > 0 && banks.Exists(o => configValue.Exists(p => p.KEY.Replace(" ", "").IndexOf(string.Format(".{0}Info", o)) > -1)))
                    {
                        if (configValue.Count == 1)
                        {
                            CommonParam param = new CommonParam();
                            MOS.TDO.QrPaymentGenerateTDO tdo = new MOS.TDO.QrPaymentGenerateTDO();
                            tdo.TransReqId = data.ID;
                            tdo.Bank = configValue[0].KEY.Contains("MBB") ? "MBB" : configValue[0].KEY.Contains("VCB") ? "VCB" : "CTG";
                            tdo.BankConfig = configValue[0].VALUE;
                            data = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRANS_REQ>("api/HisTransReq/QrPaymentGenerate", ApiConsumers.MosConsumer, tdo, param);
                            if (data == null)
                            {
                                XtraMessageBox.Show(param.GetMessage());
                                return null;
                            }
                        }
                        else
                        {
                            configValue = configValue.Where(o => !banks.Exists(p => o.KEY.IndexOf(p) > -1)).ToList();
                        }
                    }
                    if (data != null && !string.IsNullOrEmpty(data.QR_TEXT))
                    {
                        configValue = new List<HIS_CONFIG>();
                        configValue.Add(new HIS_CONFIG() { KEY = "DYNAMIC", VALUE = data.QR_TEXT });
                        IsQrDynamic = true;
                    }
                }
                #endregion

                if (configValue != null && configValue.Count > 0)
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
                                if (IsGenQrBidvApi && QrBidv != null && QrBidv.TransReqId == data.ID)
                                {
                                    result[key] = Convert.FromBase64String(ConvertString(QrBidv.ImgQrBase64));
                                }
                                else
                                {
                                    ResultQrCode qrData = IsQrDynamic ? new ResultQrCode() { Data = config.VALUE } : bankQrCode.GetQrCode(bankType);
                                    DicContentBank[data.TRANS_REQ_CODE] = qrData.Data;
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
            }
            return result;
        }

        private static string ConvertString(string imgBase64)
        {
            try
            {
                int n = imgBase64.Length % 4;
                if (n > 0)
                {
                    for (int i = 0; i < (4 - n); i++)
                    {
                        imgBase64 += "=";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return imgBase64;
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
                case "HIS.Desktop.Plugins.PaymentQrCode.LPBankInfo":
                    result = "PAYMENT_QR_CODE_LPBANK";
                    bankType = ProvinceType.LPBANK;
                    break;
                case "HIS.Desktop.Plugins.PaymentQrCode.PVCBInfo":
                    result = "PAYMENT_QR_CODE_PVCB";
                    bankType = ProvinceType.PVCB;
                    break;
                case "DYNAMIC":
                    result = "PAYMENT_QR_CODE_DYNAMIC";
                    bankType = ProvinceType.DYNAMIC;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
