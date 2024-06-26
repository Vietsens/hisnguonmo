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

namespace HIS.UC.BidMaterialTypeGrid
{
    public class MaterialTypeADO : V_HIS_MATERIAL_TYPE
    {
        public bool? IsChemicalSubstance { get; set; }
        public bool IsStopImp { get; set; }
        public bool? IsLeaf { get; set; }
        public bool? IsStent { get; set; }
        public bool? IsAutoExpend { get; set; }
        public bool? IsCPNG { get; set; }
        public bool? IsExprireDate { get; set; }
        public bool? IsAllowExportOdd { get; set; }
        public string heinLimitPriceInTimeStr { get; set; }
        public string heinLimitPriceIntrTimeStr { get; set; }
        public decimal? ImpVatRatio100 { get; set; }
        public decimal? HeinLimitVatRatio100 { get; set; }
        public decimal? HeinLimitVatRatioOld100 { get; set; }
        public decimal? AMOUNT { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public decimal? AllowAmount { get; set; }
        public long? BID_ID { get; set; }

        public MaterialTypeADO()
        {
        }

        public MaterialTypeADO(V_HIS_MATERIAL_TYPE _data)
        {
            this.IS_SALE_EQUAL_IMP_PRICE = _data.IS_SALE_EQUAL_IMP_PRICE;
            this.IS_BUSINESS = _data.IS_BUSINESS;

            this.ALERT_EXPIRED_DATE = _data.ALERT_EXPIRED_DATE;
            this.ALERT_MIN_IN_STOCK = _data.ALERT_MIN_IN_STOCK;
            this.APP_CREATOR = _data.APP_CREATOR;
            this.APP_MODIFIER = _data.APP_MODIFIER;
            this.BILL_OPTION = _data.BILL_OPTION;
            this.CONCENTRA = _data.CONCENTRA;
            this.CREATE_TIME = _data.CREATE_TIME;
            this.CREATOR = _data.CREATOR;
            this.DESCRIPTION = _data.DESCRIPTION;
            this.GROUP_CODE = _data.GROUP_CODE;
            this.HEIN_ORDER = _data.HEIN_ORDER;
            this.HEIN_SERVICE_BHYT_CODE = _data.HEIN_SERVICE_BHYT_CODE;
            this.HEIN_SERVICE_BHYT_NAME = _data.HEIN_SERVICE_BHYT_NAME;
            this.HEIN_SERVICE_TYPE_CODE = _data.HEIN_SERVICE_TYPE_CODE;
            this.HEIN_SERVICE_TYPE_ID = _data.HEIN_SERVICE_TYPE_ID;
            this.HEIN_SERVICE_TYPE_NAME = _data.HEIN_SERVICE_TYPE_NAME;
            this.ID = _data.ID;
            this.IMP_PRICE = _data.IMP_PRICE;
            this.IMP_VAT_RATIO = _data.IMP_VAT_RATIO;
            this.INTERNAL_PRICE = _data.INTERNAL_PRICE;
            this.IS_ACTIVE = _data.IS_ACTIVE;
            this.IS_ALLOW_ODD = _data.IS_ALLOW_ODD;
            this.IS_AUTO_EXPEND = _data.IS_AUTO_EXPEND;
            this.IS_CHEMICAL_SUBSTANCE = _data.IS_CHEMICAL_SUBSTANCE;
            this.IS_DELETE = _data.IS_DELETE;
            this.IS_IN_KTC_FEE = _data.IS_IN_KTC_FEE;
            this.IS_LEAF = _data.IS_LEAF;
            this.IS_OUT_PARENT_FEE = _data.IS_OUT_PARENT_FEE;
            this.IS_REQUIRE_HSD = _data.IS_REQUIRE_HSD;
            this.IS_STENT = _data.IS_STENT;
            this.IS_STOP_IMP = _data.IS_STOP_IMP;
            this.IS_ALLOW_EXPORT_ODD = _data.IS_ALLOW_EXPORT_ODD;
            this.MANUFACTURER_CODE = _data.MANUFACTURER_CODE;
            this.MANUFACTURER_ID = _data.MANUFACTURER_ID;
            this.MANUFACTURER_NAME = _data.MANUFACTURER_NAME;
            this.MATERIAL_TYPE_CODE = _data.MATERIAL_TYPE_CODE;
            this.MATERIAL_TYPE_NAME = _data.MATERIAL_TYPE_NAME;
            this.MEMA_GROUP_ID = _data.MEMA_GROUP_ID;
            this.MODIFIER = _data.MODIFIER;
            this.MODIFY_TIME = _data.MODIFY_TIME;
            this.NATIONAL_NAME = _data.NATIONAL_NAME;
            this.NUM_ORDER = _data.NUM_ORDER;
            this.PACKING_TYPE_NAME = _data.PACKING_TYPE_NAME;
            this.HEIN_LIMIT_PRICE = _data.HEIN_LIMIT_PRICE;
            this.HEIN_LIMIT_PRICE_OLD = _data.HEIN_LIMIT_PRICE_OLD;
            this.HEIN_LIMIT_RATIO = _data.HEIN_LIMIT_RATIO;
            this.HEIN_LIMIT_RATIO_OLD = _data.HEIN_LIMIT_RATIO_OLD;
            this.HEIN_ORDER = _data.HEIN_ORDER;
            this.PACKING_TYPE_NAME = _data.PACKING_TYPE_NAME;
            this.ImpVatRatio100 = _data.IMP_VAT_RATIO != null ? _data.IMP_VAT_RATIO * 100 : null;
            this.HeinLimitVatRatio100 = _data.HEIN_LIMIT_RATIO != null ? _data.HEIN_LIMIT_RATIO * 100 : null;
            this.HeinLimitVatRatioOld100 = _data.HEIN_LIMIT_RATIO_OLD != null ? _data.HEIN_LIMIT_RATIO_OLD * 100 : null;
            this.heinLimitPriceInTimeStr = _data.HEIN_LIMIT_PRICE_IN_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(_data.HEIN_LIMIT_PRICE_IN_TIME ?? 0) : "";

            this.heinLimitPriceIntrTimeStr = _data.HEIN_LIMIT_PRICE_INTR_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(_data.HEIN_LIMIT_PRICE_INTR_TIME ?? 0) : "";
            this.PARENT_ID = _data.PARENT_ID;
            this.SERVICE_ID = _data.SERVICE_ID;
            this.SERVICE_TYPE_ID = _data.SERVICE_TYPE_ID;
            this.SERVICE_UNIT_CODE = _data.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_ID = _data.SERVICE_UNIT_ID;
            this.SERVICE_UNIT_NAME = _data.SERVICE_UNIT_NAME;
            IsLeaf = (_data.IS_LEAF == 1);
            this.IsChemicalSubstance = (_data.IS_CHEMICAL_SUBSTANCE == 1);
            this.IsStopImp = (_data.IS_STOP_IMP == 1);
            IsStent = (_data.IS_STENT == 1);
            IsAutoExpend = (_data.IS_AUTO_EXPEND == 1);

        }

        public MaterialTypeADO(MaterialTypeADO _data)
        {
            this.IS_SALE_EQUAL_IMP_PRICE = _data.IS_SALE_EQUAL_IMP_PRICE;
            this.IS_BUSINESS = _data.IS_BUSINESS;

            this.ALERT_EXPIRED_DATE = _data.ALERT_EXPIRED_DATE;
            this.ALERT_MIN_IN_STOCK = _data.ALERT_MIN_IN_STOCK;
            this.APP_CREATOR = _data.APP_CREATOR;
            this.APP_MODIFIER = _data.APP_MODIFIER;
            this.BILL_OPTION = _data.BILL_OPTION;
            this.CONCENTRA = _data.CONCENTRA;
            this.CREATE_TIME = _data.CREATE_TIME;
            this.CREATOR = _data.CREATOR;
            this.DESCRIPTION = _data.DESCRIPTION;
            this.GROUP_CODE = _data.GROUP_CODE;
            this.HEIN_ORDER = _data.HEIN_ORDER;
            this.HEIN_SERVICE_BHYT_CODE = _data.HEIN_SERVICE_BHYT_CODE;
            this.HEIN_SERVICE_BHYT_NAME = _data.HEIN_SERVICE_BHYT_NAME;
            this.HEIN_SERVICE_TYPE_CODE = _data.HEIN_SERVICE_TYPE_CODE;
            this.HEIN_SERVICE_TYPE_ID = _data.HEIN_SERVICE_TYPE_ID;
            this.HEIN_SERVICE_TYPE_NAME = _data.HEIN_SERVICE_TYPE_NAME;
            this.ID = _data.ID;
            this.IMP_PRICE = _data.IMP_PRICE;
            this.IMP_VAT_RATIO = _data.IMP_VAT_RATIO;
            this.INTERNAL_PRICE = _data.INTERNAL_PRICE;
            this.IS_ACTIVE = _data.IS_ACTIVE;
            this.IS_ALLOW_ODD = _data.IS_ALLOW_ODD;
            this.IS_AUTO_EXPEND = _data.IS_AUTO_EXPEND;
            this.IS_CHEMICAL_SUBSTANCE = _data.IS_CHEMICAL_SUBSTANCE;
            this.IS_DELETE = _data.IS_DELETE;
            this.IS_IN_KTC_FEE = _data.IS_IN_KTC_FEE;
            this.IS_LEAF = _data.IS_LEAF;
            this.IS_OUT_PARENT_FEE = _data.IS_OUT_PARENT_FEE;
            this.IS_REQUIRE_HSD = _data.IS_REQUIRE_HSD;
            this.IS_STENT = _data.IS_STENT;
            this.IS_STOP_IMP = _data.IS_STOP_IMP;
            this.IS_ALLOW_EXPORT_ODD = _data.IS_ALLOW_EXPORT_ODD;
            this.MANUFACTURER_CODE = _data.MANUFACTURER_CODE;
            this.MANUFACTURER_ID = _data.MANUFACTURER_ID;
            this.MANUFACTURER_NAME = _data.MANUFACTURER_NAME;
            this.MATERIAL_TYPE_CODE = _data.MATERIAL_TYPE_CODE;
            this.MATERIAL_TYPE_NAME = _data.MATERIAL_TYPE_NAME;
            this.MEMA_GROUP_ID = _data.MEMA_GROUP_ID;
            this.MODIFIER = _data.MODIFIER;
            this.MODIFY_TIME = _data.MODIFY_TIME;
            this.NATIONAL_NAME = _data.NATIONAL_NAME;
            this.NUM_ORDER = _data.NUM_ORDER;
            this.PACKING_TYPE_NAME = _data.PACKING_TYPE_NAME;
            this.HEIN_LIMIT_PRICE = _data.HEIN_LIMIT_PRICE;
            this.HEIN_LIMIT_PRICE_OLD = _data.HEIN_LIMIT_PRICE_OLD;
            this.HEIN_LIMIT_RATIO = _data.HEIN_LIMIT_RATIO;
            this.HEIN_LIMIT_RATIO_OLD = _data.HEIN_LIMIT_RATIO_OLD;
            this.HEIN_ORDER = _data.HEIN_ORDER;
            this.PACKING_TYPE_NAME = _data.PACKING_TYPE_NAME;
            this.ImpVatRatio100 = _data.IMP_VAT_RATIO != null ? _data.IMP_VAT_RATIO * 100 : null;
            this.HeinLimitVatRatio100 = _data.HEIN_LIMIT_RATIO != null ? _data.HEIN_LIMIT_RATIO * 100 : null;
            this.HeinLimitVatRatioOld100 = _data.HEIN_LIMIT_RATIO_OLD != null ? _data.HEIN_LIMIT_RATIO_OLD * 100 : null;
            this.heinLimitPriceInTimeStr = _data.HEIN_LIMIT_PRICE_IN_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(_data.HEIN_LIMIT_PRICE_IN_TIME ?? 0) : "";

            this.heinLimitPriceIntrTimeStr = _data.HEIN_LIMIT_PRICE_INTR_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(_data.HEIN_LIMIT_PRICE_INTR_TIME ?? 0) : "";
            this.PARENT_ID = _data.PARENT_ID;
            this.SERVICE_ID = _data.SERVICE_ID;
            this.SERVICE_TYPE_ID = _data.SERVICE_TYPE_ID;
            this.SERVICE_UNIT_CODE = _data.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_ID = _data.SERVICE_UNIT_ID;
            this.SERVICE_UNIT_NAME = _data.SERVICE_UNIT_NAME;
            this.AMOUNT = _data.AMOUNT;
            this.SUPPLIER_ID = _data.SUPPLIER_ID;
            this.SUPPLIER_CODE = _data.SUPPLIER_CODE;
            this.SUPPLIER_NAME = _data.SUPPLIER_NAME;
            this.BID_ID = _data.BID_ID;
            IsLeaf = (_data.IS_LEAF == 1);
            this.IsChemicalSubstance = (_data.IS_CHEMICAL_SUBSTANCE == 1);
            this.IsStopImp = (_data.IS_STOP_IMP == 1);
            IsStent = (_data.IS_STENT == 1);
            IsAutoExpend = (_data.IS_AUTO_EXPEND == 1);

        }
    }
}
