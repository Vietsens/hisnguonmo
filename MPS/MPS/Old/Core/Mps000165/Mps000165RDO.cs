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

namespace MPS.Core.Mps000165
{
    public class Mps000165RDO : RDOBase
    {
        V_HIS_OTHER_EXP_MEST _OtherExpMest = null;
        List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;
        List<V_HIS_EXP_MEST_MATERIAL> _Materials = null;
        V_HIS_EXP_MEST_1 expMest = null;
        internal List<Mps000092ADO> listAdo = new List<Mps000092ADO>();

        long expMesttSttId__Draft = 1;// trạng thái nháp
        long expMesttSttId__Request = 2;// trạng thái yêu cầu
        long expMesttSttId__Reject = 3;// không duyệt
        long expMesttSttId__Approval = 4; // duyệt
        long expMesttSttId__Export = 5;// đã xuất

        public Mps000165RDO(V_HIS_OTHER_EXP_MEST otherExpMest, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial)
        {
            this._OtherExpMest = otherExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
        }

        public Mps000165RDO(V_HIS_OTHER_EXP_MEST otherExpMest, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial,V_HIS_EXP_MEST_1 expMest)
        {
            this._OtherExpMest = otherExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.expMest = expMest;
        }

        public Mps000165RDO(V_HIS_OTHER_EXP_MEST otherExpMest, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export)
        {
            this._OtherExpMest = otherExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
        }

        internal override void SetSingleKey()
        {
            try
            {
                Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredMedi = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredMate = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();
                decimal totalPrice = 0;
                decimal discount = 0;
                if (this._OtherExpMest != null)
                {
                    keyValues.Add(new KeyValue(Mps000092ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._OtherExpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000092ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._OtherExpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000092ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._OtherExpMest.CREATE_TIME ?? 0)));
                    GlobalQuery.AddObjectKeyIntoListkey(this._OtherExpMest, keyValues, false);
                    GlobalQuery.AddObjectKeyIntoListkey(this.expMest, keyValues, false);
                }

                if (this._Medicines != null && this._Medicines.Count > 0)
                {
                    this._Medicines = this._Medicines.OrderBy(o => o.ID).ToList();
                    if (_OtherExpMest.EXP_MEST_STT_ID == expMesttSttId__Approval || _OtherExpMest.EXP_MEST_STT_ID == expMesttSttId__Export)
                    {
                        this._Medicines = this._Medicines.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                    }
                    else if (_OtherExpMest.EXP_MEST_STT_ID == expMesttSttId__Request || _OtherExpMest.EXP_MEST_STT_ID == expMesttSttId__Reject || _OtherExpMest.EXP_MEST_STT_ID == expMesttSttId__Draft)
                    {
                        this._Medicines = this._Medicines.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                    }
                    discount += this._Medicines.Sum(o => o.DISCOUNT ?? 0);
                    var Group = this._Medicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PRICE, g.VAT_RATIO, g.DISCOUNT }).ToList();
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
                            totalPrice += item.AMOUNT * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1);
                        }
                        foreach (var dic in dicExpiredMedi)
                        {
                            listAdo.Add(new Mps000092ADO(dic.Value));
                        }
                    }
                }

                if (this._Materials != null && this._Materials.Count > 0)
                {
                    this._Materials = this._Materials.OrderBy(o => o.ID).ToList();
                    if (_OtherExpMest.EXP_MEST_STT_ID == expMesttSttId__Approval || _OtherExpMest.EXP_MEST_STT_ID == expMesttSttId__Export)
                    {
                        this._Materials = this._Materials.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MATERIAL.IN_EXECUTE__TRUE).ToList();
                    }
                    else if (_OtherExpMest.EXP_MEST_STT_ID == expMesttSttId__Request || _OtherExpMest.EXP_MEST_STT_ID == expMesttSttId__Reject || _OtherExpMest.EXP_MEST_STT_ID == expMesttSttId__Draft)
                    {
                        this._Materials = this._Materials.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MATERIAL.IN_REQUEST__TRUE).ToList();
                    }
                    discount += this._Materials.Sum(s => s.DISCOUNT ?? 0);
                    var Group = this._Materials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PRICE, g.VAT_RATIO, g.DISCOUNT }).ToList();
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
                            totalPrice += item.AMOUNT * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1);
                        }
                        foreach (var dic in dicExpiredMate)
                        {
                            listAdo.Add(new Mps000092ADO(dic.Value));
                        }
                    }
                }

                keyValues.Add(new KeyValue(Mps000092ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                string sumText = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                keyValues.Add(new KeyValue(Mps000092ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));
                keyValues.Add(new KeyValue(Mps000092ExtendSingleKey.SUM_TOTAL_PRICE_AFTER_DISCOUNT, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(totalPrice - discount)));
                listAdo = listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
