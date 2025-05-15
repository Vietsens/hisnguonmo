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
using System.Threading.Tasks;

namespace HIS.Desktop.ApiConsumer
{
    public partial class HisRequestUriStore
    {
        public const string HIS_CACHE_MONITOR_CREATE = "api/HisCacheMonitor/Create";
        public const string HIS_CACHE_MONITOR_DELETE = "api/HisCacheMonitor/Delete";
        public const string HIS_CACHE_MONITOR_UPDATE = "api/HisCacheMonitor/Update";
        public const string HIS_CACHE_MONITOR_GET = "api/HisCacheMonitor/Get";

        public const string HIS_CONFIG_RESET = "/api/HisConfig/ResetAll";

        public const string HIS_RESET_LIS_CONFIG = "api/LisConfig/ResetAll";

        public const string HIS_HEIN_SERVICE_PRICE_GET = "/api/HisHeinServicePrice/Get";
        public const string HIS_MEDICINE_PATY_GET = "/api/HisMedicinePaty/Get";
        public const string HIS_MEDICINE_TYPE_TUT_GET = "/api/HisMedicineTypeTut/Get";
        public const string HIS_EXP_MEST_REASON_GET = "api/HisExpMestReason/Get";
        public const string HIS_OWE_TYPE_GET = "api/HisOweType/Get";
        public const string HIS_ANTICIPATE_GET = "/api/HisAnticipate/Get";
        public const string HIS_ANTICIPATE_GETVIEW = "api/HisAnticipate/GetView";
        public const string HIS_ANTICIPATE_TYPE_GETVIEW = "api/HisAnticipateType/GetView";
        public const string HIS_APPOINTMENT_GET = "/api/HisAppointment/Get";
        public const string HIS_EMERGENCY_WTIME_GET = "/api/HisEmergencyWtime/Get";

        public const string HIS_SERVICE_PACKAGE_GETVIEW = "/api/HisServicePackage/GetView";

        public const string HIS_BRANCH_GET = "/api/HisBranch/Get";

        public const string HIS_ACCOUNT_BOOK_GET = "api/HisAccountBook/Get";
        public const string HIS_ACCOUNT_BOOK_GETVIEW = "api/HisAccountBook/GetView";
        public const string HIS_ACCOUNT_BOOK_CREATE = "api/HisAccountBook/Create";
        public const string HIS_ACCOUNT_BOOK_UPDATE = "api/HisAccountBook/Update";
        public const string HIS_ACCOUNT_BOOK_DELETE = "api/HisAccountBook/Delete";
        public const string HIS_ACCOUNT_BOOK_CHANGE_LOG = "api/HisAccountBook/ChangeLock";

        public const string HIS_ACCIDENT_HURT_GET = "api/HisAccidentHurt/Get";
        public const string HIS_ACCIDENT_HURT_GETVIEW = "api/HisAccidentHurt/GetView";
        public const string HIS_ACCIDENT_HURT_CREATE = "api/HisAccidentHurt/Create";
        public const string HIS_ACCIDENT_HURT_UPDATE = "api/HisAccidentHurt/Update";
        public const string HIS_ACCIDENT_HURT_DELETE = "api/HisAccidentHurt/Delete";
        public const string HIS_ACCIDENT_HURT_CHANGE_LOG = "api/HisAccidentHurt/ChangeLock";

        public const string HIS_TRANSACTION_GETVIEW = "api/HisTransaction/GetView";
        public const string HIS_TRANSACTION_TYPE_GET = "api/HisTransactionType/Get";

        public const string HIS_EXAM_SERVICE_TYPE_GET = "api/HisExamServiceType/Get";
        public const string HIS_EXAM_SERVICE_TYPE_GETVIEW = "api/HisExamServiceType/GetView";
        public const string HIS_BED_SERVICE_TYPE_GET = "api/HisBedServiceType/Get";
        public const string HIS_BED_SERVICE_TYPE_GETVIEW = "api/HisBedServiceType/GetView";
        public const string HIS_DIIM_SERVICE_TYPE_GET = "api/HisDiimServiceType/Get";
        public const string HIS_DIIM_SERVICE_TYPE_GETVIEW = "api/HisDiimServiceType/GetView";
        public const string HIS_ENDO_SERVICE_TYPE_GET = "api/HisEndoServiceType/Get";
        public const string HIS_ENDO_SERVICE_TYPE_GETVIEW = "api/HisEndoServiceType/GetView";
        public const string HIS_FUEX_SERVICE_TYPE_GET = "api/HisFuexServiceType/Get";
        public const string HIS_FUEX_SERVICE_TYPE_GETVIEW = "api/HisFuexServiceType/GetView";
        public const string HIS_MISU_SERVICE_TYPE_GET = "api/HisMisuServiceType/Get";
        public const string HIS_MISU_SERVICE_TYPE_GETVIEW = "api/HisMisuServiceType/GetView";
        public const string HIS_REHA_SERVICE_TYPE_GET = "api/HisRehaServiceType/Get";
        public const string HIS_REHA_SERVICE_TYPE_GETVIEW = "api/HisRehaServiceType/GetView";
        public const string HIS_SUIM_SERVICE_TYPE_GET = "api/HisSuimServiceType/Get";
        public const string HIS_SUIM_SERVICE_TYPE_GETVIEW = "api/HisSuimServiceType/GetView";
        public const string HIS_SURG_SERVICE_TYPE_GET = "api/HisSurgServiceType/Get";
        public const string HIS_SURG_SERVICE_TYPE_GETVIEW = "api/HisSurgServiceType/GetView";
        public const string HIS_TEST_SERVICE_TYPE_GET = "api/HisTestServiceType/Get";
        public const string HIS_TEST_SERVICE_TYPE_GETVIEW = "api/HisTestServiceType/GetView";
        public const string HIS_OTHER_SERVICE_TYPE_GET = "api/HisOtherServiceType/Get";
        public const string HIS_OTHER_SERVICE_TYPE_GETVIEW = "api/HisOtherServiceType/GetView";

        public const string HIS_MATERIAL_TYPE_GET = "api/HisMaterialType/Get";
        public const string HIS_MATERIAL_TYPE_GETVIEW = "api/HisMaterialType/GetView";
        public const string HIS_MEDICINE_TYPE_GET = "api/HisMedicineType/Get";
        public const string HIS_MEDICINE_TYPE_GETVIEW = "api/HisMedicineType/GetView";
        public const string HIS_MEDICINE_TYPE_DELETE = "api/HisMedicineType/Delete";

        public const string HIS_MANUFACTURER_GET = "api/HisManufacturer/Get";
        public const string HIS_MANUFACTURER_CREATE = "api/HisManufacturer/Create";

        public const string HIS_EXP_MEST_MATERIAL_GET = "api/HisExpMestMaterial/Get";
        public const string HIS_EXP_MEST_MATERIAL_GETVIEW = "api/HisExpMestMaterial/GetView";
        public const string HIS_EXP_MEST_MATERIAL_GETVIEW_BY_AGGR_EMPMEST_ID_GROUPBY_MATERIAL = "api/HisExpMestMaterial/GetViewByAggrExpMestIdAndGroupByMaterial";
        public const string HIS_EXP_MEST_MEDICINE_GETVIEW_BY_AGGR_EMPMEST_ID_GROUPBY_MEDICINE = "api/HisExpMestMedicine/GetViewByAggrExpMestIdAndGroupByMedicine";

        public const string HIS_MEDICINE_TYPE_IN_STOCK = "api/HisMedicineType/GetInStockMedicineType";

