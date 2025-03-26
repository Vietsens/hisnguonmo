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

namespace HIS.UC.FormType
{
    public class RequestUriStore
    {

        public const string SAR_FORM_TYPE_GET = "api/SarFormType/Get";
        public const string HIS_SERVICE_UNIT_GET = "api/HisServiceUnit/Get";
        public const string HIS_FUND_GET = "api/HisFund/Get";
        public const string HIS_BLOOD_TYPE_GET = "api/HisBloodType/Get";
        public const string HIS_MATERIAL_GET = "api/HisMaterial/Get";
        public const string HIS_MEDICINE_GET = "api/HisMedicine/Get";
        public const string HIS_SUPPLIER_GET = "api/HisSupplier/Get";
        public const string HIS_INVOICE_GET = "api/HisInvoice/Get";
        public const string HIS_INVOICE_BOOK_GET = "api/HisInvoiceBook/Get";
        public const string HIS_IMP_SOURCE_GET = "api/HisImpSource/Get";
        public const string HIS_BRANCH_GET = "api/HisBranch/Get";
        public const string V_HIS_IMP_MEST_MEDICINE_GETVIEW = "api/HisImpMestMedicine/GetView";
        public const string V_HIS_IMP_MEST_MATERIAL_GETVIEW = "api/HisImpMestMaterial/GetView";
        public const string V_HIS_MEDI_STOCK_PERIOD_GETVIEW = "api/HisMediStockPeriod/GetView";
        public const string V_HIS_TREATMENT_GETVIEW = "api/HisTreatment/Get";
        public const string V_HIS_MEDICINE_TYPE_GETVIEW = "api/HisMedicineType/GetView";
        public const string HIS_MEDICINE_BEAN_GETVIEW = "api/HisMedicineBean/GetView";
        public const string HIS_MATERIAL_BEAN_GETVIEW = "api/HisMaterialBean/GetView";
        public const string HIS_MEDICINE_USE_FORM_GET = "api/HisMedicineUseForm/Get";
        public const string HIS_AREA_GET = "api/HisArea/Get";
        public const string HIS_MEDICINE_GROUP_GET = "api/HisMedicineGroup/Get";
        public const string PAC_IMAGE_GET = "api/PacImage/Get";
        public const string PAC_IMAGE_UPDATE = "api/PacImage/Update";
        public const string PAC_IMAGE_RESULT = "api/PacImage/Result";
        public const string PAC_IMAGE_UNRESULT = "api/PacImage/UnResult";

        public const string HIS_SERVICE_PACKAGE_GETVIEW = "/api/HisServicePackage/GetView";
        public const string HIS_ACCOUNT_BOOK_GET = "api/HisAccountBook/Get";
        public const string HIS_ACCOUNT_BOOK_GETVIEW = "api/HisAccountBook/GetView";
        public const string HIS_ACCOUNT_BOOK_CREATE = "api/HisAccountBook/Create";
        public const string HIS_ACCOUNT_BOOK_UPDATE = "api/HisAccountBook/Update";
        public const string HIS_ACCOUNT_BOOK_DELETE = "api/HisAccountBook/Delete";
        public const string HIS_ACCOUNT_BOOK_CHANGE_LOG = "api/HisAccountBook/ChangeLock";
        public const string HIS_PTTT_GROUP_GET = "api/HisPtttGroup/Get";

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
        public const string HIS_ROOM_GET = "api/HisRoom/Get";

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
        public const string HIS_TREATMENT_UPDATE_PATIENT = "/api/HisTreatment/UpdatePatient";

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

        public const string HIS_CASHIER_ROOM_GET = "api/HisCashierRoom/Get";
        public const string HIS_CASHIER_ROOM_GETVIEW = "api/HisCashierRoom/GetView";

        public const string HIS_MEDICINE_TYPE_ACIN_GET = "api/HisMedicineTypeAcin/Get";
        public const string HIS_MEDICINE_TYPE_ACIN_GETVIEW = "api/HisMedicineTypeAcin/GetView";
        public const string HIS_PATIENT_TYPE_ALLOW_GET = "api/HisPatientTypeAllow/Get";
        public const string HIS_PATIENT_TYPE_ALTER_GET_APPLIED = "/api/HisPatientTypeAlter/GetApplied";
        public const string HIS_PATIENT_TYPE_ALTER_GET_VIEW = "/api/HisPatientTypeAlter/GetView";
        public const string HIS_PATIENT_TYPE_ALTER_GET_LAST_BY_TREATMENTID = "/api/HisPatientTypeAlter/GetLastByTreatmentId";

        public const string HIS_PATIENT_GET = "api/HisPatient/Get";
        public const string HIS_PATIENT_GETVIEW = "api/HisPatient/GetView";

