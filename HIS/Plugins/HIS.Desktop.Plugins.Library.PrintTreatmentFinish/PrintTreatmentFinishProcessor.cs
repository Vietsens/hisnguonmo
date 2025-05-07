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
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt;
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

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish
{
    public class PrintTreatmentFinishProcessor
    {
        private static HIS_TREATMENT_EXT TreatmentExt { get; set; }
        private static HIS_TREATMENT HisTreatment { get; set; }
        private static HIS_MEDI_RECORD HisMediRecord { get; set; }
        private static HIS_PATIENT HisPatient { get; set; }
        private static V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter { get; set; }
        private long treatmentId { get; set; }
        private long patientId { get; set; }
        private static V_HIS_PATIENT VHisPatient { get; set; }

        private static HIS_SERVICE_REQ HisServiceReq { get; set; }

        private bool printNow;
        HIS_BRANCH CurrentBranch;
        private long? roomId { get; set; }
        private MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType = null;

        public PrintTreatmentFinishProcessor(HIS_TREATMENT _His_Treatment, long? roomId)
        {
            HisTreatment = _His_Treatment;
            HisPatient = null;
            VHisPatientTypeAlter = null;
            this.patientId = -1;
            this.treatmentId = -1;
        }

        public PrintTreatmentFinishProcessor(HIS_TREATMENT _His_Treatment, long? roomId, MPS.ProcessorBase.PrintConfig.PreviewType _PreviewType)
        {
            HisTreatment = _His_Treatment;
            HisPatient = null;
            VHisPatientTypeAlter = null;
            this.patientId = -1;
            this.treatmentId = -1;
            this.PreviewType = _PreviewType;
        }

        public PrintTreatmentFinishProcessor(HIS_TREATMENT _His_Treatment, HIS_BRANCH currentBranch, long? roomId)
        {
            HisTreatment = _His_Treatment;
            HisPatient = null;
            VHisPatientTypeAlter = null;
            this.patientId = -1;
            this.treatmentId = -1;
            this.CurrentBranch = currentBranch;
        }

        public PrintTreatmentFinishProcessor(long _Treatment_Id, long? roomId)
        {
            this.treatmentId = _Treatment_Id;
            this.patientId = -1;
            HisTreatment = null;
            HisPatient = null;
            VHisPatientTypeAlter = null;
            this.roomId = roomId;
        }

        public PrintTreatmentFinishProcessor(long _Treatment_Id, long _Patient_Id, long? roomId)
            : this(_Treatment_Id, roomId)
        {
            this.patientId = _Patient_Id;
        }

        public PrintTreatmentFinishProcessor(HIS_TREATMENT _His_Treatment, HIS_PATIENT _His_Patient, long? roomId)
            : this(_His_Treatment, roomId)
        {
            HisPatient = _His_Patient;
        }

        public PrintTreatmentFinishProcessor(HIS_TREATMENT _His_Treatment, HIS_PATIENT _His_Patient, V_HIS_PATIENT_TYPE_ALTER _V_His_Patient_Type_Alter, long? roomId)
            : this(_His_Treatment, _His_Patient, roomId)
        {
            VHisPatientTypeAlter = _V_His_Patient_Type_Alter;
        }

        public PrintTreatmentFinishProcessor(HIS_TREATMENT _His_Treatment, HIS_SERVICE_REQ _His_Service_Req, long? roomId)
        {
            HisTreatment = _His_Treatment;
            HisServiceReq = _His_Service_Req;
            HisPatient = null;
            VHisPatientTypeAlter = null;
            this.patientId = -1;
            this.treatmentId = -1;
        }

        public PrintTreatmentFinishProcessor(HIS_TREATMENT _His_Treatment, HIS_SERVICE_REQ _His_Service_Req, long? roomId, MPS.ProcessorBase.PrintConfig.PreviewType? _PreviewType)
        {
            HisTreatment = _His_Treatment;
            HisServiceReq = _His_Service_Req;
            HisPatient = null;
            VHisPatientTypeAlter = null;   
            this.patientId = -1;
            this.treatmentId = -1;
        }

        /// <summary>
        /// Sử dụng cấu hình để in ngay ("HIS.Desktop.Plugins.Library.PrintTreatmentFinish.Mps")
        /// </summary>
        public void Print()
        {
            try
            {
                Print(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.mps));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// in ngay
        /// </summary>
        /// <param name="PrintTypeCode">mã in</param>
        public void Print(string PrintTypeCode)
        {
            try
            {
                Print(PrintTypeCode, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void PrintMps000010(string PrintTypeCode)
        {
            try
            {
                PrintMps000010(PrintTypeCode, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrintTypeCode">Mã in (8,10,11,268)</param>
        /// <param name="PrintNow">true/false</param>
        public void Print(string PrintTypeCode, bool PrintNow)
        {
            try
            {
                this.printNow = PrintNow;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);

                switch (PrintTypeCode)
                {
                    case MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    case MPS.Processor.Mps000389.PDO.Mps000389PDO.printTypeCode:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000389.PDO.Mps000389PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    case MPS.Processor.Mps000399.PDO.Mps000399PDO.printTypeCode:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000399.PDO.Mps000399PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    case MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    case MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    case MPS.Processor.Mps000382.PDO.Mps000382PDO.printTypeCode:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000382.PDO.Mps000382PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    case MPS.Processor.Mps000268.PDO.Mps000268PDO.printTypeCode:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000268.PDO.Mps000268PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    case "Mps000269":
                        PrintTreatmentEndTypeExtProcessor printTreatmentEndTypeExtProcessor = new PrintTreatmentEndTypeExtProcessor(this.treatmentId > 0 ? this.treatmentId : (HisTreatment != null ? HisTreatment.ID : 0), CreateMenu.TYPE.DYNAMIC, this.roomId);
                        printTreatmentEndTypeExtProcessor.Print(HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.Base.PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM, PrintTreatmentEndTypeExtProcessor.OPTION.PRINT);
                        //richEditorMain.RunPrintTemplate("Mps000269", DelegateRunPrinter);
                        break;
                    case PrintEnum.IN_BANT__MPS000174:
                        richEditorMain.RunPrintTemplate(PrintEnum.IN_BANT__MPS000174, DelegateRunPrinter);
                        break;
                    case MPS.Processor.Mps000478.PDO.Mps000478PDO.printTypeCode:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000478.PDO.Mps000478PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void PrintMps000010(string PrintTypeCode, bool PrintNow)
        {
            try
            {
                this.printNow = PrintNow;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);

                switch (PrintTypeCode)
                {
                    case MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode, DelegateRunPrinterMps000010);
                        break;

                    default:
                        break;
                }
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
                if (ProcessDataBeforePrint())
                {
                    switch (printCode)
                    {
                        case MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode:
                            new PrintMps000008(printCode, fileName, ref result, VHisPatient, HisTreatment, VHisPatientTypeAlter, printNow, roomId);
                            break;
                        case MPS.Processor.Mps000399.PDO.Mps000399PDO.printTypeCode:
                            new PrintMps000399(printCode, fileName, ref result, VHisPatient, HisTreatment, printNow, roomId);
                            break;
                        case MPS.Processor.Mps000389.PDO.Mps000389PDO.printTypeCode:
                            new PrintMps000389(printCode, fileName, ref result, VHisPatient, HisTreatment, printNow, roomId);
                            break;
                        case MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode:
                            new PrintMps000010(printCode, fileName, ref result, VHisPatient, HisTreatment, VHisPatientTypeAlter, PreviewType, roomId, HisMediRecord, HisServiceReq);
                            break;
                        case MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode:
                            new PrintMps000011(printCode, fileName, ref result, VHisPatient, HisTreatment, VHisPatientTypeAlter, printNow, roomId, HisServiceReq, TreatmentExt);
                            break;
                        case MPS.Processor.Mps000268.PDO.Mps000268PDO.printTypeCode:
                            new PrintMps000268(printCode, fileName, ref result, VHisPatient, HisTreatment, this.CurrentBranch, printNow, roomId);
                            break;
                        //case "Mps000269":
                        //    new PrintMps000269(printCode, fileName, ref result, HisTreatment, VHisPatientTypeAlter, _HisSereServ, printNow, roomId);
                        //    break;
                        case PrintEnum.IN_BANT__MPS000174:
                            new PrintMps000174(printCode, fileName, ref result, HisTreatment, VHisPatient, VHisPatientTypeAlter, printNow, roomId);
                            break;
                        case MPS.Processor.Mps000382.PDO.Mps000382PDO.printTypeCode:
                            new PrintMps000382(printCode, fileName, ref result, VHisPatient, HisTreatment, VHisPatientTypeAlter, printNow, roomId);
                            break;
                        case MPS.Processor.Mps000478.PDO.Mps000478PDO.printTypeCode:
                            new PrintMps000478(printCode, fileName, ref result, printNow, HisTreatment.ID, roomId);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool DelegateRunPrinterMps000010(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                if (ProcessDataBeforePrint())
                {
                    switch (printCode)
                    {
                        case MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode:
                            new PrintMps000010(printCode, fileName, ref result, VHisPatient, HisTreatment, VHisPatientTypeAlter, PreviewType, roomId, HisMediRecord, HisServiceReq);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        private void LoadServiceReq(long id)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.IS_ACTIVE = 1;
                filter.TREATMENT_ID = id;
                var dt = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                if (dt != null && dt.Count > 0)
                    HisServiceReq = dt.FirstOrDefault(o => o.EXAM_END_TYPE == 3);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessDataBeforePrint()
        {
            bool result = false;
            if (HisConfigs.Get<string>(Config.KEY_IsPrintPrescriptionNoThread) == "1")
            {
                GetTreatment();
                GetPatient();
                GetPatientTypeAlter();
                result = true;
            }
            else
            {
                Thread treatment = new Thread(GetTreatment);
                Thread patient = new Thread(GetPatient);
                Thread patientTypeAlter = new Thread(GetPatientTypeAlter);
                try
                {
                    if (this.treatmentId <= 0 && (HisTreatment == null || (HisTreatment != null && HisTreatment.ID <= 0)))
                    {
                        return false;
                    }
                    treatment.Start();
                    patient.Start();
                    patientTypeAlter.Start();

                    treatment.Join();
                    patient.Join();
                    patientTypeAlter.Join();
                    result = true;
                }
                catch (Exception ex)
                {
                    treatment.Abort();
                    patient.Abort();
                    patientTypeAlter.Abort();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    result = false;
                }
            }
            return result;
        }

        private void GetTreatment()
        {
            try
            {
                if (HisTreatment == null && treatmentId > 0)
                {
                    HisTreatment = GetCurrentHistreatment(treatmentId);
                }
                else if (HisTreatment != null && HisTreatment.ID > 0)
                {
                    HisTreatment = GetCurrentHistreatment(HisTreatment.ID);
                }
                if (HisServiceReq == null)
                    LoadServiceReq(HisTreatment.ID);
                HisTreatmentExtFilter filter = new HisTreatmentExtFilter();
                filter.TREATMENT_ID = HisTreatment.ID;
                TreatmentExt = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT_EXT>>("api/HisTreatmentExt/Get", ApiConsumer.ApiConsumers.MosConsumer, filter,
                    HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null).FirstOrDefault();

                EmrDataStore.treatmentCode = HisTreatment.TREATMENT_CODE;
                HisMediRecord = GetMediRecord(HisTreatment.MEDI_RECORD_ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                HisTreatment = new HIS_TREATMENT();
            }
        }

        private HIS_TREATMENT GetCurrentHistreatment(long treatmentId)
        {
            HIS_TREATMENT result = new HIS_TREATMENT();
            try
            {
                if (treatmentId > 0)
                {
                    MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                    filter.ID = treatmentId;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new HIS_TREATMENT();
            }
            return result;
        }

        private HIS_MEDI_RECORD GetMediRecord(long? mediRecordId)
        {
            HIS_MEDI_RECORD result = new HIS_MEDI_RECORD();
            try
            {
                if (mediRecordId.HasValue && mediRecordId > 0)
                {
                    MOS.Filter.HisMediRecordFilter filter = new MOS.Filter.HisMediRecordFilter();
                    filter.ID = mediRecordId.Value;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_RECORD>>("api/HisMediRecord/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new HIS_MEDI_RECORD();
            }
            return result;
        }

        private void GetPatient()
        {
            try
            {
                if (HisPatient == null || HisPatient.ID <= 0)
                {
                    if (this.patientId > 0)
                    {
                        VHisPatient = GetPatientById(patientId);
                    }
                    else if (HisTreatment != null && HisTreatment.ID > 0)
                    {
                        if (HisTreatment.PATIENT_ID <= 0)
                        {
                            var treatment = GetCurrentHistreatment(HisTreatment.ID);
                            if (treatment != null)
                            {
                                VHisPatient = GetPatientById(treatment.PATIENT_ID);
                            }
                        }
                        else
                        {
                            VHisPatient = GetPatientById(HisTreatment.PATIENT_ID);
                        }
                    }
                    else
                    {
                        VHisPatient = new V_HIS_PATIENT();
                    }
                }
                else if (HisPatient.ID > 0)
                {
                    VHisPatient = GetPatientById(HisPatient.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                VHisPatient = new V_HIS_PATIENT();
            }
        }

        private V_HIS_PATIENT GetPatientById(long p)
        {
            V_HIS_PATIENT result = new V_HIS_PATIENT();
            try
            {
                if (p > 0)
                {
                    MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                    filter.ID = p;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new V_HIS_PATIENT();
            }
            return result;
        }

        private void GetPatientTypeAlter()
        {
            try
            {
                if (VHisPatientTypeAlter == null || VHisPatientTypeAlter.ID <= 0)
                {
                    if (this.treatmentId > 0)
                    {
                        VHisPatientTypeAlter = GetCurrentPatientTypeAlter(this.treatmentId);
                    }
                    else if (HisTreatment != null && HisTreatment.ID > 0)
                    {
                        VHisPatientTypeAlter = GetCurrentPatientTypeAlter(HisTreatment.ID);
                    }
                    else
                    {
                        VHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_PATIENT_TYPE_ALTER GetCurrentPatientTypeAlter(long p)
        {
            V_HIS_PATIENT_TYPE_ALTER result = new V_HIS_PATIENT_TYPE_ALTER();
            try
            {
                if (p > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientTypeAlterViewAppliedFilter filter = new MOS.Filter.HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = p;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetApplied", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new V_HIS_PATIENT_TYPE_ALTER();
            }
            return result;
        }
    }
}
