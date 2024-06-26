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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.Processor.Mps000361.PDO;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System.IO;
using HIS.Desktop.LocalStorage.ConfigApplication;

namespace MPS.Processor.Mps000361
{
    class Mps000361Processor : AbstractProcessor
    {
        Mps000361PDO rdo;
        public Mps000361Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000361PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();

                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
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

                if (rdo._Treatment != null)
                {
                    V_HIS_TRANSACTION transaction = new V_HIS_TRANSACTION();
                    decimal directlyBillingPrice = 0;
                    if (rdo.ListTransaction != null && rdo.ListTransaction.Count > 0)
                    {
                        transaction = rdo.ListTransaction.Where(o => !o.SALE_TYPE_ID.HasValue && o.IS_DIRECTLY_BILLING != 1).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault(o => o.TRANSACTION_TIME >= rdo._Treatment.OUT_TIME.Value);
                        if (transaction == null) transaction = rdo.ListTransaction.OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();

                        var listDirectlyBilling = rdo.ListTransaction.Where(o => o.IS_DIRECTLY_BILLING == 1).ToList();
                        directlyBillingPrice = listDirectlyBilling.Sum(s => s.AMOUNT);
                    }

                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(rdo._Treatment, false);
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(transaction, false);
                    AddObjectKeyIntoListkey<V_HIS_DEPARTMENT>(rdo._department, false);
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.DBO_DATE, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Treatment.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.CURRENT_TIME_SEPARATE_BEGIN_TIME_STR, HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentDateSeparateFullTime()));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.CURRENT_MONTH_SEPARATE_STR, HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentMonthSeparate()));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.CURRENT_MONTH_STR, HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentMonth()));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.CURRENT_TIME_SEPARATE_STR, HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentTime()));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR, HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentDateSeparate()));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_PRICE_STR, Inventec.Common.Number.Convert.NumberToString(rdo._Treatment.TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator)));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_HEIN_PRICE_STR, Inventec.Common.Number.Convert.NumberToString(rdo._Treatment.TOTAL_HEIN_PRICE ?? 0, ConfigApplications.NumberSeperator)));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT_STR, Inventec.Common.Number.Convert.NumberToString(rdo._Treatment.TOTAL_DEPOSIT_AMOUNT ?? 0, ConfigApplications.NumberSeperator)));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_PATIENT_PRICE_STR, Inventec.Common.Number.Convert.NumberToString(rdo._Treatment.TOTAL_PATIENT_PRICE ?? 0, ConfigApplications.NumberSeperator)));
                    decimal TotalReturn = ((rdo._Treatment.TOTAL_DEPOSIT_AMOUNT ?? 0) - (rdo._Treatment.TOTAL_PATIENT_PRICE ?? 0)) < 0 ? 0 : ((rdo._Treatment.TOTAL_DEPOSIT_AMOUNT ?? 0) - (rdo._Treatment.TOTAL_PATIENT_PRICE ?? 0));

                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_RETURN_STR, Inventec.Common.Number.Convert.NumberToString(TotalReturn, ConfigApplications.NumberSeperator)));
                    decimal TotalPrice = (rdo._Treatment.TOTAL_DISCOUNT ?? 0) + (rdo._Treatment.TOTAL_HEIN_PRICE ?? 0) + (rdo._Treatment.TOTAL_PATIENT_PRICE ?? 0);
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_PRICE_STR_1, Inventec.Common.Number.Convert.NumberToString(TotalPrice, ConfigApplications.NumberSeperator)));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_BILL_AMOUNT_STR, Inventec.Common.Number.Convert.NumberToString(rdo._Treatment.TOTAL_BILL_AMOUNT ?? 0,
ConfigApplications.NumberSeperator)));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_REPAY_AMOUNT_STR, Inventec.Common.Number.Convert.NumberToString(rdo._Treatment.TOTAL_REPAY_AMOUNT ?? 0, ConfigApplications.NumberSeperator)));
                    SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_DIRECTLY_BILL_PRICE, Inventec.Common.Number.Convert.NumberToString(directlyBillingPrice, ConfigApplications.NumberSeperator)));

                    if (transaction != null && !String.IsNullOrEmpty(transaction.TRANSACTION_INFO))
                    {
                        var transactionInfo = JsonConvert.DeserializeObject<TransactionInfoSDO>(transaction.TRANSACTION_INFO);
                        AddObjectKeyIntoListkey<TransactionInfoSDO>(transactionInfo, false);
                        SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_HEIN_PRICE_INFO, transactionInfo.TOTAL_HEIN_PRICE ?? 0));
                        SetSingleKey(new KeyValue(Mps000361ExtendSingleKey.TOTAL_HEIN_PRICE_INFO_STR, Inventec.Common.Number.Convert.NumberToString(transactionInfo.TOTAL_HEIN_PRICE ?? 0, ConfigApplications.NumberSeperator)));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
