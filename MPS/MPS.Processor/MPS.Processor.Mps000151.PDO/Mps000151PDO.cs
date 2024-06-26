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
using ACS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000151.PDO
{
    public partial class Mps000151PDO : RDOBase
    {
        public Mps000151PDO() { }

        public Mps000151PDO(
            V_HIS_PATIENT patient,
            Mps000151ADO mps000151ADO,
            List<HIS_CARE> listHisCareByTreatment,
            List<HIS_DHST> listHisDhstByHisCare,
            //List<HIS_AWARENESS> listHisAwareness,
            List<ACS_USER> listUser,
            List<V_HIS_CARE_DETAIL> listCareDetail,
            string genderCode__Male,
            string genderCode__FeMale
            )
        {
            try
            {
                this.Patient = patient;
                this.mps000151ADO = mps000151ADO;
                this.ListHisCareByTreatment = listHisCareByTreatment;
                this.ListHisDhstByHisCare = listHisDhstByHisCare;
                //this.ListHisAwareness = listHisAwareness;
                this.ListUser = listUser;
                this.ListCareDetail = listCareDetail;
                this.genderCode__Male = genderCode__Male;
                this.genderCode__FeMale = genderCode__FeMale;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000151PDO(
            V_HIS_PATIENT patient,
            Mps000151ADO mps000151ADO,
            List<HIS_CARE> listHisCareByTreatment,
            List<HIS_DHST> listHisDhstByHisCare,
            //List<HIS_AWARENESS> listHisAwareness,
            List<ACS_USER> listUser,
            List<V_HIS_CARE_DETAIL> listCareDetail,
            string genderCode__Male,
            string genderCode__FeMale,
             List<CareDescription> _careDescription,
           List<InstructionDescription> instructionDescriptions
            )
        {
            try
            {
                this.Patient = patient;
                this.mps000151ADO = mps000151ADO;
                this.ListHisCareByTreatment = listHisCareByTreatment;
                this.ListHisDhstByHisCare = listHisDhstByHisCare;
                //this.ListHisAwareness = listHisAwareness;
                this.ListUser = listUser;
                this.ListCareDetail = listCareDetail;
                this.genderCode__Male = genderCode__Male;
                this.genderCode__FeMale = genderCode__FeMale;
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
