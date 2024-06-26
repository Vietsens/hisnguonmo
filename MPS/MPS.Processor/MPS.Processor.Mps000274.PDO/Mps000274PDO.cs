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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000274.PDO
{
    /// <summary>
    /// In yeu cau kham.
    /// Dau vao:
    /// PatyAlterBhyt: doi tuong the bhyt
    /// ServiceReq: yeu cau dich vu
    /// PatientType: doi tuong benh nhan
    /// </summary>
    /// 
    public partial class Mps000274PDO : RDOBase
    {
        public const string printTypeCode = "Mps000274";

        public List<V_HIS_TREATMENT_BED_ROOM> ListTreatmentBedRooms { get; set; }
        public List<V_HIS_TREATMENT> ListTreatments { get; set; }
        public List<HIS_RATION_TIME> ListRationTime { get; set; }
        public List<HIS_PATIENT_TYPE> ListPatientType { get; set; }

        public Mps000274PDO() { }

        public Mps000274PDO(V_HIS_RATION_SUM rationSum, List<V_HIS_SERE_SERV_15> ListSereServ, Mps000274ADO ado)
        {
            this.rationSum = rationSum;
            this.ListSereServ = ListSereServ;
            this.ado = ado;
        }

        public Mps000274PDO(V_HIS_RATION_SUM rationSum, List<V_HIS_SERE_SERV_15> ListSereServ, Mps000274ADO ado,
            List<V_HIS_TREATMENT> _ListTreatments, List<V_HIS_TREATMENT_BED_ROOM> _ListTreatmentBedRooms,
            List<HIS_RATION_TIME> _ListRationTime, List<HIS_PATIENT_TYPE> _ListPatientType)
        {
            this.rationSum = rationSum;
            this.ListSereServ = ListSereServ;
            this.ado = ado;
            this.ListTreatments = _ListTreatments;
            this.ListTreatmentBedRooms = _ListTreatmentBedRooms;
            this.ListRationTime = _ListRationTime;
            this.ListPatientType = _ListPatientType;
        }
    }
}