        public const string HIS_IMP_MEST_MATERIAL_GET = "api/HisImpMestMaterial/Get";
        public const string HIS_IMP_MEST_MATERIAL_GETVIEW = "api/HisImpMestMaterial/GetView";
        public const string HIS_IMP_MEST_MATERIAL_GETVIEW_BY_AGGR_IMPMEST_ID_GROUPBY_MATERIAL = "api/HisImpMestMaterial/GetViewByAggrImpMestIdAndGroupByMaterial";
        public const string HIS_IMP_MEST_MEDICINE_GETVIEW_BY_AGGR_IMPMEST_ID_GROUPBY_MEDICINE = "api/HisImpMestMedicine/GetViewByAggrImpMestIdAndGroupByMedicine";

        public const string HIS_AGGREGATE_EXP_MEST_CREATE = "/api/HisAggrExpMest/Create";
        public const string HIS_AGGREGATE_IMP_MEST_CREATE = "/api/HisAggrImpMest/Create";

        public const string HIS_EXP_MEST_GET = "api/HisExpMest/Get";
        public const string HIS_EXP_MEST_GETVIEW = "api/HisExpMest/GetView";
        public const string HIS_EXP_MEST_TEMPLATE_GET = "api/HisExpMestTemplate/Get";
        public const string HIS_EXP_MEST_TEMPLATE_CREATE = "api/HisExpMestTemplate/Create";

        public const string HIS_EXP_MEST_GETVIEW_WITHOUT_DATA_DOMAIN = "api/HisExpMest/GetViewWithoutDataDomainFilter";
        public const string HIS_EXP_MEST_GET_WITHOUT_DATA_DOMAIN = "api/HisExpMest/GetWithoutDataDomainFilter";

        public const string HIS_IMP_MEST_GET = "api/HisImpMest/Get";
        public const string HIS_IMP_MEST_GET_WITHOUT_DATA_DOMAIN = "api/HisImpMest/GetWithoutDataDomainFilter";
        public const string HIS_IMP_MEST_GETVIEW = "api/HisImpMest/GetView";
        public const string HIS_IMP_MEST_GETVIEW_WITHOUT_DATA_DOMAIN = "api/HisImpMest/GetViewWithoutDataDomainFilter";

        public const string HIS_AGGR_EXP_MEST_GETVIEW = "api/HisAggrExpMest/GetView";
        public const string HIS_AGGR_EXP_MEST_CREATE = "api/HisAggrExpMest/Create";

        public const string HIS_AGGR_IMP_MEST_GETVIEW = "api/HisAggrImpMest/GetView";
        public const string HIS_AGGR_IMP_MEST_CREATE = "api/HisAggrImpMest/Create";

        public const string HIS_EXP_MEST_MEDICINE_GET = "api/HisExpMestMedicine/Get";
        public const string HIS_EXP_MEST_MEDICINE_GETVIEW = "api/HisExpMestMedicine/GetView";

        public const string HIS_EXP_MEST_MEDICINE_GETSDO_BY_TREATMENT_ID = "/api/HisExpMestMedicine/GetSdoByTreatmentId";

        public const string HIS_IMP_MEST_MEDICINE_GET = "api/HisImpMestMedicine/Get";
        public const string HIS_IMP_MEST_MEDICINE_GETVIEW = "api/HisImpMestMedicine/GetView";

        public const string HIS_CAREER_GET = "api/HisCareer/Get";

        public const string HIS_BED_ROOM_GETVIEW = "api/HisBedRoom/GetView";

        public const string HIS_EXECUTE_ROOM_GETVIEW = "api/HisExecuteRoom/GetView";

        public const string HIS_ROOM_GETVIEW = "api/HisRoom/GetView";

        public const string HIS_TREATMENT_GETVIEW = "api/HisTreatment/GetView";
        public const string HIS_TREATMENT_GETFEEVIEW = "api/HisTreatment/GetFeeView";
        public const string HIS_TREATMENT_GETSUMMARYVIEW = "api/HisTreatment/GetSummaryView";
        public const string HIS_TREATMENT_GET = "api/HisTreatment/Get";
        public const string HIS_TREATMENT_GETCOMMON_INFO = "api/HisTreatment/GetCommonInfo";
        public const string HIS_TREATMENT_GETCOMMON_INFO_WITHOUT_PROFILE = "api/HisTreatment/GetCommonInfoWithoutProfile";
        public const string HIS_TREATMENT_GETCOMMON_GET_PROFILE_INFO = "api/HisTreatment/GetProfileInfo";
        public const string HIS_TREATMENT_GET_SDO_WAS_IN_DEPARTMENT = "api/HisTreatment/GetSdoWasInDepartment";
        public const string HIS_TREATMENT_FINISH = "api/HisTreatment/Finish";
        public const string HIS_TREATMENT_UNFINISH = "/api/HisTreatment/Unfinish";
        public const string HIS_TREATMENT_GETVIEW_PATIENT_TYPE_ID = "/api/HisTreatment/GetViewByPatientTypeId";


        public const string HIS_MEDI_REACT_GETVIEW = "api/HisMediReact/GetView";
        public const string HIS_MEDI_REACT_GETFEEVIEW = "api/HisMediReact/GetFeeView";
        public const string HIS_MEDI_REACT_GET = "api/HisMediReact/Get";
        public const string HIS_MEDI_REACT_GETCOMMON_INFO = "api/HisMediReact/GetCommonInfo";
        public const string HIS_MEDI_REACT_GET_SDO_WAS_IN_DEPARTMENT = "api/HisMediReact/GetSdoWasInDepartment";

        public const string HIS_MEDI_REACT_DELETE = "api/HisMediReact/Delete";
        public const string HIS_MEDI_REACT_CREATE = "api/HisMediReact/Create";
        public const string HIS_MEDI_REACT_CHECK = "api/HisMediReact/Check";
        public const string HIS_MEDI_REACT_UN_CHECK = "api/HisMediReact/UnCheck";
        public const string HIS_MEDI_REACT_UN_EXECUTE = "api/HisMediReact/UnExecute";
        public const string HIS_MEDI_REACT_EXECUTE = "api/HisMediReact/Execute";
        public const string HIS_MEDI_REACT_FINISH = "api/HisMediReact/Finish";
        public const string HIS_MEDI_REACT_UNFINISH = "/api/HisMediReact/Unfinish";
        public const string HIS_MEDI_REACT_DETAIL_GETVIEW = "api/HisMediReactDetail/GetView";

        public const string HIS_DHST_GET = "api/HisDhst/Get";
        public const string HIS_DHST_DELETE = "api/HisDhst/Delete";
        public const string HIS_TRACKING_GET = "api/HisTracking/Get";
        public const string HIS_TRACKING_GETVIEW = "api/HisTracking/GetView";
        public const string HIS_TRACKING_DELETE = "api/HisTracking/Delete";
        public const string HIS_TRACKING_CREATE = "api/HisTracking/Create";
        public const string HIS_TRACKING_UPDATE = "api/HisTracking/Update";

        public const string HIS_BED_TYPE_GET = "api/HisBedType/Get";
        public const string HIS_BED_TYPE_DELETE = "api/HisBedType/Delete";
        public const string HIS_BED_TYPE_CREATE = "api/HisBedType/Create";
        public const string HIS_BED_TYPE_UPDATE = "api/HisBedType/Update";

        public const string HIS_CARE_GET = "api/HisCare/Get";
        public const string HIS_CARE_DELETE = "api/HisCare/Delete";
        public const string HIS_CARE_CREATE = "api/HisCare/Create";
        public const string HIS_CARE_UPDATE = "api/HisCare/Update";

        public const string HIS_AWARENESS_GET = "api/HisAwareness/Get";
        public const string HIS_AWARENESS_DELETE = "api/HisAwareness/Delete";
        public const string HIS_AWARENESS_CREATE = "api/HisAwareness/Create";
        public const string HIS_AWARENESS_UPDATE = "api/HisAwareness/Update";

        public const string HIS_CARE_DETAIL_GETVIEW = "api/HisCareDetail/GetView";
        public const string HIS_CARE_DETAIL_DELETE = "api/HisCareDetail/Delete";
        public const string HIS_CARE_DETAIL_CREATE = "api/HisCareDetail/Create";
        public const string HIS_CARE_DETAIL_UPDATE = "api/HisCareDetail/Update";

