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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AddExamInfor;
using HIS.Desktop.Plugins.Library.CheckIcd;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Base;
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
using Inventec.Common.Logging;
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
    public partial class frmTrackingCreateNew : FormBase
    {           
        #region Declare
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        int action = -1;
        internal HIS_TRACKING currentTracking { get; set; }
        internal List<long> serviceReqIds;
        internal HisTrackingSDO trackingSDOs;
        internal HIS_TRACKING trackingOutSave;
        internal HIS_TRACKING trackingOutSaveSign;
        HIS_DHST _Dhst { get; set; }
        internal bool _IsMaterial { get; set; }

        internal List<HIS_SERVICE_REQ> _ServiceReqByTrackings = new List<HIS_SERVICE_REQ>();
        internal List<HIS_SERVICE_REQ> _ServiceReqByTrackingsTab2 = new List<HIS_SERVICE_REQ>();
        List<long> serviceReqIdsIncludeByTrackingCreated;
        List<long> careIdsIncludeByTrackingCreated;
        List<HIS_TRACKING_TEMP> trackingTemps { get; set; }

        List<HIS_TRACKING> trackingOlds;
        List<HisTrackingADO> HisTrackingADO;

        IcdProcessor icdProcessor;
        UserControl ucIcd;
        IcdProcessor icdYhctProcessor;
        UserControl ucIcdYhct;
        int positionHandleControl = -1;

        long treatmentId = 0;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.TrackingCreate";
        SDA_CONFIG_APP _currentConfigApp;
        SDA_CONFIG_APP_USER currentConfigAppUser;
        ConfigADO _ConfigADO;
        string trackingCreateOptionCFG = "";
        string StartTimeMustBeGreaterThanInstructionTime = "";
        string ASSIGN_SERVICE_SIMULTANEITY_OPTION = "";
        string IsCheckSubIcdExceedLimit = "";
        string IsReadOnlyCareInstruction = "";
        string ServiceReqIcdOption = "";
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;
        internal SecondaryIcdProcessor subIcdYhctProcessor;
        internal SecondaryIcdProcessor subIcdPbProcessor;
        internal UserControl ucSecondaryIcdYhct;
        internal UserControl ucSecondaryIcdPb;

        internal DHSTProcessor dhstProcessor;

        UserControl ucControlDHST;

        Action<HIS_TRACKING> actCallBack;
        HIS_TRACKING __AddExamInfo = new HIS_TRACKING();
        string updateTreatmentIcd = null;
        string loginName = null;
        V_HIS_ROOM currentRoom;
        bool IsLoadFromByTracking = false;
        HisTreatmentBedRoomLViewFilter DataTransferTreatmentBedRoomFilter { get; set; }
        public List<ACS_USER> ListUsser { get; private set; }
        bool IsFirstLoadForm = true;

        bool checksign;
        #endregion

        #region Constructor
        public frmTrackingCreateNew(HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
        {
            InitializeComponent();
            this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
            updateSizeControl();
        }
        public frmTrackingCreateNew(Inventec.Desktop.Common.Modules.Module currentModule, HIS_TRACKING hisTracking, Action<HIS_TRACKING> actCallBack = null, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
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
        public frmTrackingCreateNew(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, Action<HIS_TRACKING> actCallBack = null, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
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

        public frmTrackingCreateNew(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, HIS_DHST currentDhst, Action<HIS_TRACKING> actCallBack = null, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;

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

        #endregion

        #region Init UC
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
        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = FocusTxtIcdYhct;

                if (ServiceReqIcdOption == "1")
                {
                    ado.IsColor = true;
                }
                Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;
                //ado.delegateCheckICD = CheckICDCode;
                if (activeScreenDimensions != null) ado.Width = activeScreenDimensions.Width / 2 + 5;
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
        private void InitUcdSecondIcd()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == 1).ToList();
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), data);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcdToDo;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;
                ado.hisTreatment = this._Treatment;
                if (activeScreenDimensions != null) ado.Width = activeScreenDimensions.Width / 2 - 10;
                ado.Height = 24;
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.layoutControlSecondIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
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
                ado.hisTreatment = this._Treatment;
                Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;
                //ado.delegateCheckICD = CheckICDCodeYHCT;
                if (activeScreenDimensions != null) ado.Width = activeScreenDimensions.Width / 2 + 5;
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
                //txtIcdExtraCode.Focus();
                //txtIcdExtraCode.SelectAll();
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
                //ado.delegateCheckICD = CheckICDSecondYHCT;
                Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;

                if (activeScreenDimensions != null) ado.Width = activeScreenDimensions.Width / 2 - 10;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ YHCT phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán y học cổ truyền kèm theo";
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcdYhct = (UserControl)subIcdYhctProcessor.Run(ado);

                if (ucSecondaryIcdYhct != null)
                {
                    this.layoutControlISubIcdYhct.Controls.Add(ucSecondaryIcdYhct);
                    ucSecondaryIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitUcSecondaryIcdPb()
        {
            try
            {
                var icdYhct = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == 1).ToList();
                subIcdPbProcessor = new SecondaryIcdProcessor(new CommonParam(), icdYhct);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcdToDo;
                //ado.DelegateGetIcdMain = GetIcdMainCodeYhct;
                //ado.delegateCheckICD = CheckICDSecondYHCT;
                Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;

                if (activeScreenDimensions != null) ado.Width = activeScreenDimensions.Width / 2 - 10;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ phân biệt:";
                ado.TootiplciIcdSubCode = "Chẩn đoán phân biệt kèm theo";
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcdPb = (UserControl)subIcdPbProcessor.Run(ado);

                if (ucSecondaryIcdPb != null)
                {
                    this.layoutControlSubIcdPb.Controls.Add(ucSecondaryIcdPb);
                    ucSecondaryIcdPb.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool CheckICDPb()
        {
            bool result = true;
            try
            {
                result = (this.ucSecondaryIcdPb != null && (bool)this.subIcdPbProcessor.GetValidate(this.ucSecondaryIcdPb)) && result;
                if (this.icdProcessor != null && this.ucIcd != null || (this.subIcdProcessor != null && this.ucSecondaryIcd != null)
                    || (this.subIcdPbProcessor != null && this.ucSecondaryIcdPb != null))
                {
                    var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                    var icdValueSecond = subIcdProcessor.GetValue(this.ucSecondaryIcd);
                    var icdValuePB = subIcdPbProcessor.GetValue(this.ucSecondaryIcdPb);
                    string mainCode = "";
                    string mainCodeSecond = "";
                    if (icdValue != null)
                    {
                        mainCode = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;

                    }
                    if (icdValueSecond != null)
                    {
                        mainCodeSecond = ((SecondaryIcdDataADO)icdValueSecond).ICD_SUB_CODE;
                    }
                    if (icdValuePB != null)
                    {

                        string subcodePb = ((SecondaryIcdDataADO)icdValuePB).ICD_SUB_CODE;
                        List<string> listSubCode = new List<string>();

                        if (!string.IsNullOrEmpty(mainCodeSecond))
                        {
                            listSubCode = mainCodeSecond.Split(';').ToList();
                        }
                        listSubCode.Add(mainCode);
                        List<string> listSubCodePB = new List<string>();
                        if (!string.IsNullOrEmpty(subcodePb))
                        {
                            listSubCodePB = subcodePb.Split(';').ToList();
                        }
                        //check co hay k
                        string error = "";
                        var commonValues = listSubCodePB.Intersect(listSubCode).ToList();
                        if (commonValues != null && commonValues.Count() > 0)
                        {
                            error += string.Format("Mã bệnh phân biệt {0} đã sử dụng cho mã bệnh chính và phụ. Vui lòng kiểm tra lại", string.Join(",", commonValues.ToList()));

                            result = false;
                        }
                        LogSystem.Debug("SetError: " + error);
                        this.subIcdPbProcessor.SetError(ucSecondaryIcdPb, error);
                    }

                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private bool CheckICD(string icd_code, string icd_sub_code, ref string mess, ref string code_error, bool ref_code)
        {
            bool valid = false;

            try
            {
                HIS.Desktop.Plugins.Library.CheckIcd.CheckIcdManager mana = new CheckIcdManager(DlgIcdSubCode, this._Treatment);

                if (string.IsNullOrEmpty(mess) && !mana.ProcessCheckIcd(icd_code, icd_sub_code, ref mess, false, true))
                {
                    Inventec.Common.Logging.LogSystem.Debug("icd: " + icd_code + " va icd_phu: " + icd_sub_code + "co trung nhom ICD10");
                    valid = true;
                }

            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
        private void DlgIcdSubCode(string icdCodes, string icdNames)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("DlgIcdSubCode.1");

                Inventec.Common.Logging.LogSystem.Debug("DlgIcdSubCode.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
        private void InitUCDHST()
        {
            try
            {
                dhstProcessor = new DHSTProcessor();
                DHSTInitADO ado = new DHSTInitADO();
                ado.delegateOutFocus = NextFocusDhst;
                ado.IsVisibleUrineAndCapillaryBloodGlucose = true;
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
                //btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion            

        private void frmTrackingCreateNew_Load(object sender, EventArgs e)
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                SetIconFrm();          
                this.SetCaptionByLanguageKey();
                if (this.action == GlobalVariables.ActionEdit)
                    lciUpdateTimeDHST.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                this.AddBarManager(this.barManager1);

                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                this.trackingCreateOptionCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_CREATE_OPTION);
                this.StartTimeMustBeGreaterThanInstructionTime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_StartTimeMustBeGreaterThanInstructionTime);
                this.ASSIGN_SERVICE_SIMULTANEITY_OPTION = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_ASSIGN_SERVICE_SIMULTANEITY_OPTION);
                this.updateTreatmentIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_CRETATE_UPDATE_TREATMENT_ICD);
                this.IsReadOnlyCareInstruction = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_CRETATE_IsReadOnlyCareInstruction);
                this.IsCheckSubIcdExceedLimit = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_IsCheckSubIcdExceedLimit);
                BloodPresOption = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingCreate.BloodPresOption") == "1");
                this.ServiceReqIcdOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_TRACKING_SERVICE_REQ_ICD_OPTION);

                var IsMine = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_CRETATE_IS_MINE_CHECKED_BY_DEFAULT));

                if (IsMine == 1)
                {
                    chkIsMineNew.CheckState = CheckState.Checked;
                }

                LoadTreatment();
                InitUser();
                InitUcIcd();
                InitUcdSecondIcd();
                InitUcIcdYhct();
                InitUcSecondaryIcdYhct();
                InitUcSecondaryIcdPb();
                cboLogin.EditValue = this.loginName;
                txtLoginName.EditValue = this.loginName;
                //InitComboUser();
                //cboLogin.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //txtLoginName.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //SetIconFrm();
                //SetCaptionByLanguageKey();
                SetValueDefaultControl();
                InitUCDHST();
                ValidControl();
                CheckConfigIsMaterial();
                InitTrackingTemp();
                //trackingTemps = LoadTrackingTemp();
                //InitComboTrackingTemp(trackingTemps);
                InitTrackingOld();
                //HisTrackingADO = LoadTrackingOld();
                //InitComboTrackingOld(HisTrackingADO);
                icdProcessor.FocusControl(ucIcd);
                InitControlState();

                //dtExecuteTimeDhst.DateTime = DateTime.Now;
                //InitIcd();
                LoadAndFillIcdByTreatment();//tu dong fill icd ho so dieu tri

                // CheckRoomIsSurgery();
                if (currentTracking != null && currentTracking.ID > 0)
                {
                    FillDataToControlByHisTracking(currentTracking);
                }
                this.EnableButton((currentTracking != null && currentTracking.ID > 0));
                InitDhst();

                if (!IsLoadFromByTracking)
                {
                    isSearch = false;

                    LoadDataSS(false);
                    LoadDataSSTab2(false);
                    IsLoadFromByTracking = true;
                }
                //LoadTreatment();

                treeListServiceReq.ToolTipController = toolTipController1;
                treeListPreventive.ToolTipController = toolTipController1;
                //LoadConfigHisAcc();
                InitConfigAcs();
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadTreatment()
        {
            try
            {
                if (this.treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                    filter.ID = this.treatmentId;
                    var rs = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param);
                    if (rs != null)
                    {
                        this._Treatment = rs.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
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
        void SetValueDefaultControl()
        {
            try
            {
                cboDepartment.SelectedIndex = 0;
                dtTimeToNew.EditValue = DateTime.Now;
                dtTimeFromNew.EditValue = DateTime.Now;
                dtTrackingTime.EditValue = DateTime.Now;

                dteFromPreventive.EditValue = DateTime.Now;
                dteToPreventive.EditValue = DateTime.Now;
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
                btnKeDonMau.Enabled = false;

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
                        else if (item.KEY == ControlStateConstan.chkChiLayYLTuBB)
                        {
                            chkChiLayYLTuBBNew.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkUpdateTimeDHST)
                        {
                            chkUpdateTimeDHST.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkLockInfor.Name)
                        {
                            chkLockInfor.Checked = item.VALUE == "1";
                        }
                    }
                    chkPrintDocumentSigned.Enabled = chkSign.Checked;
                    if (chkSign.Checked == false)
                    {
                        chkPrintDocumentSigned.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadAndFillIcdByTreatment()
        {
            try
            {
                if (this._Treatment != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    //MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                    //filter.ID = this.treatmentId;
                    //var rs = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param);
                    if (_Treatment != null)
                    {

                        HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                        //icd.ICD_ID = rs.FirstOrDefault().ICD_ID;
                        icd.ICD_CODE = _Treatment.ICD_CODE;
                        icd.ICD_NAME = _Treatment.ICD_NAME;
                        if (ucIcd != null)
                        {
                            icdProcessor.Reload(ucIcd, icd);
                        }

                        SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                        subIcd.ICD_SUB_CODE = _Treatment.ICD_SUB_CODE;
                        subIcd.ICD_TEXT = _Treatment.ICD_TEXT;
                        if (ucSecondaryIcd != null)
                        {
                            subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                        }

                        //txtIcdExtraName.Text = _Treatment.ICD_TEXT;
                        //txtIcdExtraCode.Text = _Treatment.ICD_SUB_CODE;

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
                        lblAdress.Text = _Treatment.TDL_PATIENT_ADDRESS;
                        lblTreatmentCode.Text = _Treatment.TREATMENT_CODE;
                        lblPatientName.Text = _Treatment.TDL_PATIENT_NAME;
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_Treatment.TDL_PATIENT_DOB);  //_Treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? _Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(_Treatment.TDL_PATIENT_DOB);
                        lblGenderName.Text = _Treatment.TDL_PATIENT_GENDER_NAME;
                        lblHeinCardNumber.Text = _Treatment.TDL_HEIN_CARD_NUMBER;
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

                    result = result.Where(o => o.CREATOR == this.loginName || o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == DepartmentID || o.IS_PUBLIC == 1).ToList();
                }

            }
            catch (Exception ex)
            {
                result = new List<HIS_TRACKING_TEMP>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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
        private void FillDataToControlByHisTracking(HIS_TRACKING data)
        {
            try
            {
                //LogSystem.Debug("To dieu tri tu chuc nang danh sach truyen qua. " + LogUtil.TraceData("HIS_TRACKING", data));
                CommonParam param = new CommonParam();
                if (data != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
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
                    if (!String.IsNullOrWhiteSpace(data.ICD_DIFF_CODE) || !String.IsNullOrWhiteSpace(data.ICD_DIFF_TEXT))
                    {
                        SecondaryIcdDataADO subPbIcd = new SecondaryIcdDataADO();
                        subPbIcd.ICD_SUB_CODE = data.ICD_DIFF_CODE;
                        subPbIcd.ICD_TEXT = data.ICD_DIFF_TEXT;
                        if (ucSecondaryIcdPb != null)
                        {
                            this.subIcdPbProcessor.Reload(ucSecondaryIcdPb, subPbIcd);
                        }
                    }

                    SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = data.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = data.ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
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

                    //txtIcdExtraName.Text = data.ICD_TEXT;
                    //txtIcdExtraCode.Text = data.ICD_SUB_CODE;
                    txtContent.Text = data.CONTENT;
                    txtMedicalInstruction.Text = data.MEDICAL_INSTRUCTION;
                    txtResultCLS.Text = data.SUBCLINICAL_PROCESSES;
                    memReha.Text = data.REHABILITATION_CONTENT;
                    txtTheoDoiChamSoc.Text = data.CARE_INSTRUCTION;
                    txtDiseaseStage.Text = data.DISEASE_STAGE;

                    dtTrackingTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TRACKING_TIME);
                    dtTimeToNew.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TRACKING_TIME);
                    dtTimeFromNew.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TRACKING_TIME);


                    dteFromPreventive.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TRACKING_TIME);
                    dteToPreventive.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TRACKING_TIME);

                    txtSheetOrder.Text = data.SHEET_ORDER.ToString();

                    //Load DHST
                    //MOS.Filter.HisDhstFilter dhstFilter = new MOS.Filter.HisDhstFilter();
                    //dhstFilter.TRACKING_ID = data.ID;
                    ////dhstFilter.TREATMENT_ID = data.TREATMENT_ID;
                    //var rsDhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);
                    //HIS_DHST rs = new HIS_DHST();
                    //if (rsDhst != null && rsDhst.Count > 0)
                    //{
                    //    //rs = rsDhst.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).FirstOrDefault();
                    //    rs = rsDhst.FirstOrDefault();
                    //    this._Dhst = rs;
                    //}
                    //if (rs != null)
                    //{
                    //    FillDataDhstToControl(rs);
                    //}
                    btnSaveTemp.Enabled = true;

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
                if (dhstProcessor != null)
                {
                    dhstProcessor.SetValue(ucControlDHST, data);
                }
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

                if (this.trackingCreateOptionCFG == "1")
                {
                    this.btnKeDonMau.Enabled = enabled;
                    this.btnKeDonThuoc.Enabled = enabled;
                    this.btnKeDonYHCT.Enabled = enabled;
                    this.btnTuTruc.Enabled = enabled;
                    this.btnChamSoc.Enabled = enabled;
                    this.btnAssService.Enabled = enabled;
                    this.btnAssignPan.Enabled = enabled;
                }
                else if (this.trackingCreateOptionCFG == "2")
                {
                    this.btnKeDonMau.Enabled = true;
                    this.btnKeDonThuoc.Enabled = true;
                    this.btnKeDonYHCT.Enabled = true;
                    this.btnTuTruc.Enabled = true;
                    this.btnChamSoc.Enabled = true;
                    this.btnAssService.Enabled = true;
                    this.btnAssignPan.Enabled = true;
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
                CommonParam param = new CommonParam();
                SDA.Filter.SdaConfigAppFilter configAppFilter = new SDA.Filter.SdaConfigAppFilter();
                configAppFilter.APP_CODE_ACCEPT = GlobalVariables.APPLICATION_CODE;
                configAppFilter.KEY = "CONFIG_KEY__HIS_PLUGINS_TRACKING_LIST__IS_SIGN_IS_PRINT_DOCUMENT_SIGNED";

                _currentConfigApp = new BackendAdapter(param).Get<List<SDA_CONFIG_APP>>("api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, configAppFilter, param).FirstOrDefault();

                string key = "";
                if (_currentConfigApp != null)
                {
                    key = _currentConfigApp.DEFAULT_VALUE;
                    SDA.Filter.SdaConfigAppUserFilter appUserFilter = new SDA.Filter.SdaConfigAppUserFilter();
                    appUserFilter.LOGINNAME = this.loginName;
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
        #region InitCombo
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
        private void InitComboTrackingOld(List<HisTrackingADO> data)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Info("InitComboTrackingOld: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));

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
        #endregion
        #region Validate
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
                SetMaxlength(txtDiseaseStage, 300, false);
                SetMaxlength(memReha, 4000, false);

                ValidSheetOrder();
                ValidBenhPhu();
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
        private void ValidSheetOrder()
        {
            SheetOrderValidationRule SheetOrder = new SheetOrderValidationRule();
            SheetOrder.txtSheetOrder = txtSheetOrder;
            SheetOrder.ErrorText = ResourceMessage.TextEdit__KhongDuocNhapSoKhong;
            SheetOrder.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(txtSheetOrder, SheetOrder);
        }
        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        #endregion

        private void btnKeDonMau_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAssignBlood").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisAssignBlood");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    AssignBloodADO ado;
                    long treatID;
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    if (treatmentId != 0)
                    {
                        treatID = treatmentId;
                    }
                    else
                    {
                        treatID = currentTracking != null ? currentTracking.TREATMENT_ID != null ? currentTracking.TREATMENT_ID : 0 : 0;
                    }

                    Inventec.Common.Logging.LogSystem.Warn("____________TREATMENT ID: " + treatID);
                    ado = new AssignBloodADO(treatID, intructionTime, 0);

                    if (this.trackingCreateOptionCFG == "2")
                    {
                        ado.IntructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime) ?? 0;
                    }
                    else
                        ado.IntructionTime = currentTracking != null ? currentTracking.TRACKING_TIME : Inventec.Common.DateTime.Get.Now() ?? 0;

                    if (currentTracking != null)
                    {
                        ado.Tracking = currentTracking;
                    }


                    //Inventec.Common.Logging.LogSystem.Debug(
                    //    Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateTreatmentIcd), updateTreatmentIcd)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTracking), currentTracking));


                    listArgs.Add(ado);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)SelectDataResult);
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

        private void cboTrackingOld_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {

                    cboTrackingOld.EditValue = null;

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
                        memReha.Text = trackingOld.REHABILITATION_CONTENT;
                        txtResultCLS.Text = trackingOld.SUBCLINICAL_PROCESSES;
                        txtTheoDoiChamSoc.Text = trackingOld.CARE_INSTRUCTION;
                        txtMedicalInstruction.Text = trackingOld.MEDICAL_INSTRUCTION;

                        //if (!String.IsNullOrWhiteSpace(trackingOld.ICD_CODE) || !String.IsNullOrWhiteSpace(trackingOld.ICD_NAME))
                        //{
                        HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                        icd.ICD_NAME = trackingOld.ICD_NAME;
                        icd.ICD_CODE = trackingOld.ICD_CODE;
                        if (ucIcd != null)
                        {
                            icdProcessor.Reload(ucIcd, icd);
                        }
                        //}
                        SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                        subIcd.ICD_SUB_CODE = trackingOld.ICD_SUB_CODE;
                        subIcd.ICD_TEXT = trackingOld.ICD_TEXT;
                        if (ucSecondaryIcd != null)
                        {
                            subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                        }
                        //txtIcdExtraName.Text = trackingOld.ICD_TEXT;
                        //txtIcdExtraCode.Text = trackingOld.ICD_SUB_CODE;

                        //if (!String.IsNullOrWhiteSpace(trackingOld.TRADITIONAL_ICD_CODE) || !String.IsNullOrWhiteSpace(trackingOld.TRADITIONAL_ICD_NAME))
                        //{
                        HIS.UC.Icd.ADO.IcdInputADO icdYhct = new HIS.UC.Icd.ADO.IcdInputADO();
                        icdYhct.ICD_NAME = trackingOld.TRADITIONAL_ICD_NAME;
                        icdYhct.ICD_CODE = trackingOld.TRADITIONAL_ICD_CODE;
                        if (ucIcdYhct != null)
                        {
                            icdYhctProcessor.Reload(ucIcdYhct, icdYhct);
                        }
                        //}

                        //if (!String.IsNullOrWhiteSpace(trackingOld.TRADITIONAL_ICD_SUB_CODE) || !String.IsNullOrWhiteSpace(trackingOld.TRADITIONAL_ICD_TEXT))
                        //{
                        SecondaryIcdDataADO subYhctIcd = new SecondaryIcdDataADO();
                        subYhctIcd.ICD_SUB_CODE = trackingOld.TRADITIONAL_ICD_SUB_CODE;
                        subYhctIcd.ICD_TEXT = trackingOld.TRADITIONAL_ICD_TEXT;
                        if (ucSecondaryIcdYhct != null)
                        {
                            subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, subYhctIcd);
                        }
                        //}
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
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._Dhst), this._Dhst));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                    //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _Dhst), _Dhst));
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

            //Inventec.Common.Logging.LogSystem.Info("CheckCtorDhst: " + result);
            return result;
        }
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

            //Inventec.Common.Logging.LogSystem.Info("CheckDhst: " + result);
            return result;
        }

        private void dtTrackingTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtTrackingTime.EditValue != null)
                {
                    WaitingManager.Show();
                    dtTimeToNew.EditValue = dtTrackingTime.EditValue;
                    dtTimeFromNew.EditValue = dtTrackingTime.EditValue;
                    isSearch = true;
                    //backgroundWorker1.RunWorkerAsync();
                    dteFromPreventive.EditValue = dtTrackingTime.EditValue;
                    dteToPreventive.EditValue = dtTrackingTime.EditValue;
                    //backgroundWorker2.RunWorkerAsync();
                    chkUpdateTimeDHST_CheckedChanged(null, null);
                    if (IsLoadFromByTracking)
                    {
                        LoadDataSS(true);
                        LoadDataSSTab2(true);

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

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkPrintDocumentSigned.Enabled = chkSign.Checked;
                if (chkSign.Checked == false)
                {
                    chkPrintDocumentSigned.Checked = false;
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
                    if (this.trackingCreateOptionCFG == "2")
                    {
                        assignServiceADO.IntructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime) ?? 0;
                    }
                    else
                        assignServiceADO.IntructionTime = currentTracking != null ? currentTracking.TRACKING_TIME : Inventec.Common.DateTime.Get.Now() ?? 0;

                    if (currentTracking != null)
                    {
                        assignServiceADO.Tracking = currentTracking;
                    }

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

        private void barButtonItem__KeDOnThuoc_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnKeDonThuoc_Click(null, null);
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

                    if (currentTracking != null)
                    {
                        assignServiceADO.Tracking = currentTracking;
                    }
                    //Inventec.Common.Logging.LogSystem.Debug("btnKeDonThuoc_Click____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => assignServiceADO), assignServiceADO)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTracking), currentTracking));
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

                    if (currentTracking != null)
                    {
                        assignServiceADO.Tracking = currentTracking;
                    }

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

        private void barButtonItem__AssService_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAssService_Click(null, null);
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
                    isSearch = false;
                    //backgroundWorker1.RunWorkerAsync();
                    //backgroundWorker2.RunWorkerAsync();
                    LoadDataSS(false);
                    LoadDataSSTab2(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                    if (currentTracking != null)
                    {
                        assignServiceADO.Tracking = currentTracking;
                    }

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

        private bool IsCheckLockInfor(string input)
        {
            return !chkLockInfor.Checked || (chkLockInfor.Checked && string.IsNullOrEmpty(input.Trim()));
        }

        private void FillDataToControlByHisTrackingTemp(HIS_TRACKING_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    txtContent.Text = IsCheckLockInfor(txtContent.Text) ? data.CONTENT : txtContent.Text;
                    txtMedicalInstruction.Text = IsCheckLockInfor(txtMedicalInstruction.Text) ? data.MEDICAL_INSTRUCTION : txtMedicalInstruction.Text;

                    if (dhstProcessor != null)//Dhst
                    {
                        var dhstAdo = (DHSTADO)dhstProcessor.GetValue(ucControlDHST);

                        var dhst = new HIS_DHST();
                        dhst.BELLY = IsCheckLockInfor(dhstAdo.BELLY != null ? dhstAdo.BELLY.ToString() : "") ? data.BELLY : dhstAdo.BELLY;
                        dhst.BLOOD_PRESSURE_MAX = IsCheckLockInfor(dhstAdo.BLOOD_PRESSURE_MAX != null ? dhstAdo.BLOOD_PRESSURE_MAX.ToString() : "") ? data.BLOOD_PRESSURE_MAX : dhstAdo.BLOOD_PRESSURE_MAX;
                        dhst.BLOOD_PRESSURE_MIN = IsCheckLockInfor(dhstAdo.BLOOD_PRESSURE_MIN != null ? dhstAdo.BLOOD_PRESSURE_MIN.ToString() : "") ? data.BLOOD_PRESSURE_MIN : dhstAdo.BLOOD_PRESSURE_MIN;
                        dhst.BREATH_RATE = IsCheckLockInfor(dhstAdo.BREATH_RATE != null ? dhstAdo.BREATH_RATE.ToString() : "") ? data.BREATH_RATE : dhstAdo.BREATH_RATE;
                        dhst.CHEST = IsCheckLockInfor(dhstAdo.CHEST != null ? dhstAdo.CHEST.ToString() : "") ? data.CHEST : dhstAdo.CHEST;
                        dhst.HEIGHT = IsCheckLockInfor(dhstAdo.HEIGHT != null ? dhstAdo.HEIGHT.ToString() : "") ? data.HEIGHT : dhstAdo.HEIGHT;
                        dhst.PULSE = IsCheckLockInfor(dhstAdo.PULSE != null ? dhstAdo.PULSE.ToString() : "") ? data.PULSE : dhstAdo.PULSE;
                        dhst.TEMPERATURE = IsCheckLockInfor(dhstAdo.TEMPERATURE != null ? dhstAdo.TEMPERATURE.ToString() : "") ? data.BELLY : dhstAdo.TEMPERATURE;
                        dhst.WEIGHT = IsCheckLockInfor(dhstAdo.WEIGHT != null ? dhstAdo.WEIGHT.ToString() : "") ? data.BELLY : dhstAdo.WEIGHT; ;
                        dhstProcessor.SetValue(ucControlDHST, dhst);
                    }

                    btnSaveTemp.Enabled = true;
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

                    btnSaveTemp.Enabled = false;
                    //btnChamSoc.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private void bbtnTrackingTemp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSaveTemp_Click(null, null);
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

                    if (hi.Column != null && hi.Column.FieldName == "IMG")
                    {
                        var o = hi.Node;
                        string text = "";
                        var data = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(o);
                        if (data != null)
                        {
                            if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            {
                                text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__TRACKING_CREATE__CHUA_XU_LY", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            {
                                text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__TRACKING_CREATE__DANG_XU_LY", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            {
                                text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__TRACKING_CREATE__HOAN_THANH", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                            }
                        }
                        info = new DevExpress.Utils.ToolTipControlInfo(o, text);
                        e.Info = info;
                    }

                }
            }
            catch (Exception ex)
            {
                //Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListServiceReq_SelectImageClick(object sender, NodeClickEventArgs e)
        {
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

        private void treeListServiceReq_GetSelectImage(object sender, GetSelectImageEventArgs e)
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

        private void barButtonItemSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (xtraTabControl2.SelectedTabPageIndex == 0)
                    btnSearch_Click(null, null);
                else
                    btnSearchNewTab2_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void txtIcdExtraName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.F1)
                //{

                //    WaitingManager.Show();
                //    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                //    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                //    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins");
                //    HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtIcdExtraCode.Text, txtIcdExtraName.Text);
                //    List<object> listArgs = new List<object>();
                //    listArgs.Add(secondaryIcdADO);
                //    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                //    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null"); WaitingManager.Hide();
                //    ((Form)extenceInstance).Show(this);
                //}
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
                //if (e.KeyCode == Keys.Enter)
                //{
                //    txtIcdExtraCode.SelectAll();
                //    txtIcdExtraCode.Focus();
                //}
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
                ///// thay thế code cũ bằng UC
                //string seperate = ";";
                //string strIcdNames = "";
                //string strWrongIcdCodes = "";
                //string[] periodSeparators = new string[1];
                //periodSeparators[0] = seperate;
                //string[] arrIcdExtraCodes = txtIcdExtraCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                //if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                //{
                //    var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                //    foreach (var itemCode in arrIcdExtraCodes)
                //    {
                //        var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                //        if (icdByCode != null && icdByCode.ID > 0)
                //        {
                //            strIcdNames += (seperate + icdByCode.ICD_NAME);
                //        }
                //        else
                //        {
                //            strWrongIcdCodes += (seperate + itemCode);
                //        }
                //    }
                //    strIcdNames += seperate;
                //    if (!String.IsNullOrEmpty(strWrongIcdCodes))
                //    {
                //        MessageManager.Show(String.Format(ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
                //    }
                //}
                //txtIcdExtraName.Text = strIcdNames;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
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

                    if (DataTransferTreatmentBedRoomFilter != null)
                        listArgs.Add(DataTransferTreatmentBedRoomFilter);

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

        /// <summary>
        ///  thay thế code cũ bằng uc
        /// </summary>
        /// 
        //private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(delegateIcdNames))
        //        {
        //            txtIcdExtraName.Text = delegateIcdNames;
        //        }
        //        if (!string.IsNullOrEmpty(delegateIcdCodes))
        //        {
        //            txtIcdExtraCode.Text = delegateIcdCodes;
        //        }
        //        CheckICDSecond();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        //private void UpdatetxtIcdExtraName(HIS_ICD dataIcd)
        //{
        //    try
        //    {
        //        if (dataIcd != null)
        //        {
        //            txtIcdExtraCode.Text = txtIcdExtraCode.Text + dataIcd.ICD_CODE + " - " + dataIcd.ICD_NAME + ", ";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

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
        private void UpdateServiceReqInstructionDateByTrackingTime()
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackingCreateOptionCFG), trackingCreateOptionCFG));
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
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqByTrackings), serviceReqByTrackings));
                        foreach (var serviceReqUpdate in serviceReqByTrackings)
                        {
                            if (this.action == GlobalVariables.ActionEdit)
                            {
                                serviceReqUpdate.INTRUCTION_TIME = currentTracking.TRACKING_TIME;
                                serviceReqUpdate.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReqUpdate.INTRUCTION_TIME).Value.ToString("yyyyMMdd") + "000000");
                            }
                            param = new CommonParam();
                            var rs = new BackendAdapter(param)
                                .Post<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>("api/HisServiceReq/Update", ApiConsumers.MosConsumer, serviceReqUpdate, param);
                            if (rs == null)
                            {
                                if (this.action == GlobalVariables.ActionEdit)
                                {
                                    param.Messages.Add("Cập nhật ngày y lệnh theo ngày của tờ điều trị thất bại");
                                }
                                MessageManager.Show(this.ParentForm, param, false);
                            }
                            else
                            {
                                isUpdateSuccess = true;
                            }
                            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTracking), currentTracking)
                            //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqUpdate), serviceReqUpdate)
                            //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                        }

                        if (isUpdateSuccess && this.action == GlobalVariables.ActionEdit)
                        {
                            isSearch = false;
                            //backgroundWorker1.RunWorkerAsync();
                            //backgroundWorker2.RunWorkerAsync();
                            LoadDataSS(false);
                            LoadDataSSTab2(false);
                        }
                    }
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
                    trackingSave.CREATOR = trackingOutSave.CREATOR;
                }
                else if (currentTracking != null)
                {
                    trackingSave.ID = currentTracking.ID;
                    trackingSave.CREATOR = currentTracking.CREATOR;
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

                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        trackingSave.ICD_SUB_CODE = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        trackingSave.ICD_TEXT = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }
                if (this.ucSecondaryIcdPb != null)
                {
                    var subICDPB = this.subIcdPbProcessor.GetValue(ucSecondaryIcdPb);
                    if (subICDPB != null && subICDPB is SecondaryIcdDataADO)
                    {
                        trackingSave.ICD_DIFF_CODE = ((SecondaryIcdDataADO)subICDPB).ICD_SUB_CODE;
                        trackingSave.ICD_DIFF_TEXT = ((SecondaryIcdDataADO)subICDPB).ICD_TEXT;
                    }
                }
                //thay bang uc
                //trackingSave.ICD_TEXT = txtIcdExtraName.Text;
                //trackingSave.ICD_SUB_CODE = txtIcdExtraCode.Text;
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
                trackingSave.REHABILITATION_CONTENT = memReha.Text.Trim();
                trackingSave.SHEET_ORDER = !string.IsNullOrEmpty(txtSheetOrder.Text) ? long.Parse(txtSheetOrder.Text) : (long?)null;
                trackingSave.DISEASE_STAGE = txtDiseaseStage.Text.Trim();

                trackingSDOs.Tracking = trackingSave;

                //List ServiceReq
                List<TreeSereServADO> dataCheckTree = GetListCheck();
                List<TreeSereServADO> dataCheckTreeTab2 = GetListCheckTab2();
                if (dataCheckTreeTab2 != null && dataCheckTreeTab2.Count > 0)
                {

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataCheckTreeTab2), dataCheckTreeTab2));
                    var dataCheckGroups = dataCheckTreeTab2.GroupBy(p => p.SERVICE_REQ_ID).Select(p => p.ToList()).ToList();
                    trackingSDOs.UsedForServiceReqIds = new List<long>();
                    foreach (var item in dataCheckGroups)
                    {
                        trackingSDOs.UsedForServiceReqIds.Add(item.First().SERVICE_REQ_ID ?? 0);
                    }
                }
                if (dataCheckTree != null && dataCheckTree.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataCheckTree), dataCheckTree));
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

                        //                  if (currentTracking == null)
                        //                  {
                        //                      sdo.IsTracking = itemGr.FirstOrDefault(o => o.tabType == TreeSereServADO.TAB_TYPE.TAB_1) != null;
                        //                      sdo.IsUsedForTracking = itemGr.FirstOrDefault(o => o.tabType == TreeSereServADO.TAB_TYPE.TAB_2) != null;
                        //}

                        trackingSDOs.ServiceReqs.Add(sdo);
                    }
                }
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackingSDOs.ServiceReqs), trackingSDOs.ServiceReqs));
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackingSDOs.UsedForServiceReqIds), trackingSDOs.UsedForServiceReqIds));
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => careIdsIncludeByTrackingCreated), careIdsIncludeByTrackingCreated));
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
                        dhst.EXECUTE_LOGINNAME = this.loginName;
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
                        dhst.URINE = dhstAdo.URINE;

                        trackingSDOs.Dhst = dhst;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
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
                    configAppUserUpdate.LOGINNAME = this.loginName;
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
        private bool CheckICDPreSave()
        {
            bool valid = true;
            try
            {
                if (this.icdProcessor != null && this.ucIcd != null || (this.icdYhctProcessor != null && this.ucIcdYhct != null) || (this.subIcdProcessor != null && this.ucSecondaryIcd != null)
                    || (this.subIcdYhctProcessor != null && this.ucSecondaryIcdYhct != null))
                {
                    var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                    var icdValueSecond = subIcdProcessor.GetValue(this.ucSecondaryIcd);
                    var icdYHCT = this.icdYhctProcessor.GetValue(this.ucIcdYhct);
                    var icdValueSecondYHCT = subIcdYhctProcessor.GetValue(this.ucSecondaryIcdYhct);

                    var config = HisConfigs.Get<string>("HIS.Desktop.Plugins.CheckIcdWhenSave");
                    string mess = "";
                    var mainCode = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    var mainCodeSecond = ((SecondaryIcdDataADO)icdValueSecond).ICD_SUB_CODE;
                    List<string> listicd = new List<string>();
                    string icd_code_error = "";
                    if (!string.IsNullOrEmpty(mainCode) || !string.IsNullOrEmpty(mainCodeSecond))
                    {
                        List<string> listSubCode = new List<string>();

                        if (!string.IsNullOrEmpty(mainCodeSecond))
                        {
                            listSubCode = mainCodeSecond.Split(';').ToList();
                        }
                        if (CheckICD(mainCode, mainCodeSecond, ref mess, ref icd_code_error, false))
                        {

                            if (config == "1")
                            {
                                if (MessageBox.Show(this, mess, "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    valid = true;
                                }
                                else valid = false;
                            }
                            else if (config == "2")
                            {
                                MessageBox.Show(this, mess, "Thông báo", MessageBoxButtons.OK);

                                valid = false;
                            }


                        }
                    }




                    //yhct
                    var subCode = ((HIS.UC.Icd.ADO.IcdInputADO)icdYHCT).ICD_CODE;
                    var subcodeSecond = ((SecondaryIcdDataADO)icdValueSecondYHCT).ICD_SUB_CODE;
                    string messYhct = "";
                    string error_code_yhct = "";
                    if (!string.IsNullOrEmpty(subCode) || !string.IsNullOrEmpty(subcodeSecond))
                    {
                        List<string> listSubCodeYhct = new List<string>();

                        if (!string.IsNullOrEmpty(subcodeSecond))
                        {
                            listSubCodeYhct = subcodeSecond.Split(';').ToList();
                        }
                        //var listSubCodeYhct = subcodeSecond.Split(';').ToList();
                        if (CheckICD(subCode, subcodeSecond, ref messYhct, ref error_code_yhct, false))
                        {
                            var param_error = error_code_yhct.Split(';').ToList();
                            if (!string.IsNullOrEmpty(messYhct))
                            {
                                if (config == "1")
                                {
                                    if (MessageBox.Show(this, messYhct, "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        valid = true;
                                    }
                                    else valid = false;
                                }
                                else if (config == "2")
                                {
                                    MessageBox.Show(this, messYhct, "Thông báo", MessageBoxButtons.OK);

                                    valid = false;
                                }
                            }
                        }
                    }


                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //HIS.Desktop.Plugins.TrackingCreate.Demo.Form1 frm = new Demo.Form1();
                //frm.ShowDialog();
                this.positionHandleControl = -1;
                bool IsValid = true;
                IsValid = (bool)icdProcessor.ValidationIcd(this.ucIcd);
                IsValid = (bool)(this.ucSecondaryIcd != null && this.subIcdProcessor.GetValidate(this.ucSecondaryIcd)) && IsValid;

                if (this.layoutControlItem12.Visible && this.layoutControlISubIcdYhct.Visible)
                    IsValid = (bool)(this.ucSecondaryIcdYhct != null && this.subIcdYhctProcessor.GetValidate(this.ucSecondaryIcdYhct)) && IsValid;
                if (this.layoutControlItem9.Visible && this.layoutControlIcdYhct.Visible)
                    IsValid = (bool)(this.ucIcdYhct != null && (bool)this.icdYhctProcessor.ValidationIcd(this.ucIcdYhct)) && IsValid;

                IsValid = IsValid && dxValidationProvider1.Validate();
                if (!IsValid)
                    return;

                IsValid = IsValid && CheckICDPreSave();
                IsValid = IsValid && CheckICDPb();

                if (!IsValid)
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

                    if (!CheckBeforeCallApiUpdate())
                        return;
                    trackingOutSave = new HIS_TRACKING();
                    trackingOutSave = new BackendAdapter(param).Post<HIS_TRACKING>(HisRequestUriStore.HIS_TRACKING_CREATE, ApiConsumers.MosConsumer, this.trackingSDOs, param);
                    WaitingManager.Show();
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

                        UpdateServiceReqInstructionDateByTrackingTime();

                        if (chkPrint.Checked || chkSign.Checked)
                        {
                            PrintProcess(PrintType.IN_TO_DIEU_TRI);
                        }
                    }
                }
                else if (this.action == GlobalVariables.ActionEdit)
                {
                    GetDataToSave();
                    if (!CheckBeforeCallApiUpdate())
                        return;
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("api/HisTracking/Update this.trackingSDOs: ", this.trackingSDOs));
                    trackingOutSave = new HIS_TRACKING();
                    trackingOutSave = new BackendAdapter(param).Post<HIS_TRACKING>(HisRequestUriStore.HIS_TRACKING_UPDATE, ApiConsumers.MosConsumer, this.trackingSDOs, param);
                    WaitingManager.Show();
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
                WaitingManager.Hide();
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

        private void txtMedicalInstruction_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtContent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                isSearch = true;
                //backgroundWorker1.RunWorkerAsync();
                LoadDataSS(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLuuMau_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TextLibrary").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TextLibrary");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    TextLibraryInfoADO ado = new TextLibraryInfoADO();
                    ado.Content = txtTheoDoiChamSoc.Text;
                    ado.Hashtag = "ThuVienChamSocLuuMau";
                    listArgs.Add(ado);

                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDanhSachMau_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TextLibrary").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TextLibrary");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    TextLibraryInfoADO ado = new TextLibraryInfoADO();
                    ado.Content = null;
                    ado.Hashtag = "ThuVienChamSocDanhSachMau";
                    listArgs.Add(ado);
                    listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLib);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessDataTextLib(MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib)
        {
            try
            {
                this.txtTheoDoiChamSoc.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        /// thay thế bằng UC
        /// </summary>
        //private void txtIcdExtraCode_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            string seperate = ";";
        //            string strIcdNames = "";
        //            string strWrongIcdCodes = "";
        //            string[] periodSeparators = new string[1];
        //            periodSeparators[0] = seperate;
        //            string[] arrIcdExtraCodes = txtIcdExtraCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
        //            if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
        //            {
        //                var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
        //                foreach (var itemCode in arrIcdExtraCodes)
        //                {
        //                    var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
        //                    if (icdByCode != null && icdByCode.ID > 0)
        //                    {
        //                        strIcdNames += (seperate + icdByCode.ICD_NAME);
        //                    }
        //                    else
        //                    {
        //                        strWrongIcdCodes += (seperate + itemCode);
        //                    }
        //                }
        //                strIcdNames += seperate;
        //                if (!String.IsNullOrEmpty(strWrongIcdCodes))
        //                {
        //                    MessageManager.Show(String.Format(ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
        //                }
        //            }
        //            txtIcdExtraName.Text = strIcdNames;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void treeListServiceReq_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeListServiceReq.GetDataRecordByNode(e.Node) as TreeSereServADO;
                if (data != null)
                {
                    if ((data.LEVER == 3 && !data.IsMedicinePreventive) || (data.LEVER == 4 && data.IsServiceUseForTracking))
                    {
                        var workingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                        string creator = data.CREATOR;
                        string reqLoginName = data.REQUEST_LOGINNAME;
                        long reqSttId = data.SERVICE_REQ_STT_ID;
                        long serReqTypeId = data.TDL_SERVICE_REQ_TYPE_ID;
                        short? isNoExecute = data.IS_NO_EXECUTE;
                        long requestDepartmentId = data.TDL_REQUEST_DEPARTMENT_ID;

                        if (e.Column.FieldName == "ServiceReqDelete")
                        {
                            if (data.LEVER == 4 && data.IsServiceUseForTracking)
                            {
                                if (data.USED_FOR_TRACKING_ID != null)
                                {
                                    e.RepositoryItem = repositoryItemServiceReqDeleteDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemServiceReqDelete;
                                }
                            }
                            else if (data.LEVER == 3 && !data.IsMedicinePreventive)
                            {
                                if ((creator == this.loginName || reqLoginName == this.loginName || CheckLoginAdmin.IsAdmin(this.loginName) || (this.currentRoom != null && requestDepartmentId == this.currentRoom.DEPARTMENT_ID && serReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                    && (reqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL))
                                {
                                    e.RepositoryItem = repositoryItemServiceReqDelete;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemServiceReqDeleteDisable;
                                }
                            }
                        }
                        else if (e.Column.FieldName == "ServiceReqEdit")
                        {
                            if (data.LEVER == 4 && data.IsServiceUseForTracking)
                            {
                                if (data.USED_FOR_TRACKING_ID != null)
                                {
                                    e.RepositoryItem = repositoryItemServiceReqUseTimeDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemServiceReqUseTime;
                                }
                            }
                            else if (data.LEVER == 3 && !data.IsMedicinePreventive)
                            {
                                if (serReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN)
                                {
                                    e.RepositoryItem = repositoryItemServiceReqEditDisable;
                                }
                                else if ((creator == this.loginName || reqLoginName == this.loginName || CheckLoginAdmin.IsAdmin(this.loginName))
                                     && (((reqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                     && isNoExecute != 1)
                                     || ((reqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                     && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1"))
                                     && data.TDL_TRACKING_ID == null)
                                {
                                    e.RepositoryItem = repositoryItemServiceReqEdit;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemServiceReqEditDisable;
                                }
                            }
                        }
                        else if (e.Column.FieldName == "ServiceReqUse")
                        {
                            if (data.LEVER == 4 && data.IsServiceUseForTracking)
                            {
                                if (data.USED_FOR_TRACKING_ID != null || data.IS_TEMPORARY_PRES != 1)
                                {
                                    e.RepositoryItem = repositoryItemServiceReqUseDisable;
                                }
                                else if (data.IS_TEMPORARY_PRES == 1)
                                {
                                    e.RepositoryItem = repositoryItemServiceReqUseEnable;
                                }
                            }
                            else if (data.LEVER == 3 && !data.IsMedicinePreventive)
                            {
                                if (data.IS_TEMPORARY_PRES == 1)
                                {
                                    e.RepositoryItem = repositoryItemServiceReqUseEnable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemServiceReqUseDisable;
                                }
                            }
                        }
                        else if (e.Column.FieldName == "IMG")
                        {
                            if ((data.LEVER == 4 && data.IsServiceUseForTracking) || (data.LEVER == 3 && !data.IsMedicinePreventive))
                                e.RepositoryItem = repositoryItempicServiceReqStatus;
                            else
                                e.RepositoryItem = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBenhPhu()
        {
            try
            {

                //BenhPhuValidationRule mainRule = new BenhPhuValidationRule();
                //mainRule.maBenhPhuTxt = txtIcdExtraCode;
                //mainRule.tenBenhPhuTxt = txtIcdExtraName;
                //mainRule.getIcdMain = this.GetIcdMainCode;
                //mainRule.listIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == 1).ToList();
                //mainRule.ErrorType = ErrorType.Warning;
                //this.dxValidationProvider1.SetValidationRule(txtIcdExtraCode, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                var icdValue = this.UcIcdGetValue();
                if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                {
                    mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }
        public object UcIcdGetValue()
        {
            object result = null;
            try
            {
                HIS.UC.Icd.ADO.IcdInputADO outPut = new HIS.UC.Icd.ADO.IcdInputADO();
                HIS.UC.Icd.ADO.IcdInputADO OjecIcd = (HIS.UC.Icd.ADO.IcdInputADO)icdProcessor.GetValue(ucIcd);
                outPut.ICD_NAME = OjecIcd != null ? OjecIcd.ICD_NAME : "";
                outPut.ICD_CODE = OjecIcd != null ? OjecIcd.ICD_CODE : "";
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        public object SubUcIcdGetValue()
        {
            object result = null;
            try
            {
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO outPut = new SecondaryIcdDataADO();
                SecondaryIcdDataADO OjecIcd = (SecondaryIcdDataADO)subIcdProcessor.GetValue(ucSecondaryIcd);
                outPut.ICD_SUB_CODE = OjecIcd != null ? OjecIcd.ICD_SUB_CODE : "";
                outPut.ICD_TEXT = OjecIcd != null ? OjecIcd.ICD_TEXT : "";
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void txtTheoDoiChamSoc_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    LoaiChamSoc lcs = new LoaiChamSoc((HIS.Desktop.Common.DelegateSelectData)ReloadSoVaoVien);
                    lcs.ShowDialog();


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ReloadSoVaoVien(object obj)
        {
            try
            {
                if (obj != null && obj is string)
                {
                    string dienBien = obj as string;
                    txtTheoDoiChamSoc.Text = dienBien;

                }
                else
                {
                    txtTheoDoiChamSoc.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTheoDoiChamSoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (this.IsReadOnlyCareInstruction != "1")
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkChiLayYLTuBB_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (chkChiLayYLTuBBNew.Checked)
                {
                    treeSereServ_CheckYLFromBB(treeListServiceReq.Nodes);
                }
                else
                {
                    treeSereServ_CheckAllNode(treeListServiceReq.Nodes);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barLuuKy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                btnSaveSign_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnSaveSign_Click(object sender, EventArgs e)
        {
            try
            {
                checksign = true;
                this.positionHandleControl = -1;
                bool IsValid = true;
                IsValid = (bool)icdProcessor.ValidationIcd(this.ucIcd);
                IsValid = (bool)this.subIcdProcessor.GetValidate(this.ucSecondaryIcd) && IsValid;
                IsValid = (bool)this.subIcdYhctProcessor.GetValidate(this.ucSecondaryIcdYhct) && IsValid;

                IsValid = (bool)this.icdYhctProcessor.ValidationIcd(this.ucIcdYhct) && IsValid;
                IsValid = IsValid && CheckICDPb();

                if (!IsValid)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;

                {

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

                    if (!CheckBeforeCallApiUpdate())
                        return;
                    trackingOutSave = new HIS_TRACKING();
                    trackingOutSave = new BackendAdapter(param).Post<HIS_TRACKING>(HisRequestUriStore.HIS_TRACKING_CREATE, ApiConsumers.MosConsumer, this.trackingSDOs, param);
                    WaitingManager.Show();
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

                        PrintProcess(PrintType.IN_TO_DIEU_TRI);

                    }
                }
                else if (this.action == GlobalVariables.ActionEdit)
                {
                    GetDataToSave();
                    if (!CheckBeforeCallApiUpdate())
                        return;
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("api/HisTracking/Update this.trackingSDOs: ", this.trackingSDOs));
                    trackingOutSave = new HIS_TRACKING();
                    trackingOutSave = new BackendAdapter(param).Post<HIS_TRACKING>(HisRequestUriStore.HIS_TRACKING_UPDATE, ApiConsumers.MosConsumer, this.trackingSDOs, param);
                    WaitingManager.Show();
                    if (trackingOutSave != null)
                    {
                        success = true;
                        this.action = GlobalVariables.ActionEdit;
                        currentTracking = new HIS_TRACKING();
                        currentTracking = trackingOutSave;
                        UpdateServiceReqInstructionDateByTrackingTime();

                        PrintProcess(PrintType.IN_TO_DIEU_TRI);

                    }
                }
                WaitingManager.Hide();

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
        private bool CheckBeforeCallApiUpdate()
        {
            bool result = true;
            try
            {
                if (trackingCreateOptionCFG == "2")
                {
                    if (StartTimeMustBeGreaterThanInstructionTime == "1" || StartTimeMustBeGreaterThanInstructionTime == "2")
                    {
                        WaitingManager.Hide();
                        HIS_SERVICE_REQ minStartTime = null;
                        HIS_SERVICE_REQ minFinishTime = null;
                        if (trackingSDOs.ServiceReqs != null && trackingSDOs.ServiceReqs.Count > 0)
                        {
                            minStartTime = rsServiceReq.Where(o => (StartTimeMustBeGreaterThanInstructionTime == "2" ? !new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT }.Contains(o.SERVICE_REQ_TYPE_ID) : true) && trackingSDOs.ServiceReqs.Exists(p => p.ServiceReqId == o.ID) && o.START_TIME != null).OrderBy(o => o.START_TIME).ThenBy(o => o.ID).FirstOrDefault();
                            minFinishTime = rsServiceReq.Where(o => (StartTimeMustBeGreaterThanInstructionTime == "2" ? !new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT }.Contains(o.SERVICE_REQ_TYPE_ID) : true) && trackingSDOs.ServiceReqs.Exists(p => p.ServiceReqId == o.ID) && o.FINISH_TIME != null).OrderBy(o => o.FINISH_TIME).ThenBy(o => o.ID).FirstOrDefault();
                        }
                        if (trackingSDOs.UsedForServiceReqIds != null && trackingSDOs.UsedForServiceReqIds.Count > 0)
                        {
                            var tmpMinSt = rsServiceReqTab2.Where(o => (StartTimeMustBeGreaterThanInstructionTime == "2" ? !new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT }.Contains(o.SERVICE_REQ_TYPE_ID) : true) && trackingSDOs.UsedForServiceReqIds.Contains(o.ID) && o.START_TIME != null).OrderBy(o => o.START_TIME).ThenBy(o => o.ID).FirstOrDefault();
                            minStartTime = minStartTime != null && tmpMinSt.START_TIME > minStartTime.START_TIME ? minStartTime : tmpMinSt;
                            var tmpMinFn = rsServiceReqTab2.Where(o => (StartTimeMustBeGreaterThanInstructionTime == "2" ? !new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT }.Contains(o.SERVICE_REQ_TYPE_ID) : true) && trackingSDOs.UsedForServiceReqIds.Contains(o.ID) && o.FINISH_TIME != null).OrderBy(o => o.FINISH_TIME).ThenBy(o => o.ID).FirstOrDefault();
                            minFinishTime = minFinishTime != null && tmpMinFn.FINISH_TIME > minFinishTime.FINISH_TIME ? minFinishTime : tmpMinFn;
                        }
                        HIS_SERVICE_REQ minData = minFinishTime;
                        bool IsStartTime = false;
                        if ((minStartTime != null ? minStartTime.START_TIME : Int64.MaxValue) < (minFinishTime != null ? minFinishTime.FINISH_TIME : Int64.MaxValue))
                        {
                            minData = minStartTime; IsStartTime = true;
                        }
                        if (minData != null && trackingSDOs.Tracking.TRACKING_TIME > (IsStartTime ? minData.START_TIME : minData.FINISH_TIME) && DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Thời gian tờ điều trị lớn hơn thời gian bắt đầu {0} y lệnh {1}.", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(IsStartTime ? minData.START_TIME ?? 0 : minData.FINISH_TIME ?? 0), (IsStartTime ? minData.SERVICE_REQ_CODE : minData.SERVICE_REQ_CODE)), "Thông báo") == DialogResult.OK)
                        {
                            result = false;
                        }

                    }
                    if (result && (ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1" || ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2"))
                    {
                        List<HIS_SERVICE_REQ> lstServiceReq = new List<HIS_SERVICE_REQ>();
                        if (trackingSDOs.ServiceReqs != null && trackingSDOs.ServiceReqs.Count > 0)
                        {
                            lstServiceReq.AddRange(rsServiceReq.Where(o => trackingSDOs.ServiceReqs.Exists(p => p.ServiceReqId == o.ID)));
                        }
                        if (trackingSDOs.UsedForServiceReqIds != null && trackingSDOs.UsedForServiceReqIds.Count > 0)
                        {
                            lstServiceReq.AddRange(rsServiceReqTab2.Where(o => trackingSDOs.UsedForServiceReqIds.Exists(p => p == o.ID)));
                        }
                        if (lstServiceReq != null && lstServiceReq.Count > 0)
                        {
                            CommonParam param = new CommonParam();
                            HisServiceReqCheckSereTimesSDO sdo = new HisServiceReqCheckSereTimesSDO();
                            sdo.TreatmentId = this.treatmentId;
                            sdo.Loginnames = lstServiceReq.Select(o => o.REQUEST_LOGINNAME).Distinct().ToList();
                            sdo.SereTimes = new List<long> { trackingSDOs.Tracking.TRACKING_TIME };
                            Inventec.Common.Logging.LogSystem.Debug("HisServiceReqCheckSereTimesSDO:" + LogUtil.TraceData("HisServiceReqCheckSereTimesSDO", sdo));
                            bool rs = new BackendAdapter(param).Post<bool>("/api/HisServiceReq/CheckSereTimes", ApiConsumers.MosConsumer, sdo, param);
                            if (!rs)
                            {
                                WaitingManager.Hide();
                                if (ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1")
                                {
                                    MessageManager.Show(this, param, rs);
                                    result = false;
                                }
                                else
                                {
                                    result = XtraMessageBox.Show(this, param.GetMessage() + "Bạn có muốn tiếp tục?", "Thông Báo", MessageBoxButtons.YesNo) == DialogResult.Yes;

                                }
                            }
                        }
                    }
                    if (result && ServiceReqIcdOption == "1")
                    {
                        WaitingManager.Hide();
                        if (Encoding.UTF8.GetByteCount((trackingSDOs.Tracking.ICD_CODE ?? "") + (trackingSDOs.Tracking.ICD_SUB_CODE ?? "")) > 100 && XtraMessageBox.Show(this, "Mã chẩn đoán phụ nhập quá 100 ký tự", "Thông Báo", MessageBoxButtons.OK) == DialogResult.OK)
                            result = false;
                        if (result && Encoding.UTF8.GetByteCount((trackingSDOs.Tracking.TRADITIONAL_ICD_CODE ?? "") + (trackingSDOs.Tracking.TRADITIONAL_ICD_SUB_CODE ?? "")) > 255 && XtraMessageBox.Show(this, "Mã chẩn đoán YHCT phụ nhập quá 255 ký tự", "Thông Báo", MessageBoxButtons.OK) == DialogResult.OK)
                            result = false;
                        if (result && Encoding.UTF8.GetByteCount((trackingSDOs.Tracking.ICD_NAME ?? "") + (trackingSDOs.Tracking.ICD_TEXT ?? "")) > 1500 && XtraMessageBox.Show(this, "Tên chẩn đoán phụ nhập quá 1500 ký tự", "Thông Báo", MessageBoxButtons.OK) == DialogResult.OK)
                            result = false;
                        if (result && (IsCheckSubIcdExceedLimit == "1" || IsCheckSubIcdExceedLimit == "2") && !string.IsNullOrEmpty(trackingSDOs.Tracking.ICD_SUB_CODE) && trackingSDOs.Tracking.ICD_SUB_CODE.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Count() > 12)
                        {
                            if (IsCheckSubIcdExceedLimit == "1")
                            {
                                XtraMessageBox.Show(this, "Chẩn đoán phụ nhập quá 12 mã bệnh. Vui lòng kiểm tra lại.", "Thông Báo", MessageBoxButtons.OK);
                                result = false;
                            }
                            else if (XtraMessageBox.Show(this, "Chẩn đoán phụ nhập quá 12 mã bệnh. Bạn có muốn tiếp tục?", "Thông Báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                result = false;
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void txtContent_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    FormKetLuanHoiChan.frmKetLuanHoiChan form = new FormKetLuanHoiChan.frmKetLuanHoiChan(this.treatmentId, SelectKetLuanHoiChan);
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectKetLuanHoiChan(object obj)
        {
            try
            {
                if (obj != null && obj is string)
                {
                    string ketLuan = obj as string;
                    string resultText = txtContent.Text;
                    if (!String.IsNullOrWhiteSpace(resultText))
                        resultText += "\r\n";
                    txtContent.Text = resultText + ketLuan;

                    txtContent.Select(txtContent.Text.Length, 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPhieuVoBenhAn_Click(object sender, EventArgs e)
        {
            try
            {
                EmrInputADO emrInputAdo = new EmrInputADO();
                emrInputAdo.PatientId = this._Treatment.PATIENT_ID;
                emrInputAdo.roomId = currentModule.RoomId;
                emrInputAdo.TreatmentId = this._Treatment.ID;

                if (this._Treatment.EMR_COVER_TYPE_ID != null && this._Treatment.EMR_COVER_TYPE_ID > 0)
                {
                    emrInputAdo.EmrCoverTypeId = this._Treatment.EMR_COVER_TYPE_ID;
                }
                else
                {
                    HisEmrCoverConfigFilter filter = new HisEmrCoverConfigFilter();
                    filter.WORKING_ROOM_ID = currentModule.RoomId;
                    if (this._Treatment.TDL_TREATMENT_TYPE_ID != null && this._Treatment.TDL_TREATMENT_TYPE_ID > 0)
                        filter.TREATMENT_TYPE_ID = this._Treatment.TDL_TREATMENT_TYPE_ID;
                    var emrCoverConfig = new BackendAdapter(new CommonParam()).Get<List<HIS_EMR_COVER_CONFIG>>("api/HisEmrCoverConfig/Get", ApiConsumers.MosConsumer, filter, null);

                    if (emrCoverConfig != null && emrCoverConfig.Count() == 1)
                    {
                        emrInputAdo.EmrCoverTypeId = emrCoverConfig.First().ID;
                    }
                    else if (emrCoverConfig != null && emrCoverConfig.Count() > 1)
                    {
                        emrInputAdo.lstEmrCoverTypeId = emrCoverConfig.Select(o => o.ID).ToList();
                    }
                    else
                    {
                        HisEmrCoverConfigFilter filter2 = new HisEmrCoverConfigFilter();
                        filter2.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).DepartmentId;
                        if (this._Treatment.TDL_TREATMENT_TYPE_ID != null && this._Treatment.TDL_TREATMENT_TYPE_ID > 0)
                            filter2.TREATMENT_TYPE_ID = this._Treatment.TDL_TREATMENT_TYPE_ID;
                        var emrCoverConfig2 = new BackendAdapter(new CommonParam()).Get<List<HIS_EMR_COVER_CONFIG>>("api/HisEmrCoverConfig/Get", ApiConsumers.MosConsumer, filter2, null);
                        if (emrCoverConfig2 != null && emrCoverConfig2.Count() == 1)
                            emrInputAdo.EmrCoverTypeId = emrCoverConfig2.First().ID;
                        else if (emrCoverConfig != null && emrCoverConfig2.Count() > 1)
                            emrInputAdo.lstEmrCoverTypeId = emrCoverConfig2.Select(o => o.ID).ToList();
                    }
                }

                HIS.Desktop.Plugins.Library.FormMedicalRecord.FromConfig.frmPhieu frmPhieu = new HIS.Desktop.Plugins.Library.FormMedicalRecord.FromConfig.frmPhieu(emrInputAdo);
                frmPhieu.Show();

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
                positionHandleControl = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider1, dxErrorProvider1);
                SetValueDefaultControl();
                EnableButton((currentTracking != null && currentTracking.ID > 0));
                //InitUCDHST();
                //FillDataDhstToControl(new HIS_DHST());


                //icdProcessor.Reload(ucIcd, null);
                //icdYhctProcessor.Reload(ucIcdYhct, null);
                //subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, null);


                txtGeneralExpression.Text = null;
                txtOrientationCapacity.Text = null;
                txtEmotion.Text = null;
                txtPerception.Text = null;
                txtContentOfThinking.Text = null;
                txtAwarenessBehavior.Text = null;
                txtInstinctivelyBehavior.Text = null;
                txtMemory.Text = null;
                txtIntellectual.Text = null;
                txtConcentration.Text = null;
                txtCardiovascular.Text = null;
                txtRespiratory.Text = null;

                txtEyeTensionLeft.Text = null;
                txtEyeTensionRight.Text = null;
                txtEyesightGlassLeft.Text = null;
                txtEyesightGlassRight.Text = null;
                txtEyesightLeft.Text = null;
                txtEyesightRight.Text = null;

                //txtIcdExtraName.Text = null;
                //txtIcdExtraCode.Text = null;
                txtContent.Text = null;
                txtMedicalInstruction.Text = null;
                txtResultCLS.Text = null;
                txtTheoDoiChamSoc.Text = null;
                memReha.Text = null;
                txtSheetOrder.Text = null;
                txtDiseaseStage.Text = null;
                this.currentTracking = null;
                if (action == GlobalVariables.ActionEdit)
                {
                    isSearch = false;
                    LoadDataSS(isSearch);
                    LoadDataSSTab2(isSearch);
                }
                else if (action == GlobalVariables.ActionAdd)
                {
                    BindingList<TreeSereServADO> records = new BindingList<TreeSereServADO>(SereServADOsFirstForm);
                    treeListServiceReq.DataSource = records;
                    treeListServiceReq.ExpandAll();
                    if (chkChiLayYLTuBBNew.Checked)
                    {
                        treeSereServ_CheckYLFromBB(treeListServiceReq.Nodes);
                    }
                    else
                    {
                        treeSereServ_CheckAllNode(treeListServiceReq.Nodes);
                    }
                    BindingList<TreeSereServADO> recordsTab2 = new BindingList<TreeSereServADO>(SereServADOsFirstFormTab2);
                    treeListPreventive.DataSource = recordsTab2;
                    treeListPreventive.ExpandAll();
                    treeSereServ_CheckAllNodeTab2(treeListPreventive.Nodes);

                }
                this.action = GlobalVariables.ActionAdd;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListPreventive_GetSelectImage(object sender, GetSelectImageEventArgs e)
        {
            try
            {
                var data = (TreeSereServADO)treeListPreventive.GetDataRecordByNode(e.Node);
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

        private void treeListPreventive_SelectImageClick(object sender, NodeClickEventArgs e)
        {
            try
            {
                var data = (TreeSereServADO)treeListPreventive.GetDataRecordByNode(e.Node);
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

        private void treeListPreventive_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeListPreventive.GetDataRecordByNode(e.Node) as TreeSereServADO;
                if (data != null)
                {
                    if (data.LEVER == 3)
                    {
                        var workingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                        string creator = data.CREATOR;
                        string reqLoginName = data.REQUEST_LOGINNAME;
                        long reqSttId = data.SERVICE_REQ_STT_ID;
                        long serReqTypeId = data.TDL_SERVICE_REQ_TYPE_ID;
                        short? isNoExecute = data.IS_NO_EXECUTE;
                        long requestDepartmentId = data.TDL_REQUEST_DEPARTMENT_ID;

                        if (e.Column.FieldName == "ServiceReqDelete")
                        {
                            if (data.TDL_TRACKING_ID != null)
                            {
                                e.RepositoryItem = repositoryItemServiceReqDeleteDisablePreventive;
                            }
                            else if ((creator == this.loginName || reqLoginName == this.loginName || CheckLoginAdmin.IsAdmin(this.loginName) || (this.currentRoom != null && requestDepartmentId == this.currentRoom.DEPARTMENT_ID && serReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                    && (reqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                    && data.TDL_TRACKING_ID == null)
                            {
                                e.RepositoryItem = repositoryItemServiceReqDeletePreventive;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemServiceReqDeleteDisablePreventive;
                            }
                        }
                        else if (e.Column.FieldName == "ServiceReqEdit")
                        {
                            if (serReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN || data.TDL_TRACKING_ID != null)
                            {
                                e.RepositoryItem = repositoryItemServiceReqEditDisablePreventive;
                            }
                            else if ((creator == this.loginName || reqLoginName == this.loginName || CheckLoginAdmin.IsAdmin(this.loginName))
                                 && (((reqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                 && isNoExecute != 1)
                                 || ((reqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                 && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1"))
                                 && data.TDL_TRACKING_ID == null)
                            {
                                e.RepositoryItem = repositoryItemServiceReqEditPreventive;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemServiceReqEditDisablePreventive;
                            }
                        }
                        else if (e.Column.FieldName == "IMG")
                        {
                            e.RepositoryItem = repositoryItempicServiceReqStatusPreventive;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListPreventive_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (TreeSereServADO)treeListPreventive.GetDataRecordByNode(e.Node);
                if (e.Node.HasChildren)
                {
                    if (data.IS_THUHOI)
                    {
                        e.Appearance.ForeColor = Color.Red;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else if (data.IS_OUT_MEDI_MATE)
                    {
                        e.Appearance.ForeColor = Color.Red;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else if (data.IS_RATION)
                    {
                        e.Appearance.ForeColor = Color.Green;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
                else
                {
                    e.Appearance.ForeColor = Color.Black;
                }
                if (e.Node.Checked && this._ServiceReqByTrackingsTab2 != null && this._ServiceReqByTrackingsTab2.Count > 0)
                {
                    foreach (var item in this._ServiceReqByTrackingsTab2)
                    {
                        if (data.SERVICE_REQ_ID == item.ID)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListPreventive_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                var data = (TreeSereServADO)treeListPreventive.GetDataRecordByNode(e.Node);
                if (data != null && e.Column.FieldName == "AMOUNT_STR" && !e.Node.HasChildren)
                {
                    e.Value = data.AMOUNT;
                }
                else if (e.Column.FieldName == "IMG" && data.LEVER == 3)
                {
                    try
                    {
                        long statusId = data.SERVICE_REQ_STT_ID;
                        if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                        else
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot icon trang thai yeu cau dich vu IMG", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListPreventive_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            try
            {
                if (e.Node.HasChildren)
                {
                    //e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
                    if (e.Node.Checked)
                    {
                        e.Node.UncheckAll();
                    }
                    else
                    {
                        e.Node.CheckAll();
                    }
                    TreeListNode node = e.Node;
                    CheckNodesParent(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListPreventive_AfterCheckNode(object sender, NodeEventArgs e)
        {

        }

        private void treeListPreventive_CustomDrawNodeCheckBox(object sender, CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                if (!e.Node.HasChildren)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemServiceReqDeletePreventive_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var rowData = treeListPreventive.GetDataRecordByNode(treeListServiceReq.FocusedNode) as TreeSereServADO;
                if (rowData == null)
                {
                    //Inventec.Common.Logging.LogSystem.Info("rowData thuc hien huy yeu cau dich vu null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowData), rowData));
                    return;
                }

                if (MessageBox.Show(Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (CheckParentBeforeDelete(rowData.ID) && DevExpress.XtraEditors.XtraMessageBox.Show("Đã có y lệnh đính kèm (CLS). Bạn có chắc chắn muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }

                    WaitingManager.Show();
                    MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                    sdo.Id = rowData.SERVICE_REQ_ID;
                    sdo.RequestRoomId = this.currentModule.RoomId;
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(UriStores.HIS_SERVICE_REQ_DELETE, ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    WaitingManager.Hide();
                    if (success)
                    {
                        isSearch = false;
                        //backgroundWorker2.RunWorkerAsync();
                        LoadDataSSTab2(false);
                    }

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void repositoryItemServiceReqEditPreventive_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = treeListPreventive.GetDataRecordByNode(treeListPreventive.FocusedNode) as TreeSereServADO;
                if (data != null)// && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                {
                    var paramCommon = new CommonParam();
                    var treatment = new HIS_TREATMENT();
                    HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                    treatFilter.ID = data.TDL_TREATMENT_ID;
                    var currentTreats = new BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>(UriStores.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (currentTreats != null && currentTreats.Count == 1)
                    {
                        var treat = currentTreats.FirstOrDefault();
                        if (treat.IS_PAUSE == 1 || treat.IS_ACTIVE != 1)
                        {
                            //Inventec.Common.Logging.LogSystem.Debug(Resources.ResourceMessage.HoSoDieuTriDangTamKhoa);
                            MessageBox.Show(Resources.ResourceMessage.HoSoDieuTriDangTamKhoa);
                            return;
                        }
                    }
                    else
                    {
                        //Inventec.Common.Logging.LogSystem.Debug(Resources.ResourceMessage.KhongTimThayHoSoDieuTri);
                        MessageBox.Show(Resources.ResourceMessage.KhongTimThayHoSoDieuTri);
                        return;
                    }

                    var serviceReqPrintRaw = GetServiceReqForPrint(data.SERVICE_REQ_ID ?? 0);

                    if (data.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                    {
                        WaitingManager.Show();
                        List<object> sendObj = new List<object>() { serviceReqPrintRaw.ID };
                        CallModule("HIS.Desktop.Plugins.UpdateExamServiceReq", sendObj);
                        WaitingManager.Hide();
                    }
                    else if (data.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                        data.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT ||
                        data.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                    {
                        WaitingManager.Show();
                        AssignPrescriptionEditADO assignEditADO = null;
                        var serviceReq = new HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(serviceReq, serviceReqPrintRaw);
                        HisExpMestFilter expfilter = new HisExpMestFilter();
                        expfilter.SERVICE_REQ_ID = serviceReqPrintRaw.ID;
                        var expMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (expMests != null && expMests.Count == 1)
                        {
                            var expMest = expMests.FirstOrDefault();
                            if (expMest.IS_NOT_TAKEN.HasValue && expMest.IS_NOT_TAKEN.Value == 1)
                            {
                                WaitingManager.Hide();
                                MessageBox.Show(Resources.ResourceMessage.DonKhongLayKhongChoPhepSua);
                                return;
                            }
                            assignEditADO = new AssignPrescriptionEditADO(serviceReq, expMest, FillDataApterSaveTab2);
                        }
                        else
                        {
                            assignEditADO = new AssignPrescriptionEditADO(serviceReq, null, FillDataApterSaveTab2);
                        }

                        if (data.IS_EXECUTE_KIDNEY_PRES == 1)
                        {
                            AssignPrescriptionKidneyADO assignPrescriptionKidneyADO = new AssignPrescriptionKidneyADO();
                            assignPrescriptionKidneyADO.AssignPrescriptionEditADO = assignEditADO;
                            List<object> sendObj = new List<object>() { assignPrescriptionKidneyADO };

                            CallModule("HIS.Desktop.Plugins.AssignPrescriptionKidney", sendObj);
                        }
                        else
                        {
                            var assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(data.TDL_TREATMENT_ID ?? 0, 0, serviceReq.ID);
                            assignServiceADO.GenderName = data.TDL_PATIENT_GENDER_NAME;
                            assignServiceADO.PatientDob = data.TDL_PATIENT_DOB;
                            assignServiceADO.PatientName = data.TDL_PATIENT_NAME;

                            assignServiceADO.AssignPrescriptionEditADO = assignEditADO;

                            List<object> sendObj = new List<object>() { assignServiceADO };

                            if (data.PRESCRIPTION_TYPE_ID == 1)
                            {
                                CallModule("HIS.Desktop.Plugins.AssignPrescriptionPK", sendObj);
                            }
                            else if (data.PRESCRIPTION_TYPE_ID == 2)
                            {
                                CallModule("HIS.Desktop.Plugins.AssignPrescriptionYHCT", sendObj);
                            }
                            else if (data.PRESCRIPTION_TYPE_ID == 3)
                            {
                                CallModule("HIS.Desktop.Plugins.AssignPrescriptionCLS", sendObj);
                            }
                        }

                        WaitingManager.Hide();
                    }
                    else if (data.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM)
                    {
                        // MessageManager.Show(Resources.ResourceMessage.DonMauKhongChoPhepSua);
                        var serviceReq = new HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(serviceReq, serviceReqPrintRaw);

                        HIS.Desktop.ADO.AssignBloodADO assignBloodADO = new HIS.Desktop.ADO.AssignBloodADO(data.TDL_TREATMENT_ID ?? 0, 0, 0);
                        assignBloodADO.PatientDob = data.TDL_PATIENT_DOB;
                        assignBloodADO.DgProcessDataResult = FillDataApterSave;
                        assignBloodADO.PatientName = data.TDL_PATIENT_NAME;
                        assignBloodADO.GenderName = data.TDL_PATIENT_GENDER_NAME;
                        List<object> sendObj = new List<object>() { assignBloodADO, serviceReq };
                        CallModule("HIS.Desktop.Plugins.HisAssignBlood", sendObj);
                    }
                    else
                    {
                        AssignServiceEditADO assignServiceEditADO = new AssignServiceEditADO(data.SERVICE_REQ_ID ?? 0, data.TDL_INTRUCTION_TIME, (HIS.Desktop.Common.RefeshReference)RefreshClick);
                        List<object> sendObj = new List<object>() { assignServiceEditADO };
                        CallModule("HIS.Desktop.Plugins.AssignServiceEdit", sendObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataApterSaveTab2(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    btnSearchNewTab2_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnSearchNewTab2_Click(object sender, EventArgs e)
        {
            try
            {
                isSearch = true;
                //backgroundWorker2.RunWorkerAsync();
                LoadDataSSTab2(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboUser()
        {
            ListUsser = BackendDataWorker.Get<ACS_USER>() ?? new List<ACS_USER>();
            ListUsser = ListUsser.Where(o => !string.IsNullOrEmpty(o.LOGINNAME) && !string.IsNullOrEmpty(o.USERNAME)).ToList();
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
            columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
            ControlEditorLoader.Load(cboLogin, ListUsser, controlEditorADO);
            cboLogin.Properties.ImmediatePopup = true;
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtLoginName.Text))
                    {
                        var check = ListUsser.FirstOrDefault(o => o.LOGINNAME == txtLoginName.Text.Trim());
                        if (check != null)
                        {
                            cboLogin.EditValue = check.LOGINNAME;
                            dteFromPreventive.Focus();
                        }
                        else
                        {
                            cboLogin.Focus();
                            cboLogin.ShowPopup();
                        }
                    }
                    else
                    {
                        cboLogin.Focus();
                        cboLogin.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLogin_EditValueChanged(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControl2_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                //            if(IsFirstLoadForm)
                //{
                //                WaitingManager.Show();
                //                LoadDataSSTab2(false);
                //                WaitingManager.Hide();
                //            }
                //            IsFirstLoadForm = false;

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemServiceReqUseTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                repositoryItemServiceReqEdit_ButtonClick(null, null);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLogin_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {

                    cboLogin.EditValue = null;
                    txtLoginName.Text = "";

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLogin_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboLogin.EditValue != null)
                {

                    var check = ListUsser.FirstOrDefault(o => o.LOGINNAME == cboLogin.EditValue.ToString());
                    if (check != null)
                    {
                        txtLoginName.Text = check.LOGINNAME;
                        dteFromPreventive.Focus();
                    }
                    else
                    {

                        cboLogin.ShowPopup();
                    }
                }
                else
                {

                    cboLogin.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }

        private void frmTrackingCreateNew_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //PrintDocumentSigned
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdatePrintDocumentSigned = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkPrintDocumentSigned && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdatePrintDocumentSigned != null)
                {
                    csAddOrUpdatePrintDocumentSigned.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdatePrintDocumentSigned = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdatePrintDocumentSigned.KEY = ControlStateConstan.chkPrintDocumentSigned;
                    csAddOrUpdatePrintDocumentSigned.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                    csAddOrUpdatePrintDocumentSigned.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdatePrintDocumentSigned);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

                // print
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdatePrint = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkPrint && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdatePrint), csAddOrUpdatePrint));
                if (csAddOrUpdatePrint != null)
                {
                    csAddOrUpdatePrint.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdatePrint = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdatePrint.KEY = ControlStateConstan.chkPrint;
                    csAddOrUpdatePrint.VALUE = (chkPrint.Checked ? "1" : "");
                    csAddOrUpdatePrint.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdatePrint);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

                // sign
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateSign = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkSign && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateSign != null)
                {
                    csAddOrUpdateSign.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdateSign = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateSign.KEY = ControlStateConstan.chkSign;
                    csAddOrUpdateSign.VALUE = (chkSign.Checked ? "1" : "");
                    csAddOrUpdateSign.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateSign);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

                //ChiLayYLTuBB
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateChiLayYLTuBB = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkChiLayYLTuBB && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateChiLayYLTuBB != null)
                {
                    csAddOrUpdateChiLayYLTuBB.VALUE = (chkChiLayYLTuBBNew.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdateChiLayYLTuBB = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateChiLayYLTuBB.KEY = ControlStateConstan.chkChiLayYLTuBB;
                    csAddOrUpdateChiLayYLTuBB.VALUE = (chkChiLayYLTuBBNew.Checked ? "1" : "");
                    csAddOrUpdateChiLayYLTuBB.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateChiLayYLTuBB);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

                // UpdateTimeDHST
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateTimeDHST = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkUpdateTimeDHST && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateTimeDHST != null)
                {
                    csAddOrUpdateTimeDHST.VALUE = (chkUpdateTimeDHST.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdateTimeDHST = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateTimeDHST.KEY = ControlStateConstan.chkUpdateTimeDHST;
                    csAddOrUpdateTimeDHST.VALUE = (chkUpdateTimeDHST.Checked ? "1" : "");
                    csAddOrUpdateTimeDHST.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateTimeDHST);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmTrackingCreateNew
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TrackingCreate.Resources.Lang", typeof(frmTrackingCreateNew).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPhieuVoBenhAn.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnPhieuVoBenhAn.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveSign.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnSaveSign.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveTemp.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnSaveTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChamSoc.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnChamSoc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAssService.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnAssService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKeDonYHCT.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnKeDonYHCT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKeDonThuoc.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnKeDonThuoc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTuTruc.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnTuTruc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKeDonMau.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnKeDonMau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintDocumentSigned.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.chkPrintDocumentSigned.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSign.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.chkSign.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrint.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.chkPrint.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchNew.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnSearchNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemPrint.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.barButtonItemPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSearch.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.barButtonItemSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnTrackingTemp.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.bbtnTrackingTemp.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__AssService.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.barButtonItem__AssService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__KeDOnThuoc.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.barButtonItem__KeDOnThuoc.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barLuuKy.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.barLuuKy.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem7.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.barButtonItem7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkChiLayYLTuBBNew.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.chkChiLayYLTuBBNew.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsMineNew.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.chkIsMineNew.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServiceReq_Edit.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.gridColumn_ServiceReq_Edit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServiceReq_Delete.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.gridColumn_ServiceReq_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServiceReq_Stt.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.gridColumn_ServiceReq_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem53.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem53.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem54.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem54.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem54.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem54.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem55.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem55.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem56.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem56.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage5.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.xtraTabPage5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServiceReq_Edit_Preventive.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.gridColumn_ServiceReq_Edit_Preventive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServiceReq_Delete_Preventive.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.gridColumn_ServiceReq_Delete_Preventive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServiceReq_Stt_Preventive.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.gridColumn_ServiceReq_Stt_Preventive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1_Preventive.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.treeListColumn1_Preventive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3_Preventive.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.treeListColumn3_Preventive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2_Preventive.Caption = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.treeListColumn2_Preventive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchNewTab2.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnSearchNewTab2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLogin.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.cboLogin.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem47.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem47.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDanhSachMau.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnDanhSachMau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnLuuMau.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnLuuMau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoseResult.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.btnChoseResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageDhst.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.xtraTabPageDhst.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem39.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem39.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem40.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem40.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem41.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem41.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem42.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem43.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem43.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGeneralExpression.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciGeneralExpression.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEmotion.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciEmotion.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciContentOfThinking.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciContentOfThinking.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInstinctivelyBehavior.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciInstinctivelyBehavior.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIntellectual.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciIntellectual.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOrientationCapacity.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciOrientationCapacity.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConcentration.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciConcentration.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPerception.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciPerception.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAwarenessBehavior.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciAwarenessBehavior.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMemory.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciMemory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage4.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.xtraTabPage4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCardiovascular.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciCardiovascular.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRespiratory.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lciRespiratory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTheoDoiChamSoc.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.txtTheoDoiChamSoc.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtResultCLS.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.txtResultCLS.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtContent.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.txtContent.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlISubIcdYhct.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlISubIcdYhct.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.txtIcdExtraName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.txtIcdExtraName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlIcdYhct.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlIcdYhct.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlUcIcd.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlUcIcd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTrackingOld.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.cboTrackingOld.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTrackingTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.cboTrackingTemp.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem50.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.layoutControlItem50.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignPan_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS.UC.Icd.ADO.IcdInputADO icdADO = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                    SecondaryIcdDataADO icdSubADO = SubUcIcdGetValue() as SecondaryIcdDataADO;
                    listArgs.Add(icdADO);
                    listArgs.Add(icdSubADO);
                    listArgs.Add(this.treatmentId);
                    listArgs.Add(this.currentTracking);
                    listArgs.Add((HIS.Desktop.Common.RefeshReference)DelegateActionSave);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void DelegateActionSave()
        {
            try
            {
                RefreshClick();
                RefreshClickTab2();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkUpdateTimeDHST_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.action == GlobalVariables.ActionAdd && chkUpdateTimeDHST.Checked)
                {
                    if (dtTrackingTime != null && dtTrackingTime.DateTime != DateTime.MinValue)
                        dhstProcessor.SetExecuteTime(ucControlDHST, Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkLockInfor_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkLockInfor.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkLockInfor.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkLockInfor.Name;
                    csAddOrUpdate.VALUE = (chkLockInfor.Checked ? "1" : "");
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
