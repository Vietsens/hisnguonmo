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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Delegate;
using HIS.Desktop.Plugins.SurgServiceReqExecute.EkipTemp;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Config;
using DevExpress.XtraGrid.Views.Tile;
using Inventec.Common.Logging;
using System.IO;
using System.Drawing;
using DevExpress.XtraTab;
using HIS.Desktop.ModuleExt;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.RichEditor.Base;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class SurgServiceReqExecuteControl : UserControlBase
    {
        #region Khai báo
        internal UpdatePatientStatus updatePatientStatus;
        public int positionHandle = -1;
        internal List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs { get; set; }
        internal HisSurgServiceReqUpdateListSDO currentHisSurgResultSDO { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serviceReq { get; set; }
        internal MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT SereServExt { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5 sereServ { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServbyServiceReqs { get; set; }
        internal List<PtttMethodADO> listSesePtttMetod { get; set; }

        internal MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT sereServPTTT { get; set; }
        internal V_HIS_TREATMENT vhisTreatment { get; set; }
        List<HIS_BLOOD_ABO> dataBloodAbo = null;
        List<HIS_BLOOD_RH> dataBloodRh = null;
        internal HIS_PATIENT Patient { get; set; }
        internal List<V_HIS_SERE_SERV_5> sereServInEkips { get; set; }
        internal List<V_HIS_SERE_SERV_5> sereServInPackages { get; set; }
        internal Inventec.Desktop.Common.Modules.Module Module;
        internal List<V_HIS_SERE_SERV_PTTT> hisSereServPttt = new List<V_HIS_SERE_SERV_PTTT>();
        internal List<HIS_EXECUTE_ROLE_USER> executeRoleUsers { get; set; }
        internal List<HIS_EKIP_TEMP> ekipTemps { get; set; }
        private bool isAllowEditInfo;
        private bool isStartTimeMustBeGreaterThanInstructionTime;
        internal List<HIS_SERE_SERV> sereServLasts { get; set; }
        internal List<AcsUserADO> AcsUserADOList { get; set; }
        internal List<HIS_ICD> dataIcds { get; set; }
        internal List<HIS_ICD_CM> dataIcdCms { get; set; }
        internal long autoCheckIcd;
        private long MannerIsRequired;
        private long MethodIsRequired;
        private long PriorityIsRequired;
        private long IcdCmIsRequired;
        private bool isEstimateDuration = true;
        private HIS_SERVICE currentHisService;

        private HIS_SERE_SERV sereServLast;
        internal bool checkSTT = false;
        List<HIS_STENT_CONCLUDE> stentConcludeSave;
        private HIS_EYE_SURGRY_DESC currentEyeSurgDesc;
        private SkinSurgeryDesADO SkinSurgeryDes;

        internal List<HIS_SERE_SERV_EXT> ListSereServExt = new List<HIS_SERE_SERV_EXT>();
        private List<HIS_SERE_SERV_PTTT> hisSereServPttt_forSaveGroup = new List<HIS_SERE_SERV_PTTT>();
        V_HIS_SERE_SERV_PTTT SereServePTTTInfoStent = new V_HIS_SERE_SERV_PTTT();
        internal HisSurgServiceReqUpdateSDO currentSurgResultSDO { get; set; }

        private bool isNotLoadWhileChangeControlStateInFirst;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private string MODULELINK = "FormEkipUser";
        private List<HisEkipUserADO> lstAllEkip = new List<HisEkipUserADO>();
        List<ImageADO> imageADOs;
        ImageADO currentImageADO;
        List<HIS_DEPARTMENT> departmentClinic;
        UCEkipUser ucEkip;
        List<EkipUsersADO> ListEkipUser = new List<EkipUsersADO>();
        bool IsChoosePTTT = false;
        bool IsReadOnlyGridViewEkipUser = false;
        Dictionary<long, long> dicSereServCopy = new Dictionary<long, long>();
        List<V_HIS_SERVICE> lstService;
        List<HIS_EMOTIONLESS_METHOD> dataEmotionlessMethod { get; set; }
        DXErrorProvider dxErrorProviver = new DXErrorProvider();
        public static long ServiceId;
        #endregion

        #region Contructor
        public SurgServiceReqExecuteControl()
            : base()
        {
            InitializeComponent();
        }

        public SurgServiceReqExecuteControl(UpdatePatientStatus updatePatientStatus)
            : base()
        {
            this.updatePatientStatus = updatePatientStatus;
            InitializeComponent();
        }

        public SurgServiceReqExecuteControl(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_SERVICE_REQ serviceReq)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.serviceReq = serviceReq;
                this.Module = moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void SurgServiceReqExecuteControl_Load(object sender, EventArgs e)
        {
            try
            {
                LoadServiceFromRam();
                ucEkip = new UCEkipUser();
                panelControl5.Controls.Add(ucEkip);
                Inventec.Common.Logging.LogSystem.Debug("SurgServiceReqExecuteControl_Load. 1");
                isNotLoadWhileChangeControlStateInFirst = true;
                this.InitLanguage();
                this.ComboMethodICD();
                this.InitControlState();
                Inventec.Common.Logging.LogSystem.Debug("SurgServiceReqExecuteControl_Load. 2");
                this.LoadTreatment();
                this.LoadPatient();
                this.LoadExecuteRoleUser();

                Inventec.Common.Logging.LogSystem.Debug("SurgServiceReqExecuteControl_Load. 3");
                AcsUserADOList = ProcessAcsUser();
                LoadDataTocboUser();
                this.LoadDataToComboPtttTemp();
                Inventec.Common.Logging.LogSystem.Debug("SurgServiceReqExecuteControl_Load. 4");
                this.SetIcdFromServiceReq(this.serviceReq);
                ucEkip.FillDataToInformationSurg();
                Inventec.Common.Logging.LogSystem.Debug("SurgServiceReqExecuteControl_Load. 5");
                this.isAllowEditInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeys.MOS__HIS_SERVICE_REQ__ALLOW_UPDATE_SURG_INFO_AFTER_LOCKING_TREATMENT) == "1";
                this.isStartTimeMustBeGreaterThanInstructionTime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeys.StartTimeMustBeGreaterThanInstructionTime) == "1" || HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeys.StartTimeMustBeGreaterThanInstructionTime) == "2";
                this.MannerIsRequired = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(ConfigKeys.RequiredMannerOption);
                this.MethodIsRequired = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(ConfigKeys.RequiredMethodOption);
                this.PriorityIsRequired = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(ConfigKeys.RequiredPriorityOption);
                this.IcdCmIsRequired = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(ConfigKeys.RequiredIcdCmOption);

                if (PriorityIsRequired == 1 || (PriorityIsRequired == 2 && this.serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT))
                {
                    this.layoutControlItem17.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }

                if (MethodIsRequired == 1 || (MethodIsRequired == 2 && this.serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT))
                {
                    lciPhuongPhap.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }

                if (MannerIsRequired == 1 || (MannerIsRequired == 2 && this.serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT))
                {
                    lciCachThuc.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }
                if (IcdCmIsRequired == 1)
                {
                    lciIcdCmCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }
                timerInitForm.Enabled = true;
                timerInitForm.Start();
                isNotLoadWhileChangeControlStateInFirst = false;

                btnFinish.Enabled = !(HisConfigKeys.allowFinishWhenAccountIsDoctor == "1" && BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault().IS_DOCTOR != 1);

                Inventec.Common.Logging.LogSystem.Debug("SurgServiceReqExecuteControl_Load. 6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadServiceFromRam()
        {
            try
            {
                lstService = BackendDataWorker.Get<V_HIS_SERVICE>();
                ServiceId = sereServ.SERVICE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDefaultControl()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("_________HisConfigCFG.REQUIRED_GROUPPTTT_OPTION:", HisConfigCFG.REQUIRED_GROUPPTTT_OPTION));
                if (HisConfigCFG.REQUIRED_GROUPPTTT_OPTION != "1")
                {
                    this.sereServ = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5)grdViewService.GetFocusedRow();
                    if (this.sereServ != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("_________sereServ:", sereServ));
                        var surgMisuService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                        if (surgMisuService != null)
                        {
                            if (surgMisuService != null)
                            {
                                this.currentHisService = new HIS_SERVICE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(currentHisService, surgMisuService);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("_________currentHisService(lúc map data):", currentHisService));
                                if (surgMisuService.IS_OUT_OF_MANAGEMENT != null && surgMisuService.IS_OUT_OF_MANAGEMENT == 1)
                                {
                                    ucEkip.SetColorTitle(false);
                                }
                                else
                                {
                                    ucEkip.SetColorTitle(true);
                                }

                                if (!surgMisuService.PTTT_GROUP_ID.HasValue)
                                {
                                    lciPhanLoai.AppearanceItemCaption.ForeColor = Color.Black;
                                }
                                else
                                {
                                    lciPhanLoai.AppearanceItemCaption.ForeColor = Color.Maroon;
                                }
                                if (surgMisuService.ESTIMATE_DURATION != null && surgMisuService.ESTIMATE_DURATION > 0)
                                {
                                    spinExcuteTimeAdd.EditValue = surgMisuService.ESTIMATE_DURATION;
                                }
                                else
                                {
                                    this.isEstimateDuration = false;
                                    InitControlState();
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
        }

        private void timerInitForm_Tick(object sender, EventArgs e)
        {
            try
            {
                this.ValidateControl();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                this.GetSereServByTreatment();

                ucEkip.Dock = DockStyle.Fill;
                ucEkip.SetDepartmentID(Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue != null ? cboDepartment.EditValue.ToString() : "0"));
                ucEkip.FillDataToInformationSurg();
                this.ComboEkipTemp(cboEkipTemp);
                this.LoadComboDepartment(cboDepartment);
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 2");
                this.InitButtonPhatSinh();
                this.InitButtonGPBL();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 3");
                this.timerInitForm.Stop();
            }
            catch (Exception ex)
            {

                this.timerInitForm.Stop();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.SPIN_EXECUTE_TIME && !this.isEstimateDuration)
                        {
                            spinExcuteTimeAdd.Value = Convert.ToDecimal(item.VALUE);
                        }
                        else if (item.KEY == chkSaveGroup.Name)
                        {
                            chkSaveGroup.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkServiceCode.Name)
                        {
                            chkServiceCode.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkKetThuc.Name)
                        {
                            chkKetThuc.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkSign.Name)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdControlService_Click(object sender, EventArgs e)
        {
            try
            {
                this.checkSTT = false;
                this.listSesePtttMetod = this.listSesePtttMetod.Where(o => o.SERVICE_REQ_ID == this.sereServ.SERVICE_REQ_ID).ToList();
                CommonParam param = new CommonParam();
                if (chkSaveGroup.Checked)
                {
                    // cập nhật "Cách thức" và “Phân loại” vào ram
                    UpdateForSaveGroup();
                }

                this.sereServ = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5)grdViewService.GetFocusedRow();

                if (this.sereServ != null)
                {
                    lblIntructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(sereServ.TDL_INTRUCTION_TIME);
                    Inventec.Common.Logging.LogSystem.Info("grdControlService_Click");
                    if (sereServ.SERVICE_REQ_ID != null && sereServ.SERVICE_REQ_ID > 0)
                        LoadExcutime(sereServ.SERVICE_ID);
                    LoadSereServExt();
                    LoadSereServLast();
                    GetSereServPtttBySereServId();
                    //chỉ load thông tin cách thức và “Phân loại” khi check in gộp
                    if (chkSaveGroup.Checked)
                    {
                        var currentService = lstService.FirstOrDefault(o => o.ID == this.sereServ.SERVICE_ID);

                        var pttt_forSaveGroup = hisSereServPttt_forSaveGroup.FirstOrDefault(o => o.SERE_SERV_ID == (dicSereServCopy != null && dicSereServCopy.Count > 0 && dicSereServCopy.ContainsKey(sereServ.ID) ? dicSereServCopy[sereServ.ID] : this.sereServ.ID));
                        if (pttt_forSaveGroup != null)
                        {
                            this.txtMANNER.Text = pttt_forSaveGroup.MANNER;
                            this.cbbPtttGroup.EditValue = pttt_forSaveGroup.PTTT_GROUP_ID;
                        }
                        else
                        {
                            this.txtMANNER.Text = this.sereServ.TDL_SERVICE_NAME;
                            this.cbbPtttGroup.EditValue = currentService != null ? currentService.PTTT_GROUP_ID : null;
                        }
                        this.txtMANNER.Focus();
                        this.txtMANNER.SelectionStart = txtMANNER.Text.Length;
                        this.txtMANNER.SelectionLength = 0;
                    }
                    else
                    {
                        this.refreshControl();
                        // Load cac chi so
                        FillDataDefaultToControl();
                        SetDataControlBySereServPttt();
                        SetDefaultCboPTTTGroup(sereServ);
                        SetDefaultCboICD9CmChinh(sereServ);
                        LoadDetailSereServPttt();
                        ucEkip.SetDepartmentID(Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue != null ? cboDepartment.EditValue.ToString() : "0"));
                        ucEkip.FillDataToInformationSurg(true);
                    }

                    SetEnableControl();
                    SetDefaultCboPTMethod(this.sereServ, cboMethod, txtMethodCode);
                    SetDefaultCboPTMethod(this.sereServ, cboPhuongPhapThucTe, txtPhuongPhapTT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadExcutime(long? serviceId)
        {
            try
            {
                var surgMisuService = lstService.FirstOrDefault(o => o.ID == serviceId);
                if (surgMisuService != null)
                {
                    if (surgMisuService != null)
                    {
                        if (surgMisuService.ESTIMATE_DURATION != null && surgMisuService.ESTIMATE_DURATION > 0)
                        {
                            spinExcuteTimeAdd.EditValue = surgMisuService.ESTIMATE_DURATION;
                        }
                        else
                        {
                            this.isEstimateDuration = false;
                            InitControlState();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateForSaveGroup()
        {
            try
            {
                if (this.sereServ != null)
                {
                    var pttt_forSaveGroup = hisSereServPttt_forSaveGroup.FirstOrDefault(o => o.SERE_SERV_ID == (dicSereServCopy != null && dicSereServCopy.Count > 0 && dicSereServCopy.ContainsKey(sereServ.ID) ? dicSereServCopy[this.sereServ.ID] : this.sereServ.ID));
                    if (pttt_forSaveGroup != null)
                    {
                        pttt_forSaveGroup.MANNER = this.txtMANNER.Text;
                        if (cbbPtttGroup.EditValue != null)
                            pttt_forSaveGroup.PTTT_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cbbPtttGroup.EditValue ?? 0).ToString());
                    }
                    else
                    {
                        HIS_SERE_SERV_PTTT data = new HIS_SERE_SERV_PTTT();
                        data.MANNER = this.txtMANNER.Text;
                        if (cbbPtttGroup.EditValue != null)
                            data.PTTT_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cbbPtttGroup.EditValue ?? 0).ToString());
                        data.SERE_SERV_ID = this.sereServ.ID;
                        hisSereServPttt_forSaveGroup.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdViewService_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                V_HIS_SERE_SERV_5 sereServ = (V_HIS_SERE_SERV_5)grdViewService.GetRow(e.RowHandle);
                if (sereServ != null && sereServ.IS_NO_EXECUTE != null)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDepartment(HisEkipUserADO data)
        {
            try
            {
                if (data == null)
                    return;

                if (data.DEPARTMENT_ID.HasValue && data.DEPARTMENT_ID.Value > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("data.DEPARTMENT_ID.HasValue_____________" + data.DEPARTMENT_ID);
                    //if (!departmentClinic.Select(o => o.ID.ToString()).ToList().Contains(data.DEPARTMENT_ID.ToString()))
                    //{
                    //    Inventec.Common.Logging.LogSystem.Warn("data.DEPARTMENT_ID.HasValue _____________ !CONTAINS");
                    //    data.DEPARTMENT_ID = null;
                    //    data.DEPARTMENT_NAME = "";
                    //}

                    return;
                }
                Inventec.Common.Logging.LogSystem.Warn("data.DEPARTMENT_ID.NULL_____________");
                if (cboDepartment.EditValue != null)
                {
                    data.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());

                }
                else
                {
                    //var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => !String.IsNullOrWhiteSpace(data.LOGINNAME) && o.LOGINNAME.ToLower() == data.LOGINNAME.ToLower());
                    //if (employee != null)
                    //{
                    //    data.DEPARTMENT_ID = employee.DEPARTMENT_ID;
                    //    var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == employee.DEPARTMENT_ID);
                    //    data.DEPARTMENT_NAME = department != null ? department.DEPARTMENT_NAME : "";

                    //}
                    //else
                    //{
                    data.DEPARTMENT_ID = null;
                    data.DEPARTMENT_NAME = "";

                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServLast_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_SERE_SERV sereserv = (HIS_SERE_SERV)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (sereserv != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(sereserv.TDL_INTRUCTION_TIME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewService_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_SERE_SERV_5 data = null;
                if (e.RowHandle > -1)
                {
                    var index = grdViewService.GetDataSourceRowIndex(e.RowHandle);
                    data = (V_HIS_SERE_SERV_5)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "ServiceReqMaTy")
                    {
                        if (data == null || data.IS_NO_EXECUTE != null)
                        {
                            e.RepositoryItem = repositoryItemButtonEditMaty_Disabled;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEditMaty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditMaty_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_SERE_SERV_5 data = (V_HIS_SERE_SERV_5)grdViewService.GetFocusedRow();
                if (data != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisServiceReqMaty").FirstOrDefault();

                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisServiceReqMaty");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data.ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void repositoryItemSearchLookUpEdit1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        //{
        //    try
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn("repositoryItemSearchLookUpEdit1_Closed_________________");
        //        grdViewInformationSurg.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
        //        grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[2];

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        #region  Buttons click
        private void btnServiceReqMaty_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisServiceReqMaty").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisServiceReqMaty");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq.ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(save, "btnSave_Click");
        }
        private void save()
        {
            try
            {
                btnSaveClick(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }
        /// <summary>
        /// Khi Lưu mà thời gian bắt đầu PTTT # thời gian y lệnh thì đưa ra popup cảnh báo: bạn có muốn sửa thời gian y lệnh bằng thời gian bắt đầu PTTT hay không? Có thì lưu, không thì bỏ qua, trả về chưa lưu để họ sửa
        ///Nếu để = 1 thì nếu bn đã kết thúc và khóa VP thì vẫn cho phép sửa thời gian y lệnh của PTTT (đảm bảo > thời gian vào viện và < thời gian ra viện là được)
        /// </summary>
        /// <param name="notShowMess"></param>
        /// <returns></returns>

        private bool btnSaveClick(bool notShowMess)
        {
            bool success = false;
            bool valid = true;
            try
            {

                if (HisConfigCFG.IS_NOT_REQUIRED_PTTT_EXECUTE_ROLE != "1" && sereServ.IS_SENT_EXT != 1 && !CheckCountEkipUser() && !IsReadOnlyGridViewEkipUser)
                {
                    MessageBox.Show(ResourceMessage.VuiLongNhapThongTinkipThucHien, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!CheckAccountWithRole()) return false;


                HisSurgServiceReqUpdateListSDO hisSurgResultSDO = new MOS.SDO.HisSurgServiceReqUpdateListSDO();
                hisSurgResultSDO.SurgUpdateSDOs = new List<SurgUpdateSDO>();

                this.positionHandle = -1;
                ValidateControl();
                InValid();

                valid = valid && dxValidationProvider1.Validate();
                valid = valid && ((this.isAllowEditInfo && this.isStartTimeMustBeGreaterThanInstructionTime) ? this.ValidStartDatePTTT(ref hisSurgResultSDO) : true);
                //valid = valid && dxValidationProvider1.Validate();
                valid = valid && (this.sereServ != null);
                valid = valid && ValidateHisService_MaxTotalProcessTime(notShowMess);
                if (this.lciKetLuan.AppearanceItemCaption.ForeColor == Color.Maroon)
                {
                    if (string.IsNullOrEmpty(txtConclude.Text))
                    {
                        this.dxErrorProviver.SetError(txtConclude, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc), ErrorType.Warning);
                        valid = false;
                    }
                    else
                    {
                        valid = true;
                        this.dxErrorProviver.SetError(txtConclude, "", ErrorType.None);
                    }
                }
                //
                var rs = TypeRequiredEmotionlessMethodOption(this.sereServ);
                if ((rs.RequiredEmotionlessOption == 1 || rs.RequiredEmotionlessOption == 2) && rs.IsServiceTypePT && cbbEmotionlessMethod.EditValue == null)
                {
                    XtraMessageBox.Show("Chưa nhập phương pháp vô cảm", ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbbEmotionlessMethod.ShowPopup();
                    return false;
                }
                else if (rs.RequiredEmotionlessOption == 2 && rs.IsServiceTypeTT && cbbEmotionlessMethod.EditValue == null)
                {
                    if (XtraMessageBox.Show("Bạn chưa nhập phương pháp vô cảm. Bạn có muốn tiếp tục không?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        cbbEmotionlessMethod.ShowPopup();
                        return false;
                    }
                }
                V_HIS_SERVICE currentVHisService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                if (SereServExt != null && currentVHisService != null)
                {
                    if (currentVHisService.ALLOW_SIMULTANEITY != 1 && SereServExt.BEGIN_TIME != null && SereServExt.END_TIME != null)
                    {
                        if (HIS.Desktop.Plugins.SurgServiceReqExecute.Config.HisConfigKeys.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2")
                        {
                            HisSereServCheckExecuteTimesSDO inputSDO = new HisSereServCheckExecuteTimesSDO();
                            var Login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();//serviceReq.EXECUTE_LOGINNAME;
                            long beginTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStart.DateTime) ?? 0;
                            long endTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinish.DateTime) ?? 0;
                            inputSDO.ExecuteTime = new ExecuteTime
                            {
                                BeginTime = beginTime,
                                EndTime = endTime
                            };
                            inputSDO.TreatmentId = serviceReq.TREATMENT_ID;
                            List<string> dsLogin = new List<string> { Login };
                            List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
                            var dataGrid = ucEkip.GetDataSource();
                            if (dataGrid != null && dataGrid.Count() > 0)
                                foreach (var item in dataGrid)
                                {
                                    MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, item);
                                    if (ekipUser != null && ekipUser.EXECUTE_ROLE_ID != 0)
                                        ekipUsers.Add(ekipUser);
                                }
                            List<string> lstLogin = ekipUsers.Select(o => o.LOGINNAME).Distinct().ToList();
                            List<string> lstLoginValid = new List<string>();
                            foreach (string acc in lstLogin)
                            {
                                if (acc != null)
                                {
                                    lstLoginValid.Add(acc);
                                }
                            }
                            if (lstLoginValid.Count == 0)
                            {
                                lstLoginValid.Add(Login);
                            }
                            inputSDO.Loginnames = lstLoginValid;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("_________inputSDO:", inputSDO));
                            string message = "";
                            CommonParam paramCheckEx = new CommonParam();
                            bool suscess = new BackendAdapter(paramCheckEx).Post<bool>("api/HisSereServ/CheckExecuteTimes", ApiConsumers.MosConsumer, inputSDO, paramCheckEx);
                            if (suscess == false)
                            {
                                message = string.Format("{0} Bạn có muốn tiếp tục?", paramCheckEx.GetMessage());
                                if (MessageBox.Show(message, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                { return false; }
                            }
                        }
                    }
                }

                if (chkSaveGroup.Checked)
                {
                    // cập nhật "Cách thức" và “Phân loại” vào ram
                    UpdateForSaveGroup();
                }
                if (valid)
                {
                    if (chkSaveGroup.Checked)
                    {
                        foreach (var item in sereServbyServiceReqs)
                        {
                            this.SereServePTTTInfoStent = null;
                            this.sereServ = item;
                            if (hisSereServPttt != null && hisSereServPttt.Count > 0)
                            {
                                var oldPttt = sereServPTTT;
                                this.sereServPTTT = hisSereServPttt.FirstOrDefault(o => o.SERE_SERV_ID == sereServ.ID);
                                if (sereServPTTT == null)
                                    SereServePTTTInfoStent = oldPttt;
                            }

                            if (ListSereServExt != null && ListSereServExt.Count > 0)
                            {
                                this.SereServExt = ListSereServExt.FirstOrDefault(o => o.SERE_SERV_ID == sereServ.ID);
                            }

                            SurgUpdateSDO singleData = new SurgUpdateSDO();
                            singleData.SereServPttt = new HIS_SERE_SERV_PTTT();
                            singleData.SereServExt = new HIS_SERE_SERV_EXT();
                            singleData.EkipUsers = new List<HIS_EKIP_USER>();
                            if (stentConcludeSave != null && stentConcludeSave.Count > 0)
                            {
                                singleData.StentConcludes = new List<HIS_STENT_CONCLUDE>();
                                stentConcludeSave.ForEach(o =>
                                {
                                    HIS_STENT_CONCLUDE stent = new HIS_STENT_CONCLUDE();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_STENT_CONCLUDE>(stent, o);
                                    stent.SERE_SERV_ID = sereServ.ID;
                                    singleData.StentConcludes.Add(stent);
                                });
                            }
                            var ekipUserCheck = ProcessEkipUser(singleData);
                            if (!ekipUserCheck)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DuLieuEkipTrung, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return success;
                            }

                            ProcessSereServPttt(singleData);
                            //Cập nhật lại các trường Điều tị can thiệp DMV 
                            if (SereServePTTTInfoStent != null)
                            {
                                singleData.SereServPttt.PCI = SereServePTTTInfoStent.PCI;
                                singleData.SereServPttt.STENTING = SereServePTTTInfoStent.STENTING;
                                singleData.SereServPttt.LOCATION_INTERVENTION = SereServePTTTInfoStent.LOCATION_INTERVENTION;
                                singleData.SereServPttt.REASON_INTERVENTION = SereServePTTTInfoStent.REASON_INTERVENTION;
                                singleData.SereServPttt.INTRODUCER = SereServePTTTInfoStent.INTRODUCER;
                                singleData.SereServPttt.GUIDING_CATHETER = SereServePTTTInfoStent.GUIDING_CATHETER;
                                singleData.SereServPttt.GUITE_WIRE = SereServePTTTInfoStent.GUITE_WIRE;
                                singleData.SereServPttt.BALLON = SereServePTTTInfoStent.BALLON;
                                singleData.SereServPttt.STENT = SereServePTTTInfoStent.STENT;
                                singleData.SereServPttt.CONTRAST_AGENT = SereServePTTTInfoStent.CONTRAST_AGENT;
                                singleData.SereServPttt.INSTRUMENTS_OTHER = SereServePTTTInfoStent.INSTRUMENTS_OTHER;
                                singleData.SereServPttt.STENT_NOTE = SereServePTTTInfoStent.STENT_NOTE;
                            }
                            //cập nhật cách thức và “Phân loại” từ ram
                            var currentService = lstService.FirstOrDefault(o => o.ID == this.sereServ.SERVICE_ID);
                            var pttt_forSaveGroup = hisSereServPttt_forSaveGroup.FirstOrDefault(o => o.SERE_SERV_ID == this.sereServ.ID);
                            if (pttt_forSaveGroup != null)
                            {
                                singleData.SereServPttt.MANNER = pttt_forSaveGroup.MANNER;
                                singleData.SereServPttt.PTTT_GROUP_ID = pttt_forSaveGroup.PTTT_GROUP_ID;
                            }
                            else
                            {
                                singleData.SereServPttt.MANNER = this.sereServ.TDL_SERVICE_NAME;
                                singleData.SereServPttt.PTTT_GROUP_ID = currentService != null ? currentService.PTTT_GROUP_ID : null;
                            }

                            ProcessSereServExt(singleData);

                            if (this.listSesePtttMetod != null && this.listSesePtttMetod.Count > 0)
                            {
                                var lst = this.listSesePtttMetod.Where(o => o.SERE_SERV_ID == singleData.SereServPttt.SERE_SERV_ID).ToList();
                                if (lst != null && lst.Count > 0)
                                {
                                    List<HisSesePtttMethodSDO> listPtttMethodSDO = new List<HisSesePtttMethodSDO>();

                                    for (int x = 0; x < lst.Count; x++)
                                    {
                                        HisSesePtttMethodSDO ptttMethodSDO = new HisSesePtttMethodSDO();
                                        if (lst[x].EkipUsersADO != null && lst[x].EkipUsersADO.listEkipUser != null && lst[x].EkipUsersADO.listEkipUser.Count > 0)
                                        {
                                            ptttMethodSDO.EkipUsers = ProcessEkipUserOther(lst[x].EkipUsersADO.listEkipUser);
                                        }
                                        HIS_SESE_PTTT_METHOD mt = new HIS_SESE_PTTT_METHOD();
                                        mt.PTTT_METHOD_ID = lst[x].ID;
                                        mt.AMOUNT = lst[x].AMOUNT;
                                        if (lst[x].PTTT_GROUP_ID == null && !string.IsNullOrEmpty(lst[x].PTTT_GROUP_NAME))
                                        {
                                            mt.PTTT_GROUP_ID = Int64.Parse(lst[x].PTTT_GROUP_NAME.ToString());
                                        }
                                        else
                                        {
                                            mt.PTTT_GROUP_ID = lst[x].PTTT_GROUP_ID;
                                        }
                                        mt.EKIP_ID = lst[x].EKIP_ID;
                                        ptttMethodSDO.HisSesePtttMethod = mt;
                                        listPtttMethodSDO.Add(ptttMethodSDO);
                                    }

                                    singleData.SesePtttMethos = listPtttMethodSDO;
                                }
                            }

                            //var sereServOld = this.sereServbyServiceReqs.Where(o => o.SERVICE_ID == this.sereServ.SERVICE_ID);
                            //if (sereServOld != null)
                            //    singleData.SereServId = sereServOld.FirstOrDefault().ID;
                            //else
                            singleData.SereServId = this.sereServ.ID;

                            hisSurgResultSDO.SurgUpdateSDOs.Add(singleData);
                        }

                        if (HisConfigKeys.allowFinishWhenAccountIsDoctor == "1" && BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault().IS_DOCTOR != 1)
                        {
                            hisSurgResultSDO.IsFinished = false;
                        }
                        else
                        {
                            if (dtFinish.EditValue != null && chkKetThuc.Checked)
                            {
                                hisSurgResultSDO.IsFinished = true;
                            }
                        }

                        SaveSurgServiceReq(hisSurgResultSDO, ref success, notShowMess);
                    }
                    else
                    {
                        HisSurgServiceReqUpdateSDO sdo = new HisSurgServiceReqUpdateSDO();
                        SurgUpdateSDO singleData = new SurgUpdateSDO();
                        singleData.SereServPttt = new HIS_SERE_SERV_PTTT();
                        singleData.SereServExt = new HIS_SERE_SERV_EXT();
                        singleData.EkipUsers = new List<HIS_EKIP_USER>();
                        if (stentConcludeSave != null && stentConcludeSave.Count > 0)
                            singleData.StentConcludes = stentConcludeSave.Where(o => o.SERE_SERV_ID == sereServ.ID).ToList();
                        var ekipUserCheck = ProcessEkipUser(singleData);
                        if (!ekipUserCheck)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DuLieuEkipTrung, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return success;
                        }

                        ProcessSereServPttt(singleData);
                        ProcessSereServExt(singleData);
                        //var sereServOld = this.sereServbyServiceReqs.Where(o => o.SERVICE_ID == this.sereServLast.SERVICE_ID);
                        //if (sereServOld != null)
                        //    sdo.SereServId = sereServOld.FirstOrDefault().ID;
                        //else
                        sdo.SereServId = this.sereServ.ID;

                        if (singleData.EkipUsers != null)
                            sdo.EkipUsers = singleData.EkipUsers;
                        if (singleData.EyeSurgryDesc != null)
                            sdo.EyeSurgryDesc = singleData.EyeSurgryDesc;
                        if (singleData.SereServExt != null)
                            sdo.SereServExt = singleData.SereServExt;
                        if (singleData.SereServPttt != null)
                            sdo.SereServPttt = singleData.SereServPttt;
                        if (singleData.SkinSurgeryDesc != null)
                            sdo.SkinSurgeryDesc = singleData.SkinSurgeryDesc;
                        if (singleData.StentConcludes != null)
                            sdo.StentConcludes = singleData.StentConcludes;
                        if (this.listSesePtttMetod != null && this.listSesePtttMetod.Count > 0)
                        {
                            var lst = this.listSesePtttMetod.Where(o => o.SERE_SERV_ID == sdo.SereServPttt.SERE_SERV_ID).ToList();
                            if (lst != null && lst.Count > 0)
                            {
                                List<HisSesePtttMethodSDO> listPtttMethodSDO = new List<HisSesePtttMethodSDO>();

                                for (int x = 0; x < lst.Count; x++)
                                {
                                    HisSesePtttMethodSDO ptttMethodSDO = new HisSesePtttMethodSDO();
                                    if (lst[x].EkipUsersADO != null && lst[x].EkipUsersADO.listEkipUser != null && lst[x].EkipUsersADO.listEkipUser.Count > 0)
                                    {
                                        ptttMethodSDO.EkipUsers = ProcessEkipUserOther(lst[x].EkipUsersADO.listEkipUser);
                                    }
                                    HIS_SESE_PTTT_METHOD mt = new HIS_SESE_PTTT_METHOD();
                                    mt.PTTT_METHOD_ID = lst[x].ID;
                                    mt.AMOUNT = lst[x].AMOUNT;
                                    if (lst[x].PTTT_GROUP_ID == null && !string.IsNullOrEmpty(lst[x].PTTT_GROUP_NAME))
                                    {
                                        mt.PTTT_GROUP_ID = Int64.Parse(lst[x].PTTT_GROUP_NAME.ToString());
                                    }
                                    else
                                    {
                                        mt.PTTT_GROUP_ID = lst[x].PTTT_GROUP_ID;
                                    }
                                    mt.EKIP_ID = lst[x].EKIP_ID;
                                    ptttMethodSDO.HisSesePtttMethod = mt;
                                    listPtttMethodSDO.Add(ptttMethodSDO);
                                }

                                sdo.SesePtttMethos = listPtttMethodSDO;
                            }
                        }

                        sdo.UpdateInstructionTimeByStartTime = hisSurgResultSDO.UpdateInstructionTimeByStartTime;

                        if (dtFinish.EditValue != null && chkKetThuc.Checked)
                            sdo.IsFinished = true;

                        SaveSurgServiceReq(sdo, ref success, notShowMess);
                    }

                    if (success)
                    {

                        SetEnableControl();
                        if (success && dtFinish.EditValue != null && chkKetThuc.Checked && chkClose.Checked)
                        {
                            XtraTabControl main = SessionManager.GetTabControlMain();
                            XtraTabPage page = main.TabPages[GlobalVariables.SelectedTabPageIndex];
                            TabControlBaseProcess.CloseCurrentTabPage(page, main);
                        }
                    }
                }
                else
                {
                    string warning = "";
                    CommonParam param = new CommonParam();
                    if (!valid)
                    {
                        if (this.ModuleControls == null || this.ModuleControls.Count == 0)
                        {
                            ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                            this.ModuleControls = controlProcess.GetControls(this);
                        }

                        GetMessageErrorControlInvalidProcess getMessageErrorControlInvalidProcess = new Utility.GetMessageErrorControlInvalidProcess();
                        getMessageErrorControlInvalidProcess.Run(this, this.dxValidationProvider1, this.ModuleControls, param);
                        warning = param.GetMessage();
                    }

                    if (!String.IsNullOrEmpty(warning))
                    {
                        MessageBox.Show(warning, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                if (HisConfigKeys.allowFinishWhenAccountIsDoctor == "1" && BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault().IS_DOCTOR != 1 && dtFinish.EditValue != null && chkKetThuc.Checked)
                    MessageBox.Show(ResourceMessage.BanKhongPhaiLaBacSyKhongDuocKetThuc, ResourceMessage.ThongBao);
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
            return success;
        }

        private bool ValidateHisService_MaxTotalProcessTime(bool notShowMess)
        {
            bool valid = true;
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("ValidateHisService_MaxTotalProcessTime()");
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME", Config.HisConfigCFG.PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME));
                if (Config.HisConfigCFG.PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME != "1"
                    && Config.HisConfigCFG.PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME != "2")
                    return valid;
                var intructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.serviceReq.INTRUCTION_TIME);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("intructionTime", intructionTime));
                if (intructionTime == null)
                    return valid;
                var intructionTime_ToMinute = intructionTime.Value.AddSeconds(-intructionTime.Value.Second);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("intructionTime_ToMinute", intructionTime_ToMinute));
                TimeSpan processTime = (dtFinish.DateTime - intructionTime_ToMinute);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("processTime", processTime));
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("processTime.TotalMinutes", processTime.TotalMinutes));
                Dictionary<long, List<V_HIS_SERVICE>> dicInvalidServices = new Dictionary<long, List<V_HIS_SERVICE>>();
                if (chkSaveGroup.Checked)
                {
                    if (this.sereServbyServiceReqs == null)
                        return valid;
                    foreach (var item in this.sereServbyServiceReqs)
                    {
                        var service = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("service.MAX_TOTAL_PROCESS_TIME", service!= null ? service.MAX_TOTAL_PROCESS_TIME : -1));
                        if (service != null && service.MAX_TOTAL_PROCESS_TIME.HasValue && service.MAX_TOTAL_PROCESS_TIME.Value > 0
                            && processTime.TotalMinutes > service.MAX_TOTAL_PROCESS_TIME.Value && (string.IsNullOrEmpty(service.TOTAL_TIME_EXCEPT_PATY_IDS) || !("," + service.TOTAL_TIME_EXCEPT_PATY_IDS + ",").Contains("," + item.PATIENT_TYPE_ID.ToString() + ",")))
                        {
                            if (dicInvalidServices.ContainsKey(service.MAX_TOTAL_PROCESS_TIME.Value))
                            {
                                if (!dicInvalidServices[service.MAX_TOTAL_PROCESS_TIME.Value].Contains(service))
                                {
                                    dicInvalidServices[service.MAX_TOTAL_PROCESS_TIME.Value].Add(service);
                                }
                            }
                            else
                            {
                                dicInvalidServices.Add(service.MAX_TOTAL_PROCESS_TIME.Value, new List<V_HIS_SERVICE>() { service });
                            }
                        }
                    }
                }
                else
                {
                    var service = lstService.FirstOrDefault(o => o.ID == this.sereServ.SERVICE_ID);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("service.MAX_TOTAL_PROCESS_TIME", service != null ? service.MAX_TOTAL_PROCESS_TIME : -1));
                    if (service != null && service.MAX_TOTAL_PROCESS_TIME.HasValue && service.MAX_TOTAL_PROCESS_TIME.Value > 0
                            && processTime.TotalMinutes > service.MAX_TOTAL_PROCESS_TIME.Value && (string.IsNullOrEmpty(service.TOTAL_TIME_EXCEPT_PATY_IDS) || !("," + service.TOTAL_TIME_EXCEPT_PATY_IDS + ",").Contains("," + sereServ.PATIENT_TYPE_ID.ToString() + ",")))
                    {
                        dicInvalidServices.Add(service.MAX_TOTAL_PROCESS_TIME.Value, new List<V_HIS_SERVICE>() { service });
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dicInvalidServices", dicInvalidServices));
                if (dicInvalidServices.Count > 0)
                {
                    string message = "";
                    List<string> listmsg = new List<string>();
                    if (Config.HisConfigCFG.PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME == "1")
                    {
                        foreach (var item in dicInvalidServices)
                        {
                            string msg = String.Format(ResourceMessage.KhongChoPhepTraKetQuaDichVu_Sau_PhutTinhTuThoiDiemRaYLenh
                                                        , String.Join(", ", item.Value.Select(o => o.SERVICE_NAME))
                                                        , item.Key.ToString()
                                                        , Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.serviceReq.INTRUCTION_TIME));
                            listmsg.Add(msg);
                        }
                        message = String.Join("; ", listmsg) + ".";
                        if (notShowMess)
                        {
                            Inventec.Common.Logging.LogSystem.Info(message);
                            return true;
                        }
                        valid = false;
                        XtraMessageBox.Show(message, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (Config.HisConfigCFG.PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME == "2")
                    {
                        foreach (var item in dicInvalidServices)
                        {
                            string msg = String.Format(ResourceMessage.TraKetQuaDichVu_VuotQua_PhutTinhTuThoiDiemRaYLenh
                                                        , String.Join(", ", item.Value.Select(o => o.SERVICE_NAME))
                                                        , item.Key.ToString()
                                                        , Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.serviceReq.INTRUCTION_TIME));
                            listmsg.Add(msg);
                        }
                        message = String.Join("; ", listmsg) + ".";
                        if (notShowMess)
                        {
                            Inventec.Common.Logging.LogSystem.Info(message);
                            return true;
                        }
                        if (XtraMessageBox.Show(String.Format("{0} {1}", message, ResourceMessage.BanCoMuonTiepTucKhong), ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            valid = false;
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

        private bool CheckCountEkipUser()
        {
            bool result = true;
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
                var dataGrid = ucEkip.GetDataSource();
                if (dataGrid != null && dataGrid.Count() > 0)
                    foreach (var item in dataGrid)
                    {
                        MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, item);
                        if (ekipUser != null && ekipUser.EXECUTE_ROLE_ID != 0)
                            ekipUsers.Add(ekipUser);
                    }
                if (ekipUsers.Count == 0)
                    result = false;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnDepartmentTran_Click(object sender, EventArgs e)
        {
            departmentTran();
        }

        private void departmentTran()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransDepartment").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TransDepartment");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    TransDepartmentADO transDepartment = new TransDepartmentADO(serviceReq.TREATMENT_ID);
                    transDepartment.DepartmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId;
                    listArgs.Add(transDepartment);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {

            LogTheadInSessionInfo(finishClick, "btnFinish_Click");
        }

        private void finishClick()
        {
            try
            {
                if (HisConfigCFG.IS_NOT_REQUIRED_PTTT_EXECUTE_ROLE != "1" && sereServ.IS_SENT_EXT != 1 && !CheckCountEkipUser())
                {
                    MessageBox.Show(ResourceMessage.VuiLongNhapThongTinkipThucHien, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                CommonParam param = new CommonParam();
                bool success = false;
                bool valid = true;
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate() && CheckExistFinishTime())
                    return;
                string serviceCode = "";
                //dtFinish
                if (dtFinish.EditValue == null || string.IsNullOrEmpty(dtFinish.Text) || string.IsNullOrWhiteSpace(dtFinish.Text))
                {
                    //CheckLessTime(ref serviceCode);

                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DichVuKhongCoThoiGianKetThucKhongChoKetThucXuLy, this.sereServ.TDL_SERVICE_CODE));
                    return;
                }
                IsActionOtherButton = true;
                if (!btnSaveClick(true))
                {
                    return;
                }

                if (CheckLessTime(ref serviceCode))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DichVuChuaThucHienKhongChoKetThucXuLy, serviceCode));
                    return;
                }

                if (valid)
                {
                    if (serviceReq != null)
                    {
                        HIS_SERVICE_REQ serviceReqFinish = new HIS_SERVICE_REQ();
                        serviceReqFinish.ID = serviceReq.ID;

                        if (this.sereServbyServiceReqs != null && sereServbyServiceReqs.Count() > 0)
                        {
                            var SereServExts_ByServiceReq = GetListSereServExt_BySereServIds(this.sereServbyServiceReqs.Select(o => o.ID).ToList()) ?? new List<HIS_SERE_SERV_EXT>();
                            serviceReqFinish.START_TIME = SereServExts_ByServiceReq.Min(o => o.BEGIN_TIME);
                            serviceReqFinish.FINISH_TIME = SereServExts_ByServiceReq.Max(o => o.END_TIME);
                        }
                        else
                        {
                            if (dtFinish.EditValue != null)
                                serviceReqFinish.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinish.DateTime);
                            //if (dtStart.EditValue != null)
                            //    serviceReqFinish.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStart.DateTime);
                        }

                        var result = new BackendAdapter(param)
                        .Post<HIS_SERVICE_REQ>("api/HisServiceReq/FinishWithTime", ApiConsumers.MosConsumer, serviceReqFinish, param);
                        if (result != null)
                        {
                            success = true;
                            EnableButtonByServiceReqSTT(result.SERVICE_REQ_STT_ID);
                            if (result.FINISH_TIME.HasValue)
                                dtFinish.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(result.FINISH_TIME.Value) ?? DateTime.Now;

                            if (serviceReq != null)
                            {
                                serviceReq.FINISH_TIME = result.FINISH_TIME;
                            }

                            SuccessLog(serviceReq);
                            btnFinish.Enabled = false;
                        }
                    }
                }

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckAccountWithRole()
        {
            bool result = true;
            List<string> mess = new List<string>();
            try
            {
                var dataGrid = ucEkip.GetDataSource();

                if (dataGrid != null && dataGrid.Count() > 0)
                {
                    var executeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                    var lstRole = dataGrid.Select(o => o.EXECUTE_ROLE_ID).Distinct().ToList();
                    foreach (var item in lstRole)
                    {
                        var role = executeRole.FirstOrDefault(o => o.ID == item);
                        var users = dataGrid.Where(o => o.EXECUTE_ROLE_ID == item);
                        if (role != null && role.IS_SINGLE_IN_EKIP == 1 && users != null && users.Count() > 1)
                        {
                            mess.Add(role.EXECUTE_ROLE_NAME);
                        }
                    }
                    if (mess.Count() > 0)
                    {
                        result = false;
                        MessageBox.Show(String.Format("Không được phép nhập nhiều hơn 1 tài khoản đối với vai trò {0}", string.Join(",", mess), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning));
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

        private List<HIS_SERE_SERV_EXT> GetListSereServExt_BySereServIds(List<long> listSereServIds)
        {
            List<HIS_SERE_SERV_EXT> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServExtFilter ssExtFilter = new HisSereServExtFilter();
                ssExtFilter.SERE_SERV_IDs = listSereServIds;
                result = new BackendAdapter(param)
                        .Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, ssExtFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnAssignOutKip_Click(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager manager = new Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager();
                if (manager.Run(this.serviceReq.TREATMENT_ID, this.vhisTreatment.TDL_PATIENT_TYPE_ID ?? 0, this.Module.RoomId))
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>();
                        V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>(sereServ);

                        AssignServiceADO assignServiceADO = new AssignServiceADO(serviceReq.TREATMENT_ID, intructionTime, serviceReq.ID, sereServInput, LoadSereServOutKipResult, false);
                        assignServiceADO.GenderName = serviceReq.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.PatientName = serviceReq.TDL_PATIENT_NAME;
                        assignServiceADO.PatientDob = serviceReq.TDL_PATIENT_DOB;
                        assignServiceADO.IsAssignInPttt = true;

                        listArgs.Add(assignServiceADO);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServOutKipResult(object data)
        {
            try
            {
                if (data != null)
                {
                    LoadAfterAssign();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnKTDT_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentFinish").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentFinish");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    TreatmentLogADO treatmentLog = new TreatmentLogADO();
                    treatmentLog.RoomId = this.Module.RoomId;
                    treatmentLog.TreatmentId = serviceReq.TREATMENT_ID;
                    listArgs.Add(treatmentLog);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignBlood_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAssignBlood").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisAssignBlood");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AssignBloodADO assignBlood = new AssignBloodADO(serviceReq.TREATMENT_ID, intructionTime, serviceReq.ID);
                    moduleData.RoomId = this.Module.RoomId;
                    moduleData.RoomTypeId = this.Module.RoomTypeId;
                    listArgs.Add(assignBlood);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssInKip_Click(object sender, EventArgs e)
        {
            try
            {
                //var ekipUsers = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                var ekipUsers = ucEkip.GetDataSource();
                ekipUsers.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                ekipUsers.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                bool hasInvalid = ekipUsers
                   .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                       || o.EXECUTE_ROLE_ID <= 0).FirstOrDefault() != null ? true : false;
                if (hasInvalid)
                {
                    MessageBox.Show(ResourceMessage.VuiLongNhapThongTinkipThucHien, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                IsActionOtherButton = true;
                if (!btnSaveClick(true))
                    return;

                HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager manager = new Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager();
                if (manager.Run(this.serviceReq.TREATMENT_ID, this.vhisTreatment.TDL_PATIENT_TYPE_ID ?? 0, this.Module.RoomId))
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>();
                        V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>(sereServ);

                        AssignServiceADO assignServiceADO = new AssignServiceADO(serviceReq.TREATMENT_ID, intructionTime, serviceReq.ID, sereServInput, LoadSereServInKipResult, true);
                        assignServiceADO.GenderName = serviceReq.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.PatientName = serviceReq.TDL_PATIENT_NAME;
                        assignServiceADO.PatientDob = serviceReq.TDL_PATIENT_DOB;
                        assignServiceADO.IsAssignInPttt = true;

                        listArgs.Add(assignServiceADO);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignPre_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>();
                    V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>(sereServ);

                    AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(serviceReq.TREATMENT_ID, intructionTime, this.serviceReq.ID, sereServInput, LoadSereServOutKipResult);
                    assignPrescription.GenderName = serviceReq.TDL_PATIENT_GENDER_NAME;
                    assignPrescription.PatientName = serviceReq.TDL_PATIENT_NAME;
                    assignPrescription.PatientDob = serviceReq.TDL_PATIENT_DOB;
                    assignPrescription.PatientId = serviceReq.TDL_PATIENT_ID;
                    //assignPrescription.IsAutoCheckExpend = true;
                    // assignPrescription.IsExecutePTTT = true;

                    //xuandv new
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = this.serviceReq.TREATMENT_ID;

                    var rsApi = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    //if (rsApi != null && rsApi.Count > 0 && rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    //{
                    //    assignPrescription.IsExecutePTTT = true;
                    //    assignPrescription.IsAutoCheckExpend = true;
                    //}
                    if (rsApi != null && rsApi.Count > 0)
                    {
                        if (rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                            || rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU
                            || (HisConfigCFG.PrescriptionTypeOption == "1" && rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY))
                        {
                            assignPrescription.IsExecutePTTT = true;
                            assignPrescription.IsAutoCheckExpend = true;
                        }
                    }

                    listArgs.Add(assignPrescription);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnKTDT_Click_1(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentFinish").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentFinish");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    TreatmentLogADO treatmentLog = new TreatmentLogADO();
                    treatmentLog.RoomId = this.Module.RoomId;
                    treatmentLog.TreatmentId = serviceReq.TREATMENT_ID;
                    listArgs.Add(treatmentLog);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignBlood_Click_1(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAssignBlood").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisAssignBlood");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>();
                    V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>(sereServ);

                    AssignBloodADO assignBlood = new AssignBloodADO(serviceReq.TREATMENT_ID, intructionTime, 0, sereServInput, LoadSereServOutKipResult);
                    assignBlood.GenderName = serviceReq.TDL_PATIENT_GENDER_NAME;
                    assignBlood.PatientName = serviceReq.TDL_PATIENT_NAME;
                    assignBlood.PatientDob = serviceReq.TDL_PATIENT_DOB;

                    listArgs.Add(assignBlood);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
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
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>();
                    V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>(sereServ);

                    AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(serviceReq.TREATMENT_ID, intructionTime, this.serviceReq.ID, sereServInput, LoadSereServOutKipResult);
                    assignPrescription.GenderName = serviceReq.TDL_PATIENT_GENDER_NAME;
                    assignPrescription.PatientName = serviceReq.TDL_PATIENT_NAME;
                    assignPrescription.PatientDob = serviceReq.TDL_PATIENT_DOB;
                    assignPrescription.IsCabinet = true;
                    assignPrescription.IsAutoCheckExpend = true;

                    listArgs.Add(assignPrescription);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSwapService_Click_1(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SwapService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SwapService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>();
                    V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>(sereServ);
                    SwapServiceADO swapService = new SwapServiceADO(serviceReq, sereServInput);
                    swapService.delegateSwapService = SwapSericeResult;
                    listArgs.Add(swapService);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveEkipTemp_Click(object sender, EventArgs e)
        {
            try
            {
                var ekipUsers = ucEkip.GetDataSource();
                //= grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                ekipUsers.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                ekipUsers.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                bool hasInvalid = ekipUsers
                   .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                       || o.EXECUTE_ROLE_ID <= 0).FirstOrDefault() != null ? true : false;
                if (hasInvalid)
                {
                    MessageBox.Show(ResourceMessage.VuiLongNhapThongTinkipThucHien);
                    return;
                }

                var groupLoginname = ekipUsers.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME)).GroupBy(o => o.LOGINNAME).ToList();
                if (groupLoginname != null && groupLoginname.Count > 0)
                {
                    List<string> messError = new List<string>();
                    foreach (var item in groupLoginname)
                    {
                        if (item.Count() > 1)
                        {
                            var lstExeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => item.Select(s => s.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();
                            messError.Add(string.Format("Tài khoản {0} được thiết lập với các vai trò {1}", item.Key, string.Join(",", lstExeRole.Select(s => s.EXECUTE_ROLE_NAME))));
                        }
                    }

                    if (messError.Count > 0)
                    {
                        XtraMessageBox.Show(string.Join("\n", messError), ResourceMessage.ThongBao);
                        return;
                    }
                }

                frmEkipTemp frm = new frmEkipTemp(ekipUsers, RefeshDataEkipTemp, this.Module);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTrackingCreate_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listObj = new List<object>();
                listObj.Add(this.serviceReq.TREATMENT_ID);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTrackingList", this.Module.RoomId, this.Module.RoomTypeId, listObj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOther_Click(object sender, EventArgs e)
        {
            try
            {
                btnOther.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ddbPhatSinh_Click(object sender, EventArgs e)
        {
            try
            {
                ddbPhatSinh.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnKhongPhatSinh_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();

                    AssignServiceADO AssignServiceADO = new AssignServiceADO(serviceReq.TREATMENT_ID, 0, 0);
                    AssignServiceADO.TreatmentId = serviceReq.TREATMENT_ID;
                    AssignServiceADO.PatientDob = serviceReq.TDL_PATIENT_DOB;
                    AssignServiceADO.PatientName = serviceReq.TDL_PATIENT_NAME;
                    AssignServiceADO.GenderName = serviceReq.TDL_PATIENT_GENDER_NAME;
                    AssignServiceADO.IsAutoEnableEmergency = true;
                    listArgs.Add(AssignServiceADO);

                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(printClick, "btnPrint_Click");
        }
        private bool IsActionPrint = false;
        private bool IsActionOtherButton = false;
        private void printClick()
        {
            try
            {
                btnPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins");
                    HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtIcdExtraCode.Text, txtIcdText.Text);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(secondaryIcdADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                    ((Form)extenceInstance).Show(this);
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
                if (!string.IsNullOrEmpty(delegateIcdCodes))
                {
                    txtIcdExtraCode.Text = delegateIcdCodes;
                }

                if (!string.IsNullOrEmpty(delegateIcdNames))
                {
                    txtIcdText.Text = delegateIcdNames;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void update(HIS_ICD dataIcd)
        {
            txtIcdText.Text = txtIcdText.Text + dataIcd.ICD_CODE + " - " + dataIcd.ICD_NAME + ", ";
        }

        private void stringIcds(string delegateIcds)
        {
            if (!string.IsNullOrEmpty(delegateIcds))
            {
                txtIcdText.Text = delegateIcds;
            }
        }

        private void txtIcdText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCmCode.Focus();
                    txtIcdCmCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdText.Focus();
                    txtIcdText.SelectAll();
                }
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

        private void txtIcdExtraCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();
                string strIcdNames = "";
                if (!String.IsNullOrEmpty(currentValue))
                {
                    string seperate = ";";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    List<string> arrWrongCodes = new List<string>();
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
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format(ResourceMessage.KhongTimThayICDTuongUng, strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = txtIcdExtraCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = txtIcdExtraCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            txtIcdExtraCode.Focus();
                            txtIcdExtraCode.Select(startPositionWarm, lenghtPositionWarm);
                        }
                    }
                }
                SetCheckedIcdsToControl(txtIcdExtraCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCmCode.Focus();
                    txtIcdCmCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = processIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdExtraCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckedIcdCmsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdCmSubName.Text == txtIcdCmSubName.Properties.NullValuePrompt ? "" : txtIcdCmSubName.Text);
                txtIcdCmSubName.Text = processIcdCmNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdCmSubName.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdCmSubCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string processIcdNameChanged(string oldIcdNames, string newIcdNames)
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
                                var checkInList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o =>
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

        private string processIcdCmNameChanged(string oldIcdNames, string newIcdNames)
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
                                var checkInList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD_CM>().Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_CM_NAME))).FirstOrDefault();
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        private void LoadSereServInKipResult(object data)
        {
            try
            {
                if (data != null)
                {
                    if (sereServInEkips == null)
                        sereServInEkips = new List<V_HIS_SERE_SERV_5>();
                    if (sereServInPackages == null)
                        sereServInPackages = new List<V_HIS_SERE_SERV_5>();
                    if (data.GetType() == typeof(MOS.SDO.HisServiceReqListResultSDO))
                    {
                        MOS.SDO.HisServiceReqListResultSDO serviceReqResultSDO = data as MOS.SDO.HisServiceReqListResultSDO;
                        if (serviceReqResultSDO.SereServs != null)
                        {
                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, V_HIS_SERE_SERV_5>();
                            List<V_HIS_SERE_SERV_5> sereServ5s = AutoMapper.Mapper.Map<List<V_HIS_SERE_SERV>, List<V_HIS_SERE_SERV_5>>(serviceReqResultSDO.SereServs);
                            sereServInEkips.AddRange(sereServ5s);
                            sereServInPackages.AddRange(sereServ5s);
                        }
                    }

                    if (gridControlServServInEkip.DataSource != null)
                        gridControlServServInEkip.RefreshDataSource();
                    else
                        gridControlServServInEkip.DataSource = sereServInEkips;

                    if (gridControlSereServAttach.DataSource != null)
                        gridControlSereServAttach.RefreshDataSource();
                    else
                        gridControlSereServAttach.DataSource = sereServInPackages;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SwapSericeResult(object data)
        {
            try
            {
                HIS_SERE_SERV sereServResult = data as HIS_SERE_SERV;
                if (sereServResult != null)
                {
                    foreach (var sereServbyServiceReq in sereServbyServiceReqs)
                    {
                        if (sereServbyServiceReq.ID == sereServ.ID)
                        {
                            sereServbyServiceReq.IS_NO_EXECUTE = 1;
                        }
                    }

                    V_HIS_SERE_SERV_5 sereServ5 = new V_HIS_SERE_SERV_5();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_5>(sereServ5, sereServResult);
                    var service = lstService.FirstOrDefault(o => o.ID == sereServResult.SERVICE_ID);
                    if (service != null)
                    {
                        sereServ5.TDL_SERVICE_CODE = service.SERVICE_CODE;
                        sereServ5.TDL_SERVICE_NAME = service.SERVICE_NAME;
                        sereServ5.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                        sereServ5.TDL_SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                    }

                    sereServbyServiceReqs.Insert(0, sereServ5);
                    grdControlService.RefreshDataSource();
                    this.sereServ = sereServbyServiceReqs[0];
                    LoadSereServExt();
                    GetSereServPtttBySereServId();
                    FillDataDefaultToControl();
                    SetDefaultCboPTTTGroup(sereServ);
                    SetDefaultCboICD9CmChinh(sereServ);
                    LoadDetailSereServPttt();
                    SetEnableControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void cbo_UseName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        //{
        //    try
        //    {
        //        grdViewInformationSurg.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
        //        grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[2];
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void dtFinish_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtFinish.EditValue != null && dtFinish.DateTime != DateTime.MinValue && dtStart.EditValue != null && dtStart.DateTime != DateTime.MinValue)
                        spinExcuteTimeAdd.EditValue = (dtFinish.DateTime - dtStart.DateTime).TotalMinutes;
                    dtFinish.Properties.Buttons[1].Visible = true;
                    txtMANNER.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtStart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtFinish.Focus();
                    dtFinish.SelectAll();
                    dtFinish.ShowPopup();
                    dtStart.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtStart_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dtStart.Properties.Buttons[1].Visible = false;
                    dtStart.EditValue = null;
                    dtStart.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtFinish_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dtFinish.Properties.Buttons[1].Visible = false;
                    dtFinish.EditValue = null;
                    dtFinish.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataEkipTemp()
        {
            try
            {
                ComboEkipTemp(cboEkipTemp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipTemp_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEkipTemp.EditValue != null)
                    {
                        var data = this.ekipTemps.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboEkipTemp.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            ucEkip.SetDepartmentID(Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue != null ? cboDepartment.EditValue.ToString() : "0"));
                            ucEkip.LoadGridEkipUserFromTemp(data.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipTemp_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEkipTemp.Properties.Buttons[1].Visible = false;
                    cboEkipTemp.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtFinish_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal && dtFinish.EditValue != null && dtFinish.DateTime != DateTime.MinValue)
                {
                    DateTime dt = dtFinish.DateTime;
                    dtFinish.DateTime = new DateTime(dt.Year, dt.Month, dt.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                    dtFinish.Properties.Buttons[1].Visible = true;
                    if (dtFinish.EditValue != null && dtFinish.DateTime != DateTime.MinValue && dtStart.EditValue != null && dtStart.DateTime != DateTime.MinValue)
                        spinExcuteTimeAdd.EditValue = (dtFinish.DateTime - dtStart.DateTime).TotalMinutes;
                    txtMANNER.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtStart_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (dtStart.EditValue != null)
                {
                    DateTime dt = dtStart.DateTime;
                    dtStart.DateTime = new DateTime(dt.Year, dt.Month, dt.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                    dtStart.Properties.Buttons[1].Visible = true;
                    dtFinish.Focus();
                    dtFinish.SelectAll();
                    dtFinish.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dropDownButtonGPBL_Click(object sender, EventArgs e)
        {
            try
            {
                dropDownButtonGPBL.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null)
                    {

                        ucEkip.SetDepartmentID(Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue != null ? cboDepartment.EditValue.ToString() : "0"));
                        ucEkip.FillDataToGridDepartment();

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChonMauPTCHuyenKhoaMat_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.sereServPTTT != null)
                {
                    this.sereServPTTT_Detail = this.sereServPTTT;
                }
                if (this.sereServPTTT_Detail == null)
                    this.sereServPTTT_Detail = new V_HIS_SERE_SERV_PTTT();

                frmInputDetail frmInputDetail = new frmInputDetail(this.sereServ, currentEyeSurgDesc, stentConcludeSave, GetEyeSurgryDescLast, SkinSurgeryDes, GetSkinSurg, this.sereServPTTT_Detail, GetSereServPTTT, GetDmv);
                frmInputDetail.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDmv(List<HIS_STENT_CONCLUDE> obj)
        {
            try
            {
                stentConcludeSave = obj;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => stentConcludeSave), stentConcludeSave));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        V_HIS_SERE_SERV_PTTT sereServPTTT_Detail = new V_HIS_SERE_SERV_PTTT();
        private void GetSereServPTTT(V_HIS_SERE_SERV_PTTT data)
        {
            try
            {
                this.sereServPTTT_Detail = data ?? new V_HIS_SERE_SERV_PTTT();
                if (this.sereServPTTT == null)
                    this.sereServPTTT = new V_HIS_SERE_SERV_PTTT();
                this.sereServPTTT.DRAINAGE = sereServPTTT_Detail.DRAINAGE;
                this.sereServPTTT.WICK = sereServPTTT_Detail.WICK;
                this.sereServPTTT.DRAW_DATE = sereServPTTT_Detail.DRAW_DATE;
                this.sereServPTTT.CUT_SEWING_DATE = sereServPTTT_Detail.CUT_SEWING_DATE;
                this.sereServPTTT.OTHER = sereServPTTT_Detail.OTHER;

                this.sereServPTTT.PCI = sereServPTTT_Detail.PCI;
                this.sereServPTTT.STENTING = sereServPTTT_Detail.STENTING;
                this.sereServPTTT.LOCATION_INTERVENTION = sereServPTTT_Detail.LOCATION_INTERVENTION;
                this.sereServPTTT.REASON_INTERVENTION = sereServPTTT_Detail.REASON_INTERVENTION;
                this.sereServPTTT.INTRODUCER = sereServPTTT_Detail.INTRODUCER;
                this.sereServPTTT.GUIDING_CATHETER = sereServPTTT_Detail.GUIDING_CATHETER;
                this.sereServPTTT.GUITE_WIRE = sereServPTTT_Detail.GUITE_WIRE;
                this.sereServPTTT.BALLON = sereServPTTT_Detail.BALLON;
                this.sereServPTTT.STENT = sereServPTTT_Detail.STENT;
                this.sereServPTTT.CONTRAST_AGENT = sereServPTTT_Detail.CONTRAST_AGENT;
                this.sereServPTTT.INSTRUMENTS_OTHER = sereServPTTT_Detail.INSTRUMENTS_OTHER;
                this.sereServPTTT.STENT_NOTE = sereServPTTT_Detail.STENT_NOTE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetEyeSurgryDescLast(HIS_EYE_SURGRY_DESC eyeSurgDesc)
        {
            this.currentEyeSurgDesc = eyeSurgDesc;
        }

        private void GetSkinSurg(SkinSurgeryDesADO skinSurg)
        {
            this.SkinSurgeryDes = skinSurg;
        }

        private void spinExcuteTimeAdd_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                //if (spinExcuteTimeAdd.EditValue != null && spinExcuteTimeAdd.Value > 0 && (SereServExt == null || !SereServExt.END_TIME.HasValue))
                //{
                //    dtFinish.DateTime = dtStart.DateTime.AddMinutes((double)spinExcuteTimeAdd.Value);
                //}
                if (spinExcuteTimeAdd.EditValue != null && spinExcuteTimeAdd.Value > 0 && dtStart.EditValue != null)
                {
                    dtFinish.DateTime = dtStart.DateTime.AddMinutes((double)spinExcuteTimeAdd.Value);
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.SPIN_EXECUTE_TIME && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = spinExcuteTimeAdd.Value.ToString();
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.SPIN_EXECUTE_TIME;
                    csAddOrUpdate.VALUE = spinExcuteTimeAdd.Value.ToString();
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
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

        private void dtStart_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinExcuteTimeAdd.EditValue != null && spinExcuteTimeAdd.Value > 0 && (SereServExt == null || !SereServExt.END_TIME.HasValue) && dtStart.DateTime != DateTime.MinValue)
                {
                    dtFinish.DateTime = dtStart.DateTime.AddMinutes((double)spinExcuteTimeAdd.Value);
                }

                Inventec.Common.Logging.LogSystem.Info("dtStart.DateTime: " + dtStart.DateTime.ToString("yyyyMMddHHmmss"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSaveGroup_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSaveGroup.Name && o.MODULE_LINK == Module.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSaveGroup.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSaveGroup.Name;
                    csAddOrUpdate.VALUE = (chkSaveGroup.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = Module.ModuleLink;
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

        private void txtIcdCmSubCode_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
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

        private void txtIcdCmSubCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCmSubName.Focus();
                    txtIcdCmSubName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCmSubCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();
                string strIcdNames = "";
                if (!String.IsNullOrEmpty(currentValue))
                {
                    string seperate = ";";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = txtIcdCmSubCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD_CM>();
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CM_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (seperate + icdByCode.ICD_CM_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format(ResourceMessage.KhongTimThayICDTuongUng, strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = txtIcdCmSubCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = txtIcdCmSubCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            txtIcdCmSubCode.Focus();
                            txtIcdCmSubCode.Select(startPositionWarm, lenghtPositionWarm);
                        }
                    }
                }
                SetCheckedIcdCmsToControl(txtIcdCmSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCmSubName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCode2.Focus();
                    txtIcdCode2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCmSubName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins");
                    HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcdCms, txtIcdCmSubCode.Text, txtIcdCmSubName.Text);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(secondaryIcdADO);
                    listArgs.Add(true);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                    ((Form)extenceInstance).Show(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetStringIcdCms(string delegateIcdCodes, string delegateIcdNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateIcdCodes))
                {
                    txtIcdCmSubCode.Text = delegateIcdCodes;
                }

                if (!string.IsNullOrEmpty(delegateIcdNames))
                {
                    txtIcdCmSubName.Text = delegateIcdNames;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCmSubName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCode2.Focus();
                    txtIcdCode2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCreateImageLuuDo_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.imageADOs == null)
                    this.imageADOs = new List<ImageADO>();
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "(Chọn ảnh đuôi: .jpg, .jpeg, .png, .bmp, .bitmap, .gif)|*.jpg;*.jpeg;*.png;*.bmp;*.bitmap;*.gif";// "đuôi .jpg, .jpeg, .png, .bmp, .bitmap, .gif";
                open.Multiselect = true;
                if (open.ShowDialog() == DialogResult.OK)
                {
                    if (chkSaveGroup.Checked)
                    {
                        this.sereServ = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5)grdViewService.GetFocusedRow();
                        foreach (var item in sereServbyServiceReqs)
                        {
                            foreach (var file in open.FileNames)
                            {
                                System.Drawing.Image img = System.Drawing.Image.FromFile(file);
                                string fileName = file.Split('\\').LastOrDefault();
                                fileName = fileName.Split('.').FirstOrDefault();
                                string ext = Path.GetExtension(file);

                                ImageADO image = new ImageADO();
                                image.FileName = fileName + ext;
                                image.IsChecked = false;
                                image.IMAGE_DISPLAY = img;
                                byte[] buff = System.IO.File.ReadAllBytes(file);
                                image.streamImage = new System.IO.MemoryStream(buff);

                                FileHolder f1 = new FileHolder();
                                MemoryStream stream = new MemoryStream(buff);
                                // If you're going to read from the stream, you may need to reset the position to the start
                                stream.Position = 0;
                                Inventec.Common.Logging.LogSystem.Debug("chkSaveGroup.Checked =" + chkSaveGroup.Checked + "____GlobalVariables.APPLICATION_CODE" + GlobalVariables.APPLICATION_CODE + "____item.TDL_TREATMENT_CODE" + item.TDL_TREATMENT_CODE + "____fileName" + fileName + ext);
                                var rsUpload = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, item.TDL_TREATMENT_CODE, stream, fileName + ext);
                                if (rsUpload != null)
                                {
                                    CommonParam param = new CommonParam();
                                    HIS_SERE_SERV_FILE data = new HIS_SERE_SERV_FILE();
                                    data.DESCRIPTION = "";
                                    data.SERE_SERV_FILE_NAME = fileName;
                                    data.SERE_SERV_ID = item.ID;
                                    data.URL = rsUpload.Url;
                                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERE_SERV_FILE>(RequestUriStore.HIS_SERE_SERV_FILE_CREATE, ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                                    //gọi api tạo thành công thì load lại danh sách
                                    if (apiResult != null)
                                    {
                                        image.ID = apiResult.ID;
                                        if (item.ID == this.sereServ.ID)
                                        {
                                            this.imageADOs.Add(image);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(ResourceMessage.UploadFileThatBaiVuiLongLienHeQuanTriheThongDeDuocHoTro);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var file in open.FileNames)
                        {
                            System.Drawing.Image img = System.Drawing.Image.FromFile(file);
                            string fileName = file.Split('\\').LastOrDefault();
                            fileName = fileName.Split('.').FirstOrDefault();
                            string ext = Path.GetExtension(file);

                            ImageADO image = new ImageADO();
                            image.FileName = fileName + ext;
                            image.IsChecked = false;
                            image.IMAGE_DISPLAY = img;
                            byte[] buff = System.IO.File.ReadAllBytes(file);
                            image.streamImage = new System.IO.MemoryStream(buff);

                            FileHolder f1 = new FileHolder();
                            MemoryStream stream = new MemoryStream(buff);
                            // If you're going to read from the stream, you may need to reset the position to the start
                            stream.Position = 0;
                            Inventec.Common.Logging.LogSystem.Debug("GlobalVariables.APPLICATION_CODE" + GlobalVariables.APPLICATION_CODE + "____this.sereServ.TDL_TREATMENT_CODE" + this.sereServ.TDL_TREATMENT_CODE + "____fileName" + fileName + ext);
                            var rsUpload = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, this.sereServ.TDL_TREATMENT_CODE, stream, fileName + ext);
                            if (rsUpload != null)
                            {
                                CommonParam param = new CommonParam();
                                HIS_SERE_SERV_FILE data = new HIS_SERE_SERV_FILE();
                                data.DESCRIPTION = "";
                                data.SERE_SERV_FILE_NAME = fileName;
                                data.SERE_SERV_ID = this.sereServ.ID;
                                data.URL = rsUpload.Url;
                                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERE_SERV_FILE>(RequestUriStore.HIS_SERE_SERV_FILE_CREATE, ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                                //gọi api tạo thành công thì load lại danh sách
                                if (apiResult != null)
                                {
                                    image.ID = apiResult.ID;
                                    this.imageADOs.Add(image);
                                }
                            }
                            else
                            {
                                MessageBox.Show(ResourceMessage.UploadFileThatBaiVuiLongLienHeQuanTriheThongDeDuocHoTro);
                            }
                        }
                    }

                    List<long> ssIds = new List<long>();
                    ssIds.Add(this.sereServ.ID);
                    ProcessLoadSereServFile(ssIds);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ResourceMessage.UploadFileThatBaiVuiLongLienHeQuanTriheThongDeDuocHoTro);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tileView1_ContextButtonClick(object sender, DevExpress.Utils.ContextItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Name == "btnDelete")
                {
                    if (XtraMessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var dataItem = (DevExpress.XtraGrid.Views.Tile.TileViewItem)e.DataItem;
                        var item = (ImageADO)tileView1.GetRow(dataItem.RowHandle);
                        //nếu đã lưu thì gọi api xóa và check document
                        if (item.ID > 0)
                        {
                            CommonParam param = new CommonParam();
                            HIS_SERE_SERV_FILE data = new HIS_SERE_SERV_FILE();
                            data.ID = item.ID;
                            var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SERE_SERV_FILE_DELETE, ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                            //gọi api xóa thành công thì xóa ở danh sách và xóa document
                            if (apiResult)
                            {
                                tileView1.DeleteRow(dataItem.RowHandle);
                            }
                        }
                        else
                        {
                            tileView1.DeleteRow(dataItem.RowHandle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tileView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    Inventec.Common.Logging.LogSystem.Debug("tileView1_KeyDown.Delete");
                    TileView view = sender as TileView;
                    var checkedRows = view.GetCheckedRows();
                    if (checkedRows != null && checkedRows.Count() > 0)
                    {
                        this.imageADOs = this.imageADOs.Where(o => o.IsChecked == false).ToList();
                        ProcessLoadGridImage(this.imageADOs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SaveImageProcess(System.Drawing.Image imageData)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("SaveImageProcess: da chinh sua anh");
                if (this.currentImageADO != null && this.currentImageADO.IMAGE_DISPLAY != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("SaveImageProcess: 1");
                    MemoryStream stream = new MemoryStream();
                    imageData.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                    Inventec.Common.Logging.LogSystem.Info("SaveImageProcess: 2");
                    // If you're going to read from the stream, you may need to reset the position to the start
                    stream.Position = 0;
                    string ext = ".bmp";
                    var rsUpload = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, this.sereServ.TDL_TREATMENT_CODE, stream, this.currentImageADO.SERE_SERV_FILE_NAME + ext);
                    if (rsUpload != null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("SaveImageProcess: 3");
                        CommonParam param = new CommonParam();
                        HIS_SERE_SERV_FILE data = new HIS_SERE_SERV_FILE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_FILE>(data, this.currentImageADO);
                        data.URL = rsUpload.Url;
                        var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERE_SERV_FILE>(RequestUriStore.HIS_SERE_SERV_FILE_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                        //gọi api tạo thành công thì load lại danh sách
                        if (apiResult == null)
                        {
                            Inventec.Common.Logging.LogSystem.Info("SaveImageProcess: 4");
                            Inventec.Common.Logging.LogSystem.Warn("Luu anh da sua thất bại, " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info("SaveImageProcess: 5");
                            this.currentImageADO.IMAGE_DISPLAY = imageData;
                            this.currentImageADO.URL = rsUpload.Url;

                            cardControl.RefreshDataSource();
                        }
                    }
                    else
                    {
                        MessageBox.Show(ResourceMessage.UploadFileThatBaiVuiLongLienHeQuanTriheThongDeDuocHoTro);
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("SaveImageProcess: 6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileView1_ItemDoubleClick(object sender, TileViewItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("tileView1_ItemDoubleClick");
                // mở form xem ảnh
                this.currentImageADO = (ImageADO)tileView1.GetRow(e.Item.RowHandle);
                Inventec.DrawTools.frmDrawTools f = new Inventec.DrawTools.frmDrawTools(this.currentImageADO.IMAGE_DISPLAY, SaveImageProcess);
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessLoadGridImage(List<ImageADO> listImage)
        {
            try
            {
                cardControl.BeginUpdate();
                cardControl.DataSource = null;
                if (listImage != null && listImage.Count > 0)
                {
                    cardControl.DataSource = listImage;
                }
                cardControl.EndUpdate();
            }
            catch (Exception ex)
            {
                cardControl.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImagePublic_Click(object sender, EventArgs e)
        {
            try
            {
                var formSelectImage = new ViewImage.FormImageTemp(this.Module, SelectListImageTemp);
                formSelectImage.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectListImageTemp(List<HIS_TEXT_LIB> listImage)
        {
            try
            {
                if (listImage != null && listImage.Count > 0)
                {
                    if (chkSaveGroup.Checked)
                    {
                        this.sereServ = (V_HIS_SERE_SERV_5)grdViewService.GetFocusedRow();
                        foreach (var item in sereServbyServiceReqs)
                        {
                            foreach (var file in listImage)
                            {
                                string base64String = System.Text.Encoding.UTF8.GetString(file.CONTENT);
                                byte[] imageBytes = Convert.FromBase64String(base64String);

                                MemoryStream ms = new MemoryStream(imageBytes);
                                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                                string fileName = string.Format("{0}.{1}", string.Format("{0}_{1}_{2}", item.TDL_SERVICE_REQ_CODE, item.ID, DateTime.Now.ToString("HHmmss")), "jpg");

                                MemoryStream stream = new MemoryStream(imageBytes);
                                // If you're going to read from the stream, you may need to reset the position to the start
                                stream.Position = 0;
                                Inventec.Common.Logging.LogSystem.Debug("chkSaveGroup.Checked =" + chkSaveGroup.Checked + "____GlobalVariables.APPLICATION_CODE" + GlobalVariables.APPLICATION_CODE + "____item.TDL_TREATMENT_CODE" + item.TDL_TREATMENT_CODE + "____fileName" + fileName);
                                var rsUpload = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, item.TDL_TREATMENT_CODE, stream, fileName);
                                if (rsUpload != null)
                                {
                                    CommonParam param = new CommonParam();
                                    HIS_SERE_SERV_FILE data = new HIS_SERE_SERV_FILE();
                                    data.DESCRIPTION = "";
                                    data.SERE_SERV_FILE_NAME = fileName;
                                    data.SERE_SERV_ID = item.ID;
                                    data.URL = rsUpload.Url;
                                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERE_SERV_FILE>(RequestUriStore.HIS_SERE_SERV_FILE_CREATE, ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                                    if (apiResult != null)
                                    {
                                        //TODO
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(ResourceMessage.UploadFileThatBaiVuiLongLienHeQuanTriheThongDeDuocHoTro);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var file in listImage)
                        {
                            string base64String = System.Text.Encoding.UTF8.GetString(file.CONTENT);
                            byte[] imageBytes = Convert.FromBase64String(base64String);

                            MemoryStream ms = new MemoryStream(imageBytes);
                            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                            string fileName = string.Format("{0}.{1}", string.Format("{0}_{1}_{2}", this.sereServ.TDL_SERVICE_REQ_CODE, this.sereServ.ID, DateTime.Now.ToString("HHmmss")), "jpg");

                            MemoryStream stream = new MemoryStream(imageBytes);
                            // If you're going to read from the stream, you may need to reset the position to the start
                            stream.Position = 0;
                            Inventec.Common.Logging.LogSystem.Debug("GlobalVariables.APPLICATION_CODE" + GlobalVariables.APPLICATION_CODE + "____this.sereServ.TDL_TREATMENT_CODE" + this.sereServ.TDL_TREATMENT_CODE + "____fileName" + fileName);
                            var rsUpload = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, this.sereServ.TDL_TREATMENT_CODE, stream, fileName);
                            if (rsUpload != null)
                            {
                                CommonParam param = new CommonParam();
                                HIS_SERE_SERV_FILE data = new HIS_SERE_SERV_FILE();
                                data.DESCRIPTION = "";
                                data.SERE_SERV_FILE_NAME = fileName;
                                data.SERE_SERV_ID = this.sereServ.ID;
                                data.URL = rsUpload.Url;
                                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERE_SERV_FILE>(RequestUriStore.HIS_SERE_SERV_FILE_CREATE, ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                                if (apiResult != null)
                                {
                                    //TODO
                                }
                            }
                            else
                            {
                                MessageBox.Show(ResourceMessage.UploadFileThatBaiVuiLongLienHeQuanTriheThongDeDuocHoTro);
                            }
                        }
                    }

                    //gọi api tạo thành công thì load lại danh sách
                    List<long> ssIds = new List<long>();
                    ssIds.Add(this.sereServ.ID);
                    ProcessLoadSereServFile(ssIds);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSavePtttTemp_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_SERE_SERV_PTTT_TEMP dataTemp = GetDataForTemp();
                if (dataTemp != null)
                {
                    var formPtttTemp = new PtttTemp.FormPtttTemp(this.Module, dataTemp);
                    formPtttTemp.ShowDialog();
                    BackendDataWorker.Reset<HIS_SERE_SERV_PTTT_TEMP>();
                    LoadDataToComboPtttTemp();
                }
                else
                {
                    MessageBox.Show(ResourceMessage.KhongCoNoiDungLuuMau);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttTemp_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                HIS_SERE_SERV_PTTT_TEMP fillData = new HIS_SERE_SERV_PTTT_TEMP();
                if (cboPtttTemp.EditValue != null)
                {
                   
                    fillData =(cboPtttTemp.Properties.DataSource as List<HIS_SERE_SERV_PTTT_TEMP>).FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttTemp.EditValue.ToString())); 
                    //qtcode
                    if (fillData != null && !string.IsNullOrEmpty(fillData.TEXT_LIB_IDS))
                    {
                        var textLibIds = fillData.TEXT_LIB_IDS // lấy sanh sách TEXT_LIB_IDS
                            .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(a => a.Trim())
                            .ToList();

                        var hisTextLibs = BackendDataWorker.Get<HIS_TEXT_LIB>()
                            .Where(o => o.IS_ACTIVE == 1
                                && o.IS_DELETE != 1
                                && o.LIB_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_LIB_TYPE.ID__IMAGE
                                && textLibIds.Contains(o.ID.ToString()))
                            .ToList();

                        // Gọi hàm SelectListImageTemp để tạo HIS_SERE_SERV_FILE và load ảnh
                        if (hisTextLibs != null && hisTextLibs.Any())
                        {
                            SelectListImageTemp(hisTextLibs);
                        }
                        //qtcode
                    }
                }

                FillDataToControlFromTemp(fillData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPtttTemp_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPtttTemp.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadPtttGroupCode(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbPtttGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbPtttGroup.EditValue = null;
                    txtPtttGroupCode.Focus();
                    txtPtttGroupCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbPtttGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbPtttGroup.Text != null)
                    {
                        txtLoaiPT.Focus();
                        txtLoaiPT.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbPtttGroup_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbbPtttGroup.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_PTTT_GROUP data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbPtttGroup.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtPtttGroupCode.Text = data.PTTT_GROUP_CODE;
                        cbbPtttGroup.Properties.Buttons[1].Visible = cbbPtttGroup.Properties.Buttons[1].Enabled ? true : false;
                    }
                }
                else
                {
                    txtPtttGroupCode.Text = "";
                    cbbPtttGroup.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cbbPtttGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbPtttGroup.EditValue != null)
                    {
                        txtLoaiPT.Focus();
                        txtLoaiPT.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoaiPT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadLoaiPT(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiPT_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboLoaiPT.EditValue = null;
                    txtLoaiPT.Focus();
                    txtLoaiPT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiPT_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLoaiPT.EditValue != null)
                    {
                        txtBanMoCode.Focus();
                        txtBanMoCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiPT_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboLoaiPT.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_PTTT_PRIORITY data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiPT.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                        cboLoaiPT.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtLoaiPT.Text = "";
                    cboLoaiPT.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLoaiPT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiPT.EditValue != null)
                    {
                        txtBanMoCode.Focus();
                        txtBanMoCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBanMoCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadBanMo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBanMo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBanMo.EditValue = null;
                    txtBanMoCode.Focus();
                    txtBanMoCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBanMo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBanMo.EditValue != null)
                    {
                        txtMethodCode.Focus();
                        txtMethodCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBanMo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBanMo.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_PTTT_TABLE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_TABLE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBanMo.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtBanMoCode.Text = data.PTTT_TABLE_CODE;
                        cboBanMo.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtBanMoCode.Text = "";
                    cboBanMo.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBanMo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBanMo.EditValue != null)
                    {
                        txtMethodCode.Focus();
                        txtMethodCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMethodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadMethod(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMethod.EditValue = null;
                    txtMethodCode.Focus();
                    txtMethodCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMethod.EditValue != null)
                    {
                        txtPhuongPhap2.Focus();
                        txtPhuongPhap2.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMethod.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMethod.EditValue.ToString()));

                    {
                        txtMethodCode.Text = data.PTTT_METHOD_CODE;
                        cboMethod.Properties.Buttons[1].Visible = cboMethod.Properties.Buttons[1].Enabled ? true : false;
                    }
                }
                else
                {
                    txtMethodCode.Text = "";
                    cboMethod.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMethod.EditValue != null)
                    {
                        txtPhuongPhap2.Focus();
                        txtPhuongPhap2.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhuongPhap2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadPhuongPhap2(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhap2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPhuongPhap2.EditValue = null;
                    txtPhuongPhap2.Focus();
                    txtPhuongPhap2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhap2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhuongPhap2.EditValue != null)
                    {
                        txtPhuongPhapTT.Focus();
                        txtPhuongPhapTT.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhap2_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPhuongPhap2.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhuongPhap2.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                        cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtPhuongPhap2.Text = "";
                    cboPhuongPhap2.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPhuongPhap2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPhuongPhap2.EditValue != null)
                    {
                        txtPhuongPhapTT.Focus();
                        txtPhuongPhapTT.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhuongPhapTT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadPhuongPhapThucTe(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhapThucTe_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPhuongPhapThucTe.EditValue = null;
                    txtPhuongPhapTT.Focus();
                    txtPhuongPhapTT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhapThucTe_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhuongPhapThucTe.EditValue != null)
                    {
                        txtEmotionlessMethod.Focus();
                        txtEmotionlessMethod.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhapThucTe_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPhuongPhapThucTe.EditValue != null)
                {
                    //btnChoosePtttMethods.Enabled = false;
                    MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhuongPhapThucTe.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtPhuongPhapTT.Text = data.PTTT_METHOD_CODE;
                        cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    //btnChoosePtttMethods.Enabled = true;
                    txtPhuongPhapTT.Text = "";
                    cboPhuongPhapThucTe.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPhuongPhapThucTe_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPhuongPhapThucTe.EditValue != null)
                    {
                        txtEmotionlessMethod.Focus();
                        txtEmotionlessMethod.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBlood_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadBlood(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBlood_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbBlood.EditValue = null;
                    txtBlood.Focus();
                    txtBlood.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBlood_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbBlood.EditValue != null)
                    {
                        txtBloodRh.Focus();
                        txtBloodRh.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBlood_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbbBlood.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_BLOOD_ABO data = dataBloodAbo.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbBlood.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtBlood.Text = data.BLOOD_ABO_CODE;
                        cbbBlood.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtBlood.Text = "";
                    cbbBlood.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cbbBlood_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbBlood.EditValue != null)
                    {
                        txtBloodRh.Focus();
                        txtBloodRh.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBloodRh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadBloodRh(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBloodRh_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbBloodRh.EditValue = null;
                    txtBloodRh.Focus();
                    txtBloodRh.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBloodRh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbBloodRh.EditValue != null)
                    {
                        txtCatastrophe.Focus();
                        txtCatastrophe.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBloodRh_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbbBloodRh.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_BLOOD_RH data = dataBloodRh.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbBloodRh.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtBloodRh.Text = data.BLOOD_RH_CODE;
                        cbbBloodRh.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtBloodRh.Text = "";
                    cbbBloodRh.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cbbBloodRh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbBloodRh.EditValue != null)
                    {
                        txtCatastrophe.Focus();
                        txtCatastrophe.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmotionlessMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadEmotionlessMethod(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbEmotionlessMethod.EditValue = null;
                    txtEmotionlessMethod.Focus();
                    txtEmotionlessMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        txtKQVoCam.Focus();
                        txtKQVoCam.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbbEmotionlessMethod.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbEmotionlessMethod.EditValue ?? 0).ToString()) && o.IS_ACTIVE == 1);
                    if (data != null)
                    {
                        txtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                        cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtEmotionlessMethod.Text = "";
                    cbbEmotionlessMethod.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cbbEmotionlessMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        txtKQVoCam.Focus();
                        txtKQVoCam.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKQVoCam_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadKQVoCam(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKQVoCam_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboKQVoCam.EditValue = null;
                    txtKQVoCam.Focus();
                    txtKQVoCam.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKQVoCam_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboKQVoCam.EditValue != null)
                    {
                        txtCondition.Focus();
                        txtCondition.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKQVoCam_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboKQVoCam.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_RESULT data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboKQVoCam.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtKQVoCam.Text = data.EMOTIONLESS_RESULT_CODE;
                        cboKQVoCam.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtKQVoCam.Text = "";
                    cboKQVoCam.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboKQVoCam_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboKQVoCam.EditValue != null)
                    {
                        txtCondition.Focus();
                        txtCondition.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCondition_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadCondition(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCondition_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCondition.EditValue = null;
                    txtCondition.Focus();
                    txtCondition.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCondition_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCondition.EditValue != null)
                    {
                        txtBlood.Focus();
                        txtBlood.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCondition_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCondition.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CONDITION>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCondition.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtCondition.Text = data.PTTT_CONDITION_CODE;
                        cboCondition.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtCondition.Text = "";
                    cboCondition.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCondition_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCondition.EditValue != null)
                    {
                        txtBlood.Focus();
                        txtBlood.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMoKTCao_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadMoKTCao(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMoKTCao_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMoKTCao.EditValue = null;
                    txtMoKTCao.Focus();
                    txtMoKTCao.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMoKTCao_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMoKTCao.EditValue != null)
                    {
                        dtStart.Focus();
                        dtStart.SelectAll();
                        dtStart.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMoKTCao_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMoKTCao.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_PTTT_HIGH_TECH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMoKTCao.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtMoKTCao.Text = data.PTTT_HIGH_TECH_CODE;
                        cboMoKTCao.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtMoKTCao.Text = "";
                    cboMoKTCao.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMoKTCao_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMoKTCao.EditValue != null)
                    {
                        dtStart.Focus();
                        dtStart.SelectAll();
                        dtStart.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCatastrophe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadCatastrophe(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCatastrophe_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCatastrophe.EditValue = null;
                    txtCatastrophe.Focus();
                    txtCatastrophe.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCatastrophe_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCatastrophe.EditValue != null)
                    {
                        txtDeathSurg.Focus();
                        txtDeathSurg.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCatastrophe_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCatastrophe.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CATASTROPHE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCatastrophe.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtCatastrophe.Text = data.PTTT_CATASTROPHE_CODE;
                        cboCatastrophe.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtCatastrophe.Text = "";
                    cboCatastrophe.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCatastrophe_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCatastrophe.EditValue != null)
                    {
                        txtDeathSurg.Focus();
                        txtDeathSurg.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDeathSurg_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadDeathSurg(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathSurg_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDeathSurg.EditValue = null;
                    txtDeathSurg.Focus();
                    txtDeathSurg.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathSurg_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDeathSurg.EditValue != null)
                    {
                        txtMachineCode.Focus();
                        txtMachineCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathSurg_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDeathSurg.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDeathSurg.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtDeathSurg.Text = data.DEATH_CAUSE_CODE;
                        cboDeathSurg.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtDeathSurg.Text = "";
                    cboDeathSurg.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDeathSurg_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDeathSurg.EditValue != null)
                    {
                        txtMachineCode.Focus();
                        txtMachineCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMachineCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadMachine(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMachine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMachine.EditValue = null;
                    txtMachineCode.Focus();
                    txtMachineCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMachine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMachine.EditValue != null)
                    {
                        txtMoKTCao.Focus();
                        txtMoKTCao.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMachine_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMachine.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_MACHINE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMachine.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtMachineCode.Text = data.MACHINE_CODE;
                        cboMachine.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtMachineCode.Text = "";
                    cboMachine.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMachine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMachine.EditValue != null)
                    {
                        txtMoKTCao.Focus();
                        txtMoKTCao.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChoosePtttMethods_Click(object sender, EventArgs e)
        {
            try
            {
                IsChoosePTTT = false;
                List<PtttMethodADO> oldData = new List<PtttMethodADO>();
                if (this.sereServ != null && this.listSesePtttMetod != null && this.listSesePtttMetod.Count > 0)
                {
                    oldData = this.listSesePtttMetod.Where(o => o.SERE_SERV_ID == this.sereServ.ID).ToList();
                }

                PtttMethod.FormPtttMethod fm = new PtttMethod.FormPtttMethod(ChooseMethod, this.sereServ, oldData, Module);
                fm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChooseMethod(List<PtttMethodADO> obj)
        {
            try
            {
                var oldData = this.listSesePtttMetod.Where(o => o.SERE_SERV_ID == this.sereServ.ID).ToList();
                if (oldData != null && oldData.Count > 0)
                {
                    this.listSesePtttMetod = this.listSesePtttMetod.Where(o => !oldData.Select(s => s.ID).Contains(o.ID)).ToList();
                }

                if (obj != null && obj.Count > 0)
                {
                    this.listSesePtttMetod.AddRange(obj);
                }

                if (this.sereServPTTT != null)
                {
                    this.sereServPTTT.REAL_PTTT_METHOD_ID = null;
                    IsChoosePTTT = true;
                    ProcessLoadRealPtttMethod(this.sereServPTTT);
                }
                else
                {
                    var pttt = new V_HIS_SERE_SERV_PTTT();
                    pttt.SERE_SERV_ID = this.sereServ.ID;
                    IsChoosePTTT = true;
                    ProcessLoadRealPtttMethod(pttt);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkServiceCode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkServiceCode.Name && o.MODULE_LINK == Module.ModuleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkServiceCode.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkServiceCode.Name;
                    csAddOrUpdate.VALUE = (chkServiceCode.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = Module.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                LoadSereServLast();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonCoppy_Click(object sender, EventArgs e)
        {
            try
            {
                this.checkSTT = true;
                this.listSesePtttMetod = this.listSesePtttMetod.Where(o => o.SERVICE_REQ_ID == this.sereServ.SERVICE_REQ_ID).ToList();
                this.sereServLast = (MOS.EFMODEL.DataModels.HIS_SERE_SERV)gridViewSereServLast.GetFocusedRow();
                if (sereServLast != null)
                {
                    FillDataFromSereServLast(sereServLast.ID);
                    //GetSereServPtttBySereServId();
                    this.refreshControl();

                    HisSereServPtttViewFilter filter = new HisSereServPtttViewFilter();
                    filter.SERE_SERV_ID = sereServLast.ID;
                    var dataSereServPttt = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, filter, null);
                    if (dataSereServPttt != null && dataSereServPttt.Count > 0)
                    {
                        var datasereServPTTT = dataSereServPttt.FirstOrDefault();

                        txtMethodCode.Text = datasereServPTTT.PTTT_METHOD_CODE ?? "";
                        cboMethod.EditValue = datasereServPTTT.PTTT_METHOD_ID ?? null;
                        txtPtttGroupCode.Text = datasereServPTTT.PTTT_GROUP_CODE ?? "";
                        cbbPtttGroup.EditValue = datasereServPTTT.PTTT_GROUP_ID ?? null;
                        if (dataEmotionlessMethod.Exists(o => o.ID == datasereServPTTT.EMOTIONLESS_METHOD_ID))
                        {
                            cbbEmotionlessMethod.EditValue = datasereServPTTT.EMOTIONLESS_METHOD_ID;
                        }
                        else
                        {
                            cbbEmotionlessMethod.EditValue = null;
                        }
                        txtBlood.Text = Patient.BLOOD_ABO_CODE ?? sereServPTTT.BLOOD_ABO_CODE ?? "";
                        var bloodAbo = dataBloodAbo.FirstOrDefault(o => o.BLOOD_ABO_CODE == (Patient.BLOOD_ABO_CODE ?? sereServPTTT.BLOOD_ABO_CODE ?? ""));
                        if (bloodAbo != null)
                            cbbBlood.EditValue = bloodAbo.ID;
                        else
                            cbbBlood.EditValue = null;
                        txtBloodRh.Text = Patient.BLOOD_RH_CODE ?? sereServPTTT.BLOOD_RH_CODE ?? "";
                        var bloodRh = dataBloodRh.FirstOrDefault(o => o.BLOOD_RH_CODE == (Patient.BLOOD_RH_CODE ?? sereServPTTT.BLOOD_RH_CODE ?? ""));
                        if (bloodRh != null)
                            cbbBloodRh.EditValue = bloodRh.ID;
                        else
                            cbbBloodRh.EditValue = null;
                        txtCondition.Text = datasereServPTTT.PTTT_CONDITION_CODE ?? "";
                        cboCondition.EditValue = datasereServPTTT.PTTT_CONDITION_ID ?? null;
                        txtCatastrophe.Text = datasereServPTTT.PTTT_CATASTROPHE_CODE ?? "";
                        cboCatastrophe.EditValue = datasereServPTTT.PTTT_CATASTROPHE_ID ?? null;
                        txtDeathSurg.Text = datasereServPTTT.DEATH_WITHIN_CODE ?? "";
                        cboDeathSurg.EditValue = datasereServPTTT.DEATH_WITHIN_ID ?? null;

                        CommonParam param = new CommonParam();
                        HisSesePtttMethodFilter methodfilter = new HisSesePtttMethodFilter();
                        methodfilter.SERE_SERV_PTTT_ID = datasereServPTTT.ID;
                        var listMethod = new BackendAdapter(param).Get<List<HIS_SESE_PTTT_METHOD>>("api/HisSesePtttMethod/Get", ApiConsumers.MosConsumer, methodfilter, param);
                        listMethod.ForEach(o => o.TDL_SERE_SERV_ID = this.sereServ.ID);
                        LoadDataSesePtttMethodDetail(listMethod);

                        LoadDetailSereServPtttFromSereServLast(datasereServPTTT);
                    }
                }
                FillDataToInformationSurgFromSereServLast(sereServLast);
                //SetEnableControl();
                SetDefaultCboPTMethodFromSereServLast(sereServLast, cboMethod, txtMethodCode);
                SetDefaultCboPTMethodFromSereServLast(sereServLast, cboPhuongPhapThucTe, txtPhuongPhapTT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        void LoadDetailSereServPtttFromSereServLast(V_HIS_SERE_SERV_PTTT dataSereServPttt)
        {
            try
            {
                if (dataSereServPttt != null && dataSereServPttt.ID > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("LOAD 3");
                    FillDataToCboIcd(txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, dataSereServPttt.ICD_CODE, dataSereServPttt.ICD_NAME);
                    FillDataToCboIcd(txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, dataSereServPttt.BEFORE_PTTT_ICD_CODE, dataSereServPttt.BEFORE_PTTT_ICD_NAME);
                    FillDataToCboIcd(txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, dataSereServPttt.AFTER_PTTT_ICD_CODE, dataSereServPttt.AFTER_PTTT_ICD_NAME);
                    FillDataToCboIcdCm(txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm, dataSereServPttt.ICD_CM_CODE, dataSereServPttt.ICD_CM_NAME);
                    Inventec.Common.Logging.LogSystem.Warn("LOAD 4");
                    if (!string.IsNullOrEmpty(dataSereServPttt.ICD_SUB_CODE))
                    {
                        txtIcdExtraCode.Text = dataSereServPttt.ICD_SUB_CODE;
                    }
                    if (!string.IsNullOrEmpty(dataSereServPttt.ICD_TEXT))
                    {
                        txtIcdText.Text = dataSereServPttt.ICD_TEXT;
                    }

                    if (!string.IsNullOrEmpty(dataSereServPttt.ICD_CM_SUB_CODE))
                    {
                        this.txtIcdCmSubCode.Text = dataSereServPttt.ICD_CM_SUB_CODE;
                    }
                    if (!string.IsNullOrEmpty(dataSereServPttt.ICD_CM_TEXT))
                    {
                        this.txtIcdCmSubName.Text = dataSereServPttt.ICD_CM_TEXT;
                    }

                    txtMANNER.Text = dataSereServPttt.MANNER;

                    dataSereServPttt.SERE_SERV_ID = this.sereServ.ID;
                    ProcessLoadRealPtttMethod(dataSereServPttt);

                    if (dataSereServPttt.PTTT_PRIORITY_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_PTTT_PRIORITY>().FirstOrDefault(p => p.ID == dataSereServPttt.PTTT_PRIORITY_ID);
                        if (data != null)
                        {
                            txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                            cboLoaiPT.EditValue = data.ID;
                        }
                        //else
                        //{
                        //    txtLoaiPT.Text = "";
                        //    cboLoaiPT.EditValue = null;
                        //}
                    }
                    if (dataSereServPttt.EMOTIONLESS_METHOD_SECOND_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(p => p.ID == dataSereServPttt.EMOTIONLESS_METHOD_SECOND_ID);
                        if (data != null)
                        {
                            txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                            cboPhuongPhap2.EditValue = data.ID;
                        }
                        else
                        {
                            txtPhuongPhap2.Text = "";
                            cboPhuongPhap2.EditValue = null;
                        }
                    }
                    if (dataSereServPttt.EMOTIONLESS_RESULT_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>().FirstOrDefault(p => p.ID == dataSereServPttt.EMOTIONLESS_RESULT_ID);
                        if (data != null)
                        {
                            txtKQVoCam.Text = data.EMOTIONLESS_RESULT_CODE;
                            cboKQVoCam.EditValue = data.ID;
                        }
                        else
                        {
                            txtKQVoCam.Text = "";
                            cboKQVoCam.EditValue = null;
                        }
                    }
                    if (dataSereServPttt.PTTT_HIGH_TECH_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>().FirstOrDefault(p => p.ID == dataSereServPttt.PTTT_HIGH_TECH_ID);
                        if (data != null)
                        {
                            txtMoKTCao.Text = data.PTTT_HIGH_TECH_CODE;
                            cboMoKTCao.EditValue = data.ID;
                        }
                        else
                        {
                            txtMoKTCao.Text = "";
                            cboMoKTCao.EditValue = null;
                        }
                    }
                    if (dataSereServPttt.PTTT_TABLE_ID > 0)
                    {
                        var data = BackendDataWorker.Get<HIS_PTTT_TABLE>().FirstOrDefault(p => p.ID == dataSereServPttt.PTTT_TABLE_ID);
                        if (data != null)
                        {
                            txtBanMoCode.Text = data.PTTT_TABLE_CODE;
                            cboBanMo.EditValue = data.ID;
                        }
                        else
                        {
                            txtBanMoCode.Text = "";
                            cboBanMo.EditValue = null;
                        }
                    }

                    if (!string.IsNullOrEmpty(dataSereServPttt.SUBS_DIRECTOR_LOGINNAME) && lstReAcsUserADO.Exists(o => o.LOGINNAME == dataSereServPttt.SUBS_DIRECTOR_LOGINNAME))
                    {
                        cboHospSubs.EditValue = dataSereServPttt.SUBS_DIRECTOR_LOGINNAME;
                    }

                    if (!string.IsNullOrEmpty(dataSereServPttt.SUBS_HEAD_LOGINNAME) && lstReAcsUserADO.Exists(o => o.LOGINNAME == dataSereServPttt.SUBS_HEAD_LOGINNAME))
                    {
                        cboEndDeptSubs.EditValue = dataSereServPttt.SUBS_HEAD_LOGINNAME;
                    }
                }
                else if (this.sereServ != null && !this.sereServ.EKIP_ID.HasValue)
                {
                    this.txtMANNER.Text = this.sereServ.TDL_SERVICE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToInformationSurgFromSereServLast(HIS_SERE_SERV sereServLast)
        {
            try
            {

                CommonParam param = new CommonParam();
                List<HisEkipUserADO> ekipUserAdos = new List<HisEkipUserADO>();
                if (sereServLast.EKIP_ID.HasValue)
                {
                    ucEkip.SetDepartmentID(Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue != null ? cboDepartment.EditValue.ToString() : "0"));
                    ucEkip.FillDataToInformationSurgFromSereServLast(ekipUserAdos, sereServLast);

                }
                else if (this.serviceReq.EKIP_PLAN_ID.HasValue) //tiennv
                {
                    ucEkip.SetDepartmentID(Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue != null ? cboDepartment.EditValue.ToString() : "0"));
                    ucEkip.FillDataToInformationSurgFromServiceReqEkipPlan(ekipUserAdos, serviceReq);

                }

                if (sereServLast.PACKAGE_ID.HasValue)
                {
                    HIS_PACKAGE package = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.ID == sereServLast.PACKAGE_ID.Value);
                    lblPackageType.Text = package != null ? package.PACKAGE_NAME : "";
                }
                else
                {
                    lblPackageType.Text = "";
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultCboPTMethodFromSereServLast(HIS_SERE_SERV sereServLast, GridLookUpEdit txt, TextEdit cbo)
        {
            try
            {
                if (sereServLast != null)
                {
                    HIS_PTTT_METHOD ptttMethod = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.IS_ACTIVE == (short)1 && o.PTTT_METHOD_NAME.ToLower() == sereServLast.TDL_SERVICE_NAME.ToLower());
                    if (ptttMethod != null)
                    {
                        txt.Text = ptttMethod.PTTT_METHOD_CODE;
                        cbo.EditValue = ptttMethod.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task FillDataFromSereServLast(long id)
        {
            try
            {
                //nếu có thời gian bắt đầu/kết thúc đã lưu thì load theo thời gian đã lưu
                CommonParam param = new CommonParam();
                HisSereServExtFilter ssExtFilter = new HisSereServExtFilter();
                ssExtFilter.SERE_SERV_ID = id;
                var SereServExts = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, ssExtFilter, param);
                this.SereServExt = new HIS_SERE_SERV_EXT();//xuandv
                if (SereServExts != null && SereServExts.Count > 0)
                {
                    //add vào danh sách
                    foreach (var item in ListSereServExt)
                    {
                        HIS_SERE_SERV_EXT ext = ListSereServExt.FirstOrDefault(o => o.ID == item.ID);
                        if (ext != null)
                            ext = item;//gán lại để cập nhật dữ liệu mới nhất
                        else
                            ListSereServExt.Add(item);
                    }

                    this.SereServExt = SereServExts.FirstOrDefault();
                    txtConclude.Text = this.SereServExt.CONCLUDE;
                    txtDescription.Text = this.SereServExt.DESCRIPTION;
                    txtResultNote.Text = this.SereServExt.NOTE;
                    txtIntructionNote.Text = this.SereServExt.INSTRUCTION_NOTE;
                    txtMachineCode.Text = this.SereServExt.MACHINE_CODE;
                    cboMachine.EditValue = this.SereServExt.MACHINE_ID;
                    Inventec.Common.Logging.LogSystem.Info("SereServExts: " + SereServExts.Count);
                    dtStart.DateTime = DateTime.Now;
                    dtFinish.DateTime = DateTime.Now;
                    //spinExcuteTimeAdd.EditValue = 0;

                    //if (this.SereServExt.END_TIME.HasValue)
                    //{
                    //    dtFinish.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.SereServExt.END_TIME.Value) ?? DateTime.Now;
                    //    dtFinish.Properties.Buttons[1].Visible = true;
                    //    if (this.SereServExt.BEGIN_TIME.HasValue)
                    //    {
                    //        DateTime? dateBefore = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.SereServExt.BEGIN_TIME.Value);
                    //        DateTime? dateAfter = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.SereServExt.END_TIME.Value);
                    //        if (dateBefore != null && dateAfter != null)
                    //        {
                    //            TimeSpan difference = dateAfter.Value - dateBefore.Value;
                    //            Inventec.Common.Logging.LogSystem.Warn(difference + "_______________!!!!");
                    //            spinExcuteTimeAdd.EditValue = difference.TotalMinutes;
                    //        }
                    //    }

                    //}
                    //else
                    //{
                    //    dtFinish.EditValue = null;
                    //    dtFinish.Properties.Buttons[1].Visible = false;
                    //}
                }
                else
                {
                    txtConclude.Text = "";
                    txtDescription.Text = "";
                    txtResultNote.Text = "";
                    txtIntructionNote.Text = "";
                    txtMachineCode.Text = "";
                    cboMachine.EditValue = null;
                }

                //if (serviceReq != null && this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.TakeIntrucionTimeByServiceReq") == "1" && ((SereServExt != null && SereServExt.BEGIN_TIME == null) || SereServExt == null))
                //{
                //    dtStart.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME) ?? DateTime.Now;
                //}

                //để sau api GetAsync để dữ liệu được giữ nguyên.
                //nếu có cấu hình thì gán thời gian theo cấu hình
                if (!String.IsNullOrWhiteSpace(HisConfigCFG.TAKE_INTRUCTION_TIME_BY_SERVICE_REQ) && (SereServExt == null || !SereServExt.BEGIN_TIME.HasValue))
                {
                    if (serviceReq != null
                        && this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                        && HisConfigCFG.TAKE_INTRUCTION_TIME_BY_SERVICE_REQ == "1")
                    {
                        dtStart.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME) ?? DateTime.Now;
                    }
                    else if (serviceReq != null
                        && (this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                        && HisConfigCFG.TAKE_INTRUCTION_TIME_BY_SERVICE_REQ == "2")
                    {
                        dtStart.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME) ?? DateTime.Now;
                    }
                    else if (serviceReq != null
                      && (this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || this.sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                      && HisConfigCFG.TAKE_INTRUCTION_TIME_BY_SERVICE_REQ == "3")
                    {
                        dtStart.DateTime = DateTime.Now;
                    }
                }

                if (spinExcuteTimeAdd.EditValue != null && spinExcuteTimeAdd.Value > 0 && dtStart.DateTime != DateTime.MinValue && (SereServExt == null || !SereServExt.END_TIME.HasValue))
                {
                    dtFinish.DateTime = dtStart.DateTime.AddMinutes((double)spinExcuteTimeAdd.Value);
                }

                //xuandv
                this.InitButtonOther();

                List<long> ssIds = new List<long>();
                ssIds.Add(this.sereServ.ID);
                ProcessLoadSereServFile(ssIds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCoppyInfo_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(copyInfoClick, "btnCoppyInfo_Click");
        }

        private void copyInfoClick()
        {
            try
            {
                if (this.sereServbyServiceReqs != null && this.sereServbyServiceReqs.Count() > 0 && this.sereServLasts != null && this.sereServLasts.Count() > 0)
                {
                    dicSereServCopy = new Dictionary<long, long>();
                    List<V_HIS_SERE_SERV_5> sereServbyServiceReqsNew = new List<V_HIS_SERE_SERV_5>();
                    foreach (var item in sereServbyServiceReqs)
                    {
                        var lstSereServByServiceReq = sereServLasts.Where(o => o.SERVICE_ID == item.SERVICE_ID);
                        if (lstSereServByServiceReq != null && lstSereServByServiceReq.Count() > 0)
                        {
                            var ssByServiceReq = lstSereServByServiceReq.OrderByDescending(o => o.TDL_INTRUCTION_TIME).FirstOrDefault();
                            V_HIS_SERE_SERV_5 sereServ5 = new V_HIS_SERE_SERV_5();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_5>(sereServ5, ssByServiceReq);
                            sereServ5.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            sereServ5.TDL_TREATMENT_ID = item.TDL_TREATMENT_ID;
                            sereServ5.ID = item.ID;
                            sereServ5.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                            if (!dicSereServCopy.ContainsKey(item.ID))
                                dicSereServCopy[item.ID] = ssByServiceReq.ID;
                            sereServbyServiceReqsNew.Add(sereServ5);
                        }
                        else
                        {
                            sereServbyServiceReqsNew.Add(item);
                        }
                    }
                    grdControlService.BeginUpdate();
                    grdControlService.DataSource = sereServbyServiceReqsNew;
                    grdControlService.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void chkKetThuc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkKetThuc.Name && o.MODULE_LINK == Module.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkKetThuc.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkKetThuc.Name;
                    csAddOrUpdate.VALUE = (chkKetThuc.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = Module.ModuleLink;
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

        private void btnSurgAssignAndCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.serviceReq != null)
                {
                    long beginTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStart.DateTime) ?? 0;
                            long endTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinish.DateTime) ?? 0;
                    FormSurgAssignAndCopy.frmSurgAssignAndCopy form = new FormSurgAssignAndCopy.frmSurgAssignAndCopy(this.Module, this.serviceReq, this.vhisTreatment, this.sereServ, beginTime, endTime, ucEkip.GetDataSource());
                    form.ShowDialog();
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
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSign.Name && o.MODULE_LINK == Module.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSign.Name;
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = Module.ModuleLink;
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

        private void dtFinish_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtFinish.EditValue != null && dtFinish.DateTime != DateTime.MinValue && dtStart.EditValue != null && dtStart.DateTime != DateTime.MinValue)
                    spinExcuteTimeAdd.EditValue = (dtFinish.DateTime - dtStart.DateTime).TotalMinutes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEndDeptSubs_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboEndDeptSubs.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboHospSubs_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboHospSubs.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPresYhct_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionYHCT").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionYHCT");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>();
                    V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>(sereServ);

                    AssignPrescriptionADO assignServiceADO = new AssignPrescriptionADO(serviceReq.TREATMENT_ID, intructionTime, this.serviceReq.ID, sereServInput, LoadSereServOutKipResult);
                    assignServiceADO.GenderName = serviceReq.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.PatientName = serviceReq.TDL_PATIENT_NAME;
                    assignServiceADO.PatientDob = serviceReq.TDL_PATIENT_DOB;
                    assignServiceADO.PatientId = serviceReq.TDL_PATIENT_ID;
                    listArgs.Add(assignServiceADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIn_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIn.Checked)
            {
                chkXemIn.Checked = false;
            }
        }

        private void chkXemIn_CheckedChanged(object sender, EventArgs e)
        {
            if (chkXemIn.Checked)
            {
                chkIn.Checked = false;
            }
        }

        //private void chkXemIn_Click(object sender, EventArgs e)
        //{
        //    if (chkXemIn.Checked && !IsActionOtherButton)
        //    {
        //        PrintProcess(PrintTypeSurg.PHIEU_THU_THUAT_PHAU_THUAT);
        //    }
        //}
    }
}