        public const string HIS_INFUSION_GETVIEW = "api/HisInfusion/GetView";
        public const string HIS_INFUSION_GETFEEVIEW = "api/HisInfusion/GetFeeView";
        public const string HIS_INFUSION_GET = "api/HisInfusion/Get";
        public const string HIS_INFUSION_GETCOMMON_INFO = "api/HisInfusion/GetCommonInfo";
        public const string HIS_INFUSION_GET_SDO_WAS_IN_DEPARTMENT = "api/HisInfusion/GetSdoWasInDepartment";
        public const string HIS_INFUSION_DELETE = "api/HisInfusion/Delete";
        public const string HIS_INFUSION_CREATE = "/api/HisInfusion/Create";
        public const string HIS_INFUSION_FINISH = "api/HisInfusion/Finish";
        public const string HIS_INFUSION_UNFINISH = "/api/HisInfusion/Unfinish";
        public const string HIS_INFUSION_DETAIL_GETVIEW = "api/HisInfusionDetail/GetView";

        public const string HIS_ICD = "/api/HisIcd/Get";
        public const string HIS_USER_ROOM_GETVIEW = "api/HisUserRoom/GetView";
        public const string HIS_SERVICE_ROOM_GETVIEW = "api/HisServiceRoom/GetView";
        public const string HIS_MEDI_STOCK_PERIOD_GETVIEW = "api/HisMediStockPeriod/GetView";

        public const string HIS_CARE_TYPE_GET = "api/HisCareType/Get";
        public const string HIS_CARE_TYPE_CREATE = "api/HisCareType/Create";

        public const string HIS_GENDER_GET = "api/HisGender/Get";

        public const string HIS_TEXT_LIB_GET = "api/HisTextLib/Get";

        public const string HIS_MEDICINE_USE_FORM_GET = "api/HisMedicineUseForm/Get";
        public const string HIS_MEDICINE_USE_FORM_CREATE = "api/HisMedicineUseForm/Create";

        public const string HIS_DEPARTMENT_GET = "api/HisDepartment/Get";
        public const string HIS_KSK_CONTRACT_GET = "api/HisKskContract/Get";

        public const string HIS_DEPARTMENT_TRAN_GET = "api/HisDepartmentTran/Get";
        public const string HIS_DEPARTMENT_TRAN_GETHOSPITALINOUT = "api/HisDepartmentTran/GetHospitalInOut";
        public const string HIS_DEPARTMENT_TRAN_GETVIEW = "/api/HisDepartmentTran/GetView";
        public const string HIS_DEPARTMENT_TRAN_RECEIVE = "api/HisDepartmentTran/Receive";
        public const string HIS_DEPARTMENT_TRAN_UPDATE = "api/HisDepartmentTran/Update";
        public const string HIS_DEPARTMENT_TRAN_CREATE = "api/HisDepartmentTran/Create";

        public const string HIS_PATIENT_TYPE_ALTER_UPDATE = "api/HisPatientTypeAlter/Update";
        public const string HIS_PATIENT_TYPE_ALTER_CREATE = "api/HisPatientTypeAlter/Create";
        public const string HIS_PATIENT_TYPE_ALTER_AND_TRAN_PATI_UPDATE = "api/HisPatientTypeAlter/Update";

        public const string HIS_IMP_MEST_STT_GET = "api/HisImpMestStt/Get";

        public const string HIS_IMP_MEST_TYPE_GET = "api/HisImpMestType/Get";

        public const string HIS_INFUSION_STT_GET = "api/HisInfusionStt/Get";

        public const string HIS_MEDI_REACT_STT_GET = "api/HisMediReactStt/Get";

        public const string HIS_MEDI_REACT_TYPE_GET = "api/HisMediReactType/Get";

        public const string HIS_EXP_MEST_STT_GET = "api/HisExpMestStt/Get";

        public const string HIS_EXP_MEST_TYPE_GET = "api/HisExpMestType/Get";

        public const string HIS_MEDICINE_TYPE_ACIN_GET = "api/HisMedicineTypeAcin/Get";
        public const string HIS_MEDICINE_TYPE_ACIN_GETVIEW = "api/HisMedicineTypeAcin/GetView";
        public const string HIS_PATIENT_TYPE_ALLOW_GET = "api/HisPatientTypeAllow/Get";
        public const string HIS_PATIENT_TYPE_ALLOW_GETVIEW = "api/HisPatientTypeAllow/GetView";
        public const string HIS_PATIENT_TYPE_ALTER_GET = "/api/HisPatientTypeAlter/Get";
        public const string HIS_PATIENT_TYPE_ALTER_GET_APPLIED = "/api/HisPatientTypeAlter/GetApplied";
        public const string HIS_PATIENT_TYPE_ALTER_GET_VIEW = "/api/HisPatientTypeAlter/GetView";
        public const string HIS_PATIENT_TYPE_ALTER_GET_LAST_BY_TREATMENTID = "/api/HisPatientTypeAlter/GetLastByTreatmentId";

        public const string HIS_PATIENT_GET = "api/HisPatient/Get";
        public const string HIS_PATIENT_GETVIEW = "api/HisPatient/GetView";

        public const string HIS_CARD_GETVIEWBYSERVICECODE = "api/HisCard/GetViewByCode";

        public const string HIS_PATIENT_UPDATE = "api/HisPatient/Update";
        public const string HIS_PATIENT_UPDAT = "/api/HisPatient/Update";

        public const string HIS_TEST_INDEX_RANGE_GETVIEW = "api/HisTestIndexRange/GetView";

        public const string HIS_ICD_GET = "api/HisIcd/Get";

        public const string HIS_PATIENT_TYPE_GET = "api/HisPatientType/Get";

        public const string HIS_ROOM_TYPE_GET = "api/HisRoomType/Get";

        public const string HIS_SERVICE_REPORT_GET = "api/HisServiceReport/Get";

        public const string HIS_SERVICE_REQ_STT_GET = "api/HisServiceReqStt/Get";
        public const string HIS_SERVICE_REQ_TYPE_GET = "api/HisServiceReqType/Get";

        public const string HIS_SERVICE_REQ_GETVIEW = "api/HisServiceReq/GetView";
        public const string HIS_SERVICE_REQ_GETVIEWUSINGORDER = "api/HisServiceReq/GetViewUsingOrder";
        public const string HIS_SERVICE_REQ_CALL = "api/HisServiceReq/Call";

        public const string HIS_EXAM_SERVICE_REQ_GET = "api/HisExamServiceReq/Get";
        public const string HIS_EXAM_SERVICE_REQ_REGISTER = "api/HisExamServiceReq/Register";
        public const string HIS_SERVICE_REQ_EXAMREGISTER = "api/HisServiceReq/ExamRegister";
        public const string HIS_PATIENT_REGISTER_PROFILE = "api/HisPatient/RegisterProfile";
        public const string HIS_EXAM_SERVICE_REQ_GETVIEW = "api/HisExamServiceReq/GetView";

        public const string HIS_PRESCRIPTION_GETVIEW_5 = "api/HisPrescription/GetView5";
        public const string HIS_PRESCRIPTION_GETVIEW = "api/HisPrescription/GetView";
        public const string HIS_PRESCRIPTION_CREATE = "/api/HisPrescription/Create";
        public const string HIS_SERVICE_REQ__BLOODPRESCREATE = "/api/HisServiceReq/BloodPresCreate";

        public const string HIS_EXP_MEST_DELETE = "/api/HisExpMest/Delete";
        public const string HIS_IMP_MEST_DELETE = "/api/HisImpMest/Delete";

        public const string HIS_MOBA_GETVIEW = "api/HisMobaImpMest/GetView";

