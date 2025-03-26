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
using DevExpress.XtraBars.Ribbon.Drawing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisImportBid.ADO
{
    class ImportADO : MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE
    {
        // boo sung hieu luc tu hieu luc den ma ap thau
        public string VALID_FROM_TIME { get; set; }
        public string VALID_TO_TIME { get; set; }
        public string BID_APTHAU_CODE { get; set; }

        public string IS_MEDICINE { get; set; }

        public double IdRow { get; set; }
        public long Type { get; set; }
        public decimal? ImpVatRatio { get; set; }
        public decimal? AMOUNT { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string BID_NUM_ORDER { get; set; }
        public string BID_NAME { get; set; }
        public string BID_EXTRA_CODE { get; set; }
        public string BID_NUMBER { get; set; }
        public string BID_TYPE_CODE { get; set; }
        public string BID_GROUP_CODE { get; set; }
        public string BID_PACKAGE_CODE { get; set; }
        public string BID_YEAR { get; set; }
        public long? BID_TYPE_ID { get; set; }
        public string BID_TYPE_NAME { get; set; }
        public string MATERIAL_TYPE_MAP_CODE { get; set; }
        public string JOIN_BID_MATERIAL_TYPE_CODE { get; set; }
        public string BID_MATERIAL_TYPE_CODE { get; set; }
        public string BID_MATERIAL_TYPE_NAME { get; set; }

        //public string REGISTER_NUMBER { get; set; }
        public long? MONTH_LIFESPAN { get; set; }
        public long? DAY_LIFESPAN { get; set; }
        public long? HOUR_LIFESPAN { get; set; }

        public string MONTH_LIFESPAN_STR { get; set; }
        public string DAY_LIFESPAN_STR { get; set; }
        public string HOUR_LIFESPAN_STR { get; set; }

        public string ERROR { get; set; }

        public string ErrorDesc { get; set; }

        public bool IsNotNullRow{
            get {
                bool valid = true;
                if (string.IsNullOrEmpty(MEDICINE_GROUP_CODE) &&
                    string.IsNullOrEmpty(BID_NUM_ORDER) &&
                    string.IsNullOrEmpty(SUPPLIER_CODE) &&
                    AMOUNT == null &&
                    IMP_PRICE == null &&
                    IMP_VAT_RATIO == null &&
                    string.IsNullOrEmpty(BID_TYPE_CODE) &&
                    string.IsNullOrEmpty(BID_PACKAGE_CODE) &&
                    string.IsNullOrEmpty(BID_GROUP_CODE) &&
                    string.IsNullOrEmpty(BID_NUMBER) &&
                    string.IsNullOrEmpty(BID_NAME) &&
                    string.IsNullOrEmpty(HEIN_SERVICE_BHYT_NAME) &&
                    string.IsNullOrEmpty(PACKING_TYPE_NAME) &&
                    string.IsNullOrEmpty(BID_MATERIAL_TYPE_CODE) &&
                    string.IsNullOrEmpty(BID_MATERIAL_TYPE_NAME) &&
                    string.IsNullOrEmpty(JOIN_BID_MATERIAL_TYPE_CODE) &&
                    string.IsNullOrEmpty(MEDICINE_USE_FORM_CODE) &&
                    string.IsNullOrEmpty(BID_YEAR) &&
                    string.IsNullOrEmpty(CONCENTRA) &&
                    string.IsNullOrEmpty(REGISTER_NUMBER) &&
                    string.IsNullOrEmpty(MANUFACTURER_CODE) &&
                    string.IsNullOrEmpty(NATIONAL_NAME) &&
                    string.IsNullOrEmpty(VALID_FROM_TIME) &&
                    string.IsNullOrEmpty(VALID_TO_TIME) &&
                    string.IsNullOrEmpty(BID_APTHAU_CODE) &&
                    string.IsNullOrWhiteSpace(BID_GROUP_CODE.Trim()) &&
                    string.IsNullOrWhiteSpace(HEIN_SERVICE_BHYT_NAME.Trim()) &&
                    string.IsNullOrWhiteSpace(PACKING_TYPE_NAME.Trim()) &&
                    string.IsNullOrWhiteSpace(ACTIVE_INGR_BHYT_NAME.Trim()) &&
                    string.IsNullOrWhiteSpace(MEDICINE_USE_FORM_CODE.Trim()) &&
                    string.IsNullOrWhiteSpace(DOSAGE_FORM) &&
                    string.IsNullOrWhiteSpace(BID_MATERIAL_TYPE_CODE) &&
                    string.IsNullOrWhiteSpace(BID_MATERIAL_TYPE_NAME) &&
                    string.IsNullOrWhiteSpace(JOIN_BID_MATERIAL_TYPE_CODE) &&
                    string.IsNullOrWhiteSpace(BID_APTHAU_CODE) &&
                    string.IsNullOrWhiteSpace(REGISTER_NUMBER)
                    )      
                    valid =false;
                return valid;
            }
        }
    }
}
