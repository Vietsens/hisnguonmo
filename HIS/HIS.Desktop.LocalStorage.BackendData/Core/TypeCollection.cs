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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HTC.EFMODEL.DataModels;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MOS.LibraryHein.Bhyt.HeinLiveArea;
using SAR.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.LocalStorage.BackendData.Core
{
    partial class TypeCollection
    {
        internal static readonly List<Type> AcceptTypes = new List<Type>() { 
            typeof(CommuneADO),
            typeof(AgeADO), 
            typeof(HeinLiveAreaData), 
            typeof(MedicineMaterialTypeComboADO), 

            typeof(ACS_USER), 
            typeof(ACS_APPLICATION), 
            
            typeof(V_SDA_COMMUNE), 
            typeof(V_SDA_DISTRICT), 
            typeof(V_SDA_PROVINCE), 
            typeof(SDA_NATIONAL), 
            typeof(SDA_TRANSLATE), 
            typeof(SDA_COMMUNE), 
            typeof(SDA_DISTRICT), 
            typeof(SDA_PROVINCE), 
            typeof(SDA_GROUP), 
            typeof(SDA_ETHNIC), 
            typeof(SDA_LANGUAGE), 
            typeof(SDA_MODULE_FIELD), 

            typeof(SAR_REPORT_TEMPLATE), 
            typeof(SAR_REPORT_TYPE), 
            typeof(SAR_REPORT_STT), 
            typeof(SAR_PRINT_TYPE), 
            typeof(V_SAR_RETY_FOFI), 
            typeof(SAR_FORM_FIELD), 
            
            typeof(HIS_MEDI_ORG), 
            typeof(V_HIS_USER_ROOM), 
            typeof(HIS_ICD), 
            typeof(V_HIS_SERVICE_PATY), 
            typeof(V_HIS_SERVICE), 
            typeof(V_HIS_SERVICE_ROOM), 
            typeof(V_HIS_MEDICINE_TYPE), 
            typeof(V_HIS_MATERIAL_TYPE), 
            typeof(HIS_PTTT_HIGH_TECH), 
            typeof(HIS_EMOTIONLESS_RESULT), 
            typeof(HIS_PTTT_PRIORITY), 
            typeof(HIS_EXE_SERVICE_MODULE), 
            typeof(HIS_EQUIPMENT_SET), 
            typeof(HIS_MACHINE), 
            typeof(HIS_SERVICE_MACHINE), 
            typeof(HIS_EMPLOYEE), 
            typeof(HIS_PACKAGE), 
            typeof(HIS_OWE_TYPE), 
            typeof(HIS_EXP_MEST_REASON), 
            typeof(HIS_MEDICINE_TYPE_TUT), 
            typeof(HIS_MEDICINE_PATY), 
            typeof(HIS_MEDICINE_GROUP), 
            typeof(HIS_ICD_CM), 
            typeof(HIS_MEST_METY_DEPA), 
            typeof(HIS_MEDI_STOCK_MATY), 
            typeof(V_HIS_MEDI_STOCK_MATY), 
            typeof(HIS_MEDI_STOCK_METY), 
            typeof(V_HIS_MEDI_STOCK_METY), 
            typeof(HIS_ACIN_INTERACTIVE), 
            typeof(V_HIS_ACIN_INTERACTIVE), 
            typeof(HIS_HTU), 
            typeof(HIS_BRANCH), 
            typeof(HIS_MEST_PATIENT_TYPE), 
            typeof(V_HIS_BED_ROOM), 
            typeof(V_HIS_BED), 
            typeof(HIS_EXP_MEST_TEMPLATE), 
            typeof(V_HIS_EXECUTE_ROOM), 
            typeof(HIS_EXECUTE_ROOM), 
            typeof(HIS_DEPARTMENT), 
            typeof(HIS_KSK_CONTRACT), 
            typeof(HIS_CAREER), 
            typeof(HIS_DEATH_CAUSE), 
            typeof(HIS_DEATH_WITHIN), 
            typeof(V_HIS_ROOM), 
            typeof(HIS_PATIENT_TYPE), 
            typeof(HIS_IMP_MEST_TYPE), 
            typeof(HIS_IMP_MEST_STT), 
            typeof(HIS_ROOM_TYPE), 
            typeof(V_HIS_PATIENT_TYPE_ALLOW), 
            typeof(HIS_PATIENT_TYPE_ALLOW), 
            typeof(HIS_EXECUTE_GROUP), 
            typeof(HIS_TRAN_PATI_FORM), 
            typeof(HIS_TRAN_PATI_REASON), 
            typeof(HIS_TRAN_PATI_TECH), 
            typeof(HIS_SERVICE_REQ_STT), 
            typeof(HIS_SERVICE_REQ_TYPE), 
            typeof(HIS_SERVICE_UNIT), 
            typeof(HIS_GENDER), 
            typeof(HIS_HEIN_SERVICE_TYPE), 
            typeof(HIS_MEDICINE_LINE), 
            typeof(HIS_MILITARY_RANK), 
            typeof(HIS_MEDICINE_USE_FORM), 
            typeof(HIS_MEDI_REACT_TYPE), 
            typeof(HIS_EXP_MEST_STT), 
            typeof(HIS_TREATMENT_END_TYPE), 
            typeof(HIS_TREATMENT_RESULT), 
            typeof(V_HIS_SERVICE_PACKAGE), 
            typeof(HIS_SERVICE_GROUP), 
            typeof(V_HIS_SERV_SEGR), 
            typeof(HIS_SERVICE_TYPE), 
            typeof(HIS_MEDI_STOCK), 
            typeof(V_HIS_MEDI_STOCK), 
            typeof(HIS_EXP_MEST_TYPE), 
            typeof(HIS_MEDICINE_TYPE), 
            typeof(HIS_MANUFACTURER), 
            typeof(V_HIS_MEDICINE_TYPE_ACIN), 
            typeof(HIS_PACKING_TYPE), 
            typeof(V_HIS_TEST_INDEX_RANGE), 
            typeof(V_HIS_TEST_INDEX), 
            typeof(HIS_TREATMENT_TYPE), 
            typeof(HIS_MATERIAL_TYPE), 
            typeof(HIS_WORK_PLACE), 
            typeof(HIS_EMERGENCY_WTIME), 
            typeof(HIS_REHA_TRAIN_TYPE), 
            typeof(HIS_BLOOD_ABO), 
            typeof(HIS_BLOOD_RH), 
            typeof(HIS_PAY_FORM), 
            typeof(HIS_BLOOD_TYPE), 
            typeof(V_HIS_BLOOD_TYPE), 
            typeof(HIS_SUPPLIER), 
            typeof(HIS_IMP_SOURCE), 
            typeof(HIS_TRANSACTION_TYPE), 
            typeof(HIS_BORN_TYPE), 
            typeof(HIS_BORN_POSITION), 
            typeof(HIS_BORN_RESULT), 
            typeof(V_HIS_MEST_ROOM), 
            typeof(HIS_ANTICIPATE), 
            typeof(HIS_ACCIDENT_HURT_TYPE), 
            typeof(V_HIS_CASHIER_ROOM), 
            typeof(HIS_CASHIER_ROOM), 
            typeof(HIS_FUND), 
            typeof(HIS_SERVICE_FOLLOW), 
            typeof(HIS_PTTT_METHOD), 
            typeof(HIS_PTTT_GROUP), 
            typeof(HIS_EMOTIONLESS_METHOD), 
            typeof(HIS_PTTT_CONDITION), 
            typeof(HIS_PTTT_CATASTROPHE), 
            typeof(HIS_EXECUTE_ROLE), 
            typeof(HIS_BHYT_BLACKLIST), 
            typeof(HIS_BHYT_WHITELIST), 
            typeof(HIS_PAAN_POSITION), 
            typeof(HIS_PAAN_LIQUID), 
            typeof(HIS_REPAY_REASON), 
            typeof(HIS_MEDICINE_TYPE_ROOM), 
            typeof(HIS_SERE_SERV_TEMP),
 
            typeof(HTC_PERIOD), 
            typeof(HTC_EXPENSE_TYPE), 

            typeof(LIS_SAMPLE_STT),
            typeof(HIS_PATIENT_CASE)
        };
    }
}
