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

namespace MPS.Processor.Mps000158.PDO
{
    public partial class Mps000158PDO : RDOBase
    {
        public V_HIS_PATIENT patient { get; set; }
        public HIS_PATIENT_TYPE_ALTER patyAlter { get; set; }
        public List<V_HIS_DEPARTMENT_TRAN> departmentTrans { get; set; }
        public List<V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        public List<V_HIS_SERE_SERV> sereServs { get; set; }
        public V_HIS_TREATMENT treatment { get; set; }
        public string ratio;
        public long today { get; set; }
        public string departmentName;

        public PatientADO patientADO { get; set; }
        public PatyAlterBhytADO patyAlterBHYTADO { get; set; }
        public List<SereServADO> sereServADOs { get; set; }
        public List<SereServADO> heinServiceTypes { get; set; }
        public HeinServiceTypeCFG heinServiceTypeCfg { get; set; }
        public string currentDateSeparateFullTime { get; set; }
    }

    public class SereServADO : V_HIS_SERE_SERV
    {
        public int ROW_POS { get; set; }

        public long? DEPARTMENT__HEIN_SERVICE_TYPE_ID { get; set; }
        public long? SERE_SERV__HEIN_SERVICE_TYPE_ID { get; set; }
        public long? DEPARTMENT__SERE_SERV { get; set; }

        public decimal PRICE_BHYT { get; set; }
        public decimal TOTAL_PRICE_BHYT { get; set; }

        public decimal? TOTAL_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_HEIN_PRICE_DEPARTMENT { get; set; }

        public decimal? TOTAL_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_HEIN_PRICE_ONE_AMOUNT { get; set; } //bhyt chi tra cho so luong 1

        public decimal? TOTAL_PRICE_KTC_FEE_GROUP { get; set; }
        public decimal? TOTAL_HEIN_PRICE_FEE_GROUP { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_FEE_GROUP { get; set; }

        public decimal? TOTAL_PRICE_EXECUTE_GROUP { get; set; }
        public decimal? TOTAL_HEIN_PRICE_EXECUTE_GROUP { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_EXECUTE_GROUP { get; set; }

        public decimal? PRICE_POLICY { get; set; }

        //Tong nhom dich vu ky thuat cao
        public decimal TOTAL_PRICE_HIGHTECH { get; set; }
        public decimal TOTAL_HEIN_PRICE_HIGHTECH { get; set; }
        public decimal TOTAL_PATIENT_PRICE_HIGHTECH { get; set; }
        public decimal TOTAL_PRICE_HIGHTECH_HEIN_SERVICE_TYPE { get; set; }
        public decimal TOTAL_HEIN_PRICE_HIGHTECH_HEIN_SERVICE_TYPE { get; set; }
        public decimal TOTAL_PATIENT_PRICE_HIGHTECH_HEIN_SERVICE_TYPE { get; set; }
        public decimal? PRICE_CO_PAYMENT { get; set; } //dong chi tra

        public string KTC_FEE_GROUP_NAME { get; set; }
        public string EXECUTE_GROUP_NAME { get; set; }

        public SereServADO() { }

        //BHYT
        public SereServADO(V_HIS_SERE_SERV data, HIS_PATIENT_TYPE_ALTER patyAlterBHYT)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                this.PRICE_BHYT = PriceBHYTProcess(data, patyAlterBHYT);
                this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;
                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Basic
        public SereServADO(V_HIS_SERE_SERV data)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                if (this.HEIN_LIMIT_PRICE != null && this.HEIN_LIMIT_PRICE > 0)
                    this.PRICE_BHYT = (this.HEIN_LIMIT_PRICE ?? 0);
                else
                    this.PRICE_BHYT = this.VIR_PRICE_NO_ADD_PRICE ?? 0;
                this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;

                //Dong chi tra
                if (this.HEIN_LIMIT_PRICE.HasValue && this.HEIN_LIMIT_PRICE < this.ORIGINAL_PRICE)
                {
                    this.PRICE_CO_PAYMENT = this.ORIGINAL_PRICE - this.HEIN_LIMIT_PRICE.Value;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(V_HIS_SERE_SERV data, List<V_HIS_SERVICE_PATY_PRPO> servicePatyPrpos)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                if (data.PRICE_POLICY_ID.HasValue)
                {
                    var servicePatyPrpo = servicePatyPrpos.Where(o => o.SERVICE_ID == data.SERVICE_ID && o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID && o.PRICE_POLICY_ID == data.PRICE_POLICY_ID).ToList();
                    if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                    {
                        this.PRICE_POLICY = servicePatyPrpo.FirstOrDefault().PRICE;
                    }
                }
                else
                {
                    this.PRICE_POLICY = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(V_HIS_SERE_SERV data, List<V_HIS_SERVICE_PATY_PRPO> servicePatyPrpos, HIS_PATIENT_TYPE_ALTER patyAlterBHYT)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                this.PRICE_BHYT = PriceBHYTProcess(data, patyAlterBHYT);
                this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;

                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                if (data.PRICE_POLICY_ID.HasValue)
                {
                    var servicePatyPrpo = servicePatyPrpos.Where(o => o.SERVICE_ID == data.SERVICE_ID && o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID && o.PRICE_POLICY_ID == data.PRICE_POLICY_ID).ToList();
                    if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                    {
                        this.PRICE_POLICY = servicePatyPrpo.FirstOrDefault().PRICE;
                    }
                }
                else
                {
                    this.PRICE_POLICY = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public decimal PriceBHYTProcess(V_HIS_SERE_SERV sereServ, HIS_PATIENT_TYPE_ALTER patyAlterBHYT)
        {
            decimal priceBHYT = -999;
            try
            {
                if (patyAlterBHYT != null)
                {
                    if (sereServ.PATIENT_TYPE_ID != patyAlterBHYT.PATIENT_TYPE_ID)
                    {
                        priceBHYT = 0;
                    }
                    else
                    {
                        if (sereServ.HEIN_LIMIT_PRICE != null && sereServ.HEIN_LIMIT_PRICE > 0)
                            priceBHYT = (sereServ.HEIN_LIMIT_PRICE ?? -999);
                        else
                            priceBHYT = sereServ.VIR_PRICE_NO_ADD_PRICE ?? -999;
                    }
                }
                else
                {
                    priceBHYT = sereServ.VIR_PRICE_NO_ADD_PRICE ?? -999;
                }
            }
            catch (Exception ex)
            {
                priceBHYT = -999;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return priceBHYT;
        }
    }

    public class PatyAlterBhytADO : HIS_PATIENT_TYPE_ALTER
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

    public class PatientADO : V_HIS_PATIENT
    {
        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_DATE_STR { get; set; }
        public string DOB_YEAR { get; set; }
        public string GENDER_MALE { get; set; }
        public string GENDER_FEMALE { get; set; }

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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class HeinServiceTypeCFG
    {
        public long? HIGHTECH_ID { get; set; }
        public string HIGHTECH_CODE { get; set; }
        public long? MATERIAL_VTTT_ID { get; set; }
        public string MATERIAL_VTTT_CODE { get; set; }
        public string EXAM_CODE { get; set; }
        public string SURG_MISU_CODE { get; set; }
        public long? SURG_MISU_ID { get; set; }
    }

    public class SereServGroupPlusADO : V_HIS_SERE_SERV
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

    }
}
