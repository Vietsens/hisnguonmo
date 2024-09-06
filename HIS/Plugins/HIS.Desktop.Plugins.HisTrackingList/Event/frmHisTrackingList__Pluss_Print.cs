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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisTrackingList.ADO;
using HIS.Desktop.Plugins.HisTrackingList.Config;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTrackingList.Run
{
    public partial class frmHisTrackingList : HIS.Desktop.Utility.FormBase
    {
        internal enum PrintType
        {
            IN_TO_DIEU_TRI,
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_TO_DIEU_TRI:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_TO_DIEU_TRI__MPS000062, null, DelegateRunPrinter, true);
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
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_TO_DIEU_TRI__MPS000062:
                        Mps000062(printTypeCode, fileName, ref result);
                        //LoadBieuMauInToDieuTriTongHopMps000062(printTypeCode, fileName, ref result);
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

        HIS_TREATMENT _Treatment { get; set; }

        List<V_HIS_TREATMENT_BED_ROOM> _TreatmentBedRooms { get; set; }

        //List<HIS_SERVICE_REQ> _ServiceReqs { get; set; }
        Dictionary<long, HIS_SERVICE_REQ> dicServiceReqs { get; set; }

        List<HIS_SERE_SERV> _SereServs { get; set; }
        Dictionary<long, List<HIS_SERE_SERV>> dicSereServs { get; set; }

        Dictionary<long, List<HIS_SERE_SERV>> dicSerevices { get; set; }

        //List<HIS_EXP_MEST> _ExpMests { get; set; }
        Dictionary<long, HIS_EXP_MEST> dicExpMests { get; set; }

        List<HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        Dictionary<long, List<HIS_EXP_MEST_MEDICINE>> dicExpMestMedicines { get; set; }

        List<HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        Dictionary<long, List<HIS_EXP_MEST_MATERIAL>> dicExpMestMaterials { get; set; }

        Dictionary<long, List<HIS_SERVICE_REQ_METY>> dicServiceReqMetys { get; set; }
        Dictionary<long, List<HIS_SERVICE_REQ_MATY>> dicServiceReqMatys { get; set; }

        internal List<HIS_IMP_MEST> _ImpMests_input { get; set; }
        internal List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedis { get; set; }
        internal List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMates { get; set; }
        internal List<V_HIS_IMP_MEST_BLOOD> _ImpMestBloods { get; set; }

        List<HIS_SERE_SERV_EXT> _SereServExts { get; set; }
        List<V_HIS_SERE_SERV_RATION> _SereServRation { get; set; }

        //thuốc, vật tư trả lại
        private List<V_HIS_IMP_MEST_2> _MobaImpMests;
        private List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines_TL;
        private List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterial_TL;
        private List<V_HIS_IMP_MEST_BLOOD> _ImpMestBlood_TL;

        private List<HIS_CARE> ListCares;
        private List<V_HIS_CARE_DETAIL> ListCareDetails;
        private List<HIS_DHST> ListDhst;
        private List<V_HIS_EXP_MEST_BLTY_REQ_2> ExpMestBltyReq2;

        bool IsNotShowOutMediAndMate = false;

        List<V_HIS_TRACKING> _TrackingPrintsProcesss;

        private void Mps000062(string printTypeCode, string fileName, ref bool result, bool saveFile = false, System.IO.MemoryStream saveFileStream = null, List<V_HIS_TRACKING> _TrackingPrintExts = null)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("Load Stat -------------------------");
                #region ----
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<long> trackingIds = new List<long>();

                _TrackingPrintsProcesss = new List<V_HIS_TRACKING>();

                if (_TrackingPrintExts != null && _TrackingPrintExts.Count > 0)
                    _TrackingPrintsProcesss.AddRange(_TrackingPrintExts);
                else if (_TrackingPrints != null && _TrackingPrints.Count > 0)
                    _TrackingPrintsProcesss.AddRange(_TrackingPrints);

                if (_TrackingPrintsProcesss != null && _TrackingPrintsProcesss.Count > 0)
                    trackingIds = _TrackingPrintsProcesss.Select(p => p.ID).ToList();
                else
                    return;

                //gán lại ngày y lệnh theo thời gian y lệnh do trong mps sửa ngày
                if (dicServiceReqs != null && dicServiceReqs.Count > 0)
                {
                    dicServiceReqs.Values.ToList().ForEach(o => o.INTRUCTION_DATE = o.INTRUCTION_TIME - (o.INTRUCTION_TIME % 1000000));
                }

                List<V_HIS_TREATMENT_BED_ROOM> treatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
                foreach (var item in this._TrackingPrintsProcesss)
                {
                    var treatmentBedRoom = this._TreatmentBedRooms.Where(o => o.ADD_TIME <= item.TRACKING_TIME && o.ROOM_ID == item.ROOM_ID).OrderByDescending(o => o.ADD_TIME).FirstOrDefault();
                    if (treatmentBedRoom != null)
                        treatmentBedRooms.Add(treatmentBedRoom);
                }

                long keyVienTim = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_NUMBER_BY_MEDICINE_TYPE));

                #region keu cấu hiển thị key stt
                long ShowKeySTT = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKINGPRINT_USED_DAY_COUNTING_OPTION));
                long FormatShowKeySTT = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKINGPRINT_USED_DAY_COUNTING_FORMAT_OPTION));
                long UsedDayCountingOutStockOption = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKINGPRINT_USED_DAY_COUNTING_OUT_STOCK_OPTION));
                #endregion

                #endregion

                //#region Dấu hiệu sinh tồn
                //List<HIS_DHST> _Dhsts = new List<HIS_DHST>();
                //if (this.ListDhst != null && this.ListDhst.Count > 0)
                //{
                //    _Dhsts = this.ListDhst.Where(o => trackingIds.Contains(o.TRACKING_ID ?? 0)).ToList();
                //}
                //#endregion

                //#region Danh sách chăm sóc
                //List<HIS_CARE> _Cares = new List<HIS_CARE>();
                //List<V_HIS_CARE_DETAIL> _CareDetails = new List<V_HIS_CARE_DETAIL>();
                //if (this.ListCares != null && this.ListCares.Count > 0)
                //{
                //    _Cares = this.ListCares.Where(o => trackingIds.Contains(o.TRACKING_ID ?? 0)).ToList();
                //    _CareDetails = this.ListCareDetails.Where(o => _Cares.Exists(e => e.ID == o.CARE_ID)).ToList();
                //}
                //#endregion

                #region Thông tin khoa phòng hiện tại
                MOS.SDO.WorkPlaceSDO _workPlaceSDO = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.currentModule.RoomId);
                MPS.Processor.Mps000062.PDO.Mps000062SingleKey singleKey = new MPS.Processor.Mps000062.PDO.Mps000062SingleKey(_workPlaceSDO);
                singleKey.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                singleKey.USER_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                #endregion

                singleKey.IsShowMedicineLine = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.Bordereau.IsShowMedicineLine") == "1");
                //singleKey.IsOrderByType = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.IsOrderByType") == "1");
                singleKey.IsOrderByType = Convert.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.OderOption"));
                singleKey.keyVienTim = keyVienTim;
                singleKey.UsedDayCountingOption = ShowKeySTT;
                singleKey.UsedDayCountingFormatOption = FormatShowKeySTT;
                singleKey.UsedDayCountingOutStockOption = UsedDayCountingOutStockOption;
                #region Danh sách dịch vụ

                #endregion

                #region Dòng thuốc
                List<HIS_MEDICINE_LINE> listMedicineLine = new List<HIS_MEDICINE_LINE>();
                listMedicineLine = BackendDataWorker.Get<HIS_MEDICINE_LINE>();
                #endregion
                #region Dạng bào chế
                List<HIS_DOSAGE_FORM> listDosage = new List<HIS_DOSAGE_FORM>();
                listDosage = BackendDataWorker.Get<HIS_DOSAGE_FORM>();
                #endregion
                #region Mps000062
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);

                long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_PRINT_MERGE));
                if (keyPrintMerge == 1)
                {
                    string uniqueTime = "";// (_TrackingPrints != null && _TrackingPrints.Count > 0) ? _TrackingPrints[0].TRACKING_TIME + "" : "";
                    inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, uniqueTime, (_Treatment != null ? _Treatment.TREATMENT_CODE : ""));//TODO
                }
                if (_TrackingPrintsProcesss != null && _TrackingPrintsProcesss.Count > 0)
                {
                    if (_TrackingPrintsProcesss.Count == 1)
                    {
                        inputADO.DocumentName += " " + "(" + _TrackingPrintsProcesss[0].SHEET_ORDER.ToString() + ")";
                    }
                    else
                    {
                        inputADO.DocumentName += " " + "(" + string.Join(", ", _TrackingPrintsProcesss.Distinct().Select(o => o.SHEET_ORDER).ToList()) + ")";
                    }
                }

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.MergeCode), inputADO.MergeCode));

                //List<V_HIS_EXP_MEST_BLTY_REQ_2> ExpMestBltyReq2 = new List<V_HIS_EXP_MEST_BLTY_REQ_2>();
                //if (HisConfigCFG.Config_TrackingCreate_BloodPresOption != null
                //    && HisConfigCFG.Config_TrackingCreate_BloodPresOption.Trim() == "1")
                //{
                //    MOS.Filter.HisExpMestBltyReqView2Filter expMestBltyReqView2Filter = new HisExpMestBltyReqView2Filter();
                //    expMestBltyReqView2Filter.TRACKING_IDs = trackingIds;
                //    ExpMestBltyReq2 = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ_2>>("api/HisExpMestBltyReq/GetView2", ApiConsumers.MosConsumer, expMestBltyReqView2Filter, new CommonParam());
                //}

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ExpMestBltyReq2), ExpMestBltyReq2));

                var PatientTypeAlter = new BackendAdapter(new CommonParam()).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, null);


                MPS.Processor.Mps000062.PDO.Mps000062PDO mps000062RDO = new MPS.Processor.Mps000062.PDO.Mps000062PDO(
                _Treatment,
                treatmentBedRooms,
                _TrackingPrintsProcesss,
                this.ListDhst,
                dicServiceReqs,
                dicSereServs,
                dicExpMests,
                dicExpMestMedicines,
                dicExpMestMaterials,
                dicServiceReqMetys,
                dicServiceReqMatys,
                this.ListCares,
                this.ListCareDetails,
                singleKey,
                BackendDataWorker.Get<HIS_ICD>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                BackendDataWorker.Get<HIS_SERVICE_TYPE>(),
                this._SereServExts,
                BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>(),
                this._SereServRation,
                this._MobaImpMests,
                this._ImpMestMedicines_TL,
                this._ImpMestMaterial_TL,
                this.ExpMestBltyReq2,
                BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => _SereServs.Select(p => p.SERVICE_ID).Contains(o.ID)).ToList(),
                this._ImpMestBlood_TL,
                PatientTypeAlter,
                listMedicineLine,
                listDosage
                );
                Inventec.Common.Logging.LogSystem.Debug("KT ------------Truyen data MPS======-------------");
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                string ext = Path.GetExtension(fileName);
                if (ext == ".doc" || ext == ".docx")
                {
                    this.saveFilePath = GenerateTempFileWithin("", ".docx");
                }

                if (saveFile)
                {
                    if (ext == ".doc" || ext == ".docx")
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000062RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, this.saveFilePath);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000062RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, saveFileStream);
                    }
                     
                }
                else if (chkSign.Checked)
                {
                    if (chkPrintDocumentSigned.Checked)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000062RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, "", 1, this.saveFilePath) { EmrInputADO = inputADO };

                    }
                    else
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000062RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "", 1, this.saveFilePath) { EmrInputADO = inputADO };
                }
                else
                {
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000062RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "", 1, this.saveFilePath) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000062RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "", 1, this.saveFilePath) { EmrInputADO = inputADO };
                    }
                }
                result = MPS.MpsPrinter.Run(PrintData);

                if (saveFile)
                {
                    this.saveFilePath = PrintData.saveFilePath;
                    
                    if (PrintData.saveMemoryStream != null)
                    {
                        PrintData.saveMemoryStream.Position = 0;
                        PrintData.saveMemoryStream.CopyTo(saveFileStream);
                    }
                }

                dicServiceReqs = new Dictionary<long, HIS_SERVICE_REQ>();
                MOS.Filter.HisServiceReqFilter serviceReqFilterVT = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilterVT.TREATMENT_ID = treatmentId;
                var _ServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilterVT, param);
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    foreach (var item in _ServiceReqs)
                    {
                        if (!dicServiceReqs.ContainsKey(item.ID))
                        {
                            dicServiceReqs[item.ID] = new HIS_SERVICE_REQ();
                            dicServiceReqs[item.ID] = item;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataCare(object obj)
        {
            try
            {
                MOS.Filter.HisCareFilter careFilter = new HisCareFilter();
                careFilter.TREATMENT_ID = (long)obj;
                CommonParam param = new CommonParam();
                this.ListCares = new BackendAdapter(param).Get<List<HIS_CARE>>(HisRequestUriStore.HIS_CARE_GET, ApiConsumers.MosConsumer, careFilter, param);
                if (this.ListCares != null && this.ListCares.Count > 0)
                {
                    foreach (var item in this.ListCares)
                    {
                        MOS.Filter.HisCareDetailViewFilter careDetailFilter = new HisCareDetailViewFilter();
                        careDetailFilter.CARE_ID = item.ID;
                        var careDetail = new BackendAdapter(param).Get<List<V_HIS_CARE_DETAIL>>(HisRequestUriStore.HIS_CARE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, careDetailFilter, param);
                        if (careDetail != null && careDetail.Count > 0)
                        {
                            this.ListCareDetails.AddRange(careDetail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadOther(object obj)
        {
            try
            {
                MOS.Filter.HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_ID = (long)obj;
                this.ListDhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);

                HisSereServRationViewFilter filterRation = new HisSereServRationViewFilter();
                filterRation.TREATMENT_ID = (long)obj;
                this._SereServRation = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_RATION>>("api/HisSereServRation/GetView", ApiConsumers.MosConsumer, filterRation, param);

                if (HisConfigCFG.Config_TrackingCreate_BloodPresOption != null
                    && HisConfigCFG.Config_TrackingCreate_BloodPresOption.Trim() == "1")
                {
                    MOS.Filter.HisExpMestBltyReqView2Filter expMestBltyReqView2Filter = new HisExpMestBltyReqView2Filter();
                    expMestBltyReqView2Filter.TDL_TREATMENT_ID = (long)obj;
                    this.ExpMestBltyReq2 = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ_2>>("api/HisExpMestBltyReq/GetView2", ApiConsumers.MosConsumer, expMestBltyReqView2Filter, new CommonParam());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatmentNewThread(object param)
        {
            try
            {
                LoadDataTreatment((long)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatment(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentFilter();
                hisTreatmentFilter.ID = treatmentId;
                this._Treatment = new HIS_TREATMENT();
                var treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                if (treatments != null && treatments.Count > 0)
                {
                    this._Treatment = treatments.FirstOrDefault();
                }

                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_ID = treatmentId;
                this._TreatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, bedFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReqNewThread(object param)
        {
            try
            {
                LoadDataServiceReq((long)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReq(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                //danh sach yeu cau
                MOS.Filter.HisServiceReqFilter serviceReqFilterVT = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilterVT.TREATMENT_ID = treatmentId;
                var _ServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilterVT, param);
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    foreach (var item in _ServiceReqs)
                    {
                        if (!dicServiceReqs.ContainsKey(item.ID))
                        {
                            dicServiceReqs[item.ID] = new HIS_SERVICE_REQ();
                            dicServiceReqs[item.ID] = item;
                        }
                    }

                    List<Task> taskall = new List<Task>();
                    int skip = 0;
                    while (_ServiceReqs.Count - skip > 0)
                    {
                        var listSub = _ServiceReqs.Skip(skip).Take(100).ToList();
                        skip += 100;
                        List<long> _serviceReqIds = listSub.Select(s => s.ID).ToList();

                        Task tsExpMest = Task.Factory.StartNew((object obj) =>
                        {
                            LoadDataExpMest((List<long>)obj);
                        }, _serviceReqIds);
                        taskall.Add(tsExpMest);

                        Task tsMedicine = Task.Factory.StartNew((object obj) =>
                        {
                            LoadDataServiceReqMety((List<long>)obj);
                        }, _serviceReqIds);

                        taskall.Add(tsMedicine);
                        Task tsMaterial = Task.Factory.StartNew((object obj) =>
                        {
                            LoadDataServiceReqMaty((List<long>)obj);
                        }, _serviceReqIds);
                        taskall.Add(tsMaterial);

                        Task tsSereServ = Task.Factory.StartNew((object obj) =>
                        {
                            LoadDataSereServ((List<long>)obj);
                        }, _serviceReqIds);
                        taskall.Add(tsSereServ);
                    }

                    Task.WaitAll(taskall.ToArray());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMobaImpMestsNewThread(object param)
        {
            try
            {
                LoadDataMobaImpMests((long)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMobaImpMests(long treatmentId)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisImpMestView2Filter filter = new MOS.Filter.HisImpMestView2Filter();

                filter.TDL_TREATMENT_ID = treatmentId;
                filter.IMP_MEST_TYPE_IDs = new List<long>(){
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL
                    };

                this._MobaImpMests = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2>>
                    ("api/HisImpMest/GetView2", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                if (this._MobaImpMests != null && this._MobaImpMests.Count > 0)
                {
                    MOS.Filter.HisImpMestMedicineViewFilter impMestMedicineViewFilter = new MOS.Filter.HisImpMestMedicineViewFilter();
                    impMestMedicineViewFilter.IMP_MEST_IDs = this._MobaImpMests.Select(o => o.ID).ToList();
                    this._ImpMestMedicines_TL = new BackendAdapter(paramCommon).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, impMestMedicineViewFilter, paramCommon);

                    MOS.Filter.HisImpMestMaterialViewFilter impMestMaterialViewFilter = new MOS.Filter.HisImpMestMaterialViewFilter();
                    impMestMaterialViewFilter.IMP_MEST_IDs = this._MobaImpMests.Select(o => o.ID).ToList();
                    this._ImpMestMaterial_TL = new BackendAdapter(paramCommon).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, impMestMaterialViewFilter, paramCommon);

                    MOS.Filter.HisImpMestBloodViewFilter impMestBloodViewFilter = new MOS.Filter.HisImpMestBloodViewFilter();
                    impMestBloodViewFilter.IMP_MEST_IDs = this._MobaImpMests.Select(o => o.ID).ToList();
                    this._ImpMestBlood_TL = new BackendAdapter(paramCommon).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, impMestBloodViewFilter, paramCommon);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        //Thuoc trong kho
        private void LoadDataExpMestMedicineNewThread(object param)
        {
            try
            {
                LoadDataExpMestMedicine((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMedicine(List<long> _expMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                filter.EXP_MEST_IDs = _expMestIds;

                this._ExpMestMedicines = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, filter, param);
                if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                {
                    var dataGroups = this._ExpMestMedicines.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MEDICINE_TYPE_ID, p.EXP_MEST_ID, p.TUTORIAL }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        HIS_EXP_MEST_MEDICINE ado = new HIS_EXP_MEST_MEDICINE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MEDICINE>(ado, item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.TH_AMOUNT = item.Sum(p => p.TH_AMOUNT);
                        if (!dicExpMestMedicines.ContainsKey(ado.EXP_MEST_ID ?? 0))
                        {
                            dicExpMestMedicines[ado.EXP_MEST_ID ?? 0] = new List<HIS_EXP_MEST_MEDICINE>();
                            dicExpMestMedicines[ado.EXP_MEST_ID ?? 0].Add(ado);
                        }
                        else
                            dicExpMestMedicines[item[0].EXP_MEST_ID ?? 0].Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //VT Trong Kho
        private void LoadDataExpMestMaterialNewThread(object param)
        {
            try
            {
                LoadDataExpMestMaterial((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMaterial(List<long> _expMestIds)
        {
            try
            {
                long configQY7 = 0;
                configQY7 = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MATERIAL));
                if (configQY7 != 1)
                    return;
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMaterialFilter filter = new HisExpMestMaterialFilter();
                filter.EXP_MEST_IDs = _expMestIds;
                this._ExpMestMaterials = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, filter, param);
                if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                {
                    if (this.keyDoNotShowExpendMaterial == "1")
                        this._ExpMestMaterials = this._ExpMestMaterials.Where(o => o.IS_EXPEND != 1).ToList();

                    var dataGroups = this._ExpMestMaterials.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MATERIAL_TYPE_ID, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        HIS_EXP_MEST_MATERIAL ado = new HIS_EXP_MEST_MATERIAL();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MATERIAL>(ado, item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.TH_AMOUNT = item.Sum(p => p.TH_AMOUNT);
                        if (!dicExpMestMaterials.ContainsKey(ado.EXP_MEST_ID ?? 0))
                        {
                            dicExpMestMaterials[ado.EXP_MEST_ID ?? 0] = new List<HIS_EXP_MEST_MATERIAL>();
                            dicExpMestMaterials[ado.EXP_MEST_ID ?? 0].Add(ado);
                        }
                        else
                            dicExpMestMaterials[item[0].EXP_MEST_ID ?? 0].Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Thread EXP_MEST && SERVICE_REQ_METY
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadByServiceReq(object param)
        {
            Thread threadExpMest = new Thread(new ParameterizedThreadStart(LoadDataExpMestNewThread));
            Thread threadServiceReqMety = new Thread(new ParameterizedThreadStart(LoadDataServiceReqMetyNewThread));
            Thread threadServiceReqMaty = new Thread(new ParameterizedThreadStart(LoadDataServiceReqMatyNewThread));
            Thread threadSereServ = new Thread(new ParameterizedThreadStart(LoadDataSereServNewThread));

            try
            {
                threadExpMest.Start(param);
                threadServiceReqMety.Start(param);
                threadServiceReqMaty.Start(param);
                threadSereServ.Start(param);

                threadExpMest.Join();
                threadServiceReqMety.Join();
                threadServiceReqMaty.Join();
                threadSereServ.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadExpMest.Abort();
                threadServiceReqMety.Abort();
                threadServiceReqMaty.Abort();
                threadSereServ.Abort();
            }
        }

        //Exp_mest
        private void LoadDataExpMestNewThread(object param)
        {
            try
            {
                LoadDataExpMest((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMest(List<long> _serviceReqIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.SERVICE_REQ_IDs = _serviceReqIds;
                var expMestDatas = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                if (expMestDatas != null && expMestDatas.Count > 0)
                {
                    foreach (var item in expMestDatas)
                    {
                        if (!dicExpMests.ContainsKey(item.SERVICE_REQ_ID ?? 0))
                        {
                            dicExpMests[item.SERVICE_REQ_ID ?? 0] = new HIS_EXP_MEST();
                            dicExpMests[item.SERVICE_REQ_ID ?? 0] = (item);
                        }
                        else
                            dicExpMests[item.SERVICE_REQ_ID ?? 0] = (item);
                    }

                    //this._ExpMests.AddRange(expMestDatas);
                    List<Task> taskall = new List<Task>();
                    Task tsMedicine = Task.Factory.StartNew((object obj) =>
                    {
                        LoadDataExpMestMedicine((List<long>)obj);
                    }, expMestDatas.Select(s => s.ID).ToList());

                    taskall.Add(tsMedicine);
                    Task tsMaterial = Task.Factory.StartNew((object obj) =>
                    {
                        LoadDataExpMestMaterial((List<long>)obj);
                    }, expMestDatas.Select(s => s.ID).ToList());
                    taskall.Add(tsMaterial);

                    Task tsImpMest = Task.Factory.StartNew((object obj) =>
                    {
                        LoadImpMestNewThread((List<long>)obj);
                    }, expMestDatas.Select(s => s.ID).ToList());
                    taskall.Add(tsImpMest);

                    Task.WaitAll(taskall.ToArray());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Thuoc Ngoai Kho
        private void LoadDataServiceReqMetyNewThread(object param)
        {
            try
            {
                LoadDataServiceReqMety((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReqMety(List<long> _serviceReqIds)
        {
            try
            {
                if (IsNotShowOutMediAndMate)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqMetyFilter metyFIlter = new HisServiceReqMetyFilter();
                    metyFIlter.SERVICE_REQ_IDs = _serviceReqIds;
                    var metyDatas = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, metyFIlter, param);
                    if (metyDatas != null && metyDatas.Count > 0)
                    {
                        foreach (var item in metyDatas)
                        {
                            if (!dicServiceReqMetys.ContainsKey(item.SERVICE_REQ_ID))
                            {
                                dicServiceReqMetys[item.SERVICE_REQ_ID] = new List<HIS_SERVICE_REQ_METY>();
                                dicServiceReqMetys[item.SERVICE_REQ_ID].Add(item);
                            }
                            else
                                dicServiceReqMetys[item.SERVICE_REQ_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //VT Ngoai Kho
        private void LoadDataServiceReqMatyNewThread(object param)
        {
            try
            {
                LoadDataServiceReqMaty((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReqMaty(List<long> _serviceReqIds)
        {
            try
            {
                if (IsNotShowOutMediAndMate)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqMatyFilter matyFIlter = new HisServiceReqMatyFilter();
                    matyFIlter.SERVICE_REQ_IDs = _serviceReqIds;
                    var matyDatas = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, matyFIlter, param);
                    if (matyDatas != null && matyDatas.Count > 0)
                    {
                        foreach (var item in matyDatas)
                        {
                            if (!dicServiceReqMatys.ContainsKey(item.SERVICE_REQ_ID))
                            {
                                dicServiceReqMatys[item.SERVICE_REQ_ID] = new List<HIS_SERVICE_REQ_MATY>();
                                dicServiceReqMatys[item.SERVICE_REQ_ID].Add(item);
                            }
                            else
                                dicServiceReqMatys[item.SERVICE_REQ_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataSereServNewThread(object param)
        {
            try
            {
                LoadDataSereServ((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServ(List<long> _serviceReqIds)
        {
            try
            {
                if (_serviceReqIds == null || _serviceReqIds.Count <= 0)
                    return;

                this._SereServs = new List<HIS_SERE_SERV>();
                List<V_HIS_SERVICE> serviceNotShows = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_NOT_SHOW_TRACKING == 1).ToList();

                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFilter hisSereServFilterVT = new MOS.Filter.HisSereServFilter();
                hisSereServFilterVT.SERVICE_REQ_IDs = _serviceReqIds;
                var datas = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilterVT, param);
                if (datas != null && datas.Count > 0)
                {
                    //lọc bỏ các dịch vụ không hiển thị trong tờ điều trị
                    if (serviceNotShows != null && serviceNotShows.Count() > 0)
                        datas = datas.Where(o => !serviceNotShows.Exists(p => p.ID == o.SERVICE_ID)).ToList();

                    if (datas != null && datas.Count > 0)
                    {
                        this._SereServs.AddRange(datas);

                        foreach (var item in datas)
                        {
                            if (!dicSereServs.ContainsKey(item.SERVICE_REQ_ID ?? 0))
                            {
                                dicSereServs[item.SERVICE_REQ_ID ?? 0] = new List<HIS_SERE_SERV>();
                            }

                            dicSereServs[item.SERVICE_REQ_ID ?? 0].Add(item);
                        }

                        MOS.Filter.HisSereServExtFilter sereServExtFilter = new MOS.Filter.HisSereServExtFilter();
                        sereServExtFilter.SERE_SERV_IDs = datas.Select(s => s.ID).ToList();
                        var dataSS_EXTs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("/api/HisSereServExt/Get", ApiConsumers.MosConsumer, sereServExtFilter, param);
                        if (dataSS_EXTs != null && dataSS_EXTs.Count > 0)
                        {
                            this._SereServExts.AddRange(dataSS_EXTs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadImpMestNewThread(object param)
        {
            try
            {
                LoadImpMest((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadImpMest(List<long> _expMestIds)
        {
            try
            {
                long keyViewMediMateTH = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MEDI_MATE_TH));
                if (keyViewMediMateTH != 1)
                    return;
                CommonParam param = new CommonParam();
                //Ktra thu hoi
                _ImpMests_input = new List<HIS_IMP_MEST>();
                MOS.Filter.HisImpMestFilter impMestFilter = new MOS.Filter.HisImpMestFilter();
                impMestFilter.MOBA_EXP_MEST_IDs = _expMestIds;
                _ImpMests_input = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, impMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (_ImpMests_input != null && _ImpMests_input.Count > 0)
                {
                    MOS.Filter.HisImpMestMedicineViewFilter impMestMediFilter = new MOS.Filter.HisImpMestMedicineViewFilter();
                    impMestMediFilter.IMP_MEST_IDs = _ImpMests_input.Select(p => p.ID).ToList();
                    _ImpMestMedis = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumers.MosConsumer, impMestMediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    long configQY7 = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MATERIAL));
                    if (configQY7 == 1)
                    {
                        MOS.Filter.HisImpMestMaterialViewFilter impMestMateFilter = new MOS.Filter.HisImpMestMaterialViewFilter();
                        impMestMateFilter.IMP_MEST_IDs = _ImpMests_input.Select(p => p.ID).ToList();
                        _ImpMestMates = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impMestMateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    }
                    MOS.Filter.HisImpMestBloodViewFilter impMestBloodFilter = new MOS.Filter.HisImpMestBloodViewFilter();
                    impMestBloodFilter.IMP_MEST_IDs = _ImpMests_input.Select(p => p.ID).ToList();
                    this._ImpMestBloods = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, impMestBloodFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
