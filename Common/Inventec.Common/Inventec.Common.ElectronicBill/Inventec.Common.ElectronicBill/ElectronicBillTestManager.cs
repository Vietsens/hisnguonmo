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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.IO;
using Inventec.Common.ElectronicBill.MD;
using Inventec.Common.ElectronicBill.Base;
using Inventec.Common.ElectronicBill.WSPublicVNPT;

namespace Inventec.Common.ElectronicBill
{
    public static class ElectronicBillTestManager
    {
        public static ElectronicBillResult ImportAndPublishInv(string _account, string _acPass, List<Invoice> _invoices, string _pattern, string _serial, string _userName, string _passWord, int _convert)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            result.Success = false;
            result.Messages = new List<string>();
            bool vali = true;
            try
            {
                vali = vali & !String.IsNullOrEmpty(_account);
                vali = vali & !String.IsNullOrEmpty(_acPass);
                vali = vali & _invoices != null & _invoices.Count > 0;
                vali = vali & !String.IsNullOrEmpty(_userName);
                vali = vali & !String.IsNullOrEmpty(_passWord);
                vali = vali & !String.IsNullOrEmpty(_serial);
                vali = vali & !String.IsNullOrEmpty(_pattern);
                vali = vali & (_convert == 1 || _convert == 0);
                if (!vali)
                {
                    result.Messages.Add(ResultCode.WRONG_DATA);
                    return result;
                }

                foreach (var item in _invoices)
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

                //Convert list invoice to string xml format
                string xmlInvData = ElectronicBillProcessor.ConvertListInvoiceToStringXmlFormat(_invoices);

                ImportAndPublishInvRequestBody body = new ImportAndPublishInvRequestBody();
                body.Account = _account;
                body.ACpass = _acPass;
                body.xmlInvData = xmlInvData;
                body.username = _userName;
                body.password = _passWord;
                body.pattern = _pattern;
                body.serial = _serial;
                body.convert = _convert;


                ImportAndPublishInvRequest request = new ImportAndPublishInvRequest(body);
                PublishServiceSoap publishServiceSoap = new PublishServiceSoapClient();
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
                        result.Messages.Add(strResponse);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                //result = null;
            }
            return result;
        }

        public static string FormatReplaceStringPrice(string price)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(price))
                {
                    Char[] priceChar = price.ToCharArray();
                    foreach (var item in priceChar)
                    {
                        string charReplace = "";
                        if (item == ',')
                        {
                            charReplace = ".";
                        }
                        else
                        {
                            charReplace = item.ToString();
                        }
                        result += charReplace;
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }
    }
}
