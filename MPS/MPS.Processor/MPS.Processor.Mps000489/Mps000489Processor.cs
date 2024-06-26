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
using MPS.Processor.Mps000489.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000489
{
    public class Mps000489Processor : AbstractProcessor
    {
        Mps000489PDO rdo;
        public Mps000489Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            if (rdoBase != null && rdoBase is Mps000489PDO)
            {
                rdo = (Mps000489PDO)rdoBase;
            }
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

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                AddObjectKeyIntoListkey<V_HIS_DEPOSIT_REQ>(rdo.DepositReq, false);
                SetSingleKey();
                this.SetSignatureKeyImageByCFG();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        void SetSingleKey()
        {
            try
            {
                if (rdo.DepositReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000489ExtendSingleKey.TRANSACTION_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.DepositReq.TRANSACTION_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000489ExtendSingleKey.YEAR_STR, rdo.DepositReq.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000489ExtendSingleKey.AMOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToString(rdo.DepositReq.TRANSACTION_AMOUNT ?? 0))));
                    SetSingleKey(new KeyValue(Mps000489ExtendSingleKey.TRANSACTION_AMOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToString(rdo.DepositReq.TRANSACTION_AMOUNT ?? 0))));
                    SetSingleKey(new KeyValue(Mps000489ExtendSingleKey.TRANSACTION_AMOUNT_STR, Inventec.Common.Number.Convert.NumberToString(rdo.DepositReq.TRANSACTION_AMOUNT ?? 0)));

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
