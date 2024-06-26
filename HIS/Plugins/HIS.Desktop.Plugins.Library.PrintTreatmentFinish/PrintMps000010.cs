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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish
{
    class PrintMps000010
    {
        public PrintMps000010(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.V_HIS_PATIENT HisPatient, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter, bool _printNow, long? roomId, HIS_MEDI_RECORD mediRecord, HIS_SERVICE_REQ ServiceReq)
        {
            try
            {
                if (HisTreatment == null || HisTreatment.ID <= 0)
                {
                    result = false;
                    return;
                }

                MPS.Processor.Mps000010.PDO.PatientADO PatientADO = new MPS.Processor.Mps000010.PDO.PatientADO(HisPatient);

                MPS.Processor.Mps000010.PDO.Mps000010ADO ado = new MPS.Processor.Mps000010.PDO.Mps000010ADO();
                if (HisTreatment.DEATH_CAUSE_ID != null)
                {
                    var deathCause = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_CAUSE_ID.Value);
                    if (deathCause != null)
                    {
                        ado.DEATH_CAUSE_CODE = deathCause.DEATH_CAUSE_CODE;
                        ado.DEATH_CAUSE_NAME = deathCause.DEATH_CAUSE_NAME;
                    }
                }
                if (HisTreatment.DEATH_WITHIN_ID != null)
                {
                    var deathWithin = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_WITHIN_ID.Value);
                    if (deathWithin != null)
                    {
                        ado.DEATH_WITHIN_CODE = deathWithin.DEATH_WITHIN_CODE;
                        ado.DEATH_WITHIN_NAME = deathWithin.DEATH_WITHIN_NAME;
                    }
                }
                if (HisTreatment.TRAN_PATI_FORM_ID.HasValue)
                {
                    var tranPatiForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == HisTreatment.TRAN_PATI_FORM_ID.Value);
                    if (tranPatiForm != null)
                    {
                        ado.TRAN_PATI_FORM_CODE = tranPatiForm.TRAN_PATI_FORM_CODE;
                        ado.TRAN_PATI_FORM_NAME = tranPatiForm.TRAN_PATI_FORM_NAME;
                    }
                }
                if (HisTreatment.TREATMENT_RESULT_ID.HasValue)
                {
                    var treatmentResult = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == HisTreatment.TREATMENT_RESULT_ID.Value);
                    if (treatmentResult != null)
                    {
                        ado.TREATMENT_RESULT_CODE = treatmentResult.TREATMENT_RESULT_CODE;
                        ado.TREATMENT_RESULT_NAME = treatmentResult.TREATMENT_RESULT_NAME;
                    }
                }
                if (HisTreatment.TRAN_PATI_REASON_ID.HasValue)
                {
                    var tranPatiReason = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == HisTreatment.TRAN_PATI_REASON_ID.Value);
                    if (tranPatiReason != null)
                    {
                        ado.TRAN_PATI_REASON_CODE = tranPatiReason.TRAN_PATI_REASON_CODE;
                        ado.TRAN_PATI_REASON_NAME = tranPatiReason.TRAN_PATI_REASON_NAME;
                    }
                }
                if (mediRecord != null)
                {
                    ado.MEDI_RECORD_STORE_CODE = mediRecord.STORE_CODE;
                }
                if (HisTreatment.END_ROOM_ID.HasValue)
                {
                    var endRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.END_ROOM_ID.Value);
                    if (endRoom != null)
                    {
                        ado.END_DEPARTMENT_CODE = endRoom.DEPARTMENT_CODE;
                        ado.END_DEPARTMENT_NAME = endRoom.DEPARTMENT_NAME;
                        ado.END_ROOM_CODE = endRoom.ROOM_CODE;
                        ado.END_ROOM_NAME = endRoom.ROOM_NAME;
                    }
                }
                if (HisTreatment.FEE_LOCK_ROOM_ID.HasValue)
                {
                    var feelockRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.FEE_LOCK_ROOM_ID.Value);
                    if (feelockRoom != null)
                    {
                        ado.FEE_LOCK_DEPARTMENT_CODE = feelockRoom.DEPARTMENT_CODE;
                        ado.FEE_LOCK_DEPARTMENT_NAME = feelockRoom.DEPARTMENT_NAME;
                        ado.FEE_LOCK_ROOM_CODE = feelockRoom.ROOM_CODE;
                        ado.FEE_LOCK_ROOM_NAME = feelockRoom.ROOM_NAME;
                    }
                }
                if (HisTreatment.IN_ROOM_ID.HasValue)
                {
                    var inRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.IN_ROOM_ID.Value);
                    if (inRoom != null)
                    {
                        ado.IN_DEPARTMENT_CODE = inRoom.DEPARTMENT_CODE;
                        ado.IN_DEPARTMENT_NAME = inRoom.DEPARTMENT_NAME;
                        ado.IN_ROOM_CODE = inRoom.ROOM_CODE;
                        ado.IN_ROOM_NAME = inRoom.ROOM_NAME;
                    }
                }

                if (ServiceReq != null && ServiceReq.APPOINTMENT_TIME != null)
                {
                    if (ServiceReq.APPOINTMENT_EXAM_ROOM_ID != null)
                    {
                        var _Rooms = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == ServiceReq.APPOINTMENT_EXAM_ROOM_ID);
                        var _Services = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(p => p.ID == ServiceReq.APPOINTMENT_EXAM_SERVICE_ID);

                        ado.APPOINTMENT_EXAM_ROOM_IDS = ServiceReq.APPOINTMENT_EXAM_ROOM_ID.ToString();
                        if (_Rooms != null)
                        {
                            ado.APPOINTMENT_EXAM_ROOM_NAMES = _Rooms.ROOM_NAME;
                            ado.APPOINTMENT_EXAM_ROOM_CODE_NAMES = string.Format("{0} - {1}", _Rooms.ROOM_CODE, _Rooms.ROOM_NAME);
                        }
                        if (_Services != null)
                        {
                            ado.APPOINTMENT_SERVICE_NAMES = _Services.SERVICE_NAME;
                            ado.APPOINTMENT_SERVICE_CODES = _Services.SERVICE_CODE;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(HisTreatment.APPOINTMENT_EXAM_ROOM_IDS))
                    {
                        var _RoomExamADOs = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(p => p.IS_EXAM == 1).ToList();
                        string[] ids = HisTreatment.APPOINTMENT_EXAM_ROOM_IDS.Split(',');
                        List<string> _roomName = new List<string>();
                        List<string> _roomCodeName = new List<string>();
                        foreach (var item in _RoomExamADOs)
                        {
                            var dataCheck = ids.FirstOrDefault(p => p.Trim() == item.ROOM_ID.ToString().Trim());
                            if (!string.IsNullOrEmpty(dataCheck))
                            {
                                _roomName.Add(item.EXECUTE_ROOM_NAME);
                                _roomCodeName.Add(item.EXECUTE_ROOM_CODE + " - " + item.EXECUTE_ROOM_NAME);
                            }
                        }
                        ado.APPOINTMENT_EXAM_ROOM_IDS = HisTreatment.APPOINTMENT_EXAM_ROOM_IDS;
                        if (_roomName != null && _roomName.Count > 0)
                            ado.APPOINTMENT_EXAM_ROOM_NAMES = string.Join(",", _roomName);
                        if (_roomCodeName != null && _roomCodeName.Count > 0)
                            ado.APPOINTMENT_EXAM_ROOM_CODE_NAMES = string.Join(",", _roomCodeName);
                    }
                }

                MOS.Filter.HisAppointmentServViewFilter _ssFilter = new MOS.Filter.HisAppointmentServViewFilter();
                _ssFilter.TREATMENT_ID = HisTreatment.ID;
                var dataApps = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_APPOINTMENT_SERV>>("api/HisAppointmentServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, _ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);


                var appointmentPeriods = BackendDataWorker.Get<HIS_APPOINTMENT_PERIOD>();

                MPS.Processor.Mps000010.PDO.Mps000010PDO mps000010RDO = new MPS.Processor.Mps000010.PDO.Mps000010PDO(
                   PatientADO,
                   VHisPatientTypeAlter,
                   HisTreatment,
                   ado,
                   dataApps,
                   appointmentPeriods,
                   ServiceReq
                   );
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mps000010RDO), mps000010RDO));
                result = Print.RunPrint(printTypeCode, fileName, mps000010RDO, null, result, _printNow, roomId);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                //if (_printNow)
                //{
                //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000010RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                //}
                //else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000010RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                //}
                //else
                //{
                //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000010RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName));
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public PrintMps000010(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.V_HIS_PATIENT HisPatient, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter, MPS.ProcessorBase.PrintConfig.PreviewType? _previewType, long? roomId, HIS_MEDI_RECORD mediRecord, HIS_SERVICE_REQ ServiceReq)
        {
            try
            {
                if (HisTreatment == null || HisTreatment.ID <= 0)
                {
                    result = false;
                    return;
                }

                MPS.Processor.Mps000010.PDO.PatientADO PatientADO = new MPS.Processor.Mps000010.PDO.PatientADO(HisPatient);

                MPS.Processor.Mps000010.PDO.Mps000010ADO ado = new MPS.Processor.Mps000010.PDO.Mps000010ADO();
                if (HisTreatment.DEATH_CAUSE_ID != null)
                {
                    var deathCause = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_CAUSE_ID.Value);
                    if (deathCause != null)
                    {
                        ado.DEATH_CAUSE_CODE = deathCause.DEATH_CAUSE_CODE;
                        ado.DEATH_CAUSE_NAME = deathCause.DEATH_CAUSE_NAME;
                    }
                }
                if (HisTreatment.DEATH_WITHIN_ID != null)
                {
                    var deathWithin = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_WITHIN_ID.Value);
                    if (deathWithin != null)
                    {
                        ado.DEATH_WITHIN_CODE = deathWithin.DEATH_WITHIN_CODE;
                        ado.DEATH_WITHIN_NAME = deathWithin.DEATH_WITHIN_NAME;
                    }
                }
                if (HisTreatment.TRAN_PATI_FORM_ID.HasValue)
                {
                    var tranPatiForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == HisTreatment.TRAN_PATI_FORM_ID.Value);
                    if (tranPatiForm != null)
                    {
                        ado.TRAN_PATI_FORM_CODE = tranPatiForm.TRAN_PATI_FORM_CODE;
                        ado.TRAN_PATI_FORM_NAME = tranPatiForm.TRAN_PATI_FORM_NAME;
                    }
                }
                if (HisTreatment.TREATMENT_RESULT_ID.HasValue)
                {
                    var treatmentResult = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == HisTreatment.TREATMENT_RESULT_ID.Value);
                    if (treatmentResult != null)
                    {
                        ado.TREATMENT_RESULT_CODE = treatmentResult.TREATMENT_RESULT_CODE;
                        ado.TREATMENT_RESULT_NAME = treatmentResult.TREATMENT_RESULT_NAME;
                    }
                }
                if (HisTreatment.TRAN_PATI_REASON_ID.HasValue)
                {
                    var tranPatiReason = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == HisTreatment.TRAN_PATI_REASON_ID.Value);
                    if (tranPatiReason != null)
                    {
                        ado.TRAN_PATI_REASON_CODE = tranPatiReason.TRAN_PATI_REASON_CODE;
                        ado.TRAN_PATI_REASON_NAME = tranPatiReason.TRAN_PATI_REASON_NAME;
                    }
                }
                if (mediRecord != null)
                {
                    ado.MEDI_RECORD_STORE_CODE = mediRecord.STORE_CODE;
                }
                if (HisTreatment.END_ROOM_ID.HasValue)
                {
                    var endRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.END_ROOM_ID.Value);
                    if (endRoom != null)
                    {
                        ado.END_DEPARTMENT_CODE = endRoom.DEPARTMENT_CODE;
                        ado.END_DEPARTMENT_NAME = endRoom.DEPARTMENT_NAME;
                        ado.END_ROOM_CODE = endRoom.ROOM_CODE;
                        ado.END_ROOM_NAME = endRoom.ROOM_NAME;
                    }
                }
                if (HisTreatment.FEE_LOCK_ROOM_ID.HasValue)
                {
                    var feelockRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.FEE_LOCK_ROOM_ID.Value);
                    if (feelockRoom != null)
                    {
                        ado.FEE_LOCK_DEPARTMENT_CODE = feelockRoom.DEPARTMENT_CODE;
                        ado.FEE_LOCK_DEPARTMENT_NAME = feelockRoom.DEPARTMENT_NAME;
                        ado.FEE_LOCK_ROOM_CODE = feelockRoom.ROOM_CODE;
                        ado.FEE_LOCK_ROOM_NAME = feelockRoom.ROOM_NAME;
                    }
                }
                if (HisTreatment.IN_ROOM_ID.HasValue)
                {
                    var inRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.IN_ROOM_ID.Value);
                    if (inRoom != null)
                    {
                        ado.IN_DEPARTMENT_CODE = inRoom.DEPARTMENT_CODE;
                        ado.IN_DEPARTMENT_NAME = inRoom.DEPARTMENT_NAME;
                        ado.IN_ROOM_CODE = inRoom.ROOM_CODE;
                        ado.IN_ROOM_NAME = inRoom.ROOM_NAME;
                    }
                }
                if (ServiceReq != null && ServiceReq.APPOINTMENT_TIME != null)
                {
                    if (ServiceReq.APPOINTMENT_EXAM_ROOM_ID != null)
                    {
                        var _Rooms = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == ServiceReq.APPOINTMENT_EXAM_ROOM_ID);
                        var _Services = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(p => p.ID == ServiceReq.APPOINTMENT_EXAM_SERVICE_ID);

                        ado.APPOINTMENT_EXAM_ROOM_IDS = ServiceReq.APPOINTMENT_EXAM_ROOM_ID.ToString();
                        if (_Rooms != null)
                        {
                            ado.APPOINTMENT_EXAM_ROOM_NAMES = _Rooms.ROOM_NAME;
                            ado.APPOINTMENT_EXAM_ROOM_CODE_NAMES = string.Format("{0} - {1}", _Rooms.ROOM_CODE, _Rooms.ROOM_NAME);
                        }
                        if (_Services != null)
                        {
                            ado.APPOINTMENT_SERVICE_NAMES = _Services.SERVICE_NAME;
                            ado.APPOINTMENT_SERVICE_CODES = _Services.SERVICE_CODE;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(HisTreatment.APPOINTMENT_EXAM_ROOM_IDS))
                    {
                        var _RoomExamADOs = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(p => p.IS_EXAM == 1).ToList();
                        string[] ids = HisTreatment.APPOINTMENT_EXAM_ROOM_IDS.Split(',');
                        List<string> _roomName = new List<string>();
                        List<string> _roomCodeName = new List<string>();
                        foreach (var item in _RoomExamADOs)
                        {
                            var dataCheck = ids.FirstOrDefault(p => p.Trim() == item.ROOM_ID.ToString().Trim());
                            if (!string.IsNullOrEmpty(dataCheck))
                            {
                                _roomName.Add(item.EXECUTE_ROOM_NAME);
                                _roomCodeName.Add(item.EXECUTE_ROOM_CODE + " - " + item.EXECUTE_ROOM_NAME);
                            }
                        }
                        ado.APPOINTMENT_EXAM_ROOM_IDS = HisTreatment.APPOINTMENT_EXAM_ROOM_IDS;
                        if (_roomName != null && _roomName.Count > 0)
                            ado.APPOINTMENT_EXAM_ROOM_NAMES = string.Join(",", _roomName);
                        if (_roomCodeName != null && _roomCodeName.Count > 0)
                            ado.APPOINTMENT_EXAM_ROOM_CODE_NAMES = string.Join(",", _roomCodeName);
                    }
                }


                MOS.Filter.HisAppointmentServViewFilter _ssFilter = new MOS.Filter.HisAppointmentServViewFilter();
                _ssFilter.TREATMENT_ID = HisTreatment.ID;
                var dataApps = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_APPOINTMENT_SERV>>("api/HisAppointmentServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, _ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);


                var appointmentPeriods = BackendDataWorker.Get<HIS_APPOINTMENT_PERIOD>();

                MPS.Processor.Mps000010.PDO.Mps000010PDO mps000010RDO = new MPS.Processor.Mps000010.PDO.Mps000010PDO(
                   PatientADO,
                   VHisPatientTypeAlter,
                   HisTreatment,
                   ado,
                   dataApps,
                   appointmentPeriods,
                   ServiceReq
                   );
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mps000010RDO), mps000010RDO));
                //result = Print.RunPrint(printTypeCode, fileName, mps000010RDO, null, result, _printNow, roomId);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                Inventec.Common.Logging.LogSystem.Debug("_previewType" + _previewType);

                MPS.ProcessorBase.PrintConfig.PreviewType prType = MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog;

                if (_previewType.HasValue)
                {
                    prType = _previewType.Value;
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    prType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(HisTreatment.TREATMENT_CODE, printTypeCode, roomId);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000010RDO, prType, printerName) { EmrInputADO = inputADO });

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
