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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Common.Adapter;
using System.Threading;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        CommonParam _KSK_param = new CommonParam();
        List<V_HIS_TREATMENT_4> _KSK_Treatments { get; set; }
        List<V_HIS_SERE_SERV> _KSK_SereServss { get; set; }
        List<HIS_SERE_SERV_EXT> _KSK_SereServExts { get; set; }
        List<V_HIS_BED_LOG> _KSK_BedLogs { get; set; }
        List<V_HIS_PATIENT_TYPE_ALTER> _KSK_PatientTypeAlters { get; set; }
        List<V_HIS_DHST> _KSK_Dhsts { get; set; }
        List<V_HIS_SERE_SERV_TEIN> _KSK_SereServTeins { get; set; }
        List<HIS_PATIENT> _KSK_Patient { get; set; }
        List<V_HIS_SERVICE_REQ> _KSK_ServiceReqs { get; set; }
        List<HIS_KSK_DRIVER> _KSK_Driver { get; set; }
        private void LoadPhieuKiemTraKhamSucKhoeDinhKy(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                _KSK_Treatments = new List<V_HIS_TREATMENT_4>();
                _KSK_SereServss = new List<V_HIS_SERE_SERV>();
                _KSK_SereServExts = new List<HIS_SERE_SERV_EXT>();
                _KSK_BedLogs = new List<V_HIS_BED_LOG>();
                _KSK_PatientTypeAlters = new List<V_HIS_PATIENT_TYPE_ALTER>();
                _KSK_Dhsts = new List<V_HIS_DHST>();
                _KSK_SereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                _KSK_Patient = new List<HIS_PATIENT>();
                _KSK_ServiceReqs = new List<V_HIS_SERVICE_REQ>();
                _KSK_Driver = new List<HIS_KSK_DRIVER>();

                if (this.treatment != null)
                {

                    CreateThreadByTreatmentIds(this.treatment.ID);
                }

                MPS.Processor.Mps000315.PDO.Mps000315PDO mps000315RDO = new MPS.Processor.Mps000315.PDO.Mps000315PDO(
                _KSK_Treatments,
                _KSK_ServiceReqs,
                _KSK_SereServss,
                _KSK_SereServExts,
                _KSK_BedLogs,
                _KSK_PatientTypeAlters,
                _KSK_Dhsts,
                _KSK_SereServTeins,
                lstHealthExamRank,
                new List<HIS_PATIENT> { CurrentPatient},
                _KSK_Driver
                );
                WaitingManager.Hide();

                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (_KSK_Treatments != null && _KSK_Treatments.Count == 1)
                {
                    var Treatments = _KSK_Treatments.FirstOrDefault();

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatments != null ? Treatments.TREATMENT_CODE : ""), printTypeCode, this.moduleData != null ? this.moduleData.RoomId : 0);

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
                Inventec.Common.Logging.LogSystem.Error(ex);                
            } 
        }

        private void CreateThreadByTreatmentIds(long _treatmentId)
        {
            Thread t1 = new Thread(new ParameterizedThreadStart(Thread1));
            Thread t2 = new Thread(new ParameterizedThreadStart(Thread2));
            Thread t3 = new Thread(new ParameterizedThreadStart(Thread3));
            Thread t4 = new Thread(new ParameterizedThreadStart(Thread4));
            Thread t5 = new Thread(new ParameterizedThreadStart(Thread5));
            Thread t6 = new Thread(new ParameterizedThreadStart(Thread6));
            Thread t7 = new Thread(new ParameterizedThreadStart(Thread7));
            Thread t8 = new Thread(new ParameterizedThreadStart(Thread8));
            Thread t9 = new Thread(new ParameterizedThreadStart(Thread9));
            try
            {
                t1.Start(_treatmentId);
                t2.Start(_treatmentId);
                t3.Start(_treatmentId);
                t4.Start(_treatmentId);
                t5.Start(_treatmentId);
                t6.Start(_treatmentId);
                t7.Start(_treatmentId);
                t8.Start(_treatmentId);
                t9.Start(_treatmentId);
                t1.Join();
                t2.Join();
                t3.Join();
                t4.Join();
                t5.Join();
                t6.Join();
                t7.Join();
                t8.Join();
                t9.Join();
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
                t9.Abort();
            }
        }
        private void Thread9(object data)
        {
            try
            {
                GetDriver_KSK((long)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void GetDriver_KSK(long data)
        {
            try
            {
                MOS.Filter.HisKskDriverFilter filter = new HisKskDriverFilter();
                filter.TDL_TREATMENT_ID = data;

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
                GetSereServExt__KSK((long)data);
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
                GetSereServ__KSK((long)data);
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
                GetServiceReq__KSK((long)data);
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
                GetBedLog__KSK((long)data);
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
                GetPatientTypeAlter__KSK((long)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread6(object data)
        {
            try
            {
                GetDhst__KSK((long)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Thread7(object data)
        {
            try
            {
                GetSereServTein__KSK((long)data);
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
                GetTreatment__KSK((long)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        private void GetSereServExt__KSK(long treatmentId)
        {
            try
            {
                HisSereServExtFilter filter = new HisSereServExtFilter();
                filter.TDL_TREATMENT_ID = treatmentId;

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

        private void GetSereServ__KSK(long treatmentId)
        {
            try
            {
                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.TREATMENT_ID = treatmentId;
                _KSK_SereServss = new BackendAdapter(_KSK_param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, sereServFilter, _KSK_param).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetServiceReq__KSK(long treatmentId)
        {
            try
            {
                HisServiceReqViewFilter ServiceReqfilter = new HisServiceReqViewFilter();
                ServiceReqfilter.TREATMENT_ID = treatmentId;
                ServiceReqfilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                ServiceReqfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var ServiceReq = new BackendAdapter(_KSK_param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, ServiceReqfilter, _KSK_param);
                if (ServiceReq != null && ServiceReq.Count > 0)
                {
                    _KSK_ServiceReqs.AddRange(ServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetBedLog__KSK(long treatmentId)
        {
            try
            {
                HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_ID = treatmentId;

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

        private void GetPatientTypeAlter__KSK(long treatmentId)
        {
            try
            {
                MOS.Filter.HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                filter.TREATMENT_ID = treatmentId;

                var PatientTypeAlters = new BackendAdapter(_KSK_param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, _KSK_param);
                if (PatientTypeAlters != null && PatientTypeAlters.Count > 0)
                {
                    _KSK_PatientTypeAlters.AddRange(PatientTypeAlters);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void GetDhst__KSK(long treatmentId)
        {
            try
            {
                HisDhstViewFilter Dhstfilter = new HisDhstViewFilter();
                Dhstfilter.TREATMENT_ID = treatmentId;

                var Dhst = new BackendAdapter(_KSK_param).Get<List<V_HIS_DHST>>("api/HisDhst/GetView", ApiConsumers.MosConsumer, Dhstfilter, _KSK_param);
                if (Dhst != null && Dhst.Count > 0)
                {
                    _KSK_Dhsts.AddRange(Dhst);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void GetSereServTein__KSK(long treatmentId) 
        {
            try
            {
                HisSereServTeinViewFilter SereServTeinfilter = new HisSereServTeinViewFilter();
                SereServTeinfilter.TDL_TREATMENT_ID = treatmentId;

                var SereServTein = new BackendAdapter(_KSK_param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, SereServTeinfilter, _KSK_param);
                if (SereServTein != null && SereServTein.Count > 0)
                {
                    _KSK_SereServTeins.AddRange(SereServTein);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void GetTreatment__KSK(long treatmentId)
        {
            try
            {
                HisTreatmentView4Filter TreatmentView4filter = new HisTreatmentView4Filter();
                TreatmentView4filter.ID = treatmentId;

                var Treatment = new BackendAdapter(_KSK_param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumers.MosConsumer, TreatmentView4filter, _KSK_param);
                if (Treatment != null && Treatment.Count > 0)
                {
                    _KSK_Treatments.AddRange(Treatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
