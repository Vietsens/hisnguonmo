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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private void InPhieuYeuCauDichVu(string printTypeCode)
        {
            try
            {
                HisServiceReqListResultSDO serviceReqComboResultSDO = new HisServiceReqListResultSDO();
                if (HisServiceReqResult.AdditionExamResult != null)
                {
                    serviceReqComboResultSDO.ServiceReqs = new List<V_HIS_SERVICE_REQ> { HisServiceReqResult.AdditionExamResult };
                }

                if (HisServiceReqResult.Transaction != null)
                {
                    serviceReqComboResultSDO.Transactions = new List<V_HIS_TRANSACTION> { HisServiceReqResult.Transaction };
                }

                serviceReqComboResultSDO.SereServDeposits = HisServiceReqResult.SereServDeposits;

                HisTreatmentWithPatientTypeInfoSDO currentHisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                V_HIS_SERE_SERV sereServExam = new V_HIS_SERE_SERV();
                List<Action> methods = new List<Action>();
                methods.Add(() => { currentHisTreatment = LoadTreatmentWithPatientType(); });
                methods.Add(() => { sereServExam = SereServExam(); });
                ThreadCustomManager.MultipleThreadWithJoin(methods);

                if (sereServExam != null)
                {
                    serviceReqComboResultSDO.SereServs = new List<V_HIS_SERE_SERV> { sereServExam };
                }

                if (this.isPrintExamServiceAdd && !this.isSignExamServiceAdd)
                {
                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, null, moduleData != null ? moduleData.RoomId : 0, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow);
                    PrintServiceReqProcessor.Print(printTypeCode);
                }
                else if (this.isPrintExamServiceAdd && this.isSignExamServiceAdd)
                {
                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, null, moduleData != null ? moduleData.RoomId : 0, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow);
                    PrintServiceReqProcessor.Print(printTypeCode);

                }
                else if (!this.isPrintExamServiceAdd && this.isSignExamServiceAdd)
                {
                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, null, moduleData != null ? moduleData.RoomId : 0, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow);
                    PrintServiceReqProcessor.Print(printTypeCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_SERE_SERV SereServExam()
        {
            V_HIS_SERE_SERV sereServV = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServFilter filter = new HisSereServFilter();
                filter.SERVICE_REQ_ID = HisServiceReqResult.AdditionExamResult.ID;
                HIS_SERE_SERV sereServ = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (sereServ != null)
                {
                    sereServV = new V_HIS_SERE_SERV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServV, sereServ);
                    var service = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServV.SERVICE_ID);
                    if (service != null)
                    {
                        sereServV.TDL_SERVICE_CODE = service.SERVICE_CODE;
                        sereServV.TDL_SERVICE_NAME = service.SERVICE_NAME;
                        sereServV.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                        sereServV.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                        sereServV.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                        sereServV.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                        sereServV.HEIN_SERVICE_TYPE_CODE = service.HEIN_SERVICE_TYPE_CODE;
                        sereServV.HEIN_SERVICE_TYPE_NAME = service.HEIN_SERVICE_TYPE_NAME;
                        sereServV.HEIN_SERVICE_TYPE_NUM_ORDER = service.HEIN_SERVICE_TYPE_NUM_ORDER;
                    }

                    var executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == sereServV.TDL_EXECUTE_ROOM_ID);
                    if (executeRoom != null)
                    {
                        sereServV.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                        sereServV.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                        sereServV.EXECUTE_DEPARTMENT_CODE = executeRoom.DEPARTMENT_CODE;
                        sereServV.EXECUTE_DEPARTMENT_NAME = executeRoom.DEPARTMENT_NAME;
                    }

                    var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == sereServV.TDL_REQUEST_ROOM_ID);
                    if (reqRoom != null)
                    {
                        sereServV.REQUEST_DEPARTMENT_CODE = reqRoom.DEPARTMENT_CODE;
                        sereServV.REQUEST_DEPARTMENT_NAME = reqRoom.DEPARTMENT_NAME;
                        sereServV.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                        sereServV.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                    }

                    var patientTpye = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == sereServV.PATIENT_TYPE_ID);
                    if (patientTpye != null)
                    {
                        sereServV.PATIENT_TYPE_CODE = patientTpye.PATIENT_TYPE_CODE;
                        sereServV.PATIENT_TYPE_NAME = patientTpye.PATIENT_TYPE_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                sereServV = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServV;
        }

        private HisTreatmentWithPatientTypeInfoSDO LoadTreatmentWithPatientType()
        {
            HisTreatmentWithPatientTypeInfoSDO result = null;
            try
            {
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                var hisTreatments = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                if (hisTreatments != null && hisTreatments.Count > 0)
                {
                    result = hisTreatments.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}
