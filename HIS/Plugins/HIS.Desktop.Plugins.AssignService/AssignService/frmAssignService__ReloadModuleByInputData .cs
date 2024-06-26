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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.BackendData.Core.ServiceCombo;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Plugins.AssignService.Validation;
using HIS.Desktop.Utility;
using HIS.Desktop.Utility.ValidateRule;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        ServiceComboADO serviceComboADO = null;
        public void ReloadModuleByInputData(Inventec.Desktop.Common.Modules.Module module, HIS.Desktop.ADO.AssignServiceADO dataADO)
		{
			try
			{
                LogSystem.Debug("ReloadModuleByInputData Start___ " + IsUseApplyFormClosingOption);
                toggleSwitchDataChecked.EditValue = false;
                ResetControl();
                this.currentPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.currentPatientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                intructionTimeSelecteds = null;
                this.ListDepartmentTranCheckTime = null;
                this.ListCoTreatmentCheckTime = null;
                this.IsUseApplyFormClosingOption = true;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                this.actionType = GlobalVariables.ActionAdd;
                HisConfigCFG.LoadConfig();
                this.currentModule = module;
                ReloadServiceFromRam();
                ReloadRoomTimes();
                ReloadExroRoom();
                ReloadInitUC();
                ReloadValidate();
                loadCauHinhIn();
                InitControlState();
                this.workingAssignServiceADO = dataADO;
                if (dataADO != null)
                {
                    this.processDataResult = dataADO.DgProcessDataResult;
                    this.processRefeshIcd = dataADO.DgProcessRefeshIcd;
                    this.treatmentId = dataADO.TreatmentId;
                    this.previusTreatmentId = dataADO.PreviusTreatmentId;
                    this.serviceReqParentId = dataADO.ServiceReqId;
                    this.isInKip = dataADO.IsInKip;
                    this.isAssignInPttt = dataADO.IsAssignInPttt;
                    if (this.isInKip)
                        this.currentSereServInEkip = dataADO.SereServ;
                    else
                        this.currentSereServ = dataADO.SereServ;

                    this.provisionalDiagnosis = dataADO.ProvisionalDiagnosis;
                    this.icdExam = dataADO.IcdExam;
                    this.patientName = dataADO.PatientName;
                    this.patientDob = dataADO.PatientDob;
                    this.genderName = dataADO.GenderName;
                    this.currentDhst = dataADO.Dhst;
                    this.isAutoEnableEmergency = dataADO.IsAutoEnableEmergency;
                    this.isPriority = dataADO.IsPriority;
                    this.tracking = dataADO.Tracking;
                    this.ContructorIntructionTime = dataADO.IntructionTime;
                    this.examRegisterRoomId = dataADO.ExamRegisterRoomId;
                    this.isNotUseBhyt = dataADO.IsNotUseBhyt.HasValue && dataADO.IsNotUseBhyt.Value;
                    //this.GetExroRoom();
                    //this.GetRoomTimes();
                }
                ReloadUCPatientSelect();
                if (this.currentModule != null)
                {
                    currentWorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                }

                //LoadHisServiceFromRam();
                this.isInitTracking = true;
                this.requestRoom = GetRequestRoom(this.currentModule.RoomId);
                this.isNotLoadWhileChangeInstructionTimeInFirst = true;
                gridViewServiceProcess.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;//ẩn panel filter editor mặc định của grid khi gõ tìm kiếm ở các ô

                if (this.serviceReqParentId.HasValue && this.serviceReqParentId > 0)
                {
                    this.LoadDataServiceReqById(this.serviceReqParentId.Value);
                }
                BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM>();
                BackendDataWorker.Reset<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                this.allDataExecuteRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                GetLCounter1Async();
                TimerGetDataGetLCounter1(); // 5 phút
                this.SetDefaultData(true);
                if (this.treatmentId > 0)
                {
                    this.FillAllPatientInfoSelectedInForm();
                }
                this.ProcessInitEventForGridServieProcess();

                CreateThreadLoadDataForPrint();

                this.InitConfig();

                this.LoadDataToCashierRoom();
                this.LoadDataToAssignRoom();
                this.LoadServiceSameToRAM();

                if (this.treatmentId > 0)
                {
                    LogSystem.Debug("ReloadModuleByInputData => 2...");
                    ReloadPatient();
                    this.LoadTotalSereServByHeinWithTreatmentAsync(this.treatmentId);
                    ReloadServicePaty();
                    this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
                    var patientTypePrimary = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1).ToList();
                    this.InitComboPrimaryPatientType(patientTypePrimary);
                    this.InitComboUser();
                    this.InitComboServiceGroup();
                    this.InitComboExecuteRoom();
                    this.LoadTreatmentInfo__PatientType();
                    LogSystem.Debug("ReloadModuleByInputData => 3");
                    this.BindTree();
                    IsFirstLoad = true;
                    LogSystem.Debug("ReloadModuleByInputData => 4");
                    this.LoadDataDhst();
                    this.InitDefaultFocus();
                    LogSystem.Debug("ReloadModuleByInputData => 5");
                    UpdateSwithExpendAll();
                }

                this.gridControlServiceProcess.ToolTipController = this.tooltipService;
                this.isNotLoadWhileChangeInstructionTimeInFirst = false;

                this.InitComboExecuteGroup();
                this.InitComboPackage();
                this.InitPackageDetail();
                LoadDataSereServToGetPatientType();
                LogSystem.Debug("ReloadModuleByInputData => End Load...");

                watch.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), this.ModuleLink, "OpenModule", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));

            }
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

        private void ResetControl()
		{
			try
			{
                txtIcdCode.Text = null;
                cboIcds.EditValue = null;
                txtIcdMainText.Text = null;
                chkEditIcd.Checked = false;
                txtIcdCodeCause.Text = null;
                cboIcdsCause.EditValue = null;
                txtIcdMainTextCause.Text = null;
                txtProvisionalDiagnosis.Text = null;
                txtIcdSubCode.Text = null;
                txtIcdText.Text = null;
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

        private async Task ReloadInitUC()
		{
			try
			{
                Action myaction = () => {
                    this.isAutoCheckIcd = (HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue);
                    this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();

                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                UCIcdInit();
                UCIcdCauseInit();
                UcDateInit();
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

        private async Task ReloadValidate()
        {
            try
            {
                V_HIS_ROOM currentRoom = null;
                Action myaction = () => {
                    if (this.currentModule != null && this.currentModule.RoomId > 0)
                    {
                        currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                if (HisConfigCFG.ObligateIcd == GlobalVariables.CommonStringTrue && (currentRoom != null && currentRoom.IS_ALLOW_NO_ICD != 1))
                {
                    ValidationICD(10, 500, true);
                }
                else
                {
                    ValidationSingleControlWithMaxLength(txtIcdCode, false, 10);
                    ValidationSingleControlWithMaxLength(txtIcdMainText, false, 500);
                }
                ValidationSingleControlWithMaxLength(txtIcdCodeCause, false, 10);
                ValidationSingleControlWithMaxLength(txtIcdMainTextCause, false, 500);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task ReloadUCPatientSelect()
		{
			try
			{
                if (workingAssignServiceADO.OpenFromBedRoomPartial)
                {
                    layoutControlItem27.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    InitUCPatientSelect();

                }
                else
                {
                    layoutControlItem27.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

        private async Task ReloadExroRoom()
        {
            try
            {
                Action myaction = () => {
                    CommonParam param = new CommonParam();
                    V_HIS_ROOM currentWorkingRoom = null;
                    if (cboAssignRoom.EditValue != null)
                    {
                        currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == Convert.ToInt64(cboAssignRoom.EditValue));
                    }
                    else
                    {
                        currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == this.currentModule.RoomId);
                    }
                    if (currentWorkingRoom != null)
                    {
                        CommonParam paramGet = new CommonParam();
                        MOS.Filter.HisExroRoomFilter exroRoomFilter = new MOS.Filter.HisExroRoomFilter();
                        exroRoomFilter.ROOM_ID = currentWorkingRoom.ID;
                        exroRoomFilter.IS_ACTIVE = 1;
                        this.exroRooms = new BackendAdapter(paramGet).Get<List<HIS_EXRO_ROOM>>("api/HisExroRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, exroRoomFilter, null);
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task ReloadRoomTimes()
        {
            try
            {
                Action myaction = () => {
                    this.roomTimes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM_TIME>();
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadServiceFromRam()
		{
            try
            {
                this.dicServices = lstService
                    .ToDictionary(o => o.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task ReloadPatient()
		{
			try
			{
                List<HIS_PATIENT> patients = new List<HIS_PATIENT>();
                Action myaction = () => {
                    if (treatmentId > 0)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisTreatmentFilter HisTreatmentFilter = new MOS.Filter.HisTreatmentFilter();
                        HisTreatmentFilter.ID = treatmentId;
                        currentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, HisTreatmentFilter, param).FirstOrDefault();

                        MOS.Filter.HisPatientViewFilter patientViewFilter = new MOS.Filter.HisPatientViewFilter();
                        patientViewFilter.ID = currentTreatment.PATIENT_ID;
                        patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, patientViewFilter, param);
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                if (patients != null && patients.Count > 0)
                {
                    this.currentPatient = patients.FirstOrDefault();
                }
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

        private async Task ReloadServicePaty()
		{
			try
			{
                List<HIS_PATIENT_TYPE> patientTypeAll = new List<HIS_PATIENT_TYPE>();
                List<V_HIS_SERVICE_PATY> sety = new List<V_HIS_SERVICE_PATY>();
                Action myaction = () => {
                    long[] serviceTypeIdAllows = null;

                    serviceTypeIdAllows = new long[12]{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
                        };



                    patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                    List<long> patientTypeIds = new List<long>();
                    patientTypeIds = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).ToList();

                    long intructionTime = this.intructionTimeSelecteds.FirstOrDefault();
                    long treatmentTime = this.currentHisTreatment.IN_TIME;

                    //Lọc các đối tượng thanh toán không có chính sách giá
                    sety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                                            .Where(t => (patientTypeIds.Contains(t.PATIENT_TYPE_ID) || BranchDataWorker.CheckPatientTypeInherit(t.INHERIT_PATIENT_TYPE_IDS, patientTypeIds.ToList()))
                                                && t.IS_ACTIVE == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonNumberTrue
                                                && t.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()
                                                && serviceTypeIdAllows.Contains(t.SERVICE_TYPE_ID)
                                                && ((!t.TREATMENT_TO_TIME.HasValue || t.TREATMENT_TO_TIME.Value >= treatmentTime) || (!t.TO_TIME.HasValue || t.TO_TIME.Value >= intructionTime))).ToList();


                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                this.patientTypeIdAls = sety != null ? sety.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList() : null;//TODO
                var patientTypeIdPlusAfterFilter = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && this.patientTypeIdAls.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                if (patientTypeIdPlusAfterFilter != null && patientTypeIdPlusAfterFilter.Count > 0)
                {
                    patientTypeIdAls.AddRange(patientTypeIdPlusAfterFilter.Select(o => o.ID));
                }
                if (patientTypeIdAls != null)
                    patientTypeIdAls = patientTypeIdAls.Distinct().ToList();

                this.servicePatyInBranchs = sety
                            .GroupBy(o => o.SERVICE_ID)
                            .ToDictionary(o => o.Key, o => o.ToList());
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private async Task ReloadServiceComboDataWoker()
		{
            try
            {
                Action myaction = () => {
                    if (ServiceComboDataWorker.DicServiceCombo == null)
                        ServiceComboDataWorker.DicServiceCombo = new Dictionary<long, ServiceComboADO>();
                    if (ServiceComboDataWorker.DicServiceCombo.ContainsKey(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID))
                    {
                        ServiceComboDataWorker.DicServiceCombo.TryGetValue(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, out serviceComboADO);
                    }
                    else
                    {
                        serviceComboADO = ServiceComboDataWorker.GetByPatientType(currentHisPatientTypeAlter.PATIENT_TYPE_ID, this.servicePatyInBranchs);

                        ServiceComboDataWorker.DicServiceCombo.Add(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, serviceComboADO);
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
			try
			{
				if (ServiceComboDataWorker.DicServiceCombo == null)
					ServiceComboDataWorker.DicServiceCombo = new Dictionary<long, ServiceComboADO>();
				if (ServiceComboDataWorker.DicServiceCombo.ContainsKey(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID))
				{
					ServiceComboDataWorker.DicServiceCombo.TryGetValue(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, out serviceComboADO);
				}
				else
				{
					serviceComboADO = ServiceComboDataWorker.GetByPatientType(currentHisPatientTypeAlter.PATIENT_TYPE_ID, this.servicePatyInBranchs);

					ServiceComboDataWorker.DicServiceCombo.Add(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, serviceComboADO);
				}
			}
            catch (Exception ex)
			{

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //BindTree();
                if(IsFirstLoad)
                    UpdateSwithExpendAll();
                IsFirstLoad = false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
