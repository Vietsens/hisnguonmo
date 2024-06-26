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

namespace MPS.Processor.Mps000032.PDO
{
    public partial class Mps000032PDO : RDOBase
    {
        public PatientADO Patient { get; set; }
        public TreatmentADO Treatment { get; set; }
        public PatyAlterBhytADO PatyAlterBhyt { get; set; }       

        public Mps000032PDO(PatientADO patient,
            PatyAlterBhytADO PatyAlterBhyt,
            TreatmentADO currentHisTreatment,
            MOS.EFMODEL.DataModels.V_HIS_SUIM_SERVICE_REQ SuimServiceReq,
            MOS.EFMODEL.DataModels.V_HIS_SERE_SERV SereSev
        )
        {
            try
            {
                this.suimServiceReq = SuimServiceReq;
                this.Patient = patient;
                this.Treatment = currentHisTreatment;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.sereServ = SereSev;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);   
            }
        }
    }
}
