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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProxyBehavior.CTO.Model;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProxyBehavior.CTO
{
    class CTOBehavior : IRun
    {
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }
        string proxyUrl { get; set; }

        public CTOBehavior(ElectronicBillDataInput _electronicBillDataInput, string _config)
            : base()
        {
            this.ElectronicBillDataInput = _electronicBillDataInput;
            if (!String.IsNullOrWhiteSpace(_config))
            {
                string[] cfg = _config.Split('|');
                if (cfg.Count() > 1)
                {
                    proxyUrl = "";
                    for (int i = 1; i < cfg.Length; i++)
                    {
                        proxyUrl += cfg[i];
                    }
                }
            }
        }

        ElectronicBillResult IRun.Run(ElectronicBillType.ENUM _electronicBillTypeEnum, TemplateEnum.TYPE templateType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (this.Check(_electronicBillTypeEnum, ref result))
                {
                    switch (_electronicBillTypeEnum)
                    {
                        case ElectronicBillType.ENUM.CREATE_INVOICE:
                            result = PrcessCreateInvoice(templateType);
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                            result = ProcessGetInvoice();
                            break;
                        case ElectronicBillType.ENUM.DELETE_INVOICE:
                        case ElectronicBillType.ENUM.CANCEL_INVOICE:
                            result = ProcessCancelInvoice();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ElectronicBillResultUtil.Set(ref result, false, ex.Message);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool Check(ElectronicBillType.ENUM _electronicBillTypeEnum, ref ElectronicBillResult result)
        {
            bool valid = true;
            try
            {
                if (this.ElectronicBillDataInput == null)
                    throw new Exception("Không có dữ liệu phát hành");

                if (_electronicBillTypeEnum == ElectronicBillType.ENUM.CREATE_INVOICE)
                {
                    if (this.ElectronicBillDataInput == null)
                        throw new Exception("Không có dữ liệu phát hành hóa đơn.");
                    if (this.ElectronicBillDataInput.Treatment == null)
                        throw new Exception("Không có thông tin hồ sơ điều trị.");
                    if (this.ElectronicBillDataInput.Transaction == null && (this.ElectronicBillDataInput.ListTransaction == null || this.ElectronicBillDataInput.ListTransaction.Count == 0))
                        throw new Exception("Không có dữ liệu hóa đơn");
                    if (this.ElectronicBillDataInput.Transaction == null && this.ElectronicBillDataInput.ListTransaction != null && this.ElectronicBillDataInput.ListTransaction.Count > 0)
                        throw new Exception("Chưa hỗ trợ xuất hóa đơn gộp");
                    if (this.ElectronicBillDataInput.Branch == null)
                        throw new Exception("Không có thông tin chi nhánh.");
                }
            }
            catch (Exception ex)
            {
                valid = false;
                ElectronicBillResultUtil.Set(ref result, false, ex.Message);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private ElectronicBillResult ProcessCancelInvoice()
        {
            ElectronicBillResult result = new ElectronicBillResult();
            CancelTransactionInvoiceRequest data = new CancelTransactionInvoiceRequest();
            data.additionalReferenceDate = (ElectronicBillDataInput.CancelTime ?? 0).ToString();
            data.additionalReferenceDesc = ElectronicBillDataInput.CancelReason;
            data.id = ElectronicBillDataInput.InvoiceCode;
            data.invoiceNo = ElectronicBillDataInput.ENumOrder;
            data.strIssueDate = ElectronicBillDataInput.TransactionTime.ToString();
            data.supplierTaxCode = this.ElectronicBillDataInput.Branch.TAX_CODE;
            data.templateCode = ElectronicBillDataInput.TemplateCode;

            var apiresult = ApiConsumer.CreateRequest<ResponeData>(proxyUrl, UriStore.cancel_invoice, data);
            if (apiresult != null)
            {
                if (apiresult.status == "success" && apiresult.value != null)
                {
                    string fullFileName = ProcessPdfFileResult((byte[])apiresult.value);
                    result.Success = true;
                    result.InvoiceSys = ProviderType.CTO;
                    result.Messages = new List<string>() { apiresult.message };
                }
                else
                {
                    ElectronicBillResultUtil.Set(ref result, false, string.Format("{0}. {1}", apiresult.code, apiresult.message));
                    LogSystem.Error("Lay file hoa don that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                }
            }
            else
            {
                ElectronicBillResultUtil.Set(ref result, false, "Không nhận được phản hồi từ hệ thống proxy");
                LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
            }

            return result;
        }

        private ElectronicBillResult ProcessGetInvoice()
        {
            Inventec.Common.Logging.LogSystem.Debug("CTOBehavior___ProcessGetInvoice()");
            ElectronicBillResult result = new ElectronicBillResult();
            FileRequest data = new FileRequest();

            data.fileType = "PDF";
            data.id = ElectronicBillDataInput.InvoiceCode;
            data.invoiceNo = ElectronicBillDataInput.ENumOrder;
            data.supplierTaxCode = this.ElectronicBillDataInput.Branch.TAX_CODE;
            data.templateCode = ElectronicBillDataInput.TemplateCode;

            var apiresult = ApiConsumer.CreateRequest<ResponeData>(proxyUrl, UriStore.get_invoice, data);
            if (apiresult != null)
            {
                if (apiresult.status == "success" && apiresult.value != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("value: " + apiresult.value);
                    string fullFileName = ProcessPdfFileResult(System.Convert.FromBase64String(apiresult.value.ToString()));
                    result.Success = true;
                    result.InvoiceSys = ProviderType.CTO;
                    result.InvoiceLink = fullFileName;
                }
                else
                {
                    ElectronicBillResultUtil.Set(ref result, false, string.Format("{0}. {1}", apiresult.code, apiresult.message));
                    LogSystem.Error("Lay file hoa don that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                }
            }
            else
            {
                ElectronicBillResultUtil.Set(ref result, false, "Không nhận được phản hồi từ hệ thống proxy");
                LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
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
                    result = "";
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ElectronicBillResult PrcessCreateInvoice(TemplateEnum.TYPE templateType)
        {
            Inventec.Common.Logging.LogSystem.Debug("CTOBehavior___PrcessCreateInvoice()");
            ElectronicBillResult result = new ElectronicBillResult();

            DateTime createTime = DateTime.Now;
            EBill data = new EBill();
            data.currency = ElectronicBillDataInput.Currency;
            data.issued_date = createTime.ToString("yyyy-MM-ddTHH:mm:sszzz");

            if (ElectronicBillDataInput.Treatment != null)
            {
                data.patient_id = ElectronicBillDataInput.Treatment.TDL_PATIENT_CODE;
                data.encounter = ElectronicBillDataInput.Treatment.TREATMENT_CODE;
            }

            string payFormName = "";
            string cashierName = "";
            string code = "";
            if (ElectronicBillDataInput.Transaction != null)
            {
                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == ElectronicBillDataInput.Transaction.PAY_FORM_ID);
                payFormName = payForm != null ? payForm.PAY_FORM_CODE : " ";
                cashierName = ElectronicBillDataInput.Transaction.CASHIER_USERNAME;
                code = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
            }

            data.id = code;
            data.creator = cashierName;
            data.method = payFormName;
            data.status = "issued";

            int buyerType = -1;

            if (ElectronicBillDataInput.Transaction != null)
            {
                buyerType = ElectronicBillDataInput.Transaction.BUYER_TYPE ?? -1;
            }
            else if (ElectronicBillDataInput.ListTransaction != null && ElectronicBillDataInput.ListTransaction.Count > 0)
            {
                var type = ElectronicBillDataInput.ListTransaction.Where(o => o.BUYER_TYPE.HasValue).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                if (type != null)
                {
                    buyerType = type.BUYER_TYPE ?? -1;
                }
            }

            string buyerAddress = null;
            string buyerName = null;
            string buyerTax = null;
            string companyName = null;
            string companyAddress = " ";
            string companyTax = " ";

            InvoiceInfo.InvoiceInfoADO adoInfo = InvoiceInfo.InvoiceInfoProcessor.GetData(this.ElectronicBillDataInput, false);

            buyerAddress = adoInfo.BuyerAddress ?? " ";
            buyerName = adoInfo.BuyerName;
            buyerTax = adoInfo.BuyerTaxCode ?? " ";
            companyName = adoInfo.BuyerOrganization ?? " ";
            if (buyerType == 2)
            {
                companyAddress = adoInfo.BuyerAddress ?? " ";
                companyTax = adoInfo.BuyerTaxCode ?? " ";
            }

            Inventec.Common.Logging.LogSystem.Debug("__buyerType: " + buyerType);
            switch (buyerType)
            {
                case 1:
                    data.buyer = new EbInfo();
                    data.buyer.address = buyerAddress;
                    data.buyer.name = buyerName;
                    data.buyer.tax = buyerTax;
                    data.company = null;
                    break;
                case 2:
                    data.buyer = new EbInfo();
                    data.buyer.address = " ";
                    data.buyer.tax = " ";
                    data.buyer.name = buyerName;
                    data.company = new EbInfo();
                    data.company.name = companyName;
                    data.company.address = companyAddress;
                    data.company.tax = companyTax;
                    break;
                default:
                    data.buyer = new EbInfo();
                    data.buyer.address = buyerAddress;
                    data.buyer.name = buyerName;
                    data.buyer.tax = buyerTax;

                    if (!String.IsNullOrWhiteSpace(companyName) || !String.IsNullOrWhiteSpace(companyAddress) || !String.IsNullOrWhiteSpace(companyTax))
                    {
                        data.company = new EbInfo();
                        data.company.name = companyName;
                        data.company.address = companyAddress;
                        data.company.tax = companyTax;
                    }
                    break;
            }

            data.template = new EbTemplate();
            data.template.id = ElectronicBillDataInput.TemplateCode;
            data.template.symbol = ElectronicBillDataInput.SymbolCode;
            data.template.type = GetInvoiceType(ElectronicBillDataInput.TemplateCode);

            data.items = new List<EbProduct>();

            IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(templateType, ElectronicBillDataInput);
            var listProduct = iRunTemplate.Run();
            if (listProduct == null)
            {
                throw new Exception("Lỗi thông tin dịch vụ");
            }

            if (listProduct.GetType() == typeof(List<ProductBase>))
            {
                List<ProductBase> listProductBase = (List<ProductBase>)listProduct;

                if (listProductBase == null || listProductBase.Count == 0)
                {
                    throw new Exception("Lỗi thông tin dịch vụ");
                }

                foreach (var item in listProductBase)
                {
                    EbProduct product = new EbProduct();
                    product.name = item.ProdName;
                    product.quantity = (long)(item.ProdQuantity ?? 0);
                    product.tax = 0;
                    product.unit = item.ProdUnit;
                    product.amount = item.Amount;
                    product.total = item.Amount;
                    product.price = item.ProdPrice ?? 0;

                    data.items.Add(product);
                }
            }
            else if (listProduct.GetType() == typeof(List<ProductBasePlus>))
            {
                List<ProductBasePlus> listProductBase = (List<ProductBasePlus>)listProduct;

                if (listProductBase == null || listProductBase.Count == 0)
                {
                    throw new Exception("Lỗi thông tin dịch vụ");
                }

                foreach (var item in listProductBase)
                {
                    EbProduct product = new EbProduct();
                    product.name = item.ProdName;
                    product.quantity = Math.Round((double)(item.ProdQuantity ?? 0), 4);
                    product.tax = 0;
                    product.unit = item.ProdUnit;
                    product.amount = item.Amount;
                    product.total = item.Amount;
                    product.price = item.ProdPrice ?? 0;

                    data.items.Add(product);
                }
            }

            data.total_gross = data.items.Sum(s => s.total);
            data.total_net = data.items.Sum(s => s.amount);
            data.amount_inwords = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(data.total_gross))) + "đồng"; ;
            data.vat = new EbVat();

            var apiresult = ApiConsumer.CreateRequest<ResponeData>(proxyUrl, UriStore.publish_invoice, data);
            if (apiresult != null)
            {
                if (apiresult.status == "success")
                {
                    result.Success = true;
                    result.InvoiceSys = ProviderType.CTO;
                    result.InvoiceCode = data.id;
                    result.InvoiceNumOrder = apiresult.value != null ? apiresult.value.ToString() : "";
                    result.InvoiceTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(createTime);
                }
                else
                {
                    ElectronicBillResultUtil.Set(ref result, false, string.Format("{0}. {1}", apiresult.code, apiresult.message));
                    LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data) + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiresult), apiresult));
                }
            }
            else
            {
                ElectronicBillResultUtil.Set(ref result, false, "Không nhận được phản hồi từ hệ thống proxy");
                LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
            }
            return result;
        }

        //Mã loại hóa đơn chỉ nhận các giá trị sau: 01GTKT, 02GTTT, 07KPTQ, 03XKNB, 04HGDL, 01BLP
        private string GetInvoiceType(string p)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(p))
                {
                    string num = p.Substring(0, 2);
                    if (num == "01")
                    {
                        string tp = p.Substring(0, 5);
                        if (tp == "01BLP")
                        {
                            result = "01BLP";
                        }
                        else
                        {
                            result = "01GTKT";
                        }
                    }
                    else if (num == "02")
                    {
                        result = "02GTTT";
                    }
                    else if (num == "03")
                    {
                        result = "03XKNB";
                    }
                    else if (num == "04")
                    {
                        result = "04HGDL";
                    }
                    else if (num == "07")
                    {
                        result = "07KPTQ";
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
    }
}
