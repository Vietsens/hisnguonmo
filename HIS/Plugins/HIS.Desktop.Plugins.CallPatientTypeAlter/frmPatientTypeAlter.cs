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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using His.UC.UCHein;
using His.UC.UCHein.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CallPatientTypeAlter.Config;
using HIS.Desktop.Plugins.CallPatientTypeAlter.Loader;
using HIS.Desktop.Plugins.CallPatientTypeAlter.Resources;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Common.QrCodeCCCD;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    public partial class frmPatientTypeAlter : HIS.Desktop.Utility.FormBase
    {
        public PatientTypeDepartmentADO currentTreatmentLogSDO;
        internal Inventec.Desktop.Common.Modules.Module module;
        MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4 currentHisTreatment = new V_HIS_TREATMENT_4();
        RefeshReference RefeshReference;
        long treatmentId = 0;
        MainHisHeinBhyt uCMainHein;
        UserControl ucHein__BHYT = new UserControl();
        ResultDataADO ResultDataADO { get; set; }
        int ActionType = 0;
        bool? IsView { get; set; }
        int positionHandleControl = -1;
        string provindcode = null;
        HIS_PATIENT_TYPE_ALTER resultApi = null;
        HisPatientTypeAlterAndTranPatiSDO resultPatientTypeAlter = null;
        public const int ActionAdd = 1;//1 -> Add
        public const int ActionEdit = 2;//2 -> Edit
        public const int ActionView = 3;//3 -> View
        public const int ActionViewForEdit = 4;//4 -> View for edit
        List<PatientTypeDepartmentADO> lstTreatmentLog = null;
        List<V_HIS_SERE_SERV_4> lstSereServResult = null;
        public PatientTypeDepartmentADO currentTreatmentSave;
        UC_KskContract ucKskContract;
        UC_ImageBHYT ucImageBHYT;
        long patientId;
        long keyIsSetPrimaryPatientType;
        List<MOS.EFMODEL.DataModels.HIS_POSITION> dataPosition = null;
        List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> dataWorkPlace = null;
        List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK> dataMilitaryRank = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY> dataClassify = null;
        HIS_PATIENT currenPatient = null;

        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = null;
        //tientv #8559
        bool _IsShowLsKcb { get; set; }

        bool resultSuccess = false;
        bool isEdit = false;
        bool IsLoadForm;


        List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL> currentHideControls;
        string APP_CODE__EXACT = "HIS";
        List<string> currentNameControl = new List<string>();
        MOS.EFMODEL.DataModels.HIS_BRANCH branch;
        string baseNameControl = "";
        bool IsVisibleClassify = false;

        List<HIS_PATIENT_TYPE> primaryPatientTypes = new List<HIS_PATIENT_TYPE>();
        bool IsHasEmergency = false;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string ModuleLinkName = "HIS.Desktop.Plugins.CallPatientTypeAlter";
        Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>> dicSevicepatyAllows = new Dictionary<long, List<V_HIS_SERVICE_PATY>>();
        string MesError = null;
        List<V_HIS_SERE_SERV_4> newLstSerSev = new List<V_HIS_SERE_SERV_4>();
        List<HIS_SERE_SERV> lstSereServ = new List<HIS_SERE_SERV>();
        private V_HIS_ROOM currentWorkingRoom;
        bool IsFirstLoadClassify;

        public frmPatientTypeAlter(Inventec.Desktop.Common.Modules.Module _module, PatientTypeDepartmentADO _HisTreatmentLogSDO, bool? _isView, List<PatientTypeDepartmentADO> _lstTreatmentLog, RefeshReference _RefeshReference)
            : base(_module)
        {
            InitializeComponent();
            this.module = _module;
            this.isEdit = true;
            this.currentTreatmentLogSDO = _HisTreatmentLogSDO;
            this.IsView = _isView;
            this.lstTreatmentLog = _lstTreatmentLog;
            this.RefeshReference = _RefeshReference;
            if (currentTreatmentLogSDO != null)
                treatmentId = currentTreatmentLogSDO.patientTypeAlter.TREATMENT_ID;
        }

        public frmPatientTypeAlter(Inventec.Desktop.Common.Modules.Module _module, long _treatmentId, bool? _isView, List<PatientTypeDepartmentADO> _lstTreatmentLog, RefeshReference _RefeshReference)
            : base(_module)
        {
            InitializeComponent();
            this.module = _module;
            this.treatmentId = _treatmentId;
            this.IsView = _isView;
            this.lstTreatmentLog = _lstTreatmentLog;
            this.RefeshReference = _RefeshReference;
        }

        private void frmPatientTypeAlter_Load(object sender, EventArgs e)
        {
            try
            {
                this.keyIsSetPrimaryPatientType = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__MOS_HIS_SERE_SERV_IS_SET_PRIMARY_PATIENT_TYPE));
                if (this.module != null)
                {
                    currentWorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.module.RoomId);
                }
                IsLoadForm = true;
                IsFirstLoadClassify = true;
                SetIcon();
                btnSave.Enabled = IsView ?? true;
                txtPatientType.Enabled = IsView ?? true;
                cboPatientType.Enabled = IsView ?? true;
                txtTreatmentTypeCode.Enabled = IsView ?? true;
                cboTreatmentType.Enabled = IsView ?? true;
                dtLogTime.Enabled = IsView ?? true;
                WaitingManager.Show();
                GetDataVisibleControlFromPatientClassify();
                ValidControl();
                InitCombo();
                VisibleControl();
                dtLogTime.Properties.VistaDisplayMode = DefaultBoolean.True;
                dtLogTime.Properties.VistaEditTime = DefaultBoolean.True;
                HisConfigCFG.LoadConfig();
                HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.LoadConfig();
                VisiblePrimaryPatientType();
                HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.LoadConfig();
                LoadCurrentHisTreatment();
                SetCaptionByLanguageKey();
                if (this.currentTreatmentLogSDO != null)
                {
                    ActionType = ActionEdit;
                    FillDataPatientTypeAlterIntoForm(ref patientType);
                }
                else
                {
                    ActionType = ActionAdd;
                    dtLogTime.DateTime = DateTime.Now;
                    dtLogTime.Update();
                    txtTreatmentTypeCode.Text = "";
                    cboTreatmentType.EditValue = null;
                    GetPatientTypeDefault(ref patientType);
                }
                WaitingManager.Hide();

                InitPatientTypeInfo(patientType);
                cboTreatmentType_EditValueChanged(null, null);
                InitDataCombo();
                currentTreatmentSave = currentTreatmentLogSDO;
                dtLogTime.Focus();
                dtLogTime.SelectAll();
                this._IsShowLsKcb = false;
                Config.BHXHLoginCFG.LoadConfig();
                HIS.Desktop.Plugins.Library.RegisterConfig.BHXHLoginCFG.LoadConfig();
                LoadCurrentPatient();
                InitControlState();
                LoadServiceReq();
                EnableBtnPrint();
                this.ResultDataADO = new ResultDataADO();


            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmPatientTypeAlter
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientTypeAlter.Resources.Lang", typeof(frmPatientTypeAlter).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrint.ToolTip = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.btnPrint.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkAutoUpdateType.Properties.Caption = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.chkAutoUpdateType.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkAutoUpdateType.ToolTip = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.chkAutoUpdateType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtWorkplace.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.txtWorkplace.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboWorkPlace.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.cboWorkPlace.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboPosition.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.cboPosition.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboMilitaryRank.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.cboMilitaryRank.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboClassify.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.cboClassify.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboPrimaryPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.cboPrimaryPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtQrcode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.txtQrcode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciQrcode.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.lciQrcode.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPrimaryPatientType.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.lciPrimaryPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem9.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem9.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void GetDataVisibleControlFromPatientClassify()
        {
            try
            {

                var branchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>();
                this.branch = (branchs != null && branchs.Count > 0) ? branchs.FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId()) : null;


                CommonParam paramCommon = new CommonParam();
                SdaHideControlFilter filter = new SdaHideControlFilter();
                filter.MODULE_LINK__EXACT = ModuleLinkName;
                filter.APP_CODE__EXACT = APP_CODE__EXACT;
                this.currentHideControls = new BackendAdapter(paramCommon).Get<List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL>>("api/sdaHideControl/Get", ApiConsumers.SdaConsumer, filter, paramCommon);

                this.currentHideControls = (this.currentHideControls != null && this.currentHideControls.Count > 0) ? this.currentHideControls.Where(o => o.BRANCH_CODE == null || (branch != null && o.BRANCH_CODE == branch.BRANCH_CODE)).ToList() : null;

                if (this.currentHideControls != null && this.currentHideControls.Count > 0)
                {
                    currentNameControl = this.currentHideControls.Select(o => o.CONTROL_PATH).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentPatient()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.PATIENT_CODE = currentHisTreatment.TDL_PATIENT_CODE;
                currenPatient = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param).ToList().First();
                if (this.currentHisTreatment != null && !IsVisibleClassify)
                {
                    if (dataClassify.FirstOrDefault(o => o.ID == currenPatient.PATIENT_CLASSIFY_ID) != null)
                    {
                        cboClassify.EditValue = currenPatient.PATIENT_CLASSIFY_ID;
                        if (layoutControlItem10.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            cboMilitaryRank.EditValue = currenPatient.MILITARY_RANK_ID;
                            cboPosition.EditValue = currenPatient.POSITION_ID;
                            cboWorkPlace.EditValue = currenPatient.WORK_PLACE_ID;
                            txtWorkplace.Text = currenPatient.WORK_PLACE;

                        }
                    }
                }

                ReSizeForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadServiceReq()
        {
            try
            {
                if (this.currentHisTreatment != null)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqFilter serFilter = new HisServiceReqFilter();
                    serFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                    var serviceReqLog = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serFilter, param);
                    if (serviceReqLog != null && serviceReqLog.Count > 0)
                        IsHasEmergency = serviceReqLog.Where(o => o.IS_EMERGENCY == (short?)1).ToList() != null && serviceReqLog.Where(o => o.IS_EMERGENCY == (short?)1).ToList().Count > 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReSizeForm()
        {
            try
            {
                if (emptySpaceItem3.Visibility == LayoutVisibility.Never)
                {
                    this.Size = new Size(1300, 120);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisibleControl()
        {
            try
            {
                if (IsVisibleClassify)
                {
                    layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlCombo(bool IsEnable)
        {
            try
            {
                layoutControlItem10.Visibility = IsEnable ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem11.Visibility = IsEnable ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem12.Visibility = IsEnable ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem13.Visibility = IsEnable ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem2.Visibility = IsEnable ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitDataCombo()
        {
            try
            {
                LoadPatientClassify();
                LoadMilitaryRank();
                LoadPosition();
                LoadWorkPlace();
                InitComboCommon(this.cboClassify, dataClassify, "ID", "PATIENT_CLASSIFY_NAME", "PATIENT_CLASSIFY_CODE");
                InitComboCommon(this.cboMilitaryRank, dataMilitaryRank, "ID", "MILITARY_RANK_NAME", "MILITARY_RANK_CODE");
                InitComboCommon(this.cboPosition, dataPosition, "ID", "POSITION_NAME", "POSITION_CODE");
                InitComboCommon(this.cboWorkPlace, dataWorkPlace, "ID", "WORK_PLACE_NAME", "WORK_PLACE_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientClassify()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>())
                {
                    Inventec.Common.Logging.LogSystem.Error("1__________________");
                    dataClassify = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>();
                }

                else
                {
                    Inventec.Common.Logging.LogSystem.Error("2__________________");
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataClassify = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>>("api/HisPatientClassify/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataClassify != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY), dataClassify, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataClassify), dataClassify));

                if (dataClassify != null && dataClassify.Count > 0)
                {
                    if (cboPatientType.EditValue != null)
                    {
                        dataClassify = dataClassify.Where(o => o.IS_ACTIVE == 1 && (o.PATIENT_TYPE_ID == null || o.PATIENT_TYPE_ID == Int64.Parse(cboPatientType.EditValue.ToString()))).ToList();
                    }
                    else
                    {
                        dataClassify = dataClassify.Where(o => o.IS_ACTIVE == 1 && (o.PATIENT_TYPE_ID == null)).ToList();
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataClassify), dataClassify));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMilitaryRank()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>())
                {
                    dataMilitaryRank = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataMilitaryRank = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>>("api/HisMilitaryRank/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataMilitaryRank != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK), dataMilitaryRank, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (dataMilitaryRank != null && dataMilitaryRank.Count > 0)
                {
                    dataMilitaryRank = dataMilitaryRank.Where(o => o.IS_ACTIVE == 1).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPosition()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_POSITION>())
                {
                    dataPosition = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_POSITION>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataPosition = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_POSITION>>("api/HisPosition/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataPosition != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_POSITION), dataPosition, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (dataPosition != null && dataPosition.Count > 0)
                {
                    dataPosition = dataPosition.Where(o => o.IS_ACTIVE == 1).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadWorkPlace()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>())
                {
                    dataWorkPlace = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataWorkPlace = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>>("api/HisWorkPlace/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataWorkPlace != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_WORK_PLACE), dataWorkPlace, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (dataWorkPlace != null && dataWorkPlace.Count > 0)
                {
                    dataWorkPlace = dataWorkPlace.Where(o => o.IS_ACTIVE == 1).ToList();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisiblePrimaryPatientType()
        {
            try
            {
                if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                {
                    lciPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciComboPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciComboPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCombo()
        {
            try
            {
                PatientTypeLoader.LoadDataToCombo(this.cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1 && p.IS_NOT_USE_FOR_PATIENT != 1).ToList());
                TreatmentTypeLoader.LoadDataToComboTreatmentType(this.cboTreatmentType, BackendDataWorker.Get<HIS_TREATMENT_TYPE>());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitPatientTypeInfo(HIS_PATIENT_TYPE patientType)
        {
            try
            {
                if (patientType == null)
                    return;
                lciQrcode.Enabled = false;
                cboPatientType.EditValue = patientType.ID;
                txtPatientType.Text = patientType.PATIENT_TYPE_CODE;
                if (patientType.ID == HisConfigCFG.PatientTypeId__KSK)
                {
                    xclHeinCardInformation.Controls.Clear();
                    xclHeinCardInformation.Update();
                    xclHeinCardInformation.Enabled = true;

                    emptySpaceItem3.Visibility = LayoutVisibility.Never;
                    layoutControlItem8.Visibility = LayoutVisibility.Never;
                    btnSave.Size = new Size(110, btnSave.Height);
                    this.Size = new Size(this.Width, 130);

                    ucKskContract = new UC_KskContract();
                    ucKskContract.cboContract.EditValue = this.currentTreatmentLogSDO.patientTypeAlter.KSK_CONTRACT_ID;
                    ucKskContract.Dock = DockStyle.Fill;
                    xclHeinCardInformation.Controls.Add(ucKskContract);
                    if (ucKskContract != null && ucKskContract.cboContract.EditValue != null)
                    {
                        var contract = ucKskContract.listKskContract.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(ucKskContract.cboContract.EditValue.ToString()));
                        if (contract != null)
                        {
                            ucKskContract.lblNgayHetHan.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(contract.EXPIRY_DATE ?? 0);
                            ucKskContract.lblNgayHieuLuc.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(contract.EFFECT_DATE ?? 0);
                            ucKskContract.lblTenCongTy.Text = contract.WORK_PLACE_NAME;
                            ucKskContract.lblTyLeThanhToan.Text = Convert.ToInt64(contract.PAYMENT_RATIO * 100).ToString() + "%";
                        }
                    }
                }
                else
                {
                    ChoiceTemplateHeinCard(patientType.PATIENT_TYPE_CODE, false);
                    if (patientType.ID == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        lciQrcode.Enabled = true;
                    }
                }

                if (ActionType == ActionEdit)
                {
                    HIS_PATIENT_TYPE_ALTER patientTypeAlterSDO = new HIS_PATIENT_TYPE_ALTER();
                    patientTypeAlterSDO.PATIENT_TYPE_ID = patientType.ID;
                    patientTypeAlterSDO.ID = this.currentTreatmentLogSDO.patientTypeAlter.ID;
                    patientTypeAlterSDO.TDL_PATIENT_ID = this.currentTreatmentLogSDO.patientTypeAlter.TDL_PATIENT_ID;
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER, HIS_PATIENT_TYPE_ALTER>();
                    patientTypeAlterSDO = AutoMapper.Mapper.Map<V_HIS_PATIENT_TYPE_ALTER, HIS_PATIENT_TYPE_ALTER>(currentTreatmentLogSDO.patientTypeAlter);
                    if (ucHein__BHYT != null && uCMainHein != null)
                    {
                        //Refesh and fill data
                        uCMainHein.FillDataHeinInsuranceInfoByPatientTypeAlter(ucHein__BHYT, patientTypeAlterSDO);
                        uCMainHein.InitOldPatientData(ucHein__BHYT, currentTreatmentLogSDO.patientTypeAlter.TDL_PATIENT_ID, patientTypeAlterSDO.HEIN_CARD_NUMBER);
                        //Lay du lieu chuyen tuyen sau do do du lieu vao form
                        uCMainHein.FillDataTranPatiInForm(ucHein__BHYT, this.currentHisTreatment.ID);
                    }

                    if (this.currentTreatmentLogSDO.patientTypeAlter != null)
                    {
                        if (!string.IsNullOrEmpty(this.currentTreatmentLogSDO.patientTypeAlter.BHYT_URL))
                        {
                            try
                            {
                                ucImageBHYT.pictureEditImageBHYT.Image = Image.FromStream(
                                    Inventec.Fss.Client.FileDownload.GetFile(this.currentTreatmentLogSDO.patientTypeAlter.BHYT_URL));
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(ex);
                                ucImageBHYT.SetImageDefaultForPictureEdit(null);
                            }
                        }
                    }

                }
                LoadPrimaryPatientType();

                if (this.currentTreatmentLogSDO != null)
                {
                    if (this.currentTreatmentLogSDO.patientTypeAlter.PRIMARY_PATIENT_TYPE_ID != null && this.currentTreatmentLogSDO.patientTypeAlter != null)
                    {

                        cboPrimaryPatientType.EditValue = this.currentTreatmentLogSDO.patientTypeAlter.PRIMARY_PATIENT_TYPE_ID;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetPatientTypeDefault(ref HIS_PATIENT_TYPE patientType)
        {
            try
            {
                if (!String.IsNullOrEmpty(ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE)))
                {
                    patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE));
                    if (patientType == null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Phan mem RAE da duoc cau hinh doi tuong benh nhan mac dinh, tuy nhien ma doi tuong cau hinh khong ton tai trong danh muc doi tuong benh nhan, he thong tu dong lay doi tuong mac dinh la doi tuong BHYT. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE)), AppConfigKeys.CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE));
                        patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == HisConfigCFG.PatientTypeId__BHYT);
                    }
                }
                else
                {
                    patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == HisConfigCFG.PatientTypeId__BHYT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeyLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientTypeAlter.Resources.Lang", typeof(HIS.Desktop.Plugins.CallPatientTypeAlter.frmPatientTypeAlter).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                txtQrcode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmPatientTypeAlter.txtQrcode.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                if (this.module != null)
                {
                    this.Text = module.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentHisTreatment()
        {
            try
            {
                if (this.module != null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                    filter.ID = treatmentId;
                    this.currentHisTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumers.MosConsumer, filter, param).ToList().First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataPatientTypeAlterIntoForm(ref MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType)
        {
            try
            {
                dtLogTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentTreatmentLogSDO.LOG_TIME) ?? DateTime.Now;
                dtLogTime.Update();
                dtLogTime.Visible = false;
                //txtLogTime.Text = dtLogTime.DateTime.ToString("dd/MM/yyyy HH:mm");
                if (this.currentTreatmentLogSDO.patientTypeAlter == null)
                    return;
                patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == this.currentTreatmentLogSDO.patientTypeAlter.PATIENT_TYPE_ID);
                if (patientType == null) Inventec.Common.Logging.LogSystem.Debug("Khong lay duoc doi tuong benh nhan theo PATIENT_TYPE_ID. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentTreatmentLogSDO.patientTypeAlter.PATIENT_TYPE_ID), this.currentTreatmentLogSDO.patientTypeAlter.PATIENT_TYPE_ID));

                txtTreatmentTypeCode.Text = this.currentTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_CODE;
                cboTreatmentType.EditValue = this.currentTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTreatmentType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE data = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().SingleOrDefault(o => o.ID == long.Parse((cboTreatmentType.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtTreatmentTypeCode.Text = data.TREATMENT_TYPE_CODE;
                        }
                    }
                    txtPatientType.Focus();
                    txtPatientType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridSereServ(PatientTypeDepartmentADO data, List<PatientTypeDepartmentADO> listTL)
        {
            try
            {
                WaitingManager.Show();
                if (data != null && data.type == 1 && listTL != null && listTL.Count > 0)
                {
                    listTL = listTL.OrderBy(o => o.LOG_TIME).ToList();

                    PatientTypeDepartmentADO logtime = new PatientTypeDepartmentADO();
                    for (int i = 0; i < listTL.Count; i++)
                    {
                        if (listTL[i].type == 1 && listTL[i].LOG_TIME != data.LOG_TIME && listTL[i].LOG_TIME > data.LOG_TIME)
                        {
                            logtime = listTL[i];
                            break;
                        }
                    }

                    CommonParam param = new CommonParam();


                    //Dich vu da duoc thanh toan
                    HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                    sereServBillFilter.TDL_TREATMENT_ID = data.TREATMENT_ID;
                    sereServBillFilter.IS_NOT_CANCEL = true;
                    var lstSereServBill = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, sereServBillFilter, param);

                    MOS.Filter.HisSereServView4Filter hisSerwServFilter = new HisSereServView4Filter();
                    hisSerwServFilter.TREATMENT_ID = data.TREATMENT_ID;
                    List<V_HIS_SERE_SERV_4> lstHisSereServWithTreatment = new List<V_HIS_SERE_SERV_4>();
                    lstHisSereServWithTreatment = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_4>>("api/HisSereServ/GetView4", ApiConsumers.MosConsumer, hisSerwServFilter, param);

                    if (lstHisSereServWithTreatment != null && lstHisSereServWithTreatment.Count > 0)
                    {
                        //Loc ra dich vu chua duoc thanh toan thi hien thi len grid
                        //var lstSereServHasTT = new List<V_HIS_SERE_SERV_4>();
                        if (lstSereServBill != null && lstSereServBill.Count > 0)
                        {
                            lstHisSereServWithTreatment = lstHisSereServWithTreatment.Where(o => !lstSereServBill.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                        }

                        lstSereServResult = new List<V_HIS_SERE_SERV_4>();
                        if (lstHisSereServWithTreatment != null && lstHisSereServWithTreatment.Count > 0)
                        {

                            //Không có đối tượng tiếp theo thì lấy hết serv sere dịch vụ từ thời điểm đó đến hết
                            if (logtime != null)
                            {
                                //Issue 8197
                                if (logtime.LOG_TIME > 0)
                                {
                                    lstSereServResult = lstHisSereServWithTreatment.
                                         Where(o => (o.PATIENT_TYPE_ID != Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()))
                                        && (data.LOG_TIME <= o.TDL_INTRUCTION_TIME && o.TDL_INTRUCTION_TIME <= logtime.LOG_TIME)).ToList();
                                }
                                else
                                {
                                    lstSereServResult = lstHisSereServWithTreatment.
                                         Where(o => (o.PATIENT_TYPE_ID != Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()))
                                        && (data.LOG_TIME <= o.TDL_INTRUCTION_TIME)).ToList();
                                }
                            }
                            else
                            {
                                lstSereServResult = lstHisSereServWithTreatment.
                                         Where(o => (o.PATIENT_TYPE_ID != Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()))
                                        && (data.LOG_TIME <= o.TDL_INTRUCTION_TIME)).ToList();
                            }

                        }

                        if (lstSereServResult.Count > 0)
                        {
                            data.patientTypeAlter.TREATMENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "0").ToString());
                            data.patientTypeAlter.TREATMENT_TYPE_CODE = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == data.patientTypeAlter.TREATMENT_TYPE_ID).TREATMENT_TYPE_CODE;
                            data.patientTypeAlter.TREATMENT_TYPE_NAME = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == data.patientTypeAlter.TREATMENT_TYPE_ID).TREATMENT_TYPE_NAME;
                            HisPatientProfileSDO patientTypeAlterSDO = new HisPatientProfileSDO();
                            if (this.uCMainHein != null && ucHein__BHYT != null)
                            {
                                uCMainHein.UpdateDataFormIntoPatientTypeAlter(ucHein__BHYT, patientTypeAlterSDO);
                                data.patientTypeAlter.HEIN_CARD_FROM_TIME = patientTypeAlterSDO.HisPatientTypeAlter.HEIN_CARD_FROM_TIME;
                                data.patientTypeAlter.HEIN_CARD_TO_TIME = patientTypeAlterSDO.HisPatientTypeAlter.HEIN_CARD_TO_TIME;
                            }
                            long? primaryKeyPatientType = null;
                            if (cboPrimaryPatientType.EditValue != null && !string.IsNullOrEmpty(cboPrimaryPatientType.EditValue.ToString()))
                            {
                                primaryKeyPatientType = Inventec.Common.TypeConvert.Parse.ToInt64(cboPrimaryPatientType.EditValue.ToString());
                            }
                            long? patient_classify_id = null;
                            if (cboClassify.EditValue != null && !string.IsNullOrEmpty(cboClassify.EditValue.ToString()))
                            {
                                patient_classify_id = Inventec.Common.TypeConvert.Parse.ToInt64(cboClassify.EditValue.ToString());
                            }
                            HIS.Desktop.Plugins.CallPatientTypeAlter.frmSwapPatientTypeAlter frm = new frmSwapPatientTypeAlter(this.module, Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "").ToString()), primaryKeyPatientType, patient_classify_id, data,
listTL, lstSereServResult, DelegateSuccess);
                            frm.ShowDialog();
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DelegateSuccess(bool success)
        {
            try
            {
                if (success)
                {
                    resultSuccess = success;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    this.lciQrcode.Enabled = false;
                    if (Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()) == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        uCMainHein.DefaultFocusUserControl(ucHein__BHYT);
                        this.lciQrcode.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrepareForm(bool? isView)
        {
            try
            {
                if (isView.HasValue)
                {
                    txtTreatmentTypeCode.Enabled = cboTreatmentType.Enabled = txtPatientType.Enabled = cboPatientType.Enabled = xclHeinCardInformation.Enabled = !isView.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool check = false;
            bool valid = true;
            bool validKsk = true;
            bool validPatientInfo = true;
            bool validBlockBhyt = true;
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                {
                    IList<Control> invalidControls = dxValidationProvider1.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    validPatientInfo = false;

                }
                MesError = null;
                newLstSerSev = new List<V_HIS_SERE_SERV_4>();
                if (currentTreatmentLogSDO != null)
                {
                    var patientTypeOld = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == currentTreatmentLogSDO.patientTypeAlter.PATIENT_TYPE_ID);
                    var patientTypeNew = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()));
                    if (patientTypeOld != null && patientTypeNew != null && patientTypeOld.IS_COPAYMENT != 1 && patientTypeNew.IS_COPAYMENT == 1)
                    {
                        check = true;
                    }
                }
                if (uCMainHein != null && ucHein__BHYT != null && (Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()) == HisConfigCFG.PatientTypeId__BHYT || Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()) == HisConfigCFG.PatientTypeId__QN))
                {
                    valid = uCMainHein.GetInvalidControls(ucHein__BHYT);
                }
                valid = valid && this.AlertExpriedTimeHeinCardBhyt();
                validBlockBhyt = this.BlockingInvalidBhyt();

                if (validKsk && valid && validPatientInfo && validBlockBhyt)
                {
                    SaveProcess(param, check);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private bool BlockingInvalidBhyt()
        {
            try
            {
                HisPatientProfileSDO dataPatientProfile = new HisPatientProfileSDO();
                dataPatientProfile.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();
                if (uCMainHein != null)
                    //Đồng bộ dữ liệu thay đổi từ uchein sang đối tượng dữ liệu phục vụ làm đầu vào cho gọi api
                    this.uCMainHein.UpdateDataFormIntoPatientTypeAlter(this.ucHein__BHYT, dataPatientProfile);
                if (this.cboPatientType.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64((this.cboPatientType.EditValue ?? "0").ToString()) == HisConfigCFG.PatientTypeId__BHYT)
                {
                    if (dataPatientProfile != null && !string.IsNullOrEmpty(dataPatientProfile.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE))
                    {
                        var mediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == dataPatientProfile.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE);
                        if (!string.IsNullOrEmpty(BranchDataWorker.Branch.DO_NOT_ALLOW_HEIN_LEVEL_CODE) && mediOrg != null && (";" + BranchDataWorker.Branch.DO_NOT_ALLOW_HEIN_LEVEL_CODE + ";").Contains(";" + mediOrg.LEVEL_CODE + ";"))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(
                                        String.Format("Nơi đăng ký khám chữa bệnh ban đầu thuộc tuyến {0}, không được hưởng BHYT", mediOrg.LEVEL_CODE == "1" ? "trung ương" : (mediOrg.LEVEL_CODE == "2" ? "Tỉnh" : (mediOrg.LEVEL_CODE == "3" ? "Huyện" : "Xã"))),
                                        ResourceMessage.ThongBao,
                                        MessageBoxButtons.OK);
                            return false;
                        }
                    }
                    if (this.ResultDataADO != null && ResultDataADO.ResultHistoryLDO != null && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.WarningInvalidCheckHistoryHeinCard && ResultDataADO.ResultHistoryLDO.message == "Thẻ BHYT có thông tin kiểm tra thẻ chưa ra viện.")
                    {
                        DialogResult drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(ResultDataADO.ResultHistoryLDO.message + " Bạn có muốn tiếp tục?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo);
                        if (drReslt == DialogResult.No)
                        {
                            return false;
                        }
                    }
                    if (dataPatientProfile != null && dataPatientProfile.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE != MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                    {
                        if (this.ResultDataADO != null && ResultDataADO.ResultHistoryLDO != null)
                        {
                            if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsBlockingInvalidBhyt == ((int)HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.OptionKey.Option2).ToString()
                                    && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MaKetQuaBlockings.Contains(ResultDataADO.ResultHistoryLDO.maKetQua))//mã lỗi nằm trong các mã lỗi thẻ hết hạn
                            {
                                Inventec.Common.Logging.LogSystem.Info("maKetQua: " + ResultDataADO.ResultHistoryLDO.maKetQua);
                                XtraMessageBox.Show(ResourceMessage.TheBhytKhongHopLeKhongChoPhepDangKy);
                                return false;
                            }
                            else if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsBlockingInvalidBhyt != ((int)HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.OptionKey.Option2).ToString()
                                && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MaKetQuaBlockings.Contains(ResultDataADO.ResultHistoryLDO.maKetQua))//mã lỗi nằm trong các mã lỗi thẻ hết hạn
                            {
                                Inventec.Common.Logging.LogSystem.Info("maKetQua: " + ResultDataADO.ResultHistoryLDO.maKetQua);
                                DialogResult drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TheBhytKhongHopLeBanCoMuonSuDung, ResourceMessage.ThongBao,
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                                if (drReslt == DialogResult.OK)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }

            return true;
        }

        private bool AlertExpriedTimeHeinCardBhyt()
        {
            bool valid = false;
            long resultDayAlert = -1;
            try
            {
                if (this.cboPatientType.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64((this.cboPatientType.EditValue ?? "0").ToString()) == HisConfigCFG.PatientTypeId__BHYT)
                {
                    if (this.uCMainHein != null && this.ucHein__BHYT != null)
                    {
                        resultDayAlert = this.uCMainHein.AlertExpriedTimeHeinCardBhyt(this.ucHein__BHYT, ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__ALERT_EXPRIED_TIME_HEIN_CARD_BHYT), ref resultDayAlert);
                    }

                    if (resultDayAlert > -1)
                    {
                        if (MessageBox.Show(String.Format(ResourceMessage.TheSapHetHan, resultDayAlert), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            valid = true;
                        }
                        else
                            valid = false;
                    }
                    else
                        valid = true;
                }
                else
                    valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            try
            {
                MemoryStream memory = new MemoryStream();
                var bitMap = new System.Drawing.Bitmap(imageIn);
                bitMap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                return memory.ToArray();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private void SaveProcess(CommonParam param, bool check)
        {
            bool success = false;
            this.patientId = 0;
            resultApi = null;
            resultPatientTypeAlter = null;
            HisPatientTypeAlterAndTranPatiSDO patientTypeAlterAndTranPati = new HisPatientTypeAlterAndTranPatiSDO();
            patientTypeAlterAndTranPati.PatientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
            try
            {
                WaitingManager.Show();

                if (ActionType == ActionAdd)
                {
                    this.currentTreatmentLogSDO = new PatientTypeDepartmentADO();
                    this.currentTreatmentLogSDO.patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    this.currentTreatmentLogSDO.patientTypeAlter.LOG_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    this.currentTreatmentLogSDO.patientTypeAlter.TREATMENT_ID = this.currentHisTreatment.ID;
                }
                this.currentTreatmentLogSDO.patientTypeAlter.TDL_PATIENT_ID = this.currentHisTreatment.PATIENT_ID;
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_TYPE_ALTER>(patientTypeAlterAndTranPati.PatientTypeAlter, this.currentTreatmentLogSDO.patientTypeAlter);
                UpdatePatientTypeAlterFromDataForm(ref patientTypeAlterAndTranPati);


                if (ucKskContract != null && patientTypeAlterAndTranPati.PatientTypeAlter != null && patientTypeAlterAndTranPati.PatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__KSK)
                {
                    if (ucKskContract.cboContract.EditValue != null)
                        patientTypeAlterAndTranPati.PatientTypeAlter.KSK_CONTRACT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(ucKskContract.cboContract.EditValue.ToString());
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DoiTuongKhamSucKhoe, ResourceMessage.ThongBao);

                        return;
                    }
                }

                if (patientTypeAlterAndTranPati.PatientTypeAlter != null
                    && patientTypeAlterAndTranPati.PatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                    && (
                    IsChild() ||
                     patientTypeAlterAndTranPati.PatientTypeAlter.HAS_BIRTH_CERTIFICATE
                     == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE
                     )
                    && (String.IsNullOrEmpty(currentHisTreatment.TDL_PATIENT_DISTRICT_CODE)
                    || String.IsNullOrEmpty(currentHisTreatment.TDL_PATIENT_PROVINCE_CODE)
                    )
                    )
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BNTreEmCanNhapDuTT, ResourceMessage.ThongBao);
                    return;
                }



                if (patientTypeAlterAndTranPati.PatientTypeAlter != null && patientTypeAlterAndTranPati.PatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    bool isSync = true;
                    HisPatientTypeAlterFilter patiAlterFilter = new HisPatientTypeAlterFilter();
                    patiAlterFilter.TREATMENT_ID = patientTypeAlterAndTranPati.PatientTypeAlter.TREATMENT_ID;
                    patiAlterFilter.PATIENT_TYPE_ID = patientTypeAlterAndTranPati.PatientTypeAlter.PATIENT_TYPE_ID;
                    patiAlterFilter.HEIN_CARD_NUMBER__EXACT = patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_CARD_NUMBER;
                    patiAlterFilter.ID__NOT_EQUAL = patientTypeAlterAndTranPati.PatientTypeAlter.ID;

                    var result = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, patiAlterFilter, param);
                    if (result != null && result.Count() > 0)
                    {
                        foreach (var item in result)
                        {
                            isSync = isSync &&

                                ((patientTypeAlterAndTranPati.PatientTypeAlter.RIGHT_ROUTE_CODE ?? "") == (item.RIGHT_ROUTE_CODE ?? "") &&
                                (patientTypeAlterAndTranPati.PatientTypeAlter.RIGHT_ROUTE_TYPE_CODE ?? "") == (item.RIGHT_ROUTE_TYPE_CODE ?? "") &&
                                (patientTypeAlterAndTranPati.PatientTypeAlter.LIVE_AREA_CODE ?? "") == (item.LIVE_AREA_CODE ?? "") &&
                                (patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_MEDI_ORG_CODE ?? "") == (item.HEIN_MEDI_ORG_CODE ?? "") &&
                                (patientTypeAlterAndTranPati.PatientTypeAlter.JOIN_5_YEAR ?? "") == (item.JOIN_5_YEAR ?? "") &&
                                (patientTypeAlterAndTranPati.PatientTypeAlter.PAID_6_MONTH ?? "") == (item.PAID_6_MONTH ?? ""));
                        }
                    }

                    if (!isSync)
                    {
                        WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BNCoTTDienDoiTuong, ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }
                }

                if (ucImageBHYT != null && ucImageBHYT.pictureEditImageBHYT.Tag != "NoImage" && ucImageBHYT.pictureEditImageBHYT.Image != null)
                    patientTypeAlterAndTranPati.ImgBhytData = ImageToByteArray(ucImageBHYT.pictureEditImageBHYT.Image);

                if (patientTypeAlterAndTranPati.ImgBhytData == null || patientTypeAlterAndTranPati.ImgBhytData.Count() == 0)
                    patientTypeAlterAndTranPati.PatientTypeAlter.BHYT_URL = null;

                //issue 14637
                if (!IsUnusedHeinCardNumberByAnother(patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlterAndTranPati.PatientTypeAlter.TDL_PATIENT_ID))
                {
                    return;
                }
                else
                {
                    //14775
                    if (this.patientId > 0)
                        patientTypeAlterAndTranPati.PatientTypeAlter.TDL_PATIENT_ID = this.patientId;
                }

                Inventec.Common.Logging.LogSystem.Debug(LogUtil.TraceData("patientTypeAlterAndTranPati__:", patientTypeAlterAndTranPati));

                if (ActionType == ActionAdd)
                {

                    resultPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisPatientTypeAlterAndTranPatiSDO>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_CREATE, ApiConsumers.MosConsumer, patientTypeAlterAndTranPati, param);
                    if (resultPatientTypeAlter != null)
                    {
                        success = true;
                        if (!UpdatePatientClassify())
                        {
                            success = false;
                            return;
                        }
                        V_HIS_PATIENT_TYPE_ALTER vPTA = new V_HIS_PATIENT_TYPE_ALTER();
                        if (resultPatientTypeAlter.PatientTypeAlter != null)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_PATIENT_TYPE_ALTER>(vPTA, resultPatientTypeAlter.PatientTypeAlter);
                        }
                        PatientTypeDepartmentADO ado = new PatientTypeDepartmentADO();
                        ado.patientTypeAlter = vPTA;
                        ado.LOG_TIME = resultPatientTypeAlter.PatientTypeAlter.LOG_TIME;
                        ado.TREATMENT_ID = resultPatientTypeAlter.PatientTypeAlter.TREATMENT_ID;
                        ado.type = 1;
                        var tml = lstTreatmentLog;
                        tml.Add(ado);
                        if (!chkAutoUpdateType.Checked)
                            LoadDataToGridSereServ(ado, tml);
                        else
                            AutoUpdateSereServ(ado, tml);

                    }
                }
                else if (ActionType == ActionEdit)
                {
                    //issue 3572
                    //kèm issue 13419 gia hạn thêm hạn thẻ
                    long? hanThe = patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_CARD_TO_TIME;
                    if (patientTypeAlterAndTranPati.PatientTypeAlter != null && hanThe != null && hanThe > 0)
                    {
                        double valueAdd = 0;

                        if (patientTypeAlterAndTranPati.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || patientTypeAlterAndTranPati.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                        {
                            valueAdd = HisConfigs.Get<long>("MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT");
                        }

                        hanThe = Inventec.Common.DateTime.Calculation.Add(hanThe ?? 0, valueAdd, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY);
                        HIS_DEPARTMENT_TRAN departmentTran = null;
                        HisDepartmentTranFilter hisDepartmentTranFilter = new HisDepartmentTranFilter();
                        hisDepartmentTranFilter.ID = patientTypeAlterAndTranPati.PatientTypeAlter.DEPARTMENT_TRAN_ID;

                        var rsDepartmentTrans = new BackendAdapter(param).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, hisDepartmentTranFilter, param);
                        if (rsDepartmentTrans != null && rsDepartmentTrans.Count > 0)
                        {
                            HisServiceReqFilter serFilter = new HisServiceReqFilter();
                            serFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                            serFilter.REQUEST_DEPARTMENT_ID = rsDepartmentTrans.FirstOrDefault().DEPARTMENT_ID;

                            var serviceReqLog = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serFilter, param);

                            if (serviceReqLog != null && serviceReqLog.Count > 0)
                            {
                                var checkServiceReq = serviceReqLog.Where(o => o.INTRUCTION_DATE > hanThe).ToList();
                                if (checkServiceReq != null && checkServiceReq.Count > 0)
                                {

                                    WaitingManager.Hide();

                                    HisSereServFilter sereServFilter = new HisSereServFilter();
                                    sereServFilter.SERVICE_REQ_IDs = checkServiceReq.Select(o => o.ID).ToList();
                                    var sereServs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);

                                    if (sereServs != null && sereServs.Count > 0)
                                    {
                                        sereServs = sereServs.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT).ToList();

                                        if (sereServs != null && sereServs.Count > 0)
                                        {
                                            checkServiceReq = checkServiceReq.Where(o => sereServs.Select(p => p.SERVICE_REQ_ID).ToList().Contains(o.ID)).ToList();
                                            var message = String.Join(",", checkServiceReq.Select(o => o.SERVICE_REQ_CODE).ToList());
                                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.DaCoThongTinDoiTuongBHYT, message), ResourceMessage.ThongBao);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    patientTypeAlterAndTranPati.PatientTypeAlter.LOG_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;

                    if (patientTypeAlterAndTranPati.PatientTypeAlter.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT && patientTypeAlterAndTranPati.PatientTypeAlter.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__QN)
                    {
                        patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_CARD_FROM_TIME = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_CARD_NUMBER = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_CARD_TO_TIME = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_MEDI_ORG_CODE = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_MEDI_ORG_NAME = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.JOIN_5_YEAR = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.LEVEL_CODE = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.LIVE_AREA_CODE = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.PAID_6_MONTH = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.RIGHT_ROUTE_CODE = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.HNCODE = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.HAS_BIRTH_CERTIFICATE = null;
                        patientTypeAlterAndTranPati.PatientTypeAlter.IS_TEMP_QN = null;
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("patientTypeAlterAndTranPati__:", patientTypeAlterAndTranPati));
                    var PatientType = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)cboPatientType.Properties.GetDataSourceRowByKeyValue(cboPatientType.EditValue);
                    bool IsCallApiUpdateEveryPayslipInfo = false;
                    if (PatientType.IS_COPAYMENT != 1 && chkAutoUpdateType.Checked)
                    {
                        if (currentTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME == null)
                        {
                            currentTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME = patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_CARD_FROM_TIME;
                            currentTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME = patientTypeAlterAndTranPati.PatientTypeAlter.HEIN_CARD_TO_TIME;
                        }
                        IsCallApiUpdateEveryPayslipInfo = true;
                        AutoUpdateSereServ(currentTreatmentLogSDO, this.lstTreatmentLog);
                    }
                    resultPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisPatientTypeAlterAndTranPatiSDO>("api/HisPatientTypeAlter/Update", ApiConsumers.MosConsumer, patientTypeAlterAndTranPati, null);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("resultPatientTypeAlter__:", resultPatientTypeAlter));

                    if (resultPatientTypeAlter != null)
                    {
                        success = true;
                        if (!UpdatePatientClassify())
                        {
                            success = false;
                            return;
                        }
                        if (check)
                        {
                            if (!chkAutoUpdateType.Checked)
                                LoadDataToGridSereServ(currentTreatmentLogSDO, this.lstTreatmentLog);
                            else if (!IsCallApiUpdateEveryPayslipInfo)
                            {
                                if (currentTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME == null)
                                {

                                    currentTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME = resultPatientTypeAlter.PatientTypeAlter.HEIN_CARD_FROM_TIME;
                                    currentTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME = resultPatientTypeAlter.PatientTypeAlter.HEIN_CARD_TO_TIME;

                                }
                                AutoUpdateSereServ(currentTreatmentLogSDO, this.lstTreatmentLog);
                            }
                        }
                    }


                }
                WaitingManager.Hide();
                if (!string.IsNullOrEmpty(MesError))
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.DVKhongTheChuyenDoi, MesError), ResourceMessage.ThongBao);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
            #region Show message
            MessageManager.Show(this, param, success);
            #endregion

            #region Process has exception
            SessionManager.ProcessTokenLost(param);
            #endregion

            if (success)
            {
                if (RefeshReference != null)
                    RefeshReference();

                this.Close();
            }
        }
        private bool UpdatePatientClassify()
        {
            bool success = true;
            try
            {
                if (cboClassify.EditValue != null && resultPatientTypeAlter != null && resultPatientTypeAlter.PatientTypeAlter != null)
                {
                    var lastTreatment = new BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, this.currentHisTreatment.ID, null);
                    if (lastTreatment == null || (lastTreatment != null && resultPatientTypeAlter.PatientTypeAlter.ID == lastTreatment.ID && resultPatientTypeAlter.PatientTypeAlter.LOG_TIME == lastTreatment.LOG_TIME))
                    {
                        CommonParam paramPatient = new CommonParam();
                        HisPatientUpdateSDO patientUpdateSdo = new MOS.SDO.HisPatientUpdateSDO();
                        patientUpdateSdo.HisPatient = new HIS_PATIENT();
                        patientUpdateSdo.HisPatient = patientId > 0 ? GetPatientByIds(new List<long> { this.patientId }).FirstOrDefault() : GetPatientByIds(new List<long> { this.currenPatient.ID }).FirstOrDefault();
                        if (cboClassify.EditValue != null)
                            patientUpdateSdo.HisPatient.PATIENT_CLASSIFY_ID = Int64.Parse(cboClassify.EditValue.ToString());
                        if (cboMilitaryRank.EditValue != null)
                            patientUpdateSdo.HisPatient.MILITARY_RANK_ID = Int64.Parse(cboMilitaryRank.EditValue.ToString());
                        if (cboPosition.EditValue != null)
                            patientUpdateSdo.HisPatient.POSITION_ID = Int64.Parse(cboPosition.EditValue.ToString());
                        if (cboWorkPlace.EditValue != null)
                            patientUpdateSdo.HisPatient.WORK_PLACE_ID = Int64.Parse(cboWorkPlace.EditValue.ToString());
                        patientUpdateSdo.HisPatient.WORK_PLACE = txtWorkplace.Text;
                        patientUpdateSdo.TreatmentId = currentHisTreatment.ID;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientUpdateSdo), patientUpdateSdo));
                        var resultData = new BackendAdapter(paramPatient).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, patientUpdateSdo, paramPatient);
                        if (resultData != null)
                        {
                            success = true;
                            WaitingManager.Hide();
                        }
                        else
                        {
                            WaitingManager.Hide();
                            success = false;
                            string mesError = paramPatient.GetMessage();
                            if (mesError.Contains(ResourceMessage.XuLyThatBai))
                                mesError = mesError.Replace(ResourceMessage.XuLyThatBai, "");
                            XtraMessageBox.Show(string.Format(ResourceMessage.CapNhatTTDTTB, mesError), ResourceMessage.ThongBao);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }
        private long NumberMin(long n1, long n2)
        {
            long rs = 0;
            try
            {
                if (n1 > n2)
                    rs = n2;
                if (n1 < n2)
                    rs = n1;
                if (n1 == n2)
                    rs = n1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }
        private long NumberMax(long n1, long n2)
        {
            long rs = 0;
            try
            {
                if (n1 > n2)
                    rs = n1;
                if (n1 < n2)
                    rs = n2;
                if (n1 == n2)
                    rs = n1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }
        private void AutoUpdateSereServ(PatientTypeDepartmentADO data, List<PatientTypeDepartmentADO> listTL)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("PatientTypeDepartmentADO", data));
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("List<PatientTypeDepartmentADO>", listTL));
                if (data != null && data.type == 1)
                {
                    data.patientTypeAlter.HEIN_CARD_TO_TIME = data.patientTypeAlter.HEIN_CARD_TO_TIME != null ? Int64.Parse(data.patientTypeAlter.HEIN_CARD_TO_TIME.ToString().Substring(0, 8) + "235959") : 0;
                    listTL = listTL.OrderBy(o => o.LOG_TIME).ToList();

                    PatientTypeDepartmentADO logtime = new PatientTypeDepartmentADO();
                    for (int i = 0; i < listTL.Count; i++)
                    {
                        if (listTL[i].type == 1 && listTL[i].LOG_TIME != data.LOG_TIME && listTL[i].LOG_TIME > data.LOG_TIME)
                        {
                            logtime = listTL[i];
                            break;
                        }
                    }

                    CommonParam param = new CommonParam();


                    HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                    sereServBillFilter.TDL_TREATMENT_ID = data.TREATMENT_ID;
                    sereServBillFilter.IS_NOT_CANCEL = true;
                    var lstSereServBill = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, sereServBillFilter, param);

                    MOS.Filter.HisSereServView4Filter hisSerwServFilter = new HisSereServView4Filter();
                    hisSerwServFilter.TREATMENT_ID = data.TREATMENT_ID;
                    List<V_HIS_SERE_SERV_4> lstHisSereServWithTreatment = new List<V_HIS_SERE_SERV_4>();
                    lstHisSereServWithTreatment = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_4>>("api/HisSereServ/GetView4", ApiConsumers.MosConsumer, hisSerwServFilter, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstHisSereServWithTreatment.Select(o => new { o.PATIENT_TYPE_ID, o.TDL_INTRUCTION_TIME })), lstHisSereServWithTreatment.Select(o => new { o.PATIENT_TYPE_ID, o.TDL_INTRUCTION_TIME })));
                    //35017
                    ProcessDataForUpdatePaty();
                    if (lstHisSereServWithTreatment != null && lstHisSereServWithTreatment.Count > 0)
                    {
                        if (lstSereServBill != null && lstSereServBill.Count > 0)
                        {
                            lstHisSereServWithTreatment = lstHisSereServWithTreatment.Where(o => !lstSereServBill.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstHisSereServWithTreatment.Select(o => new { o.PATIENT_TYPE_ID, o.TDL_INTRUCTION_TIME })), lstHisSereServWithTreatment.Select(o => new { o.PATIENT_TYPE_ID, o.TDL_INTRUCTION_TIME })));

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.LOG_TIME), data.LOG_TIME));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.patientTypeAlter.HEIN_CARD_FROM_TIME), data.patientTypeAlter.HEIN_CARD_FROM_TIME));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.patientTypeAlter.TREATMENT_TYPE_ID), data.patientTypeAlter.TREATMENT_TYPE_ID));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.patientTypeAlter.HEIN_CARD_TO_TIME), data.patientTypeAlter.HEIN_CARD_TO_TIME));
                        lstSereServResult = new List<V_HIS_SERE_SERV_4>();
                        if (lstHisSereServWithTreatment != null && lstHisSereServWithTreatment.Count > 0)
                        {
                            if (logtime != null)
                            {
                                if (logtime.LOG_TIME > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Error("CASE1");
                                    lstSereServResult = lstHisSereServWithTreatment.
                                         Where(o => o.IS_NOT_USE_BHYT != 1 && (o.PATIENT_TYPE_ID != Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()))
                                        && (NumberMax(data.LOG_TIME, data.patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) <= o.TDL_INTRUCTION_TIME && o.TDL_INTRUCTION_TIME <=
                                        ((data.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || data.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY) ? (data.patientTypeAlter.HEIN_CARD_TO_TIME > 0 ?
                                        NumberMin(logtime.LOG_TIME, Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? DateTime.Now).AddDays(HisConfigs.Get<long>("MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT"))) ?? 0) : logtime.LOG_TIME) : (data.patientTypeAlter.HEIN_CARD_TO_TIME > 0 ? NumberMin(logtime.LOG_TIME, data.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) : logtime.LOG_TIME)))).ToList();
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Error("CASE2");
                                    lstSereServResult = lstHisSereServWithTreatment.
                                         Where(o => o.IS_NOT_USE_BHYT != 1 && (o.PATIENT_TYPE_ID != Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()))
                                        && (NumberMax(data.LOG_TIME, data.patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) <= o.TDL_INTRUCTION_TIME)).ToList();
                                    if (data.patientTypeAlter.HEIN_CARD_TO_TIME > 0)
                                        lstSereServResult = lstSereServResult.
                                        Where(o => o.TDL_INTRUCTION_TIME <= ((data.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || data.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY) ? (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? DateTime.Now).AddDays(HisConfigs.Get<long>("MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT"))) ?? 0) : data.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0)
                                       ).ToList();
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("CASE3");
                                lstSereServResult = lstHisSereServWithTreatment.
                                         Where(o => o.IS_NOT_USE_BHYT != 1 && (o.PATIENT_TYPE_ID != Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()))
                                        && (NumberMax(data.LOG_TIME, data.patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) <= o.TDL_INTRUCTION_TIME)).ToList();
                                if (data.patientTypeAlter.HEIN_CARD_TO_TIME > 0)
                                    lstSereServResult = lstHisSereServWithTreatment.
                                       Where(o => o.TDL_INTRUCTION_TIME <= ((data.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || data.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY) ? (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? DateTime.Now).AddDays(HisConfigs.Get<long>("MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT"))) ?? 0) : data.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0)).ToList();
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSereServResult.Select(o => new { o.SERVICE_ID, o.SERVICE_REQ_ID, o.SERVICE_TYPE_CODE })), lstSereServResult.Select(o => new { o.SERVICE_ID, o.SERVICE_REQ_ID, o.SERVICE_TYPE_CODE })));
                        if (lstSereServResult.Count > 0)
                        {
                            data.patientTypeAlter.TREATMENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "0").ToString());
                            data.patientTypeAlter.TREATMENT_TYPE_CODE = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == data.patientTypeAlter.TREATMENT_TYPE_ID).TREATMENT_TYPE_CODE;
                            data.patientTypeAlter.TREATMENT_TYPE_NAME = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == data.patientTypeAlter.TREATMENT_TYPE_ID).TREATMENT_TYPE_NAME;

                            long? primaryKeyPatientType = null;

                            long PatientType = 0;
                            if (cboPrimaryPatientType.EditValue != null && !string.IsNullOrEmpty(cboPrimaryPatientType.EditValue.ToString()))
                            {
                                primaryKeyPatientType = Inventec.Common.TypeConvert.Parse.ToInt64(cboPrimaryPatientType.EditValue.ToString());
                            }
                            if (cboPatientType.EditValue != null)
                                PatientType = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                            if (Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__MOS_HIS_SERE_SERV_IS_SET_PRIMARY_PATIENT_TYPE)) == 2)
                            {
                                if (primaryKeyPatientType == PatientType)
                                {
                                    primaryKeyPatientType = null;
                                }
                            }
                            SwapPatientTypeAlter(PatientType, primaryKeyPatientType, data, listTL, lstSereServResult);
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessDataForUpdatePaty()
        {
            try
            {
                var servicePatyInBranchs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_PATY>();
                var patienttpecodeAllows = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Select(o => o.PATIENT_TYPE_CODE).ToList();
                servicePatyInBranchs = servicePatyInBranchs.Where(o => patienttpecodeAllows != null && patienttpecodeAllows.Contains(o.PATIENT_TYPE_CODE)).ToList();

                var setyGroups = servicePatyInBranchs.GroupBy(o => o.SERVICE_ID).ToList();
                if (setyGroups != null && setyGroups.Count > 0)
                {
                    foreach (var item in setyGroups)
                    {
                        dicSevicepatyAllows.Add(item.Key, item.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void SwapPatientTypeAlter(long patient_type_id, long? patient_primary_patient_type_id, PatientTypeDepartmentADO HisTreatmentLogSDO, List<PatientTypeDepartmentADO> _lstTreatmentLog, List<V_HIS_SERE_SERV_4> _lstSereServ)
        {
            try
            {


                List<HIS_MEDICINE> MedicineList = new List<HIS_MEDICINE>();
                List<HIS_MATERIAL> MaterialList = new List<HIS_MATERIAL>();

                var medicineIdList = _lstSereServ
                    .Where(o => o.MEDICINE_ID.HasValue && o.MEDICINE_ID.Value > 0)
                    .Select(p => p.MEDICINE_ID.Value).Distinct().ToList();

                var materialIdList = _lstSereServ
                    .Where(o => o.MATERIAL_ID.HasValue && o.MATERIAL_ID.Value > 0)
                    .Select(p => p.MATERIAL_ID.Value).Distinct().ToList();

                if (medicineIdList != null && medicineIdList.Count > 0)
                {
                    MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    medicineFilter.IDs = medicineIdList;
                    MedicineList = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, null);
                }

                if (materialIdList != null && materialIdList.Count > 0)
                {
                    MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = materialIdList;
                    MaterialList = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, null);
                }

                HisServiceFilter serviceFilter = new HisServiceFilter();
                serviceFilter.IDs = _lstSereServ.Select(o => o.SERVICE_ID).ToList();
                var lstServiceBySereServ = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE>>("api/HisService/Get", ApiConsumers.MosConsumer, serviceFilter, null);

                foreach (var item in _lstSereServ)
                {
                    var oldPatientTypeId = item.PATIENT_TYPE_ID;
                    var service = lstServiceBySereServ.Where(o => o.ID == item.SERVICE_ID).First();
                    if (item.MEDICINE_ID.HasValue && item.MEDICINE_ID.Value > 0
                        && MedicineList != null && MedicineList.Count > 0)
                    {
                        if (patient_type_id == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            var checkMedicine = MedicineList.FirstOrDefault(o => o.ID == item.MEDICINE_ID.Value);
                            var medicineType = checkMedicine != null
                                ? BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == checkMedicine.MEDICINE_TYPE_ID)
                                : null;
                            if (medicineType != null && !String.IsNullOrWhiteSpace(medicineType.ACTIVE_INGR_BHYT_CODE)
                                && (medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM || medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL || medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT))
                            {
                                if (HisTreatmentLogSDO != null && HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                                {
                                    DateTime heinCardToTimeSys = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? DateTime.MinValue;
                                    long exceedDay = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.EXCEED_DAY_ALLOW_FOR_IN_PATIENT));

                                    if (exceedDay > 0)
                                        heinCardToTimeSys = heinCardToTimeSys.AddDays(exceedDay);


                                    if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(heinCardToTimeSys))
                                    {
                                        item.PATIENT_TYPE_ID = patient_type_id;
                                        item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                    }

                                }
                                else if (HisTreatmentLogSDO != null)
                                {
                                    if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME)
                                    {
                                        item.PATIENT_TYPE_ID = patient_type_id;
                                        item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                    }
                                }

                            }
                        }
                        else
                        {
                            item.PATIENT_TYPE_ID = patient_type_id;
                            item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                        }
                    }
                    else if (item.MATERIAL_ID.HasValue && item.MATERIAL_ID.Value > 0
                        && MaterialList != null && MaterialList.Count > 0)
                    {
                        if (patient_type_id == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            var checkMaterial = MaterialList.FirstOrDefault(o => o.ID == item.MATERIAL_ID.Value);
                            var materialType = checkMaterial != null
                                ? BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == checkMaterial.MATERIAL_TYPE_ID)
                                : null;
                            if (materialType != null && !String.IsNullOrWhiteSpace(materialType.HEIN_SERVICE_BHYT_CODE)
                                && !String.IsNullOrWhiteSpace(materialType.HEIN_SERVICE_BHYT_NAME)
                                && (materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT || materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM || materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL))
                            {
                                if (HisTreatmentLogSDO != null && HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                                {
                                    DateTime heinCardToTimeSys = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? DateTime.MinValue;
                                    long exceedDay = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.EXCEED_DAY_ALLOW_FOR_IN_PATIENT));

                                    if (exceedDay > 0)
                                        heinCardToTimeSys = heinCardToTimeSys.AddDays(exceedDay);


                                    if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(heinCardToTimeSys))
                                    {
                                        item.PATIENT_TYPE_ID = patient_type_id;

                                        item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                    }

                                }
                                else if (HisTreatmentLogSDO != null)
                                {
                                    if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME)
                                    {
                                        item.PATIENT_TYPE_ID = patient_type_id;

                                        item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                    }
                                }
                            }
                        }
                        else
                        {
                            item.PATIENT_TYPE_ID = patient_type_id;
                            item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                        }
                    }
                    else if (dicSevicepatyAllows.ContainsKey(item.SERVICE_ID))
                    {
                        if (patient_type_id == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            if (!string.IsNullOrEmpty(item.TDL_HEIN_SERVICE_BHYT_CODE) && !string.IsNullOrEmpty(item.TDL_HEIN_SERVICE_BHYT_NAME))
                            {
                                var setyCheck = dicSevicepatyAllows[item.SERVICE_ID];
                                if (setyCheck != null && setyCheck.Count > 0)
                                {
                                    if (setyCheck.FirstOrDefault(o => o.PATIENT_TYPE_ID == patient_type_id) != null)
                                    {
                                        if (HisTreatmentLogSDO != null && HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || HisTreatmentLogSDO.patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                                        {
                                            DateTime heinCardToTimeSys = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? DateTime.MinValue;
                                            long exceedDay = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.EXCEED_DAY_ALLOW_FOR_IN_PATIENT));

                                            if (exceedDay > 0)
                                                heinCardToTimeSys = heinCardToTimeSys.AddDays(exceedDay);

                                            if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(heinCardToTimeSys))
                                            {
                                                item.PATIENT_TYPE_ID = patient_type_id;
                                                item.PRIMARY_PATIENT_TYPE_ID = ProcessPrimaryPatientTypeId(item, service);

                                                item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                            }

                                        }
                                        else if (HisTreatmentLogSDO != null)
                                        {
                                            if (item.IS_NOT_USE_BHYT != 1 && HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_FROM_TIME <= item.TDL_INTRUCTION_TIME && item.TDL_INTRUCTION_TIME <= HisTreatmentLogSDO.patientTypeAlter.HEIN_CARD_TO_TIME)
                                            {
                                                item.PATIENT_TYPE_ID = patient_type_id;
                                                item.PRIMARY_PATIENT_TYPE_ID = ProcessPrimaryPatientTypeId(item, service);

                                                item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            item.PATIENT_TYPE_ID = patient_type_id;
                            item.PRIMARY_PATIENT_TYPE_ID = ProcessPrimaryPatientTypeId(item, service);
                            item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patient_type_id).PATIENT_TYPE_NAME;
                        }
                    }
                    else
                    {
                        MesError += item.TDL_SERVICE_CODE + ", ";
                    }
                    if (item.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
                        item.SERVICE_CONDITION_ID = null;
                    else if (service.DO_NOT_USE_BHYT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        item.PATIENT_TYPE_ID = oldPatientTypeId;
                        item.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == oldPatientTypeId).PATIENT_TYPE_NAME;
                    }
                    newLstSerSev.Add(item);
                }
                UpdateHisSereServ(HisTreatmentLogSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long? ProcessPrimaryPatientTypeId(V_HIS_SERE_SERV_4 item, HIS_SERVICE service)
        {
            long? primaryPatientType = null;

            try
            {
                if (keyIsSetPrimaryPatientType == 2)
                {
                    long? primaryKeyPatientType = cboPrimaryPatientType.EditValue != null ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboPrimaryPatientType.EditValue.ToString()) : null;
                    long? PatientTypeId = cboPatientType.EditValue != null ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()) : null;
                    primaryPatientType = primaryKeyPatientType != null && primaryKeyPatientType != PatientTypeId ? primaryKeyPatientType : null;
                }
                else if (keyIsSetPrimaryPatientType == 1)
                {
                    long? patient_classify_id = null;
                    if (cboClassify.EditValue != null && !string.IsNullOrEmpty(cboClassify.EditValue.ToString()))
                    {
                        patient_classify_id = Inventec.Common.TypeConvert.Parse.ToInt64(cboClassify.EditValue.ToString());
                    }
                    var setyCheck = dicSevicepatyAllows[item.SERVICE_ID];
                    if (setyCheck != null && setyCheck.Count > 0)
                    {
                        if (service != null && service.BILL_PATIENT_TYPE_ID != null && setyCheck.FirstOrDefault(o => o.PATIENT_TYPE_ID == service.BILL_PATIENT_TYPE_ID) != null
&& item.PATIENT_TYPE_ID != service.BILL_PATIENT_TYPE_ID && (service.APPLIED_PATIENT_TYPE_IDS == null || service.APPLIED_PATIENT_TYPE_IDS.Split(',').ToList().Contains(item.PATIENT_TYPE_ID.ToString()))
&& (service.APPLIED_PATIENT_CLASSIFY_IDS == null || (patient_classify_id != null && service.APPLIED_PATIENT_CLASSIFY_IDS.Split(',').ToList().Contains(patient_classify_id.ToString()))))
                        {
                            primaryPatientType = service.BILL_PATIENT_TYPE_ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return primaryPatientType;
        }

        private void UpdateHisSereServ(PatientTypeDepartmentADO HisTreatmentLogSDO)
        {
            try
            {
                lstSereServ = new List<HIS_SERE_SERV>();
                bool result = false;
                CommonParam param = new CommonParam();
                HisSereServPayslipSDO sdo = new HisSereServPayslipSDO();

                if (newLstSerSev != null && newLstSerSev.Count > 0)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_4, HIS_SERE_SERV>();
                    lstSereServ = AutoMapper.Mapper.Map<List<HIS_SERE_SERV>>(newLstSerSev);
                }

                sdo.SereServs = lstSereServ;
                sdo.Field = UpdateField.PATIENT_TYPE_ID;
                sdo.TreatmentId = HisTreatmentLogSDO.TREATMENT_ID;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                var updateSs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_SERE_SERV>>("api/HisSereServ/UpdateEveryPayslipInfo", ApiConsumers.MosConsumer, sdo, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateSs), updateSs));
                if (updateSs != null)
                {
                    result = true;
                    param.Messages = new List<string>();
                }
                #region Show message
                #endregion

                #region Process has exception
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsUnusedHeinCardNumberByAnother(string heinCardNumber, long patientId)
        {
            bool valid = true;
            try
            {
                //neu co su dung the thi kiem tra xem the nay da duoc su dung boi benh nhan khac chua
                if (!string.IsNullOrEmpty(heinCardNumber))
                {
                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterFilter filter = new HisPatientTypeAlterFilter();
                    filter.HEIN_CARD_NUMBER__EXACT = heinCardNumber;
                    //luu y: kem theo filter nay de tranh phai truy van nhieu du lieu
                    filter.TDL_PATIENT_ID__NOT_EQUAL = patientId;

                    List<HIS_PATIENT_TYPE_ALTER> ptas = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, filter, param);

                    if (ptas != null && ptas.Count > 0)
                    {
                        List<long> patientIds = ptas.Select(o => o.TDL_PATIENT_ID).ToList();
                        List<HIS_PATIENT> patients = GetPatientByIds(patientIds);
                        List<string> anotherUses = patients != null ? patients.Select(o => o.PATIENT_CODE).ToList() : null;
                        if (anotherUses != null && anotherUses.Count > 0)
                        {
                            string patientCodes = string.Join(", ", anotherUses);
                            WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.TheNayDaDuocSD, patientCodes), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                List<object> obj = new List<object>();
                                obj.Add(this.treatmentId);
                                obj.Add(anotherUses);
                                obj.Add((HIS.Desktop.Common.DelegateSelectData)DelegateSuccess);

                                CallModule call = new CallModule(CallModule.TreatmentPatientUpdate, this.module.RoomId, this.module.RoomTypeId, obj);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private void DelegateSuccess(object data)
        {
            try
            {
                if (data is long)
                    this.patientId = (long)data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_PATIENT> GetPatientByIds(List<long> ids)
        {
            List<HIS_PATIENT> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.IDs = ids;

                rs = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        bool IsChild()
        {
            bool isChild = false;
            try
            {
                var dtDateOfBird = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHisTreatment.TDL_PATIENT_DOB) ?? DateTime.Now;
                isChild = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtDateOfBird);
            }
            catch (Exception ex)
            {
                isChild = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return isChild;
        }

        private void UpdateTranpati(HisPatientTypeAlterAndTranPatiSDO tranpati, HisPatientProfileSDO patientProfileSDO)
        {
            try
            {
                if (patientProfileSDO != null && patientProfileSDO.HisTreatment != null)
                {
                    tranpati.TransferInFormId = patientProfileSDO.HisTreatment.TRANSFER_IN_FORM_ID;
                    //tranpati.TransferInIcdId = patientProfileSDO.HisTreatment.TRANSFER_IN_ICD_ID;
                    tranpati.TransferInIcdName = patientProfileSDO.HisTreatment.TRANSFER_IN_ICD_NAME;
                    tranpati.TransferInMediOrgCode = patientProfileSDO.HisTreatment.TRANSFER_IN_MEDI_ORG_CODE;
                    tranpati.TransferInMediOrgName = patientProfileSDO.HisTreatment.TRANSFER_IN_MEDI_ORG_NAME;
                    tranpati.TransferInReasonId = patientProfileSDO.HisTreatment.TRANSFER_IN_REASON_ID;
                    tranpati.TransferInCmkt = patientProfileSDO.HisTreatment.TRANSFER_IN_CMKT;//TODO
                    tranpati.TransferInCode = patientProfileSDO.HisTreatment.TRANSFER_IN_CODE;//TODO
                    tranpati.TransferInIcdCode = patientProfileSDO.HisTreatment.TRANSFER_IN_ICD_CODE;
                    tranpati.TransferInTimeFrom = patientProfileSDO.HisTreatment.TRANSFER_IN_TIME_FROM;
                    tranpati.TransferInTimeTo = patientProfileSDO.HisTreatment.TRANSFER_IN_TIME_TO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdatePatientTypeAlterFromDataForm(ref HisPatientTypeAlterAndTranPatiSDO patientTypeAlterSDO)
        {
            try
            {
                if (patientTypeAlterSDO == null)
                    patientTypeAlterSDO = new HisPatientTypeAlterAndTranPatiSDO();
                if (patientTypeAlterSDO.PatientTypeAlter == null)
                    patientTypeAlterSDO.PatientTypeAlter = new HIS_PATIENT_TYPE_ALTER();

                Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, HIS_PATIENT_TYPE_ALTER>();
                Mapper.CreateMap<HIS_PATIENT_TYPE_ALTER, HIS_PATIENT_TYPE_ALTER>();

                HisPatientProfileSDO HisPatientProfileSDO = new HisPatientProfileSDO();
                HisPatientProfileSDO.HisPatientTypeAlter = Mapper.Map<HIS_PATIENT_TYPE_ALTER, HIS_PATIENT_TYPE_ALTER>(patientTypeAlterSDO.PatientTypeAlter);

                UpdatePatientTypeAlterFromDataForm(HisPatientProfileSDO);
                patientTypeAlterSDO.PatientTypeAlter = Mapper.Map<HIS_PATIENT_TYPE_ALTER, HIS_PATIENT_TYPE_ALTER>(HisPatientProfileSDO.HisPatientTypeAlter);

                UpdateTranpati(patientTypeAlterSDO, HisPatientProfileSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdatePatientTypeAlterFromDataForm(HisPatientProfileSDO ServiceReqData)
        {
            try
            {
                if (ServiceReqData.HisPatientTypeAlter == null)
                    ServiceReqData.HisPatientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                if (ServiceReqData.HisPatientTypeAlter == null)
                    ServiceReqData.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();

                long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "").ToString());
                var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeId);
                if (patientType != null)
                {
                    if (patientType.ID == HisConfigCFG.PatientTypeId__BHYT || patientType.ID == HisConfigCFG.PatientTypeId__QN)
                    {
                        UpdateHeinCardBHYTDTOFromDataForm(ServiceReqData);
                    }

                    ServiceReqData.HisPatientTypeAlter.PATIENT_TYPE_ID = patientType.ID;

                    if (dtLogTime.EditValue != null && dtLogTime.DateTime != DateTime.MinValue)
                    {
                        ServiceReqData.HisPatientTypeAlter.LOG_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtLogTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                    }
                    if (cboTreatmentType.EditValue != null)
                        ServiceReqData.HisPatientTypeAlter.TREATMENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "0").ToString());


                    if (cboPrimaryPatientType.EditValue != null)
                    {
                        ServiceReqData.HisPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID = Convert.ToInt64(cboPrimaryPatientType.EditValue);
                    }
                    else
                    {
                        ServiceReqData.HisPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID = null;
                    }
                }
                else
                {
                    LogSystem.Debug("Lay doi tuong benh nhan theo gia tri cua combo doi tuong man hinh dang ky tiep don khong thanh cong. patientTypeId= " + patientTypeId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateHeinCardBHYTDTOFromDataForm(HisPatientProfileSDO HisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (HisPatientTypeAlter.HisPatientTypeAlter == null)
                    HisPatientTypeAlter.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();
                if (HisPatientTypeAlter.HisTreatment == null)
                    HisPatientTypeAlter.HisTreatment = new HIS_TREATMENT();

                uCMainHein.UpdateDataFormIntoPatientTypeAlter(this.ucHein__BHYT, HisPatientTypeAlter);
                uCMainHein.UpdateDataFormIntoPatientProfile(this.ucHein__BHYT, HisPatientTypeAlter);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled == true)
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLogTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatmentTypeCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentTypeCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadTreatmentType(strValue, false, cboTreatmentType, txtTreatmentTypeCode, txtPatientType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientType_KeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    bool isChangePatientType = LoadPatientType(strValue, false, cboPatientType, txtPatientType);
                    if (isChangePatientType)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetProvinceCode()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = currentHisTreatment.PATIENT_ID;
                var DataProvinceCode = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GET, ApiConsumers.MosConsumer, filter, param);
                if (DataProvinceCode != null && DataProvinceCode.Count > 0)
                {
                    provindcode = DataProvinceCode.FirstOrDefault().PROVINCE_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (uCMainHein != null && ucHein__BHYT != null)
                {
                    var dt = cboTreatmentType.EditValue != null ? Int64.Parse(cboTreatmentType.EditValue.ToString()) : 0;
                    if (currentTreatmentType == 0 || currentTreatmentType != dt)
                        uCMainHein.SetValueTreatmentType(ucHein__BHYT, dt);
                    currentTreatmentType = dt;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadTreatmentType(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit _cboTreatmentType, DevExpress.XtraEditors.TextEdit _txtTreatmentType, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    _cboTreatmentType.EditValue = null;
                    _cboTreatmentType.Focus();
                    _cboTreatmentType.ShowPopup();
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>().Where(o => o.TREATMENT_TYPE_CODE.Equals(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            _cboTreatmentType.EditValue = data[0].ID;
                            _txtTreatmentType.Text = data[0].TREATMENT_TYPE_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                        }
                        else
                        {
                            _cboTreatmentType.EditValue = null;
                            _cboTreatmentType.Focus();
                            _cboTreatmentType.ShowPopup();
                        }
                    }
                    else
                    {
                        _cboTreatmentType.EditValue = null;
                        _cboTreatmentType.Focus();
                        _cboTreatmentType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// true: Đã thay đổi giá trị patientTypeAlter . tiennv
        /// </summary>
        /// <param name="searchCode"></param>
        /// <param name="isExpand"></param>
        /// <param name="_cboPatientType"></param>
        /// <param name="_txtPatientType"></param>
        /// <returns></returns>
        public bool LoadPatientType(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit _cboPatientType, DevExpress.XtraEditors.TextEdit _txtPatientType)
        {
            bool result = false;
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    _cboPatientType.EditValue = null;
                    _cboPatientType.Focus();
                    _cboPatientType.ShowPopup();
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.PATIENT_TYPE_CODE.Equals(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            if (_cboPatientType.EditValue == null || ((long)_cboPatientType.EditValue) != data[0].ID)
                            {
                                result = true;
                                _cboPatientType.EditValue = data[0].ID;
                                _txtPatientType.Text = data[0].PATIENT_TYPE_CODE;
                            }
                        }
                        else
                        {
                            _cboPatientType.EditValue = null;
                            _cboPatientType.Focus();
                            _cboPatientType.ShowPopup();
                        }
                    }
                    else
                    {
                        _cboPatientType.EditValue = null;
                        _cboPatientType.Focus();
                        _cboPatientType.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void dtLogTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (dtLogTime.EditValue != null)
                    {
                        txtTreatmentTypeCode.Focus();
                        txtTreatmentTypeCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.lciQrcode.Enabled = false;
                if (cboPatientType.EditValue != null && cboPatientType.EditValue != cboPatientType.OldEditValue && !IsLoadForm)
                {
                    cboClassify.EditValue = null;
                    LoadPatientClassify();
                    InitComboCommon(this.cboClassify, dataClassify, "ID", "PATIENT_CLASSIFY_NAME", "PATIENT_CLASSIFY_CODE");
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)cboPatientType.Properties.GetDataSourceRowByKeyValue(cboPatientType.EditValue);
                    if (patientType != null)
                    {
                        txtPatientType.Text = patientType.PATIENT_TYPE_CODE;
                        lciPrimaryPatientType.AppearanceItemCaption.ForeColor = Color.Black;
                        if (patientType.IS_ADDITION_REQUIRED == 1)
                        {
                            lciPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lciComboPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lciPrimaryPatientType.AppearanceItemCaption.ForeColor = Color.Maroon;
                            ValidPrimaryPatientTypeCode();
                        }
                        else
                        {
                            VisiblePrimaryPatientType();
                            lciPrimaryPatientType.AppearanceItemCaption.ForeColor = Color.Black;
                            this.dxValidationProvider1.RemoveControlError(txtPrimaryPatientTypeCode);
                            this.dxValidationProvider1.SetValidationRule(txtPrimaryPatientTypeCode, null);

                        }
                        if (patientType.ID == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            ucKskContract = null;
                            ChoiceTemplateHeinCard(patientType.PATIENT_TYPE_CODE, true);
                            this.lciQrcode.Enabled = false;
                            //LoadDataToGridSereServ();
                        }
                        else if (patientType.ID == HisConfigCFG.PatientTypeId__KSK)
                        {
                            ucImageBHYT = null;
                            emptySpaceItem3.Visibility = LayoutVisibility.Never;
                            layoutControlItem8.Visibility = LayoutVisibility.Never;
                            btnSave.Size = new Size(110, btnSave.Height);
                            this.Size = new Size(this.Width, 130);

                            xclHeinCardInformation.Controls.Clear();
                            xclHeinCardInformation.Update();
                            xclHeinCardInformation.Enabled = true;
                            ucKskContract = new UC_KskContract();
                            ucKskContract.Dock = DockStyle.Fill;
                            xclHeinCardInformation.Controls.Add(ucKskContract);
                        }
                        else
                        {
                            ucKskContract = null;
                            ChoiceTemplateHeinCard(patientType.PATIENT_TYPE_CODE, false);
                        }
                        if (layoutControlItem9.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never && patientType.IS_COPAYMENT != 1)
                        {
                            if (!chkAutoUpdateType.Checked)
                                LoadDataToGridSereServ(currentTreatmentLogSDO, this.lstTreatmentLog);
                        }
                    }

                    IList<Control> invalidControls = dxValidationProvider1.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        dxValidationProvider1.RemoveControlError(invalidControls[i]);
                    }
                    dxErrorProvider1.ClearErrors();
                }
                else
                {
                    btnSave.Focus();
                    btnSave.Enabled = true;
                }
                ReSizeForm();
                LoadPrimaryPatientType();
                IsLoadForm = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void EnableSave(bool isEnable)
        {
            try
            {
                btnSave.Enabled = isEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private async void txtQrcode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (strValue.Length > 0)
                    {
                        bool IsCccd = false;
                        HeinCardData heinCardData = null;
                        var strValueSplit = strValue.Split('|');
                        if (strValueSplit[0].Length == 10 || strValueSplit[0].Length == 15)
                            heinCardData = GetDataQrCodeHeinCard(strValue);
                        else if (strValueSplit[0].Length == 12)
                        {
                            CccdCardData cccdCard = GetDataQrCodeCccdCard(strValue);
                            IsCccd = true;
                            heinCardData = new HeinCardData();
                            heinCardData.HeinCardNumber = cccdCard.CardData;
                            heinCardData.PatientName = cccdCard.PatientName;
                            heinCardData.Dob = cccdCard.Dob;
                            heinCardData.Gender = cccdCard.Gender == "NAM" ? "1" : "2";
                            heinCardData.Address = cccdCard.Address;
                        }
                        if (heinCardData != null)
                        {
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strValue), strValue));
                            string mess = "";
                            string heinAddressOfPatient = "";
                            if (this.currentHisTreatment != null)
                            {
                                CommonParam param = new CommonParam();
                                HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                                filter.PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Convert.ToInt64(this.currentHisTreatment.TDL_PATIENT_CODE));
                                var patientSDO = new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();

                                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientSDO), patientSDO));
                                if (patientSDO != null)
                                {
                                    string dob = "";
                                    if (patientSDO.IS_HAS_NOT_DAY_DOB == 1)
                                    {
                                        dob = patientSDO.DOB.ToString().Substring(0, 4);
                                    }
                                    else
                                        dob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientSDO.DOB);
                                    if ((!IsCccd && patientSDO.VIR_PATIENT_NAME.ToLower() != Inventec.Common.String.Convert.HexToUTF8Fix(heinCardData.PatientName).ToLower()) || (IsCccd && patientSDO.VIR_PATIENT_NAME.ToLower() != heinCardData.PatientName.ToLower()))
                                    {
                                        // MessageManager.Show(ResourceMessage.TheSaiHTenGov060);
                                        heinCardData = null;
                                        mess = ResourceMessage.TheSaiHTenGov060;
                                    }
                                    else if (dob != heinCardData.Dob)
                                    {
                                        //   MessageManager.Show(ResourceMessage.TheSaiNgaySinhGov070);
                                        heinCardData = null;
                                        mess = ResourceMessage.TheSaiNgaySinhGov070;
                                    }
                                    else if (string.IsNullOrEmpty(patientSDO.COMMUNE_CODE) || string.IsNullOrEmpty(patientSDO.DISTRICT_CODE) || string.IsNullOrEmpty(patientSDO.PROVINCE_CODE))
                                    {
                                        heinCardData = null;
                                        mess = "Bệnh nhân thiếu thông tin địa chỉ";
                                        //Commment
                                    }
                                    else
                                    {
                                        heinAddressOfPatient = patientSDO.HeinAddress;
                                        string patientName = Inventec.Common.String.Convert.HexToUTF8Fix(heinCardData.PatientName);
                                        if (!string.IsNullOrEmpty(patientName))
                                            heinCardData.PatientName = patientName;
                                        string address = Inventec.Common.String.Convert.HexToUTF8Fix(heinCardData.Address);
                                        if (!string.IsNullOrEmpty(address))
                                            heinCardData.Address = address;
                                    }
                                }
                            }
                            if (heinCardData != null && string.IsNullOrEmpty(mess))
                            {
                                if (!IsCccd)
                                    this.ProcessQrCodeData(heinCardData);
                                HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
                                if (!IsCccd)
                                    ResultDataADO = await heinGOVManager.Check(heinCardData, null, false, heinAddressOfPatient, dtLogTime.DateTime, true);
                                else
                                    ResultDataADO = await heinGOVManager.CheckCccdQrCode(heinCardData, null, dtLogTime.DateTime);
                                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heinCardData), heinCardData) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ResultDataADO), ResultDataADO));
                                uCMainHein.SetResultDataADOBhyt(ucHein__BHYT, ResultDataADO);
                                if (ResultDataADO != null)
                                {
                                    heinCardData.HeinCardNumber = ResultDataADO.ResultHistoryLDO.maThe ?? heinCardData.HeinCardNumber;
                                    //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                                    if (ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                                        heinCardData.HeinCardNumber = ResultDataADO.ResultHistoryLDO.maTheMoi;
                                    this.ProcessQrCodeData(heinCardData);
                                    this.CheckTTProcessResultData(heinCardData, ResultDataADO);
                                }
                            }
                            else if (!string.IsNullOrEmpty(mess))
                            {
                                string thongBao = mess + ResourceMessage.SuaTTBN;
                                DialogResult drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBao, "Thông báo!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                                if (drReslt == DialogResult.OK)
                                {
                                    if (_HisTreatment == null)
                                        _HisTreatment = this.GetSDO(this.treatmentId).SingleOrDefault();
                                    List<object> listArgs = new List<object>();
                                    listArgs.Add(_HisTreatment.PATIENT_ID);
                                    listArgs.Add(this.treatmentId);
                                    listArgs.Add((RefeshReference)RefeshTreatment);
                                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.PatientUpdate", this.module.RoomId, this.module.RoomTypeId, listArgs);
                                }
                            }

                            this.txtQrcode.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string HisToHein(string ge)
        {
            return (ge == "1") ? "2" : "1";
        }

        private void ProcessQrCodeData(HeinCardData dataHein)
        {
            try
            {
                string currentHeincardNumber = dataHein.HeinCardNumber;
                if (dataHein == null) throw new ArgumentNullException("ProcessQrCodeData => dataHein is null");
                if (!String.IsNullOrEmpty(dataHein.HeinCardNumber))
                {
                    if (dataHein.HeinCardNumber.Length > 15)
                        dataHein.HeinCardNumber = dataHein.HeinCardNumber.Substring(0, 15);
                    else if (dataHein.HeinCardNumber.Length < 15)
                        LogSystem.Info("Do dai so the bhyt cua benh nhan khong hop le. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHeincardNumber), currentHeincardNumber));
                }

                FillDataAfterFindQrCodeNoExistsCard(dataHein);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckTTProcessResultData(HeinCardData dataHein, ResultDataADO ResultDataADO)
        {
            try
            {
                if (ResultDataADO != null && ResultDataADO.ResultHistoryLDO != null)
                {
                    if (ResultDataADO.ResultHistoryLDO.maKetQua == "000" && ResultDataADO.ResultHistoryLDO.gioiTinh.ToLower() != currentHisTreatment.TDL_PATIENT_GENDER_NAME.ToLower())
                    {
                        CommonParam paramPatient = new CommonParam();
                        HisPatientUpdateSDO patientUpdateSdo = new MOS.SDO.HisPatientUpdateSDO();
                        patientUpdateSdo.HisPatient = new HIS_PATIENT();
                        patientUpdateSdo.HisPatient = patientId > 0 ? GetPatientByIds(new List<long> { this.patientId }).FirstOrDefault() : GetPatientByIds(new List<long> { this.currenPatient.ID }).FirstOrDefault();
                        var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.GENDER_NAME.ToLower() == ResultDataADO.ResultHistoryLDO.gioiTinh.ToLower());
                        if (gender != null)
                        {
                            patientUpdateSdo.HisPatient.GENDER_ID = gender.ID;
                            patientUpdateSdo.TreatmentId = currentHisTreatment.ID;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientUpdateSdo), patientUpdateSdo));
                            currenPatient = new BackendAdapter(paramPatient).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, patientUpdateSdo, paramPatient);
                            LoadCurrentHisTreatment();
                            if (string.IsNullOrEmpty(currenPatient.COMMUNE_CODE) && string.IsNullOrEmpty(currenPatient.DISTRICT_CODE) && string.IsNullOrEmpty(currenPatient.PROVINCE_CODE))
                            {

                                DialogResult drReslt = DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân thiếu thông tin địa chỉ", "Thông báo!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);

                            }
                        }
                    }

                    dataHein.FineYearMonthDate = this.ResultDataADO.ResultHistoryLDO.ngayDu5Nam;
                    Inventec.Common.Logging.LogSystem.Info("CheckTTProcessResultData => 1");
                    //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                    if (ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                    {
                        Inventec.Common.Logging.LogSystem.Info("CheckTTProcessResultData => 2");
                        if (this.uCMainHein != null && this.ucHein__BHYT != null)
                        {
                            this.uCMainHein.FillDataAfterCheckBHYT(this.ucHein__BHYT, dataHein);
                        }
                    }

                    if (ResultDataADO.IsToDate)
                    {
                        if (this.uCMainHein != null && this.ucHein__BHYT != null)
                        {
                            this.uCMainHein.FillDataAfterCheckBHYT(this.ucHein__BHYT, ResultDataADO.HeinCardData);
                        }

                        Inventec.Common.Logging.LogSystem.Debug("Ket thuc gan du lieu cho benh nhan khi doc the va khong co han den");
                    }

                    if (ResultDataADO.IsThongTinNguoiDungThayDoiSoVoiCong__Choose)
                    {
                        Inventec.Common.Logging.LogSystem.Info("CheckTTProcessResultData => 3");
                        if (this.uCMainHein != null && this.ucHein__BHYT != null)
                        {
                            this.uCMainHein.FillDataAfterCheckBHYT(this.ucHein__BHYT, dataHein);
                        }
                    }

                    if (HisConfigCFG.IsCheckExamHistory && (ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose || ResultDataADO.SuccessWithoutMessage))
                    {
                        Inventec.Common.Logging.LogSystem.Info("Mo form lich su voi data rsIns");
                        frmCheckHeinCardGOV frm = new frmCheckHeinCardGOV(ResultDataADO.ResultHistoryLDO);
                        frm.ShowDialog();
                    }

                    Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 3");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HeinCardData GetDataQrCodeHeinCard(string qrCode)
        {
            HeinCardData dataHein = null;
            try
            {
                //Lay thong tin tren th BHYT cua benh nhan khi quet the doc chuoi qrcode
                ReadQrCodeHeinCard readQrCode = new ReadQrCodeHeinCard();
                dataHein = readQrCode.ReadDataQrCode(qrCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dataHein;
        }

        private CccdCardData GetDataQrCodeCccdCard(string qrCode)
        {
            CccdCardData dataCccd = null;
            try
            {
                dataCccd = ReadQrCodeCCCD.ReadDataQrCode(qrCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dataCccd;
        }

        private void LoadPrimaryPatientType()
        {
            try
            {
                primaryPatientTypes = new List<HIS_PATIENT_TYPE>();
                var patyAlows = BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>();
                if (cboPatientType.EditValue != null)
                {
                    long patyId = Convert.ToInt64(cboPatientType.EditValue);
                    primaryPatientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1 && p.ID != patyId && patyAlows != null && patyAlows.Any(a => a.PATIENT_TYPE_ID == patyId && a.PATIENT_TYPE_ALLOW_ID == p.ID)).ToList();
                }
                if (this.cboPrimaryPatientType.EditValue != null && (this.cboPatientType.EditValue == null || Convert.ToInt64(this.cboPatientType.EditValue) == Convert.ToInt64(cboPrimaryPatientType.EditValue)))
                {
                    this.cboPrimaryPatientType.EditValue = null;
                }
                if (this.cboPrimaryPatientType.EditValue != null && (primaryPatientTypes == null || !primaryPatientTypes.Any(a => a.ID == Convert.ToInt64(this.cboPrimaryPatientType.EditValue))))
                {
                    this.cboPrimaryPatientType.EditValue = null;
                }

                PatientTypeLoader.LoadDataToCombo(this.cboPrimaryPatientType, primaryPatientTypes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPrimaryPatientTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (!String.IsNullOrWhiteSpace(strValue))
                    {
                        strValue = strValue.ToLower();
                        var listData = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1 && p.IS_NOT_USE_FOR_PATIENT != 1 && p.PATIENT_TYPE_CODE.ToLower().Contains(strValue)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            cboPrimaryPatientType.EditValue = listData[0].ID;
                            this.lciQrcode.Enabled = false;
                            if (Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()) == HisConfigCFG.PatientTypeId__BHYT)
                            {
                                uCMainHein.DefaultFocusUserControl(ucHein__BHYT);
                                this.lciQrcode.Enabled = true;
                            }
                        }
                        else
                        {
                            cboPrimaryPatientType.Focus();
                            cboPrimaryPatientType.ShowPopup();
                        }
                    }
                    else
                    {
                        cboPrimaryPatientType.Focus();
                        cboPrimaryPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrimaryPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    this.lciQrcode.Enabled = false;
                    if (Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()) == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        uCMainHein.DefaultFocusUserControl(ucHein__BHYT);
                        this.lciQrcode.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrimaryPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboPrimaryPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrimaryPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPrimaryPatientTypeCode.Text = "";
                if (cboPrimaryPatientType.EditValue != null)
                {
                    cboPrimaryPatientType.Properties.Buttons[1].Visible = true;
                    var paty = this.primaryPatientTypes != null ? this.primaryPatientTypes.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPrimaryPatientType.EditValue)) : null;
                    if (paty != null)
                    {
                        txtPrimaryPatientTypeCode.Text = paty.PATIENT_TYPE_CODE;
                    }
                }
                else
                {
                    cboPrimaryPatientType.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboClassify_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                dxValidationProvider1.SetValidationRule(cboMilitaryRank, null);
                dxValidationProvider1.SetValidationRule(cboPosition, null);
                dxValidationProvider1.SetValidationRule(cboWorkPlace, null);
                if (cboClassify.EditValue == null)
                    return;
                var PatientType = (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE)cboPatientType.Properties.GetDataSourceRowByKeyValue(cboPatientType.EditValue);
                if (!IsFirstLoadClassify && PatientType.IS_COPAYMENT != 1)
                {
                    if (!chkAutoUpdateType.Checked)
                        LoadDataToGridSereServ(currentTreatmentLogSDO, this.lstTreatmentLog);
                }
                cboClassify.Properties.Buttons[1].Visible = true;
                var patientClassify = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().FirstOrDefault(o => o.ID == Int64.Parse(this.cboClassify.EditValue.ToString()));
                if (patientClassify != null && patientClassify.IS_POLICE == 1 && !IsVisibleClassify)
                {
                    if (currentNameControl != null && currentNameControl.Count > 0)
                    {
                        string bnW = layoutControl1.Name + ".Root." + layoutControlItem10.Name;
                        string bnM = layoutControl1.Name + ".Root." + layoutControlItem11.Name;
                        string bnP = layoutControl1.Name + ".Root." + layoutControlItem12.Name;

                        if (!currentNameControl.Contains(bnW))
                        {
                            layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            ValidationSingleControl(cboMilitaryRank);
                        }

                        if (!currentNameControl.Contains(bnM))
                        {
                            layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            ValidationSingleControl(cboPosition);
                        }

                        if (!currentNameControl.Contains(bnP))
                        {
                            layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            ValidWorkPlace();
                        }
                    }
                    else
                    {
                        EnableControlCombo(true);
                        ValidationSingleControl(cboMilitaryRank);
                        ValidationSingleControl(cboPosition);
                        ValidWorkPlace();

                    }
                }
                IsFirstLoadClassify = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboClassify_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboClassify.Properties.Buttons[1].Visible = false;
                    cboClassify.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMilitaryRank_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMilitaryRank.Properties.Buttons[1].Visible = false;
                    cboMilitaryRank.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPosition_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPosition.Properties.Buttons[1].Visible = false;
                    cboPosition.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkPlace_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboWorkPlace.Properties.Buttons[2].Visible = false;
                    cboWorkPlace.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisWorkPlace").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisWorkPlace'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisWorkPlace' is not plugins");

                    List<object> listArgs = new List<object>();
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();

                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>();
                    this.LoadWorkPlace();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMilitaryRank_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMilitaryRank.EditValue != null)
                    cboMilitaryRank.Properties.Buttons[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPosition_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPosition.EditValue != null)
                    cboPosition.Properties.Buttons[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkPlace_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboWorkPlace.EditValue != null)
                    cboWorkPlace.Properties.Buttons[2].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoUpdateType.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoUpdateType.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoUpdateType.Name;
                    csAddOrUpdate.VALUE = (chkAutoUpdateType.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkAutoUpdateType.Name)
                        {
                            chkAutoUpdateType.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate("Mps000473", this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000473":
                        LoadBieuMau(printTypeCode, fileName, ref result);
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
        private void LoadBieuMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (currentHisTreatment == null)
                    return;
                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER, HIS_PATIENT_TYPE_ALTER>();
                HIS_PATIENT_TYPE_ALTER patientTypeAlter = AutoMapper.Mapper.Map<V_HIS_PATIENT_TYPE_ALTER, HIS_PATIENT_TYPE_ALTER>(currentTreatmentLogSDO.patientTypeAlter);

                CommonParam param = new CommonParam();
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = currentHisTreatment.ID;
                var treatmentRs = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param);

                HisBedLogViewFilter filter = new HisBedLogViewFilter();
                filter.TREATMENT_ID = currentHisTreatment.ID;
                var BedLogRs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, filter, param);

                HisServiceReqFilter serFilter = new HisServiceReqFilter();
                serFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                serFilter.IS_MAIN_EXAM = true;
                var serviceReqLog = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serFilter, param);
                MPS.Processor.Mps000473.PDO.Mps000473PDO pdo = new MPS.Processor.Mps000473.PDO.Mps000473PDO(currenPatient, patientTypeAlter, treatmentRs.First(), (BedLogRs != null && BedLogRs.Count > 0) ? BedLogRs.OrderByDescending(o => o.ID).First() : null, serviceReqLog != null ? serviceReqLog.First() : null, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(currentHisTreatment.TREATMENT_CODE, printTypeCode, this.module != null ? module.RoomId : 0);

                if (ConfigApplicationWorker.Get<string>("CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW") == "1")
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void EnableBtnPrint()
        {
            try
            {
                if (currentHisTreatment.IS_EMERGENCY == (short?)1)
                    btnPrint.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                if (uCMainHein != null)
                    uCMainHein.DisposeControl(ucHein__BHYT);
                if (ucImageBHYT != null)
                    ucImageBHYT.DisposeControl();
                if (ucKskContract != null)
                    ucKskContract.DisposeControl();
                lstSereServ = null;
                newLstSerSev = null;
                MesError = null;
                dicSevicepatyAllows = null;
                ModuleLinkName = null;
                currentControlStateRDO = null;
                controlStateWorker = null;
                IsHasEmergency = false;
                primaryPatientTypes = null;
                IsVisibleClassify = false;
                baseNameControl = null;
                branch = null;
                currentNameControl = null;
                APP_CODE__EXACT = null;
                ModuleLinkName = null;
                currentHideControls = null;
                IsLoadForm = false;
                IsFirstLoadClassify = false;
                isEdit = false;
                resultSuccess = false;
                _IsShowLsKcb = false;
                currenPatient = null;
                dataClassify = null;
                dataMilitaryRank = null;
                dataWorkPlace = null;
                dataPosition = null;
                keyIsSetPrimaryPatientType = 0;
                patientId = 0;
                ucImageBHYT = null;
                ucKskContract = null;
                currentTreatmentSave = null;
                lstSereServResult = null;
                lstTreatmentLog = null;
                resultPatientTypeAlter = null;
                resultApi = null;
                provindcode = null;
                positionHandleControl = 0;
                IsView = null;
                ActionType = 0;
                ucHein__BHYT = null;
                uCMainHein = null;
                treatmentId = 0;
                RefeshReference = null;
                currentHisTreatment = null;
                module = null;
                currentTreatmentLogSDO = null;
                IsRuning = false;
                _HisTreatment = null;
                listResult = null;
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.chkAutoUpdateType.CheckedChanged -= new System.EventHandler(this.checkEdit1_CheckedChanged);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.cboWorkPlace.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboWorkPlace_ButtonClick);
                this.cboWorkPlace.EditValueChanged -= new System.EventHandler(this.cboWorkPlace_EditValueChanged);
                this.cboPosition.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPosition_ButtonClick);
                this.cboPosition.EditValueChanged -= new System.EventHandler(this.cboPosition_EditValueChanged);
                this.cboMilitaryRank.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboMilitaryRank_ButtonClick);
                this.cboMilitaryRank.EditValueChanged -= new System.EventHandler(this.cboMilitaryRank_EditValueChanged);
                this.cboClassify.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboClassify_ButtonClick);
                this.cboClassify.EditValueChanged -= new System.EventHandler(this.cboClassify_EditValueChanged);
                this.cboPrimaryPatientType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPrimaryPatientType_Closed);
                this.cboPrimaryPatientType.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPrimaryPatientType_ButtonClick);
                this.cboPrimaryPatientType.EditValueChanged -= new System.EventHandler(this.cboPrimaryPatientType_EditValueChanged);
                this.txtPrimaryPatientTypeCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPrimaryPatientTypeCode_PreviewKeyDown);
                this.dtLogTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtLogTime_Closed);
                this.cboPatientType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPatientType_Closed);
                this.cboPatientType.EditValueChanged -= new System.EventHandler(this.cboPatientType_EditValueChanged);
                this.cboTreatmentType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTreatmentType_Closed);
                this.cboTreatmentType.EditValueChanged -= new System.EventHandler(this.cboTreatmentType_EditValueChanged);
                this.txtPatientType.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtPatientType_KeyDown_1);
                this.txtTreatmentTypeCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtTreatmentTypeCode_KeyDown);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.txtQrcode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtQrcode_KeyDown);
                this.Load -= new System.EventHandler(this.frmPatientTypeAlter_Load);
                gridView1.GridControl.DataSource = null;
                cboClassify.Properties.DataSource = null;
                gridLookUpEdit2View.GridControl.DataSource = null;
                cboMilitaryRank.Properties.DataSource = null;
                gridLookUpEdit3View.GridControl.DataSource = null;
                cboPosition.Properties.DataSource = null;
                gridLookUpEdit4View.GridControl.DataSource = null;
                cboWorkPlace.Properties.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboPrimaryPatientType.Properties.DataSource = null;
                cboTreatmentType.Properties.DataSource = null;
                cboPatientType.Properties.DataSource = null;
                layoutControlItem15 = null;
                layoutControlItem14 = null;
                chkAutoUpdateType = null;
                btnPrint = null;
                emptySpaceItem2 = null;
                layoutControlItem13 = null;
                layoutControlItem12 = null;
                layoutControlItem11 = null;
                layoutControlItem10 = null;
                layoutControlItem9 = null;
                gridView1 = null;
                cboClassify = null;
                gridLookUpEdit2View = null;
                cboMilitaryRank = null;
                gridLookUpEdit3View = null;
                cboPosition = null;
                gridLookUpEdit4View = null;
                cboWorkPlace = null;
                txtWorkplace = null;
                emptySpaceItem3 = null;
                lciComboPrimaryPatientType = null;
                lciPrimaryPatientType = null;
                txtPrimaryPatientTypeCode = null;
                gridLookUpEdit1View = null;
                cboPrimaryPatientType = null;
                lciQrcode = null;
                txtQrcode = null;
                layoutControlItem8 = null;
                panelControlImageBHYT = null;
                layoutControlItem2 = null;
                dtLogTime = null;
                barButtonItem1 = null;
                bar2 = null;
                barManager1 = null;
                barDockControlTop = null;
                barDockControlBottom = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                dxErrorProvider1 = null;
                dxValidationProvider1 = null;
                layoutControlItem7 = null;
                layoutControlItem6 = null;
                cboTreatmentType = null;
                cboPatientType = null;
                layoutControlItem5 = null;
                layoutControlItem1 = null;
                txtTreatmentTypeCode = null;
                txtPatientType = null;
                emptySpaceItem1 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                btnSave = null;
                xclHeinCardInformation = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public long currentLogTime { get; set; }
        public long currentTreatmentType { get; set; }
        private void dtLogTime_Leave(object sender, EventArgs e)
        {
            try
            {
                if (uCMainHein != null && ucHein__BHYT != null)
                {
                    var dt = dtLogTime.EditValue != null ? Int64.Parse(dtLogTime.DateTime.ToString("yyyyMMdd0000")) : 0;
                    if (currentLogTime == 0 || currentLogTime != dt)
                        uCMainHein.SetValueLogTime(ucHein__BHYT, dt);
                    currentLogTime = dt;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uCMainHein != null && ucHein__BHYT != null)
                {
                    uCMainHein.ShowComboSoThe(ucHein__BHYT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
