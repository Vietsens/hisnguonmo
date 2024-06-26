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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000010.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000010PDO : RDOBase
    {
        public const string printTypeCode = "Mps000010";

        public List<V_HIS_APPOINTMENT_SERV> _AppointmentServs { get; set; }
        public List<HIS_APPOINTMENT_PERIOD> ListAppointmentPeriod { get; set; }

        public Mps000010PDO() { }

        public Mps000010PDO(
            PatientADO Patient,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT currentTreatment,
            Mps000010ADO _Mps000010ADO,
            List<V_HIS_APPOINTMENT_SERV> _appointmentServs)
        {
            try
            {
                this.Patient = Patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentTreatment = currentTreatment;
                this.Mps000010ADO = _Mps000010ADO;
                this._AppointmentServs = _appointmentServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000010PDO(
            PatientADO Patient,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT currentTreatment,
            Mps000010ADO _Mps000010ADO,
            List<V_HIS_APPOINTMENT_SERV> _appointmentServs,
            List<HIS_APPOINTMENT_PERIOD> listAppointmentPeriod)
        {
            try
            {
                this.Patient = Patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentTreatment = currentTreatment;
                this.Mps000010ADO = _Mps000010ADO;
                this._AppointmentServs = _appointmentServs;
                this.ListAppointmentPeriod = listAppointmentPeriod;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000010PDO(
            PatientADO Patient,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT currentTreatment,
            HIS_MEDI_RECORD _mediRecord,
            Mps000010ADO _Mps000010ADO,
            List<V_HIS_APPOINTMENT_SERV> _appointmentServs)
        {
            try
            {
                this.Patient = Patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentTreatment = currentTreatment;
                this.Mps000010ADO = _Mps000010ADO;
                this._AppointmentServs = _appointmentServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //public Mps000010PDO(
        //    PatientADO Patient,
        //    V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
        //    HIS_TREATMENT currentTreatment,
        //    Mps000010ADO _Mps000010ADO)
        //{
        //    try
        //    {
        //        this.Patient = Patient;
        //        this.PatyAlterBhyt = PatyAlterBhyt;
        //        this.currentTreatment = currentTreatment;
        //        this.Mps000010ADO = _Mps000010ADO;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}


        public Mps000010PDO(
            PatientADO Patient,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT currentTreatment,
            Mps000010ADO _Mps000010ADO,
            List<V_HIS_APPOINTMENT_SERV> _appointmentServs,
            HIS_SERVICE_REQ _ServiceReq)
        {
            try
            {
                this.Patient = Patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentTreatment = currentTreatment;
                this.Mps000010ADO = _Mps000010ADO;
                this._AppointmentServs = _appointmentServs;
                this.ServiceReq = _ServiceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000010PDO(
            PatientADO Patient,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT currentTreatment,
            Mps000010ADO _Mps000010ADO,
            List<V_HIS_APPOINTMENT_SERV> _appointmentServs,
            List<HIS_APPOINTMENT_PERIOD> listAppointmentPeriod,
            HIS_SERVICE_REQ _ServiceReq)
        {
            try
            {
                this.Patient = Patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentTreatment = currentTreatment;
                this.Mps000010ADO = _Mps000010ADO;
                this._AppointmentServs = _appointmentServs;
                this.ListAppointmentPeriod = listAppointmentPeriod;
                this.ServiceReq = _ServiceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000010PDO(
            PatientADO Patient,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT currentTreatment,
            HIS_MEDI_RECORD _mediRecord,
            Mps000010ADO _Mps000010ADO,
            List<V_HIS_APPOINTMENT_SERV> _appointmentServs,
            HIS_SERVICE_REQ _ServiceReq)
        {
            try
            {
                this.Patient = Patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentTreatment = currentTreatment;
                this.Mps000010ADO = _Mps000010ADO;
                this._AppointmentServs = _appointmentServs;
                this.ServiceReq = _ServiceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

