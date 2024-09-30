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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AssignPaan.ADO;
using HIS.Desktop.Plugins.AssignPaan.Config;
using HIS.Desktop.Plugins.AssignPaan.Resources;
using HIS.Desktop.Plugins.AssignPaan.Validation;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPaan
{
    public partial class frmAssignPaan : HIS.Desktop.Utility.FormBase
    {
        private const string MPS_000167 = "Mps000167";

        IcdProcessor icdProcessor = null;
        UserControl ucIcd = null;

        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;

        Inventec.Desktop.Common.Modules.Module currentModule;

        long treatmentId = 0;
        long _sereServParentId = 0;
        long serviceReqId = 0;

        V_HIS_TREATMENT treatment = null;
        V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter = null;

        List<HIS_PATIENT_TYPE> listPatientTypeAllow = null;

        Dictionary<long, List<V_HIS_SERVICE_PATY>> dicServicePaty = new Dictionary<long, List<V_HIS_SERVICE_PATY>>();
        Dictionary<long, List<V_HIS_SERVICE_ROOM>> dicServiceRoom = new Dictionary<long, List<V_HIS_SERVICE_ROOM>>();

        List<V_HIS_SERVICE_PATY> hisCurrentServicePatys = new List<V_HIS_SERVICE_PATY>();
        List<V_HIS_SERVICE_ROOM> hisCurrentServiceRooms = new List<V_HIS_SERVICE_ROOM>();

        List<V_HIS_SERVICE> HisService = new List<V_HIS_SERVICE>();

        HisServiceReqResultSDO resultSDO = null;

        HIS.UC.Icd.ADO.IcdInputADO icdAdo = null;
        HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO secondIcdAdo = null;

        const string commonString__true = "1";

        int positionHandleControl = -1;
        List<TrackingAdo> trackingAdos;

        HIS_PATIENT_TYPE HisPatientType = new HIS_PATIENT_TYPE();
        List<V_HIS_SERVICE_PATY> hisAllowServicePatys = new List<V_HIS_SERVICE_PATY>();
        List<HIS_PATIENT_TYPE> listPatientTypeAllowService = new List<HIS_PATIENT_TYPE>();
        HIS_TRACKING currentTracking;
        HIS.Desktop.Common.RefeshReference delegateActionSave;
        #region Constructors
        public frmAssignPaan(Inventec.Desktop.Common.Modules.Module module, long treatmentId, HIS.UC.Icd.ADO.IcdInputADO icdAdo, HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO secondIcdAdo, HIS_TRACKING hisTracking, HIS.Desktop.Common.RefeshReference delegateActionSave)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.icdAdo = icdAdo;
                this.secondIcdAdo = secondIcdAdo;
                this.currentModule = module;
                this.treatmentId = treatmentId;
                this.currentTracking = hisTracking;
                this.delegateActionSave = delegateActionSave;
                this.InitUcIcd();
                this.InitUcSecondaryIcd();
                Resources.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmAssignPaan(Inventec.Desktop.Common.Modules.Module module, long treatmentId, HIS.UC.Icd.ADO.IcdInputADO icdAdo, HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO secondIcdAdo, long serviceReqId, HIS_TRACKING hisTracking, HIS.Desktop.Common.RefeshReference delegateActionSave)
            : this(module, treatmentId, icdAdo, secondIcdAdo, hisTracking, delegateActionSave)
        {
            this.serviceReqId = serviceReqId;
        }

        public frmAssignPaan(Inventec.Desktop.Common.Modules.Module module, V_HIS_SERE_SERV_5 _sereServ, HIS.UC.Icd.ADO.IcdInputADO icdAdo, HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO secondIcdAdo, HIS_TRACKING hisTracking, HIS.Desktop.Common.RefeshReference delegateActionSave)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.icdAdo = icdAdo;
                this.secondIcdAdo = secondIcdAdo;
                this.currentModule = module;
                this.treatmentId = _sereServ.TDL_TREATMENT_ID ?? 0;
                this._sereServParentId = _sereServ.ID;
                this.currentTracking = hisTracking;
                this.delegateActionSave = delegateActionSave;
                this.InitUcIcd();
                this.InitUcSecondaryIcd();
                Resources.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmAssignPaan(Inventec.Desktop.Common.Modules.Module module, V_HIS_SERE_SERV_5 _sereServ, HIS.UC.Icd.ADO.IcdInputADO icdAdo, HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO secondIcdAdo, long serviceReqId, HIS_TRACKING hisTracking, HIS.Desktop.Common.RefeshReference delegateActionSave)
            : this(module, _sereServ, icdAdo, secondIcdAdo, hisTracking, delegateActionSave)
        {
            this.serviceReqId = serviceReqId;
        }
        #endregion

        void LoadDataToTrackingCombo()
        {
            List<TrackingAdo> result = new List<TrackingAdo>();

            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTrackingViewFilter trackingFilter = new HisTrackingViewFilter();//20181227195959
                trackingFilter.TREATMENT_ID = this.treatmentId;
                trackingFilter.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).DepartmentId;
                var trackings = new BackendAdapter(param).Get<List<V_HIS_TRACKING>>("api/HisTracking/GetView", ApiConsumer.ApiConsumers.MosConsumer, trackingFilter, param);
                if (trackings != null && trackings.Count > 0)
                {
                    trackings = trackings.OrderByDescending(o => o.TRACKING_TIME).ToList();
                    foreach (var tracking in trackings)
                    {
                        var trackingAdo = new TrackingAdo(tracking);
                        result.Add(trackingAdo);
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TrackingTimeStr", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboTracking, result, controlEditorADO);
                SetDefautTrackingCombo(result, AppConfig.IsDefaultTracking);
            }
            catch (Exception ex)
            {
                result = new List<TrackingAdo>();
                LogSystem.Error(ex);
            }
            this.trackingAdos = result;
        }

        private void SetDefautTrackingCombo(List<TrackingAdo> result, string isDefaultTracking)
        {
            try
            {
                if (isDefaultTracking == "0" && currentTracking != null)
                {
                    cboTracking.EditValue = currentTracking.ID;
                }
                else if (isDefaultTracking == "1")
                {

                    if (this.dtInstructionTime.EditValue != null && dtInstructionTime.DateTime != DateTime.MinValue && result != null && result.Count > 0)
                    {
                        var strInstructionTime = dtInstructionTime.DateTime.ToString("yyyyMMdd");
                        var trackingDefaults = result.Where(o => o.DEPARTMENT_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).DepartmentId && o.TRACKING_TIME.ToString().Substring(0, 8) == strInstructionTime)
                         .OrderByDescending(o => o.TRACKING_TIME).ToList();
                        if (trackingDefaults != null && trackingDefaults.Count > 0)
                        {
                            cboTracking.EditValue = trackingDefaults.FirstOrDefault().ID;
                        }
                        else
                        {
                            cboTracking.EditValue = null;
                        }
                    }
                    else
                    {
                        cboTracking.EditValue = null;
                    }
                }
                else
                {
                    cboTracking.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList());
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.Width = 660;
                ado.Height = 24;
                ado.TextLblIcd = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_ICD_TEXT", Resources.ResourceLangManager.LanguageFrmAssignPaan, LanguageManager.GetCulture());
                ado.TextNullValue = Inventec.Common.Resource.Get.Value("frmAssignPaan.txtIcdExtraNames.Properties.NullValuePrompt", Resources.ResourceLangManager.LanguageFrmAssignPaan, LanguageManager.GetCulture());
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusOut()
        {
            dtInstructionTime.Focus();
            dtInstructionTime.SelectAll();
        }

        void InitUcIcd()
        {
            try
            {
                icdProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = TxtIcdText;
                ado.Width = 440;
                ado.Height = 24;
                ado.IsColor = (AppConfig.ObligateIcd == commonString__true);
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().ToList();
                this.ucIcd = (UserControl)icdProcessor.Run(ado);

                if (this.ucIcd != null)
                {
                    this.panelControlUcIcd.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void TxtIcdText()
        {
            try
            {
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.FocusControl(ucSecondaryIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmAssignPaan_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetIcon();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                this.LoadLanguageKey();
                this.InitDataByTreatmentId();
                if (this.treatment != null && this.currentPatientTypeAlter != null)
                {
                    this.ResetControlValue();
                    this.ValidControl();
                    this.InitComboPatientType();
                    this.InitComboPaanServiceType(null);
                    this.InitComboExecuteRoom();
                    this.InitComboPaanPosition();
                    this.InitComboPaanLiquid();
                    this.InitComboUsername();
                    this.SetDataSourceCboPatientType();
                    this.SetDataSourceCboExecuteRoom();
                    this.LoadDefaultUser();
                    
                    this.InitDefaultPatientType();
                    this.SetDataSourceCboPaanServiceType();
                    this.InitComboTestSampleType();
                }
                
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitDataByTreatmentId()
        {
            try
            {
                if (treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentViewFilter treatFilter = new HisTreatmentViewFilter();
                    treatFilter.ID = this.treatmentId;
                    var listTreat = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatFilter, param);
                    if (listTreat == null || listTreat.Count != 1)
                    {
                        throw new Exception("Khong lay duoc VHisTreatemnt theo treatmentId:" + treatmentId);
                    }
                    this.treatment = listTreat.FirstOrDefault();

                    this.LoadDataPatientTypeByInstructionTime(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    List<long> _serviceIds = new List<long>();
                    var service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => p.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL && p.IS_ACTIVE == 1).ToList();
                    if (service != null && service.Count > 0)
                    {
                        _serviceIds = service.Select(p => p.ID).ToList();
                    }
                    foreach (var item in BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList())
                    {
                        if (item.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL)
                            continue;
                        if (!_serviceIds.Contains(item.SERVICE_ID))
                            continue;
                        if (!dicServicePaty.ContainsKey(item.PATIENT_TYPE_ID))
                            dicServicePaty[item.PATIENT_TYPE_ID] = new List<V_HIS_SERVICE_PATY>();
                        dicServicePaty[item.PATIENT_TYPE_ID].Add(item);
                    }
                    this.hisAllowServicePatys = new List<V_HIS_SERVICE_PATY>();
                    if (listPatientTypeAllow != null && listPatientTypeAllow.Count > 0)
                    {
                        foreach (var item in listPatientTypeAllow)
                        {
                            if (dicServicePaty.ContainsKey(item.ID))
                                this.hisAllowServicePatys.AddRange(dicServicePaty[item.ID]);
                        }
                    }
                    List<long> _roomIds = new List<long>();
                    var rooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.IS_ACTIVE == 1).ToList();
                    if (rooms != null && rooms.Count > 0)
                    {
                        _roomIds = rooms.Select(p => p.ID).ToList();
                    }
                    var executeRoomIDs = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && _roomIds.Contains(p.ROOM_ID)).Select(o => o.ROOM_ID).ToList();
                    var dataold = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    
                    foreach (var item in dataold)
                    {
                        if (item.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL || item.BRANCH_ID != WorkPlace.GetBranchId())
                            continue;
                        if (executeRoomIDs.Contains(item.ROOM_ID))
                        {
                            if (!dicServiceRoom.ContainsKey(item.SERVICE_ID))
                                dicServiceRoom[item.SERVICE_ID] = new List<V_HIS_SERVICE_ROOM>();
                            dicServiceRoom[item.SERVICE_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void LoadDataPatientTypeByInstructionTime(long instructionTime)
        {
            try
            {
                this.currentPatientTypeAlter = null;
                this.listPatientTypeAllow = new List<HIS_PATIENT_TYPE>();
                HisPatientTypeAlterViewAppliedFilter appliedPatientTypeFilter = new HisPatientTypeAlterViewAppliedFilter();
                appliedPatientTypeFilter.TreatmentId = this.treatment.ID;
                appliedPatientTypeFilter.InstructionTime = instructionTime;
                this.currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("/api/HisPatientTypeAlter/GetApplied", ApiConsumers.MosConsumer, appliedPatientTypeFilter, null);
                if (currentPatientTypeAlter == null)
                {
                    throw new Exception("Khong lay duoc VHisPatientTypeAlter thep treatmentId" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.treatment), this.treatment));
                }
                listPatientTypeAllow = new List<HIS_PATIENT_TYPE>();
                List<long> listPatientTypeId = new List<long>() { this.currentPatientTypeAlter.PATIENT_TYPE_ID };
                var listAllow = BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>().Where(o => o.PATIENT_TYPE_ID == this.currentPatientTypeAlter.PATIENT_TYPE_ID && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (listAllow != null && listAllow.Count > 0)
                {
                    listPatientTypeId.AddRange(listAllow.Select(s => s.PATIENT_TYPE_ALLOW_ID).ToList());
                }
                listPatientTypeAllow = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => listPatientTypeId.Contains(o.ID) && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                //var lstPatientTypeAll = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => listPatientTypeAllow.Where(p => p.ID == this.currentPatientTypeAlter.PATIENT_TYPE_ID).ToList().Exists(t => t.BASE_PATIENT_TYPE_ID == o.ID)).ToList();

                //if (lstPatientTypeAll != null && lstPatientTypeAll.Count > 0)
                //{
                //    listPatientTypeAllow.AddRange(lstPatientTypeAll);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitComboPatientType()
        {
            try
            {
                cboPatientType.Properties.DataSource = null;
                cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ValueMember = "ID";
                cboPatientType.Properties.ForceInitialize();
                cboPatientType.Properties.Columns.Clear();
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_CODE", "", 40));
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_NAME", "", 80));
                cboPatientType.Properties.ShowHeader = false;
                cboPatientType.Properties.ImmediatePopup = true;
                cboPatientType.Properties.DropDownRows = 10;
                cboPatientType.Properties.PopupWidth = 120;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitComboPaanServiceType(object datas)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 210, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 280);
                ControlEditorLoader.Load(this.cboPaanServiceType, datas, controlEditorADO);

                //cboPaanServiceType.Properties.DataSource = null;
                //cboPaanServiceType.Properties.DisplayMember = "SERVICE_NAME";
                //cboPaanServiceType.Properties.ValueMember = "ID";
                //cboPaanServiceType.Properties.ForceInitialize();
                //cboPaanServiceType.Properties.Columns.Clear();
                //cboPaanServiceType.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_CODE", "", 70));
                //cboPaanServiceType.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_NAME", "", 210));
                //cboPaanServiceType.Properties.ShowHeader = false;
                //cboPaanServiceType.Properties.ImmediatePopup = true;
                //cboPaanServiceType.Properties.DropDownRows = 10;
                //cboPaanServiceType.Properties.PopupWidth = 280;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitComboExecuteRoom()
        {
            try
            {
                cboExecuteRoom.Properties.DataSource = null;
                cboExecuteRoom.Properties.DisplayMember = "ROOM_NAME";
                cboExecuteRoom.Properties.ValueMember = "ROOM_ID";
                cboExecuteRoom.Properties.ForceInitialize();
                cboExecuteRoom.Properties.Columns.Clear();
                cboExecuteRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_CODE", "", 70));
                cboExecuteRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_NAME", "", 210));
                cboExecuteRoom.Properties.ShowHeader = false;
                cboExecuteRoom.Properties.ImmediatePopup = true;
                cboExecuteRoom.Properties.DropDownRows = 10;
                cboExecuteRoom.Properties.PopupWidth = 280;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitComboPaanPosition()
        {
            try
            {
                cboPaanPosition.Properties.DataSource = BackendDataWorker.Get<HIS_PAAN_POSITION>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                cboPaanPosition.Properties.DisplayMember = "PAAN_POSITION_NAME";
                cboPaanPosition.Properties.ValueMember = "ID";
                cboPaanPosition.Properties.ForceInitialize();
                cboPaanPosition.Properties.Columns.Clear();
                cboPaanPosition.Properties.Columns.Add(new LookUpColumnInfo("PAAN_POSITION_CODE", "", 80));
                cboPaanPosition.Properties.Columns.Add(new LookUpColumnInfo("PAAN_POSITION_NAME", "", 200));
                cboPaanPosition.Properties.ShowHeader = false;
                cboPaanPosition.Properties.ImmediatePopup = true;
                cboPaanPosition.Properties.DropDownRows = 10;
                cboPaanPosition.Properties.PopupWidth = 280;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitComboPaanLiquid()
        {
            try
            {
                cboPaanLiquid.Properties.DataSource = BackendDataWorker.Get<HIS_PAAN_LIQUID>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                cboPaanLiquid.Properties.DisplayMember = "PAAN_LIQUID_NAME";
                cboPaanLiquid.Properties.ValueMember = "ID";
                cboPaanLiquid.Properties.ForceInitialize();
                cboPaanLiquid.Properties.Columns.Clear();
                cboPaanLiquid.Properties.Columns.Add(new LookUpColumnInfo("PAAN_LIQUID_CODE", "", 80));
                cboPaanLiquid.Properties.Columns.Add(new LookUpColumnInfo("PAAN_LIQUID_NAME", "", 200));
                cboPaanLiquid.Properties.ShowHeader = false;
                cboPaanLiquid.Properties.ImmediatePopup = true;
                cboPaanLiquid.Properties.DropDownRows = 10;
                cboPaanLiquid.Properties.PopupWidth = 280;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void InitComboTestSampleType()
        {
            try
            {
                if ((AppConfig.IntegrationVersionValue == "1" && AppConfig.IntegrationOptionValue != "1") || (AppConfig.IntegrationVersionValue == "2" && AppConfig.IntegrationTypeValue != "1"))
                {
                    lciSampleType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    cboTestSampleType.Properties.DataSource = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    cboTestSampleType.Properties.DisplayMember = "TEST_SAMPLE_TYPE_NAME";
                    cboTestSampleType.Properties.ValueMember = "ID";
                    cboTestSampleType.Properties.ForceInitialize();
                    cboTestSampleType.Properties.Columns.Clear();
                    cboTestSampleType.Properties.Columns.Add(new LookUpColumnInfo("TEST_SAMPLE_TYPE_CODE", "", 80));
                    cboTestSampleType.Properties.Columns.Add(new LookUpColumnInfo("TEST_SAMPLE_TYPE_NAME", "", 200));
                    cboTestSampleType.Properties.ShowHeader = false;
                    cboTestSampleType.Properties.ImmediatePopup = true;
                    cboTestSampleType.Properties.DropDownRows = 10;
                    cboTestSampleType.Properties.PopupWidth = 280;
                }
                else
                {
                    lciSampleType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Size = lciLiquidDate.Size;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void InitComboUsername()
        {
            try
            {
                try
                {
                    cboUsername.Properties.DataSource = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    cboUsername.Properties.DisplayMember = "USERNAME";
                    cboUsername.Properties.ValueMember = "ID";
                    cboUsername.Properties.ForceInitialize();
                    cboUsername.Properties.Columns.Clear();
                    cboUsername.Properties.Columns.Add(new LookUpColumnInfo("LOGINNAME", "", 80));
                    cboUsername.Properties.Columns.Add(new LookUpColumnInfo("USERNAME", "", 140));
                    cboUsername.Properties.ShowHeader = false;
                    cboUsername.Properties.ImmediatePopup = true;
                    cboUsername.Properties.DropDownRows = 10;
                    cboUsername.Properties.PopupWidth = 220;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ResetControlValue()
        {
            try
            {
                this.InitDefaultControl();
                resultSDO = null;
                checkIsSurgery.Checked = false;
                txtPaanServiceTypeCode.Text = "";
                cboPaanServiceType.EditValue = null;
                cboTestSampleType.EditValue = null;
                cboExecuteRoom.EditValue = null;
                spinPrice.Value = 0;
                txtPaanPositionCode.Text = "";
                cboPaanPosition.EditValue = null;
                txtPaanLiquidCode.Text = "";
                cboPaanLiquid.EditValue = null;
                txtDescription.Text = "";
                btnPrint.Enabled = false;
                //btnSave.Enabled = true;
                //btnSavePrint.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitDefaultControl()
        {
            try
            {
                this.InitDefaultIcd();
                if (AppConfig.IsDefaultTracking == "0" && currentTracking != null)
                {
                    dtInstructionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTracking.TRACKING_TIME) ?? DateTime.Now;
                }
                else
                {
                    dtInstructionTime.DateTime = DateTime.Now;
                }
                dtLiquidTime.DateTime = DateTime.Now;
                LoadDataToTrackingCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitDefaultPatientType()
        {
            try
            {
                cboPatientType.EditValue = this.currentPatientTypeAlter != null ? new Nullable<long>(this.currentPatientTypeAlter.PATIENT_TYPE_ID) : null;

                if (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) > DateTime.Now || Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) < DateTime.Now)
                {
                    cboPatientType.EditValue = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;
                }

                Inventec.Common.Logging.LogSystem.Debug("InitDefaultPatientType()_Config.HisPatientTypeCFG.DEFAULT_PATIENT_TYPE_OPTION: " + Config.HisPatientTypeCFG.DEFAULT_PATIENT_TYPE_OPTION);
                Inventec.Common.Logging.LogSystem.Debug("this.serviceReqId: " + this.serviceReqId);
                Inventec.Common.Logging.LogSystem.Debug("this._sereServParentId: " + this._sereServParentId);
                if (Config.HisPatientTypeCFG.DEFAULT_PATIENT_TYPE_OPTION.Trim() == "1" && (this.serviceReqId > 0 || this._sereServParentId > 0))
                {
                    V_HIS_SERE_SERV_5 sereServ = new V_HIS_SERE_SERV_5();
                    CommonParam param = new CommonParam();
                    HisSereServView5Filter filter = new HisSereServView5Filter();
                    if (this._sereServParentId > 0)
                    {
                        filter.ID = this._sereServParentId;
                    }
                    else if (this.serviceReqId > 0)
                    {
                        filter.SERVICE_REQ_ID = this.serviceReqId;
                    }
                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, filter, param);
                    if (result != null && result.Count > 0)
                    {
                        sereServ = result.FirstOrDefault();
                        cboPatientType.EditValue = sereServ.PATIENT_TYPE_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitDefaultIcd()
        {
            try
            {
                HIS.UC.Icd.ADO.IcdInputADO inputIcd = new HIS.UC.Icd.ADO.IcdInputADO();
                inputIcd.ICD_CODE = this.icdAdo != null ? this.icdAdo.ICD_CODE : "";
                inputIcd.ICD_NAME = this.icdAdo != null ? this.icdAdo.ICD_NAME : "";

                if (this.treatment != null)
                {
                    inputIcd.ICD_CODE = this.treatment.ICD_CODE;
                    inputIcd.ICD_NAME = this.treatment.ICD_NAME;
                }
                this.icdProcessor.Reload(ucIcd, inputIcd);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                subIcd.ICD_SUB_CODE = this.secondIcdAdo != null ? this.secondIcdAdo.ICD_SUB_CODE : "";
                subIcd.ICD_TEXT = this.secondIcdAdo != null ? this.secondIcdAdo.ICD_TEXT : "";
                if (this.treatment != null)
                {
                    subIcd.ICD_SUB_CODE = this.treatment.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = this.treatment.ICD_TEXT;
                }
                subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetDataSourceCboPaanServiceType()
        {
            try
            {
                this.HisService = null;
                if (this.hisAllowServicePatys != null && this.hisAllowServicePatys.Count > 0)
                {
                    this.HisService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => this.hisAllowServicePatys.Select(s => s.SERVICE_ID).Distinct().Contains(o.ID)).ToList();
                }
                this.cboPaanServiceType.Properties.DataSource = this.HisService;
                if (this.hisAllowServicePatys != null && this.hisAllowServicePatys.Count == 1)
                {
                    cboPaanServiceType.EditValue = this.hisAllowServicePatys.FirstOrDefault().SERVICE_ID;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetDataSourceCboExecuteRoom()
        {
            try
            {
                this.cboExecuteRoom.Properties.DataSource = this.hisCurrentServiceRooms;
                //this.cboExecuteRoom.Refresh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetDataSourceCboPatientType()
        {
            try
            {
                if (cboPaanServiceType.EditValue == null)
                {
                    this.cboPatientType.Properties.DataSource = this.listPatientTypeAllow;
                }
                else
                {
                    this.cboPatientType.Properties.DataSource = this.listPatientTypeAllowService;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadDefaultUser()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).ToList();
                if (data != null)
                {
                    cboUsername.EditValue = data[0].ID;
                    txtLoginname.Text = data[0].LOGINNAME;
                }

                //Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
                //- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
                //- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
                cboUsername.Enabled = (Config.AppConfig.ShowRequestUser == "1");
                txtLoginname.Enabled = (Config.AppConfig.ShowRequestUser == "1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidControl()
        {
            try
            {
                ValidControlInstructionTime();
                ValidControlPatientType();
                ValidControlPaanServiceType();
                ValidControlExecuteRoom();

                if (AppConfig.IsRequiredTracking && this.currentPatientTypeAlter != null && (this.currentPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.currentPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU))
                {
                    ValidControlCboTracking(true);
                    lciTracking.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }
                else
                {
                    ValidControlCboTracking(false);
                    lciTracking.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlCboTracking(bool isRequied)
        {
            try
            {
                TrackingValidationRule rule = new TrackingValidationRule();
                rule.cboTracking = cboTracking;
                rule.isRequired = isRequied;
                dxValidationProvider1.SetValidationRule(cboTracking, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidControlInstructionTime()
        {
            try
            {
                InstructionTimeValidationRule instructionTimeRule = new InstructionTimeValidationRule();
                instructionTimeRule.dtInstructionTime = dtInstructionTime;
                dxValidationProvider1.SetValidationRule(dtInstructionTime, instructionTimeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidControlPatientType()
        {
            try
            {
                PatientTypeValidationRule patientTypeRule = new PatientTypeValidationRule();
                patientTypeRule.cboPatientType = cboPatientType;
                dxValidationProvider1.SetValidationRule(cboPatientType, patientTypeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidControlPaanServiceType()
        {
            try
            {
                PaanServiceTypeValidationRule serviceRule = new PaanServiceTypeValidationRule();
                serviceRule.txtPaanServiceTypeCode = txtPaanServiceTypeCode;
                serviceRule.cboPaanServiceType = cboPaanServiceType;
                dxValidationProvider1.SetValidationRule(txtPaanServiceTypeCode, serviceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void ValidControlSampleType()
        {
            try
            {
                SampleTypeValidationRule serviceRule = new SampleTypeValidationRule();
                serviceRule.cbo = cboTestSampleType;
                dxValidationProvider1.SetValidationRule(cboTestSampleType, serviceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void ValidControlExecuteRoom()
        {
            try
            {
                ExecuteRoomValidationRule exeRoomRule = new ExecuteRoomValidationRule();
                exeRoomRule.cboExecuteRoom = cboExecuteRoom;
                dxValidationProvider1.SetValidationRule(cboExecuteRoom, exeRoomRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadLanguageKey()
        {
            try
            {
                var lang = Resources.ResourceLangManager.LanguageFrmAssignPaan;
                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                //Button
                btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__BTN_PRINT", lang, cul);
                btnRefersh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__BTN_REFRESH", lang, cul);
                btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__BTN_SAVE", lang, cul);
                btnSavePrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__BTN_SAVE_PRINT", lang, cul);

                //Layout
                lciCboExecuteRoom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_EXECUTE_ROOM", lang, cul);
                lciDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_DESCRIPTION", lang, cul);
                //lciIcdText.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_ICD_TEXT", lang, cul);
                lciInstructionTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_INSTRUCTION_TIME", lang, cul);
                lciIsSurgery.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_IS_SURGERY", lang, cul);
                lciLiquidDate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_LIQUID_TIME", lang, cul);
                lciPaanLiquid.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_PAAN_LIQUID", lang, cul);
                lciPaanPosition.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_PAAN_POSITION", lang, cul);
                lciPaanServiceType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_PAAN_SERVICE_TYPE", lang, cul);
                lciPatientType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_PATIENT_TYPE", lang, cul);
                lciPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ASSIGN_PAAN__LAYOUT_PRICE", lang, cul);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestSampleType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboTestSampleType.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
