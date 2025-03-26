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
using EO.Pdf;
using Inventec.Common.ElectronicBill.Base;
using Inventec.Common.ElectronicBill.MD;
using Inventec.Common.ElectronicBill.PorttalServiceVNPT;
using Inventec.Common.ElectronicBill.WSPublicVNPT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill
{
    public class ElectronicBillProcessor : IRun
    {
        ElectronicBillInput electronicBillInput;
        MappingError mapError;

        const string license_code = "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";

        public ElectronicBillProcessor(ElectronicBillInput _electronicBillInput)
        {
            try
            {
                this.electronicBillInput = _electronicBillInput;
                mapError = new MappingError();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ElectronicBillResult IRun.Run(int cmdType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                switch (cmdType)
                {
                    case CmdType.ImportAndPublishInv:
                        result = this.ImportAndPublishInv();
                        break;
                    case CmdType.downloadInvPDFFkeyNoPay:
                        result = this.downloadInvPDFFkeyNoPay();
                        break;
                    case CmdType.ConvertForStoreFkey:
                        result = this.ConvertForStoreFkey();
                        break;
                    case CmdType.ConvertForVerifyFkey:
                        result = this.ConvertForVerifyFkey();
                        break;
                    case CmdType.DeleteInvFkey:
                        result = this.DeleteInvFKey();
                        break;
                    case CmdType.DeleteInvoiceByFkey:
                        result = this.DeleteInvoiceByFkey();
                        break;
                    case CmdType.CancelInv:
                        result = this.CancelInv();
                        break;
                    case CmdType.CancelInvNoPay:
                        result = this.CancelInvNoPay();
                        break;
                    case CmdType.listInvByCusFkey:
                        result = this.ListInvByCusFkey();
                        break;
                    case CmdType.GetInvErrorViewFkey:
                        result = this.GetInvErrorViewFkey();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = null;
                throw ex;
            }
            return result;
        }

        public ElectronicBillResult ImportAndPublishInv()
        {
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali && !String.IsNullOrEmpty(electronicBillInput.account);
                vali = vali && !String.IsNullOrEmpty(electronicBillInput.acPass);
                vali = vali && !String.IsNullOrEmpty(electronicBillInput.serviceUrl);
                vali = vali && !String.IsNullOrEmpty(electronicBillInput.userName);
                vali = vali && !String.IsNullOrEmpty(electronicBillInput.passWord);
                vali = vali && !String.IsNullOrEmpty(electronicBillInput.serial);
                vali = vali && !String.IsNullOrEmpty(electronicBillInput.pattern);
                vali = vali && (electronicBillInput.convert == 1 || electronicBillInput.convert == 0);
                //vali = vali && ((electronicBillInput.invoices != null && electronicBillInput.invoices.Count > 0)
                //    || (electronicBillInput.invoicesBm != null && electronicBillInput.invoicesBm.Count > 0)
                //    || (electronicBillInput.invoiceTT78s != null && electronicBillInput.invoiceTT78s.Count > 0));
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                if ((electronicBillInput.invoices != null && electronicBillInput.invoices.Count > 0)
                    || (electronicBillInput.invoiceTT78s != null && electronicBillInput.invoiceTT78s.Count > 0))
                {
                    string xmlInvData = "";

                    if (electronicBillInput.invoices != null && electronicBillInput.invoices.Count > 0)
                    {
                        foreach (var item in electronicBillInput.invoices)
                        {
                            item.InvoiceDetail.Amount = FormatReplaceStringPrice(item.InvoiceDetail.Amount);
                            item.InvoiceDetail.Total = FormatReplaceStringPrice(item.InvoiceDetail.Total);
                            item.InvoiceDetail.DiscountAmount = FormatReplaceStringPrice(item.InvoiceDetail.DiscountAmount);
                            item.InvoiceDetail.VATAmount = FormatReplaceStringPrice(item.InvoiceDetail.VATAmount);
                            if (item.InvoiceDetail.Products != null && item.InvoiceDetail.Products.Count > 0)
                            {
                                foreach (var product in item.InvoiceDetail.Products)
                                {
                                    product.Amount = FormatReplaceStringPrice(product.Amount);
                                    product.ProdQuantity = FormatReplaceStringPrice(product.ProdQuantity);
                                    product.ProdPrice = FormatReplaceStringPrice(product.ProdPrice);
                                }
                            }
                        }

                        xmlInvData = ElectronicBillProcessor.ConvertListInvoiceToStringXmlFormat(electronicBillInput.invoices);

                        if (!String.IsNullOrWhiteSpace(electronicBillInput.DataXmlStringPlus))
                        {
                            xmlInvData = xmlInvData.Replace("</Invoice>", electronicBillInput.DataXmlStringPlus + "</Invoice>");
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("xmlInvData", xmlInvData));
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("xmlInvData", xmlInvData));
                        }
                    }
                    else if (electronicBillInput.invoiceTT78s != null && electronicBillInput.invoiceTT78s.Count > 0)
                    {
                        foreach (var invoice in electronicBillInput.invoiceTT78s)
                        {
                            invoice.dLHDon.nDHDon.tToan.TgTCThue = FormatReplaceStringPrice(invoice.dLHDon.nDHDon.tToan.TgTCThue);
                            invoice.dLHDon.nDHDon.tToan.TgTThue = FormatReplaceStringPrice(invoice.dLHDon.nDHDon.tToan.TgTThue);
                            invoice.dLHDon.nDHDon.tToan.TgTTTBSo = FormatReplaceStringPrice(invoice.dLHDon.nDHDon.tToan.TgTTTBSo);
                            invoice.dLHDon.nDHDon.tToan.TTCKTMai = FormatReplaceStringPrice(invoice.dLHDon.nDHDon.tToan.TTCKTMai);

                            foreach (var item in invoice.dLHDon.nDHDon.hHDVu)
                            {
                                item.DGia = FormatReplaceStringPrice(item.DGia);
                                item.SLuong = FormatReplaceStringPrice(item.SLuong, true);
                                item.STCKhau = FormatReplaceStringPrice(item.STCKhau);
                                item.ThTien = FormatReplaceStringPrice(item.ThTien);
                                item.TSThue = FormatReplaceStringPrice(item.TSThue);
                                item.TThue = FormatReplaceStringPrice(item.TThue);
                            }

                            if (invoice.dLHDon.nDHDon.tToan.lTSuat != null && invoice.dLHDon.nDHDon.tToan.lTSuat.Count > 0)
                            {
                                foreach (var item in invoice.dLHDon.nDHDon.tToan.lTSuat)
                                {
                                    item.ThTien = FormatReplaceStringPrice(item.ThTien);
                                    item.TThue = FormatReplaceStringPrice(item.TThue);
                                }
                            }
                        }

                        xmlInvData = ElectronicBillProcessor.ConvertListInvoiceToStringXmlFormat(electronicBillInput.invoiceTT78s, "DSHDon");
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("xmlInvData TT78:", xmlInvData));
                    }

                    ImportAndPublishInvRequestBody body = new ImportAndPublishInvRequestBody();
                    body.Account = electronicBillInput.account;
                    body.ACpass = electronicBillInput.acPass;
                    body.xmlInvData = xmlInvData;
                    body.username = electronicBillInput.userName;
                    body.password = electronicBillInput.passWord;
                    body.pattern = electronicBillInput.pattern;
                    body.serial = electronicBillInput.serial;
                    body.convert = electronicBillInput.convert;

                    BasicHttpBinding binding = new BasicHttpBinding();
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;
                    ImportAndPublishInvRequest request = new ImportAndPublishInvRequest(body);
                    EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/PublishService.asmx");//+"/PublishService.asmx"

                    PublishServiceSoap publishServiceSoap = new PublishServiceSoapClient(binding, epAdd);
                    ImportAndPublishInvResponse response = publishServiceSoap.ImportAndPublishInv(request);
                    string strResponse = "";
                    if (response != null)
                    {
                        strResponse = response.Body.ImportAndPublishInvResult;
                        if (strResponse.Contains("OK"))
                        {
                            result.Success = true;
                            result.Data = strResponse;
                        }
                        else
                        {
                            string message = strResponse;
                            if (mapError.dicMapping.ContainsKey(strResponse))
                            {
                                message += string.Format(" ({0})", mapError.dicMapping[strResponse]);
                            }
                            result.Messages.Add(message);
                        }
                    }
                }
                else if (electronicBillInput.invoicesBm != null && electronicBillInput.invoicesBm.Count > 0)
                {
                    foreach (var item in electronicBillInput.invoicesBm)
                    {
                        item.InvoiceDetailBm.Amount = FormatReplaceStringPrice(item.InvoiceDetailBm.Amount);
                        item.InvoiceDetailBm.Total = FormatReplaceStringPrice(item.InvoiceDetailBm.Total);
                        //item.InvoiceDetailBm.DiscountAmount = FormatReplaceStringPrice(item.InvoiceDetailBm.DiscountAmount);
                        item.InvoiceDetailBm.VATAmount = FormatReplaceStringPrice(item.InvoiceDetailBm.VATAmount);
                        item.InvoiceDetailBm.AmountValue = FormatReplaceStringPrice(item.InvoiceDetailBm.AmountValue);
                        item.InvoiceDetailBm.PayKH = FormatReplaceStringPrice(item.InvoiceDetailBm.PayKH);
                        item.InvoiceDetailBm.RePayKH = FormatReplaceStringPrice(item.InvoiceDetailBm.RePayKH);
                        item.InvoiceDetailBm.TamUng = FormatReplaceStringPrice(item.InvoiceDetailBm.TamUng);
                        if (item.InvoiceDetailBm.Products != null && item.InvoiceDetailBm.Products.Count > 0)
                        {
                            foreach (var product in item.InvoiceDetailBm.Products)
                            {
                                product.Amount = FormatReplaceStringPrice(product.Amount);
                                product.ProdQuantity = FormatReplaceStringPrice(product.ProdQuantity);
                                product.ProdPrice = FormatReplaceStringPrice(product.ProdPrice);
                            }
                        }
                    }

                    string xmlInvData = ElectronicBillProcessor.ConvertListInvoiceToStringXmlFormat(electronicBillInput.invoicesBm);

                    Inventec.Common.Logging.LogSystem.Info("xmlInvDataBM:\r\n" + xmlInvData);

                    VNPTBachMai.ImportAndPublishInvRequestBody body = new VNPTBachMai.ImportAndPublishInvRequestBody();
                    body.Account = electronicBillInput.account;
                    body.ACpass = electronicBillInput.acPass;
                    body.xmlInvData = xmlInvData;
                    body.username = electronicBillInput.userName;
                    body.password = electronicBillInput.passWord;
                    body.pattern = electronicBillInput.pattern;
                    body.serial = electronicBillInput.serial;
                    body.convert = electronicBillInput.convert;

                    BasicHttpBinding binding = new BasicHttpBinding();
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;
                    VNPTBachMai.ImportAndPublishInvRequest request = new VNPTBachMai.ImportAndPublishInvRequest(body);

                    EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/PublishService.asmx");//+"/PublishService.asmx"

                    VNPTBachMai.PublishServiceSoap publishServiceSoap = new VNPTBachMai.PublishServiceSoapClient(binding, epAdd);
                    VNPTBachMai.ImportAndPublishInvResponse response = publishServiceSoap.ImportAndPublishInv(request);
                    string strResponse = "";
                    if (response != null)
                    {
                        strResponse = response.Body.ImportAndPublishInvResult;
                        if (strResponse.Contains("OK"))
                        {
                            result.Success = true;
                            result.Data = strResponse;

                            VNPTBachMai.UpdateCusRequestBody cusBody = new VNPTBachMai.UpdateCusRequestBody();
                            cusBody.username = electronicBillInput.userName;
                            cusBody.pass = electronicBillInput.passWord;
                            cusBody.XMLCusData = ProcessCustomerXml(electronicBillInput.invoicesBm);
                            if (!String.IsNullOrWhiteSpace(cusBody.XMLCusData))
                            {
                                var reqCus = new VNPTBachMai.UpdateCusRequest(cusBody);
                                var cus = publishServiceSoap.UpdateCus(reqCus);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cus), cus));
                            }
                        }
                        else
                        {
                            string message = strResponse;
                            if (mapError.dicMapping.ContainsKey(strResponse))
                            {
                                message += string.Format(" ({0})", mapError.dicMapping[strResponse]);
                            }
                            result.Messages.Add(message);
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Messages.Add(ex.Message);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => electronicBillInput), electronicBillInput));
            }
            return result;
        }

        private string ProcessCustomerXml(List<Invoice_BM> list)
        {
            string result = "";
            try
            {
                List<Customer> listCus = new List<Customer>();
                foreach (var item in list)
                {
                    Customer cus = new Customer();
                    cus.Address = item.InvoiceDetailBm.CusAddress;
                    cus.Code = item.InvoiceDetailBm.CusCode;
                    cus.Name = item.InvoiceDetailBm.CusName;
                    cus.Phone = item.InvoiceDetailBm.CusPhone;
                    cus.TaxCode = item.InvoiceDetailBm.CusTaxCode;
                    cus.BankAccountName = "";
                    cus.BankName = "";
                    cus.BankNumber = "";
                    cus.ContactPerson = "";
                    cus.CusType = 0;
                    cus.Email = " ";
                    cus.Fax = "";
                    cus.RepresentPerson = "";
                    listCus.Add(cus);
                }

                result = ConvertListCustomerToStringXmlFormat(listCus);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public ElectronicBillResult DeleteInvFKey()
        {
            Inventec.Common.Logging.LogSystem.Info("DeleteInvFKey");
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.account);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.acPass);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.serviceUrl);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.userName);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.passWord);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.pattern);
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                //Convert list invoice to string xml format
                deleteInvFkeyRequestBody body = new deleteInvFkeyRequestBody();
                body.Account = electronicBillInput.account;
                body.ACpass = electronicBillInput.acPass;
                body.pattern = electronicBillInput.pattern;
                body.userName = electronicBillInput.userName;
                body.pass = electronicBillInput.passWord;
                body.lsFkey = electronicBillInput.fKey;

                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                deleteInvFkeyRequest request = new deleteInvFkeyRequest(body);
                EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/PublishService.asmx");//+"/PublishService.asmx"

                PublishServiceSoap publishServiceSoap = new PublishServiceSoapClient(binding, epAdd);
                deleteInvFkeyResponse response = publishServiceSoap.deleteInvFkey(request);
                string strResponse = "";
                if (response != null)
                {
                    strResponse = response.Body.deleteInvFkeyResult;
                    if (strResponse.Contains("OK"))
                    {
                        result.Success = true;
                        result.Data = strResponse;
                    }
                    else
                    {
                        string message = strResponse;
                        if (mapError.dicMapping.ContainsKey(strResponse))
                        {
                            message += string.Format(" ({0})", mapError.dicMapping[strResponse]);
                        }
                        result.Messages.Add(message);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //result = null;
            }
            return result;
        }

        public ElectronicBillResult DeleteInvoiceByFkey()
        {
            Inventec.Common.Logging.LogSystem.Info("DeleteInvoiceByFkey");
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.account);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.acPass);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.serviceUrl);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.userName);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.passWord);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.pattern);
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                //Convert list invoice to string xml format
                VNPTNinhThuan.deleteInvoiceByFkeyRequestBody body = new VNPTNinhThuan.deleteInvoiceByFkeyRequestBody();
                body.Account = electronicBillInput.account;
                body.ACpass = electronicBillInput.acPass;
                //body.pattern = electronicBillInput.pattern;
                body.username = electronicBillInput.userName;
                body.password = electronicBillInput.passWord;
                body.lstFkey = electronicBillInput.fKey;

                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                VNPTNinhThuan.deleteInvoiceByFkeyRequest request = new VNPTNinhThuan.deleteInvoiceByFkeyRequest(body);
                EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/PublishService.asmx");//+"/PublishService.asmx"

                VNPTNinhThuan.PublishServiceSoap publishServiceSoap = new VNPTNinhThuan.PublishServiceSoapClient(binding, epAdd);
                VNPTNinhThuan.deleteInvoiceByFkeyResponse response = publishServiceSoap.deleteInvoiceByFkey(request);
                string strResponse = "";
                if (response != null)
                {
                    strResponse = response.Body.deleteInvoiceByFkeyResult;
                    if (strResponse.Contains("OK"))
                    {
                        result.Success = true;
                        result.Data = strResponse;
                    }
                    else
                    {
                        string message = strResponse;
                        if (mapError.dicMapping.ContainsKey(strResponse))
                        {
                            message += string.Format(" ({0})", mapError.dicMapping[strResponse]);
                        }
                        result.Messages.Add(message);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //result = null;
            }
            return result;
        }

        public ElectronicBillResult CancelInv()
        {
            Inventec.Common.Logging.LogSystem.Info("CancelInv");
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.account);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.acPass);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.serviceUrl);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.userName);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.passWord);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.pattern);
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                //Convert list invoice to string xml format
                BusinessserviceVNPT.cancelInvRequestBody body = new BusinessserviceVNPT.cancelInvRequestBody();
                body.Account = electronicBillInput.account;
                body.ACpass = electronicBillInput.acPass;
                body.userName = electronicBillInput.userName;
                body.userPass = electronicBillInput.passWord;
                body.fkey = electronicBillInput.fKey;

                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                BusinessserviceVNPT.cancelInvRequest request = new BusinessserviceVNPT.cancelInvRequest(body);
                EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/businessservice.asmx");//+"/PublishService.asmx"

                BusinessserviceVNPT.BusinessServiceSoap businessServiceSoap = new BusinessserviceVNPT.BusinessServiceSoapClient(binding, epAdd);
                BusinessserviceVNPT.cancelInvResponse response = businessServiceSoap.cancelInv(request);
                string strResponse = "";
                if (response != null)
                {
                    strResponse = response.Body.cancelInvResult;
                    if (strResponse.Contains("OK"))
                    {
                        result.Success = true;
                        result.Data = strResponse;
                    }
                    else
                    {
                        string message = strResponse;
                        if (mapError.dicMapping.ContainsKey(strResponse))
                        {
                            message += string.Format(" ({0})", mapError.dicMapping[strResponse]);
                        }
                        result.Messages.Add(message);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //result = null;
            }
            return result;
        }

        public ElectronicBillResult CancelInvNoPay()
        {
            Inventec.Common.Logging.LogSystem.Info("CancelInvNoPay");
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.account);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.acPass);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.serviceUrl);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.userName);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.passWord);
                vali = vali & !String.IsNullOrEmpty(electronicBillInput.pattern);
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                //Convert list invoice to string xml format
                BusinessserviceVNPT.cancelInvNoPayRequestBody body = new BusinessserviceVNPT.cancelInvNoPayRequestBody();
                body.Account = electronicBillInput.account;
                body.ACpass = electronicBillInput.acPass;
                body.userName = electronicBillInput.userName;
                body.userPass = electronicBillInput.passWord;
                body.fkey = electronicBillInput.fKey;

                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                BusinessserviceVNPT.cancelInvNoPayRequest request = new BusinessserviceVNPT.cancelInvNoPayRequest(body);
                EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/businessservice.asmx");//+"/PublishService.asmx"

                BusinessserviceVNPT.BusinessServiceSoap businessServiceSoap = new BusinessserviceVNPT.BusinessServiceSoapClient(binding, epAdd);
                BusinessserviceVNPT.cancelInvNoPayResponse response = businessServiceSoap.cancelInvNoPay(request);
                string strResponse = "";
                if (response != null)
                {
                    strResponse = response.Body.cancelInvNoPayResult;
                    if (strResponse.Contains("OK"))
                    {
                        result.Success = true;
                        result.Data = strResponse;
                    }
                    else
                    {
                        string message = strResponse;
                        if (mapError.dicMapping.ContainsKey(strResponse))
                        {
                            message += string.Format(" ({0})", mapError.dicMapping[strResponse]);
                        }
                        result.Messages.Add(message);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //result = null;
            }
            return result;
        }

        public static string FormatReplaceStringPrice(string price, bool removeDecimal = false)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(price))
                {
                    result = price.Replace(',', '.');

                    //làm tròn dữ liệu số lượng lên và bỏ phần thập phân
                    if (removeDecimal && result.Contains("."))
                    {
                        decimal dataPrice = 0;
                        if (decimal.TryParse(price, out dataPrice))
                        {
                            dataPrice = Math.Round(dataPrice, 0, MidpointRounding.AwayFromZero);
                            long newlongData = 0;
                            if (long.TryParse(dataPrice.ToString(), out newlongData))
                            {
                                result = newlongData.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }

        public ElectronicBillResult downloadInvPDFFkeyNoPay()
        {
            Inventec.Common.Logging.LogSystem.Info("downloadInvPDFFkeyNoPay");
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.fKey);
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.userName);
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.passWord);
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                //Convert invoice to string xml format
                downloadInvPDFFkeyNoPayRequestBody body = new downloadInvPDFFkeyNoPayRequestBody();
                body.fkey = this.electronicBillInput.fKey;
                body.userName = this.electronicBillInput.userName;
                body.userPass = this.electronicBillInput.passWord;
                downloadInvPDFFkeyNoPayRequest request = new downloadInvPDFFkeyNoPayRequest(body);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.MaxBufferSize = Int32.MaxValue;
                EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/PortalService.asmx");

                PortalServiceSoap porttal = new PortalServiceSoapClient(binding, epAdd);
                downloadInvPDFFkeyNoPayResponse response = porttal.downloadInvPDFFkeyNoPay(request);
                string strResponse = "";
                if (response != null)
                {
                    strResponse = response.Body.downloadInvPDFFkeyNoPayResult;
                    if (strResponse.Contains("OK"))
                    {
                        result.Success = true;
                        result.Data = strResponse;
                        string fullFileName = ProcessPdfStringResult(strResponse);
                        result.InvoiceLink = fullFileName;
                    }
                    else
                    {
                        string message = strResponse;
                        if (mapError.dicMapping.ContainsKey(strResponse))
                        {
                            message += string.Format(" ({0})", mapError.dicMapping[strResponse]);
                        }
                        result.Messages.Add(message);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
                //result = null;
            }

            return result;
        }

        public ElectronicBillResult ConvertForStoreFkey()
        {
            Inventec.Common.Logging.LogSystem.Info("ConvertForStoreFkey");
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.fKey);
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.userName);
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.passWord);
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                //Convert invoice to string xml format
                convertForStoreFkeyRequestBody body = new convertForStoreFkeyRequestBody();
                body.fkey = this.electronicBillInput.fKey;
                body.userName = this.electronicBillInput.userName;
                body.userPass = this.electronicBillInput.passWord;
                convertForStoreFkeyRequest request = new convertForStoreFkeyRequest(body);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.MaxBufferSize = Int32.MaxValue;
                EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/PortalService.asmx");

                PortalServiceSoap porttal = new PortalServiceSoapClient(binding, epAdd);
                convertForStoreFkeyResponse response = porttal.convertForStoreFkey(request);
                string strResponse = "";
                if (response != null)
                {
                    strResponse = response.Body.convertForStoreFkeyResult;
                    if (!strResponse.StartsWith("ERR:"))
                    {
                        result.Success = true;
                        result.Data = strResponse;
                        string fullFileName = ProcessPdfFromHtmlFileResult(strResponse);
                        result.InvoiceLink = fullFileName;
                    }
                    else
                    {
                        string message = strResponse;
                        if (mapError.dicMapping.ContainsKey(strResponse))
                        {
                            message += string.Format(" ({0})", mapError.dicMapping[strResponse]);
                        }
                        result.Messages.Add(message);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
                //result = null;
            }

            return result;
        }

        public ElectronicBillResult GetInvErrorViewFkey()
        {
            Inventec.Common.Logging.LogSystem.Info("GetInvErrorViewFkey");
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.fKey);
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.userName);
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.passWord);
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                //Convert invoice to string xml format
                GetInvErrorViewFkeyRequestBody body = new GetInvErrorViewFkeyRequestBody();
                body.fkey = this.electronicBillInput.fKey;
                body.userName = this.electronicBillInput.userName;
                body.userPass = this.electronicBillInput.passWord;
                GetInvErrorViewFkeyRequest request = new GetInvErrorViewFkeyRequest(body);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.MaxBufferSize = Int32.MaxValue;
                EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/PortalService.asmx");

                PortalServiceSoap porttal = new PortalServiceSoapClient(binding, epAdd);
                GetInvErrorViewFkeyResponse response = porttal.GetInvErrorViewFkey(request);
                string strResponse = "";
                if (response != null)
                {
                    strResponse = response.Body.GetInvErrorViewFkeyResult;
                    if (!strResponse.StartsWith("ERR:"))
                    {
                        result.Success = true;
                        result.Data = strResponse;
                        string fullFileName = ProcessPdfFromHtmlFileResult(strResponse);
                        result.InvoiceLink = fullFileName;
                    }
                    else
                    {
                        string message = strResponse;
                        if (mapError.dicMapping.ContainsKey(strResponse))
                        {
                            message += string.Format(" ({0})", mapError.dicMapping[strResponse]);
                        }
                        result.Messages.Add(message);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
                //result = null;
            }

            return result;
        }

        public ElectronicBillResult ConvertForVerifyFkey()
        {
            Inventec.Common.Logging.LogSystem.Info("ConvertForVerifyFkey");
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.fKey);
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.userName);
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.passWord);
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                //Convert invoice to string xml format
                convertForVerifyFkeyRequestBody body = new convertForVerifyFkeyRequestBody();
                body.fkey = this.electronicBillInput.fKey;
                body.userName = this.electronicBillInput.userName;
                body.userPass = this.electronicBillInput.passWord;
                convertForVerifyFkeyRequest request = new convertForVerifyFkeyRequest(body);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.MaxBufferSize = Int32.MaxValue;
                EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/PortalService.asmx");

                PortalServiceSoap porttal = new PortalServiceSoapClient(binding, epAdd);
                convertForVerifyFkeyResponse response = porttal.convertForVerifyFkey(request);
                string strResponse = "";
                if (response != null)
                {
                    strResponse = response.Body.convertForVerifyFkeyResult;
                    if (!strResponse.StartsWith("ERR:"))
                    {
                        result.Success = true;
                        result.Data = strResponse;
                        string fullFileName = ProcessPdfFromHtmlFileResult(strResponse);
                        result.InvoiceLink = fullFileName;
                    }
                    else
                    {
                        string message = strResponse;
                        if (mapError.dicMapping.ContainsKey(strResponse + ".1"))
                        {
                            message += string.Format(" ({0})", mapError.dicMapping[strResponse + ".1"]);
                        }
                        result.Messages.Add(message);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
                //result = null;
            }

            return result;
        }

        private string ProcessPdfFromHtmlFileResult(string strHtmlResponse)
        {
            string result = "";
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessPdfFromHtmlFileResult EO.Base.Runtime");
                string tempFileName = Path.GetTempFileName();
                string htmlName = tempFileName.Replace("tmp", "html");
                if (File.Exists(htmlName))
                {
                    File.Delete(htmlName);
                }
                File.WriteAllText(htmlName, strHtmlResponse);

                string pdfFileName = tempFileName.Replace("tmp", "pdf");
                if (File.Exists(pdfFileName))
                {
                    File.Delete(pdfFileName);
                }

                EO.Base.Runtime.EnableEOWP = true;
                EO.Pdf.Runtime.AddLicense(license_code);

                HtmlToPdf.Options.UsePrintMedia = true;

                float x = 0.2f;

                HtmlToPdf.Options.PageSize = EO.Pdf.PdfPageSizes.A4;
                HtmlToPdf.Options.OutputArea = new System.Drawing.RectangleF(x, x, EO.Pdf.PdfPageSizes.A4.Width - x * 2, EO.Pdf.PdfPageSizes.A4.Height - x * 2);

                HtmlToPdf.ConvertHtml(strHtmlResponse, pdfFileName);

                result = pdfFileName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private float GetFirstWidth(string strHtmlResponse)
        {
            float result = 0;
            if (!String.IsNullOrWhiteSpace(strHtmlResponse))
            {
                //lấy vị trí width đầu tiên
                int idx = strHtmlResponse.IndexOf("width");

                string dataWidth = strHtmlResponse.Substring(idx + 5, strHtmlResponse.IndexOf(";", idx) - idx - 5);

                string intData = "";
                int lastindex = 0;
                for (int i = 0; i < dataWidth.Length; i++)
                {
                    if (Char.IsNumber(dataWidth[i]))
                    {
                        intData += dataWidth[i];
                        lastindex = i;
                    }
                    else if (lastindex > 0)
                    {
                        break;
                    }
                }

                float.TryParse(intData, out result);
            }
            return result;
        }

        private string ProcessPdfStringResult(string base64string)
        {
            string result = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(base64string))
                {
                    byte[] data = System.Convert.FromBase64String(base64string);
                    result = ProcessPdfFileResult(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
                result = "";
            }
            return result;
        }

        private string ProcessPdfFileResult(byte[] fileToBytes)
        {
            string result = "";
            try
            {
                string tempFileName = Path.GetTempFileName();
                tempFileName = tempFileName.Replace("tmp", "pdf");
                try
                {
                    File.WriteAllBytes(tempFileName, fileToBytes);
                    result = tempFileName;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ex);
                    result = "";
                }
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }

        //internal static string ConvertListInvoiceToStringXmlFormat(List<Invoice> invoices)
        //{
        //    string result = "";
        //    try
        //    {
        //        if (invoices != null && invoices.Count > 0)
        //        {
        //            XmlSerializer xmlSerializer = new XmlSerializer(invoices.GetType(),
        //                new XmlRootAttribute("Invoices"));

        //            using (var sww = new StringWriter())
        //            {
        //                using (XmlWriter writer = XmlWriter.Create(sww))
        //                {
        //                    xmlSerializer.Serialize(writer, invoices);
        //                    result = sww.ToString(); // Your XML
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = "";
        //    }
        //    return result;
        //}

        internal static string ConvertListInvoiceToStringXmlFormat(object invoices, string elementName)
        {
            string result = "";
            try
            {
                if (invoices != null)
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(invoices.GetType(),
                        new XmlRootAttribute(elementName));

                    using (var sww = new StringWriter())
                    {
                        var xmlNamespaces = new XmlSerializerNamespaces();
                        xmlNamespaces.Add("", "");
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            xmlSerializer.Serialize(writer, invoices, xmlNamespaces);
                            result = sww.ToString(); // Your XML
                            result = result.Substring(result.IndexOf("?>") + 2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static string ConvertListInvoiceToStringXmlFormat(object invoices)
        {
            return ConvertListInvoiceToStringXmlFormat(invoices, "Invoices");
        }

        internal string ConvertListCustomerToStringXmlFormat(List<Customer> customer)
        {
            string result = "";
            try
            {
                if (customer != null && customer.Count > 0)
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(customer.GetType(),
                        new XmlRootAttribute("Customers"));

                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            xmlSerializer.Serialize(writer, customer);
                            result = sww.ToString(); // Your XML
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }

        public ElectronicBillResult ListInvByCusFkey()
        {
            Inventec.Common.Logging.LogSystem.Info("GetInvViewFkeyNoPay");
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.fKey);
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.userName);
                vali = vali & !String.IsNullOrEmpty(this.electronicBillInput.passWord);
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                //Convert invoice to string xml format
                listInvByCusFkeyRequestBody body = new listInvByCusFkeyRequestBody();
                body.key = this.electronicBillInput.fKey;
                body.userName = this.electronicBillInput.userName;
                body.userPass = this.electronicBillInput.passWord;
                listInvByCusFkeyRequest request = new listInvByCusFkeyRequest(body);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.MaxBufferSize = Int32.MaxValue;
                EndpointAddress epAdd = new EndpointAddress(electronicBillInput.serviceUrl + "/PortalService.asmx");

                PortalServiceSoap porttal = new PortalServiceSoapClient(binding, epAdd);
                listInvByCusFkeyResponse response = porttal.listInvByCusFkey(request);
                string strResponse = "";
                if (response != null)
                {
                    strResponse = response.Body.listInvByCusFkeyResult;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strResponse), strResponse));
                    if (strResponse.Contains("<Data><Item>"))
                    {
                        result.Success = true;

                        ListInv listInvByCusFkey = ConvertStringXMLToObject<ListInv>(strResponse);
                        InvByCusFkey invByCusFkey = listInvByCusFkey.Item != null && listInvByCusFkey.Item.Count > 0 ? listInvByCusFkey.Item.First() : new InvByCusFkey();

                        //OK: pattern;serial1-key1_num1,key2_num12,key3_num3…
                        result.Data = string.Format("ok:{0};{1}-{2}_{3}", invByCusFkey.pattern, invByCusFkey.serial, this.electronicBillInput.fKey, invByCusFkey.invNum);
                        Inventec.Common.Logging.LogSystem.Info("result.Data: " + result.Data);
                    }
                    else
                    {
                        string message = strResponse;
                        if (mapError.dicMapping.ContainsKey(strResponse))
                        {
                            message += string.Format(" ({0})", mapError.dicMapping[strResponse]);
                        }
                        result.Messages.Add(message);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
                //result = null;
            }

            return result;
        }

        private T ConvertStringXMLToObject<T>(string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var textReader = new StringReader(xml))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }
    }
}
