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

namespace MPS.Processor.Mps000258
{
    public class Mps000258SDO : V_HIS_SERVICE_PATY
    {
        public string HEIN_SERVICE_BHYT_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_NAME { get; set; }
        public long? PARENT_ID { get; set; }
        public short? IS_LEAF { get; set; }

        public decimal? HEIN_LIMIT_PRICE { get; set; }
        public long? HEIN_LIMIT_PRICE_IN_TIME { get; set; }
        public long? HEIN_LIMIT_PRICE_INTR_TIME { get; set; }
        public decimal? HEIN_LIMIT_PRICE_OLD { get; set; }
        public decimal? HEIN_LIMIT_RATIO { get; set; }
        public decimal? HEIN_LIMIT_RATIO_OLD { get; set; }

        public decimal? PRICE_1 { get; set; }
        public decimal? VAT_RATIO_1 { get; set; }
        public string PATIENT_TYPE_NAME_1 { get; set; }
        public decimal? PRICE_2 { get; set; }
        public decimal? VAT_RATIO_2 { get; set; }
        public string PATIENT_TYPE_NAME_2 { get; set; }
        public decimal? PRICE_3 { get; set; }
        public decimal? VAT_RATIO_3 { get; set; }
        public string PATIENT_TYPE_NAME_3 { get; set; }
        public decimal? PRICE_4 { get; set; }
        public decimal? VAT_RATIO_4 { get; set; }
        public string PATIENT_TYPE_NAME_4 { get; set; }
        public decimal? PRICE_5 { get; set; }
        public decimal? VAT_RATIO_5 { get; set; }
        public string PATIENT_TYPE_NAME_5 { get; set; }
        public decimal? PRICE_6 { get; set; }
        public decimal? VAT_RATIO_6 { get; set; }
        public string PATIENT_TYPE_NAME_6 { get; set; }
        public decimal? PRICE_7 { get; set; }
        public decimal? VAT_RATIO_7 { get; set; }
        public string PATIENT_TYPE_NAME_7 { get; set; }
        public decimal? PRICE_8 { get; set; }
        public decimal? VAT_RATIO_8 { get; set; }
        public string PATIENT_TYPE_NAME_8 { get; set; }
        public decimal? PRICE_9 { get; set; }
        public decimal? VAT_RATIO_9 { get; set; }
        public string PATIENT_TYPE_NAME_9 { get; set; }
        public decimal? PRICE_10 { get; set; }
        public decimal? VAT_RATIO_10 { get; set; }
        public string PATIENT_TYPE_NAME_10 { get; set; }
        public decimal? PRICE_11 { get; set; }
        public decimal? VAT_RATIO_11 { get; set; }
        public string PATIENT_TYPE_NAME_11 { get; set; }
        public decimal? PRICE_12 { get; set; }
        public decimal? VAT_RATIO_12 { get; set; }
        public string PATIENT_TYPE_NAME_12 { get; set; }
        public decimal? PRICE_13 { get; set; }
        public decimal? VAT_RATIO_13 { get; set; }
        public string PATIENT_TYPE_NAME_13 { get; set; }
        public decimal? PRICE_14 { get; set; }
        public decimal? VAT_RATIO_14 { get; set; }
        public string PATIENT_TYPE_NAME_14 { get; set; }
        public decimal? PRICE_15 { get; set; }
        public decimal? VAT_RATIO_15 { get; set; }
        public string PATIENT_TYPE_NAME_15 { get; set; }
    }
}
