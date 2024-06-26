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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.PrintPatientUpdate.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.Processor.Mps000001.PDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintPatientUpdate
{
    public class PrintPatientUpdateProcessor
    {
        List<HIS_TREATMENT> _ListTreatment { get; set; }
        V_HIS_PATIENT _PatientPrint { get; set; }
        V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter { get; set; }
        long roomId = 0;

        public PrintPatientUpdateProcessor(object data, long _roomId)
        {
            try
            {
                if (data is List<HIS_TREATMENT>)
                {
                    _ListTreatment = (List<HIS_TREATMENT>)data;
                }
                this.roomId = _roomId;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham In
        /// </summary>
        /// <param name="PrintTypeCode">mã in</param>
        public void Print()
        {
            try
            {
                string key = ConfigApplicationWorker.Get<string>("CONFIG_KEY__HIS_PLUGINS_PATIENT_UPDATE__IS_BARCODE_IS_EXAM_SERVICE_REQ");

                if (!string.IsNullOrEmpty(key) && this._ListTreatment != null && this._ListTreatment.Count > 0)
                {
                    LoadData();
                    LoadConfigHisAcc();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadConfigHisAcc()
        {
            try
            {
                // string key = ConfigApplicationWorker.Get<string>("CONFIG_KEY__HIS_PLUGINS_PATIENT_UPDATE__IS_BARCODE_IS_EXAM_SERVICE_REQ");

                CommonParam param = new CommonParam();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SDA.Filter.SdaConfigAppFilter configAppFilter = new SDA.Filter.SdaConfigAppFilter();
                configAppFilter.APP_CODE_ACCEPT = GlobalVariables.APPLICATION_CODE;
                configAppFilter.KEY = "CONFIG_KEY__HIS_PLUGINS_PATIENT_UPDATE__IS_BARCODE_IS_EXAM_SERVICE_REQ";

                var _currentConfigApp = new BackendAdapter(param).Get<List<SDA_CONFIG_APP>>("api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, configAppFilter, param).FirstOrDefault();

                string key = "";
                if (_currentConfigApp != null)
                {
                    key = _currentConfigApp.DEFAULT_VALUE;
                    SDA.Filter.SdaConfigAppUserFilter appUserFilter = new SDA.Filter.SdaConfigAppUserFilter();
                    appUserFilter.LOGINNAME = loginName;
                    appUserFilter.CONFIG_APP_ID = _currentConfigApp.ID;
                  var  currentConfigAppUser = new BackendAdapter(param).Get<List<SDA_CONFIG_APP_USER>>("api/SdaConfigAppUser/Get", ApiConsumers.SdaConsumer, appUserFilter, param).FirstOrDefault();
                    if (currentConfigAppUser != null)
                    {
                        key = currentConfigAppUser.VALUE;
                    }
                }

                if (!string.IsNullOrEmpty(key))
                {
                   var _ConfigADO = (ConfigADO)Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigADO>(key);
                    if (_ConfigADO != null)
                    {
                        Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                        if (_ConfigADO.IsPrintBarcode == "1")
                        {
                            richEditorMain.RunPrintTemplate("Mps000178", DelegateRunPrinter);
                        }
                        if (_ConfigADO.IsPrintExamServiceReq == "1")
                        {
                            richEditorMain.RunPrintTemplate("Mps000001", DelegateRunPrinter);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                CommonParam param = new CommonParam();

                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = _ListTreatment[0].PATIENT_ID;
                _PatientPrint = new BackendAdapter(param)
                          .Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();


                currentHispatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                if (instructionTime < _ListTreatment[0].IN_TIME)
                {
                    instructionTime = _ListTreatment[0].IN_TIME;
                }
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = _ListTreatment[0].ID;
                if (instructionTime > 0)
                    filter.InstructionTime = instructionTime;
                else
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                currentHispatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);

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
                    case "Mps000178":
                        LoadBieuMauTheBenhNhan(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000001":
                        LoadBieuMauYCKham(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauTheBenhNhan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                //   V_HIS_PATIENT _PatientPrint = new V_HIS_PATIENT();

                V_HIS_TREATMENT_4 treatment4 = new V_HIS_TREATMENT_4();

                CommonParam param = new CommonParam();

                HisTreatmentView4Filter tFilter = new HisTreatmentView4Filter();
                tFilter.ID = this._ListTreatment[0].ID;
                tFilter.ORDER_DIRECTION = "DESC";
                tFilter.ORDER_FIELD = "MODIFY_TIME";
                treatment4 = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumers.MosConsumer, tFilter, null).FirstOrDefault();


                WaitingManager.Hide();

                MPS.Processor.Mps000178.PDO.Mps000178PDO mps000178RDO = new MPS.Processor.Mps000178.PDO.Mps000178PDO(
                   _PatientPrint,
                   currentHispatientTypeAlter,
                   treatment4
                   );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment4 != null ? treatment4.TREATMENT_CODE : ""), printTypeCode, this.roomId);
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

        private void LoadBieuMauYCKham(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                List<V_HIS_SERVICE_REQ> _ServiceReqs = new List<V_HIS_SERVICE_REQ>();

                //  V_HIS_PATIENT _PatientPrint = new V_HIS_PATIENT();

                HIS_TREATMENT treatmentPrint = new HIS_TREATMENT();

                CommonParam param = new CommonParam();

                List<long> _treatmentIds = new List<long>();
                treatmentPrint = _ListTreatment.FirstOrDefault();
                _treatmentIds = _ListTreatment.Select(p => p.ID).Distinct().ToList();

                MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                serviceReqFilter.TREATMENT_IDs = _treatmentIds;
                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                _ServiceReqs = new BackendAdapter(new CommonParam())
                          .Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, serviceReqFilter, null);



                string strHeinRatio = "";
                if (currentHispatientTypeAlter != null)
                    strHeinRatio = this.GetDefaultHeinRatioForView(currentHispatientTypeAlter.HEIN_CARD_NUMBER, currentHispatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT, currentHispatientTypeAlter.RIGHT_ROUTE_CODE);

                List<V_HIS_SERE_SERV> _SereServs = new List<V_HIS_SERE_SERV>();

                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    MOS.Filter.HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.SERVICE_REQ_IDs = _ServiceReqs.Select(p => p.ID).Distinct().ToList();
                    _SereServs = new BackendAdapter(new CommonParam())
                              .Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, ssFilter, null);
                }


                foreach (var serviceReq in _ServiceReqs)
                {
                    var sereservs = _SereServs.Where(o => o.SERVICE_REQ_ID == serviceReq.ID).ToList();

                    List<Mps000001_ListSereServs> mps000001_ListSereServs = (from m in sereservs select new Mps000001_ListSereServs(m)).ToList();
                    MPS.Processor.Mps000001.PDO.Mps000001ADO mps000001ADO = GenerateMps000001ADO(serviceReq, strHeinRatio, treatmentPrint);

                    List<long> _ssIds = sereservs.Select(p => p.SERVICE_ID).Distinct().ToList();
                    var dataSS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ssIds.Contains(p.ID)).ToList();
                    if (dataSS != null && dataSS.Count > 0)
                    {
                        var _service = dataSS.FirstOrDefault(p => p.PARENT_ID != null);
                        if (_service != null)
                        {
                            var serviceN = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(p => p.ID == _service.PARENT_ID.Value);
                            mps000001ADO.PARENT_NAME = serviceN != null ? serviceN.SERVICE_NAME : "";
                        }

                    }

                    var room = this.GetRoomById(serviceReq.EXECUTE_ROOM_ID);
                    if (room != null)
                    {
                        serviceReq.EXECUTE_ROOM_CODE = room.ROOM_CODE;
                        serviceReq.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                        serviceReq.EXECUTE_DEPARTMENT_CODE = room.DEPARTMENT_CODE;
                        serviceReq.EXECUTE_DEPARTMENT_NAME = room.DEPARTMENT_NAME;

                        mps000001ADO.ExecuteRoom = room;
                    }

                    HIS_DHST _HIS_DHST = new HIS_DHST();

                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.TREATMENT_ID = treatmentPrint.ID;
                    dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                    dhstFilter.ORDER_DIRECTION = "DESC";
                    _HIS_DHST = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();

                    HIS_WORK_PLACE _WORK_PLACE = new HIS_WORK_PLACE();
                    if (_PatientPrint.WORK_PLACE_ID != null)
                    {
                        _WORK_PLACE = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().FirstOrDefault(p => p.ID == _PatientPrint.WORK_PLACE_ID);
                    }

                    WaitingManager.Hide();

                    MPS.Processor.Mps000001.PDO.Mps000001PDO mps000001PDO = new MPS.Processor.Mps000001.PDO.Mps000001PDO(
                        serviceReq,
                        currentHispatientTypeAlter,
                        _PatientPrint,
                        mps000001_ListSereServs,
                        treatmentPrint,
                        mps000001ADO,
                        _HIS_DHST,
                        _WORK_PLACE);

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatmentPrint != null ? treatmentPrint.TREATMENT_CODE : ""), printTypeCode, this.roomId);
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000001PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000001PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
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

        private V_HIS_ROOM GetRoomById(long id)
        {
            V_HIS_ROOM result = null;
            try
            {
                result = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new V_HIS_ROOM());
        }

        private MPS.Processor.Mps000001.PDO.Mps000001ADO GenerateMps000001ADO(V_HIS_SERVICE_REQ serviceReq, string strHeinRatio, HIS_TREATMENT _treatment)
        {
            MPS.Processor.Mps000001.PDO.Mps000001ADO mps000001ADO = new MPS.Processor.Mps000001.PDO.Mps000001ADO();
            try
            {
                mps000001ADO.ratio_text = strHeinRatio;
                mps000001ADO.firstExamRoomName = serviceReq.EXECUTE_ROOM_NAME;
                if ((_treatment.IN_ROOM_ID ?? 0) > 0)
                {
                    var room = GetRoomById((_treatment.IN_ROOM_ID ?? 0)) ?? new V_HIS_ROOM();
                    mps000001ADO.IN_ROOM_NAME = room.ROOM_NAME;
                    mps000001ADO.IN_DEPARTMENT_NAME = room.DEPARTMENT_NAME;
                }
                if ((_treatment.TRAN_PATI_REASON_ID ?? 0) > 0)
                    mps000001ADO.TRANSFER_IN_REASON_NAME = (GetTranPatiReasonById((_treatment.TRAN_PATI_REASON_ID ?? 0)) ?? new HIS_TRAN_PATI_REASON()).TRAN_PATI_REASON_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return mps000001ADO;
        }

        private HIS_TRAN_PATI_REASON GetTranPatiReasonById(long id)
        {
            HIS_TRAN_PATI_REASON result = null;
            try
            {
                result = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new HIS_TRAN_PATI_REASON());
        }
    }
}
