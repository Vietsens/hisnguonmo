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

namespace HIS.Desktop.Plugins.HisMedicalContractImport.ADO
{
    class HisMedicalContractADO : HIS_MEDI_CONTRACT_METY
    {
        public long STT { get; set; }
        public bool isMedicine { get; set; }
        public string IS_MEDICINE { get; set; }
        public string dataType_forDisplay { get; set; }
        public string MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAMEorMATERIAL_TYPE_NAME { get; set; }
        public string BHYT { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        //public decimal AMOUNT { get; set; }
        //public decimal? CONTRACT_PRICE { get; set; }
        public string MEDICINE_USE_FORM_CODE { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        //public string ACTIVE_INGR_BHYT_NAME { get; set; }
        //public string MEDICINE_REGISTER_NUMBER { get; set; }
        public string MANUFACTURER_CODE { get; set; }
        //public long? MANUFACTURER_ID { get; set; }
        //public string NATIONAL_NAME { get; set; }
        //public string BID_GROUP_CODE { get; set; }
        //public long? IMP_EXPIRED_DATE { get; set; }
        public string IMP_EXPIRED_DATE_ForDisplay { get; set; }
        //public long? EXPIRED_DATE { get; set; }
        public string EXPIRED_DATE_ForDisplay { get; set; }
        //public string CONCENTRA { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public long? SUPPLIER_ID { get; set; }
        //public string BID_NUMBER { get; set; }
        public long? BID_ID { get; set; }
        //public long? MONTH_LIFESPAN { get; set; }
        //public long? DAY_LIFESPAN { get; set; }
        //public long? HOUR_LIFESPAN { get; set; }
        public string MEDICAL_CONTRACT_CODE { get; set; }
        public long? VALID_FROM_DATE { get; set; }
        public string VALID_FROM_DATE_ForDisplay { get; set; }
        public long? VALID_TO_DATE { get; set; }
        public string VALID_TO_DATE_ForDisplay { get; set; }
        public string VENTURE_AGREENING { get; set; }
        //public string NOTE { get; set; }
        public long? DOCUMENT_SUPPLIER_ID { get; set; }
        public string DOCUMENT_SUPPLIER_CODE { get; set; }
        public string DOCUMENT_SUPPLIER_NAME { get; set; }
        public string ERROR { get; set; }

        public long? DISCOUNT_FROM_DATE { get; set; }
        public string DISCOUNT_FROM_DATE_STR { get; set; }
        public long? DISCOUNT_TO_DATE { get; set; }
        public string DISCOUNT_TO_DATE_STR { get; set; }
    }
}
