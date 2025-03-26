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
using Inventec.Common.EBillSoftDreams.Model;
using Inventec.Common.EBillSoftDreams.ModelXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Common.EBillSoftDreams
{
    class EBillSoftDreamsProcessor : IRun
    {
        private DataInitApi DataApi;

        internal EBillSoftDreamsProcessor(DataInitApi DataApi)
        {
            this.DataApi = DataApi;
        }

        Response IRun.Run(object data)
        {
            Response result = new Response();
            try
            {
                if (data.GetType() == typeof(InvCreate))
                {
                    result = Create((InvCreate)data);
                }
                //else if (data.GetType() == typeof(CancelInvoice))
                //{
                //    result = Cancel((CancelInvoice)data);
                //}
                //else if (data.GetType() == typeof(GetFile))
                //{
                //    result = GetFile((GetFile)data);
                //}
                else
                {
                    result.Messages.Add("Error type Data");
                }
            }
            catch (Exception ex)
            {
                result = null;
                throw ex;
            }
            return result;
        }

        private Response Create(InvCreate dataCreateInvoice)
        {
            Response result = new Response();
            try
            {
                if (CheckData(dataCreateInvoice, ref result))
                {

                    string xmlInvData = ConvertListInvoiceToStringXmlFormat(new List<Inv>() { dataCreateInvoice.Inv });

                    CreateInvoice invoice = new CreateInvoice();
                    invoice.Pattern = dataCreateInvoice.Pattern;
                    invoice.Serial = dataCreateInvoice.Serial;
                    invoice.XmlData = xmlInvData;

                    var apiData = new Base.ApiConsumer(DataApi.Address, DataApi.User, DataApi.Pass).PostData<ResultData<CreateInvoiceResult>>(Base.RequestUriStore.ImportInvoice, invoice);
                    if (apiData != null)
                    {
                        if (apiData.Status == 2)
                        {
                            result.Success = true;
                            if (apiData.Data != null && apiData.Data.Invoices != null)
                            {
                                result.Ikey = apiData.Data.Invoices.First().Ikey;
                                if (!String.IsNullOrWhiteSpace(apiData.Data.Invoices.First().No) && apiData.Data.Invoices.First().No != "0")
                                {
                                    result.invoiceNo = apiData.Data.Invoices.First().No;
                                }
                                result.LookupCode = apiData.Data.Invoices.First().LookupCode;
                                result.IssueDate = apiData.Data.Invoices.First().IssueDate;
                            }
                        }
                        else
                        {
                            result.Success = false;
                            result.Messages.Add("ErroCode" + apiData.Status);
                            result.Messages.Add(apiData.Message);
                            if (apiData.Data != null && apiData.Data.KeyInvoiceMsg != null)
                            {
                                foreach (var item in apiData.Data.KeyInvoiceMsg)
                                {
                                    result.Messages.Add(string.Format("{0}({1})", item.Value, item.Key));
                                }
                            }
                        }
                    }
                    else
                    {
                        result.Success = false;
                        result.Messages.Add("Lỗi khi gọi api");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Messages.Add(ex.Message);
            }
            return result;
        }

        private string ConvertListInvoiceToStringXmlFormat(List<Inv> invoices)
        {
            string result = "";
            try
            {
                if (invoices != null)
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(invoices.GetType(), new XmlRootAttribute("Invoices"));

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

        private bool CheckData(InvCreate dataCreateInvoice, ref Response resp)
        {
            bool result = false;
            try
            {
                if (dataCreateInvoice != null)
                {
                    List<string> error = new List<string>();
                    if (dataCreateInvoice.Inv == null || dataCreateInvoice.Inv.Invoice == null || String.IsNullOrEmpty(dataCreateInvoice.Inv.Invoice.Ikey))
                    {
                        error.Add("nội dung");
                    }

                    if (String.IsNullOrEmpty(dataCreateInvoice.Pattern))
                    {
                        error.Add("mẫu");
                    }

                    if (String.IsNullOrEmpty(dataCreateInvoice.Serial))
                    {
                        error.Add("ký hiệu");
                    }

                    if (error.Count > 0)
                    {
                        resp.Success = false;
                        resp.Messages.Add(string.Format("Không xác định được {0} hóa đơn", string.Join(", ", error)));
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    resp.Success = false;
                    resp.Messages.Add("Không xác định được dữ liệu hóa đơn");
                }
            }
            catch (Exception ex)
            {
                result = false;
                resp.Success = false;
                resp.Messages.Add(ex.Message);
            }
            return result;
        }
    }
}
