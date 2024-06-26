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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AddExamInfor;
using HIS.Desktop.Plugins.TrackingCreate.ADO;
using HIS.Desktop.Plugins.TrackingCreate.Config;
using HIS.Desktop.Plugins.TrackingCreate.frmGhiChu;
using HIS.Desktop.Plugins.TrackingCreate.Resources;
using HIS.Desktop.Plugins.TrackingCreate.ValidationRuleControl;
using HIS.Desktop.Utility;
using HIS.UC.DHST;
using HIS.UC.DHST.ADO;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackingCreate
{
    public partial class frmTrackingCreate : FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        int action = -1;
        internal HIS_TRACKING currentTracking { get; set; }
        internal List<long> serviceReqIds;
        internal HisTrackingSDO trackingSDOs;
        internal HIS_TRACKING trackingOutSave;
        HIS_DHST _Dhst { get; set; }
        internal bool _IsMaterial { get; set; }

        internal List<HIS_SERVICE_REQ> _ServiceReqByTrackings = new List<HIS_SERVICE_REQ>();
        List<long> serviceReqIdsIncludeByTrackingCreated;
        List<long> careIdsIncludeByTrackingCreated;
        List<HIS_TRACKING_TEMP> trackingTemps;

        List<HIS_TRACKING> trackingOlds;
        List<HisTrackingADO> HisTrackingADO;

        IcdProcessor icdProcessor;
        UserControl ucIcd;
        IcdProcessor icdYhctProcessor;
        UserControl ucIcdYhct;
        int positionHandleControl = -1;

        long treatmentId = 0;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.TrackingCreate";
        SDA_CONFIG_APP _currentConfigApp;
        SDA_CONFIG_APP_USER currentConfigAppUser;
        ConfigADO _ConfigADO;
        string trackingCreateOptionCFG = "";

        internal SecondaryIcdProcessor subIcdYhctProcessor;
        internal UserControl ucSecondaryIcdYhct;

        internal DHSTProcessor dhstProcessor;

        UserControl ucControlDHST;

        Action<HIS_TRACKING> actCallBack;
        public frmTrackingCreate()
        {
            InitializeComponent();
            InitUcIcd();
            InitUcIcdYhct();
            InitUcSecondaryIcdYhct();
            updateSizeControl();
        }

        public frmTrackingCreate(Inventec.Desktop.Common.Modules.Module currentModule, HIS_TRACKING hisTracking, Action<HIS_TRACKING> actCallBack = null)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                InitUcIcd();
                InitUcIcdYhct();
                InitUcSecondaryIcdYhct();
                this.currentModule = currentModule;
                this.currentTracking = hisTracking;
                this.treatmentId = hisTracking.TREATMENT_ID;
                this.action = GlobalVariables.ActionEdit;
                this.actCallBack = actCallBack;
                updateSizeControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTrackingCreate(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, Action<HIS_TRACKING> actCallBack = null)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                InitUcIcd();
                InitUcIcdYhct();
                InitUcSecondaryIcdYhct();
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                this.action = GlobalVariables.ActionAdd;
                this.actCallBack = actCallBack;
                updateSizeControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTrackingCreate(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, HIS_DHST currentDhst, Action<HIS_TRACKING> actCallBack = null)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                InitUcIcd();
                InitUcIcdYhct();
                InitUcSecondaryIcdYhct();
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                this.action = GlobalVariables.ActionAdd;
                this._Dhst = currentDhst;
                this.actCallBack = actCallBack;
                updateSizeControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTrackingCreate_Load(object sender, EventArgs e)
        {
            try
            {
                this.trackingCreateOptionCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_CREATE_OPTION);
                SetIconFrm();
                SetCaptionByLanguageKey();
                SetValueDefaultControl();
                InitUCDHST();
                ValidControl();
                CheckConfigIsMaterial();
                trackingTemps = LoadTrackingTemp();
                InitComboTrackingTemp(trackingTemps);
                HisTrackingADO = LoadTrackingOld();
                InitComboTrackingOld(HisTrackingADO);
                icdProcessor.FocusControl(ucIcd);
                InitControlState();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                //dtExecuteTimeDhst.DateTime = DateTime.Now;

                LoadAndFillIcdByTreatment();//tu dong fill icd ho so dieu tri

                // CheckRoomIsSurgery();

                if (currentTracking != null && currentTracking.ID > 0)
                {
                    FillDataToControlByHisTracking(currentTracking);
                }
                this.EnableButton((currentTracking != null && currentTracking.ID > 0));
                if (Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_SHOWLASTEST_DHST)) == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    if (this._Dhst != null && treatmentId > 0)
                    {
                        MOS.Filter.HisDhstFilter dhstFilter1 = new MOS.Filter.HisDhstFilter();
                        dhstFilter1.TREATMENT_ID = treatmentId;
                        var rsDhst = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter1, new CommonParam());
                        HIS_DHST rs1 = new HIS_DHST();
                        rs1 = rsDhst.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).FirstOrDefault();
                        FillDataDhstToControl(rs1);
                        //FillDataDhstToControl(this._Dhst);
                    }
                    else if (this._Dhst == null && treatmentId > 0)
                    {
                        MOS.Filter.HisDhstFilter dhstFilter1 = new MOS.Filter.HisDhstFilter();
                        dhstFilter1.TREATMENT_ID = treatmentId;
                        var rsDhst = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter1, new CommonParam());
                        HIS_DHST rs1 = new HIS_DHST();
                        if (rsDhst != null && rsDhst.Count > 0)
                        {
                            rs1 = rsDhst.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).FirstOrDefault();

                            if (rs1 != null)
                            {
                                FillDataDhstToControl(rs1);
                            }
                        }
                    }
                }
                else
                {
                    FillDataDhstToControl(null);
                }


                LoadDataSS(false);

                //LoadTreatment();

                treeListServiceReq.ToolTipController = toolTipController1;
                LoadConfigHisAcc();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HisTrackingADO> LoadTrackingOld()
        {
            List<HisTrackingADO> result = new List<HisTrackingADO>();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTrackingFilter trackingFilter = new MOS.Filter.HisTrackingFilter();
                trackingFilter.TREATMENT_ID = this.treatmentId;
                trackingFilter.ORDER_FIELD = "TRACKING_TIME";
                trackingFilter.ORDER_DIRECTION = "DESC";
                trackingOlds = new BackendAdapter(param).Get<List<HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GET, ApiConsumers.MosConsumer, trackingFilter, param);
                if (trackingOlds != null && trackingOlds.Count > 0)
                {
                    foreach (var item in trackingOlds)
                    {
                        HisTrackingADO AddItem = new HisTrackingADO();

                        Inventec.Common.Mapper.DataObjectMapper.Map<HisTrackingADO>(AddItem, item);
                        AddItem.ID = item.ID;
                        AddItem.DEPARTMENT_NAME = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == item.DEPARTMENT_ID).DEPARTMENT_NAME;
                        AddItem.TRACKING_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.TRACKING_TIME);

                        result.Add(AddItem);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InitComboTrackingOld(List<HisTrackingADO> data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("InitComboTrackingOld: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CREATOR", "Người tạo", 250, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa tạo", 600, 2));
                columnInfos.Add(new ColumnInfo("TRACKING_TIME_STR", "Thời gian", 350, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, true, 1200);
                ControlEditorLoader.Load(this.cboTrackingOld, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void updateSizeControl()
        {
            try
            {
                //Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;
                //if (activeScreenDimensions != null)
                //{
                //    if (activeScreenDimensions.Width > 1366)
                //    {

                //        emptySpaceItem7.Width = 124;
                //        emptySpaceItem14.Width = 124;
                //        emptySpaceItem15.Width = 124;
                //        emptySpaceItem16.Width = 124;
                //        emptySpaceItem17.Width = 124;
                //    }
                //    else
                //    {
                //        emptySpaceItem7.Width = 54;
                //        emptySpaceItem14.Width = 54;
                //        emptySpaceItem15.Width = 54;
                //        emptySpaceItem16.Width = 54;
                //        emptySpaceItem17.Width = 54;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstan.chkPrint)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkSign)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkPrintDocumentSigned)
                        {
                            chkPrintDocumentSigned.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void LoadTreatment()
        //{
        //    try
        //    {
        //        this._Treatment = new HIS_TREATMENT();
        //        MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
        //        treatmentFilter.ID = this.treatmentId;
        //        this._Treatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, null).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //Ktra phong mo ra la phong PTTT hay k ?
        //Neu true thy ktra xem co tao tdt chua thỳ cho load va in lai
        private void CheckRoomIsSurgery()
        {
            try
            {
                if (this.currentModule != null && this.currentModule.RoomId > 0 && this.treatmentId > 0)
                {
                    var room = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p =>
                        p.ROOM_ID == this.currentModule.RoomId
                        && p.IS_SURGERY == 1
                        );
                    if (room != null)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisTrackingFilter trackingFilter = new MOS.Filter.HisTrackingFilter();
                        trackingFilter.TREATMENT_ID = this.treatmentId;
                        var workPlace = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId);
                        if (workPlace != null)
                        {
                            trackingFilter.DEPARTMENT_ID = workPlace.DepartmentId;
                        }
                        var result = new BackendAdapter(param).Get<List<HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GET, ApiConsumers.MosConsumer, trackingFilter, param);
                        if (result != null)
                        {
                            this.currentTracking = result.FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void CheckConfigIsMaterial()
        {
            try
            {
                long configQY7 = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MATERIAL));
                if (configQY7 != 1)
                {
                    this._IsMaterial = true;
                }
                else
                {
                    this._IsMaterial = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetValueDefaultControl()
        {
            try
            {
                dtTimeFrom.EditValue = DateTime.Now;
                dtTimeTo.EditValue = DateTime.Now;
                dtTrackingTime.EditValue = DateTime.Now;
                //dtExecuteTimeDhst.EditValue = null;

                //spinBelly.EditValue = null;
                //spinBloodPressureMax.EditValue = null;
                //spinBloodPressureMin.EditValue = null;
                //spinBreathRate.EditValue = null;
                //spinChest.EditValue = null;
                //spinHeight.EditValue = null;
                //spinPulse.EditValue = null;
                //spinTemperature.EditValue = null;
                //spinWeight.EditValue = null;
                //spinDuongMau.EditValue = null;
                //spinEditSpo2.EditValue = null;
                //txtNote.Text = "";

                txtSheetOrder.Text = "";

                btnAssService.Enabled = false;
                btnKeDonThuoc.Enabled = false;
                btnKeDonYHCT.Enabled = false;
                btnTuTruc.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_TRACKING_TEMP> LoadTrackingTemp()
        {
            List<HIS_TRACKING_TEMP> result = new List<HIS_TRACKING_TEMP>();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTrackingTempFilter trackingFilter = new MOS.Filter.HisTrackingTempFilter();
                trackingFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                trackingFilter.ORDER_FIELD = "TRACKING_TEMP_NAME";
                trackingFilter.ORDER_DIRECTION = "ASC";

                result = new BackendAdapter(param).Get<List<HIS_TRACKING_TEMP>>("api/HisTrackingTemp/Get", ApiConsumer.ApiConsumers.MosConsumer, trackingFilter, param);

                if (result != null && result.Count() > 0)
                {
                    var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).DepartmentId;

                    result = result.Where(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == DepartmentID || o.IS_PUBLIC == 1).ToList();
                }

            }
            catch (Exception ex)
            {
                result = new List<HIS_TRACKING_TEMP>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InitComboTrackingTemp(List<HIS_TRACKING_TEMP> db)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TRACKING_TEMP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TRACKING_TEMP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TRACKING_TEMP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboTrackingTemp, db, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TrackingCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.TrackingCreate.frmTrackingCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageDhst.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.xtraTabPageDhst.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabEye.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.xtraTabEye.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.barButtonItemPrint.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.barButtonItemPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.barButtonItemSearch.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.barButtonItemSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bbtnTrackingTemp.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.bbtnTrackingTemp.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.barButtonItem__AssService.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.barButtonItem__AssService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.barButtonItem__KeDOnThuoc.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.barButtonItem__KeDOnThuoc.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyeTensionRirght.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciEyeTensionRirght.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightRight.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciEyesightRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightGlassRight.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciEyesightGlassRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightLeft.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciEyesightLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightGlassLeft.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciEyesightGlassLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyeTensionLeft.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciEyeTensionLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabMental.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.xtraTabMental.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGeneralExpression.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciGeneralExpression.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOrientationCapacity.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciOrientationCapacity.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciContentOfThinking.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciContentOfThinking.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAwarenessBehavior.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciAwarenessBehavior.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEmotion.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciEmotion.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPerception.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciPerception.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInstinctivelyBehavior.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciInstinctivelyBehavior.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMemory.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciMemory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConcentration.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciConcentration.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIntellectual.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciIntellectual.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabOther.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.xtraTabOther.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCardiovascular.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciCardiovascular.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRespiratory.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciRespiratory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlIcdYhct.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlIcdYhct.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintDocumentSigned.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.chkPrintDocumentSigned.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSign.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.chkSign.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrint.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.chkPrint.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTuTruc.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnTuTruc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnAddExamInfo.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnAddExamInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKeDonThuoc.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnKeDonThuoc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKeDonYHCT.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnKeDonYHCT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoseResult.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnChoseResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtResultCLS.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTrackingCreate.txtResultCLS.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAssService.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnAssService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveTemp.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnSaveTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTrackingTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTrackingCreate.cboTrackingTemp.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlUcIcd.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlUcIcd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreate.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeFrom.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.lciTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtIcdExtraName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTrackingCreate.txtIcdExtraName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChamSoc.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnChamSoc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem43.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem43.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem45.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem45.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem37.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreate.layoutControlItem37.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableButton(bool enabled)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackingCreateOptionCFG), trackingCreateOptionCFG)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => enabled), enabled));
                if (this.trackingCreateOptionCFG == "1")
                {
                    this.btnKeDonMau.Enabled = enabled;
                    this.btnKeDonThuoc.Enabled = enabled;
                    this.btnKeDonYHCT.Enabled = enabled;
                    this.btnTuTruc.Enabled = enabled;
                    this.btnChamSoc.Enabled = enabled;
                    this.btnAssService.Enabled = enabled;
                }
                else if (this.trackingCreateOptionCFG == "2")
                {
                    this.btnKeDonMau.Enabled = !enabled;
                    this.btnKeDonThuoc.Enabled = !enabled;
                    this.btnKeDonYHCT.Enabled = !enabled;
                    this.btnTuTruc.Enabled = !enabled;
                    this.btnChamSoc.Enabled = !enabled;
                    this.btnAssService.Enabled = !enabled;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = FocusTxtIcdYhct;

                Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;

                if (activeScreenDimensions != null)
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("activeScreenDimensions.Width: ", activeScreenDimensions.Width));
                    if (activeScreenDimensions.Width > 1366)
                    {
                        ado.Width = 960;
                    }
                    else
                    {
                        ado.Width = 650;
                    }
                }
                ado.Height = 24;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>();
                this.ucIcd = (UserControl)icdProcessor.Run(ado);

                if (this.ucIcd != null)
                {
                    this.layoutControlUcIcd.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusTxtIcdYhct()
        {
            try
            {
                icdYhctProcessor.FocusControl(ucIcdYhct);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcdYhct()
        {
            try
            {
                icdYhctProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = FocusTxtIcdExtraCode;

                Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;

                if (activeScreenDimensions != null)
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("activeScreenDimensions.Width: ", activeScreenDimensions.Width));
                    if (activeScreenDimensions.Width > 1366)
                    {
                        ado.Width = 960;
                    }
                    else
                    {
                        ado.Width = 650;
                    }
                }
                ado.Height = 24;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
                ado.LblIcdMain = "CĐ YHCT";
                ado.ToolTipsIcdMain = "Chẩn đoán y học cổ truyền";
                this.ucIcdYhct = (UserControl)icdYhctProcessor.Run(ado);

                if (this.ucIcdYhct != null)
                {
                    this.layoutControlIcdYhct.Controls.Add(this.ucIcdYhct);
                    this.ucIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusTxtIcdExtraCode()
        {
            try
            {
                txtIcdExtraCode.Focus();
                txtIcdExtraCode.SelectAll();
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
                var icdYhct = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
                subIcdYhctProcessor = new SecondaryIcdProcessor(new CommonParam(), icdYhct);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcdToDo;
                ado.DelegateGetIcdMain = GetIcdMainCodeYhct;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ YHCT phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán y học cổ truyền kèm theo";
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcdYhct = (UserControl)subIcdYhctProcessor.Run(ado);

                if (ucSecondaryIcdYhct != null)
                {
                    this.panelSubIcdYhct.Controls.Add(ucSecondaryIcdYhct);
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

        private void NextForcusSubIcdToDo()
        {
            try
            {
                txtContent.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataSS(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAndFillIcdByTreatment()
        {
            try
            {
                if (this.treatmentId > 0)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                    filter.ID = this.treatmentId;
                    var rs = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param);
                    if (rs != null && rs.Count > 0)
                    {
                        this._Treatment = rs.FirstOrDefault();
                        HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                        //icd.ICD_ID = rs.FirstOrDefault().ICD_ID;
                        icd.ICD_CODE = _Treatment.ICD_CODE;
                        icd.ICD_NAME = _Treatment.ICD_NAME;
                        if (ucIcd != null)
                        {
                            icdProcessor.Reload(ucIcd, icd);
                        }
                        txtIcdExtraName.Text = _Treatment.ICD_TEXT;
                        txtIcdExtraCode.Text = _Treatment.ICD_SUB_CODE;

                        HIS.UC.Icd.ADO.IcdInputADO icdYhct = new HIS.UC.Icd.ADO.IcdInputADO();
                        icdYhct.ICD_CODE = _Treatment.TRADITIONAL_ICD_CODE;
                        icdYhct.ICD_NAME = _Treatment.TRADITIONAL_ICD_NAME;
                        if (ucIcdYhct != null)
                        {
                            icdYhctProcessor.Reload(ucIcdYhct, icdYhct);
                        }

                        if (!String.IsNullOrWhiteSpace(_Treatment.TRADITIONAL_ICD_SUB_CODE) || !String.IsNullOrWhiteSpace(_Treatment.TRADITIONAL_ICD_TEXT))
                        {
                            SecondaryIcdDataADO subYhctIcd = new SecondaryIcdDataADO();
                            subYhctIcd.ICD_SUB_CODE = _Treatment.TRADITIONAL_ICD_SUB_CODE;
                            subYhctIcd.ICD_TEXT = _Treatment.TRADITIONAL_ICD_TEXT;
                            if (ucSecondaryIcdYhct != null)
                            {
                                subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, subYhctIcd);
                            }
                        }
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Event from
        private void FillDataToControlByHisTracking(HIS_TRACKING data)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (data != null)
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                    icd.ICD_NAME = data.ICD_NAME;
                    icd.ICD_CODE = data.ICD_CODE;
                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }

                    if (!String.IsNullOrWhiteSpace(data.TRADITIONAL_ICD_CODE) || !String.IsNullOrWhiteSpace(data.TRADITIONAL_ICD_NAME))
                    {
                        HIS.UC.Icd.ADO.IcdInputADO icdYhct = new HIS.UC.Icd.ADO.IcdInputADO();
                        icdYhct.ICD_NAME = data.TRADITIONAL_ICD_NAME;
                        icdYhct.ICD_CODE = data.TRADITIONAL_ICD_CODE;
                        if (ucIcdYhct != null)
                        {
                            icdYhctProcessor.Reload(ucIcdYhct, icdYhct);
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(data.TRADITIONAL_ICD_SUB_CODE) || !String.IsNullOrWhiteSpace(data.TRADITIONAL_ICD_TEXT))
                    {
                        SecondaryIcdDataADO subYhctIcd = new SecondaryIcdDataADO();
                        subYhctIcd.ICD_SUB_CODE = data.TRADITIONAL_ICD_SUB_CODE;
                        subYhctIcd.ICD_TEXT = data.TRADITIONAL_ICD_TEXT;
                        if (ucSecondaryIcdYhct != null)
                        {
                            subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, subYhctIcd);
                        }
                    }

                    txtGeneralExpression.Text = data.GENERAL_EXPRESSION;
                    txtOrientationCapacity.Text = data.ORIENTATION_CAPACITY;
                    txtEmotion.Text = data.EMOTION;
                    txtPerception.Text = data.PERCEPTION;
                    txtContentOfThinking.Text = data.CONTENT_OF_THINKING;
                    txtAwarenessBehavior.Text = data.AWARENESS_BEHAVIOR;
                    txtInstinctivelyBehavior.Text = data.INSTINCTIVELY_BEHAVIOR;
                    txtMemory.Text = data.MEMORY;
                    txtIntellectual.Text = data.INTELLECTUAL;
                    txtConcentration.Text = data.CONCENTRATION;
                    txtCardiovascular.Text = data.CARDIOVASCULAR;
                    txtRespiratory.Text = data.RESPIRATORY;

                    txtEyeTensionLeft.Text = data.EYE_TENSION_LEFT;
                    txtEyeTensionRight.Text = data.EYE_TENSION_RIGHT;
                    txtEyesightGlassLeft.Text = data.EYESIGHT_GLASS_LEFT;
                    txtEyesightGlassRight.Text = data.EYESIGHT_GLASS_RIGHT;
                    txtEyesightLeft.Text = data.EYESIGHT_LEFT;
                    txtEyesightRight.Text = data.EYESIGHT_RIGHT;

                    txtIcdExtraName.Text = data.ICD_TEXT;
                    txtIcdExtraCode.Text = data.ICD_SUB_CODE;
                    txtContent.Text = data.CONTENT;
                    txtMedicalInstruction.Text = data.MEDICAL_INSTRUCTION;
                    txtResultCLS.Text = data.SUBCLINICAL_PROCESSES;
                    txtTheoDoiChamSoc.Text = data.CARE_INSTRUCTION;

                    dtTrackingTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TRACKING_TIME);
                    dtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TRACKING_TIME);
                    dtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TRACKING_TIME);

                    txtSheetOrder.Text = data.SHEET_ORDER.ToString();

                    //Load DHST
                    MOS.Filter.HisDhstFilter dhstFilter = new MOS.Filter.HisDhstFilter();
                    dhstFilter.TRACKING_ID = data.ID;
                    //dhstFilter.TREATMENT_ID = data.TREATMENT_ID;
                    var rsDhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);
                    HIS_DHST rs = new HIS_DHST();
                    if (rsDhst != null && rsDhst.Count > 0)
                    {
                        //rs = rsDhst.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).FirstOrDefault();
                        rs = rsDhst.FirstOrDefault();
                        this._Dhst = rs;
                    }
                    if (rs != null)
                    {
                        FillDataDhstToControl(rs);
                    }
                    btnPrint.Enabled = true;
                    if (trackingCreateOptionCFG == "1")
                    {
                        btnKeDonMau.Enabled = true;
                    }
                    else if (trackingCreateOptionCFG == "2")
                    {
                        btnKeDonMau.Enabled = false;
                    }
                    //btnChamSoc.Enabled = true;
                    //btnKeDonThuoc.Enabled = true;
                    //btnKeDonYHCT.Enabled = true;
                    //btnAssService.Enabled = true;
                    //btnTuTruc.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataDhstToControl(HIS_DHST data)
        {
            try
            {
                if (data != null && dhstProcessor != null)
                {
                    dhstProcessor.SetValue(ucControlDHST, data);



                    //spinPulse.EditValue = data.PULSE;
                    //spinTemperature.EditValue = data.TEMPERATURE;
                    //spinBloodPressureMax.EditValue = data.BLOOD_PRESSURE_MAX;
                    //spinBloodPressureMin.EditValue = data.BLOOD_PRESSURE_MIN;
                    //spinBreathRate.EditValue = data.BREATH_RATE;
                    //spinWeight.EditValue = data.WEIGHT;
                    //spinHeight.EditValue = data.HEIGHT;
                    //spinChest.EditValue = data.CHEST;
                    //spinBelly.EditValue = data.BELLY;
                    //spinDuongMau.EditValue = data.CAPILLARY_BLOOD_GLUCOSE;
                    //spinEditSpo2.EditValue = data.SPO2;
                    //if (data.EXECUTE_TIME != null)
                    //{
                    //    dtExecuteTimeDhst.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXECUTE_TIME ?? 0);
                    //}
                    //else
                    //    dtExecuteTimeDhst.DateTime = DateTime.Now;
                    //txtNote.Text = data.NOTE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdExtraName.Focus();
                    txtIcdExtraName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConten_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtMedicalInstruction_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //HIS.Desktop.Plugins.TrackingCreate.Demo.Form1 frm = new Demo.Form1();
                //frm.ShowDialog();
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                //this._Treatment = new HIS_TREATMENT();
                //MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                //treatmentFilter.ID = this.treatmentId;
                //var rsTreatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, null);
                if (this._Treatment != null && this._Treatment.ID > 0)
                {
                    //this._Treatment = rsTreatment.FirstOrDefault();
                    DateTime trackingTime = dtTrackingTime.DateTime;
                    DateTime inTimeTreatment = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this._Treatment.IN_TIME) ?? DateTime.Now;
                    if (trackingTime < inTimeTreatment)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.Validate_Tracking_Time, ResourceMessage.ThongBao);
                        dtTrackingTime.Focus();
                        dtTrackingTime.SelectAll();
                        return;
                    }
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                if (this.action == GlobalVariables.ActionAdd)
                {
                    GetDataToSave();

                    trackingOutSave = new HIS_TRACKING();
                    trackingOutSave = new BackendAdapter(param).Post<HIS_TRACKING>(HisRequestUriStore.HIS_TRACKING_CREATE, ApiConsumers.MosConsumer, this.trackingSDOs, param);
                    WaitingManager.Hide();
                    if (trackingOutSave != null)
                    {
                        success = true;
                        this.action = GlobalVariables.ActionEdit;

                        currentTracking = new HIS_TRACKING();
                        currentTracking = trackingOutSave;
                        btnPrint.Enabled = true;
                        if (trackingCreateOptionCFG == "1")
                        {
                            btnKeDonMau.Enabled = true;
                        }
                        else if (trackingCreateOptionCFG == "2")
                        {
                            btnKeDonMau.Enabled = false;
                        }
                        //btnChamSoc.Enabled = true;
                        //btnAssService.Enabled = true;
                        //btnKeDonThuoc.Enabled = true;
                        //btnKeDonYHCT.Enabled = true;
                        //btnTuTruc.Enabled = true;
                        ReloadDhst();
                        EnableButton(true);
                        if (chkPrint.Checked || chkSign.Checked)
                        {
                            PrintProcess(PrintType.IN_TO_DIEU_TRI);
                        }
                    }
                }
                else if (this.action == GlobalVariables.ActionEdit)
                {
                    GetDataToSave();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("api/HisTracking/Update this.trackingSDOs: ", this.trackingSDOs));
                    trackingOutSave = new HIS_TRACKING();
                    trackingOutSave = new BackendAdapter(param).Post<HIS_TRACKING>(HisRequestUriStore.HIS_TRACKING_UPDATE, ApiConsumers.MosConsumer, this.trackingSDOs, param);
                    WaitingManager.Hide();
                    if (trackingOutSave != null)
                    {
                        success = true;
                        this.action = GlobalVariables.ActionEdit;
                        currentTracking = new HIS_TRACKING();
                        currentTracking = trackingOutSave;
                        UpdateServiceReqInstructionDateByTrackingTime();
                        if (chkPrint.Checked || chkSign.Checked)
                        {
                            PrintProcess(PrintType.IN_TO_DIEU_TRI);
                        }
                    }
                }

                HisTrackingADO = LoadTrackingOld();
                InitComboTrackingOld(HisTrackingADO);

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        HIS_TRACKING __AddExamInfo = new HIS_TRACKING();

        private void ProcessPrint()
        {
            try
            {
                ConfigADO ado = new ConfigADO();
                //richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                if (chkSign.Checked)
                {
                    ado.IsSign = "1";
                    //richEditorMain.RunPrintTemplate("Mps000178", DelegateRunPrinter);
                }

                if (chkPrintDocumentSigned.Checked)
                {
                    ado.IsPrintDocumentSigned = "1";
                    // richEditorMain.RunPrintTemplate("Mps000001", DelegateRunPrinter);
                }

                if (this._ConfigADO != null && (this._ConfigADO.IsSign != ado.IsSign || this._ConfigADO.IsPrintDocumentSigned != ado.IsPrintDocumentSigned))
                {
                    string value = Newtonsoft.Json.JsonConvert.SerializeObject(ado);

                    //Update cònig
                    SDA_CONFIG_APP_USER configAppUserUpdate = new SDA_CONFIG_APP_USER();
                    configAppUserUpdate.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    configAppUserUpdate.VALUE = value;
                    configAppUserUpdate.CONFIG_APP_ID = _currentConfigApp.ID;
                    if (currentConfigAppUser != null)
                        configAppUserUpdate.ID = currentConfigAppUser.ID;
                    string api = configAppUserUpdate.ID > 0 ? "api/SdaConfigAppUser/Update" : "api/SdaConfigAppUser/Create";
                    CommonParam param = new CommonParam();
                    var UpdateResult = new BackendAdapter(param).Post<SDA_CONFIG_APP_USER>(
                            api, ApiConsumers.SdaConsumer, configAppUserUpdate, param);

                    //if (UpdateResult != null)
                    //{
                    //    success = true;
                    //}

                    MessageManager.Show(this.ParentForm, param, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataToSave()
        {
            try
            {
                trackingSDOs = new MOS.SDO.HisTrackingSDO();
                trackingSDOs.ServiceReqs = new List<TrackingServiceReq>();
                HIS_TRACKING trackingSave = new HIS_TRACKING();
                var work = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId);
                if (this.currentModule != null && this.currentModule.RoomId > 0)
                {
                    trackingSDOs.WorkingRoomId = this.currentModule.RoomId;
                }
                if (work != null)
                {
                    trackingSave.DEPARTMENT_ID = work.DepartmentId;
                }
                if (trackingOutSave != null)
                {
                    trackingSave.ID = trackingOutSave.ID;
                }
                else if (currentTracking != null)
                {
                    trackingSave.ID = currentTracking.ID;
                }

                if (__AddExamInfo != null
                    && (!string.IsNullOrEmpty(__AddExamInfo.GENERAL_EXPRESSION)
                     || !string.IsNullOrEmpty(__AddExamInfo.ORIENTATION_CAPACITY)
                     || !string.IsNullOrEmpty(__AddExamInfo.EMOTION)
                     || !string.IsNullOrEmpty(__AddExamInfo.PERCEPTION)
                     || !string.IsNullOrEmpty(__AddExamInfo.AWARENESS_BEHAVIOR)
                     || !string.IsNullOrEmpty(__AddExamInfo.INSTINCTIVELY_BEHAVIOR)
                     || !string.IsNullOrEmpty(__AddExamInfo.CONTENT_OF_THINKING)
                     || !string.IsNullOrEmpty(__AddExamInfo.MEMORY)
                     || !string.IsNullOrEmpty(__AddExamInfo.INTELLECTUAL)
                     || !string.IsNullOrEmpty(__AddExamInfo.CONCENTRATION)
                     || !string.IsNullOrEmpty(__AddExamInfo.CARDIOVASCULAR)
                     || !string.IsNullOrEmpty(__AddExamInfo.RESPIRATORY))
                    )
                {
                    trackingSave.GENERAL_EXPRESSION = __AddExamInfo.GENERAL_EXPRESSION;
                    trackingSave.ORIENTATION_CAPACITY = __AddExamInfo.ORIENTATION_CAPACITY;
                    trackingSave.EMOTION = __AddExamInfo.EMOTION;
                    trackingSave.PERCEPTION = __AddExamInfo.PERCEPTION;
                    trackingSave.CONTENT_OF_THINKING = __AddExamInfo.CONTENT_OF_THINKING;
                    trackingSave.AWARENESS_BEHAVIOR = __AddExamInfo.AWARENESS_BEHAVIOR;
                    trackingSave.INSTINCTIVELY_BEHAVIOR = __AddExamInfo.INSTINCTIVELY_BEHAVIOR;
                    trackingSave.MEMORY = __AddExamInfo.MEMORY;
                    trackingSave.INTELLECTUAL = __AddExamInfo.INTELLECTUAL;
                    trackingSave.CONCENTRATION = __AddExamInfo.CONCENTRATION;
                    trackingSave.CARDIOVASCULAR = __AddExamInfo.CARDIOVASCULAR;
                    trackingSave.RESPIRATORY = __AddExamInfo.RESPIRATORY;
                }

                trackingSave.ICD_TEXT = txtIcdExtraName.Text;
                trackingSave.ICD_SUB_CODE = txtIcdExtraCode.Text;
                trackingSave.TRACKING_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime) ?? 0;
                trackingSave.TREATMENT_ID = this.treatmentId;
                trackingSave.CONTENT = txtContent.Text;
                trackingSave.MEDICAL_INSTRUCTION = txtMedicalInstruction.Text;
                trackingSave.CARE_INSTRUCTION = txtTheoDoiChamSoc.Text;
                if (ucIcd != null)
                {
                    var icdValue = icdProcessor.GetValue(ucIcd);
                    if (icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                    {
                        //trackingSave.ICD_ID = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_ID;
                        trackingSave.ICD_NAME = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                        trackingSave.ICD_CODE = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }

                if (ucIcdYhct != null)
                {
                    var icdValue = icdYhctProcessor.GetValue(ucIcdYhct);
                    if (icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                    {
                        //trackingSave.ICD_ID = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_ID;
                        trackingSave.TRADITIONAL_ICD_NAME = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                        trackingSave.TRADITIONAL_ICD_CODE = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }

                if (ucSecondaryIcdYhct != null)
                {
                    var subIcd = subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        trackingSave.TRADITIONAL_ICD_SUB_CODE = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        trackingSave.TRADITIONAL_ICD_TEXT = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }

                trackingSave.SUBCLINICAL_PROCESSES = txtResultCLS.Text;

                trackingSave.GENERAL_EXPRESSION = txtGeneralExpression.Text;
                trackingSave.ORIENTATION_CAPACITY = txtOrientationCapacity.Text;
                trackingSave.EMOTION = txtEmotion.Text;
                trackingSave.PERCEPTION = txtPerception.Text;
                trackingSave.CONTENT_OF_THINKING = txtContentOfThinking.Text;
                trackingSave.AWARENESS_BEHAVIOR = txtAwarenessBehavior.Text;
                trackingSave.INSTINCTIVELY_BEHAVIOR = txtInstinctivelyBehavior.Text;
                trackingSave.MEMORY = txtMemory.Text;
                trackingSave.INTELLECTUAL = txtIntellectual.Text;
                trackingSave.CONCENTRATION = txtConcentration.Text;
                trackingSave.CARDIOVASCULAR = txtCardiovascular.Text;
                trackingSave.RESPIRATORY = txtRespiratory.Text;

                trackingSave.EYE_TENSION_LEFT = txtEyeTensionLeft.Text;
                trackingSave.EYE_TENSION_RIGHT = txtEyeTensionRight.Text;
                trackingSave.EYESIGHT_GLASS_LEFT = txtEyesightGlassLeft.Text;
                trackingSave.EYESIGHT_GLASS_RIGHT = txtEyesightGlassRight.Text;
                trackingSave.EYESIGHT_LEFT = txtEyesightLeft.Text;
                trackingSave.EYESIGHT_RIGHT = txtEyesightRight.Text;

                trackingSave.SHEET_ORDER = !string.IsNullOrEmpty(txtSheetOrder.Text) ? long.Parse(txtSheetOrder.Text) : (long?)null;

                trackingSDOs.Tracking = trackingSave;

                //List ServiceReq
                List<TreeSereServADO> dataCheckTree = GetListCheck();
                if (dataCheckTree != null && dataCheckTree.Count > 0)
                {
                    var dataCheckGroups = dataCheckTree.GroupBy(p => p.SERVICE_REQ_ID).Select(p => p.ToList()).ToList();
                    foreach (var itemGr in dataCheckGroups)
                    {
                        TrackingServiceReq sdo = new TrackingServiceReq();
                        var medi = itemGr.FirstOrDefault(p => p.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                        if (medi != null)
                            sdo.IsNotShowMedicine = false;
                        else
                            sdo.IsNotShowMedicine = true;

                        var mate = itemGr.FirstOrDefault(p => p.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                        if (mate != null)
                            sdo.IsNotShowMaterial = false;
                        else
                            sdo.IsNotShowMaterial = true;
                        var mediReq = itemGr.FirstOrDefault(p => p.TDL_SERVICE_TYPE_ID == 998);
                        if (mediReq != null)
                            sdo.IsNotShowOutMedi = false;
                        else
                            sdo.IsNotShowOutMedi = true;
                        var mateReq = itemGr.FirstOrDefault(p => p.TDL_SERVICE_TYPE_ID == 999);
                        if (mateReq != null)
                            sdo.IsNotShowOutMate = false;
                        else
                            sdo.IsNotShowOutMate = true;
                        sdo.ServiceReqId = itemGr.FirstOrDefault().SERVICE_REQ_ID ?? 0;
                        trackingSDOs.ServiceReqs.Add(sdo);
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackingSDOs.ServiceReqs), trackingSDOs.ServiceReqs));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => careIdsIncludeByTrackingCreated), careIdsIncludeByTrackingCreated));
                if (careIdsIncludeByTrackingCreated != null && careIdsIncludeByTrackingCreated.Count > 0)
                {
                    trackingSDOs.CareIds = careIdsIncludeByTrackingCreated;
                }
                //Dhst
                //không nhập thông tin dhst sẽ không truyền vào
                if (dhstProcessor != null)
                {
                    var ado = dhstProcessor.GetValue(ucControlDHST);
                    if (!CheckCtorDhst() || !CheckDhst((DHSTADO)ado))
                    {
                        var dhstAdo = ado as DHSTADO;
                        HIS_DHST dhst = new HIS_DHST();

                        if (this._Dhst != null)
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DHST>(dhst, this._Dhst);

                        dhst.TREATMENT_ID = this.treatmentId;//TreatmentId
                        dhst.EXECUTE_ROOM_ID = this.currentModule.RoomId;
                        dhst.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        dhst.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                        dhst.BELLY = dhstAdo.BELLY;
                        dhst.BLOOD_PRESSURE_MAX = dhstAdo.BLOOD_PRESSURE_MAX;
                        dhst.BLOOD_PRESSURE_MIN = dhstAdo.BLOOD_PRESSURE_MIN;
                        dhst.BREATH_RATE = dhstAdo.BREATH_RATE;
                        dhst.CAPILLARY_BLOOD_GLUCOSE = dhstAdo.CAPILLARY_BLOOD_GLUCOSE;
                        dhst.CHEST = dhstAdo.CHEST;
                        dhst.EXECUTE_TIME = dhstAdo.EXECUTE_TIME;
                        dhst.HEIGHT = dhstAdo.HEIGHT;
                        dhst.NOTE = dhstAdo.NOTE;
                        dhst.PULSE = dhstAdo.PULSE;
                        dhst.SPO2 = dhstAdo.SPO2;
                        dhst.TEMPERATURE = dhstAdo.TEMPERATURE;
                        dhst.WEIGHT = dhstAdo.WEIGHT;

                        trackingSDOs.Dhst = dhst;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateServiceReqInstructionDateByTrackingTime()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackingCreateOptionCFG), trackingCreateOptionCFG));
                if (currentTracking != null && currentTracking.ID > 0 && trackingCreateOptionCFG == "2")
                {
                    CommonParam param = new CommonParam();
                    //Load đơn phòng khám
                    HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.TREATMENT_ID = this.treatmentId;
                    serviceReqFilter.TRACKING_ID = currentTracking.ID;
                    var serviceReqByTrackings = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);
                    if (serviceReqByTrackings != null && serviceReqByTrackings.Count > 0)
                    {
                        bool isUpdateSuccess = false;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqByTrackings), serviceReqByTrackings));
                        foreach (var serviceReqUpdate in serviceReqByTrackings)
                        {
                            serviceReqUpdate.INTRUCTION_TIME = currentTracking.TRACKING_TIME;
                            serviceReqUpdate.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReqUpdate.INTRUCTION_TIME).Value.ToString("yyyyMMdd") + "000000");

                            param = new CommonParam();
                            var rs = new BackendAdapter(param)
                                .Post<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>("api/HisServiceReq/Update", ApiConsumers.MosConsumer, serviceReqUpdate, param);
                            if (rs == null)
                            {
                                param.Messages.Add("Cập nhật ngày y lệnh theo ngày của tờ điều trị thất bại");
                                MessageManager.Show(this.ParentForm, param, false);
                            }
                            else
                            {
                                isUpdateSuccess = true;
                            }
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTracking), currentTracking)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqUpdate), serviceReqUpdate)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                        }

                        if (isUpdateSuccess)
                            LoadDataSS(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetListServiceReqIdByDate(HIS_TRACKING data)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilter.TRACKING_ID = data.ID;
                serviceReqIds = new List<long>();
                var rs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param);
                if (rs != null && rs.Count > 0)
                {
                    serviceReqIds = rs.Select(x => x.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintType.IN_TO_DIEU_TRI);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdatetxtIcdExtraName(HIS_ICD dataIcd)
        {
            try
            {
                if (dataIcd != null)
                {
                    txtIcdExtraCode.Text = txtIcdExtraCode.Text + dataIcd.ICD_CODE + " - " + dataIcd.ICD_NAME + ", ";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateIcdNames))
                {
                    txtIcdExtraName.Text = delegateIcdNames;
                }
                if (!string.IsNullOrEmpty(delegateIcdCodes))
                {
                    txtIcdExtraCode.Text = delegateIcdCodes;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                //ValidSpinPulse(spinPulse);
                //ValidSpinPulse(spinBloodPressureMax);
                //ValidSpinPulse(spinBloodPressureMin);
                //ValidSpinPulse(spinTemperature);
                //ValidSpinPulse(spinBreathRate);
                //ValidSpinPulse(spinHeight);
                //ValidSpinPulse(spinWeight);
                //ValidSpinPulse(spinChest);
                //ValidSpinPulse(spinBelly);
                //ValidSpinPulse(spinDuongMau);
                //ValidSpinPulse(spinEditSpo2);
                ValidContent();
                SetMaxlength(txtResultCLS, 4000, false);
                SetMaxlength(txtMedicalInstruction, 4000, false);
                SetMaxlength(txtTheoDoiChamSoc, 4000, false);
                //SetMaxlength(txtNote, 4000, false);
                SetMaxlength(txtTheoDoiChamSoc, 4000, false);

                SetMaxlength(txtGeneralExpression, 4000, false);
                SetMaxlength(txtOrientationCapacity, 4000, false);
                SetMaxlength(txtEmotion, 4000, false);
                SetMaxlength(txtPerception, 4000, false);
                SetMaxlength(txtContentOfThinking, 4000, false);
                SetMaxlength(txtAwarenessBehavior, 4000, false);
                SetMaxlength(txtInstinctivelyBehavior, 4000, false);
                SetMaxlength(txtMemory, 4000, false);
                SetMaxlength(txtIntellectual, 4000, false);
                SetMaxlength(txtConcentration, 4000, false);
                SetMaxlength(txtCardiovascular, 4000, false);
                SetMaxlength(txtRespiratory, 4000, false);

                SetMaxlength(txtEyeTensionLeft, 500, false);
                SetMaxlength(txtEyeTensionRight, 500, false);
                SetMaxlength(txtEyesightGlassLeft, 500, false);
                SetMaxlength(txtEyesightGlassRight, 500, false);
                SetMaxlength(txtEyesightLeft, 500, false);
                SetMaxlength(txtEyesightRight, 500, false);

                ValidSheetOrder();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidSpinPulse(SpinEdit spinEdit)
        {
            SpinEditValidationRule spin = new SpinEditValidationRule();
            spin.spinEdit = spinEdit;
            spin.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spin.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(spinEdit, spin);
        }

        private void ValidContent()
        {
            MemoEditValidationRule spin = new MemoEditValidationRule();
            spin.txtTextEdit = txtContent;
            this.dxValidationProvider1.SetValidationRule(txtContent, spin);
        }

        private void ValidSheetOrder()
        {
            SheetOrderValidationRule SheetOrder = new SheetOrderValidationRule();
            SheetOrder.txtSheetOrder = txtSheetOrder;
            SheetOrder.ErrorText = ResourceMessage.TextEdit__KhongDuocNhapSoKhong;
            SheetOrder.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(txtSheetOrder, SheetOrder);
        }

        private void SetMaxlength(BaseEdit control, int maxlenght, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = IsRequired;
                validate.ErrorText = string.Format(ResourceMessage.NhapQuaMaxlength, maxlenght);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
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

        private void btnChamSoc_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentTracking != null || (this.trackingCreateOptionCFG == "2"))
                {
                    WaitingManager.Show();

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CareCreate").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.CareCreate'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.CareCreate' is not plugins");

                    List<object> listArgs = new List<object>();
                    if (currentTracking != null)
                        listArgs.Add(currentTracking);
                    else listArgs.Add(treatmentId);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)DelegateProcessDataResultReload);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).Show(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Care()
        {
            try
            {
                if (btnChamSoc.Enabled)
                {
                    btnChamSoc_Click(null, null);
                }
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
                if (btnChamSoc.Enabled)
                {
                    btnChamSoc_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdExtraCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string seperate = ";";
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                string[] periodSeparators = new string[1];
                periodSeparators[0] = seperate;
                string[] arrIcdExtraCodes = txtIcdExtraCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                {
                    var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                    foreach (var itemCode in arrIcdExtraCodes)
                    {
                        var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                        if (icdByCode != null && icdByCode.ID > 0)
                        {
                            strIcdNames += (seperate + icdByCode.ICD_NAME);
                        }
                        else
                        {
                            strWrongIcdCodes += (seperate + itemCode);
                        }
                    }
                    strIcdNames += seperate;
                    if (!String.IsNullOrEmpty(strWrongIcdCodes))
                    {
                        MessageManager.Show(String.Format(ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
                    }
                }
                txtIcdExtraName.Text = strIcdNames;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdExtraCode.SelectAll();
                    txtIcdExtraCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {

                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins");
                    HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtIcdExtraCode.Text, txtIcdExtraName.Text);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(secondaryIcdADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null"); WaitingManager.Hide();
                    ((Form)extenceInstance).Show(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraName_Leave(object sender, EventArgs e)
        {
            try
            {
                if (subIcdYhctProcessor != null && ucSecondaryIcdYhct != null)
                {
                    subIcdYhctProcessor.FocusControl(ucSecondaryIcdYhct);
                }
                //txtContent.Focus();
                //txtContent.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void barButtonItemSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListServiceReq_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                var data = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(e.Node);
                if (data != null
                    && data.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                    && data.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                    && data.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                {
                    if (!e.Node.HasChildren)
                    {
                        e.NodeImageIndex = 0;
                    }
                    else
                    {
                        e.NodeImageIndex = -1;
                    }
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListServiceReq_SelectImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            //DevExpress.XtraEditors.XtraMessageBox.Show("Click bổ sung ghi chú");
            try
            {
                var data = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(e.Node);
                if (data != null && data.ID > 0 && !e.Node.HasChildren)
                {
                    frmInstructionNote frm = new frmInstructionNote(data.ID);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl is DevExpress.XtraTreeList.TreeList)// trvService)
                {
                    DevExpress.Utils.ToolTipControlInfo info = null;
                    TreeListHitInfo hi = treeListServiceReq.CalcHitInfo(e.ControlMousePosition);
                    if (hi.HitInfoType == HitInfoType.SelectImage)
                    {
                        var o = hi.Node;
                        string text = "Ghi chú (Nhấn vào để bổ sung)";
                        //var data = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(o);
                        //if (data != null)
                        //{

                        //}
                        info = new DevExpress.Utils.ToolTipControlInfo(o, text);
                        e.Info = info;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTrackingTemp_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTrackingTemp.EditValue != null)
                    {
                        var trackingTemp = trackingTemps.First(o => o.ID == Int64.Parse(cboTrackingTemp.EditValue.ToString()));
                        txtTrackingTempCode.Text = trackingTemp.TRACKING_TEMP_CODE;
                        FillDataToControlByHisTrackingTemp(trackingTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlByHisTrackingTemp(HIS_TRACKING_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    txtContent.Text = data.CONTENT;
                    txtMedicalInstruction.Text = data.MEDICAL_INSTRUCTION;

                    if (dhstProcessor != null)//Dhst
                    {
                        var dhst = new HIS_DHST();
                        dhst.BELLY = data.BELLY;
                        dhst.BLOOD_PRESSURE_MAX = data.BLOOD_PRESSURE_MAX;
                        dhst.BLOOD_PRESSURE_MIN = data.BLOOD_PRESSURE_MIN;
                        dhst.BREATH_RATE = data.BREATH_RATE;
                        dhst.CHEST = data.CHEST;
                        dhst.HEIGHT = data.HEIGHT;
                        dhst.PULSE = data.PULSE;
                        dhst.TEMPERATURE = data.TEMPERATURE;
                        dhst.WEIGHT = data.WEIGHT;
                        dhstProcessor.SetValue(ucControlDHST, dhst);
                    }

                    btnPrint.Enabled = true;
                    //btnChamSoc.Enabled = true;
                    //btnAddExamInfo.Enabled = true;
                }
                else
                {
                    txtContent.Text = "";
                    txtMedicalInstruction.Text = "";
                    if (dhstProcessor != null)//Dhst
                    {
                        dhstProcessor.SetValue(ucControlDHST, new HIS_DHST());
                    }

                    btnPrint.Enabled = false;
                    //btnChamSoc.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_TRACKING_TEMP trackingTemp = new HIS_TRACKING_TEMP();
                GetDataToSaveTemp(ref trackingTemp);
                List<object> listArgs = new List<object>();
                listArgs.Add(trackingTemp);

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTrackingTemp", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                cboTrackingTemp.EditValue = null;
                trackingTemps = LoadTrackingTemp();
                InitComboTrackingTemp(trackingTemps);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataToSaveTemp(ref HIS_TRACKING_TEMP trackingSave)
        {
            try
            {
                if (trackingSave == null)
                {
                    trackingSave = new HIS_TRACKING_TEMP();
                }

                if (!String.IsNullOrWhiteSpace(txtContent.Text)
                    || !String.IsNullOrWhiteSpace(txtMedicalInstruction.Text)
                    )
                {
                    trackingSave.CONTENT = txtContent.Text;
                    trackingSave.MEDICAL_INSTRUCTION = txtMedicalInstruction.Text;
                }

                if (dhstProcessor != null)//Dhst
                {
                    var dhstAdo = (DHSTADO)dhstProcessor.GetValue(ucControlDHST);
                    if (dhstAdo != null)
                    {
                        trackingSave.BELLY = dhstAdo.BELLY;
                        trackingSave.BLOOD_PRESSURE_MAX = dhstAdo.BLOOD_PRESSURE_MAX;
                        trackingSave.BLOOD_PRESSURE_MIN = dhstAdo.BLOOD_PRESSURE_MIN;
                        trackingSave.BREATH_RATE = dhstAdo.BREATH_RATE;
                        trackingSave.CHEST = dhstAdo.CHEST;
                        trackingSave.HEIGHT = dhstAdo.HEIGHT;
                        trackingSave.PULSE = dhstAdo.PULSE;
                        trackingSave.TEMPERATURE = dhstAdo.TEMPERATURE;
                        trackingSave.WEIGHT = dhstAdo.WEIGHT;
                    }
                }

                if (!trackingSave.BELLY.HasValue
                    && !trackingSave.BLOOD_PRESSURE_MAX.HasValue
                    && !trackingSave.BLOOD_PRESSURE_MIN.HasValue
                    && !trackingSave.BREATH_RATE.HasValue
                    && !trackingSave.CHEST.HasValue
                    && String.IsNullOrWhiteSpace(trackingSave.CONTENT)
                    && String.IsNullOrWhiteSpace(trackingSave.MEDICAL_INSTRUCTION)
                    && !trackingSave.HEIGHT.HasValue
                    && !trackingSave.PULSE.HasValue
                    && !trackingSave.TEMPERATURE.HasValue
                    && !trackingSave.WEIGHT.HasValue)
                {
                    trackingSave = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnTrackingTemp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSaveTemp_Click(null, null);
        }

        private void txtTrackingTempCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadTrackingTempCombo(strValue);

                    if (cboTrackingTemp.EditValue != null)
                    {
                        var trackingTemp = trackingTemps.First(o => o.ID == Int64.Parse(cboTrackingTemp.EditValue.ToString()));
                        FillDataToControlByHisTrackingTemp(trackingTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTrackingTempCombo(string _Code)
        {
            try
            {
                List<HIS_TRACKING_TEMP> listResult = new List<HIS_TRACKING_TEMP>();
                listResult = this.trackingTemps.Where(o => (o.TRACKING_TEMP_CODE != null && o.TRACKING_TEMP_CODE.StartsWith(_Code))).ToList();

                if (listResult.Count == 1)
                {
                    cboTrackingTemp.EditValue = listResult[0].ID;
                    txtTrackingTempCode.Text = listResult[0].TRACKING_TEMP_CODE;
                    dtTrackingTime.Focus();
                }
                else if (listResult.Count > 1)
                {
                    cboTrackingTemp.EditValue = null;
                    cboTrackingTemp.Focus();
                    cboTrackingTemp.ShowPopup();
                }
                else
                {
                    cboTrackingTemp.EditValue = null;
                    cboTrackingTemp.Focus();
                    cboTrackingTemp.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTrackingTemp_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtTrackingTempCode.Text = "";
                    cboTrackingTemp.EditValue = null;
                    FillDataToControlByHisTrackingTemp(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssService_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAssService.Enabled)
                    return;
                //if (currentTracking != null)
                //{
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(treatmentId);
                    HIS.Desktop.ADO.AssignServiceADO assignServiceADO = new HIS.Desktop.ADO.AssignServiceADO(treatmentId, 0, 0);
                    assignServiceADO.PatientDob = this._Treatment.TDL_PATIENT_DOB;
                    assignServiceADO.PatientName = this._Treatment.TDL_PATIENT_NAME;
                    assignServiceADO.GenderName = this._Treatment.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.DgProcessDataResult = DelegateProcessDataResultReload;
                    if (this.trackingCreateOptionCFG == "2")
                    {
                        assignServiceADO.IntructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime) ?? 0;
                    }
                    else
                        assignServiceADO.IntructionTime = currentTracking != null ? currentTracking.TRACKING_TIME : Inventec.Common.DateTime.Get.Now() ?? 0;
                    listArgs.Add(assignServiceADO);
                    listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegateProcessDataResultReload(object data)
        {
            try
            {
                if (data != null)
                {
                    if (this.trackingCreateOptionCFG == "2")
                    {
                        if (data is OutPatientPresResultSDO)
                        {
                            var dataResult = data as OutPatientPresResultSDO;
                            if (serviceReqIdsIncludeByTrackingCreated == null)
                                serviceReqIdsIncludeByTrackingCreated = new List<long>();
                            if (dataResult.ServiceReqs != null && dataResult.ServiceReqs.Count > 0)
                                serviceReqIdsIncludeByTrackingCreated.AddRange(dataResult.ServiceReqs.Select(o => o.ID).ToList());
                        }
                        else if (data is InPatientPresResultSDO)
                        {
                            var dataResult = data as InPatientPresResultSDO;
                            if (serviceReqIdsIncludeByTrackingCreated == null)
                                serviceReqIdsIncludeByTrackingCreated = new List<long>();
                            if (dataResult.ServiceReqs != null && dataResult.ServiceReqs.Count > 0)
                                serviceReqIdsIncludeByTrackingCreated.AddRange(dataResult.ServiceReqs.Select(o => o.ID).ToList());
                        }
                        else if (data is HIS_CARE)
                        {
                            var dataResult = data as HIS_CARE;
                            if (careIdsIncludeByTrackingCreated == null)
                                careIdsIncludeByTrackingCreated = new List<long>();
                            if (dataResult != null && dataResult.ID > 0)
                                careIdsIncludeByTrackingCreated.Add(dataResult.ID);
                        }
                    }

                    LoadDataSS(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__AssService_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAssService_Click(null, null);
        }

        private void btnChoseResult_Click(object sender, EventArgs e)
        {
            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
            if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(this.treatmentId);
                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)SelectDataResult);
                listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                ((Form)extenceInstance).ShowDialog();
            }
        }

        private void SelectDataResult(object data)
        {
            //if (data != null && data is List<V_HIS_SERE_SERV_TEIN>)
            //{
            //    var dataNews = data as List<V_HIS_SERE_SERV_TEIN>;

            //    //TODO
            //    string dienBien = "";
            //    foreach (var item in dataNews)
            //    {
            //        dienBien += item.TEST_INDEX_NAME + ": " + item.VALUE + "; ";
            //    }

            //    txtResultCLS.Text = dienBien;
            //}
            if (data != null && data is string)
            {
                string dienBien = data as string;
                txtResultCLS.Text = dienBien;
            }
            else
            {
                txtResultCLS.Text = "";
            }
        }

        private void btnKeDonYHCT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnKeDonYHCT.Enabled)
                    return;
                //if (currentTracking != null)
                //{
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionYHCT").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionYHCT");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(treatmentId, 0, 0);
                    assignServiceADO.PatientDob = this._Treatment.TDL_PATIENT_DOB;
                    assignServiceADO.PatientName = this._Treatment.TDL_PATIENT_NAME;
                    assignServiceADO.GenderName = this._Treatment.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.TreatmentCode = this._Treatment.TREATMENT_CODE;
                    assignServiceADO.TreatmentId = this._Treatment.ID;
                    assignServiceADO.DgProcessDataResult = DelegateProcessDataResultReload;
                    if (this.trackingCreateOptionCFG == "2")
                    {
                        assignServiceADO.IntructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime) ?? 0;
                    }
                    else
                        assignServiceADO.IntructionTime = currentTracking != null ? currentTracking.TRACKING_TIME : Inventec.Common.DateTime.Get.Now() ?? 0;
                    listArgs.Add(assignServiceADO);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnKeDonThuoc_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnKeDonThuoc.Enabled)
                    return;
                //if (currentTracking != null)
                //{

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(treatmentId, 0, 0);
                    assignServiceADO.PatientDob = this._Treatment.TDL_PATIENT_DOB;
                    assignServiceADO.PatientName = this._Treatment.TDL_PATIENT_NAME;
                    assignServiceADO.GenderName = this._Treatment.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.TreatmentCode = this._Treatment.TREATMENT_CODE;
                    assignServiceADO.TreatmentId = this._Treatment.ID;
                    assignServiceADO.DgProcessDataResult = DelegateProcessDataResultReload;
                    if (this.trackingCreateOptionCFG == "2")
                    {
                        assignServiceADO.IntructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime) ?? 0;
                    }
                    else
                        assignServiceADO.IntructionTime = currentTracking != null ? currentTracking.TRACKING_TIME : Inventec.Common.DateTime.Get.Now() ?? 0;
                    listArgs.Add(assignServiceADO);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
                //}
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__KeDOnThuoc_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnKeDonThuoc_Click(null, null);
        }

        //private void btnAddExamInfo_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        HIS_TRACKING ado = new HIS_TRACKING();
        //        if (currentTracking != null && currentTracking.ID > 0)
        //        {
        //            ado = currentTracking;
        //        }
        //        else if (__AddExamInfo != null
        //            && (!string.IsNullOrEmpty(__AddExamInfo.GENERAL_EXPRESSION)
        //             || !string.IsNullOrEmpty(__AddExamInfo.ORIENTATION_CAPACITY)
        //             || !string.IsNullOrEmpty(__AddExamInfo.EMOTION)
        //             || !string.IsNullOrEmpty(__AddExamInfo.PERCEPTION)
        //             || !string.IsNullOrEmpty(__AddExamInfo.AWARENESS_BEHAVIOR)
        //             || !string.IsNullOrEmpty(__AddExamInfo.INSTINCTIVELY_BEHAVIOR)
        //             || !string.IsNullOrEmpty(__AddExamInfo.CONTENT_OF_THINKING)
        //             || !string.IsNullOrEmpty(__AddExamInfo.MEMORY)
        //             || !string.IsNullOrEmpty(__AddExamInfo.INTELLECTUAL)
        //             || !string.IsNullOrEmpty(__AddExamInfo.CONCENTRATION)
        //             || !string.IsNullOrEmpty(__AddExamInfo.CARDIOVASCULAR)
        //             || !string.IsNullOrEmpty(__AddExamInfo.RESPIRATORY))
        //            )
        //        {
        //            ado = __AddExamInfo;
        //        }
        //        frmAddExamInfor frmAdd = new frmAddExamInfor(ado);
        //        frmAdd.MyGetData = new frmAddExamInfor.GetString(UpdateAddExamInfo);
        //        frmAdd.Form = this;
        //        frmAdd.ShowDialog();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        internal void UpdateAddExamInfo(HIS_TRACKING ado)
        {
            try
            {
                if (ado != null)
                {
                    __AddExamInfo = ado;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNote_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTuTruc_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnTuTruc.Enabled)
                    return;

                //if (currentTracking != null)
                //{

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(treatmentId, 0, 0);
                    assignServiceADO.IsCabinet = true;
                    assignServiceADO.PatientDob = this._Treatment.TDL_PATIENT_DOB;
                    assignServiceADO.PatientName = this._Treatment.TDL_PATIENT_NAME;
                    assignServiceADO.GenderName = this._Treatment.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.TreatmentCode = this._Treatment.TREATMENT_CODE;
                    assignServiceADO.TreatmentId = this._Treatment.ID;
                    assignServiceADO.Tracking = this.currentTracking;
                    if (this.trackingCreateOptionCFG == "2")
                    {
                        assignServiceADO.IntructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime) ?? 0;
                    }
                    else
                        assignServiceADO.IntructionTime = currentTracking != null ? currentTracking.TRACKING_TIME : Inventec.Common.DateTime.Get.Now() ?? 0;
                    assignServiceADO.DgProcessDataResult = DelegateProcessDataResultReload;
                    listArgs.Add(assignServiceADO);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }

                //}
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkPrint && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkPrint;
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
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

        private void LoadConfigHisAcc()
        {
            try
            {
                CommonParam param = new CommonParam();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SDA.Filter.SdaConfigAppFilter configAppFilter = new SDA.Filter.SdaConfigAppFilter();
                configAppFilter.APP_CODE_ACCEPT = GlobalVariables.APPLICATION_CODE;
                configAppFilter.KEY = "CONFIG_KEY__HIS_PLUGINS_TRACKING_LIST__IS_SIGN_IS_PRINT_DOCUMENT_SIGNED";

                _currentConfigApp = new BackendAdapter(param).Get<List<SDA_CONFIG_APP>>("api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, configAppFilter, param).FirstOrDefault();

                string key = "";
                if (_currentConfigApp != null)
                {
                    key = _currentConfigApp.DEFAULT_VALUE;
                    SDA.Filter.SdaConfigAppUserFilter appUserFilter = new SDA.Filter.SdaConfigAppUserFilter();
                    appUserFilter.LOGINNAME = loginName;
                    appUserFilter.CONFIG_APP_ID = _currentConfigApp.ID;
                    currentConfigAppUser = new BackendAdapter(param).Get<List<SDA_CONFIG_APP_USER>>("api/SdaConfigAppUser/Get", ApiConsumers.SdaConsumer, appUserFilter, param).FirstOrDefault();
                    if (currentConfigAppUser != null)
                    {
                        key = currentConfigAppUser.VALUE;
                    }
                }

                if (!string.IsNullOrEmpty(key))
                {
                    _ConfigADO = (ConfigADO)Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigADO>(key);
                    if (_ConfigADO != null)
                    {
                        if (_ConfigADO.IsSign == "1")
                            chkSign.Checked = true;
                        else
                            chkSign.Checked = false;
                        if (_ConfigADO.IsPrintDocumentSigned == "1")
                            chkPrintDocumentSigned.Checked = true;
                        else
                            chkPrintDocumentSigned.Checked = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkPrintDocumentSigned.Enabled = chkSign.Checked;
                WaitingManager.Show();
                if (chkSign.Checked == false)
                {
                    chkPrintDocumentSigned.Checked = false;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkSign && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkSign;
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
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkPrintDocumentSigned && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkPrintDocumentSigned;
                    csAddOrUpdate.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
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

        private void dtTrackingTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtTrackingTime.EditValue != null)
                {
                    dtTimeFrom.EditValue = dtTrackingTime.EditValue;
                    dtTimeTo.EditValue = dtTrackingTime.EditValue;
                    LoadDataSS(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private bool CheckDhst()
        //{
        //    bool result = true;
        //    try
        //    {
        //        result = result && CheckCtorDhst();
        //        result = result && spinPulse.EditValue == null;
        //        result = result && spinBloodPressureMax.EditValue == null;
        //        result = result && spinBloodPressureMin.EditValue == null;
        //        result = result && spinTemperature.EditValue == null;
        //        result = result && spinBreathRate.EditValue == null;
        //        result = result && spinHeight.EditValue == null;
        //        result = result && spinWeight.EditValue == null;
        //        result = result && spinChest.EditValue == null;
        //        result = result && spinBelly.EditValue == null;
        //        result = result && spinDuongMau.EditValue == null;
        //        result = result && spinEditSpo2.EditValue == null;
        //        //result = result && (dtExecuteTimeDhst.EditValue == null || dtExecuteTimeDhst.DateTime == DateTime.MinValue);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //    return result;
        //}

        private bool CheckDhst(DHSTADO data)
        {
            bool result = true;
            try
            {
                result = result && !data.PULSE.HasValue;
                result = result && !data.BLOOD_PRESSURE_MAX.HasValue;
                result = result && !data.BLOOD_PRESSURE_MIN.HasValue;
                result = result && !data.TEMPERATURE.HasValue;
                result = result && !data.BREATH_RATE.HasValue;
                result = result && !data.HEIGHT.HasValue;
                result = result && !data.WEIGHT.HasValue;
                result = result && !data.CHEST.HasValue;
                result = result && !data.BELLY.HasValue;
                result = result && !data.CAPILLARY_BLOOD_GLUCOSE.HasValue;
                result = result && !data.SPO2.HasValue;
                //result = result && (dtExecuteTimeDhst.EditValue == null || dtExecuteTimeDhst.DateTime == DateTime.MinValue);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            Inventec.Common.Logging.LogSystem.Info("CheckDhst: " + result);
            return result;
        }

        private bool CheckCtorDhst()
        {
            bool result = true;
            try
            {
                if (this._Dhst != null && this._Dhst.ID > 0)
                {
                    result = result && !_Dhst.PULSE.HasValue;
                    result = result && !_Dhst.BLOOD_PRESSURE_MAX.HasValue;
                    result = result && !_Dhst.BLOOD_PRESSURE_MIN.HasValue;
                    result = result && !_Dhst.TEMPERATURE.HasValue;
                    result = result && !_Dhst.BREATH_RATE.HasValue;
                    result = result && !_Dhst.HEIGHT.HasValue;
                    result = result && !_Dhst.WEIGHT.HasValue;
                    result = result && !_Dhst.CHEST.HasValue;
                    result = result && !_Dhst.BELLY.HasValue;
                    result = result && !_Dhst.CAPILLARY_BLOOD_GLUCOSE.HasValue;
                    result = result && !_Dhst.SPO2.HasValue;
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _Dhst), _Dhst));
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            Inventec.Common.Logging.LogSystem.Info("CheckCtorDhst: " + result);
            return result;
        }

        private void InitUCDHST()
        {
            try
            {
                dhstProcessor = new DHSTProcessor();
                DHSTInitADO ado = new DHSTInitADO();
                ado.delegateOutFocus = NextFocusDhst;
                this.ucControlDHST = (UserControl)dhstProcessor.Run(ado);
                if (this.ucControlDHST != null)
                {
                    this.xtraTabPageDhst.Controls.Add(this.ucControlDHST);
                    this.ucControlDHST.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextFocusDhst()
        {
            try
            {
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadDhst()
        {
            try
            {
                if (currentTracking != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisDhstFilter dhstFilter = new MOS.Filter.HisDhstFilter();
                    //dhstFilter.TRACKING_ID = currentTracking.ID;

                    dhstFilter.TREATMENT_ID = currentTracking.TREATMENT_ID;
                    var rsDhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);
                    HIS_DHST rs = new HIS_DHST();
                    if (rsDhst != null && rsDhst.Count > 0)
                    {

                        rs = rsDhst.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).FirstOrDefault();
                        this._Dhst = rs;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._Dhst), this._Dhst));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSheetOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTrackingOld_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    Inventec.Common.Logging.LogSystem.Warn("cboTrackingOld.EditValue: " + cboTrackingOld.EditValue);
                    if (cboTrackingOld.EditValue != null)
                    {
                        var trackingOld = HisTrackingADO.FirstOrDefault(o => o.ID == Int64.Parse(cboTrackingOld.EditValue.ToString()));
                        txtContent.Text = trackingOld.CONTENT;
                        txtResultCLS.Text = trackingOld.SUBCLINICAL_PROCESSES;
                        txtTheoDoiChamSoc.Text = trackingOld.CARE_INSTRUCTION;
                        txtMedicalInstruction.Text = trackingOld.MEDICAL_INSTRUCTION;

                        if (!String.IsNullOrWhiteSpace(trackingOld.ICD_CODE) || !String.IsNullOrWhiteSpace(trackingOld.ICD_NAME))
                        {
                            HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                            icd.ICD_NAME = trackingOld.ICD_NAME;
                            icd.ICD_CODE = trackingOld.ICD_CODE;
                            if (ucIcd != null)
                            {
                                icdProcessor.Reload(ucIcd, icd);
                            }
                        }

                        txtIcdExtraName.Text = trackingOld.ICD_TEXT;
                        txtIcdExtraCode.Text = trackingOld.ICD_SUB_CODE;

                        if (!String.IsNullOrWhiteSpace(trackingOld.TRADITIONAL_ICD_CODE) || !String.IsNullOrWhiteSpace(trackingOld.TRADITIONAL_ICD_NAME))
                        {
                            HIS.UC.Icd.ADO.IcdInputADO icdYhct = new HIS.UC.Icd.ADO.IcdInputADO();
                            icdYhct.ICD_NAME = trackingOld.TRADITIONAL_ICD_NAME;
                            icdYhct.ICD_CODE = trackingOld.TRADITIONAL_ICD_CODE;
                            if (ucIcdYhct != null)
                            {
                                icdYhctProcessor.Reload(ucIcdYhct, icdYhct);
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(trackingOld.TRADITIONAL_ICD_SUB_CODE) || !String.IsNullOrWhiteSpace(trackingOld.TRADITIONAL_ICD_TEXT))
                        {
                            SecondaryIcdDataADO subYhctIcd = new SecondaryIcdDataADO();
                            subYhctIcd.ICD_SUB_CODE = trackingOld.TRADITIONAL_ICD_SUB_CODE;
                            subYhctIcd.ICD_TEXT = trackingOld.TRADITIONAL_ICD_TEXT;
                            if (ucSecondaryIcdYhct != null)
                            {
                                subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, subYhctIcd);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTrackingOld_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtTrackingTempCode.Text = "";
                    cboTrackingTemp.EditValue = null;
                    txtContent.Text = "";
                    txtResultCLS.Text = "";
                    txtTheoDoiChamSoc.Text = "";
                    txtMedicalInstruction.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnKeDonMau_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAssignBlood").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisAssignBlood");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    if (trackingCreateOptionCFG == "1")
                    {
                        listArgs.Add(currentTracking.TRACKING_TIME);
                    }
                    else if (trackingCreateOptionCFG == "2")
                    {
                        listArgs.Add(dtTrackingTime.EditValue.ToString());
                    }
                
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)SelectDataResult);
                    listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
