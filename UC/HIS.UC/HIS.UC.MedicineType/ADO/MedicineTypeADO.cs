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

namespace HIS.UC.MedicineType.ADO
{
    public class MedicineTypeADO : V_HIS_MEDICINE_TYPE
    {
        public bool IsStopImp { get; set; }
        public bool IsCPNG { get; set; }
        public bool IsFood { get; set; }
        public bool? IsLeaf { get; set; }
        public bool? IsExprireDate { get; set; }
        public bool? IsAllowExportOdd { get; set; }
        public bool? IsAutoExpend { get; set; }
        public string heinLimitPriceInTimeStr { get; set; }
        public string heinLimitPriceIntrTimeStr { get; set; }
        public decimal? ImpVatRatio100 { get; set; }
        public decimal? HeinLimitVatRatio100 { get; set; }
        public decimal? HeinLimitVatRatioOld100 { get; set; }
        public bool? IsMustPrepare { get; set; }
        public string KEY_WORD { get; set; }

        // số lượng, đơn giá, VAT trong thầu
        public decimal AMOUNT_IN_BID { get; set; }
        public decimal? IMP_PRICE_IN_BID { get; set; }
        public decimal? IMP_VAT_RATIO_IN_BID { get; set; }

        public long? MEDI_CONTRACT_METY_ID { get; set; }

        // số lượng, đơn giá, VAT trong Hợp đồng
        public decimal? AMOUNT_IN_CONTRACT { get; set; }
        public decimal? IMP_PRICE_IN_CONTRACT { get; set; }
        public decimal? IMP_VAT_RATIO_IN_CONTRACT { get; set; }

        // Giá hợp đồng
        public decimal? CONTRACT_PRICE { get; set; }

        public string BidGroupCode { get; set; }
        public string TDL_BID_GROUP_CODE { get; set; }

        public string KeyField { get; set; }
        public string ParentField { get; set; }
        
        public long? ID_HIS_MEDI_CONTRACT_METY { get; set; }
        public decimal? ADJUST_AMOUNT { get; set; }
        public long? DISCOUNT_FROM_DATE { get; set; }
        public long? DISCOUNT_TO_DATE { get; set; }
        public string DISCOUNT_FROM_DATE_STR { get; set; }
        public string DISCOUNT_TO_DATE_STR { get; set; }
        public MedicineTypeADO()
        {
        }

        public string convertToUnSign3(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return "";

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public MedicineTypeADO(V_HIS_MEDICINE_TYPE medicineType)
        {
            //set truc tiep cac truong trong doi tuong
            Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeADO>(this, medicineType);

            this.ImpVatRatio100 = medicineType.IMP_VAT_RATIO != null ? medicineType.IMP_VAT_RATIO * 100 : null;
            this.HeinLimitVatRatio100 = medicineType.HEIN_LIMIT_RATIO != null ? medicineType.HEIN_LIMIT_RATIO * 100 : null;
            this.HeinLimitVatRatioOld100 = medicineType.HEIN_LIMIT_RATIO_OLD != null ? medicineType.HEIN_LIMIT_RATIO_OLD * 100 : null;
            this.heinLimitPriceInTimeStr = medicineType.HEIN_LIMIT_PRICE_IN_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(medicineType.HEIN_LIMIT_PRICE_IN_TIME ?? 0) : "";

            this.heinLimitPriceIntrTimeStr = medicineType.HEIN_LIMIT_PRICE_INTR_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(medicineType.HEIN_LIMIT_PRICE_INTR_TIME ?? 0) : "";

            IsLeaf = (medicineType.IS_LEAF == 1);

            IsMustPrepare = (medicineType.IS_MUST_PREPARE == 1);

            if (medicineType.IS_AUTO_EXPEND == 1)
            {
                this.IsAutoExpend = true;
            }
            else
            {
                this.IsAutoExpend = false;
            }

            if (medicineType.IS_FUNCTIONAL_FOOD == 1)
            {
                this.IsFood = true;
            }
            else
            {
                this.IsFood = false;
            }
            if (medicineType.IS_STOP_IMP == 1)
            {
                this.IsStopImp = true;
            }
            else
            {
                this.IsStopImp = false;
            }
            this.KEY_WORD = convertToUnSign3(this.MEDICINE_TYPE_NAME)
                + this.MEDICINE_TYPE_NAME
                + convertToUnSign3(this.MEDICINE_TYPE_CODE)
                + this.MEDICINE_TYPE_CODE
                + convertToUnSign3(this.MEDICINE_LINE_NAME)
                + this.MEDICINE_LINE_NAME
                + convertToUnSign3(this.MEDICINE_USE_FORM_NAME)
                + this.MEDICINE_USE_FORM_NAME
                + convertToUnSign3(this.HEIN_SERVICE_BHYT_NAME)
                + this.HEIN_SERVICE_BHYT_NAME
                + convertToUnSign3(this.SERVICE_UNIT_NAME)
                + this.SERVICE_UNIT_NAME
                + convertToUnSign3(this.MANUFACTURER_CODE)
                + this.MANUFACTURER_CODE
                + convertToUnSign3(this.MANUFACTURER_NAME)
                + this.MANUFACTURER_NAME
                + convertToUnSign3(this.ACTIVE_INGR_BHYT_CODE)
                + this.ACTIVE_INGR_BHYT_CODE
                + convertToUnSign3(this.ACTIVE_INGR_BHYT_NAME)
                + this.ACTIVE_INGR_BHYT_NAME;
            //this.AMOUNT_PLUS = service.AMOUNT;
            //this.VAT = service.VAT_RATIO * 100;
            //this.CONCRETE_ID__IN_SETY = (service.SERVICE_TYPE_ID + "." + service.CONCRETE_ID);
            //this.PARENT_ID__IN_SETY = (service.SERVICE_TYPE_ID + "." + service.PARENT_ID);

            this.ALLOW_MISSING_PKG_INFO = medicineType.ALLOW_MISSING_PKG_INFO;
            this.KeyField = this.ID + "" + "_"+Guid.NewGuid().ToString();
            this.ParentField = this.PARENT_ID + "";
        }
    }
}
