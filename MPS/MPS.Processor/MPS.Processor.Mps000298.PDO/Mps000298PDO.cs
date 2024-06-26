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

namespace MPS.Processor.Mps000298.PDO
{
    public partial class Mps000298PDO : RDOBase
    {
        public Mps000298PDO() { }

        public Mps000298PDO(V_HIS_TREATMENT treatment)
        {
            this.TreatmentView = treatment;
        }

        public Mps000298PDO(V_HIS_TREATMENT treatment,
            V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
            HIS_SERE_SERV _hisSereServ)
        {
            this.TreatmentView = treatment;
            this.PatientTypeAlter = patientTypeAlter;
            this._HisSereServ = _hisSereServ;
        }

        public Mps000298PDO(V_HIS_TREATMENT treatment,
            V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
            HIS_PATIENT patient,
            HIS_SERE_SERV _hisSereServ)
        {
            this.TreatmentView = treatment;
            this.PatientTypeAlter = patientTypeAlter;
            this._HisSereServ = _hisSereServ;
            this.Patient = patient;
        }
    }
}
