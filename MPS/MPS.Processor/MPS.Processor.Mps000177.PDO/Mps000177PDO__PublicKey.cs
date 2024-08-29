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
using Inventec.Common.DateTime;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000177.PDO
{
    public partial class Mps000177PDO : RDOBase
    {
        public List<PatientADO> currentPatient { get; set; }
        public Dictionary<long, V_HIS_TREATMENT_BED_ROOM> bedRoomName { get; set; }
        public string departmentName { get; set; }
        public List<Mps000177DAY> Mps000177DAY { get; set; }
        public List<Mps000177MediMate> Mps000177MediMate { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> VExpMestMedicine { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> VExpMestMaterial { get; set; }
        public List<V_HIS_EXP_MEST_BLOOD> VExpMestBlood { get; set; }
        public long DaySize { get; set; }
    }

    public class Mps000177DAY
    {
        public string Day1 { get; set; }
        public string Day2 { get; set; }
        public string Day3 { get; set; }
        public string Day4 { get; set; }
        public string Day5 { get; set; }
        public string Day6 { get; set; }
        public string Day7 { get; set; }
        public string Day8 { get; set; }
        public string Day9 { get; set; }
        public string Day10 { get; set; }
        public string Day11 { get; set; }
        public string Day12 { get; set; }
        public string Day13 { get; set; }
        public string Day14 { get; set; }
        public string Day15 { get; set; }
        public string Day16 { get; set; }
        public string Day17 { get; set; }
        public string Day18 { get; set; }
        public string Day19 { get; set; }
        public string Day20 { get; set; }
        public string Day21 { get; set; }
        public string Day22 { get; set; }
        public string Day23 { get; set; }
        public string Day24 { get; set; }
        public long treatment_id { get; set; }
        //public List<Mps000177MediMate> Mps000177MediMateADOs { get; set; }
    }

    public class Mps000177MediMate : V_HIS_EXP_MEST_MEDICINE
    {
        public string Day1 { get; set; }
        public string Day2 { get; set; }
        public string Day3 { get; set; }
        public string Day4 { get; set; }
        public string Day5 { get; set; }
        public string Day6 { get; set; }
        public string Day7 { get; set; }
        public string Day8 { get; set; }
        public string Day9 { get; set; }
        public string Day10 { get; set; }
        public string Day11 { get; set; }
        public string Day12 { get; set; }
        public string Day13 { get; set; }
        public string Day14 { get; set; }
        public string Day15 { get; set; }
        public string Day16 { get; set; }
        public string Day17 { get; set; }
        public string Day18 { get; set; }
        public string Day19 { get; set; }
        public string Day20 { get; set; }
        public string Day21 { get; set; }
        public string Day22 { get; set; }
        public string Day23 { get; set; }
        public string Day24 { get; set; }
        public long treatment_id { get; set; }
        public int type { get; set; }
    }
    public class EMMedicine : V_HIS_EXP_MEST_MEDICINE
    {
        public long treatment_id { get; set; }
        public long Page { get; set; }
        public Dictionary<long,decimal?> DicAmount { get; set; }
    }
    public class EMMaterial : V_HIS_EXP_MEST_MATERIAL
    {
        public long treatment_id { get; set; }
        public long Page { get; set; }
        public Dictionary<long, decimal?> DicAmount { get; set; }
    }
    public class EMBlood : V_HIS_EXP_MEST_BLOOD
    {
        public long treatment_id { get; set; }
        public long Page { get; set; }
        public Dictionary<long, decimal?> DicAmount { get; set; }
    }
    public class PatientADO : MOS.EFMODEL.DataModels.V_HIS_PATIENT
    {
        public long? CLINICAL_IN_TIME { get; set; }
        public string DATA_STORE_CODE { get; set; }
        public long? DATA_STORE_ID { get; set; }
        public string DATA_STORE_NAME { get; set; }
        public string END_DEPARTMENT_CODE { get; set; }
        public long? END_DEPARTMENT_ID { get; set; }
        public string END_DEPARTMENT_NAME { get; set; }
        public string END_LOGINNAME { get; set; }
        public long? END_ORDER { get; set; }
        public short? END_ORDER_REQUEST { get; set; }
        public string END_ROOM_CODE { get; set; }
        public long? END_ROOM_ID { get; set; }
        public string END_ROOM_NAME { get; set; }
        public string END_USERNAME { get; set; }
        public long? FEE_LOCK_DEPARTMENT_ID { get; set; }
        public long? FEE_LOCK_ORDER { get; set; }
        public long? FEE_LOCK_ROOM_ID { get; set; }
        public long? FEE_LOCK_TIME { get; set; }
        public string ICD_CODE { get; set; }
        public long? ICD_ID { get; set; }
        public string ICD_MAIN_TEXT { get; set; }
        public string ICD_NAME { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_TEXT { get; set; }
        public string IN_CODE { get; set; }
        public long? IN_DEPARTMENT_ID { get; set; }
        public long? IN_ORDER { get; set; }
        public short? IN_ORDER_REQUEST { get; set; }
        public long? IN_ROOM_ID { get; set; }
        public long IN_TIME { get; set; }
        public long? LOCK_TIME { get; set; }
        public long? OUT_TIME { get; set; }
        public long? OWE_MODIFY_TIME { get; set; }
        public string OWE_TYPE_CODE { get; set; }
        public long? OWE_TYPE_ID { get; set; }
        public string OWE_TYPE_NAME { get; set; }
        public long? PROGRAM_ID { get; set; }
        public string STORE_CODE { get; set; }
        public long? STORE_TIME { get; set; }
        public string TREATMENT_CODE { get; set; }
        public long? TREATMENT_END_NO_ID { get; set; }
        public string TREATMENT_END_TYPE_CODE { get; set; }
        public long? TREATMENT_END_TYPE_ID { get; set; }
        public string TREATMENT_END_TYPE_NAME { get; set; }
        public string TREATMENT_RESULT_CODE { get; set; }
        public long? TREATMENT_RESULT_ID { get; set; }
        public string TREATMENT_RESULT_NAME { get; set; }

        public string ROOM_NAME { get; set; }
        public string BED_NAME { get; set; }
        public long Page { get; set; }
        public Dictionary<long,long> DicMediDay { get; set; }
        public Dictionary<long, long> DicMateDay { get; set; }
        public Dictionary<long, long> DicBloodDay { get; set; }

        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_DATE_STR { get; set; }
        public string DOB_YEAR { get; set; }
        public string GENDER_MALE { get; set; }
        public string GENDER_FEMALE { get; set; }
        public long treatment_id { get; set; }
        public string Day1 { get; set; }
        public string Day2 { get; set; }
        public string Day3 { get; set; }
        public string Day4 { get; set; }
        public string Day5 { get; set; }
        public string Day6 { get; set; }
        public string Day7 { get; set; }
        public string Day8 { get; set; }
        public string Day9 { get; set; }
        public string Day10 { get; set; }
        public string Day11 { get; set; }
        public string Day12 { get; set; }
        public string Day13 { get; set; }
        public string Day14 { get; set; }
        public string Day15 { get; set; }
        public string Day16 { get; set; }
        public string Day17 { get; set; }
        public string Day18 { get; set; }
        public string Day19 { get; set; }
        public string Day20 { get; set; }
        public string Day21 { get; set; }
        public string Day22 { get; set; }
        public string Day23 { get; set; }
        public string Day24 { get; set; }

        public PatientADO() { }

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
                    this.CMND_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.CMND_DATE ?? 0);

                    if (this.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        this.GENDER_MALE = "";
                        this.GENDER_FEMALE = "X";
                    }
                    else if (this.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        this.GENDER_MALE = "X";
                        this.GENDER_FEMALE = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