        public const string HIS_SERVICE_REQ_GET = "api/HisServiceReq/Get";
        public const string HIS_SERVICE_REQ_GET_VIEW_WITH_HOSPITAL_FEE_INFO = "api/HisServiceReq/GetViewWithHospitalFeeInfo";
        public const string HIS_SERVICE_TYPE_GET = "api/HisServiceType/Get";

        public const string HIS_SERVICE_GROUP_GET = "api/HisServiceGroup/Get";

        public const string HIS_SERVICE_PATY_GETVIEW = "api/HisServicePaty/GetView";

        public const string HIS_MEDI_STOCK_GETVIEW = "api/HisMediStock/GetView";
        public const string HIS_MEDI_STOCK_GET = "api/HisMediStock/Get";

        public const string HIS_SERVICE_GETVIEW = "api/HisService/GetView";

        public const string HIS_SERVICE_GET = "api/HisService/Get";

        public const string HIS_SERV_SEGR_GET = "api/HisServSegr/Get";
        public const string HIS_SERV_SEGR_GETVIEW = "api/HisServSegr/GetView";
        public const string HIS_SERE_SERV_GETVIEW2 = "api/HisSereServ/GetView2";
        public const string HIS_SERE_SERV_GETVIEW = "api/HisSereServ/GetView";
        public const string HIS_SERVICE_REQ_START = "api/HisServiceReq/Start";

        public const string HIS_SERVICE_REQ_FINISH = "/api/HisServiceReq/Finish";
        public const string HIS_SERVICE_REQ_UPDATE = "/api/HisServiceReq/UpdateSereServ";
        public const string HIS_SERVICE_REQ_CREATE_COMBO = "api/HisServiceReq/CreateList";

        public const string HIS_SERVICE_REQ_UNSTART = "api/HisServiceReq/UnStart";

        public const string HIS_SERE_SERV_UPDATE = "/api/HisSereServ/UpdatePatientTypeInfo";

        public const string HIS_SERE_SERV_UPDATE_PATENT_TYPE_INFO_LIST = "/api/HisSereServ/UpdatePatientTypeInfoList";

        public const string HIS_TRAN_PATI_TYPE_GET = "api/HisTranPatiType/Get";

        //public const string HIS_PATY_ALTER_BHYT_GET = "api/HisPatyAlterBhyt/Get";
        //public const string HIS_PATY_ALTER_BHYT_GETVIEW = "api/HisPatyAlterBhyt/GetView";
        public const string HIS_PATY_ALTER_BHYT_GETVIEW_BY_HEIN_CARD_NUMBER = "api/HisPatyAlterBhyt/GetViewByHeinCardNumber";
        //public const string HIS_PATY_ALTER_KSK_GETVIEW = "api/HisPatyAlterKsk/GetView";
        //public const string HIS_PATY_ALTER_AIA_GETVIEW = "api/HisPatyAlterAia/GetView";

        public const string HIS_TRAN_PATI_FORM_GET = "api/HisTranPatiForm/Get";

        public const string HIS_TRAN_PATI_REASON_GET = "api/HisTranPatiReason/Get";

        public const string HIS_TRAN_PATI_TECH_GET = "api/HisTranPatiTech/Get";

        public const string HIS_TREATMENT_END_TYPE_GET = "api/HisTreatmentEndType/Get";

        public const string HIS_TREATMENT_RESULT_GET = "api/HisTreatmentResult/Get";

        public const string HIS_DISEASE_RELATION_GET = "api/HisDiseaseRelation/Get";

        public const string HIS_TREATMENT_LOG_TYPE_GET = "api/HisTreatmentLogType/Get";
        public const string HIS_TREATMENT_TYPE_GET = "api/HisTreatmentType/Get";
        public const string HIS_TREATMENT_LOG_GET = "/api/HisTreatmentLog/GetCombo";
        public const string HIS_HEIN_SERVICE_TYPE_GET = "api/HisHeinServiceType/Get";

        public const string HIS_TREATMENT_LOG_DELETE_DEPARTMENT = "/api/HisDepartmentTran/Delete";
        public const string HIS_TREATMENT_LOG_DELETE_MEDI_RECORD = "/api/HisMediRecord/Delete";
        public const string HIS_TREATMENT_LOG_DELETE_PATIENT_TYPE_ALTER = "/api/HisPatientTypeAlter/Delete";
        public const string HIS_TREATMENT_LOG_UPDATE_MEDI_RECORD = "/api/HisMediRecord/Update";
        public const string HIS_TREATMENT_LOG_CREATE_MEDI_RECORD = "/api/HisMediRecord/Create";
        public const string HIS_TREATMENT_LOG_CREATE_DEPARTMENT_TRAN = "/api/HisDepartmentTran/Create";
        public const string HIS_TREATMENT_LOG_UPDATE_DEPARTMENT_TRAN = "/api/HisDepartmentTran/Update";

        public const string HIS_DISEASE_RELATION = "/api/HisDiseaseRelation/Get";

        public const string HIS_EXAM_SERVICE_REQ_UPDATE = "/api/HisExamServiceReq/Update";

        public const string HIS_SERVICE_REQ_GET_ = "/api/HisServiceReq/Get";

        public const string HIS_SERE_SERV_GET = "/api/HisSereServ/Get";

        public const string HIS_TEST_INDEX_RANGE_GET = "/api/HisTestIndexRange/GetView";

        public const string HIS_TEST_SERVICE_REQ_UPDATE = "/api/HisTestServiceReq/UpdateResult";

        public const string HIS_TEST_SERVICE_REQ_GET = "/api/HisTestServiceReq/GetView";

        public const string HIS_SERE_SERV_TEIN_GET = "/api/HisSereServTein/GetView";

        public const string HIS_TEST_INDEX_GET = "/api/HisTestIndex/GetView";

        public const string HIS_SERE_SERV_UPDATE_WITH_FILE = "/api/HisSereServ/UpdateWithFile";

        public const string HIS_DIIM_SERVICE_REQ_GET = "/api/HisDiimServiceReq/GetView";

        public const string HIS_REHA_SERVICE_REQ_UPDATE = "/api/HisRehaServiceReq/Update";

        public const string HIS_SERE_SERV_REHA__CREATE = "api/HisSereServReha/Create";

        public const string HIS_REHA_TRAIN__DELETE = "api/HisRehaTrain/Delete";

        public const string HIS_REHA_TRAIN_GET_BY_SERVICE_REQ_ID = "api/HisRehaTrain/GetViewByServiceReqId";

        public const string HIS_REST_RETR_TYPE_GETVIEW = "api/HisRestRetrType/GetView";

        public const string HIS_REHA_SERVICE_REQ_GET = "/api/HisRehaServiceReq/Get";

        public const string HIS_REHA_TRAIN_TYPE_GET = "api/HisRehaTrainType/Get";
        public const string HIS_REHA_TRAIN_TYPE_GETVIEW = "api/HisRehaTrainType/GetView";

        public const string HIS_REHA_TRAIN_GETVIEW = "api/HisRehaTrain/GetView";

        public const string HIS_REHA_TRAIN__CREATE = "api/HisRehaTrain/CreateList";

        public const string HIS_ENDO_SERVICE_REQ_GETVIEW = "/api/HisEndoServiceReq/GetView";

        public const string HIS_FUEX_SERVICE_REQ_GETVIEW = "/api/HisFuexServiceReq/GetView";

        public const string HIS_SUMI_SERVICE_REQ_GETVIEW = "/api/HisSuimServiceReq/GetView";

        public const string HIS_SERE_SERV_FILE_GET = "/api/HisSereServFile/Get";

        public const string HIS_OTHER_SERVICE_REQ_GETVIEW = "/api/HisOtherServiceReq/GetView";

        public const string HIS_SERVICE_REQ_CHANGEROOM = "/api/HisServiceReq/ChangeRoom";