        public const string HIS_CARD_GETVIEWBYSERVICECODE = "api/HisCard/GetViewByCode";

        public const string HIS_PATIENT_UPDATE = "api/HisPatient/Update";
        public const string HIS_PATIENT_UPDAT = "/api/HisPatient/Update";

        public const string HIS_HIS_TEST_INDEX_RANGE_GETVIEW = "api/HisTestIndexRange/GetView";

        public const string HIS_ICD_GET = "api/HisIcd/Get";
        public const string HIS_PTTT_METHOD = "api/HisPtttMethod/Get";
        public const string HIS_BLOOD = "api/HisBlood/Get";
        public const string HIS_BLOOD_RH = "api/HisBloodRh/Get";
        public const string HIS_EXECUTE_ROLE = "api/HisExecuteRole/Get";
        public const string HIS_EMOTIONLESS_METHOD = "api/HisEmotionlessMethod/Get";
        public const string HIS_PTTT_CONDITION = "api/HisPtttCondition/Get";
        public const string HIS_PTTT_CATASTROPHE = "api/HisPtttCatastrophe/Get";
        public const string HIS_PRICE_POLICY = "api/HisPricePolicy/Get";
        public const string HIS_PTTT_GROUP = "api/HisPtttGroup/Get";

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
        public const string HIS_PATIENT_REGISTER_PROFILE = "api/HisPatient/RegisterProfile";
        public const string HIS_EXAM_SERVICE_REQ_GETVIEW = "api/HisExamServiceReq/GetView";

        public const string HIS_PRESCRIPTION_GETVIEW = "api/HisPrescription/GetView";
        public const string HIS_PRESCRIPTION_CREATE = "/api/HisPrescription/Create";
        public const string HIS_CHRM_EXP_MEST_CREATE = "/api/HisChmsExpMest/Create";
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

        public const string HIS_SERV_SEGR_GETVIEW = "api/HisServSegr/GetView";
        public const string HIS_SERE_SERV_GETVIEW2 = "api/HisSereServ/GetView2";
        public const string HIS_SERE_SERV_GETVIEW = "api/HisSereServ/GetView";
        public const string HIS_SERVICE_REQ_START = "api/HisServiceReq/Start";
        public const string HIS_SERVICE_REQ_FINISH = "/api/HisServiceReq/Finish";
        public const string HIS_SERVICE_REQ_UPDATE = "/api/HisServiceReq/UpdateSereServ";
        public const string HIS_SERVICE_REQ_CREATE_COMBO = "api/HisServiceReq/CreateCombo";

        public const string HIS_SERVICE_REQ_UNSTART = "api/HisServiceReq/UnStart";

        public const string HIS_SERE_SERV_UPDATE = "/api/HisSereServ/UpdatePatientTypeInfo";

        public const string HIS_SERE_SERV_UPDATE_PATENT_TYPE_INFO_LIST = "/api/HisSereServ/UpdatePatientTypeInfoList";

        public const string HIS_TRAN_PATI_TYPE_GET = "api/HisTranPatiType/Get";


        public const string HIS_TRAN_PATI_FORM_GET = "api/HisTranPatiForm/Get";
        public const string HIS_PAY_FORM_GET = "api/HisPayForm/Get";

        public const string HIS_TRAN_PATI_REASON_GET = "api/HisTranPatiReason/Get";
        public const string HIS_EXP_MEST_REASON_GET = "api/HisExpMestReason/Get";

        public const string HIS_TREATMENT_END_TYPE_GET = "api/HisTreatmentEndType/Get";

        public const string HIS_TREATMENT_RESULT_GET = "api/HisTreatmentResult/Get";

        public const string HIS_DISEASE_RELATION_GET = "api/HisDiseaseRelation/Get";

        public const string HIS_TREATMENT_LOG_TYPE_GET = "api/HisTreatmentLogType/Get";
        public const string HIS_TREATMENT_TYPE_GET = "api/HisTreatmentType/Get";
        public const string HIS_TREATMENT_LOG_GET = "/api/HisTreatmentLog/GetCombo";

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

        public const string HIS_REHA_TRAIN_TYPE_GETVIEW = "api/HisRehaTrainType/GetView";

        public const string HIS_REHA_TRAIN_GETVIEW = "api/HisRehaTrain/GetView";

        public const string HIS_REHA_TRAIN__CREATE = "api/HisRehaTrain/CreateList";

        public const string HIS_ENDO_SERVICE_REQ_GETVIEW = "/api/HisEndoServiceReq/GetView";

        public const string HIS_FUEX_SERVICE_REQ_GETVIEW = "/api/HisFuexServiceReq/GetView";

