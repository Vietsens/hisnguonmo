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

namespace MPS.Processor.Mps000100.ADO
{
    class Mps000100ADO : V_HIS_IMP_MEST_MEDICINE
    {
        public decimal AMOUNT_APPROVED { get; set; }
        public string AMOUNT_APPROVED_STRING { get; set; }
        public decimal AMOUNT_IMPORTED { get; set; }
        public string AMOUNT_IMPORTED_STRING { get; set; }
        public string AMOUNT_STRING { get; set; }
        public string CONCENTRA_PACKING_TYPE_NAME { get; set; }

        public string DESCRIPTION { get; set; }
        public string IS_BHYT { get; set; }
        public bool IS_MEDICINE { get; set; }
        public decimal PRICE_EXPORTED { get; set; }
        public long Type { get; set; }
        public string MEDICINE_GROUP_CODE { get; set; }
        public string MEDICINE_GROUP_NAME { get; set; }
        public string MEDICINE_PARENT_CODE { get; set; }
        public long? MEDICINE_PARENT_ID { get; set; }
        public string MEDICINE_PARENT_NAME { get; set; }
        //public long? MEDICINE_USE_FORM_NUM_ORDER { get; set; }
       // public string MEDICINE_TYPE_NAME { get; set; }
        public Mps000100ADO() { }

        public Mps000100ADO(List<V_HIS_IMP_MEST_MATERIAL> datas, long _impMestSttId, long HisImpMestSttId__Imported, long HisImpMestSttId__Approved)
        {
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<Mps000100ADO>(this, datas[0]);
                    this.Type = 2;
                    this.MEDICINE_TYPE_CODE = datas[0].MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = datas[0].MATERIAL_TYPE_NAME;
                    this.MEDICINE_TYPE_ID = datas[0].MATERIAL_TYPE_ID;
                    this.MEDICINE_ID = datas[0].MATERIAL_ID;
                    this.IS_MEDICINE = false;
                    this.SERVICE_UNIT_NAME = datas[0].SERVICE_UNIT_NAME;
                    this.NUM_ORDER = datas[0].NUM_ORDER;
                    this.EXPIRED_DATE = datas[0].EXPIRED_DATE;
                    this.AMOUNT = datas.Sum(p => p.AMOUNT);
                    //this.AMOUNT = datas[0].AMOUNT;
                    if (_impMestSttId == HisImpMestSttId__Imported)
                    {
                        this.AMOUNT_IMPORTED = this.AMOUNT;
                    }
                    if (_impMestSttId == HisImpMestSttId__Imported || _impMestSttId == HisImpMestSttId__Approved)
                    {
                        this.AMOUNT_APPROVED = this.AMOUNT;
                    }
                    this.AMOUNT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT)));
                    this.AMOUNT_IMPORTED_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_IMPORTED)));
                    this.AMOUNT_APPROVED_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_APPROVED)));

                    this.REQ_AMOUNT = datas.Sum(p => p.REQ_AMOUNT ?? 0);
                    this.NOTE = string.Join("; ", datas.Select(s => s.NOTE).Distinct());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000100ADO(List<V_HIS_IMP_MEST_MEDICINE> datas, List<V_HIS_MEDICINE_TYPE> vHisMedicineTypes, long _impMestSttId, long HisImpMestSttId__Imported, long HisImpMestSttId__Approved)
        {
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<Mps000100ADO>(this, datas[0]);
                    this.Type = 1;
                    this.IS_MEDICINE = true;
                    this.EXPIRED_DATE = datas[0].EXPIRED_DATE;
                    this.AMOUNT = datas.Sum(p => p.AMOUNT);
                    //this.AMOUNT = datas[0].AMOUNT;
                    if (_impMestSttId == HisImpMestSttId__Imported)
                    {
                        this.AMOUNT_IMPORTED = this.AMOUNT;
                    }
                    if (_impMestSttId == HisImpMestSttId__Imported || _impMestSttId == HisImpMestSttId__Approved)
                    {
                        this.AMOUNT_APPROVED = this.AMOUNT;
                    }
                    this.AMOUNT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT)));
                    this.AMOUNT_IMPORTED_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_IMPORTED)));
                    this.AMOUNT_APPROVED_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_APPROVED)));
                    this.MEDICINE_USE_FORM_NUM_ORDER = datas[0].MEDICINE_USE_FORM_NUM_ORDER;
                    this.NUM_ORDER = datas[0].NUM_ORDER;
                    this.SERVICE_UNIT_NAME = datas[0].SERVICE_UNIT_NAME;
                    this.MEDICINE_TYPE_NAME = datas[0].MEDICINE_TYPE_NAME;
                    this.REQ_AMOUNT = datas.Sum(p => p.REQ_AMOUNT ?? 0);
                    this.NOTE = string.Join("; ", datas.Select(s => s.NOTE).Distinct());

                    if (vHisMedicineTypes != null && vHisMedicineTypes.Count > 0)
                    {
                        V_HIS_MEDICINE_TYPE MedicineType = vHisMedicineTypes.FirstOrDefault(o => o.ID == datas[0].MEDICINE_TYPE_ID);

                        if (MedicineType != null)
                        {
                            this.MEDICINE_PARENT_ID = MedicineType.PARENT_ID;
                            this.MEDICINE_PARENT_CODE = MedicineType.PARENT_CODE;
                            this.MEDICINE_PARENT_NAME = MedicineType.PARENT_NAME;
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => datas[0].MEDICINE_TYPE_NAME), datas[0].MEDICINE_TYPE_NAME));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => datas[0].MEDICINE_USE_FORM_NUM_ORDER), datas[0].MEDICINE_USE_FORM_NUM_ORDER));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
