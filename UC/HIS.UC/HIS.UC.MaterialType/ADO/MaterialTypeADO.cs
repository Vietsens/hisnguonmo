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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HIS.UC.MaterialType.ADO
{
    public class MaterialTypeADO : V_HIS_MATERIAL_TYPE
    {
        public bool? IsChemicalSubstance { get; set; }
        public bool IsStopImp { get; set; }
        public bool? IsLeaf { get; set; }
        public bool? IsStent { get; set; }
        public bool? IsAutoExpend { get; set; }
        public bool? IsMustPrepare { get; set; }
        public bool? IsCPNG { get; set; }
        public bool? IsExprireDate { get; set; }
        public bool? IsAllowExportOdd { get; set; }
        public bool? IsRawMaterial { get; set; }
        public string heinLimitPriceInTimeStr { get; set; }
        public string heinLimitPriceIntrTimeStr { get; set; }
        public decimal? ImpVatRatio100 { get; set; }
        public decimal? HeinLimitVatRatio100 { get; set; }
        public decimal? HeinLimitVatRatioOld100 { get; set; }
        public string KEY_WORD { get; set; }
        public long? MEDI_CONTRACT_MATY_ID { get; set; }

        public string KeyField { get; set; }
        public string ParentField { get; set; }

        // số lượng, đơn giá, VAT trong thầu
        public decimal AMOUNT_IN_BID { get; set; }
        public decimal? IMP_PRICE_IN_BID { get; set; }
        public decimal? IMP_VAT_RATIO_IN_BID { get; set; }

        // số lượng, đơn giá, VAT trong Hợp đồng
        public decimal? AMOUNT_IN_CONTRACT { get; set; }
        public decimal? IMP_PRICE_IN_CONTRACT { get; set; }
        public decimal? IMP_VAT_RATIO_IN_CONTRACT { get; set; }


        //Giá hợp đồng
        public decimal? CONTRACT_PRICE { get; set; }

        public bool IsMaterialTypeMap { get; set; }

        public string BidGroupCode { get; set; }

        public string TDL_BID_GROUP_CODE { get; set; }
        public decimal? ADJUST_AMOUNT { get; set; }

        public long? DISCOUNT_FROM_DATE { get; set; }
        public long? DISCOUNT_TO_DATE { get; set; }
        public string DISCOUNT_FROM_DATE_STR { get; set; }
        public string DISCOUNT_TO_DATE_STR { get; set; }
        public MaterialTypeADO()
        {
        }

        public MaterialTypeADO(V_HIS_MATERIAL_TYPE _data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeADO>(this, _data);

            this.ImpVatRatio100 = _data.IMP_VAT_RATIO != null ? _data.IMP_VAT_RATIO * 100 : null;
            this.HeinLimitVatRatio100 = _data.HEIN_LIMIT_RATIO != null ? _data.HEIN_LIMIT_RATIO * 100 : null;
            this.HeinLimitVatRatioOld100 = _data.HEIN_LIMIT_RATIO_OLD != null ? _data.HEIN_LIMIT_RATIO_OLD * 100 : null;
            this.heinLimitPriceInTimeStr = _data.HEIN_LIMIT_PRICE_IN_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(_data.HEIN_LIMIT_PRICE_IN_TIME ?? 0) : "";
            this.LAST_IMP_PRICE = _data.LAST_IMP_PRICE != null ? _data.LAST_IMP_PRICE : null;
            this.LAST_EXP_PRICE = _data.LAST_EXP_PRICE != null ? _data.LAST_EXP_PRICE : null;
            this.LAST_IMP_VAT_RATIO = _data.LAST_IMP_VAT_RATIO != null ? _data.LAST_IMP_VAT_RATIO : 0;
            this.LAST_EXP_VAT_RATIO = _data.LAST_EXP_VAT_RATIO != null ? _data.LAST_EXP_VAT_RATIO : 0;

            this.heinLimitPriceIntrTimeStr = _data.HEIN_LIMIT_PRICE_INTR_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(_data.HEIN_LIMIT_PRICE_INTR_TIME ?? 0) : "";
            this.PARENT_ID = _data.PARENT_ID;
            this.SERVICE_ID = _data.SERVICE_ID;
            this.SERVICE_TYPE_ID = _data.SERVICE_TYPE_ID;
            this.SERVICE_UNIT_CODE = _data.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_ID = _data.SERVICE_UNIT_ID;
            this.SERVICE_UNIT_NAME = _data.SERVICE_UNIT_NAME;
            this.REGISTER_NUMBER = _data.REGISTER_NUMBER;
            IsLeaf = (_data.IS_LEAF == 1);
            this.IsChemicalSubstance = (_data.IS_CHEMICAL_SUBSTANCE == 1);
            this.IsRawMaterial = (_data.IS_RAW_MATERIAL == 1);
            this.IsStopImp = (_data.IS_STOP_IMP == 1);
            IsStent = (_data.IS_STENT == 1);
            IsAutoExpend = (_data.IS_AUTO_EXPEND == 1);
            IsMustPrepare = (_data.IS_MUST_PREPARE == 1);

            this.IS_REUSABLE = _data.IS_REUSABLE;
            this.MAX_REUSE_COUNT = _data.MAX_REUSE_COUNT;

            this.KeyField = String.Format("VT_{0}_{1}", this.ID, Guid.NewGuid().ToString());

            this.KEY_WORD = convertToUnSign3(this.MATERIAL_TYPE_NAME)
              + this.MATERIAL_TYPE_NAME
              + convertToUnSign3(this.MATERIAL_TYPE_CODE)
              + this.MATERIAL_TYPE_CODE
              + convertToUnSign3(this.MANUFACTURER_CODE)
              + this.MANUFACTURER_CODE
              + convertToUnSign3(this.MANUFACTURER_NAME)
              + this.MANUFACTURER_NAME
              + convertToUnSign3(this.PACKING_TYPE_NAME)
              + this.PACKING_TYPE_NAME
              + convertToUnSign3(this.SERVICE_UNIT_NAME)
              + this.SERVICE_UNIT_NAME
              + convertToUnSign3(this.REGISTER_NUMBER)
              + this.REGISTER_NUMBER;

        }

        public void SetMaterialTypeMap(V_HIS_MATERIAL_TYPE child)
        {
            this.ID = child.MATERIAL_TYPE_MAP_ID.Value;
            this.MATERIAL_TYPE_CODE = child.MATERIAL_TYPE_MAP_CODE;
            this.MATERIAL_TYPE_NAME = child.MATERIAL_TYPE_MAP_NAME;
            this.KeyField = String.Format("MAP_{0}_{1}", child.MATERIAL_TYPE_MAP_ID, Guid.NewGuid().ToString());
            this.IsMaterialTypeMap = true;

            this.SERVICE_UNIT_CODE = child.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_ID = child.SERVICE_UNIT_ID;
            this.SERVICE_UNIT_NAME = child.SERVICE_UNIT_NAME;

            this.KEY_WORD = convertToUnSign3(this.MATERIAL_TYPE_NAME)
              + this.MATERIAL_TYPE_NAME
              + convertToUnSign3(this.MATERIAL_TYPE_CODE)
              + this.MATERIAL_TYPE_CODE
              + convertToUnSign3(this.MANUFACTURER_CODE)
              + this.MANUFACTURER_CODE
              + convertToUnSign3(this.MANUFACTURER_NAME)
              + this.MANUFACTURER_NAME
              + convertToUnSign3(this.PACKING_TYPE_NAME)
              + this.PACKING_TYPE_NAME
              + convertToUnSign3(this.SERVICE_UNIT_NAME)
              + this.SERVICE_UNIT_NAME
              + convertToUnSign3(this.SERVICE_UNIT_NAME)
              + this.SERVICE_UNIT_NAME;

        }

        public string convertToUnSign3(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return "";

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}
