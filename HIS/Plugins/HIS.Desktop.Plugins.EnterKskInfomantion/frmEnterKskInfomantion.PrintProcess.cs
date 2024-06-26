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
using HIS.Desktop.Plugins.Library.EmrGenerate;
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

namespace HIS.Desktop.Plugins.EnterKskInfomantion
{
    partial class frmEnterKskInfomantion
    {
        const string printTypeCode = "MPS000449";
        private Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        CommonParam _KSK_param = new CommonParam();
        private bool printNow;
        List<V_HIS_SERVICE_REQ> listServiceReqForPrint;
        List<HIS_KSK_GENERAL> listKskGeneral;
        List<HIS_SERE_SERV> listSereServ;
        List<HIS_SERE_SERV_TEIN> listSereServTein;
        List<HIS_HEALTH_EXAM_RANK> listHealthExamRank;
        List<HIS_DHST> listDhst;
        List<HIS_TEST_INDEX> listTestIndex;
        List<HIS_SERE_SERV_EXT> listSereServExt;

        public void PrintProcess(string PrintTypeCode, List<ADO.ServiceReqADO> listPrint)
        {
            try
            {
                listServiceReqForPrint = new List<V_HIS_SERVICE_REQ>();
                listKskGeneral = new List<HIS_KSK_GENERAL>();
                listSereServ = new List<HIS_SERE_SERV>();
                listSereServTein = new List<HIS_SERE_SERV_TEIN>();
                listHealthExamRank = new List<HIS_HEALTH_EXAM_RANK>();
                listDhst = new List<HIS_DHST>();
                listTestIndex = new List<HIS_TEST_INDEX>();
                listSereServExt = new List<HIS_SERE_SERV_EXT>();
                WaitingManager.Show();
                int start = 0;
                int count = listPrint.Count;
                while (count > 0)
                {
                    int limit = (count <= 100) ? count : 100;
                    var listSub = listPrint.Skip(start).Take(limit).ToList();
                    List<long> _serviceReqIds = new List<long>();
                    _serviceReqIds = listSub.Select(p => p.ID).Distinct().ToList();
                    CreateThreadByTreatmentIds(_serviceReqIds);

                    start += 100;
                    count -= 100;
                }
                WaitingManager.Hide();
                Print(PrintTypeCode, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Print(string PrintTypeCode, bool PrintNow)
        {
            try
            {


                Inventec.Common.Logging.LogSystem.Info("Begin Print: Phieu tu van KSK");
                this.printNow = PrintNow;
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);

                richEditorMain.RunPrintTemplate(PrintTypeCode, DelegateRunPrinter);
                Inventec.Common.Logging.LogSystem.Info("End Print: Phieu tu van KSK");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000449.PDO.Mps000449PDO mps000449RDO = new MPS.Processor.Mps000449.PDO.Mps000449PDO(
                    listServiceReqForPrint,
                    listKskGeneral,
                    listSereServ,
                    listSereServTein,
                    listHealthExamRank,
                    listDhst,
                    listTestIndex,
                    listSereServExt);
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (listServiceReqForPrint != null && listServiceReqForPrint.Count == 1)
                {
                    var Treatments = listServiceReqForPrint.FirstOrDefault();

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatments != null ? Treatments.TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000449RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000449RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                }
                else
                {
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000449RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000449RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        private void CreateThreadByTreatmentIds(List<long> _serviceReqIds)
        {
            try
            {
                GetServiceReq_KSK(_serviceReqIds);
                GetSereServ_KSK(_serviceReqIds);
                GetSereServTein_KSK();
                GetKskGeneral_KSK(_serviceReqIds);
                GetHealthExamRank_KSK();
                GetDhst_KSK();
                GetTestIndex_KSK();
                GetSereServExt_KSK(_serviceReqIds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread1(object data)
        {
            try
            {
                GetServiceReq_KSK((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceReq_KSK(List<long> _serviceReqIds)
        {
            try
            {
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.IDs = _serviceReqIds;
                var rs = new BackendAdapter(_KSK_param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, _KSK_param);
                if (rs != null && rs.Count > 0)
                {
                    listServiceReqForPrint.AddRange(rs);
                    listServiceReqForPrint = listServiceReqForPrint.OrderByDescending(o => o.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Thread2(object data)
        {
            try
            {
                GetSereServ_KSK((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServ_KSK(List<long> _serviceReqIds)
        {
            try
            {
                if (listServiceReqForPrint != null && listServiceReqForPrint.Count > 0)
                {
                    MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                    filter.TREATMENT_IDs = listServiceReqForPrint.Select(o=>o.TREATMENT_ID).ToList();
                    var rs = new BackendAdapter(_KSK_param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, _KSK_param).ToList();
                    if (rs != null && rs.Count > 0)
                    {
                        listSereServ.AddRange(rs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }

        private void Thread3()
        {
            try
            {
                GetSereServTein_KSK();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServTein_KSK()
        {
            try
            {
                if (listSereServ != null && listSereServ.Count > 0)
                {
                    MOS.Filter.HisSereServTeinFilter filter = new MOS.Filter.HisSereServTeinFilter();
                    filter.SERE_SERV_IDs = this.listSereServ.Select(o => o.ID).ToList();
                    var rs = new BackendAdapter(_KSK_param).Get<List<HIS_SERE_SERV_TEIN>>("api/HisSereServTein/Get", ApiConsumers.MosConsumer, filter, _KSK_param).ToList();
                    if (rs != null && rs.Count > 0)
                    {
                        listSereServTein.AddRange(rs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }

        private void Thread4(object data)
        {
            try
            {
                GetKskGeneral_KSK((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetKskGeneral_KSK(List<long> _serviceReqIds)
        {
            try
            {
                MOS.Filter.HisKskGeneralFilter filter = new MOS.Filter.HisKskGeneralFilter();
                filter.SERVICE_REQ_IDs = _serviceReqIds;
                var rs = new BackendAdapter(_KSK_param).Get<List<HIS_KSK_GENERAL>>("api/HisKskGeneral/Get", ApiConsumers.MosConsumer, filter, _KSK_param).ToList();
                if (rs != null && rs.Count > 0)
                {
                    listKskGeneral.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }

        private void Thread5()
        {
            try
            {
                GetHealthExamRank_KSK();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetHealthExamRank_KSK()
        {
            try
            {

               listHealthExamRank =  BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread6()
        {
            try
            {
                GetDhst_KSK();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDhst_KSK()
        {
            try
            {
                if (listKskGeneral != null && listKskGeneral.Count > 0)
                {
                    HisDhstFilter filter = new HisDhstFilter();
                    filter.IDs = listKskGeneral.Select(o => o.DHST_ID ?? 0).ToList();
                    var rs = new BackendAdapter(_KSK_param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, _KSK_param).ToList();
                    if (rs != null && rs.Count > 0)
                    {
                        listDhst.AddRange(rs);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread7()
        {
            try
            {
                GetTestIndex_KSK();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTestIndex_KSK()
        {
            try
            {
                if (listSereServTein != null && listSereServTein.Count > 0)
                {
                    HisTestIndexFilter filter = new HisTestIndexFilter();
                    filter.IDs = listSereServTein.Select(o => o.TEST_INDEX_ID ?? 0).ToList();
                    var rs = new BackendAdapter(_KSK_param).Get<List<HIS_TEST_INDEX>>("api/HisTestIndex/Get", ApiConsumers.MosConsumer, filter, _KSK_param).ToList();
                    if (rs != null && rs.Count > 0)
                    {
                        listTestIndex.AddRange(rs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread8(object data)
        {
            try
            {
                GetSereServExt_KSK((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServExt_KSK(List<long> _serviceReqIds)
        {
            try
            {
                if (listSereServTein != null && listSereServTein.Count > 0)
                {
                    HisSereServExtFilter filter = new HisSereServExtFilter();
                    filter.TDL_SERVICE_REQ_IDs = _serviceReqIds;
                    var rs = new BackendAdapter(_KSK_param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, _KSK_param).ToList();
                    if (rs != null && rs.Count > 0)
                    {
                        listSereServExt.AddRange(rs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
