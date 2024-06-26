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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using HIS.Desktop.Print;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Plugins.AllocateExecuteRoom.ADO;
using System.Threading;
using AutoMapper;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.SDO;
namespace HIS.Desktop.Plugins.AllocateExecuteRoom
{
    public partial class UCAllocateExecuteRoom : UserControlBase
    {
        private void InPhieuYeuCauDichVu(string printTypeCode)
        {
            try
            {
                if (serviceReqPrintRaw != null)
                {
                    CommonParam param = new CommonParam();
                    ThreadChiDinhDichVuADO data = new ThreadChiDinhDichVuADO(this.resultPrint);
                    CreateThreadLoadDataForService(data);

                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
                    LoadCurrentPatientTypeAlter(this.resultPrint.TREATMENT_ID, ref patientTypeAlter);
                    data.vHisPatientTypeAlter = patientTypeAlter;

                    HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                    HisServiceReqSDO.SereServs = data.listVHisSereServ;
                    HisServiceReqSDO.ServiceReqs = new List<V_HIS_SERVICE_REQ>() { this.serviceReqPrintRaw };
                    HisServiceReqSDO.SereServBills = data.ListSereServBill;
                    HisServiceReqSDO.SereServDeposits = data.ListSereServDeposit;

                    List<V_HIS_BED_LOG> listBedLogs = new List<V_HIS_BED_LOG>();

                    HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                    bedLogFilter.TREATMENT_ID = serviceReqPrintRaw.TREATMENT_ID;
                    var resultBedlog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
                    if (resultBedlog != null)
                    {
                        listBedLogs = resultBedlog;
                    }

                    HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(HisTreatment, data.hisTreatment);

                    if (data.vHisPatientTypeAlter != null)
                    {
                        HisTreatment.PATIENT_TYPE_CODE = data.vHisPatientTypeAlter.PATIENT_TYPE_CODE;
                        HisTreatment.HEIN_CARD_FROM_TIME = data.vHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                        HisTreatment.HEIN_CARD_NUMBER = data.vHisPatientTypeAlter.HEIN_CARD_NUMBER;
                        HisTreatment.HEIN_CARD_TO_TIME = data.vHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                        HisTreatment.HEIN_MEDI_ORG_CODE = data.vHisPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                        HisTreatment.LEVEL_CODE = data.vHisPatientTypeAlter.LEVEL_CODE;
                        HisTreatment.RIGHT_ROUTE_CODE = data.vHisPatientTypeAlter.RIGHT_ROUTE_CODE;
                        HisTreatment.RIGHT_ROUTE_TYPE_CODE = data.vHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                        HisTreatment.TREATMENT_TYPE_CODE = data.vHisPatientTypeAlter.TREATMENT_TYPE_CODE;
                        HisTreatment.HEIN_CARD_ADDRESS = data.vHisPatientTypeAlter.ADDRESS;
                    }

                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, listBedLogs, currentModule != null ? currentModule.RoomId : 0);
                    PrintServiceReqProcessor.Print(printTypeCode, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void CreateThreadLoadDataForService(ThreadChiDinhDichVuADO data)
        {
            Thread threadTreatment = new Thread(new ParameterizedThreadStart(LoadDataTreatment));
            Thread threadSereServ = new Thread(new ParameterizedThreadStart(LoadDataSereServ));
            Thread threadSereServBill = new Thread(new ParameterizedThreadStart(LoadDataSereServBill));
            Thread threadSereServDeposit = new Thread(new ParameterizedThreadStart(LoadDataSereServDeposit));

            try
            {
                threadTreatment.Start(data);
                threadSereServ.Start(data);
                threadSereServBill.Start(data);
                threadSereServDeposit.Start(data);

                threadTreatment.Join();
                threadSereServ.Join();
                threadSereServBill.Join();
                threadSereServDeposit.Join();
            }
            catch (Exception ex)
            {
                threadTreatment.Abort();
                threadSereServ.Abort();
                threadSereServBill.Abort();
                threadSereServDeposit.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatment(object data)
        {
            try
            {
                LoadThreadDataTreatment((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataTreatment(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    data.hisTreatment = getTreatment(data.vHisServiceReq2Print.TREATMENT_ID);

                    data.vHisPatientTypeAlter = getPatientTypeAlter(data.vHisServiceReq2Print.TREATMENT_ID, 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private HIS_TREATMENT getTreatment(long treatmentId)
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin get HIS_TREATMENT");
            CommonParam param = new CommonParam();
            HIS_TREATMENT currentHisTreatment = new HIS_TREATMENT();
            try
            {
                MOS.Filter.HisTreatmentFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentFilter();
                hisTreatmentFilter.ID = treatmentId;
                var treatments = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, hisTreatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (treatments != null && treatments.Count > 0)
                {
                    currentHisTreatment = treatments.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("End get HIS_TREATMENT");
            return currentHisTreatment;
        }

        private V_HIS_PATIENT_TYPE_ALTER getPatientTypeAlter(long treatmentId, long instructTime)
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin get HispatientTypeAlter");
            CommonParam param = new CommonParam();
            MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
            try
            {
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter hisPTAlterFilter = new HisPatientTypeAlterViewAppliedFilter();
                hisPTAlterFilter.TreatmentId = treatmentId;
                if (instructTime > 0)
                    hisPTAlterFilter.InstructionTime = instructTime;
                else
                    hisPTAlterFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                currentHispatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, hisPTAlterFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
            }
            catch (Exception ex)
            {
                currentHispatientTypeAlter = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("End get HispatientTypeAlter");
            return currentHispatientTypeAlter;
        }

        private void LoadDataSereServ(object data)
        {
            try
            {
                LoadThreadDataSereServ((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServ(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    data.listVHisSereServ = GetSereServByServiceReqId(data.vHisServiceReq2Print.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_SERE_SERV> GetSereServByServiceReqId(long serviceReqId)
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin get List<V_HIS_SERE_SERV>");
            List<V_HIS_SERE_SERV> result = new List<V_HIS_SERE_SERV>();
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.SERVICE_REQ_ID = serviceReqId;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null && apiResult.Count > 0)
                {
                    apiResult = apiResult.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                    foreach (var item in apiResult)
                    {
                        V_HIS_SERE_SERV ss11 = new V_HIS_SERE_SERV();
                        Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_SERE_SERV, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                        ss11 = Mapper.Map<MOS.EFMODEL.DataModels.HIS_SERE_SERV, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>(item);

                        var service = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().FirstOrDefault(o => o.ID == ss11.SERVICE_ID);
                        if (service != null)
                        {
                            ss11.TDL_SERVICE_CODE = service.SERVICE_CODE;
                            ss11.TDL_SERVICE_NAME = service.SERVICE_NAME;
                            ss11.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                            ss11.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                            ss11.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                            ss11.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                            ss11.HEIN_SERVICE_TYPE_CODE = service.HEIN_SERVICE_TYPE_CODE;
                            ss11.HEIN_SERVICE_TYPE_NAME = service.HEIN_SERVICE_TYPE_NAME;
                            ss11.HEIN_SERVICE_TYPE_NUM_ORDER = service.HEIN_SERVICE_TYPE_NUM_ORDER;
                        }

                        var executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.TDL_EXECUTE_ROOM_ID);
                        if (executeRoom != null)
                        {
                            ss11.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                            ss11.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                            ss11.EXECUTE_DEPARTMENT_CODE = executeRoom.DEPARTMENT_CODE;
                            ss11.EXECUTE_DEPARTMENT_NAME = executeRoom.DEPARTMENT_NAME;
                        }

                        var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.TDL_REQUEST_ROOM_ID);
                        if (reqRoom != null)
                        {
                            ss11.REQUEST_DEPARTMENT_CODE = reqRoom.DEPARTMENT_CODE;
                            ss11.REQUEST_DEPARTMENT_NAME = reqRoom.DEPARTMENT_NAME;
                            ss11.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                            ss11.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                        }

                        var patientTpye = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == item.PATIENT_TYPE_ID);
                        if (patientTpye != null)
                        {
                            ss11.PATIENT_TYPE_CODE = patientTpye.PATIENT_TYPE_CODE;
                            ss11.PATIENT_TYPE_NAME = patientTpye.PATIENT_TYPE_NAME;
                        }
                        result.Add(ss11);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_SERE_SERV>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("End get List<V_HIS_SERE_SERV>");
            return result;
        }

        private void LoadDataSereServBill(object data)
        {
            try
            {
                LoadThreadDataSereServBill((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServBill(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    CommonParam paramCommon = new CommonParam();
                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.TDL_TREATMENT_ID = data.vHisServiceReq2Print.TREATMENT_ID;
                    ssBillFilter.IS_NOT_CANCEL = true;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        data.ListSereServBill = apiResult;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServDeposit(object data)
        {
            try
            {
                LoadThreadDataSereServDeposit((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServDeposit(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    List<HIS_SERE_SERV_DEPOSIT> ssDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                    List<HIS_SESE_DEPO_REPAY> ssRepay = new List<HIS_SESE_DEPO_REPAY>();

                    CommonParam paramCommon = new CommonParam();
                    HisSereServDepositFilter ssDepositFilter = new HisSereServDepositFilter();
                    ssDepositFilter.TDL_TREATMENT_ID = data.vHisServiceReq2Print.TREATMENT_ID;
                    ssDepositFilter.IS_CANCEL = false;
                    var apiDepositResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, ssDepositFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiDepositResult != null && apiDepositResult.Count > 0)
                    {
                        ssDeposit = apiDepositResult;
                    }

                    HisSeseDepoRepayFilter ssRepayFilter = new HisSeseDepoRepayFilter();
                    ssRepayFilter.TDL_TREATMENT_ID = data.vHisServiceReq2Print.TREATMENT_ID;
                    ssRepayFilter.IS_CANCEL = false;
                    var apiRepayResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumers.MosConsumer, ssRepayFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiRepayResult != null && apiRepayResult.Count > 0)
                    {
                        ssRepay = apiRepayResult;
                    }

                    data.ListSereServDeposit = ssDeposit.Where(o => !ssRepay.Exists(e => e.SERE_SERV_DEPOSIT_ID == o.ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadCurrentPatientTypeAlter(long treatmentId, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();
                hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
