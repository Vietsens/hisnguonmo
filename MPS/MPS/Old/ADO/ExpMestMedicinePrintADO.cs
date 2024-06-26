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

namespace MPS.ADO
{
    public class ExpMestMedicinePrintADO : MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE
    {
        public int TYPE_ID { get; set; }//1:THuoc 2:VT 3:Mau
        public bool IS_MEDICINE { get; set; }
        public bool IS_BLOOD { get; set; }
        public short? IS_CHEMICAL_SUBSTANCE { set; get; }
        public string USE_TIME_TO_STR { get; set; }
        public decimal? AMOUNT_EXPORTED { get; set; }
        public decimal? AMOUNT_EXCUTE { get; set; }
        public decimal? AMOUNT_REQUEST { get; set; }
        public string AMOUNT_EXPORTED_STRING { get; set; }
        public string AMOUNT_EXCUTE_STRING { get; set; }
        public string AMOUNT_STRING { get; set; }
        public string DESCRIPTION { get; set; }
        public string CONCENTRA_PACKING_TYPE_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public string PACKING_TYPE_NAME { get; set; }
        public string TREATMENT_CODE { get; set; }

        public MOS.EFMODEL.DataModels.V_HIS_PATIENT Patient { get; set; }
        public long TreatmentId { get; set; }
    }
}
