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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000304.PDO
{
    public class HisConfigValue
    {
        public bool IsPriceWithDifference { get; set; }

        public bool IsNotSameDepartment { get; set; }
        public bool IsGroupReqDepartment { get; set; }
    }

    public class SingleKeyValue
    {
        public string ratio { get; set; }
        public long today { get; set; }
        public string departmentName { get; set; }
        public string currentDateSeparateFullTime { get; set; }
        public string userNameReturnResult { get; set; }
        public string statusTreatmentOut { get; set; }
        public string mediStockName { get; set; }
        public string roomName { get; set; }
        public string BankQrCode { get; set; }
    }

    public class HeinServiceTypeCFG
    {
        public long? HEIN_SERVICE_TYPE__HIGHTECH_ID { get; set; }
        public long? HEIN_SERVICE_TYPE__MATERIAL_VTTT_ID { get; set; }
        public long? HEIN_SERVICE_TYPE__EXAM_ID { get; set; }
        public long? HEIN_SERVICE_TYPE__SURG_MISU_ID { get; set; }
        public long? HEIN_SERVICE_TYPE__MEDI_MATE_FROM_CABINET_ID { get; set; }
    }

    public class PatientTypeCFG
    {
        public long? PATIENT_TYPE__BHYT { get; set; }
        public long? PATIENT_TYPE__FEE { get; set; }
    }

    public class ServiceTypeCFG
    {
        public long? SERVICE_TYPE_ID__BED { get; set; }
        public long? SERVICE_TYPE_ID__BLOOD { get; set; }
        public long? SERVICE_TYPE_ID__DIIM { get; set; }
        public long? SERVICE_TYPE_ID__ENDO { get; set; }
        public long? SERVICE_TYPE_ID__EXAM { get; set; }
        public long? SERVICE_TYPE_ID__FUEX { get; set; }
        public long? SERVICE_TYPE_ID__MATE { get; set; }
        public long? SERVICE_TYPE_ID__MEDI { get; set; }
        public long? SERVICE_TYPE_ID__MISU { get; set; }
        public long? SERVICE_TYPE_ID__PAAN { get; set; }
        public long? SERVICE_TYPE_ID__REHA { get; set; }
        public long? SERVICE_TYPE_ID__SUIM { get; set; }
        public long? SERVICE_TYPE_ID__SURG { get; set; }
        public long? SERVICE_TYPE_ID__TEST { get; set; }
    }
}
