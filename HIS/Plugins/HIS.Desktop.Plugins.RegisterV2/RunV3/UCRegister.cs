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
using CHC.WCFClient.CheckHeinCardService;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.RegisterV2.Choice;
using HIS.Desktop.Utility;
using HIS.UC.AddressCombo.ADO;
using HIS.UC.KskContract;
using HIS.UC.KskContract.ADO;
using HIS.UC.ServiceRoom;
using HIS.UC.UCPatientRaw.ADO;
using HIS.UC.UCTransPati.ADO;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        #region Delcare
        internal HisPatientSDO currentPatientSDO { get; set; }
        internal HisCardSDO cardSearch = null;
        internal UCTransPatiADO transPatiADO = null;
        internal List<ServiceReqDetailSDO> serviceReqDetailSDOs;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        internal KskContractProcessor kskContractProcessor;
        internal UserControl ucKskContract;
        internal int registerNumber = 0;
        internal bool isShowMess;
        internal List<long> serviceReqPrintIds { get; set; }
        const string IsDefaultRightRouteType__True = "1";

        List<HIS_PATIENT_TYPE> currentPatientTypeAllowByPatientType;
        internal UserControl ucHeinBHYT;
        internal His.UC.UCHein.MainHisHeinBhyt mainHeinProcessor;
        RoomExamServiceProcessor roomExamServiceProcessor;
        UCAddressADO dataAddressPatient = new UCAddressADO();
        UCPatientRawADO dataPatientRaw = new UCPatientRawADO();
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;
        frmTransPati frm;
        HisServiceReqExamRegisterResultSDO currentHisExamServiceReqResultSDO { get; set; }
        HisPatientProfileSDO resultHisPatientProfileSDO = null;
        Inventec.Common.QrCodeBHYT.HeinCardData _HeinCardData { get; set; }
        ResultDataADO ResultDataADO { get; set; }
        public bool isCheckSS { get; set; }
        internal bool isNotPatientDayDob = false;
        int actionType = 0;
        bool isPrintNow;
        bool isReadQrCode = true;
        string appointmentCode = "";
        bool ValidatedTTCT { get; set; }
        bool IsPresent { get; set; }
        bool IsPresentAndAppointment { get; set; }
        long _TreatmnetIdByAppointmentCode = 0;
        bool _isPatientAppointmentCode = false;
        bool isResetForm = false;
        bool isNotLoadWhileChangeControlStateInFirst;
        const string moduleLink = "HIS.Desktop.Plugins.RegisterV2";
        const string moduleLinkConfigCall = "HIS.Desktop.Plugins.RegisterV2.frmConfigCall";
        const long PRIORITY_TRUE = 1;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDOModuleLink;
        string CallConfigString = "";
        bool isAlertTreatmentEndInDay { get; set; }
        bool isNotCheckTT = false;
        bool _IsDungTuyenCapCuuByTime = false;
        bool isSaveWithRoomHasConfigAllowNotChooseService = false;
        long roomId;
        string numSttNow = "0";
        string numTotal;
        string txtNumberPer = "";
        string baseNameControl = "";
        bool IsEmergency = false;
        List<V_HIS_SERVICE> lstService;
        bool IsActionSavePrint = false;
        #endregion

        #region Construct - Load
        public UCRegister(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCRegister .1");
                this.currentModule = module;
                BackendDataWorker.Reset<HIS_PATIENT_TYPE>();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister .2");
                HisConfigCFG.LoadConfig();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister .3");
                AppConfigs.LoadConfig();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister .4");
                InitializeComponent();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister .5");
                this.roomId = module.RoomId;
                SetCaptionByLanguageKey();
                LoadServiceFromRam();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister .6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServiceFromRam()
        {
            try
            {
                lstService = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCRegister_Load(object sender, EventArgs e)
        {
            try
            {

                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load .1");
                timerInitForm.Enabled = true;
                timerInitForm.Interval = 1000;
                RegisterTimer(currentModule.ModuleLink, "timerInitForm", timerInitForm.Interval, timerInitForm_Tick);
                StartTimer(currentModule.ModuleLink, "timerInitForm");
                timerRefeshAutoCreateBill.Enabled = true;
                timerRefeshAutoCreateBill.Interval = 2000;
                RegisterTimer(currentModule.ModuleLink, "timerRefeshAutoCreateBill", timerRefeshAutoCreateBill.Interval, timerRefeshAutoCreateBill_Tick);
                StartTimer(currentModule.ModuleLink, "timerRefeshAutoCreateBill");
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load .2");
                //this.ConfigLayout();
                this.InitControlState();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load .3");
                this.ucPatientRaw1.LoadDataCboDoiTuong(roomId);
                this.ucPatientRaw1.InitData(PatientTypeEditValueChanged, GetIntructionTime, PatientClassifyChanged);
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load .4");
                SetDelegateForResetRegister();
                this.actionType = GlobalVariables.ActionAdd;

                this.SetDefaultRegisterForm();
                LogSystem.Debug("UCRegister_Load .5");

                this.CreateThreadInitWCFReadCard();
                LogSystem.Debug("UCRegister_Load .6");

                LogSystem.Debug("UCRegister_Load .7");
                LoadDefaultScreenSaver();

                LogSystem.Debug("UCRegister_Load .8");
                if (!string.IsNullOrEmpty(HisConfigCFG.ModuleLinkApply))
                {
                    lstModuleLinkApply = HisConfigCFG.ModuleLinkApply.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load .9");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PatientClassifyChanged(long? PatientClassifyId)
        {
            try
            {
                if (ucServiceRoomInfo1 != null)
                    ucServiceRoomInfo1.SetPatientClassify(PatientClassifyId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCRegister
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RegisterV2.Resources.Lang", typeof(UCRegister).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCRegister.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCRegister.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoPay.Properties.Caption = Inventec.Common.Resource.Get.Value("UCRegister.chkAutoPay.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoPay.ToolTip = Inventec.Common.Resource.Get.Value("UCRegister.chkAutoPay.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoDeposit.Properties.Caption = Inventec.Common.Resource.Get.Value("UCRegister.chkAutoDeposit.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoDeposit.ToolTip = Inventec.Common.Resource.Get.Value("UCRegister.chkAutoDeposit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAssignDoctor.Properties.Caption = Inventec.Common.Resource.Get.Value("UCRegister.chkAssignDoctor.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAssignDoctor.ToolTip = Inventec.Common.Resource.Get.Value("UCRegister.chkAssignDoctor.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintExam.Properties.Caption = Inventec.Common.Resource.Get.Value("UCRegister.chkPrintExam.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoCreateBill.Properties.Caption = Inventec.Common.Resource.Get.Value("UCRegister.chkAutoCreateBill.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoCreateBill.ToolTip = Inventec.Common.Resource.Get.Value("UCRegister.chkAutoCreateBill.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintPatientCard.Properties.Caption = Inventec.Common.Resource.Get.Value("UCRegister.chkPrintPatientCard.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintPatientCard.ToolTip = Inventec.Common.Resource.Get.Value("UCRegister.chkPrintPatientCard.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTTChuyenTuyen.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnTTChuyenTuyen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCRegister.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTo.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.txtTo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtFrom.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.txtFrom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnGiayTo.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnGiayTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDepositRequest.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnDepositRequest.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dropDownButton__Other.Text = Inventec.Common.Resource.Get.Value("UCRegister.dropDownButton__Other.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRecallPatient.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnRecallPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRecallPatient.ToolTip = Inventec.Common.Resource.Get.Value("UCRegister.btnRecallPatient.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCallPatient.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnCallPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtGateNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRegister.txtGateNumber.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtGateNumber.ToolTip = Inventec.Common.Resource.Get.Value("UCRegister.txtGateNumber.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtStepNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRegister.txtStepNumber.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCashierRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboCashierRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPatientNew.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnPatientNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPatientNew.ToolTip = Inventec.Common.Resource.Get.Value("UCRegister.btnPatientNew.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveAndAssain.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnSaveAndAssain.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDepositDetail.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnDepositDetail.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTreatmentBedRoom.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnTreatmentBedRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNewContinue.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnNewContinue.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCRegister.layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCRegister.layoutControlItem8.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCRegister.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCRegister.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultScreenSaver()
        {
            try
            {

                List<object> _listObj = new List<object>();
                WaitingManager.Hide();
                var SCREEN_SAVER = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                if (SCREEN_SAVER != null)
                {
                    var dtDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == SCREEN_SAVER.DEPARTMENT_ID);
                    if (dtDepartment != null)
                    {
                        ucPatientRaw1.SetEmergencyFromDepartment(dtDepartment.IS_EMERGENCY == (short?)1);
                        IsEmergency = dtDepartment.IS_EMERGENCY == (short?)1;
                    }
                    if (!string.IsNullOrEmpty(SCREEN_SAVER.SCREEN_SAVER_MODULE_LINK))
                    {
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(SCREEN_SAVER.SCREEN_SAVER_MODULE_LINK, this.currentModule.RoomId, this.currentModule.RoomTypeId, _listObj);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void timerInitForm_Tick()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timer1_Tick .1");
                StopTimer(currentModule.ModuleLink, "timerInitForm");
                Inventec.Common.Logging.LogSystem.Debug("timer1_Tick .2");
                this.FocusNextUserControl();
                Inventec.Common.Logging.LogSystem.Debug("timer1_Tick .3");
                this.InitInputDataUCHeinInfo();
                Inventec.Common.Logging.LogSystem.Debug("timer1_Tick .4");
                this.SetDefaultCashierRoom();
                Inventec.Common.Logging.LogSystem.Debug("timer1_Tick .5");
                this.InitPopupMenuOther();
                Inventec.Common.Logging.LogSystem.Debug("timer1_Tick .6");
                this.GATE();

                this.ucOtherServiceReqInfo1.InitFieldFromAsync();
                this.ucRelativeInfo1.InitFieldFromAsync();
                this.ucPatientRaw1.FocusUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void TimerRefeshAutoCreateBill_Tick(object sender, System.EventArgs e)
        {
            timerRefeshAutoCreateBill_Tick();
        }
        private void timerRefeshAutoCreateBill_Tick()
        {
            try
            {
                bool autoCreateBill = (GlobalVariables.AuthorityAccountBook != null && GlobalVariables.AuthorityAccountBook.AccountBookId.HasValue);
                if (!chkAutoDeposit.Checked)
                    chkAutoCreateBill.Checked = autoCreateBill;
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
                        if (item.KEY == chkPrintPatientCard.Name)
                        {
                            chkPrintPatientCard.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkAutoCreateBill.Name)
                        {
                            chkAutoCreateBill.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkAutoDeposit.Name)
                        {
                            chkAutoDeposit.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkPrintExam.Name)
                        {
                            chkPrintExam.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkAssignDoctor.Name)
                        {
                            chkAssignDoctor.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkAutoPay.Name)
                        {
                            chkAutoPay.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == txtGateNumber.Name)
                        {
                            txtGateNumber.Text = item.VALUE;
                        }
                        else if (item.KEY == txtStepNumber.Name)
                        {
                            txtStepNumber.Text = item.VALUE;
                        }
                        else if (item.KEY == chkSignExam.Name)
                        {
                            chkSignExam.Checked = item.VALUE == "1";
                        }
                    }
                }
                this.currentControlStateRDOModuleLink = controlStateWorker.GetData(moduleLinkConfigCall);
                foreach (var item in this.currentControlStateRDOModuleLink)
                {
                    if (item.KEY == moduleLinkConfigCall)
                    {
                        CallConfigString = item.VALUE;
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private Method
        DateTime GetIntructionTime()
        {
            try
            {
                return Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.ucOtherServiceReqInfo1.GetValue().IntructionTime).Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return new DateTime();
        }

        bool GetIsChild()
        {
            bool valid = false;
            try
            {
                valid = ucPatientRaw1.GetIsChild();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        void PatientTypeEditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("PatientTypeEditValueChanged => 1");
                long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(((DevExpress.XtraEditors.BaseEdit)sender).EditValue.ToString());
                if (patientTypeId > 0)
                {
                    WaitingManager.Show();

                    this.ucHeinInfo1.SetTypeCardTemp(patientTypeId);

                    if (patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN || this.GetIsChild())
                        this.ucHeinInfo1.SetDisableHasCardTemp(true);

                    SuspendLayoutWithPatientTypeChanged(patientTypeId);

                    if (patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT ||
                        patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN)
                    {
                        this.ucHeinInfo1.InitValidateRule(patientTypeId);
                        this.ucHeinInfo1.InitFieldFromAsync();
                        DelegateFocusNextUserControl dlg = this.ucHeinInfo1.FocusUserControl;
                        this.ucAddressCombo1.FocusNextUserControl(dlg);
                    }
                    else if (patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__KSK)
                    {
                        if (this.ucKskContract != null && this.kskContractProcessor != null)
                        {
                            DelegateFocusNextUserControl dlg = FocusInKskContract;
                            this.ucAddressCombo1.FocusNextUserControl(dlg);
                        }
                        this.ucHeinInfo1.RefreshUserControl();
                        this.ucHeinInfo1.ResetRequiredField();
                    }
                    else
                    {
                        if (this.ucServiceRoomInfo1 != null)
                        {
                            DelegateFocusNextUserControl dlg = this.ucServiceRoomInfo1.FocusUserControl;
                            this.ucAddressCombo1.FocusNextUserControl(dlg);
                        }
                        this.ucHeinInfo1.RefreshUserControl();
                        this.ucHeinInfo1.ResetRequiredField();
                    }

                    this.ucOtherServiceReqInfo1.ChangePatientType(patientTypeId);
                    //Lấy danh sách id các đối tượng thanh toán được phép chuyển đổi từ đối tượng bệnh nhân
                    GlobalStore.PatientTypeIdAllows = (BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>()
                        .Where(o => o.PATIENT_TYPE_ID == patientTypeId)
                        .Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList());

                    //Lay danh sach doi tuong kham theo doi tuong benh nhan da chon, 
                    //sau do set gia tri mac dinh cho doi tuong kham
                    this.currentPatientTypeAllowByPatientType = this.LoadPatientTypeExamByPatientType(patientTypeId);

                    this.ucServiceRoomInfo1.RefreshUserControl();

                    this.ReloadExamServiceRoom();
                    if (this.ucPatientRaw1.GetValue().DOB != null && patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                    {
                        DateTime dateofbirth = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.ucPatientRaw1.GetValue().DOB) ?? DateTime.MinValue;
                        if (dateofbirth != DateTime.MinValue)
                        {
                            if (DateTime.Now.Year - dateofbirth.Year < 6)
                            {
                                this.ucHeinInfo1.SetEnableChkSS(true);

                            }
                            else
                            {
                                this.ucHeinInfo1.SetEnableChkSS(false);
                            }
                        }

                    }
                    if (HisConfigCFG.AutoCheckPrintExam__PatientTypeIds != null && HisConfigCFG.AutoCheckPrintExam__PatientTypeIds.Count > 0 && HisConfigCFG.AutoCheckPrintExam__PatientTypeIds.Contains(patientTypeId))
                    {
                        chkPrintExam.Checked = true;
                    }
                    else if (HisConfigCFG.AutoCheckPrintExam__PatientTypeIds != null && HisConfigCFG.AutoCheckPrintExam__PatientTypeIds.Count > 0)
                        chkPrintExam.Checked = false;
                    else
                    {
                        //Mặc định check theo lần sử dụng trước đó, đã xử lý ở hàm InitControlState bên trên
                    }

                    WaitingManager.Hide();
                    this.ucPatientRaw1.FocusUserControl();
                }
                LogSystem.Debug("PatientTypeEditValueChanged => 2");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hàm xử lý thay đổi layout các vùng thông tin bhyt, phòng-dịch vụ khi có sự thay đổi đối tượng bệnh nhân cần hiển thị giao diện tương ứng
        /// </summary>
        /// <param name="patientTypeId"></param>
        void SuspendLayoutWithPatientTypeChanged(long patientTypeId)
        {
            Inventec.Common.Logging.LogSystem.Debug("SuspendLayoutWithPatientTypeChanged.1");
            layoutControl1.BeginUpdate();
            layoutControl1.SuspendLayout();
            if ((patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT > 0) || (patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN > 0))
            {
                Inventec.Common.Logging.LogSystem.Debug("SuspendLayoutWithPatientTypeChanged.2");
                //Phải chạy 2 lần mới thay đổi layout
                lciUCHeinInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                //Phải chạy 2 lần mới thay đổi layout
                lciUCServiceRoomInfo.BeginInit();
                lciUCServiceRoomInfo.OptionsTableLayoutItem.RowIndex = 16;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnIndex = 0;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.RowSpan = 3;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnSpan = 2;
                lciUCServiceRoomInfo.EndInit();

                lciUCHeinInfo.BeginInit();
                Control ucHeinInfoTemp = lciUCHeinInfo.Control;
                lciUCHeinInfo.Control = this.ucHeinInfo1;
                ucHeinInfoTemp.Parent = null;
                lciUCHeinInfo.OptionsTableLayoutItem.RowIndex = 9;
                lciUCHeinInfo.OptionsTableLayoutItem.ColumnIndex = 0;
                lciUCHeinInfo.OptionsTableLayoutItem.RowSpan = 7;
                lciUCHeinInfo.OptionsTableLayoutItem.ColumnSpan = 2;
                lciUCHeinInfo.EndInit();
                //layoutControl1.EndUpdate();
                //layoutControl1.ResumeLayout(false);

                layoutControl1.BeginUpdate();
                //layoutControl1.SuspendLayout();
                lciUCServiceRoomInfo.BeginInit();
                lciUCServiceRoomInfo.OptionsTableLayoutItem.RowIndex = 16;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnIndex = 0;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.RowSpan = 3;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnSpan = 2;
                lciUCServiceRoomInfo.EndInit();

                lciUCHeinInfo.BeginInit();
                lciUCHeinInfo.Control = this.ucHeinInfo1;
                lciUCHeinInfo.OptionsTableLayoutItem.RowIndex = 9;
                lciUCHeinInfo.OptionsTableLayoutItem.ColumnIndex = 0;
                lciUCHeinInfo.OptionsTableLayoutItem.RowSpan = 7;
                lciUCHeinInfo.OptionsTableLayoutItem.ColumnSpan = 2;
                lciUCHeinInfo.EndInit();

                layoutControl1.EndUpdate();
            }
            else if (patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__KSK && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__KSK > 0)
            {
                Inventec.Common.Logging.LogSystem.Debug("SuspendLayoutWithPatientTypeChanged.3");
                lciUCHeinInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                KskContractInput kskContractInput = new KskContractInput();
                kskContractInput.DeleOutFocus = DeleOutFocusKskContract;
                kskContractProcessor = new KskContractProcessor(TemplateType.ENUM.TEMPLATE_2);
                ucKskContract = (UserControl)kskContractProcessor.Run(kskContractInput);

                lciUCServiceRoomInfo.BeginInit();
                lciUCServiceRoomInfo.OptionsTableLayoutItem.RowIndex = 14;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnIndex = 0;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.RowSpan = 5;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnSpan = 2;
                lciUCServiceRoomInfo.EndInit();

                lciUCHeinInfo.BeginInit();
                Control ucHeinInfoTemp = lciUCHeinInfo.Control;
                lciUCHeinInfo.Control = ucKskContract;
                ucHeinInfoTemp.Parent = null;
                lciUCHeinInfo.OptionsTableLayoutItem.RowIndex = 9;
                lciUCHeinInfo.OptionsTableLayoutItem.ColumnIndex = 0;
                lciUCHeinInfo.OptionsTableLayoutItem.RowSpan = 5;
                lciUCHeinInfo.OptionsTableLayoutItem.ColumnSpan = 2;
                lciUCHeinInfo.EndInit();

                lciUCServiceRoomInfo.BeginInit();
                lciUCServiceRoomInfo.OptionsTableLayoutItem.RowIndex = 14;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnIndex = 0;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.RowSpan = 5;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnSpan = 2;
                lciUCServiceRoomInfo.EndInit();

                lciUCHeinInfo.BeginInit();
                lciUCHeinInfo.Control = ucKskContract;
                lciUCHeinInfo.OptionsTableLayoutItem.RowIndex = 9;
                lciUCHeinInfo.OptionsTableLayoutItem.ColumnIndex = 0;
                lciUCHeinInfo.OptionsTableLayoutItem.RowSpan = 5;
                lciUCHeinInfo.OptionsTableLayoutItem.ColumnSpan = 2;
                lciUCHeinInfo.EndInit();
            }
            else
            {
                Inventec.Common.Logging.LogSystem.Debug("SuspendLayoutWithPatientTypeChanged.4");
                lciUCHeinInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //lciUCHeinInfo.BeginInit();

                //lciUCHeinInfo.OptionsTableLayoutItem.RowIndex = 9;
                //lciUCHeinInfo.OptionsTableLayoutItem.ColumnIndex = 0;
                //lciUCHeinInfo.OptionsTableLayoutItem.RowSpan = 1;
                //lciUCHeinInfo.OptionsTableLayoutItem.ColumnSpan = 2;
                //lciUCHeinInfo.EndInit();

                lciUCServiceRoomInfo.BeginInit();
                lciUCServiceRoomInfo.OptionsTableLayoutItem.RowIndex = 9;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnIndex = 0;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.RowSpan = 10;
                lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnSpan = 2;
                lciUCServiceRoomInfo.EndInit();
            }

            layoutControl1.ResumeLayout(false);
            layoutControl1.EndUpdate();
            Inventec.Common.Logging.LogSystem.Debug("SuspendLayoutWithPatientTypeChanged.5");
        }

        private void FocusInKskContract()
        {
            try
            {
                if (this.ucKskContract != null && this.kskContractProcessor != null)
                {
                    this.kskContractProcessor.InFocus(this.ucKskContract);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DeleOutFocusKskContract()
        {
            try
            {
                this.ucServiceRoomInfo1.FocusUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ConfigLayout()
        {
            try
            {
                layoutControl1.BeginUpdate();
                for (int i = 0; i < layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions.Count; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions[i].SizeType = SizeType.Percent;
                    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions[i].Width = (100 / 6);
                }

                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 1");
                for (int i = 0; i < layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Count; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions[i].SizeType = SizeType.Percent;
                    layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions[i].Height = (100 / 14);
                }
                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 2");
                for (int i = layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions.Count; i < 6; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions.Add(
                        new ColumnDefinition()
                        {
                            SizeType = SizeType.Percent,
                            Width = (100 / 6)
                        });
                }
                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 3");
                for (int i = layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Count; i < 14; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Add(
                        new RowDefinition()
                        {
                            SizeType = SizeType.Percent,
                            Height = (100 / 14)
                        });
                }
                layoutControl1.EndUpdate();
                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long? GetPatientClassifyId()
        {
            try
            {
                return ucPatientRaw1.GetValue().PATIENT_CLASSIFY_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private long GetPatientTypeId()
        {
            try
            {
                return ucPatientRaw1.GetValue().PATIENTTYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return 0;
        }

        private void ChangeFindTypeInPatientRaw(bool isEnable)
        {
            try
            {
                this.ucOtherServiceReqInfo1.SetEnableChkExamOnline(isEnable);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool IsPatientTypeUsingHeinInfo()
        {
            return (ucPatientRaw1 != null && ucPatientRaw1.GetValue() != null && ucHeinInfo1 != null && ucPatientRaw1.GetValue().PATIENTTYPE_ID > 0 && (ucPatientRaw1.GetValue().PATIENTTYPE_ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT || ucPatientRaw1.GetValue().PATIENTTYPE_ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN));
        }

        private void FillDataAfterSearchPatientInUCPatientRaw(object data)
        {
            try
            {
                if (data != null)
                {
                    this.IsReadCardTheViet = false;
                    string heinCardNumber = "";
                    Inventec.Common.QrCodeBHYT.HeinCardData dataCheck = new Inventec.Common.QrCodeBHYT.HeinCardData();
                    DataResultADO dataResult = (DataResultADO)data;
                    this.SetValueVariableUCAddressCombo(dataResult);
                    if (dataResult.SearchTypePatient == 4)//Thẻ việt
                    {
                        IsReadCardTheViet = true;
                        HtProvinceCode = dataResult.HisPatientSDO != null ? dataResult.HisPatientSDO.HT_PROVINCE_CODE : null;
                        HtDistrictCode = dataResult.HisPatientSDO != null ? dataResult.HisPatientSDO.HT_DISTRICT_CODE : null;
                        HtCommuneCode = dataResult.HisPatientSDO != null ? dataResult.HisPatientSDO.HT_COMMUNE_CODE : null;
                        HtProvinceName = dataResult.HisPatientSDO != null ? dataResult.HisPatientSDO.HT_PROVINCE_NAME : null;
                        HtDistrictName = dataResult.HisPatientSDO != null ? dataResult.HisPatientSDO.HT_DISTRICT_NAME : null;
                        HtCommuneName = dataResult.HisPatientSDO != null ? dataResult.HisPatientSDO.HT_COMMUNE_NAME : null;
                    }
                    if (dataResult.OldPatient == false && dataResult.UCRelativeADO != null)
                        FillDataIntoUCRelativeInfo(dataResult.UCRelativeADO);
                    else if (dataResult.OldPatient == false && dataResult.HeinCardData != null)
                    {
                        this.FillDataAfterSaerchPatientInUCPatientRaw(dataResult);
                        FillDataIntoUCPlusInfo(dataResult.HisPatientSDO, dataResult.IsReadQr);
                        dataCheck = dataResult.HeinCardData;
                    }
                    else if (dataResult.HisPatientSDO != null)
                    {
                        this._isPatientAppointmentCode = (dataResult.SearchTypePatient == 2);
                        if (!String.IsNullOrEmpty(dataResult.AppointmentCode))
                            this.appointmentCode = dataResult.AppointmentCode;
                        this._TreatmnetIdByAppointmentCode = (dataResult.TreatmnetIdByAppointmentCode == null ? 0 : dataResult.TreatmnetIdByAppointmentCode);
                        this.currentPatientSDO = dataResult.HisPatientSDO;
                        FillDataIntoUCPlusInfo(currentPatientSDO, dataResult.IsReadQr);
                        FillDataIntoUCRelativeInfo(currentPatientSDO);
                        FillDataIntoUCAddressInfo(currentPatientSDO);
                        if (dataResult.SearchTypePatient == 4 && dataResult.OldPatient == false)
                            this.FillDataIntoUCHeinInfoByPatientTypeAlter(currentPatientSDO);
                        if (dataResult.SearchTypePatient == 5 && dataResult.OldPatient == false)
                            this.FillDataIntoUCHeinInfoByPatientTypeAlter(currentPatientSDO);//TODO
                        else if (!String.IsNullOrEmpty(currentPatientSDO.HeinCardNumber))
                            FillDataIntoUCHeinInfo(currentPatientSDO);
                        else if (IsPatientTypeUsingHeinInfo())
                            this.ucHeinInfo1.RefreshUserControl();
                        FillDataIntoUCImage(currentPatientSDO);
                        FillDataIntoUCOtherServiceReqInfo(currentPatientSDO);
                        //Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)//xuandv
                        this.FillDataToExamServiceReqByPatient(currentPatientSDO);

                        if (currentPatientSDO.IS_HAS_NOT_DAY_DOB == 1)
                            dataCheck.Dob = currentPatientSDO.DOB.ToString();
                        else
                            dataCheck.Dob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientSDO.DOB);
                        dataCheck.PatientName = currentPatientSDO.VIR_PATIENT_NAME;
                        dataCheck.HeinCardNumber = currentPatientSDO.HeinCardNumber;
                        dataCheck.Gender = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.HisToHein(currentPatientSDO.GENDER_ID.ToString());
                        dataCheck.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientSDO.HeinCardFromTime ?? 0);
                        dataCheck.MediOrgCode = currentPatientSDO.HeinMediOrgCode;
                        dataCheck.Address = currentPatientSDO.HeinAddress;
                        dataCheck.LiveAreaCode = currentPatientSDO.LiveAreaCode;
                        dataCheck.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientSDO.HeinCardToTime ?? 0);
                    }

                    UCPatientRawADO patientRawADO = this.ucPatientRaw1.GetValue();

                    long? treatmentTypeId = null;
                    if (this.ucPatientRaw1 != null && !String.IsNullOrEmpty(dataCheck.HeinCardNumber))
                    {
                        patientRawADO = this.ucPatientRaw1.GetValue();
                        if (patientRawADO.PATIENTTYPE_ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                        {
                            this.CheckTTProcessResultData(dataCheck, null, false);
                        }

                        treatmentTypeId = patientRawADO.TREATMENT_TYPE_ID;
                    }

                    heinCardNumber = dataCheck.HeinCardNumber;

                    this.SetPatientSearchPanel(dataResult.OldPatient);
                    this.cardSearch = dataResult.HisCardSDO;

                    this.FillDataCareerUnder6AgeByHeinCardNumber(heinCardNumber);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("FillDataAfterSearchPatientInUCPatientRaw(object data) treatmentTypeId ", treatmentTypeId));
                    if (HisConfigCFG.IsDefaultTreatmentTypeExam)
                    {
                        AutoSetTreatmentTypeCombo(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM, dataResult);
                    }
                    else if (treatmentTypeId.HasValue && treatmentTypeId.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    {
                        AutoSetTreatmentTypeCombo(treatmentTypeId, dataResult);
                    }
                }
                else
                {
                    btnNewContinue_Click(null, null);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_CAREER GetCareerByBhytWhiteListConfig(string heinCardNumder)
        {
            HIS_CAREER result = null;
            try
            {
                if (!String.IsNullOrEmpty(heinCardNumder))
                {
                    var bhytWhiteList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>().FirstOrDefault(o => !String.IsNullOrEmpty(heinCardNumder) && o.BHYT_WHITELIST_CODE.ToUpper() == heinCardNumder.Substring(0, 3).ToUpper());
                    if (bhytWhiteList != null && (bhytWhiteList.CAREER_ID ?? 0) > 0)
                    {
                        result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.ID == bhytWhiteList.CAREER_ID.Value);
                        if (result == null)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("GetCareerByBhytWhiteListConfig => Khong lay duoc nghe nghiep theo id = " + bhytWhiteList.CAREER_ID);
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

        private void FillDataCareerUnder6AgeByHeinCardNumber(string heinCardNumder)
        {
            try
            {
                //Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
                //27/10/2017 => sửa lại => Căn cứ vào đầu thẻ BHYT và dữ liệu cấu hình trong bảng HIS_BHYT_WHITELIST để tự động điền nghề nghiệp tương ứng
                var patientRawObj = this.ucPatientRaw1.GetValue();
                if (!String.IsNullOrEmpty(heinCardNumder))
                {
                    MOS.EFMODEL.DataModels.HIS_CAREER career = GetCareerByBhytWhiteListConfig(heinCardNumder);
                    if (career == null)
                    {
                        if (patientRawObj.DOB > 0)
                        {
                            DateTime dtDob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientRawObj.DOB).Value;
                            if (dtDob != DateTime.MinValue && MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtDob))
                            {
                                career = HisConfigCFG.CareerUnder6Age;
                            }
                            else if (DateTime.Now.Year - dtDob.Year <= 18)
                            {
                                career = HisConfigCFG.CareerHS;
                            }
                            else
                            {
                                career = HisConfigCFG.CareerBase;
                            }
                        }
                        else
                        {
                            career = HisConfigCFG.CareerBase;
                        }
                    }
                    if (career != null && career.ID > 0)
                    {
                        patientRawObj.CARRER_ID = career.ID;
                        patientRawObj.CARRER_CODE = career.CAREER_CODE;
                        patientRawObj.CARRER_NAME = career.CAREER_NAME;
                        Inventec.Common.Logging.LogSystem.Error("FillDataCareerUnder6AgeByHeinCardNumber");
                        this.ucPatientRaw1.SetValue(patientRawObj);
                    }
                }

                this.ucOtherServiceReqInfo1.AutoCheckPriorityByPriorityType(patientRawObj.DOB, heinCardNumder);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessWhileChangeDOb()
        {
            try
            {
                var heindata = this.ucHeinInfo1.GetValuePatientTypeAlter();

                var patientRaw = this.ucPatientRaw1.GetValue();
                if (heindata != null && patientRaw != null)
                    this.ucHeinInfo1.ShowCheckSS(DateTime.Now.Year - (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientRaw.DOB) ?? DateTime.MinValue).Year < 6);
                this.ucOtherServiceReqInfo1.AutoCheckPriorityByPriorityType(patientRaw.DOB, heindata.HisPatientTypeAlter.HEIN_CARD_NUMBER);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueVariableUCAddressCombo(DataResultADO data)
        {
            try
            {
                if (data.OldPatient == true)
                    this.ucAddressCombo1.isPatientBHYT = true;
                else
                    this.ucAddressCombo1.isPatientBHYT = false;
                if (data.HeinCardData != null)
                    this.ucAddressCombo1.isReadCard = true;
                else
                    this.ucAddressCombo1.isReadCard = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetPatientForm()
        {
            try
            {
                this.ResetPatientInfo();
                this.SetPatientSearchPanel(false);

                this.ucHeinInfo1.Controls.Clear();

                this.SetDefaultRegisterForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetPatientInfo()
        {
            try
            {
                if (!this.layoutControl2.IsInitialized) return;
                this.layoutControl2.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl2.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            if (lci.Name == "lciGateNumber" || lci.Name == "lciStepNumber" || lci.Name == "lcicboCashierRoom")
                            {
                                continue;
                            }
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    this.layoutControl2.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void NewContinue()
        {
            try
            {
                this.isResetForm = true;
                this.IsReadCardTheViet = false;
                this.isCheckSS = false;
                this.RefreshUserControl();
                var patientTypeDefault = HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.PatientTypeDefault;
                if (!(patientTypeDefault != null && patientTypeDefault.ID > 0) && !HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.UsingPatientTypeOfPreviousPatient)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Truong hop khong co cau hinh mac dinh doi tuong benh nhan => reset vung thong tin bhyt___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeDefault), patientTypeDefault));
                    this.SuspendLayoutWithPatientTypeChanged(0);
                }
                var pa = this.ucPatientRaw1.GetValue();
                long patientTypeId = pa != null ? pa.PATIENTTYPE_ID : 0;
                if (HisConfigCFG.AutoCheckPrintExam__PatientTypeIds != null && HisConfigCFG.AutoCheckPrintExam__PatientTypeIds.Count > 0 && HisConfigCFG.AutoCheckPrintExam__PatientTypeIds.Contains(patientTypeId))
                {
                    chkPrintExam.Checked = true;
                }
                else if (HisConfigCFG.AutoCheckPrintExam__PatientTypeIds != null && HisConfigCFG.AutoCheckPrintExam__PatientTypeIds.Count > 0)
                    chkPrintExam.Checked = false;
                else
                {
                    //InitControlState();
                }
                this.isResetForm = false;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region Event Control
        private void btnNewContinue_Click(object sender, EventArgs e)
        {
            try
            {
                LogTheadInSessionInfo(NewContinue, "btnNewContinue_Click");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                LogTheadInSessionInfo(SaveNotPrintAction, "btnSave_Click");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveNotPrintAction()
        {
            Save(false);
        }

        private void SaveAndPrintAction()
        {
            IsActionSavePrint = true;
            Save(true);
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                LogTheadInSessionInfo(SaveAndPrintAction, "btnSaveAndPrint_Click");
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
                IsActionSavePrint = false;
                LogTheadInSessionInfo(this.PrintLogSS, "btnPrint_Click");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void PrintLogSS()
        {
            try
            {
                this.Print();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepositDetail_Click(object sender, EventArgs e)
        {
            try
            {
                this.DepositDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepositRequest_Click(object sender, EventArgs e)
        {
            try
            {
                this.DepositRequestClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBill_Click(object sender, EventArgs e)
        {
            try
            {
                this.Bill();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTreatmentBedRoom_Click(object sender, EventArgs e)
        {
            try
            {
                TreatmentBedRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveAndAssain_Click(object sender, EventArgs e)
        {
            try
            {
                SaveAndAssain();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                this.CreateThreadCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRecallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                this.CreateThreadRecallCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPatientNew_Click(object sender, EventArgs e)
        {
            try
            {
                this.GetDataBeforeClickbtnPatientNew();
                this.RefreshUserControl();
                this.dataPatientRaw.PATIENT_CODE = "";
                this.dataPatientRaw.CARRER_ID = null;
                this.dataPatientRaw.PATIENT_CODE = "";
                this.dataPatientRaw.ReceptionForm = null;
                Inventec.Common.Logging.LogSystem.Error("btnPatientNew_Click");
                this.ucPatientRaw1.SetValue(dataPatientRaw);
                this.SetPatientSearchPanel(false);
                this.ucPatientRaw1.FocusToPatientType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTTChuyenTuyen_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnTTChuyenTuyen.Enabled)
                    return;
                if (this.ucHeinInfo1 != null)
                {
                    this.IsPresent = this.ucHeinInfo1.HeinRightRouteTypeIsPresent();
                    this.IsPresentAndAppointment = this.ucHeinInfo1.HeinRightRouteTypeIsPresentAndAppointment();
                }
                else
                {
                    this.IsPresent = false;
                    this.IsPresentAndAppointment = false;
                }
                if (HisConfigCFG.KeyValueObligatoryTranferMediOrg == 1 && this.IsPresent)
                    this.ShowFormThongTinChuyenTuyen(true);
                else if (HisConfigCFG.KeyValueObligatoryTranferMediOrg == 2 && (this.IsPresent || this.IsPresentAndAppointment))
                    this.ShowFormThongTinChuyenTuyen(true);
                else if (HisConfigCFG.KeyValueObligatoryTranferMediOrg == 3 && this.IsPresent)
                    this.ShowFormThongTinChuyenTuyen(true);
                else
                    this.ShowFormThongTinChuyenTuyen(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGiayTo_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("btnGiayTo_Click.1");
                if (!btnGiayTo.Enabled)
                    return;

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTreatmentFile").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTreatmentFile");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("btnGiayTo_Click.2");
                    List<object> listArgs = new List<object>();
                    listArgs.Add(GetTreatmentIdFromResultData());
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
                Inventec.Common.Logging.LogSystem.Debug("btnGiayTo_Click.3");
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("btnGiayTo_Click.4");
        }
        #endregion

        #region --- Event BTN Other -----
        private async Task InitPopupMenuOther()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageUCRegister = new ResourceManager("HIS.Desktop.Plugins.RegisterV2.Resources.Lang", typeof(HIS.Desktop.Plugins.RegisterV2.RunV3.UCRegister).Assembly);
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem("Chỉ định giải phẫu bệnh", new EventHandler(onClickAssignPan)));

                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("UCRegister.PopupMenuOther.btnDHST", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickDHST)));

                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("UCRegister.PopupMenuOther.btnTaiNanThuongTich", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickTaiNanThuongTich)));

                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("UCRegister.PopupMenuOther.RequestDeposit", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(btnRequestDepositClick)));

                menu.Items.Add(new DXMenuItem("Thông tin dịch tễ", new EventHandler(onClickThongTinDichTe)));


                menu.Items.Add(new DXMenuItem(ResourceMessage.Title_InTheBenhNhan, new EventHandler(PrintTheBenhNhan)));

                this.dropDownButton__Other.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickAssignPan(object sender, EventArgs e)
        {
            long _treatmentId = 0;
            if (this.resultHisPatientProfileSDO != null)
            {
                _treatmentId = this.resultHisPatientProfileSDO.HisTreatment.ID;
            }
            else if (this.currentHisExamServiceReqResultSDO != null)
            {
                _treatmentId = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID;
            }
            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
            if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(_treatmentId);
                listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                ((Form)extenceInstance).ShowDialog();
            }
        }

        private void PrintTheBenhNhan(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate("Mps000178", DelegateRunPrinterInTheBenhNhan);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickDHST(object sender, EventArgs e)
        {
            try
            {
                long _treatmentId = 0;
                if (this.resultHisPatientProfileSDO != null)
                {
                    _treatmentId = this.resultHisPatientProfileSDO.HisTreatment.ID;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    _treatmentId = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID;
                }

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisDhst").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisDhst");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_treatmentId);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickTaiNanThuongTich(object sender, EventArgs e)
        {
            try
            {
                long _treatmentId = 0;
                if (this.resultHisPatientProfileSDO != null)
                {
                    _treatmentId = this.resultHisPatientProfileSDO.HisTreatment.ID;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    _treatmentId = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID;
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AccidentHurt").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AccidentHurt");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_treatmentId);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickThongTinDichTe(object sender, EventArgs e)
        {
            try
            {
                long _treatmentId = 0;
                if (this.resultHisPatientProfileSDO != null)
                {
                    _treatmentId = this.resultHisPatientProfileSDO.HisTreatment.ID;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    _treatmentId = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID;
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.EpidemiologyInfo").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.EpidemiologyInfo");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_treatmentId);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRequestDepositClick(object sender, EventArgs e)
        {
            try
            {
                long _treatmentId = 0;
                if (this.resultHisPatientProfileSDO != null)
                {
                    _treatmentId = this.resultHisPatientProfileSDO.HisTreatment.ID;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    _treatmentId = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID;
                }
                if (_treatmentId > 0)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RequestDeposit").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RequestDeposit'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(_treatmentId);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dropDownButton__Other_Click(object sender, EventArgs e)
        {
            try
            {
                this.dropDownButton__Other.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void chkPrintPatientCard_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintPatientCard.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintPatientCard.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintPatientCard.Name;
                    csAddOrUpdate.VALUE = (chkPrintPatientCard.Checked ? "1" : "");
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

        private void chkAutoCreateBill_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAutoCreateBill.Checked)
                {
                    chkAutoDeposit.Checked = false;
                }

                ChangeCheckAutoPaid();
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoCreateBill.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoCreateBill.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoCreateBill.Name;
                    csAddOrUpdate.VALUE = (chkAutoCreateBill.Checked ? "1" : "");
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

        private void chkPrintExam_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintExam.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintExam.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintExam.Name;
                    csAddOrUpdate.VALUE = (chkPrintExam.Checked ? "1" : "");
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

        private void chkAssignDoctor_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAssignDoctor.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAssignDoctor.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAssignDoctor.Name;
                    csAddOrUpdate.VALUE = (chkAssignDoctor.Checked ? "1" : "");
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

        private void chkAutoDeposit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAutoDeposit.Checked)
                {
                    chkAutoCreateBill.Checked = false;
                }

                ChangeCheckAutoPaid();
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoDeposit.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoDeposit.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoDeposit.Name;
                    csAddOrUpdate.VALUE = (chkAutoDeposit.Checked ? "1" : "");
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

        private void SetDefaultRegisterForm()
        {
            try
            {
                LogSystem.Debug("SetDefaultRegisterForm. 1");
                this._HeinCardData = new Inventec.Common.QrCodeBHYT.HeinCardData();
                this.ResultDataADO = new ResultDataADO();
                this.actionType = GlobalVariables.ActionAdd;
                this.isReadQrCode = false;
                this.cardSearch = null;
                this.isNotPatientDayDob = false;
                this.baseNameControl = layoutControl2.Name + "." + layoutControlGroup2.Name + "." + layoutControlItem1.Name + "." + layoutControl1.Name + "." + layoutControlGroup1.Name + "." + pnlServiceRoomInfomation.Name + "." + ucPatientRaw1.Name;
                this.ucPatientRaw1.SetNameControl(this.baseNameControl);
                var ethnicDefault = HisConfigCFG.EthinicBase;
                MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = null;
                if (!String.IsNullOrEmpty(AppConfigs.PatientTypeCodeDefault))
                {
                    patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == AppConfigs.PatientTypeCodeDefault && o.IS_ACTIVE == 1 && o.IS_NOT_USE_FOR_PATIENT != 1);
                    if (patientType == null)
                    {
                        //Inventec.Common.Logging.LogSystem.Warn("Phan mem RAE da duoc cau hinh doi tuong benh nhan mac dinh, tuy nhien ma doi tuong cau hinh khong ton tai trong danh muc doi tuong benh nhan, he thong tu dong lay doi tuong mac dinh la doi tuong BHYT. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AppConfigs.PatientTypeCodeDefault), AppConfigs.PatientTypeCodeDefault));
                        //patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT);
                    }
                }
                //else
                //{
                //    patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT);
                //}

                LogSystem.Debug("SetDefaultRegisterForm.2");
                if (patientType != null)
                {
                    ucPatientRaw1.SetValuePatientType(patientType.ID);
                    //Lay danh sach doi tuong kham theo doi tuong benh nhan da chon, 
                    //sau do set gia tri mac dinh cho doi tuong kham
                    this.currentPatientTypeAllowByPatientType = this.LoadPatientTypeExamByPatientType(patientType.ID);


                    //Lấy danh sách id các đối tượng thanh toán được phép chuyển đổi từ đối tượng bệnh nhân
                    GlobalStore.PatientTypeIdAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>()
                        .Where(o => o.PATIENT_TYPE_CODE == patientType.PATIENT_TYPE_CODE)
                        .Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList();

                    if (patientType.ID > 0 && patientType.ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                    {
                        this.ucHeinInfo1.InitFieldFromAsync();
                    }
                    else
                    {
                        LogSystem.Debug("SetDefaultRegisterForm.3");
                        this.SuspendLayoutWithPatientTypeChanged(patientType.ID);
                    }



                    ////Goi ham xu ly load dong vung thong tin bhyt theo doi tuong benh nhan,ung
                    //// voi tung doi tuong se goi den thu vien His.UC.UCHein thuc hien load dong giao dien
                    //this.ChoiceTemplateHeinCard(patientType.PATIENT_TYPE_CODE, false);
                }
                else
                {
                    //this.ChoiceTemplateHeinCard("", false);

                    LogSystem.Debug("SetDefaultRegisterForm. 4");
                    this.SuspendLayoutWithPatientTypeChanged(0);

                    Inventec.Common.Logging.LogSystem.Debug("Khong lay duoc doi tuong benh nhan mac dinh");
                }

                InitExamServiceRoom();
                this.ucServiceRoomInfo1.RefreshUserControl();

                LogSystem.Debug("t5: end");
                this.EnableButton(this.actionType, false);

                this.SetValidationByChildrenUnder6Years(false, true);

                this._TreatmnetIdByAppointmentCode = 0;
                this.isAlertTreatmentEndInDay = false;
                LogSystem.Debug("SetDefaultRegisterForm. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoPaid_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoPay.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoPay.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoPay.Name;
                    csAddOrUpdate.VALUE = (chkAutoPay.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeCheckAutoPaid()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                if (chkAutoDeposit.Checked || chkAutoCreateBill.Checked)
                {
                    chkAutoPay.Enabled = true;

                    if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                    {
                        foreach (var item in this.currentControlStateRDO)
                        {
                            if (item.KEY == chkAutoPay.Name)
                            {
                                chkAutoPay.Checked = item.VALUE == "1";
                                break;
                            }
                        }
                    }
                }
                else
                {
                    chkAutoPay.Enabled = false;
                    chkAutoPay.Checked = false;
                }

                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStepNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGateNumber_Leave(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == txtGateNumber.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = txtGateNumber.Text;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = txtGateNumber.Name;
                    csAddOrUpdate.VALUE = txtGateNumber.Text;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStepNumber_Leave(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == txtStepNumber.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = txtStepNumber.Text;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = txtStepNumber.Name;
                    csAddOrUpdate.VALUE = txtStepNumber.Text;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void txtGateNumber_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    frmConfigCall frm = new frmConfigCall(ReloadConfigState);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadConfigState(bool obj)
        {
            try
            {
                if (obj)
                {
                    this.currentControlStateRDOModuleLink = controlStateWorker.GetData(moduleLinkConfigCall);
                    foreach (var item in this.currentControlStateRDOModuleLink)
                    {
                        if (item.KEY == moduleLinkConfigCall)
                        {
                            CallConfigString = item.VALUE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSignExam_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignExam.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignExam.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignExam.Name;
                    csAddOrUpdate.VALUE = (chkSignExam.Checked ? "1" : "");
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
    }
}