        public const string HIS_SERVICE_REQ_CHANGEROOMLIST = "/api/HisServiceReq/ChangeRoomList";

        public const string HIS_SURG_SERVICE_REQ_UPDATE = "/api/HisSurgServiceReq/Update";

        public const string HIS_EMERGENCY_TIME_GET = "/api/HisEmergencyWTime/Get";

        public const string HIS_USER_ROOM_GET = "api/HisUserRoom/Get";
        public const string HIS_ROOM_GETVIEW_COUNTER = "api/HisRoom/GetCounterView";
        public const string HIS_TREATMENT_BED_ROOM_GETVIEW = "/api/HisTreatmentBedRoom/GetView";
        public const string HIS_TREATMENT_BED_ROOM_GETVIEW_CURRENT_IN = "/api/HisTreatmentBedRoom/GetViewCurrentIn";
        public const string HIS_TREATMENT_BED_ROOM_GETVIEW_CURRENT_IN_BY_BED_ROOM_ID = "/api/HisTreatmentBedRoom/GetViewSdoCurrentInByBedRoomId";

        public const string HIS_DEATH_CAUSE_GET = "/api/HisDeathCause/Get";
        public const string HIS_DEATH_WITHIN_GET = "/api/HisDeathWithin/Get";
        public const string HIS_DEATH_GET = "api/HisDeath/Get";
        public const string HIS_DEATH_DELETE = "/api/HisDeath/Delete";
        public const string V_HIS_DEATH_GET = "/api/HisDeath/Get";
        public const string HIS_DEATH_UPDATE = "/api/HisDeath/Update";

        public const string HIS_DHST_CREATE = "api/HisDhst/Create";
        public const string HIS_DHST_UPDATE = "api/HisDhst/Update";

        public const string HIS_DEBATE_USER_GET = "api/HisDebateUser/Get";
        public const string HIS_DEBATE_CREATE = "/api/HisDebate/Create";
        public const string HIS_DEBATE_UPDATE = "/api/HisDebate/Update";
        public const string HIS_DEBATE_DELETE = "/api/HisDebate/Delete";

        public const string HIS_BLOOD_ABO__GET = "api/HisBloodABO/Get";

        public const string HIS_BLOOD_RH__GET = "api/HisBloodRh/Get";

        public const string HIS_MEST_ROOM_GETVIEW = "api/HisMestRoom/GetView";

        public const string HIS_MEST_PATIENT_TYPE_GET = "api/HisMestPatientType/Get";

        public const string HIS_EXP_MEST_MEDICINE = "api/HisExpMestMedicine/GetView";

        public const string HIS_METERIAL_TYPE_GET_IN_STOCK_MATERIAL_TYPE = " api/HisMaterialType/GetInStockMaterialType";

        public const string HIS_METERIAL_TYPE_GET = "api/HisMedicineType/GetView";

        public const string HIS_SERVICE_REQ_DELETE = "/api/HisServiceReq/Delete";

        public const string HIS_EXP_MEST_METY_GETVIEW = "/api/HisExpMestMety/GetView";

        public const string HIS_EXAM_SERE_DIRE_GETVIEW = "/api/HisExamSereDire/GetView";

        public const string HIS_EXP_MEST_MATY_GETVIEW = "/api/HisExpMestMaty/GetView";

        public const string HIS_EMTE_MATERIAL_TYPE_GETVIEW = "api/HisEmteMaterialType/GetView";

        public const string HIS_EMTE_MEDICINE_TYPE_GETVIEW = "api/HisEmteMedicineType/GetView";

        public const string HIS_MOBA_IMP_MEST_CREATE = "api/HisMobaImpMest/Create";

        public const string HIS_SERVICE_REQ_UNFINISH = "/api/HisServiceReq/Unfinish";

        public const string HIS_REHA_SERVICE_REQ_GETVIEW = "api/HisRehaServiceReq/GetView";
        public const string HIS_REHA_SUM_GETVIEW = "api/HisRehaSum/GetView";
        public const string HIS_REHA_SUM_CREATE = "api/HisRehaSum/Create";
        public const string HIS_REHA_SUM_UPDATE = "api/HisRehaSum/Update";
        public const string HIS_REHA_SUM_DELETE = "/api/HisRehaSum/Delete";

        public const string HIS_SERE_SERV_REHA_GETVIEW = "api/HisSereServReha/GetView";

        public const string HIS_BED_GETVIEW = "api/HisBed/GetView";

        public const string HIS_TREATMENT_BEDROOM_REMOVE = "/api/HisTreatmentBedRoom/Remove";
        public const string HIS_TREATMENT_BEDROOM_CREATE = "api/HisTreatmentBedRoom/Create";
        public const string HIS_TREATMENT_BEDROOM_CREATE_SDO = "api/HisTreatmentBedRoom/CreateSdo";

        public const string HIS_SERVICE_UNIT_GET = "api/HisServiceUnit/Get";

        public const string HIS_TRAN_PATI_UPDATE = "api/HisTranPati/Update";
        public const string HIS_TRAN_PATI_DELETE = "api/HisTranPati/Delete";
        public const string HIS_TRAN_PATI_GETVIEW = "api/HisTranPati/GetView";

        public const string HIS_MATERIAL_GETVIEW = "/api/HisMaterial/GetView";

        public const string HIS_MEDICINE_GETVIEW = "/api/HisMedicine/GetView";
        public const string HIS_MEDICINE_LINE_GET = "/api/HisMedicineLine/Get";
        public const string HIS_MEDICINE_LINE_CREATE = "/api/HisMedicineLine/Create";

        public const string HIS_TREATMENT_OUT_GETVIEW = "api/HisTreatmentOut/GetView";

        public const string HIS_PATIENT_TYPE_ALTER__GETVIEW = "api/HisPatientTypeAlter/GetView";

        public const string HIS_SERE_SERV_PTTT_GETVIEW = "api/HisSereServPttt/GetView";

        public const string HIS_MILITARY_RANK_GET = "api/HisMilitaryRank/Get";
        public const string HIS_WORK_PLACE_GET = "api/HisWorkPlace/Get";
        public const string HIS_WORK_PLACE_CREATE = "api/HisWorkPlace/Create";
        public const string HIS_REHA_TRAIN_GET_BY_REHA_SUM_ID = "api/HisRehaTrain/GetViewByRehaSumId";
        public const string HIS_INFUSION_SUM_GETVIEW = "api/HisInfusionSum/GetView";
        public const string HIS_INFUSION_SUM_CREATE = "api/HisInfusionSum/Create";
        public const string HIS_INFUSION_SUM_UPDATE = "api/HisInfusionSum/Update";
        public const string HIS_INFUSION_SUM_DELETE = "/api/HisInfusionSum/Delete";
        public const string HIS_TRACKING_SUM_GETVIEW = "api/HisTrackingSum/GetView";
        public const string HIS_TRACKING_SUM_CREATE = "api/HisTrackingSum/Create";
        public const string HIS_TRACKING_SUM_UPDATE = "api/HisTrackingSum/Update";
        public const string HIS_TRACKING_SUM_DELETE = "/api/HisTrackingSum/Delete";
        public const string HIS_CARE_SUM_GETVIEW = "api/HisCareSum/GetView";
        public const string HIS_CARE_SUM_CREATE = "api/HisCareSum/Create";
        public const string HIS_CARE_SUM_UPDATE = "api/HisCareSum/Update";
        public const string HIS_CARE_SUM_DELETE = "/api/HisCareSum/Delete";
        public const string HIS_EXECUTE_GROUP_GET = "api/HisExecuteGroup/Get";
        public const string HIS_TREATMENT_UPDATE_JSON = "/api/HisTreatment/UpdateJsonPrintId";
        public const string HIS_SERVICE_REQ_UPDATE_JSON = "/api/HisServiceReq/UpdateJsonPrintId";

