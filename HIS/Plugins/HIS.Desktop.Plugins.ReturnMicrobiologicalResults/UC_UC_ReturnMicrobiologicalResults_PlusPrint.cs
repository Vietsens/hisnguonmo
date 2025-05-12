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
using AutoMapper;
using Bartender.PrintClient;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ADO;
using HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults
{
    public partial class UC_ReturnMicrobiologicalResults : UserControlBase
    {
        public enum PRINT_OPTION
        {
            IN,
            IN_TACH_THEO_NHOM
        }

        #region Print Barcode

        private void onClickBtnPrintBarCode()
        {
            try
            {
                if (LisConfigCFG.PRINT_BARCODE_BY_BARTENDER == "1")
                {
                    this.PrintBarcodeByBartender();
                }
                else
                {
                    PrintType type = new PrintType();
                    type = PrintType.IN_BARCODE;
                    PrintProcess(type);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal enum PrintType
        {
            IN_BARCODE,
        }

        private void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_BARCODE:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000077.PDO.Mps000077PDO.PrintTypeCode.Mps000077, DelegateRunPrinter);
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

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000077.PDO.Mps000077PDO.PrintTypeCode.Mps000077:
                        LoadBieuMauPhieuYCInBarCode(printTypeCode, fileName, ref result);
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

        internal void LoadBieuMauPhieuYCInBarCode(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                rowSample2 = null;
                rowSample2 = (V_LIS_SAMPLE_2)gridViewSample.GetFocusedRow();
                AutoMapper.Mapper.CreateMap<V_LIS_SAMPLE_2, V_LIS_SAMPLE>();
                rowSample = AutoMapper.Mapper.Map<V_LIS_SAMPLE>(rowSample2);
                if (rowSample == null)
                    return;
                //bool refresh = false;
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        WaitingManager.Hide();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowSample);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                        WaitingManager.Show();
                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = rowSample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            rowSample.SAMPLE_ORDER = curentSTT.SAMPLE_ORDER;
                            FillDataToGridControl();
                            gridViewSample.RefreshData();
                        }
                    }
                }

                MOS.Filter.HisServiceReqViewFilter ServiceReqViewFilter = new HisServiceReqViewFilter();
                ServiceReqViewFilter.SERVICE_REQ_CODE = rowSample.SERVICE_REQ_CODE;
                var rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, ServiceReqViewFilter, param).FirstOrDefault();

                LisSampleServiceViewFilter samServiceFilter = new LisSampleServiceViewFilter();
                samServiceFilter.SAMPLE_ID = rowSample.ID;
                var rsSampleService = new BackendAdapter(param).Get<List<V_LIS_SAMPLE_SERVICE>>("api/LisSampleService/GetView", ApiConsumers.LisConsumer, samServiceFilter, param);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((rs != null ? rs.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                MPS.Processor.Mps000077.PDO.Mps000077PDO mps000077RDO = new MPS.Processor.Mps000077.PDO.Mps000077PDO(
                           rowSample,
                           rs,
                           rsSampleService
                           );
                WaitingManager.Hide();

                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
                FillDataToGridControl();
                gridViewSample.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void PrintBarcodeByBartender()
        {
            try
            {
                rowSample2 = null;
                rowSample2 = (V_LIS_SAMPLE_2)gridViewSample.GetFocusedRow();
                AutoMapper.Mapper.CreateMap<V_LIS_SAMPLE_2, V_LIS_SAMPLE>();
                rowSample = AutoMapper.Mapper.Map<V_LIS_SAMPLE>(rowSample2);
                if (rowSample == null)
                    return;
                WaitingManager.Show();
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        WaitingManager.Hide();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowSample);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                        WaitingManager.Show();
                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        CommonParam param = new CommonParam();
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = rowSample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            rowSample.SAMPLE_ORDER = curentSTT.SAMPLE_ORDER;
                            FillDataToGridControl();
                            gridViewSample.RefreshData();
                        }
                    }
                }
                if (StartAppPrintBartenderProcessor.OpenAppPrintBartender())
                {

                    ClientPrintADO ado = new ClientPrintADO();
                    ado.Barcode = rowSample.BARCODE;
                    if (rowSample.SAMPLE_TYPE_CODE != null)
                    {
                        ado.SampleTypeCode = rowSample.SAMPLE_TYPE_CODE;
                        ado.SampleTypeName = rowSample.SAMPLE_TYPE_NAME;
                    }
                    else
                    {
                        LIS.Filter.LisSampleServiceFilter dataLisSample = new LisSampleServiceFilter();
                        dataLisSample.SAMPLE_ID = rowSample.ID;
                        CommonParam param = new CommonParam();
                        List<LIS_SAMPLE_SERVICE> listSampleService = new BackendAdapter(param).Get<List<LIS_SAMPLE_SERVICE>>("api/LisSampleService/get", ApiConsumers.LisConsumer, dataLisSample, param);

                        if (listSampleService != null && listSampleService.Count > 0)
                        {
                            List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>();
                            var matchedHisServices = services
                            .Where(h => listSampleService.Any(s => s.SERVICE_CODE == h.SERVICE_CODE))
                            .ToList();

                            string stringFilter = string.Join(",", matchedHisServices
                            .Select(h => h.SAMPLE_TYPE_CODE)
                            .Where(code => !string.IsNullOrWhiteSpace(code))
                            .Distinct());

                            ado.SampleTypeCode = stringFilter;
                        }


                    }
                    if (rowSample.DOB.HasValue)
                    {
                        ado.DobYear = rowSample.DOB.Value.ToString().Substring(0, 4);
                        ado.DobAge = MPS.AgeUtil.CalculateFullAge(rowSample.DOB.Value);
                    }
                    ado.ExecuteRoomCode = rowSample.EXECUTE_ROOM_CODE;
                    ado.ExecuteRoomName = rowSample.EXECUTE_ROOM_NAME ?? "";
                    ado.ExecuteRoomName_Unsign = Inventec.Common.String.Convert.UnSignVNese(rowSample.EXECUTE_ROOM_NAME ?? "");
                    ado.GenderName = (!String.IsNullOrWhiteSpace(rowSample.GENDER_CODE)) ? (rowSample.GENDER_CODE == "01" ? "Nữ" : "Nam") : "";
                    ado.GenderName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.GenderName);
                    ado.PatientCode = rowSample.PATIENT_CODE ?? "";
                    List<string> name = new List<string>();
                    if (!String.IsNullOrWhiteSpace(rowSample.LAST_NAME))
                    {
                        name.Add(rowSample.LAST_NAME);
                    }
                    if (!String.IsNullOrWhiteSpace(rowSample.FIRST_NAME))
                    {
                        name.Add(rowSample.FIRST_NAME);
                    }
                    ado.PatientName = String.Join(" ", name);
                    ado.PatientName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.PatientName);
                    ado.RequestDepartmentCode = rowSample.REQUEST_DEPARTMENT_CODE ?? "";
                    ado.RequestDepartmentName = rowSample.REQUEST_DEPARTMENT_NAME ?? "";
                    ado.RequestDepartmentName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.RequestDepartmentName);
                    ado.ServiceReqCode = rowSample.SERVICE_REQ_CODE ?? "";
                    ado.TreatmentCode = rowSample.TREATMENT_CODE;
                    BartenderPrintClientManager client = new BartenderPrintClientManager();
                    bool success = client.BartenderPrint(ado);
                    if (!success)
                    {
                        LogSystem.Error("In barcode Bartender that bai. Check log BartenderPrint");
                    }
                }
                else
                {
                    LogSystem.Warn("Khong mo duoc APP Print Bartender");
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Print Result

        internal enum PrintTypeKXN
        {
            IN_KET_QUA_XET_NGHIEM,
        }
        void PrintProcess(PrintTypeKXN printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeKXN.IN_KET_QUA_XET_NGHIEM:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000341.PDO.PrintTypeCode.Mps000341, DelegateRunPrinterKXN);
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

        bool DelegateRunPrinterKXN(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000341.PDO.PrintTypeCode.Mps000341:
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauInKetQuaXetNghiemV2(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                rowSample2 = null;
                rowSample2 = (V_LIS_SAMPLE_2)gridViewSample.GetFocusedRow();
                AutoMapper.Mapper.CreateMap<V_LIS_SAMPLE_2, V_LIS_SAMPLE>();
                rowSample = AutoMapper.Mapper.Map<V_LIS_SAMPLE>(rowSample2);
                if (!ValidTime()) return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LisSampleViewFilter filterSample = new LisSampleViewFilter();
                filterSample.ID = rowSample.ID;
                V_LIS_SAMPLE vLisSample = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumer.ApiConsumers.LisConsumer, filterSample, param).FirstOrDefault();
                List<LisResultADO> lstResultPrint = new List<LisResultADO>();
                if (lstCheckPrint != null && lstCheckPrint.Count > 0)
                {
                    List<string> serviceCodes = lstCheckPrint.Select(o => o.SERVICE_CODE).Distinct().ToList();
                    lstResultPrint = _LisResults.Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();
                }
                WaitingManager.Hide();

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((rowSample != null ? rowSample.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                //LIS_SAMPLE same = new LIS_SAMPLE();
                //Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(same, this.rowSample);
                List<HIS_SERVICE_REQ> apiResult = null;
                CommonParam paramCommon = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.SERVICE_REQ_CODE__EXACT = this.rowSample.SERVICE_REQ_CODE;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                apiResult = new BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                HIS_SERVICE_REQ hisService = new HIS_SERVICE_REQ();
                if (apiResult != null && apiResult.Count > 0)
                {
                    hisService = apiResult.FirstOrDefault();
                }

                List<LIS_PATIENT_CONDITION> lstpatientCondition;
                CommonParam paramCommonCondition = new CommonParam();
                LisPatientConditionFilter conditionFilter = new LisPatientConditionFilter();
                lstpatientCondition = new BackendAdapter(paramCommonCondition).Get<List<LIS_PATIENT_CONDITION>>("api/LisPatientCondition/Get", ApiConsumer.ApiConsumers.LisConsumer, conditionFilter, paramCommonCondition);

                List<HIS_TREATMENT> treatmentApiResult = null;
                CommonParam paramCo = new CommonParam();
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                filter.TREATMENT_CODE__EXACT = this.rowSample.TREATMENT_CODE;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                treatmentApiResult = new BackendAdapter(paramCo).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, paramCo);
                HIS_TREATMENT treatment = new HIS_TREATMENT();
                if (apiResult != null && apiResult.Count > 0)
                {
                    treatment = treatmentApiResult.FirstOrDefault();
                }

                HIS_PATIENT patient = null;
                if (treatment != null)
                {
                    HisPatientFilter patiFilter = new HisPatientFilter();
                    patiFilter.ID = treatment.PATIENT_ID;
                    var lstPatient = new BackendAdapter(paramCo).Get<List<HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GET, ApiConsumers.MosConsumer, patiFilter, paramCo);
                    patient = lstPatient != null ? lstPatient.FirstOrDefault() : null;
                }

                if (PrintOption == PRINT_OPTION.IN)
                {
                    LIS_SAMPLE_SERVICE sameService = new LIS_SAMPLE_SERVICE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE_SERVICE>(sameService, this.currentTestSamResultADO);

                    LIS_MACHINE machine = null;
                    List<V_LIS_RESULT> lisResult = new List<V_LIS_RESULT>();

                    foreach (var item in lstResultPrint)
                    {
                        V_LIS_RESULT rs = new V_LIS_RESULT();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_RESULT>(rs, item);
                        lisResult.Add(rs);
                    }

                    MPS.Processor.Mps000341.PDO.Mps000341PDO mps000341RDO = new MPS.Processor.Mps000341.PDO.Mps000341PDO(
                               vLisSample,
                               sameService,
                               machine,
                               lisResult,
                               hisService,
                               treatment,
                               patient,
                               lstpatientCondition);

                    MPS.ProcessorBase.Core.PrintData PrintData = null;

                    if (HisConfigCFG.IS_USE_SIGN_EMR == "1")
                    {
                        LogSystem.Info("IS_USE_SIGN_EMR = 1");
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName);

                    }
                    else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }

                    PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                    PrintData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                else if (PrintOption == PRINT_OPTION.IN_TACH_THEO_NHOM)
                {
                    List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>();
                    if (services == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach dich vu");
                        return;
                    }

                    Dictionary<long, List<TestLisResultADO>> dicServiceTest = new Dictionary<long, List<TestLisResultADO>>();
                    foreach (var item in this.lstSampleServiceADOs)
                    {
                        LIS_SAMPLE_SERVICE sameService = new LIS_SAMPLE_SERVICE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE_SERVICE>(sameService, item);
                        sameService.ID = item.SAMPLE_SERVICE_ID ?? 0;

                        LIS_MACHINE machine = _Machines.Where(o => o.ID == item.MACHINE_ID).FirstOrDefault();
                        List<V_LIS_RESULT> lisResult = new List<V_LIS_RESULT>();
                        if (item.LResultDetails != null && item.LResultDetails.Count > 0)
                            foreach (var item1 in item.LResultDetails)
                            {
                                V_LIS_RESULT rs = new V_LIS_RESULT();
                                Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_RESULT>(rs, item1);
                                lisResult.Add(rs);
                            }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lisResult), lisResult));
                        MPS.Processor.Mps000341.PDO.Mps000341PDO mps000341RDO = new MPS.Processor.Mps000341.PDO.Mps000341PDO(
                                   vLisSample,
                                   sameService,
                                   machine,
                                   lisResult,
                                   hisService,
                                   treatment,
                                   patient,
                                   lstpatientCondition);

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (HisConfigCFG.IS_USE_SIGN_EMR == "1")
                        {
                            LogSystem.Info("IS_USE_SIGN_EMR = 1");
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                                SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                                return;
                            }
                            else if (chkSignProcess.Checked)
                            {
                                bool isPrintEmr = true; //Check trình ký có thiết lập ký gồm loginname hay không
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                                SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrintEmr);
                                if (!isPrintEmr) //Thiết lập ký không gồm loginname thì tạo văn bản ký trong chi tiết bệnh án sau đó in văn bản bình thường
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                                }
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000341RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void SetUpSign(Inventec.Common.SignLibrary.ADO.InputADO inputADO, MPS.ProcessorBase.Core.PrintData PrintData, LIS_SAMPLE curentSTT, ref bool result, ref string errorMessage)
        {
            try
            {
                using (PrintData.saveMemoryStream = new MemoryStream())
                {
                    PrintData.EmrInputADO = inputADO;
                    MPS.MpsPrinter.Run(PrintData);

                    Inventec.Common.SignLibrary.ADO.InputADO signEmrInputADO = (Inventec.Common.SignLibrary.ADO.InputADO)PrintData.EmrInputADO;
                    string base64File = "";
                    try
                    {
                        using (MemoryStream pdfStream = new MemoryStream())
                        {
                            PrintData.saveMemoryStream.Position = 0;
                            ConvertExcelToPdfByFlexCel(PrintData.saveMemoryStream, pdfStream);
                            pdfStream.Position = 0;
                            base64File = System.Convert.ToBase64String(Inventec.Common.SignLibrary.Utils.StreamToByte(pdfStream));
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    signEmrInputADO.IsSign = true;
                    if (this.SignConfigData != null)
                    {
                        if (SignConfigData.listSign != null && SignConfigData.listSign.Count > 0)
                        {
                            signEmrInputADO.IsSignConfig = true;
                            signEmrInputADO.SignerConfigs = new List<Inventec.Common.SignLibrary.DTO.SignerConfigDTO>();
                            bool isMe = false;
                            if (!SignConfigData.listSign.Exists(o => o.Loginname == "%me%"))
                            {
                                Inventec.Common.SignLibrary.DTO.SignerConfigDTO dto = new Inventec.Common.SignLibrary.DTO.SignerConfigDTO();
                                isMe = true;
                                dto.NumOrder = 1;
                                dto.Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                signEmrInputADO.SignerConfigs.Add(dto);
                            }
                            foreach (var item in SignConfigData.listSign)
                            {
                                Inventec.Common.SignLibrary.DTO.SignerConfigDTO dto = new Inventec.Common.SignLibrary.DTO.SignerConfigDTO();
                                dto.NumOrder = isMe ? item.NumOrder + 1 : item.NumOrder;
                                dto.Loginname = item.Loginname;
                                signEmrInputADO.SignerConfigs.Add(dto);
                            }
                        }
                    }

                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                    DocumentSignedResultDTO signNowResult = new DocumentSignedResultDTO();
                    signNowResult = libraryProcessor.SignAndShowPrintPreview(base64File, Inventec.Common.SignLibrary.FileType.Pdf, signEmrInputADO);
                    if (signNowResult != null)
                    {
                        errorMessage = !String.IsNullOrWhiteSpace(signNowResult.Message) ? signNowResult.Message : "Tạo văn bản thất bại";
                        result = !signNowResult.Success;
                        if (!String.IsNullOrWhiteSpace(signNowResult.DocumentCode))
                        {
                            CommonParam paramUpdate = new CommonParam();
                            curentSTT.EMR_RESULT_DOCUMENT_CODE = signNowResult.DocumentCode;
                            var apiresult = new BackendAdapter(paramUpdate).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, curentSTT, paramUpdate);
                            if (apiresult == null)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Lỗi cập nhật thông tin văn bản điện tử của mẫu: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                result = true;
                                errorMessage += " " + paramUpdate.GetMessage();
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Lỗi không có thông tin văn bản điện tử: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                            result = true;
                            errorMessage += "Không có thông tin văn bản điện tử";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetUpSignProcess(Inventec.Common.SignLibrary.ADO.InputADO inputADO, MPS.ProcessorBase.Core.PrintData PrintData, LIS_SAMPLE curentSTT, ref bool result, ref string errorMessage, ref bool isPrintEmr)
        {
            try
            {
                using (PrintData.saveMemoryStream = new MemoryStream())
                {
                    Inventec.Common.Logging.LogSystem.Info("Begin SetUpSignProcess");
                    bool isMe = false;
                    PrintData.EmrInputADO = inputADO;
                    MPS.MpsPrinter.Run(PrintData);
                    Inventec.Common.SignLibrary.ADO.InputADO signEmrInputADO = (Inventec.Common.SignLibrary.ADO.InputADO)PrintData.EmrInputADO;
                    string base64File = "";
                    try
                    {
                        using (MemoryStream pdfStream = new MemoryStream())
                        {
                            PrintData.saveMemoryStream.Position = 0;
                            ConvertExcelToPdfByFlexCel(PrintData.saveMemoryStream, pdfStream);
                            pdfStream.Position = 0;
                            base64File = System.Convert.ToBase64String(Inventec.Common.SignLibrary.Utils.StreamToByte(pdfStream));
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    signEmrInputADO.IsSign = true;

                    if (this.SignConfigData != null)
                    {
                        if (SignConfigData.listSign != null && SignConfigData.listSign.Count > 0)
                        {
                            signEmrInputADO.IsSignConfig = true;
                            signEmrInputADO.SignerConfigs = new List<Inventec.Common.SignLibrary.DTO.SignerConfigDTO>();
                            foreach (var item in SignConfigData.listSign)
                            {
                                Inventec.Common.SignLibrary.DTO.SignerConfigDTO dto = new Inventec.Common.SignLibrary.DTO.SignerConfigDTO();
                                dto.NumOrder = item.NumOrder;
                                if (item.Loginname == "%me%")
                                {
                                    isMe = true;
                                    dto.Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                }
                                else
                                {
                                    dto.Loginname = item.Loginname;
                                }

                                signEmrInputADO.SignerConfigs.Add(dto);
                            }
                        }
                    }
                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                    if (isMe)
                    {
                        isPrintEmr = true;
                        DocumentSignedResultDTO signNowResult = new DocumentSignedResultDTO();
                        signNowResult = libraryProcessor.SignAndShowPrintPreview(base64File, Inventec.Common.SignLibrary.FileType.Pdf, signEmrInputADO);
                        if (signNowResult != null)
                        {
                            errorMessage = !String.IsNullOrWhiteSpace(signNowResult.Message) ? signNowResult.Message : "Tạo văn bản thất bại";
                            result = !signNowResult.Success;
                            if (!String.IsNullOrWhiteSpace(signNowResult.DocumentCode))
                            {
                                CommonParam paramUpdate = new CommonParam();
                                curentSTT.EMR_RESULT_DOCUMENT_CODE = signNowResult.DocumentCode;
                                var apiresult = new BackendAdapter(paramUpdate).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, curentSTT, paramUpdate);
                                if (apiresult == null)
                                {
                                    Inventec.Common.Logging.LogSystem.Error("Lỗi cập nhật thông tin văn bản điện tử của mẫu: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                    result = true;
                                    errorMessage += " " + paramUpdate.GetMessage();
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("Lỗi không có thông tin văn bản điện tử: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                result = true;
                                errorMessage += "Không có thông tin văn bản điện tử";
                            }
                        }
                    }
                    else
                    {
                        isPrintEmr = false;
                        result = libraryProcessor.CreateDocument(signEmrInputADO, base64File);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ConvertExcelToPdfByFlexCel(MemoryStream excelStream, MemoryStream pdfStream)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2.1");
                FlexCel.Render.FlexCelPdfExport flexCelPdfExport1 = new FlexCel.Render.FlexCelPdfExport();

                flexCelPdfExport1.FontEmbed = FlexCel.Pdf.TFontEmbed.Embed;
                flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                flexCelPdfExport1.PageSize = null;
                FlexCel.Pdf.TPdfProperties tPdfProperties1 = new FlexCel.Pdf.TPdfProperties();
                tPdfProperties1.Author = null;
                tPdfProperties1.Creator = null;
                tPdfProperties1.Keywords = null;
                tPdfProperties1.Subject = null;
                tPdfProperties1.Title = null;
                flexCelPdfExport1.Properties = tPdfProperties1;
                flexCelPdfExport1.Workbook = new FlexCel.XlsAdapter.XlsFile();
                flexCelPdfExport1.Workbook.Open(excelStream);
                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2.2");
                int SaveSheet = flexCelPdfExport1.Workbook.ActiveSheet;
                try
                {
                    flexCelPdfExport1.BeginExport(pdfStream);
                    flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                    flexCelPdfExport1.ExportSheet();
                    flexCelPdfExport1.EndExport();
                }
                finally
                {
                    flexCelPdfExport1.Workbook.ActiveSheet = SaveSheet;
                }
                pdfStream.Position = 0;

                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
