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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000008.PDO
{
    public partial class Mps000008PDO : RDOBase
    {
        public const string printTypeCode = "Mps000008";

        public long timeIn { get; set; }
        public List<HIS_APPOINTMENT_PERIOD> ListAppointmentPeriod;
        public HIS_TRACKING HisTracking;

        public Mps000008PDO() { }

        public Mps000008PDO(
            PatientADO Patient,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT currentTreatment,
            Mps000008ADO mps000008Ado,
            long timeIn,
            List<V_HIS_EKIP_USER> _listEkipUser
            )
        {
            try
            {
                this.Patient = Patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentTreatment = currentTreatment;
                this.timeIn = timeIn;
                this.Mps000008ADO = mps000008Ado;
                this.listEkipUser = _listEkipUser;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000008PDO(
            PatientADO Patient,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT currentTreatment,
            Mps000008ADO mps000008Ado,
            long timeIn,
            List<V_HIS_EKIP_USER> _listEkipUser,
            List<HIS_APPOINTMENT_PERIOD> _listAppointmentPeriod,
            HIS_TRACKING tracking
            )
        {
            try
            {
                this.Patient = Patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentTreatment = currentTreatment;
                this.timeIn = timeIn;
                this.Mps000008ADO = mps000008Ado;
                this.listEkipUser = _listEkipUser;
                this.ListAppointmentPeriod = _listAppointmentPeriod;
                this.HisTracking = tracking;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
