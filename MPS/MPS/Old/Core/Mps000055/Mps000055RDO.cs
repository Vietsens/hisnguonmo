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

namespace MPS.Core.Mps000055
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000055RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal TreatmentADO currentHisTreatment { get; set; }
        internal V_HIS_MEDI_REACT serviceReq { get; set; }
        internal List<MPS.ADO.ExeMediReactADO> expMestMediReact { get; set; }
        string bedRoomName;

        public Mps000055RDO(
            PatientADO patient,
            TreatmentADO currentHisTreatment,
            V_HIS_MEDI_REACT serviceReq,
            List<MPS.ADO.ExeMediReactADO> expMestMediReact,
            string bedRoomName
            )
        {
            try
            {
                this.Patient = patient;
                this.currentHisTreatment = currentHisTreatment;
                this.serviceReq = serviceReq;
                this.expMestMediReact = expMestMediReact;
                this.bedRoomName = bedRoomName;
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
                keyValues.Add(new KeyValue(Mps000055ExtendSingleKey.BED_ROOM_NAME, bedRoomName));
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_MEDI_REACT>(serviceReq, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
