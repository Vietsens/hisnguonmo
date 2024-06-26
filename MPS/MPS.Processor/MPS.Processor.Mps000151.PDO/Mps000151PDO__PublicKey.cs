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
        public V_HIS_PATIENT Patient { get; set; }
        public Mps000151ADO mps000151ADO;
        public string genderCode__Male;
        public string genderCode__FeMale;

        public List<HIS_CARE> ListHisCareByTreatment { get; set; }
        public List<HIS_DHST> ListHisDhstByHisCare { get; set; }
        //public List<HIS_AWARENESS> ListHisAwareness { get; set; }
        public List<V_HIS_CARE_DETAIL> ListCareDetail { get; set; }
        public List<ACS_USER> ListUser { get; set; }
        public List<CareDescription> lstCareDescription { get; set; }

        public List<InstructionDescription> lstInstructionDescription { get; set; }
        public class Mps000151ADO
        {
            public string DEPARTMENT_NAME { get; set; }
            public string ROOM_NAME { get; set; }
            public string BED_CODE { get; set; }
            public string BED_NAME { get; set; }
            public long? ICD_ID { get; set; }
            public string ICD_MAIN_TEXT { get; set; }
            public string ICD_SUB_CODE { get; set; }
            public string ICD_CODE { get; set; }
            public string ICD_NAME { get; set; }
            public string ICD_TEXT { get; set; }
        }

        public class CareDescription
        {
            public long PARENT_ID { get; set; }
            public string CARE_DESCRIPTION { get; set; }
            public string CARE_DESCRIPTION_1 { get; set; }
            public string CARE_DESCRIPTION_2 { get; set; }
            public string CARE_DESCRIPTION_3 { get; set; }
            public string CARE_DESCRIPTION_4 { get; set; }
            public string CARE_DESCRIPTION_5 { get; set; }
            public string CARE_DESCRIPTION_6 { get; set; }
            public string CARE_DESCRIPTION_7 { get; set; }
            public string CARE_DESCRIPTION_8 { get; set; }
            public string CARE_DESCRIPTION_9 { get; set; }
            public string CARE_DESCRIPTION_10 { get; set; }
            public string CARE_DESCRIPTION_11 { get; set; }
            public string CARE_DESCRIPTION_12 { get; set; }
        }

        public class InstructionDescription
        {
            public long PARENT_ID { get; set; }
            public string INSTRUCTION_DESCRIPTION { get; set; }
            public string INSTRUCTION_DESCRIPTION_1 { get; set; }
            public string INSTRUCTION_DESCRIPTION_2 { get; set; }
            public string INSTRUCTION_DESCRIPTION_3 { get; set; }
            public string INSTRUCTION_DESCRIPTION_4 { get; set; }
            public string INSTRUCTION_DESCRIPTION_5 { get; set; }
            public string INSTRUCTION_DESCRIPTION_6 { get; set; }
            public string INSTRUCTION_DESCRIPTION_7 { get; set; }
            public string INSTRUCTION_DESCRIPTION_8 { get; set; }
            public string INSTRUCTION_DESCRIPTION_9 { get; set; }
            public string INSTRUCTION_DESCRIPTION_10 { get; set; }
            public string INSTRUCTION_DESCRIPTION_11 { get; set; }
            public string INSTRUCTION_DESCRIPTION_12 { get; set; }
        }
    }
}
