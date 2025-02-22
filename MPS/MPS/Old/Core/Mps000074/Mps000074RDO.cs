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
using MPS.ADO;

namespace MPS.Core.Mps000074
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000074RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal TreatmentADO currentTreatment { get; set; }
        internal V_HIS_INFUSION Infusion { get; set; }
        internal List<MPS.ADO.ExeInfusionDetailSDO> infusionDetaiAdos {get; set;}

        public Mps000074RDO(
            PatientADO patient,
            TreatmentADO currentTreatment,
            V_HIS_INFUSION Infusion,
            List<MPS.ADO.ExeInfusionDetailSDO> infusionDetaiAdos
            )
        {
            try
            {
                this.Patient = patient;
                this.currentTreatment = currentTreatment;
                this.Infusion = Infusion;
                this.infusionDetaiAdos = infusionDetaiAdos;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_INFUSION>(Infusion, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
