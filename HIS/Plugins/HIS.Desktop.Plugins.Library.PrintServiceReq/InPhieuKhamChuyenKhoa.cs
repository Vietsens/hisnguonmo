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
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq
{
    class InPhieuKhamChuyenKhoa
    {
        public InPhieuKhamChuyenKhoa(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO,
            Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>> dicServiceReqData,
            Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>> dicSereServData,
            bool printNow, ref bool result, long? roomId, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType,
            Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint, List<HIS_TRANS_REQ> TransReq, List<HIS_CONFIG> Configs, Action<Inventec.Common.SignLibrary.DTO.DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned)
        {
            try
            {
                var lstServieReq_71 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH].ToList();
                if (lstServieReq_71 == null || lstServieReq_71.Count <= 0) return;

                var patientADO = PrintGlobalStore.GetPatientById(chiDinhDichVuADO.treament.PATIENT_ID);

                MPS.Processor.Mps000071.PDO.PatientADO patient = new MPS.Processor.Mps000071.PDO.PatientADO(patientADO);

                foreach (var serviceReq in lstServieReq_71)
                {
                    List<V_HIS_SERE_SERV> lstSereServ = new List<V_HIS_SERE_SERV>();
                    if (dicSereServData != null && dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        lstSereServ = dicSereServData[serviceReq.ID];
                    }

                    V_HIS_SERE_SERV sereServExamSerivceReq = new V_HIS_SERE_SERV();

                    List<MPS.Processor.Mps000071.PDO.ExeSereServSdo> sereServExamServiceReqs = new List<MPS.Processor.Mps000071.PDO.ExeSereServSdo>();
                    if (lstSereServ != null && lstSereServ.Count > 0)
                    {
                        sereServExamServiceReqs = (from m in lstSereServ select new MPS.Processor.Mps000071.PDO.ExeSereServSdo(m, serviceReq)).ToList();
                        sereServExamSerivceReq = lstSereServ.FirstOrDefault();
                    }

                    V_HIS_SERVICE_REQ serviceReqPre = new V_HIS_SERVICE_REQ();
                    if (serviceReq != null && serviceReq.PREVIOUS_SERVICE_REQ_ID != null)
                    {
                        serviceReqPre = GetPreviousServiceReq(serviceReq.PREVIOUS_SERVICE_REQ_ID);
                    }
                    V_HIS_TREATMENT_BED_ROOM treatmentBedRoom = GetTreatmentBedRoom(chiDinhDichVuADO.treament);

                    List<V_HIS_TRANSACTION> transaction = null;
                    if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                    {
                        List<long> transactionIds = new List<long>();
                        if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                        {
                            transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => sereServExamServiceReqs.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                        }

                        if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                        {
                            transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => sereServExamServiceReqs.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                        }

                        transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                    }

                    HIS_DHST hisDhst = new HIS_DHST();
                    if (serviceReq != null && serviceReq.DHST_ID != null)
                    {
                        hisDhst = GetDhst(serviceReq.DHST_ID);
                    }
                    else if (serviceReq != null && serviceReq.PREVIOUS_SERVICE_REQ_ID != null)
                    {
                        if (serviceReqPre != null && serviceReqPre.DHST_ID != null)
                        {
                            hisDhst = GetDhst(serviceReqPre.DHST_ID);
                        }
                    }

                    MPS.Processor.Mps000071.PDO.Mps000071PDO mps000071RDO = new MPS.Processor.Mps000071.PDO.Mps000071PDO(
                        patient,
                        chiDinhDichVuADO.patientTypeAlter,
                        sereServExamServiceReqs,
                        sereServExamSerivceReq,
                        serviceReq,
                        serviceReqPre,
                        chiDinhDichVuADO.FirstExamRoomName,
                        chiDinhDichVuADO.Ratio,
                        chiDinhDichVuADO.ListSereServDeposit,
                        chiDinhDichVuADO.ListSereServBill,
                        transaction,
                        hisDhst,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                        Configs,
                        treatmentBedRoom,
                        chiDinhDichVuADO.treament);
                    Print.PrintData(printTypeCode, fileName, mps000071RDO, printNow, ref result, roomId, false, PreviewType, lstSereServ.Count, savedData, serviceReq.TREATMENT_CODE, DlgSendResultSigned);
                }
            }
            catch (Exception ex)
            {
                cancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_SERVICE_REQ GetPreviousServiceReq(long? previousServiceReqId)
        {
            V_HIS_SERVICE_REQ result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = previousServiceReqId;
                var apiResult = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    result = apiResult.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return result;
        }
        private V_HIS_TREATMENT_BED_ROOM GetTreatmentBedRoom(HIS_TREATMENT treatment)
        {
            V_HIS_TREATMENT_BED_ROOM result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentBedRoomViewFilter filter = new HisTreatmentBedRoomViewFilter();
                filter.TREATMENT_ID = treatment.ID;
                var apiResult = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    result = apiResult.Where(o => o.DEPARTMENT_ID == treatment.LAST_DEPARTMENT_ID).OrderByDescending(o => o.ADD_TIME).ThenByDescending(o => o.ID).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return result;
        }
        private HIS_DHST GetDhst(long? dhstID)
        {
            HIS_DHST result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisDhstFilter filter = new HisDhstFilter();
                filter.ID = dhstID;
                var apiResult = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    result = apiResult.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return result;
        }
    }
}
