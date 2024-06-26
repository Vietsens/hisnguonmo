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
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq
{
    class InCacPhieuChiDinh
    {
        HIS_DHST _HIS_DHST = new HIS_DHST();
        HIS_WORK_PLACE _WORK_PLACE = new HIS_WORK_PLACE();
        ADO.ChiDinhDichVuADO chiDinhDichVuADO;
        Dictionary<long, List<V_HIS_SERVICE_REQ>> dicReq;
        Dictionary<long, List<V_HIS_SERE_SERV>> dicSs;
        Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData;
        List<V_HIS_BED_LOG> bedLogs;
        List<LIS.EFMODEL.DataModels.V_LIS_SAMPLE> LisSamples;

        bool printNow;
        long? roomId;
        bool isView;
        MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType;
        List<HisServiceReqMaxNumOrderSDO> ReqMaxNumOrderSDO;
        bool IsMethodSaveNPrint;

        bool printCDHA;
        bool printTDCN;
        bool printXN;
        Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> SavedData;
        Action<DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned;
        Action<string> CancelPrint;
        Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        List<HIS_TRANS_REQ> TransReq;
        List<HIS_CONFIG> Configs;
        public InCacPhieuChiDinh(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO,
            Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>> dicServiceReqData,
            Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>> dicSereServData,
            Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs,
            bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType,
            List<HisServiceReqMaxNumOrderSDO> reqMaxNumOrderSDO, bool isMethodSaveNPrint, Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint, List<HIS_TRANS_REQ> TransReq, List<HIS_CONFIG> Configs, Action<DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned)
        {
            try
            {
                this.DlgSendResultSigned = DlgSendResultSigned;
                this.TransReq = TransReq;
                this.Configs = Configs;
                this.SavedData = savedData;
                this.CancelPrint = cancelPrint;
                this.IsMethodSaveNPrint = isMethodSaveNPrint;
                this.chiDinhDichVuADO = chiDinhDichVuADO;
                this.dicSereServExtData = dicSereServExtData;
                this.bedLogs = bedLogs;
                this.printNow = printNow;
                this.roomId = roomId;
                this.isView = isView;
                this.PreviewType = PreviewType;
                this.ReqMaxNumOrderSDO = reqMaxNumOrderSDO;
                this.richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                this.richEditorMain.SetActionCancelChooseTemplate(cancelPrint);

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

                _HIS_DHST = chiDinhDichVuADO._HIS_DHST;
                _WORK_PLACE = chiDinhDichVuADO._WORK_PLACE;

                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.IN__MPS000026__XET_NGHIEM:
                        bool printXN = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN);
                        XnPrint(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printXN);
                        break;
                    case PrintTypeCodeStore.IN__MPS000028__CHUAN_DOAN_HINH_ANH:
                        bool printCDHA = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA);
                        CdhaPrint(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printCDHA);
                        break;
                    case PrintTypeCodeStore.IN__MPS000029__NOI_SOI:
                        bool printNS = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS);
                        Mps000029Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printNS);
                        break;
                    case PrintTypeCodeStore.IN__MPS000030__SIEU_AM:
                        bool printSA = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA);
                        Mps000030Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printSA);
                        break;
                    case PrintTypeCodeStore.IN__MPS000031__THU_THUAT:
                        bool printTT = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT);
                        Mps000031Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printTT);
                        break;
                    case PrintTypeCodeStore.IN__MPS000036__PHAU_THUAT:
                        bool printPT = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT);
                        Mps000036Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printPT);
                        break;
                    case PrintTypeCodeStore.IN__MPS000038__THAM_DO_CHUC_NANG:
                        bool printTDCN = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN);
                        TdcnPrint(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printTDCN);
                        break;
                    case PrintTypeCodeStore.IN__MPS000040__DICH_VU_KHAC:
                        bool printKHAC = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC);
                        Mps000040Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printKHAC);
                        break;
                    case PrintTypeCodeStore.IN__MPS000042__GIUONG:
                        bool printG = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G);
                        Mps000042Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printG);
                        break;
                    case PrintTypeCodeStore.IN__MPS000053__PHUC_HOI_CHUC_NANG:
                        bool printPHCN = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN);
                        Mps000053Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printPHCN);
                        break;
                    case PrintTypeCodeStore.IN__MPS000167__GPBL:
                        bool printGPBL = listTypeSplit.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL);
                        Mps000167Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printGPBL);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                cancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TdcnPrint(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printTDCN)
        {
            try
            {
                this.printTDCN = printTDCN;
                if (HisConfigs.Get<string>(Config.OptionPrintDifferenceMps) == "1")
                {
                    var lstServieReq_38 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN].ToList();
                    foreach (var serviceReq in lstServieReq_38)
                    {
                        if (!dicSereServData.ContainsKey(serviceReq.ID))
                        {
                            Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                            continue;
                        }

                        dicReq = new Dictionary<long, List<V_HIS_SERVICE_REQ>>();
                        dicReq.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN, new List<V_HIS_SERVICE_REQ>() { serviceReq });
                        dicSs = new Dictionary<long, List<V_HIS_SERE_SERV>>();
                        dicSs.Add(serviceReq.ID, dicSereServData[serviceReq.ID]);

                        var listService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => dicSereServData[serviceReq.ID].Select(s => s.SERVICE_ID).Contains(o.ID)).ToList();
                        if (listService.Exists(o => o.FUEX_TYPE_ID.HasValue))
                        {
                            long fuexType = listService.FirstOrDefault(o => o.FUEX_TYPE_ID.HasValue).FUEX_TYPE_ID ?? 0;
                            switch (fuexType)
                            {
                                case IMSys.DbConfig.HIS_RS.HIS_FUEX_TYPE.ID__DT:
                                    this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000366__TDCN__DT, DelegateRunPrinter);
                                    break;
                                case IMSys.DbConfig.HIS_RS.HIS_FUEX_TYPE.ID__DN:
                                    this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000367__TDCN__DN, DelegateRunPrinter);
                                    break;
                                case IMSys.DbConfig.HIS_RS.HIS_FUEX_TYPE.ID__DC:
                                    this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000368__TDCN__DC, DelegateRunPrinter);
                                    break;
                                default:
                                    Mps000038Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printTDCN);
                                    break;
                            }
                        }
                        else
                        {
                            Mps000038Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printTDCN);
                        }
                    }
                }
                else
                {
                    Mps000038Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printTDCN);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CdhaPrint(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printCDHA)
        {
            try
            {
                this.printCDHA = printCDHA;
                if (HisConfigs.Get<string>(Config.OptionPrintDifferenceMps) == "1")
                {
                    var lstServieReq_28 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA].ToList();
                    foreach (var serviceReq in lstServieReq_28)
                    {
                        if (!dicSereServData.ContainsKey(serviceReq.ID))
                        {
                            Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                            continue;
                        }

                        dicReq = new Dictionary<long, List<V_HIS_SERVICE_REQ>>();
                        dicReq.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA, new List<V_HIS_SERVICE_REQ>() { serviceReq });
                        dicSs = new Dictionary<long, List<V_HIS_SERE_SERV>>();
                        dicSs.Add(serviceReq.ID, dicSereServData[serviceReq.ID]);

                        var listService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => dicSereServData[serviceReq.ID].Select(s => s.SERVICE_ID).Contains(o.ID)).ToList();
                        if (listService.Exists(o => o.DIIM_TYPE_ID.HasValue))
                        {
                            long diimType = listService.FirstOrDefault(o => o.DIIM_TYPE_ID.HasValue).DIIM_TYPE_ID ?? 0;
                            switch (diimType)
                            {
                                case IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__XQ:
                                    this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000363__CDHA__XQ, DelegateRunPrinter);
                                    break;
                                case IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__CT:
                                    this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000364__CDHA__CT, DelegateRunPrinter);
                                    break;
                                case IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__MRI:
                                    this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000365__CDHA__MRI, DelegateRunPrinter);
                                    break;
                                default:
                                    Mps000028Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printCDHA, TransReq, Configs);
                                    break;
                            }
                        }
                        else
                        {
                            Mps000028Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printCDHA, TransReq, Configs);
                        }
                    }

                    result = true;
                }
                else
                {
                    Mps000028Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqData, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printCDHA, TransReq, Configs);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void XnPrint(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printXN)
        {
            try
            {
                this.printXN = printXN;
                var lstServieReq_26 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();

                if (lstServieReq_26 != null && lstServieReq_26.Count > 0)
                {
                    ProcessGetLisSample(lstServieReq_26);
                }

                Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqNew = new Dictionary<long, List<V_HIS_SERVICE_REQ>>();
                dicServiceReqNew[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN] = new List<V_HIS_SERVICE_REQ>();
                if (lstServieReq_26 != null && lstServieReq_26.Count > 0 && Config.departmentCodeTestGroup != null && Config.departmentCodeTestGroup.Count > 0 &&
                    lstServieReq_26.Exists(o => Config.departmentCodeTestGroup.Contains(o.EXECUTE_DEPARTMENT_CODE)))
                {
                    var groupDepartment = lstServieReq_26.GroupBy(o => o.EXECUTE_DEPARTMENT_CODE).ToList();
                    foreach (var gr in groupDepartment)
                    {
                        if (Config.departmentCodeTestGroup.Contains(gr.Key))
                        {
                            Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReq432 = new Dictionary<long, List<V_HIS_SERVICE_REQ>>();
                            dicServiceReq432[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN] = gr.ToList();

                            dicReq = new Dictionary<long, List<V_HIS_SERVICE_REQ>>();
                            dicReq.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN, gr.ToList());
                            dicSs = new Dictionary<long, List<V_HIS_SERE_SERV>>();
                            foreach (var ss in gr)
                            {
                                dicSs.Add(ss.ID, dicSereServData[ss.ID]);
                            }

                            this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000432__XET_NGHIEM_GOP_KHOA_XU_LY, DelegateRunPrinter);
                        }
                        else
                        {
                            dicServiceReqNew[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN] = gr.ToList();

                            if (HisConfigs.Get<string>(Config.OptionPrintXetNghiemDiffMps) == "1")
                            {
                                var lstServieReq = dicServiceReqNew[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                                foreach (var serviceReq in lstServieReq)
                                {
                                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                                    {
                                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                                        continue;
                                    }

                                    var listService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => dicSereServData[serviceReq.ID].Select(s => s.SERVICE_ID).Contains(o.ID)).ToList();
                                    var groupType = listService.GroupBy(o => o.TEST_TYPE_ID).ToList();
                                    foreach (var item in groupType)
                                    {
                                        dicReq = new Dictionary<long, List<V_HIS_SERVICE_REQ>>();
                                        dicReq.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN, new List<V_HIS_SERVICE_REQ>() { serviceReq });
                                        dicSs = new Dictionary<long, List<V_HIS_SERE_SERV>>();
                                        var sereServ = dicSereServData[serviceReq.ID].Where(o => item.Select(s => s.ID).Contains(o.SERVICE_ID)).ToList();
                                        if (sereServ == null || sereServ.Count <= 0)
                                        {
                                            continue;
                                        }

                                        dicSs.Add(serviceReq.ID, sereServ);
                                        switch (item.Key)
                                        {
                                            case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__HH:
                                                this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000423__XN__HH, DelegateRunPrinter);
                                                break;
                                            case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__VS:
                                                this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000424__XN__VS, DelegateRunPrinter);
                                                break;
                                            case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__MD:
                                                this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000425__XN__MD, DelegateRunPrinter);
                                                break;
                                            case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__SH:
                                                this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000426__XN__SH, DelegateRunPrinter);
                                                break;
                                            case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__TEST:
                                                this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000465__XN__TEST, DelegateRunPrinter);
                                                break;
                                            case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__GPB:
                                                this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000466__XN__GPB, DelegateRunPrinter);
                                                break;
                                            case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__NT:
                                                this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000467__XN__NT, DelegateRunPrinter);
                                                break;
                                            default:
                                                Mps000026Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printXN, TransReq, Configs);
                                                break;
                                        }
                                    }
                                }

                                result = true;
                            }
                            else
                            {
                                Mps000026Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqNew, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printXN, TransReq, Configs);
                            }
                        }
                    }
                }
                else if (lstServieReq_26 != null && lstServieReq_26.Count > 0)
                {
                    dicServiceReqNew[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN] = lstServieReq_26;

                    if (HisConfigs.Get<string>(Config.OptionPrintXetNghiemDiffMps) == "1")
                    {
                        var lstServieReq = dicServiceReqNew[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                        foreach (var serviceReq in lstServieReq)
                        {
                            if (!dicSereServData.ContainsKey(serviceReq.ID))
                            {
                                Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                                continue;
                            }

                            var listService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => dicSereServData[serviceReq.ID].Select(s => s.SERVICE_ID).Contains(o.ID)).ToList();
                            var groupType = listService.GroupBy(o => o.TEST_TYPE_ID).ToList();
                            foreach (var item in groupType)
                            {
                                dicReq = new Dictionary<long, List<V_HIS_SERVICE_REQ>>();
                                dicReq.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN, new List<V_HIS_SERVICE_REQ>() { serviceReq });
                                dicSs = new Dictionary<long, List<V_HIS_SERE_SERV>>();
                                var sereServ = dicSereServData[serviceReq.ID].Where(o => item.Select(s => s.ID).Contains(o.SERVICE_ID)).ToList();
                                if (sereServ == null || sereServ.Count <= 0)
                                {
                                    continue;
                                }

                                dicSs.Add(serviceReq.ID, sereServ);
                                switch (item.Key)
                                {
                                    case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__HH:
                                        this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000423__XN__HH, DelegateRunPrinter);
                                        break;
                                    case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__VS:
                                        this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000424__XN__VS, DelegateRunPrinter);
                                        break;
                                    case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__MD:
                                        this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000425__XN__MD, DelegateRunPrinter);
                                        break;
                                    case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__SH:
                                        this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000426__XN__SH, DelegateRunPrinter);
                                        break;
                                    case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__TEST:
                                        this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000465__XN__TEST, DelegateRunPrinter);
                                        break;
                                    case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__GPB:
                                        this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000466__XN__GPB, DelegateRunPrinter);
                                        break;
                                    case IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__NT:
                                        this.richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__MPS000467__XN__NT, DelegateRunPrinter);
                                        break;
                                    default:
                                        Mps000026Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printXN, TransReq, Configs);
                                        break;
                                }
                            }
                        }

                        result = true;
                    }
                    else
                    {
                        Mps000026Print(printTypeCode, fileName, chiDinhDichVuADO, dicServiceReqNew, dicSereServData, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printXN, TransReq, Configs);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetLisSample(List<V_HIS_SERVICE_REQ> lstServieReq_26)
        {
            try
            {
                if (lstServieReq_26 != null && lstServieReq_26.Count > 0)
                {
                    List<V_HIS_SERVICE_REQ> serviceReqHasExecute = lstServieReq_26.Where(o => o.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL).ToList();
                    bool integrationTypeInventec = HisConfigs.Get<string>(Config.LisIntegrationTypeCFG) == "1";
                    if (integrationTypeInventec && serviceReqHasExecute != null && serviceReqHasExecute.Count > 0)
                    {
                        CommonParam paramCommon = new CommonParam();
                        LIS.Filter.LisSampleViewFilter filter = new LIS.Filter.LisSampleViewFilter();
                        filter.SERVICE_REQ_CODEs = serviceReqHasExecute.Select(s => s.SERVICE_REQ_CODE).Distinct().ToList();
                        this.LisSamples = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<LIS.EFMODEL.DataModels.V_LIS_SAMPLE>>("api/LisSample/GetView",
                            ApiConsumer.ApiConsumers.LisConsumer, filter, paramCommon);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.IN__MPS000363__CDHA__XQ:
                        Mps000363Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printCDHA);
                        break;
                    case PrintTypeCodeStore.IN__MPS000364__CDHA__CT:
                        Mps000364Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printCDHA);
                        break;
                    case PrintTypeCodeStore.IN__MPS000365__CDHA__MRI:
                        Mps000365Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printCDHA);
                        break;
                    case PrintTypeCodeStore.IN__MPS000366__TDCN__DT:
                        Mps000366Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printTDCN);
                        break;
                    case PrintTypeCodeStore.IN__MPS000367__TDCN__DN:
                        Mps000367Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printTDCN);
                        break;
                    case PrintTypeCodeStore.IN__MPS000368__TDCN__DC:
                        Mps000368Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, printTDCN);
                        break;
                    case PrintTypeCodeStore.IN__MPS000423__XN__HH:
                        Mps000423Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType);
                        break;
                    case PrintTypeCodeStore.IN__MPS000424__XN__VS:
                        Mps000424Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType);
                        break;
                    case PrintTypeCodeStore.IN__MPS000425__XN__MD:
                        Mps000425Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType);
                        break;
                    case PrintTypeCodeStore.IN__MPS000426__XN__SH:
                        Mps000426Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType);
                        break;
                    case PrintTypeCodeStore.IN__MPS000432__XET_NGHIEM_GOP_KHOA_XU_LY:
                        new InGopXetNghiem(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, this.PreviewType, ReqMaxNumOrderSDO, IsMethodSaveNPrint, this.SavedData, this.CancelPrint, this.DlgSendResultSigned);
                        break;
                    case PrintTypeCodeStore.IN__MPS000465__XN__TEST:
                        Mps000465Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, this.printXN);
                        break;
                    case PrintTypeCodeStore.IN__MPS000466__XN__GPB:
                        Mps000466Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, this.printXN);
                        break;
                    case PrintTypeCodeStore.IN__MPS000467__XN__NT:
                        Mps000467Print(printTypeCode, fileName, chiDinhDichVuADO, dicReq, dicSs, dicSereServExtData, bedLogs, printNow, ref result, roomId, isView, PreviewType, this.printXN);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = false;
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000053Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printPHCN)
        {
            try
            {
                var lstServieReq_53 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN].ToList();
                foreach (var serviceReq in lstServieReq_53)
                {
                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printPHCN)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000053.PDO.Mps000053ADO mps000053ADO = new MPS.Processor.Mps000053.PDO.Mps000053ADO();
                            mps000053ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000053ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000053ADO.ratio = chiDinhDichVuADO.Ratio;

                            mps000053ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000053ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            if (serviceParent != null)
                            {
                                mps000053ADO.PARENT_NAME = serviceParent.SERVICE_NAME;
                            }

                            var lstSereServ_53 = new List<MPS.Processor.Mps000053.PDO.Mps000053_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se53 = new MPS.Processor.Mps000053.PDO.Mps000053_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se53.SERVICE_ID);
                                se53.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se53.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se53.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se53.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se53.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se53.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se53.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                lstSereServ_53.Add(se53);
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_53.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_53.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            MPS.Processor.Mps000053.PDO.Mps000053PDO mps000053RDO = new MPS.Processor.Mps000053.PDO.Mps000053PDO(
                                serviceReq,
                                lstSereServ_53,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                mps000053ADO,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction
                                );

                            Print.PrintData(printTypeCode, fileName, mps000053RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_53.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000053.PDO.Mps000053ADO mps000053ADO = new MPS.Processor.Mps000053.PDO.Mps000053ADO();
                        mps000053ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000053ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000053ADO.ratio = chiDinhDichVuADO.Ratio;

                        mps000053ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000053ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_53 = new List<MPS.Processor.Mps000053.PDO.Mps000053_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se53 = new MPS.Processor.Mps000053.PDO.Mps000053_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se53.SERVICE_ID);
                            se53.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se53.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se53.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se53.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se53.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se53.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se53.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }

                            lstSereServ_53.Add(se53);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000053ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_53.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_53.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        MPS.Processor.Mps000053.PDO.Mps000053PDO mps000053RDO = new MPS.Processor.Mps000053.PDO.Mps000053PDO(
                            serviceReq,
                            lstSereServ_53,
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            mps000053ADO,
                            _HIS_DHST,
                            _WORK_PLACE,
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction);
                        Print.PrintData(printTypeCode, fileName, mps000053RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_53.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000042Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printG)
        {
            try
            {
                var lstServieReq_42 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G].ToList();
                foreach (var serviceReq in lstServieReq_42)
                {
                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printG)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000042.PDO.Mps000042ADO mps000042ADO = new MPS.Processor.Mps000042.PDO.Mps000042ADO();
                            mps000042ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000042ADO.ratio = chiDinhDichVuADO.Ratio;

                            mps000042ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000042ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            mps000042ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_42 = new List<MPS.Processor.Mps000042.PDO.Mps000042_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se42 = new MPS.Processor.Mps000042.PDO.Mps000042_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se42.SERVICE_ID);
                                se42.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se42.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se42.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se42.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se42.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se42.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se42.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                lstSereServ_42.Add(se42);
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_42.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_42.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            MPS.Processor.Mps000042.PDO.Mps000042PDO mps000042RDO = new MPS.Processor.Mps000042.PDO.Mps000042PDO(
                                serviceReq,
                                lstSereServ_42,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                mps000042ADO,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction
                                );

                            Print.PrintData(printTypeCode, fileName, mps000042RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_42.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000042.PDO.Mps000042ADO mps000042ADO = new MPS.Processor.Mps000042.PDO.Mps000042ADO();
                        mps000042ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000042ADO.ratio = chiDinhDichVuADO.Ratio;

                        mps000042ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000042ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_42 = new List<MPS.Processor.Mps000042.PDO.Mps000042_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se42 = new MPS.Processor.Mps000042.PDO.Mps000042_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se42.SERVICE_ID);
                            se42.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se42.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se42.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se42.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se42.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se42.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se42.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }

                            lstSereServ_42.Add(se42);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000042ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_42.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_42.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        MPS.Processor.Mps000042.PDO.Mps000042PDO mps000042RDO = new MPS.Processor.Mps000042.PDO.Mps000042PDO(
                            serviceReq,
                            lstSereServ_42,
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            mps000042ADO,
                            _HIS_DHST,
                            _WORK_PLACE,
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction);
                        Print.PrintData(printTypeCode, fileName, mps000042RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_42.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000040Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printKHAC)
        {
            try
            {
                var lstServieReq_40 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC].ToList();
                foreach (var serviceReq in lstServieReq_40)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printKHAC)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000040.PDO.Mps000040ADO mps000040ADO = new MPS.Processor.Mps000040.PDO.Mps000040ADO();
                            mps000040ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000040ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000040ADO.ratio = chiDinhDichVuADO.Ratio;

                            mps000040ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000040ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            mps000040ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_40 = new List<MPS.Processor.Mps000040.PDO.Mps000040_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se40 = new MPS.Processor.Mps000040.PDO.Mps000040_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se40.SERVICE_ID);
                                se40.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se40.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se40.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se40.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se40.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se40.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se40.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                lstSereServ_40.Add(se40);
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_40.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_40.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            MPS.Processor.Mps000040.PDO.Mps000040PDO mps000040RDO = new MPS.Processor.Mps000040.PDO.Mps000040PDO(
                                serviceReq,
                                lstSereServ_40,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                mps000040ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                Configs
                                );

                            Print.PrintData(printTypeCode, fileName, mps000040RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_40.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000040.PDO.Mps000040ADO mps000040ADO = new MPS.Processor.Mps000040.PDO.Mps000040ADO();
                        mps000040ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000040ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000040ADO.ratio = chiDinhDichVuADO.Ratio;

                        mps000040ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000040ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_40 = new List<MPS.Processor.Mps000040.PDO.Mps000040_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se40 = new MPS.Processor.Mps000040.PDO.Mps000040_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se40.SERVICE_ID);
                            se40.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se40.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se40.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se40.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se40.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se40.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se40.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }

                            lstSereServ_40.Add(se40);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000040ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_40.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_40.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        MPS.Processor.Mps000040.PDO.Mps000040PDO mps000040RDO = new MPS.Processor.Mps000040.PDO.Mps000040PDO(
                            serviceReq,
                            lstSereServ_40,
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            mps000040ADO,
                            bedLog,
                            _HIS_DHST,
                            _WORK_PLACE,
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                Configs);

                        Print.PrintData(printTypeCode, fileName, mps000040RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_40.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000038Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printTDCN)
        {
            try
            {
                var lstServieReq_38 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN].ToList();
                foreach (var serviceReq in lstServieReq_38)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printTDCN)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000038.PDO.Mps000038ADO mps000038ADO = new MPS.Processor.Mps000038.PDO.Mps000038ADO();
                            mps000038ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000038ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000038ADO.ratio = chiDinhDichVuADO.Ratio;

                            mps000038ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000038ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            mps000038ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_38 = new List<MPS.Processor.Mps000038.PDO.Mps000038_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se38 = new MPS.Processor.Mps000038.PDO.Mps000038_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se38.SERVICE_ID);
                                se38.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se38.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se38.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se38.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se38.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se38.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se38.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                lstSereServ_38.Add(se38);
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_38.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_38.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            MPS.Processor.Mps000038.PDO.Mps000038PDO mps000038RDO = new MPS.Processor.Mps000038.PDO.Mps000038PDO(
                                serviceReq,
                                lstSereServ_38,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                mps000038ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                Configs
                                );

                            Print.PrintData(printTypeCode, fileName, mps000038RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_38.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000038.PDO.Mps000038ADO mps000038ADO = new MPS.Processor.Mps000038.PDO.Mps000038ADO();
                        mps000038ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000038ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000038ADO.ratio = chiDinhDichVuADO.Ratio;

                        mps000038ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000038ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_38 = new List<MPS.Processor.Mps000038.PDO.Mps000038_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se38 = new MPS.Processor.Mps000038.PDO.Mps000038_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se38.SERVICE_ID);
                            se38.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se38.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se38.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se38.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se38.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se38.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se38.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }
                            lstSereServ_38.Add(se38);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000038ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_38.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_38.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        MPS.Processor.Mps000038.PDO.Mps000038PDO mps000038RDO = new MPS.Processor.Mps000038.PDO.Mps000038PDO(
                            serviceReq,
                            lstSereServ_38,
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            mps000038ADO,
                            bedLog,
                            _HIS_DHST,
                            _WORK_PLACE,
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                Configs);
                        Print.PrintData(printTypeCode, fileName, mps000038RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_38.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000036Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printPT)
        {
            try
            {
                var lstServieReq_36 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT].ToList();
                foreach (var serviceReq in lstServieReq_36)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printPT)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000036.PDO.SingleKeyValue mps000036ADO = new MPS.Processor.Mps000036.PDO.SingleKeyValue();
                            mps000036ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000036ADO.RatioText = (chiDinhDichVuADO.Ratio * 100).ToString() + "%";

                            mps000036ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000036ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            mps000036ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_36 = new List<MPS.Processor.Mps000036.PDO.Mps000036_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se36 = new MPS.Processor.Mps000036.PDO.Mps000036_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se36.SERVICE_ID);
                                se36.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se36.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                se36.PTTT_GROUP_ID = service != null ? service.PTTT_GROUP_ID : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se36.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se36.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se36.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se36.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se36.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                lstSereServ_36.Add(se36);
                            }

                            if (Config.SugrPrintSplitByType == "1")
                            {
                                var lstSereServ_36Group = lstSereServ_36.GroupBy(o => o.PTTT_GROUP_ID);
                                foreach (var Pttt36 in lstSereServ_36Group)
                                {
                                    List<V_HIS_TRANSACTION> transaction = null;
                                    if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                                    {
                                        List<long> transactionIds = new List<long>();
                                        if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                        {
                                            transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => Pttt36.ToList().Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                        }

                                        if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                        {
                                            transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => Pttt36.ToList().Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                        }

                                        transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                                    }

                                    MPS.Processor.Mps000036.PDO.Mps000036PDO mps000036RDO = new MPS.Processor.Mps000036.PDO.Mps000036PDO(
                                        serviceReq,
                                        Pttt36.ToList(),
                                        chiDinhDichVuADO.patientTypeAlter,
                                        chiDinhDichVuADO.treament,
                                        mps000036ADO,
                                        bedLog,
                                        _HIS_DHST,
                                        _WORK_PLACE,
                                        chiDinhDichVuADO.ListSereServDeposit,
                                        chiDinhDichVuADO.ListSereServBill,
                                        transaction,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                Configs
                                        );

                                    Print.PrintData(printTypeCode, fileName, mps000036RDO, printNow, ref result, roomId, isView, PreviewType, Pttt36.ToList().Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                                }
                            }
                            else
                            {
                                List<V_HIS_TRANSACTION> transaction = null;
                                if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                                {
                                    List<long> transactionIds = new List<long>();
                                    if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                    {
                                        transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_36.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                    }

                                    if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                    {
                                        transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_36.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                    }

                                    transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                                }

                                MPS.Processor.Mps000036.PDO.Mps000036PDO mps000036RDO = new MPS.Processor.Mps000036.PDO.Mps000036PDO(
                                    serviceReq,
                                    lstSereServ_36,
                                    chiDinhDichVuADO.patientTypeAlter,
                                    chiDinhDichVuADO.treament,
                                    mps000036ADO,
                                    bedLog,
                                    _HIS_DHST,
                                    _WORK_PLACE,
                                    chiDinhDichVuADO.ListSereServDeposit,
                                    chiDinhDichVuADO.ListSereServBill,
                                    transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs
                                    );

                                Print.PrintData(printTypeCode, fileName, mps000036RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_36.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                            }
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000036.PDO.SingleKeyValue mps000036ADO = new MPS.Processor.Mps000036.PDO.SingleKeyValue();
                        mps000036ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000036ADO.RatioText = (chiDinhDichVuADO.Ratio * 100).ToString() + "%";

                        mps000036ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000036ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_36 = new List<MPS.Processor.Mps000036.PDO.Mps000036_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se36 = new MPS.Processor.Mps000036.PDO.Mps000036_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se36.SERVICE_ID);
                            se36.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se36.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            se36.PTTT_GROUP_ID = service != null ? service.PTTT_GROUP_ID : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se36.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se36.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se36.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se36.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se36.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }

                            lstSereServ_36.Add(se36);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000036ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        if (Config.SugrPrintSplitByType == "1")
                        {
                            var lstSereServ_36Group = lstSereServ_36.GroupBy(o => o.PTTT_GROUP_ID);
                            foreach (var Pttt36 in lstSereServ_36Group)
                            {
                                List<V_HIS_TRANSACTION> transaction = null;
                                if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                                {
                                    List<long> transactionIds = new List<long>();
                                    if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                    {
                                        transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => Pttt36.ToList().Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                    }

                                    if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                    {
                                        transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => Pttt36.ToList().Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                    }

                                    transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                                }

                                MPS.Processor.Mps000036.PDO.Mps000036PDO mps000036RDO = new MPS.Processor.Mps000036.PDO.Mps000036PDO(
                                    serviceReq,
                                    Pttt36.ToList(),
                                    chiDinhDichVuADO.patientTypeAlter,
                                    chiDinhDichVuADO.treament,
                                    mps000036ADO,
                                    bedLog,
                                    _HIS_DHST,
                                    _WORK_PLACE,
                                    chiDinhDichVuADO.ListSereServDeposit,
                                    chiDinhDichVuADO.ListSereServBill,
                                    transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs);

                                Print.PrintData(printTypeCode, fileName, mps000036RDO, printNow, ref result, roomId, isView, PreviewType, Pttt36.ToList().Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                            }
                        }
                        else
                        {
                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_36.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_36.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            MPS.Processor.Mps000036.PDO.Mps000036PDO mps000036RDO = new MPS.Processor.Mps000036.PDO.Mps000036PDO(
                                serviceReq,
                                lstSereServ_36,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                mps000036ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs);

                            Print.PrintData(printTypeCode, fileName, mps000036RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_36.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000031Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printTT)
        {
            try
            {
                var lstServieReq_31 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT].ToList();
                foreach (var serviceReq in lstServieReq_31)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printTT)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000031.PDO.Mps000031ADO mps000031ADO = new MPS.Processor.Mps000031.PDO.Mps000031ADO();
                            mps000031ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000031ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000031ADO.ratio = chiDinhDichVuADO.Ratio;

                            mps000031ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000031ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            mps000031ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_31 = new List<MPS.Processor.Mps000031.PDO.Mps000031_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se31 = new MPS.Processor.Mps000031.PDO.Mps000031_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se31.SERVICE_ID);
                                se31.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se31.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                se31.PTTT_GROUP_ID = service != null ? service.PTTT_GROUP_ID : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se31.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se31.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se31.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se31.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se31.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                lstSereServ_31.Add(se31);
                            }

                            if (Config.SugrPrintSplitByType == "1")
                            {
                                var lstSereServ_31Group = lstSereServ_31.GroupBy(o => o.PTTT_GROUP_ID);
                                foreach (var Pttt31 in lstSereServ_31Group)
                                {
                                    List<V_HIS_TRANSACTION> transaction = null;
                                    if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                                    {
                                        List<long> transactionIds = new List<long>();
                                        if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                        {
                                            transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => Pttt31.ToList().Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                        }

                                        if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                        {
                                            transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => Pttt31.ToList().Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                        }

                                        transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                                    }

                                    MPS.Processor.Mps000031.PDO.Mps000031PDO mps000031RDO = new MPS.Processor.Mps000031.PDO.Mps000031PDO(
                                        serviceReq,
                                        Pttt31.ToList(),
                                        chiDinhDichVuADO.patientTypeAlter,
                                        chiDinhDichVuADO.treament,
                                        mps000031ADO,
                                        bedLog,
                                        _HIS_DHST,
                                        _WORK_PLACE,
                                        chiDinhDichVuADO.ListSereServDeposit,
                                        chiDinhDichVuADO.ListSereServBill,
                                        transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs
                                        );

                                    Print.PrintData(printTypeCode, fileName, mps000031RDO, printNow, ref result, roomId, isView, PreviewType, Pttt31.ToList().Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                                }
                            }
                            else
                            {
                                List<V_HIS_TRANSACTION> transaction = null;
                                if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                                {
                                    List<long> transactionIds = new List<long>();
                                    if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                    {
                                        transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_31.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                    }

                                    if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                    {
                                        transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_31.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                    }

                                    transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                                }

                                MPS.Processor.Mps000031.PDO.Mps000031PDO mps000031RDO = new MPS.Processor.Mps000031.PDO.Mps000031PDO(
                                    serviceReq,
                                    lstSereServ_31,
                                    chiDinhDichVuADO.patientTypeAlter,
                                    chiDinhDichVuADO.treament,
                                    mps000031ADO,
                                    bedLog,
                                    _HIS_DHST,
                                    _WORK_PLACE,
                                    chiDinhDichVuADO.ListSereServDeposit,
                                    chiDinhDichVuADO.ListSereServBill,
                                    transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs
                                    );

                                Print.PrintData(printTypeCode, fileName, mps000031RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_31.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                            }
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000031.PDO.Mps000031ADO mps000031ADO = new MPS.Processor.Mps000031.PDO.Mps000031ADO();
                        mps000031ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000031ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000031ADO.ratio = chiDinhDichVuADO.Ratio;

                        mps000031ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000031ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_31 = new List<MPS.Processor.Mps000031.PDO.Mps000031_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se31 = new MPS.Processor.Mps000031.PDO.Mps000031_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se31.SERVICE_ID);
                            se31.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se31.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            se31.PTTT_GROUP_ID = service != null ? service.PTTT_GROUP_ID : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se31.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se31.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se31.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se31.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se31.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }
                            lstSereServ_31.Add(se31);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000031ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        if (Config.SugrPrintSplitByType == "1")
                        {
                            var lstSereServ_31Group = lstSereServ_31.GroupBy(o => o.PTTT_GROUP_ID);
                            foreach (var pttt31 in lstSereServ_31Group)
                            {
                                List<V_HIS_TRANSACTION> transaction = null;
                                if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                                {
                                    List<long> transactionIds = new List<long>();
                                    if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                    {
                                        transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => pttt31.ToList().Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                    }

                                    if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                    {
                                        transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => pttt31.ToList().Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                    }

                                    transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                                }

                                MPS.Processor.Mps000031.PDO.Mps000031PDO mps000031RDO = new MPS.Processor.Mps000031.PDO.Mps000031PDO(
                                    serviceReq,
                                    pttt31.ToList(),
                                    chiDinhDichVuADO.patientTypeAlter,
                                    chiDinhDichVuADO.treament,
                                    mps000031ADO,
                                    bedLog,
                                    _HIS_DHST,
                                    _WORK_PLACE,
                                    chiDinhDichVuADO.ListSereServDeposit,
                                    chiDinhDichVuADO.ListSereServBill,
                                    transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs);
                                Print.PrintData(printTypeCode, fileName, mps000031RDO, printNow, ref result, roomId, isView, PreviewType, pttt31.ToList().Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                            }
                        }
                        else
                        {
                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_31.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_31.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            MPS.Processor.Mps000031.PDO.Mps000031PDO mps000031RDO = new MPS.Processor.Mps000031.PDO.Mps000031PDO(
                                serviceReq,
                                lstSereServ_31,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                mps000031ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs);
                            Print.PrintData(printTypeCode, fileName, mps000031RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_31.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000030Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printSA)
        {
            try
            {
                var lstServieReq_30 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA].ToList();
                foreach (var serviceReq in lstServieReq_30)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printSA)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000030.PDO.Mps000030ADO mps000030ADO = new MPS.Processor.Mps000030.PDO.Mps000030ADO();
                            mps000030ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000030ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000030ADO.ratio = chiDinhDichVuADO.Ratio;

                            mps000030ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000030ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            mps000030ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_30 = new List<MPS.Processor.Mps000030.PDO.Mps000030_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se30 = new MPS.Processor.Mps000030.PDO.Mps000030_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se30.SERVICE_ID);
                                se30.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se30.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se30.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se30.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se30.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se30.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se30.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                if (sere.SERVICE_CONDITION_ID != null)
                                {
                                    var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);
                                    if (Condition != null)
                                    {
                                        se30.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                        se30.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                    }
                                }
                                lstSereServ_30.Add(se30);
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_30.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_30.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            MPS.Processor.Mps000030.PDO.Mps000030PDO mps000030RDO = new MPS.Processor.Mps000030.PDO.Mps000030PDO(
                                serviceReq,
                                lstSereServ_30,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                mps000030ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs
                                );

                            Print.PrintData(printTypeCode, fileName, mps000030RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_30.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000030.PDO.Mps000030ADO mps000030ADO = new MPS.Processor.Mps000030.PDO.Mps000030ADO();
                        mps000030ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000030ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000030ADO.ratio = chiDinhDichVuADO.Ratio;

                        mps000030ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000030ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_30 = new List<MPS.Processor.Mps000030.PDO.Mps000030_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se30 = new MPS.Processor.Mps000030.PDO.Mps000030_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se30.SERVICE_ID);
                            se30.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se30.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se30.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se30.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se30.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se30.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se30.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }

                            if (se30.SERVICE_CONDITION_ID != null)
                            {
                                var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == se30.SERVICE_CONDITION_ID);
                                if (Condition != null)
                                {
                                    se30.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                    se30.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                }
                            }
                            lstSereServ_30.Add(se30);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000030ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_30.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_30.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        MPS.Processor.Mps000030.PDO.Mps000030PDO mps000030RDO = new MPS.Processor.Mps000030.PDO.Mps000030PDO(
                            serviceReq,
                            lstSereServ_30,
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            mps000030ADO,
                            bedLog,
                            _HIS_DHST,
                            _WORK_PLACE,
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs);

                        Print.PrintData(printTypeCode, fileName, mps000030RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_30.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000029Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printNS)
        {
            try
            {
                var lstServieReq_29 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS].ToList();
                foreach (var serviceReq in lstServieReq_29)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printNS)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000029.PDO.Mps000029ADO Mps000029ADO = new MPS.Processor.Mps000029.PDO.Mps000029ADO();
                            Mps000029ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            Mps000029ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            Mps000029ADO.ratio = chiDinhDichVuADO.Ratio;

                            Mps000029ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            Mps000029ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            Mps000029ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_29 = new List<MPS.Processor.Mps000029.PDO.Mps000029_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se29 = new MPS.Processor.Mps000029.PDO.Mps000029_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se29.SERVICE_ID);
                                se29.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se29.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se29.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se29.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se29.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se29.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se29.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                if (sere.SERVICE_CONDITION_ID != null)
                                {
                                    var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);
                                    if (Condition != null)
                                    {
                                        se29.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                        se29.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                    }
                                }
                                lstSereServ_29.Add(se29);
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_29.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_29.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            MPS.Processor.Mps000029.PDO.Mps000029PDO mps000029RDO = new MPS.Processor.Mps000029.PDO.Mps000029PDO(
                                serviceReq,
                                lstSereServ_29,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                Mps000029ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs);

                            Print.PrintData(printTypeCode, fileName, mps000029RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_29.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000029.PDO.Mps000029ADO Mps000029ADO = new MPS.Processor.Mps000029.PDO.Mps000029ADO();
                        Mps000029ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        Mps000029ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        Mps000029ADO.ratio = chiDinhDichVuADO.Ratio;

                        Mps000029ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        Mps000029ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_29 = new List<MPS.Processor.Mps000029.PDO.Mps000029_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se29 = new MPS.Processor.Mps000029.PDO.Mps000029_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se29.SERVICE_ID);
                            se29.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se29.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se29.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se29.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se29.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se29.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se29.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }

                            if (sere.SERVICE_CONDITION_ID != null)
                            {
                                var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);
                                if (Condition != null)
                                {
                                    se29.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                    se29.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                }
                            }
                            lstSereServ_29.Add(se29);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                Mps000029ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_29.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_29.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        MPS.Processor.Mps000029.PDO.Mps000029PDO mps000029RDO = new MPS.Processor.Mps000029.PDO.Mps000029PDO(
                            serviceReq,
                            lstSereServ_29,
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            Mps000029ADO,
                            bedLog,
                            _HIS_DHST,
                            _WORK_PLACE,
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                                 Configs);

                        Print.PrintData(printTypeCode, fileName, mps000029RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_29.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000028Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printCDHA, List<HIS_TRANS_REQ> TransReq, List<HIS_CONFIG> Configs)
        {
            try
            {
                var genderCode = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == chiDinhDichVuADO.treament.TDL_PATIENT_GENDER_ID);

                var lstServieReq_28 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA].ToList();
                foreach (var serviceReq in lstServieReq_28)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    MPS.Processor.Mps000028.PDO.Mps000028PDO mps000028RDO = null;

                    if (printCDHA)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServCDHA = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
                        {
                            var service = ProcessDictionaryData.GetService(item.SERVICE_ID);
                            if (service != null)
                            {
                                if (!dicSereServCDHA.ContainsKey(service.PARENT_ID ?? 0))
                                    dicSereServCDHA[service.PARENT_ID ?? 0] = new List<V_HIS_SERE_SERV>();
                                dicSereServCDHA[service.PARENT_ID ?? 0].Add(item);
                            }
                            else
                            {
                                if (dicSereServCDHA[0] == null)
                                    dicSereServCDHA[0] = new List<V_HIS_SERE_SERV>();
                                dicSereServCDHA[0].Add(item);
                            }
                        }

                        foreach (var item in dicSereServCDHA)
                        {
                            MPS.Processor.Mps000028.PDO.Mps000028ADO Mps000028ADO = new MPS.Processor.Mps000028.PDO.Mps000028ADO();
                            Mps000028ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            Mps000028ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            Mps000028ADO.ratio = chiDinhDichVuADO.Ratio;
                            Mps000028ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                            Mps000028ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            Mps000028ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            var lstSereServ_28 = new List<MPS.Processor.Mps000028.PDO.SereServADO>();
                            foreach (var sere in item.Value)
                            {
                                MPS.Processor.Mps000028.PDO.SereServADO sereServADO = new MPS.Processor.Mps000028.PDO.SereServADO(sere);
                                var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == sere.TDL_SERVICE_TYPE_ID);
                                sereServADO.patientIdQr = chiDinhDichVuADO.treament.TREATMENT_CODE + "_"
                                    + sere.TDL_SERVICE_REQ_CODE + "_"
                                    + (serviceType != null ? serviceType.SERVICE_TYPE_CODE : "") + "_"
                                    + sere.TDL_SERVICE_CODE + "_"
                                    + chiDinhDichVuADO.treament.TDL_PATIENT_CODE;

                                sereServADO.patientNameQr = chiDinhDichVuADO.treament.TDL_PATIENT_NAME;
                                sereServADO.studyDescriptionQr = chiDinhDichVuADO.treament.TDL_PATIENT_DOB + "_"
                                    + (genderCode != null ? genderCode.GENDER_CODE : "03");

                                var service = ProcessDictionaryData.GetService(sere.SERVICE_ID);
                                sereServADO.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                sereServADO.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    sereServADO.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    sereServADO.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    sereServADO.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    sereServADO.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    sereServADO.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                if (sere.SERVICE_CONDITION_ID != null)
                                {
                                    var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);
                                    if (Condition != null)
                                    {
                                        sereServADO.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                        sereServADO.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                    }
                                }
                                lstSereServ_28.Add(sereServADO);
                            }

                            var parent = ProcessDictionaryData.GetService(item.Key);
                            Mps000028ADO.TITLE = parent != null ? parent.SERVICE_NAME : "";
                            Mps000028ADO.PARENT_NAME = parent != null ? parent.SERVICE_NAME : "";

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_28.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_28.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            mps000028RDO = new MPS.Processor.Mps000028.PDO.Mps000028PDO(
                                serviceReq,
                                lstSereServ_28,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                Mps000028ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                        Configs
                                );

                            Print.PrintData(printTypeCode, fileName, mps000028RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_28.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000028.PDO.Mps000028ADO Mps000028ADO = new MPS.Processor.Mps000028.PDO.Mps000028ADO();
                        Mps000028ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        Mps000028ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        Mps000028ADO.ratio = chiDinhDichVuADO.Ratio;
                        Mps000028ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                        Mps000028ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        Mps000028ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_28 = new List<MPS.Processor.Mps000028.PDO.SereServADO>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            MPS.Processor.Mps000028.PDO.SereServADO sereServADO = new MPS.Processor.Mps000028.PDO.SereServADO(sere);
                            var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == sere.TDL_SERVICE_TYPE_ID);
                            sereServADO.patientIdQr = chiDinhDichVuADO.treament.TREATMENT_CODE + "_"
                                + sere.TDL_SERVICE_REQ_CODE + "_"
                                + (serviceType != null ? serviceType.SERVICE_TYPE_CODE : "") + "_"
                                + sere.TDL_SERVICE_CODE + "_"
                                + chiDinhDichVuADO.treament.TDL_PATIENT_CODE;

                            sereServADO.patientNameQr = chiDinhDichVuADO.treament.TDL_PATIENT_NAME;
                            sereServADO.studyDescriptionQr = chiDinhDichVuADO.treament.TDL_PATIENT_DOB + "_"
                                + (genderCode != null ? genderCode.GENDER_CODE : "03");

                            var service = ProcessDictionaryData.GetService(sere.SERVICE_ID);
                            sereServADO.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            sereServADO.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                sereServADO.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                sereServADO.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                sereServADO.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                sereServADO.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                sereServADO.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }

                            if (sere.SERVICE_CONDITION_ID != null)
                            {
                                var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);
                                if (Condition != null)
                                {
                                    sereServADO.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                    sereServADO.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                }
                            }
                            lstSereServ_28.Add(sereServADO);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                Mps000028ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_28.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_28.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        mps000028RDO = new MPS.Processor.Mps000028.PDO.Mps000028PDO(
                            serviceReq,
                                lstSereServ_28,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                Mps000028ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                        TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null,
                        Configs

                            );

                        Print.PrintData(printTypeCode, fileName, mps000028RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_28.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000026Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printTEST, List<HIS_TRANS_REQ> TransReq, List<HIS_CONFIG> Configs)
        {
            try
            {
                var lstServieReq_26 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                foreach (var serviceReq in lstServieReq_26)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    Inventec.Common.Logging.LogSystem.Debug("printTEST " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printTEST), printTEST));

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    LIS.EFMODEL.DataModels.V_LIS_SAMPLE lisSample = null;
                    if (this.LisSamples != null && this.LisSamples.Count > 0)
                    {
                        var sample = this.LisSamples.Where(o => o.SERVICE_REQ_CODE == serviceReq.SERVICE_REQ_CODE).ToList();
                        if (sample != null && sample.Count > 0)
                        {
                            lisSample = sample.OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                        }
                    }

                    if (printTEST)
                    {
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in dicSereServData[serviceReq.ID])
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
                            MPS.Processor.Mps000026.PDO.Mps000026ADO mps000026ADO = new MPS.Processor.Mps000026.PDO.Mps000026ADO();
                            mps000026ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000026ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000026ADO.ratio = chiDinhDichVuADO.Ratio;
                            mps000026ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                            mps000026ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000026ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            mps000026ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_26 = item.Value;
                            var lstSereServExt_26 = new List<HIS_SERE_SERV_EXT>();
                            if (dicSereServExtData != null)
                            {
                                foreach (var sere in item.Value)
                                {
                                    if (dicSereServExtData.ContainsKey(sere.ID))
                                    {
                                        lstSereServExt_26.Add(dicSereServExtData[sere.ID]);
                                    }
                                }
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_26.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_26.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            MPS.Processor.Mps000026.PDO.Mps000026PDO mps000026RDO = new MPS.Processor.Mps000026.PDO.Mps000026PDO(
                                chiDinhDichVuADO.treament,
                                serviceReq,
                                lstSereServ_26,
                                chiDinhDichVuADO.patientTypeAlter,
                                mps000026ADO,
                                bedLog,
                                serviceParent,
                                _HIS_DHST,
                                _WORK_PLACE,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                                lisSample,
                                lstSereServExt_26,
                                BackendDataWorker.Get<HIS_SERVICE_CONDITION>(),
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                                );

                            Print.PrintData(printTypeCode, fileName, mps000026RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_26.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000026.PDO.Mps000026ADO mps000026ADO = new MPS.Processor.Mps000026.PDO.Mps000026ADO();
                        mps000026ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000026ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000026ADO.ratio = chiDinhDichVuADO.Ratio;
                        mps000026ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                        mps000026ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000026ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_26 = dicSereServData[serviceReq.ID];
                        var lstSereServExt_26 = new List<HIS_SERE_SERV_EXT>();
                        if (dicSereServExtData != null)
                        {
                            foreach (var sere in dicSereServData[serviceReq.ID])
                            {
                                if (dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    lstSereServExt_26.Add(dicSereServExtData[sere.ID]);
                                }
                            }
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000026ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_26.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_26.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        MPS.Processor.Mps000026.PDO.Mps000026PDO mps000026RDO = new MPS.Processor.Mps000026.PDO.Mps000026PDO(
                            chiDinhDichVuADO.treament,
                            serviceReq,
                            lstSereServ_26,
                            chiDinhDichVuADO.patientTypeAlter,
                            mps000026ADO,
                            bedLog,
                            null,
                            _HIS_DHST,
                            _WORK_PLACE,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction,
                            lisSample,
                            lstSereServExt_26,
                            BackendDataWorker.Get<HIS_SERVICE_CONDITION>(),
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                            );

                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReq), serviceReq));
                        Print.PrintData(printTypeCode, fileName, mps000026RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_26.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000167Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printGPBL)
        {
            try
            {
                var lstServieReq_421 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL].ToList();
                foreach (var serviceReq in lstServieReq_421)
                {
                    // new
                    var paanLiquilds = BackendDataWorker.Get<HIS_PAAN_LIQUID>();
                    HIS_PAAN_LIQUID paanLiquild = paanLiquilds.SingleOrDefault(o => o.ID == serviceReq.PAAN_LIQUID_ID);
                    var paanPositions = BackendDataWorker.Get<HIS_PAAN_POSITION>();
                    HIS_PAAN_POSITION paanPosition = paanPositions.SingleOrDefault(o => o.ID == serviceReq.PAAN_POSITION_ID);

                    var listSereServDic = dicSereServData[serviceReq.ID].ToList();

                    HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                    ssFilter.ID = listSereServDic.FirstOrDefault().ID;
                    var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                    if (listSereServ == null || listSereServ.Count != 1)
                    {
                        throw new Exception("Khong lay duoc V_HIS_SERE_SERV_5 theo Id: " + listSereServDic.FirstOrDefault().ID);
                    }

                    var sereServPrint = listSereServ.FirstOrDefault();

                    int SetDefaultDepositPrice = Inventec.Common.TypeConvert.Parse.ToInt32(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT"));
                    if (SetDefaultDepositPrice == 1)
                    {
                        if (chiDinhDichVuADO.patientTypeAlter != null && chiDinhDichVuADO.patientTypeAlter.PATIENT_TYPE_ID == Config.PatientTypeId__BHYT && chiDinhDichVuADO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            V_HIS_SERE_SERV sereServ = new V_HIS_SERE_SERV();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServ, sereServPrint);
                            sereServPrint.VIR_PRICE = MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPatientPriceForOutBhyt(sereServ);
                        }
                    }

                    MPS.Processor.Mps000167.PDO.Mps000167ADO mps167ADO = new MPS.Processor.Mps000167.PDO.Mps000167ADO();
                    if (paanLiquild != null)
                    {
                        mps167ADO.PAAN_LIQUID_CODE = paanLiquild.PAAN_LIQUID_CODE;
                        mps167ADO.PAAN_LIQUID_NAME = paanLiquild.PAAN_LIQUID_NAME;
                    }

                    if (paanPosition != null)
                    {
                        mps167ADO.PAAN_POSITION_CODE = paanPosition.PAAN_POSITION_CODE;
                        mps167ADO.PAAN_POSITION_NAME = paanPosition.PAAN_POSITION_NAME;
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);
                    mps167ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    CommonParam param = new CommonParam();

                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = serviceReq.TREATMENT_ID;
                    var treatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();

                    mps167ADO.KSK_ORDER = treatment.KSK_ORDER;

                    List<V_HIS_TRANSACTION> transaction = null;
                    if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                    {
                        List<long> transactionIds = new List<long>();
                        if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                        {
                            transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => listSereServDic.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                        }

                        if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                        {
                            transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => listSereServDic.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                        }

                        transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((serviceReq != null ? serviceReq.TREATMENT_CODE : ""), printTypeCode, this.roomId);
                    MPS.Processor.Mps000167.PDO.Mps000167PDO rdo = new MPS.Processor.Mps000167.PDO.Mps000167PDO(
                        serviceReq,
                        sereServPrint,
                        chiDinhDichVuADO.patientTypeAlter,
                        mps167ADO,
                        listSereServDic,
                        chiDinhDichVuADO.ListSereServDeposit,
                        chiDinhDichVuADO.ListSereServBill,
                        transaction,
                        treatment,
                        Configs,
                         TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null);

                    Print.PrintData(printTypeCode, fileName, rdo, printNow, ref result, roomId, isView, PreviewType, listSereServDic.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000363Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printCDHA)
        {
            try
            {
                var lstServieReq_28 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA].ToList();
                foreach (var serviceReq in lstServieReq_28)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    MPS.Processor.Mps000363.PDO.Mps000363PDO mps000363RDO = null;

                    if (printCDHA)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServCDHA = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
                        {
                            var service = ProcessDictionaryData.GetService(item.SERVICE_ID);
                            if (service != null)
                            {
                                if (!dicSereServCDHA.ContainsKey(service.PARENT_ID ?? 0))
                                    dicSereServCDHA[service.PARENT_ID ?? 0] = new List<V_HIS_SERE_SERV>();
                                dicSereServCDHA[service.PARENT_ID ?? 0].Add(item);
                            }
                            else
                            {
                                if (dicSereServCDHA[0] == null)
                                    dicSereServCDHA[0] = new List<V_HIS_SERE_SERV>();
                                dicSereServCDHA[0].Add(item);
                            }
                        }

                        foreach (var item in dicSereServCDHA)
                        {
                            MPS.Processor.Mps000363.PDO.Mps000363ADO Mps000363ADO = new MPS.Processor.Mps000363.PDO.Mps000363ADO();
                            Mps000363ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            Mps000363ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            Mps000363ADO.ratio = chiDinhDichVuADO.Ratio;
                            Mps000363ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                            Mps000363ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                            if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                            {
                                foreach (var sere in item.Value)
                                {
                                    if (dicSereServExtData.ContainsKey(sere.ID))
                                    {
                                        lstExt.Add(dicSereServExtData[sere.ID]);
                                    }
                                }
                            }

                            List<long> _ssIds = item.Value.Select(p => p.SERVICE_ID).Distinct().ToList();
                            var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                            if (dataSS != null && dataSS.Count > 0)
                            {
                                var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                                if (_service != null)
                                {
                                    var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                    Mps000363ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                                }
                            }

                            var parent = ProcessDictionaryData.GetService(item.Key);
                            Mps000363ADO.TITLE = parent != null ? parent.SERVICE_NAME : "";

                            mps000363RDO = new MPS.Processor.Mps000363.PDO.Mps000363PDO(
                                serviceReq,
                                item.Value.ToList(),
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                Mps000363ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                lstExt,
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                                );

                            Print.PrintData(printTypeCode, fileName, mps000363RDO, printNow, ref result, roomId, isView, PreviewType, item.Value.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000363.PDO.Mps000363ADO Mps000363ADO = new MPS.Processor.Mps000363.PDO.Mps000363ADO();
                        Mps000363ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        Mps000363ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        Mps000363ADO.ratio = chiDinhDichVuADO.Ratio;
                        Mps000363ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                        Mps000363ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                        if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                        {
                            foreach (var sere in dicSereServData[serviceReq.ID])
                            {
                                if (dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    lstExt.Add(dicSereServExtData[sere.ID]);
                                }
                            }
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                Mps000363ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        mps000363RDO = new MPS.Processor.Mps000363.PDO.Mps000363PDO(
                            serviceReq,
                            dicSereServData[serviceReq.ID],
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            Mps000363ADO,
                            bedLog,
                            _HIS_DHST,
                            _WORK_PLACE,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            lstExt, 
                            Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                            );

                        Print.PrintData(printTypeCode, fileName, mps000363RDO, printNow, ref result, roomId, isView, PreviewType, dicSereServData[serviceReq.ID].Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000364Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printCDHA)
        {
            try
            {
                var lstServieReq_28 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA].ToList();
                foreach (var serviceReq in lstServieReq_28)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    MPS.Processor.Mps000364.PDO.Mps000364PDO mps000364RDO = null;

                    if (printCDHA)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServCDHA = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
                        {
                            var service = ProcessDictionaryData.GetService(item.SERVICE_ID);
                            if (service != null)
                            {
                                if (!dicSereServCDHA.ContainsKey(service.PARENT_ID ?? 0))
                                    dicSereServCDHA[service.PARENT_ID ?? 0] = new List<V_HIS_SERE_SERV>();
                                dicSereServCDHA[service.PARENT_ID ?? 0].Add(item);
                            }
                            else
                            {
                                if (dicSereServCDHA[0] == null)
                                    dicSereServCDHA[0] = new List<V_HIS_SERE_SERV>();
                                dicSereServCDHA[0].Add(item);
                            }
                        }

                        foreach (var item in dicSereServCDHA)
                        {
                            MPS.Processor.Mps000364.PDO.Mps000364ADO Mps000364ADO = new MPS.Processor.Mps000364.PDO.Mps000364ADO();
                            Mps000364ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            Mps000364ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            Mps000364ADO.ratio = chiDinhDichVuADO.Ratio;
                            Mps000364ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                            Mps000364ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                            if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                            {
                                foreach (var sere in item.Value)
                                {
                                    if (dicSereServExtData.ContainsKey(sere.ID))
                                    {
                                        lstExt.Add(dicSereServExtData[sere.ID]);
                                    }
                                }
                            }

                            var parent = ProcessDictionaryData.GetService(item.Key);
                            Mps000364ADO.TITLE = parent != null ? parent.SERVICE_NAME : "";
                            Mps000364ADO.PARENT_NAME = parent != null ? parent.SERVICE_NAME : "";

                            mps000364RDO = new MPS.Processor.Mps000364.PDO.Mps000364PDO(
                                serviceReq,
                                item.Value.ToList(),
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                Mps000364ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                lstExt, 
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                                );

                            Print.PrintData(printTypeCode, fileName, mps000364RDO, printNow, ref result, roomId, isView, PreviewType, item.Value.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000364.PDO.Mps000364ADO Mps000364ADO = new MPS.Processor.Mps000364.PDO.Mps000364ADO();
                        Mps000364ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        Mps000364ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        Mps000364ADO.ratio = chiDinhDichVuADO.Ratio;
                        Mps000364ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                        Mps000364ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                        if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                        {
                            foreach (var sere in dicSereServData[serviceReq.ID])
                            {
                                if (dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    lstExt.Add(dicSereServExtData[sere.ID]);
                                }
                            }
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                Mps000364ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        mps000364RDO = new MPS.Processor.Mps000364.PDO.Mps000364PDO(
                            serviceReq,
                            dicSereServData[serviceReq.ID],
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            Mps000364ADO,
                            bedLog,
                            _HIS_DHST,
                            _WORK_PLACE,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            lstExt,
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                            );

                        Print.PrintData(printTypeCode, fileName, mps000364RDO, printNow, ref result, roomId, isView, PreviewType, dicSereServData[serviceReq.ID].Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000365Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printCDHA)
        {
            try
            {
                var lstServieReq_28 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA].ToList();
                foreach (var serviceReq in lstServieReq_28)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    MPS.Processor.Mps000365.PDO.Mps000365PDO mps000365RDO = null;

                    if (printCDHA)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServCDHA = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
                        {
                            var service = ProcessDictionaryData.GetService(item.SERVICE_ID);
                            if (service != null)
                            {
                                if (!dicSereServCDHA.ContainsKey(service.PARENT_ID ?? 0))
                                    dicSereServCDHA[service.PARENT_ID ?? 0] = new List<V_HIS_SERE_SERV>();
                                dicSereServCDHA[service.PARENT_ID ?? 0].Add(item);
                            }
                            else
                            {
                                if (dicSereServCDHA[0] == null)
                                    dicSereServCDHA[0] = new List<V_HIS_SERE_SERV>();
                                dicSereServCDHA[0].Add(item);
                            }
                        }

                        foreach (var item in dicSereServCDHA)
                        {
                            MPS.Processor.Mps000365.PDO.Mps000365ADO Mps000365ADO = new MPS.Processor.Mps000365.PDO.Mps000365ADO();
                            Mps000365ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            Mps000365ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            Mps000365ADO.ratio = chiDinhDichVuADO.Ratio;
                            Mps000365ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                            Mps000365ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                            if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                            {
                                foreach (var sere in item.Value)
                                {
                                    if (dicSereServExtData.ContainsKey(sere.ID))
                                    {
                                        lstExt.Add(dicSereServExtData[sere.ID]);
                                    }
                                }
                            }

                            var parent = ProcessDictionaryData.GetService(item.Key);
                            Mps000365ADO.TITLE = parent != null ? parent.SERVICE_NAME : "";
                            Mps000365ADO.PARENT_NAME = parent != null ? parent.SERVICE_NAME : "";

                            mps000365RDO = new MPS.Processor.Mps000365.PDO.Mps000365PDO(
                                serviceReq,
                                item.Value.ToList(),
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                Mps000365ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                lstExt,
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                                );

                            Print.PrintData(printTypeCode, fileName, mps000365RDO, printNow, ref result, roomId, isView, PreviewType, item.Value.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000365.PDO.Mps000365ADO Mps000365ADO = new MPS.Processor.Mps000365.PDO.Mps000365ADO();
                        Mps000365ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        Mps000365ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        Mps000365ADO.ratio = chiDinhDichVuADO.Ratio;
                        Mps000365ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                        Mps000365ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                        if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                        {
                            foreach (var sere in dicSereServData[serviceReq.ID])
                            {
                                if (dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    lstExt.Add(dicSereServExtData[sere.ID]);
                                }
                            }
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                Mps000365ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        mps000365RDO = new MPS.Processor.Mps000365.PDO.Mps000365PDO(
                            serviceReq,
                            dicSereServData[serviceReq.ID],
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            Mps000365ADO,
                            bedLog,
                            _HIS_DHST,
                            _WORK_PLACE,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            lstExt,
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                            );

                        Print.PrintData(printTypeCode, fileName, mps000365RDO, printNow, ref result, roomId, isView, PreviewType, dicSereServData[serviceReq.ID].Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000366Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printTDCN)
        {
            try
            {
                var lstServieReq_38 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN].ToList();
                foreach (var serviceReq in lstServieReq_38)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printTDCN)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000366.PDO.Mps000366ADO mps000366ADO = new MPS.Processor.Mps000366.PDO.Mps000366ADO();
                            mps000366ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000366ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000366ADO.ratio = chiDinhDichVuADO.Ratio;

                            mps000366ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                            if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                            {
                                foreach (var sere in item.Value)
                                {
                                    if (dicSereServExtData.ContainsKey(sere.ID))
                                    {
                                        lstExt.Add(dicSereServExtData[sere.ID]);
                                    }
                                }
                            }

                            var parent = ProcessDictionaryData.GetService(item.Key);
                            mps000366ADO.TITLE = parent != null ? parent.SERVICE_NAME : "";
                            mps000366ADO.PARENT_NAME = parent != null ? parent.SERVICE_NAME : "";

                            MPS.Processor.Mps000366.PDO.Mps000366PDO mps000366RDO = new MPS.Processor.Mps000366.PDO.Mps000366PDO(
                                serviceReq,
                                item.Value,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                mps000366ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                lstExt,
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                                );

                            Print.PrintData(printTypeCode, fileName, mps000366RDO, printNow, ref result, roomId, isView, PreviewType, item.Value.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000366.PDO.Mps000366ADO mps000366ADO = new MPS.Processor.Mps000366.PDO.Mps000366ADO();
                        mps000366ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000366ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000366ADO.ratio = chiDinhDichVuADO.Ratio;

                        mps000366ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                        if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                        {
                            foreach (var sere in dicSereServData[serviceReq.ID])
                            {
                                if (dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    lstExt.Add(dicSereServExtData[sere.ID]);
                                }
                            }
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000366ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        MPS.Processor.Mps000366.PDO.Mps000366PDO mps000366RDO = new MPS.Processor.Mps000366.PDO.Mps000366PDO(
                            serviceReq,
                            dicSereServData[serviceReq.ID],
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            mps000366ADO,
                            bedLog,
                            _HIS_DHST,
                            _WORK_PLACE,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            lstExt,
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null);
                        Print.PrintData(printTypeCode, fileName, mps000366RDO, printNow, ref result, roomId, isView, PreviewType, dicSereServData[serviceReq.ID].Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000367Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printTDCN)
        {
            try
            {
                var lstServieReq_38 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN].ToList();
                foreach (var serviceReq in lstServieReq_38)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printTDCN)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000367.PDO.Mps000367ADO mps000367ADO = new MPS.Processor.Mps000367.PDO.Mps000367ADO();
                            mps000367ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000367ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000367ADO.ratio = chiDinhDichVuADO.Ratio;

                            mps000367ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                            if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                            {
                                foreach (var sere in item.Value)
                                {
                                    if (dicSereServExtData.ContainsKey(sere.ID))
                                    {
                                        lstExt.Add(dicSereServExtData[sere.ID]);
                                    }
                                }
                            }

                            var parent = ProcessDictionaryData.GetService(item.Key);
                            mps000367ADO.TITLE = parent != null ? parent.SERVICE_NAME : "";
                            mps000367ADO.PARENT_NAME = parent != null ? parent.SERVICE_NAME : "";

                            MPS.Processor.Mps000367.PDO.Mps000367PDO mps000367RDO = new MPS.Processor.Mps000367.PDO.Mps000367PDO(
                                serviceReq,
                                item.Value,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                mps000367ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                lstExt,
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                                );

                            Print.PrintData(printTypeCode, fileName, mps000367RDO, printNow, ref result, roomId, isView, PreviewType, item.Value.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000367.PDO.Mps000367ADO mps000367ADO = new MPS.Processor.Mps000367.PDO.Mps000367ADO();
                        mps000367ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000367ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000367ADO.ratio = chiDinhDichVuADO.Ratio;

                        mps000367ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                        if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                        {
                            foreach (var sere in dicSereServData[serviceReq.ID])
                            {
                                if (dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    lstExt.Add(dicSereServExtData[sere.ID]);
                                }
                            }
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000367ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        MPS.Processor.Mps000367.PDO.Mps000367PDO mps000367RDO = new MPS.Processor.Mps000367.PDO.Mps000367PDO(
                            serviceReq,
                            dicSereServData[serviceReq.ID],
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            mps000367ADO,
                            bedLog,
                            _HIS_DHST,
                            _WORK_PLACE,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            lstExt,
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null);
                        Print.PrintData(printTypeCode, fileName, mps000367RDO, printNow, ref result, roomId, isView, PreviewType, dicSereServData[serviceReq.ID].Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000368Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printTDCN)
        {
            try
            {
                var lstServieReq_38 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN].ToList();
                foreach (var serviceReq in lstServieReq_38)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    if (printTDCN)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000368.PDO.Mps000368ADO mps000368ADO = new MPS.Processor.Mps000368.PDO.Mps000368ADO();
                            mps000368ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000368ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000368ADO.ratio = chiDinhDichVuADO.Ratio;

                            mps000368ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                            if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                            {
                                foreach (var sere in item.Value)
                                {
                                    if (dicSereServExtData.ContainsKey(sere.ID))
                                    {
                                        lstExt.Add(dicSereServExtData[sere.ID]);
                                    }
                                }
                            }

                            var parent = ProcessDictionaryData.GetService(item.Key);
                            mps000368ADO.TITLE = parent != null ? parent.SERVICE_NAME : "";
                            mps000368ADO.PARENT_NAME = parent != null ? parent.SERVICE_NAME : "";

                            MPS.Processor.Mps000368.PDO.Mps000368PDO mps000368RDO = new MPS.Processor.Mps000368.PDO.Mps000368PDO(
                                serviceReq,
                                item.Value,
                                chiDinhDichVuADO.patientTypeAlter,
                                chiDinhDichVuADO.treament,
                                mps000368ADO,
                                bedLog,
                                _HIS_DHST,
                                _WORK_PLACE,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                lstExt,
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null
                                );

                            Print.PrintData(printTypeCode, fileName, mps000368RDO, printNow, ref result, roomId, isView, PreviewType, item.Value.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000368.PDO.Mps000368ADO mps000368ADO = new MPS.Processor.Mps000368.PDO.Mps000368ADO();
                        mps000368ADO.bedRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000368ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000368ADO.ratio = chiDinhDichVuADO.Ratio;

                        mps000368ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                        if (dicSereServExtData != null && dicSereServExtData.Count > 0)
                        {
                            foreach (var sere in dicSereServData[serviceReq.ID])
                            {
                                if (dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    lstExt.Add(dicSereServExtData[sere.ID]);
                                }
                            }
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000368ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        MPS.Processor.Mps000368.PDO.Mps000368PDO mps000368RDO = new MPS.Processor.Mps000368.PDO.Mps000368PDO(
                            serviceReq,
                            dicSereServData[serviceReq.ID],
                            chiDinhDichVuADO.patientTypeAlter,
                            chiDinhDichVuADO.treament,
                            mps000368ADO,
                            bedLog,
                            _HIS_DHST,
                            _WORK_PLACE,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            lstExt,
                                Configs,
                                TransReq != null && TransReq.Count > 0 ? (Config.HisTranReqQRCodeTreatmentPrint ? TransReq.FirstOrDefault(o=>o.ID == serviceReq.TRANS_REQ_ID) : TransReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault()) : null);
                        Print.PrintData(printTypeCode, fileName, mps000368RDO, printNow, ref result, roomId, isView, PreviewType, dicSereServData[serviceReq.ID].Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000423Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicReq, Dictionary<long, List<V_HIS_SERE_SERV>> dicSs, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType)
        {
            try
            {
                var lstServieReq_423 = dicReq[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                foreach (var serviceReq in lstServieReq_423)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }
                    MPS.Processor.Mps000423.PDO.Mps000423PDO mps000423RDO = null;

                    if (!dicSs.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    var lstSereServ_423 = new List<MPS.Processor.Mps000423.PDO.Mps000423_ListSereServ>();
                    foreach (var sere in dicSs[serviceReq.ID])
                    {
                        var se423 = new MPS.Processor.Mps000423.PDO.Mps000423_ListSereServ(sere);
                        var service = ProcessDictionaryData.GetService(se423.SERVICE_ID);
                        se423.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                        se423.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                        if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                        {
                            se423.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                            se423.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                            se423.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                            se423.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                            se423.NOTE = dicSereServExtData[sere.ID].NOTE;
                        }

                        lstSereServ_423.Add(se423);
                    }

                    MPS.Processor.Mps000423.PDO.Mps000423ADO mps000423ADO = new MPS.Processor.Mps000423.PDO.Mps000423ADO();
                    mps000423ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                    mps000423ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                    mps000423ADO.ratio = chiDinhDichVuADO.Ratio;
                    mps000423ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                    mps000423ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                    mps000423ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    List<long> _ssIds = dicSs[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                    var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                    if (dataSS != null && dataSS.Count > 0)
                    {
                        var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                        if (_service != null)
                        {
                            var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                            mps000423ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                        }
                    }

                    mps000423RDO = new MPS.Processor.Mps000423.PDO.Mps000423PDO(
                        chiDinhDichVuADO.treament,
                        serviceReq,
                        lstSereServ_423,
                        chiDinhDichVuADO.patientTypeAlter,
                        mps000423ADO,
                        bedLog,
                        null,
                        _HIS_DHST,
                        _WORK_PLACE,
                        BackendDataWorker.Get<V_HIS_SERVICE>()
                        );

                    Print.PrintData(printTypeCode, fileName, mps000423RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_423.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000426Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicReq, Dictionary<long, List<V_HIS_SERE_SERV>> dicSs, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType)
        {
            try
            {
                var lstServieReq_426 = dicReq[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                foreach (var serviceReq in lstServieReq_426)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }
                    MPS.Processor.Mps000426.PDO.Mps000426PDO mps000426RDO = null;

                    if (!dicSs.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    var lstSereServ_426 = new List<MPS.Processor.Mps000426.PDO.Mps000426_ListSereServ>();
                    foreach (var sere in dicSs[serviceReq.ID])
                    {
                        var se426 = new MPS.Processor.Mps000426.PDO.Mps000426_ListSereServ(sere);
                        var service = ProcessDictionaryData.GetService(se426.SERVICE_ID);
                        se426.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                        se426.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                        if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                        {
                            se426.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                            se426.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                            se426.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                            se426.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                            se426.NOTE = dicSereServExtData[sere.ID].NOTE;
                        }

                        lstSereServ_426.Add(se426);
                    }

                    MPS.Processor.Mps000426.PDO.Mps000426ADO mps000426ADO = new MPS.Processor.Mps000426.PDO.Mps000426ADO();
                    mps000426ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                    mps000426ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                    mps000426ADO.ratio = chiDinhDichVuADO.Ratio;
                    mps000426ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                    mps000426ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                    mps000426ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    List<long> _ssIds = dicSs[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                    var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                    if (dataSS != null && dataSS.Count > 0)
                    {
                        var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                        if (_service != null)
                        {
                            var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                            mps000426ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                        }
                    }

                    mps000426RDO = new MPS.Processor.Mps000426.PDO.Mps000426PDO(
                        chiDinhDichVuADO.treament,
                        serviceReq,
                        lstSereServ_426,
                        chiDinhDichVuADO.patientTypeAlter,
                        mps000426ADO,
                        bedLog,
                        null,
                        _HIS_DHST,
                        _WORK_PLACE,
                        BackendDataWorker.Get<V_HIS_SERVICE>()
                        );

                    Print.PrintData(printTypeCode, fileName, mps000426RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_426.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000425Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicReq, Dictionary<long, List<V_HIS_SERE_SERV>> dicSs, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType)
        {
            try
            {
                var lstServieReq_425 = dicReq[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                foreach (var serviceReq in lstServieReq_425)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }
                    MPS.Processor.Mps000425.PDO.Mps000425PDO mps000425RDO = null;

                    if (!dicSs.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    var lstSereServ_425 = new List<MPS.Processor.Mps000425.PDO.Mps000425_ListSereServ>();
                    foreach (var sere in dicSs[serviceReq.ID])
                    {
                        var se425 = new MPS.Processor.Mps000425.PDO.Mps000425_ListSereServ(sere);
                        var service = ProcessDictionaryData.GetService(se425.SERVICE_ID);
                        se425.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                        se425.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                        if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                        {
                            se425.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                            se425.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                            se425.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                            se425.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                            se425.NOTE = dicSereServExtData[sere.ID].NOTE;
                        }

                        lstSereServ_425.Add(se425);
                    }

                    MPS.Processor.Mps000425.PDO.Mps000425ADO mps000425ADO = new MPS.Processor.Mps000425.PDO.Mps000425ADO();
                    mps000425ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                    mps000425ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                    mps000425ADO.ratio = chiDinhDichVuADO.Ratio;
                    mps000425ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                    mps000425ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                    mps000425ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    List<long> _ssIds = dicSs[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                    var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                    if (dataSS != null && dataSS.Count > 0)
                    {
                        var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                        if (_service != null)
                        {
                            var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                            mps000425ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                        }
                    }

                    mps000425RDO = new MPS.Processor.Mps000425.PDO.Mps000425PDO(
                        chiDinhDichVuADO.treament,
                        serviceReq,
                        lstSereServ_425,
                        chiDinhDichVuADO.patientTypeAlter,
                        mps000425ADO,
                        bedLog,
                        null,
                        _HIS_DHST,
                        _WORK_PLACE,
                        BackendDataWorker.Get<V_HIS_SERVICE>()
                        );

                    Print.PrintData(printTypeCode, fileName, mps000425RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_425.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000424Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicReq, Dictionary<long, List<V_HIS_SERE_SERV>> dicSs, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType)
        {
            try
            {
                var lstServieReq_424 = dicReq[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                foreach (var serviceReq in lstServieReq_424)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }
                    MPS.Processor.Mps000424.PDO.Mps000424PDO mps000424RDO = null;

                    if (!dicSs.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    var lstSereServ_424 = new List<MPS.Processor.Mps000424.PDO.Mps000424_ListSereServ>();
                    foreach (var sere in dicSs[serviceReq.ID])
                    {
                        var se424 = new MPS.Processor.Mps000424.PDO.Mps000424_ListSereServ(sere);
                        var service = ProcessDictionaryData.GetService(se424.SERVICE_ID);
                        se424.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                        se424.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                        if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                        {
                            se424.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                            se424.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                            se424.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                            se424.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                            se424.NOTE = dicSereServExtData[sere.ID].NOTE;
                        }

                        lstSereServ_424.Add(se424);
                    }

                    MPS.Processor.Mps000424.PDO.Mps000424ADO mps000424ADO = new MPS.Processor.Mps000424.PDO.Mps000424ADO();
                    mps000424ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                    mps000424ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                    mps000424ADO.ratio = chiDinhDichVuADO.Ratio;
                    mps000424ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                    mps000424ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                    mps000424ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    List<long> _ssIds = dicSs[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                    var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                    if (dataSS != null && dataSS.Count > 0)
                    {
                        var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                        if (_service != null)
                        {
                            var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                            mps000424ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                        }
                    }

                    mps000424RDO = new MPS.Processor.Mps000424.PDO.Mps000424PDO(
                        chiDinhDichVuADO.treament,
                        serviceReq,
                        lstSereServ_424,
                        chiDinhDichVuADO.patientTypeAlter,
                        mps000424ADO,
                        bedLog,
                        null,
                        _HIS_DHST,
                        _WORK_PLACE,
                        BackendDataWorker.Get<V_HIS_SERVICE>()
                        );

                    Print.PrintData(printTypeCode, fileName, mps000424RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_424.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Phiếu yêu cầu xét nghiệm Test
        private void Mps000465Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printXN)
        {
            try
            {
                var lstServieReq_465 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                foreach (var serviceReq in lstServieReq_465)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }
                    MPS.Processor.Mps000465.PDO.Mps000465PDO mps000465RDO = null;
                    Inventec.Common.Logging.LogSystem.Debug("printXN " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printXN), printXN));

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    LIS.EFMODEL.DataModels.V_LIS_SAMPLE lisSample = null;
                    if (this.LisSamples != null && this.LisSamples.Count > 0)
                    {
                        var sample = this.LisSamples.Where(o => o.SERVICE_REQ_CODE == serviceReq.SERVICE_REQ_CODE).ToList();
                        if (sample != null && sample.Count > 0)
                        {
                            lisSample = sample.OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                        }
                    }

                    if (printXN)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000465.PDO.Mps000465ADO mps000465ADO = new MPS.Processor.Mps000465.PDO.Mps000465ADO();
                            mps000465ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000465ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000465ADO.ratio = chiDinhDichVuADO.Ratio;
                            mps000465ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                            mps000465ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000465ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            mps000465ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_465 = new List<MPS.Processor.Mps000465.PDO.Mps000465_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se465 = new MPS.Processor.Mps000465.PDO.Mps000465_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se465.SERVICE_ID);
                                se465.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se465.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se465.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se465.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se465.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se465.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se465.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                if (sere.SERVICE_CONDITION_ID != null)
                                {
                                    var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);

                                    if (Condition != null)
                                    {
                                        se465.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                        se465.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                    }
                                }
                                lstSereServ_465.Add(se465);
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_465.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_465.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            mps000465RDO = new MPS.Processor.Mps000465.PDO.Mps000465PDO(
                                chiDinhDichVuADO.treament,
                                serviceReq,
                                lstSereServ_465,
                                chiDinhDichVuADO.patientTypeAlter,
                                mps000465ADO,
                                bedLog,
                                serviceParent,
                                _HIS_DHST,
                                _WORK_PLACE,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                                lisSample
                                );

                            Print.PrintData(printTypeCode, fileName, mps000465RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_465.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000465.PDO.Mps000465ADO mps000465ADO = new MPS.Processor.Mps000465.PDO.Mps000465ADO();
                        mps000465ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000465ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000465ADO.ratio = chiDinhDichVuADO.Ratio;
                        mps000465ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                        mps000465ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000465ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_465 = new List<MPS.Processor.Mps000465.PDO.Mps000465_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se465 = new MPS.Processor.Mps000465.PDO.Mps000465_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se465.SERVICE_ID);
                            se465.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se465.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se465.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se465.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se465.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se465.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se465.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }

                            if (sere.SERVICE_CONDITION_ID != null)
                            {
                                var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);

                                if (Condition != null)
                                {
                                    se465.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                    se465.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                }
                            }

                            lstSereServ_465.Add(se465);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000465ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_465.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_465.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        mps000465RDO = new MPS.Processor.Mps000465.PDO.Mps000465PDO(
                            chiDinhDichVuADO.treament,
                            serviceReq,
                            lstSereServ_465,
                            chiDinhDichVuADO.patientTypeAlter,
                            mps000465ADO,
                            bedLog,
                            null,
                            _HIS_DHST,
                            _WORK_PLACE,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction,
                            lisSample
                            );

                        Print.PrintData(printTypeCode, fileName, mps000465RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_465.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Phiếu yêu cầu xét nghiệm Giải phẫu bệnh
        private void Mps000466Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printXN)
        {
            try
            {
                var lstServieReq_466 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                foreach (var serviceReq in lstServieReq_466)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }
                    MPS.Processor.Mps000466.PDO.Mps000466PDO mps000466RDO = null;

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    LIS.EFMODEL.DataModels.V_LIS_SAMPLE lisSample = null;
                    if (this.LisSamples != null && this.LisSamples.Count > 0)
                    {
                        var sample = this.LisSamples.Where(o => o.SERVICE_REQ_CODE == serviceReq.SERVICE_REQ_CODE).ToList();
                        if (sample != null && sample.Count > 0)
                        {
                            lisSample = sample.OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                        }
                    }

                    if (printXN)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000466.PDO.Mps000466ADO mps000466ADO = new MPS.Processor.Mps000466.PDO.Mps000466ADO();
                            mps000466ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000466ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000466ADO.ratio = chiDinhDichVuADO.Ratio;
                            mps000466ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                            mps000466ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000466ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            mps000466ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_466 = new List<MPS.Processor.Mps000466.PDO.Mps000466_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se466 = new MPS.Processor.Mps000466.PDO.Mps000466_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se466.SERVICE_ID);
                                se466.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se466.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se466.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se466.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se466.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se466.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se466.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                if (sere.SERVICE_CONDITION_ID != null)
                                {
                                    var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);

                                    if (Condition != null)
                                    {
                                        se466.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                        se466.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                    }
                                }
                                lstSereServ_466.Add(se466);
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_466.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_466.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            mps000466RDO = new MPS.Processor.Mps000466.PDO.Mps000466PDO(
                                chiDinhDichVuADO.treament,
                                serviceReq,
                                lstSereServ_466,
                                chiDinhDichVuADO.patientTypeAlter,
                                mps000466ADO,
                                bedLog,
                                serviceParent,
                                _HIS_DHST,
                                _WORK_PLACE,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                                lisSample
                                );

                            Print.PrintData(printTypeCode, fileName, mps000466RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_466.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000466.PDO.Mps000466ADO mps000466ADO = new MPS.Processor.Mps000466.PDO.Mps000466ADO();
                        mps000466ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000466ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000466ADO.ratio = chiDinhDichVuADO.Ratio;
                        mps000466ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                        mps000466ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000466ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_466 = new List<MPS.Processor.Mps000466.PDO.Mps000466_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se466 = new MPS.Processor.Mps000466.PDO.Mps000466_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se466.SERVICE_ID);
                            se466.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se466.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se466.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se466.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se466.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se466.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se466.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }

                            if (sere.SERVICE_CONDITION_ID != null)
                            {
                                var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);

                                if (Condition != null)
                                {
                                    se466.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                    se466.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                }
                            }

                            lstSereServ_466.Add(se466);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000466ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_466.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_466.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        mps000466RDO = new MPS.Processor.Mps000466.PDO.Mps000466PDO(
                            chiDinhDichVuADO.treament,
                            serviceReq,
                            lstSereServ_466,
                            chiDinhDichVuADO.patientTypeAlter,
                            mps000466ADO,
                            bedLog,
                            null,
                            _HIS_DHST,
                            _WORK_PLACE,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction,
                            lisSample
                            );

                        Print.PrintData(printTypeCode, fileName, mps000466RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_466.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Phiếu yêu cầu xét nghiệm Nước tiểu
        private void Mps000467Print(string printTypeCode, string fileName, ADO.ChiDinhDichVuADO chiDinhDichVuADO, Dictionary<long, List<V_HIS_SERVICE_REQ>> dicServiceReqData, Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServData, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData, List<V_HIS_BED_LOG> bedLogs, bool printNow, ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType, bool printXN)
        {
            try
            {
                var lstServieReq_467 = dicServiceReqData[IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN].ToList();
                foreach (var serviceReq in lstServieReq_467)
                {
                    V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                    if (bedLogs != null && bedLogs.Count > 0)
                    {
                        bedLog = bedLogs.Where(o => o.START_TIME <= serviceReq.INTRUCTION_TIME).OrderByDescending(o => o.START_TIME).FirstOrDefault();
                    }
                    MPS.Processor.Mps000467.PDO.Mps000467PDO mps000467RDO = null;

                    if (!dicSereServData.ContainsKey(serviceReq.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Error("dicSereServData ko co: " + serviceReq.SERVICE_REQ_CODE);
                        continue;
                    }

                    HisServiceReqMaxNumOrderSDO roomSdo = null;
                    if (ReqMaxNumOrderSDO != null && ReqMaxNumOrderSDO.Count > 0)
                    {
                        roomSdo = ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == serviceReq.EXECUTE_ROOM_ID);
                    }

                    var acsUser = ProcessDictionaryData.GetUserMobile(serviceReq.REQUEST_LOGINNAME);

                    LIS.EFMODEL.DataModels.V_LIS_SAMPLE lisSample = null;
                    if (this.LisSamples != null && this.LisSamples.Count > 0)
                    {
                        var sample = this.LisSamples.Where(o => o.SERVICE_REQ_CODE == serviceReq.SERVICE_REQ_CODE).ToList();
                        if (sample != null && sample.Count > 0)
                        {
                            lisSample = sample.OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                        }
                    }

                    if (printXN)
                    {
                        var listSereServ = dicSereServData[serviceReq.ID].ToList();
                        Dictionary<long, List<V_HIS_SERE_SERV>> dicSereServTest = new Dictionary<long, List<V_HIS_SERE_SERV>>();

                        foreach (var item in listSereServ)
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
                            MPS.Processor.Mps000467.PDO.Mps000467ADO mps000467ADO = new MPS.Processor.Mps000467.PDO.Mps000467ADO();
                            mps000467ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                            mps000467ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                            mps000467ADO.ratio = chiDinhDichVuADO.Ratio;
                            mps000467ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                            mps000467ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                            mps000467ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            V_HIS_SERVICE serviceParent = ProcessDictionaryData.GetService(item.Key);
                            mps000467ADO.PARENT_NAME = serviceParent != null ? serviceParent.SERVICE_NAME : "";

                            var lstSereServ_467 = new List<MPS.Processor.Mps000467.PDO.Mps000467_ListSereServ>();
                            foreach (var sere in item.Value)
                            {
                                var se467 = new MPS.Processor.Mps000467.PDO.Mps000467_ListSereServ(sere);
                                var service = ProcessDictionaryData.GetService(se467.SERVICE_ID);
                                se467.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                                se467.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                                if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                                {
                                    se467.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                    se467.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                    se467.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                    se467.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                    se467.NOTE = dicSereServExtData[sere.ID].NOTE;
                                }

                                if (sere.SERVICE_CONDITION_ID != null)
                                {
                                    var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);

                                    if (Condition != null)
                                    {
                                        se467.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                        se467.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                    }
                                }
                                lstSereServ_467.Add(se467);
                            }

                            List<V_HIS_TRANSACTION> transaction = null;
                            if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                            {
                                List<long> transactionIds = new List<long>();
                                if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_467.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                                }

                                if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                                {
                                    transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_467.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                                }

                                transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                            }

                            mps000467RDO = new MPS.Processor.Mps000467.PDO.Mps000467PDO(
                                chiDinhDichVuADO.treament,
                                serviceReq,
                                lstSereServ_467,
                                chiDinhDichVuADO.patientTypeAlter,
                                mps000467ADO,
                                bedLog,
                                serviceParent,
                                _HIS_DHST,
                                _WORK_PLACE,
                                BackendDataWorker.Get<V_HIS_SERVICE>(),
                                chiDinhDichVuADO.ListSereServDeposit,
                                chiDinhDichVuADO.ListSereServBill,
                                transaction,
                                lisSample
                                );

                            Print.PrintData(printTypeCode, fileName, mps000467RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_467.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                        }
                    }
                    else
                    {
                        MPS.Processor.Mps000467.PDO.Mps000467ADO mps000467ADO = new MPS.Processor.Mps000467.PDO.Mps000467ADO();
                        mps000467ADO.bebRoomName = chiDinhDichVuADO.BedRoomName;
                        mps000467ADO.firstExamRoomName = chiDinhDichVuADO.FirstExamRoomName;
                        mps000467ADO.ratio = chiDinhDichVuADO.Ratio;
                        mps000467ADO.PatientTypeId__Bhyt = Config.PatientTypeId__BHYT;

                        mps000467ADO.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo != null ? (long?)roomSdo.MAX_NUM_ORDER : null;
                        mps000467ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        var lstSereServ_467 = new List<MPS.Processor.Mps000467.PDO.Mps000467_ListSereServ>();
                        foreach (var sere in dicSereServData[serviceReq.ID])
                        {
                            var se467 = new MPS.Processor.Mps000467.PDO.Mps000467_ListSereServ(sere);
                            var service = ProcessDictionaryData.GetService(se467.SERVICE_ID);
                            se467.SERVICE_NUM_ORDER = service != null ? service.NUM_ORDER : null;
                            se467.ESTIMATE_DURATION = service != null ? service.ESTIMATE_DURATION : null;
                            if (dicSereServExtData != null && dicSereServExtData.ContainsKey(sere.ID))
                            {
                                se467.CONCLUDE = dicSereServExtData[sere.ID].CONCLUDE;
                                se467.BEGIN_TIME = dicSereServExtData[sere.ID].BEGIN_TIME;
                                se467.END_TIME = dicSereServExtData[sere.ID].END_TIME;
                                se467.INSTRUCTION_NOTE = dicSereServExtData[sere.ID].INSTRUCTION_NOTE;
                                se467.NOTE = dicSereServExtData[sere.ID].NOTE;
                            }

                            if (sere.SERVICE_CONDITION_ID != null)
                            {
                                var Condition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == sere.SERVICE_CONDITION_ID);

                                if (Condition != null)
                                {
                                    se467.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                    se467.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                }
                            }

                            lstSereServ_467.Add(se467);
                        }

                        List<long> _ssIds = dicSereServData[serviceReq.ID].Select(p => p.SERVICE_ID).Distinct().ToList();
                        var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                        if (dataSS != null && dataSS.Count > 0)
                        {
                            var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                            if (_service != null)
                            {
                                var serviceN = ProcessDictionaryData.GetService(_service.PARENT_ID.Value);
                                mps000467ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                            }
                        }

                        List<V_HIS_TRANSACTION> transaction = null;
                        if (chiDinhDichVuADO.ListTransaction != null && chiDinhDichVuADO.ListTransaction.Count > 0)
                        {
                            List<long> transactionIds = new List<long>();
                            if (chiDinhDichVuADO.ListSereServBill != null && chiDinhDichVuADO.ListSereServBill.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServBill.Where(o => lstSereServ_467.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.BILL_ID));
                            }

                            if (chiDinhDichVuADO.ListSereServDeposit != null && chiDinhDichVuADO.ListSereServDeposit.Count > 0)
                            {
                                transactionIds.AddRange(chiDinhDichVuADO.ListSereServDeposit.Where(o => lstSereServ_467.Exists(e => e.ID == o.SERE_SERV_ID)).Select(s => s.DEPOSIT_ID));
                            }

                            transaction = chiDinhDichVuADO.ListTransaction.Where(o => transactionIds.Contains(o.ID)).ToList();
                        }

                        mps000467RDO = new MPS.Processor.Mps000467.PDO.Mps000467PDO(
                            chiDinhDichVuADO.treament,
                            serviceReq,
                            lstSereServ_467,
                            chiDinhDichVuADO.patientTypeAlter,
                            mps000467ADO,
                            bedLog,
                            null,
                            _HIS_DHST,
                            _WORK_PLACE,
                            BackendDataWorker.Get<V_HIS_SERVICE>(),
                            chiDinhDichVuADO.ListSereServDeposit,
                            chiDinhDichVuADO.ListSereServBill,
                            transaction,
                            lisSample
                            );

                        Print.PrintData(printTypeCode, fileName, mps000467RDO, printNow, ref result, roomId, isView, PreviewType, lstSereServ_467.Count, this.SavedData, serviceReq.TREATMENT_CODE, this.DlgSendResultSigned);
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
