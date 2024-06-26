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

namespace MPS.Core.Mps000151
{
   public class Mps000151RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        internal List<HIS_CARE> careByTreatmentHasIcd { get; set; }
        internal HIS_ICD icd { get; set; }
        internal V_HIS_CARE_SUM currentSumCare { get; set; }
        internal List<MPS.ADO.CareViewPrintADO> lstCareViewPrintADO { get; set; }
        internal List<MPS.ADO.CareDetailViewPrintADO> lstCareDetailViewPrintADO { get; set; }
        internal Mps000151ADO mps000151ADO;

        public Mps000151RDO(
            PatientADO patient,
            V_HIS_CARE_SUM currentSumCare,
            List<MPS.ADO.CareViewPrintADO> lstCareViewPrintADO,
            List<MPS.ADO.CareDetailViewPrintADO> lstCareDetailViewPrintADO
            )
        {
            try
            {
                this.Patient = patient;
                this.currentSumCare = currentSumCare;
                this.lstCareViewPrintADO = lstCareViewPrintADO;
                this.lstCareDetailViewPrintADO = lstCareDetailViewPrintADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000151RDO(
            PatientADO patient,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            List<HIS_CARE> careByTreatmentHasIcd,
            HIS_ICD icd,
            List<MPS.ADO.CareViewPrintADO> lstCareViewPrintADO,
            List<MPS.ADO.CareDetailViewPrintADO> lstCareDetailViewPrintADO
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.careByTreatmentHasIcd = careByTreatmentHasIcd;
                this.icd = icd;
                this.lstCareViewPrintADO = lstCareViewPrintADO;
                this.lstCareDetailViewPrintADO = lstCareDetailViewPrintADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000151RDO(
            PatientADO patient,
            Mps000151ADO mps000151ADO,
            List<HIS_CARE> careByTreatmentHasIcd,
            List<MPS.ADO.CareViewPrintADO> lstCareViewPrintADO,
            List<MPS.ADO.CareDetailViewPrintADO> lstCareDetailViewPrintADO
            )
        {
            try
            {
                this.Patient = patient;
                this.mps000151ADO = mps000151ADO;
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
                GlobalQuery.AddObjectKeyIntoListkey<Mps000151ADO>(mps000151ADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_CARE_SUM>(currentSumCare, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
