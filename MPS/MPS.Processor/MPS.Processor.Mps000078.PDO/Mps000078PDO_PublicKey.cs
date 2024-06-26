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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000078.PDO
{
    public partial class Mps000078PDO : RDOBase
    {
        public List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines { get; set; }
        public List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterials { get; set; }
        public V_HIS_IMP_MEST AggrImpMest { get; set; }
        public HIS_DEPARTMENT Department { get; set; }
        public List<long> ServiceUnitIds { get; set; }
        public List<long> UseFormIds { get; set; }
        public List<long> RoomIds { get; set; }
        public bool IsMedicine { get; set; }
        public bool Ismaterial { get; set; }
        public IsTittle keyNameTitles;

    }

    public class Mps000078ADO : V_HIS_IMP_MEST_MEDICINE
    {
        public bool IS_MEDICINE { get; set; }
        //public string USE_TIME_TO_STR { get; set; }
        public decimal AMOUNT_IMPORTED { get; set; }
        public decimal AMOUNT_APPROVED { get; set; }
        public string AMOUNT_IMPORTED_STRING { get; set; }
        public string AMOUNT_APPROVED_STRING { get; set; }
        public string AMOUNT_STRING { get; set; }
        public string DESCRIPTION { get; set; }
        public string CONCENTRA_PACKING_TYPE_NAME { get; set; }
        public string IS_BHYT { get; set; }
        //public long ROOM_ASSIGN_ID { get; set; }
        //public long INTRUCTION_TIME { get; set; }
        public decimal PRICE_EXPORTED { get; set; }

        public decimal TOTAL_IMP_PRICE { get; set; }

        public string REQ_LOGINNAME { get; set; }
        public string REQ_USERNAME { get; set; }

        public Mps000078ADO() { }

        public Mps000078ADO(List<V_HIS_IMP_MEST_MEDICINE> datas, long _impMestSttId, long HisImpMestSttId__Imported, long HisImpMestSttId__Approved)
        {
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<Mps000078ADO>(this, datas[0]);
                    this.IS_MEDICINE = true;
                    this.AMOUNT = datas.Sum(p => p.AMOUNT);
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
                    this.TOTAL_IMP_PRICE = this.AMOUNT * this.IMP_PRICE * (1 + this.IMP_VAT_RATIO);

                    this.REQ_AMOUNT = datas.Sum(p => p.REQ_AMOUNT ?? 0);
                    this.NOTE = string.Join("; ", datas.Select(s => s.NOTE).Distinct());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000078ADO(List<V_HIS_IMP_MEST_MATERIAL> datas, long _impMestSttId, long HisImpMestSttId__Imported, long HisImpMestSttId__Approved)
        {
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<Mps000078ADO>(this, datas[0]);
                    this.MEDICINE_TYPE_CODE = datas[0].MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = datas[0].MATERIAL_TYPE_NAME;
                    this.MEDICINE_TYPE_ID = datas[0].MATERIAL_TYPE_ID;
                    this.MEDICINE_ID = datas[0].MATERIAL_ID;
                    this.IS_MEDICINE = false;
                    this.AMOUNT = datas.Sum(p => p.AMOUNT);
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
                    this.TOTAL_IMP_PRICE = this.AMOUNT * this.IMP_PRICE * (1 + this.IMP_VAT_RATIO);

                    this.REQ_AMOUNT = datas.Sum(p => p.REQ_AMOUNT ?? 0);
                    this.NOTE = string.Join("; ", datas.Select(s => s.NOTE).Distinct());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