        public const string HIS_BLOOD_TYPE_GET = "/api/HisBloodType/Get";
        public const string HIS_BLOOD_TYPE_GETVIEW = "api/HisBloodType/GetView";
        public const string HIS_BLOOD_TYPE_CREATE = "api/HisBloodType/Create";
        public const string HIS_BLOOD_TYPE_CHANGE_LOCK = "api/HisBloodType/ChangeLock";
        public const string HIS_BLOOD_TYPE_UPDATE = "api/HisBloodType/Update";
        public const string HIS_BLOOD_TYPE_DELETE = "api/HisBloodType/Delete";


        public const string HIS_ACTIVE_INGR_BHYT_GET = "api/HisActiveIngrBhyt/Get";
        public const string HIS_MEDICINE_TYPE_CREATE = "api/HisMedicineType/Create";
        public const string HIS_MEDICINE_TYPE_CHANGE_LOCK = "api/HisMedicineType/ChangeLock";
        public const string HIS_MEDICINE_TYPE_UPDATE = "api/HisMedicineType/Update";

        public const string HIS_HEIN_SERVICE_BHYT_CREATE = "api/HisHeinServiceBhyt/Create";
        public const string HIS_HEIN_SERVICE_GET = "api/HisHeinService/Get";
        public const string HIS_HEIN_SERVICE_BHYT_GETVIEW = "api/HisHeinServiceBhyt/GetView";

        public const string HIS_MEDI_ORG_GET = "api/HisMediOrg/Get";
        public const string HIS_PACKAGE_GET = "api/HisPackage/Get";
        public const string HIS_PACKING_TYPE_GET = "api/HisPackingType/Get";

        public const string TOKEN__UPDATE_WORK_PLACE = "api/Token/UpdateWorkplace";

        public const string HIS_SERE_SERV_HEIN_GETVIEW = "/api/HisSereServHein/GetView";

        public const string HIS_HEIN_APPROVAL_BHYT_CREATE = "/api/HisHeinApprovalBhyt/Create";

        public const string HIS_BILL_GETVIEW = "api/HisBill/GetView";
        public const string HIS_BILL_GET = "api/HisBill/Get";

        public const string HIS_EXP_MEST_BLTY_GET = "api/HisExpMestBlty/Get";
        public const string HIS_EXP_MEST_BLTY_GETVIEW = "api/HisExpMestBlty/GetView";
        public const string HIS_EXP_MEST_BLTY_CREATE = "api/HisExpMestBlty/Create";
        public const string HIS_BLOOD_GET = "api/HisBlood/Get";
        public const string HIS_BLOOD_GETVIEW = "api/HisBlood/GetView";
        public const string HIS_EXP_MEST_EXPORTBLOOD = "api/HisExpMest/ExportBlood";
        public const string HIS_EXP_MEST_BLOOD_GET = "api/HisExpMestBlood/Get";
        public const string HIS_EXP_MEST_BLOOD_GETVIEW = "api/HisExpMestBlood/GetView";

        public static string HIS_MATERIAL_TYPE_CHANGE_LOCK = "api/HisMaterialType/ChangeLock";
        public static string HIS_MATERIAL_TYPE_DELETE = "api/HisMaterialType/Delete";
        public static string HIS_MATERIAL_TYPE_CREATE = "api/HisMaterialType/Create";
        public static string HIS_MATERIAL_TYPE_UPDATE = "api/HisMaterialType/update";

        public static string HIS_PACKING_TYPE_CREATE = "api/HisPackingType/Create";

        public const string HIS_DEPOSIT_DELETE = "api/HisDeposit/Delete";
        public const string HIS_DEPOSIT_CREATE = "api/HisTransaction/CreateDeposit";

        public const string HIS_PAY_FORM_GET = "api/HisPayForm/Get";

        public const string HIS_DERE_DETAIL_GETVIEW = "api/HisDereDetail/GetView";
        public const string HIS_DERE_DETAIL_GET = "api/HisDereDetail/Get";

        public const string HIS_SUPPLIER_GET = "api/HisSupplier/Get";

        public const string HIS_INVOICE_BOOK_GET__VIEW = "api/HisInvoiceBook/GetView";
        public const string HIS_INVOICE_BOOK_CREATE = "api/HisInvoiceBook/Create";
        public const string HIS_INVOICE_BOOK_DELETE = "api/HisInvoiceBook/Delete";

        public const string HIS_INVOICE__CREATE = "api/HisInvoice/Create";

        public const string HIS_INVOICE_GET__VIEW = "api/HisInvoice/GetView";
        public const string HIS_INVOICE_DETAIL = "api/HisInvoiceDetail/Get";

        public const string HIS_USER_INVOICE_BOOK_GET__VIEW = "api/HisUserInvoiceBook/GetView";
        public const string HIS_USER_INVOICE_BOOK_CREATE = "api/HisUserInvoiceBook/Create";

        public const string HIS_IMP_SOURCE_GET = "api/HisImpSource/Get";

        public const string HIS_INIT_IMP_MEST_GET = "api/HisInitImpMest/Get";
        public const string HIS_INVE_IMP_MEST_GET = "api/HisInveImpMest/Get";
        public const string HIS_OTHER_IMP_MEST_GET = "api/HisOtherImpMest/Get";
        public const string HIS_MANU_IMP_MEST_GET = "api/HisManuImpMest/Get";
        public const string HIS_IMP_MEST_UPDATE_STATUS = "api/HisImpMest/UpdateStatus";
        public const string HIS_IMP_MEST_IMPORT = "api/HisImpMest/Import";

        public const string HIS_HEIN_APPROVAL_GET = "api/HisHeinApproval/Get";
        public const string HIS_PATY_ALTER_BHYT_GETVIEW_DISTINCT = "/api/HisPatyAlterBhyt/GetViewDistinct";

        public const string HIS_MANU_IMP_MEST_CREATE = "api/HisManuImpMest/Create";
        public const string HIS_MANU_IMP_MEST_UPDATE = "api/HisManuImpMest/Update";
        public const string HIS_MANU_IMP_MEST_GETVIEW = "api/HisManuImpMest/GetView";

   

        public static string HIS_SERVICE_PATY_POLICY = "api/HisServicePatyPrPo/GetView";

        public const string HIS_BID_GET = "api/HisBid/Get";
        public const string HIS_BID_GETVIEW = "api/HisBid/GetView";
        public const string HIS_BID_DELETE = "api/HisBid/Delete";

        public const string HIS_PTTT_GROUP_GET = "/api/HisPtttGroup/Get";
        public const string HIS_BED_BSTY_GETVIEW = "/api/HisBedBsty/GetView";
        public const string HIS_BED_LOG_CREATE = "/api/HisBedLog/Create";
        public const string HIS_BED_LOG_UPDATE = "api/HisBedLog/Update";

        public const string HIS_SERE_SERV_UPDATEDISCOUNT = "/api/HisSereServ/UpdateDiscount";
        public const string HIS_REPAY_CREATE = "api/HisTransaction/CreateRepay";

        public const string HIS_BILL_CANCEL = "api/HisBill/Cancel";
        public const string HIS_TREATMENT_LOCK_GET = "api/HisTreatmentLock/Get";
        public const string HIS_TREATMENT_GETVIEW_1 = "api/HisTreatment/GetView1";

        public const string HIS_MEDICINE_TYPE_GETINSTOCKMEDICINETYPE = "api/HisMedicineType/GetInStockMedicineType";
        public const string HIS_EXP_MEST_MEDICINE_GETVIEWANDINCLUDECHIILDRENBYEXPMESTID = "api/HisExpMestMedicine/GetViewAndIncludeChildrenByExpMestId";

        public const string HIS_TREATMENT_LOCK_HEIN = "api/HisTreatment/LockHein";
        public const string HIS_TREATMENT_UNLOCK_HEIN = "api/HisTreatment/UnlockHein";

