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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    public class FilterConfig
    {
        const string StrOutput0 = "_OUTPUT0:";

        const string StrOutputLimitCode = "_LIMIT_CODE";

        public static string HisFilterTypes(string _jsonOutput)
        {
            string FilterTypeCode = null;

            try
            {

                if (_jsonOutput != null && !IsCodeField(_jsonOutput.Replace(StrOutputLimitCode, "")))
                {
                    if (_jsonOutput.Contains("CONFIG_KEY\""))
                    {
                        if (_jsonOutput.Contains("MEDICINE_TYPE_ID") && _jsonOutput.Contains("\"CONCENTRA\""))////)
                            FilterTypeCode = "HIS_CONCENTRA_MEDICINE_TYPE";
                        else if (_jsonOutput.Contains("DEPARTMENT_ID") && _jsonOutput.Contains("\"DEACTIVE\""))////)
                            FilterTypeCode = "HIS_DEACTIVE_DEPARTMENT";
                        else if (_jsonOutput.Contains("SERVICE_ID") && _jsonOutput.Contains("\"DEACTIVE\""))////)
                            FilterTypeCode = "HIS_DEACTIVE_SERVICE";
                        else if (_jsonOutput.Contains("\"THIS_ROLE\""))
                        {
                            if (_jsonOutput.Contains("DEPARTMENT_ID"))
                            {
                                FilterTypeCode = "HIS_THIS_ROLE_DEPARTMENT";
                            }
                            if (_jsonOutput.Contains("ROOM_ID")&&!_jsonOutput.StartsWith("\"EXACT_"))
                            {
                                FilterTypeCode = "HIS_THIS_ROLE_ROOM";
                            }
                            if (_jsonOutput.Contains("MEDI_STOCK_ID"))
                            {
                                FilterTypeCode = "HIS_THIS_ROLE_MEDI_STOCK";
                            }
                        }
                    }
                    else if (_jsonOutput.StartsWith("\"CURRENTBRANCH_"))
                    {
                        if (_jsonOutput.Contains("DEPARTMENT_ID"))
                            FilterTypeCode = "HIS_CURRENTBRANCH_DEPARTMENT";
                        else if (_jsonOutput.Contains("ROOM_ID"))
                            FilterTypeCode = "HIS_CURRENTBRANCH_ROOM";
                        else if (_jsonOutput.Contains("MEDI_STOCK_ID"))
                            FilterTypeCode = "HIS_CURRENTBRANCH_MEDI_STOCK";
                    }
                    else if (_jsonOutput.StartsWith("\"EXACT_"))
                    {
                        if (_jsonOutput.Contains("BRANCH_ROOM_ID"))
                            FilterTypeCode = "HIS_BRANCH_ROOM";
                        if (_jsonOutput.Contains("CASHIER_ROOM_ID"))
                            FilterTypeCode = "HIS_CASHIER_ROOM";
                        else if (_jsonOutput.Contains("EXT_CASHIER_ROOM_ID"))
                            FilterTypeCode = "HIS_EXT_CASHIER_ROOM";
                        else if (_jsonOutput.Contains("PARENT_SERVICE_ID"))
                            FilterTypeCode = "HIS_PARENT_SERVICE";
                        else if (_jsonOutput.Contains("PARENT_SERVICE_RAW_MEDICINAL_HERBS_ID"))/// 24/02/2025
                            FilterTypeCode = "HIS_PARENT_SERVICE_RAW_MEDICINAL_HERBS";
                        else if (_jsonOutput.Contains("PARENT_MEDICINE_TYPE_ID"))/// 22/04/2025
                            FilterTypeCode = "HIS_PARENT_MEDICINE_TYPE";
                        else if (_jsonOutput.Contains("CHILD_SERVICE_ID"))
                            FilterTypeCode = "HIS_CHILD_SERVICE";
                        else if (_jsonOutput.Contains("MEST_ROOM_ID"))
                            FilterTypeCode = "HIS_MEST_ROOM";
                        else if (_jsonOutput.Contains("SAMPLE_ROOM_ID"))
                            FilterTypeCode = "HIS_SAMPLE_ROOM";
                        else if (_jsonOutput.Contains("RECEPTION_ROOM_ID"))
                            FilterTypeCode = "HIS_RECEPTION_ROOM";
                        else if (_jsonOutput.Contains("BED_ROOM_ID"))
                            FilterTypeCode = "HIS_BED_ROOM";
                        else if (_jsonOutput.Contains("EXECUTE_ROOM_ID"))
                            FilterTypeCode = "HIS_EXECUTE_ROOM";
                        else if (_jsonOutput.Contains("TREATMENT_BED_ROOM_ID"))
                            FilterTypeCode = "HIS_TREATMENT_BED_ROOM";
                        else if (_jsonOutput.Contains("SERVICE_ROOM_ID"))
                            FilterTypeCode = "HIS_SERVICE_ROOM";
                        else if (_jsonOutput.Contains("STORE_MEDI_STOCK_ID"))
                            FilterTypeCode = "HIS_STORE_MEDI_STOCK";
                    }
                    else if (_jsonOutput.Contains("ALLOUT_MEDI_SERVICE_ID"))
                        FilterTypeCode = "HIS_ALLOUT_MEDI_SERVICE";
                    else if (_jsonOutput.Contains("NONE_MEDI_SERVICE_ID"))
                        FilterTypeCode = "HIS_NONE_MEDI_SERVICE";
                    else if (_jsonOutput.Contains("DATA_STORE_ID"))
                        FilterTypeCode = "HIS_DATA_STORE";
                    else if (_jsonOutput.Contains("OTHER_PAY_SOURCE_ID"))
                        FilterTypeCode = "HIS_OTHER_PAY_SOURCE";
                    else if (_jsonOutput.Contains("BID_TYPE_ID"))
                        FilterTypeCode = "HIS_BID_TYPE";
                    else if (_jsonOutput.Contains("PATIENT_CLASSIFY_ID"))
                        FilterTypeCode = "HIS_PATIENT_CLASSIFY";
                    else if (_jsonOutput.Contains("PATY_AREA_ID"))
                        FilterTypeCode = "HIS_PATY_AREA";
                    else if (_jsonOutput.Contains("DEBATE_TYPE_ID"))
                        FilterTypeCode = "HIS_DEBATE_TYPE";
                    else if (_jsonOutput.Contains("DEPA_AREA_ID"))
                        FilterTypeCode = "HIS_DEPA_AREA";
                    else if (_jsonOutput.Contains("AREA_ID"))
                        FilterTypeCode = "HIS_AREA";
                    else if (_jsonOutput.Contains("ACCIDENT_RESULT_ID"))
                        FilterTypeCode = "HIS_ACCIDENT_RESULT";
                    else if (_jsonOutput.Contains("SERVICE_UNIT_ID"))
                        FilterTypeCode = "HIS_SERVICE_UNIT";
                    else if (_jsonOutput.Contains("MEDICINE_USE_FORM_ID"))
                        FilterTypeCode = "HIS_MEDICINE_USE_FORM";
                    else if (_jsonOutput.Contains("MEDICINE_GROUP_ID"))
                        FilterTypeCode = "HIS_MEDICINE_GROUP";
                    else if (_jsonOutput.Contains("ACCIDENT_HURT_TYPE_ID"))
                        FilterTypeCode = "HIS_ACCIDENT_HURT_TYPE";
                    else if (_jsonOutput.Contains("WORKING_SHIFT_ID"))
                        FilterTypeCode = "HIS_WORKING_SHIFT";
                    else if (_jsonOutput.Contains("EMPLOYEE_ID"))
                        FilterTypeCode = "HIS_EMPLOYEE";
                    else if (_jsonOutput.Contains("MEDICINE_ID"))
                        FilterTypeCode = "HIS_MEDICINE";
                    else if (_jsonOutput.Contains("PAY_FORM_ID"))
                        FilterTypeCode = "HIS_PAY_FORM";
                    else if (_jsonOutput.Contains("FUND_ID"))
                        FilterTypeCode = "HIS_FUND";
                    else if (_jsonOutput.Contains("MATERIAL_ID"))
                        FilterTypeCode = "HIS_MATERIAL";
                    else if (_jsonOutput.Contains("BLOOD_TYPE_ID"))
                        FilterTypeCode = "HIS_BLOOD_TYPE";
                    else if (_jsonOutput.Contains("MEDICINE_LINE_ID"))
                        FilterTypeCode = "HIS_MEDICINE_LINE";
                    else if (_jsonOutput.Contains("CASHIER_LOGINNAME"))
                        FilterTypeCode = "HIS_CASHIER_LOGINNAME";
                    //else if (JSONOUTPUT.Contains("BED_ROOM_ID"))
                    //    FilterTypeCode = "HIS_BED_ROOM";
                    else if (_jsonOutput.Contains("MEDI_SUPPLIER_STOCK_ID"))
                        FilterTypeCode = "HIS_MEDI_SUPPLIER_STOCK";
                    else if (_jsonOutput.Contains("MEDI_BIG_STOCK_ID"))
                        FilterTypeCode = "HIS_MEDI_BIG_STOCK";
                    else if (_jsonOutput.Contains("MEDI_STOCK_NOT_CABIN_ID"))
                        FilterTypeCode = "HIS_MEDI_BIG_STOCK_NOT_CABIN";
                    else if (_jsonOutput.Contains("REQUEST_ROOM_ID"))////)
                        FilterTypeCode = "HIS_REQUEST_ROOM";
                    else if (_jsonOutput.Contains("MY_SURG_ROOM_ID"))////)
                        FilterTypeCode = "HIS_MY_SURG_ROOM";
                    else if (_jsonOutput.Contains("EXAM_ROOM_ID"))////)
                        FilterTypeCode = "HIS_EXAM_ROOM";
                    else if (_jsonOutput.Contains("CLINICAL_ROOM_ID"))////)
                        FilterTypeCode = "HIS_CLINICAL_ROOM";
                    else if (_jsonOutput.Contains("SURG_ROOM_ID"))////)
                        FilterTypeCode = "HIS_SURG_ROOM";
                    else if (_jsonOutput.Contains("MEDICINE_TYPE_ID"))////)
                        FilterTypeCode = "HIS_MEDICINE_TYPE";
                    else if (_jsonOutput.Contains("MATERIAL_TYPE_ID"))////)
                        FilterTypeCode = "HIS_MATERIAL_TYPE";
                    else if (_jsonOutput.Contains("SUPPLIER_ID"))////)
                        FilterTypeCode = "HIS_SUPPLIER";
                    else if (_jsonOutput.Contains("INVOICE_ID"))////)
                        FilterTypeCode = "HIS_INVOICE";
                    else if (_jsonOutput.Contains("INVOICE_BOOK_ID"))////)
                        FilterTypeCode = "HIS_INVOICE_BOOK";
                    else if (_jsonOutput.Contains("IMP_SOURCE_ID"))////)
                        FilterTypeCode = "HIS_IMP_SOURCE";
                    else if (_jsonOutput.Contains("MEDI_STOCK_NOT_BUSINESS_ID"))////)
                        FilterTypeCode = "HIS_MEDI_STOCK_NOT_BUSINESS";
                    else if (_jsonOutput.Contains("MEDI_STOCK_BUSINESS_ID"))////)
                        FilterTypeCode = "HIS_MEDI_STOCK_BUSINESS";
                    else if (_jsonOutput.Contains("MY_LOGINNAME"))////)
                        FilterTypeCode = "ACS_MY_LOGINNAME";
                    else if (_jsonOutput.Contains("LOGINNAME_SALE"))////)
                        FilterTypeCode = "ACS_USER_SALE";
                    else if (_jsonOutput.Contains("LOGINNAME_DOCTOR"))
                        FilterTypeCode = "ACS_USER_DOCTOR";
                    else if (_jsonOutput.Contains("DOCTOR_LOGINNAME"))
                        FilterTypeCode = "ACS_USER_DOCTOR";
                    else if (_jsonOutput.Contains("SERVICE_GROUP_ID"))////)
                        FilterTypeCode = "HIS_SERVICE_GROUP";
                    else if (_jsonOutput.Contains("CAREER_ID"))////)
                        FilterTypeCode = "HIS_CAREER";
                    else if (_jsonOutput.Contains("KSK_CONTRACT_ID"))////)
                        FilterTypeCode = "HIS_KSK_CONTRACT";
                    else if (_jsonOutput.Contains("BRANCH_ID"))////)
                        FilterTypeCode = "HIS_BRANCH";
                    else if (_jsonOutput.Contains("EXAM_SERVICE_TYPE_ID"))////)
                        FilterTypeCode = "HIS_EXAM_SERVICE_TYPE";
                    else if (_jsonOutput.Contains("CAREER_ID"))////)
                        FilterTypeCode = "HIS_CAREER";
                    else if (_jsonOutput.Contains("DEATH_CAUSE_ID"))////)
                        FilterTypeCode = "HIS_DEATH_CAUSE";
                    else if (_jsonOutput.Contains("PROGRAM_ID"))////)
                        FilterTypeCode = "HIS_PROGRAM";
                    else if (_jsonOutput.Contains("PATIENT_TYPE_ID"))////)
                        FilterTypeCode = "HIS_PATIENT_TYPE";
                    else if (_jsonOutput.Contains("PATIENT_RAW_MEDICINAL_HERBS_TYPE_ID")) /// 24/02/2025
                        FilterTypeCode = "HIS_PATIENT_RAW_MEDICINAL_HERBS_TYPE";
                    else if (_jsonOutput.Contains("IMP_MEST_TYPE_ID"))////)
                        FilterTypeCode = "HIS_IMP_MEST_TYPE";
                    else if (_jsonOutput.Contains("IMP_MEST_STT_ID"))////)
                        FilterTypeCode = "HIS_IMP_MEST_STT";
                    //else if (JSONOUTPUT.Contains("EXECUTE_ROOM_ID"))////)
                    //    FilterTypeCode = "HIS_EXECUTE_ROOM";
                    else if (_jsonOutput.Contains("EXECUTE_DEPARTMENT_ID"))////)
                        FilterTypeCode = "HIS_DEPARTMENT";
                    else if (_jsonOutput.Contains("CLINICAL_DEPARTMENT_ID"))////)
                        FilterTypeCode = "HIS_CLINICAL_DEPARTMENT";
                    else if (_jsonOutput.Contains("MY_DEPARTMENT_ID"))////)
                        FilterTypeCode = "HIS_MY_DEPARTMENT";
                    else if (_jsonOutput.Contains("ROOM_TYPE_ID"))////)
                        FilterTypeCode = "HIS_ROOM_TYPE";
                    else if (_jsonOutput.Contains("EXECUTE_GROUP_ID"))////)
                        FilterTypeCode = "HIS_EXECUTE_GROUP";
                    else if (_jsonOutput.Contains("INFUSION_STT_ID"))////)
                        FilterTypeCode = "HIS_INFUSION_STT";
                    else if (_jsonOutput.Contains("GENDER_ID"))////)
                        FilterTypeCode = "HIS_GENDER";
                    else if (_jsonOutput.Contains("EXP_MEST_STT_ID"))////)
                        FilterTypeCode = "HIS_EXP_MEST_STT";
                    else if (_jsonOutput.Contains("EXP_MEST_TYPE_ID"))////)
                        FilterTypeCode = "HIS_EXP_MEST_TYPE";
                    else if (_jsonOutput.Contains("TREATMENT_RESULT_ID"))////)
                        FilterTypeCode = "HIS_TREATMENT_RESULT";
                    else if (_jsonOutput.Contains("TREATMENT_END_TYPE_ID"))////)
                        FilterTypeCode = "HIS_TREATMENT_END_TYPE";
                    else if (_jsonOutput.Contains("SERVICE_GROUP_ID"))////)
                        FilterTypeCode = "HIS_SERVICE_GROUP";
                    else if (_jsonOutput.Contains("SERVICE_TYPE_ID"))////)
                        FilterTypeCode = "HIS_SERVICE_TYPE";
                    else if (_jsonOutput.Contains("SERVICE_FULL_ID"))////)
                        FilterTypeCode = "HIS_SERVICE_FULL";
                    else if (_jsonOutput.Contains("SERVICE_ID"))////)
                        FilterTypeCode = "HIS_SERVICE";
                    //else if (JSONOUTPUT.Contains("CASHIER_ROOM_ID"))////)
                    //    FilterTypeCode = "HIS_CASHIER_ROOM";
                    else if (_jsonOutput.Contains("TREATMENT_TYPE_ID"))////)
                        FilterTypeCode = "HIS_TREATMENT_TYPE";
                    else if (_jsonOutput.Contains("ACCOUNT_BOOK_ID"))////)
                        FilterTypeCode = "HIS_ACCOUNT_BOOK";
                    else if (_jsonOutput.Contains("ICD_GROUP_ID"))////)
                        FilterTypeCode = "HIS_ICD_GROUP";
                    else if (_jsonOutput.Contains("PTTT_METHOD_ID"))////)
                        FilterTypeCode = "HIS_PTTT_METHOD";
                    else if (_jsonOutput.Contains("PTTT_GROUP_ID"))////)
                        FilterTypeCode = "HIS_PTTT_GROUP";
                    else if (_jsonOutput.Contains("BLOOD_ID"))////)
                        FilterTypeCode = "HIS_BLOOD";
                    else if (_jsonOutput.Contains("BLOOD_RH_ID"))////)
                        FilterTypeCode = "HIS_BLOOD_RH";
                    else if (_jsonOutput.Contains("BID_ID"))////)
                        FilterTypeCode = "HIS_BID";
                    else if (_jsonOutput.Contains("BED_ID"))////)
                        FilterTypeCode = "HIS_BED";
                    else if (_jsonOutput.Contains("EXECUTE_ROLE_ID"))////)
                        FilterTypeCode = "HIS_EXECUTE_ROLE";
                    else if (_jsonOutput.Contains("SERVICE_REQ_TYPE_ID"))////)
                        FilterTypeCode = "HIS_SERVICE_REQ_TYPE";
                    else if (_jsonOutput.Contains("SERVICE_REQ_STT_ID"))////)
                        FilterTypeCode = "HIS_SERVICE_REQ_STT";
                    else if (_jsonOutput.Contains("REPORT_TYPE_CAT_ID"))////)
                        FilterTypeCode = "HIS_REPORT_TYPE_CAT";
                    else if (_jsonOutput.Contains("OTHER_MEDI_STOCK_ID"))////)
                        FilterTypeCode = "OTHER_HIS_MEDI_STOCK";
                    else if (_jsonOutput.Contains("IMP_MEDI_STOCK_ID"))////)
                        FilterTypeCode = "HIS_MEDI_STOCK";
                    else if (_jsonOutput.Contains("MEDI_STOCK_ID"))////)
                        FilterTypeCode = "HIS_MEDI_STOCK";
                    else if (_jsonOutput.Contains("LOGINNAME"))////)
                        FilterTypeCode = "ACS_USER";
                    else if (_jsonOutput.Contains("DEPARTMENT_ID"))////)
                        FilterTypeCode = "HIS_DEPARTMENT";
                    else if (_jsonOutput.Contains("ICD_ID"))////)
                        FilterTypeCode = "HIS_ICD";
                    else if (_jsonOutput.Contains("MEST_ROOM_ID"))
                        FilterTypeCode = "HIS_MEST_ROOM";
                    else if (_jsonOutput.Contains("ROOM_ID"))////)
                        FilterTypeCode = "HIS_ROOM";
                    else if (_jsonOutput.Contains("EXP_MEST_REASON_ID"))
                        FilterTypeCode = "HIS_EXP_MEST_REASON";
                    else if (_jsonOutput.Contains("MEDI_STOCK_PERIOD_ID"))
                        FilterTypeCode = "HIS_MEDI_STOCK_PERIOD";
                    else if (_jsonOutput.Contains("WORK_PLACE_ID"))
                        FilterTypeCode = "HIS_WORK_PLACE";
                    else if (_jsonOutput.Contains("THE_BRANCH__ID") || _jsonOutput.Contains("THE_BRANCH__ID_ADMIN"))
                        FilterTypeCode = "THE_BRANCH";
                    else if (_jsonOutput.Contains("AOS_ACCOUNT_TYPE_ID"))
                        FilterTypeCode = "AOS_ACCOUNT_TYPE";
                    else if (_jsonOutput.Contains("TRANSACTION_TYPE_ID"))
                        FilterTypeCode = "HIS_TRANSACTION_TYPE";
                    else if (_jsonOutput.Contains("MACHINE_ID"))
                        FilterTypeCode = "HIS_MACHINE";
                    else if (_jsonOutput.Contains("MEDI_ORG_ID"))
                        FilterTypeCode = "HIS_MEDI_ORG";
                    else if (_jsonOutput.Contains("AOS_BANK_ACCOUNT_TYPE_ID"))
                        FilterTypeCode = "AOS_BANK_ACCOUNT_TYPE";
                    else if (_jsonOutput.Contains("MEDI_STOCK_CABINET_ID"))
                        FilterTypeCode = "HIS_MEDI_STOCK_CABINET";
                    else if (_jsonOutput.Contains("MEST_ROOM_STOCK_ID"))
                        FilterTypeCode = "HIS_MEST_ROOM_MEDI_STOCK";
                    else if (_jsonOutput.Contains("FORM_TYPE_ID"))
                        FilterTypeCode = "SAR_FORM_TYPE";
                    else if (_jsonOutput.Contains("DISTRICT_ID"))
                        FilterTypeCode = "SDA_DISTRICT";
                    else if (_jsonOutput.Contains("COMMUNE_ID"))
                        FilterTypeCode = "SDA_COMMUNE";
                    else if (_jsonOutput.Contains("PROVINCE_ID"))
                        FilterTypeCode = "SDA_PROVINCE";
                    else if (_jsonOutput.Contains("INPUT_DATA_ID"))
                    {
                        RemoveStrOutput0(ref _jsonOutput);
                        FilterTypeCode = _jsonOutput;
                    }
                    else if (_jsonOutput.Contains("MRSINPUT"))
                    {
                        RemoveStrOutput0(ref _jsonOutput);
                        FilterTypeCode = _jsonOutput;
                    }

                }
                else
                {

                    if (_jsonOutput.Contains("CONFIG_KEY\""))
                    {
                        if (_jsonOutput.Contains("LOGINNAME_DOCTOR") && _jsonOutput.Contains("\"DEPARTMENT\""))////)
                            FilterTypeCode = "ACS_USER_DOCTOR_DEPA";
                        else if (_jsonOutput.Contains("DOCTOR_LOGINNAME") && _jsonOutput.Contains("\"DEPARTMENT\""))////)
                            FilterTypeCode = "ACS_USER_DOCTOR_DEPA";

                    }
                    else if (_jsonOutput.StartsWith("\"CURRENTBRANCH_"))
                    {
                        if (_jsonOutput.Contains("DEPARTMENT_CODE"))
                            FilterTypeCode = "HIS_CURRENTBRANCH_DEPARTMENT";
                        else if (_jsonOutput.Contains("ROOM_CODE"))
                            FilterTypeCode = "HIS_CURRENTBRANCH_ROOM";
                        else if (_jsonOutput.Contains("MEDI_STOCK_CODE"))
                            FilterTypeCode = "HIS_CURRENTBRANCH_MEDI_STOCK";
                    }
                    else if (_jsonOutput.StartsWith("\"EXACT_"))
                    {
                        if (_jsonOutput.Contains("CASHIER_ROOM_CODE"))
                            FilterTypeCode = "HIS_CASHIER_ROOM";
                        else if (_jsonOutput.Contains("EXT_CASHIER_ROOM_CODE"))
                            FilterTypeCode = "HIS_EXT_CASHIER_ROOM";
                        else if (_jsonOutput.Contains("PARENT_SERVICE_CODE"))
                            FilterTypeCode = "HIS_PARENT_SERVICE";
                        else if (_jsonOutput.Contains("PARENT_SERVICE_RAW_MEDICINAL_HERBS_CODE"))// 24/02/2025
                            FilterTypeCode = "HIS_PARENT_SERVICE_RAW_MEDICINAL_HERBS";
                        else if (_jsonOutput.Contains("PARENT_MEDICINE_TYPE_CODE")) /// 22/04/2025
                            FilterTypeCode = "HIS_PARENT_MEDICINE_TYPE";
                        else if (_jsonOutput.Contains("CHILD_SERVICE_CODE"))
                            FilterTypeCode = "HIS_CHILD_SERVICE";
                        else if (_jsonOutput.Contains("MEST_ROOM_CODE"))
                            FilterTypeCode = "HIS_MEST_ROOM";
                        else if (_jsonOutput.Contains("SAMPLE_ROOM_CODE"))
                            FilterTypeCode = "HIS_SAMPLE_ROOM";
                        else if (_jsonOutput.Contains("RECEPTION_ROOM_CODE"))
                            FilterTypeCode = "HIS_RECEPTION_ROOM";
                        else if (_jsonOutput.Contains("BED_ROOM_CODE"))
                            FilterTypeCode = "HIS_BED_ROOM";
                        else if (_jsonOutput.Contains("EXECUTE_ROOM_CODE"))
                            FilterTypeCode = "HIS_EXECUTE_ROOM";
                        else if (_jsonOutput.Contains("TREATMENT_BED_ROOM_CODE"))
                            FilterTypeCode = "HIS_TREATMENT_BED_ROOM";
                        else if (_jsonOutput.Contains("SERVICE_ROOM_CODE"))
                            FilterTypeCode = "HIS_SERVICE_ROOM";
                        else if (_jsonOutput.Contains("QR_BANK_CODE"))
                            FilterTypeCode = "HIS_CONFIG";
                    }
                    else if (_jsonOutput.Contains("QR_BANK_CODE"))
                        FilterTypeCode = "HIS_CONFIG";
                    else if (_jsonOutput.Contains("ACCIDENT_RESULT_CODE"))
                        FilterTypeCode = "HIS_ACCIDENT_RESULT";
                    else if (_jsonOutput.Contains("SERVICE_UNIT_CODE"))
                        FilterTypeCode = "HIS_SERVICE_UNIT";
                    else if (_jsonOutput.Contains("MEDICINE_USE_FORM_CODE"))
                        FilterTypeCode = "HIS_MEDICINE_USE_FORM";
                    else if (_jsonOutput.Contains("ACCIDENT_HURT_TYPE_CODE"))
                        FilterTypeCode = "HIS_ACCIDENT_HURT_TYPE";
                    else if (_jsonOutput.Contains("WORKING_SHIFT_CODE"))
                        FilterTypeCode = "HIS_WORKING_SHIFT";
                    else if (_jsonOutput.Contains("EMPLOYEE_CODE"))
                        FilterTypeCode = "HIS_EMPLOYEE";
                    else if (_jsonOutput.Contains("MEDICINE_CODE"))
                        FilterTypeCode = "HIS_MEDICINE";
                    else if (_jsonOutput.Contains("PAY_FORM_CODE"))
                        FilterTypeCode = "HIS_PAY_FORM";
                    else if (_jsonOutput.Contains("FUND_CODE"))
                        FilterTypeCode = "HIS_FUND";
                    else if (_jsonOutput.Contains("MATERIAL_CODE"))
                        FilterTypeCode = "HIS_MATERIAL";
                    else if (_jsonOutput.Contains("BLOOD_TYPE_CODE"))
                        FilterTypeCode = "HIS_BLOOD_TYPE";
                    else if (_jsonOutput.Contains("CASHIER_LOGINNAME"))
                        FilterTypeCode = "HIS_CASHIER_LOGINNAME";
                    //else if (JSONOUTPUT.Contains("BED_ROOM_CODE"))
                    //    FilterTypeCode = "HIS_BED_ROOM";
                    else if (_jsonOutput.Contains("MEDI_SUPPLIER_STOCK_CODE"))
                        FilterTypeCode = "HIS_MEDI_SUPPLIER_STOCK";
                    else if (_jsonOutput.Contains("MEDI_BIG_STOCK_CODE"))
                        FilterTypeCode = "HIS_MEDI_BIG_STOCK";
                    else if (_jsonOutput.Contains("REQUEST_ROOM_CODE"))////)
                        FilterTypeCode = "HIS_REQUEST_ROOM";
                    else if (_jsonOutput.Contains("MY_SURG_ROOM_CODE"))////)
                        FilterTypeCode = "HIS_MY_SURG_ROOM";
                    else if (_jsonOutput.Contains("SURG_ROOM_CODE"))////)
                        FilterTypeCode = "HIS_SURG_ROOM";
                    else if (_jsonOutput.Contains("EXAM_ROOM_CODE"))////)
                        FilterTypeCode = "HIS_EXAM_ROOM";
                    else if (_jsonOutput.Contains("CLINICAL_ROOM_CODE"))////)
                        FilterTypeCode = "HIS_CLINICAL_ROOM";
                    else if (_jsonOutput.Contains("MEDICINE_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_MEDICINE_TYPE";
                    else if (_jsonOutput.Contains("MATERIAL_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_MATERIAL_TYPE";
                    else if (_jsonOutput.Contains("SUPPLIER_CODE"))////)
                        FilterTypeCode = "HIS_SUPPLIER";
                    else if (_jsonOutput.Contains("INVOICE_CODE"))////)
                        FilterTypeCode = "HIS_INVOICE";
                    else if (_jsonOutput.Contains("INVOICE_BOOK_CODE"))////)
                        FilterTypeCode = "HIS_INVOICE_BOOK";
                    else if (_jsonOutput.Contains("IMP_SOURCE_CODE"))////)
                        FilterTypeCode = "HIS_IMP_SOURCE";
                    else if (_jsonOutput.Contains("MEDI_STOCK_NOT_BUSINESS_CODE"))////)
                        FilterTypeCode = "HIS_MEDI_STOCK_NOT_BUSINESS";
                    else if (_jsonOutput.Contains("MEDI_STOCK_BUSINESS_CODE"))////)
                        FilterTypeCode = "HIS_MEDI_STOCK_BUSINESS";
                    else if (_jsonOutput.Contains("LOGINNAME_SALE"))////)
                        FilterTypeCode = "ACS_USER_SALE";
                    else if (_jsonOutput.Contains("LOGINNAME_DOCTOR"))
                        FilterTypeCode = "ACS_USER_DOCTOR";
                    else if (_jsonOutput.Contains("DOCTOR_LOGINNAME"))
                        FilterTypeCode = "ACS_USER_DOCTOR";
                    else if (_jsonOutput.Contains("SERVICE_GROUP_CODE"))////)
                        FilterTypeCode = "HIS_SERVICE_GROUP";
                    else if (_jsonOutput.Contains("CAREER_CODE"))////)
                        FilterTypeCode = "HIS_CAREER";
                    else if (_jsonOutput.Contains("KSK_CONTRACT_CODE"))////)
                        FilterTypeCode = "HIS_KSK_CONTRACT";
                    else if (_jsonOutput.Contains("BRANCH_CODE"))////)
                        FilterTypeCode = "HIS_BRANCH";
                    else if (_jsonOutput.Contains("EXAM_SERVICE_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_EXAM_SERVICE_TYPE";
                    else if (_jsonOutput.Contains("CAREER_CODE"))////)
                        FilterTypeCode = "HIS_CAREER";
                    else if (_jsonOutput.Contains("DEATH_CAUSE_CODE"))////)
                        FilterTypeCode = "HIS_DEATH_CAUSE";
                    else if (_jsonOutput.Contains("PROGRAM_CODE"))////)
                        FilterTypeCode = "HIS_PROGRAM";
                    else if (_jsonOutput.Contains("PATIENT_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_PATIENT_TYPE";
                    else if (_jsonOutput.Contains("PATIENT_RAW_MEDICINAL_HERBS_TYPE_CODE"))
                        FilterTypeCode = "HIS_PATIENT_RAW_MEDICINAL_HERBS_TYPE";
                    else if (_jsonOutput.Contains("IMP_MEST_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_IMP_MEST_TYPE";
                    else if (_jsonOutput.Contains("IMP_MEST_STT_CODE"))////)
                        FilterTypeCode = "HIS_IMP_MEST_STT";
                    //else if (JSONOUTPUT.Contains("EXECUTE_ROOM_CODE"))////)
                    //    FilterTypeCode = "HIS_EXECUTE_ROOM";
                    else if (_jsonOutput.Contains("EXECUTE_DEPARTMENT_CODE"))////)
                        FilterTypeCode = "HIS_DEPARTMENT";
                    else if (_jsonOutput.Contains("CLINICAL_DEPARTMENT_CODE"))////)
                        FilterTypeCode = "HIS_CLINICAL_DEPARTMENT";
                    else if (_jsonOutput.Contains("ROOM_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_ROOM_TYPE";
                    else if (_jsonOutput.Contains("EXECUTE_GROUP_CODE"))////)
                        FilterTypeCode = "HIS_EXECUTE_GROUP";
                    else if (_jsonOutput.Contains("INFUSION_STT_CODE"))////)
                        FilterTypeCode = "HIS_INFUSION_STT";
                    else if (_jsonOutput.Contains("GENDER_CODE"))////)
                        FilterTypeCode = "HIS_GENDER";
                    else if (_jsonOutput.Contains("EXP_MEST_STT_CODE"))////)
                        FilterTypeCode = "HIS_EXP_MEST_STT";
                    else if (_jsonOutput.Contains("EXP_MEST_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_EXP_MEST_TYPE";
                    else if (_jsonOutput.Contains("TREATMENT_RESULT_CODE"))////)
                        FilterTypeCode = "HIS_TREATMENT_RESULT";
                    else if (_jsonOutput.Contains("TREATMENT_END_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_TREATMENT_END_TYPE";
                    else if (_jsonOutput.Contains("SERVICE_GROUP_CODE"))////)
                        FilterTypeCode = "HIS_SERVICE_GROUP";
                    else if (_jsonOutput.Contains("SERVICE_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_SERVICE_TYPE";
                    else if (_jsonOutput.Contains("SERVICE_CODE"))////)
                        FilterTypeCode = "HIS_SERVICE";
                    //else if (JSONOUTPUT.Contains("CASHIER_ROOM_CODE"))////)
                    //    FilterTypeCode = "HIS_CASHIER_ROOM";
                    else if (_jsonOutput.Contains("TREATMENT_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_TREATMENT_TYPE";
                    else if (_jsonOutput.Contains("ACCOUNT_BOOK_CODE"))////)
                        FilterTypeCode = "HIS_ACCOUNT_BOOK";
                    else if (_jsonOutput.Contains("ICD_GROUP_CODE"))////)
                        FilterTypeCode = "HIS_ICD_GROUP";
                    else if (_jsonOutput.Contains("PTTT_METHOD_CODE"))////)
                        FilterTypeCode = "HIS_PTTT_METHOD";
                    else if (_jsonOutput.Contains("PTTT_GROUP_CODE"))////)
                        FilterTypeCode = "HIS_PTTT_GROUP";
                    else if (_jsonOutput.Contains("BLOOD_CODE"))////)
                        FilterTypeCode = "HIS_BLOOD";
                    else if (_jsonOutput.Contains("BLOOD_RH_CODE"))////)
                        FilterTypeCode = "HIS_BLOOD_RH";
                    else if (_jsonOutput.Contains("BID_CODE"))////)
                        FilterTypeCode = "HIS_BID";
                    else if (_jsonOutput.Contains("BED_CODE"))////)
                        FilterTypeCode = "HIS_BED";
                    else if (_jsonOutput.Contains("EXECUTE_ROLE_CODE"))////)
                        FilterTypeCode = "HIS_EXECUTE_ROLE";
                    else if (_jsonOutput.Contains("SERVICE_REQ_TYPE_CODE"))////)
                        FilterTypeCode = "HIS_SERVICE_REQ_TYPE";
                    else if (_jsonOutput.Contains("SERVICE_REQ_STT_CODE"))////)
                        FilterTypeCode = "HIS_SERVICE_REQ_STT";
                    else if (_jsonOutput.Contains("REPORT_TYPE_CAT_CODE"))////)
                        FilterTypeCode = "HIS_REPORT_TYPE_CAT";
                    else if (_jsonOutput.Contains("OTHER_MEDI_STOCK_CODE"))////)
                        FilterTypeCode = "OTHER_HIS_MEDI_STOCK";
                    else if (_jsonOutput.Contains("IMP_MEDI_STOCK_CODE"))////)
                        FilterTypeCode = "HIS_MEDI_STOCK";
                    else if (_jsonOutput.Contains("MEDI_STOCK_CODE"))////)
                        FilterTypeCode = "HIS_MEDI_STOCK";
                    else if (_jsonOutput.Contains("LOGINNAME"))////)
                        FilterTypeCode = "ACS_USER";
                    else if (_jsonOutput.Contains("DEPARTMENT_CODE"))////)
                        FilterTypeCode = "HIS_DEPARTMENT";
                    else if (_jsonOutput.Contains("ICD_CODE"))////)
                        FilterTypeCode = "HIS_ICD";
                    else if (_jsonOutput.Contains("MEST_ROOM_CODE"))
                        FilterTypeCode = "HIS_MEST_ROOM";
                    else if (_jsonOutput.Contains("ROOM_CODE"))////)
                        FilterTypeCode = "HIS_ROOM";
                    else if (_jsonOutput.Contains("EXP_MEST_REASON_CODE"))
                        FilterTypeCode = "HIS_EXP_MEST_REASON";
                    else if (_jsonOutput.Contains("MEDI_STOCK_PERIOD_CODE"))
                        FilterTypeCode = "HIS_MEDI_STOCK_PERIOD";
                    else if (_jsonOutput.Contains("WORK_PLACE_CODE"))
                        FilterTypeCode = "HIS_WORK_PLACE";
                    else if (_jsonOutput.Contains("THE_BRANCH__CODE") || _jsonOutput.Contains("THE_BRANCH__CODE_ADMIN"))
                        FilterTypeCode = "THE_BRANCH";
                    else if (_jsonOutput.Contains("AOS_ACCOUNT_TYPE_CODE"))
                        FilterTypeCode = "AOS_ACCOUNT_TYPE";
                    else if (_jsonOutput.Contains("TRANSACTION_TYPE_CODE"))
                        FilterTypeCode = "HIS_TRANSACTION_TYPE";
                    else if (_jsonOutput.Contains("MACHINE_CODE"))
                        FilterTypeCode = "HIS_MACHINE";
                    else if (_jsonOutput.Contains("MEDI_ORG_CODE"))
                        FilterTypeCode = "HIS_MEDI_ORG";
                    else if (_jsonOutput.Contains("AOS_BANK_ACCOUNT_TYPE_CODE"))
                        FilterTypeCode = "AOS_BANK_ACCOUNT_TYPE";
                    else if (_jsonOutput.Contains("MEDI_STOCK_CABINET_CODE"))
                        FilterTypeCode = "HIS_MEDI_STOCK_CABINET";
                    else if (_jsonOutput.Contains("MEST_ROOM_STOCK_CODE"))
                        FilterTypeCode = "HIS_MEST_ROOM_MEDI_STOCK";
                    else if (_jsonOutput.Contains("BANK_BRANCH_CODE"))
                        FilterTypeCode = "THE_BANK_BRANCH";
                    else if (_jsonOutput.Contains("FORM_TYPE_CODE"))
                        FilterTypeCode = "SAR_FORM_TYPE";
                    else if (_jsonOutput.Contains("ISSUE_BRANCH_CODE"))
                        FilterTypeCode = "THE_BRANCH";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                FilterTypeCode = null;
            }
            return FilterTypeCode;
        }


        public static string[] GetLimitCodes(string _jsonOutput)
        {
            string[] result = null;
            try
            {
                string limitCodeNameLike = "_LIMIT_CODE\":\"";
                if (!string.IsNullOrWhiteSpace(_jsonOutput) && _jsonOutput.Contains(limitCodeNameLike))
                {
                    int start = _jsonOutput.IndexOf(limitCodeNameLike);
                    int end = _jsonOutput.IndexOf("\"", (start + limitCodeNameLike.Length));
                    string limitCodeSt = end != -1 ? _jsonOutput.Substring(start + limitCodeNameLike.Length, end - (start + limitCodeNameLike.Length)) : _jsonOutput.Substring(start + limitCodeNameLike.Length);
                    if (!string.IsNullOrWhiteSpace(limitCodeSt))
                    {
                        result = limitCodeSt.Split(new char[] { ',' });
                    }

                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public static void GetValueOutput0(string _jsonOutput, ref string Output0)
        {
            try
            {
                //string JSON_OUTPUT = "sdfsdf_OUTPUT0:2x";
                int lastIndex = _jsonOutput.LastIndexOf(StrOutput0);
                if (lastIndex >= 0)
                {
                    Output0 = _jsonOutput.Substring(lastIndex + StrOutput0.Length);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public static void RemoveStrOutput0(ref string _jsonOutput)
        {
            try
            {
                //string JSON_OUTPUT = "sdfsdf_OUTPUT0:2x";
                int lastIndex = _jsonOutput.LastIndexOf(StrOutput0);
                if (lastIndex >= 0)
                {
                    _jsonOutput = _jsonOutput.Substring(0, lastIndex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public static void GetListfilter(string _jsonOutput, ref string[] Filters)
        {
            try
            {
                RemoveStrOutput0(ref _jsonOutput);
                string toJson = "{" + (_jsonOutput ?? "").Replace("{", "\"").Replace("}", "\"") + "}";
                Inventec.Common.Logging.LogSystem.Info("toJson:" + toJson);
                Dictionary<string, string> dicFilter = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(toJson);

                Filters = dicFilter.Where(o => !o.Key.Contains(StrOutputLimitCode)).OrderBy(o => o.Value).Select(o => o.Key).ToArray<string>();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }



        public static bool IsCodeField(string filter)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(filter) && (filter.Contains("CODE") || filter.Contains("LOGINNAME")))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
