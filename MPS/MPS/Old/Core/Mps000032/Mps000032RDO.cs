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
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000032
{
    public class Mps000032RDO : RDOBase
    {
        internal MOS.EFMODEL.DataModels.V_HIS_SUIM_SERVICE_REQ suimServiceReq { get; set; }
        internal PatientADO Patient { get; set; }
        internal TreatmentADO Treatment { get; set; }
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_SERE_SERV sereServ { get; set; }

        public Mps000032RDO(PatientADO patient,
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

        internal override void SetSingleKey()
        {
            try
            {
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SUIM_SERVICE_REQ>(suimServiceReq, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>(this.sereServ, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
