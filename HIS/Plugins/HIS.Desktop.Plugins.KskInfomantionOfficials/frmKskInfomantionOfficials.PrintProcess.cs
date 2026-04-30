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

namespace HIS.Desktop.Plugins.KskInfomantionOfficials
{
    partial class frmKskInfomantionOfficials
    {
        // Cache du lieu load song song bang Thread cho Mps000507 (giong pattern Mps000481 trong UCTreatmentList)
        CommonParam _print507_param;
        List<V_HIS_SERE_SERV> _print507_SereServs;
        List<HIS_SERVICE> _print507_Services;
        List<HIS_SERE_SERV_EXT> _print507_SereServExts;
        List<V_HIS_TEST_INDEX> _print507_TestIndexs;
        List<V_HIS_SERE_SERV_TEIN> _print507_SereServTeins;

        public void PrintMps000507()
        {
            try
            {
                if (currentData == null) return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                _print507_param = param;
                _print507_SereServs = new List<V_HIS_SERE_SERV>();
                _print507_Services = new List<HIS_SERVICE>();
                _print507_SereServExts = new List<HIS_SERE_SERV_EXT>();
                _print507_TestIndexs = new List<V_HIS_TEST_INDEX>();
                _print507_SereServTeins = new List<V_HIS_SERE_SERV_TEIN>();

                // 1. Service Req
                V_HIS_SERVICE_REQ serviceReq = null;
                {
                    var filter = new HisServiceReqFilter();
                    filter.ID = currentData.ID;
                    var rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
                    serviceReq = rs != null && rs.Count > 0 ? rs.FirstOrDefault() : null;
                }

                // 2. KSK General
                HIS_KSK_GENERAL kskGeneral = currentData.KSK_GENERAL;

                // 3. DHST
                HIS_DHST dhst = new HIS_DHST();
                if (kskGeneral != null && kskGeneral.HIS_DHST != null)
                {
                    dhst = kskGeneral.HIS_DHST;
                }

                // 4. Treatment
                V_HIS_TREATMENT_4 treatment = null;
                List<long> _treatmentIds = new List<long>();
                if (currentData.TREATMENT_ID > 0)
                {
                    var tFilter = new HisTreatmentView4Filter { ID = currentData.TREATMENT_ID };
                    var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumers.MosConsumer, tFilter, param);
                    treatment = treatments != null && treatments.Count > 0 ? treatments.FirstOrDefault() : null;
                    _treatmentIds.Add(currentData.TREATMENT_ID);
                }

                // 4.1. Load song song SereServ + SereServExt + SereServTein theo TREATMENT_IDs (3 thread)
                if (_treatmentIds.Count > 0)
                {
                    CreateThreadByTreatmentIds_507(_treatmentIds);

                    // Sau khi co SereServ -> load HIS_SERVICE theo SERVICE_IDs
                    var serviceIds = (_print507_SereServs ?? new List<V_HIS_SERE_SERV>())
                        .Select(o => o.SERVICE_ID).Distinct().ToList();
                    GetService_507(serviceIds);

                    // Sau khi co SereServTein -> load V_HIS_TEST_INDEX theo IDs
                    var testIndexIds = (_print507_SereServTeins ?? new List<V_HIS_SERE_SERV_TEIN>())
                        .Where(o => o.TEST_INDEX_ID != null)
                        .Select(o => (long)o.TEST_INDEX_ID).Distinct().ToList();
                    GetTestIndex_507(testIndexIds);
                }

                // 5. Health Exam Ranks
                var examRanks = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                // 6. Disease Details (V_HIS_DISEASE_DETAIL)
                var diseaseDetailList = diseaseDetails ?? new List<V_HIS_DISEASE_DETAIL>();

                // 7. Disease Detail Results
                var diseaseResultList = diseaseResults ?? new List<HIS_DISEASE_DETAIL_RESULT>();

                // 8. Employees (lay tu BackendDataWorker cache — phuc vu lookup ten BS theo loginname trong template)
                var employees = BackendDataWorker.Get<V_HIS_EMPLOYEE>() ?? new List<V_HIS_EMPLOYEE>();

                WaitingManager.Hide();

                // Tạo PDO (constructor day du 13 tham so — them V_HIS_EMPLOYEE)
                MPS.Processor.Mps000507.PDO.Mps000507PDO rdo = new MPS.Processor.Mps000507.PDO.Mps000507PDO(
                    kskGeneral,
                    serviceReq,
                    dhst,
                    treatment,
                    examRanks,
                    diseaseDetailList,
                    diseaseResultList,
                    _print507_SereServs,
                    _print507_Services,
                    _print507_SereServExts,
                    _print507_TestIndexs,
                    _print507_SereServTeins,
                    employees
                );

                // Gọi print
                string mps000507Code = "Mps000507";
                Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumer.ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditor.RunPrintTemplate(mps000507Code, delegate (string printTypeCode507, string fileName507)
                {
                    bool printResult = false;
                    try
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor()
                            .GenerateInputADOWithPrintTypeCode(
                                serviceReq != null ? serviceReq.TREATMENT_CODE : "",
                                mps000507Code,
                                moduleData != null ? moduleData.RoomId : 0);

                        MPS.ProcessorBase.Core.PrintData printData;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode507, fileName507, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode507, fileName507, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        printResult = MPS.MpsPrinter.Run(printData);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    return printResult;
                });
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Load song song SereServ/SereServExt/SereServTein cho Mps000507 (tham khao Mps000481/UCTreatmentList)

        private void CreateThreadByTreatmentIds_507(List<long> _treatmentIds)
        {
            Thread t1 = new Thread(new ParameterizedThreadStart(Thread507_GetSereServ));
            Thread t2 = new Thread(new ParameterizedThreadStart(Thread507_GetSereServExt));
            Thread t3 = new Thread(new ParameterizedThreadStart(Thread507_GetSereServTein));
            try
            {
                t1.Start(_treatmentIds);
                t2.Start(_treatmentIds);
                t3.Start(_treatmentIds);
                t1.Join();
                t2.Join();
                t3.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                t1.Abort();
                t2.Abort();
                t3.Abort();
            }
        }

        private void Thread507_GetSereServ(object data)
        {
            try
            {
                GetSereServ_507((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread507_GetSereServExt(object data)
        {
            try
            {
                GetSereServExt_507((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread507_GetSereServTein(object data)
        {
            try
            {
                GetSereServTein_507((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServ_507(List<long> treatmentIds)
        {
            try
            {
                if (treatmentIds == null || treatmentIds.Count == 0) return;
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_IDs = treatmentIds;

                var rs = new BackendAdapter(_print507_param).Get<List<V_HIS_SERE_SERV>>(
                    "api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, _print507_param);
                if (rs != null && rs.Count > 0)
                {
                    _print507_SereServs.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetSereServExt_507(List<long> treatmentIds)
        {
            try
            {
                if (treatmentIds == null || treatmentIds.Count == 0) return;
                HisSereServExtFilter filter = new HisSereServExtFilter();
                filter.TDL_TREATMENT_IDs = treatmentIds;

                var rs = new BackendAdapter(_print507_param).Get<List<HIS_SERE_SERV_EXT>>(
                    "api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, _print507_param);
                if (rs != null && rs.Count > 0)
                {
                    _print507_SereServExts.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetSereServTein_507(List<long> treatmentIds)
        {
            try
            {
                if (treatmentIds == null || treatmentIds.Count == 0) return;
                HisSereServTeinViewFilter filter = new HisSereServTeinViewFilter();
                filter.TDL_TREATMENT_IDs = treatmentIds;

                var rs = new BackendAdapter(_print507_param).Get<List<V_HIS_SERE_SERV_TEIN>>(
                    "api/HisSereServTein/GetView", ApiConsumers.MosConsumer, filter, _print507_param);
                if (rs != null && rs.Count > 0)
                {
                    _print507_SereServTeins.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetService_507(List<long> serviceIds)
        {
            try
            {
                if (serviceIds == null || serviceIds.Count == 0) return;
                HisServiceFilter filter = new HisServiceFilter();
                filter.IDs = serviceIds;

                var rs = new BackendAdapter(_print507_param).Get<List<HIS_SERVICE>>(
                    "api/HisService/Get", ApiConsumers.MosConsumer, filter, _print507_param);
                if (rs != null && rs.Count > 0)
                {
                    _print507_Services.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetTestIndex_507(List<long> testIndexIds)
        {
            try
            {
                if (testIndexIds == null || testIndexIds.Count == 0) return;
                HisTestIndexViewFilter filter = new HisTestIndexViewFilter();
                filter.IDs = testIndexIds;

                var rs = new BackendAdapter(_print507_param).Get<List<V_HIS_TEST_INDEX>>(
                    "api/HisTestIndex/GetView", ApiConsumers.MosConsumer, filter, _print507_param);
                if (rs != null && rs.Count > 0)
                {
                    _print507_TestIndexs.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
