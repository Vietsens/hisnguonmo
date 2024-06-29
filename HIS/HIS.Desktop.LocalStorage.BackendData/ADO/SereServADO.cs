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

namespace HIS.Desktop.LocalStorage.BackendData.ADO
{
    public class SereServADO : MOS.EFMODEL.DataModels.V_HIS_SERE_SERV
    {
        public long? AssignNumOrder { get; set; }
        public long? SERVICE_CONDITION_ID { get; set; }
        public string OTHER_PAY_SOURCE_NAME { get; set; }
        public string OTHER_PAY_SOURCE_CODE { get; set; }
        public string SERVICE_CONDITION_NAME { get; set; }
        public short? IS_CONDITIONED { get; set; }
        public string NOTICE { get; set; }
        public long? SERVICE_NUM_ORDER { get; set; }
        public long SERVICE_TYPE_ID { get; set; }
        public long? MIN_DURATION { get; set; }
        public string InstructionNote { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string SERVICE_CODE_HIDDEN { get; set; }
        public string SERVICE_NAME_HIDDEN { get; set; }
        public long? GENDER_ID { get; set; }
        public long? AGE_FROM { get; set; }
        public long? AGE_TO { get; set; }
        public long? BILL_PATIENT_TYPE_ID { get; set; }
        public short? IS_NOT_CHANGE_BILL_PATY { get; set; }

        public List<long> SERVICE_GROUP_ID_SELECTEDs { get; set; }
        public string PTTT_GROUP_NAME { get; set; }
        public short? IS_MULTI_REQUEST { get; set; }
        public short? IsAllowExpend { get; set; }
        public bool IsChecked { get; set; }
        public decimal? HEIN_LIMIT_PRICE_OLD { get; set; }
        public decimal? HEIN_LIMIT_RATIO_OLD { get; set; }
        public long? HEIN_LIMIT_PRICE_IN_TIME { get; set; }
        public long? HEIN_LIMIT_PRICE_INTR_TIME { get; set; }
        public double IdRow { get; set; }
        public bool? IsExpend { get; set; }
        public decimal? PRICE_KSK { get; set; }
        public short? IsAutoExpend { get; set; }
        public bool IsServiceKsk { get; set; }
        public bool? IsNoDifference { get; set; }
        public bool? IsKHBHYT { get; set; }
        public bool IsOutParentFee { get; set; }
        public bool? IsOutKtcFee { get; set; }
        public long? ShareCount { get; set; }
        public decimal? AssignSurgPriceEdit { get; set; }
        public decimal? AssignPackagePriceEdit { get; set; }
        public short? IS_ENABLE_ASSIGN_PRICE { get; set; }
        public long? PackagePriceId { get; set; }
        public bool IsNotChangePrimaryPaty { get; set; }
        public short? DO_NOT_USE_BHYT { get; set; }

        public long? BedStartTime { get; set; }
        public long? BedFinishTime { get; set; }
        public long? BedId { get; set; }

        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeAmount { get; set; }
        public string ErrorMessageAmount { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypePatientTypeId { get; set; }
        public string ErrorMessagePatientTypeId { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeIsAssignDay { get; set; }
        public string ErrorMessageIsAssignDay { get; set; }
        public bool IsNotLoadDefaultPatientType { get; set; }
        public bool IsContainAppliedPatientType { get; set; }
        public long? DEFAULT_PATIENT_TYPE_ID { get; set; }
        public bool IsNotUseBhyt { get; set; }
        public long OldPatientType { get; set; }
        public SereServEkipADO SereServEkipADO { get; set; }
        public long? MAX_AMOUNT { get; set; }
        public long TEST_SAMPLE_TYPE_ID { get; set; }
        public string TEST_SAMPLE_TYPE_CODE { get; set; }
        public string TEST_SAMPLE_TYPE_NAME { get; set; }
        public string TEST_SAMPLE_TYPE_CODE_DEFAULT { get; set; }
        public SereServADO()
        {

        }
        public long? AppointmentSereServId { get; set; }
        public SereServADO(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV expMestMedicine)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, expMestMedicine); hieu nang khong tot => gan truc tiep

                this.GROUP_CODE = expMestMedicine.GROUP_CODE;
                this.HEIN_LIMIT_PRICE = expMestMedicine.HEIN_LIMIT_PRICE;
                this.HEIN_LIMIT_RATIO = expMestMedicine.HEIN_LIMIT_RATIO;
                this.HEIN_SERVICE_TYPE_CODE = expMestMedicine.HEIN_SERVICE_TYPE_CODE;
                this.HEIN_SERVICE_TYPE_NAME = expMestMedicine.HEIN_SERVICE_TYPE_NAME;
                this.HEIN_SERVICE_TYPE_NUM_ORDER = expMestMedicine.HEIN_SERVICE_TYPE_NUM_ORDER;
                this.ID = expMestMedicine.ID;
                this.IS_ACTIVE = expMestMedicine.IS_ACTIVE;
                this.IS_DELETE = expMestMedicine.IS_DELETE;
                this.IS_OUT_PARENT_FEE = expMestMedicine.IS_OUT_PARENT_FEE;
                this.MODIFIER = expMestMedicine.MODIFIER;
                this.MODIFY_TIME = expMestMedicine.MODIFY_TIME;
                this.PACKAGE_ID = expMestMedicine.PACKAGE_ID;
                //this.PackagePriceId = expMestMedicine.PACKAGE_ID;
                this.PARENT_ID = expMestMedicine.PARENT_ID;
                this.SERVICE_ID = expMestMedicine.ID;
                this.TDL_SERVICE_TYPE_ID = expMestMedicine.TDL_SERVICE_TYPE_ID;
                this.SERVICE_TYPE_ID = expMestMedicine.TDL_SERVICE_TYPE_ID;
                this.SERVICE_TYPE_CODE = expMestMedicine.SERVICE_TYPE_CODE;
                this.SERVICE_TYPE_NAME = expMestMedicine.SERVICE_TYPE_NAME;
                this.TDL_SERVICE_UNIT_ID = expMestMedicine.TDL_SERVICE_UNIT_ID;
                this.SERVICE_UNIT_CODE = expMestMedicine.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_NAME = expMestMedicine.SERVICE_UNIT_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(MOS.EFMODEL.DataModels.V_HIS_SERVICE service, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType, bool isAssignDay, long serviceTypeId__Surg, long serviceTypeId__Misu)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, service); hieu nang khong tot => gan truc tiep
                this.TEST_SAMPLE_TYPE_CODE_DEFAULT = service.SAMPLE_TYPE_CODE;
                this.MAX_AMOUNT = service.MAX_AMOUNT;
                this.GROUP_CODE = service.GROUP_CODE;
                this.HEIN_LIMIT_PRICE = service.HEIN_LIMIT_PRICE;
                this.HEIN_LIMIT_RATIO = service.HEIN_LIMIT_RATIO;
                this.HEIN_LIMIT_PRICE_OLD = service.HEIN_LIMIT_PRICE_OLD;
                this.HEIN_LIMIT_RATIO_OLD = service.HEIN_LIMIT_RATIO_OLD;
                this.HEIN_LIMIT_PRICE_IN_TIME = service.HEIN_LIMIT_PRICE_IN_TIME;
                this.HEIN_LIMIT_PRICE_INTR_TIME = service.HEIN_LIMIT_PRICE_INTR_TIME;
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
                //this.PackagePriceId = service.PACKAGE_ID;
                this.PARENT_ID = service.PARENT_ID;
                this.SERVICE_ID = service.ID;
                this.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                this.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                this.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                this.IS_MULTI_REQUEST = service.IS_MULTI_REQUEST;
                this.IsAllowExpend = service.IS_ALLOW_EXPEND;
                this.IS_ENABLE_ASSIGN_PRICE = service.IS_ENABLE_ASSIGN_PRICE;
                this.AGE_FROM = service.AGE_FROM;
                this.AGE_TO = service.AGE_TO;
                this.NOTICE = service.NOTICE;

                this.MIN_DURATION = service.MIN_DURATION;
                this.GENDER_ID = service.GENDER_ID;
                this.AMOUNT = 1;
                this.IsExpend = false;
                this.IsAutoExpend = service.IS_AUTO_EXPEND;
                this.IsServiceKsk = false;
                this.IsKHBHYT = false;

                this.SERVICE_NUM_ORDER = service.NUM_ORDER;
                if (patientType != null)
                {
                    this.PATIENT_TYPE_ID = patientType.ID;
                    this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                    this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                }
                this.TDL_SERVICE_CODE = service.SERVICE_CODE;
                this.TDL_SERVICE_NAME = service.SERVICE_NAME;
                this.SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                this.TDL_SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                this.TDL_SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
                this.TDL_HEIN_SERVICE_TYPE_ID = service.HEIN_SERVICE_TYPE_ID;
                this.TDL_HEIN_SERVICE_BHYT_CODE = service.HEIN_SERVICE_BHYT_CODE;
                this.TDL_HEIN_SERVICE_BHYT_NAME = service.HEIN_SERVICE_BHYT_NAME;
                this.TDL_HEIN_ORDER = service.HEIN_ORDER;
                this.BILL_PATIENT_TYPE_ID = service.BILL_PATIENT_TYPE_ID;
                this.IS_NOT_CHANGE_BILL_PATY = service.IS_NOT_CHANGE_BILL_PATY;

                this.PTTT_GROUP_NAME = service.PTTT_GROUP_NAME;
                
                this.IsOutKtcFee = ((service.IS_OUT_PARENT_FEE ?? -1) == 1);//TODO
                this.IsNoDifference = false;
                this.SERVICE_NAME_HIDDEN = convertToUnSign3(this.TDL_SERVICE_NAME) + this.TDL_SERVICE_NAME;
                this.SERVICE_CODE_HIDDEN = convertToUnSign3(this.TDL_SERVICE_CODE) + this.TDL_SERVICE_CODE;
                this.DEFAULT_PATIENT_TYPE_ID = service.DEFAULT_PATIENT_TYPE_ID;
                this.DO_NOT_USE_BHYT = service.DO_NOT_USE_BHYT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string convertToUnSign3(string s)
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

        public SereServADO(SereServADO service, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, service);
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
    }
}
