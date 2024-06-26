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
using HIS.Desktop.Plugins.HisTrackingList.Event;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000374.PDO;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        public bool IsActionButtonSave = false;
        private void LoadBieuMauPhieuYCKhamBenhVaoVien(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Bieu mau phieu kham benh vao vien");
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                WaitingManager.Show();

                List<Action> methods = new List<Action>();
                methods.Add(ReloadTreatment);
                methods.Add(ReloadViewTreatment);
                methods.Add(LoadClsSereServ);
                methods.Add(LoadDepartmentTran);
                //methods.Add(LoadTranPati);
                //methods.Add(LoadPatient);
                methods.Add(LoadDHST);
                methods.Add(LoadServiceReqView);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                CommonParam param = new CommonParam();
                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatment.PATIENT_ID;
                var patients = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param);

                this.patient = patients.FirstOrDefault();
                LoadPatientTypeAlter();
                string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                string executeRoomName = "";
                string executeDepartmentName = "";
                string hospitalizeDepartmentCode = "";
                string hospitalizeDepartmentName = "";

                executeRoomName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.moduleData.RoomId).RoomName;

                var executeDepartment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == this.HisServiceReqView.EXECUTE_DEPARTMENT_ID);
                var hospitalizeDepartment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == this.treatment.HOSPITALIZE_DEPARTMENT_ID);
                if (executeDepartment != null)
                {
                    executeDepartmentName = executeDepartment.DEPARTMENT_NAME;
                }
                if (hospitalizeDepartment != null)
                {
                    hospitalizeDepartmentCode = hospitalizeDepartment.DEPARTMENT_CODE;
                    hospitalizeDepartmentName = hospitalizeDepartment.DEPARTMENT_NAME;
                }

                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                string ratio_text = "";
                if (patientTypeAlter != null)
                    ratio_text = GetDefaultHeinRatioForView(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patientTypeAlter.RIGHT_ROUTE_CODE);

                MPS.Processor.Mps000007.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000007.PDO.SingleKeyValue();
                singleKeyValue.ExecuteRoomName = executeRoomName;
                singleKeyValue.ExecuteDepartmentName = executeDepartmentName;
                singleKeyValue.RatioText = ratio_text;
                singleKeyValue.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                singleKeyValue.Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                singleKeyValue.HospitalizeDepartmentCode = hospitalizeDepartmentCode;
                singleKeyValue.HospitalizeDepartmentName = hospitalizeDepartmentName;
                if (treatment.ICD_NAME != null)
                {
                    singleKeyValue.Icd_Name = treatment.ICD_NAME;
                }

                var ExamRoomList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_EXAM == 1).ToList();
                List<V_HIS_EXP_MEST_BLOOD> ExpMestBloodList = new List<V_HIS_EXP_MEST_BLOOD>();
                List<V_HIS_EXP_MEST_BLTY_REQ> ExpMestBltyReqList = new List<V_HIS_EXP_MEST_BLTY_REQ>();
                List<V_HIS_EXP_MEST_MEDICINE> ExpMestMedicineList = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> ExpMestMaterialList = new List<V_HIS_EXP_MEST_MATERIAL>();
                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.REQ_ROOM_IDs = ExamRoomList.Select(o => o.ROOM_ID).Distinct().ToList();
                expMestFilter.TDL_TREATMENT_ID = treatment.ID;
                expMestFilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                var expMestList = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, null);
                if (expMestList != null && expMestList.Count > 0)
                {
                    MOS.Filter.HisExpMestBloodViewFilter expMestBloodFilter = new HisExpMestBloodViewFilter();
                    expMestBloodFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestBloodList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestBloodFilter, null);

                    MOS.Filter.HisExpMestBltyReqViewFilter expMestBltyReqFilter = new HisExpMestBltyReqViewFilter();
                    expMestBltyReqFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestBltyReqList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestBltyReqFilter, null);

                    MOS.Filter.HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestMedicineList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMedicineFilter, null);

                    MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestMaterialList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialFilter, null);
                }

                //AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TREATMENT, HIS_TREATMENT>();
                //HIS_TREATMENT treatmentRaw = AutoMapper.Mapper.Map<V_HIS_TREATMENT, HIS_TREATMENT>(treatment);

                WaitingManager.Hide();

                MPS.Processor.Mps000007.PDO.Mps000007PDO rdo = new MPS.Processor.Mps000007.PDO.Mps000007PDO(
                    patient,
                    patientTypeAlter,
                    departmentTrans,
                    this.HisServiceReqView,
                    dhst,
                    ViewTreatment,
                    ClsSereServ,
                    singleKeyValue,
                    ExpMestBloodList,
                    ExpMestBltyReqList,
                    ExpMestMedicineList,
                    ExpMestMaterialList
                    );

                MPS.ProcessorBase.PrintConfig.PreviewType PreviewType;
                if (chkTreatmentFinish.Checked && IsActionButtonSave)
                {
                    if (this.IsPrintExam && this.IsSignExam)
                    {
                        PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                    }
                    else if (this.IsSignExam)
                    {
                        PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                    }
                    else
                    {
                        PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                    }
                }
                else
                {
                    if (this.isPrintSign)
                    {
                        PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                    }
                    else if (this.isSign)
                    {
                        PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                    }
                    else
                    {
                        PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                    }
                }
                IsActionButtonSave = false;
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment != null ? treatment.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, PreviewType, "") { EmrInputADO = inputADO });

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauPhieuYCKhamBenhCapCuu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Bieu mau phieu kham benh vao vien");
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                WaitingManager.Show();

                List<Action> methods = new List<Action>();
                methods.Add(ReloadTreatment);
                methods.Add(LoadClsSereServ);
                methods.Add(LoadDepartmentTran);
                //methods.Add(LoadTranPati);
                methods.Add(LoadPatient);
                methods.Add(LoadDHST);
                methods.Add(LoadServiceReqView);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                LoadPatientTypeAlter();
                string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                string executeRoomName = "";
                string executeDepartmentName = "";

                executeRoomName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.moduleData.RoomId).RoomName;

                var executeDepartment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == this.HisServiceReqView.EXECUTE_DEPARTMENT_ID);
                if (executeDepartment != null)
                {
                    executeDepartmentName = executeDepartment.DEPARTMENT_NAME;
                }

                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                string ratio_text = "";
                if (patientTypeAlter != null)
                    ratio_text = GetDefaultHeinRatioForView(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patientTypeAlter.RIGHT_ROUTE_CODE);

                MPS.Processor.Mps000374.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000374.PDO.SingleKeyValue();
                singleKeyValue.ExecuteRoomName = executeRoomName;
                singleKeyValue.ExecuteDepartmentName = executeDepartmentName;
                singleKeyValue.RatioText = ratio_text;
                singleKeyValue.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                singleKeyValue.Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                if (treatment.ICD_NAME != null)
                {
                    singleKeyValue.Icd_Name = treatment.ICD_NAME;
                }

                var ExamRoomList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_EXAM == 1).ToList();
                List<V_HIS_EXP_MEST_BLOOD> ExpMestBloodList = new List<V_HIS_EXP_MEST_BLOOD>();
                List<V_HIS_EXP_MEST_BLTY_REQ> ExpMestBltyReqList = new List<V_HIS_EXP_MEST_BLTY_REQ>();
                List<V_HIS_EXP_MEST_MEDICINE> ExpMestMedicineList = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> ExpMestMaterialList = new List<V_HIS_EXP_MEST_MATERIAL>();
                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.REQ_ROOM_IDs = ExamRoomList.Select(o => o.ROOM_ID).Distinct().ToList();
                expMestFilter.TDL_TREATMENT_ID = treatment.ID;
                expMestFilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                var expMestList = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, null);
                if (expMestList != null && expMestList.Count > 0)
                {
                    MOS.Filter.HisExpMestBloodViewFilter expMestBloodFilter = new HisExpMestBloodViewFilter();
                    expMestBloodFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestBloodList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestBloodFilter, null);

                    MOS.Filter.HisExpMestBltyReqViewFilter expMestBltyReqFilter = new HisExpMestBltyReqViewFilter();
                    expMestBltyReqFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestBltyReqList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestBltyReqFilter, null);

                    MOS.Filter.HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestMedicineList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMedicineFilter, null);

                    MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestMaterialList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialFilter, null);
                }
                // begin tracking

                Inventec.Common.Logging.LogSystem.Debug("Load Stat -------------------------");
                #region ----
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<long> trackingIds = new List<long>();

                MOS.Filter.HisTrackingViewFilter trackingFilter = new MOS.Filter.HisTrackingViewFilter();
                trackingFilter.TREATMENT_ID = this.treatmentId;
                trackingFilter.ORDER_FIELD = "TRACKING_TIME";
                trackingFilter.ORDER_DIRECTION = "DESC";

                var vHisTrackingPrint = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GETVIEW, ApiConsumers.MosConsumer, trackingFilter, null);

                if (vHisTrackingPrint != null && vHisTrackingPrint.Count > 0)
                {
                    long? finishTime = null;
                    if (HisServiceReqResult != null && HisServiceReqResult.ServiceReq != null)
                    {
                        finishTime = HisServiceReqResult.ServiceReq.FINISH_TIME;
                    }
                    else
                    {
                        finishTime = HisServiceReqView.FINISH_TIME;
                    }

                    vHisTrackingPrint = vHisTrackingPrint.Where(o => o.ROOM_ID == this.moduleData.RoomId
                        && (!finishTime.HasValue || 
                        (finishTime.HasValue
                        && o.TRACKING_TIME <= finishTime))
                        ).ToList();
                }

                trackingIds = vHisTrackingPrint != null && vHisTrackingPrint.Count > 0
                    ? vHisTrackingPrint.Select(p => p.ID).ToList()
                    : new List<long>();

                _TrackingPrints = new List<HIS_TRACKING>();
                if (vHisTrackingPrint != null && vHisTrackingPrint.Count > 0)
                {
                    foreach (var item in vHisTrackingPrint)
                    {
                        HIS_TRACKING ado = new HIS_TRACKING();
                        AutoMapper.Mapper.CreateMap<V_HIS_TRACKING, HIS_TRACKING>();
                        ado = AutoMapper.Mapper.Map<V_HIS_TRACKING, HIS_TRACKING>(item);
                        _TrackingPrints.Add(ado);
                    }
                }

                _TrackingPrints = _TrackingPrints != null && _TrackingPrints.Count > 0
                    ? _TrackingPrints.OrderBy(p => p.TRACKING_TIME).ToList()
                    : _TrackingPrints;

                _TreatmentBedRoom = new V_HIS_TREATMENT_BED_ROOM();

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

                if (this.treatmentId > 0)
                {
                    CreateThreadLoadData(this.treatmentId);
                }

                int start = 0;
                int count = this._ServiceReqs.Count;
                while (count > 0)
                {
                    int limit = (count <= 100) ? count : 100;
                    var listSub = this._ServiceReqs.Skip(start).Take(limit).ToList();
                    List<long> _serviceReqIds = new List<long>();
                    _serviceReqIds = listSub.Select(p => p.ID).Distinct().ToList();

                    CreateThreadByServiceReq(_serviceReqIds);

                    start += 100;
                    count -= 100;
                }
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
                MPS.Processor.Mps000374.PDO.Mps000374SingleKey singleKey = new MPS.Processor.Mps000374.PDO.Mps000374SingleKey(_workPlaceSDO);
                singleKey.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                singleKey.USER_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                #endregion

                singleKey.IsShowMedicineLine = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.Bordereau.IsShowMedicineLine") == "1");
                //singleKey.IsOrderByType = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.IsOrderByType") == "1");

                singleKey.IsOrderByType = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.OderOption"));

                //end trachKing

                WaitingManager.Hide();

                MPS.Processor.Mps000374.PDO.Mps000374PDO rdo = new MPS.Processor.Mps000374.PDO.Mps000374PDO(
                    treatment,
                _TreatmentBedRoom,
                _TrackingPrints,
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
                    patient,
                    patientTypeAlter,
                    departmentTrans,
                    this.HisServiceReqView,
                    dhst,
                    ClsSereServ,
                    singleKeyValue,
                    ExpMestBloodList,
                    ExpMestBltyReqList,
                    ExpMestMedicineList,
                    ExpMestMaterialList
                    );

                MPS.ProcessorBase.PrintConfig.PreviewType PreviewType;
                if (this.isPrintSign)
                {
                    PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                }
                else if (this.isSign)
                {
                    PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                }
                else
                {
                    PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment != null ? treatment.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, PreviewType, "") { EmrInputADO = inputADO });

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauPhieuBenhAnNgoaiChan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Bieu mau phieu kham benh vao vien");
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                WaitingManager.Show();

                List<Action> methods = new List<Action>();
                methods.Add(ReloadTreatment);
                methods.Add(LoadClsSereServ);
                methods.Add(LoadDepartmentTran);
                //methods.Add(LoadTranPati);
                methods.Add(LoadPatient);
                methods.Add(LoadDHST);
                methods.Add(LoadServiceReqView);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                LoadPatientTypeAlter();
                string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                string executeRoomName = "";
                string executeDepartmentName = "";

                executeRoomName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.moduleData.RoomId).RoomName;

                var executeDepartment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == this.HisServiceReqView.EXECUTE_DEPARTMENT_ID);
                if (executeDepartment != null)
                {
                    executeDepartmentName = executeDepartment.DEPARTMENT_NAME;
                }

                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                string ratio_text = "";
                if (patientTypeAlter != null)
                    ratio_text = GetDefaultHeinRatioForView(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patientTypeAlter.RIGHT_ROUTE_CODE);

                MPS.Processor.Mps000362.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000362.PDO.SingleKeyValue();
                singleKeyValue.ExecuteRoomName = executeRoomName;
                singleKeyValue.ExecuteDepartmentName = executeDepartmentName;
                singleKeyValue.RatioText = ratio_text;
                singleKeyValue.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                singleKeyValue.Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                if (treatment.ICD_NAME != null)
                {
                    singleKeyValue.Icd_Name = treatment.ICD_NAME;
                }

                WaitingManager.Hide();

                MPS.Processor.Mps000362.PDO.Mps000362PDO rdo = new MPS.Processor.Mps000362.PDO.Mps000362PDO(
                    patient,
                    patientTypeAlter,
                    this.HisServiceReqView,
                    dhst,
                    treatment,
                    singleKeyValue
                    );

                MPS.ProcessorBase.PrintConfig.PreviewType PreviewType;
                if (this.isPrintSign)
                {
                    PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                }
                else if (this.isSign)
                {
                    PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                }
                else
                {
                    PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment != null ? treatment.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, PreviewType, "") { EmrInputADO = inputADO });

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                //Lấy thông tin chuyển khoa
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatmentId;
                var aipResult = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param);
                if (aipResult != null)
                {
                    treatment = aipResult.FirstOrDefault();
                    if (this.treatment != null)
                    {
                        UpdateNeedSickLeaveCertControl(this.treatment.NEED_SICK_LEAVE_CERT);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ReloadViewTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                //Lấy thông tin chuyển khoa
                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = treatmentId;
                var aipResult = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentFilter, param);
                if (aipResult != null)
                {
                    ViewTreatment = aipResult.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
