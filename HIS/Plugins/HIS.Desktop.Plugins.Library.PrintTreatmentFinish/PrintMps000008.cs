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
using ACS.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish
{
    class PrintMps000008
    {
        MPS.Processor.Mps000008.PDO.Mps000008PDO mps000008RDO;
        MPS.Processor.Mps000008.PDO.Mps000008ADO mps000008ADO = new MPS.Processor.Mps000008.PDO.Mps000008ADO();
        List<V_HIS_EKIP_USER> ListEkipUser = new List<V_HIS_EKIP_USER>();
        long timeIn;    
        HIS_TRACKING tracking;

        public PrintMps000008(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.V_HIS_PATIENT HisPatient, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter, bool _printNow, long? roomId)
        {
            try
            {
                if (HisTreatment == null || HisTreatment.ID <= 0)
                {
                    result = false;
                    return;
                }

                if (HisConfigs.Get<string>(Config.KEY_IN_RA_VIEN) == "1" && HisTreatment.IS_ACTIVE == 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Resources.ResourceLanguageManager.BenhNhanChuaKhoaVienPhi, Resources.ResourceLanguageManager.GiayRaVien));
                }
                else
                {
                    ProcessThreadGetData(HisTreatment);

                    var appointmentPeriods = BackendDataWorker.Get<HIS_APPOINTMENT_PERIOD>();
                    MPS.Processor.Mps000008.PDO.PatientADO PatientADO = new MPS.Processor.Mps000008.PDO.PatientADO(HisPatient);

                    mps000008RDO = new MPS.Processor.Mps000008.PDO.Mps000008PDO(
                       PatientADO,
                       VHisPatientTypeAlter,
                       HisTreatment,
                       mps000008ADO,
                       timeIn,
                       ListEkipUser,
                       appointmentPeriods,
                       tracking);

                    result = Print.RunPrint(printTypeCode, fileName, mps000008RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _printNow, roomId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //public PrintMps000008(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.V_HIS_PATIENT HisPatient, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter, MPS.ProcessorBase.PrintConfig.PreviewType? _previewType, long? roomId)
        //{
        //    try
        //    {
        //        if (HisTreatment == null || HisTreatment.ID <= 0)
        //        {
        //            result = false;
        //            return;
        //        }

        //        if (HisConfigs.Get<string>(Config.KEY_IN_RA_VIEN) == "1" && HisTreatment.IS_ACTIVE == 1)
        //        {
        //            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Resources.ResourceLanguageManager.BenhNhanChuaKhoaVienPhi, Resources.ResourceLanguageManager.GiayRaVien));
        //        }
        //        else
        //        {
        //            ProcessThreadGetData(HisTreatment);

        //            var appointmentPeriods = BackendDataWorker.Get<HIS_APPOINTMENT_PERIOD>();
        //            MPS.Processor.Mps000008.PDO.PatientADO PatientADO = new MPS.Processor.Mps000008.PDO.PatientADO(HisPatient);

        //            mps000008RDO = new MPS.Processor.Mps000008.PDO.Mps000008PDO(
        //               PatientADO,
        //               VHisPatientTypeAlter,
        //               HisTreatment,
        //               mps000008ADO,
        //               timeIn,
        //               ListEkipUser,
        //               appointmentPeriods,
        //               tracking);

        //            result = Print.RunPrint(printTypeCode, fileName, mps000008RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _previewType, roomId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void ProcessThreadGetData(HIS_TREATMENT HisTreatment)
        {
            Thread ado = new Thread(ProcessAdo);
            Thread ekip = new Thread(ProcessEkip);
            Thread time = new Thread(ProcessTimeIn);
            Thread tracking = new Thread(ProcessTracking);
            try
            {
                ado.Start(HisTreatment);
                ekip.Start(HisTreatment);
                time.Start(HisTreatment);
                tracking.Start(HisTreatment);

                ado.Join();
                ekip.Join();
                time.Join();
                tracking.Join();
            }
            catch (Exception ex)
            {
                ado.Abort();
                ekip.Abort();
                time.Abort();
                tracking.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessTracking(object obj)
        {
            try
            {
                if (obj != null && obj.GetType() == typeof(HIS_TREATMENT))
                {
                    CommonParam param = new CommonParam();
                    HisTrackingFilter filter = new HisTrackingFilter();
                    filter.TREATMENT_ID = ((HIS_TREATMENT)obj).ID;
                    var listTracking = new BackendAdapter(param).Get<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (listTracking != null && listTracking.Count > 0)
                    {
                        listTracking = listTracking.OrderBy(o => o.TRACKING_TIME).ThenBy(o => o.ID).ToList();
                        tracking = listTracking.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessTimeIn(object obj)
        {
            try
            {
                if (obj != null && obj.GetType() == typeof(HIS_TREATMENT))
                {
                    var curentTreatment = obj as HIS_TREATMENT;
                    HIS_SERVICE_REQ service = null;
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.TREATMENT_ID = curentTreatment.ID;
                    var lstService = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_SERVICE_REQ>>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (lstService != null && lstService.Count > 0)
                    {
                        lstService = lstService.OrderBy(o => o.START_TIME ?? 9999999999999999).ThenBy(o => o.CREATE_TIME).ToList();
                        service = lstService.First();

                        lstService = lstService.Where(o => o.FINISH_TIME.HasValue && (o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT)).OrderByDescending(o => o.SERVICE_REQ_TYPE_ID).ThenBy(o => o.INTRUCTION_TIME).ToList();
                        if (lstService != null && lstService.Count > 0)
                        {
                            mps000008ADO.SURGRY_END_TIME = lstService.First().FINISH_TIME;
                        }
                    }

                    this.timeIn = curentTreatment.IN_TIME;

                    if (!curentTreatment.CLINICAL_IN_TIME.HasValue) //khám
                    {
                        if (service != null && service.ID > 0)
                        {
                            this.timeIn = service.START_TIME ?? 0;
                        }
                        else
                        {
                            this.timeIn = curentTreatment.IN_TIME;
                        }
                    }
                    else
                    {
                        this.timeIn = curentTreatment.CLINICAL_IN_TIME.HasValue ? curentTreatment.CLINICAL_IN_TIME.Value : curentTreatment.IN_TIME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessEkip(object obj)
        {
            try
            {
                if (obj != null && obj.GetType() == typeof(HIS_TREATMENT))
                {
                    // lấy về các phẫu thuật viên chính
                    MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                    sereServFilter.TREATMENT_ID = ((HIS_TREATMENT)obj).ID;
                    sereServFilter.TDL_SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT;
                    var sereServs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, null);
                    if (sereServs != null && sereServs.Count() > 0)
                    {
                        List<long> ekipIds = sereServs.Where(p => p.EKIP_ID.HasValue).Select(o => o.EKIP_ID.Value).Distinct().ToList();
                        if (ekipIds.Count > 0)
                        {
                            ListEkipUser = new List<V_HIS_EKIP_USER>();
                        }
                        int skip = 0;
                        while (ekipIds.Count - skip > 0)
                        {
                            var listIds = ekipIds.Skip(skip).Take(100).ToList();
                            skip += 100;

                            MOS.Filter.HisEkipUserViewFilter ekipFilter = new MOS.Filter.HisEkipUserViewFilter();
                            ekipFilter.EKIP_IDs = listIds;
                            var EkipUsers = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EKIP_USER>>("api/HisEkipUser/GetView", ApiConsumer.ApiConsumers.MosConsumer, ekipFilter, null);
                            if (EkipUsers != null && EkipUsers.Count > 0)
                            {
                                ListEkipUser.AddRange(EkipUsers);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAdo(object obj)
        {
            try
            {
                if (obj != null && obj.GetType() == typeof(HIS_TREATMENT))
                {
                    var HisTreatment = obj as HIS_TREATMENT;

                    List<Task> taskall = new List<Task>();
                    Task tsDeathCause = Task.Factory.StartNew((object data) =>
                    {
                        var treatment = data as HIS_TREATMENT;
                        if (treatment.DEATH_CAUSE_ID != null)
                        {
                            var deathCause = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == treatment.DEATH_CAUSE_ID.Value);
                            if (deathCause != null)
                            {
                                mps000008ADO.DEATH_CAUSE_CODE = deathCause.DEATH_CAUSE_CODE;
                                mps000008ADO.DEATH_CAUSE_NAME = deathCause.DEATH_CAUSE_NAME;
                            }
                        }
                    }, obj);
                    taskall.Add(tsDeathCause);

                    Task tsDeathWithin = Task.Factory.StartNew((object data) =>
                    {
                        var treatment = data as HIS_TREATMENT;
                        if (treatment.DEATH_WITHIN_ID != null)
                        {
                            var deathWithin = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == treatment.DEATH_WITHIN_ID.Value);
                            if (deathWithin != null)
                            {
                                mps000008ADO.DEATH_WITHIN_CODE = deathWithin.DEATH_WITHIN_CODE;
                                mps000008ADO.DEATH_WITHIN_NAME = deathWithin.DEATH_WITHIN_NAME;
                            }
                        }
                    }, obj);
                    taskall.Add(tsDeathWithin);

                    Task tsTreatmentResult = Task.Factory.StartNew((object data) =>
                    {
                        var treatment = data as HIS_TREATMENT;
                        if (treatment.TREATMENT_RESULT_ID != null)
                        {
                            var resultTreatment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == treatment.TREATMENT_RESULT_ID.Value);
                            if (resultTreatment != null)
                            {
                                mps000008ADO.TREATMENT_RESULT_NAME = resultTreatment.TREATMENT_RESULT_NAME;
                                mps000008ADO.TREATMENT_RESULT_CODE = resultTreatment.TREATMENT_RESULT_CODE;
                            }
                        }
                    }, obj);
                    taskall.Add(tsTreatmentResult);

                    Task tsTranPatiForm = Task.Factory.StartNew((object data) =>
                    {
                        var treatment = data as HIS_TREATMENT;
                        if (treatment.TRAN_PATI_FORM_ID != null)
                        {
                            var tranPatiForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == treatment.TRAN_PATI_FORM_ID.Value);
                            if (tranPatiForm != null)
                            {
                                mps000008ADO.TRAN_PATI_FORM_CODE = tranPatiForm.TRAN_PATI_FORM_CODE;
                                mps000008ADO.TRAN_PATI_FORM_NAME = tranPatiForm.TRAN_PATI_FORM_NAME;
                            }
                        }
                    }, obj);
                    taskall.Add(tsTranPatiForm);

                    Task tsTranPatiReason = Task.Factory.StartNew((object data) =>
                    {
                        var treatment = data as HIS_TREATMENT;
                        if (treatment.TRAN_PATI_REASON_ID != null)
                        {
                            var tranPatiReason = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == treatment.TRAN_PATI_REASON_ID.Value);
                            if (tranPatiReason != null)
                            {
                                mps000008ADO.TRAN_PATI_REASON_CODE = tranPatiReason.TRAN_PATI_REASON_CODE;
                                mps000008ADO.TRAN_PATI_REASON_NAME = tranPatiReason.TRAN_PATI_REASON_NAME;
                            }
                        }
                    }, obj);
                    taskall.Add(tsTranPatiReason);

                    Task tsRooms = Task.Factory.StartNew((object data) =>
                    {
                        var treatment = data as HIS_TREATMENT;

                        if (treatment.END_ROOM_ID.HasValue)
                        {
                            var endRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == treatment.END_ROOM_ID.Value);
                            if (endRoom != null)
                            {
                                mps000008ADO.END_DEPARTMENT_CODE = endRoom.DEPARTMENT_CODE;
                                mps000008ADO.END_DEPARTMENT_NAME = endRoom.DEPARTMENT_NAME;
                                mps000008ADO.END_ROOM_CODE = endRoom.ROOM_CODE;
                                mps000008ADO.END_ROOM_NAME = endRoom.ROOM_NAME;
                            }
                        }

                        if (treatment.FEE_LOCK_ROOM_ID.HasValue)
                        {
                            var feelockRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == treatment.FEE_LOCK_ROOM_ID.Value);
                            if (feelockRoom != null)
                            {
                                mps000008ADO.FEE_LOCK_DEPARTMENT_CODE = feelockRoom.DEPARTMENT_CODE;
                                mps000008ADO.FEE_LOCK_DEPARTMENT_NAME = feelockRoom.DEPARTMENT_NAME;
                                mps000008ADO.FEE_LOCK_ROOM_CODE = feelockRoom.ROOM_CODE;
                                mps000008ADO.FEE_LOCK_ROOM_NAME = feelockRoom.ROOM_NAME;
                            }
                        }

                        if (treatment.IN_ROOM_ID.HasValue)
                        {
                            var inRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == treatment.IN_ROOM_ID.Value);
                            if (inRoom != null)
                            {
                                mps000008ADO.IN_DEPARTMENT_CODE = inRoom.DEPARTMENT_CODE;
                                mps000008ADO.IN_DEPARTMENT_NAME = inRoom.DEPARTMENT_NAME;
                                mps000008ADO.IN_ROOM_CODE = inRoom.ROOM_CODE;
                                mps000008ADO.IN_ROOM_NAME = inRoom.ROOM_NAME;
                            }
                        }
                    }, obj);
                    taskall.Add(tsRooms);

                    Task tsTime = Task.Factory.StartNew((object data) =>
                    {
                        //Lay gio server
                        TimerSDO timeSync = new BackendAdapter(new CommonParam()).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam());
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeSync), timeSync));

                        if (timeSync != null)
                        {
                            mps000008ADO.CURRENT_SERVER_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(timeSync.DateNow) ?? 0;
                        }
                    }, obj);
                    taskall.Add(tsTime);

                    Task.WaitAll(taskall.ToArray());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_TRACKING GetTracking(HIS_TREATMENT HisTreatment)
        {
            HIS_TRACKING result = new HIS_TRACKING();
            try
            {
                if (HisTreatment != null)
                {
                }
            }
            catch (Exception ex)
            {
                result = new HIS_TRACKING();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void EventLogPrint()
        {
            try
            {
                string message = "In phiếu ra viện. Mã in : Mps000008" + "  TREATMENT_CODE: " + this.mps000008RDO.currentTreatment.TREATMENT_CODE + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
