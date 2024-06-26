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

namespace MPS.Processor.Mps000102.PDO
{
    public partial class Mps000102PDO : RDOBase
    {
        public MOS.EFMODEL.DataModels.V_HIS_TRANSACTION deposit { get; set; }
        public V_HIS_TREATMENT_FEE currentHisTreatment { get; set; }
        public PatientADO patientADO { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER patyAlterBhytADO { get; set; }
        public V_HIS_SERVICE_REQ hisServiceReq { get; set; }
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

                    this.AGE = CalculateFullAge(this.DOB);
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

        internal static string CalculateFullAge(long ageNumber)
        {
            string tuoi;
            string cboAge;
            try
            {
                DateTime dtNgSinh = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                TimeSpan diff = DateTime.Now - dtNgSinh;
                long tongsogiay = diff.Ticks;
                if (tongsogiay < 0)
                {
                    tuoi = "";
                    cboAge = "Tuổi";
                    return "";
                }
                DateTime newDate = new DateTime(tongsogiay);

                int nam = newDate.Year - 1;
                int thang = newDate.Month - 1;
                int ngay = newDate.Day - 1;
                int gio = newDate.Hour;
                int phut = newDate.Minute;
                int giay = newDate.Second;

                if (nam > 0)
                {
                    tuoi = nam.ToString();
                    cboAge = "Tuổi";
                }
                else
                {
                    if (thang > 0)
                    {
                        tuoi = thang.ToString();
                        cboAge = "Tháng";
                    }
                    else
                    {
                        if (ngay > 0)
                        {
                            tuoi = ngay.ToString();
                            cboAge = "ngày";
                        }
                        else
                        {
                            tuoi = "";
                            cboAge = "Giờ";
                        }
                    }
                }
                return tuoi + " " + cboAge;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }

    public class SereServGroupPlusADO : V_HIS_SERE_SERV_12
    {
        public string KTC_FEE_GROUP_NAME { get; set; }
        public decimal TOTAL_PRICE_GROUP { get; set; }
        public string EXECUTE_GROUP_NAME { get; set; }
        public string HEIN_SERVICE_TYPE_NAME { get; set; }
        public decimal TOTAL_PRICE_SERVICE_GROUP { get; set; }
        public decimal TOTAL_PATIENT_PRICE_SERVICE_GROUP { get; set; }
        public decimal TOTAL_HEIN_PRICE_SERVICE_GROUP { get; set; }

        public decimal TOTAL_PRICE_DEPARTMENT_GROUP { get; set; }
        public decimal TOTAL_PATIENT_PRICE_DEPARTMENT_GROUP { get; set; }
        public decimal TOTAL_HEIN_PRICE_DEPARTMENT_GROUP { get; set; }

        public decimal VIR_TOTAL_PRICE_KTC { get; set; }
        public decimal VIR_TOTAL_HEIN_PRICE_SUM { get; set; }
        public decimal VIR_TOTAL_PATIENT_PRICE_SUM { get; set; }
        public decimal VIR_TOTAL_PATIENT_PRICE_SUM_END { get; set; }

        public decimal VIR_TOTAL_PRICE_KTC_HIGHTTECH_GROUP { get; set; }
        public decimal VIR_TOTAL_HEIN_PRICE_HIGHTTECH_GROUP { get; set; }
        public decimal VIR_TOTAL_PATIENT_PRICE_HIGHTTECH_GROUP { get; set; }

        public decimal ROW_POS { get; set; }
        public decimal PRICE_BHYT { get; set; }
        public decimal PRICE_POLICY { get; set; }

        public long DEPARTMENT__GROUP_SERE_SERV { get; set; }
        public long DEPARTMENT__GROUP_SERVICE_REPORT { get; set; }
        public long SERE_SERV__GROUP_SERVICE_REPORT { get; set; }

        public string SERVICE_UNIT_NAME { get; set; }

        public string EXECUTE_ROOM_NAME { get; set; }
    }
}
