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
using BSECUS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EHoaDon
{
    public class EHoaDonProcessor : IRun
    {
        WSPublicEHoaDon.WSPublicEHoaDon eHDService;
        RemoteCommand remoteCommand = null;
        List<InvoiceDataWS> invoiceDataWSs { get; set; }
        public EHoaDonProcessor(string serviceUrl, BkavPartner bkavPartner, List<InvoiceDataWS> invoiceDataWSs)
        {
            try
            {
                this.invoiceDataWSs = invoiceDataWSs;
                eHDService = new WSPublicEHoaDon.WSPublicEHoaDon();
                eHDService.Url = serviceUrl;
                remoteCommand = new RemoteCommand(eHDService.ExecuteCommand, bkavPartner.BkavPartnerGUID, bkavPartner.BkavPartnerToken, bkavPartner.Mode);
            }
            catch (Exception)
            {

                throw;
            }
        }

        List<InvoiceResult> IRun.Run(int cmdType)
        {
            List<InvoiceResult> result = new List<InvoiceResult>(); ;
            try
            {
                switch (cmdType)
                {
                    case CmdType.CreateInvoiceMT:
                    case CmdType.CreateInvoiceTR:
                    case CmdType.CreateInvoiceWithFormSerial:
                    case CmdType.CreateInvoiceWithFormSerialNo:
                    case CmdType.CreateInvoiceWithFormSerialReturnNo:
                        string sGUID = null;
                        result = DoCreateInvoice(cmdType, out sGUID);
                        break;
                    case CmdType.GetInvoiceLink:
                    case CmdType.GetInvoiceShow:
                    case CmdType.CancelInvoiceByPartnerInvoiceID:
                    case CmdType.DeleteInvoiceByPartnerInvoiceID:
                        result = ProcessInvoiceByParentInvoiceId(cmdType);
                        break;
                    default:
                        
                        break;
                }

            }
            catch (Exception)
            {
                result = null;
                throw;
            }
            return result;
        }

        List<InvoiceResult> DoCreateInvoice(int cmdType, out string sGUID)
        {
            List<InvoiceResult> invoiceResults = new List<InvoiceResult>();
            string msg = "";
            sGUID = null;

            string list = null;
            msg = GetListInvoiceDataWS(cmdType, out list);
            if (msg.Length > 0)
            {
                InvoiceResult invoiceResult = new InvoiceResult();
                invoiceResult.Status = 1;
                invoiceResult.MessLog = msg;
                invoiceResults.Add(invoiceResult);
                return invoiceResults;
            }

            Result result = null;
            msg = remoteCommand.TransferCommandAndProcessResult(cmdType, list, out result);
            if (msg.Length > 0)
            {
                InvoiceResult invoiceResult = new InvoiceResult();
                invoiceResult.Status = 1;
                invoiceResult.MessLog = msg;
                invoiceResults.Add(invoiceResult);
                return invoiceResults;
            }

            // Không có lỗi, Hệ thống trả ra danh sách kết quả của Hóa đơn

            msg = Convertor.StringToObject(false, Convert.ToString(result.Object), out invoiceResults);
            if (msg.Length > 0)
            {
                InvoiceResult invoiceResult = new InvoiceResult();
                invoiceResult.Status = 1;
                invoiceResult.MessLog = msg;
                invoiceResults.Add(invoiceResult);
                return invoiceResults;
            }


            foreach (InvoiceResult item in invoiceResults)
            {
                if (!String.IsNullOrEmpty(msg))
                {
                    item.MessLog = msg + ";" + item.MessLog;
                }
            }

            return invoiceResults;
        }

        List<InvoiceResult> ProcessInvoiceByParentInvoiceId(int cmdType)
        {
            List<InvoiceResult> invoiceResults = new List<InvoiceResult>();
            string msg = "";
            try
            {
                string list = null;
                msg = Convertor.ObjectToString<List<InvoiceDataWS>>(false, this.invoiceDataWSs, out list);

                Result result = null;
                msg = remoteCommand.TransferCommandAndProcessResult(cmdType, list, out result);
                if (msg.Length > 0)
                {
                    InvoiceResult invoiceResult = new InvoiceResult();
                    invoiceResult.Status = 1;
                    invoiceResult.MessLog = msg;
                    invoiceResults.Add(invoiceResult);
                    return invoiceResults;
                }

                // Không có lỗi, Hệ thống trả ra danh sách kết quả của Hóa đơn
                msg = Convertor.StringToObject(false, Convert.ToString(result.Object), out invoiceResults);
                if (msg.Length > 0)
                {
                    InvoiceResult invoiceResult = new InvoiceResult();
                    invoiceResult.Status = 1;
                    invoiceResult.MessLog = msg;
                    invoiceResults.Add(invoiceResult);
                    return invoiceResults;
                }
            }
            catch (Exception ex)
            {

            }
            return invoiceResults;
        }

        string GetListInvoiceDataWS(int commandType, out string list)
        {
            list = null;
            PrepareInvoiceData(commandType);
            return Convertor.ObjectToString<List<InvoiceDataWS>>(false, this.invoiceDataWSs, out list);
        }

        void PrepareInvoiceData(int commandType)
        {
            // Một tờ Hóa đơn được lưu trong 1 object InvoiceDataWS. Object này có 4 properties Invoice, ListInvoiceDetails, IsSetInvoiceNo, TransactionID
            // Header của tờ Hóa đơn (như thông tin người bán, người mua...) được lưu trong Invoice
            // Chi tiết của tờ Hóa đơn (là các item hàng hóa, dịch vụ) được lưu trong InvoiceDetails. Nhiều item được lưu trong ListInvoiceDetails

            // Tạo Header của tờ Hóa đơn
            foreach (var item in this.invoiceDataWSs)
            {
                item.Invoice = GetOneInvoiceWS(commandType, item.Invoice);
            }
        }

        /// <summary>
        /// Khởi tạo dữ ilệu InvoiceWS
        /// </summary>
        /// <returns></returns>
        InvoiceWS GetOneInvoiceWS(int commandType, InvoiceWS invoice)
        {
            InvoiceWS invoiceWS = invoice;

            switch (commandType)
            {
                case CmdType.CreateInvoiceMT:
                    invoiceWS.InvoiceNo = 0;
                    invoiceWS.InvoiceForm = "";
                    invoiceWS.InvoiceSerial = ""; break;
                case CmdType.CreateInvoiceTR:
                    invoiceWS.InvoiceNo = 0;
                    invoiceWS.InvoiceForm = "";
                    invoiceWS.InvoiceSerial = ""; break;
                case CmdType.CreateInvoiceWithFormSerial:
                case CmdType.CreateInvoiceWithFormSerialNo:
                case CmdType.CreateInvoiceWithFormSerialReturnNo:
                    break;
                //case CmdType.CreateInvoiceWithFormSerial:
                //    invoiceWS.InvoiceNo = 0;
                //    invoiceWS.InvoiceForm = "01GTKT0/003";
                //    invoiceWS.InvoiceSerial = "AB/17E"; break;
                //case CmdType.CreateInvoiceWithFormSerialNo:
                //    invoiceWS.InvoiceNo = 12034;
                //    invoiceWS.InvoiceForm = "01GTKT0/003";
                //    invoiceWS.InvoiceSerial = "AB/17E"; break;
                default:
                    invoiceWS.InvoiceNo = 0;
                    invoiceWS.InvoiceForm = "";
                    invoiceWS.InvoiceSerial = "";
                    break;
            }
            return invoiceWS;
        }
    }
}
