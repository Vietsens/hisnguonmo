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
    public class ImpMestMedicinePrintADO : MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE
    {
        public bool IS_MEDICINE { get; set; }
        public bool IS_BLOOD { get; set; }
        public short? IS_CHEMICAL_SUBSTANCE { set; get; }
        public string USE_TIME_TO_STR { get; set; }
        public decimal? AMOUNT_IMPORTED { get; set; }
        public decimal? AMOUNT_APPROVED { get; set; }
        public string AMOUNT_IMPORTED_STRING { get; set; }
        public string AMOUNT_APPROVED_STRING { get; set; }
        public string AMOUNT_STRING { get; set; }
        public string DESCRIPTION { get; set; }
        public string CONCENTRA_PACKING_TYPE_NAME { get; set; }
        public string IS_BHYT { get; set; }
        public long ROOM_ASSIGN_ID { get; set; }
        public long INTRUCTION_TIME { get; set; }
        public decimal? PRICE_EXPORTED { get; set; }
        public string TREATMENT_CODE { get; set; }

        public MOS.EFMODEL.DataModels.V_HIS_PATIENT Patient { get; set; }
        public long TreatmentId { get; set; }
    }
}