        public const string HIS_TREATMENT_GETVIEW_2 = "api/HisTreatment/GetView2";
        public const string HIS_TREATMENT_CHANGE_LOCK = "api/HisTreatment/ChangeLock";

        public const string HIS_AGGR_IMP_MEST_CREATE_ODD = "api/HisAggrImpMest/CreateOdd";
        public const string HIS_ANTICIPATE_DELETE = "/api/HisAnticipate/Delete";

        public const string HIS_BABY_CREATE = "api/HisBaby/Create";
        public const string HIS_BABY_UPDATE = "api/HisBaby/Update";
        public const string HIS_BABY_DELETE = "api/HisBaby/Delete";
        public const string HIS_BABY_GETVIEW = "api/HisBaby/GetView";
        public const string HIS_DEBATE_GET = "api/HisDebate/Get";

        public const string HIS_BILL_CREATE = "api/HisBill/Create";
        public const string HIS_FUND_GET = "api/HisFund/Get";

        public const string HIS_INVOICE_PRINT_CREATE = "api/HisInvoicePrint/Create";
        public const string HIS_ACCIDENT_HURT_TYPE_GET = "api/HisAccidentHurtType/Get";
        public const string HIS_TRAN_PATI_GET = "api/HisTranPati/Get";
        public const string HIS_HEIN_APPROVAL_BHYT_CREATENEW = "/api/HisHeinApprovalBhyt/CreateNew";
        public const string HIS_TREATMENT_GETVIEW_3 = "api/HisTreatment/GetView3";
        public const string HIS_SERE_SERV_GETVIEW_2 = "api/HisSereServ/GetView2";
        public const string HIS_SERE_SERV_GETVIEW_4 = "api/HisSereServ/GetView4";
        public const string HIS_GIBA_IMP_MEST_CREATE = "api/HisGibaImpMest/Create";
        public const string HIS_SERVICE_PATY_GET_APPLIED_VIEW = "api/HisServicePaty/GetAppliedView";
        public const string HIS_INIT_IMP_MEST_CREATE = "api/HisInitImpMest/Create";
        public const string HIS_INIT_IMP_MEST_UPDATE = "api/HisInitImpMest/Update";
        public const string HIS_INVE_IMP_MEST_CREATE = "api/HisInveImpMest/Create";
        public const string HIS_INVE_IMP_MEST_UPDATE = "api/HisInveImpMest/Update";
        public const string HIS_OTHER_IMP_MEST_CREATE = "api/HisOtherImpMest/Create";
        public const string HIS_OTHER_IMP_MEST_UPDATE = "api/HisOtherImpMest/Update";

        public const string HIS_MATERIAL_TYPE_GET_IN_STOCK = "api/HisMaterialType/GetInStockMaterialType";
        public const string HIS_CHMS_EXP_MEST_CREATE = "api/HisChmsExpMest/Create";
        public const string HIS_CHMS_EXP_MEST_UPDATE = "api/HisChmsExpMest/Update";
        public const string HIS_CHMS_EXP_MEST_GETVIEW = "api/HisChmsExpMest/GetView";
        public const string HIS_SALE_EXP_MEST_CREATE = "api/HisSaleExpMest/Create";
        public const string HIS_SALE_EXP_MEST_UPDATE = "api/HisSaleExpMest/Update";
        public const string HIS_TRANSACTION_CHANGE_LOCK = "api/HisTransaction/ChangeLock";
        public const string HIS_SALE_EXP_MEST_GETVIEW = "api/HisSaleExpMest/GetView";
        public const string HIS_DEPA_EXP_MEST_CREATE = "api/HisDepaExpMest/Create";
        public const string HIS_DEPA_EXP_MEST_UPDATE = "api/HisDepaExpMest/Update";
        public const string HIS_DEPA_EXP_MEST_GETVIEW = "api/HisDepaExpMest/GetView";

        public const string HIS_CASHIER_ROOM_GET = "api/HisCashierRoom/Get";
        public const string HIS_CASHIER_ROOM_GETVIEW = "api/HisCashierRoom/GetView";
        public const string HIS_EXPMEST_REMOVEAGGR = "api/HisExpmest/RemoveAggr";
        public const string HIS_EXP_MEST_MEDICINE_GETVIEW1 = "api/HisExpMestMedicine/GetView1";
        public const string HIS_EXP_MEST_MATERIAL_GETVIEW1 = "api/HisExpMestMaterial/GetView1";
        public const string HIS_IMP_MEST_MEDICINE_GETVIEW_AND_INCLUDE_CHILDREN_BY_IMP_MEST_ID = "api/HisImpMestMedicine/GetViewAndIncludeChildrenByImpMestId";
        public const string HIS_IMP_MEST_MATERIAL_GETVIEW_AND_INCLUDE_CHILDREN_BY_IMP_MEST_ID = "api/HisImpMestMaterial/GetViewAndIncludeChildrenByImpMestId";
        public const string HIS_IMP_MEST_BLOOD_GETVIEW = "api/HisImpMestBlood/GetView";
        public const string HIS_MOBA_IMP_MEST_GETVIEW = "api/HisMoBaImpMest/GetView";
        public const string HIS_CHMS_IMP_MEST_GETVIEW = "api/HisChmsImpMest/GetView";

        public const string HIS_PRESCRIPTION_1_GETVIEW = "api/HisPrescription/GetView1";
        public const string HIS_EXP_MEST_UPDATE_STATUS = "api/HisExpMest/UpdateStatus";
        public const string HIS_EXP_MEST_EXPORT = "api/HisExpMest/Export";
        public const string HIS_EXP_MEST_1_GETVIEW = "api/HisExpMest/GetView1";
        public const string HIS_ANTICIPATE_CREATE = "api/HisAnticipate/Create";
        public const string HIS_ANTICIPATE_BLTY_GETVIEW = "api/HisAnticipateBlty/GetView";
        public const string HIS_ANTICIPATE_MATY_GETVIEW = "api/HisAnticipateMaty/GetView";
        public const string HIS_ANTICIPATE_METY_GETVIEW = "api/HisAnticipateMety/GetView";

        public const string HIS_BID_BLOOD_TYPE_GETVIEW = "api/HisBidBloodType/GetView";
        public const string HIS_BID_MATERIAL_TYPE_GETVIEW = "api/HisBidMaterialType/GetView";
        public const string HIS_BID_MEDICINE_TYPE_GETVIEW = "api/HisBidMedicineType/GetView";
        public const string HIS_BID_CREATE = "api/HisBid/Create";

        public const string HIS_CHMS_EXP_MEST_1_GETVIEW = "api/HisChmsExpMest/GetView1";

        public const string HIS_MEDICINE_1_GETVIEW = "api/HisMedicine/GetView1";
        public const string HIS_MEDICINE_LOCK = "api/HisMedicine/Lock";
        public const string HIS_MEDICINE_UNLOCK = "api/HisMedicine/UnLock";
        public const string HIS_MEDICINE_UPDATE = "api/HisMedicine/Update";

        public const string HIS_MATERIAL_1_GETVIEW = "api/HisMaterial/GetView1";
        public const string HIS_MATERIAL_LOCK = "api/HisMaterial/Lock";
        public const string HIS_MATERIAL_UNLOCK = "api/HisMaterial/UnLock";
        public const string HIS_MATERIAL_UPDATE = "api/HisMaterial/Update";

        public const string HIS_TREATMENT_UPDATE_PATIENT = "api/HisTreatment/UpdatePatient";

        public const string HIS_SERE_SERV_TEMP_GET = "api/HisSereServTemp/Get";
        public const string HIS_SERE_SERV_TEMP_CREATE = "api/HisSereServTemp/Create";
        public const string HIS_SERE_SERV_TEMP_UPDATE = "api/HisSereServTemp/Update";
        public const string HIS_SERE_SERV_TEMP_DELETE = "api/HisSereServTemp/Delete";

