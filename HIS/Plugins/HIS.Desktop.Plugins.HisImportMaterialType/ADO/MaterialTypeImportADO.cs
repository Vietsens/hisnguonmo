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

namespace HIS.Desktop.Plugins.HisImportMaterialType.ADO
{
    public class MaterialTypeImportADO : V_HIS_MATERIAL_TYPE
    {
        public string ALERT_EXPIRED_DATE_STR { get; set; }
        public string ALERT_MIN_IN_STOCK_STR { get; set; }
        public string HEIN_LIMIT_PRICE_STR { get; set; }
        public string HEIN_LIMIT_PRICE_IN_TIME_STR { get; set; }
        public string HEIN_LIMIT_PRICE_INTR_TIME_STR { get; set; }
        public string HEIN_LIMIT_PRICE_OLD_STR { get; set; }
        public string HEIN_LIMIT_RATIO_STR { get; set; }
        public string HEIN_LIMIT_RATIO_OLD_STR { get; set; }
        public string IMP_PRICE_STR { get; set; }
        public string IMP_VAT_RATIO_STR { get; set; }
        public string INTERNAL_PRICE_STR { get; set; }
        public string COGS_STR { get; set; }
        public string NUM_ORDER_STR { get; set; }
        public string ALERT_MAX_IN_PRESCRIPTION_STR { get; set; }
        public string ALERT_MAX_IN_DAY_STR { get; set; }
        public string RECORDING_TRANSACTION { get; set; }

        public string CHEMICAL_SUBSTANCE { get; set; }
        public string ALLOW_EXPORT_ODD { get; set; }
        public string ALLOW_ODD { get; set; }
        public string AUTO_EXPEND { get; set; }
        public string BUSINESS { get; set; }
        public string IN_KTC_FEE { get; set; }
        public string OUT_PARENT_FEE { get; set; }
        public string REQUIRE_HSD { get; set; }
        public string SALE_EQUAL_IMP_PRICE { get; set; }
        public string STENT { get; set; }
        public string STOP_IMP { get; set; }
        public decimal? COGS { get; set; }
        public string IDENTITY_MANAGEMEN { get; set; }

        public string PARENT_CODE { get; set; }
        public string ERROR { get; set; }
        public bool IS_LESS_MANUFACTURER { get; set; }
        public string NOT_SHOW_TRACKING_STR { get; set; }

        public string TDL_GENDER_CODE { get; set; }
        public string TDL_GENDER_NAME { get; set; }

        public MaterialTypeImportADO()
        {
        }

        public MaterialTypeImportADO(V_HIS_MATERIAL_TYPE data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeImportADO>(this, data);
        }

        public int PARENT_CODE_ERROR { get; set; }
        public int STOP_IMP_ERROR { get; set; }
        public int IN_KTC_FEE_ERROR { get; set; }
        public int ALLOW_ODD_ERROR { get; set; }
        public int STENT_ERROR { get; set; }
        public int REQUIRE_HSD_ERROR { get; set; }
        public int CHEMICAL_SUBSTANCE_ERROR { get; set; }
        public int BUSINESS_ERROR { get; set; }
        public int AUTO_EXPEND_ERROR { get; set; }
        public int ALLOW_EXPORT_ODD_ERROR { get; set; }
        public int OUT_PARENT_FEE_ERROR { get; set; }
        public int SALE_EQUAL_IMP_PRICE_ERROR { get; set; }
        public int HEIN_SERVICE_TYPE_CODE_ERROR { get; set; }
        public int SERVICE_UNIT_CODE_ERROR { get; set; }
        public int HEIN_LIMIT_PRICE_IN_TIME_STR_ERROR { get; set; }
        public int HEIN_LIMIT_PRICE_INTR_TIME_STR_ERROR { get; set; }
        public int NUM_ORDER_STR_ERROR { get; set; }
        public int HEIN_SERVICE_BHYT_CODE_ERROR { get; set; }
        public int HEIN_SERVICE_BHYT_NAME_ERROR { get; set; }
        public int HEIN_ORDER_ERROR { get; set; }
        public int HEIN_LIMIT_RATIO_STR_ERROR { get; set; }
        public int HEIN_LIMIT_RATIO_OLD_STR_ERROR { get; set; }
        public int HEIN_LIMIT_PRICE_OLD_STR_ERROR { get; set; }
        public int IMP_VAT_RATIO_STR_ERROR { get; set; }
        public int INTERNAL_PRICE_STR_ERROR { get; set; }
        public int IMP_PRICE_STR_ERROR { get; set; }
        public int ALERT_EXPIRED_DATE_STR_ERROR { get; set; }
        public int ALERT_MIN_IN_STOCK_STR_ERROR { get; set; }
        public int HEIN_LIMIT_PRICE_STR_ERROR { get; set; }
        public int MATERIAL_TYPE_CODE_ERROR { get; set; }
        public int MATERIAL_TYPE_NAME_ERROR { get; set; }
        public int PACKING_TYPE_NAME_ERROR { get; set; }
        public int NATIONAL_NAME_ERROR { get; set; }
        public int MANUFACTURER_CODE_ERROR { get; set; }
        public int MANUFACTURER_NAME_ERROR { get; set; }
        public int DESCRIPTION_ERROR { get; set; }
        public int CONCENTRA_ERROR { get; set; }
        public int ALERT_MAX_IN_PRESCRIPTION_STR_ERROR { get; set; }
        public int COGS_STR_ERROR { get; set; }
        public int TDL_GENDER_CODE_ERROR { get; set; }
        public int MATERIAL_GROUP_BHYT_ERROR { get; set; }
        public int RECORDING_TRANSACTION_ERROR { get; set; }
        public int ALERT_MAX_IN_DAY_STR_ERROR { get; set; }
        public int IDENTITY_MANAGEMEN_ERROR { get; set; }
    }
}