        public const string HIS_SUMI_SERVICE_REQ_GETVIEW = "/api/HisSuimServiceReq/GetView";

        public const string HIS_SERE_SERV_FILE_GET = "/api/HisSereServFile/Get";

        public const string HIS_OTHER_SERVICE_REQ_GETVIEW = "/api/HisOtherServiceReq/GetView";

        public const string HIS_SERVICE_REQ_CHANGEROOM = "/api/HisServiceReq/ChangeRoom";

        public const string HIS_SURG_SERVICE_REQ_UPDATE = "/api/HisSurgServiceReq/Update";

        public const string HIS_EMERGENCY_TIME_GET = "/api/HisEmergencyWTime/Get";
        public const string HIS_EMERGENCY_GETVIEW = "/api/HisEmergency/GetView";
        //MRS
        public const string MRS_REPORT_CREATE = "/api/MrsReport/Create";

        //SAR
        public const string SAR_REPORT_CREATE = "api/SarReport/Create";
        public const string SAR_REPORT_TYPE_GET = "api/SarReportType/Get";
        public const string SAR_REPORT_TEMPLATE_GET = "api/SarReportTemplate/Get";
        public const string SAR_REPORT_STT_GET = "api/SarReportStt/Get";
        public const string SAR_PRINT_TYPE_GET = "api/SarPrintType/Get";
        public const string SAR_PRINT_GET = "api/SarPrint/Get";
        public const string SAR_PRINT_CREATE = "api/SarPrint/Create";
        public const string SAR_PRINT_UPDATE = "api/SarPrint/Update";
        public const string SAR_FORM_FIELD_GET = "api/SarFormField/Get";
        public const string SAR_RETY_FOFI_GET = "api/SarRetyFofi/Get";
        public const string SAR_RETY_FOFI_GETVIEW = "api/SarRetyFofi/GetView";

        //SDA
        public const string SDA_ETHNIC_GET = "api/SdaEthnic/Get";
        public const string SDA_GROUP_GET = "api/SdaGroup/Get";
        public const string SDA_PROVINCE_GETVIEW = "api/SdaProvince/GetView";
        public const string SDA_DISTRICT_GETVIEW = "api/SdaDistrict/GetView";
        public const string SDA_COMMUNE_GETVIEW = "api/SdaCommune/GetView";
        public const string SDA_NATIONAL_GET = "api/SdaNational/Get";

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

        public const string HIS_DEPOSIT_REQ_CREATE = "api/HisDepositReq/Create";
        public const string HIS_DEPOSIT_REQ_UPDATE = "api/HisDepositReq/Update";
        public const string HIS_DEPOSIT_REQ_GETVIEW = "api/HisDepositReq/GetView";
        public const string HIS_DEPOSIT_REQ_DELETE = "api/HisDepositReq/Delete";
        public const string ACS_USER_GET = "api/AcsUser/Get";

        public const string HIS_DEBATE_USER_GET = "api/HisDebateUser/Get";
        public const string HIS_DEBATE_CREATE = "/api/HisDebate/Create";
        public const string HIS_DEBATE_UPDATE = "/api/HisDebate/Update";
        public const string HIS_DEBATE_DELETE = "/api/HisDebate/Delete";

        public const string HIS_MEST_ROOM_GETVIEW = "api/HisMestRoom/GetView";

        public const string HIS_MEST_PATIENT_TYPE_GET = "api/HisMestPatientType/Get";

        public const string HIS_EXP_MEST_MEDICINE = "api/HisExpMestMedicine/GetView";

        public const string HIS_METERIAL_TYPE_GET_IN_STOCK_MATERIAL_TYPE = " api/HisMaterialType/GetInStockMaterialType";

        public const string HIS_METERIAL_TYPE_GET = "api/HisMedicineType/GetView";

        public const string HIS_SERVICE_REQ_DELETE = "/api/HisServiceReq/Delete";

        public const string HIS_EXP_MEST_METY_GETVIEW = "/api/HisExpMestMety/GetView";

        public const string HIS_EXP_MEST_OTHER_GETVIEW = "/api/HisExpMestOther/GetView";

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
        public const string HIS_TREATMENT_SDO = "/api/HisTreatment/GetSdoById";
        public const string HIS_TREATMENT_BEDROOM_CREATE = "api/HisTreatmentBedRoom/Create";
        public const string HIS_TREATMENT_BEDROOM_CREATE_SDO = "api/HisTreatmentBedRoom/CreateSdo";

        public const string HIS_SERVICE_UNIT_GET_RAW = "api/HisServiceUnit/Get";

