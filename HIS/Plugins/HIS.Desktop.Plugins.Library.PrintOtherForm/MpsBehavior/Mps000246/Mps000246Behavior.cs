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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using HIS.Desktop.Plugins.Library.PrintOtherForm.RunPrintTemplate;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000246
{
    public partial class Mps000246Behavior : MpsDataBase, ILoad
    {
        V_HIS_PATIENT patient { get; set; }
        V_HIS_SERVICE_REQ serviceReq { get; set; }
        V_HIS_SERE_SERV_REHA sereServPTTT { get; set; }
        HIS_SERE_SERV_EXT sereServExt { get; set; }
        HIS_SERE_SERV sereServ { get; set; }
        V_HIS_TREATMENT treatment { get; set; }
        List<V_HIS_EKIP_USER> ekipUsers { get; set; }
        List<V_HIS_TREATMENT_BED_ROOM> treatmentBedRooms { get; set; }

        public Mps000246Behavior(long? _serviceReqId, long? _sereServId, long _treatmentId, long _patientId)
            : base(_serviceReqId, _sereServId, _treatmentId, _patientId)
        {

        }

        bool ILoad.Load(string printTypeCode, UpdateType.TYPE updateType)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                this.LoadData();

                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);
                if (printTemplate == null)
                {
                    throw new Exception("Khong tim thay template");
                }

                this.SetDicParamPatient(ref dicParamPlus);
                this.SetDicParamServiceReq(ref dicParamPlus);
                this.SetDicParamBedAndBedRoomFromTreatment(ref dicParamPlus);
                //this.SetDicParamEkipUser(ref dicParamPlus);
                this.SetDicParamSereServPTTT(ref dicParamPlus);
                this.SetDicParamSereServExt(ref dicParamPlus);

                HisTreatmentWithPatientTypeInfoSDO treatmentWithPatientType = LoadDataToCurrentTreatmentData(this.treatment.ID, this.serviceReq.INTRUCTION_TIME);
                if (treatmentWithPatientType != null)
                {
                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(treatmentWithPatientType, dicParamPlus);

                    dicParamPlus.Add("HEIN_CARD_NUMBER_SEPARATE", SetHeinCardNumberDisplayByNumber(treatmentWithPatientType.HEIN_CARD_NUMBER));
                    dicParamPlus.Add("STR_HEIN_CARD_FROM_TIME", Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatmentWithPatientType.HEIN_CARD_FROM_TIME));
                    dicParamPlus.Add("STR_HEIN_CARD_TO_TIME", Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatmentWithPatientType.HEIN_CARD_TO_TIME));
                    dicParamPlus.Add("OUT_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(treatmentWithPatientType.OUT_TIME ?? 0));
                }
                else
                {
                    treatmentWithPatientType = new HisTreatmentWithPatientTypeInfoSDO();
                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(treatmentWithPatientType, dicParamPlus);
                }

                V_HIS_TREATMENT_BED_ROOM treatmentBedRoom = GetTreatmentBedRoom(this.treatment.ID);
                if (treatmentBedRoom != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT_BED_ROOM>(treatmentBedRoom, dicParamPlus);
                }
                else
                {
                    treatmentBedRoom = new V_HIS_TREATMENT_BED_ROOM();
                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT_BED_ROOM>(treatmentBedRoom, dicParamPlus);
                }

                V_HIS_DEPARTMENT_TRAN departmentTran = GetDepartmentTran(treatment.ID);
                if (departmentTran != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_DEPARTMENT_TRAN>(departmentTran, dicParamPlus);
                }
                else
                {
                    departmentTran = new V_HIS_DEPARTMENT_TRAN();
                    AddKeyIntoDictionaryPrint<V_HIS_DEPARTMENT_TRAN>(departmentTran, dicParamPlus);
                }

                //Thoi gian vao vien
                if (this.treatment != null)
                {
                    dicParamPlus.Add("CLINICAL_IN_TIME_STR", this.treatment.CLINICAL_IN_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.treatment.CLINICAL_IN_TIME.Value) : "");
                    dicParamPlus.Add("IN_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.treatment.IN_TIME));
                    dicParamPlus.Add("AGE_STRING", Inventec.Common.DateTime.Calculation.AgeString(this.treatment.TDL_PATIENT_DOB, "", "", "", "", this.treatment.IN_TIME));
                }

                //Thoi gian bat dau
                dicParamPlus.Add("START_TIME_STR", this.serviceReq.START_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.serviceReq.START_TIME.Value) : "");
                dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentTime());

                if (this.treatment != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(this.treatment, dicParamPlus);
                }
                else
                {
                    this.treatment = new V_HIS_TREATMENT();
                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(this.treatment, dicParamPlus);
                }

                List<string> keys = new List<string>();
                foreach (var item in dicParamPlus)
                {
                    if (item.Value == null)
                    {
                        keys.Add(item.Key);
                    }
                }

                if (keys.Count > 0)
                {
                    foreach (var key in keys)
                    {
                        dicParamPlus[key] = "";
                    }
                }
                WaitingManager.Hide();

                IRunTemp behavior = RunUpdateFactory.RunTemplateByUpdateType(this.serviceReq, updateType);
                result = behavior != null ? (behavior.Run(printTemplate, dicParamPlus, dicImagePlus, richEditorMain, null)) : false;
            }
            catch (Exception ex)
            {
                result = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
                {
                    string separateSymbol = "-";
                    result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        private HisTreatmentWithPatientTypeInfoSDO LoadDataToCurrentTreatmentData(long treatmentId, long? intructionTime)
        {
            HisTreatmentWithPatientTypeInfoSDO treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.INTRUCTION_TIME = intructionTime ?? (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.UtcNow) ?? 0);
                var apiResult = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    treatment = apiResult.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        private V_HIS_DEPARTMENT_TRAN GetDepartmentTran(long treatmentId)
        {
            V_HIS_DEPARTMENT_TRAN result = new V_HIS_DEPARTMENT_TRAN();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDepartmentTranLastFilter treatmentFilter = new MOS.Filter.HisDepartmentTranLastFilter();
                treatmentFilter.TREATMENT_ID = treatmentId;
                result = new BackendAdapter(param).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param);
            }
            catch (Exception ex)
            {
                result = new V_HIS_DEPARTMENT_TRAN();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private V_HIS_TREATMENT_BED_ROOM GetTreatmentBedRoom(long treatmentId)
        {
            CommonParam param = new CommonParam();
            V_HIS_TREATMENT_BED_ROOM result = new V_HIS_TREATMENT_BED_ROOM();
            try
            {
                MOS.Filter.HisTreatmentBedRoomViewFilter hisTreatmentBedRoomFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                hisTreatmentBedRoomFilter.TREATMENT_ID = treatmentId;
                result = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentBedRoomFilter, param).FirstOrDefault(o => o.REMOVE_TIME == null);
            }
            catch (Exception ex)
            {
                result = new V_HIS_TREATMENT_BED_ROOM();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
