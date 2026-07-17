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
using System.IO;
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

        // Nút "Xuất excel KSK": chọn nơi lưu (UI thread) rồi chạy fetch+build+ghi file trên LUỒNG NỀN để KHÔNG treo UI.
        private void ProcessExcell(List<V_HIS_TREATMENT_4> lstData)
        {
            try
            {
                if (lstData == null || lstData.Count == 0) return;
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "Excel file|*.xlsx|All file|*.*";
                if (saveFile.ShowDialog() != DialogResult.OK) return;
                string filePath = saveFile.FileName;

                WaitingManager.Show();
                System.Threading.Tasks.Task.Factory.StartNew(
                    () => BuildAndSaveKskExcel(lstData, filePath),
                    System.Threading.CancellationToken.None,
                    System.Threading.Tasks.TaskCreationOptions.LongRunning,
                    System.Threading.Tasks.TaskScheduler.Default)
                    .ContinueWith(t =>
                    {
                        WaitingManager.Hide();
                        if (t.Exception != null)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(t.Exception);
                            DevExpress.XtraEditors.XtraMessageBox.Show("Có lỗi khi xuất Excel kết quả khám sức khỏe. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(
                                "Xuất Excel kết quả khám sức khỏe thành công (" + lstData.Count + " bản ghi)." + Environment.NewLine + "File: " + filePath,
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    },
                    System.Threading.CancellationToken.None,
                    System.Threading.Tasks.TaskContinuationOptions.None,
                    System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Fetch dữ liệu + dựng workbook + lưu file. CHẠY TRÊN LUỒNG NỀN — TUYỆT ĐỐI không gọi control UI ở đây.
        private void BuildAndSaveKskExcel(List<V_HIS_TREATMENT_4> lstData, string filePath)
        {
            try
            {
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
                    if (ListSereServ.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL).ToList() != null && ListSereServ.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL).ToList().Count > 0)
                    {
                        ListSSTein = GetSereServTeinToExcel(lstData.Select(o => o.ID).ToList(), ListSereServ.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN ||o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL).Select(o => o.ID).ToList());
                    }
                }
                // Dựng ExcellDataADO (phần gọi API theo TỪNG bệnh nhân) chạy SONG SONG cho mượt — thay vì tuần tự N+1.
                // Nâng giới hạn kết nối HTTP (mặc định .NET chỉ 2/host) để các luồng không bị xếp hàng.
                if (System.Net.ServicePointManager.DefaultConnectionLimit < 24)
                    System.Net.ServicePointManager.DefaultConnectionLimit = 24;
                var adoArray = new ADO.ExcellDataADO[lstData.Count];
                // Cache "Tên Đoàn" theo contract ID dùng chung giữa các luồng — tránh gọi API trùng cho cùng 1 hợp đồng.
                var workPlaceCache = new System.Collections.Concurrent.ConcurrentDictionary<long, string>();
                System.Threading.Tasks.Parallel.For(0, lstData.Count,
                    new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = 5 },
                    idx =>
                    {
                        try { adoArray[idx] = new ADO.ExcellDataADO(lstData[idx], workPlaceCache); }
                        catch (Exception exAdo) { Inventec.Common.Logging.LogSystem.Warn(exAdo); }
                    });
                ListADO = adoArray.Where(o => o != null).ToList();

                // Gom sẵn theo khóa để tra O(1) — tránh quét lại toàn bộ list mỗi bệnh nhân/dịch vụ (O(n^2) -> O(n)).
                var sereByTreatment = (ListSereServ != null) ? ListSereServ.ToLookup(o => o.TDL_TREATMENT_ID) : null;
                var concludeBySereServ = new Dictionary<long, string>();
                if (ListSSExt != null)
                    foreach (var sse in ListSSExt)
                        if (!concludeBySereServ.ContainsKey(sse.SERE_SERV_ID)) concludeBySereServ[sse.SERE_SERV_ID] = sse.CONCLUDE;
                var teinBySereServ = (ListSSTein != null) ? ListSSTein.ToLookup(o => o.SERE_SERV_ID) : null;

                // Dựng cột động dịch vụ/XN (thuần in-memory — không gọi API).
                foreach (var item in lstData)
                {
                    #region
                    if (sereByTreatment != null)
                    {
                        var sereOfTreatment = sereByTreatment[item.ID];
                        var seenService = new HashSet<string>();
                        var seenXN = new HashSet<string>();
                        #region Khác XN + Khám
                        var CheckListSereServ = sereOfTreatment.Where(o => o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
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
                                if (!seenService.Add(itemSereServ.TDL_SERVICE_NAME)) continue;
                                ADO.TempExcelDataADO adoTemp = new ADO.TempExcelDataADO();
                                adoTemp.ID_TREATMENT = item.ID;
                                adoTemp.TDL_SERVICE_NAME = itemSereServ.TDL_SERVICE_NAME;
                                string concludeVal;
                                if (concludeBySereServ.TryGetValue(itemSereServ.ID, out concludeVal))
                                    adoTemp.CONCLUDE = concludeVal;
                                if (string.IsNullOrEmpty(adoTemp.CONCLUDE) && teinBySereServ != null)
                                {
                                    var CheckListSSTein = teinBySereServ[itemSereServ.ID].ToList();
                                    if (CheckListSSTein.Count == 1)
                                        adoTemp.VALUE = CheckListSSTein[0].VALUE;
                                    else if (CheckListSSTein.Count > 1)
                                        adoTemp.VALUE = string.Join("; ", CheckListSSTein.Select(t => t.TEST_INDEX_NAME + ": " + t.VALUE));
                                }
                                ListTemp.Add(adoTemp);
                            }
                        }
                        #endregion
                        #region XN
                        var CheckListSereServXN = sereOfTreatment.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).OrderByDescending(o => o.TDL_INTRUCTION_TIME).ToList();
                        if (CheckListSereServXN != null && CheckListSereServXN.Count > 0)
                        {
                            foreach (var itemSereServ in CheckListSereServXN)
                            {
                                if (!seenXN.Add(itemSereServ.TDL_SERVICE_NAME)) continue;
                                ADO.TempExcelDataADO adoTemp = new ADO.TempExcelDataADO();
                                adoTemp.ID_TREATMENT = item.ID;
                                adoTemp.TDL_SERVICE_NAME = itemSereServ.TDL_SERVICE_NAME;
                                if (teinBySereServ != null)
                                {
                                    var CheckListSSTein = teinBySereServ[itemSereServ.ID].ToList();
                                    if (CheckListSSTein.Count == 1)
                                        adoTemp.VALUE = CheckListSSTein[0].VALUE;
                                    else if (CheckListSSTein.Count > 1)
                                        adoTemp.VALUE = string.Join("; ", CheckListSSTein.Select(t => t.TEST_INDEX_NAME + ": " + t.VALUE));
                                }
                                ListTempXN.Add(adoTemp);
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                // Danh sách cột động (dịch vụ khác XN, và XN) — sắp theo tên dịch vụ.
                if (ListTemp != null && ListTemp.Count > 0)
                {
                    ListTemp = ListTemp.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    lstHeaderColumns = ListTemp.Select(o => o.TDL_SERVICE_NAME).Distinct().ToList();
                }
                if (ListTempXN != null && ListTempXN.Count > 0)
                {
                    ListTempXN = ListTempXN.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    lstHeaderColumnsXN = ListTempXN.Select(o => o.TDL_SERVICE_NAME).Distinct().ToList();
                }

                // Tra nhanh O(1) giá trị cột động theo khóa (mã đợt điều trị | tên dịch vụ) — thay CustomUnboundColumnData O(n^2).
                Dictionary<string, string> dictService = new Dictionary<string, string>();
                foreach (var t in ListTemp)
                {
                    string key = t.ID_TREATMENT + "|" + t.TDL_SERVICE_NAME;
                    if (!dictService.ContainsKey(key))
                        dictService[key] = !string.IsNullOrEmpty(t.CONCLUDE) ? t.CONCLUDE : t.VALUE;
                }
                Dictionary<string, string> dictXN = new Dictionary<string, string>();
                foreach (var t in ListTempXN)
                {
                    string key = t.ID_TREATMENT + "|" + t.TDL_SERVICE_NAME;
                    if (!dictXN.ContainsKey(key))
                        dictXN[key] = t.VALUE;
                }

                // Xuất trực tiếp ra .xlsx bằng Aspose.Cells (không qua grid): nhẹ, không magic number cột, không CustomUnboundColumnData.
                SetLicenseForAsposeCell();
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[0];
                Aspose.Cells.Cells cells = sheet.Cells;

                List<KskExcelColumn> fixedColumns = GetKskExcelFixedColumns();

                Aspose.Cells.Style headerStyle = workbook.CreateStyle();
                headerStyle.Font.IsBold = true;

                // Header (dòng 0): cột cố định + cột dịch vụ động + cột XN động.
                int colIndex = 0;
                foreach (var fc in fixedColumns)
                {
                    Aspose.Cells.Cell hc = cells[0, colIndex++];
                    hc.PutValue(fc.Caption);
                    hc.SetStyle(headerStyle);
                }
                foreach (var h in lstHeaderColumns)
                {
                    Aspose.Cells.Cell hc = cells[0, colIndex++];
                    hc.PutValue(h);
                    hc.SetStyle(headerStyle);
                }
                foreach (var h in lstHeaderColumnsXN)
                {
                    Aspose.Cells.Cell hc = cells[0, colIndex++];
                    hc.PutValue(h);
                    hc.SetStyle(headerStyle);
                }

                // Dữ liệu (từ dòng 1).
                for (int i = 0; i < ListADO.Count; i++)
                {
                    var bn = ListADO[i];
                    int rowIndex = i + 1;
                    colIndex = 0;
                    foreach (var fc in fixedColumns)
                        PutCell(cells[rowIndex, colIndex++], fc.Value(bn, i));
                    foreach (var h in lstHeaderColumns)
                    {
                        string v;
                        if (dictService.TryGetValue(bn.ID + "|" + h, out v))
                            PutCell(cells[rowIndex, colIndex], v);
                        colIndex++;
                    }
                    foreach (var h in lstHeaderColumnsXN)
                    {
                        string v;
                        if (dictXN.TryGetValue(bn.ID + "|" + h, out v))
                            PutCell(cells[rowIndex, colIndex], v);
                        colIndex++;
                    }
                }

                workbook.Save(filePath, Aspose.Cells.SaveFormat.Xlsx);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                throw; // Ném lại để ContinueWith trên UI thread báo lỗi cho người dùng.
            }
        }

        /// <summary>Định nghĩa 1 cột cố định của báo cáo Excel KSK: tiêu đề + hàm lấy giá trị theo dòng (o = bản ghi, i = chỉ số dòng).</summary>
        private class KskExcelColumn
        {
            public string Caption { get; private set; }
            public Func<ADO.ExcellDataADO, int, object> Value { get; private set; }
            public KskExcelColumn(string caption, Func<ADO.ExcellDataADO, int, object> value)
            {
                this.Caption = caption;
                this.Value = value;
            }
        }

        /// <summary>Danh sách 45 cột cố định (nguồn sự thật duy nhất về thứ tự + tiêu đề + dữ liệu). Cột động (dịch vụ/XN) thêm sau danh sách này.</summary>
        private List<KskExcelColumn> GetKskExcelFixedColumns()
        {
            return new List<KskExcelColumn>
            {
                new KskExcelColumn("STT", (o, i) => i + 1),
                new KskExcelColumn("Mã bệnh nhân", (o, i) => o.TDL_PATIENT_CODE),
                new KskExcelColumn("Họ và tên", (o, i) => o.TDL_PATIENT_NAME),
                new KskExcelColumn("Năm sinh nam", (o, i) => o.TDL_PATIENT_DOB_MEN),
                new KskExcelColumn("Năm sinh nữ", (o, i) => o.TDL_PATIENT_DOB_WOM),
                new KskExcelColumn("Tên chức vụ", (o, i) => o.TDL_PATIENT_POSITION_NAME),
                new KskExcelColumn("Tên Đoàn", (o, i) => o.WORK_PLACE_NAME),
                new KskExcelColumn("Số nhà", (o, i) => o.TDL_PATIENT_ADDRESS),
                new KskExcelColumn("Số CMND", (o, i) => o.TDL_PATIENT_CMND_NUMBER),
                new KskExcelColumn("SĐT", (o, i) => o.PHONE),
                new KskExcelColumn("Chiều cao", (o, i) => o.HEIGHT),
                new KskExcelColumn("Cân nặng", (o, i) => o.WEIGHT),
                new KskExcelColumn("BMI", (o, i) => o.VIR_BMI),
                new KskExcelColumn("Mạch", (o, i) => o.PULSE),
                new KskExcelColumn("Huyết áp", (o, i) => o.BLOOD_PRESSURE_MAX),
                new KskExcelColumn("Tuần hoàn", (o, i) => o.EXAM_CIRCULATION),
                new KskExcelColumn("Hô hấp", (o, i) => o.EXAM_RESPIRATORY),
                new KskExcelColumn("Tiêu hóa", (o, i) => o.EXAM_DIGESTION),
                new KskExcelColumn("Nội tiết", (o, i) => o.EXAM_OEND),
                new KskExcelColumn("Cơ xương khớp", (o, i) => o.EXAM_MUSCLE_BONE),
                new KskExcelColumn("Thần kinh", (o, i) => o.EXAM_NEUROLOGICAL),
                new KskExcelColumn("Tâm thần", (o, i) => o.EXAM_MENTAL),
                new KskExcelColumn("Da liễu", (o, i) => o.EXAM_DERMATOLOGY),
                new KskExcelColumn("Thận tiết niệu", (o, i) => o.EXAM_KIDNEY_UROLOGY),
                new KskExcelColumn("Ngoại khoa", (o, i) => o.EXAM_SURGERY),
                new KskExcelColumn("Sản", (o, i) => o.EXAM_OBSTETRIC),
                new KskExcelColumn("Tên ICD sản", (o, i) => o.OBSTETRIC_ICD_NAME),
                new KskExcelColumn("Mắt", (o, i) => o.EXAM_EYE),
                new KskExcelColumn("Tai mũi họng", (o, i) => o.EXAM_ENT),
                new KskExcelColumn("Răng hàm mặt", (o, i) => o.EXAM_STOMATOLOGY),
                new KskExcelColumn("Nhận xét CTM", (o, i) => o.NOTE_BLOOD),
                new KskExcelColumn("Nhận xét sinh hóa", (o, i) => o.NOTE_BIOCHEMICAL),
                new KskExcelColumn("Siêu âm tiền liệt tuyến", (o, i) => o.NOTE_PROSTASE),
                new KskExcelColumn("Siêu âm ổ bụng tổng quát", (o, i) => o.NOTE_SUPERSONIC),
                new KskExcelColumn("XQ tổng quát", (o, i) => o.NOTE_XRAY),
                new KskExcelColumn("Phân loại sức khỏe", (o, i) => o.HEIGH_RANK_NAME),
                new KskExcelColumn("Bệnh tật khác", (o, i) => o.DISEASES),
                new KskExcelColumn("Hướng giải quyết", (o, i) => o.TREATMENT_INSTRUCTION),
                new KskExcelColumn("Kết luận khám (ICD)", (o, i) => o.CONCLUSION_ICD_CODE),
                new KskExcelColumn("Kết luận chung tên (ICD)", (o, i) => o.CONCLUSION_ICD_NAME),
                new KskExcelColumn("Kết luận khám", (o, i) => o.EXAM_CONCLUSION),
                new KskExcelColumn("Kết luận chung", (o, i) => o.CONCLUSION),
                new KskExcelColumn("Nhiệt độ", (o, i) => o.TEMPERATURE),
                new KskExcelColumn("Nhịp thở", (o, i) => o.BREATH_RATE),
                new KskExcelColumn("Mã KCB", (o, i) => o.TREATMENT_CODE),
            };
        }

        /// <summary>Ghi giá trị vào 1 ô Aspose theo đúng kiểu (chuỗi/số) — bỏ qua null/chuỗi rỗng.</summary>
        private static void PutCell(Aspose.Cells.Cell cell, object value)
        {
            if (value == null) return;
            if (value is string)
            {
                string s = (string)value;
                if (!string.IsNullOrEmpty(s)) cell.PutValue(s);
            }
            else if (value is decimal) cell.PutValue(System.Convert.ToDouble((decimal)value));
            else if (value is double) cell.PutValue((double)value);
            else if (value is int) cell.PutValue((int)value);
            else if (value is long) cell.PutValue(System.Convert.ToDouble((long)value));
            else cell.PutValue(value.ToString());
        }

        /// <summary>Đặt license Aspose.Cells trước khi tạo Workbook (tránh watermark bản eval). Dùng chung key với chức năng ExportXml QĐ130.</summary>
        private void SetLicenseForAsposeCell()
        {
            try
            {
                if (!string.IsNullOrEmpty(Aspose_Key))
                {
                    using (var stream = new MemoryStream(Convert.FromBase64String(Aspose_Key)))
                    {
                        var license = new Aspose.Cells.License();
                        license.SetLicense(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private readonly string Aspose_Key =
            "PExpY2Vuc2U+DQogIDxEYXRhPg0KICAgIDxMaWNlbnNlZFRvPkFzcG9zZSBTY290bGFuZCB" +
            "UZWFtPC9MaWNlbnNlZFRvPg0KICAgIDxFbWFpbFRvPmJpbGx5Lmx1bmRpZUBhc3Bvc2UuY2" +
            "9tPC9FbWFpbFRvPg0KICAgIDxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlV" +
            "HlwZT4NCiAgICA8TGljZW5zZU5vdGU+TGltaXRlZCB0byAxIGRldmVsb3BlciwgdW5saW1p" +
            "dGVkIHBoeXNpY2FsIGxvY2F0aW9uczwvTGljZW5zZU5vdGU+DQogICAgPE9yZGVySUQ+MTQ" +
            "wNDA4MDUyMzI0PC9PcmRlcklEPg0KICAgIDxVc2VySUQ+OTQyMzY8L1VzZXJJRD4NCiAgIC" +
            "A8T0VNPlRoaXMgaXMgYSByZWRpc3RyaWJ1dGFibGUgbGljZW5zZTwvT0VNPg0KICAgIDxQc" +
            "m9kdWN0cz4NCiAgICAgIDxQcm9kdWN0PkFzcG9zZS5Ub3RhbCBmb3IgLk5FVDwvUHJvZHVj" +
            "dD4NCiAgICA8L1Byb2R1Y3RzPg0KICAgIDxFZGl0aW9uVHlwZT5FbnRlcnByaXNlPC9FZGl" +
            "0aW9uVHlwZT4NCiAgICA8U2VyaWFsTnVtYmVyPjlhNTk1NDdjLTQxZjAtNDI4Yi1iYTcyLT" +
            "djNDM2OGYxNTFkNzwvU2VyaWFsTnVtYmVyPg0KICAgIDxTdWJzY3JpcHRpb25FeHBpcnk+M" +
            "jAxNTEyMzE8L1N1YnNjcmlwdGlvbkV4cGlyeT4NCiAgICA8TGljZW5zZVZlcnNpb24+My4w" +
            "PC9MaWNlbnNlVmVyc2lvbj4NCiAgICA8TGljZW5zZUluc3RydWN0aW9ucz5odHRwOi8vd3d" +
            "3LmFzcG9zZS5jb20vY29ycG9yYXRlL3B1cmNoYXNlL2xpY2Vuc2UtaW5zdHJ1Y3Rpb25zLm" +
            "FzcHg8L0xpY2Vuc2VJbnN0cnVjdGlvbnM+DQogIDwvRGF0YT4NCiAgPFNpZ25hdHVyZT5GT" +
            "zNQSHNibGdEdDhGNTlzTVQxbDFhbXlpOXFrMlY2RThkUWtJUDdMZFRKU3hEaWJORUZ1MXpP" +
            "aW5RYnFGZkt2L3J1dHR2Y3hvUk9rYzF0VWUwRHRPNmNQMVpmNkowVmVtZ1NZOGkvTFpFQ1R" +
            "Hc3pScUpWUVJaME1vVm5CaHVQQUprNWVsaTdmaFZjRjhoV2QzRTRYUTNMemZtSkN1YWoyTk" +
            "V0ZVJpNUhyZmc9PC9TaWduYXR1cmU+DQo8L0xpY2Vuc2U+";

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

        /// <summary>
        /// Hàm này lấy dữ liệu mở rộng dịch vụ theo danh sách ID, chia nhỏ thành từng lô 100 phần tử để gọi API nhiều lần, tổng hợp kết quả và trả về. Nếu có lỗi thì trả về null.
        /// </summary>
        /// <param name="sereServId"></param>
        /// <returns></returns>
        private List<HIS_SERE_SERV_EXT> GetSereServExtToExcel(List<long> sereServId)
        {
            List<HIS_SERE_SERV_EXT> rs = new List<HIS_SERE_SERV_EXT>();
            try
            {
                if (sereServId != null && sereServId.Count > 0)
                {
                    int skip = 0;
                    while (skip < sereServId.Count)
                    {
                        var batchIds = sereServId.Skip(skip).Take(100).ToList();
                        skip += 100;
                        CommonParam param = new CommonParam();
                        HisSereServExtFilter filter = new HisSereServExtFilter();
                        filter.SERE_SERV_IDs = batchIds;
                        var batchRs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, param);
                        if (batchRs != null && batchRs.Count > 0)
                        {
                            rs.AddRange(batchRs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        /// <summary>
        /// Hàm này lấy dữ liệu mở rộng dịch vụ theo danh sách ID, chia nhỏ thành từng lô 100 phần tử để gọi API nhiều lần, tổng hợp kết quả và trả về. Nếu có lỗi thì trả về null.
        /// </summary>
        /// <param name="lstTreatmentId"></param>
        /// <param name="lstSSid"></param>
        /// <returns></returns>
        private List<V_HIS_SERE_SERV_TEIN> GetSereServTeinToExcel(List<long> lstTreatmentId, List<long> lstSSid)
        {
            List<V_HIS_SERE_SERV_TEIN> rs = new List<V_HIS_SERE_SERV_TEIN>();
            try
            {
                if (lstSSid != null && lstSSid.Count > 0)
                {
                    int skip = 0;
                    while (skip < lstSSid.Count)
                    {
                        var batchIds = lstSSid.Skip(skip).Take(100).ToList();
                        skip += 100;
                        CommonParam param = new CommonParam();
                        HisSereServTeinViewFilter filter = new HisSereServTeinViewFilter();
                        filter.SERE_SERV_IDs = batchIds;
                        filter.TDL_TREATMENT_IDs = lstTreatmentId;
                        var batchRs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, filter, param);
                        if (batchRs != null && batchRs.Count > 0)
                        {
                            rs.AddRange(batchRs);
                        }
                    }
                }
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
