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
using AutoMapper;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000388.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000388
{
    public class Mps000388Processor : AbstractProcessor
    {
        Mps000388PDO rdo;

        private List<DebtGoodsADO> listDebtGoods = new List<DebtGoodsADO>();

        public Mps000388Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000388PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                ProcessObjectTag();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "DebtGoods", this.listDebtGoods);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessObjectTag()
        {
            try
            {
                if (rdo._DebtGoods != null && rdo._DebtGoods.Count > 0)
                {
                    Mapper.CreateMap<HIS_DEBT_GOODS, DebtGoodsADO>();
                    List<DebtGoodsADO> debtGoods = Mapper.Map<List<DebtGoodsADO>>(rdo._DebtGoods);

                    debtGoods.ForEach(o =>
                        {
                            o.PRICE_VAT = (o.PRICE * ((decimal)1 + (o.VAT_RATIO ?? 0)));
                        });


                    Mapper.CreateMap<DebtGoodsADO, DebtGoodsADO>();

                    List<DebtGoodsADO> listMedicine = debtGoods.Where(o => o.MEDICINE_TYPE_ID.HasValue).ToList();
                    
                    if (listMedicine != null && listMedicine.Count > 0)
                    {
                        var Groups = listMedicine.GroupBy(g => new { g.MEDICINE_TYPE_ID.Value, g.PRICE_VAT }).ToList();
                        foreach (var g in Groups)
                        {
                            DebtGoodsADO ado = Mapper.Map<DebtGoodsADO>(g.FirstOrDefault());
                            ado.AMOUNT = g.Sum(s => s.AMOUNT);
                            ado.DISCOUNT = g.Sum(s => (s.DISCOUNT ?? 0));
                            if (g.Any(a => a.EXPIRED_DATE.HasValue))
                            {
                                ado.EXPIRED_DATE = g.Where(o => o.EXPIRED_DATE.HasValue).OrderByDescending(d => d.EXPIRED_DATE.Value).FirstOrDefault().EXPIRED_DATE;
                            }
                            ado.TOTAL_PRICE = ado.AMOUNT * ado.PRICE_VAT;
                            ado.TOTAL_PRICE_DISCOUNT = ado.TOTAL_PRICE - (ado.DISCOUNT ?? 0);
                            listDebtGoods.Add(ado);
                        }
                    }

                    List<DebtGoodsADO> listMaterial = debtGoods.Where(o => o.MATERIAL_TYPE_ID.HasValue).ToList();
                    
                    if (listMaterial != null && listMaterial.Count > 0)
                    {
                        var Groups = listMaterial.GroupBy(g => new { g.MATERIAL_TYPE_ID.Value, g.PRICE_VAT }).ToList();
                        foreach (var g in Groups)
                        {
                            DebtGoodsADO ado = Mapper.Map<DebtGoodsADO>(g.FirstOrDefault());
                            ado.AMOUNT = g.Sum(s => s.AMOUNT);
                            ado.DISCOUNT = g.Sum(s => (s.DISCOUNT ?? 0));
                            if (g.Any(a => a.EXPIRED_DATE.HasValue))
                            {
                                ado.EXPIRED_DATE = g.Where(o => o.EXPIRED_DATE.HasValue).OrderByDescending(d => d.EXPIRED_DATE.Value).FirstOrDefault().EXPIRED_DATE;
                            }
                            ado.TOTAL_PRICE = ado.AMOUNT * ado.PRICE_VAT;
                            ado.TOTAL_PRICE_DISCOUNT = ado.TOTAL_PRICE - (ado.DISCOUNT ?? 0);
                            listDebtGoods.Add(ado);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo._Transaction != null)
                {
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    string temp = rdo._Transaction.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.AMOUNT_NUM, rdo._Transaction.AMOUNT));
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToString(rdo._Transaction.AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                    //string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT));

                    string amountText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo._Transaction.AMOUNT, 0);
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.AMOUNT_TEXT, rdo._Transaction.AMOUNT));
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, amountText));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToString(amountAfterExem, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_NUM, amountAfterExem));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(amountAfterExem, 4);
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, amountAfterExemText));
                    decimal ratio = ((rdo._Transaction.EXEMPTION ?? 0) * 100) / rdo._Transaction.AMOUNT;
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Transaction.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.DESCRIPTION, rdo._Transaction.DESCRIPTION));
                    SetSingleKey(new KeyValue(Mps000388ExtendSingleKey.EXEMPTION_REASON, rdo._Transaction.EXEMPTION_REASON));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static class AgeUtil
        {
            public static string CalculateFullAge(long ageNumber)
            {
                string tuoi;
                string cboAge;
                try
                {
                    DateTime dtNgSinh = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        tuoi = "";
                        cboAge = "Tuổi";
                        return "";
                    }
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;

                    if (nam > 0)
                    {
                        tuoi = nam.ToString();
                        cboAge = "Tuổi";
                    }
                    else
                    {
                        if (thang > 0)
                        {
                            tuoi = thang.ToString();
                            cboAge = "Tháng";
                        }
                        else
                        {
                            if (ngay > 0)
                            {
                                tuoi = ngay.ToString();
                                cboAge = "ngày";
                            }
                            else
                            {
                                tuoi = "";
                                cboAge = "Giờ";
                            }
                        }
                    }
                    return tuoi + " " + cboAge;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    return "";
                }
            }
        }
    }
}
