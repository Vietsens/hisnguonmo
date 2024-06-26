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
using Core.ServiceCombo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.Core.ServiceCombo;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.LocalCacheManager
{
    class ProcessData
    {
        internal static void ResetDataExt(string type)
        {
            try
            {
                if (type == (typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE)).ToString()
                    || type == (typeof(MOS.EFMODEL.DataModels.HIS_SERVICE)).ToString()
                    || type == (typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY)).ToString()
                    || type == (typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM)).ToString())
                {
                    ServiceComboDataWorker.DicServiceCombo = new Dictionary<long, LocalStorage.BackendData.ADO.ServiceComboADO>();
                }

                if (type == (typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE)).ToString()
                    || type == (typeof(MOS.EFMODEL.DataModels.HIS_SERVICE)).ToString()
                    || type == (typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY)).ToString())
                {
                    ServiceByPatientTypeDataWorker.dicServiceByPatientType = new Dictionary<long, List<LocalStorage.BackendData.ADO.ServiceADO>>();
                }

                if (type == (typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY)).ToString())
                {
                    BranchDataWorker.ResetServicePaty();
                }

                if (type == (typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE)).ToString()
                    || type == (typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE)).ToString())
                {
                    BackendDataWorker.Reset<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>();
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>();
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>();
                }

                if (type == (typeof(SDA.EFMODEL.DataModels.V_SDA_COMMUNE)).ToString()
                   || type == (typeof(SDA.EFMODEL.DataModels.V_SDA_DISTRICT)).ToString()
                   || type == (typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE)).ToString())
                {
                    if (type == (typeof(SDA.EFMODEL.DataModels.V_SDA_COMMUNE)).ToString())
                    {
                        BackendDataWorker.Reset<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                        BackendDataWorker.Reset<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    }
                    else if (type == (typeof(SDA.EFMODEL.DataModels.V_SDA_DISTRICT)).ToString())
                    {
                        BackendDataWorker.Reset<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                        BackendDataWorker.Reset<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    }
                    else if (type == (typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE)).ToString())
                    {
                        BackendDataWorker.Reset<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                        BackendDataWorker.Reset<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                    }
                    BackendDataWorker.Reset<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void ProcessSyncAllData()
        {
            try
            {
                BackendDataWorker.ResetAll();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        internal static void ReloadCacheBackendData(Type typeInput)
        {
            try
            {
                object type = Activator.CreateInstance(typeInput);
                if (type == null) return;

                if (type.GetType() == typeof(SDA.EFMODEL.DataModels.SDA_CONFIG_APP))
                {
                    HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                }
                else if (type.GetType() == typeof(SDA.EFMODEL.DataModels.SDA_CONFIG))
                {
                    Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.Refresh();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_CONFIG))
                {
                    HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                }
                else if (type.GetType() == typeof(HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO))
                {
                    var rs = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                }
                else if (type.GetType() == typeof(ACS.EFMODEL.DataModels.ACS_USER))
                {
                    var rs = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }
                else if (type.GetType() == typeof(SDA.EFMODEL.DataModels.V_SDA_COMMUNE))
                {
                    var rs = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                    var rs1 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                }
                else if (type.GetType() == typeof(SDA.EFMODEL.DataModels.V_SDA_DISTRICT))
                {
                    var rs = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                    var rs1 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                }
                else if (type.GetType() == typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE))
                {
                    var rs = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    var rs1 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                }
                else if (type.GetType() == typeof(SDA.EFMODEL.DataModels.SDA_NATIONAL))
                {
                    var rs = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>();
                }
                else if (type.GetType() == typeof(SDA.EFMODEL.DataModels.SDA_GROUP))
                {
                    var rs = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_GROUP>();
                }
                else if (type.GetType() == typeof(SDA.EFMODEL.DataModels.SDA_ETHNIC))
                {
                    var rs = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                }

                #region SAR
                else if (type.GetType() == typeof(SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE))
                {
                    var rs = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE>();
                }
                else if (type.GetType() == typeof(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE))
                {
                    var rs = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>();
                }
                else if (type.GetType() == typeof(SAR.EFMODEL.DataModels.SAR_REPORT_STT))
                {
                    var rs = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_REPORT_STT>();
                }
                else if (type.GetType() == typeof(SAR.EFMODEL.DataModels.SAR_PRINT_TYPE))
                {
                    var rs = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                }
                else if (type.GetType() == typeof(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI))
                {
                    var rs = BackendDataWorker.Get<SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI>();
                }
                else if (type.GetType() == typeof(SAR.EFMODEL.DataModels.SAR_FORM_FIELD))
                {
                    var rs = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_FORM_FIELD>();
                }
                #endregion

                #region MOS
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_EMPLOYEE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_OWE_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_ICD_CM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD_CM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_MATY))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_MATY>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_MATY))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_MATY>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_METY))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_METY>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_ACIN_INTERACTIVE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ACIN_INTERACTIVE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE>();
                }
                else if (type.GetType() == typeof(HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO))
                {
                    var rs = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_ORG))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_HTU))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HTU>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_BRANCH))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_USER_ROOM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_BED_ROOM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_BED))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_DEPARTMENT))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_CAREER))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_ROOM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_ROOM_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_ICD))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_GENDER))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PACKAGE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PACKAGE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_SERV_SEGR))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERV_SEGR>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>();
                    var rs761 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
                    var rs761 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MANUFACTURER))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_PACKING_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PACKING_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_LOG_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_LOG_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>();
                    var rs761 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>();
                    var rs761 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_WORK_PLACE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_REHA_TRAIN_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_REHA_TRAIN_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_ABO))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_RH))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_SUPPLIER))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_IMP_SOURCE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_SOURCE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_BORN_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BORN_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_BORN_POSITION))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BORN_POSITION>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_BORN_RESULT))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BORN_RESULT>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_ROOM_TYPE_MODULE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE_MODULE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_ANTICIPATE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ANTICIPATE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT_TYPE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT_TYPE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_FUND))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_FUND>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_ROOM_COUNTER))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM_COUNTER>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_METHOD))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_METHOD>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_GROUP))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_GROUP>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_PAAN_POSITION))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAAN_POSITION>();
                }
                else if (type.GetType() == typeof(MOS.EFMODEL.DataModels.HIS_PAAN_LIQUID))
                {
                    var rs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAAN_LIQUID>();
                }
                #endregion

                #region HTC
                else if (type.GetType() == typeof(HTC.EFMODEL.DataModels.HTC_PERIOD))
                {
                    var rs = BackendDataWorker.Get<HTC.EFMODEL.DataModels.HTC_PERIOD>();
                }
                else if (type.GetType() == typeof(HTC.EFMODEL.DataModels.HTC_EXPENSE_TYPE))
                {
                    var rs = BackendDataWorker.Get<HTC.EFMODEL.DataModels.HTC_EXPENSE_TYPE>();
                }
                #endregion
                #region LIS
                else if (type.GetType() == typeof(LIS.EFMODEL.DataModels.LIS_SAMPLE_STT))
                {
                    var rs = BackendDataWorker.Get<LIS.EFMODEL.DataModels.LIS_SAMPLE_STT>();
                }
                else if (type.GetType() == typeof(HTC.EFMODEL.DataModels.HTC_EXPENSE_TYPE))
                {
                    var rs = BackendDataWorker.Get<HTC.EFMODEL.DataModels.HTC_EXPENSE_TYPE>();
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        internal static void ReloadAllCacheBackendData(List<string> oldKeys)
        {
            try
            {
                #region ACS
                if (oldKeys.Contains(typeof(ACS.EFMODEL.DataModels.ACS_USER).ToString()))
                {
                    var rs = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }

                #endregion

                ServiceComboDataWorker.DicServiceCombo = new Dictionary<long, LocalStorage.BackendData.ADO.ServiceComboADO>();
                ServiceByPatientTypeDataWorker.dicServiceByPatientType = new Dictionary<long, List<LocalStorage.BackendData.ADO.ServiceADO>>();
                BackendDataWorker.Reset<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>();

                InitOtherReferenceConstant();

                #region Config
                Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.Refresh();
                HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                HIS.Desktop.LocalStorage.LisConfig.ConfigLoader.Refresh();
                HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                HIS.Desktop.LocalStorage.ConfigHideControl.ConfigHideControlWorker.Init();
                #endregion

                #region SDA
                if (oldKeys.Contains(typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE).ToString()))
                {
                    var rs7 = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                }
                if (oldKeys.Contains(typeof(SDA.EFMODEL.DataModels.V_SDA_DISTRICT).ToString()))
                {
                    var rs6 = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                }
                if (oldKeys.Contains(typeof(SDA.EFMODEL.DataModels.V_SDA_COMMUNE).ToString()))
                {
                    var rs5 = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                    var rs1 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                }
                if (oldKeys.Contains(typeof(SDA.EFMODEL.DataModels.SDA_NATIONAL).ToString()))
                {
                    var rs8 = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>();
                }
                if (oldKeys.Contains(typeof(SDA.EFMODEL.DataModels.SDA_ETHNIC).ToString()))
                {
                    var rs10 = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                }
                #endregion

                #region SAR
                if (oldKeys.Contains(typeof(SAR.EFMODEL.DataModels.SAR_PRINT_TYPE).ToString()))
                {
                    var rs14 = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                }
                #endregion

                #region MOS
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_EMPLOYEE).ToString()))
                {
                    var rs19a = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_OWE_TYPE).ToString()))
                {
                    var rs19 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON).ToString()))
                {
                    var rs20 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT).ToString()))
                {
                    var rs21 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA).ToString()))
                {
                    var rs23 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_MATY).ToString()))
                {
                    var rs24 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_MATY>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_MATY).ToString()))
                {
                    var rs25 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_MATY>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_METY).ToString()))
                {
                    var rs26 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_METY>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY).ToString()))
                {
                    var rs27 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_ACIN_INTERACTIVE).ToString()))
                {
                    var rs28 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ACIN_INTERACTIVE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE).ToString()))
                {
                    var rs29 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEDI_ORG).ToString()))
                {
                    var rs31 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>();
                }
                if (oldKeys.Contains(typeof(HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO).ToString()))
                {
                    var rs30 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO>();
                }
                if (oldKeys.Contains(typeof(MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData).ToString()))
                {
                    var rs33 = BackendDataWorker.Get<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData>();
                }
                if (oldKeys.Contains(typeof(HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO).ToString()))
                {
                    var rs761 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_HTU).ToString()))
                {
                    var rs34 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HTU>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_BRANCH).ToString()))
                {
                    var rs35 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_USER_ROOM).ToString()))
                {
                    var rs36 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE).ToString()))
                {
                    var rs37 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_BED_ROOM).ToString()))
                {
                    var rs38 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_BED).ToString()))
                {
                    var rs39 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE).ToString()))
                {
                    var rs40 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM).ToString()))
                {
                    var rs41 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM).ToString()))
                {
                    var rs42 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_DEPARTMENT).ToString()))
                {
                    var rs43 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_CAREER).ToString()))
                {
                    var rs47 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE).ToString()))
                {
                    var rs48 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN).ToString()))
                {
                    var rs49 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_ROOM).ToString()))
                {
                    var rs50 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE).ToString()))
                {
                    var rs51 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE).ToString()))
                {
                    var rs52 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT).ToString()))
                {
                    var rs53 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_ROOM_TYPE).ToString()))
                {
                    var rs54 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW).ToString()))
                {
                    var rs55 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW).ToString()))
                {
                    var rs56 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_ICD).ToString()))
                {
                    var rs57 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP).ToString()))
                {
                    var rs58 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM).ToString()))
                {
                    var rs59 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON).ToString()))
                {
                    var rs61 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY).ToString()))
                {
                    var rs61_1 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY).ToString()))
                {
                    var rs62 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
                    var rs62a = BranchDataWorker.DicServicePatyInBranch;
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT).ToString()))
                {
                    var rs63 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE).ToString()))
                {
                    var rs64 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT).ToString()))
                {
                    var rs65 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_GENDER).ToString()))
                {
                    var rs66 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE).ToString()))
                {
                    var rs67 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE).ToString()))
                {
                    var rs68 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK).ToString()))
                {
                    var rs70 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM).ToString()))
                {
                    var rs71 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE).ToString()))
                {
                    var rs72 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT).ToString()))
                {
                    var rs73 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE).ToString()))
                {
                    var rs74 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT).ToString()))
                {
                    var rs75 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PACKAGE).ToString()))
                {
                    var rs77 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PACKAGE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE).ToString()))
                {
                    var rs78 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
                    var rs761 = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM).ToString()))
                {
                    var rs79 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP).ToString()))
                {
                    var rs80 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_SERV_SEGR).ToString()))
                {
                    var rs81 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERV_SEGR>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE).ToString()))
                {
                    var rs82 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK).ToString()))
                {
                    var rs84 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE).ToString()))
                {
                    var rs85 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE).ToString()))
                {
                    var rs86 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>();
                    BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE).ToString()))
                {
                    var rs87 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
                    BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MANUFACTURER).ToString()))
                {
                    var rs88 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN).ToString()))
                {
                    var rs89 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_PACKING_TYPE).ToString()))
                {
                    var rs90 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PACKING_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE).ToString()))
                {
                    var rs91 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX).ToString()))
                {
                    var rs92 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE).ToString()))
                {
                    var rs93 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_LOG_TYPE).ToString()))
                {
                    var rs94 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_LOG_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE).ToString()))
                {
                    var rs95 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>();
                    BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE).ToString()))
                {
                    var rs96 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>();
                    BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK).ToString()))
                {
                    var rs97 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_WORK_PLACE).ToString()))
                {
                    var rs98 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME).ToString()))
                {
                    var rs101 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_REHA_TRAIN_TYPE).ToString()))
                {
                    var rs102 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_REHA_TRAIN_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_ABO).ToString()))
                {
                    var rs103 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_RH).ToString()))
                {
                    var rs104 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM).ToString()))
                {
                    var rs105 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE).ToString()))
                {
                    var rs108 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE).ToString()))
                {
                    var rs109 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_SUPPLIER).ToString()))
                {
                    var rs110 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_IMP_SOURCE).ToString()))
                {
                    var rs113 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_SOURCE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE).ToString()))
                {
                    var rs116 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_BORN_TYPE).ToString()))
                {
                    var rs117 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BORN_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_BORN_POSITION).ToString()))
                {
                    var rs118 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BORN_POSITION>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_BORN_RESULT).ToString()))
                {
                    var rs119 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BORN_RESULT>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM).ToString()))
                {
                    var rs120 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_ANTICIPATE).ToString()))
                {
                    var rs122 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ANTICIPATE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT_TYPE).ToString()))
                {
                    var rs123 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT_TYPE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM).ToString()))
                {
                    var rs124 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_FUND).ToString()))
                {
                    var rs125 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_FUND>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW).ToString()))
                {
                    var rs128 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW).ToString()))
                {
                    var rs129 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_PTTT_METHOD).ToString()))
                {
                    var rs130 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_METHOD>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_PTTT_GROUP).ToString()))
                {
                    var rs131 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_GROUP>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD).ToString()))
                {
                    var rs132 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION).ToString()))
                {
                    var rs133 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE).ToString()))
                {
                    var rs134 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE).ToString()))
                {
                    var rs136 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST).ToString()))
                {
                    var rs139 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST>();
                }
                if (oldKeys.Contains(typeof(MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST).ToString()))
                {
                    var rs140 = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>();
                }
                #endregion

                #region LIS
                if (oldKeys.Contains(typeof(LIS.EFMODEL.DataModels.LIS_SAMPLE_STT).ToString()))
                {
                    var rs146 = BackendDataWorker.Get<LIS.EFMODEL.DataModels.LIS_SAMPLE_STT>();
                }
                if (oldKeys.Contains(typeof(HTC.EFMODEL.DataModels.HTC_EXPENSE_TYPE).ToString()))
                {
                    var rs147 = BackendDataWorker.Get<HTC.EFMODEL.DataModels.HTC_EXPENSE_TYPE>();
                }

                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        internal static void InitOtherReferenceConstant()
        {
            try
            {
                //MPS constant
                MPS.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                MPS.PrintConfig.HisVRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>();

                MPS.ProcessorBase.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                MPS.ProcessorBase.PrintConfig.MediOrgCode = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT;
                MPS.ProcessorBase.PrintConfig.OrganizationName = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.ORGANIZATION_NAME;
                MPS.ProcessorBase.PrintConfig.ParentOrganizationName = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.PARENT_ORGANIZATION_NAME;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

    }
}