        public const string HIS_TRAN_PATI_UPDATE = "api/HisTranPati/Update";
        public const string HIS_TRAN_PATI_DELETE = "api/HisTranPati/Delete";
        public const string HIS_TRAN_PATI_GETVIEW = "api/HisTranPati/GetView";

        public const string HIS_MATERIAL_GETVIEW = "/api/HisMaterial/GetView";
        public const string HIS_MATERIAL_GETVIEW2 = "/api/HisMaterial/GetView2";
        public const string HIS_MEDICINE_GETVIEW2 = "/api/HisMedicine/GetView2";
        public const string HIS_MATERIAL_GETVIEW1 = "/api/HisMaterial/GetView1";
        public const string HIS_MEDICINE_GETVIEW1 = "/api/HisMedicine/GetView1";

        public const string HIS_MEDICINE_GETVIEW = "/api/HisMedicine/GetView";

        public const string HIS_TREATMENT_OUT_GETVIEW = "api/HisTreatmentOut/GetView";

        public const string HIS_PATIENT_TYPE_ALTER_BHYT_GETVIEW = "api/HisTypeAlterBhyt/GetView";

        public const string HIS_SERE_SERV_PTTT_GETVIEW = "api/HisSereServPttt/GetView";

        public const string HIS_SERE_SERV_PTTT_GET = "api/HisSereServPttt/Get";

        public const string HIS_SERE_SERV_PTUS_GETVIEW = "api/HisSereServPtus/GetView";

        public const string HIS_MILITARY_RANK_GET = "api/HisMilitaryRank/Get";

        public const string HIS_WORK_PLACE_GET = "api/HisWorkPlace/Get";

        public const string HIS_WORK_PLACE_CREATE = "api/HisWorkPlace/Create";

        public const string TOS_CONNECTOR_CONNECT_TERMINAL = "api/TosConnector/ConnectTerminal";

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

        public const string HIS_ICD_GROUP = "/api/HisIcdGroup/Get";
        public const string HIS_PROGRAM_GET = "/api/HisProgram/Get";
        public const string HIS_PATIEN_PROGRAM_GET_VIEW = "/api/HisPatientProgram/GetView";
        public const string HIS_PATIEN_PROGRAM_DELETE = "/api/HisPatientProgram/Delete";
        public const string HIS_PATIEN_PROGRAM_CREATE = "/api/HisPatientProgram/Create";
        public const string HIS_PATIEN_PROGRAM_UPDATE = "/api/HisPatientProgram/Update";
        public const string HIS_PATIEN_PROGRAM_GET = "/api/HisPatientProgram/GetViewByCode";

        public const string HIS_ACCIDENT_BODY_PART_GET = "/api/HisAccidentBodyPart/Get";
        public const string HIS_ACCIDENT_CARE_GET = "/api/HisAccidentCare/Get";
        public const string HIS_ACCIDENT_HELMET_GET = "/api/HisAccidentHelmet/Get";
        public const string HIS_ACCIDENT_HURT_TYPE_GET = "/api/HisAccidentHurtType/Get";
        public const string HIS_ACCIDENT_LOCATION_GET = "/api/HisAccidentLocation/Get";
        public const string HIS_ACCIDENT_POISON_GET = "/api/HisAccidentPoison/Get";
        public const string HIS_ACCIDENT_RESULT_GET = "/api/HisAccidentResult/Get";
        public const string HIS_ACCIDENT_VEHICLE_GET = "/api/HisAccidentVehicle/Get";
        public const string HIS_ACCIDENT_HURT_CREATE = "/api/HisAccidentHurt/Create";

        public const string HIS_APPOINTMENT_GETV = "/api/HisAppointment/GetViewByCode";
        public const string HIS_APPOINTMENT_GET = "/api/HisAppointment/Get";

        public const string HIS_DATA_STORE_GETVIEW = "api/HisDataStore/GetView";

        public const string HIS_MEDI_RECORD_GETVIEW = "api/HisMediRecord/GetView";
        public const string HIS_MEDI_RECORD_UPDATE_STORE_ID = "api/HisMediRecord/UpdateDataStoreId";
        public const string HIS_SERVICE_RETY_CAT_GETVIEW = "api/HisServiceRetyCat/Getview";
        public const string HIS_BID = "api/HisBid/Get";
        public const string HIS_REPORT_TYPE_CAT_GET = "/api/HisReportTypeCat/Get";


        public const string THE_BRANCH_GET = "/api/TheBranch/Get";

        public static string MRS_REPORT_GETINPUT = "/api/MrsReport/GetInput";
        public const string AOS_ACCOUNT_GET = "/api/AosAccount/Get";

        public static string MRS_REPORT_REFRESH = "/api/MrsReport/Refresh";
    }
}
