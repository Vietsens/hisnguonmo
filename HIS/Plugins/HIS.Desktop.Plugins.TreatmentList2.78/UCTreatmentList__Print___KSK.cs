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
        List<V_HIS_SERE_SERV> _KSK_SereServs { get; set; }
        List<HIS_SERE_SERV_EXT> _KSK_SereServExts { get; set; }
        List<V_HIS_BED_LOG> _KSK_BedLogs { get; set; }
        List<V_HIS_PATIENT_TYPE_ALTER> _KSK_PatientTypeAlters { get; set; }

        private void ProcessPrintf(List<V_HIS_TREATMENT_4> _KSK_Treatments_Check)
        {
            try
            {
                _KSK_param = new CommonParam();
                _KSK_Treatments = new List<V_HIS_TREATMENT_4>();
                _KSK_ServiceReqs = new List<V_HIS_SERVICE_REQ>();
                _KSK_SereServs = new List<V_HIS_SERE_SERV>();
                _KSK_SereServExts = new List<HIS_SERE_SERV_EXT>();
                _KSK_BedLogs = new List<V_HIS_BED_LOG>();
                _KSK_PatientTypeAlters = new List<V_HIS_PATIENT_TYPE_ALTER>();

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

                //TODO
                KSK__Print();


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
                Mps000315(printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000315(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();


                MPS.Processor.Mps000315.PDO.Mps000315PDO mps000315RDO = new MPS.Processor.Mps000315.PDO.Mps000315PDO(
                _KSK_Treatments,
                _KSK_ServiceReqs,
                _KSK_SereServs,
                _KSK_SereServExts,
                _KSK_BedLogs,
                _KSK_PatientTypeAlters
                );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000315RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");// { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000315RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
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

            try
            {
                t1.Start(_treatmentIds);
                t2.Start(_treatmentIds);
                t3.Start(_treatmentIds);
                t4.Start(_treatmentIds);
                t5.Start(_treatmentIds);

                t1.Join();
                t2.Join();
                t3.Join();
                t4.Join();
                t5.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                t1.Abort();
                t2.Abort();
                t3.Abort();
                t4.Abort();
                t5.Abort();
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

    }
}
