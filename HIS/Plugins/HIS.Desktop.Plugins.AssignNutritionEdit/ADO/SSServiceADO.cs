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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutritionEdit.ADO
{
    public class SSServiceADO : MOS.EFMODEL.DataModels.V_HIS_SERVICE
    {
        public string NOTE { get; set; }
        public decimal AMOUNT {get;set;}
        public long PATIENT_TYPE_ID { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public long ROOM_ID { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public long TDL_EXECUTE_BRANCH_ID { get; set; }
        public decimal PRICE { get; set; }
        public List<long> RationTimeIds { get; set; }
        public string BUA_AN_NAME { get; set; }
        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public string SERVICE_CODE_HIDDEN { get; set; }
        public string SERVICE_NAME_HIDDEN { get; set; }
        public long? SERVICE_CONDITION_ID { get; set; }
        public string OTHER_PAY_SOURCE_NAME { get; set; }
        public string OTHER_PAY_SOURCE_CODE { get; set; }
        public string SERVICE_CONDITION_NAME { get; set; }
        public short? IS_CONDITIONED { get; set; }
        public long? OTHER_PAY_SOURCE_ID { get; set; }
        public decimal? OTHER_SOURCE_PRICE { get; set; }
        public long? PRIMARY_PATIENT_TYPE_ID { get; set; }
        public bool IsNotChangePrimaryPaty { get; set; }

        public long? SERE_SERV_RATIO_ID { get; set; }
        public SSServiceADO()
        {

        }


        public SSServiceADO(MOS.EFMODEL.DataModels.V_HIS_SERVICE service, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SSServiceADO>(this, service);
                this.AMOUNT = 0;
                if (patientType != null)
                {
                    this.PATIENT_TYPE_ID = patientType.ID;
                    this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                    this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SSServiceADO(MOS.EFMODEL.DataModels.V_HIS_SERVICE service, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType, bool isAssignDay)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, service); hieu nang khong tot => gan truc tiep
                this.GROUP_CODE = service.GROUP_CODE;
                this.HEIN_LIMIT_PRICE = service.HEIN_LIMIT_PRICE;
                this.HEIN_LIMIT_RATIO = service.HEIN_LIMIT_RATIO;
                this.HEIN_SERVICE_TYPE_CODE = service.HEIN_SERVICE_TYPE_CODE;
                this.HEIN_SERVICE_TYPE_NAME = service.HEIN_SERVICE_TYPE_NAME;
                this.HEIN_SERVICE_TYPE_NUM_ORDER = service.HEIN_SERVICE_TYPE_NUM_ORDER;
                this.ID = service.ID;
                this.IS_ACTIVE = service.IS_ACTIVE;
                this.IS_DELETE = service.IS_DELETE;
                this.IS_OUT_PARENT_FEE = service.IS_OUT_PARENT_FEE;
                this.MODIFIER = service.MODIFIER;
                this.MODIFY_TIME = service.MODIFY_TIME;
                this.PACKAGE_ID = service.PACKAGE_ID;
                this.PARENT_ID = service.PARENT_ID;
                this.ID = service.ID;
                this.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                this.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                this.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                this.IS_MULTI_REQUEST = service.IS_MULTI_REQUEST;
                this.AGE_FROM = service.AGE_FROM;
                this.AGE_TO = service.AGE_TO;
                this.NOTICE = service.NOTICE;
                this.CAPACITY = service.CAPACITY;
                this.MIN_DURATION = service.MIN_DURATION;
                this.GENDER_ID = service.GENDER_ID;
                this.AMOUNT = 0;   
                this.NUM_ORDER = service.NUM_ORDER;
                if (patientType != null)
                {
                    this.PATIENT_TYPE_ID = patientType.ID;
                    this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                    this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                }
                this.SERVICE_CODE = service.SERVICE_CODE;
                this.SERVICE_NAME = service.SERVICE_NAME;
                this.SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;            
                this.SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
                this.HEIN_SERVICE_TYPE_ID = service.HEIN_SERVICE_TYPE_ID;
                this.HEIN_SERVICE_BHYT_CODE = service.HEIN_SERVICE_BHYT_CODE;
                this.HEIN_SERVICE_BHYT_NAME = service.HEIN_SERVICE_BHYT_NAME;
                this.HEIN_ORDER = service.HEIN_ORDER;
                this.BILL_PATIENT_TYPE_ID = service.BILL_PATIENT_TYPE_ID;
                if (!String.IsNullOrWhiteSpace(service.PTTT_GROUP_NAME))
                {
                    this.PTTT_GROUP_NAME = service.PTTT_GROUP_NAME;
                }
                else
                {
                    this.PTTT_GROUP_NAME = service.SERVICE_TYPE_NAME;
                }
                this.SERVICE_NAME_HIDDEN = convertToUnSign3(this.SERVICE_NAME) + this.SERVICE_NAME;
                this.SERVICE_CODE_HIDDEN = convertToUnSign3(this.SERVICE_CODE) + this.SERVICE_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string convertToUnSign3(string s)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(s))
                    return "";

                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = s.Normalize(NormalizationForm.FormD);
                return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            }
            catch
            {

            }
            return "";
        }

        public List<long> SERVICE_GROUP_ID_SELECTEDs { get; set; }
        public bool IsChecked { get; set; }
        public double IdRow { get; set; }

        public bool cot0 { get; set; }
        public bool cot1 { get; set; }
        public bool cot2 { get; set; }
        public bool cot3 { get; set; }
        public bool cot4 { get; set; }
        public bool cot5 { get; set; }
        public bool cot6 { get; set; }
        public bool cot7 { get; set; }
        public bool cot8 { get; set; }
        public bool cot9 { get; set; }
        public bool cot10 { get; set; }
        public bool cot11 { get; set; }
        public bool cot12 { get; set; }
        public bool cot13 { get; set; }
        public bool cot14 { get; set; }
        public bool cot15 { get; set; }
        public bool cot16 { get; set; }
        public bool cot17 { get; set; }
        public bool cot18 { get; set; }
        public bool cot19 { get; set; }
        public bool cot20 { get; set; }

        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeAmount { get; set; }
        public string ErrorMessageAmount { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypePatientTypeId { get; set; }
        public string ErrorMessagePatientTypeId { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeIsAssignDay { get; set; }
        public string ErrorMessageIsAssignDay { get; set; }
    }
}
