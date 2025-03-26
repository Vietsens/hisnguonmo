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
using Inventec.Common.ElectronicBillMoit.ModelXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBillMoit
{
    public class BillMoitManager
    {
        DataInitApi DataApi;

        public BillMoitManager(DataInitApi dataApi)
        {
            this.DataApi = dataApi;
        }

        public InvoiceResult CreateInvoice(List<Invoice> _invoices, string _pattern, string _serial, int _convert)
        {
            InvoiceResult result = new InvoiceResult();
            try
            {
                bool vali = true;

                Inventec.Common.Logging.LogSystem.Info("CreateInvoice");

                vali = vali & _invoices != null & _invoices.Count > 0;
                vali = vali & !String.IsNullOrEmpty(_serial);
                vali = vali & !String.IsNullOrEmpty(_pattern);
                vali = vali & (_convert == 1 || _convert == 0);
                vali = vali & (CheckDataApi(DataApi));
                if (!vali)
                {
                    result.MessLog = "Thieu du lieu bat buoc ";
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _invoices), _invoices));
                    Inventec.Common.Logging.LogSystem.Debug("____pattern|" + _pattern + "_______serial|" + _serial + "_______convert|" + _convert);
                    return result;
                }

                foreach (var item in _invoices)
                {
                    item.InvoiceDetail.Amount = FormatReplaceStringPrice(item.InvoiceDetail.Amount);
                    item.InvoiceDetail.Total = FormatReplaceStringPrice(item.InvoiceDetail.Total);
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

                string xmlInvData = ConvertListInvoiceToStringXmlFormat(_invoices);

                Model.CreateInvoice invoice = new Model.CreateInvoice();
                invoice.convert = _convert == 1 ? true : false;
                invoice.pattern = _pattern;
                invoice.serial = _serial;
                invoice.xmlData = xmlInvData;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));

                var apiData = new Base.ApiConsumer(DataApi.Address, DataApi.User, DataApi.Pass, DataApi.SupplierTaxCode).PostData<Model.ApiResultData>(Base.RequestUriStore.CreateInvoice, invoice);
                if (apiData != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiData), apiData));

                    result.Status = apiData.success;
                    result.MessLog = apiData.error + " " + apiData.messages;
                    if (apiData.data != null)
                    {
                        string dt = Newtonsoft.Json.JsonConvert.SerializeObject(apiData.data);
                        List<Model.ResultDataDetail> datas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Model.ResultDataDetail>>(dt);

                        if (datas != null && datas.Count > 0)
                        {
                            result.Key = datas.First().key;
                            result.fKey = datas.First().fkey;
                            result.NumOrder = datas.First().no;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckDataApi(DataInitApi DataApi)
        {
            bool result = true;
            try
            {
                if (DataApi == null) throw new Exception("DataInitApi null");

                if (String.IsNullOrWhiteSpace(DataApi.Address)) throw new Exception("Address is null");

                if (String.IsNullOrWhiteSpace(DataApi.User)) throw new Exception("User is null");

                if (String.IsNullOrWhiteSpace(DataApi.Pass)) throw new Exception("Pass is null");

                if (String.IsNullOrWhiteSpace(DataApi.SupplierTaxCode)) throw new Exception("SupplierTaxCode is null");
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public InvoiceResult CancelInvoice(string _pattern, string Key)
        {
            InvoiceResult result = new InvoiceResult();
            try
            {
                if (string.IsNullOrWhiteSpace(_pattern) || string.IsNullOrWhiteSpace(Key))
                {
                    throw new Exception("key hoac pattern khong duoc de trong " + _pattern + "_" + Key);
                }

                if (!CheckDataApi(DataApi))
                {
                    throw new Exception("DataApi khong dung");
                }

                Inventec.Common.Logging.LogSystem.Info("CancelInvoice");

                Model.CommonData invoices = new Model.CommonData();
                invoices.fkey = Key;
                invoices.pattern = _pattern;

                var apiData = new Base.ApiConsumer(DataApi.Address, DataApi.User, DataApi.Pass, DataApi.SupplierTaxCode).PostData<Model.ApiResultData>(Base.RequestUriStore.CancelInvoice, invoices);
                if (apiData != null)
                {
                    result.Status = apiData.success;
                    result.MessLog = apiData.error + " " + apiData.messages;
                }

                if (!apiData.success)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoices), invoices));
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.MessLog = "Loi trong qua trinh huy";
                result.PdfFile = "";
                result.Key = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public InvoiceResult GetFileInvoice(string _pattern, string Key)
        {
            InvoiceResult result = new InvoiceResult();
            try
            {
                if (string.IsNullOrWhiteSpace(_pattern) || string.IsNullOrWhiteSpace(Key))
                {
                    throw new Exception("key hoac pattern khong duoc de trong " + _pattern + "_" + Key);
                }

                if (!CheckDataApi(DataApi))
                {
                    throw new Exception("DataApi khong dung");
                }

                Inventec.Common.Logging.LogSystem.Info("GetFileInvoice");

                Model.CommonData invoices = new Model.CommonData();
                invoices.fkey = Key;
                invoices.pattern = _pattern;

                var apiData = new Base.ApiConsumer(DataApi.Address, DataApi.User, DataApi.Pass, DataApi.SupplierTaxCode).GetData<Model.ApiResultData>(Base.RequestUriStore.GetFileInvoice, invoices);
                if (apiData != null)
                {
                    result.Status = apiData.success;
                    result.MessLog = apiData.error + " " + apiData.messages;
                    result.PdfFile = apiData.data.ToString();
                }

                if (!apiData.success)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoices), invoices));
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.MessLog = "Loi trong qua trinh lay file";
                result.PdfFile = "";
                result.Key = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string ConvertListInvoiceToStringXmlFormat(List<Invoice> invoices)
        {
            string result = "";
            try
            {
                if (invoices != null && invoices.Count > 0)
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(invoices.GetType(),
                        new XmlRootAttribute("Invoices"));

                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            xmlSerializer.Serialize(writer, invoices);
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

        /// <summary>
        /// "," ==> "."
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public string FormatReplaceStringPrice(string price)
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

        public InvoiceResult CreateInvoiceTT78(List<Invoice> _invoices, string _pattern, string _serial, int _convert)
        {
            InvoiceResult result = new InvoiceResult();
            try
            {
                bool vali = true;

                Inventec.Common.Logging.LogSystem.Info("CreateInvoiceTT78");

                vali = vali & _invoices != null & _invoices.Count > 0;
                vali = vali & !String.IsNullOrEmpty(_serial);
                vali = vali & !String.IsNullOrEmpty(_pattern);
                vali = vali & (_convert == 1 || _convert == 0);
                vali = vali & (CheckDataApi(DataApi));
                if (!vali)
                {
                    result.MessLog = "Thieu du lieu bat buoc ";
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _invoices), _invoices));
                    Inventec.Common.Logging.LogSystem.Debug("____pattern|" + _pattern + "_______serial|" + _serial + "_______convert|" + _convert);
                    return result;
                }

                foreach (var item in _invoices)
                {
                    item.InvoiceDetail.Amount = FormatReplaceStringPrice(item.InvoiceDetail.Amount);
                    item.InvoiceDetail.Total = FormatReplaceStringPrice(item.InvoiceDetail.Total);
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

                string xmlInvData = ConvertListInvoiceToStringXmlFormat(_invoices);

                Model.CreateInvoice invoice = new Model.CreateInvoice();
                invoice.convert = _convert == 1 ? true : false;
                invoice.pattern = _pattern;
                invoice.serial = _serial;
                invoice.xmlData = xmlInvData;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));

                var apiData = new Base.ApiConsumer(DataApi.Address, DataApi.User, DataApi.Pass, DataApi.SupplierTaxCode).PostData<Model.ApiResultData>(Base.RequestUriStore.CreateInvoiceTT78, invoice);
                if (apiData != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiData), apiData));

                    result.Status = apiData.success;
                    result.MessLog = apiData.error + " " + apiData.messages;
                    if (apiData.data != null)
                    {
                        string dt = Newtonsoft.Json.JsonConvert.SerializeObject(apiData.data);
                        List<Model.ResultDataDetail> datas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Model.ResultDataDetail>>(dt);

                        if (datas != null && datas.Count > 0)
                        {
                            result.Key = datas.First().key;
                            result.fKey = datas.First().fkey;
                            result.NumOrder = datas.First().no;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public InvoiceResult GetFileInvoiceTT78(string _pattern, string _serial, string Key)
        {
            InvoiceResult result = new InvoiceResult();
            try
            {
                if (string.IsNullOrWhiteSpace(_pattern) || string.IsNullOrWhiteSpace(Key))
                {
                    throw new Exception("key hoac pattern khong duoc de trong " + _pattern + "_" + Key);
                }

                if (!CheckDataApi(DataApi))
                {
                    throw new Exception("DataApi khong dung");
                }

                Inventec.Common.Logging.LogSystem.Info("GetFileInvoiceTT78");

                Model.CommonData invoices = new Model.CommonData();
                invoices.fkey = Key;
                invoices.pattern = _pattern;
                invoices.serial = _serial;

                var apiData = new Base.ApiConsumer(DataApi.Address, DataApi.User, DataApi.Pass, DataApi.SupplierTaxCode).GetData<Model.ApiResultData>(Base.RequestUriStore.GetFileInvoiceTT78, invoices);
                if (apiData != null)
                {
                    result.Status = apiData.success;
                    result.MessLog = apiData.error + " " + apiData.messages;
                    result.PdfFile = apiData.data.ToString();
                }

                if (!apiData.success)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoices), invoices));
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.MessLog = "Loi trong qua trinh lay file";
                result.PdfFile = "";
                result.Key = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public InvoiceResult CancelInvoiceTT78(string _pattern, string _serial, string Key)
        {
            InvoiceResult result = new InvoiceResult();
            try
            {
                if (string.IsNullOrWhiteSpace(_pattern) || string.IsNullOrWhiteSpace(Key))
                {
                    throw new Exception("key hoac pattern khong duoc de trong " + _pattern + "_" + Key);
                }

                if (!CheckDataApi(DataApi))
                {
                    throw new Exception("DataApi khong dung");
                }

                Inventec.Common.Logging.LogSystem.Info("CancelInvoiceTT78");

                Model.CommonData invoices = new Model.CommonData();
                invoices.fkey = Key;
                //invoices.pattern = _pattern;
                //invoices.serial = _serial;

                var apiData = new Base.ApiConsumer(DataApi.Address, DataApi.User, DataApi.Pass, DataApi.SupplierTaxCode).PostData<Model.ApiResultData>(Base.RequestUriStore.DeleteInvoiceTT78, invoices);
                if (apiData != null)
                {
                    result.Status = apiData.success;
                    result.MessLog = apiData.error + " " + apiData.messages;
                }

                if (!apiData.success)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoices), invoices));
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.MessLog = "Loi trong qua trinh huy";
                result.PdfFile = "";
                result.Key = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
