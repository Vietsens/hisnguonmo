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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using HIS.Desktop.LocalStorage.LocalData;

namespace MPS.Core.Mps000152
{
    public class Mps000152RDO : RDOBase
    {
        private V_HIS_INVOICE _invoice = null;

        internal List<ADO.InvoiceDetailADO> _listInvoiceDetailsADOs = null;

        internal List<HIS_INVOICE_DETAIL> _listInvoiceDetails = null;

        internal List<MPS.ADO.TotalNextPage> _totalNextPageSdos = null;

        internal List<string> _titles = null;
        internal List<HIS_PAY_FORM> _payForm = null;


        public Mps000152RDO(V_HIS_INVOICE invoice, List<HIS_INVOICE_DETAIL> listInvoiceDetails, List<ADO.InvoiceDetailADO> listInvoiceDetailsADOs, List<MPS.ADO.TotalNextPage> totalNextPageSdos, List<string> titles, List<HIS_PAY_FORM> payForm)
        {
            try
            {
                _invoice = invoice;
                _listInvoiceDetailsADOs = listInvoiceDetailsADOs;
                _totalNextPageSdos = totalNextPageSdos;
                _listInvoiceDetails = listInvoiceDetails;
                _titles = titles;
                _payForm = payForm;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                decimal totalPrice = 0;
                string payFormName;
                if (this._invoice != null && this._listInvoiceDetailsADOs != null && this._listInvoiceDetailsADOs.Count > 0 && this._payForm!= null)
                {
                        totalPrice = this._listInvoiceDetailsADOs.Sum(s => s.VIR_TOTAL_PRICE ?? 0);
                        totalPrice = Math.Round(totalPrice - this._invoice.DISCOUNT ?? 0, 0);
                        payFormName = this._payForm.FirstOrDefault().PAY_FORM_NAME;


                        string patientPriceText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                        string patientPriceSeparate = Inventec.Common.Number.Convert.NumberToStringRoundMax4(totalPrice);
                        string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(patientPriceText);
                        keyValues.Add(new KeyValue(Mps000152ExtendSingleKey.TOTAL_PATIENT_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(patientPriceText)));
                        keyValues.Add(new KeyValue(Mps000152ExtendSingleKey.TOTAl_PATIENT_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(patientPriceText)));
                        keyValues.Add(new KeyValue(Mps000152ExtendSingleKey.TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundMax4(totalPrice)));
                        keyValues.Add(new KeyValue(Mps000152ExtendSingleKey.PAY_FORM_NAME, payFormName));

                        //keyValues.Add(new KeyValue(Mps000152ExtendSingleKey.TITLE, Inventec.Common.String.Convert.CurrencyToVneseString(_titles[i])));
                        GlobalQuery.AddObjectKeyIntoListkey<V_HIS_INVOICE>(this._invoice, this.keyValues, false);           
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
