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
using FlexCel.Report;

namespace MPS.Core.Mps000069
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000069RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal List<HIS_CARE> careByTreatmentHasIcd { get; set; }
        internal List<MPS.ADO.CareViewPrintADO> lstCareViewPrintADO { get; set; }
        internal List<MPS.ADO.CareDetailViewPrintADO> lstCareDetailViewPrintADO { get; set; }
        internal Mps000069ADO mps000069ADO;
        internal HIS_TREATMENT currentTreatment { get; set; }

        public Mps000069RDO(
            PatientADO patient,
            Mps000069ADO mps000069ADO,
            HIS_TREATMENT currentTreatment,
            List<HIS_CARE> careByTreatmentHasIcd,
            List<MPS.ADO.CareViewPrintADO> lstCareViewPrintADO,
            List<MPS.ADO.CareDetailViewPrintADO> lstCareDetailViewPrintADO
            )
        {
            try
            {
                this.Patient = patient;
                this.mps000069ADO = mps000069ADO;
                this.currentTreatment = currentTreatment;
                this.careByTreatmentHasIcd = careByTreatmentHasIcd;
                this.lstCareViewPrintADO = lstCareViewPrintADO;
                this.lstCareDetailViewPrintADO = lstCareDetailViewPrintADO;
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
                GlobalQuery.AddObjectKeyIntoListkey<HIS_TREATMENT>(currentTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<Mps000069ADO>(mps000069ADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
