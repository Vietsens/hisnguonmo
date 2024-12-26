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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestCreate.ADO
{
    public class ImportMediMateADO
    {
        public string IS_MEDICINE { get; set; }

        public string MEDI_MATE_CODE { get; set; }
        public decimal? IMP_AMOUNT { get; set; }
        public decimal? IMP_PRICE { get; set; }
        public decimal? IMP_VAT_RATIO { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string SALE_EQUAL_IMP_PRICE { get; set; }

        public string TDL_BID_GROUP_CODE { get; set; }
        public string TDL_BID_NUM_ORDER { get; set; }
        public string TDL_BID_PACKAGE_CODE { get; set; }
        public string TDL_BID_YEAR { get; set; }
        public string TDL_BID_NUMBER { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public string MANUFACTURER_CODE { get; set; }
        public string REGISTER_NUMBER { get; set; }

        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string MEDICINE_USE_FORM_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_NAME { get; set; }
        public string PACKING_TYPE_NAME { get; set; }
        public string DOSAGE_FORM { get; set; }

        public long? MONTH_LIFESPAN { get; set; }
        public long? DAY_LIFESPAN { get; set; }
        public long? HOUR_LIFESPAN { get; set; }

        public string MONTH_LIFESPAN_STR { get; set; }
        public string DAY_LIFESPAN_STR { get; set; }
        public string HOUR_LIFESPAN_STR { get; set; }

        public string PATIENT_TYPE_CODE_01 { get; set; }
        public decimal? EXP_PRICE_01 { get; set; }
        public decimal? EXP_VAT_RATIO_01 { get; set; }

        public string PATIENT_TYPE_CODE_02 { get; set; }
        public decimal? EXP_PRICE_02 { get; set; }
        public decimal? EXP_VAT_RATIO_02 { get; set; }

        public string PATIENT_TYPE_CODE_03 { get; set; }
        public decimal? EXP_PRICE_03 { get; set; }
        public decimal? EXP_VAT_RATIO_03 { get; set; }

        public string PATIENT_TYPE_CODE_04 { get; set; }
        public decimal? EXP_PRICE_04 { get; set; }
        public decimal? EXP_VAT_RATIO_04 { get; set; }

        public string PATIENT_TYPE_CODE_05 { get; set; }
        public decimal? EXP_PRICE_05 { get; set; }
        public decimal? EXP_VAT_RATIO_05 { get; set; }

        public string PATIENT_TYPE_CODE_06 { get; set; }
        public decimal? EXP_PRICE_06 { get; set; }
        public decimal? EXP_VAT_RATIO_06 { get; set; }

        public string PATIENT_TYPE_CODE_07 { get; set; }
        public decimal? EXP_PRICE_07 { get; set; }
        public decimal? EXP_VAT_RATIO_07 { get; set; }

        public string PATIENT_TYPE_CODE_08 { get; set; }
        public decimal? EXP_PRICE_08 { get; set; }
        public decimal? EXP_VAT_RATIO_08 { get; set; }

        public string PATIENT_TYPE_CODE_09 { get; set; }
        public decimal? EXP_PRICE_09 { get; set; }
        public decimal? EXP_VAT_RATIO_09 { get; set; }

        public string PATIENT_TYPE_CODE_10 { get; set; }
        public decimal? EXP_PRICE_10 { get; set; }
        public decimal? EXP_VAT_RATIO_10 { get; set; }

        // tt thau
        public string TDL_BID_EXTRA_CODE { get; set; }
        public string INFORMATION_BID { get; set; }
        public string TT_THAU { get; set; }

    }
}
