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
using MPS.Processor.Mps000342.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000342
{
    public class Mps000342Processor : AbstractProcessor
    {
        Mps000342PDO rdo;
        List<Mps000342ADO> ListAdo = new List<Mps000342ADO>();
        public Mps000342Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000342PDO)rdoBase;
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

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListPrint1", ListAdo);
                objectTag.AddObjectData(store, "ListPrint2", ListAdo);
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
                    AddObjectKeyIntoListkey<HIS_TRANSACTION>(rdo._Transaction, false);

                    string amountText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo._Transaction.AMOUNT, 0);
                    SetSingleKey(new KeyValue(Mps000342ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, amountText));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000342ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, amountAfterExem));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(amountAfterExem, 4);
                    SetSingleKey(new KeyValue(Mps000342ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000342ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, amountAfterExemText));
                    decimal ratio = ((rdo._Transaction.EXEMPTION ?? 0) * 100) / rdo._Transaction.AMOUNT;
                    SetSingleKey(new KeyValue(Mps000342ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    decimal canthu = rdo._Transaction.AMOUNT - (rdo._Transaction.KC_AMOUNT ?? 0) - (rdo._Transaction.EXEMPTION ?? 0);
                    if ((rdo._Transaction.TDL_BILL_FUND_AMOUNT ?? 0) > 0)
                    {
                        canthu = canthu - (rdo._Transaction.TDL_BILL_FUND_AMOUNT ?? 0);
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                    SetSingleKey(new KeyValue(Mps000342ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu)));
                    SetSingleKey(new KeyValue(Mps000342ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.Number.Convert.NumberToStringRoundAuto(canthu, 4)));

                    if (rdo._ExpMests != null && rdo._ExpMests.Count > 0)
                    {
                        HIS_EXP_MEST expMest = rdo._ExpMests.OrderByDescending(o => o.ID).FirstOrDefault();
                        AddObjectKeyIntoListkey<HIS_EXP_MEST>(expMest, false);
                    }

                    Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredMedi = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                    Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredMate = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();
                    decimal totalPrice = 0;
                    decimal totalPriceNotRound = 0;

                    if (this.rdo._Medicines != null && this.rdo._Medicines.Count > 0)
                    {
                        this.rdo._Medicines = this.rdo._Medicines.OrderBy(o => o.ID).ToList();
                        var Group = this.rdo._Medicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PRICE, g.VAT_RATIO, g.DISCOUNT, g.PACKAGE_NUMBER }).ToList();
                        foreach (var group in Group)
                        {
                            dicExpiredMedi.Clear();
                            var listByGroup = group.ToList<V_HIS_EXP_MEST_MEDICINE>();
                            foreach (var item in listByGroup)
                            {
                                string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                                if (!dicExpiredMedi.ContainsKey(key))
                                    dicExpiredMedi[key] = new List<V_HIS_EXP_MEST_MEDICINE>();
                                dicExpiredMedi[key].Add(item);
                                totalPrice += (item.AMOUNT - (item.TH_AMOUNT ?? 0)) * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1) - (item.DISCOUNT ?? 0);
                            }
                            foreach (var dic in dicExpiredMedi)
                            {
                                Mps000342ADO ado = new Mps000342ADO(dic.Value);
                                ado.EXPIRED_DATE_STR = ConvertToDataString(dic.Key);
                                totalPriceNotRound += ado.SUM_TOTAL_PRICE_ROUND;
                                ListAdo.Add(ado);
                            }
                        }
                    }

                    if (this.rdo._Materials != null && this.rdo._Materials.Count > 0)
                    {
                        this.rdo._Materials = this.rdo._Materials.OrderBy(o => o.ID).ToList();
                        var Group = this.rdo._Materials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PRICE, g.VAT_RATIO, g.DISCOUNT, g.PACKAGE_NUMBER }).ToList();
                        foreach (var group in Group)
                        {
                            dicExpiredMate.Clear();
                            var listByGroup = group.ToList<V_HIS_EXP_MEST_MATERIAL>();
                            foreach (var item in listByGroup)
                            {
                                string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                                if (!dicExpiredMate.ContainsKey(key))
                                    dicExpiredMate[key] = new List<V_HIS_EXP_MEST_MATERIAL>();
                                dicExpiredMate[key].Add(item);
                                totalPrice += (item.AMOUNT - (item.TH_AMOUNT ?? 0)) * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1) - (item.DISCOUNT ?? 0);
                            }
                            foreach (var dic in dicExpiredMate)
                            {
                                Mps000342ADO ado = new Mps000342ADO(dic.Value);
                                ado.EXPIRED_DATE_STR = ConvertToDataString(dic.Key);
                                totalPriceNotRound += ado.SUM_TOTAL_PRICE_ROUND;
                                ListAdo.Add(ado);
                            }
                        }
                    }

                    if (this.rdo._BillGoods != null && this.rdo._BillGoods.Count > 0 && ListAdo.Count <= 0)
                    {
                        foreach (var item in this.rdo._BillGoods)
                        {
                            dicExpiredMate.Clear();
                            totalPrice += (item.AMOUNT * item.PRICE) - (item.DISCOUNT ?? 0);
                            Mps000342ADO ado = new Mps000342ADO(item);
                            totalPriceNotRound += ado.SUM_TOTAL_PRICE_ROUND;
                            ListAdo.Add(ado);
                        }
                    }

                    ListAdo = ListAdo.Where(o => o.TOTAL_AMOUNT > 0).ToList();

                    string sumText = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    SetSingleKey(new KeyValue(Mps000342ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));
                    SetSingleKey(new KeyValue(Mps000342ExtendSingleKey.SUM_TOTAL_PRICE_ROUND, totalPriceNotRound));
                    string sumNotRoundText = String.Format("{0:0}", totalPriceNotRound);
                    SetSingleKey(new KeyValue(Mps000342ExtendSingleKey.SUM_TOTAL_PRICE_ROUND_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumNotRoundText)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static string ConvertToDataString(string date)
        {
            string result = "";
            if (!String.IsNullOrWhiteSpace(date) && date.Length == 8)
            {
                string year = date.Substring(0, 4);
                string month = date.Substring(4, 2);
                string day = date.Substring(6, 2);
                result = day + "/" + month + "/" + year;//27/10/2019
            }
            return result;
        }
    }
}
