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
using MPS.Processor.Mps000343.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000343
{
    public class Mps000343Processor : AbstractProcessor
    {
        Mps000343PDO rdo;
        List<Mps000343ADO> listAdo = new List<Mps000343ADO>();
        public Mps000343Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000343PDO)rdoBase;
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
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                ProcessListData();
                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListPrint1", listAdo);
                objectTag.AddObjectData(store, "ListPrint2", listAdo);
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
                if (rdo._Transaction != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);

                    string amountText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo._Transaction.AMOUNT, 0);
                    SetSingleKey(new KeyValue(Mps000343ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, amountText));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000343ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, amountAfterExem));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(amountAfterExem, 4);
                    SetSingleKey(new KeyValue(Mps000343ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000343ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, amountAfterExemText));
                    decimal ratio = ((rdo._Transaction.EXEMPTION ?? 0) * 100) / rdo._Transaction.AMOUNT;
                    SetSingleKey(new KeyValue(Mps000343ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    decimal canthu = rdo._Transaction.AMOUNT - (rdo._Transaction.KC_AMOUNT ?? 0) - (rdo._Transaction.EXEMPTION ?? 0);
                    if ((rdo._Transaction.TDL_BILL_FUND_AMOUNT ?? 0) > 0)
                    {
                        canthu = canthu - (rdo._Transaction.TDL_BILL_FUND_AMOUNT ?? 0);
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                    SetSingleKey(new KeyValue(Mps000343ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu)));
                    SetSingleKey(new KeyValue(Mps000343ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.Number.Convert.NumberToStringRoundAuto(canthu, 4)));

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessListData()
        {
            try
            {
                if (this.rdo._SereServBills != null && this.rdo._SereServBills.Count > 0)
                {
                    foreach (HIS_SERE_SERV_BILL ssBill in this.rdo._SereServBills)
                    {
                        var unit = this.rdo._ServiceUnits != null ? this.rdo._ServiceUnits.FirstOrDefault(o => o.ID == ssBill.TDL_SERVICE_TYPE_ID) : null;
                        Mps000343ADO ado = new Mps000343ADO(ssBill, unit);
                        listAdo.Add(ado);
                    }
                }

                if (this.rdo._BillGoods != null && this.rdo._BillGoods.Count > 0)
                {
                    foreach (HIS_BILL_GOODS billGood in this.rdo._BillGoods)
                    {
                        Mps000343ADO ado = new Mps000343ADO(billGood);
                        listAdo.Add(ado);
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
