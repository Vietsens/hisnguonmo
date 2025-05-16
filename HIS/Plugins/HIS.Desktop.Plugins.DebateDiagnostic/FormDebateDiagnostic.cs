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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.DebateDiagnostic.ADO;
using HIS.Desktop.Plugins.DebateDiagnostic.Config;
using HIS.Desktop.Plugins.DebateDiagnostic.UcDebateDetail;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.DebateDiagnostic
{
    public partial class FormDebateDiagnostic : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int action = 0;
        long treatment_id = 0;
     
        System.Globalization.CultureInfo cultureLang;
        List<ADO.HisDebateUserADO> lstParticipantDebate;
        internal MOS.EFMODEL.DataModels.HIS_DEBATE currentHisDebate { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_TREATMENT vHisTreatment { get; set; }
        MOS.EFMODEL.DataModels.HIS_DEBATE hisDebate;
        MOS.SDO.WorkPlaceSDO WorkPlaceSDO;
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ examServiceReq;
        internal MOS.EFMODEL.DataModels.HIS_SERVICE hisService;
        List<MOS.EFMODEL.DataModels.HIS_DEBATE> medicinePrints;
        HIS_DEBATE_TEMP datacombox;
        List<ACS_USER> acsUsers = new List<ACS_USER>();
        List<HIS_DEPARTMENT> listDepartment = new List<HIS_DEPARTMENT>();
        MOS.EFMODEL.DataModels.HIS_TRACKING tracking { get; set; }
        List<TrackingADO> trackingADOs { get; set; }
        bool isInitTracking;
        internal int actionType = 0;
        long treatmentId = 0;
        int positionHandleControl = -1;
        internal List<long> intructionTimeSelecteds = new List<long>();
        internal List<DateTime?> intructionTimeSelected = new List<DateTime?>();
        DateTime timeSelested;
        internal long InstructionTime { get; set; }
        public List<HIS_ICD> Icds { get; private set; }
        public List<V_HIS_EMPLOYEE> Employee { get; private set; }

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO__Create;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO__Update;
        DetailProcessor detailProcessor;

        List<HIS_EXECUTE_ROLE> ListExecuteRole;
        bool isNotLoadWhileChangeControlStateInFirst; 
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.DebateDiagnostic";

        List<AcsUserADO> lstReAcsUserADO;
        List<HIS_DEBATE> lstDebate;
        List<HisDebateADO> lstHisDebateADO;
        List<HisDebateInvateUserADO> lstDebateInvateUserADO;

        UserControl ucDetail;
        List<InvateADO> lstInvateADO;
        bool IsNotLoadFirst = false;
        #endregion

        #region Construct
        public FormDebateDiagnostic(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();

            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                WorkPlaceSDO = new MOS.SDO.WorkPlaceSDO();
                this.moduleData = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormDebateDiagnostic(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ exam, List<MOS.EFMODEL.DataModels.HIS_DEBATE> medicinePrints, Inventec.Desktop.Common.Modules.Module module)
            : this(module)
        {
            try
            {
                this.Text = module.text;
                this.treatment_id = exam.TREATMENT_ID;
                this.examServiceReq = exam;
                this.medicinePrints = medicinePrints;
                this.action = GlobalVariables.ActionAdd;
                this.moduleData = module;
                btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormDebateDiagnostic(MOS.EFMODEL.DataModels.HIS_SERVICE _hisService, List<MOS.EFMODEL.DataModels.HIS_DEBATE> medicinePrints, Inventec.Desktop.Common.Modules.Module module, long treatmentId)
            : this(module)
        {
            try
            {
                this.Text = module.text;
                this.treatment_id = treatmentId;
                this.hisService = _hisService;
                this.medicinePrints = medicinePrints;
                this.action = GlobalVariables.ActionAdd;
                this.moduleData = module;
                btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormDebateDiagnostic(HIS.Desktop.ADO.TreatmentLogADO _treatmentAdo, Inventec.Desktop.Common.Modules.Module module, List<MOS.EFMODEL.DataModels.HIS_DEBATE> medicinePrints)
            : this(module)
        {
            try
            {
                this.Text = module.text;
                this.treatment_id = _treatmentAdo.TreatmentId;
                this.medicinePrints = medicinePrints;
                this.action = GlobalVariables.ActionAdd;
                this.moduleData = module;
                btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormDebateDiagnostic(MOS.EFMODEL.DataModels.HIS_DEBATE debate, Inventec.Desktop.Common.Modules.Module module, List<MOS.EFMODEL.DataModels.HIS_DEBATE> medicinePrints)
            : this(module)
        {
            try
            {
                this.Text = module.text;
                this.currentHisDebate = debate;
                this.treatment_id = debate.TREATMENT_ID;
                hisDebate = debate;
                this.medicinePrints = medicinePrints;
                this.action = GlobalVariables.ActionEdit;
                this.moduleData = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void LoadAcsUser()
        {
            try
            {
                acsUsers = BackendDataWorker.Get<ACS_USER>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadEmployee()
        {
            try
            {
                Employee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDepartment()
        {
            try
            {
                listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadExecuteRole()
        {
            try
            {
                ListExecuteRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadIcd()
        {
            try
            {
                Icds = Base.GlobalStore.HisIcds;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FormDebateDiagnostic_Load(object sender, EventArgs e)
        {
            try
            {
                IsNotLoadFirst = true;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("currentHisDebate___:", currentHisDebate));
                WaitingManager.Show();
                SetIcon();
                HisConfigCFG.LoadConfig();
                lciAutoSign.Visibility = HisConfigCFG.IsUseSignEmr ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciAutoCreateEmr.Visibility = HisConfigCFG.IsUseSignEmr ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                WorkPlaceSDO = WorkPlace.WorkPlaceSDO.Where(o => o.RoomId == moduleData.RoomId && o.RoomTypeId == moduleData.RoomTypeId).FirstOrDefault();
                List<Action> methods = new List<Action>();
                methods.Add(LoadAcsUser);
                methods.Add(LoadEmployee);
                methods.Add(LoadDepartment);
                methods.Add(LoadExecuteRole);
                methods.Add(LoadIcd);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                InitComboDebateTemp();

                InitControlState();
                LoadKeysFromlanguage();
                SetDefaultValueControl();
                FillDataToParticipants();//Load danh sach nguoi dung
                FillDataToInvateUser();
                DataToComboChuanDoan();
                LoadDataToGridParticipants();
                LoadDataCombocboRequestLoggin();
                LoadDataCombocboDebateType();
                InitComboDebateReason();
                LoadDataComboDepartment();
                ProcessLoadExecuteRole();

                if (this.currentHisDebate != null && this.currentHisDebate.CONTENT_TYPE.HasValue)
                {
                    if (this.currentHisDebate.CONTENT_TYPE.Value == (long)DetailEnum.Khac)
                    {
                        ChkOther.Checked = true;
                    }
                    else if (this.currentHisDebate.CONTENT_TYPE.Value == (long)DetailEnum.Pttt)
                    {
                        ChkPttt.Checked = true;
                    }
                    else
                    {
                        CheckThuoc.Checked = true;
                    }
                }
                else
                {
                    //Mo tu chuc nang ke don
                    if (this.medicinePrints != null)
                    {
                        CheckThuoc.Checked = true;
                    }
                    else
                    {
                        ChkOther.Checked = true;
                    }
                }
                LoadComboThamGia();

                LoadData(this.treatment_id);// Load thong tin BN
                LoadDataTracking();
                if (HisConfigCFG.DebateDiagnostic_IsDefaultTracking == true)
                {

                }

                if ((this.action == GlobalVariables.ActionEdit || this.action == GlobalVariables.ActionView) && this.currentHisDebate != null)
                {
                    LoadDataDebateDiagnostic(this.currentHisDebate);//Load du lieu san co
                    LoadDataInviteDebate(this.currentHisDebate);
                    if (detailProcessor != null)
                    {
                        detailProcessor.SetData(GetTypeDetail(), currentHisDebate);
                    }
                }
                else if (this.examServiceReq != null && this.examServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                {
                    LoadDataDebateDiagnostic(examServiceReq);
                    if (detailProcessor != null)
                    {
                        detailProcessor.SetData(GetTypeDetail(), examServiceReq);
                    }
                }

                time.Tick += Time_Tick;
                time.Interval = 100;

                LoadDataComboPhieuChuanDoanCu();

                if (hisService != null)
                {
                    ChkOther.Checked = true;
                }
                time.Start();
                WaitingManager.Hide();
                IsNotLoadFirst = false;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            ValidationControl();
            FillDataToButtonPrint();//nút in

            DisableControlItem();

            EnableColumnAddDelete();
        }

        System.Windows.Forms.Timer time = new System.Windows.Forms.Timer();

        private async Task InitComboDebateReason()
        {
            try
            {
                List<ColumnInfo> columnInfor = new List<ColumnInfo>();
                columnInfor.Add(new ColumnInfo("DEBATE_REASON_CODE", "", 100, 1));
                columnInfor.Add(new ColumnInfo("DEBATE_REASON_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEBATE_REASON_NAME", "ID", columnInfor, false, 350);
                ControlEditorLoader.Load(this.cboDebateReason, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEBATE_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), controlEditorADO);
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
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO__Create = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK_DEBATE);
                this.currentControlStateRDO__Update = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK_DEBATE_DIAGNOSTIC);
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO__Create)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD)
                        {
                            chkAutoSign.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == ControlStateConstant.CHECK_AUTO_CREATE_EMR)
                        {
                            chkAutoCreateEmr.Checked = item.VALUE == "1";
                        }
                    }
                }
                if (currentControlStateRDO__Update != null && currentControlStateRDO__Update.Count > 0 && !chkAutoSign.Checked)
                {
                    foreach (var item in this.currentControlStateRDO__Update)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD)
                        {
                            chkAutoSign.Checked = item.VALUE == "1";
                        }
                    }
                }
                
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkLockInfor.Name)
                        {
                            chkLockInfor.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
       
        private void ValidationControl()
        {
            //ValidationControlMaxLength(txtMedicineName, 500, false);
            //ValidationControlMaxLength(txtUseForm, 100, false);
            //ValidationControlMaxLength(txtConcena, 1000, false);
            //ValidationControlMaxLength(txtUserManual, 2000, false);
            //ValidationControlMaxLength(txtRequestContent, 1000, false);
            ValidationControlMaxLength(icdMainText, 500, false);
            ValidationControlMaxLength(txtIcdTextName, 4000, false);
            //ValidationControlMaxLength(txtPathologicalHistory, 2000, false);
            //ValidationControlMaxLength(txtHospitalizationState, 2000, false);
            //ValidationControlMaxLength(txtBeforeDiagnostic, 2000, false);
            //ValidationControlMaxLength(txtTreatmentTracking, 2000, false);
            //ValidationControlMaxLength(txtDiscussion, 2000, false);
            //ValidationControlMaxLength(txtDiagnostic, 2000, false);
            //ValidationControlMaxLength(txtTreatmentMethod, 2000, false);
            //ValidationControlMaxLength(txtCareMethod, 2000, false);
            //ValidationControlMaxLength(txtConclusion, 2000, false);

            ValidControlTime();
        }

        private void ValidControlTime()
        {
            try
            {
                TimeValidationRule Time = new TimeValidationRule();
                Time.DateEdit1 = this.dtInTime;
                Time.DateEdit2 = this.dtOutTime;
                dxValidationProvider1.SetValidationRule(this.dtOutTime, Time);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequired;
                validate.ErrorText = "Nhập quá kí tự cho phép [" + maxLength + "]";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private async Task LoadDataComboPhieuChuanDoanCu()
        {
            try
            {
                lstHisDebateADO = new List<HisDebateADO>();

                Action myaction = () =>
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisDebateFilter filter = new HisDebateFilter();
                    filter.TREATMENT_ID = treatment_id;
                    lstDebate = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE>>("api/HisDebate/Get", ApiConsumers.MosConsumer, filter, param);

                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                if (lstDebate != null)
                {

                    lstDebate = lstDebate.OrderByDescending(o => o.DEBATE_TIME).ToList();
                    foreach (var item in lstDebate)
                    {
                        HisDebateADO ADO = new HisDebateADO(item);
                        lstHisDebateADO.Add(ADO);
                    }
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("REQUEST_LOGINNAME", "Người tạo", 100, 1));
                    columnInfos.Add(new ColumnInfo("HisDebateTimeString", "Thời gian", 200, 2));
                    columnInfos.Add(new ColumnInfo("LOCATION", "Khoa tạo", 150, 3));
                    columnInfos.Add(new ColumnInfo("ContentTypeName", "Loại hội chẩn", 150, 4));

                    ControlEditorADO controlEditorADO = new ControlEditorADO("ContentTypeName", "ID", columnInfos, true, 400);

                    ControlEditorLoader.Load(cboBienBanHoiChanCu, lstHisDebateADO, controlEditorADO);
                    cboBienBanHoiChanCu.Properties.ImmediatePopup = true;
                    cboBienBanHoiChanCu.Properties.Buttons[1].Visible = false;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private async Task LoadDataComboDepartment()
        {
            try
            {
                CommonParam param = new CommonParam();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboDepartment, listDepartment, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private async Task LoadDataCombocboRequestLoggin()
        {
            List<ColumnInfo> columnInfos2 = new List<ColumnInfo>();
            columnInfos2.Add(new ColumnInfo("LOGINNAME", "LOGINNAME", 50, 1));
            columnInfos2.Add(new ColumnInfo("USERNAME", "USERNAME", 150, 2));
            ControlEditorADO controlEditorADO2 = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos2, true, 200);
            ControlEditorLoader.Load(cboRequestLoggin, acsUsers, controlEditorADO2);
        }

        private List<HIS_DEBATE_TYPE> GetDebateTypeDb()
        {
            List<HIS_DEBATE_TYPE> result = null;
            try
            {
                result = BackendDataWorker.Get<HIS_DEBATE_TYPE>();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private async Task LoadDataCombocboDebateType()
        {
            List<HIS_DEBATE_TYPE> Data = null;
            Action myaction = () =>
            {
                Data = BackendDataWorker.Get<HIS_DEBATE_TYPE>();
            };
            Task task = new Task(myaction);
            task.Start();

            await task;
            List<ColumnInfo> columnInfos2 = new List<ColumnInfo>();
            columnInfos2.Add(new ColumnInfo("DEBATE_TYPE_CODE", "Mã", 50, 1));
            columnInfos2.Add(new ColumnInfo("DEBATE_TYPE_NAME", "Tên", 150, 2));
            ControlEditorADO controlEditorADO2 = new ControlEditorADO("DEBATE_TYPE_NAME", "ID", columnInfos2, true, 200);
            ControlEditorLoader.Load(cboDebateType, Data, controlEditorADO2);
        }

        #endregion

        #region Private method
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__BTN_PRINT",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                this.btnSaveTemp.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__BTN_SAVE_TEMP",
                   Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                   cultureLang);
                //this.groupBoxParticipants.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__GRB_PARTICIPANTS",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                //this.groupBoxResultBody.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__GRB_RESULT_BODY",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                //this.groupBoxTrackings.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__GRB_TRACKINGS",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                //this.Gc_Add.Caption = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__GC_ADD",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                this.Gc_LoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__GC_LOGIN_NAME",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                this.Gc_President.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__GC_PRESIDENT",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                this.Gc_Secretary.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__GC_SECRETARY",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                this.Gc_Titles.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__GC_TITLES",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                this.Gc_UserName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__GC_USER_NAME",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                //this.lciBeforeDiagnostic.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_BEFORE_DIAGNOSTIC",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                //this.lciCareMethod.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_CARE_METHOD",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                this.lciCheckEdit.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_CHECK_EDIT",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                //this.lciConclusion.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_CONCLUSION",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                this.lciRequestContent.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_REQUEST_CONTENT",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                this.lciDebateTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_DEBATE_TIME",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                //this.lciDiagnostic.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_DIAGNOSTIC",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                //this.lciHospitalizationState.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_HOSPITALIZATION_STATE",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                this.lciIcdMain.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_ICD_MAIN",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                this.lciIcdSubCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_ICD_TEXT",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                this.lciIcdSubCode1.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_ICD_TEXT",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                //this.lciPathologicalHistory.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_PATHOLOGICAL_HISTORY",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                //this.lciTreatmentMethod.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_TREATMENT_METHOD",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                //this.lciTreatmentTracking.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__LCI_TREATMENT_TRACKING",
                //    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //    cultureLang);
                this.txtIcdTextName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_DEBATE_DIAGNOSTIC__TXT_SECONDARY_ICD__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);

                this.icdLocation.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__LOCALTION",
                    Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                    cultureLang);
                //this.ictMedicineName.Text = Inventec.Common.Resource.Get.Value(
                //   "IVT_LANGUAGE_KEY__MEDICINE_NAME",
                //   Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //   cultureLang);
                //this.ictUseForm.Text = Inventec.Common.Resource.Get.Value(
                //   "IVT_LANGUAGE_KEY__USE_FORM",
                //   Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //   cultureLang);
                //this.ictDateUse.Text = Inventec.Common.Resource.Get.Value(
                //   "IVT_LANGUAGE_KEY__DATEUSE",
                //   Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //   cultureLang);
                //this.ictHDSD.Text = Inventec.Common.Resource.Get.Value(
                //   "IVT_LANGUAGE_KEY__HDSD",
                //   Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //   cultureLang);
                //this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value(
                //   "IVT_LANGUAGE_KEY__YKienThaoLuan",
                //   Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                //   cultureLang);
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__InTime",
                   Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                   cultureLang);
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__OutTime",
                   Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                   cultureLang);
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__FacultyDepartment",
                   Resources.ResourceLanguageManager.LanguageFormDebateDiagnostic,
                   cultureLang);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                btnSendTMP.Enabled = false;

                dtDebateTime.EditValue = DateTime.Now;
                string strTimeDisplay = DateTime.Now.ToString("dd/MM");
                txtDebateTemp.Focus();
                txtDebateTemp.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToParticipants()
        {
            try
            {
                if (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit)
                {
                    lstParticipantDebate = new List<ADO.HisDebateUserADO>();
                    CommonParam param = new CommonParam();
                    ADO.HisDebateUserADO participant = new ADO.HisDebateUserADO();
                    participant.Action = GlobalVariables.ActionAdd;
                    lstParticipantDebate.Add(participant);
                    gridControl.DataSource = lstParticipantDebate;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToInvateUser()
        {
            try
            {
                if (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit)
                {
                    lstDebateInvateUserADO = new List<ADO.HisDebateInvateUserADO>();
                    CommonParam param = new CommonParam();
                    ADO.HisDebateInvateUserADO participant = new ADO.HisDebateInvateUserADO();
                    participant.Action = GlobalVariables.ActionAdd;
                    lstDebateInvateUserADO.Add(participant);
                    gridControl1.DataSource = lstDebateInvateUserADO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private async Task LoadData(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                hisTreatmentFilter.ID = treatmentId;
                var hisTreatmentList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                if (hisTreatmentList != null && hisTreatmentList.Count > 0)
                {
                    this.vHisTreatment = hisTreatmentList.FirstOrDefault();
                    LoadcboIcdsValue();

                    //Hiển thị thông tin thời gian điều trị
                    dtInTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vHisTreatment.IN_TIME);
                    dtOutTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vHisTreatment.OUT_TIME ?? 0);

                    //Khoa điều trị hiện tại
                    long departmentID = vHisTreatment.LAST_DEPARTMENT_ID ?? 0;
                    if (departmentID > 0)
                    {
                        cboDepartment.EditValue = departmentID;
                        cboDepartment.Enabled = false;
                        //Địa điểm hội chẩn
                        var departments = listDepartment.Where(dp => dp.ID == departmentID).ToList();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => departments), departments));

                        if (departments != null && departments.Count > 0)
                            txtLocation.EditValue = departments.FirstOrDefault().DEPARTMENT_NAME;
                    }

                    //Người yêu cầu
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (!String.IsNullOrEmpty(loginName))
                    {
                        txtRequestLoggin.EditValue = loginName;
                        cboRequestLoggin.EditValue = loginName;
                        txtRequestLoggin.Enabled = false;
                        cboRequestLoggin.Enabled = false;
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicinePrints), medicinePrints));
                    if (this.medicinePrints != null && this.medicinePrints.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("LoadData.1");
                        if (detailProcessor != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("LoadData.2");

                            detailProcessor.SetDataMedicine(this.medicinePrints[0]);
                            Inventec.Common.Logging.LogSystem.Debug("LoadData.3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadcboIcdsValue()
        {
            try
            {
                txtIcdMain.Text = vHisTreatment.ICD_CODE;
                if (!string.IsNullOrEmpty(vHisTreatment.ICD_NAME))
                {
                    checkEdit.Checked = true;
                    icdMainText.Text = vHisTreatment.ICD_NAME;
                    cboIcdMain.EditValue = vHisTreatment.ICD_CODE;
                }
                else
                {
                    cboIcdMain.EditValue = vHisTreatment.ICD_CODE;
                }
                txtIcdTextName.Text = vHisTreatment.ICD_TEXT;
                txtIcdTextCode.Text = vHisTreatment.ICD_SUB_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task DataToComboChuanDoan()
        {
            cboIcdMain.Properties.DataSource = Icds;
            cboIcdMain.Properties.DisplayMember = "ICD_NAME";
            cboIcdMain.Properties.ValueMember = "ICD_CODE";

            cboIcdMain.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            cboIcdMain.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            cboIcdMain.Properties.ImmediatePopup = true;
            cboIcdMain.ForceInitialize();
            cboIcdMain.Properties.View.Columns.Clear();

            DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboIcdMain.Properties.View.Columns.AddField("ICD_CODE");
            aColumnCode.Caption = "Mã";
            aColumnCode.Visible = true;
            aColumnCode.VisibleIndex = 1;
            aColumnCode.Width = 100;

            GridColumn aColumnName = cboIcdMain.Properties.View.Columns.AddField("ICD_NAME");
            aColumnName.Caption = "Tên";
            aColumnName.Visible = true;
            aColumnName.VisibleIndex = 2;
            aColumnName.Width = 200;
        }

        private async Task LoadDataToGridParticipants()
        {
            try
            {
                this.lstReAcsUserADO = new List<AcsUserADO>();

                Action myaction = () =>
                {
                    foreach (var item in acsUsers)
                    {
                        if (string.IsNullOrEmpty(item.USERNAME) || item.IS_ACTIVE != 1)
                            continue;
                        AcsUserADO ado = new AcsUserADO(item);

                        var VhisEmployee = Employee.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                        if (VhisEmployee != null)
                        {
                            ado.DOB = VhisEmployee.DOB;
                            ado.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(VhisEmployee.DOB ?? 0);
                            ado.DIPLOMA = VhisEmployee.DIPLOMA;
                            ado.DEPARTMENT_CODE = VhisEmployee.DEPARTMENT_CODE;
                            ado.DEPARTMENT_ID = VhisEmployee.DEPARTMENT_ID;
                            ado.DEPARTMENT_NAME = VhisEmployee.DEPARTMENT_NAME;
                        }

                        this.lstReAcsUserADO.Add(ado);
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 250, 2));
                columnInfos.Add(new ColumnInfo("DOB_STR", "Ngày sinh", 150, 3));
                columnInfos.Add(new ColumnInfo("DIPLOMA", "CCHN", 150, 4));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa phòng", 200, 5));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 800);
                ControlEditorLoader.Load(LookUpEditUserName, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);

                LoadDataToGridUserInvate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridUserInvate()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 250, 2));
                columnInfos.Add(new ColumnInfo("DOB_STR", "Ngày sinh", 150, 3));
                columnInfos.Add(new ColumnInfo("DIPLOMA", "CCHN", 150, 4));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa phòng", 200, 5));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 800);
                ControlEditorLoader.Load(LookUpEditUserNameInvate, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataDebateDiagnostic(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebate)
        {
            try
            {
                //
                dtInTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisDebate.TREATMENT_FROM_TIME ?? 0);
                dtOutTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisDebate.TREATMENT_TO_TIME ?? 0);
                cboDepartment.EditValue = hisDebate.DEPARTMENT_ID;
                cboRequestLoggin.EditValue = hisDebate.REQUEST_LOGINNAME;
                txtRequestLoggin.EditValue = hisDebate.REQUEST_LOGINNAME;
                txtLocation.EditValue = hisDebate.LOCATION;
                //txtMedicineName.EditValue = hisDebate.MEDICINE_TYPE_NAME;
                //txtConcena.EditValue = hisDebate.MEDICINE_CONCENTRA;
                //txtUserManual.EditValue = hisDebate.MEDICINE_TUTORIAL;
                //txtUseForm.EditValue = hisDebate.MEDICINE_USE_FORM_NAME;
                //dtTimeUse.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisDebate.MEDICINE_USE_TIME ?? 0);
                //txtDiscussion.EditValue = hisDebate.DISCUSSION;
                checkEdit.Checked = false;
                dtDebateTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisDebate.DEBATE_TIME ?? 0);
                if (hisDebate.TRACKING_ID != null)
                {
                    cboPhieuDieuTri.EditValue = hisDebate.TRACKING_ID;
                }

              
                cboDebateType.EditValue = hisDebate.DEBATE_TYPE_ID;
                Inventec.Common.Logging.LogSystem.Debug("DEBATE_REASON_ID___2" + hisDebate.DEBATE_REASON_ID);
                cboDebateReason.EditValue = hisDebate.DEBATE_REASON_ID;
                //txtRequestContent.Text = hisDebate.REQUEST_CONTENT;
                //txtPathologicalHistory.Text = hisDebate.PATHOLOGICAL_HISTORY;
                //txtHospitalizationState.Text = hisDebate.HOSPITALIZATION_STATE;
                //txtBeforeDiagnostic.Text = hisDebate.BEFORE_DIAGNOSTIC;
                //txtTreatmentTracking.Text = hisDebate.TREATMENT_TRACKING;
                //txtDiagnostic.Text = hisDebate.DIAGNOSTIC;
                //txtTreatmentMethod.Text = hisDebate.TREATMENT_METHOD;
                //txtCareMethod.Text = hisDebate.CARE_METHOD;
                //txtConclusion.Text = hisDebate.CONCLUSION;

                txtIcdTextName.Text = hisDebate.ICD_TEXT;
                txtIcdTextCode.Text = hisDebate.ICD_SUB_CODE;
                if (!string.IsNullOrEmpty(hisDebate.ICD_CODE))
                {
                    var icd = Base.GlobalStore.HisIcds.Where(o => o.ICD_CODE == hisDebate.ICD_CODE).FirstOrDefault();
                    cboIcdMain.EditValue = icd.ICD_CODE;
                    txtIcdMain.Text = icd.ICD_CODE;
                }
                if (!string.IsNullOrEmpty(hisDebate.ICD_NAME))
                {
                    checkEdit.Checked = true;
                    icdMainText.Text = hisDebate.ICD_NAME;
                }
                LoadDebateUser(hisDebate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task LoadDebateUser(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebate)
        {

            try
            {
                List<ADO.HisDebateUserADO> lstHisDebateUserADO = new List<ADO.HisDebateUserADO>();

                Action myaction = () =>
                {
                    if (hisDebate.ID > 0)
                    {
                        MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                        hisDebateUserFilter.DEBATE_ID = hisDebate.ID;
                        List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                        if (lstHisDebateUser != null && lstHisDebateUser.Count > 0)
                        {
                            foreach (var item_DebateUser in lstHisDebateUser)
                            {
                                ADO.HisDebateUserADO hisDebateUserADO = new ADO.HisDebateUserADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisDebateUserADO>(hisDebateUserADO, item_DebateUser);
                                if (item_DebateUser.IS_PRESIDENT == 1)
                                {
                                    hisDebateUserADO.PRESIDENT = true;
                                }

                                if (item_DebateUser.IS_SECRETARY == 1)
                                {
                                    hisDebateUserADO.SECRETARY = true;
                                }
                                if (item_DebateUser.IS_OUT_OF_HOSPITAL == 1)
                                {
                                    hisDebateUserADO.OUT_OF_HOSPITAL = true;
                                }
                                hisDebateUserADO.Action = GlobalVariables.ActionEdit;
                                lstHisDebateUserADO.Add(hisDebateUserADO);
                            }
                        }
                        else
                        {
                            MOS.Filter.HisDebateInviteUserFilter hisDebateInviteUserFilter = new MOS.Filter.HisDebateInviteUserFilter();
                            hisDebateInviteUserFilter.DEBATE_ID = hisDebate.ID;
                            List<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER> lstHisDebateInviteUser = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER>>("api/HisDebateInviteUser/Get", ApiConsumer.ApiConsumers.MosConsumer, hisDebateInviteUserFilter, null);

                            if (lstHisDebateInviteUser != null && lstHisDebateInviteUser.Count > 0)
                            {
                                foreach (var item_DebateUser in lstHisDebateInviteUser)
                                {
                                    if (item_DebateUser.IS_PARTICIPATION != 1)
                                        continue;
                                    ADO.HisDebateUserADO hisDebateUserADO = new ADO.HisDebateUserADO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisDebateUserADO>(hisDebateUserADO, item_DebateUser);
                                    if (item_DebateUser.IS_PRESIDENT == 1)
                                    {
                                        hisDebateUserADO.PRESIDENT = true;
                                    }

                                    if (item_DebateUser.IS_SECRETARY == 1)
                                    {
                                        hisDebateUserADO.SECRETARY = true;
                                    }
                                    hisDebateUserADO.Action = GlobalVariables.ActionEdit;

                                    lstHisDebateUserADO.Add(hisDebateUserADO);
                                }
                            }

                        }
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;

                if (lstHisDebateUserADO != null && lstHisDebateUserADO.Count > 0)
                {
                    lstHisDebateUserADO[0].Action = GlobalVariables.ActionAdd;
                    gridControl.DataSource = lstHisDebateUserADO;
                    lstParticipantDebate = lstHisDebateUserADO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private async Task LoadDataInviteDebate(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebate)
        {
            try
            {
                List<ADO.HisDebateInvateUserADO> lstHisDebateInviteUserADO = new List<HisDebateInvateUserADO>();
                MOS.Filter.HisDebateInviteUserFilter hisDebateInviteUserFilter = new MOS.Filter.HisDebateInviteUserFilter();
                hisDebateInviteUserFilter.DEBATE_ID = hisDebate.ID;
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER> lstHisDebateInviteUser = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER>>("api/HisDebateInviteUser/Get", ApiConsumer.ApiConsumers.MosConsumer, hisDebateInviteUserFilter, null);

                if (lstHisDebateInviteUser != null && lstHisDebateInviteUser.Count > 0)
                {
                    foreach (var item_DebateUser in lstHisDebateInviteUser)
                    {
                        ADO.HisDebateInvateUserADO hisDebateUserADO = new ADO.HisDebateInvateUserADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisDebateInvateUserADO>(hisDebateUserADO, item_DebateUser);
                        if (item_DebateUser.IS_PRESIDENT == 1)
                        {
                            hisDebateUserADO.PRESIDENT = true;
                        }

                        if (item_DebateUser.IS_SECRETARY == 1)
                        {
                            hisDebateUserADO.SECRETARY = true;
                        }
                        hisDebateUserADO.Action = GlobalVariables.ActionEdit;
                        if (!(item_DebateUser.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            if (item_DebateUser.IS_PARTICIPATION != null)
                            {
                                hisDebateUserADO.IS_PARTICIPATION_str = lstInvateADO.First(o => o.ID == item_DebateUser.IS_PARTICIPATION).NAME;
                            }
                        }
                        else
                        {
                            if (item_DebateUser.IS_PARTICIPATION != null)
                                hisDebateUserADO.IS_PARTICIPATION_str = item_DebateUser.IS_PARTICIPATION.ToString();
                        }
                        lstHisDebateInviteUserADO.Add(hisDebateUserADO);
                    }
                }

                if (lstHisDebateInviteUserADO != null && lstHisDebateInviteUserADO.Count > 0)
                {
                    lstHisDebateInviteUserADO[0].Action = GlobalVariables.ActionAdd;
                    gridView5.GridControl.DataSource = lstHisDebateInviteUserADO;
                    lstDebateInvateUserADO = lstHisDebateInviteUserADO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataDebateDiagnostic(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisDebate)
        {
            try
            {

                if (!string.IsNullOrEmpty(hisDebate.ICD_NAME))
                {
                    checkEdit.Checked = true;
                    icdMainText.Text = hisDebate.ICD_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DisableControlItem()
        {
            try
            {
                if (this.action == GlobalVariables.ActionView || vHisTreatment.IS_PAUSE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    dtDebateTime.ReadOnly = true;
                    txtRequestLoggin.ReadOnly = true;
                    gridColumnParticipants_Id.OptionsColumn.AllowEdit = false;
                    Gc_LoginName.OptionsColumn.AllowEdit = false;
                    Gc_UserName.OptionsColumn.AllowEdit = false;
                    Gc_President.OptionsColumn.AllowEdit = false;
                    Gc_Secretary.OptionsColumn.AllowEdit = false;
                    Gc_Titles.OptionsColumn.AllowEdit = false;
                    Gc_Add.OptionsColumn.AllowEdit = false;
                    //txtPathologicalHistory.ReadOnly = true;
                    //txtBeforeDiagnostic.ReadOnly = true;
                    //txtHospitalizationState.ReadOnly = true;
                    //txtTreatmentTracking.ReadOnly = true;
                    //txtDiagnostic.ReadOnly = true;
                    //txtCareMethod.ReadOnly = true;
                    //txtTreatmentMethod.ReadOnly = true;
                    //txtConclusion.ReadOnly = true;
                    btnSave.Enabled = false;
                    if (detailProcessor != null)
                    {
                        detailProcessor.DisableControlItem(GetTypeDetail());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadIcdDebate()
        {
            try
            {
                if (string.IsNullOrEmpty(vHisTreatment.ICD_NAME))
                {
                    txtIcdMain.Text = vHisTreatment.ICD_CODE;
                    cboIcdMain.EditValue = vHisTreatment.ICD_CODE;
                    txtIcdTextCode.Text = vHisTreatment.ICD_SUB_CODE;
                    txtIcdTextName.Text = vHisTreatment.ICD_TEXT;
                }
                else
                {
                    checkEdit.Checked = true;
                    txtIcdMain.Text = vHisTreatment.ICD_CODE;
                    cboIcdMain.EditValue = vHisTreatment.ICD_CODE;
                    txtIcdTextName.Text = vHisTreatment.ICD_TEXT;
                    txtIcdTextCode.Text = vHisTreatment.ICD_SUB_CODE;
                    icdMainText.Text = vHisTreatment.ICD_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    bool isOutOfHospital = Inventec.Common.TypeConvert.Parse.ToBoolean((gridView.GetRowCellValue(e.RowHandle, "OUT_OF_HOSPITAL") ?? "").ToString());
                    if (e.Column.FieldName == "BtnDelete")
                    {
                        int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridView.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                        if (action == GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = ButtonAdd;
                        }
                        else if (action == GlobalVariables.ActionEdit)
                        {
                            e.RepositoryItem = ButtonDelete;
                        }
                    }
                    else if (e.Column.FieldName == "USERNAME")
                    {
                        if (isOutOfHospital)
                        {
                            e.RepositoryItem = TextEditUserName;
                        }
                        else
                        {
                            e.RepositoryItem = LookUpEditUserName;
                            string loginName = (gridView.GetRowCellValue(e.RowHandle, "LOGINNAME") ?? "").ToString();
                            gridView.SetRowCellValue(e.RowHandle, Gc_UserName, loginName);
                        }
                    }
                    else if (e.Column.FieldName == "LOGINNAME")
                    {
                        if (isOutOfHospital)
                            e.RepositoryItem = TextEditLoginNameDis;
                        else
                            e.RepositoryItem = TextEditLoginName;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "USERNAME")
                    {
                        try
                        {
                            string status = (view.GetRowCellValue(e.ListSourceRowIndex, "USERNAME") ?? "").ToString();
                            ACS.EFMODEL.DataModels.ACS_USER data = Base.GlobalStore.HisAcsUser.SingleOrDefault(o => o.LOGINNAME == status);
                            e.Value = data.USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi hien thi gia tri cot USERNAME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "USERNAME")
                {
                    gridView.ShowEditor();
                    ((HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit)gridView.ActiveEditor).ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }    
        }

        private void FillDatatoControlByHisDebateTemp(HIS_DEBATE_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    datacombox = data;
                    HIS_DEBATE hisDebate = new HIS_DEBATE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DEBATE>(hisDebate, data);
                    hisDebate.DISCUSSION = data.DISCUSSION;

                    if (detailProcessor != null)
                    {
                        var debate = new HIS_DEBATE();
                        detailProcessor.GetData(GetTypeDetail(), ref debate);
                        switch (GetTypeDetail())
                        {
                            case DetailEnum.Khac:
                                //qtcode
                                debate.SUBCLINICAL_PROCESSES = IsCheckLockInfor(debate.SUBCLINICAL_PROCESSES) ? hisDebate.SUBCLINICAL_PROCESSES : debate.SUBCLINICAL_PROCESSES;
                                //qtcode
                                debate.MEDICINE_TYPE_IDS = IsCheckLockInfor(debate.MEDICINE_TYPE_IDS) ? hisDebate.MEDICINE_TYPE_IDS : debate.MEDICINE_TYPE_IDS;
                                debate.ACTIVE_INGREDIENT_IDS = IsCheckLockInfor(debate.ACTIVE_INGREDIENT_IDS) ? hisDebate.ACTIVE_INGREDIENT_IDS : debate.ACTIVE_INGREDIENT_IDS;
                                debate.MEDICINE_TYPE_NAME = IsCheckLockInfor(debate.MEDICINE_TYPE_NAME) ? hisDebate.MEDICINE_TYPE_NAME : debate.MEDICINE_TYPE_NAME;
                                debate.MEDICINE_CONCENTRA = IsCheckLockInfor(debate.MEDICINE_CONCENTRA) ? hisDebate.MEDICINE_CONCENTRA : debate.MEDICINE_CONCENTRA;
                                debate.MEDICINE_USE_FORM_NAME = IsCheckLockInfor(debate.MEDICINE_USE_FORM_NAME) ? hisDebate.MEDICINE_USE_FORM_NAME : debate.MEDICINE_USE_FORM_NAME;
                                debate.MEDICINE_TUTORIAL = IsCheckLockInfor(debate.MEDICINE_TUTORIAL) ? hisDebate.MEDICINE_TUTORIAL : debate.MEDICINE_TUTORIAL;
                                debate.DISCUSSION = IsCheckLockInfor(debate.DISCUSSION) ? hisDebate.DISCUSSION : debate.DISCUSSION;
                                debate.BEFORE_DIAGNOSTIC = IsCheckLockInfor(debate.BEFORE_DIAGNOSTIC) ? hisDebate.BEFORE_DIAGNOSTIC : debate.BEFORE_DIAGNOSTIC;
                                debate.CARE_METHOD = IsCheckLockInfor(debate.CARE_METHOD) ? hisDebate.CARE_METHOD : debate.CARE_METHOD;
                                debate.CONCLUSION = IsCheckLockInfor(debate.CONCLUSION) ? hisDebate.CONCLUSION : debate.CONCLUSION;
                                debate.DIAGNOSTIC = IsCheckLockInfor(debate.DIAGNOSTIC) ? hisDebate.DIAGNOSTIC : debate.DIAGNOSTIC;
                                debate.HOSPITALIZATION_STATE = IsCheckLockInfor(debate.HOSPITALIZATION_STATE) ? hisDebate.HOSPITALIZATION_STATE : debate.HOSPITALIZATION_STATE;
                                debate.PATHOLOGICAL_HISTORY = IsCheckLockInfor(debate.PATHOLOGICAL_HISTORY) ? hisDebate.PATHOLOGICAL_HISTORY : debate.PATHOLOGICAL_HISTORY;
                                debate.REQUEST_CONTENT = IsCheckLockInfor(debate.REQUEST_CONTENT) ? hisDebate.REQUEST_CONTENT : debate.REQUEST_CONTENT;
                                debate.TREATMENT_METHOD = IsCheckLockInfor(debate.TREATMENT_METHOD) ? hisDebate.TREATMENT_METHOD : debate.TREATMENT_METHOD;
                                debate.TREATMENT_TRACKING = IsCheckLockInfor(debate.TREATMENT_TRACKING) ? hisDebate.TREATMENT_TRACKING : debate.TREATMENT_TRACKING;
                                debate.MEDICINE_USE_TIME = IsCheckLockInfor(debate.MEDICINE_USE_TIME.ToString()) ? hisDebate.MEDICINE_USE_TIME : debate.MEDICINE_USE_TIME;
                                break;
                            case DetailEnum.Thuoc:
                                //qtcode
                                debate.SUBCLINICAL_PROCESSES = IsCheckLockInfor(debate.SUBCLINICAL_PROCESSES) ? hisDebate.SUBCLINICAL_PROCESSES : debate.SUBCLINICAL_PROCESSES;
                                //qtcode

                                debate.MEDICINE_TYPE_IDS = IsCheckLockInfor(debate.MEDICINE_TYPE_IDS) ? hisDebate.MEDICINE_TYPE_IDS : debate.MEDICINE_TYPE_IDS;
                                debate.ACTIVE_INGREDIENT_IDS = IsCheckLockInfor(debate.ACTIVE_INGREDIENT_IDS) ? hisDebate.ACTIVE_INGREDIENT_IDS : debate.ACTIVE_INGREDIENT_IDS;
                                debate.MEDICINE_TYPE_NAME = IsCheckLockInfor(debate.MEDICINE_TYPE_NAME) ? hisDebate.MEDICINE_TYPE_NAME : debate.MEDICINE_TYPE_NAME;
                                debate.MEDICINE_CONCENTRA = IsCheckLockInfor(debate.MEDICINE_CONCENTRA) ? hisDebate.MEDICINE_CONCENTRA : debate.MEDICINE_CONCENTRA;
                                debate.MEDICINE_USE_FORM_NAME = IsCheckLockInfor(debate.MEDICINE_USE_FORM_NAME) ? hisDebate.MEDICINE_USE_FORM_NAME : debate.MEDICINE_USE_FORM_NAME;
                                debate.MEDICINE_TUTORIAL = IsCheckLockInfor(debate.MEDICINE_TUTORIAL) ? hisDebate.MEDICINE_TUTORIAL : debate.MEDICINE_TUTORIAL;
                                debate.DISCUSSION = IsCheckLockInfor(debate.DISCUSSION) ? hisDebate.DISCUSSION : debate.DISCUSSION;
                                debate.BEFORE_DIAGNOSTIC = IsCheckLockInfor(debate.BEFORE_DIAGNOSTIC) ? hisDebate.BEFORE_DIAGNOSTIC : debate.BEFORE_DIAGNOSTIC;
                                debate.CARE_METHOD = IsCheckLockInfor(debate.CARE_METHOD) ? hisDebate.CARE_METHOD : debate.CARE_METHOD;
                                debate.CONCLUSION = IsCheckLockInfor(debate.CONCLUSION) ? hisDebate.CONCLUSION : debate.CONCLUSION;
                                debate.DIAGNOSTIC = IsCheckLockInfor(debate.DIAGNOSTIC) ? hisDebate.DIAGNOSTIC : debate.DIAGNOSTIC;
                                debate.HOSPITALIZATION_STATE = IsCheckLockInfor(debate.HOSPITALIZATION_STATE) ? hisDebate.HOSPITALIZATION_STATE : debate.HOSPITALIZATION_STATE;
                                debate.PATHOLOGICAL_HISTORY = IsCheckLockInfor(debate.PATHOLOGICAL_HISTORY) ? hisDebate.PATHOLOGICAL_HISTORY : debate.PATHOLOGICAL_HISTORY;

                                debate.REQUEST_CONTENT = IsCheckLockInfor(debate.REQUEST_CONTENT) ? hisDebate.REQUEST_CONTENT : debate.REQUEST_CONTENT;
                                debate.TREATMENT_METHOD = IsCheckLockInfor(debate.TREATMENT_METHOD) ? hisDebate.TREATMENT_METHOD : debate.TREATMENT_METHOD;
                                debate.TREATMENT_TRACKING = IsCheckLockInfor(debate.TREATMENT_TRACKING) ? hisDebate.TREATMENT_TRACKING : debate.TREATMENT_TRACKING;
                                debate.MEDICINE_USE_TIME = IsCheckLockInfor(debate.MEDICINE_USE_TIME.ToString()) ? hisDebate.MEDICINE_USE_TIME : debate.MEDICINE_USE_TIME;
                                break;
                            case DetailEnum.Pttt:
                                debate.SUBCLINICAL_PROCESSES = IsCheckLockInfor(debate.SUBCLINICAL_PROCESSES) ? hisDebate.SUBCLINICAL_PROCESSES : debate.SUBCLINICAL_PROCESSES;
                                debate.INTERNAL_MEDICINE_STATE = IsCheckLockInfor(debate.INTERNAL_MEDICINE_STATE) ? hisDebate.INTERNAL_MEDICINE_STATE : debate.INTERNAL_MEDICINE_STATE;
                                debate.TREATMENT_TRACKING = IsCheckLockInfor(debate.TREATMENT_TRACKING) ? hisDebate.TREATMENT_TRACKING : debate.TREATMENT_TRACKING;
                                debate.TREATMENT_METHOD = IsCheckLockInfor(debate.TREATMENT_METHOD) ? hisDebate.TREATMENT_METHOD : debate.TREATMENT_METHOD;
                                debate.PTTT_METHOD_ID = IsCheckLockInfor(debate.PTTT_METHOD_ID.ToString()) ? hisDebate.PTTT_METHOD_ID : debate.PTTT_METHOD_ID;
                                debate.PTTT_METHOD_NAME = IsCheckLockInfor(debate.PTTT_METHOD_NAME) ? hisDebate.PTTT_METHOD_NAME : debate.PTTT_METHOD_NAME;
                                debate.EMOTIONLESS_METHOD_ID = IsCheckLockInfor(debate.EMOTIONLESS_METHOD_ID.ToString()) ? hisDebate.EMOTIONLESS_METHOD_ID : debate.EMOTIONLESS_METHOD_ID;
                                debate.SURGERY_TIME = IsCheckLockInfor(debate.SURGERY_TIME.ToString()) ? hisDebate.SURGERY_TIME : debate.SURGERY_TIME;
                                debate.PROGNOSIS = IsCheckLockInfor(debate.PROGNOSIS) ? hisDebate.PROGNOSIS : debate.PROGNOSIS;
                                debate.CONCLUSION = IsCheckLockInfor(debate.CONCLUSION) ? hisDebate.CONCLUSION : debate.CONCLUSION;
                                debate.PATHOLOGICAL_HISTORY = IsCheckLockInfor(debate.PATHOLOGICAL_HISTORY) ? hisDebate.PATHOLOGICAL_HISTORY : debate.PATHOLOGICAL_HISTORY;
                                debate.HOSPITALIZATION_STATE = IsCheckLockInfor(debate.HOSPITALIZATION_STATE) ? hisDebate.HOSPITALIZATION_STATE : debate.HOSPITALIZATION_STATE;
                                debate.BEFORE_DIAGNOSTIC = IsCheckLockInfor(debate.BEFORE_DIAGNOSTIC) ? hisDebate.BEFORE_DIAGNOSTIC : debate.BEFORE_DIAGNOSTIC;
                                debate.DISCUSSION = IsCheckLockInfor(debate.DISCUSSION) ? hisDebate.DISCUSSION : debate.DISCUSSION;
                                debate.CARE_METHOD = IsCheckLockInfor(debate.CARE_METHOD) ? hisDebate.CARE_METHOD : debate.CARE_METHOD;
                                //debate.HIS_DEBATE_EKIP_USER = IsCheckLockInforList((debate.HIS_DEBATE_EKIP_USER).ToList()) ? hisDebate.HIS_DEBATE_EKIP_USER : debate.HIS_DEBATE_EKIP_USER;
                                break;
                            default:
                                break;
                        }
                        detailProcessor.SetData(GetTypeDetail(), debate);
                    }

                    HisDebateUserFilter dbuFilter = new HisDebateUserFilter();
                    dbuFilter.DEBATE_TEMP_ID = data.ID;
                    List<HIS_DEBATE_USER> listUser = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, dbuFilter, null);
                    List<ADO.HisDebateUserADO> lstHisDebateUserADO = new List<ADO.HisDebateUserADO>();
                    foreach (var item_DebateUser in listUser)
                    {
                        ADO.HisDebateUserADO hisDebateUserADO = new ADO.HisDebateUserADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisDebateUserADO>(hisDebateUserADO, item_DebateUser);
                        if (item_DebateUser.IS_PRESIDENT == 1)
                        {
                            hisDebateUserADO.PRESIDENT = true;
                        }

                        if (item_DebateUser.IS_SECRETARY == 1)
                        {
                            hisDebateUserADO.SECRETARY = true;
                        }
                        if (item_DebateUser.IS_OUT_OF_HOSPITAL == 1)
                        {
                            hisDebateUserADO.OUT_OF_HOSPITAL = true;
                        }
                        hisDebateUserADO.Action = GlobalVariables.ActionEdit;
                        lstHisDebateUserADO.Add(hisDebateUserADO);
                    }

                    if (lstHisDebateUserADO != null && lstHisDebateUserADO.Count > 0)
                    {
                        lstHisDebateUserADO[0].Action = GlobalVariables.ActionAdd;
                    }
                    else
                    {
                        ADO.HisDebateUserADO participant = new ADO.HisDebateUserADO();
                        participant.Action = GlobalVariables.ActionAdd;
                        lstHisDebateUserADO.Add(participant);
                    }

                    gridControl.DataSource = lstHisDebateUserADO;
                    lstParticipantDebate = lstHisDebateUserADO;
                }
                else
                {
                    HIS_DEBATE hisDebate = new HIS_DEBATE();
                    if (detailProcessor != null)
                    {
                        detailProcessor.SetData(GetTypeDetail(), hisDebate);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitComboDebateTemp()
        {
            try
            {
                List<HIS_DEBATE_TEMP> data = null;
                Action myaction = () =>
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEBATE_TEMP>()
                        .Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE &&
                            (p.IS_PUBLIC == 1 || p.DEPARTMENT_ID == WorkPlaceSDO.DepartmentId || p.CREATOR == loginName)).ToList();
                };
                Task task = new Task(myaction);
                task.Start();

                await task;

                List<ColumnInfo> columnInfor = new List<ColumnInfo>();
                columnInfor.Add(new ColumnInfo("DEBATE_TEMP_CODE", "", 100, 1));
                columnInfor.Add(new ColumnInfo("DEBATE_TEMP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEBATE_TEMP_NAME", "ID", columnInfor, false, 350);
                ControlEditorLoader.Load(this.cboDebateTemp, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDebateTempCombo(string _Code)
        {
            try
            {
                List<HIS_DEBATE_TEMP> listResult = new List<HIS_DEBATE_TEMP>();
                listResult = BackendDataWorker.Get<HIS_DEBATE_TEMP>().Where(o => (o.DEBATE_TEMP_CODE != null && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.DEBATE_TEMP_CODE.StartsWith(_Code))).ToList();

                if (listResult.Count == 1)
                {
                    cboDebateTemp.EditValue = listResult[0].ID;
                    txtDebateTemp.Text = listResult[0].DEBATE_TEMP_CODE;
                    dtDebateTime.Focus();
                }
                else if (listResult.Count > 1)
                {
                    cboDebateTemp.EditValue = null;
                    cboDebateTemp.Focus();
                    cboDebateTemp.ShowPopup();
                }
                else
                {
                    cboDebateTemp.EditValue = null;
                    cboDebateTemp.Focus();
                    cboDebateTemp.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region shotcut
        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnSave.Enabled)
                    return;
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

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

        private void chkAutoSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAdd = (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0) ? this.currentControlStateRDO__Create.Where(o => o.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_DEBATE).FirstOrDefault() : null;

                HIS.Desktop.Library.CacheClient.ControlStateRDO csUpdate = (this.currentControlStateRDO__Update != null && this.currentControlStateRDO__Update.Count > 0) ? this.currentControlStateRDO__Update.Where(o => o.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_DEBATE_DIAGNOSTIC).FirstOrDefault() : null;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAdd), csAdd));
                if (csAdd != null)
                {
                    csAdd.VALUE = (chkAutoSign.Checked ? "1" : "");
                }
                else if (csUpdate != null)
                {
                    csUpdate.VALUE = (chkAutoSign.Checked ? "1" : "");
                }
                else
                {
                    csAdd = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAdd.KEY = ControlStateConstant.CHECK_CLEAR_TO_ADD;
                    csAdd.VALUE = (chkAutoSign.Checked ? "1" : "");
                    csAdd.MODULE_LINK = ControlStateConstant.MODULE_LINK_DEBATE;
                    if (this.currentControlStateRDO__Create == null)
                        this.currentControlStateRDO__Create = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Create.Add(csAdd);

                    csUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csUpdate.KEY = ControlStateConstant.CHECK_CLEAR_TO_ADD;
                    csUpdate.VALUE = (chkAutoSign.Checked ? "1" : "");
                    csUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK_DEBATE_DIAGNOSTIC;
                    if (this.currentControlStateRDO__Update == null)
                        this.currentControlStateRDO__Update = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Update.Add(csUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO__Create);
                this.controlStateWorker.SetData(this.currentControlStateRDO__Update);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoCreateEmr_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAdd = (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0) ? this.currentControlStateRDO__Create.Where(o => o.KEY == ControlStateConstant.CHECK_AUTO_CREATE_EMR && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_DEBATE).FirstOrDefault() : null;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAdd), csAdd));
                if (csAdd != null)
                {
                    csAdd.VALUE = (chkAutoCreateEmr.Checked ? "1" : "");
                }
                else
                {
                    csAdd = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAdd.KEY = ControlStateConstant.CHECK_AUTO_CREATE_EMR;
                    csAdd.VALUE = (chkAutoCreateEmr.Checked ? "1" : "");
                    csAdd.MODULE_LINK = ControlStateConstant.MODULE_LINK_DEBATE;
                    if (this.currentControlStateRDO__Create == null)
                        this.currentControlStateRDO__Create = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Create.Add(csAdd);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO__Create);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckThuoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckThuoc.Checked)
                {
                    ChkPttt.Checked = false;
                    ChkOther.Checked = false;
                    LoadDataControlDetail();
                    var data = BackendDataWorker.Get<HIS_DEBATE_TEMP>().FirstOrDefault(p => p.ID == Int64.Parse(cboDebateTemp.EditValue.ToString()));
                    FillDatatoControlByHisDebateTemp(data);
                    if (detailProcessor != null)
                    {
                        detailProcessor.SetData(GetTypeDetail(), currentHisDebate);
                    }
                }
                else if (!ChkPttt.Checked && !ChkOther.Checked)
                {
                    CheckThuoc.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkPttt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChkPttt.Checked)
                {
                    CheckThuoc.Checked = false;      
                    ChkOther.Checked = false;
                    LoadDataControlDetail();
                    var data = BackendDataWorker.Get<HIS_DEBATE_TEMP>().FirstOrDefault(p => p.ID == Int64.Parse(cboDebateTemp.EditValue.ToString()));
                    FillDatatoControlByHisDebateTemp(data);

                    //if (detailProcessor != null)
                    //{
                    //    detailProcessor.SetData(GetTypeDetail(), currentHisDebate);
                    //}
                    if (this.action == GlobalVariables.ActionAdd)
                    {
                        if (this.examServiceReq == null)
                        {
                            examServiceReq = ProcessGetExamServiceReq();
                        }
                        if (detailProcessor != null && examServiceReq != null)
                        {
                            //detailProcessor.SetData(GetTypeDetail(), examServiceReq);
                            //var data = BackendDataWorker.Get<HIS_DEBATE_TEMP>().FirstOrDefault(p => p.ID == Int64.Parse(cboDebateTemp.EditValue.ToString()));
                            FillDatatoControlByHisDebateTemp(data);
                        }
                    }
                }
                else if (!CheckThuoc.Checked && !ChkOther.Checked)
                {
                    ChkPttt.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_SERVICE_REQ ProcessGetExamServiceReq()
        {
            HIS_SERVICE_REQ result = null;
            try
            {
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.TREATMENT_ID = this.treatment_id;
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                filter.IS_NO_EXECUTE = false;
                var listServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    result = listServiceReq.OrderBy(o => o.IS_MAIN_EXAM ?? 9999).ThenBy(o => o.INTRUCTION_TIME).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ChkOther_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChkOther.Checked)
                {
                    ChkPttt.Checked = false;
                    CheckThuoc.Checked = false;
                    LoadDataControlDetail();
                    HIS_DEBATE_TEMP data = null;
                    data = BackendDataWorker.Get<HIS_DEBATE_TEMP>().FirstOrDefault(p => p.ID == Int64.Parse(cboDebateTemp.EditValue.ToString()));
                    //if (cboDebateTemp.EditValue!= null)
                    //{
                    //    data = BackendDataWorker.Get<HIS_DEBATE_TEMP>().FirstOrDefault(p => p.ID == Int64.Parse(cboDebateTemp.EditValue.ToString()));
                    //}
                    FillDatatoControlByHisDebateTemp(data);
                }
                else if (!ChkPttt.Checked && !CheckThuoc.Checked)     
                {
                    ChkOther.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataControlDetail()
        {
            try
            {
                panelDetail.Controls.Clear();

                if (detailProcessor == null)
                {
                    if (hisService != null)
                    {

                        detailProcessor = new DetailProcessor(treatment_id, moduleData.RoomId, moduleData.RoomTypeId, hisService, moduleData);
                    }
                    else
                    {
                        detailProcessor = new DetailProcessor(treatment_id, moduleData.RoomId, moduleData.RoomTypeId, moduleData);
                    }
                }
                detailProcessor.DepartmentList = listDepartment;
                detailProcessor.UserList = acsUsers;
                detailProcessor.ExecuteRoleList = ListExecuteRole;
                detailProcessor.EmployeeList = Employee;
                ucDetail = detailProcessor.GetControl(GetTypeDetail());
                panelDetail.Controls.Add(ucDetail);
                ucDetail.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataControlFirstCheck(object data)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private DetailEnum GetTypeDetail()
        {
            DetailEnum result = DetailEnum.Thuoc;
            try
            {
                if (ChkPttt.Checked)
                {
                    result = UcDebateDetail.DetailEnum.Pttt;
                }
                else if (ChkOther.Checked)
                {
                    result = UcDebateDetail.DetailEnum.Khac;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void repositoryItemCboExecuteRole_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    List<object> sendObj = new List<object>();
                    sendObj.Add(true);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisExecuteRole", moduleData.RoomId, moduleData.RoomTypeId, sendObj);
                    ProcessLoadExecuteRole();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExecuteList()
        {
            ProcessLoadExecuteRole();
        }

        private void ProcessLoadExecuteRole()
        {
            try
            {
                LoadDataToExecuteRole(repositoryItemCboExecuteRole, ListExecuteRole);
                LoadDataToExecuteRole(recboExecuteRole, ListExecuteRole);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                //var row = (ADO.HisDebateUserADO)gridView.GetFocusedRow();

                if (view.FocusedColumn.FieldName == Gc_Titles.FieldName && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        FillDataIntoExecuteRoleCombo(editor);
                        //editor.EditValue = row.EXECUTE_ROLE_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataIntoExecuteRoleCombo(GridLookUpEdit editor)
        {
            try
            {
                var data = ListExecuteRole.Where(o => o.IS_POSITION == 1 || o.IS_TITLE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(editor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToExecuteRole(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo, List<HIS_EXECUTE_ROLE> data)
        {
            try
            {
                cbo.DataSource = data;
                cbo.DisplayMember = "EXECUTE_ROLE_NAME";
                cbo.ValueMember = "ID";

                cbo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.ImmediatePopup = true;
                cbo.View.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.View.Columns.AddField("EXECUTE_ROLE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.View.Columns.AddField("EXECUTE_ROLE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var ado = (ADO.HisDebateUserADO)this.gridView.GetFocusedRow();
                if (ado != null)
                {
                    if (e.Column.FieldName == Gc_Titles.FieldName)
                    {
                        var data = ListExecuteRole.Where(o => o.ID == ado.EXECUTE_ROLE_ID).ToList();
                        if (data != null && data.Count > 0)
                        {
                            ado.DESCRIPTION = data.First().EXECUTE_ROLE_NAME;
                        }
                        gridControl.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboExecuteRole_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit edit = sender as GridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    if ((edit.EditValue ?? 0).ToString() != (edit.OldEditValue ?? 0).ToString())
                    {
                        var data = ListExecuteRole.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(edit.EditValue.ToString())).ToList();
                        if (data != null && data.Count > 0)
                        {
                            gridView.SetRowCellValue(gridView.FocusedRowHandle, Gc_Titles, data.First().ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTracking(GridLookUpEdit cbo)
        {
            try
            {
                // cbo.Properties.Buttons[1].Visible = false;
                if (trackingADOs == null)
                    return;

                if (cbo.EditValue == null)
                {
                    if (this.actionType == GlobalVariables.ActionEdit)
                    {
                        cbo.EditValue = null;
                        // cbo.Properties.Buttons[1].Visible = false;
                    }
                    //else
                    //{
                    //    //Neu la nhieu ngay thi khong gan gia tri mac dinh
                    //    if (this.ucDateProcessor.GetChkMultiDateState(this.ucDate) == false)
                    //    {
                    //        cbo.EditValue = trackingADOs[0].ID; //Set mac dinh cai dau tien
                    //        cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
                    //    }
                    //}
                }
                //else
                //{
                //    if (chkMultiIntructionTime.Checked)
                //    {
                //        cbo.EditValue = null;
                //        cbo.Properties.Buttons[1].Visible = false;
                //    }
                //}

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TrackingTimeStr", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, trackingADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhieuDieuTri_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //cboPhieuDieuTri.Properties.Buttons[1].Visible = (cboPhieuDieuTri.EditValue != null);
                if (this.isInitTracking)
                {
                    return;
                }
                //if (HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
                //{
                if (cboPhieuDieuTri.EditValue != null)
                {
                    var trackingData = this.trackingADOs != null ? this.trackingADOs.FirstOrDefault(o => o.ID == (long)cboPhieuDieuTri.EditValue) : null;
                    if (trackingData != null)
                    {
                        this.tracking = new HIS_TRACKING();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRACKING>(tracking, trackingData);
                    }
                    else
                    {
                        this.tracking = null;
                    }
                }
                else
                {
                    this.tracking = null;
                }
                //this.LoadIcdDefault();
                // }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhieuDieuTri_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPhieuDieuTri.Properties.Buttons[1].Visible = false;
                    cboPhieuDieuTri.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(vHisTreatment.ID);
                        listArgs.Add((Action<HIS_TRACKING>)this.ProcessAfterChangeTrackingTime);//TODO
                        //if (this.currentDhst != null)
                        //{
                        //    listArgs.Add(this.currentDhst);
                        //}
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                        this.LoadDataTracking();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhieuDieuTri_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhieuDieuTri.EditValue != null)
                    {
                        cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
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
                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.1__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tracking), tracking));
                //Trường hợp tạo/sửa tờ điều trị trước khi lưu đơn
                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();

                dateInputADO.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(tracking.TRACKING_TIME).Value;

                dateInputADO.Dates = new List<DateTime?>();
                dateInputADO.Dates.Add(dateInputADO.Time);

                //UcDateReload(dateInputADO);
                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.2");
                //if (this.actionType == GlobalVariables.ActionView)
                //{
                //    //Trường hợp tạo/sửa tờ điều trị sau khi lưu đơn(nút lưu bị disable) ==> tự động gọi hàm lưu kê đơn để cập nhật lại ngày kê đơn theo ngày ở tờ điều trị
                //    Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.3");
                //    // this.InitWorker();
                //    this.actionType = GlobalVariables.ActionEdit;

                //    //  this.ProcessSaveData(HIS.Desktop.Plugins.AssignPrescriptionPK.SAVETYPE.SAVE);//TODO cần check tiếp
                //    Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.4");
                //}
                Inventec.Common.Logging.LogSystem.Debug("ProcessAfterChangeTrackingTime.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Co loi khi delegate ProcessAfterChangeTrackingTime duoc goi tu chuc nang tao/sua to dieu tri", ex);
            }
        }
        //private void InitWorker()
        //{
        //    try
        //    {
        //        AssignPrescriptionWorker.MediMatyCreateWorker = new MediMatyCreateWorker(GetDataAmountOutOfStock, SetDefaultMediStockForData, ChoosePatientTypeDefaultlService, ChoosePatientTypeDefaultlServiceOther, GetPatientTypeId, GetNumRow, SetNumRow, GetMediMatyTypeADOs, GetIsAutoCheckExpend);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        private void LoadDataTracking(bool isChangeData = true)
        {
            try
            {
                // Neu truyen vao tracking thi hien thi mac dinh tracking nay #23109
                // Set giá trị mặc định cho tờ điều trị ở chức năng tủ trực #24587
                // + nếu tờ điều trị đã được lưu, thì chọn mặc định tờ điều trị trên form kê đơn là tờ điều trị đang được mở.
                // + nếu tờ điều trị chưa được lưu, thì hiển thị mặc định theo cấu hình hệ thống "HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking"
                LogSystem.Debug("LoadDataTracking => 1");

                CommonParam param = new CommonParam();
                HisTrackingFilter filter = new HisTrackingFilter();
                filter.TREATMENT_ID = treatment_id;
                filter.DEPARTMENT_ID = WorkPlaceSDO.DepartmentId;
                List<HIS_TRACKING> trackings = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);

                if (trackings == null || trackings.Count == 0)
                {
                    this.isInitTracking = false;
                    return;
                }

                trackings = trackings.OrderByDescending(o => o.TRACKING_TIME).ToList();

                this.trackingADOs = new List<TrackingADO>();
                foreach (var item in trackings)
                {
                    TrackingADO tracking = new TrackingADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TrackingADO>(tracking, item);
                    tracking.TrackingTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(tracking.TRACKING_TIME);
                    this.trackingADOs.Add(tracking);
                }
                trackingADOs = trackingADOs.OrderByDescending(o => o.TRACKING_TIME).ToList();
                InitComboTracking(cboPhieuDieuTri); // chết ở hàm này
                if (HisConfigCFG.DebateDiagnostic_IsDefaultTracking == true && (this.action == GlobalVariables.ActionAdd || this.currentHisDebate == null))
                {
                    ChangeIntructionTime(dtDebateTime.DateTime);
                    List<string> intructionDateSelectedProcess = new List<string>();
                    foreach (var item in this.intructionTimeSelecteds)
                    {
                        string intructionDate = item.ToString().Substring(0, 8);
                        intructionDateSelectedProcess.Add(intructionDate);
                    }

                    var trackingTemps = trackings.Where(o => intructionDateSelectedProcess.Contains(o.TRACKING_TIME.ToString().Substring(0, 8))
                        && o.DEPARTMENT_ID == WorkPlaceSDO.DepartmentId)
                        .OrderByDescending(o => o.TRACKING_TIME).ToList();
                    if (trackingTemps != null && trackingTemps.Count > 0)
                    {
                        cboPhieuDieuTri.EditValue = trackingTemps.First().ID;
                        //cboPhieuDieuTri.Properties.Buttons[1].Visible = true;  
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDebateTime_EditValueChanged(object sender, EventArgs e)
        {
            if (!IsNotLoadFirst && HisConfigCFG.DebateDiagnostic_IsDefaultTracking == true)
            {
                LoadDataTracking();
            }
        }
        private void ChangeIntructionTime(DateTime intructTime)
        {
            try
            {


                this.intructionTimeSelected = new List<DateTime?>();
                this.intructionTimeSelected.Add(intructTime);
                this.intructionTimeSelecteds = (this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList());
                this.InstructionTime = this.intructionTimeSelecteds.OrderByDescending(o => o).First();
                this.cboPhieuDieuTri.EditValue = null;
                //this.LoadDataTracking();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.intructionTimeSelecteds), this.intructionTimeSelecteds));
                Inventec.Common.Logging.LogSystem.Debug("ChangeIntructionTime. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBienBanHoiChanCu_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBienBanHoiChanCu.Properties.Buttons[1].Visible = false;
                    cboBienBanHoiChanCu.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBienBanHoiChanCu_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBienBanHoiChanCu.EditValue != null)
                    {
                        cboBienBanHoiChanCu.Properties.Buttons[1].Visible = true;

                        // panelDetail.Controls.Clear();
                        HIS_DEBATE debatechoose = lstDebate.Where(o => o.ID == Int32.Parse(cboBienBanHoiChanCu.EditValue.ToString())).FirstOrDefault();

                        if (debatechoose != null && debatechoose.CONTENT_TYPE.HasValue)
                        {
                            if (debatechoose.CONTENT_TYPE.Value == (long)DetailEnum.Khac)
                            {
                                ChkOther.Checked = true;
                            }
                            else if (debatechoose.CONTENT_TYPE.Value == (long)DetailEnum.Pttt)
                            {
                                ChkPttt.Checked = true;
                            }
                            else
                            {
                                CheckThuoc.Checked = true;
                            }
                        }

                        if (detailProcessor != null)
                        {
                            detailProcessor.SetData(GetTypeDetail(), debatechoose);
                        }


                        SetDataHisDebate(debatechoose);
                        SetDataDebateInvateUser(debatechoose);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        //---NEW
        private void EnableColumnAddDelete()
        {
            try
            {
                if ((this.cboRequestLoggin.EditValue ?? "").ToString() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                {
                    gridColumn8.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    gridColumn8.OptionsColumn.AllowEdit = false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboThamGia()
        {
            try
            {
                lstInvateADO = new List<InvateADO>();
                lstInvateADO.Add(new InvateADO(1, "Có"));
                lstInvateADO.Add(new InvateADO(0, "Không"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(regluThamGia, lstInvateADO, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTreatmentHistory_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                List<object> listArgs = new List<object>();
                //Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                TreatmentHistoryADO currentInput = new TreatmentHistoryADO();
                currentInput.treatmentId = vHisTreatment.ID;
                currentInput.treatment_code = vHisTreatment.TREATMENT_CODE;
                listArgs.Add(currentInput);
                WaitingManager.Hide();
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentHistory", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView5_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "FeedBack")
                    {
                        if ((this.cboRequestLoggin.EditValue ?? "").ToString() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            e.RepositoryItem = rebtnFeedBackEnable;
                        }
                        else
                        {
                            e.RepositoryItem = rebtnFeedBackDisable;
                        }
                    }
                    else if (e.Column.FieldName == "BtnDeleteInvateUser")
                    {
                        int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridView5.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                        if (action == GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = rebtnAddInvateUser;
                        }
                        else if (action == GlobalVariables.ActionEdit)
                        {
                            e.RepositoryItem = rebtnMinusInvateUser;
                        }
                    }
                    else if (e.Column.FieldName == "IS_PARTICIPATION_str")
                    {
                        if ((gridView5.GetRowCellValue(e.RowHandle, "LOGINNAME") ?? "").ToString() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && Int64.Parse((gridView5.GetRowCellValue(e.RowHandle, "ID") ?? "").ToString()) > 0)
                        {
                            if (!string.IsNullOrEmpty((gridView5.GetRowCellValue(e.RowHandle, "IS_PARTICIPATION_str") ?? "").ToString()) && ((gridView5.GetRowCellValue(e.RowHandle, "IS_PARTICIPATION_str") ?? "").ToString() == "Có" || (gridView5.GetRowCellValue(e.RowHandle, "IS_PARTICIPATION_str") ?? "").ToString() == "Không"))
                            {
                                long? id = lstInvateADO.First(o => o.NAME == (gridView5.GetRowCellValue(e.RowHandle, "IS_PARTICIPATION_str") ?? "").ToString()).ID;
                                gridView5.SetRowCellValue(e.RowHandle, gridColumn2, id);
                            }

                            e.RepositoryItem = regluThamGia;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty((gridView5.GetRowCellValue(e.RowHandle, "IS_PARTICIPATION_str") ?? "").ToString()) && ((gridView5.GetRowCellValue(e.RowHandle, "IS_PARTICIPATION_str") ?? "").ToString() == "1" || (gridView5.GetRowCellValue(e.RowHandle, "IS_PARTICIPATION_str") ?? "").ToString() == "0"))
                            {
                                string name = lstInvateADO.First(o => o.ID == Int16.Parse((gridView5.GetRowCellValue(e.RowHandle, "IS_PARTICIPATION_str") ?? "").ToString())).NAME;
                                gridView5.SetRowCellValue(e.RowHandle, gridColumn2, name);
                            }
                            e.RepositoryItem = reTxtDisable;
                        }
                    }
                    else if (e.Column.FieldName == "USERNAME")
                    {
                        if ((this.cboRequestLoggin.EditValue ?? "").ToString() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            e.RepositoryItem = LookUpEditUserNameInvate;
                            string loginName = (gridView5.GetRowCellValue(e.RowHandle, "LOGINNAME") ?? "").ToString();
                            gridView5.SetRowCellValue(e.RowHandle, gridColumn3, loginName);
                        }
                        else
                        {
                            e.RepositoryItem = reTxtDisable;
                        }
                    }
                    else if (e.Column.FieldName == "EXECUTE_ROLE_ID")
                    {
                        if ((this.cboRequestLoggin.EditValue ?? "").ToString() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            e.RepositoryItem = recboExecuteRole;
                        }
                        else
                        {
                            e.RepositoryItem = reTxtDisable;
                        }
                    }
                    else if (e.Column.FieldName == "PRESIDENT")
                    {
                        if ((this.cboRequestLoggin.EditValue ?? "").ToString() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            e.RepositoryItem = rechkChuToa;
                        }
                        else
                        {
                            e.RepositoryItem = rechkChuToaDisable;
                        }
                    }
                    else if (e.Column.FieldName == "SECRETARY")
                    {
                        if ((this.cboRequestLoggin.EditValue ?? "").ToString() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            e.RepositoryItem = rechkThuKy;
                        }
                        else
                        {
                            e.RepositoryItem = rechkThuKyDisable;
                        }
                    }
                    else if (e.Column.FieldName == "COMMENT_DOCTOR")
                    {
                        if ((gridView5.GetRowCellValue(e.RowHandle, "LOGINNAME") ?? "").ToString() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && (gridView5.GetRowCellValue(e.RowHandle, "IS_PARTICIPATION") ?? "").ToString() == "1")
                        {
                            e.RepositoryItem = reTxtComment;
                        }
                        else
                        {
                            e.RepositoryItem = reTxtDisable;
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rebtnAddInvateUser_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                ADO.HisDebateInvateUserADO participant = new ADO.HisDebateInvateUserADO();
                lstDebateInvateUserADO = gridControl1.DataSource as List<HisDebateInvateUserADO>;
                if (lstDebateInvateUserADO != null && lstDebateInvateUserADO.Count > 0)
                {
                    HisDebateInvateUserADO parti = new HisDebateInvateUserADO();
                    lstDebateInvateUserADO.Add(parti);
                    lstDebateInvateUserADO.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    lstDebateInvateUserADO.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = lstDebateInvateUserADO;
                }
                else
                {
                    HisDebateInvateUserADO parti = new HisDebateInvateUserADO();
                    lstDebateInvateUserADO.Add(parti);
                    lstDebateInvateUserADO.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = lstDebateInvateUserADO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rebtnMinusInvateUser_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                CommonParam param = new CommonParam();
                var participant = (ADO.HisDebateInvateUserADO)gridView5.GetFocusedRow();
                if (participant != null)
                {
                    lstDebateInvateUserADO.Remove(participant);
                    lstDebateInvateUserADO.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    lstDebateInvateUserADO.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = lstDebateInvateUserADO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rebtnFeedBackEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.HisDebateInvateUserADO)gridView5.GetFocusedRow();
                if (row != null)
                {
                    detailProcessor.SetDataDiscussion(GetTypeDetail(), row.COMMENT_DOCTOR);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView5_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "USERNAME")
                    {
                        try
                        {
                            string status = (view.GetRowCellValue(e.ListSourceRowIndex, "USERNAME") ?? "").ToString();
                            ACS.EFMODEL.DataModels.ACS_USER data = Base.GlobalStore.HisAcsUser.SingleOrDefault(o => o.LOGINNAME == status);
                            e.Value = data.USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi hien thi gia tri cot USERNAME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void regluThamGia_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    var cpn = sender as GridLookUpEdit;
                    cpn.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void regluThamGia_Closed(object sender, ClosedEventArgs e)
        {

        }

        private void recboExecuteRole_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    List<object> sendObj = new List<object>();
                    sendObj.Add(true);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisExecuteRole", moduleData.RoomId, moduleData.RoomTypeId, sendObj);
                    ProcessLoadExecuteRole();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LookUpEditUserNameInvate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit edit = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    if ((edit.EditValue ?? 0).ToString() != (edit.OldEditValue ?? 0).ToString())
                    {
                        var participant = Base.GlobalStore.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == edit.EditValue.ToString());
                        if (participant != null)
                        {
                            gridView5.SetRowCellValue(gridView5.FocusedRowHandle, gcLoginName, participant.LOGINNAME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void reTxtComment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Down)
                {
                    var participant = (ADO.HisDebateInvateUserADO)gridView5.GetFocusedRow();
                    if (!string.IsNullOrEmpty(participant.COMMENT_DOCTOR))
                    {
                        memContent.Text = participant.COMMENT_DOCTOR;
                    }
                    else
                    {
                        memContent.Text = "";
                    }
                    ButtonEdit editor = sender as ButtonEdit;
                    Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                    string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
                    string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();

                    if (Int64.Parse(screenWidth) > 1600)
                    {
                        popupControlContainer1.Height = 200;
                        popupControlContainer1.Width = 500;
                    }
                    else
                    {
                        popupControlContainer1.Height = 200;
                        popupControlContainer1.Width = 350;
                    }
                    popupControlContainer1.ShowPopup(new Point(buttonPosition.X + 650, buttonPosition.Bottom + 170));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                var participant = (ADO.HisDebateInvateUserADO)gridView5.GetFocusedRow();
                if (!string.IsNullOrEmpty(memContent.Text))
                {
                    participant.COMMENT_DOCTOR = memContent.Text;
                }
                else
                {
                    participant.COMMENT_DOCTOR = null;
                }
                gridView5.SetRowCellValue(gridView5.FocusedRowHandle, gridColumn7, participant.COMMENT_DOCTOR);
                popupControlContainer1.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                memContent.Text = "";
                popupControlContainer1.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void reTxtComment_Leave(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                var participant = (ADO.HisDebateInvateUserADO)gridView5.GetFocusedRow();
                if (participant != null && !string.IsNullOrEmpty(participant.COMMENT_DOCTOR))
                {
                    string strName = "";
                    if (Inventec.Common.String.CountVi.Count(participant.COMMENT_DOCTOR) > 1000)
                    {
                        strName = participant.LOGINNAME + " - " + Base.GlobalStore.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == participant.LOGINNAME).USERNAME + " có \"Nhận xét\" vượt quá 1000 ký tự.\r\n";
                    }
                    if (!string.IsNullOrEmpty(strName))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(strName, "Thông báo danh sách mời tham gia", MessageBoxButtons.OK);
                        return;
                    }
                }
                WaitingManager.Show();
                HIS_DEBATE_INVITE_USER obj = new HIS_DEBATE_INVITE_USER();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER>(obj, participant);
                var name = Base.GlobalStore.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == participant.LOGINNAME);
                if (name != null)
                {
                    obj.LOGINNAME = name.LOGINNAME;
                    if (!string.IsNullOrEmpty(name.USERNAME))
                    {
                        obj.USERNAME = name.USERNAME;
                    }
                }
                obj.ID = 0;
                if (participant.ID > 0)
                {
                    obj.ID = participant.ID;
                }
                if (participant.EXECUTE_ROLE_ID > 0)
                    obj.DESCRIPTION = ListExecuteRole.FirstOrDefault(o => o.ID == participant.EXECUTE_ROLE_ID).EXECUTE_ROLE_NAME;
                if (participant.PRESIDENT == true)
                    obj.IS_PRESIDENT = 1;
                else
                    obj.IS_PRESIDENT = null;

                if (participant.SECRETARY == true)

                    obj.IS_SECRETARY = 1;
                else
                    obj.IS_SECRETARY = null;
                var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER>("api/HisDebateInviteUser/Update", ApiConsumers.MosConsumer, obj, param);
                if (resultData != null)
                {
                    success = true;
                }
                WaitingManager.Hide();

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void regluThamGia_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void reTxtComment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    reTxtComment_Leave(null, null);
                    gridView5.FocusedColumn = gridView5.VisibleColumns[7];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView5_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "USERNAME")
                {
                    gridView5.ShowEditor();
                    ((HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit)gridView5.ActiveEditor).ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void reTxtComment_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                var participant = (ADO.HisDebateInvateUserADO)gridView5.GetFocusedRow();
                ButtonEdit txt = sender as ButtonEdit;
                if (!string.IsNullOrEmpty(txt.Text))
                {
                    participant.COMMENT_DOCTOR = txt.Text.Trim();
                }
                else
                {
                    participant.COMMENT_DOCTOR = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void regluThamGia_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit grd = sender as GridLookUpEdit;

                if (grd.EditValue == null)
                    return;
                CommonParam param = new CommonParam();
                bool success = false;
                var participant = (ADO.HisDebateInvateUserADO)gridView5.GetFocusedRow();
                if (participant != null && !string.IsNullOrEmpty(participant.COMMENT_DOCTOR))
                {
                    string strName = "";
                    if (Inventec.Common.String.CountVi.Count(participant.COMMENT_DOCTOR) > 1000)
                    {
                        strName = participant.LOGINNAME + " - " + Base.GlobalStore.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == participant.LOGINNAME).USERNAME + " có \"Nhận xét\" vượt quá 1000 ký tự.\r\n";
                    }
                    if (!string.IsNullOrEmpty(strName))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(strName, "Thông báo danh sách mời tham gia", MessageBoxButtons.OK);
                        return;
                    }
                }
                WaitingManager.Show();
                string NameParticipation = "";
                //if (participant.IS_PARTICIPATION_str == "1" || participant.IS_PARTICIPATION_str == "0")
                //{
                //	NameParticipation = lstInvateADO.First(o => o.ID == Int16.Parse(participant.IS_PARTICIPATION_str)).NAME;
                //	gridView5.SetRowCellValue(gridView5.FocusedRowHandle, gridColumn2, NameParticipation);

                //}
                //else
                //{
                NameParticipation = lstInvateADO.First(o => o.ID == Int16.Parse(grd.EditValue.ToString())).NAME;
                gridView5.SetRowCellValue(gridView5.FocusedRowHandle, gridColumn2, grd.EditValue);
                gridView5.SetRowCellValue(gridView5.FocusedRowHandle, gridColumn9, grd.EditValue);
                //}
                participant.IS_PARTICIPATION = Int16.Parse(grd.EditValue.ToString());
                HIS_DEBATE_INVITE_USER obj = new HIS_DEBATE_INVITE_USER();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER>(obj, participant);
                var name = Base.GlobalStore.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == participant.LOGINNAME);
                if (name != null)
                {
                    obj.LOGINNAME = name.LOGINNAME;
                    if (!string.IsNullOrEmpty(name.USERNAME))
                    {
                        obj.USERNAME = name.USERNAME;
                    }
                }
                obj.ID = 0;
                if (participant.ID > 0)
                {
                    obj.ID = participant.ID;
                }
                if (participant.EXECUTE_ROLE_ID > 0)
                    obj.DESCRIPTION = ListExecuteRole.FirstOrDefault(o => o.ID == participant.EXECUTE_ROLE_ID).EXECUTE_ROLE_NAME;
                if (participant.PRESIDENT == true)
                    obj.IS_PRESIDENT = 1;
                else
                    obj.IS_PRESIDENT = null;

                if (participant.SECRETARY == true)

                    obj.IS_SECRETARY = 1;
                else
                    obj.IS_SECRETARY = null;

                var hisDebateInvateResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER>("api/HisDebateInviteUser/Update", ApiConsumer.ApiConsumers.MosConsumer, obj, param);
                if (hisDebateInvateResult != null)
                {
                    success = true;
                    SDA.EFMODEL.DataModels.SDA_NOTIFY updateDTO = new SDA.EFMODEL.DataModels.SDA_NOTIFY();
                    updateDTO.CONTENT = String.Format("Tài khoản {0} - {1}  xác nhận {2} tham gia hội chẩn bệnh nhân {3} – {4}, {5}. Mời bạn vào chức năng “Tạo biên bản hội chẩn” để xem chi tiết",
                        Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                        Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName(),
                        NameParticipation,
                        vHisTreatment.TREATMENT_CODE,
                        vHisTreatment.TDL_PATIENT_NAME,
                        listDepartment.FirstOrDefault(o => o.ID == Int64.Parse((cboDepartment.EditValue ?? "").ToString())).DEPARTMENT_NAME);
                    updateDTO.TITLE = "Xác nhận mời tham gia hội chẩn";
                    updateDTO.FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    updateDTO.TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "235959");
                    updateDTO.RECEIVER_LOGINNAME = this.cboRequestLoggin.EditValue != null ? this.cboRequestLoggin.EditValue.ToString() : "";

                    Inventec.Common.Logging.LogSystem.Debug("updateDTO___SDA" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                    var resultData = new BackendAdapter(param).Post<SDA.EFMODEL.DataModels.SDA_NOTIFY>("api/SdaNotify/Create", ApiConsumers.SdaConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        //TODO
                    }
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this, param, success);

                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView5_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "COMMENT_DOCTOR")
                {
                    this.gridView_CustomRowError(sender, e);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridView5.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControl1.DataSource as List<HisDebateInvateUserADO>;
                var row = listDatas[index];
                if (e.ColumnName == "COMMENT_DOCTOR")
                {
                    if (row.ErrorTypeCommentDoctor == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning)
                    {
                        e.Info.ErrorType = (DevExpress.XtraEditors.DXErrorProvider.ErrorType)(row.ErrorTypeCommentDoctor);
                        e.Info.ErrorText = (string)(row.ErrorMessageCommentDoctor);
                    }
                    else
                    {
                        e.Info.ErrorType = (DevExpress.XtraEditors.DXErrorProvider.ErrorType)(DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidMicrobiProcessing(HisDebateInvateUserADO ado)
        {
            try
            {
                if (ado != null)
                {
                    if (!String.IsNullOrEmpty(ado.COMMENT_DOCTOR) && Inventec.Common.String.CountVi.Count(ado.COMMENT_DOCTOR) > 1000)
                    {

                        ado.ErrorMessageCommentDoctor = "Vượt quá độ dài cho phép 1000 ký tự";
                        ado.ErrorTypeCommentDoctor = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                        return;
                    }
                    else
                    {
                        ado.ErrorMessageCommentDoctor = "";
                        ado.ErrorTypeCommentDoctor = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView5_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var AntibioticMicrobiADO = (HisDebateInvateUserADO)this.gridView5.GetFocusedRow();
                if (AntibioticMicrobiADO != null)
                {
                    if (e.Column.FieldName == "COMMENT_DOCTOR")
                    {
                        //ValidMicrobiProcessing(AntibioticMicrobiADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSendTMP_Click(object sender, EventArgs e)
        {
            try
            {
                var connectInfo = HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.Telemedicine.ConnectionInfo");

                if (!string.IsNullOrEmpty(connectInfo))
                {
                    var infoArr = connectInfo.Split('|');
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("infoArr___", infoArr));
                    HIS.Library.Telemedicine.TelemedicineProcessor tl = new HIS.Library.Telemedicine.TelemedicineProcessor(infoArr[0], infoArr[1], infoArr[2]);
                    if (this.vHisTreatment == null || this.vHisTreatment.ID == 0)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                        hisTreatmentFilter.ID = currentHisDebate.TREATMENT_ID;
                        var hisTreatmentList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                        vHisTreatment = hisTreatmentList.FirstOrDefault();
                    }

                    List<V_HIS_SERVICE_REQ> lstServiceReqs = new List<V_HIS_SERVICE_REQ>();
                    List<HIS_SERE_SERV> lstSereServs = new List<HIS_SERE_SERV>();
                    List<V_HIS_SERE_SERV_TEIN> lstSereServTein = new List<V_HIS_SERE_SERV_TEIN>();
                    List<V_HIS_TREATMENT_BED_ROOM> lstTreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
                    V_HIS_PATIENT patient = new V_HIS_PATIENT();
                    V_HIS_DEBATE debate = new V_HIS_DEBATE();



                    if (cboPhieuDieuTri.EditValue != null)
                    {
                        lstServiceReqs = GetServiceReqs__Connect(Inventec.Common.TypeConvert.Parse.ToInt64(cboPhieuDieuTri.EditValue.ToString()));
                        lstSereServs = GetSereServs__Connect(vHisTreatment.ID);
                        lstSereServTein = GetSereServTein__Connect(vHisTreatment.ID);
                    }
                    else
                    {
                        lstServiceReqs = GetServiceReqsExecute__Connect(vHisTreatment.ID);
                        lstSereServs = GetSereServs__Connect(vHisTreatment.ID);
                        lstSereServTein = GetSereServTein__Connect(vHisTreatment.ID);
                    }

                    lstTreatmentBedRooms = GetTreatmentBedRoom__Connect(vHisTreatment.ID);
                    patient = GetPatient__Connect(vHisTreatment.PATIENT_ID);
                    debate = GetDebate__Connect(currentHisDebate.ID);
                    var rsConnect = tl.SendToTmp(patient, vHisTreatment, debate, lstServiceReqs, lstSereServs, lstSereServTein, lstTreatmentBedRooms, HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch);
                    if (!rsConnect.Success)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(rsConnect.Message, "Thông báo", MessageBoxButtons.OK);
                        return;
                    }

                    CommonParam paramCommon = new CommonParam();
                    DebateTelemedicineSDO sdo = new DebateTelemedicineSDO();
                    sdo.TmpId = rsConnect.TmpId;
                    sdo.DebateId = debate.ID;
                    var rs = new BackendAdapter(paramCommon).Post<bool>("api/HisDebate/UpdateTelemedicineInfo", ApiConsumers.MosConsumer, sdo, paramCommon);
                    MessageManager.Show(this, paramCommon, rs);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<V_HIS_SERVICE_REQ> GetServiceReqsExecute__Connect(long treatmentId)
        {
            List<V_HIS_SERVICE_REQ> lstServiceReqsExecute = new List<V_HIS_SERVICE_REQ>();
            try
            {
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.TREATMENT_ID = treatmentId;
                lstServiceReqsExecute = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, null);
                if (lstServiceReqsExecute.Count() > 0)
                {
                    lstServiceReqsExecute = lstServiceReqsExecute.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lstServiceReqsExecute;
        }

        V_HIS_DEBATE GetDebate__Connect(long debateId)
        {
            V_HIS_DEBATE debate = new V_HIS_DEBATE();
            try
            {
                MOS.Filter.HisDebateViewFilter dbFilter = new MOS.Filter.HisDebateViewFilter();
                dbFilter.ID = debateId;
                CommonParam param = new CommonParam();
                debate = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_DEBATE>>("api/HisDebate/GetView", ApiConsumer.ApiConsumers.MosConsumer, dbFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return debate;
        }

        V_HIS_PATIENT GetPatient__Connect(long patientId)
        {
            V_HIS_PATIENT patient = new V_HIS_PATIENT();
            try
            {
                HisPatientViewFilter filter = new HisPatientViewFilter();
                filter.ID = patientId;
                patient = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, null).First();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return patient;
        }

        List<V_HIS_TREATMENT_BED_ROOM> GetTreatmentBedRoom__Connect(long treatmentId)
        {
            List<V_HIS_TREATMENT_BED_ROOM> lstTreatmentBedRoom = new List<V_HIS_TREATMENT_BED_ROOM>();
            try
            {
                HisTreatmentBedRoomViewFilter filter = new HisTreatmentBedRoomViewFilter();
                filter.TREATMENT_ID = treatmentId;
                lstTreatmentBedRoom = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lstTreatmentBedRoom;
        }

        List<V_HIS_SERE_SERV_TEIN> GetSereServTein__Connect(long treatmentId)
        {
            List<V_HIS_SERE_SERV_TEIN> lstSereServTein = new List<V_HIS_SERE_SERV_TEIN>();
            try
            {
                HisSereServTeinViewFilter filter = new HisSereServTeinViewFilter();
                filter.TDL_TREATMENT_ID = treatmentId;
                lstSereServTein = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lstSereServTein;
        }

        List<HIS_SERE_SERV> GetSereServs__Connect(long treatmentId)
        {
            List<HIS_SERE_SERV> lstSereServs = new List<HIS_SERE_SERV>();
            try
            {
                HisSereServFilter filter = new HisSereServFilter();
                filter.TREATMENT_ID = treatmentId;
                lstSereServs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lstSereServs;
        }

        List<V_HIS_SERVICE_REQ> GetServiceReqs__Connect(long trackingId)
        {
            List<V_HIS_SERVICE_REQ> lstServiceReqs = new List<V_HIS_SERVICE_REQ>();
            try
            {
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.TRACKING_ID = trackingId;
                lstServiceReqs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lstServiceReqs;
        }
        private void cboDebateReason_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDebateReason.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckEditBsNgoaiVien_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit checkBSNgoaiVien = sender as CheckEdit;
                if (checkBSNgoaiVien == null) return;
                if (checkBSNgoaiVien.Checked)
                {
                    gridView.SetRowCellValue(gridView.FocusedRowHandle, Gc_OutOfHospital, true);
                }
                else
                {
                    gridView.SetRowCellValue(gridView.FocusedRowHandle, Gc_OutOfHospital, false);
                }
                gridView.SetRowCellValue(gridView.FocusedRowHandle, Gc_LoginName, null);
                gridView.SetRowCellValue(gridView.FocusedRowHandle, Gc_UserName, null);
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
        
        private bool IsCheckLockInfor(string input)
        {
            return !chkLockInfor.Checked || (chkLockInfor.Checked && string.IsNullOrEmpty(input));
        }

        private bool IsCheckLockInforList(List<HIS_DEBATE_EKIP_USER> lstData)
        {
            return !chkLockInfor.Checked || (chkLockInfor.Checked && (lstData == null || lstData.Count == 0));
        }

        private void chkAutoCreateTracking_CheckedChanged(object sender, EventArgs e) //  đây là cái checkbox cần thêm
        {

        }
    }
}
