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
using MPS.Processor.Mps000055.PDO;

namespace MPS.Processor.Mps000055.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000055PDO : RDOBase
    {
        public PatientADO Patient { get; set; }
        public TreatmentADO currentHisTreatment { get; set; }
        public V_HIS_MEDI_REACT serviceReq { get; set; }
        public List<ExeMediReactADO> expMestMediReact { get; set; }
        public string bedRoomName;
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

    public class TreatmentADO : MOS.EFMODEL.DataModels.V_HIS_TREATMENT
    {
        public string LOCK_TIME_STR { get; set; }
    }
    public class ExeMediReactADO : MOS.EFMODEL.DataModels.V_HIS_MEDI_REACT
    {
        public string EXECUTE_TIME_STR { get; set; }
        public string CHECK_TIME_STR { get; set; }
        public ExeMediReactADO(V_HIS_MEDI_REACT data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ExeMediReactADO>(this, data);
                this.EXECUTE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                this.CHECK_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CHECK_TIME ?? 0);
            }
        }
    }
}
