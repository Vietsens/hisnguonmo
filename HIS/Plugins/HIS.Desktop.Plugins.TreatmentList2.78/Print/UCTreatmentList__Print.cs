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
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt;
using HIS.Desktop.Plugins.TreatmentList.Base;
using HIS.Desktop.Print;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class UCTreatmentList : UserControlBase
    {
        private void InGiayRaVien(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;

                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatment4.ID;
                HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                //Thông tin bệnh nhân
                var currentPatient = PrintGlobalStore.getPatient(treatment.ID);
                //

                //MOS.Filter.HisTreatmentOutViewFilter treatmentOutFilter = new MOS.Filter.HisTreatmentOutViewFilter();
                //treatmentOutFilter.TREATMENT_ID = treatment.ID;

                //MOS.EFMODEL.DataModels.V_HIS_TREATMENT_OUT treatmentOut = new MOS.EFMODEL.DataModels.V_HIS_TREATMENT_OUT();
                //if (treatmentOuts != null && treatmentOuts.Count > 0)
                //{

                //    treatmentOut = treatmentOuts.FirstOrDefault();
                //}

                //Thông tin bảo hiểm y tế
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment.ID, instructionTime, ref PatyAlterBhyt);

                //Lấy dữ liệu về Ra viện của Bn

                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(treatment.ID);

                long timeIn = GetTimeIn(treatment4);

                MPS.Processor.Mps000008.PDO.PatientADO PatientADO = new MPS.Processor.Mps000008.PDO.PatientADO(currentPatient);

                // lấy về các phẫu thuật viên chính
                List<V_HIS_EKIP_USER> ListEkipUser = new List<V_HIS_EKIP_USER>();

                MOS.Filter.HisSereServView5Filter sereServFilter = new MOS.Filter.HisSereServView5Filter();
                sereServFilter.TDL_TREATMENT_ID = treatment.ID;
                sereServFilter.TDL_SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT;
                var sereServs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, null);
                if (sereServs != null && sereServs.Count() > 0)
                {
                    MOS.Filter.HisEkipUserViewFilter ekipFilter = new MOS.Filter.HisEkipUserViewFilter();
                    ekipFilter.EKIP_IDs = sereServs.Where(p => p.EKIP_ID.HasValue).Select(o => o.EKIP_ID.Value).Distinct().ToList();
                    ekipFilter.IS_SURG_MAIN = true;
                    ListEkipUser = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EKIP_USER>>("api/HisEkipUser/GetView", ApiConsumer.ApiConsumers.MosConsumer, ekipFilter, null);
                }

                MPS.Processor.Mps000008.PDO.Mps000008PDO mps000008RDO = new MPS.Processor.Mps000008.PDO.Mps000008PDO(
                           PatientADO,
                           PatyAlterBhyt,
                           treatment,
                           null,
                           0,
                           ListEkipUser
                           );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (AllowPrintFinishCFG.ALLOW_PRINT_RA_VIEN == 1)
                {
                    if (treatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân chưa khóa viện phí");
                    }
                    else
                    {
                        if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000008RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000008RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                        }
                    }
                }
                else
                {
                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000008RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000008RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                    }
                    //result = MPS.Printer.Run(printTypeCode, fileName, mps000008RDO);
                }
                WaitingManager.Hide();
            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        ////bệnh nhân nhập viện:
        ////- Cấp cứu: Lấy giờ đăng ký tiếp đón
        ////- Phòng khám: Lấy giờ xử trí nhập viện
        ////bệnh nhân không nhập viện:
        ////- Cấp cứu: Lấy giờ đăng ký tiếp đón
        ////- Phòng khám: Lấy giờ khám bệnh. (Tức là lúc bác sĩ làm thao tác xử lý)
        private long GetTimeIn(V_HIS_TREATMENT_4 curentTreatment)
        {
            long result = 0;
            try
            {

                if (curentTreatment.IS_EMERGENCY == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)//cấp cứu
                {
                    result = curentTreatment.IN_TIME;
                }
                else if (!curentTreatment.CLINICAL_IN_TIME.HasValue) //khám
                {
                    HIS_SERVICE_REQ service = GetFirstServiceReq(curentTreatment.ID);
                    if (service != null && service.ID > 0)
                    {
                        result = service.START_TIME ?? 0;
                    }
                    else
                    {
                        result = curentTreatment.IN_TIME;
                    }
                }
                else
                {
                    result = curentTreatment.CLINICAL_IN_TIME.HasValue ? curentTreatment.CLINICAL_IN_TIME.Value : curentTreatment.IN_TIME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }

        private HIS_SERVICE_REQ GetFirstServiceReq(long treatmentId)
        {
            HIS_SERVICE_REQ result = new HIS_SERVICE_REQ();
            try
            {
                if (treatmentId > 0)
                {
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.TREATMENT_ID = treatmentId;
                    var lstService = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, null);
                    if (lstService != null && lstService.Count > 0)
                    {
                        lstService = lstService.OrderBy(o => o.START_TIME ?? 9999999999999999).ThenBy(o => o.CREATE_TIME).ToList();
                        result = lstService.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void InGiayHenKham(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                //Cap nhat ho so dieu tri
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatment4.ID;
                HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();

                //Thông tin bệnh nhân
                var currentPatient = PrintGlobalStore.getPatient(treatment.ID);


                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment.ID, instructionTime, ref PatyAlterBhyt);

                //Lấy dữ liệu về Ra viện của Bn
                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(treatment.ID);

                WaitingManager.Hide();

                MPS.Processor.Mps000010.PDO.PatientADO PatientADO = new MPS.Processor.Mps000010.PDO.PatientADO(currentPatient);
                MPS.Processor.Mps000010.PDO.Mps000010ADO mps000010ADO = new MPS.Processor.Mps000010.PDO.Mps000010ADO();
                if (treatment.DEATH_CAUSE_ID != null)
                {
                    var deathCause = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == treatment.DEATH_CAUSE_ID.Value);
                    if (deathCause != null)
                    {
                        mps000010ADO.DEATH_CAUSE_CODE = deathCause.DEATH_CAUSE_CODE;
                        mps000010ADO.DEATH_CAUSE_NAME = deathCause.DEATH_CAUSE_NAME;
                    }
                }
                if (treatment.DEATH_WITHIN_ID != null)
                {
                    var deathWithin = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == treatment.DEATH_WITHIN_ID.Value);
                    if (deathWithin != null)
                    {
                        mps000010ADO.DEATH_WITHIN_CODE = deathWithin.DEATH_WITHIN_CODE;
                        mps000010ADO.DEATH_WITHIN_NAME = deathWithin.DEATH_WITHIN_NAME;
                    }
                }
                if (treatment.TRAN_PATI_FORM_ID.HasValue)
                {
                    var tranPatiForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == treatment.TRAN_PATI_FORM_ID.Value);
                    if (tranPatiForm != null)
                    {
                        mps000010ADO.TRAN_PATI_FORM_CODE = tranPatiForm.TRAN_PATI_FORM_CODE;
                        mps000010ADO.TRAN_PATI_FORM_NAME = tranPatiForm.TRAN_PATI_FORM_NAME;
                    }
                }
                if (treatment.TRAN_PATI_REASON_ID.HasValue)
                {
                    var tranPatiReason = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == treatment.TRAN_PATI_REASON_ID.Value);
                    if (tranPatiReason != null)
                    {
                        mps000010ADO.TRAN_PATI_REASON_CODE = tranPatiReason.TRAN_PATI_REASON_CODE;
                        mps000010ADO.TRAN_PATI_REASON_NAME = tranPatiReason.TRAN_PATI_REASON_NAME;
                    }
                }
                if (treatment.END_ROOM_ID.HasValue)
                {
                    var endRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == treatment.END_ROOM_ID.Value);
                    if (endRoom != null)
                    {
                        mps000010ADO.END_DEPARTMENT_CODE = endRoom.DEPARTMENT_CODE;
                        mps000010ADO.END_DEPARTMENT_NAME = endRoom.DEPARTMENT_NAME;
                        mps000010ADO.END_ROOM_CODE = endRoom.ROOM_CODE;
                        mps000010ADO.END_ROOM_NAME = endRoom.ROOM_NAME;
                    }
                }
                if (treatment.FEE_LOCK_ROOM_ID.HasValue)
                {
                    var feelockRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == treatment.FEE_LOCK_ROOM_ID.Value);
                    if (feelockRoom != null)
                    {
                        mps000010ADO.FEE_LOCK_DEPARTMENT_CODE = feelockRoom.DEPARTMENT_CODE;
                        mps000010ADO.FEE_LOCK_DEPARTMENT_NAME = feelockRoom.DEPARTMENT_NAME;
                        mps000010ADO.FEE_LOCK_ROOM_CODE = feelockRoom.ROOM_CODE;
                        mps000010ADO.FEE_LOCK_ROOM_NAME = feelockRoom.ROOM_NAME;
                    }
                }
                if (treatment.IN_ROOM_ID.HasValue)
                {
                    var inRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == treatment.IN_ROOM_ID.Value);
                    if (inRoom != null)
                    {
                        mps000010ADO.IN_DEPARTMENT_CODE = inRoom.DEPARTMENT_CODE;
                        mps000010ADO.IN_DEPARTMENT_NAME = inRoom.DEPARTMENT_NAME;
                        mps000010ADO.IN_ROOM_CODE = inRoom.ROOM_CODE;
                        mps000010ADO.IN_ROOM_NAME = inRoom.ROOM_NAME;
                    }
                }

                if (!string.IsNullOrEmpty(treatment.APPOINTMENT_EXAM_ROOM_IDS))
                {
                    var _RoomExamADOs = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(p => p.IS_EXAM == 1).ToList();
                    string[] ids = treatment.APPOINTMENT_EXAM_ROOM_IDS.Split(',');
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
                    if (_roomName != null && _roomName.Count > 0)
                        mps000010ADO.APPOINTMENT_EXAM_ROOM_NAMES = string.Join(",", _roomName);
                    if (_roomCodeName != null && _roomCodeName.Count > 0)
                        mps000010ADO.APPOINTMENT_EXAM_ROOM_CODE_NAMES = string.Join(",", _roomCodeName);
                }

                MPS.Processor.Mps000010.PDO.Mps000010PDO mps000010RDO = new MPS.Processor.Mps000010.PDO.Mps000010PDO(
                   PatientADO,
                   PatyAlterBhyt,
                   treatment,
                   mps000010ADO,
                   null
                   );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000010RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000010RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
                //  result = MPS.Printer.Run(printTypeCode, fileName, mps000010RDO);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InGiayChuyenVien(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;

                //Cap nhat ho so dieu tri
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatment4.ID;
                HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();



                //Thông tin bệnh nhân
                var currentPatient = PrintGlobalStore.getPatient(treatment.ID);


                //Thong tin ve the BHYt
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment.ID, instructionTime, ref PatyAlterBhyt);

                //Lấy dữ liệu về Ra viện của Bn
                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(treatment.ID);
                List<MPS.Processor.Mps000011.PDO.TranPatiReasonADO> TranpatiReasonSDO = new List<MPS.Processor.Mps000011.PDO.TranPatiReasonADO>();
                HIS_TRAN_PATI_FORM TranpatiForm = new HIS_TRAN_PATI_FORM();

                if (treatment.TRAN_PATI_REASON_ID != null)
                {
                    //Lý do chuyển viện
                    List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> listTranpatiReason = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>();

                    if (listTranpatiReason != null && listTranpatiReason.Count > 0)
                    {
                        foreach (var item in listTranpatiReason)
                        {
                            MPS.Processor.Mps000011.PDO.TranPatiReasonADO aTranPatiReason = new MPS.Processor.Mps000011.PDO.TranPatiReasonADO(item);
                            if (item.ID == treatment.TRAN_PATI_REASON_ID)
                            {
                                aTranPatiReason.checkbox = "X";
                            }
                            else
                            {
                                aTranPatiReason.checkbox = "";
                            }
                            TranpatiReasonSDO.Add(aTranPatiReason);
                        }
                    }
                }
                if (treatment.TRAN_PATI_FORM_ID != null)
                {
                    //phuong thuc chuyen vien
                    List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> listTranpatiForm = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>();
                    TranpatiForm = (listTranpatiForm != null && listTranpatiForm.Count > 0) ? listTranpatiForm.Where(o => o.ID == treatment.TRAN_PATI_FORM_ID).ToList().FirstOrDefault() : new HIS_TRAN_PATI_FORM();

                }

                HIS_TRAN_PATI_TECH tranPatiTech = null;

                if (treatment.TRAN_PATI_TECH_ID != null)
                {
                    CommonParam param = new CommonParam();

                    HisTranPatiTechFilter hisTranPatiTechFilter = new HisTranPatiTechFilter();
                    hisTranPatiTechFilter.ID = treatment.TRAN_PATI_TECH_ID;
                    var rsApi = new BackendAdapter(param).Get<List<HIS_TRAN_PATI_TECH>>("api/HisTranPatiTech/Get", ApiConsumers.MosConsumer, hisTranPatiTechFilter, param);
                    if (rsApi != null && rsApi.Count > 0)
                    {
                        tranPatiTech = rsApi.FirstOrDefault();
                    }
                }


                WaitingManager.Hide();
                MPS.Processor.Mps000011.PDO.PatientADO PatientADO = new MPS.Processor.Mps000011.PDO.PatientADO(currentPatient);
                MPS.Processor.Mps000011.PDO.Mps000011PDO mps000011RDO = new MPS.Processor.Mps000011.PDO.Mps000011PDO(
                   PatientADO,
                   PatyAlterBhyt,
                   treatment,
                   TranpatiReasonSDO,
                   TranpatiForm,
                   null, tranPatiTech
                   );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (AllowPrintFinishCFG.ALLOW_PRINT_CHUYEN_VIEN == 1)
                {
                    if (treatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân chưa khóa viện phí");
                    }
                    else
                    {
                        if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000011RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000011RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                        }
                    }
                }
                else
                {
                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000011RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000011RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauPhieuYCBenhAnNgoaiTru(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;

                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatment4.ID;
                HIS_TREATMENT treatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatment.PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();


                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                if (instructionTime < treatment.IN_TIME)
                {
                    instructionTime = treatment.IN_TIME;
                }
                PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment4.ID, instructionTime, ref patientTypeAlter);

                //Lấy thông tin chuyển khoa
                HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                departmentTranFilter.TREATMENT_ID = treatment.ID;
                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departmentTranFilter, param);

                //danh sach yeu cau kham
                HisServiceReqViewFilter serviceReqViewFilter = new HisServiceReqViewFilter();
                V_HIS_SERVICE_REQ ServiceReq = null;
                serviceReqViewFilter.TREATMENT_ID = treatment.ID;
                serviceReqViewFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                ServiceReq = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, serviceReqViewFilter, param).OrderBy(o => o.INTRUCTION_TIME).FirstOrDefault();

                string requestDepartmentName = "";
                if (ServiceReq != null)
                    requestDepartmentName = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == ServiceReq.REQUEST_DEPARTMENT_ID).DEPARTMENT_NAME;
                List<HIS_ICD> listICD = new List<HIS_ICD>();
                if (treatment.TRANSFER_IN_ICD_CODE != null)
                    listICD.Add(new HIS_ICD { ICD_CODE = treatment.TRANSFER_IN_ICD_CODE, ICD_NAME = treatment.TRANSFER_IN_ICD_NAME });
                //thuoc
                MOS.Filter.HisExpMestFilter prescriptionViewFIlter = new HisExpMestFilter();
                prescriptionViewFIlter.TDL_TREATMENT_ID = treatment.ID;

                var prescriptions = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, prescriptionViewFIlter, param) ?? new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();


                List<long> expMestIds = prescriptions.Select(o => o.ID).ToList();
                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_IDs = expMestIds;//TODO
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param) ?? new List<V_HIS_EXP_MEST_MEDICINE>();

                HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.TDL_TREATMENT_ID = treatment.ID;
                List<V_HIS_SERE_SERV_5> sereServMedis = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, param).Where(o => o.MEDICINE_ID != null).ToList();

                HisDhstFilter DhstFilter = new HisDhstFilter();
                DhstFilter.TREATMENT_ID = treatment.ID;
                List<HIS_DHST> listDhst = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, DhstFilter, param).ToList();
                var Dhst = (listDhst != null && listDhst.Count > 0) ? listDhst.First() : new HIS_DHST();

                WaitingManager.Hide();

                //Cấn sửa lại in bệnh án 
                MPS.Processor.Mps000174.PDO.Mps000174PDO.Mps000174ADO ado = new MPS.Processor.Mps000174.PDO.Mps000174PDO.Mps000174ADO();
                if (treatment.TREATMENT_RESULT_ID.HasValue)
                {
                    var treatmentResult = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == treatment.TREATMENT_RESULT_ID.Value);
                    ado.TREATMENT_RESULT_CODE = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_CODE : "";
                    ado.TREATMENT_RESULT_NAME = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_NAME : "";
                }

                MPS.Processor.Mps000174.PDO.Mps000174PDO mps000174RDO = new MPS.Processor.Mps000174.PDO.Mps000174PDO(
                    patient,
                    departmentTrans,
                    patientTypeAlter,
                    ServiceReq,
                    Dhst,
                    treatment,
                    listICD,
                    prescriptions,
                    expMestMedicines,
                    requestDepartmentName,
                    ado
                                );

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000174RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000174RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadBieuMauTheBenhNhan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;

                //Loai Patient_type_name
                V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                if (instructionTime < treatment4.IN_TIME)
                {
                    instructionTime = treatment4.IN_TIME;
                }
                PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment4.ID, instructionTime, ref currentHispatientTypeAlter);
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatment4.PATIENT_ID;
                var currentPatient = new BackendAdapter(param)
                          .Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();
                WaitingManager.Hide();

                MPS.Processor.Mps000178.PDO.Mps000178PDO mps000178RDO = new MPS.Processor.Mps000178.PDO.Mps000178PDO(
                   currentPatient,
                   currentHispatientTypeAlter,
                   treatment4
                   );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment4 != null ? treatment4.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauKhamBenhVaoVien(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatment4.ID;

                var currentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                //Loai Patient_type_name
                V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                if (instructionTime < currentTreatment.IN_TIME)
                {
                    instructionTime = currentTreatment.IN_TIME;
                }
                PrintGlobalStore.LoadCurrentPatientTypeAlter(currentTreatment.ID, instructionTime, ref currentHispatientTypeAlter);

                var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == (currentTreatment.IN_TREATMENT_TYPE_ID ?? 0));
                if (treatmentType != null && currentHispatientTypeAlter != null)
                {
                    currentHispatientTypeAlter.TREATMENT_TYPE_ID = treatmentType.ID;
                    currentHispatientTypeAlter.TREATMENT_TYPE_CODE = treatmentType.TREATMENT_TYPE_CODE;
                    currentHispatientTypeAlter.TREATMENT_TYPE_NAME = treatmentType.TREATMENT_TYPE_NAME;
                }

                //Mức hưởng BHYT
                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                string ratio_text = "";
                if (currentHispatientTypeAlter != null)
                {

                    ratio_text = GetDefaultHeinRatioForView(currentHispatientTypeAlter.HEIN_CARD_NUMBER, currentHispatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, currentHispatientTypeAlter.RIGHT_ROUTE_CODE);
                }
                //yeu cau kham
                V_HIS_SERVICE_REQ currentHisVExamServiceReq = null;
                HisServiceReqViewFilter examServiceReqFilter = new HisServiceReqViewFilter();
                examServiceReqFilter.TREATMENT_ID = currentTreatment.ID;
                examServiceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                var examServiceReqs = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, examServiceReqFilter, param);
                if (examServiceReqs != null)
                {
                    currentHisVExamServiceReq = examServiceReqs.OrderBy(o => o.INTRUCTION_TIME).FirstOrDefault();
                }

                //cac can lam sang
                HisSereServFilter ClsSereServFilter = new HisSereServFilter();
                ClsSereServFilter.TREATMENT_ID = currentTreatment.ID;
                ClsSereServFilter.TDL_SERVICE_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                };
                ClsSereServFilter.HAS_EXECUTE = true;
                //ClsSereServFilter.TDL_INTRUCTION_TIME_TO = currentTreatment.CLINICAL_IN_TIME;
                var ClsSereServs = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ClsSereServFilter, param);
                // tai khoan cho vao vien
                string userName = currentTreatment.IN_USERNAME ?? "";
                V_HIS_ROOM InRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == (currentTreatment.IN_ROOM_ID ?? 0));
                //phong cho vao vien
                string executeRoomName = InRoom != null ? InRoom.ROOM_NAME : "";
                //khoa cho vao vien
                string executeDepartmentName = InRoom != null ? InRoom.DEPARTMENT_NAME : "";

                //chuyen khoa tiep theo
                // V_HIS_DEPARTMENT_TRAN departmentTran = new V_HIS_DEPARTMENT_TRAN();
                MOS.Filter.HisDepartmentTranViewFilter hisDepartmentTranFilter = new HisDepartmentTranViewFilter();
                hisDepartmentTranFilter.TREATMENT_ID = currentTreatment.ID;
                //hisDepartmentTranFilter.PREVIOUS_ID = currentTreatment.IN_DEPARTMENT_ID;
                hisDepartmentTranFilter.ORDER_DIRECTION = "DEPARTMENT_IN_TIME";
                hisDepartmentTranFilter.ORDER_FIELD = "ASC";
                var departmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, hisDepartmentTranFilter, param);

                MPS.Processor.Mps000007.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000007.PDO.SingleKeyValue();
                singleKeyValue.ExecuteDepartmentName = executeDepartmentName;
                singleKeyValue.ExecuteRoomName = executeRoomName;
                singleKeyValue.RatioText = ratio_text;
                singleKeyValue.Username = userName;
                singleKeyValue.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (currentTreatment.ICD_NAME != null)
                {
                    singleKeyValue.Icd_Name = currentTreatment.ICD_NAME;
                }
                else
                {
                    singleKeyValue.Icd_Name = currentTreatment.ICD_NAME;
                }

                //departmentTrans = departmentTrans.Where(p => previous(p, departmentTrans).DEPARTMENT_ID == currentTreatment.IN_DEPARTMENT_ID).ToList()??new List<V_HIS_DEPARTMENT_TRAN>();
                //if (departmentTrans != null && departmentTrans.Count > 0)
                //{
                //    departmentTran = departmentTrans.First();

                //}

                var dhst = new HIS_DHST();
                if (currentHisVExamServiceReq != null && currentHisVExamServiceReq.DHST_ID.HasValue)
                {
                    HisDhstFilter hisDhstFilter = new MOS.Filter.HisDhstFilter();
                    hisDhstFilter.ID = currentHisVExamServiceReq.DHST_ID.Value;
                    dhst = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, hisDhstFilter, param).FirstOrDefault();
                }


                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = currentTreatment.PATIENT_ID;
                var currentPatient = new BackendAdapter(param)
                          .Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                WaitingManager.Hide();

                MPS.Processor.Mps000007.PDO.Mps000007PDO mps000007RDO = new MPS.Processor.Mps000007.PDO.Mps000007PDO(
                    currentPatient,
                    currentHispatientTypeAlter,
                    departmentTrans,
                    currentHisVExamServiceReq,
                    dhst,
                    currentTreatment,
                    ClsSereServs,
                    singleKeyValue
                    );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment4 != null ? treatment4.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000007RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000007RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private V_HIS_DEPARTMENT_TRAN previous(V_HIS_DEPARTMENT_TRAN p, List<V_HIS_DEPARTMENT_TRAN> l)
        {
            var prev = l.Where(o => o.ID == p.PREVIOUS_ID).ToList();
            return (prev != null && prev.Count > 0) ? prev.First() : new V_HIS_DEPARTMENT_TRAN();
        }

        public string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void Mps000206(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                V_HIS_TREATMENT_4 _Treatment = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;

                //yeu cau kham
                HisServiceReqViewFilter examServiceReqFilter = new HisServiceReqViewFilter();
                examServiceReqFilter.TREATMENT_ID = _Treatment.ID;
                examServiceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                var _currentExam = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, examServiceReqFilter, param).FirstOrDefault();

                MOS.Filter.HisSereServView7Filter sereServFilter = new HisSereServView7Filter();
                sereServFilter.TDL_TREATMENT_ID = _Treatment.ID;
                sereServFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                var _currentSereServTests = new BackendAdapter(param)
                    .Get<List<V_HIS_SERE_SERV_7>>("api/HisSereServ/GetView7", ApiConsumers.MosConsumer, sereServFilter, param);

                MOS.Filter.HisDhstFilter dhstFilter = new HisDhstFilter();
                if (_currentExam != null && _currentExam.DHST_ID > 0)
                {
                    dhstFilter.ID = _currentExam.DHST_ID;
                }
                else
                {
                    dhstFilter.TREATMENT_ID = _Treatment.ID;
                }

                var _currentDhst = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();


                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = _Treatment.PATIENT_ID;
                var _currentPatient = new BackendAdapter(param)
                          .Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                SCN.Filter.ScnAllergicViewFilter allergicFilter = new SCN.Filter.ScnAllergicViewFilter();
                //allergicFilter.PATIENT_CODE__EXACT = _currentPatient.PATIENT_CODE;
                //var _currentAllergic = new BackendAdapter(param)
                //          .Get<List<V_SCN_ALLERGIC>>("api/ScnAllergic/GetView", ApiConsumers.ScnConsumer, allergicFilter, param);
                var listAllergic = new BackendAdapter(param)
                          .Get<List<V_SCN_ALLERGIC>>("api/ScnAllergic/GetView", ApiConsumers.ScnConsumer, allergicFilter, param);
                List<V_SCN_ALLERGIC> _currentAllergic = new List<V_SCN_ALLERGIC>();
                if (listAllergic != null && listAllergic.Count > 0)
                {
                    _currentAllergic = listAllergic.Where(p => p.PERSON_CODE == _currentPatient.PATIENT_CODE).ToList();
                }

                SCN.Filter.ScnDisabilityViewFilter disabilityFilter = new SCN.Filter.ScnDisabilityViewFilter();
                disabilityFilter.PERSON_CODE__EXACT = _currentPatient.PATIENT_CODE;
                var _currentDisability = new BackendAdapter(param)
                          .Get<List<V_SCN_DISABILITY>>("api/ScnDisability/GetView", ApiConsumers.ScnConsumer, disabilityFilter, param);

                SCN.Filter.ScnDiseaseViewFilter diseaseFilter = new SCN.Filter.ScnDiseaseViewFilter();
                //diseaseFilter.PATIENT_CODE__EXACT = _currentPatient.PATIENT_CODE;
                //var _currentDisease = new BackendAdapter(param)
                //          .Get<List<V_SCN_DISEASE>>("api/ScnDisease/GetView", ApiConsumers.ScnConsumer, diseaseFilter, param);
                List<V_SCN_DISEASE> _currentDisease = new List<V_SCN_DISEASE>();
                var listDisease = new BackendAdapter(param)
                          .Get<List<V_SCN_DISEASE>>("api/ScnDisease/GetView", ApiConsumers.ScnConsumer, diseaseFilter, param);
                if (listDisease != null && listDisease.Count > 0)
                {
                    _currentDisease = listDisease.Where(p => p.PERSON_CODE == _currentPatient.PATIENT_CODE).ToList();
                }

                SCN.Filter.ScnHealthRiskViewFilter healthRiskFilter = new SCN.Filter.ScnHealthRiskViewFilter();
                healthRiskFilter.PERSON_CODE__EXACT = _currentPatient.PATIENT_CODE;
                var _currentHealthRisk = new BackendAdapter(param)
                          .Get<List<V_SCN_HEALTH_RISK>>("api/ScnHealthRisk/GetView", ApiConsumers.ScnConsumer, healthRiskFilter, param);

                SCN.Filter.ScnSurgeryFilter surgeryFilter = new SCN.Filter.ScnSurgeryFilter();
                surgeryFilter.PERSON_CODE__EXACT = _currentPatient.PATIENT_CODE;
                var _currentSurgery = new BackendAdapter(param)
                          .Get<List<SCN_SURGERY>>("api/ScnSurgery/Get", ApiConsumers.ScnConsumer, surgeryFilter, param);



                List<V_SCN_ALLERGIC> _fatherAllergic = new List<V_SCN_ALLERGIC>();
                List<V_SCN_DISEASE> _fatherDisease = new List<V_SCN_DISEASE>();
                //if (_currentPatient != null && _currentPatient.FATHER_ID > 0)
                //{
                //    MOS.Filter.HisPatientFilter fatherFilter = new HisPatientFilter();
                //    fatherFilter.ID = _currentPatient.FATHER_ID;
                //    var _fatherPatient = new BackendAdapter(param)
                //                .Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, fatherFilter, param).FirstOrDefault();

                //    _fatherAllergic = listAllergic != null ? (listAllergic.Count > 0 ? listAllergic.Where(x => x.PATIENT_CODE == _fatherPatient.PATIENT_CODE).ToList() : null) : null;

                //    _fatherDisease = listDisease != null ? (listDisease.Count > 0 ? listDisease.Where(x => x.PATIENT_CODE == _fatherPatient.PATIENT_CODE).ToList() : null) : null;
                //}

                List<V_SCN_ALLERGIC> _motherAllergic = new List<V_SCN_ALLERGIC>();
                List<V_SCN_DISEASE> _motherDisease = new List<V_SCN_DISEASE>();
                //if (_currentPatient != null && _currentPatient.MOTHER_ID > 0)
                //{
                //    MOS.Filter.HisPatientFilter motherFilter = new HisPatientFilter();
                //    motherFilter.ID = _currentPatient.MOTHER_ID;
                //    var _motherPatient = new BackendAdapter(param)
                //               .Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, motherFilter, param).FirstOrDefault();


                //    _motherAllergic = listAllergic != null ? (listAllergic.Count > 0 ? listAllergic.Where(x => x.PATIENT_CODE == _motherPatient.PATIENT_CODE).ToList() : null) : null;

                //    _motherDisease = listDisease != null ? (listDisease.Count > 0 ? listDisease.Where(x => x.PATIENT_CODE == _motherPatient.PATIENT_CODE).ToList() : null) : null;

                //}

                SCN.Filter.ScnAllergicTypeFilter allergicTypeFilter = new SCN.Filter.ScnAllergicTypeFilter();
                var _currentAllergicType = new BackendAdapter(param)
                          .Get<List<SCN_ALLERGIC_TYPE>>("api/ScnAllergicType/Get", ApiConsumers.ScnConsumer, allergicTypeFilter, param);
                List<MPS.Processor.Mps000206.PDO.Mps000206PDO.AllergicTypeRelative> _AllergicTypeRelatives = new List<MPS.Processor.Mps000206.PDO.Mps000206PDO.AllergicTypeRelative>();
                if (_currentAllergicType != null && _currentAllergicType.Count > 0)
                {
                    foreach (var item in _currentAllergicType)
                    {
                        MPS.Processor.Mps000206.PDO.Mps000206PDO.AllergicTypeRelative ado = new MPS.Processor.Mps000206.PDO.Mps000206PDO.AllergicTypeRelative();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000206.PDO.Mps000206PDO.AllergicTypeRelative>(ado, item);
                        V_SCN_ALLERGIC allergicFather = _fatherAllergic != null ? _fatherAllergic.Count > 0 ? _fatherAllergic.FirstOrDefault(p => p.ALLERGIC_TYPE_ID == item.ID) : null : null;
                        if (allergicFather != null)
                        {
                            ado.Father = "Bố";
                            ado.Description_Father = allergicFather.DESCRIPTION;
                        }

                        V_SCN_ALLERGIC allergicMother = _motherAllergic != null ? _motherAllergic.Count > 0 ? _motherAllergic.FirstOrDefault(p => p.ALLERGIC_TYPE_ID == item.ID) : null : null;
                        if (allergicMother != null)
                        {
                            ado.Mother = "Mẹ";
                            ado.Description_Mother = allergicMother.DESCRIPTION;
                        }
                        _AllergicTypeRelatives.Add(ado);
                    }
                }


                SCN.Filter.ScnDiseaseTypeFilter diseaseTypeFilter = new SCN.Filter.ScnDiseaseTypeFilter();
                var _currentDiseaseType = new BackendAdapter(param)
                          .Get<List<SCN_DISEASE_TYPE>>("api/ScnDiseaseType/Get", ApiConsumers.ScnConsumer, diseaseTypeFilter, param);
                List<MPS.Processor.Mps000206.PDO.Mps000206PDO.DiseaseTypeRelative> _DiseaseTypeRelatives = new List<MPS.Processor.Mps000206.PDO.Mps000206PDO.DiseaseTypeRelative>();
                if (_currentDiseaseType != null && _currentDiseaseType.Count > 0)
                {
                    foreach (var item in _currentDiseaseType)
                    {
                        MPS.Processor.Mps000206.PDO.Mps000206PDO.DiseaseTypeRelative ado = new MPS.Processor.Mps000206.PDO.Mps000206PDO.DiseaseTypeRelative();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000206.PDO.Mps000206PDO.DiseaseTypeRelative>(ado, item);
                        V_SCN_DISEASE allergicFather = _fatherDisease != null ? _fatherDisease.Count > 0 ? _fatherDisease.FirstOrDefault(p => p.DISEASE_TYPE_ID == item.ID) : null : null;
                        if (allergicFather != null)
                        {
                            ado.Father = "Bố";
                            ado.Description_Father = allergicFather.DESCRIPTION;
                        }

                        V_SCN_DISEASE diseaseMother = _motherDisease != null ? _motherDisease.Count > 0 ? _motherDisease.FirstOrDefault(p => p.DISEASE_TYPE_ID == item.ID) : null : null;
                        if (diseaseMother != null)
                        {
                            ado.Mother = "Mẹ";
                            ado.Description_Mother = diseaseMother.DESCRIPTION;
                        }

                        _DiseaseTypeRelatives.Add(ado);
                    }
                }



                WaitingManager.Hide();
                MPS.Processor.Mps000206.PDO.Mps000206PDO mps000206RDO = new MPS.Processor.Mps000206.PDO.Mps000206PDO(
                    _Treatment,
                    _currentPatient,
                    _currentExam,
                    _currentDhst,
                    _currentSereServTests,
                    _currentAllergic,
                    _currentDisability,
                    _currentDisease,
                    _currentHealthRisk,
                    _currentSurgery,
                    _AllergicTypeRelatives,
                    _DiseaseTypeRelatives
                    );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode);
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000206RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000206RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void Mps000268(string printTypeCode)
        {
            try
            {
                var treatment4 = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetFocusedRow();

                if (treatment4 != null)
                {
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, treatment4);
                    HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor print = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(treatment, BranchDataWorker.Branch, currentModule != null ? currentModule.RoomId : 0);
                    print.Print(printTypeCode, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Mps000269(string printTypeCode)
        {
            try
            {
                var treatment4 = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetFocusedRow();

                if (treatment4 != null)
                {
                    PrintTreatmentEndTypeExtProcessor printTreatmentEndTypeExtProcessor = new PrintTreatmentEndTypeExtProcessor(treatment4.ID, CreateMenu.TYPE.DYNAMIC, currentModule != null ? currentModule.RoomId : 0);

                    printTreatmentEndTypeExtProcessor.Print(HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.Base.PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM, PrintTreatmentEndTypeExtProcessor.OPTION.PRINT);
                    //HIS_TREATMENT treatment = new HIS_TREATMENT();
                    //Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, treatment4);
                    //HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor print = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(treatment, currentModule != null ? currentModule.RoomId : 0);
                    //print.Print(printTypeCode, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Mps000330(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;

                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatment4.ID;
                HIS_TREATMENT treatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatment.PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();


                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                if (instructionTime < treatment.IN_TIME)
                {
                    instructionTime = treatment.IN_TIME;
                }
                PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment4.ID, instructionTime, ref patientTypeAlter);

                //Lấy thông tin chuyển khoa
                HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                departmentTranFilter.TREATMENT_ID = treatment.ID;
                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departmentTranFilter, param);

                //danh sach yeu cau kham
                HisServiceReqViewFilter serviceReqViewFilter = new HisServiceReqViewFilter();
                V_HIS_SERVICE_REQ ServiceReq = null;
                serviceReqViewFilter.TREATMENT_ID = treatment.ID;
                serviceReqViewFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                ServiceReq = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, serviceReqViewFilter, param).OrderBy(o => o.INTRUCTION_TIME).FirstOrDefault();

                string requestDepartmentName = "";
                if (ServiceReq != null)
                    requestDepartmentName = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == ServiceReq.REQUEST_DEPARTMENT_ID).DEPARTMENT_NAME;
                List<HIS_ICD> listICD = new List<HIS_ICD>();
                if (treatment.TRANSFER_IN_ICD_CODE != null)
                    listICD.Add(new HIS_ICD { ICD_CODE = treatment.TRANSFER_IN_ICD_CODE, ICD_NAME = treatment.TRANSFER_IN_ICD_NAME });
                //thuoc
                MOS.Filter.HisExpMestFilter prescriptionViewFIlter = new HisExpMestFilter();
                prescriptionViewFIlter.TDL_TREATMENT_ID = treatment.ID;

                var prescriptions = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, prescriptionViewFIlter, param) ?? new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();


                List<long> expMestIds = prescriptions.Select(o => o.ID).ToList();
                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_IDs = expMestIds;//TODO
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param) ?? new List<V_HIS_EXP_MEST_MEDICINE>();

                HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.TDL_TREATMENT_ID = treatment.ID;
                List<V_HIS_SERE_SERV_5> sereServMedis = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, param).Where(o => o.MEDICINE_ID != null).ToList();

                HisDhstFilter DhstFilter = new HisDhstFilter();
                DhstFilter.TREATMENT_ID = treatment.ID;
                List<HIS_DHST> listDhst = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, DhstFilter, param).ToList();
                var Dhst = (listDhst != null && listDhst.Count > 0) ? listDhst.First() : new HIS_DHST();

                WaitingManager.Hide();

                //Cấn sửa lại in bệnh án 

                MPS.Processor.Mps000330.PDO.Mps000330PDO mps000330RDO = new MPS.Processor.Mps000330.PDO.Mps000330PDO(
                    patient,
                    departmentTrans,
                    patientTypeAlter,
                    ServiceReq,
                    Dhst,
                    treatment,
                    listICD,
                    prescriptions,
                    expMestMedicines,
                    requestDepartmentName
                                );

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000330RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000330RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void Mps000331(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;

                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatment4.ID;
                HIS_TREATMENT treatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatment.PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();


                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                if (instructionTime < treatment.IN_TIME)
                {
                    instructionTime = treatment.IN_TIME;
                }
                PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment4.ID, instructionTime, ref patientTypeAlter);

                //Lấy thông tin chuyển khoa
                HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                departmentTranFilter.TREATMENT_ID = treatment.ID;
                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departmentTranFilter, param);

                //danh sach yeu cau kham
                HisServiceReqViewFilter serviceReqViewFilter = new HisServiceReqViewFilter();
                V_HIS_SERVICE_REQ ServiceReq = null;
                serviceReqViewFilter.TREATMENT_ID = treatment.ID;
                serviceReqViewFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                ServiceReq = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, serviceReqViewFilter, param).OrderBy(o => o.INTRUCTION_TIME).FirstOrDefault();

                string requestDepartmentName = "";
                if (ServiceReq != null)
                    requestDepartmentName = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == ServiceReq.REQUEST_DEPARTMENT_ID).DEPARTMENT_NAME;
                List<HIS_ICD> listICD = new List<HIS_ICD>();
                if (treatment.TRANSFER_IN_ICD_CODE != null)
                    listICD.Add(new HIS_ICD { ICD_CODE = treatment.TRANSFER_IN_ICD_CODE, ICD_NAME = treatment.TRANSFER_IN_ICD_NAME });
                //thuoc
                MOS.Filter.HisExpMestFilter prescriptionViewFIlter = new HisExpMestFilter();
                prescriptionViewFIlter.TDL_TREATMENT_ID = treatment.ID;

                var prescriptions = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, prescriptionViewFIlter, param) ?? new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();


                List<long> expMestIds = prescriptions.Select(o => o.ID).ToList();
                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_IDs = expMestIds;//TODO
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param) ?? new List<V_HIS_EXP_MEST_MEDICINE>();

                HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.TDL_TREATMENT_ID = treatment.ID;
                List<V_HIS_SERE_SERV_5> sereServMedis = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, param).Where(o => o.MEDICINE_ID != null).ToList();

                HisDhstFilter DhstFilter = new HisDhstFilter();
                DhstFilter.TREATMENT_ID = treatment.ID;
                List<HIS_DHST> listDhst = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, DhstFilter, param).ToList();
                var Dhst = (listDhst != null && listDhst.Count > 0) ? listDhst.First() : new HIS_DHST();

                WaitingManager.Hide();

                //Cấn sửa lại in bệnh án 

                MPS.Processor.Mps000331.PDO.Mps000331PDO mps000331RDO = new MPS.Processor.Mps000331.PDO.Mps000331PDO(
                    patient,
                    departmentTrans,
                    patientTypeAlter,
                    ServiceReq,
                    Dhst,
                    treatment,
                    listICD,
                    prescriptions,
                    expMestMedicines,
                    requestDepartmentName
                                );

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000331RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000331RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
    }
}
