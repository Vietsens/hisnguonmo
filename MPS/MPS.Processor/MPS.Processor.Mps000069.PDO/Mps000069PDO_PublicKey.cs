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
        public PatientADO Patient { get; set; }
        public List<HIS_CARE> careByTreatmentHasIcd { get; set; }
        public List<CareViewPrintADO> lstCareViewPrintADO { get; set; }
        public List<CareDetailViewPrintADO> lstCareDetailViewPrintADO { get; set; }
        public Mps000069ADO mps000069ADO;
        public HIS_TREATMENT currentTreatment { get; set; }
        public List<CareDescription> lstCareDescription { get; set; }

        public List<InstructionDescription> lstInstructionDescription { get; set; }
    }

    public class PatientADO : V_HIS_PATIENT
    {
        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_DATE_STR { get; set; }
        public string DOB_YEAR { get; set; }
        public string GENDER_MALE { get; set; }
        public string GENDER_FEMALE { get; set; }

        public PatientADO()
        {

        }

        public PatientADO(V_HIS_PATIENT data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_PATIENT>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, item.GetValue(data));
                    }

                    this.AGE = AgeUtil.CalculateFullAge(this.DOB);
                    this.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.DOB);
                    string temp = this.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        this.DOB_YEAR = temp.Substring(0, 4);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class CareViewPrintADO
    {
        public CareViewPrintADO() { }

        public long? AWARENESS_ID { get; set; }
        public string CARE_CODE { get; set; }
        public string DEJECTA { get; set; }
        public long? DHST_ID { get; set; }
        public string EDUCATION { get; set; }
        public string EXECUTE_LOGINNAME { get; set; }
        public long? EXECUTE_TIME { get; set; }
        public string EXECUTE_USERNAME { get; set; }
        public string GROUP_CODE { get; set; }
        public short? HAS_ADD_MEDICINE { get; set; }
        public short? HAS_MEDICINE { get; set; }
        public short? HAS_TEST { get; set; }
        public long ID { get; set; }
        public short? IS_ACTIVE { get; set; }
        public short? IS_DELETE { get; set; }
        public string MODIFIER { get; set; }
        public long? MODIFY_TIME { get; set; }
        public string MUCOCUTANEOUS { get; set; }
        public string NUTRITION { get; set; }
        public string SANITARY { get; set; }
        public long TREATMENT_ID { get; set; }
        public string TUTORIAL { get; set; }
        public string URINE { get; set; }

        public string AWARENESS_NAME { get; set; }

        public decimal? BLOOD_PRESSURE_MAX { get; set; }
        public decimal? BLOOD_PRESSURE_MIN { get; set; }
        public decimal? BMI { get; set; }
        public decimal? BODY_SURFACE_AREA { get; set; }
        public decimal? BREATH_RATE { get; set; }
        public string DHST_CODE { get; set; }
        public decimal? HEIGHT { get; set; }
        public decimal? PULSE { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public decimal? WEIGHT { get; set; }

        public string EXECUTE_TIME_DISPLAY { get; set; }

        public string CARE_TITLE1 { get; set; }
        public string CARE_TITLE2 { get; set; }
        public string CARE_1 { get; set; }
        public string CARE_2 { get; set; }
        public string CARE_3 { get; set; }
        public string CARE_4 { get; set; }
        public string CARE_5 { get; set; }
        public string CARE_6 { get; set; }
        public string CARE_7 { get; set; }
        public string CARE_8 { get; set; }
        public string CARE_9 { get; set; }
        public string CARE_10 { get; set; }
        public string CARE_11 { get; set; }
        public string CARE_12 { get; set; }

    }

    public class CareDetailViewPrintADO
    {
        public long CARE_TYPE_ID { get; set; }
        public long CARE_ID { get; set; }
        public string CARE_TITLE { get; set; }
        public string CARE_DETAIL { get; set; }
        public string CARE_DETAIL_1 { get; set; }
        public string CARE_DETAIL_2 { get; set; }
        public string CARE_DETAIL_3 { get; set; }
        public string CARE_DETAIL_4 { get; set; }
        public string CARE_DETAIL_5 { get; set; }
        public string CARE_DETAIL_6 { get; set; }
    }

    public class Mps000069ADO
    {
        public string DEPARTMENT_NAME { get; set; }
        public string ROOM_NAME { get; set; }
        public string BED_NAME { get; set; }
        public long? ICD_ID { get; set; }
        public string ICD_MAIN_TEXT { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_TEXT { get; set; }
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
    }

    public class CreatorADO
    {
        public long CARE_ID { get; set; }
        public string CREATOR_1 { get; set; }
        public string CREATOR_2 { get; set; }
        public string CREATOR_3 { get; set; }
        public string CREATOR_4 { get; set; }
        public string CREATOR_5 { get; set; }
        public string CREATOR_6 { get; set; }
        public string CREATOR_7 { get; set; }
        public string CREATOR_8 { get; set; }
        public string CREATOR_9 { get; set; }
        public string CREATOR_10 { get; set; }
        public string CREATOR_11 { get; set; }
        public string CREATOR_12 { get; set; }

        public string USER_NAME_1 { get; set; }
        public string USER_NAME_2 { get; set; }
        public string USER_NAME_3 { get; set; }
        public string USER_NAME_4 { get; set; }
        public string USER_NAME_5 { get; set; }
        public string USER_NAME_6 { get; set; }
        public string USER_NAME_7 { get; set; }
        public string USER_NAME_8 { get; set; }
        public string USER_NAME_9 { get; set; }
        public string USER_NAME_10 { get; set; }
        public string USER_NAME_11 { get; set; }
        public string USER_NAME_12 { get; set; }
    }

    public class CareDescription
    {
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
