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
using FlexCel.Report;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000152.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000152
{
    public class Mps000152Processor : AbstractProcessor
    {
        Mps000152PDO rdo;
        public Mps000152Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000152PDO)rdoBase;
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);

                objectTag.AddObjectData(store, "sereServGroups1", rdo._listInvoiceDetailsADOs);
                objectTag.AddObjectData(store, "sereServGroups2", rdo._listInvoiceDetailsADOs);

                objectTag.AddObjectData(store, "PayForm1", rdo._payForm);
                objectTag.AddObjectData(store, "PayForm2", rdo._payForm);

                objectTag.AddObjectData(store, "totalNextPages", rdo._totalNextPageSdos);
                objectTag.AddObjectData(store, "totalNextPages1", rdo._totalNextPageSdos);
                store.SetCommonFunctions();
                objectTag.AddRelationship(store, "totalNextPages", "sereServGroups1", "Id", "PageId");
                objectTag.AddRelationship(store, "totalNextPages1", "sereServGroups2", "Id", "PageId");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                decimal totalPrice = 0;
                string payFormName;
                if (rdo._invoice != null && rdo._listInvoiceDetailsADOs != null && rdo._listInvoiceDetailsADOs.Count > 0 && rdo._payForm != null)
                {
                    totalPrice = rdo._listInvoiceDetailsADOs.Sum(s => s.VIR_TOTAL_PRICE ?? 0);
                    totalPrice = Math.Round(totalPrice - rdo._invoice.DISCOUNT ?? 0, 0);
                    payFormName = rdo._payForm.FirstOrDefault().PAY_FORM_NAME;


                    string patientPriceText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    string patientPriceSeparate = Inventec.Common.Number.Convert.NumberToStringRoundMax4(totalPrice);
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(patientPriceText);
                    SetSingleKey(new KeyValue(Mps000152ExtendSingleKey.TOTAL_PATIENT_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(patientPriceText)));
                    SetSingleKey(new KeyValue(Mps000152ExtendSingleKey.TOTAl_PATIENT_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(patientPriceText)));
                    SetSingleKey(new KeyValue(Mps000152ExtendSingleKey.TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundMax4(totalPrice)));
                    SetSingleKey(new KeyValue(Mps000152ExtendSingleKey.PAY_FORM_NAME, payFormName));

                    //SetSingleKey(new KeyValue(Mps000152ExtendSingleKey.TITLE, Inventec.Common.String.Convert.CurrencyToVneseString(_titles[i])));
                    AddObjectKeyIntoListkey<V_HIS_INVOICE>(rdo._invoice,  false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
