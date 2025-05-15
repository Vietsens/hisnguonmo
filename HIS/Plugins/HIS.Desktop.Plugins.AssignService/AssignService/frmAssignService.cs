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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.ApplicationFont;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Plugins.AssignService.Resources;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utilities.Extentions;
using HIS.Desktop.Utility;
using HIS.UC.DateEditor;
using HIS.UC.PatientSelect;
using HIS.UC.SecondaryIcd;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.Library.CheckIcd;
using SDA.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.ConfigSystem;
using System.Reflection;
using Inventec.Desktop.Common.Modules;
using DevExpress.XtraBars;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using DevExpress.XtraBars.Controls;
using static MPS.ProcessorBase.PrintConfig;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
	public partial class frmAssignService : HIS.Desktop.Utility.FormBase
	{
		#region Reclare variable
		List<string> arrControlEnableNotChange = new List<string>();
		Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
		CheckIcdManager checkIcdManager { get; set; }
		string[] periodSeparators = new string[] { "," };
		string[] icdSeparators = new string[] { ";" };
		const string commonString__true = "1";
		long? serviceReqParentId;
		int positionHandleControl = -1;
		int actionType = 0;
		long treatmentId = 0;
		long previusTreatmentId = 0;
		long? examRegisterRoomId;
		internal bool isMultiDateState = false;
		internal List<long> intructionTimeSelecteds = new List<long>();
		internal List<DateTime?> intructionTimeSelected = new List<DateTime?>();
		DateTime timeSelested;
		internal long InstructionTime { get; set; }
		bool isInitUcDate;

		V_HIS_SERE_SERV currentSereServ { get; set; }
		V_HIS_SERE_SERV currentSereServInEkip { get; set; }
		HIS.Desktop.ADO.AssignServiceADO.DelegateProcessDataResult processDataResult;
		HIS.Desktop.ADO.AssignServiceADO.DelegateProcessRefeshIcd processRefeshIcd;
		bool isInKip;
		bool isAssignInPttt;
		string patientName;
		long patientDob;
		string genderName;
		bool isAutoEnableEmergency;
		bool isPriority;
		string provisionalDiagnosis;

		HisTreatmentWithPatientTypeInfoSDO currentHisTreatment { get; set; }
		MOS.EFMODEL.DataModels.HIS_SERVICE_REQ serviceReqMain { get; set; }
		MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
		List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
		List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6> currentPreServiceReqs;
		HisServiceReqListResultSDO serviceReqComboResultSDO { get; set; }

		HideCheckBoxHelper hideCheckBoxHelper__Service;
		BindingList<ServiceADO> records;
		List<SereServADO> ServiceIsleafADOs = null;
		List<ServiceADO> ServiceParentADOs;
		List<ServiceADO> ServiceParentADOForGridServices;
		List<ServiceADO> ServiceAllADOs;
		ServiceADO SereServADO__Main;

		List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServWithTreatment = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
		List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServsInTreatment = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
		List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServsInTreatmentRaw = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
		Inventec.Desktop.Common.Modules.Module currentModule;

		Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs;
		Dictionary<long, V_HIS_SERVICE> dicServices;
		List<HIS_ICD_SERVICE> icdServicePhacDos { get; set; }
		List<L_HIS_ROOM_COUNTER_1> hisRoomCounters;

		bool isNotUseBhyt;

		decimal currentExpendInServicePackage = 0;
		bool isSaveAndPrint = false;

		int groupType__ServiceTypeName = 1;
		int groupType__PtttGroupName = 2;
		bool notSearch;

		bool isStopEventChangeMultiDate;
		bool IsObligatoryTranferMediOrg = false;
		bool IsAcceptWordNotInData = false;
		bool isAutoCheckIcd;
		string _TextIcdName = "";
		string _TextIcdNameCause = "";

        List<HIS_ICD> currentIcds;

		List<HIS_PATIENT_TYPE> currentPatientTypes;
		List<V_HIS_PATIENT_TYPE_ALLOW> currentPatientTypeAllows;
		HIS_DHST currentDhst;
		bool IscheckAllTreeService = false;
		bool isYes = false;
		decimal totalHeinByTreatment = 0;
		decimal totalHeinPriceByTreatment = 0;
		decimal totalHeinPriceByTreatmentBK = 0;
		internal HIS_ICD icdChoose { get; set; }
		List<HIS_ROOM_TIME> roomTimes;
		List<MOS.EFMODEL.DataModels.HIS_EXRO_ROOM> exroRooms;
		List<TrackingAdo> trackingAdos;
		MOS.EFMODEL.DataModels.V_HIS_ROOM requestRoom;
		HIS_DEPARTMENT currentDepartment = null;
		long[] serviceTypeIdAllows;
		List<long> patientTypeIdAls;
		List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> currentExecuteRooms;
		List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> allDataExecuteRooms;
		List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_SAME> currentServiceSames;
		MOS.EFMODEL.DataModels.HIS_SERVICE_REQ icdExam;
		bool isNotHandlerWhileChangeToggetSwith;
		bool isHandlerResetServiceStateCheckeds;
		bool isHandlerResetServiceStateCheckedForTreeNodes;
		bool isProcessingAfternodeChecked;
		bool isNotEventByChangeServiceParent;
		bool isRunInitEventForGridServieProcess;
		bool isNotLoadWhileChangeInstructionTimeInFirst;
		bool isNotLoadWhileChangeControlStateInFirst;
		bool isUseTrackingInputWhileChangeTrackingTime;
		bool IsFirstloadConditionService;
		bool IsFirstloadForm;
		List<HIS_SERVICE_CONDITION> lstConditionService;
		HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
		List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
		string moduleLink = "HIS.Desktop.Plugins.AssignService";
		List<V_HIS_ROOM> assRoomsWorks = null;
		long ContructorIntructionTime;

		SereServADO currentRowSereServADO;
		List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY> serviceConditions;
		int lastRowHandle = -1;
		ToolTipControlInfo lastInfo = null;
		GridColumn lastColumn = null;
		bool isInitTracking;
		List<LoaiPhieuInADO> lstLoaiPhieu;
		HIS.Desktop.ADO.AssignServiceADO workingAssignServiceADO;

		V_HIS_TREATMENT_FEE treatmentPrint;
		V_HIS_PATIENT patientPrint;
		HIS_PATIENT currentPatient;
		HIS_TREATMENT currentTreatment;

		List<ServiceGroupADO> selectedSeviceGroups;
		List<ServiceGroupADO> selectedSeviceGroupCopys;
		List<ServiceGroupADO> workingServiceGroupADOs;
		List<SereServADO> serviceDeleteWhileSelectSeviceGroups;
		bool isShowContainerMediMaty = false;
		bool isShowContainerMediMatyForChoose = false;
		bool isShow = true;

		int popupHeight = 400;
		bool statecheckColumn;

		MOS.EFMODEL.DataModels.HIS_TRACKING tracking { get; set; }
		List<HIS_TRACKING> trackings;

		decimal transferTreatmentFeeBK = 0;
		decimal transferTreatmentFee = 0;
		HIS_PATIENT_TYPE patientTypeByPT;

		List<HIS.Desktop.Plugins.AssignService.ADO.IcdADO> icdSubcodeAdoChecks;
		HIS.Desktop.Plugins.AssignService.ADO.IcdADO subIcdPopupSelect;
		bool isNotProcessWhileChangedTextSubIcd;
		List<V_HIS_TREATMENT_BED_ROOM> TreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
		bool IsTreatmentInBedRoom;
		V_HIS_ROOM currentWorkingRoom = null;

		string PatientKskCode = null;
		HIS_SERE_SERV hisSereServForGetPatientType = null;
		List<V_HIS_SERVICE> lstService = null;

		List<HIS_TRACKING> Listtrackings { get; set; }
		internal PatientSelectProcessor patientSelectProcessor;
		internal UserControl ucPatientSelect;
		private Dictionary<string, string> dicValidIcd = new Dictionary<string, string>();
		Dictionary<long, List<HIS_PATIENT_TYPE>> dicPatientType = new Dictionary<long, List<HIS_PATIENT_TYPE>>();
		public enum TypeButton
		{
			SAVE,
			SAVE_AND_PRINT,
			EDIT
		}
		List<string> ListMessError = new List<string>();
		private List<string> lstModuleLinkApply;
		private bool IsFirstLoad = false;
		bool IsActionKey = false;
		Dictionary<long?, List<string>> dicMaxAmount = new Dictionary<long?, List<string>>();
		Dictionary<long, HisServiceReqListResultSDO> dicServiceReqList = new Dictionary<long, HisServiceReqListResultSDO>();
		bool assignMulti = false;

		Dictionary<long, string> dicSessionCode = new Dictionary<long, string>();
		List<long> ServicePDDTIds { get; set; }
		DateTime dteCommonParam { get; set; }
		List<long> serviceTypeIdSplitReq { get; set; }
		List<long> serviceTypeIdRequired { get; set; }
		frmSettingAssignOld sAssignOld { get; set; }
		int indexServiceType = 0;

		internal IcdProcessor icdYhctProcessor;
		internal UserControl ucIcdYhct;
		internal SecondaryIcdProcessor subIcdYhctProcessor;
		internal UserControl ucSecondaryIcdYhct;
		List<HIS_PATIENT_TYPE_ROOM> PatientTypeRooms { get; set; }
		#endregion

		#region Construct

		public frmAssignService(Inventec.Desktop.Common.Modules.Module module, HIS.Desktop.ADO.AssignServiceADO dataADO)
			: base(module)
		{
			try
			{
				Inventec.Common.Logging.LogSystem.Info("frmAssignService.Init .1");
				InitializeComponent();
				try
				{
					string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
					this.Icon = Icon.ExtractAssociatedIcon(iconPath);
				}
				catch (Exception ex)
				{
					LogSystem.Warn(ex);
				}

				this.actionType = GlobalVariables.ActionAdd;
				Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignService.AssignService.frmAssignService).Assembly);
				HisConfigCFG.LoadConfig();
				this.currentModule = module;
				this.InitUC();
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
					this.GetExroRoom();
					this.GetRoomTimes();
				}

				if (this.currentModule != null)
				{
					currentWorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
				}
				//Inventec.Common.Logging.LogSystem.Info("frmAssignService.Init .2___sereServsInTreatmentRaw.count=" + (sereServsInTreatmentRaw != null ? sereServsInTreatmentRaw.Count : 0));
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		#endregion

		#region Private method

		private void InitUcIcdYhct()
		{
			try
			{
				icdYhctProcessor = new HIS.UC.Icd.IcdProcessor();
				HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
				ado.DelegateNextFocus = NextForcusOutYhct;
				ado.IsUCCause = false;
				ado.Width = 440;
				ado.LabelTextSize = 90;
				ado.Height = 24;
				ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
				ado.AutoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.AutoCheckIcd") == "1";
				ado.LblIcdMain = "CĐ YHCT:";
				ado.ToolTipsIcdMain = "Chẩn đoán y học cổ truyền";
				ucIcdYhct = (UserControl)icdYhctProcessor.Run(ado);

				if (ucIcdYhct != null)
				{
					this.pnIcdTranditional.Controls.Add(ucIcdYhct);
					ucIcdYhct.Dock = DockStyle.Fill;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void NextForcusOutYhct()
		{
			try
			{
				if (subIcdYhctProcessor != null && ucSecondaryIcdYhct != null && ucSecondaryIcdYhct.Visible == true)
				{
					ModuleControlProcess controlProcess = new ModuleControlProcess(true);
					ModuleControls = controlProcess.GetControls(ucSecondaryIcdYhct);
					int count = 0;
					foreach (var itemCtrl in ModuleControls)
					{
						if (itemCtrl.ControlName == "txtIcdSubCode")
						{
							if (itemCtrl.IsVisible)
							{
								count = count + 1;
							}
						}
						else if (itemCtrl.ControlName == "txtIcdText")
						{
							if (itemCtrl.IsVisible)
							{
								count = count + 1;
							}
						}
					}

					if (count > 0)
					{
						subIcdYhctProcessor.FocusControl(ucSecondaryIcdYhct);
					}
				}
				else
				{
					SendKeys.Send("{TAB}");
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void InitUcSecondaryIcdYhct()
		{
			try
			{
				var icdYhct = BackendDataWorker.Get<V_HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
				subIcdYhctProcessor = new SecondaryIcdProcessor(new CommonParam(), icdYhct);
				HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
				ado.DelegateGetIcdMain = GetIcdMainCodeYhct;
				ado.Width = 440;
				ado.TextSize = 80;
				ado.Height = 24;
				ado.TextLblIcd = "CĐ YHCT phụ:";
				ado.TootiplciIcdSubCode = "Chẩn đoán y học cổ truyền phụ";
				ado.TextNullValue = "Nhấn F1 để chọn bệnh";
				ado.ViewHisIcds = icdYhct;
				ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
				ucSecondaryIcdYhct = (UserControl)subIcdYhctProcessor.Run(ado);

				if (ucSecondaryIcdYhct != null)
				{
					this.pnSubIcdTranditional.Controls.Add(ucSecondaryIcdYhct);
					ucSecondaryIcdYhct.Dock = DockStyle.Fill;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private string GetIcdMainCodeYhct()
		{
			string mainCode = "";
			try
			{
				if (this.icdYhctProcessor != null && this.ucIcdYhct != null)
				{
					var icdValue = this.icdYhctProcessor.GetValue(this.ucIcdYhct);
					if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
					{
						mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return mainCode;
		}


		bool isCheckAssignServiceSimultaneityOption = false;
		bool isCallingApi = false;
		private void CheckAssignServiceSimultaneityOption()
		{
			try
			{
				isCheckAssignServiceSimultaneityOption = false;
				if ((HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION != "1" && HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION != "2") || cboUser.EditValue == null || intructionTimeSelecteds == null || intructionTimeSelecteds.Count == 0)
					return;
				CommonParam param = new CommonParam();
				HisServiceReqCheckSereTimesSDO sdo = new HisServiceReqCheckSereTimesSDO();
				sdo.TreatmentId = treatmentId;
				sdo.Loginnames = new List<string> { cboUser.EditValue.ToString() };
				sdo.SereTimes = intructionTimeSelecteds;
				var CheckSereTimes = new BackendAdapter(param).Post<bool>("api/HisServiceReq/CheckSereTimes", ApiConsumers.MosConsumer, sdo, ProcessLostToken, param);
				if (!CheckSereTimes)
				{
					if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1")
					{
						isCheckAssignServiceSimultaneityOption = true;
						btnSave.Enabled = btnSaveAndPrint.Enabled = btnEdit.Enabled = false;
						MessageManager.Show(this, param, CheckSereTimes);
					}
					else if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2")
					{ 
						if (XtraMessageBox.Show(param.GetMessage() + " Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
						{
							isCheckAssignServiceSimultaneityOption = true;
							btnSave.Enabled = btnSaveAndPrint.Enabled = btnEdit.Enabled = false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

		}
		private void InitCheckIcdManager()
		{
			try
			{
				checkIcdManager = new CheckIcdManager(DlgIcdSubCode, currentTreatment);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void DlgIcdSubCode(string icdCodes, string icdNames)
		{
			try
			{
				Inventec.Common.Logging.LogSystem.Debug("DlgIcdSubCode.1");
				this.isNotProcessWhileChangedTextSubIcd = true;
				ProcessIcdSub(icdCodes, icdNames);
				this.isNotProcessWhileChangedTextSubIcd = false;
				Inventec.Common.Logging.LogSystem.Debug("DlgIcdSubCode.2");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ProcessIcdSub(string icdCodes, string icdNames)
		{
			try
			{
				var lstIcdCode = icdCodes.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
				var lstIcdName = icdNames.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
				var lstIcdCodeScreen = txtIcdSubCode.Text.Trim().Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
				lstIcdCodeScreen.AddRange(lstIcdCode);
				lstIcdCodeScreen = lstIcdCodeScreen.Distinct().ToList();
				string icdCode = string.Join(";", lstIcdCodeScreen);

				var lstIcdNameScreen = txtIcdText.Text.Trim().Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
				lstIcdNameScreen.AddRange(lstIcdName);
				lstIcdNameScreen = lstIcdNameScreen.Distinct().ToList();
				string icdName = string.Join(";", lstIcdNameScreen);
				if (!string.IsNullOrEmpty(icdCode))
				{
					txtIcdSubCode.Text = icdCode;
				}
				else
				{
					txtIcdSubCode.Text = "";
				}
				if (!string.IsNullOrEmpty(icdName))
				{
					txtIcdText.Text = icdName;
				}
				else
				{
					txtIcdText.Text = "";
				}
				ReloadIcdSubContainerByCodeChanged();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private async Task LoadDataSereServToGetPatientType()
		{
			try
			{
				if (serviceReqParentId != null && serviceReqParentId > 0)
				{
					CommonParam param = new CommonParam();
					HisSereServFilter fl = new HisSereServFilter();
					fl.SERVICE_REQ_ID = serviceReqParentId;
					var datas = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, fl, param);
					if (datas != null && datas.Count > 0)
						hisSereServForGetPatientType = datas[0];
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void SetCaptionByLanguageKey()
		{
			try
			{
				////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
				this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnServiceReqList.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnServiceReqList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnServiceReqList.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.btnServiceReqList.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAssignService.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.barbtnSaveShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnSaveShortcut.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.barbtnSaveAndPrintShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnSaveAndPrintShortcut.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.barbtnPrintShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnPrintShortcut.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.barbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				//this.btnSereservInTreatmentPreview.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnSereservInTreatmentPreview.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnBoSungPhacDo.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnBoSungPhacDo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnPrintPhieuHuongDanBN.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnPrintPhieuHuongDanBN.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnPrintPhieuHuongDanBN.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.btnPrintPhieuHuongDanBN.Tooltip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnNew.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.btnNew.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkExpendAll.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkExpendAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboExecuteGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboExecuteGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				//this.cboServiceGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboServiceGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkIsNotRequireFee.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkEmergency.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkPriority.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkPriority.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciTracking.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciTracking.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIsInformResultBySms.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciIsInformResultBySms.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIsEmergency.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciIsEmergency.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIsEmergency.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciIsEmergency.OptionsToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIsInformResultBySms.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciIsInformResultBySms.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnShowDetail.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnShowDetail.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnCreateBill.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnCreateBill.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnDepositService.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnDepositService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridViewServiceProcess.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignService.gridViewServiceProcess.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcChecked_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcChecked_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcChecked_TabService.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.grcChecked_TabService.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcServiceCode_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcServiceCode_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcServiceName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcServiceName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn_PtttGroup.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn_PtttGroup.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnPatientTypeName__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnPatientTypeName__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnPatientTypeName__TabService.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnPatientTypeName__TabService.Tooltip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

				this.gridColumn_Service_PrimaryPatientType.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnPrimaryPatientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn_Service_PrimaryPatientType.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnPrimaryPatientType.Tooltip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcExpend_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcExpend_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnIsKH__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnIsKH__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnExecuteRoomName__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnExecuteRoomName__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemcboExcuteRoom_TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboExcuteRoom_TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcAmount_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcAmount_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcPrice_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcPrice_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcPrice_ServicePatyPrpo.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcPrice_ServicePatyPrpo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnServiceTypeName.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnServiceTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn_InstructionNote.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnInstructionNote.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnShareCount.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnChiPhiNgoaiGoi_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnChiPhiNgoaiGoi_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnChiPhiNgoaiGoi_TabService.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnChiPhiNgoaiGoi_TabService.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnEstimateDuration.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnEstimateDuration.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnShareCount.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnShareCount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnNoDifference.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnNoDifference.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnHeadCardNumberNoDifference.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnHeadCardNumberNoDifference.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemBtnChecked__TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemBtnChecked__TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemcboPatientType_TabService1.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboPatientType_TabService1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemcboPatientType_TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboPatientType_TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.treeService.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignService.treeService.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.treeListServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.treeListServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.treeListServiceReqName.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.treeListServiceReqName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.treeListPrice.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.treeListPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciServiceGroup.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciExecuteGroup.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciExecuteGroup.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciTotalServicePrice.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciTotalServicePrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lcichkPriority.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcichkPriority.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lcichkPriority.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lcichkPriority.OptionsToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciEmergency.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciEmergency.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lcichkExpendAll.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcichkExpendAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lcichkExpendAll.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lcichkExpendAll.OptionsToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.toggleSwitchDataChecked.Properties.OffText = Inventec.Common.Resource.Get.Value("frmAssignService.toggleSwitchDataChecked.Properties.OffText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.toggleSwitchDataChecked.Properties.OnText = Inventec.Common.Resource.Get.Value("frmAssignService.toggleSwitchDataChecked.Properties.OnText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciPreServiceReq.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciPreServiceReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciCashierRoom.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciCashierRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciCashierRoom.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciCashierRoom.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

				this.BtnPrint.Text = Inventec.Common.Resource.Get.Value("frmAssignService.BtnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

				this.lciForlblWeight.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblWeight.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.lciForlblHeight.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblHeight.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.lciForlblBMI.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblBMI.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.lciFortxtProvisionalDiagnosis.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciFortxtProvisionalDiagnosis.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.lciFortxtAssignRoomCode.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciFortxtAssignRoomCode.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

				this.lciForlblSoDuTaiKhoan.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblSoDuTaiKhoan.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

				this.lciTotalServicePriceBhyt.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciTotalServicePriceBhyt.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.lciTotalServicePriceOther.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciTotalServicePriceOther.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

				this.lciIcdTextCause.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lblIcdCauseText.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.lciIcdTextCause.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lblIcdCauseText.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.chkEditIcdCause.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_CHECK_EDIT_ICD", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

				this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_ICD_MAIN", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.chkEditIcd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_CHECK_EDIT_ICD", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				//ado.TextLblIcd = Inventec.Common.Resource.Get.Value("frmAssignService.lciIcdText.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.lciIcdSubCode.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciIcdSubCode.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignService.txtIcdExtraNames.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.btnConfiguration.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.btnConfiguration.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				if (this.currentModule != null)
				{
					this.Text = this.currentModule.text;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		/// <summary>
		///Hàm xét ngôn ngữ cho giao diện frmAssignService
		/// </summary>
		private void SetCaptionByLanguageKeyNew()
		{
			try
			{
				////Khoi tao doi tuong resource
				Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(frmAssignService).Assembly);
				this.lcichkExpendAll.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lcichkExpendAll.OptionsToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lcichkPriority.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lcichkPriority.OptionsToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIsEmergency.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciIsEmergency.OptionsToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIsInformResultBySms.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciIsInformResultBySms.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
				this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboKsk.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboKsk.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAssignService.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.barbtnSaveShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnSaveShortcut.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.barbtnSaveAndPrintShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnSaveAndPrintShortcut.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.barbtnPrintShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnPrintShortcut.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.barbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.BarSavePrint_PrintTH.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.BarSavePrint_PrintTH.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.barBtnSaveNShow.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barBtnSaveNShow.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.bbtnEditCtrlU.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.bbtnEditCtrlU.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnCreateServiceGroup.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnCreateServiceGroup.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnCreateServiceGroup.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.btnCreateServiceGroup.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.ckTK.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.ckTK.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.ckTK.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.ckTK.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lblCaptionFortoggleSwitchDataChecked.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lblCaptionFortoggleSwitchDataChecked.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.Switch_ExpendAll.Properties.OffText = Inventec.Common.Resource.Get.Value("frmAssignService.Switch_ExpendAll.Properties.OffText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.Switch_ExpendAll.Properties.OnText = Inventec.Common.Resource.Get.Value("frmAssignService.Switch_ExpendAll.Properties.OnText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboPackage.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboPackage.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboConsultantUser.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboConsultantUser.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboAssignRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboAssignRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnDepositService.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnDepositService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkPrintDocumentSigned.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkPrintDocumentSigned.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkSign.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkSign.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkPrint.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkPrint.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				//toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.BtnPrint.Text = Inventec.Common.Resource.Get.Value("frmAssignService.BtnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lcCause.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcCause.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboIcdsCause.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboIcdsCause.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkEditIcdCause.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkEditIcdCause.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIcdTextCause.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciIcdTextCause.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIcdTextCause.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciIcdTextCause.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControl21.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControl21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridViewServiceProcess.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignService.gridViewServiceProcess.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcView_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcView_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcChecked_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcChecked_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcChecked_TabService.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.grcChecked_TabService.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcServiceCode_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcServiceCode_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnTDL_HEIN_SERVICE_BHYT_CODE.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnTDL_HEIN_SERVICE_BHYT_CODE.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcServiceName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcServiceName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn_PtttGroup.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn_PtttGroup.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn_InstructionNote.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn_InstructionNote.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnPatientTypeName__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnPatientTypeName__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnPatientTypeName__TabService.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnPatientTypeName__TabService.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcExpend_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcExpend_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcExpend_TabService.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.grcExpend_TabService.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnIsKH__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnIsKH__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnExecuteRoomName__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnExecuteRoomName__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemcboExcuteRoom_TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboExcuteRoom_TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcAmount_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcAmount_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcPrice_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcPrice_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnNoDifference.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnNoDifference.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnNoDifference.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnNoDifference.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnHeadCardNumberNoDifference.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnHeadCardNumberNoDifference.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcPrice_ServicePatyPrpo.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcPrice_ServicePatyPrpo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnServiceTypeName.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnServiceTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnShareCount.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnShareCount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnSERVICE_CONDITION_ID.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnSERVICE_CONDITION_ID.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnSERVICE_CONDITION_NAME.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnSERVICE_CONDITION_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnChiPhiNgoaiGoi_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnChiPhiNgoaiGoi_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnChiPhiNgoaiGoi_TabService.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnChiPhiNgoaiGoi_TabService.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnEstimateDuration.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnEstimateDuration.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnServiceTypeId.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnServiceTypeId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnPtttGroupName.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnPtttGroupName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn_Service_PrimaryPatientType.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn_Service_PrimaryPatientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn_Service_PrimaryPatientType.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn_Service_PrimaryPatientType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumnPACKAGE_ID.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnPACKAGE_ID.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemBtnChecked__TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemBtnChecked__TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemcboPatientType_TabService1.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboPatientType_TabService1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemcboPatientType_TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboPatientType_TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemcboShareCount.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboShareCount.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemCboPrimaryPatientType.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemCboPrimaryPatientType.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemCboPatientTypeReadOnly.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemCboPatientTypeReadOnly.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.repositoryItemcboExcuteRoomPlus_TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboExcuteRoomPlus_TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnPrintPhieuHuongDanBN.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnPrintPhieuHuongDanBN.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnPrintPhieuHuongDanBN.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.btnPrintPhieuHuongDanBN.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkIsEmergency.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkIsEmergency.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkIsInformResultBySms.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkIsInformResultBySms.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnBoSungPhacDo.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnBoSungPhacDo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboTracking.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboTracking.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboCashierRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboCashierRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnCreateBill.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnCreateBill.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkMultiIntructionTime.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkMultiIntructionTime.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciDateEditor.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciDateEditor.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciDateEditor.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciDateEditor.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControl15.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControl15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignService.txtIcdText.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIcdSubCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciIcdSubCode.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIcdSubCode.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciIcdSubCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControl13.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControl13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboIcds.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboIcds.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkEditIcd.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkEditIcd.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIcdText.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciIcdText.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciIcdText.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

				this.cboPriviousServiceReq.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboPriviousServiceReq.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnServiceReqList.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnServiceReqList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.toggleSwitchDataChecked.Properties.OffText = Inventec.Common.Resource.Get.Value("frmAssignService.toggleSwitchDataChecked.Properties.OffText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.toggleSwitchDataChecked.Properties.OnText = Inventec.Common.Resource.Get.Value("frmAssignService.toggleSwitchDataChecked.Properties.OnText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkExpendAll.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkExpendAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboExecuteGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboExecuteGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkIsNotRequireFee.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkIsNotRequireFee.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.chkPriority.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkPriority.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnShowDetail.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnShowDetail.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.treeService.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignService.treeService.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.treeListServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.treeListServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.treeListServiceReqName.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.treeListServiceReqName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.treeListPrice.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.treeListPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboUser.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboUser.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lcitxtLoginName.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcitxtLoginName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciCashierRoom.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciCashierRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciPreServiceReq.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciPreServiceReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciForlblWeight.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblWeight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciForlblHeight.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblHeight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciForlblBMI.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblBMI.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciFortxtAssignRoomCode.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciFortxtAssignRoomCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciFortxtProvisionalDiagnosis.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciFortxtProvisionalDiagnosis.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciFortxtProvisionalDiagnosis.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciFortxtProvisionalDiagnosis.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciEmergency.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciEmergency.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lcichkPriority.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcichkPriority.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciConsultantUser.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciConsultantUser.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lcichkExpendAll.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcichkExpendAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIsEmergency.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciIsEmergency.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciIsInformResultBySms.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciIsInformResultBySms.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciExecuteGroup.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciExecuteGroup.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciPackage.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciPackage.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciPackage.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciPackage.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciSwitch_ExpendAll.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciSwitch_ExpendAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciForlblChiPhiBNPhaiTra.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblChiPhiBNPhaiTra.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciForlblChiPhiBNPhaiTra.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblChiPhiBNPhaiTra.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciForlblDaDong.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblDaDong.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblConThua.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciTotalServicePrice.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciTotalServicePrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciForlblSoDuTaiKhoan.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblSoDuTaiKhoan.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciForlblSoDuTaiKhoan.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblSoDuTaiKhoan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciServiceGroup.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciServiceGroup.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem12.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem12.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem26.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem26.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciTracking.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciTracking.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciTotalServicePriceBhyt.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciTotalServicePriceBhyt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.lciTotalServicePriceOther.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciTotalServicePriceOther.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.customGridViewWithFilterMultiColumn3.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignService.customGridViewWithFilterMultiColumn3.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.customGridViewWithFilterMultiColumn2.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignService.customGridViewWithFilterMultiColumn2.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.Text = Inventec.Common.Resource.Get.Value("frmAssignService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.grcExpend_TabService.ToolTip = Inventec.Common.Resource.Get.Value("grcExpend_TabService.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				ButtonEdit_IsExpenDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("grcExpend_TabService.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		#region Load data
		private void InitUC()
		{
			try
			{
				//this.SetCaptionByLanguageKey();
				SetCaptionByLanguageKeyNew();
				this.isAutoCheckIcd = (HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue);
				this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();

				UCIcdInit();
				UCIcdCauseInit();
				UcDateInit();
				V_HIS_ROOM currentRoom = null;

				if (this.currentModule != null && this.currentModule.RoomId > 0)
				{
					currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
				}
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

				loadCauHinhIn();
				InitControlState();

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		void InitControlState()
		{
			try
			{
				isNotLoadWhileChangeControlStateInFirst = true;
				this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
				this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
				if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
				{
					foreach (var item in this.currentControlStateRDO)
					{
						if (item.KEY == ControlStateConstant.chkPrint)
						{
							chkPrint.Checked = item.VALUE == "1";
							if (chkPrint.Checked)
								chkPrintDocumentSigned.Checked = false;
						}
						else if (item.KEY == ControlStateConstant.chkSign)
						{
							chkSign.Checked = item.VALUE == "1";
						}
						else if (item.KEY == ControlStateConstant.chkPrintDocumentSigned)
						{
							chkPrintDocumentSigned.Checked = item.VALUE == "1";
							if (chkPrintDocumentSigned.Checked)
								chkPrint.Checked = false;
						}
						else if (item.KEY == Switch_ExpendAll.Name && item.VALUE == "1")
						{
							Switch_ExpendAll.IsOn = true;
							UpdateSwithExpendAll();
						}
						else if (item.KEY == ControlStateConstant.ckTK)
						{
							ckTK.Checked = item.VALUE == "1";

						}
						else if (item.KEY == chkAutoCheckPDDT.Name)
						{
							chkAutoCheckPDDT.Checked = item.VALUE == "1";

						}
						else if (item.KEY == cboPriviousServiceReq.Name)
						{
							indexServiceType = Convert.ToInt32(item.VALUE);
						}

						foreach (var phieu in lstLoaiPhieu)
						{
							if (item.KEY == phieu.ID)
							{
								phieu.Check = item.VALUE == "1";
							}
						}
					}
				}
				chkNotCheckService.Checked = ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY_CHOOSING_WHEN_SEARCH) == "1";
				isNotLoadWhileChangeControlStateInFirst = false;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void frmAssignService_Load(object sender, EventArgs e)
		{
			try
			{
				LogSystem.Debug("frmAssignService_Load => Starting...");
				LoadHisServiceFromRam();
				this.IsFirstloadForm = true;
				this.isInitTracking = true;
				this.requestRoom = GetRequestRoom(this.currentModule.RoomId);
				this.IsFirstloadConditionService = true;
				this.isNotLoadWhileChangeInstructionTimeInFirst = true;
				gridViewServiceProcess.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;//ẩn panel filter editor mặc định của grid khi gõ tìm kiếm ở các ô
				this.currentPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				this.currentPatientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				if (this.serviceReqParentId.HasValue && this.serviceReqParentId > 0)
				{
					this.LoadDataServiceReqById(this.serviceReqParentId.Value);
				}
				//BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM>();
				//BackendDataWorker.Reset<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
				this.allDataExecuteRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				InitUcIcdYhct();
				InitUcSecondaryIcdYhct();
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
				this.LoadSampleType();
				this.LoadDataToCashierRoom();
				this.LoadDataToAssignRoom();
				this.LoadServiceSameToRAM();
				this.InitTabIndex();
				this.InitMenuToButtonPrint();
				if (this.treatmentId > 0)
				{
					LogSystem.Debug("frmAssignService_Load => 2...");
					LoadCurrentPatient();
					InitComboKsk();
					DateTime intructTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);
					this.LoadTotalSereServByHeinWithTreatmentAsync(this.treatmentId);
					this.LoadServicePaty();
					this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
					//this.InitComboRepositoryCondition(null);
					var patientTypePrimary = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1).ToList();
					this.InitComboPrimaryPatientType(patientTypePrimary);
					this.InitGridCheckMarksSelectionServiceGroup();
					this.InitComboUser();
					this.InitComboServiceGroup();
					this.InitComboExecuteRoom();
					this.LoadTreatmentInfo__PatientType();
					LogSystem.Debug("frmAssignService_Load => 3");
					this.BindTree();
					IsFirstLoad = true;
					LogSystem.Debug("frmAssignService_Load => 4");
					this.LoadDataDhst();
					this.InitDefaultFocus();
					LogSystem.Debug("frmAssignService_Load => 5");
					this.LoadDataToGridParticipants();
				}
				this.gridControlServiceProcess.ToolTipController = this.tooltipService;
				this.isNotLoadWhileChangeInstructionTimeInFirst = false;
				this.AddBarManager(this.barManager1);

				timerInitForm.Interval = 5000;//Fix
				timerInitForm.Enabled = true;
				timerInitForm.Start();
				LoadDataSereServToGetPatientType();
				LogSystem.Debug("frmAssignService_Load => End...");
				VisibleGridPatient();
				ModuleList(); 
				IsFirstloadForm = false;
				CheckEnableBtnQR();
				CheckAssignServiceSimultaneityOption();
				//nếu diện dt là khám thì k có dự trù
				layoutControlItemDutru.Visibility = currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM 
					? DevExpress.XtraLayout.Utils.LayoutVisibility.Never 
					: DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
				txtDutruTime.Enabled = !chkMultiIntructionTime.Checked;

                string configValue = HisConfigCFG.IsAllowSignaturePrint;

                if (!string.IsNullOrWhiteSpace(configValue))
                {
                    var allowedModules = configValue
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();

                    if (allowedModules.Contains("HIS.Desktop.Plugins.AssignService"))
                    {
                        layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else
                    {
                        layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                }
                else
                {
                    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ModuleList()
		{
			try
			{
				if (!string.IsNullOrEmpty(HisConfigCFG.ModuleLinkApply))
				{
					lstModuleLinkApply = HisConfigCFG.ModuleLinkApply.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		private void LoadHisServiceFromRam()
		{
			try
			{
				lstService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void timerInitForm_Tick(object sender, EventArgs e)
		{
			try
			{
				LogSystem.Debug("timerInitForm_Tick => 1...");
				timerInitForm.Stop();
				this.InitComboExecuteGroup();
				this.InitComboPackage();
				this.InitPackageDetail();
				this.InitCheckIcdManager();
				LogSystem.Debug("timerInitForm_Tick => 2");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ProcessInitEventForGridServieProcess()
		{
			try
			{
				//if (ApplicationFontWorker.GetFontSize() != ApplicationFontConfig.FontSize825)
				//{
				this.gridViewServiceProcess.CalcRowHeight += new DevExpress.XtraGrid.Views.Grid.RowHeightEventHandler(this.gridViewServiceProcess_CalcRowHeight);
				//}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void FillAllPatientInfoSelectedInForm()
		{
			try
			{
				LogSystem.Debug("FillAllPatientInfoSelectedInForm => 1");
				if (HisConfigCFG.IsUsingServerTime == commonString__true
					&& this.currentHisTreatment != null)
				{
					return;
				}
				//this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(ucDate);
				//this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(ucDate);
				this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.FirstOrDefault());
				this.SetDateUc();
				this.ProcessDataWithTreatmentWithPatientTypeInfo();
				LogSystem.Debug("FillAllPatientInfoSelectedInForm => 2");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		V_HIS_ROOM GetRequestRoom(long requestRoomId)
		{
			V_HIS_ROOM result = new V_HIS_ROOM();
			try
			{
				if (requestRoomId > 0)
				{
					result = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == requestRoomId);
					this.currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == result.DEPARTMENT_ID);
				}
			}
			catch (Exception ex)
			{
				result = new V_HIS_ROOM();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return result;
		}

		private void GetRoomTimes()
		{
			try
			{
				this.roomTimes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM_TIME>();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private async Task CheckOverTotalPatientPrice()
		{
			try
			{
				CommonParam param = new CommonParam();
				MOS.Filter.HisTreatmentFeeViewFilter hisTreatmentFeeViewFilter = new HisTreatmentFeeViewFilter();
				hisTreatmentFeeViewFilter.IS_ACTIVE = 1;
				hisTreatmentFeeViewFilter.ID = this.treatmentId;
				Inventec.Common.Logging.LogSystem.Debug("begin call HisTreatment/GetFeeView");
				var treatmentFees = await new BackendAdapter(param).GetAsync<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFeeViewFilter, param);

				if (treatmentFees != null && treatmentFees.Count > 0)
				{
					var treatmentFee = treatmentFees.FirstOrDefault();
					//decimal totalReceiveMore = (treatmentFee.TOTAL_PATIENT_PRICE ?? 0) - (treatmentFee.TOTAL_DEPOSIT_AMOUNT ?? 0) - (treatmentFee.TOTAL_BILL_AMOUNT ?? 0) + (treatmentFee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) + (treatmentFee.TOTAL_REPAY_AMOUNT ?? 0);
					decimal totalPrice = 0;
					decimal totalHeinPrice = 0;
					decimal totalPatientPrice = 0;
					decimal totalDeposit = 0;
					decimal totalBill = 0;
					decimal totalBillTransferAmount = 0;
					decimal totalRepay = 0;
					decimal exemption = 0;
					decimal total_obtained_price = 0;
					totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
					totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
					totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
					totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
					totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
					totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
					exemption = treatmentFees[0].TOTAL_BILL_EXEMPTION ?? 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
					totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
					total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
					this.transferTreatmentFee = totalPatientPrice - total_obtained_price;//Phai thu benh nhan


					lblChiPhiBNPhaiTra.Text = Inventec.Common.Number.Convert.NumberToString(totalPatientPrice, ConfigApplications.NumberSeperator);
					lblDaDong.Text = Inventec.Common.Number.Convert.NumberToString(total_obtained_price, ConfigApplications.NumberSeperator);
					if (this.transferTreatmentFee > 0)
					{
						lblConThua.Text = Inventec.Common.Number.Convert.NumberToString(Math.Abs(this.transferTreatmentFee), ConfigApplications.NumberSeperator);
						lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblConThieu.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
						lciForlblConThua.AppearanceItemCaption.ForeColor = System.Drawing.Color.Red;
					}
					else
					{
						lblConThua.Text = Inventec.Common.Number.Convert.NumberToString(Math.Abs(this.transferTreatmentFee), ConfigApplications.NumberSeperator);
						this.lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblConThua.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
					}

					this.patientTypeByPT = (currentHisPatientTypeAlter != null && currentHisPatientTypeAlter.PATIENT_TYPE_ID > 0) ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.ID == currentHisPatientTypeAlter.PATIENT_TYPE_ID).FirstOrDefault() : null;

					// - Trong trường hợp ĐỐI TƯỢNG BỆNH NHÂN được check "Không cho phép chỉ định dịch vụ nếu thiếu tiền" (HIS_PATIENT_TYPE có IS_CHECK_FEE_WHEN_ASSIGN = 1) và hồ sơ là "Khám" (HIS_TREATMENT có TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM) thì kiểm tra:
					//+ Nếu hồ sơ đang không thừa tiền "Còn thừa" = 0 hoặc hiển thị "Còn thiếu" thì hiển thị thông báo "Bệnh nhân đang nợ tiền, không cho phép chỉ định dịch vụ", người dùng nhấn "Đồng ý" thì tắt form chỉ định.
					//+ Nếu hồ sơ đang thừa tiền ("Còn thừa" > 0), thì khi người dùng check chọn dịch vụ, nếu số tiền "Phát sinh" > "Còn thừa" thì hiển thị cảnh báo: "Không cho phép chỉ định dịch vụ vượt quá số tiền còn thừa" và không cho phép người dùng check chọn dịch vụ đó.
					if (this.patientTypeByPT != null && this.patientTypeByPT.IS_CHECK_FEE_WHEN_ASSIGN == 1
							&& this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
							&& this.transferTreatmentFee >= 0 && this.currentModule.RoomTypeId != IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TD
						)
					{
						HisSereServView17Filter filterSs = new HisSereServView17Filter();
						filterSs.TDL_TREATMENT_ID = treatmentId;
						var SereSerView17 = await new BackendAdapter(param).GetAsync<List<V_HIS_SERE_SERV_17>>("api/HisSereServ/GetView17", ApiConsumer.ApiConsumers.MosConsumer, filterSs, param);

						frmDetailsSereServ frm = new frmDetailsSereServ(SereSerView17.ToList(), (HIS.Desktop.Common.RefeshReference)this.Close);
						frm.ShowDialog();
						return;
					}


					if (treatmentFee.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
					{
						return;
					}

					if ((HisConfigCFG.WarningOverTotalPatientPrice__IsCheck == "1" || HisConfigCFG.WarningOverTotalPatientPrice__IsCheck == "3") && this.currentHisPatientTypeAlter != null && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU && !string.IsNullOrEmpty(HisConfigCFG.WarningOverTotalPatientPrice))
					{
						decimal warningOverTotalCGF = Convert.ToInt64(HisConfigCFG.WarningOverTotalPatientPrice);

						if (transferTreatmentFee > warningOverTotalCGF && this.transferTreatmentFeeBK != this.transferTreatmentFee)
						{
							if (MessageBox.Show(String.Format(ResourceMessage.BenhNhanDangThieuVienPhi, Inventec.Common.Number.Convert.NumberToString(transferTreatmentFee, ConfigApplications.NumberSeperator)), "Cảnh báo",
		MessageBoxButtons.YesNo, MessageBoxIcon.Question,
		MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
							{
								this.Close();
							}
						}
					}

					this.transferTreatmentFeeBK = this.transferTreatmentFee;//Phai thu benh nhan
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitDefaultFocus()
		{
			try
			{
				UcIcdFocusComtrol();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private async Task InitDefaultDataService()
		{
			try
			{
				LogSystem.Debug("frmAssignService => InitDefaultDataService .1");
				if (this.currentSereServ != null)
				{
					//Chỉ định từ màn hình xử lý PTTT            
					this.LoadDataByPackageService(this.currentSereServ);
					this.SereServADO__Main = this.ServiceAllADOs.FirstOrDefault(o => o.ID == this.currentSereServ.SERVICE_ID);
					LogSystem.Debug("frmAssignService_Load => Loaded CreateThreadLoadDataByPackageService");
				}
				else if (this.currentSereServInEkip != null)
				{
					//Chỉ định từ màn hình xử lý PTTT            
					this.LoadDataByPackageService(this.currentSereServInEkip);
					this.SereServADO__Main = ServiceAllADOs.FirstOrDefault(o => o.ID == this.currentSereServInEkip.SERVICE_ID);
					LogSystem.Debug("frmAssignService_Load => Loaded CreateThreadLoadDataByPackageService");
				}
				else if (this.previusTreatmentId > 0)
				{
					this.LoadPageServiceInAppointmentServices(this.previusTreatmentId);

				}
				LogSystem.Debug("frmAssignService => InitDefaultDataService .2");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		void DelegateSelectedIcd(HIS_ICD icdSelected)
		{
			try
			{
				//Bổ sung key cấu hình: 
				//"0 (hoặc ko khai báo): Không kiểm tra 
				//+ 1: Có kiểm tra dịch vụ đã yêu cầu có nằm trong danh sách đã được cấu hình tương ứng với ICD của bệnh nhân hay không. Nếu tồn tại dịch vụ không được cấu hình thì hiển thị thông báo và không cho lưu.
				//+ 2: Có kiểm tra, nhưng chỉ hiển thị cảnh báo, và hỏi "Bạn có muốn tiếp tục không". Nếu người dùng chọn "OK" thì vẫn cho phép lưu"


				if (HisConfigCFG.IcdServiceHasCheck != "1" && HisConfigCFG.IcdServiceHasCheck != "2" && HisConfigCFG.IcdServiceHasCheck != "3" && HisConfigCFG.IcdServiceHasCheck != "4" && HisConfigCFG.IcdServiceHasCheck != "5")
					return;

				List<HIS_ICD> icdFromUc = new List<HIS_ICD>();
				if (icdSelected != null)
				{
					icdFromUc.Add(icdSelected);
				}


				var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;
				if (subIcd != null)
				{
					string icd_sub_code = subIcd.ICD_SUB_CODE;
					if (!string.IsNullOrEmpty(icd_sub_code))
					{
						String[] icdCodes = icd_sub_code.Split(';');
						foreach (var item in icdCodes)
						{
							var icd = this.currentIcds.Where(o => o.IS_TRADITIONAL != 1 && o.ICD_CODE == item).FirstOrDefault();
							if (icd != null && (icdSelected == null || (icdSelected != null && icd.ICD_CODE != icdSelected.ICD_CODE)))
							{
								HIS_ICD icdSub = new HIS_ICD();
								icdSub.ID = icd != null ? icd.ID : 0;
								icdSub.ICD_NAME = icd != null ? icd.ICD_NAME : "";
								icdSub.ICD_CODE = icd != null ? icd.ICD_CODE : "";
								icdFromUc.Add(icdSub);
							}
						}
					}
				}

				if (icdFromUc != null && icdFromUc.Count > 0)
				{
					CommonParam param = new CommonParam();
					MOS.Filter.HisIcdServiceFilter filter = new HisIcdServiceFilter();
					filter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
					this.icdServicePhacDos = new BackendAdapter(param).Get<List<HIS_ICD_SERVICE>>(
												"api/HisIcdService/Get",

									   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
									   filter,
									   param);
				}
				else
				{
					this.icdServicePhacDos = null;
				}

				if (this.icdServicePhacDos != null && this.icdServicePhacDos.Count > 0)
				{
					Inventec.Common.Logging.LogSystem.Debug("DelegateSelectedIcd. 1. this.icdServicePhacDos.count=" + this.icdServicePhacDos.Count);
					//if ((bool)this.toggleSwitchDataChecked.EditValue == false)
					//{
					//    this.toggleSwitchDataChecked.EditValue = true;
					//}
					ProcessChoiceIcdPhacDo(this.icdServicePhacDos);
				}
				else
				{
					//if ((bool)this.toggleSwitchDataChecked.EditValue == true)
					//{
					//    this.toggleSwitchDataChecked.EditValue = false;
					//}
					this.ResetDefaultGridData();
				}
			}
			catch (Exception ex)
			{
				LogSystem.Warn(ex);
			}
		}

		private void ResetDefaultGridData()
		{
			try
			{
				Inventec.Common.Logging.LogSystem.Debug("ResetDefaultGridData. 1");
				this.gridViewServiceProcess.ActiveFilter.Clear();
				this.gridViewServiceProcess.ClearColumnsFilter();
				this.gridControlServiceProcess.DataSource = null;
				Inventec.Common.Logging.LogSystem.Debug("ResetDefaultGridData. 2");
				foreach (var item in this.ServiceIsleafADOs)
				{
					item.AMOUNT = 1;
					item.IsChecked = false;
					item.ShareCount = null;
					item.PATIENT_TYPE_ID = 0;
					item.PATIENT_TYPE_CODE = "";
					item.PATIENT_TYPE_NAME = "";
					item.PRICE = 0;
					item.TDL_EXECUTE_ROOM_ID = 0;
					item.IsExpend = false;
					item.IsOutKtcFee = false;
					item.IsKHBHYT = false;
					item.InstructionNote = "";
					item.SERVICE_GROUP_ID_SELECTEDs = null;
					item.IsNoDifference = false;
					item.ErrorMessageAmount = "";
					item.ErrorMessageIsAssignDay = "";
					item.ErrorMessagePatientTypeId = "";
					item.AssignPackagePriceEdit = null;
					item.AssignSurgPriceEdit = null;
					item.ErrorTypeAmount = ErrorType.None;
					item.ErrorTypeIsAssignDay = ErrorType.None;
					item.ErrorTypePatientTypeId = ErrorType.None;
					item.PRIMARY_PATIENT_TYPE_ID = null;
					item.IsNotChangePrimaryPaty = false;
					item.BedFinishTime = null;
					item.BedId = null;
					item.BedStartTime = null;
                    item.SereServEkipADO = null;
                }

				var allDatas = this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0 ? this.ServiceIsleafADOs.AsQueryable() : null;
				this.gridControlServiceProcess.DataSource = allDatas.ToList();
				this.toggleSwitchDataChecked.EditValue = false;
				isCheckAssignServiceSimultaneityOption = false;
				this.SetEnableButtonControl(this.actionType);
				Inventec.Common.Logging.LogSystem.Debug("ResetDefaultGridData. 3");
			}
			catch (Exception ex)
			{
				LogSystem.Warn(ex);
			}
		}

		#endregion

		#region Control editor
		private void chkIsEmergency_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					chkIsInformResultBySms.Focus();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkIsEmergency_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (chkIsEmergency.CheckState == CheckState.Checked)
				{
					chkIsNotRequireFee.Enabled = true;
					chkIsNotRequireFee.CheckState = CheckState.Checked;
				}
				else
				{
					chkIsNotRequireFee.Enabled = false;
					chkIsNotRequireFee.CheckState = CheckState.Unchecked;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Down)
				{
					var rowCount = (this.gridViewServiceProcess.DataSource as List<SereServADO>).Count();
					if (rowCount == 1)
					{
						this.gridViewServiceProcess.Focus();
						this.gridViewServiceProcess.FocusedRowHandle = 0;
					}
					else if (rowCount > 1)
					{
						this.gridViewServiceProcess.Focus();
						this.gridViewServiceProcess.FocusedRowHandle = 1;
					}
					else
					{
						//Nothing
					}
				}
				else if (e.KeyCode == Keys.Enter)
				{
					var rowCount = (this.gridViewServiceProcess.DataSource as List<SereServADO>).Count();
					if (rowCount > 0)
					{
						var rowUpdate = this.gridViewServiceProcess.GetFocusedRow() as SereServADO;
						rowUpdate.IsChecked = true;
					}
				}
				else
				{
					this.LoadDataToGrid(false);
				}
			}
			catch (Exception ex)
			{
				LogSystem.Warn(ex);
			}
		}

		private void btnUnExpendAll_Click(object sender, EventArgs e)
		{
			try
			{
				this.treeService.CollapseAll();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnExpendAll_Click(object sender, EventArgs e)
		{
			try
			{
				IscheckAllTreeService = true;
				this.treeService.ExpandAll();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void treeService_Click(object sender, EventArgs e)
		{
			try
			{

				this.isHandlerResetServiceStateCheckedForTreeNodes = false;

				if (this.isProcessingAfternodeChecked)
				{
					//Inventec.Common.Logging.LogSystem.Debug("isProcessingAfternodeChecked = true");
					this.isProcessingAfternodeChecked = false;
					return;
				}
				this.isNotEventByChangeServiceParent = true;
				this.IscheckAllTreeService = false;
				bool isChangeParentNode = false;
				if (!Switch_ExpendAll.IsOn)
					this.UnCheckOtherParentChecked(ref isChangeParentNode);
				if (this.treeService.FocusedNode != null)
				{
					//Process expand node
					var parent = this.treeService.FocusedNode.ParentNode;
					//Trường hợp node đang chọn có cha
					if (parent != null)
					{
						if (!Switch_ExpendAll.IsOn)
							this.ProcessExpandTree(this.treeService.FocusedNode);
						else
							if (HisConfigCFG.IsSingleCheckservice == commonString__true)
						{
							this.treeService.UncheckAll();
						}
						//Inventec.Common.Logging.LogSystem.Debug("treeService_Click.3");
					}
					//Trường hợp node đang chọn không có cha
					else
					{
						//Inventec.Common.Logging.LogSystem.Debug("treeService_Click.4");
						if (!Switch_ExpendAll.IsOn)
							this.treeService.CollapseAll();

						this.treeService.FocusedNode.Expanded = true;
						bool checkState = this.treeService.FocusedNode.Checked;

						if (HisConfigCFG.IsSingleCheckservice == commonString__true && !Switch_ExpendAll.IsOn)
						{
							this.treeService.UncheckAll();
						}

						if (checkState)
							this.treeService.FocusedNode.CheckAll();
						//Inventec.Common.Logging.LogSystem.Debug("treeService_Click.5");
					}

					//Process check state node is leaf
					var data = this.treeService.GetDataRecordByNode(this.treeService.FocusedNode);
					this.isHandlerResetServiceStateCheckeds = (data != null && data is ServiceADO && ((ServiceADO)data).IsParentServiceId == true);
					if (this.isHandlerResetServiceStateCheckeds || this.isHandlerResetServiceStateCheckedForTreeNodes)
					{
						this.ResetServiceGroupSelected();
						this.ResetServiceKskSelected();
					}
					if (this.treeService.FocusedNode != null
						&& !this.treeService.FocusedNode.HasChildren
						&& data != null
						&& data is ServiceADO)
					{
						//Cấu hình cho phép chọn một/nhiều nhóm dịch vụ trên cây là node lá
						//Nếu không có cấu hình thì mặc định là chọn nhiều
						//Nếu có cấu hình thì xử lý theo cấu hình
						if (HisConfigCFG.IsSingleCheckservice == commonString__true)
						{
							if (parent != null)
							{
								parent.UncheckAll();
							}
							this.treeService.FocusedNode.Checked = true;
						}
						else
						{
							this.treeService.FocusedNode.Checked = (this.treeService.FocusedNode.CheckState == CheckState.Checked ? false : true);
						}
					}
					//Inventec.Common.Logging.LogSystem.Debug("treeService_Click.6");
					this.isNotHandlerWhileChangeToggetSwith = true;
					this.toggleSwitchDataChecked.EditValue = false;
					if (!this.treeService.FocusedNode.HasChildren || isChangeParentNode)
					{
						this.LoadDataToGrid(true);
						//Inventec.Common.Logging.LogSystem.Debug("treeService_Click.7");
						this.SetDefaultSerServTotalPrice();
						this.SetEnableButtonControl(this.actionType);
						//Inventec.Common.Logging.LogSystem.Debug("treeService_Click.8");
					}
					this.isNotHandlerWhileChangeToggetSwith = false;
					this.isNotEventByChangeServiceParent = false;
					//Inventec.Common.Logging.LogSystem.Debug("treeService_Click.9");
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void UnCheckOtherParentChecked(ref bool isChangeParentNode)
		{
			try
			{
				//Inventec.Common.Logging.LogSystem.Debug("UnCheckOtherParentChecked.1");
				if (HisConfigCFG.IsSingleCheckservice != commonString__true)
				{
					// Nếu đang chọn huyết học của xét nghiệm mà bấm vào chẩn đoán hình ảnh (check ô vuông) 
					// thì chưa hủy tick huyết học => MM: tự động mở cây của chẩn đoán hình ảnh và tick các dịch vụ con 
					// của chẩn đoán hình ảnh đồng thời hủy tick huyết học, thu lại cây xét nghiệm.
					List<TreeListNode> allParentNodes = new List<TreeListNode>();
					foreach (TreeListNode treeListNode in treeService.GetAllCheckedNodes())
					{
						if (treeListNode.RootNode != this.treeService.FocusedNode.RootNode)
						{
							if (!allParentNodes.Contains(treeListNode.RootNode))
							{
								isChangeParentNode = true;
								allParentNodes.Add(treeListNode.RootNode);
								treeListNode.RootNode.UncheckAll();
							}
						}
					}

					//if (allParentNodes != null && allParentNodes.Count > 0)
					//{
					//    foreach (var itemNode in allParentNodes)
					//    {
					//        itemNode.CheckState = CheckState.Unchecked;
					//        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemNode.Id), itemNode.Id));
					//    }
					//}
					//Inventec.Common.Logging.LogSystem.Debug("UnCheckOtherParentChecked.2");
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ProcessExpandTree(TreeListNode focusedNode)
		{
			try
			{
				TreeListNode parent = focusedNode.ParentNode;
				//bool checkState = treeService.FocusedNode.Checked;
				if (parent != null)
				{
					this.treeService.CollapseAll();
					List<TreeListNode> allParentNodes = new List<TreeListNode>();
					this.GetParent(focusedNode, allParentNodes);
					if (allParentNodes != null && allParentNodes.Count > 0)
					{
						var nodes = this.treeService.GetNodeList();
						foreach (var item in nodes)
						{
							//item.Checked = false;
							if (focusedNode == item)
							{
								focusedNode.Expanded = true;
								//var childNodes = nodes.Where(o => o.ParentNode == focusedNode).ToList();
								//if (childNodes != null && childNodes.Count > 0)
								//{
								//    foreach (var childOfChild in childNodes)
								//    {
								//        //childOfChild.Expanded = true;
								//    }
								//}
							}
							else if (allParentNodes.ToArray().Contains(item))
							{
								item.Expanded = true;
							}
							else
							{
								item.Expanded = false;
							}
						}
					}

					//treeService.FocusedNode.Expanded = true;

					//treeService.UncheckAll();
					//if (checkState)
					//    treeService.FocusedNode.CheckAll();
				}
				this.treeService.FocusedNode = focusedNode;
				//parent.ExpandAll();
				//else
				//{
				//    treeService.CollapseAll();
				//}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void GetParent(TreeListNode focusedNode, List<TreeListNode> parentNodes)
		{
			try
			{
				if (focusedNode != null && focusedNode.ParentNode != null && !parentNodes.Contains(focusedNode.ParentNode))
				{
					parentNodes.Add(focusedNode.ParentNode);
					this.GetParent(focusedNode.ParentNode, parentNodes);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void treeService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
		{
			try
			{
				var data = this.treeService.GetDataRecordByNode(e.Node);
				if (data != null && data is ServiceADO)
				{
					var noteData = (HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO)data;
					if (String.IsNullOrEmpty(noteData.PARENT_ID__IN_SETY) && noteData.ID == 0)
					{
						e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}


		/// trường hợp cấu hình chỉ tích 1 cái:
		///+ tích vào ô vuông:  Chọn tất cả dịch vụ thuộc loại dịch vụ và các dịch vụ khác là con của loại dịch vụ đang được chọn
		///+ tích vào tên loại dịch vụ: Mở các dịch vụ cấp 2 là con của loại dịch vụ (nhưng ko tick bất kì dv nào) đồng thời đóng hết các cây dịch vụ khác đang mở (và bỏ check)
		///
		///Cấp 2:
		///- Bấm chuột vào tên dv cấp 2 (hoặc tick ô vuông): Load tất cả dịch vụ là con của dv cấp 2 và các dịch vụ cấp 3 và con của cấp 3
		///- Chọn dịch vụ cấp 2 khác: Hủy tick dv cấp 2 đã chọn trước đó. chỉ hiển thị dv con của dịch vụ vừa được chọn.                
		private void treeService_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
		{
			try
			{
				//Inventec.Common.Logging.LogSystem.Debug("treeService_AfterCheckNode.1");
				var data = this.treeService.GetDataRecordByNode(e.Node) as ServiceADO;
				bool isChangeParentNode = false;
				if (!Switch_ExpendAll.IsOn)
					this.UnCheckOtherParentChecked(ref isChangeParentNode);
				this.isNotEventByChangeServiceParent = true;
				this.isNotHandlerWhileChangeToggetSwith = true;
				this.toggleSwitchDataChecked.EditValue = false;
				this.isHandlerResetServiceStateCheckeds = (data != null && data is ServiceADO && ((ServiceADO)data).IsParentServiceId == true);
				if (this.isHandlerResetServiceStateCheckeds || this.isHandlerResetServiceStateCheckedForTreeNodes)
				{
					this.ResetServiceGroupSelected();
					this.ResetServiceKskSelected();
				}
				this.LoadDataToGrid(true);
				//Inventec.Common.Logging.LogSystem.Debug("treeService_AfterCheckNode.4");
				this.SetDefaultSerServTotalPrice();
				this.SetEnableButtonControl(this.actionType);
				this.isProcessingAfternodeChecked = true;
				this.isNotHandlerWhileChangeToggetSwith = false;
				this.isNotEventByChangeServiceParent = false;
				//Inventec.Common.Logging.LogSystem.Debug("treeService_AfterCheckNode.5");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void treeService_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
		{
			try
			{
				//issue 13991
				this.isHandlerResetServiceStateCheckedForTreeNodes = false;
				if (HisConfigCFG.IsSingleCheckservice == commonString__true)
				{
					this.treeService.UncheckAll();
				}
				e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
				this.treeService.FocusedNode = e.Node;
				//Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => e.State), e.State));
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void treeService_NodeChanged(object sender, DevExpress.XtraTreeList.NodeChangedEventArgs e)
		{
			try
			{
				var dataRecord = this.treeService.GetDataRecordByNode(e.Node) as ServiceADO;
				if (dataRecord != null && dataRecord.IsParentServiceId == true)
				{
					//Inventec.Common.Logging.LogSystem.Debug("treeService_NodeChanged.1");
					this.isHandlerResetServiceStateCheckedForTreeNodes = true;
					//Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataRecord), dataRecord));
					//Inventec.Common.Logging.LogSystem.Debug("treeService_NodeChanged.2");
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void treeService_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (this.treeService.FocusedNode != null)
					{
						this.treeService.FocusedNode.Checked = true;
						this.gridControlServiceProcess.Focus();
						this.gridViewServiceProcess.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;
					}
				}
				else if (e.KeyCode == Keys.Space)
				{
					var node = this.treeService.FocusedNode;
					var data = this.treeService.GetDataRecordByNode(node);
					if (node != null && node.HasChildren && data != null && data is ServiceADO)
					{
						node.Expanded = !node.Expanded;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void treeService_BeforeExpand(object sender, DevExpress.XtraTreeList.BeforeExpandEventArgs e)
		{
			try
			{
				if (e.Node != null && !IscheckAllTreeService)
				{
					//Process expand node
					var parent = e.Node.ParentNode;
					//Trường hợp node đang chọn có cha
					if (parent != null)
					{
						this.ProcessExpandTree(e.Node);
					}
					//Trường hợp node đang chọn không có cha
					else
					{
						if (!Switch_ExpendAll.IsOn)
						{
							this.treeService.CollapseAll();
						}

						e.Node.Expanded = true;
					}
					// bỏ focus node tránh trường hợp sang hàm click tree
					this.treeService.FocusedNode = e.Node;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void toggleSwitchDataChecked_Toggled(object sender, EventArgs e)
		{
			try
			{

				lblCaptionFortoggleSwitchDataChecked.Text = ((this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() != "true") ? toggleSwitchDataChecked.Properties.OffText : toggleSwitchDataChecked.Properties.OnText;
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lblCaptionFortoggleSwitchDataChecked.Text", lblCaptionFortoggleSwitchDataChecked.Text));
				if (isNotHandlerWhileChangeToggetSwith)
				{
					//isNotHandlerWhileChangeToggetSwith = false;
					return;
				}

				//Inventec.Common.Logging.LogSystem.Debug("toggleSwitchDataChecked_Toggled. 1");
				ToggleSwitch toggleSwitch = sender as ToggleSwitch;
				if (toggleSwitch != null)
				{
					//Inventec.Common.Logging.LogSystem.Debug("toggleSwitchDataChecked_Toggled. 2");
					WaitingManager.Show();
					this.LoadDataToGrid(false);
					WaitingManager.Hide();
					//Inventec.Common.Logging.LogSystem.Debug("toggleSwitchDataChecked_Toggled. 3");
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ProcessNoDifferenceHeinServicePrice(SereServADO sereServADO)
		{
			try
			{

				bool finded = false;
				if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
					&& HisConfigCFG.NoDifference == commonString__true)
				{
					var headCards = !String.IsNullOrEmpty(HisConfigCFG.HeadCardNumberNoDifference) ? HisConfigCFG.HeadCardNumberNoDifference.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !String.IsNullOrEmpty(o.Trim())).ToList() : null;
					if ((headCards != null && !String.IsNullOrEmpty(this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER) && headCards.Where(o => this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER.StartsWith(o.Trim())).Any())
						)
					{
						sereServADO.IsNoDifference = true;
						finded = true;
					}

					var departmentCodes = !String.IsNullOrEmpty(HisConfigCFG.DepartmentCodeNoDifference) ? HisConfigCFG.DepartmentCodeNoDifference.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !String.IsNullOrEmpty(o.Trim())).ToList() : null;
					if (departmentCodes != null && departmentCodes.Contains(this.requestRoom.DEPARTMENT_CODE))
					{
						sereServADO.IsNoDifference = true;
						finded = true;
					}
					//IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.
					var heinService = lstService.FirstOrDefault(o => o.ID == sereServADO.SERVICE_ID);
					if (heinService != null)
					{
						sereServADO.HEIN_LIMIT_PRICE = heinService.HEIN_LIMIT_PRICE;
					}

					if (!finded)
					{
						sereServADO.IsNoDifference = false;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ResetOneService(SereServADO item)
		{
			try
			{
				item.PATIENT_TYPE_ID = 0;
				item.PATIENT_TYPE_CODE = null;
				item.PATIENT_TYPE_NAME = null;
				item.AssignNumOrder = null;
				item.TDL_EXECUTE_ROOM_ID = 0;
				item.IsNotLoadDefaultPatientType = false;
				item.IsContainAppliedPatientType = false;
				item.SERVICE_CONDITION_ID = null;
				item.SERVICE_CONDITION_NAME = null;
				item.OTHER_PAY_SOURCE_ID = null;
				item.OTHER_PAY_SOURCE_CODE = "";
				item.OTHER_PAY_SOURCE_NAME = "";
				item.IsNotChangePrimaryPaty = false;
				item.IsExpend = false;
				item.IsServiceKsk = false;
				item.IsNoDifference = false;
				item.PRIMARY_PATIENT_TYPE_ID = null;
				item.ErrorMessageAmount = "";
				item.ErrorTypeAmount = ErrorType.None;
				item.ErrorMessagePatientTypeId = "";
				item.ErrorTypePatientTypeId = ErrorType.None;
				item.ErrorMessageIsAssignDay = "";
				item.ErrorTypeIsAssignDay = ErrorType.None;
				item.IsNotUseBhyt = false;
				item.TEST_SAMPLE_TYPE_ID = 0;
				item.TEST_SAMPLE_TYPE_CODE = null;
				item.TEST_SAMPLE_TYPE_NAME = null;
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}

		private void gridViewServiceProcess_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
		{
			try
			{
				if (e.RowHandle >= 0)
				{
					SereServADO data = (SereServADO)gridViewServiceProcess.GetRow(e.RowHandle);
					if (e.Column.FieldName == "PATIENT_TYPE_ID")
					{
						if (data != null && data.PackagePriceId.HasValue)
							e.RepositoryItem = this.repositoryItemCboPatientTypeReadOnly;
						else
							e.RepositoryItem = this.repositoryItemcboPatientType_TabService;
					}
					else if (e.Column.FieldName == "PRIMARY_PATIENT_TYPE_ID")
					{
						if (data != null && (data.PackagePriceId.HasValue || data.IsNotChangePrimaryPaty))
							e.RepositoryItem = this.repositoryItemCboPatientTypeReadOnly;
						else
							e.RepositoryItem = this.repositoryItemCboPrimaryPatientType;
					}
					else if (e.Column.FieldName == "IsChecked")
					{
						if (data != null && data.PackagePriceId.HasValue)
							e.RepositoryItem = this.repositoryItemchkIsCheckedDisable;
						else
							e.RepositoryItem = this.repositoryItemchkIsChecked;
						//e.RepositoryItem = (isAllowChecked == (short)1 ? this.repositoryItemchkIsChecked : this.repositoryItemchkIsCheckedDisable);//TODO
					}
					else if (e.Column.FieldName == "IsExpend")
					{
						short isAllowExpend = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewServiceProcess.GetRowCellValue(e.RowHandle, "IsAllowExpend") ?? "").ToString());
						if (isAllowExpend == (short)1 && !(data != null && data.PackagePriceId.HasValue))
							e.RepositoryItem = this.repositoryItemChkIsExpend_TabService;
						else
						{
							e.RepositoryItem = this.ButtonEdit_IsExpenDisable;
						}
					}
					else if (e.Column.FieldName == "IsKHBHYT")
					{
						if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
						{
							e.RepositoryItem = this.repositoryItemChkIsKH_TabService;
						}
					}
					else if (e.Column.FieldName == "AMOUNT")
					{
						short isMultiRequest = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewServiceProcess.GetRowCellValue(e.RowHandle, "IS_MULTI_REQUEST") ?? "").ToString());
						if (isMultiRequest == 1 && !(data != null && data.PackagePriceId.HasValue))
						{
							e.RepositoryItem = this.repositoryItemSpinAmount_TabService;
						}
						else
						{
							e.RepositoryItem = this.repositoryItemSpinAmount__Disable_TabService;
						}
					}
					else if (e.Column.FieldName == "PRICE_DISPLAY")
					{
						//#15544
						//- Chỉ hiển thị icon này, nếu dịch vụ là phẫu thuật, hoặc dịch vụ có cấu hình "Gói dịch vụ" (có package_id), và:
						//+ Khoa người dùng làm việc có cấu hình "Cho phép chỉ định giá phẫu thuật" --> hiển thị icon "sửa" ở ô "Giá" 
						//+ Khoa người dùng làm việc có cấu hình "Cho phép chỉ định giá gói" --> hiển thị icon "sửa" ở ô "Giá gói" 
						//Lưu ý: chỉ cho sửa 1 trong 2 trường ("giá" hoặc "giá gói"), chứ ko cho phép sửa cả 2. Ưu tiên "giá gói"
						//Lưu ý: Ko cho sửa nếu ĐTTT hoặc đối tượng phụ thu là BHYT

						if (data.IsServiceKsk && data.PATIENT_TYPE_CODE == PatientKskCode)
						{
							e.RepositoryItem = repositoryItembtnEditDonGia_TextDisable;
						}
						else
						{
							bool isEditCtrol = data.IS_ENABLE_ASSIGN_PRICE == 1;
							isEditCtrol = isEditCtrol && IsAllowShowEditPrice(e.RowHandle);
							//isEditCtrol = isEditCtrol && IsAllowEditSurgeryPrice(e.RowHandle);
							if (data != null && (data.PACKAGE_ID.HasValue || data.PackagePriceId.HasValue || !isEditCtrol))
								e.RepositoryItem = repositoryItemTxtReadOnly;
							else
								e.RepositoryItem = repositoryItembtnEditDonGia_TextDisable;
						}
					}
					else if (e.Column.FieldName == "PRICE_PRPO_DISPLAY")
					{
						//#15544
						//- Chỉ hiển thị icon này, nếu dịch vụ là phẫu thuật, hoặc dịch vụ có cấu hình "Gói dịch vụ" (có package_id), và:
						//+ Khoa người dùng làm việc có cấu hình "Cho phép chỉ định giá phẫu thuật" --> hiển thị icon "sửa" ở ô "Giá" 
						//+ Khoa người dùng làm việc có cấu hình "Cho phép chỉ định giá gói" --> hiển thị icon "sửa" ở ô "Giá gói"   
						bool isEditCtrol = data.IS_ENABLE_ASSIGN_PRICE == 1;//bool isEditCtrol = SereServCFG.AllowAssignPrice;
						isEditCtrol = isEditCtrol && IsAllowShowEditPrice(e.RowHandle);
						//isEditCtrol = isEditCtrol && IsAllowEditPackagePrice(e.RowHandle);
						if (data != null && (!data.PACKAGE_ID.HasValue || data.PackagePriceId.HasValue || !isEditCtrol))
							e.RepositoryItem = this.repositoryItemTxtReadOnly;
						else
							e.RepositoryItem = repositoryItembtnEditGiaGoi_TextDisable;
					}
					else if (e.Column.FieldName == "TDL_EXECUTE_ROOM_ID")
					{
						if (this.IsTreatmentInBedRoom && data != null && data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && data.TDL_EXECUTE_ROOM_ID > 0)
						{
							var room = currentExecuteRooms.FirstOrDefault(o => o.ROOM_ID == data.TDL_EXECUTE_ROOM_ID);
							if (room != null)
							{
								e.RepositoryItem = this.repositoryItemcboExcuteRoom_TabService;
							}
							else
							{
								e.RepositoryItem = this.repositoryItemcboExcuteRoomPlus_TabService;
							}
						}
						else
						{
							e.RepositoryItem = this.repositoryItemcboExcuteRoom_TabService;
						}
					}
					//else if (e.Column.FieldName == "SERVICE_CONDITION_ID")
					//{
					//    e.RepositoryItem = this.repositoryItemcboCondition;
					//}
					else if (e.Column.FieldName == "IsOutKtcFee")
					{
						if (data != null && data.PackagePriceId.HasValue)
							e.RepositoryItem = this.repositoryItemchkIsCheckedDisable;
						else
							e.RepositoryItem = this.repositoryItemChkIsOutKtcFee_Enable_TabService;
					}
					else if (e.Column.FieldName == "ShareCount")
					{
						long serviceReqTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceProcess.GetRowCellValue(e.RowHandle, "TDL_SERVICE_TYPE_ID") ?? "").ToString());
						e.RepositoryItem = (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G ? this.repositoryItemcboShareCount : this.repositoryItemTxtReadOnly);
						//Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqTypeId), serviceReqTypeId) + "____" + Inventec.Common.Logging.LogUtil.TraceData("IMSys.DbConfig.HIS_RS.TDL_SERVICE_TYPE_ID.ID__G", IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G));
					}
					else if (e.Column.FieldName == "IsNotUseBhyt")
					{
						if (data != null && (!data.IsChecked || data.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT))
							e.RepositoryItem = this.repositoryItemchkIsCheckedDisable;
						else
							e.RepositoryItem = this.repositoryItemCheckEditIsNotUseBhyt;
					}
					else if (e.Column.FieldName == "EKIP_TEMP")
					{
						if (data != null && data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && workingAssignServiceADO.OpenFromBedRoomPartial)
							e.RepositoryItem = this.repositoryItemButtonEkipTempEn;
						else
							e.RepositoryItem = this.repositoryItemButtonEkipTempDis;
					}
					else if (e.Column.FieldName == "TEST_SAMPLE_TYPE_ID")
					{
						if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && data.SERVICE_TYPE_ID > 0 && serviceTypeIdSplitReq != null && serviceTypeIdSplitReq.Count > 0 && serviceTypeIdSplitReq.Exists(o => o == data.SERVICE_TYPE_ID))
						{
							e.RepositoryItem = this.repSampleType;
						}
						else
						{
							e.RepositoryItem = this.repSampleTypeDis;
						}
					}
					else if(e.Column.FieldName == "AssignNumOrder")
					{
						if (data.IsChecked && !assignMulti)
							e.RepositoryItem = this.repositoryItemSpinAmount_TabService;
						else
							e.RepositoryItem = this.repositoryItemSpinAmount__Disable_TabService;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewServiceProcess_CellValueChanged(object sender, CellValueChangedEventArgs e)
		{
			try
			{
				var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
				if (sereServADO != null)
				{
					if (e.Column.FieldName == this.grcChecked_TabService.FieldName && sereServADO.IsChecked)
					{
						if (lstSereServExist != null && lstSereServExist.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID) != null && DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dịch vụ có thời gian chỉ định nằm trong khoảng thời gian thiết lập của phác đồ điều trị. Thời gian chỉ định {0} (mã y lệnh: {1}). Bạn có muốn tiếp tục?", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(lstSereServExist.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID).TDL_INTRUCTION_TIME), lstSereServExist.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID).TDL_SERVICE_REQ_CODE), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
						{
							sereServADO.IsChecked = false;
						}
						if (sereServADO.IsChecked)
						{
							if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
							{
								MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
							}
							ValidOnlyShowNoticeService(sereServADO);
						}
					}
					if (e.Column.FieldName == this.grcSampleType.FieldName)
					{
						if (sereServADO.IsChecked && sereServADO.TEST_SAMPLE_TYPE_ID > 0)
						{
							var sampleType = dataListTestSampleType.FirstOrDefault(o => o.ID == sereServADO.TEST_SAMPLE_TYPE_ID);
							if (sampleType != null)
							{
								sereServADO.TEST_SAMPLE_TYPE_CODE = sampleType.TEST_SAMPLE_TYPE_CODE;
								sereServADO.TEST_SAMPLE_TYPE_NAME = sampleType.TEST_SAMPLE_TYPE_NAME;
							}
						}
						else
						{
							sereServADO.TEST_SAMPLE_TYPE_ID = 0;
							sereServADO.TEST_SAMPLE_TYPE_CODE = null;
							sereServADO.TEST_SAMPLE_TYPE_NAME = null;
						}
						this.gridControlServiceProcess.RefreshDataSource();
						this.SetEnableButtonControl(this.actionType);
					}
					if(e.Column.FieldName == this.grcChecked_TabService.FieldName)
					{
						if (sereServADO.IsChecked)
							this.SetAssignNumOrder(sereServADO);
					}
					if (e.Column.FieldName == this.grcChecked_TabService.FieldName
						|| e.Column.FieldName == this.grcExpend_TabService.FieldName
						|| e.Column.FieldName == this.grcAmount_TabService.FieldName
						|| e.Column.FieldName == this.gridColumnPatientTypeName__TabService.FieldName
						|| e.Column.FieldName == this.gridColumnExecuteRoomName__TabService.FieldName
						|| e.Column.FieldName == this.grcSampleType.FieldName
						)
					{
						if (sereServADO.IsChecked)
						{
							//Phân biệt giá trị TEST_SAMPLE_TYPE_CODE mặc định bởi TEST_SAMPLE_TYPE_ID = 0;
							if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && sereServADO.SERVICE_TYPE_ID > 0 && serviceTypeIdSplitReq != null && serviceTypeIdSplitReq.Count > 0 && serviceTypeIdSplitReq.Exists(o => o == sereServADO.SERVICE_TYPE_ID))
							{
								if (dataListTestSampleType != null && dataListTestSampleType.Count > 0 && sereServADO.TEST_SAMPLE_TYPE_ID == 0 && !string.IsNullOrEmpty(sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT))
								{
									var sampleType = dataListTestSampleType.FirstOrDefault(o => o.TEST_SAMPLE_TYPE_CODE == sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT);
									if (sampleType != null)
									{
										sereServADO.TEST_SAMPLE_TYPE_ID = sampleType.ID;
										sereServADO.TEST_SAMPLE_TYPE_CODE = sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT;
										sereServADO.TEST_SAMPLE_TYPE_NAME = sampleType.TEST_SAMPLE_TYPE_NAME;
									}
								}
							}
							bool isNotChange = (e.Column.FieldName == this.grcAmount_TabService.FieldName || e.Column.FieldName == this.gridColumnExecuteRoomName__TabService.FieldName);
							if (sereServADO.PATIENT_TYPE_ID > 0)
							{
								this.ChoosePatientTypeDefaultlService(sereServADO.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO, isNotChange, null, true);
							}
							else
							{
								this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO, isNotChange);
							}

							if (sereServADO.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && sereServADO.IsNotUseBhyt)
							{
								if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn đã tích chọn \"Không hưởng BHYT\", nếu đổi đối tượng sang BHYT, phần mềm sẽ tự động bỏ chọn. Bạn có muốn thực hiện không?", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
								{
									sereServADO.IsNotUseBhyt = false;
								}
								else
								{
									sereServADO.PATIENT_TYPE_ID = sereServADO.OldPatientType;
									gridViewServiceProcess.FocusedColumn = gridColumnPatientTypeName__TabService;
								}
							}
							sereServADO.OldPatientType = sereServADO.PATIENT_TYPE_ID;
							this.FillDataOtherPaySourceDataRow(sereServADO);

							if (e.Column.FieldName == this.gridColumnPatientTypeName__TabService.FieldName || e.Column.FieldName == this.grcChecked_TabService.FieldName)
							{
								List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
								FilterExecuteRoom(sereServADO, ref executeRoomList);
								long executeRoomId = this.SetPriorityRequired(executeRoomList);
								if (executeRoomId <= 0)
									executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);
								//if (sereServADO.TDL_EXECUTE_ROOM_ID <= 0 && executeRoomId > 0)
								//{
								sereServADO.TDL_EXECUTE_ROOM_ID = executeRoomId;
								//}
								sereServADO.SERVICE_CONDITION_ID = null;
								sereServADO.SERVICE_CONDITION_NAME = null;

                            }
							this.VerifyWarningOverCeiling();
							this.ValidServiceDetailProcessing(sereServADO);
							this.ProcessNoDifferenceHeinServicePrice(sereServADO);

							if (this.selectedSeviceGroups != null && this.selectedSeviceGroups.Count > 0
								&& sereServADO.SERVICE_GROUP_ID_SELECTEDs != null && sereServADO.SERVICE_GROUP_ID_SELECTEDs.Count > 0
								&& this.serviceDeleteWhileSelectSeviceGroups != null && this.serviceDeleteWhileSelectSeviceGroups.Count > 0)
							{
								var svRemove = this.serviceDeleteWhileSelectSeviceGroups.FirstOrDefault(k => k.SERVICE_ID == sereServADO.SERVICE_ID);
								if (svRemove != null)
								{
									this.serviceDeleteWhileSelectSeviceGroups.Remove(svRemove);
								}
							}

							if (!VerifyCheckFeeWhileAssign())
							{
								this.ResetOneService(sereServADO);
								sereServADO.IsChecked = false;
								return;
							}

							if (sereServADO.IsAutoExpend == (short?)1 && sereServADO.IsAllowExpend == (short?)1 && !sereServADO.PackagePriceId.HasValue)
								sereServADO.IsExpend = true;

							if (e.Column.FieldName == this.grcChecked_TabService.FieldName)
							{
								try
								{
									var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(sereServADO.SERVICE_ID, new List<long> { (sereServADO.PATIENT_TYPE_ID > 0 ? sereServADO.PATIENT_TYPE_ID : this.currentHisPatientTypeAlter != null ? this.currentHisPatientTypeAlter.PATIENT_TYPE_ID : 0) });

                                    long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
									List<V_HIS_SERVICE_PATY> dataSource = new List<V_HIS_SERVICE_PATY>(); 
									long? intructionNumByType = null;
                                    List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == sereServADO.SERVICE_TYPE_ID).ToList() : null;
                                    List<HIS_SERE_SERV> sameService = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.SERVICE_ID == sereServADO.SERVICE_ID).ToList() : null;
                                    intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                                    var intructionNum = sameService != null ? (long)sameService.Count() + 1 : 1;
                                    foreach (var con in dataCondition)
                                    {
                                        var dt = MOS.ServicePaty.ServicePatyUtil.GetApplied(new List<V_HIS_SERVICE_PATY>() { con }, sereServADO.TDL_EXECUTE_BRANCH_ID, sereServADO.TDL_EXECUTE_ROOM_ID, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, sereServADO.SERVICE_ID, sereServADO.PATIENT_TYPE_ID, intructionNum, intructionNumByType, sereServADO.PackagePriceId, con.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                                        if (dt != null)
                                            dataSource.Add(dt);
                                    }
									dataCondition = dataSource;
                                    if (dataCondition != null && dataCondition.Count > 0)
									{
										dataCondition = dataCondition.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_CONDITION_ID.HasValue && o.SERVICE_CONDITION_ID > 0 && o.SERVICE_ID == sereServADO.SERVICE_ID).ToList();
										if (dataCondition != null && dataCondition.Count > 0)
										{
											List<V_HIS_SERVICE_PATY> dataConditionTmps = new List<V_HIS_SERVICE_PATY>();
											foreach (var item in dataCondition)
											{
												if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_NAME == item.SERVICE_CONDITION_NAME && t.HEIN_RATIO == item.HEIN_RATIO))
												{
													dataConditionTmps.Add(item);
												}
											}
											dataCondition.Clear();
											dataCondition.AddRange(dataConditionTmps);
											GridViewInfo info = gridViewServiceProcess.GetViewInfo() as GridViewInfo;
											GridCellInfo cellInfo = info.GetGridCellInfo(gridViewServiceProcess.FocusedRowHandle, gridColumnSERVICE_CONDITION_NAME);
											//TODO
											Rectangle buttonPosition = cellInfo != null ? cellInfo.Bounds : default(Rectangle);
											popupControlContainerCondition.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 170));
											if (dataCondition != null && dataCondition.Count > 0 && lstConditionService != null && lstConditionService.Count > 0)
											{
												dataCondition = dataCondition.Where(o => lstConditionService.Exists(p => p.SERVICE_ID == sereServADO.SERVICE_ID && p.ID == o.SERVICE_CONDITION_ID)).ToList();
											}
											gridControlCondition.DataSource = null;
											gridControlCondition.DataSource = dataCondition;
											gridControlCondition.Focus();
											gridViewCondition.FocusedRowHandle = 0;
										}
									}
								}
								catch (Exception exx)
								{
									Inventec.Common.Logging.LogSystem.Warn(exx);
								}
							}
						}
						else
						{
							if (this.selectedSeviceGroups != null && this.selectedSeviceGroups.Count > 0 && sereServADO.SERVICE_GROUP_ID_SELECTEDs != null && sereServADO.SERVICE_GROUP_ID_SELECTEDs.Count > 0)
							{
								if (this.serviceDeleteWhileSelectSeviceGroups == null)
								{
									this.serviceDeleteWhileSelectSeviceGroups = new List<SereServADO>();
								}
								this.serviceDeleteWhileSelectSeviceGroups.Add(sereServADO);
							}
							else
							{
								this.serviceDeleteWhileSelectSeviceGroups = new List<SereServADO>();
							}
							this.ResetOneService(sereServADO);
						}
						this.gridControlServiceProcess.RefreshDataSource();
						this.SetEnableButtonControl(this.actionType);
					}
					else if (e.Column.FieldName == this.gridColumn_Service_PrimaryPatientType.FieldName)
					{
						if (!VerifyCheckFeeWhileAssign())
						{
							this.ResetOneService(sereServADO);
							sereServADO.IsChecked = false;
							return;
						}
					}

					this.SetDefaultSerServTotalPrice();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewServiceProcess_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
				{
					GridView view = sender as GridView;
					GridHitInfo hi = view.CalcHitInfo(e.Location);
					if (hi.InRowCell)
					{
						if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
						{
							view.FocusedRowHandle = hi.RowHandle;
							view.FocusedColumn = hi.Column;

							int rowHandle = gridViewServiceProcess.GetVisibleRowHandle(hi.RowHandle);
							var dataRow = (SereServADO)gridViewServiceProcess.GetRow(rowHandle);
							if (dataRow != null)
							{
								//if (hi.Column.FieldName == "IsChecked" && (dataRow.IsAllowChecked == false))
								//{
								//    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
								//    return;
								//}
							}

							view.ShowEditor();
							CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
							if (checkEdit == null)
								return;
							if (checkEdit.ReadOnly)
								return;
							DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
							Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
							GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
							Rectangle gridGlyphRect =
								new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
								 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
								 glyphRect.Width,
								 glyphRect.Height);
							if (!gridGlyphRect.Contains(e.Location))
							{
								view.CloseEditor();
								if (!view.IsCellSelected(hi.RowHandle, hi.Column))
								{
									view.SelectCell(hi.RowHandle, hi.Column);
								}
								else
								{
									view.UnselectCell(hi.RowHandle, hi.Column);
								}
							}
							else
							{
								checkEdit.Checked = !checkEdit.Checked;
								view.CloseEditor();
							}
							(e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
						}
					}
					else if (hi.InColumnPanel && hi.InColumn)//check all
					{
						if (hi.Column.Name == this.grcChecked_TabService.Name)
						{
							WaitingManager.Show();

							List<SereServADO> data = null;
							if (view.DataSource != null)
							{
								data = (List<SereServADO>)view.DataSource;
							}

							if (data != null && data.Count > 0)
							{
								//check all thì bỏ check
								if (data.Count == data.Count(o => o.IsChecked))
								{
									foreach (var item in data)
									{
										this.ResetOneService(item);
										item.IsChecked = false;
									}

									if ((bool)this.toggleSwitchDataChecked.EditValue == true)
									{
										this.isNotHandlerWhileChangeToggetSwith = true;
										this.toggleSwitchDataChecked.EditValue = false;
										this.LoadDataToGrid(true);
										this.isNotHandlerWhileChangeToggetSwith = false;
									}
								}
								else //1 check hoăc chưa check thì check all
								{
									foreach (var sereServADO in data)
									{
										if (!sereServADO.IsChecked)
										{
											sereServADO.IsChecked = true;
											this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);

											List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
											FilterExecuteRoom(sereServADO, ref executeRoomList);
											long executeRoomId = this.SetPriorityRequired(executeRoomList);
											if (executeRoomId <= 0)
												executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);
											if (sereServADO.TDL_EXECUTE_ROOM_ID <= 0 && executeRoomId > 0)
											{
												sereServADO.TDL_EXECUTE_ROOM_ID = executeRoomId;
											}
											this.FillDataOtherPaySourceDataRow(sereServADO);
											this.ValidServiceDetailProcessing(sereServADO);
											this.SetAssignNumOrder(sereServADO);
											if (!VerifyCheckFeeWhileAssign())
											{
												this.ResetOneService(sereServADO);
												sereServADO.IsChecked = false;
												break;
											}
										}
									}
								}

								view.GridControl.RefreshDataSource();
								this.SetEnableButtonControl(this.actionType);
								this.VerifyWarningOverCeiling();
								this.SetDefaultSerServTotalPrice();
							}

							WaitingManager.Hide();
						}
					}
				}
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private long SetDefaultExcuteRoom(List<V_HIS_EXECUTE_ROOM> excuteRoomList)
		{
			long sereServADO = 0;
			try
			{
				if (HisConfigCFG.ShowDefaultExecuteRoom == "2")
				{
					gridColumnExecuteRoomName__TabService.OptionsColumn.AllowEdit = false;
					return sereServADO;
				}
				if (HisConfigCFG.ShowDefaultExecuteRoom == "1" && excuteRoomList != null && excuteRoomList.Count > 0)
				{
					V_HIS_EXECUTE_ROOM priority = excuteRoomList.Where(o => this.exroRooms != null && this.exroRooms.Any(a => a.IS_PRIORITY_REQUIRE == (short)1 && a.EXECUTE_ROOM_ID == o.ID)).FirstOrDefault();

					if (priority != null)
					{
						sereServADO = priority.ROOM_ID;
						return sereServADO;
					}

					// cùng phòng làm việc
					var roomCheck = excuteRoomList.FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
					if (roomCheck != null)
					{
						sereServADO = roomCheck.ROOM_ID;
					}
					else
					{
						var currentRoomCheck = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
						if (currentRoomCheck != null)
						{
							// cùng khoa
							var roomCheck1 = excuteRoomList.FirstOrDefault(o => o.DEPARTMENT_ID == currentRoomCheck.DEPARTMENT_ID);
							if (roomCheck1 != null)
							{
								sereServADO = roomCheck1 != null ? roomCheck1.ROOM_ID : -1;
							}
							else
							{
								// cùng chi nhánh
								var roomCheck2 = excuteRoomList.FirstOrDefault(o => o.BRANCH_ID == currentRoomCheck.BRANCH_ID);
								sereServADO = roomCheck2 != null ? roomCheck2.ROOM_ID : -1;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return sereServADO;
		}

		private long SetPriorityRequired(List<V_HIS_EXECUTE_ROOM> excuteRoomList)
		{
			long roomId = 0;
			try
			{
				if (excuteRoomList != null && excuteRoomList.Count > 0)
				{
					List<V_HIS_EXECUTE_ROOM> lstPriority = excuteRoomList.Where(o => this.exroRooms != null && this.exroRooms.Any(a => a.IS_PRIORITY_REQUIRE == (short)1 && a.EXECUTE_ROOM_ID == o.ID)).ToList();
					if (lstPriority != null && lstPriority.Count == 1)
					{
						return lstPriority[0].ROOM_ID;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return roomId;
		}

		private void gridViewServiceProcess_ShownEditor(object sender, EventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				BaseEdit edit = view.ActiveEditor;
				if (edit != null && view.FocusedRowHandle == GridControl.AutoFilterRowHandle)
				{
					edit.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
					edit.Properties.EditValueChangedDelay = 500;
				}
				else
				{
					SereServADO data = view.GetFocusedRow() as SereServADO;
					if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
					{
						GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
						if (data != null)
						{
							var dataSource = editor.Properties.DataSource;
							this.FillDataIntoPatientTypeCombo(data, editor);
							data.IsNotLoadDefaultPatientType = true;
							editor.EditValue = data.PATIENT_TYPE_ID;
							Inventec.Common.Logging.LogSystem.Warn("gridViewServiceProcess_ShownEditor PATIENT_TYPE_ID");
						}
					}
					else if (view.FocusedColumn.FieldName == "PRIMARY_PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
					{
						GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
						if (data != null && data.IsChecked && !data.IsNotChangePrimaryPaty)
						{
							this.FillDataIntoPrimaryPatientTypeCombo(data, editor);
							editor.EditValue = data.PRIMARY_PATIENT_TYPE_ID;
						}
						else
						{
							editor.ReadOnly = true;
						}
					}
					else if (view.FocusedColumn.FieldName == "TDL_EXECUTE_ROOM_ID" && view.ActiveEditor is GridLookUpEdit)
					{
						GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
						if (editor != null)
						{
							this.FillDataIntoExcuteRoomCombo(data, editor);
							List<V_HIS_EXECUTE_ROOM> dataSource = (List<V_HIS_EXECUTE_ROOM>)editor.Properties.DataSource;
							//data.TDL_EXECUTE_ROOM_ID = executeRoomDefault;
							if (data != null && data.TDL_EXECUTE_ROOM_ID <= 0)
							{
								long executeRoomId = SetDefaultExcuteRoom(dataSource);
								editor.EditValue = executeRoomId;
							}
							else
							{
								editor.EditValue = data.TDL_EXECUTE_ROOM_ID;
							}
						}
					}
					else if (view.FocusedColumn.FieldName == "IsKHBHYT" && view.ActiveEditor is CheckEdit)
					{
						CheckEdit editor = view.ActiveEditor as CheckEdit;
						editor.ReadOnly = true;
						// Kiểm tra các điều kiện: 
						//1. Đối tượng BN = BHYT
						//2. Loại hình thanh toán !=BHYT
						//3. Dịch vụ đó có giá bán = BHYT
						//4. Dịch vụ đó có giá bán BHYT<giá bán của loại đối tượng TT (xemlai...)
						if (this.currentHisPatientTypeAlter != null
							&& this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
							&& data.PATIENT_TYPE_ID != this.currentHisPatientTypeAlter.PATIENT_TYPE_ID)
						{
							if (BranchDataWorker.HasServicePatyWithListPatientType(data.SERVICE_ID, new List<long>() { this.currentHisPatientTypeAlter.PATIENT_TYPE_ID }))
								editor.ReadOnly = false;
						}
					}
					else if (view.FocusedColumn.FieldName == "InstructionNote" && view.ActiveEditor is MemoExEdit)
					{
						MemoExEdit editor = view.ActiveEditor as MemoExEdit;
						// editor.Text = data.InstructionNote;
					}
					else if (view.FocusedColumn.FieldName == "TEST_SAMPLE_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
					{
						if (data != null && data.IsChecked)
						{
							GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
							this.FillSampleType(data, editor);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewServiceProcess_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
		{
			try
			{
				if (e.ColumnName == "AMOUNT" || e.ColumnName == "PATIENT_TYPE_ID" || e.ColumnName == "TDL_SERVICE_NAME")
				{
					this.gridViewServiceProcess_CustomRowError(sender, e);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewServiceProcess_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
		{
			try
			{
				var index = this.gridViewServiceProcess.GetDataSourceRowIndex(e.RowHandle);
				if (index < 0)
				{
					e.Info.ErrorType = ErrorType.None;
					e.Info.ErrorText = "";
					return;
				}
				var listDatas = this.gridControlServiceProcess.DataSource as List<SereServADO>;
				var row = listDatas[index];
				if (e.ColumnName == "AMOUNT")
				{
					if (row.IsChecked && row.ErrorTypeAmount == ErrorType.Warning)
					{
						e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmount);
						e.Info.ErrorText = (string)(row.ErrorMessageAmount);
					}
					else
					{
						e.Info.ErrorType = (ErrorType)(ErrorType.None);
						e.Info.ErrorText = "";
					}
				}
				else if (e.ColumnName == "PATIENT_TYPE_ID")
				{
					if (row.IsChecked && row.ErrorTypePatientTypeId == ErrorType.Warning)
					{
						e.Info.ErrorType = (ErrorType)(row.ErrorTypePatientTypeId);
						e.Info.ErrorText = (string)(row.ErrorMessagePatientTypeId);
					}
					else
					{
						e.Info.ErrorType = (ErrorType)(ErrorType.None);
						e.Info.ErrorText = "";
					}
				}
				else if (e.ColumnName == "TDL_SERVICE_NAME")
				{
					if (row.ErrorTypeIsAssignDay == ErrorType.Warning)
					{
						e.Info.ErrorType = (ErrorType)(row.ErrorTypeIsAssignDay);
						e.Info.ErrorText = (string)(row.ErrorMessageIsAssignDay);
					}
					else
					{
						e.Info.ErrorType = (ErrorType)(ErrorType.None);
						e.Info.ErrorText = "";
					}
				}
			}
			catch (Exception ex)
			{
				try
				{
					e.Info.ErrorType = (ErrorType)(ErrorType.None);
					e.Info.ErrorText = "";
				}
				catch { }

				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewServiceProcess_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
					{
						SereServADO oneServiceSDO = (SereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
						long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
						if (oneServiceSDO != null)
						{
							if (e.Column.FieldName == "PRICE_DISPLAY" && oneServiceSDO.IsChecked)
							{

								if (oneServiceSDO.AssignSurgPriceEdit.HasValue && (oneServiceSDO.AssignSurgPriceEdit > 0 || oneServiceSDO.IsServiceKsk))
								{
									e.Value = oneServiceSDO.AssignSurgPriceEdit;
								}
								else
								{
									e.Value = GetPriceBySurg(oneServiceSDO);
								}

							}
							if (e.Column.FieldName == "PRICE_PRPO_DISPLAY")
							{
								if (oneServiceSDO.AssignPackagePriceEdit.HasValue && oneServiceSDO.AssignPackagePriceEdit > 0)
								{
									e.Value = oneServiceSDO.AssignPackagePriceEdit;
								}
								else
								{
									e.Value = GetPriceBypackage(oneServiceSDO);
								}
							}
							if (e.Column.FieldName == "HEIN_LIMIT_PRICE_DISPLAY")
							{
								e.Value = GetHeinLimitPriceByDataRow(oneServiceSDO);
							}
							else if (e.Column.FieldName == "ESTIMATE_DURATION_DISPLAY")
							{
								var oneService = this.ServiceAllADOs.Where(o => o.ID == oneServiceSDO.SERVICE_ID).FirstOrDefault();
								if (oneService != null)
								{
									e.Value = oneService.ESTIMATE_DURATION;
								}
							}
						}
						else
						{
							e.Value = null;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewServiceProcess_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				if (!this.ValidPatientTypeForAdd())
					return;

				GridView view = (GridView)sender;
				Point pt = view.GridControl.PointToClient(Control.MousePosition);
				GridHitInfo info = view.CalcHitInfo(pt);
				if ((info.InRow || info.InRowCell)
					&& info.Column.FieldName != this.grcChecked_TabService.FieldName
					&& info.Column.FieldName != this.gridColumnPatientTypeName__TabService.FieldName
					&& info.Column.FieldName != this.grcAmount_TabService.FieldName
					&& info.Column.FieldName != this.grcExpend_TabService.FieldName
					&& info.Column.FieldName != this.gridColumnNoDifference.FieldName)
				{
					var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
					if (sereServADO != null
						//&& (sereServADO.IsAllowChecked == null || sereServADO.IsAllowChecked == true)
						)
					{
						UpdateCurrentFocusRow(sereServADO);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewServiceProcess_CalcRowHeight(object sender, RowHeightEventArgs e)
		{
			try
			{
				if (gridViewServiceProcess.IsFilterRow(e.RowHandle))
				{
					var fontSize = ApplicationFontWorker.GetFontSize();
					if (fontSize == ApplicationFontConfig.FontSize825)
					{
						e.RowHeight = 23;
					}
					else if (fontSize == ApplicationFontConfig.FontSize875)
					{
						e.RowHeight = 23;
						//txtServiceName_Search.Location = new Point(180, 23);
						//txtServiceCode_Search.Location = new Point(31, 23);
						//txtServiceBhytCode_Search.Location = new Point(107, 23);
					}
					else if (fontSize == ApplicationFontConfig.FontSize925)
					{
						e.RowHeight = 25;
						//txtServiceName_Search.Location = new Point(180, 25);
						//txtServiceCode_Search.Location = new Point(31, 25);
						//txtServiceBhytCode_Search.Location = new Point(107, 23);
					}
					else if (fontSize == ApplicationFontConfig.FontSize975)
					{
						e.RowHeight = 27;
						//txtServiceName_Search.Location = new Point(180, 27);
						//txtServiceCode_Search.Location = new Point(31, 27);
						//txtServiceBhytCode_Search.Location = new Point(107, 23);
					}
					else if (fontSize == ApplicationFontConfig.FontSize1025)
					{
						//txtServiceName_Search.Location = new Point(180, 29);
						//txtServiceCode_Search.Location = new Point(31, 29);
						//txtServiceBhytCode_Search.Location = new Point(107, 23);
						e.RowHeight = 30;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewServiceProcess_ColumnFilterChanged(object sender, EventArgs e)
		{
			try
			{
				if (IsClosingForm)
					return;
				if ((gridViewServiceProcess.FocusedColumn == gridColumnTDL_HEIN_SERVICE_BHYT_CODE ||
				   gridViewServiceProcess.FocusedColumn == grcServiceCode_TabService ||
				   gridViewServiceProcess.FocusedColumn == grcServiceName_TabService)
				   && !string.IsNullOrEmpty(gridViewServiceProcess.GetFocusedDisplayText())
				   && gridViewServiceProcess.FocusedRowHandle == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle)
				{
					toggleSwitchDataChecked.IsOn = false;
				}

				if (gridViewServiceProcess.RowCount == 2 && !chkNotCheckService.Checked)
				{
					var sereServADO = (SereServADO)this.gridViewServiceProcess.GetRow(0);
					if (sereServADO != null)
					{
						if (lstSereServExist != null && lstSereServExist.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID) != null && DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dịch vụ có thời gian chỉ định nằm trong khoảng thời gian thiết lập của phác đồ điều trị. Thời gian chỉ định {0} (mã y lệnh: {1}). Bạn có muốn tiếp tục?", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(lstSereServExist.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID).TDL_INTRUCTION_TIME), lstSereServExist.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID).TDL_SERVICE_REQ_CODE), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
						{
							sereServADO.IsChecked = false;
							return;
						}
						sereServADO.IsChecked = true;

						if (sereServADO.IsChecked)
						{
							//Phân biệt giá trị TEST_SAMPLE_TYPE_CODE mặc định bởi TEST_SAMPLE_TYPE_ID = 0;
							if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && sereServADO.SERVICE_TYPE_ID > 0 && serviceTypeIdSplitReq != null && serviceTypeIdSplitReq.Count > 0 && serviceTypeIdSplitReq.Exists(o => o == sereServADO.SERVICE_TYPE_ID))
							{
								if (dataListTestSampleType != null && dataListTestSampleType.Count > 0 && sereServADO.TEST_SAMPLE_TYPE_ID == 0 && !string.IsNullOrEmpty(sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT))
								{
									var sampleType = dataListTestSampleType.FirstOrDefault(o => o.TEST_SAMPLE_TYPE_CODE == sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT);
									if (sampleType != null)
									{
										sereServADO.TEST_SAMPLE_TYPE_ID = sampleType.ID;
										sereServADO.TEST_SAMPLE_TYPE_CODE = sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT;
										sereServADO.TEST_SAMPLE_TYPE_NAME = sampleType.TEST_SAMPLE_TYPE_NAME;
									}
								}
							}
							if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
							{
								MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
							}
							ValidOnlyShowNoticeService(sereServADO);
							//if (HisConfigCFG.DefaultPatientTypeOption && this.serviceReqParentId != null && this.hisSereServForGetPatientType != null && !sereServADO.IsNotLoadDefaultPatientType)
							//{
							//    sereServADO.PATIENT_TYPE_ID = this.hisSereServForGetPatientType.PATIENT_TYPE_ID;
							//    sereServADO.PATIENT_TYPE_CODE = currentPatientTypes.First(o => o.ID == this.hisSereServForGetPatientType.PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
							//    sereServADO.PATIENT_TYPE_NAME = currentPatientTypes.First(o => o.ID == this.hisSereServForGetPatientType.PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
							//}
							if (sereServADO.PATIENT_TYPE_ID > 0)
							{
								this.ChoosePatientTypeDefaultlService(sereServADO.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO, false, null, true);
							}
							else
							{
								this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
							}
							if (!VerifyCheckFeeWhileAssign())
							{
								this.ResetOneService(sereServADO);
								sereServADO.IsChecked = false;
								return;
							}
							this.FillDataOtherPaySourceDataRow(sereServADO);

							List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
							FilterExecuteRoom(sereServADO, ref executeRoomList);
							long executeRoomId = this.SetPriorityRequired(executeRoomList);
							if (executeRoomId <= 0)
								executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);
							if (sereServADO.TDL_EXECUTE_ROOM_ID <= 0 && executeRoomId > 0)
							{
								sereServADO.TDL_EXECUTE_ROOM_ID = executeRoomId;
							}
							if (sereServADO.IsAutoExpend == (short?)1 && sereServADO.IsAllowExpend == (short?)1 && !sereServADO.PackagePriceId.HasValue)
								sereServADO.IsExpend = true;
							this.ValidServiceDetailProcessing(sereServADO);
							this.ProcessNoDifferenceHeinServicePrice(sereServADO);
							this.SetAssignNumOrder(sereServADO);
							this.VerifyWarningOverCeiling();
						}
						else
						{
							this.ResetOneService(sereServADO);
							sereServADO.IsNoDifference = false;
						}

						this.gridControlServiceProcess.RefreshDataSource();
						this.SetEnableButtonControl(this.actionType);
						this.SetDefaultSerServTotalPrice();

						gridViewServiceProcess.ActiveEditor.SelectAll();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetAssignNumOrder(SereServADO sereServ)
		{
			try
			{
				if (!sereServ.IsChecked)
				{
					sereServ.AssignNumOrder = null;
					return;
				}
				if (workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null)
				{
					var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
					if (lstPatientSelect != null && lstPatientSelect.Count > 1)
					{
						sereServ.AssignNumOrder = null;
						return;
					}
				}
				var ss = sereServsInTreatmentRaw.Where(o => o.SERVICE_ID == sereServ.SERVICE_ID && o.TDL_INTRUCTION_TIME <= InstructionTime).ToList();
				sereServ.AssignNumOrder = ss != null && ss.Count > 0 ? ss.Count + 1 : 1;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void gridViewServiceProcess_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					this.gridViewServiceProcess.FocusedRowHandle = GridControl.AutoFilterRowHandle;

					if (this.gridViewServiceProcess.FocusedColumn != this.grcServiceCode_TabService
						&& this.gridViewServiceProcess.FocusedColumn != this.gridColumnTDL_HEIN_SERVICE_BHYT_CODE
						&& this.gridViewServiceProcess.FocusedColumn != this.grcServiceName_TabService)
					{
						this.gridViewServiceProcess.FocusedColumn = this.grcServiceCode_TabService;
						this.gridViewServiceProcess.ClearColumnsFilter();
					}

					//var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
					//if (sereServADO != null)
					//{
					//    if (sereServADO.IsChecked)
					//    {
					//        this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
					//        this.FillDataOtherPaySourceDataRow(sereServADO);
					//        this.ValidServiceDetailProcessing(sereServADO);
					//        this.ProcessNoDifferenceHeinServicePrice(sereServADO);
					//        this.VerifyWarningOverCeiling();
					//        if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
					//        {
					//            MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
					//        }
					//    }
					//    else
					//    {
					//        this.ResetOneService(sereServADO);
					//        sereServADO.IsNoDifference = false;
					//    }

					//    this.gridControlServiceProcess.RefreshDataSource();
					//    this.SetEnableButtonControl(this.actionType);
					//    this.SetDefaultSerServTotalPrice();
					//}
				}
				else if (e.KeyCode == Keys.Space)
				{
					var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
					if (sereServADO != null)
					{
						UpdateCurrentFocusRow(sereServADO);
					}
				}
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}

		private void gridViewServiceProcess_RowCellStyle(object sender, RowCellStyleEventArgs e)
		{
			try
			{
				if (this.currentHisPatientTypeAlter != null
						&& this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
				{
					var index = this.gridViewServiceProcess.GetDataSourceRowIndex(e.RowHandle);
					if (index < 0) return;

					var listDatas = this.gridControlServiceProcess.DataSource as List<SereServADO>;
					var dataRow = listDatas[index];
					if (dataRow != null && dataRow.PATIENT_TYPE_ID > 0
						&& dataRow.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
					{
						//Đối tượng điều trị là BHYT, nhưng do ko có chính sách giá theo BHYT nên khi tích chọn dịch vụ, sẽ hiển thị màu tím.
						//Có chính sách giá nhưng là đối tượng khác, không phải BHYT ==> màu tím

						var bFindservice = (BranchDataWorker.DicServicePatyInBranch != null
							&& BranchDataWorker.DicServicePatyInBranch.ContainsKey(dataRow.SERVICE_ID)) ? BranchDataWorker.HasServicePatyWithListPatientType(dataRow.SERVICE_ID, new List<long>() { this.currentHisPatientTypeAlter.PATIENT_TYPE_ID }) : false;
						if (!bFindservice)
							e.Appearance.ForeColor = System.Drawing.Color.Violet;
					}

					if (dataRow != null)
					{
						var bFindservice = !String.IsNullOrWhiteSpace(dataRow.TDL_HEIN_SERVICE_BHYT_CODE) && (BranchDataWorker.DicServicePatyInBranch != null
						   && BranchDataWorker.DicServicePatyInBranch.ContainsKey(dataRow.SERVICE_ID)) ? BranchDataWorker.HasServicePatyWithListPatientType(dataRow.SERVICE_ID, new List<long>() { HisConfigCFG.PatientTypeId__BHYT }) : false;
						if (bFindservice && !String.IsNullOrEmpty(HisConfigCFG.BhytColorCode))
						{
							e.Appearance.ForeColor = GetColor(HisConfigCFG.BhytColorCode);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		Color GetColor(string colorCode)
		{
			try
			{
				if (!String.IsNullOrEmpty(colorCode))
				{
					return System.Drawing.ColorTranslator.FromHtml(colorCode);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return Color.Red;
		}

		private void gridViewServiceProcess_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
		{
			try
			{
				var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
				string rowValue = Convert.ToString(this.gridViewServiceProcess.GetGroupRowValue(e.RowHandle, info.Column));
				info.GroupText = rowValue;
			}
			catch (Exception ex)
			{
				LogSystem.Warn(ex);
			}
		}

		private void repositoryItembtnEditDonGia_TextDisable_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				SereServADO ssADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
				if (ssADO != null)
				{
					frmPriceEdit frmPriceEdit = new frmPriceEdit(ssADO, UpdateSurgPrice, PriceEditType.EditTypeSurgPrice, GetPriceBypackage, GetPriceBySurg);
					frmPriceEdit.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repositoryItembtnEditGiaGoi_TextDisable_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				SereServADO ssADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
				if (ssADO != null)
				{
					frmPriceEdit frmPriceEdit = new frmPriceEdit(ssADO, UpdatePackagePrice, PriceEditType.EditTypePackagePrice, GetPriceBypackage, GetPriceBySurg);
					frmPriceEdit.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repositoryItemCboPrimaryPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == ButtonPredefines.Delete)
				{
					SereServADO ssADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
					if (ssADO != null)
					{
						ssADO.PRIMARY_PATIENT_TYPE_ID = null;
						this.gridControlServiceProcess.RefreshDataSource();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void UpdateCurrentFocusRow(SereServADO sereServADO)
		{
			try
			{
				if (sereServADO == null || (sereServADO.IsChecked && sereServADO.PackagePriceId.HasValue))
					return;

				sereServADO.IsChecked = !sereServADO.IsChecked;
				if (sereServADO.IsChecked)
				{
					this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
					this.FillDataOtherPaySourceDataRow(sereServADO);
					if (!VerifyCheckFeeWhileAssign())
					{
						this.ResetOneService(sereServADO);
						sereServADO.IsChecked = false;
						return;
					}


					List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
					FilterExecuteRoom(sereServADO, ref executeRoomList);

					long executeRoomId = this.SetPriorityRequired(executeRoomList);

					if (executeRoomId <= 0)
						executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);

					//data.TDL_EXECUTE_ROOM_ID = executeRoomDefault;
					if (sereServADO.TDL_EXECUTE_ROOM_ID <= 0)
					{
						sereServADO.TDL_EXECUTE_ROOM_ID = executeRoomId;
					}
					if (sereServADO.IsAutoExpend == (short?)1 && sereServADO.IsAllowExpend == (short?)1 && !sereServADO.PackagePriceId.HasValue)
						sereServADO.IsExpend = true;
					this.ValidServiceDetailProcessing(sereServADO);
					this.ProcessNoDifferenceHeinServicePrice(sereServADO);
					//if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
					//{
					//    MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
					//}
				}
				else
				{
					this.ResetOneService(sereServADO);
					sereServADO.IsNoDifference = false;
				}

				this.gridControlServiceProcess.RefreshDataSource();
				if (sereServADO.IsChecked)
				{
					this.VerifyWarningOverCeiling();
				}
				this.SetEnableButtonControl(this.actionType);
				this.SetDefaultSerServTotalPrice();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void tooltipService_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
		{
			try
			{
				if (e.Info == null && e.SelectedControl == this.gridControlServiceProcess)
				{
					DevExpress.XtraGrid.Views.Grid.GridView view = this.gridControlServiceProcess.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
					GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
					if (info.InRowCell && (info.Column.FieldName == "SERVICE_CONDITION_NAME" || info.Column.FieldName == "TDL_SERVICE_NAME" || info.Column.FieldName == "TDL_HEIN_SERVICE_BHYT_CODE"))
					{
						if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
						{
							lastColumn = info.Column;
							lastRowHandle = info.RowHandle;
							string text = "";
							if (info.Column.FieldName == "SERVICE_CONDITION_NAME")
							{
								text = (view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_NAME") ?? "").ToString();
							}
							if (info.Column.FieldName == "TDL_SERVICE_NAME")
							{
								text = (view.GetRowCellValue(lastRowHandle, "TDL_SERVICE_NAME") ?? "").ToString();
							}
							if (info.Column.FieldName == "TDL_HEIN_SERVICE_BHYT_CODE")
							{
								text = (view.GetRowCellValue(lastRowHandle, "TDL_HEIN_SERVICE_BHYT_CODE") ?? "").ToString();
							}
							lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
						}
						e.Info = lastInfo;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetEnableButtonControl(int actionType)
		{
			try
			{
				if (this.actionType == GlobalVariables.ActionAdd)
				{
					List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
					this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = this.btnCreateServiceGroup.Enabled = isCheckAssignServiceSimultaneityOption ? false : (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0);
					this.pnlPrintAssignService.Enabled = this.btnShowDetail.Enabled = this.btnCreateBill.Enabled = this.btnDepositService.Enabled = this.btnPrintPhieuHuongDanBN.Enabled = this.BtnPrint.Enabled = this.btnEdit.Enabled = false;

				}
				else
				{
					this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = this.btnCreateServiceGroup.Enabled = false;
					this.pnlPrintAssignService.Enabled = this.btnShowDetail.Enabled = this.btnCreateBill.Enabled = this.btnDepositService.Enabled = this.btnPrintPhieuHuongDanBN.Enabled = this.BtnPrint.Enabled = this.btnEdit.Enabled = true;
				}

				//hiển thị ảnh checkbox
				List<SereServADO> data = null;
				if (gridViewServiceProcess.DataSource != null)
				{
					data = (List<SereServADO>)gridViewServiceProcess.DataSource;
				}

				if (data != null && data.Count == data.Count(o => o.IsChecked) && this.imageCollection1.Images.Count > 1)
				{
					this.grcChecked_TabService.Image = this.imageCollection1.Images[1];
				}
				else if (data != null && data.Exists(o => o.IsChecked) && this.imageCollection1.Images.Count > 2)
				{
					this.grcChecked_TabService.Image = this.imageCollection1.Images[2];
				}
				else
				{
					this.grcChecked_TabService.Image = this.imageCollection1.Images[0];
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private bool ValidPatientTypeForAdd()
		{
			bool valid = true;
			try
			{
				if (this.currentHisPatientTypeAlter == null || this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == 0)
				{
					MessageManager.Show(String.Format(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh, Inventec.Common.DateTime.Convert.TimeNumberToDateString(intructionTimeSelecteds.First())));
					valid = false;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return valid;
		}

		private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
		{
			try
			{
				cboEditor.Focus();
				cboEditor.ShowPopup();
				if (isSelectFirstRow)
					PopupLoader.SelectFirstRowPopup(cboEditor);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
		{
			try
			{
				cboEditor.Focus();
				cboEditor.ShowPopup();
				if (isSelectFirstRow)
					PopupLoader.SelectFirstRowPopup(cboEditor);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
					if (String.IsNullOrEmpty(searchCode))
					{
						this.cboUser.EditValue = null;
						this.FocusShowpopup(cboUser, true);
					}
					else
					{
						var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
							.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

						var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
						if (searchResult != null && searchResult.Count == 1)
						{
							this.cboUser.EditValue = searchResult[0].LOGINNAME;
							this.txtLoginName.Text = searchResult[0].LOGINNAME;
							CheckAssignServiceSimultaneityOption();
							this.FocusWhileSelectedUser();
						}
						else
						{
							this.cboUser.EditValue = null;
							this.FocusShowpopup(cboUser, true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void FocusWhileSelectedUser()
		{
			try
			{
				this.gridControlServiceProcess.Focus();
				this.gridViewServiceProcess.FocusedRowHandle = 0;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboUser_Closed(object sender, ClosedEventArgs e)
		{
			try
			{
				if (e.CloseMode == PopupCloseMode.Normal)
				{
					if (this.cboUser.EditValue != null)
					{
						ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
						if (data != null)
						{
							this.txtLoginName.Text = data.LOGINNAME;

							CheckAssignServiceSimultaneityOption();
						}
					}
					this.FocusWhileSelectedUser();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboUser_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (this.cboUser.EditValue != null)
					{
						ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
						if (data != null)
						{
							this.FocusWhileSelectedUser();
							this.txtLoginName.Text = data.LOGINNAME;
							CheckAssignServiceSimultaneityOption();
						}
					}
				}
				else
				{
					this.cboUser.ShowPopup();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void DelegateSelectMultiDate(List<DateTime?> datas, DateTime time)
		{
			try
			{
				this.intructionTimeSelecteds = (this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList());
				this.isMultiDateState = chkMultiIntructionTime.Checked;

				ChangeIntructionTime(time);

				if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign && this.currentWorkingRoom != null && currentWorkingRoom.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
				{
					ProcessGetDataDepartment();
					CheckTimeInDepartment(this.intructionTimeSelecteds);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ChangeIntructionTime(DateTime intructTime)
		{
			try
			{
				if (HisConfigCFG.IsUsingServerTime == commonString__true
					&& this.currentHisTreatment != null)
				{
					return;
				}
				LogSystem.Debug("ChangeIntructionTime => 1");
				//this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(ucDate);
				//this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(ucDate);
				this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.FirstOrDefault());
				this.SetDateUc();
				this.ProcessDataWithTreatmentWithPatientTypeInfo();
				this.LoadTotalSereServByHeinWithTreatment(this.treatmentId);
				this.LoadServicePaty();
				this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
				var patientTypePrimary = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1).ToList();
				this.InitComboPrimaryPatientType(patientTypePrimary);
				this.InitComboExecuteRoom();
				this.BindTree();
				this.LoadDataSereServWithTreatment(this.currentHisTreatment, intructTime);
				this.LoadTreatmentInfo__PatientType();

				if (HisConfigCFG.IcdServiceHasCheck == "1" || HisConfigCFG.IcdServiceHasCheck == "2" || HisConfigCFG.IcdServiceHasCheck == "3" || HisConfigCFG.IcdServiceHasCheck == "4" || HisConfigCFG.IcdServiceHasCheck == "5")
				{
					//xử lý chọn lại phác đồ khi sửa thời gian chỉ định
					if (this.icdServicePhacDos != null && this.icdServicePhacDos.Count > 0)
					{
						Inventec.Common.Logging.LogSystem.Debug("ChangeIntructionTime. 1. this.icdServicePhacDos.count=" + this.icdServicePhacDos.Count);

						ProcessChoiceIcdPhacDo(this.icdServicePhacDos);
					}
					else
					{
						this.ResetDefaultGridData();
					}
				}

				//this.SetDefautTrackingCombo(this.trackingAdos, HisConfigCFG.IsDefaultTracking);
				this.notSearch = false;
				LogSystem.Debug("ChangeIntructionTime => 2");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkPriority_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (this.chkPriority.Checked)
						this.chkPriority.CheckState = CheckState.Unchecked;
					else
						this.chkPriority.CheckState = CheckState.Checked;
					this.txtDescription.Focus();
					this.txtDescription.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ShowKskServiceProcess(List<HIS_KSK_SERVICE> lstkskService)
		{
			try
			{
				if (lstkskService != null && lstkskService.Count > 0)
				{
					List<SereServADO> sereServADOs = new List<SereServADO>();
					bool hasService = false;
					this.ResetServiceKskSelected();
					this.InitComboRepositoryPatientType(BackendDataWorker.Get<HIS_PATIENT_TYPE>().ToList());
					foreach (var item in lstkskService)
					{
						var service = this.ServiceIsleafADOs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
						if (service != null)
						{
							hasService = true;
							service.IsChecked = true;
							service.PATIENT_TYPE_ID = currentTreatment.TDL_PATIENT_TYPE_ID ?? 0;
							service.AMOUNT = item.AMOUNT;
							if (item.PRICE != null)
								service.AssignSurgPriceEdit = item.PRICE;
							service.IsServiceKsk = true;
							if (!VerifyCheckFeeWhileAssign())
							{
								this.ResetOneService(service);
								service.IsChecked = false;
								break;
							}
							this.FillDataOtherPaySourceDataRow(service);
							this.SetAssignNumOrder(service);
							List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
							FilterExecuteRoom(service, ref executeRoomList);

							long executeRoomId = this.SetPriorityRequired(executeRoomList);

							if (executeRoomId <= 0)
								executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);

							//data.TDL_EXECUTE_ROOM_ID = executeRoomDefault;
							if (service.TDL_EXECUTE_ROOM_ID <= 0)
							{
								service.TDL_EXECUTE_ROOM_ID = executeRoomId;
							}

							this.ValidServiceDetailProcessing(service);
						}
					}
					sereServADOs = this.ServiceIsleafADOs.Where(o => o.IsChecked).OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
					if (hasService && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() != "true")
						this.toggleSwitchDataChecked.EditValue = true;

					this.ValidOnlyShowNoticeService(sereServADOs);

					this.SetEnableButtonControl(this.actionType);
					this.SetDefaultSerServTotalPrice();
					this.VerifyWarningOverCeiling();
					this.gridControlServiceProcess.DataSource = null;
					this.gridControlServiceProcess.DataSource = sereServADOs;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SelectOneServiceGroupProcess(List<ServiceGroupADO> svgrs, bool isCheckChange = true)
		{
			try
			{
				List<SereServADO> services = null;
				StringBuilder strMessage = new StringBuilder();
				StringBuilder strMessageTemp__CoDichVuKhongCauHinh = new StringBuilder();
				StringBuilder strMessageTemp__KhongDichVu = new StringBuilder();
				bool hasMessage = false;
				bool hasService = false;
				if (isCheckChange)
					this.ResetServiceGroupSelected();
				if (svgrs != null && svgrs.Count > 0)
				{

					var idSelecteds = svgrs.Select(o => o.ID).Distinct().ToList();
					var servSegrAllows = BackendDataWorker.Get<V_HIS_SERV_SEGR>().Where(o => idSelecteds.Contains(o.SERVICE_GROUP_ID)).ToList();
					if (servSegrAllows != null && servSegrAllows.Count > 0)
					{
						Inventec.Common.Logging.LogSystem.Debug("SelectOneServiceGroupProcess. 1");
						var serviceOfGroupsInGroupbys = servSegrAllows.GroupBy(o => o.SERVICE_GROUP_ID).ToDictionary(o => o.Key, o => o.ToList());
						foreach (var item in serviceOfGroupsInGroupbys)
						{
							List<V_HIS_SERV_SEGR> servSegrErrors = new List<V_HIS_SERV_SEGR>();
							foreach (var svInGr in serviceOfGroupsInGroupbys[item.Key])
							{
								var service = this.ServiceIsleafADOs.FirstOrDefault(o => svInGr.SERVICE_ID == o.SERVICE_ID);
								#endregion
								if (service != null)
								{
									if (serviceDeleteWhileSelectSeviceGroups != null && serviceDeleteWhileSelectSeviceGroups.Count > 0 && serviceDeleteWhileSelectSeviceGroups.Exists(t => t.SERVICE_ID == service.SERVICE_ID))
									{
										Inventec.Common.Logging.LogSystem.Debug("Dich vu da co trong thiet lap nhom dich vu - dich vu, tuy nhien dich vụ này đã có nhóm trước đó chọn và đã bỏ chọn đi, sẽ không được tich____" + service.SERVICE_ID);
										continue;
									}
									//Phân biệt giá trị TEST_SAMPLE_TYPE_CODE mặc định bởi TEST_SAMPLE_TYPE_ID = 0;
									if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && service.SERVICE_TYPE_ID > 0 && serviceTypeIdSplitReq != null && serviceTypeIdSplitReq.Count > 0 && serviceTypeIdSplitReq.Exists(o => o == service.SERVICE_TYPE_ID))
									{
										if (dataListTestSampleType != null && dataListTestSampleType.Count > 0 && service.TEST_SAMPLE_TYPE_ID == 0 && !string.IsNullOrEmpty(service.TEST_SAMPLE_TYPE_CODE_DEFAULT))
										{
											var sampleType = dataListTestSampleType.FirstOrDefault(o => o.TEST_SAMPLE_TYPE_CODE == service.TEST_SAMPLE_TYPE_CODE_DEFAULT);
											if (sampleType != null)
											{
												service.TEST_SAMPLE_TYPE_ID = sampleType.ID;
												service.TEST_SAMPLE_TYPE_CODE = service.TEST_SAMPLE_TYPE_CODE_DEFAULT;
												service.TEST_SAMPLE_TYPE_NAME = sampleType.TEST_SAMPLE_TYPE_NAME;
											}
										}
									}
									hasService = true;
									service.IsChecked = true;
									this.SetAssignNumOrder(service);
									service.IsKHBHYT = false;
									service.SERVICE_GROUP_ID_SELECTEDs = idSelecteds;
									var searchServiceOfGroups = servSegrAllows.Where(o => o.SERVICE_ID == service.SERVICE_ID).ToList();
									if (searchServiceOfGroups != null && searchServiceOfGroups.Count > 0)
									{
										if (service.IS_MULTI_REQUEST == 1)
										{
											service.AMOUNT = searchServiceOfGroups.Sum(o => o.AMOUNT);
										}
										else
										{
											service.AMOUNT = 1;
										}
										service.InstructionNote = searchServiceOfGroups[0].NOTE;
										service.IsExpend = (searchServiceOfGroups[0].IS_EXPEND == 1);
									}
									this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, service.SERVICE_ID, service);

									if (!VerifyCheckFeeWhileAssign())
									{
										this.ResetOneService(service);
										service.IsChecked = false;
										break;
									}

									this.FillDataOtherPaySourceDataRow(service);

									List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
									FilterExecuteRoom(service, ref executeRoomList);

									long executeRoomId = this.SetPriorityRequired(executeRoomList);

									if (executeRoomId <= 0)
										executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);

									//data.TDL_EXECUTE_ROOM_ID = executeRoomDefault;
									if (service.TDL_EXECUTE_ROOM_ID <= 0)
									{
										service.TDL_EXECUTE_ROOM_ID = executeRoomId;
									}

									this.ValidServiceDetailProcessing(service);
								}
								else
								{
									servSegrErrors.Add(svInGr);
								}
							}

							if (servSegrErrors != null && servSegrErrors.Count > 0)
							{
								if (String.IsNullOrEmpty(strMessageTemp__CoDichVuKhongCauHinh.ToString()))
								{
									strMessageTemp__CoDichVuKhongCauHinh.Append("; ");
								}
								strMessageTemp__CoDichVuKhongCauHinh.Append(String.Format(ResourceMessage.NhomDichVuChiTiet, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(servSegrErrors[0].SERVICE_GROUP_NAME, Color.Red), FontStyle.Bold), String.Join(",", servSegrErrors.Select(o => Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(o.SERVICE_CODE, Color.Black), FontStyle.Bold)))));

								hasMessage = true;
							}
							servSegrErrors = new List<V_HIS_SERV_SEGR>();
						}
						Inventec.Common.Logging.LogSystem.Debug("SelectOneServiceGroupProcess. 2");
						services = this.ServiceIsleafADOs.Where(o => o.IsChecked).OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
					}
					var sgNotIn = servSegrAllows.Select(o => o.SERVICE_GROUP_ID).Distinct().ToArray();
					var searchServiceOfGroups__NoService = svgrs.Where(o => !sgNotIn.Contains(o.ID)).ToList();
					if (searchServiceOfGroups__NoService != null && searchServiceOfGroups__NoService.Count > 0)
					{
						strMessageTemp__KhongDichVu.Append(String.Join(",", searchServiceOfGroups__NoService.Select(o => Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(o.SERVICE_GROUP_NAME, Color.Red), FontStyle.Bold))));
						hasMessage = true;
					}
					Inventec.Common.Logging.LogSystem.Debug("SelectOneServiceGroupProcess. 3");

					if (hasService && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() != "true")
						this.toggleSwitchDataChecked.EditValue = true;

					if (hasMessage)
					{
						strMessage.Append(ResourceMessage.NhomDichVuCoDichVuDuocCauHinhTrongNhomNhungKhongCoCauHinhChinhSach);
						if (!String.IsNullOrEmpty(strMessageTemp__CoDichVuKhongCauHinh.ToString()))
						{
							strMessage.Append("\r\n" + String.Format(ResourceMessage.NhomDichVuCoDichVuKhongCoCauHinh, strMessageTemp__CoDichVuKhongCauHinh.ToString()));
						}
						if (!String.IsNullOrEmpty(strMessageTemp__KhongDichVu.ToString()))
						{
							strMessage.Append("\r\n" + String.Format(ResourceMessage.NhomDichVuKhongCoDichVu, strMessageTemp__KhongDichVu.ToString()));
						}
						strMessage.Append("\r\n" + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.CacDichVuKhongCoChinhSachGiaHoacKhongCoCauHinhSeKhongDuocChon, Color.Maroon), FontStyle.Italic));
						WaitingManager.Hide();
						MessageManager.Show(strMessage.ToString());
					}
					else
					{
						WaitingManager.Hide();
					}

					//this.ValidOnlyShowNoticeService(services);

					this.SetEnableButtonControl(this.actionType);
					this.SetDefaultSerServTotalPrice();
					this.VerifyWarningOverCeiling();
				}
				else
				{
					services = this.ServiceIsleafADOs;
					if ((this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true")
						this.toggleSwitchDataChecked.EditValue = false;
					this.SetDefaultSerServTotalPrice();
				}
				this.gridControlServiceProcess.DataSource = null;
				this.gridControlServiceProcess.DataSource = services;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void ResetServiceGroupSelected()
		{
			try
			{
				if (this.ServiceIsleafADOs.Any(o => o.IsChecked == true))
				{
					foreach (var item in this.ServiceIsleafADOs)
					{
						if (item.SERVICE_GROUP_ID_SELECTEDs != null && item.SERVICE_GROUP_ID_SELECTEDs.Count > 0)
						{
							item.IsChecked = false;
							item.SERVICE_GROUP_ID_SELECTEDs = null;
							item.TDL_EXECUTE_ROOM_ID = 0;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ResetServiceKskSelected()
		{
			try
			{
				if (this.ServiceIsleafADOs.Any(o => o.IsChecked == true))
				{
					this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
					foreach (var item in this.ServiceIsleafADOs)
					{
						if (item.IsServiceKsk)
						{
							item.IsChecked = false;
							item.AssignSurgPriceEdit = null;
							item.IsServiceKsk = false;
							item.TDL_EXECUTE_ROOM_ID = 0;
						}
					}
				}
				this.gridControlServiceProcess.DataSource = ServiceIsleafADOs;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private Rectangle GetClientRectangle(Control control, ref int heightPlus)
		{
			Rectangle bounds = default(Rectangle);
			if (control != null)
			{
				bounds = control.Bounds;
				if (control.Parent != null && !(control is UserControl))
				{
					heightPlus += bounds.Y;
					return GetClientRectangle(control.Parent, ref heightPlus);
				}
			}
			return bounds;
		}

		private Rectangle GetAllClientRectangle(Control control, ref int heightPlus)
		{
			Rectangle bounds = default(Rectangle);
			if (control != null)
			{
				bounds = control.Bounds;
				if (control.Parent != null)
				{
					heightPlus += bounds.Y;
					return GetAllClientRectangle(control.Parent, ref heightPlus);
				}
			}
			return bounds;
		}

		void ProcessShowpopupControlContainerRoom()
		{
			int heightPlus = 0;
			Rectangle bounds = GetClientRectangle(this.beditRoom, ref heightPlus);
			Rectangle bounds1 = GetAllClientRectangle(this.Parent, ref heightPlus);
			if (bounds == null)
			{
				bounds = beditRoom.Bounds;
			}

			//xử lý tính toán lại vị trí hiển thị popup tương đối phụ thuộc theo chiều cao của popup, kích thước màn hình, đối tượng bệnh nhân(bhyt/...)
			if (bounds1.Height <= 768)
			{
				if (popupHeight == 400)
				{
					heightPlus = bounds.Y >= 650 ? -125 : (bounds.Y > 410 ? (-262) : (bounds.Y < 230 ? (-bounds.Y - 227) : -276));
				}
				else
					heightPlus = bounds.Y >= 650 ? -60 : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 27 : -78)));
			}
			else
			{
				if (popupHeight == 400)
				{
					heightPlus = bounds.Y >= 650 ? -327 : (bounds.Y > 410 ? -260 : (bounds.Y < 230 ? (-bounds.Y - 225) : -608));
				}
				else
					heightPlus = bounds.Y >= 650 ? (-122) : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 25 : -180)));
			}

			Rectangle buttonBounds = new Rectangle(beditRoom.Bounds.X + 100, bounds.Y, bounds.Width, bounds.Height);
			popupControlContainerRoom.ShowPopup(new Point(beditRoom.Bounds.X, beditRoom.Bounds.Y + beditRoom.Bounds.Height + 25));
			gridViewContainerRoom.Focus();
			gridViewContainerRoom.FocusedRowHandle = 0;
		}
		void ProcessShowpopupControlContainerAssignOld()
		{
			int heightPlus = 0;
			Rectangle bounds = GetClientRectangle(this.cboPriviousServiceReq, ref heightPlus);
			Rectangle bounds1 = GetAllClientRectangle(this.Parent, ref heightPlus);
			if (bounds == null)
			{
				bounds = cboPriviousServiceReq.Bounds;
			}

			//xử lý tính toán lại vị trí hiển thị popup tương đối phụ thuộc theo chiều cao của popup, kích thước màn hình, đối tượng bệnh nhân(bhyt/...)
			if (bounds1.Height <= 768)
			{
				if (popupHeight == 400)
				{
					heightPlus = bounds.Y >= 650 ? -125 : (bounds.Y > 410 ? (-262) : (bounds.Y < 230 ? (-bounds.Y - 227) : -276));
				}
				else
					heightPlus = bounds.Y >= 650 ? -60 : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 27 : -78)));
			}
			else
			{
				if (popupHeight == 400)
				{
					heightPlus = bounds.Y >= 650 ? -327 : (bounds.Y > 410 ? -260 : (bounds.Y < 230 ? (-bounds.Y - 225) : -608));
				}
				else
					heightPlus = bounds.Y >= 650 ? (-122) : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 25 : -180)));
			}

			Rectangle buttonBounds = new Rectangle(cboPriviousServiceReq.Bounds.X + 100, bounds.Y, bounds.Width, bounds.Height);
			popupControlContainer3.ShowPopup(new Point(cboPriviousServiceReq.Bounds.X, cboPriviousServiceReq.Bounds.Y + cboPriviousServiceReq.Bounds.Height + 25));
			gridView14.Focus();
			gridView14.FocusedRowHandle = 0;
		}
		private void beditRoom_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Down)
				{
					ProcessShowpopupControlContainerRoom();
				}
				else if (e.Control && e.KeyCode == Keys.A)
				{
					beditRoom.SelectAll();
					e.Handled = true;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void beditRoom_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (!String.IsNullOrEmpty(beditRoom.Text))
				{
					beditRoom.Refresh();
					//if (beditRoom.Text.Contains(",") || txtRoomCode.Text.Contains(","))
					//{
					//    isShowContainerMediMatyForChoose = true;
					//}
					if (isShowContainerMediMatyForChoose)
					{
						gridViewContainerRoom.ActiveFilter.Clear();
					}
					else
					{
						if (!isShowContainerMediMaty)
						{
							isShowContainerMediMaty = true;
						}

						//Filter data
						gridViewContainerRoom.ActiveFilterString = String.Format("[SERVICE_GROUP_NAME] Like '%{0}%' OR [SERVICE_GROUP_CODE] Like '%{0}%' OR [SERVICE_GROUP_NAME__UNSIGN] Like '%{0}%'", beditRoom.Text);
						gridViewContainerRoom.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
						gridViewContainerRoom.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
						gridViewContainerRoom.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
						gridViewContainerRoom.FocusedRowHandle = 0;
						gridViewContainerRoom.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
						gridViewContainerRoom.OptionsFind.HighlightFindResults = true;

						if (isShow)
						{
							ProcessShowpopupControlContainerRoom();
							isShow = false;
						}

						beditRoom.Focus();
					}
					isShowContainerMediMatyForChoose = false;
				}
				else
				{
					gridViewContainerRoom.ActiveFilter.Clear();
					if (!isShowContainerMediMaty)
					{
						popupControlContainerRoom.HidePopup();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void beditRoom_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
				{
					ProcessShowpopupControlContainerRoom();
				}
				else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
				{
					this.beditRoom.EditValue = null;
					this.beditRoom.Properties.Buttons[1].Visible = false;
					DevExpress.XtraGrid.Columns.GridColumn gridColumnCheck = gridViewContainerRoom.Columns["IsChecked"];
					if (gridColumnCheck != null)
					{
						gridColumnCheck.ImageAlignment = StringAlignment.Center;
						gridColumnCheck.Image = this.imageCollection1.Images[0];
					}
					this.gridControlServiceProcess.DataSource = null;
					this.selectedSeviceGroups = null;
					this.workingServiceGroupADOs.ForEach(o => o.IsChecked = false);
					this.ResetServiceGroupSelected();
					this.toggleSwitchDataChecked.EditValue = false;
					this.SetEnableButtonControl(this.actionType);
					this.SetDefaultSerServTotalPrice();
				}
				else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
				{
					List<object> listArgs = new List<object>();
					listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);
					HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisServSegr", currentModule.RoomId, currentModule.RoomTypeId, listArgs);

					HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>();
					InitComboServiceGroup();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewContainerRoom_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				ColumnView View = (ColumnView)gridControlContainerRoom.FocusedView;
				if (e.KeyCode == Keys.Space)
				{
					if (this.gridViewContainerRoom.IsEditing)
						this.gridViewContainerRoom.CloseEditor();

					if (this.gridViewContainerRoom.FocusedRowModified)
						this.gridViewContainerRoom.UpdateCurrentRow();

					var rawRoom = (ServiceGroupADO)this.gridViewContainerRoom.GetFocusedRow();

					if (rawRoom != null && rawRoom.IsChecked)
					{
						rawRoom.IsChecked = false;
					}
					else if (rawRoom != null)
					{
						rawRoom.IsChecked = true;
					}
					gridControlContainerRoom.RefreshDataSource();
					ProcessDisplayRoomWithData();
				}
				else if (e.KeyCode == Keys.Enter)
				{
					var rawRoom = (ServiceGroupADO)this.gridViewContainerRoom.GetFocusedRow();
					if ((workingServiceGroupADOs != null && !workingServiceGroupADOs.Any(o => o.IsChecked)) && rawRoom != null)
					{
						rawRoom.IsChecked = true;
						gridControlContainerRoom.RefreshDataSource();
						ProcessDisplayRoomWithData();
					}
					isShowContainerMediMaty = false;
					isShowContainerMediMatyForChoose = true;
					popupControlContainerRoom.HidePopup();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridViewContainerRoom_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
		{
			try
			{

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridViewContainerRoom_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
				{
					GridView view = sender as GridView;
					GridHitInfo hi = view != null ? view.CalcHitInfo(e.Location) : null;

					if (hi != null && hi.Column != null && hi.Column.FieldName == "IsChecked" && hi.InRowCell)
					{
						if (hi.Column.RealColumnEdit != null && hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
						{
							view.FocusedRowHandle = hi.RowHandle;
							view.FocusedColumn = hi.Column;
							view.ShowEditor();
							CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
							DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
							Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
							GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
							Rectangle gridGlyphRect =
								new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
								 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
								 glyphRect.Width,
								 glyphRect.Height);
							if (!gridGlyphRect.Contains(e.Location))
							{
								view.CloseEditor();
								if (!view.IsCellSelected(hi.RowHandle, hi.Column))
								{
									view.SelectCell(hi.RowHandle, hi.Column);
								}
								else
								{
									view.UnselectCell(hi.RowHandle, hi.Column);
								}
							}
							else
							{
								checkEdit.Checked = !checkEdit.Checked;
								view.CloseEditor();
							}

							gridControlContainerRoom.RefreshDataSource();
							ProcessDisplayRoomWithData();

							(e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
						}
					}
					else if (hi != null && hi.Column != null && hi.Column.FieldName == "IsChecked" && hi.InColumnPanel)
					{
						statecheckColumn = !statecheckColumn;
						this.SetCheckAllColumn(statecheckColumn);
						var rawRoom = (ServiceGroupADO)this.gridViewContainerRoom.GetFocusedRow();
						long roomIdFocus = rawRoom != null ? rawRoom.ID : 0;
						this.workingServiceGroupADOs.ForEach(o => o.IsChecked = statecheckColumn);
						var roomFocus = this.workingServiceGroupADOs.FirstOrDefault(o => o.ID == roomIdFocus);
						if (roomFocus != null)
						{
							roomFocus.IsChecked = !roomFocus.IsChecked;
						}
						gridControlContainerRoom.RefreshDataSource();
						ProcessDisplayRoomWithData();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetCheckAllColumn(bool state)
		{
			try
			{
				DevExpress.XtraGrid.Columns.GridColumn gridColumnCheck = gridViewContainerRoom.Columns["IsChecked"];
				gridColumnCheck.ImageAlignment = StringAlignment.Center;
				gridColumnCheck.Image = (state ? this.imageCollection1.Images[1] : this.imageCollection1.Images[0]);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridControlContainerRoom_ProcessGridKey(object sender, KeyEventArgs e)
		{
			try
			{
				GridControl grid = sender as GridControl;
				GridView view = grid.FocusedView as GridView;
				if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
				{
					if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0))
					{
						if (view.IsEditing)
							view.CloseEditor();
						//grid.SelectNextControl(btnChoice, e.Modifiers == Keys.None, false, false, true);
						e.Handled = true;
					}
				}
				else if (e.KeyCode == Keys.Space)
				{
					if (this.gridViewContainerRoom.IsEditing)
						this.gridViewContainerRoom.CloseEditor();

					if (this.gridViewContainerRoom.FocusedRowModified)
						this.gridViewContainerRoom.UpdateCurrentRow();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridControlContainerRoom_Click(object sender, EventArgs e)
		{
			try
			{
				var rawRoom = (ServiceGroupADO)this.gridViewContainerRoom.GetFocusedRow();
				if (rawRoom != null)
				{
					rawRoom.IsChecked = !rawRoom.IsChecked;
					gridControlContainerRoom.RefreshDataSource();
					isShowContainerMediMaty = true;
					ProcessDisplayRoomWithData();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void popupControlContainerRoom_CloseUp(object sender, EventArgs e)
		{
			try
			{
				//bool isExistsCheckeds = this.workingServiceGroupADOs.Any(o => o.IsChecked);
				//this.beditRoom.Properties.Buttons[1].Visible = isExistsCheckeds;
				isShow = true;
				//if (isExistsCheckeds)
				//{
				//    this.isNotHandlerWhileChangeToggetSwith = true;
				//    this.ResetServiceGroupSelected();
				//    this.selectedSeviceGroups = this.workingServiceGroupADOs.Where(o => o.IsChecked).ToList();
				//    this.SelectOneServiceGroupProcess(this.selectedSeviceGroups);
				//    this.isNotHandlerWhileChangeToggetSwith = false;

				//    //this.FocusTotxtExamServiceCode();
				//}
				//else
				//{
				//    //try
				//    //{
				//    //    if (this.dlgFocusNextUserControl != null)
				//    //    {
				//    //        this.dlgFocusNextUserControl();
				//    //    }
				//    //}
				//    //catch (Exception ex)
				//    //{
				//    //    Inventec.Common.Logging.LogSystem.Warn(ex);
				//    //    SendKeys.Send("{TAB}");
				//    //}
				//}
				this.selectedSeviceGroupCopys = this.workingServiceGroupADOs.Where(o => o.IsChecked).ToList();

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		void ProcessDisplayRoomWithData()
		{
			StringBuilder sbText = new StringBuilder();
			StringBuilder sbCode = new StringBuilder();
			var currentRoomExts = this.workingServiceGroupADOs.Where(o => o.IsChecked).ToList();
			if (currentRoomExts != null && currentRoomExts.Count > 0)
			{
				foreach (ServiceGroupADO rv in currentRoomExts)
				{
					if (rv != null)
					{
						if (sbText.ToString().Length > 0) { sbText.Append(", "); }
						sbText.Append(rv.SERVICE_GROUP_NAME);
						if (sbCode.ToString().Length > 0) { sbCode.Append(", "); }
						sbCode.Append(rv.SERVICE_GROUP_CODE);
					}
				}
			}
			this.beditRoom.Properties.Buttons[1].Visible = (currentRoomExts != null && currentRoomExts.Count > 0);
			isShowContainerMediMatyForChoose = true;
			this.beditRoom.Text = sbText.ToString();

			this.isNotHandlerWhileChangeToggetSwith = true;
			//this.ResetServiceGroupSelected();//TODO

			if (currentRoomExts == null || currentRoomExts.Count == 0)
			{
				serviceDeleteWhileSelectSeviceGroups = new List<SereServADO>();
			}

			this.selectedSeviceGroups = currentRoomExts;
			this.SelectOneServiceGroupProcess(this.selectedSeviceGroups);
			this.SetDefaultSerServTotalPrice();
			this.isNotHandlerWhileChangeToggetSwith = false;
		}

		void ProcessDisplayKskWithData(List<HIS_KSK_SERVICE> lstkskService)
		{
			try
			{
				this.isNotHandlerWhileChangeToggetSwith = true;
				this.ShowKskServiceProcess(lstkskService);
				this.SetDefaultSerServTotalPrice();
				this.isNotHandlerWhileChangeToggetSwith = false;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboServiceGroup_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				SetDefaultSerServTotalPrice();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboExecuteGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == ButtonPredefines.Delete)
				{
					this.cboExecuteGroup.EditValue = null;
					this.cboExecuteGroup.Properties.Buttons[1].Visible = false;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboExecuteGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
		{
			try
			{
				if (e.CloseMode == PopupCloseMode.Normal)
				{
					if (this.cboExecuteGroup.EditValue != null)
					{
						MOS.Filter.HisExecuteGroupFilter filter = new MOS.Filter.HisExecuteGroupFilter();
						filter.ID = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboExecuteGroup.EditValue ?? "0").ToString());
						MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP data = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXECUTE_GROUP>>("api/HisExecuteGroup/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, new CommonParam()).FirstOrDefault();
						if (data != null)
							this.cboExecuteGroup.Properties.Buttons[1].Visible = true;
					}
					this.txtDescription.Focus();
					this.txtDescription.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboExecuteGroup_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (this.cboExecuteGroup.EditValue != null)
					{
						MOS.Filter.HisExecuteGroupFilter filter = new MOS.Filter.HisExecuteGroupFilter();
						filter.ID = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboExecuteGroup.EditValue ?? "0").ToString());
						MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP data = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXECUTE_GROUP>>("api/HisExecuteGroup/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, new CommonParam()).FirstOrDefault();
						if (data != null)
						{
							this.cboExecuteGroup.Properties.Buttons[1].Visible = true;
							this.txtDescription.Focus();
							this.txtDescription.SelectAll();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					txtLoginName.Focus();
					txtLoginName.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkEmergency_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					this.chkPriority.Properties.FullFocusRect = true;
					this.chkPriority.Focus();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkExpendAll_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					this.gridControlServiceProcess.Focus();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkExpendAll_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0)
				{
					bool isCheckExpendAll = (this.chkExpendAll.Checked);
					foreach (var item in this.ServiceIsleafADOs)
					{
						if (item.IsAllowExpend == (short)1 && !item.PackagePriceId.HasValue)
						{
							item.IsExpend = isCheckExpendAll;
						}
					}

					this.gridControlServiceProcess.RefreshDataSource();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboPriviousServiceReq_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
				{
					ProcessShowpopupControlContainerAssignOld();
				}
				else if (e.Button.Kind == ButtonPredefines.Delete)
				{
					this.rowdataAssignOld = null;
					this.cboPriviousServiceReq.Properties.Buttons[1].Visible = false;
					this.cboPriviousServiceReq.Text = null;
					this.gridControlServiceProcess.DataSource = null;
					foreach (var item in this.ServiceIsleafADOs)
					{
						item.IsChecked = false;
						item.TDL_EXECUTE_ROOM_ID = 0;
					}
					this.toggleSwitchDataChecked.EditValue = false;
					this.SetEnableButtonControl(this.actionType);
					this.SetDefaultSerServTotalPrice();
				}
				else if (e.Button.Kind == ButtonPredefines.Search)
				{
					WaitingManager.Show();
					LogSystem.Debug("Begin FillDataToComboPriviousServiceReq");
					this.FillDataToComboPriviousServiceReq();
					LogSystem.Debug("End FillDataToComboPriviousServiceReq");
					WaitingManager.Hide();
				}
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboPriviousServiceReq_Closed(object sender, ClosedEventArgs e)
		{
			try
			{
				if (e.CloseMode == PopupCloseMode.Normal)
				{
					if (this.cboPriviousServiceReq.EditValue != null && this.currentPreServiceReqs != null)
					{
						MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6 data = this.currentPreServiceReqs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPriviousServiceReq.EditValue ?? "0").ToString()));
						if (data != null)
						{
							this.cboPriviousServiceReq.Properties.Buttons[1].Visible = true;
							this.ProcessChoiceServiceReqPrevious(data);
							this.btnSave.Focus();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboPriviousServiceReq_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
		{
			try
			{
				if (e.FieldName == "RENDERER_INTRUCTION_TIME")
				{
					//var item = ((List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6>)this.cboPriviousServiceReq.Properties.DataSource)[e.RecordIndex];
					//if (item != null)
					//    e.Value = string.Format("{0}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.INTRUCTION_TIME));
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboPriviousServiceReq_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (rowdataAssignOld != null && this.currentPreServiceReqs != null)
					{
						cboPriviousServiceReq.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowdataAssignOld.INTRUCTION_TIME);
						this.cboPriviousServiceReq.Properties.Buttons[1].Visible = true;
						this.ProcessChoiceServiceReqPrevious(rowdataAssignOld);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboPriviousServiceReq_Leave(object sender, EventArgs e)
		{
			try
			{
				this.cboPriviousServiceReq.Properties.Buttons[1].Visible = rowdataAssignOld == null ? false : true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboCashierRoom_Closed(object sender, ClosedEventArgs e)
		{
			try
			{
				if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
				{
					if (cboCashierRoom.EditValue != null)
					{
						var account = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboCashierRoom.EditValue));
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboTracking_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == ButtonPredefines.Delete)
				{
					//cboTracking.Properties.Buttons[1].Visible = true;
					cboTracking.EditValue = null;
					GridCheckMarksSelection gridCheckMark = cboTracking.Properties.Tag as GridCheckMarksSelection;
					if (gridCheckMark != null)
					{
						gridCheckMark.ClearSelection(cboTracking.Properties.View);
					}
				}
				else if (e.Button.Kind == ButtonPredefines.Plus)
				{

					Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
					if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
					if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
					{
						List<object> listArgs = new List<object>();
						listArgs.Add(this.treatmentId);
						if (this.currentDhst != null)
						{
							listArgs.Add(this.currentDhst);
						}
						listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));

						listArgs.Add((Action<HIS_TRACKING>)ProcessAfterChangeTrackingTime);

						var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
						if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

						((Form)extenceInstance).ShowDialog();

						//Load lại tracking
						LoadDataToTrackingCombo();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboTracking_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					txtDescription.Focus();
					txtDescription.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboTracking_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				tracking = new HIS_TRACKING();
				cboTracking.Properties.Buttons[1].Visible = (cboTracking.EditValue != null);
				if (this.isInitTracking)
				{
					return;
				}
				else if (cboTracking.EditValue != null && !string.IsNullOrEmpty(cboTracking.EditValue.ToString()))
				{
					var trackingData = this.trackingAdos != null ? this.trackingAdos.FirstOrDefault(o => o.ID == (long)cboTracking.EditValue) : null;
					if (trackingData != null)
					{
						this.Listtrackings = new List<HIS_TRACKING>();
						Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRACKING>(tracking, trackingData);
						this.Listtrackings.Add(tracking);
						
					}
					else
					{
						this.Listtrackings = null;
					}
				}
				else
				{
					this.Listtrackings = null;
				}



				if (!HisConfigCFG.IsServiceReqIcdOption && HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
				{
					this.LoadIcdDefault();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void repositoryItemMemoExEdit_IntructionNote_Popup(object sender, EventArgs e)
		{
			try
			{
				MemoExPopupForm form = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as MemoExPopupForm;
				form.OkButton.Text = Inventec.Common.Resource.Get.Value("frmAssignService.InstructionNoteMemoEx.OkButtion", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				form.CloseButton.Text = Inventec.Common.Resource.Get.Value("frmAssignService.InstructionNoteMemoEx.CloseButtion", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkPrint_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (isNotLoadWhileChangeControlStateInFirst)
				{
					return;
				}
				isNotLoadWhileChangeControlStateInFirst = true;
				if (chkPrint.Checked)
					chkPrintDocumentSigned.Checked = !chkPrint.Checked;
				isNotLoadWhileChangeControlStateInFirst = false;
				ChangeCheckPrintAndPreview();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkSign_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				//chkPrintDocumentSigned.Enabled = chkSign.Checked;
				if (isNotLoadWhileChangeControlStateInFirst)
				{
					return;
				}
				WaitingManager.Show();
				//if (chkSign.Checked == false)
				//{
				//    chkPrintDocumentSigned.Checked = false;
				//}

				HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkSign && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
				if (csAddOrUpdate != null)
				{
					csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
				}
				else
				{
					csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
					csAddOrUpdate.KEY = ControlStateConstant.chkSign;
					csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
					csAddOrUpdate.MODULE_LINK = moduleLink;
					if (this.currentControlStateRDO == null)
						this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
					this.currentControlStateRDO.Add(csAddOrUpdate);
				}
				this.controlStateWorker.SetData(this.currentControlStateRDO);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkPrintDocumentSigned_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (isNotLoadWhileChangeControlStateInFirst)
				{
					return;
				}
				isNotLoadWhileChangeControlStateInFirst = true;
				if (chkPrintDocumentSigned.Checked)
					chkPrint.Checked = !chkPrintDocumentSigned.Checked;
				isNotLoadWhileChangeControlStateInFirst = false;
				ChangeCheckPrintAndPreview();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ChangeCheckPrintAndPreview()
		{
			try
			{
				HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdatePrintDocumentSigned = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkPrintDocumentSigned && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
				if (csAddOrUpdatePrintDocumentSigned != null)
				{
					csAddOrUpdatePrintDocumentSigned.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
				}
				else
				{
					csAddOrUpdatePrintDocumentSigned = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
					csAddOrUpdatePrintDocumentSigned.KEY = ControlStateConstant.chkPrintDocumentSigned;
					csAddOrUpdatePrintDocumentSigned.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
					csAddOrUpdatePrintDocumentSigned.MODULE_LINK = moduleLink;
					if (this.currentControlStateRDO == null)
						this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
					this.currentControlStateRDO.Add(csAddOrUpdatePrintDocumentSigned);
				}

				HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkPrint && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
				if (csAddOrUpdate != null)
				{
					csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
				}
				else
				{
					csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
					csAddOrUpdate.KEY = ControlStateConstant.chkPrint;
					csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
					csAddOrUpdate.MODULE_LINK = moduleLink;
					if (this.currentControlStateRDO == null)
						this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
					this.currentControlStateRDO.Add(csAddOrUpdate);
				}

				this.controlStateWorker.SetData(this.currentControlStateRDO);

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboAssignRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == ButtonPredefines.Delete)
				{
					this.cboAssignRoom.EditValue = null;
					this.cboAssignRoom.Properties.Buttons[1].Visible = false;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboAssignRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
		{
			try
			{
				if (e.CloseMode == PopupCloseMode.Normal)
				{
					if (this.cboAssignRoom.EditValue != null)
					{
						var data = this.assRoomsWorks.FirstOrDefault(o => o.ID == (long)this.cboAssignRoom.EditValue);
						if (data != null)
						{
							this.cboAssignRoom.Properties.Buttons[1].Visible = true;
							txtAssignRoomCode.Text = data.ROOM_CODE;
						}
					}
					this.txtLoginName.Focus();
					this.txtLoginName.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboAssignRoom_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (this.cboAssignRoom.EditValue != null)
					{
						var data = this.assRoomsWorks.FirstOrDefault(o => o.ID == (long)this.cboAssignRoom.EditValue);
						if (data != null)
						{
							this.cboAssignRoom.Properties.Buttons[1].Visible = true;
							txtAssignRoomCode.Text = data.ROOM_CODE;

							this.txtLoginName.Focus();
							this.txtLoginName.SelectAll();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtAssignRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
					if (String.IsNullOrEmpty(searchCode))
					{
						this.cboAssignRoom.EditValue = null;
						this.cboAssignRoom.Properties.Buttons[1].Visible = false;
						this.FocusShowpopup(cboAssignRoom, true);
					}
					else
					{
						var data = this.assRoomsWorks
							.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ROOM_CODE.ToUpper().Contains(searchCode.ToUpper())).ToList();

						var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.ROOM_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
						if (searchResult != null && searchResult.Count == 1)
						{
							this.cboAssignRoom.EditValue = searchResult[0].ID;
							this.cboAssignRoom.Properties.Buttons[1].Visible = true;
							this.txtAssignRoomCode.Text = searchResult[0].ROOM_CODE;
							this.txtLoginName.Focus();
							this.txtLoginName.SelectAll();
						}
						else
						{
							this.cboAssignRoom.EditValue = null;
							this.cboAssignRoom.Properties.Buttons[1].Visible = false;
							this.FocusShowpopup(cboAssignRoom, true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		#endregion


		#region bắt lỗi mã ICD khi lưu
		bool ValidForSave()
		{
			bool valid = true;
			try
			{
				if (!String.IsNullOrEmpty(this.txtIcdCode.Text))
				{
					var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(this.txtIcdCode.Text)).ToList();
					var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == this.txtIcdCode.Text).ToList() : listData) : null;
					if (result == null || result.Count <= 0)
					{
						txtIcdCode.DoValidate();
						//MessageBox.Show("Mã ICD bạn nhập không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						valid = false;
						return valid;
					}
					else
					{
						txtIcdCode.ErrorText = "";
						dxValidationProviderControl.RemoveControlError(txtIcdCode);
					}
				}

				this.dxValidationProviderControl.RemoveControlError(txtIcdCode);
				this.positionHandleControl = -1;
				valid = dxValidationProviderControl.Validate() && valid;
			}
			catch (Exception ex)
			{
				valid = false;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return valid;
		}
		#endregion

		private bool ValidForSaveGridPatientSelect(List<V_HIS_TREATMENT_BED_ROOM> lstTreatmentBedRoom)
		{
			bool valid = true;
			List<bool> lstValid = new List<bool>();
			try
			{
				foreach (var item in lstTreatmentBedRoom)
				{
					if (!String.IsNullOrEmpty(item.ICD_CODE))
					{
						var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(item.ICD_CODE)).ToList();
						var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == item.ICD_CODE).ToList() : listData) : null;
						if (result == null || result.Count <= 0)
						{
							Inventec.Common.Logging.LogSystem.Warn("CASE 1");
							if (!dicValidIcd.ContainsKey(item.TREATMENT_CODE))
								dicValidIcd[item.TREATMENT_CODE] = item.TDL_PATIENT_NAME;
							lstValid.Add(false);
						}
					}
					else
					{
						Inventec.Common.Logging.LogSystem.Warn("CASE 2");
						if (!dicValidIcd.ContainsKey(item.TREATMENT_CODE))
							dicValidIcd[item.TREATMENT_CODE] = item.TDL_PATIENT_NAME;
						lstValid.Add(false);
					}
				}
				this.positionHandleControl = -1;
				valid = lstValid != null && lstValid.Count > 0 ? (lstValid.IndexOf(false) > -1 ? false : true) : true;
			}
			catch (Exception ex)
			{
				valid = false;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return valid;
		}

		#region Button
		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				IsActionButtonPrintBill = false;
				SaveWithGridpatientSelect(TypeButton.SAVE, chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ValidGenderServiceAllowGridpatientSelect(List<V_HIS_TREATMENT_BED_ROOM> lst, List<SereServADO> serviceCheckeds__Send, ref string MessageGender, ref string MessageAge, ref string MessageType)
		{
			try
			{

				foreach (var item in lst.GroupBy(o => o.TREATMENT_ID))
				{
					var genderCheck = GetDiffGender(serviceCheckeds__Send, item.First().TDL_PATIENT_GENDER_ID);
					if (genderCheck != null && genderCheck.Count() > 0)
					{
						string gender = genderCheck.FirstOrDefault().GENDER_ID == 1 ? "nữ" : "nam";
						MessageGender += "Dịch vụ không chỉ định cho giới tính " + gender + ": " + String.Join("; ", genderCheck.Select(o => o.TDL_SERVICE_NAME).ToArray()) + "\r\n";
					}

					// check tuổi từ - đến (DVKT)
					var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisTreatment.TDL_PATIENT_DOB);
					//int age = DateTime.Now.Year - int.Parse(this.currentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4));
					TimeSpan timeSpan2 = System.DateTime.Now.Date - ageDate.Value.Date;
					long ticks = timeSpan2.Ticks;
					System.DateTime dateTime = new System.DateTime(ticks);
					int ageMonth = (dateTime.Year - 1) * 12 + dateTime.Month - 1;
					//int ageMonth = (DateTime.Now - (ageDate ?? DateTime.Now)).Days / 30;
					Inventec.Common.Logging.LogSystem.Debug("age: " + ageMonth);

					var checkAge = serviceCheckeds__Send.Where(o => (o.AGE_FROM.HasValue && o.AGE_FROM > ageMonth) || (o.AGE_TO.HasValue && o.AGE_TO < ageMonth));

					if (checkAge != null && checkAge.Count() > 0)
					{
						MessageAge += "Độ tuổi của bệnh nhân " + item.First().TDL_PATIENT_NAME + " có mã điều trị " + item.First().TREATMENT_CODE + " không phù hợp với điều kiện của dịch vụ " + String.Join("; ", checkAge.Select(o => o.TDL_SERVICE_NAME).ToArray()) + "\r\n";
					}

					// check dịch vụ giường với diện điều trị là khám, điều trị ngoại trú, điều trị ban ngày
					//if (item.First().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM || item.First().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY || item.First().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
					//{
					//    var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == item.First().TDL_TREATMENT_TYPE_ID);
					//    var dichVuGiuong = serviceCheckeds__Send.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
					//    if (dichVuGiuong != null && dichVuGiuong.Count() > 0 && treatmentType != null)
					//    {
					//        MessageType += "Diện điều trị của bệnh nhân " + item.First().TDL_PATIENT_NAME + " có mã điều trị " + item.First().TREATMENT_CODE + " là " + treatmentType.TREATMENT_TYPE_NAME;                          
					//    }
					//}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private bool ValidICD()
        {
			bool isValid = true;
            try
            {
				int icd_code = txtIcdCode.Text.Length;
				Inventec.Common.Logging.LogSystem.Debug("Do dai icd code: " + icd_code);
				int icd_name = Inventec.Common.String.CountVi.Count(cboIcds.Text) ?? 0;
				Inventec.Common.Logging.LogSystem.Debug("Do dai icd name: " + icd_name);
				int icd_sub_code = txtIcdSubCode.Text.Length;
				Inventec.Common.Logging.LogSystem.Debug("Do dai icd sub code: " + icd_sub_code);
				int icd_text = Inventec.Common.String.CountVi.Count(txtIcdText.Text) ?? 0;
				Inventec.Common.Logging.LogSystem.Debug("Do dai icd sub name: " + icd_text);
				int icd_yhct_len = 0;
				int icd_yhct_sub_len = 0;
				if(this.ucIcdYhct != null)
                {
					var icdValue = this.icdYhctProcessor.GetValue(ucIcdYhct);
					if(icdValue != null && icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                    {
						var rs = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
						if (rs != null) icd_yhct_len = rs.ToString().Length;
						Inventec.Common.Logging.LogSystem.Debug("Do dai icd yhct code: " + icd_yhct_len);
					}

				}
				if (this.ucSecondaryIcdYhct != null)
				{
					var subIcd = this.subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
					if (subIcd != null)//&& subIcd is SecondaryIcdDataADO
					{
						var rs = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
						if (rs != null) icd_yhct_sub_len = rs.ToString().Length;
						Inventec.Common.Logging.LogSystem.Debug("Do dai icd yhct sub code: " + icd_yhct_sub_len);
					}
				}

				string errror_string = "";
				if (icd_code + icd_sub_code > 100)
				{
					errror_string = "Mã chẩn đoán phụ nhập quá 100 ký tự";
				}
				else if (icd_name + icd_text > 1500)
				{
					errror_string = "Tên chẩn đoán phụ nhập quá 1500 ký tự";
				}
				else if (icd_yhct_len + icd_yhct_sub_len > 255)
				{
					errror_string = "Mã chẩn đoán YHCT phụ nhập quá 255 ký tự";
				}
				if (!string.IsNullOrEmpty(errror_string))
				{
					MessageBox.Show(this, errror_string, "Thông báo", MessageBoxButtons.OK);
					isValid = false;
				}


			}
            catch (Exception ex)
            {
				isValid = false;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return isValid;
		}
		private bool CheckIcd(List<V_HIS_TREATMENT_BED_ROOM> lst)
		{
			bool valid = true;
			try
			{
				string messErr = null;
				foreach (var item in lst)
				{
					if (HisConfigCFG.CheckIcdWhenSave == "1" || HisConfigCFG.CheckIcdWhenSave == "2")
					{
						currentTreatment = GetTreatment(item.TREATMENT_ID);
						InitCheckIcdManager();
						//viec 178289 thinhdt1 them bien vao thu vien khi luu
						if (!checkIcdManager.ProcessCheckIcd(item.ICD_CODE, item.ICD_SUB_CODE, ref messErr, HisConfigCFG.CheckIcdWhenSave == "1" || HisConfigCFG.CheckIcdWhenSave == "2",true))
						{
							if (HisConfigCFG.CheckIcdWhenSave == "1")
							{
								if (DevExpress.XtraEditors.XtraMessageBox.Show((!string.IsNullOrEmpty(item.TREATMENT_CODE) ? item.TREATMENT_CODE + ": " : null) + messErr + ". Bạn có muốn tiếp tục?",
							 HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
							 MessageBoxButtons.YesNo) == DialogResult.No) valid = false;
							}
							else
							{
								DevExpress.XtraEditors.XtraMessageBox.Show((!string.IsNullOrEmpty(item.TREATMENT_CODE) ? item.TREATMENT_CODE + ": " : null) + messErr,
							 HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
							 MessageBoxButtons.OK);
								valid = false;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				valid = false;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return valid;
		}
		private void SaveWithGridpatientSelect(TypeButton type, bool isSaveAndPrint, bool printTH, bool isSaveAndShow, bool isSign = false, bool isPrintDocumentSigned = false)
		{
			try
			{
                if (!string.IsNullOrEmpty(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
				{
					LoadVServiceReq();
					if (vServiceReq != null && vServiceReq.START_TIME.HasValue && Inventec.Common.DateTime.Calculation.DifferenceTime(vServiceReq.START_TIME.Value, InstructionTime, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.SECOND) < Int32.Parse(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
					{
						DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Thời gian chỉ định {0} phải cách thời gian bắt đầu khám {1} là {2} giây mới được phép chỉ định", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(InstructionTime), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vServiceReq.START_TIME.Value), HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
						return;
					}
				}
				assignMulti = false;
				if (workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null)
				{
					dicValidIcd = new Dictionary<string, string>();
					ListMessError = new List<string>();
					var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
					if (lstPatientSelect != null && lstPatientSelect.Count > 1)
					{
						icdServicePhacDos = null;
						assignMulti = true;
						var actionTmp = actionType;
						bool isValid = true;
						List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
						if (serviceTypeIdRequired != null && serviceTypeIdRequired.Count > 0)
						{
							var serviceTypeInGrid = serviceCheckeds__Send.Select(o => new { o.TDL_SERVICE_NAME, o.SERVICE_TYPE_ID, o.TEST_SAMPLE_TYPE_ID }).ToList();
							var lstServiceName = serviceTypeInGrid.Where(item => serviceTypeIdRequired.Exists(o => o == item.SERVICE_TYPE_ID) && item.TEST_SAMPLE_TYPE_ID <= 0).Select(o => o.TDL_SERVICE_NAME);
							if (lstServiceName != null && lstServiceName.Count() > 0)
							{
								DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dịch vụ {0} bắt buộc chọn Loại mẫu bệnh phẩm xét nghiệm", String.Join(", ", lstServiceName.ToList())), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
								return;
							}
						}
						isValid = isValid && this.Valid(serviceCheckeds__Send);
						isValid = isValid && this.CheckIcd(lstPatientSelect);
						isValid = isValid && this.ValidICD();

						if (!ValidForSaveGridPatientSelect(lstPatientSelect))
						{
							string message = "Các bệnh nhân sau có mã ICD không hợp lệ";
							message += "<br>";
							foreach (var item in dicValidIcd)
							{
								message += string.Format("Bệnh nhân {0} (mã điều trị: {1})", item.Value, item.Key);
							}
							XtraMessageBox.Show(message, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
							return;
						}


						string MessGender = "";
						string MessAge = "";
						string MessType = "";
						#region Valid ICD
						var icd = lstPatientSelect.Select(o => o.ICD_CODE).ToList();
						var icdSub = lstPatientSelect.Where(o => !string.IsNullOrEmpty(o.ICD_SUB_CODE)).Select(o => o.ICD_SUB_CODE).ToList();
						MOS.Filter.HisIcdFilter icdFilter = new HisIcdFilter();
						icdFilter.ICD_CODEs = icd;
						var icdData = new BackendAdapter(null).Get<List<HIS_ICD>>("api/HisIcd/Get", ApiConsumer.ApiConsumers.MosConsumer, icdFilter, null);
						if (icdData != null && icdData.Count > 0)
						{
							MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
							icdServiceFilter.ICD_CODE__EXACTs = icd;
							icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);

							//isValid = isValid && ValidServiceIcdForIcdSelected(icdServices, serviceCheckeds__Send);
							isValid = isValid && ValidServiceIcdForServiceSelected(icdData, icdServicePhacDos, serviceCheckeds__Send);
							if (!isValid && HisConfigCFG.IcdServiceHasCheck == "4")
								isValid = false;
						}
						else if (HisConfigCFG.IcdServiceHasCheck == "3" && serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
						{
							MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
							icdServiceFilter.SERVICE_IDs = serviceCheckeds__Send.Select(o => o.SERVICE_ID).Distinct().ToList();
							icdServicePhacDos = new BackendAdapter(new CommonParam()).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);

							if (icdServicePhacDos != null && icdServicePhacDos.Count > 0 && icdData != null && icdData.Count > 0)
							{
								icdServicePhacDos = icdServicePhacDos.Where(o => !icdData.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).ToList();
							}
							if (icdServicePhacDos != null && icdServicePhacDos.Count > 0)
							{
								frmMissingIcd frmWaringConfigIcdService = new frmMissingIcd(icdData, serviceCheckeds__Send, this.currentModule, icdServicePhacDos, getDataFromMissingIcdDelegate);
								frmWaringConfigIcdService.ShowDialog();
							}
						}
						#endregion
						#region Valid ServiceAllow
						ValidGenderServiceAllowGridpatientSelect(lstPatientSelect, serviceCheckeds__Send, ref MessGender, ref MessAge, ref MessType);
						bool IsValidBed = ValidCheckTreatmentTypeBed(serviceCheckeds__Send, ref MessType, lstPatientSelect);
						if (!string.IsNullOrEmpty(MessGender))
						{
							XtraMessageBox.Show(MessGender, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
							return;
						}
						if (!string.IsNullOrEmpty(MessAge))
						{
							XtraMessageBox.Show(MessAge, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
							return;
						}
						if (!string.IsNullOrEmpty(MessType))
						{
							if ((HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "1" && MessageBox.Show(MessType + ResourceMessage.KhongPhaiNoiTruChiDinhGiuong, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes) || (HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "2" && MessageBox.Show(MessType + ResourceMessage.ChanKhongPhaiNoiTruChiDinhGiuong, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK))
							{
								return;
							}
							//if (MessageBox.Show(MessType + ResourceMessage._BanCoMuonChiDinhGiuong, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
							//    return;
						}
						#endregion
						#region ValidSereServWithMinDuration
						List<HIS_SERE_SERV> sereServMinDurations = new List<HIS_SERE_SERV>();                       
						foreach (var item in lstPatientSelect)
						{
							var dt = getSereServWithMinDuration(serviceCheckeds__Send, item.PATIENT_ID);
							if (dt != null)
								sereServMinDurations.AddRange(dt);
						}
						if (sereServMinDurations != null && sereServMinDurations.Count > 0)
						{
							sereServMinDurations = sereServMinDurations.Distinct().ToList();
							string sereServMinDurationStr = "";
							foreach (var item in sereServMinDurations)
							{
								sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + " - " +
								   Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.TDL_INTRUCTION_TIME) +
								   " (" + item.TDL_SERVICE_REQ_CODE +
								   "); ";
							}
							
							if (HisConfigCFG.IsSereServMinDurationAlert == 1)
							{
								if (MessageBox.Show(string.Format(ResourceMessage.SereServMinDurationAlert__BanCoMuonChuyenDoiDTTTSangVienPhi,string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr)), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
								{
									foreach (var sv in serviceCheckeds__Send)
									{
										//Thực hiện tự động chuyển đổi đối tượng sang viện phí                     
										if (sereServMinDurations.Any(o => o.SERVICE_ID == sv.SERVICE_ID))
										{
											sv.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__VP;
										}
									}
								}
								else
								{
									return;
								}
							}
							else if (HisConfigCFG.IsSereServMinDurationAlert == 2)
							{
                                if (MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    if (!assignMulti)
                                    {
                                        switch (type)
                                        {
                                            case TypeButton.SAVE:
                                                LogTheadInSessionInfo(() => ProcessSaveData(chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked), "SaveAssignServiceDefault");
                                                break;
                                            case TypeButton.SAVE_AND_PRINT:
                                                LogTheadInSessionInfo(() => ProcessSaveData(true, false, false), "SaveAndPrintAssignServiceDefault");
                                                break;
                                            case TypeButton.EDIT:
                                                LogTheadInSessionInfo(() => ProcessSaveData(chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked), "EditAssignServiceDefault");
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                else
								{
									return;
								}
							}
							else
							{
								if (HisConfigCFG.IsSereServMinDurationAlert == 0 || (HisConfigCFG.IsSereServMinDurationAlert != 1 && HisConfigCFG.IsSereServMinDurationAlert != 2))
								{
                                    if (MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        if (!assignMulti)
                                        {
                                            switch (type)
                                            {
                                                case TypeButton.SAVE:
                                                    LogTheadInSessionInfo(() => ProcessSaveData(chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked), "SaveAssignServiceDefault");
                                                    break;
                                                case TypeButton.SAVE_AND_PRINT:
                                                    LogTheadInSessionInfo(() => ProcessSaveData(true, false, false), "SaveAndPrintAssignServiceDefault");
                                                    break;
                                                case TypeButton.EDIT:
                                                    LogTheadInSessionInfo(() => ProcessSaveData(chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked), "EditAssignServiceDefault");
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    else
									{
										return;
									}
								}
							}
						}
						#endregion
						isValid = isValid && ValidSereServWithCondition(serviceCheckeds__Send);
						isValid = isValid && CheckMaxPatientbyDayOption(serviceCheckeds__Send);
						List<string> lstIcd = new List<string>();
						if (!string.IsNullOrEmpty(txtIcdCode.Text))
						{
							var arrIcdCode = txtIcdCode.Text.Trim().Split(';');
							foreach (var item in arrIcdCode)
							{
								if (!string.IsNullOrEmpty(item))
									lstIcd.Add(item);
							}
						}
						List<string> lstSubIcd = new List<string>();
						if (!string.IsNullOrEmpty(txtIcdSubCode.Text))
						{
							var arrIcdCode = txtIcdSubCode.Text.Trim().Split(';');
							foreach (var item in arrIcdCode)
							{
								if (!string.IsNullOrEmpty(item))
									lstSubIcd.Add(item);
							}
						}
						isValid = isValid && checkContraindicated(lstIcd, lstSubIcd, icdServicePhacDos, serviceCheckeds__Send);
						isValid = isValid && ValidSereServWithOtherPaySource(serviceCheckeds__Send);
						IsTreatmentInBedRoom = true;
						isValid = isValid && ValidSereServWithBed(serviceCheckeds__Send);
						foreach (var item in lstPatientSelect)
						{
							ValidConsultationReqiured(serviceCheckeds__Send, item.TREATMENT_ID);
							isValid = isValid && CheckMaxAmount(serviceCheckeds__Send, new List<long>() { item.TREATMENT_ID });
						}
						if (isValid)
						{
							ChangeLockButtonWhileProcess(false);

							foreach (var item in lstPatientSelect)
							{
								AssignServiceSDO serviceReqSDO = new AssignServiceSDO();
								serviceReqSDO.ServiceReqDetails = new List<ServiceReqDetailSDO>();
								bool isDupicate = false;
								this.ProcessServiceReqSDO(serviceReqSDO, serviceCheckeds__Send, ref isDupicate, item.TREATMENT_ID, false);
								serviceReqSDO.IcdCode = item.ICD_CODE;
								serviceReqSDO.IcdName = item.ICD_NAME;
								serviceReqSDO.IcdCauseCode = item.ICD_CAUSE_CODE;
								serviceReqSDO.IcdCauseName = item.ICD_CAUSE_NAME;
								serviceReqSDO.IcdSubCode = item.ICD_SUB_CODE;
								serviceReqSDO.IcdText = item.ICD_TEXT;
								currentHisPatientTypeAlter.PATIENT_TYPE_ID = item.TDL_PATIENT_TYPE_ID ?? 0;
								currentHisPatientTypeAlter.TREATMENT_TYPE_ID = item.TDL_TREATMENT_TYPE_ID ?? 0;
								if (this.ServiceAttachForServicePrimary(ref serviceReqSDO, this.currentHisPatientTypeAlter.PATIENT_TYPE_ID))
								{
									this.SaveServiceReqCombo(serviceReqSDO, isSaveAndPrint, printTH, isSaveAndShow, isSign, isPrintDocumentSigned, true, item.TDL_PATIENT_NAME, item.TREATMENT_CODE);
									this.actionType = actionTmp;
								}
							}
							if (ListMessError != null && ListMessError.Count > 0)
							{
								string mess = "Các bệnh nhân sau chỉ định thất bại. \r\n";
								mess += string.Join("\r\n", ListMessError);
								DevExpress.XtraEditors.XtraMessageBox.Show(mess, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
								this.ChangeLockButtonWhileProcess(true);
								return;
							}
							else
							{

								MessageManager.Show(this, new CommonParam(), true);
								this.actionType = GlobalVariables.ActionEdit;
							}

							if (isSaveAndPrint)
							{
								long isClosedForm = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY_HIS_DESKTOP_ASSIGN_SERVICE_CLOSED_FORM_AFTER_PRINT);
								if (isClosedForm == 1)
								{
									this.Dispose();
									this.Close();
								}
							}
							this.ChangeLockButtonWhileProcess(true);
						}

					}
				}

				if (!assignMulti)
				{
					switch (type)
					{
						case TypeButton.SAVE:
							LogTheadInSessionInfo(() => ProcessSaveData(chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked), "SaveAssignServiceDefault");
							break;
						case TypeButton.SAVE_AND_PRINT:
							LogTheadInSessionInfo(() => ProcessSaveData(true, false, false), "SaveAndPrintAssignServiceDefault");
							break;
						case TypeButton.EDIT:
							LogTheadInSessionInfo(() => ProcessSaveData(chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked), "EditAssignServiceDefault");
							break;
						default:
							break;
					}
				}
			}
			catch (Exception ex)
			{
				this.ChangeLockButtonWhileProcess(true);
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnSaveAndPrint_Click(object sender, EventArgs e)
		{
			try
			{
				SaveWithGridpatientSelect(TypeButton.SAVE_AND_PRINT, true, false, false);
                //if (LblBtnPrint.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && BtnPrint.Enabled)
                //{
                //    PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, null, currentModule != null ? currentModule.RoomId : 0);
                //    InPhieuYeuCauDichVu(true);
                //}
            }
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ChangeLockButtonWhileProcess(bool isLock)
		{
			try
			{
				if (this.actionType == GlobalVariables.ActionEdit)
					return;

				this.btnSave.Enabled = isCheckAssignServiceSimultaneityOption ? false : isLock;
				this.btnSaveAndPrint.Enabled = isCheckAssignServiceSimultaneityOption ? false : isLock;
				this.btnCreateServiceGroup.Enabled = isLock;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void barbtnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				this.Close();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void barbtnSaveShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (this.btnSave.Enabled)
					this.btnSave_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void barbtnSaveAndPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (this.btnSaveAndPrint.Enabled)
					this.btnSaveAndPrint_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void barbtnPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (!this.btnSave.Enabled)
				{
					DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037, false, null);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnNew_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				this.isCheckAssignServiceSimultaneityOption = false;
				this.SetDefaultData();
				this.LoadIcdDefault();
				this.DisablecheckEmergencyPriorityByConfig();
				this.treeService.UncheckAll();
				this.isPrinted = false;

				foreach (var item in this.ServiceIsleafADOs)
				{
					item.AssignNumOrder = null;
					item.AMOUNT = 1;
					item.IsChecked = false;
					item.ShareCount = null;
					item.PATIENT_TYPE_ID = 0;
					item.PATIENT_TYPE_CODE = "";
					item.PATIENT_TYPE_NAME = "";
					item.PRICE = 0;
					item.TDL_EXECUTE_ROOM_ID = 0;
					item.IsExpend = false;
					item.IsOutKtcFee = false;
					item.IsKHBHYT = false;
					item.InstructionNote = "";
					item.SERVICE_GROUP_ID_SELECTEDs = null;
					item.SERVICE_CONDITION_ID = null;
					item.SERVICE_CONDITION_NAME = "";
					item.AssignPackagePriceEdit = null;
					item.AssignSurgPriceEdit = null;
					item.IsNoDifference = false;
					item.ErrorMessageAmount = "";
					item.ErrorMessageIsAssignDay = "";
					item.ErrorMessagePatientTypeId = "";
					item.ErrorTypeAmount = ErrorType.None;
					item.ErrorTypeIsAssignDay = ErrorType.None;
					item.ErrorTypePatientTypeId = ErrorType.None;
					item.PRIMARY_PATIENT_TYPE_ID = null;
					item.IsNotChangePrimaryPaty = false;
					item.PackagePriceId = null;
					item.SERVICE_CONDITION_ID = null;
					item.SERVICE_CONDITION_NAME = null;

					item.OTHER_PAY_SOURCE_ID = null;
					item.OTHER_PAY_SOURCE_CODE = null;
					item.OTHER_PAY_SOURCE_NAME = null;
					item.BedFinishTime = null;
					item.BedId = null;
					item.BedStartTime = null;
					item.TEST_SAMPLE_TYPE_ID = 0;
					item.TEST_SAMPLE_TYPE_CODE = null;
					item.TEST_SAMPLE_TYPE_NAME = null;
					item.SereServEkipADO = null;
				}

				this.isNotHandlerWhileChangeToggetSwith = true;
				if ((this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true")
					this.toggleSwitchDataChecked.EditValue = false;
				this.isNotHandlerWhileChangeToggetSwith = false;

				this.gridControlServiceProcess.DataSource = null;
				if (!HisConfigCFG.IsNotAutoLoadServiceOpenAssignService)
				{
					this.gridControlServiceProcess.DataSource = this.ServiceIsleafADOs;
					this.gridControlServiceProcess.RefreshDataSource();
				} 

				this.gridViewServiceProcess.ClearColumnsFilter();
				this.EnableCboTracking();
				//this.CheckOverTotalPatientPrice();
				this.LoadTotalSereServByHeinWithTreatment(this.treatmentId);
				this.RefeshSereServInTreatmentData();
				this.SetEnableButtonControl(this.actionType);
				this.CheckAssignServiceSimultaneityOption();
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				WaitingManager.Hide();
			}
		}

		private void barbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				this.btnNew_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnShowDetail_Click(object sender, EventArgs e)
		{
			try
			{
				frmDetail frmDetail = new frmDetail(this.serviceReqComboResultSDO, this.currentHisPatientTypeAlter, currentHisTreatment, this.currentModule);
				frmDetail.ShowDialog();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnSereservInTreatmentPreview_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SereservInTreatment").FirstOrDefault();
				if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SereservInTreatment'");
				if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SereservInTreatment' is not plugins");
				HIS.Desktop.ADO.SereservInTreatmentADO sereservInTreatmentADO = new HIS.Desktop.ADO.SereservInTreatmentADO(this.treatmentId, intructionTimeSelecteds.First(), serviceReqParentId ?? 0);
				List<object> listArgs = new List<object>();
				listArgs.Add(sereservInTreatmentADO);
				listArgs.Add(this.currentModule);
				var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
				if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

				WaitingManager.Hide();
				((Form)extenceInstance).Show();
			}
			catch (NullReferenceException ex)
			{
				WaitingManager.Hide();
				MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnServiceReqList_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
				if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
				if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ServiceReqList' is not plugins");

				MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
				treatment.ID = this.treatmentId;
				List<object> listArgs = new List<object>();
				listArgs.Add(treatment);
				var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
				if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

				WaitingManager.Hide();
				((Form)extenceInstance).Show();
			}
			catch (NullReferenceException ex)
			{
				WaitingManager.Hide();
				MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnTomLuocVienPhi_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();

				Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
				if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
				if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.AggrHospitalFees' is not plugins");

				List<object> listArgs = new List<object>();
				listArgs.Add(this.treatmentId);
				var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
				if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

				WaitingManager.Hide();
				((Form)extenceInstance).Show();
			}
			catch (NullReferenceException ex)
			{
				WaitingManager.Hide();
				MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnPrintPhieuHuongDanBN_Click(object sender, EventArgs e)
		{
			try
			{
				var PrintServiceReqProcessor = new HIS.Desktop.Plugins.Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor(this.serviceReqComboResultSDO.ServiceReqs, currentModule != null ? this.currentModule.RoomId : 0);

				LogTheadInSessionInfo(() => PrintServiceReqProcessor.Print("Mps000276", false), "btnPrintPhieuHuongDanBN_Click");
				//PrintServiceReqProcessor.Print("Mps000276", false);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnCreateBill_Click(object sender, EventArgs e)
		{
			try
			{
				// get treatment
				CommonParam param = new CommonParam();
				V_HIS_TREATMENT_FEE currentTreatment = null;
				MOS.Filter.HisTreatmentFeeViewFilter treatmentViewFilter = new HisTreatmentFeeViewFilter();
				treatmentViewFilter.ID = this.treatmentId;
				var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, treatmentViewFilter, param);
				if (treatments != null && treatments.Count > 0)
				{
					currentTreatment = treatments.FirstOrDefault();
				}

				// get sereServs
				//- Lấy các dịch vụ đã chỉ định mà chưa thanh toán (ko thuộc sere_SErv_bill).
				//- Áp dụng cho các dịch vụ viện phí (Không load các dịch vụ có đối tượng thanh toán là BHYT)
				//- Lấy các dịch vụ có creator là người đăng nhập.
				//- Mở form thanh toán như của thu ngân.
				//- Phòng thanh toán là phòng thu ngân mà người dùng đang mở cùng với phòng xử lý (giải pháp như tiếp đón).
				MOS.Filter.HisSereServView5Filter sereServViewFilter = new HisSereServView5Filter();
				sereServViewFilter.TDL_TREATMENT_ID = this.treatmentId;
				var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumer.ApiConsumers.MosConsumer, sereServViewFilter, param);
				// get sereServBills
				if (sereServs == null || sereServs.Count == 0)
				{
					return;
				}
				MOS.Filter.HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
				sereServBillFilter.SERE_SERV_IDs = sereServs.Select(p => p.ID).Distinct().ToList();
				var sereServBills = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServBillFilter, param);
				if (sereServBills != null && sereServBills.Count > 0)
				{
					sereServs = sereServs.Where(o => !sereServBills.Select(p => p.SERE_SERV_ID).Distinct().ToList().Contains(o.ID)).ToList();
				}
				// lọc các dịch vụ viện phí, các dịch vụ có creator là người đăng nhập
				string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
				sereServs = sereServs.Where(o => o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT && o.CREATOR == loginName).ToList();

				if (!btnCreateBill.Enabled || currentTreatment == null)
					return;
				if (cboCashierRoom.EditValue == null)
				{
					MessageBox.Show(ResourceMessage.ChuaChonPhongThuNgan, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
					return;
				}

				var cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboCashierRoom.EditValue.ToString()));
				Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBill").FirstOrDefault();
				if (sereServs == null || sereServs.Count == 0)
				{
					MessageBox.Show(ResourceMessage.HSDTKhongCoHoacDaThanhToanDichVu, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
					return;
				}
				if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionBill'");
				if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
				{
					moduleData.RoomId = cashierRoom.ROOM_ID;
					moduleData.RoomTypeId = cashierRoom.ROOM_TYPE_ID;
					List<object> listArgs = new List<object>();
					listArgs.Add(currentTreatment);
					listArgs.Add(moduleData);
					listArgs.Add(sereServs);
					var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, cashierRoom.ROOM_ID, cashierRoom.ROOM_TYPE_ID), listArgs);
					if (extenceInstance == null)
					{
						throw new ArgumentNullException("extenceInstance is null");
					}

					((Form)extenceInstance).ShowDialog();
					//FillDataToControlBySelectTreatment(true);
					//txtFindTreatmentCode.Focus();
					//txtFindTreatmentCode.SelectAll();
				}
			}
			catch (NullReferenceException ex)
			{
				WaitingManager.Hide();
				MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnBoSungPhacDo_Click(object sender, EventArgs e)
		{

			try
			{
				//Lay danh sach icd
				List<HIS_ICD> icdCodeArr = GetIcdCodeListFromUcIcd();

				if (icdCodeArr == null || icdCodeArr.Count == 0)
				{
					MessageBox.Show(ResourceMessage.KhongTimThayThongTinICD, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
									MessageBoxButtons.OK, MessageBoxIcon.Question);
					return;
				}
				List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
				if (serviceCheckeds__Send == null || serviceCheckeds__Send.Count == 0)
				{
					MessageBox.Show(ResourceMessage.VuiLongChonDichVu, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
									MessageBoxButtons.OK, MessageBoxIcon.Question);
					return;
				}
				List<long> serviceIds = serviceCheckeds__Send.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).ToList();
				CommonParam param = new CommonParam();
				HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
				icdServiceFilter.SERVICE_IDs = serviceIds;
				icdServiceFilter.ICD_CODE__EXACTs = icdCodeArr.Select(o => o.ICD_CODE).Distinct().ToList();
				List<HIS_ICD_SERVICE> icdServices = new BackendAdapter(param)
				.Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);
				List<long> icdServiceIds = icdServices.Select(o => o.SERVICE_ID ?? 0).Distinct().ToList();
				List<long> serviceNotConfigIds = new List<long>();
				foreach (var item in serviceIds)
				{
					if (!icdServiceIds.Contains(item))
					{
						serviceNotConfigIds.Add(item);
					}
				}

				if (serviceNotConfigIds == null || serviceNotConfigIds.Count == 0)
				{
					MessageBox.Show(ResourceMessage.KhongTimThayDVChuaDuocCauHinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
									MessageBoxButtons.OK, MessageBoxIcon.Question);
					return;
				}

				List<HIS_ICD> icds = this.currentIcds.Where(o => icdCodeArr.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).Distinct().ToList();
				if (icds == null || icds.Count == 0)
				{
					LogSystem.Debug("Khong tim thay ICD");
					return;
				}

				//if (icds.Count == 1)
				//{
				//    icdChoose = icds[0];
				//}
				//else
				//{
				//    //Mo form chon icd
				//    icdChoose = new HIS_ICD();
				//    frmChooseICD frm = new frmChooseICD(icds, refeshChooseIcd);
				//    frm.ShowDialog();
				//}

				//if (icdChoose == null || (icdChoose != null && icdChoose.ID == 0))
				//    return;

				List<object> listObj = new List<object>();
				listObj.Add(icdCodeArr);
				listObj.Add(serviceNotConfigIds);
				HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceIcd", currentModule.RoomId, currentModule.RoomTypeId, listObj);

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void BarSavePrint_PrintTH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			//try
			//{
			//    if (LciBtnSavePrintNPrintTH.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && this.BtnSavePrintNPrintTH.Enabled)
			//        BtnSavePrintNPrintTH_Click(null, null);
			//}
			//catch (Exception ex)
			//{
			//    Inventec.Common.Logging.LogSystem.Error(ex);
			//}
		}

		private void BtnSavePrintNPrintTH_Click(object sender, EventArgs e)
		{
			try
			{
				this.ProcessSaveData(true, true, true);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void BtnSaveNShow_Click(object sender, EventArgs e)
		{
			try
			{
				this.ProcessSaveData(true, false, true);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void barBtnSaveNShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				//if (LciBtnSaveNShow.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && this.BtnSaveNShow.Enabled)
				//{
				//    BtnSaveNShow_Click(null, null);
				//}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void BtnPrint_Click(object sender, EventArgs e)
		{
			try
			{
				if (LblBtnPrint.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && BtnPrint.Enabled)
				{
					PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, null, currentModule != null ? currentModule.RoomId : 0);
					
					InPhieuYeuCauDichVu(true);
					
					//PrintServiceReqProcessor.Print("Mps000340", false);
				}
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnERMSign_Click(object sender, EventArgs e)
		{
			try
			{
				InPhieuYeuCauDichVuVaERM();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		#endregion

		#region UCSecondaryIcd
		internal bool ShowPopupIcdChoose()
		{
			try
			{
				WaitingManager.Show();
				frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, BackendDataWorker.Get<V_HIS_ICD>().Where(o => o.IS_TRADITIONAL != 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), currentTreatment);
				WaitingManager.Hide();
				FormSecondaryIcd.ShowDialog();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				return false;
			}
			return true;
		}

		private void ReloadIcdSubContainerByCodeChanged()
		{
			try
			{
				Inventec.Common.Logging.LogSystem.Debug("ReloadIcdSubContainerByCodeChanged.1");
				string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
				this.icdSubcodeAdoChecks = (from m in this.currentIcds.Where(o => o.IS_TRADITIONAL != 1).ToList() select new ADO.IcdADO(m, codes)).ToList();
				customGridControlSubIcdName.DataSource = null;
				customGridControlSubIcdName.DataSource = this.icdSubcodeAdoChecks;
				Inventec.Common.Logging.LogSystem.Debug("ReloadIcdSubContainerByCodeChanged.2");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtIcdSubCode_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (!string.IsNullOrEmpty(txtIcdSubCode.Text))
					{
						if (!ProccessorByIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
						{
							e.Handled = true;
						}
						else
						{
							ReloadIcdSubContainerByCodeChanged();
							txtIcdText.Focus();
							txtIcdText.SelectionStart = txtIcdText.Text.Length;
							txtIcdText.SelectAll();
						}
					}
					else
					{
						
						txtIcdText.Focus();
						
					}
					if (HisConfigCFG.IcdServiceHasCheck != "1" && HisConfigCFG.IcdServiceHasCheck != "2" && HisConfigCFG.IcdServiceHasCheck != "3" && HisConfigCFG.IcdServiceHasCheck != "4" && HisConfigCFG.IcdServiceHasCheck != "5")
						return;
					List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
					MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
					icdServiceFilter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
					icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
					if (icdServicePhacDos != null && icdServicePhacDos.Count > 0)
						ProcessChoiceIcdPhacDo(icdServicePhacDos);
					else
					{
						this.ResetDefaultGridData();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtIcdSubCode_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.F1)
				{
					WaitingManager.Show();
					frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, BackendDataWorker.Get<V_HIS_ICD>().Where(o => o.IS_TRADITIONAL != 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), currentTreatment);
					WaitingManager.Hide();
					FormSecondaryIcd.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					//if (!isShow)
					//{
					//    this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO;
					//    if (this.customGridViewSubIcdName.RowCount == 1 && this.subIcdPopupSelect != null && !this.subIcdPopupSelect.IsChecked)
					//    {
					//        this.subIcdPopupSelect.IsChecked = true;
					//        this.customGridControlSubIcdName.RefreshDataSource();
					//    }
					isShowContainerMediMaty = false;
					isShowContainerMediMatyForChoose = true;
					popupControlContainerSubIcdName.HidePopup();

					//    SetCheckedSubIcdsToControl();

					//    txtIcdText.Focus();
					//    txtIcdText.SelectionStart = txtIcdText.Text.Length;
					//}
					//else
					//{
					//    isShowContainerMediMaty = false;
					//    isShowContainerMediMatyForChoose = true;
					//    popupControlContainerSubIcdName.HidePopup();
					//}
					txtIcdCodeCause.Focus();
					//UcIcdNextForcusOut();
				}
				else if (e.KeyCode == Keys.Down)
				{
					int rowHandlerNext = 0;
					//int countInGridRows = customGridViewSubIcdName.RowCount;
					//if (countInGridRows > 1)
					//{
					//    rowHandlerNext = 1;
					//}

					ShowPopupContainerIcsSub();
					customGridViewSubIcdName.Focus();
					customGridViewSubIcdName.FocusedRowHandle = rowHandlerNext;
				}
				else if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.ShiftKey)
				{
					customGridViewSubIcdName.ActiveFilter.Clear();
					ShowPopupContainerIcsSub();
					txtIcdText.Focus();
					txtIcdText.SelectionStart = txtIcdText.Text.Length;
					e.Handled = true;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.F1)
				{

					WaitingManager.Show();
					frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, BackendDataWorker.Get<V_HIS_ICD>().Where(o => o.IS_TRADITIONAL != 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), currentTreatment);
					WaitingManager.Hide();
					FormSecondaryIcd.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void stringIcds(string icdCode, string icdName)
		{
			try
			{
				this.isNotProcessWhileChangedTextSubIcd = true;
				txtIcdSubCode.Text = icdCode;
				txtIcdText.Text = icdName;
				ReloadIcdSubContainerByCodeChanged();
				this.isNotProcessWhileChangedTextSubIcd = false;
				if (HisConfigCFG.IcdServiceHasCheck != "1" && HisConfigCFG.IcdServiceHasCheck != "2" && HisConfigCFG.IcdServiceHasCheck != "3" && HisConfigCFG.IcdServiceHasCheck != "4" && HisConfigCFG.IcdServiceHasCheck != "5")
					return;
				List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
				MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
				icdServiceFilter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
				icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
				if (icdServicePhacDos != null && icdServicePhacDos.Count > 0)
					ProcessChoiceIcdPhacDo(icdServicePhacDos);
				else
				{
					this.ResetDefaultGridData();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
		{
			try
			{
				string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
				txtIcdText.Text = ProcessIcdNameChanged(icdName__Olds, icdNames);
				if (icdNames.Equals(IcdUtil.seperator))
				{
					txtIcdText.Text = "";
				}
				if (icdCodes.Equals(IcdUtil.seperator))
				{
					txtIcdSubCode.Text = "";
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private string ProcessIcdNameChanged(string oldIcdNames, string newIcdNames)
		{
			//Thuat toan xu ly khi thay doi lai danh sach icd da chon
			//1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
			//2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
			//   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
			//   -> icd do da sua doi
			//   -> cong vao chuoi ket qua
			string result = "";
			try
			{
				result = newIcdNames;

				if (!String.IsNullOrEmpty(oldIcdNames))
				{
					var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
					if (arrNames != null && arrNames.Length > 0)
					{
						foreach (var item in arrNames)
						{
							if (!String.IsNullOrEmpty(item)
								&& !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
								)
							{
								var checkInList = this.currentIcds.Where(o => o.IS_TRADITIONAL != 1 &&
									IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
								if (checkInList == null || checkInList.ID == 0)
								{
									result += item + IcdUtil.seperator;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return result;
		}

		private bool CheckIcdWrongCode(ref string strIcdNames, ref string strWrongIcdCodes)
		{
			bool valid = true;
			try
			{
				if (!String.IsNullOrEmpty(this.txtIcdSubCode.Text))
				{
					strWrongIcdCodes = "";
					List<string> arrWrongCodes = new List<string>();
					List<string> lstIcdCodes = new List<string>();
					List<string> lstIcdSubName = new List<string>();
					List<string> arrIcdExtraCodes = this.txtIcdSubCode.Text.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !string.IsNullOrEmpty(o)).Select(o => o.Trim()).Distinct().Where(o => !string.IsNullOrEmpty(o)).ToList();
					if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
					{
						foreach (var itemCode in arrIcdExtraCodes)
						{
							var icdByCode = this.currentIcds.Where(o => o.IS_TRADITIONAL != 1).ToList().FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
							if (icdByCode != null && icdByCode.ID > 0)
							{
								string messErr = null;
								if (!checkIcdManager.ProcessCheckIcd(null, icdByCode.ICD_CODE, ref messErr,false))
								{
									XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
									continue;
								}
								strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
								lstIcdCodes.Add(icdByCode.ICD_CODE);
								lstIcdSubName.Add(icdByCode.ICD_NAME);
							}
							else
							{
								arrWrongCodes.Add(itemCode);
								strWrongIcdCodes += (IcdUtil.seperator + itemCode);
							}
						}
						strIcdNames += IcdUtil.seperator;
						isNotProcessWhileChangedTextSubIcd = true;
						if (lstIcdCodes != null && lstIcdCodes.Count > 0)
						{
							this.txtIcdSubCode.Text = String.Join(";", lstIcdCodes);
							this.txtIcdText.Text = String.Join(";", lstIcdSubName);
						}
						else
						{
							this.txtIcdSubCode.Text = null;
							this.txtIcdText.Text = null;
						}
						if (!String.IsNullOrEmpty(strWrongIcdCodes))
						{
							valid = false;
							this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, this.txtIcdText.Text);
							XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, string.Join(",", arrWrongCodes)), "Thông báo", MessageBoxButtons.OK);
							ShowPopupIcdChoose();


							//MessageManager.Show(String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
							//int startPositionWarm = 0;
							//int lenghtPositionWarm = this.txtIcdSubCode.Text.Length - 1;
							//if (arrWrongCodes != null && arrWrongCodes.Count > 0)
							//{
							//    startPositionWarm = this.txtIcdSubCode.Text.IndexOf(arrWrongCodes[0]);
							//    lenghtPositionWarm = arrWrongCodes[0].Length;
							//}
							//this.txtIcdSubCode.Focus();
							//this.txtIcdSubCode.Select(startPositionWarm, lenghtPositionWarm);
							//valid = false;
						}
						isNotProcessWhileChangedTextSubIcd = false;
					}
				}
				else
				{
					txtIcdText.Text = this.txtIcdSubCode.Text = null;
					txtIcdText.Focus();
				}
			}
			catch (Exception ex)
			{
				valid = false;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return valid;
		}

		private bool ProccessorByIcdCode(string currentValue)
		{
			bool valid = true;
			try
			{
				string strIcdNames = "";
				string strWrongIcdCodes = "";
				if (!CheckIcdWrongCode(ref strIcdNames, ref strWrongIcdCodes))
				{
					valid = false;
					Inventec.Common.Logging.LogSystem.Debug("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
				}
				//this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, strIcdNames);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return valid;
		}


		private void txtIcdText_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.isNotProcessWhileChangedTextSubIcd)
				{
					Inventec.Common.Logging.LogSystem.Debug("txtIcdText_TextChanged____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isNotProcessWhileChangedTextSubIcd), isNotProcessWhileChangedTextSubIcd));
					return;
				}
				if (!String.IsNullOrEmpty(txtIcdText.Text))
				{
					string strIcdSubText = "";

					txtIcdText.Refresh();
					if (txtIcdText.Text.LastIndexOf(";") > -1)
					{
						strIcdSubText = txtIcdText.Text.Substring(txtIcdText.Text.LastIndexOf(";")).Replace(";", "");
					}
					else
						strIcdSubText = txtIcdText.Text;
					if (isShowContainerMediMatyForChoose)
					{
						customGridViewSubIcdName.ActiveFilter.Clear();
					}
					else
					{
						if (!isShowContainerMediMaty)
						{
							isShowContainerMediMaty = true;
						}

						//Filter data
						customGridViewSubIcdName.ActiveFilterString = String.Format("[ICD_NAME_UNSIGN] Like '%{0}%'", Inventec.Common.String.Convert.UnSignVNese(strIcdSubText).ToLower());
						customGridViewSubIcdName.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
						customGridViewSubIcdName.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
						customGridViewSubIcdName.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
						customGridViewSubIcdName.FocusedRowHandle = 0;
						customGridViewSubIcdName.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
						customGridViewSubIcdName.OptionsFind.HighlightFindResults = true;

						if (isShow)
						{
							ShowPopupContainerIcsSub();
							isShow = false;
						}

						txtIcdText.Focus();
					}
					isShowContainerMediMatyForChoose = false;
				}
				else
				{
					customGridViewSubIcdName.ActiveFilter.Clear();
					this.subIcdPopupSelect = null;
					if (!isShowContainerMediMaty)
					{
						popupControlContainerSubIcdName.HidePopup();
					}
					else
					{
						customGridViewSubIcdName.FocusedRowHandle = 0;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtIcdText_KeyPress(object sender, KeyPressEventArgs e)
		{
			try
			{
				//if (e.KeyChar == ((char)System.Windows.Forms.Keys.ControlKey | (char)System.Windows.Forms.Keys.A))
				//{
				//    txtIcdText.Focus();
				//    txtIcdText.SelectAll();
				//}
				//else if (e.KeyChar == ((char)System.Windows.Forms.Keys.Delete) || e.KeyChar == ((char)System.Windows.Forms.Keys.Back))
				//{
				//    this.isNotProcessWhileChangedTextSubIcd = true;
				//}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ShowPopupContainerIcsSub()
		{
			try
			{
				popupControlContainerSubIcdName.Width = 600;
				popupControlContainerSubIcdName.Height = 250;

				Rectangle buttonBounds = new Rectangle(panelControlSubIcd.Bounds.X, panelControlSubIcd.Bounds.Y, panelControlSubIcd.Bounds.Width, panelControlSubIcd.Bounds.Height);
				popupControlContainerSubIcdName.ShowPopup(new Point(buttonBounds.X + 300, buttonBounds.Bottom + 22));
				Inventec.Common.Logging.LogSystem.Debug("buttonBounds.X + 300=" + buttonBounds.X + 300 + ", buttonBounds.Bottom + 22=" + (buttonBounds.Bottom + 22));
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SetCheckedSubIcdsToControl()
		{
			try
			{
				Inventec.Common.Logging.LogSystem.Debug("SetCheckedSubIcdsToControl.1");
				this.isNotProcessWhileChangedTextSubIcd = true;
				string strIcdSubText = "";
				if (txtIcdText.Text.LastIndexOf(";") > -1)
				{
					strIcdSubText = txtIcdText.Text.Substring(txtIcdText.Text.LastIndexOf(";")).Replace(";", "");
				}
				else
					strIcdSubText = txtIcdText.Text;

				string icdNames = null;// IcdUtil.seperator;
				string icdCodes = null;// IcdUtil.seperator;
				string icdName__Olds = txtIcdText.Text;
				var checkList = this.icdSubcodeAdoChecks.Where(o => o.IsChecked == true).ToList();
				int count = 0;
				foreach (var item in checkList)
				{
					count++;
					string messErr = null;
					if (!checkIcdManager.ProcessCheckIcd(null, item.ICD_CODE, ref messErr,false))
					{
						XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
						item.IsChecked = false;
						continue;
					}
					if (count == checkList.Count)
					{
						icdCodes += item.ICD_CODE;
						icdNames += item.ICD_NAME;
					}
					else
					{
						icdCodes += item.ICD_CODE + IcdUtil.seperator;
						icdNames += item.ICD_NAME + IcdUtil.seperator;
					}
				}
				string newtxtIcdText = ProcessIcdNameChanged(icdName__Olds, icdNames);

				txtIcdText.Text = newtxtIcdText;
				txtIcdSubCode.Text = icdCodes;
				//if (!String.IsNullOrEmpty(strIcdSubText))
				//{
				//    txtIcdText.Text = newtxtIcdText.Substring(0, newtxtIcdText.LastIndexOf(IcdUtil.seperator + strIcdSubText + IcdUtil.seperator) + 1);
				//}
				//if (icdNames.Equals(IcdUtil.seperator))
				//{
				//    txtIcdText.Text = "";
				//}
				//if (icdCodes.Equals(IcdUtil.seperator))
				//{
				//    txtIcdSubCode.Text = "";
				//}
				this.isNotProcessWhileChangedTextSubIcd = false;
				Inventec.Common.Logging.LogSystem.Debug("SetCheckedSubIcdsToControl.2");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void customGridViewSubIcdName_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.AssignService.ADO.IcdADO;
					if (this.subIcdPopupSelect != null)
					{
						isShowContainerMediMaty = false;
						isShowContainerMediMatyForChoose = true;

						this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
						this.customGridControlSubIcdName.RefreshDataSource();

						SetCheckedSubIcdsToControl();
						popupControlContainerSubIcdName.HidePopup();

						txtIcdText.Focus();
						txtIcdText.SelectionStart = txtIcdText.Text.Length;
					}
				}
				//if (e.KeyCode == Keys.Space)
				//{
				//    this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO;
				//    if (this.subIcdPopupSelect != null)
				//    {
				//        int currentFocusRow = customGridViewSubIcdName.FocusedRowHandle;

				//        this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
				//        this.customGridControlSubIcdName.RefreshDataSource();
				//        this.customGridViewSubIcdName.FocusedRowHandle = currentFocusRow;
				//        SetCheckedSubIcdsToControl();
				//    }
				//}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void customGridControlSubIcdName_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.AssignService.ADO.IcdADO;
				if (this.subIcdPopupSelect != null)
				{
					isShowContainerMediMaty = false;
					isShowContainerMediMatyForChoose = true;

					this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
					this.customGridControlSubIcdName.RefreshDataSource();

					SetCheckedSubIcdsToControl();
					popupControlContainerSubIcdName.HidePopup();


					txtIcdText.Focus();
					txtIcdText.SelectionStart = txtIcdText.Text.Length;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void customGridViewSubIcdName_CellValueChanged(object sender, CellValueChangedEventArgs e)
		{
			try
			{
				if (e.Column.FieldName == "IsChecked")
				{
					SetCheckedSubIcdsToControl();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void customGridViewSubIcdName_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
				{
					Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.1");
					GridView view = sender as GridView;
					GridHitInfo hi = view.CalcHitInfo(e.Location);
					if (hi.InRowCell)
					{
						Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.2");
						if (hi.Column.FieldName == "IsChecked" && hi.Column.RealColumnEdit != null && hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
						{
							Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.3");
							view.FocusedRowHandle = hi.RowHandle;
							view.FocusedColumn = hi.Column;
							view.ShowEditor();
							CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
							DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
							Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
							GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
							Rectangle gridGlyphRect =
								new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
								 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
								 glyphRect.Width,
								 glyphRect.Height);
							if (!gridGlyphRect.Contains(e.Location))
							{
								view.CloseEditor();
								if (!view.IsCellSelected(hi.RowHandle, hi.Column))
								{
									view.SelectCell(hi.RowHandle, hi.Column);
								}
								else
								{
									view.UnselectCell(hi.RowHandle, hi.Column);
								}
							}
							else
							{
								checkEdit.Checked = !checkEdit.Checked;
								view.CloseEditor();
							}
							(e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
							Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.4");
						}
					}
				}
				Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.5");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void popupControlContainerSubIcdName_CloseUp(object sender, EventArgs e)
		{
			try
			{
				isShow = true;
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}
		#endregion

		#region UcIcd
		private void txtIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					LoadIcdCombo(txtIcdCode.Text.ToUpper());
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadIcdCombo(string searchCode)
		{
			try
			{
				bool showCbo = true;
				if (!String.IsNullOrEmpty(searchCode))
				{
					var listData = currentIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
					var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
					if (result != null && result.Count > 0)
					{
						showCbo = false;
						txtIcdCode.Text = result.First().ICD_CODE;
						txtIcdMainText.Text = result.First().ICD_NAME;
						cboIcds.EditValue = listData.First().ID;
						chkEditIcd.Checked = (chkEditIcd.Enabled ? this.isAutoCheckIcd : false);
						string messErr = null;
						if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), txtIcdSubCode.Text.Trim(), ref messErr,false))
						{
							XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
							if (Desktop.Plugins.Library.CheckIcd.CheckIcdManager.IcdCodeError.Equals(txtIcdCode.Text))
							{
								txtIcdCode.Text = txtIcdMainText.Text = null;
								cboIcds.EditValue = null;
							}
							return;
						}
						if (chkEditIcd.Checked)
						{
							txtIcdMainText.Focus();
							txtIcdMainText.SelectAll();
						}
						else
						{
							cboIcds.Focus();
							cboIcds.SelectAll();
						}
					}
				}

				if (showCbo)
				{
					cboIcds.Focus();
					cboIcds.ShowPopup();
				}
				else
				{
					if (HisConfigCFG.IcdServiceHasCheck != "1" && HisConfigCFG.IcdServiceHasCheck != "2" && HisConfigCFG.IcdServiceHasCheck != "3" && HisConfigCFG.IcdServiceHasCheck != "4" && HisConfigCFG.IcdServiceHasCheck != "5")
						return;
					List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
					MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
					icdServiceFilter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
					icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
					if (icdServicePhacDos != null && icdServicePhacDos.Count > 0)
						ProcessChoiceIcdPhacDo(icdServicePhacDos);
					else
					{
						this.ResetDefaultGridData();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtIcdMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
				{
					chkEditIcd.Focus();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboIcds_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
		{
			try
			{
				if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
				{
					if (cboIcds.EditValue != null)
						this.ChangecboChanDoanTD();
					else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdName))
						this.ChangecboChanDoanTD_V2_GanICDNAME(this._TextIcdName);
					else
						SendKeys.Send("{TAB}");
					if (HisConfigCFG.IcdServiceHasCheck != "1" && HisConfigCFG.IcdServiceHasCheck != "2" && HisConfigCFG.IcdServiceHasCheck != "3" && HisConfigCFG.IcdServiceHasCheck != "4" && HisConfigCFG.IcdServiceHasCheck != "5")
						return;
					List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
					MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
					icdServiceFilter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
					icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
					if (icdServicePhacDos != null && icdServicePhacDos.Count > 0)
						ProcessChoiceIcdPhacDo(icdServicePhacDos);
					else
					{
						this.ResetDefaultGridData();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ChangecboChanDoanTD()
		{
			try
			{
				txtIcdCode.ErrorText = "";
				dxValidationProviderControl.RemoveControlError(txtIcdCode);

				cboIcds.Properties.Buttons[1].Visible = true;
				if (currentIcds != null)
				{
					Inventec.Common.Logging.LogSystem.Debug("currentIcds count " + currentIcds.Count);
				}

				MOS.EFMODEL.DataModels.HIS_ICD icd = currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
				if (icd != null)
				{
					txtIcdCode.Text = icd.ICD_CODE;
					txtIcdMainText.Text = icd.ICD_NAME;
					chkEditIcd.Checked = (chkEditIcd.Enabled ? this.isAutoCheckIcd : false);
					string messErr = null;
					if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), txtIcdSubCode.Text.Trim(), ref messErr,false))
					{
						XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
						if (Desktop.Plugins.Library.CheckIcd.CheckIcdManager.IcdCodeError.Equals(txtIcdCode.Text))
						{
							txtIcdCode.Text = txtIcdMainText.Text = null;
							cboIcds.EditValue = null;
						}
						return;
					}
					if (chkEditIcd.Checked)
					{
						this.NextForcusSubIcd();
					}
					else if (chkEditIcd.Enabled)
					{
						chkEditIcd.Focus();
					}
					else
					{
						this.NextForcusSubIcd();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ChangecboChanDoanTD_V2_GanICDNAME(string text)
		{
			try
			{
				if (string.IsNullOrEmpty(text))
					return;
				if (HisConfigCFG.AutoCheckIcd != "2")
				{
					this.chkEditIcd.Enabled = true;
					this.chkEditIcd.Checked = true;
				}
				this.txtIcdMainText.Text = text;
				this.txtIcdMainText.Focus();
				this.txtIcdMainText.SelectAll();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboIcds_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.Control & e.KeyCode == Keys.A)
				{
					cboIcds.ClosePopup();
					cboIcds.SelectAll();
				}
				else if (e.KeyCode == Keys.Enter)
				{
					cboIcds.ClosePopup();
					if (cboIcds.EditValue != null)
						this.ChangecboChanDoanTD();
				}
				else
					cboIcds.ShowPopup();
				e.Handled = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkEditIcd_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (chkEditIcd.Checked == true)
				{
					cboIcds.Visible = false;
					txtIcdMainText.Visible = true;
					if (this.IsObligatoryTranferMediOrg)
						txtIcdMainText.Text = this._TextIcdName;
					else
						txtIcdMainText.Text = cboIcds.Text;
					txtIcdMainText.Focus();
					txtIcdMainText.SelectAll();
				}
				else if (chkEditIcd.Checked == false)
				{
					txtIcdMainText.Visible = false;
					cboIcds.Visible = true;
					txtIcdMainText.Text = cboIcds.Text;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkEditIcd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (txtIcdMainText.Text != null)
					{
						//this.data.DelegateRefeshIcdMainText(txtIcdMainText.Text);
					}
					if (cboIcds.EditValue != null)
					{
						//var hisIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == (long)cboIcds.EditValue).FirstOrDefault();
						//this.data.DelegateRefeshIcd(hisIcd);
					}
					NextForcusSubIcd();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtIcdCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
		{
			try
			{
				e.ErrorText = Resources.ResourceMessage.IcdKhongDung;
				AutoValidate = AutoValidate.EnableAllowFocusChange;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtIcdCode_Validating(object sender, CancelEventArgs e)
		{
			try
			{
				var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
				if (!String.IsNullOrEmpty(search))
				{
					var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
					var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
					if (result == null || result.Count <= 0)
					{
						e.Cancel = true;
					}
					else
					{
						txtIcdCode.ErrorText = "";
						dxValidationProviderControl.RemoveControlError(txtIcdCode);
						ValidationICD(10, 500, true);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboIcds_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == ButtonPredefines.Delete)
				{
					//if (!cboIcds.Properties.Buttons[1].Visible)
					//    return;
					this._TextIcdName = "";
					cboIcds.EditValue = null;
					txtIcdCode.Text = "";
					txtIcdMainText.Text = "";
					cboIcds.Properties.Buttons[1].Visible = false;
					txtIcdCode.ErrorText = "";
					dxValidationProviderControl.RemoveControlError(txtIcdCode);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboIcds_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (String.IsNullOrEmpty(cboIcds.Text))
				{
					cboIcds.EditValue = null;
					txtIcdMainText.Text = "";
					chkEditIcd.Checked = false;
				}
				else
				{
					this._TextIcdName = cboIcds.Text;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboIcds_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				HIS_ICD icd = null;
				if (this.cboIcds.EditValue != null)
					icd = this.currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboIcds.EditValue.ToString()));

				if (icd != null)
				{
					if (icd != null && icd.IS_REQUIRE_CAUSE == 1)
					{
						this.LoadRequiredCause(true);
					}
					else
						this.LoadRequiredCause(false);
				}
				else
				{
					this.LoadRequiredCause(false);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		#endregion

		#region UcIcdCause
		private void txtIcdCodeCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					LoadIcdComboCause(txtIcdCodeCause.Text.ToUpper());
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadIcdComboCause(string searchCode)
		{
			try
			{
				bool showCbo = true;
				if (!String.IsNullOrEmpty(searchCode))
				{
					var listData = currentIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
					var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
					if (result != null && result.Count > 0)
					{
						showCbo = false;
						txtIcdCodeCause.Text = result.First().ICD_CODE;
						txtIcdMainTextCause.Text = result.First().ICD_NAME;
						cboIcdsCause.EditValue = listData.First().ID;
						chkEditIcdCause.Checked = (chkEditIcdCause.Enabled ? this.isAutoCheckIcd : false);

						if (chkEditIcdCause.Checked)
						{
							txtIcdMainTextCause.Focus();
							txtIcdMainTextCause.SelectAll();
						}
						else
						{
							cboIcdsCause.Focus();
							cboIcdsCause.SelectAll();
						}
					}
				}

				if (showCbo)
				{
					cboIcdsCause.Focus();
					cboIcdsCause.ShowPopup();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtIcdMainTextCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
				{
					chkEditIcdCause.Focus();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboIcdsCause_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
		{
			try
			{
				if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
				{
					if (cboIcdsCause.EditValue != null)
						this.ChangecboChanDoanTDCause();
					else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdNameCause))
						this.ChangecboChanDoanTD_V2_GanICDNAMECause(this._TextIcdNameCause);
					else
						SendKeys.Send("{TAB}");
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ChangecboChanDoanTDCause()
		{
			try
			{
				cboIcdsCause.Properties.Buttons[1].Visible = true;
				MOS.EFMODEL.DataModels.HIS_ICD icd = currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcdsCause.EditValue ?? 0).ToString()));
				if (icd != null)
				{
					txtIcdCodeCause.Text = icd.ICD_CODE;
					txtIcdMainTextCause.Text = icd.ICD_NAME;
					chkEditIcdCause.Checked = (chkEditIcdCause.Enabled ? this.isAutoCheckIcd : false);
					if (chkEditIcdCause.Checked)
					{
						this.NextForcusSubIcdCause();
					}
					else if (chkEditIcdCause.Enabled)
					{
						chkEditIcdCause.Focus();
					}
					else
					{
						this.NextForcusSubIcdCause();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ChangecboChanDoanTD_V2_GanICDNAMECause(string text)
		{
			try
			{
				if (string.IsNullOrEmpty(text))
					return;
				if (HisConfigCFG.AutoCheckIcd != "2")
				{
					this.chkEditIcdCause.Enabled = true;
					this.chkEditIcdCause.Checked = true;
				}
				this.txtIcdMainTextCause.Text = text;
				this.txtIcdMainTextCause.Focus();
				this.txtIcdMainTextCause.SelectAll();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboIcdsCause_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.Control & e.KeyCode == Keys.A)
				{
					cboIcdsCause.ClosePopup();
					cboIcdsCause.SelectAll();
				}
				else if (e.KeyCode == Keys.Enter)
				{
					cboIcdsCause.ClosePopup();
					if (cboIcdsCause.EditValue != null)
						this.ChangecboChanDoanTDCause();
				}
				else
					cboIcdsCause.ShowPopup();
				e.Handled = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkEditIcdCause_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (chkEditIcdCause.Checked == true)
				{
					cboIcdsCause.Visible = false;
					txtIcdMainTextCause.Visible = true;
					if (this.IsObligatoryTranferMediOrg)
						txtIcdMainTextCause.Text = this._TextIcdName;
					else
						txtIcdMainTextCause.Text = cboIcds.Text;
					txtIcdMainTextCause.Focus();
					txtIcdMainTextCause.SelectAll();
				}
				else if (chkEditIcdCause.Checked == false)
				{
					txtIcdMainTextCause.Visible = false;
					cboIcdsCause.Visible = true;
					txtIcdMainTextCause.Text = cboIcds.Text;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkEditIcdCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (txtIcdMainTextCause.Text != null)
					{
						//this.data.DelegateRefeshIcdMainText(txtIcdMainText.Text);
					}
					if (cboIcdsCause.EditValue != null)
					{
						//var hisIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == (long)cboIcds.EditValue).FirstOrDefault();
						//this.data.DelegateRefeshIcd(hisIcd);
					}
					NextForcusSubIcdCause();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtIcdCodeCause_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
		{
			try
			{
				e.ErrorText = Resources.ResourceMessage.IcdKhongDung;
				AutoValidate = AutoValidate.EnableAllowFocusChange;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtIcdCodeCause_Validating(object sender, CancelEventArgs e)
		{
			try
			{
				var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
				if (!String.IsNullOrEmpty(search))
				{
					var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
					var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
					if (result == null || result.Count <= 0)
					{
						e.Cancel = true;
					}
					else
					{
						txtIcdCodeCause.ErrorText = "";
						dxValidationProviderControl.RemoveControlError(txtIcdCodeCause);
						ValidationICDCause(10, 500, true);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboIcdsCause_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == ButtonPredefines.Delete)
				{
					if (!cboIcdsCause.Properties.Buttons[1].Visible)
						return;
					this._TextIcdNameCause = "";
					cboIcdsCause.EditValue = null;
					txtIcdCodeCause.Text = "";
					txtIcdMainTextCause.Text = "";
					cboIcdsCause.Properties.Buttons[1].Visible = false;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboIcdsCause_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (String.IsNullOrEmpty(cboIcdsCause.Text))
				{
					cboIcdsCause.EditValue = null;
					txtIcdMainTextCause.Text = "";
					chkEditIcdCause.Checked = false;
				}
				else
				{
					this._TextIcdNameCause = cboIcdsCause.Text;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		#endregion

		#region UcDate
		private void dtInstructionTime_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				//Thay đổi ngày chỉ định, phải load lại đối tượng thanh toán của BN tương ứng với ngày đó
				if (!this.isNotLoadWhileChangeInstructionTimeInFirst)
				{
					this.ChangeIntructionTimeEditor(this.dtInstructionTime.DateTime);
				}
				if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign)
				{
					this.intructionTimeSelected = new List<DateTime?>();
					this.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtInstructionTime.DateTime.ToString("yyyyMMdd") + this.timeSelested.ToString("HHmm") + "00");
					this.intructionTimeSelected.Add(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.InstructionTime));
					this.intructionTimeSelecteds = new List<long>();
					this.intructionTimeSelecteds.Add(this.InstructionTime);
					ProcessGetDataDepartment();
					CheckTimeInDepartment(this.intructionTimeSelecteds);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void dtInstructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					timeIntruction.Focus();
					timeIntruction.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtInstructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					timeIntruction.Focus();
					timeIntruction.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtInstructionTime_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == ButtonPredefines.Glyph)
				{
					frmMultiIntructonTime frmChooseIntructionTime = new frmMultiIntructonTime(intructionTimeSelected, timeSelested, SelectMultiIntructionTime);
					frmChooseIntructionTime.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkMultiIntructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					NextForcusUCDate();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkMultiIntructionTime_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (isStopEventChangeMultiDate)
				{
					return;
				}

				this.txtInstructionTime.Visible = this.chkMultiIntructionTime.Checked;
				this.dtInstructionTime.Visible = !this.chkMultiIntructionTime.Checked;

				this.timeIntruction.EditValue = DateTime.Now.ToString("HH:mm");
				if (this.chkMultiIntructionTime.Checked)
				{
					string strTimeDisplay = DateTime.Now.ToString("dd/MM");
					this.txtInstructionTime.Text = strTimeDisplay;
					txtDutruTime.Enabled = false;
				}
				else
				{
					this.dtInstructionTime.EditValue = DateTime.Now;
					txtDutruTime.Enabled = true;
				}
				this.DelegateMultiDateChanged();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void timeIntruction_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (this.chkMultiIntructionTime.Enabled || lcichkMultiDate.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
					{
						this.chkMultiIntructionTime.Focus();
					}
					else
					{
						this.NextForcusUCDate();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void timeIntruction_Leave(object sender, EventArgs e)
		{
			try
			{
				//Thay đổi ngày chỉ định, phải load lại đối tượng thanh toán của BN tương ứng với ngày đó
				if (!this.isNotLoadWhileChangeInstructionTimeInFirst)
				{
					this.ChangeIntructionTimeEditor(this.dtInstructionTime.DateTime);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion

		private void btnDepositService_Click(object sender, EventArgs e)
		{
			try
			{
				if (!btnDepositService.Enabled || this.treatmentId == 0)
					return;
				if (cboCashierRoom.EditValue == null)
				{
					MessageBox.Show("Chưa chọn phòng thu ngân");
					return;
				}
				Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DepositService").FirstOrDefault();
				if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.DepositService'");

				V_HIS_TREATMENT_FEE treatmentFee = new V_HIS_TREATMENT_FEE();
				List<V_HIS_SERE_SERV_5> listSereServ5 = new List<V_HIS_SERE_SERV_5>();
				MOS.Filter.HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
				filter.ID = this.treatmentId;
				var treatmentFeeList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, null);
				if (treatmentFeeList != null && treatmentFeeList.Count > 0)
				{
					treatmentFee = treatmentFeeList.FirstOrDefault();
				}

				if (this.serviceReqComboResultSDO != null && this.serviceReqComboResultSDO.SereServs != null && this.serviceReqComboResultSDO.SereServs.Count > 0)
				{
					AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV, V_HIS_SERE_SERV_5>();
					listSereServ5 = AutoMapper.Mapper.Map<List<V_HIS_SERE_SERV_5>>(this.serviceReqComboResultSDO.SereServs);
				}

				if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
				{
					moduleData.RoomId = this.currentModule.RoomId;
					moduleData.RoomTypeId = this.currentModule.RoomTypeId;
					List<object> listArgs = new List<object>();
					DepositServiceADO ado = new DepositServiceADO();
					ado.hisTreatment = treatmentFee;
					ado.BRANCH_ID = WorkPlace.GetBranchId();
					ado.CashierRoomId = Int64.Parse(cboCashierRoom.EditValue.ToString());
					ado.IsDepositAll = true;
					ado.returnSuccess = returnData;

					ado.SereServs = listSereServ5;
					listArgs.Add(ado);
					listArgs.Add(moduleData);
					var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
					if (extenceInstance == null)
					{
						throw new ArgumentNullException("moduleData is null");
					}

					((Form)extenceInstance).ShowDialog();
					//FillDataToControlBySelectTreatment(true);
					//txtFindTreatmentCode.Focus();
					//txtFindTreatmentCode.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void returnData(bool success)
		{
			try
			{
				if (success && this.serviceReqComboResultSDO.ServiceReqs != null && this.serviceReqComboResultSDO.ServiceReqs.Count > 0)
				{
					Parallel.ForEach(this.serviceReqComboResultSDO.ServiceReqs.Where(f => f.ID > 0), l => l.IS_COLLECTED = 1);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtConsultantLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
					if (String.IsNullOrEmpty(searchCode))
					{
						this.cboConsultantUser.EditValue = null;
						this.FocusShowpopup(cboConsultantUser, true);
					}
					else
					{
						var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
							.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

						var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
						if (searchResult != null && searchResult.Count == 1)
						{
							this.cboConsultantUser.EditValue = searchResult[0].LOGINNAME;
							this.txtConsultantLoginname.Text = searchResult[0].LOGINNAME;
							this.FocusWhileSelectedUser();
						}
						else
						{
							this.cboConsultantUser.EditValue = null;
							this.FocusShowpopup(cboConsultantUser, true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboConsultantUser_Closed(object sender, ClosedEventArgs e)
		{
			try
			{
				if (e.CloseMode == PopupCloseMode.Normal)
				{
					if (this.cboConsultantUser.EditValue != null)
					{
						ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboConsultantUser.EditValue ?? "").ToString()));
						if (data != null)
						{
							this.txtConsultantLoginname.Text = data.LOGINNAME;
						}
					}

					this.FocusWhileSelectedUser();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboConsultantUser_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (this.cboConsultantUser.EditValue != null)
					{
						ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboConsultantUser.EditValue ?? "").ToString()));
						if (data != null)
						{
							this.FocusWhileSelectedUser();
							this.txtConsultantLoginname.Text = data.LOGINNAME;
						}
					}
				}
				else
				{
					this.cboConsultantUser.ShowPopup();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboPackage_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == ButtonPredefines.Delete)
				{
					cboPackage.EditValue = null;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboPackage_Closed(object sender, ClosedEventArgs e)
		{
			try
			{
				if (e.CloseMode == PopupCloseMode.Normal)
				{
					txtDescription.Focus();
					txtDescription.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboPackage_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				this.isNotHandlerWhileChangeToggetSwith = true;
				this.ResetPackageselected();
				if (cboPackage.EditValue != null)
				{
					HIS_PACKAGE package = BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPackage.EditValue));
					this.SelectPackageProcess(package);

				}
				gridControlServiceProcess.RefreshDataSource();
				if (this.toggleSwitchDataChecked.EditValue != null && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true")
				{
					this.LoadDataToGrid(false);
				}
				this.isNotHandlerWhileChangeToggetSwith = false;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboPackage_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					txtDescription.Focus();
					txtDescription.SelectAll();
				}
				else
				{
					cboPackage.ShowPopup();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ResetPackageselected()
		{
			try
			{
				if (this.ServiceIsleafADOs.Any(o => o.IsChecked == true))
				{
					foreach (var item in this.ServiceIsleafADOs)
					{
						if (item.PackagePriceId.HasValue)
						{
							item.IsChecked = false;
							item.PackagePriceId = null;
							item.PATIENT_TYPE_ID = 0;
							item.OTHER_PAY_SOURCE_ID = null;
							item.OTHER_PAY_SOURCE_CODE = "";
							item.OTHER_PAY_SOURCE_NAME = "";
							item.PRIMARY_PATIENT_TYPE_ID = null;
							item.IsNotChangePrimaryPaty = false;
							item.ShareCount = null;
							item.PATIENT_TYPE_CODE = "";
							item.PATIENT_TYPE_NAME = "";
							item.PRICE = 0;
							item.TDL_EXECUTE_ROOM_ID = 0;
							item.IsExpend = false;
							item.IsOutKtcFee = false;
							item.IsKHBHYT = false;
							item.InstructionNote = "";
							item.SERVICE_GROUP_ID_SELECTEDs = null;
							item.IsNoDifference = false;
							item.ErrorMessageAmount = "";
							item.ErrorMessageIsAssignDay = "";
							item.ErrorMessagePatientTypeId = "";
							item.AssignPackagePriceEdit = null;
							item.AssignSurgPriceEdit = null;
							item.ErrorTypeAmount = ErrorType.None;
							item.ErrorTypeIsAssignDay = ErrorType.None;
							item.ErrorTypePatientTypeId = ErrorType.None;
							item.BedFinishTime = null;
							item.BedId = null;
							item.BedStartTime = null;
                            item.SereServEkipADO = null;
                        }
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SelectPackageProcess(HIS_PACKAGE package)
		{
			try
			{
				//this.ResetServiceGroupSelected();
				if (package == null) return;

				List<SereServADO> services = null;
				StringBuilder strMessage = new StringBuilder();

				bool hasError = false;

				List<V_HIS_PACKAGE_DETAIL> packageDetails = BackendDataWorker.Get<V_HIS_PACKAGE_DETAIL>().Where(o => o.PACKAGE_ID == package.ID && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				if (packageDetails != null && packageDetails.Count > 0)
				{
					List<string> otherErrors = new List<string>();
					List<V_HIS_PACKAGE_DETAIL> notHasServicePatys = new List<V_HIS_PACKAGE_DETAIL>();
					Inventec.Common.Logging.LogSystem.Debug("SelectPackageProcess. 1");

					var serviceInPackageIds = packageDetails.Select(o => o.SERVICE_ID).ToList();
					Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceInPackageIds), serviceInPackageIds)
						+ Inventec.Common.Logging.LogUtil.TraceData("package.ID", package.ID));

					foreach (V_HIS_PACKAGE_DETAIL pDetail in packageDetails)
					{
						var service = this.ServiceIsleafADOs.FirstOrDefault(o => pDetail.SERVICE_ID == o.SERVICE_ID);
						if (service != null)
						{
							service.IsChecked = true;
							service.PackagePriceId = package.ID;
							service.AMOUNT = pDetail.AMOUNT;
							service.IsExpend = false;
							service.IsOutKtcFee = false;
							service.IsKHBHYT = false;
							service.InstructionNote = "";
							service.SERVICE_GROUP_ID_SELECTEDs = null;
							service.AssignPackagePriceEdit = null;
							service.AssignSurgPriceEdit = null;
							service.IsNoDifference = false;
							service.OTHER_PAY_SOURCE_ID = null;
							service.OTHER_PAY_SOURCE_CODE = "";
							service.OTHER_PAY_SOURCE_NAME = "";
							this.SetAssignNumOrder(service);
							HIS_PATIENT_TYPE paty = this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, service.SERVICE_ID, service);

							if (!VerifyCheckFeeWhileAssign())
							{
								this.ResetOneService(service);
								service.IsChecked = false;
								break;
							}

							this.FillDataOtherPaySourceDataRow(service);
							if (paty == null || paty.ID != this.currentHisPatientTypeAlter.PATIENT_TYPE_ID)
							{
								notHasServicePatys.Add(pDetail);
							}
							this.ValidServiceDetailProcessing(service);
						}
						else
						{
							notHasServicePatys.Add(pDetail);
						}
					}

					if (notHasServicePatys != null && notHasServicePatys.Count > 0)
					{
						string sJoin = String.Join(", ", notHasServicePatys.Select(s => s.SERVICE_NAME).ToList());
						strMessage.Append(String.Format(ResourceMessage.CacDichVuTrongGoiChuaDuocThietLapChinhSachGiaHoacPhongThucHien, this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME, sJoin));
						hasError = true;
					}

					if (otherErrors != null && otherErrors.Count > 0)
					{
						string sJoin = String.Join("; ", notHasServicePatys.Select(s => s.SERVICE_NAME).ToList());
						if (hasError)
							strMessage.Append("; ").Append(sJoin);
						else
							strMessage.Append(sJoin);
						hasError = true;
					}

					Inventec.Common.Logging.LogSystem.Debug("SelectOneServiceGroupProcess. 2");
				}
				else
				{
					strMessage.Append(ResourceMessage.GoiDichVuChuaDuocThietLapDichVu);
					hasError = true;
				}

				WaitingManager.Hide();
				if (hasError)
				{
					this.ResetPackageselected();
					MessageManager.Show(strMessage.ToString());
					this.cboPackage.EditValue = null;
				}

				this.SetEnableButtonControl(this.actionType);
				this.VerifyWarningOverCeiling();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboAssignRoom_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (!IsFirstloadForm || (IsFirstloadForm && cboAssignRoom.EditValue != null && Int64.Parse(cboAssignRoom.EditValue.ToString()) != currentModule.RoomId))
					this.GetExroRoom();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void UpdateSwithExpendAll()
		{
			try
			{
				if (Switch_ExpendAll.IsOn)
				{
					lciSwitch_ExpendAll.Text = Inventec.Common.Resource.Get.Value("frmAssignService.OpenAll", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
					IscheckAllTreeService = true;
					this.treeService.ExpandAll();

				}
				else
				{
					lciSwitch_ExpendAll.Text = Inventec.Common.Resource.Get.Value("frmAssignService.CloseAll", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
					this.treeService.CollapseAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void Switch_ExpendAll_Toggled(object sender, EventArgs e)
		{
			try
			{
				UpdateSwithExpendAll();

				if (isNotLoadWhileChangeControlStateInFirst)
				{
					return;
				}
				WaitingManager.Show();
				HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == Switch_ExpendAll.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
				//Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
				if (csAddOrUpdate != null)
				{
					csAddOrUpdate.VALUE = (Switch_ExpendAll.IsOn ? "1" : "");
				}
				else
				{
					csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
					csAddOrUpdate.KEY = Switch_ExpendAll.Name;
					csAddOrUpdate.VALUE = (Switch_ExpendAll.IsOn ? "1" : "");
					csAddOrUpdate.MODULE_LINK = moduleLink;
					if (this.currentControlStateRDO == null)
						this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
					this.currentControlStateRDO.Add(csAddOrUpdate);
				}
				this.controlStateWorker.SetData(this.currentControlStateRDO);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnConfiguration_Click(object sender, EventArgs e)
		{
			try
			{
				popupControlContainer1.ShowPopup(new Point(btnConfiguration.Bounds.X, btnConfiguration.Bounds.Bottom - 170));
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		/// <summary>
		/// load gridview cấu hình
		/// </summary>
		private void loadCauHinhIn()
		{
			try
			{
				lstLoaiPhieu = new List<LoaiPhieuInADO>()
				{
					new LoaiPhieuInADO("gridView7_1", "Phiếu yêu cầu dịch vụ",true),
					new LoaiPhieuInADO("gridView7_2", "Hướng dẫn bệnh nhân"),
					new LoaiPhieuInADO("gridView7_3", "Yêu cầu thanh toán QR")
				};

				gridView7.BeginUpdate();
				gridView7.GridControl.DataSource = lstLoaiPhieu;
				gridView7.EndUpdate();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridView7_CellValueChanged(object sender, CellValueChangedEventArgs e)
		{
			try
			{
				WaitingManager.Show();
				foreach (var item in lstLoaiPhieu)
				{
					//if (item.ID == "gridView7_3") continue;
					HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == item.ID && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
					if (csAddOrUpdate != null)
					{
						csAddOrUpdate.VALUE = (item.Check ? "1" : "");
					}
					else
					{
						csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
						csAddOrUpdate.KEY = item.ID;
						csAddOrUpdate.VALUE = (item.Check ? "1" : "");
						csAddOrUpdate.MODULE_LINK = moduleLink;
						if (this.currentControlStateRDO == null)
							this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
						this.currentControlStateRDO.Add(csAddOrUpdate);
					}
				}
				this.controlStateWorker.SetData(this.currentControlStateRDO);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void popupControlContainer1_CloseUp(object sender, EventArgs e)
		{

			if (this.gridView7.IsEditing)
				this.gridView7.CloseEditor();

			if (this.gridView7.FocusedRowModified)
				this.gridView7.UpdateCurrentRow();
		}

		private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				CheckEdit edit = sender as CheckEdit;
				if (edit != null)
				{
					gridView7.SetRowCellValue(gridView7.FocusedRowHandle, gridColumn6, edit.Checked);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			try
			{
				SaveWithGridpatientSelect(TypeButton.EDIT, chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void bbtnEditCtrlU_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			if (btnEdit.Enabled)
				btnEdit_Click(null, null);
		}

		private void repositoryItemButtonCondition_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				this.currentRowSereServADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
				if (this.currentRowSereServADO != null)
				{
					if (e.Button.Kind == ButtonPredefines.Down)
					{
						ButtonEdit editor = sender as ButtonEdit;
						Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
						popupControlContainerCondition.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 160));
                        long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
                        var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(this.currentRowSereServADO.SERVICE_ID, new List<long> { (this.currentRowSereServADO.PATIENT_TYPE_ID > 0 ? this.currentRowSereServADO.PATIENT_TYPE_ID : this.currentHisPatientTypeAlter.PATIENT_TYPE_ID) });
						if (dataCondition != null && dataCondition.Count > 0)
						{
							List<V_HIS_SERVICE_PATY> dataSource = new List<V_HIS_SERVICE_PATY>();
                            long? intructionNumByType = null;
                            List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == currentRowSereServADO.SERVICE_TYPE_ID).ToList() : null;
                            List<HIS_SERE_SERV> sameService = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.SERVICE_ID == currentRowSereServADO.SERVICE_ID).ToList() : null;
                            intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                            var intructionNum = sameService != null ? (long)sameService.Count() + 1 : 1;
                            foreach (var item in dataCondition)
							{
                                var dt = MOS.ServicePaty.ServicePatyUtil.GetApplied(new List<V_HIS_SERVICE_PATY>() { item }, currentRowSereServADO.TDL_EXECUTE_BRANCH_ID, currentRowSereServADO.TDL_EXECUTE_ROOM_ID, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, currentRowSereServADO.SERVICE_ID, currentRowSereServADO.PATIENT_TYPE_ID, intructionNum, intructionNumByType, currentRowSereServADO.PackagePriceId, item.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
								if (dt != null)
									dataSource.Add(dt);
                            }
                            dataCondition = dataSource.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_CONDITION_ID.HasValue && o.SERVICE_CONDITION_ID > 0).ToList();
							if (dataCondition != null && dataCondition.Count > 0)
							{
								List<V_HIS_SERVICE_PATY> dataConditionTmps = new List<V_HIS_SERVICE_PATY>();
								foreach (var item in dataCondition)
								{
									if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_ID == item.SERVICE_CONDITION_ID))
									{
										dataConditionTmps.Add(item);
									}
								}
								dataCondition.Clear();
								dataCondition.AddRange(dataConditionTmps);
							}
						}
						if (dataCondition != null && dataCondition.Count > 0 && lstConditionService != null && lstConditionService.Count > 0)
						{
							dataCondition = dataCondition.Where(o => lstConditionService.Exists(p => p.SERVICE_ID == currentRowSereServADO.SERVICE_ID && p.ID == o.SERVICE_CONDITION_ID)).ToList();
						}

						gridControlCondition.DataSource = null;
						gridControlCondition.DataSource = dataCondition;
						gridControlCondition.Focus();
						if (this.currentRowSereServADO.SERVICE_CONDITION_ID > 0 && dataCondition != null && dataCondition.Count > 0)
						{
							int focusRow = 0;
							for (int i = 0; i < dataCondition.Count; i++)
							{
								if (dataCondition[i].SERVICE_CONDITION_ID == this.currentRowSereServADO.SERVICE_CONDITION_ID)
								{
									focusRow = i;
								}
							}
							gridViewCondition.FocusedRowHandle = focusRow;
						}
						else
						{
							gridViewCondition.FocusedRowHandle = 0;
						}
					}
					else if (e.Button.Kind == ButtonPredefines.Delete)
					{
						this.currentRowSereServADO.SERVICE_CONDITION_ID = null;
						this.currentRowSereServADO.SERVICE_CONDITION_NAME = "";
						this.gridControlServiceProcess.RefreshDataSource();

						if (this.gridViewServiceProcess.IsEditing)
							this.gridViewServiceProcess.CloseEditor();

						if (this.gridViewServiceProcess.FocusedRowModified)
							this.gridViewServiceProcess.UpdateCurrentRow();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridControlCondition_Click(object sender, EventArgs e)
		{
			try
			{
				V_HIS_SERVICE_PATY conditionRow = (V_HIS_SERVICE_PATY)gridViewCondition.GetFocusedRow();
				if (conditionRow != null)
				{
					popupControlContainerCondition.HidePopup();
					SereServADO ssADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
					if (ssADO != null)
					{
						ssADO.SERVICE_CONDITION_ID = conditionRow.SERVICE_CONDITION_ID;
						ssADO.SERVICE_CONDITION_NAME = conditionRow.SERVICE_CONDITION_NAME;
						this.gridControlServiceProcess.RefreshDataSource();

						if (this.gridViewServiceProcess.IsEditing)
							this.gridViewServiceProcess.CloseEditor();

						if (this.gridViewServiceProcess.FocusedRowModified)
							this.gridViewServiceProcess.UpdateCurrentRow();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridViewCondition_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					var conditionRow = (V_HIS_SERVICE_PATY)this.gridViewCondition.GetFocusedRow();
					if (conditionRow != null)
					{
						popupControlContainerCondition.HidePopup();
						SereServADO ssADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
						if (ssADO != null)
						{
							ssADO.SERVICE_CONDITION_ID = conditionRow.SERVICE_CONDITION_ID;
							ssADO.SERVICE_CONDITION_NAME = conditionRow.SERVICE_CONDITION_NAME;
							this.gridControlServiceProcess.RefreshDataSource();

							if (this.gridViewServiceProcess.IsEditing)
								this.gridViewServiceProcess.CloseEditor();

							if (this.gridViewServiceProcess.FocusedRowModified)
								this.gridViewServiceProcess.UpdateCurrentRow();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}

		private void gridViewCondition_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
					{
						SereServADO ssADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
						if (ssADO != null)
						{
							List<V_HIS_SERVICE_PATY> servicePatieDatas = ((List<V_HIS_SERVICE_PATY>)((BaseView)sender).DataSource);

							V_HIS_SERVICE_PATY oneServiceSDO = (V_HIS_SERVICE_PATY)servicePatieDatas[e.ListSourceRowIndex];
							if (oneServiceSDO != null)
							{
								if (e.Column.FieldName == "HEIN_RATIO_DISPLAY")
								{
									e.Value = oneServiceSDO.HEIN_RATIO.HasValue ? (decimal?)Inventec.Common.Number.Convert.NumberToNumberRoundMax4((decimal)((oneServiceSDO.HEIN_RATIO ?? 0) * 100)) : null;
								}
							}
							else
							{
								e.Value = null;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void repositoryItemButtonEditOtherPaySource_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				this.currentRowSereServADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
				if (this.currentRowSereServADO != null && this.currentRowSereServADO.IsChecked)
				{
					if (e.Button.Kind == ButtonPredefines.Down || e.Button.Kind == ButtonPredefines.DropDown)
					{
						ButtonEdit editor = sender as ButtonEdit;
						Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
						popupControlContainerOtherPaySource.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 160));

						var dataOtherPaySources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
						List<HIS_OTHER_PAY_SOURCE> dataOtherPaySourceTmps = new List<HIS_OTHER_PAY_SOURCE>();
						dataOtherPaySources = (dataOtherPaySources != null && dataOtherPaySources.Count > 0) ? dataOtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
						if (dataOtherPaySources != null && dataOtherPaySources.Count > 0)
						{
							var workingPatientType = currentPatientTypes.Where(t => t.ID == this.currentRowSereServADO.PATIENT_TYPE_ID).FirstOrDefault();

							if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS))
							{
								dataOtherPaySourceTmps = dataOtherPaySources.Where(o => ("," + workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList();
							}
							else
							{
								dataOtherPaySourceTmps.AddRange(dataOtherPaySources);
							}

						}

						gridControlOtherPaySource.DataSource = null;
						gridControlOtherPaySource.DataSource = dataOtherPaySourceTmps;
						gridControlOtherPaySource.Focus();

						int focusRow = 0;
						if (this.currentRowSereServADO.OTHER_PAY_SOURCE_ID > 0 && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Count > 0)
						{

							for (int i = 0; i < dataOtherPaySourceTmps.Count; i++)
							{
								if (dataOtherPaySourceTmps[i].ID == this.currentRowSereServADO.OTHER_PAY_SOURCE_ID)
								{
									focusRow = i;
								}
							}
						}
						gridViewOtherPaySource.FocusedRowHandle = focusRow;
					}
					else if (e.Button.Kind == ButtonPredefines.Delete)
					{
						this.currentRowSereServADO.OTHER_PAY_SOURCE_ID = null;
						this.currentRowSereServADO.OTHER_PAY_SOURCE_CODE = "";
						this.currentRowSereServADO.OTHER_PAY_SOURCE_NAME = "";
						this.gridControlServiceProcess.RefreshDataSource();

						if (this.gridViewServiceProcess.IsEditing)
							this.gridViewServiceProcess.CloseEditor();

						if (this.gridViewServiceProcess.FocusedRowModified)
							this.gridViewServiceProcess.UpdateCurrentRow();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridControlOtherPaySource_Click(object sender, EventArgs e)
		{
			try
			{
				HIS_OTHER_PAY_SOURCE conditionRow = (HIS_OTHER_PAY_SOURCE)gridViewOtherPaySource.GetFocusedRow();
				if (conditionRow != null)
				{
					popupControlContainerOtherPaySource.HidePopup();
					this.currentRowSereServADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
					if (this.currentRowSereServADO != null)
					{
						this.currentRowSereServADO.OTHER_PAY_SOURCE_ID = conditionRow.ID;
						this.currentRowSereServADO.OTHER_PAY_SOURCE_NAME = conditionRow.OTHER_PAY_SOURCE_NAME;
						this.gridControlServiceProcess.RefreshDataSource();

						if (this.gridViewServiceProcess.IsEditing)
							this.gridViewServiceProcess.CloseEditor();

						if (this.gridViewServiceProcess.FocusedRowModified)
							this.gridViewServiceProcess.UpdateCurrentRow();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridViewOtherPaySource_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Enter)
				{
					var conditionRow = (HIS_OTHER_PAY_SOURCE)this.gridViewOtherPaySource.GetFocusedRow();
					if (conditionRow != null)
					{
						popupControlContainerOtherPaySource.HidePopup();
						this.currentRowSereServADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
						if (this.currentRowSereServADO != null)
						{
							this.currentRowSereServADO.OTHER_PAY_SOURCE_ID = conditionRow.ID;
							this.currentRowSereServADO.OTHER_PAY_SOURCE_NAME = conditionRow.OTHER_PAY_SOURCE_NAME;
							this.gridControlServiceProcess.RefreshDataSource();

							if (this.gridViewServiceProcess.IsEditing)
								this.gridViewServiceProcess.CloseEditor();

							if (this.gridViewServiceProcess.FocusedRowModified)
								this.gridViewServiceProcess.UpdateCurrentRow();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}

		private void gridViewOtherPaySource_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
					{
						SereServADO ssADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
						if (ssADO != null)
						{
							List<HIS_OTHER_PAY_SOURCE> servicePatieDatas = ((List<HIS_OTHER_PAY_SOURCE>)((BaseView)sender).DataSource);

							HIS_OTHER_PAY_SOURCE oneServiceSDO = (HIS_OTHER_PAY_SOURCE)servicePatieDatas[e.ListSourceRowIndex];
							if (oneServiceSDO != null)
							{
								if (e.Column.FieldName == "HEIN_RATIO_DISPLAY")
								{
									//e.Value = oneServiceSDO.HEIN_RATIO.HasValue ? (decimal?)Inventec.Common.Number.Convert.NumberToNumberRoundMax4((decimal)((oneServiceSDO.HEIN_RATIO ?? 0) * 100)) : null;
								}
							}
							else
							{
								e.Value = null;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ProcessAfterChangeTrackingTime(HIS_TRACKING tracking)
		{
			try
			{
				DateTime now = DateTime.Now;
				now = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(tracking.TRACKING_TIME) ?? DateTime.Now;
				if (now != null && now != DateTime.MinValue)
				{
					this.timeIntruction.EditValue = now.ToString("HH:mm");
					this.dtInstructionTime.EditValue = now;
					if (actionType == GlobalVariables.ActionEdit)//sửa
					{
						this.ProcessSaveData(false, false, false);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn("Co loi khi delegate ProcessAfterChangeTrackingTime duoc goi tu chuc nang tao/sua to dieu tri", ex);
			}
		}

		private void ckTK_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (isNotLoadWhileChangeControlStateInFirst)
				{
					Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isNotLoadWhileChangeControlStateInFirst), isNotLoadWhileChangeControlStateInFirst));
					return;
				}

				this.ProcessComboConsultant();
				this.cboConsultantUser.EditValue = null;
				this.txtConsultantLoginname.Text = null;
				HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.ckTK && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
				if (csAddOrUpdate != null)
				{
					csAddOrUpdate.VALUE = (ckTK.Checked ? "1" : "");
				}
				else
				{
					csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
					csAddOrUpdate.KEY = ControlStateConstant.ckTK;
					csAddOrUpdate.VALUE = (ckTK.Checked ? "1" : "");
					csAddOrUpdate.MODULE_LINK = moduleLink;
					if (this.currentControlStateRDO == null)
						this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
					this.currentControlStateRDO.Add(csAddOrUpdate);
				}

				this.controlStateWorker.SetData(this.currentControlStateRDO);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn("Co loi khi delegate ProcessAfterChangeTrackingTime duoc goi tu chuc nang tao/sua to dieu tri", ex);
			}
		}
		bool IsClosingForm = false;
		private void frmAssignService_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				IsClosingForm = true;
				this.lstSereServExist = new List<HIS_SERE_SERV>();
				gridViewServiceProcess.ActiveFilter.Clear();
				gridViewServiceProcess.ClearColumnsFilter();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnCreateServiceGroup_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.btnCreateServiceGroup.Enabled)
				{
					List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
					ServiceGroup.FormServiceGroupCreate frm = new ServiceGroup.FormServiceGroupCreate(this.currentModule, serviceCheckeds__Send);
					frm.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repositoryItemIsView_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
				frmAssignServiceHistory frm = new frmAssignServiceHistory(sereServADO, currentHisTreatment, this.currentModule);
				frm.ShowDialog();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repositoryItemcboExcuteRoomPlus_TabService_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == ButtonPredefines.Plus)
				{
					var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
					if (sereServADO != null)
					{
						long time = this.InstructionTime;
						if (this.intructionTimeSelecteds.Count > 1)
						{
							time = this.intructionTimeSelecteds.Min(o => o);
						}

						List<HIS_TREATMENT> lst = new List<HIS_TREATMENT>();
						if (workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null)
						{
							dicValidIcd = new Dictionary<string, string>();
							ListMessError = new List<string>();
							var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
							foreach (var item in lstPatientSelect)
							{
								lst.Add(new HIS_TREATMENT() { TDL_TREATMENT_TYPE_ID = item.TDL_TREATMENT_TYPE_ID, TDL_PATIENT_NAME = item.TDL_PATIENT_NAME });
							}
						}
						else
						{
							lst.Add(currentTreatment);
						}

						BedInfo.FormBedInfo formBed = new BedInfo.FormBedInfo(this.currentModule, this.currentDepartment, time, sereServADO, lst);
						formBed.ShowDialog();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repositoryItemcboExcuteRoom_TabService_Closed(object sender, ClosedEventArgs e)
		{
			try
			{
				DevExpress.XtraEditors.GridLookUpEdit edit = sender as DevExpress.XtraEditors.GridLookUpEdit;
				if (edit == null) return;
				if (edit.EditValue != null)
				{
					this.gridViewServiceProcess.FocusedColumn = this.grcServiceCode_TabService;
					this.gridViewServiceProcess.FocusedColumn = this.gridColumnExecuteRoomName__TabService;
					gridViewServiceProcess.SetRowCellValue(gridViewServiceProcess.FocusedRowHandle, gridColumnExecuteRoomName__TabService, edit.EditValue);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadCurrentPatient()
		{
			try
			{
				if (treatmentId > 0)
				{
					currentTreatment = GetTreatment(treatmentId);
					CommonParam param = new CommonParam();
					MOS.Filter.HisPatientViewFilter patientViewFilter = new MOS.Filter.HisPatientViewFilter();
					patientViewFilter.ID = currentTreatment.PATIENT_ID;
					var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, patientViewFilter, param);
					if (patients != null && patients.Count > 0)
					{
						this.currentPatient = patients.FirstOrDefault();
					}

				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private HIS_TREATMENT GetTreatment(long treatmentId)
		{
			HIS_TREATMENT data = null;
			try
			{
				CommonParam param = new CommonParam();
				HisTreatmentFilter filter = new HisTreatmentFilter();
				filter.ID = treatmentId;
				data = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return data;
		}

		private void cboKsk_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (cboKsk.EditValue != null)
				{
					this.cboKsk.Properties.Buttons[1].Visible = true;
					CommonParam param = new CommonParam();
					HisKskServiceFilter filter = new HisKskServiceFilter();
					filter.KSK_ID = Int64.Parse(cboKsk.EditValue.ToString());
					var dtKskService = new BackendAdapter(param).Get<List<HIS_KSK_SERVICE>>("api/HisKskService/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
					if (dtKskService != null && dtKskService.Count > 0)
					{
						ProcessDisplayKskWithData(dtKskService);
					}
					else
					{
						ResetServiceKskSelected();
						this.toggleSwitchDataChecked.EditValue = false;
						this.SetEnableButtonControl(this.actionType);
						this.SetDefaultSerServTotalPrice();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboKsk_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				if (e.Button.Kind == ButtonPredefines.Delete)
				{
					cboKsk.EditValue = null;
					this.cboKsk.Properties.Buttons[1].Visible = false;
					this.gridControlServiceProcess.DataSource = null;
					this.ResetServiceKskSelected();
					this.toggleSwitchDataChecked.EditValue = false;
					this.SetEnableButtonControl(this.actionType);
					this.SetDefaultSerServTotalPrice();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private bool IsContainString(string arrStr, string str)
		{
			bool result = false;
			try
			{
				if (arrStr.Contains(","))
				{
					var arr = arrStr.Split(',');
					for (int i = 0; i < arr.Length; i++)
					{
						result = arr[i] == str;
						if (result) break;
					}
				}
				else
				{
					result = arrStr == str;
				}
			}
			catch (Exception ex)
			{
				result = false;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return result;
		}

		private void cboTracking_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
		{
			try
			{
				//vì combobox disable nên phải gán lại để hiển thị text

				e.DisplayText = "";
				string ToDieuTri = "";
				if (this.Listtrackings != null && this.Listtrackings.Count > 0)
				{
					foreach (var item in this.Listtrackings)
					{
						if (item == null)
							continue;
						ToDieuTri += Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.TRACKING_TIME);

						if (this.chkMultiIntructionTime.Checked)
						{
							ToDieuTri += ", ";
						}
					}
				}

				e.DisplayText = ToDieuTri;

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void repositoryItemcboPatientType_TabService1_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				//           var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
				//           if (sereServADO != null)
				//           {
				//               if(sereServADO.IsChecked)
				//{
				//                   GridLookUpEdit cbo = sender as GridLookUpEdit;
				//                   if(cbo.EditValue != null)
				//                       sereServADO.PATIENT_TYPE_ID = Int64.Parse(cbo.EditValue.ToString());

				//               }                        
				//           }
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridControlServiceProcess_ProcessGridKey(object sender, KeyEventArgs e)
		{
			try
			{

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void VisibleGridPatient()
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
		private void InitUCPatientSelect()
		{
			try
			{
				this.patientSelectProcessor = new PatientSelectProcessor();
				HIS.UC.PatientSelect.ADO.PatientSelectInitADO ado = new HIS.UC.PatientSelect.ADO.PatientSelectInitADO();
				ado.SelectedSingleRow = PatientSelectedChange;
				ado.IsInitForm = true;
				ado.RoomId = currentModule.RoomId;
				ado.TreatmentId = treatmentId;
				ado.IsShowColumnBedRoomName = true;
				//ado.IsAutoWidth = false;
				ado.LanguageInputADO = new UC.PatientSelect.ADO.LanguageInputADO();
				ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColBedName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColBedName__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColDob__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColDob__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColPatientName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColPatientName__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColSTT__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColSTT__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColTreatmentCode__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColTreatmentCode__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColBedRoomName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColBedRoomName__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColClassifyName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColClassifyName__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
				this.ucPatientSelect = (UserControl)this.patientSelectProcessor.Run(ado);
				if (this.ucPatientSelect != null)
				{
					this.pnlUCPanelRightTop.Controls.Clear();
					this.pnlUCPanelRightTop.Controls.Add(this.ucPatientSelect);
					this.ucPatientSelect.Dock = DockStyle.Fill;
					this.patientSelectProcessor.LoadWithFilter(this.ucPatientSelect, workingAssignServiceADO.FilterFromBedRoomPartiral);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void PatientSelectedChange(V_HIS_TREATMENT_BED_ROOM data)
		{
			try
			{
				if (this.treatmentId == data.TREATMENT_ID)
				{
					Inventec.Common.Logging.LogSystem.Debug("Goi ham thay doi benh nhan nhung kiem tra ma dieu tri cu van nhu ma dieu tri hien tai____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.TREATMENT_ID), data.TREATMENT_ID));
					return;
				}
				this.treatmentId = data.TREATMENT_ID;
				this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.FirstOrDefault());
				this.SetDateUc();
				this.ProcessDataWithTreatmentWithPatientTypeInfo();
				CreateThreadLoadDataForPrint();
				LoadCurrentPatient();
				InitComboKsk();
				DateTime intructTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);
				this.LoadTotalSereServByHeinWithTreatmentAsync(this.treatmentId);
				ProcessPatientSelecttWithPatientTypeInfo();
				this.LoadServicePaty();
				this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
				var patientTypePrimary = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1).ToList();
				this.InitComboPrimaryPatientType(patientTypePrimary);
				this.InitComboUser();
				this.InitComboServiceGroup();
				this.InitComboExecuteRoom();
				this.LoadTreatmentInfo__PatientType();
				this.BindTree();
				this.LoadDataDhst();
				this.InitDefaultFocus();
				assignMulti = false;
				if (workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null)
				{
					var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
					if (lstPatientSelect != null && lstPatientSelect.Count > 1)
					{
						assignMulti = true;
					}
				}
				CheckAssignServiceSimultaneityOption();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (this.patientSelectProcessor != null && this.ucPatientSelect != null)
				{
					this.patientSelectProcessor.FocusSearchTextbox(this.ucPatientSelect);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void repositoryItemButtonEkipTempEn_Click(object sender, EventArgs e)
		{
			try
			{
				var row = this.gridViewServiceProcess.GetFocusedRow() as SereServADO;

				FormEkipUser frm = new FormEkipUser(SetEkipUser, row, this.currentModule);
				frm.ShowDialog();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetEkipUser(SereServADO data)
		{
			try
			{
				var Alls = this.gridControlServiceProcess.DataSource as List<SereServADO>;
				var ss = Alls.FirstOrDefault(o => o.ID == data.ID);
				ss.SereServEkipADO = data.SereServEkipADO;
				gridControlServiceProcess.RefreshDataSource();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkAutoCheckPDDT_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (isNotLoadWhileChangeControlStateInFirst)
				{
					return;
				}
				WaitingManager.Show();
				HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoCheckPDDT.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
				if (csAddOrUpdate != null)
				{
					csAddOrUpdate.VALUE = (chkAutoCheckPDDT.Checked ? "1" : "");
				}
				else
				{
					csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
					csAddOrUpdate.KEY = chkAutoCheckPDDT.Name;
					csAddOrUpdate.VALUE = (chkAutoCheckPDDT.Checked ? "1" : "");
					csAddOrUpdate.MODULE_LINK = moduleLink;
					if (this.currentControlStateRDO == null)
						this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
					this.currentControlStateRDO.Add(csAddOrUpdate);
				}
				this.controlStateWorker.SetData(this.currentControlStateRDO);
				WaitingManager.Hide();
				if (HisConfigCFG.IcdServiceHasCheck != "1" && HisConfigCFG.IcdServiceHasCheck != "2" && HisConfigCFG.IcdServiceHasCheck != "3" && HisConfigCFG.IcdServiceHasCheck != "4" && HisConfigCFG.IcdServiceHasCheck != "5")
					return;
				var ServiceIdsCheck = ServiceIsleafADOs.Where(o => o.IsChecked).Select(o => o.SERVICE_ID).ToList();
				if (ServicePDDTIds != null && ServicePDDTIds.Count == ServiceIdsCheck.Count && ServiceIdsCheck.Exists(o => ServicePDDTIds.Exists(p => p == o)) || ServiceIdsCheck.Count == 0)
				{
					if (!chkAutoCheckPDDT.Checked)
						DelegateSelectedIcd(icdMain);
					else
					{
						List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
						if (icdFromUc != null && icdFromUc.Count > 0)
						{
							CommonParam param = new CommonParam();
							MOS.Filter.HisIcdServiceFilter filter = new HisIcdServiceFilter();
							filter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
							this.icdServicePhacDos = new BackendAdapter(param).Get<List<HIS_ICD_SERVICE>>(
														"api/HisIcdService/Get",

											   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
											   filter,
											   param);
						}
						else
						{
							this.icdServicePhacDos = null;
						}

						if (this.icdServicePhacDos != null && this.icdServicePhacDos.Count > 0)
						{
							ProcessChoiceIcdPhacDo(this.icdServicePhacDos);
						}
						else
						{
							this.ResetDefaultGridData();
						}
					}
				}
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void chkNotCheckService_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (isNotLoadWhileChangeControlStateInFirst)
					return;
				bool HasKey = ConfigApplicationWorker.GetAllConfig().ContainsKey(AppConfigKeys.CONFIG_KEY_CHOOSING_WHEN_SEARCH);
				if (HasKey)
				{
					List<SDA_CONFIG_APP_USER> configAppUserCreats = new List<SDA_CONFIG_APP_USER>();
					List<SDA_CONFIG_APP_USER> configAppUserUpdates = new List<SDA_CONFIG_APP_USER>();
					var configAppADO = ConfigApplicationWorker.GetAllConfig()[AppConfigKeys.CONFIG_KEY_CHOOSING_WHEN_SEARCH];
					var currentConfigAppUsers = ConfigApplicationWorker.ListSdaConfigAppUser.Where(o => o.CONFIG_APP_ID == configAppADO.CONFIG_APP_ID).ToList();
					if (currentConfigAppUsers == null || currentConfigAppUsers.Count == 0)
					{
						SDA_CONFIG_APP_USER configAppUserCreat = new SDA_CONFIG_APP_USER();
						configAppUserCreat.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
						configAppUserCreat.VALUE = chkNotCheckService.Checked ? "1" : "0";
						configAppUserCreat.CONFIG_APP_ID = configAppADO.CONFIG_APP_ID;
						configAppUserCreats.Add(configAppUserCreat);
					}
					else
					{
						SDA_CONFIG_APP_USER configAppUserUpdate = new SDA_CONFIG_APP_USER();
						configAppUserUpdate.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
						configAppUserUpdate.VALUE = chkNotCheckService.Checked ? "1" : "0";
						configAppUserUpdate.CONFIG_APP_ID = configAppADO.CONFIG_APP_ID;
						configAppUserUpdate.ID = currentConfigAppUsers.FirstOrDefault().ID;
						configAppUserUpdates.Add(configAppUserUpdate);
					}
					if (configAppUserCreats != null && configAppUserCreats.Count > 0)
					{
						CallApiConfigUserCreate(configAppUserCreats);
					}
					if (configAppUserUpdates != null && configAppUserUpdates.Count > 0)
					{
						CallApiConfigUserUpdate(configAppUserUpdates);
					}
				}
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private async Task CallApiConfigUserCreate(List<SDA_CONFIG_APP_USER> configAppUserCreats)
		{
			try
			{
				if (configAppUserCreats != null && configAppUserCreats.Count > 0)
				{
					CommonParam param = new CommonParam();
					var createResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
															   "api/SdaConfigAppUser/CreateList",
															   ApiConsumers.SdaConsumer,
															   configAppUserCreats,
															   param);
					if (createResult != null)
					{
						ConfigApplicationWorker.ReloadAll();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private async Task CallApiConfigUserUpdate(List<SDA_CONFIG_APP_USER> configAppUserUpdates)
		{
			try
			{
				CommonParam param = new CommonParam();
				if (configAppUserUpdates != null && configAppUserUpdates.Count > 0)
				{
					var createResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
						"api/SdaConfigAppUser/UpdateList", ApiConsumers.SdaConsumer, configAppUserUpdates, param);
					if (createResult != null)
					{
						ConfigApplicationWorker.ReloadAll();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private bool IsActionButtonPrintBill = false;
		private void btnPrintBill_Click(object sender, EventArgs e)
		{
			try
			{
				IsActionButtonPrintBill = true;
				InYeuCauThanhToanQR(chkPrint.Checked, false, true);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InPhieuPhieuThanhToan()
		{
			try
			{
				CommonParam param = new CommonParam();
				V_HIS_TREATMENT_FEE currentTreatment = null;
				MOS.Filter.HisTreatmentFeeViewFilter treatmentViewFilter = new HisTreatmentFeeViewFilter();
				treatmentViewFilter.ID = this.treatmentId;
				var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, treatmentViewFilter, param);
				if (treatments != null && treatments.Count > 0)
				{
					currentTreatment = treatments.FirstOrDefault();
				}
				HisTransReqFilter filter = new HisTransReqFilter();
				HIS_TRANS_REQ transReq = null;
				List<HIS_SESE_TRANS_REQ> SeseTransReqList = null;
				List<V_HIS_SERE_SERV> SereServViewList = null;
				filter.TREATMENT_ID = treatmentId;
				var transReqLst = new Inventec.Common.Adapter.BackendAdapter(param)
				  .Get<List<MOS.EFMODEL.DataModels.HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
				if (transReqLst != null && transReqLst.Count > 0)
					transReqLst = transReqLst.Where(o => o.TRANS_REQ_TYPE == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_SERVICE).ToList();
				if (transReqLst != null && transReqLst.Count > 0)
					transReq = transReqLst.OrderByDescending(o => o.CREATE_TIME).ToList()[0];
				if (transReq != null)
				{
					MOS.Filter.HisSeseTransReqFilter SeseTransReqFilter = new HisSeseTransReqFilter();
					SeseTransReqFilter.TRANS_REQ_ID = transReq.ID;
					SeseTransReqList = new BackendAdapter(param).Get<List<HIS_SESE_TRANS_REQ>>("api/HisSeseTransReq/Get", ApiConsumer.ApiConsumers.MosConsumer, SeseTransReqFilter, param);
					if (SeseTransReqList != null && SeseTransReqList.Count > 0)
					{
						MOS.Filter.HisSereServViewFilter SereServViewFilter = new HisSereServViewFilter();
						SereServViewFilter.IDs = SeseTransReqList.Select(o => o.SERE_SERV_ID).ToList();
						SereServViewList = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, SereServViewFilter, param);
					}
				}
				List<V_HIS_ROOM> listRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
				List<V_HIS_SERVICE> listService = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
				List<HIS_SERVICE_TYPE> listServiceType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repSampleType_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				this.currentRowSereServADO = (SereServADO)gridViewServiceProcess.GetFocusedRow();
				if (this.currentRowSereServADO != null && this.currentRowSereServADO.IsChecked && e.Button.Kind == ButtonPredefines.Delete)
				{
					this.currentRowSereServADO.TEST_SAMPLE_TYPE_CODE = null;
					this.currentRowSereServADO.TEST_SAMPLE_TYPE_ID = 0;
					this.currentRowSereServADO.TEST_SAMPLE_TYPE_NAME = null;
					this.gridControlServiceProcess.RefreshDataSource();

					if (this.gridViewServiceProcess.IsEditing)
						this.gridViewServiceProcess.CloseEditor();

					if (this.gridViewServiceProcess.FocusedRowModified)
						this.gridViewServiceProcess.UpdateCurrentRow();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnSettingAssignOld_Click(object sender, EventArgs e)
		{
			try
			{
				sAssignOld = new frmSettingAssignOld(indexServiceType, (index) =>
				{
					indexServiceType = index;
					HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboPriviousServiceReq.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
					if (csAddOrUpdate != null)
					{
						csAddOrUpdate.VALUE = indexServiceType.ToString();
					}
					else
					{
						csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
						csAddOrUpdate.KEY = cboPriviousServiceReq.Name;
						csAddOrUpdate.VALUE = indexServiceType.ToString();
						csAddOrUpdate.MODULE_LINK = moduleLink;
						if (this.currentControlStateRDO == null)
							this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
						this.currentControlStateRDO.Add(csAddOrUpdate);
					}
					this.controlStateWorker.SetData(this.currentControlStateRDO);

					WaitingManager.Show();
					LogSystem.Debug("Begin FillDataToComboPriviousServiceReq");
					this.FillDataToComboPriviousServiceReq();
					LogSystem.Debug("End FillDataToComboPriviousServiceReq");
					WaitingManager.Hide();

				});
				sAssignOld.ShowDialog();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridView14_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
					{
						V_HIS_SERVICE_REQ_6 oneServiceSDO = (V_HIS_SERVICE_REQ_6)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
						if (oneServiceSDO != null)
						{
							if (e.Column.FieldName == "INTRUCTION_TIME_str")
							{
								e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(oneServiceSDO.INTRUCTION_TIME);
							}
						}
						else
						{
							e.Value = null;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		V_HIS_SERVICE_REQ_6 rowdataAssignOld { get; set; }
		private void gridView14_RowCellClick(object sender, RowCellClickEventArgs e)
		{

			try
			{
				rowdataAssignOld = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6)gridView14.GetFocusedRow();

				if (rowdataAssignOld != null)
				{
					cboPriviousServiceReq.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowdataAssignOld.INTRUCTION_TIME);
					this.cboPriviousServiceReq.Properties.Buttons[1].Visible = true;
					this.ProcessChoiceServiceReqPrevious(rowdataAssignOld);
					this.btnSave.Focus();
				}
				popupControlContainer3.HidePopup();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

		}
		List<HIS_CONFIG> listConfig = new List<HIS_CONFIG>();
		#region thanh toan QR
		private void CheckEnableBtnQR()
		{
			try
			{
				listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
				
				btnQRPay.Enabled = true;
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}
		private HIS_CONFIG selectedConfig = new HIS_CONFIG();
		private void btnQRPay_Click(object sender, EventArgs e)
		{
			try
            {
				if (currentWorkingRoom != null && !string.IsNullOrEmpty(currentWorkingRoom.QR_CONFIG_JSON))
                {
                    List<object> listArgs = new List<object>();
                    TransReqQRADO adoqr = new TransReqQRADO();
                    try
                    {
                        var json = Newtonsoft.Json.JsonConvert.DeserializeObject<BankInfo>(currentWorkingRoom.QR_CONFIG_JSON);
						if (json != null)
						{
							adoqr.ConfigValue = new HIS_CONFIG() { VALUE = json.VALUE, KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", json.BANK) };
							adoqr.BankName = json.BANK;
						}
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        XtraMessageBox.Show("Định dạng Qr thiết lập trong kho phòng không hợp lệ");
                        return;
                    }
                    adoqr.TreatmentId = this.treatmentId;
                    adoqr.TransReqId = 0;
                    adoqr.DelegtePrint = this.serviceReqComboResultSDO != null ? (HIS.Desktop.Common.RefeshReference)IN_QR : null;
                    listArgs.Add(adoqr);
                    LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR ; KEY: " + selectedConfig.KEY);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
				else
				{
					if (listConfig != null)
					{
						if (listConfig.Count > 1)
						{
							popupMenu1.ClearLinks();
							foreach (var item in listConfig)
							{
								string key = "";
								string value = item.KEY;
								int index = value.IndexOf("Info");
								if (index > 0)
								{
									var shotkey = value.Substring(0, index);
									string[] parts = shotkey.Split('.');
									if (parts.Length > 0)
									{
										key = parts[parts.Length - 1]; // Lấy phần cuối cùng sau khi tách
									}
								}
								else
								{
									key = item.KEY;
								}


								BarButtonItem btnOption = new BarButtonItem(null, key);
								btnOption.ItemClick += (s, args) =>
								{

									selectedConfig = item;
									List<object> listArgs = new List<object>();
									TransReqQRADO adoqr = new TransReqQRADO();
									adoqr.TreatmentId = this.treatmentId;
									adoqr.ConfigValue = selectedConfig;
									adoqr.TransReqId = 0;
									adoqr.DelegtePrint = this.serviceReqComboResultSDO != null ? (HIS.Desktop.Common.RefeshReference)IN_QR : null;
									adoqr.BankName = key;
									listArgs.Add(adoqr);
									LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR ; KEY: " + selectedConfig.KEY);

									HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

								};
								popupMenu1.AddItem(btnOption);
							}

							popupMenu1.ShowPopup(Control.MousePosition);
						}
						else
						{
							selectedConfig = listConfig[0];
							List<object> listArgs = new List<object>();
							TransReqQRADO adoqr = new TransReqQRADO();
							adoqr.TreatmentId = this.treatmentId;
							adoqr.ConfigValue = selectedConfig;
							adoqr.TransReqId = 0;
							adoqr.DelegtePrint = this.serviceReqComboResultSDO != null ? (HIS.Desktop.Common.RefeshReference)IN_QR : null;

							string key = "";
							string value = selectedConfig.KEY;
							int index = value.IndexOf("Info");
							if (index > 0)
							{
								var shotkey = value.Substring(0, index);
								string[] parts = shotkey.Split('.');
								if (parts.Length > 0)
								{
									key = parts[parts.Length - 1]; // Lấy phần cuối cùng sau khi tách
								}
							}
							else
							{
								key = selectedConfig.KEY;
							}

							adoqr.BankName = key;
							listArgs.Add(adoqr);
							LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + selectedConfig.KEY);
							HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);


						}

					}
				}
			}
			catch (Exception ex)
			{
				LogSystem.Error("Loi khi thuc hien thanh toan QR tam thu: "+ex);
			}
		}
		private void IN_QR()
		{
			try
			{

				if (this.lstLoaiPhieu != null && this.lstLoaiPhieu.Count > 0)
				{
					var checkHDBN = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_2");

					var checkYCDV = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_1");

					var checkQR = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_3");

					if (checkHDBN != null)
					{
						InPhieuHuoangDanBenhNhan(true);
					}

					if (checkYCDV != null)
					{
						InPhieuYeuCauDichVu(true);
					}

					if (checkQR != null)
					{
						InYeuCauThanhToanQR(chkPrint.Checked, false, true);
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

					case "Mps000498":
						InPhieuThanhToan(printTypeCode, fileName, ref result);
						break;
					default:
						break;

				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				result = false;
			}
			return result;
		}

		private void InPhieuThanhToan(string printTypeCode, string fileName, ref bool result)
		{
			try
			{
				V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
				CommonParam param = new CommonParam();

				treatment = BackendDataWorker.Get<V_HIS_TREATMENT>().Where(s => s.ID == treatmentId).FirstOrDefault();

				HisTransReqFilter filter = new HisTransReqFilter();
				HIS_TRANS_REQ transReq = null;
				filter.TREATMENT_ID = treatmentId;
				var transReqLst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
				if (transReqLst != null && transReqLst.Count > 0)
					transReqLst = transReqLst.Where(o => o.TRANS_REQ_TYPE == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_SERVICE).ToList();
				if (transReqLst != null && transReqLst.Count > 0)
					transReq = transReqLst.OrderByDescending(o => o.CREATE_TIME).ToList()[0];
				List<HIS_CONFIG> configRooms = new List<HIS_CONFIG>();
                if (currentWorkingRoom != null && !string.IsNullOrEmpty(currentWorkingRoom.QR_CONFIG_JSON))
                {
                    List<object> listArgs = new List<object>();
                    TransReqQRADO adoqr = new TransReqQRADO();
                    try
                    {
                        var json = Newtonsoft.Json.JsonConvert.DeserializeObject<BankInfo>(currentWorkingRoom.QR_CONFIG_JSON);
                        if (json != null)
                        {
							configRooms.Add(new HIS_CONFIG() { VALUE = json.VALUE, KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", json.BANK) });
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        XtraMessageBox.Show("Định dạng Qr thiết lập trong kho phòng không hợp lệ");
                        return;
                    }
                }
                MPS.Processor.Mps000498.PDO.Mps000498PDO pdo = new MPS.Processor.Mps000498.PDO.Mps000498PDO(treatment, transReq, configRooms != null && configRooms.Count > 0 ? configRooms : listConfig);

				 
				MPS.ProcessorBase.Core.PrintData printData = null;

				string printerName = "";
				if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
				{
					printerName = GlobalVariables.dicPrinter[printTypeCode];
				}

				Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((""), printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);
				WaitingManager.Hide();
				if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
				{

					result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
				}
				else
				{
					result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
				}
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}

        #endregion
        #region dutru time
        private void txtDutruTime_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				selectedDates.Clear();
				Point txtDuTruTimeLocationOnScreen = txtDutruTime.PointToScreen(new Point(0, txtDutruTime.Height));
				popupControlContainer4.ShowPopup(txtDuTruTimeLocationOnScreen);
				foreach (var date in selectedDates)
				{
					calendarControl1.DateTime = date;
				}

			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private List<DateTime> selectedDates = new List<DateTime>();
		public List<long> USE_TIME { get; set; }
		private void btnSave_duTru_Click(object sender, EventArgs e)
		{
			try
			{
				PopupContainerBarControl control = popupControlContainer4.Parent as PopupContainerBarControl;
				control.ClosePopup();
				txtDutruTime.Text = "";
				if(selectedDates != null)selectedDates.Clear();
				if(this.USE_TIME!= null) this.USE_TIME.Clear();
				chkMultiIntructionTime.Enabled = true;
				foreach (DateRange item in calendarControl1.SelectedRanges)
				{
					if (item != null)
					{
						var dt = item.StartDate;
						while (dt.Date < item.EndDate.Date)
						{

							if (!selectedDates.Contains(dt)) selectedDates.Add(dt);
							dt = dt.AddDays(1);
						}
					}


				}
				UpdateSelectedDatesText();
				
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void txtDuTruTime_Validated(object sender, EventArgs e)
		{
			try
			{

				if (!string.IsNullOrEmpty(txtDutruTime.Text))
				{
					var date = txtDutruTime.Text.Trim().Split(';');
					foreach (var _date in date)
					{
						DateTime selectedDate;
						bool isValidDate = DateTime.TryParse(_date.ToString(), out selectedDate);
						if (isValidDate && !selectedDates.Contains(selectedDate)) selectedDates.Add(selectedDate);
					}
					UpdateSelectedDatesText();
				}
				else
				{
					txtDutruTime.Text = "";
					if (selectedDates != null) selectedDates.Clear();
					if (this.USE_TIME != null) this.USE_TIME.Clear();
					chkMultiIntructionTime.Enabled = true;
				}

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void txtDutruTime_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
                if (string.IsNullOrEmpty(txtDutruTime.Text))
                {
					txtDutruTime.Text = "";
					if (selectedDates != null) selectedDates.Clear();
					if (this.USE_TIME != null) this.USE_TIME.Clear();
					chkMultiIntructionTime.Enabled = true;
				}
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		// Hàm cập nhật TextEdit với chuỗi ngày đã chọn
		private void UpdateSelectedDatesText()
		{
			try
			{
				selectedDates = selectedDates.Distinct().ToList();

				txtDutruTime.Text = string.Join(";", selectedDates.Select(d => d.ToString("dd/MM")).ToArray());
				
				
				if (this.USE_TIME == null) this.USE_TIME = new List<long>();
				this.USE_TIME.Clear();
				selectedDates.ForEach(date =>
				{
					try
					{
						var date_number = Convert.ToInt64(date.ToString("yyyyMMdd") + "000000");
						if (date_number > 0)
						{
							if (!this.USE_TIME.Contains(date_number))
								this.USE_TIME.Add(date_number);
						}
					}
					catch (Exception)
					{

						throw new Exception("Loi khi convert date");
					}
				});
				if (!string.IsNullOrEmpty(txtDutruTime.Text))
				{
					chkMultiIntructionTime.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
        #endregion

        
    }
    public class BankInfo
    {
        public BankInfo() { }
        public string BANK { get; set; }
        public string VALUE { get; set; }
    }
}