        public const string HIS_MANU_IMP_MEST_UPDATE_INFO = "api/HisManuImpMest/UpdateInfo";

        public const string HIS_ROOM_TYPE_MODULE__GET = "api/HisRoomTypeModule/Get";
        public const string TOKEN__UPDATE_WORK_PLACE_LIST = "api/Token/UpdateWorkPlaceList";
        public const string TOKEN__UPDATE_WORK_PLACE_INFO = "api/Token/UpdateWorkInfo";
        public const string HIS_EXECUTE_ROOM_GET = "api/HisExecuteRoom/Get";
        public const string HIS_SERVICE_FOLLOW_GET = "api/HisServiceFollow/Get";
        public const string HIS_SERVICE_FOLLOW_GETVIEW = "api/HisServiceFollow/GetView";

        public const string HIS_PTTT_METHOD_GET = "api/HisPtttMethod/Get";
        public const string HIS_EMOTIONLESS_METHOD_GET = "api/HisEmotionlessMethod/Get";
        public const string HIS_PTTT_CONDITION_GET = "api/HisPtttCondition/Get";
        public const string HIS_PTTT_CATASTROPHE_GET = "api/HisPtttCatastrophe/Get";
        public const string HIS_PRICE_POLICY_GET = "api/HisPricePolicy/Get";
        public const string HIS_EXECUTE_ROLE_GET = "api/HisExecuteRole/Get";
        public const string HIS_BORN_TYPE_GET = "api/HisBornType/Get";
        public const string HIS_BORN_POSITION_GET = "api/HisBornPosition/Get";
        public const string HIS_BORN_RESULT_GET = "api/HisBornResult/Get";

        public const string HIS_MEDISTOCKDISDO_GET = "api/HisMediStock/GetInStockSdo";
        public const string HIS_MEDISTOCKDISDO_GET1 = "api/HisMediStock/GetDHisMediStock1";
        public const string HIS_PRESCRIPTION_CREATELIST = "/api/HisPrescription/CreateList";

        public const string HIS_HTU_GET = "/api/HisHtu/Get";
        public const string HIS_SERVICE_PATY_PRPO__GETVIEW = "/api/HisServicePatyPrPo/GetView";
        public const string HIS_ACIN_INTERACTIVE_GET = "/api/HisAcinInteractive/Get";
        public const string HIS_ACIN_INTERACTIVE_GETVIEW = "/api/HisAcinInteractive/GetView";

        public const string HIS_MEDI_STOCK_MATY_GET = "/api/HisMediStockMaty/Get";
        public const string HIS_MEDI_STOCK_MATY_GETVIEW = "/api/HisMediStockMaty/GetView";

        public const string HIS_MEDI_STOCK_METY_GET = "/api/HisMediStockMety/Get";
        public const string HIS_MEDI_STOCK_METY_GETVIEW = "/api/HisMediStockMety/GetView";

        public const string HIS_EKIP_USER_GETVIEW = "api/HisEkipUser/GetView";
        public const string HIS_SERE_SERV_GETVIEW_6 = "api/HisSereServ/GetView6";
        public const string HIS_SERVICE_REQ_GETVIEW_1 = "api/HisServiceReq/GetView1";
        public const string HIS_SERE_SERV_REHA_CREATE_SINGLE = "HisSereServReha/CreateSingle";

        public const string HIS_MEST_PERIOD_METY_GETVIEW = "/api/HisMestPeriodMety/GetView";
        public const string HIS_MEST_PERIOD_MATY_GETVIEW = "/api/HisMestPeriodMaty/GetView";
        public const string HIS_MEST_STOCK_PERIOD_CREATE = "/api/HisMediStockPeriod/Create";
        public const string HIS_CHMS_IMP_MEST_CREATE = "/api/HisChmsImpMest/Create";
        public const string HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE = "/api/HisMedicine/GetInStockMedicineWithTypeTree";
        public const string HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE = "/api/HisMaterial/GetInStockMaterialWithTypeTree";
        public const string HIS_IMP_MEST_MEDICINE_GETVIEW_WITH_IN_STOCK_AMOUNT = "/api/HisImpMestMedicine/GetViewWithInStockAmount";
        public const string HIS_IMP_MEST_MATERIAL_GETVIEW_WITH_IN_STOCK_AMOUNT = "/api/HisImpMestMaterial/GetViewWithInStockAmount";
        public const string HIS_MANU_EXP_MEST_CREATE = "/api/HisManuExpMest/Create";
        public const string HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE_BY_EXPIRED_DATE = "/api/HisMedicine/GetInStockMedicineWithTypeTreeOrderByExpiredDate";
        public const string HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE_EXPRIRED_DATE = "/api/HisMaterial/GetInStockMaterialWithTypeTreeOrderByExpiredDate";
        public const string HIS_BLOOD_TYPE_GETVIEW_BY_IN_STOCK = "/api/HisBloodType/GetInStockBloodType";

        public const string HIS_REGISTER_GATE_GET = "/api/HisRegisterGate/Get";

        public const string HIS_REGISTER_REQ_CREATE = "/api/HisRegisterReq/Create";

        public const string HIS_CARD_GET_VIEW_BY_CODE = "/api/HisCard/GetViewByCode";

        public const string HIS_SERE_SERV_GETVIEW_7 = "api/HisSereServ/GetView7";

        public const string HIS_SERVICE_REQ_GET_GROUP_BY_DATE = "api/HisServiceReq/GetGroupByDate";

        public const string HIS_EXP_MEST_OTHER_GETVIEW = "/api/HisExpMestOther/GetView";

        public const string HIS_SERVICE_UNIT_GET_RAW = "api/HisServiceUnit/Get";

        public const string HIS_TRAN_PATI_CREATE = "api/HisTranPati/Create";

        public const string HIS_DATA_STORE_GETVIEW = "api/HisDataStore/GetView";

        public const string HIS_MEDI_RECORD_GETVIEW = "api/HisMediRecord/GetView";

        public const string HIS_MEDI_RECORD_UPDATE_DATA_STORE_ID = "api/HisMediRecord/UpdateDataStoreId";

        public const string HIS_MEDI_RECORD_UPDATE_LIST_DATA_STORE_ID = "api/HisMediRecord/UpdateListDataStoreId";

        public const string HIS_TEST_SERVICE_REQ_RESEND = "/api/HisTestServiceReq/ResendOrderToRoche";

        public const string HIS_TREATMENT_GET_VIEW_5 = "/api/HisTreatment/GetView5";

        public const string HIS_SERVICE_REQ_GETVIEW_2 = "api/HisServiceReq/GetView2";

        public const string HIS_AGGR_EXP_MEST_CREATE_SDO = "api/HisAggrExpMest/CreateSdo";

        public const string HIS_BHYT_BLACKLIST_GET = "api/HisBhytBlacklist/Get";

        public const string HIS_BHYT_WHITELIST_GET = "api/HisBhytWhitelist/Get";

        public const string HIS_SERVICE_REQ_GET_D_HIS_SERVICE_REQ_2 = "api/HisServiceReq/GetDHisServiceReq2";//

        public const string HIS_MEST_METY_DEPA_GET = "api/HisMestMetyDepa/Get";
        public const string HIS_MEST_METY_DEPA_GETVIEW = "api/HisMestMetyDepa/GetView";
        public const string HIS_ICD_CM_GET = "api/HisIcdCm/Get";

        public const string HIS_MEDICINE_GROUP_GET = "api/HisMedicineGroup/Get";

        public const string HIS_USER_ACCOUNT_BOOK_GET = "api/HisUserAccountBook/Get";
        public const string HIS_CARO_ACCOUNT_BOOK_GET = "api/HisCaroAccountBook/Get";
        public const string HIS_SPECIALIST_EXAM_CREATE = "api/HisSpecialistExam/Create";
    }
}
