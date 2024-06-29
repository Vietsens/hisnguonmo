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
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq
{
    class InGopXetNghiem
    {
        List<V_HIS_SERVICE_REQ> ServiceReq = new List<V_HIS_SERVICE_REQ>();
        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV>>();

        //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
        //cần xử lý để gộp file đủ 
        public InGopXetNghiem(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO,
            Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData,
            Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData,
            Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData,
            List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView,
            MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, List<HisServiceReqMaxNumOrderSDO> ReqMaxNumOrderSDO,
            bool isMethodSaveNPrint, Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint, Action<Inventec.Common.SignLibrary.DTO.DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned)
        {
            try
            {
                var lstServieReq_XN = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                if (lstServieReq_XN != null && lstServieReq_XN.Count > 0)
                {
                    ServiceReq.AddRange(lstServieReq_XN);

                    foreach (var item in dicSereServData)
                    {
                        dicSereServ.Add(item.Key, item.Value);
                    }

                    if (!isMethodSaveNPrint && lstServieReq_XN.Count == 1)
                    {
                        CreateThreadGetData(lstServieReq_XN);
                    }

                    MPS.Processor.Mps000432.PDO.Mps000432ADO Mps000432ADO = new MPS.Processor.Mps000432.PDO.Mps000432ADO();
                    Mps000432ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                    Mps000432ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                    Mps000432ADO.ratio = chiDinhDichVuADO.Ratio;
                    Mps000432ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                    HIS_DHST _HIS_DHST = chiDinhDichVuADO._HIS_DHST;
                    HIS_WORK_PLACE _WORK_PLACE = chiDinhDichVuADO._WORK_PLACE;

                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();

                    List<long> sereServPaidIds = new List<long>();
                    if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                    {
                        sereServPaidIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o=>ServiceReq.Exists(p=>p.ID == o.TDL_SERVICE_REQ_ID)).Select(s => s.SERE_SERV_ID).ToList());
                    }
                    if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                    {
                        sereServPaidIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => ServiceReq.Exists(p => p.ID == o.TDL_SERVICE_REQ_ID)).Select(s => s.SERE_SERV_ID).ToList());
                    }
                    sereServPaidIds = sereServPaidIds.Distinct().ToList();

                    List<V_HIS_SERE_SERV> listSereServCurrentPrint = new List<V_HIS_SERE_SERV>();
                    foreach (var item in lstServieReq_XN)
                    {
                        if (dicSereServ.ContainsKey(item.ID))
                        {
                            listSereServCurrentPrint.AddRange(dicSereServ[item.ID]);
                        }
                    }

                    var groupExecuteDepartment = ServiceReq.GroupBy(o => o.EXECUTE_DEPARTMENT_ID).OrderBy(o => o.Key).ToList();
                    int cgr = 0;

                    foreach (var req in groupExecuteDepartment)
                    {
                        cgr++;
                        int countss = 0;
                        //in nhóm cuối sẽ gán tất cả số lượng
                        if (groupExecuteDepartment.Count == cgr)
                        {
                            countss = listSereServCurrentPrint.Count;
                        }

                        List<V_HIS_SERE_SERV> listSereServ = new List<V_HIS_SERE_SERV>();
                        foreach (var item in req)
                        {
                            if (dicSereServ.ContainsKey(item.ID))
                            {
                                listSereServ.AddRange(dicSereServ[item.ID]);
                            }
                        }

                        var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == req.First().REQUEST_LOGINNAME);
                        Mps000432ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        if (bedLogs != null && bedLogs.Count > 0)
                        {
                            var IntructionTime = req.Min(o => o.INTRUCTION_TIME);
                            bedLog = bedLogs.Where(o => o.START_TIME <= IntructionTime).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                        }
                        List<V_HIS_SERE_SERV> listSereServPaid = new List<V_HIS_SERE_SERV>();
                        List<V_HIS_SERE_SERV> listSereServNoPaid = new List<V_HIS_SERE_SERV>();
                        //gom theo khoa nhưng tách theo thanh toán

                        Inventec.Common.Logging.LogSystem.Debug("INGOPXETNGHIEM___________"+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listSereServ.Select(o=>new {o.ID,o.VIR_TOTAL_PATIENT_PRICE})), listSereServ.Select(o=>new { o.ID, o.VIR_TOTAL_PATIENT_PRICE })));

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServPaidIds), sereServPaidIds));
                        if (sereServPaidIds != null && sereServPaidIds.Count > 0)
                        {
                            listSereServPaid = listSereServ.Where(o => sereServPaidIds.Contains(o.ID) || (o.VIR_TOTAL_PATIENT_PRICE ?? 0)== 0).ToList();
                            listSereServNoPaid = listSereServ.Where(o => !sereServPaidIds.Contains(o.ID) && (o.VIR_TOTAL_PATIENT_PRICE ?? 0)!= 0).ToList();
                        }else
                        {
                            listSereServNoPaid = listSereServ.ToList();
                        }

                        if (listSereServPaid != null && listSereServPaid.Count > 0)
                        {
                            int countssp = 0;
                            if (countss > 0)
                            {
                                if (listSereServNoPaid != null && listSereServNoPaid.Count > 0)
                                {
                                    countssp = 1;
                                    countss--;
                                }
                                else
                                {
                                    countssp = countss;
                                    countss = 0;
                                }
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => listSereServPaid.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => listSereServPaid.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            List<V_HIS_SERVICE_REQ> reqPaid = req.Where(o => listSereServPaid.Exists(e => e.SERVICE_REQ_ID == o.ID)).ToList();

                            MPS.Processor.Mps000432.PDO.Mps000432PDO mps000432PDO = new MPS.Processor.Mps000432.PDO.Mps000432PDO(
                                chiDinhDichVuADO.treament,
                                chiDinhDichVuADO.patientTypeAlter,
                                reqPaid,
                                listSereServPaid,
                                Mps000432ADO,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                BackendDataWorker.Get<HIS_SERVICE_CONDITION>(),
                                ReqMaxNumOrderSDO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction
                                );

                            Print.PrintData(printTypeCode, fileName, mps000432PDO, printNow, ref result, roomId, isView, PreviewType, countssp, savedData, chiDinhDichVuADO.treament.TREATMENT_CODE, DlgSendResultSigned);
                        }

                        if (listSereServNoPaid != null && listSereServNoPaid.Count > 0)
                        {
                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => listSereServNoPaid.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => listSereServNoPaid.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            List<V_HIS_SERVICE_REQ> reqNoPaid = req.Where(o => listSereServNoPaid.Exists(e => e.SERVICE_REQ_ID == o.ID)).ToList();

                            MPS.Processor.Mps000432.PDO.Mps000432PDO mps000432PDO = new MPS.Processor.Mps000432.PDO.Mps000432PDO(
                                chiDinhDichVuADO.treament,
                                chiDinhDichVuADO.patientTypeAlter,
                                reqNoPaid,
                                listSereServNoPaid,
                                Mps000432ADO,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                BackendDataWorker.Get<HIS_SERVICE_CONDITION>(),
                                ReqMaxNumOrderSDO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction
                                );

                            Print.PrintData(printTypeCode, fileName, mps000432PDO, printNow, ref result, roomId, isView, PreviewType, countss, savedData, chiDinhDichVuADO.treament.TREATMENT_CODE, DlgSendResultSigned);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadGetData(List<V_HIS_SERVICE_REQ> lstServieReq_XN)
        {
            try
            {
                List<V_HIS_SERVICE_REQ> lstData = new List<V_HIS_SERVICE_REQ>();
                foreach (var item in lstServieReq_XN)
                {
                    var serviceReq = GetServiceReqByAssignTurn(item);
                    lstData.AddRange(serviceReq);
                }

                lstData = lstData.GroupBy(o => o.ID).Select(s => s.First()).ToList();
                if (lstData != null && lstData.Count > 0)
                {
                    ServiceReq = lstData;
                    var sereServ = GetSereServByServiceReqId(lstData.Select(s => s.ID).ToList());
                    if (sereServ != null && sereServ.Count > 0)
                    {
                        var groupss = sereServ.GroupBy(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        foreach (var item in groupss)
                        {
                            dicSereServ[item.Key] = item.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_SERVICE_REQ> GetServiceReqByAssignTurn(V_HIS_SERVICE_REQ serviceReqPrintRaw)
        {
            List<V_HIS_SERVICE_REQ> result = new List<V_HIS_SERVICE_REQ>();
            try
            {
                HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                reqFilter.TREATMENT_ID = serviceReqPrintRaw.TREATMENT_ID;
                reqFilter.EXECUTE_DEPARTMENT_ID = serviceReqPrintRaw.EXECUTE_DEPARTMENT_ID;
                reqFilter.SERVICE_REQ_TYPE_ID = serviceReqPrintRaw.SERVICE_REQ_TYPE_ID;
                if (!String.IsNullOrWhiteSpace(serviceReqPrintRaw.ASSIGN_TURN_CODE))
                {
                    reqFilter.ASSIGN_TURN_CODE__EXACT = serviceReqPrintRaw.ASSIGN_TURN_CODE;
                }
                else
                {
                    reqFilter.INTRUCTION_TIME_FROM = serviceReqPrintRaw.INTRUCTION_TIME;
                    reqFilter.INTRUCTION_TIME_TO = serviceReqPrintRaw.INTRUCTION_TIME;
                }

                CommonParam param = new CommonParam();
                var dataServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, reqFilter, param);
                if (dataServiceReq != null && dataServiceReq.Count > 0)
                {
                    foreach (var item in dataServiceReq)
                    {
                        V_HIS_SERVICE_REQ req = new V_HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(req, item);

                        if (item.ASSIGN_REASON_ID.HasValue)
                        {
                            var assignReason = BackendDataWorker.Get<HIS_ASSIGN_REASON>().FirstOrDefault(o => o.ID == item.ASSIGN_REASON_ID);
                            if (assignReason != null)
                            {
                                req.ASSIGN_REASON_CODE = assignReason.ASSIGN_REASON_CODE;
                                req.ASSIGN_REASON_NAME = assignReason.ASSIGN_REASON_NAME;
                            }
                        }
                        if (item.SAMPLE_ROOM_ID.HasValue)
                        {
                            var sampleRoom = BackendDataWorker.Get<HIS_SAMPLE_ROOM>().FirstOrDefault(o => o.ID == item.SAMPLE_ROOM_ID);
                            if (sampleRoom != null)
                            {
                                req.SAMPLE_ROOM_CODE = sampleRoom.SAMPLE_ROOM_CODE;
                                req.SAMPLE_ROOM_NAME = sampleRoom.SAMPLE_ROOM_NAME;
                            }
                        }

                        var stt = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().FirstOrDefault(o => o.ID == item.SERVICE_REQ_STT_ID);
                        if (stt != null)
                        {
                            req.SERVICE_REQ_STT_CODE = stt.SERVICE_REQ_STT_CODE;
                            req.SERVICE_REQ_STT_NAME = stt.SERVICE_REQ_STT_NAME;
                        }

                        var type = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(o => o.ID == item.SERVICE_REQ_TYPE_ID);
                        if (type != null)
                        {
                            req.SERVICE_REQ_TYPE_CODE = type.SERVICE_REQ_TYPE_CODE;
                            req.SERVICE_REQ_TYPE_NAME = type.SERVICE_REQ_TYPE_NAME;
                        }

                        var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == item.EXECUTE_DEPARTMENT_ID);
                        if (executeDepartment != null)
                        {
                            req.EXECUTE_DEPARTMENT_CODE = executeDepartment.DEPARTMENT_CODE;
                            req.EXECUTE_DEPARTMENT_NAME = executeDepartment.DEPARTMENT_NAME;
                        }

                        var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == item.REQUEST_DEPARTMENT_ID);
                        if (requestDepartment != null)
                        {
                            req.REQUEST_DEPARTMENT_CODE = requestDepartment.DEPARTMENT_CODE;
                            req.REQUEST_DEPARTMENT_NAME = requestDepartment.DEPARTMENT_NAME;
                        }

                        var executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.EXECUTE_ROOM_ID);
                        if (executeRoom != null)
                        {
                            req.EXECUTE_ROOM_ADDRESS = executeRoom.ADDRESS;
                            req.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                            req.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                        }

                        var requestRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.REQUEST_ROOM_ID);
                        if (requestRoom != null)
                        {
                            req.REQUEST_ROOM_ADDRESS = requestRoom.ADDRESS;
                            req.REQUEST_ROOM_CODE = requestRoom.ROOM_CODE;
                            req.REQUEST_ROOM_NAME = requestRoom.ROOM_NAME;
                            req.REQUEST_ROOM_TYPE_CODE = requestRoom.ROOM_TYPE_CODE;
                            req.REQUEST_ROOM_TYPE_NAME = requestRoom.ROOM_TYPE_NAME;
                        }

                        result.Add(req);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_SERE_SERV> GetSereServByServiceReqId(List<long> serviceReqIds)
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin get List<V_HIS_SERE_SERV>");
            List<V_HIS_SERE_SERV> result = new List<V_HIS_SERE_SERV>();
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.SERVICE_REQ_IDs = serviceReqIds;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null && apiResult.Count > 0)
                {
                    apiResult = apiResult.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                    foreach (var item in apiResult)
                    {
                        V_HIS_SERE_SERV ss11 = item;
                        //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(ss11, item);

                        //var service = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().FirstOrDefault(o => o.ID == ss11.SERVICE_ID);
                        //if (service != null)
                        //{
                        //    ss11.TDL_SERVICE_CODE = service.SERVICE_CODE;
                        //    ss11.TDL_SERVICE_NAME = service.SERVICE_NAME;
                        //    ss11.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                        //    ss11.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                        //    ss11.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                        //    ss11.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                        //    ss11.HEIN_SERVICE_TYPE_CODE = service.HEIN_SERVICE_TYPE_CODE;
                        //    ss11.HEIN_SERVICE_TYPE_NAME = service.HEIN_SERVICE_TYPE_NAME;
                        //    ss11.HEIN_SERVICE_TYPE_NUM_ORDER = service.HEIN_SERVICE_TYPE_NUM_ORDER;
                        //}

                        //var executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.TDL_EXECUTE_ROOM_ID);
                        //if (executeRoom != null)
                        //{
                        //    ss11.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                        //    ss11.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                        //    ss11.EXECUTE_DEPARTMENT_CODE = executeRoom.DEPARTMENT_CODE;
                        //    ss11.EXECUTE_DEPARTMENT_NAME = executeRoom.DEPARTMENT_NAME;
                        //}

                        //var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.TDL_REQUEST_ROOM_ID);
                        //if (reqRoom != null)
                        //{
                        //    ss11.REQUEST_DEPARTMENT_CODE = reqRoom.DEPARTMENT_CODE;
                        //    ss11.REQUEST_DEPARTMENT_NAME = reqRoom.DEPARTMENT_NAME;
                        //    ss11.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                        //    ss11.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                        //}

                        //var patientTpye = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == item.PATIENT_TYPE_ID);
                        //if (patientTpye != null)
                        //{
                        //    ss11.PATIENT_TYPE_CODE = patientTpye.PATIENT_TYPE_CODE;
                        //    ss11.PATIENT_TYPE_NAME = patientTpye.PATIENT_TYPE_NAME;
                        //}
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
    }
}
