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

namespace MPS.Processor.Mps000192.PDO
{
    public partial class Mps000192PDO : RDOBase
    {
        public V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter { get; set; }
        public HIS_DHST hisDhst { get; set; }
        public HIS_SERVICE_REQ vHisPrescription5 { get; set; }
        public List<ExpMestMedicineSDO> expMestMedicines { get; set; }
        public HIS_SERVICE_REQ hisServiceReq_Exam { get; set; }
        public Mps000192ADO Mps000192ADO { get; set; }
        public HIS_TREATMENT HisTreatment { get; set; }
        public long? KeyUseForm;
    }

    public class Mps000192ADO : V_HIS_PATIENT
    {
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string TITLE { get; set; }
        public string REQUEST_PHONE { get; set; }
        public string EXECUTE_PHONE { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }

    public static class AgeUtil
    {
        public static string CalculateFullAge(long ageNumber)
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

    public class ExpMestMedicineSDO : V_HIS_EXP_MEST_MEDICINE
    {
        public short? IS_ADDICTIVE { get; set; }
        public short? IS_NEUROLOGICAL { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public int Type { get; set; }//1: thuoc // 2: vat tu, 3: thuoc trong kho, 4: thuoc ngoai kho, 5: tu tuc
        public decimal? PRES_AMOUNT { get; set; }
        public decimal? USING_COUNT_NUMBER { get; set; }
    }
}
