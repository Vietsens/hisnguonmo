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

namespace MPS.Processor.Mps000069.PDO
{
    public partial class Mps000069PDO : RDOBase
    {
        public List<CreatorADO> _Creator { get; set; }
        public Mps000069PDO() { }

        public Mps000069PDO(
                        PatientADO patient,
                        Mps000069ADO mps000069ADO,
                        HIS_TREATMENT currentTreatment,
                        List<HIS_CARE> careByTreatmentHasIcd,
                        List<CareViewPrintADO> lstCareViewPrintADO,
                        List<CareDetailViewPrintADO> lstCareDetailViewPrintADO
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

        public Mps000069PDO(
                        PatientADO patient,
                        Mps000069ADO mps000069ADO,
                        HIS_TREATMENT currentTreatment,
                        List<HIS_CARE> careByTreatmentHasIcd,
                        List<CareViewPrintADO> lstCareViewPrintADO,
                        List<CareDetailViewPrintADO> lstCareDetailViewPrintADO,
             List<CreatorADO> _creator,
            List<CareDescription> _careDescription
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
                this._Creator = _creator;
                this.lstCareDescription = _careDescription;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000069PDO(
                       PatientADO patient,
                       Mps000069ADO mps000069ADO,
                       HIS_TREATMENT currentTreatment,
                       List<HIS_CARE> careByTreatmentHasIcd,
                       List<CareViewPrintADO> lstCareViewPrintADO,
                       List<CareDetailViewPrintADO> lstCareDetailViewPrintADO,
            List<CreatorADO> _creator,
           List<CareDescription> _careDescription,
           List<InstructionDescription> instructionDescriptions
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
                this._Creator = _creator;
                this.lstCareDescription = _careDescription;
                this.lstInstructionDescription = instructionDescriptions;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
