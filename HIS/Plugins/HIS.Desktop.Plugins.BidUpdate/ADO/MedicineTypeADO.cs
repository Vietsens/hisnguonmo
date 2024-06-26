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

namespace HIS.Desktop.Plugins.BidUpdate.ADO
{
    public class MedicineTypeADO : MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE
    {
        public double IdRow { get; set; }
        public decimal? AMOUNT { get; set; }
        public long Type { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string BID_NUM_ORDER { get; set; }
        public string IS_MEDICINE { get; set; }
        public long BID_ID { get; set; }
        public long BID_MEDI_MATY_BLO_ID { get; set; }
        public decimal? ImpVatRatio { get; set; }
        public decimal? ImpMoreRatio { get; set; }
        public decimal? ADJUST_AMOUNT { get; set; }
        public string BID_GROUP_CODE { get; set; }
        public string BID_PACKAGE_CODE { get; set; }
        public string BID_EXTRA_CODE { get; set; }

        public decimal Min_AMOUNT { get; set; }
        public bool AllowDelete { get; set; }

        public long? EXPIRED_DATE { get; set; }

        public bool IsMaterialTypeMap { get; set; }

        public string MATERIAL_TYPE_MAP_CODE { get; set; }

        public long? MONTH_LIFESPAN { get; set; }
        public long? DAY_LIFESPAN { get; set; }
        public long? HOUR_LIFESPAN { get; set; }

        public string MONTH_LIFESPAN_STR { get; set; }
        public string DAY_LIFESPAN_STR { get; set; }
        public string HOUR_LIFESPAN_STR { get; set; }

        public string DOSAGE_FORM { get; set; }

        public string BID_MATERIAL_TYPE_CODE { get; set; }
        public string BID_MATERIAL_TYPE_NAME { get; set; }
        public string JOIN_BID_MATERIAL_TYPE_CODE { get; set; }
        public string NOTE { get; set; }
        public long? INFORMATION_BID { get; set; }
        public MedicineTypeADO() { }

        public bool IsNotNullRow
        {
            get
            {
                bool valid = true;
                if (string.IsNullOrEmpty(MEDICINE_GROUP_CODE) &&
                    string.IsNullOrEmpty(BID_NUM_ORDER) &&
                    string.IsNullOrEmpty(SUPPLIER_CODE) &&
                    AMOUNT == null &&
                    IMP_PRICE == null &&
                    IMP_VAT_RATIO == null &&
               
                    string.IsNullOrEmpty(BID_PACKAGE_CODE) &&
                    string.IsNullOrEmpty(BID_GROUP_CODE) &&
                   
                    string.IsNullOrEmpty(HEIN_SERVICE_BHYT_NAME) &&
                     string.IsNullOrEmpty(PACKING_TYPE_NAME) &&
                    string.IsNullOrEmpty(BID_MATERIAL_TYPE_CODE) &&
                    string.IsNullOrEmpty(BID_MATERIAL_TYPE_NAME) &&
                    string.IsNullOrEmpty(JOIN_BID_MATERIAL_TYPE_CODE) &&
                    string.IsNullOrEmpty(MEDICINE_USE_FORM_CODE) &&
                  
                     string.IsNullOrEmpty(CONCENTRA) &&
                    string.IsNullOrEmpty(REGISTER_NUMBER) &&
                    string.IsNullOrEmpty(MANUFACTURER_CODE) &&
                    string.IsNullOrEmpty(NATIONAL_NAME)
                    )
                    valid = false;
                return valid;
            }
        }
    }
}
