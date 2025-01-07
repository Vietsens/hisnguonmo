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
using ACS.Filter;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraLayout;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Config;
using HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Resources;
using HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Validation;
using HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Validtion;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LocalStorage.Location;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqUpdateInstruction
{
    public partial class frmServiceReqUpdateInstruction : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module module;
        private long service_req_id;
        private V_HIS_SERVICE_REQ currentServiceReq = null;
        private HIS_TREATMENT currentTreatment = null;
        //private HIS_SERVICE_REQ currentServiceReq1 = null;
        private HIS.Desktop.Common.RefeshReference refeshData;
        private List<MOS.EFMODEL.DataModels.HIS_ICD> listIcd;
        internal List<HIS_USER_ROOM> _UserRoom { get; set; }
        private IcdProcessor icdProcessor;
        private IcdProcessor icdProcessorYHCT;
        private UserControl ucIcd;
        private UserControl ucIcdYHCT;
        private SecondaryIcdProcessor subIcdProcessor;
        private SecondaryIcdProcessor subIcdProcessorYHCT;
        private UserControl ucSecondaryIcd;
        private UserControl ucSecondaryIcdYHCT;
        private int positionHandleControl = -1;
        private string LoggingName = "";
        internal string CheckIcdWhenSave = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.CheckIcdWhenSave");

        internal HIS.UC.Icd.IcdProcessor IcdCauseProcessor { get; set; }
        internal UserControl ucIcdCause;
        List<V_HIS_SERVICE> services;

        public frmServiceReqUpdateInstruction(Inventec.Desktop.Common.Modules.Module _module, long _service_req_id, HIS.Desktop.Common.RefeshReference _refeshData)
            : base(_module)
        {
            InitializeComponent();
            this.module = _module;
            this.service_req_id = _service_req_id;
            this.refeshData = _refeshData;
            LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            SetCaptionByLanguageKey();
            listIcd = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
        }
        private bool isLoading = true;
        private void frmServiceReqUpdateInstruction_Load(object sender, EventArgs e)
        {
            WaitingManager.Show();
            dtTime.EditValueChanged -= dtTime_EditValueChanged;
            InitUcCauseIcd();
            GetData();
            GetTreatment();
            VisibleLayout();
            InitUcIcd();
            InitUcSecondaryIcd();
            InitUcIcdYhct();
            InitUcSecondaryIcdYhct();
            FillDataCommandToControl(this.currentServiceReq);
            LoadUser();
            ValidControlInform();
            SetIcon();
            dtTime.Focus();
            dtTime.SelectAll();
            InitEnabledControl();
            services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
            WaitingManager.Hide();
            dtTime.EditValueChanged += dtTime_EditValueChanged;
            isLoading = false;
        }

        private void VisibleLayout()
        {
            try
            {
                VisibleResultApprover();
                VisibleAppointmentTimeAndDes();
                VisibleAssignTimeTo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleAssignTimeTo()
        {
            try
            {
                if (currentServiceReq != null && currentServiceReq.SERVICE_REQ_TYPE_ID == 15 && currentServiceReq.REMEDY_COUNT != null)
                {
                    ControlDtAssignTimeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    ControlDtAssignTimeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleAppointmentTimeAndDes()
        {
            try
            {
                if (currentServiceReq != null && currentServiceReq.APPOINTMENT_TIME != null)
                {
                    lciAppointmentTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciEmptyAppointmentTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciAppointmentDes.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciAppointmentTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciEmptyAppointmentTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciAppointmentDes.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleResultApprover()
        {
            try
            {
                if (currentServiceReq != null && currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                {
                    lciResultApprover.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciComboApprover.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciResultApprover.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciComboApprover.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
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
                long autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.IsUCCause = false;
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.DelegateRequiredCause = LoadRequiredCause;
                ado.DelegateRefreshSubIcd = LoadSubIcd;
                ado.Width = 440;
                ado.Height = 24;
                ado.IsColor = true;
                ado.DataIcds = listIcd;
                ado.AutoCheckIcd = autoCheckIcd == 1 ? true : false;
                ado.hisTreatment = currentTreatment;
                ucIcd = (UserControl)icdProcessor.Run(ado);

                if (ucIcd != null)
                {
                    this.panelControlUcIcd.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
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
                long autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                icdProcessorYHCT = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.LblIcdMain = "CĐ YHCT:";
                ado.ToolTipsIcdMain = "Chẩn đoán y học cổ truyền";
                ado.Width = 440;
                ado.Height = 30;
                ado.IsYHCT = true;
                //ado.IsColor = true;
                ado.DataIcds = listIcd.Where(s => s.IS_TRADITIONAL == 1 && s.IS_ACTIVE == 1).ToList();
                ado.AutoCheckIcd = autoCheckIcd == 1 ? true : false;
                ado.hisTreatment = currentTreatment;
                ucIcdYHCT = (UserControl)icdProcessorYHCT.Run(ado);

                if (ucIcdYHCT != null)
                {
                    this.panelControlCDYHCT.Controls.Add(ucIcdYHCT);
                    ucIcdYHCT.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadSubIcd(string icdCodes, string icdNames)
        {
            try
            {
                SecondaryIcdDataADO data = new SecondaryIcdDataADO();
                data.ICD_SUB_CODE = icdCodes;
                data.ICD_TEXT = icdNames;
                if (this.subIcdProcessor != null && this.ucSecondaryIcd != null)
                {
                    this.subIcdProcessor.SetAttachIcd(this.ucSecondaryIcd, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                NextForcusSubIcdToDo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcCauseIcd()
        {
            try
            {
                long autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                this.IcdCauseProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.IsUCCause = true;
                ado.DelegateNextFocus = NextForcusSubIcdCause;
                ado.Width = 440;
                ado.LblIcdMain = "NN ngoài:";
                ado.ToolTipsIcdMain = "Nguyên nhân ngoài";
                ado.Height = 24;
                ado.IsColor = false;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_CAUSE == 1).OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = autoCheckIcd == 1 ? true : false;
                this.ucIcdCause = (UserControl)this.IcdCauseProcessor.Run(ado);

                if (this.ucIcdCause != null)
                {
                    this.panelControlCauseIcd.Controls.Add(this.ucIcdCause);
                    this.ucIcdCause.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadRequiredCause(bool isRequired)
        {
            try
            {
                if (this.IcdCauseProcessor != null && this.ucIcdCause != null)
                {
                    this.IcdCauseProcessor.SetRequired(this.ucIcdCause, isRequired);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcdCause()
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

        private void NextForcusSubIcdToDo()
        {
            try
            {
                if (IcdCauseProcessor != null && ucIcdCause != null)
                {
                    IcdCauseProcessor.FocusControl(ucIcdCause);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdCauseToControl(string icdCode, string icdName)
        {
            try
            {
                UC.Icd.ADO.IcdInputADO icd = new UC.Icd.ADO.IcdInputADO();
                icd.ICD_CODE = icdCode;
                icd.ICD_NAME = icdName;
                if (this.ucIcdCause != null)
                {
                    this.IcdCauseProcessor.Reload(this.ucIcdCause, icd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadUser()
        {
            try
            {

                List<ACS_USER> listResult = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                // Get listemployee
                List<HIS_EMPLOYEE> listHisEmployee;
                if (BackendDataWorker.IsExistsKey<HIS_EMPLOYEE>())
                {
                    listHisEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic dynamicFilter = new System.Dynamic.ExpandoObject();
                    listHisEmployee = await new BackendAdapter(paramCommon).GetAsync<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumers.MosConsumer, dynamicFilter, paramCommon);

                    if (listHisEmployee != null) BackendDataWorker.UpdateToRam(typeof(HIS_EMPLOYEE), listHisEmployee, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                var listLoginNameEmployee = listHisEmployee != null ? listHisEmployee.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(p => p.LOGINNAME).ToList() : null;

                if (listLoginNameEmployee != null && listLoginNameEmployee.Count > 0)
                {
                    listResult = listResult.Where(o => listLoginNameEmployee.Contains(o.LOGINNAME)).ToList();
                }

                this._UserRoom = new List<HIS_USER_ROOM>();
                MOS.Filter.HisUserRoomFilter filter = new HisUserRoomFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ROOM_ID = currentServiceReq.EXECUTE_ROOM_ID;

                Inventec.Common.Logging.LogSystem.Debug("currentServiceReq.EXECUTE_ROOM_ID: " + currentServiceReq.EXECUTE_ROOM_ID);
                this._UserRoom = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_ROOM>>("api/HisUserRoom/Get", ApiConsumers.MosConsumer, filter, null);
                List<string> listLoginNameHandler = _UserRoom.Select(o => o.LOGINNAME).ToList();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listLoginNameHandler), listLoginNameHandler));
                List<ACS_USER> listHandler = listResult.Where(o => listLoginNameHandler.Contains(o.LOGINNAME)).ToList();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listHandler), listHandler));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(cboEndServiceReq, listResult, controlEditorADO);
                ControlEditorLoader.Load(cboRequestUser, listResult, controlEditorADO);
                ControlEditorLoader.Load(cboResultApprover, listResult, controlEditorADO);
                ControlEditorLoader.Load(cboConsultant, listResult, controlEditorADO);
                ControlEditorLoader.Load(cboHandler, listHandler, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlInform()
        {
            ValidControlDtInstructionTime();
            ValidControlDtStartTime();
            ValidControlCboUser();
            if (currentServiceReq != null && currentServiceReq.APPOINTMENT_TIME != null)
            {
                ValidationSingleControl(dtAppointmentTime, dxValidationProvider1);
            }
        }

        private void ValidControlDtInstructionTime()
        {
            try
            {
                InstructionTimeValidationRule instructionTimeRule = new InstructionTimeValidationRule();
                instructionTimeRule.dtInstructionTime = this.dtTime;
                instructionTimeRule.dtStartTime = this.dtStartTime;
                dxValidationProvider1.SetValidationRule(this.dtTime, instructionTimeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDtStartTime()
        {
            try
            {
                StartTimeValidationRule startTimeRule = new StartTimeValidationRule();
                startTimeRule.dtStartTime = this.dtStartTime;
                startTimeRule.dtEndTime = this.dtEndTime;
                dxValidationProvider1.SetValidationRule(this.dtStartTime, startTimeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlCboUser()
        {
            try
            {
                RequestUserValidationRule rule = new RequestUserValidationRule();
                rule.cboUser = cboRequestUser;
                rule.txtLoginname = txtRequestUser;
                dxValidationProvider1.SetValidationRule(txtRequestUser, rule);
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
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), listIcd);

                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.DelegateGetIcdMain = DelegateCheckICDMain;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.TextNullValue = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.cboSub.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ado.hisTreatment = currentTreatment;
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
        private void InitUcSecondaryIcdYhct()
        {
            try
            {
                subIcdProcessorYHCT = new SecondaryIcdProcessor(new CommonParam(), listIcd.Where(s => s.IS_TRADITIONAL == 1).ToList());
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                //ado.DelegateGetIcdMain = DelegateCheckICDSub;
                ado.Width = 440;
                ado.Height = 30;
                ado.TextLblIcd = "CĐ YHCT Phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán y học cổ truyền phụ";
                ado.TextNullValue = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.cboSub.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ado.hisTreatment = currentTreatment;
                ucSecondaryIcdYHCT = (UserControl)subIcdProcessorYHCT.Run(ado);

                if (ucSecondaryIcdYHCT != null)
                {
                    this.panelControlICDSubYHCT.Controls.Add(ucSecondaryIcdYHCT);
                    ucSecondaryIcdYHCT.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string DelegateCheckICDSub()
        {
            string result = "";
            try
            {
                var rs = icdProcessorYHCT.GetValue(ucIcdYHCT);
                if (ucIcdYHCT != null && rs is IcdInputADO)
                {
                    result = ((IcdInputADO)rs).ICD_CODE;

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string DelegateCheckICDMain()
        {
            string result = "";
            try
            {
                var rs = icdProcessor.GetValue(ucIcd);
                if (ucIcd != null && rs is IcdInputADO)
                {
                    result = ((IcdInputADO)rs).ICD_CODE;

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void SetDefaultValue()
        {
            try
            {
                cboConsultant.EditValue = LoggingName;
                txtConsultant.Text = LoggingName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPayFormCombo(string _payFormCode)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> listResult = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                listResult = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => (o.LOGINNAME != null && o.LOGINNAME.Equals(_payFormCode))).ToList();

                if (listResult.Count == 1)
                {
                    cboEndServiceReq.EditValue = listResult[0].LOGINNAME;
                    txtLoginname.Text = listResult[0].LOGINNAME;
                    txtRequestUser.Focus();
                    txtRequestUser.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboEndServiceReq.EditValue = null;
                    cboEndServiceReq.Focus();
                    cboEndServiceReq.ShowPopup();
                }
                else
                {
                    cboEndServiceReq.EditValue = null;
                    cboEndServiceReq.Focus();
                    cboEndServiceReq.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPayFormComboRequest(string _payFormCode)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> listResult = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                listResult = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => (o.LOGINNAME != null && o.LOGINNAME.Equals(_payFormCode))).ToList();

                if (listResult.Count == 1)
                {
                    cboRequestUser.EditValue = listResult[0].LOGINNAME;
                    txtRequestUser.Text = listResult[0].LOGINNAME;
                    this.icdProcessor.FocusControl(this.ucIcd);
                }
                else if (listResult.Count > 1)
                {
                    cboRequestUser.EditValue = null;
                    cboRequestUser.Focus();
                    cboRequestUser.ShowPopup();
                }
                else
                {
                    cboRequestUser.EditValue = null;
                    cboRequestUser.Focus();
                    cboRequestUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusOut()
        {
            try
            {
                //btnSave.Focus();
                mmNOTE.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceReqUpdateInstruction.frmServiceReqUpdateInstruction).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciThoiGianYLenh.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStartTime.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.lciStartTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEndTime.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.lciEndTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciUserName.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.lciUserName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetData()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = service_req_id;
                currentServiceReq = new BackendAdapter(param)
                    .Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataCommandToControl(V_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                if (currentServiceReq != null)
                {
                    LoadDobPatientToForm(serviceReq);
                    IcdInputADO icd = new IcdInputADO();
                    icd.ICD_CODE = serviceReq.ICD_CODE;
                    icd.ICD_NAME = serviceReq.ICD_NAME;

                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }

                    SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = serviceReq.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = serviceReq.ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }
                    //yhct
                    IcdInputADO icdYHCT = new IcdInputADO();
                    icdYHCT.ICD_CODE = serviceReq.TRADITIONAL_ICD_CODE;
                    icdYHCT.ICD_NAME = serviceReq.TRADITIONAL_ICD_NAME;

                    if (ucIcdYHCT != null)
                    {
                        icdProcessorYHCT.Reload(ucIcdYHCT, icdYHCT);
                    }

                    SecondaryIcdDataADO subIcdYHCT = new SecondaryIcdDataADO();
                    subIcdYHCT.ICD_SUB_CODE = serviceReq.TRADITIONAL_ICD_SUB_CODE;
                    subIcdYHCT.ICD_TEXT = serviceReq.TRADITIONAL_ICD_TEXT;
                    if (ucSecondaryIcdYHCT != null)
                    {
                        subIcdProcessorYHCT.Reload(ucSecondaryIcdYHCT, subIcdYHCT);
                    }
                    //
                    txtLoginname.Text = serviceReq.EXECUTE_LOGINNAME;
                    cboEndServiceReq.EditValue = serviceReq.EXECUTE_LOGINNAME;

                    txtRequestUser.Text = serviceReq.REQUEST_LOGINNAME;
                    cboRequestUser.EditValue = serviceReq.REQUEST_LOGINNAME;
                    mmNOTE.Text = serviceReq.NOTE;

                    if (serviceReq.CONSULTANT_LOGINNAME == null)
                    {
                        SetDefaultValue();
                    }
                    else
                    {
                        txtConsultant.Text = serviceReq.CONSULTANT_LOGINNAME;
                        cboConsultant.EditValue = serviceReq.CONSULTANT_LOGINNAME;
                    }

                    if (serviceReq.ASSIGNED_EXECUTE_LOGINNAME != null)
                    {

                        txtHandler.Text = serviceReq.ASSIGNED_EXECUTE_LOGINNAME;
                        cboHandler.EditValue = serviceReq.ASSIGNED_EXECUTE_LOGINNAME;
                    }
                    if (currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                    {
                        txtResultApproverLoginname.Text = currentServiceReq.RESULT_APPROVER_LOGINNAME ?? "";
                        cboResultApprover.EditValue = currentServiceReq.RESULT_APPROVER_LOGINNAME;
                    }

                    if (serviceReq.START_TIME != null)
                    {
                        dtStartTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.START_TIME ?? 0) ?? DateTime.MinValue;
                    }
                    if (serviceReq.FINISH_TIME != null)
                    {
                        dtEndTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.FINISH_TIME ?? 0) ?? DateTime.MinValue;
                    }

                    if (!string.IsNullOrEmpty(serviceReq.ICD_CAUSE_CODE))
                    {
                        var dataIcd = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(p => p.ICD_CODE == serviceReq.ICD_CAUSE_CODE.Trim());
                        if (dataIcd != null)
                        {
                            LoadIcdCauseToControl(dataIcd.ICD_CODE, serviceReq.ICD_CAUSE_NAME);
                        }
                    }
                    if (currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                    {
                        mmNOTE.Text = currentServiceReq.NOTE;
                        layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                    else
                    {
                        layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.Size = new Size(this.Size.Width, this.Size.Height - layoutControlItem16.Size.Height - 20);
                    }
                    ///
                    if (currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                        || currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA
                        || currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                        || currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                        || currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                        || currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL)
                    {
                        chkIsNotComplete.Checked = (serviceReq.IS_NOT_REQUIRED_COMPLETE == 1 ? true : false);
                        layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                    else
                    {
                        layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    chkPriority.Checked = (serviceReq.PRIORITY == 1 ? true : false);
                    chkIsEmergency.Checked = (serviceReq.IS_EMERGENCY == 1 ? true : false);
                    chkIsNotRequireFee.Checked = (serviceReq.IS_NOT_REQUIRE_FEE == 1 ? true : false);
                    chkIsNotUseBHYT.Checked = (serviceReq.IS_NOT_USE_BHYT == 1 ? true : false);

                    //mmNOTE.Enabled = (serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN ? true : false);
                    if (serviceReq.USE_TIME != null)
                    {
                        dtUseTime.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.USE_TIME ?? 0);
                    }
                    if (serviceReq.APPOINTMENT_TIME != null)
                    {
                        dtAppointmentTime.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.APPOINTMENT_TIME ?? 0);
                    }
                    if (serviceReq.APPOINTMENT_DESC != null)
                    {
                        txtAppointmentDes.Text = serviceReq.APPOINTMENT_DESC;
                    }
                    if (serviceReq.ASSIGN_TIME_TO != null && serviceReq.ASSIGN_TIME_TO > 0)
                    {
                        dtAssignTimeTo.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.ASSIGN_TIME_TO ?? 0);
                    }
                    else
                    {
                        dtAssignTimeTo.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDobPatientToForm(V_HIS_SERVICE_REQ serviceReqDTO)
        {
            try
            {
                if (serviceReqDTO != null)
                {
                    string nthnm = serviceReqDTO.INTRUCTION_TIME.ToString();

                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReqDTO.INTRUCTION_TIME) ?? DateTime.MinValue;
                    dtTime.DateTime = dtNgSinh;
                    int age = Inventec.Common.TypeConvert.Parse.ToInt32(nthnm.Substring(8, 2));
                    bool isGKS = true;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateData()
        {
            try
            {
                var icdValue = icdProcessor.GetValue(ucIcd);
                if (icdValue is IcdInputADO)
                {
                    currentServiceReq.ICD_CODE = ((IcdInputADO)icdValue).ICD_CODE;
                    //currentServiceReq.ICD_ID = ((IcdInputADO)icdValue).ICD_ID;
                    currentServiceReq.ICD_NAME = ((IcdInputADO)icdValue).ICD_NAME;
                }
                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        currentServiceReq.ICD_SUB_CODE = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        currentServiceReq.ICD_TEXT = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }

                //yhct
                if (ucIcdYHCT != null)
                {
                    var subIcd = icdProcessorYHCT.GetValue(ucIcdYHCT);
                    if (subIcd != null && subIcd is IcdInputADO)
                    {
                        currentServiceReq.TRADITIONAL_ICD_CODE = ((IcdInputADO)subIcd).ICD_CODE;
                        currentServiceReq.TRADITIONAL_ICD_NAME = ((IcdInputADO)subIcd).ICD_NAME;
                    }
                }
                if (ucSecondaryIcdYHCT != null)
                {
                    var subIcd = subIcdProcessorYHCT.GetValue(ucSecondaryIcdYHCT);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        currentServiceReq.TRADITIONAL_ICD_SUB_CODE = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        currentServiceReq.TRADITIONAL_ICD_TEXT = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                        var icd = BackendDataWorker.Get<HIS_ICD>()
                       .Where(s => s.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && s.IS_TRADITIONAL == 1).ToList();

                        if (!string.IsNullOrEmpty(currentServiceReq.TRADITIONAL_ICD_SUB_CODE))
                        {
                            foreach (var item in currentServiceReq.TRADITIONAL_ICD_SUB_CODE.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList())
                            {
                                if (!icd.Exists(o => o.ICD_CODE == item))
                                {
                                    MessageBox.Show("Chẩn đoán YHCT phụ không có trong danh mục");
                                    throw new InvalidOperationException("Chẩn đoán YHCT phụ không có trong danh mục"); // Ném ngoại lệ khi có lỗi
                                }
                            }
                        }
                    }
                }
                if (this.ucIcdCause != null)
                {
                    var icdCauseValue = this.IcdCauseProcessor.GetValue(this.ucIcdCause);
                    if (icdCauseValue != null && icdCauseValue is UC.Icd.ADO.IcdInputADO)
                    {
                        currentServiceReq.ICD_CAUSE_CODE = ((UC.Icd.ADO.IcdInputADO)icdCauseValue).ICD_CODE;
                        currentServiceReq.ICD_CAUSE_NAME = ((UC.Icd.ADO.IcdInputADO)icdCauseValue).ICD_NAME;
                    }
                }

                currentServiceReq.INTRUCTION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime) ?? 0;
                if (dtStartTime.EditValue != null && dtStartTime.DateTime != DateTime.MinValue)
                    currentServiceReq.START_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtStartTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                else
                    currentServiceReq.START_TIME = null;

                if (dtEndTime.EditValue != null && dtEndTime.DateTime != DateTime.MinValue)
                    currentServiceReq.FINISH_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtEndTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                else
                    currentServiceReq.FINISH_TIME = null;

                if (cboEndServiceReq.EditValue != null)
                    currentServiceReq.EXECUTE_LOGINNAME = cboEndServiceReq.EditValue.ToString();
                else
                    currentServiceReq.EXECUTE_LOGINNAME = null;

                if (cboRequestUser.EditValue != null)
                    currentServiceReq.REQUEST_LOGINNAME = cboRequestUser.EditValue.ToString();
                else
                    currentServiceReq.REQUEST_LOGINNAME = null;
                if (cboRequestUser.Properties.DataSource != null && cboRequestUser.Properties.DataSource is List<ACS.EFMODEL.DataModels.ACS_USER>)
                {
                    currentServiceReq.REQUEST_USERNAME = ((cboRequestUser.Properties.DataSource as List<ACS.EFMODEL.DataModels.ACS_USER>).FirstOrDefault(o => o.LOGINNAME == currentServiceReq.REQUEST_LOGINNAME) ?? new ACS.EFMODEL.DataModels.ACS_USER()).USERNAME;
                }

                if (cboConsultant.EditValue != null)
                    currentServiceReq.CONSULTANT_LOGINNAME = cboConsultant.EditValue.ToString();
                else
                    currentServiceReq.CONSULTANT_LOGINNAME = null;
                if (cboConsultant.Properties.DataSource != null && cboConsultant.Properties.DataSource is List<ACS.EFMODEL.DataModels.ACS_USER>)
                {
                    currentServiceReq.CONSULTANT_USERNAME = ((cboConsultant.Properties.DataSource as List<ACS.EFMODEL.DataModels.ACS_USER>).FirstOrDefault(o => o.LOGINNAME == currentServiceReq.CONSULTANT_LOGINNAME) ?? new ACS.EFMODEL.DataModels.ACS_USER()).USERNAME;
                }

                if (cboEndServiceReq.Properties.DataSource != null && cboEndServiceReq.Properties.DataSource is List<ACS.EFMODEL.DataModels.ACS_USER>)
                {
                    currentServiceReq.EXECUTE_USERNAME = ((cboEndServiceReq.Properties.DataSource as List<ACS.EFMODEL.DataModels.ACS_USER>).FirstOrDefault(o => o.LOGINNAME == currentServiceReq.EXECUTE_LOGINNAME) ?? new ACS.EFMODEL.DataModels.ACS_USER()).USERNAME;
                }

                if (currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && cboResultApprover.EditValue != null)
                {
                    currentServiceReq.RESULT_APPROVER_LOGINNAME = cboResultApprover.EditValue.ToString();
                    ACS_USER u = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == currentServiceReq.RESULT_APPROVER_LOGINNAME);
                    if (u != null)
                    {
                        currentServiceReq.RESULT_APPROVER_USERNAME = u.USERNAME;
                    }
                    else
                    {
                        currentServiceReq.RESULT_APPROVER_USERNAME = null;
                    }
                }
                if (cboHandler.EditValue != null)
                    currentServiceReq.ASSIGNED_EXECUTE_LOGINNAME = cboHandler.EditValue.ToString();
                else
                    currentServiceReq.ASSIGNED_EXECUTE_LOGINNAME = null;
                if (cboHandler.Properties.DataSource != null && cboHandler.Properties.DataSource is List<ACS.EFMODEL.DataModels.ACS_USER>)
                {
                    currentServiceReq.ASSIGNED_EXECUTE_USERNAME = ((cboHandler.Properties.DataSource as List<ACS.EFMODEL.DataModels.ACS_USER>).FirstOrDefault(o => o.LOGINNAME == currentServiceReq.ASSIGNED_EXECUTE_LOGINNAME) ?? new ACS.EFMODEL.DataModels.ACS_USER()).USERNAME;
                }
                else
                {
                    currentServiceReq.RESULT_APPROVER_LOGINNAME = null;
                    currentServiceReq.RESULT_APPROVER_USERNAME = null;
                }
                if (cboRequestUser.Properties.DataSource != null && cboRequestUser.Properties.DataSource is List<ACS.EFMODEL.DataModels.ACS_USER>)
                {
                    currentServiceReq.REQUEST_USERNAME = ((cboRequestUser.Properties.DataSource as List<ACS.EFMODEL.DataModels.ACS_USER>).FirstOrDefault(o => o.LOGINNAME == currentServiceReq.REQUEST_LOGINNAME) ?? new ACS.EFMODEL.DataModels.ACS_USER()).USERNAME;
                }
                if (mmNOTE.EditValue != null)
                {
                    currentServiceReq.NOTE = mmNOTE.EditValue.ToString();
                }
                currentServiceReq.NOTE = mmNOTE.Text;
                currentServiceReq.PRIORITY = (long)(chkPriority.Checked ? 1 : 0);
                currentServiceReq.IS_EMERGENCY = (short)(chkIsEmergency.Checked ? 1 : 0);
                currentServiceReq.IS_NOT_REQUIRE_FEE = (short)(chkIsNotRequireFee.Checked ? 1 : 0);
                currentServiceReq.IS_NOT_USE_BHYT = (short)(chkIsNotUseBHYT.Checked ? 1 : 0);
                currentServiceReq.IS_NOT_REQUIRED_COMPLETE = (short)(chkIsNotComplete.Checked ? 1 : 0);
                currentServiceReq.USE_TIME = dtUseTime.EditValue != null && dtUseTime.DateTime != DateTime.MinValue ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtUseTime.DateTime) : null;
                if (dtAssignTimeTo.DateTime != null && dtAssignTimeTo.DateTime != DateTime.MinValue && dtAssignTimeTo.DateTime != DateTime.MaxValue)
                {
                    currentServiceReq.ASSIGN_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAssignTimeTo.DateTime);
                }
                else
                {
                    currentServiceReq.ASSIGN_TIME_TO = null;
                }
                //currentServiceReq.ASSIGN_TIME_TO = dtAssignTimeTo.EditValue != null && dtAssignTimeTo.DateTime != DateTime.MinValue ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAssignTimeTo.DateTime) : null;
                if (lciAppointmentTime.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && lciAppointmentDes.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    currentServiceReq.APPOINTMENT_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAppointmentTime.DateTime);
                    currentServiceReq.APPOINTMENT_DESC = txtAppointmentDes.Text;
                }


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
                if (checkTime())
                {
                    XtraMessageBox.Show("Thời gian bắt đầu, thời gian kết thúc không được nhỏ hơn thời gian y lệnh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (chkIsNotUseBHYT.Checked)
                {
                    if ((XtraMessageBox.Show("Bạn có chắc không cho bệnh nhân hưởng bhyt các chi phí phát sinh tại phòng khám ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No))
                        return;
                }
                if (!CheckUseTime())
                {
                    XtraMessageBox.Show("Thời gian dự trù không được nhỏ hơn thời gian y lệnh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtUseTime.Focus();
                    return;
                }

                string codeCheckCD = "";
                string nameCheckCD = "";
                string codeCheckCDYHCT = "";

                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        codeCheckCD = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        nameCheckCD = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }
                if (ucIcd != null)
                {
                    var icdValue = icdProcessor.GetValue(ucIcd);
                    if (icdValue is IcdInputADO)
                    {
                        codeCheckCD += ((IcdInputADO)icdValue).ICD_CODE;
                        nameCheckCD += ((IcdInputADO)icdValue).ICD_NAME;
                    }
                }
                if (ucSecondaryIcdYHCT != null)
                {
                    var subIcdYHCT = subIcdProcessorYHCT.GetValue(ucSecondaryIcdYHCT);
                    if (subIcdYHCT != null && subIcdYHCT is SecondaryIcdDataADO)
                    {
                        codeCheckCDYHCT = ((SecondaryIcdDataADO)subIcdYHCT).ICD_SUB_CODE;
                    }
                }
                if (ucIcdYHCT != null)
                {
                    var IcdYHCT = icdProcessorYHCT.GetValue(ucIcdYHCT);
                    if (IcdYHCT != null && IcdYHCT is IcdInputADO)
                    {
                        codeCheckCDYHCT += ((IcdInputADO)IcdYHCT).ICD_CODE;
                    }
                }

                if (Inventec.Common.String.CountVi.Count(codeCheckCD) > 100)
                {
                    XtraMessageBox.Show("Mã chẩn đoán phụ nhập quá 100 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Inventec.Common.String.CountVi.Count(nameCheckCD) > 1500)
                {
                    XtraMessageBox.Show("Tên chẩn đoán phụ nhập quá 1500 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Inventec.Common.String.CountVi.Count(codeCheckCDYHCT) > 255)
                {
                    XtraMessageBox.Show("Mã chẩn đoán YHCT phụ nhập quá 255 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.positionHandleControl = -1;
                bool vali = true;
                CommonParam param = new CommonParam();
                bool succes = false;
                vali = (IsValiICDCause() || (panelControlUcIcd.Enabled == false));

                vali = vali && ((bool)subIcdProcessor.GetValidate(ucSecondaryIcd) || (panelControlUcIcd.Enabled == false));
                vali = vali && ((bool)icdProcessor.ValidationIcd(ucIcd) || (panelControlUcIcd.Enabled == false));
                vali = vali && ((bool)icdProcessorYHCT.ValidationIcd(ucIcdYHCT) || (panelControlCDYHCT.Enabled == false));
                vali = vali && (bool)subIcdProcessorYHCT.GetValidate(ucSecondaryIcdYHCT) || (panelControlICDSubYHCT.Enabled == false);
                if (!vali || !dxValidationProvider1.Validate())
                    return;
                vali = vali & CheckIntructionTimeWithInTime() & CheckMinDuration() & CheckHIS_DEPARTMENT_TRAN();
                vali = vali && CheckStartTimeMainExam();
                if (vali)
                {
                    WaitingManager.Show();
                    UpdateData();

                    if (currentServiceReq != null && currentServiceReq.ASSIGN_TIME_TO != null && currentServiceReq.INTRUCTION_TIME > (currentServiceReq.ASSIGN_TIME_TO ?? 0))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Chỉ định đến không được nhỏ hơn thời gian y lệnh"),
                           "Thông báo",
                          MessageBoxButtons.OK) == DialogResult.OK)
                            return;
                    }
                    HIS.Desktop.Plugins.Library.CheckIcd.CheckIcdManager check = new Desktop.Plugins.Library.CheckIcd.CheckIcdManager(null, currentTreatment);
                    string message = null;
                    if (CheckIcdWhenSave == "1" || CheckIcdWhenSave == "2")
                    {
                        if (!check.ProcessCheckIcd(currentServiceReq.ICD_CODE, currentServiceReq.ICD_SUB_CODE, ref message, true))
                        {
                            if (CheckIcdWhenSave == "1" && !String.IsNullOrEmpty(message))
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("{0}.Bạn có muốn tiếp tục không?", message),
                                "Thông báo",
                               MessageBoxButtons.YesNo) == DialogResult.No)
                                    return;
                            }
                            if (CheckIcdWhenSave == "2" && !String.IsNullOrEmpty(message))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                                return;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
                    {
                        if (this.currentServiceReq != null && this.currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                        {
                            HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                            filter.PARENT_ID = this.currentServiceReq.ID;
                            V_HIS_SERVICE_REQ vServiceReq = new BackendAdapter(param)
                                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                            if (vServiceReq != null && this.currentServiceReq.START_TIME.HasValue && Inventec.Common.DateTime.Calculation.DifferenceTime(this.currentServiceReq.START_TIME.Value, vServiceReq.INTRUCTION_TIME, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.SECOND) < Int32.Parse(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Thời gian chỉ định {0} trong y lệnh {1} phải cách thời gian bắt đầu khám {2} là {3} giây mới được phép chỉ định", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vServiceReq.INTRUCTION_TIME), vServiceReq.SERVICE_REQ_CODE, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentServiceReq.START_TIME.Value), HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                return;
                            }
                        }
                        if (this.currentServiceReq != null && this.currentServiceReq.PARENT_ID.HasValue)
                        {
                            HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                            filter.ID = this.currentServiceReq.PARENT_ID;
                            filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                            V_HIS_SERVICE_REQ vServiceReq1 = new BackendAdapter(param)
                                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                            if (vServiceReq1 != null && vServiceReq1.START_TIME.HasValue && Inventec.Common.DateTime.Calculation.DifferenceTime(vServiceReq1.START_TIME.Value, this.currentServiceReq.INTRUCTION_TIME, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.SECOND) < Int32.Parse(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Thời gian chỉ định {0} trong y lệnh {1} phải cách thời gian bắt đầu khám {2} là {3} giây mới được phép chỉ định", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentServiceReq.INTRUCTION_TIME), this.currentServiceReq.SERVICE_REQ_CODE, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vServiceReq1.START_TIME.Value), HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                return;
                            }
                        }
                    }
                    var update = new HIS_SERVICE_REQ();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(update, this.currentServiceReq);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => update), update));
                    var serviceReqUpdate = new BackendAdapter(param)
                        .Post<HIS_SERVICE_REQ>("api/HisServiceReq/UpdateCommonInfo", ApiConsumers.MosConsumer, update, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqUpdate), serviceReqUpdate));

                    if (serviceReqUpdate != null)
                    {
                        succes = true;
                        if (refeshData != null)
                            refeshData();
                        this.Close();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, succes);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckUseTime()
        {
            bool rs = true;
            try
            {
                if (dtUseTime.EditValue != null && dtUseTime.DateTime != DateTime.MinValue && dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue)
                {
                    if (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtUseTime.DateTime) < Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime))
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        bool checkTime()
        {
            bool success = false;
            try
            {
                if (Config.HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "1")
                {
                    if ((dtStartTime.EditValue != null && dtTime.EditValue != null && dtStartTime.DateTime < dtTime.DateTime) || (dtTime.EditValue != null && dtEndTime.EditValue != null && dtEndTime.DateTime < dtTime.DateTime))
                    {
                        return true;
                    }
                }

                if (Config.HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "2")
                {
                    List<long> ReqTypeId = new List<long>()
                    {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                    };
                    if (!ReqTypeId.Contains(currentServiceReq.SERVICE_REQ_TYPE_ID)
                        && ((dtStartTime.EditValue != null && dtTime.EditValue != null && dtStartTime.DateTime < dtTime.DateTime)
                        || (dtTime.EditValue != null && dtEndTime.EditValue != null && dtEndTime.DateTime < dtTime.DateTime)))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
        bool IsValiICDCause()
        {
            bool result = true;
            try
            {
                result = (bool)this.IcdCauseProcessor.ValidationIcd(this.ucIcdCause);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckIntructionTimeWithInTime()
        {
            bool result = true;
            try
            {
                if (currentTreatment != null && dtTime.EditValue != null)
                {
                    long timeInstruction = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime) ?? 0;
                    if (currentTreatment.IN_TIME > timeInstruction)
                    {
                        MessageManager.Show(String.Format(ResourceMessage.KhongChoNhapThoiGianNhoHonThoiGianVaoVien));
                        result = false;
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
        private bool CheckStartTimeMainExam()
        {
            bool result = true;
            try
            {
                if (currentServiceReq != null && currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && dtStartTime.EditValue != null)
                {
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == currentServiceReq.TDL_PATIENT_TYPE_ID);
                    if (patientType != null && patientType.PATIENT_TYPE_CODE == Config.HisConfigCFG.MOS_PATIENT_TYPE_CODE__BHYT)
                    {
                        long startTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStartTime.DateTime) ?? 0;
                        CommonParam param = new CommonParam();
                        HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                        filter.TREATMENT_ID = currentServiceReq.TREATMENT_ID;
                        filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                        filter.SERVICE_REQ_STT_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT };
                        var serviceReqs = new BackendAdapter(param)
                            .Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        if (serviceReqs != null)
                        {
                            if (currentServiceReq.IS_MAIN_EXAM == 1)
                            {
                                var serviceReqErrs = serviceReqs.Where(o => o.ID != currentServiceReq.ID && o.START_TIME < startTime).ToList();
                                if (serviceReqErrs != null && serviceReqErrs.Count > 0)
                                {
                                    MessageManager.Show(String.Format("Thời gian bắt đầu của khám chính {0} không được lớn hơn thời gian bắt đầu của khám phụ {1} có mã y lệnh là {2}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(startTime), String.Join(", ", serviceReqErrs.Select(o => Inventec.Common.DateTime.Convert.TimeNumberToTimeString(o.START_TIME ?? 0))), String.Join(", ", serviceReqErrs.Select(o => o.SERVICE_REQ_CODE))));
                                    result = false;
                                    dtStartTime.Focus();
                                }

                            }
                            else
                            {
                                var serviceReqMainExam = serviceReqs.FirstOrDefault(o => o.IS_MAIN_EXAM == 1);
                                if (serviceReqMainExam != null && serviceReqMainExam.START_TIME > startTime)
                                {
                                    MessageManager.Show(String.Format("Thời gian bắt đầu của khám phụ {0} không được nhỏ hơn thời gian bắt đầu của khám chính {1} có mã y lệnh là {2}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(startTime), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(serviceReqMainExam.START_TIME ?? 0), serviceReqMainExam.SERVICE_REQ_CODE));
                                    result = false;
                                    dtStartTime.Focus();
                                }
                            }
                        }
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

        private void GetTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = this.currentServiceReq.TREATMENT_ID;
                currentTreatment = new BackendAdapter(param)
                    .Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckMinDuration()
        {
            bool result = true;
            try
            {
                CommonParam param = new CommonParam();

                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.SERVICE_REQ_ID = currentServiceReq.ID;
                var servServMinDuration = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);

                if (servServMinDuration != null && servServMinDuration.Count > 0)
                {
                    var servicesDt = services.Where(o => servServMinDuration.Select(p => p.SERVICE_ID).Contains(o.ID) && o.MIN_DURATION.HasValue).ToList();

                    List<ServiceDuration> serviceDurations = new List<ServiceDuration>();

                    if (servicesDt != null && servicesDt.Count > 0)
                    {
                        foreach (var item in servicesDt)
                        {
                            ServiceDuration sd = new ServiceDuration();
                            sd.MinDuration = item.MIN_DURATION.Value;
                            sd.ServiceId = item.ID;
                            serviceDurations.Add(sd);
                        }
                    }

                    if (serviceDurations != null && serviceDurations.Count > 0)
                    {
                        HisSereServMinDurationFilter filter = new HisSereServMinDurationFilter();
                        filter.PatientId = currentServiceReq.TDL_PATIENT_ID;
                        filter.InstructionTime = currentServiceReq.INTRUCTION_TIME;
                        filter.ServiceDurations = serviceDurations;
                        filter.ServiceReqId = currentServiceReq.ID;
                        var sereServMinduration = new BackendAdapter(param)
                            .Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (sereServMinduration != null && sereServMinduration.Count > 0)
                        {
                            List<string> listMessage = new List<string>();

                            foreach (var item in sereServMinduration)
                            {
                                string message = "";
                                message += item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + ";";
                                listMessage.Add(message);
                            }

                            listMessage = listMessage.Distinct().ToList();

                            string serviceMessage = "";
                            foreach (var it in listMessage)
                            {
                                serviceMessage += it;
                            }

                            string messageStr = string.Format("Các dịch vụ sau có thời gian chỉ định nằm trong khoảng thời gian không cho phép: {0} Bạn có muốn tiếp tục?", serviceMessage);
                            if (MessageBox.Show(messageStr, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                result = false;
                            }
                        }
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

        private bool CheckHIS_DEPARTMENT_TRAN()
        {
            bool result = true;
            try
            {
                if (this.currentServiceReq != null)
                {
                    long checkINTRUCTION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime) ?? 0;

                    CommonParam param = new CommonParam();
                    HisDepartmentTranFilter filter = new HisDepartmentTranFilter();
                    filter.DEPARTMENT_ID = this.currentServiceReq.REQUEST_DEPARTMENT_ID;
                    filter.TREATMENT_ID = this.currentServiceReq.TREATMENT_ID;
                    var data = new BackendAdapter(param)
                            .Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (data != null && data.Count == 1)
                    {
                        var thisDepartmentTran = data.First();

                        HisDepartmentTranFilter filterDt = new HisDepartmentTranFilter();
                        filterDt.PREVIOUS_ID = thisDepartmentTran.ID;
                        filterDt.TREATMENT_ID = this.currentServiceReq.TREATMENT_ID;
                        var checkIfHasNextDepartmentTran = new BackendAdapter(param)
                                .Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filterDt, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        if (checkIfHasNextDepartmentTran != null)
                        {
                            var nextDepartmentTran = checkIfHasNextDepartmentTran.FirstOrDefault();
                            if (nextDepartmentTran == null || nextDepartmentTran.DEPARTMENT_IN_TIME == null)
                            {
                                result = true;
                            }
                            else if (nextDepartmentTran.DEPARTMENT_IN_TIME <= checkINTRUCTION_TIME)
                            {
                                var thisDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == thisDepartmentTran.DEPARTMENT_ID).FirstOrDefault();
                                var nextDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == nextDepartmentTran.DEPARTMENT_ID).FirstOrDefault();
                                string messageStr = string.Format("Y lệnh {0} {1} có thời gian y lệnh lớn hơn thời gian vào {2}! Bạn có muốn tiếp tục?", this.currentServiceReq.SERVICE_REQ_CODE, thisDepartment.DEPARTMENT_NAME, nextDepartment.DEPARTMENT_NAME);
                                if (MessageBox.Show(messageStr, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                {
                                    result = false;
                                }
                                else
                                {
                                    result = true;
                                }
                            }
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (data != null && data.Count > 1)
                    {
                        var checkIfHasThisDepartmentTran = data.OrderByDescending(o => o.DEPARTMENT_IN_TIME)
                                                    .Where(o => o.DEPARTMENT_IN_TIME <= this.currentServiceReq.INTRUCTION_TIME);
                        if (checkIfHasThisDepartmentTran == null || checkIfHasThisDepartmentTran.Count() == 0)
                        {
                            return true;
                        }
                        var thisDepartmentTran = checkIfHasThisDepartmentTran.First();

                        HisDepartmentTranFilter filterDt = new HisDepartmentTranFilter();
                        filterDt.PREVIOUS_ID = thisDepartmentTran.ID;
                        filterDt.TREATMENT_ID = this.currentServiceReq.TREATMENT_ID;
                        var checkIfHasNextDepartmentTran = new BackendAdapter(param)
                                .Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filterDt, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (checkIfHasNextDepartmentTran != null)
                        {
                            var nextDepartmentTran = data.Where(o => o.PREVIOUS_ID == thisDepartmentTran.ID).FirstOrDefault();
                            if (nextDepartmentTran == null || nextDepartmentTran.DEPARTMENT_IN_TIME == null)
                            {
                                result = true;
                            }
                            else if (nextDepartmentTran.DEPARTMENT_IN_TIME <= checkINTRUCTION_TIME)
                            {
                                var thisDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == thisDepartmentTran.DEPARTMENT_ID).FirstOrDefault();
                                var nextDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == nextDepartmentTran.DEPARTMENT_ID).FirstOrDefault();
                                string messageStr = string.Format("Y lệnh {0} {1} có thời gian y lệnh lớn hơn thời gian vào {2}! Bạn có muốn tiếp tục?", this.currentServiceReq.SERVICE_REQ_CODE, thisDepartment.DEPARTMENT_NAME, nextDepartment.DEPARTMENT_NAME);
                                if (MessageBox.Show(messageStr, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                {
                                    result = false;
                                }
                                else
                                {
                                    result = true;
                                }
                            }
                        }
                        else
                        {
                            result = true;
                        }
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

        private void dtTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtStartTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void cboSub_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadPayFormCombo(txtLoginname.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndServiceReq_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEndServiceReq.EditValue != null && cboEndServiceReq.EditValue != cboEndServiceReq.OldEditValue)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboEndServiceReq.EditValue.ToString());
                        if (commune != null)
                        {
                            txtLoginname.Text = commune.LOGINNAME;
                            txtRequestUser.Focus();
                            txtRequestUser.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndServiceReq_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboEndServiceReq.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboEndServiceReq.EditValue.ToString());
                        if (commune != null)
                        {
                            txtLoginname.Text = commune.LOGINNAME;
                            txtRequestUser.Focus();
                            txtRequestUser.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtStartTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtEndTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtEndTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLoginname.Focus();
                    txtLoginname.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndServiceReq_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboEndServiceReq.EditValue = null;
                    txtLoginname.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtStartTime_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    dtStartTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboRequestUser.EditValue = null;
                    txtRequestUser.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRequestUser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadPayFormComboRequest(txtRequestUser.Text);
                    CheckTimeSereServ();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRequestUser_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRequestUser.EditValue != null && cboRequestUser.EditValue != cboRequestUser.OldEditValue)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboRequestUser.EditValue.ToString());
                        if (commune != null)
                        {
                            txtRequestUser.Text = commune.LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }

                    }
                }
                CheckTimeSereServ();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRequestUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboRequestUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboRequestUser.EditValue.ToString());
                        if (commune != null)
                        {
                            txtRequestUser.Text = commune.LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtResultApproverLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboResultApprover.EditValue = null;
                        this.cboResultApprover.Focus();
                        this.cboResultApprover.ShowPopup();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboResultApprover.EditValue = searchResult[0].LOGINNAME;
                            this.txtResultApproverLoginname.Text = searchResult[0].LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                        else
                        {
                            this.cboResultApprover.EditValue = null;
                            this.cboResultApprover.Focus();
                            this.cboResultApprover.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultApprover_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboResultApprover.EditValue != null)
                    {
                        ACS_USER data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboResultApprover.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtResultApproverLoginname.Text = data.LOGINNAME;
                        }
                    }
                    this.icdProcessor.FocusControl(this.ucIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultApprover_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboResultApprover.EditValue != null)
                    {
                        ACS_USER data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboResultApprover.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtResultApproverLoginname.Text = data.LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
                else
                {
                    this.cboResultApprover.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultApprover_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboResultApprover.EditValue = null;
                    txtResultApproverLoginname.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboConsultant_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboConsultant.EditValue != null && cboConsultant.EditValue != cboConsultant.OldEditValue)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboConsultant.EditValue.ToString());
                        if (commune != null)
                        {
                            txtConsultant.Text = commune.LOGINNAME;
                            cboConsultant.Properties.Buttons[1].Visible = true;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConsultant_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboConsultant.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboConsultant.EditValue.ToString());
                        if (commune != null)
                        {
                            txtConsultant.Text = commune.LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
                else
                {
                    cboConsultant.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConsultant_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadFormComboConsultant(txtConsultant.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadFormComboConsultant(string _code)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> listResult = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                listResult = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (o.LOGINNAME != null && o.LOGINNAME.Equals(_code))).ToList();

                // Get list
                var listLoginNameEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(p => p.LOGINNAME).ToList();

                if (listLoginNameEmployee != null && listLoginNameEmployee.Count > 0)
                {
                    listResult = listResult.Where(o => listLoginNameEmployee.Contains(o.LOGINNAME)).ToList();
                }

                if (listResult.Count == 1)
                {
                    cboConsultant.EditValue = listResult[0].LOGINNAME;
                    txtConsultant.Text = listResult[0].LOGINNAME;
                    this.icdProcessor.FocusControl(this.ucIcd);
                }
                else if (listResult.Count > 1)
                {
                    cboConsultant.EditValue = null;
                    cboConsultant.Focus();
                    cboConsultant.ShowPopup();
                }
                else
                {
                    cboConsultant.EditValue = null;
                    cboConsultant.Focus();
                    cboConsultant.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConsultant_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboConsultant.EditValue = null;
                    txtConsultant.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHandler_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHandler.EditValue != null && cboHandler.EditValue != cboHandler.OldEditValue)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboHandler.EditValue.ToString());
                        if (commune != null)
                        {
                            txtHandler.Text = commune.LOGINNAME;
                            cboHandler.Properties.Buttons[1].Visible = true;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHandler_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHandler.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboHandler.EditValue.ToString());
                        if (commune != null)
                        {
                            txtHandler.Text = commune.LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
                else
                {
                    cboHandler.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHandler_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadFormComboHandler(txtHandler.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadFormComboHandler(string _code)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> listResult = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                listResult = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => (o.LOGINNAME != null && o.LOGINNAME.Equals(_code))).ToList();

                if (listResult.Count == 1)
                {
                    cboHandler.EditValue = listResult[0].LOGINNAME;
                    txtHandler.Text = listResult[0].LOGINNAME;
                    this.icdProcessor.FocusControl(this.ucIcd);
                }
                else if (listResult.Count > 1)
                {
                    cboHandler.EditValue = null;
                    cboHandler.Focus();
                    cboHandler.ShowPopup();
                }
                else
                {
                    cboHandler.EditValue = null;
                    cboHandler.Focus();
                    cboHandler.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHandler_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboHandler.EditValue = null;
                    txtHandler.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isLoading)
                    return;
                CheckTimeSereServ();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void CheckTimeSereServ()
        {

            try
            {

                if (dtTime.EditValue == null && cboRequestUser.EditValue == null || cboRequestUser.EditValue == null) return;
                Inventec.Common.Logging.LogSystem.Debug("Check Sere Serv Time____Start");
                var config = BackendDataWorker.Get<HIS_CONFIG>().Where(s => s.KEY == "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION").FirstOrDefault();
                CommonParam param = new CommonParam();
                if (config != null)
                {
                    if (config.VALUE == "1" || config.VALUE == "2")
                    {
                        HisServiceReqCheckSereTimesSDO sdo = new HisServiceReqCheckSereTimesSDO();
                        sdo.TreatmentId = currentTreatment.ID;
                        var username = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && cboRequestUser.EditValue.ToString() == p.LOGINNAME).FirstOrDefault();
                        if (username != null) sdo.Loginnames = new List<string>() { username.LOGINNAME };
                        long sereTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime) ?? 0;
                        sdo.SereTimes = new List<long> { sereTime };
                        Inventec.Common.Logging.LogSystem.Debug("HisServiceReqCheckSereTimesSDO:" + LogUtil.TraceData("HisServiceReqCheckSereTimesSDO", sdo));
                        bool rs = new BackendAdapter(param).Post<bool>("/api/HisServiceReq/CheckSereTimes", ApiConsumers.MosConsumer, sdo, param);
                        if (!rs)
                        {
                            if (config.VALUE == "1")
                            {
                                MessageManager.Show(this, param, rs);
                                btnSave.Enabled = false;
                            }
                            else
                            {
                                btnSave.Enabled = MessageBox.Show(this, param.GetMessage() + "Bạn có muốn tiếp tục?", "Thông Báo", MessageBoxButtons.YesNo) == DialogResult.Yes;

                            }
                        }
                        else btnSave.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
