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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServiceExecute.EkipTemp;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
using HIS.Desktop.Utility;
using HIS.UC.Icd;
using HIS.UC.SecondaryIcd;
using System.IO;
using DevExpress.XtraGrid.Views.Tile;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Common.SignLibrary.ADO;
using System.Threading;
using Inventec.Common.SignLibrary;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Reflection;
using AutoMapper;
using Inventec.Desktop.CustomControl;
using System.Resources;
namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmClsInfo : Form
    {
        #region 
        ADO.ServiceADO currentServiceADO;
        private HIS_SERE_SERV_EXT sereServExt;
        private V_HIS_SERVICE_REQ serviceReq;
        internal HIS_SERE_SERV_PTTT sereServPTTT { get; set; }
        private List<HisEkipUserADO> ekipUserAdos = new List<HisEkipUserADO>();
        Action<HIS_SERE_SERV_PTTT, HIS_SERE_SERV_EXT, ServiceADO> actSaveClick;
        internal List<HIS_EKIP_TEMP> ekipTemps { get; set; }
        internal List<HIS_ICD> dataIcds { get; set; }
        internal long autoCheckIcd;
        string _TextIcdName1 = "";
        string _TextIcdName2 = "";
        string _TextIcdName3 = "";
        public int positionHandle = -1;
        Inventec.Desktop.Common.Modules.Module Module;
        bool isAllowEditInfo;
        internal V_HIS_TREATMENT vhisTreatment { get; set; }
        List<AcsUserADO> AcsUserADOList;
        string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.SubclinicalProcessingInformationOption);

        internal IcdProcessor icdMainProcessor;
        internal IcdProcessor icdBeforeProcessor;
        internal IcdProcessor icdAfterProcessor;
        internal UserControl ucIcdMain;
        internal UserControl ucIcdBefore;
        internal UserControl ucIcdAfter;
        internal List<MOS.EFMODEL.DataModels.HIS_ICD> listIcd;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;
        internal string AutoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.AutoCheckIcd");
        internal HIS.UC.Icd.IcdProcessor IcdCauseProcessor { get; set; }
        internal UserControl ucIcdCause;
        private List<ModuleControlADO> ModuleControls { get; set; }

        List<ImageADO> imageADOs = new List<ImageADO>();
        ImageADO currentImageADO;
        List<HIS_PTTT_GROUP> datasPtttGroup = null;
        List<HIS_PTTT_METHOD> datasPtttMethod = null;
        List<HIS_EMOTIONLESS_METHOD> datasEmotionLessMethod = null;
        List<HIS_PTTT_CONDITION> datasPtttCondition = null;
        List<HIS_PTTT_CATASTROPHE> datasPtttCatastrophe = null;
        List<HIS_MACHINE> datasMachine = null;
        List<HIS_PTTT_PRIORITY> datasPtttPriority = null;
        List<HIS_PTTT_TABLE> datasPtttTable = null;
        List<HIS_EMOTIONLESS_METHOD> datasEmotionlessMethod2 = null;
        List<HIS_EMOTIONLESS_RESULT> datasEmotionlessResult = null;
        List<HIS_BLOOD_ABO> datasBloodABO = null;
        List<HIS_BLOOD_RH> datasBloodRh = null;
        List<HIS_DEATH_WITHIN> datasDeathWithin = null;
        List<HIS_PTTT_HIGH_TECH> datasPtttHighTech = null;
        UCEkipUser ucEkip;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private string MODULELINK = "HIS.Desktop.Plugins.ServiceExecute.frmClsInfo";
        private string NumUserExecute = "1";
        private Common.RefeshReference delegateRefresh;
        internal List<HIS_ICD_CM> dataIcdCms { get; set; }
        List<V_HIS_SERVICE> lstService { get; set; }
        List<HIS_EXECUTE_ROLE> lstExecuteRole { get; set; }
        #endregion
        public frmClsInfo(Inventec.Desktop.Common.Modules.Module moduleData, ADO.ServiceADO serviceADO, Action<HIS_SERE_SERV_PTTT, HIS_SERE_SERV_EXT, ServiceADO> actsaveclick, HIS_SERE_SERV_PTTT sereservPTTT, HIS_SERE_SERV_EXT sereServExt, V_HIS_SERVICE_REQ serviceReq, Common.RefeshReference delegateRefresh, List<V_HIS_SERVICE> lstService, List<HIS_EXECUTE_ROLE> lstExecuteRole)
        {

            try
            {
                InitializeComponent();
                this.Module = moduleData;
                this.currentServiceADO = serviceADO;
                this.sereServPTTT = sereservPTTT;
                this.actSaveClick = actsaveclick;
                this.serviceReq = serviceReq;
                this.sereServExt = sereServExt;
                this.delegateRefresh = delegateRefresh;
                this.lstService = lstService;
                this.lstExecuteRole = lstExecuteRole;
                if (HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.IsRequiredPtttPriority)
                {
                    lciHinhThucPTTT.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region
        private void frmClsInfo_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                if (this.currentServiceADO != null)
                {
                    List<HIS_ICD_CM> dataCms = null;
                    if (BackendDataWorker.IsExistsKey<HIS_ICD_CM>())
                    {
                        dataCms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD_CM>();
                    }
                    else
                    {
                        CommonParam paramCommon = new CommonParam();
                        dynamic filter = new System.Dynamic.ExpandoObject();
                        dataCms = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_ICD_CM>>("api/HisIcdCm/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                        if (dataCms != null) BackendDataWorker.UpdateToRam(typeof(HIS_ICD_CM), dataCms, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    }

                    this.dataIcdCms = new List<HIS_ICD_CM>();
                    if (dataCms != null && dataCms.Count > 0)
                        this.dataIcdCms = dataCms.Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    DataToComboChuanDoanTD(cboIcdCmName, true);
                    if (this.sereServPTTT != null)
                    {
                        NumUserExecute = sereServPTTT.PARTICIPANT_NUMBER;
                    }
                    NumUserExecute = !string.IsNullOrEmpty(NumUserExecute) ? NumUserExecute : "1";
                    txtNumExecute.Text = NumUserExecute;
                    ucEkip = new UCEkipUser(Plus_Click, Delete_Click, NumExe, Int32.Parse(NumUserExecute), lstExecuteRole);
                    ucEkip.Dock = DockStyle.Fill;
                    panel1.Controls.Add(ucEkip);
                    LoadComboEkip();
                    InitControlState();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.sereServPTTT), this.sereServPTTT) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentServiceADO), this.currentServiceADO));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.sereServExt), this.sereServExt));
                    this.isAllowEditInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.AppConfigKeys.MOS__HIS_SERVICE_REQ__ALLOW_UPDATE_SURG_INFO_AFTER_LOCKING_TREATMENT) == "1";
                    this.LoadTreatment();
                    this.GetSereServPtttBySereServId();
                    this.SetEnableControl();
                    //this.ComboMethodICD();
                    //this.ComboAcsUser();
                    listIcd = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                    InitUcIcdMain();
                    InitUcSecondaryIcd();
                    InitUcIcdBefore();
                    InitUcIcdAfter();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("serviceReq___:", serviceReq));
                    this.SetIcdFromServiceReq(this.serviceReq);
                    //this.ComboPTTTGroup();
                    //this.ComboLoaiPT();
                    //this.ComboEmotionlessMothod();
                    this.SetDefaultCboPTTTGroupOnly();
                    this.LoadSereServExt();
                    this.SetDataControlBySereServPttt();

                    this.SetButtonDeleteGridLookup();
                    this.ValidateControl();
                    this.LoadDataToComboPtttTemp();
                    //LoadWarningNotPTTTGROUP();
                    FillDefaultEkipUser();
                    ucEkip.FillDataToInformationSurg();
                    this.autoCheckIcd = Int64.Parse(this.AutoCheckIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void DataToComboChuanDoanTD(CustomGridLookUpEditWithFilterMultiColumn cbo, bool isIcdCM = false)
        {
            try
            {
                List<IcdADO> listADO = new List<IcdADO>();
                if (isIcdCM)
                {
                    foreach (var item in dataIcdCms)
                    {
                        IcdADO icd = new IcdADO();
                        icd.ID = item.ID;
                        icd.ICD_CODE = item.ICD_CM_CODE;
                        icd.ICD_NAME = item.ICD_CM_NAME;
                        icd.ICD_NAME_UNSIGN = convertToUnSign3(item.ICD_CM_NAME);
                        listADO.Add(icd);
                    }
                }
                else
                {
                    foreach (var item in dataIcds)
                    {
                        IcdADO icd = new IcdADO();
                        icd.ID = item.ID;
                        icd.ICD_CODE = item.ICD_CODE;
                        icd.ICD_NAME = item.ICD_NAME;
                        icd.ICD_NAME_UNSIGN = convertToUnSign3(item.ICD_NAME);
                        listADO.Add(icd);
                    }
                }

                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "ICD_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("ICD_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["ICD_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NumExe(int num)
        {
            try
            {
                txtNumExecute.Text = num.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDefaultEkipUser()
        {
            try
            {

                CommonParam param = new CommonParam();
                List<HisEkipUserADO> ekipUserAdos = new List<HisEkipUserADO>();
                if (this.currentServiceADO.EKIP_ID.HasValue)
                {
                    HisEkipUserViewFilter hisEkipUserFilter = new HisEkipUserViewFilter();
                    hisEkipUserFilter.EKIP_ID = this.currentServiceADO.EKIP_ID;
                    hisEkipUserFilter.IS_ACTIVE = 1;
                    var lstEkipUser = new BackendAdapter(param).Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, hisEkipUserFilter, param);
                    if (lstEkipUser != null && lstEkipUser.Count > 0)
                    {
                        ekipUserAdos = new List<HisEkipUserADO>();
                        foreach (var item in lstEkipUser)
                        {
                            var dataCheck = lstExecuteRole.FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                            if (dataCheck == null || dataCheck.ID == 0)
                                continue;

                            HisEkipUserADO ekipAdo = new HisEkipUserADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HisEkipUserADO>(ekipAdo, item);

                            if (ekipUserAdos.Count == 0)
                            {
                                ekipAdo.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            }
                            else
                            {
                                ekipAdo.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                            }

                            SetDepartment(ekipAdo);
                            ekipUserAdos.Add(ekipAdo);
                        }
                        ekipUserAdos.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                        ekipUserAdos.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        ucEkip.FillDataToGrid(ekipUserAdos);
                    }
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
                    return;
                }
                Inventec.Common.Logging.LogSystem.Warn("data.DEPARTMENT_ID.NULL_____________");
                if (cboDepartment.EditValue != null)
                {
                    data.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());

                }
                else
                {

                    data.DEPARTMENT_ID = null;
                    data.DEPARTMENT_NAME = "";

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Plus_Click(int num)
        {
            try
            {
                txtNumExecute.Text = num.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void Delete_Click(int num)
        {
            try
            {
                txtNumExecute.Text = num.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadComboEkip()
        {
            await InitComboEkip();
            await InitComboDepartment();
        }

        private async Task InitComboEkip()
        {
            try
            {
                var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId;

                string logginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam param = new CommonParam();
                HisEkipTempFilter filter = new HisEkipTempFilter();
                ekipTemps = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>>("api/HisEkipTemp/Get", ApiConsumers.MosConsumer, filter, param);
                if (ekipTemps != null && ekipTemps.Count > 0)
                {
                    ekipTemps = ekipTemps.Where(o => (o.IS_PUBLIC == 1 || o.CREATOR == logginName || (o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == DepartmentID)) && o.IS_ACTIVE == 1).OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EKIP_TEMP_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EKIP_TEMP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboEkipTemp, ekipTemps, controlEditorADO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async Task InitComboDepartment()
        {
            try
            {
                var departmentClinic = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == 1 && o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboDepartment, departmentClinic, controlEditorADO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitUcIcdMain()
        {
            try
            {
                icdMainProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                //ado.DelegateRequiredCause = LoadRequiredCause;
                ado.IsUCCause = false;
                ado.Width = 440;
                ado.Height = 20;
                ado.IsColor = false;
                ado.DataIcds = listIcd;
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ucIcdMain = (UserControl)icdMainProcessor.Run(ado);

                if (ucIcdMain != null)
                {
                    this.panelControlIcdMain.Controls.Add(ucIcdMain);
                    ucIcdMain.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcdBefore()
        {
            try
            {
                icdBeforeProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusIcdAfter;
                //ado.DelegateRequiredCause = LoadRequiredCause;
                ado.IsUCCause = false;
                ado.Width = 440;
                ado.Height = 20;
                ado.IsColor = false;
                ado.DataIcds = listIcd;
                ado.LblIcdMain = "CĐ trước:";
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ucIcdBefore = (UserControl)icdBeforeProcessor.Run(ado);

                if (ucIcdBefore != null)
                {
                    this.panelControlIcdBefore.Controls.Add(ucIcdBefore);
                    ucIcdBefore.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcdAfter()
        {
            try
            {
                icdAfterProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusGroupCode;
                //ado.DelegateRequiredCause = LoadRequiredCause;
                ado.IsUCCause = false;
                ado.Width = 440;
                ado.Height = 20;
                ado.IsColor = false;
                ado.DataIcds = listIcd;
                ado.LblIcdMain = "CĐ sau:";
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ucIcdAfter = (UserControl)icdAfterProcessor.Run(ado);

                if (ucIcdAfter != null)
                {
                    this.panelControlIcdAfter.Controls.Add(ucIcdAfter);
                    ucIcdAfter.Dock = DockStyle.Fill;
                }
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
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ phụ";
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
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

        private void NextForcusIcdAfter()
        {
            try
            {
                if (ucIcdAfter != null && ucIcdAfter.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucSecondaryIcd);
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
                        icdAfterProcessor.FocusControl(ucIcdAfter);
                    }
                    //else
                    //{
                    //    NextForcusOut();
                    //}
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

        private void NextForcusOut()
        {
            try
            {
                if (ucIcdBefore != null && ucIcdBefore.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucIcdBefore);
                    int count = 0;
                    foreach (var itemCtrl in ModuleControls)
                    {
                        if (itemCtrl.ControlName == "txtIcdCode")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "chkEditIcd")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "cboIcds")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "txtIcdMainText")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                    }
                    if (txtIcdCmCode.Visible)
                    {
                        txtIcdCmCode.Focus();
                        txtIcdCmCode.SelectAll();
                    }
                    else if (count > 0)
                    {
                        icdBeforeProcessor.FocusControl(ucIcdBefore);
                        //icdBeforeProcessor.FocusControl(ucIcdBefore, HIS.UC.Icd.ADO.Template.NoFocus);
                    }
                    else
                    {
                        NextForcusOutBefore();
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

        private void NextForcusOutBefore()
        {
            try
            {
                if (ucIcdAfter != null && ucIcdAfter.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucIcdAfter);
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
                        icdAfterProcessor.FocusControl(ucIcdAfter);
                    }
                    //else
                    //{
                    //    //NextForcusSubIcdToDo();
                    //}
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

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.icdMainProcessor != null && this.ucIcdMain != null)
                {
                    var icdValue = this.icdMainProcessor.GetValue(this.ucIcdMain);
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

        private void LoadRequiredCause(bool isRequired)
        {
            try
            {
                if (this.IcdCauseProcessor != null && this.ucIcdCause != null)
                {
                    this.IcdCauseProcessor.SetRequired(this.ucIcdCause, isRequired, HIS.UC.Icd.ADO.Template.NoFocus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusGroupCode()
        {
            try
            {
                if (ucSecondaryIcd != null && ucSecondaryIcd.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucSecondaryIcd);
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
                        SendKeys.Send("{TAB}");
                    }
                    //else
                    //{
                    //    NextForcusOut();
                    //}
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

        private void NextForcusSubIcd()
        {
            try
            {
                if (ucSecondaryIcd != null && ucSecondaryIcd.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucSecondaryIcd);
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
                        subIcdProcessor.FocusControl(ucSecondaryIcd);
                    }
                    else
                    {
                        txtIcdCmCode.Focus();
                        txtIcdCmCode.SelectAll();
                        //NextForcusOut();
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

        private void LoadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = serviceReq.TREATMENT_ID;
                this.vhisTreatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadWarningNotPTTTGROUP()
        {
            CommonParam param = new CommonParam();
            HisServiceFilter filter = new HisServiceFilter();
            filter.ID = this.currentServiceADO.SERVICE_ID;
            var services = new Inventec.Common.Adapter.BackendAdapter
                (param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE>>
                (HisRequestUriStore.HIS_SERVICE_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            if (services != null && services.Count > 0)
            {
                if (services.FirstOrDefault().PTTT_GROUP_ID == null)
                {
                    WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.
                   Show("Dịch vụ " + services.FirstOrDefault().SERVICE_NAME + " chưa có thông tin nhóm PTTT", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                        return;
                }
            }
        }

        private void SetEnableControl()
        {
            try
            {
                if (this.currentServiceADO != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetEnableControl.1");
                    if ((this.currentServiceADO.IS_NO_EXECUTE != null || this.serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT) || this.vhisTreatment.IS_PAUSE == 1)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetEnableControl.2");
                        if (this.isAllowEditInfo)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("SetEnableControl.3");
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("SetEnableControl.4");

                            this.icdMainProcessor.ReadOnly(ucIcdMain, true);
                            this.icdBeforeProcessor.ReadOnly(ucIcdBefore, true);
                            this.icdAfterProcessor.ReadOnly(ucIcdAfter, true);
                            this.subIcdProcessor.ReadOnly(ucSecondaryIcd, true);
                            txtPtttGroupCode.ReadOnly = true;
                            cbbPtttGroup.ReadOnly = true;
                            txtMethodCode.ReadOnly = true;
                            cboMethod.ReadOnly = true;
                            txtBlood.ReadOnly = true;
                            cbbBlood.ReadOnly = true;
                            txtEmotionlessMethod.ReadOnly = true;
                            cbbEmotionlessMethod.ReadOnly = true;
                            txtCondition.ReadOnly = true;
                            cboCondition.ReadOnly = true;
                            txtBloodRh.ReadOnly = true;
                            cbbBloodRh.ReadOnly = true;
                            txtCatastrophe.ReadOnly = true;
                            cboCatastrophe.ReadOnly = true;
                            txtDeathSurg.ReadOnly = true;
                            cboDeathSurg.ReadOnly = true;
                            //dtStart.ReadOnly = true;
                            //dtFinish.ReadOnly = true;
                            txtMANNER.ReadOnly = true;
                            //txtDescription.ReadOnly = true;
                            //txtConclude.ReadOnly = true;
                            //txtResultNote.ReadOnly = true;
                            cbbPtttGroup.Properties.Buttons[0].Enabled = false;
                            //dtStart.Properties.Buttons[0].Enabled = false;
                            //dtFinish.Properties.Buttons[0].Enabled = false;
                            cbbPtttGroup.Properties.Buttons[1].Enabled = false;
                            //dtStart.Properties.Buttons[1].Enabled = false;
                            //dtFinish.Properties.Buttons[1].Enabled = false;

                            btnSave.Enabled = false;
                        }
                    }
                    else if (!String.IsNullOrWhiteSpace(HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.CheckPermisson) && HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.CheckPermisson.Contains("1"))
                    {
                        btnSave.Enabled = true;

                        txtPtttGroupCode.ReadOnly = false;
                        cbbPtttGroup.ReadOnly = false;
                        cbbPtttGroup.Properties.Buttons[0].Enabled = true;
                        cbbPtttGroup.Properties.Buttons[1].Enabled = true;

                        //#17292
                        bool isDoctor = false;
                        var _employee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(p => p.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                        if (_employee != null && _employee.IS_DOCTOR == (short)1)
                        {
                            isDoctor = true;
                        }

                        if (isDoctor)
                        {
                            txtMANNER.ReadOnly = false;
                            //txtDescription.ReadOnly = false;
                            //txtConclude.ReadOnly = false;
                            //txtResultNote.ReadOnly = false;

                            txtMachineCode.ReadOnly = true;
                            cboMachine.ReadOnly = true;
                            txtLoaiPT.ReadOnly = true;
                            cboLoaiPT.ReadOnly = true;
                            txtBanMoCode.ReadOnly = true;
                            cboBanMo.ReadOnly = true;
                            txtPhuongPhap2.ReadOnly = true;
                            cboPhuongPhap2.ReadOnly = true;
                            cboPhuongPhapThucTe.ReadOnly = true;
                            txtPhuongPhapTT.ReadOnly = true;
                            txtKQVoCam.ReadOnly = true;
                            cboKQVoCam.ReadOnly = true;
                            txtMoKTCao.ReadOnly = true;
                            cboMoKTCao.ReadOnly = true;

                            txtMethodCode.ReadOnly = true;
                            cboMethod.ReadOnly = true;
                            txtBlood.ReadOnly = true;
                            cbbBlood.ReadOnly = true;
                            txtEmotionlessMethod.ReadOnly = true;
                            cbbEmotionlessMethod.ReadOnly = true;
                            txtCondition.ReadOnly = true;
                            cboCondition.ReadOnly = true;
                            txtBloodRh.ReadOnly = true;
                            cbbBloodRh.ReadOnly = true;
                            txtCatastrophe.ReadOnly = true;
                            cboCatastrophe.ReadOnly = true;
                            txtDeathSurg.ReadOnly = true;
                            cboDeathSurg.ReadOnly = true;
                            //dtStart.ReadOnly = true;
                            //dtFinish.ReadOnly = true;
                            //dtStart.Properties.Buttons[0].Enabled = false;
                            //dtFinish.Properties.Buttons[0].Enabled = false;
                            //dtStart.Properties.Buttons[1].Enabled = false;
                            //dtFinish.Properties.Buttons[1].Enabled = false;
                            this.icdMainProcessor.ReadOnly(ucIcdMain, true);
                            this.icdBeforeProcessor.ReadOnly(ucIcdBefore, true);
                            this.icdAfterProcessor.ReadOnly(ucIcdAfter, true);
                            this.subIcdProcessor.ReadOnly(ucSecondaryIcd, true);
                        }
                        else
                        {
                            txtMANNER.ReadOnly = true;
                            //txtDescription.ReadOnly = true;
                            //txtConclude.ReadOnly = true;
                            //txtResultNote.ReadOnly = true;

                            txtMachineCode.ReadOnly = false;
                            cboMachine.ReadOnly = false;
                            txtLoaiPT.ReadOnly = false;
                            cboLoaiPT.ReadOnly = false;
                            txtBanMoCode.ReadOnly = false;
                            cboBanMo.ReadOnly = false;
                            txtPhuongPhap2.ReadOnly = false;
                            cboPhuongPhap2.ReadOnly = false;
                            txtPhuongPhapTT.ReadOnly = false;
                            cboPhuongPhapThucTe.ReadOnly = false;
                            txtKQVoCam.ReadOnly = false;
                            cboKQVoCam.ReadOnly = false;
                            txtMoKTCao.ReadOnly = false;
                            cboMoKTCao.ReadOnly = false;

                            txtMethodCode.ReadOnly = false;
                            cboMethod.ReadOnly = false;
                            txtBlood.ReadOnly = false;
                            cbbBlood.ReadOnly = false;
                            txtEmotionlessMethod.ReadOnly = false;
                            cbbEmotionlessMethod.ReadOnly = false;
                            txtCondition.ReadOnly = false;
                            cboCondition.ReadOnly = false;
                            txtBloodRh.ReadOnly = false;
                            cbbBloodRh.ReadOnly = false;
                            txtCatastrophe.ReadOnly = false;
                            cboCatastrophe.ReadOnly = false;
                            txtDeathSurg.ReadOnly = false;
                            cboDeathSurg.ReadOnly = false;
                            //dtStart.ReadOnly = false;
                            //dtFinish.ReadOnly = false;
                            //dtStart.Properties.Buttons[0].Enabled = true;
                            //dtFinish.Properties.Buttons[0].Enabled = true;
                            //dtStart.Properties.Buttons[1].Enabled = true;
                            //dtFinish.Properties.Buttons[1].Enabled = true;
                            this.icdMainProcessor.ReadOnly(ucIcdMain, false);
                            this.icdBeforeProcessor.ReadOnly(ucIcdBefore, false);
                            this.icdAfterProcessor.ReadOnly(ucIcdAfter, false);
                            this.subIcdProcessor.ReadOnly(ucSecondaryIcd, false);
                        }
                    }
                    else
                    {
                        txtPtttGroupCode.ReadOnly = false;
                        cbbPtttGroup.ReadOnly = false;
                        txtMethodCode.ReadOnly = false;
                        cboMethod.ReadOnly = false;
                        txtBlood.ReadOnly = false;
                        cbbBlood.ReadOnly = false;
                        txtEmotionlessMethod.ReadOnly = false;
                        cbbEmotionlessMethod.ReadOnly = false;
                        txtCondition.ReadOnly = false;
                        cboCondition.ReadOnly = false;
                        txtBloodRh.ReadOnly = false;
                        cbbBloodRh.ReadOnly = false;
                        txtCatastrophe.ReadOnly = false;
                        cboCatastrophe.ReadOnly = false;
                        txtDeathSurg.ReadOnly = false;
                        cboDeathSurg.ReadOnly = false;
                        //dtStart.ReadOnly = false;
                        //dtFinish.ReadOnly = false;
                        txtMANNER.ReadOnly = false;
                        //txtDescription.ReadOnly = false;
                        //txtConclude.ReadOnly = false;
                        //txtResultNote.ReadOnly = false;
                        btnSave.Enabled = true;

                        cbbPtttGroup.Properties.Buttons[0].Enabled = true;
                        //dtStart.Properties.Buttons[0].Enabled = true;
                        //dtFinish.Properties.Buttons[0].Enabled = true;
                        cbbPtttGroup.Properties.Buttons[1].Enabled = true;
                        //dtStart.Properties.Buttons[1].Enabled = true;
                        //dtFinish.Properties.Buttons[1].Enabled = true;
                        this.icdMainProcessor.ReadOnly(ucIcdMain, false);
                        this.icdBeforeProcessor.ReadOnly(ucIcdBefore, false);
                        this.icdAfterProcessor.ReadOnly(ucIcdAfter, false);
                        this.subIcdProcessor.ReadOnly(ucSecondaryIcd, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataControlBySereServPttt()
        {
            try
            {

                //this.refreshControl();

                this.ComboMethodPTTT();
                this.ComboPTTTGroup();
                this.ComboEmotionlessMothod();
                this.ComboBlood();//Nhóm máu
                this.ComboBloodRh();//Nhóm máu RH
                this.ComboPtttCondition();//Tình hình Pttt
                this.ComboCatastrophe();//Tai biến trong PTTT
                this.ComboDeathWithin();//Tử vong trong PTTT

                this.ComboLoaiPT();
                this.ComboPhuongPhap2();
                this.ComboKQVoCam();
                this.ComboMoKTCao();
                this.ComboHisMachine();
                this.LoadComboPtttTable(cboBanMo);
                this.ComboPhuongPhapThucTe();

                //if (this.sereServPTTT != null && this.sereServPTTT.ID > 0)
                if (this.sereServPTTT != null)
                {
                    FillDataToCboIcdCm(txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm, this.sereServPTTT.ICD_CM_CODE, this.sereServPTTT.ICD_CM_NAME);
                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_CM_SUB_CODE))
                    {
                        this.txtIcdCmSubCode.Text = this.sereServPTTT.ICD_CM_SUB_CODE;
                    }
                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_CM_TEXT))
                    {
                        this.txtIcdCmSubName.Text = this.sereServPTTT.ICD_CM_TEXT;
                    }
                    if (!String.IsNullOrEmpty(this.sereServPTTT.ICD_CODE) && !String.IsNullOrEmpty(this.sereServPTTT.ICD_NAME))
                    {
                        this.LoadDefaultIcd(icdMainProcessor, ucIcdMain, this.sereServPTTT.ICD_CODE, this.sereServPTTT.ICD_NAME);
                    }

                    if (!String.IsNullOrEmpty(this.sereServPTTT.BEFORE_PTTT_ICD_CODE) && !String.IsNullOrEmpty(this.sereServPTTT.BEFORE_PTTT_ICD_NAME))
                    {
                        this.LoadDefaultIcd(icdBeforeProcessor, ucIcdBefore, this.sereServPTTT.BEFORE_PTTT_ICD_CODE, this.sereServPTTT.BEFORE_PTTT_ICD_NAME);
                    }

                    if (!String.IsNullOrEmpty(this.sereServPTTT.AFTER_PTTT_ICD_CODE) && !String.IsNullOrEmpty(this.sereServPTTT.AFTER_PTTT_ICD_NAME))
                    {
                        this.LoadDefaultIcd(icdAfterProcessor, ucIcdAfter, this.sereServPTTT.AFTER_PTTT_ICD_CODE, this.sereServPTTT.AFTER_PTTT_ICD_NAME);
                    }

                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_SUB_CODE) && !string.IsNullOrEmpty(this.sereServPTTT.ICD_TEXT))
                    {
                        HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();

                        subIcd.ICD_SUB_CODE = this.sereServPTTT.ICD_SUB_CODE;
                        subIcd.ICD_TEXT = this.sereServPTTT.ICD_TEXT;
                        if (ucSecondaryIcd != null)
                        {
                            subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                        }
                    }

                    //if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_SUB_CODE))
                    //{
                    //    this.txtIcdExtraCode.Text = this.sereServPTTT.ICD_SUB_CODE;
                    //}
                    //if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_TEXT))
                    //{
                    //    this.txtIcdText.Text = this.sereServPTTT.ICD_TEXT;
                    //}

                    this.txtMANNER.Text = this.sereServPTTT.MANNER;
                }
                else
                {
                    if (this.currentServiceADO != null && !this.currentServiceADO.EKIP_ID.HasValue)
                    {
                        this.txtMANNER.Text = this.currentServiceADO.TDL_SERVICE_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultIcd(IcdProcessor processor, UserControl ucIcd, string code, string name)
        {
            try
            {
                if (ucIcd != null)
                {
                    HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                    Inventec.Common.Logging.LogSystem.Debug("!!!!!!!!!");
                    icd.ICD_CODE = code;
                    icd.ICD_NAME = name;
                    processor.Reload(ucIcd, icd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadPtttGroupCode(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbPtttGroup.Focus();
                    cbbPtttGroup.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().Where(o => o.PTTT_GROUP_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbPtttGroup.EditValue = data[0].ID;
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                            txtLoaiPT.Focus();
                            txtLoaiPT.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_GROUP_CODE == searchCode);
                            if (search != null)
                            {
                                cbbPtttGroup.EditValue = search.ID;
                                cbbPtttGroup.Properties.Buttons[1].Visible = true;
                                txtLoaiPT.Focus();
                                txtLoaiPT.SelectAll();
                            }
                            else
                            {
                                cbbPtttGroup.EditValue = null;
                                cbbPtttGroup.Focus();
                                cbbPtttGroup.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cbbPtttGroup.EditValue = null;
                        cbbPtttGroup.Focus();
                        cbbPtttGroup.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPtttGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cbbPtttGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbPtttGroup.Text != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_GROUP data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbPtttGroup.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                            txtPtttGroupCode.Text = data.PTTT_GROUP_CODE;
                            txtLoaiPT.Focus();
                            txtLoaiPT.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_GROUP data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbPtttGroup.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPtttGroupCode.Text = data.PTTT_GROUP_CODE;
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                            txtLoaiPT.Focus();
                            txtLoaiPT.SelectAll();
                        }
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

        private void cbbEmotionlessMethod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbEmotionlessMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            txtKQVoCam.Focus();
                            txtKQVoCam.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbEmotionlessMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            txtKQVoCam.Focus();
                            txtKQVoCam.SelectAll();
                        }
                    }
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
                    cbbPtttGroup.Properties.Buttons[1].Visible = false;
                    cbbPtttGroup.EditValue = null;
                    txtPtttGroupCode.Text = "";
                    txtPtttGroupCode.Focus();
                    txtPtttGroupCode.SelectAll();
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
                    cbbEmotionlessMethod.Properties.Buttons[1].Visible = false;
                    cbbEmotionlessMethod.EditValue = null;
                    txtEmotionlessMethod.Text = "";
                    txtEmotionlessMethod.Focus();
                    txtEmotionlessMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadLoaiPT(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboLoaiPT.Focus();
                    cboLoaiPT.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().Where(o => o.PTTT_PRIORITY_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboLoaiPT.EditValue = data[0].ID;
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                            //txtMethodCode.Focus();
                            //txtMethodCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_PRIORITY_CODE == searchCode);
                            if (search != null)
                            {
                                cboLoaiPT.EditValue = search.ID;
                                cboLoaiPT.Properties.Buttons[1].Visible = true;
                                //txtMethodCode.Focus();
                                //txtMethodCode.SelectAll();
                            }
                            else
                            {
                                cboLoaiPT.EditValue = null;
                                cboLoaiPT.Focus();
                                cboLoaiPT.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboLoaiPT.EditValue = null;
                        cboLoaiPT.Focus();
                        cboLoaiPT.ShowPopup();
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
                    cboLoaiPT.Properties.Buttons[1].Visible = false;
                    cboLoaiPT.EditValue = null;
                    txtLoaiPT.Text = "";
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
                    if (cboLoaiPT.Text != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_PRIORITY data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiPT.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                            txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                            //txtMethodCode.Focus();
                            //txtMethodCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_PRIORITY data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiPT.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                            //txtMethodCode.Focus();
                            //txtMethodCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonDeleteGridLookup(GridLookUpEdit control)
        {
            try
            {
                if (control.EditValue != null)
                {
                    control.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    control.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonDeleteLookup(LookUpEdit control)
        {
            try
            {
                if (control.EditValue != null)
                {
                    control.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    control.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboExecuteRole(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                data = data.Where(dt => dt.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<AcsUserADO> acsUserAlows = new List<AcsUserADO>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = AcsUserADOList.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }
                else
                {
                    acsUserAlows = AcsUserADOList;
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboAcsUser()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(p => p.IS_ACTIVE == 1).ToList();
                var employeeList = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == 1).ToList();
                var departmentList = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList();
                AcsUserADOList = new List<AcsUserADO>();

                foreach (var item in datas)
                {
                    AcsUserADO user = new AcsUserADO();
                    user.ID = item.ID;
                    user.LOGINNAME = item.LOGINNAME;
                    user.USERNAME = item.USERNAME;
                    user.MOBILE = item.MOBILE;
                    user.PASSWORD = item.PASSWORD;

                    var check = employeeList.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                    if (check != null)
                    {
                        var checkDepartment = departmentList.FirstOrDefault(o => o.ID == check.DEPARTMENT_ID);
                        if (checkDepartment != null)
                        {
                            user.DEPARTMENT_NAME = checkDepartment.DEPARTMENT_NAME;
                        }
                    }
                    AcsUserADOList.Add(user);
                }

                AcsUserADOList = AcsUserADOList.OrderBy(o => o.USERNAME).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tài khoản", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 250, 2));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên khoa", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 750);
                Inventec.Common.Logging.LogSystem.Debug("AcsUserADOList count: " + (AcsUserADOList != null ? AcsUserADOList.Count : 0));
                ControlEditorLoader.Load(cbo_UseName, AcsUserADOList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo, long? deparmentId)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                var employeeList = BackendDataWorker.Get<V_HIS_EMPLOYEE>();
                var department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == deparmentId).FirstOrDefault();
                List<AcsUserADO> AcsUserADOList = new List<AcsUserADO>();
                var check = employeeList.Where(o => o.DEPARTMENT_ID == deparmentId || o.DEPARTMENT_ID == null).ToList();
                if (check != null && check.Count > 0)
                {
                    foreach (var item in check)
                    {
                        AcsUserADO user = new AcsUserADO();
                        var userData = data.Where(o => o.LOGINNAME == item.LOGINNAME).FirstOrDefault();
                        if (userData != null)
                        {
                            user.ID = userData.ID;
                            user.LOGINNAME = userData.LOGINNAME;
                            user.USERNAME = userData.USERNAME;
                            user.MOBILE = userData.MOBILE;
                            user.PASSWORD = userData.PASSWORD;
                            user.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            AcsUserADOList.Add(user);
                        }

                    }
                }

                AcsUserADOList = AcsUserADOList.OrderBy(o => o.USERNAME).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tài khoản", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 250, 2));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên khoa", 200, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 600);
                ControlEditorLoader.Load(cbo, AcsUserADOList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetSereServPtttBySereServId()
        {
            try
            {
                if (this.sereServPTTT == null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServPtttFilter hisSereServPtttFilter = new HisSereServPtttFilter();
                    hisSereServPtttFilter.SERE_SERV_ID = this.currentServiceADO.ID;
                    var hisSereServPttts = new BackendAdapter(param)
                      .Get<List<HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/Get", ApiConsumers.MosConsumer, hisSereServPtttFilter, param);
                    this.sereServPTTT = (hisSereServPttts != null && hisSereServPttts.Count > 0) ? hisSereServPttts.FirstOrDefault() : null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.sereServPTTT), this.sereServPTTT));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultCboPTTTGroupOnly()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.1");
                if (this.currentServiceADO.EKIP_ID == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.2");
                    long ptttGroupId = 0;

                    var surgMisuService = lstService.FirstOrDefault(o =>
                        o.ID == this.currentServiceADO.SERVICE_ID);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentServiceADO.SERVICE_ID), this.currentServiceADO.SERVICE_ID) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => surgMisuService), surgMisuService));
                    if (surgMisuService != null && surgMisuService.PTTT_GROUP_ID.HasValue)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.3");
                        HIS_PTTT_GROUP ptttGroup = BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == surgMisuService.PTTT_GROUP_ID);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ptttGroup), ptttGroup));
                        ptttGroupId = ptttGroup.ID;
                        txtPtttGroupCode.Text = ptttGroup.PTTT_GROUP_CODE;
                        cbbPtttGroup.Properties.Buttons[1].Visible = true;
                    }

                    if (ptttGroupId > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.3.1");
                        cbbPtttGroup.EditValue = ptttGroupId;
                        cbbPtttGroup.Enabled = false;
                        txtPtttGroupCode.Enabled = false;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.3.2");
                        cbbPtttGroup.EditValue = null;
                        cbbPtttGroup.Enabled = true;
                        txtPtttGroupCode.Enabled = true;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.4");
                    var surgService = lstService.FirstOrDefault(o => o.ID == this.currentServiceADO.SERVICE_ID);
                    if (surgService != null && surgService.PTTT_GROUP_ID != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.5");
                        HIS_PTTT_GROUP ptttGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == surgService.PTTT_GROUP_ID);
                        cbbPtttGroup.EditValue = ptttGroup.ID;
                        txtPtttGroupCode.Text = ptttGroup.PTTT_GROUP_CODE;
                        cbbPtttGroup.Properties.Buttons[1].Visible = true;
                        cbbPtttGroup.Enabled = false;
                        txtPtttGroupCode.Enabled = false;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Lay PTTT_GROUP_ID mac dinh theo dich vu khong co du lieu____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => surgService), surgService));
                    }
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
                HisSurgServiceReqUpdateSDO hisSurgResultSDO = new MOS.SDO.HisSurgServiceReqUpdateSDO();
                bool valid = true;
                this.positionHandle = -1;
                valid = valid && dxValidationProvider1.Validate();
                //valid = valid && (this.isAllowEditInfo ? this.ValidStartDatePTTT(ref hisSurgResultSDO) : true);
                valid = valid && (this.currentServiceADO != null);
                if (valid)
                {
                    //CheckEkipWithKey();
                    SaveProcessor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcessor()
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                HisSereServExtSDO data = new HisSereServExtSDO();
                data.HisSereServExt = this.sereServExt;
                data.HisEkipUsers = new List<HIS_EKIP_USER>();
                var ekipUserCheck = ProcessEkipUser(ref data);
                if (!ekipUserCheck)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu ekip trùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if(data.HisEkipUsers != null && data.HisEkipUsers.Count > 0)
                {
                    var executeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                    var groupLogin = from p in data.HisEkipUsers
                                     group p by new
                                     {
                                         p.LOGINNAME
                                     } into g
                                     select new { Key = g.Key, CareDetail = g.ToList() };
                    if (groupLogin != null && groupLogin.Count() > 0)
                    {
                        List<string> loginListName = new List<string>();
                        foreach (var item in groupLogin)
                        {
                            if (item.CareDetail.Count > 1)
                            {
                                loginListName.Add(string.Format("Tài khoản {0} đang được nhập nhiều hơn 1 vai trò ({1}).", item.CareDetail.First().LOGINNAME, string.Join(", ", executeRole.Where(o => item.CareDetail.Select(p => p.EXECUTE_ROLE_ID).ToList().Exists(k => k == o.ID)).Select(o => o.EXECUTE_ROLE_NAME).ToList())));
                            }
                        }
                        if (loginListName != null && loginListName.Count > 0)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Join("\r\n", loginListName),
                           ResourceMessage.ThongBao,
                           MessageBoxButtons.OK);
                            return;
                        }
                    }


                    var lstEkipUser = data.HisEkipUsers;
                    var groupExecuteRole = from p in lstEkipUser
                                               group p by new
                                               {
                                                   p.EXECUTE_ROLE_ID
                                               } into g
                                               select new { Key = g.Key, CareDetail = g.ToList() };
                    if (groupExecuteRole != null && groupExecuteRole.Count() > 0)
                    {
                            List<string> roleListName = new List<string>();
                            foreach (var item in groupExecuteRole)
                            {
                                var checkRole = executeRole.FirstOrDefault(o => o.ID == item.CareDetail.First().EXECUTE_ROLE_ID);
                                if (item.CareDetail.Count > 1 && checkRole != null && checkRole.IS_SINGLE_IN_EKIP == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    roleListName.Add(checkRole.EXECUTE_ROLE_NAME);
                                }
                            }
                            if (roleListName != null && roleListName.Count > 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Không cho phép nhập nhiều hơn 1 tài khoản đối với vai trò {0}.", string.Join(", ",roleListName)),
                               ResourceMessage.ThongBao,
                               MessageBoxButtons.OK);
                                return;
                            }
                    }
                   
                }
                ProcessSereServPtttForSave();
                data.HisSereServPttt = this.sereServPTTT;
                ProcessSereServExt();
                data.HisSereServExt = this.sereServExt;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                MOS.SDO.HisSereServExtWithFileSDO apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
                   <MOS.SDO.HisSereServExtWithFileSDO>
                   (this.sereServExt.ID == 0 ?
                   RequestUriStore.HIS_SERE_SERV_EXT_CREATE_SDO :
                   RequestUriStore.HIS_SERE_SERV_EXT_UPDATE_SDO,
                   ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                if (apiResult != null)
                {
                    sereServPTTT = apiResult.SereServPttt;
                    sereServExt = apiResult.SereServExt;
                    List<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER> ekipUsersView = new List<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER>();
                    if (apiResult.EkipUsers != null && apiResult.EkipUsers.Count > 0)
                    {
                        foreach (var item in apiResult.EkipUsers)
                        {
                            MOS.EFMODEL.DataModels.V_HIS_EKIP_USER VekipUser = new V_HIS_EKIP_USER();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EKIP_USER>(VekipUser, item);
                            ekipUsersView.Add(VekipUser);
                        }
                        currentServiceADO.lstEkipUser = ekipUsersView;
                        if (ekipUsersView != null)
                            currentServiceADO.EKIP_ID = apiResult.EkipUsers.First().EKIP_ID;
                        FillDefaultEkipUser();
                    }
                    success = true;
                    actSaveClick(apiResult.SereServPttt, apiResult.SereServExt, currentServiceADO);
                    if (delegateRefresh != null)
                        delegateRefresh();
                    if (chkPrint.Checked || chkPreView.Checked || chkSign.Checked)
                        btnPrint_Click(null, null);
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #region
        private bool ProcessEkipUser(ref HisSereServExtSDO hisSurgResultSDO)
        {
            bool result = true;
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
                var sereServPTTTADOs = ucEkip.GetDataSource();
                //= grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                if (sereServPTTTADOs != null && sereServPTTTADOs.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServPTTTADOs), sereServPTTTADOs));
                    foreach (var item in sereServPTTTADOs)
                    {

                        MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, item);

                        var acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == ekipUser.LOGINNAME);
                        if (acsUser != null)
                        {
                            ekipUser.USERNAME = acsUser.USERNAME;

                            if (currentServiceADO != null && currentServiceADO.EKIP_ID.HasValue)
                            {
                                ekipUser.EKIP_ID = currentServiceADO.EKIP_ID.Value;
                            }
                            //if (item.DEPARTMENT_ID <= 0 ||item.DEPARTMENT_ID == null)
                            //{
                            //    ekipUser.DEPARTMENT_ID = null;
                            //}

                            ekipUsers.Add(ekipUser);

                        }
                    }
                }

                var groupEkipUser = ekipUsers.GroupBy(x => new { x.LOGINNAME, x.EXECUTE_ROLE_ID });
                foreach (var item in groupEkipUser)
                {
                    if (item.Count() >= 2)
                    {
                        return false;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ekipUsers), ekipUsers));
                hisSurgResultSDO.HisEkipUsers = ekipUsers;
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckEkipWithKey()
        {
            try
            {
                bool result = true;
                if (key != "1")
                {
                    if (this.ekipUserAdos != null && this.ekipUserAdos.Count > 0)
                    {
                        if (this.ekipUserAdos.Count == 1)
                        {
                            List<HisEkipUserADO> checkExist = this.ekipUserAdos.Where(o => o.EXECUTE_ROLE_ID == 0 && (o.DEPARTMENT_ID == 0 || o.DEPARTMENT_ID == null) && string.IsNullOrEmpty(o.LOGINNAME)).ToList();
                            List<HisEkipUserADO> checkExistRole = this.ekipUserAdos.Where(o => o.EXECUTE_ROLE_ID != 0 && string.IsNullOrEmpty(o.LOGINNAME)).ToList();
                            if (checkExist != null && checkExist.Count > 0)
                            {
                                WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.
                               Show("Bạn chưa nhập kíp thực hiện!", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                                    result = false;
                            }

                            else if (checkExistRole != null && checkExistRole.Count > 0)
                            {
                                WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.
                               Show("Bạn cần nhập tên cho vai trò", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                                    result = false;
                            }
                        }
                        else
                        {
                            foreach (var item in this.ekipUserAdos)
                            {
                                List<HisEkipUserADO> checkExist = ekipUserAdos.Where(o => o.EXECUTE_ROLE_ID == 0 && (o.DEPARTMENT_ID == 0 || o.DEPARTMENT_ID == null) && string.IsNullOrEmpty(o.LOGINNAME)).ToList();
                                List<HisEkipUserADO> checkExistRole = ekipUserAdos.Where(o => o.EXECUTE_ROLE_ID != 0 && string.IsNullOrEmpty(o.LOGINNAME)).ToList();
                                if (checkExist != null && checkExist.Count > 0)
                                {
                                    WaitingManager.Hide();
                                    if (DevExpress.XtraEditors.XtraMessageBox.
                                   Show("Bạn chưa nhập kíp thực hiện!", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                                        result = false;
                                }
                                else if (checkExistRole != null && checkExistRole.Count > 0)
                                {
                                    WaitingManager.Hide();
                                    if (DevExpress.XtraEditors.XtraMessageBox.
                                   Show("Bạn cần nhập tên cho vai trò " + item.EXECUTE_ROLE_CODE, "Thông báo", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                                        result = false;
                                }
                            }
                        }
                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        void ProcessSereServPtttForSave()
        {
            try
            {
                if (this.sereServPTTT == null)
                {
                    this.sereServPTTT = new HIS_SERE_SERV_PTTT();
                    this.sereServPTTT.SERE_SERV_ID = currentServiceADO.ID;
                    this.sereServPTTT.TDL_TREATMENT_ID = currentServiceADO.TDL_TREATMENT_ID;
                }

                //Phuong phap vô cảm
                this.sereServPTTT.EMOTIONLESS_METHOD_ID = cbbEmotionlessMethod.EditValue != null ? (long?)cbbEmotionlessMethod.EditValue : null;

                //Loai PTTT
                this.sereServPTTT.PTTT_GROUP_ID = cbbPtttGroup.EditValue != null ? (long?)cbbPtttGroup.EditValue : null;

                this.sereServPTTT.PTTT_PRIORITY_ID = cboLoaiPT.EditValue != null ? (long?)cboLoaiPT.EditValue : null;

                if (this.ucSecondaryIcd != null)
                {
                    var subIcd = this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                    if (subIcd != null && subIcd is HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)
                    {
                        this.sereServPTTT.ICD_TEXT = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_TEXT;
                        this.sereServPTTT.ICD_SUB_CODE = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                    }
                }

                // nhom mau
                if (cbbBlood.EditValue != null)
                {
                    this.sereServPTTT.BLOOD_ABO_ID = (long)cbbBlood.EditValue;
                }
                else
                {
                    this.sereServPTTT.BLOOD_ABO_ID = null;
                }
                //Nhom mau RH
                if (cbbBloodRh.EditValue != null)
                {
                    this.sereServPTTT.BLOOD_RH_ID = (long)cbbBloodRh.EditValue;
                }
                else
                {
                    this.sereServPTTT.BLOOD_RH_ID = null;
                }
                this.sereServPTTT.PARTICIPANT_NUMBER = txtNumExecute.Text;
                //Tai bien PTTT
                if (cboCatastrophe.EditValue != null)
                {
                    this.sereServPTTT.PTTT_CATASTROPHE_ID = (long)cboCatastrophe.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_CATASTROPHE_ID = null;
                }
                //Tinh hinh PTTT
                if (cboCondition.EditValue != null)
                {
                    this.sereServPTTT.PTTT_CONDITION_ID = (long)cboCondition.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_CONDITION_ID = null;
                }

                //Phuong phap PTTT
                if (cboMethod.EditValue != null)
                {
                    this.sereServPTTT.PTTT_METHOD_ID = (long)cboMethod.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_METHOD_ID = null;
                }
                //Phuong phap Thuc te
                if (cboPhuongPhapThucTe.EditValue != null)
                {
                    this.sereServPTTT.REAL_PTTT_METHOD_ID = (long)cboPhuongPhapThucTe.EditValue;
                }
                else
                {
                    this.sereServPTTT.REAL_PTTT_METHOD_ID = null;
                }

                if (!String.IsNullOrEmpty(txtMANNER.Text))
                {
                    this.sereServPTTT.MANNER = txtMANNER.Text;
                }

                //Tu vong
                if (cboDeathSurg.EditValue != null)
                {
                    this.sereServPTTT.DEATH_WITHIN_ID = (long)cboDeathSurg.EditValue;
                }
                else
                {
                    this.sereServPTTT.DEATH_WITHIN_ID = null;
                }
                if (cboLoaiPT.EditValue != null)
                {
                    this.sereServPTTT.PTTT_PRIORITY_ID = (long)cboLoaiPT.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_PRIORITY_ID = null;
                }
                if (cboPhuongPhap2.EditValue != null)
                {
                    this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID = (long)cboPhuongPhap2.EditValue;
                }
                else
                {
                    this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID = null;
                }
                if (cboKQVoCam.EditValue != null)
                {
                    this.sereServPTTT.EMOTIONLESS_RESULT_ID = (long)cboKQVoCam.EditValue;
                }
                else
                {
                    this.sereServPTTT.EMOTIONLESS_RESULT_ID = null;
                }
                if (cboMoKTCao.EditValue != null)
                {
                    this.sereServPTTT.PTTT_HIGH_TECH_ID = (long)cboMoKTCao.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_HIGH_TECH_ID = null;
                }
                if (cboBanMo.EditValue != null)
                {
                    this.sereServPTTT.PTTT_TABLE_ID = (long)cboBanMo.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_TABLE_ID = null;
                }

                if (this.ucIcdMain != null)
                {
                    var icdValue = this.icdMainProcessor.GetValue(this.ucIcdMain);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        this.sereServPTTT.ICD_CODE = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        this.sereServPTTT.ICD_NAME = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }

                if (this.ucIcdBefore != null)
                {
                    var icdValue = this.icdMainProcessor.GetValue(this.ucIcdBefore);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        this.sereServPTTT.BEFORE_PTTT_ICD_CODE = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        this.sereServPTTT.BEFORE_PTTT_ICD_NAME = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }

                if (this.ucIcdAfter != null)
                {
                    var icdValue = this.icdMainProcessor.GetValue(this.ucIcdAfter);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        this.sereServPTTT.AFTER_PTTT_ICD_CODE = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        this.sereServPTTT.AFTER_PTTT_ICD_NAME = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }

                if (txtIcdCmName.ErrorText == "")
                {
                    if (chkIcdCm.Checked)
                        sereServPTTT.ICD_CM_NAME = txtIcdCmName.Text;
                    else
                        sereServPTTT.ICD_CM_NAME = cboIcdCmName.Text;

                    sereServPTTT.ICD_CM_CODE = txtIcdCmCode.Text;
                }

                sereServPTTT.ICD_CM_TEXT = txtIcdCmSubName.Text;
                sereServPTTT.ICD_CM_SUB_CODE = txtIcdCmSubCode.Text;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Luu__sereServPTTT___:", sereServPTTT));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSereServExt()
        {
            try
            {
                if (this.currentServiceADO != null)
                {
                    if (this.sereServExt == null)
                    {
                        this.sereServExt = new HIS_SERE_SERV_EXT();
                    }

                    sereServExt.SERE_SERV_ID = this.currentServiceADO.ID;
                    //if (dtStart.EditValue != null)
                    //    sereServExt.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStart.DateTime);
                    //else
                    //    sereServExt.BEGIN_TIME = null;
                    //if (dtFinish.EditValue != null)
                    //    sereServExt.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinish.DateTime);
                    //else
                    //{
                    //    sereServExt.END_TIME = null;
                    //}
                    if (cboMachine.EditValue != null)
                    {
                        sereServExt.MACHINE_ID = (long)cboMachine.EditValue;
                        sereServExt.MACHINE_CODE = this.txtMachineCode.Text;
                    }
                    else
                    {
                        sereServExt.MACHINE_ID = null;
                        sereServExt.MACHINE_CODE = "";
                    }
                    sereServExt.INSTRUCTION_NOTE = txtIntructionNote.Text;
                    sereServExt.DESCRIPTION = txtDescription.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSaveShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
                btnSave_Click(null, null);
        }

        #region --- Event ICD ---

        //private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        //{

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(delegateIcdCodes))
        //        {

        //            txtIcdExtraCode.Text = delegateIcdCodes;

        //        }


        //        if (!string.IsNullOrEmpty(delegateIcdNames))
        //        {

        //            txtIcdText.Text = delegateIcdNames;

        //        }

        //    }

        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Warn(ex);

        //    }

        //}

        //void update(HIS_ICD dataIcd)
        //{
        //    txtIcdText.Text = txtIcdText.Text + dataIcd.ICD_CODE + " - " + dataIcd.ICD_NAME + ", ";
        //}

        //void stringIcds(string delegateIcds)
        //{
        //    if (!string.IsNullOrEmpty(delegateIcds))
        //    {
        //        txtIcdText.Text = delegateIcds;
        //    }
        //}

        //private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        //{
        //    try
        //    {
        //        string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
        //        txtIcdText.Text = processIcdNameChanged(icdName__Olds, icdNames);
        //        if (icdNames.Equals(IcdUtil.seperator))
        //        {
        //            txtIcdText.Text = "";
        //        }
        //        if (icdCodes.Equals(IcdUtil.seperator))
        //        {
        //            txtIcdExtraCode.Text = "";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

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
        #endregion

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

        private void cboMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMethodCode.Text = data.PTTT_METHOD_CODE;
                            cboMethod.Properties.Buttons[1].Visible = true;
                            txtPhuongPhap2.Focus();
                            txtPhuongPhap2.SelectAll();
                        }
                    }
                }
                else
                {
                    cboMethod.ShowPopup();
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMethod.EditValue.ToString()));
                        {
                            txtMethodCode.Text = data.PTTT_METHOD_CODE;
                            cboMethod.Properties.Buttons[1].Visible = true;
                            txtPhuongPhap2.Focus();
                            txtPhuongPhap2.SelectAll();
                        }
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

        private void cbbBlood_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbBlood.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_BLOOD_ABO data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbBlood.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBlood.Text = data.BLOOD_ABO_CODE;
                            cbbBlood.Properties.Buttons[1].Visible = true;
                            txtBloodRh.Focus();
                            txtBloodRh.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_BLOOD_ABO data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbBlood.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBlood.Text = data.BLOOD_ABO_CODE;
                            cbbBlood.Properties.Buttons[1].Visible = true;
                            txtBloodRh.Focus();
                            txtBloodRh.SelectAll();
                        }
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

        private void cboCondition_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCondition.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CONDITION>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCondition.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtCondition.Text = data.PTTT_CONDITION_CODE;
                            cboCondition.Properties.Buttons[1].Visible = true;
                            txtBlood.Focus();
                            txtBlood.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CONDITION>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCondition.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtCondition.Text = data.PTTT_CONDITION_CODE;
                            cboCondition.Properties.Buttons[1].Visible = true;
                            txtBlood.Focus();
                        }
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

        private void cbbBloodRh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbBloodRh.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_BLOOD_RH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbBloodRh.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBloodRh.Text = data.BLOOD_RH_CODE;
                            cbbBloodRh.Properties.Buttons[1].Visible = true;
                            txtCatastrophe.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_BLOOD_RH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbBloodRh.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBloodRh.Text = data.BLOOD_RH_CODE;
                            cbbBloodRh.Properties.Buttons[1].Visible = true;
                            txtCatastrophe.Focus();
                        }
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

        private void cboCatastrophe_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCatastrophe.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CATASTROPHE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCatastrophe.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtCatastrophe.Text = data.PTTT_CATASTROPHE_CODE;
                            cboCatastrophe.Properties.Buttons[1].Visible = true;
                            txtDeathSurg.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CATASTROPHE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCatastrophe.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtCatastrophe.Text = data.PTTT_CATASTROPHE_CODE;
                            cboCatastrophe.Properties.Buttons[1].Visible = true;
                        }
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

        private void cboDeathSurg_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDeathSurg.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDeathSurg.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtDeathSurg.Text = data.DEATH_CAUSE_CODE;
                            cboDeathSurg.Properties.Buttons[1].Visible = true;
                            txtMachineCode.Focus();
                            txtMachineCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDeathSurg.EditValue ?? 0).ToString()));
                        {
                            txtDeathSurg.Text = data.DEATH_CAUSE_CODE;
                            cboDeathSurg.Properties.Buttons[1].Visible = true;
                            txtMachineCode.Focus();
                            txtMachineCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        //private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            //txtConclude.Focus();
        //            //txtConclude.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtConclude_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtResultNote.Focus();
        //            //txtResultNote.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtResultNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            btnSave.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void cboPosition_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LookUpEdit edit = sender as LookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    if ((edit.EditValue ?? 0).ToString() != (edit.OldEditValue ?? 0).ToString())
                    {
                        //grdViewInformationSurg.SetRowCellValue(grdViewInformationSurg.FocusedRowHandle, gridColumnParticipants_Id, edit.EditValue);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMANNER_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (txtIntructionNote.Enabled)
                    //    txtIntructionNote.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void dtFinish_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            dtFinish.Properties.Buttons[1].Visible = true;
        //            txtMANNER.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void dtStart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            dtFinish.Focus();
        //            dtFinish.SelectAll();
        //            dtFinish.ShowPopup();
        //            dtStart.Properties.Buttons[1].Visible = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //}

        private void cboMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMethod.Properties.Buttons[1].Visible = false;
                    cboMethod.EditValue = null;
                    txtMethodCode.Text = "";
                    txtMethodCode.Focus();
                    txtMethodCode.SelectAll();
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
                    cbbBlood.Properties.Buttons[1].Visible = false;
                    cbbBlood.EditValue = null;
                    txtBlood.Text = "";
                    txtBlood.Focus();
                    txtBlood.SelectAll();
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
                    cboCondition.Properties.Buttons[1].Visible = false;
                    cboCondition.EditValue = null;
                    txtCondition.Text = "";
                    txtCondition.Focus();
                    txtCondition.SelectAll();
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
                    cbbBloodRh.Properties.Buttons[1].Visible = false;
                    cbbBloodRh.EditValue = null;
                    txtBloodRh.Text = "";
                    txtBloodRh.Focus();
                    txtBloodRh.SelectAll();
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
                    cboCatastrophe.Properties.Buttons[1].Visible = false;
                    cboCatastrophe.EditValue = null;
                    txtCatastrophe.Text = "";
                    txtCatastrophe.Focus();
                    txtCatastrophe.SelectAll();
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
                    cboDeathSurg.Properties.Buttons[1].Visible = false;
                    cboDeathSurg.EditValue = null;
                    txtDeathSurg.Text = "";
                    txtDeathSurg.Focus();
                    txtDeathSurg.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void dtStart_ButtonClick(object sender, ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Button.Kind == ButtonPredefines.Delete)
        //        {
        //            dtStart.Properties.Buttons[1].Visible = false;
        //            dtStart.EditValue = null;
        //            dtStart.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void dtFinish_ButtonClick(object sender, ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Button.Kind == ButtonPredefines.Delete)
        //        {
        //            dtFinish.Properties.Buttons[1].Visible = false;
        //            dtFinish.EditValue = null;
        //            dtFinish.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void dtFinish_Closed(object sender, ClosedEventArgs e)
        //{
        //    try
        //    {
        //        if (dtFinish.EditValue != null)
        //        {
        //            DateTime dt = dtFinish.DateTime;
        //            dtFinish.DateTime = new DateTime(dt.Year, dt.Month, dt.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        //            dtFinish.Properties.Buttons[1].Visible = true;
        //            txtMANNER.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void dtStart_Closed(object sender, ClosedEventArgs e)
        //{
        //    try
        //    {
        //        if (dtStart.EditValue != null)
        //        {
        //            DateTime dt = dtStart.DateTime;
        //            dtStart.DateTime = new DateTime(dt.Year, dt.Month, dt.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        //            dtStart.Properties.Buttons[1].Visible = true;
        //            dtFinish.Focus();
        //            dtFinish.SelectAll();
        //            dtFinish.ShowPopup();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}


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

        private void LoadPhuongPhap2(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPhuongPhap2.Focus();
                    cboPhuongPhap2.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o =>
                        o.IS_ACTIVE == 1
                        && (o.IS_SECOND == 1 || (o.IS_FIRST != 1 && o.IS_SECOND != 1))
                        && o.EMOTIONLESS_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPhuongPhap2.EditValue = data[0].ID;
                            cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                            txtPhuongPhapTT.Focus();
                            txtPhuongPhapTT.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cboPhuongPhap2.EditValue = search.ID;
                                cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                                txtPhuongPhapTT.Focus();
                                txtPhuongPhapTT.SelectAll();
                            }
                            else
                            {
                                cboPhuongPhap2.EditValue = null;
                                cboPhuongPhap2.Focus();
                                cboPhuongPhap2.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboPhuongPhap2.EditValue = null;
                        cboPhuongPhap2.Focus();
                        cboPhuongPhap2.ShowPopup();
                    }
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
                    cboPhuongPhap2.Properties.Buttons[1].Visible = false;
                    cboPhuongPhap2.EditValue = null;
                    txtPhuongPhap2.Text = "";
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
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPhuongPhap2.EditValue.ToString()));
                        {
                            txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                            cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                            txtPhuongPhapTT.Focus();
                            txtPhuongPhapTT.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhuongPhap2.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                            cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                            txtPhuongPhapTT.Focus();
                            txtPhuongPhapTT.SelectAll();
                        }
                    }
                }
                else
                {
                    cboPhuongPhap2.ShowPopup();
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

        private void LoadKQVoCam(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboKQVoCam.Focus();
                    cboKQVoCam.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.EMOTIONLESS_RESULT_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboKQVoCam.EditValue = data[0].ID;
                            cboKQVoCam.Properties.Buttons[1].Visible = true;
                            txtCondition.Focus();
                            txtCondition.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_RESULT_CODE == searchCode);
                            if (search != null)
                            {
                                cboKQVoCam.EditValue = search.ID;
                                cboKQVoCam.Properties.Buttons[1].Visible = true;
                                txtCondition.Focus();
                                txtCondition.SelectAll();
                            }
                            else
                            {
                                cboKQVoCam.EditValue = null;
                                cboKQVoCam.Focus();
                                cboKQVoCam.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboKQVoCam.EditValue = null;
                        cboKQVoCam.Focus();
                        cboKQVoCam.ShowPopup();
                    }
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
                    cboKQVoCam.Properties.Buttons[1].Visible = false;
                    cboKQVoCam.EditValue = null;
                    txtKQVoCam.Text = "";
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
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_RESULT data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboKQVoCam.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtKQVoCam.Text = data.EMOTIONLESS_RESULT_CODE;
                            cboKQVoCam.Properties.Buttons[1].Visible = true;
                            txtCondition.Focus();
                            txtCondition.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_RESULT data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboKQVoCam.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtKQVoCam.Text = data.EMOTIONLESS_RESULT_CODE;
                            cboKQVoCam.Properties.Buttons[1].Visible = true;
                            txtCondition.Focus();
                            txtCondition.SelectAll();
                        }
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

        private void LoadMoKTCao(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMoKTCao.Focus();
                    cboMoKTCao.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.PTTT_HIGH_TECH_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMoKTCao.EditValue = data[0].ID;
                            cboMoKTCao.Properties.Buttons[1].Visible = true;
                            txtMANNER.Focus();
                            txtMANNER.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_HIGH_TECH_CODE == searchCode);
                            if (search != null)
                            {
                                cboMoKTCao.EditValue = search.ID;
                                cboMoKTCao.Properties.Buttons[1].Visible = true;
                                txtMANNER.Focus();
                                txtMANNER.SelectAll();
                            }
                            else
                            {
                                cboMoKTCao.EditValue = null;
                                cboMoKTCao.Focus();
                                cboMoKTCao.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboMoKTCao.EditValue = null;
                        cboMoKTCao.Focus();
                        cboMoKTCao.ShowPopup();
                    }
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
                    cboMoKTCao.Properties.Buttons[1].Visible = false;
                    cboMoKTCao.EditValue = null;
                    txtMoKTCao.Text = "";
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_HIGH_TECH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMoKTCao.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMoKTCao.Text = data.PTTT_HIGH_TECH_CODE;
                            cboMoKTCao.Properties.Buttons[1].Visible = true;
                            txtMANNER.Focus();
                            txtMANNER.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_HIGH_TECH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMoKTCao.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMoKTCao.Text = data.PTTT_HIGH_TECH_CODE;
                            cboMoKTCao.Properties.Buttons[1].Visible = true;
                            txtMANNER.Focus();
                            txtMANNER.SelectAll();
                        }
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

        private void LoadMachine(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMachine.Focus();
                    cboMachine.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.MACHINE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMachine.EditValue = data[0].ID;
                            cboMachine.Properties.Buttons[1].Visible = true;
                            txtMoKTCao.Focus();
                            txtMoKTCao.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.MACHINE_CODE == searchCode);
                            if (search != null)
                            {
                                cboMachine.EditValue = search.ID;
                                cboMachine.Properties.Buttons[1].Visible = true;
                                txtMoKTCao.Focus();
                                txtMoKTCao.SelectAll();
                            }
                            else
                            {
                                cboMachine.EditValue = null;
                                cboMachine.Focus();
                                cboMachine.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboMachine.EditValue = null;
                        cboMachine.Focus();
                        cboMachine.ShowPopup();
                    }
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
                    cboMachine.Properties.Buttons[1].Visible = false;
                    cboMachine.EditValue = null;
                    txtMachineCode.Text = "";
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
                        MOS.EFMODEL.DataModels.HIS_MACHINE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMachine.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMachineCode.Text = data.MACHINE_CODE;
                            cboMachine.Properties.Buttons[1].Visible = true;
                            txtMoKTCao.Focus();
                            txtMoKTCao.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_MACHINE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMachine.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMachineCode.Text = data.MACHINE_CODE;
                            cboMachine.Properties.Buttons[1].Visible = true;
                            txtMoKTCao.Focus();
                            txtMoKTCao.SelectAll();
                        }
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

        private void LoadBanMo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboBanMo.Focus();
                    cboBanMo.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_TABLE>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.PTTT_TABLE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            txtBanMoCode.Text = data[0].PTTT_TABLE_CODE;
                            cboBanMo.EditValue = data[0].ID;
                            cboBanMo.Properties.Buttons[1].Visible = true;
                            txtMethodCode.Focus();
                            txtMethodCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_TABLE_CODE == searchCode);
                            if (search != null)
                            {
                                cboBanMo.EditValue = search.ID;
                                cboBanMo.Properties.Buttons[1].Visible = true;
                                txtMethodCode.Focus();
                                txtMethodCode.SelectAll();
                            }
                            else
                            {
                                cboBanMo.EditValue = null;
                                cboBanMo.Focus();
                                cboBanMo.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboBanMo.EditValue = null;
                        cboBanMo.Focus();
                        cboBanMo.ShowPopup();
                    }
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
                    cboBanMo.Properties.Buttons[1].Visible = false;
                    cboBanMo.EditValue = null;
                    txtBanMoCode.Text = "";
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_TABLE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_TABLE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBanMo.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBanMoCode.Text = data.PTTT_TABLE_CODE;
                            cboBanMo.Properties.Buttons[1].Visible = true;
                            txtMethodCode.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_TABLE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_TABLE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBanMo.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBanMoCode.Text = data.PTTT_TABLE_CODE;
                            cboBanMo.Properties.Buttons[1].Visible = true;
                        }
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

        internal void LoadPhuongPhapThucTe(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPhuongPhapThucTe.Focus();
                    cboPhuongPhapThucTe.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.PTTT_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPhuongPhapThucTe.EditValue = data[0].ID;
                            cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                            txtEmotionlessMethod.Focus();
                            txtEmotionlessMethod.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cboPhuongPhapThucTe.EditValue = search.ID;
                                cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                                txtEmotionlessMethod.Focus();
                                txtEmotionlessMethod.SelectAll();
                            }
                            else
                            {
                                cboPhuongPhapThucTe.EditValue = null;
                                cboPhuongPhapThucTe.Focus();
                                cboPhuongPhapThucTe.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboPhuongPhapThucTe.EditValue = null;
                        cboPhuongPhapThucTe.Focus();
                        cboPhuongPhapThucTe.ShowPopup();
                    }
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
                    cboPhuongPhapThucTe.Properties.Buttons[1].Visible = false;
                    cboPhuongPhapThucTe.EditValue = null;
                    txtPhuongPhapTT.Text = "";
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPhuongPhapThucTe.EditValue.ToString()));
                        {
                            txtPhuongPhapTT.Text = data.PTTT_METHOD_CODE;
                            cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                            txtEmotionlessMethod.Focus();
                            txtEmotionlessMethod.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhuongPhapThucTe.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPhuongPhapTT.Text = data.PTTT_METHOD_CODE;
                            cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                            txtEmotionlessMethod.Focus();
                            txtEmotionlessMethod.SelectAll();
                        }
                    }
                }
                else
                {
                    cboPhuongPhapThucTe.ShowPopup();
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

        private void frmClsInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // sign
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateSign = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSign.Name && o.MODULE_LINK == MODULELINK).FirstOrDefault() : null;
                if (csAddOrUpdateSign != null)
                {
                    csAddOrUpdateSign.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdateSign = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateSign.KEY = chkSign.Name;
                    csAddOrUpdateSign.VALUE = (chkSign.Checked ? "1" : "");
                    csAddOrUpdateSign.MODULE_LINK = MODULELINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateSign);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

                // print

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdatePrint = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrint.Name && o.MODULE_LINK == MODULELINK).FirstOrDefault() : null;
                if (csAddOrUpdatePrint != null)
                {
                    csAddOrUpdatePrint.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdatePrint = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdatePrint.KEY = chkPrint.Name;
                    csAddOrUpdatePrint.VALUE = (chkPrint.Checked ? "1" : "");
                    csAddOrUpdatePrint.MODULE_LINK = MODULELINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdatePrint);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

                // print preview
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdatePreView = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPreView.Name && o.MODULE_LINK == MODULELINK).FirstOrDefault() : null;
                if (csAddOrUpdatePreView != null)
                {
                    csAddOrUpdatePreView.VALUE = (chkPreView.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdatePreView = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdatePreView.KEY = chkPreView.Name;
                    csAddOrUpdatePreView.VALUE = (chkPreView.Checked ? "1" : "");
                    csAddOrUpdatePreView.MODULE_LINK = MODULELINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdatePreView);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                DisposeForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DisposeForm()
        {
            try
            {
                lstExecuteRole = null;
                lstService = null;
                dataIcdCms = null;
                delegateRefresh = null;
                NumUserExecute = null;
                MODULELINK = null;
                currentControlStateRDO = null;
                controlStateWorker = null;
                ucEkip = null;
                datasPtttHighTech = null;
                datasDeathWithin = null;
                datasBloodRh = null;
                datasBloodABO = null;
                datasEmotionlessResult = null;
                datasEmotionlessMethod2 = null;
                datasPtttTable = null;
                datasPtttPriority = null;
                datasMachine = null;
                datasPtttCatastrophe = null;
                datasPtttCondition = null;
                datasEmotionLessMethod = null;
                datasPtttMethod = null;
                datasPtttGroup = null;
                currentImageADO = null;
                imageADOs = null;
                ModuleControls = null;
                ucIcdCause = null;
                IcdCauseProcessor = null;
                AutoCheckIcd = null;
                ucSecondaryIcd = null;
                subIcdProcessor = null;
                listIcd = null;
                ucIcdAfter = null;
                ucIcdBefore = null;
                ucIcdMain = null;
                icdAfterProcessor = null;
                icdBeforeProcessor = null;
                icdMainProcessor = null;
                key = null;
                AcsUserADOList = null;
                vhisTreatment = null;
                isAllowEditInfo = false;
                Module = null;
                positionHandle = 0;
                _TextIcdName3 = null;
                _TextIcdName2 = null;
                _TextIcdName1 = null;
                autoCheckIcd = 0;
                dataIcds = null;
                ekipTemps = null;
                actSaveClick = null;
                ekipUserAdos = null;
                sereServPTTT = null;
                serviceReq = null;
                sereServExt = null;
                currentServiceADO = null;
                this.bbtnSaveShortcut.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSaveShortcut_ItemClick);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.chkIcdCm.CheckedChanged -= new System.EventHandler(this.chkIcdCm_CheckedChanged);
                this.chkIcdCm.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkIcdCm_PreviewKeyDown);
                this.txtIcdCmSubName.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtIcdCmSubName_KeyDown);
                this.txtIcdCmSubName.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtIcdCmSubName_KeyUp);
                this.txtIcdCmSubCode.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtIcdCmSubCode_InvalidValue);
                this.txtIcdCmSubCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdCmSubCode_PreviewKeyDown);
                this.txtIcdCmSubCode.Validating -= new System.ComponentModel.CancelEventHandler(this.txtIcdCmSubCode_Validating);
                this.txtIcdCmName.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdCmName_PreviewKeyDown);
                this.cboIcdCmName.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboIcdCmName_Closed);
                this.cboIcdCmName.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboIcdCmName_ButtonClick);
                this.cboIcdCmName.TextChanged -= new System.EventHandler(this.cboIcdCmName_TextChanged);
                this.cboIcdCmName.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboIcdCmName_KeyUp);
                this.txtIcdCmCode.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtIcdCmCode_InvalidValue);
                this.txtIcdCmCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdCmCode_PreviewKeyDown);
                this.txtIcdCmCode.Validating -= new System.ComponentModel.CancelEventHandler(this.txtIcdCmCode_Validating);
                this.txtNumExecute.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtNumExecute_KeyPress);
                this.txtNumExecute.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtNumExecute_KeyUp);
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.chkPreView.CheckedChanged -= new System.EventHandler(this.chkPreView_CheckedChanged);
                this.chkPrint.CheckedChanged -= new System.EventHandler(this.chkPrint_CheckedChanged);
                this.cboDepartment.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDepartment_Closed);
                this.cboDepartment.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDepartment_ButtonClick);
                this.btnSaveEkipTemp.Click -= new System.EventHandler(this.btnSaveEkipTemp_Click);
                this.cboEkipTemp.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboEkipTemp_Closed);
                this.cboEkipTemp.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboEkipTemp_ButtonClick);
                this.btnSavePtttTemp.Click -= new System.EventHandler(this.btnSavePtttTemp_Click);
                this.cboPtttTemp.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPtttTemp_ButtonClick);
                this.cboPtttTemp.EditValueChanged -= new System.EventHandler(this.cboPtttTemp_EditValueChanged);
                this.btnChooseLuocDoPTTT.Click -= new System.EventHandler(this.btnChooseLuocDoPTTT_Click);
                this.btnAddImageLuocDoPTTT.Click -= new System.EventHandler(this.btnAddImageLuocDoPTTT_Click);
                this.cboPhuongPhapThucTe.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPhuongPhapThucTe_Closed);
                this.cboPhuongPhapThucTe.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPhuongPhapThucTe_ButtonClick);
                this.cboPhuongPhapThucTe.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboPhuongPhapThucTe_KeyUp);
                this.txtPhuongPhapTT.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPhuongPhapTT_PreviewKeyDown);
                this.cboBanMo.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboBanMo_Closed);
                this.cboBanMo.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboBanMo_ButtonClick);
                this.cboBanMo.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboBanMo_KeyUp);
                this.txtBanMoCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBanMoCode_PreviewKeyDown);
                this.cboMachine.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMachine_Closed);
                this.cboMachine.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboMachine_ButtonClick);
                this.cboMachine.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboMachine_KeyUp);
                this.txtMachineCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMachineCode_PreviewKeyDown);
                this.cboMoKTCao.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMoKTCao_Closed);
                this.cboMoKTCao.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboMoKTCao_ButtonClick);
                this.cboMoKTCao.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboMoKTCao_KeyUp);
                this.txtKQVoCam.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKQVoCam_PreviewKeyDown);
                this.cboKQVoCam.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboKQVoCam_Closed);
                this.cboKQVoCam.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboKQVoCam_ButtonClick);
                this.cboKQVoCam.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboKQVoCam_KeyUp);
                this.txtMoKTCao.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMoKTCao_PreviewKeyDown);
                this.cboPhuongPhap2.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPhuongPhap2_Closed);
                this.cboPhuongPhap2.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPhuongPhap2_ButtonClick);
                this.cboPhuongPhap2.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboPhuongPhap2_KeyUp);
                this.txtPhuongPhap2.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPhuongPhap2_PreviewKeyDown);
                this.cboLoaiPT.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboLoaiPT_Closed);
                this.cboLoaiPT.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboLoaiPT_ButtonClick);
                this.cboLoaiPT.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboLoaiPT_KeyUp);
                this.txtLoaiPT.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtLoaiPT_PreviewKeyDown);
                this.txtPtttGroupCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPtttGroupCode_PreviewKeyDown);
                this.cboMethod.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMethod_Closed);
                this.cboMethod.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboMethod_ButtonClick);
                this.cboMethod.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboMethod_KeyUp);
                this.txtMANNER.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMANNER_PreviewKeyDown);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.cboDeathSurg.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDeathSurg_Closed);
                this.cboDeathSurg.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDeathSurg_ButtonClick);
                this.cboDeathSurg.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboDeathSurg_KeyUp);
                this.txtDeathSurg.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDeathSurg_PreviewKeyDown);
                this.cboCatastrophe.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCatastrophe_Closed);
                this.cboCatastrophe.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboCatastrophe_ButtonClick);
                this.cboCatastrophe.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboCatastrophe_KeyUp);
                this.txtCatastrophe.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCatastrophe_PreviewKeyDown);
                this.cboCondition.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCondition_Closed);
                this.cboCondition.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboCondition_ButtonClick);
                this.cboCondition.EditValueChanged -= new System.EventHandler(this.cboPosition_EditValueChanged);
                this.cboCondition.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboCondition_KeyUp);
                this.txtCondition.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCondition_PreviewKeyDown);
                this.cbbBloodRh.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cbbBloodRh_Closed);
                this.cbbBloodRh.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbbBloodRh_ButtonClick);
                this.cbbBloodRh.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cbbBloodRh_KeyUp);
                this.txtBloodRh.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBloodRh_PreviewKeyDown);
                this.cbbBlood.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cbbBlood_Closed);
                this.cbbBlood.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbbBlood_ButtonClick);
                this.cbbBlood.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cbbBlood_KeyUp);
                this.txtBlood.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBlood_PreviewKeyDown);
                this.cbbEmotionlessMethod.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cbbEmotionlessMethod_Closed);
                this.cbbEmotionlessMethod.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbbEmotionlessMethod_ButtonClick);
                this.cbbEmotionlessMethod.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cbbEmotionlessMethod_KeyUp);
                this.cbbPtttGroup.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cbbPtttGroup_Closed);
                this.cbbPtttGroup.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbbPtttGroup_ButtonClick);
                this.cbbPtttGroup.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cbbPtttGroup_KeyUp);
                this.txtEmotionlessMethod.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtEmotionlessMethod_PreviewKeyDown);
                this.txtMethodCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMethodCode_PreviewKeyDown);
                this.tileView1.ItemDoubleClick -= new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileView1_ItemDoubleClick);
                this.tileView1.ContextButtonClick -= new DevExpress.Utils.ContextItemClickEventHandler(this.tileView1_ContextButtonClick);
                this.tileView1.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.tileView1_KeyDown);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(this.frmClsInfo_FormClosed);
                this.Load -= new System.EventHandler(this.frmClsInfo_Load);
                customGridLookUpEdit1View.GridControl.DataSource = null;
                gridView6.GridControl.DataSource = null;
                gridView7.GridControl.DataSource = null;
                gridView1.GridControl.DataSource = null;
                cardControl.DataSource = null;
                repositoryItemCustomGridLookUpEdit1View.GridControl.DataSource = null;
                cbo_UseName.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                gridView2.GridControl.DataSource = null;
                gridLookUpEdit2View.GridControl.DataSource = null;
                gridLookUpEdit3View.GridControl.DataSource = null;
                gridLookUpEdit4View.GridControl.DataSource = null;
                gridView3.GridControl.DataSource = null;
                gridView4.GridControl.DataSource = null;
                gridView5.GridControl.DataSource = null;
                customGridLookUpEditWithFilterMultiColumn1View.GridControl.DataSource = null;
                customGridViewWithFilterMultiColumn1.GridControl.DataSource = null;
                customGridViewWithFilterMultiColumn2.GridControl.DataSource = null;
                layoutControlItem45 = null;
                chkIcdCm = null;
                txtIcdCmName = null;
                customGridLookUpEdit1View = null;
                cboIcdCmName = null;
                layoutControlItem44 = null;
                txtIcdCmSubName = null;
                layoutControlItem43 = null;
                layoutControlItem42 = null;
                layoutControlItem33 = null;
                txtIcdCmCode = null;
                panel2 = null;
                txtIcdCmSubCode = null;
                layoutControlItem40 = null;
                panel1 = null;
                layoutControlItem37 = null;
                txtNumExecute = null;
                barButtonItem1 = null;
                layoutControlItem36 = null;
                layoutControlItem35 = null;
                layoutControlItem34 = null;
                layoutControlItem31 = null;
                chkSign = null;
                chkPrint = null;
                chkPreView = null;
                btnPrint = null;
                layoutControlItem30 = null;
                layoutControlItem17 = null;
                layoutControlItem15 = null;
                gridView6 = null;
                cboEkipTemp = null;
                btnSaveEkipTemp = null;
                gridView7 = null;
                cboDepartment = null;
                layoutControlItem13 = null;
                btnSavePtttTemp = null;
                layoutControlItem11 = null;
                gridView1 = null;
                cboPtttTemp = null;
                txtDescription = null;
                layoutControlItem10 = null;
                layoutControlItem6 = null;
                btnAddImageLuocDoPTTT = null;
                btnChooseLuocDoPTTT = null;
                layoutControlItem5 = null;
                memoEdit1 = null;
                xtraTabPage1 = null;
                gridColumn3 = null;
                gridColumn2 = null;
                gridColumn1 = null;
                cardView1 = null;
                repositoryItemButtonEdit1 = null;
                repositoryItemCheckEdit1 = null;
                repositoryItemSpinEdit1 = null;
                tileViewColumn5 = null;
                tileViewColumn4 = null;
                tileViewColumn3 = null;
                repositoryItemPictureEdit1 = null;
                tileViewColumn2 = null;
                tileViewColumn1 = null;
                tileViewItemElement4 = null;
                tileViewItemElement3 = null;
                tileView1 = null;
                cardControl = null;
                xtraTabPage3 = null;
                xtraTabControl1 = null;
                emptySpaceItem1 = null;
                layoutControlItem3 = null;
                panelControlSubIcd = null;
                layoutControlItem4 = null;
                panelControlIcdMain = null;
                layoutControlItem8 = null;
                layoutControlItem7 = null;
                panelControlIcdBefore = null;
                panelControlIcdAfter = null;
                dxValidationProvider1 = null;
                repositoryItemCustomGridLookUpEdit1View = null;
                cbo_UseName = null;
                emptySpaceItem2 = null;
                layoutControlItem39 = null;
                layoutControlItem25 = null;
                layoutControlItem9 = null;
                layoutControlItem26 = null;
                lciTuVongTrong = null;
                layoutControlItem24 = null;
                lciTaiBien = null;
                layoutControlItem29 = null;
                layoutControlItem22 = null;
                lciKTC = null;
                layoutControlItem18 = null;
                lciNhomMau = null;
                lciMachine = null;
                layoutControlItem38 = null;
                layoutControlItem19 = null;
                lciHinhThucPTTT = null;
                layoutControlItem41 = null;
                lciBanMo = null;
                layoutControlItem28 = null;
                layoutControlItem16 = null;
                lciVoCam = null;
                layoutControlItem12 = null;
                lciPhuongPhap = null;
                layoutControlItem27 = null;
                layoutControlItem23 = null;
                layoutControlItem21 = null;
                layoutControlItem32 = null;
                layoutControlItem2 = null;
                lciPhanLoai = null;
                layoutControlItem1 = null;
                lciCachThuc = null;
                lciTinhTrang = null;
                layoutControlItem14 = null;
                layoutControlItem20 = null;
                lciRh = null;
                lciRight = null;
                txtMethodCode = null;
                txtEmotionlessMethod = null;
                cbbPtttGroup = null;
                cbbEmotionlessMethod = null;
                txtBlood = null;
                cbbBlood = null;
                txtBloodRh = null;
                cbbBloodRh = null;
                txtCondition = null;
                cboCondition = null;
                txtCatastrophe = null;
                cboCatastrophe = null;
                txtDeathSurg = null;
                cboDeathSurg = null;
                btnSave = null;
                txtMANNER = null;
                gridLookUpEdit1View = null;
                cboMethod = null;
                lblPackageType = null;
                txtPtttGroupCode = null;
                txtIntructionNote = null;
                txtLoaiPT = null;
                gridView2 = null;
                cboLoaiPT = null;
                txtPhuongPhap2 = null;
                gridLookUpEdit2View = null;
                cboPhuongPhap2 = null;
                txtMoKTCao = null;
                gridLookUpEdit3View = null;
                cboKQVoCam = null;
                txtKQVoCam = null;
                gridLookUpEdit4View = null;
                cboMoKTCao = null;
                txtMachineCode = null;
                gridView3 = null;
                cboMachine = null;
                txtBanMoCode = null;
                gridView4 = null;
                cboBanMo = null;
                txtPhuongPhapTT = null;
                gridView5 = null;
                cboPhuongPhapThucTe = null;
                Root = null;
                layoutControl1 = null;
                customGridLookUpEditWithFilterMultiColumn1View = null;
                customGridViewWithFilterMultiColumn1 = null;
                customGridViewWithFilterMultiColumn2 = null;
                layoutControlRight = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                bbtnSaveShortcut = null;
                bar1 = null;
                barManager1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        //private void cboDepartment_Closed(object sender, ClosedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.CloseMode == PopupCloseMode.Normal)
        //        {
        //            if (cboDepartment.EditValue != null)
        //            {
        //                var dataEkipList = (List<HisEkipUserADO>)gridControlEkip.DataSource;
        //                if (dataEkipList != null && dataEkipList.Count > 0)
        //                {
        //                    Parallel.ForEach(dataEkipList.Where(f => f.ID >= 0), l => l.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue.ToString())));
        //                }
        //                gridControlEkip.BeginUpdate();
        //                gridControlEkip.DataSource = null;
        //                gridControlEkip.DataSource = dataEkipList;
        //                gridControlEkip.EndUpdate();
        //                //ComboAcsUser(repositoryItemGridLookUpUsername, Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue.ToString())));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void btnChooseLuocDoPTTT_Click(object sender, EventArgs e)
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
                    foreach (var file in listImage)
                    {
                        string base64String = System.Text.Encoding.UTF8.GetString(file.CONTENT);
                        byte[] imageBytes = Convert.FromBase64String(base64String);

                        MemoryStream ms = new MemoryStream(imageBytes);
                        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                        string fileName = string.Format("{0}.{1}", string.Format("{0}_{1}_{2}", this.currentServiceADO.TDL_SERVICE_REQ_CODE, this.currentServiceADO.ID, DateTime.Now.ToString("HHmmss")), "jpg");

                        MemoryStream stream = new MemoryStream(imageBytes);
                        // If you're going to read from the stream, you may need to reset the position to the start
                        stream.Position = 0;
                        Inventec.Common.Logging.LogSystem.Debug("GlobalVariables.APPLICATION_CODE" + GlobalVariables.APPLICATION_CODE + "____this.currentServiceADO.TDL_TREATMENT_CODE" + this.currentServiceADO.TDL_TREATMENT_CODE + "____fileName" + fileName);
                        var rsUpload = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, this.currentServiceADO.TDL_TREATMENT_CODE, stream, fileName);
                        if (rsUpload != null)
                        {
                            CommonParam param = new CommonParam();
                            HIS_SERE_SERV_FILE data = new HIS_SERE_SERV_FILE();
                            data.DESCRIPTION = "";
                            data.SERE_SERV_FILE_NAME = fileName;
                            data.SERE_SERV_ID = this.currentServiceADO.ID;
                            data.URL = rsUpload.Url;
                            var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERE_SERV_FILE>(RequestUriStore.HIS_SERE_SERV_FILE_CREATE, ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                            if (apiResult != null)
                            {
                                //TODO
                            }
                        }
                        else
                        {
                            MessageBox.Show("Upload file thất bại, vui lòng liên hệ quản trị hệ thống để được hỗ trợ.");
                        }
                    }

                    //gọi api tạo thành công thì load lại danh sách
                    List<long> ssIds = new List<long>();
                    ssIds.Add(this.currentServiceADO.ID);
                    ProcessLoadSereServFile(ssIds);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessLoadSereServFile(List<long> sereServId)
        {
            bool result = false;
            try
            {
                var currentSereServFiles = GetSereServFilesBySereServId(sereServId);
                if (currentSereServFiles != null && currentSereServFiles.Count > 0)
                {
                    result = true;
                    this.imageADOs = new List<ImageADO>();
                    foreach (MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE item in currentSereServFiles)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("item.URL", item.URL) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServId), sereServId));
                        System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);
                        if (stream != null && stream.Length > 0)
                        {
                            ImageADO tileNew = new ImageADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_FILE>(tileNew, item);
                            tileNew.FileName = item.SERE_SERV_FILE_NAME + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            tileNew.IsChecked = false;

                            tileNew.streamImage = new MemoryStream();
                            stream.Position = 0;
                            stream.CopyTo(tileNew.streamImage);
                            stream.Position = 0;
                            tileNew.IMAGE_DISPLAY = System.Drawing.Image.FromStream(stream);
                            this.imageADOs.Add(tileNew);
                        }
                    }
                    ProcessLoadGridImage(this.imageADOs);
                }
                else
                {
                    cardControl.DataSource = null;
                    this.imageADOs = null;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HIS_SERE_SERV_FILE> GetSereServFilesBySereServId(List<long> sereServId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> result = null;
            try
            {
                if (sereServId != null && sereServId.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisSereServFileFilter filter = new MOS.Filter.HisSereServFileFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.SERE_SERV_IDs = sereServId;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "ID";
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>(RequestUriStore.HIS_SERE_SERV_FILE_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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

        private void btnAddImageLuocDoPTTT_Click(object sender, EventArgs e)
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
                        Inventec.Common.Logging.LogSystem.Debug("GlobalVariables.APPLICATION_CODE" + GlobalVariables.APPLICATION_CODE + "____this.currentServiceADO.TDL_TREATMENT_CODE" + this.currentServiceADO.TDL_TREATMENT_CODE + "____fileName" + fileName + ext);
                        var rsUpload = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, this.currentServiceADO.TDL_TREATMENT_CODE, stream, fileName + ext);
                        if (rsUpload != null)
                        {
                            CommonParam param = new CommonParam();
                            HIS_SERE_SERV_FILE data = new HIS_SERE_SERV_FILE();
                            data.DESCRIPTION = "";
                            data.SERE_SERV_FILE_NAME = fileName;
                            data.SERE_SERV_ID = this.currentServiceADO.ID;
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
                            MessageBox.Show("Upload file thất bại, vui lòng liên hệ quản trị hệ thống để được hỗ trợ.");
                        }
                    }

                    List<long> ssIds = new List<long>();
                    ssIds.Add(this.currentServiceADO.ID);
                    ProcessLoadSereServFile(ssIds);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Upload file thất bại, vui lòng liên hệ quản trị hệ thống để được hỗ trợ.");
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
                    var rsUpload = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, this.currentServiceADO.TDL_TREATMENT_CODE, stream, this.currentImageADO.SERE_SERV_FILE_NAME + ext);
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
                        MessageBox.Show("Upload file thất bại, vui lòng liên hệ quản trị hệ thống để được hỗ trợ.");
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("SaveImageProcess: 6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileView1_ItemDoubleClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
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

        private void tileView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    Inventec.Common.Logging.LogSystem.Debug("tileView1_KeyDown.Delete");
                    TileView view = sender as TileView;
                    var checkedRows = view.GetCheckedRows();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkedRows), checkedRows));
                    if (checkedRows != null && checkedRows.Count() > 0)
                    {
                        this.imageADOs = this.imageADOs.Where(o => o.IsChecked == false).ToList();
                        ProcessLoadGridImage(this.imageADOs);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("imageADOs.Count", imageADOs != null ? imageADOs.Count : 0));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboPtttTemp()
        {
            try
            {
                var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId;
                string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                var ptttTemp = BackendDataWorker.Get<HIS_SERE_SERV_PTTT_TEMP>().Where(o => o.IS_ACTIVE == 1 && (o.IS_PUBLIC == 1 || (o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == DepartmentID) || (o.CREATOR == loginname))).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERE_SERV_PTTT_TEMP_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("SERE_SERV_PTTT_TEMP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERE_SERV_PTTT_TEMP_NAME", "ID", columnInfos, false, 400);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboPtttTemp, ptttTemp, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion
        #region
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

        private void cboPtttTemp_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                HIS_SERE_SERV_PTTT_TEMP fillData = new HIS_SERE_SERV_PTTT_TEMP();
                if (cboPtttTemp.EditValue != null)
                {
                    fillData = BackendDataWorker.Get<HIS_SERE_SERV_PTTT_TEMP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPtttTemp.EditValue.ToString()));
                }
                FillDataToControl(fillData);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl(HIS_SERE_SERV_PTTT_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    cbbPtttGroup.EditValue = data.PTTT_GROUP_ID;
                    txtPtttGroupCode.Text = data.PTTT_GROUP_ID != null && data.PTTT_GROUP_ID > 0 ? datasPtttGroup.First(o => o.ID == data.PTTT_GROUP_ID).PTTT_GROUP_CODE : "";
                    cboMethod.EditValue = data.PTTT_METHOD_ID;
                    txtMethodCode.Text = data.PTTT_METHOD_ID != null && data.PTTT_METHOD_ID > 0 ? datasPtttMethod.First(o => o.ID == data.PTTT_METHOD_ID).PTTT_METHOD_CODE : "";
                    cboPhuongPhapThucTe.EditValue = data.REAL_PTTT_METHOD_ID;
                    txtPhuongPhapTT.Text = data.REAL_PTTT_METHOD_ID != null && data.REAL_PTTT_METHOD_ID > 0 ? datasPtttMethod.First(o => o.ID == data.REAL_PTTT_METHOD_ID && o.IS_ACTIVE == 1).PTTT_METHOD_CODE : "";
                    cbbEmotionlessMethod.EditValue = data.EMOTIONLESS_METHOD_ID;
                    txtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_ID != null && data.EMOTIONLESS_METHOD_ID > 0 ? datasEmotionLessMethod.First(o => o.ID == data.EMOTIONLESS_METHOD_ID).EMOTIONLESS_METHOD_CODE : "";
                    cboCondition.EditValue = data.PTTT_CONDITION_ID;
                    txtCondition.Text = data.PTTT_CONDITION_ID != null && data.PTTT_CONDITION_ID > 0 ? datasPtttCondition.First(o => o.ID == data.PTTT_CONDITION_ID).PTTT_CONDITION_CODE : "";
                    cboCatastrophe.EditValue = data.PTTT_CATASTROPHE_ID;
                    txtCatastrophe.Text = data.PTTT_CATASTROPHE_ID != null && data.PTTT_CATASTROPHE_ID > 0 ? datasPtttCatastrophe.First(o => o.ID == data.PTTT_CATASTROPHE_ID).PTTT_CATASTROPHE_CODE : "";
                    cboLoaiPT.EditValue = data.PTTT_PRIORITY_ID;
                    txtLoaiPT.Text = data.PTTT_PRIORITY_ID != null && data.PTTT_PRIORITY_ID > 0 ? datasPtttPriority.First(o => o.ID == data.PTTT_PRIORITY_ID).PTTT_PRIORITY_CODE : "";
                    cboBanMo.EditValue = data.PTTT_TABLE_ID;
                    txtBanMoCode.Text = data.PTTT_TABLE_ID != null && data.PTTT_TABLE_ID > 0 ? datasPtttTable.First(o => o.ID == data.PTTT_TABLE_ID).PTTT_TABLE_CODE : "";
                    cboPhuongPhap2.EditValue = data.EMOTIONLESS_METHOD_SECOND_ID;
                    txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_SECOND_ID != null && data.EMOTIONLESS_METHOD_SECOND_ID > 0 ? datasEmotionlessMethod2.First(o => o.ID == data.EMOTIONLESS_METHOD_SECOND_ID).EMOTIONLESS_METHOD_CODE : "";
                    cboKQVoCam.EditValue = data.EMOTIONLESS_RESULT_ID;
                    txtKQVoCam.Text = data.EMOTIONLESS_RESULT_ID != null && data.EMOTIONLESS_RESULT_ID > 0 ? datasEmotionlessResult.First(o => o.ID == data.EMOTIONLESS_RESULT_ID).EMOTIONLESS_RESULT_CODE : "";
                    cbbBlood.EditValue = data.BLOOD_ABO_ID;
                    txtBlood.Text = data.BLOOD_ABO_ID != null && data.BLOOD_ABO_ID > 0 ? datasBloodABO.First(o => o.ID == data.BLOOD_ABO_ID).BLOOD_ABO_CODE : "";
                    cbbBloodRh.EditValue = data.BLOOD_RH_ID;
                    txtBloodRh.Text = data.BLOOD_RH_ID != null && data.BLOOD_RH_ID > 0 ? datasBloodRh.First(o => o.ID == data.BLOOD_RH_ID).BLOOD_RH_CODE : "";
                    cboDeathSurg.EditValue = data.DEATH_WITHIN_ID;
                    txtDeathSurg.Text = data.DEATH_WITHIN_ID != null && data.DEATH_WITHIN_ID > 0 ? datasDeathWithin.First(o => o.ID == data.DEATH_WITHIN_ID).DEATH_WITHIN_CODE : "";
                    cboMoKTCao.EditValue = data.PTTT_HIGH_TECH_ID;
                    txtMoKTCao.Text = data.PTTT_HIGH_TECH_ID != null && data.PTTT_HIGH_TECH_ID > 0 ? datasPtttHighTech.First(o => o.ID == data.PTTT_HIGH_TECH_ID).PTTT_HIGH_TECH_CODE : "";
                    txtMANNER.Text = data.MANNER;
                    txtIntructionNote.Text = data.NOTE;
                    txtDescription.Text = data.DESCRIPTION;
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
                    MessageBox.Show("Không có nội dung lưu mẫu");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_SERE_SERV_PTTT_TEMP GetDataForTemp()
        {
            HIS_SERE_SERV_PTTT_TEMP result = null;
            try
            {
                bool valid = true;
                valid = valid && cbbBlood.EditValue == null;
                valid = valid && cbbBloodRh.EditValue == null;
                valid = valid && cbbEmotionlessMethod.EditValue == null;
                valid = valid && cboCatastrophe.EditValue == null;
                valid = valid && cboCondition.EditValue == null;
                valid = valid && cbbPtttGroup.EditValue == null;
                valid = valid && cboMethod.EditValue == null;
                valid = valid && cboPhuongPhapThucTe.EditValue == null;
                valid = valid && cboDeathSurg.EditValue == null;
                valid = valid && cboLoaiPT.EditValue == null;
                valid = valid && cboPhuongPhap2.EditValue == null;
                valid = valid && cboKQVoCam.EditValue == null;
                valid = valid && cboMoKTCao.EditValue == null;
                valid = valid && cboBanMo.EditValue == null;
                valid = valid && String.IsNullOrWhiteSpace(txtMANNER.Text);
                valid = valid && String.IsNullOrWhiteSpace(txtIntructionNote.Text);
                valid = valid && String.IsNullOrWhiteSpace(txtDescription.Text);

                // Tất cả null thì ko lưu mẫu
                if (!valid)
                {
                    result = new HIS_SERE_SERV_PTTT_TEMP();

                    if (cbbBlood.EditValue != null)
                    {
                        result.BLOOD_ABO_ID = (long)cbbBlood.EditValue;
                    }

                    //Nhom mau RH
                    if (cbbBloodRh.EditValue != null)
                    {
                        result.BLOOD_RH_ID = (long)cbbBloodRh.EditValue;
                    }

                    //Phuong phap vô cảm
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        result.EMOTIONLESS_METHOD_ID = (long)cbbEmotionlessMethod.EditValue;
                    }

                    //Tai bien PTTT
                    if (cboCatastrophe.EditValue != null)
                    {
                        result.PTTT_CATASTROPHE_ID = (long)cboCatastrophe.EditValue;
                    }

                    //Tinh hinh PTTT
                    if (cboCondition.EditValue != null)
                    {
                        result.PTTT_CONDITION_ID = (long)cboCondition.EditValue;
                    }

                    //Loai PTTT
                    if (cbbPtttGroup.EditValue != null)
                    {
                        result.PTTT_GROUP_ID = (long)cbbPtttGroup.EditValue;
                    }

                    //Phuong phap PTTT
                    if (cboMethod.EditValue != null)
                    {
                        result.PTTT_METHOD_ID = (long)cboMethod.EditValue;
                    }

                    //Phuong phap Thuc te
                    if (cboPhuongPhapThucTe.EditValue != null)
                    {
                        result.REAL_PTTT_METHOD_ID = (long)cboPhuongPhapThucTe.EditValue;
                    }

                    if (!String.IsNullOrEmpty(txtMANNER.Text))
                    {
                        result.MANNER = txtMANNER.Text;
                    }

                    if (!String.IsNullOrEmpty(txtIntructionNote.Text))
                    {
                        result.NOTE = txtIntructionNote.Text;
                    }

                    if (!String.IsNullOrEmpty(txtDescription.Text))
                    {
                        result.DESCRIPTION = txtDescription.Text;
                    }

                    //Tu vong
                    if (cboDeathSurg.EditValue != null)
                    {
                        result.DEATH_WITHIN_ID = (long)cboDeathSurg.EditValue;
                    }

                    if (cboLoaiPT.EditValue != null)
                    {
                        result.PTTT_PRIORITY_ID = (long)cboLoaiPT.EditValue;
                    }

                    if (cboPhuongPhap2.EditValue != null)
                    {
                        result.EMOTIONLESS_METHOD_SECOND_ID = (long)cboPhuongPhap2.EditValue;
                    }

                    if (cboKQVoCam.EditValue != null)
                    {
                        result.EMOTIONLESS_RESULT_ID = (long)cboKQVoCam.EditValue;
                    }

                    if (cboMoKTCao.EditValue != null)
                    {
                        result.PTTT_HIGH_TECH_ID = (long)cboMoKTCao.EditValue;
                    }

                    if (cboBanMo.EditValue != null)
                    {
                        result.PTTT_TABLE_ID = (long)cboBanMo.EditValue;
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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
                       || o.EXECUTE_ROLE_ID <= 0 || o.EXECUTE_ROLE_ID == null).FirstOrDefault() != null ? true : false;
                if (hasInvalid)
                {
                    MessageBox.Show("Không có thông tin kip thực hiện");
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
                            var lstExeRole = lstExecuteRole.Where(o => item.Select(s => s.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();
                            messError.Add(string.Format("Tài khoản {0} được thiết lập với các vai trò {1}", item.Key, string.Join(",", lstExeRole.Select(s => s.EXECUTE_ROLE_NAME))));
                        }
                    }

                    if (messError.Count > 0)
                    {
                        XtraMessageBox.Show(string.Join("\n", messError), "Thông báo");
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

        private void RefeshDataEkipTemp()
        {
            try
            {
                InitComboEkip();
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
                if (this.chkPrint.Checked)
                    this.chkPreView.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPreView_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkPreView.Checked)
                    this.chkPrint.Checked = false;
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
                this.currentControlStateRDO = controlStateWorker.GetData(MODULELINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrint.Name)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkSign.Name)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkPreView.Name)
                        {
                            chkPreView.Checked = item.VALUE == "1";
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
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate("Mps000033", DelegateRunPrinterSurg);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinterSurg(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000033":
                        LoadBieuMauPhieuThuThuatPhauThuat(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        public void LoadBieuMauPhieuThuThuatPhauThuat(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var focus = currentServiceADO;
                if (focus == null) return;

                CommonParam param = new CommonParam();
                WaitingManager.Show();

                MOS.Filter.HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.ID = focus.ID;
                V_HIS_SERE_SERV_5 SereServ5 = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, param).FirstOrDefault();

                // Lấy thông tin bệnh nhân
                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = focus.TDL_PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                MPS.Processor.Mps000033.PDO.PatientADO currentPatient = new MPS.Processor.Mps000033.PDO.PatientADO(patient);
                //Lấy thông tin chuyển khoa
                var departmentTran = HIS.Desktop.Print.PrintGlobalStore.getDepartmentTran(focus.TDL_TREATMENT_ID ?? 0);

                //Thông tin Misu
                V_HIS_TREATMENT treatmentView = new V_HIS_TREATMENT();
                V_HIS_SERVICE_REQ ServiceReq = new V_HIS_SERVICE_REQ();

                //Khoa hien tai
                if (serviceReq != null)
                {
                    MOS.Filter.HisServiceReqViewFilter ServiceReqFilter = new HisServiceReqViewFilter();
                    ServiceReqFilter.ID = serviceReq.ID;
                    ServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, ServiceReqFilter, param).FirstOrDefault();
                    HIS_DEPARTMENT department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_DEPARTMENT_ID);
                    if (department != null)
                    {
                        ServiceReq.REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }

                    V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                    if (room != null)
                    {
                        ServiceReq.REQUEST_ROOM_NAME = room.ROOM_NAME;
                    }

                    MOS.Filter.HisServiceReqViewFilter treatmentFilter = new HisServiceReqViewFilter();
                    treatmentFilter.ID = serviceReq.TREATMENT_ID;
                    treatmentView = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                }

                List<V_HIS_EKIP_USER> vEkipUsers = new List<V_HIS_EKIP_USER>();
                if (currentServiceADO.EKIP_ID != null)
                {
                    HisEkipUserViewFilter ekipUserFilter = new HisEkipUserViewFilter();
                    ekipUserFilter.EKIP_ID = currentServiceADO.EKIP_ID;
                    vEkipUsers = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER>>("api/HisEkipUser/GetView", ApiConsumers.MosConsumer, ekipUserFilter, param);
                }

                object dfdf = Activator.CreateInstance(vEkipUsers.GetType());

                MOS.Filter.HisSereServPtttViewFilter filter = new MOS.Filter.HisSereServPtttViewFilter();
                filter.SERE_SERV_ID = currentServiceADO.ID;

                var sereServPttts = new BackendAdapter(param)
                   .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                HisSereServFileFilter fil = new HisSereServFileFilter();
                fil.SERE_SERV_ID = focus.ID;
                var sereServFile = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>("api/HisSereServFile/Get", ApiConsumers.MosConsumer, fil, param);

                HisSesePtttMethodViewFilter filterSese = new HisSesePtttMethodViewFilter();
                filterSese.TDL_SERE_SERV_ID = focus.ID;
                var sesePtttMethod = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.V_HIS_SESE_PTTT_METHOD>>("api/HisSesePtttMethod/GetView", ApiConsumers.MosConsumer, filterSese, param);

                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                MPS.Processor.Mps000033.PDO.Mps000033PDO rdo = new MPS.Processor.Mps000033.PDO.Mps000033PDO(currentPatient, departmentTran, ServiceReq, SereServ5, sereServExt, sereServPttts, treatmentView, vEkipUsers, null, null, null, null, null, sereServFile, sesePtttMethod);
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentView != null ? treatmentView.TREATMENT_CODE : "", printTypeCode, Module != null ? Module.RoomId : 0);
                if (chkPrint.Checked)
                {
                    if (chkSign.Checked)
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, "");
                    else
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else if (chkPreView.Checked)
                {
                    if (chkSign.Checked)
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview, "");
                    else
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                else if (chkSign.Checked)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "");
                }
                else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }

                PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                PrintData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(PrintData);
                if (result != null && (PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow || PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow))
                {
                    MessageManager.Show(this.ParentForm, new CommonParam(), true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", this.Module.RoomId, this.Module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtNumExecute_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtNumExecute_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
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
                if (btnPrint.Enabled)
                    btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToCboIcdCm(
            TextEdit txtIcdCode,
            TextEdit txtIcdMain,
            CustomGridLookUpEditWithFilterMultiColumn cbo,
            CheckEdit chkEdit,
            string _ICD_CODE,
            string _ICD_NAME)
        {
            try
            {
                if (!string.IsNullOrEmpty(_ICD_CODE))
                {
                    var icd = this.dataIcdCms.Where(p => p.ICD_CM_CODE == _ICD_CODE).FirstOrDefault();
                    if (icd != null)
                    {
                        txtIcdCode.Text = icd.ICD_CM_CODE;
                        cbo.EditValue = icd.ID;
                        if (this.autoCheckIcd == 1
                            || (!String.IsNullOrEmpty(_ICD_NAME)
                            && (_ICD_NAME ?? "").Trim().ToLower() != (icd.ICD_CM_NAME ?? "").Trim().ToLower()))
                        {
                            chkEdit.Checked = (this.autoCheckIcd != 2);
                            txtIcdMain.Text = _ICD_NAME;
                        }
                        else
                        {
                            chkEdit.Checked = false;
                            txtIcdMain.Text = icd.ICD_CM_NAME;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCmCode_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Bạn nhập mã chẩn đoán không đúng. Vui lòng kiểm tra lại";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCmCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCMCombo(txtIcdCmCode.Text.ToUpper(), txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdCMCombo(string searchCode, TextEdit txtIcdCode, TextEdit txtIcdMain, CustomGridLookUpEditWithFilterMultiColumn cbo, CheckEdit chkEdit)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = this.dataIcdCms.Where(o => o.ICD_CM_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CM_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtIcdCode.Text = result.First().ICD_CM_CODE;
                        txtIcdMain.Text = result.First().ICD_CM_NAME;
                        cbo.EditValue = listData.First().ID;
                        chkEdit.Checked = (chkEdit.Enabled ? (this.autoCheckIcd == 1) : false);

                        if (chkEdit.Checked)
                        {
                            txtIcdMain.Focus();
                            txtIcdMain.SelectAll();
                        }
                        else
                        {
                            cbo.Focus();
                            cbo.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cbo.Focus();
                    cbo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCmCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataIcdCms.Where(o => o.ICD_CM_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CM_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCmCode.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtIcdCmCode);
                        ValidationICD(10, 500, false, txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm, layoutControlItem33);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCmName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkIcdCm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCmName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcdCmName.ClosePopup();
                    cboIcdCmName.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcdCmName.ClosePopup();
                    if (cboIcdCmName.EditValue != null)
                        this.ChangecboIcdCmTD(txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm, txtIcdCmSubCode);
                }
                else
                    cboIcdCmName.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangecboIcdCmTD(TextEdit txtIcdCode, TextEdit txtIcdMain, CustomGridLookUpEditWithFilterMultiColumn cbo, CheckEdit chkEdit, TextEdit nextFocus)
        {
            try
            {
                cbo.Properties.Buttons[1].Visible = true;
                var icd = dataIcdCms.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbo.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtIcdCode.Text = icd.ICD_CM_CODE;
                    txtIcdMain.Text = icd.ICD_CM_NAME;
                    chkEdit.Checked = (chkEdit.Enabled ? (this.autoCheckIcd == 1) : false);
                    if (chkEdit.Checked && nextFocus != null)
                    {
                        nextFocus.Focus();
                        nextFocus.SelectAll();
                    }
                    else if (chkEdit.Enabled)
                    {
                        chkEdit.Focus();
                    }
                    else
                    {
                        if (nextFocus != null)
                        {
                            nextFocus.Focus();
                            nextFocus.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCmName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcdCmName.Properties.Buttons[1].Visible)
                        return;
                    cboIcdCmName.EditValue = null;
                    txtIcdCmCode.Text = "";
                    txtIcdCmName.Text = "";
                    cboIcdCmName.Properties.Buttons[1].Visible = false;
                    txtIcdCmCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCmName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcdCmName.EditValue != null)
                        this.ChangecboIcdCmTD(txtIcdCmCode, txtIcdCmName, cboIcdCmName, chkIcdCm, txtIcdCmSubCode);
                    else
                    {
                        txtIcdCmSubCode.Focus();
                        txtIcdCmSubCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCmName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcdCmName.Text))
                {
                    cboIcdCmName.EditValue = null;
                    txtIcdCmName.Text = "";
                    chkIcdCm.Checked = false;
                }
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

        private void txtIcdCmSubName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    icdBeforeProcessor.FocusControl(ucIcdBefore);
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

        private void chkIcdCm_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcdCm.Checked == true)
                {
                    cboIcdCmName.Visible = false;
                    txtIcdCmName.Visible = true;
                    txtIcdCmName.Text = cboIcdCmName.Text;
                    txtIcdCmName.Focus();
                    txtIcdCmName.SelectAll();
                }
                else if (chkIcdCm.Checked == false)
                {
                    txtIcdCmName.Visible = false;
                    cboIcdCmName.Visible = true;
                    txtIcdCmName.Text = cboIcdCmName.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcdCm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCmSubCode.Focus();
                    txtIcdCmSubCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmClsInfo
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo = new ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Lang", typeof(frmClsInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.tileViewColumn2.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.tileViewColumn2.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.tileViewColumn3.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.tileViewColumn3.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.bar1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.bbtnSaveShortcut.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.bbtnSaveShortcut.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlRight.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlRight.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.chkIcdCm.Properties.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.chkIcdCm.Properties.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.txtIcdCmSubName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmClsInfo.txtIcdCmSubName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboIcdCmName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboIcdCmName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.chkPreView.Properties.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.chkPreView.Properties.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.chkPrint.Properties.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.chkPrint.Properties.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.chkSign.Properties.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.chkSign.Properties.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboEkipTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboEkipTemp.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.btnSavePtttTemp.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.btnSavePtttTemp.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboPtttTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboPtttTemp.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.btnChooseLuocDoPTTT.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.btnChooseLuocDoPTTT.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.btnAddImageLuocDoPTTT.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.btnAddImageLuocDoPTTT.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboPhuongPhapThucTe.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboPhuongPhapThucTe.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboBanMo.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboBanMo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboMachine.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboMachine.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboMoKTCao.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboMoKTCao.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboKQVoCam.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboKQVoCam.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboPhuongPhap2.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboPhuongPhap2.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboLoaiPT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboLoaiPT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboMethod.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboMethod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.btnSave.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboDeathSurg.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboDeathSurg.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboCatastrophe.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboCatastrophe.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cboCondition.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cboCondition.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cbbBloodRh.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cbbBloodRh.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cbbBlood.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cbbBlood.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cbbEmotionlessMethod.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cbbEmotionlessMethod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.cbbPtttGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmClsInfo.cbbPtttGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.tileViewItemElement3.Text = Inventec.Common.Resource.Get.Value("tileViewItemElement3.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.tileViewItemElement4.Text = Inventec.Common.Resource.Get.Value("tileViewItemElement4.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.tileViewColumn1.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.tileViewColumn1.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.tileViewColumn4.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.tileViewColumn4.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.tileViewColumn5.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.tileViewColumn5.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmClsInfo.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciRh.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciRh.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciTinhTrang.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciTinhTrang.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciCachThuc.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciCachThuc.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciPhanLoai.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciPhanLoai.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem2.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem2.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem21.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem21.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciPhuongPhap.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciPhuongPhap.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciVoCam.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciVoCam.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem28.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem28.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciBanMo.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciBanMo.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciHinhThucPTTT.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.lciHinhThucPTTT.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciHinhThucPTTT.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciHinhThucPTTT.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciMachine.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.lciMachine.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciMachine.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciMachine.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciNhomMau.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciNhomMau.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciKTC.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.lciKTC.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciKTC.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciKTC.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciTaiBien.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciTaiBien.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.lciTuVongTrong.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.lciTuVongTrong.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem9.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem9.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem11.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem11.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem30.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem37.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem37.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem37.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem37.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem33.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem33.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem43.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem43.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.layoutControlItem43.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.layoutControlItem43.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmClsInfo.Text", Resources.ResourceLanguageManager.LanguageResourcefrmClsInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
