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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Print;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using DevExpress.XtraBars;
using Inventec.Desktop.Common.LanguageManager;
using MOS.SDO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Common.FlexCelPrint.Ado;

namespace HIS.Desktop.Plugins.Debate
{
    public partial class frmDebate : HIS.Desktop.Utility.FormBase
    {
        private void PrintMedicine_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintDebateMenuProcessor.ModuleType type = (PrintDebateMenuProcessor.ModuleType)(e.Item.Tag);

                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    switch (type)
                    {
                        case PrintDebateMenuProcessor.ModuleType.BIEN_BAN_HOI_CHAN:
                            richEditorMain.RunPrintTemplate(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__TRICH_BIEN_BAN_HOI_CHAN__MPS000019, DelegateRunPrinter);
                            break;
                        case PrintDebateMenuProcessor.ModuleType.BIEN_BAN_HOI_CHAN_THUOC_DAU_SAO:
                            richEditorMain.RunPrintTemplate(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__SO_BIEN_BAN_HOI_CHAN_DAU_SAO__MPS000323, DelegateRunPrinter);
                            break;
                        case PrintDebateMenuProcessor.ModuleType.SO_BIEN_BAN_HOI_CHAN:
                            richEditorMain.RunPrintTemplate(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__SO_BIEN_BAN_HOI_CHAN__MPS000020, DelegateRunPrinter);
                            break;
                        case PrintDebateMenuProcessor.ModuleType.HOI_CHAN_PTTT:
                            richEditorMain.RunPrintTemplate("Mps000387", DelegateRunPrinter);
                            break;
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
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__TRICH_BIEN_BAN_HOI_CHAN__MPS000019:
                        InTrichBienBanHoiChanProcess(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__SO_BIEN_BAN_HOI_CHAN_DAU_SAO__MPS000323:
                        InTrichBienBanHoiChanThuocDauSaoProcess(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__SO_BIEN_BAN_HOI_CHAN__MPS000020:
                        if (isGroupMps020)
                        {
                            this.currentTypeCode020 = printTypeCode;
                            this.currentFileName020 = fileName;
                            InSoBienBanHoiChanProcessGroup(printTypeCode, fileName, ref result, this.debateToPrint);
                        }
                        else
                            InSoBienBanHoiChanProcess(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000387":
                        InHoiChanPtttProcess(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InSoBienBanHoiChanProcessGroup(string printTypeCode, string fileName, ref bool result, V_HIS_DEBATE debate)
        {
            try
            {
                if (debate == null)
                    return;
                var patientADO = PrintGlobalStore.getPatient(debate.TREATMENT_ID);
                //Lấy dữ liệu ra viện
                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(debate.TREATMENT_ID);

                string currentDateSeparateFullTime = HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentDateSeparateFullTime();

                //Thông tin bảo hiểm y tế
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var patyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(debate.TREATMENT_ID, instructionTime, ref patyAlterBhyt);

                HIS_DEBATE _HisDebate = GetDebate(debate.ID);
                //chủ tọa, thu ký
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = null;
                if (_HisDebate.HIS_DEBATE_USER == null || _HisDebate.HIS_DEBATE_USER.Count == 0)
                {
                    MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                    hisDebateUserFilter.DEBATE_ID = _HisDebate.ID;
                    hisDebateUserFilter.ORDER_DIRECTION = "ASC";
                    hisDebateUserFilter.ORDER_FIELD = "ID";
                    lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                    _HisDebate.HIS_DEBATE_USER = lstHisDebateUser;
                }
                else
                {
                    lstHisDebateUser = _HisDebate.HIS_DEBATE_USER.ToList();
                }

                MOS.Filter.HisDebateViewFilter dbFilter = new MOS.Filter.HisDebateViewFilter();
                dbFilter.ID = _HisDebate.ID;
                var DebateView = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEBATE>>("api/HisDebate/GetView", ApiConsumer.ApiConsumers.MosConsumer, dbFilter, null);

                WaitingManager.Hide();

                MPS.Processor.Mps000020.PDO.Mps000020PDO Mps000020PDO = new MPS.Processor.Mps000020.PDO.Mps000020PDO(
                    patientADO,
                   roomName,
                   departmentName,
                   _HisDebate,
                   departmentTran,
                   currentDateSeparateFullTime,
                   patyAlterBhyt,
                   treatmentToPrint,
                     DebateView[0]);

                //Print.PrintData(printTypeCode, fileName, Mps000020PDO, null, SetDataGroup);
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatmentToPrint != null ? treatmentToPrint.TREATMENT_CODE : ""), printTypeCode, this.Module != null ? this.Module.RoomId : 0);

                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000020PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000020PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }

                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_DEBATE GetDebate(long iD)
        {
            HIS_DEBATE result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDebateViewFilter filter = new MOS.Filter.HisDebateViewFilter();
                filter.ID = iD;
                result = new BackendAdapter(param).Get<List<HIS_DEBATE>>("api/HisDebate/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SetDataGroup(Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo data)
        {
            try
            {
                if (data != null)
                {
                    if (this.GroupStreamPrint == null)
                    {
                        this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    }

                    this.GroupStreamPrint.Add(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InTrichBienBanHoiChanThuocDauSaoProcess(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();//Lấy thông tin bệnh nhân
                var currentVDebate = (V_HIS_DEBATE)gridViewDebateReq.GetFocusedRow();
                if (currentVDebate == null)
                    return;
                var patient = PrintGlobalStore.getPatient(currentVDebate.TREATMENT_ID);

                //Lấy dữ liệu ra viện
                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(currentVDebate.TREATMENT_ID);

                //chủ tọa, thu ký
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = null;
                if (currentHisDebate != null)
                {
                    if (currentHisDebate.HIS_DEBATE_USER == null || currentHisDebate.HIS_DEBATE_USER.Count == 0)
                    {
                        MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                        hisDebateUserFilter.DEBATE_ID = currentHisDebate.ID;
                        hisDebateUserFilter.ORDER_DIRECTION = "ASC";
                        hisDebateUserFilter.ORDER_FIELD = "ID";
                        lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                        currentHisDebate.HIS_DEBATE_USER = lstHisDebateUser;
                    }
                    else
                    {
                        lstHisDebateUser = currentHisDebate.HIS_DEBATE_USER.ToList();
                    }
                }

                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_ID = currentVDebate.TREATMENT_ID;
                var _TreatmentBedRoom = new BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, bedFilter, null);

                MPS.Processor.Mps000323.PDO.Mps000323PDO.Mps000323SingleKey single = new MPS.Processor.Mps000323.PDO.Mps000323PDO.Mps000323SingleKey();
                single.bebRoomName = roomName;
                single.departmentName = departmentName;
                single.genderCode__Male = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE).GENDER_CODE;
                single.genderCode__FeMale = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE).GENDER_CODE;
                if (_TreatmentBedRoom != null && _TreatmentBedRoom.Count > 0)
                {
                    var bed = _TreatmentBedRoom.Where(o => o.BED_ID.HasValue).OrderByDescending(o => o.OUT_TIME.HasValue).FirstOrDefault();
                    if (bed != null)
                    {
                        var lastBed = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED>().FirstOrDefault(o => o.ID == bed.BED_ID);
                        single.BED_CODE = lastBed != null ? lastBed.BED_CODE : "";
                        single.BED_NAME = lastBed != null ? lastBed.BED_NAME : "";
                    }
                }

                WaitingManager.Hide();

                MPS.Processor.Mps000323.PDO.Mps000323PDO Mps000323PDO = new MPS.Processor.Mps000323.PDO.Mps000323PDO(
                    patient,
                   currentHisDebate,
                   departmentTran,
                   single,
                   treatmentToPrint);
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                string printerName = "";

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000323PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000323PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentToPrint != null ? treatmentToPrint.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);
                if (lciChkAutoSign.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoSign.Checked)
                {
                    List<SignerConfigDTO> signerList = ProcessSigner(lstHisDebateUser);
                    inputADO.SignerConfigs = signerList;
                }
                PrintData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InTrichBienBanHoiChanProcess(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();//Lấy thông tin bệnh nhân
                var currentVDebate = (V_HIS_DEBATE)gridViewDebateReq.GetFocusedRow();
                var patientADO = PrintGlobalStore.getPatient(currentVDebate.TREATMENT_ID);

                //Lấy dữ liệu ra viện
                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(currentVDebate.TREATMENT_ID);

                //chủ tọa, thu ký
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = null;
                if (currentHisDebate.HIS_DEBATE_USER == null || currentHisDebate.HIS_DEBATE_USER.Count == 0)
                {
                    MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                    hisDebateUserFilter.DEBATE_ID = currentVDebate.ID;
                    hisDebateUserFilter.ORDER_DIRECTION = "ASC";
                    hisDebateUserFilter.ORDER_FIELD = "ID";
                    lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                    currentHisDebate.HIS_DEBATE_USER = lstHisDebateUser;
                }
                else
                {
                    lstHisDebateUser = currentHisDebate.HIS_DEBATE_USER.ToList();
                }

                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_ID = currentVDebate.TREATMENT_ID;
                var _TreatmentBedRoom = new BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, bedFilter, null);

                MPS.Processor.Mps000019.PDO.Mps000019PDO.Mps000019SingleKey single = new MPS.Processor.Mps000019.PDO.Mps000019PDO.Mps000019SingleKey();
                single.bebRoomName = roomName;
                single.departmentName = departmentName;
                single.genderCode__Male = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE).GENDER_CODE;
                single.genderCode__FeMale = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE).GENDER_CODE;
                if (_TreatmentBedRoom != null && _TreatmentBedRoom.Count > 0)
                {
                    var bed = _TreatmentBedRoom.Where(o => o.BED_ID.HasValue).OrderByDescending(o => o.OUT_TIME.HasValue).FirstOrDefault();
                    if (bed != null)
                    {
                        var lastBed = BackendDataWorker.Get<V_HIS_BED>().FirstOrDefault(o => o.ID == bed.BED_ID);
                        single.BED_CODE = lastBed != null ? lastBed.BED_CODE : "";
                        single.BED_NAME = lastBed != null ? lastBed.BED_NAME : "";
                    }
                }
                if (treatmentToPrint != null)
                {
                    single.IN_CODE = treatmentToPrint.IN_CODE;
                }
                //single.genderCode__FeMale
                WaitingManager.Hide();

                MPS.Processor.Mps000019.PDO.Mps000019PDO Mps000019PDO = new MPS.Processor.Mps000019.PDO.Mps000019PDO(
                    patientADO,
                   currentVDebate,
                   departmentTran,
                   single,
                   treatmentToPrint,
                   lstHisDebateUser);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000019PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000019PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }



                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentToPrint != null ? treatmentToPrint.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);
                if (lciChkAutoSign.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoSign.Checked)
                {
                    List<SignerConfigDTO> signerList = ProcessSigner(lstHisDebateUser);
                    inputADO.SignerConfigs = signerList;
                }

                PrintData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<SignerConfigDTO> ProcessSigner(List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> debateUserList)
        {
            List<SignerConfigDTO> result = new List<SignerConfigDTO>();
            try
            {
                if (debateUserList == null || debateUserList.Count == 0)
                {
                    return null;
                }
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                debateUserList = debateUserList.OrderBy(o => o.ID).ToList();
                int dem = 3;
                foreach (var debateUser in debateUserList)
                {
                    SignerConfigDTO singerLogin = new SignerConfigDTO();

                    if (debateUser.LOGINNAME == loginName)
                    {
                        if (debateUser.IS_SECRETARY == (short)1)
                        {
                            singerLogin.Loginname = loginName;
                            singerLogin.NumOrder = 2;
                        }
                        else if (!debateUser.IS_PRESIDENT.HasValue && !debateUser.IS_SECRETARY.HasValue)
                        {
                            singerLogin.Loginname = loginName;
                            singerLogin.NumOrder = 1;
                        }
                        else if (debateUser.IS_PRESIDENT == (short)1)
                        {
                            singerLogin.Loginname = debateUser.LOGINNAME;
                            singerLogin.NumOrder = 100;
                        }
                    }
                    else if (debateUser.IS_PRESIDENT == (short)1)
                    {
                        singerLogin.Loginname = debateUser.LOGINNAME;
                        singerLogin.NumOrder = 100;
                    }
                    else if (debateUser.IS_SECRETARY == (short)1)
                    {
                        singerLogin.Loginname = debateUser.LOGINNAME;
                        singerLogin.NumOrder = 2;
                    }
                    else
                    {
                        singerLogin.Loginname = debateUser.LOGINNAME;
                        singerLogin.NumOrder = dem++;
                    }

                    result.Add(singerLogin);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private void InSoBienBanHoiChanProcess(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();//Lấy thông tin bệnh nhân
                if (currentHisDebate == null)
                    return;
                var patientADO = PrintGlobalStore.getPatient(currentHisDebate.TREATMENT_ID);
                //Lấy dữ liệu ra viện
                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(currentHisDebate.TREATMENT_ID);

                string currentDateSeparateFullTime = HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentDateSeparateFullTime();

                //Thông tin bảo hiểm y tế
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var patyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(currentHisDebate.TREATMENT_ID, instructionTime, ref patyAlterBhyt);

                //chủ tọa, thu ký
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = null;
                if (currentHisDebate.HIS_DEBATE_USER == null || currentHisDebate.HIS_DEBATE_USER.Count == 0)
                {
                    MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                    hisDebateUserFilter.DEBATE_ID = currentHisDebate.ID;
                    hisDebateUserFilter.ORDER_DIRECTION = "ASC";
                    hisDebateUserFilter.ORDER_FIELD = "ID";
                    lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                    currentHisDebate.HIS_DEBATE_USER = lstHisDebateUser;
                }
                else
                {
                    lstHisDebateUser = currentHisDebate.HIS_DEBATE_USER.ToList();
                }
                MOS.Filter.HisDebateViewFilter dbFilter = new MOS.Filter.HisDebateViewFilter();
                dbFilter.ID = currentHisDebate.ID;
                var DebateView = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEBATE>>("api/HisDebate/GetView", ApiConsumer.ApiConsumers.MosConsumer, dbFilter, null).FirstOrDefault(); ;

                WaitingManager.Hide();

                MPS.Processor.Mps000020.PDO.Mps000020PDO Mps000020PDO = new MPS.Processor.Mps000020.PDO.Mps000020PDO(
                    patientADO,
                   roomName,
                   departmentName,
                   currentHisDebate,
                   departmentTran,
                   currentDateSeparateFullTime,
                   patyAlterBhyt,
                   treatmentToPrint,
                   DebateView);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000020PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000020PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentToPrint != null ? treatmentToPrint.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);
                if (lciChkAutoSign.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoSign.Checked)
                {
                    List<SignerConfigDTO> signerList = ProcessSigner(lstHisDebateUser);
                    inputADO.SignerConfigs = signerList;
                }

                PrintData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InHoiChanPtttProcess(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                var currentVDebate = (V_HIS_DEBATE)gridViewDebateReq.GetFocusedRow();
                if (currentVDebate == null || currentHisDebate == null)
                    return;
                V_HIS_DEBATE debatePrint = new V_HIS_DEBATE();
                MOS.Filter.HisDebateViewFilter dbFilter = new MOS.Filter.HisDebateViewFilter();
                dbFilter.ID = currentHisDebate.ID;
                var lstdebate = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_DEBATE>>("api/HisDebate/GetView", ApiConsumer.ApiConsumers.MosConsumer, dbFilter, null);
                if (lstdebate != null && lstdebate.Count > 0)
                {
                    debatePrint = lstdebate.FirstOrDefault();
                }

                //Lấy dữ liệu ra viện
                V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(currentVDebate.TREATMENT_ID);

                //chủ tọa, thu ký
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = null;
                if (currentHisDebate.HIS_DEBATE_USER == null || currentHisDebate.HIS_DEBATE_USER.Count == 0)
                {
                    MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                    hisDebateUserFilter.DEBATE_ID = currentHisDebate.ID;
                    lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                    currentHisDebate.HIS_DEBATE_USER = lstHisDebateUser;
                }
                else
                {
                    lstHisDebateUser = currentHisDebate.HIS_DEBATE_USER.ToList();
                }

                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_ID = currentHisDebate.TREATMENT_ID;
                var _TreatmentBedRoom = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bedFilter, null);

                MPS.Processor.Mps000387.PDO.Mps000387PDO.Mps000387SingleKey single = new MPS.Processor.Mps000387.PDO.Mps000387PDO.Mps000387SingleKey();
                single.BEB_ROOM_NAME = roomName;
                single.DEPARTMENT_NAME = departmentName;
                if (_TreatmentBedRoom != null && _TreatmentBedRoom.Count > 0)
                {
                    var bed = _TreatmentBedRoom.Where(o => o.BED_ID.HasValue).OrderByDescending(o => o.OUT_TIME.HasValue).FirstOrDefault();
                    if (bed != null)
                    {
                        var lastBed = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED>().FirstOrDefault(o => o.ID == bed.BED_ID);
                        single.BED_CODE = lastBed != null ? lastBed.BED_CODE : "";
                        single.BED_NAME = lastBed != null ? lastBed.BED_NAME : "";
                    }
                }

                List<V_HIS_DEBATE_EKIP_USER> lstHisDebatEkipUser = null;
                MOS.Filter.HisDebateEkipUserViewFilter hisDebateEkipUserFilter = new MOS.Filter.HisDebateEkipUserViewFilter();
                hisDebateEkipUserFilter.DEBATE_ID = currentHisDebate.ID;
                lstHisDebatEkipUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_DEBATE_EKIP_USER>>("api/HisDebateEkipUser/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisDebateEkipUserFilter, null);

                //Thông tin bảo hiểm y tế
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var patyAlterBhyt = new MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(currentVDebate.TREATMENT_ID, instructionTime, ref patyAlterBhyt);

                WaitingManager.Hide();
                MPS.Processor.Mps000387.PDO.Mps000387PDO Mps000387PDO = new MPS.Processor.Mps000387.PDO.Mps000387PDO(
                    treatmentToPrint,
                    debatePrint,
                    departmentTran,
                    patyAlterBhyt,
                    lstHisDebateUser,
                    lstHisDebatEkipUser,
                    single
                   );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                string printerName = "";

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000387PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000387PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentToPrint != null ? treatmentToPrint.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);
                if (lciChkAutoSign.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoSign.Checked)
                {
                    List<SignerConfigDTO> signerList = ProcessSigner(lstHisDebateUser);
                    inputADO.SignerConfigs = signerList;
                }
                PrintData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

