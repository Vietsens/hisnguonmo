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

namespace MPS.Processor.Mps000302.ADO
{
    public class PatyAlterBhytADO : MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER
    {
        public string PATIENT_TYPE_NAME { get; set; }
        public string HEIN_CARD_NUMBER_SEPARATE { get; set; }
        public string IS_HEIN { get; set; }
        public string IS_VIENPHI { get; set; }
        public string STR_HEIN_CARD_FROM_TIME { get; set; }
        public string STR_HEIN_CARD_TO_TIME { get; set; }
        public string RATIO { get; set; }
        public string HEIN_CARD_NUMBER_1 { get; set; }
        public string HEIN_CARD_NUMBER_2 { get; set; }
        public string HEIN_CARD_NUMBER_3 { get; set; }
        public string HEIN_CARD_NUMBER_4 { get; set; }
        public string HEIN_CARD_NUMBER_5 { get; set; }
        public string HEIN_CARD_NUMBER_6 { get; set; }
        public long TIME_IN_TREATMENT { get; set; }
        public string RATIO_STR { get; set; }
        public string KEY { get; set; }
        public string KBCB_TIME_FROM_STR { get; set; }
        public string KBCB_TIME_TO_STR { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public decimal? TOTAL_PRICE_BHYT { get; set; }
        public decimal? TOTAL_PRICE_HEIN { get; set; }
        public decimal? TOTAL_PRICE_OTHER { get; set; }
        public decimal? TOTAL_PRICE_PATIENT { get; set; }
        public decimal? TOTAL_PRICE_PATIENT_SELF { get; set; }

        public decimal TOTAL_PATIENT_PRICE_LEFT { get; set; }
        public decimal TOTAL_PRICE_VP { get; set; }
        public decimal TOTAL_BHYT_PRICE { get; set; }
    }
}
