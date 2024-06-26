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
using MPS.Processor.Mps000075.PDO;

namespace MPS.Processor.Mps000075.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000075PDO : RDOBase
    {
        public PatyAlterBhytADO PatyAlterBhyt { get; set; }

        public string DepartmentName { get; set; }
        public PatientADO Patient { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        public V_HIS_TREATMENT currentTreatment { get; set; }
        public string ratio;
        public List<V_HIS_DEPARTMENT_TRAN> departmentTrans { get; set; }
        public V_HIS_TRAN_PATI hisTranPati { get; set; }
        public List<V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        public string currentDateSeparateFullTime = "";

        public List<V_HIS_SERE_SERV> hisSereServ_Bordereaus;

        public List<ServiceGroupPrintADO> highTechServiceReports { get; set; }
        public List<V_HIS_SERE_SERV> highTechDepartments { get; set; }
        public List<SereServADO> hightTechServices { get; set; }
        public List<SereServADO> serviceInHightTechs { get; set; }

        public List<ServiceGroupPrintADO> ListGroupServiceTypeADO { get; set; }
        public List<SereServADO> departmentADOs { get; set; }
        public List<SereServADO> sereServADOs { get; set; }

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
    public class PatyAlterBhytADO : V_HIS_PATY_ALTER_BHYT
    {
        public string PATIENT_TYPE_NAME { get; set; }
        public string HEIN_CARD_NUMBER_SEPARATE { get; set; }
        public string IS_HEIN { get; set; }
        public string IS_VIENPHI { get; set; }
        public string STR_HEIN_CARD_FROM_TIME { get; set; }
        public string STR_HEIN_CARD_TO_TIME { get; set; }
        public string RATIO { get; set; }
        public string HEIN_CARD_NUMBER_1 { get; set; }
        public string HEIN_CARD_NUMBER_2 { get; set; }
        public string HEIN_CARD_NUMBER_3 { get; set; }
        public string HEIN_CARD_NUMBER_4 { get; set; }
        public string HEIN_CARD_NUMBER_5 { get; set; }
        public string HEIN_CARD_NUMBER_6 { get; set; }
        public long TIME_IN_TREATMENT { get; set; }
    }
    public class ServiceGroupPrintADO : MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE
    {
        public long SERVICE_TYPE_ID { get; set; }
        public long HEIN_SERVICE_TYPE_ID { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public decimal? VIR_HEIN_PRICE { get; set; }
        public decimal? VIR_PATIENT_PRICE { get; set; }
        public decimal? VIR_PRICE { get; set; } //thay doi
        public decimal? VIR_PRICE_NO_EXPEND { get; set; }
        public decimal? VIR_TOTAL_HEIN_PRICE { get; set; } //thay doi
        public decimal? VIR_TOTAL_PATIENT_PRICE { get; set; } //thay doi
        public decimal? VIR_TOTAL_PRICE { get; set; } //thay doi
        public decimal? VIR_TOTAL_PRICE_NO_EXPEND { get; set; }
        public decimal? VIR_TOTAL_PRICE_OTHER { get; set; } //them moi

        public decimal? VIR_TOTAL_HEIN_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PATIENT_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PRICE_OTHER_SUM { get; set; }

        public int NUM_ORDER { get; set; }
    }
    public class SereServADO : V_HIS_SERE_SERV
    {
        public decimal? VIR_TOTAL_HEIN_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PATIENT_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PRICE_NO_EXPEND_SUM { get; set; }
        public decimal? PRICE_BHYT { get; set; }
        public long? SERVICE_PACKAGE_ID { get; set; }
        public string DEPARTMENT__SERVICE_GROUP__ID { get; set; }

        public string patientIdQr { get; set; }
        public byte[] bPatientQr { get; set; }

        public string patientNameQr { get; set; }
        public byte[] bPatientNameQr { get; set; }

        public string studyDescriptionQr { get; set; }
        public byte[] bStudyDescriptionQr { get; set; }

        public SereServADO()
        {

        }
        public SereServADO(V_HIS_SERE_SERV data)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                SetSumServiceChildInServicePackage(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetSumServiceChildInServicePackage(V_HIS_SERE_SERV data)
        {
            try
            {
                VIR_TOTAL_PRICE_NO_EXPEND_SUM = null;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
