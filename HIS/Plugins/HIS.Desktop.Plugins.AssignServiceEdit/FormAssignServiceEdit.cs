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
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.ApplicationFont;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.AssignServiceEdit.ADO;
using HIS.Desktop.Plugins.AssignServiceEdit.AssignServiceEdit;
using HIS.Desktop.Plugins.AssignServiceEdit.ChooseICD;
using HIS.Desktop.Plugins.AssignServiceEdit.Config;
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignServiceEdit
{
    public partial class FormAssignServiceEdit : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        private int positionHandleControl;
        private List<HIS_DEPARTMENT_TRAN> ListDepartmentTranCheckTime = null;
        private List<HIS_CO_TREATMENT> ListCoTreatmentCheckTime = null;
        bool isNotLoadWhileChangeInstructionTimeInFirst;
        long serviceReqId = 0;
        long instructionTime = 0;
        System.Globalization.CultureInfo cultureLang;
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq;
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter;
        List<HIS_PATIENT_TYPE> glstPatientType;
        List<HIS_PATIENT_TYPE> glstPrimaryPatientType;
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServWithTreatment = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();

        List<ADO.HisSereServADO> listSereServCurrent;
        List<ADO.HisSereServADO> allSereServ;
        List<ADO.HisSereServADO> listSereServAdd;
        List<HisSereServADO> SereServAdditonSdos;
        Inventec.Desktop.Common.Modules.Module currentModule;

        bool isCheckAll = true;
        HIS.Desktop.Common.RefeshReference RefeshReference;
        short IS_TRUE = 1;
        internal HIS_ICD icdChoose { get; set; }
        bool isYes = false;
        List<HisSereServADO> listDatasFix { get; set; }

        private V_HIS_ROOM currentWorkingRoom;
        HIS_DEPARTMENT currentDepartment = null;
        long? PrimaryPatientTypeId = null;

        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment { get; set; }

        decimal transferTreatmentFee = 0;
        HIS_PATIENT_TYPE patientTypeByPT;
        List<V_HIS_SERVICE> lstService = null;
        private List<long> intructionTimeSelecteds;
        #endregion

        #region Construct
        public FormAssignServiceEdit()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormAssignServiceEdit(long id, long _instructionTime, HIS.Desktop.Common.RefeshReference _RefeshReference, Inventec.Desktop.Common.Modules.Module _module)
            : this()
        {
            try
            {
                this.Text = _module.text;
                this.currentModule = _module;
                this.serviceReqId = id;
                this.instructionTime = _instructionTime;
                if (_RefeshReference != null)
                {
                    this.RefeshReference = _RefeshReference;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormAssignServiceEdit_Load(object sender, EventArgs e)
        {
            try
            {
                dtInstructionTime.EditValueChanged -= dtInstructionTime_EditValueChanged;
                currentWorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                this.currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == currentWorkingRoom.DEPARTMENT_ID);
                isNotLoadWhileChangeInstructionTimeInFirst = true;
                UcDateInit();
                HisConfigCFG.LoadConfig();
                SetIcon();
                LoadHisServiceFromRam();
                LoadKeysFromlanguage();

                LoadServiceSameToRAM();

                VisibleColumnInGridControlService();

                SetDefaultValueControl();
                PatientTypeWithPatientTypeAlter();
                FillDataToGrid();
                InitComboExecuteRoom();
                CheckOverTotalPatientPrice();
                if (HisConfigCFG.IcdServiceAllowUpdate == "1")
                {
                    btnBoSungPhacDo.Enabled = true;
                }
                else
                {
                    btnBoSungPhacDo.Enabled = false;
                }
                GridViewService.Focus();
                //txtServiceName_Search.Width = Gc_ServiceName.Width - 10;
                //txtServiceCode_Search.Width = Gc_ServiceCode.VisibleWidth - 2;
                this.listDatasFix = GridControlService.DataSource as List<ADO.HisSereServADO>;

                isNotLoadWhileChangeInstructionTimeInFirst = false;
                dtInstructionTime.EditValueChanged += dtInstructionTime_EditValueChanged;
            }
            catch (Exception ex)
            {

                isNotLoadWhileChangeInstructionTimeInFirst = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

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

        #region Private method
        #region load
        void LoadServiceSameToRAM()
        {
            try
            {
                if (ServiceSameADO.ServiceSameAllADOs == null || ServiceSameADO.ServiceSameAllADOs.Count == 0)
                {
                    MOS.Filter.HisServiceSameViewFilter serviceSameViewFilter = new HisServiceSameViewFilter();
                    serviceSameViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    ServiceSameADO.ServiceSameAllADOs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_SAME>>("api/HisServiceSame/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceSameViewFilter, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

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
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                    cultureLang);
                this.toggleSwitch.Properties.OffText = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__TOGGLE_OFF_TEXT",
                    Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                    cultureLang);
                this.toggleSwitch.Properties.OnText = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__TOGGLE_ON_TEXT",
                    Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                    cultureLang);
                this.Gc_Amount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__GC_AMOUNT",
                    Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                    cultureLang);
                this.Gc_Expend.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__GC_EXPEND",
                    Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                    cultureLang);
                this.Gc_PatientTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__GC_PATIENT_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                    cultureLang);
                this.Gc_Price.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__GC_PRICE",
                    Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                    cultureLang);
                this.Gc_ServiceCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__GC_SERVICE_CODE",
                    Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                    cultureLang);
                this.Gc_ServiceName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__GC_SERVICE_NAME",
                    Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                    cultureLang);
                this.Gc_IsOutKtcFee.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__GC_IS_OUT_KTC_FEE",
                    Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                    cultureLang);
                this.lciIcd.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__LCI_ICD",
                   Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                   cultureLang);
                this.lciSubIcd.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__LCI_SUB_ICD",
                   Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                   cultureLang);
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__LCI_PATIENT_NAME",
                   Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                   cultureLang);
                this.lciTreatmentCode.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__LCI_TREATMENT_CODE",
                   Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                   cultureLang);
                this.lciInstructionTime.Text = Inventec.Common.Resource.Get.Value(
                 "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__LCI_INSTRUCTION_TIME",
                 Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                 cultureLang);
                this.lciRoom.Text = Inventec.Common.Resource.Get.Value(
                "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__LCI_ROOM",
                Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                cultureLang);
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value(
                 "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__LCI_PATIENT_CODE",
                 Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                 cultureLang);
                this.lciServiceReqCode.Text = Inventec.Common.Resource.Get.Value(
                 "IVT_LANGUAGE_KEY__FORM_ASSIGN_SERVICE_EDIT__LCI_SERVICE_REQ_CODE",
                 Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit,
                 cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToCommon(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                if (serviceReq != null)
                {
                    lblTreatmentCode.Text = serviceReq.TDL_TREATMENT_CODE;
                    lblPatientName.Text = serviceReq.TDL_PATIENT_NAME;
                    lblIcd.Text = serviceReq.ICD_CODE + " - " + serviceReq.ICD_NAME;
                    lblSubIcd.Text = serviceReq.ICD_SUB_CODE + " - " + serviceReq.ICD_TEXT;
                    intructionTimeSelecteds = new List<long>();
                    intructionTimeSelecteds.Add(this.instructionTime);
                    dtInstructionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.instructionTime) ?? DateTime.Now;
                    lblPatientCode.Text = serviceReq.TDL_PATIENT_CODE;
                    lblServiceReqCode.Text = serviceReq.SERVICE_REQ_CODE;
                    cboRoom.EditValue = serviceReq.EXECUTE_ROOM_ID;
                    if (serviceReq.USE_TIME != null && serviceReq.USE_TIME > 0)
                    {
                        dtUseTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.USE_TIME ?? 0) ?? DateTime.MinValue;
                    }
                    else
                    {
                        dtUseTime.EditValue = null;
                    }
                }
                else
                {
                    lblSubIcd.Text = "";
                    lblIcd.Text = "";
                    lciPatientName.Text = "";
                    lblTreatmentCode.Text = "";
                    dtInstructionTime.EditValue = null;
                    lblPatientCode.Text = "";
                    lblServiceReqCode.Text = "";
                    cboRoom.Text = "";
                    dtUseTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Phòng Thực hiện
        private void InitComboExecuteRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                //Lấy danh sách service id được check
                List<ADO.HisSereServADO> serviceCheckeds__Send = this.allSereServ.FindAll(o => o.IsChecked);
                List<long> serviceIdChecked = serviceCheckeds__Send.Select(o => o.SERVICE_ID).ToList();
                int serviceCount = serviceIdChecked.Count;

                MOS.Filter.HisServiceRoomFilter serviceRoomFilter = new MOS.Filter.HisServiceRoomFilter();
                serviceRoomFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                serviceRoomFilter.SERVICE_IDS = serviceIdChecked;
                //List<long> services = data.Select(s => s.SERVICE_ID).Distinct().ToList();
                var listServiceRooms = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_ROOM>>("api/HisServiceRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceRoomFilter, param);
                List<long> rooms = listServiceRooms.Select(s => s.ROOM_ID).Distinct().ToList();
                List<long> getRoomID = new List<long>();
                foreach (var item in rooms)
                {
                    var data = listServiceRooms.Where(o => o.ROOM_ID == item).Count();
                    if (data == serviceCount)
                    {
                        getRoomID.Add(item);
                    }
                }

                //4 phòng
                HisRoomViewFilter roomViewFilter = new HisRoomViewFilter();
                roomViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                roomViewFilter.IDs = getRoomID;
                var listRooms = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>("api/HisRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, roomViewFilter, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 10, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 110);
                ControlEditorLoader.Load(cboRoom, listRooms, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.ID = this.serviceReqId;
                var serviceReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (serviceReq != null && serviceReq.Count > 0)
                {
                    HisServiceReq = serviceReq.FirstOrDefault();
                    ProcessGetDataDepartment();
                    LoadCurrentTreatment(HisServiceReq.TREATMENT_ID, instructionTime);
                    SetDataToCommon(this.HisServiceReq);
                    LoadDataSereServWithTreatment(this.HisServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleColumnInGridControlService()
        {
            try
            {
                //An hien cot cp ngoai goi
                long isVisibleColumnCPNgoaiGoi = ConfigApplicationWorker.Get<long>(Base.AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_CP_NGOAI_GOI);
                if (isVisibleColumnCPNgoaiGoi == 1)
                {
                    Gc_IsOutKtcFee.Visible = false;
                    Gc_IsOutKtcFee.Visible = false;
                }

                //An hien cot hao phi
                long isVisibleColumnHaoPhi = ConfigApplicationWorker.Get<long>(Base.AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_HAO_PHI);
                if (isVisibleColumnHaoPhi == 1)
                {
                    Gc_Expend.Visible = false;
                    Gc_Expend.Visible = false;
                }

                if (HisConfigCFG.IsSetPrimaryPatientType == "1")
                {
                    gridColumnDoiTuongPhuThu.Visible = true;
                    gridColumnDoiTuongPhuThu.OptionsColumn.AllowEdit = true;
                }
                else if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                {
                    gridColumnDoiTuongPhuThu.Visible = true;
                    gridColumnDoiTuongPhuThu.OptionsColumn.AllowEdit = false;
                }
                else
                {
                    gridColumnDoiTuongPhuThu.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentTreatment(long treatmentId, long intructionTime)
        {
            try
            {
                CommonParam param = new CommonParam();

                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filterTreat = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filterTreat.TREATMENT_ID = treatmentId;
                if (intructionTime > 0)
                {
                    filterTreat.INTRUCTION_TIME = intructionTime;
                }
                else
                {
                    filterTreat.INTRUCTION_TIME = Inventec.Common.DateTime.Get.Now() ?? 0;
                }
                var hisTreatments = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumers.MosConsumer, filterTreat, ProcessLostToken, param);
                this.currentHisTreatment = hisTreatments != null && hisTreatments.Count > 0 ? hisTreatments.FirstOrDefault() : null;

                MOS.Filter.HisPatientTypeAlterViewAppliedFilter filter = new MOS.Filter.HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatmentId;
                if (intructionTime > 0)
                    filter.InstructionTime = intructionTime;
                else
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                currentHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(ApiConsumer.HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadLoadDataSereServWithTreatment()
        {
            System.Threading.Thread thread = new System.Threading.Thread(SetDefaultValueControl);
            thread.Priority = System.Threading.ThreadPriority.Highest;
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void LoadDataSereServWithTreatment(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ currentHisServiceReq)
        {
            try
            {
                if (currentHisServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisSereServFilter hisSereServFilter = new MOS.Filter.HisSereServFilter();
                    hisSereServFilter.TREATMENT_ID = currentHisServiceReq.TREATMENT_ID;
                    hisSereServFilter.TDL_SERVICE_REQ_TYPE_ID = currentHisServiceReq.SERVICE_REQ_TYPE_ID;
                    if (this.instructionTime > 0)
                    {
                        hisSereServFilter.TDL_INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        this.instructionTime.ToString().Substring(0, 8) + "000000");
                        hisSereServFilter.TDL_INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        this.instructionTime.ToString().Substring(0, 8) + "235959");
                    }
                    var sereServ = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(Base.GlobalStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, hisSereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    List<long> setyAllowsIds = new List<long>();
                    setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU);
                    setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                    setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                    if (sereServ != null && sereServ.Count > 0)
                    {
                        this.sereServWithTreatment = sereServ.Where(o => !setyAllowsIds.Contains(o.TDL_SERVICE_TYPE_ID)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientTypeWithPatientTypeAlter()
        {
            try
            {
                currentPatientTypeWithPatientTypeAlter = null;
                if (Base.GlobalStore.HisVPatientTypeAllows != null && Base.GlobalStore.HisVPatientTypeAllows.Count > 0 && Base.GlobalStore.HisPatientTypes != null)
                {
                    if (this.currentHisPatientTypeAlter != null)
                    {
                        var patientTypeAllow = Base.GlobalStore.HisVPatientTypeAllows.Where(o => o.PATIENT_TYPE_ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        if (patientTypeAllow != null && patientTypeAllow.Count > 0)
                        {
                            this.currentPatientTypeWithPatientTypeAlter = Base.GlobalStore.HisPatientTypes.Where(o => patientTypeAllow.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();
                        }
                        else
                        {
                            MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                        }
                    }
                    else
                    {
                        MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                    }
                }
                else
                {
                    MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                }
                glstPatientType = this.currentPatientTypeWithPatientTypeAlter;//.Where(pt => pt.IS_ADDITION != 1).ToList();
                if (currentHisPatientTypeAlter != null && !glstPatientType.Exists(o => o.ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID))//glstPatientType không có đối tượng bn thì thêm đối tượng tương ứng với đối tượng của bệnh nhân
                {
                    glstPatientType.AddRange(Base.GlobalStore.HisPatientTypes.Where(o => o.ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID).ToList());
                }
                LoadDataToCboPatientType(glstPatientType);
                glstPrimaryPatientType = currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0 ? this.currentPatientTypeWithPatientTypeAlter.Where(pt => pt.IS_ADDITION == 1).ToList() : null;
                LoadDataToCboPrimaryPatientType(glstPrimaryPatientType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                allSereServ = new List<ADO.HisSereServADO>();
                // lấy tất cả sereServ của serviceReq muốn sửa
                listSereServCurrent = new List<ADO.HisSereServADO>();

                MOS.Filter.HisSereServFilter sereServViewFilter = new MOS.Filter.HisSereServFilter();
                sereServViewFilter.SERVICE_REQ_ID = this.serviceReqId;
                var sereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                if (sereServs != null && sereServs.Count > 0)
                {
                    // lấy sereServExt
                    MOS.Filter.HisSereServExtFilter hisSereServExtFilter = new HisSereServExtFilter();
                    hisSereServExtFilter.SERE_SERV_IDs = sereServs.Select(o => o.ID).Distinct().ToList();
                    var sereServExts = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, hisSereServExtFilter, null);

                    foreach (var item in sereServs)
                    {
                        ADO.HisSereServADO serviceReqDetailSdo = new ADO.HisSereServADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisSereServADO>(serviceReqDetailSdo, item);
                        serviceReqDetailSdo.IsNotUseBhyt = item.IS_NOT_USE_BHYT == 1;
                        if (currentHisPatientTypeAlter != null && HisConfigCFG.IsSetPrimaryPatientType == "2")
                            serviceReqDetailSdo.PRIMARY_PATIENT_TYPE_ID = currentHisPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID;
                        else
                            serviceReqDetailSdo.PRIMARY_PATIENT_TYPE_ID = item.PRIMARY_PATIENT_TYPE_ID;

                        if (!item.PRIMARY_PATIENT_TYPE_ID.HasValue)
                            serviceReqDetailSdo.IsNotSetPrimaryPatientTypeId = true;
                        if (serviceReqDetailSdo.PRIMARY_PATIENT_TYPE_ID.HasValue && item.PATIENT_TYPE_ID == serviceReqDetailSdo.PRIMARY_PATIENT_TYPE_ID)
                            serviceReqDetailSdo.PRIMARY_PATIENT_TYPE_ID = null;

                        var service = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID && o.IS_ACTIVE == 1);
                        if (service != null)
                            serviceReqDetailSdo.IsAllowExpend = service.IS_ALLOW_EXPEND;

                        serviceReqDetailSdo.ExecuteRoomId = this.HisServiceReq.EXECUTE_ROOM_ID;
                        serviceReqDetailSdo.serviceType = Base.GlobalStore.SERE_SERV_TYPE;
                        if (sereServExts != null && sereServExts.Count > 0)
                        {
                            var sereServExt = sereServExts.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                            serviceReqDetailSdo.Instruction_Note = sereServExt != null ? sereServExt.INSTRUCTION_NOTE : "";
                        }

                        serviceReqDetailSdo.SERVICE_NAME_HIDDEN = convertToUnSign3(serviceReqDetailSdo.SERVICE_NAME) + serviceReqDetailSdo.SERVICE_NAME;
                        serviceReqDetailSdo.SERVICE_CODE_HIDDEN = convertToUnSign3(serviceReqDetailSdo.SERVICE_CODE) + serviceReqDetailSdo.SERVICE_CODE;

                        if (item.PACKAGE_ID.HasValue)
                        {
                            var package = BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.ID == item.PACKAGE_ID);
                            if (package != null)
                            {
                                serviceReqDetailSdo.PACKAGE_NAME = package.PACKAGE_NAME;
                                serviceReqDetailSdo.IS_NOT_FIXED_SERVICE = !package.IS_NOT_FIXED_SERVICE.HasValue;
                            }
                        }

                        listSereServCurrent.Add(serviceReqDetailSdo);
                    }
                }

                // lấy các service thuộc room đang chọn
                listSereServAdd = null;
                if (cboRoom.EditValue == null)
                {
                    GridControlService.DataSource = null;
                    return;
                }
                MOS.Filter.HisServiceRoomViewFilter serviceRoomViewFilter = new MOS.Filter.HisServiceRoomViewFilter();
                serviceRoomViewFilter.ROOM_ID = (long)cboRoom.EditValue;
                serviceRoomViewFilter.IS_LEAF = true;
                List<long> serviceTypeIds = Base.GlobalStore.MAPPING[this.HisServiceReq.SERVICE_REQ_TYPE_ID];
                serviceRoomViewFilter.SERVICE_TYPE_IDs = serviceTypeIds;
                var listServiceRooms = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_ROOM_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, serviceRoomViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (listServiceRooms != null && listServiceRooms.Count > 0 && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    listServiceRooms = listServiceRooms.Where(o => Base.GlobalStore.HisVServicePatys != null
                        && Base.GlobalStore.HisVServicePatys.Any(a =>
                            a.SERVICE_ID == o.SERVICE_ID
                            && (currentPatientTypeWithPatientTypeAlter.Exists(p => p.ID == a.PATIENT_TYPE_ID) || (a.INHERIT_PATIENT_TYPE_IDS != null && currentPatientTypeWithPatientTypeAlter.Exists(p => ("," + a.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + p.ID + ","))))
                            && a.IS_ACTIVE == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonNumberTrue
                            && a.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()
                            && ((!a.TREATMENT_TO_TIME.HasValue || a.TREATMENT_TO_TIME.Value >= currentHisTreatment.IN_TIME) || (!a.TO_TIME.HasValue || a.TO_TIME.Value >= instructionTime))
                        )).ToList();
                    listSereServAdd = new List<HisSereServADO>();
                    foreach (var item in listServiceRooms)
                    {
                        ADO.HisSereServADO serviceRoomSdo = new ADO.HisSereServADO();

                        var service = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID && o.IS_ACTIVE == 1);

                        long? exeServiceModuleId = null;
                        if (service != null)
                        {
                            serviceRoomSdo.IsAllowExpend = service.IS_ALLOW_EXPEND;

                            if (service.EXE_SERVICE_MODULE_ID.HasValue)
                            {
                                exeServiceModuleId = service.EXE_SERVICE_MODULE_ID.Value;
                            }
                            else if (service.SETY_EXE_SERVICE_MODULE_ID.HasValue)
                            {
                                exeServiceModuleId = service.SETY_EXE_SERVICE_MODULE_ID.Value;
                            }

                            if (!HisServiceReq.EXE_SERVICE_MODULE_ID.HasValue
                                || (exeServiceModuleId.HasValue && exeServiceModuleId == HisServiceReq.EXE_SERVICE_MODULE_ID))
                            {
                                serviceRoomSdo.SERVICE_ID = service.ID;
                                serviceRoomSdo.SERVICE_CODE = service.SERVICE_CODE;
                                serviceRoomSdo.SERVICE_NAME = service.SERVICE_NAME;
                                serviceRoomSdo.TDL_SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                                serviceRoomSdo.serviceType = Base.GlobalStore.SERVICE_ROOM_TYPE;
                                serviceRoomSdo.ExecuteRoomId = this.HisServiceReq.EXECUTE_ROOM_ID;
                                serviceRoomSdo.isMulti = (item.IS_MULTI_REQUEST ?? 0) == IS_TRUE;
                                serviceRoomSdo.SERVICE_NAME_HIDDEN = convertToUnSign3(serviceRoomSdo.SERVICE_NAME) + serviceRoomSdo.SERVICE_NAME;
                                serviceRoomSdo.SERVICE_CODE_HIDDEN = convertToUnSign3(serviceRoomSdo.SERVICE_CODE) + serviceRoomSdo.SERVICE_CODE;
                                serviceRoomSdo.BILL_PATIENT_TYPE_ID = service.BILL_PATIENT_TYPE_ID;
                                serviceRoomSdo.IS_NOT_CHANGE_BILL_PATY = service.IS_NOT_CHANGE_BILL_PATY;
                                serviceRoomSdo.DEFAULT_PATIENT_TYPE_ID = service.DEFAULT_PATIENT_TYPE_ID;
                                listSereServAdd.Add(serviceRoomSdo);
                            }
                        }
                    }
                }
                if (listSereServAdd != null && listSereServAdd.Count > 0)
                    allSereServ.AddRange(listSereServAdd);

                foreach (var sereServ in allSereServ)
                {
                    var current = listSereServCurrent.FirstOrDefault(o => o.SERVICE_ID == sereServ.SERVICE_ID);
                    if (current != null) // load doi tuong da chon
                    {
                        sereServ.ID = current.ID;
                        sereServ.IsChecked = true;
                        sereServ.AMOUNT = current.AMOUNT;
                        sereServ.IsNotSetPrimaryPatientTypeId = current.IsNotSetPrimaryPatientTypeId;
                        sereServ.PRICE = current.PRICE;
                        sereServ.Instruction_Note = current.Instruction_Note;
                        sereServ.PATIENT_TYPE_ID = current.PATIENT_TYPE_ID;
                        sereServ.PRIMARY_PATIENT_TYPE_ID = current.PRIMARY_PATIENT_TYPE_ID;
                        sereServ.TDL_INTRUCTION_TIME = current.TDL_INTRUCTION_TIME;
                        sereServ.serviceType = current.serviceType;
                        sereServ.USER_PRICE = current.USER_PRICE;
                        sereServ.PACKAGE_NAME = current.PACKAGE_NAME;
                        sereServ.IS_NOT_FIXED_SERVICE = current.IS_NOT_FIXED_SERVICE;
                        sereServ.PACKAGE_ID = current.PACKAGE_ID;
                        if (current.IS_EXPEND == IS_TRUE)
                        {
                            sereServ.IsExpend = true;
                        }
                        if (current.IS_NOT_USE_BHYT == IS_TRUE)
                        {
                            sereServ.IsNotUseBhyt = true;
                        }
                        if (current.IS_OUT_PARENT_FEE == IS_TRUE)
                        {
                            sereServ.IsOutKtcFee = true;
                        }
                        var checkService = lstService.Find(o => o.ID == sereServ.SERVICE_ID);
                        if (checkService != null)
                        {
                            if (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_TYPE_IDS) || IsContainString(checkService.APPLIED_PATIENT_TYPE_IDS, sereServ.PATIENT_TYPE_ID.ToString()))
                            {
                                sereServ.IsContainAppliedPatientType = true;
                            }
                            if (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_CLASSIFY_IDS) || (currentHisTreatment.TDL_PATIENT_CLASSIFY_ID != null && IsContainString(checkService.APPLIED_PATIENT_CLASSIFY_IDS, currentHisTreatment.TDL_PATIENT_CLASSIFY_ID.ToString())))
                            {
                                sereServ.IsContainAppliedPatientClassifyType = true;
                            }

                        }
                        sereServ.ASSIGN_NUM_ORDER = current.ASSIGN_NUM_ORDER;
                        sereServ.AssignNumOrder = current.ASSIGN_NUM_ORDER;
                        List<long> hasPatyPatientTypeIds = Base.GlobalStore.HisPatientTypes.Where(o =>
                        Base.GlobalStore.HisVServicePatys != null
                        && Base.GlobalStore.HisVServicePatys.Any(a =>
                            a.SERVICE_ID == sereServ.SERVICE_ID
                            && ((a.PATIENT_TYPE_ID == o.ID) || (a.INHERIT_PATIENT_TYPE_IDS != null && ("," + a.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + o.ID + ",")))
                        )).Select(s => s.ID).ToList();
                        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = currentPatientTypeWithPatientTypeAlter.Where(o => hasPatyPatientTypeIds.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();
                        if (dataCombo != null && dataCombo.Count > 0)
                        {
                            if (HisConfigCFG.IsSetPrimaryPatientType == "1"
                                && sereServ.BILL_PATIENT_TYPE_ID.HasValue
                                && sereServ.PATIENT_TYPE_ID != sereServ.BILL_PATIENT_TYPE_ID.Value
                                && basePatientTypeId(sereServ.PATIENT_TYPE_ID) != sereServ.BILL_PATIENT_TYPE_ID.Value
                                && dataCombo.Exists(o => o.ID == sereServ.BILL_PATIENT_TYPE_ID.Value)
                                && sereServ.IsContainAppliedPatientType
                                &&sereServ.IsContainAppliedPatientClassifyType)
                            {
                                sereServ.IsNotChangePrimaryPaty = (sereServ.IS_NOT_CHANGE_BILL_PATY == (short)1);

                            }
                        }

                        ValidServiceDetailProcessing(current);
                    }

                    long time = this.instructionTime > 0 ? this.instructionTime : Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd"));

                    var sereServReplate = this.sereServWithTreatment.Where(o => o.SERVICE_ID == sereServ.SERVICE_ID && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == time.ToString().Substring(0, 8)).ToList();
                    if (sereServReplate != null && sereServReplate.Count > 0)
                    {
                        sereServ.IsAssignDay = true;
                    }
                    sereServ.IsNotLoadDefaultPatientType = false;
                }

                WaitingManager.Hide();
                GridControlService.BeginUpdate();
                GridControlService.DataSource = null;
                allSereServ = (allSereServ != null && allSereServ.Count > 0) ? allSereServ.OrderByDescending(o => o.IsChecked).ThenBy(o => o.SERVICE_NAME).ToList() : allSereServ;
                if (toggleSwitch.IsOn)
                {
                    var data = (allSereServ != null && allSereServ.Count > 0) ? allSereServ.Where(o => o.IsChecked == true).ToList() : null;
                    GridControlService.DataSource = data;
                }
                else
                {
                    GridControlService.DataSource = allSereServ;
                }

                GridControlService.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void SetAssignNumOrder(HisSereServADO sereServ)
        {
            try
            {
                if (!sereServ.IsChecked)
                {
                    sereServ.AssignNumOrder = 0;
                    return;
                }
                var ss = sereServWithTreatment.Where(o => o.SERVICE_ID == sereServ.SERVICE_ID && o.TDL_INTRUCTION_TIME <= instructionTime).ToList();
                sereServ.AssignNumOrder = sereServ.ASSIGN_NUM_ORDER == null || sereServ.ASSIGN_NUM_ORDER == 0 ? ss != null && ss.Count > 0 ? ss.Count + 1 : 1 : sereServ.ASSIGN_NUM_ORDER;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string convertToUnSign3(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return "";

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private void CheckServiceSameByServiceId(ADO.HisSereServADO sereServADO, List<V_HIS_SERVICE_SAME> serviceSameAll, ref List<HIS_SERE_SERV> result, ref List<ADO.HisSereServADO> resultSelect)
        {
            try
            {
                result = null;
                resultSelect = null;
                if (sereServADO != null && serviceSameAll != null && serviceSameAll.Count > 0)
                {
                    //Lay ra cac dich vu cung co che voi dich vu dang duoc chon
                    //Lay cac dich vu cung co che voi no
                    List<long> serviceSameId1s = serviceSameAll
                        .Where(o => o.SERVICE_ID == sereServADO.SERVICE_ID && o.SAME_ID != sereServADO.SERVICE_ID)
                        .Select(o => o.SAME_ID).ToList();
                    //Hoac cac dich vu ma no cung co che
                    List<long> serviceSameId2s = serviceSameAll
                        .Where(o => o.SAME_ID == sereServADO.SERVICE_ID && o.SERVICE_ID != sereServADO.SERVICE_ID)
                        .Select(o => o.SERVICE_ID).ToList();

                    List<long> serviceSameIds = new List<long>();

                    if (serviceSameId1s != null)
                    {
                        serviceSameIds.AddRange(serviceSameId1s);
                    }
                    if (serviceSameId2s != null)
                    {
                        serviceSameIds.AddRange(serviceSameId2s);
                    }
                    result = new List<HIS_SERE_SERV>();

                    if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count() > 0)
                    {
                        var checkServiceSame = this.sereServWithTreatment.Where(o => serviceSameIds.Contains(o.SERVICE_ID));

                        if (checkServiceSame != null && checkServiceSame.Count() > 0)
                        {

                            var groupServiceSame = checkServiceSame.GroupBy(o => o.SERVICE_ID).ToList();
                            foreach (var serviceSameItems in groupServiceSame)
                            {
                                result.Add(serviceSameItems.FirstOrDefault());
                            }
                        }
                        else
                        {
                            result = null;
                        }
                    }

                    List<ADO.HisSereServADO> serviceCheckeds__Send = this.allSereServ.FindAll(o => o.IsChecked);
                    if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                    {
                        var checkServiceSame = serviceCheckeds__Send.Where(o => serviceSameIds.Contains(o.SERVICE_ID));
                        resultSelect = new List<ADO.HisSereServADO>();
                        if (checkServiceSame != null && checkServiceSame.Count() > 0)
                        {
                            var groupServiceSame = checkServiceSame.GroupBy(o => o.SERVICE_ID).ToList();
                            foreach (var serviceSameItems in groupServiceSame)
                            {
                                resultSelect.Add(serviceSameItems.FirstOrDefault());
                            }
                        }
                        else
                        {
                            resultSelect = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidServiceDetailProcessing(ADO.HisSereServADO sereServADO)
        {
            try
            {
                this.ValidServiceDetailProcessing(sereServADO, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidServiceDetailProcessing(ADO.HisSereServADO sereServADO, bool isValidExecuteRoom)
        {
            try
            {
                if (sereServADO != null)
                {
                    List<HIS_SERE_SERV> serviceSames = null;
                    List<ADO.HisSereServADO> serviceSameResult = null;
                    CheckServiceSameByServiceId(sereServADO, ServiceSameADO.ServiceSameAllADOs, ref serviceSames, ref serviceSameResult);
                    var existsSereServInDate = this.sereServWithTreatment.Any(o => o.SERVICE_ID == sereServADO.SERVICE_ID && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == instructionTime.ToString().Substring(0, 8));

                    if (existsSereServInDate && (serviceSames != null && serviceSames.Count > 0))
                    {
                        sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuVaDichVuCungCoCheDaChiDinhTrongNgay, string.Join("; ", serviceSames.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
                    else if (serviceSames != null && serviceSames.Count > 0)
                    {
                        sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuCungCoCheDaChiDinhTrongNgay, string.Join("; ", serviceSames.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
                    else if (serviceSameResult != null && serviceSameResult.Count > 0)
                    {
                        sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuCungCoChe, string.Join("; ", serviceSameResult.Select(o => o.SERVICE_NAME).ToArray()));
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
                    else
                    {
                        sereServADO.ErrorMessageIsAssignDay = "";
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.None;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(repositoryItemCboPatientType, data, controlEditorADO);
                }
                else
                    ControlEditorLoader.Load(repositoryItemCboPatientType, glstPatientType, controlEditorADO);

                ControlEditorLoader.Load(repositoryItemCustomCboDisable, glstPatientType, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboPrimaryPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(repositoryItemCboPrimaryPatientType, data, controlEditorADO);
                }
                else
                    ControlEditorLoader.Load(repositoryItemCboPrimaryPatientType, glstPrimaryPatientType, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void LoadAppliedPatientType(long patientTypeId, long serviceId, ref HisSereServADO sereServADO)
        {
            try
            {
                if (serviceId > 0)
                {
                    var checkService = lstService.Find(o => o.ID == serviceId);
                    if (checkService != null)
                    {
                        if (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_TYPE_IDS) || IsContainString(checkService.APPLIED_PATIENT_TYPE_IDS, patientTypeId.ToString()))
                        {
                            sereServADO.IsContainAppliedPatientType = true;
                        }
                        else
                        {
                            sereServADO.IsContainAppliedPatientType = false;

                        }
                        if (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_CLASSIFY_IDS) || (currentHisTreatment.TDL_PATIENT_CLASSIFY_ID != null && IsContainString(checkService.APPLIED_PATIENT_CLASSIFY_IDS, currentHisTreatment.TDL_PATIENT_CLASSIFY_ID.ToString())))
                        {
                            sereServADO.IsContainAppliedPatientClassifyType = true;
                        }
                        else
                        {
                            sereServADO.IsContainAppliedPatientClassifyType = false;

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Event
        private void ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, ADO.HisSereServADO sereServADO)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                if (Base.GlobalStore.HisPatientTypes != null && Base.GlobalStore.HisPatientTypes.Count > 0)
                {
                    this.LoadAppliedPatientType(patientTypeId, sereServADO.SERVICE_ID, ref sereServADO);
                    //var arrPatientTypeid = Base.GlobalStore.HisVServicePatys.Where(o => o.SERVICE_ID == serviceId).Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                    List<long> hasPatyPatientTypeIds = Base.GlobalStore.HisPatientTypes.Where(o =>
                        Base.GlobalStore.HisVServicePatys != null
                        && Base.GlobalStore.HisVServicePatys.Any(a =>
                            a.SERVICE_ID == serviceId
                            && ((a.PATIENT_TYPE_ID == o.ID) || (a.INHERIT_PATIENT_TYPE_IDS != null && ("," + a.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + o.ID + ",")))
                        )).Select(s => s.ID).ToList();

                    if (currentPatientTypeWithPatientTypeAlter != null
                        && hasPatyPatientTypeIds != null
                        && hasPatyPatientTypeIds.Count > 0)
                    {
                        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = currentPatientTypeWithPatientTypeAlter.Where(o => hasPatyPatientTypeIds.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();

                        var service = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(o => o.ID == serviceId && o.IS_ACTIVE == 1);
                        var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && o.IS_ACTIVE == 1);
                        if (service != null && service.DO_NOT_USE_BHYT == 1 && employee != null && employee.IS_ADMIN != 1)
                        {
                            dataCombo = dataCombo.Where(o => o.ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();
                        }
                        if (dataCombo != null && dataCombo.Count > 0)
                        {
                            if (this.currentWorkingRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                                         && this.currentWorkingRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT
                                         && dataCombo.Exists(e => e.ID == this.currentWorkingRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                            {
                                result = dataCombo.FirstOrDefault(o => o.ID == this.currentWorkingRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                                Inventec.Common.Logging.LogSystem.Debug("2.1_______Loai dich vụ mặc định với phòng có cấu hình đối tượng thanh toán: " + this.currentWorkingRoom.DEFAULT_INSTR_PATIENT_TYPE_ID + " data: " + Inventec.Common.Logging.LogUtil.TraceData("_result", result));
                            }
                            else if (HisConfigCFG.IsSetPrimaryPatientType != "1"
                               && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                               && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT
                               && dataCombo.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                            {
                                result = dataCombo.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                                Inventec.Common.Logging.LogSystem.Debug("1.1_______Loai dich vụ mặc định với phòng không có cấu hình đối tượng thanh toán: " + this.currentWorkingRoom.DEFAULT_INSTR_PATIENT_TYPE_ID + " data: " + Inventec.Common.Logging.LogUtil.TraceData("_result", result));
                            }
                            else
                            {
                                result = (dataCombo != null ? (dataCombo.FirstOrDefault(o => o.ID == patientTypeId) ?? dataCombo[0]) : null);
                                Inventec.Common.Logging.LogSystem.Debug("1.2_______Loai dich vụ mặc định với phòng không có cấu hình đối tượng thanh toán: " + this.currentWorkingRoom.DEFAULT_INSTR_PATIENT_TYPE_ID + " data: " + Inventec.Common.Logging.LogUtil.TraceData("_result", result));
                            }
                            Inventec.Common.Logging.LogSystem.Debug("1.3_______Loai dich vụ mặc định với phòng không có cấu hình đối tượng thanh toán: " + result.PATIENT_TYPE_NAME);
                            
                            
                            if (sereServADO.DEFAULT_PATIENT_TYPE_ID != null && dataCombo.FirstOrDefault(o => o.ID == sereServADO.DEFAULT_PATIENT_TYPE_ID.Value) != null && !sereServADO.IsNotLoadDefaultPatientType)
                            {
                                result = dataCombo.FirstOrDefault(o => o.ID == sereServADO.DEFAULT_PATIENT_TYPE_ID.Value);
                            }

                            if (result != null && sereServADO != null)
                            {
                                sereServADO.PATIENT_TYPE_ID = result.ID;
                            }

                            if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = this.currentHisPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID;
                            }
                            else if (HisConfigCFG.IsSetPrimaryPatientType == "1"
                                && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                                && sereServADO.PATIENT_TYPE_ID != sereServADO.BILL_PATIENT_TYPE_ID.Value
                                && basePatientTypeId(sereServADO.PATIENT_TYPE_ID) != sereServADO.BILL_PATIENT_TYPE_ID.Value
                                && dataCombo.Exists(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value)
                                && sereServADO.IsContainAppliedPatientType
                                && sereServADO.IsContainAppliedPatientClassifyType)
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = sereServADO.BILL_PATIENT_TYPE_ID;
                                sereServADO.IsNotChangePrimaryPaty = (sereServADO.IS_NOT_CHANGE_BILL_PATY == (short)1);
                            }
                            else if (HisConfigCFG.IsSetPrimaryPatientType == "1"
                                && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                                && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT
                                && dataCombo.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                                && result.ID != this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID;
                            }
                            else
                            {
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                            }

                            if (sereServADO.PRIMARY_PATIENT_TYPE_ID.HasValue && sereServADO.PATIENT_TYPE_ID == sereServADO.PRIMARY_PATIENT_TYPE_ID)
                                sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private long basePatientTypeId(long patientTypeId)
        {
            long basePatientTypeId = 0;
            try
            {
                HIS_PATIENT_TYPE paty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeId);
                basePatientTypeId = paty != null ? (paty.BASE_PATIENT_TYPE_ID ?? 0) : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return basePatientTypeId;
        }
        private bool IsBhytOrVp(long patientTypeId)
        {
            try
            {
                HIS_PATIENT_TYPE paty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeId);
                long basePatientTypeId = paty != null ? (paty.BASE_PATIENT_TYPE_ID ?? 0) : 0;
                if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT
                    || patientTypeId == HisConfigCFG.PatientTypeId__VP
                    || basePatientTypeId == HisConfigCFG.PatientTypeId__BHYT
                    || basePatientTypeId == HisConfigCFG.PatientTypeId__VP)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private void toggleSwitch_Toggled(object sender, EventArgs e)
        {
            try
            {
                ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                allSereServ = allSereServ != null && allSereServ.Count > 0 ? allSereServ.OrderByDescending(o => o.IsChecked).ThenBy(p => p.SERVICE_NAME).ToList() : allSereServ;
                if (toggleSwitch.IsOn)
                {
                    var data = GridControlService.DataSource as List<ADO.HisSereServADO>;
                    data = data.Where(o => o.IsChecked == true).ToList();
                    GridControlService.DataSource = data;
                }
                else GridControlService.DataSource = allSereServ;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(ADO.HisSereServADO data, DevExpress.XtraEditors.GridLookUpEdit patientTypeCombo, bool isPatientType = false)
        {
            try
            {
                if (Base.GlobalStore.HisVServicePatys != null && Base.GlobalStore.HisVServicePatys.Count > 0)
                {
                    List<string> hasPatyPatientTypeCodes = Base.GlobalStore.HisPatientTypes.Where(o =>
                             Base.GlobalStore.HisVServicePatys != null
                             && Base.GlobalStore.HisVServicePatys.Any(a =>
                                 a.SERVICE_ID == data.SERVICE_ID
                                 && ((a.PATIENT_TYPE_ID == o.ID) || (a.INHERIT_PATIENT_TYPE_IDS != null && ("," + a.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + o.ID + ",")))
                             )).Select(s => s.PATIENT_TYPE_CODE).ToList();
                    //var arrPatientTypeCode = Base.GlobalStore.HisVServicePatys.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).Select(o => o.PATIENT_TYPE_CODE).ToList();
                    if (hasPatyPatientTypeCodes != null && hasPatyPatientTypeCodes.Count > 0)
                    {
                        var PatientType = glstPatientType.Where(o => hasPatyPatientTypeCodes.Contains(o.PATIENT_TYPE_CODE)).ToList();

                        var service = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(o => o.ID == data.SERVICE_ID && o.IS_ACTIVE == 1);
                        var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && o.IS_ACTIVE == 1);
                        if (isPatientType && service != null && service.DO_NOT_USE_BHYT == 1 && employee != null && employee.IS_ADMIN != 1)
                        {
                            PatientType = PatientType.Where(o => o.ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();
                        }
                        InitComboPatientType(patientTypeCombo, PatientType);
                    }
                    else
                    {
                        InitComboPatientType(patientTypeCombo, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPrimaryPatientTypeCombo(ADO.HisSereServADO data, DevExpress.XtraEditors.GridLookUpEdit patientTypeCombo)
        {
            try
            {
                if (Base.GlobalStore.HisVServicePatys != null && Base.GlobalStore.HisVServicePatys.Count > 0)
                {
                    List<string> hasPatyPatientTypeCodes = Base.GlobalStore.HisPatientTypes.Where(o =>
                          Base.GlobalStore.HisVServicePatys != null
                          && Base.GlobalStore.HisVServicePatys.Any(a =>
                              a.SERVICE_ID == data.SERVICE_ID
                              && ((a.PATIENT_TYPE_ID == o.ID) || (a.INHERIT_PATIENT_TYPE_IDS != null && ("," + a.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + o.ID + ",")))
                          )).Select(s => s.PATIENT_TYPE_CODE).ToList();

                    //var arrPatientTypeCode = Base.GlobalStore.HisVServicePatys.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).Select(o => o.PATIENT_TYPE_CODE).ToList();
                    if (hasPatyPatientTypeCodes != null && hasPatyPatientTypeCodes.Count > 0)
                    {
                        if (data.PATIENT_TYPE_ID <= 0)
                            throw new Exception("Chua chon doi tuong thanh toan");

                        var PatientType = glstPrimaryPatientType.Where(o => hasPatyPatientTypeCodes.Contains(o.PATIENT_TYPE_CODE)
                            && data.PATIENT_TYPE_ID != o.ID
                            && o.ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();

                        //Load chinh sach gia
                        List<V_HIS_SERVICE_PATY> servicePatys = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_PATY>();
                        if (servicePatys == null || servicePatys.Count == 0)
                            throw new Exception("Khong lay duoc du lieu chinh sach gia");

                        V_HIS_SERVICE_PATY servicePatyDTTT = servicePatys.FirstOrDefault(o => (o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID || (o.INHERIT_PATIENT_TYPE_IDS != null && ("," + o.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + data.PATIENT_TYPE_ID + ","))) && data.SERVICE_ID == o.SERVICE_ID);
                        decimal priceServicePatyDTTT = servicePatyDTTT != null ? servicePatyDTTT.PRICE : 0;

                        List<V_HIS_SERVICE_PATY> servicePatyTemps = servicePatys.Where(o => PatientType.Any(a => a.ID == o.PATIENT_TYPE_ID || (o.INHERIT_PATIENT_TYPE_IDS != null && ("," + o.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + a.ID + ",")))
                            && data.SERVICE_ID == o.SERVICE_ID
                            && o.PRICE > priceServicePatyDTTT).ToList();

                        PatientType = PatientType.Where(o => servicePatyTemps.Select(p => p.PATIENT_TYPE_ID).Contains(o.ID)).ToList();

                        InitComboPatientType(patientTypeCombo, PatientType);
                    }
                    else
                    {
                        InitComboPatientType(patientTypeCombo, null);
                    }
                }
            }
            catch (Exception ex)
            {
                InitComboPatientType(patientTypeCombo, null);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBoSungPhacDo_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    GridViewService.PostEditor();
                    var data = GridControlService.DataSource as List<ADO.HisSereServADO>;
                    var lstdata = data.Where(o => o.IsChecked == true).ToList();
                    // gan du lieu cho sereServ Delete
                    List<long> SereServDeleteIds = lstdata.Where(o => o.serviceType == Base.GlobalStore.SERE_SERV_TYPE).Select(o => o.ID).ToList();
                    var sereServDeleteInputDatas = this.listSereServCurrent.Where(o => !SereServDeleteIds.Contains(o.ID)).ToList();
                    // gan du lieu cho service Req add
                    SereServAdditonSdos = lstdata.Where(o => o.serviceType == Base.GlobalStore.SERVICE_ROOM_TYPE).ToList();
                    //Lay danh sach icd
                    List<HIS_ICD> icdArr = getIcdListFromUcIcd();

                    if (icdArr == null || icdArr.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy thông tin ICD", "Thông báo",
                                        MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    if (this.SereServAdditonSdos == null || this.SereServAdditonSdos.Count == 0)
                    {
                        MessageBox.Show("Vui lòng chọn dịch vụ", "Thông báo",
                                        MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    List<long> serviceIds = this.SereServAdditonSdos.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).ToList();
                    CommonParam param = new CommonParam();
                    HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                    icdServiceFilter.SERVICE_IDs = serviceIds;
                    icdServiceFilter.ICD_CODE__EXACTs = icdArr.Select(o => o.ICD_CODE).Distinct().ToList();
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
                        MessageBox.Show("Không tìm thấy dịch vụ chưa được cấu hình", "Thông báo",
                                        MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }

                    List<HIS_ICD> icds = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && icdArr.Select(p => p.ICD_CODE).Distinct().ToList().Contains(o.ICD_CODE)).Distinct().ToList();
                    if (icds == null || icds.Count == 0)
                    {
                        LogSystem.Debug("Khong tim thay ICD");
                        return;
                    }

                    if (icds.Count == 1)
                    {
                        icdChoose = icds[0];
                    }
                    else
                    {
                        //Mo form chon icd
                        icdChoose = new HIS_ICD();
                        frmChooseICD frm = new frmChooseICD(icds, refeshChooseIcd);
                        frm.ShowDialog();
                    }

                    if (icdChoose == null || (icdChoose != null && icdChoose.ID == 0))
                        return;

                    List<object> listObj = new List<object>();
                    listObj.Add(icdChoose);
                    listObj.Add(serviceNotConfigIds);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceIcd", currentModule.RoomId, currentModule.RoomTypeId, listObj);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void refeshChooseIcd(object data)
        {
            try
            {
                if (data != null && data is ICDADO)
                {
                    icdChoose = new HIS_ICD();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ICD>(icdChoose, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType(GridLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetError(int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {
                if (column.FieldName == "PATIENT_TYPE_ID")
                {
                    ADO.HisSereServADO data = (ADO.HisSereServADO)GridViewService.GetRow(rowHandle);
                    if (data == null)
                        return string.Empty;

                    if (data.PATIENT_TYPE_ID == 0)
                    {
                        return ResourceMessage.KhongCoDoiTuongThanhToanThuocChiNhanhHienTai;
                    }
                }
                if (column.FieldName == "SERVICE_CODE")
                {
                    ADO.HisSereServADO data = (ADO.HisSereServADO)GridViewService.GetRow(rowHandle);
                    if (data == null)
                        return string.Empty;
                    if (data.IsAssignDay)
                    {
                        return ResourceMessage.DichVuDaDuocChiDinhTrongNgay;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return string.Empty;
        }

        private void UpdatePayslipInfoProcess(MOS.SDO.HisSereServPayslipSDO hisSereServPayslipSDO)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServResults = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/UpdatePayslipInfo", ApiConsumer.ApiConsumers.MosConsumer, hisSereServPayslipSDO, param);
                if (sereServResults != null && sereServResults.Count > 0)
                {
                    success = true;
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static bool UpdatePatientType(MOS.EFMODEL.DataModels.HIS_SERE_SERV hisSereServ, ref CommonParam param)
        {
            bool success = false;
            try
            {
                WaitingManager.Show();
                if (hisSereServ != null)
                {
                    MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                    var sereServUpdate = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERE_SERV>(ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, hisSereServ, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (sereServUpdate != null)
                    {
                        success = true;
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
            return success;
        }

        private void GridViewService_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (ADO.HisSereServADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "PATIENT_TYPE_ID")
                        {
                            if (data.IS_NOT_FIXED_SERVICE)
                                e.RepositoryItem = repositoryItemCustomCboDisable;
                            else
                                e.RepositoryItem = repositoryItemCboPatientType;
                        }
                        else if (e.Column.FieldName == "PRIMARY_PATIENT_TYPE_ID")
                        {
                            if (data.IS_NOT_FIXED_SERVICE || data.IsNotChangePrimaryPaty)
                                e.RepositoryItem = repositoryItemCustomCboDisable;
                            else
                                e.RepositoryItem = repositoryItemCboPrimaryPatientType;
                        }
                        else if (e.Column.FieldName == "IsExpend")
                        {
                            if (data.IsAllowExpend != 1 || data.IS_NOT_FIXED_SERVICE)
                            {
                                e.RepositoryItem = ButtonEdit_IsExpendDisable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheckExpend;
                            }
                        }
                        else if (e.Column.FieldName == "AMOUNT")
                        {
                            if (!data.isMulti)
                            {
                                e.RepositoryItem = repositoryItemSpinAmountDisable;
                            }
                            else
                                e.RepositoryItem = repositoryItemSpinAmount;
                        }
                        else if (e.Column.FieldName == "IsOutKtcFee")
                        {
                            if (data.IS_NOT_FIXED_SERVICE)
                                e.RepositoryItem = ButtonEdit_IsExpendDisable;
                            else
                                e.RepositoryItem = repositoryItemCheckIsOutKtcFee;
                        }
                        else if (e.Column.FieldName == gc_Check.FieldName)
                        {
                            if (data.PACKAGE_ID.HasValue)
                                e.RepositoryItem = repositoryItemChk_IsChecked_Disable;
                            else
                                e.RepositoryItem = CheckEdit_IsChecked;
                        }
                        else if (e.Column.FieldName == "IsNotUseBhyt")
                        {
                            if (data.PATIENT_TYPE_ID != 1 && data.IsChecked)
                                e.RepositoryItem = repositoryItemCheckIsNotUseBhyt;
                            else
                                e.RepositoryItem = repositoryItemCheckIsNotUseBhytDis;
                        }
                        else if (e.Column.FieldName == "AssignNumOrder")
                        {
                            if (data.IsChecked)
                                e.RepositoryItem = repositoryItemSpinAmount;
                            else
                                e.RepositoryItem = repositoryItemSpinAmountDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridViewService_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        ADO.HisSereServADO data_ServiceSDO = (ADO.HisSereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data_ServiceSDO != null && data_ServiceSDO.IsChecked == true)
                        {
                            if (e.Column.FieldName == "PRICE_DISPLAY")
                            {
                                long? patientTypeId = data_ServiceSDO.PRIMARY_PATIENT_TYPE_ID.HasValue
                                    ? data_ServiceSDO.PRIMARY_PATIENT_TYPE_ID.Value : data_ServiceSDO.PATIENT_TYPE_ID > 0 ? (long?)data_ServiceSDO.PATIENT_TYPE_ID : null;

                                if (patientTypeId.HasValue)
                                {
                                    var data_ServicePrice = Base.GlobalStore.HisVServicePatys.Where(o => o.SERVICE_ID == data_ServiceSDO.SERVICE_ID && o.PATIENT_TYPE_ID == patientTypeId.Value).OrderByDescending(m => m.MODIFY_TIME).ToList();
                                    if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                                    {
                                        e.Value = (data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO));
                                    }
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

        private void GridViewService_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.Clicks == 2 && (e.Column.FieldName == "SERVICE_CODE" || e.Column.FieldName == "SERVICE_NAME"))
                {
                    CheckEdit_IsChecked_CheckedChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.HitTest == GridHitTest.Column)
                {
                    if (hi.Column.FieldName == "IsChecked")
                    {
                        GridViewService.BeginUpdate();
                        if (isCheckAll == true)
                        {
                            foreach (var item in this.allSereServ)
                            {
                                item.IsChecked = true;
                                if (item.AMOUNT == 0)
                                    item.AMOUNT = 1;
                                this.SetAssignNumOrder(item);
                                ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, item.SERVICE_ID, item);
                            }

                            isCheckAll = false;
                            if (!VerifyCheckFeeWhileAssign())
                            {
                                foreach (var item in this.allSereServ)
                                {
                                    item.IsChecked = false;
                                    item.AMOUNT = 0;
                                    item.PATIENT_TYPE_ID = 0;
                                    item.AssignNumOrder = null;
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in this.allSereServ)
                            {
                                if (!item.PACKAGE_ID.HasValue)
                                {
                                    item.IsChecked = false;
                                    item.AMOUNT = 0;
                                    item.PATIENT_TYPE_ID = 0;
                                    item.IsExpend = false;
                                    item.IsNotUseBhyt = false;
                                    item.AssignNumOrder = null;
                                }
                            }

                            isCheckAll = true;
                        }

                        GridViewService.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                GridViewService.EndUpdate();
            }
        }

        private void GridViewService_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var IsAssignDay = Inventec.Common.TypeConvert.Parse.ToBoolean((view.GetRowCellValue(e.RowHandle, "IsAssignDay") ?? "").ToString());
                    if (IsAssignDay)
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Red;
                    }

                    var isChecked = (view.GetRowCellValue(e.RowHandle, "IsChecked") ?? "").ToString();

                    if (Inventec.Common.TypeConvert.Parse.ToBoolean(isChecked ?? "") == true)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewService_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    ADO.HisSereServADO data = view.GetFocusedRow() as ADO.HisSereServADO;
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        FillDataIntoPatientTypeCombo(data, editor, true);
                        data.IsNotLoadDefaultPatientType = true;
                        editor.EditValue = data.PATIENT_TYPE_ID;
                        //if (data.PATIENT_TYPE_ID > 0 && HisConfigCFG.IsSetPrimaryPatientType != "2")
                        //{
                        //    data.PRIMARY_PATIENT_TYPE_ID = null;
                        //}

                        if (editor.Properties.DataSource == null)
                        {
                            string error = GetError(GridViewService.FocusedRowHandle, GridViewService.FocusedColumn);
                            if (error == string.Empty) return;
                            GridViewService.SetColumnError(GridViewService.FocusedColumn, error);
                        }
                    }
                }
                else if (view.FocusedColumn.FieldName == "PRIMARY_PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    ADO.HisSereServADO data = view.GetFocusedRow() as ADO.HisSereServADO;
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null && (!data.IsNotChangePrimaryPaty || HisConfigCFG.IsSetPrimaryPatientType == "2"))
                    {

                        FillDataIntoPrimaryPatientTypeCombo(data, editor);

                        if (data.PRIMARY_PATIENT_TYPE_ID != null)
                        {
                            editor.EditValue = data.PRIMARY_PATIENT_TYPE_ID;
                            editor.Properties.Buttons[1].Visible = true;
                            editor.ButtonClick += reposityButtonClick;
                        }

                        if (editor.Properties.DataSource == null)
                        {
                            string error = GetError(GridViewService.FocusedRowHandle, GridViewService.FocusedColumn);
                            if (error == string.Empty) return;
                            GridViewService.SetColumnError(GridViewService.FocusedColumn, error);
                        }
                    }
                    else
                    {
                        editor.ReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridViewService_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "SERVICE_NAME")
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
                var index = this.GridViewService.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.GridControlService.DataSource as List<HisSereServADO>;
                var row = listDatas[index];

                if (e.ColumnName == "SERVICE_NAME")
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

        private void GridViewService_CalcRowHeight(object sender, RowHeightEventArgs e)
        {
            try
            {
                if (GridViewService.IsFilterRow(e.RowHandle))
                {
                    var fontSize = ApplicationFontWorker.GetFontSize();
                    if (fontSize == ApplicationFontConfig.FontSize825)
                    {
                        //txtServiceName_Search.Point = Point(126, 22);
                        //txtServiceCode_Search.Point = Point(44, 22);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize875)
                    {
                        e.RowHeight = 23;
                        //txtServiceName_Search.Location = new Point(126, 24);
                        //txtServiceCode_Search.Location = new Point(44, 24);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize925)
                    {
                        e.RowHeight = 25;
                        //txtServiceName_Search.Location = new Point(126, 26);
                        //txtServiceCode_Search.Location = new Point(44, 26);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize975)
                    {
                        e.RowHeight = 27;
                        //txtServiceName_Search.Location = new Point(126, 28);
                        //txtServiceCode_Search.Location = new Point(44, 28);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize1025)
                    {
                        //txtServiceName_Search.Location = new Point(126, 30);
                        //txtServiceCode_Search.Location = new Point(44, 30);
                        e.RowHeight = 30;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridViewService_ColumnWidthChanged(object sender, ColumnEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "SERVICE_NAME")
                {
                    //    txtServiceName_Search.Width = e.Column.Width - 2;
                }
                else if (e.Column.FieldName == "SERVICE_CODE")
                {
                    //    txtServiceCode_Search.Width = e.Column.Width - 2;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void reposityButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridLookUpEdit editor = sender as GridLookUpEdit;
                if (editor != null)
                {
                    editor.EditValue = null;
                    editor.Properties.Buttons[1].Visible = false;
                }
            }
        }

        private void CheckEdit_IsChecked_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ADO.HisSereServADO)GridViewService.GetFocusedRow();
                if (row != null && !row.PACKAGE_ID.HasValue)
                {
                    //GridViewService.BeginUpdate();
                    row.IsChecked = !row.IsChecked;
                    if (row.IsChecked)
                    {
                        if (row.AMOUNT == 0)
                        {
                            row.AMOUNT = 1;
                        }
                        if (row.PATIENT_TYPE_ID == 0)
                        {
                            ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, row.SERVICE_ID, row);
                        }
                        this.ValidServiceDetailProcessing(row);
                        this.SetAssignNumOrder(row);
                        if (!VerifyCheckFeeWhileAssign())
                        {
                            row.IsChecked = false;
                            row.AMOUNT = 0;
                            row.PRIMARY_PATIENT_TYPE_ID = null;
                            row.PATIENT_TYPE_ID = 0;

                            row.AssignNumOrder = null;
                            GridControlService.RefreshDataSource();

                            return;
                        }

                        if (row.PACKAGE_ID.HasValue)
                        {
                            foreach (var item in allSereServ)
                            {
                                if (item.ID != row.ID && item.PACKAGE_ID == row.PACKAGE_ID)
                                {
                                    item.IsChecked = row.IsChecked;
                                    item.AMOUNT = row.AMOUNT;
                                    item.PRIMARY_PATIENT_TYPE_ID = row.PRIMARY_PATIENT_TYPE_ID;
                                    item.PATIENT_TYPE_ID = row.PATIENT_TYPE_ID;
                                }
                            }
                        }
                    }
                    else
                    {
                        row.AMOUNT = 0;
                        row.PATIENT_TYPE_ID = 0;
                        row.PRIMARY_PATIENT_TYPE_ID = null;
                        row.IsExpend = false;
                        row.IsNotUseBhyt = false;
                        row.ErrorMessageIsAssignDay = "";
                        row.ErrorTypeIsAssignDay = ErrorType.None;
                        row.IsNotChangePrimaryPaty = false;

                        if (row.PACKAGE_ID.HasValue)
                        {
                            foreach (var item in allSereServ)
                            {
                                if (item.ID != row.ID && item.PACKAGE_ID == row.PACKAGE_ID)
                                {
                                    item.IsChecked = row.IsChecked;
                                    item.AMOUNT = row.AMOUNT;
                                    item.PRIMARY_PATIENT_TYPE_ID = row.PRIMARY_PATIENT_TYPE_ID;
                                    item.PATIENT_TYPE_ID = row.PATIENT_TYPE_ID;
                                }
                            }
                        }
                    }
                    GridControlService.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                GridViewService.EndUpdate();
            }
        }

        private void repositoryItemCboPatientType1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit dt = sender as GridLookUpEdit;


                var row = (ADO.HisSereServADO)GridViewService.GetFocusedRow();
                if (row != null && row.serviceType != Base.GlobalStore.SERE_SERV_TYPE)
                {
                    row.serviceType = Base.GlobalStore.SERVICE_ROOM_TYPE;
                }
                if (dt.EditValue != null && Convert.ToInt64(dt.EditValue) == 1 && row.IsNotUseBhyt)
                {
                    if (MessageBox.Show(this, "Bạn đã tích chọn \"Không hưởng BHYT\", nếu đổi đối tượng sang BHYT, phần mềm sẽ tự động bỏ chọn. Bạn có muốn thực hiện không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        GridViewService.SetRowCellValue(GridViewService.FocusedRowHandle, Gc_PatientTypeName, row.PATIENT_TYPE_ID);
                        dt.Focus();
                        return;
                    }
                    else
                    {
                        GridViewService.SetRowCellValue(GridViewService.FocusedRowHandle, g_C_IsNotUserBhyt, false);
                    }
                }

                if (row.PRIMARY_PATIENT_TYPE_ID != null)
                {
                    this.PrimaryPatientTypeId = row.PRIMARY_PATIENT_TYPE_ID;
                }

                if (dt.EditValue != null && row.PRIMARY_PATIENT_TYPE_ID != null && Convert.ToInt64(dt.EditValue) == row.PRIMARY_PATIENT_TYPE_ID)
                {
                    GridViewService.SetRowCellValue(GridViewService.FocusedRowHandle, gridColumnDoiTuongPhuThu, null);
                }
                else if (this.PrimaryPatientTypeId != null)
                {
                    GridViewService.SetRowCellValue(GridViewService.FocusedRowHandle, gridColumnDoiTuongPhuThu, PrimaryPatientTypeId);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MemoExEdit_InstructionNote_Popup(object sender, EventArgs e)
        {
            try
            {
                MemoExPopupForm form = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as MemoExPopupForm;
                form.OkButton.Text = Inventec.Common.Resource.Get.Value("frmAssignServiceEdit.InstructionNoteMemoEx.OkButtion", Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit, LanguageManager.GetCulture()); ;
                form.CloseButton.Text = Inventec.Common.Resource.Get.Value("frmAssignServiceEdit.InstructionNoteMemoEx.CloseButtion", Resources.ResourceLanguageManager.LanguageFormAssignServiceEdit, LanguageManager.GetCulture()); ;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Save
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSave.Visible && btnSave.Enabled)
                    SaveProcess(false);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void getDataFromOtherFormDelegate(object data)
        {
            try
            {
                if (data != null && data is bool)
                {
                    isYes = (bool)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_SERE_SERV> getSereServWithMinDuration(List<HisSereServADO> serviceCheckeds)
        {
            List<HIS_SERE_SERV> listSereServResult = null;
            try
            {
                if (serviceCheckeds != null && serviceCheckeds.Count > 0)
                {
                    // gán dữ liệu min_duration
                    var services = lstService;
                    serviceCheckeds.ForEach(o => o.MIN_DURATION = services.FirstOrDefault(m => m.ID == o.SERVICE_ID).MIN_DURATION);
                    List<HisSereServADO> sereServADOExistMinDUration = serviceCheckeds.Where(o => o.MIN_DURATION != null).ToList();
                    if (sereServADOExistMinDUration != null && sereServADOExistMinDUration.Count > 0)
                    {
                        List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                        foreach (var item in sereServADOExistMinDUration)
                        {
                            ServiceDuration serviceDuration = new ServiceDuration();
                            serviceDuration.ServiceId = item.SERVICE_ID;
                            serviceDuration.MinDuration = (item.MIN_DURATION ?? 0);
                            serviceDurations.Add(serviceDuration);
                        }
                        // gọi api để lấy về thông báo
                        CommonParam param = new CommonParam();
                        HisSereServMinDurationFilter hisSereServMinDurationFilter = new HisSereServMinDurationFilter();
                        hisSereServMinDurationFilter.ServiceDurations = serviceDurations;
                        hisSereServMinDurationFilter.InstructionTime = this.instructionTime;

                        hisSereServMinDurationFilter.PatientId = this.HisServiceReq.TDL_PATIENT_ID;

                        listSereServResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, hisSereServMinDurationFilter, param);

                        var listSereServResultTemp = from SereServResult in listSereServResult
                                                     group SereServResult by SereServResult.SERVICE_ID into g
                                                     orderby g.Key
                                                     select g.FirstOrDefault();
                        listSereServResult = listSereServResultTemp.ToList();
                    }
                    else
                    {
                        listSereServResult = null;
                    }
                }
                else
                {
                    listSereServResult = null;
                }
            }
            catch (Exception ex)
            {
                listSereServResult = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return listSereServResult;
        }

        private void UpdataDataForProcess(MOS.SDO.HisServiceReqUpdateSDO serviceReqUpdate)
        {
            try
            {
                var lstdata = this.allSereServ.Where(o => o.IsChecked == true).ToList();
                // gan du lieu cho sereServ Delete
                List<long> SereServDeleteIds = lstdata.Where(o => o.serviceType == Base.GlobalStore.SERE_SERV_TYPE).Select(o => o.ID).ToList();
                var sereServDeleteInputDatas = this.listSereServCurrent.Where(o => !SereServDeleteIds.Contains(o.ID)).ToList();
                // gan du lieu cho service Req add
                SereServAdditonSdos = lstdata.Where(o => o.serviceType == Base.GlobalStore.SERVICE_ROOM_TYPE).ToList();

                serviceReqUpdate.ServiceReqId = this.HisServiceReq.ID;
                serviceReqUpdate.DeleteSereServIds = new List<long>();
                serviceReqUpdate.InsertServices = new List<MOS.SDO.ServiceReqDetailSDO>();
                serviceReqUpdate.DeleteSereServIds = sereServDeleteInputDatas.Select(o => o.ID).ToList();
                serviceReqUpdate.ExecuteRoomId = (long)cboRoom.EditValue;
                serviceReqUpdate.InstructionTime = instructionTime;
                if (dtUseTime.DateTime != null && dtUseTime.DateTime != DateTime.MinValue && dtUseTime.DateTime != DateTime.MaxValue)
                {
                    serviceReqUpdate.UseTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtUseTime.DateTime);
                }
                else
                { 
                    serviceReqUpdate.UseTime = null;
                }
                foreach (var item in SereServAdditonSdos)
                {
                    MOS.SDO.ServiceReqDetailSDO serviceReqDetail = new MOS.SDO.ServiceReqDetailSDO();
                    serviceReqDetail.Amount = item.AMOUNT;
                    serviceReqDetail.ServiceId = item.SERVICE_ID;
                    serviceReqDetail.RoomId = (long)cboRoom.EditValue;
                    serviceReqDetail.PatientTypeId = item.PATIENT_TYPE_ID;
                    serviceReqDetail.PrimaryPatientTypeId = item.PRIMARY_PATIENT_TYPE_ID;
                    serviceReqDetail.ParentId = item.PARENT_ID;
                    serviceReqDetail.EkipId = item.EKIP_ID;
                    serviceReqDetail.PackageId = item.PACKAGE_ID;
                    serviceReqDetail.ShareCount = item.SHARE_COUNT;
                    serviceReqDetail.UserPrice = item.USER_PRICE;
                    serviceReqDetail.IsNotUseBhyt = item.IsNotUseBhyt;
                    if (item.IsExpend)
                    {
                        serviceReqDetail.IsExpend = IS_TRUE;
                    }
                    if (item.IsOutKtcFee)
                    {
                        serviceReqDetail.IsOutParentFee = IS_TRUE;
                    }
                    serviceReqDetail.InstructionNote = item.Instruction_Note;
                    serviceReqDetail.AssignNumOrder = item.AssignNumOrder;
                    serviceReqUpdate.InsertServices.Add(serviceReqDetail);
                }

                // gan du lieu cho sereServs Update
                serviceReqUpdate.UpdateServices = new List<ServiceReqDetailSDO>();
                List<HisSereServADO> SereServUpdates = lstdata.Where(o => o.serviceType == Base.GlobalStore.SERE_SERV_TYPE).ToList();

                foreach (var item in SereServUpdates)
                {
                    MOS.SDO.ServiceReqDetailSDO serviceReqDetail = new MOS.SDO.ServiceReqDetailSDO();
                    serviceReqDetail.Amount = item.AMOUNT;
                    serviceReqDetail.ServiceId = item.SERVICE_ID;
                    serviceReqDetail.RoomId = (long)cboRoom.EditValue;
                    serviceReqDetail.PatientTypeId = item.PATIENT_TYPE_ID;
                    serviceReqDetail.ParentId = item.PARENT_ID;
                    serviceReqDetail.SereServId = item.ID;
                    serviceReqDetail.EkipId = item.EKIP_ID;
                    serviceReqDetail.ShareCount = item.SHARE_COUNT;
                    serviceReqDetail.PrimaryPatientTypeId = item.PRIMARY_PATIENT_TYPE_ID;
                    serviceReqDetail.PackageId = item.PACKAGE_ID;
                    serviceReqDetail.UserPrice = item.USER_PRICE;
                    if (item.IsExpend)
                    {
                        serviceReqDetail.IsExpend = IS_TRUE;
                    }
                    if (item.IsOutKtcFee)
                    {
                        serviceReqDetail.IsOutParentFee = IS_TRUE;
                    }
                    serviceReqDetail.IsNotUseBhyt = item.IsNotUseBhyt;
                    serviceReqDetail.InstructionNote = item.Instruction_Note;
                    serviceReqDetail.AssignNumOrder = item.AssignNumOrder;
                    serviceReqUpdate.UpdateServices.Add(serviceReqDetail);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //- 0 (hoặc ko khai báo): Không thay đổi gì, giữ nguyên nghiệp vụ như hiện tại
        //- 1: Có kiểm tra dịch vụ đã kê có nằm trong danh sách đã được cấu hình tương ứng với ICD (căn cứ theo ICD_CODE và ICD_SUB_CODE) của bệnh nhân hay không. Nếu tồn tại dịch vụ không được cấu hình thì hiển thị thông báo và không cho lưu.
        //- 2: Có kiểm tra, nhưng chỉ hiển thị cảnh báo, và hỏi "Bạn có muốn tiếp tục không". Nếu người dùng chọn "OK" thì vẫn cho phép lưu
        private List<HIS_ICD> getIcdListFromUcIcd()
        {
            List<HIS_ICD> icdList = new List<HIS_ICD>();
            try
            {
                if (this.HisServiceReq != null && !string.IsNullOrEmpty(this.HisServiceReq.ICD_CODE))
                {
                    HIS_ICD icdMain = new HIS_ICD();
                    //icdMain.ID = this.HisServiceReq.ICD_ID ?? 0;
                    icdMain.ICD_NAME = this.HisServiceReq.ICD_NAME;
                    icdMain.ICD_CODE = this.HisServiceReq.ICD_CODE;
                    icdList.Add(icdMain);
                }

                if (this.HisServiceReq != null && !string.IsNullOrEmpty(this.HisServiceReq.ICD_SUB_CODE))
                {
                    string icd_sub_code = this.HisServiceReq.ICD_SUB_CODE;
                    if (!string.IsNullOrEmpty(icd_sub_code))
                    {
                        String[] icdCodes = icd_sub_code.Split(';');
                        foreach (var item in icdCodes)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var icd = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE == item);
                                if (icd != null)
                                {
                                    icdList.Add(icd);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                icdList = new List<HIS_ICD>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return icdList;
        }

        private bool CheckIcdService(ref string messageErr, List<HisSereServADO> ServiceCheckeds_Send, ref List<HisSereServADO> ServiceNotConfigResult, List<HIS_ICD> icdList)
        {
            bool valid = true;
            try
            {
                ServiceNotConfigResult = new List<HisSereServADO>();
                // kiểm tra dịch vụ theo cấu hình ICD - Dịch vụ

                if (icdList == null || icdList.Count == 0)
                {
                    valid = true;
                    return valid;
                }

                List<long> serviceIdChecks = ServiceCheckeds_Send.Select(o => o.SERVICE_ID).Distinct().ToList();

                if (serviceIdChecks == null || serviceIdChecks.Count == 0)
                {
                    valid = true;
                    return valid;
                }

                MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                icdServiceFilter.SERVICE_IDs = serviceIdChecks;
                icdServiceFilter.ICD_CODE__EXACTs = icdList.Select(o => o.ICD_CODE).Distinct().ToList();

                var IcdServices = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                if (ServiceCheckeds_Send != null && ServiceCheckeds_Send.Count > 0)
                {
                    foreach (var item in ServiceCheckeds_Send)
                    {
                        if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC)
                        {
                            continue;
                        }

                        var checkIcdService = IcdServices.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
                        if (checkIcdService == null)
                        {
                            valid = false;
                            ServiceNotConfigResult.Add(item);
                            messageErr += item.SERVICE_CODE + " - " + item.SERVICE_NAME + "; ";
                            Inventec.Common.Logging.LogSystem.Warn("Dich vu (" + item.SERVICE_CODE + "-" + item.SERVICE_NAME + " chua duoc cau hinh ICD - Dich vu.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckData(CommonParam param)
        {
            bool valid = true;
            try
            {
                var data = GridControlService.DataSource as List<ADO.HisSereServADO>;
                string errAmount = "";
                data = data.Where(o => o.IsChecked == true).ToList();
                if (data.Count == 0)
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Warn("Không có dịch vụ nào được chọn");
                }
                if (!valid)
                {
                    param.Messages.Add(ResourceMessage.KHONG_CHON_DICH_VU);
                }

                var amountZero = data.Where(o => o.AMOUNT <= 0);
                if (amountZero != null && amountZero.Count() > 0)
                {
                    valid = false;
                    errAmount = String.Join(", ", amountZero.Select(o => o.SERVICE_NAME).ToArray());
                }

                if (!valid)
                {
                    param.Messages.Add(String.Format("Dịch vụ có số lượng <=0: {0} ", errAmount));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool CheckPatientTypeValidation(List<MOS.SDO.ServiceReqDetailSDO> data, CommonParam param)
        {
            bool valid = true;
            try
            {
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        if (item.PatientTypeId == 0)
                        {
                            valid = false;
                            Inventec.Common.Logging.LogSystem.Warn("Dich vu (" + item.ServiceId + "-" + " khong co doi tuong thanh toan.");
                        }
                    }
                    if (!valid)
                    {
                        param.Messages.Add(ResourceMessage.KhongCoDoiTuongThanhToan);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckAmountValidation(List<MOS.SDO.ServiceReqDetailSDO> dataInserts, List<MOS.SDO.ServiceReqDetailSDO> dataUpdates, CommonParam param)
        {
            bool valid = true;
            try
            {
                if (dataInserts.Count > 0 || dataUpdates.Count() > 0)
                {
                    Dictionary<string, long?> dicErr = new Dictionary<string, long?>();

                    foreach (var item in dataInserts)
                    {
                        if (item.Amount <= 0)
                        {
                            param.Messages.Add(ResourceMessage.KhongCoSoLuong);
                            return false;
                        }
                        var service = lstService.FirstOrDefault(o => o.ID == item.ServiceId);
                        var y = sereServWithTreatment.Where(o => (o.IS_NO_EXECUTE == null || o.IS_NO_EXECUTE != 1) && (o.IS_DELETE == null || o.IS_DELETE != 1) && item.ServiceId == o.SERVICE_ID).Sum(o => o.AMOUNT);
                        if (service != null && service.MAX_AMOUNT != null && service.MAX_AMOUNT > 0 && y + item.Amount > service.MAX_AMOUNT)
                        {
                            if (!String.IsNullOrEmpty(service.SERVICE_NAME) && !dicErr.ContainsKey(service.SERVICE_NAME))
                            {
                                dicErr.Add(service.SERVICE_NAME, service.MAX_AMOUNT);
                            }
                        }
                    }
                    foreach (var item in dataUpdates)
                    {
                        var service = lstService.FirstOrDefault(o => o.ID == item.ServiceId);
                        var oldValue = listSereServCurrent.FirstOrDefault(o => o.ID == item.SereServId);
                        var y = sereServWithTreatment.Where(o => (o.IS_NO_EXECUTE == null || o.IS_NO_EXECUTE != 1) && (o.IS_DELETE == null || o.IS_DELETE != 1) && item.ServiceId == o.SERVICE_ID).Sum(o => o.AMOUNT);
                        if (service != null && service.MAX_AMOUNT != null && service.MAX_AMOUNT > 0 && oldValue != null && item.Amount > oldValue.AMOUNT)
                        {
                            if (y + item.Amount - oldValue.AMOUNT > service.MAX_AMOUNT && !String.IsNullOrEmpty(service.SERVICE_NAME) && !dicErr.ContainsKey(service.SERVICE_NAME))
                            {
                                dicErr.Add(service.SERVICE_NAME, service.MAX_AMOUNT);
                            }
                        }

                    }
                    if (dicErr.Count() > 0)
                    {
                        WaitingManager.Hide();
                        string msg = "";
                        var dicErrGroup = dicErr.GroupBy(o => o.Value);
                        foreach (var item in dicErrGroup)
                        {
                            msg += string.Format("Dịch vụ {0} vượt quá số lượng thực hiện cho phép({1}). ", string.Join(",", item.Select(o => o.Key).ToList()), item.First().Value);
                        }
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(msg + "Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool VerifyCheckFeeWhileAssign()
        {
            bool valid = true;
            try
            {
                List<ServiceReqDetailSDO> serviceReqDetails = new List<ServiceReqDetailSDO>();
                Inventec.Common.Logging.LogSystem.Debug("VerifyCheckFeeWhileAssign.1");
                this.patientTypeByPT = (currentHisPatientTypeAlter != null && currentHisPatientTypeAlter.PATIENT_TYPE_ID > 0) ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.ID == currentHisPatientTypeAlter.PATIENT_TYPE_ID).FirstOrDefault() : null;

                decimal totalPriceServiceSelected = 0;
                var lstdata = this.allSereServ.Where(o => o.IsChecked == true && o.IsExpend == false).ToList();
                if (lstdata != null && lstdata.Count > 0)
                    foreach (var item in lstdata)
                    {
                        if (item.SERVICE_ID > 0 && item.PATIENT_TYPE_ID > 0)
                        {
                            if (BranchDataWorker.DicServicePatyInBranch.ContainsKey(item.SERVICE_ID))
                            {
                                var data_ServicePrice = BranchDataWorker.ServicePatyWithPatientType(item.SERVICE_ID, item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                                if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                                {
                                    totalPriceServiceSelected += item.AMOUNT * (data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO));
                                }
                            }
                        }
                    }


                // - Trong trường hợp ĐỐI TƯỢNG BỆNH NHÂN được check "Không cho phép chỉ định dịch vụ nếu thiếu tiền" (HIS_PATIENT_TYPE có IS_CHECK_FEE_WHEN_ASSIGN = 1) và hồ sơ là "Khám" (HIS_TREATMENT có TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM) thì kiểm tra:
                //+ Nếu hồ sơ đang không thừa tiền "Còn thừa" = 0 hoặc hiển thị "Còn thiếu" thì hiển thị thông báo "Bệnh nhân đang nợ tiền, không cho phép chỉ định dịch vụ", người dùng nhấn "Đồng ý" thì tắt form chỉ định.
                //+ Nếu hồ sơ đang thừa tiền ("Còn thừa" > 0), thì khi người dùng check chọn dịch vụ, nếu số tiền "Phát sinh" > "Còn thừa" thì hiển thị cảnh báo: "Không cho phép chỉ định dịch vụ vượt quá số tiền còn thừa" và không cho phép người dùng check chọn dịch vụ đó.
                if (this.patientTypeByPT != null && this.patientTypeByPT.IS_CHECK_FEE_WHEN_ASSIGN == 1
                        && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                        &&
                        (
                        this.transferTreatmentFee >= 0 ||
                        (this.transferTreatmentFee < 0 && totalPriceServiceSelected > Math.Abs(this.transferTreatmentFee))
                        )
                    )
                {
                    //DialogResult myResult = MessageBox.Show(this, String.Format(ResourceMessage.BenhNhanDangNoTienKhogChoPhepChiDinhDV, Inventec.Common.Number.Convert.NumberToString(this.transferTreatmentFee, ConfigApplications.NumberSeperator)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    MessageBox.Show(this, String.Format(ResourceMessage.KhongChoPhepChiDInhDVVuotQuaSoTienCOnThua), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    Inventec.Common.Logging.LogSystem.Warn("co cau hinh IS_CHECK_FEE_WHEN_ASSIGN va ke don phong kham ==>" + ResourceMessage.KhongChoPhepChiDInhDVVuotQuaSoTienCOnThua);


                    //if (myResult == DialogResult.Yes)
                    //{

                    valid = false;
                    //}
                    Inventec.Common.Logging.LogSystem.Debug("VerifyCheckFeeWhileAssign.2");
                }
                Inventec.Common.Logging.LogSystem.Debug("VerifyCheckFeeWhileAssign.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        #endregion
        #endregion

        #region Shotcut
        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void cboRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeInstructionTimeInFirst)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                allSereServ = new List<ADO.HisSereServADO>();
                // lấy các service thuộc room đang chọn
                listSereServAdd = null;
                MOS.Filter.HisServiceRoomViewFilter serviceRoomViewFilter = new MOS.Filter.HisServiceRoomViewFilter();
                serviceRoomViewFilter.ROOM_ID = (long)cboRoom.EditValue;
                serviceRoomViewFilter.IS_LEAF = true;
                List<long> serviceTypeIds = Base.GlobalStore.MAPPING[this.HisServiceReq.SERVICE_REQ_TYPE_ID];
                serviceRoomViewFilter.SERVICE_TYPE_IDs = serviceTypeIds;
                var listServiceRooms = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_ROOM_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, serviceRoomViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (listServiceRooms != null && listServiceRooms.Count > 0 && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    listServiceRooms = listServiceRooms.Where(o => Base.GlobalStore.HisVServicePatys != null
                        && Base.GlobalStore.HisVServicePatys.Any(a =>
                            a.SERVICE_ID == o.SERVICE_ID
                            && (currentPatientTypeWithPatientTypeAlter.Exists(p => p.ID == a.PATIENT_TYPE_ID) || (a.INHERIT_PATIENT_TYPE_IDS != null && currentPatientTypeWithPatientTypeAlter.Exists(p => ("," + a.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + p.ID + ","))))
                            && a.IS_ACTIVE == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonNumberTrue
                            && a.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()
                            && ((!a.TREATMENT_TO_TIME.HasValue || a.TREATMENT_TO_TIME.Value >= currentHisTreatment.IN_TIME) || (!a.TO_TIME.HasValue || a.TO_TIME.Value >= instructionTime))
                        )).ToList();
                    listSereServAdd = new List<HisSereServADO>();
                    foreach (var item in listServiceRooms)
                    {
                        ADO.HisSereServADO serviceRoomSdo = new ADO.HisSereServADO();
                        var service = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID && o.IS_ACTIVE == 1);

                        long? exeServiceModuleId = null;
                        if (service != null)
                        {
                            serviceRoomSdo.IsAllowExpend = service.IS_ALLOW_EXPEND;

                            if (service.EXE_SERVICE_MODULE_ID.HasValue)
                            {
                                exeServiceModuleId = service.EXE_SERVICE_MODULE_ID.Value;
                            }
                            else if (service.SETY_EXE_SERVICE_MODULE_ID.HasValue)
                            {
                                exeServiceModuleId = service.SETY_EXE_SERVICE_MODULE_ID.Value;
                            }

                            if (!HisServiceReq.EXE_SERVICE_MODULE_ID.HasValue
                                || (exeServiceModuleId.HasValue && exeServiceModuleId == HisServiceReq.EXE_SERVICE_MODULE_ID))
                            {
                                serviceRoomSdo.SERVICE_ID = service.ID;
                                serviceRoomSdo.SERVICE_CODE = service.SERVICE_CODE;
                                serviceRoomSdo.SERVICE_NAME = service.SERVICE_NAME;
                                serviceRoomSdo.TDL_SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                                serviceRoomSdo.serviceType = Base.GlobalStore.SERVICE_ROOM_TYPE;
                                serviceRoomSdo.ExecuteRoomId = (long)cboRoom.EditValue;
                                serviceRoomSdo.isMulti = (item.IS_MULTI_REQUEST ?? 0) == IS_TRUE;
                                serviceRoomSdo.SERVICE_NAME_HIDDEN = convertToUnSign3(serviceRoomSdo.SERVICE_NAME) + serviceRoomSdo.SERVICE_NAME;
                                serviceRoomSdo.SERVICE_CODE_HIDDEN = convertToUnSign3(serviceRoomSdo.SERVICE_CODE) + serviceRoomSdo.SERVICE_CODE;
                                serviceRoomSdo.BILL_PATIENT_TYPE_ID = service.BILL_PATIENT_TYPE_ID;
                                listSereServAdd.Add(serviceRoomSdo);
                            }
                        }
                    }
                }
                if (listSereServAdd != null && listSereServAdd.Count > 0)
                    allSereServ.AddRange(listSereServAdd);

                foreach (var sereServ in allSereServ)
                {
                    var current = listSereServCurrent.FirstOrDefault(o => o.SERVICE_ID == sereServ.SERVICE_ID);
                    if (current != null) // load doi tuong da chon
                    {
                        sereServ.ID = current.ID;
                        sereServ.IsChecked = true;
                        sereServ.AMOUNT = current.AMOUNT;
                        sereServ.IsNotSetPrimaryPatientTypeId = current.IsNotSetPrimaryPatientTypeId;
                        sereServ.PRICE = current.PRICE;
                        sereServ.Instruction_Note = current.Instruction_Note;
                        sereServ.PATIENT_TYPE_ID = current.PATIENT_TYPE_ID;
                        sereServ.PRIMARY_PATIENT_TYPE_ID = current.PRIMARY_PATIENT_TYPE_ID;
                        sereServ.TDL_INTRUCTION_TIME = current.TDL_INTRUCTION_TIME;
                        sereServ.serviceType = current.serviceType;
                        sereServ.USER_PRICE = current.USER_PRICE;
                        sereServ.PACKAGE_NAME = current.PACKAGE_NAME;
                        sereServ.IS_NOT_FIXED_SERVICE = current.IS_NOT_FIXED_SERVICE;
                        sereServ.PACKAGE_ID = current.PACKAGE_ID;
                        if (current.IS_EXPEND == IS_TRUE)
                        {
                            sereServ.IsExpend = true;
                        }
                        if (current.IS_NOT_USE_BHYT == IS_TRUE)
                        {
                            sereServ.IsNotUseBhyt = true;
                        }
                        if (current.IS_OUT_PARENT_FEE == IS_TRUE)
                        {
                            sereServ.IsOutKtcFee = true;
                        }

                        ValidServiceDetailProcessing(current);
                    }

                    long time = this.instructionTime > 0 ? this.instructionTime : Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd"));

                    var sereServReplate = this.sereServWithTreatment.Where(o => o.SERVICE_ID == sereServ.SERVICE_ID && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == time.ToString().Substring(0, 8)).ToList();
                    if (sereServReplate != null && sereServReplate.Count > 0)
                    {
                        sereServ.IsAssignDay = true;
                    }
                }
                WaitingManager.Hide();
                GridControlService.BeginUpdate();
                GridControlService.DataSource = null;
                allSereServ = (allSereServ != null && allSereServ.Count > 0) ? allSereServ.OrderByDescending(o => o.IsChecked).ThenBy(o => o.SERVICE_NAME).ToList() : allSereServ;
                if (toggleSwitch.IsOn)
                {
                    var data = (allSereServ != null && allSereServ.Count > 0) ? allSereServ.Where(o => o.IsChecked == true).ToList() : null;
                    GridControlService.DataSource = data;
                }
                else
                {
                    GridControlService.DataSource = allSereServ;
                }

                GridControlService.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void barBtnSaveAndPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSaveAndPrint.Enabled && btnSaveAndPrint.Visible)
                {
                    SaveProcess(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcess(bool isPrintNow)
        {
            try
            {
                GridViewService.PostEditor();
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                MOS.SDO.HisServiceReqUpdateSDO serviceReqUpdate = new MOS.SDO.HisServiceReqUpdateSDO();
                UpdataDataForProcess(serviceReqUpdate);
                bool valid = true;
                this.positionHandleControl = -1;
                valid = valid && dxValidationProvider1.Validate();
                valid = valid && CheckService(serviceReqUpdate);
                valid = valid && CheckData(param);
                valid = valid && CheckPatientTypeValidation(serviceReqUpdate.InsertServices, param);
                valid = valid && CheckAmountValidation(serviceReqUpdate.InsertServices, serviceReqUpdate.UpdateServices, param);
                valid = valid && VerifyCheckFeeWhileAssign();
                if (!valid) return;

                // check theo cấu hình ICD - dịch vụ
                string serviceErrStr = "";
                string icdServiceCFG = HisConfigCFG.IcdServiceHasCheck;
                List<HisSereServADO> sereServAdoResult = new List<HisSereServADO>();
                List<HIS_ICD> icdFromUc = new List<HIS_ICD>();
                icdFromUc = getIcdListFromUcIcd();
                bool checkServiceIcd = this.CheckIcdService(ref serviceErrStr, this.SereServAdditonSdos, ref sereServAdoResult, icdFromUc);
                if (icdServiceCFG == "1" && !checkServiceIcd)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ResourceMessage.DichVuChuaDuocCauHinhICDDichVu + serviceErrStr);
                    WaitingManager.Hide();
                    MessageManager.Show(ResourceMessage.DichVuChuaDuocCauHinhICDDichVu + serviceErrStr);
                    return;
                }
                else if (icdServiceCFG == "2" && !checkServiceIcd)
                {
                    WaitingManager.Hide();
                    frmWaringConfigIcdService frmWaringConfigIcdService = new frmWaringConfigIcdService(icdFromUc, sereServAdoResult, this.currentModule, getDataFromOtherFormDelegate);
                    frmWaringConfigIcdService.ShowDialog();
                    if (!isYes)
                        return;
                }
                List<HIS_SERE_SERV> sereServMinDurations = getSereServWithMinDuration(this.SereServAdditonSdos);
                if (sereServMinDurations != null && sereServMinDurations.Count > 0)
                {
                    string sereServMinDurationStr = "";
                    foreach (var item in sereServMinDurations)
                    {
                        sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                    }

                    WaitingManager.Hide();
                    if (HisConfigCFG.IsSereServMinDurationAlert)
                    {
                        if (MessageBox.Show(string.Format(ResourceMessage.SereServMinDurationAlert__BanCoMuonChuyenDoiDTTTSangVienPhi, sereServMinDurationStr), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            foreach (var sv in serviceReqUpdate.InsertServices)
                            {
                                //Thực hiện tự động chuyển đổi đối tượng sang viện phí
                                if (sereServMinDurations.Any(o => o.SERVICE_ID == sv.ServiceId))
                                {
                                    sv.PatientTypeId = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;
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
                        if (MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                    }
                }

                Inventec.Common.Logging.LogSystem.Warn("Dữ liệu đầu vào sửa chỉ định: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqUpdate), serviceReqUpdate));

                var apiresult = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.SDO.HisServiceReqUpdateResultSDO>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, serviceReqUpdate, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);


                if (apiresult != null)
                {
                    success = true;
                    this.RefeshReference();
                    InitComboExecuteRoom();
                    this.Close();
                    if (isPrintNow)
                    {
                        CommonParam paramPrint = new CommonParam();
                        List<V_HIS_BED_LOG> bedLogs = new List<V_HIS_BED_LOG>();
                        // get bedLog
                        HisServiceReqListResultSDO serviceReqComboResultSDO = new HisServiceReqListResultSDO();
                        if (apiresult.ServiceReq != null)
                        {
                            serviceReqComboResultSDO.ServiceReqs = new List<V_HIS_SERVICE_REQ>();
                            serviceReqComboResultSDO.ServiceReqs.Add(apiresult.ServiceReq);
                        }

                        if (apiresult.SereServs != null && apiresult.SereServs.Count > 0)
                        {
                            serviceReqComboResultSDO.SereServs = new List<V_HIS_SERE_SERV>();
                            serviceReqComboResultSDO.SereServs.AddRange(apiresult.SereServs);
                        }

                        if (this.currentHisTreatment != null && serviceReqComboResultSDO != null && serviceReqComboResultSDO.ServiceReqs != null && serviceReqComboResultSDO.ServiceReqs.Count > 0)
                        {

                            MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                            bedLogViewFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                            bedLogViewFilter.DEPARTMENT_IDs = serviceReqComboResultSDO.ServiceReqs.Select(o => o.REQUEST_DEPARTMENT_ID).Distinct().ToList();
                            bedLogs = new Inventec.Common.Adapter.BackendAdapter(paramPrint).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, paramPrint);
                        }
                        var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, bedLogs, currentModule != null ? currentModule.RoomId : 0, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow);
                        PrintServiceReqProcessor.SaveNPrint(false);
                    }

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                }
                else
                {
                    MessageManager.Show(this, param, false);
                }

                WaitingManager.Hide();

               

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckService(HisServiceReqUpdateSDO serviceReqUpdate)
        {
            bool valid = true;
            try
            {
                if (serviceReqUpdate != null)
                {
                    var lstServiceIds = serviceReqUpdate.InsertServices.Select(o => o.ServiceId).ToList();
                    if (lstServiceIds == null)
                        lstServiceIds = new List<long>();
                    lstServiceIds.AddRange(serviceReqUpdate.UpdateServices.Select(o => o.ServiceId).ToList());
                    lstServiceIds = lstServiceIds.Distinct().ToList();
                    if (lstServiceIds != null && lstServiceIds.Count > 0)
                    {
                        string Message = null;
                        string MessageGender = null;
                        string gender = null;

                        List<string> lstServiceName = new List<string>();
                        foreach (var item in lstServiceIds)
                        {
                            var service = lstService.FirstOrDefault(o => o.ID == item);
                            if (service != null && service.GENDER_ID != null && service.GENDER_ID != currentHisTreatment.TDL_PATIENT_GENDER_ID)
                            {

                                var genders = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>();
                                gender = genders.FirstOrDefault(o => o.ID == currentHisTreatment.TDL_PATIENT_GENDER_ID).GENDER_NAME;
                                lstServiceName.Add(service.SERVICE_NAME);
                            }

                            // check tuổi từ - đến (DVKT)
                            var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentHisTreatment.TDL_PATIENT_DOB);

                            int ageMonth = (DateTime.Now - (ageDate ?? DateTime.Now)).Days / 30;
                            //Inventec.Common.Logging.LogSystem.Debug("age: " + age);
                            string ageType = null;
                            long? age = 0;
                            if ((service.AGE_FROM != null && service.AGE_FROM > ageMonth) || (service.AGE_TO != null && service.AGE_TO < ageMonth))
                            {
                                if (service.AGE_FROM != null && service.AGE_TO == null)
                                {
                                    if (service.AGE_FROM < 72) { age = service.AGE_FROM; ageType = "tháng tuổi"; }
                                    else if (service.AGE_FROM >= 72) { age = service.AGE_FROM / 12; ageType = "tuổi"; }

                                    Message += "Dịch vụ " + service.SERVICE_NAME + " chỉ cho phép chỉ định với bệnh nhân từ " + age + " " + ageType + "\r\n";
                                }
                                else if (service.AGE_TO != null && service.AGE_FROM == null)
                                {
                                    if (service.AGE_TO < 72) { age = service.AGE_TO; ageType = "tháng tuổi"; }
                                    else if (service.AGE_TO >= 72) { age = service.AGE_TO / 12; ageType = "tuổi"; }

                                    Message += "Dịch vụ " + service.SERVICE_NAME + " chỉ cho phép chỉ định với bệnh nhân dưới " + age + " " + ageType + "\r\n";
                                }
                                else if (service.AGE_TO != null && service.AGE_FROM != null)
                                {
                                    string ageType2 = null;
                                    long? age2 = 0;
                                    if (service.AGE_FROM < 72) { age = service.AGE_FROM; ageType = "tháng tuổi"; }
                                    else if (service.AGE_FROM >= 72) { age = service.AGE_FROM / 12; ageType = "tuổi"; }

                                    if (service.AGE_TO < 72) { age2 = service.AGE_TO; ageType2 = "tháng tuổi"; }
                                    else if (service.AGE_TO >= 72) { age2 = service.AGE_TO / 12; ageType2 = "tuổi"; }

                                    Message += "Dịch vụ " + service.SERVICE_NAME + " chỉ cho phép chỉ định với bệnh nhân từ " + age + " " + ageType + " đến " + age2 + " " + ageType2 + "\r\n";
                                }
                            }
                        }


                        if (lstServiceName != null && lstServiceName.Count > 0)
                        {
                            WaitingManager.Hide();
                            MessageGender += "Dịch vụ " + String.Join(", ", lstServiceName) + " không cho phép chỉ định đối với bệnh nhân giới tính " + gender + "\r\n";
                            XtraMessageBox.Show(MessageGender, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                            return valid = false;
                        }

                        if (!string.IsNullOrEmpty(Message))
                        {
                            WaitingManager.Hide();
                            XtraMessageBox.Show(Message, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                            return valid = false;
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

        private void CheckOverTotalPatientPrice()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFeeViewFilter hisTreatmentFeeViewFilter = new HisTreatmentFeeViewFilter();
                hisTreatmentFeeViewFilter.IS_ACTIVE = 1;
                hisTreatmentFeeViewFilter.ID = this.currentHisTreatment.ID;
                Inventec.Common.Logging.LogSystem.Debug("begin call HisTreatment/GetFeeView");
                var treatmentFees = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFeeViewFilter, param);
                if (treatmentFees != null && treatmentFees.Count > 0)
                {
                    var treatmentFee = treatmentFees.FirstOrDefault();
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

                    this.patientTypeByPT = (currentHisPatientTypeAlter != null && currentHisPatientTypeAlter.PATIENT_TYPE_ID > 0) ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.ID == currentHisPatientTypeAlter.PATIENT_TYPE_ID).FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void GridViewService_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var row = (ADO.HisSereServADO)GridViewService.GetFocusedRow();
                if (row != null)
                {
                    if(e.Column.FieldName == this.gc_Check.FieldName)
                    {
                        this.SetAssignNumOrder(row);
                    }    
                    if (e.Column.FieldName == this.gc_Check.FieldName
                        || e.Column.FieldName == this.Gc_PatientTypeName.FieldName
                        )
                    {
                        if (row.IsChecked)
                        {
                            ChoosePatientTypeDefaultlService(row.PATIENT_TYPE_ID, row.SERVICE_ID, row);
                        }
                        this.GridControlService.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtInstructionTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this.isNotLoadWhileChangeInstructionTimeInFirst)
                {
                    this.ChangeIntructionTimeEditor(this.dtInstructionTime.DateTime);
                }
                if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign)
                {
                    CheckTimeInDepartment(this.intructionTimeSelecteds);
                }
                if (dtInstructionTime.EditValue != null) CheckTimeSereSev();
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void CheckTimeSereSev()
        {
            try
            {
                var config = BackendDataWorker.Get<HIS_CONFIG>().Where(s => s.KEY == "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION").FirstOrDefault();
                CommonParam param = new CommonParam();
                if (config != null)
                {
                    if (config.VALUE == "1" || config.VALUE == "2")
                    {
                        HisServiceReqCheckSereTimesSDO sdo = new HisServiceReqCheckSereTimesSDO();
                        sdo.TreatmentId = HisServiceReq.TREATMENT_ID;
                        var username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        sdo.Loginnames = new List<string>() { username };
                        long sereTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.DateTime) ?? 0;
                        sdo.SereTimes = new List<long> { sereTime };
                        Inventec.Common.Logging.LogSystem.Debug("HisServiceReqCheckSereTimesSDO:" + LogUtil.TraceData("HisServiceReqCheckSereTimesSDO", sdo));
                        bool rs = new BackendAdapter(param).Post<bool>("api/HisServiceReq/CheckSereTimes", ApiConsumers.MosConsumer, sdo, param);
                        if (!rs)
                        {
                            if (config.VALUE == "1")
                            {
                                MessageManager.Show(this, param, rs);
                                btnSave.Enabled = btnSaveAndPrint.Enabled = false;
                            }
                            else
                            {
                                btnSave.Enabled = btnSaveAndPrint.Enabled = MessageBox.Show(this, param.GetMessage() + "Bạn có muốn tiếp tục?", "Thông Báo", MessageBoxButtons.YesNo) == DialogResult.Yes;

                            }
                        }
                        else btnSave.Enabled = btnSaveAndPrint.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ChangeIntructionTimeEditor(DateTime intructTime)
        {
            try
            {
                instructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(intructTime.ToString("yyyyMMddHHmm00"));
                this.intructionTimeSelecteds = new List<long>();
                this.intructionTimeSelecteds.Add(instructionTime);
                this.ChangeIntructionTime();

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
        private void ProcessGetDataDepartment()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessGetDataDepartment.Begin");
                CommonParam paramGet = new CommonParam();
                if (this.ListDepartmentTranCheckTime == null)
                {
                    HisDepartmentTranFilter filter = new HisDepartmentTranFilter();
                    filter.TREATMENT_ID = this.HisServiceReq.TREATMENT_ID;
                    this.ListDepartmentTranCheckTime = new BackendAdapter(paramGet).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                }

                if (this.ListCoTreatmentCheckTime == null)
                {
                    HisCoTreatmentFilter filter = new HisCoTreatmentFilter();
                    filter.TDL_TREATMENT_ID = this.HisServiceReq.TREATMENT_ID;
                    this.ListCoTreatmentCheckTime = new BackendAdapter(paramGet).Get<List<HIS_CO_TREATMENT>>("api/HisCoTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                }

                Inventec.Common.Logging.LogSystem.Debug("ProcessGetDataDepartment.End");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private bool CheckTimeInDepartment(List<long> listTime)
        {
            bool result = true;
            try
            {
                V_HIS_ROOM currentWorkingRoom = null;
                currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == this.currentModule.RoomId);

                List<HIS_DEPARTMENT_TRAN> curremtTrans = null;
                if (this.ListDepartmentTranCheckTime != null && this.ListDepartmentTranCheckTime.Count > 0)
                {
                    curremtTrans = this.ListDepartmentTranCheckTime.Where(o => o.DEPARTMENT_ID == currentWorkingRoom.DEPARTMENT_ID && o.DEPARTMENT_IN_TIME.HasValue).ToList();
                }

                List<HIS_CO_TREATMENT> currentCo = null;
                if (this.ListCoTreatmentCheckTime != null && this.ListCoTreatmentCheckTime.Count > 0)
                {
                    currentCo = this.ListCoTreatmentCheckTime.Where(o => o.DEPARTMENT_ID == currentWorkingRoom.DEPARTMENT_ID && o.START_TIME.HasValue).ToList();
                }

                foreach (var intructionTime in listTime)
                {
                    bool hasTran = false;

                    List<string> times = new List<string>();
                    if (curremtTrans != null && curremtTrans.Count > 0)
                    {
                        curremtTrans = curremtTrans.OrderBy(o => o.DEPARTMENT_IN_TIME ?? 0).ToList();

                        long fromTime = 0;
                        long toTime = 0;

                        foreach (var item in curremtTrans)
                        {
                            fromTime = item.DEPARTMENT_IN_TIME ?? 0;
                            toTime = long.MaxValue;
                            HIS_DEPARTMENT_TRAN nextTran = this.ListDepartmentTranCheckTime.FirstOrDefault(o => o.PREVIOUS_ID == item.ID);
                            if (nextTran != null)
                            {
                                toTime = nextTran.DEPARTMENT_IN_TIME ?? long.MaxValue;
                            }

                            hasTran = hasTran || (fromTime <= intructionTime && intructionTime <= toTime);

                            times.Add(string.Format("từ {0}{1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(fromTime),
                            (toTime > 0 && toTime != long.MaxValue) ? " đến " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(toTime) : ""));
                        }
                    }

                    if (!hasTran && times.Count > 0 && currentCo != null && currentCo.Count > 0)
                    {
                        times.Clear();
                    }

                    if (!hasTran && currentCo != null && currentCo.Count > 0)
                    {
                        currentCo = currentCo.OrderBy(o => o.START_TIME ?? 0).ToList();

                        long fromTime = 0;
                        long toTime = 0;

                        foreach (var item in currentCo)
                        {
                            fromTime = item.START_TIME ?? 0;
                            toTime = item.FINISH_TIME ?? long.MaxValue;

                            hasTran = hasTran || (fromTime <= intructionTime && intructionTime <= toTime);

                            times.Add(string.Format("từ {0}{1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(fromTime),
                            (toTime > 0 && toTime != long.MaxValue) ? " đến " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(toTime) : ""));
                        }
                    }

                    if (!hasTran)
                    {
                        XtraMessageBox.Show(string.Format(ResourceMessage.ThoiGianYLenhKhongThuocKhoangThoiGianTrongKhoa,
                           string.Join(",", times)), "Thông báo");
                        this.isNotLoadWhileChangeInstructionTimeInFirst = true;
                        this.dtInstructionTime.Focus();
                        this.isNotLoadWhileChangeInstructionTimeInFirst = false;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ChangeIntructionTime()
        {
            try
            {
                if (HisConfigCFG.IsUsingServerTime == "1"
                    && this.currentHisTreatment != null)
                {
                    return;
                }
                LogSystem.Debug("ChangeIntructionTime => 1");
                LoadCurrentTreatment(HisServiceReq.TREATMENT_ID, instructionTime);
                LoadDataSereServWithTreatment(this.HisServiceReq);
                PatientTypeWithPatientTypeAlter();
                FillDataToGrid();
                InitComboExecuteRoom();
                CheckOverTotalPatientPrice();
                LogSystem.Debug("ChangeIntructionTime => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UcDateInit()
        {
            try
            {
                this.ValidationSingleControl(this.dtInstructionTime, this.dxValidationProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

        private void GridControlService_Click(object sender, EventArgs e)
        {

        }
    }
}

