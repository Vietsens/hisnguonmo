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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.SignLibrary.DTO;
using MOS.EFMODEL.DataModels;
using Inventec.Core;

namespace HIS.Desktop.Plugins.DebateDiagnostic
{
    public partial class FormDebateDiagnostic : HIS.Desktop.Utility.FormBase
    {
        public enum ModulePrintType
        {
            BIEN_BAN_HOI_CHAN,
            BIEN_BAN_HOI_CHAN_THUOC_DAU_SAO,
            SO_BIEN_BAN_HOI_CHAN,
            HOI_CHAN_PTTT
        }

        private void FillDataToButtonPrint()
        {
            try
            {
                DevExpress.Utils.Menu.DXPopupMenu menu = new DevExpress.Utils.Menu.DXPopupMenu();
                DevExpress.Utils.Menu.DXMenuItem bienBanHoiChan = new DevExpress.Utils.Menu.DXMenuItem(
                    Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__PRINT_DEBATE",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang));
                bienBanHoiChan.Tag = ModulePrintType.BIEN_BAN_HOI_CHAN;
                bienBanHoiChan.Click += onClickDebatePrint;
                menu.Items.Add(bienBanHoiChan);

                DevExpress.Utils.Menu.DXMenuItem soBienBanHoiChanDauSao = new DevExpress.Utils.Menu.DXMenuItem(
                    Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__PRINT_BIEN_BAN_HOI_CHAN_THUOC_DAU_SAO",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang));
                soBienBanHoiChanDauSao.Tag = ModulePrintType.BIEN_BAN_HOI_CHAN_THUOC_DAU_SAO;
                soBienBanHoiChanDauSao.Click += onClickDebatePrint;
                menu.Items.Add(soBienBanHoiChanDauSao);

                DevExpress.Utils.Menu.DXMenuItem soBienBanHoiChan = new DevExpress.Utils.Menu.DXMenuItem(
                    Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__PRINT_SO_HOI_CHAN",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang));
                soBienBanHoiChan.Tag = ModulePrintType.SO_BIEN_BAN_HOI_CHAN;
                soBienBanHoiChan.Click += onClickDebatePrint;
                menu.Items.Add(soBienBanHoiChan);

                DevExpress.Utils.Menu.DXMenuItem hoiChanPttt = new DevExpress.Utils.Menu.DXMenuItem("Biên bản hội chẩn trước phẫu thuật");
                hoiChanPttt.Tag = ModulePrintType.HOI_CHAN_PTTT;
                hoiChanPttt.Click += onClickDebatePrint;
                menu.Items.Add(hoiChanPttt);

                btnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickDebatePrint(object sender, EventArgs e)
        {
            try
            {
                isCreateEmrDocument = false;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);

                var btnIn = sender as DevExpress.Utils.Menu.DXMenuItem;
                ModulePrintType printType = (ModulePrintType)btnIn.Tag;
                switch (printType)
                {
                    case ModulePrintType.BIEN_BAN_HOI_CHAN:
                        richEditorMain.RunPrintTemplate(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__TRICH_BIEN_BAN_HOI_CHAN__MPS000019, DelegateRunPrinter);
                        break;
                    case ModulePrintType.BIEN_BAN_HOI_CHAN_THUOC_DAU_SAO:
                        richEditorMain.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__BIEN_BAN_HOI_CHAN_THUOC_DAU_SAO__MPS000323, DelegateRunPrinter);
                        break;
                    case ModulePrintType.SO_BIEN_BAN_HOI_CHAN:
                        richEditorMain.RunPrintTemplate(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__SO_BIEN_BAN_HOI_CHAN__MPS000020, DelegateRunPrinter);
                        break;
                    case ModulePrintType.HOI_CHAN_PTTT:
                        richEditorMain.RunPrintTemplate("Mps000387", DelegateRunPrinter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__TRICH_BIEN_BAN_HOI_CHAN__MPS000019:
                        InTrichBienBanHoiChanProcess(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCode.PRINT_TYPE_CODE__BIEN_BAN_HOI_CHAN_THUOC_DAU_SAO__MPS000323:
                        InTrichBienBanHoiChanThuocDauSaoProcess(printTypeCode, fileName, ref result);
                        break;
                    case HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__SO_BIEN_BAN_HOI_CHAN__MPS000020:
                        InSoBienBanHoiChanProcess(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000387":
                        InHoiChanPtttProcess(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        #region Print Process
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

        private bool isCreateEmrDocument = false;

        private void InTrichBienBanHoiChanProcess(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();//Lấy thông tin bệnh nhân
                var patient = PrintGlobalStore.getPatient(treatment_id);

                // get V_HIS_DEBATE
                MOS.Filter.HisDebateViewFilter dbFilter = new MOS.Filter.HisDebateViewFilter();
                dbFilter.ID = currentHisDebate.ID;
                CommonParam param = new CommonParam();
                var v_his_debate = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_DEBATE>>("api/HisDebate/GetView", ApiConsumer.ApiConsumers.MosConsumer, dbFilter, param).FirstOrDefault();
                
                //Lấy dữ liệu ra viện
                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(treatment_id);

                //Tên buồng
                string bedRoomName = this.WorkPlaceSDO.RoomName;
                //Tên khoa
                string departmentName = this.WorkPlaceSDO.DepartmentName;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisDebate), currentHisDebate));
                
                
                //chủ tọa, thu ký
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = null;
                if (currentHisDebate.HIS_DEBATE_USER == null || currentHisDebate.HIS_DEBATE_USER.Count == 0)
                {
                    if (hisDebate != null && hisDebate.ID > 0)
                    {
                        MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                        hisDebateUserFilter.DEBATE_ID = hisDebate.ID;
                        hisDebateUserFilter.ORDER_DIRECTION = "ASC";
                        hisDebateUserFilter.ORDER_FIELD = "ID";
                        lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                        currentHisDebate.HIS_DEBATE_USER = lstHisDebateUser;
                    }
                    else
                    {
                        currentHisDebate.HIS_DEBATE_USER = new List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>();
                    }
                }
                else
                {
                    lstHisDebateUser = currentHisDebate.HIS_DEBATE_USER.ToList();
                }

                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_ID = currentHisDebate.TREATMENT_ID;
                var _TreatmentBedRoom = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bedFilter, null);

                MPS.Processor.Mps000019.PDO.Mps000019PDO.Mps000019SingleKey single = new MPS.Processor.Mps000019.PDO.Mps000019PDO.Mps000019SingleKey();
                single.bebRoomName = bedRoomName;
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
                if (vHisTreatment != null)
                {
                    single.IN_CODE = vHisTreatment.IN_CODE;
                }
                WaitingManager.Hide();

                if (lstHisDebateUser == null)
                    lstHisDebateUser = new List<HIS_DEBATE_USER>();

                MPS.Processor.Mps000019.PDO.Mps000019PDO Mps000019PDO = new MPS.Processor.Mps000019.PDO.Mps000019PDO(
                    patient,
                   v_his_debate,
                   departmentTran,
                   single,
                   vHisTreatment,
                   lstHisDebateUser);
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                string printerName = "";

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                if (isCreateEmrDocument && lciAutoCreateEmr.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoCreateEmr.Checked)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000019PDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrCreateDocument, printerName);
                }
                else if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000019PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000019PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.vHisTreatment != null ? this.vHisTreatment.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);
                if (lciAutoSign.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoSign.Checked)
                {
                    List<SignerConfigDTO> signerList = ProcessSigner(lstHisDebateUser);
                    inputADO.SignerConfigs = signerList;
                }
                PrintData.EmrInputADO = inputADO;

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("MPS.Processor.Mps000019.PDO.Mps000019PDO inputADO: ", inputADO));

                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InTrichBienBanHoiChanThuocDauSaoProcess(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();//Lấy thông tin bệnh nhân
                var patient = PrintGlobalStore.getPatient(treatment_id);

                //Lấy dữ liệu ra viện
                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(treatment_id);

                //Tên buồng
                string bedRoomName = this.WorkPlaceSDO.RoomName;
                //Tên khoa
                string departmentName = this.WorkPlaceSDO.DepartmentName;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisDebate), currentHisDebate));
                //chủ tọa, thu ký
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = null;
                if (currentHisDebate.HIS_DEBATE_USER == null || currentHisDebate.HIS_DEBATE_USER.Count == 0)
                {
                    if (hisDebate != null && hisDebate.ID > 0)
                    {
                        MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                        hisDebateUserFilter.DEBATE_ID = hisDebate.ID;
                        hisDebateUserFilter.ORDER_DIRECTION = "ASC";
                        hisDebateUserFilter.ORDER_FIELD = "ID";
                        lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                        currentHisDebate.HIS_DEBATE_USER = lstHisDebateUser;
                    }
                    else
                    {
                        currentHisDebate.HIS_DEBATE_USER = new List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>();
                    }
                }
                else
                {
                    lstHisDebateUser = currentHisDebate.HIS_DEBATE_USER.ToList();
                }

                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_ID = currentHisDebate.TREATMENT_ID;
                var _TreatmentBedRoom = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bedFilter, null);
                //foreach (var mp in this.medicinePrints)
                //{
                MPS.Processor.Mps000323.PDO.Mps000323PDO.Mps000323SingleKey single = new MPS.Processor.Mps000323.PDO.Mps000323PDO.Mps000323SingleKey();
                single.bebRoomName = bedRoomName;
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
                   vHisTreatment);
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

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.vHisTreatment != null ? this.vHisTreatment.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);
                if (lciAutoSign.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoSign.Checked)
                {
                    List<SignerConfigDTO> signerList = ProcessSigner(lstHisDebateUser);
                    inputADO.SignerConfigs = signerList;
                }

                PrintData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(PrintData);
                //}
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InSoBienBanHoiChanProcess(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();//Lấy thông tin bệnh nhân
                var patient = PrintGlobalStore.getPatient(treatment_id);

                //Lấy dữ liệu ra viện
                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(treatment_id);

                //Tên buồng
                string bedRoomName = WorkPlaceSDO.RoomName;
                //Tên khoa
                string departmentName = WorkPlaceSDO.DepartmentName;

                string currentDateSeparateFullTime = HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentDateSeparateFullTime();

                //Thông tin bảo hiểm y tế
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var patyAlterBhyt = new MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment_id, instructionTime, ref patyAlterBhyt);

                //chủ tọa, thu ký
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = null;
                if (currentHisDebate.HIS_DEBATE_USER == null || currentHisDebate.HIS_DEBATE_USER.Count == 0)
                {
                    if (hisDebate != null && hisDebate.ID > 0)
                    {
                        MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                        hisDebateUserFilter.DEBATE_ID = hisDebate.ID;
                        hisDebateUserFilter.ORDER_DIRECTION = "ASC";
                        hisDebateUserFilter.ORDER_FIELD = "ID";
                        lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                        currentHisDebate.HIS_DEBATE_USER = lstHisDebateUser;
                    }
                    else
                    {
                        currentHisDebate.HIS_DEBATE_USER = new List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>();
                    }
                }
                else
                {
                    lstHisDebateUser = currentHisDebate.HIS_DEBATE_USER.ToList();
                }

                MOS.Filter.HisDebateViewFilter dbFilter = new MOS.Filter.HisDebateViewFilter();
                dbFilter.ID = currentHisDebate.ID;
                var DebateView = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEBATE>>("api/HisDebate/GetView", ApiConsumer.ApiConsumers.MosConsumer, dbFilter, null);


                WaitingManager.Hide();

                MPS.Processor.Mps000020.PDO.Mps000020PDO Mps000019PDO = new MPS.Processor.Mps000020.PDO.Mps000020PDO(
                    patient,
                   bedRoomName,
                   departmentName,
                   currentHisDebate,
                   departmentTran,
                   currentDateSeparateFullTime,
                   patyAlterBhyt,
                   vHisTreatment, 
                   DebateView[0]);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                string printerName = "";

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (isCreateEmrDocument && lciAutoCreateEmr.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoCreateEmr.Checked)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000019PDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrCreateDocument, printerName);
                }
                else if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000019PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000019PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }
             
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.vHisTreatment != null ? this.vHisTreatment.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);
                if (lciAutoSign.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoSign.Checked)
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
                V_HIS_DEBATE debatePrint = new V_HIS_DEBATE();
                MOS.Filter.HisDebateViewFilter dbFilter = new MOS.Filter.HisDebateViewFilter();
                dbFilter.ID = currentHisDebate.ID;
                CommonParam param = new CommonParam();
                List<V_HIS_DEBATE> lstdebate = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_DEBATE>>("api/HisDebate/GetView", ApiConsumer.ApiConsumers.MosConsumer, dbFilter, param);
                if (lstdebate != null && lstdebate.Count > 0)
                {
                    debatePrint = lstdebate.FirstOrDefault();
                }

                //Lấy dữ liệu ra viện
                V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(treatment_id);

                //chủ tọa, thu ký
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = new List<HIS_DEBATE_USER>();
                if (currentHisDebate.HIS_DEBATE_USER == null || currentHisDebate.HIS_DEBATE_USER.Count == 0)
                {
                    if (debatePrint != null && debatePrint.ID > 0)
                    {
                        MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                        hisDebateUserFilter.DEBATE_ID = debatePrint.ID;
                        lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                    }
                }
                else
                {
                    lstHisDebateUser = currentHisDebate.HIS_DEBATE_USER.ToList();
                }

                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_ID = debatePrint.TREATMENT_ID;
                var _TreatmentBedRoom = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bedFilter, null);

                MPS.Processor.Mps000387.PDO.Mps000387PDO.Mps000387SingleKey single = new MPS.Processor.Mps000387.PDO.Mps000387PDO.Mps000387SingleKey();
                if (WorkPlaceSDO != null)
                {
                    single.BEB_ROOM_NAME = WorkPlaceSDO.RoomName;
                    single.DEPARTMENT_NAME = WorkPlaceSDO.DepartmentName;
                }

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
                hisDebateEkipUserFilter.DEBATE_ID = debatePrint.ID;
                lstHisDebatEkipUser = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_DEBATE_EKIP_USER>>("api/HisDebateEkipUser/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisDebateEkipUserFilter, null);

                //Thông tin bảo hiểm y tế
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var patyAlterBhyt = new MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment_id, instructionTime, ref patyAlterBhyt);

                WaitingManager.Hide();
                MPS.Processor.Mps000387.PDO.Mps000387PDO Mps000387PDO = new MPS.Processor.Mps000387.PDO.Mps000387PDO(
                    vHisTreatment,
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

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.vHisTreatment != null ? this.vHisTreatment.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);
                if (lciAutoSign.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoSign.Checked)
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
        #endregion
    }
}
