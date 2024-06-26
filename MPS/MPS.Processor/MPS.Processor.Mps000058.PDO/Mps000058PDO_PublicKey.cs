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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MOS.SDO;
using MPS.Processor.Mps000058.PDO;

namespace MPS.Processor.Mps000058.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000058PDO : RDOBase
    {
        public PatientADO Patient { get; set; }
        public TreatmentADO currentTreatment { get; set; }
        public V_HIS_INFUSION_SUM sumInfusion { get; set; }
        public List<ExeInfusionDetailSDO> infusionDetaiAdos { get; set; }
    }
    public class PatientADO : MOS.EFMODEL.DataModels.V_HIS_PATIENT
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
    public class ExeInfusionDetailSDO : MOS.EFMODEL.DataModels.V_HIS_INFUSION
    {
        public string CREATE_TIME_STR { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string START_TIME_STR { get; set; }
        public string FINISH_TIME_STR { get; set; }
        public decimal? SPEED { get; set; }
        public string EXECUTE_LOGINNAME { get; set; }
        public string EXECUTE_USERNAME { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string EXECUTE_DEPARTMENT_CODE { get; set; }
        public long? EXECUTE_DEPARTMENT_ID { get; set; }
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public long? EXECUTE_ROOM_ID { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
    }
    public class TreatmentADO : MOS.EFMODEL.DataModels.V_HIS_TREATMENT
    {
        public string LOCK_TIME_STR { get; set; }
    }
}
