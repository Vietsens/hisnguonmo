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
using MPS.Processor.Mps000115.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000115
{
    public class Mps000115Processor : AbstractProcessor
    {
        Mps000115PDO rdo;

        public Mps000115Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000115PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                List<PrintFullRowData> printFullRowDatas = new List<PrintFullRowData>();
                for (int i = 0; i < 50; i++)
                {
                    printFullRowDatas.Add(new PrintFullRowData(""));
                }

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                objectTag.AddObjectData(store, "Print1", rdo._listInvoiceDetails);
                objectTag.AddObjectData(store, "Print2", rdo._listInvoiceDetails);
                objectTag.AddObjectData(store, "Print3", rdo._listInvoiceDetails);

                objectTag.AddObjectData(store, "PrintRow1", printFullRowDatas);
                objectTag.AddObjectData(store, "PrintRow2", printFullRowDatas);
                objectTag.AddObjectData(store, "PrintRow3", printFullRowDatas);
                objectTag.SetUserFunction(store, "FuncFixPrintFullRow", new FuncFixPrintFullRow(rdo._listInvoiceDetails));

                objectTag.AddObjectData(store, "sereServGroups1", rdo._listInvoiceDetailsADOs);
                objectTag.AddObjectData(store, "sereServGroups2", rdo._listInvoiceDetailsADOs);
                objectTag.AddObjectData(store, "sereServGroups3", rdo._listInvoiceDetailsADOs);

                objectTag.AddObjectData(store, "PayForm1", rdo._payForm);
                objectTag.AddObjectData(store, "PayForm2", rdo._payForm);
                objectTag.AddObjectData(store, "PayForm3", rdo._payForm);

                objectTag.AddObjectData(store, "totalNextPages", rdo._totalNextPageSdos);
                objectTag.AddObjectData(store, "totalNextPages1", rdo._totalNextPageSdos);
                objectTag.AddObjectData(store, "totalNextPages2", rdo._totalNextPageSdos);

                objectTag.AddRelationship(store, "totalNextPages", "sereServGroups1", "Id", "PageId");
                objectTag.AddRelationship(store, "totalNextPages1", "sereServGroups2", "Id", "PageId");
                objectTag.AddRelationship(store, "totalNextPages2", "sereServGroups3", "Id", "PageId");

                store.SetCommonFunctions();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                decimal totalPrice = 0;
                string invoiceDateSeparate = "";
                string createTimeSeparate = "";
                string payFormName = "";
                string templateCode = "";
                string symbolCode = "";
                if (rdo._invoice != null && rdo._listInvoiceDetails != null && rdo._listInvoiceDetails.Count > 0)
                {
                    totalPrice = rdo._listInvoiceDetails.Sum(s => s.VIR_TOTAL_PRICE ?? 0);
                    totalPrice = Math.Round(totalPrice - rdo._invoice.DISCOUNT ?? 0, 0);
                    invoiceDateSeparate = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._invoice.INVOICE_TIME);
                    createTimeSeparate = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm") + "00"));
                    if (rdo._payForm != null)
                    {
                        payFormName = rdo._payForm.FirstOrDefault().PAY_FORM_NAME;
                    }
                    templateCode = rdo._invoice.TEMPLATE_CODE;
                    symbolCode = rdo._invoice.SYMBOL_CODE;

                    string patientPriceText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    string patientPriceSeparate = Inventec.Common.Number.Convert.NumberToStringRoundMax4(totalPrice);
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(patientPriceText);
                    SetSingleKey(new KeyValue(Mps000115ExtendSingleKey.TOTAL_PATIENT_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(patientPriceText)));
                    SetSingleKey(new KeyValue(Mps000115ExtendSingleKey.TOTAl_PATIENT_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(patientPriceText)));
                    SetSingleKey(new KeyValue(Mps000115ExtendSingleKey.TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundMax4(totalPrice)));
                    //AddObjectKeyIntoListkey<V_HIS_INVOICE>(rdo._invoice, false);
                    SetSingleKey(new KeyValue(Mps000115ExtendSingleKey.INVOICE_DATE_SEPARATE, invoiceDateSeparate));
                    SetSingleKey(new KeyValue(Mps000115ExtendSingleKey.CREATE_DATE_SEPARATE, createTimeSeparate));
                    SetSingleKey(new KeyValue(Mps000115ExtendSingleKey.PAY_FORM_NAME, payFormName));
                    SetSingleKey(new KeyValue(Mps000115ExtendSingleKey.TITLE, rdo._titles));
                    SetSingleKey(new KeyValue(Mps000115ExtendSingleKey.CREATOR_USERNAME, rdo.creatorUserName));
                    SetSingleKey(new KeyValue(Mps000115ExtendSingleKey.VIR_TEMPLATE_CODE, templateCode));
                    SetSingleKey(new KeyValue(Mps000115ExtendSingleKey.VIR_SYMBOL_CODE, symbolCode));

                    AddObjectKeyIntoListkey<V_HIS_INVOICE>(rdo._invoice, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                decimal totalPrice = 0;
                totalPrice = rdo._listInvoiceDetails.Sum(s => s.VIR_TOTAL_PRICE ?? 0);
                totalPrice = Math.Round(totalPrice - rdo._invoice.DISCOUNT ?? 0, 0);
                log += "Hoá đơn thanh toán";
                log += string.Format(" TEMPLATE_CODE:{0}", rdo._invoice.TEMPLATE_CODE);
                log += string.Format(" SYMBOL_CODE:{0}", rdo._invoice.SYMBOL_CODE);
                log += string.Format(" VIR_NUM_ORDER:{0}", rdo._invoice.VIR_NUM_ORDER);
                log += string.Format(" Người bán:{0}", rdo._invoice.SELLER_NAME);
                log += string.Format(" Người mua:{0}", rdo._invoice.BUYER_NAME);
                log += string.Format(" Tổng tiền:{0}", totalPrice);

                log = Inventec.Common.String.CountVi.SubStringVi(log, 2000);
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo._invoice != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}", this.printTypeCode, rdo._invoice.TEMPLATE_CODE, rdo._invoice.SYMBOL_CODE, rdo._invoice.VIR_NUM_ORDER, rdo._invoice.ID);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }

    internal class PrintFullRowData
    {
        public string Data { get; set; }

        public PrintFullRowData(string data)
        {
            this.Data = data;
        }
    }

    internal class FuncFixPrintFullRow : TFlexCelUserFunction
    {
        IList list;

        internal FuncFixPrintFullRow(IList list)
        {
            this.list = list;
        }

        public override object Evaluate(object[] parameters)
        {
            bool result = false;
            int count = 0;
            try
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                if (this.list is IList)
                {
                    count = this.list.Count;
                }

                int rowPosition = Convert.ToInt32(parameters[0]);
                int maxRowCount = Convert.ToInt32(parameters[1]);

                if (count < maxRowCount)
                {
                    int rowCountRuntime = maxRowCount - count;
                    if (rowPosition > rowCountRuntime)
                    {
                        result = true;
                    }
                }
                else
                    result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }

            return result;
        }
    }
}
