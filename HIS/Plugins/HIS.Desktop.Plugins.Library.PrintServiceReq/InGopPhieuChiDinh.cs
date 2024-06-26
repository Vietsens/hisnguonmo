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
using HIS.Desktop.LocalStorage.HisConfig;
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
    class InGopPhieuChiDinh
    {
        public InGopPhieuChiDinh(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO,
            Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>> dicServiceReqData,
            Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>> dicSereServData,
            Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs,
            bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType,
            List<HisServiceReqMaxNumOrderSDO> ReqMaxNumOrderSDO, Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint, Action<DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned)
        {
            try
            {
                var typeCodeSplit = HisConfigs.Get<string>(Config.CONFIG_KEY__AssignServicePrint_CODE);
                List<HIS_SERVICE_REQ_TYPE> listTypeSplit = new List<HIS_SERVICE_REQ_TYPE>();
                if (!string.IsNullOrWhiteSpace(typeCodeSplit))
                {
                    var codes = typeCodeSplit.Split(',');
                    foreach (var item in codes)
                    {
                        var ReqType = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(o => o.SERVICE_REQ_TYPE_CODE == item);
                        if (ReqType != null)
                        {
                            listTypeSplit.Add(ReqType);
                        }
                    }
                }

                HIS_DHST _HIS_DHST = chiDinhDichVuADO._HIS_DHST;
                HIS_WORK_PLACE _WORK_PLACE = chiDinhDichVuADO._WORK_PLACE;

                MPS.Processor.Mps000340.PDO.Mps000340ADO Mps000340ADO = new MPS.Processor.Mps000340.PDO.Mps000340ADO();
                Mps000340ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                Mps000340ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                Mps000340ADO.ratio = chiDinhDichVuADO.Ratio;
                Mps000340ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ> listServiceReq = new List<V_HIS_SERVICE_REQ>();
                List<MPS.Processor.Mps000340.PDO.ServiceType> type = new List<MPS.Processor.Mps000340.PDO.ServiceType>();
                List<MPS.Processor.Mps000340.PDO.SereServADO> sereServ = new List<MPS.Processor.Mps000340.PDO.SereServADO>();

                V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();

                int num = 1;
                foreach (var lstServiceReq in dicServiceReqData)
                {
                    long svtId = 0;

                    //đổi sang loại dịch vụ để làm qr code
                    if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN;
                    }
                    else if (lstServiceReq.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN)
                    {
                        svtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN;
                    }

                    var svt = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == svtId);
                    if (svt == null) continue;

                    foreach (var req in lstServiceReq.Value)
                    {
                        if (listTypeSplit.Exists(o => o.ID == lstServiceReq.Key))
                        {
                            Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                            foreach (var item in dicSereServData[req.ID])
                            {
                                var service = ProcessDictionaryData.GetService(item.SERVICE_ID);
                                if (service != null)
                                {
                                    if (!dicSereServTest.ContainsKey(service.PARENT_ID ?? 0))
                                        dicSereServTest[service.PARENT_ID ?? 0] = new List<V_HIS_SERE_SERV>();
                                    dicSereServTest[service.PARENT_ID ?? 0].Add(item);
                                }
                                else
                                {
                                    if (dicSereServTest[0] == null)
                                        dicSereServTest[0] = new List<V_HIS_SERE_SERV>();
                                    dicSereServTest[0].Add(item);
                                }
                            }

                            foreach (var item in dicSereServTest)
                            {
                                MPS.Processor.Mps000340.PDO.ServiceType tp = new MPS.Processor.Mps000340.PDO.ServiceType();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000340.PDO.ServiceType>(tp, svt);
                                tp.SERVICE_REQ_ID = req.ID;
                                tp.SERVICE_TYPE_GROUP_ID = num;
                                List<long> _ssIds = item.Value.Select(p => p.SERVICE_ID).Distinct().ToList();
                                var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                                if (dataSS != null && dataSS.Count > 0)
                                {
                                    var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                                    if (_service != null)
                                    {
                                        var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                        tp.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                                    }
                                }

                                foreach (var sere in item.Value)
                                {
                                    var se = new MPS.Processor.Mps000340.PDO.SereServADO(sere);
                                    var service = ProcessDictionaryData.GetService(se.SERVICE_ID);
                                    se.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                    se.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                    if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                    {
                                        se.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                        se.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                        se.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                        se.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                        se.NOTE = dicSereServExtData[sere.ID].NOTE;
                                    }

                                    se.SERVICE_TYPE_GROUP_ID = tp.SERVICE_TYPE_GROUP_ID;
                                    sereServ.Add(se);
                                }

                                type.Add(tp);
                                num++;
                            }

                            listServiceReq.Add(req);
                        }
                        else
                        {
                            MPS.Processor.Mps000340.PDO.ServiceType tp = new MPS.Processor.Mps000340.PDO.ServiceType();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000340.PDO.ServiceType>(tp, svt);
                            tp.SERVICE_REQ_ID = req.ID;
                            tp.SERVICE_TYPE_GROUP_ID = num;

                            List<long> _ssIds = dicSereServData[req.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                            var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                            if (dataSS != null && dataSS.Count > 0)
                            {
                                var _service = dataSS.OrderBy(o => o.SERVICE_CODE).FirstOrDefault(p => p.PARENT_ID != null);
                                if (_service != null)
                                {
                                    var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                    tp.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                                }
                            }

                            foreach (var sere in dicSereServData[req.ID])
                            {
                                var se = new MPS.Processor.Mps000340.PDO.SereServADO(sere);
                                var service = ProcessDictionaryData.GetService(se.SERVICE_ID);
                                se.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                se.SERVICE_TYPE_GROUP_ID = tp.SERVICE_TYPE_GROUP_ID;
                                sereServ.Add(se);
                            }

                            listServiceReq.Add(req);
                            type.Add(tp);

                            num++;
                        }
                    }
                }

                if (bedLogs != null && bedLogs.Count > 0)
                {
                    var IntructionTime = listServiceReq.Min(o => o.INTRUCTION_TIME);
                    bedLog = bedLogs.Where(o => o.START_TIME <= IntructionTime).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                }

                var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == listServiceReq.First().REQUEST_LOGINNAME);
                Mps000340ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                MPS.Processor.Mps000340.PDO.Mps000340PDO mps000340PDO = new MPS.Processor.Mps000340.PDO.Mps000340PDO(
                    chiDinhDichVuADO.treament,
                    listServiceReq,
                    sereServ,
                    chiDinhDichVuADO.patientTypeAlter,
                    Mps000340ADO,
                    bedLog,
                    _HIS_DHST,
                    _WORK_PLACE,
                    type,
                    ReqMaxNumOrderSDO
                    );

                Print.PrintData(printTypeCode, fileName, mps000340PDO, printNow, ref result, roomId, isView, PreviewType, sereServ.Count, savedData, chiDinhDichVuADO.treament.TREATMENT_CODE, DlgSendResultSigned);
            }
            catch (Exception ex)
            {
                cancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
