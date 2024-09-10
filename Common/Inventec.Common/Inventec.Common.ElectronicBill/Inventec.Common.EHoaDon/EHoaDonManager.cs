using Inventec.Common.EHoaDon.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EHoaDon
{
    public class EHoaDonManager
    {

        EHoaDon.ServiceType serviceType { get; set; }
        BkavPartner bkavPartner { get; set; }
        List<InvoiceDataWS> invoiceDataWSs { get; set; }

        string serviceUrl { get; set; }
        public EHoaDonManager(string serviceUrl, BkavPartner bkavPartner, List<InvoiceDataWS> invoiceDataWSs)
        {
            try
            {
                this.serviceType = serviceType;
                this.bkavPartner = bkavPartner;
                this.invoiceDataWSs = invoiceDataWSs;
                this.serviceUrl = serviceUrl;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<InvoiceResult> Run(int cmdType)
        {
            List<InvoiceResult> result = new List<InvoiceResult>();
            IRun iRun = null;
            try
            {
                if (this.Check(ref result))
                {

                    iRun = new EHoaDonProcessor(this.serviceUrl, this.bkavPartner, this.invoiceDataWSs);
                    result = iRun != null ? iRun.Run(cmdType) : null;
                }
            }
            catch (Exception)
            {
                result = null;
                throw;
            }
            return result;
        }


        private bool Check(ref List<InvoiceResult> invoiceResults)
        {
            bool result = true;
            try
            {
                InvoiceResult invoiceResult = new InvoiceResult();
                if (String.IsNullOrEmpty(this.serviceUrl))
                {
                    invoiceResult.Status = -1;
                    invoiceResult.MessLog = "" + ResultCode.LOI_KHONG_THAY_WEBSERVICE;
                    invoiceResults.Add(invoiceResult);
                    return false;
                }
                //Check bkavPartner
                
                if (this.bkavPartner == null || String.IsNullOrEmpty(this.bkavPartner.BkavPartnerGUID) || String.IsNullOrEmpty(this.bkavPartner.BkavPartnerToken))
                {
                    invoiceResult.Status = -1;
                    invoiceResult.MessLog = "" + ResultCode.LOI_TRUYEN_THIEU_BKAV_PARTNER;
                    invoiceResults.Add(invoiceResult);
                    return false;
                }
                if (this.invoiceDataWSs == null || this.invoiceDataWSs.Count == 0)
                {
                    invoiceResult.Status = -1;
                    invoiceResult.MessLog = "" + ResultCode.INVOICE_DATA_NULL;
                    invoiceResults.Add(invoiceResult);
                    return false;
                }

                //foreach (var item in this.invoiceDataWSs)
                //{
                //    if (item.Invoice == null || item.ListInvoiceDetailsWS == null
                //        || (item.PartnerInvoiceID == 0 && String.IsNullOrEmpty(item.PartnerInvoiceStringID)))
                //    {
                //        invoiceResult.Status = -1;
                //        invoiceResult.MessLog = "" + ResultCode.INVOICE_DATA_NULL;
                //        invoiceResults.Add(invoiceResult);
                //        return false;
                //    }
                //}

            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }
    }
}
