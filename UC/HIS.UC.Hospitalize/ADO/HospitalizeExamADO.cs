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
using HIS.UC.SecondaryIcd.ADO;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.Hospitalize.ADO
{
    public class HospitalizeExamADO
    {
        public HisDepartmentTranHospitalizeSDO HisDepartmentTranHospitalizeSDO { get; set; }
        public bool IsPrintMps178 { get; set; }
        public bool IsPrintHospitalizeExam { get; set; }
        public long? FinishTime { get; set; }
        public bool IsSign { get; set; }
        public bool IsPrintSign { get; set; }

        public HIS.UC.Icd.ADO.IcdInputADO icdADOInTreatment { get; set; }
        public HIS.UC.Icd.ADO.IcdInputADO TraditionalIcdADOInTreatment { get; set; }
        public SecondaryIcdDataADO icdSubADOInTreatment { get; set; }
        public SecondaryIcdDataADO tradtionalIcdSub { get; set; }
        public string Note { get; set; }

    }
}
