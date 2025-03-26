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

namespace HIS.Desktop.Plugins.MedicineIsUsedPatient.ADO
{
    class ExpMestMediMateADO
    {
        public long ID { get; set; }
        public string CONCRETE_ID__IN_SETY { get; set; }
        public string SERVICE_REQ_CODE { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string INTRUCTION_TIME { get; set; }
        public long? MEDIMATE_ID { get; set; }
        public string MEDIMATE_TYPE_CODE { get; set; }
        public string MEDIMATE_TYPE_NAME { get; set; }
        public bool? IS_USED { get; set; }
        public bool IS_MEDICINE { get; set; }
        public bool IS_MATERIAL { get; set; }
        public decimal? AMOUNT { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public long EXP_MEST_MEDI_MATE_ID { get; set; }
        public bool IS_PARENT { get; set; }
        public string MORNING { get; set; }
        public bool? MORNING_CHK { get; set; }
        public bool? LUNCH_CHK { get; set; }
        public bool? AFTERNOON_CHK { get; set; }
        public bool? DINNER_CHK { get; set; }
        public string NOON { get; set; }
        public string AFTERNOON { get; set; }
        public string EVENING { get; set; }
        public short? MORNING_IS_USED { get; set; }
        public short? NOON_IS_USED { get; set; }
        public short? AFTERNOON_IS_USED { get; set; }
        public short? EVENING_IS_USED { get; set; }
        public ExpMestMediMateADO()
        {
        }
    }
}
