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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackingInMediRecord.TrackingInMediRecord
{
    public partial class frmTrackingInMediRecord : FormBase
    {
        internal enum PrintType
        {
            IN_TO_DIEU_TRI_TRONG_BENH_AN,
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_TO_DIEU_TRI_TRONG_BENH_AN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_YEU_CAU_IN_TO_DIEU_TRI_TRONG_BENH_AN_MPS000429, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }
                Inventec.Desktop.Common.Message.MessageManager.Show(this, new CommonParam(), true);
                //ProcessPrint();
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
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_YEU_CAU_IN_TO_DIEU_TRI_TRONG_BENH_AN_MPS000429:
                        Mps000429(printTypeCode, fileName, ref result);
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

        List<HIS_SERVICE_REQ> _ServiceReqs { get; set; }
        Dictionary<long, HIS_SERVICE_REQ> dicServiceReqs { get; set; }

        List<HIS_SERE_SERV> _SereServs { get; set; }
        Dictionary<long, List<HIS_SERE_SERV>> dicSereServs { get; set; }

        List<HIS_EXP_MEST> _ExpMests { get; set; }
        Dictionary<long, HIS_EXP_MEST> dicExpMests { get; set; }

        List<HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        Dictionary<long, List<HIS_EXP_MEST_MEDICINE>> dicExpMestMedicines { get; set; }

        List<HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        Dictionary<long, List<HIS_EXP_MEST_MATERIAL>> dicExpMestMaterials { get; set; }

        Dictionary<long, List<HIS_SERVICE_REQ_METY>> dicServiceReqMetys { get; set; }
        Dictionary<long, List<HIS_SERVICE_REQ_MATY>> dicServiceReqMatys { get; set; }

        List<HIS_SERE_SERV_EXT> _SereServExts { get; set; }

        bool IsNotShowOutMediAndMate = false;

        private void Mps000429(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                #region ----
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<long> trackingIds = new List<long>();
                if (this.lstTrackingCheck != null && this.lstTrackingCheck.Count > 0)
                    trackingIds = this.lstTrackingCheck.Select(p => p.ID).ToList();
                else
                    return;


                _ServiceReqs = new List<HIS_SERVICE_REQ>();
                dicServiceReqs = new Dictionary<long, HIS_SERVICE_REQ>();

                _SereServs = new List<HIS_SERE_SERV>();
                dicSereServs = new Dictionary<long, List<HIS_SERE_SERV>>();

                _ExpMests = new List<HIS_EXP_MEST>();
                dicExpMests = new Dictionary<long, HIS_EXP_MEST>();

                _ExpMestMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                dicExpMestMedicines = new Dictionary<long, List<HIS_EXP_MEST_MEDICINE>>();

                _ExpMestMaterials = new List<HIS_EXP_MEST_MATERIAL>();
                dicExpMestMaterials = new Dictionary<long, List<HIS_EXP_MEST_MATERIAL>>();

                dicServiceReqMetys = new Dictionary<long, List<HIS_SERVICE_REQ_METY>>();

                dicServiceReqMatys = new Dictionary<long, List<HIS_SERVICE_REQ_MATY>>();

                IsNotShowOutMediAndMate = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.IsNotShowOutMediAndMate") == "1");

                //TH
                this._ImpMests_input = new List<HIS_IMP_MEST>();
                this._ImpMestMedis = new List<V_HIS_IMP_MEST_MEDICINE>();
                this._ImpMestMates = new List<V_HIS_IMP_MEST_MATERIAL>();

                this._SereServExts = new List<HIS_SERE_SERV_EXT>();

                List<long> treatmentIds = this.lstTrackingCheck.Select(o => o.TREATMENT_ID).Distinct().ToList();

                if (treatmentIds.Count > 0)
                {
                    LoadDataServiceReq(treatmentIds);
                }

                int start = 0;
                int count = this._ServiceReqs.Count;
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => count), count));
                while (count > 0)
                {
                    int limit = (count <= 100) ? count : 100;
                    var listSub = this._ServiceReqs.Skip(start).Take(limit).ToList();
                    List<long> _serviceReqIds = new List<long>();
                    _serviceReqIds = listSub.Select(p => p.ID).Distinct().ToList();

                    Inventec.Common.Logging.LogSystem.Info("_serviceReqIds: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _serviceReqIds), _serviceReqIds));
                    CreateThreadByServiceReq(_serviceReqIds);

                    start += 100;
                    count -= 100;
                }
                Inventec.Common.Logging.LogSystem.Info("_ExpMests: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMests), _ExpMests));
                List<long> expMestIds = new List<long>();
                if (this._ExpMests != null && this._ExpMests.Count > 0)
                {
                    expMestIds = this._ExpMests.Select(p => p.ID).Distinct().ToList();
                    CreateThreadLoadDataExpMest(expMestIds);
                }

                if (this._SereServs != null && this._SereServs.Count > 0)
                {
                    int startSS = 0;
                    int countSS = this._SereServs.Count;
                    // Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => countSS), countSS));
                    while (countSS > 0)
                    {
                        int limit = (countSS <= 100) ? countSS : 100;
                        var listSub = this._SereServs.Skip(startSS).Take(limit).ToList();
                        List<long> _sereServIds = new List<long>();
                        _sereServIds = listSub.Select(p => p.ID).Distinct().ToList();

                        //Get SERE_SERV_EXT
                        MOS.Filter.HisSereServExtFilter sereServExtFilter = new MOS.Filter.HisSereServExtFilter();
                        sereServExtFilter.SERE_SERV_IDs = _sereServIds;

                        var dataSS_EXTs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("/api/HisSereServExt/Get", ApiConsumers.MosConsumer, sereServExtFilter, param);
                        if (dataSS_EXTs != null && dataSS_EXTs.Count > 0)
                        {
                            this._SereServExts.AddRange(dataSS_EXTs);
                        }

                        startSS += 100;
                        countSS -= 100;
                    }
                }

                //Kiem tra cấu hình
                long keyVienTim = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_NUMBER_BY_MEDICINE_TYPE));
                #endregion

                #region Dấu hiệu sinh tồn
                MOS.Filter.HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TRACKING_IDs = trackingIds;
                var _Dhsts = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);
                #endregion

                #region Danh sách chăm sóc
                List<HIS_CARE> _Cares = new List<HIS_CARE>();
                List<V_HIS_CARE_DETAIL> _CareDetails = new List<V_HIS_CARE_DETAIL>();
                foreach (var itemTrackingId in trackingIds)
                {
                    MOS.Filter.HisCareFilter careFilter = new HisCareFilter();
                    careFilter.TRACKING_ID = itemTrackingId;
                    var care = new BackendAdapter(param).Get<List<HIS_CARE>>(HisRequestUriStore.HIS_CARE_GET, ApiConsumers.MosConsumer, careFilter, param).FirstOrDefault();
                    if (care != null)
                    {
                        _Cares.Add(care);
                        MOS.Filter.HisCareDetailViewFilter careDetailFilter = new HisCareDetailViewFilter();
                        careDetailFilter.CARE_ID = care.ID;
                        var careDetail = new BackendAdapter(param).Get<List<V_HIS_CARE_DETAIL>>(HisRequestUriStore.HIS_CARE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, careDetailFilter, param);
                        _CareDetails.AddRange(careDetail);
                    }
                }
                #endregion

                #region Thông tin khoa phòng hiện tại
                MOS.SDO.WorkPlaceSDO _workPlaceSDO = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.moduleData.RoomId);
                MPS.Processor.Mps000429.PDO.Mps000429SingleKey singleKey = new MPS.Processor.Mps000429.PDO.Mps000429SingleKey(_workPlaceSDO);
                singleKey.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                singleKey.USER_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                #endregion

                singleKey.IsShowMedicineLine = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.Bordereau.IsShowMedicineLine") == "1");
                //singleKey.IsOrderByType = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.IsOrderByType") == "1");

                singleKey.IsOrderByType = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.OderOption"));

                #region MobaImpMest
                CommonParam paramCommon = new CommonParam();
                List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2> mobaImpMests = null;
                List<V_HIS_IMP_MEST_MEDICINE> impMestMedicineAPIs = null;
                List<V_HIS_IMP_MEST_MATERIAL> impMestMaterialAPIs = null;

                if (this.lsttreatment != null && this.lsttreatment.Count > 0)
                {
                    mobaImpMests = new List<V_HIS_IMP_MEST_2>();
                    List<long> mobaTreatmentIds = this.lsttreatment.Select(o => o.ID).ToList();

                    int skip = 0;
                    while (mobaTreatmentIds.Count - skip > 0)
                    {
                        var lstIds = mobaTreatmentIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        MOS.Filter.HisImpMestView2Filter mobaFilter = new MOS.Filter.HisImpMestView2Filter();
                        mobaFilter.TDL_TREATMENT_IDs = lstIds;
                        mobaFilter.IMP_MEST_TYPE_IDs = new List<long>(){
                            IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL,
                            IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL,
                            IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL,
                            IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL
                        };

                        var mobaImpMestApis = new Inventec.Common.Adapter.BackendAdapter
                             (paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2>>
                             ("api/HisImpMest/GetView2", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, mobaFilter, paramCommon);
                        if (mobaImpMestApis != null && mobaImpMestApis.Count > 0)
                        {
                            mobaImpMests.AddRange(mobaImpMestApis);
                        }
                    }

                    if (mobaImpMests != null && mobaImpMests.Count > 0)
                    {
                        impMestMedicineAPIs = new List<V_HIS_IMP_MEST_MEDICINE>();
                        impMestMaterialAPIs = new List<V_HIS_IMP_MEST_MATERIAL>();

                        var lstMobaImpMestIds = mobaImpMests.Select(s => s.ID).ToList();

                        int skip2 = 0;
                        while (lstMobaImpMestIds.Count - skip2 > 0)
                        {
                            var ids = lstMobaImpMestIds.Skip(skip2).Take(100).ToList();
                            skip2 += 100;

                            paramCommon = new CommonParam();
                            MOS.Filter.HisImpMestMedicineViewFilter impMestMedicineViewFilter = new MOS.Filter.HisImpMestMedicineViewFilter();
                            impMestMedicineViewFilter.IMP_MEST_IDs = ids;
                            var impMestMedicineResults = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);
                            if (impMestMedicineResults != null && impMestMedicineResults.Count > 0)
                            {
                                impMestMedicineAPIs.AddRange(impMestMedicineResults);
                            }


                            paramCommon = new CommonParam();
                            MOS.Filter.HisImpMestMaterialViewFilter impMestMaterialViewFilter = new MOS.Filter.HisImpMestMaterialViewFilter();
                            impMestMaterialViewFilter.IMP_MEST_IDs = ids;
                            var impMestMaterialResults = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);
                            if (impMestMaterialResults != null && impMestMaterialResults.Count > 0)
                            {
                                impMestMaterialAPIs.AddRange(impMestMaterialResults);
                            }
                        }

                        impMestMedicineAPIs = impMestMedicineAPIs != null ? impMestMedicineAPIs.OrderBy(o => o.ID).ToList() : null;
                        impMestMaterialAPIs = impMestMaterialAPIs != null ? impMestMaterialAPIs.OrderBy(o => o.ID).ToList() : null;
                    }
                }

                #endregion

                #region Mps000429
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.lsttreatment.FirstOrDefault() != null ? this.lsttreatment.FirstOrDefault().TREATMENT_CODE : ""), printTypeCode, this.moduleData != null ? this.moduleData.RoomId : 0);

                long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_PRINT_MERGE));
                if (keyPrintMerge == 1)
                {
                    string uniqueTime = "";// (_TrackingPrints != null && _TrackingPrints.Count > 0) ? _TrackingPrints[0].TRACKING_TIME + "" : "";
                    inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, uniqueTime, (this.MediRecord != null ? MediRecord.PATIENT_CODE : ""));//TODO
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.MergeCode), inputADO.MergeCode));

                HIS_MEDI_RECORD Record = new HIS_MEDI_RECORD();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDI_RECORD>(Record, this.MediRecord);

                var patient = BackendDataWorker.Get<V_HIS_PATIENT>().FirstOrDefault(o => o.ID == this.MediRecord.PATIENT_ID);
                Inventec.Common.Logging.LogSystem.Info("patient: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patient), patient));

                MPS.Processor.Mps000429.PDO.Mps000429PDO mps000429RDO = new MPS.Processor.Mps000429.PDO.Mps000429PDO(
                    patient,
                    Record,
                    this.lsttreatment,
                    this.lstTrackingCheck,
                    _Dhsts,
                    dicServiceReqs,
                    dicSereServs,
                    dicExpMests,
                    dicExpMestMedicines,
                    dicExpMestMaterials,
                    dicServiceReqMetys,
                    dicServiceReqMatys,
                    _Cares,
                    _CareDetails,
                    singleKey,
                    BackendDataWorker.Get<HIS_ICD>(),
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    BackendDataWorker.Get<HIS_SERVICE_TYPE>(),
                    keyVienTim,
                    //Thu Hoi
                    this._ImpMests_input,
                    this._ImpMestMedis,
                    this._ImpMestMates,
                    this._SereServExts,
                    BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>(),
                    mobaImpMests,
                    impMestMedicineAPIs,
                    impMestMaterialAPIs
                    );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000429RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000429RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }


                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);

                if (PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow || PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow)
                {
                    WaitingManager.Hide();
                    Inventec.Desktop.Common.Message.MessageManager.Show(this, new CommonParam(), true);
                }
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Thuoc
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadLoadDataExpMest(object param)
        {
            Thread threadMedicine = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataExpMestMedicineNewThread));
            Thread threadMaterial = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataExpMestMaterialNewThread));
            Thread threadImpMest = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadImpMestNewThread));

            try
            {
                threadMedicine.Start(param);
                threadMaterial.Start(param);
                threadImpMest.Start(param);

                threadMedicine.Join();
                threadMaterial.Join();
                threadImpMest.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedicine.Abort();
                threadMaterial.Abort();
                threadImpMest.Abort();
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                if (this._IsMaterial)
                    return;
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMaterialFilter filter = new HisExpMestMaterialFilter();
                filter.EXP_MEST_IDs = _expMestIds;
                this._ExpMestMaterials = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, filter, param);

                if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                {
                    var dataGroups = this._ExpMestMaterials.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MATERIAL_TYPE_ID, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        HIS_EXP_MEST_MATERIAL ado = new HIS_EXP_MEST_MATERIAL();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MATERIAL>(ado, item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
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
                    var dataGroups = this._ExpMestMedicines.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MEDICINE_TYPE_ID, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        HIS_EXP_MEST_MEDICINE ado = new HIS_EXP_MEST_MEDICINE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MEDICINE>(ado, item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
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

            threadExpMest.Priority = ThreadPriority.Normal;
            threadServiceReqMety.Priority = ThreadPriority.Normal;
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
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFilter hisSereServFilterVT = new MOS.Filter.HisSereServFilter();
                hisSereServFilterVT.SERVICE_REQ_IDs = _serviceReqIds;
                var datas = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilterVT, param);
                if (datas != null && datas.Count > 0)
                {
                    this._SereServs.AddRange(datas);
                    foreach (var item in datas)
                    {
                        if (!dicSereServs.ContainsKey(item.SERVICE_REQ_ID ?? 0))
                        {
                            dicSereServs[item.SERVICE_REQ_ID ?? 0] = new List<HIS_SERE_SERV>();
                            dicSereServs[item.SERVICE_REQ_ID ?? 0].Add(item);
                        }
                        else
                            dicSereServs[item.SERVICE_REQ_ID ?? 0].Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
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
                }
                this._ExpMests.AddRange(expMestDatas);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataServiceReq(List<long> treatmentIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                //danh sach yeu cau
                MOS.Filter.HisServiceReqFilter serviceReqFilterVT = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilterVT.TREATMENT_IDs = treatmentIds;
                this._ServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilterVT, param);
                if (this._ServiceReqs != null && this._ServiceReqs.Count > 0)
                {
                    foreach (var item in this._ServiceReqs)
                    {
                        if (!dicServiceReqs.ContainsKey(item.ID))
                        {
                            dicServiceReqs[item.ID] = new HIS_SERVICE_REQ();
                            dicServiceReqs[item.ID] = item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
