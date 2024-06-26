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
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000134
{
    public class Mps000134RDO : RDOBase
    {
        V_HIS_DEPA_EXP_MEST _DepaExpMest = null;
        List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;
        List<V_HIS_EXP_MEST_MATERIAL> _Materials = null;
        HIS_EXP_MEST _ExpMest = null;
        internal string Title = "";
        internal List<Mps000134ADO> listAdo = new List<Mps000134ADO>();

        public Mps000134RDO(V_HIS_DEPA_EXP_MEST depaExpMest, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, string title)
        {
            this._DepaExpMest = depaExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.Title = title;
        }

        public Mps000134RDO(V_HIS_DEPA_EXP_MEST depaExpMest, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, string title, HIS_EXP_MEST expMest)
        {
            this._DepaExpMest = depaExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.Title = title;
            this._ExpMest = expMest;
        }


        internal override void SetSingleKey()
        {
            try
            {
                Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredMedi = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredMate = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();

                decimal totalPrice = 0;
                keyValues.Add(new KeyValue(Mps000134ExtendSingleKey.TITLED_BILL, this.Title));
                if (this._DepaExpMest != null)
                {
                    keyValues.Add(new KeyValue(Mps000134ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._DepaExpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000134ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._DepaExpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000134ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._DepaExpMest.CREATE_TIME ?? 0)));
                    GlobalQuery.AddObjectKeyIntoListkey(this._DepaExpMest, keyValues, false);
                }

                if (this._ExpMest != null)
                {
                    keyValues.Add(new KeyValue(Mps000134ExtendSingleKey.DESCRIPTION_STR, this._ExpMest.DESCRIPTION));
                }

                if (this._Medicines != null && this._Medicines.Count > 0)
                {
                    var Group = this._Medicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).ToList();
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
                            totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                        }
                        foreach (var dic in dicExpiredMedi)
                        {
                            listAdo.Add(new Mps000134ADO(dic.Value));
                        }
                    }
                }

                if (this._Materials != null && this._Materials.Count > 0)
                {
                    var Group = this._Materials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).ToList();
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
                            totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                        }
                        foreach (var dic in dicExpiredMate)
                        {
                            listAdo.Add(new Mps000134ADO(dic.Value));
                        }
                    }
                }

                keyValues.Add(new KeyValue(Mps000134ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                string sumText = String.Format("0:0.####", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                keyValues.Add(new KeyValue(Mps000134ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));

                listAdo = listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
