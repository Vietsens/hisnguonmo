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
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TreatmentList.Base;
//using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.DAL;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
//using MPS.Old.Config;
using SCN.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class UCTreatmentList : UserControlBase
    {
        CommonParam _KSK_param = new CommonParam();
        List<V_HIS_TREATMENT_4> _KSK_Treatments { get; set; }
        List<V_HIS_SERVICE_REQ> _KSK_ServiceReqs { get; set; }
        List<HIS_SERVICE> _KSK_Services { get; set; }
        List<V_HIS_SERE_SERV> _KSK_SereServs { get; set; }
        List<HIS_SERE_SERV_EXT> _KSK_SereServExts { get; set; }
        List<V_HIS_BED_LOG> _KSK_BedLogs { get; set; }
        List<V_HIS_PATIENT_TYPE_ALTER> _KSK_PatientTypeAlters { get; set; }
        List<V_HIS_DHST> _KSK_Dhsts { get; set; }
        List<V_HIS_SERE_SERV_TEIN> _KSK_SereServTeins { get; set; }
        List<V_HIS_TEST_INDEX> _KSK_TestIndexs { get; set; }
        List<HIS_PATIENT> _KSK_Patient { get; set; }
        List<HIS_KSK_GENERAL> _KSK_General { get; set; }
        List<HIS_KSK_DRIVER> _KSK_Driver { get; set; }
        private void ProcessPrintf(List<V_HIS_TREATMENT_4> _KSK_Treatments_Check)
        {
            try
            {
                WaitingManager.Show();
                _KSK_param = new CommonParam();
                _KSK_Treatments = new List<V_HIS_TREATMENT_4>();
                _KSK_ServiceReqs = new List<V_HIS_SERVICE_REQ>();
                _KSK_SereServs = new List<V_HIS_SERE_SERV>();
                _KSK_SereServExts = new List<HIS_SERE_SERV_EXT>();
                _KSK_BedLogs = new List<V_HIS_BED_LOG>();
                _KSK_PatientTypeAlters = new List<V_HIS_PATIENT_TYPE_ALTER>();
                _KSK_Dhsts = new List<V_HIS_DHST>();
                _KSK_SereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                _KSK_Patient = new List<HIS_PATIENT>();
                _KSK_Driver = new List<HIS_KSK_DRIVER>();

                this._KSK_Treatments = _KSK_Treatments_Check;

                int start = 0;
                int count = this._KSK_Treatments.Count;
                while (count > 0)
                {
                    int limit = (count <= 100) ? count : 100;
                    var listSub = this._KSK_Treatments.Skip(start).Take(limit).ToList();
                    List<long> _treatmentIds = new List<long>();
                    _treatmentIds = listSub.Select(p => p.ID).Distinct().ToList();

                    CreateThreadByTreatmentIds(_treatmentIds);

                    start += 100;
                    count -= 100;
                }

                List<long> patientIds = _KSK_Treatments.Select(s => s.PATIENT_ID).Distinct().ToList();
                int skip = 0;
                while (patientIds.Count - skip > 0)
                {
                    var listIds = patientIds.Skip(skip).Take(100).ToList();
                    skip += 100;
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.IDs = listIds;
                    var rs = new BackendAdapter(_KSK_param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, _KSK_param);
                    if (rs != null && rs.Count > 0)
                    {
                        _KSK_Patient.AddRange(rs);
                    }
                }
                WaitingManager.Hide();

                //TODO
                KSK__Print();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintKQCLSKSK(List<V_HIS_TREATMENT_4> _KSK_Treatments_Check)
        {
            try
            {
                WaitingManager.Show();
                _KSK_param = new CommonParam();
                _KSK_Treatments = new List<V_HIS_TREATMENT_4>();
                _KSK_ServiceReqs = new List<V_HIS_SERVICE_REQ>();
                _KSK_SereServs = new List<V_HIS_SERE_SERV>();
                _KSK_Services = new List<HIS_SERVICE>();
                _KSK_SereServExts = new List<HIS_SERE_SERV_EXT>();
                _KSK_SereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                _KSK_TestIndexs = new List<V_HIS_TEST_INDEX>();
                _KSK_General = new List<HIS_KSK_GENERAL>();

                this._KSK_Treatments = _KSK_Treatments_Check;

                int start = 0;
                int count = this._KSK_Treatments.Count;
                while (count > 0)
                {
                    int limit = (count <= 100) ? count : 100;
                    var listSub = this._KSK_Treatments.Skip(start).Take(limit).ToList();
                    List<long> _treatmentIds = new List<long>();
                    _treatmentIds = listSub.Select(p => p.ID).Distinct().ToList();

                    CreateThreadByTreatmentIdsKsk(_treatmentIds);

                    start += 100;
                    count -= 100;
                }
                WaitingManager.Hide();

                //TODO
                KSK__Print__481();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void KSK__Print__481()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate("Mps000481", KSK__DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadByTreatmentIdsKsk(List<long> _treatmentIds)
        {
            try
            {
                GetServiceReq_KSK(_treatmentIds);
                GetKskGeneral_KSK(_KSK_ServiceReqs.Select(o => o.ID).ToList());
                GetSereServ__KSK(_treatmentIds);
                GetSereServExt__KSK(_treatmentIds);
                GetSereServTein__KSK(_treatmentIds);
                GetService__KSK(_KSK_SereServs.Select(o => o.SERVICE_ID).ToList());
                GetTestIndex__KSK(_KSK_SereServTeins.Where(o => o.TEST_INDEX_ID != null).Select(o => (long)o.TEST_INDEX_ID).ToList());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTestIndex__KSK(List<long> lstId)
        {
            try
            {
                if (lstId != null)
                {
                    HisTestIndexViewFilter filter = new HisTestIndexViewFilter();
                    filter.IDs = lstId;

                    var rs = new BackendAdapter(_KSK_param).Get<List<V_HIS_TEST_INDEX>>("api/HisTestIndex/GetView", ApiConsumers.MosConsumer, filter, _KSK_param);
                    if (rs != null && rs.Count > 0)
                    {
                        _KSK_TestIndexs.AddRange(rs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetService__KSK(List<long> lstId)
        {
            try
            {
                if (lstId != null)
                {
                    HisServiceFilter filter = new HisServiceFilter();
                    filter.IDs = lstId;

                    var rs = new BackendAdapter(_KSK_param).Get<List<HIS_SERVICE>>("api/HisService/Get", ApiConsumers.MosConsumer, filter, _KSK_param);
                    if (rs != null && rs.Count > 0)
                    {
                        _KSK_Services.AddRange(rs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServTein__KSK(List<long> _treatmentIds)
        {
            try
            {
                if (_treatmentIds != null)
                {
                    HisSereServTeinViewFilter filter = new HisSereServTeinViewFilter();
                    filter.TDL_TREATMENT_IDs = _treatmentIds;

                    var rs = new BackendAdapter(_KSK_param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, filter, _KSK_param);
                    if (rs != null && rs.Count > 0)
                    {
                        _KSK_SereServTeins.AddRange(rs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void KSK__Print()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate("Mps000315", KSK__DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool KSK__DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (printTypeCode == "Mps000315")
                {
                    Mps000315(printTypeCode, fileName, ref result);
                }
                else if (printTypeCode == "Mps000481")
                {
                    Mps000481(printTypeCode, fileName, ref result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000481(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                var KskRank = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>();
                var KskPosition = BackendDataWorker.Get<HIS_POSITION>();
                MPS.Processor.Mps000481.PDO.Mps000481PDO mps000481RDO = new MPS.Processor.Mps000481.PDO.Mps000481PDO(
                _KSK_Treatments,
                _KSK_ServiceReqs,
                _KSK_General,
                _KSK_SereServs,
                _KSK_Services,
                _KSK_SereServExts,
                _KSK_TestIndexs,
                _KSK_SereServTeins,
                KskRank,
                KskPosition
                );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (_KSK_Treatments != null && _KSK_Treatments.Count == 1)
                {
                    var Treatments = _KSK_Treatments.FirstOrDefault();

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatments != null ? Treatments.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000481RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000481RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                }
                else
                {
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000481RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");// { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000481RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Mps000315(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                var KskRank = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>();

                MPS.Processor.Mps000315.PDO.Mps000315PDO mps000315RDO = new MPS.Processor.Mps000315.PDO.Mps000315PDO(
                _KSK_Treatments,
                _KSK_ServiceReqs,
                _KSK_SereServs,
                _KSK_SereServExts,
                _KSK_BedLogs,
                _KSK_PatientTypeAlters,
                _KSK_Dhsts,
                _KSK_SereServTeins,
                KskRank,
                _KSK_Patient,
                _KSK_Driver
                );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (_KSK_Treatments != null && _KSK_Treatments.Count == 1)
                {
                    var Treatments = _KSK_Treatments.FirstOrDefault();

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatments != null ? Treatments.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000315RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000315RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                }
                else
                {
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000315RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");// { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000315RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadByTreatmentIds(List<long> _treatmentIds)
        {
            Thread t1 = new Thread(new ParameterizedThreadStart(Thread1));
            Thread t2 = new Thread(new ParameterizedThreadStart(Thread2));
            Thread t3 = new Thread(new ParameterizedThreadStart(Thread3));
            Thread t4 = new Thread(new ParameterizedThreadStart(Thread4));
            Thread t5 = new Thread(new ParameterizedThreadStart(Thread5));
            Thread t6 = new Thread(new ParameterizedThreadStart(Thread6));
            Thread t7 = new Thread(new ParameterizedThreadStart(Thread7));
            Thread t8 = new Thread(new ParameterizedThreadStart(Thread8));
            try
            {
                t1.Start(_treatmentIds);
                t2.Start(_treatmentIds);
                t3.Start(_treatmentIds);
                t4.Start(_treatmentIds);
                t5.Start(_treatmentIds);
                t6.Start(_treatmentIds);
                t7.Start(_treatmentIds);
                t8.Start(_treatmentIds);
                t1.Join();
                t2.Join();
                t3.Join();
                t4.Join();
                t5.Join();
                t6.Join();
                t7.Join();
                t8.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                t1.Abort();
                t2.Abort();
                t3.Abort();
                t4.Abort();
                t5.Abort();
                t6.Abort();
                t7.Abort();
                t8.Abort();
            }
        }
        private void Thread8(object data)
        {
            try
            {
                GetDriver_KSK((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDriver_KSK(List<long> data)
        {
            try
            {
                MOS.Filter.HisKskDriverFilter filter = new HisKskDriverFilter();
                filter.TDL_TREATMENT_IDs = data;

                var rs = new BackendAdapter(_KSK_param).Get<List<HIS_KSK_DRIVER>>("api/HisKskDriver/Get", ApiConsumers.MosConsumer, filter, _KSK_param);
                if (rs != null && rs.Count > 0)
                {
                    _KSK_Driver.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void Thread2(object data)
        {
            try
            {
                GetSereServ__KSK((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread3(object data)
        {
            try
            {
                GetSereServExt__KSK((List<long>)data);
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
                GetBedLog__KSK((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread5(object data)
        {
            try
            {
                GetPatientTypeAlter__KSK((List<long>)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread6(object obj)
        {
            try
            {
                if (obj != null)
                {
                    HisSereServTeinViewFilter filter = new HisSereServTeinViewFilter();
                    filter.TDL_TREATMENT_IDs = obj as List<long>;

                    var rs = new BackendAdapter(_KSK_param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, filter, _KSK_param);
                    if (rs != null && rs.Count > 0)
                    {
                        _KSK_SereServTeins.AddRange(rs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread7(object obj)
        {
            try
            {
                if (obj != null)
                {
                    HisDhstViewFilter filter = new HisDhstViewFilter();
                    filter.TREATMENT_IDs = obj as List<long>;

                    var rs = new BackendAdapter(_KSK_param).Get<List<V_HIS_DHST>>("api/HisDhst/GetView", ApiConsumers.MosConsumer, filter, _KSK_param);
                    if (rs != null && rs.Count > 0)
                    {
                        var group = rs.OrderByDescending(o => o.EXECUTE_TIME ?? 0).ThenByDescending(o => o.MODIFY_TIME).GroupBy(o => o.TREATMENT_ID).Select(s => s.First()).ToList();
                        _KSK_Dhsts.AddRange(group);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceReq_KSK(List<long> treatmentIds)
        {
            try
            {
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.TREATMENT_IDs = treatmentIds;
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;

                var rs = new BackendAdapter(_KSK_param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, _KSK_param);
                if (rs != null && rs.Count > 0)
                {
                    _KSK_ServiceReqs.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetKskGeneral_KSK(List<long> serviceReqIds)
        {
            try
            {
                HisKskGeneralFilter filter = new HisKskGeneralFilter();
                filter.SERVICE_REQ_IDs = serviceReqIds;

                var rs = new BackendAdapter(_KSK_param).Get<List<HIS_KSK_GENERAL>>("api/HisKskGeneral/Get", ApiConsumers.MosConsumer, filter, _KSK_param);
                if (rs != null && rs.Count > 0)
                {
                    _KSK_General.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetSereServ__KSK(List<long> treatmentIds)
        {
            try
            {
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_IDs = treatmentIds;

                var rs = new BackendAdapter(_KSK_param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, _KSK_param);
                if (rs != null && rs.Count > 0)
                {
                    _KSK_SereServs.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetSereServExt__KSK(List<long> treatmentIds)
        {
            try
            {
                HisSereServExtFilter filter = new HisSereServExtFilter();
                filter.TDL_TREATMENT_IDs = treatmentIds;

                var rs = new BackendAdapter(_KSK_param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, _KSK_param);
                if (rs != null && rs.Count > 0)
                {
                    _KSK_SereServExts.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetBedLog__KSK(List<long> treatmentIds)
        {
            try
            {
                HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_IDs = treatmentIds;

                var rs = new BackendAdapter(_KSK_param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, _KSK_param);
                if (rs != null && rs.Count > 0)
                {
                    _KSK_BedLogs.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetPatientTypeAlter__KSK(List<long> treatmentIds)
        {
            try
            {
                MOS.Filter.HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                filter.TREATMENT_IDs = treatmentIds;

                var rs = new BackendAdapter(_KSK_param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, _KSK_param);
                if (rs != null && rs.Count > 0)
                {
                    _KSK_PatientTypeAlters.AddRange(rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPrintfSoKSK(V_HIS_TREATMENT_4 _KSK_Treatments_Check)
        {
            try
            {
                WaitingManager.Show();
                _KSK_param = new CommonParam();
                _KSK_Treatments = new List<V_HIS_TREATMENT_4>();
                _KSK_Patient = new List<HIS_PATIENT>();

                this._KSK_Treatments.Add(_KSK_Treatments_Check);


                List<long> patientIds = _KSK_Treatments.Select(s => s.PATIENT_ID).Distinct().ToList();
                int skip = 0;
                while (patientIds.Count - skip > 0)
                {
                    var listIds = patientIds.Skip(skip).Take(100).ToList();
                    skip += 100;
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.IDs = listIds;
                    var rs = new BackendAdapter(_KSK_param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, _KSK_param);
                    if (rs != null && rs.Count > 0)
                    {
                        _KSK_Patient.AddRange(rs);
                    }
                }
                WaitingManager.Hide();

                //TODO
                SoKSK__Print();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SoKSK__Print()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate("Mps000450", SoKSK__DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool SoKSK__DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                Mps000450(printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000450(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                MPS.Processor.Mps000450.PDO.Mps000450PDO mps000450 = new MPS.Processor.Mps000450.PDO.Mps000450PDO(
                _KSK_Treatments.FirstOrDefault(),
                _KSK_Patient.FirstOrDefault()
                );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (_KSK_Treatments != null && _KSK_Treatments.Count == 1)
                {
                    var Treatments = _KSK_Treatments.FirstOrDefault();

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatments != null ? Treatments.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000450, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000450, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                }
                else
                {
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000450, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");// { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000450, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessExcell(List<V_HIS_TREATMENT_4> lstData)
        {
            try
            {
                WaitingManager.Show();
                ListTemp = new List<ADO.TempExcelDataADO>();
                ListTempXN = new List<ADO.TempExcelDataADO>();
                lstHeaderColumns = new List<string>();
                lstHeaderColumnsXN = new List<string>();
                List<HIS_SERE_SERV> ListSereServ = GetSereServToExcel(lstData.Select(o => o.ID).ToList()).OrderByDescending(o => o.TDL_INTRUCTION_TIME).ToList();
                List<HIS_SERE_SERV_EXT> ListSSExt = new List<HIS_SERE_SERV_EXT>();
                List<V_HIS_SERE_SERV_TEIN> ListSSTein = new List<V_HIS_SERE_SERV_TEIN>();
                List<ADO.ExcellDataADO> ListADO = new List<ADO.ExcellDataADO>();
                if (ListSereServ != null && ListSereServ.Count > 0)
                {
                    ListSSExt = GetSereServExtToExcel(ListSereServ.Select(o => o.ID).ToList());
                    if (ListSereServ.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList() != null && ListSereServ.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Count > 0)
                    {
                        ListSSTein = GetSereServTeinToExcel(lstData.Select(o => o.ID).ToList(), ListSereServ.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).Select(o => o.ID).ToList());
                    }
                }
                foreach (var item in lstData)
                {
                    HIS.Desktop.Plugins.TreatmentList.ADO.ExcellDataADO ado = new ADO.ExcellDataADO(item);
                    ListADO.Add(ado);
                    #region
                    if (ListSereServ != null && ListSereServ.Count > 0)
                    {
                        #region Khác XN + Khám
                        var CheckListSereServ = ListSereServ.Where(o => o.TDL_TREATMENT_ID == item.ID && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                            && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                            && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                            && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                             && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                                   && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                                   && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN
                            ).OrderByDescending(o => o.TDL_INTRUCTION_TIME).ToList();
                        if (CheckListSereServ != null && CheckListSereServ.Count > 0)
                        {
                            foreach (var itemSereServ in CheckListSereServ)
                            {
                                if (ListTemp != null && ListTemp.Count > 0)
                                {
                                    if (ListTemp.Where(o => o.TDL_SERVICE_NAME == itemSereServ.TDL_SERVICE_NAME && o.ID_TREATMENT == item.ID).ToList() != null && ListTemp.Where(o => o.TDL_SERVICE_NAME == itemSereServ.TDL_SERVICE_NAME && o.ID_TREATMENT == item.ID).ToList().Count > 0)
                                    {
                                        continue;
                                    }
                                }
                                ADO.TempExcelDataADO adoTemp = new ADO.TempExcelDataADO();
                                adoTemp.ID_TREATMENT = item.ID;
                                adoTemp.TDL_SERVICE_NAME = itemSereServ.TDL_SERVICE_NAME;
                                if (ListSSExt != null && ListSSExt.Count > 0)
                                {
                                    if (ListSSExt.Where(o => o.SERE_SERV_ID == itemSereServ.ID).ToList() != null && ListSSExt.Where(o => o.SERE_SERV_ID == itemSereServ.ID).ToList().Count > 0)
                                    {
                                        adoTemp.CONCLUDE = ListSSExt.Where(o => o.SERE_SERV_ID == itemSereServ.ID).FirstOrDefault().CONCLUDE;
                                    }
                                }
                                ListTemp.Add(adoTemp);
                            }
                        }
                        #endregion
                        #region XN
                        var CheckListSereServXN = ListSereServ.Where(o => o.TDL_TREATMENT_ID == item.ID && o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).OrderByDescending(o => o.TDL_INTRUCTION_TIME).ToList();
                        if (CheckListSereServXN != null && CheckListSereServXN.Count > 0)
                        {
                            foreach (var itemSereServ in CheckListSereServXN)
                            {
                                if (ListTempXN != null && ListTempXN.Count > 0)
                                {
                                    if (ListTempXN.Where(o => o.TDL_SERVICE_NAME == itemSereServ.TDL_SERVICE_NAME && o.ID_TREATMENT == item.ID).ToList() != null && ListTemp.Where(o => o.TDL_SERVICE_NAME == itemSereServ.TDL_SERVICE_NAME && o.ID_TREATMENT == item.ID).ToList().Count > 0)
                                    {
                                        continue;
                                    }
                                }
                                ADO.TempExcelDataADO adoTemp = new ADO.TempExcelDataADO();
                                adoTemp.ID_TREATMENT = item.ID;
                                adoTemp.TDL_SERVICE_NAME = itemSereServ.TDL_SERVICE_NAME;
                                if (ListSSTein != null && ListSSTein.Count > 0)
                                {
                                    var CheckListSSTein = ListSSTein.Where(o => o.SERE_SERV_ID == itemSereServ.ID && o.TDL_TREATMENT_ID == item.ID).ToList();
                                    if (CheckListSSTein != null && CheckListSSTein.Count > 0)
                                    {
                                        if (CheckListSSTein.Count == 1)
                                        {
                                            adoTemp.VALUE = CheckListSSTein.FirstOrDefault().VALUE;
                                        }
                                        else
                                        {
                                            List<string> lst = new List<string>();
                                            foreach (var itemSSTein in CheckListSSTein)
                                            {
                                                lst.Add(itemSSTein.TEST_INDEX_NAME + ": " + itemSSTein.VALUE);
                                            }
                                            adoTemp.VALUE = string.Join("; ", lst);
                                        }

                                    }
                                }
                                ListTempXN.Add(adoTemp);
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                if (gridView5.Columns.Count() > 39)
                {
                    int count = gridView5.Columns.Count() - 39;
                    while (true)
                    {
                        if (count == 0)
                        {
                            break;
                        }
                        else
                        {
                            gridView5.Columns.RemoveAt(39);
                            count--;
                        }
                    }
                }
                int dem = 39;
                gridControl1.BeginUpdate();
                if (ListTemp != null && ListTemp.Count > 0)
                {
                    ListTemp = ListTemp.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    lstHeaderColumns = ListTemp.Select(o => o.TDL_SERVICE_NAME).Distinct().ToList();
                    for (int i = 0; i < lstHeaderColumns.Count; i++)
                    {
                        DevExpress.XtraGrid.Columns.GridColumn gridColumnExcel = new DevExpress.XtraGrid.Columns.GridColumn();
                        gridColumnExcel.Caption = lstHeaderColumns[i];
                        gridColumnExcel.Name = "Gr" + i;
                        gridColumnExcel.FieldName = "TDL_SERVICE_NAME_" + i;
                        gridColumnExcel.VisibleIndex = dem;
                        gridColumnExcel.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                        dem++;
                        gridView5.Columns.Add(gridColumnExcel);
                    }

                }
                int demXN = 0;
                if (ListTempXN != null && ListTempXN.Count > 0)
                {
                    ListTempXN = ListTempXN.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    lstHeaderColumnsXN = ListTempXN.Select(o => o.TDL_SERVICE_NAME).Distinct().ToList();
                    for (int i = demXN; i < lstHeaderColumnsXN.Count; i++)
                    {
                        DevExpress.XtraGrid.Columns.GridColumn gridColumnExcel = new DevExpress.XtraGrid.Columns.GridColumn();
                        gridColumnExcel.Caption = lstHeaderColumnsXN[i];
                        gridColumnExcel.Name = "Gr__SERE_SERV_TEIN" + i;
                        gridColumnExcel.FieldName = "TDL_SERVICE_NAME___SERE_SERV_TEIN" + i;
                        gridColumnExcel.VisibleIndex = dem;
                        gridColumnExcel.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                        dem++;
                        gridView5.Columns.Add(gridColumnExcel);
                    }
                }
                gridControl1.DataSource = ListADO;
                layoutControlItem38.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem38.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                gridControl1.EndUpdate();
                WaitingManager.Hide();
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "Excel file|*.xlsx|All file|*.*";
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    gridView5.RefreshData();
                    gridControl1.Refresh();
                    gridControl1.ExportToXlsx(saveFile.FileName);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private List<HIS_SERE_SERV> GetSereServToExcel(List<long> treatmentId)
        {
            List<HIS_SERE_SERV> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServFilter filter = new HisSereServFilter();
                filter.TREATMENT_IDs = treatmentId;
                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }
        private List<HIS_SERE_SERV_EXT> GetSereServExtToExcel(List<long> sereServId)
        {
            List<HIS_SERE_SERV_EXT> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServExtFilter filter = new HisSereServExtFilter();
                filter.SERE_SERV_IDs = sereServId;
                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }
        private List<V_HIS_SERE_SERV_TEIN> GetSereServTeinToExcel(List<long> lstTreatmentId, List<long> lstSSid)
        {
            List<V_HIS_SERE_SERV_TEIN> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServTeinViewFilter filter = new HisSereServTeinViewFilter();
                filter.SERE_SERV_IDs = lstSSid;
                filter.TDL_TREATMENT_IDs = lstTreatmentId;
                rs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }
    }
}
